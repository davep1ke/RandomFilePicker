using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace RandomFilePicker
{
    public class FileCache
    {
        private int hits = 0;
        private int missed = 0;
        private int cache_Start = 0;
        private int cache_stale = 0;
        string status = "uninitialised";

        

        [Serializable]
        private class DirectoryCacheInfo
        {
            public String path;
            public DateTime expiry;
            public String extensionFilter = ""; //can be "" if we are not using extension filters
            public Guid Guid;
            public int fileCount = 0;
        }

        //for putting things into and out of XML as it complains about root nodes if you do a List
        [Serializable]
        public class FileCacheInfo
        {
            public Guid dirGuid;
            public List<FileShortInfo> files;
        }


        public XmlSerializer xmlSer = new XmlSerializer(typeof(FileCacheInfo));

        private List<DirectoryCacheInfo> theCache = new List<DirectoryCacheInfo>();
        private List<FileCacheInfo> theFileCache = new List<FileCacheInfo>();

        FileInfo cacheFile;
        String cacheDirectory = "";
        FileInfo lockFile;

        public FileCache()
        {
            if (PickRandomFile.cachefolder == "")
            {
                cacheDirectory = new Uri(System.IO.Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().CodeBase)).LocalPath;
            }
            else
            {
                cacheDirectory = PickRandomFile.cachefolder;
            }
            status = "unavailable";
        
            cacheFile = new FileInfo(cacheDirectory + "\\RandomFilePicker.cache");
            lockFile = new FileInfo(cacheDirectory + "\\RandomFilePicker.lock");

            //TODO try and lock

            bool ignoreCache = false;
            while (lockFile.Exists && status == "unavailable" && ignoreCache == false)
            {
                DialogResult r = MessageBox.Show("Cache Locked. Retry, Ignore (skip cache), or Abort?", "Cache Locked", MessageBoxButtons.AbortRetryIgnore);

                //r == DialogResult.Retry - will loop
                if (r == DialogResult.Abort) { Application.Exit(); }
                if (r == DialogResult.Ignore)
                {
                    ignoreCache = true;
                    PickRandomFile.useCache = false;
                }
            }

            
            if (!ignoreCache)
            {
                //if there is no existing cache, then the cache is ready (but unpopulated). Should be written on exit.
                if (cacheFile.Exists) 
                {
                    //TODO lock cache


                    //load cache
                    using (Stream stream = File.Open(cacheFile.FullName, FileMode.Open))
                    {
                        var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                        theCache = (List<DirectoryCacheInfo> )binaryFormatter.Deserialize(stream);
                    }

                    cache_Start = theCache.Count;

                    //expire anything that is stale. 
                    List<DirectoryCacheInfo> stale = new List<DirectoryCacheInfo>();
                    foreach (DirectoryCacheInfo dic in theCache)
                    {
                        if (DateTime.Compare(DateTime.Now, dic.expiry) > 0)
                        {
                            stale.Add(dic);
                        }
                    }
                    cache_stale = stale.Count;
                    foreach (DirectoryCacheInfo dic in stale)
                    {
                        theCache.Remove(dic);
                    }

                }

                status = "ready";

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="directoryName"></param>
        /// <returns>Null if not in cache</returns>
        public List<FileShortInfo> getFilesFromCache(String directoryName)
        {
            //TODO check cache status first. should be ready
            foreach (DirectoryCacheInfo dic in theCache)
            {
                if (dic.path == directoryName && dic.extensionFilter == PickRandomFile.fileFilter)
                {
                    hits++;
                    return getCachedFiles(dic);
                }
            }
            missed++;
            return null;
        }

        /// <summary>
        /// Loads the cache for a view of a folder when we need it. 
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        private List<FileShortInfo> getCachedFiles(DirectoryCacheInfo dir)
        {
            if (dir.fileCount == 0) { return new List<FileShortInfo>(); }
            else
            {

                //load cache
                try
                {

                    FileCacheInfo tempFiles = new FileCacheInfo();
                    string cacheFilePath = cacheDirectory + "\\" + dir.Guid.ToString();
                    using (StreamReader stream = new StreamReader(cacheFilePath, Encoding.UTF8))
                    {
                        tempFiles = (FileCacheInfo)xmlSer.Deserialize(stream);
                    }
                    return tempFiles.files;
                }
                catch (Exception e)
                {
                    throw new Exception("Error reading a set of cache info", e);
                }
            }
        }

        public void addCacheData(String path, String extFilter, List<FileShortInfo> files)
        {
            //TODO check cache status first. should be ready
            //TODO theoretically this shouldnt exist in the cache. we should probably remove it anyway if it does.
            
            Random r = new Random(DateTime.Now.Millisecond + DateTime.Now.Second + DateTime.Now.Minute);

            Guid g = Guid.NewGuid();

            theCache.Add(new DirectoryCacheInfo()
            {
                fileCount = files.Count,
                Guid = g,
                path = path,
                extensionFilter = extFilter,
                expiry = DateTime.Now.AddHours(r.Next(PickRandomFile.cacheHoursMin, PickRandomFile.cacheHoursMax))

            });

            theFileCache.Add(new FileCacheInfo()
            {
                dirGuid = g,
                files = files
            });
            
            
        }

        public void saveCache()
        {

            //save the FULL list of cached files (including ones we didnt open).
            using (Stream stream = File.Open(cacheFile.FullName, FileMode.Create))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, theCache);
            }
            
            //save all opened file lists. 
            foreach (FileCacheInfo fci in theFileCache)
            {
                if (fci.files.Count > 0)
                {
                    string cacheFilePath = cacheDirectory + "\\" + fci.dirGuid.ToString();
                    StreamWriter stream = new StreamWriter(cacheFilePath, false, Encoding.UTF8);
                    xmlSer.Serialize(stream, fci);
                    stream.Close();
                }
            }


            //TODO - clear lock.
            status = "unavailable";
        }

        public string getStats()
        {
            return
                "Cache Expired: " + cache_stale.ToString() + "\n" +
                "Cache Hits: " + hits.ToString() + ", Misses:" + missed.ToString() + "\n" +
                "Cache Size Start: " + cache_Start.ToString() + "\n" +
                "Cache Size End: " + theCache.Count.ToString();
        }
    }
}
