using UnityEngine;
using CSVS.GodMode.Destruction;

namespace CSVS.GodMode.Environment
{
    public class CityGenerator : MonoBehaviour
    {
        [Header("Grid Layout")]
        [SerializeField] private int _gridWidth = 6;
        [SerializeField] private int _gridLength = 6;
        [SerializeField] private float _blockSpacing = 45.0f;
        [SerializeField] private float _buildingHeightScale = 50.0f;

        [Header("Materials & Colors")]
        [SerializeField] private Material _buildingMaterial;
        [SerializeField] private Material _roadMaterial;

        private void Start()
        {
            GenerateCitySandbox();
        }

        public void GenerateCitySandbox()
        {
            // Create Ground / Road Plane
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "CityGroundPlane";
            ground.transform.position = Vector3.zero;
            ground.transform.localScale = new Vector3(_gridWidth * 6.0f, 1.0f, _gridLength * 6.0f);

            // Generate Skyscraper Blocks
            for (int x = -_gridWidth / 2; x <= _gridWidth / 2; x++)
            {
                for (int z = -_gridLength / 2; z <= _gridLength / 2; z++)
                {
                    // Leave central plaza / streets open
                    if (x == 0 && z == 0) continue;

                    Vector3 blockPosition = new Vector3(x * _blockSpacing, 0.0f, z * _blockSpacing);
                    CreateDestructibleBuilding(blockPosition, x, z);
                }
            }
        }

        private void CreateDestructibleBuilding(Vector3 position, int x, int z)
        {
            GameObject buildingObj = new GameObject($"Building_{x}_{z}");
            buildingObj.transform.position = position;

            float height = Random.Range(35.0f, _buildingHeightScale);
            int floors = Mathf.FloorToInt(height / 4.0f);

            // Intact Root & Box Collider
            BoxCollider col = buildingObj.AddComponent<BoxCollider>();
            col.size = new Vector3(18.0f, height, 18.0f);
            col.center = new Vector3(0.0f, height * 0.5f, 0.0f);

            GameObject intactModel = GameObject.CreatePrimitive(PrimitiveType.Cube);
            intactModel.name = "IntactModel";
            intactModel.transform.SetParent(buildingObj.transform, false);
            intactModel.transform.localPosition = new Vector3(0.0f, height * 0.5f, 0.0f);
            intactModel.transform.localScale = new Vector3(18.0f, height, 18.0f);
            Destroy(intactModel.GetComponent<Collider>()); // Use parent collider

            // Pre-fractured floor slices
            GameObject fractureRoot = new GameObject("FractureRoot");
            fractureRoot.transform.SetParent(buildingObj.transform, false);

            GameObject[] floorSlices = new GameObject[floors];
            for (int f = 0; f < floors; f++)
            {
                GameObject floorObj = new GameObject($"Floor_{f}");
                floorObj.transform.SetParent(fractureRoot.transform, false);
                floorObj.transform.localPosition = new Vector3(0.0f, f * 4.0f + 2.0f, 0.0f);

                // Add 4 chunk blocks per floor
                for (int cx = -1; cx <= 1; cx += 2)
                {
                    for (int cz = -1; cz <= 1; cz += 2)
                    {
                        GameObject chunk = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        chunk.name = $"Chunk_{cx}_{cz}";
                        chunk.transform.SetParent(floorObj.transform, false);
                        chunk.transform.localPosition = new Vector3(cx * 4.5f, 0.0f, cz * 4.5f);
                        chunk.transform.localScale = new Vector3(8.5f, 3.8f, 8.5f);

                        Rigidbody rb = chunk.AddComponent<Rigidbody>();
                        rb.isKinematic = true;
                        rb.mass = 250.0f;
                    }
                }

                floorSlices[f] = floorObj;
            }

            fractureRoot.SetActive(false);

            // Attach DestructibleBuilding component
            DestructibleBuilding destructible = buildingObj.AddComponent<DestructibleBuilding>();
            // Configured via serialized layout
        }
    }
}
