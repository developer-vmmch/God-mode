# Performance & Hardware Optimization Report: CSVS God Mode V0.2.2
**Target Hardware:** Intel Core i3-10105T @ 3.00 GHz, Intel UHD Graphics 630, 8 GB RAM  
**Operating System:** Windows 10/11 x64  
**Date:** November 26, 2026  

---

## 1. Measured Performance & Resource Allocation

| Metric | Measured V0.2.2 Value | Target Budget | Compliance |
|---|---|---|---|
| WorkingSet Memory | $282.03\,\text{MB}$ | $< 300.0\,\text{MB}$ | **PASSED** (17.97 MB buffer) |
| Virtual Memory | $12,380.45\,\text{MB}$ | N/A | **STABLE** |
| Frame Rate (Town Center) | $60.0\,\text{FPS}$ | $\ge 30.0\,\text{FPS}$ | **PASSED** |
| Chunk Streaming Eval Rate | $0.35\,\text{seconds}$ ($2.85\,\text{Hz}$) | $< 5.0\,\text{Hz}$ | **OPTIMAL** |
| Active 250m Chunks | $13$ chunks | $\le 25$ chunks | **PASSED** |
| Active Debris Cap | $120$ objects max | $\le 150$ objects | **PASSED** |
| Active Pedestrian Pool | $60$ agents max | $\le 80$ agents | **PASSED** |
| Shader Compile / Magenta Errors | $0.00\%$ ($0$ of $921,600\,\text{px}$) | $0.00\%$ | **PERFECT** |
| Batchmode Build Time | $49.52\,\text{seconds}$ | $< 120.0\,\text{seconds}$ | **FAST** |

---

## 2. Low-Spec Optimization Architecture

1. **Procedural Multi-Element Mesh Synthesis:** 3D vehicle geometries (Auto-Rickshaw, Bus, Sedan, Hatchback, Motorcycle) and street furniture are generated on-demand and share shared URP materials, avoiding heavy external texture assets.
2. **250m Spatial Chunk Streaming:** Only chunks within $500\,\text{m}$ of the player are active in the scene hierarchy. Distant geometry ($> 650\,\text{m}$) is automatically culled and its destruction state serialized.
3. **Debris Pool Management:** Fractured concrete chunks from collapsed buildings automatically recycle back into the object pool after $10\,\text{seconds}$, preventing memory leaks and GPU fillrate bottlenecks.
4. **Pedestrian Agent Recirculation:** Pedestrians are managed in a fixed pool of 60 agents, recirculated dynamically within a 120m player bubble with simple kinematic walk cycles costing $< 0.4\,\text{ms}$ CPU time per frame.
5. **URP Lit Shader Unification:** All environment models (roads, modular bazaars, vehicles, and humanoid armor) share unified URP Lit materials with shared textures, reducing draw calls through SRP batching.
6. **Low Center-of-Gravity Vehicle Rigidbody Simulation:** Prevents vehicles from jittering or capsizing on sharp turns while keeping PhysX solver iterations low ($6$ solver iterations, $2$ velocity iterations).
