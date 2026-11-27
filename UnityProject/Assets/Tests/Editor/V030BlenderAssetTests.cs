using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using CSVS.GodMode.Art;
using CSVS.GodMode.Environment;

namespace CSVS.GodMode.Tests.Editor
{
    [TestFixture]
    public class V030BlenderAssetTests
    {
        [Test]
        public void BlenderTestAsset_ImportsWithValidMeshAndMaterials()
        {
            var testObj = AssetDatabase.LoadAssetAtPath<GameObject>(BlenderAssetBridge.VERIFY_TEST_PATH);
            Assert.IsNotNull(testObj, "CSVS_Test.fbx must be imported into Unity AssetDatabase.");

            var mf = testObj.GetComponentInChildren<MeshFilter>();
            Assert.IsNotNull(mf, "CSVS_Test must contain a MeshFilter.");
            Assert.IsNotNull(mf.sharedMesh, "CSVS_Test mesh must not be null.");
            Assert.Greater(mf.sharedMesh.vertexCount, 0, "CSVS_Test mesh must have vertices.");

            var mr = testObj.GetComponentInChildren<MeshRenderer>();
            Assert.IsNotNull(mr, "CSVS_Test must contain a MeshRenderer.");
        }

        [Test]
        public void BlenderHeroModel_ImportsWithCorrectArmorGeometry()
        {
            var hero = AssetDatabase.LoadAssetAtPath<GameObject>(BlenderAssetBridge.HERO_MODEL_PATH);
            Assert.IsNotNull(hero, "Hero_Armored.fbx must be imported into Unity AssetDatabase.");

            var mf = hero.GetComponentInChildren<MeshFilter>();
            Assert.IsNotNull(mf, "Hero model must contain MeshFilter.");
            Assert.Greater(mf.sharedMesh.vertexCount, 100, "Hero model must have detailed polygon mesh.");

            var mr = hero.GetComponentInChildren<MeshRenderer>();
            Assert.IsNotNull(mr, "Hero model must have MeshRenderer.");
            Assert.GreaterOrEqual(mr.sharedMaterials.Length, 1, "Hero model must have materials assigned.");
        }

        [Test]
        public void BlenderAutoRickshawModel_ImportsWithThreeWheelsAndCanopy()
        {
            var rickshaw = AssetDatabase.LoadAssetAtPath<GameObject>(BlenderAssetBridge.RICKSHAW_MODEL_PATH);
            Assert.IsNotNull(rickshaw, "AutoRickshaw.fbx must be imported into Unity AssetDatabase.");

            var mf = rickshaw.GetComponentInChildren<MeshFilter>();
            Assert.IsNotNull(mf, "AutoRickshaw model must contain MeshFilter.");
            Assert.Greater(mf.sharedMesh.vertexCount, 150, "AutoRickshaw model must be a detailed multi-element mesh.");
        }

        [Test]
        public void BlenderCommercialBazaarModel_ImportsWithStorefrontsAndSignboards()
        {
            var bazaar = AssetDatabase.LoadAssetAtPath<GameObject>(BlenderAssetBridge.BAZAAR_MODEL_PATH);
            Assert.IsNotNull(bazaar, "Commercial_Bazaar.fbx must be imported into Unity AssetDatabase.");

            var mf = bazaar.GetComponentInChildren<MeshFilter>();
            Assert.IsNotNull(mf, "Commercial_Bazaar model must contain MeshFilter.");
            Assert.Greater(mf.sharedMesh.vertexCount, 100, "Commercial_Bazaar model must contain shop shutters and parapets.");
        }

        [Test]
        public void BlenderMotorcycleModel_ImportsWithFrameAndWheels()
        {
            var bike = AssetDatabase.LoadAssetAtPath<GameObject>(BlenderAssetBridge.MOTORCYCLE_MODEL_PATH);
            Assert.IsNotNull(bike, "Motorcycle.fbx must be imported into Unity AssetDatabase.");

            var mf = bike.GetComponentInChildren<MeshFilter>();
            Assert.IsNotNull(mf, "Motorcycle model must contain MeshFilter.");
            Assert.Greater(mf.sharedMesh.vertexCount, 100, "Motorcycle model must be detailed.");
        }

        [Test]
        public void BlenderUtilityPoleModel_ImportsWithTransformerAndLights()
        {
            var pole = AssetDatabase.LoadAssetAtPath<GameObject>(BlenderAssetBridge.UTILITY_POLE_PATH);
            Assert.IsNotNull(pole, "UtilityPole.fbx must be imported into Unity AssetDatabase.");

            var mf = pole.GetComponentInChildren<MeshFilter>();
            Assert.IsNotNull(mf, "UtilityPole model must contain MeshFilter.");
            Assert.Greater(mf.sharedMesh.vertexCount, 50, "UtilityPole model must contain crossarms and transformers.");
        }

        [Test]
        public void VerticalSliceManager_SpawnsBazaarStreetHierarchy()
        {
            var root = new GameObject("TestVerticalSliceRoot");
            var vsm = root.AddComponent<KaraikalVerticalSliceManager>();
            vsm.ConstructVerticalSliceStreet();

            var street = root.transform.Find("Karaikal_VerticalSlice_CommercialStreet");
            Assert.IsNotNull(street, "Vertical slice street container must be spawned.");

            var road = street.Find("VerticalSlice_Road_Asphalt");
            Assert.IsNotNull(road, "Asphalt road must be instantiated.");

            Object.DestroyImmediate(root);
        }
    }
}
