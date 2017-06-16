using System;
using System.Diagnostics;
using System.Collections.Generic;
//using System.Linq;
using System.Xml.Serialization;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace RandomFilePicker
{

       

    public static class PickRandomFile
    {

        private static double tim_dir = 0;
        private static double tim_file = 0;
        private static int dirs_scanned = 0;
        private static int files_scanned = 0;


        public static List<DirectoryInfo> folderList = new List<DirectoryInfo>();
        public static List<DirectoryInfo> excludeList = new List<DirectoryInfo>();
        public static List<string> fileTypeList = new List<string>();
        public static List<extAppPair> appOverrides = new List<extAppPair>();

        public static List<FileShortInfo> allFiles = new List<FileShortInfo>();
        //if shuffle mode enabled, files that have been picked get moved here, then the lists are swapped round once all 
        //have been picked. Prevents same files from being picked multiple times.
        public static List<FileShortInfo> pickedFiles = new List<FileShortInfo>();

        public static bool recurse = true;
        public static bool usingFileTypes = false;
        public static bool firstfileonly = false;
        public static bool hideSpawnedWindows = false;
        public static bool shuffle = false;
        public static bool showStats = false;
        public static bool useCache = false;
        public static bool showCacheStats = false;
        public static bool ignoreMissingFolders = false;

        public static int cacheHoursMin = 0;
        public static int cacheHoursMax = 0;
        public static int threadWaitDuration = 0;
        public static int threadRepeatCount = -1;
        public static bool showChooser = false;
        public static string fileFilter = "";
        public static string cachefolder = "";

        private static FileCache cache;

        public static void scanDirectories()
        {
            try
            {
                tim_file = 0;
                tim_dir = 0;
                dirs_scanned = 0;
                files_scanned = 0;
                DateTime time_start = DateTime.Now;

                //if in cache mode, load the cache.
                try
                {
                    if (useCache) { cache = new FileCache(); }
                }
                catch (Exception e)
                {
                    throw new Exception("Error loading cache", e);
                }

                //add all files from our pathlist to a list
                List<string> folders = new List<string>();
                try
                {
                    foreach (DirectoryInfo d in PickRandomFile.folderList)
                    {
                        if (d.Exists)
                        {
                            try
                            {
                                folders.Add(d.FullName);
                            }
                            catch
                            {
                                MessageBox.Show("Directory didnt have a fullname?");
                                MessageBox.Show(d.Name);
                            }
                            //add all subfolders
                            if (recurse)
                            {
                                foreach (DirectoryInfo di in d.GetDirectories("*", SearchOption.AllDirectories))
                                {
                                    if (di.Exists) { folders.Add(di.FullName); }
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Directory " + d.FullName + " doesnt exist");
                        }
                    }
                    tim_dir = DateTime.Now.Subtract(time_start).Seconds;
                }
                catch (Exception e)
                {
                    throw new Exception("Error during directory sweep", e);
                }
                //Now check for files (or use cache where appropriate)
                try
                {
                    foreach (String s in folders)
                    {
                        List<FileShortInfo> files = null;
                        if (useCache)
                        {
                            files = cache.getFilesFromCache(s);
                        }

                        //get the files directly if we havent found them in the cache, or we arent using it
                        if (files == null)
                        {
                            getFilesFromFolder(s);
                        }
                        else
                        {
                            try
                            {
                                foreach (FileShortInfo f in files)
                                {
                                    if (addFileToResults(f)) { allFiles.Add(f); }
                                }
                            }
                            catch (Exception e)
                            {
                                throw new Exception("Error transferring file to results object", e);
                            } 
                        }

                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Error scanning for files", e);
                }

                if (showCacheStats)
                {
                    MessageBox.Show(
                        "Total Time: " + DateTime.Now.Subtract(time_start).TotalSeconds.ToString() + "\n" +
                        "Files " + files_scanned.ToString() + ", Directories " + dirs_scanned.ToString() + "\n" +
                        "Files Added: " + allFiles.Count.ToString() + "\n" +
                        "File Time: " + tim_file.ToString() + "\n" +
                        "Dir Time: " + tim_dir.ToString() +
                        (useCache ? "\n" + cache.getStats() : "")
                        );
                }
                if (useCache) { cache.saveCache(); }
            }
            catch (Exception e)
            {
                throw new Exception("Error during scanDirectories", e);
            }
                
        }
        public static void pickRandom()
        {
            
            //pick a random file from our list
            Random r = new Random(DateTime.Now.Millisecond + DateTime.Now.Second + DateTime.Now.Minute);

            //are we shuffling, and at the end of the shuffled list?
            if (shuffle && allFiles.Count == 0 && pickedFiles.Count > 0)
            {
                allFiles = pickedFiles;
                pickedFiles = new List<FileShortInfo>();
            }

            
            if (allFiles.Count > 0)
            {
                FileShortInfo fi_s = allFiles[r.Next(allFiles.Count)];
                FileInfo fi = new FileInfo(fi_s.FullPath);
                String overrideApp = isExtensionOverridden(fi.Extension);
                ProcessStartInfo startinfo = new ProcessStartInfo();

                if (shuffle)
                {
                    pickedFiles.Add(fi_s);
                    allFiles.Remove(fi_s);
                }


                if(overrideApp == "")
                {
                    startinfo = new ProcessStartInfo(fi.FullName);

                }
                else{
                    startinfo = new ProcessStartInfo(overrideApp, "\"" + fi.FullName + "\"");
                }
                if (hideSpawnedWindows) 
                { 
                    startinfo.CreateNoWindow = true;
                    startinfo.WindowStyle = ProcessWindowStyle.Hidden;
                    startinfo.UseShellExecute = false;
                }

                try
                {
                    Process.Start(startinfo);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Couldnt start program - " + overrideApp + " - " + fi.FullName + " - " + e.Message + " " + e.StackTrace, "Couldnt start program");
                }

            }
        }
        public static void pickChoice()
        {
            Chooser c = new Chooser();
            c.ShowDialog();
            if (c.DialogResult == DialogResult.OK)
            {
                openFile(c.result);
            }
        }

        public static void openFile(FileShortInfo fi)
        {
            String overrideApp = isExtensionOverridden(fi.Extension);

            if (overrideApp == "")
            {
                System.Diagnostics.Process.Start(fi.FileName);
            }
            else
            {
                System.Diagnostics.Process.Start(overrideApp, "\"" + fi.FullPath + "\"");
            }
        }

        /// <summary>
        /// Scan directories recursively using the Kernel32 shite at the bottom.
        /// </summary>
        /// <param name="dirName"></param>
        static void getFilesFromFolder(String dirName)
        {
            IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
            WIN32_FIND_DATAW findData;
            IntPtr findHandle = INVALID_HANDLE_VALUE;

            List<FileShortInfo> filesForCache = new List<FileShortInfo>();

            try
            {
                findHandle = FindFirstFileW(dirName + @"\*", out findData);
                if (findHandle != INVALID_HANDLE_VALUE)
                {
                    do
                    {
                        //ignore the "up" folders
                        //ignore subfolders (recursion handled above now)
                        if (findData.cFileName == "."
                            || findData.cFileName == ".."
                            || PickRandomFile.isDirectoryExcluded(findData.cFileName)
                            || (findData.dwFileAttributes & FileAttributes.Directory) != 0)
                            continue;

                        string fullpath = dirName + (dirName.EndsWith("\\") ? "" : "\\") + findData.cFileName;

                        /*/Directory - recurse
                        if ((findData.dwFileAttributes & FileAttributes.Directory) != 0 && recurse == true)
                        {
                            dirs_scanned++;
                             getFilesFromFolder(fullpath);
                        }*/

                        DateTime file_start = DateTime.Now;
                        files_scanned++;
                        FileShortInfo f = new FileShortInfo()
                        {
                            CreatedDate = FTimeToDateTime(findData.ftCreationTime),
                            ModifiedDate = FTimeToDateTime(findData.ftLastWriteTime),
                            FullPath = fullpath,
                            Extension = fullpath.Substring(fullpath.LastIndexOf(".")),
                            FileName = fullpath.Substring(fullpath.LastIndexOf("\\") + 1),
                            Directory = fullpath.Substring(0, fullpath.LastIndexOf("\\")),
                            Size = findData.nFileSizeLow
                        };


                        //add file to results
                        if (addFileToResults(f))
                        {
                            allFiles.Add(f);
                            filesForCache.Add(f);
                        }

                        tim_file += DateTime.Now.Subtract(file_start).TotalSeconds;
                    }
                    while (FindNextFile(findHandle, out findData));

                    //now update the cache
                    if (useCache) {cache.addCacheData(dirName, PickRandomFile.fileFilter, filesForCache);}

                }
            }
            catch (Exception e)
            {
                if (!ignoreMissingFolders)
                {
                    MessageBox.Show("Exception when scanning folder" + dirName + "\n" + e.Message + "\n\n" + e.StackTrace + "\n\n" + e.Source);
                }
            }
            finally
            {
                if (findHandle != INVALID_HANDLE_VALUE) FindClose(findHandle);
            }

        }

        static bool addFileToResults(FileShortInfo f)
        {
            try
            {
                if (usingFileTypes)
                {
                    if (PickRandomFile.isExtensionIncluded(f.Extension))
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
                
            }
            catch (Exception e)
            {
                throw new Exception("Error adding file to results", e);
            }
            return false;
        }


        /*static void getFilesFromFolder(DirectoryInfo d)
        {
             try
             {      
                 if (!PickRandomFile.isDirectoryExcluded(d.FullName))
                 {
                     //loop through any subdirs
                     if (recurse == true)
                     {
                         foreach (DirectoryInfo subdir in d.GetDirectories())
                         {
                             //recurse and all all subdirs
                             try
                             {
                                 getFilesFromFolder(subdir);
                             }
                             catch (UnauthorizedAccessException)
                             {
                                 //ignore it..
                             }
                         }
                     }

                     //add files from current folder
                     FileInfo[] fiArray = d.GetFiles();
                     if (firstfileonly && fiArray.Length > 0)
                     {
                         allFiles.Add(fiArray[0]);
                     }

                     else
                     {

                         foreach (FileInfo f in fiArray)
                         {
                             if (usingFileTypes)
                             {
                                 if (PickRandomFile.isExtensionIncluded(f.Extension))
                                 {
                                     allFiles.Add(f);
                                 }
                             }
                             else
                             {
                                 allFiles.Add(f);
                             }
                         }
                     }
                 }
             }
             catch (DirectoryNotFoundException) { };


        }*/





        public static void addPath(String folderPath)
        {
            DirectoryInfo d = new DirectoryInfo(folderPath);
            folderList.Add(d);
        }
        public static void excludePath(String folderPath)
        {
            DirectoryInfo d = new DirectoryInfo(folderPath);
            excludeList.Add(d);
        }

        public static void addOverride(string ext, string app)
        {
            if (!ext.StartsWith(".")) ext = "." + ext;
            appOverrides.Add(new extAppPair(ext, app));
        }

        public static void addFileType(String fileType)
        {
            if (!fileType.StartsWith(".")) fileType = "." + fileType;
            fileTypeList.Add(fileType);
        }

        public static bool isExtensionIncluded(string extension)
        {
            if (fileTypeList.Contains(extension))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool isDirectoryExcluded(string fullname)
        {
            foreach (DirectoryInfo di in excludeList)
            {
                if (di.FullName == fullname)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// returns blank string for a non-overridden ext, otherwise app
        /// </summary>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static string isExtensionOverridden(string ext)
        {
            foreach (extAppPair eap in appOverrides)
            {
                if (eap.extension == ext)
                {
                    return eap.application;
                }
            }

            return "";
        }

        //Kernel32 wrappers for fast scanning
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr FindFirstFileW(string lpFileName, out WIN32_FIND_DATAW lpFindFileData);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern bool FindNextFile(IntPtr hFindFile, out WIN32_FIND_DATAW lpFindFileData);

        [DllImport("kernel32.dll")]
        private static extern bool FindClose(IntPtr hFindFile);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct WIN32_FIND_DATAW
        {
            public FileAttributes dwFileAttributes;
            internal System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
            internal System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
            internal System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
            public int nFileSizeHigh;
            public int nFileSizeLow;
            public int dwReserved0;
            public int dwReserved1;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string cFileName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
            public string cAlternateFileName;
        }

        private static DateTime FTimeToDateTime(System.Runtime.InteropServices.ComTypes.FILETIME filetime)
        {
            long highBits = filetime.dwHighDateTime;
            highBits = highBits << 32;
            return DateTime.FromFileTimeUtc(highBits + (long)filetime.dwLowDateTime);
        }
    }
    [Serializable]
    public class FileShortInfo
    {
        public string FullPath;
        public string Extension;
        public string FileName;
        public string Directory;
        public int Size;
        public DateTime ModifiedDate;
        public DateTime CreatedDate;
    }
}
