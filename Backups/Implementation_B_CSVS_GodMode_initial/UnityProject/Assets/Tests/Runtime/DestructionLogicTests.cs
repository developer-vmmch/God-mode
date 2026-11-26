using NUnit.Framework;
using UnityEngine;
using CSVS.GodMode.Destruction;

namespace CSVS.GodMode.Tests
{
    [TestFixture]
    public class DestructionLogicTests
    {
        private GameObject _managerObj;
        private DebrisPoolManager _poolManager;

        [SetUp]
        public void SetUp()
        {
            _managerObj = new GameObject("TestPoolManager");
            _poolManager = _managerObj.AddComponent<DebrisPoolManager>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_managerObj);
        }

        [Test]
        public void DebrisPoolManager_EnforcesHardCapOf150Bodies()
        {
            Assert.AreEqual(150, _poolManager.MaxActiveBudget, "Active rigid body budget must not exceed 150 for WebGL.");

            // Create 155 test chunks and register them
            GameObject[] testObjects = new GameObject[155];
            for (int i = 0; i < 155; i++)
            {
                testObjects[i] = new GameObject($"TestChunk_{i}");
                testObjects[i].AddComponent<Rigidbody>();
                BuildingFractureChunk chunk = testObjects[i].AddComponent<BuildingFractureChunk>();
                _poolManager.RegisterActiveChunk(chunk);
            }

            // Verify active count never exceeds the 150 cap
            Assert.LessOrEqual(_poolManager.CurrentActiveCount, 150, "Active dynamic chunks must be clamped to the 150 ceiling.");

            for (int i = 0; i < 155; i++)
            {
                Object.DestroyImmediate(testObjects[i]);
            }
        }

        [Test]
        public void FractureChunk_FreezesWhenVelocityBelowThreshold()
        {
            GameObject chunkObj = new GameObject("SettlingChunk");
            Rigidbody rb = chunkObj.AddComponent<Rigidbody>();
            BuildingFractureChunk chunk = chunkObj.AddComponent<BuildingFractureChunk>();

            Assert.IsFalse(chunk.IsDestroyed);

            // Execute freeze
            chunk.FreezeToStatic();

            Assert.IsTrue(rb.isKinematic, "Rigidbody must convert to kinematic upon freezing.");
            Assert.IsTrue(chunk.IsDestroyed, "Chunk must report destroyed/frozen state.");

            Object.DestroyImmediate(chunkObj);
        }
    }
}
