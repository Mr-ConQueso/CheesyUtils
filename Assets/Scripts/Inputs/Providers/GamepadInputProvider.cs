using UnityEngine;
using UnityEngine.InputSystem;

namespace CheesyUtils.Inputs
{
    /// <summary>
    /// Handles all input from standard gamepads using the Unity Input System.
    /// Provides navigation, camera control, and gameplay actions mapped to controller sticks and buttons.
    /// </summary>
    [DefaultExecutionOrder(-99)]
    public class GamepadInputProvider : MonoBehaviour, IInputProvider
    {
        // --- Serialized Fields ---
        [SerializeField] private InputActionAsset _inputAsset;

        // --- Non-serialized Fields ---
        private InputAction _navigateAction;
        private InputAction _lookAction;
        private InputAction _submitAction;
        private InputAction _escapeAction;
        private InputAction _attackAction;
        private InputAction _jumpAction;
        private InputAction _interactAction;
        private InputAction _zoomPositiveAction;
        private InputAction _zoomNegativeAction;
        private InputAction _panAction;
        private InputAction _rotateAction;

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
        public Vector2 MousePosition => _mousePosition;   // not used on pad, stays zero
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
                Debug.LogError("[GamepadInputProvider] Missing InputActionAsset reference.");
                return;
            }

            _inputAsset.Enable();
            CacheActions();
        }

        public void Disable()
        {
            if (_inputAsset)
                _inputAsset.Disable();
        }

        private void Update()
        {
            if (_navigateAction == null) return;

            _navigationInput = _navigateAction.ReadValue<Vector2>();
            _lookInput = _lookAction?.ReadValue<Vector2>() ?? Vector2.zero;

            _rotateHeld = _rotateAction?.IsPressed() ?? false;
            _panHeld = _panAction?.IsPressed() ?? false;

            _attackPressed = _attackAction?.WasPerformedThisFrame() ?? false;
            _jumpPressed = _jumpAction?.WasPerformedThisFrame() ?? false;
            _interactPressed = _interactAction?.WasPerformedThisFrame() ?? false;
            _submitPressed = _submitAction?.WasPerformedThisFrame() ?? false;
            _escapePressed = _escapeAction?.WasPerformedThisFrame() ?? false;

            // Shoulder zoom mapping
            float zoomUp = _zoomPositiveAction?.ReadValue<float>() ?? 0f;
            float zoomDown = _zoomNegativeAction?.ReadValue<float>() ?? 0f;
            _zoomAxis = zoomUp - zoomDown;
        }

        // --- Private Helpers ---
        private void CacheActions()
        {
            _navigateAction = _inputAsset.FindAction("Navigate");
            _lookAction = _inputAsset.FindAction("Look");
            _submitAction = _inputAsset.FindAction("Submit");
            _escapeAction = _inputAsset.FindAction("Escape");
            _attackAction = _inputAsset.FindAction("Attack");
            _jumpAction = _inputAsset.FindAction("Jump");
            _interactAction = _inputAsset.FindAction("Interact");
            _rotateAction = _inputAsset.FindAction("Rotate");
            _panAction = _inputAsset.FindAction("Pan");
            _zoomPositiveAction = _inputAsset.FindAction("ZoomIn");
            _zoomNegativeAction = _inputAsset.FindAction("ZoomOut");

            Debug.Assert(_navigateAction != null, "Missing Navigate action in Gamepad map.");
            Debug.Assert(_lookAction != null, "Missing Look action in Gamepad map.");
        }
    }
}