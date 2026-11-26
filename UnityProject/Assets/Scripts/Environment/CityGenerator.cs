using System.Collections.Generic;
using UnityEngine;
using CSVS.GodMode.Core;
using CSVS.GodMode.Destruction;
using CSVS.GodMode.Player;
using CSVS.GodMode.Powers;

namespace CSVS.GodMode.Environment
{
    /// <summary>
    /// Master Open-World Generator for Karaikal, Puducherry, India.
    /// Replaces generic city blocks with geographically grounded Karaikal districts,
    /// authentic South Indian architecture, Kamarajar Salai (NH 32), Bharathiyar Road,
    /// Karaikal Beach, Arasalar River & Bridge, and modular destructible structures.
    /// </summary>
    public class CityGenerator : MonoBehaviour
    {
        [Header("Modular Systems")]
        [SerializeField] private KaraikalRoadNetwork _roadNetwork;
        [SerializeField] private ModularIndianBuildingSystem _buildingSystem;

        [Header("Materials")]
        [SerializeField] private Material _buildingMaterial;
        [SerializeField] private Material _roadMaterial;
        [SerializeField] private Material _chunkMaterial;
        [SerializeField] private Material _heroMaterial;

        private GameObject _environmentRoot;

        private void Awake()
        {
            if (_roadNetwork == null)
            {
                _roadNetwork = GetComponent<KaraikalRoadNetwork>();
                if (_roadNetwork == null) _roadNetwork = gameObject.AddComponent<KaraikalRoadNetwork>();
            }

            if (_buildingSystem == null)
            {
                _buildingSystem = GetComponent<ModularIndianBuildingSystem>();
                if (_buildingSystem == null) _buildingSystem = gameObject.AddComponent<ModularIndianBuildingSystem>();
            }

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
            if (uShader == null) uShader = Shader.Find("Standard");

            if (_roadMaterial == null && uShader != null)
            {
                _roadMaterial = new Material(uShader);
                _roadMaterial.name = "Runtime_RoadMaterial";
                SetMatColor(_roadMaterial, new Color(0.18f, 0.20f, 0.22f, 1.0f));
            }

            if (_buildingMaterial == null && uShader != null)
            {
                _buildingMaterial = new Material(uShader);
                _buildingMaterial.name = "Runtime_BuildingMaterial";
                SetMatColor(_buildingMaterial, new Color(0.75f, 0.78f, 0.82f, 1.0f));
            }

            if (_chunkMaterial == null && uShader != null)
            {
                _chunkMaterial = new Material(uShader);
                _chunkMaterial.name = "Runtime_ChunkMaterial";
                SetMatColor(_chunkMaterial, new Color(0.58f, 0.54f, 0.50f, 1.0f));
            }

            if (_heroMaterial == null && uShader != null)
            {
                _heroMaterial = new Material(uShader);
                _heroMaterial.name = "Runtime_HeroMaterial";
                SetMatColor(_heroMaterial, new Color(0.12f, 0.38f, 0.85f, 1.0f));
            }
        }

        private void SetMatColor(Material mat, Color col)
        {
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", col);
            else if (mat.HasProperty("_Color")) mat.SetColor("_Color", col);
        }

        public void GenerateCitySandbox()
        {
            if (_environmentRoot != null)
            {
                DestroyImmediate(_environmentRoot);
            }

            _environmentRoot = new GameObject("Karaikal_Environment_Root");

            if (_roadNetwork == null) _roadNetwork = gameObject.AddComponent<KaraikalRoadNetwork>();
            if (_buildingSystem == null) _buildingSystem = gameObject.AddComponent<ModularIndianBuildingSystem>();

            // 1. Generate Geographically Grounded Terrain & Water (Bay of Bengal + Arasalar River)
            _roadNetwork.GenerateTerrainAndWater(_environmentRoot.transform);

            // 2. Generate Road Network (NH32 Kamarajar Salai, Bharathiyar Rd, Beach Rd, Arasalar Bridge)
            _roadNetwork.GenerateRoadNetwork(_environmentRoot.transform);

            // 3. Generate Karaikal City Districts
            GenerateDistricts(_environmentRoot.transform);
        }

        private void GenerateDistricts(Transform parent)
        {
            GameObject districtContainer = new GameObject("Karaikal_Districts");
            districtContainer.transform.SetParent(parent, false);

            int seed = 1001;

            // District 1: Town Center & Bazaar (Commercial Complexes along Kamarajar Salai & Bharathiyar Rd)
            for (float x = -140f; x <= 140f; x += 45f)
            {
                for (float z = -120f; z <= 150f; z += 45f)
                {
                    // Leave main intersection and avenues clear
                    if (Mathf.Abs(x) < 22f || Mathf.Abs(z) < 20f) continue;
                    Vector3 pos = new Vector3(x, 0f, z);
                    GameObject b = _buildingSystem.ConstructBuilding(pos, KaraikalDistrictType.TownCenter, seed++);
                    b.transform.SetParent(districtContainer.transform, true);
                }
            }

            // District 2: Residential Neighborhoods (South Indian Terrace Houses to West & South-West)
            for (float x = -320f; x <= -180f; x += 40f)
            {
                for (float z = -150f; z <= 120f; z += 40f)
                {
                    Vector3 pos = new Vector3(x, 0f, z);
                    GameObject b = _buildingSystem.ConstructBuilding(pos, KaraikalDistrictType.HeritageTempleZone, seed++);
                    b.transform.SetParent(districtContainer.transform, true);
                }
            }

            // District 3: Karaikal Beach Promenade (Coastal structures along Beach Road X = 520m to 580m)
            for (float z = -200f; z <= 400f; z += 65f)
            {
                if (Mathf.Abs(z) < 30f) continue; // beach entrance plaza open
                Vector3 pos = new Vector3(560f, 0f, z);
                GameObject b = _buildingSystem.ConstructBuilding(pos, KaraikalDistrictType.BeachPromenade, seed++);
                b.transform.SetParent(districtContainer.transform, true);
            }

            // District 4: Heritage & Temple Zone (Near Karaikal Ammaiyar Temple X = -220, Z = +180)
            for (float x = -300f; x <= -140f; x += 45f)
            {
                for (float z = 180f; z <= 320f; z += 45f)
                {
                    Vector3 pos = new Vector3(x, 0f, z);
                    GameObject b = _buildingSystem.ConstructBuilding(pos, KaraikalDistrictType.HeritageTempleZone, seed++);
                    b.transform.SetParent(districtContainer.transform, true);
                }
            }

            // District 5: Transport Hub (Bus Stand & Railway Approach Z = +450 to +650, X = -350 to -100)
            for (float x = -350f; x <= -120f; x += 55f)
            {
                for (float z = 480f; z <= 620f; z += 55f)
                {
                    Vector3 pos = new Vector3(x, 0f, z);
                    GameObject b = _buildingSystem.ConstructBuilding(pos, KaraikalDistrictType.TransportHub, seed++);
                    b.transform.SetParent(districtContainer.transform, true);
                }
            }

            // District 6: Port & Industrial Zone (South of Arasalar River, Z = -600 to -850, X = 150 to 450)
            for (float x = 160f; x <= 400f; x += 75f)
            {
                for (float z = -800f; z <= -600f; z += 75f)
                {
                    Vector3 pos = new Vector3(x, 0f, z);
                    GameObject b = _buildingSystem.ConstructBuilding(pos, KaraikalDistrictType.PortIndustrialZone, seed++);
                    b.transform.SetParent(districtContainer.transform, true);
                }
            }

            // District 7: Modern High-Rise Expansion (Northern Coast X = 250 to 450, Z = 600 to 850)
            for (float x = 250f; x <= 450f; x += 65f)
            {
                for (float z = 600f; z <= 850f; z += 75f)
                {
                    Vector3 pos = new Vector3(x, 0f, z);
                    GameObject b = _buildingSystem.ConstructBuilding(pos, KaraikalDistrictType.ModernExpansion, seed++);
                    b.transform.SetParent(districtContainer.transform, true);
                }
            }
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

            // Spawn Superhero Character in central plaza (Town Center)
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
