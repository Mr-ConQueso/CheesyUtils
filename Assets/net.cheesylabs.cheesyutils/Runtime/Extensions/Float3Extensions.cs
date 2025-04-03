using System;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CheesyUtils {
    public static class Float3Extensions {
        /// <summary>
        /// Sets any x y z values of a float3
        /// </summary>
        public static float3 With(this float3 vector, float? x = null, float? y = null, float? z = null) {
            return new float3(x ?? vector.x, y ?? vector.y, z ?? vector.z);
        }

        /// <summary>
        /// Adds to any x y z values of a float3
        /// </summary>
        public static float3 Add(this float3 vector, float x = 0, float y = 0, float z = 0) {
            return new float3(vector.x + x, vector.y + y, vector.z + z);
        }

        /// <summary>
        /// Returns a Boolean indicating whether the current float3 is in a given range from another float3
        /// </summary>
        /// <param name="current">The current float3 position</param>
        /// <param name="target">The float3 position to compare against</param>
        /// <param name="range">The range value to compare against</param>
        /// <returns>True if the current float3 is in the given range from the target float3, false otherwise</returns>
        public static bool InRangeOf(this float3 current, float3 target, float range) {
            return (current - target).SqrMagnitude() <= range * range;
        }
        
        /// <summary>
        /// Divides two float3 objects component-wise.
        /// </summary>
        /// <remarks>
        /// For each component in v0 (x, y, z), it is divided by the corresponding component in v1 if the component in v1 is not zero. 
        /// Otherwise, the component in v0 remains unchanged.
        /// </remarks>
        /// <example>
        /// Use 'ComponentDivide' to scale a game object proportionally:
        /// <code>
        /// myObject.transform.localScale = originalScale.ComponentDivide(targetDimensions);
        /// </code>
        /// This scales the object size to fit within the target dimensions while maintaining its original proportions.
        ///</example>
        /// <param name="v0">The float3 object that this method extends.</param>
        /// <param name="v1">The float3 object by which v0 is divided.</param>
        /// <returns>A new float3 object resulting from the component-wise division.</returns>
        public static float3 ComponentDivide(this float3 v0, float3 v1){
            return new float3( 
                v1.x != 0 ? v0.x / v1.x : v0.x, 
                v1.y != 0 ? v0.y / v1.y : v0.y, 
                v1.z != 0 ? v0.z / v1.z : v0.z);  
        }
        
        /// <summary>
        /// Converts a float2 to a float3 with a y value of 0.
        /// </summary>
        /// <param name="v2">The Vector2 to convert.</param>
        /// <returns>A float3 with the x and z values of the Vector2 and a y value of 0.</returns>
        public static float3 ToFloat3(this float2 v2) {
            return new float3(v2.x, 0, v2.y);
        }
        
        /// <summary>
        /// Computes a random point in an annulus (a ring-shaped area) based on minimum and 
        /// maximum radius values around a central float3 point (origin).
        /// </summary>
        /// <param name="origin">The center float3 point of the annulus.</param>
        /// <param name="minRadius">Minimum radius of the annulus.</param>
        /// <param name="maxRadius">Maximum radius of the annulus.</param>
        /// <returns>A random float3 point within the specified annulus.</returns>
        public static float3 RandomPointInAnnulus(this float3 origin, float minRadius, float maxRadius) {
            float angle = Random.value * Mathf.PI * 2f;
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    
            // Squaring and then square-rooting radii to ensure uniform distribution within the annulus
            float minRadiusSquared = minRadius * minRadius;
            float maxRadiusSquared = maxRadius * maxRadius;
            float distance = Mathf.Sqrt(Random.value * (maxRadiusSquared - minRadiusSquared) + minRadiusSquared);
    
            // Converting the 2D direction vector to a 3D position vector
            float3 position = new float3(direction.x, 0, direction.y) * distance;
            return origin + position;
        }

        public static float3 ClampMagnitude(this float3 vector, float min, float max)
        {
            float sqrMagnitude = vector.x * vector.x + vector.y * vector.y + vector.z * vector.z;
            if (sqrMagnitude == 0f)
            {
                return vector; // Can't scale a zero vector
            }

            float magnitude = MathF.Sqrt(sqrMagnitude);
    
            if (magnitude < min)
            {
                float scale = min / magnitude;
                return new float3(vector.x * scale, vector.y * scale, vector.z * scale);
            }
            else if (magnitude > max)
            {
                float scale = max / magnitude;
                return new float3(vector.x * scale, vector.y * scale, vector.z * scale);
            }
    
            return vector;
        }
        
        public static float3 Normalized(this float3 vector)
        {
            return vector / vector.Magnitude();
        }
        
        public static float SqrMagnitude(this float3 vector)
        {
            return (float)Math.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
        }
        
        public static float Magnitude(this float3 vector)
        {
            return vector.x * vector.x + vector.y * vector.y + vector.z * vector.z;
        }
    }
}
