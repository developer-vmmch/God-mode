using UnityEngine;
using CSVS.GodMode.Core;
using CSVS.GodMode.Destruction;
using CSVS.GodMode.Player;

namespace CSVS.GodMode.Powers
{
    /// <summary>
    /// Executes kinetic Ground Slam shockwaves upon supersonic impact.
    /// Employs standardized quadratic radial force attenuation:
    /// F = Fmax * Pow(Clamp01(1 - distance / radius), 2)
    /// </summary>
    public class GroundSlamSystem : MonoBehaviour
    {
        [Header("Slam Shockwave Parameters")]
        [SerializeField] private float _maxForce = 1500.0f;
        [SerializeField] private float _radius = 25.0f;
        [SerializeField] private float _damage = 500.0f;
        [SerializeField] private float _upwardModifier = 1.5f;

        [Header("VFX & Audio")]
        [SerializeField] private ParticleSystem _shockwavePrefab;
        [SerializeField] private AudioClip _slamAudio;

        private void Start()
        {
            if (GameplayConfig.Instance != null)
            {
                _maxForce = GameplayConfig.Instance.GroundSlamMaxForce;
                _radius = GameplayConfig.Instance.GroundSlamRadius;
                _damage = GameplayConfig.Instance.GroundSlamDamage;
            }
        }

        public void ExecuteGroundSlam(Vector3 epicenter, Vector3 normal, float impactSpeed)
        {
            // Camera shake
            SuperheroCameraController cam = FindFirstObjectByType<SuperheroCameraController>();
            if (cam != null)
            {
                cam.TriggerShake(1.2f);
            }

            // Spawn shockwave FX
            if (_shockwavePrefab != null)
            {
                Instantiate(_shockwavePrefab, epicenter + normal * 0.2f, Quaternion.LookRotation(normal));
            }

            if (_slamAudio != null)
            {
                AudioSource.PlayClipAtPoint(_slamAudio, epicenter, 1.0f);
            }

            // OverlapSphere to identify affected entities
            Collider[] hitColliders = Physics.OverlapSphere(epicenter, _radius);
            foreach (Collider hit in hitColliders)
            {
                if (hit.transform == transform || hit.transform.IsChildOf(transform)) continue;

                float distance = Vector3.Distance(epicenter, hit.transform.position);
                if (distance > _radius) continue;

                // Exact quadratic falloff: F = Fmax * (1 - d/R)^2
                float quadraticFalloff = GameplayConfig.CalculateGroundSlamForce(distance, _radius, 1.0f);
                float finalForce = _maxForce * quadraticFalloff;
                float finalDamage = _damage * quadraticFalloff;

                // Deliver explosive damage to destructibles
                IDamageable damageable = hit.GetComponentInParent<IDamageable>();
                if (damageable != null)
                {
                    Vector3 hitNormal = (hit.transform.position - epicenter).normalized;
                    damageable.TakeDamage(finalDamage, DamageType.Explosive, hit.ClosestPoint(epicenter), hitNormal);
                }

                // Apply physical impulse
                Rigidbody rb = hit.attachedRigidbody;
                if (rb != null && !rb.isKinematic)
                {
                    rb.AddExplosionForce(finalForce, epicenter, _radius, _upwardModifier, ForceMode.Impulse);
                }
            }

            // Award Chaos score
            if (ChaosSystem.Instance != null)
            {
                ChaosSystem.Instance.AddChaos(1000, "GroundSlam");
            }
        }
    }
}
