using UnityEngine;
using UnityEngine.InputSystem;

namespace CSVS.GodMode.Player
{
    public class SuperheroCameraController : MonoBehaviour
    {
        [Header("Target & Follow")]
        [SerializeField] private Transform _target;
        [SerializeField] private Vector3 _followOffset = new Vector3(0.0f, 2.5f, -6.0f);
        [SerializeField] private float _followSmoothTime = 0.08f;

        [Header("Look Orbit")]
        [SerializeField] private float _mouseSensitivity = 2.0f;
        [SerializeField] private float _minPitch = -75.0f;
        [SerializeField] private float _maxPitch = 85.0f;

        [Header("Dynamic FOV")]
        [SerializeField] private Camera _camera;
        [SerializeField] private float _normalFOV = 60.0f;
        [SerializeField] private float _supersonicFOV = 85.0f;
        [SerializeField] private float _fovLerpSpeed = 5.0f;

        [Header("Camera Shake")]
        [SerializeField] private float _shakeDamping = 3.0f;

        private float _yaw = 0.0f;
        private float _pitch = 10.0f;
        private Vector3 _currentFollowVelocity;
        private Vector3 _shakeOffset = Vector3.zero;
        private float _currentShakeIntensity = 0.0f;
        private SuperheroFlightController _flightController;

        private void Start()
        {
            if (_camera == null) _camera = GetComponent<Camera>();
            if (_camera == null) _camera = Camera.main;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (_target != null)
            {
                _flightController = _target.GetComponent<SuperheroFlightController>();
                if (_flightController != null)
                {
                    _flightController.OnGroundSlamImpact += TriggerShakeOnSlam;
                }
            }
        }

        private void OnDestroy()
        {
            if (_flightController != null)
            {
                _flightController.OnGroundSlamImpact -= TriggerShakeOnSlam;
            }
        }

        private void LateUpdate()
        {
            if (_target == null) return;

            HandleMouseLook();
            HandleFollow();
            HandleDynamicFOV();
            UpdateCameraShake();
        }

        private void HandleMouseLook()
        {
            if (Mouse.current == null || Cursor.lockState != CursorLockMode.Locked) return;

            Vector2 mouseDelta = Mouse.current.delta.ReadValue() * (_mouseSensitivity * 0.1f);
            _yaw += mouseDelta.x;
            _pitch -= mouseDelta.y;
            _pitch = Mathf.Clamp(_pitch, _minPitch, _maxPitch);
        }

        private void HandleFollow()
        {
            Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0.0f);
            Vector3 desiredPosition = _target.position + (rotation * _followOffset) + _shakeOffset;

            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref _currentFollowVelocity, _followSmoothTime);
            transform.LookAt(_target.position + Vector3.up * 1.5f);
        }

        private void HandleDynamicFOV()
        {
            if (_camera == null) return;

            bool isBoosting = _flightController != null && _flightController.IsBoosting;
            float targetFOV = isBoosting ? _supersonicFOV : _normalFOV;
            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, targetFOV, Time.deltaTime * _fovLerpSpeed);
        }

        private void UpdateCameraShake()
        {
            if (_currentShakeIntensity > 0.001f)
            {
                _shakeOffset = Random.insideUnitSphere * _currentShakeIntensity;
                _currentShakeIntensity = Mathf.Lerp(_currentShakeIntensity, 0.0f, Time.deltaTime * _shakeDamping);
            }
            else
            {
                _shakeOffset = Vector3.zero;
            }
        }

        public void TriggerShake(float intensity)
        {
            _currentShakeIntensity = Mathf.Max(_currentShakeIntensity, intensity);
        }

        private void TriggerShakeOnSlam(Vector3 point, float force)
        {
            TriggerShake(1.2f);
        }
    }
}
