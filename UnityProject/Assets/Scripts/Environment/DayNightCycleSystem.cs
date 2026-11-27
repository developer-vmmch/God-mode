// DayNightCycleSystem.cs
// CSVS GOD MODE — V0.3 Dynamic Day/Night & Weather System
// Extends the V0.2.2 four-preset KaraikalAtmosphereSystem into a smooth time-of-day
// cycle with real solar angle simulation at Karaikal latitude (10.925° N).

using System.Collections;
using UnityEngine;

namespace CSVS.GodMode.Environment
{
    public enum WeatherState { Clear, Cloudy, Monsoon, CoastalFog }
    public enum TimeOfDay    { Dawn, Morning, Afternoon, GoldenHour, Twilight, Night }

    public class DayNightCycleSystem : MonoBehaviour
    {
        // ─── Inspector ────────────────────────────────────────────────────
        [Header("Time Control")]
        [SerializeField] private float dayDurationSeconds = 600f;   // 10-min day cycle
        [SerializeField, Range(0f, 24f)] private float startHour = 8f;
        [SerializeField] private bool pauseTime;

        [Header("Sun & Sky")]
        [SerializeField] private Light sunLight;
        [SerializeField] private Light ambientFill;
        [SerializeField] private Gradient sunColorGradient;
        [SerializeField] private AnimationCurve sunIntensityCurve;
        [SerializeField] private float maxSunIntensity = 1.35f;

        [Header("Weather")]
        [SerializeField] private WeatherState currentWeather = WeatherState.Clear;
        [SerializeField] private ParticleSystem rainParticles;
        [SerializeField] private float fogDensityClear = 0.0006f;
        [SerializeField] private float fogDensityMonsoon = 0.0048f;
        [SerializeField] private float fogDensityFog = 0.0025f;

        [Header("Street & Building Lights")]
        [SerializeField] private Light[] streetLights;
        [SerializeField] private Renderer[] buildingWindowRenderers;
        [SerializeField] private Material windowLitMaterial;
        [SerializeField] private Material windowUnlitMaterial;

        // ─── State ─────────────────────────────────────────────────────────
        public float CurrentHour    { get; private set; }
        public TimeOfDay TimeOfDay  { get; private set; }
        public WeatherState Weather => currentWeather;

        private static DayNightCycleSystem _instance;
        public static DayNightCycleSystem Instance => _instance;

        // Karaikal latitude for approximate solar elevation calculation
        private const float KaraikalLatitude = 10.925f;

        // ─── Lifecycle ─────────────────────────────────────────────────────
        private void Awake()
        {
            _instance = this;
            CurrentHour = startHour;
            SetupDefaultGradientIfNeeded();
        }

        private void Update()
        {
            if (pauseTime) return;
            CurrentHour += (24f / dayDurationSeconds) * Time.deltaTime;
            if (CurrentHour >= 24f) CurrentHour -= 24f;

            ApplySunPosition();
            ApplyAtmosphere();
            ApplyStreetLights();
        }

        // ─── Solar Position ────────────────────────────────────────────────
        private void ApplySunPosition()
        {
            if (!sunLight) return;

            // Simplified solar elevation: treat hour 12 = solar noon, 0 = midnight
            float solarT    = CurrentHour / 24f;         // 0..1
            float sunAngle  = (solarT - 0.25f) * 360f;  // 0h = -90°, noon = 0°, 18h = 90°

            // Elevation: Karaikal tropical sun peaks ~79° at noon
            float elevation = Mathf.Sin(solarT * Mathf.PI * 2f - Mathf.PI * 0.5f) * (90f - KaraikalLatitude + 10f);
            float azimuth   = sunAngle;

            sunLight.transform.rotation = Quaternion.Euler(elevation, azimuth, 0f);

            // Color & intensity from gradient / curve
            sunLight.color     = sunColorGradient != null ? sunColorGradient.Evaluate(solarT) : GetFallbackSunColor();
            sunLight.intensity = sunIntensityCurve != null
                ? sunIntensityCurve.Evaluate(solarT) * maxSunIntensity
                : ComputeFallbackIntensity(solarT);

            if (ambientFill)
            {
                ambientFill.intensity = Mathf.Clamp01(sunLight.intensity * 0.35f);
                ambientFill.color = new Color(0.4f, 0.5f, 0.75f);  // Soft sky fill
            }

            // Map to TimeOfDay enum
            if      (CurrentHour < 5.5f)  TimeOfDay = TimeOfDay.Night;
            else if (CurrentHour < 7.0f)  TimeOfDay = TimeOfDay.Dawn;
            else if (CurrentHour < 12.0f) TimeOfDay = TimeOfDay.Morning;
            else if (CurrentHour < 16.0f) TimeOfDay = TimeOfDay.Afternoon;
            else if (CurrentHour < 18.5f) TimeOfDay = TimeOfDay.GoldenHour;
            else if (CurrentHour < 19.5f) TimeOfDay = TimeOfDay.Twilight;
            else                          TimeOfDay = TimeOfDay.Night;
        }

        // ─── Atmosphere & Weather ──────────────────────────────────────────
        private void ApplyAtmosphere()
        {
            // Sky ambient
            float t = CurrentHour / 24f;
            Color skyColor = GetSkyAmbientColor(t);
            RenderSettings.ambientLight = skyColor;

            // Weather fog
            RenderSettings.fog = true;
            switch (currentWeather)
            {
                case WeatherState.Clear:
                    RenderSettings.fogColor   = GetFogColor(t);
                    RenderSettings.fogDensity = fogDensityClear;
                    if (rainParticles && rainParticles.isPlaying) rainParticles.Stop();
                    break;

                case WeatherState.Cloudy:
                    RenderSettings.fogColor   = new Color(0.65f, 0.68f, 0.72f);
                    RenderSettings.fogDensity = fogDensityClear * 2.5f;
                    if (rainParticles && rainParticles.isPlaying) rainParticles.Stop();
                    break;

                case WeatherState.Monsoon:
                    RenderSettings.fogColor   = new Color(0.30f, 0.35f, 0.42f);
                    RenderSettings.fogDensity = fogDensityMonsoon;
                    if (rainParticles && !rainParticles.isPlaying) rainParticles.Play();
                    // Wet road: handled by KaraikalAtmosphereSystem material parameter
                    break;

                case WeatherState.CoastalFog:
                    RenderSettings.fogColor   = new Color(0.78f, 0.82f, 0.88f);
                    RenderSettings.fogDensity = fogDensityFog;
                    if (rainParticles && rainParticles.isPlaying) rainParticles.Stop();
                    break;
            }
        }

        // ─── Street Lights ─────────────────────────────────────────────────
        private void ApplyStreetLights()
        {
            bool lightsOn = TimeOfDay == TimeOfDay.Night || TimeOfDay == TimeOfDay.Twilight
                         || TimeOfDay == TimeOfDay.Dawn;

            foreach (var sl in streetLights)
                if (sl) sl.enabled = lightsOn;

            if (buildingWindowRenderers != null && windowLitMaterial && windowUnlitMaterial)
            {
                Material winMat = lightsOn ? windowLitMaterial : windowUnlitMaterial;
                foreach (var r in buildingWindowRenderers)
                    if (r) r.material = winMat;
            }
        }

        // ─── Weather Transition API ────────────────────────────────────────
        public void TransitionWeather(WeatherState newWeather, float durationSeconds = 10f)
            => StartCoroutine(WeatherTransitionCoroutine(newWeather, durationSeconds));

        private IEnumerator WeatherTransitionCoroutine(WeatherState newWeather, float duration)
        {
            float elapsed = 0f;
            var prev = currentWeather;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                // Gradual fog blend happens in next Update cycle
                yield return null;
            }
            currentWeather = newWeather;
            Debug.Log($"[DayNight] Weather → {newWeather}");
        }

        // ─── Time-of-day API (UI / missions) ──────────────────────────────
        public void SetTimeOfDay(float hour)
        {
            CurrentHour = Mathf.Clamp(hour, 0f, 24f);
        }

        public void SetWeather(WeatherState weather)
        {
            currentWeather = weather;
        }

        // ─── Colour Helpers ────────────────────────────────────────────────
        private Color GetFallbackSunColor()
        {
            float t = CurrentHour / 24f;
            if (TimeOfDay == TimeOfDay.GoldenHour || TimeOfDay == TimeOfDay.Twilight)
                return new Color(1f, 0.52f, 0.18f);
            if (TimeOfDay == TimeOfDay.Night || TimeOfDay == TimeOfDay.Dawn)
                return new Color(0.08f, 0.08f, 0.18f);
            return new Color(1f, 0.97f, 0.90f); // Warm tropical daylight
        }

        private float ComputeFallbackIntensity(float t)
        {
            // Bell curve: peaks at noon (t=0.5), zero at midnight (t=0 or 1)
            return Mathf.Max(0f, Mathf.Sin(t * Mathf.PI) * maxSunIntensity);
        }

        private Color GetSkyAmbientColor(float t)
        {
            if (t < 0.22f || t > 0.85f)   return new Color(0.02f, 0.02f, 0.08f);  // Night
            if (t < 0.28f)                 return new Color(0.30f, 0.22f, 0.35f);  // Dawn
            if (t < 0.65f)                 return new Color(0.45f, 0.52f, 0.65f);  // Day
            if (t < 0.75f)                 return new Color(0.55f, 0.35f, 0.20f);  // Golden hour
            return new Color(0.15f, 0.10f, 0.18f);                                  // Dusk
        }

        private Color GetFogColor(float t)
        {
            if (t < 0.22f || t > 0.85f) return new Color(0.01f, 0.01f, 0.05f);    // Night
            if (t < 0.28f)              return new Color(0.55f, 0.42f, 0.50f);    // Dawn pink
            return new Color(0.70f, 0.76f, 0.90f);                                  // Day blue haze
        }

        private void SetupDefaultGradientIfNeeded()
        {
            if (sunColorGradient != null) return;
            // Fallback: use procedural colour in Update
        }
    }
}

