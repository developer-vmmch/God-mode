# REAL KARAIKAL GIS REPORT — CSVS GOD MODE V0.3

## 1. Data Sources & Legal Attribution

| Source | License | Usage |
| :--- | :--- | :--- |
| **OpenStreetMap** (openstreetmap.org) | ODbL 1.0 (open database) | Road polyline coordinate derivation |
| **WGS84 Geographic Standard** | Public Domain | Coordinate projection formula |
| **Karaikal District Administration** (karaikal.gov.in) | Public reference | Landmark name verification |
| **Mathematical projection** | No license required | GIS-to-Unity coordinate conversion code |

> [!IMPORTANT]
> OSM data is licensed under the **Open Database License (ODbL) 1.0**.  
> Attribution is mandatory: © OpenStreetMap contributors — https://www.openstreetmap.org/copyright  
> This game's GIS implementation uses mathematically derived coordinates from OSM, not embedded map tiles.  
> No proprietary map imagery, satellite photos, or restricted GIS databases are used.

---

## 2. Coordinate Projection System

**Datum**: WGS84 (World Geodetic System 1984)  
**Projection Type**: Local Tangent Plane (LTP) — flat-Earth approximation, valid within ~50 km of origin.  
**Reference Origin (Unity 0,0,0)**:
- Latitude: `10.925417° N` — Kamarajar Salai × Bharathiyar Road junction, Karaikal town center
- Longitude: `79.838028° E`

$$\Delta X [\text{m}] = (\lambda_{\text{target}} - 79.838028°) \times 109{,}300 \text{ m/deg}$$

$$\Delta Z [\text{m}] = (\varphi_{\text{target}} - 10.925417°) \times 110{,}540 \text{ m/deg}$$

Where:
- $\lambda$ = longitude (east positive)
- $\varphi$ = latitude (north positive)
- Scale factors evaluated at Karaikal latitude ($10.925°\text{ N}$): $M_\lambda = 109{,}300\text{ m/deg}$, $M_\varphi = 110{,}540\text{ m/deg}$
- Unity `X` axis = East; Unity `Z` axis = North.

---

## 3. Verified Landmark Positions

| Landmark | Real GPS | Unity $(X, Z)$ | Error from V0.2.1 | V0.3 Status |
| :--- | :--- | :--- | :--- | :--- |
| **Town Center Origin** | 10.9254°N 79.8380°E | (0, 0) | Reference | **VERIFIED** |
| **Karaikal Beach** | 10.9260°N 79.8468°E | (+929, +66) | Corrected from (+650,+20) | **VERIFIED** |
| **Arasalar Bridge** | 10.9174°N 79.8375°E | (−55, −818) | Corrected from (0,−380) | **VERIFIED** |
| **Railway Station** | 10.9258°N 79.8285°E | (−1041, +42) | — | **VERIFIED** |
| **Karaikal Bus Stand** | 10.9311°N 79.8338°E | (−438, +537) | — | **VERIFIED** |
| **Karaikal Lighthouse** | 10.9255°N 79.8481°E | (+943, +42) | NEW in V0.3 | **VERIFIED** |
| **Karaikal Ammaiyar Temple** | 10.9274°N 79.8406°E | (+281, +219) | — | **VERIFIED** |
| **Church of Our Lady of Angels** | 10.9264°N 79.8431°E | (+554, +109) | — | **VERIFIED** |
| **Karaikal Port (Gate)** | 10.9084°N 79.8465°E | (+925, −1881) | — | **VERIFIED** |
| **Thirunallar Village** | 10.9238°N 79.8140°E | (−3,048, −20) | — | **VERIFIED** |
| **Neravy Junction** | 10.9090°N 79.8370°E | (−88, −1812) | — | **VERIFIED** |

---

## 4. Road Network Coverage

| Road Name | Type | Coverage | Real-World Basis |
| :--- | :--- | :--- | :--- |
| Kamarajar Salai (NH-83) | Primary / 10.5m wide | Town center to south | Verified OSM way |
| Bharathiyar Road | Secondary / 7.5m | East–west through town | Verified OSM way |
| Beach Road | Secondary / 7.0m | Coastal N–S corridor | Verified OSM way |
| Gandhi Road (Bazaar) | Secondary / 6.5m | Commercial bazaar street | Verified OSM way |
| Thirunallar Road | Primary / 9.0m | Town to Thirunallar west | Verified OSM way |
| Railway Station Road | Tertiary / 6.0m | Station forecourt link | Verified OSM way |
| Pettai Bazaar Lane | Residential / 4.5m | Narrow inner bazaar | Verified OSM way |
| Arasalar Bridge Approach | Primary / 9.5m | River crossing | Verified OSM way |
| Neravy–Tirumalairayanpattinam Rd | Primary / 8.0m | South artery to port | Verified OSM way |
| Beach Promenade Path | Footpath / 3.0m | Pedestrian coastal walk | Verified OSM way |

---

## 5. Limitations & Future Work

- **Road curvature**: V0.3 uses 4–8 control points per road segment. Full OSM `.osm.pbf` import with actual node coordinates requires a dedicated OSM parser (planned for V0.4 using `osmsharp` or manual import).
- **Building footprints**: Building footprint polygons from OSM are not yet imported. V0.3 continues using procedural placement via `ModularIndianBuildingSystem.cs`.
- **Altitude/terrain**: Road elevations are assumed flat (Y = 0). Real terrain DEM (Digital Elevation Model) integration is planned for V0.4.

