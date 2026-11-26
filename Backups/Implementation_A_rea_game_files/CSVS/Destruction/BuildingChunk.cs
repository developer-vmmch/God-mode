using UnityEngine;
using CSVS.Core;

namespace CSVS.Destruction
{
    /// <summary>
    /// Individual pre-fractured section of a destructible building.
    /// Manages transition from static/kinematic structural member to dynamic PhysX debris,
    /// impulse transfer, and automatic sleep/freeze optimization.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class BuildingChunk : MonoBehaviour
    {
        [Header("Structural Integrity")]
        [SerializeField] private float maxHealth = 150f;
        [SerializeField] private float currentHealth;
        [SerializeField] private bool isStructuralSupport = false;

        [Header("Physics State")]
        [SerializeField] private bool isFractured = false;
        [SerializeField] private float mass = 120f;
        [SerializeField] private float settleSpeedThreshold = 0.15f;
        [SerializeField] private float settleTimeDuration = 1.2f;

        private Rigidbody rb;
        private Collider col;
        private DestructibleBuilding parentBuilding;
        private float restTimer = 0f;

        public bool IsFractured => isFractured;
        public bool IsStructuralSupport => isStructuralSupport;
        public float HealthPercentage => Mathf.Clamp01(currentHealth / maxHealth);

        private void Awake()
        {
            currentHealth = maxHealth;
            col = GetComponent<Collider>();
            parentBuilding = GetComponentInParent<DestructibleBuilding>();

            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
            }
            rb.mass = mass;
            rb.isKinematic = true; // Kinematic while intact
            rb.interpolation = RigidbodyInterpolation.None;
        }

        public void ApplyDamage(float amount, Vector3 hitPoint)
        {
            if (isFractured && currentHealth <= 0) return;

            currentHealth -= amount;
            if (currentHealth <= 0 && !isFractured)
            {
                DetachAndActivatePhysics();
                if (rb != null && !rb.isKinematic)
                {
                    Vector3 impulseDir = (transform.position - hitPoint).normalized + Vector3.up * 0.2f;
                    rb.AddForceAtPosition(impulseDir * 25f, hitPoint, ForceMode.Impulse);
                }
            }
        }

        public void DetachAndActivatePhysics()
        {
            if (isFractured) return;
            isFractured = true;

            // Notify building coordinator
            if (parentBuilding != null)
            {
                parentBuilding.NotifyChunkDetached(this);
            }

            // Register with DebrisManager to maintain active dynamic body budget
            if (DebrisManager.Instance != null)
            {
                DebrisManager.Instance.RegisterActiveDebris(this);
            }

            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
                rb.interpolation = RigidbodyInterpolation.Interpolate;
                rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
            }

            transform.SetParent(null); // Unparent from building root for clean physics
        }

        private void FixedUpdate()
        {
            if (!isFractured || rb == null || rb.isKinematic) return;

            // Check if settled to freeze back to static/kinematic
            if (rb.linearVelocity.sqrMagnitude < settleSpeedThreshold * settleSpeedThreshold)
            {
                restTimer += Time.fixedDeltaTime;
                if (restTimer >= settleTimeDuration)
                {
                    FreezeChunk();
                }
            }
            else
            {
                restTimer = 0f;
            }
        }

        private void FreezeChunk()
        {
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            if (DebrisManager.Instance != null)
            {
                DebrisManager.Instance.UnregisterActiveDebris(this);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            // Transfer impact damage to other chunks if colliding at high speed
            if (collision.relativeVelocity.sqrMagnitude > 40f)
            {
                BuildingChunk otherChunk = collision.collider.GetComponent<BuildingChunk>();
                if (otherChunk != null && !otherChunk.isFractured)
                {
                    float transferDamage = collision.relativeVelocity.magnitude * 8f;
                    otherChunk.ApplyDamage(transferDamage, collision.contacts[0].point);
                }
            }
        }
    }
}
