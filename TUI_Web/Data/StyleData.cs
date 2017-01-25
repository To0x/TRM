using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TUI_Web.Data
{
    class StyleData
    {
        private int fontColor = 1;
        private int fontStyle = 1;
        private int lookAndFeel = 1;

        public void changeStyle(object sender, CursorEventSizeArgs sizeArgs)
        {
            ManipulationTypes manType = ((CursorElement)sender).getElement().getManipulationType();
            int changingSteps = sizeArgs.changeSteps * -1;

            switch (manType)
            { 
                case ManipulationTypes.FontColor:
                    fontColor += changingSteps;
                    break;

                case ManipulationTypes.FontSize:
                    fontStyle += changingSteps;
                    break;

                case ManipulationTypes.LookAndFeel:
                    lookAndFeel += lookAndFeel;
                    break;
            }

            checkMaximumRange();
        }

        private void checkMaximumRange()
        {
            if (fontColor > Settings.SettingsControler.STYLE_COUNT)
                fontColor = 1;

            if (fontStyle > Settings.SettingsControler.STYLE_COUNT)
                fontStyle = 1;

            if (lookAndFeel > Settings.SettingsControler.STYLE_COUNT)
                lookAndFeel = 1;
        }

        public int getFontColor()
        {
            return fontColor;
        }

        public int getFontStyle()
        {
            return fontStyle;
        }

        public int getLookAndFeel()
        {
            return lookAndFeel;
        }
    }
}
