# Karaikal Map Integration Report: CSVS God Mode V0.2.0

**Project**: CSVS GOD MODE — City of Chaos  
**Milestone**: V0.2.0 — Karaikal Geographic Foundation & Indian Open-World Upgrade  
**Studio**: CSVS IT Tech  
**Setting**: Karaikal, Puducherry, India  
**Date**: November 26, 2026  
**Canonical Workspace**: `D:\Development\CSVS-GodMode\UnityProject`  

---

## 1. Executive Summary

This report documents the transformation of the CSVS God Mode prototype from a generic test grid into an open-world superhero environment set in the coastal enclave of **Karaikal, Puducherry, India**.

Inspired by the open-world depth, neighborhood identity, and cinematic street presentation of *GTA San Andreas*, *GTA V*, and *GTA VI*, the world geometry is now grounded in actual Karaikal geographic data (WGS84 10.9254° N, 79.8380° E), featuring the Bay of Bengal coastline, Arasalar River and Bridge, Kamarajar Salai (NH 32), Bharathiyar Road, and authentic South Indian modular architectural complexes.

---

## 2. GIS Coordinate Transformation & World Scale

The spatial coordinates are mapped from real-world WGS84 coordinates into Unity world space (meters) relative to the Karaikal Town Center origin:

$$\text{Local } X = (\text{Longitude} - 79.8380^\circ) \times 109,280\,\text{m/deg}$$
$$\text{Local } Z = (\text{Latitude} - 10.9254^\circ) \times 110,574\,\text{m/deg}$$

### Landmark Matrix & Coordinate Bindings

| Landmark Name (Tamil / English) | Geographic Coordinates | Unity World Position (X, Y, Z) | District Classification |
| :--- | :--- | :--- | :--- |
| **காரைக்கால் நகரம் / Karaikal Town Center** | 10.9254° N, 79.8380° E | `(0, 0, 0)` | District 1: Town Center |
| **காரைக்கால் கடற்கரை / Karaikal Beach & Promenade** | 10.9260° N, 79.8465° E | `(650, 0, 20)` | District 2: Beach Promenade |
| **அரசலாறு பாலம் / Arasalar River Bridge** | 10.9180° N, 79.8375° E | `(0, 2, -380)` | District 3: Riverfront |
| **காரைக்கால் அம்மையார் கோயில் / Ammaiyar Temple** | 10.9272° N, 79.8355° E | `(-220, 0, 180)` | District 4: Heritage & Temple Zone |
| **புதிய பேருந்து நிலையம் / Karaikal New Bus Stand** | 10.9310° N, 79.8340° E | `(-180, 0, 520)` | District 5: Transport Hub |
| **காரைக்கால் ரயில் நிலையம் / Railway Station** | 10.9325° N, 79.8280° E | `(-550, 0, 650)` | District 5: Transport Hub |
| **திருநள்ளாறு சாலை / Thirunallar Highway** | 10.9258° N, 79.8250° E | `(-750, 0, 50)` | District 6: Rural Outskirts |
| **காரைக்கால் துறைமுகம் / Karaikal Port & Logistics** | 10.8350° N, 79.8450° E | `(350, 0, -850)` | District 7: Port & Industrial |
| **நவீன கடற்கரை வளாகம் / Modern Expansion Zone** | 10.9320° N, 79.8440° E | `(450, 0, 650)` | District 8: Modern High-Rise |

---

## 3. Road & Water Infrastructure System

Implemented in `Assets/Scripts/Environment/KaraikalRoadNetwork.cs`:

1. **Kamarajar Salai (NH 32)**:
   - 18-meter wide 4-lane arterial road traversing North-South through the entire district.
   - Raised median, yellow/black warning curbs, concrete electric utility poles, and streetlights.
2. **Bharathiyar Road**:
   - 14-meter commercial street connecting Town Center Eastward directly to Beach Road.
   - Lined with bazaar shops, bilingual shop signboards, and tea stalls.
3. **Arasalar River & Bridge**:
   - 70-meter wide river channel cutting West to East into the Bay of Bengal at $Z = -380\,\text{m}$.
   - 85-meter concrete bridge structure with dual pedestrian sidewalks, reinforced crash railings, and riverbed support pillars.
4. **Karaikal Beach & Bay of Bengal**:
   - 350-meter wide golden sand dune strip along the eastern coast ($X = +550\,\text{m}$ to $+900\,\text{m}$).
   - Parallel 10-meter wide paved pedestrian promenade overlooking the ocean.
   - PBR-shaded water plane with specular highlights.

---

## 4. Modular South Indian Architectural System

Implemented in `Assets/Scripts/Environment/ModularIndianBuildingSystem.cs`:

- **Commercial Bazaar Complexes**: 3–4 stories, ground-floor shuttered storefronts with Tamil and English bilingual signboards ("காரைக்கால் அங்காடி", "அம்மா உணவகம்", "டீ கடை & பேக்கரி"), cantilevered concrete sunshades (chajjas), and pastel plaster finishes.
- **Residential Terrace Houses**: 1–2 stories, perimeter compound walls with iron gates, front verandahs (thinnai), terracotta tile roof trimmings, and rooftop Sintex black polymer water tanks on stilt slabs.
- **Traditional Heritage Architecture**: Limestone facades, ornamental Dravidian parapets, and pillared entrance mandapams.
- **Port Warehouses**: Corrugated metal sheds with stacked multi-color shipping containers (Red, Blue, Green, Yellow).
- **Modern High-Rise Towers**: 8–13 story curtain-wall reflective glass skyscrapers with rooftop helipads, situated in the coastal expansion district.

Every building is integrated with `DestructibleBuilding` and pre-fractured floor chunk meshes (`BuildingFractureChunk`), ensuring that Super Punch, Heat Vision, and Ground Slam abilities cause localized kinetic collapse.

---

## 5. GTA-Inspired UI & Minimap Integration

Implemented in `Assets/Scripts/UI/HUDController.cs`:
- **Real-Time GPS Minimap**: Located in the lower-left corner; displays player position, compass heading, road network lines, coastline, river, and landmark blips.
- **District Banner**: Dynamically detects player coordinates and announces the active district in Tamil and English (e.g., `"காரைக்கால் நகரம் \| KARAIKAL TOWN CENTER"`).
- **Full-Screen Map (`M` Key)**: Interactive full-screen map modal displaying the Karaikal district boundary, arterial highways, and landmark pins.
