﻿using System;
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
			rows[cursor.getRow()].elements[cursor.getCell()].setElement(cursor.getElement());
		}

		private void removeOverlayElement(CursorElement cursor)
		{
			rows[cursor.getRow()].elements[cursor.getCell()].setElement(null);
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

                    if (currentElement.getElement() != null)
                    {
                        currentElement = currentElement.getElement();
                        currentElement.setElement(null);
                    }
                }
            }
            cursorElements.Clear();
        }

        #region EVENTS
        public void InputListener_EVENT_updateObject(object sender, TUIO.TuioObject obj)
		{
            try
            {
                CursorElement cursor = getExistingElement(obj);

                // TODO: StartTransaction -> if error, reset states

                // calculate cursor Position
                cursor.writeCursorPosition(obj);

				// if there is already a element -> stop
				//if (rows[cursor.getRow()].elementExists(cursor))
				//{
				//	return;
				//}

                // write new size of the cursor-element, may it has been changed
                cursor.writeCursorSize(obj, rows[cursor.getRow()]);

                // new Element is finished, set it to the grid!
                setOverlayElement(cursor);


                Console.Write("update object( " + obj.SymbolID + " )\r\n" );
                int counter = 0;
                foreach (GridElement element in rows[cursor.getRow()].elements)
                {
                    if (element.getElement() != null)
						Console.Write("[" + cursor.getRow() + ";" + counter + "]:" + element.getElement().size + "\r\n ");
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

                    reCalculateSize(element.getElement(), cursorArgs.changeSteps);
                    counter++;

                    if (counter == affectedRow.elementCount)
                        break;
                }
            }
            else if (cursorArgs.changeType == SizeChangingType.RemoveLast)
            {
                GridElement element = rows[cursor.getRow()].getSmallestElement();
                if (element.getElement() != null)
                    element.getElement().size = 0;
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
            OverlayElement affectedElement = rows[cursor.getRow()].elements[cursor.getCell()].getElement();


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
					rows[posArgs.oldPosition.row].elements[posArgs.oldPosition.cell].setElement(oldElement);
				}
				else
				{
					rows[posArgs.oldPosition.row].elements[posArgs.oldPosition.cell].setElement(null);
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

                cursor.writeCursorPosition(obj);
				cursor.getElement().type = getType(obj);

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
