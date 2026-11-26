using System;
using UnityEngine;
using CSVS.GodMode.Core;

namespace CSVS.GodMode.Player
{
    /// <summary>
    /// Original procedural humanoid superhero visual model and kinematic animator.
    /// Replaces the placeholder capsule with an athletic articulated armored humanoid superhero:
    /// - Metallic cobalt blue and dark navy armored plating.
    /// - Polished silver shoulder pauldrons and gauntlets.
    /// - Glowing cyan chest energy core and visor eye emitters.
    /// - Procedural kinematic animation driving idle, run, hover, 6DOF supersonic flight,
    ///   ground slam, super punch, and overhead lifting poses.
    /// </summary>
    public class SuperheroHumanoidVisual : MonoBehaviour
    {
        [Header("Materials")]
        [SerializeField] private Material _suitNavyMaterial;
        [SerializeField] private Material _armorSilverMaterial;
        [SerializeField] private Material _cyanEnergyGlowMaterial;
        [SerializeField] private Material _visorDarkMaterial;

        private SuperheroFlightController _flightController;

        // Limb Transforms for Procedural Animation
        private Transform _modelRoot;
        private Transform _headTransform;
        private Transform _chestTransform;
        private Transform _leftArm;
        private Transform _rightArm;
        private Transform _leftForearm;
        private Transform _rightForearm;
        private Transform _leftLeg;
        private Transform _rightLeg;
        private Transform _leftShin;
        private Transform _rightShin;
        private Transform _chestCore;

        private float _animTime = 0.0f;

        private void Awake()
        {
            _flightController = GetComponentInParent<SuperheroFlightController>();
            InitializeMaterials();
            BuildHumanoidGeometry();
        }

        private void InitializeMaterials()
        {
            Shader litShader = Shader.Find("Universal Render Pipeline/Lit");
            if (litShader == null) litShader = Shader.Find("Universal Render Pipeline/Simple Lit");
            if (litShader == null) litShader = Shader.Find("Standard");

            if (_suitNavyMaterial == null && litShader != null)
                _suitNavyMaterial = CreateMat(litShader, new Color(0.10f, 0.22f, 0.55f), 0.65f); // Deep Cobalt Navy

            if (_armorSilverMaterial == null && litShader != null)
                _armorSilverMaterial = CreateMat(litShader, new Color(0.78f, 0.82f, 0.86f), 0.85f); // Silver Metallic Armor

            if (_visorDarkMaterial == null && litShader != null)
                _visorDarkMaterial = CreateMat(litShader, new Color(0.08f, 0.09f, 0.12f), 0.90f);

            if (_cyanEnergyGlowMaterial == null && litShader != null)
            {
                _cyanEnergyGlowMaterial = CreateMat(litShader, new Color(0.15f, 0.92f, 1.0f), 0.95f);
                if (_cyanEnergyGlowMaterial.HasProperty("_EmissionColor"))
                {
                    _cyanEnergyGlowMaterial.EnableKeyword("_EMISSION");
                    _cyanEnergyGlowMaterial.SetColor("_EmissionColor", new Color(0.2f, 1.2f, 1.5f));
                }
            }
        }

        private Material CreateMat(Shader shader, Color col, float smoothness)
        {
            Material m = new Material(shader);
            if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", col);
            else if (m.HasProperty("_Color")) m.SetColor("_Color", col);
            if (m.HasProperty("_Smoothness")) m.SetFloat("_Smoothness", smoothness);
            return m;
        }

        private void BuildHumanoidGeometry()
        {
            // Disable any existing primitive capsule mesh renderer on the root
            MeshRenderer rootRenderer = GetComponent<MeshRenderer>();
            if (rootRenderer != null) rootRenderer.enabled = false;

            _modelRoot = new GameObject("HumanoidHero_VisualRig").transform;
            _modelRoot.SetParent(transform, false);
            _modelRoot.localPosition = new Vector3(0f, -0.95f, 0f);

            // 1. Pelvis / Hips
            GameObject hips = CreateBone("Hips", _modelRoot, new Vector3(0f, 0.95f, 0f), new Vector3(0.38f, 0.20f, 0.26f), _suitNavyMaterial);

            // 2. Chest & Torso
            GameObject chest = CreateBone("Chest", hips.transform, new Vector3(0f, 0.32f, 0f), new Vector3(0.48f, 0.44f, 0.32f), _suitNavyMaterial);
            _chestTransform = chest.transform;

            // Chest Armor Plate (Silver Pauldron & Breastplate)
            CreateBone("Breastplate", _chestTransform, new Vector3(0f, 0.04f, 0.08f), new Vector3(0.42f, 0.32f, 0.22f), _armorSilverMaterial);

            // Glowing Cyan Energy Core in Center of Chest
            GameObject core = CreateBone("CyanEnergyCore", _chestTransform, new Vector3(0f, 0.06f, 0.18f), new Vector3(0.14f, 0.14f, 0.06f), _cyanEnergyGlowMaterial);
            _chestCore = core.transform;

            // 3. Head & Helmet
            GameObject neck = CreateBone("Neck", _chestTransform, new Vector3(0f, 0.26f, 0f), new Vector3(0.16f, 0.12f, 0.16f), _armorSilverMaterial);
            GameObject head = CreateBone("Head", neck.transform, new Vector3(0f, 0.18f, 0f), new Vector3(0.24f, 0.28f, 0.26f), _suitNavyMaterial);
            _headTransform = head.transform;

            // Helmet Visor (Dark + Cyan Eye Slits)
            CreateBone("HelmetVisor", _headTransform, new Vector3(0f, 0.02f, 0.12f), new Vector3(0.20f, 0.10f, 0.08f), _visorDarkMaterial);
            CreateBone("EyeLeft", _headTransform, new Vector3(-0.06f, 0.03f, 0.15f), new Vector3(0.04f, 0.02f, 0.03f), _cyanEnergyGlowMaterial);
            CreateBone("EyeRight", _headTransform, new Vector3(0.06f, 0.03f, 0.15f), new Vector3(0.04f, 0.02f, 0.03f), _cyanEnergyGlowMaterial);

            // 4. Arms & Gauntlets
            // Left Arm
            GameObject lShoulder = CreateBone("Shoulder_L", _chestTransform, new Vector3(-0.30f, 0.16f, 0f), new Vector3(0.18f, 0.16f, 0.18f), _armorSilverMaterial);
            _leftArm = lShoulder.transform;
            GameObject lUpperArm = CreateBone("UpperArm_L", _leftArm, new Vector3(0f, -0.16f, 0f), new Vector3(0.15f, 0.28f, 0.15f), _suitNavyMaterial);
            GameObject lForearm = CreateBone("Forearm_L", lUpperArm.transform, new Vector3(0f, -0.22f, 0f), new Vector3(0.14f, 0.26f, 0.14f), _armorSilverMaterial);
            _leftForearm = lForearm.transform;
            CreateBone("Fist_L", _leftForearm, new Vector3(0f, -0.16f, 0f), new Vector3(0.12f, 0.12f, 0.12f), _suitNavyMaterial);

            // Right Arm
            GameObject rShoulder = CreateBone("Shoulder_R", _chestTransform, new Vector3(0.30f, 0.16f, 0f), new Vector3(0.18f, 0.16f, 0.18f), _armorSilverMaterial);
            _rightArm = rShoulder.transform;
            GameObject rUpperArm = CreateBone("UpperArm_R", _rightArm, new Vector3(0f, -0.16f, 0f), new Vector3(0.15f, 0.28f, 0.15f), _suitNavyMaterial);
            GameObject rForearm = CreateBone("Forearm_R", rUpperArm.transform, new Vector3(0f, -0.22f, 0f), new Vector3(0.14f, 0.26f, 0.14f), _armorSilverMaterial);
            _rightForearm = rForearm.transform;
            CreateBone("Fist_R", _rightForearm, new Vector3(0f, -0.16f, 0f), new Vector3(0.12f, 0.12f, 0.12f), _suitNavyMaterial);

            // 5. Legs & Boots
            // Left Leg
            GameObject lThigh = CreateBone("Thigh_L", hips.transform, new Vector3(-0.14f, -0.18f, 0f), new Vector3(0.17f, 0.38f, 0.18f), _suitNavyMaterial);
            _leftLeg = lThigh.transform;
            GameObject lShin = CreateBone("Shin_L", _leftLeg, new Vector3(0f, -0.32f, 0f), new Vector3(0.15f, 0.36f, 0.16f), _armorSilverMaterial);
            _leftShin = lShin.transform;
            CreateBone("Boot_L", _leftShin, new Vector3(0f, -0.22f, 0.05f), new Vector3(0.16f, 0.12f, 0.24f), _suitNavyMaterial);

            // Right Leg
            GameObject rThigh = CreateBone("Thigh_R", hips.transform, new Vector3(0.14f, -0.18f, 0f), new Vector3(0.17f, 0.38f, 0.18f), _suitNavyMaterial);
            _rightLeg = rThigh.transform;
            GameObject rShin = CreateBone("Shin_R", _rightLeg, new Vector3(0f, -0.32f, 0f), new Vector3(0.15f, 0.36f, 0.16f), _armorSilverMaterial);
            _rightShin = rShin.transform;
            CreateBone("Boot_R", _rightShin, new Vector3(0f, -0.22f, 0.05f), new Vector3(0.16f, 0.12f, 0.24f), _suitNavyMaterial);
        }

        private GameObject CreateBone(string name, Transform parent, Vector3 localPos, Vector3 localScale, Material mat)
        {
            GameObject bone = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bone.name = name;
            bone.transform.SetParent(parent, false);
            bone.transform.localPosition = localPos;
            bone.transform.localScale = localScale;

            Destroy(bone.GetComponent<Collider>());

            Renderer r = bone.GetComponent<Renderer>();
            if (r != null && mat != null) r.sharedMaterial = mat;

            return bone;
        }

        private void LateUpdate()
        {
            if (_flightController == null)
            {
                _flightController = GetComponentInParent<SuperheroFlightController>();
                if (_flightController == null) return;
            }

            _animTime += Time.deltaTime;
            AnimateHumanoidPose();
        }

        private void AnimateHumanoidPose()
        {
            float speed = _flightController.CurrentSpeed;
            LocomotionMode mode = _flightController.CurrentLocomotionMode;
            AbilityState ability = _flightController.CurrentAbilityState;

            // Reset Root Rotation
            _modelRoot.localRotation = Quaternion.identity;

            if (mode == LocomotionMode.Flight || mode == LocomotionMode.Hover)
            {
                AnimateFlightPose(speed, _flightController.IsBoosting);
            }
            else
            {
                AnimateGroundPose(speed);
            }

            // Apply Ability Overlays
            ApplyAbilityPose(ability);
        }

        private void AnimateGroundPose(float speed)
        {
            if (speed > 0.5f)
            {
                // Walk / Run Cycle (Stride frequency proportional to speed)
                float strideFreq = Mathf.Min(speed * 0.8f, 10f);
                float legAngle = Mathf.Sin(_animTime * strideFreq) * 35.0f;
                float armAngle = -legAngle * 0.8f;

                _leftLeg.localRotation = Quaternion.Euler(legAngle, 0f, 0f);
                _rightLeg.localRotation = Quaternion.Euler(-legAngle, 0f, 0f);
                _leftArm.localRotation = Quaternion.Euler(armAngle, 0f, 0f);
                _rightArm.localRotation = Quaternion.Euler(-armAngle, 0f, 0f);
            }
            else
            {
                // Ground Idle (Breathing chest motion)
                float breathe = Mathf.Sin(_animTime * 2.0f) * 2.0f;
                _leftArm.localRotation = Quaternion.Euler(breathe, 0f, 5f);
                _rightArm.localRotation = Quaternion.Euler(breathe, 0f, -5f);
                _leftLeg.localRotation = Quaternion.identity;
                _rightLeg.localRotation = Quaternion.identity;
            }
        }

        private void AnimateFlightPose(float speed, bool isBoosting)
        {
            if (isBoosting || speed > 25f)
            {
                // High-Speed Supersonic Flight: Forward streamlined dive pose (75° pitch forward)
                _modelRoot.localRotation = Quaternion.Euler(75.0f, 0f, 0f);

                // Both arms thrust forward in diving punch pose
                _leftArm.localRotation = Quaternion.Euler(-60f, 0f, 10f);
                _rightArm.localRotation = Quaternion.Euler(-60f, 0f, -10f);

                // Legs streamlined trailing behind
                _leftLeg.localRotation = Quaternion.Euler(-15f, 0f, 4f);
                _rightLeg.localRotation = Quaternion.Euler(-15f, 0f, -4f);
            }
            else
            {
                // Hover Mode: Floating gently with subtle vertical leg sway
                float floatBob = Mathf.Sin(_animTime * 3.0f) * 6.0f;
                _modelRoot.localRotation = Quaternion.Euler(15.0f + floatBob * 0.5f, 0f, 0f);

                _leftArm.localRotation = Quaternion.Euler(10f, 0f, 18f);
                _rightArm.localRotation = Quaternion.Euler(10f, 0f, -18f);

                _leftLeg.localRotation = Quaternion.Euler(15f + floatBob, 0f, 6f);
                _rightLeg.localRotation = Quaternion.Euler(20f - floatBob, 0f, -6f);
            }
        }

        private void ApplyAbilityPose(AbilityState ability)
        {
            switch (ability)
            {
                case AbilityState.SuperPunch:
                    // Right arm extended forward in powerful superhero strike
                    _rightArm.localRotation = Quaternion.Euler(-95f, 20f, 0f);
                    _rightForearm.localRotation = Quaternion.Euler(-15f, 0f, 0f);
                    break;

                case AbilityState.HoldingObject:
                case AbilityState.Grabbing:
                    // Both arms raised overhead in a high-strength lifting pose
                    _leftArm.localRotation = Quaternion.Euler(-140f, -20f, 0f);
                    _rightArm.localRotation = Quaternion.Euler(-140f, 20f, 0f);
                    break;

                case AbilityState.GroundSlam:
                    // Diving downward strike pose
                    _modelRoot.localRotation = Quaternion.Euler(90f, 0f, 0f);
                    _rightArm.localRotation = Quaternion.Euler(-110f, 0f, 0f);
                    break;
            }
        }
    }
}
