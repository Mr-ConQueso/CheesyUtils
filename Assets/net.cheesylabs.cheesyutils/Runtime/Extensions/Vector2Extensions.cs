using UnityEngine;

namespace CheesyUtils {
    public static class Vector2Extensions {
        /// <summary>
        /// Adds to any x y values of a Vector2
        /// </summary>
        public static Vector2 Add(this Vector2 vector2, float x = 0, float y = 0) {
            return new Vector2(vector2.x + x, vector2.y + y);
        }

        /// <summary>
        /// Sets any x y values of a Vector2
        /// </summary>
        public static Vector2 With(this Vector2 vector2, float? x = null, float? y = null) {
            return new Vector2(x ?? vector2.x, y ?? vector2.y);
        }
        
        /// <summary>
        /// Rounds the x and y values of a Vector2 to the specified number of decimal places.
        /// </summary>
        /// <param name="vector">The Vector2 to round.</param>
        /// <param name="to">The number of decimal places to round to.</param>
        /// <returns></returns>
        public static Vector2 Round(this Vector2 vector, int to = 0) => new Vector2(vector.x.Round(to), vector.y.Round(to)); 

        /// <summary>
        /// Rotates a Vector2 by a specified angle around a pivot point.
        /// </summary>
        /// <param name="vector">The Vector2 to rotate.</param>
        /// <param name="angle">The angle to rotate by.</param>
        /// <param name="pivot">The pivot point to rotate around.</param>
        /// <returns></returns>
        public static Vector2 Rotate(this Vector2 vector, float angle, Vector2 pivot = default(Vector2)) 
        {
            Vector2 rotated = Quaternion.Euler(new Vector3(0f, 0f, angle)) * (vector - pivot);
            return rotated + pivot;
        }
        
        /// <summary>
        /// Adds to any x y values of a Vector2
        /// </summary>
        /// <param name="v">The Vector2 to add to</param>
        /// <param name="x">The amount to add to the x value</param>
        /// <returns>The Vector2 with the x value modified</returns>
        public static Vector2 AddX(this Vector2 v, float x)
        {
            v.x = v.x + x;
            return v;
        }

        /// <summary>
        /// Returns a Boolean indicating whether the current Vector2 is in a given range from another Vector2
        /// </summary>
        /// <param name="current">The current Vector2 position</param>
        /// <param name="target">The Vector2 position to compare against</param>
        /// <param name="range">The range value to compare against</param>
        /// <returns>True if the current Vector2 is in the given range from the target Vector2, false otherwise</returns>
        public static bool InRangeOf(this Vector2 current, Vector2 target, float range) {
            return (current - target).sqrMagnitude <= range * range;
        }
        
        /// <summary>
        /// Computes a random point in an annulus (a ring-shaped area) based on minimum and 
        /// maximum radius values around a central Vector2 point (origin).
        /// </summary>
        /// <param name="origin">The center Vector2 point of the annulus.</param>
        /// <param name="minRadius">Minimum radius of the annulus.</param>
        /// <param name="maxRadius">Maximum radius of the annulus.</param>
        /// <returns>A random Vector2 point within the specified annulus.</returns>
        public static Vector2 RandomPointInAnnulus(this Vector2 origin, float minRadius, float maxRadius) {
            float angle = Random.value * Mathf.PI * 2f;
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    
            // Squaring and then square-rooting radii to ensure uniform distribution within the annulus
            float minRadiusSquared = minRadius * minRadius;
            float maxRadiusSquared = maxRadius * maxRadius;
            float distance = Mathf.Sqrt(Random.value * (maxRadiusSquared - minRadiusSquared) + minRadiusSquared);
    
            // Calculate the position vector
            Vector2 position = direction * distance;
            return origin + position;
        }
    }
}