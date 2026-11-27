# Project Changelog: CSVS God Mode — City of Chaos

All notable changes to this project are documented in this file in chronological order.

---

## [V0.3.0] — 2026-11-27 (Blender 3D Asset Pipeline & Playable Alpha Vertical Slice)

### Added
- **Verified Blender 4.5.16 LTS Asset Pipeline (`Tools/Blender/`)**:
  - Developed automated headless Python generation scripts for rapid, reproducible 3D asset creation:
    - `generate_hero.py`: Armored superhero character model with dark navy/cobalt plating, brushed silver chevron trim, glowing cyan visor optics, and aerodynamic cape.
    - `generate_auto_rickshaw.py`: 3-wheel South Indian auto-rickshaw featuring yellow canopy, black passenger tub, chrome steering fork, circular headlight with guard, and detailed wheels.
    - `generate_commercial_building.py`: 3-storey South Indian commercial bazaar with 3 shop shutter bays, sunshade chajjas, fascia signboards, and rooftop Sintex water tanks.
    - `generate_motorcycle.py`: 150cc commuter motorcycle with sculpted red fuel tank, stepped seat, telescopic forks, and chrome exhaust.
    - `generate_street_props.py`: 8.5m concrete power distribution pole with cylindrical transformer, crossarms, ceramic insulators, and streetlight fixture.
    - `export_unity_assets.py`: Batch pipeline runner saving `.blend` master files to `D:\Development\CSVS-GodMode\BlenderAssets\` and exporting optimized `.fbx` models directly to `UnityProject\Assets\Art\Models\`.
- **Unity Asset Bridge & Vertical Slice (`BlenderAssetBridge.cs` & `KaraikalVerticalSliceManager.cs`)**:
  - Created runtime/editor asset loader verifying FBX models, scale, meshes, and materials.
  - Constructed Phase 10 Playable Vertical Slice along Karaikal Bharathiyar Road featuring commercial bazaar shophouses, utility poles, auto-rickshaws, and motorcycles.
- **Automated V0.3 Test Suite (`V030BlenderAssetTests.cs` & `V030FeatureTests.cs`)**:
  - Added unit tests verifying Blender test asset import, hero armor geometry, auto-rickshaw 3-wheel configuration, commercial bazaar shopfronts, motorcycle frame, and vertical slice hierarchy.
  - Full automated test suite expanded to **53 passing tests**.
- **Comprehensive Documentation**:
  - Authored `BLENDER_PIPELINE_REPORT.md`, `ARCHITECTURE_MODEL_INVENTORY.md`, `HERO_RIG_VERIFICATION.md`, and `ASSET_GENERATION_REPORT.md`.

### Fixed
- Fixed method-versus-property access across `GameInputManager.cs` to ensure backward compatibility with V0.2.2 powers and flight systems.
- Resolved type ambiguity and missing namespace imports in `ThreatResponseManager.cs` and `MissionSystem.cs`.

---

## [V0.2.2] — 2026-11-26 (Realistic 3D Graphics, Character & Living Karaikal City)
