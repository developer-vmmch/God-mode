using UnityEngine;
using TMPro;
using CSVS.GodMode.Core;
using CSVS.GodMode.Player;

namespace CSVS.GodMode.UI
{
    public class HUDController : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI _chaosScoreText;
        [SerializeField] private TextMeshProUGUI _wantedTierText;
        [SerializeField] private TextMeshProUGUI _speedometerText;
        [SerializeField] private GameObject _tutorialPanel;
        [SerializeField] private GameObject _crosshair;

        [Header("Player Reference")]
        [SerializeField] private SuperheroFlightController _playerFlight;

        private void Start()
        {
            if (_playerFlight == null)
            {
                _playerFlight = FindFirstObjectByType<SuperheroFlightController>();
            }

            if (ChaosSystem.Instance != null)
            {
                ChaosSystem.Instance.OnChaosScoreChanged += UpdateChaosScore;
                ChaosSystem.Instance.OnWantedTierEscalated += UpdateWantedTier;
                UpdateChaosScore(ChaosSystem.Instance.CurrentChaosScore, 0);
                UpdateWantedTier(ChaosSystem.Instance.CurrentWantedTier);
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnTutorialSkipped += HideTutorial;
            }
        }

        private void OnDestroy()
        {
            if (ChaosSystem.Instance != null)
            {
                ChaosSystem.Instance.OnChaosScoreChanged -= UpdateChaosScore;
                ChaosSystem.Instance.OnWantedTierEscalated -= UpdateWantedTier;
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnTutorialSkipped -= HideTutorial;
            }
        }

        private void Update()
        {
            if (_playerFlight != null && _speedometerText != null)
            {
                float speedKmh = _playerFlight.CurrentSpeed * 3.6f;
                string boostTag = _playerFlight.IsBoosting ? " <color=#FF3300>[SUPERSONIC]</color>" : "";
                _speedometerText.text = $"{Mathf.RoundToInt(speedKmh)} KM/H{boostTag}";
            }
        }

        private void UpdateChaosScore(long newScore, long addedAmount)
        {
            if (_chaosScoreText != null)
            {
                _chaosScoreText.text = $"CHAOS: ${newScore:N0}";
            }
        }

        private void UpdateWantedTier(WantedTier tier)
        {
            if (_wantedTierText != null)
            {
                _wantedTierText.text = $"THREAT LEVEL: {tier.ToString().ToUpper()}";
            }
        }

        private void HideTutorial()
        {
            if (_tutorialPanel != null)
            {
                _tutorialPanel.SetActive(false);
            }
        }
    }
}
