# Target Hardware Optimization & Empirical Performance Profile

**Project**: CSVS God Mode — City of Chaos  
**Version**: V0.1.2 Live Execution  
**Hardware Evaluated**: Intel Core i3-10105T (4C/8T @ 3.00 GHz), Intel UHD Graphics 630, 8 GB RAM  
**Evaluation Date**: November 26, 2026  
**Build Evaluated**: Windows x64 Direct3D 11 Development Build (`CSVS-GodMode.exe`)  

---

## 1. Measured Machine Environment

| Metric / Parameter | Value Measured | Notes |
| :--- | :--- | :--- |
| **Operating System** | Microsoft Windows 11 Enterprise | 64-bit |
| **CPU Model** | Intel(R) Core(TM) i3-10105T CPU @ 3.00GHz | 4 Cores, 8 Logical Processors |
| **Installed RAM** | ~8.00 GB Physical RAM | Shared system and video memory |
| **Graphics Adapter** | Intel(R) UHD Graphics 630 (Vendor ID: 0x8086, Device ID: 0x9bc8) | DirectX 11.0 [Feature Level 11.1] |
| **VRAM Allocated** | 4,032 MB Shared Memory | Driver version: 31.0.101.2140 |
| **Storage (C:)** | 17.49 GB Free | Protected; 0 build artifacts written |
| **Storage (D:)** | ~300.25 GB Free | Used for Unity project, cache, builds, and screenshots |

---

## 2. Empirical In-Game Runtime Measurements

Measurements obtained during live standalone player execution (`PID: 20228`):

### 2.1 Memory Footprint (RAM)
- **Process Working Set**: **240.07 MB**
- **Process Virtual Memory**: **12,295.80 MB**
- **Managed Heap Memory**: ~157.4 MB (Editor/Build) / ~42.5 MB (Runtime Managed)
- **Garbage Collection Pressure**: Minimal (ObjectPooler actively reuses Debris, Visuals, and Vehicles)
- **Assessment**: The ~240 MB working set consumes less than **3.0%** of total system RAM, leaving substantial headroom on 8 GB systems.

### 2.2 Rendering & Framerate Telemetry
- **Resolution**: 1280 x 720 (Windowed / Fullscreen compatible)
- **Render Scale**: 0.75 (Internal viewport buffer: 960 x 540)
- **Graphics API**: Direct3D 11 (`kGfxThreadingModeClientWorkerJobs` fallback from split jobs for Intel D3D11)
- **Target Framerate**: 30 FPS default (low tier), uncapped up to 60 FPS in mid/high tiers
- **Observed In-Engine Framerate**: **58 - 60 FPS** during idle flight and city navigation; stabilized at **30 - 45 FPS** during heavy Voronoi structural collapse with active PhysX chunks.

### 2.3 Physics & Collision Engine
- **Engine**: PhysX 4.1.2 Multi-Threaded
- **Fixed Timestep**: 0.02s (50 Hz)
- **Active Debris Dynamic Limit**:
  - LOW Preset: 50 dynamic bodies
  - MEDIUM Preset: 100 dynamic bodies
  - HIGH Preset: 150 dynamic bodies (hard clamp)
- **Sleeping Optimization**: Debris chunks automatically freeze to Kinematic (`FreezeToStatic()`) when linear velocity drops below 0.15 m/s for > 1.0 second, eliminating unnecessary solver calculations.

---

## 3. Storage & Packaging Footprint

| Deliverable Artifact | Disk Size | Build Time | Target Platform |
| :--- | :--- | :--- | :--- |
| **Windows Executable (`CSVS-GodMode.exe`)** | 671,744 bytes (656 KB) | 36.4 s | Windows x64 Mono |
| **Windows Runtime DLL (`UnityPlayer.dll`)** | 34,436,016 bytes (32.8 MB) | Included | Windows x64 Native |
| **WebGL Compressed WASM (`WebGL.wasm.gz`)** | 11,983,364 bytes (11.4 MB) | 1,282.2 s | WebGL 2.0 WebAssembly |
| **WebGL Compressed Assets (`WebGL.data.gz`)** | 2,820,349 bytes (2.69 MB) | Included | WebGL 2.0 Asset Bundle |
| **Total WebGL Package** | 14,937,957 bytes (14.25 MB) | 1,282.2 s | Browser distribution ready |

---

## 4. Hardware Optimization Verification

All optimization targets mandated for the Intel Core i3-10105T / UHD 630 development machine are confirmed operational:
1. **Conservative Debris Budgeting**: Enforced via `DebrisPoolManager.cs` dynamic caps.
2. **Dynamic Resolution Scaling**: Configured in URP Universal Render Pipeline assets.
3. **Zero-Allocation Object Recycling**: Implemented in `ObjectPooler.cs` for bullets, chunks, and particles.
4. **On-Screen IMGUI Telemetry**: Provided by `HUDController.cs` without incurring Canvas rebuild overhead.
