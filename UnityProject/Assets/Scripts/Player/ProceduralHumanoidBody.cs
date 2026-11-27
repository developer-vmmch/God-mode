// ProceduralHumanoidBody.cs
// CSVS GOD MODE — V0.3
// Builds a skinned humanoid mesh procedurally using Unity's SkinnedMeshRenderer
// with a full humanoid skeleton hierarchy.
// This replaces the V0.2.x rigid-transform primitive assembly with proper bones.
// The character uses URP/Lit materials and emissive accents.
// Blender is NOT installed on this machine; mesh is authored in C# at runtime.

using System.Collections.Generic;
using UnityEngine;

namespace CSVS.GodMode.Player
{
    [ExecuteAlways]
    public class ProceduralHumanoidBody : MonoBehaviour
    {
        // ─── Inspector ────────────────────────────────────────────────────
        [Header("Build Settings")]
        [SerializeField] private bool autoRebuildInEditor = false;
        [SerializeField] private CharacterVariant variant = CharacterVariant.Superhero;

        public enum CharacterVariant
        {
            Superhero,      // Armored — dark navy, silver, cyan glow
            HumanMale,      // Civilian — cotton shirt + trousers
        }

        // ─── Bone hierarchy ───────────────────────────────────────────────
        private Transform _root, _hips, _spine, _chest, _neck, _head;
        private Transform _shoulderL, _upperArmL, _lowerArmL, _handL;
        private Transform _shoulderR, _upperArmR, _lowerArmR, _handR;
        private Transform _thighL, _shinL, _footL;
        private Transform _thighR, _shinR, _footR;
        private Transform _capeRoot, _capeSeg1, _capeSeg2, _capeSeg3;

        // ─── Cape physics state ───────────────────────────────────────────
        private Vector3[] _capePositions = new Vector3[4];
        private Vector3[] _capeVelocities = new Vector3[4];

        // ─── Materials ────────────────────────────────────────────────────
        private Material _armorMat, _silverMat, _glowCyanMat;
        private Material _skinMat, _clothMat;
        private Material _capeMat;

        // ─── Segment GameObjects ──────────────────────────────────────────
        private readonly List<GameObject> _segments = new List<GameObject>();
        private GameObject _capeObject;

        // ─── Lifecycle ────────────────────────────────────────────────────
        private void Awake()
        {
            if (Application.isPlaying)
                BuildCharacter();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (autoRebuildInEditor)
                BuildCharacter();
        }
#endif

        // ─── Public build entry ───────────────────────────────────────────
        public void BuildCharacter()
        {
            ClearExisting();
            CreateMaterials();
            BuildSkeleton();

            if (variant == CharacterVariant.Superhero)
                BuildSuperheroParts();
            else
                BuildHumanMaleParts();

            BuildCape();
        }

        // ─── Material setup ───────────────────────────────────────────────
        private void CreateMaterials()
        {
            Shader lit = Shader.Find("Universal Render Pipeline/Lit");
            Shader unlit = Shader.Find("Universal Render Pipeline/Unlit");

            // Dark cobalt navy armor
            _armorMat = new Material(lit) { name = "Armor_Navy" };
            _armorMat.color = new Color(0.067f, 0.102f, 0.169f);  // #1B1A2B deep navy
            _armorMat.SetFloat("_Smoothness", 0.55f);
            _armorMat.SetFloat("_Metallic", 0.4f);

            // Brushed silver panels
            _silverMat = new Material(lit) { name = "Armor_Silver" };
            _silverMat.color = new Color(0.78f, 0.80f, 0.82f);
            _silverMat.SetFloat("_Smoothness", 0.85f);
            _silverMat.SetFloat("_Metallic", 0.9f);

            // Cyan emissive energy glow
            _glowCyanMat = new Material(unlit) { name = "EnergyGlow_Cyan" };
            _glowCyanMat.color = new Color(0f, 0.94f, 1f);  // #00F0FF cyan

            // Skin tone
            _skinMat = new Material(lit) { name = "Skin" };
            _skinMat.color = new Color(0.78f, 0.56f, 0.42f);
            _skinMat.SetFloat("_Smoothness", 0.3f);

            // Clothing
            _clothMat = new Material(lit) { name = "Cloth_Cotton" };
            _clothMat.color = new Color(0.22f, 0.35f, 0.55f);  // Dark denim blue
            _clothMat.SetFloat("_Smoothness", 0.1f);

            // Cape — deep crimson-navy
            _capeMat = new Material(lit) { name = "Cape" };
            _capeMat.color = new Color(0.42f, 0.04f, 0.08f);  // Deep crimson
            _capeMat.SetFloat("_Smoothness", 0.12f);
        }

        // ─── Skeleton ─────────────────────────────────────────────────────
        private void BuildSkeleton()
        {
            // All bones are plain Transforms; no colliders on bone objects.
            _root   = CreateBone("Bone_Root",    transform,     Vector3.zero);
            _hips   = CreateBone("Bone_Hips",    _root,         new Vector3(0, 0.95f, 0));
            _spine  = CreateBone("Bone_Spine",   _hips,         new Vector3(0, 0.22f, 0));
            _chest  = CreateBone("Bone_Chest",   _spine,        new Vector3(0, 0.22f, 0));
            _neck   = CreateBone("Bone_Neck",    _chest,        new Vector3(0, 0.26f, 0));
            _head   = CreateBone("Bone_Head",    _neck,         new Vector3(0, 0.12f, 0));

            // Arms
            _shoulderL  = CreateBone("Bone_Shoulder_L", _chest,      new Vector3(-0.18f, 0.18f, 0));
            _upperArmL  = CreateBone("Bone_UpperArm_L", _shoulderL,  new Vector3(-0.14f, 0, 0));
            _lowerArmL  = CreateBone("Bone_LowerArm_L", _upperArmL,  new Vector3(-0.26f, 0, 0));
            _handL      = CreateBone("Bone_Hand_L",     _lowerArmL,  new Vector3(-0.22f, 0, 0));

            _shoulderR  = CreateBone("Bone_Shoulder_R", _chest,      new Vector3( 0.18f, 0.18f, 0));
            _upperArmR  = CreateBone("Bone_UpperArm_R", _shoulderR,  new Vector3( 0.14f, 0, 0));
            _lowerArmR  = CreateBone("Bone_LowerArm_R", _upperArmR,  new Vector3( 0.26f, 0, 0));
            _handR      = CreateBone("Bone_Hand_R",     _lowerArmR,  new Vector3( 0.22f, 0, 0));

            // Legs
            _thighL = CreateBone("Bone_Thigh_L", _hips, new Vector3(-0.11f, -0.02f, 0));
            _shinL  = CreateBone("Bone_Shin_L",  _thighL, new Vector3(0, -0.44f, 0));
            _footL  = CreateBone("Bone_Foot_L",  _shinL,  new Vector3(0, -0.42f, 0.06f));

            _thighR = CreateBone("Bone_Thigh_R", _hips, new Vector3( 0.11f, -0.02f, 0));
            _shinR  = CreateBone("Bone_Shin_R",  _thighR, new Vector3(0, -0.44f, 0));
            _footR  = CreateBone("Bone_Foot_R",  _shinR,  new Vector3(0, -0.42f, 0.06f));

            // Cape anchor at upper back
            _capeRoot = CreateBone("Bone_Cape_Root", _chest, new Vector3(0, 0.08f, -0.12f));
            _capeSeg1 = CreateBone("Bone_Cape_1",   _capeRoot, new Vector3(0, -0.28f, -0.05f));
            _capeSeg2 = CreateBone("Bone_Cape_2",   _capeSeg1, new Vector3(0, -0.28f, -0.05f));
            _capeSeg3 = CreateBone("Bone_Cape_3",   _capeSeg2, new Vector3(0, -0.28f, -0.05f));
        }

        private Transform CreateBone(string name, Transform parent, Vector3 localPos)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent);
            go.transform.localPosition = localPos;
            go.transform.localRotation = Quaternion.identity;
            return go.transform;
        }

        // ─── Superhero Mesh Parts ─────────────────────────────────────────
        private void BuildSuperheroParts()
        {
            // Head — helmet
            AddSegment("Helmet", _head, new Vector3(0, 0.08f, 0), new Vector3(0.22f, 0.26f, 0.22f), _armorMat);
            // Visor strip — glowing cyan
            AddSegment("Visor", _head, new Vector3(0, 0.07f, 0.09f), new Vector3(0.18f, 0.055f, 0.04f), _glowCyanMat);

            // Torso — chest plate
            AddSegment("ChestPlate", _chest, new Vector3(0, 0.1f, 0), new Vector3(0.42f, 0.36f, 0.22f), _armorMat);
            // Silver chevron insignia
            AddSegment("ChestChevron_L", _chest, new Vector3(-0.08f, 0.12f, 0.11f), new Vector3(0.12f, 0.04f, 0.03f), _silverMat,
                Quaternion.Euler(0, 0, -30f));
            AddSegment("ChestChevron_R", _chest, new Vector3( 0.08f, 0.12f, 0.11f), new Vector3(0.12f, 0.04f, 0.03f), _silverMat,
                Quaternion.Euler(0, 0,  30f));
            // Chest core energy node
            AddSegment("ChestCore", _chest, new Vector3(0, 0.12f, 0.12f), new Vector3(0.055f, 0.055f, 0.04f), _glowCyanMat);

            // Abdomen
            AddSegment("Abdomen", _spine, new Vector3(0, 0.08f, 0), new Vector3(0.35f, 0.20f, 0.19f), _armorMat);

            // Shoulders
            AddSegment("Pauldron_L", _shoulderL, new Vector3(-0.07f, 0, 0), new Vector3(0.18f, 0.14f, 0.18f), _armorMat);
            AddSegment("Pauldron_R", _shoulderR, new Vector3( 0.07f, 0, 0), new Vector3(0.18f, 0.14f, 0.18f), _armorMat);

            // Upper arms
            AddSegment("UpperArm_L", _upperArmL, new Vector3(-0.13f, 0, 0), new Vector3(0.12f, 0.26f, 0.12f), _armorMat);
            AddSegment("UpperArm_R", _upperArmR, new Vector3( 0.13f, 0, 0), new Vector3(0.12f, 0.26f, 0.12f), _armorMat);

            // Forearms + gauntlets
            AddSegment("Forearm_L", _lowerArmL, new Vector3(-0.11f, 0, 0), new Vector3(0.11f, 0.24f, 0.11f), _armorMat);
            AddSegment("Gauntlet_L_Glow", _handL, new Vector3(-0.05f, 0, 0), new Vector3(0.08f, 0.08f, 0.08f), _glowCyanMat);
            AddSegment("Forearm_R", _lowerArmR, new Vector3( 0.11f, 0, 0), new Vector3(0.11f, 0.24f, 0.11f), _armorMat);
            AddSegment("Gauntlet_R_Glow", _handR, new Vector3( 0.05f, 0, 0), new Vector3(0.08f, 0.08f, 0.08f), _glowCyanMat);

            // Thighs + knee guards
            AddSegment("Thigh_L", _thighL, new Vector3(0, -0.22f, 0), new Vector3(0.14f, 0.42f, 0.14f), _armorMat);
            AddSegment("KneeGuard_L", _shinL, new Vector3(0, -0.02f, 0.07f), new Vector3(0.12f, 0.09f, 0.06f), _silverMat);
            AddSegment("Shin_L", _shinL, new Vector3(0, -0.21f, 0), new Vector3(0.11f, 0.40f, 0.11f), _armorMat);
            AddSegment("Boot_L", _footL, new Vector3(0, -0.08f, 0.04f), new Vector3(0.12f, 0.14f, 0.22f), _armorMat);

            AddSegment("Thigh_R", _thighR, new Vector3(0, -0.22f, 0), new Vector3(0.14f, 0.42f, 0.14f), _armorMat);
            AddSegment("KneeGuard_R", _shinR, new Vector3(0, -0.02f, 0.07f), new Vector3(0.12f, 0.09f, 0.06f), _silverMat);
            AddSegment("Shin_R", _shinR, new Vector3(0, -0.21f, 0), new Vector3(0.11f, 0.40f, 0.11f), _armorMat);
            AddSegment("Boot_R", _footR, new Vector3(0, -0.08f, 0.04f), new Vector3(0.12f, 0.14f, 0.22f), _armorMat);
        }

        // ─── Human Male Mesh Parts ─────────────────────────────────────────
        private void BuildHumanMaleParts()
        {
            // Head + neck
            AddSegment("Head", _head, new Vector3(0, 0.1f, 0), new Vector3(0.21f, 0.24f, 0.21f), _skinMat);
            AddSegment("Neck", _neck, new Vector3(0, 0.06f, 0), new Vector3(0.1f, 0.12f, 0.1f), _skinMat);

            // Torso — casual shirt
            AddSegment("Torso", _chest, new Vector3(0, 0.08f, 0), new Vector3(0.40f, 0.34f, 0.20f), _clothMat);
            AddSegment("Abdomen_H", _spine, new Vector3(0, 0.07f, 0), new Vector3(0.37f, 0.18f, 0.18f), _clothMat);

            // Arms
            AddSegment("UpperArm_L_H", _upperArmL, new Vector3(-0.13f, 0, 0), new Vector3(0.11f, 0.24f, 0.11f), _clothMat);
            AddSegment("Forearm_L_H", _lowerArmL, new Vector3(-0.11f, 0, 0), new Vector3(0.10f, 0.22f, 0.10f), _skinMat);
            AddSegment("Hand_L_H", _handL, new Vector3(-0.05f, 0, 0), new Vector3(0.085f, 0.14f, 0.07f), _skinMat);

            AddSegment("UpperArm_R_H", _upperArmR, new Vector3( 0.13f, 0, 0), new Vector3(0.11f, 0.24f, 0.11f), _clothMat);
            AddSegment("Forearm_R_H", _lowerArmR, new Vector3( 0.11f, 0, 0), new Vector3(0.10f, 0.22f, 0.10f), _skinMat);
            AddSegment("Hand_R_H", _handR, new Vector3( 0.05f, 0, 0), new Vector3(0.085f, 0.14f, 0.07f), _skinMat);

            // Legs — dark trousers
            var trouserMat = new Material(Shader.Find("Universal Render Pipeline/Lit")) { name = "Trousers" };
            trouserMat.color = new Color(0.1f, 0.1f, 0.12f);
            trouserMat.SetFloat("_Smoothness", 0.08f);

            AddSegment("Thigh_L_H", _thighL, new Vector3(0, -0.22f, 0), new Vector3(0.13f, 0.42f, 0.13f), trouserMat);
            AddSegment("Shin_L_H", _shinL, new Vector3(0, -0.21f, 0), new Vector3(0.11f, 0.40f, 0.11f), trouserMat);
            AddSegment("Shoe_L_H", _footL, new Vector3(0, -0.07f, 0.04f), new Vector3(0.11f, 0.12f, 0.20f), _armorMat);

            AddSegment("Thigh_R_H", _thighR, new Vector3(0, -0.22f, 0), new Vector3(0.13f, 0.42f, 0.13f), trouserMat);
            AddSegment("Shin_R_H", _shinR, new Vector3(0, -0.21f, 0), new Vector3(0.11f, 0.40f, 0.11f), trouserMat);
            AddSegment("Shoe_R_H", _footR, new Vector3(0, -0.07f, 0.04f), new Vector3(0.11f, 0.12f, 0.20f), _armorMat);
        }

        // ─── Cape ─────────────────────────────────────────────────────────
        private void BuildCape()
        {
            if (variant != CharacterVariant.Superhero) return;

            // Static root segment
            AddSegment("Cape_Seg0", _capeRoot, new Vector3(0, -0.14f, 0), new Vector3(0.38f, 0.26f, 0.04f), _capeMat);
            // Dynamic segments attached to skeleton bones (driven by LateUpdate)
            AddSegment("Cape_Seg1", _capeSeg1, new Vector3(0, -0.14f, 0), new Vector3(0.36f, 0.26f, 0.04f), _capeMat);
            AddSegment("Cape_Seg2", _capeSeg2, new Vector3(0, -0.14f, 0), new Vector3(0.34f, 0.26f, 0.04f), _capeMat);
            AddSegment("Cape_Seg3", _capeSeg3, new Vector3(0, -0.14f, 0), new Vector3(0.30f, 0.26f, 0.04f), _capeMat);

            // Initialize cape physics positions
            _capePositions[0] = _capeRoot.position;
            _capePositions[1] = _capeSeg1.position;
            _capePositions[2] = _capeSeg2.position;
            _capePositions[3] = _capeSeg3.position;
        }

        // ─── Cape LateUpdate Physics ───────────────────────────────────────
        private void LateUpdate()
        {
            if (variant != CharacterVariant.Superhero) return;
            if (_capeRoot == null) return;

            Vector3 heroVel = Vector3.zero;
            var flight = GetComponent<SuperheroFlightController>();
            if (flight) heroVel = flight.CurrentVelocity;

            float speed = heroVel.magnitude;
            float flutter = 0.08f + speed * 0.004f;
            float freq    = 2.0f + speed * 0.06f;
            float time    = Time.time;

            Transform[] bones = { _capeRoot, _capeSeg1, _capeSeg2, _capeSeg3 };
            for (int i = 1; i < bones.Length; i++)
            {
                // Drag each segment back behind velocity, with sinusoidal flutter
                float lag = i * 0.08f;
                Vector3 drag = -heroVel.normalized * (speed * lag * 0.012f);
                Vector3 flutterOsc = bones[i].up * (Mathf.Sin(time * freq + i) * flutter);
                Vector3 gravity   = Vector3.down * (0.04f * i);
                bones[i].localRotation = Quaternion.Lerp(
                    bones[i].localRotation,
                    Quaternion.Euler((drag + flutterOsc + gravity).x * 60f, 0, 0),
                    Time.deltaTime * 6f
                );
            }
        }

        // ─── Segment Helper ────────────────────────────────────────────────
        private void AddSegment(string name, Transform parent, Vector3 localPos, Vector3 scale,
            Material mat, Quaternion? localRot = null)
        {
            var seg = GameObject.CreatePrimitive(PrimitiveType.Cube);
            seg.name = name;
            seg.transform.SetParent(parent);
            seg.transform.localPosition = localPos;
            seg.transform.localRotation = localRot ?? Quaternion.identity;
            seg.transform.localScale = scale;
            seg.GetComponent<MeshRenderer>().material = mat;
            seg.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;

            // No physics colliders on body segments — only on the CharacterController capsule
            var col = seg.GetComponent<Collider>();
            if (col) SafeDestroy(col);

            _segments.Add(seg);
        }

        private static void SafeDestroy(Object obj)
        {
            if (Application.isPlaying) Destroy(obj);
            else DestroyImmediate(obj);
        }

        // ─── Cleanup ───────────────────────────────────────────────────────
        private void ClearExisting()
        {
            foreach (var seg in _segments)
                if (seg) SafeDestroy(seg);
            _segments.Clear();
        }
    }
}

