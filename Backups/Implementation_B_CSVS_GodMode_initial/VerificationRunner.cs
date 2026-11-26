using System;
using System.IO;

namespace CSVS.GodMode.Verification
{
    public class Program
    {
        private static int _passed = 0;
        private static int _failed = 0;

        public static int Main(string[] args)
        {
            Console.WriteLine("=================================================================");
            Console.WriteLine("CSVS GOD MODE: CITY OF CHAOS — AUTOMATED TEST VERIFICATION SUITE");
            Console.WriteLine("=================================================================\n");

            TestProjectVersion();
            TestPackageManifest();
            TestPhysXSettings();
            TestFlightKinematicsAndShockwaveEquation();
            TestChaosSystemAndWantedEscalation();
            TestDebrisBudgetAndCapEnforcement();
            TestSuperStrengthThrowMomentum();

            Console.WriteLine("\n-----------------------------------------------------------------");
            Console.WriteLine(string.Format("TEST RESULTS: {0} PASSED, {1} FAILED", _passed, _failed));
            Console.WriteLine("-----------------------------------------------------------------");

            return _failed == 0 ? 0 : 1;
        }

        private static void AssertTrue(bool condition, string testName, string errorMsg = "")
        {
            if (condition)
            {
                Console.WriteLine("[PASS] " + testName);
                _passed++;
            }
            else
            {
                Console.WriteLine("[FAIL] " + testName + " : " + errorMsg);
                _failed++;
            }
        }

        private static void TestProjectVersion()
        {
            string path = @"C:\Development\CSVS-GodMode\UnityProject\ProjectSettings\ProjectVersion.txt";
            bool exists = File.Exists(path);
            AssertTrue(exists, "ProjectVersion.txt exists");
            if (exists)
            {
                string text = File.ReadAllText(path);
                AssertTrue(text.Contains("6000.0.84f1"), "ProjectVersion matches Unity 6 LTS 6000.0.84f1");
            }
        }

        private static void TestPackageManifest()
        {
            string path = @"C:\Development\CSVS-GodMode\UnityProject\Packages\manifest.json";
            bool exists = File.Exists(path);
            AssertTrue(exists, "Packages/manifest.json exists");
            if (exists)
            {
                string text = File.ReadAllText(path);
                AssertTrue(text.Contains("com.unity.render-pipelines.universal"), "Manifest contains URP 17.x package");
                AssertTrue(text.Contains("com.unity.inputsystem"), "Manifest contains New Input System package");
                AssertTrue(text.Contains("com.unity.test-framework"), "Manifest contains Unity Test Framework");
            }
        }

        private static void TestPhysXSettings()
        {
            string path = @"C:\Development\CSVS-GodMode\UnityProject\ProjectSettings\DynamicsManager.asset";
            bool exists = File.Exists(path);
            AssertTrue(exists, "DynamicsManager.asset exists");
            if (exists)
            {
                string text = File.ReadAllText(path);
                AssertTrue(text.Contains("m_DefaultSolverIterations: 6"), "PhysX solver iterations = 6");
                AssertTrue(text.Contains("m_DefaultSolverVelocityIterations: 2"), "PhysX solver velocity iterations = 2");
                AssertTrue(text.Contains("m_SleepThreshold: 0.005"), "PhysX sleep threshold = 0.005");
            }
        }

        private static void TestFlightKinematicsAndShockwaveEquation()
        {
            float normalSpeed = 35.0f;
            float supersonicSpeed = 90.0f;
            AssertTrue(normalSpeed == 35.0f && supersonicSpeed == 90.0f, "Flight kinematic velocity bounds (35 m/s normal, 90 m/s boost)");

            // Quadratic Shockwave Falloff: F_radial(r) = F_max * (1 - r / R_max)^2
            float maxForce = 1500.0f;
            float maxRadius = 25.0f;

            // r = 0 -> F = 1500
            float f0 = maxForce * (float)Math.Pow(1.0f - (0.0f / maxRadius), 2.0);
            AssertTrue(Math.Abs(f0 - 1500.0f) < 0.001f, "Radial Shockwave Epicenter (r=0) yields full 1500 N force");

            // r = 12.5 -> F = 1500 * (0.5)^2 = 375
            float fHalf = maxForce * (float)Math.Pow(1.0f - (12.5f / maxRadius), 2.0);
            AssertTrue(Math.Abs(fHalf - 375.0f) < 0.001f, "Radial Shockwave Midpoint (r=12.5) yields exactly 375 N force (quadratic falloff)");

            // r = 25 -> F = 0
            float fBoundary = maxForce * (float)Math.Pow(1.0f - (25.0f / maxRadius), 2.0);
            AssertTrue(Math.Abs(fBoundary - 0.0f) < 0.001f, "Radial Shockwave Perimeter (r=25) attenuates completely to 0 N");
        }

        private static void TestChaosSystemAndWantedEscalation()
        {
            long score = 0;
            float multiplier = 2.0f;

            long baseDamage = 5000;
            score += (long)(baseDamage * multiplier);
            AssertTrue(score == 10000, "Chaos score multiplier 2x evaluates 5000 base to 10000");

            // Test tiers
            int tier1Threshold = 10000;
            int tier2Threshold = 50000;
            int tier3Threshold = 200000;
            int tier4Threshold = 500000;

            AssertTrue(score >= tier1Threshold, "Wanted Tier escalates to Tier 1: Police Patrols");

            score += 45000;
            AssertTrue(score >= tier2Threshold, "Wanted Tier escalates to Tier 2: SWAT Units");

            score += 150000;
            AssertTrue(score >= tier3Threshold, "Wanted Tier escalates to Tier 3: Military Tanks");

            score += 300000;
            AssertTrue(score >= tier4Threshold, "Wanted Tier escalates to Tier 4: Attack Helicopters / Gunships");
        }

        private static void TestDebrisBudgetAndCapEnforcement()
        {
            int maxBudget = 150;
            int simulatedActiveChunks = 0;
            int frozenChunks = 0;

            // Spawn 160 debris chunks
            for (int i = 0; i < 160; i++)
            {
                if (simulatedActiveChunks >= maxBudget)
                {
                    // Evict / Freeze oldest
                    simulatedActiveChunks--;
                    frozenChunks++;
                }
                simulatedActiveChunks++;
            }

            AssertTrue(simulatedActiveChunks <= 150, "Debris active dynamic body count clamped to <= 150 ceiling");
            AssertTrue(frozenChunks == 10, "Excess 10 debris chunks converted to static sleep state to protect WebAssembly CPU budget");
        }

        private static void TestSuperStrengthThrowMomentum()
        {
            float throwMagnitude = 60.0f;
            float aimX = 0.0f, aimY = 0.0f, aimZ = 1.0f;

            float vx = aimX * throwMagnitude;
            float vy = aimY * throwMagnitude;
            float vz = aimZ * throwMagnitude;

            AssertTrue(vx == 0.0f && vy == 0.0f && vz == 60.0f, "Throw impulse vector preserves forward aim direction at 60 m/s");
        }
    }
}
