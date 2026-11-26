using System;
using System.Collections.Generic;
using UnityEngine;
using CSVS.GodMode.Destruction;

namespace CSVS.GodMode.Environment
{
    public enum IndianBuildingType
    {
        CommercialBazaar = 0,
        ResidentialHouse = 1,
        TraditionalHeritage = 2,
        TransportTerminal = 3,
        IndustrialWarehouse = 4,
        ModernSkyscraper = 5
    }

    /// <summary>
    /// Procedural South Indian modular architectural generator for Karaikal.
    /// Builds authentic regional structures with compound walls, balconies, rooftop water tanks,
    /// commercial signboards, and pre-fractured destructible slices compatible with the CSVS destruction engine.
    /// </summary>
    public class ModularIndianBuildingSystem : MonoBehaviour
    {
        [Header("PBR Materials")]
        [SerializeField] private Material _concreteMaterial;
        [SerializeField] private Material _plasterPastelMaterial;
        [SerializeField] private Material _terracottaMaterial;
        [SerializeField] private Material _glassMaterial;
        [SerializeField] private Material _industrialMetalMaterial;
        [SerializeField] private Material _signboardMaterial;
        [SerializeField] private Material _waterTankMaterial;

        private static readonly Color[] PastelWallColors = new Color[]
        {
            new Color(0.88f, 0.78f, 0.65f), // Warm Cream / Chunam
            new Color(0.85f, 0.55f, 0.30f), // South Indian Saffron / Ochre
            new Color(0.45f, 0.72f, 0.62f), // Coastal Mint / Green
            new Color(0.78f, 0.62f, 0.70f), // Soft Lilac Rose
            new Color(0.82f, 0.75f, 0.45f), // Turmeric Yellow
            new Color(0.40f, 0.60f, 0.80f)  // Temple Sky Blue
        };

        private static readonly Color[] ContainerColors = new Color[]
        {
            new Color(0.75f, 0.15f, 0.15f), // Cargo Red
            new Color(0.15f, 0.35f, 0.75f), // Cargo Blue
            new Color(0.20f, 0.60f, 0.30f), // Cargo Green
            new Color(0.85f, 0.65f, 0.10f)  // Cargo Yellow
        };

        private static readonly string[] TamilShopNames = new string[]
        {
            "காரைக்கால் அங்காடி / Karaikal Bazaar",
            "அம்மா உணவகம் / Traditional Mess",
            "டீ கடை & பேக்கரி / Fresh Tea & Bakery",
            "செல்வம் டெக்ஸ்டைல்ஸ் / Selvam Silks & Textiles",
            "சரவணா ஸ்டோர்ஸ் / Saravana Departmental",
            "ஸ்ரீ முருகன் ஃபார்மசி / Sri Murugan Pharmacy",
            "பாரதி எலக்ட்ரிக்கல்ஸ் / Bharathi Electricals",
            "கடற்கரை கஃபே / Coastal Sea Breeze Cafe"
        };

        public void InitializeMaterials()
        {
            Shader uShader = Shader.Find("Universal Render Pipeline/Lit");
            if (uShader == null) uShader = Shader.Find("Universal Render Pipeline/Simple Lit");
            if (uShader == null) uShader = Shader.Find("Standard");

            if (_concreteMaterial == null && uShader != null)
            {
                _concreteMaterial = CreateMat(uShader, new Color(0.70f, 0.72f, 0.74f), 0.25f);
            }
            if (_plasterPastelMaterial == null && uShader != null)
            {
                _plasterPastelMaterial = CreateMat(uShader, new Color(0.88f, 0.78f, 0.65f), 0.35f);
            }
            if (_terracottaMaterial == null && uShader != null)
            {
                _terracottaMaterial = CreateMat(uShader, new Color(0.75f, 0.35f, 0.22f), 0.15f);
            }
            if (_glassMaterial == null && uShader != null)
            {
                _glassMaterial = CreateMat(uShader, new Color(0.20f, 0.45f, 0.65f), 0.85f);
            }
            if (_industrialMetalMaterial == null && uShader != null)
            {
                _industrialMetalMaterial = CreateMat(uShader, new Color(0.45f, 0.48f, 0.52f), 0.45f);
            }
            if (_signboardMaterial == null && uShader != null)
            {
                _signboardMaterial = CreateMat(uShader, new Color(0.85f, 0.20f, 0.15f), 0.50f);
            }
            if (_waterTankMaterial == null && uShader != null)
            {
                _waterTankMaterial = CreateMat(uShader, new Color(0.12f, 0.14f, 0.16f), 0.60f); // Sintex black polymer
            }
        }

        private Material CreateMat(Shader shader, Color col, float smoothness)
        {
            Material m = new Material(shader);
            if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", col);
            else if (m.HasProperty("_Color")) m.SetColor("_Color", col);
            if (m.HasProperty("_Smoothness")) m.SetFloat("_Smoothness", smoothness);
            return m;
        }

        public GameObject ConstructBuilding(Vector3 position, KaraikalDistrictType district, int seed)
        {
            InitializeMaterials();
            System.Random rand = new System.Random(seed);

            IndianBuildingType type = SelectTypeForDistrict(district, rand);
            GameObject root = new GameObject($"KaraikalBuilding_{district}_{seed}");
            root.transform.position = position;

            switch (type)
            {
                case IndianBuildingType.CommercialBazaar:
                    BuildCommercialBazaar(root, rand);
                    break;
                case IndianBuildingType.ResidentialHouse:
                    BuildResidentialHouse(root, rand);
                    break;
                case IndianBuildingType.TraditionalHeritage:
                    BuildTraditionalHeritage(root, rand);
                    break;
                case IndianBuildingType.IndustrialWarehouse:
                    BuildIndustrialWarehouse(root, rand);
                    break;
                case IndianBuildingType.ModernSkyscraper:
                    BuildModernSkyscraper(root, rand);
                    break;
                default:
                    BuildCommercialBazaar(root, rand);
                    break;
            }

            return root;
        }

        private IndianBuildingType SelectTypeForDistrict(KaraikalDistrictType district, System.Random rand)
        {
            switch (district)
            {
                case KaraikalDistrictType.TownCenter:
                    return rand.NextDouble() < 0.75 ? IndianBuildingType.CommercialBazaar : IndianBuildingType.ResidentialHouse;
                case KaraikalDistrictType.HeritageTempleZone:
                    return rand.NextDouble() < 0.60 ? IndianBuildingType.TraditionalHeritage : IndianBuildingType.ResidentialHouse;
                case KaraikalDistrictType.BeachPromenade:
                    return rand.NextDouble() < 0.50 ? IndianBuildingType.CommercialBazaar : IndianBuildingType.ResidentialHouse;
                case KaraikalDistrictType.PortIndustrialZone:
                    return IndianBuildingType.IndustrialWarehouse;
                case KaraikalDistrictType.ModernExpansion:
                    return IndianBuildingType.ModernSkyscraper;
                case KaraikalDistrictType.TransportHub:
                    return rand.NextDouble() < 0.50 ? IndianBuildingType.CommercialBazaar : IndianBuildingType.IndustrialWarehouse;
                default:
                    return IndianBuildingType.ResidentialHouse;
            }
        }

        private void BuildCommercialBazaar(GameObject root, System.Random rand)
        {
            float width = 16.0f;
            float depth = 16.0f;
            int floors = rand.Next(3, 5); // 3 to 4 stories
            float floorHeight = 3.8f;
            float totalHeight = floors * floorHeight;

            Color wallColor = PastelWallColors[rand.Next(PastelWallColors.Length)];
            Material wallMat = CreateMat(_plasterPastelMaterial.shader, wallColor, 0.3f);

            // Building Root Box Collider
            BoxCollider col = root.AddComponent<BoxCollider>();
            col.size = new Vector3(width, totalHeight, depth);
            col.center = new Vector3(0f, totalHeight * 0.5f, 0f);

            // Intact Model Container
            GameObject intact = new GameObject("IntactModel");
            intact.transform.SetParent(root.transform, false);

            // Ground Floor (Bazaar Shops & Shutters)
            GameObject groundFloor = CreatePrimitiveChild(PrimitiveType.Cube, intact.transform,
                new Vector3(0f, floorHeight * 0.5f, 0f), new Vector3(width, floorHeight, depth), wallMat);

            // Bilingual Shop Signboard on Ground Floor facade
            GameObject signboard = CreatePrimitiveChild(PrimitiveType.Cube, intact.transform,
                new Vector3(0f, floorHeight - 0.5f, depth * 0.5f + 0.15f), new Vector3(width * 0.9f, 0.8f, 0.2f), _signboardMaterial);

            // Upper Floors with Balconies / Sunshades
            for (int f = 1; f < floors; f++)
            {
                float y = f * floorHeight + floorHeight * 0.5f;
                GameObject upper = CreatePrimitiveChild(PrimitiveType.Cube, intact.transform,
                    new Vector3(0f, y, 0f), new Vector3(width, floorHeight, depth), wallMat);

                // Sunshade chajja
                CreatePrimitiveChild(PrimitiveType.Cube, intact.transform,
                    new Vector3(0f, y - floorHeight * 0.35f, depth * 0.5f + 0.3f), new Vector3(width * 0.85f, 0.15f, 0.6f), _concreteMaterial);
            }

            // Rooftop Parapet, Staircase Headroom & Water Tank
            BuildRooftopFeatures(intact.transform, width, depth, totalHeight);

            // Setup Pre-fractured Chunks for Destruction Engine
            SetupFractureSystem(root, width, depth, floors, floorHeight, wallMat);

            root.AddComponent<DestructibleBuilding>();
        }

        private void BuildResidentialHouse(GameObject root, System.Random rand)
        {
            float width = 14.0f;
            float depth = 14.0f;
            int floors = rand.Next(1, 3); // 1 to 2 stories
            float floorHeight = 3.5f;
            float totalHeight = floors * floorHeight;

            Color wallColor = PastelWallColors[rand.Next(PastelWallColors.Length)];
            Material wallMat = CreateMat(_plasterPastelMaterial.shader, wallColor, 0.35f);

            BoxCollider col = root.AddComponent<BoxCollider>();
            col.size = new Vector3(width + 4f, totalHeight, depth + 4f);
            col.center = new Vector3(0f, totalHeight * 0.5f, 0f);

            GameObject intact = new GameObject("IntactModel");
            intact.transform.SetParent(root.transform, false);

            // Main House Structure
            CreatePrimitiveChild(PrimitiveType.Cube, intact.transform,
                new Vector3(0f, totalHeight * 0.5f, 0f), new Vector3(width, totalHeight, depth), wallMat);

            // Compound Perimeter Wall with Front Gate
            BuildCompoundWall(intact.transform, width + 4f, depth + 4f, 1.8f);

            // Front Verandah / Thinnai Roof
            CreatePrimitiveChild(PrimitiveType.Cube, intact.transform,
                new Vector3(0f, floorHeight, depth * 0.5f + 1.2f), new Vector3(width * 0.7f, 0.2f, 2.4f), _terracottaMaterial);

            // Rooftop Sintex Water Tank
            BuildRooftopFeatures(intact.transform, width, depth, totalHeight);

            SetupFractureSystem(root, width, depth, floors, floorHeight, wallMat);
            root.AddComponent<DestructibleBuilding>();
        }

        private void BuildTraditionalHeritage(GameObject root, System.Random rand)
        {
            float width = 16.0f;
            float depth = 16.0f;
            int floors = 2;
            float floorHeight = 3.6f;
            float totalHeight = floors * floorHeight;

            Color heritageColor = new Color(0.85f, 0.78f, 0.60f); // Heritage limestone
            Material heritageMat = CreateMat(_plasterPastelMaterial.shader, heritageColor, 0.2f);

            BoxCollider col = root.AddComponent<BoxCollider>();
            col.size = new Vector3(width + 3f, totalHeight + 3f, depth + 3f);
            col.center = new Vector3(0f, totalHeight * 0.5f, 0f);

            GameObject intact = new GameObject("IntactModel");
            intact.transform.SetParent(root.transform, false);

            // Base Heritage Structure
            CreatePrimitiveChild(PrimitiveType.Cube, intact.transform,
                new Vector3(0f, totalHeight * 0.5f, 0f), new Vector3(width, totalHeight, depth), heritageMat);

            // Traditional Terracotta Sloped Parapet Trim
            CreatePrimitiveChild(PrimitiveType.Cube, intact.transform,
                new Vector3(0f, totalHeight + 0.3f, 0f), new Vector3(width + 0.8f, 0.6f, depth + 0.8f), _terracottaMaterial);

            // Mandapam Pillars on Front
            for (float px = -width * 0.35f; px <= width * 0.35f; px += width * 0.35f)
            {
                CreatePrimitiveChild(PrimitiveType.Cylinder, intact.transform,
                    new Vector3(px, floorHeight * 0.5f, depth * 0.5f + 1.2f), new Vector3(0.5f, floorHeight * 0.5f, 0.5f), _concreteMaterial);
            }

            SetupFractureSystem(root, width, depth, floors, floorHeight, heritageMat);
            root.AddComponent<DestructibleBuilding>();
        }

        private void BuildIndustrialWarehouse(GameObject root, System.Random rand)
        {
            float width = 24.0f;
            float depth = 20.0f;
            float height = 9.0f;

            BoxCollider col = root.AddComponent<BoxCollider>();
            col.size = new Vector3(width, height, depth);
            col.center = new Vector3(0f, height * 0.5f, 0f);

            GameObject intact = new GameObject("IntactModel");
            intact.transform.SetParent(root.transform, false);

            // Corrugated Metal Shed
            CreatePrimitiveChild(PrimitiveType.Cube, intact.transform,
                new Vector3(0f, height * 0.5f, 0f), new Vector3(width, height, depth), _industrialMetalMaterial);

            // Stacked Cargo Containers nearby
            for (int i = 0; i < 3; i++)
            {
                Color cCol = ContainerColors[rand.Next(ContainerColors.Length)];
                Material cMat = CreateMat(_industrialMetalMaterial.shader, cCol, 0.4f);
                float cy = 1.3f + i * 2.6f;
                CreatePrimitiveChild(PrimitiveType.Cube, intact.transform,
                    new Vector3(width * 0.5f + 3.5f, cy, 0f), new Vector3(2.5f, 2.5f, 6.0f), cMat);
            }

            SetupFractureSystem(root, width, depth, 2, height * 0.5f, _industrialMetalMaterial);
            root.AddComponent<DestructibleBuilding>();
        }

        private void BuildModernSkyscraper(GameObject root, System.Random rand)
        {
            float width = 22.0f;
            float depth = 22.0f;
            int floors = rand.Next(8, 14); // 8 to 13 stories for modern expansion
            float floorHeight = 4.0f;
            float totalHeight = floors * floorHeight;

            BoxCollider col = root.AddComponent<BoxCollider>();
            col.size = new Vector3(width, totalHeight, depth);
            col.center = new Vector3(0f, totalHeight * 0.5f, 0f);

            GameObject intact = new GameObject("IntactModel");
            intact.transform.SetParent(root.transform, false);

            // Reflective Glass Tower Body
            CreatePrimitiveChild(PrimitiveType.Cube, intact.transform,
                new Vector3(0f, totalHeight * 0.5f, 0f), new Vector3(width, totalHeight, depth), _glassMaterial);

            // Concrete Floor Ribs & Spire
            for (int f = 0; f <= floors; f++)
            {
                CreatePrimitiveChild(PrimitiveType.Cube, intact.transform,
                    new Vector3(0f, f * floorHeight, 0f), new Vector3(width + 0.4f, 0.4f, depth + 0.4f), _concreteMaterial);
            }

            // Rooftop Helipad
            CreatePrimitiveChild(PrimitiveType.Cylinder, intact.transform,
                new Vector3(0f, totalHeight + 0.2f, 0f), new Vector3(width * 0.6f, 0.2f, width * 0.6f), _concreteMaterial);

            SetupFractureSystem(root, width, depth, floors, floorHeight, _glassMaterial);
            root.AddComponent<DestructibleBuilding>();
        }

        private void BuildRooftopFeatures(Transform parent, float width, float depth, float totalHeight)
        {
            // Rooftop Parapet Wall
            CreatePrimitiveChild(PrimitiveType.Cube, parent,
                new Vector3(0f, totalHeight + 0.4f, depth * 0.5f), new Vector3(width, 0.8f, 0.25f), _concreteMaterial);
            CreatePrimitiveChild(PrimitiveType.Cube, parent,
                new Vector3(0f, totalHeight + 0.4f, -depth * 0.5f), new Vector3(width, 0.8f, 0.25f), _concreteMaterial);

            // Staircase Headroom Shed
            CreatePrimitiveChild(PrimitiveType.Cube, parent,
                new Vector3(-width * 0.25f, totalHeight + 1.2f, -depth * 0.25f), new Vector3(3.5f, 2.4f, 3.5f), _plasterPastelMaterial);

            // Sintex Black Polymer Water Tank on Stilt Slab
            CreatePrimitiveChild(PrimitiveType.Cube, parent,
                new Vector3(width * 0.25f, totalHeight + 0.5f, depth * 0.25f), new Vector3(2.2f, 0.8f, 2.2f), _concreteMaterial);
            CreatePrimitiveChild(PrimitiveType.Cylinder, parent,
                new Vector3(width * 0.25f, totalHeight + 1.8f, depth * 0.25f), new Vector3(1.6f, 1.2f, 1.6f), _waterTankMaterial);
        }

        private void BuildCompoundWall(Transform parent, float width, float depth, float height)
        {
            // Perimeter compound wall
            CreatePrimitiveChild(PrimitiveType.Cube, parent,
                new Vector3(0f, height * 0.5f, depth * 0.5f), new Vector3(width, height, 0.25f), _concreteMaterial);
            CreatePrimitiveChild(PrimitiveType.Cube, parent,
                new Vector3(0f, height * 0.5f, -depth * 0.5f), new Vector3(width, height, 0.25f), _concreteMaterial);
            CreatePrimitiveChild(PrimitiveType.Cube, parent,
                new Vector3(width * 0.5f, height * 0.5f, 0f), new Vector3(0.25f, height, depth), _concreteMaterial);
            CreatePrimitiveChild(PrimitiveType.Cube, parent,
                new Vector3(-width * 0.5f, height * 0.5f, 0f), new Vector3(0.25f, height, depth), _concreteMaterial);
        }

        private void SetupFractureSystem(GameObject root, float width, float depth, int floors, float floorHeight, Material chunkMat)
        {
            GameObject fractureRoot = new GameObject("FractureRoot");
            fractureRoot.transform.SetParent(root.transform, false);

            for (int f = 0; f < floors; f++)
            {
                GameObject floorObj = new GameObject($"Floor_{f}");
                floorObj.transform.SetParent(fractureRoot.transform, false);
                floorObj.transform.localPosition = new Vector3(0f, f * floorHeight + floorHeight * 0.5f, 0f);

                for (int cx = -1; cx <= 1; cx += 2)
                {
                    for (int cz = -1; cz <= 1; cz += 2)
                    {
                        GameObject chunk = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        chunk.name = $"Chunk_{cx}_{cz}";
                        chunk.transform.SetParent(floorObj.transform, false);
                        chunk.transform.localPosition = new Vector3(cx * (width * 0.25f), 0f, cz * (depth * 0.25f));
                        chunk.transform.localScale = new Vector3(width * 0.48f, floorHeight * 0.95f, depth * 0.48f);

                        Renderer r = chunk.GetComponent<Renderer>();
                        if (r != null) r.sharedMaterial = chunkMat;

                        Rigidbody rb = chunk.AddComponent<Rigidbody>();
                        rb.isKinematic = true;
                        rb.mass = 250f;
                    }
                }
            }

            fractureRoot.SetActive(false);
        }

        private GameObject CreatePrimitiveChild(PrimitiveType primType, Transform parent, Vector3 localPos, Vector3 localScale, Material mat)
        {
            GameObject go = GameObject.CreatePrimitive(primType);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.transform.localScale = localScale;

            Destroy(go.GetComponent<Collider>());

            Renderer r = go.GetComponent<Renderer>();
            if (r != null && mat != null) r.sharedMaterial = mat;

            return go;
        }
    }
}
