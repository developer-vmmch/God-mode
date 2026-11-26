using System;
using UnityEngine;

namespace CSVS.GodMode.Core
{
    public enum GraphicsQualityTier
    {
        Low = 0,      // Target: Core i3-10105T / Intel UHD 630 (30 FPS target, 50 debris, 1280x720, 0.75 render scale)
        Medium = 1,   // Target: Mid-range PC (60 FPS target, 100 debris, 1920x1080, 0.85 render scale)
        High = 2      // Target: High-spec PC (60 FPS target, 150 debris hard cap, 1920x1080, 1.0 render scale)
    }

    /// <summary>
    /// Centralized authoritative gameplay configuration system.
    /// Provides standardized design values, physics constants, and hardware optimization presets.
    /// </summary>
    public class GameplayConfig : MonoBehaviour
    {
        public static GameplayConfig Instance { get; private set; }

        [Header("Flight Kinematics (Standardized)")]
        [SerializeField] private float _normalFlightSpeed = 35.0f;
        [SerializeField] private float _boostFlightSpeed = 95.0f;
        [SerializeField] private float _maxConfigurableSpeed = 150.0f;
        [SerializeField] private float _flightAcceleration = 45.0f;
        [SerializeField] private float _flightDeceleration = 30.0f;
        [SerializeField] private float _hoverBobbingFrequency = 1.5f;
        [SerializeField] private float _hoverBobbingAmplitude = 0.25f;

        [Header("Ground Locomotion")]
        [SerializeField] private float _walkSpeed = 8.0f;
        [SerializeField] private float _runSpeed = 16.0f;
        [SerializeField] private float _jumpHeight = 3.5f;
        [SerializeField] private float _groundGravity = 25.0f;

        [Header("Ground Slam Physics (Quadratic Falloff)")]
        [SerializeField] private float _groundSlamMaxForce = 1500.0f;
        [SerializeField] private float _groundSlamRadius = 25.0f;
        [SerializeField] private float _groundSlamMinSpeed = 40.0f;
        [SerializeField] private float _groundSlamDamage = 500.0f;

        [Header("Superpowers")]
        [SerializeField] private float _superPunchDamage = 450.0f;
        [SerializeField] private float _superPunchImpulse = 85.0f;
        [SerializeField] private float _heatVisionDPS = 350.0f;
        [SerializeField] private float _heatVisionRange = 250.0f;
        [SerializeField] private float _grabRange = 12.0f;
        [SerializeField] private float _throwForce = 60.0f;
        [SerializeField] private float _crushDamage = 1000.0f;
        [SerializeField] private float _gravityTetherRange = 100.0f;
        [SerializeField] private float _gravityTetherSpring = 250.0f;

        [Header("Debris Budget per Graphics Tier")]
        [SerializeField] private int _debrisTargetLow = 50;
        [SerializeField] private int _debrisTargetMedium = 100;
        [SerializeField] private int _debrisTargetHigh = 150;
        [SerializeField] private int _debrisHardCap = 150;

        [Header("Active Quality Preset")]
        [SerializeField] private GraphicsQualityTier _currentTier = GraphicsQualityTier.Low;

        public float NormalFlightSpeed => _normalFlightSpeed;
        public float BoostFlightSpeed => _boostFlightSpeed;
        public float MaxConfigurableSpeed => _maxConfigurableSpeed;
        public float FlightAcceleration => _flightAcceleration;
        public float FlightDeceleration => _flightDeceleration;
        public float HoverBobbingFrequency => _hoverBobbingFrequency;
        public float HoverBobbingAmplitude => _hoverBobbingAmplitude;

        public float WalkSpeed => _walkSpeed;
        public float RunSpeed => _runSpeed;
        public float JumpHeight => _jumpHeight;
        public float GroundGravity => _groundGravity;

        public float GroundSlamMaxForce => _groundSlamMaxForce;
        public float GroundSlamRadius => _groundSlamRadius;
        public float GroundSlamMinSpeed => _groundSlamMinSpeed;
        public float GroundSlamDamage => _groundSlamDamage;

        public float SuperPunchDamage => _superPunchDamage;
        public float SuperPunchImpulse => _superPunchImpulse;
        public float HeatVisionDPS => _heatVisionDPS;
        public float HeatVisionRange => _heatVisionRange;
        public float GrabRange => _grabRange;
        public float ThrowForce => _throwForce;
        public float CrushDamage => _crushDamage;
        public float GravityTetherRange => _gravityTetherRange;
        public float GravityTetherSpring => _gravityTetherSpring;

        public int DebrisTargetLow => _debrisTargetLow;
        public int DebrisTargetMedium => _debrisTargetMedium;
        public int DebrisTargetHigh => _debrisTargetHigh;
        public int DebrisHardCap => _debrisHardCap;
        public GraphicsQualityTier CurrentQualityTier => _currentTier;

        public int CurrentDebrisBudget
        {
            get
            {
                switch (_currentTier)
                {
                    case GraphicsQualityTier.Low: return _debrisTargetLow;
                    case GraphicsQualityTier.Medium: return _debrisTargetMedium;
                    case GraphicsQualityTier.High: return _debrisTargetHigh;
                    default: return _debrisTargetLow;
                }
            }
        }

        public event Action<GraphicsQualityTier> OnQualityTierChanged;

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

        public void SetQualityTier(GraphicsQualityTier tier)
        {
            _currentTier = tier;
            OnQualityTierChanged?.Invoke(_currentTier);
        }

        /// <summary>
        /// Exact quadratic ground slam shockwave force calculation:
        /// F = Fmax * Pow(Clamp01(1 - distance / radius), 2)
        /// </summary>
        public static float CalculateGroundSlamForce(float distance, float radius, float maxForce)
        {
            if (radius <= 0f) return 0f;
            float normalized = Mathf.Clamp01(1.0f - (distance / radius));
            return maxForce * Mathf.Pow(normalized, 2.0f);
        }
    }
}
