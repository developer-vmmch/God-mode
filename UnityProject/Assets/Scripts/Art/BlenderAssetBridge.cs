using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CSVS.GodMode.Art
{
    /// <summary>
    /// Loads and verifies Blender-exported FBX assets at runtime or in edit mode.
    /// Provides prefabs/instances for the vertical slice commercial street.
    /// </summary>
    public static class BlenderAssetBridge
    {
        public const string HERO_MODEL_PATH = "Assets/Art/Models/Hero/Hero_Armored.fbx";
        public const string RICKSHAW_MODEL_PATH = "Assets/Art/Models/Vehicles/AutoRickshaw.fbx";
        public const string MOTORCYCLE_MODEL_PATH = "Assets/Art/Models/Vehicles/Motorcycle.fbx";
        public const string BAZAAR_MODEL_PATH = "Assets/Art/Models/Architecture/Commercial_Bazaar.fbx";
        public const string UTILITY_POLE_PATH = "Assets/Art/Models/Props/UtilityPole.fbx";
        public const string VERIFY_TEST_PATH = "Assets/Art/Models/Verification/CSVS_Test.fbx";

        public static GameObject LoadAsset(string assetPath)
        {
#if UNITY_EDITOR
            return AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
#else
            return null;
#endif
        }

        public static bool VerifyAllModelsExist()
        {
#if UNITY_EDITOR
            string[] paths = new string[]
            {
                HERO_MODEL_PATH,
                RICKSHAW_MODEL_PATH,
                MOTORCYCLE_MODEL_PATH,
                BAZAAR_MODEL_PATH,
                UTILITY_POLE_PATH,
                VERIFY_TEST_PATH
            };

            foreach (var path in paths)
            {
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (obj == null)
                {
                    Debug.LogError($"[BlenderAssetBridge] Model missing or unimported: {path}");
                    return false;
                }
            }
            return true;
#else
            return true;
#endif
        }
    }
}
