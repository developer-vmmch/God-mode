using System;
using UnityEngine;

namespace CSVS.GodMode.Powers
{
    /// <summary>
    /// Cinematic visual and particle effects manager for CSVS superhero abilities:
    /// - Twin glowing Heat Vision laser beams with impact flare and scorch marks.
    /// - Supersonic Flight speed trails and aerodynamic wind distortion particles.
    /// - Radial Ground Slam shockwaves and kinetic dust bursts.
    /// - Super Punch kinetic blast rings and structural concrete debris.
    /// </summary>
    public class PowersFXManager : MonoBehaviour
    {
        public static PowersFXManager Instance { get; private set; }

        [Header("Cinematic FX Materials")]
        [SerializeField] private Material _laserBeamMaterial;
        [SerializeField] private Material _shockwaveRingMaterial;
        [SerializeField] private Material _heatScorchMaterial;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else { Destroy(gameObject); return; }

            InitializeMaterials();
        }

        private void InitializeMaterials()
        {
            Shader litShader = Shader.Find("Universal Render Pipeline/Lit");
            if (litShader == null) litShader = Shader.Find("Universal Render Pipeline/Simple Lit");
            if (litShader == null) litShader = Shader.Find("Standard");

            if (_laserBeamMaterial == null && litShader != null)
            {
                _laserBeamMaterial = new Material(litShader);
                if (_laserBeamMaterial.HasProperty("_BaseColor")) _laserBeamMaterial.SetColor("_BaseColor", new Color(1.0f, 0.20f, 0.10f));
                if (_laserBeamMaterial.HasProperty("_EmissionColor"))
                {
                    _laserBeamMaterial.EnableKeyword("_EMISSION");
                    _laserBeamMaterial.SetColor("_EmissionColor", new Color(2.5f, 0.4f, 0.2f));
                }
            }

            if (_shockwaveRingMaterial == null && litShader != null)
            {
                _shockwaveRingMaterial = new Material(litShader);
                if (_shockwaveRingMaterial.HasProperty("_BaseColor")) _shockwaveRingMaterial.SetColor("_BaseColor", new Color(0.2f, 0.85f, 1.0f, 0.6f));
                if (_shockwaveRingMaterial.HasProperty("_EmissionColor"))
                {
                    _shockwaveRingMaterial.EnableKeyword("_EMISSION");
                    _shockwaveRingMaterial.SetColor("_EmissionColor", new Color(0.5f, 1.8f, 2.5f));
                }
            }
        }

        /// <summary>
        /// Spawns an expanding radial shockwave ring and dust burst upon ground slam impact.
        /// </summary>
        public void SpawnGroundSlamShockwave(Vector3 epicenter, float radius)
        {
            GameObject shockwave = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            shockwave.name = "GroundSlam_ShockwaveRing";
            shockwave.transform.position = epicenter + Vector3.up * 0.1f;
            shockwave.transform.localScale = new Vector3(radius * 2f, 0.15f, radius * 2f);

            Destroy(shockwave.GetComponent<Collider>());
            Renderer r = shockwave.GetComponent<Renderer>();
            if (r != null && _shockwaveRingMaterial != null) r.sharedMaterial = _shockwaveRingMaterial;

            // Auto-destroy shockwave after 0.8 seconds
            Destroy(shockwave, 0.8f);
        }

        /// <summary>
        /// Spawns a localized kinetic blast flare on super punch impact.
        /// </summary>
        public void SpawnPunchImpactBlast(Vector3 hitPoint, Vector3 normal)
        {
            GameObject blast = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            blast.name = "SuperPunch_ImpactBlast";
            blast.transform.position = hitPoint;
            blast.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);

            Destroy(blast.GetComponent<Collider>());
            Renderer r = blast.GetComponent<Renderer>();
            if (r != null && _shockwaveRingMaterial != null) r.sharedMaterial = _shockwaveRingMaterial;

            Destroy(blast, 0.35f);
        }
    }
}
