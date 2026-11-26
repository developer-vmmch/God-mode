using System;
using System.Collections.Generic;
using UnityEngine;
using CSVS.GodMode.Destruction;

namespace CSVS.GodMode.Environment
{
    /// <summary>
    /// Scalable 250-meter spatial chunk world streamer for the Karaikal open world.
    /// Dynamically loads, activates, and culls world sectors based on player proximity.
    /// Preserves structural destruction states across chunk lifecycles.
    /// Optimized for low-spec hardware (Intel Core i3 / UHD 630 / 8GB RAM).
    /// </summary>
    public class KaraikalWorldStreamer : MonoBehaviour
    {
        public const float ChunkSize = 250.0f; // 250m grid sectors

        [Header("Streaming Configuration")]
        [SerializeField] private float _loadRadius = 500.0f;     // Active chunk radius (~2 chunks)
        [SerializeField] private float _unloadRadius = 650.0f;   // Deactivation hysteresis buffer
        [SerializeField] private Transform _targetPlayer;
        [SerializeField] private ModularIndianBuildingSystem _buildingSystem;

        private readonly Dictionary<Vector2Int, ChunkData> _activeChunks = new Dictionary<Vector2Int, ChunkData>();
        private readonly Dictionary<string, bool> _destroyedBuildingState = new Dictionary<string, bool>();

        private Vector2Int _currentCenterChunkCoord;
        private float _updateTimer = 0.0f;
        private const float UpdateInterval = 0.35f; // Evaluate streaming 3x per second

        public class ChunkData
        {
            public Vector2Int Coord;
            public Vector3 WorldCenter;
            public GameObject RootObject;
            public bool IsActive;

            public ChunkData(Vector2Int coord, Vector3 center, GameObject root)
            {
                Coord = coord;
                WorldCenter = center;
                RootObject = root;
                IsActive = true;
            }
        }

        private void Awake()
        {
            if (_buildingSystem == null)
            {
                _buildingSystem = GetComponent<ModularIndianBuildingSystem>();
                if (_buildingSystem == null) _buildingSystem = gameObject.AddComponent<ModularIndianBuildingSystem>();
            }
        }

        private void Start()
        {
            if (_targetPlayer == null)
            {
                var hero = GameObject.FindGameObjectWithTag("Player");
                if (hero != null) _targetPlayer = hero.transform;
            }

            UpdateStreamingGrid(true);
        }

        private void Update()
        {
            if (_targetPlayer == null)
            {
                var hero = GameObject.FindGameObjectWithTag("Player");
                if (hero != null) _targetPlayer = hero.transform;
                else return;
            }

            _updateTimer += Time.deltaTime;
            if (_updateTimer >= UpdateInterval)
            {
                _updateTimer = 0.0f;
                UpdateStreamingGrid(false);
            }
        }

        public static Vector2Int WorldToChunkCoord(Vector3 worldPos)
        {
            int cx = Mathf.FloorToInt((worldPos.x + ChunkSize * 0.5f) / ChunkSize);
            int cz = Mathf.FloorToInt((worldPos.z + ChunkSize * 0.5f) / ChunkSize);
            return new Vector2Int(cx, cz);
        }

        public static Vector3 ChunkCoordToWorldCenter(Vector2Int coord)
        {
            return new Vector3(coord.x * ChunkSize, 0f, coord.y * ChunkSize);
        }

        public void UpdateStreamingGrid(bool forceImmediate)
        {
            if (_targetPlayer == null) return;

            Vector3 playerPos = _targetPlayer.position;
            Vector2Int centerCoord = WorldToChunkCoord(playerPos);

            if (!forceImmediate && centerCoord == _currentCenterChunkCoord)
            {
                return;
            }
            _currentCenterChunkCoord = centerCoord;

            int chunkRadius = Mathf.CeilToInt(_loadRadius / ChunkSize);
            HashSet<Vector2Int> desiredChunks = new HashSet<Vector2Int>();

            // Collect chunks within spherical load radius
            for (int dx = -chunkRadius; dx <= chunkRadius; dx++)
            {
                for (int dz = -chunkRadius; dz <= chunkRadius; dz++)
                {
                    Vector2Int coord = new Vector2Int(centerCoord.x + dx, centerCoord.y + dz);
                    Vector3 chunkCenter = ChunkCoordToWorldCenter(coord);
                    float dist = Vector2.Distance(new Vector2(playerPos.x, playerPos.z), new Vector2(chunkCenter.x, chunkCenter.z));

                    if (dist <= _loadRadius)
                    {
                        desiredChunks.Add(coord);
                    }
                }
            }

            // 1. Deactivate or Unload chunks outside unload radius
            List<Vector2Int> toRemove = new List<Vector2Int>();
            foreach (var kvp in _activeChunks)
            {
                Vector2Int coord = kvp.Key;
                ChunkData chunk = kvp.Value;
                float dist = Vector2.Distance(new Vector2(playerPos.x, playerPos.z), new Vector2(chunk.WorldCenter.x, chunk.WorldCenter.z));

                if (dist > _unloadRadius)
                {
                    if (chunk.RootObject != null)
                    {
                        SaveChunkDestructionState(chunk);
                        chunk.RootObject.SetActive(false);
                        chunk.IsActive = false;
                    }
                    toRemove.Add(coord);
                }
            }

            foreach (var coord in toRemove)
            {
                _activeChunks.Remove(coord);
            }

            // 2. Load or Activate desired chunks
            foreach (var coord in desiredChunks)
            {
                if (!_activeChunks.ContainsKey(coord))
                {
                    LoadChunk(coord);
                }
                else if (!_activeChunks[coord].IsActive)
                {
                    _activeChunks[coord].RootObject.SetActive(true);
                    _activeChunks[coord].IsActive = true;
                }
            }
        }

        private void LoadChunk(Vector2Int coord)
        {
            Vector3 center = ChunkCoordToWorldCenter(coord);
            string chunkName = $"Chunk_{coord.x}_{coord.y}";

            Transform existingTransform = transform.Find(chunkName);
            GameObject chunkObj;

            if (existingTransform != null)
            {
                chunkObj = existingTransform.gameObject;
                chunkObj.SetActive(true);
            }
            else
            {
                chunkObj = new GameObject(chunkName);
                chunkObj.transform.SetParent(transform, false);
                chunkObj.transform.position = center;
                PopulateChunkContent(chunkObj, coord, center);
            }

            ChunkData data = new ChunkData(coord, center, chunkObj);
            _activeChunks[coord] = data;
            ApplyChunkDestructionState(data);
        }

        private void PopulateChunkContent(GameObject chunkRoot, Vector2Int coord, Vector3 center)
        {
            KaraikalDistrictType district = KaraikalGeoProjection.GetDistrictAtPosition(center);
            int seed = (coord.x * 73856093) ^ (coord.y * 19349663);
            System.Random rand = new System.Random(seed);

            // Populate building clusters within the 250m chunk boundary
            // Place 4 to 8 modular buildings per chunk depending on district density
            int buildingCount = (district == KaraikalDistrictType.TownCenter || district == KaraikalDistrictType.ModernExpansion) ? 6 : 4;
            float halfSpan = ChunkSize * 0.38f;

            for (int i = 0; i < buildingCount; i++)
            {
                float ox = (float)(rand.NextDouble() * 2.0 - 1.0) * halfSpan;
                float oz = (float)(rand.NextDouble() * 2.0 - 1.0) * halfSpan;

                // Avoid placing buildings directly on Kamarajar Salai (X ~ 0) or Bharathiyar Rd (Z ~ 0)
                Vector3 bWorldPos = center + new Vector3(ox, 0f, oz);
                if (Mathf.Abs(bWorldPos.x) < 18f || Mathf.Abs(bWorldPos.z) < 14f) continue;
                if (bWorldPos.z > -900f && bWorldPos.z < -730f) continue; // Arasalar riverbed

                GameObject bldg = _buildingSystem.ConstructBuilding(bWorldPos, district, seed + i);
                bldg.name = $"Bldg_{coord.x}_{coord.y}_{i}";
                bldg.transform.SetParent(chunkRoot.transform, true);
            }
        }

        private void SaveChunkDestructionState(ChunkData chunk)
        {
            if (chunk.RootObject == null) return;
            var destructibles = chunk.RootObject.GetComponentsInChildren<DestructibleBuilding>(true);
            foreach (var d in destructibles)
            {
                _destroyedBuildingState[d.gameObject.name] = d.IsDestroyed;
            }
        }

        private void ApplyChunkDestructionState(ChunkData chunk)
        {
            if (chunk.RootObject == null) return;
            var destructibles = chunk.RootObject.GetComponentsInChildren<DestructibleBuilding>(true);
            foreach (var d in destructibles)
            {
                if (_destroyedBuildingState.TryGetValue(d.gameObject.name, out bool destroyed) && destroyed)
                {
                    // Trigger fracture immediately to restore state
                    d.FractureBuilding();
                }
            }
        }

        public int ActiveChunkCount => _activeChunks.Count;
    }
}
