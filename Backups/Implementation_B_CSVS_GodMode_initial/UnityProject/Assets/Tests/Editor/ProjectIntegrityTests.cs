using System.IO;
using NUnit.Framework;
using UnityEngine;

namespace CSVS.GodMode.Tests.Editor
{
    [TestFixture]
    public class ProjectIntegrityTests
    {
        [Test]
        public void ProjectVersion_MatchesUnity6LTS()
        {
            string projectVersionPath = Path.Combine(Application.dataPath, "../ProjectSettings/ProjectVersion.txt");
            Assert.IsTrue(File.Exists(projectVersionPath), "ProjectVersion.txt must exist.");

            string content = File.ReadAllText(projectVersionPath);
            Assert.IsTrue(content.Contains("6000.0.84f1"), "Project must target Unity 6 LTS 6000.0.84f1.");
        }

        [Test]
        public void PackageManifest_ContainsURPAndInputSystem()
        {
            string manifestPath = Path.Combine(Application.dataPath, "../Packages/manifest.json");
            Assert.IsTrue(File.Exists(manifestPath), "Packages/manifest.json must exist.");

            string content = File.ReadAllText(manifestPath);
            Assert.IsTrue(content.Contains("com.unity.render-pipelines.universal"), "URP must be present in manifest.");
            Assert.IsTrue(content.Contains("com.unity.inputsystem"), "New Input System must be present in manifest.");
            Assert.IsTrue(content.Contains("com.unity.test-framework"), "Test framework must be present in manifest.");
        }

        [Test]
        public void PhysXSettings_MatchObservedTimestepAndSolvers()
        {
            string dynamicsPath = Path.Combine(Application.dataPath, "../ProjectSettings/DynamicsManager.asset");
            Assert.IsTrue(File.Exists(dynamicsPath), "DynamicsManager.asset must exist.");

            string content = File.ReadAllText(dynamicsPath);
            Assert.IsTrue(content.Contains("m_DefaultSolverIterations: 6"), "PhysX solver iterations must be 6.");
            Assert.IsTrue(content.Contains("m_DefaultSolverVelocityIterations: 2"), "PhysX velocity solver iterations must be 2.");
            Assert.IsTrue(content.Contains("m_SleepThreshold: 0.005"), "PhysX sleep threshold must be 0.005.");
        }
    }
}
