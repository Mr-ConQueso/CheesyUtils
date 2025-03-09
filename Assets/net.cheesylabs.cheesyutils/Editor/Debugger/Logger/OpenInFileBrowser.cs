// http://wiki.unity3d.com/index.php/OpenInFileBrowser
// CC BY-SA 3.0 http://creativecommons.org/licenses/by-sa/3.0/

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace CheesyUtils.Editor
{
    public static class OpenInFileBrowser
    {
        private static bool IsInMacOS => SystemInfo.operatingSystem.IndexOf("Mac OS", StringComparison.Ordinal) != -1;

        private static bool IsInWinOS => SystemInfo.operatingSystem.IndexOf("Windows", StringComparison.Ordinal) != -1;

        private static bool IsInLinuxOS => SystemInfo.operatingSystem.IndexOf("Linux", StringComparison.Ordinal) != -1;

        private static void OpenInMac(string path)
        {
            bool openInsidesOfFolder = false;

            // try mac
            string macPath = path.Replace("\\", "/"); // mac finder doesn't like backward slashes

            if (Directory
                .Exists(macPath)) // if path requested is a folder, automatically open insides of that folder
            {
                openInsidesOfFolder = true;
            }

            if (!macPath.StartsWith("\""))
            {
                macPath = "\"" + macPath;
            }

            if (!macPath.EndsWith("\""))
            {
                macPath = macPath + "\"";
            }

            string arguments = (openInsidesOfFolder ? "" : "-R ") + macPath;

            try
            {
                Process.Start("open", arguments);
            }
            catch (Win32Exception e)
            {
                // tried to open mac finder in windows
                // just silently skip error
                // we currently have no platform define for the current OS we are in, so we resort to this
                e.HelpLink = ""; // do anything with this variable to silence warning about not using it
            }
        }

        private static void OpenInWin(string path)
        {
            bool openInsidesOfFolder = false;

            // try windows
            string winPath = path.Replace("/", "\\"); // windows explorer doesn't like forward slashes

            if (Directory
                .Exists(winPath)) // if path requested is a folder, automatically open insides of that folder
            {
                openInsidesOfFolder = true;
            }

            try
            {
                Process.Start("explorer.exe",
                    (openInsidesOfFolder ? "/root," : "/select,") + winPath);
            }
            catch (Win32Exception e)
            {
                // tried to open win explorer in mac
                // just silently skip error
                // we currently have no platform define for the current OS we are in, so we resort to this
                e.HelpLink = ""; // do anything with this variable to silence warning about not using it
            }
        }

        private static void OpenInLinux(string path)
        {
            // try linux
            string linuxPath = path.Replace("/", "\\"); // linux file manager doesn't like forward slashes

            if (Directory
                .Exists(linuxPath)) // if path requested is a folder, automatically open insides of that folder
            {
            }

            try
            {
                Process.Start("xdg-open", linuxPath);
            }
            catch (Win32Exception e)
            {
                // tried to open linux file manager in mac
                // just silently skip error
                // we currently have no platform define for the current OS we are in, so we resort to this
                e.HelpLink = ""; // do anything with this variable to silence warning about not using it
            }
        }

        public static void Open(string path)
        {
            if (IsInWinOS)
            {
                OpenInWin(path);
            }
            else if (IsInMacOS)
            {
                OpenInMac(path);
            }
            else if (IsInLinuxOS)
            {
                OpenInLinux(path);
            }
            else // couldn't determine OS
            {
                Debug.LogError("Could not determine OS");
            }
        }
    }
}