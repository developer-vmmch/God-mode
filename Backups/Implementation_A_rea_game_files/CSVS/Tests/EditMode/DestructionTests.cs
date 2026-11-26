using NUnit.Framework;
using UnityEngine;
using CSVS.Destruction;

namespace CSVS.Tests
{
    public class DestructionTests
    {
        private GameObject buildingObj;
        private DestructibleBuilding building;
        private GameObject chunkObj;
        private BuildingChunk chunk;

        [SetUp]
        public void SetUp()
        {
            buildingObj = new GameObject("TestBuilding");
            building = buildingObj.AddComponent<DestructibleBuilding>();

            chunkObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            chunkObj.transform.SetParent(buildingObj.transform);
            chunk = chunkObj.AddComponent<BuildingChunk>();
        }

        [TearDown]
        public void TearDown()
        {
            if (chunkObj != null) Object.DestroyImmediate(chunkObj);
            if (buildingObj != null) Object.DestroyImmediate(buildingObj);
        }

        [Test]
        public void Chunk_InitializesKinematic_And_FullHealth()
        {
            Rigidbody rb = chunkObj.GetComponent<Rigidbody>();
            Assert.IsNotNull(rb);
            Assert.IsTrue(rb.isKinematic);
            Assert.IsFalse(chunk.IsFractured);
            Assert.AreEqual(1.0f, chunk.HealthPercentage);
        }

        [Test]
        public void Chunk_TakesDamage_And_FracturesAtZeroHealth()
        {
            chunk.ApplyDamage(200f, Vector3.zero);
            Assert.IsTrue(chunk.IsFractured);

            Rigidbody rb = chunkObj.GetComponent<Rigidbody>();
            Assert.IsFalse(rb.isKinematic);
        }

        [Test]
        public void DebrisManager_EnforcesHardBudget()
        {
            GameObject dmObj = new GameObject("DebrisManager_Test");
            DebrisManager dm = dmObj.AddComponent<DebrisManager>();
            dm.SetMaxActiveDebris(3);

            GameObject[] testChunks = new GameObject[5];
            for (int i = 0; i < 5; i++)
            {
                testChunks[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
                var c = testChunks[i].AddComponent<BuildingChunk>();
                c.DetachAndActivatePhysics();
            }

            Assert.LessOrEqual(dm.CurrentActiveDebrisCount, 3);

            for (int i = 0; i < 5; i++) Object.DestroyImmediate(testChunks[i]);
            Object.DestroyImmediate(dmObj);
        }
    }
}
