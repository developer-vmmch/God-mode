using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using CSVS.GodMode.Environment;
using CSVS.GodMode.UI;
using CSVS.GodMode.AI;

namespace CSVS.GodMode.Editor
{
    public static class KaraikalCaptureVerification
    {
        public static void CaptureScreenshots()
        {
            Debug.Log("[KaraikalCaptureVerification] Initializing Karaikal Open World V0.2.2 scene capture...");
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

            KaraikalAtmosphereSystem atmo = Object.FindFirstObjectByType<KaraikalAtmosphereSystem>();

            // 1. Capture Town Center Commercial Street (Kamarajar Salai)
            // Street-level perspective with hero in athletic navy/silver armor, tea stall, utility poles, auto-rickshaws
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                cam.transform.position = player.transform.position - player.transform.forward * 4.5f + Vector3.up * 2.0f;
                cam.transform.LookAt(player.transform.position + Vector3.up * 1.4f + player.transform.forward * 10f);
            }
            else
            {
                cam.transform.position = new Vector3(0f, 2.5f, -8f);
                cam.transform.LookAt(Vector3.forward * 20f);
            }

            string shot1Path = Path.Combine(screenshotDir, "gameplay_v022_town_center.png");
            CaptureCameraToFile(cam, shot1Path, "Town Center (Armored Superhero, Tea Stall, Utility Poles & Kamarajar Salai)");

            // 2. Capture Auto-Rickshaw Close-up
            GameObject auto = GameObject.Find("Vehicle_AutoRickshaw");
            if (auto != null)
            {
                cam.transform.position = auto.transform.position + auto.transform.forward * 3.5f + auto.transform.right * 2.5f + Vector3.up * 1.5f;
                cam.transform.LookAt(auto.transform.position + Vector3.up * 0.9f);
            }
            else
            {
                cam.transform.position = new Vector3(-13f, 1.8f, 28f);
                cam.transform.LookAt(new Vector3(-13f, 0.8f, 32f));
            }

            string shot2Path = Path.Combine(screenshotDir, "gameplay_v022_auto_rickshaw.png");
            CaptureCameraToFile(cam, shot2Path, "South Indian Auto-Rickshaw (Yellow Canopy, Black Body & Headlight)");

            // 3. Capture Golden Sunset / Coastal Twilight Atmosphere
            if (atmo != null)
            {
                atmo.SetWeatherMode(KaraikalWeatherMode.GoldenSunset);
            }
            cam.transform.position = new Vector3(12f, 18f, -40f);
            cam.transform.LookAt(new Vector3(0f, 4f, 60f));

            string shot3Path = Path.Combine(screenshotDir, "gameplay_v022_sunset_twilight.png");
            CaptureCameraToFile(cam, shot3Path, "Karaikal Golden Sunset & Twilight Street Lighting");

            // 4. Capture Karaikal Beach Promenade & Ocean Horizon
            cam.transform.position = new Vector3(880f, 14f, 20f);
            cam.transform.LookAt(new Vector3(1150f, 0f, 40f));

            string shot4Path = Path.Combine(screenshotDir, "gameplay_v022_beach_promenade.png");
            CaptureCameraToFile(cam, shot4Path, "Karaikal Beach Promenade & Bay of Bengal Coastline");

            Debug.Log("[KaraikalCaptureVerification] All V0.2.2 screenshots successfully generated.");
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
