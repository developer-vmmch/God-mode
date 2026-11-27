using UnityEngine;
using UnityEngine.InputSystem;
using CSVS.GodMode.Core;

namespace CSVS.GodMode.Player
{
    public enum CameraPerspective
    {
        ThirdPerson = 0,
        FirstPerson = 1
    }

    /// <summary>
    /// Consolidated superhero camera controller.
    /// Supports First-Person and Third-Person perspectives (toggle with V),
    /// dynamic FOV acceleration zoom, anti-clipping collision sweeps,
    /// and trauma-based 6DOF camera shake.
    /// </summary>
    public class SuperheroCameraController : MonoBehaviour
    {
        public static SuperheroCameraController Instance { get; private set; }

        [Header("Target & Perspective")]
        [SerializeField] private Transform _target;
        [SerializeField] private CameraPerspective _perspective = CameraPerspective.ThirdPerson;
        [SerializeField] private Camera _camera;

        [Header("Offsets")]
        [SerializeField] private Vector3 _thirdPersonOffset = new Vector3(0.0f, 2.2f, -5.5f);
        [SerializeField] private Vector3 _firstPersonOffset = new Vector3(0.0f, 1.6f, 0.3f);
        [SerializeField] private float _followSmoothTime = 0.06f;
        [SerializeField] private LayerMask _collisionLayers = ~0;

        [Header("Look Orbit")]
        [SerializeField] private float _mouseSensitivity = 2.0f;
        [SerializeField] private float _minPitch = -75.0f;
        [SerializeField] private float _maxPitch = 85.0f;

        [Header("Dynamic FOV")]
        [SerializeField] private float _normalFOV = 60.0f;
        [SerializeField] private float _supersonicFOV = 85.0f;
        [SerializeField] private float _fovLerpSpeed = 5.0f;

        [Header("Trauma Camera Shake")]
        [SerializeField] private float _traumaDecayRate = 2.0f;
        [SerializeField] private float _maxShakeOffset = 0.4f;

        private float _yaw = 0.0f;
        private float _pitch = 10.0f;
        private Vector3 _currentFollowVelocity;
        private float _currentTrauma = 0.0f;
        private SuperheroFlightController _flightController;

        public CameraPerspective Perspective => _perspective;
        public Camera MainCamera => _camera;
        public Vector3 LookDirection => Quaternion.Euler(_pitch, _yaw, 0.0f) * Vector3.forward;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (_camera == null) _camera = GetComponent<Camera>();
            if (_camera == null) _camera = Camera.main;
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (_target != null)
            {
                SetTarget(_target);
            }
        }

        public void SetPlayerMode(object mode) { /* mode compatibility stub */ }

        public void SetTarget(Transform newTarget)
        {
            _target = newTarget;
            if (_target != null)
            {
                _flightController = _target.GetComponent<SuperheroFlightController>();
                _yaw = _target.eulerAngles.y;
            }
        }

        private void LateUpdate()
        {
            if (_target == null) return;

            HandleInput();
            HandlePerspectiveToggle();
            UpdateCameraTransform();
            UpdateDynamicFOV();
            UpdateTraumaShake();
        }

        private void HandleInput()
        {
            Vector2 delta = GameInputManager.MouseDelta * (_mouseSensitivity * 0.1f);
            _yaw += delta.x;
            _pitch -= delta.y;
            _pitch = Mathf.Clamp(_pitch, _minPitch, _maxPitch);
        }

        private void HandlePerspectiveToggle()
        {
            if (GameInputManager.WasCameraToggleTriggered)
            {
                _perspective = (_perspective == CameraPerspective.ThirdPerson)
                    ? CameraPerspective.FirstPerson
                    : CameraPerspective.ThirdPerson;
            }
        }

        private void UpdateCameraTransform()
        {
            Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0.0f);
            Vector3 desiredOffset = (_perspective == CameraPerspective.ThirdPerson) ? _thirdPersonOffset : _firstPersonOffset;
            Vector3 targetPivot = _target.position + Vector3.up * 1.5f;
            Vector3 desiredPosition = targetPivot + (rotation * desiredOffset);

            // Anti-clipping spherecast for 3rd person camera
            if (_perspective == CameraPerspective.ThirdPerson)
            {
                Vector3 rayDir = (desiredPosition - targetPivot).normalized;
                float targetDistance = Vector3.Distance(targetPivot, desiredPosition);

                if (Physics.SphereCast(targetPivot, 0.25f, rayDir, out RaycastHit hit, targetDistance, _collisionLayers))
                {
                    desiredPosition = targetPivot + rayDir * Mathf.Max(0.5f, hit.distance - 0.2f);
                }
            }

            // Apply trauma shake offset
            Vector3 shakeOffset = Vector3.zero;
            if (_currentTrauma > 0.001f)
            {
                float shakeMagnitude = Mathf.Pow(_currentTrauma, 2.0f) * _maxShakeOffset;
                shakeOffset = Random.insideUnitSphere * shakeMagnitude;
            }

            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition + shakeOffset, ref _currentFollowVelocity, _followSmoothTime);

            if (_perspective == CameraPerspective.ThirdPerson)
            {
                transform.LookAt(targetPivot);
            }
            else
            {
                transform.rotation = rotation;
            }
        }

        private void UpdateDynamicFOV()
        {
            if (_camera == null) return;

            bool isBoosting = _flightController != null && _flightController.IsBoosting;
            float targetFOV = isBoosting ? _supersonicFOV : _normalFOV;
            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, targetFOV, Time.deltaTime * _fovLerpSpeed);
        }

        private void UpdateTraumaShake()
        {
            if (_currentTrauma > 0.0f)
            {
                _currentTrauma = Mathf.Clamp01(_currentTrauma - Time.deltaTime * _traumaDecayRate);
            }
        }

        public void TriggerShake(float trauma)
        {
            _currentTrauma = Mathf.Clamp01(_currentTrauma + trauma);
        }
    }
}
