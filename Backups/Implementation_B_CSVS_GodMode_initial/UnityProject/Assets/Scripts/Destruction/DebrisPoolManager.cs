using System;
using System.Collections.Generic;
using UnityEngine;
using CSVS.GodMode.Core;

namespace CSVS.GodMode.Destruction
{
    public class DebrisPoolManager : MonoBehaviour
    {
        public static DebrisPoolManager Instance { get; private set; }

        [Header("WebGL Performance Budget")]
        [SerializeField] private int _maxActiveDynamicBodies = 150;
        [SerializeField] private float _defaultDespawnTime = 12.0f;

        private readonly List<BuildingFractureChunk> _activeChunks = new List<BuildingFractureChunk>();

        public int CurrentActiveCount => _activeChunks.Count;
        public int MaxActiveBudget => _maxActiveDynamicBodies;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void RegisterActiveChunk(BuildingFractureChunk chunk)
        {
            if (chunk == null) return;

            // If we exceeded the budget of 150 bodies, freeze the oldest active chunk
            if (_activeChunks.Count >= _maxActiveDynamicBodies)
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
