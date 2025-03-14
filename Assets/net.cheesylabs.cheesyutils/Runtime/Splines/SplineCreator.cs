using UnityEngine;
using UnityEngine.Splines;

namespace CheesyUtils
{
    public static class SplineCreator
    {
        /// <summary>
        /// Creates a new spline from positions and returns the Spline instance
        /// </summary>
        public static Spline CreateSpline(Vector3[] positions, bool closed = false, 
            float tension = 1f)
        {
            if (positions == null || positions.Length < 2)
            {
                Debug.LogError("Spline creation requires at least 2 positions");
                return null;
            }

            Spline spline = new Spline();

            // Add knots with calculated tangents
            for (int i = 0; i < positions.Length; i++)
            {
                AddKnotWithTangents(spline, positions, i, closed, tension);
            }

            spline.Closed = closed;
            return spline;
        }

        /// <summary>
        /// Updates an existing spline with new positions
        /// </summary>
        public static void UpdateSpline(Spline spline, Vector3[] positions, 
            bool closed = false, float tension = 1f)
        {
            if (spline == null || positions == null || positions.Length < 2) return;

            spline.Clear();
            spline.Closed = closed;

            for (int i = 0; i < positions.Length; i++)
            {
                AddKnotWithTangents(spline, positions, i, closed, tension);
            }
        }

        private static void AddKnotWithTangents(Spline spline, Vector3[] positions, 
            int index, bool closed, float tension)
        {
            Vector3 position = positions[index];
            
            // Calculate auto-smooth tangents
            Vector3 prev = GetAdjacentPosition(positions, index - 1, closed);
            Vector3 next = GetAdjacentPosition(positions, index + 1, closed);
            
            Vector3 tangent = SplineUtility.GetAutoSmoothTangent(
                prev,
                position,
                next,
                tension
            );

            BezierKnot knot = new BezierKnot(position)
            {
                TangentIn = -tangent,
                TangentOut = tangent
            };

            spline.Add(knot);
        }

        private static Vector3 GetAdjacentPosition(Vector3[] positions, int index, bool closed)
        {
            if (!closed) return positions[Mathf.Clamp(index, 0, positions.Length - 1)];
            
            index = (index % positions.Length + positions.Length) % positions.Length;
            return positions[index];
        }
    }
}