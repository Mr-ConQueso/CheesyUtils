using System;
using System.Text;
using UnityEngine;

namespace CheesyUtils
{
    /// <summary>
    /// A SerializableQuaternion is a structure that allows for the serialization 
    /// of Unity's Quaternion type by exposing its components X, Y, Z, and W as floats.
    /// This is useful for saving and loading quaternion data outside of Unity's native format.
    /// </summary>
    [Serializable]
    public struct SerializableQuaternion
    {
        public float X;
        public float Y;
        public float Z;
        public float W;
        
        /// <summary>
        /// Constructs a new SerializableQuaternion with the given components.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="w"></param>
        public SerializableQuaternion(float x, float y, float z, float w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }
    
        /// <summary>
        /// Returns a string representation of the SerializableQuaternion.
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
            sb.Append(", ");
            sb.Append(W);
            sb.Append("]");
            return sb.ToString();
        }
        
        public static implicit operator Quaternion(SerializableQuaternion value) => new Quaternion(value.X, value.Y, value.Z, value.W);
        public static implicit operator SerializableQuaternion(Quaternion value) => new SerializableQuaternion(value.x, value.y, value.z, value.w);
    }
}
