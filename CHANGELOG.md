# Project Changelog: CSVS God Mode — City of Chaos

All notable changes to this project are documented in this file in chronological order.

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
