using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CheesyUtils.Editor.Documentation
{
    public class AssetZooGenerator : UnityEditor.Editor
    {
        // 1. Add menu item to the Right-Click context menu in the Project View
        [MenuItem("Assets/Generate Asset Zoo Scene", false, 20)]
        private static void GenerateZoo()
        {
            // Get the path of the selected folder
            string selectedPath = AssetDatabase.GetAssetPath(Selection.activeObject);

            // Validation: Ensure it's a folder
            if (!Directory.Exists(selectedPath))
            {
                Debug.LogError("Please select a folder to generate an Asset Zoo.");
                return;
            }

            CreateZooScene(selectedPath);
        }

        // Validator: Only enable the option if a folder is selected
        [MenuItem("Assets/Generate Asset Zoo Scene", true)]
        private static bool ValidateGenerateZoo()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            return AssetDatabase.IsValidFolder(path);
        }

        private static void CreateZooScene(string folderPath)
        {
            // 2. Find all Prefabs and Models in the folder
            // We search for type:Prefab and type:Model to catch everything relevant
            string[] guids = AssetDatabase.FindAssets("t:Prefab t:Model", new[] { folderPath });
            
            if (guids.Length == 0)
            {
                Debug.LogWarning($"No prefabs or models found in {folderPath}");
                return;
            }

            // 3. Create a new empty scene
            Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            newScene.name = "AssetZoo_" + Path.GetFileName(folderPath);

            GameObject root = new GameObject("Zoo_Root");
            
            // Layout settings
            Vector3 currentPos = Vector3.zero;
            float rowHeight = 0f;
            float spacing = 2.0f;
            int itemsPerRow = 10;
            int currentColumn = 0;

            List<GameObject> spawnedObjects = new List<GameObject>();

            // 4. Instantiate and Place Assets
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

                if (asset == null) continue;

                // Instantiate
                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(asset);
                instance.transform.parent = root.transform;
                instance.name = asset.name;

                // Calculate Bounds to prevent overlap
                Bounds bounds = GetBounds(instance);
                
                // Adjust Y position so the object sits ON the ground, not IN it
                float yOffset = -bounds.min.y;
                
                // Set Position
                instance.transform.position = currentPos + new Vector3(0, yOffset, 0);

                // Create a simple text label (optional, requires TextMeshPro or standard 3D Text)
                CreateLabel(instance, instance.name, bounds);

                // Move cursor for next item
                float width = bounds.size.x;
                currentPos.x += width + spacing;

                // Track max height for this row
                if (bounds.size.z > rowHeight) rowHeight = bounds.size.z;

                // Grid Logic: Move to next row if needed
                currentColumn++;
                if (currentColumn >= itemsPerRow)
                {
                    currentColumn = 0;
                    currentPos.x = 0;
                    currentPos.z += rowHeight + spacing;
                    rowHeight = 0f; // Reset row height
                }
            }

            // 5. Cleanup
            Selection.activeGameObject = root;
            SceneView.lastActiveSceneView.FrameSelected();
            Debug.Log($"Generated Asset Zoo for {folderPath} with {guids.Length} assets.");
        }

        // Helper: accurate bounds calculation even for complex nested objects
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

        // Helper: Create a simple debug label
        private static void CreateLabel(GameObject target, string text, Bounds bounds)
        {
            GameObject label = new GameObject(text + "_Label");
            label.transform.position = target.transform.position + new Vector3(0, bounds.max.y + 0.5f, 0);
            label.transform.parent = target.transform;
            
            // Using TextMesh (Legacy) for zero-dependency simplicity. 
            // In a real project, you'd likely use TMP.
            TextMesh tm = label.AddComponent<TextMesh>();
            tm.text = text;
            tm.characterSize = 0.2f;
            tm.fontSize = 50;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.alignment = TextAlignment.Center;
            tm.color = Color.white;
        }
    }
}