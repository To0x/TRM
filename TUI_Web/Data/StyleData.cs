﻿using System;
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
        private bool saved = false;

        public void changeStyle(object sender, CursorEventSizeArgs sizeArgs)
        {
            ManipulationTypes manType = ((CursorElement)sender).getElement().getManipulationType();
            int changingSteps = sizeArgs.changeSteps * -1;

            switch (manType)
            { 
                case ManipulationTypes.FontColor:
                    fontColor += changingSteps;
                    break;

                case ManipulationTypes.FontStyle:
                    fontStyle += changingSteps;
                    break;

                case ManipulationTypes.LookAndFeel:
                    lookAndFeel += changingSteps;
                    break;
            }

            checkRanges();
        }

        private void checkRanges()
        {
            if (fontColor > Settings.SettingsControler.STYLE_COUNT)
                fontColor = 1;

            if (fontColor < 1)
                fontColor = Settings.SettingsControler.STYLE_COUNT;

            if (fontStyle > Settings.SettingsControler.STYLE_COUNT)
                fontStyle = 1;

            if (fontStyle < 1)
                fontStyle = Settings.SettingsControler.STYLE_COUNT;

            if (lookAndFeel > Settings.SettingsControler.STYLE_COUNT)
                lookAndFeel = 1;

            if (lookAndFeel < 1)
                lookAndFeel = Settings.SettingsControler.STYLE_COUNT;
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

        public void setSaved(bool saveState)
        {
            saved = saveState;
        }

        public bool getSaved()
        {
            return saved;
        }
    }
}
