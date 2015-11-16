namespace RandomFilePicker
{
    partial class Monitor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnStopThread = new System.Windows.Forms.Button();
            this.lbl_stats = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnStopThread
            // 
            this.btnStopThread.BackColor = System.Drawing.SystemColors.Control;
            this.btnStopThread.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnStopThread.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStopThread.Location = new System.Drawing.Point(13, 13);
            this.btnStopThread.Margin = new System.Windows.Forms.Padding(4);
            this.btnStopThread.Name = "btnStopThread";
            this.btnStopThread.Size = new System.Drawing.Size(74, 28);
            this.btnStopThread.TabIndex = 0;
            this.btnStopThread.Text = "Stop";
            this.btnStopThread.UseVisualStyleBackColor = false;
            this.btnStopThread.Click += new System.EventHandler(this.btnStopThread_Click);
            // 
            // lbl_stats
            // 
            this.lbl_stats.AutoSize = true;
            this.lbl_stats.Location = new System.Drawing.Point(0, 42);
            this.lbl_stats.Name = "lbl_stats";
            this.lbl_stats.Size = new System.Drawing.Size(46, 17);
            this.lbl_stats.TabIndex = 1;
            this.lbl_stats.Text = "label1";
            this.lbl_stats.Visible = false;
            // 
            // Monitor
            // 
            this.AcceptButton = this.btnStopThread;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnStopThread;
            this.CausesValidation = false;
            this.ClientSize = new System.Drawing.Size(100, 60);
            this.ControlBox = false;
            this.Controls.Add(this.lbl_stats);
            this.Controls.Add(this.btnStopThread);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Location = new System.Drawing.Point(10, 10);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(100, 60);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(100, 60);
            this.Name = "Monitor";
            this.Opacity = 0.65D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "RFP";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStopThread;
        private System.Windows.Forms.Label lbl_stats;

    }
}

