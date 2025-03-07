using System;

namespace CheesyUtils
{
    public static class FloatExtensions
    {
        /// <summary>
        /// Rounds a float to the specified number of decimal places.
        /// </summary>
        /// <param name="value">The value to round.</param>
        /// <param name="places">The number of decimal places to round to.</param>
        /// <returns></returns>
        public static float Round(this float value, int places)
        {
            return (float)Math.Round(value, places);
        }
    }
}