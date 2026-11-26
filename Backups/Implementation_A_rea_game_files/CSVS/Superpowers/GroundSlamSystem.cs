using UnityEngine;
using CSVS.Destruction;
using CSVS.Core;
using CSVS.Camera;

namespace CSVS.Superpowers
{
    /// <summary>
    /// Executes devastating kinetic Ground Slams upon high-velocity impact with the ground or buildings.
    /// Emits radial physics impulses, shatters nearby structures, and injects camera trauma.
    /// </summary>
    public class GroundSlamSystem : MonoBehaviour
    {
        [Header("Slam Blast Parameters")]
        [SerializeField] private float baseBlastRadius = 22f;
        [SerializeField] private float baseExplosiveForce = 1800f;
        [SerializeField] private float upwardModifier = 1.8f;
        [SerializeField] private float maxDamage = 600f;
        [SerializeField] private LayerMask affectedLayers;

        [Header("Effects & Audio")]
        [SerializeField] private ParticleSystem slamShockwavePrefab;
        [SerializeField] private AudioClip slamAudioClip;

        public void ExecuteGroundSlam(Vector3 impactPoint, Vector3 surfaceNormal, float impactVelocity)
        {
            float speedRatio = Mathf.Clamp(impactVelocity / 40f, 1f, 3f);
            float effectiveRadius = baseBlastRadius * speedRatio;
            float effectiveForce = baseExplosiveForce * speedRatio;

            // 1. Camera Shake
            if (SuperheroCameraController.Instance != null)
            {
                SuperheroCameraController.Instance.AddTrauma(0.85f);
            }

            // 2. Spawn Shockwave Particle FX
            if (slamShockwavePrefab != null)
            {
                Instantiate(slamShockwavePrefab, impactPoint + surfaceNormal * 0.2f, Quaternion.LookRotation(surfaceNormal));
            }

            // 3. Audio Burst
            if (slamAudioClip != null)
            {
                AudioSource.PlayClipAtPoint(slamAudioClip, impactPoint, 1.0f);
            }

            // 4. OverlapSphere to identify affected Rigidbodies and Destructibles
            Collider[] colliders = Physics.OverlapSphere(impactPoint, effectiveRadius, affectedLayers);
            foreach (var col in colliders)
            {
                float dist = Vector3.Distance(impactPoint, col.transform.position);
                float damageFalloff = 1f - Mathf.Clamp01(dist / effectiveRadius);

                // Destructible Building Damage
                DestructibleBuilding building = col.GetComponentInParent<DestructibleBuilding>();
                if (building != null)
                {
                    building.ApplyExplosiveDamage(impactPoint, maxDamage * damageFalloff * speedRatio, effectiveRadius);
                }

                BuildingChunk chunk = col.GetComponent<BuildingChunk>();
                if (chunk != null)
                {
                    chunk.ApplyDamage(maxDamage * damageFalloff, impactPoint);
                }

                // Physics Impulse
                Rigidbody rb = col.attachedRigidbody;
                if (rb != null && !rb.isKinematic)
                {
                    rb.AddExplosionForce(effectiveForce, impactPoint, effectiveRadius, upwardModifier, ForceMode.Impulse);
                }
            }

            // 5. Award Chaos points
            if (ChaosManager.Instance != null)
            {
                ChaosManager.Instance.AddChaos((long)(500 * speedRatio), "GroundSlam");
            }
        }
    }
}
