using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CheesyUtils.Editor.Documentation
{
    public class AssetZooWindow : EditorWindow
    {
        // --- Configuration Data ---
        [SerializeField] private SceneAsset targetScene;
        [SerializeField] private List<DefaultAsset> sourceFolders = new List<DefaultAsset>();
        
        // Settings
        private bool fullRegeneration = false;
        private float spacing = 2.0f;
        private float labelOffset = 0.5f;
        private int itemsPerRow = 10;
        private SpawnMode spawnMode = SpawnMode.CalculateBottom;

        // Scroll position for the editor window
        private Vector2 scrollPos;

        // Enum for the dropdown
        public enum SpawnMode
        {
            UsePivot,       // Places object exactly at (0,0,0) of grid point
            CalculateBottom // Offsets object so its lowest mesh point sits on Y=0
        }

        [MenuItem("Tools/Asset Zoo Manager")]
        public static void ShowWindow()
        {
            GetWindow<AssetZooWindow>("Asset Zoo");
        }

        private void OnGUI()
        {
            // Styling
            GUILayout.Label("Asset Zoo Configuration", EditorStyles.boldLabel);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            // 1. Scene Selection
            EditorGUILayout.Space();
            GUILayout.Label("Target Scene", EditorStyles.label);
            targetScene = (SceneAsset)EditorGUILayout.ObjectField("Zoo Scene", targetScene, typeof(SceneAsset), false);

            // 2. Folder Selection
            EditorGUILayout.Space();
            GUILayout.Label("Source Folders", EditorStyles.label);
            
            // simple custom drawer for a list of folders
            ScriptableObject target = this;
            SerializedObject so = new SerializedObject(target);
            SerializedProperty foldersProp = so.FindProperty("sourceFolders");
            EditorGUILayout.PropertyField(foldersProp, true);
            so.ApplyModifiedProperties();

            // 3. Settings
            EditorGUILayout.Space();
            GUILayout.Label("Generation Settings", EditorStyles.boldLabel);
            
            fullRegeneration = EditorGUILayout.Toggle(new GUIContent("Full Regenerate", "If True: Destroys everything and rebuilds.\nIf False: Only adds missing items and re-arranges."), fullRegeneration);
            
            spawnMode = (SpawnMode)EditorGUILayout.EnumPopup("Alignment", spawnMode);
            spacing = EditorGUILayout.FloatField("Grid Spacing", spacing);
            itemsPerRow = EditorGUILayout.IntField("Items Per Row", itemsPerRow);

            EditorGUILayout.Space(20);

            // 4. Action Button
            if (GUILayout.Button("Generate / Update Zoo", GUILayout.Height(40)))
            {
                if (IsValidConfig())
                {
                    GenerateZoo();
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private bool IsValidConfig()
        {
            if (targetScene == null)
            {
                EditorUtility.DisplayDialog("Error", "Please assign a Target Scene.", "OK");
                return false;
            }
            if (sourceFolders == null || sourceFolders.Count == 0 || sourceFolders.All(f => f == null))
            {
                EditorUtility.DisplayDialog("Error", "Please assign at least one Source Folder.", "OK");
                return false;
            }
            return true;
        }

        private void GenerateZoo()
        {
            // A. Open the specific scene
            string scenePath = AssetDatabase.GetAssetPath(targetScene);
            Scene openScene = SceneManager.GetActiveScene();
            
            if (openScene.path != scenePath)
            {
                if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
                openScene = EditorSceneManager.OpenScene(scenePath);
            }

            // B. Find or Create the Zoo Root
            GameObject root = GameObject.Find("Zoo_Root");
            if (root == null)
            {
                root = new GameObject("Zoo_Root");
                // Reset root to zero so grid math works
                root.transform.position = Vector3.zero; 
            }

            // C. Logic: Destroy if regenerating
            if (fullRegeneration)
            {
                // Destroy all children
                int childCount = root.transform.childCount;
                for (int i = childCount - 1; i >= 0; i--)
                {
                    DestroyImmediate(root.transform.GetChild(i).gameObject);
                }
            }

            // D. Gather all Assets from all folders
            List<GameObject> allAssets = new List<GameObject>();
            foreach (var folderAsset in sourceFolders)
            {
                if (folderAsset == null) continue;
                string path = AssetDatabase.GetAssetPath(folderAsset);
                
                // Check if it's actually a directory
                if (!Directory.Exists(path)) continue;

                string[] guids = AssetDatabase.FindAssets("t:Prefab t:Model", new[] { path });
                foreach (string guid in guids)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                    if (go != null && !allAssets.Contains(go))
                    {
                        allAssets.Add(go);
                    }
                }
            }

            // Sort alphabetically for neatness
            allAssets.Sort((a, b) => string.Compare(a.name, b.name));

            // E. Processing Loop (Instantiate or Reuse + Layout)
            Vector3 currentPos = Vector3.zero;
            float rowMaxZ = 0f;
            int colIndex = 0;

            // Cache existing children for "Update" mode
            List<Transform> existingChildren = new List<Transform>();
            foreach(Transform t in root.transform) existingChildren.Add(t);

            foreach (GameObject asset in allAssets)
            {
                // 1. Try to find existing instance (if not regenerating)
                GameObject instance = null;
                if (!fullRegeneration)
                {
                    // Simple match by name
                    Transform existing = existingChildren.FirstOrDefault(t => t.name == asset.name);
                    if (existing != null)
                    {
                        instance = existing.gameObject;
                        // Remove from list so we don't double count or process orphans later if we wanted
                        existingChildren.Remove(existing); 
                    }
                }

                // 2. Instantiate if missing
                if (instance == null)
                {
                    instance = (GameObject)PrefabUtility.InstantiatePrefab(asset);
                    instance.name = asset.name; // Remove "(Clone)"
                    instance.transform.parent = root.transform;
                    
                    // Add Label only for new objects (or check if missing)
                    UpdateOrAddLabel(instance, asset.name);
                }

                // 3. Layout / Positioning
                // Reset rotation to ensure bounds calc is consistent
                instance.transform.rotation = Quaternion.identity; 
                
                Bounds b = GetBounds(instance);
                
                // Calculate Y Offset
                float yPos = 0;
                if (spawnMode == SpawnMode.CalculateBottom)
                {
                    // If the bottom of the mesh is at -2, we need to add +2 to bring it to 0
                    float distFromPivotToBottom = -b.min.y; // b.min.y is usually negative relative to pivot if pivot is center
                    // Actually, bounds are world space, but if we just spawned at 0,0,0, they are effectively local relative to world 0
                    // Let's rely on the relative difference
                    yPos = -b.min.y + instance.transform.position.y;
                }
                else
                {
                    yPos = 0;
                }

                instance.transform.position = new Vector3(currentPos.x, yPos, currentPos.z);
                
                // Update Label Position (in case we moved the object)
                UpdateLabelPosition(instance, b);

                // 4. Advance Grid
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

            // Optional: Destroy "Ghost" objects (Objects in scene that are no longer in folders)?
            // For now, we leave them or let the user delete them manually to be safe.

            EditorSceneManager.MarkSceneDirty(openScene);
            Debug.Log($"Zoo Updated: Processed {allAssets.Count} assets.");
        }

        // --- Helpers ---

        private void UpdateOrAddLabel(GameObject target, string text)
        {
            Transform existingLabel = target.transform.Find("Zoo_Label");
            if (existingLabel != null) return; // Already has one

            GameObject label = new GameObject("Zoo_Label");
            label.transform.SetParent(target.transform);
            
            TextMesh tm = label.AddComponent<TextMesh>();
            tm.text = text;
            tm.characterSize = 0.1f; // Smaller char size = sharper text usually
            tm.fontSize = 60;
            tm.anchor = TextAnchor.UpperCenter;
            tm.alignment = TextAlignment.Center;
            tm.color = Color.white;
        }

        private void UpdateLabelPosition(GameObject target, Bounds bounds)
        {
            Transform label = target.transform.Find("Zoo_Label");
            if (label == null) return;

            // Position: Center X, Bottom Y (min.y), Center Z - offset
            // We want it slightly in front or below. 
            // User asked for "Bottom of the model".
            
            // Let's place it at the feet, slightly forward so it's readable
            Vector3 labelPos = new Vector3(0, bounds.min.y - labelOffset, -bounds.extents.z);
            
            // Since label is child, we need local position relative to pivot
            // Bounds are world space. Converting logic:
            
            // Easier approach: Calculate world pos then assign
            Vector3 worldFeet = new Vector3(bounds.center.x, bounds.min.y - labelOffset, bounds.center.z);
            
            // To prevent Z-fighting with floor, usually float it just above floor or rotate it flat?
            // Standard "Zoo" usually has floating text.
            // Let's put it at the "feet" (Y=0 relative to grid) but offset in Z so it's in front of the object.
            
            label.position = new Vector3(target.transform.position.x, target.transform.position.y + 0.1f, target.transform.position.z - (bounds.extents.z + 0.5f));
            
            // Rotate text to face camera or lie flat? 
            // Standard is usually billboard or lie flat. Let's lie flat on ground in front of object.
            label.rotation = Quaternion.Euler(90, 0, 0); 
        }

        private Bounds GetBounds(GameObject go)
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