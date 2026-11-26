using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace CSVS.GodMode.Core
{
    /// <summary>
    /// Master game manager handling pause/resume, tutorial gating, and instant scene reset.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public bool IsPaused { get; private set; }
        public bool IsTutorialActive { get; private set; } = true;

        public event Action<bool> OnPauseToggled;
        public event Action OnTutorialSkipped;
        public event Action OnSceneResetTriggered;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            if (Keyboard.current == null) return;

            // P or Escape key toggles Pause
            if (GameInputManager.WasPauseTriggered)
            {
                TogglePause();
            }

            // T key skips Tutorial
            if (Keyboard.current != null && Keyboard.current.tKey.wasPressedThisFrame && IsTutorialActive)
            {
                SkipTutorial();
            }

            // R key triggers Scene Reset
            if (GameInputManager.WasResetTriggered)
            {
                ResetCurrentScene();
            }
        }

        public void TogglePause()
        {
            IsPaused = !IsPaused;
            Time.timeScale = IsPaused ? 0.0f : 1.0f;
            Cursor.lockState = IsPaused ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = IsPaused;
            OnPauseToggled?.Invoke(IsPaused);
        }

        public void SkipTutorial()
        {
            IsTutorialActive = false;
            OnTutorialSkipped?.Invoke();
        }

        public void ResetCurrentScene()
        {
            Time.timeScale = 1.0f;
            IsPaused = false;

            if (ChaosSystem.Instance != null)
            {
                ChaosSystem.Instance.ResetScore();
            }

            OnSceneResetTriggered?.Invoke();
            Scene activeScene = SceneManager.GetActiveScene();
            if (activeScene.isLoaded)
            {
                SceneManager.LoadScene(activeScene.buildIndex);
            }
        }
    }
}
