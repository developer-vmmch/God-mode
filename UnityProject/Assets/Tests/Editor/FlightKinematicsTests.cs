using NUnit.Framework;
using UnityEngine;
using CSVS.GodMode.Core;
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
        public void TargetMaxSpeed_ReflectsStandardizedSpeeds()
        {
            // Base flight speed is 35 m/s
            Assert.AreEqual(35.0f, _controller.TargetMaxSpeed, 0.1f, "Base flight speed must be 35 m/s.");
        }

        [Test]
        public void GroundSlam_ExactQuadraticRadialImpulseValidation()
        {
            // Equation: F = Fmax * Pow(Clamp01(1 - distance / radius), 2)
            float maxForce = 1500.0f;
            float maxRadius = 25.0f;

            // 1. Epicenter (d = 0): Expected 1500 N
            float fEpicenter = GameplayConfig.CalculateGroundSlamForce(0.0f, maxRadius, maxForce);
            Assert.AreEqual(1500.0f, fEpicenter, 0.001f, "Ground slam force at epicenter must equal exactly 1500 N.");

            // 2. Midpoint (d = 12.5): Expected 375 N (1500 * 0.5^2)
            float fMidpoint = GameplayConfig.CalculateGroundSlamForce(12.5f, maxRadius, maxForce);
            Assert.AreEqual(375.0f, fMidpoint, 0.001f, "Ground slam force at midpoint (radius/2) must equal exactly 375 N.");

            // 3. Perimeter (d = 25): Expected 0 N
            float fPerimeter = GameplayConfig.CalculateGroundSlamForce(25.0f, maxRadius, maxForce);
            Assert.AreEqual(0.0f, fPerimeter, 0.001f, "Ground slam force at perimeter must equal exactly 0 N.");

            // 4. Outside perimeter (d = 30): Expected 0 N
            float fOutside = GameplayConfig.CalculateGroundSlamForce(30.0f, maxRadius, maxForce);
            Assert.AreEqual(0.0f, fOutside, 0.001f, "Ground slam force outside perimeter must be clamped to 0 N.");
        }
    }
}
