# Project Changelog: CSVS God Mode — City of Chaos

All notable changes to this project are documented in this file in chronological order.

---

## [V0.2.1] — 2026-11-26 (Real Karaikal Map Correction & Realism Upgrade)

### Added
- **Authoritative WGS84 Local Tangent Plane Projection (`KaraikalGeoProjection.cs`)**:
  - Implemented exact geodesic metric conversion evaluated at reference origin ($10.925417^\circ\,\text{N}, 79.838028^\circ\,\text{E}$ at Kamarajar Salai [NH 32] & Bharathiyar Rd).
  - Meridional scale $M = 110,574.6\,\text{m/deg}$ Lat, Parallel scale $S_{\lambda} = 109,281.3\,\text{m/deg}$ Lon.
  - Bidirectional conversion round-trip verified at $< 10^{-6}$ degrees ($< 0.1\,\text{m}$).
- **Authoritative Landmark Registry**: Corrected and verified landmark offsets:
  - Karaikal Beach Promenade: $X = +929.4\,\text{m}, Z = +65.8\,\text{m}$ (corrected from inaccurate $650, 20$).
  - Arasalar River Bridge: $X = -55.0\,\text{m}, Z = -818.5\,\text{m}$ (corrected from inaccurate $0, -380$).
  - Karaikal Railway Station: $X = -1093.7\,\text{m}, Z = +784.3\,\text{m}$ (corrected from inaccurate $-550, 650$).
  - Karaikal Deepwater Port: $X = +761.9\,\text{m}, Z = -9997.7\,\text{m}$ (corrected from inaccurate $350, -850$).
  - Karaikal Ammaiyar Temple: $X = -278.4\,\text{m}, Z = +200.5\,\text{m}$.
  - New Bus Stand Terminal: $X = -423.8\,\text{m}, Z = +619.6\,\text{m}$.
- **250m Chunk Spatial World Streaming (`KaraikalWorldStreamer.cs`)**:
  - Distance-based dynamic chunk loading (500m load radius, 650m unload radius hysteresis).
  - Preserves persistent building collapse and destruction state across chunk lifecycles.
- **Original Armored Humanoid Superhero Model (`SuperheroHumanoidVisual.cs`)**:
  - Athletic anatomy, deep cobalt armor, heavy silver pauldrons, glowing cyan visor and chest core.
  - Dynamic procedural cape simulation responding to flight velocity and aerodynamics.
  - Procedural kinematic animation driving walk/run, hover, supersonic boost, punch, and ground slam.
- **Playable Indian Vehicles (`PlayableIndianVehicle.cs`)**:
  - Drivable Indian Hatchback, Sedan, South Indian Auto-Rickshaw (yellow/black canopy), and Commuter Motorcycle.
  - Player enter/exit (`F` key), suspension physics, steering, headlights, and damage handling.
- **Automated GIS Test Suite (`KaraikalGeoTests.cs`)**:
  - Added 8 automated EditMode tests verifying WGS84 projection, round-trip fidelity, landmark offsets, and 250m chunk coordinates.
  - Total automated test count increased to **43 tests** (27 EditMode + 16 PlayMode), achieving a 100% pass rate.
- **Live Windows Standalone Verification**:
  - Built and executed `CSVS-GodMode.exe` (ExitCode 0).
  - Measured live memory footprint: WorkingSet = 270.66 MB (within $< 300\,\text{MB}$ strict target).
  - Captured verified screenshots `gameplay_v021_town_center.png` and `gameplay_v021_minimap.png` with 0.00% magenta shader errors.

---

## [V0.2.0] — 2026-11-26 (Karaikal Open-World Foundation & Indian Architecture)

### Added
- **Karaikal Geographic Foundation**: Integrated real-world GIS coordinates for Karaikal, Puducherry, India (`KaraikalGeoData.cs`, reference origin: 10.9254° N, 79.8380° E at Kamarajar Salai & Bharathiyar Rd).
- **Arterial Highway & Road Network**: Authored `KaraikalRoadNetwork.cs` generating Kamarajar Salai (NH 32), Bharathiyar Road, Beach Road, Thirunallar Highway, Arasalar River Channel, and the 85-meter concrete Arasalar Bridge.
- **Bay of Bengal & Coastal Dunes**: Modeled 350-meter golden sand beach strip and ocean water plane with paved coastal promenade.
- **Modular South Indian Architecture**: Authored `ModularIndianBuildingSystem.cs` constructing authentic regional buildings:
  - Commercial Bazaar Complexes with bilingual Tamil/English signboards, roll-up shutters, and concrete sunshade chajjas.
  - South Indian Residential Houses with perimeter compound walls, iron gates, front verandahs (thinnai), terracotta tile trims, and rooftop Sintex water tanks.
  - Traditional Heritage Structures with Dravidian parapets and entrance mandapams.
  - Port Industrial Warehouses with stacked shipping containers (Red, Blue, Green, Yellow).
  - Modern Coastal High-Rise Towers (8–13 stories) with rooftop helipads for superhero flight and destruction.
- **GTA-Style Karaikal GPS Minimap**: Upgraded `HUDController.cs` with an interactive rotating minimap displaying player heading, coastline, river, highway axes, live GPS coordinates, and current district banners in Tamil and English.
- **Full-Screen District Map**: Added interactive full-screen city map modal toggled with the `M` key showing landmark pins.
- **Full Test Suite Verification**: Re-verified all 19 EditMode and 16 PlayMode tests in Unity batchmode with a 100% pass rate.
- **Live Windows x64 Build**: Rebuilt and executed `D:\Development\CSVS-GodMode\Builds\Windows\CSVS-GodMode.exe` on Intel UHD 630 GPU; verified WorkingSet = 261.23 MB, 0% magenta pixels, 0 missing script warnings.

---

## [V0.1.2] — 2026-11-26 (Live Unity Execution & Standalone Builds)

### Added
- **Unity 6 LTS Live Execution**: Confirmed operational deployment under Unity 6000.0.84f1 on target machine (Intel Core i3-10105T, Intel UHD Graphics 630).
- **Unity Test Runner Automation**: Successfully executed **19 EditMode tests** (`EDITMODE_RESULTS.xml`) and **16 PlayMode tests** (`PLAYMODE_RESULTS.xml`) with a 100% pass rate.
- **Standalone Windows x64 Mono Player**: Built and verified `D:\Development\CSVS-GodMode\Builds\Windows\CSVS-GodMode.exe` (671 KB).
- **Standalone WebGL 2.0 Player**: Built and verified `D:\Development\CSVS-GodMode\Builds\WebGL\` (14.25 MB compressed package, 11.98 MB wasm).
- **Live Performance Verification**: Measured real memory footprint (WorkingSet = 240.07 MB) and confirmed Direct3D 11 / PhysX 4.1.2 initialization on Intel UHD 630 GPU.
- **In-Game IMGUI Telemetry Overlay**: Added comprehensive HUD telemetry overlay in `HUDController.cs` displaying Speed (KM/H & m/s), Wanted Tier, Chaos Score, Ability state, and Framerate.
- **Gameplay Screenshot Capture**: Verified rendering pipeline and captured live gameplay screenshot to `D:\Development\CSVS-GodMode\Screenshots\gameplay_v012.png`.

### Fixed
- **URP Render Pipeline & Shaders Hotfix**: Created and assigned `UniversalRendererData` and `UniversalRenderPipelineAsset` in `GraphicsSettings.asset` and `QualitySettings.asset`. Generated URP Lit materials (`RoadMaterial`, `BuildingMaterial`, `BuildingChunkMaterial`, `PlayerMaterial`), and wired them into `CityGenerator.cs`, `Main Camera`, and `Directional Light`, completely resolving the magenta/pink shader rendering failure (verified 0.00% magenta pixels in live gameplay).
- **YAML Scene Component Binding**: Corrected placeholder GUIDs in `Assets/Scenes/MainCityScene.unity` to match exact runtime script GUIDs, completely eliminating "referenced script is missing" warnings.
- **Batchmode Multi-Instance Lock Conflict**: Added automated process hygiene and `Temp/UnityLockfile` cleanup to build runners.
- **Headless Camera Fallback**: Resolved null camera dereferences in headless batchmode test runs inside `SuperheroFlightController.cs`.
- **Component Awake Timing**: Added lazy-initialization safety guards for `Rigidbody` in `BuildingFractureChunk.cs` and `SuperheroFlightController.cs` during EditMode testing.
- **Ambiguous Shadow Quality Reference**: Fully qualified `UnityEngine.ShadowQuality` in `QualitySettingsManager.cs` to eliminate URP namespace ambiguity.

---

## [V0.1.1] — 2026-11-26 (Consolidation, Correction & Migration)

### Added
- **Canonical D: Drive Workspace**: Migrated active game development to `D:\Development\CSVS-GodMode` (utilizing 338 GB free space and protecting C: drive).
- **Authoritative Gameplay Configuration**: Authored `Core/GameplayConfig.cs` providing a single source of truth for speeds, physics equations, and quality presets.
- **Explicit Ability State Machine**: Authored `Player/AbilityState.cs` defining `Idle`, `HeatVision`, `Grabbing`, `HoldingObject`, `Throwing`, `Crushing`, `GravityTether`, `SuperPunch`, and `GroundSlam` states.
- **Super Punch Ability**: Consolidated `Powers/SuperPunchSystem.cs` delivering 450 structural damage and 85 N knockback impulse.
- **Object Pooler**: Migrated `Core/ObjectPooler.cs` for zero-allocation recycling of debris, particles, and impact flashes.
- **Hardware Optimization Presets**: Authored `Settings/QualitySettingsManager.cs` tailored for Intel Core i3-10105T / UHD 630 (Default LOW: 1280x720, 0.75 render scale, 30 FPS target, 50 active debris).
- **First/Third-Person Camera Toggle**: Added instantaneous `V` key perspective switching, anti-clipping obstacle spherecasts, and trauma-based 6DOF shake decay to `Player/SuperheroCameraController.cs`.
- **Instant Scene Reset**: Added `R` key scene reload and score reset to `Core/GameManager.cs`.

### Changed
- **Ground Locomotion Merged**: Consolidated ground locomotion directly into `Player/SuperheroFlightController.cs`.
- **Supersonic Boost Speed Standardized**: Updated flight kinematics boost speed from 90 m/s to **95 m/s** (~342 km/h).
- **Ground Slam Falloff Corrected**: Implemented exact quadratic attenuation formula:
  $$F(d) = 1500 \cdot \left(\text{Clamp01}\left(1 - \frac{d}{25}\right)\right)^2$$
- **Input Conflict Elimination**: Heat Vision is strictly gated by `CanActivateHeatVision()` so Left/Right Click while holding an object only executes Throw/Crush.
- **Dynamic Debris Budgets**: Dynamic caps (50 on Low, 100 on Med, 150 on High).

### Security & Integrity
- **REA Framework Protected**: Zero files in `C:\Users\Admin\Downloads\rea-main` were modified or deleted; REA MCP remains fully functional.
- **Evidence Preserved**: All 7 reverse-engineering reports and evidence summaries mirrored cleanly.

---

## [V0.1.0] — 2026-11-25 (Initial Clean-Room Prototype)
- Completed initial REA passive observation of reference WebGL target.
- Extracted architecture specs (Unity 6 LTS `6000.0.84f1`, URP, PhysX single-threaded 50 Hz).
- Authored initial 7 technical reports.
