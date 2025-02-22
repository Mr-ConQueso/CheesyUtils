using UnityEngine;

namespace CheesyUtils
{
    public static class HelperFunctions
    {
        /// <summary>
        /// Cycle between a given number and a maximum one
        /// </summary>
        /// <param name="numberToCycle">The current number to cycle.</param>
        /// <param name="maxValue">The maximum value (exclusive) to cycle to.</param>
        /// <returns>The next cycled number.</returns>
        public static int CycleNumber(int numberToCycle, int maxValue)
        {
            return (numberToCycle + 1) % maxValue;
        }
        
        /// <summary>
        /// Attempts to find the first GameObject with the specified tag and returns its transform.
        /// </summary>
        /// <param name="tag">The tag to search for the GameObject.</param>
        /// <param name="transform">Output parameter that holds the Transform if found.</param>
        /// <returns>Returns true if a GameObject with the specified tag is found, otherwise false.</returns>
        public static bool TryGetTransformWithTag(string tag, out Transform transform)
        {
            GameObject obj = GameObject.FindWithTag(tag);
            if (obj != null)
            {
                transform = obj.transform;
                return true;
            }
            else
            {
                transform = null;
                return false;
            }
        }
        
        public static Color GetRandomColor(Color[] colors)
        {
            return colors[Random.Range(0, colors.Length)];
        }
        
        public static Material GetRandomMaterial(Material[] materials)
        {
            return materials[Random.Range(0, materials.Length)];
        }
    }
}