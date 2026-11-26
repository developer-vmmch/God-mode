using NUnit.Framework;
using UnityEngine;
using CSVS.GodMode.Core;

namespace CSVS.GodMode.Tests
{
    [TestFixture]
    public class ChaosSystemTests
    {
        private GameObject _chaosObj;
        private ChaosSystem _chaosSystem;

        [SetUp]
        public void SetUp()
        {
            _chaosObj = new GameObject("TestChaosSystem");
            _chaosSystem = _chaosObj.AddComponent<ChaosSystem>();
            _chaosSystem.ResetScore();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_chaosObj);
        }

        [Test]
        public void InitialWantedTier_IsPeaceful()
        {
            Assert.AreEqual(WantedTier.Peaceful, _chaosSystem.CurrentWantedTier);
            Assert.AreEqual(0, _chaosSystem.CurrentChaosScore);
        }

        [Test]
        public void AddChaos_AccumulatesScoreAndAppliesMultiplier()
        {
            _chaosSystem.SetMultiplier(2.0f);
            _chaosSystem.AddChaos(5000, "TestBuilding");

            Assert.AreEqual(10000, _chaosSystem.CurrentChaosScore, "Score should be multiplied by 2x.");
        }

        [Test]
        public void WantedTier_EscalatesToPolice_AtThreshold()
        {
            WantedTier capturedTier = WantedTier.Peaceful;
            _chaosSystem.OnWantedTierEscalated += tier => capturedTier = tier;

            _chaosSystem.AddChaos(12000, "CityDestruction");

            Assert.AreEqual(WantedTier.Police, _chaosSystem.CurrentWantedTier);
            Assert.AreEqual(WantedTier.Police, capturedTier);
        }

        [Test]
        public void WantedTier_EscalatesToHelicopters_AtMaxThreshold()
        {
            _chaosSystem.AddChaos(600000, "CityWideCataclysm");
            Assert.AreEqual(WantedTier.AttackHelicopters, _chaosSystem.CurrentWantedTier);
        }
    }
}
