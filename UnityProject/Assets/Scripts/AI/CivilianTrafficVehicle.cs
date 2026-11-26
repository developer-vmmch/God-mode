using UnityEngine;
using CSVS.GodMode.Core;
using CSVS.GodMode.Destruction;

namespace CSVS.GodMode.AI
{
    [RequireComponent(typeof(Rigidbody))]
    public class CivilianTrafficVehicle : MonoBehaviour, IDamageable
    {
        [Header("Traffic Navigation")]
        [SerializeField] private float _cruiseSpeed = 12.0f;
        [SerializeField] private Vector3[] _waypoints;
        [SerializeField] private float _waypointReachThreshold = 2.0f;

        [Header("Vehicle Health & Chaos")]
        [SerializeField] private float _maxHealth = 150.0f;
        [SerializeField] private long _chaosValue = 3500;
        [SerializeField] private float _explosionRadius = 8.0f;
        [SerializeField] private float _explosionForce = 800.0f;

        private Rigidbody _rigidbody;
        private int _currentWaypointIndex = 0;
        private float _health;
        private bool _isWrecked = false;

        public float CurrentHealth => _health;
        public float MaxHealth => _maxHealth;
        public bool IsDestroyed => _isWrecked;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _health = _maxHealth;
        }

        private void Update()
        {
            if (_isWrecked) return;
            DriveAlongWaypoints(Time.deltaTime);
        }

        public void SetWaypoints(Vector3[] waypoints)
        {
            _waypoints = waypoints;
            _currentWaypointIndex = 0;
        }

        private void DriveAlongWaypoints(float deltaTime)
        {
            if (_waypoints == null || _waypoints.Length == 0) return;

            Vector3 target = _waypoints[_currentWaypointIndex];
            target.y = transform.position.y;

            Vector3 dir = (target - transform.position);
            if (dir.magnitude <= _waypointReachThreshold)
            {
                _currentWaypointIndex = (_currentWaypointIndex + 1) % _waypoints.Length;
            }
            else
            {
                Quaternion targetRot = Quaternion.LookRotation(dir.normalized, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 5.0f * deltaTime);
                transform.position += transform.forward * (_cruiseSpeed * deltaTime);
            }
        }

        public void TakeDamage(float amount, DamageType damageType, Vector3 hitPoint, Vector3 hitNormal)
        {
            if (_isWrecked) return;

            _health -= amount;

            // Trigger physical ragdoll tumbling on hit
            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = true;
            _rigidbody.AddForceAtPosition(-hitNormal * (amount * 5.0f), hitPoint, ForceMode.Impulse);

            if (_health <= 0.0f)
            {
                WreckVehicle();
            }
        }

        private void WreckVehicle()
        {
            _isWrecked = true;

            // Explosive shockwave
            Collider[] colliders = Physics.OverlapSphere(transform.position, _explosionRadius);
            foreach (Collider hit in colliders)
            {
                if (hit.gameObject == gameObject) continue;

                Rigidbody rb = hit.attachedRigidbody;
                if (rb != null && !rb.isKinematic)
                {
                    rb.AddExplosionForce(_explosionForce, transform.position, _explosionRadius, 1.0f, ForceMode.Impulse);
                }

                IDamageable damageable = hit.GetComponentInParent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(120.0f, DamageType.Explosive, hit.ClosestPoint(transform.position), Vector3.up);
                }
            }

            if (ChaosSystem.Instance != null)
            {
                ChaosSystem.Instance.AddChaos(_chaosValue, "Vehicle");
            }
        }
    }
}
