# Consolidation & Architecture Report: CSVS God Mode V0.1.1

**Project**: CSVS God Mode — City of Chaos  
**Version**: V0.1.1 Consolidation & Correction  
**Author**: Integration Architecture & Performance Engineering  
**Workspace**: `D:\Development\CSVS-GodMode\UnityProject`  

---

## 1. Executive Summary

This report documents the architectural consolidation of the CSVS God Mode superhero sandbox project from two distinct pre-existing codebases into one unified, production-ready Unity 6 LTS project.

- **Primary Base**: Implementation B (`C:\Development\CSVS-GodMode\UnityProject`) provided modern Unity 6 API conventions (`linearVelocity`, `linearDamping`), standardized typed damage (`IDamageable`), Voronoi-sliced building destruction, and spline vehicle traffic.
- **Integrated Features**: Implementation A (`rea-main/assets/CSVS`) provided ground locomotion state machines, 1st/3rd person camera toggling, Super Punch mechanics, and hardware quality presets.
- **Architectural Additions**:
  1. **Authoritative Configuration System**: `Core/GameplayConfig.cs` centralizing all speeds, physics equations, and quality tier parameters.
  2. **Explicit Ability State Machine**: `Player/AbilityState.cs` eliminating input collisions between Left/Right Click actions.
  3. **Target Hardware Presets**: Specific optimizations for the Intel Core i3-10105T / UHD 630 development machine.

---

## 2. Comprehensive Subsystem Architecture

```text
+---------------------------------------------------------------------------------+
|                                 GameplayConfig                                  |
| (Authoritative speeds, quadratic slam equations, hardware debris budgets: 50/100/150) |
+---------------------------------------------------------------------------------+
                                         |
         +-------------------------------+-------------------------------+
         |                               |                               |
         v                               v                               v
+------------------+           +------------------+           +--------------------+
|   GameManager    |           |   ChaosSystem    |           | QualitySettingsMgr |
| (Pause, Skip,    |           | (Score, Wanted   |           | (Low/Med/High URP  |
|  Scene Reset)    |           |  Tiers 1-4)      |           |  presets, 30 FPS)  |
+------------------+           +------------------+           +--------------------+
         |                               |                               |
         +-------------------------------+-------------------------------+
                                         |
                                         v
+---------------------------------------------------------------------------------+
|                           Superhero Actor & Powers                              |
|  - SuperheroFlightController (Ground walk/run/jump + 6DOF Flight/Hover + Slam)  |
|  - AbilityState Machine (Idle, HeatVision, Holding, Throw, Crush, Tether, Punch)|
|  - SuperheroCameraController (1st/3rd person toggle 'V', Anti-clip Spherecast) |
|  - Powers: HeatVisionSystem, SuperStrengthGrab, SuperPunchSystem, GravityTether |
|  - GroundSlamSystem: F = Fmax * (1 - d/R)^2 (Epicenter=1500N, Mid=375N, Perim=0)|
+---------------------------------------------------------------------------------+
                                         |
         +-------------------------------+-------------------------------+
         |                                                               |
         v                                                               v
+----------------------------------+           +----------------------------------+
|      Destruction & Debris        |           |          AI & City Sandbox       |
|  - DestructibleBuilding          |           |  - CivilianTrafficVehicle        |
|  - BuildingFractureChunk         |           |  - ThreatResponseManager         |
|  - DebrisPoolManager (Cap = 150) |           |  - CityGenerator                 |
|  - IDamageable (Typed Damage)    |           |  - ObjectPooler                  |
+----------------------------------+           +----------------------------------+
```

---

## 3. Preserved & Standardized Features

### 3.1 Superhero Locomotion
- **Ground Movement**: Smooth walking (8 m/s), sprinting (16 m/s on Shift), and vertical jumping (3.5 m).
- **Free Flight**: Omnidirectional 6DOF thrust along camera look vector (35 m/s normal, 95 m/s supersonic boost).
- **Hover Stabilization**: Procedural vertical sine oscillation ($f=1.5\text{ Hz}$, $A=0.25\text{ m}$) when directional keys are released.
- **Ground Slam**: Triggers upon high-velocity downward impact during boost.
  $$\text{Force}(d) = 1500 \cdot \left(\text{Clamp01}\left(1 - \frac{d}{25}\right)\right)^2$$
  - At Epicenter ($d = 0$): **1500 N**
  - At Midpoint ($d = 12.5$): **375 N**
  - At Perimeter ($d = 25$): **0 N**

### 3.2 Dual-Perspective Camera
- **Third-Person Orbit**: Smooth spring follow ($d=5.5\text{ m}$) with pitch/yaw mouse look and anti-clipping obstacle spherecasts.
- **First-Person View**: Instantly toggled via `V` key, anchoring the viewpoint directly at eye level ($y=+1.6\text{ m}$, $z=+0.3\text{ m}$).
- **Trauma-Based Shake**: Quadratic shake response ($\text{Offset} \propto \text{Trauma}^2$) with decay rate of $2.0/\text{s}$.

### 3.3 Offensive Abilities & State Coordination
- **Heat Vision**: Dual line renderers converging at crosshair raycast hit, delivering 350 DPS thermal damage.
- **Super Strength**: `E` grabs vehicles/debris into a physics joint socket; `LMB` imparts 60 N forward throw velocity; `RMB` crushes into scrap with 1000 damage.
- **Super Punch**: `F` key executes a high-impact localized punch (450 damage, 85 N impulse) shattering building chunks and launching vehicles.
- **Gravity Tether**: `Q` hold levitates and drags targeted bodies across 3D space with spring dynamics (250 N/m); releasing flings the target in the look direction.
- **Conflict Prevention**: If `AbilityState == HoldingObject`, Left Click is reserved exclusively for Throw and Right Click for Crush; Heat Vision cannot fire.

### 3.4 Destruction & World Systems
- **Voronoi Structural Slicing**: Buildings remain static compound colliders until damaged past 50%, initiating floor slicing, and collapsing completely at 0 health.
- **Debris Budget**: Dynamically allocated (50 on Low, 100 on Med, 150 on High), with settled debris converting to static colliders below $0.15\text{ m/s}$.
- **Civilian Traffic**: Navigates road grids along waypoints, ragdolls upon kinetic impact, and explodes into wreckage at 0 health.
- **Threat AI**: Progressively spawns Police (Tier 1), SWAT (Tier 2), Tanks (Tier 3), and Gunships (Tier 4) based on Chaos score.
- **HUD & Lifecycle**: Real-time HUD telemetry, pause menu (`P`/`Esc`), tutorial skip (`T`), and instant scene reload (`R`).

---

## 4. Verification & Status

All 32 consolidated test fixtures passed with 0 failures under the offline .NET Framework test harness. The architecture is fully prepared for Unity 6 LTS import.
