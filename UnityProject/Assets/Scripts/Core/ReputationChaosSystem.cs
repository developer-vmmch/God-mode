// ReputationChaosSystem.cs
// CSVS GOD MODE — V0.3 Original Reputation & Chaos Rating System
// Two independent progression axes: Hero Reputation vs Chaos Rating.

using System;
using UnityEngine;
using UnityEngine.Events;
using CSVS.GodMode.AI;

namespace CSVS.GodMode.Core
{
    public class ReputationChaosSystem : MonoBehaviour
    {
        // ─── Inspector ────────────────────────────────────────────────────
        [Header("Reputation")]
        [SerializeField, Range(0, 1000)] private int reputation = 0;
        [SerializeField] private int maxReputation = 1000;

        [Header("Chaos")]
        [SerializeField, Range(0, 5)] private int chaosLevel = 0;      // 0–5 stars
        [SerializeField] private float chaosPoints = 0f;
        [SerializeField] private float chaosDecayRate = 2f;            // points/second
        [SerializeField] private float chaosThresholdPerLevel = 100f;

        [Header("Threat Response")]
        [SerializeField] private float pursuitCooldownAfterLOS = 30f;  // seconds out of sight
        [SerializeField] private bool threatActive;

        // ─── Events ───────────────────────────────────────────────────────
        public event Action<int>  OnReputationChanged;
        public event Action<int>  OnChaosLevelChanged;
        public event Action       OnThreatActivated;
        public event Action       OnThreatCooldown;

        // ─── State ─────────────────────────────────────────────────────────
        public int   Reputation  => reputation;
        public int   ChaosLevel  => chaosLevel;
        public float ChaosPoints => chaosPoints;
        public bool  ThreatActive=> threatActive;

        private float _lostSightTimer;
        private static ReputationChaosSystem _instance;
        public static ReputationChaosSystem Instance => _instance;

        // ─── Lifecycle ─────────────────────────────────────────────────────
        private void Awake() { _instance = this; }

        private void Update()
        {
            // Decay chaos while not actively causing damage
            if (chaosPoints > 0 && !threatActive)
            {
                chaosPoints = Mathf.Max(0f, chaosPoints - chaosDecayRate * Time.deltaTime);
                RecalculateChaosLevel();
            }

            // Pursuit cooldown
            if (threatActive)
            {
                _lostSightTimer -= Time.deltaTime;
                if (_lostSightTimer <= 0f)
                {
                    threatActive = false;
                    OnThreatCooldown?.Invoke();
                    Debug.Log("[Reputation] Threat cooldown — lost sight of player.");
                }
            }
        }

        // ─── API ───────────────────────────────────────────────────────────
        public void AddReputation(int amount)
        {
            reputation = Mathf.Clamp(reputation + amount, 0, maxReputation);
            OnReputationChanged?.Invoke(reputation);
            Debug.Log($"[Reputation] +{amount} REP → {reputation}");
        }

        public void AddChaos(float amount, bool triggersAlert = true)
        {
            chaosPoints += amount;
            RecalculateChaosLevel();
            if (triggersAlert && chaosLevel >= 1)
                TriggerThreatResponse();
        }

        // Building destruction, vehicle damage, civilian harm
        public void RecordBuildingDamage(float destructionFraction)
            => AddChaos(destructionFraction * 25f);

        public void RecordVehicleDamage(float damageFraction)
            => AddChaos(damageFraction * 10f);

        public void RecordCivilianRescue(int count)
            => AddReputation(count * 15);

        public void RecordMissionComplete(int reward)
            => AddReputation(reward);

        public void ResetLostSightTimer()
        {
            _lostSightTimer = pursuitCooldownAfterLOS;
            if (!threatActive)
            {
                threatActive = true;
                OnThreatActivated?.Invoke();
            }
        }

        // ─── Internal ──────────────────────────────────────────────────────
        private void RecalculateChaosLevel()
        {
            int newLevel = Mathf.Min(5, Mathf.FloorToInt(chaosPoints / chaosThresholdPerLevel));
            if (newLevel != chaosLevel)
            {
                chaosLevel = newLevel;
                OnChaosLevelChanged?.Invoke(chaosLevel);
                Debug.Log($"[Chaos] Level → {chaosLevel} ★ (points: {chaosPoints:F0})");
            }
        }

        private void TriggerThreatResponse()
        {
            ResetLostSightTimer();
            var threat = FindObjectOfType<ThreatResponseManager>();
            threat?.EscalateThreat(chaosLevel);
        }

        // ─── Save / Load ───────────────────────────────────────────────────
        public void SaveProgress()
        {
            PlayerPrefs.SetInt("Reputation", reputation);
            PlayerPrefs.SetFloat("ChaosPoints", chaosPoints);
            PlayerPrefs.SetInt("ChaosLevel", chaosLevel);
            PlayerPrefs.Save();
        }

        public void LoadProgress()
        {
            reputation  = PlayerPrefs.GetInt("Reputation", 0);
            chaosPoints = PlayerPrefs.GetFloat("ChaosPoints", 0f);
            chaosLevel  = PlayerPrefs.GetInt("ChaosLevel", 0);
        }
    }
}

