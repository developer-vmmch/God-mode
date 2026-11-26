using UnityEngine;
using UnityEngine.Rendering.Universal;
using CSVS.GodMode.Core;
using CSVS.GodMode.Destruction;

namespace CSVS.GodMode.Settings
{
    /// <summary>
    /// Graphics & hardware preset manager.
    /// Configured specifically for Intel Core i3-10105T / UHD 630 (8 GB RAM)
    /// with default LOW settings: 1280x720, render scale 0.75, 30 FPS target, 50 debris target.
    /// </summary>
    public class QualitySettingsManager : MonoBehaviour
    {
        public static QualitySettingsManager Instance { get; private set; }

        [SerializeField] private GraphicsQualityTier initialPreset = GraphicsQualityTier.Low;
        [SerializeField] private UniversalRenderPipelineAsset urpAsset;

        public GraphicsQualityTier CurrentPreset => GameplayConfig.Instance != null ? GameplayConfig.Instance.CurrentQualityTier : initialPreset;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            ApplyPreset(initialPreset);
        }

        public void ApplyPreset(GraphicsQualityTier preset)
        {
            if (GameplayConfig.Instance != null)
            {
                GameplayConfig.Instance.SetQualityTier(preset);
            }

            switch (preset)
            {
                case GraphicsQualityTier.Low:
                    // Optimized default for Intel UHD 630 / WebGL / Core i3-10105T
                    SetRenderScale(0.75f);
                    SetShadowDistance(35.0f);
                    QualitySettings.vSyncCount = 1;
                    QualitySettings.shadows = UnityEngine.ShadowQuality.HardOnly;
                    Application.targetFrameRate = 30;
                    if (DebrisPoolManager.Instance != null)
                    {
                        DebrisPoolManager.Instance.SetBudget(50);
                    }
                    break;

                case GraphicsQualityTier.Medium:
                    SetRenderScale(0.85f);
                    SetShadowDistance(60.0f);
                    QualitySettings.vSyncCount = 1;
                    QualitySettings.shadows = UnityEngine.ShadowQuality.All;
                    Application.targetFrameRate = 60;
                    if (DebrisPoolManager.Instance != null)
                    {
                        DebrisPoolManager.Instance.SetBudget(100);
                    }
                    break;

                case GraphicsQualityTier.High:
                    SetRenderScale(1.0f);
                    SetShadowDistance(100.0f);
                    QualitySettings.vSyncCount = 1;
                    QualitySettings.shadows = UnityEngine.ShadowQuality.All;
                    Application.targetFrameRate = 60;
                    if (DebrisPoolManager.Instance != null)
                    {
                        DebrisPoolManager.Instance.SetBudget(150);
                    }
                    break;
            }
        }

        public void CyclePreset()
        {
            GraphicsQualityTier next = (GraphicsQualityTier)(((int)CurrentPreset + 1) % 3);
            ApplyPreset(next);
        }

        private void SetRenderScale(float scale)
        {
            if (urpAsset != null)
            {
                urpAsset.renderScale = scale;
            }
        }

        private void SetShadowDistance(float distance)
        {
            if (urpAsset != null)
            {
                urpAsset.shadowDistance = distance;
            }
            QualitySettings.shadowDistance = distance;
        }
    }
}
