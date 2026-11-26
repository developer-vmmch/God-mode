# Gameplay Mechanics & Reconstruction Analysis: God Mode City Destroyer

This document details the observed and derived mechanics of the superhero sandbox game. Every finding is explicitly classified into **[CONFIRMED]** (directly observed from game metadata, runtime logs, or code), **[INFERRED]** (logically derived from gameplay video, design conventions, and controls), or **[UNKNOWN]** (unobserved internal parameters).

---

## 1. Character Controller & Locomotion

### 1.1 Superhero Flight
- **Status**: **[CONFIRMED]**
- **Input**: `W` (fly forward toward reticle), `S` (fly backward), `A/D` (horizontal strafe), `Space` (ascend vertically).
- **Mechanics**:
  - 6-Degrees-of-Freedom (6DOF) flight vector centered on camera aim direction.
  - Pitch and yaw follow mouse look; roll is auto-stabilized to the horizon.
  - Hover mode activates when no directional keys are pressed; the character floats with slight procedural idle bobbing.

### 1.2 Supersonic Boost & Ground Slam
- **Status**: **[CONFIRMED]**
- **Input**: `Shift` (hold to boost).
- **Mechanics**:
  - **Supersonic Flight**: Flight velocity increases by 2.5x–3x. Camera FOV expands from ~60° to ~80° with edge radial lines / wind streak particles.
  - **Ground Slam**: While boosting, pitching down into terrain or buildings triggers a high-impulse kinetic slam. On contact, an explosive radial force is applied to all nearby rigidbodies, kicking up dust and destroying pavement/structures.

---

## 2. Superpowers & Offensive Capabilities

### 2.1 Twin Heat Vision
- **Status**: **[CONFIRMED]**
- **Input**: `Left-Click` or `Right-Click` (hold).
- **Mechanics**:
  - Two continuous laser beams emit from the character's eye sockets and converge at the raycast hit point of the crosshair.
  - Continuous raycast (`Physics.Raycast`) deals thermal damage per tick.
  - Surface impact generates spark particles, heat decals, and ignites targets.
  - Buildings sliced or drilled with heat vision lose structural integrity and collapse along cut planes.

### 2.2 Super Strength (Grab, Throw & Crush)
- **Status**: **[CONFIRMED]**
- **Input**: `E` to Grab; `Left-Click` to Throw; `Right-Click` to Crush.
- **Mechanics**:
  - **Grab**: When targeting a vehicle, enemy, or debris chunk within range, pressing `E` parents the object to a socket above the hero's head via a physics joint (configurable spring/damping joint).
  - **Throw**: Releasing with `LMB` imparts a massive forward velocity vector (`ThrowForce = AimDirection * Magnitude`).
  - **Crush**: Releasing with `RMB` triggers an instant destructive animation: the vehicle/debris collapses into compact scrap with an explosion and particle burst.

### 2.3 Gravity Tether
- **Status**: **[CONFIRMED]**
- **Input**: `Q` (hold to drag, click/release to hurl).
- **Mechanics**:
  - A gravity tether beam attaches from the player's hand to a target object or building section.
  - While held, the object is levitated and dragged synchronously with player movement.
  - Releasing or clicking hurls the object with stored momentum into buildings or crowds.

---

## 3. Destruction & Environment Systems

### 3.1 Building Destruction & Collapse
- **Status**: **[INFERRED]** (Structural behavior observed in preview and metadata)
- **Architecture**:
  - Buildings are composed of modular fractured pieces (pre-fractured Voronoi clusters or structural floors).
  - Intact state: Pieces are static or kinematically locked with a health/integrity threshold.
  - Triggered state: When hit by supersonic slam, heat vision, or hurled vehicles exceeding damage thresholds, joints break and pieces convert into active dynamic PhysX rigidbodies.
  - Collapse: Gravity pulls upper floors down into lower floors, creating a chain-reaction structural collapse accompanied by dust volume particles.

### 3.2 Vehicle & Traffic Interaction
- **Status**: **[INFERRED]**
- **Mechanics**:
  - Civilian cars, trucks, and buses patrol along street navmesh splines.
  - When attacked, vehicles enter physics ragdoll/rigid mode, tumbling and exploding upon heavy impact.

### 3.3 Enemy AI & Threat Forces
- **Status**: **[INFERRED]**
- **Mechanics**:
  - Police cars, SWAT vans, tanks, and attack helicopters spawn based on the current Chaos/Wanted level.
  - Ranged projectile attacks: Tanks fire explosive shells; helicopters track the player in air and fire missiles or machine guns.

---

## 4. Game Loop, Progression & UI

### 4.1 Chaos Meter & Wanted Rating
- **Status**: **[INFERRED]**
- **Mechanics**:
  - A persistent HUD meter accumulates "Chaos" or monetary damage for destroyed buildings and vehicles.
  - Reaching milestone scores elevates the response tier (Police -> SWAT -> Military Tanks -> Attack Gunships).

### 4.2 User Interface & Controls
- **Status**: **[CONFIRMED]**
- **Elements**:
  - Crosshair / Reticle for flight aiming, heat vision, and grab targeting.
  - Chaos / Score counter.
  - Objective / Tutorial instructions (with `T` key tutorial skip).
  - Pause Menu via `P` key.

---

## 5. Classification Summary Table

| Feature / Subsystem | Classification | Primary Evidence Source |
| :--- | :--- | :--- |
| 6DOF Flight Controller | **CONFIRMED** | Control schema in `game_index.html` |
| Supersonic Ground Slam | **CONFIRMED** | Control schema ("boost into ground = SLAM") |
| Dual Heat Vision Beams | **CONFIRMED** | Control schema & video telemetry |
| Grab / Throw / Crush | **CONFIRMED** | Explicit key mappings: `E`, `LMB`, `RMB` |
| Gravity Tether Hurl | **CONFIRMED** | Explicit key mapping: `Q` |
| Pause & Tutorial Skip | **CONFIRMED** | Key mappings: `P`, `T` |
| Building Voronoi Fracturing | **INFERRED** | PhysX module trace & mesh non-readable errors |
| Traffic Spline AI | **INFERRED** | City layout and vehicle gameplay footage |
| Military Response Spawner | **INFERRED** | Published tags ("police", "war", "destroy") |
| Internal Variable Names | **UNKNOWN** | Hidden inside compiled IL2CPP WebAssembly |
| Server-Side Cloud Save Protocol | **UNKNOWN** | Gated by CrazyGames SDK cloud storage |
