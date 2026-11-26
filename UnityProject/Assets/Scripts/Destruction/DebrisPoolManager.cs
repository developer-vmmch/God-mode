using System.Collections.Generic;
using UnityEngine;
using CSVS.GodMode.Core;

namespace CSVS.GodMode.Destruction
{
    /// <summary>
    /// Manages active dynamic debris rigidbodies.
    /// Enforces dynamic budgets (Low=50, Med=100, High=150) and a hard cap of 150
    /// to guarantee smooth frame pacing on target hardware and WebGL.
    /// </summary>
    public class DebrisPoolManager : MonoBehaviour
    {
        public static DebrisPoolManager Instance { get; private set; }

        [Header("WebGL / Hardware Budget")]
        [SerializeField] private int _activeDebrisBudget = 50; // Default Low
        [SerializeField] private int _hardCapCeiling = 150;
        [SerializeField] private float _defaultDespawnTime = 12.0f;

        private readonly List<BuildingFractureChunk> _activeChunks = new List<BuildingFractureChunk>();

        public int CurrentActiveCount => _activeChunks.Count;
        public int MaxActiveBudget => _activeDebrisBudget;
        public int HardCapCeiling => _hardCapCeiling;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (GameplayConfig.Instance != null)
            {
                _activeDebrisBudget = GameplayConfig.Instance.CurrentDebrisBudget;
            }
        }

        public void SetBudget(int budget)
        {
            _activeDebrisBudget = Mathf.Clamp(budget, 10, _hardCapCeiling);

            // If current active count exceeds new budget, immediately freeze oldest excess
            while (_activeChunks.Count > _activeDebrisBudget)
            {
                BuildingFractureChunk oldest = _activeChunks[0];
                _activeChunks.RemoveAt(0);
                if (oldest != null)
                {
                    oldest.FreezeToStatic();
                }
            }
        }

        public void RegisterActiveChunk(BuildingFractureChunk chunk)
        {
            if (chunk == null) return;

            // If we exceeded the current budget, freeze oldest active chunk
            if (_activeChunks.Count >= _activeDebrisBudget)
            {
                BuildingFractureChunk oldest = _activeChunks[0];
                _activeChunks.RemoveAt(0);
                if (oldest != null)
                {
                    oldest.FreezeToStatic();
                }
            }

            _activeChunks.Add(chunk);
        }

        public void UnregisterChunk(BuildingFractureChunk chunk)
        {
            if (chunk != null)
            {
                _activeChunks.Remove(chunk);
            }
        }
    }
}
