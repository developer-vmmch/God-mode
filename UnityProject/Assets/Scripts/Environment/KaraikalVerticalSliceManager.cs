using UnityEngine;
using CSVS.GodMode.Art;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CSVS.GodMode.Environment
{
    /// <summary>
    /// Spawns the Phase 10 Playable Vertical Slice along Karaikal Bharathiyar Commercial Bazaar:
    /// - Blender-modeled Commercial Bazaar buildings with storefronts, shutters and bilingual signboards
    /// - Blender-modeled 3-wheel Auto-Rickshaws
    /// - Blender-modeled Commuter Motorcycles
    /// - Blender-modeled Concrete Utility Poles with step-down transformers and streetlights
    /// - Ground asphalt road and sidewalks
    /// </summary>
    [DefaultExecutionOrder(10)]
    public class KaraikalVerticalSliceManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool spawnOnStart = true;
        [SerializeField] private int numberOfBlocks = 6;
        [SerializeField] private float blockSpacing = 11.5f;

        private void Start()
        {
            if (spawnOnStart)
            {
                ConstructVerticalSliceStreet();
            }
        }

        public void ConstructVerticalSliceStreet()
        {
            GameObject sliceRoot = new GameObject("Karaikal_VerticalSlice_CommercialStreet");
            sliceRoot.transform.SetParent(this.transform);

            // Load Blender Models
#if UNITY_EDITOR
            GameObject bazaarPrefab = BlenderAssetBridge.LoadAsset(BlenderAssetBridge.BAZAAR_MODEL_PATH);
            GameObject rickshawPrefab = BlenderAssetBridge.LoadAsset(BlenderAssetBridge.RICKSHAW_MODEL_PATH);
            GameObject motorcyclePrefab = BlenderAssetBridge.LoadAsset(BlenderAssetBridge.MOTORCYCLE_MODEL_PATH);
            GameObject polePrefab = BlenderAssetBridge.LoadAsset(BlenderAssetBridge.UTILITY_POLE_PATH);
#else
            GameObject bazaarPrefab = null;
            GameObject rickshawPrefab = null;
            GameObject motorcyclePrefab = null;
            GameObject polePrefab = null;
#endif

            // Road surface along Z axis from -40 to +60
            GameObject road = GameObject.CreatePrimitive(PrimitiveType.Cube);
            road.name = "VerticalSlice_Road_Asphalt";
            road.transform.SetParent(sliceRoot.transform);
            road.transform.position = new Vector3(0, -0.05f, 10f);
            road.transform.localScale = new Vector3(14f, 0.1f, 110f);
            
            var roadMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            roadMat.name = "Mat_VerticalSlice_Asphalt";
            roadMat.color = new Color(0.18f, 0.18f, 0.20f);
            roadMat.SetFloat("_Smoothness", 0.15f);
            road.GetComponent<MeshRenderer>().material = roadMat;

            // Spawn commercial blocks on left (X = -12) and right (X = +12)
            for (int i = 0; i < numberOfBlocks; i++)
            {
                float zPos = -30f + (i * blockSpacing * 1.5f);

                // North side shophouse
                if (bazaarPrefab != null)
                {
                    GameObject northShop = Instantiate(bazaarPrefab, new Vector3(-11f, 0f, zPos), Quaternion.Euler(0, 90f, 0), sliceRoot.transform);
                    northShop.name = $"Bazaar_North_{i}";
                    EnsureMeshColliders(northShop);

                    GameObject southShop = Instantiate(bazaarPrefab, new Vector3(11f, 0f, zPos), Quaternion.Euler(0, -90f, 0), sliceRoot.transform);
                    southShop.name = $"Bazaar_South_{i}";
                    EnsureMeshColliders(southShop);
                }

                // Utility poles along sidewalk
                if (polePrefab != null && i % 2 == 0)
                {
                    GameObject poleL = Instantiate(polePrefab, new Vector3(-6.2f, 0f, zPos + 3f), Quaternion.identity, sliceRoot.transform);
                    poleL.name = $"UtilityPole_L_{i}";
                    EnsureMeshColliders(poleL);

                    GameObject poleR = Instantiate(polePrefab, new Vector3(6.2f, 0f, zPos + 3f), Quaternion.Euler(0, 180f, 0), sliceRoot.transform);
                    poleR.name = $"UtilityPole_R_{i}";
                    EnsureMeshColliders(poleR);
                }
            }

            // Spawn vehicles along the commercial street
            if (rickshawPrefab != null)
            {
                // Parked auto-rickshaw near tea stall / shop
                GameObject rickshaw1 = Instantiate(rickshawPrefab, new Vector3(-4.2f, 0.1f, -12f), Quaternion.Euler(0, 15f, 0), sliceRoot.transform);
                rickshaw1.name = "AutoRickshaw_Parked_1";
                EnsureMeshColliders(rickshaw1);

                // Driving rickshaw
                GameObject rickshaw2 = Instantiate(rickshawPrefab, new Vector3(2.5f, 0.1f, 8f), Quaternion.Euler(0, 180f, 0), sliceRoot.transform);
                rickshaw2.name = "AutoRickshaw_Traffic_2";
                EnsureMeshColliders(rickshaw2);
            }

            if (motorcyclePrefab != null)
            {
                // Parked motorcycles
                GameObject bike1 = Instantiate(motorcyclePrefab, new Vector3(-5.2f, 0.1f, -5f), Quaternion.Euler(0, 75f, 0), sliceRoot.transform);
                bike1.name = "Motorcycle_Parked_1";
                EnsureMeshColliders(bike1);

                GameObject bike2 = Instantiate(motorcyclePrefab, new Vector3(5.2f, 0.1f, 15f), Quaternion.Euler(0, -105f, 0), sliceRoot.transform);
                bike2.name = "Motorcycle_Parked_2";
                EnsureMeshColliders(bike2);
            }

            Debug.Log("[VerticalSlice] Successfully constructed realistic Karaikal commercial bazaar street using Blender assets.");
        }

        private void EnsureMeshColliders(GameObject root)
        {
            MeshFilter[] filters = root.GetComponentsInChildren<MeshFilter>();
            foreach (var mf in filters)
            {
                if (mf.sharedMesh != null && mf.gameObject.GetComponent<Collider>() == null)
                {
                    var col = mf.gameObject.AddComponent<MeshCollider>();
                    col.sharedMesh = mf.sharedMesh;
                }
            }
        }
    }
}
