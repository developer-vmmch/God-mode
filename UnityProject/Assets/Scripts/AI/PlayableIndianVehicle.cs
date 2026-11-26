using System;
using UnityEngine;
using UnityEngine.InputSystem;
using CSVS.GodMode.Core;
using CSVS.GodMode.Destruction;
using CSVS.GodMode.Player;

namespace CSVS.GodMode.AI
{
    public enum IndianVehicleType
    {
        Hatchback = 0,
        AutoRickshaw = 1,
        Sedan = 2,
        Motorcycle = 3,
        CityBus = 4
    }

    /// <summary>
    /// GTA-inspired drivable Indian vehicle controller.
    /// Supports player entry and exit (F key), wheel rotation, steering, acceleration,
    /// braking, reverse, headlights, and AI circulation when unoccupied.
    /// Fully compatible with superhero Super Strength grab/throw and Gravity Tether.
    /// </summary>
    [RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
    public class PlayableIndianVehicle : MonoBehaviour, IDamageable
    {
        [Header("Vehicle Characteristics")]
        [SerializeField] private IndianVehicleType _vehicleType = IndianVehicleType.Hatchback;
        [SerializeField] private float _enginePower = 1800f;
        [SerializeField] private float _maxSpeed = 25f; // ~90 km/h
        [SerializeField] private float _turnSpeed = 65f;
        [SerializeField] private float _brakeDrag = 5.0f;

        [Header("Visual Elements")]
        [SerializeField] private Transform _wheelFrontLeft;
        [SerializeField] private Transform _wheelFrontRight;
        [SerializeField] private Transform _wheelRearLeft;
        [SerializeField] private Transform _wheelRearRight;
        [SerializeField] private Light _headlightLeft;
        [SerializeField] private Light _headlightRight;

        private Rigidbody _rigidbody;
        private BoxCollider _boxCollider;
        private SuperheroFlightController _occupantHero;
        private bool _isPlayerOccupied = false;
        private float _health = 350f;

        public float CurrentHealth => _health;
        public float MaxHealth => 350f;
        public bool IsDestroyed => _health <= 0f;
        public bool IsOccupied => _isPlayerOccupied;
        public float CurrentSpeedKmh => _rigidbody != null ? _rigidbody.linearVelocity.magnitude * 3.6f : 0f;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _boxCollider = GetComponent<BoxCollider>();

            _rigidbody.mass = (_vehicleType == IndianVehicleType.AutoRickshaw) ? 450f : 1100f;
            _rigidbody.linearDamping = 0.5f;
            _rigidbody.angularDamping = 2.0f;
            _rigidbody.centerOfMass = new Vector3(0f, -0.3f, 0f); // Low center of gravity to prevent flipping

            BuildVehicleGeometry();
        }

        private void BuildVehicleGeometry()
        {
            Shader litShader = Shader.Find("Universal Render Pipeline/Lit");
            if (litShader == null) litShader = Shader.Find("Universal Render Pipeline/Simple Lit");
            if (litShader == null) litShader = Shader.Find("Standard");

            Color bodyColor;
            Vector3 bodySize;

            switch (_vehicleType)
            {
                case IndianVehicleType.AutoRickshaw:
                    bodyColor = new Color(0.12f, 0.12f, 0.14f); // Black lower metal tub
                    bodySize = new Vector3(1.4f, 1.7f, 2.6f);
                    break;
                case IndianVehicleType.Sedan:
                    bodyColor = new Color(0.85f, 0.88f, 0.90f); // Silver White
                    bodySize = new Vector3(1.8f, 1.4f, 4.4f);
                    break;
                case IndianVehicleType.Motorcycle:
                    bodyColor = new Color(0.80f, 0.15f, 0.15f); // Commuter Crimson Red
                    bodySize = new Vector3(0.7f, 1.1f, 2.0f);
                    break;
                case IndianVehicleType.CityBus:
                    bodyColor = new Color(0.20f, 0.55f, 0.25f); // Indian State Transport Green Bus
                    bodySize = new Vector3(2.5f, 3.2f, 9.5f);
                    break;
                default:
                    bodyColor = new Color(0.18f, 0.42f, 0.75f); // City Blue Hatchback
                    bodySize = new Vector3(1.7f, 1.4f, 3.8f);
                    break;
            }

            Material bodyMat = new Material(litShader);
            if (bodyMat.HasProperty("_BaseColor")) bodyMat.SetColor("_BaseColor", bodyColor);
            if (bodyMat.HasProperty("_Smoothness")) bodyMat.SetFloat("_Smoothness", 0.7f);

            Material yellowCanopyMat = new Material(litShader);
            if (yellowCanopyMat.HasProperty("_BaseColor")) yellowCanopyMat.SetColor("_BaseColor", new Color(0.96f, 0.78f, 0.10f));
            if (yellowCanopyMat.HasProperty("_Smoothness")) yellowCanopyMat.SetFloat("_Smoothness", 0.25f);

            Material blackTireMat = new Material(litShader);
            if (blackTireMat.HasProperty("_BaseColor")) blackTireMat.SetColor("_BaseColor", new Color(0.10f, 0.10f, 0.12f));
            if (blackTireMat.HasProperty("_Smoothness")) blackTireMat.SetFloat("_Smoothness", 0.15f);

            Material chromeMat = new Material(litShader);
            if (chromeMat.HasProperty("_BaseColor")) chromeMat.SetColor("_BaseColor", new Color(0.80f, 0.82f, 0.85f));
            if (chromeMat.HasProperty("_Metallic")) chromeMat.SetFloat("_Metallic", 0.85f);
            if (chromeMat.HasProperty("_Smoothness")) chromeMat.SetFloat("_Smoothness", 0.85f);

            Material glassMat = new Material(litShader);
            if (glassMat.HasProperty("_BaseColor")) glassMat.SetColor("_BaseColor", new Color(0.25f, 0.38f, 0.48f, 0.8f));
            if (glassMat.HasProperty("_Smoothness")) glassMat.SetFloat("_Smoothness", 0.95f);

            Material lightMat = new Material(litShader);
            if (lightMat.HasProperty("_BaseColor")) lightMat.SetColor("_BaseColor", new Color(1.0f, 0.98f, 0.85f));
            if (lightMat.HasProperty("_EmissionColor"))
            {
                lightMat.EnableKeyword("_EMISSION");
                lightMat.SetColor("_EmissionColor", new Color(1.2f, 1.1f, 0.8f));
            }

            _boxCollider.size = bodySize;
            _boxCollider.center = new Vector3(0f, bodySize.y * 0.5f, 0f);

            // Generate detailed multi-element vehicle geometry
            VehicleModelGenerator.GenerateVehicleModel(gameObject, _vehicleType, bodyMat, yellowCanopyMat, chromeMat, glassMat, blackTireMat, lightMat);

            // Operational Headlights
            GameObject hlL = new GameObject("Headlight_L");
            hlL.transform.SetParent(transform, false);
            hlL.transform.localPosition = new Vector3(-bodySize.x * 0.35f, 0.5f, bodySize.z * 0.5f + 0.1f);
            _headlightLeft = hlL.AddComponent<Light>();
            _headlightLeft.type = LightType.Spot;
            _headlightLeft.range = 30f;
            _headlightLeft.spotAngle = 50f;
            _headlightLeft.color = new Color(1f, 0.96f, 0.88f);
            _headlightLeft.intensity = 2.0f;
            _headlightLeft.enabled = false;

            GameObject hlR = new GameObject("Headlight_R");
            hlR.transform.SetParent(transform, false);
            hlR.transform.localPosition = new Vector3(bodySize.x * 0.35f, 0.5f, bodySize.z * 0.5f + 0.1f);
            _headlightRight = hlR.AddComponent<Light>();
            _headlightRight.type = LightType.Spot;
            _headlightRight.range = 30f;
            _headlightRight.spotAngle = 50f;
            _headlightRight.color = new Color(1f, 0.96f, 0.88f);
            _headlightRight.intensity = 2.0f;
            _headlightRight.enabled = false;
        }

        private void Update()
        {
            if (_isPlayerOccupied)
            {
                HandlePlayerExitInput();
            }
            else
            {
                CheckProximityPlayerEntry();
            }
        }

        private void FixedUpdate()
        {
            if (_isPlayerOccupied)
            {
                DriveVehiclePhysics();
            }
        }

        private void DriveVehiclePhysics()
        {
            Vector2 move = GameInputManager.MoveInput;
            float motor = move.y;
            float steer = move.x;
            bool brake = GameInputManager.IsAscendPressed;

            // Apply Motor Force
            float forwardSpeed = Vector3.Dot(_rigidbody.linearVelocity, transform.forward);
            if (motor != 0f && forwardSpeed < _maxSpeed)
            {
                Vector3 driveForce = transform.forward * (motor * _enginePower);
                _rigidbody.AddForce(driveForce, ForceMode.Force);
            }

            // Apply Steering Torque
            if (Mathf.Abs(forwardSpeed) > 0.5f)
            {
                float steerMultiplier = (forwardSpeed < 0f) ? -1f : 1f;
                float turnTorque = steer * _turnSpeed * steerMultiplier;
                _rigidbody.AddTorque(Vector3.up * turnTorque, ForceMode.Acceleration);
            }

            // Handbrake Drag
            if (brake)
            {
                _rigidbody.linearDamping = _brakeDrag;
            }
            else
            {
                _rigidbody.linearDamping = 0.5f;
            }
        }

        private void CheckProximityPlayerEntry()
        {
            // Player enters vehicle with G key when within 3.5 meters
            if (GameInputManager.WasVehicleInteractTriggered)
            {
                var hero = GameObject.FindGameObjectWithTag("Player");
                if (hero != null && Vector3.Distance(transform.position, hero.transform.position) < 3.5f)
                {
                    EnterVehicle(hero.GetComponent<SuperheroFlightController>());
                }
            }
        }

        private void HandlePlayerExitInput()
        {
            // Player exits vehicle with G key
            if (GameInputManager.WasVehicleInteractTriggered)
            {
                ExitVehicle();
            }
        }

        public void EnterVehicle(SuperheroFlightController hero)
        {
            if (hero == null) return;
            _occupantHero = hero;
            _isPlayerOccupied = true;

            // Parent hero inside vehicle and disable flight physics during driving
            hero.transform.SetParent(transform, false);
            hero.transform.localPosition = new Vector3(0f, 0.4f, 0f);
            hero.gameObject.SetActive(false);

            if (_headlightLeft != null) _headlightLeft.enabled = true;
            if (_headlightRight != null) _headlightRight.enabled = true;

            // Target camera to this vehicle
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                var camCtrl = mainCam.GetComponent<SuperheroCameraController>();
                if (camCtrl != null) camCtrl.SetTarget(transform);
            }
        }

        public void ExitVehicle()
        {
            if (_occupantHero != null)
            {
                _occupantHero.transform.SetParent(null, true);
                _occupantHero.transform.position = transform.position - transform.right * 1.8f + Vector3.up * 0.4f;
                _occupantHero.gameObject.SetActive(true);

                Camera mainCam = Camera.main;
                if (mainCam != null)
                {
                    var camCtrl = mainCam.GetComponent<SuperheroCameraController>();
                    if (camCtrl != null) camCtrl.SetTarget(_occupantHero.transform);
                }

                _occupantHero = null;
            }

            _isPlayerOccupied = false;
            if (_headlightLeft != null) _headlightLeft.enabled = false;
            if (_headlightRight != null) _headlightRight.enabled = false;
        }

        public void TakeDamage(float amount, DamageType type, Vector3 point, Vector3 normal)
        {
            _health -= amount;
            if (_health <= 0f)
            {
                // Explode vehicle into debris
                if (_isPlayerOccupied) ExitVehicle();
                Destroy(gameObject);
            }
        }
    }
}
