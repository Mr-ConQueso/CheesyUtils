// opens external file in default viewer for that filetype
// for example: powerpoint file would open in powerpoint

using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace CheesyUtils
{
    public class OpenExternalFile : MonoBehaviour
    {

        // opens external file in default viewer
        public static void OpenFile(string fullPath)
        {
            UnityEngine.Debug.Log("opening:" + fullPath);

            if (File.Exists(fullPath))
            {
                try
                {
                    Process myProcess = new Process();
                    myProcess.StartInfo.FileName = fullPath;
                    myProcess.Start();
                    //				myProcess.WaitForExit();
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.Log(e);
                }
            }
        }
    }
}