using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TUI_Web.Data
{
    class DataControler
    {
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
            Console.WriteLine("update object");
            throw new NotImplementedException();
        }

        public void InputListener_EVENT_removeObject(object sender, TUIO.TuioObject e)
        {
            // what to do, wenn an object is removed?
            Console.WriteLine("remove object");
            throw new NotImplementedException();
        }

        public void InputListener_EVENT_newObject(object sender, TUIO.TuioObject e)
        {
            // what to do, wenn an object is created?
            Console.WriteLine("new object");
            throw new NotImplementedException();
        }
    }
}
