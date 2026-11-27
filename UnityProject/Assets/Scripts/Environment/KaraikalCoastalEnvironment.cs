// KaraikalCoastalEnvironment.cs
// CSVS GOD MODE — V0.3 Realistic Coastline, Water & Vegetation
// Implements Bay of Bengal coastline, animated ocean waves,
// Arasalar River surface, beach sand, and coastal vegetation.

using System.Collections;
using UnityEngine;

namespace CSVS.GodMode.Environment
{
    public class KaraikalCoastalEnvironment : MonoBehaviour
    {
        [Header("Water Settings")]
        [SerializeField] private bool enableWaves = true;
        [SerializeField] private float waveHeight    = 0.35f;   // metres
        [SerializeField] private float waveFrequency = 0.8f;    // Hz
        [SerializeField] private float waveSpeed     = 1.2f;    // spatial speed
        [SerializeField] private int   waterResolution = 32;    // vertices per dimension

        [Header("Quality Scaling")]
        [SerializeField] private bool lowSpecMode;   // Reduces vertex count & wave fidelity

        [Header("Scene References")]
        [SerializeField] private Light sunLight;

        // ─── Generated objects ─────────────────────────────────────────────
        private Mesh _oceanMesh;
        private Vector3[] _baseVerts;
        private GameObject _oceanObj;
        private GameObject _riverObj;
        private GameObject _beachObj;
        private Material _oceanMat, _riverMat, _sandMat, _wetSandMat;

        // Ocean extends from X=+900 to X=+2000, full Z span
        private const float OCEAN_X_START = 900f;
        private const float OCEAN_X_END   = 2200f;
        private const float OCEAN_Z_MIN   = -2000f;
        private const float OCEAN_Z_MAX   = 2000f;

        // Arasalar River band: Z ≈ -818 ± 30m
        private const float RIVER_Z_CENTER = -818f;
        private const float RIVER_WIDTH    = 48f;

        // ─── Lifecycle ─────────────────────────────────────────────────────
        private void Start()
        {
            CreateMaterials();
            BuildOcean();
            BuildRiver();
            BuildBeach();
            PlantCoastalVegetation();
            PlaceLighthouse();
        }

        private void Update()
        {
            if (enableWaves) AnimateOceanWaves();
            UpdateWaterReflection();
        }

        // ─── Materials ─────────────────────────────────────────────────────
        private void CreateMaterials()
        {
            Shader lit = Shader.Find("Universal Render Pipeline/Lit");

            // Deep ocean — translucent deep teal
            _oceanMat = new Material(lit) { name = "OceanWater" };
            _oceanMat.color = new Color(0.03f, 0.28f, 0.42f, 0.88f);
            _oceanMat.SetFloat("_Smoothness", 0.92f);
            _oceanMat.SetFloat("_Metallic", 0.0f);
            _oceanMat.EnableKeyword("_ALPHABLEND_ON");
            _oceanMat.renderQueue = 3000;

            // River — lighter fresh teal
            _riverMat = new Material(lit) { name = "RiverWater" };
            _riverMat.color = new Color(0.08f, 0.45f, 0.52f, 0.80f);
            _riverMat.SetFloat("_Smoothness", 0.85f);

            // Dry sand beach
            _sandMat = new Material(lit) { name = "BeachSand" };
            _sandMat.color = new Color(0.88f, 0.80f, 0.60f);  // Warm golden sand
            _sandMat.SetFloat("_Smoothness", 0.04f);

            // Wet sand at waterline
            _wetSandMat = new Material(lit) { name = "WetSand" };
            _wetSandMat.color = new Color(0.62f, 0.56f, 0.42f);
            _wetSandMat.SetFloat("_Smoothness", 0.55f);  // Wet sheen
        }

        // ─── Ocean Mesh ────────────────────────────────────────────────────
        private void BuildOcean()
        {
            int res = lowSpecMode ? 16 : waterResolution;
            float xRange = OCEAN_X_END - OCEAN_X_START;
            float zRange = OCEAN_Z_MAX - OCEAN_Z_MIN;

            int vertsX = res + 1;
            int vertsZ = res + 1;
            _baseVerts = new Vector3[vertsX * vertsZ];
            var uvs   = new Vector2[_baseVerts.Length];
            var tris  = new System.Collections.Generic.List<int>();

            for (int zi = 0; zi < vertsZ; zi++)
            {
                for (int xi = 0; xi < vertsX; xi++)
                {
                    int idx = zi * vertsX + xi;
                    float x = OCEAN_X_START + (xi / (float)res) * xRange;
                    float z = OCEAN_Z_MIN   + (zi / (float)res) * zRange;
                    _baseVerts[idx] = new Vector3(x, -0.3f, z);  // Slightly below ground
                    uvs[idx]        = new Vector2(x / 20f, z / 20f);
                }
            }

            for (int zi = 0; zi < res; zi++)
            {
                for (int xi = 0; xi < res; xi++)
                {
                    int bl = zi * vertsX + xi;
                    tris.AddRange(new[] { bl, bl + vertsX, bl + 1,
                                          bl + 1, bl + vertsX, bl + vertsX + 1 });
                }
            }

            _oceanMesh = new Mesh { name = "OceanSurface" };
            _oceanMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            _oceanMesh.vertices  = _baseVerts;
            _oceanMesh.uv        = uvs;
            _oceanMesh.triangles = tris.ToArray();
            _oceanMesh.RecalculateNormals();

            _oceanObj = new GameObject("BayOfBengal_Ocean");
            _oceanObj.transform.SetParent(transform);
            _oceanObj.AddComponent<MeshFilter>().mesh = _oceanMesh;
            var mr = _oceanObj.AddComponent<MeshRenderer>();
            mr.material = _oceanMat;
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }

        // ─── Animated Waves ────────────────────────────────────────────────
        private void AnimateOceanWaves()
        {
            if (_oceanMesh == null || _baseVerts == null) return;

            var verts = _oceanMesh.vertices;
            float t = Time.time;

            for (int i = 0; i < _baseVerts.Length; i++)
            {
                float x = _baseVerts[i].x;
                float z = _baseVerts[i].z;
                // Layered Gerstner-inspired wave (simplified for performance)
                float wave1 = Mathf.Sin(x * 0.08f - t * waveSpeed) * waveHeight;
                float wave2 = Mathf.Sin(z * 0.05f + t * waveSpeed * 0.7f) * waveHeight * 0.4f;
                float wave3 = Mathf.Sin((x + z) * 0.04f - t * waveSpeed * 1.3f) * waveHeight * 0.2f;
                verts[i].y = _baseVerts[i].y + wave1 + wave2 + wave3;
            }

            _oceanMesh.vertices = verts;
            _oceanMesh.RecalculateNormals();
        }

        // ─── River ─────────────────────────────────────────────────────────
        private void BuildRiver()
        {
            // Arasalar River — flows east from X=-300 to X=+950
            float riverLength = 1250f;
            float startX = -300f;

            var verts = new Vector3[]
            {
                new Vector3(startX,          -0.1f, RIVER_Z_CENTER - RIVER_WIDTH * 0.5f),
                new Vector3(startX,          -0.1f, RIVER_Z_CENTER + RIVER_WIDTH * 0.5f),
                new Vector3(startX + riverLength, -0.1f, RIVER_Z_CENTER - RIVER_WIDTH * 0.5f),
                new Vector3(startX + riverLength, -0.1f, RIVER_Z_CENTER + RIVER_WIDTH * 0.5f),
            };
            var mesh = new Mesh { name = "ArasalarRiver" };
            mesh.vertices  = verts;
            mesh.uv        = new[] { Vector2.zero, Vector2.up, Vector2.right, Vector2.one };
            mesh.triangles = new[] { 0, 1, 2, 1, 3, 2 };
            mesh.RecalculateNormals();

            _riverObj = new GameObject("ArasalarRiver");
            _riverObj.transform.SetParent(transform);
            _riverObj.AddComponent<MeshFilter>().mesh = mesh;
            var mr = _riverObj.AddComponent<MeshRenderer>();
            mr.material = _riverMat;
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }

        // ─── Beach ─────────────────────────────────────────────────────────
        private void BuildBeach()
        {
            // Sand strip from X=+850 to X=+950, full Z span
            var verts = new Vector3[]
            {
                new Vector3(850f,  0.0f, -800f),
                new Vector3(950f,  0.0f, -800f),
                new Vector3(850f,  0.0f,  800f),
                new Vector3(950f,  0.0f,  800f),
            };
            var mesh = new Mesh { name = "BeachSand" };
            mesh.vertices  = verts;
            mesh.uv        = new[] { Vector2.zero, new Vector2(1,0), Vector2.up, Vector2.one };
            mesh.triangles = new[] { 0, 2, 1, 1, 2, 3 };
            mesh.RecalculateNormals();

            _beachObj = new GameObject("KaraikalBeach");
            _beachObj.transform.SetParent(transform);
            _beachObj.AddComponent<MeshFilter>().mesh = mesh;
            _beachObj.AddComponent<MeshRenderer>().material = _sandMat;
            _beachObj.AddComponent<MeshCollider>().sharedMesh = mesh;

            // Wet sand strip (waterline transition)
            var wetMesh = new Mesh { name = "WetSand" };
            var wv = new Vector3[]
            {
                new Vector3(900f, 0.01f, -800f), new Vector3(950f, 0.01f, -800f),
                new Vector3(900f, 0.01f,  800f), new Vector3(950f, 0.01f,  800f),
            };
            wetMesh.vertices  = wv;
            wetMesh.uv        = new[] { Vector2.zero, new Vector2(1,0), Vector2.up, Vector2.one };
            wetMesh.triangles = new[] { 0, 2, 1, 1, 2, 3 };
            wetMesh.RecalculateNormals();
            var wetObj = new GameObject("WetSand");
            wetObj.transform.SetParent(transform);
            wetObj.AddComponent<MeshFilter>().mesh = wetMesh;
            wetObj.AddComponent<MeshRenderer>().material = _wetSandMat;
        }

        // ─── Coastal Vegetation ─────────────────────────────────────────────
        private void PlantCoastalVegetation()
        {
            var vegContainer = new GameObject("CoastalVegetation");
            vegContainer.transform.SetParent(transform);

            // Coconut palms along Beach Road (X ≈ 870–880)
            float[] palmZ = { -300f, -150f, 0f, 100f, 220f, 350f, 480f };
            foreach (float z in palmZ)
                PlaceCoconutPalm(new Vector3(870f, 0f, z), vegContainer.transform);

            // Inland palm line (X ≈ 650)
            float[] inlandZ = { -100f, 50f, 180f, 310f };
            foreach (float z in inlandZ)
                PlaceCoconutPalm(new Vector3(650f, 0f, z), vegContainer.transform);
        }

        private void PlaceCoconutPalm(Vector3 position, Transform parent)
        {
            var palm = new GameObject("CoconutPalm");
            palm.transform.SetParent(parent);
            palm.transform.position = position;
            palm.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

            Shader lit = Shader.Find("Universal Render Pipeline/Lit");

            // Trunk — slightly curved cylinder (simulated with tall capsule)
            var trunk = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            trunk.name = "PalmTrunk";
            trunk.transform.SetParent(palm.transform);
            trunk.transform.localPosition = new Vector3(0, 5f, 0);
            trunk.transform.localScale = new Vector3(0.22f, 5f, 0.22f);
            trunk.transform.localRotation = Quaternion.Euler(0, 0, Random.Range(-6f, 6f));
            var trunkMat = new Material(lit) { color = new Color(0.42f, 0.32f, 0.22f) };
            trunkMat.SetFloat("_Smoothness", 0.1f);
            trunk.GetComponent<MeshRenderer>().material = trunkMat;
            DestroyImmediate(trunk.GetComponent<CapsuleCollider>());

            // Canopy — dark green sphere cluster
            for (int i = 0; i < 5; i++)
            {
                var frond = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                frond.name = $"PalmFrond_{i}";
                frond.transform.SetParent(palm.transform);
                float angle = i * 72f;
                float rad = 1.4f;
                frond.transform.localPosition = new Vector3(
                    Mathf.Sin(angle * Mathf.Deg2Rad) * rad, 10.2f + Random.Range(-0.3f, 0.3f),
                    Mathf.Cos(angle * Mathf.Deg2Rad) * rad
                );
                frond.transform.localScale = new Vector3(1.0f, 0.35f, 2.0f);
                var frondMat = new Material(lit) { color = new Color(0.18f, 0.42f, 0.12f) };
                frondMat.SetFloat("_Smoothness", 0.2f);
                frond.GetComponent<MeshRenderer>().material = frondMat;
                DestroyImmediate(frond.GetComponent<SphereCollider>());
            }
        }

        // ─── Lighthouse ────────────────────────────────────────────────────
        private void PlaceLighthouse()
        {
            var lh = new GameObject("KaraikalLighthouse");
            lh.transform.SetParent(transform);
            lh.transform.position = new Vector3(960f, 0f, 90f);  // Near beach promenade

            Shader lit = Shader.Find("Universal Render Pipeline/Lit");

            // Tower body
            var tower = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            tower.name = "LighthouseTower";
            tower.transform.SetParent(lh.transform);
            tower.transform.localPosition = new Vector3(0, 10f, 0);
            tower.transform.localScale = new Vector3(2.4f, 10f, 2.4f);
            var towerMat = new Material(lit) { color = new Color(0.90f, 0.88f, 0.84f) };
            towerMat.SetFloat("_Smoothness", 0.3f);
            tower.GetComponent<MeshRenderer>().material = towerMat;

            // Red stripe
            var stripe = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            stripe.name = "LighthouseStripe";
            stripe.transform.SetParent(lh.transform);
            stripe.transform.localPosition = new Vector3(0, 11f, 0);
            stripe.transform.localScale = new Vector3(2.6f, 1.0f, 2.6f);
            stripe.GetComponent<MeshRenderer>().material = new Material(lit) { color = new Color(0.75f, 0.08f, 0.08f) };

            // Lantern room
            var lantern = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            lantern.name = "LanternRoom";
            lantern.transform.SetParent(lh.transform);
            lantern.transform.localPosition = new Vector3(0, 22f, 0);
            lantern.transform.localScale = new Vector3(3.0f, 1.5f, 3.0f);
            lantern.GetComponent<MeshRenderer>().material = new Material(lit) { color = new Color(0.4f, 0.4f, 0.5f) };

            // Revolving beacon light
            var beacon = new GameObject("LighthouseBeacon");
            beacon.transform.SetParent(lh.transform);
            beacon.transform.localPosition = new Vector3(0, 24f, 0);
            var beaconLight = beacon.AddComponent<Light>();
            beaconLight.type      = LightType.Spot;
            beaconLight.color     = new Color(1f, 0.95f, 0.70f);
            beaconLight.intensity = 8f;
            beaconLight.spotAngle = 15f;
            beaconLight.range     = 400f;
            beacon.AddComponent<LighthouseBeaconRotator>();
        }

        // ─── Water Reflection ──────────────────────────────────────────────
        private void UpdateWaterReflection()
        {
            if (!sunLight || !_oceanMat) return;
            // Approximate specular highlight intensity based on sun angle
            float sunDot = Vector3.Dot(sunLight.transform.forward, Vector3.down);
            float specular = Mathf.Clamp01(sunDot) * 0.8f + 0.1f;
            _oceanMat.SetFloat("_Smoothness", specular);
        }
    }

    /// <summary>Rotates the lighthouse beacon in the scene.</summary>
    public class LighthouseBeaconRotator : MonoBehaviour
    {
        [SerializeField] private float rpm = 4f;
        private void Update()
            => transform.Rotate(Vector3.up, rpm * 360f / 60f * Time.deltaTime);
    }
}

