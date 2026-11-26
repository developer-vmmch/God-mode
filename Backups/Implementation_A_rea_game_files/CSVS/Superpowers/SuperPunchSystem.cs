using UnityEngine;
using CSVS.Input;
using CSVS.Destruction;
using CSVS.Core;
using CSVS.Camera;

namespace CSVS.Superpowers
{
    /// <summary>
    /// Super Punch ability (F key).
    /// Delivers localized kinetic punch dealing high structural wall damage and vehicle knockback.
    /// </summary>
    public class SuperPunchSystem : MonoBehaviour
    {
        [Header("Punch Parameters")]
        [SerializeField] private float punchRange = 3.5f;
        [SerializeField] private float punchDamage = 450f;
        [SerializeField] private float punchImpulseForce = 85f;
        [SerializeField] private LayerMask punchLayers;

        [Header("VFX & Audio")]
        [SerializeField] private ParticleSystem punchImpactFX;
        [SerializeField] private AudioClip punchAudio;

        private void Update()
        {
            if (SuperheroInput.Instance != null && SuperheroInput.Instance.SuperPunchPressed)
            {
                ExecutePunch();
            }
        }

        private void ExecutePunch()
        {
            UnityEngine.Camera cam = UnityEngine.Camera.main;
            Ray ray = cam != null
                ? cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f))
                : new Ray(transform.position + Vector3.up * 1.5f, transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, punchRange, punchLayers))
            {
                // Structural building chunk damage
                BuildingChunk chunk = hit.collider.GetComponent<BuildingChunk>();
                if (chunk != null)
                {
                    chunk.ApplyDamage(punchDamage, hit.point);
                }

                DestructibleBuilding building = hit.collider.GetComponentInParent<DestructibleBuilding>();
                if (building != null)
                {
                    building.ApplyExplosiveDamage(hit.point, punchDamage, 3f);
                }

                // Push rigidbodies
                Rigidbody rb = hit.collider.attachedRigidbody;
                if (rb != null && !rb.isKinematic)
                {
                    rb.AddForceAtPosition(ray.direction * punchImpulseForce, hit.point, ForceMode.Impulse);
                }

                // Camera shake
                if (SuperheroCameraController.Instance != null)
                {
                    SuperheroCameraController.Instance.AddTrauma(0.5f);
                }

                if (punchImpactFX != null)
                {
                    Instantiate(punchImpactFX, hit.point, Quaternion.LookRotation(hit.normal));
                }

                if (punchAudio != null)
                {
                    AudioSource.PlayClipAtPoint(punchAudio, hit.point);
                }

                if (ChaosManager.Instance != null)
                {
                    ChaosManager.Instance.AddChaos(200, "SuperPunch");
                }
            }
        }
    }
}
