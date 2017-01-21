using System;
using System.Collections.Generic;
using TUI_Web.Settings;
using TUIO;

namespace TUI_Web.Data
{
	class DataControler
	{
        #region VARIABLES
        public event EventHandler<List<GridRow>> EVENT_dataUpdated;
		private List<GridRow> rows;
		private Dictionary<int, CursorElement> cursorElements;
        private Dictionary<int, myTimer> timer;

        private object lockObject = new object();
        #endregion

        #region CONSTRUCTOR
        public DataControler(List<GridRow> rows = null)
		{
			if (rows == null)
			{
				for (int i = 0; i < SettingsControler.LINES_DISPLAYED; i++)
				{
					rows.Add(new GridRow());
				}
			}
			else
			{
				this.rows = rows;
			}

			cursorElements = new Dictionary<int, CursorElement>();
            timer = new Dictionary<int, myTimer>();
		}
        #endregion

        public List<GridRow> getData()
		{
			return this.rows;
		}

        public void save(object sender = null, object e = null)
        {
            for (int i = 0; i < SettingsControler.LINES_DISPLAYED; i++)
            {
                for (int j = 0; j < SettingsControler.GRID_ELEMENTS; j++)
                {
                    GridElement currentElement = rows[i].elements[j];

                    if (currentElement.getOverlay() != null)
                    {
                        currentElement = currentElement.getOverlay();
                        currentElement.setOverlay(null);
                    }
                }
            }
            cursorElements.Clear();
        }

        #region private
        private CursorElement createNewElement(TUIO.TuioObject obj)
		{
			CursorElement cursor = new CursorElement();
            cursor.EVENT_SizeChanged += Cursor_EVENT_SizeChanged;
            cursor.EVENT_PositionChanged += Cursor_EVENT_PositionChanged;

            cursor.setAngle(obj.AngleDegrees);
			try
			{
				cursorElements.Add(obj.SymbolID, cursor);
			}
			catch (Exception e)
			{
				Console.WriteLine("createNewElement: " + e.Message);
			}

			return cursor;
		}

        private CursorElement getExistingElement(TUIO.TuioObject obj) 
		{
			CursorElement cursor = default(CursorElement);
			cursorElements.TryGetValue(obj.SymbolID, out cursor);
			return cursor;
		}

		private void setOverlayElement(CursorElement cursor)
		{
			getElement(cursor.getRow(), cursor.getCell()).setOverlay(cursor.getElement());
		}

		private void removeOverlayElement(CursorElement cursor)
		{
			getElement(cursor.getRow(), cursor.getCell()).setOverlay(null);
		}

        private void setRealElement(CursorElement cursor)
        {
			rows[cursor.getRow()].elements[cursor.getCell()] = cursor.getElement();
        }

		private ElementTypes getType(TUIO.TuioObject obj)
		{
			ElementTypes type = ElementTypes.None;
			switch (obj.SymbolID)
			{
				case 0:
					type = ElementTypes.Graphic;
					break;

				case 1:
					type = ElementTypes.Text;
					break;

				case 2:
					type = ElementTypes.Topic;
					break;
			}

			return type;
		}

        // calculates the new size of the given element 
        // the calculation only will be provided if:
        // -> the current size is not 0 -> this elements will not be in-, decreased anymore!
        // -> the new size will not overstep the max. size
        // -> the new size will not fall below to the min. size
        private bool reCalculateSize(OverlayElement element, int sizeChanged)
        {
            int newSize = element.size + sizeChanged;
            if (element.size != 0 &&
                newSize >= SettingsControler.MINIMUN_ELEMENT_SIZE &&
                newSize <= SettingsControler.MAXIMUM_ELEMENT_SIZE)
                element.size += sizeChanged;

            if (element.size == 0)
                return true;

            return false;
        }

        private GridElement getElement(int row, int cell)
        {
            return rows[row].elements[cell];
        }

        private void removeObject(TuioObject removeableObj)
        {
            try
            {
                CursorElement cursor = getExistingElement(removeableObj);
                removeOverlayElement(cursor);
                cursorElements.Remove(removeableObj.SymbolID);
                //Console.WriteLine("remove object -->" + removeableObj.SymbolID);
                EVENT_dataUpdated?.Invoke(this, rows);
            }
            catch (Exception e)
            {
                Console.WriteLine("removeObject " + e.Message);
            }
        }
        #endregion

        #region INPUT_EVENTS
        public void InputListener_EVENT_updateObject(object sender, TUIO.TuioObject obj)
        {
            lock (lockObject)
            {
                try
                {
                    CursorElement cursor = getExistingElement(obj);

                    // TODO: StartTransaction -> if error, reset states

                    // calculate cursor Position
                    cursor.writeCursorPosition(obj, rows);

                    // if there is already a element -> stop
                    //if (rows[cursor.getRow()].elementExists(cursor))
                    //{
                    //	return;
                    //}

                    // write new size of the cursor-element, may it has been changed
                    cursor.writeCursorSize(obj, rows[cursor.getRow()]);

                    // new Element is finished, set it to the grid!
                    setOverlayElement(cursor);


                    //Console.Write("update object( " + obj.SymbolID + " )\r\n" );
                    int counter = 0;
                    /*
                    foreach (GridElement element in rows[cursor.getRow()].elements)
                    {
                        if (element.getOverlay() != null)
                            Console.Write("[" + cursor.getRow() + ";" + counter + "]:" + element.getOverlay().size + "\r\n ");
                        else
                            Console.Write("[" + cursor.getRow() + ";" + counter + "]:" + element.size + "\r\n ");

                        counter++;
                    }

                    int i = 0;
                    foreach (GridRow row in rows)
                    {
                        Console.WriteLine("R[" + i + "] aff:" + row.getAffectedCount());
                        i++;
                    }
                    */

                    EVENT_dataUpdated?.Invoke(this, rows);
                }
                catch (Exception)
                {
                    // object couldn´t be updated. 
                    // cause the cursorlist is cleared after saving current page.
                    // in this case we create new objects instead of searching for existing ones.
                    InputListener_EVENT_newObject(sender, obj);
                }
                
            }
        }

        public void InputListener_EVENT_removeObject(object sender, TuioObject obj)
        {
            lock (lockObject)
            {
                if (timer.ContainsKey(obj.SymbolID))
                {
                    // timer is still running? 
                    // proceed
                    return;
                }
                else
                {
                    myTimer myTimer = new myTimer();
                    timer.Add(obj.SymbolID, myTimer);
                    myTimer.EVENT_TimeOut += MyTimer_EVENT_TimeOut;

                    Console.WriteLine("[{0}]START: {1}", obj.SymbolID, DateTime.Now.ToString("h:mm:ss.fff"));
                    myTimer.startTimer(obj);
                }
            }
            /*

            if (!trigger)
                return;

            trigger = false;


			try
			{
                CursorElement cursor = getExistingElement(obj);
                removeOverlayElement(cursor);
				cursorElements.Remove(obj.SymbolID);
				Console.WriteLine("remove object -->" + obj.SymbolID);
				EVENT_dataUpdated?.Invoke(this, rows);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}

            trigger = true;
            */
        }

        public void InputListener_EVENT_newObject(object sender, TuioObject obj)
        {
            lock (lockObject)
            {
                if (timer.ContainsKey(obj.SymbolID))
                {
                    myTimer objTimer = null;
                    timer.TryGetValue(obj.SymbolID, out objTimer);
                    objTimer.stopTimer();
                    Console.WriteLine("[{0}]CANCEL: {1}", obj.SymbolID ,  DateTime.Now.ToString("h:mm:ss.fff"));
                    timer.Remove(obj.SymbolID);
                }
                else
                {
                    if (!cursorElements.ContainsKey(obj.SymbolID))
                    {
                        try
                        {
                            CursorElement cursor = createNewElement(obj);

                            cursor.writeCursorPosition(obj, rows);
                            cursor.getElement().type = getType(obj);

                            setOverlayElement(cursor);

                            Console.WriteLine("new object -->" + obj.SymbolID);
                            EVENT_dataUpdated?.Invoke(this, rows);
                        }
                        catch (Exception e) { Console.WriteLine("InputListener_EVENT_newObject: " + e.Message); }
                    }
                    else return;
                }
            }

            /*
            if (!trigger)
                return;

            trigger = false;

    */


            //trigger = true;
        }
        #endregion

        #region CURSOR_EVENTS
        private void Cursor_EVENT_SizeChanged(object sender, CursorEventSizeArgs cursorArgs)
        {
            CursorElement cursor = (CursorElement)sender;
            GridRow affectedRow = rows[cursor.getRow()];

            // say the row, that there is a size-manipulation!
            // only once per cursor.element
            if (!cursor.affected)
            {
                affectedRow.increaseSizeAffected();
                cursor.affected = true;

                if (affectedRow.getAffected())
                {
                    foreach (KeyValuePair<int, CursorElement> myCursors in cursorElements)
                    {
                        if (myCursors.Value == cursor)
                            continue;

                        if (myCursors.Value.getRow() == cursor.getRow())
                        {
                            if (!myCursors.Value.affected)
                            {
                                myCursors.Value.affected = true;
                                affectedRow.increaseSizeAffected();
                            }
                        }
                    }
                }
            }

            if ((cursorArgs.changeType == SizeChangingType.DecreaseOther) ||
                (cursorArgs.changeType == SizeChangingType.IncreaseOther))
            {
                int counter = 0;
                foreach (GridElement element in rows[cursor.getRow()].elements)
                {
                    if (element == affectedRow.elements[cursor.getCell()])
                        continue;

                    reCalculateSize(element.getOverlay(), cursorArgs.changeSteps);
                    counter++;

                    if (counter == affectedRow.elementCount)
                        break;
                }
            }
            else if (cursorArgs.changeType == SizeChangingType.RemoveLast)
            {
                GridElement element = rows[cursor.getRow()].getSmallestElement();
                if (element.getOverlay() != null)
                    element.getOverlay().size = 0;
                else
                {
                    GridElement overlay = new GridElement();
                    overlay.size = 0;
                    overlay.type = element.type;
                    element = overlay;
                }

                rows[cursor.getRow()].elementCount--;
            }
        }

        private void Cursor_EVENT_PositionChanged(object sender, CursorEventPositionArgs posArgs)
        {
            CursorElement cursor = (CursorElement)sender;
            OverlayElement affectedElement = getElement(cursor.getRow(), cursor.getCell()).getOverlay();


            // if the element was moved from a affected row
            // tell the row, that there is an element less
            // if there are no affected elements on the row, it will reset itself
            if ((affectedElement != null && affectedElement.size != SettingsControler.DEFAULT_ELEMENT_SIZE) ||
               (cursor.getElement().size != SettingsControler.DEFAULT_ELEMENT_SIZE))
            {
                if (cursor.affected)
                {
                    if (posArgs.oldPosition.row != posArgs.newPosition.row)
                    {
                        rows[posArgs.oldPosition.row].decreaseSizeAffected();
                        if (rows[posArgs.newPosition.row].getAffected())
                        {
                            rows[posArgs.newPosition.row].increaseSizeAffected();
                        }
                        else
                        {
                            cursor.affected = false;
                        }
                    }
                }
                else
                {
                    if (rows[posArgs.newPosition.row].getAffected())
                    {
                        rows[posArgs.newPosition.row].increaseSizeAffected();
                        cursor.affected = true;
                    }
                }
            }


            // old cursor-element have to save their size, before the cursor is moved!
            OverlayElement oldElement = null;
            if (posArgs.oldPosition.cell >= 0 && posArgs.oldPosition.row >= 0)
            {
                if (rows[posArgs.oldPosition.row].getAffected())
                {
                    oldElement = new OverlayElement();
                    oldElement.size = cursor.getElement().size;
                    oldElement.type = ElementTypes.None;
                    oldElement.cursor = false;
                    getElement(posArgs.oldPosition.row, posArgs.oldPosition.cell).setOverlay(oldElement);
                }
                else
                {
                    getElement(posArgs.oldPosition.row, posArgs.oldPosition.cell).setOverlay(null);
                }

            }

            // if on the new element a size-manipulation already happenened, we need to adopt it
            if (affectedElement != null)
            {
                // there could only be one element at the same time.
                if (affectedElement.type == ElementTypes.None)
                {
                    cursor.getElement().size = affectedElement.size;
                }
            }
            else
                cursor.getElement().size = SettingsControler.DEFAULT_ELEMENT_SIZE;

            affectedElement = cursor.getElement();
        }
        #endregion

        #region OTHER_EVENTS
        private void MyTimer_EVENT_TimeOut(object sender, TuioObject timeObject)
        {
            if (timer.ContainsKey(timeObject.SymbolID))
            {
                Console.WriteLine("[{0}]STOP: {1}", timeObject.SymbolID, DateTime.Now.ToString("h:mm:ss.fff"));
                removeObject(timeObject);
                timer.Remove(timeObject.SymbolID);
            }
        }
        #endregion

    }
}
