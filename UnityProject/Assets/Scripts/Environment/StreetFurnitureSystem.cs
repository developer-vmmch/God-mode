using System;
using UnityEngine;

namespace CSVS.GodMode.Environment
{
    /// <summary>
    /// Generates authentic South Indian street environment assets in Karaikal:
    /// - Concrete utility poles with transformers, crossbars and overhead cables (media_1795684003602_21ecf166.jpg, media_1795684049634_b692d5d2.jpg).
    /// - Galvanized octagonal streetlights (media_1795683980433_86fe4afe.jpg).
    /// - Traditional roadside tea stall (Chai kadai) with hanging incandescent bulb (media_1795684185309_f7bd2912.jpg).
    /// - Bus shelter with bilingual transit sign.
    /// - Speed breakers with painted yellow/black hazard chevrons.
    /// - Roadside coconut palms and neem trees.
    /// </summary>
    public static class StreetFurnitureSystem
    {
        public static GameObject SpawnConcreteUtilityPole(Transform parent, Vector3 position, Material concreteMat, Material metalMat)
        {
            GameObject pole = new GameObject("ConcreteUtilityPole");
            pole.transform.SetParent(parent, false);
            pole.transform.position = position;

            // Main Concrete Mast (Height 8.5m, slightly tapered)
            CreateChild(PrimitiveType.Cube, pole.transform, new Vector3(0f, 4.25f, 0f), new Vector3(0.35f, 8.5f, 0.35f), concreteMat);

            // Steel Crossarms on top
            CreateChild(PrimitiveType.Cube, pole.transform, new Vector3(0f, 7.8f, 0f), new Vector3(2.4f, 0.12f, 0.12f), metalMat);
            CreateChild(PrimitiveType.Cube, pole.transform, new Vector3(0f, 8.2f, 0f), new Vector3(1.8f, 0.12f, 0.12f), metalMat);

            // Step-down Cylindrical Distribution Transformer (on select poles)
            CreateChild(PrimitiveType.Cylinder, pole.transform, new Vector3(0.40f, 5.8f, 0f), new Vector3(0.45f, 0.55f, 0.45f), metalMat);

            return pole;
        }

        public static GameObject SpawnStreetlight(Transform parent, Vector3 position, Material metalMat, Material lightGlowMat)
        {
            GameObject light = new GameObject("Streetlight_Pole");
            light.transform.SetParent(parent, false);
            light.transform.position = position;

            // Mast (Height 7m)
            CreateChild(PrimitiveType.Cylinder, light.transform, new Vector3(0f, 3.5f, 0f), new Vector3(0.18f, 3.5f, 0.18f), metalMat);
            // Curved Arm Overhanging Road
            CreateChild(PrimitiveType.Cube, light.transform, new Vector3(0.75f, 7.1f, 0f), new Vector3(1.6f, 0.12f, 0.12f), metalMat);
            // Light Fixture Housing
            CreateChild(PrimitiveType.Cube, light.transform, new Vector3(1.45f, 6.95f, 0f), new Vector3(0.35f, 0.12f, 0.22f), lightGlowMat);

            // Real Point/Spot Light
            GameObject lightObj = new GameObject("Streetlight_Luminary");
            lightObj.transform.SetParent(light.transform, false);
            lightObj.transform.localPosition = new Vector3(1.45f, 6.8f, 0f);
            Light l = lightObj.AddComponent<Light>();
            l.type = LightType.Spot;
            l.range = 22f;
            l.spotAngle = 65f;
            l.color = new Color(1f, 0.92f, 0.75f); // Warm sodium/LED yellow
            l.intensity = 2.2f;

            return light;
        }

        public static GameObject SpawnTeaStall(Transform parent, Vector3 position, Material woodMat, Material roofMat, Material metalMat, Material bulbMat)
        {
            // South Indian Chai Kadai (2.8m x 2.2m)
            GameObject stall = new GameObject("RoadsideTeaStall_ChaiKadai");
            stall.transform.SetParent(parent, false);
            stall.transform.position = position;

            // Wooden Service Counter
            CreateChild(PrimitiveType.Cube, stall.transform, new Vector3(0f, 0.55f, 0.70f), new Vector3(2.4f, 1.1f, 0.6f), woodMat);
            // Rear Storage Shelves & Wall
            CreateChild(PrimitiveType.Cube, stall.transform, new Vector3(0f, 1.1f, -0.90f), new Vector3(2.6f, 2.2f, 0.2f), woodMat);
            // Corner Support Posts
            CreateChild(PrimitiveType.Cube, stall.transform, new Vector3(-1.2f, 1.1f, 0.9f), new Vector3(0.1f, 2.2f, 0.1f), woodMat);
            CreateChild(PrimitiveType.Cube, stall.transform, new Vector3(1.2f, 1.1f, 0.9f), new Vector3(0.1f, 2.2f, 0.1f), woodMat);

            // Slanted Corrugated Sheet Roof Canopy
            CreateChild(PrimitiveType.Cube, stall.transform, new Vector3(0f, 2.25f, 0.1f), new Vector3(2.8f, 0.1f, 2.4f), roofMat, new Vector3(8f, 0f, 0f));

            // Tea Brewing Urn / Samovar on Counter
            CreateChild(PrimitiveType.Cylinder, stall.transform, new Vector3(-0.6f, 1.35f, 0.70f), new Vector3(0.35f, 0.30f, 0.35f), metalMat);
            // Wooden Bench for Customers
            CreateChild(PrimitiveType.Cube, stall.transform, new Vector3(0f, 0.25f, 1.6f), new Vector3(2.0f, 0.45f, 0.40f), woodMat);

            // Hanging Warm Yellow Incandescent Bulb
            CreateChild(PrimitiveType.Sphere, stall.transform, new Vector3(0f, 1.95f, 0.5f), new Vector3(0.16f, 0.16f, 0.16f), bulbMat);
            GameObject bulbLight = new GameObject("TeaStall_WarmBulb");
            bulbLight.transform.SetParent(stall.transform, false);
            bulbLight.transform.localPosition = new Vector3(0f, 1.9f, 0.5f);
            Light bl = bulbLight.AddComponent<Light>();
            bl.type = LightType.Point;
            bl.range = 8.0f;
            bl.color = new Color(1.0f, 0.75f, 0.25f); // Deep warm amber
            bl.intensity = 2.5f;

            return stall;
        }

        public static GameObject SpawnBusShelter(Transform parent, Vector3 position, Material metalMat, Material glassMat, Material signMat)
        {
            GameObject shelter = new GameObject("KaraikalBusShelter");
            shelter.transform.SetParent(parent, false);
            shelter.transform.position = position;

            // Dimensions: 4.5m wide, 2.5m deep, 2.6m high
            CreateChild(PrimitiveType.Cube, shelter.transform, new Vector3(0f, 2.55f, 0f), new Vector3(4.5f, 0.12f, 2.4f), metalMat);
            // Back Glass Wall
            CreateChild(PrimitiveType.Cube, shelter.transform, new Vector3(0f, 1.25f, -1.05f), new Vector3(4.2f, 2.3f, 0.05f), glassMat);
            // Metal Bench
            CreateChild(PrimitiveType.Cube, shelter.transform, new Vector3(0f, 0.45f, -0.65f), new Vector3(3.6f, 0.10f, 0.45f), metalMat);
            // Bilingual Tamil/English Signboard
            CreateChild(PrimitiveType.Cube, shelter.transform, new Vector3(0f, 2.4f, 1.15f), new Vector3(4.2f, 0.45f, 0.10f), signMat);

            return shelter;
        }

        public static GameObject SpawnSpeedBreaker(Transform parent, Vector3 position, float roadWidth, Material chevronMat)
        {
            GameObject hump = GameObject.CreatePrimitive(PrimitiveType.Cube);
            hump.name = "SpeedBreaker_Hump";
            hump.transform.SetParent(parent, false);
            hump.transform.position = position + Vector3.up * 0.06f;
            hump.transform.localScale = new Vector3(roadWidth, 0.12f, 1.4f);

            Renderer r = hump.GetComponent<Renderer>();
            if (r != null && chevronMat != null) r.sharedMaterial = chevronMat;

            return hump;
        }

        public static GameObject SpawnCoconutPalm(Transform parent, Vector3 position, Material trunkMat, Material frondMat, float height = 11.0f)
        {
            GameObject palm = new GameObject("CoconutPalm_Tree");
            palm.transform.SetParent(parent, false);
            palm.transform.position = position;

            // Curved Slender Trunk (Cylinders stacked with slight lean)
            float segH = height / 3.0f;
            CreateChild(PrimitiveType.Cylinder, palm.transform, new Vector3(0f, segH * 0.5f, 0f), new Vector3(0.40f, segH * 0.5f, 0.40f), trunkMat);
            CreateChild(PrimitiveType.Cylinder, palm.transform, new Vector3(0.2f, segH * 1.5f, 0f), new Vector3(0.35f, segH * 0.5f, 0.35f), trunkMat, new Vector3(0f, 0f, -4f));
            CreateChild(PrimitiveType.Cylinder, palm.transform, new Vector3(0.5f, segH * 2.5f, 0f), new Vector3(0.30f, segH * 0.5f, 0.30f), trunkMat, new Vector3(0f, 0f, -6f));

            // Crown of Palm Fronds on Top
            Vector3 crownPos = new Vector3(0.7f, height, 0f);
            for (int a = 0; a < 8; a++)
            {
                float rotY = a * 45f;
                CreateChild(PrimitiveType.Cube, palm.transform, crownPos + Quaternion.Euler(0f, rotY, 0f) * new Vector3(0f, -0.3f, 1.8f),
                    new Vector3(0.6f, 0.08f, 3.6f), frondMat, new Vector3(25f, rotY, 0f));
            }

            return palm;
        }

        private static GameObject CreateChild(PrimitiveType type, Transform parent, Vector3 localPos, Vector3 scale, Material mat, Vector3? rot = null)
        {
            GameObject go = GameObject.CreatePrimitive(type);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.transform.localScale = scale;
            if (rot.HasValue) go.transform.localRotation = Quaternion.Euler(rot.Value);

            SafeDestroy(go.GetComponent<Collider>());
            Renderer r = go.GetComponent<Renderer>();
            if (r != null && mat != null) r.sharedMaterial = mat;

            return go;
        }

        private static void SafeDestroy(UnityEngine.Object obj)
        {
            if (obj == null) return;
            if (Application.isPlaying) UnityEngine.Object.Destroy(obj);
            else UnityEngine.Object.DestroyImmediate(obj);
        }
    }
}
