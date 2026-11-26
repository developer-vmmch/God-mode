using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CSVS.GodMode.Core
{
    /// <summary>
    /// Centralized, configurable input manager for CSVS God Mode.
    /// Eliminates all input conflicts across superhero powers, camera perspectives,
    /// vehicle entry/exit, UI modals, and flight locomotion.
    /// Supports the new Unity Input System with fallback safety.
    /// </summary>
    public static class GameInputManager
    {
        // Configurable Input Bindings
        public static Key KeyMoveForward = Key.W;
        public static Key KeyMoveBackward = Key.S;
        public static Key KeyMoveLeft = Key.A;
        public static Key KeyMoveRight = Key.D;

        public static Key KeyJumpOrAscend = Key.Space;
        public static Key KeyDescend = Key.LeftCtrl;
        public static Key KeyDescendAlt = Key.C;
        public static Key KeySprintOrBoost = Key.LeftShift;

        public static Key KeyGrabInteract = Key.E;
        public static Key KeyGravityTether = Key.Q;
        public static Key KeySuperPunch = Key.F;
        public static Key KeyVehicleInteract = Key.G;
        public static Key KeyCameraToggle = Key.V;
        public static Key KeyMapToggle = Key.M;
        public static Key KeyReset = Key.R;
        public static Key KeyPause = Key.Escape;
        public static Key KeyPauseAlt = Key.P;

        public static Vector2 MoveInput
        {
            get
            {
                if (Keyboard.current == null) return Vector2.zero;
                float x = 0f;
                float y = 0f;

                if (Keyboard.current[KeyMoveRight].isPressed || Keyboard.current.rightArrowKey.isPressed) x += 1f;
                if (Keyboard.current[KeyMoveLeft].isPressed || Keyboard.current.leftArrowKey.isPressed) x -= 1f;
                if (Keyboard.current[KeyMoveForward].isPressed || Keyboard.current.upArrowKey.isPressed) y += 1f;
                if (Keyboard.current[KeyMoveBackward].isPressed || Keyboard.current.downArrowKey.isPressed) y -= 1f;

                Vector2 input = new Vector2(x, y);
                if (input.sqrMagnitude > 1f) input.Normalize();
                return input;
            }
        }

        public static Vector2 MouseDelta
        {
            get
            {
                if (Mouse.current == null || Cursor.lockState != CursorLockMode.Locked) return Vector2.zero;
                return Mouse.current.delta.ReadValue();
            }
        }

        public static bool IsAscendPressed => Keyboard.current != null && Keyboard.current[KeyJumpOrAscend].isPressed;
        public static bool WasAscendTriggered => Keyboard.current != null && Keyboard.current[KeyJumpOrAscend].wasPressedThisFrame;
        public static bool IsDescendPressed => Keyboard.current != null && (Keyboard.current[KeyDescend].isPressed || Keyboard.current[KeyDescendAlt].isPressed);
        public static bool IsSprintOrBoostPressed => Keyboard.current != null && (Keyboard.current[KeySprintOrBoost].isPressed || Keyboard.current.rightShiftKey.isPressed);

        public static bool WasSuperPunchTriggered => Keyboard.current != null && Keyboard.current[KeySuperPunch].wasPressedThisFrame;
        public static bool WasVehicleInteractTriggered => Keyboard.current != null && Keyboard.current[KeyVehicleInteract].wasPressedThisFrame;
        public static bool WasGrabInteractTriggered => Keyboard.current != null && Keyboard.current[KeyGrabInteract].wasPressedThisFrame;

        public static bool WasGravityTetherTriggered => Keyboard.current != null && Keyboard.current[KeyGravityTether].wasPressedThisFrame;
        public static bool WasGravityTetherReleased => Keyboard.current != null && Keyboard.current[KeyGravityTether].wasReleasedThisFrame;
        public static bool IsGravityTetherHeld => Keyboard.current != null && Keyboard.current[KeyGravityTether].isPressed;

        public static bool IsPrimaryFireHeld => Mouse.current != null && Mouse.current.leftButton.isPressed;
        public static bool WasPrimaryFireTriggered => Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        public static bool WasSecondaryFireTriggered => Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame;
        public static bool IsSecondaryFireHeld => Mouse.current != null && Mouse.current.rightButton.isPressed;

        public static bool WasCameraToggleTriggered => Keyboard.current != null && Keyboard.current[KeyCameraToggle].wasPressedThisFrame;
        public static bool WasMapToggleTriggered => Keyboard.current != null && Keyboard.current[KeyMapToggle].wasPressedThisFrame;
        public static bool WasResetTriggered => Keyboard.current != null && Keyboard.current[KeyReset].wasPressedThisFrame;
        public static bool WasPauseTriggered => Keyboard.current != null && (Keyboard.current[KeyPause].wasPressedThisFrame || Keyboard.current[KeyPauseAlt].wasPressedThisFrame);

        /// <summary>
        /// Generates a clean, context-sensitive string of on-screen control hints for the HUD.
        /// </summary>
        public static string GetCurrentControlHints(bool isFlying, bool isHoldingObject, bool isDriving, bool nearVehicle)
        {
            if (isDriving)
            {
                return "[W/S] Accelerate/Brake   [A/D] Steer   [SPACE] Handbrake   [G] Exit Vehicle   [V] Camera";
            }

            if (isHoldingObject)
            {
                return "[LMB] Throw Object   [RMB] Crush Object   [E] Drop   [WASD] Move   [V] Camera";
            }

            if (isFlying)
            {
                string baseHints = "[WASD] Fly   [L-SHIFT] Supersonic (95 m/s)   [SPACE] Ascend   [CTRL] Descend   [LMB] Heat Vision   [F] Super Punch   [Q] Tether";
                if (nearVehicle) baseHints += "   [G] Enter Vehicle";
                return baseHints;
            }

            // Ground Locomotion
            string groundHints = "[WASD] Move   [L-SHIFT] Sprint   [SPACE x2] Takeoff / Hover   [F] Super Punch   [E] Grab Car/Debris   [LMB] Heat Vision   [Q] Tether";
            if (nearVehicle) groundHints += "   [G] Enter Vehicle";
            return groundHints;
        }
    }
}
