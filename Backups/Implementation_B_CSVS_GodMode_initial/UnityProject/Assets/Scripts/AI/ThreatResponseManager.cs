using UnityEngine;
using CSVS.GodMode.Core;

namespace CSVS.GodMode.AI
{
    public class ThreatResponseManager : MonoBehaviour
    {
        [Header("Enemy Prefab Spawning")]
        [SerializeField] private GameObject _policePrefab;
        [SerializeField] private GameObject _swatPrefab;
        [SerializeField] private GameObject _tankPrefab;
        [SerializeField] private GameObject _helicopterPrefab;

        [Header("Spawn Parameters")]
        [SerializeField] private float _spawnRadius = 80.0f;
        [SerializeField] private float _helicopterAltitude = 40.0f;
        [SerializeField] private Transform _playerTransform;

        private void Start()
        {
            if (_playerTransform == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) _playerTransform = player.transform;
            }

            if (ChaosSystem.Instance != null)
            {
                ChaosSystem.Instance.OnWantedTierEscalated += HandleWantedEscalation;
            }
        }

        private void OnDestroy()
        {
            if (ChaosSystem.Instance != null)
            {
                ChaosSystem.Instance.OnWantedTierEscalated -= HandleWantedEscalation;
            }
        }

        private void HandleWantedEscalation(WantedTier newTier)
        {
            if (_playerTransform == null) return;

            switch (newTier)
            {
                case WantedTier.Police:
                    SpawnGroundSquad(_policePrefab, 3);
                    break;
                case WantedTier.Swat:
                    SpawnGroundSquad(_swatPrefab, 2);
                    break;
                case WantedTier.MilitaryTanks:
                    SpawnGroundSquad(_tankPrefab, 2);
                    break;
                case WantedTier.AttackHelicopters:
                    SpawnAirGunship(_helicopterPrefab, 2);
                    break;
            }
        }

        private void SpawnGroundSquad(GameObject prefab, int count)
        {
            if (prefab == null || _playerTransform == null) return;

            for (int i = 0; i < count; i++)
            {
                Vector2 randomCircle = Random.insideUnitCircle.normalized * _spawnRadius;
                Vector3 spawnPos = _playerTransform.position + new Vector3(randomCircle.x, 0.5f, randomCircle.y);

                GameObject unit = Instantiate(prefab, spawnPos, Quaternion.identity);
                unit.name = $"{prefab.name}_{i + 1}";
            }
        }

        private void SpawnAirGunship(GameObject prefab, int count)
        {
            if (prefab == null || _playerTransform == null) return;

            for (int i = 0; i < count; i++)
            {
                Vector2 randomCircle = Random.insideUnitCircle.normalized * (_spawnRadius * 1.2f);
                Vector3 spawnPos = _playerTransform.position + new Vector3(randomCircle.x, _helicopterAltitude, randomCircle.y);

                GameObject gunship = Instantiate(prefab, spawnPos, Quaternion.identity);
                gunship.name = $"Gunship_{i + 1}";
            }
        }
    }
}
