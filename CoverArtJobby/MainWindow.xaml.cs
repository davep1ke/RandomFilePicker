using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.IO;
using System.Web;
using System.Net;
using Id3Lib;

namespace CoverArtJobby
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool imageUpdated = false;
        private bool tagsUpdated = false;
        private TagModel tagModel = new TagModel();
        private TagHandler tagHandler = null;
        


        public MainWindow()
        {
            InitializeComponent();
            populate_Treeview_Drives();

            //todo - store/load previous folder. 
            tryExpand(FolderBrowser.Items, @"C:\");
            tryExpand(FolderBrowser.Items, @"Users");
            tryExpand(FolderBrowser.Items, @"Phil");
            tryExpand(FolderBrowser.Items, @"Dropbox");
            tryExpand(FolderBrowser.Items, @"Music");
            tryExpand(FolderBrowser.Items, @"musictemp",true);
            

        }
        #region FolderBrowser
        
        public void tryExpand(ItemCollection rootCollection, string name, bool select = false)
        {
            
            //loop through the items and try and expand / populate the item if one exists
            foreach (TreeViewItem i in rootCollection)
            {
                if (i.Header.ToString().ToUpper() == name.ToUpper()) {
                    if (select)
                    {
                        i.IsSelected = true;
                    }
                    else
                    {
                        i.IsExpanded = true;
                    }
                
                }
                //if it is just our "loading.." message, drop out
                if (!((i.Items.Count == 1) && (i.Items[0] is string)))
                {
                    tryExpand(i.Items, name);
                }
            }

            FolderBrowser.UpdateLayout();
        }

        

        public void populate_Treeview_Drives()
        {


            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo driveInfo in drives)
                FolderBrowser.Items.Add(CreateFolderTreeItem(driveInfo));

        }

        private TreeViewItem CreateFolderTreeItem(object o)
        {
            TreeViewItem item = new TreeViewItem();
            item.Expanded += new RoutedEventHandler(TreeViewItem_Expanded);
            item.Selected += new RoutedEventHandler(TreeViewItem_Selected);
            item.Header = o.ToString();
            item.Tag = o;
            item.Items.Add("Loading...");
            return item;
        }

        public void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = e.Source as TreeViewItem;
            if ((item.Items.Count == 1) && (item.Items[0] is string))
            {
                item.Items.Clear();

                DirectoryInfo expandedDir = null;
                if (item.Tag is DriveInfo)
                    expandedDir = (item.Tag as DriveInfo).RootDirectory;
                if (item.Tag is DirectoryInfo)
                    expandedDir = (item.Tag as DirectoryInfo);
                try
                {
                    foreach (DirectoryInfo subDir in expandedDir.GetDirectories())
                        item.Items.Add(CreateFolderTreeItem(subDir));
                }
                catch { }
            }
        }


        public void TreeViewItem_Selected(object sender, RoutedEventArgs e)
        {
            populateFileList();
        }

        #endregion

        #region fileList
        public void populateFileList()
        {
            TreeViewItem selectedItem = FolderBrowser.SelectedItem as TreeViewItem;
            if (selectedItem != null && selectedItem.Tag is DirectoryInfo)
            {
                DirectoryInfo di = selectedItem.Tag as DirectoryInfo ;
                //foreach (FileInfo f in di.EnumerateFiles()) //todo - restrict to mp3s?
                //{
                    

                //}
                if (chk_recurse.IsChecked == true) 
                {
                    FileList.ItemsSource = di.EnumerateFiles("*", SearchOption.AllDirectories);
                }
                else
                {
                    FileList.ItemsSource = di.EnumerateFiles();
                }
            }
        }
        private void FileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            refreshCurrentTag();


        }

        
        #endregion

        #region currentTag
        public void refreshCurrentTag()
        {
            //loop through the selected files and grab the ID3 tags and info

            if (FileList.SelectedItems.Count > 0)
            {
                try
                {
                    FileInfo file = FileList.SelectedItems[0] as FileInfo;

                    tagModel = Id3Lib.TagManager.Deserialize(file.OpenRead());
                    tagHandler = new TagHandler(tagModel);


                    if (tagHandler.Picture != null)
                    {
                        SetImageFromBitmap(tagHandler.Picture, false);
                    }
                    else
                    {
                        Tag_Image.Source = null;
                    }
                    Tag_Album.Text = tagHandler.Album;
                    Tag_Artist.Text = tagHandler.Artist;
                    Tag_File.Text = file.Name;
                    Tag_Song.Text = tagHandler.Song;
                    imageUpdated = false;
                    tagsUpdated = false;

                }
                catch (Id3Lib.Exceptions.TagNotFoundException)
                {

                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }

            }
            else
            {
                //todo - lock fields
            }
        }


        private void SetImageFromBitmap(System.Drawing.Image image, bool updateID3)
        {
            
            Bitmap b = new Bitmap(image);
            BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(b.GetHbitmap(),
              IntPtr.Zero,
              System.Windows.Int32Rect.Empty,
              BitmapSizeOptions.FromWidthAndHeight(b.Width, b.Height));

            Tag_Image.Source = bs;
            Tag_ImageDims.Text = b.Width.ToString() + "x" + b.Height.ToString();

            if (updateID3)
            {
                Tag_Image.Tag = image;
                //System.Drawing.Image imageTag = System.Drawing.Image.FromHbitmap(b.GetHbitmap(), IntPtr.Zero);
                //Tag_Image.Tag = imageTag;
                imageUpdated = true;
            }
        }

        private void SetImageFromUri(Uri uri)
        {
            string fileName = System.IO.Path.GetTempFileName();
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    byte[] data = webClient.DownloadData(uri);
                    MemoryStream ms = new MemoryStream(data);
                    System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
                    SetImageFromBitmap(image, true);

                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }




        private void Tag_Image_Drop(object sender, DragEventArgs e)
        {
            //should be a URL or a file URI
            //http://stackoverflow.com/questions/8442085/receiving-an-image-dragged-from-web-page-to-wpf-window

            System.Windows.IDataObject data = e.Data;
            string[] formats = data.GetFormats();


            if (formats.Contains("text/html"))
            {

                var obj = data.GetData("text/html");
                string html = string.Empty;
                if (obj is string)
                {
                    html = (string)obj;
                }
                else if (obj is MemoryStream)
                {
                    MemoryStream ms = (MemoryStream)obj;
                    byte[] buffer = new byte[ms.Length];
                    ms.Read(buffer, 0, (int)ms.Length);
                    if (buffer[1] == (byte)0)  // Detecting unicode
                    {
                        html = System.Text.Encoding.Unicode.GetString(buffer);
                    }
                    else
                    {
                        html = System.Text.Encoding.ASCII.GetString(buffer);
                    }
                }
                // Using a regex to parse HTML, but JUST FOR THIS EXAMPLE :-)
                var match = new Regex(@"<img[^/]src=""([^""]*)""").Match(html);
                if (match.Success)
                {
                    Uri uri = new Uri(match.Groups[1].Value);
                    SetImageFromUri(uri);
                }
                else
                {
                    // Try look for a URL to an image, encoded (thanks google image search....)
                    match = new Regex(@"url=(.*?)&").Match(html);
                    //url=http%3A%2F%2Fi.imgur.com%2FK1lxb2L.jpg&amp
                    if (match.Success)
                    {
                        Uri uri = new Uri(Uri.UnescapeDataString(match.Groups[1].Value));
                        SetImageFromUri(uri);
                    }
                }
                
                


            }


        }


        private void btnReloadTag_Click(object sender, RoutedEventArgs e)
        {
            refreshCurrentTag();
        }

        #endregion

        //Load image
            //        _openFileDialog.Multiselect= false;
            //_openFileDialog.CheckFileExists = true;
            //_openFileDialog.CheckPathExists = true;
            //_openFileDialog.Title = "Select a picture";
            //_openFileDialog.Filter = "Picture Files(*.bmp;*.jpg;*.gif;*.png)|*.bpm;*.jpg;*.gif;*.png|Bitmap (*.bmp)|*.bmp|jpg (*.jpg)|*.jpg|jpeg (*.jpeg)|*.jpeg|gif (*.gif)|*.gif|gif (*.png)|*.png";
            //if(_openFileDialog.ShowDialog() == DialogResult.OK)
            //{ 
            //    using (FileStream stream = File.Open(_openFileDialog.FileName,FileMode.Open,FileAccess.Read,FileShare.Read))
            //    {
            //        byte[] buffer = new Byte[stream.Length];
            //        stream.Read(buffer,0,buffer.Length);
            //        if(buffer != null)
            //        {
            //            MemoryStream memoryStream = new MemoryStream(buffer,false);
            //            this._artPictureBox.Image = Image.FromStream(memoryStream);
            //        }
            //    }
            //}



        private void btn_Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnGuessTag_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_Search_File_Click(object sender, RoutedEventArgs e)
        {
            string filename = Tag_File.Text;
            //drop the extension

            var match = new Regex(@"(.*)\..+").Match(filename);
            if (match.Success)
            {
                filename = match.Groups[1].Value;
            }


            string URL = "https://www.google.co.uk/search?safe=off&source=lnms&tbm=isch&q=";
            URL += Uri.EscapeUriString(filename);

            //string cleaned = 
//            string URL = "https://www.google.co.uk/search?q=" + Tag_File.Text.Replace(" ","+") +"&safe=off&source=lnms&tbm=isch";
            System.Diagnostics.Process.Start(URL);

        }

        private void btnSaveTag_Click(object sender, RoutedEventArgs e)
        {
            
            if (imageUpdated)
            {
                
                System.Drawing.Image image = Tag_Image.Tag as System.Drawing.Image;
                //Bitmap b = Tag_Image.Tag as Bitmap;
                tagHandler.Picture = image;
            }

            if (tagsUpdated)
            {
                 tagHandler.Album = Tag_Album.Text ;
                 tagHandler.Artist = Tag_Artist.Text;
                 tagHandler.Song = Tag_Song.Text;
            }

            FileInfo fi = FileList.SelectedItems[0] as FileInfo;
            FileStream fs = fi.OpenWrite();
            TagManager.Serialize(tagModel, fs) ;
            fs.Close();
        }











    }
}
