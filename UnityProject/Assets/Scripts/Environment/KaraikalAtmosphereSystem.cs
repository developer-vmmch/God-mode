using System;
using UnityEngine;

namespace CSVS.GodMode.Environment
{
    public enum KaraikalWeatherMode
    {
        ClearDay = 0,
        GoldenSunset = 1, // Coastal golden hour / twilight (media_1795684185309_f7bd2912.jpg)
        RainyEvening = 2, // Wet asphalt reflections, soft drizzle (media_1795684185309_f7bd2912.jpg)
        NightCityLights = 3 // Illuminated streetlights & neon shop signs (media_1795684160588_218f9585.jpg)
    }

    /// <summary>
    /// Dynamic coastal atmospheric lighting and weather system for Karaikal.
    /// Manages sun orientation, ambient sky gradients, fog density, streetlight activation,
    /// and wet road specular reflections.
    /// Fully optimized for URP 17.0.3 on low-spec integrated graphics.
    /// </summary>
    public class KaraikalAtmosphereSystem : MonoBehaviour
    {
        [Header("Atmosphere Configuration")]
        [SerializeField] private KaraikalWeatherMode _currentMode = KaraikalWeatherMode.GoldenSunset;
        [SerializeField] private Light _sunDirectionalLight;
        [SerializeField] private Material _roadAsphaltMaterial;

        [Header("Day-Night Cycle")]
        [SerializeField] private bool _enableAutomaticCycle = false;
        [SerializeField] private float _cycleDurationMinutes = 15f;

        private float _timeOfDay = 0.65f; // 0.0 = midnight, 0.25 = sunrise, 0.5 = noon, 0.7 = sunset

        public KaraikalWeatherMode CurrentMode => _currentMode;

        private void Awake()
        {
            Initialize();
        }

        public void Initialize(Light sunLight = null)
        {
            if (sunLight != null) _sunDirectionalLight = sunLight;
            if (_sunDirectionalLight == null)
            {
                Light[] lights = FindObjectsByType<Light>(FindObjectsSortMode.None);
                foreach (Light l in lights)
                {
                    if (l.type == LightType.Directional)
                    {
                        _sunDirectionalLight = l;
                        break;
                    }
                }
            }

            ApplyWeatherPreset(_currentMode);
        }

        private void Update()
        {
            if (_enableAutomaticCycle)
            {
                _timeOfDay += (Time.deltaTime / (_cycleDurationMinutes * 60f));
                if (_timeOfDay > 1f) _timeOfDay -= 1f;

                UpdateSunAngleFromTime(_timeOfDay);
            }
        }

        public void SetWeatherMode(KaraikalWeatherMode mode)
        {
            _currentMode = mode;
            if (_sunDirectionalLight == null) Initialize();
            ApplyWeatherPreset(mode);
        }

        private void ApplyWeatherPreset(KaraikalWeatherMode mode)
        {
            if (_sunDirectionalLight == null) return;

            switch (mode)
            {
                case KaraikalWeatherMode.ClearDay:
                    _sunDirectionalLight.transform.rotation = Quaternion.Euler(55f, -30f, 0f);
                    _sunDirectionalLight.color = new Color(1.0f, 0.98f, 0.92f);
                    _sunDirectionalLight.intensity = 1.6f;
                    RenderSettings.ambientLight = new Color(0.45f, 0.55f, 0.65f);
                    RenderSettings.fog = true;
                    RenderSettings.fogColor = new Color(0.70f, 0.80f, 0.90f);
                    RenderSettings.fogDensity = 0.0015f;
                    SetRoadWetness(0.2f);
                    SetStreetlightsActive(false);
                    break;

                case KaraikalWeatherMode.GoldenSunset:
                    // Coastal Bay of Bengal sunset / twilight
                    _sunDirectionalLight.transform.rotation = Quaternion.Euler(14f, 75f, 0f);
                    _sunDirectionalLight.color = new Color(1.0f, 0.62f, 0.32f); // Golden amber
                    _sunDirectionalLight.intensity = 1.8f;
                    RenderSettings.ambientLight = new Color(0.55f, 0.35f, 0.38f);
                    RenderSettings.fog = true;
                    RenderSettings.fogColor = new Color(0.85f, 0.55f, 0.38f);
                    RenderSettings.fogDensity = 0.0025f;
                    SetRoadWetness(0.4f);
                    SetStreetlightsActive(true);
                    break;

                case KaraikalWeatherMode.RainyEvening:
                    // Soft coastal drizzle with wet asphalt reflections
                    _sunDirectionalLight.transform.rotation = Quaternion.Euler(25f, 60f, 0f);
                    _sunDirectionalLight.color = new Color(0.75f, 0.70f, 0.68f);
                    _sunDirectionalLight.intensity = 0.85f;
                    RenderSettings.ambientLight = new Color(0.35f, 0.38f, 0.42f);
                    RenderSettings.fog = true;
                    RenderSettings.fogColor = new Color(0.45f, 0.48f, 0.52f);
                    RenderSettings.fogDensity = 0.0055f;
                    SetRoadWetness(0.88f); // High specular wet road reflections
                    SetStreetlightsActive(true);
                    break;

                case KaraikalWeatherMode.NightCityLights:
                    // Night with sodium streetlights and illuminated shop signs
                    _sunDirectionalLight.transform.rotation = Quaternion.Euler(-25f, 75f, 0f);
                    _sunDirectionalLight.color = new Color(0.15f, 0.22f, 0.40f); // Moonlit navy
                    _sunDirectionalLight.intensity = 0.25f;
                    RenderSettings.ambientLight = new Color(0.12f, 0.15f, 0.24f);
                    RenderSettings.fog = true;
                    RenderSettings.fogColor = new Color(0.08f, 0.10f, 0.18f);
                    RenderSettings.fogDensity = 0.0035f;
                    SetRoadWetness(0.35f);
                    SetStreetlightsActive(true);
                    break;
            }
        }

        private void SetRoadWetness(float smoothness)
        {
            if (_roadAsphaltMaterial != null)
            {
                if (_roadAsphaltMaterial.HasProperty("_Smoothness"))
                {
                    _roadAsphaltMaterial.SetFloat("_Smoothness", smoothness);
                }
            }
        }

        private void SetStreetlightsActive(bool active)
        {
            var streetlights = FindObjectsByType<Light>(FindObjectsSortMode.None);
            foreach (var l in streetlights)
            {
                if (l.type == LightType.Spot && l.gameObject.name.Contains("Streetlight"))
                {
                    l.enabled = active;
                }
            }
        }

        private void UpdateSunAngleFromTime(float t)
        {
            float sunAngle = (t * 360f) - 90f;
            _sunDirectionalLight.transform.rotation = Quaternion.Euler(sunAngle, 60f, 0f);
        }
    }
}
