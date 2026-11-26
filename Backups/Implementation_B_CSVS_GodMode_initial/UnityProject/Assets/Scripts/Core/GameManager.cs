using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CSVS.GodMode.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public bool IsPaused { get; private set; }
        public bool IsTutorialActive { get; private set; } = true;

        public event Action<bool> OnPauseToggled;
        public event Action OnTutorialSkipped;

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
            // P key toggles Pause
            if (Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame)
            {
                TogglePause();
            }

            // T key skips Tutorial
            if (Keyboard.current != null && Keyboard.current.tKey.wasPressedThisFrame && IsTutorialActive)
            {
                SkipTutorial();
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
    }
}
