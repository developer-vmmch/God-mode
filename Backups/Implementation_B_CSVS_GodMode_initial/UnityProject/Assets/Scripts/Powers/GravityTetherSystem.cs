using UnityEngine;
using UnityEngine.InputSystem;

namespace CSVS.GodMode.Powers
{
    public class GravityTetherSystem : MonoBehaviour
    {
        [Header("Tether Dynamics")]
        [SerializeField] private float _tetherRange = 100.0f;
        [SerializeField] private float _tetherSpringForce = 250.0f;
        [SerializeField] private float _tetherDamping = 15.0f;
        [SerializeField] private LineRenderer _tetherBeam;
        [SerializeField] private LayerMask _tetherMask = ~0;

        private Camera _mainCamera;
        private Rigidbody _tetheredBody;
        private Vector3 _currentAnchorPoint;

        public bool IsTethering => _tetheredBody != null;

        private void Start()
        {
            _mainCamera = Camera.main;
            if (_tetherBeam != null) _tetherBeam.enabled = false;
        }

        private void Update()
        {
            if (_mainCamera == null) _mainCamera = Camera.main;
            if (Keyboard.current == null) return;

            // Q key hold initiates tether
            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                TryAttachTether();
            }
            else if (Keyboard.current.qKey.wasReleasedThisFrame)
            {
                ReleaseTether();
            }

            UpdateTetherVisuals();
        }

        private void FixedUpdate()
        {
            if (_tetheredBody == null || _mainCamera == null) return;

            // Ray anchor moves with camera aim
            Ray ray = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
            _currentAnchorPoint = ray.origin + ray.direction * 25.0f;

            // Spring Force: F_tether = k_tether * (p_anchor - p_obj) - damping * v_obj
            Vector3 delta = _currentAnchorPoint - _tetheredBody.position;
            Vector3 springForce = delta * _tetherSpringForce;
            Vector3 dampingForce = _tetheredBody.linearVelocity * _tetherDamping;

            Vector3 totalForce = springForce - dampingForce;
            _tetheredBody.AddForce(totalForce, ForceMode.Acceleration);
        }

        private void TryAttachTether()
        {
            if (_mainCamera == null) return;

            Ray ray = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
            if (Physics.Raycast(ray, out RaycastHit hit, _tetherRange, _tetherMask))
            {
                Rigidbody rb = hit.rigidbody;
                if (rb != null && !rb.isKinematic)
                {
                    _tetheredBody = rb;
                    _tetheredBody.useGravity = false;
                    _currentAnchorPoint = hit.point;

                    if (_tetherBeam != null) _tetherBeam.enabled = true;
                }
            }
        }

        public void ReleaseTether()
        {
            if (_tetheredBody != null)
            {
                _tetheredBody.useGravity = true;

                // Fling impulse in direction of camera look
                if (_mainCamera != null)
                {
                    _tetheredBody.linearVelocity += _mainCamera.transform.forward * 20.0f;
                }

                _tetheredBody = null;
            }

            if (_tetherBeam != null) _tetherBeam.enabled = false;
        }

        private void UpdateTetherVisuals()
        {
            if (_tetherBeam == null || !IsTethering) return;

            _tetherBeam.SetPosition(0, transform.position + transform.right * 0.4f);
            _tetherBeam.SetPosition(1, _tetheredBody.position);
        }
    }
}
