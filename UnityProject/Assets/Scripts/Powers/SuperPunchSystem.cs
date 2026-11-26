using UnityEngine;
using UnityEngine.InputSystem;
using CSVS.GodMode.Core;
using CSVS.GodMode.Destruction;
using CSVS.GodMode.Player;

namespace CSVS.GodMode.Powers
{
    /// <summary>
    /// Super Punch melee strike (F key).
    /// Delivers localized kinetic punch dealing high structural wall damage and vehicle knockback.
    /// Integrated with AbilityState to prevent conflicting ability activation.
    /// </summary>
    public class SuperPunchSystem : MonoBehaviour
    {
        [Header("Punch Parameters")]
        [SerializeField] private float _punchRange = 3.5f;
        [SerializeField] private float _punchDamage = 450.0f;
        [SerializeField] private float _punchImpulse = 85.0f;
        [SerializeField] private LayerMask _punchLayers = ~0;

        [Header("VFX & Audio Hooks")]
        [SerializeField] private ParticleSystem _punchImpactFX;
        [SerializeField] private AudioClip _punchAudio;

        private Camera _mainCamera;
        private SuperheroFlightController _flightController;

        private void Start()
        {
            _mainCamera = Camera.main;
            _flightController = GetComponent<SuperheroFlightController>();

            if (GameplayConfig.Instance != null)
            {
                _punchDamage = GameplayConfig.Instance.SuperPunchDamage;
                _punchImpulse = GameplayConfig.Instance.SuperPunchImpulse;
            }
        }

        private void Update()
        {
            if (_mainCamera == null) _mainCamera = Camera.main;

            bool punchPressed = false;
            if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
            {
                punchPressed = true;
            }

            if (punchPressed)
            {
                // Can only punch if not holding an object or firing heat vision
                if (_flightController != null && !_flightController.CanActivateOffensiveAbility())
                {
                    return;
                }

                ExecutePunch();
            }
        }

        public void ExecutePunch()
        {
            if (_mainCamera == null) return;

            if (_flightController != null)
            {
                _flightController.SetAbilityState(AbilityState.SuperPunch);
            }

            Ray ray = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));

            if (Physics.Raycast(ray, out RaycastHit hit, _punchRange, _punchLayers))
            {
                // Structural damage via unified IDamageable
                IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(_punchDamage, DamageType.Kinetic, hit.point, hit.normal);
                }

                // Push rigidbodies
                Rigidbody rb = hit.collider.attachedRigidbody;
                if (rb != null && !rb.isKinematic)
                {
                    rb.AddForceAtPosition(ray.direction * _punchImpulse, hit.point, ForceMode.Impulse);
                }

                // Camera trauma
                SuperheroCameraController cam = FindFirstObjectByType<SuperheroCameraController>();
                if (cam != null)
                {
                    cam.TriggerShake(0.6f);
                }

                // Visual FX
                if (_punchImpactFX != null)
                {
                    Instantiate(_punchImpactFX, hit.point, Quaternion.LookRotation(hit.normal));
                }

                if (_punchAudio != null)
                {
                    AudioSource.PlayClipAtPoint(_punchAudio, hit.point, 1.0f);
                }

                if (ChaosSystem.Instance != null)
                {
                    ChaosSystem.Instance.AddChaos(200, "SuperPunch");
                }
            }

            // Return to Idle ability state
            if (_flightController != null && _flightController.CurrentAbilityState == AbilityState.SuperPunch)
            {
                _flightController.SetAbilityState(AbilityState.Idle);
            }
        }
    }
}
