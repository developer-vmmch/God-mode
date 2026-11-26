using System.Collections.Generic;
using UnityEngine;

namespace CSVS.GodMode.Core
{
    [System.Serializable]
    public class PoolItem
    {
        public string tag;
        public GameObject prefab;
        public int size = 20;
        public bool expandable = true;
    }

    /// <summary>
    /// High-performance object pooler for debris, impacts, and particle bursts.
    /// Prevents runtime GC allocations and heap churn on target hardware.
    /// </summary>
    public class ObjectPooler : MonoBehaviour
    {
        public static ObjectPooler Instance { get; private set; }

        [SerializeField] private List<PoolItem> itemsToPool = new List<PoolItem>();
        private Dictionary<string, Queue<GameObject>> poolDictionary;
        private Dictionary<string, PoolItem> itemConfigDictionary;

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

        private void InitializePools()
        {
            poolDictionary = new Dictionary<string, Queue<GameObject>>();
            itemConfigDictionary = new Dictionary<string, PoolItem>();

            foreach (PoolItem item in itemsToPool)
            {
                if (item.prefab == null) continue;

                Queue<GameObject> objectPool = new Queue<GameObject>();
                for (int i = 0; i < item.size; i++)
                {
                    GameObject obj = Instantiate(item.prefab, transform);
                    obj.SetActive(false);
                    objectPool.Enqueue(obj);
                }

                poolDictionary[item.tag] = objectPool;
                itemConfigDictionary[item.tag] = item;
            }
        }

        public GameObject SpawnFromPool(string poolTag, Vector3 position, Quaternion rotation)
        {
            if (poolDictionary == null || !poolDictionary.ContainsKey(poolTag))
            {
                return null;
            }

            Queue<GameObject> pool = poolDictionary[poolTag];

            if (pool.Count == 0)
            {
                if (itemConfigDictionary.ContainsKey(poolTag) && itemConfigDictionary[poolTag].expandable)
                {
                    GameObject newObj = Instantiate(itemConfigDictionary[poolTag].prefab, transform);
                    newObj.transform.position = position;
                    newObj.transform.rotation = rotation;
                    newObj.SetActive(true);
                    return newObj;
                }
                return null;
            }

            GameObject objToSpawn = pool.Dequeue();
            if (objToSpawn == null) return null;

            objToSpawn.transform.position = position;
            objToSpawn.transform.rotation = rotation;
            objToSpawn.SetActive(true);

            return objToSpawn;
        }

        public void ReturnToPool(string poolTag, GameObject obj)
        {
            if (obj == null) return;

            obj.SetActive(false);
            obj.transform.SetParent(transform);

            if (poolDictionary != null && poolDictionary.ContainsKey(poolTag))
            {
                poolDictionary[poolTag].Enqueue(obj);
            }
            else
            {
                Destroy(obj);
            }
        }
    }
}
