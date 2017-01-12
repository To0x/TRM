using System;
using TUI_Web.Settings;

namespace TUI_Web.Data
{
    // this class is only for this Controler, to save the position of the elements
    // to reset cursors and smth. else for existing objects
    class CursorElement
    {
        public event EventHandler<int> EVENT_SizeChanged;
        public event EventHandler EVENT_PositionChanged;

        private GridElement element;
        private int row;
        private int cell;

        private float angle;

        public CursorElement()
        {
            element = new GridElement();
            element.setCursor();
            row = -1;
            cell = -1;
            angle = 0;
            //size = SettingsControler.BOOTSTRAP_SIZE / SettingsControler.GRID_ELEMENTS;
        }

        #region getterSetter
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

        public float getAngle()
        {
            return angle;
        }

        public void setAngle(float angle)
        {
            this.angle = angle;
        } 

        public GridElement getElement()
        {
            return element;
        }

        public void setElement(GridElement element)
        {
            this.element = element;
        }
        #endregion

        public void writeCursorPosition(TUIO.TuioObject obj)
        {
            // Position des Elementes anhand der Gesamtanzahl der Elemente berechnen
            // z.B Pro Zeile 3 Elemente:
            // linkes Feld == Position.X < 1/3 
            // mittleres Feld == Position.X < 2/3
            // rechtes Feld == Position.X < 1
            TUIO.TuioPoint p = obj.Position;
            bool positionChanged = false;
            for (int i = 0; i <= SettingsControler.GRID_ELEMENTS; i++)
            {
                if (p.X < ((float)(i + 1) / (float)SettingsControler.GRID_ELEMENTS))
                {
                    if (cell != i)
                        positionChanged = true;

                    cell = i;
                    break;
                }
            }

            for (int i = 0; i <= SettingsControler.LINES_DISPLAYED; i++)
            {
                if (p.Y < ((float)(i + 1) / (float)SettingsControler.LINES_DISPLAYED))
                {
                    if (row != i)
                        positionChanged = true;

                    row = i;
                    break;
                }
            }

            if (positionChanged)
                EVENT_PositionChanged?.Invoke(this, null);
        }

        public void writeCursorSize(TUIO.TuioObject obj)
        {
            // if there is only one element in the line we cannot adjust the size
            //if (rows[cursor.getRow()].elementCount == 1)
            //    return;

            if (obj.AngleDegrees != angle)
            {
                // the angle of the object was changed 
                if (obj.AngleDegrees > angle + SettingsControler.sizeStep())
                {
                    // the size can only increased if the following are true:
                    // 1. Maximum Size is not Reached yet
                    // 2. Minimum Size of the next Element is not reached yet.
                    if (element.size < SettingsControler.MAXIMUM_ELEMENT_SIZE - (SettingsControler.GRID_ELEMENTS - 1) * SettingsControler.MINIMUN_ELEMENT_SIZE)
                    {
                        // maximum size not reached!
                        element.size += (SettingsControler.GRID_ELEMENTS-1);
                        Console.WriteLine("Size++");

                        EVENT_SizeChanged?.Invoke(this, (SettingsControler.GRID_ELEMENTS - 1));

                        
                    }
                    angle = obj.AngleDegrees;
                }
                else if (obj.AngleDegrees < angle - SettingsControler.sizeStep())
                {
                    if (element.size >= (SettingsControler.MINIMUN_ELEMENT_SIZE + SettingsControler.GRID_ELEMENTS-1))
                    {
                        // minimum size not reached!
                        element.size -= (SettingsControler.GRID_ELEMENTS - 1);
                        //size--;
                        System.Console.WriteLine("Size--");

                        EVENT_SizeChanged?.Invoke(this, (SettingsControler.GRID_ELEMENTS - 1) * -1);
                    }
                    angle = obj.AngleDegrees;
                }
            }
        }
    }
}
