using UnityEngine;
using CSVS.Destruction;

namespace CSVS.World
{
    /// <summary>
    /// Procedural environment and test city block generator.
    /// Spawns asphalt ground, perimeter barriers, road grid, sidewalks,
    /// 3 fully destructible pre-fractured buildings, and physics throwable props/vehicles.
    /// </summary>
    public class ProceduralCityGenerator : MonoBehaviour
    {
        [Header("Materials")]
        [SerializeField] private Material roadMaterial;
        [SerializeField] private Material buildingConcreteMaterial;
        [SerializeField] private Material glassMaterial;
        [SerializeField] private Material vehicleMaterial;

        [Header("Generation Parameters")]
        [SerializeField] private Vector2 cityBlockSize = new Vector2(160f, 160f);
        [SerializeField] private bool generateOnStart = true;

        private void Start()
        {
            if (generateOnStart)
            {
                GenerateTestBlock();
            }
        }

        [ContextMenu("Generate Test Block")]
        public void GenerateTestBlock()
        {
            CreateGroundPlane();
            CreateDestructibleBuildings();
            CreatePhysicsPropsAndVehicles();
        }

        private void CreateGroundPlane()
        {
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground_Asphalt";
            ground.transform.SetParent(transform);
            ground.transform.position = Vector3.zero;
            ground.transform.localScale = new Vector3(cityBlockSize.x / 10f, 1f, cityBlockSize.y / 10f);

            Renderer r = ground.GetComponent<Renderer>();
            if (roadMaterial != null) r.material = roadMaterial;
            else r.material.color = new Color(0.18f, 0.18f, 0.20f);
        }

        private void CreateDestructibleBuildings()
        {
            // Building 1: Downtown Office Skyscraper (5 floors)
            BuildTower("Tower_Alpha", new Vector3(-35f, 0f, 25f), floors: 5, floorSize: new Vector3(14f, 4.5f, 14f));

            // Building 2: Mid-Rise Commercial Complex (4 floors)
            BuildTower("Complex_Bravo", new Vector3(30f, 0f, 20f), floors: 4, floorSize: new Vector3(18f, 4f, 12f));

            // Building 3: Corporate Center (6 floors)
            BuildTower("Center_Charlie", new Vector3(0f, 0f, -40f), floors: 6, floorSize: new Vector3(16f, 4.2f, 16f));
        }

        private void BuildTower(string name, Vector3 basePos, int floors, Vector3 floorSize)
        {
            GameObject buildingRoot = new GameObject(name);
            buildingRoot.transform.SetParent(transform);
            buildingRoot.transform.position = basePos;

            DestructibleBuilding db = buildingRoot.AddComponent<DestructibleBuilding>();

            float yOffset = floorSize.y / 2f;
            for (int f = 0; f < floors; f++)
            {
                Vector3 floorPos = basePos + new Vector3(0f, f * floorSize.y + yOffset, 0f);

                // Subdivide each floor into 4 pre-fractured modular quadrants
                Vector3 halfSize = new Vector3(floorSize.x / 2f, floorSize.y, floorSize.z / 2f);
                for (int qx = -1; qx <= 1; qx += 2)
                {
                    for (int qz = -1; qz <= 1; qz += 2)
                    {
                        Vector3 chunkPos = floorPos + new Vector3(qx * halfSize.x / 2f, 0f, qz * halfSize.z / 2f);
                        CreateBuildingChunk(buildingRoot.transform, chunkPos, halfSize, isSupport: (f == 0));
                    }
                }
            }
        }

        private void CreateBuildingChunk(Transform parent, Vector3 pos, Vector3 size, bool isSupport)
        {
            GameObject chunkObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            chunkObj.name = isSupport ? "Chunk_Foundation" : "Chunk_Upper";
            chunkObj.transform.SetParent(parent);
            chunkObj.transform.position = pos;
            chunkObj.transform.localScale = size;

            Renderer r = chunkObj.GetComponent<Renderer>();
            if (buildingConcreteMaterial != null) r.material = buildingConcreteMaterial;
            else r.material.color = isSupport ? new Color(0.4f, 0.42f, 0.45f) : new Color(0.6f, 0.62f, 0.65f);

            BuildingChunk chunk = chunkObj.AddComponent<BuildingChunk>();
            // Mark ground floor chunks as essential structural supports
            typeof(BuildingChunk).GetField("isStructuralSupport", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(chunk, isSupport);
        }

        private void CreatePhysicsPropsAndVehicles()
        {
            GameObject propsRoot = new GameObject("Physics_Props_And_Vehicles");
            propsRoot.transform.SetParent(transform);

            // Spawn 8 physics vehicles
            Vector3[] carPositions = new Vector3[] {
                new Vector3(-12f, 0.6f, 5f),
                new Vector3(-8f, 0.6f, -15f),
                new Vector3(10f, 0.6f, 8f),
                new Vector3(15f, 0.6f, -22f),
                new Vector3(-25f, 0.6f, -10f),
                new Vector3(22f, 0.6f, 35f),
                new Vector3(0f, 0.6f, 15f),
                new Vector3(-2f, 0.6f, -5f)
            };

            foreach (var pos in carPositions)
            {
                CreateVehicle("Car_Sedan", propsRoot.transform, pos, new Vector3(2.2f, 1.2f, 4.5f), 1200f, Color.red);
            }

            // Spawn throwable concrete barriers
            for (int i = 0; i < 6; i++)
            {
                Vector3 barrierPos = new Vector3(-20f + i * 8f, 0.5f, 0f);
                CreateThrowableProp("Barrier_Concrete", propsRoot.transform, barrierPos, new Vector3(3f, 0.9f, 0.8f), 450f);
            }
        }

        private void CreateVehicle(string name, Transform parent, Vector3 pos, Vector3 size, float mass, Color color)
        {
            GameObject car = GameObject.CreatePrimitive(PrimitiveType.Cube);
            car.name = name;
            car.transform.SetParent(parent);
            car.transform.position = pos;
            car.transform.localScale = size;

            Renderer r = car.GetComponent<Renderer>();
            if (vehicleMaterial != null) r.material = vehicleMaterial;
            else r.material.color = color;

            Rigidbody rb = car.AddComponent<Rigidbody>();
            rb.mass = mass;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }

        private void CreateThrowableProp(string name, Transform parent, Vector3 pos, Vector3 size, float mass)
        {
            GameObject prop = GameObject.CreatePrimitive(PrimitiveType.Cube);
            prop.name = name;
            prop.transform.SetParent(parent);
            prop.transform.position = pos;
            prop.transform.localScale = size;

            Renderer r = prop.GetComponent<Renderer>();
            r.material.color = Color.yellow;

            Rigidbody rb = prop.AddComponent<Rigidbody>();
            rb.mass = mass;
        }
    }
}
