// shows renderer stats from selected objects in the editor

using UnityEngine;
using UnityEditor;

namespace CheesyUtils.EditorTools
{
    public class RendererTypeCounter : EditorWindow
    {
        private int _activeMeshRendererCount;
        private int _inactiveMeshRendererCount;
        private int _activeSkinnedMeshRendererCount;
        private int _inactiveSkinnedMeshRendererCount;
        private int _activeSpriteRendererCount;
        private int _inactiveSpriteRendererCount;
        private int _totalGameObjectCount;

        [MenuItem("Tools/Renderer Type Counter")]
        public static void ShowWindow()
        {
            GetWindow<RendererTypeCounter>("Renderer Type Counter");
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Count Renderers"))
            {
                CountRenderersInSelection();
            }

            GUILayout.Space(10);

            GUILayout.Label("Active Mesh Renderers: " + _activeMeshRendererCount);
            GUILayout.Label("Inactive Mesh Renderers: " + _inactiveMeshRendererCount);
            GUILayout.Label("Active Skinned Mesh Renderers: " + _activeSkinnedMeshRendererCount);
            GUILayout.Label("Inactive Skinned Mesh Renderers: " + _inactiveSkinnedMeshRendererCount);
            GUILayout.Label("Active Sprite Renderers: " + _activeSpriteRendererCount);
            GUILayout.Label("Inactive Sprite Renderers: " + _inactiveSpriteRendererCount);
            GUILayout.Label("Total GameObjects: " + _totalGameObjectCount);
        }

        private void CountRenderersInSelection()
        {
            _activeMeshRendererCount = 0;
            _inactiveMeshRendererCount = 0;
            _activeSkinnedMeshRendererCount = 0;
            _inactiveSkinnedMeshRendererCount = 0;
            _activeSpriteRendererCount = 0;
            _inactiveSpriteRendererCount = 0;
            _totalGameObjectCount = 0;

            foreach (GameObject obj in Selection.gameObjects)
            {
                CountRenderersRecursively(obj);
            }

            Repaint();
        }

        private void CountRenderersRecursively(GameObject obj)
        {
            _totalGameObjectCount++;

            bool isActive = obj.activeInHierarchy;

            if (obj.GetComponent<MeshRenderer>())
            {
                if (isActive)
                {
                    _activeMeshRendererCount++;
                }
                else
                {
                    _inactiveMeshRendererCount++;
                }
            }

            if (obj.GetComponent<SkinnedMeshRenderer>())
            {
                if (isActive)
                {
                    _activeSkinnedMeshRendererCount++;
                }
                else
                {
                    _inactiveSkinnedMeshRendererCount++;
                }
            }

            if (obj.GetComponent<SpriteRenderer>())
            {
                if (isActive)
                {
                    _activeSpriteRendererCount++;
                }
                else
                {
                    _inactiveSpriteRendererCount++;
                }
            }

            foreach (Transform child in obj.transform)
            {
                CountRenderersRecursively(child.gameObject);
            }
        }
    }
}
