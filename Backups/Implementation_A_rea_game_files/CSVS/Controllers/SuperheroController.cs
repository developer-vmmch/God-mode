using UnityEngine;
using CSVS.Input;
using CSVS.Camera;
using CSVS.Superpowers;

namespace CSVS.Controllers
{
    public enum LocomotionMode
    {
        Ground,
        Hover,
        Flight
    }

    /// <summary>
    /// Advanced 6DOF Superhero Locomotion Controller.
    /// Handles ground locomotion, omnidirectional hover, high-speed supersonic flight,
    /// and kinetic Ground Slam impact detection with anti-clipping collision sweeps.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class SuperheroController : MonoBehaviour
    {
        [Header("Locomotion State")]
        [SerializeField] private LocomotionMode currentMode = LocomotionMode.Flight;

        [Header("Flight Parameters")]
        [SerializeField] private float normalFlightSpeed = 35f;
        [SerializeField] private float supersonicFlightSpeed = 95f;
        [SerializeField] private float flightAcceleration = 45f;
        [SerializeField] private float flightDeceleration = 30f;
        [SerializeField] private float hoverVerticalSpeed = 15f;
        [SerializeField] private float hoverBobFrequency = 1.5f;
        [SerializeField] private float hoverBobAmplitude = 0.2f;

        [Header("Ground Locomotion Parameters")]
        [SerializeField] private float walkSpeed = 8f;
        [SerializeField] private float runSpeed = 16f;
        [SerializeField] private float jumpHeight = 3.5f;
        [SerializeField] private float gravity = 25f;

        [Header("Ground Slam Mechanics")]
        [SerializeField] private float minSlamVelocity = 40f;
        [SerializeField] private GroundSlamSystem groundSlamSystem;

        [Header("VFX & Audio Hooks")]
        [SerializeField] private TrailRenderer[] flightTrails;
        [SerializeField] private ParticleSystem supersonicShockwaveFX;
        [SerializeField] private AudioSource flightAudioSource;

        private CharacterController characterController;
        private Vector3 currentVelocity;
        private float hoverTimer = 0f;
        private bool isGrounded = false;
        private float verticalVelocity = 0f;

        public LocomotionMode CurrentMode => currentMode;
        public Vector3 Velocity => currentVelocity;
        public float Speed => currentVelocity.magnitude;
        public bool IsSupersonic => currentVelocity.magnitude > (normalFlightSpeed * 1.5f);

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            if (groundSlamSystem == null)
            {
                groundSlamSystem = GetComponent<GroundSlamSystem>();
            }
        }

        private void Start()
        {
            if (SuperheroCameraController.Instance != null)
            {
                SuperheroCameraController.Instance.SetTarget(transform);
            }
        }

        private void Update()
        {
            isGrounded = characterController.isGrounded;

            // Toggle mode check (Double space or auto flight transition)
            if (currentMode == LocomotionMode.Ground && !isGrounded && SuperheroInput.Instance != null && SuperheroInput.Instance.VerticalFlight > 0)
            {
                SetLocomotionMode(LocomotionMode.Flight);
            }

            switch (currentMode)
            {
                case LocomotionMode.Flight:
                case LocomotionMode.Hover:
                    HandleFlightLocomotion();
                    break;
                case LocomotionMode.Ground:
                    HandleGroundLocomotion();
                    break;
            }

            UpdateFlightVFX();
        }

        public void SetLocomotionMode(LocomotionMode newMode)
        {
            currentMode = newMode;
            if (currentMode == LocomotionMode.Ground)
            {
                verticalVelocity = 0f;
            }
        }

        private void HandleFlightLocomotion()
        {
            if (SuperheroInput.Instance == null) return;

            Vector2 moveInput = SuperheroInput.Instance.MoveVector;
            float verticalInput = SuperheroInput.Instance.VerticalFlight;
            bool isBoosting = SuperheroInput.Instance.IsBoosting;

            UnityEngine.Camera cam = UnityEngine.Camera.main;
            Vector3 camForward = cam != null ? cam.transform.forward : transform.forward;
            Vector3 camRight = cam != null ? cam.transform.right : transform.right;

            // 6DOF desired flight vector
            Vector3 desiredDirection = (camForward * moveInput.y + camRight * moveInput.x).normalized;

            // Add vertical lift / drop
            desiredDirection += Vector3.up * verticalInput;
            if (desiredDirection.sqrMagnitude > 1f) desiredDirection.Normalize();

            float targetSpeed = isBoosting ? supersonicFlightSpeed : normalFlightSpeed;

            if (desiredDirection.sqrMagnitude > 0.01f)
            {
                currentMode = LocomotionMode.Flight;
                // Accelerate toward target flight vector
                Vector3 targetVel = desiredDirection * targetSpeed;
                currentVelocity = Vector3.MoveTowards(currentVelocity, targetVel, flightAcceleration * Time.deltaTime);

                // Align superhero character facing with look / flight trajectory
                if (desiredDirection.sqrMagnitude > 0.1f)
                {
                    Quaternion targetRot = Quaternion.LookRotation(desiredDirection, Vector3.up);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 12f);
                }
            }
            else
            {
                // Decelerate toward hover mode
                currentVelocity = Vector3.MoveTowards(currentVelocity, Vector3.zero, flightDeceleration * Time.deltaTime);
                if (currentVelocity.sqrMagnitude < 0.5f)
                {
                    currentMode = LocomotionMode.Hover;
                    hoverTimer += Time.deltaTime * hoverBobFrequency;
                    float bobOffset = Mathf.Sin(hoverTimer) * hoverBobAmplitude * Time.deltaTime;
                    currentVelocity = new Vector3(0f, bobOffset, 0f);
                }
            }

            // High-speed collision sweep test
            PerformCollisionSweepAndMove();
        }

        private void PerformCollisionSweepAndMove()
        {
            Vector3 motion = currentVelocity * Time.deltaTime;
            float speed = currentVelocity.magnitude;

            // Check for high-velocity ground slam collision
            if (speed >= minSlamVelocity && motion.y < -0.1f)
            {
                RaycastHit hit;
                if (Physics.SphereCast(transform.position, characterController.radius * 0.9f, currentVelocity.normalized, out hit, motion.magnitude + 0.5f))
                {
                    // Trigger Ground Slam!
                    if (groundSlamSystem != null)
                    {
                        groundSlamSystem.ExecuteGroundSlam(hit.point, hit.normal, speed);
                    }
                    currentVelocity = Vector3.zero;
                    SetLocomotionMode(LocomotionMode.Ground);
                    return;
                }
            }

            CollisionFlags flags = characterController.Move(motion);

            // If we collide with the ground while flying downward at low speed, revert to ground mode
            if ((flags & CollisionFlags.Below) != 0 && currentVelocity.y < 0)
            {
                SetLocomotionMode(LocomotionMode.Ground);
                currentVelocity = Vector3.zero;
            }
        }

        private void HandleGroundLocomotion()
        {
            if (SuperheroInput.Instance == null) return;

            Vector2 moveInput = SuperheroInput.Instance.MoveVector;
            bool isSprinting = SuperheroInput.Instance.IsBoosting;

            UnityEngine.Camera cam = UnityEngine.Camera.main;
            Vector3 camForward = cam != null ? Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up).normalized : transform.forward;
            Vector3 camRight = cam != null ? Vector3.ProjectOnPlane(cam.transform.right, Vector3.up).normalized : transform.right;

            Vector3 moveDir = (camForward * moveInput.y + camRight * moveInput.x).normalized;
            float currentSpeed = isSprinting ? runSpeed : walkSpeed;

            if (moveDir.sqrMagnitude > 0.01f)
            {
                Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 14f);
            }

            if (isGrounded)
            {
                verticalVelocity = -2f; // Slight downward grounding pressure
                if (SuperheroInput.Instance.VerticalFlight > 0)
                {
                    verticalVelocity = Mathf.Sqrt(jumpHeight * 2f * gravity);
                }
            }
            else
            {
                verticalVelocity -= gravity * Time.deltaTime;
            }

            Vector3 groundVelocity = moveDir * currentSpeed + Vector3.up * verticalVelocity;
            characterController.Move(groundVelocity * Time.deltaTime);
            currentVelocity = groundVelocity;
        }

        private void UpdateFlightVFX()
        {
            bool activeTrails = (currentMode == LocomotionMode.Flight && currentVelocity.magnitude > 20f);
            if (flightTrails != null)
            {
                foreach (var trail in flightTrails)
                {
                    if (trail != null) trail.emitting = activeTrails;
                }
            }

            if (supersonicShockwaveFX != null)
            {
                if (IsSupersonic && !supersonicShockwaveFX.isPlaying)
                {
                    supersonicShockwaveFX.Play();
                }
                else if (!IsSupersonic && supersonicShockwaveFX.isPlaying)
                {
                    supersonicShockwaveFX.Stop();
                }
            }
        }
    }
}
