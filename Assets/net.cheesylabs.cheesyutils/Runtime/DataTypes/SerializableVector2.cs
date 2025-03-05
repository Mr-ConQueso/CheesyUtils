using System;
using System.Text;
using UnityEngine;

namespace CheesyUtils
{
    /// <summary>
    /// A SerializableVector2 is a structure that allows for the serialization 
    /// of Unity's Vector2 type by exposing its components X and Y as floats.
    /// This is useful for saving and loading vector data outside of Unity's native format.
    /// </summary>
    [Serializable]
    public struct SerializableVector2
    {
        public float X;
        public float Y;
        
        /// <summary>
        /// Constructs a new SerializableVector2 with the given components.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public SerializableVector2(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }
    
        /// <summary>
        /// Returns a string representation of the SerializableVector2.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            sb.Append(X);
            sb.Append(", ");
            sb.Append(Y);
            sb.Append("]");
            return sb.ToString();
        }
        
        public static implicit operator Vector2(SerializableVector2 value) => new Vector2(value.X, value.Y);
        public static implicit operator SerializableVector2(Vector2 value) => new SerializableVector2(value.x, value.y);
    }
}
