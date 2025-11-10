using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

namespace CheesyUtils.Inputs
{
    /// <summary>
    /// Handles input for XR devices (headset & motion controllers).
    /// Reads tracking pose, thumb-sticks, triggers, and grip buttons,
    /// translating them to the common IInputProvider interface.
    /// </summary>
    [DefaultExecutionOrder(-99)]
    public class XRInputProvider : MonoBehaviour, IInputProvider
    {
        // --- Serialized Fields ---
        [SerializeField, Tooltip("Input action asset containing XR bindings.")]
        private InputActionAsset _inputAsset;

        [SerializeField, Tooltip("Headset transform for orientation & movement reference.")]
        private Transform _xrCamera;

        // --- Non-serialized Fields ---
        private InputAction _moveAction;
        private InputAction _lookAction;
        private InputAction _triggerAction;
        private InputAction _gripAction;
        private InputAction _primaryButtonAction;
        private InputAction _secondaryButtonAction;
        private InputAction _zoomAction;

        // --- Cached Values ---
        private Vector2 _navigationInput;
        private Vector2 _lookInput;
        private Vector2 _mousePosition;
        private float _zoomAxis;
        private bool _rotateHeld;
        private bool _panHeld;
        private bool _attackPressed;
        private bool _jumpPressed;
        private bool _interactPressed;
        private bool _submitPressed;
        private bool _escapePressed;

        // --- Properties (IInputProvider) ---
        public Vector2 NavigationInput => _navigationInput;
        public Vector2 LookInput => _lookInput;
        public Vector2 MousePosition => _mousePosition; // Not used in XR
        public bool SubmitPressed => _submitPressed;
        public bool EscapePressed => _escapePressed;
        public bool AttackPressed => _attackPressed;
        public bool JumpPressed => _jumpPressed;
        public bool InteractPressed => _interactPressed;
        public float ZoomAxis => _zoomAxis;
        public bool RotateHeld => _rotateHeld;
        public bool PanHeld => _panHeld;

        // --- Unity Lifecycle ---
        public void Enable()
        {
            if (!_inputAsset)
            {
                Debug.LogError("[XRInputProvider] Missing InputActionAsset reference.");
                return;
            }

            _inputAsset.Enable();
            CacheActions();

            if (!_xrCamera && Camera.main)
                _xrCamera = Camera.main.transform;
        }

        public void Disable()
        {
            if (_inputAsset)
                _inputAsset.Disable();
        }

        private void Update()
        {
            if (!XRSettings.enabled) return;

            _navigationInput = _moveAction?.ReadValue<Vector2>() ?? Vector2.zero;
            _lookInput = _lookAction?.ReadValue<Vector2>() ?? Vector2.zero;

            _attackPressed = _triggerAction?.WasPerformedThisFrame() ?? false;
            _interactPressed = _gripAction?.WasPerformedThisFrame() ?? false;
            _submitPressed = _primaryButtonAction?.WasPerformedThisFrame() ?? false;
            _escapePressed = _secondaryButtonAction?.WasPerformedThisFrame() ?? false;

            _rotateHeld = _gripAction?.IsPressed() ?? false;
            _panHeld = _triggerAction?.IsPressed() ?? false;
            _zoomAxis = _zoomAction?.ReadValue<float>() ?? 0f;

            // Optionally, update orientation-based look input
            if (!_xrCamera) return;

            Vector3 forward = _xrCamera.forward;
            _lookInput = new Vector2(forward.x, forward.z);
        }

        // --- Private Helpers ---
        private void CacheActions()
        {
            _moveAction = _inputAsset.FindAction("Move");
            _lookAction = _inputAsset.FindAction("Look");
            _triggerAction = _inputAsset.FindAction("Trigger");
            _gripAction = _inputAsset.FindAction("Grip");
            _primaryButtonAction = _inputAsset.FindAction("Primary");
            _secondaryButtonAction = _inputAsset.FindAction("Secondary");
            _zoomAction = _inputAsset.FindAction("Zoom");

            Debug.Assert(_moveAction != null, "Missing Move action in XR map.");
            Debug.Assert(_triggerAction != null, "Missing Trigger action in XR map.");
        }
    }
}