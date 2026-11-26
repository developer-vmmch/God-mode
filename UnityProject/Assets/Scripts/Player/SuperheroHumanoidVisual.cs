using System;
using UnityEngine;
using CSVS.GodMode.Core;

namespace CSVS.GodMode.Player
{
    /// <summary>
    /// Upgraded articulated humanoid superhero visual model and kinematic animator.
    /// Matched to authoritative concept design (media_1795684100658_9664b2fa.jpg):
    /// - Deep cobalt navy base armor suit with athletic male anatomy.
    /// - Geometric silver chevron breastplate and shoulder pauldrons.
    /// - Glowing cyan angular visor eye plates and wrist gauntlet power nodes.
    /// - Silver knee guards, greaves, and armored combat boots.
    /// - Dynamic procedural superhero cape fluttering with velocity and atmospheric drag.
    /// - Full kinematic procedural animation: walk, run, sprint, hover bobbing,
    ///   supersonic 6DOF dive (95 m/s), flight banking roll, punch strike, and slam.
    /// </summary>
    [ExecuteAlways]
    public class SuperheroHumanoidVisual : MonoBehaviour
    {
        [Header("PBR Armor Materials")]
        [SerializeField] private Material _suitNavyMaterial;
        [SerializeField] private Material _armorSilverMaterial;
        [SerializeField] private Material _cyanEnergyGlowMaterial;
        [SerializeField] private Material _visorDarkMaterial;
        [SerializeField] private Material _capeMaterial;

        private SuperheroFlightController _flightController;

        // Limb & Joint Hierarchy
        private Transform _modelRoot;
        private Transform _hipsTransform;
        private Transform _chestTransform;
        private Transform _headTransform;
        private Transform _leftArm;
        private Transform _rightArm;
        private Transform _leftForearm;
        private Transform _rightForearm;
        private Transform _leftLeg;
        private Transform _rightLeg;
        private Transform _leftShin;
        private Transform _rightShin;
        private Transform _chestCore;

        // Dynamic Cape Segments
        private Transform[] _capeSegments;
        private const int CapeSegmentCount = 4;

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
            {
                _suitNavyMaterial = CreateMat(litShader, new Color(0.08f, 0.16f, 0.32f), 0.60f); // Deep Cobalt Navy
                if (_suitNavyMaterial.HasProperty("_Metallic")) _suitNavyMaterial.SetFloat("_Metallic", 0.40f);
            }

            if (_armorSilverMaterial == null && litShader != null)
            {
                _armorSilverMaterial = CreateMat(litShader, new Color(0.85f, 0.88f, 0.92f), 0.85f); // Silver Metallic Armor
                if (_armorSilverMaterial.HasProperty("_Metallic")) _armorSilverMaterial.SetFloat("_Metallic", 0.90f);
            }

            if (_visorDarkMaterial == null && litShader != null)
            {
                _visorDarkMaterial = CreateMat(litShader, new Color(0.05f, 0.06f, 0.08f), 0.95f);
            }

            if (_cyanEnergyGlowMaterial == null && litShader != null)
            {
                _cyanEnergyGlowMaterial = CreateMat(litShader, new Color(0.0f, 0.95f, 1.0f), 0.95f);
                if (_cyanEnergyGlowMaterial.HasProperty("_EmissionColor"))
                {
                    _cyanEnergyGlowMaterial.EnableKeyword("_EMISSION");
                    _cyanEnergyGlowMaterial.SetColor("_EmissionColor", new Color(0.1f, 1.8f, 2.2f));
                }
            }

            if (_capeMaterial == null && litShader != null)
            {
                _capeMaterial = CreateMat(litShader, new Color(0.05f, 0.10f, 0.22f), 0.30f); // Midnight Blue Cloth Cape
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

        public void BuildHumanoidGeometry()
        {
            if (_modelRoot != null) return;
            InitializeMaterials();

            MeshRenderer rootRenderer = GetComponent<MeshRenderer>();
            if (rootRenderer != null) rootRenderer.enabled = false;

            _modelRoot = new GameObject("HumanoidHero_VisualRig").transform;
            _modelRoot.SetParent(transform, false);
            _modelRoot.localPosition = new Vector3(0f, -0.95f, 0f);

            // 1. Pelvis / Hips & Armored Belt
            GameObject hips = CreateBone("Hips", _modelRoot, new Vector3(0f, 0.95f, 0f), new Vector3(0.38f, 0.20f, 0.26f), _suitNavyMaterial);
            _hipsTransform = hips.transform;
            CreateBone("ArmorBelt", _hipsTransform, new Vector3(0f, 0.05f, 0f), new Vector3(0.40f, 0.08f, 0.28f), _armorSilverMaterial);

            // 2. Chest & Torso (Sculpted Athletic Pectorals)
            GameObject chest = CreateBone("Chest", _hipsTransform, new Vector3(0f, 0.32f, 0f), new Vector3(0.48f, 0.44f, 0.32f), _suitNavyMaterial);
            _chestTransform = chest.transform;

            // Chevron Breastplate & Pauldrons (Silver Geometric Crest from Concept Art)
            CreateBone("ChestChevronTop", _chestTransform, new Vector3(0f, 0.12f, 0.10f), new Vector3(0.44f, 0.14f, 0.18f), _armorSilverMaterial);
            CreateBone("ChestChevronV", _chestTransform, new Vector3(0f, -0.04f, 0.12f), new Vector3(0.24f, 0.18f, 0.14f), _armorSilverMaterial);

            // Glowing Cyan Energy Core in Center of Chest
            GameObject core = CreateBone("CyanEnergyCore", _chestTransform, new Vector3(0f, 0.08f, 0.18f), new Vector3(0.12f, 0.12f, 0.06f), _cyanEnergyGlowMaterial);
            _chestCore = core.transform;

            // 3. Head & Helmet with Cyan Visor Slits
            GameObject neck = CreateBone("Neck", _chestTransform, new Vector3(0f, 0.26f, 0f), new Vector3(0.15f, 0.10f, 0.15f), _armorSilverMaterial);
            GameObject head = CreateBone("Head", neck.transform, new Vector3(0f, 0.18f, 0f), new Vector3(0.24f, 0.28f, 0.26f), _suitNavyMaterial);
            _headTransform = head.transform;

            // Silver Cranial Crest & Dark Visor
            CreateBone("HelmetRidge", _headTransform, new Vector3(0f, 0.12f, -0.02f), new Vector3(0.08f, 0.08f, 0.28f), _armorSilverMaterial);
            CreateBone("HelmetVisor", _headTransform, new Vector3(0f, 0.02f, 0.12f), new Vector3(0.22f, 0.10f, 0.08f), _visorDarkMaterial);
            // Glowing Cyan Angular Eye Plates
            CreateBone("VisorEye_L", _headTransform, new Vector3(-0.06f, 0.03f, 0.15f), new Vector3(0.05f, 0.02f, 0.03f), _cyanEnergyGlowMaterial);
            CreateBone("VisorEye_R", _headTransform, new Vector3(0.06f, 0.03f, 0.15f), new Vector3(0.05f, 0.02f, 0.03f), _cyanEnergyGlowMaterial);

            // 4. Arms, Shoulders & Gauntlets
            // Left Arm
            GameObject lShoulder = CreateBone("Shoulder_L", _chestTransform, new Vector3(-0.31f, 0.16f, 0f), new Vector3(0.20f, 0.18f, 0.20f), _armorSilverMaterial);
            _leftArm = lShoulder.transform;
            GameObject lUpperArm = CreateBone("UpperArm_L", _leftArm, new Vector3(0f, -0.16f, 0f), new Vector3(0.16f, 0.28f, 0.16f), _suitNavyMaterial);
            GameObject lForearm = CreateBone("Forearm_L", lUpperArm.transform, new Vector3(0f, -0.22f, 0f), new Vector3(0.15f, 0.26f, 0.15f), _armorSilverMaterial);
            _leftForearm = lForearm.transform;
            // Cyan Power Emitter on Gauntlet
            CreateBone("GauntletNode_L", _leftForearm, new Vector3(-0.08f, 0f, 0f), new Vector3(0.04f, 0.08f, 0.04f), _cyanEnergyGlowMaterial);
            CreateBone("Fist_L", _leftForearm, new Vector3(0f, -0.16f, 0f), new Vector3(0.12f, 0.12f, 0.12f), _suitNavyMaterial);

            // Right Arm
            GameObject rShoulder = CreateBone("Shoulder_R", _chestTransform, new Vector3(0.31f, 0.16f, 0f), new Vector3(0.20f, 0.18f, 0.20f), _armorSilverMaterial);
            _rightArm = rShoulder.transform;
            GameObject rUpperArm = CreateBone("UpperArm_R", _rightArm, new Vector3(0f, -0.16f, 0f), new Vector3(0.16f, 0.28f, 0.16f), _suitNavyMaterial);
            GameObject rForearm = CreateBone("Forearm_R", rUpperArm.transform, new Vector3(0f, -0.22f, 0f), new Vector3(0.15f, 0.26f, 0.15f), _armorSilverMaterial);
            _rightForearm = rForearm.transform;
            // Cyan Power Emitter on Gauntlet
            CreateBone("GauntletNode_R", _rightForearm, new Vector3(0.08f, 0f, 0f), new Vector3(0.04f, 0.08f, 0.04f), _cyanEnergyGlowMaterial);
            CreateBone("Fist_R", _rightForearm, new Vector3(0f, -0.16f, 0f), new Vector3(0.12f, 0.12f, 0.12f), _suitNavyMaterial);

            // 5. Legs, Knee Guards & Armored Greaves
            // Left Leg
            GameObject lThigh = CreateBone("Thigh_L", _hipsTransform, new Vector3(-0.14f, -0.18f, 0f), new Vector3(0.18f, 0.38f, 0.18f), _suitNavyMaterial);
            _leftLeg = lThigh.transform;
            GameObject lShin = CreateBone("Shin_L", _leftLeg, new Vector3(0f, -0.32f, 0f), new Vector3(0.16f, 0.36f, 0.16f), _suitNavyMaterial);
            _leftShin = lShin.transform;
            // Silver Knee Guard & Shin Plating
            CreateBone("KneeGuard_L", _leftShin, new Vector3(0f, 0.12f, 0.08f), new Vector3(0.14f, 0.12f, 0.08f), _armorSilverMaterial);
            CreateBone("Greave_L", _leftShin, new Vector3(0f, -0.06f, 0.07f), new Vector3(0.13f, 0.20f, 0.06f), _armorSilverMaterial);
            CreateBone("Boot_L", _leftShin, new Vector3(0f, -0.22f, 0.05f), new Vector3(0.16f, 0.12f, 0.24f), _suitNavyMaterial);

            // Right Leg
            GameObject rThigh = CreateBone("Thigh_R", _hipsTransform, new Vector3(0.14f, -0.18f, 0f), new Vector3(0.18f, 0.38f, 0.18f), _suitNavyMaterial);
            _rightLeg = rThigh.transform;
            GameObject rShin = CreateBone("Shin_R", _rightLeg, new Vector3(0f, -0.32f, 0f), new Vector3(0.16f, 0.36f, 0.16f), _suitNavyMaterial);
            _rightShin = rShin.transform;
            // Silver Knee Guard & Shin Plating
            CreateBone("KneeGuard_R", _rightShin, new Vector3(0f, 0.12f, 0.08f), new Vector3(0.14f, 0.12f, 0.08f), _armorSilverMaterial);
            CreateBone("Greave_R", _rightShin, new Vector3(0f, -0.06f, 0.07f), new Vector3(0.13f, 0.20f, 0.06f), _armorSilverMaterial);
            CreateBone("Boot_R", _rightShin, new Vector3(0f, -0.22f, 0.05f), new Vector3(0.16f, 0.12f, 0.24f), _suitNavyMaterial);

            // 6. Dynamic Cloth Superhero Cape
            BuildDynamicCape();
        }

        private void BuildDynamicCape()
        {
            _capeSegments = new Transform[CapeSegmentCount];
            Transform parent = _chestTransform;

            for (int i = 0; i < CapeSegmentCount; i++)
            {
                float segmentLength = 0.28f;
                Vector3 pos = (i == 0) ? new Vector3(0f, 0.14f, -0.16f) : new Vector3(0f, -segmentLength, 0f);
                float widthScale = 0.45f + (i * 0.06f);

                GameObject seg = CreateBone($"CapeSegment_{i}", parent, pos, new Vector3(widthScale, segmentLength, 0.02f), _capeMaterial);
                _capeSegments[i] = seg.transform;
                parent = seg.transform;
            }
        }

        private GameObject CreateBone(string name, Transform parent, Vector3 localPos, Vector3 localScale, Material mat)
        {
            GameObject bone = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bone.name = name;
            bone.transform.SetParent(parent, false);
            bone.transform.localPosition = localPos;
            bone.transform.localScale = localScale;

            SafeDestroy(bone.GetComponent<Collider>());
            Renderer r = bone.GetComponent<Renderer>();
            if (r != null && mat != null) r.sharedMaterial = mat;

            return bone;
        }

        private void SafeDestroy(UnityEngine.Object obj)
        {
            if (obj == null) return;
            if (Application.isPlaying) Destroy(obj);
            else DestroyImmediate(obj);
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
            AnimateDynamicCape();
        }

        private void AnimateHumanoidPose()
        {
            float speed = _flightController.CurrentSpeed;
            LocomotionMode mode = _flightController.CurrentLocomotionMode;
            AbilityState ability = _flightController.CurrentAbilityState;

            _modelRoot.localRotation = Quaternion.identity;

            if (mode == LocomotionMode.Flight || mode == LocomotionMode.Hover)
            {
                AnimateFlightPose(speed, _flightController.IsBoosting);
            }
            else
            {
                AnimateGroundPose(speed);
            }

            ApplyAbilityPose(ability);
        }

        private void AnimateGroundPose(float speed)
        {
            if (speed > 0.5f)
            {
                // Walk / Run Cycle (Stride frequency proportional to speed)
                float strideFreq = Mathf.Min(speed * 0.85f, 12f);
                float legAngle = Mathf.Sin(_animTime * strideFreq) * 36.0f;
                float armAngle = -legAngle * 0.85f;

                _leftLeg.localRotation = Quaternion.Euler(legAngle, 0f, 0f);
                _rightLeg.localRotation = Quaternion.Euler(-legAngle, 0f, 0f);
                _leftArm.localRotation = Quaternion.Euler(armAngle, 0f, 0f);
                _rightArm.localRotation = Quaternion.Euler(-armAngle, 0f, 0f);

                // Knee flexion during recovery phase
                _leftShin.localRotation = (legAngle < 0f) ? Quaternion.Euler(-legAngle * 0.8f, 0f, 0f) : Quaternion.identity;
                _rightShin.localRotation = (legAngle > 0f) ? Quaternion.Euler(legAngle * 0.8f, 0f, 0f) : Quaternion.identity;
            }
            else
            {
                // Ground Idle (Subtle breathing chest rise and relaxed limbs)
                float breathe = Mathf.Sin(_animTime * 2.2f) * 2.5f;
                _chestTransform.localPosition = new Vector3(0f, 0.32f + breathe * 0.003f, 0f);
                _leftArm.localRotation = Quaternion.Euler(breathe, 0f, 6f);
                _rightArm.localRotation = Quaternion.Euler(breathe, 0f, -6f);
                _leftLeg.localRotation = Quaternion.identity;
                _rightLeg.localRotation = Quaternion.identity;
                _leftShin.localRotation = Quaternion.identity;
                _rightShin.localRotation = Quaternion.identity;
            }
        }

        private void AnimateFlightPose(float speed, bool isBoosting)
        {
            if (isBoosting || speed > 20f)
            {
                // Supersonic Dive & Flight (Up to 95 m/s): streamlined body diving forward 75°
                _modelRoot.localRotation = Quaternion.Euler(75.0f, 0f, 0f);

                // Both arms extended forward in aerodynamic diving punch pose
                _leftArm.localRotation = Quaternion.Euler(-65f, 0f, 8f);
                _rightArm.localRotation = Quaternion.Euler(-65f, 0f, -8f);
                _leftForearm.localRotation = Quaternion.Euler(-10f, 0f, 0f);
                _rightForearm.localRotation = Quaternion.Euler(-10f, 0f, 0f);

                // Legs trailing behind
                _leftLeg.localRotation = Quaternion.Euler(-15f, 0f, 4f);
                _rightLeg.localRotation = Quaternion.Euler(-15f, 0f, -4f);
                _leftShin.localRotation = Quaternion.Euler(10f, 0f, 0f);
                _rightShin.localRotation = Quaternion.Euler(10f, 0f, 0f);
            }
            else
            {
                // Hover Mode: floating with vertical sine bobbing
                float floatBob = Mathf.Sin(_animTime * 2.8f) * 6.0f;
                _modelRoot.localRotation = Quaternion.Euler(14.0f + floatBob * 0.5f, 0f, 0f);

                _leftArm.localRotation = Quaternion.Euler(12f, 0f, 18f);
                _rightArm.localRotation = Quaternion.Euler(12f, 0f, -18f);

                _leftLeg.localRotation = Quaternion.Euler(16f + floatBob, 0f, 6f);
                _rightLeg.localRotation = Quaternion.Euler(20f - floatBob, 0f, -6f);
                _leftShin.localRotation = Quaternion.Euler(18f, 0f, 0f);
                _rightShin.localRotation = Quaternion.Euler(22f, 0f, 0f);
            }
        }

        private void AnimateDynamicCape()
        {
            if (_capeSegments == null) return;

            float speed = _flightController != null ? _flightController.CurrentSpeed : 0f;
            bool isFlying = _flightController != null && (_flightController.CurrentLocomotionMode == LocomotionMode.Flight || _flightController.CurrentLocomotionMode == LocomotionMode.Hover);

            // Angle of cape backward trail depends on forward speed
            float baseTrailAngle = Mathf.Clamp(speed * 1.5f, 10f, 85f);
            float flutterFreq = 8f + speed * 0.5f;

            for (int i = 0; i < _capeSegments.Length; i++)
            {
                if (_capeSegments[i] == null) continue;

                float flutter = Mathf.Sin(_animTime * flutterFreq + i * 0.8f) * (6f + i * 4f);
                float segAngle = (isFlying) ? (baseTrailAngle * 0.35f + flutter) : (10f + flutter * 0.3f);

                _capeSegments[i].localRotation = Quaternion.Euler(-segAngle, 0f, flutter * 0.2f);
            }
        }

        private void ApplyAbilityPose(AbilityState ability)
        {
            switch (ability)
            {
                case AbilityState.SuperPunch:
                    _rightArm.localRotation = Quaternion.Euler(-95f, 18f, 0f);
                    _rightForearm.localRotation = Quaternion.Euler(-20f, 0f, 0f);
                    break;

                case AbilityState.HoldingObject:
                case AbilityState.Grabbing:
                    _leftArm.localRotation = Quaternion.Euler(-145f, -18f, 0f);
                    _rightArm.localRotation = Quaternion.Euler(-145f, 18f, 0f);
                    break;

                case AbilityState.GroundSlam:
                    _modelRoot.localRotation = Quaternion.Euler(90f, 0f, 0f);
                    _rightArm.localRotation = Quaternion.Euler(-115f, 0f, 0f);
                    break;
            }
        }
    }
}
