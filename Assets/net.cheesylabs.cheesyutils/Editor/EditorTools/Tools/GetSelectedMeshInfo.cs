// display selected gameobject mesh stats (should work on prefabs,models in project window also)

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CheesyUtils.EditorTools
{
    public class GetSelectedMeshInfo : EditorWindow
    {
        private bool _selectionChanged = false;

        private int _totalMeshes = 0;
        private int _totalVertices = 0;
        private int _totalTris = 0;

        private readonly Dictionary<int, int> _topList = new Dictionary<int, int>();
        private IOrderedEnumerable<KeyValuePair<int, int>> _sortedTopList;

        private MeshFilter[] _meshes;

        [MenuItem("Tools/UnityLibrary/GetMeshInfo")]
        public static void ShowWindow()
        {
            var window = GetWindow(typeof(GetSelectedMeshInfo));
            window.titleContent = new GUIContent("MeshInfo");
        }

        private void OnGUI()
        {
            // TODO process all selected gameobjects
            var selection = Selection.activeGameObject;

            // if have selection
            if (selection == null)
            {
                EditorGUILayout.LabelField("Select gameobject from scene or hierarchy..");
            }
            else
            {
                EditorGUILayout.LabelField("Selected: " + selection.name);

                // update mesh info only if selection changed
                if (_selectionChanged == true)
                {
                    _selectionChanged = false;

                    // clear old top results
                    _topList.Clear();

                    _totalMeshes = 0;
                    _totalVertices = 0;
                    _totalTris = 0;

                    // check all meshes
                    _meshes = selection.GetComponentsInChildren<MeshFilter>();
                    for (int i = 0, length = _meshes.Length; i < length; i++)
                    {
                        int verts = _meshes[i].sharedMesh.vertexCount;
                        _totalVertices += verts;
                        // not for point/line meshes
                        if (_meshes[i].sharedMesh.GetTopology(0) == MeshTopology.Triangles) _totalTris += _meshes[i].sharedMesh.triangles.Length / 3;
                        _totalMeshes++;
                        _topList.Add(i, verts);
                    }

                    // sort top list
                    _sortedTopList = _topList.OrderByDescending(x => x.Value);
                }

                // display stats
                // String.Format("{0:n0}", 9876); // No digits after the decimal point. Output: 9,876
                EditorGUILayout.LabelField("Meshes: " + $"{_totalMeshes:n0}");
                EditorGUILayout.LabelField("Vertices: " + $"{_totalVertices:n0}");
                EditorGUILayout.LabelField("Triangles: " + $"{_totalTris:n0}");
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("TOP 20", EditorStyles.boldLabel);

                // top list
                if (_meshes != null && _sortedTopList != null)
                {
                    int i = 0;
                    foreach (var item in _sortedTopList)
                    {
                        int percent = (int)(item.Value / (float)_totalVertices * 100f);
                        EditorGUILayout.BeginHorizontal();
                        // ping button
                        if (GUILayout.Button(new GUIContent(" ", "Ping"), GUILayout.Width(16)))
                        {
                            EditorGUIUtility.PingObject(_meshes[item.Key].transform);
                        }
                        EditorGUILayout.LabelField(_meshes[item.Key].name + " = " + $"{item.Value:n0}" + " (" + percent + "%)");
                        GUILayout.ExpandWidth(true);
                        EditorGUILayout.EndHorizontal();

                        // show only first 20
                        if (++i > 20) break;
                    }
                }
            }
        }

        private void OnSelectionChange()
        {
            _selectionChanged = true;
            // force redraw window
            Repaint();
        }
    }
}
