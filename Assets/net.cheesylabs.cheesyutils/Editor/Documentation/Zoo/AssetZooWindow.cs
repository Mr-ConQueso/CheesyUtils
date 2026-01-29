using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CheesyUtils.Editor.Documentation
{
    public class AssetZooWindow : EditorWindow
    {
        [SerializeField] private SceneAsset targetScene;
        [SerializeField] private List<DefaultAsset> sourceFolders = new();
    
        private bool _fullRegeneration;
        private float _spacing = 2.0f;
        private float _labelOffset = 0.5f;
        private int _itemsPerRow = 10;
        private SpawnMode _spawnMode = SpawnMode.CalculateBottom;

        private Vector2 _scrollPos;

        public enum SpawnMode
        {
            UsePivot,       // Places object exactly at (0,0,0) of grid point
            CalculateBottom // Offsets object so its lowest mesh point sits on Y=0
        }

        private static string PrefsKey(string key) => $"CheesyUtils_AssetZoo_{Application.dataPath.GetHashCode()}_{key}";

        [MenuItem("Tools/Asset Zoo Manager")]
        public static void ShowWindow()
        {
            GetWindow<AssetZooWindow>("Asset Zoo");
        }

        private void OnEnable()
        {
            LoadSettings();
        }

        private void OnDisable()
        {
            SaveSettings();
        }

        private void OnGUI()
        {
            GUILayout.Label("Asset Zoo Configuration", EditorStyles.boldLabel);
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.Space();
            GUILayout.Label("Target Scene", EditorStyles.label);
            targetScene = (SceneAsset)EditorGUILayout.ObjectField("Zoo Scene", targetScene, typeof(SceneAsset), false);

            EditorGUILayout.Space();
            GUILayout.Label("Source Folders", EditorStyles.label);
            
            ScriptableObject target = this;
            SerializedObject so = new SerializedObject(target);
            SerializedProperty foldersProp = so.FindProperty("sourceFolders");
            EditorGUILayout.PropertyField(foldersProp, true);
            so.ApplyModifiedProperties();

            EditorGUILayout.Space();
            GUILayout.Label("Generation Settings", EditorStyles.boldLabel);
            
            _fullRegeneration = EditorGUILayout.Toggle(new GUIContent("Full Regenerate", "If True: Destroys everything and rebuilds.\nIf False: Only adds missing items and re-arranges."), _fullRegeneration);
            
            _spawnMode = (SpawnMode)EditorGUILayout.EnumPopup("Alignment", _spawnMode);
            _spacing = EditorGUILayout.FloatField("Grid Spacing", _spacing);
            _itemsPerRow = EditorGUILayout.IntField("Items Per Row", _itemsPerRow);

            if (EditorGUI.EndChangeCheck())
            {
                SaveSettings();
            }

            EditorGUILayout.Space(20);

            if (GUILayout.Button("Generate / Update Zoo", GUILayout.Height(40)))
            {
                if (IsValidConfig())
                {
                    AssetZooGenerator.GenerateZoo(targetScene, sourceFolders, _fullRegeneration, _spacing, _labelOffset, _itemsPerRow, _spawnMode);
                }
            }

            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// Loads settings from EditorPrefs using project-specific keys.
        /// </summary>
        private void LoadSettings()
        {
            _fullRegeneration = EditorPrefs.GetBool(PrefsKey("FullRegen"), false);
            _spacing = EditorPrefs.GetFloat(PrefsKey("Spacing"), 2.0f);
            _itemsPerRow = EditorPrefs.GetInt(PrefsKey("ItemsPerRow"), 10);
            _spawnMode = (SpawnMode)EditorPrefs.GetInt(PrefsKey("SpawnMode"), (int)SpawnMode.CalculateBottom);

            string sceneGuid = EditorPrefs.GetString(PrefsKey("TargetScene"), "");
            if (!string.IsNullOrEmpty(sceneGuid))
            {
                string path = AssetDatabase.GUIDToAssetPath(sceneGuid);
                if (!string.IsNullOrEmpty(path))
                {
                    targetScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
                }
            }

            string foldersStr = EditorPrefs.GetString(PrefsKey("SourceFolders"), "");
            if (!string.IsNullOrEmpty(foldersStr))
            {
                sourceFolders = new List<DefaultAsset>();
                string[] guids = foldersStr.Split(';');
                foreach (string guid in guids)
                {
                    if (string.IsNullOrEmpty(guid)) continue;
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    if (!string.IsNullOrEmpty(path))
                    {
                        DefaultAsset asset = AssetDatabase.LoadAssetAtPath<DefaultAsset>(path);
                        if (asset) sourceFolders.Add(asset);
                    }
                }
            }
        }

        /// <summary>
        /// Saves settings to EditorPrefs immediately.
        /// </summary>
        private void SaveSettings()
        {
            EditorPrefs.SetBool(PrefsKey("FullRegen"), _fullRegeneration);
            EditorPrefs.SetFloat(PrefsKey("Spacing"), _spacing);
            EditorPrefs.SetInt(PrefsKey("ItemsPerRow"), _itemsPerRow);
            EditorPrefs.SetInt(PrefsKey("SpawnMode"), (int)_spawnMode);

            if (targetScene)
            {
                string path = AssetDatabase.GetAssetPath(targetScene);
                string guid = AssetDatabase.AssetPathToGUID(path);
                EditorPrefs.SetString(PrefsKey("TargetScene"), guid);
            }
            else
            {
                EditorPrefs.DeleteKey(PrefsKey("TargetScene"));
            }

            if (sourceFolders is { Count: > 0 })
            {
                IEnumerable<string> validGuids = sourceFolders
                    .Where(a => a)
                    .Select(a => AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(a)))
                    .Where(g => !string.IsNullOrEmpty(g));
                
                EditorPrefs.SetString(PrefsKey("SourceFolders"), string.Join(";", validGuids));
            }
            else
            {
                EditorPrefs.DeleteKey(PrefsKey("SourceFolders"));
            }
        }

        private bool IsValidConfig()
        {
            if (!targetScene)
            {
                EditorUtility.DisplayDialog("Error", "Please assign a Target Scene.", "OK");
                return false;
            }
            if (sourceFolders == null || sourceFolders.Count == 0 || sourceFolders.All(f => !f))
            {
                EditorUtility.DisplayDialog("Error", "Please assign at least one Source Folder.", "OK");
                return false;
            }
            return true;
        }
    }
}