using UnityEngine;
using UnityEngine.InputSystem;

namespace CheesyUtils.Inputs
{
    /// <summary>
    /// Handles keyboard and mouse input using the Unity Input System.
    /// Provides navigation, gameplay, and camera controls for desktop users.
    /// </summary>
    [DefaultExecutionOrder(-99)]
    public class KeyboardMouseInputProvider : MonoBehaviour, IInputProvider
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
        private InputAction _zoomAction;
        private InputAction _panAction;
        private InputAction _rotateAction;
        private InputAction _mousePosAction;

        // --- Cached Values ---
        private Vector2 _navigationInput;
        private Vector2 _lookInput;
        private Vector2 _mousePosition;
        private float _zoomAxis;
        private bool _panHeld;
        private bool _rotateHeld;
        private bool _attackPressed;
        private bool _jumpPressed;
        private bool _interactPressed;
        private bool _submitPressed;
        private bool _escapePressed;

        // --- Properties (IInputProvider) ---
        public Vector2 NavigationInput => _navigationInput;
        public Vector2 LookInput => _lookInput;
        public Vector2 MousePosition => _mousePosition;
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
                Debug.LogError("[KeyboardMouseInputProvider] Missing InputActionAsset reference.");
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
            _mousePosition = _mousePosAction?.ReadValue<Vector2>() ?? Vector2.zero;
            _zoomAxis = _zoomAction?.ReadValue<float>() ?? 0f;

            _panHeld = _panAction?.IsPressed() ?? false;
            _rotateHeld = _rotateAction?.IsPressed() ?? false;

            _attackPressed = _attackAction?.WasPerformedThisFrame() ?? false;
            _jumpPressed = _jumpAction?.WasPerformedThisFrame() ?? false;
            _interactPressed = _interactAction?.WasPerformedThisFrame() ?? false;
            _submitPressed = _submitAction?.WasPerformedThisFrame() ?? false;
            _escapePressed = _escapeAction?.WasPerformedThisFrame() ?? false;
        }

        // --- Private Helpers ---
        private void CacheActions()
        {
            _navigateAction = _inputAsset.FindAction("Navigate");
            _lookAction = _inputAsset.FindAction("Look");
            _mousePosAction = _inputAsset.FindAction("Point");
            _zoomAction = _inputAsset.FindAction("Zoom");
            _panAction = _inputAsset.FindAction("Pan");
            _rotateAction = _inputAsset.FindAction("Rotate");

            _attackAction = _inputAsset.FindAction("Attack");
            _jumpAction = _inputAsset.FindAction("Jump");
            _interactAction = _inputAsset.FindAction("Interact");
            _submitAction = _inputAsset.FindAction("Submit");
            _escapeAction = _inputAsset.FindAction("Escape");

            // Validation
            Debug.Assert(_navigateAction != null, "Missing Navigate action");
            Debug.Assert(_lookAction != null, "Missing Look action");
        }
    }
}