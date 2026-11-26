using System;
using System.Collections.Generic;
using UnityEngine;

namespace CSVS.GodMode.AI
{
    public enum IndianAttireType
    {
        VeshtiAndShirt = 0, // Traditional South Indian white cotton veshti/lungi & collared shirt
        CasualPantsAndShirt = 1, // Trousers & casual shirt / polo
        Saree = 2, // Colorful saree with pallu draped over shoulder
        Uniform = 3 // Khaki uniform (auto driver / police constable)
    }

    /// <summary>
    /// Manages South Indian street life and civilian pedestrians across Karaikal.
    /// Spawns animated civilians walking along sidewalks with Left-Hand Traffic awareness,
    /// varied regional clothing (veshti, sarees, shirts, uniforms), and distance-based pooling.
    /// </summary>
    public class KaraikalPedestrianSystem : MonoBehaviour
    {
        [Header("Pedestrian Spawning Specs")]
        [SerializeField] private int _maxPedestrians = 35;
        [SerializeField] private float _spawnRadius = 75f;
        [SerializeField] private float _despawnRadius = 95f;

        private static readonly Color[] SareeColors = new Color[]
        {
            new Color(0.85f, 0.15f, 0.25f), // Crimson Red
            new Color(0.92f, 0.65f, 0.12f), // Mustard Gold
            new Color(0.15f, 0.65f, 0.55f), // Peacock Teal
            new Color(0.70f, 0.20f, 0.65f), // Magenta Violet
            new Color(0.18f, 0.50f, 0.85f)  // Royal Blue
        };

        private static readonly Color[] ShirtColors = new Color[]
        {
            new Color(0.92f, 0.92f, 0.94f), // White cotton
            new Color(0.25f, 0.45f, 0.75f), // Blue
            new Color(0.75f, 0.60f, 0.40f), // Khaki / Sand
            new Color(0.65f, 0.20f, 0.20f), // Maroon
            new Color(0.35f, 0.65f, 0.40f)  // Olive green
        };

        private readonly List<PedestrianInstance> _activePedestrians = new List<PedestrianInstance>();
        private Transform _playerTransform;
        private float _updateTimer = 0f;

        public class PedestrianInstance
        {
            public GameObject Root;
            public Transform LeftLeg;
            public Transform RightLeg;
            public Transform LeftArm;
            public Transform RightArm;
            public Vector3 WalkDirection;
            public float WalkSpeed;
            public float AnimPhase;
        }

        private void Start()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) _playerTransform = player.transform;
            PopulatePedestrians();
        }

        private void Update()
        {
            if (_playerTransform == null)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) _playerTransform = player.transform;
                else return;
            }

            float dt = Time.deltaTime;
            _updateTimer += dt;

            // Animate and Move active pedestrians
            for (int i = _activePedestrians.Count - 1; i >= 0; i--)
            {
                var ped = _activePedestrians[i];
                if (ped.Root == null)
                {
                    _activePedestrians.RemoveAt(i);
                    continue;
                }

                // Check Despawn distance from player
                float dist = Vector3.Distance(ped.Root.transform.position, _playerTransform.position);
                if (dist > _despawnRadius)
                {
                    // Reposition ahead of player
                    RepositionPedestrian(ped);
                    continue;
                }

                // Forward Step
                ped.Root.transform.position += ped.WalkDirection * (ped.WalkSpeed * dt);

                // Walking limb animation
                ped.AnimPhase += dt * ped.WalkSpeed * 3.5f;
                float legAngle = Mathf.Sin(ped.AnimPhase) * 26.0f;
                float armAngle = -legAngle * 0.8f;

                if (ped.LeftLeg != null) ped.LeftLeg.localRotation = Quaternion.Euler(legAngle, 0f, 0f);
                if (ped.RightLeg != null) ped.RightLeg.localRotation = Quaternion.Euler(-legAngle, 0f, 0f);
                if (ped.LeftArm != null) ped.LeftArm.localRotation = Quaternion.Euler(armAngle, 0f, 0f);
                if (ped.RightArm != null) ped.RightArm.localRotation = Quaternion.Euler(-armAngle, 0f, 0f);
            }
        }

        public void PopulatePedestrians()
        {
            if (_playerTransform == null) return;
            Vector3 center = _playerTransform.position;

            for (int i = 0; i < _maxPedestrians; i++)
            {
                float angle = (float)i / _maxPedestrians * Mathf.PI * 2f;
                float dist = UnityEngine.Random.Range(15f, _spawnRadius);
                Vector3 pos = center + new Vector3(Mathf.Cos(angle) * dist, 0f, Mathf.Sin(angle) * dist);

                // Snap to sidewalk (offset from road axis)
                pos.x = (pos.x >= 0f) ? Mathf.Max(pos.x, 9.5f) : Mathf.Min(pos.x, -9.5f);

                SpawnPedestrianAt(pos);
            }
        }

        private void SpawnPedestrianAt(Vector3 pos)
        {
            IndianAttireType attire = (IndianAttireType)UnityEngine.Random.Range(0, 4);
            GameObject root = new GameObject($"Pedestrian_{attire}");
            root.transform.SetParent(transform, false);
            root.transform.position = pos;

            Vector3 walkDir = (UnityEngine.Random.value > 0.5f) ? Vector3.forward : Vector3.back;
            root.transform.rotation = Quaternion.LookRotation(walkDir, Vector3.up);

            PedestrianInstance ped = new PedestrianInstance
            {
                Root = root,
                WalkDirection = walkDir,
                WalkSpeed = UnityEngine.Random.Range(1.2f, 1.8f),
                AnimPhase = UnityEngine.Random.Range(0f, 10f)
            };

            BuildPedestrianVisual(root.transform, attire, ped);
            _activePedestrians.Add(ped);
        }

        private void RepositionPedestrian(PedestrianInstance ped)
        {
            if (_playerTransform == null) return;
            float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
            float dist = UnityEngine.Random.Range(20f, _spawnRadius * 0.85f);
            Vector3 newPos = _playerTransform.position + new Vector3(Mathf.Cos(angle) * dist, 0f, Mathf.Sin(angle) * dist);
            newPos.x = (newPos.x >= 0f) ? Mathf.Max(newPos.x, 9.5f) : Mathf.Min(newPos.x, -9.5f);

            ped.Root.transform.position = newPos;
            ped.WalkDirection = (UnityEngine.Random.value > 0.5f) ? Vector3.forward : Vector3.back;
            ped.Root.transform.rotation = Quaternion.LookRotation(ped.WalkDirection, Vector3.up);
        }

        private void BuildPedestrianVisual(Transform parent, IndianAttireType attire, PedestrianInstance ped)
        {
            Shader litShader = Shader.Find("Universal Render Pipeline/Lit");
            if (litShader == null) litShader = Shader.Find("Universal Render Pipeline/Simple Lit");
            if (litShader == null) litShader = Shader.Find("Standard");

            Color shirtColor = ShirtColors[UnityEngine.Random.Range(0, ShirtColors.Length)];
            Color bottomColor = (attire == IndianAttireType.VeshtiAndShirt) ? new Color(0.95f, 0.95f, 0.93f) : new Color(0.18f, 0.22f, 0.28f);
            if (attire == IndianAttireType.Saree)
            {
                Color sc = SareeColors[UnityEngine.Random.Range(0, SareeColors.Length)];
                shirtColor = sc;
                bottomColor = sc;
            }
            else if (attire == IndianAttireType.Uniform)
            {
                shirtColor = new Color(0.68f, 0.52f, 0.32f); // Khaki uniform
                bottomColor = new Color(0.68f, 0.52f, 0.32f);
            }

            Material shirtMat = new Material(litShader);
            if (shirtMat.HasProperty("_BaseColor")) shirtMat.SetColor("_BaseColor", shirtColor);

            Material bottomMat = new Material(litShader);
            if (bottomMat.HasProperty("_BaseColor")) bottomMat.SetColor("_BaseColor", bottomColor);

            Material skinMat = new Material(litShader);
            if (skinMat.HasProperty("_BaseColor")) skinMat.SetColor("_BaseColor", new Color(0.48f, 0.32f, 0.22f)); // South Indian warm brown skin tone

            Material hairMat = new Material(litShader);
            if (hairMat.HasProperty("_BaseColor")) hairMat.SetColor("_BaseColor", new Color(0.08f, 0.08f, 0.09f));

            // Torso / Shirt
            CreateChild(PrimitiveType.Cube, parent, new Vector3(0f, 1.15f, 0f), new Vector3(0.40f, 0.50f, 0.24f), shirtMat);

            // Head & Hair
            CreateChild(PrimitiveType.Cube, parent, new Vector3(0f, 1.55f, 0f), new Vector3(0.20f, 0.24f, 0.20f), skinMat);
            CreateChild(PrimitiveType.Cube, parent, new Vector3(0f, 1.66f, -0.02f), new Vector3(0.22f, 0.08f, 0.22f), hairMat);

            // Arms
            GameObject armL = CreateChild(PrimitiveType.Cube, parent, new Vector3(-0.25f, 1.10f, 0f), new Vector3(0.12f, 0.44f, 0.12f), shirtMat);
            ped.LeftArm = armL.transform;
            GameObject armR = CreateChild(PrimitiveType.Cube, parent, new Vector3(0.25f, 1.10f, 0f), new Vector3(0.12f, 0.44f, 0.12f), shirtMat);
            ped.RightArm = armR.transform;

            // Legs
            GameObject legL = CreateChild(PrimitiveType.Cube, parent, new Vector3(-0.11f, 0.45f, 0f), new Vector3(0.16f, 0.85f, 0.16f), bottomMat);
            ped.LeftLeg = legL.transform;
            GameObject legR = CreateChild(PrimitiveType.Cube, parent, new Vector3(0.11f, 0.45f, 0f), new Vector3(0.16f, 0.85f, 0.16f), bottomMat);
            ped.RightLeg = legR.transform;

            // Saree Draped Pallu or Veshti Border
            if (attire == IndianAttireType.Saree)
            {
                CreateChild(PrimitiveType.Cube, parent, new Vector3(-0.05f, 1.05f, 0.14f), new Vector3(0.22f, 0.55f, 0.04f), bottomMat, new Vector3(0f, 0f, 25f));
            }
        }

        private static GameObject CreateChild(PrimitiveType type, Transform parent, Vector3 localPos, Vector3 scale, Material mat, Vector3? rot = null)
        {
            GameObject go = GameObject.CreatePrimitive(type);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.transform.localScale = scale;
            if (rot.HasValue) go.transform.localRotation = Quaternion.Euler(rot.Value);

            SafeDestroy(go.GetComponent<Collider>());
            Renderer r = go.GetComponent<Renderer>();
            if (r != null && mat != null) r.sharedMaterial = mat;

            return go;
        }

        private static void SafeDestroy(UnityEngine.Object obj)
        {
            if (obj == null) return;
            if (Application.isPlaying) Destroy(obj);
            else DestroyImmediate(obj);
        }

        public int ActivePedestrianCount => _activePedestrians.Count;
    }
}
