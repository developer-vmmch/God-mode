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
        public void DebrisPoolManager_ConfiguresStandardizedBudgets()
        {
            // Low = 50, Med = 100, High = 150, Hard Cap = 150
            Assert.AreEqual(150, _poolManager.HardCapCeiling, "Hard cap ceiling must be 150 dynamic bodies.");

            _poolManager.SetBudget(50);
            Assert.AreEqual(50, _poolManager.MaxActiveBudget, "Low graphics debris budget must equal 50.");

            _poolManager.SetBudget(100);
            Assert.AreEqual(100, _poolManager.MaxActiveBudget, "Medium graphics debris budget must equal 100.");

            _poolManager.SetBudget(150);
            Assert.AreEqual(150, _poolManager.MaxActiveBudget, "High graphics debris budget must equal 150.");

            // Exceeding ceiling clamps to 150
            _poolManager.SetBudget(250);
            Assert.AreEqual(150, _poolManager.MaxActiveBudget, "Budget exceeding 150 must clamp to 150 ceiling.");
        }

        [Test]
        public void DebrisPoolManager_EnforcesLowGraphicsCapOf50Bodies()
        {
            _poolManager.SetBudget(50);

            // Create 55 test chunks and register them
            GameObject[] testObjects = new GameObject[55];
            for (int i = 0; i < 55; i++)
            {
                testObjects[i] = new GameObject($"TestChunk_{i}");
                testObjects[i].AddComponent<Rigidbody>();
                BuildingFractureChunk chunk = testObjects[i].AddComponent<BuildingFractureChunk>();
                _poolManager.RegisterActiveChunk(chunk);
            }

            // Verify active count never exceeds the 50 cap
            Assert.LessOrEqual(_poolManager.CurrentActiveCount, 50, "Active dynamic chunks on Low must be clamped to 50.");

            for (int i = 0; i < 55; i++)
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
