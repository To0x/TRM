using System;
using System.Collections.Generic;

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
				for (int i = 0; i < Settings.SettingsControler.LINES_DISPLAYED; i++)
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

		#region EVENTS
		public void InputListener_EVENT_updateObject(object sender, TUIO.TuioObject obj)
		{
			try
			{
				CursorElement cursor = getExistingElement(obj);
				removeOverlayElement(cursor);
				cursor.writeCursorPosition(obj);
				setOverlayElement(cursor);

				Console.WriteLine("update object( " + obj.SymbolID + " ) x=" + obj.Position.X + "y=" + obj.Position.Y);
				EVENT_dataUpdated?.Invoke(this, rows);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
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

        public void InputListener_EVENT_save(object sender, TUIO.TuioObject obj)
        {
            foreach (KeyValuePair<int, CursorElement> cursor  in cursorElements)
            {
                removeOverlayElement(cursor.Value);
                setRealElement(cursor.Value);
            }
        }
		#endregion
	}
}
