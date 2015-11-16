using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RandomFilePicker
{
    public partial class Help : Form
    {
        public Help(String text)
        {
            InitializeComponent();
            this.txtHelp.Multiline = true;
            this.txtHelp.Text = text;
        }
    }
}
