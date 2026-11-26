# Development Roadmap: Independent Unity Superhero Sandbox ("God Mode")

## 1. Project Overview & Philosophy
- **Goal**: Develop an original, high-fidelity 3D superhero city destruction game inspired by the analyzed mechanics of "God Mode City Destroyer".
- **Rule of Clean Room**: Zero proprietary assets, models, sounds, or decompiled code will be copied. All code will be authored fresh in C#, and all art assets will be original or sourced under open commercial licenses (CC0 / Unity Asset Store).
- **Target Platforms**: PC Standalone (Windows/macOS/Linux) and WebGL (Browser).

---

## 2. Phase-by-Phase Roadmap

```
+-------------------------------------------------------------+
| Phase 1: Foundation (Unity 6 + URP + New Input System)      |
+-------------------------------------------------------------+
                               |
                               v
+-------------------------------------------------------------+
| Phase 2: Superhero Locomotion (6DOF Flight & Supersonic)    |
+-------------------------------------------------------------+
                               |
                               v
+-------------------------------------------------------------+
| Phase 3: Superpowers (Heat Vision, Grab/Crush, Gravity Beam)|
+-------------------------------------------------------------+
                               |
                               v
+-------------------------------------------------------------+
| Phase 4: City Destruction & Structural Collapse Engine      |
+-------------------------------------------------------------+
                               |
                               v
+-------------------------------------------------------------+
| Phase 5: Traffic & Response AI (Police, SWAT, Gunships)     |
+-------------------------------------------------------------+
                               |
                               v
+-------------------------------------------------------------+
| Phase 6: Game Loop, Audio, HUD & WebGL Optimization         |
+-------------------------------------------------------------+
```

---

### Phase 1: Project Setup & Baseline Architecture
- [ ] Initialize Unity 6 LTS project with Universal Render Pipeline (URP).
- [ ] Configure PhysX physics settings (`Fixed Timestep = 0.02s`, `Solver Iterations = 6`).
- [ ] Install and configure New Input System package (`com.unity.inputsystem`).
- [ ] Establish folder hierarchy: `Scripts/Core`, `Scripts/Player`, `Scripts/Physics`, `Scripts/AI`, `Scripts/Destruction`.

### Phase 2: Superhero Locomotion & Camera
- [ ] Build third-person Cinemachine virtual camera with free mouse pitch/yaw and procedural roll stabilization.
- [ ] Implement `SuperheroFlightController.cs`:
  - 6DOF thrust along look vector (`W/S/A/D/Space`).
  - Hover mode with floating bobbing oscillations.
  - Supersonic Boost (`Shift`): 3x speed, FOV widening (60° -> 80°), radial wind particles.
  - Supersonic Ground Slam: Detect terrain/building impact while boosting, applying radial explosive impulse and camera shake.

### Phase 3: Superpowers & Physical Manipulation
- [ ] Implement `HeatVisionSystem.cs`:
  - Dual line-renderers / particle beams emitted from eye sockets.
  - Continuous raycast delivering damage and heat decals to hit surfaces.
- [ ] Implement `SuperStrengthGrab.cs`:
  - `E` key raycast to grab vehicles or rubble chunks.
  - Damped physics spring socket above player's head.
  - `LMB` forward throw impulse; `RMB` crush animation and scrap explosion.
- [ ] Implement `GravityTetherSystem.cs`:
  - `Q` key targeting and persistent tether beam.
  - Dragging objects across 3D space with spring dynamics and release fling.

### Phase 4: Environment & Structural Destruction
- [ ] Build a modular city block with pre-fractured Voronoi skyscraper models.
- [ ] Implement `DestructibleBuilding.cs` and `BuildingFractureChunk.cs`:
  - Health threshold transition from static compound collider to dynamic PhysX rigidbodies.
  - Floor-by-floor structural collapse logic.
  - Dust volume particle emitter and screen shake upon collapse.
- [ ] Implement `DebrisPoolManager.cs` to enforce a hard cap of 150 active dynamic bodies.

### Phase 5: Traffic & Threat Response AI
- [ ] Create road grid with spline-based waypoint vehicle traffic.
- [ ] Implement Wanted / Chaos system tracking total property damage.
- [ ] Spawn escalating AI response tiers:
  - Tier 1: Police patrol cruisers (pursue and ram).
  - Tier 2: SWAT vans and armored units.
  - Tier 3: Military tanks with ballistic cannons.
  - Tier 4: Attack helicopters tracking and firing homing missiles.

### Phase 6: Game Loop, Audio & WebGL Polish
- [ ] Build UI overlay: Reticle, Chaos/Destruction score counter, Superpower cooldowns, Pause menu.
- [ ] Integrate sound effects: Supersonic sonic boom, laser drone, building collapse rumble, vehicle crush metal impacts.
- [ ] WebGL profiling: Configure Brotli compression, memory allocators, and shader stripping for smooth browser play.

---

## 3. Review & Sign-Off Gate
Before writing the game implementation code, all seven technical reports must be reviewed and approved by the team.
