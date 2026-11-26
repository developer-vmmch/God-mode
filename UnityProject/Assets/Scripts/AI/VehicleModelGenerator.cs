using System;
using UnityEngine;

namespace CSVS.GodMode.AI
{
    /// <summary>
    /// Procedural 3D model generator for authentic Indian vehicles in Karaikal.
    /// Constructs detailed multi-element vehicle geometry matching reference designs:
    /// - South Indian Auto-Rickshaw (Yellow canvas canopy, black passenger tub, chrome single front wheel fork, headlight guard, interior seats).
    /// - Indian Hatchback (Maruti Swift styling, curved cabin, headlights, tinted glass).
    /// - Executive Sedan (Extended boot, chrome grill, alloy wheels).
    /// - Commuter Motorcycle (Hero Splendor / Pulsar styling, fuel tank, handlebars, exhaust pipe).
    /// - City Bus (Ashok Leyland style green/yellow transport bus).
    /// </summary>
    public static class VehicleModelGenerator
    {
        public static void GenerateVehicleModel(GameObject root, IndianVehicleType vehicleType, Material bodyMat, Material yellowCanopyMat, Material metalMat, Material glassMat, Material rubberMat, Material lightMat)
        {
            Transform parent = root.transform;

            switch (vehicleType)
            {
                case IndianVehicleType.AutoRickshaw:
                    BuildAutoRickshaw(parent, bodyMat, yellowCanopyMat, metalMat, glassMat, rubberMat, lightMat);
                    break;
                case IndianVehicleType.Hatchback:
                    BuildHatchback(parent, bodyMat, metalMat, glassMat, rubberMat, lightMat);
                    break;
                case IndianVehicleType.Sedan:
                    BuildSedan(parent, bodyMat, metalMat, glassMat, rubberMat, lightMat);
                    break;
                case IndianVehicleType.Motorcycle:
                    BuildMotorcycle(parent, bodyMat, metalMat, rubberMat, lightMat);
                    break;
                case IndianVehicleType.CityBus:
                    BuildCityBus(parent, bodyMat, metalMat, glassMat, rubberMat, lightMat);
                    break;
                default:
                    BuildAutoRickshaw(parent, bodyMat, yellowCanopyMat, metalMat, glassMat, rubberMat, lightMat);
                    break;
            }
        }

        private static void BuildAutoRickshaw(Transform parent, Material blackBodyMat, Material yellowCanopyMat, Material chromeMat, Material glassMat, Material rubberMat, Material lightMat)
        {
            // Auto-Rickshaw Dimensions: Width 1.4m, Length 2.6m, Height 1.8m
            GameObject model = new GameObject("AutoRickshaw_DetailedMesh");
            model.transform.SetParent(parent, false);

            // 1. Lower Black Metal Body / Passenger Tub
            CreatePart(PrimitiveType.Cube, model.transform, new Vector3(0f, 0.45f, -0.15f), new Vector3(1.35f, 0.50f, 1.80f), blackBodyMat);
            // Front Tapered Nose
            CreatePart(PrimitiveType.Cube, model.transform, new Vector3(0f, 0.40f, 0.95f), new Vector3(0.90f, 0.45f, 0.70f), blackBodyMat);

            // 2. Yellow Canvas Roof Canopy (Curved arched top)
            CreatePart(PrimitiveType.Cube, model.transform, new Vector3(0f, 1.65f, -0.10f), new Vector3(1.38f, 0.12f, 1.95f), yellowCanopyMat);
            CreatePart(PrimitiveType.Cube, model.transform, new Vector3(0f, 1.55f, 0.80f), new Vector3(1.10f, 0.12f, 0.50f), yellowCanopyMat); // Front sun visor canopy
            // Yellow Upper Side Pillars / Soft Curtains
            CreatePart(PrimitiveType.Cube, model.transform, new Vector3(-0.66f, 1.25f, -0.95f), new Vector3(0.06f, 0.75f, 0.08f), yellowCanopyMat);
            CreatePart(PrimitiveType.Cube, model.transform, new Vector3(0.66f, 1.25f, -0.95f), new Vector3(0.06f, 0.75f, 0.08f), yellowCanopyMat);
            CreatePart(PrimitiveType.Cube, model.transform, new Vector3(-0.55f, 1.25f, 0.70f), new Vector3(0.06f, 0.75f, 0.08f), yellowCanopyMat);
            CreatePart(PrimitiveType.Cube, model.transform, new Vector3(0.55f, 1.25f, 0.70f), new Vector3(0.06f, 0.75f, 0.08f), yellowCanopyMat);

            // 3. Front Windshield Glass & Frame
            CreatePart(PrimitiveType.Cube, model.transform, new Vector3(0f, 1.15f, 0.75f), new Vector3(1.05f, 0.65f, 0.04f), glassMat);
            // Black Wiper
            CreatePart(PrimitiveType.Cube, model.transform, new Vector3(0f, 1.10f, 0.78f), new Vector3(0.03f, 0.45f, 0.02f), chromeMat);

            // 4. Front Chrome Headlight & Guard Rail (Iconic Single Center Light)
            CreatePart(PrimitiveType.Cylinder, model.transform, new Vector3(0f, 0.55f, 1.32f), new Vector3(0.24f, 0.08f, 0.24f), lightMat, new Vector3(90f, 0f, 0f));
            CreatePart(PrimitiveType.Cube, model.transform, new Vector3(0f, 0.40f, 1.36f), new Vector3(0.55f, 0.25f, 0.05f), chromeMat); // Crash guard bumper

            // 5. Interior Passenger & Driver Bench Seats
            CreatePart(PrimitiveType.Cube, model.transform, new Vector3(0f, 0.55f, -0.65f), new Vector3(1.25f, 0.25f, 0.55f), blackBodyMat); // Rear passenger seat
            CreatePart(PrimitiveType.Cube, model.transform, new Vector3(0f, 0.75f, -0.90f), new Vector3(1.25f, 0.40f, 0.12f), blackBodyMat); // Backrest
            CreatePart(PrimitiveType.Cube, model.transform, new Vector3(0f, 0.60f, 0.15f), new Vector3(0.70f, 0.22f, 0.38f), blackBodyMat);  // Driver seat
            // Handlebars
            CreatePart(PrimitiveType.Cube, model.transform, new Vector3(0f, 0.95f, 0.60f), new Vector3(0.60f, 0.04f, 0.04f), chromeMat);

            // 6. Wheels (Three wheels: Single front center, two rear)
            CreateWheel(model.transform, new Vector3(0f, 0.25f, 1.05f), rubberMat, chromeMat);    // Front wheel
            CreateWheel(model.transform, new Vector3(-0.65f, 0.25f, -0.65f), rubberMat, chromeMat); // Rear Left wheel
            CreateWheel(model.transform, new Vector3(0.65f, 0.25f, -0.65f), rubberMat, chromeMat);  // Rear Right wheel
        }

        private static void BuildHatchback(Transform parent, Material bodyMat, Material chromeMat, Material glassMat, Material rubberMat, Material lightMat)
        {
            // Hatchback Dimensions: Width 1.75m, Length 3.9m, Height 1.5m
            GameObject model = new GameObject("Hatchback_DetailedMesh");
            model.transform.SetParent(parent, false);

            // Lower Chassis & Hood
            CreatePart(PrimitiveType.Cube, model.transform, new Vector3(0f, 0.45f, 0f), new Vector3(1.70f, 0.45f, 3.80f), bodyMat);
            // Passenger Cabin Roof
            CreatePart(PrimitiveType.Cube, model.transform, new Vector3(0f, 1.15f, -0.25f), new Vector3(1.50f, 0.55f, 2.20f), bodyMat);

            // Glass Windshields & Windows
            CreatePart(PrimitiveType.Cube, model.transform, new Vector3(0f, 1.08f, 0.95f), new Vector3(1.42f, 0.48f, 0.06f), glassMat, new Vector3(-35f, 0f, 0f)); // Front windshield
            CreatePart(PrimitiveType.Cube, model.transform, new Vector3(0f, 1.08f, -1.35f), new Vector3(1.42f, 0.48f, 0.06f), glassMat, new Vector3(30f, 0f, 0f));  // Rear windshield
            CreatePart(PrimitiveType.Cube, model.transform, new Vector3(0.76f, 1.15f, -0.25f), new Vector3(0.04f, 0.42f, 1.90f), glassMat); // Side windows Right
            CreatePart(PrimitiveType.Cube, model.transform, new Vector3(-0.76f, 1.15f, -0.25f), new Vector3(0.04f, 0.42f, 1.90f), glassMat); // Side windows Left

            // Headlights & Tail Lights
            CreatePart(PrimitiveType.Cube, model.transform, new Vector3(-0.62f, 0.55f, 1.92f), new Vector3(0.32f, 0.18f, 0.08f), lightMat);
            CreatePart(PrimitiveType.Cube, model.transform, new Vector3(0.62f, 0.55f, 1.92f), new Vector3(0.32f, 0.18f, 0.08f), lightMat);
            // Grille
            CreatePart(PrimitiveType.Cube, model.transform, new Vector3(0f, 0.50f, 1.92f), new Vector3(0.70f, 0.14f, 0.06f), chromeMat);

            // 4 Wheels
            CreateWheel(model.transform, new Vector3(-0.82f, 0.32f, 1.15f), rubberMat, chromeMat);
            CreateWheel(model.transform, new Vector3(0.82f, 0.32f, 1.15f), rubberMat, chromeMat);
            CreateWheel(model.transform, new Vector3(-0.82f, 0.32f, -1.15f), rubberMat, chromeMat);
            CreateWheel(model.transform, new Vector3(0.82f, 0.32f, -1.15f), rubberMat, chromeMat);
        }

        private static void BuildSedan(Transform parent, Material bodyMat, Material chromeMat, Material glassMat, Material rubberMat, Material lightMat)
        {
            GameObject model = new GameObject("Sedan_DetailedMesh");
            model.transform.SetParent(parent, false);

            // Lower Chassis
            CreatePart(PrimitiveType.Cube, model.transform, new Vector3(0f, 0.45f, 0f), new Vector3(1.78f, 0.45f, 4.40f), bodyMat);
            // Cabin Roof
            CreatePart(PrimitiveType.Cube, model.transform, new Vector3(0f, 1.15f, -0.10f), new Vector3(1.52f, 0.55f, 2.30f), bodyMat);
            // Extended Rear Trunk / Boot
            CreatePart(PrimitiveType.Cube, model.transform, new Vector3(0f, 0.65f, -1.65f), new Vector3(1.68f, 0.25f, 1.05f), bodyMat);

            // Windshields
            CreatePart(PrimitiveType.Cube, model.transform, new Vector3(0f, 1.08f, 1.12f), new Vector3(1.44f, 0.50f, 0.06f), glassMat, new Vector3(-38f, 0f, 0f));
            CreatePart(PrimitiveType.Cube, model.transform, new Vector3(0f, 1.08f, -1.30f), new Vector3(1.44f, 0.48f, 0.06f), glassMat, new Vector3(35f, 0f, 0f));

            // Headlights & Tail Lights
            CreatePart(PrimitiveType.Cube, model.transform, new Vector3(-0.65f, 0.55f, 2.22f), new Vector3(0.35f, 0.16f, 0.08f), lightMat);
            CreatePart(PrimitiveType.Cube, model.transform, new Vector3(0.65f, 0.55f, 2.22f), new Vector3(0.35f, 0.16f, 0.08f), lightMat);

            // 4 Wheels
            CreateWheel(model.transform, new Vector3(-0.84f, 0.32f, 1.35f), rubberMat, chromeMat);
            CreateWheel(model.transform, new Vector3(0.84f, 0.32f, 1.35f), rubberMat, chromeMat);
            CreateWheel(model.transform, new Vector3(-0.84f, 0.32f, -1.35f), rubberMat, chromeMat);
            CreateWheel(model.transform, new Vector3(0.84f, 0.32f, -1.35f), rubberMat, chromeMat);
        }

        private static void BuildMotorcycle(Transform parent, Material bodyMat, Material chromeMat, Material rubberMat, Material lightMat)
        {
            GameObject model = new GameObject("Motorcycle_DetailedMesh");
            model.transform.SetParent(parent, false);

            // Frame & Engine Block
            CreatePart(PrimitiveType.Cube, model.transform, new Vector3(0f, 0.55f, 0f), new Vector3(0.24f, 0.35f, 0.85f), chromeMat);
            // Fuel Tank
            CreatePart(PrimitiveType.Cube, model.transform, new Vector3(0f, 0.85f, 0.20f), new Vector3(0.32f, 0.25f, 0.55f), bodyMat);
            // Seat
            CreatePart(PrimitiveType.Cube, model.transform, new Vector3(0f, 0.78f, -0.32f), new Vector3(0.28f, 0.14f, 0.65f), chromeMat);
            // Handlebars
            CreatePart(PrimitiveType.Cube, model.transform, new Vector3(0f, 1.05f, 0.52f), new Vector3(0.72f, 0.04f, 0.04f), chromeMat);
            // Headlight
            CreatePart(PrimitiveType.Cylinder, model.transform, new Vector3(0f, 0.95f, 0.68f), new Vector3(0.18f, 0.08f, 0.18f), lightMat, new Vector3(90f, 0f, 0f));
            // Exhaust Pipe
            CreatePart(PrimitiveType.Cylinder, model.transform, new Vector3(0.18f, 0.28f, -0.20f), new Vector3(0.08f, 0.75f, 0.08f), chromeMat, new Vector3(90f, 0f, 0f));

            // Wheels
            CreateWheel(model.transform, new Vector3(0f, 0.32f, 0.85f), rubberMat, chromeMat, 0.12f);
            CreateWheel(model.transform, new Vector3(0f, 0.32f, -0.85f), rubberMat, chromeMat, 0.12f);
        }

        private static void BuildCityBus(Transform parent, Material bodyMat, Material chromeMat, Material glassMat, Material rubberMat, Material lightMat)
        {
            GameObject model = new GameObject("CityBus_DetailedMesh");
            model.transform.SetParent(parent, false);

            // Length 9.5m, Width 2.5m, Height 3.2m
            CreatePart(PrimitiveType.Cube, model.transform, new Vector3(0f, 1.70f, 0f), new Vector3(2.45f, 2.60f, 9.20f), bodyMat);
            // Window Strips along both sides
            CreatePart(PrimitiveType.Cube, model.transform, new Vector3(1.24f, 2.05f, 0.20f), new Vector3(0.06f, 0.90f, 7.80f), glassMat);
            CreatePart(PrimitiveType.Cube, model.transform, new Vector3(-1.24f, 2.05f, 0.20f), new Vector3(0.06f, 0.90f, 7.80f), glassMat);
            // Front Windshield
            CreatePart(PrimitiveType.Cube, model.transform, new Vector3(0f, 2.05f, 4.62f), new Vector3(2.30f, 1.10f, 0.06f), glassMat);
            // Headlights
            CreatePart(PrimitiveType.Cube, model.transform, new Vector3(-0.85f, 0.75f, 4.62f), new Vector3(0.35f, 0.20f, 0.06f), lightMat);
            CreatePart(PrimitiveType.Cube, model.transform, new Vector3(0.85f, 0.75f, 4.62f), new Vector3(0.35f, 0.20f, 0.06f), lightMat);

            // 6 Wheels (Front dual, Rear quad)
            CreateWheel(model.transform, new Vector3(-1.15f, 0.48f, 2.80f), rubberMat, chromeMat, 0.32f);
            CreateWheel(model.transform, new Vector3(1.15f, 0.48f, 2.80f), rubberMat, chromeMat, 0.32f);
            CreateWheel(model.transform, new Vector3(-1.15f, 0.48f, -2.40f), rubberMat, chromeMat, 0.32f);
            CreateWheel(model.transform, new Vector3(1.15f, 0.48f, -2.40f), rubberMat, chromeMat, 0.32f);
        }

        private static void CreateWheel(Transform parent, Vector3 localPos, Material rubberMat, Material rimMat, float thickness = 0.24f)
        {
            GameObject wheel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            wheel.name = "Wheel";
            wheel.transform.SetParent(parent, false);
            wheel.transform.localPosition = localPos;
            wheel.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
            wheel.transform.localScale = new Vector3(0.65f, thickness * 0.5f, 0.65f);

            SafeDestroy(wheel.GetComponent<Collider>());
            Renderer r = wheel.GetComponent<Renderer>();
            if (r != null) r.sharedMaterial = rubberMat;

            // Chrome Rim / Hubcap
            GameObject rim = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            rim.name = "Rim";
            rim.transform.SetParent(wheel.transform, false);
            rim.transform.localPosition = Vector3.zero;
            rim.transform.localScale = new Vector3(0.65f, 1.05f, 0.65f);
            SafeDestroy(rim.GetComponent<Collider>());
            Renderer rr = rim.GetComponent<Renderer>();
            if (rr != null) rr.sharedMaterial = rimMat;
        }

        private static GameObject CreatePart(PrimitiveType type, Transform parent, Vector3 localPos, Vector3 scale, Material mat, Vector3? rotation = null)
        {
            GameObject part = GameObject.CreatePrimitive(type);
            part.transform.SetParent(parent, false);
            part.transform.localPosition = localPos;
            part.transform.localScale = scale;
            if (rotation.HasValue) part.transform.localRotation = Quaternion.Euler(rotation.Value);

            SafeDestroy(part.GetComponent<Collider>());
            Renderer r = part.GetComponent<Renderer>();
            if (r != null && mat != null) r.sharedMaterial = mat;

            return part;
        }

        private static void SafeDestroy(UnityEngine.Object obj)
        {
            if (obj == null) return;
            if (Application.isPlaying) UnityEngine.Object.Destroy(obj);
            else UnityEngine.Object.DestroyImmediate(obj);
        }
    }
}
