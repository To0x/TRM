using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUI_Web.Settings;

namespace TUI_Web.Data
{
	// this class is only for this Controler, to save the position of the elements
	// to reset cursors and smth. else for existing objects
	class CursorElement
	{
		private GridElement element;
		private int row;
		private int cell;

		public CursorElement()
		{
			element = new GridElement();
			row = -1;
			cell = -1;
		}

		public void getPosition(ref int x, ref int y)
		{
			x = row;
			y = cell;
		}

		public int getRow()
		{
			return row;
		}

		public int getCell()
		{
			return cell;
		}

		public GridElement getElement()
		{
			return element;
		}

		public void setElement(GridElement element)
		{
			this.element = element;
		}

		public void writeCursorPosition(TUIO.TuioObject obj)
		{
			// Position des Elementes anhand der Gesamtanzahl der Elemente berechnen
			// z.B Pro Zeile 3 Elemente:
			// linkes Feld == Position.X < 1/3 
			// mittleres Feld == Position.X < 2/3
			// rechtes Feld == Position.X < 1
			TUIO.TuioPoint p = obj.Position;
			for (int i = 0; i <= SettingsControler.GRID_ELEMENTS; i++)
			{
				if (p.X < ((float)(i + 1) / (float)SettingsControler.GRID_ELEMENTS))
				{
					row = i;
					break;
				}
			}

			IEnumerator<GridRow> rowEnum = rows.GetEnumerator();
			for (int i = 0; i <= SettingsControler.LINES_DISPLAYED; i++)
			{
				rowEnum.MoveNext();

				if (p.Y < ((float)(i + 1) / (float)SettingsControler.LINES_DISPLAYED))
				{
					cell = i;
					break;
				}
			}
		}
	}

	class DataControler
	{
		public event EventHandler<List<GridRow>> EVENT_dataUpdated;
		private List<GridRow> rows;
		private GridElement oldElement;
		private Dictionary<int, CursorElement> currentElements;

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

			currentElements = new Dictionary<int, CursorElement>();
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
				currentElements.Add(obj.SymbolID, cursor);
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
			currentElements.TryGetValue(obj.SymbolID, out cursor);
			return cursor;
		}

		private bool removeExistingElement(TUIO.TuioObject obj)
		{
			try
			{
				currentElements.Remove(obj.SymbolID);
				return true;
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				return false;
			}

		}

		private bool updateExistingObject(TUIO.TuioObject obj)
		{
			try
			{

				CursorElement cursor = getExistingElement(obj);

				// alten Cursor löschen
				removeOverlayElement(cursor);

				// neue Position errechnen
				cursor.writeCursorPosition(obj);

				// neue Position ins Grid schreiben
				setOverlayElement(cursor);

				return true;
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				return false;
			}
		}

		private void setOverlayElement(CursorElement cursor)
		{
			rows[cursor.getRow()].elements[cursor.getCell()].overlayElement = cursor.getElement();
		}

		private void removeOverlayElement(CursorElement cursor)
		{
			rows[cursor.getRow()].elements[cursor.getCell()].overlayElement = null;
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
				currentElements.Remove(obj.SymbolID);
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
