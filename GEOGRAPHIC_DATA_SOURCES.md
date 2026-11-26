# Geographic Data Sources & Attribution: Karaikal Open World

**Project**: CSVS GOD MODE — City of Chaos  
**Milestone**: V0.2.0 Open-World Foundation  
**Region**: Karaikal, Puducherry Union Territory, India  
**Date**: November 26, 2026  

---

## 1. Legal & Public Geographic Data Sources

In compliance with project guidelines, all geographic representations are derived from authorized, open, and legally usable datasets:

1. **OpenStreetMap (OSM)**:
   - Data Source: OpenStreetMap contributors (ODbL License).
   - Reference URL: [OpenStreetMap Karaikal, India](https://www.openstreetmap.org/search?query=Karaikal%2C%20India)
   - Usage: Road network geometry, arterial alignments (NH 32 / Kamarajar Salai, Bharathiyar Rd, Beach Rd, Thirunallar Rd), river course (Arasalar), coastline orientation, and major public landmark nodes.
   - Attribution Notice: *Map data © OpenStreetMap contributors, licensed under the Open Database License (ODbL).*

2. **D-Maps Open Vector Cartography**:
   - Source: Public domain / educational outline maps for Karaikal district geography and taluk boundaries.
   - Reference: [d-maps.com Karaikal District](https://d-maps.com/)
   - Usage: District boundary shape, canal courses, and inter-town relationships (Thirunallar, Neravy, TR Pattinam, Nedungadu).

3. **Public Elevation & Hydrographic References**:
   - Data Source: Public SRTM / CartoDEM terrain summaries.
   - Usage: Coastal elevation profile (flat coastal plain, 0m to 8m elevation above mean sea level), tidal river mouth characteristics.

---

## 2. Geographic Boundary & Scale Definitions

- **Geographic Bounding Box**:
  - Minimum Latitude: `10.8200° N` (Southern Port & TR Pattinam boundary)
  - Maximum Latitude: `10.9800° N` (Northern boundary toward Nedungadu)
  - Minimum Longitude: `79.7700° E` (Western boundary toward Thirunallar)
  - Maximum Longitude: `79.8550° E` (Eastern Bay of Bengal shoreline)
- **Local Simulation Space**:
  - Coordinate Origin `(0, 0, 0)`: Centered at the intersection of Kamarajar Salai and Bharathiyar Road (`10.9254° N, 79.8380° E`).
  - Active Playable Sector: $2,400\,\text{m} \times 2,400\,\text{m}$ spanning from $X = -1,200\,\text{m}$ to $+1,200\,\text{m}$, and $Z = -1,200\,\text{m}$ to $+1,200\,\text{m}$.

---

## 3. Distinction Between Real and Fictional Elements

| Feature | Category | Real Geographic Basis | Fictional Adaptation |
| :--- | :--- | :--- | :--- |
| **Arasalar River & Bridge** | River & Infrastructure | Real waterway dividing northern and southern Karaikal; real NH 32 bridge location. | Concrete piers and collision geometry optimized for vehicle driving and superhero flight. |
| **Karaikal Beach & Promenade** | Coastline & Tourism | Real beach on Bay of Bengal with public promenade and lighthouse park. | Extended paved viewing plazas for pedestrian sandbox interactions. |
| **Kamarajar Salai & Bharathiyar Rd** | Arterials | Real primary commercial avenues of Karaikal. | Procedural traffic node placement for AI circulation. |
| **Karaikal Ammaiyar Temple Zone** | Cultural Heritage | Celebrated historical Saivite temple dedicated to saint Karaikal Ammaiyar. | Protected cultural sanctuary zone with Dravidian architectural elements. |
| **Modern High-Rise District** | Fictional Expansion | None in real Karaikal (which consists primarily of low-to-mid-rise buildings). | Added on the northern coast as a high-altitude superhero combat arena with destructible glass skyscrapers. |
