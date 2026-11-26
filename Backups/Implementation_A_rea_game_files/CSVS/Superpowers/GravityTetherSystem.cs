using UnityEngine;
using CSVS.Input;
using CSVS.Destruction;
using CSVS.Core;

namespace CSVS.Superpowers
{
    /// <summary>
    /// Gravity Tether superpower.
    /// Q key attaches an energy beam to a target physics object or building chunk,
    /// suspending and dragging it through 3D space, and launching/hurling it upon release.
    /// </summary>
    public class GravityTetherSystem : MonoBehaviour
    {
        [Header("Tether Parameters")]
        [SerializeField] private float maxTetherRange = 75f;
        [SerializeField] private float tetherPullStrength = 350f;
        [SerializeField] private float launchMultiplier = 40f;
        [SerializeField] private LayerMask tetherLayers;

        [Header("Beam Visualization")]
        [SerializeField] private LineRenderer tetherLine;
        [SerializeField] private Transform handEmitter;

        private Rigidbody tetheredRigidbody = null;
        private Vector3 targetAnchorOffset;

        private void Start()
        {
            if (handEmitter == null)
            {
                GameObject emitter = new GameObject("HandEmitter");
                emitter.transform.SetParent(transform);
                emitter.transform.localPosition = new Vector3(0.35f, 1.2f, 0.5f);
                handEmitter = emitter.transform;
            }

            if (tetherLine == null && handEmitter != null)
            {
                tetherLine = handEmitter.gameObject.AddComponent<LineRenderer>();
                tetherLine.startWidth = 0.08f;
                tetherLine.endWidth = 0.04f;
                tetherLine.positionCount = 2;
                tetherLine.useWorldSpace = true;
                tetherLine.material = new Material(Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Sprites/Default"));
                tetherLine.startColor = new Color(0.2f, 0.7f, 1f, 0.9f);
                tetherLine.endColor = new Color(0.6f, 0.2f, 1f, 0.9f);
                tetherLine.enabled = false;
            }
        }

        private void Update()
        {
            if (SuperheroInput.Instance == null) return;

            if (SuperheroInput.Instance.GravityTetherHeld)
            {
                if (tetheredRigidbody == null)
                {
                    TryAnchorTether();
                }
                else
                {
                    UpdateTetherLine();
                }
            }

            if (SuperheroInput.Instance.GravityTetherReleased)
            {
                ReleaseAndHurl();
            }
        }

        private void FixedUpdate()
        {
            if (tetheredRigidbody == null) return;

            UnityEngine.Camera cam = UnityEngine.Camera.main;
            Vector3 holdPos = cam != null
                ? cam.transform.position + cam.transform.forward * 14f
                : transform.position + transform.forward * 14f;

            Vector3 delta = holdPos - tetheredRigidbody.position;
            Vector3 force = delta * tetherPullStrength - tetheredRigidbody.linearVelocity * 15f;
            tetheredRigidbody.AddForce(force, ForceMode.Acceleration);
        }

        private void TryAnchorTether()
        {
            UnityEngine.Camera cam = UnityEngine.Camera.main;
            Ray ray = cam != null
                ? cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f))
                : new Ray(transform.position + Vector3.up * 1.5f, transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, maxTetherRange, tetherLayers))
            {
                Rigidbody rb = hit.collider.attachedRigidbody;
                if (rb != null)
                {
                    BuildingChunk chunk = hit.collider.GetComponent<BuildingChunk>();
                    if (chunk != null) chunk.DetachAndActivatePhysics();

                    tetheredRigidbody = rb;
                    tetheredRigidbody.useGravity = false;
                    if (tetherLine != null) tetherLine.enabled = true;
                }
            }
        }

        private void UpdateTetherLine()
        {
            if (tetherLine == null || tetheredRigidbody == null || handEmitter == null) return;
            tetherLine.SetPosition(0, handEmitter.position);
            tetherLine.SetPosition(1, tetheredRigidbody.position);
        }

        private void ReleaseAndHurl()
        {
            if (tetheredRigidbody == null) return;

            UnityEngine.Camera cam = UnityEngine.Camera.main;
            Vector3 launchDir = cam != null ? cam.transform.forward : transform.forward;

            tetheredRigidbody.useGravity = true;
            tetheredRigidbody.linearVelocity = launchDir * launchMultiplier;

            tetheredRigidbody = null;
            if (tetherLine != null) tetherLine.enabled = false;

            if (ChaosManager.Instance != null)
            {
                ChaosManager.Instance.AddChaos(150, "GravityHurl");
            }
        }
    }
}
