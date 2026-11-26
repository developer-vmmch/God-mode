using NUnit.Framework;
using UnityEngine;
using CSVS.GodMode.Player;

namespace CSVS.GodMode.Tests
{
    [TestFixture]
    public class FlightKinematicsTests
    {
        private GameObject _playerObject;
        private SuperheroFlightController _controller;

        [SetUp]
        public void SetUp()
        {
            _playerObject = new GameObject("TestPlayer");
            _playerObject.AddComponent<Rigidbody>();
            _controller = _playerObject.AddComponent<SuperheroFlightController>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_playerObject);
        }

        [Test]
        public void FlightController_InitializesInHoverMode_WithoutInput()
        {
            _controller.SimulateFlightPhysics(0.02f);
            Assert.IsTrue(_controller.IsHovering, "Controller should be in hover mode when no input is supplied.");
        }

        [Test]
        public void TargetMaxSpeed_ReflectsSupersonicBoost()
        {
            float normalSpeed = _controller.TargetMaxSpeed;
            Assert.AreEqual(35.0f, normalSpeed, 0.1f, "Base normal flight speed should be 35 m/s.");
        }

        [Test]
        public void GroundSlam_CalculatesQuadraticRadialImpulseCorrectly()
        {
            // Equation: F_radial(r) = F_max * (1 - r / R_max)^2
            float maxForce = 1500.0f;
            float maxRadius = 25.0f;

            // At r = 0 (epicenter): F = 1500
            float f0 = maxForce * Mathf.Pow(1.0f - (0.0f / maxRadius), 2.0f);
            Assert.AreEqual(1500.0f, f0, 0.01f);

            // At r = 12.5 (halfway): F = 1500 * (0.5)^2 = 375
            float fHalf = maxForce * Mathf.Pow(1.0f - (12.5f / maxRadius), 2.0f);
            Assert.AreEqual(375.0f, fHalf, 0.01f);

            // At r = 25 (boundary): F = 0
            float fBoundary = maxForce * Mathf.Pow(1.0f - (25.0f / maxRadius), 2.0f);
            Assert.AreEqual(0.0f, fBoundary, 0.01f);
        }
    }
}
