using UnityEngine;

namespace CheesyUtils.Inputs
{
    /// <summary>
    /// Central dispatcher that selects and delegates to the active input provider.
    /// </summary>
    [DefaultExecutionOrder(-100)] // ensure it updates before gameplay scripts
    public class InputManager : Singleton<InputManager>
    {
        // --- Serialized Fields ---
        [SerializeField] private bool _forceTouchProvider;
        [SerializeField] private bool _forceXRProvider;

        // --- Private Fields ---
        private IInputProvider _activeProvider;

        // --- Properties ---
        public IInputProvider Current => _activeProvider;

        // --- Unity Lifecycle ---
        protected override void Awake()
        {
            base.Awake();
            SelectProvider();
            _activeProvider?.Enable();
        }

        private void OnDestroy()
        {
            _activeProvider?.Disable();
        }

        private void Update()
        {
            // Example usage proxy (optional)
            // Could broadcast events or feed gameplay systems.
        }

        // --- Private Helpers ---
        private void SelectProvider()
        {
#if UNITY_XR
            if (_forceXRProvider || UnityEngine.XR.XRSettings.enabled)
            {
                _activeProvider = gameObject.AddComponent<XRInputProvider>();
                return;
            }
#endif
#if UNITY_ANDROID || UNITY_IOS
            if (_forceTouchProvider || Application.isMobilePlatform)
            {
                _activeProvider = gameObject.AddComponent<TouchInputProvider>();
                return;
            }
#endif
            if (UnityEngine.InputSystem.Gamepad.current != null)
            {
                _activeProvider = gameObject.AddComponent<GamepadInputProvider>();
            }
            else
            {
                _activeProvider = gameObject.AddComponent<KeyboardMouseInputProvider>();
            }
        }
    }
}