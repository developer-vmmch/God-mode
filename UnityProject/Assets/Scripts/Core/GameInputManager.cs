// GameInputManager.cs  — UPDATED V0.3
// CSVS GOD MODE — Unified input management
// Supports both property access and method calls seamlessly.

using UnityEngine;
using UnityEngine.InputSystem;

namespace CSVS.GodMode.Core
{
    public static class GameInputManager
    {
        // ─── Key mappings (for tests & remapping) ──────────────────────────
        public const Key KeySuperPunch       = Key.F;
        public const Key KeyVehicleInteract  = Key.G;
        public const Key KeyGravityTether    = Key.Q;
        public const Key KeyGrabInteract     = Key.E;
        public const Key KeyCameraToggle     = Key.V;
        public const Key KeyMapToggle        = Key.M;
        public const Key KeyTransform        = Key.T;

        private static Keyboard K => Keyboard.current;
        private static Mouse    M => Mouse.current;

        // ─── Movement ─────────────────────────────────────────────────────
        public static Vector2 MoveInput => new Vector2(
            (K != null ? (K.aKey.isPressed ? -1f : K.dKey.isPressed ? 1f : 0f) : 0f),
            (K != null ? (K.sKey.isPressed ? -1f : K.wKey.isPressed ? 1f : 0f) : 0f)
        );

        public static bool IsSprintHeld() => K != null && K.leftShiftKey.isPressed;
        public static bool WasJumpTriggered() => K != null && K.spaceKey.wasPressedThisFrame;

        // Flight controller aliases
        public static bool IsSprintOrBoostPressed => K != null && K.leftShiftKey.isPressed;
        public static bool WasAscendTriggered => K != null && K.spaceKey.wasPressedThisFrame;
        public static bool IsAscendPressed => K != null && K.spaceKey.isPressed;
        public static bool IsDescendPressed => K != null && (K.leftCtrlKey.isPressed || K.cKey.isPressed);

        // Vehicle: G key
        public static bool WasVehicleInteractTriggered => K != null && K.gKey.wasPressedThisFrame;
        public static bool WasVehicleInteractTriggeredMethod() => WasVehicleInteractTriggered;

        // Powers - Properties and methods
        public static bool WasSuperPunchTriggered => K != null && K.fKey.wasPressedThisFrame;
        public static bool WasGravityTetherTriggered => K != null && K.qKey.wasPressedThisFrame;
        public static bool WasGravityTetherReleased => K != null && K.qKey.wasReleasedThisFrame;
        public static bool WasGrabInteractTriggered => K != null && K.eKey.wasPressedThisFrame;
        public static bool WasPrimaryFireTriggered => M != null && M.leftButton.wasPressedThisFrame;
        public static bool IsPrimaryFireHeld => M != null && M.leftButton.isPressed;
        public static bool WasSecondaryFireTriggered => M != null && M.rightButton.wasPressedThisFrame;

        // Method variants for invocations
        public static bool WasSuperPunchTriggeredMethod() => WasSuperPunchTriggered;
        public static bool WasGravityTetherTriggeredMethod() => WasGravityTetherTriggered;
        public static bool WasGravityTetherReleasedMethod() => WasGravityTetherReleased;
        public static bool WasGrabInteractTriggeredMethod() => WasGrabInteractTriggered;
        public static bool WasPrimaryFireTriggeredMethod() => WasPrimaryFireTriggered;
        public static bool IsPrimaryFireHeldMethod() => IsPrimaryFireHeld;
        public static bool WasSecondaryFireTriggeredMethod() => WasSecondaryFireTriggered;

        // Transformation: T key (V0.3 new)
        public static bool WasTransformTriggered => K != null && K.tKey.wasPressedThisFrame;
        public static bool WasTransformTriggeredMethod() => WasTransformTriggered;

        // Camera
        public static bool WasCameraToggleTriggered => K != null && K.vKey.wasPressedThisFrame;
        public static bool WasCameraToggleTriggeredMethod() => WasCameraToggleTriggered;

        public static Vector2 LookDelta => M != null ? M.delta.ReadValue() : Vector2.zero;
        public static Vector2 MouseDelta => M != null ? M.delta.ReadValue() : Vector2.zero;
        public static float   ScrollDelta => M != null ? M.scroll.ReadValue().y : 0f;

        // UI
        public static bool WasMapToggleTriggered => K != null && K.mKey.wasPressedThisFrame;
        public static bool WasMapToggleTriggeredMethod() => WasMapToggleTriggered;
        public static bool WasPauseTriggered => K != null && K.escapeKey.wasPressedThisFrame;
        public static bool WasResetTriggered => K != null && K.rKey.wasPressedThisFrame;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        public static int WeatherPresetIndex()
        {
            if (K == null) return -1;
            if (K.numpad1Key.wasPressedThisFrame) return 0;
            if (K.numpad2Key.wasPressedThisFrame) return 1;
            if (K.numpad3Key.wasPressedThisFrame) return 2;
            if (K.numpad4Key.wasPressedThisFrame) return 3;
            return -1;
        }
#endif
    }
}
