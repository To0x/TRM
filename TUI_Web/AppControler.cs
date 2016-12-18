using System;
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

        private Thread listenerThread = null;
        #endregion

        public AppControler(MainView view, Settings.SettingsControler settings)
        {
            mainView = view;
            settingsControler = settings;

            inputControler = new Data.InputControler();
            dataControler = new Data.DataControler(Data.TestData.generateTestData());
            exportControler = new Export.ExportControler(settingsControler);

			// if new data comes over the TCP-Connection send it to DataControler
            inputControler.EVENT_newObject += dataControler.InputListener_EVENT_newObject;
            inputControler.EVENT_removeObject += dataControler.InputListener_EVENT_removeObject;
            inputControler.EVENT_updateObject += dataControler.InputListener_EVENT_updateObject;

			// if the html-export is finished call appControler function
            exportControler.EVENT_exportFinished += ExportControler_EVENT_exportFinished;

			// if the data if parsed to internal format correctly call exportToHtml
			dataControler.EVENT_dataUpdated += exportControler.exportToHtml;

            // threads
            // TCP-Server for data is listening as a threads
            listenerThread = new Thread(inputControler.connect);
            listenerThread.Start();

            // thread for Exporting data
            //Thread exporterThread = new Thread(exportControler.listen);
            //exporterThread.Start();
            exportControler.exportToHtml(this, dataControler.getData());
        }

        private void ExportControler_EVENT_exportFinished(object sender, EventArgs e)
        {
            // do smth. when export is finished? 
        }

        public void close()
        {
            inputControler.disconnect();
			listenerThread.Abort();

			// wait until the export is finished
			while (exportControler.getLockState())
			{
				Console.WriteLine("locked");
				Thread.Sleep(100);
			}


            exportControler.close();
            listenerThread = null;
            dataControler = null;
            exportControler = null;
            inputControler = null;
            mainView = null;
            // Shut down the application
        }
    }
}
