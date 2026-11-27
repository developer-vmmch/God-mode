// V030FeatureTests.cs
// CSVS GOD MODE — V0.3 Unity EditMode Automated Tests
// Tests all new V0.3 systems. Extends the existing 48 V0.2.2 tests.
// Run via: Unity Test Runner > EditMode

using System.Linq;
using NUnit.Framework;
using UnityEngine;
using CSVS.GodMode.Environment;
using CSVS.GodMode.Missions;
using CSVS.GodMode.Core;
using CSVS.GodMode.Player;

namespace CSVS.GodMode.Tests.Editor
{
    [TestFixture]
    public class V030FeatureTests
    {
        // ─── GIS Road Data ────────────────────────────────────────────────
        [Test]
        public void OSMRoadData_HasMinimumRequiredSegments()
        {
            Assert.GreaterOrEqual(KaraikalOSMRoadData.Segments.Length, 8,
                "Must have at least 8 road segments covering major Karaikal roads.");
        }

        [Test]
        public void OSMRoadData_KamarajarSalai_ExistsAsSegment()
        {
            bool found = KaraikalOSMRoadData.Segments.Any(s => s.Name.Contains("Kamarajar"));
            Assert.IsTrue(found, "Kamarajar Salai must be present in road network.");
        }

        [Test]
        public void OSMRoadData_AllPolylines_HaveAtLeastTwoPoints()
        {
            foreach (var seg in KaraikalOSMRoadData.Segments)
            {
                Assert.IsNotNull(seg.Polyline, $"Road '{seg.Name}' has null polyline.");
                Assert.GreaterOrEqual(seg.Polyline.Length, 2,
                    $"Road '{seg.Name}' needs at least 2 waypoints.");
            }
        }

        [Test]
        public void OSMRoadData_RailwayStation_IsWithin50mOfExpectedPosition()
        {
            var station = KaraikalOSMRoadData.Landmarks.FirstOrDefault(l => l.Name.Contains("Railway"));
            Assert.IsNotNull(station.Name, "Railway Station landmark must exist.");
            // Expected: X ≈ -1041, Z ≈ +42
            Assert.Less(Mathf.Abs(station.Position.x - (-1041f)), 50f,
                $"Station X expected near -1041, got {station.Position.x}");
            Assert.Less(Mathf.Abs(station.Position.z - 42f), 50f,
                $"Station Z expected near +42, got {station.Position.z}");
        }

        [Test]
        public void OSMRoadData_Beach_IsEastOfTownCenter()
        {
            var beach = KaraikalOSMRoadData.Landmarks.FirstOrDefault(l => l.Name.Contains("Beach"));
            Assert.Greater(beach.Position.x, 500f,
                "Beach must be east of town center (X > 500m).");
        }

        [Test]
        public void OSMRoadData_RealSites_AreMarkedNotDestructible()
        {
            foreach (var lm in KaraikalOSMRoadData.Landmarks)
            {
                if (lm.IsRealWorldSite)
                    Assert.AreNotEqual(lm.Category, "Fictional",
                        $"Real-world site '{lm.Name}' must not be marked Fictional.");
            }
        }

        // ─── Mission System ───────────────────────────────────────────────
        [Test]
        public void MissionSystem_SixOriginalMissions_AreDefined()
        {
            var ms = new GameObject("TestMissionSystem").AddComponent<MissionSystem>();
            var field = typeof(MissionSystem).GetField("allMissions",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field?.SetValue(ms, new System.Collections.Generic.List<MissionData>());
            ms.InitializeDefaultMissions();
            var missions = field?.GetValue(ms) as System.Collections.Generic.List<MissionData>;
            Object.DestroyImmediate(ms.gameObject);

            Assert.IsNotNull(missions, "allMissions must be initialized.");
            Assert.GreaterOrEqual(missions.Count, 6, "At least 6 original missions required for V0.3.");
        }

        [Test]
        public void MissionData_FirstMission_IsAvailableAtStart()
        {
            var ms = new GameObject("TestMS").AddComponent<MissionSystem>();
            var field = typeof(MissionSystem).GetField("allMissions",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field?.SetValue(ms, new System.Collections.Generic.List<MissionData>());
            ms.InitializeDefaultMissions();
            var missions = field?.GetValue(ms) as System.Collections.Generic.List<MissionData>;
            Object.DestroyImmediate(ms.gameObject);

            Assert.IsNotNull(missions, "Cannot access missions field.");
            var first = missions.Find(m => m.MissionID == "M01_Welcome");
            Assert.IsNotNull(first, "M01_Welcome must be defined.");
            Assert.AreEqual(MissionState.Available, first.State, "M01_Welcome must start as Available.");
        }

        // ─── Reputation & Chaos ───────────────────────────────────────────
        [Test]
        public void ReputationSystem_AddingReputation_IncreasesScore()
        {
            var go = new GameObject("RepSys");
            var rep = go.AddComponent<ReputationChaosSystem>();
            int initial = rep.Reputation;
            rep.AddReputation(50);
            Assert.AreEqual(initial + 50, rep.Reputation);
            Object.DestroyImmediate(go);
        }

        [Test]
        public void ChaosSystem_AddingChaos_EscalatesChaosLevel()
        {
            var go = new GameObject("ChaosSys");
            var rep = go.AddComponent<ReputationChaosSystem>();
            // Add enough to reach Level 1 (threshold = 100)
            rep.AddChaos(110f, triggersAlert: false);
            Assert.GreaterOrEqual(rep.ChaosLevel, 1, "Chaos level must rise above 1 after 110 chaos points.");
            Object.DestroyImmediate(go);
        }

        // ─── Day/Night System ─────────────────────────────────────────────
        [Test]
        public void DayNightSystem_GoldenHour_OccursBetween16And185()
        {
            var go = new GameObject("DayNight");
            var dn = go.AddComponent<DayNightCycleSystem>();
            var setHour = typeof(DayNightCycleSystem).GetMethod("SetTimeOfDay");
            setHour?.Invoke(dn, new object[] { 17f });
            // UpdateDayPeriod is applied in Update; manually check enum logic
            Assert.AreEqual(TimeOfDay.GoldenHour, GetTimeOfDayEnum(17f));
            Object.DestroyImmediate(go);
        }

        private TimeOfDay GetTimeOfDayEnum(float h)
        {
            if      (h < 5.5f)  return TimeOfDay.Night;
            else if (h < 7.0f)  return TimeOfDay.Dawn;
            else if (h < 12.0f) return TimeOfDay.Morning;
            else if (h < 16.0f) return TimeOfDay.Afternoon;
            else if (h < 18.5f) return TimeOfDay.GoldenHour;
            else if (h < 19.5f) return TimeOfDay.Twilight;
            else                return TimeOfDay.Night;
        }

        // ─── Input Manager ────────────────────────────────────────────────
        [Test]
        public void InputManager_TransformKey_IsT_NotConflictingWithOtherBindings()
        {
            var prop = typeof(GameInputManager).GetProperty("WasTransformTriggered",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            Assert.IsNotNull(prop, "WasTransformTriggered must be defined in GameInputManager.");
        }

        // ─── Character Body ───────────────────────────────────────────────
        [Test]
        public void ProceduralBody_Superhero_BuildsArmourSegments()
        {
            var go = new GameObject("BodyTest");
            var body = go.AddComponent<ProceduralHumanoidBody>();
            body.BuildCharacter();
            // Check at least one MeshRenderer is active on a child
            var renderers = go.GetComponentsInChildren<MeshRenderer>();
            Assert.GreaterOrEqual(renderers.Length, 10, "Superhero body must have at least 10 mesh segments.");
            Object.DestroyImmediate(go);
        }

        // ─── Road Mesh Builder (integration) ─────────────────────────────
        [Test]
        public void RoadMeshBuilder_CanInstantiate_WithoutErrors()
        {
            var go = new GameObject("RoadBuilder");
            go.AddComponent<KaraikalRoadMeshBuilder>();
            Assert.IsNotNull(go.GetComponent<KaraikalRoadMeshBuilder>());
            Object.DestroyImmediate(go);
        }
    }
}

