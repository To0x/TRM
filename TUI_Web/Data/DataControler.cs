using System;
using System.Collections.Generic;
using TUI_Web.Settings;

namespace TUI_Web.Data
{
	class DataControler
	{
		public event EventHandler<List<GridRow>> EVENT_dataUpdated;
		private List<GridRow> rows;
		private Dictionary<int, CursorElement> cursorElements;

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
		}

		public List<GridRow> getData()
		{
			return this.rows;
		}

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
				Console.WriteLine(e.Message);
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
			rows[cursor.getRow()].elements[cursor.getCell()].overlayElement = cursor.getElement();
		}

		private void removeOverlayElement(CursorElement cursor)
		{
			rows[cursor.getRow()].elements[cursor.getCell()].overlayElement = null;
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

        public void save(object sender = null, object e = null)
        {
            for (int i = 0; i < SettingsControler.LINES_DISPLAYED; i++)
            {
                for (int j = 0; j < SettingsControler.GRID_ELEMENTS; j++)
                {
                    GridElement currentElement = rows[i].elements[j];

                    if (currentElement.overlayElement != null)
                    {
                        currentElement = currentElement.overlayElement;
                        currentElement.overlayElement = null;
                    }
                }
            }

            /*
            foreach (KeyValuePair<int, CursorElement> cursor in cursorElements)
            {
                removeOverlayElement(cursor.Value);
                setRealElement(cursor.Value);
            }
            */

            cursorElements.Clear();
        }

        #region EVENTS
        public void InputListener_EVENT_updateObject(object sender, TUIO.TuioObject obj)
		{
			try
			{
				CursorElement cursor = getExistingElement(obj);

                // old element has chaned, remove it from grid!
				removeOverlayElement(cursor);

                // calculate cursor Position
                cursor.writeCursorPosition(obj);

                // write new size of the cursor-element, may it has been changed
                cursor.writeCursorSize(obj);
                //resetOverlaySize();

                // new Element is finished, set it to the grid!
                setOverlayElement(cursor);
                //calcElementSizesInRow(cursor);

                //calculateSize(obj, cursor);
                //calculateCursorSize(obj, cursor);
                //calculateNextElementSize(cursor);

                Console.WriteLine("update object( " + obj.SymbolID + " ) x=" + Math.Round(obj.Position.X,4) + ";y=" + Math.Round(obj.Position.Y,4) + ";element.size: " + cursor.getElement().size);
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

        private void Cursor_EVENT_SizeChanged2(object sender, int sizeChanged)
        {
            CursorElement cursor = (CursorElement)sender;
            GridRow affectedRow = rows[cursor.getRow()];

            int changedSteps = SettingsControler.DEFAULT_ELEMENT_SIZE - cursor.getElement().size;

            // if the cursor-element gets bigger, all others needs to be smaller (therfore the -1)
            int reducingSteps = sizeChanged / (affectedRow.elementCount - 1) * -1;

            foreach (GridElement element in affectedRow.elements)
            {
                if (element == affectedRow.elements[cursor.getCell()])
                    continue;

                reCalculateSize(element, reducingSteps);
            }

            //throw new NotImplementedException();
        }

        private void Cursor_EVENT_SizeChanged(object sender, int sizeChanged)
        {
            CursorElement cursor = (CursorElement)sender;
            GridRow affectedRow = rows[cursor.getRow()];

            int changedSteps = SettingsControler.DEFAULT_ELEMENT_SIZE - cursor.getElement().size;

            // if the cursor-element gets bigger, all others needs to be smaller (therfore the -1)
            int reducingSteps = sizeChanged / (affectedRow.elementCount - 1) * -1;

            foreach (GridElement element in affectedRow.elements)
            {
                if (element == affectedRow.elements[cursor.getCell()])
                    continue;

                if (reCalculateSize(element, reducingSteps))
                    affectedRow.elementCount--;
            }

            //throw new NotImplementedException();
        }


        private void Cursor_EVENT_PositionChanged(object sender, EventArgs e)
        {
            CursorElement cursor = (CursorElement)sender;
            GridElement affectedElement = rows[cursor.getRow()].elements[cursor.getCell()];

            if (affectedElement.overlayElement != null)
                cursor.getElement().size = affectedElement.overlayElement.size;

            affectedElement = cursor.getElement();
            //Cursor_EVENT_SizeChanged(sender, 0);
            //throw new NotImplementedException();
        }

        private bool reCalculateSize(GridElement element, int sizeChanged)
        {
            if (element.overlayElement == null)
            {
                element.overlayElement = new GridElement();
                element.overlayElement.type = element.type;
            }

            element.overlayElement.size = element.overlayElement.size + sizeChanged;

            if (element.overlayElement.size == 0)
                return true;

            return false;
        }

        /*
        private void resetOverlaySize()
        {
            foreach (GridRow row in rows)
            {
                foreach (GridElement element in row.elements)
                {
                    if (element.overlayElement != null)
                    {
                        element.overlayElement.size = SettingsControler.DEFAULT_ELEMENT_SIZE;
                    }
                }
            }
        }

        private void calcElementSizesInRow(CursorElement cursor)
        {
            GridRow row = rows[cursor.getRow()];
            int changedSteps = SettingsControler.DEFAULT_ELEMENT_SIZE - cursor.getElement().size;
            int reducingSteps = changedSteps / (row.elementCount - 1);

            //if (maxSize > )

            for (int i = cursor.getCell(); i < row.elementCount; i++)
            {

            }

            foreach (GridElement element in row.elements)
            {
                if (element.overlayElement == cursor.getElement())
                    continue;

                element.overlayElement.size += reducingSteps;
            }
        }
        */

        private GridElement getElement(int row, int cell)
        {
            return rows[row].elements[cell];
        }

        /* Next Element could also be the previous Element, if the cursor is the last element in current row.
        // if there is only 1 Element in the row, return null
        private GridElement getNextElement(CursorElement cursor)
        {
            GridElement nextElement = null;
            if (rows[cursor.getRow()].elementCount != 1)
            {
                if (cursor.getCell() < rows[cursor.getRow()].elementCount)
                    nextElement = rows[cursor.getRow()].elements[cursor.getCell() + 1];
                else
                    nextElement = rows[cursor.getRow()].elements[cursor.getCell() - 1];

                return nextElement;
            }
            return nextElement;
        }
        */

        /* calculte the size of the cursor
        // this will only change the variable cursor.size
        // no element.size will be changed and therefore nor the webpage
        private void calculateCursorSize(TUIO.TuioObject obj, CursorElement cursor)
        {
            // if there is only one element in the line we cannot adjust the size
            if (rows[cursor.getRow()].elementCount == 1)
                return;

            if (obj.AngleDegrees != cursor.getAngle())
            {
                // the angle of the object was changed 
                if (obj.AngleDegrees > cursor.getAngle() + SettingsControler.sizeStep())
                {
                    // the size can only increased if the following are true:
                    // 1. Maximum Size is not Reached yet
                    // 2. Minimum Size of the next Element is not reached yet.
                    if (cursor.size < SettingsControler.MAXIMUM_ELEMENT_SIZE - (SettingsControler.GRID_ELEMENTS - 1) * SettingsControler.MINIMUN_ELEMENT_SIZE
                        && getNextElement(cursor).size > SettingsControler.MINIMUN_ELEMENT_SIZE)
                    {
                        // maximum size not reached!
                        cursor.size++;
                        Console.WriteLine("Size++");
                    }
                    cursor.setAngle(obj.AngleDegrees);
                }
                else if (obj.AngleDegrees < cursor.getAngle() - SettingsControler.sizeStep())
                {
                    if (cursor.size > SettingsControler.MINIMUN_ELEMENT_SIZE)
                    {
                        // minimum size not reached!
                        cursor.size--;
                        Console.WriteLine("Size--");
                    }
                    cursor.setAngle(obj.AngleDegrees);
                }
            }
        }

        //!!! DO NOT USE
        // calculate the cursor size and also the size of the next element
        // both will directly parsed into the webPage
   
        // Problem:
        // if the element is moved to another position
        // the element.size also be moved, but the other elements not
        // this curses into an broken bootstrap-format!
        private void calculateSize(TUIO.TuioObject obj, CursorElement cursor)
        {
            // if there is only one element in the line we cannot adjust the size
            if (rows[cursor.getRow()].elementCount == 1)
                return;

            if (obj.AngleDegrees != cursor.getAngle())
            {
                // Das Objekt wurde gedreht - neue Größe muss errechnet werden.
                GridElement element = cursor.getElement();
                GridElement nextElement = getNextElement(cursor);

                if (obj.AngleDegrees > cursor.getAngle() + obj.AngleDegrees / SettingsControler.SIZESTEPS_PER_CIRCLE)
                {
                    if (element.size < SettingsControler.MAXIMUM_ELEMENT_SIZE - (SettingsControler.GRID_ELEMENTS-1) * SettingsControler.MINIMUN_ELEMENT_SIZE
                        && nextElement.size > SettingsControler.MINIMUN_ELEMENT_SIZE)
                    {
                        // maximum size not reached yet!
                        element.size++;
                        nextElement.size--;
                        Console.WriteLine("Size++");
                    }
                    cursor.setAngle(obj.AngleDegrees);
                }
                else if (obj.AngleDegrees < cursor.getAngle() - obj.AngleDegrees / SettingsControler.SIZESTEPS_PER_CIRCLE)
                {
                    if (element.size > SettingsControler.MINIMUN_ELEMENT_SIZE)
                    {
                        // minimum size not reached yet!
                        element.size--;
                        nextElement.size++;
                        Console.WriteLine("Size--");
                    }
                    cursor.setAngle(obj.AngleDegrees);
                }
            }
        }
        */

        public void InputListener_EVENT_removeObject(object sender, TUIO.TuioObject obj)
		{
			try
			{
				cursorElements.Remove(obj.SymbolID);
				Console.WriteLine("remove object -->" + obj.SymbolID);
				EVENT_dataUpdated?.Invoke(this, rows);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}

		public void InputListener_EVENT_newObject(object sender, TUIO.TuioObject obj)
		{
			try
			{
				CursorElement cursor = createNewElement(obj);
                GridElement existingOverlayElement = null;

                cursor.writeCursorPosition(obj);
				cursor.getElement().type = getType(obj);
                existingOverlayElement = rows[cursor.getRow()].elements[cursor.getCell()].overlayElement;

                if (existingOverlayElement != null)
                    cursor.getElement().size = existingOverlayElement.size;

				setOverlayElement(cursor);

				Console.WriteLine("new object -->" + obj.SymbolID);
				EVENT_dataUpdated?.Invoke(this, rows);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}
        #endregion
	}
}
