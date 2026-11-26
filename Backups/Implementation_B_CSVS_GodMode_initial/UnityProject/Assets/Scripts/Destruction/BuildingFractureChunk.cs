using UnityEngine;

namespace CSVS.GodMode.Destruction
{
    [RequireComponent(typeof(Rigidbody))]
    public class BuildingFractureChunk : MonoBehaviour, IDamageable
    {
        [Header("Sleep & Cleanup")]
        [SerializeField] private float _sleepVelocityThreshold = 0.15f;
        [SerializeField] private float _sleepDurationRequired = 1.0f;
        [SerializeField] private float _autoDespawnTime = 15.0f;

        private Rigidbody _rigidbody;
        private Collider _collider;
        private float _lowSpeedTimer = 0.0f;
        private float _age = 0.0f;
        private bool _isFrozen = false;

        public float CurrentHealth { get; private set; } = 100.0f;
        public float MaxHealth { get; private set; } = 100.0f;
        public bool IsDestroyed => _isFrozen || CurrentHealth <= 0;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
        }

        private void Start()
        {
            if (DebrisPoolManager.Instance != null)
            {
                DebrisPoolManager.Instance.RegisterActiveChunk(this);
            }
        }

        private void Update()
        {
            if (_isFrozen) return;

            _age += Time.deltaTime;
            if (_age >= _autoDespawnTime)
            {
                FreezeToStatic();
                return;
            }

            // Early Sleep State Detection
            if (_rigidbody != null && !_rigidbody.isKinematic)
            {
                if (_rigidbody.linearVelocity.sqrMagnitude < _sleepVelocityThreshold * _sleepVelocityThreshold)
                {
                    _lowSpeedTimer += Time.deltaTime;
                    if (_lowSpeedTimer >= _sleepDurationRequired)
                    {
                        FreezeToStatic();
                    }
                }
                else
                {
                    _lowSpeedTimer = 0.0f;
                }
            }
        }

        public void FreezeToStatic()
        {
            _isFrozen = true;
            if (_rigidbody != null)
            {
                _rigidbody.linearVelocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
                _rigidbody.isKinematic = true;
            }

            if (DebrisPoolManager.Instance != null)
            {
                DebrisPoolManager.Instance.UnregisterChunk(this);
            }
        }

        public void TakeDamage(float amount, DamageType damageType, Vector3 hitPoint, Vector3 hitNormal)
        {
            CurrentHealth -= amount;
            if (_rigidbody != null && !_rigidbody.isKinematic)
            {
                _rigidbody.AddForceAtPosition(-hitNormal * (amount * 2.0f), hitPoint, ForceMode.Impulse);
            }

            if (CurrentHealth <= 0 && !_isFrozen)
            {
                FreezeToStatic();
            }
        }
    }
}
