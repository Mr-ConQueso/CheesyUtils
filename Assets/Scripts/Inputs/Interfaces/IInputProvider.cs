using UnityEngine;

namespace CheesyUtils.Inputs
{
    /// <summary>
    /// Base interface for all input providers (keyboard, gamepad, touch, XR).
    /// </summary>
    public interface IInputProvider
    {
        Vector2 NavigationInput { get; }
        Vector2 LookInput { get; }
        Vector2 MousePosition { get; }
        bool SubmitPressed { get; }
        bool EscapePressed { get; }
        bool AttackPressed { get; }
        bool JumpPressed { get; }
        bool InteractPressed { get; }
        float ZoomAxis { get; }
        bool RotateHeld { get; }
        bool PanHeld { get; }

        void Enable();
        void Disable();
    }
}