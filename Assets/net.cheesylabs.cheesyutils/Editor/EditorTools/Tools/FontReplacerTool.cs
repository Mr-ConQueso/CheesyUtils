using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CheesyUtils.EditorTools
{
    public class FontReplacerTool : EditorWindow
    {
        private TMP_FontAsset _oldFont;
        private TMP_FontAsset _newFont;

        [MenuItem("Tools/Fonts/Font Replacer")]
        public static void ShowWindow()
        {
            GetWindow<FontReplacerTool>("Font Replacer");
        }

        private void OnGUI()
        {
            GUILayout.Label("Font Replacer Tool", EditorStyles.boldLabel);

            _oldFont = (TMP_FontAsset)EditorGUILayout.ObjectField("Font to Replace", _oldFont, typeof(TMP_FontAsset), false);
            _newFont = (TMP_FontAsset)EditorGUILayout.ObjectField("Replacement Font", _newFont, typeof(TMP_FontAsset), false);

            GUILayout.Space(10);

            GUILayout.FlexibleSpace();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Replace in Current Scene"))
            {
                if (ValidateInput())
                {
                    ReplaceFontsInScene(SceneManager.GetActiveScene());
                }
            }

            if (GUILayout.Button("Replace in All Scenes"))
            {
                if (ValidateInput() && EditorUtility.DisplayDialog("Confirm Replacement", "This will replace fonts in ALL scenes. Are you sure?", "Yes", "No"))
                {
                    ReplaceFontsInAllScenes();
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private bool ValidateInput()
        {
            if (_oldFont && _newFont) return true;
            
            EditorUtility.DisplayDialog("Error", "Please select both fonts before proceeding.", "OK");
            return false;
        }

        private void ReplaceFontsInScene(Scene scene)
        {
            int count = 0;
            foreach (var text in FindObjectsByType<TMP_Text>(FindObjectsSortMode.None))
            {
                if (text.font == _oldFont)
                {
                    Undo.RecordObject(text, "Replace Font");
                    text.font = _newFont;
                    count++;
                }
            }

            EditorUtility.DisplayDialog("Font Replacement Complete", $"Replaced {count} instance(s) of the font in scene: {scene.name}.", "OK");
        }

        private void ReplaceFontsInAllScenes()
        {
            string[] scenePaths = AssetDatabase.FindAssets("t:Scene");
            int totalCount = 0;

            foreach (string scenePathGUID in scenePaths)
            {
                string scenePath = AssetDatabase.GUIDToAssetPath(scenePathGUID);
                Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

                int countBefore = totalCount;
                ReplaceFontsInScene(scene);

                totalCount += totalCount - countBefore;

                EditorSceneManager.SaveScene(scene);
            }
            EditorUtility.DisplayDialog("Font Replacement Complete", $"Replaced {totalCount} instance(s) of the font across all scenes.", "OK");
        }
    }
}
