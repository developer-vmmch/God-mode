// HumanoidCharacterController.cs
// CSVS GOD MODE — V0.3 Human/Superhero Dual-Mode Player Controller
// ORIGINAL CODE — CSVS IT Tech. No third-party character copyrights used.
//
// Architecture:
//   - Single MonoBehaviour drives BOTH human and superhero modes.
//   - CharacterController for ground movement in human mode.
//   - Rigidbody-free flight handled by SuperheroFlightController in superhero mode.
//   - SkinnedMeshRenderer humanoid with Animator state machine for smooth blends.
//   - Transformation sequence: T key (default) → energy burst VFX → 1.2 s cinematic.

using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using CSVS.GodMode.Core;

namespace CSVS.GodMode.Player
{
    public enum PlayerMode { Human, Superhero }

    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Animator))]
    public class HumanoidCharacterController : MonoBehaviour
    {
        // ─── Inspector ────────────────────────────────────────────────────
        [Header("Movement — Human Mode")]
        [SerializeField] private float walkSpeed    = 2.8f;    // m/s
        [SerializeField] private float runSpeed     = 6.5f;    // m/s
        [SerializeField] private float sprintSpeed  = 10.0f;   // m/s
        [SerializeField] private float jumpHeight   = 1.4f;    // metres
        [SerializeField] private float gravity      = -18f;    // m/s²
        [SerializeField] private float groundDrag   = 0.92f;
        [SerializeField] private float turnSpeed    = 540f;    // deg/s

        [Header("Animator")]
        [SerializeField] private Animator animator;
        [SerializeField] private string paramSpeed  = "Speed";
        [SerializeField] private string paramHoriz  = "Horizontal";
        [SerializeField] private string paramJump   = "Jump";
        [SerializeField] private string paramGrounded = "IsGrounded";
        [SerializeField] private string paramTransform  = "Transform";
        [SerializeField] private string paramSuperMode  = "SuperheroMode";

        [Header("Visual Models")]
        [SerializeField] private GameObject humanBodyRoot;
        [SerializeField] private GameObject superheroBodyRoot;

        [Header("Transform VFX")]
        [SerializeField] private ParticleSystem transformParticles;
        [SerializeField] private Light transformEnergyLight;
        [SerializeField] private float transformDuration = 1.2f;   // seconds

        [Header("Camera Reference")]
        [SerializeField] private SuperheroCameraController cameraController;

        [Header("Linked Systems (superhero mode)")]
        [SerializeField] private SuperheroFlightController flightController;
        [SerializeField] private MonoBehaviour[] superheroOnlySystems;   // Disabled in human mode

        // ─── State ─────────────────────────────────────────────────────────
        public PlayerMode CurrentMode { get; private set; } = PlayerMode.Human;
        public bool IsGrounded { get; private set; }
        public float CurrentSpeed { get; private set; }
        public int Health { get; private set; } = 100;
        public int MaxHealth { get; private set; } = 100;
        public float Energy { get; private set; } = 100f;
        public float MaxEnergy { get; private set; } = 100f;

        private CharacterController _cc;
        private Vector3 _velocity;
        private bool _isTransforming;
        private Vector3 _worldMoveDir;

        // Animation hash cache
        private int _hashSpeed, _hashHoriz, _hashJump, _hashGrounded, _hashTransform, _hashSuper;

        // ─── Lifecycle ─────────────────────────────────────────────────────
        private void Awake()
        {
            _cc = GetComponent<CharacterController>();
            if (!animator) animator = GetComponent<Animator>();

            _hashSpeed     = Animator.StringToHash(paramSpeed);
            _hashHoriz     = Animator.StringToHash(paramHoriz);
            _hashJump      = Animator.StringToHash(paramJump);
            _hashGrounded  = Animator.StringToHash(paramGrounded);
            _hashTransform = Animator.StringToHash(paramTransform);
            _hashSuper     = Animator.StringToHash(paramSuperMode);

            ApplyMode(PlayerMode.Human, instant: true);
        }

        private void Update()
        {
            if (_isTransforming) return;

            // Transformation trigger
            if (GameInputManager.WasTransformTriggered)
            {
                StartCoroutine(PerformTransformation());
                return;
            }

            if (CurrentMode == PlayerMode.Human)
                HandleHumanMovement();
            // Superhero movement is managed by SuperheroFlightController
        }

        // ─── Human Mode Movement ───────────────────────────────────────────
        private void HandleHumanMovement()
        {
            IsGrounded = _cc.isGrounded;
            if (IsGrounded && _velocity.y < 0f)
                _velocity.y = -2f;

            var input = GameInputManager.MoveInput;
            var cam   = cameraController ? cameraController.transform : Camera.main?.transform;

            Vector3 camFwd = cam ? new Vector3(cam.forward.x, 0, cam.forward.z).normalized : Vector3.forward;
            Vector3 camRight = cam ? new Vector3(cam.right.x, 0, cam.right.z).normalized : Vector3.right;

            _worldMoveDir = (camFwd * input.y + camRight * input.x);
            if (_worldMoveDir.sqrMagnitude > 1f) _worldMoveDir.Normalize();

            bool isSprinting = GameInputManager.IsSprintHeld() && _worldMoveDir.sqrMagnitude > 0.1f;
            bool isRunning   = _worldMoveDir.sqrMagnitude > 0.1f;

            float targetSpeed = isRunning ? (isSprinting ? sprintSpeed : runSpeed) : 0f;
            CurrentSpeed = Mathf.Lerp(CurrentSpeed, targetSpeed, Time.deltaTime * 8f);

            // Rotate character toward movement
            if (_worldMoveDir.sqrMagnitude > 0.01f)
            {
                Quaternion targetRot = Quaternion.LookRotation(_worldMoveDir);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
            }

            // Apply horizontal movement
            Vector3 move = _worldMoveDir * CurrentSpeed;
            _cc.Move((move + Vector3.up * _velocity.y) * Time.deltaTime);

            // Jump
            if (GameInputManager.WasJumpTriggered() && IsGrounded)
            {
                _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                animator.SetTrigger(_hashJump);
            }

            // Gravity
            _velocity.y += gravity * Time.deltaTime;
            if (IsGrounded) _velocity.y *= groundDrag;

            // Animator
            float normSpeed = Mathf.InverseLerp(0f, sprintSpeed, CurrentSpeed);
            animator.SetFloat(_hashSpeed, normSpeed, 0.1f, Time.deltaTime);
            animator.SetFloat(_hashHoriz, input.x, 0.1f, Time.deltaTime);
            animator.SetBool(_hashGrounded, IsGrounded);
        }

        // ─── Transformation ────────────────────────────────────────────────
        private IEnumerator PerformTransformation()
        {
            if (_isTransforming) yield break;
            _isTransforming = true;

            // Trigger animation
            animator.SetTrigger(_hashTransform);

            // VFX burst
            if (transformParticles) transformParticles.Play();
            if (transformEnergyLight)
            {
                transformEnergyLight.enabled = true;
                transformEnergyLight.color = new Color(0f, 0.94f, 1f);  // Cyan
                transformEnergyLight.intensity = 0f;
            }

            // Animate light intensity up → peak → down
            float elapsed = 0f;
            while (elapsed < transformDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / transformDuration;
                float intensity = Mathf.Sin(t * Mathf.PI) * 8f;
                if (transformEnergyLight) transformEnergyLight.intensity = intensity;
                yield return null;
            }

            // Swap mode
            PlayerMode newMode = CurrentMode == PlayerMode.Human
                ? PlayerMode.Superhero : PlayerMode.Human;
            ApplyMode(newMode, instant: false);

            if (transformParticles) transformParticles.Stop();
            if (transformEnergyLight) transformEnergyLight.enabled = false;
            _isTransforming = false;
        }

        // ─── Mode Switching ────────────────────────────────────────────────
        private void ApplyMode(PlayerMode mode, bool instant)
        {
            CurrentMode = mode;
            bool isSuper = mode == PlayerMode.Superhero;

            // Swap visible body
            if (humanBodyRoot)    humanBodyRoot.SetActive(!isSuper);
            if (superheroBodyRoot) superheroBodyRoot.SetActive(isSuper);

            // Animator flag
            if (animator) animator.SetBool(_hashSuper, isSuper);

            // Enable/disable superhero-only systems
            if (flightController) flightController.enabled = isSuper;
            foreach (var sys in superheroOnlySystems)
                if (sys) sys.enabled = isSuper;

            // CharacterController active in human mode; flight controller takes over in super mode
            _cc.enabled = !isSuper;

            if (cameraController)
                cameraController.SetPlayerMode(mode);

            Debug.Log($"[PlayerMode] → {mode}{(instant ? " (instant)" : " (animated)")}");
        }

        // ─── Health / Energy ───────────────────────────────────────────────
        public void TakeDamage(int amount)
        {
            Health = Mathf.Max(0, Health - amount);
        }

        public void RestoreHealth(int amount)
        {
            Health = Mathf.Min(MaxHealth, Health + amount);
        }

        public void DrainEnergy(float amount)
        {
            Energy = Mathf.Max(0f, Energy - amount);
        }

        public void RechargeEnergy(float rate)
        {
            Energy = Mathf.Min(MaxEnergy, Energy + rate * Time.deltaTime);
        }

        public bool HasEnergy(float required) => Energy >= required;

        // ─── Vehicle handoff ───────────────────────────────────────────────
        public void OnEnterVehicle()
        {
            _cc.enabled = false;
            if (humanBodyRoot) humanBodyRoot.SetActive(false);
            if (superheroBodyRoot) superheroBodyRoot.SetActive(false);
        }

        public void OnExitVehicle(Vector3 exitPosition)
        {
            transform.position = exitPosition;
            _cc.enabled = (CurrentMode == PlayerMode.Human);
            if (humanBodyRoot) humanBodyRoot.SetActive(CurrentMode == PlayerMode.Human);
            if (superheroBodyRoot) superheroBodyRoot.SetActive(CurrentMode == PlayerMode.Superhero);
        }
    }
}

