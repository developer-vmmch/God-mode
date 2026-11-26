using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using CSVS.GodMode.Environment;
using CSVS.GodMode.UI;

namespace CSVS.GodMode.Editor
{
    public static class KaraikalCaptureVerification
    {
        public static void CaptureScreenshots()
        {
            Debug.Log("[KaraikalCaptureVerification] Initializing Karaikal Open World scene capture...");
            string scenePath = "Assets/Scenes/MainCityScene.unity";
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

            CityGenerator cityGen = Object.FindFirstObjectByType<CityGenerator>();
            if (cityGen != null)
            {
                cityGen.GenerateCitySandbox();
                Debug.Log("[KaraikalCaptureVerification] Generated Karaikal open world environment.");
            }

            Camera cam = Camera.main;
            if (cam == null)
            {
                Debug.LogError("[KaraikalCaptureVerification] Main Camera not found!");
                EditorApplication.Exit(1);
                return;
            }

            string screenshotDir = @"D:\Development\CSVS-GodMode\Screenshots";
            if (!Directory.Exists(screenshotDir)) Directory.CreateDirectory(screenshotDir);

            // 1. Capture Town Center Screenshot
            // Position camera behind superhero facing Kamarajar Salai and modular bazaar buildings
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                cam.transform.position = player.transform.position - player.transform.forward * 4.5f + Vector3.up * 2.2f;
                cam.transform.LookAt(player.transform.position + Vector3.up * 1.5f + player.transform.forward * 10f);
            }
            else
            {
                cam.transform.position = new Vector3(0f, 3f, -10f);
                cam.transform.LookAt(Vector3.forward * 20f);
            }

            string shot1Path = Path.Combine(screenshotDir, "gameplay_v021_town_center.png");
            CaptureCameraToFile(cam, shot1Path, "Town Center (Bazaar, Superhero & Kamarajar Salai)");

            // 2. Capture Aerial Overview / Minimap Perspective
            cam.transform.position = new Vector3(0f, 120f, -150f);
            cam.transform.LookAt(new Vector3(0f, 0f, 150f));

            string shot2Path = Path.Combine(screenshotDir, "gameplay_v021_minimap.png");
            CaptureCameraToFile(cam, shot2Path, "Karaikal Geographic Overview & Road Network");

            Debug.Log("[KaraikalCaptureVerification] All screenshots successfully generated.");
            EditorApplication.Exit(0);
        }

        private static void CaptureCameraToFile(Camera cam, string filePath, string label)
        {
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
            int total = pixels.Length;
            int magenta = 0;
            for (int i = 0; i < total; i++)
            {
                Color c = pixels[i];
                if (c.r > 0.8f && c.g < 0.15f && c.b > 0.8f) magenta++;
            }

            float magentaRatio = (float)magenta / total;
            File.WriteAllBytes(filePath, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);

            Debug.Log($"[KaraikalCaptureVerification] {label} saved to: {filePath}. Magenta={magenta} ({magentaRatio * 100:F2}%), TotalPixels={total}");

            if (magentaRatio > 0.02f)
            {
                Debug.LogError($"[KaraikalCaptureVerification] FAILED: Excessive magenta error pixels in {label} ({magentaRatio * 100:F1}%)!");
                EditorApplication.Exit(1);
            }
        }
    }
}
