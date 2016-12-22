using System;
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
