using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace CheesyUtils {
    public static class GameObjectExtensions {
        
        /// <summary>
        /// Checks if the specified GameObject has any children.
        /// </summary>
        /// <param name="obj">The GameObject to check for children.</param>
        /// <returns>Returns true if the GameObject has one or more children, otherwise false.</returns>
        public static bool HasChildren(this GameObject obj)
        {
            return obj.transform.childCount > 0;
        }
        
                /// <summary>
        /// Recursively changes the layer of the specified GameObject and all its descendants.
        /// </summary>
        /// <param name="obj">The GameObject whose layer is to be changed.</param>
        /// <param name="newLayer">The new layer to assign to the GameObject and its children.</param>
        public static void ChangeLayerRecursively(this GameObject obj, int newLayer)
        {
            obj.layer = newLayer;
            foreach (Transform child in obj.transform)
            {
                ChangeLayerRecursively(child.gameObject, newLayer);
            }
        }

        /// <summary>
        /// Searches for a component of type T in the children of the specified GameObject.
        /// </summary>
        /// <typeparam name="T">The type of component to search for.</typeparam>
        /// <param name="parent">The parent GameObject in which to search for the component.</param>
        /// <param name="component">Output parameter that holds the found component.</param>
        /// <returns>Returns true if the component is found in a child, otherwise false.</returns>
        public static bool TryGetComponentInChildren<T>(this GameObject parent, out T component) where T : Component
        {
            component = parent.GetComponentInChildren<T>();
            return component != null;
        }

        /// <summary>
        /// Finds all components of type T within the children of the specified parent GameObject.
        /// </summary>
        /// <typeparam name="T">The type of component to find.</typeparam>
        /// <param name="parent">The parent GameObject whose children are to be searched.</param>
        /// <returns>A list of all components of type T found in the children.</returns>
        public static List<T> GetComponentsInChildren<T>(this GameObject parent) where T : Component
        {
            List<T> components = new List<T>();
            T[] foundComponents = parent.GetComponentsInChildren<T>();

            if (foundComponents != null && foundComponents.Length > 0)
            {
                components.AddRange(foundComponents);
            }

            return components;
        }

        /// <summary>
        /// Determines if the specified GameObject is on the given layer.
        /// </summary>
        /// <param name="obj">The GameObject whose layer is to be compared.</param>
        /// <param name="layer">The layer to compare against the GameObject's layer.</param>
        /// <returns>Returns true if the GameObject is on the specified layer, otherwise false.</returns>
        public static bool CompareLayer(this GameObject obj, int layer)
        {
            return obj.layer == layer;
        }

        /// <summary>
        /// Determines if the specified GameObject is on the layer with the given name.
        /// </summary>
        /// <param name="obj">The GameObject whose layer is to be compared.</param>
        /// <param name="layerName">The name of the layer to compare against the GameObject's layer.</param>
        /// <returns>Returns true if the GameObject is on the specified layer, otherwise false.</returns>
        public static bool CompareLayer(this GameObject obj, string layerName)
        {
            return obj.layer == LayerMask.NameToLayer(layerName);
        }
        
        /// <summary>
        /// This method is used to hide the GameObject in the Hierarchy view.
        /// </summary>
        /// <param name="gameObject"></param>
        public static void HideInHierarchy(this GameObject gameObject) {
            gameObject.hideFlags = HideFlags.HideInHierarchy;
        }

        /// <summary>
        /// Gets a component of the given type attached to the GameObject. If that type of component does not exist, it adds one.
        /// </summary>
        /// <remarks>
        /// This method is useful when you don't know if a GameObject has a specific type of component,
        /// but you want to work with that component regardless. Instead of checking and adding the component manually,
        /// you can use this method to do both operations in one line.
        /// </remarks>
        /// <typeparam name="T">The type of the component to get or add.</typeparam>
        /// <param name="gameObject">The GameObject to get the component from or add the component to.</param>
        /// <returns>The existing component of the given type, or a new one if no such component exists.</returns>    
        public static T GetOrAdd<T>(this GameObject gameObject) where T : Component {
            T component = gameObject.GetComponent<T>();
            if (!component) component = gameObject.AddComponent<T>();

            return component;
        }

        /// <summary>
        /// Returns the object itself if it exists, null otherwise.
        /// </summary>
        /// <remarks>
        /// This method helps differentiate between a null reference and a destroyed Unity object. Unity's "== null" check
        /// can incorrectly return true for destroyed objects, leading to misleading behaviour. The OrNull method use
        /// Unity's "null check", and if the object has been marked for destruction, it ensures an actual null reference is returned,
        /// aiding in correctly chaining operations and preventing NullReferenceExceptions.
        /// </remarks>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="obj">The object being checked.</param>
        /// <returns>The object itself if it exists and not destroyed, null otherwise.</returns>
        public static T OrNull<T>(this T obj) where T : Object => obj ? obj : null;

        /// <summary>
        /// Destroys all children of the game object
        /// </summary>
        /// <param name="gameObject">GameObject whose children are to be destroyed.</param>
        public static void DestroyChildren(this GameObject gameObject) {
            gameObject.transform.DestroyChildren();
        }

        /// <summary>
        /// Immediately destroys all children of the given GameObject.
        /// </summary>
        /// <param name="gameObject">GameObject whose children are to be destroyed.</param>
        public static void DestroyChildrenImmediate(this GameObject gameObject) {
            gameObject.transform.DestroyChildrenImmediate();
        }

        /// <summary>
        /// Enables all child GameObjects associated with the given GameObject.
        /// </summary>
        /// <param name="gameObject">GameObject whose child GameObjects are to be enabled.</param>
        public static void EnableChildren(this GameObject gameObject) {
            gameObject.transform.EnableChildren();
        }

        /// <summary>
        /// Disables all child GameObjects associated with the given GameObject.
        /// </summary>
        /// <param name="gameObject">GameObject whose child GameObjects are to be disabled.</param>
        public static void DisableChildren(this GameObject gameObject) {
            gameObject.transform.DisableChildren();
        }

        /// <summary>
        /// Resets the GameObject's transform's position, rotation, and scale to their default values.
        /// </summary>
        /// <param name="gameObject">GameObject whose transformation is to be reset.</param>
        public static void ResetTransformation(this GameObject gameObject) {
            gameObject.transform.Reset();
        }

        /// <summary>
        /// Returns the hierarchical path in the Unity scene hierarchy for this GameObject.
        /// </summary>
        /// <param name="gameObject">The GameObject to get the path for.</param>
        /// <returns>A string representing the full hierarchical path of this GameObject in the Unity scene.
        /// This is a '/'-separated string where each part is the name of a parent, starting from the root parent and ending
        /// with the name of the specified GameObjects parent.</returns>
        public static string Path(this GameObject gameObject) {
            return "/" + string.Join("/",
                gameObject.GetComponentsInParent<Transform>().Select(t => t.name).Reverse().ToArray());
        }

        /// <summary>
        /// Returns the full hierarchical path in the Unity scene hierarchy for this GameObject.
        /// </summary>
        /// <param name="gameObject">The GameObject to get the path for.</param>
        /// <returns>A string representing the full hierarchical path of this GameObject in the Unity scene.
        /// This is a '/'-separated string where each part is the name of a parent, starting from the root parent and ending
        /// with the name of the specified GameObject itself.</returns>
        public static string PathFull(this GameObject gameObject) {
            return gameObject.Path() + "/" + gameObject.name;
        }

        /// <summary>
        /// Recursively sets the provided layer for this GameObject and all of its descendants in the Unity scene hierarchy.
        /// </summary>
        /// <param name="gameObject">The GameObject to set layers for.</param>
        /// <param name="layer">The layer number to set for GameObject and all of its descendants.</param>
        public static void SetLayersRecursively(this GameObject gameObject, int layer) {
            gameObject.layer = layer;
            gameObject.transform.ForEveryChild(child => child.gameObject.SetLayersRecursively(layer));
        }
    }
}
