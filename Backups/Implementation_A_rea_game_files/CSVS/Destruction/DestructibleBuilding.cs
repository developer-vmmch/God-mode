using System.Collections.Generic;
using UnityEngine;
using CSVS.Core;

namespace CSVS.Destruction
{
    /// <summary>
    /// Master coordinator for a destructible building.
    /// Manages structural support graphs, foundation failure thresholds,
    /// progressive floor-by-floor cascading collapse, and debris particles.
    /// </summary>
    public class DestructibleBuilding : MonoBehaviour
    {
        [Header("Building Specifications")]
        [SerializeField] private string buildingName = "Commercial High-Rise";
        [SerializeField] private int totalFloors = 5;
        [SerializeField] private float buildingTotalHealth = 2500f;
        [SerializeField] private float currentBuildingHealth;

        [Header("Collapse Cascading")]
        [SerializeField] private float collapseThresholdRatio = 0.45f; // Collapse if >45% support destroyed
        [SerializeField] private float floorCollapseDelay = 0.22f;
        [SerializeField] private bool isCollapsing = false;

        [Header("VFX & Dust Volumes")]
        [SerializeField] private ParticleSystem collapseDustFXPrefab;
        [SerializeField] private AudioClip collapseSound;

        private List<BuildingChunk> allChunks = new List<BuildingChunk>();
        private List<BuildingChunk> supportChunks = new List<BuildingChunk>();
        private int destroyedSupportCount = 0;

        public bool IsCollapsing => isCollapsing;
        public float HealthRatio => Mathf.Clamp01(currentBuildingHealth / buildingTotalHealth);

        private void Awake()
        {
            currentBuildingHealth = buildingTotalHealth;
            IndexBuildingChunks();
        }

        private void IndexBuildingChunks()
        {
            BuildingChunk[] chunks = GetComponentsInChildren<BuildingChunk>();
            foreach (var chunk in chunks)
            {
                allChunks.Add(chunk);
                if (chunk.IsStructuralSupport)
                {
                    supportChunks.Add(chunk);
                }
            }
        }

        public void ApplyThermalDamage(Vector3 hitPoint, float damage)
        {
            if (isCollapsing) return;

            currentBuildingHealth -= damage;
            CheckCollapseCriteria();
        }

        public void ApplyExplosiveDamage(Vector3 center, float force, float radius)
        {
            if (isCollapsing) return;

            currentBuildingHealth -= force * 1.5f;

            // Damage nearby chunks within blast radius
            foreach (var chunk in allChunks)
            {
                if (chunk == null || chunk.IsFractured) continue;
                float d = Vector3.Distance(center, chunk.transform.position);
                if (d <= radius)
                {
                    float falloff = 1f - (d / radius);
                    chunk.ApplyDamage(force * falloff, center);
                }
            }

            CheckCollapseCriteria();
        }

        public void NotifyChunkDetached(BuildingChunk chunk)
        {
            if (chunk.IsStructuralSupport)
            {
                destroyedSupportCount++;
                CheckCollapseCriteria();
            }
        }

        private void CheckCollapseCriteria()
        {
            if (isCollapsing) return;

            float supportRatio = supportChunks.Count > 0 ? (float)destroyedSupportCount / supportChunks.Count : 1f;

            if (supportRatio >= collapseThresholdRatio || currentBuildingHealth <= 0f)
            {
                TriggerStructuralCollapse();
            }
        }

        public void TriggerStructuralCollapse()
        {
            if (isCollapsing) return;
            isCollapsing = true;

            StartCoroutine(CollapseSequenceCoroutine());

            if (collapseDustFXPrefab != null)
            {
                Instantiate(collapseDustFXPrefab, transform.position, Quaternion.identity);
            }

            if (collapseSound != null)
            {
                AudioSource.PlayClipAtPoint(collapseSound, transform.position);
            }

            if (ChaosManager.Instance != null)
            {
                ChaosManager.Instance.AddChaos(2500, "BuildingCollapse: " + buildingName);
            }
        }

        private System.Collections.IEnumerator CollapseSequenceCoroutine()
        {
            // Sort chunks by altitude (bottom to top)
            allChunks.Sort((a, b) => a.transform.position.y.CompareTo(b.transform.position.y));

            // Collapse in ascending waves to simulate real progressive gravity pancaking
            int batchSize = Mathf.Max(3, allChunks.Count / totalFloors);
            for (int i = 0; i < allChunks.Count; i += batchSize)
            {
                int end = Mathf.Min(i + batchSize, allChunks.Count);
                for (int j = i; j < end; j++)
                {
                    if (allChunks[j] != null && !allChunks[j].IsFractured)
                    {
                        allChunks[j].DetachAndActivatePhysics();
                    }
                }
                yield return new WaitForSeconds(floorCollapseDelay);
            }
        }
    }
}
