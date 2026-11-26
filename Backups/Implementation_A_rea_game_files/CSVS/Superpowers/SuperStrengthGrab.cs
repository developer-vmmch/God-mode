using UnityEngine;
using CSVS.Input;
using CSVS.Destruction;
using CSVS.Core;

namespace CSVS.Superpowers
{
    /// <summary>
    /// Super Strength Grab, Throw, and Crush superpower.
    /// Allows the superhero to grab vehicles, debris, and objects (E),
    /// hold them hoisted above the head, throw them forward (LMB), or crush them into scrap (RMB).
    /// </summary>
    public class SuperStrengthGrab : MonoBehaviour
    {
        [Header("Sockets & Range")]
        [SerializeField] private Transform holdSocket;
        [SerializeField] private float grabRange = 12f;
        [SerializeField] private float throwForce = 65f;
        [SerializeField] private LayerMask grabbableLayers;

        [Header("Damped Spring Hold Physics")]
        [SerializeField] private float springStrength = 450f;
        [SerializeField] private float damping = 35f;

        [Header("Crush FX")]
        [SerializeField] private ParticleSystem crushScrapFXPrefab;
        [SerializeField] private AudioClip crushSound;
        [SerializeField] private AudioClip throwSound;

        private Rigidbody grabbedRigidbody = null;
        private Collider grabbedCollider = null;

        public bool IsHoldingObject => grabbedRigidbody != null;

        private void Start()
        {
            if (holdSocket == null)
            {
                GameObject socket = new GameObject("GrabSocket");
                socket.transform.SetParent(transform);
                socket.transform.localPosition = new Vector3(0f, 2.4f, 0.4f);
                holdSocket = socket.transform;
            }
        }

        private void Update()
        {
            if (SuperheroInput.Instance == null) return;

            if (SuperheroInput.Instance.GrabPressed)
            {
                if (grabbedRigidbody == null)
                {
                    TryGrabObject();
                }
                else
                {
                    ReleaseGrabbedObject();
                }
            }

            if (grabbedRigidbody != null)
            {
                // Throw with Left Click
                if (UnityEngine.Input.GetMouseButtonDown(0))
                {
                    ThrowObject();
                }
                // Crush with Right Click
                else if (UnityEngine.Input.GetMouseButtonDown(1))
                {
                    CrushObject();
                }
            }
        }

        private void FixedUpdate()
        {
            if (grabbedRigidbody == null || holdSocket == null) return;

            // Damped spring physics toward socket
            Vector3 delta = holdSocket.position - grabbedRigidbody.position;
            Vector3 force = delta * springStrength - grabbedRigidbody.linearVelocity * damping;
            grabbedRigidbody.AddForce(force, ForceMode.Acceleration);

            // Match socket rotation
            grabbedRigidbody.MoveRotation(Quaternion.Slerp(grabbedRigidbody.rotation, holdSocket.rotation, Time.fixedDeltaTime * 15f));
        }

        private void TryGrabObject()
        {
            UnityEngine.Camera cam = UnityEngine.Camera.main;
            Ray ray = cam != null
                ? cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f))
                : new Ray(transform.position + Vector3.up * 1.5f, transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, grabRange, grabbableLayers))
            {
                Rigidbody rb = hit.collider.attachedRigidbody;
                if (rb != null)
                {
                    // If target is part of a building chunk that is kinematic, awaken it
                    BuildingChunk chunk = hit.collider.GetComponent<BuildingChunk>();
                    if (chunk != null)
                    {
                        chunk.DetachAndActivatePhysics();
                    }

                    grabbedRigidbody = rb;
                    grabbedCollider = hit.collider;
                    grabbedRigidbody.useGravity = false;
                    grabbedRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
                }
            }
        }

        private void ThrowObject()
        {
            if (grabbedRigidbody == null) return;

            UnityEngine.Camera cam = UnityEngine.Camera.main;
            Vector3 throwDir = cam != null ? cam.transform.forward : transform.forward;

            grabbedRigidbody.useGravity = true;
            grabbedRigidbody.linearVelocity = throwDir * throwForce;

            if (throwSound != null) AudioSource.PlayClipAtPoint(throwSound, transform.position);

            grabbedRigidbody = null;
            grabbedCollider = null;

            if (ChaosManager.Instance != null)
            {
                ChaosManager.Instance.AddChaos(100, "SuperThrow");
            }
        }

        private void CrushObject()
        {
            if (grabbedRigidbody == null) return;

            Vector3 crushPos = grabbedRigidbody.position;

            // Spawn scrap burst particles
            if (crushScrapFXPrefab != null)
            {
                Instantiate(crushScrapFXPrefab, crushPos, Quaternion.identity);
            }

            if (crushSound != null) AudioSource.PlayClipAtPoint(crushSound, crushPos);

            // Destroy original object and award massive chaos
            Destroy(grabbedRigidbody.gameObject);
            grabbedRigidbody = null;
            grabbedCollider = null;

            if (ChaosManager.Instance != null)
            {
                ChaosManager.Instance.AddChaos(350, "SuperCrush");
            }
        }

        private void ReleaseGrabbedObject()
        {
            if (grabbedRigidbody == null) return;
            grabbedRigidbody.useGravity = true;
            grabbedRigidbody = null;
            grabbedCollider = null;
        }
    }
}
