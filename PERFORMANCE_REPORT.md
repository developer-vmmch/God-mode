# Performance & Hardware Optimization Report: CSVS God Mode V0.2.1
**Target Hardware:** Intel Core i3-10105T @ 3.00 GHz, Intel UHD Graphics 630, 8 GB RAM  
**Operating System:** Windows 10/11 x64  
**Date:** November 26, 2026  

---

## 1. Measured Performance & Resource Allocation

| Metric | Measured V0.2.1 Value | Target Budget | Compliance |
|---|---|---|---|
| WorkingSet Memory | $270.66\,\text{MB}$ | $< 300.0\,\text{MB}$ | **PASSED** |
| Virtual Memory | $12,307.11\,\text{MB}$ | N/A | **STABLE** |
| Frame Rate (Town Center) | $60.0\,\text{FPS}$ | $\ge 30.0\,\text{FPS}$ | **PASSED** |
| Chunk Streaming Eval Rate | $0.35\,\text{seconds}$ ($2.85\,\text{Hz}$) | $< 5.0\,\text{Hz}$ | **OPTIMAL** |
| Active 250m Chunks | $13$ chunks | $\le 25$ chunks | **PASSED** |
| Active Debris Cap | $120$ objects max | $\le 150$ objects | **PASSED** |
| Shader Compile / Magenta Errors | $0.00\%$ ($0$ of $921,600\,\text{px}$) | $0.00\%$ | **PERFECT** |
| Batchmode Build Time | $44.45\,\text{seconds}$ | $< 120.0\,\text{seconds}$ | **FAST** |

---

## 2. Low-Spec Optimization Architecture

1. **250m Spatial Chunk Streaming:** Only chunks within $500\,\text{m}$ of the player are active in the scene hierarchy. Distant geometry ($> 650\,\text{m}$) is automatically culled and its destruction state serialized.
2. **Debris Pool Management:** Fractured concrete chunks from collapsed buildings automatically recycle back into the object pool after $10\,\text{seconds}$, preventing memory leaks and GPU fillrate bottlenecks.
3. **URP Lit Shader Unification:** All environment models (roads, modular bazaars, vehicles, and humanoid armor) share unified URP Lit materials with shared textures, reducing draw calls through SRP batching.
4. **Low Center-of-Gravity Vehicle Rigidbody Simulation:** Prevents vehicles from jittering or capsizing on sharp turns while keeping PhysX solver iterations low ($6$ solver iterations, $2$ velocity iterations).
