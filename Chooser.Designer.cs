namespace RandomFilePicker
{
    partial class Chooser
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
            this.components = new System.ComponentModel.Container();
            this.btngo = new System.Windows.Forms.Button();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnFolder = new System.Windows.Forms.Button();
            this.btn_Open_Only = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.listView1 = new System.Windows.Forms.ListView();
            this.colFolder = new System.Windows.Forms.ColumnHeader();
            this.colFile = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // btngo
            // 
            this.btngo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btngo.Location = new System.Drawing.Point(544, 185);
            this.btngo.Margin = new System.Windows.Forms.Padding(2);
            this.btngo.Name = "btngo";
            this.btngo.Size = new System.Drawing.Size(110, 30);
            this.btngo.TabIndex = 1;
            this.btngo.Text = "Go";
            this.btngo.UseVisualStyleBackColor = true;
            this.btngo.Click += new System.EventHandler(this.btngo_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearch.Location = new System.Drawing.Point(10, 194);
            this.txtSearch.Margin = new System.Windows.Forms.Padding(2);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(509, 20);
            this.txtSearch.TabIndex = 2;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            this.txtSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearch_KeyDown);
            // 
            // btnFolder
            // 
            this.btnFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFolder.Location = new System.Drawing.Point(544, 48);
            this.btnFolder.Margin = new System.Windows.Forms.Padding(2);
            this.btnFolder.Name = "btnFolder";
            this.btnFolder.Size = new System.Drawing.Size(110, 33);
            this.btnFolder.TabIndex = 3;
            this.btnFolder.Text = "Open Folder";
            this.btnFolder.UseVisualStyleBackColor = true;
            this.btnFolder.Click += new System.EventHandler(this.btnFolder_Click);
            // 
            // btn_Open_Only
            // 
            this.btn_Open_Only.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Open_Only.Location = new System.Drawing.Point(544, 11);
            this.btn_Open_Only.Margin = new System.Windows.Forms.Padding(2);
            this.btn_Open_Only.Name = "btn_Open_Only";
            this.btn_Open_Only.Size = new System.Drawing.Size(87, 33);
            this.btn_Open_Only.TabIndex = 4;
            this.btn_Open_Only.Text = "Open";
            this.btn_Open_Only.UseVisualStyleBackColor = true;
            this.btn_Open_Only.Click += new System.EventHandler(this.btn_Open_Only_Click);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(635, 11);
            this.button1.Margin = new System.Windows.Forms.Padding(2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(19, 33);
            this.button1.TabIndex = 5;
            this.button1.Text = "X";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btn_Delete_Click);
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colFolder,
            this.colFile});
            this.listView1.Location = new System.Drawing.Point(13, 13);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(506, 176);
            this.listView1.TabIndex = 6;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.MouseHover += new System.EventHandler(this.listView1_MouseHover);
            // 
            // colFolder
            // 
            this.colFolder.Text = "Folder";
            this.colFolder.Width = 250;
            // 
            // colFile
            // 
            this.colFile.Text = "File";
            this.colFile.Width = 246;
            // 
            // Chooser
            // 
            this.AcceptButton = this.btngo;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(663, 225);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btn_Open_Only);
            this.Controls.Add(this.btnFolder);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.btngo);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Chooser";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Chooser";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.Chooser_Shown);
            this.Activated += new System.EventHandler(this.Chooser_Activated);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btngo;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnFolder;
        private System.Windows.Forms.Button btn_Open_Only;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader colFolder;
        private System.Windows.Forms.ColumnHeader colFile;
    }
}