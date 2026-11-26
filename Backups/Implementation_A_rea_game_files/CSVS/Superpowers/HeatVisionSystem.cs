using UnityEngine;
using CSVS.Input;
using CSVS.Destruction;
using CSVS.Core;
using CSVS.Camera;

namespace CSVS.Superpowers
{
    /// <summary>
    /// Dual-beam Heat Vision superpower.
    /// Projects continuous twin raycast lasers from the hero's eye sockets to the reticle point,
    /// dealing progressive thermal damage to destructible buildings, enemies, and vehicles.
    /// </summary>
    public class HeatVisionSystem : MonoBehaviour
    {
        [Header("Beam Emitters")]
        [SerializeField] private Transform leftEyeSocket;
        [SerializeField] private Transform rightEyeSocket;
        [SerializeField] private LineRenderer leftBeam;
        [SerializeField] private LineRenderer rightBeam;

        [Header("Beam Parameters")]
        [SerializeField] private float maxRange = 150f;
        [SerializeField] private float thermalDamagePerSecond = 250f;
        [SerializeField] private float forceImpulse = 40f;
        [SerializeField] private LayerMask hitLayers;

        [Header("VFX & Audio")]
        [SerializeField] private ParticleSystem impactSparkFX;
        [SerializeField] private ParticleSystem beamEnergyFX;
        [SerializeField] private AudioSource beamAudioSource;

        [Header("Heat Budget")]
        [SerializeField] private float maxHeat = 100f;
        [SerializeField] private float heatPerSecond = 20f;
        [SerializeField] private float coolDownPerSecond = 35f;
        private float currentHeat = 0f;
        private bool isOverheated = false;

        public float CurrentHeat => currentHeat;
        public float MaxHeat => maxHeat;
        public bool IsOverheated => isOverheated;

        private void Start()
        {
            SetupDefaultSockets();
            SetBeamsActive(false);
        }

        private void SetupDefaultSockets()
        {
            if (leftEyeSocket == null)
            {
                GameObject le = new GameObject("LeftEyeSocket");
                le.transform.SetParent(transform);
                le.transform.localPosition = new Vector3(-0.15f, 1.7f, 0.25f);
                leftEyeSocket = le.transform;
            }

            if (rightEyeSocket == null)
            {
                GameObject re = new GameObject("RightEyeSocket");
                re.transform.SetParent(transform);
                re.transform.localPosition = new Vector3(0.15f, 1.7f, 0.25f);
                rightEyeSocket = re.transform;
            }

            if (leftBeam == null && leftEyeSocket != null)
            {
                leftBeam = leftEyeSocket.gameObject.AddComponent<LineRenderer>();
                ConfigureBeamLine(leftBeam);
            }

            if (rightBeam == null && rightEyeSocket != null)
            {
                rightBeam = rightEyeSocket.gameObject.AddComponent<LineRenderer>();
                ConfigureBeamLine(rightBeam);
            }
        }

        private void ConfigureBeamLine(LineRenderer lr)
        {
            lr.startWidth = 0.12f;
            lr.endWidth = 0.08f;
            lr.positionCount = 2;
            lr.useWorldSpace = true;
            lr.material = new Material(Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Sprites/Default"));
            lr.startColor = new Color(1f, 0.2f, 0.05f, 1f);
            lr.endColor = new Color(1f, 0.7f, 0.1f, 1f);
        }

        private void Update()
        {
            bool isFiring = SuperheroInput.Instance != null && SuperheroInput.Instance.IsHeatVisionActive;

            if (isFiring && !isOverheated)
            {
                FireHeatVision();
                currentHeat += heatPerSecond * Time.deltaTime;
                if (currentHeat >= maxHeat)
                {
                    isOverheated = true;
                }
            }
            else
            {
                StopHeatVision();
                currentHeat = Mathf.Max(0f, currentHeat - coolDownPerSecond * Time.deltaTime);
                if (isOverheated && currentHeat <= 20f)
                {
                    isOverheated = false;
                }
            }
        }

        private void FireHeatVision()
        {
            SetBeamsActive(true);

            UnityEngine.Camera cam = UnityEngine.Camera.main;
            Ray camRay = cam != null
                ? cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f))
                : new Ray(transform.position + Vector3.up * 1.6f, transform.forward);

            Vector3 targetPoint;
            RaycastHit hit;

            if (Physics.Raycast(camRay, out hit, maxRange, hitLayers))
            {
                targetPoint = hit.point;

                // Apply continuous thermal damage
                float frameDamage = thermalDamagePerSecond * Time.deltaTime;

                // Destructible building damage
                DestructibleBuilding building = hit.collider.GetComponentInParent<DestructibleBuilding>();
                if (building != null)
                {
                    building.ApplyThermalDamage(hit.point, frameDamage);
                }

                BuildingChunk chunk = hit.collider.GetComponent<BuildingChunk>();
                if (chunk != null)
                {
                    chunk.ApplyDamage(frameDamage, hit.point);
                }

                // Push dynamic rigidbodies
                Rigidbody rb = hit.collider.attachedRigidbody;
                if (rb != null && !rb.isKinematic)
                {
                    rb.AddForceAtPosition(camRay.direction * forceImpulse, hit.point, ForceMode.Force);
                }

                // Position impact VFX
                if (impactSparkFX != null)
                {
                    impactSparkFX.transform.position = hit.point;
                    impactSparkFX.transform.rotation = Quaternion.LookRotation(hit.normal);
                    if (!impactSparkFX.isPlaying) impactSparkFX.Play();
                }

                if (ChaosManager.Instance != null)
                {
                    ChaosManager.Instance.AddChaos((long)(15 * Time.deltaTime * 60f), "HeatVision");
                }
            }
            else
            {
                targetPoint = camRay.origin + camRay.direction * maxRange;
                if (impactSparkFX != null && impactSparkFX.isPlaying)
                {
                    impactSparkFX.Stop();
                }
            }

            // Update beam coordinates
            leftBeam.SetPosition(0, leftEyeSocket.position);
            leftBeam.SetPosition(1, targetPoint);

            rightBeam.SetPosition(0, rightEyeSocket.position);
            rightBeam.SetPosition(1, targetPoint);

            // Subtle camera rumble
            if (SuperheroCameraController.Instance != null)
            {
                SuperheroCameraController.Instance.AddTrauma(0.04f * Time.deltaTime * 60f);
            }
        }

        private void StopHeatVision()
        {
            SetBeamsActive(false);
            if (impactSparkFX != null && impactSparkFX.isPlaying)
            {
                impactSparkFX.Stop();
            }
        }

        private void SetBeamsActive(bool active)
        {
            if (leftBeam != null) leftBeam.enabled = active;
            if (rightBeam != null) rightBeam.enabled = active;

            if (beamAudioSource != null)
            {
                if (active && !beamAudioSource.isPlaying) beamAudioSource.Play();
                else if (!active && beamAudioSource.isPlaying) beamAudioSource.Stop();
            }
        }
    }
}
