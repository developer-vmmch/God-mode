using System;
using UnityEngine;

namespace CSVS.Core
{
    /// <summary>
    /// Tracks destruction score, chaos meter, and military/police escalation level.
    /// </summary>
    public class ChaosManager : MonoBehaviour
    {
        public static ChaosManager Instance { get; private set; }

        [Header("Chaos Progression")]
        [SerializeField] private long totalChaosScore = 0;
        [SerializeField] private int currentWantedLevel = 0; // 0 to 6
        [SerializeField] private float chaosMultiplier = 1.0f;
        [SerializeField] private float multiplierDecayTimer = 5f;

        public long TotalChaosScore => totalChaosScore;
        public int CurrentWantedLevel => currentWantedLevel;
        public float ChaosMultiplier => chaosMultiplier;

        public event Action<long> OnScoreChanged;
        public event Action<int> OnWantedLevelChanged;

        private readonly long[] wantedThresholds = new long[] {
            1000,     // Level 1: Police response
            5000,     // Level 2: Enhanced emergency
            15000,    // Level 3: Armored units
            40000,    // Level 4: Military aircraft
            100000,   // Level 5: Superpowered enemies
            250000    // Level 6: Cinematic boss
        };

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Update()
        {
            if (multiplierDecayTimer > 0)
            {
                multiplierDecayTimer -= Time.deltaTime;
                if (multiplierDecayTimer <= 0)
                {
                    chaosMultiplier = 1.0f;
                }
            }
        }

        public void AddChaos(long basePoints, string source = "")
        {
            long awarded = (long)(basePoints * chaosMultiplier);
            totalChaosScore += awarded;

            // Increase multiplier on continuous destruction
            chaosMultiplier = Mathf.Min(chaosMultiplier + 0.1f, 5.0f);
            multiplierDecayTimer = 6.0f;

            EvaluateWantedLevel();
            OnScoreChanged?.Invoke(totalChaosScore);
        }

        private void EvaluateWantedLevel()
        {
            int newLevel = 0;
            for (int i = 0; i < wantedThresholds.Length; i++)
            {
                if (totalChaosScore >= wantedThresholds[i])
                {
                    newLevel = i + 1;
                }
            }

            if (newLevel != currentWantedLevel)
            {
                currentWantedLevel = newLevel;
                OnWantedLevelChanged?.Invoke(currentWantedLevel);
            }
        }

        public void ResetScore()
        {
            totalChaosScore = 0;
            currentWantedLevel = 0;
            chaosMultiplier = 1.0f;
            OnScoreChanged?.Invoke(0);
            OnWantedLevelChanged?.Invoke(0);
        }
    }
}
