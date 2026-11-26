# ASSET LICENSES & PROVENANCE REPORT — CSVS GOD MODE V0.2.2

## 1. Compliance Statement

**CSVS GOD MODE — CITY OF CHAOS** is an entirely original open-world superhero game developed by **CSVS IT Tech**.

No proprietary game assets, 3D meshes, copyrighted textures, ripped sound files, reverse-engineered binaries, or proprietary source code from any commercial title (including the *Grand Theft Auto* franchise or commercial superhero games) are present or utilized in this project.

---

## 2. Component Provenance Matrix

| Asset Component | Implementation Method | License / Origin | Proprietary Third-Party Dependencies |
| :--- | :--- | :--- | :--- |
| **C# Source Code** | 100% Original C# scripts authored for CSVS IT Tech | Proprietary / CSVS IT Tech All Rights Reserved | None |
| **3D Vehicle Models** | Procedural multi-element geometric meshes generated via `VehicleModelGenerator.cs` | Original Procedural Code (CSVS IT Tech) | None |
| **Character Meshes** | Procedural articulated humanoid superhero rig generated via `SuperheroHumanoidVisual.cs` | Original Procedural Code (CSVS IT Tech) | None |
| **Architectural Meshes** | Procedural modular South Indian building meshes generated via `CityGenerator.cs` | Original Procedural Code (CSVS IT Tech) | None |
| **Street Furniture** | Procedural concrete poles, transformers, streetlights, chai kadai via `StreetFurnitureSystem.cs` | Original Procedural Code (CSVS IT Tech) | None |
| **Pedestrian Models** | Procedural bipedal humanoid meshes with authentic South Indian attire via `KaraikalPedestrianSystem.cs` | Original Procedural Code (CSVS IT Tech) | None |
| **Shaders & Pipeline** | Universal Render Pipeline standard shaders (`URP/Lit`, `URP/Simple Lit`, `URP/Unlit`) | Unity Companion License / Unity Technologies | None |
| **Geographic Benchmarks**| Mathematical geographic projections from public domain WGS84 GPS benchmarks | Public Domain Geographic Standards | None |
| **Textures & Materials**| Procedurally generated diffuse, normal, and emissive textures created at runtime | Original Procedural Code (CSVS IT Tech) | None |

---

## 3. Reverse-Engineering Separation

The game was produced using independent, clean-room implementation practices. The REA v6.10.1 framework remains located in its external repository and was used exclusively for investigation and research. No code, binaries, or reverse-engineered assets from REA have been copied or linked into the standalone game build.
