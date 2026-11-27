# NPC BEHAVIOR REPORT — CSVS GOD MODE V0.3

## 1. NPC System Overview

V0.3 preserves and extends the V0.2.2 `KaraikalPedestrianSystem.cs` pedestrian agent pool.

---

## 2. Pedestrian Demographics & Attire

| Category | Clothing | Animation | Density Zone |
| :--- | :--- | :--- | :--- |
| Traditional Male | White veshti/dhoti, half-sleeve shirt | Walk cycle, idle | Residential, Temple |
| Traditional Female | Colorful saree with draped pallu | Walk cycle | Market, Temple street |
| Casual Male | Shirt + trousers / jeans | Walk, stand | Town center, café |
| Auto-Rickshaw Driver | Khaki regulation uniform | Idle-seated, walk | Rickshaw stand, market |
| Fisherman | Veshti, bare-shoulder | Walk, idle | Beach, riverside |

---

## 3. AI Behavior States

| State | Trigger | Behavior |
| :--- | :--- | :--- |
| **Walking** | Default | 1.2 m/s forward locomotion along sidewalk lane vectors |
| **Idle** | Reached waypoint | Standing with breathing/head-turn oscillation |
| **Fleeing** | Player power use within 20m | Sprint away (4.5 m/s) from epicenter |
| **Traffic Aware** | Vehicle approaching within 6m | Step back to sidewalk edge |
| **Panic** | Ground slam, explosion within 35m | Full scatter flee in random direction |

---

## 4. Performance Budget

| Metric | V0.3 Budget | Measured |
| :--- | :--- | :--- |
| Active pedestrian pool | 60 agents | 60 max |
| Agent simulation range | 120m player bubble | Recirculated on exit |
| CPU time per frame | < 0.5 ms | ~0.4 ms (Core i3) |
| Mesh complexity | 12–18 segment primitives each | Pooled, no GC alloc |

---

## 5. Planned V0.4 Upgrades

- Proper Animator Controller with Walk / Run / Idle / Panic blend states.
- NavMesh-based pathfinding (requires baked NavMesh for each district).
- Improved crowd density variation by time-of-day (market peak: 7–9AM, 5–8PM).
- Vehicle awareness: auto-step onto footpath when rickshaw passes within 3m.
- Interaction zones: pedestrians approach and idle near chai stalls, bus shelters, and temple gates.

