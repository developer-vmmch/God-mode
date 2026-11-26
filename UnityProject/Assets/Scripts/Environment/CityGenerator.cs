using System.Collections.Generic;
using UnityEngine;
using CSVS.GodMode.Core;
using CSVS.GodMode.Destruction;
using CSVS.GodMode.Player;
using CSVS.GodMode.Powers;
using CSVS.GodMode.AI;

namespace CSVS.GodMode.Environment
{
    /// <summary>
    /// Master Open-World Generator for Karaikal, Puducherry, India.
    /// Geographically grounded in real Karaikal coordinates (10.9254° N, 79.8380° E).
    /// Assembles Kamarajar Salai (NH 32), Bharathiyar Road, Arasalar River & Bridge,
    /// Karaikal Beach Promenade, modular South Indian architectural complexes,
    /// humanoid superhero actor, and playable Indian vehicles.
    /// </summary>
    public class CityGenerator : MonoBehaviour
    {
        [Header("Modular Systems")]
        [SerializeField] private KaraikalRoadNetwork _roadNetwork;
        [SerializeField] private ModularIndianBuildingSystem _buildingSystem;
        [SerializeField] private KaraikalWorldStreamer _worldStreamer;

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

            if (_worldStreamer == null)
            {
                _worldStreamer = GetComponent<KaraikalWorldStreamer>();
                if (_worldStreamer == null) _worldStreamer = gameObject.AddComponent<KaraikalWorldStreamer>();
            }

            EnsureMaterials();
        }

        private void Start()
        {
            GenerateCitySandbox();
            EnsurePlayerExists();
            SpawnPlayableVehicles();
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

            // 1. Generate Geographically Validated Terrain & Water (Bay of Bengal + Arasalar River)
            _roadNetwork.GenerateTerrainAndWater(_environmentRoot.transform);

            // 2. Generate Road Network (NH32 Kamarajar Salai, Bharathiyar Rd, Beach Rd, Arasalar Bridge)
            _roadNetwork.GenerateRoadNetwork(_environmentRoot.transform);

            // 3. Generate Initial Karaikal Playable Urban Core
            GenerateDistricts(_environmentRoot.transform);
        }

        private void GenerateDistricts(Transform parent)
        {
            GameObject districtContainer = new GameObject("Karaikal_Districts");
            districtContainer.transform.SetParent(parent, false);

            int seed = 2001;

            // District 1: Town Center & Bazaar (Commercial Complexes along Kamarajar Salai & Bharathiyar Rd)
            for (float x = -150f; x <= 150f; x += 45f)
            {
                for (float z = -140f; z <= 160f; z += 45f)
                {
                    if (Mathf.Abs(x) < 22f || Mathf.Abs(z) < 20f) continue;
                    Vector3 pos = new Vector3(x, 0f, z);
                    GameObject b = _buildingSystem.ConstructBuilding(pos, KaraikalDistrictType.TownCenter, seed++);
                    b.transform.SetParent(districtContainer.transform, true);
                }
            }

            // District 2: Residential Neighborhoods (South Indian Terrace Houses to West & South-West)
            for (float x = -380f; x <= -200f; x += 42f)
            {
                for (float z = -180f; z <= 140f; z += 42f)
                {
                    Vector3 pos = new Vector3(x, 0f, z);
                    GameObject b = _buildingSystem.ConstructBuilding(pos, KaraikalDistrictType.HeritageTempleZone, seed++);
                    b.transform.SetParent(districtContainer.transform, true);
                }
            }

            // District 3: Karaikal Beach Promenade (Coastal structures along Beach Road X = 860m to 920m)
            for (float z = -250f; z <= 450f; z += 70f)
            {
                if (Mathf.Abs(z) < 35f) continue; // beach entrance plaza open
                Vector3 pos = new Vector3(880f, 0f, z);
                GameObject b = _buildingSystem.ConstructBuilding(pos, KaraikalDistrictType.BeachPromenade, seed++);
                b.transform.SetParent(districtContainer.transform, true);
            }

            // District 4: Heritage & Temple Zone (Near Karaikal Ammaiyar Temple X = -280, Z = +200)
            for (float x = -360f; x <= -180f; x += 45f)
            {
                for (float z = 190f; z <= 340f; z += 45f)
                {
                    Vector3 pos = new Vector3(x, 0f, z);
                    GameObject b = _buildingSystem.ConstructBuilding(pos, KaraikalDistrictType.HeritageTempleZone, seed++);
                    b.transform.SetParent(districtContainer.transform, true);
                }
            }

            // District 5: Transport Hub (Bus Stand & Railway Approach Z = +550 to +750, X = -450 to -150)
            for (float x = -420f; x <= -160f; x += 55f)
            {
                for (float z = 550f; z <= 720f; z += 55f)
                {
                    Vector3 pos = new Vector3(x, 0f, z);
                    GameObject b = _buildingSystem.ConstructBuilding(pos, KaraikalDistrictType.TransportHub, seed++);
                    b.transform.SetParent(districtContainer.transform, true);
                }
            }

            // District 6: Port & Industrial Zone (South of Arasalar River, Z = -950 to -1150, X = 200 to 500)
            for (float x = 200f; x <= 460f; x += 80f)
            {
                for (float z = -1100f; z <= -900f; z += 80f)
                {
                    Vector3 pos = new Vector3(x, 0f, z);
                    GameObject b = _buildingSystem.ConstructBuilding(pos, KaraikalDistrictType.PortIndustrialZone, seed++);
                    b.transform.SetParent(districtContainer.transform, true);
                }
            }

            // District 7: Modern High-Rise Expansion (Northern Coast X = 300 to 550, Z = 650 to 950)
            for (float x = 300f; x <= 520f; x += 70f)
            {
                for (float z = 650f; z <= 920f; z += 80f)
                {
                    Vector3 pos = new Vector3(x, 0f, z);
                    GameObject b = _buildingSystem.ConstructBuilding(pos, KaraikalDistrictType.ModernExpansion, seed++);
                    b.transform.SetParent(districtContainer.transform, true);
                }
            }
        }

        private void SpawnPlayableVehicles()
        {
            GameObject vehicleGroup = new GameObject("Karaikal_Playable_Vehicles");
            vehicleGroup.transform.position = Vector3.zero;

            // 1. Indian City Hatchback on Bharathiyar Road
            SpawnVehicle("Vehicle_IndianHatchback", new Vector3(14f, 0.4f, 25f), Quaternion.identity, IndianVehicleType.Hatchback, vehicleGroup.transform);

            // 2. Auto-Rickshaw parked near Town Center junction
            SpawnVehicle("Vehicle_AutoRickshaw", new Vector3(-13f, 0.4f, 32f), Quaternion.Euler(0f, 180f, 0f), IndianVehicleType.AutoRickshaw, vehicleGroup.transform);

            // 3. Indian Commuter Motorcycle near Tea Stall
            SpawnVehicle("Vehicle_Motorcycle", new Vector3(14.5f, 0.3f, 10f), Quaternion.Euler(0f, 90f, 0f), IndianVehicleType.Motorcycle, vehicleGroup.transform);
        }

        private void SpawnVehicle(string name, Vector3 pos, Quaternion rot, IndianVehicleType type, Transform parent)
        {
            GameObject vObj = new GameObject(name);
            vObj.transform.SetParent(parent, false);
            vObj.transform.position = pos;
            vObj.transform.rotation = rot;

            var veh = vObj.AddComponent<PlayableIndianVehicle>();
            // Use reflection or serialized fields to set vehicle type if private
            var field = typeof(PlayableIndianVehicle).GetField("_vehicleType", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null) field.SetValue(veh, type);
        }

        private void EnsurePlayerExists()
        {
            SuperheroFlightController existing = FindFirstObjectByType<SuperheroFlightController>();
            if (existing != null)
            {
                if (existing.GetComponentInChildren<SuperheroHumanoidVisual>() == null)
                {
                    existing.gameObject.AddComponent<SuperheroHumanoidVisual>();
                }
                return;
            }

            // Spawn Superhero Character in central plaza (Town Center)
            GameObject hero = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            hero.name = "SuperheroPlayer";
            hero.tag = "Player";
            hero.transform.position = new Vector3(0.0f, 15.0f, 0.0f);

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

            // Attach Original Humanoid Armored Superhero Model
            hero.AddComponent<SuperheroHumanoidVisual>();

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
