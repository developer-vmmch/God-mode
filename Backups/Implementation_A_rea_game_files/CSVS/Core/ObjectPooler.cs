using System.Collections.Generic;
using UnityEngine;

namespace CSVS.Core
{
    /// <summary>
    /// High-performance generic object pooler for debris, particle bursts, and audio nodes.
    /// </summary>
    public class ObjectPooler : MonoBehaviour
    {
        public static ObjectPooler Instance { get; private set; }

        [System.Serializable]
        public class Pool
        {
            public string tag;
            public GameObject prefab;
            public int size = 20;
        }

        [SerializeField] private List<Pool> pools = new List<Pool>();
        private Dictionary<string, Queue<GameObject>> poolDictionary;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            InitializePools();
        }

        public void InitializePools()
        {
            poolDictionary = new Dictionary<string, Queue<GameObject>>();

            foreach (var pool in pools)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();

                for (int i = 0; i < pool.size; i++)
                {
                    if (pool.prefab == null) continue;
                    GameObject obj = Instantiate(pool.prefab, transform);
                    obj.SetActive(false);
                    objectPool.Enqueue(obj);
                }

                poolDictionary[pool.tag] = objectPool;
            }
        }

        public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning($"Pool with tag '{tag}' doesn't exist.");
                return null;
            }

            Queue<GameObject> objectPool = poolDictionary[tag];
            if (objectPool.Count == 0)
            {
                return null;
            }

            GameObject objToSpawn = objectPool.Dequeue();
            objToSpawn.transform.position = position;
            objToSpawn.transform.rotation = rotation;
            objToSpawn.SetActive(true);

            objectPool.Enqueue(objToSpawn);
            return objToSpawn;
        }

        public void RegisterDynamicPool(string tag, GameObject prefab, int initialCount)
        {
            if (poolDictionary == null)
            {
                poolDictionary = new Dictionary<string, Queue<GameObject>>();
            }

            if (poolDictionary.ContainsKey(tag)) return;

            Queue<GameObject> queue = new Queue<GameObject>();
            for (int i = 0; i < initialCount; i++)
            {
                GameObject obj = Instantiate(prefab, transform);
                obj.SetActive(false);
                queue.Enqueue(obj);
            }
            poolDictionary[tag] = queue;
        }
    }
}
