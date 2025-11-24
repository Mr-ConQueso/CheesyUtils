using UnityEngine;

namespace CheesyUtils {
    public class Singleton<T> : MonoBehaviour where T : Component {
        protected static T instance;

        public static bool HasInstance => instance;
        public static T TryGetInstance() => HasInstance ? instance : null;

        public static T Instance {
            get {
                if (!instance) {
                    instance = FindAnyObjectByType<T>();
                    if (!instance) {
                        var go = new GameObject(typeof(T).Name + " Auto-Generated");
                        instance = go.AddComponent<T>();
                    }
                }

                return instance;
            }
        }

        /// <summary>
        /// Make sure to call base.Awake() in override if you need awake.
        /// </summary>
        protected virtual void Awake() {
            InitializeSingleton();
        }

        protected virtual void InitializeSingleton() {
            if (!Application.isPlaying) return;

            instance = this as T;
        }
    }
}
