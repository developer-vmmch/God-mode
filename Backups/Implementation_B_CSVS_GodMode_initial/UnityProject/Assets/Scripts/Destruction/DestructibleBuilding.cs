using System;
using System.Collections.Generic;
using UnityEngine;
using CSVS.GodMode.Core;

namespace CSVS.GodMode.Destruction
{
    public class DestructibleBuilding : MonoBehaviour, IDamageable
    {
        [Header("Building Structural Specs")]
        [SerializeField] private float _maxHealth = 2000.0f;
        [SerializeField] private long _chaosValue = 25000;
        [SerializeField] private int _totalFloors = 8;
        [SerializeField] private float _floorHeight = 4.0f;

        [Header("Fracture Slices")]
        [SerializeField] private GameObject _intactModel;
        [SerializeField] private GameObject _fractureRoot;
        [SerializeField] private GameObject[] _floorSections;

        private float _currentHealth;
        private bool _isCollapsed = false;

        public float CurrentHealth => _currentHealth;
        public float MaxHealth => _maxHealth;
        public bool IsDestroyed => _isCollapsed;

        public event Action<DestructibleBuilding> OnBuildingCollapsed;

        private void Awake()
        {
            _currentHealth = _maxHealth;

            if (_fractureRoot != null)
            {
                _fractureRoot.SetActive(false);
            }
        }

        public void TakeDamage(float amount, DamageType damageType, Vector3 hitPoint, Vector3 hitNormal)
        {
            if (_isCollapsed) return;

            _currentHealth -= amount;

            // Partial Floor Fracture Trigger at 50% health
            if (_currentHealth <= _maxHealth * 0.5f && _intactModel != null && _intactModel.activeSelf)
            {
                InitiatePartialFracture(hitPoint);
            }

            // Total Structural Collapse at 0 health
            if (_currentHealth <= 0.0f)
            {
                CollapseEntireBuilding(hitPoint, hitNormal);
            }
        }

        private void InitiatePartialFracture(Vector3 epicenter)
        {
            if (_intactModel != null) _intactModel.SetActive(false);
            if (_fractureRoot != null) _fractureRoot.SetActive(true);

            // Calculate floor index near epicenter
            float relativeY = epicenter.y - transform.position.y;
            int floorIndex = Mathf.Clamp(Mathf.FloorToInt(relativeY / _floorHeight), 0, _totalFloors - 1);

            ActivateFloorRigidbodies(floorIndex);
        }

        public void CollapseEntireBuilding(Vector3 epicenter, Vector3 impulseNormal)
        {
            if (_isCollapsed) return;
            _isCollapsed = true;

            if (_intactModel != null) _intactModel.SetActive(false);
            if (_fractureRoot != null) _fractureRoot.SetActive(true);

            // Activate all floor rigidbodies
            for (int i = 0; i < _totalFloors; i++)
            {
                ActivateFloorRigidbodies(i);
            }

            // Award Chaos destruction score
            if (ChaosSystem.Instance != null)
            {
                ChaosSystem.Instance.AddChaos(_chaosValue, gameObject.name);
            }

            OnBuildingCollapsed?.Invoke(this);
        }

        private void ActivateFloorRigidbodies(int floorIndex)
        {
            if (_floorSections == null || floorIndex >= _floorSections.Length || floorIndex < 0) return;

            GameObject floorObj = _floorSections[floorIndex];
            if (floorObj == null) return;

            Rigidbody[] rbs = floorObj.GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rb in rbs)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
                rb.AddExplosionForce(500.0f, transform.position + Vector3.up * (floorIndex * _floorHeight), 15.0f, 1.0f, ForceMode.Impulse);

                if (rb.GetComponent<BuildingFractureChunk>() == null)
                {
                    rb.gameObject.AddComponent<BuildingFractureChunk>();
                }
            }
        }
    }
}
