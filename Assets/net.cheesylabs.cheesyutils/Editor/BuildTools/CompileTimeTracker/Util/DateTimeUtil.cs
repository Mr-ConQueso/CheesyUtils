using System;

namespace CheesyUtils.Editor.BuildTools
{
    public static class DateTimeUtil
    {
        public static bool SameDay(DateTime a, DateTime b)
        {
            return a.Date == b.Date;
        }
    }
}
