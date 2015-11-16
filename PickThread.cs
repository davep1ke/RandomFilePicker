using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Threading;

namespace RandomFilePicker
{
    static class PickThread
    {
        private static Thread mainThread;
        private static Boolean stopFlag = false;
        private static Monitor mainWindow;

        #region constructor
        public static void StartThread(Monitor monitorWindow)
        {
            mainWindow = monitorWindow;
            mainThread = new Thread(new ThreadStart(mainThreadProcess));
            mainThread.Start();
        }
        #endregion

        #region Thread
        private static void mainThreadProcess()
        {
            int repeats = 0;
            PickRandomFile.scanDirectories();
            while (stopFlag == false && repeats != PickRandomFile.threadRepeatCount)
            {
                PickRandomFile.pickRandom();
                Thread.Sleep(PickRandomFile.threadWaitDuration);
                repeats++;

                if (PickRandomFile.showStats) 
                {
                    String text = "";
                    //if its a repeat-until pick, show the number remaining.
                    if (PickRandomFile.threadRepeatCount != -1) { text += "R" + repeats.ToString() + "\\" + PickRandomFile.threadRepeatCount.ToString() + " "; }
                    //if its a shuffle, show the folder stats
                    if (PickRandomFile.shuffle) { text += "F" + PickRandomFile.allFiles.Count.ToString(); }
                    mainWindow.setStatText(text);
    
                }


            }
            mainWindow.CloseMe();
        }

        #endregion

        public static void StopThread()
        {
            stopFlag = true;
            mainWindow.Close();
        }
    }
}
