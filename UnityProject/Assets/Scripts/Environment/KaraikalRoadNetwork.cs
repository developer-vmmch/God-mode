using System;
using System.Collections.Generic;
using UnityEngine;

namespace CSVS.GodMode.Environment
{
    /// <summary>
    /// Procedural road and terrain network generator reflecting the actual geography of Karaikal, Puducherry.
    /// Constructs Kamarajar Salai (NH 32), Bharathiyar Road, Beach Road, Arasalar River & Bridge,
    /// and secondary streets with Indian road markings, curbs, and street props.
    /// </summary>
    public class KaraikalRoadNetwork : MonoBehaviour
    {
        [Header("Materials")]
        [SerializeField] private Material _asphaltMaterial;
        [SerializeField] private Material _curbMaterial;
        [SerializeField] private Material _beachSandMaterial;
        [SerializeField] private Material _waterMaterial;
        [SerializeField] private Material _paddyFieldMaterial;
        [SerializeField] private Material _promenadeMaterial;

        public void InitializeMaterials()
        {
            Shader uShader = Shader.Find("Universal Render Pipeline/Lit");
            if (uShader == null) uShader = Shader.Find("Universal Render Pipeline/Simple Lit");
            if (uShader == null) uShader = Shader.Find("Standard");

            if (_asphaltMaterial == null && uShader != null)
                _asphaltMaterial = CreateMat(uShader, new Color(0.18f, 0.20f, 0.22f), 0.12f);
            if (_curbMaterial == null && uShader != null)
                _curbMaterial = CreateMat(uShader, new Color(0.85f, 0.75f, 0.15f), 0.20f); // Indian yellow/black warning curb
            if (_beachSandMaterial == null && uShader != null)
                _beachSandMaterial = CreateMat(uShader, new Color(0.84f, 0.76f, 0.58f), 0.05f);
            if (_waterMaterial == null && uShader != null)
                _waterMaterial = CreateMat(uShader, new Color(0.12f, 0.38f, 0.58f), 0.90f);
            if (_paddyFieldMaterial == null && uShader != null)
                _paddyFieldMaterial = CreateMat(uShader, new Color(0.28f, 0.52f, 0.22f), 0.10f);
            if (_promenadeMaterial == null && uShader != null)
                _promenadeMaterial = CreateMat(uShader, new Color(0.78f, 0.74f, 0.70f), 0.30f);
        }

        private Material CreateMat(Shader shader, Color col, float smoothness)
        {
            Material m = new Material(shader);
            if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", col);
            else if (m.HasProperty("_Color")) m.SetColor("_Color", col);
            if (m.HasProperty("_Smoothness")) m.SetFloat("_Smoothness", smoothness);
            return m;
        }

        public void GenerateTerrainAndWater(Transform parent)
        {
            InitializeMaterials();

            // 1. Base Ground (Green Agricultural / Delta Terrain)
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Karaikal_Terrain_Base";
            ground.transform.SetParent(parent, false);
            ground.transform.position = new Vector3(-200f, -0.05f, 0f);
            ground.transform.localScale = new Vector3(140f, 1f, 200f);
            SetSharedMat(ground, _paddyFieldMaterial);

            // 2. Karaikal Beach Sand Strip (Bay of Bengal Coastline on East: X = +550 to +900)
            GameObject beach = GameObject.CreatePrimitive(PrimitiveType.Plane);
            beach.name = "Karaikal_Beach_Sand";
            beach.transform.SetParent(parent, false);
            beach.transform.position = new Vector3(720f, -0.02f, 50f);
            beach.transform.localScale = new Vector3(35f, 1f, 180f);
            SetSharedMat(beach, _beachSandMaterial);

            // 3. Bay of Bengal Ocean Water Plane (X > +800)
            GameObject ocean = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ocean.name = "Bay_Of_Bengal_Ocean";
            ocean.transform.SetParent(parent, false);
            ocean.transform.position = new Vector3(1200f, -0.8f, 50f);
            ocean.transform.localScale = new Vector3(60f, 1f, 200f);
            SetSharedMat(ocean, _waterMaterial);

            // 4. Arasalar River Channel (Flows West to East at Z = -380m into the Sea)
            GameObject river = GameObject.CreatePrimitive(PrimitiveType.Plane);
            river.name = "Arasalar_River_Water";
            river.transform.SetParent(parent, false);
            river.transform.position = new Vector3(0f, -0.6f, -380f);
            river.transform.localScale = new Vector3(180f, 1f, 7f); // ~70m wide river channel
            SetSharedMat(river, _waterMaterial);

            // 5. Beach Promenade Paved Walkway (along Beach Road at X = +620)
            CreateRoadSegment(parent, "Beach_Promenade_Walkway", new Vector3(625f, 0.05f, 50f), new Vector3(10f, 0.15f, 1200f), _promenadeMaterial);
        }

        public void GenerateRoadNetwork(Transform parent)
        {
            InitializeMaterials();

            // 1. Kamarajar Salai (NH 32 North-South Major Highway)
            CreateRoadSegment(parent, "Road_KamarajarSalai_NH32", new Vector3(0f, 0.02f, 0f), new Vector3(18f, 0.08f, 2200f), _asphaltMaterial);
            CreateCurbs(parent, "Curbs_KamarajarSalai", new Vector3(0f, 0.1f, 0f), 18.2f, 2200f, true);

            // 2. Bharathiyar Road (East-West Commercial Spine connecting town center to beach)
            CreateRoadSegment(parent, "Road_BharathiyarRoad", new Vector3(0f, 0.02f, 0f), new Vector3(1300f, 0.08f, 14f), _asphaltMaterial);
            CreateCurbs(parent, "Curbs_BharathiyarRoad", new Vector3(0f, 0.1f, 0f), 1300f, 14.2f, false);

            // 3. Beach Road (North-South Coastal Arterial parallel to promenade)
            CreateRoadSegment(parent, "Road_BeachRoad", new Vector3(610f, 0.02f, 50f), new Vector3(14f, 0.08f, 1400f), _asphaltMaterial);

            // 4. Thirunallar Highway (Westward road toward Thirunallar Temple)
            CreateRoadSegment(parent, "Road_ThirunallarHighway", new Vector3(-650f, 0.02f, 180f), new Vector3(1100f, 0.08f, 14f), _asphaltMaterial);

            // 5. Arasalar River Bridge (Spanning the Arasalar River on Kamarajar Salai)
            BuildArasalarBridge(parent);

            // 6. Secondary Grid Streets (Residential and Market Avenues)
            for (int z = -200; z <= 600; z += 180)
            {
                if (Mathf.Abs(z) < 30 || Mathf.Abs(z - (-380)) < 80) continue;
                CreateRoadSegment(parent, $"Street_Cross_{z}", new Vector3(250f, 0.02f, z), new Vector3(500f, 0.06f, 10f), _asphaltMaterial);
                CreateRoadSegment(parent, $"Street_West_{z}", new Vector3(-250f, 0.02f, z), new Vector3(500f, 0.06f, 10f), _asphaltMaterial);
            }

            // 7. Place Indian Street Props along Kamarajar Salai & Bharathiyar Rd
            PlaceStreetProps(parent);
        }

        private void BuildArasalarBridge(Transform parent)
        {
            GameObject bridge = new GameObject("Arasalar_Bridge_Structure");
            bridge.transform.SetParent(parent, false);
            bridge.transform.position = new Vector3(0f, 0f, -380f);

            // Bridge Deck
            GameObject deck = GameObject.CreatePrimitive(PrimitiveType.Cube);
            deck.name = "BridgeDeck";
            deck.transform.SetParent(bridge.transform, false);
            deck.transform.localPosition = new Vector3(0f, 0.6f, 0f);
            deck.transform.localScale = new Vector3(18f, 1.2f, 85f);
            SetSharedMat(deck, _asphaltMaterial);

            // Bridge Concrete Railings (East & West sides)
            GameObject railL = GameObject.CreatePrimitive(PrimitiveType.Cube);
            railL.transform.SetParent(bridge.transform, false);
            railL.transform.localPosition = new Vector3(-9.2f, 1.8f, 0f);
            railL.transform.localScale = new Vector3(0.6f, 1.2f, 85f);
            SetSharedMat(railL, _curbMaterial);

            GameObject railR = GameObject.CreatePrimitive(PrimitiveType.Cube);
            railR.transform.SetParent(bridge.transform, false);
            railR.transform.localPosition = new Vector3(9.2f, 1.8f, 0f);
            railR.transform.localScale = new Vector3(0.6f, 1.2f, 85f);
            SetSharedMat(railR, _curbMaterial);

            // Bridge Support Pillars beneath in river
            for (float pz = -25f; pz <= 25f; pz += 25f)
            {
                GameObject pillar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                pillar.transform.SetParent(bridge.transform, false);
                pillar.transform.localPosition = new Vector3(0f, -1.5f, pz);
                pillar.transform.localScale = new Vector3(3.5f, 3.0f, 3.5f);
                SetSharedMat(pillar, _curbMaterial);
            }
        }

        private void CreateRoadSegment(Transform parent, string name, Vector3 pos, Vector3 scale, Material mat)
        {
            GameObject road = GameObject.CreatePrimitive(PrimitiveType.Cube);
            road.name = name;
            road.transform.SetParent(parent, false);
            road.transform.position = pos;
            road.transform.localScale = scale;
            SetSharedMat(road, mat);
        }

        private void CreateCurbs(Transform parent, string name, Vector3 pos, float dimX, float dimZ, bool isNorthSouth)
        {
            GameObject curbGroup = new GameObject(name);
            curbGroup.transform.SetParent(parent, false);
            curbGroup.transform.position = pos;

            if (isNorthSouth)
            {
                CreateRoadSegment(curbGroup.transform, "Curb_L", new Vector3(-dimX * 0.5f - 0.2f, 0.1f, 0f), new Vector3(0.4f, 0.2f, dimZ), _curbMaterial);
                CreateRoadSegment(curbGroup.transform, "Curb_R", new Vector3(dimX * 0.5f + 0.2f, 0.1f, 0f), new Vector3(0.4f, 0.2f, dimZ), _curbMaterial);
                // Center Median
                CreateRoadSegment(curbGroup.transform, "Median", new Vector3(0f, 0.1f, 0f), new Vector3(0.8f, 0.22f, dimZ), _curbMaterial);
            }
            else
            {
                CreateRoadSegment(curbGroup.transform, "Curb_N", new Vector3(0f, 0.1f, dimZ * 0.5f + 0.2f), new Vector3(dimX, 0.2f, 0.4f), _curbMaterial);
                CreateRoadSegment(curbGroup.transform, "Curb_S", new Vector3(0f, 0.1f, -dimZ * 0.5f - 0.2f), new Vector3(dimX, 0.2f, 0.4f), _curbMaterial);
            }
        }

        private void PlaceStreetProps(Transform parent)
        {
            GameObject propsGroup = new GameObject("Karaikal_Street_Props");
            propsGroup.transform.SetParent(parent, false);

            // Concrete Electric Poles along Kamarajar Salai
            for (float z = -250f; z <= 500f; z += 60f)
            {
                if (Mathf.Abs(z - (-380)) < 60) continue; // skip bridge span

                // Electric Pole (Left)
                GameObject poleL = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                poleL.transform.SetParent(propsGroup.transform, false);
                poleL.transform.position = new Vector3(-10.5f, 4.5f, z);
                poleL.transform.localScale = new Vector3(0.35f, 4.5f, 0.35f);
                SetSharedMat(poleL, _curbMaterial);

                // Crossbar
                GameObject armL = GameObject.CreatePrimitive(PrimitiveType.Cube);
                armL.transform.SetParent(poleL.transform, false);
                armL.transform.localPosition = new Vector3(0.5f, 0.85f, 0f);
                armL.transform.localScale = new Vector3(3.0f, 0.2f, 0.2f);
                SetSharedMat(armL, _asphaltMaterial);
            }

            // Roadside Tea Stall with Blue Tarpaulin Awning near Bharathiyar Rd Junction
            GameObject teaStall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            teaStall.name = "Prop_TeaStall_Bazaar";
            teaStall.transform.SetParent(propsGroup.transform, false);
            teaStall.transform.position = new Vector3(14f, 1.4f, 12f);
            teaStall.transform.localScale = new Vector3(4.5f, 2.8f, 3.2f);
            SetSharedMat(teaStall, _promenadeMaterial);
        }

        private void SetSharedMat(GameObject go, Material mat)
        {
            Renderer r = go.GetComponent<Renderer>();
            if (r != null && mat != null) r.sharedMaterial = mat;
        }
    }
}
