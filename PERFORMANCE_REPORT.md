# Target Hardware Optimization & Performance Profile

**Project**: CSVS God Mode — City of Chaos  
**Version**: V0.1.1 Consolidation  
**Target Hardware Profile**: Low-Specification Desktop / Integrated Graphics  
**Evaluation Date**: November 26, 2026  

---

## 1. Development Machine Hardware Specifications

| Hardware Component | Specification | Constraints & Architectural Impact |
| :--- | :--- | :--- |
| **CPU** | Intel Core i3-10105T (4 Cores / 8 Threads, 3.00 GHz base, 3.90 GHz turbo) | PhysX simulation runs synchronously on main thread; requires conservative solver steps (50 Hz / 0.02s) and tight debris body caps. |
| **RAM** | ~8 GB Total System Memory | Shared memory between OS, Unity Editor/Runtime, and GPU VRAM. Requires strict avoidance of GC allocations and asset bloat. |
| **GPU** | Intel UHD Graphics 630 (Integrated, 24 Execution Units, DirectX 12 / OpenGL ES 3.0) | Fill-rate and pixel shader limited. Requires reduced render scale (0.75), hard shadows only, and minimal fragment shader passes. |
| **Storage (C:)** | ~8.56 GB Free | Severely constrained. Must NOT be used for build outputs, editor packages, or large caches. |
| **Storage (D:)** | ~338.26 GB Free | Primary canonical storage for Unity projects, caches, test results, and platform builds. |

---

## 2. Default Hardware Optimization Profile (Tier: LOW)

In accordance with hardware profiling, the game initializes in **Tier: LOW** by default:

```text
================================================================================
LOW GRAPHICS PRESET CONFIGURATION (DEFAULT)
================================================================================
Target Display Resolution      : 1280 x 720
Dynamic Render Scale           : 0.75 (internal buffer: 960 x 540)
Target Frame Rate              : 30 FPS (Application.targetFrameRate = 30)
VSync Mode                     : 1 (Every VBlank)
Active Debris Rigidbody Budget : 50 dynamic bodies (Hard Cap: 150)
Shadow Filtering               : Hard Only (ShadowQuality.HardOnly)
Shadow Draw Distance           : 35.0 meters
Physics Solver Iterations      : 6 position, 2 velocity
Physics Sleep Threshold        : 0.005
Early Chunk Sleep Velocity     : < 0.15 m/s for > 1.0 second -> Freeze to Kinematic
Particle Budget                : Short-lived GPU burst particles; continuous beams use LineRenderer
Post-Processing                : Minimal (Bloom threshold high, depth of field disabled, motion blur disabled)
================================================================================
```

---

## 3. Comparison of Hardware Optimization Presets

| Setting Parameter | Low Preset (Default / UHD 630) | Medium Preset (Mainstream) | High Preset (Dedicated GPU) |
| :--- | :--- | :--- | :--- |
| **Target Hardware** | Intel UHD Graphics 630 / WebGL | GTX 1050 / Radeon 560 | RTX 3060 / Modern Dedicated |
| **Target Frame Rate** | 30 FPS | 60 FPS | 60 FPS |
| **Target Resolution** | 1280 x 720 | 1920 x 1080 | 1920 x 1080 / 1440p |
| **URP Render Scale** | 0.75 | 0.85 | 1.00 |
| **Active Debris Budget** | **50 bodies** | **100 bodies** | **150 bodies (Hard Cap)** |
| **Shadow Distance** | 35 meters | 60 meters | 100 meters |
| **Shadow Quality** | Hard Only | Soft Shadows (All) | Soft Shadows (All) |
| **Traffic Max Count** | 15 vehicles | 30 vehicles | 60 vehicles |
| **Threat AI Squads** | 2 units / wave | 3 units / wave | 4 units / wave |

---

## 4. Performance Optimization Techniques Implemented

1. **Active Debris Clamping & Instant Freezing**:
   `DebrisPoolManager` continuously enforces the active body target. When a 51st debris chunk is spawned on Low tier, the oldest dynamic chunk is immediately frozen (`isKinematic = true`, linear/angular velocities zeroed), freeing PhysX CPU cycles.
2. **Early Velocity Sleep State**:
   `BuildingFractureChunk` monitors rigid body velocity. If a settled chunk moves slower than $0.15\text{ m/s}$ for more than 1 second, it transitions to static sleep without waiting for the full 15-second auto-despawn timer.
3. **Zero-Allocation Runtime Pooling**:
   `ObjectPooler` pre-allocates reusable instances for scrap debris, impact flashes, and particle bursts, preventing runtime GC heap expansion and garbage collector stutter.
4. **Distance-Based NPC Throttling**:
   `CivilianTrafficVehicle` checks player distance; vehicles beyond 120m disable physics sweeps and advance via lightweight mathematical transforms.
5. **GPU Instancing**:
   Building slices, roads, and vehicles are authored for GPU instanced rendering in URP, collapsing draw calls across repeated Voronoi floor blocks.

---

## 5. Empirical Verification Notice

> [!NOTE]
> In strict accordance with performance engineering guidelines, target frame rates (30 FPS / 60 FPS) represent configured engine caps and simulation budgets. Final empirical frame rates, frame time percentiles (99th/95th), and GPU draw call counts will be measured and documented once Unity Editor compilation and actual Windows/WebGL executable runs occur.
