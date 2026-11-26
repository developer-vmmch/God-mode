# CSVS GOD MODE — ROAD NETWORK & TRAFFIC REPORT
**Version:** V0.2.1  
**Studio:** CSVS IT Tech  
**Setting:** Karaikal, Puducherry, India  
**Date:** November 26, 2026  

---

## 1. Road Hierarchy & GIS Alignment

The road network in `KaraikalRoadNetwork.cs` models the genuine arterial corridors of Karaikal, structured according to Indian Road Congress (IRC) urban geometric guidelines and Left-Hand Traffic (LHT) rules:

### 1.1 Arterial Corridors
1. **Kamarajar Salai (National Highway 32):**
   - **Orientation:** North-South through origin ($X = 0\,\text{m}$, extending from $Z = -1200\,\text{m}$ to $Z = +1200\,\text{m}$).
   - **Width:** 16.0 meters (4 lanes, 2 lanes per direction with asphalt texturing and painted white dashed dividers).
   - **Crossings:** Crosses the Arasalar River Bridge at $Z = -818.5\,\text{m}$.
2. **Bharathiyar Road:**
   - **Orientation:** East-West through origin ($Z = 0\,\text{m}$, extending from $X = -1200\,\text{m}$ to $X = +930\,\text{m}$).
   - **Width:** 14.0 meters (dual carriageway connecting Thirunallar Road to Karaikal Beach).
3. **Karaikal Beach Road:**
   - **Orientation:** North-South coastal promenade along the Bay of Bengal ($X = +930\,\text{m}$, extending from $Z = -400\,\text{m}$ to $Z = +600\,\text{m}$).
   - **Width:** 12.0 meters, flanked by pedestrian sidewalks, coconut trees, and sea-view benches.
4. **Church Street & Temple Ring Roads:**
   - **Orientation:** Commercial grid perimeter roads ($X = \pm 150\,\text{m}, Z = \pm 150\,\text{m}$).
   - **Width:** 10.0 meters, serving dense bazaars, shopfronts, and heritage structures.
5. **Thirunallar Arterial Highway:**
   - **Orientation:** Heading West-Northwest ($X < -600\,\text{m}$) connecting to the pilgrimage temple town.

---

## 2. Left-Hand Traffic (LHT) & Intersection Design

- **Driving Direction:** Strict Left-Hand Traffic (drive on the left, pass on the right).
- **Lane Offsets:**
  - Forward Lane: $-3.5\,\text{m}$ lateral offset from road centerline.
  - Reverse Lane: $+3.5\,\text{m}$ lateral offset from road centerline.
- **Intersections:** Flat level-grade junctions with pedestrian zebra crossings painted in high-contrast matte white.
- **Speed Limits:**
  - Urban Town Center: $40\,\text{km/h}$ ($11.1\,\text{m/s}$).
  - Kamarajar Salai (NH 32): $60\,\text{km/h}$ ($16.6\,\text{m/s}$).
  - Beach Promenade: $30\,\text{km/h}$ ($8.3\,\text{m/s}$).
