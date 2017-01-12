using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TUI_Web
{
    public partial class MainView : Form
    {
        private bool running = false;
        private AppControler appControler = null;

        public event EventHandler EVENT_View_SaveClicked;

        public MainView()
        {
            InitializeComponent();
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            EVENT_View_SaveClicked?.Invoke(sender, e);
        }

        private void protectAllFields(bool protect = true)
        {
            if (protect)
            {
                txt_autoRefresh.Enabled = false;
                btn_setLocation.Enabled = false;
                //btn_save.Enabled = false;
                btn_start.Text = "Stop";
                lab_running.Text = "running";
                lab_running.Visible = true;
            }
            else
            {
                txt_autoRefresh.Enabled = true;
                btn_setLocation.Enabled = true;
                btn_save.Enabled = true;
                btn_start.Text = "Start";
                lab_running.Text = "";
                lab_running.Visible = false;
            }
        }

        private void btn_start_Click(object sender, EventArgs e)
        {
            if (txt_filePath.Text.Equals(""))
            {
                lab_running.Text = "No File Location!";
                lab_running.Visible = true;
                return;
            }

            if (!running)
            {
                protectAllFields();
                appControler = new AppControler(this, new Settings.SettingsControler(Int32.Parse(txt_autoRefresh.Text), txt_filePath.Text));
            }
            else
            {
                protectAllFields(false);
                appControler.close();
                
            }
            running = !running;
        }

        private void txt_autoRefresh_TextChanged(object sender, EventArgs e)
        {

        }

        private void MainView_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (running)
            {
                appControler.close();
            }
            System.Environment.Exit(0);
        }

        private void btn_setLocation_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.FileName = "output.html";
            saveDialog.DefaultExt = "html";
            saveDialog.Filter = "HTML File (.html)|*.html";

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                txt_filePath.Text = saveDialog.FileName;
            }

        }
    }
}
