using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace CheesyUtils
{
    public static class SplineExtensions
    {
        /// <summary>
        /// Returns the closest point on the spline to a given position in world space
        /// </summary>
        public static Vector3 GetClosestPoint(this Spline spline, Vector3 position)
        {
            SplineUtility.GetNearestPoint(spline, 
                position, 
                out float3 nearest, 
                out float interpolationRatio, 
                16,
                2);

            return nearest;
        }

        /// <summary>
        /// Returns the tangent direction at the closest point on the spline to a given position
        /// </summary>
        public static Vector3 GetClosestTangent(this Spline spline, Vector3 position)
        {
            SplineUtility.GetNearestPoint(spline, 
                position, 
                out float3 nearest, 
                out float interpolationRatio, 
                16,
                2);

            return (Vector3)spline.EvaluateTangent(interpolationRatio);
        }

        /// <summary>
        /// Determines which side of the spline a position is on using the spline's normal vector
        /// </summary>
        /// <returns>1 for right side, -1 for left side</returns>
        public static int GetSideOfSpline(this Spline spline, Vector3 position, Transform containerTransform = null)
        {
            var closestPoint = spline.GetClosestPoint(position);
            var tangent = spline.GetClosestTangent(position);
            var toPosition = position - closestPoint;

            // Calculate normal using spline up vector
            var normal = Vector3.Cross(tangent, containerTransform?.up ?? Vector3.up).normalized;
            var dot = Vector3.Dot(toPosition, normal);
            
            return dot >= 0 ? 1 : -1;
        }
    }
}