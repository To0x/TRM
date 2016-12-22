using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.UI;
using TUI_Web.Settings;

namespace TUI_Web.Export
{
    class ExportControler
    {
        public event EventHandler EVENT_exportFinished;

        private HtmlTextWriter writer = null;
        private Settings.SettingsControler settingsControler = null;
		public Object lockObjekt = new Object();
		private bool writingInProgress = false;
        
        public ExportControler(int refreshRate, string filePath = "./test.html")
        {
            settingsControler = new Settings.SettingsControler(refreshRate, filePath);
            //createHtmlWriter();
        }

        public ExportControler(Settings.SettingsControler settings)
        {
            settingsControler = settings;
            //createHtmlWriter();
        }

        // export the current content to the html-file
        public void exportToHtml(object sender, List<GridRow> rows)
        {
			createHtmlWriter();
			writingInProgress = true;
			lock(lockObjekt)
			{
				// Datie nicht immer neu schreiben, sondern erneuern!
				//File.WriteAllText(settingsControler.getFileLocation(), String.Empty);
				writer.RenderBeginTag(HtmlTextWriterTag.Html);
				{
					writeHead();
					writeBody(rows);
				}
				writer.RenderEndTag();
				writer.Flush();
			}
			writingInProgress = false;
			close();
            EVENT_exportFinished?.Invoke(this, null);
        }

		public bool getLockState()
		{
			return writingInProgress;
		}


        private void close()
        {
            writer.Flush();
            writer.Close();
			writer = null;
        }

        // is used to create the HtmlTextWriter-Class
        // is also used to create the file or open it (if it exists)
        private void createHtmlWriter()
        {
            string filePath = settingsControler.getFileLocation();
            FileStream fs;
            if (!File.Exists(filePath))
            {
                fs = new FileStream(filePath, FileMode.Create);
            }
            else
            {
                fs = new FileStream(filePath, FileMode.Open);
            }
            StreamWriter streamWriter = new StreamWriter(fs, Encoding.UTF8);
            writer = new HtmlTextWriter(streamWriter);
        }

        private void writeElement(GridElement element)
        {
            string test = String.Format("col-md-{0}", element.size);

            if (element.cursor == true)
            {
                test += String.Format(" cursor");
            }

            writer.AddAttribute(HtmlTextWriterAttribute.Class, test);
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.RenderEndTag();
            writer.WriteLine();
        }

        private void writeRow(GridRow row)
        {
            writer.WriteLine();
			writer.AddAttribute(HtmlTextWriterAttribute.Class, String.Format("col-md-{0} col-sm-{0}", SettingsControler.BOOTSTRAP_SIZE));
            writer.RenderBeginTag(HtmlTextWriterTag.Div);


            foreach (GridElement element in row.elements)
            {
                writeElement(element);
            }

            writer.RenderEndTag();

        }

        private void writeBody(List<GridRow> rows)
        {
            writer.RenderBeginTag(HtmlTextWriterTag.Body);
            foreach (GridRow row in rows)
            {
                writeRow(row);
            }
            writer.RenderEndTag();
        }

        private void writeHead()
        {
            // Title
            writer.RenderBeginTag(HtmlTextWriterTag.Head);
            writer.RenderBeginTag(HtmlTextWriterTag.Title);
            writer.Write(DateTime.Now);
            writer.RenderEndTag();

            // Auto Refresh
            if (settingsControler.getRefreshRate() != 0)
            {
                writer.AddAttribute("http-equiv", "refresh");
                writer.AddAttribute("content", (settingsControler.getRefreshRate() / 1000).ToString());
                writer.RenderBeginTag(HtmlTextWriterTag.Meta);
                writer.RenderEndTag();
            }

            // end Head
            writer.RenderEndTag();
            writer.WriteLine();

            // CSS
            writer.AddAttribute(HtmlTextWriterAttribute.Href, "bootstrap.css");
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "text/css");
            writer.AddAttribute(HtmlTextWriterAttribute.Rel, "stylesheet");
            writer.RenderBeginTag(HtmlTextWriterTag.Link);
            writer.RenderEndTag();

            writer.AddAttribute(HtmlTextWriterAttribute.Href, "dasRad.css");
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "text/css");
            writer.AddAttribute(HtmlTextWriterAttribute.Rel, "stylesheet");
            writer.RenderBeginTag(HtmlTextWriterTag.Link);
            writer.RenderEndTag();
        }
        
    }
}

        
