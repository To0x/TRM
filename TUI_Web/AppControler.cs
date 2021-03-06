﻿using System;
using System.Threading;

namespace TUI_Web
{
    class AppControler
    {
        #region variables

        // not implemented yet
        public Version version { get; private set; } = null;

        // holds the mainForm View
        private MainView mainView { get; set; } = null;

        // holds all the data
        private Data.DataControler dataControler { get; set; } = null;

        // control the interface for DataInput
        private Data.InputControler inputControler { get; set; } = null;

        // controls the export to the html document
        private Export.ExportControler exportControler { get; set; } = null;

        private Settings.SettingsControler settingsControler { get; set; } = null;

        private Data.StyleData styleData { get; set; } = null;

		private myTimer savedTimer = null;

		// is the website already opened in browser?
		private bool opened = false;
        private int saved_Timer = 0;

        #endregion

        public AppControler(MainView view, Settings.SettingsControler settings)
        {
            mainView = view;
            settingsControler = settings;

            inputControler = new Data.InputControler();
            dataControler = new Data.DataControler(Data.TestData.generateTestData());
            exportControler = new Export.ExportControler(settingsControler);
            styleData = new Data.StyleData();

			// if new data comes over the TCP-Connection send it to DataControler
            inputControler.EVENT_newObject += dataControler.InputListener_EVENT_newObject;
            inputControler.EVENT_removeObject += dataControler.InputListener_EVENT_removeObject;
            inputControler.EVENT_updateObject += dataControler.InputListener_EVENT_updateObject;

            dataControler.EVENT_styleChanged += styleData.changeStyle;

            dataControler.EVENT_Saved += DataControler_EVENT_Saved;

			// if the html-export is finished call appControler function
            exportControler.EVENT_exportFinished += ExportControler_EVENT_exportFinished;

            // if the data if parsed to internal format correctly call exportToHtml
            dataControler.EVENT_dataUpdated += DataControler_EVENT_dataUpdated;

			// start udp-listener (connect also creates a new thread)
			inputControler.connect();

			// create the first html-file to open
			exportControler.exportToHtml(this, dataControler.getData(), true, styleData);


            mainView.EVENT_View_SaveClicked += dataControler.save;
        }

        private void DataControler_EVENT_Saved(object sender, EventArgs e)
        {
			if (savedTimer == null)
			{
				styleData.setSaved(true);
				savedTimer = new myTimer();
				savedTimer.EVENT_TimeOut += Timer_EVENT_TimeOut;
				savedTimer.startTimer(null, Settings.SettingsControler.SAVE_FRAMES);
				//exportControler.exportToHtml(null, dataControler.getData(), false, styleData);
				Console.WriteLine("START SAVE!!!!!!!");
			} 
        }

		private void Timer_EVENT_TimeOut(object sender, TUIO.TuioObject args)
		{
			Console.WriteLine("STOP SAVE!!!!!!!");
			styleData.setSaved(false);
			savedTimer = null;
			exportControler.exportToHtml(null, dataControler.getData(), false, styleData);
		}

        private void DataControler_EVENT_dataUpdated(object sender, System.Collections.Generic.List<GridRow> e)
        {
			/*if (styleData.getSaved())
			{
				if (saved_Timer > Settings.SettingsControler.SAVE_FRAMES)
				{
					styleData.setSaved(false);
					saved_Timer = 0;
				}
				else
					saved_Timer++;
			}*/

            exportControler.exportToHtml(sender, e, styleData);
        }

        private void ExportControler_EVENT_exportFinished(object sender, EventArgs e)
        {
			if (!opened)
			{
				opened =! opened;
			}
        }

        public void close()
        {
            inputControler.disconnect();

			// wait until the export is finished
			while (exportControler.getLockState())
			{
				Console.WriteLine("locked");
				Thread.Sleep(100);
			}

            dataControler = null;
            exportControler = null;
            inputControler = null;
            mainView = null;
            // Shut down the application
        }
    }
}
