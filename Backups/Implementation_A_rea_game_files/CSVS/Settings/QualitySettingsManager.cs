using UnityEngine;
using UnityEngine.Rendering.Universal;
using CSVS.Destruction;

namespace CSVS.Settings
{
    public enum QualityPreset
    {
        Low,
        Medium,
        High,
        Ultra
    }

    /// <summary>
    /// Configures graphics presets, dynamic render scale, shadow distances,
    /// and active debris budgets aligned with hardware capabilities (e.g. Ryzen 7430U / WebGL).
    /// </summary>
    public class QualitySettingsManager : MonoBehaviour
    {
        public static QualitySettingsManager Instance { get; private set; }

        [SerializeField] private QualityPreset currentPreset = QualityPreset.Medium;
        [SerializeField] private UniversalRenderPipelineAsset urpAsset;

        public QualityPreset CurrentPreset => currentPreset;

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
            ApplyPreset(currentPreset);
        }

        public void ApplyPreset(QualityPreset preset)
        {
            currentPreset = preset;

            switch (preset)
            {
                case QualityPreset.Low:
                    // Optimized for WebGL 2.0 and low-tier integrated GPUs
                    SetRenderScale(0.75f);
                    SetShadowDistance(35f);
                    if (DebrisManager.Instance != null) DebrisManager.Instance.SetMaxActiveDebris(80);
                    QualitySettings.vSyncCount = 1;
                    QualitySettings.shadows = ShadowQuality.HardOnly;
                    break;

                case QualityPreset.Medium:
                    // Baseline recommended target for Ryzen 5 7430U (60 FPS)
                    SetRenderScale(0.85f);
                    SetShadowDistance(60f);
                    if (DebrisManager.Instance != null) DebrisManager.Instance.SetMaxActiveDebris(140);
                    QualitySettings.vSyncCount = 1;
                    QualitySettings.shadows = ShadowQuality.All;
                    break;

                case QualityPreset.High:
                    SetRenderScale(1.0f);
                    SetShadowDistance(100f);
                    if (DebrisManager.Instance != null) DebrisManager.Instance.SetMaxActiveDebris(200);
                    QualitySettings.vSyncCount = 1;
                    QualitySettings.shadows = ShadowQuality.All;
                    break;

                case QualityPreset.Ultra:
                    SetRenderScale(1.0f);
                    SetShadowDistance(160f);
                    if (DebrisManager.Instance != null) DebrisManager.Instance.SetMaxActiveDebris(280);
                    QualitySettings.vSyncCount = 1;
                    QualitySettings.shadows = ShadowQuality.All;
                    break;
            }

            Debug.Log($"[QualitySettingsManager] Applied Preset: {preset}");
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
