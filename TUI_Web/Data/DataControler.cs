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
		private GridElement oldElement;

        public DataControler(List<GridRow> rows = null)
        {
            if (rows == null)
            {
				for (int i = 0; i < Settings.SettingsControler.LINES_DISPLAYED; i++)
				{
					rows.Add(new GridRow());
				}
            } else
            {
                this.rows = rows;
            }

        }

        public List<GridRow> getData()
        {
            return this.rows;
        }

		private void doStuff(TUIO.TuioObject obj)
		{
			GridElement element = getElement(obj);
			setNewElement(element);
			element.cursor = true;
			element.type = getType(obj);
		}

        public void InputListener_EVENT_updateObject(object sender, TUIO.TuioObject obj)
        {
			doStuff(obj);
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
			doStuff(obj);
			// what to do, wenn an object is created?
			// TODO
			Console.WriteLine("new object -->" + obj.SymbolID);
			EVENT_dataUpdated?.Invoke(this, rows);
        }

		private void setNewElement(GridElement element)
		{
			if (oldElement != null)
			{
				oldElement.cursor = false;
				// TODO: Was war vorher da? Was passiert, wenn man über eine Grafik eine Überschrift legt?
				// TODO: aktuell gibt es immer nur ein Element auf der Oberfläche
				// TODO: Mittels klick muss dieses Element gespeichert werden - implementieren!
				oldElement.type = ElementTypes.None;
			}

			oldElement = element;
		}

		private GridElement getElement(TUIO.TuioObject obj)
		{
			GridElement element = null;
			int x = -1, y = -1;

			try
			{
				getPosition(obj.Position, ref x, ref y);
				element = rows[y].elements[x];
			}
			catch (Exception e)
			{
				System.Console.WriteLine(e.Message);	
			}

			return element;
		}


		private void setCursor(GridElement element)
		{
			//if (oldElement != null)
			//	oldElement.cursor = false;

			element.cursor = true;
			//oldElement = element;
		}

		private void getPosition(TUIO.TuioPoint p, ref int x, ref int y)
		{
			x = -1;
			y = -1;
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

			IEnumerator<GridRow> rowEnum = rows.GetEnumerator();
			for (int i = 0; i <= SettingsControler.LINES_DISPLAYED; i++)
			{
				rowEnum.MoveNext();

				if (p.Y < ((float)(i + 1) / (float)SettingsControler.LINES_DISPLAYED))
				{
					y = i;
					break;
				}
			}
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
