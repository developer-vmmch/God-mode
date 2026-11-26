# Building Asset Inventory & Architectural Catalog

**Project**: CSVS GOD MODE — City of Chaos  
**Milestone**: V0.2.0 Modular Architecture  
**Render Pipeline**: Universal Render Pipeline (URP 17.0.3)  
**Date**: November 26, 2026  

---

## 1. Architectural Categories & Construction Rules

All buildings in the Karaikal open world are procedurally assembled via `ModularIndianBuildingSystem.cs` using optimized PBR-compatible URP Lit materials:

| Category | Typical Dimensions (W x H x D) | Floor Count | Defining Features | Material Palette |
| :--- | :--- | :--- | :--- | :--- |
| **Commercial Bazaar Complex** | $16\,\text{m} \times (11-15)\,\text{m} \times 16\,\text{m}$ | 3–4 Floors | Ground-floor retail shutters, Tamil/English signboards, concrete sunshades (chajjas), rooftop stairwell headroom shed, black Sintex water tank. | Cream/Pastel Plaster (`#E0C8A6`, `#D98C4C`), Concrete (`#B2B8BC`), Signboard Red/Yellow (`#D93326`). |
| **South Indian Residential House** | $14\,\text{m} \times (7-11)\,\text{m} \times 14\,\text{m}$ | 1–2 Floors | Perimeter compound wall ($1.8\,\text{m}$ high) with entrance gate, front verandah (thinnai), terracotta roof trimmings, rooftop water tank on stilt slab. | Pastel Mint/Ochre (`#73B89E`, `#D98C4C`), Terracotta Tile (`#C05838`), Concrete Wall (`#B2B8BC`). |
| **Traditional Heritage Structure** | $16\,\text{m} \times 10\,\text{m} \times 16\,\text{m}$ | 2 Floors | Limestone walls, ornamental Dravidian parapet trimmings, cylindrical front mandapam pillars. | Heritage Limestone (`#D9C799`), Terracotta Tile (`#C05838`), Polished Pillar Granite (`#B2B8BC`). |
| **Port Industrial Warehouse** | $24\,\text{m} \times 9\,\text{m} \times 20\,\text{m}$ | 1–2 Floors (High Bay) | Corrugated metal siding, loading docks, stacked 20ft/40ft shipping containers. | Industrial Metal (`#737A85`), Cargo Red (`#BF2626`), Cargo Blue (`#2659BF`), Cargo Green (`#33994D`). |
| **Modern Coastal High-Rise Tower** | $22\,\text{m} \times (32-52)\,\text{m} \times 22\,\text{m}$ | 8–13 Floors | Curtain-wall reflective glass, structural concrete floor slabs, rooftop helipad. | Reflective Glass (`#3373A6`), Architectural Concrete (`#B2B8BC`). |

---

## 2. PBR Material & Shader Configuration

All materials utilize `Universal Render Pipeline/Lit` to ensure hardware compatibility with the Intel UHD 630 GPU:
1. `_concreteMaterial`: Roughness/Smoothness: `0.25`, BaseColor: `(0.70, 0.72, 0.74)`
2. `_plasterPastelMaterial`: Roughness/Smoothness: `0.35`, BaseColor: Dynamic South Indian pastel hues
3. `_terracottaMaterial`: Roughness/Smoothness: `0.15`, BaseColor: `(0.75, 0.35, 0.22)`
4. `_glassMaterial`: Roughness/Smoothness: `0.85`, BaseColor: `(0.20, 0.45, 0.65)`
5. `_industrialMetalMaterial`: Roughness/Smoothness: `0.45`, BaseColor: `(0.45, 0.48, 0.52)`
6. `_waterTankMaterial`: Roughness/Smoothness: `0.60`, BaseColor: `(0.12, 0.14, 0.16)` (Black Sintex polymer)
7. `_signboardMaterial`: Roughness/Smoothness: `0.50`, BaseColor: `(0.85, 0.20, 0.15)`

---

## 3. Destruction Engine Integration

Every generated building automatically configures:
- **`BoxCollider`**: Provides collision for the intact building body.
- **`IntactModel`**: The active visible visual mesh while undamaged.
- **`FractureRoot`**: Container holding pre-sliced floor chunks (`BuildingFractureChunk`).
- **`DestructibleBuilding`**: Monitors structural health, handles localized damage from Super Punch (450 HP) and Heat Vision (150 HP/s), and transfers chunks to `DebrisPoolManager` upon structural fracture.
