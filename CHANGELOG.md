# Project Changelog: CSVS God Mode — City of Chaos

All notable changes to this project are documented in this file in chronological order.

---

## [V0.1.1] — 2026-11-26 (Consolidation, Correction & Migration)

### Added
- **Canonical D: Drive Workspace**: Migrated active game development to `D:\Development\CSVS-GodMode` (utilizing 338 GB free space and protecting C: drive).
- **Authoritative Gameplay Configuration**: Authored `Core/GameplayConfig.cs` providing a single source of truth for speeds, physics equations, and quality presets.
- **Explicit Ability State Machine**: Authored `Player/AbilityState.cs` defining `Idle`, `HeatVision`, `Grabbing`, `HoldingObject`, `Throwing`, `Crushing`, `GravityTether`, `SuperPunch`, and `GroundSlam` states.
- **Super Punch Ability**: Consolidated `Powers/SuperPunchSystem.cs` (`F` key) delivering 450 structural damage and 85 N knockback impulse.
- **Object Pooler**: Migrated `Core/ObjectPooler.cs` for zero-allocation recycling of debris, particles, and impact flashes.
- **Hardware Optimization Presets**: Authored `Settings/QualitySettingsManager.cs` tailored for Intel Core i3-10105T / UHD 630 (Default LOW: 1280x720, 0.75 render scale, 30 FPS target, 50 active debris).
- **First/Third-Person Camera Toggle**: Added instantaneous `V` key perspective switching, anti-clipping obstacle spherecasts, and trauma-based 6DOF shake decay to `Player/SuperheroCameraController.cs`.
- **Instant Scene Reset**: Added `R` key scene reload and score reset to `Core/GameManager.cs`.
- **Automated Verification Harness**: Authored `VerificationRunner.cs` executing 32 unit tests and generating `EDITMODE_RESULTS.xml` and `PLAYMODE_RESULTS.xml`.

### Changed
- **Ground Locomotion Merged**: Consolidated ground locomotion (walk 8 m/s, sprint 16 m/s, jump 3.5 m) directly into `Player/SuperheroFlightController.cs`, enabling seamless transitions between ground, hover, and flight modes.
- **Supersonic Boost Speed Standardized**: Updated flight kinematics boost speed from 90 m/s to **95 m/s** (~342 km/h).
- **Ground Slam Falloff Corrected**: Implemented exact quadratic attenuation formula:
  $$F(d) = 1500 \cdot \left(\text{Clamp01}\left(1 - \frac{d}{25}\right)\right)^2$$
  Yielding exactly 1500 N at epicenter, 375 N at midpoint, and 0 N at perimeter.
- **Input Conflict Elimination**: Heat Vision is strictly gated by `CanActivateHeatVision()` so Left/Right Click while holding an object only executes Throw/Crush.
- **Dynamic Debris Budgets**: `Destruction/DebrisPoolManager.cs` now dynamically adjusts active body limits based on graphics preset (50 on Low, 100 on Med, 150 on High), while enforcing a strict ceiling of 150 bodies.
- **HUD Telemetry Enhanced**: `UI/HUDController.cs` displays real-time Speedometer (KM/H), Locomotion mode, Ability state, Wanted rating, Chaos score, and Camera perspective.

### Security & Integrity
- **REA Framework Protected**: Zero files in `C:\Users\Admin\Downloads\rea-main` were modified or deleted; REA MCP remains fully functional.
- **Evidence Preserved**: All 7 reverse-engineering reports and evidence summaries mirrored cleanly in `D:\Development\CSVS-GodMode\reports` and `evidence`.
- **Backups Created**: Complete cold backups stored in `D:\Development\CSVS-GodMode\Backups`.

---

## [V0.1.0] — 2026-11-25 (Initial Clean-Room Prototype)
- Completed initial REA passive observation of reference WebGL target.
- Extracted architecture specs (Unity 6 LTS `6000.0.84f1`, URP, PhysX single-threaded 50 Hz).
- Authored initial 7 technical reports.
- Initialized independent clean-room Unity project structure under `C:\Development\CSVS-GodMode\UnityProject`.
- Authored core flight controller, dual heat vision, super strength grab, building fracture slices, and civilian traffic simulation.
