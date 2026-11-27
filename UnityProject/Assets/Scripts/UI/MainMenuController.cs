// MainMenuController.cs
// CSVS GOD MODE — V0.3 Original Main Menu
// Design: Dark glass panel, CSVS IT Tech studio branding,
// animated title, navigation to game modes.
// NO Rockstar Games UI assets, fonts or branding reproduced.

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace CSVS.GodMode.UI
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("CSVS IT Tech Branding")]
        [SerializeField] private TextMeshProUGUI studioLabel;    // "CSVS IT Tech"
        [SerializeField] private TextMeshProUGUI gameTitleText;  // "CSVS GOD MODE"
        [SerializeField] private TextMeshProUGUI subtitleText;   // "CITY OF CHAOS — KARAIKAL"
        [SerializeField] private TextMeshProUGUI versionText;    // "V0.3.0 Playable Alpha"

        [Header("Menu Buttons")]
        [SerializeField] private Button btnNewGame;
        [SerializeField] private Button btnContinue;
        [SerializeField] private Button btnFreeRoam;
        [SerializeField] private Button btnSettings;
        [SerializeField] private Button btnControls;
        [SerializeField] private Button btnExit;

        [Header("Panels")]
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject controlsPanel;

        [Header("Title Animation")]
        [SerializeField] private float titlePulseSpeed = 1.2f;
        [SerializeField] private Color titleColorA = new Color(0f, 0.90f, 1f);   // Cyan
        [SerializeField] private Color titleColorB = new Color(0.75f, 0.80f, 0.90f);  // Silver

        [Header("Scene Names")]
        [SerializeField] private string gameSceneName = "MainGame";

        // ─── Lifecycle ─────────────────────────────────────────────────────
        private void Awake()
        {
            // Unlock cursor for menu
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible   = true;
        }

        private void Start()
        {
            SetupBranding();
            SetupButtonListeners();
            StartCoroutine(AnimateTitlePulse());
        }

        // ─── Branding ──────────────────────────────────────────────────────
        private void SetupBranding()
        {
            if (studioLabel)   studioLabel.text   = "CSVS IT TECH";
            if (gameTitleText) gameTitleText.text  = "CSVS GOD MODE";
            if (subtitleText)  subtitleText.text   = "CITY OF CHAOS  ·  KARAIKAL";
            if (versionText)   versionText.text    = "V0.3.0 PLAYABLE ALPHA";
        }

        // ─── Buttons ───────────────────────────────────────────────────────
        private void SetupButtonListeners()
        {
            btnNewGame?.onClick.AddListener(OnNewGame);
            btnContinue?.onClick.AddListener(OnContinue);
            btnFreeRoam?.onClick.AddListener(OnFreeRoam);
            btnSettings?.onClick.AddListener(OnSettings);
            btnControls?.onClick.AddListener(OnControls);
            btnExit?.onClick.AddListener(OnExit);

            // Disable Continue if no save exists
            if (btnContinue)
                btnContinue.interactable = PlayerPrefs.HasKey("Reputation");
        }

        // ─── Button Actions ────────────────────────────────────────────────
        private void OnNewGame()
        {
            PlayerPrefs.DeleteAll();   // Fresh start
            PlayerPrefs.SetInt("FreeRoam", 0);
            PlayerPrefs.Save();
            LoadGameScene();
        }

        private void OnContinue()
        {
            PlayerPrefs.SetInt("FreeRoam", 0);
            LoadGameScene();
        }

        private void OnFreeRoam()
        {
            PlayerPrefs.SetInt("FreeRoam", 1);
            LoadGameScene();
        }

        private void OnSettings()
        {
            settingsPanel?.SetActive(!(settingsPanel?.activeSelf ?? false));
            controlsPanel?.SetActive(false);
        }

        private void OnControls()
        {
            controlsPanel?.SetActive(!(controlsPanel?.activeSelf ?? false));
            settingsPanel?.SetActive(false);
        }

        private void OnExit()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        private void LoadGameScene()
        {
            SceneManager.LoadScene(gameSceneName);
        }

        // ─── Title Animation ───────────────────────────────────────────────
        private IEnumerator AnimateTitlePulse()
        {
            if (!gameTitleText) yield break;
            while (true)
            {
                float t = (Mathf.Sin(Time.unscaledTime * titlePulseSpeed) + 1f) * 0.5f;
                gameTitleText.color = Color.Lerp(titleColorA, titleColorB, t);
                yield return null;
            }
        }
    }
}

