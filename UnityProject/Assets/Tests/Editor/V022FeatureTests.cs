using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using CSVS.GodMode.Core;
using CSVS.GodMode.AI;
using CSVS.GodMode.Environment;
using CSVS.GodMode.Player;

namespace CSVS.GodMode.Tests.Editor
{
    [TestFixture]
    public class V022FeatureTests
    {
        [Test]
        public void GameInputManager_KeyBindings_AreCompletelyUnconflicted()
        {
            // F key is dedicated to Super Punch
            Assert.AreEqual(Key.F, GameInputManager.KeySuperPunch, "Super Punch must be bound to F key.");

            // G key is dedicated to Vehicle Enter/Exit
            Assert.AreEqual(Key.G, GameInputManager.KeyVehicleInteract, "Vehicle interaction must be bound to G key.");

            // Verify mutual exclusivity
            Assert.AreNotEqual(GameInputManager.KeySuperPunch, GameInputManager.KeyVehicleInteract, "Punch and Vehicle interact must not share the same key.");

            // Other core bindings
            Assert.AreEqual(Key.Q, GameInputManager.KeyGravityTether, "Gravity Tether must be bound to Q.");
            Assert.AreEqual(Key.E, GameInputManager.KeyGrabInteract, "Grab/Interact must be bound to E.");
            Assert.AreEqual(Key.V, GameInputManager.KeyCameraToggle, "Camera perspective toggle must be bound to V.");
            Assert.AreEqual(Key.M, GameInputManager.KeyMapToggle, "Full-screen map modal must be bound to M.");
        }

        [Test]
        public void VehicleModelGenerator_ConstructsSubPartsForAutoRickshaw()
        {
            GameObject root = new GameObject("TestAutoRickshawRoot");
            Material testMat = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard"));

            VehicleModelGenerator.GenerateVehicleModel(
                root,
                IndianVehicleType.AutoRickshaw,
                testMat, testMat, testMat, testMat, testMat, testMat
            );

            Transform model = root.transform.Find("AutoRickshaw_DetailedMesh");
            Assert.IsNotNull(model, "AutoRickshaw_DetailedMesh container must be created.");

            // Verify three wheels are generated (Single front + 2 rear)
            int wheelCount = 0;
            foreach (Transform child in model)
            {
                if (child.name == "Wheel") wheelCount++;
            }
            Assert.AreEqual(3, wheelCount, "Auto-Rickshaw must have exactly 3 wheels.");

            Object.DestroyImmediate(root);
            Object.DestroyImmediate(testMat);
        }

        [Test]
        public void StreetFurniture_SpawnsUtilityPoleAndTeaStall()
        {
            GameObject parent = new GameObject("TestFurnitureParent");
            Material testMat = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard"));

            GameObject pole = StreetFurnitureSystem.SpawnConcreteUtilityPole(parent.transform, Vector3.zero, testMat, testMat);
            Assert.IsNotNull(pole, "Utility pole must be generated.");
            Assert.IsTrue(pole.transform.childCount >= 3, "Utility pole must have mast, crossarms, and transformer.");

            GameObject stall = StreetFurnitureSystem.SpawnTeaStall(parent.transform, new Vector3(5f, 0f, 0f), testMat, testMat, testMat, testMat);
            Assert.IsNotNull(stall, "Tea stall (Chai kadai) must be generated.");
            Assert.IsTrue(stall.transform.childCount >= 6, "Tea stall must have counter, roof, samovar, bench, and warm bulb.");

            Object.DestroyImmediate(parent);
            Object.DestroyImmediate(testMat);
        }

        [Test]
        public void AtmosphereSystem_AppliesGoldenSunsetAndRainyPresets()
        {
            GameObject atmoObj = new GameObject("TestAtmosphere");
            GameObject sunObj = new GameObject("TestSun");
            Light sun = sunObj.AddComponent<Light>();
            sun.type = LightType.Directional;

            var atmo = atmoObj.AddComponent<KaraikalAtmosphereSystem>();
            atmo.Initialize(sun);

            // Test Sunset
            atmo.SetWeatherMode(KaraikalWeatherMode.GoldenSunset);
            Assert.AreEqual(KaraikalWeatherMode.GoldenSunset, atmo.CurrentMode);
            Assert.IsTrue(sun.color.r > sun.color.b, "Sunset sun must have golden amber color (Red > Blue).");

            // Test Rainy
            atmo.SetWeatherMode(KaraikalWeatherMode.RainyEvening);
            Assert.AreEqual(KaraikalWeatherMode.RainyEvening, atmo.CurrentMode);
            Assert.IsTrue(RenderSettings.fogDensity > 0.003f, "Rainy evening must feature denser coastal fog.");

            Object.DestroyImmediate(atmoObj);
            Object.DestroyImmediate(sunObj);
        }

        [Test]
        public void SuperheroVisual_BuildsArticulatedRigAndDynamicCape()
        {
            GameObject hero = new GameObject("TestHero");
            hero.AddComponent<Rigidbody>();
            hero.AddComponent<SuperheroFlightController>();
            var visual = hero.AddComponent<SuperheroHumanoidVisual>();
            visual.BuildHumanoidGeometry();

            Transform rig = hero.transform.Find("HumanoidHero_VisualRig");
            Assert.IsNotNull(rig, "HumanoidHero_VisualRig must be created.");

            // Verify dynamic cape segments exist
            Transform chest = rig.Find("Hips/Chest");
            Assert.IsNotNull(chest, "Chest bone must exist.");
            Transform capeSeg0 = chest.Find("CapeSegment_0");
            Assert.IsNotNull(capeSeg0, "Dynamic cape must be anchored to chest/pauldrons.");

            Object.DestroyImmediate(hero);
        }
    }
}
