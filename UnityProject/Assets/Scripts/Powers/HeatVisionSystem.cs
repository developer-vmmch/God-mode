using UnityEngine;
using UnityEngine.InputSystem;
using CSVS.GodMode.Core;
using CSVS.GodMode.Destruction;
using CSVS.GodMode.Player;

namespace CSVS.GodMode.Powers
{
    /// <summary>
    /// Dual twin heat vision laser beams emitting from eye sockets.
    /// Delivers continuous thermal damage per tick.
    /// Coordinated via AbilityState so it cannot conflict with Throw/Crush or Tether.
    /// </summary>
    public class HeatVisionSystem : MonoBehaviour
    {
        [Header("Beam Origin & Targeting")]
        [SerializeField] private Transform _leftEyeSocket;
        [SerializeField] private Transform _rightEyeSocket;
        [SerializeField] private LineRenderer _leftBeam;
        [SerializeField] private LineRenderer _rightBeam;
        [SerializeField] private float _maxRange = 250.0f;
        [SerializeField] private LayerMask _hitMask = ~0;

        [Header("Beam Thermal Physics")]
        [SerializeField] private float _damagePerSecond = 350.0f;
        [SerializeField] private float _kineticPushPerSecond = 80.0f;

        [Header("VFX")]
        [SerializeField] private ParticleSystem _impactSparks;

        private Camera _mainCamera;
        private SuperheroFlightController _flightController;
        public bool IsFiring { get; private set; }

        private void Start()
        {
            _mainCamera = Camera.main;
            _flightController = GetComponent<SuperheroFlightController>();

            if (GameplayConfig.Instance != null)
            {
                _damagePerSecond = GameplayConfig.Instance.HeatVisionDPS;
                _maxRange = GameplayConfig.Instance.HeatVisionRange;
            }

            SetBeamActive(false);
        }

        private void Update()
        {
            if (_mainCamera == null) _mainCamera = Camera.main;

            CheckInput();

            if (IsFiring)
            {
                ProcessBeams(Time.deltaTime);
            }
        }

        private void CheckInput()
        {
            if (Mouse.current == null) return;

            // Only fire if not holding an object, tethering, or executing another ability
            bool canFire = _flightController == null || _flightController.CanActivateHeatVision();
            bool fireRequested = Mouse.current.leftButton.isPressed && canFire;

            if (fireRequested != IsFiring)
            {
                IsFiring = fireRequested;
                SetBeamActive(IsFiring);

                if (_flightController != null)
                {
                    _flightController.SetAbilityState(IsFiring ? AbilityState.HeatVision : AbilityState.Idle);
                }
            }
        }

        private void ProcessBeams(float deltaTime)
        {
            if (_mainCamera == null) return;

            Ray ray = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
            Vector3 targetPoint;

            if (Physics.Raycast(ray, out RaycastHit hit, _maxRange, _hitMask))
            {
                targetPoint = hit.point;

                // Deliver thermal damage
                IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(_damagePerSecond * deltaTime, DamageType.Thermal, hit.point, hit.normal);
                }

                // Apply slight kinetic push
                if (hit.rigidbody != null && !hit.rigidbody.isKinematic)
                {
                    hit.rigidbody.AddForceAtPosition(ray.direction * (_kineticPushPerSecond * deltaTime), hit.point, ForceMode.Force);
                }

                // Move impact particle effect
                if (_impactSparks != null)
                {
                    _impactSparks.transform.position = hit.point;
                    _impactSparks.transform.rotation = Quaternion.LookRotation(hit.normal);
                    if (!_impactSparks.isPlaying) _impactSparks.Play();
                }

                if (ChaosSystem.Instance != null)
                {
                    ChaosSystem.Instance.AddChaos((long)(15 * deltaTime * 60f), "HeatVision");
                }
            }
            else
            {
                targetPoint = ray.origin + ray.direction * _maxRange;
                if (_impactSparks != null && _impactSparks.isPlaying) _impactSparks.Stop();
            }

            // Update LineRenderer points
            Vector3 leftOrigin = _leftEyeSocket != null ? _leftEyeSocket.position : transform.position + transform.right * -0.2f + transform.up * 0.5f;
            Vector3 rightOrigin = _rightEyeSocket != null ? _rightEyeSocket.position : transform.position + transform.right * 0.2f + transform.up * 0.5f;

            if (_leftBeam != null)
            {
                _leftBeam.SetPosition(0, leftOrigin);
                _leftBeam.SetPosition(1, targetPoint);
            }

            if (_rightBeam != null)
            {
                _rightBeam.SetPosition(0, rightOrigin);
                _rightBeam.SetPosition(1, targetPoint);
            }
        }

        private void SetBeamActive(bool active)
        {
            if (_leftBeam != null) _leftBeam.enabled = active;
            if (_rightBeam != null) _rightBeam.enabled = active;
            if (_impactSparks != null && !active) _impactSparks.Stop();
        }
    }
}
