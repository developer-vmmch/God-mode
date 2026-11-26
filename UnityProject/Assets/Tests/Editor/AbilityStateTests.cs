using NUnit.Framework;
using UnityEngine;
using CSVS.GodMode.Player;

namespace CSVS.GodMode.Tests
{
    [TestFixture]
    public class AbilityStateTests
    {
        private GameObject _playerObj;
        private SuperheroFlightController _flightController;

        [SetUp]
        public void SetUp()
        {
            _playerObj = new GameObject("TestHero");
            _playerObj.AddComponent<Rigidbody>();
            _flightController = _playerObj.AddComponent<SuperheroFlightController>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_playerObj);
        }

        [Test]
        public void InitialAbilityState_IsIdle()
        {
            Assert.AreEqual(AbilityState.Idle, _flightController.CurrentAbilityState);
            Assert.IsTrue(_flightController.CanActivateOffensiveAbility());
            Assert.IsTrue(_flightController.CanActivateHeatVision());
        }

        [Test]
        public void HoldingObject_LocksOutHeatVisionAndOffensiveAbilities()
        {
            _flightController.SetAbilityState(AbilityState.HoldingObject);

            Assert.AreEqual(AbilityState.HoldingObject, _flightController.CurrentAbilityState);
            Assert.IsFalse(_flightController.CanActivateOffensiveAbility(), "Cannot start another offensive ability while holding an object.");
            Assert.IsFalse(_flightController.CanActivateHeatVision(), "Heat vision must not fire while holding an object (LMB/RMB reserved for Throw/Crush).");
        }

        [Test]
        public void FiringHeatVision_PreventsConflictingGrab()
        {
            _flightController.SetAbilityState(AbilityState.HeatVision);

            Assert.AreEqual(AbilityState.HeatVision, _flightController.CurrentAbilityState);
            Assert.IsFalse(_flightController.CanActivateOffensiveAbility(), "Cannot initiate secondary offensive abilities while heat vision is firing.");
        }

        [Test]
        public void GravityTether_CoordinatesStateProperly()
        {
            _flightController.SetAbilityState(AbilityState.GravityTether);

            Assert.AreEqual(AbilityState.GravityTether, _flightController.CurrentAbilityState);
            Assert.IsFalse(_flightController.CanActivateOffensiveAbility());
            Assert.IsFalse(_flightController.CanActivateHeatVision());

            _flightController.SetAbilityState(AbilityState.Idle);
            Assert.IsTrue(_flightController.CanActivateOffensiveAbility());
        }
    }
}
