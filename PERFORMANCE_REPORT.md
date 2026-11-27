# PERFORMANCE REPORT — CSVS GOD MODE V0.3

## 1. Hardware Target Profile

| Component | Specification | Setting Target |
| :--- | :--- | :--- |
| CPU | Intel Core i3-10105T (4C/8T, 3.0–3.9 GHz) | Dedicated AI & physics threads |
| GPU | Intel UHD Graphics 630 (GT2, 24 EU, 1.1 GHz, 23 GB/s bandwidth) | LOW setting, 720p |
| RAM | 8 GB DDR4 2666 MHz | ≤ 400 MB working set (expanded from V0.2.2 300 MB) |
| Storage | SATA SSD | Async streaming ≤ 2s per chunk |
| OS | Windows 10/11 x64 | DirectX 11 |

> [!NOTE]
> The V0.2.2 memory budget of 300 MB has been revised to **≤ 400 MB** in V0.3. The additional V0.3 systems (road mesh builder, coastal environment, mission system, improved camera) add approximately 25–40 MB overhead. This remains well within safe operating bounds for 8 GB RAM.

---

## 2. Quality Preset Definitions

### LOW (Target: i3 / UHD 630 / 8 GB)
- Render scale: 0.70 (896×504 internal, upscaled to 1280×720)
- Target FPS: 30 FPS (VSync disabled)
- Shadow Distance: 40m, 1 cascade
- Pedestrians: 20 active agents
- Traffic: 8 vehicles
- Destruction bodies: 50 max simultaneous
- Water: Static ocean (no vertex animation)
- Post-processing: Minimal (no bloom, no DOF)
- LOD bias: 0.5

### MEDIUM (Target: i5 / GTX 1050 / 8 GB)
- Render scale: 0.85
- Target FPS: 60 FPS
- Shadow Distance: 80m, 2 cascades
- Pedestrians: 40 agents
- Traffic: 16 vehicles
- Destruction bodies: 100 max
- Water: Animated ocean (wave vertex simulation)
- Post-processing: Bloom, ambient occlusion
- LOD bias: 1.0

### HIGH (Target: i7 / RTX 2060 / 16 GB)
- Render scale: 1.0 (native 1920×1080)
- Target FPS: 60 FPS
- Shadow Distance: 150m, 4 cascades
- Pedestrians: 60 agents
- Traffic: 24 vehicles
- Destruction bodies: 150 max
- Water: Full Gerstner wave animation
- Post-processing: Full suite
- LOD bias: 1.5

---

## 3. V0.3 Optimization Systems

| Optimization | Implementation | Status |
| :--- | :--- | :--- |
| 250m Chunk Streaming | `KaraikalWorldStreamer.cs` | ACTIVE from V0.2.1 |
| Object Pooling | `ObjectPooler.cs`, `DebrisPoolManager.cs` | ACTIVE from V0.1 |
| Pedestrian Recirculation | 120m player bubble | ACTIVE from V0.2.2 |
| Road Mesh Coroutine Spreading | 1 segment/frame async generation | NEW in V0.3 |
| Physics Sleeping | Rigidbody.Sleep() on chunks > 30m | ACTIVE |
| SRP Batching | Shared URP materials on road/building meshes | ALWAYS ON |
| Distance-based AI | Pedestrians disable animation > 80m | ACTIVE |

---

## 4. Memory Budget Estimation (V0.3)

| System | Estimated RAM |
| :--- | :--- |
| Unity Runtime | ~80 MB |
| URP Pipeline & Shaders | ~35 MB |
| Road meshes (10 segments) | ~8 MB |
| Building geometry (streamed) | ~25 MB active |
| Pedestrian pool (60 agents) | ~12 MB |
| Traffic pool (16 vehicles) | ~10 MB |
| Ocean mesh + particles | ~15 MB |
| Mission & audio systems | ~6 MB |
| Destruction pool (50 chunks) | ~18 MB |
| Textures (procedural) | ~20 MB |
| **Total Estimate** | **~229–280 MB** |
| **Budget with overhead** | **≤ 400 MB** |
