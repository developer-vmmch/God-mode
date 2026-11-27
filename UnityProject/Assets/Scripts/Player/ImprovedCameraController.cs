// ImprovedCameraController.cs
// CSVS GOD MODE — V0.3 Professional Third-Person Camera
// Smooth orbit, collision prevention, FOV transitions,
// mode-aware switching (ground / flight / vehicle / combat).

using UnityEngine;
using CSVS.GodMode.Player;
using CSVS.GodMode.Core;

namespace CSVS.GodMode.Player
{
    public class ImprovedCameraController : MonoBehaviour
    {
        public enum CameraMode
        {
            ThirdPerson,
            FirstPerson,
            Flight,
            HighSpeedFlight,
            Vehicle,
            Motorcycle,
            Combat,
        }

        // ─── Inspector ─────────────────────────────────────────────────────
        [Header("Target")]
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 shoulderOffset = new Vector3(0.4f, 1.65f, 0f);

        [Header("Distance")]
        [SerializeField] private float defaultDistance  = 4.5f;
        [SerializeField] private float flightDistance   = 6.0f;
        [SerializeField] private float vehicleDistance  = 5.5f;
        [SerializeField] private float minDistance      = 1.2f;
        [SerializeField] private float maxDistance      = 14f;
        [SerializeField] private float distanceScrollSpeed = 2.5f;

        [Header("Sensitivity")]
        [SerializeField] private float lookSensitivity  = 2.5f;
        [SerializeField] private float smoothTime       = 0.08f;

        [Header("Clamp")]
        [SerializeField] private float minPitch = -25f;
        [SerializeField] private float maxPitch =  60f;

        [Header("Collision")]
        [SerializeField] private LayerMask collisionMask;
        [SerializeField] private float cameraRadius    = 0.3f;
        [SerializeField] private float collisionBuffer = 0.15f;

        [Header("FOV")]
        [SerializeField] private float defaultFOV         = 65f;
        [SerializeField] private float highSpeedFOV       = 90f;
        [SerializeField] private float combatFOV          = 58f;
        [SerializeField] private float fovTransitionSpeed = 5f;

        // ─── State ─────────────────────────────────────────────────────────
        public CameraMode ActiveMode { get; private set; } = CameraMode.ThirdPerson;

        private float _yaw, _pitch;
        private float _currentDistance;
        private Vector3 _velocitySmooth;
        private Camera _cam;
        private bool _firstPerson;

        // ─── Lifecycle ─────────────────────────────────────────────────────
        private void Awake()
        {
            _cam = GetComponent<Camera>() ?? Camera.main;
            _currentDistance = defaultDistance;

            // Hide & lock cursor in-game
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible   = false;
        }

        private void LateUpdate()
        {
            if (!target) return;

            HandleInput();
            UpdateDistance();
            PositionCamera();
            UpdateFOV();
        }

        // ─── Input ─────────────────────────────────────────────────────────
        private void HandleInput()
        {
            if (GameInputManager.WasCameraToggleTriggered)
                ToggleFirstPerson();

            if (!_firstPerson)
            {
                Vector2 look = GameInputManager.LookDelta * lookSensitivity * Time.deltaTime * 60f;
                _yaw   += look.x;
                _pitch -= look.y;
                _pitch  = Mathf.Clamp(_pitch, minPitch, maxPitch);
            }

            // Scroll zoom
            float scroll = GameInputManager.ScrollDelta;
            _currentDistance = Mathf.Clamp(_currentDistance - scroll * distanceScrollSpeed, minDistance, maxDistance);
        }

        // ─── Distance by mode ──────────────────────────────────────────────
        private void UpdateDistance()
        {
            float targetDist = ActiveMode switch {
                CameraMode.Flight          => flightDistance,
                CameraMode.HighSpeedFlight => flightDistance + 2f,
                CameraMode.Vehicle         => vehicleDistance,
                CameraMode.Motorcycle      => vehicleDistance - 1f,
                _                          => defaultDistance,
            };

            if (_firstPerson) targetDist = 0f;
            _currentDistance = Mathf.Lerp(_currentDistance, targetDist, Time.deltaTime * 5f);
        }

        // ─── Camera position & collision ───────────────────────────────────
        private void PositionCamera()
        {
            if (_firstPerson)
            {
                transform.position = target.position + shoulderOffset;
                transform.rotation = Quaternion.Euler(_pitch, _yaw, 0f);
                return;
            }

            Quaternion rotation  = Quaternion.Euler(_pitch, _yaw, 0f);
            Vector3   pivotPoint = target.position + target.TransformVector(shoulderOffset);
            Vector3   desiredPos = pivotPoint - rotation * Vector3.forward * _currentDistance;

            // Sphere-cast collision check
            Vector3 actualPos = SolveCollision(pivotPoint, desiredPos);

            transform.position = Vector3.SmoothDamp(transform.position, actualPos, ref _velocitySmooth, smoothTime);
            transform.LookAt(pivotPoint);
        }

        private Vector3 SolveCollision(Vector3 pivot, Vector3 desired)
        {
            Vector3 dir = (desired - pivot).normalized;
            float dist  = Vector3.Distance(pivot, desired);

            if (Physics.SphereCast(pivot, cameraRadius, dir, out RaycastHit hit, dist, collisionMask))
                return pivot + dir * (hit.distance - collisionBuffer);

            return desired;
        }

        // ─── FOV ───────────────────────────────────────────────────────────
        private void UpdateFOV()
        {
            if (!_cam) return;
            float targetFOV = ActiveMode switch {
                CameraMode.HighSpeedFlight => highSpeedFOV,
                CameraMode.Combat          => combatFOV,
                _                          => defaultFOV,
            };
            _cam.fieldOfView = Mathf.Lerp(_cam.fieldOfView, targetFOV, Time.deltaTime * fovTransitionSpeed);
        }

        // ─── API ───────────────────────────────────────────────────────────
        public void SetPlayerMode(PlayerMode mode)
        {
            ActiveMode = mode == PlayerMode.Superhero ? CameraMode.Flight : CameraMode.ThirdPerson;
        }

        public void SetCameraMode(CameraMode mode)
        {
            ActiveMode = mode;
        }

        public void SetHighSpeedMode(bool highSpeed)
        {
            if (ActiveMode == CameraMode.Flight || ActiveMode == CameraMode.HighSpeedFlight)
                ActiveMode = highSpeed ? CameraMode.HighSpeedFlight : CameraMode.Flight;
        }

        private void ToggleFirstPerson()
        {
            _firstPerson = !_firstPerson;
            ActiveMode   = _firstPerson ? CameraMode.FirstPerson : CameraMode.ThirdPerson;
        }
    }
}

