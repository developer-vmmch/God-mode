using System;
using UnityEngine;
using UnityEngine.InputSystem;
using CSVS.GodMode.Destruction;

namespace CSVS.GodMode.Powers
{
    public class SuperStrengthGrab : MonoBehaviour
    {
        [Header("Grab Configuration")]
        [SerializeField] private float _grabRange = 12.0f;
        [SerializeField] private Transform _holdSocket;
        [SerializeField] private float _springStrength = 300.0f;
        [SerializeField] private float _damping = 25.0f;
        [SerializeField] private LayerMask _grabbableMask = ~0;

        [Header("Throw & Crush Dynamics")]
        [SerializeField] private float _throwForce = 60.0f;
        [SerializeField] private float _crushDamage = 1000.0f;

        private Camera _mainCamera;
        private Rigidbody _grabbedRigidbody;
        private ConfigurableJoint _joint;

        public bool IsHoldingObject => _grabbedRigidbody != null;
        public Rigidbody GrabbedObject => _grabbedRigidbody;

        public event Action<GameObject> OnObjectGrabbed;
        public event Action<GameObject, Vector3> OnObjectThrown;
        public event Action<GameObject> OnObjectCrushed;

        private void Start()
        {
            _mainCamera = Camera.main;

            if (_holdSocket == null)
            {
                GameObject socketObj = new GameObject("HoldSocket");
                socketObj.transform.SetParent(transform);
                socketObj.transform.localPosition = new Vector3(0.0f, 2.5f, 2.0f);
                _holdSocket = socketObj.transform;
            }
        }

        private void Update()
        {
            if (_mainCamera == null) _mainCamera = Camera.main;
            if (Keyboard.current == null || Mouse.current == null) return;

            // E key toggles Grab / Release
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                if (IsHoldingObject)
                {
                    ReleaseGrabbedObject();
                }
                else
                {
                    TryGrabTarget();
                }
            }

            if (IsHoldingObject)
            {
                // Left Click = Throw forward
                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    ThrowObject();
                }
                // Right Click = Crush in hands
                else if (Mouse.current.rightButton.wasPressedThisFrame)
                {
                    CrushObject();
                }
            }
        }

        public bool TryGrabTarget()
        {
            if (_mainCamera == null) return false;

            Ray ray = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
            if (Physics.Raycast(ray, out RaycastHit hit, _grabRange, _grabbableMask))
            {
                Rigidbody rb = hit.rigidbody;
                if (rb != null && !rb.isKinematic)
                {
                    AttachObject(rb);
                    return true;
                }
            }
            return false;
        }

        private void AttachObject(Rigidbody targetRb)
        {
            _grabbedRigidbody = targetRb;
            _grabbedRigidbody.useGravity = false;
            _grabbedRigidbody.linearDamping = 10.0f;

            // Create soft physics spring joint to hold socket
            _joint = gameObject.AddComponent<ConfigurableJoint>();
            _joint.connectedBody = _grabbedRigidbody;
            _joint.autoConfigureConnectedAnchor = false;
            _joint.anchor = transform.InverseTransformPoint(_holdSocket.position);
            _joint.connectedAnchor = Vector3.zero;

            JointDrive drive = new JointDrive
            {
                positionSpring = _springStrength,
                positionDamper = _damping,
                maximumForce = Mathf.Infinity
            };

            _joint.xDrive = drive;
            _joint.yDrive = drive;
            _joint.zDrive = drive;

            OnObjectGrabbed?.Invoke(_grabbedRigidbody.gameObject);
        }

        public void ThrowObject()
        {
            if (!IsHoldingObject) return;

            Rigidbody rb = _grabbedRigidbody;
            ReleaseJointOnly();

            Vector3 throwDir = _mainCamera != null ? _mainCamera.transform.forward : transform.forward;
            rb.linearVelocity = throwDir * _throwForce;
            rb.AddTorque(UnityEngine.Random.insideUnitSphere * 15.0f, ForceMode.Impulse);

            OnObjectThrown?.Invoke(rb.gameObject, throwDir);
        }

        public void CrushObject()
        {
            if (!IsHoldingObject) return;

            GameObject obj = _grabbedRigidbody.gameObject;
            IDamageable damageable = obj.GetComponentInParent<IDamageable>();
            Vector3 crushPoint = obj.transform.position;

            ReleaseJointOnly();

            if (damageable != null)
            {
                damageable.TakeDamage(_crushDamage, DamageType.Crush, crushPoint, Vector3.up);
            }
            else
            {
                // If not damageable, apply high explosive damage or scale down
                obj.transform.localScale *= 0.4f;
                Rigidbody rb = obj.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = UnityEngine.Random.insideUnitSphere * 10.0f;
                }
            }

            OnObjectCrushed?.Invoke(obj);
        }

        public void ReleaseGrabbedObject()
        {
            if (!IsHoldingObject) return;
            ReleaseJointOnly();
        }

        private void ReleaseJointOnly()
        {
            if (_joint != null)
            {
                Destroy(_joint);
                _joint = null;
            }

            if (_grabbedRigidbody != null)
            {
                _grabbedRigidbody.useGravity = true;
                _grabbedRigidbody.linearDamping = 0.05f;
                _grabbedRigidbody = null;
            }
        }
    }
}
