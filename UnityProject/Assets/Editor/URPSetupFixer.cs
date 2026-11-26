using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using CSVS.GodMode.Settings;
using CSVS.GodMode.Environment;

namespace CSVS.GodMode.Editor
{
    public static class URPSetupFixer
    {
        public static void FixAndConfigureURP()
        {
            Debug.Log("[URPSetupFixer] Starting URP configuration hotfix...");

            // 1. Ensure Settings & Materials directories exist
            if (!AssetDatabase.IsValidFolder("Assets/Settings"))
            {
                AssetDatabase.CreateFolder("Assets", "Settings");
            }
            if (!AssetDatabase.IsValidFolder("Assets/Materials"))
            {
                AssetDatabase.CreateFolder("Assets", "Materials");
            }

            // 2. Create or load UniversalRendererData
            string rendererPath = "Assets/Settings/UniversalRendererData.asset";
            UniversalRendererData rendererData = AssetDatabase.LoadAssetAtPath<UniversalRendererData>(rendererPath);
            if (rendererData == null)
            {
                rendererData = ScriptableObject.CreateInstance<UniversalRendererData>();
                AssetDatabase.CreateAsset(rendererData, rendererPath);
                Debug.Log($"[URPSetupFixer] Created UniversalRendererData at {rendererPath}");
            }

            // 3. Create or load UniversalRenderPipelineAsset
            string pipelineAssetPath = "Assets/Settings/UniversalRenderPipelineAsset.asset";
            UniversalRenderPipelineAsset urpAsset = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(pipelineAssetPath);
            if (urpAsset == null)
            {
                urpAsset = UniversalRenderPipelineAsset.Create(rendererData);
                AssetDatabase.CreateAsset(urpAsset, pipelineAssetPath);
                Debug.Log($"[URPSetupFixer] Created UniversalRenderPipelineAsset at {pipelineAssetPath}");
            }

            // Configure URP performance properties for target hardware
            urpAsset.renderScale = 0.75f;
            urpAsset.shadowDistance = 35.0f;
            urpAsset.supportsHDR = false;
            urpAsset.msaaSampleCount = 1;
            EditorUtility.SetDirty(urpAsset);

            // 4. Assign URP Asset to GraphicsSettings and QualitySettings
            GraphicsSettings.defaultRenderPipeline = urpAsset;
            Debug.Log("[URPSetupFixer] Assigned defaultRenderPipeline in GraphicsSettings");

            // Assign across all Quality levels
            for (int i = 0; i < QualitySettings.names.Length; i++)
            {
                QualitySettings.SetQualityLevel(i, false);
                QualitySettings.renderPipeline = urpAsset;
            }
            Debug.Log($"[URPSetupFixer] Assigned renderPipeline across all {QualitySettings.names.Length} Quality levels");

            // 5. Create or verify URP Materials
            Shader litShader = Shader.Find("Universal Render Pipeline/Lit");
            if (litShader == null) litShader = Shader.Find("Universal Render Pipeline/Simple Lit");
            if (litShader == null) litShader = Shader.Find("Universal Render Pipeline/Unlit");
            Debug.Log($"[URPSetupFixer] Using URP Shader: {litShader?.name}");

            Material roadMat = CreateOrUpdateMaterial("Assets/Materials/RoadMaterial.mat", litShader, new Color(0.18f, 0.20f, 0.22f, 1f), 0.1f);
            Material buildingMat = CreateOrUpdateMaterial("Assets/Materials/BuildingMaterial.mat", litShader, new Color(0.72f, 0.76f, 0.80f, 1f), 0.35f);
            Material chunkMat = CreateOrUpdateMaterial("Assets/Materials/BuildingChunkMaterial.mat", litShader, new Color(0.58f, 0.54f, 0.50f, 1f), 0.15f);
            Material playerMat = CreateOrUpdateMaterial("Assets/Materials/PlayerMaterial.mat", litShader, new Color(0.12f, 0.38f, 0.85f, 1f), 0.65f);

            // 6. Open MainCityScene.unity, inspect and update components
            string scenePath = "Assets/Scenes/MainCityScene.unity";
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

            // Camera Setup
            Camera cam = Camera.main;
            if (cam != null)
            {
                var additionalCamData = cam.GetComponent<UniversalAdditionalCameraData>();
                if (additionalCamData == null)
                {
                    additionalCamData = cam.gameObject.AddComponent<UniversalAdditionalCameraData>();
                    Debug.Log("[URPSetupFixer] Added UniversalAdditionalCameraData to Main Camera");
                }
                additionalCamData.renderShadows = true;
                EditorUtility.SetDirty(cam.gameObject);
            }

            // Directional Light Setup
            Light dirLight = Object.FindFirstObjectByType<Light>();
            if (dirLight != null)
            {
                var additionalLightData = dirLight.GetComponent<UniversalAdditionalLightData>();
                if (additionalLightData == null)
                {
                    additionalLightData = dirLight.gameObject.AddComponent<UniversalAdditionalLightData>();
                    Debug.Log("[URPSetupFixer] Added UniversalAdditionalLightData to Directional Light");
                }
                EditorUtility.SetDirty(dirLight.gameObject);
            }

            // Wire CityGenerator materials
            CityGenerator cityGen = Object.FindFirstObjectByType<CityGenerator>();
            if (cityGen != null)
            {
                var so = new SerializedObject(cityGen);
                var propBuilding = so.FindProperty("_buildingMaterial");
                if (propBuilding != null) propBuilding.objectReferenceValue = buildingMat;
                var propRoad = so.FindProperty("_roadMaterial");
                if (propRoad != null) propRoad.objectReferenceValue = roadMat;
                var propChunk = so.FindProperty("_chunkMaterial");
                if (propChunk != null) propChunk.objectReferenceValue = chunkMat;
                var propHero = so.FindProperty("_heroMaterial");
                if (propHero != null) propHero.objectReferenceValue = playerMat;
                so.ApplyModifiedProperties();
                Debug.Log("[URPSetupFixer] Serialized materials on CityGenerator");
            }

            // Wire QualitySettingsManager URP asset
            QualitySettingsManager qm = Object.FindFirstObjectByType<QualitySettingsManager>();
            if (qm != null)
            {
                var so = new SerializedObject(qm);
                var propUrp = so.FindProperty("urpAsset");
                if (propUrp != null) propUrp.objectReferenceValue = urpAsset;
                so.ApplyModifiedProperties();
                Debug.Log("[URPSetupFixer] Serialized urpAsset on QualitySettingsManager");
            }

            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Debug.Log("[URPSetupFixer] Hotfix completed and scene saved successfully!");
        }

        private static Material CreateOrUpdateMaterial(string path, Shader shader, Color baseColor, float smoothness)
        {
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null)
            {
                mat = new Material(shader);
                AssetDatabase.CreateAsset(mat, path);
                Debug.Log($"[URPSetupFixer] Created material at {path}");
            }
            else
            {
                mat.shader = shader;
            }

            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", baseColor);
            else if (mat.HasProperty("_Color")) mat.SetColor("_Color", baseColor);

            if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", smoothness);

            EditorUtility.SetDirty(mat);
            return mat;
        }

        public static void VerifySceneRendering()
        {
            Debug.Log("[URPSetupFixer] Verifying scene camera rendering...");
            string scenePath = "Assets/Scenes/MainCityScene.unity";
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

            // Trigger CityGenerator in editor if needed to have geometry
            CityGenerator cityGen = Object.FindFirstObjectByType<CityGenerator>();
            if (cityGen != null)
            {
                cityGen.GenerateCitySandbox();
            }

            Camera cam = Camera.main;
            if (cam == null)
            {
                Debug.LogError("[URPSetupFixer] Main Camera not found for rendering test!");
                EditorApplication.Exit(1);
                return;
            }

            RenderTexture rt = new RenderTexture(1280, 720, 24, RenderTextureFormat.ARGB32);
            cam.targetTexture = rt;
            RenderTexture.active = rt;

            cam.Render();

            Texture2D tex = new Texture2D(1280, 720, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0);
            tex.Apply();

            cam.targetTexture = null;
            RenderTexture.active = null;
            Object.DestroyImmediate(rt);

            Color[] pixels = tex.GetPixels();
            int totalPixels = pixels.Length;
            int magentaCount = 0;
            int nonMagentaCount = 0;

            for (int i = 0; i < totalPixels; i++)
            {
                Color c = pixels[i];
                // Unity error magenta is roughly R > 0.85, G < 0.15, B > 0.85
                if (c.r > 0.8f && c.g < 0.15f && c.b > 0.8f)
                {
                    magentaCount++;
                }
                else
                {
                    nonMagentaCount++;
                }
            }

            float magentaRatio = (float)magentaCount / totalPixels;
            Debug.Log($"[URPSetupFixer] Rendered test frame: Total={totalPixels}, MagentaPixels={magentaCount} ({magentaRatio * 100:F2}%), ValidPixels={nonMagentaCount}");

            string screenshotDir = @"D:\Development\CSVS-GodMode\Screenshots";
            if (!Directory.Exists(screenshotDir)) Directory.CreateDirectory(screenshotDir);
            string outPath = Path.Combine(screenshotDir, "scene_render_verification.png");
            File.WriteAllBytes(outPath, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);

            Debug.Log($"[URPSetupFixer] Saved render verification screenshot to: {outPath}");

            if (magentaRatio > 0.05f)
            {
                Debug.LogError($"[URPSetupFixer] FAILED: Excessive magenta error pixels detected ({magentaRatio * 100:F1}%)!");
                EditorApplication.Exit(1);
            }
            else
            {
                Debug.Log("[URPSetupFixer] SUCCESS: Scene rendered normally with zero magenta error shaders!");
                EditorApplication.Exit(0);
            }
        }
    }
}
