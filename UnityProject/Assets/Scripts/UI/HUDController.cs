using System;
using UnityEngine;
using TMPro;
using CSVS.GodMode.Core;
using CSVS.GodMode.Player;
using CSVS.GodMode.Settings;
using CSVS.GodMode.Environment;

namespace CSVS.GodMode.UI
{
    /// <summary>
    /// Consolidated GTA-inspired HUD and Karaikal Open-World Telemetry controller.
    /// Renders real-time flight telemetry, ability states, chaos score, wanted rating,
    /// dynamic Karaikal GPS Minimap, district notifications, and full-screen city map overlay.
    /// </summary>
    public class HUDController : MonoBehaviour
    {
        [Header("UI Text Displays (Optional UGUI)")]
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

        private float _fpsAccumulator = 0f;
        private int _fpsFrames = 0;
        private float _fpsTimeLeft = 0.5f;
        private float _currentFps = 60f;
        private bool _showFullMap = false;

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

            if (_playerFlight == null)
            {
                _playerFlight = FindFirstObjectByType<SuperheroFlightController>();
            }

            // Toggle Full-Screen Karaikal Map with M key
            if (Input.GetKeyDown(KeyCode.M))
            {
                _showFullMap = !_showFullMap;
            }
        }

        private void UpdateTelemetry()
        {
            if (_playerFlight != null)
            {
                if (_speedometerText != null)
                {
                    float speedKmh = _playerFlight.CurrentSpeed * 3.6f;
                    string modeTag = $"[{_playerFlight.CurrentLocomotionMode.ToString().ToUpper()}]";
                    string boostTag = _playerFlight.IsBoosting ? " <color=#FF3300>[SUPERSONIC]</color>" : "";
                    _speedometerText.text = $"{Mathf.RoundToInt(speedKmh)} KM/H {modeTag}{boostTag}";
                }

                if (_abilityStateText != null)
                {
                    _abilityStateText.text = $"ABILITY: {_playerFlight.CurrentAbilityState.ToString().ToUpper()}";
                }
            }

            if (_cameraController != null && _cameraModeText != null)
            {
                _cameraModeText.text = $"CAMERA: {_cameraController.Perspective.ToString().ToUpper()}";
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
            if (_tutorialPanel != null) _tutorialPanel.SetActive(false);
        }

        private void SetPauseMenuVisibility(bool isPaused)
        {
            if (_pauseMenuPanel != null) _pauseMenuPanel.SetActive(isPaused);
        }

        private void OnGUI()
        {
            // FPS measurement
            _fpsFrames++;
            _fpsAccumulator += Time.unscaledDeltaTime;
            if (_fpsAccumulator >= _fpsTimeLeft)
            {
                _currentFps = _fpsFrames / _fpsAccumulator;
                _fpsFrames = 0;
                _fpsAccumulator = 0f;
            }

            Vector3 playerPos = _playerFlight != null ? _playerFlight.transform.position : Vector3.zero;
            float playerYaw = _playerFlight != null ? _playerFlight.transform.eulerAngles.y : 0f;
            KaraikalDistrictType currentDistrict = KaraikalGeoData.GetDistrictAtPosition(playerPos);
            string districtLabel = KaraikalGeoData.GetDistrictDisplayName(currentDistrict);

            // 1. Top-Left Telemetry Box
            DrawTelemetryPanel(playerPos, districtLabel);

            // 2. Bottom-Left GTA-Style Karaikal Minimap
            DrawMinimap(playerPos, playerYaw, districtLabel);

            // 3. Full-Screen Map Overlay (when M key is pressed)
            if (_showFullMap)
            {
                DrawFullScreenMap(playerPos, currentDistrict);
            }
        }

        private void DrawTelemetryPanel(Vector3 playerPos, string districtLabel)
        {
            GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
            boxStyle.fontSize = 12;
            boxStyle.alignment = TextAnchor.UpperLeft;

            GUIStyle titleStyle = new GUIStyle(GUI.skin.box);
            titleStyle.fontSize = 13;
            titleStyle.fontStyle = FontStyle.Bold;
            titleStyle.normal.textColor = Color.yellow;
            titleStyle.alignment = TextAnchor.UpperLeft;

            GUILayout.BeginArea(new Rect(15, 15, 360, 240), boxStyle);
            GUILayout.Label("CSVS GOD MODE — KARAIKAL OPEN WORLD", titleStyle);
            GUILayout.Space(4);

            float speed = _playerFlight != null ? _playerFlight.CurrentSpeed : 0f;
            string mode = _playerFlight != null ? _playerFlight.CurrentLocomotionMode.ToString().ToUpper() : "FLIGHT";
            string boost = (_playerFlight != null && _playerFlight.IsBoosting) ? " [SUPERSONIC]" : "";
            string ability = _playerFlight != null ? _playerFlight.CurrentAbilityState.ToString().ToUpper() : "IDLE";
            long chaos = ChaosSystem.Instance != null ? ChaosSystem.Instance.CurrentChaosScore : 0;
            string threat = ChaosSystem.Instance != null ? ChaosSystem.Instance.CurrentWantedTier.ToString().ToUpper() : "NONE";

            GUILayout.Label($"LOCATION: {districtLabel}");
            GUILayout.Label($"SPEED: {speed * 3.6f:F0} KM/H ({speed:F1} m/s) [{mode}]{boost}");
            GUILayout.Label($"CHAOS SCORE: ${chaos:N0} | WANTED: {threat}");
            GUILayout.Label($"ACTIVE ABILITY: {ability} | FPS: {_currentFps:F0}");
            GUILayout.Label($"ALTITUDE: {playerPos.y:F1} m | HEADING: {GetCompassHeading(playerFlightYaw())}");
            GUILayout.Space(4);
            GUILayout.Label("CONTROLS:", titleStyle);
            GUILayout.Label("WASD: Move | Shift: Supersonic Boost | Space/Ctrl: Ascend/Descend");
            GUILayout.Label("L-Click: Punch/Laser | R-Click: Grab/Throw | F: Slam | V: 1P/3P | M: Map");
            GUILayout.EndArea();
        }

        private void DrawMinimap(Vector3 playerPos, float playerYaw, string districtLabel)
        {
            float mmWidth = 220f;
            float mmHeight = 180f;
            float mmX = 15f;
            float mmY = Screen.height - mmHeight - 15f;

            GUIStyle mmBox = new GUIStyle(GUI.skin.box);
            mmBox.normal.textColor = Color.cyan;
            mmBox.fontSize = 11;
            mmBox.fontStyle = FontStyle.Bold;

            GUI.Box(new Rect(mmX, mmY, mmWidth, mmHeight), "KARAIKAL GPS RADAR", mmBox);

            // Radar inner display rect
            Rect radarRect = new Rect(mmX + 10, mmY + 25, mmWidth - 20, mmHeight - 65);
            GUI.Box(radarRect, "");

            // Radar Scale: 1 pixel = 8 meters (range +- 800m)
            Vector2 radarCenter = new Vector2(radarRect.x + radarRect.width * 0.5f, radarRect.y + radarRect.height * 0.5f);
            float scale = 0.12f;

            // Draw Coastline Indicator (East)
            float coastScreenX = radarCenter.x + (650f - playerPos.x) * scale;
            if (coastScreenX >= radarRect.x && coastScreenX <= radarRect.xMax)
            {
                GUI.color = new Color(0.2f, 0.6f, 1.0f, 0.8f);
                GUI.DrawTexture(new Rect(coastScreenX, radarRect.y, 3, radarRect.height), Texture2D.whiteTexture);
            }

            // Draw Arasalar River Indicator (South)
            float riverScreenY = radarCenter.y - (-380f - playerPos.z) * scale;
            if (riverScreenY >= radarRect.y && riverScreenY <= radarRect.yMax)
            {
                GUI.color = new Color(0.15f, 0.45f, 0.85f, 0.8f);
                GUI.DrawTexture(new Rect(radarRect.x, riverScreenY, radarRect.width, 3), Texture2D.whiteTexture);
            }

            // Draw Kamarajar Salai (NH 32 North-South)
            float nhScreenX = radarCenter.x + (0f - playerPos.x) * scale;
            if (nhScreenX >= radarRect.x && nhScreenX <= radarRect.xMax)
            {
                GUI.color = new Color(0.9f, 0.8f, 0.2f, 0.7f);
                GUI.DrawTexture(new Rect(nhScreenX, radarRect.y, 2, radarRect.height), Texture2D.whiteTexture);
            }

            // Draw Bharathiyar Road (East-West)
            float bharathiScreenY = radarCenter.y - (0f - playerPos.z) * scale;
            if (bharathiScreenY >= radarRect.y && bharathiScreenY <= radarRect.yMax)
            {
                GUI.color = new Color(0.9f, 0.8f, 0.2f, 0.7f);
                GUI.DrawTexture(new Rect(radarRect.x, bharathiScreenY, radarRect.width, 2), Texture2D.whiteTexture);
            }

            // Draw Player Marker in Center
            GUI.color = Color.red;
            GUI.DrawTexture(new Rect(radarCenter.x - 4, radarCenter.y - 4, 8, 8), Texture2D.whiteTexture);
            GUI.color = Color.white;

            // District & Coordinate Label
            double currentLat = KaraikalGeoData.OriginLat + (playerPos.z / KaraikalGeoData.MetersPerDegreeLat);
            double currentLon = KaraikalGeoData.OriginLon + (playerPos.x / KaraikalGeoData.MetersPerDegreeLon);

            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.fontSize = 10;
            labelStyle.normal.textColor = Color.white;

            GUI.Label(new Rect(mmX + 10, mmY + mmHeight - 38, mmWidth - 20, 18), $"GPS: {currentLat:F4}°N, {currentLon:F4}°E", labelStyle);
            GUI.Label(new Rect(mmX + 10, mmY + mmHeight - 20, mmWidth - 20, 18), $"[M] FULL-SCREEN MAP", labelStyle);
        }

        private void DrawFullScreenMap(Vector3 playerPos, KaraikalDistrictType currentDistrict)
        {
            float mapW = Mathf.Min(Screen.width * 0.85f, 750f);
            float mapH = Mathf.Min(Screen.height * 0.85f, 550f);
            float mapX = (Screen.width - mapW) * 0.5f;
            float mapY = (Screen.height - mapH) * 0.5f;

            GUIStyle mapBox = new GUIStyle(GUI.skin.box);
            mapBox.fontSize = 14;
            mapBox.fontStyle = FontStyle.Bold;
            mapBox.normal.textColor = Color.yellow;

            GUI.Box(new Rect(mapX, mapY, mapW, mapH), "KARAIKAL DISTRICT MAP — PUDUCHERRY, INDIA", mapBox);

            Rect mapInner = new Rect(mapX + 20, mapY + 35, mapW - 40, mapH - 60);
            GUI.Box(mapInner, "");

            Vector2 center = new Vector2(mapInner.x + mapInner.width * 0.5f, mapInner.y + mapInner.height * 0.5f);
            float scale = 0.22f;

            // Draw Bay of Bengal Coastline & Sea (East)
            float coastX = center.x + 650f * scale;
            GUI.color = new Color(0.12f, 0.40f, 0.75f, 0.5f);
            GUI.DrawTexture(new Rect(coastX, mapInner.y, mapInner.xMax - coastX, mapInner.height), Texture2D.whiteTexture);

            // Draw Arasalar River (South)
            float riverY = center.y - (-380f) * scale;
            GUI.color = new Color(0.15f, 0.50f, 0.85f, 0.8f);
            GUI.DrawTexture(new Rect(mapInner.x, riverY, mapInner.width, 14), Texture2D.whiteTexture);

            // Draw Kamarajar Salai (NH 32)
            GUI.color = new Color(0.95f, 0.85f, 0.20f, 0.9f);
            GUI.DrawTexture(new Rect(center.x - 3, mapInner.y, 6, mapInner.height), Texture2D.whiteTexture);

            // Draw Bharathiyar Road
            GUI.DrawTexture(new Rect(mapInner.x, center.y - 3, mapInner.width, 6), Texture2D.whiteTexture);

            // Draw Landmark Points
            GUIStyle pinStyle = new GUIStyle(GUI.skin.label);
            pinStyle.fontSize = 10;
            pinStyle.fontStyle = FontStyle.Bold;
            pinStyle.normal.textColor = Color.white;

            foreach (var lm in KaraikalGeoData.KeyLandmarks)
            {
                float px = center.x + lm.LocalPosition.x * scale;
                float py = center.y - lm.LocalPosition.z * scale;

                if (px >= mapInner.x && px <= mapInner.xMax && py >= mapInner.y && py <= mapInner.yMax)
                {
                    GUI.color = Color.magenta;
                    GUI.DrawTexture(new Rect(px - 4, py - 4, 8, 8), Texture2D.whiteTexture);
                    GUI.color = Color.white;
                    GUI.Label(new Rect(px + 6, py - 8, 180, 20), lm.NameEnglish, pinStyle);
                }
            }

            // Draw Player Position Pin
            float playerPinX = center.x + playerPos.x * scale;
            float playerPinY = center.y - playerPos.z * scale;
            GUI.color = Color.green;
            GUI.DrawTexture(new Rect(playerPinX - 6, playerPinY - 6, 12, 12), Texture2D.whiteTexture);
            GUI.color = Color.white;
            GUI.Label(new Rect(playerPinX + 8, playerPinY - 8, 120, 20), "YOU (HERO)", pinStyle);
        }

        private float playerFlightYaw()
        {
            return _playerFlight != null ? _playerFlight.transform.eulerAngles.y : 0f;
        }

        private string GetCompassHeading(float yaw)
        {
            float norm = (yaw % 360f + 360f) % 360f;
            if (norm >= 337.5f || norm < 22.5f) return "N";
            if (norm >= 22.5f && norm < 67.5f) return "NE";
            if (norm >= 67.5f && norm < 112.5f) return "E";
            if (norm >= 112.5f && norm < 157.5f) return "SE";
            if (norm >= 157.5f && norm < 202.5f) return "S";
            if (norm >= 202.5f && norm < 247.5f) return "SW";
            if (norm >= 247.5f && norm < 292.5f) return "W";
            return "NW";
        }
    }
}
