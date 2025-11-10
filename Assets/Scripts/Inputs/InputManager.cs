using UnityEngine;
using UnityEngine.InputSystem;

namespace CheesyUtils.Inputs
{
    public class InputManager : Singleton<InputManager>
    {
        // ---- / Public Variables / ---- //
        
        #region Navigation
        public Vector2 navigationInput { get; private set; }

        public static bool WasSubmitPressed;

        public static bool WasEscapePressed;
        #endregion
        
        #region Gameplay
        public static bool WasAttackPressed;
        public static bool WasAttackReleased;
        public static bool IsAttackBeingPressed;
        public static bool WasJumpPressed;
        public static bool WasInteractPressed;
        public static Vector2 MousePosition;
        #endregion
        
        #region Minitropolis
        public static Vector2 PanAxis;
        public static bool IsPanPressed;
        public static bool IsRotatePressed;
        public static float ZoomAxis;
        #endregion
        
        #region Others
        public static bool WasToggleDevConsolePressed;
        public static bool WasDebugSnapshotPressed;
        #endregion

        // ---- / Private Variables / ---- //
        private static PlayerInput _playerInput;

        #region Navigation
        private InputAction _navigationAction;

        private InputAction _submitAction;

        private InputAction _escapeAction;
        #endregion
        
        #region Gameplay
        private InputAction _attackAction;
        private InputAction _jumpAction;
        private InputAction _interactAction;
        private InputAction _mousePositionAction;
        #endregion

        #region Minitropolis
        private InputAction _panAxisAction;
        private InputAction _panAction;
        private InputAction _rotateAction;
        private InputAction _zoomAction;
        #endregion
        
        #region Others
        private InputAction _devConsoleAction;
        private InputAction _debugSnapshotAction;
        #endregion

        protected override void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            _playerInput.camera = Camera.main;
            
            #region Navigation
            _navigationAction = _playerInput.actions["Navigate"];

            _submitAction = _playerInput.actions["Submit"];

            _escapeAction = _playerInput.actions["Escape"];
            #endregion
            
            #region Gameplay
            _attackAction = _playerInput.actions["Attack"];
            _jumpAction = _playerInput.actions["Jump"];
            _interactAction = _playerInput.actions["Interact"];
            _mousePositionAction = _playerInput.actions["Point"];
            #endregion

            #region Minitropolis
            _panAxisAction = _playerInput.actions["PanAxis"];
            _panAction = _playerInput.actions["Pan"];
            _rotateAction = _playerInput.actions["Rotate"];
            _zoomAction = _playerInput.actions["Zoom"];
            #endregion
            
            #region Others
            _devConsoleAction = _playerInput.actions["ToggleDevConsole"];
            _debugSnapshotAction = _playerInput.actions["DebugSnapshot"];
            #endregion
        }

        private void Update()
        {
            #region Navigation
            navigationInput = _navigationAction.ReadValue<Vector2>();

            WasSubmitPressed = _submitAction.WasPerformedThisFrame();

            WasEscapePressed = _escapeAction.WasPressedThisFrame();
            #endregion
            
            #region Gameplay
            WasAttackPressed = _attackAction.WasPerformedThisFrame();
            WasAttackReleased = _attackAction.WasReleasedThisFrame();
            IsAttackBeingPressed = _attackAction.IsPressed();
            
            WasJumpPressed = _jumpAction.WasPerformedThisFrame();
            WasInteractPressed = _interactAction.WasPerformedThisFrame();

            MousePosition = _mousePositionAction.ReadValue<Vector2>();
            #endregion

            #region Minitropolis
            PanAxis = _panAxisAction.ReadValue<Vector2>();
            IsPanPressed = _panAction.IsPressed();
            IsRotatePressed = _rotateAction.IsPressed();
            ZoomAxis = _zoomAction.ReadValue<float>();
            #endregion
            
            #region Others
            WasToggleDevConsolePressed = _devConsoleAction.WasPerformedThisFrame();
            WasDebugSnapshotPressed = _debugSnapshotAction.WasPerformedThisFrame();
            #endregion
        }
    }
}