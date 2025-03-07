using System;
using UnityEngine;

namespace CheesyUtils.Pooling
{
    [Serializable]
    public class PoolObject
    {
        /// <summary>
        /// The gameobject that you want to spawn
        /// </summary>
        public GameObject ObjToPool;
        /// <summary>
        /// The amount of objects that you want to spawn
        /// </summary>
        public int AmountToPool;
    }
}