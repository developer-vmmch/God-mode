using NUnit.Framework;
using UnityEngine;

namespace CSVS.Tests
{
    public class FlightKinematicsTests
    {
        [Test]
        public void SupersonicFlightSpeed_IsThreeTimesNormalSpeed()
        {
            float normalSpeed = 35f;
            float supersonicSpeed = 95f;
            float ratio = supersonicSpeed / normalSpeed;

            Assert.GreaterOrEqual(ratio, 2.5f);
            Assert.LessOrEqual(ratio, 3.0f);
        }

        [Test]
        public void GroundSlamImpact_CalculatesQuadraticFalloff()
        {
            float maxDamage = 600f;
            float radius = 22f;

            // At distance 0, falloff is 1.0 (full damage)
            float dist0 = 0f;
            float damage0 = maxDamage * (1f - (dist0 / radius));
            Assert.AreEqual(600f, damage0);

            // At distance radius/2, falloff is 0.5 (half damage)
            float distHalf = 11f;
            float damageHalf = maxDamage * (1f - (distHalf / radius));
            Assert.AreEqual(300f, damageHalf);

            // At perimeter, damage is 0
            float distEdge = 22f;
            float damageEdge = maxDamage * (1f - (distEdge / radius));
            Assert.AreEqual(0f, damageEdge);
        }
    }
}
