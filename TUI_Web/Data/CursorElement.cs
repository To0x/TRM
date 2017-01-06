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
                    cell = i;
                    break;
                }
            }

            for (int i = 0; i <= SettingsControler.LINES_DISPLAYED; i++)
            {
                if (p.Y < ((float)(i + 1) / (float)SettingsControler.LINES_DISPLAYED))
                {
                    row = i;
                    break;
                }
            }
        }
    }
}
