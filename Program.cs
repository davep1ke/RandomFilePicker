using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace RandomFilePicker
{
    public static class Program
    {
        /// <summary>
        /// General program mode - are we picking a file randomly, showing a list of files, or quiting.
        /// </summary>
        enum pickModes
        {
            single = 1,
            threaded = 2,
            noPick = 3
        }

        /// <summary>
        /// List of follow-up strings that we are expecting.
        /// </summary>
        enum parseModes
        {
            Default = 1,
            exclude = 2,
            filetype = 3,
            repeatDur = 4,
            repeatCnt = 5,
            overrideext = 6,
            overrideapp = 7,
            loadfile = 8,
            cache_min = 9,
            cache_max = 10,
            cachefolder =11

        }
        private static pickModes PickMode = pickModes.single;
        private static parseModes ParseMode = parseModes.Default;

        private static List<String> commands = new List<string>();
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                //Load all the commands in args[] into a list so that we can add to them later if we get passed a filename,
                foreach (String s1 in args)
                {
                    commands.Add(s1);
                }
                string s = "";
                string lastext = "";

                if (args.Length > 0)
                {

                    while (commands.Count > 0)
                    {
                        s = commands[0];
                        s.Trim();
                        commands.RemoveAt(0);
                        switch (ParseMode)
                        {
                            //deafault mode
                            case parseModes.Default:
                                switch (s)
                                {
                                    case "-x":
                                        ParseMode = parseModes.exclude;
                                        break;

                                    case "-nr":
                                        PickRandomFile.recurse = false;
                                        break;

                                    case "-1":
                                        PickRandomFile.firstfileonly = true;
                                        break;

                                    case "-shuffle":
                                        PickRandomFile.shuffle = true;
                                        break;

                                    case "-stat":
                                        PickRandomFile.showStats = true;
                                        break;

                                    case "-i":
                                        PickRandomFile.ignoreMissingFolders = true;
                                        break;

                                    case "-f":
                                        ParseMode = parseModes.filetype;
                                        PickRandomFile.usingFileTypes = true;
                                        break;

                                    case "-?":
                                        Help helpwindow = new Help(

                                            "Usage: \n" +
                                            "<dir>" + "\t\t" + "Include directory" + "\n" +
                                            "-c" + "\t\t" + "Choose file after search" + "\n" +
                                            "-x <dir>" + "\t\t" + "Exclude directory" + "\n" +
                                            "-nr" + "\t\t" + "Disable recursing directories" + "\n" +
                                            "-f <list>" + "\t\t" + "Comma separated list of filetypes (e.g. .avi,.mpg,.mpeg)" + "\n" +
                                            "-r" + "\t\t" + "Repeat pick files " + "\n" +
                                            "-rc <cnt>" + "\t\t" + "How many times to repeat the pick operation (infinite if not specified)" + "\n" +
                                            "-rd <dur>" + "\t\t" + "Duration between pick attempts (seconds)" + "\n" +
                                            "-stat" + "\t\t" + "Show stats (for certain modes) in the window" + "\n" +
                                            "-cachestats" + "\t\t" + "Show cache / pick stats" + "\n" +
                                            "-1" + "\t\t" + "Pick first file in directory" + "\n" +
                                            "-o <ext> <app>" + "\t" + "Override app to open a filetype with" + "\n" +
                                            "-l <filename>" + "\t" + "Load a set of commands from a file" + "\n" +
                                            "-h" + "\t" + "Runs any spawned applications in the background" + "\n" +
                                            "-i" + "\t" + "Silently ignore any missing directories" + "\n" +
                                            "-shuffle" + "\t" + "Picks one at a time until all files have been picked once" + "\n" +
                                            "-cache <min> <max>" + "\t" + "Cache file access for min < x < max hours" + "\n" +
                                            "-cachefolder <directory>" + "\t" + "Folder where cache data should be written" + "\n" +
                                            "--" + "\t" + "Null argument, use in scripts as a placeholder" + "\n" +
                                            "\n" +
                                            "Example File:" + "\n" +
                                            "-f" + "\n" +
                                            ".avi,.mpg,.mpeg,.mkv,.mp4,.rm" + "\n" +
                                            @"E:\eps\30 rock" + "\n" +
                                            @"E:\eps\drawn together" + "\n" +
                                            @"E:\eps\Harvey Birdman" + "\n"
                                            );
                                        Application.Run(helpwindow);
                                        PickMode = pickModes.noPick;
                                        break;

                                    case "-h":
                                        PickRandomFile.hideSpawnedWindows = true;
                                        break;

                                    case "-c":
                                        PickRandomFile.showChooser = true;
                                        break;

                                    case "-r":
                                        PickMode = pickModes.threaded;
                                        break;

                                    case "-rc":
                                        ParseMode = parseModes.repeatCnt;
                                        break;

                                    case "-rd":
                                        ParseMode = parseModes.repeatDur;
                                        break;

                                    case "-o":
                                        ParseMode = parseModes.overrideext;
                                        break;

                                    case "-l":
                                        ParseMode = parseModes.loadfile;
                                    break;
                                        

                                    case "-cachestats":
                                        PickRandomFile.showCacheStats = true;
                                    break;

                                    case "-cache":
                                        PickRandomFile.useCache = true;
                                        ParseMode = parseModes.cache_min;
                                    break;

                                    case "-cachefolder":
                                        ParseMode = parseModes.cachefolder;
                                    break;

                                    case "--":
                                        //Nothing needed here
                                        break;

                                    default:
                                        PickRandomFile.addPath(s);
                                        break;
                                }
                                break;
                            //exclude directory mode
                            case parseModes.exclude:
                                ParseMode = parseModes.Default;
                                PickRandomFile.excludePath(s);
                                break;

                            case parseModes.cachefolder:
                                ParseMode = parseModes.Default;
                                PickRandomFile.cachefolder = s;
                                break;

                            case parseModes.filetype:
                                ParseMode = parseModes.Default;
                                PickRandomFile.usingFileTypes = true;
                                PickRandomFile.fileFilter = s;
                                string[] split = s.Split(',');
                                foreach (string filetype in split)
                                {
                                    PickRandomFile.addFileType(filetype);
                                }
                                break;

                            case parseModes.repeatDur:
                                ParseMode = parseModes.Default;
                                int duration;
                                if (Int32.TryParse(s, out duration))
                                {

                                    PickRandomFile.threadWaitDuration = duration * 1000;
                                }
                                break;

                            case parseModes.repeatCnt:
                                ParseMode = parseModes.Default;
                                int count;
                                if (Int32.TryParse(s, out count))
                                {

                                    PickRandomFile.threadRepeatCount = count;
                                }
                                break;

                            case parseModes.overrideext:
                                ParseMode = parseModes.overrideapp;
                                lastext = s;
                                break;

                            case parseModes.overrideapp:
                                ParseMode = parseModes.Default;
                                PickRandomFile.addOverride(lastext, s);
                                break;

                            case parseModes.loadfile:
                                //load a file one row at a time into args[]
                                ParseMode = parseModes.Default;
                                StreamReader f = new StreamReader(s);
                                string line;
                                while ((line = f.ReadLine()) != null)
                                {
                                    commands.Add(line);
                                }
                                break;

                            case parseModes.cache_min:
                                ParseMode = parseModes.cache_max;
                                PickRandomFile.cacheHoursMin = Convert.ToInt32(s);
                                break;

                            case parseModes.cache_max:
                                ParseMode = parseModes.Default;
                                PickRandomFile.cacheHoursMax = Convert.ToInt32(s);
                                break;
                        }

                    }
                    #region pickModes
                    switch (PickMode)
                    {
                        case pickModes.single:
                            PickRandomFile.scanDirectories();
                            if (PickRandomFile.showChooser)
                            {
                                PickRandomFile.pickChoice();
                            }
                            else
                            {
                                PickRandomFile.pickRandom();
                            }
                            break;

                        case pickModes.threaded:
                            Application.Run(new Monitor());

                            break;

                    }
                    #endregion

                }
                //else
                //{


                //    Application.EnableVisualStyles();
                //    Application.SetCompatibleTextRenderingDefault(false);
                //    Application.Run(new Monitor());
                //}
            }

            catch (Exception e)
            {
                MessageBox.Show(e.StackTrace + "\n\n" + e.InnerException + "\n\n" + e.Source + "\n\n" + e.Data , e.Message);
            }
        }
    }


}