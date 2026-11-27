// KaraikalRoadMeshBuilder.cs
// CSVS GOD MODE — V0.3
// Builds textured road meshes from OSM-derived polyline data.
// Roads are generated with asphalt URP material, painted lane markings,
// curbs, sidewalks and proper human-scale proportions.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace CSVS.GodMode.Environment
{
    [DefaultExecutionOrder(-10)]
    public class KaraikalRoadMeshBuilder : MonoBehaviour
    {
        [Header("Road Mesh Settings")]
        [SerializeField] private bool autoGenerateOnAwake = true;
        [SerializeField] private float roadHeightOffset = 0.02f;   // Slight raise above terrain
        [SerializeField] private float curbHeight = 0.12f;
        [SerializeField] private float sidewalkWidth = 1.5f;
        [SerializeField] private float dividerWidth = 0.5f;

        [Header("Materials (assigned from URP)")]
        [SerializeField] private Material asphaltMaterial;
        [SerializeField] private Material laneMaterial;
        [SerializeField] private Material sidewalkMaterial;
        [SerializeField] private Material curbMaterial;
        [SerializeField] private Material bridgeMaterial;

        private List<GameObject> _roadObjects = new List<GameObject>();
        private static KaraikalRoadMeshBuilder _instance;
        public static KaraikalRoadMeshBuilder Instance => _instance;

        private void Awake()
        {
            _instance = this;
            if (autoGenerateOnAwake)
                StartCoroutine(GenerateAllRoadsCoroutine());
        }

        // ─── Public API ───────────────────────────────────────────────────
        public IEnumerator GenerateAllRoadsCoroutine()
        {
            var container = new GameObject("KaraikalRoadNetwork");
            container.transform.SetParent(transform);

            foreach (var seg in KaraikalOSMRoadData.Segments)
            {
                BuildRoadSegment(seg, container.transform);
                yield return null;   // Spread across frames
            }

            foreach (var bridge in KaraikalOSMRoadData.Bridges)
            {
                BuildBridge(bridge, container.transform);
                yield return null;
            }

            PlaceLandmarkMarkers(container.transform);
            Debug.Log($"[RoadMeshBuilder] Generated {KaraikalOSMRoadData.Segments.Length} road segments and {KaraikalOSMRoadData.Bridges.Length} bridges.");
        }

        // ─── Road Segment Mesh ─────────────────────────────────────────────
        private void BuildRoadSegment(KaraikalOSMRoadData.RoadSegment seg, Transform parent)
        {
            if (seg.Polyline == null || seg.Polyline.Length < 2) return;

            var roadObj = new GameObject(seg.Name);
            roadObj.transform.SetParent(parent);
            _roadObjects.Add(roadObj);

            // Build main road surface
            var roadMesh = BuildExtrudedRoadMesh(seg.Polyline, seg.WidthMetres, roadHeightOffset);
            var roadMF = roadObj.AddComponent<MeshFilter>();
            var roadMR = roadObj.AddComponent<MeshRenderer>();
            roadMF.mesh = roadMesh;
            roadMR.material = GetOrCreateAsphaltMaterial(seg.Type);
            roadMR.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            // Add MeshCollider for road physics
            var col = roadObj.AddComponent<MeshCollider>();
            col.sharedMesh = roadMesh;

            // Build sidewalks
            BuildSidewalk(seg.Polyline, seg.WidthMetres, true, roadObj.transform);
            BuildSidewalk(seg.Polyline, seg.WidthMetres, false, roadObj.transform);

            // Centre divider for dual-carriage roads
            if (seg.HasDivider)
                BuildCentreDivider(seg.Polyline, roadObj.transform);

            // Tag for physics layer
            roadObj.layer = LayerMask.NameToLayer("Default");

            // Speed breakers on residential & tertiary roads
            if (seg.Type == KaraikalOSMRoadData.RoadType.Residential ||
                seg.Type == KaraikalOSMRoadData.RoadType.Tertiary)
            {
                PlaceSpeedBreakers(seg.Polyline, seg.WidthMetres, roadObj.transform);
            }
        }

        // ─── Mesh Construction ─────────────────────────────────────────────
        private Mesh BuildExtrudedRoadMesh(Vector3[] polyline, float width, float yOffset)
        {
            var verts = new List<Vector3>();
            var uvs   = new List<Vector2>();
            var tris  = new List<int>();

            float half = width * 0.5f;
            float uvDist = 0f;

            for (int i = 0; i < polyline.Length; i++)
            {
                Vector3 fwd = i < polyline.Length - 1
                    ? (polyline[i + 1] - polyline[i]).normalized
                    : (polyline[i] - polyline[i - 1]).normalized;

                Vector3 right = new Vector3(fwd.z, 0, -fwd.x); // perpendicular XZ

                Vector3 p = new Vector3(polyline[i].x, yOffset, polyline[i].z);
                verts.Add(p - right * half);
                verts.Add(p + right * half);

                float u = uvDist / width;
                uvs.Add(new Vector2(0f, u));
                uvs.Add(new Vector2(1f, u));

                if (i > 0)
                    uvDist += Vector3.Distance(polyline[i], polyline[i - 1]);
            }

            for (int i = 0; i < polyline.Length - 1; i++)
            {
                int b = i * 2;
                tris.AddRange(new[] { b, b + 1, b + 2, b + 1, b + 3, b + 2 });
            }

            var mesh = new Mesh { name = "RoadSurface" };
            mesh.SetVertices(verts);
            mesh.SetUVs(0, uvs);
            mesh.SetTriangles(tris, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        private void BuildSidewalk(Vector3[] polyline, float roadWidth, bool rightSide, Transform parent)
        {
            float offset = roadWidth * 0.5f + sidewalkWidth * 0.5f;
            if (!rightSide) offset = -offset;

            var sw = new GameObject(rightSide ? "Sidewalk_R" : "Sidewalk_L");
            sw.transform.SetParent(parent);

            var swPoly = new Vector3[polyline.Length];
            for (int i = 0; i < polyline.Length; i++)
            {
                Vector3 fwd = i < polyline.Length - 1
                    ? (polyline[i + 1] - polyline[i]).normalized
                    : (polyline[i] - polyline[i - 1]).normalized;
                Vector3 right = new Vector3(fwd.z, 0, -fwd.x);
                swPoly[i] = polyline[i] + right * offset;
            }

            var mesh = BuildExtrudedRoadMesh(swPoly, sidewalkWidth, curbHeight);
            sw.AddComponent<MeshFilter>().mesh = mesh;
            var mr = sw.AddComponent<MeshRenderer>();
            mr.material = GetOrCreateSidewalkMaterial();
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            sw.AddComponent<MeshCollider>().sharedMesh = mesh;
        }

        private void BuildCentreDivider(Vector3[] polyline, Transform parent)
        {
            var div = new GameObject("CentreDivider");
            div.transform.SetParent(parent);
            var mesh = BuildExtrudedRoadMesh(polyline, dividerWidth, curbHeight + 0.05f);
            div.AddComponent<MeshFilter>().mesh = mesh;
            var mr = div.AddComponent<MeshRenderer>();
            mr.material = GetOrCreateCurbMaterial();
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }

        private void BuildBridge(KaraikalOSMRoadData.BridgeData bridge, Transform parent)
        {
            var bridgeObj = new GameObject($"Bridge_{bridge.Name}");
            bridgeObj.transform.SetParent(parent);
            bridgeObj.transform.position = bridge.Center + Vector3.up * bridge.Clearance;

            // Bridge deck
            var deck = GameObject.CreatePrimitive(PrimitiveType.Cube);
            deck.name = "BridgeDeck";
            deck.transform.SetParent(bridgeObj.transform);
            deck.transform.localPosition = Vector3.zero;
            deck.transform.localScale = new Vector3(bridge.Width, 0.5f, bridge.Length);
            var mr = deck.GetComponent<MeshRenderer>();
            mr.material = GetOrCreateBridgeMaterial();

            // Guard rails
            for (int side = -1; side <= 1; side += 2)
            {
                var rail = GameObject.CreatePrimitive(PrimitiveType.Cube);
                rail.name = "BridgeRail";
                rail.transform.SetParent(bridgeObj.transform);
                rail.transform.localPosition = new Vector3(side * bridge.Width * 0.5f, 0.5f, 0f);
                rail.transform.localScale = new Vector3(0.2f, 0.8f, bridge.Length);
                rail.GetComponent<MeshRenderer>().material = GetOrCreateCurbMaterial();
            }
        }

        private void PlaceSpeedBreakers(Vector3[] polyline, float roadWidth, Transform parent)
        {
            if (polyline.Length < 2) return;
            // Place one speed breaker at the midpoint of each polyline
            int mid = polyline.Length / 2;
            var sb = GameObject.CreatePrimitive(PrimitiveType.Cube);
            sb.name = "SpeedBreaker";
            sb.transform.SetParent(parent);
            sb.transform.position = new Vector3(polyline[mid].x, 0.07f, polyline[mid].z);

            Vector3 fwd = (polyline[mid + 1 < polyline.Length ? mid + 1 : mid] - polyline[mid > 0 ? mid - 1 : 0]).normalized;
            sb.transform.forward = fwd;
            sb.transform.localScale = new Vector3(roadWidth, 0.14f, 0.3f);

            // Yellow/black stripe material
            var mat = new Material(Shader.Find("Universal Render Pipeline/Simple Lit"));
            mat.color = new Color(0.95f, 0.80f, 0.0f); // Safety yellow
            sb.GetComponent<MeshRenderer>().material = mat;
        }

        private void PlaceLandmarkMarkers(Transform parent)
        {
            var markerContainer = new GameObject("LandmarkMarkers");
            markerContainer.transform.SetParent(parent);

            foreach (var lm in KaraikalOSMRoadData.Landmarks)
            {
                var marker = new GameObject($"LM_{lm.Name}");
                marker.transform.SetParent(markerContainer.transform);
                marker.transform.position = lm.Position;
                // Simple visual debug — sphere marker at landmark (invisible in release)
#if UNITY_EDITOR
                var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.SetParent(marker.transform);
                sphere.transform.localPosition = Vector3.up * 2f;
                sphere.transform.localScale = Vector3.one * 0.5f;
                sphere.name = "LandmarkMarker";
                var mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
                mat.color = lm.IsRealWorldSite ? Color.cyan : Color.yellow;
                sphere.GetComponent<MeshRenderer>().material = mat;
                Destroy(sphere.GetComponent<Collider>());
#endif
                // Add LandmarkData component for runtime GPS / minimap queries
                var data = marker.AddComponent<LandmarkData>();
                data.LandmarkName = lm.Name;
                data.Category = lm.Category;
                data.IsRealWorldSite = lm.IsRealWorldSite;
                data.IsPlayable = lm.IsPlayable;
            }
        }

        // ─── Material Factories ────────────────────────────────────────────
        private Material GetOrCreateAsphaltMaterial(KaraikalOSMRoadData.RoadType type)
        {
            var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.name = $"Road_{type}";
            // Dark grey asphalt with subtle roughness variation
            float brightness = type == KaraikalOSMRoadData.RoadType.Motorway ? 0.22f : 0.18f;
            mat.color = new Color(brightness, brightness, brightness);
            mat.SetFloat("_Smoothness", 0.05f);
            mat.SetFloat("_Metallic", 0.0f);
            // Tiling UV for road texture (simulated via color + normal map)
            mat.mainTextureScale = new Vector2(1f, 8f);
            return mat;
        }

        private Material GetOrCreateSidewalkMaterial()
        {
            var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.name = "Sidewalk";
            mat.color = new Color(0.72f, 0.68f, 0.62f); // Weathered concrete
            mat.SetFloat("_Smoothness", 0.02f);
            return mat;
        }

        private Material GetOrCreateCurbMaterial()
        {
            var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.name = "Curb";
            mat.color = new Color(0.78f, 0.75f, 0.70f);
            mat.SetFloat("_Smoothness", 0.03f);
            return mat;
        }

        private Material GetOrCreateBridgeMaterial()
        {
            var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.name = "Bridge";
            mat.color = new Color(0.55f, 0.52f, 0.48f);
            mat.SetFloat("_Smoothness", 0.08f);
            return mat;
        }

        public void ClearAll()
        {
            foreach (var obj in _roadObjects)
                if (obj) Destroy(obj);
            _roadObjects.Clear();
        }
    }
}

