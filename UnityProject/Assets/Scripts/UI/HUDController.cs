using UnityEngine;
using TMPro;
using CSVS.GodMode.Core;
using CSVS.GodMode.Player;
using CSVS.GodMode.Settings;

namespace CSVS.GodMode.UI
{
    /// <summary>
    /// Consolidated HUD controller displaying real-time flight telemetry,
    /// current ability state, wanted level, chaos score, camera perspective, and graphics tier.
    /// </summary>
    public class HUDController : MonoBehaviour
    {
        [Header("UI Text Displays")]
        [SerializeField] private TextMeshProUGUI _chaosScoreText;
        [SerializeField] private TextMeshProUGUI _wantedTierText;
        [SerializeField] private TextMeshProUGUI _speedometerText;
        [SerializeField] private TextMeshProUGUI _abilityStateText;
        [SerializeField] private TextMeshProUGUI _cameraModeText;
        [SerializeField] private TextMeshProUGUI _graphicsTierText;

        [Header("Panels")]
        [SerializeField] private GameObject _tutorialPanel;
        [SerializeField] private GameObject _pauseMenuPanel;
        [SerializeField] private GameObject _crosshair;

        [Header("Player & Camera References")]
        [SerializeField] private SuperheroFlightController _playerFlight;
        [SerializeField] private SuperheroCameraController _cameraController;

        private void Start()
        {
            if (_playerFlight == null)
            {
                _playerFlight = FindFirstObjectByType<SuperheroFlightController>();
            }

            if (_cameraController == null)
            {
                _cameraController = FindFirstObjectByType<SuperheroCameraController>();
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
                GameManager.Instance.OnPauseToggled += SetPauseMenuVisibility;
            }

            if (GameplayConfig.Instance != null)
            {
                GameplayConfig.Instance.OnQualityTierChanged += UpdateGraphicsTierDisplay;
                UpdateGraphicsTierDisplay(GameplayConfig.Instance.CurrentQualityTier);
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
                GameManager.Instance.OnPauseToggled -= SetPauseMenuVisibility;
            }

            if (GameplayConfig.Instance != null)
            {
                GameplayConfig.Instance.OnQualityTierChanged -= UpdateGraphicsTierDisplay;
            }
        }

        private void Update()
        {
            UpdateTelemetry();
        }

        private void UpdateTelemetry()
        {
            if (_playerFlight != null)
            {
                // Speedometer
                if (_speedometerText != null)
                {
                    float speedKmh = _playerFlight.CurrentSpeed * 3.6f;
                    string modeTag = $"[{_playerFlight.CurrentLocomotionMode.ToString().ToUpper()}]";
                    string boostTag = _playerFlight.IsBoosting ? " <color=#FF3300>[SUPERSONIC]</color>" : "";
                    _speedometerText.text = $"{Mathf.RoundToInt(speedKmh)} KM/H {modeTag}{boostTag}";
                }

                // Ability State
                if (_abilityStateText != null)
                {
                    _abilityStateText.text = $"ABILITY: {_playerFlight.CurrentAbilityState.ToString().ToUpper()}";
                }
            }

            // Camera Perspective
            if (_cameraController != null && _cameraModeText != null)
            {
                _cameraModeText.text = $"CAM: {_cameraController.Perspective.ToString().ToUpper()} (V)";
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

        private void UpdateGraphicsTierDisplay(GraphicsQualityTier tier)
        {
            if (_graphicsTierText != null)
            {
                _graphicsTierText.text = $"QUALITY: {tier.ToString().ToUpper()}";
            }
        }

        private void HideTutorial()
        {
            if (_tutorialPanel != null)
            {
                _tutorialPanel.SetActive(false);
            }
        }

        private void SetPauseMenuVisibility(bool isPaused)
        {
            if (_pauseMenuPanel != null)
            {
                _pauseMenuPanel.SetActive(isPaused);
            }
        }

        private float _fpsAccumulator = 0f;
        private int _fpsFrames = 0;
        private float _fpsTimeLeft = 0.5f;
        private float _currentFps = 60f;

        private void OnGUI()
        {
            _fpsFrames++;
            _fpsAccumulator += Time.unscaledDeltaTime;
            if (_fpsAccumulator >= _fpsTimeLeft)
            {
                _currentFps = _fpsFrames / _fpsAccumulator;
                _fpsFrames = 0;
                _fpsAccumulator = 0f;
            }

            GUIStyle titleStyle = new GUIStyle(GUI.skin.box);
            titleStyle.fontSize = 14;
            titleStyle.fontStyle = FontStyle.Bold;
            titleStyle.normal.textColor = Color.yellow;
            titleStyle.alignment = TextAnchor.UpperLeft;

            GUIStyle hudStyle = new GUIStyle(GUI.skin.box);
            hudStyle.fontSize = 12;
            hudStyle.normal.textColor = Color.white;
            hudStyle.alignment = TextAnchor.UpperLeft;

            // HUD Top-Left Panel
            GUILayout.BeginArea(new Rect(15, 15, 340, 240), titleStyle);
            GUILayout.Label("CSVS GOD MODE — V0.1.2 PROTOTYPE", titleStyle);
            GUILayout.Space(5);

            float speed = _playerFlight != null ? _playerFlight.CurrentSpeed : 0f;
            string mode = _playerFlight != null ? _playerFlight.CurrentLocomotionMode.ToString().ToUpper() : "FLIGHT";
            string boost = (_playerFlight != null && _playerFlight.IsBoosting) ? " [SUPERSONIC]" : "";
            string ability = _playerFlight != null ? _playerFlight.CurrentAbilityState.ToString().ToUpper() : "IDLE";
            long chaos = ChaosSystem.Instance != null ? ChaosSystem.Instance.CurrentChaosScore : 0;
            string threat = ChaosSystem.Instance != null ? ChaosSystem.Instance.CurrentWantedTier.ToString().ToUpper() : "NONE";

            GUILayout.Label($"SPEED: {speed * 3.6f:F0} KM/H ({speed:F1} m/s) [{mode}]{boost}");
            GUILayout.Label($"CHAOS SCORE: ${chaos:N0}");
            GUILayout.Label($"THREAT TIER: {threat}");
            GUILayout.Label($"ACTIVE ABILITY: {ability}");
            GUILayout.Label($"FPS: {_currentFps:F0} | TARGET: 30-60 FPS");
            GUILayout.Space(5);
            GUILayout.Label("CONTROLS:", titleStyle);
            GUILayout.Label("WASD: Move | Shift: Supersonic Boost");
            GUILayout.Label("Space: Ascend | Ctrl: Descend");
            GUILayout.Label("L-Click: Punch/Laser | R-Click: Grab/Throw");
            GUILayout.Label("F: Ground Slam | V: Toggle Camera");
            GUILayout.EndArea();
        }
    }
}
