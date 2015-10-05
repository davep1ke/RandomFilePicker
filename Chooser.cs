using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace RandomFilePicker
{

    public partial class Chooser : Form
    {
        private List<FileShortInfo> allFiles = new List<FileShortInfo>();
        public FileShortInfo result;
        public Chooser()
        {
            allFiles.AddRange(PickRandomFile.allFiles);
            InitializeComponent();

            populateListView("");
            

            txtSearch.Focus();
        }

        private void populateListView(string filterString)
        {
            listView1.BeginUpdate();
            listView1.Items.Clear();

            foreach (FileShortInfo fi in allFiles)
            {
                if (filterString == "" || 
                    fi.FullPath.ToUpper().Contains(filterString.ToUpper()) ||
                    fi.FullPath.ToUpper().Contains(filterString.ToUpper()))
                {
                    
                    ListViewItem li = new ListViewItem(fi.Directory);
                    li.SubItems.Add(fi.FileName);
                    li.Tag = fi;
                    listView1.Items.Add(li);
                
                }
                listView1.Refresh();
            }
            listView1.EndUpdate();
        }

        private void btngo_Click(object sender, EventArgs e)
        {
            if ( listView1.SelectedItems.Count == 1)
            {
                DialogResult = DialogResult.OK;
                result = (FileShortInfo)listView1.SelectedItems[0].Tag;
            }
            else
            {
                DialogResult = DialogResult.Abort;
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (txtSearch.Text.Length > 1)
            {
                populateListView(txtSearch.Text);
            }
        }

       

        private void Chooser_Activated(object sender, EventArgs e)
        {
            txtSearch.Focus();
        }

        private void Chooser_Shown(object sender, EventArgs e)
        {
            txtSearch.Focus();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            int i = -1;
            if (listView1.SelectedItems.Count > 0 && listView1.SelectedIndices[0] > 0) { i = listView1.SelectedIndices[0];}

            if (e.KeyData == Keys.Up && i > 0)
            {
                //deselect everything
                foreach (ListViewItem li in listView1.Items) {li.Selected = false;}
                listView1.Items[i-1].Selected = true;
            }
            else if (e.KeyData == Keys.Down && i >= 0 && i < listView1.Items.Count -1)
            {
                //deselect everything
                foreach (ListViewItem li in listView1.Items) { li.Selected = false; }
                listView1.Items[i + 1].Selected = true;
            }
        }

        private void btnFolder_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem li in listView1.SelectedItems)
            {
                FileShortInfo fi = (FileShortInfo)li.Tag;
                System.Diagnostics.Process.Start("Explorer.exe", fi.Directory);
            }                
        }

        private void btn_Open_Only_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem li in listView1.SelectedItems)
            {
                PickRandomFile.openFile((FileShortInfo)li.Tag);
            }
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                PickRandomFile.openFile((FileShortInfo)listView1.SelectedItems[0].Tag);
            }
        }

        private void btn_Delete_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem li in listView1.SelectedItems)
            {
                FileShortInfo fi = (FileShortInfo)li.Tag;
                DialogResult r = MessageBox.Show("Are you sure you want to delete this file / these files? \n\r" + fi.FullPath + "\n\r\n\rNote that the file will remain in the list until you search again", "Are you sure?", MessageBoxButtons.YesNo);

                if (r == DialogResult.Yes)
                {
                    FileInfo ffi = new FileInfo(fi.FullPath);
                    if (ffi.Exists)
                    {
                        ffi.Delete();
                    }

                }
            }
        }



        private void listView1_MouseHover(object sender, EventArgs e)
        {
            ListViewItem li = listView1.GetItemAt(ListBox.MousePosition.X, ListBox.MousePosition.Y);
            if (li != null)
            {
                FileShortInfo fi = (FileShortInfo)li.Tag;

                toolTip1.SetToolTip(listView1, fi.Directory);
                toolTip1.Show(fi.Directory, this, 10000);
            }
        }
    }
}