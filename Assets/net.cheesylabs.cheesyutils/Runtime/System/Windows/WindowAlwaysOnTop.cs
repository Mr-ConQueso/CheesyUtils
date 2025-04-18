// keep executable window always on top, good for multiplayer testing

using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace CheesyUtils
{
    public class WindowAlwaysOnTop : MonoBehaviour
    {
#if !UNITY_STANDALONE_LINUX

        // https://stackoverflow.com/a/34703664/5452781
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private const UInt32 SWP_NOSIZE = 0x0001;
        private const UInt32 SWP_NOMOVE = 0x0002;
        private const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        // https://forum.unity.com/threads/unity-window-handle.115364/#post-1650240
        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();

        public static IntPtr GetWindowHandle()
        {
            return GetActiveWindow();
        }
#endif

        private void Awake()
        {
            // TODO save current window pos to player prefs on exit, then can open into same position next time
#if !UNITY_EDITOR && !UNITY_STANDALONE_LINUX
        Debug.Log("Make window stay on top");
        SetWindowPos(GetActiveWindow(), HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
#endif
        }
    }
}
