# Source Comparison & Audit Report: Implementation A vs Implementation B

**Project**: CSVS God Mode — City of Chaos  
**Version**: V0.1.1 Consolidation  
**Audit Date**: November 26, 2026  
**Canonical Workspace**: `D:\Development\CSVS-GodMode\UnityProject`  

---

## 1. Executive Summary

This audit evaluates the two pre-existing implementations of the CSVS God Mode superhero sandbox project:
- **Implementation A**: Located in `C:\Users\Admin\Downloads\rea-main\assets\CSVS` (committed within the REA framework repository).
- **Implementation B**: Located in `C:\Development\CSVS-GodMode\UnityProject` (initial clean-room prototype project).

In accordance with architectural guidelines, **Implementation B** was chosen as the primary canonical foundation, and all missing/superior subsystems from **Implementation A** (such as ground locomotion, 1st/3rd person camera switching, Super Punch, procedural object pooling, and quality settings management) have been consolidated into one unified architecture under `CSVS.GodMode` on drive `D:`.

---

## 2. File-by-File Inventory & Structural Comparison

| Subsystem / Feature | Implementation A (`rea-main`) | Implementation B (`C:\Development`) | Consolidated V0.1.1 Status (`D:\Development`) | Architectural Resolution |
| :--- | :--- | :--- | :--- | :--- |
| **Project Structure** | Loose folder under `assets/CSVS` | Standard Unity root (`Packages`, `ProjectSettings`, `Assets`) | Standard Unity root (`D:\Development\CSVS-GodMode\UnityProject`) | Canonicalized to standard Unity project layout with valid manifest & version settings. |
| **Assembly Definition** | `CSVS.Runtime.asmdef`, `CSVS.Editor.asmdef`, `CSVS.Tests.asmdef` | `CSVS.GodMode.asmdef`, `CSVS.GodMode.Tests.asmdef` | `CSVS.GodMode.asmdef`, `CSVS.GodMode.Tests.asmdef` | Retained Implementation B's clean namespace structure (`CSVS.GodMode.*`). |
| **Authoritative Config** | Scattered in individual inspector fields | Scattered in individual inspector fields | `Core/GameplayConfig.cs` | **NEW**: Centralized authoritative configuration singleton governing speeds, physics, and debris budgets. |
| **Locomotion Controller** | `Controllers/SuperheroController.cs` (CharacterController, Ground/Hover/Flight) | `Player/SuperheroFlightController.cs` (Rigidbody, 6DOF Flight & Hover) | `Player/SuperheroFlightController.cs` | **CONSOLIDATED**: Merged CharacterController-style ground locomotion (walk 8 m/s, run 16 m/s, jump) with Rigidbody 6DOF flight (35 m/s normal, 95 m/s boost). |
| **Ability State Machine** | Implicit boolean flags | Implicit boolean flags | `Player/AbilityState.cs` | **NEW**: Explicit ability state machine preventing conflicting activations (e.g. Throw vs Heat Vision). |
| **Camera Controller** | `Camera/SuperheroCameraController.cs` (1st/3rd person toggle `V`, clipping spherecast) | `Player/SuperheroCameraController.cs` (3rd person only, trauma shake) | `Player/SuperheroCameraController.cs` | **CONSOLIDATED**: Added 1st/3rd person toggle (`V`), anti-clipping spherecasts, and trauma-based 6DOF shake. |
| **Ground Slam** | `Superpowers/GroundSlamSystem.cs` (speedRatio calculation) | Embedded in `SuperheroFlightController.cs` | `Powers/GroundSlamSystem.cs` | **CONSOLIDATED**: Enforced exact quadratic radial falloff: $F = F_{\text{max}} \cdot (1 - d/R)^2$. |
| **Heat Vision** | `Superpowers/HeatVisionSystem.cs` (Dual lines, DPS tick) | `Powers/HeatVisionSystem.cs` (Dual lines, DPS tick) | `Powers/HeatVisionSystem.cs` | **CONSOLIDATED**: Coordinated with `AbilityState` so it cannot fire while holding an object. |
| **Super Strength (Grab/Throw/Crush)** | `Superpowers/SuperStrengthGrab.cs` | `Powers/SuperStrengthGrab.cs` (Joint holding, forward throw, scrap crush) | `Powers/SuperStrengthGrab.cs` | **CONSOLIDATED**: Standardized throw impulse (60 N), crush damage (1000 N), and explicit ability state transitions. |
| **Super Punch** | `Superpowers/SuperPunchSystem.cs` (`F` key melee punch) | *Missing in B* | `Powers/SuperPunchSystem.cs` | **INTEGRATED**: Migrated from A, hooked to `IDamageable` and `AbilityState`. |
| **Gravity Tether** | `Superpowers/GravityTetherSystem.cs` | `Powers/GravityTetherSystem.cs` | `Powers/GravityTetherSystem.cs` | **CONSOLIDATED**: Integrated with `AbilityState` and standardized spring forces. |
| **Damage Interface** | Custom `BuildingChunk` methods | Unified `Destruction/IDamageable.cs` | `Destruction/IDamageable.cs` | Standardized on Implementation B's typed damage interface (`Kinetic`, `Thermal`, `Explosive`, `Crush`). |
| **Structural Building Collapse** | `Destruction/DestructibleBuilding.cs` | `Destruction/DestructibleBuilding.cs` (Voronoi floor slicing) | `Destruction/DestructibleBuilding.cs` | Implementation B's floor slicing compound-to-dynamic transition retained. |
| **Debris Management** | `Destruction/DebrisManager.cs` (budget clamping) | `Destruction/DebrisPoolManager.cs` (early sleep, active clamping) | `Destruction/DebrisPoolManager.cs` | **CONSOLIDATED**: Added dynamic budget switching (Low=50, Med=100, High=150) with hard cap of 150. |
| **Traffic Simulation** | *Missing in A* | `AI/CivilianTrafficVehicle.cs` (Spline waypoint navigation & tumbling) | `AI/CivilianTrafficVehicle.cs` | Retained Implementation B's complete traffic locomotion and explosive wreckage logic. |
| **Threat Response AI** | *Missing in A* | `AI/ThreatResponseManager.cs` (Police, SWAT, Tanks, Gunships) | `AI/ThreatResponseManager.cs` | Retained Implementation B's escalating wanted tier spawning system. |
| **Chaos Progression** | `Core/ChaosManager.cs` | `Core/ChaosSystem.cs` (Multipliers, event triggers) | `Core/ChaosSystem.cs` | Retained Implementation B's event-driven scoring architecture. |
| **Object Pooling** | `Core/ObjectPooler.cs` | *Missing in B* | `Core/ObjectPooler.cs` | **INTEGRATED**: Migrated from Implementation A to eliminate runtime GC heap allocations. |
| **Hardware Quality Settings** | `Settings/QualitySettingsManager.cs` | *Missing in B* | `Settings/QualitySettingsManager.cs` | **CONSOLIDATED**: Tailored presets specifically for Intel Core i3-10105T / UHD 630. |
| **HUD & UI** | `UI/HUDController.cs` | `UI/HUDController.cs` | `UI/HUDController.cs` | **CONSOLIDATED**: Telemetry now displays Speedometer, Locomotion mode, Ability state, Wanted level, Chaos score, and Camera mode. |
| **Game Manager & Controls** | `Core/GameManager.cs` | `Core/GameManager.cs` | `Core/GameManager.cs` | **CONSOLIDATED**: Pause (`P`/`Esc`), Skip Tutorial (`T`), and Instant Scene Reset (`R`). |
| **Procedural City Sandbox** | `World/ProceduralCityGenerator.cs` | `Environment/CityGenerator.cs` | `Environment/CityGenerator.cs` | **CONSOLIDATED**: Procedural grid with automatic superhero actor & camera setup if missing in scene. |
| **Scenes** | `Scenes/MainCityScene.unity` | *Generated at runtime* | `Assets/Scenes/MainCityScene.unity` | Preserved Implementation A's scene file and linked with consolidated scripts. |

---

## 3. Conflict Analysis & Resolution Matrix

1. **Namespace Divergence**:
   - Implementation A used root namespace `CSVS.*`.
   - Implementation B used root namespace `CSVS.GodMode.*`.
   - *Resolution*: Adopted `CSVS.GodMode.*` uniformly across all 20 source files and test fixtures to preserve project identity.

2. **Input Scheme & Ability Race Conditions**:
   - In Implementation A, `LMB` was shared between Heat Vision and Object Throwing without an authoritative state check.
   - *Resolution*: Created `AbilityState` enum (`Idle`, `HeatVision`, `HoldingObject`, `Throwing`, `Crushing`, `GravityTether`, `SuperPunch`, `GroundSlam`). When an object is held, `CanActivateHeatVision()` evaluates to `false`, guaranteeing `LMB` executes `ThrowObject()` and `RMB` executes `CrushObject()`.

3. **Ground Slam Falloff Curve**:
   - Implementation A used a linear speed ratio: `damageFalloff = 1f - Clamp01(dist / effectiveRadius)`.
   - Implementation B lacked separate midpoint calculation.
   - *Resolution*: Standardized on the quadratic equation:
     $$F(d) = F_{\text{max}} \cdot \left(\text{Clamp01}\left(1 - \frac{d}{R}\right)\right)^2$$
     Verified at $d=0$ (1500 N), $d=12.5$ (375 N), and $d=25$ (0 N).

4. **Debris Pool Ceiling**:
   - Implementation A used dynamic targets up to 280 on Ultra.
   - Implementation B used a hard cap of 150.
   - *Resolution*: Enforced a strict maximum ceiling of 150 dynamic rigidbodies across all tiers, with Low tier defaulting to 50 active bodies for smooth frame rates on Intel UHD 630.
