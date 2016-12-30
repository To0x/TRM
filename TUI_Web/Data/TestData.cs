using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TUI_Web.Data
{
    class TestData
    {
        public static List<GridRow> generateTestData()
        {
            List<GridRow> rows = new List<GridRow>();
			for (int i = 0; i < Settings.SettingsControler.LINES_DISPLAYED; i++)
            {
                GridRow row = new GridRow();

                switch (i)
                {
                    default:
                        {
                            //row.elements[i].cursor = true;
                            break;
                        }
                }

                rows.Add(row);
            }
            return rows;
        }
    }
}
