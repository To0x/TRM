using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.UI;

class HtmlParser 
{
    HtmlTextWriter writer = null;

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
        writer.AddAttribute(HtmlTextWriterAttribute.Class, String.Format("col-md-{0} col-sm-{0}", GridRow.MAX_BOOTSTRAP_SIZE));
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
        foreach (GridRow row in rows) {
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
        //writer.AddAttribute("http-equiv", "refresh");
        //writer.AddAttribute("content", "0.1");
        //writer.RenderBeginTag(HtmlTextWriterTag.Meta);
        //writer.RenderEndTag();

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

    public void writeHtmlDocument(int i, List<GridRow> rows)
    {
        using (FileStream fs = new FileStream("test.html", FileMode.Open))
        {
            using (StreamWriter stringWriter = new StreamWriter(fs, Encoding.UTF8))
            {
                using (this.writer = new HtmlTextWriter(stringWriter))
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Html);
                    {
                        writeHead();
                        writeBody(rows);
                    }
                    writer.RenderEndTag();
                }
            }
        }
        Thread.Sleep(10);
    }


}
