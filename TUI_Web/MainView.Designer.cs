namespace TUI_Web
{
    partial class MainView
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.btn_start = new System.Windows.Forms.Button();
            this.btn_save = new System.Windows.Forms.Button();
            this.txt_autoRefresh = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_filePath = new System.Windows.Forms.TextBox();
            this.btn_setLocation = new System.Windows.Forms.Button();
            this.lab_running = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btn_start
            // 
            this.btn_start.Location = new System.Drawing.Point(206, 173);
            this.btn_start.Name = "btn_start";
            this.btn_start.Size = new System.Drawing.Size(75, 47);
            this.btn_start.TabIndex = 0;
            this.btn_start.Text = "Start";
            this.btn_start.UseVisualStyleBackColor = true;
            this.btn_start.Click += new System.EventHandler(this.btn_start_Click);
            // 
            // btn_save
            // 
            this.btn_save.Location = new System.Drawing.Point(15, 173);
            this.btn_save.Name = "btn_save";
            this.btn_save.Size = new System.Drawing.Size(75, 47);
            this.btn_save.TabIndex = 1;
            this.btn_save.Text = "Save";
            this.btn_save.UseVisualStyleBackColor = true;
            this.btn_save.Click += new System.EventHandler(this.btn_save_Click);
            // 
            // txt_autoRefresh
            // 
            this.txt_autoRefresh.Location = new System.Drawing.Point(181, 21);
            this.txt_autoRefresh.Name = "txt_autoRefresh";
            this.txt_autoRefresh.Size = new System.Drawing.Size(100, 20);
            this.txt_autoRefresh.TabIndex = 2;
            this.txt_autoRefresh.Text = "1000";
            this.txt_autoRefresh.TextChanged += new System.EventHandler(this.txt_autoRefresh_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(144, 26);
            this.label1.TabIndex = 3;
            this.label1.Text = "AutoRefresh Browser [msec.]\r\n0 = disable";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "save Path";
            // 
            // txt_filePath
            // 
            this.txt_filePath.Location = new System.Drawing.Point(181, 68);
            this.txt_filePath.Name = "txt_filePath";
            this.txt_filePath.ReadOnly = true;
            this.txt_filePath.Size = new System.Drawing.Size(100, 20);
            this.txt_filePath.TabIndex = 5;
            // 
            // btn_setLocation
            // 
            this.btn_setLocation.Location = new System.Drawing.Point(287, 66);
            this.btn_setLocation.Name = "btn_setLocation";
            this.btn_setLocation.Size = new System.Drawing.Size(33, 23);
            this.btn_setLocation.TabIndex = 6;
            this.btn_setLocation.Text = "...";
            this.btn_setLocation.UseVisualStyleBackColor = true;
            this.btn_setLocation.Click += new System.EventHandler(this.btn_setLocation_Click);
            // 
            // lab_running
            // 
            this.lab_running.AutoSize = true;
            this.lab_running.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lab_running.Location = new System.Drawing.Point(107, 110);
            this.lab_running.Name = "lab_running";
            this.lab_running.Size = new System.Drawing.Size(104, 31);
            this.lab_running.TabIndex = 7;
            this.lab_running.Text = "running";
            this.lab_running.Visible = false;
            // 
            // MainView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(343, 241);
            this.Controls.Add(this.lab_running);
            this.Controls.Add(this.btn_setLocation);
            this.Controls.Add(this.txt_filePath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txt_autoRefresh);
            this.Controls.Add(this.btn_save);
            this.Controls.Add(this.btn_start);
            this.Name = "MainView";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainView_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_start;
        private System.Windows.Forms.Button btn_save;
        private System.Windows.Forms.TextBox txt_autoRefresh;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_filePath;
        private System.Windows.Forms.Button btn_setLocation;
        private System.Windows.Forms.Label lab_running;
    }
}

