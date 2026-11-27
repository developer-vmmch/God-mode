// V030HUDController.cs
// CSVS GOD MODE — V0.3 Professional Game HUD
// Original dark-glass UI with navy/silver/cyan design language.
// NO Rockstar Games UI assets or code reproduced.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CSVS.GodMode.Core;
using CSVS.GodMode.Environment;
using CSVS.GodMode.Player;

namespace CSVS.GodMode.UI
{
    public class V030HUDController : MonoBehaviour
    {
        // ─── Inspector References (assigned in Unity Editor) ──────────────
        [Header("Health & Energy")]
        [SerializeField] private Slider healthBar;
        [SerializeField] private Slider energyBar;
        [SerializeField] private Image  healthBarFill;
        [SerializeField] private Image  energyBarFill;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI energyText;

        [Header("Speed & Altitude")]
        [SerializeField] private TextMeshProUGUI speedText;      // km/h
        [SerializeField] private TextMeshProUGUI altitudeText;   // metres

        [Header("Reputation & Chaos")]
        [SerializeField] private TextMeshProUGUI reputationText;
        [SerializeField] private TextMeshProUGUI chaosText;
        [SerializeField] private List<Image> chaosStars;         // 5 star images

        [Header("Minimap")]
        [SerializeField] private RawImage minimapImage;
        [SerializeField] private Transform minimapPlayerIcon;
        [SerializeField] private Camera    minimapCamera;
        [SerializeField] private TextMeshProUGUI districtLabel;

        [Header("Mission")]
        [SerializeField] private GameObject missionPanel;
        [SerializeField] private TextMeshProUGUI missionTitle;
        [SerializeField] private TextMeshProUGUI objectiveText;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private Slider objectiveProgress;
        [SerializeField] private GameObject missionCompletePanel;
        [SerializeField] private TextMeshProUGUI missionCompleteText;
        [SerializeField] private TextMeshProUGUI repRewardText;

        [Header("Active Power Indicator")]
        [SerializeField] private TextMeshProUGUI activePowerText;
        [SerializeField] private Image activePowerIcon;

        [Header("Control Hints")]
        [SerializeField] private TextMeshProUGUI controlHintsText;
        [SerializeField] private GameObject controlHintsPanel;

        [Header("Mode Indicator")]
        [SerializeField] private TextMeshProUGUI playerModeText;   // "HUMAN" / "SUPERHERO"
        [SerializeField] private Image modeIndicatorBG;

        [Header("Time & Weather")]
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI weatherText;

        // ─── Colour Constants (original CSVS design language) ─────────────
        private static readonly Color ColorHealthFull    = new Color(0.15f, 0.80f, 0.35f);
        private static readonly Color ColorHealthLow     = new Color(0.90f, 0.20f, 0.15f);
        private static readonly Color ColorEnergy        = new Color(0.00f, 0.85f, 1.00f);  // Cyan
        private static readonly Color ColorHumanMode     = new Color(0.60f, 0.65f, 0.70f);
        private static readonly Color ColorSuperheroMode = new Color(0.00f, 0.85f, 1.00f);

        // ─── State ─────────────────────────────────────────────────────────
        private HumanoidCharacterController _player;
        private ReputationChaosSystem _repSys;
        private DayNightCycleSystem _dayNight;
        private bool _minimapExpanded;

        // Control hint strings
        private const string HUMAN_HINTS     = "WASD: Move   Shift: Sprint   Space: Jump\nG: Enter Vehicle   T: Transform   M: Map";
        private const string SUPERHERO_HINTS = "WASD: Fly   Space: Boost   Q: Tether   E: Grab\nF: Super Punch   LMB: Heat Vision   G: Vehicle   T: Transform";

        // ─── Lifecycle ─────────────────────────────────────────────────────
        private void Start()
        {
            _player   = FindObjectOfType<HumanoidCharacterController>();
            _repSys   = FindObjectOfType<ReputationChaosSystem>();
            _dayNight = FindObjectOfType<DayNightCycleSystem>();

            if (_repSys)
            {
                _repSys.OnChaosLevelChanged  += UpdateChaosStars;
                _repSys.OnReputationChanged  += UpdateReputation;
            }

            HideMissionPanel();
            HideMissionComplete();
            UpdateControlHints(PlayerMode.Human);
        }

        private void Update()
        {
            UpdateHealthEnergy();
            UpdateSpeedAltitude();
            UpdateTimeWeather();
            UpdateModeIndicator();
            UpdateMinimap();

            if (GameInputManager.WasMapToggleTriggered)
                ToggleMinimap();
        }

        // ─── Health / Energy ───────────────────────────────────────────────
        private void UpdateHealthEnergy()
        {
            if (!_player) return;

            float hp = _player.Health / (float)_player.MaxHealth;
            if (healthBar) healthBar.value = hp;
            if (healthBarFill)
                healthBarFill.color = Color.Lerp(ColorHealthLow, ColorHealthFull, hp);
            if (healthText)
                healthText.text = $"{_player.Health}/{_player.MaxHealth}";

            float en = _player.Energy / _player.MaxEnergy;
            if (energyBar) energyBar.value = en;
            if (energyBarFill) energyBarFill.color = ColorEnergy;
            if (energyText)
                energyText.text = $"{Mathf.RoundToInt(_player.Energy)}/{Mathf.RoundToInt(_player.MaxEnergy)}";
        }

        // ─── Speed / Altitude ──────────────────────────────────────────────
        private void UpdateSpeedAltitude()
        {
            if (!_player) return;

            float speedMS = _player.CurrentSpeed;
            float speedKMH = speedMS * 3.6f;
            if (speedText) speedText.text = $"{speedKMH:F0} km/h";

            float altitude = _player.transform.position.y;
            if (altitudeText) altitudeText.text = $"{altitude:F1} m";
        }

        // ─── Reputation & Chaos Stars ──────────────────────────────────────
        private void UpdateChaosStars(int chaosLevel)
        {
            for (int i = 0; i < chaosStars.Count; i++)
            {
                if (chaosStars[i])
                    chaosStars[i].color = i < chaosLevel
                        ? new Color(1f, 0.65f, 0f) : new Color(0.3f, 0.3f, 0.3f);
            }
            if (chaosText) chaosText.text = $"★ x{chaosLevel}";
        }

        private void UpdateReputation(int rep)
        {
            if (reputationText) reputationText.text = $"REP: {rep}";
        }

        // ─── Time & Weather ────────────────────────────────────────────────
        private void UpdateTimeWeather()
        {
            if (!_dayNight) return;
            float h = _dayNight.CurrentHour;
            int hours   = Mathf.FloorToInt(h);
            int minutes = Mathf.FloorToInt((h - hours) * 60f);
            if (timeText)
                timeText.text = $"{hours:D2}:{minutes:D2}";

            if (weatherText)
                weatherText.text = _dayNight.TimeOfDay.ToString().ToUpper();
        }

        // ─── Mode Indicator ────────────────────────────────────────────────
        private void UpdateModeIndicator()
        {
            if (!_player) return;
            bool isSuper = _player.CurrentMode == PlayerMode.Superhero;
            if (playerModeText)
            {
                playerModeText.text  = isSuper ? "SUPERHERO MODE" : "HUMAN MODE";
                playerModeText.color = isSuper ? ColorSuperheroMode : ColorHumanMode;
            }
            if (modeIndicatorBG)
                modeIndicatorBG.color = isSuper
                    ? new Color(0f, 0.2f, 0.35f, 0.75f)
                    : new Color(0.15f, 0.15f, 0.18f, 0.75f);
        }

        // ─── Minimap ───────────────────────────────────────────────────────
        private void UpdateMinimap()
        {
            if (!_player || !minimapCamera) return;

            // Top-down minimap camera follows player
            minimapCamera.transform.position = new Vector3(
                _player.transform.position.x,
                _player.transform.position.y + 120f,
                _player.transform.position.z
            );

            // Rotate player icon on minimap
            if (minimapPlayerIcon)
                minimapPlayerIcon.rotation = Quaternion.Euler(0, 0,
                    -_player.transform.eulerAngles.y);

            // Update district label from nearest landmark
            UpdateDistrictLabel();
        }

        private void UpdateDistrictLabel()
        {
            if (!_player || districtLabel == null) return;
            Vector3 pos = _player.transform.position;
            float nearest = float.MaxValue;
            string nearestName = "Karaikal";
            foreach (var lm in KaraikalOSMRoadData.Landmarks)
            {
                float d = Vector3.Distance(pos, lm.Position);
                if (d < nearest)
                {
                    nearest = d;
                    nearestName = lm.Name;
                }
            }
            if (nearest < 300f) districtLabel.text = nearestName;
        }

        private void ToggleMinimap()
        {
            _minimapExpanded = !_minimapExpanded;
            if (minimapImage)
            {
                var rt = minimapImage.rectTransform;
                rt.sizeDelta = _minimapExpanded ? new Vector2(600f, 600f) : new Vector2(200f, 200f);
            }
        }

        // ─── Mission HUD ───────────────────────────────────────────────────
        public void ShowMissionStart(string title, string briefing)
        {
            if (missionPanel) missionPanel.SetActive(true);
            if (missionTitle) missionTitle.text = title.ToUpper();
            if (objectiveText) objectiveText.text = briefing;
        }

        public void UpdateObjective(string desc)
        {
            if (objectiveText) objectiveText.text = desc;
        }

        public void UpdateObjectiveProgress(string desc, int progress, int max)
        {
            if (objectiveText) objectiveText.text = $"{desc} ({progress}/{max})";
            if (objectiveProgress) objectiveProgress.value = progress / (float)max;
        }

        public void UpdateTimer(float remaining)
        {
            if (timerText) timerText.text = $"{remaining:F1}s";
        }

        public void ShowMissionComplete(string text, int repReward)
        {
            HideMissionPanel();
            if (missionCompletePanel) missionCompletePanel.SetActive(true);
            if (missionCompleteText) missionCompleteText.text = text;
            if (repRewardText) repRewardText.text = $"+{repReward} REPUTATION";
            StartCoroutine(HideMissionCompleteAfterDelay(5f));
        }

        public void ShowMissionFailed()
        {
            HideMissionPanel();
            if (missionCompleteText) missionCompleteText.text = "MISSION FAILED";
            if (missionCompletePanel) missionCompletePanel.SetActive(true);
            StartCoroutine(HideMissionCompleteAfterDelay(4f));
        }

        private IEnumerator HideMissionCompleteAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            HideMissionComplete();
        }

        private void HideMissionPanel()
        {
            if (missionPanel) missionPanel.SetActive(false);
        }

        private void HideMissionComplete()
        {
            if (missionCompletePanel) missionCompletePanel.SetActive(false);
        }

        // ─── Control Hints ─────────────────────────────────────────────────
        public void UpdateControlHints(PlayerMode mode)
        {
            if (controlHintsText)
                controlHintsText.text = mode == PlayerMode.Superhero ? SUPERHERO_HINTS : HUMAN_HINTS;
        }

        // ─── Active Power Indicator ────────────────────────────────────────
        public void SetActivePower(string powerName)
        {
            if (activePowerText) activePowerText.text = powerName.ToUpper();
        }
    }
}

