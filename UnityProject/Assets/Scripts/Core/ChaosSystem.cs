using System;
using UnityEngine;

namespace CSVS.GodMode.Core
{
    public enum WantedTier
    {
        Peaceful = 0,
        Police = 1,
        Swat = 2,
        MilitaryTanks = 3,
        AttackHelicopters = 4
    }

    public class ChaosSystem : MonoBehaviour
    {
        public static ChaosSystem Instance { get; private set; }

        [Header("Chaos Scoring")]
        [SerializeField] private long _currentChaosScore = 0;
        [SerializeField] private float _chaosMultiplier = 1.0f;

        [Header("Escalation Thresholds")]
        [SerializeField] private long _policeThreshold = 10000;
        [SerializeField] private long _swatThreshold = 50000;
        [SerializeField] private long _militaryThreshold = 200000;
        [SerializeField] private long _helicopterThreshold = 500000;

        public long CurrentChaosScore => _currentChaosScore;
        public float ChaosMultiplier => _chaosMultiplier;
        public WantedTier CurrentWantedTier { get; private set; } = WantedTier.Peaceful;

        public event Action<long, long> OnChaosScoreChanged; // (newScore, addedAmount)
        public event Action<WantedTier> OnWantedTierEscalated;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void AddChaos(long baseAmount, string destructionSource = "")
        {
            if (baseAmount <= 0) return;

            long finalAmount = (long)(baseAmount * _chaosMultiplier);
            _currentChaosScore += finalAmount;

            EvaluateWantedTier();
            OnChaosScoreChanged?.Invoke(_currentChaosScore, finalAmount);
        }

        public void SetMultiplier(float multiplier)
        {
            _chaosMultiplier = Mathf.Max(1.0f, multiplier);
        }

        private void EvaluateWantedTier()
        {
            WantedTier newTier = WantedTier.Peaceful;

            if (_currentChaosScore >= _helicopterThreshold)
            {
                newTier = WantedTier.AttackHelicopters;
            }
            else if (_currentChaosScore >= _militaryThreshold)
            {
                newTier = WantedTier.MilitaryTanks;
            }
            else if (_currentChaosScore >= _swatThreshold)
            {
                newTier = WantedTier.Swat;
            }
            else if (_currentChaosScore >= _policeThreshold)
            {
                newTier = WantedTier.Police;
            }

            if (newTier > CurrentWantedTier)
            {
                CurrentWantedTier = newTier;
                OnWantedTierEscalated?.Invoke(CurrentWantedTier);
            }
        }

        public void ResetScore()
        {
            _currentChaosScore = 0;
            CurrentWantedTier = WantedTier.Peaceful;
            OnChaosScoreChanged?.Invoke(0, 0);
        }
    }
}
