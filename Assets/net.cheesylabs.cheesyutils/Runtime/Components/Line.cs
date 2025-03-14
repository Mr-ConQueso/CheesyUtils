using UnityEngine;
using UnityEngine.Splines;

namespace CheesyUtils
{
    public struct Line
    {
        public Spline Spline;
        
        public bool IsValid => Spline != null;
        
        public bool IsClosed => Spline.Closed;
        
        public bool IsPointToTheRightOfSpline(Vector3 point)
        {
            if (!IsValid) return false;
    
            Vector3 start = Start();
            Vector3 end = End();
    
            Vector3 lineDirection = end - start;
            Vector3 pointDirection = point - start;
    
            float crossProduct = (lineDirection.x * pointDirection.z) - (lineDirection.z * pointDirection.x);
    
            return crossProduct < 0;
        }
        
        public Vector3 Start()
        {
            return Spline[0].Position.xxx;
        }

        public Vector3 End()
        {
            return Spline[^1].Position.xxx;
        }
    }
}