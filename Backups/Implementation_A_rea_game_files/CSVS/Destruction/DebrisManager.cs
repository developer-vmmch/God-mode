using System.Collections.Generic;
using UnityEngine;

namespace CSVS.Destruction
{
    /// <summary>
    /// Enforces the active dynamic physics debris budget (≤ 200 bodies).
    /// Recycles or freezes settled/distant debris chunks to preserve 60 FPS WebGL/PC performance.
    /// </summary>
    public class DebrisManager : MonoBehaviour
    {
        public static DebrisManager Instance { get; private set; }

        [Header("Performance Guardrails")]
        [SerializeField] private int maxActiveDynamicDebris = 180; // Strict limit within 200 target
        [SerializeField] private float autoDespawnDistance = 120f;

        private LinkedList<BuildingChunk> activeDebrisList = new LinkedList<BuildingChunk>();

        public int CurrentActiveDebrisCount => activeDebrisList.Count;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void SetMaxActiveDebris(int limit)
        {
            maxActiveDynamicDebris = Mathf.Clamp(limit, 50, 300);
            EnforceBudget();
        }

        public void RegisterActiveDebris(BuildingChunk chunk)
        {
            if (chunk == null) return;
            activeDebrisList.AddLast(chunk);
            EnforceBudget();
        }

        public void UnregisterActiveDebris(BuildingChunk chunk)
        {
            if (chunk == null) return;
            activeDebrisList.Remove(chunk);
        }

        private void EnforceBudget()
        {
            while (activeDebrisList.Count > maxActiveDynamicDebris)
            {
                // Pop oldest active chunk and freeze or despawn it
                var oldestNode = activeDebrisList.First;
                if (oldestNode != null)
                {
                    BuildingChunk chunk = oldestNode.Value;
                    activeDebrisList.RemoveFirst();

                    if (chunk != null)
                    {
                        Rigidbody rb = chunk.GetComponent<Rigidbody>();
                        if (rb != null)
                        {
                            rb.isKinematic = true; // Instantly freeze to save PhysX CPU
                        }
                    }
                }
            }
        }

        private void Update()
        {
            // Periodically check for distant debris
            if (Time.frameCount % 60 == 0)
            {
                PruneDistantDebris();
            }
        }

        private void PruneDistantDebris()
        {
            UnityEngine.Camera cam = UnityEngine.Camera.main;
            if (cam == null) return;

            Vector3 camPos = cam.transform.position;
            var current = activeDebrisList.First;

            while (current != null)
            {
                var next = current.Next;
                BuildingChunk chunk = current.Value;

                if (chunk != null)
                {
                    float distSqr = (chunk.transform.position - camPos).sqrMagnitude;
                    if (distSqr > autoDespawnDistance * autoDespawnDistance)
                    {
                        Rigidbody rb = chunk.GetComponent<Rigidbody>();
                        if (rb != null) rb.isKinematic = true;
                        activeDebrisList.Remove(current);
                    }
                }
                else
                {
                    activeDebrisList.Remove(current);
                }

                current = next;
            }
        }
    }
}
