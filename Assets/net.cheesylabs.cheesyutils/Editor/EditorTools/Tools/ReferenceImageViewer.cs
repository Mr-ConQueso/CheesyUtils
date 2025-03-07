// simple image viewer inside unity editor
// use case: to keep reference image visible while working in single monitor

using UnityEngine;
using UnityEditor;

namespace CheesyUtils.EditorTools
{
    public class ReferenceImageViewer : EditorWindow
    {
        private Texture2D _tex;
        private bool _keepAspectRatio = false;

        [MenuItem("Window/Tools/Reference Image Viewer")]
        private static void Init()
        {
            ReferenceImageViewer window = (ReferenceImageViewer)EditorWindow.GetWindow(typeof(ReferenceImageViewer));
            window.titleContent = new GUIContent("ReferenceImageViewer");
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Image", GUILayout.Width(50));
            _tex = (Texture2D)EditorGUILayout.ObjectField(_tex, typeof(Texture2D), true, GUILayout.MinWidth(100));
            GUILayout.FlexibleSpace();
            _keepAspectRatio = EditorGUILayout.ToggleLeft("KeepAspect", _keepAspectRatio);
            EditorGUILayout.EndHorizontal();

            if (_tex != null)
            {
                int topOffset = 20;

                // prep
                var maxWidth = position.width;
                var maxHeight = position.height - topOffset;
                var imgWidth = (float)_tex.width;
                var imgHeight = (float)_tex.height;

                // calc
                var widthRatio = maxWidth / imgWidth;
                var heightRatio = maxHeight / imgHeight;
                var bestRatio = Mathf.Min(widthRatio, heightRatio);

                // output
                var newWidth = imgWidth * bestRatio;
                var newHeight = imgHeight * bestRatio;

                if (_keepAspectRatio == true)
                {
                    EditorGUI.DrawPreviewTexture(new Rect(0, topOffset, newWidth, newHeight), _tex);
                }
                else
                {
                    EditorGUI.DrawPreviewTexture(new Rect(0, topOffset, maxWidth, maxHeight), _tex);
                }
            }
        }
    }
}
