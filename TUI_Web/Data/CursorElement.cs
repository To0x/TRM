using System;
using TUI_Web.Settings;
using System.Collections.Generic;

namespace TUI_Web.Data
{
    public enum SizeChangingType { IncreaseOther, DecreaseOther, RemoveLast };

    public class GridPosition
    {
        public int row;
        public int cell;
    }

    public class CursorEventPositionArgs : EventArgs
    {
        public GridPosition oldPosition;
        public GridPosition newPosition;

        public CursorEventPositionArgs()
        {
            oldPosition = new GridPosition();
            newPosition = new GridPosition();
        }
    }

    public class CursorEventSizeArgs : EventArgs
    {
        public SizeChangingType changeType { get; set; }
        public int changeSteps { get; set; }
    }

    // this class is only for this Controler, to save the position of the elements
    // to reset cursors and smth. else for existing objects
    public class CursorElement
    {
        public event EventHandler<CursorEventSizeArgs> EVENT_SizeChanged;
        public event EventHandler<CursorEventPositionArgs> EVENT_PositionChanged;

        private OverlayElement element;
		public bool affected;

        private GridPosition position;
        //private int row;
        //private int cell;

        private float angle;

        public CursorElement()
        {
            element = new OverlayElement();
            position = new GridPosition();
            element.setCursor();
            position.row = -1;
            position.cell = -1;
			angle = .0f;
        }

        #region getterSetter
        public void getPosition(ref int x, ref int y)
        {
            x = position.row;
            y = position.cell;
        }

        public GridPosition getPosition()
        {
            return position;
        }

        public int getRow()
        {
            return position.row;
        }

        public int getCell()
        {
            return position.cell;
        }

        public float getAngle()
        {
            return angle;
        }

        public void setAngle(float angle)
        {
            this.angle = angle;
        } 

        public OverlayElement getElement()
        {
            return element;
        }

        public void setElement(OverlayElement element)
        {
            this.element = element;
        }
        #endregion

        public void writeCursorPosition(TUIO.TuioObject obj, List<GridRow> rows)
        {
            // Position des Elementes anhand der Gesamtanzahl der Elemente berechnen
            // z.B Pro Zeile 3 Elemente:
            // linkes Feld == Position.X < 1/3 
            // mittleres Feld == Position.X < 2/3
            // rechtes Feld == Position.X < 1
            TUIO.TuioPoint p = obj.Position;
            bool positionChanged = false;
            CursorEventPositionArgs posArgs = new CursorEventPositionArgs();
			posArgs.oldPosition.row = this.getRow();
			posArgs.oldPosition.cell = this.getCell();


            if (SettingsControler.INVERTED)
            {
                for (int i = 1; i <= (SettingsControler.LINES_DISPLAYED + 1); i++)
                {
                    if (p.Y < ((float)(i) / (float)SettingsControler.LINES_DISPLAYED))
                    {
                        if (position.row != (SettingsControler.LINES_DISPLAYED - (i)))
                        {
                            positionChanged = true;
                            posArgs.oldPosition.row = position.row;
                        }

                        position.row = SettingsControler.LINES_DISPLAYED - (i);
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i <= SettingsControler.LINES_DISPLAYED; i++)
                {
                    if (p.Y < ((float)(i + 1) / (float)SettingsControler.LINES_DISPLAYED))
                    {
                        if (position.row != i)
                        { 
                            positionChanged = true;
                            posArgs.oldPosition.row = position.row;
                        }

                        position.row = i;
                        break;
                    }

                }
            }

			for (int i = 0; i <= rows[position.row].elementCount; i++)
            {
				if (p.X < ((float)(i + 1) / (float)rows[position.row].elementCount)) 
                {
                    if (position.cell != i)
                    {
                        positionChanged = true;
                        posArgs.oldPosition.cell = position.cell;
                    }

                    position.cell = i;
                    break;
                }
            }

			if (positionChanged)
			{
				posArgs.newPosition = this.position;
				EVENT_PositionChanged?.Invoke(this, posArgs);
			}
        }

        private bool isIncreased(TUIO.TuioObject obj)
        {
            if (SettingsControler.INVERTED)
            {
                if (obj.AngleDegrees < angle - SettingsControler.sizeStep())
                {
                    return true;
                }
                else if (obj.AngleDegrees > angle + SettingsControler.sizeStep())
                {
                    return false;
                }
            }
            else
            {
                // || (obj.AngleDegrees < 0 + SettingsControler.sizeStep() && angle > 360 - SettingsControler.sizeStep()))
                if (obj.AngleDegrees > angle + SettingsControler.sizeStep())
                {
                    return true;
                }
                // || obj.AngleDegrees > 360 - SettingsControler.sizeStep() && angle < 0 + SettingsControler.sizeStep())
                else if (obj.AngleDegrees < angle - SettingsControler.sizeStep() )
                {
                    return false;
                }
            }

            return false;
        }

        private bool isDecreased(TUIO.TuioObject obj)
        {
            if (SettingsControler.INVERTED)
            {
                if (obj.AngleDegrees < angle - SettingsControler.sizeStep())
                {
                    return false;
                }
                else if (obj.AngleDegrees > angle + SettingsControler.sizeStep())
                {
                    return true;
                }
            }
            else
            {
                // || (obj.AngleDegrees < 0 + SettingsControler.sizeStep() && angle > 360 - SettingsControler.sizeStep()))
                if (obj.AngleDegrees > angle + SettingsControler.sizeStep())
                    
                {
                    return false;
                }
                // ||  obj.AngleDegrees > 360 - SettingsControler.sizeStep() && angle < 0 + SettingsControler.sizeStep())
                else if (obj.AngleDegrees < angle - SettingsControler.sizeStep())
                {
                    return true;
                }
            }

            return false;
        }

        public void writeCursorSize(TUIO.TuioObject obj, GridRow row)
        {
            Console.WriteLine("obj-angle: " + obj.AngleDegrees + "SAVED: " + angle);

            if ((int)(obj.AngleDegrees) != (int)(angle))
            {
                CursorEventSizeArgs sizeArgs = new CursorEventSizeArgs();

                // the angle of the object was changed 
                //if (obj.AngleDegrees > angle + SettingsControler.sizeStep())
                if (isIncreased(obj))
                {
                    // the size can only increase if the following are true:
                    // 1. Maximum Size is not Reached yet
                    // 2.1. Other elements can be decrease OR
                    // 2.2. another element can be set their size to 0. 
                    if (element.size < SettingsControler.MAXIMUM_ELEMENT_SIZE)
                    {

                        // 2.1. other elements can be decreased
                        int count = 0;
                        if ((count += row.getDecreasableElementCount(element)) > 0)
                        {

                            // these elememnts must decrease their size
                            if (element.size + count <= SettingsControler.MAXIMUM_ELEMENT_SIZE
                                && element.size + count >= SettingsControler.MINIMUN_ELEMENT_SIZE)
                            {
                                element.size += count;

                                sizeArgs.changeType = SizeChangingType.DecreaseOther;
                                sizeArgs.changeSteps = -1;
                            }
                            else
                            {
                                element.size += SettingsControler.MINIMUN_ELEMENT_SIZE;

                                sizeArgs.changeType = SizeChangingType.DecreaseOther;
                                sizeArgs.changeSteps = -2;
                            }
                        }
                        // 2.2. kick out the smallest element
                        else if (row.getSmallestSize() == SettingsControler.MINIMUN_ELEMENT_SIZE
                            && row.getSmallestElement().type == ElementTypes.None)
                        {

                            element.size += SettingsControler.MINIMUN_ELEMENT_SIZE;

                            sizeArgs = new CursorEventSizeArgs();
                            sizeArgs.changeType = SizeChangingType.RemoveLast;
                            sizeArgs.changeSteps = SettingsControler.MAXIMUM_ELEMENT_SIZE;
                        }


                        Console.WriteLine("Size++");
                        EVENT_SizeChanged?.Invoke(this, sizeArgs);
                    }
                    angle = obj.AngleDegrees;

                }
                //
                else if (isDecreased(obj))
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

                                sizeArgs = new CursorEventSizeArgs();
                                sizeArgs.changeType = SizeChangingType.IncreaseOther;
                                sizeArgs.changeSteps = 1;

                                System.Console.WriteLine("Size--");
                                EVENT_SizeChanged?.Invoke(this, sizeArgs);
                            }
                        }
                    }
                    angle = obj.AngleDegrees;
                }
            }
                /*
                else
                {
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
                            if ((count += row.getDecreasableElementCount(element)) > 0)
                            {

                                // these elememnts must decrease their size
                                if (element.size + count <= SettingsControler.MAXIMUM_ELEMENT_SIZE
                                    && element.size + count >= SettingsControler.MINIMUN_ELEMENT_SIZE)
                                {
                                    element.size += count;

                                    sizeArgs.changeType = SizeChangingType.DecreaseOther;
                                    sizeArgs.changeSteps = -1;
                                }
                                else
                                {
                                    element.size += SettingsControler.MINIMUN_ELEMENT_SIZE;

                                    sizeArgs.changeType = SizeChangingType.DecreaseOther;
                                    sizeArgs.changeSteps = -2;
                                }
                            }
                            // 2.2. kick out the smallest element
                            else if (row.getSmallestSize() == SettingsControler.MINIMUN_ELEMENT_SIZE
                                && row.getSmallestElement().type == ElementTypes.None)
                            {

                                element.size += SettingsControler.MINIMUN_ELEMENT_SIZE;

                                sizeArgs = new CursorEventSizeArgs();
                                sizeArgs.changeType = SizeChangingType.RemoveLast;
                                sizeArgs.changeSteps = SettingsControler.MAXIMUM_ELEMENT_SIZE;
                            }


                            Console.WriteLine("Size++");
                            EVENT_SizeChanged?.Invoke(this, sizeArgs);
                        }
                        angle = obj.AngleDegrees;

                    }
                    //
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

                                    sizeArgs = new CursorEventSizeArgs();
                                    sizeArgs.changeType = SizeChangingType.IncreaseOther;
                                    sizeArgs.changeSteps = 1;

                                    System.Console.WriteLine("Size--");
                                    EVENT_SizeChanged?.Invoke(this, sizeArgs);
                                }
                            }
                        }
                        angle = obj.AngleDegrees;
                    }
                }
                */
            }
        }
    }
