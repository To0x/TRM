﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TUI_Web.Settings
{
    class SettingsControler
    {
        private int refreshRate;
        private string fileLocation;

        public const int GRID_ELEMENTS = 3;
        public const int BOOTSTRAP_SIZE = 12;

        public const int DEFAULT_ELEMENT_SIZE = BOOTSTRAP_SIZE / GRID_ELEMENTS;

        public const int LINES_DISPLAYED = 3;

        // tells how many size steps can be used while the element turned around.
        // 10 steps is one complete round of the element (360°)
        public const int SIZESTEPS_PER_CIRCLE = 6;

        // each element could not be smaller than the following size
        public const int MINIMUN_ELEMENT_SIZE = 2;

        // each element could not be bigger than the following size
        public const int MAXIMUM_ELEMENT_SIZE = BOOTSTRAP_SIZE;

		// should be false
		// only enable if you want to see all grids at the same time
		public static bool DEFAULT_CURSOR_STATE = false;

        // how long we have to wait, until the object can rly be removed!
        // problem is, that the camera (or the reactivision-software) triggers remove & new even if the element is not moved!
        public static int DELETE_WAITTIME = 1000;


        public static int SAVE_WAITTIME = 1000;
        public static int REMOVE_WAITTIME = 2500;
    

        // is the cmaera pic inverted? 
        // should be true, if the camera captures the picture from below
        public static bool INVERTED = true;

        // if this is set to true, more than one element can be on the ground at the same time
        public static bool MULTI_ELEMENT = true;

        // if this is set to true, the output is more verbose
        // this will show the count of affected elements on the current row
        public static bool DATACONTROLLER_SHOW_AFFECTED = false;

        public static int STYLE_COUNT = 6;

        public static bool SHOW_TIMER_INFORMATION = false;

        public static int SAVE_FRAMES = 5000;

        // how much degrees have the angle to be changed, before the size will also be changed.
        // for example: 360° includes 10 steps for size-changing. Each 36° degrees, the size will be changed
        public static float sizeStep()
        {
            return 360 / SIZESTEPS_PER_CIRCLE;
        }

        public SettingsControler()
        {

        }

        public SettingsControler(int refreshRate, string fileLocation)
        {
            this.refreshRate = refreshRate;
            this.fileLocation = fileLocation;
        }

        public void setRefreshRate(int refreshRate)
        {
            this.refreshRate = refreshRate;
        }

        public int getRefreshRate()
        {
            return refreshRate;
        }

        public void setFileLocation(string fileLocation)
        {

        }

        public string getFileLocation()
        {
            return fileLocation;
        }
    }
}
