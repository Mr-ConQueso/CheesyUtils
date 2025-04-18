using System;
using System.Globalization;

namespace CheesyUtils.Editor.BuildTools
{
    public static class TrackingUtil
        {
        public static string FormatMSTime(int ms)
        {
            return string.Format("{0}s", (ms / 1000.0f).ToString("F2", CultureInfo.InvariantCulture));
        }

        public static int GetMilliseconds()
        {
            return (int)(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond);
        }
    }
}