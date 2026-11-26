using System;
using UnityEngine;
using UnityEngine.InputSystem;
using CSVS.GodMode.Core;
using CSVS.GodMode.Destruction;
using CSVS.GodMode.Powers;

namespace CSVS.GodMode.Player
{
    /// <summary>
    /// Consolidated 6DOF Superhero Locomotion Controller.
    /// Supports Ground locomotion (walk, run, jump), omnidirectional Hover bobbing,
    /// high-speed Supersonic Flight (35 m/s normal, 95 m/s boost),
    /// and kinetic Ground Slam impact detection with quadratic shockwave falloff.
    /// Fully manages AbilityState to eliminate input conflicts.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class SuperheroFlightController : MonoBehaviour
    {
        [Header("Locomotion State")]
        [SerializeField] private LocomotionMode _currentMode = LocomotionMode.Flight;
        [SerializeField] private AbilityState _abilityState = AbilityState.Idle;

        [Header("Flight Kinematics")]
        [SerializeField] private float _baseFlightSpeed = 35.0f;       // 35 m/s
        [SerializeField] private float _boostFlightSpeed = 95.0f;      // 95 m/s (Standardized)
        [SerializeField] private float _flightAcceleration = 45.0f;
        [SerializeField] private float _flightDrag = 4.0f;
        [SerializeField] private float _hoverVerticalSpeed = 15.0f;
        [SerializeField] private float _hoverBobbingFrequency = 1.5f;
        [SerializeField] private float _hoverBobbingAmplitude = 0.25f;

        [Header("Ground Locomotion")]
        [SerializeField] private float _walkSpeed = 8.0f;
        [SerializeField] private float _runSpeed = 16.0f;
        [SerializeField] private float _jumpForce = 8.0f;
        [SerializeField] private float _groundGravity = 25.0f;
        [SerializeField] private LayerMask _groundLayers = ~0;

        [Header("Ground Slam Shockwave")]
        [SerializeField] private float _groundSlamMinSpeed = 40.0f;
        [SerializeField] private GroundSlamSystem _groundSlamSystem;

        private Rigidbody _rigidbody;
        private Transform _cameraTransform;
        private float _hoverTimer = 0.0f;
        private bool _isGrounded = false;

        public LocomotionMode CurrentLocomotionMode => _currentMode;
        public AbilityState CurrentAbilityState => _abilityState;
        public bool IsBoosting { get; private set; }
        public bool IsHovering => _currentMode == LocomotionMode.Hover;
        public float CurrentSpeed => _rigidbody != null ? _rigidbody.linearVelocity.magnitude : 0.0f;
        public float TargetMaxSpeed => IsBoosting ? _boostFlightSpeed : _baseFlightSpeed;

        public event Action<bool> OnBoostChanged;
        public event Action<LocomotionMode> OnLocomotionModeChanged;
        public event Action<AbilityState> OnAbilityStateChanged;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.useGravity = false;
            _rigidbody.linearDamping = _flightDrag;
            _rigidbody.angularDamping = 8.0f;
            _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            _rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            if (_groundSlamSystem == null)
            {
                _groundSlamSystem = GetComponent<GroundSlamSystem>();
                if (_groundSlamSystem == null) _groundSlamSystem = gameObject.AddComponent<GroundSlamSystem>();
            }

            if (Camera.main != null)
            {
                _cameraTransform = Camera.main.transform;
            }
        }

        private void Start()
        {
            if (GameplayConfig.Instance != null)
            {
                _baseFlightSpeed = GameplayConfig.Instance.NormalFlightSpeed;
                _boostFlightSpeed = GameplayConfig.Instance.BoostFlightSpeed;
                _flightAcceleration = GameplayConfig.Instance.FlightAcceleration;
                _walkSpeed = GameplayConfig.Instance.WalkSpeed;
                _runSpeed = GameplayConfig.Instance.RunSpeed;
            }
        }

        private void Update()
        {
            if (_cameraTransform == null && Camera.main != null)
            {
                _cameraTransform = Camera.main.transform;
            }

            ReadInput();
            CheckGroundStatus();
        }

        private void FixedUpdate()
        {
            switch (_currentMode)
            {
                case LocomotionMode.Flight:
                case LocomotionMode.Hover:
                    SimulateFlightPhysics(Time.fixedDeltaTime);
                    break;
                case LocomotionMode.Ground:
                    SimulateGroundPhysics(Time.fixedDeltaTime);
                    break;
            }
        }

        private void ReadInput()
        {
            // Shift for Supersonic Boost / Ground Sprint
            bool boostPressed = GameInputManager.IsSprintOrBoostPressed;
            if (boostPressed != IsBoosting)
            {
                IsBoosting = boostPressed;
                OnBoostChanged?.Invoke(IsBoosting);
            }

            // Space: in ground mode, jumps or transitions to flight
            if (GameInputManager.WasAscendTriggered)
            {
                if (_currentMode == LocomotionMode.Ground)
                {
                    _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
                    SetLocomotionMode(LocomotionMode.Flight);
                }
            }
        }

        private void CheckGroundStatus()
        {
            _isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, 0.4f, _groundLayers);

            // Auto-land to ground mode if touching ground with low vertical velocity
            if (_isGrounded && _currentMode != LocomotionMode.Ground && _rigidbody.linearVelocity.y <= 0.5f)
            {
                if (!GameInputManager.IsAscendPressed)
                {
                    SetLocomotionMode(LocomotionMode.Ground);
                }
            }
        }

        public void SetLocomotionMode(LocomotionMode newMode)
        {
            if (_currentMode == newMode) return;
            _currentMode = newMode;
            OnLocomotionModeChanged?.Invoke(_currentMode);
        }

        public void SetAbilityState(AbilityState newState)
        {
            _abilityState = newState;
            OnAbilityStateChanged?.Invoke(_abilityState);
        }

        public bool CanActivateOffensiveAbility()
        {
            return _abilityState == AbilityState.Idle;
        }

        public bool CanActivateHeatVision()
        {
            // Heat vision can fire during Idle or HeatVision, but NOT while holding an object or crushing
            return _abilityState == AbilityState.Idle || _abilityState == AbilityState.HeatVision;
        }

        public void SimulateFlightPhysics(float deltaTime)
        {
            if (_rigidbody == null) _rigidbody = GetComponent<Rigidbody>();
            if (_rigidbody == null) return;

            Vector3 camForward = _cameraTransform != null ? _cameraTransform.forward : transform.forward;
            Vector3 camRight = _cameraTransform != null ? _cameraTransform.right : transform.right;

            Vector2 rawMove = GameInputManager.MoveInput;
            Vector3 moveInput = camForward * rawMove.y + camRight * rawMove.x;
            if (GameInputManager.IsAscendPressed) moveInput += Vector3.up;
            if (GameInputManager.IsDescendPressed) moveInput += Vector3.down;

            if (moveInput.sqrMagnitude > 0.001f)
            {
                moveInput.Normalize();
                SetLocomotionMode(LocomotionMode.Flight);

                float targetSpeed = TargetMaxSpeed;
                Vector3 targetVelocity = moveInput * targetSpeed;

                _rigidbody.linearVelocity = Vector3.MoveTowards(_rigidbody.linearVelocity, targetVelocity, _flightAcceleration * deltaTime);

                // Align hero rotation with movement direction
                if (_rigidbody.linearVelocity.sqrMagnitude > 1.0f)
                {
                    Quaternion targetRot = Quaternion.LookRotation(_rigidbody.linearVelocity.normalized, Vector3.up);
                    _rigidbody.rotation = Quaternion.Slerp(_rigidbody.rotation, targetRot, 10.0f * deltaTime);
                }
            }
            else
            {
                // Hover Mode: dampen velocity and apply subtle vertical sine bobbing
                SetLocomotionMode(LocomotionMode.Hover);
                _hoverTimer += deltaTime * _hoverBobbingFrequency;
                float verticalOffset = Mathf.Sin(_hoverTimer) * _hoverBobbingAmplitude;

                Vector3 dampenedVelocity = Vector3.Lerp(_rigidbody.linearVelocity, Vector3.zero, 6.0f * deltaTime);
                dampenedVelocity.y += verticalOffset * 0.5f;
                _rigidbody.linearVelocity = dampenedVelocity;

                // Stabilize rotation to horizon
                Vector3 forwardFlat = Vector3.ProjectOnPlane(camForward, Vector3.up).normalized;
                if (forwardFlat.sqrMagnitude > 0.01f)
                {
                    Quaternion horizonRot = Quaternion.LookRotation(forwardFlat, Vector3.up);
                    _rigidbody.rotation = Quaternion.Slerp(_rigidbody.rotation, horizonRot, 5.0f * deltaTime);
                }
            }
        }

        public void SimulateGroundPhysics(float deltaTime)
        {
            if (_rigidbody == null) _rigidbody = GetComponent<Rigidbody>();
            if (_rigidbody == null) return;

            Vector3 camForward = _cameraTransform != null ? _cameraTransform.forward : transform.forward;
            Vector3 camRight = _cameraTransform != null ? _cameraTransform.right : transform.right;

            Vector3 flatForward = Vector3.ProjectOnPlane(camForward, Vector3.up).normalized;
            Vector3 flatRight = Vector3.ProjectOnPlane(camRight, Vector3.up).normalized;
            Vector2 rawMove = GameInputManager.MoveInput;
            Vector3 moveInput = flatForward * rawMove.y + flatRight * rawMove.x;

            float speed = IsBoosting ? _runSpeed : _walkSpeed;

            if (moveInput.sqrMagnitude > 0.001f)
            {
                moveInput.Normalize();
                Vector3 targetVel = moveInput * speed;
                targetVel.y = _rigidbody.linearVelocity.y - (_groundGravity * deltaTime);
                _rigidbody.linearVelocity = Vector3.MoveTowards(_rigidbody.linearVelocity, targetVel, 40.0f * deltaTime);

                Quaternion targetRot = Quaternion.LookRotation(moveInput, Vector3.up);
                _rigidbody.rotation = Quaternion.Slerp(_rigidbody.rotation, targetRot, 12.0f * deltaTime);
            }
            else
            {
                Vector3 vel = _rigidbody.linearVelocity;
                vel.x = Mathf.MoveTowards(vel.x, 0.0f, 30.0f * deltaTime);
                vel.z = Mathf.MoveTowards(vel.z, 0.0f, 30.0f * deltaTime);
                vel.y -= _groundGravity * deltaTime;
                _rigidbody.linearVelocity = vel;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            float impactSpeed = collision.relativeVelocity.magnitude;
            Vector3 contactPoint = collision.GetContact(0).point;
            Vector3 contactNormal = collision.GetContact(0).normal;

            // Supersonic Ground / Structure Slam Trigger
            if (IsBoosting && impactSpeed >= _groundSlamMinSpeed)
            {
                if (_groundSlamSystem != null)
                {
                    _groundSlamSystem.ExecuteGroundSlam(contactPoint, contactNormal, impactSpeed);
                }
            }
        }
    }
}
