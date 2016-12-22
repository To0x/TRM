using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUI_Web.Settings;

namespace TUI_Web.Data
{
	// Main Controler to parse data from extern TUI-Controler to internal representation
	// if parsing is finished call EVENT_dataUpdated!
	 
    class DataControler
    {
		public event EventHandler<List<GridRow>> EVENT_dataUpdated;
        private List<GridRow> rows;
		private GridElement markedElement;

        public DataControler(List<GridRow> rows = null)
        {
            if (rows == null)
            {
                // initialize rows
            } else
            {
                this.rows = rows;
            }

        }

        public List<GridRow> getData()
        {
            return this.rows;
        }

        public void InputListener_EVENT_updateObject(object sender, TUIO.TuioObject obj)
        {
			setCursorAndType(obj);

            // what to do, wenn an object is updated?
			// TODO
			Console.WriteLine("update object( " + obj.SymbolID + " ) x=" + obj.Position.X + "y=" + obj.Position.Y);
			EVENT_dataUpdated?.Invoke(this, rows);
        }

        public void InputListener_EVENT_removeObject(object sender, TUIO.TuioObject obj)
        {
            // what to do, wenn an object is removed?
			// TODO
			Console.WriteLine("remove object -->" + obj.SymbolID);
			EVENT_dataUpdated?.Invoke(this, rows);
        }

		public void InputListener_EVENT_newObject(object sender, TUIO.TuioObject obj)
		{
			setCursorAndType(obj);

            // what to do, wenn an object is created?
			// TODO
			Console.WriteLine("new object -->" + obj.SymbolID);
			EVENT_dataUpdated?.Invoke(this, rows);
        }

		private void setCursorAndType(TUIO.TuioObject obj)
		{
			if (markedElement != null)
				markedElement.cursor = false;


			try
			{
				int x = -1, y = -1;
				GridElement element = null;

				getPosition(obj.Position, ref x, ref y);
				element = rows[0].elements[x];
				element.type = getType(obj);
				element.cursor = true;
				markedElement = element;
			}
			catch (Exception e)
			{
				System.Console.WriteLine(e.Message);
			}
		}

		private void getPosition(TUIO.TuioPoint p, ref int x, ref int y)
		{
			// Position des Elementes anhand der Gesamtanzahl der Elemente berechnen
			// z.B Pro Zeile 3 Elemente:
			// linkes Feld == Position.X < 1/3 
			// mittleres Feld == Position.X < 2/3
			// rechtes Feld == Position.X < 1
			for (int i = 0; i <= SettingsControler.GRID_ELEMENTS; i++)
			{
				if (p.X < ((float)(i+1) / (float)SettingsControler.GRID_ELEMENTS))
				{
					x = i;
					break;
				}
			}

			// richtige Zeile finden
			/*
			IEnumerator<GridRow> rowEnum = rows.GetEnumerator();
			do
			{
				rowEnum.MoveNext();
			} while (--x == 0);

			rowEnum.Current.elements[y].type = getType(e);
			*/
			y = 0;
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
    }
}
