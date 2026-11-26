using NUnit.Framework;
using UnityEngine;
using CSVS.Core;

namespace CSVS.Tests
{
    public class ChaosManagerTests
    {
        private GameObject chaosGameObject;
        private ChaosManager chaosManager;

        [SetUp]
        public void SetUp()
        {
            chaosGameObject = new GameObject("ChaosManager_Test");
            chaosManager = chaosGameObject.AddComponent<ChaosManager>();
        }

        [TearDown]
        public void TearDown()
        {
            if (chaosGameObject != null)
            {
                Object.DestroyImmediate(chaosGameObject);
            }
        }

        [Test]
        public void ChaosScore_StartsAtZero()
        {
            Assert.AreEqual(0, chaosManager.TotalChaosScore);
            Assert.AreEqual(0, chaosManager.CurrentWantedLevel);
        }

        [Test]
        public void AddChaos_AccumulatesScore_And_IncreasesMultiplier()
        {
            chaosManager.AddChaos(500, "Destruction");
            Assert.AreEqual(500, chaosManager.TotalChaosScore);
            Assert.Greater(chaosManager.ChaosMultiplier, 1.0f);

            // Second hit benefits from streak multiplier
            chaosManager.AddChaos(500, "Destruction");
            Assert.Greater(chaosManager.TotalChaosScore, 1000);
        }

        [Test]
        public void AddChaos_EscalatesWantedLevel_AtThresholds()
        {
            // Level 1 threshold is 1000
            chaosManager.AddChaos(1200, "BigDestruction");
            Assert.AreEqual(1, chaosManager.CurrentWantedLevel);

            // Level 2 threshold is 5000
            chaosManager.AddChaos(10000, "MassiveCollapse");
            Assert.GreaterOrEqual(chaosManager.CurrentWantedLevel, 2);
        }
    }
}
