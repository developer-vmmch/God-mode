# Game Architecture Technical Blueprint: God Mode City Destroyer

## 1. Engine & Runtime Core

### 1.1 Engine Specifications
- **Engine**: Unity 6 LTS (`6000.0.84f1`)
- **Compilation Target**: WebGL 2.0 (asm.js/WebAssembly dual pipeline compiled via Emscripten / Clang backend).
- **Execution Model**: Single-threaded WebAssembly execution loop within the browser event thread.
- **Garbage Collection**: Incremental GC enabled for managed C# objects.

```
+-------------------------------------------------------------+
|                     Browser Page Shell                      |
| (https://games.crazygames.com/en_US/god-mode-city-destroyer)|
+-------------------------------------------------------------+
                              |
                     [iframe postMessage]
                              v
+-------------------------------------------------------------+
|                    Unity Game Container                     |
|  - HTML5 Canvas (#unity-canvas)                             |
|  - Unity 6 Loader (WebGL.loader.js)                         |
|  - Audio Controller (GainNode muting / gesture recovery)    |
+-------------------------------------------------------------+
                              |
                   [WASM Instantiate / Memory]
                              v
+-------------------------------------------------------------+
|              Unity 6 Runtime Engine (PhysX 4.1.2)           |
|  - Universal Render Pipeline (URP 17.x)                     |
|  - New Input System (Mouse / Keyboard Events)               |
|  - Player Data (data.unity3d deserialization)               |
|  - Physics Scene & Simulation Step                          |
+-------------------------------------------------------------+
```

---

## 2. Rendering Pipeline Architecture

### 2.1 Universal Render Pipeline (URP) Configuration
The observed shader failures and context logs identify the rendering architecture as Unity's Universal Render Pipeline:
- **Renderer**: Forward+ / Single-Pass WebGL forward renderer.
- **Lighting Model**:
  - Main directional light (Sunlight) with real-time shadow mapping.
  - Ambient environment reflection probe.
  - Additional lights (heat vision, explosions, muzzle flashes) budgeted to low cluster count.
- **Dynamic Resolution & Quality Management**:
  - The runtime interrogates WebGL renderer strings (`ANGLE Intel UHD 630 Direct3D11`).
  - Automatically selects a quality tier (e.g. `Low`), enforcing `renderScale: 0.75` and `msaa: 1`.
  - Shader fallbacks ensure graceful degradation when compute or advanced vertex interpolators are unsupported.
- **Post-Processing Stack**:
  - Bloom (amplifies heat vision and shockwave glows).
  - Radial / Motion Blur (triggers during high-speed supersonic flight).
  - Camera Shake (driven via noise impulse on impacts and ground slams).

---

## 3. Physics & Simulation Architecture

### 3.1 PhysX Integration
- **Backend ID**: `0xf2b8ea05` (PhysX SDK 4.1.2).
- **Threading Model**: Single-threaded. In WebGL, PhysX simulation steps occur synchronously inside `FixedUpdate` (typically 50 Hz / 0.02s step).
- **Collision Matrix Optimization**:
  - Layer 1: Player (Character Controller or kinematic Rigidbody during flight).
  - Layer 2: Debris / Fragments (Active dynamic rigidbodies with simplified box/convex hull colliders).
  - Layer 3: Intact Structures (Static compound colliders).
  - Layer 4: Vehicles / Enemies (Dynamic ragdolls and wheeled/tracked physics actors).
  - Layer 5: Gravity Field / Area of Effect (Trigger volumes).

### 3.2 Performance Management on WebGL
- `isReadable: false` on static meshes (confirmed by mesh access warnings). Triangles are uploaded directly to GPU VRAM and discarded from system RAM to conserve the 32-bit WebAssembly heap limit.
- Debris pooling and lifetime pruning (destruction shards despawn or freeze to static colliders after coming to rest).

---

## 4. Input Architecture

- Powered by Unity's **New Input System** (`com.unity.inputsystem`).
- Key Input Mappings:
  - `Pointer / Delta`: Mouse pitch & yaw camera rotation.
  - `Move / Vector2`: W/A/S/D direction vectors relative to camera look angle.
  - `Ascend / Jump`: Space key for vertical flight ascent.
  - `Sprint / Boost`: Left Shift for supersonic flight acceleration.
  - `Primary / Secondary Action`: Left Mouse Button (LMB) & Right Mouse Button (RMB) for twin Heat Vision beams.
  - `Interact / Grab`: E key for physical object manipulation (Grab -> Throw / Crush).
  - `Special / Gravity Tether`: Q key for gravity beam attachment and hurling.
  - `Utility`: P (Pause), T (Skip Tutorial).

---

## 5. Memory Management & Boot Config

The runtime establishes pre-allocated memory arenas to prevent WebAssembly heap fragmentation:
- **Heap Allocator**: 16 MB chunk size for main, graphics, and worker threads.
- **Temporary Allocators**: 4 MB main temp arena, 2 MB job temp arena.
- **Profile & Typetree**: 2 MB allocations for reflection data.

---

## 6. Blueprint Guidelines for Clean-Room Unity Implementation

To achieve identical or superior performance in an original Unity game:
1. **Engine Target**: Unity 6 LTS (`6000.0.x`) or Unity 2022.3 LTS with URP.
2. **Physics**: Default PhysX with `Fixed Timestep = 0.02`, `Solver Iteration Count = 6`, `Solver Velocity Iterations = 2` to fit WebGL CPU budgets.
3. **Meshes**: Ensure `Read/Write Enabled` is unchecked on environment models, and use pre-fractured Voronoi mesh clusters for destructible buildings.
4. **Shaders**: Use lightweight URP Unlit/SimpleLit shaders with GPU instancing enabled for all city buildings, foliage, and vehicles.
