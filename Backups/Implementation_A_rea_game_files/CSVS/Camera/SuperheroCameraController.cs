using UnityEngine;
using CSVS.Input;

namespace CSVS.Camera
{
    public enum CameraPerspective
    {
        ThirdPerson,
        FirstPerson
    }

    /// <summary>
    /// Cinematic superhero camera controller supporting 1st/3rd person modes,
    /// dynamic FOV acceleration zoom, spherecast collision clipping prevention,
    /// and trauma-based 6DOF camera shake.
    /// </summary>
    public class SuperheroCameraController : MonoBehaviour
    {
        public static SuperheroCameraController Instance { get; private set; }

        [Header("Target & Perspective")]
        [SerializeField] private Transform target;
        [SerializeField] private CameraPerspective perspective = CameraPerspective.ThirdPerson;
        [SerializeField] private UnityEngine.Camera targetCamera;

        [Header("Third Person Offsets")]
        [SerializeField] private Vector3 thirdPersonOffset = new Vector3(0f, 1.8f, -4.5f);
        [SerializeField] private Vector3 firstPersonOffset = new Vector3(0f, 1.6f, 0.2f);
        [SerializeField] private float minDistance = 1.0f;
        [SerializeField] private float maxDistance = 5.0f;
        [SerializeField] private LayerMask collisionLayers;

        [Header("Pitch & Yaw Limits")]
        [SerializeField] private float minPitch = -85f;
        [SerializeField] private float maxPitch = 85f;

        [Header("FOV & Boost Effects")]
        [SerializeField] private float baseFOV = 65f;
        [SerializeField] private float boostFOV = 88f;
        [SerializeField] private float fovDamping = 6f;

        [Header("Camera Shake (Trauma)")]
        [SerializeField] private float traumaDecayRate = 1.5f;
        [SerializeField] private float maxShakeAngle = 8f;
        [SerializeField] private float maxShakeOffset = 0.35f;

        private float currentYaw = 0f;
        private float currentPitch = 10f;
        private float currentDistance;
        private float currentTrauma = 0f;
        private float shakeTime = 0f;

        public CameraPerspective Perspective => perspective;
        public UnityEngine.Camera MainCamera => targetCamera;
        public Vector3 LookDirection => Quaternion.Euler(currentPitch, currentYaw, 0f) * Vector3.forward;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (targetCamera == null)
            {
                targetCamera = GetComponentInChildren<UnityEngine.Camera>();
                if (targetCamera == null) targetCamera = UnityEngine.Camera.main;
            }

            currentDistance = thirdPersonOffset.magnitude;
        }

        private void Start()
        {
            if (target != null)
            {
                Vector3 angles = target.eulerAngles;
                currentYaw = angles.y;
                currentPitch = angles.x;
            }
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        private void LateUpdate()
        {
            if (target == null) return;

            HandleInput();
            HandlePerspectiveToggle();
            UpdateRotationAndPosition();
            UpdateFOV();
            UpdateCameraShake();
        }

        private void HandleInput()
        {
            if (SuperheroInput.Instance == null) return;

            Vector2 look = SuperheroInput.Instance.LookVector;
            currentYaw += look.x;
            currentPitch -= look.y;
            currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);
        }

        private void HandlePerspectiveToggle()
        {
            if (SuperheroInput.Instance != null && SuperheroInput.Instance.ToggleCameraPerspective)
            {
                perspective = (perspective == CameraPerspective.ThirdPerson)
                    ? CameraPerspective.FirstPerson
                    : CameraPerspective.ThirdPerson;
            }
        }

        private void UpdateRotationAndPosition()
        {
            Quaternion targetRotation = Quaternion.Euler(currentPitch, currentYaw, 0f);

            if (perspective == CameraPerspective.FirstPerson)
            {
                Vector3 desiredPos = target.position + target.TransformDirection(firstPersonOffset);
                transform.position = desiredPos;
                transform.rotation = targetRotation;
            }
            else
            {
                Vector3 focusPoint = target.position + Vector3.up * thirdPersonOffset.y;
                Vector3 desiredDirection = targetRotation * Vector3.back;
                float desiredDist = -thirdPersonOffset.z;

                // Spherecast collision test to prevent wall clipping
                Ray ray = new Ray(focusPoint, desiredDirection);
                if (Physics.SphereCast(ray, 0.25f, out RaycastHit hit, desiredDist, collisionLayers, QueryTriggerInteraction.Ignore))
                {
                    currentDistance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
                }
                else
                {
                    currentDistance = Mathf.Lerp(currentDistance, desiredDist, Time.deltaTime * 10f);
                }

                transform.position = focusPoint + desiredDirection * currentDistance;
                transform.rotation = targetRotation;
            }
        }

        private void UpdateFOV()
        {
            if (targetCamera == null) return;

            bool isBoosting = SuperheroInput.Instance != null && SuperheroInput.Instance.IsBoosting;
            float targetFOV = isBoosting ? boostFOV : baseFOV;
            targetCamera.fieldOfView = Mathf.Lerp(targetCamera.fieldOfView, targetFOV, Time.deltaTime * fovDamping);
        }

        public void AddTrauma(float amount)
        {
            currentTrauma = Mathf.Clamp01(currentTrauma + amount);
        }

        private void UpdateCameraShake()
        {
            if (currentTrauma <= 0f) return;

            shakeTime += Time.deltaTime * 25f;
            float shakeIntensity = currentTrauma * currentTrauma; // Quadratic shake feel

            // Perlin noise rotational shake
            float yawShake = (Mathf.PerlinNoise(shakeTime, 0f) * 2f - 1f) * maxShakeAngle * shakeIntensity;
            float pitchShake = (Mathf.PerlinNoise(0f, shakeTime) * 2f - 1f) * maxShakeAngle * shakeIntensity;
            float rollShake = (Mathf.PerlinNoise(shakeTime, shakeTime) * 2f - 1f) * maxShakeAngle * 0.5f * shakeIntensity;

            // Perlin noise translational shake
            float xOffset = (Mathf.PerlinNoise(shakeTime + 10f, 0f) * 2f - 1f) * maxShakeOffset * shakeIntensity;
            float yOffset = (Mathf.PerlinNoise(0f, shakeTime + 10f) * 2f - 1f) * maxShakeOffset * shakeIntensity;

            transform.rotation *= Quaternion.Euler(pitchShake, yawShake, rollShake);
            transform.position += transform.right * xOffset + transform.up * yOffset;

            // Decay trauma over time
            currentTrauma = Mathf.MoveTowards(currentTrauma, 0f, Time.deltaTime * traumaDecayRate);
        }
    }
}
