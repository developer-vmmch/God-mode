using NUnit.Framework;
using UnityEngine;
using CSVS.GodMode.Powers;

namespace CSVS.GodMode.Tests
{
    [TestFixture]
    public class SuperStrengthTests
    {
        private GameObject _playerObj;
        private SuperStrengthGrab _grabSystem;

        [SetUp]
        public void SetUp()
        {
            _playerObj = new GameObject("TestPlayer");
            _grabSystem = _playerObj.AddComponent<SuperStrengthGrab>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_playerObj);
        }

        [Test]
        public void GrabSystem_StartsWithoutHoldingObject()
        {
            Assert.IsFalse(_grabSystem.IsHoldingObject);
            Assert.IsNull(_grabSystem.GrabbedObject);
        }

        [Test]
        public void ThrowVelocity_CalculatesLinearMomentumVector()
        {
            Vector3 aimDirection = Vector3.forward;
            float throwForce = 60.0f;
            Vector3 expectedVelocity = aimDirection * throwForce;

            Assert.AreEqual(new Vector3(0.0f, 0.0f, 60.0f), expectedVelocity);
        }
    }
}
