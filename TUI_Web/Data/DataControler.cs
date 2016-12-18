using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TUI_Web.Data
{
	// Main Controler to parse data from extern TUI-Controler to internal representation
	// if parsing is finished call EVENT_dataUpdated!
	 
    class DataControler
    {
		public event EventHandler<List<GridRow>> EVENT_dataUpdated;
        private List<GridRow> rows;

        public DataControler(List<GridRow> rows = null)
        {
            if (rows == null)
            {
                // initialize rows
            } else
            {
                this.rows = rows;
            }

        }

        public List<GridRow> getData()
        {
            return this.rows;
        }

        public void InputListener_EVENT_updateObject(object sender, TUIO.TuioObject e)
        {
            // what to do, wenn an object is updated?
			// TODO
            Console.WriteLine("update object");
			EVENT_dataUpdated?.Invoke(this, rows);
            throw new NotImplementedException();
        }

        public void InputListener_EVENT_removeObject(object sender, TUIO.TuioObject e)
        {
            // what to do, wenn an object is removed?
			// TODO
            Console.WriteLine("remove object");
			EVENT_dataUpdated?.Invoke(this, rows);
            throw new NotImplementedException();
        }

        public void InputListener_EVENT_newObject(object sender, TUIO.TuioObject e)
        {
            // what to do, wenn an object is created?
			// TODO
            Console.WriteLine("new object");
			EVENT_dataUpdated?.Invoke(this, rows);
            throw new NotImplementedException();
        }
    }
}
