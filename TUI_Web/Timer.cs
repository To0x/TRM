using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using TUIO;

namespace TUI_Web
{
    class myTimer
    {
        public event EventHandler<TuioObject> EVENT_TimeOut;
        private TuioObject timeObject = null;

        private bool stopFlag = false;

        // wait 1000ms until the timer triggers the event
        public void startTimer(TuioObject timerObject = null, int timeTillEvent = -1)
        {
            if (timeTillEvent < 0)
                timeTillEvent = Settings.SettingsControler.DELETE_WAITTIME;

            timeObject = timerObject;
            Timer timer = new Timer(trigger, null, timeTillEvent, 0);
        }

        public void stopTimer()
        {
            stopFlag = true;
        }

        private void trigger(object info)
        {
            if (!stopFlag)
            {
                Console.WriteLine("triggered!");
                EVENT_TimeOut?.Invoke(this, timeObject);
                //autoEvent.Set();
            }
        }
    }
}