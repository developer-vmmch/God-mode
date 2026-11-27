# ASSET PIPELINE REPORT — CSVS GOD MODE V0.3

## 1. Pipeline Architecture Overview

CSVS GOD MODE V0.3 uses a **procedural runtime asset generation** pipeline, since Blender is not installed on the development machine. All 3D geometry is authored in C# and generated at game initialization.

> [!NOTE]
> This approach is deliberate for the low-spec hardware target (Intel UHD 630). Procedural generation avoids large binary asset files and allows real-time LOD/quality scaling.

---

## 2. Component Inventory

| Asset Category | Implementation | Script | License |
| :--- | :--- | :--- | :--- |
| **Road meshes** | Procedural poly-strip extrusion from OSM polylines | `KaraikalRoadMeshBuilder.cs` | Original code |
| **Building meshes** | Modular South Indian architecture primitives | `ModularIndianBuildingSystem.cs` | Original code |
| **Vehicle bodies** | Multi-element parametric 3D geometry | `VehicleModelGenerator.cs` | Original code |
| **Superhero body** | 20-bone procedural articulated humanoid | `ProceduralHumanoidBody.cs` | Original code |
| **Civilian body** | Lightweight procedural biped | `KaraikalPedestrianSystem.cs` | Original code |
| **Coconut palms** | Layered capsule trunk + sphere frond clusters | `KaraikalCoastalEnvironment.cs` | Original code |
| **Ocean surface** | 32×32 animated vertex wave mesh | `KaraikalCoastalEnvironment.cs` | Original code |
| **Street furniture** | Utility poles, chai stalls, bus shelters | `StreetFurnitureSystem.cs` | Original code |
| **URP materials** | Procedurally configured `URP/Lit` and `URP/Unlit` | All generator scripts | Unity Companion License |

---

## 3. Blender Availability Status

**Blender is NOT installed** on this machine (searched `C:\Program Files\Blender Foundation` and `D:\` — not found).

### Impact
- Mesh quality: Characters and vehicles are built from primitive shapes (cube, capsule, sphere, cylinder) with surface detail encoded via material color, metallic, and smoothness properties.
- UV mapping: Procedurally assigned at vertex level (no handcrafted UV islands).
- Normal maps: Not applied (no Blender-generated normal map bakes).
- Rigging: Procedural Transform-hierarchy bones (not Unity Humanoid avatar / SkinnedMeshRenderer).

### Decision Required
To achieve photorealistic mesh quality, Blender must be installed and `.fbx` export → Unity import workflow established. This is recommended for **V0.4** after user confirms approval.

---

## 4. Future Professional Asset Roadmap

| Phase | Action | Requirement |
| :--- | :--- | :--- |
| V0.4-A | Install Blender 3.6+ LTS | User approval needed |
| V0.4-B | Author Auto-Rickshaw `.blend` with proper poly mesh | Blender |
| V0.4-C | Bake normal, roughness, AO maps | Blender Cycles |
| V0.4-D | Export to `.fbx` → import to Unity | Blender + Unity |
| V0.4-E | License PBR textures from ambientCG / PolyHaven | Free CCO license |
| V0.4-F | Build HDRI skybox for coastal atmosphere | PolyHaven HDRIs (CCO) |

---

## 5. Legally Licensed External Assets (Future Use Only)

The following free-to-use asset sources are identified for future import. **None are imported yet.**

| Source | License | Planned Use |
| :--- | :--- | :--- |
| [ambientCG.com](https://ambientcg.com) | CC0 1.0 (Public Domain) | PBR asphalt, concrete, sand textures |
| [polyhaven.com](https://polyhaven.com) | CC0 1.0 (Public Domain) | HDRIs, tropical vegetation |
| [kenney.nl](https://kenney.nl) | CC0 1.0 (Public Domain) | UI icons, placeholder assets |
| Unity Asset Store — Free tier | Unity EULA | Audio clips, shaders |

No assets from these sources have been imported in V0.3. All geometry and materials are original CSVS IT Tech code.

