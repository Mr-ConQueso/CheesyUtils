using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace CheesyUtils
{
    public class DebugSnapshot : MonoBehaviour
    {
        private const string SnapshotFolder = "DebugSnapshots";

        [Header("Settings")]
        public bool captureOnError = true;
        public int maxDepth = 3;

        private string snapshotPath;
        private HashSet<object> visitedObjects = new HashSet<object>();

        private void Awake()
        {
            Application.logMessageReceived += OnLogMessage;
            InitializeSnapshotSystem();
        }

        private void InitializeSnapshotSystem()
        {
            snapshotPath = Path.Combine(
                Application.persistentDataPath,
                SnapshotFolder,
                DateTime.UtcNow.ToString("ddMMyyyy_HHmmss")
            );
            Directory.CreateDirectory(snapshotPath);
        }

        private void OnLogMessage(string condition, string stackTrace, LogType type)
        {
            if (captureOnError && (type == LogType.Error || type == LogType.Exception))
            {
                var fileName = string.Concat("error_", DateTime.UtcNow.ToString("HHmmss"));
                CaptureSnapshot(fileName);
            }
        }

        public void CaptureSnapshot(string fileName)
        {
            StartCoroutine(CaptureSnapshotAsync(fileName));
        }

        private IEnumerator CaptureSnapshotAsync(string fileName)
        {
            var state = new SystemStateSnapshot
            {
                timestamp = DateTime.UtcNow,
                gameState = CaptureGameState(),
                SystemSnapshotState = CaptureSystemState()
            };

            string json = JsonUtility.ToJson(state, true);
            string filePath = Path.Combine(snapshotPath, $"{fileName}.json");
            
            File.WriteAllText(filePath, json);
            
            Debug.Log($"Snapshot saved: {filePath}");
            yield return null;
        }

        private GameState CaptureGameState()
        {
            visitedObjects.Clear();
            var state = new GameState();
            
            foreach (GameObject root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
            {
                state.gameObjects.Add(CaptureGameObjectState(root));
            }
            
            return state;
        }

        private GameObjectState CaptureGameObjectState(GameObject go, int currentDepth = 0)
        {
            if (currentDepth >= maxDepth) return null;
            if (visitedObjects.Contains(go)) return null;
            visitedObjects.Add(go);

            var state = new GameObjectState
            {
                name = go.name,
                active = go.activeSelf,
                position = go.transform.position,
                components = new List<ComponentState>()
            };

            foreach (Component component in go.GetComponents<Component>())
            {
                state.components.Add(CaptureComponentState(component, currentDepth + 1));
            }

            return state;
        }

        private ComponentState CaptureComponentState(Component component, int depth)
        {
            var state = new ComponentState
            {
                type = component.GetType().FullName,
                fields = new List<FieldValue>(),
                properties = new List<PropertyValue>()
            };

            // Capture fields
            foreach (FieldInfo field in component.GetType().GetFields(
                BindingFlags.Public | 
                BindingFlags.NonPublic | 
                BindingFlags.Instance))
            {
                try
                {
                    state.fields.Add(new FieldValue
                    {
                        name = field.Name,
                        value = GetValueString(field.GetValue(component), depth)
                    });
                }
                catch { /* Handle reflection errors */ }
            }

            // Capture properties
            foreach (PropertyInfo property in component.GetType().GetProperties(
                BindingFlags.Public | 
                BindingFlags.NonPublic | 
                BindingFlags.Instance))
            {
                try
                {
                    if (property.CanRead)
                    {
                        state.properties.Add(new PropertyValue
                        {
                            name = property.Name,
                            value = GetValueString(property.GetValue(component), depth)
                        });
                    }
                }
                catch { /* Handle reflection errors */ }
            }

            return state;
        }

        private string GetValueString(object value, int depth)
        {
            if (value == null) return "null";
            
            if (value is UnityEngine.Object unityObj)
            {
                return $"[UnityObject] {unityObj.name} ({unityObj.GetType().Name})";
            }

            if (value is IEnumerable enumerable && !(value is string))
            {
                var items = new List<string>();
                foreach (var item in enumerable)
                {
                    items.Add(GetValueString(item, depth));
                }
                return $"[{string.Join(", ", items)}]";
            }

            // Add special handling for complex types
            return value.ToString();
        }

        private SystemSnapshotState CaptureSystemState()
        {
            return new SystemSnapshotState
            {
                time = DateTime.UtcNow.ToString("o"),
                frameCount = Time.frameCount,
                systemMemory = SystemInfo.systemMemorySize,
                graphicsMemory = SystemInfo.graphicsMemorySize,
                // Add more system info as needed
            };
        }
    }

    [Serializable]
    public class SystemStateSnapshot
    {
        public DateTime timestamp;
        public GameState gameState;
        public SystemSnapshotState SystemSnapshotState;
    }

    [Serializable]
    public class GameState
    {
        public List<GameObjectState> gameObjects = new List<GameObjectState>();
    }

    [Serializable]
    public class GameObjectState
    {
        public string name;
        public bool active;
        public Vector3 position;
        public List<ComponentState> components;
    }

    [Serializable]
    public class ComponentState
    {
        public string type;
        public List<FieldValue> fields;
        public List<PropertyValue> properties;
    }

    [Serializable]
    public class FieldValue
    {
        public string name;
        public string value;
    }

    [Serializable]
    public class PropertyValue
    {
        public string name;
        public string value;
    }

    [Serializable]
    public class SystemSnapshotState
    {
        public string time;
        public int frameCount;
        public int systemMemory;
        public int graphicsMemory;
    }
}