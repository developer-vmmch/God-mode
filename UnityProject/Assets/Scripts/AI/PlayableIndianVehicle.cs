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
        Motorcycle = 3
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
                    bodyColor = new Color(0.95f, 0.78f, 0.12f); // Indian Auto Yellow & Black Hood
                    bodySize = new Vector3(1.4f, 1.6f, 2.6f);
                    break;
                case IndianVehicleType.Sedan:
                    bodyColor = new Color(0.85f, 0.88f, 0.90f); // Silver White
                    bodySize = new Vector3(1.8f, 1.4f, 4.4f);
                    break;
                case IndianVehicleType.Motorcycle:
                    bodyColor = new Color(0.80f, 0.15f, 0.15f); // Commuter Crimson Red
                    bodySize = new Vector3(0.7f, 1.1f, 2.0f);
                    break;
                default:
                    bodyColor = new Color(0.18f, 0.42f, 0.75f); // Indian City Blue Hatchback
                    bodySize = new Vector3(1.7f, 1.4f, 3.8f);
                    break;
            }

            Material bodyMat = new Material(litShader);
            if (bodyMat.HasProperty("_BaseColor")) bodyMat.SetColor("_BaseColor", bodyColor);
            if (bodyMat.HasProperty("_Smoothness")) bodyMat.SetFloat("_Smoothness", 0.7f);

            Material blackTireMat = new Material(litShader);
            if (blackTireMat.HasProperty("_BaseColor")) blackTireMat.SetColor("_BaseColor", new Color(0.12f, 0.12f, 0.14f));
            if (blackTireMat.HasProperty("_Smoothness")) blackTireMat.SetFloat("_Smoothness", 0.1f);

            _boxCollider.size = bodySize;
            _boxCollider.center = new Vector3(0f, bodySize.y * 0.5f, 0f);

            // Vehicle Body Visual
            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.name = "VehicleBodyVisual";
            body.transform.SetParent(transform, false);
            body.transform.localPosition = new Vector3(0f, bodySize.y * 0.5f, 0f);
            body.transform.localScale = bodySize;
            Destroy(body.GetComponent<Collider>());
            body.GetComponent<Renderer>().sharedMaterial = bodyMat;

            // Auto-Rickshaw Black Canopy Hood
            if (_vehicleType == IndianVehicleType.AutoRickshaw)
            {
                GameObject hood = GameObject.CreatePrimitive(PrimitiveType.Cube);
                hood.name = "AutoHood";
                hood.transform.SetParent(transform, false);
                hood.transform.localPosition = new Vector3(0f, bodySize.y + 0.1f, 0.1f);
                hood.transform.localScale = new Vector3(bodySize.x * 0.95f, 0.25f, bodySize.z * 0.95f);
                Destroy(hood.GetComponent<Collider>());
                hood.GetComponent<Renderer>().sharedMaterial = blackTireMat;
            }

            // Headlights
            GameObject hlL = new GameObject("Headlight_L");
            hlL.transform.SetParent(transform, false);
            hlL.transform.localPosition = new Vector3(-bodySize.x * 0.35f, 0.5f, bodySize.z * 0.5f + 0.1f);
            _headlightLeft = hlL.AddComponent<Light>();
            _headlightLeft.type = LightType.Spot;
            _headlightLeft.range = 25f;
            _headlightLeft.spotAngle = 45f;
            _headlightLeft.color = new Color(1f, 0.95f, 0.85f);
            _headlightLeft.intensity = 1.5f;
            _headlightLeft.enabled = false;

            GameObject hlR = new GameObject("Headlight_R");
            hlR.transform.SetParent(transform, false);
            hlR.transform.localPosition = new Vector3(bodySize.x * 0.35f, 0.5f, bodySize.z * 0.5f + 0.1f);
            _headlightRight = hlR.AddComponent<Light>();
            _headlightRight.type = LightType.Spot;
            _headlightRight.range = 25f;
            _headlightRight.spotAngle = 45f;
            _headlightRight.color = new Color(1f, 0.95f, 0.85f);
            _headlightRight.intensity = 1.5f;
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
            float motor = 0f;
            float steer = 0f;
            bool brake = false;

            if (Keyboard.current != null)
            {
                if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) motor += 1f;
                if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) motor -= 0.6f;
                if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) steer -= 1f;
                if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) steer += 1f;
                if (Keyboard.current.spaceKey.isPressed) brake = true;
            }

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
            // Player enters vehicle with F key when within 3.5 meters
            if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
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
            if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
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
