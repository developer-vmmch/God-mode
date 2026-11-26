using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CSVS.Core;
using CSVS.Controllers;
using CSVS.Superpowers;
using CSVS.Camera;
using CSVS.Settings;

namespace CSVS.UI
{
    /// <summary>
    /// Modern superhero HUD controller displaying flight telemetry,
    /// chaos progress, superpower heat levels, pause menu, and graphics settings.
    /// </summary>
    public class HUDController : MonoBehaviour
    {
        public static HUDController Instance { get; private set; }

        [Header("Telemetry UI")]
        [SerializeField] private TextMeshProUGUI speedText;
        [SerializeField] private TextMeshProUGUI altitudeText;
        [SerializeField] private TextMeshProUGUI chaosScoreText;
        [SerializeField] private TextMeshProUGUI wantedLevelText;
        [SerializeField] private Image heatVisionBar;
        [SerializeField] private GameObject crosshairObj;

        [Header("Tutorial Banner")]
        [SerializeField] private GameObject tutorialBanner;
        [SerializeField] private TextMeshProUGUI tutorialControlsText;

        [Header("Pause & Settings Menus")]
        [SerializeField] private GameObject pauseMenuPanel;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private TextMeshProUGUI fpsCounterText;

        [Header("References")]
        [SerializeField] private SuperheroController playerController;
        [SerializeField] private HeatVisionSystem heatVision;

        private float fpsTimer = 0f;
        private int frameCount = 0;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            if (playerController == null)
            {
                playerController = FindFirstObjectByType<SuperheroController>();
            }

            if (heatVision == null && playerController != null)
            {
                heatVision = playerController.GetComponent<HeatVisionSystem>();
            }

            if (tutorialControlsText != null)
            {
                tutorialControlsText.text = "<b>W/A/S/D:</b> Fly  |  <b>Space:</b> Ascend  |  <b>Shift:</b> Supersonic Boost / Slam\n" +
                                            "<b>LMB/RMB:</b> Heat Vision  |  <b>E:</b> Grab / Throw / Crush  |  <b>Q:</b> Gravity Tether  |  <b>F:</b> Super Punch\n" +
                                            "<b>V:</b> 1st/3rd Person  |  <b>P:</b> Pause  |  <b>T:</b> Hide Tutorial";
            }

            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(false);

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
            }

            if (ChaosManager.Instance != null)
            {
                ChaosManager.Instance.OnScoreChanged += UpdateScoreDisplay;
                ChaosManager.Instance.OnWantedLevelChanged += UpdateWantedDisplay;
            }
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
            }
            if (ChaosManager.Instance != null)
            {
                ChaosManager.Instance.OnScoreChanged -= UpdateScoreDisplay;
                ChaosManager.Instance.OnWantedLevelChanged -= UpdateWantedDisplay;
            }
        }

        private void Update()
        {
            UpdateTelemetry();
            UpdateFPS();

            if (UnityEngine.Input.GetKeyDown(KeyCode.T) && tutorialBanner != null)
            {
                tutorialBanner.SetActive(!tutorialBanner.activeSelf);
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.P) || UnityEngine.Input.GetKeyDown(KeyCode.Escape))
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.TogglePause();
                }
            }
        }

        private void UpdateTelemetry()
        {
            if (playerController != null)
            {
                float speedKmh = playerController.Speed * 3.6f;
                if (speedText != null)
                {
                    speedText.text = $"{speedKmh:F0} <size=60%>KM/H</size>";
                    speedText.color = playerController.IsSupersonic ? Color.cyan : Color.white;
                }

                if (altitudeText != null)
                {
                    float altitude = Mathf.Max(0f, playerController.transform.position.y);
                    altitudeText.text = $"ALT: {altitude:F1}M";
                }
            }

            if (heatVision != null && heatVisionBar != null)
            {
                heatVisionBar.fillAmount = heatVision.CurrentHeat / heatVision.MaxHeat;
                heatVisionBar.color = heatVision.IsOverheated ? Color.red : Color.Lerp(Color.yellow, Color.red, heatVisionBar.fillAmount);
            }
        }

        private void UpdateFPS()
        {
            frameCount++;
            fpsTimer += Time.unscaledDeltaTime;
            if (fpsTimer >= 0.5f)
            {
                float fps = frameCount / fpsTimer;
                if (fpsCounterText != null)
                {
                    fpsCounterText.text = $"FPS: {fps:F0}";
                }
                frameCount = 0;
                fpsTimer = 0f;
            }
        }

        private void UpdateScoreDisplay(long score)
        {
            if (chaosScoreText != null)
            {
                chaosScoreText.text = $"CHAOS: ${score:N0}";
            }
        }

        private void UpdateWantedDisplay(int level)
        {
            if (wantedLevelText != null)
            {
                string stars = new string('★', level) + new string('☆', 6 - level);
                wantedLevelText.text = $"THREAT LEVEL: {stars}";
                wantedLevelText.color = level > 3 ? Color.red : Color.yellow;
            }
        }

        private void HandleGameStateChanged(GameState state)
        {
            if (pauseMenuPanel != null)
            {
                pauseMenuPanel.SetActive(state == GameState.Paused);
            }
        }

        public void OnResumeClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetState(GameState.Playing);
            }
        }

        public void OnResetClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ResetScene();
            }
        }

        public void OnSettingsClicked()
        {
            if (settingsPanel != null)
            {
                settingsPanel.SetActive(!settingsPanel.activeSelf);
            }
        }
    }
}
