using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUIO;

namespace TUI_Web.Data
{

    class InputControler : TuioListener
    {
        public event EventHandler<TuioObject> EVENT_newObject;
        public event EventHandler<TuioObject> EVENT_updateObject;
        public event EventHandler<TuioObject> EVENT_removeObject;

        private TuioClient client;
        /*
         private Dictionary<long, TuioObject> objectList;
        private Dictionary<long, TuioCursor> cursorList;
        private Dictionary<long, TuioBlob> blobList;
        private object cursorSync = new object();
        private object objectSync = new object();
        private object blobSync = new object();
		*/

        public InputControler(int port = 3333)
        {
            //objectList = new Dictionary<long, TuioObject>(128);
            client = new TuioClient(port);
        }

        public void connect()
        {
            client.addTuioListener(this);
            client.connect();
        }

        public void disconnect()
        {
            client.removeTuioListener(this);
            client.disconnect();
        }

        public void removeTuioObject(TuioObject tobj)
        {
            EVENT_removeObject?.Invoke(this, tobj);
        }

        public void updateTuioObject(TuioObject tobj)
        {
            EVENT_updateObject?.Invoke(this, tobj);
        }

        public void addTuioObject(TuioObject tobj)
        {
            EVENT_newObject?.Invoke(this, tobj);
        }

        // cursows and blobs are not implemented yet!
        #region notImplemented
        public void addTuioBlob(TuioBlob tblb)
        {
            //throw new NotImplementedException();
        }

        public void addTuioCursor(TuioCursor tcur)
        {
            //throw new NotImplementedException();
        }

        public void refresh(TuioTime ftime)
        {
            //throw new NotImplementedException();
        }

        public void removeTuioBlob(TuioBlob tblb)
        {
            //throw new NotImplementedException();
        }

        public void removeTuioCursor(TuioCursor tcur)
        {
            //throw new NotImplementedException();
        }

        public void updateTuioBlob(TuioBlob tblb)
        {
            //throw new NotImplementedException();
        }

        public void updateTuioCursor(TuioCursor tcur)
        {
            //throw new NotImplementedException();
        }
        #endregion
    }
}
