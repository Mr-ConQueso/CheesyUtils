using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
using ReadOnlyArray = UnityEngine.InputSystem.Utilities.ReadOnlyArray<UnityEngine.InputSystem.EnhancedTouch.Touch>;

namespace CheesyUtils.Inputs
{
    /// <summary>
    /// Handles all input for touchscreen devices.
    /// Interprets pinch, rotate, and drag gestures using the EnhancedTouch API.
    /// </summary>
    [DefaultExecutionOrder(-99)]
    public class TouchInputProvider : MonoBehaviour, IInputProvider
    {
        // --- Serialized Fields ---
        [SerializeField] [Tooltip("Minimum distance in pixels to start recognizing a pinch gesture.")]
        private float _pinchThreshold = 15.0f;

        [SerializeField] [Tooltip("Minimum angular change (degrees) to start a rotation gesture.")]
        private float _rotationThreshold = 1.0f;

        [SerializeField] [Tooltip("Minimum distance (pixels) between two touches to allow rotation detection.")]
        private float _minRotationDistance = 10.0f;

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

        private Vector2 _rotationStartVector;
        private bool _isRotating;

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
            EnhancedTouchSupport.Enable();
        }

        public void Disable()
        {
            EnhancedTouchSupport.Disable();
        }

        private void Update()
        {
            ReadOnlyArray touches = Touch.activeTouches;
            if (touches.Count == 0)
            {
                ResetFrameValues();
                return;
            }

            UpdateTouchGestures(touches);
        }

        // --- Private Helpers ---
        private void ResetFrameValues()
        {
            _zoomAxis = 0f;
            _lookInput = Vector2.zero;
            _mousePosition = Vector2.zero;
            _rotateHeld = false;
            _panHeld = false;
        }

        private void UpdateTouchGestures(ReadOnlyArray touches)
        {
            if (touches.Count == 1)
            {
                // Single-finger drag = pan
                _panHeld = true;
                _mousePosition = touches[0].screenPosition;
                _lookInput = touches[0].delta;
                _zoomAxis = 0f;
                _rotateHeld = false;
                return;
            }

            if (touches.Count < 2)
            {
                _panHeld = false;
                _rotateHeld = false;
                _zoomAxis = 0f;
                return;
            }

            // Two-finger gestures
            Touch t0 = touches[0];
            Touch t1 = touches[1];

            if (t0.phase == TouchPhase.Began || t1.phase == TouchPhase.Began)
                _rotationStartVector = t1.screenPosition - t0.screenPosition;

            Vector2 currentVector = t1.screenPosition - t0.screenPosition;
            float angle = Vector2.SignedAngle(_rotationStartVector, currentVector);
            float distance = _rotationStartVector.magnitude;

            if (Mathf.Abs(angle) > _rotationThreshold && distance > _minRotationDistance)
            {
                _lookInput.x = angle;
                _isRotating = true;
                _rotateHeld = true;
            }
            else
            {
                _rotateHeld = false;
                _isRotating = false;
                _lookInput.x = 0f;
            }

            // Pinch zoom (only if not rotating)
            if (!_isRotating)
            {
                Vector2 prev0 = t0.screenPosition - t0.delta;
                Vector2 prev1 = t1.screenPosition - t1.delta;
                float prevDistance = Vector2.Distance(prev0, prev1);
                float currentDistance = Vector2.Distance(t0.screenPosition, t1.screenPosition);
                float delta = currentDistance - prevDistance;

                if (Mathf.Abs(delta) >= _pinchThreshold)
                    _zoomAxis = delta * 0.01f;
                else
                    _zoomAxis = 0f;
            }

            _rotationStartVector = currentVector;
        }
    }
}