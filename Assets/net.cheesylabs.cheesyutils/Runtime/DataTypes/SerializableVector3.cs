using System;
using System.Text;
using UnityEngine;

namespace CheesyUtils
{
    /// <summary>
    /// A SerializableVector3 is a structure that allows for the serialization 
    /// of Unity's Vector3 type by exposing its components X, Y, and Z as floats.
    /// This is useful for saving and loading vector data outside of Unity's native format.
    /// </summary>
    [Serializable]
    public struct SerializableVector3
    {
        public float X;
        public float Y;
        public float Z;
        
        /// <summary>
        /// Constructs a new SerializableVector3 with the given components.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public SerializableVector3(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        /// <summary>
        /// Returns a string representation of the SerializableVector3.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            sb.Append(X);
            sb.Append(", ");
            sb.Append(Y);
            sb.Append(", ");
            sb.Append(Z);
            sb.Append("]");
            return sb.ToString();
        }
        
        public static implicit operator Vector3(SerializableVector3 value) => new Vector3(value.X, value.Y, value.Z);
        public static implicit operator SerializableVector3(Vector3 value) => new SerializableVector3(value.x, value.y, value.z);
    }
}
