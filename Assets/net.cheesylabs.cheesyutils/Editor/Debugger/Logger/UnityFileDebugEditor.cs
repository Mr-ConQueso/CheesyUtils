using UnityEditor;
using UnityEngine;

namespace CheesyUtils.Editor
{
    [CustomEditor(typeof(UnityFileDebug))]
    public class UnityFileDebugEditor : UnityEditor.Editor
    {
        private UnityFileDebug _instance;

        private SerializedProperty _showAbsolute;
        private SerializedProperty _absolutePath;
        private GUIContent _absolutePathContent;

        private SerializedProperty _fileName;
        private GUIContent _fileNameContent;

        private SerializedProperty _filePath;
        private SerializedProperty _filePathFull;

        private SerializedProperty _fileType;
        private GUIContent _fileTypeContent;

        private string _copyPath;

        private void OnEnable()
        {
            _instance = (UnityFileDebug)target;

            _absolutePathContent = new GUIContent
            {
                text = "Absolute Path",
                tooltip = "The absolute system path to store the outputted log files"
            };

            _fileNameContent = new GUIContent
            {
                text = "Export File Name",
                tooltip = "The filename (without extension) you would like to save logs as"
            };

            _fileTypeContent = new GUIContent
            {
                text = "Export File Type",
                tooltip = "Export file type"
            };

            // Update references to serialized objects
            _showAbsolute = serializedObject.FindProperty("UseAbsolutePath");
            _absolutePath = serializedObject.FindProperty("AbsolutePath");
            _fileName = serializedObject.FindProperty("FileName");
            _filePath = serializedObject.FindProperty("FilePath");
            _filePathFull = serializedObject.FindProperty("FilePathFull");
            _fileType = serializedObject.FindProperty("FileType");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            _instance.UpdateFilePath();

            // Filename
            EditorGUILayout.PropertyField(_fileName, _fileNameContent);

            // File Type
            EditorGUILayout.PropertyField(_fileType, _fileTypeContent);

            // Output path type
            EditorGUILayout.PropertyField(_showAbsolute);
            if (_showAbsolute.boolValue)
            {
                EditorGUILayout.PropertyField(_absolutePath, _absolutePathContent);
            }
            else
            {
                EditorGUILayout.LabelField("using Application.persistentDataPath:\t" + Application.persistentDataPath);
            }

            // Open output path, copy html to output path
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Open Output Path"))
            {
                OpenInFileBrowser.Open(_filePath.stringValue);
            }
            if (GUILayout.Button("Copy HTML to Output Path"))
            {
                _copyPath = _filePath.stringValue.Replace('\\', '/');
                if (!_copyPath.EndsWith("/")) { _copyPath += "/"; }
                _copyPath += "UnityFileDebugViewer.html";
                FileUtil.ReplaceFile("Assets/UnityFileDebug/Lib/Viewer/UnityFileDebugViewer.html", _copyPath);
            }
            EditorGUILayout.EndHorizontal();

            // If running, show full output path and count
            if (Application.isPlaying)
            {
                EditorGUILayout.Separator();
                EditorGUILayout.Separator();
                EditorGUILayout.Separator();
                EditorGUILayout.Separator();
                EditorGUILayout.Separator();

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Copy Output Filepath"))
                {
                    EditorGUIUtility.systemCopyBuffer = _filePathFull.stringValue;
                }
                EditorGUILayout.LabelField(_filePathFull.stringValue);
                EditorGUILayout.EndHorizontal();
                // EditorGUILayout.LabelField("Logs added: " + instance.count);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}