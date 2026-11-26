using UnityEngine;
using CSVS.GodMode.Core;
using CSVS.GodMode.Destruction;
using CSVS.GodMode.Player;
using CSVS.GodMode.Powers;

namespace CSVS.GodMode.Environment
{
    /// <summary>
    /// Procedural sandbox city generator.
    /// Automatically builds road planes, Voronoi-fracturable skyscrapers with URP-compatible materials,
    /// and instantiates the superhero actor and camera if not already present.
    /// </summary>
    public class CityGenerator : MonoBehaviour
    {
        [Header("Grid Layout")]
        [SerializeField] private int _gridWidth = 6;
        [SerializeField] private int _gridLength = 6;
        [SerializeField] private float _blockSpacing = 45.0f;
        [SerializeField] private float _buildingHeightScale = 50.0f;

        [Header("Materials")]
        [SerializeField] private Material _buildingMaterial;
        [SerializeField] private Material _roadMaterial;
        [SerializeField] private Material _chunkMaterial;
        [SerializeField] private Material _heroMaterial;

        private void Awake()
        {
            EnsureMaterials();
        }

        private void Start()
        {
            GenerateCitySandbox();
            EnsurePlayerExists();
        }

        public void EnsureMaterials()
        {
            Shader uShader = Shader.Find("Universal Render Pipeline/Lit");
            if (uShader == null) uShader = Shader.Find("Universal Render Pipeline/Simple Lit");
            if (uShader == null) uShader = Shader.Find("Universal Render Pipeline/Unlit");
            if (uShader == null) uShader = Shader.Find("Standard");
            if (uShader == null) uShader = Shader.Find("Diffuse");

            if (_roadMaterial == null && uShader != null)
            {
                _roadMaterial = new Material(uShader);
                _roadMaterial.name = "Runtime_RoadMaterial";
                SetMaterialColor(_roadMaterial, new Color(0.18f, 0.20f, 0.22f, 1.0f));
                if (_roadMaterial.HasProperty("_Smoothness")) _roadMaterial.SetFloat("_Smoothness", 0.1f);
            }

            if (_buildingMaterial == null && uShader != null)
            {
                _buildingMaterial = new Material(uShader);
                _buildingMaterial.name = "Runtime_BuildingMaterial";
                SetMaterialColor(_buildingMaterial, new Color(0.72f, 0.76f, 0.80f, 1.0f));
                if (_buildingMaterial.HasProperty("_Smoothness")) _buildingMaterial.SetFloat("_Smoothness", 0.35f);
            }

            if (_chunkMaterial == null && uShader != null)
            {
                _chunkMaterial = new Material(uShader);
                _chunkMaterial.name = "Runtime_ChunkMaterial";
                SetMaterialColor(_chunkMaterial, new Color(0.58f, 0.54f, 0.50f, 1.0f));
                if (_chunkMaterial.HasProperty("_Smoothness")) _chunkMaterial.SetFloat("_Smoothness", 0.15f);
            }

            if (_heroMaterial == null && uShader != null)
            {
                _heroMaterial = new Material(uShader);
                _heroMaterial.name = "Runtime_HeroMaterial";
                SetMaterialColor(_heroMaterial, new Color(0.12f, 0.38f, 0.85f, 1.0f));
                if (_heroMaterial.HasProperty("_Smoothness")) _heroMaterial.SetFloat("_Smoothness", 0.65f);
            }
        }

        private void SetMaterialColor(Material mat, Color col)
        {
            if (mat.HasProperty("_BaseColor"))
                mat.SetColor("_BaseColor", col);
            else if (mat.HasProperty("_Color"))
                mat.SetColor("_Color", col);
        }

        public void GenerateCitySandbox()
        {
            EnsureMaterials();

            // Create Ground / Road Plane
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "CityGroundPlane";
            ground.transform.position = Vector3.zero;
            ground.transform.localScale = new Vector3(_gridWidth * 6.0f, 1.0f, _gridLength * 6.0f);
            ground.layer = LayerMask.NameToLayer("Default");

            Renderer groundRenderer = ground.GetComponent<Renderer>();
            if (groundRenderer != null && _roadMaterial != null)
            {
                groundRenderer.sharedMaterial = _roadMaterial;
            }

            // Generate Skyscraper Blocks
            for (int x = -_gridWidth / 2; x <= _gridWidth / 2; x++)
            {
                for (int z = -_gridLength / 2; z <= _gridLength / 2; z++)
                {
                    // Leave central plaza and main cross avenues open
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
            Destroy(intactModel.GetComponent<Collider>());

            Renderer intactRenderer = intactModel.GetComponent<Renderer>();
            if (intactRenderer != null && _buildingMaterial != null)
            {
                intactRenderer.sharedMaterial = _buildingMaterial;
            }

            // Pre-fractured floor slices
            GameObject fractureRoot = new GameObject("FractureRoot");
            fractureRoot.transform.SetParent(buildingObj.transform, false);

            GameObject[] floorSlices = new GameObject[floors];
            for (int f = 0; f < floors; f++)
            {
                GameObject floorObj = new GameObject($"Floor_{f}");
                floorObj.transform.SetParent(fractureRoot.transform, false);
                floorObj.transform.localPosition = new Vector3(0.0f, f * 4.0f + 2.0f, 0.0f);

                for (int cx = -1; cx <= 1; cx += 2)
                {
                    for (int cz = -1; cz <= 1; cz += 2)
                    {
                        GameObject chunk = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        chunk.name = $"Chunk_{cx}_{cz}";
                        chunk.transform.SetParent(floorObj.transform, false);
                        chunk.transform.localPosition = new Vector3(cx * 4.5f, 0.0f, cz * 4.5f);
                        chunk.transform.localScale = new Vector3(8.5f, 3.8f, 8.5f);

                        Renderer chunkRenderer = chunk.GetComponent<Renderer>();
                        if (chunkRenderer != null)
                        {
                            chunkRenderer.sharedMaterial = _chunkMaterial != null ? _chunkMaterial : _buildingMaterial;
                        }

                        Rigidbody rb = chunk.AddComponent<Rigidbody>();
                        rb.isKinematic = true;
                        rb.mass = 250.0f;
                    }
                }

                floorSlices[f] = floorObj;
            }

            fractureRoot.SetActive(false);

            DestructibleBuilding destructible = buildingObj.AddComponent<DestructibleBuilding>();
        }

        private void EnsurePlayerExists()
        {
            SuperheroFlightController existing = FindFirstObjectByType<SuperheroFlightController>();
            if (existing != null)
            {
                Renderer exRenderer = existing.GetComponent<Renderer>();
                if (exRenderer != null && _heroMaterial != null)
                {
                    exRenderer.sharedMaterial = _heroMaterial;
                }
                return;
            }

            // Spawn Superhero Character in central plaza
            GameObject hero = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            hero.name = "SuperheroPlayer";
            hero.tag = "Player";
            hero.transform.position = new Vector3(0.0f, 15.0f, 0.0f);

            Renderer heroRenderer = hero.GetComponent<Renderer>();
            if (heroRenderer != null && _heroMaterial != null)
            {
                heroRenderer.sharedMaterial = _heroMaterial;
            }

            Rigidbody rb = hero.GetComponent<Rigidbody>();
            if (rb == null) rb = hero.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.linearDamping = 4.0f;
            rb.angularDamping = 8.0f;

            SuperheroFlightController flight = hero.AddComponent<SuperheroFlightController>();
            hero.AddComponent<HeatVisionSystem>();
            hero.AddComponent<SuperStrengthGrab>();
            hero.AddComponent<GravityTetherSystem>();
            hero.AddComponent<SuperPunchSystem>();
            hero.AddComponent<GroundSlamSystem>();

            // Setup Camera
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                SuperheroCameraController camCtrl = mainCam.GetComponent<SuperheroCameraController>();
                if (camCtrl == null) camCtrl = mainCam.gameObject.AddComponent<SuperheroCameraController>();
                camCtrl.SetTarget(hero.transform);
            }
        }
    }
}
