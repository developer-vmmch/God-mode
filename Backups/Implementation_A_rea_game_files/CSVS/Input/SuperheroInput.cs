using UnityEngine;

namespace CSVS.Input
{
    /// <summary>
    /// Centralized input reader bridging Unity Input and gameplay controllers.
    /// Supports smooth fallback between standard and new input pipelines.
    /// </summary>
    public class SuperheroInput : MonoBehaviour
    {
        public static SuperheroInput Instance { get; private set; }

        [Header("Movement")]
        public Vector2 MoveVector { get; private set; }
        public Vector2 LookVector { get; private set; }
        public float VerticalFlight { get; private set; } // +1 = Space (Up), -1 = Ctrl (Down)
        public bool IsBoosting { get; private set; } // Left Shift

        [Header("Abilities")]
        public bool IsHeatVisionActive { get; private set; } // LMB or RMB
        public bool GrabPressed { get; private set; } // E key
        public bool GravityTetherHeld { get; private set; } // Q key held
        public bool GravityTetherReleased { get; private set; } // Q key released
        public bool SuperPunchPressed { get; private set; } // F key

        [Header("System")]
        public bool ToggleCameraPerspective { get; private set; } // V key
        public bool PausePressed { get; private set; } // P or Escape
        public bool SkipTutorialPressed { get; private set; } // T key

        [Header("Sensitivity Settings")]
        [Range(0.1f, 5.0f)] public float mouseSensitivity = 1.8f;
        public bool invertY = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Update()
        {
            ReadKeyboardAndMouse();
        }

        private void ReadKeyboardAndMouse()
        {
            // Move: W/S/A/D
            float h = UnityEngine.Input.GetAxisRaw("Horizontal");
            float v = UnityEngine.Input.GetAxisRaw("Vertical");
            MoveVector = new Vector2(h, v).normalized;

            // Look: Mouse X / Y
            float mouseX = UnityEngine.Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = UnityEngine.Input.GetAxis("Mouse Y") * mouseSensitivity * (invertY ? 1f : -1f);
            LookVector = new Vector2(mouseX, mouseY);

            // Vertical Flight: Space / Ctrl
            float vertical = 0f;
            if (UnityEngine.Input.GetKey(KeyCode.Space)) vertical += 1f;
            if (UnityEngine.Input.GetKey(KeyCode.LeftControl) || UnityEngine.Input.GetKey(KeyCode.C)) vertical -= 1f;
            VerticalFlight = vertical;

            // Boost
            IsBoosting = UnityEngine.Input.GetKey(KeyCode.LeftShift);

            // Superpowers
            IsHeatVisionActive = UnityEngine.Input.GetMouseButton(0) || UnityEngine.Input.GetMouseButton(1);
            GrabPressed = UnityEngine.Input.GetKeyDown(KeyCode.E);
            GravityTetherHeld = UnityEngine.Input.GetKey(KeyCode.Q);
            GravityTetherReleased = UnityEngine.Input.GetKeyUp(KeyCode.Q);
            SuperPunchPressed = UnityEngine.Input.GetKeyDown(KeyCode.F);

            // System
            ToggleCameraPerspective = UnityEngine.Input.GetKeyDown(KeyCode.V);
            PausePressed = UnityEngine.Input.GetKeyDown(KeyCode.P) || UnityEngine.Input.GetKeyDown(KeyCode.Escape);
            SkipTutorialPressed = UnityEngine.Input.GetKeyDown(KeyCode.T);
        }
    }
}
