using System;
using UnityEngine;
using UnityEngine.InputSystem;
using CSVS.GodMode.Destruction;

namespace CSVS.GodMode.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class SuperheroFlightController : MonoBehaviour
    {
        [Header("Flight Kinematics")]
        [SerializeField] private float _baseSpeed = 35.0f;           // 35 m/s (~126 km/h)
        [SerializeField] private float _supersonicSpeed = 90.0f;     // 90 m/s (~324 km/h)
        [SerializeField] private float _acceleration = 45.0f;
        [SerializeField] private float _drag = 4.0f;
        [SerializeField] private float _ascendSpeed = 25.0f;

        [Header("Hover Stabilization")]
        [SerializeField] private float _hoverBobbingFrequency = 1.5f;
        [SerializeField] private float _hoverBobbingAmplitude = 0.25f;

        [Header("Ground Slam Shockwave")]
        [SerializeField] private float _groundSlamMinSpeed = 40.0f;
        [SerializeField] private float _groundSlamRadius = 25.0f;
        [SerializeField] private float _groundSlamMaxForce = 1500.0f;
        [SerializeField] private float _groundSlamDamage = 500.0f;

        private Rigidbody _rigidbody;
        private Transform _cameraTransform;
        private float _hoverTimer = 0.0f;
        private Vector3 _currentVelocity = Vector3.zero;

        public bool IsBoosting { get; private set; }
        public bool IsHovering { get; private set; }
        public float CurrentSpeed => _rigidbody != null ? _rigidbody.linearVelocity.magnitude : 0.0f;
        public float TargetMaxSpeed => IsBoosting ? _supersonicSpeed : _baseSpeed;

        public event Action<bool> OnBoostChanged;
        public event Action<Vector3, float> OnGroundSlamImpact; // (position, force)

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.useGravity = false;
            _rigidbody.linearDamping = _drag;
            _rigidbody.angularDamping = 8.0f;
            _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            _rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            if (Camera.main != null)
            {
                _cameraTransform = Camera.main.transform;
            }
        }

        private void Update()
        {
            if (_cameraTransform == null && Camera.main != null)
            {
                _cameraTransform = Camera.main.transform;
            }

            ReadInput();
        }

        private void FixedUpdate()
        {
            SimulateFlightPhysics(Time.fixedDeltaTime);
        }

        private void ReadInput()
        {
            if (Keyboard.current == null) return;

            // Shift for Supersonic Boost
            bool boostPressed = Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed;
            if (boostPressed != IsBoosting)
            {
                IsBoosting = boostPressed;
                OnBoostChanged?.Invoke(IsBoosting);
            }
        }

        public void SimulateFlightPhysics(float deltaTime)
        {
            if (_cameraTransform == null) return;

            Vector3 moveInput = Vector3.zero;
            if (Keyboard.current != null)
            {
                if (Keyboard.current.wKey.isPressed) moveInput += _cameraTransform.forward;
                if (Keyboard.current.sKey.isPressed) moveInput -= _cameraTransform.forward;
                if (Keyboard.current.dKey.isPressed) moveInput += _cameraTransform.right;
                if (Keyboard.current.aKey.isPressed) moveInput -= _cameraTransform.right;
                if (Keyboard.current.spaceKey.isPressed) moveInput += Vector3.up;
            }

            if (moveInput.sqrMagnitude > 0.001f)
            {
                moveInput.Normalize();
                IsHovering = false;

                float maxSpeed = TargetMaxSpeed;
                Vector3 targetVelocity = moveInput * maxSpeed;

                _currentVelocity = Vector3.MoveTowards(_rigidbody.linearVelocity, targetVelocity, _acceleration * deltaTime);
                _rigidbody.linearVelocity = _currentVelocity;

                // Face movement direction
                if (_currentVelocity.sqrMagnitude > 1.0f)
                {
                    Quaternion targetRot = Quaternion.LookRotation(_currentVelocity.normalized, Vector3.up);
                    _rigidbody.rotation = Quaternion.Slerp(_rigidbody.rotation, targetRot, 10.0f * deltaTime);
                }
            }
            else
            {
                // Hover Mode: dampen velocity and apply subtle vertical sine bobbing
                IsHovering = true;
                _hoverTimer += deltaTime * _hoverBobbingFrequency;
                float verticalOffset = Mathf.Sin(_hoverTimer) * _hoverBobbingAmplitude;

                Vector3 dampenedVelocity = Vector3.Lerp(_rigidbody.linearVelocity, Vector3.zero, 6.0f * deltaTime);
                dampenedVelocity.y += verticalOffset * 0.5f;
                _rigidbody.linearVelocity = dampenedVelocity;

                // Stabilize rotation to horizon
                Vector3 forwardFlat = _cameraTransform != null ? Vector3.ProjectOnPlane(_cameraTransform.forward, Vector3.up).normalized : transform.forward;
                if (forwardFlat.sqrMagnitude > 0.01f)
                {
                    Quaternion horizonRot = Quaternion.LookRotation(forwardFlat, Vector3.up);
                    _rigidbody.rotation = Quaternion.Slerp(_rigidbody.rotation, horizonRot, 5.0f * deltaTime);
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            float impactSpeed = collision.relativeVelocity.magnitude;
            Vector3 contactPoint = collision.GetContact(0).point;

            // Supersonic Ground / Structure Slam Trigger
            if (IsBoosting && impactSpeed >= _groundSlamMinSpeed)
            {
                ExecuteGroundSlam(contactPoint, impactSpeed);
            }
        }

        public void ExecuteGroundSlam(Vector3 epicenter, float impactSpeed)
        {
            OnGroundSlamImpact?.Invoke(epicenter, _groundSlamMaxForce);

            Collider[] hitColliders = Physics.OverlapSphere(epicenter, _groundSlamRadius);
            foreach (Collider hit in hitColliders)
            {
                if (hit.transform == transform || hit.transform.IsChildOf(transform)) continue;

                float distance = Vector3.Distance(epicenter, hit.transform.position);
                if (distance > _groundSlamRadius) continue;

                // Quadratic falloff formula: F_radial(r) = F_max * (1 - r / R_max)^2
                float normalizedDist = distance / _groundSlamRadius;
                float falloff = Mathf.Pow(Mathf.Clamp01(1.0f - normalizedDist), 2.0f);
                float finalForce = _groundSlamMaxForce * falloff;
                float finalDamage = _groundSlamDamage * falloff;

                // Deliver explosive damage to destructibles
                IDamageable damageable = hit.GetComponentInParent<IDamageable>();
                if (damageable != null)
                {
                    Vector3 normal = (hit.transform.position - epicenter).normalized;
                    damageable.TakeDamage(finalDamage, DamageType.Explosive, hit.ClosestPoint(epicenter), normal);
                }

                // Apply physical impulse
                Rigidbody rb = hit.attachedRigidbody;
                if (rb != null && !rb.isKinematic)
                {
                    rb.AddExplosionForce(finalForce, epicenter, _groundSlamRadius, 2.0f, ForceMode.Impulse);
                }
            }
        }
    }
}
