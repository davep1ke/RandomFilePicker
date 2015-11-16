using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RandomFilePicker
{
    public partial class Monitor : Form
    {
        public Monitor()
        {
            InitializeComponent();
            PickThread.StartThread(this);
            SetTopLevel(true);
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog f = new FolderBrowserDialog();
            f.ShowDialog();
            if (!String.IsNullOrEmpty(f.SelectedPath))
            {
                PickRandomFile.addPath(f.SelectedPath); 
            }
        }

        private void btnStopThread_Click(object sender, EventArgs e)
        {
            PickThread.StopThread();
        }

        public void setStatText(String text)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    lbl_stats.Visible = true;
                    lbl_stats.Text = text;

                });

            }
            else
            {
                lbl_stats.Visible = true;
                lbl_stats.Text = text;
            }
        }


        public void CloseMe()
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    this.Close();
                });
            }
            else
            {
                this.Close();
            }
        }
    }
}