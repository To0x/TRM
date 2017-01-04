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

    /*public class DataUpdatedArguments : EventArgs
    {
        public List<GridRow> rows;
        public Dictionary<int, GridElement> cursorElements;

        public DataUpdatedArguments(List<GridRow> rows, Dictionary<int, CursorElement> cursorElements)
        {
            this.rows = rows;
            this.cursorElements = cursorElements;
        }
    }

    public struct CursorElement
    {
        public GridElement element;
        public int row;
        public int cell;
    }
    */

    class DataControler
    {
        public event EventHandler<List<GridRow>> EVENT_dataUpdated;
        private List<GridRow> rows;
		private GridElement oldElement;
        private Dictionary<int, GridElement> currentElements;

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

            currentElements = new Dictionary<int, GridElement>();
        }

        public List<GridRow> getData()
        {
            return this.rows;
        }

		private void createNewObject(TUIO.TuioObject obj)
		{
            GridElement element = new GridElement();
            element.cursor = true;
            element.type = getType(obj);
            setElementPosition(obj.Position, element);

            /*
            setElementPosition(obj.Position, element);
            CursorElement current = new CursorElement();
            setCursorPosition(obj.Position, current);
            current.element = new GridElement();
            current.element.cursor = true;
            current.element.type = getType(obj);
            //setNewElement(element);
            */

            try
            {
                currentElements.Add(obj.SymbolID, element);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
		}

        private GridElement updateObject(TUIO.TuioObject obj)
        {
            GridElement current = null;
            try
            {
                currentElements.TryGetValue(obj.SymbolID, out current);
            } catch (ArgumentNullException e)
            {
                Console.WriteLine(e.Message);
            }

            setElementPosition(obj.Position, current);
            return current;
        }

        public void InputListener_EVENT_updateObject(object sender, TUIO.TuioObject obj)
        {
			updateObject(obj);
			// TODO
			Console.WriteLine("update object( " + obj.SymbolID + " ) x=" + obj.Position.X + "y=" + obj.Position.Y);
            EVENT_dataUpdated?.Invoke(this, rows);
            //EVENT_dataUpdated2?.Invoke(this, new DataUpdatedArguments(rows, currentElements));
        }

        public void InputListener_EVENT_removeObject(object sender, TUIO.TuioObject obj)
        {
            currentElements.Remove(obj.SymbolID);
            // what to do, wenn an object is removed?
			// TODO
			Console.WriteLine("remove object -->" + obj.SymbolID);
			EVENT_dataUpdated?.Invoke(this, rows);
            //EVENT_dataUpdated2?.Invoke(this, new DataUpdatedArguments(rows, currentElements));
        }

		public void InputListener_EVENT_newObject(object sender, TUIO.TuioObject obj)
		{
            createNewObject(obj);
			// what to do, wenn an object is created?
			// TODO
			Console.WriteLine("new object -->" + obj.SymbolID);
            EVENT_dataUpdated?.Invoke(this, rows);
            //EVENT_dataUpdated2?.Invoke(this, new DataUpdatedArguments(rows, currentElements));
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

        /*
		private CursorElement createCursorElement(TUIO.TuioObject obj)
		{
            CursorElement cursorElement = new CursorElement();
			//GridElement element = null;
			int x = -1, y = -1;

			try
			{
				getPosition(obj.Position, ref x, ref y);
                cursorElement.row = x;
                cursorElement.cell = y;
                //cursorElement.element = rows[x].elements[y];
				//element = rows[y].elements[x];
			}
			catch (Exception e)
			{
				System.Console.WriteLine(e.Message);	
			}

			return cursorElement;
		}
        */

		private void setCursor(GridElement element)
		{
			//if (oldElement != null)
			//	oldElement.cursor = false;

			element.cursor = true;
			//oldElement = element;
		}

        private void setElementPosition(TUIO.TuioPoint p, GridElement element)
        {
            int x = -1, y = -1;
            getPosition(p, ref x, ref y);
            rows[x].elements[y].overlayElement = element;
        }

        /*
        private void setCursorPosition(TUIO.TuioPoint p, CursorElement cursor)
        {
            int x = -1, y = -1;
            getPosition(p, ref x, ref y);
            cursor.row = x;
            cursor.cell = y;
        }
        */

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
