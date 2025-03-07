using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CheesyUtils.Pooling
{
    [Serializable]
    public class Pool
    {
        /// <summary>
        /// The objects that you want to add to the pool
        /// </summary>
        public List<PoolObject> ObjectsToPool = new List<PoolObject>();

        /// <summary>
        /// All the objects that were added to the pool
        /// </summary>
        private List<GameObject> _poolObjects = new List<GameObject>();

        /// <summary>
        /// Initializes the pool in your scene
        /// </summary>
        public void Start_Pool()
        {
            var pool = new GameObject("Pool");

            foreach (var item in ObjectsToPool)
            {
                var subParent = new GameObject(item.ObjToPool.name);

                for (int i = 0; i < item.AmountToPool; i++)
                {
                    var obj = Object.Instantiate(item.ObjToPool, subParent.transform, false);
                    obj.SetActive(false);
                    _poolObjects.Add(obj);
                }

                subParent.transform.SetParent(pool.transform, false);
            }
        }

        /// <summary>
        /// Gets the item from the pool and spawns it at the required position
        /// </summary>
        /// <param name="type">The type of script in the pool that you need to spawn</param>
        /// <param name="position">The position where you want the item to spawn</param>
        /// <returns>A component of the needed type</returns>
        public Component Spawn_Item(Type type, Vector3 position, Quaternion direction)
        {
            foreach (GameObject item in _poolObjects)
            {
                item.TryGetComponent(type, out Component component);

                if (component == null)
                    continue;

                if (item.activeSelf)
                    continue;

                item.transform.position = position;
                item.transform.rotation = direction;
                item.SetActive(true);
                return component;

            }

            return null;
        }

        /// <summary>
        /// Brings the item back to the pool
        /// </summary>
        /// <param name="item">The item to bring back to the pool</param>
        public void Return_Item(GameObject item)
        {
            item.SetActive(false);
            item.transform.position = Vector3.zero;
        }
    }
}