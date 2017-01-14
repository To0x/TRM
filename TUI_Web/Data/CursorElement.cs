using System;
using TUI_Web.Settings;

namespace TUI_Web.Data
{
    public enum SizeChangingType { IncreaseOther, DecreaseOther, RemoveLast };
    public class CursorEventSizeArgs : EventArgs
    {
        public SizeChangingType changeType { get; set; }
        public int changeSteps { get; set; }
    }

    // this class is only for this Controler, to save the position of the elements
    // to reset cursors and smth. else for existing objects
    class CursorElement
    {
        public event EventHandler<CursorEventSizeArgs> EVENT_SizeChanged;
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

        public void writeCursorSize(TUIO.TuioObject obj, GridRow row)
        {
            if (obj.AngleDegrees != angle)
            {
                CursorEventSizeArgs cursorArg = new CursorEventSizeArgs();

                // the angle of the object was changed 
                if (obj.AngleDegrees > angle + SettingsControler.sizeStep())
                {
                    // the size can only increase if the following are true:
                    // 1. Maximum Size is not Reached yet
                    // 2.1. Other elements can be decrease OR
                    // 2.2. another element can be set their size to 0. 
                    if (element.size < SettingsControler.MAXIMUM_ELEMENT_SIZE)
                    {

                        // 2.1. other elements can be decreased
                        int count = 0;
                        if ((count += row.getDecreasableElementCount(this.)) > 0)
                        {
                            
                            // these elememnts must decrease their size
                            if (element.size + count <= SettingsControler.MAXIMUM_ELEMENT_SIZE)
                            {
                                element.size += count;

                                cursorArg.changeType = SizeChangingType.DecreaseOther;
                                cursorArg.changeSteps = -1;
                            }
                        }
                        // 2.2. kick out the smallest element
                        else if (row.getSmallestSize() == SettingsControler.MINIMUN_ELEMENT_SIZE 
                            && row.getSmallestElement().type == ElementTypes.None)
                        {
                            
                            element.size += SettingsControler.MINIMUN_ELEMENT_SIZE;

                            cursorArg = new CursorEventSizeArgs();
                            cursorArg.changeType = SizeChangingType.RemoveLast;
                            cursorArg.changeSteps = SettingsControler.MAXIMUM_ELEMENT_SIZE;
                        }


                        Console.WriteLine("Size++");
                        EVENT_SizeChanged?.Invoke(this, cursorArg);
                    }
                    angle = obj.AngleDegrees;

                }
                else if (obj.AngleDegrees < angle - SettingsControler.sizeStep())
                {
                    // the size can only decrease if the following are true:
                    // 1. Minimum Size is not Reached yet
                    // 2. Other elements can increase OR
                    // TODO?: Another Element can inserted
                    if (element.size >= SettingsControler.MINIMUN_ELEMENT_SIZE)
                    {
                        // 2. Other elements can increase OR
                        int count = 0;
                        if ((count += row.getIncreasableElementCount(this.element)) > 0)
                        {
                            // if there are no elements to increase, we cant adjust the size anymore
                            if (count == 0)
                                return;

                            if (element.size - count >= SettingsControler.MINIMUN_ELEMENT_SIZE)
                            {
                                element.size -= count;

                                cursorArg = new CursorEventSizeArgs();
                                cursorArg.changeType = SizeChangingType.IncreaseOther;
                                cursorArg.changeSteps = 1;

                                System.Console.WriteLine("Size--");
                                EVENT_SizeChanged?.Invoke(this, cursorArg);
                            }
                        }
                    }
                    angle = obj.AngleDegrees;
                }
            }
        }
    }
}
