using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CheesyUtils.Editor.Documentation
{
    public class AssetZooGenerator : UnityEditor.Editor
    {
        /// <summary>
        /// Generates a "zoo" layout in the specified scene by organizing assets into a grid structure based on the provided parameters.
        /// </summary>
        /// <param name="targetScene">The target scene in which the "zoo" will be generated.</param>
        /// <param name="sourceFolders">The list of asset folders to search for prefabs or models to populate the zoo.</param>
        /// <param name="fullRegeneration">A flag indicating whether to fully regenerate the zoo by clearing existing items.</param>
        /// <param name="spacing">The spacing between items in the zoo layout.</param>
        /// <param name="labelOffset">The vertical offset to position labels above or below the items.</param>
        /// <param name="itemsPerRow">The number of items to place in each row before wrapping to the next row.</param>
        /// <param name="spawnMode">The mode for positioning items, determining whether to use the model's pivot or calculate the bottom alignment.</param>
        public static void GenerateZoo(SceneAsset targetScene, List<DefaultAsset> sourceFolders,
            bool fullRegeneration, float spacing, float labelOffset, int itemsPerRow,
            AssetZooWindow.SpawnMode spawnMode)
        {
            string scenePath = AssetDatabase.GetAssetPath(targetScene);
            Scene openScene = SceneManager.GetActiveScene();

            if (openScene.path != scenePath)
            {
                if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
                openScene = EditorSceneManager.OpenScene(scenePath);
            }

            GameObject root = GameObject.Find("Zoo_Root");
            if (!root)
            {
                root = new GameObject("Zoo_Root")
                {
                    transform =
                    {
                        position = Vector3.zero
                    }
                };
            }

            if (fullRegeneration)
            {
                int childCount = root.transform.childCount;
                for (int i = childCount - 1; i >= 0; i--)
                {
                    DestroyImmediate(root.transform.GetChild(i).gameObject);
                }
            }

            List<GameObject> allAssets = new List<GameObject>();
            foreach (DefaultAsset folderAsset in sourceFolders)
            {
                if (!folderAsset) continue;
                string path = AssetDatabase.GetAssetPath(folderAsset);

                if (!Directory.Exists(path)) continue;

                string[] guids = AssetDatabase.FindAssets("t:Prefab t:Model", new[] { path });
                foreach (string guid in guids)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                    if (go && !allAssets.Contains(go))
                    {
                        allAssets.Add(go);
                    }
                }
            }

            allAssets.Sort((a, b) => string.CompareOrdinal(a.name, b.name));

            Vector3 currentPos = Vector3.zero;
            float rowMaxZ = 0f;
            int colIndex = 0;

            List<Transform> existingChildren = new List<Transform>();
            foreach(Transform t in root.transform) existingChildren.Add(t);

            foreach (GameObject asset in allAssets)
            {
                GameObject instance = null;
                if (!fullRegeneration)
                {
                    Transform existing = existingChildren.FirstOrDefault(t => t.name == asset.name);
                    if (existing)
                    {
                        instance = existing.gameObject;
                        existingChildren.Remove(existing); 
                    }
                }

                if (!instance)
                {
                    instance = (GameObject)PrefabUtility.InstantiatePrefab(asset);
                    instance.name = asset.name; // Remove "(Clone)"
                    instance.transform.parent = root.transform;
                    
                    UpdateOrAddLabel(instance, asset.name);
                }

                instance.transform.rotation = Quaternion.identity; 
                Bounds b = GetBounds(instance);
                
                float yPos = 0;
                if (spawnMode == AssetZooWindow.SpawnMode.CalculateBottom)
                {
                    float distFromPivotToBottom = -b.min.y;
                    yPos = -b.min.y + instance.transform.position.y;
                }
                else
                {
                    yPos = 0;
                }

                instance.transform.position = new Vector3(currentPos.x, yPos, currentPos.z);
                
                UpdateLabelPosition(instance, b, labelOffset);

                if (ZooDebugger.ValidateZooItem(instance, out List<string> errorReport))
                {
                    ZooDebugger.AttachErrorVisuals(instance, errorReport);
                    Debug.LogError($"Issues found in {instance.name}: {string.Join(", ", errorReport)}");
                }
                else
                {
                    Transform oldError = instance.transform.Find("Debug_Error_Flag");
                    if (oldError) DestroyImmediate(oldError.gameObject);
                }

                float width = b.size.x;
                float length = b.size.z;

                if (length > rowMaxZ) rowMaxZ = length;

                currentPos.x += width + spacing;
                colIndex++;

                if (colIndex >= itemsPerRow)
                {
                    colIndex = 0;
                    currentPos.x = 0;
                    currentPos.z += rowMaxZ + spacing;
                    rowMaxZ = 0f;
                }
            }
            
            EditorSceneManager.MarkSceneDirty(openScene);
            Debug.Log($"Zoo Updated: Processed {allAssets.Count} assets.");
        }
        
        private static void UpdateOrAddLabel(GameObject target, string text)
        {
            Transform existingLabel = target.transform.Find("Zoo_Label");
            if (existingLabel) return;

            GameObject label = new GameObject("Zoo_Label");
            label.transform.SetParent(target.transform);
            
            TextMesh tm = label.AddComponent<TextMesh>();
            tm.text = text;
            tm.characterSize = 0.1f;
            tm.fontSize = 60;
            tm.anchor = TextAnchor.UpperCenter;
            tm.alignment = TextAlignment.Center;
            tm.color = Color.white;
        }

        private static void UpdateLabelPosition(GameObject target, Bounds bounds, float labelOffset)
        {
            Transform label = target.transform.Find("Zoo_Label");
            if (!label) return;
            
            label.position = new Vector3(
                target.transform.position.x,
                0.1f,
                target.transform.position.z - (bounds.extents.z + 0.5f));
            
            label.rotation = Quaternion.Euler(90, 0, 0); 
        }

        private static Bounds GetBounds(GameObject go)
        {
            Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0) return new Bounds(go.transform.position, Vector3.one);

            Bounds b = renderers[0].bounds;
            foreach (Renderer r in renderers)
            {
                b.Encapsulate(r.bounds);
            }
            return b;
        }
    }
}