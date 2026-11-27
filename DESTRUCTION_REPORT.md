# DESTRUCTION SYSTEM REPORT — CSVS GOD MODE V0.3

## 1. Existing Destruction Architecture (Preserved from V0.2.2)

The building destruction system uses pre-fracture chunk activation:

| Component | File | Function |
| :--- | :--- | :--- |
| `DestructibleBuilding` | `Scripts/Destruction/DestructibleBuilding.cs` | Damage tracking, fracture trigger |
| `BuildingFractureChunk` | `Scripts/Destruction/BuildingFractureChunk.cs` | Physics chunk behaviour |
| `DebrisPoolManager` | `Scripts/Destruction/DebrisPoolManager.cs` | Debris recycling pool |

---

## 2. V0.3 Destruction Enhancements

### Real-World Landmark Protection Policy
- **13 real-world landmarks** (Karaikal Ammaiyar Temple, Church of Our Lady of Angels, Railway Station, etc.) are marked `IsRealWorldSite = true` in `KaraikalOSMRoadData.Landmarks`.
- These buildings are `DestructibleBuilding.SetIndestructible(true)` by default.
- Only fictional gameplay buildings (Chaos Arena, CSVS Industrial Complex, procedural commercial blocks) are destructible.

### Chaos Rating Integration
- Each building destruction event calls `ReputationChaosSystem.RecordBuildingDamage(fraction)`.
- Vehicle destruction calls `RecordVehicleDamage(fraction)`.
- Chaos accumulates and escalates the threat response system.

### Destruction Performance Budget

| Metric | V0.3 Budget |
| :--- | :--- |
| Simultaneous active Rigidbodies (chunks) | ≤ 50 at LOW quality |
| Simultaneous active Rigidbodies (chunks) | ≤ 150 at HIGH quality |
| Debris pool recycle time | 10 seconds |
| Chunk sleep distance | > 30m from player |

---

## 3. V0.4 Planned Improvements

- **Breakable windows**: Separate window panel mesh per building that shatters on impact.
- **Structural support graph**: Connected support columns — destroying a column weakens upper floors.
- **Exposed interiors**: Buildings with interior room geometry visible after wall destruction.
- **Progressive collapse**: Floor-by-floor sequential failure when structural support removed.
- **Scorch marks**: Decal projector applying heat vision burn decals on surfaces.
- **Smoke particles**: Lingering smoke from destroyed buildings (pooled particle system).

