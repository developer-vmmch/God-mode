# CSVS GOD MODE — CITY OF CHAOS

[![Unity](https://img.shields.io/badge/Unity-6000.0.84f1_6_LTS-blue.svg)](https://unity.com/)
[![Render Pipeline](https://img.shields.io/badge/Render_Pipeline-URP_17.0.3-green.svg)](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@17.0/manual/index.html)
[![Blender](https://img.shields.io/badge/Blender-4.5.16_LTS-orange.svg)](https://www.blender.org/)
[![Git LFS](https://img.shields.io/badge/Git_LFS-Enabled-brightgreen.svg)](https://git-lfs.github.com/)
[![License](https://img.shields.io/badge/License-Proprietary-red.svg)](LICENSE)
[![Version](https://img.shields.io/badge/Version-v0.3.0--alpha-purple.svg)]()

> **An original, high-octane Indian open-world superhero action & environmental destruction sandbox set in the coastal city of Karaikal, Puducherry.**

Developed by **CSVS IT Tech**  
Lead Architect & Developer: **Martin Lourduraj**  
Target Platforms: **Windows x64**, **WebGL 2.0**, **Linux (planned)**

---

## 📖 1. Overview & Vision

**CSVS GOD MODE — CITY OF CHAOS** combines the god-tier superpower locomotion and physics-driven environmental destruction of superhero sandboxes with the living, grounded atmosphere of coastal South India. 

Set within a geographically faithful recreation of **Karaikal, Puducherry**, players command an armored superhuman entity capable of supersonic flight, kinetic shockwaves, telekinesis, and high-impact structural demolition. Whether flying along the Bay of Bengal coastline, weaving through bustling bazaar streets in a customized auto-rickshaw, or collapsing monolithic towers in intense police battles, the game delivers AAA-inspired visual fidelity, robust vehicle dynamics, and emergent open-world chaos.

---

## 🗺️ 2. Setting: Karaikal, Puducherry, India

Karaikal is a historic coastal port municipality in the Union Territory of Puducherry, surrounded by Tamil Nadu and bounded on the east by the Bay of Bengal. The game captures its unique blend of French-colonial coastal heritage and traditional Dravidian South Indian culture:

- **Arasalar River & Delta:** Bisects the municipality, featuring functional arch bridges, fishing boats, and mangrove riverbanks.
- **Karaikal Beach & Promenade:** Expansive sandy coastline along the Bay of Bengal with seafront promenade pathways, lighthouse towers, and seaside cafes.
- **Karaikal Ammiyar Temple & Bazaar Quarter:** Intricate Dravidian multi-tiered gopurams, brass lamps, and dense street-level bazaars with street vendors and textile shops.
- **French Heritage French Quarter:** Colonial pastel-walled villas with louvered wooden windows, terracotta-tiled hip roofs, compound walls, and wrought-iron gates.
- **Port Highway & Industrial Corridor (NH32 / ECR):** Four-lane transit corridor linking the city core to the deep-water Karaikal Port complex, tank farms, and shipping container yards.
- **Thirunallar Arterial Road:** Bustling inland pilgrimage route flanked by commercial shophouses, shade trees, and transit stops.

---

## ⚡ 3. Core Gameplay Systems

### 🦸 Superhuman Locomotion
- **Supersonic Flight:** Seamless transition between grounded running and three-dimensional free flight with dynamic FOV scaling, air-friction effects, and sonic boom particle shockwaves (`Left Shift` boost).
- **Air Dash & Hover:** Instant vector repositioning (`F` to toggle flight / hover; `Space` / `Ctrl` for vertical thrust).
- **Kinetic Ground Slam:** Plunge downward at terminal velocity from flight to release a devastating radial shockwave that launches nearby vehicles and fractures asphalt.

### 💥 Physics-Driven Structural Destruction
- **Voronoi Mesh Demolition:** Procedural stress fractures that dynamically segment buildings into physics-active rigidbodies upon weapon or superhero impact.
- **Progressive Collapse:** Monolithic high-rises and commercial complexes deform based on load-bearing integrity; lower-story damage cascades into catastrophic collapses.
- **Interactive Debris:** Every shattered chunk of concrete, metal beam, and brick fragment interacts with physics gravity, hero abilities, and pedestrian AI.

### 🚗 Authentic Indian Vehicles & Traffic Simulation
- **Auto-Rickshaw (3-Wheeled Autorickshaw):** Authentic rear-engine torque physics, responsive drift steering, open canvas sides, and fare-meter interior.
- **Classic 350cc Motorcycle:** Single-cylinder low-rpm audio pulse, dual shock suspension, and authentic low-center-of-mass handling.
- **Ambassador Commercial Taxi:** Classic monocoque sedan with robust body roll, chrome bumpers, and distinctive yellow/black livery.
- **Transit Bus & Cargo Trucks:** Heavy commercial vehicles serving as mobile environmental obstacles and destructible cover.
- **Spline GIS Traffic AI:** Autonomous navigation along actual Karaikal road networks with intersection rules, collision avoidance, and pedestrian crossings.

### 🚨 Chaos & Escalation System
- **Dynamic Wanted Level (1 to 5 Stars):**
  - **★☆☆☆☆ (Civil Patrol):** Local police cruisers attempt interception and megaphone warnings.
  - **★★☆☆☆ (Active Pursuit):** Aggressive roadblocks and spike strips deployed along key avenues.
  - **★★★☆☆ (Armed Tactical):** SWAT tactical units in armored vans with suppression fire.
  - **★★★★☆ (State Defense):** Heavy APCs, combat barricades, and sniper helicopter support.
  - **★★★★★ (National Guard / Full Military):** Attack gunships, heavy armor, and tactical airstrike response.

---

## 🎨 4. Blender 4.5 Automated Asset Pipeline

The project features a fully automated, headless 3D asset generation pipeline built on **Blender 4.5.16 LTS Python API** (`bpy`):

```
Tools/Blender/
├── blender_utils.py               # Procedural mesh geometry, material, and UV helpers
├── generate_hero.py               # Armored superhero character with segmented plates
├── generate_auto_rickshaw.py      # Authentic 3-wheeled auto-rickshaw model
├── generate_commercial_building.py# Multi-story Karaikal commercial bazaar shophouse
├── generate_motorcycle.py         # Classic Indian motorcycle with engine detail
├── generate_street_props.py       # Dravidian utility poles, street lamps, and barriers
└── export_unity_assets.py         # Headless batch generator producing .blend & .fbx
```

- **Zero Manual Bottlenecks:** Assets can be procedurally generated or modified from code in seconds.
- **URP Compatibility:** Meshes export with clean UVs, standard transform origins, and automatic material assignments matching Unity's Universal Render Pipeline Lit shader.
- **Unity Asset Bridge:** `BlenderAssetBridge.cs` and `KaraikalVerticalSliceManager.cs` automatically load exported models into the active scene at runtime.

---

## 🛠️ 5. Technology Stack

| Layer | Technology | Specification / Details |
|---|---|---|
| **Game Engine** | Unity 6 LTS | Version `6000.0.84f1` |
| **Render Pipeline** | Universal Render Pipeline (URP) | Version `17.0.3` (Forward+ Renderer, Linear Color Space) |
| **Scripting Backend** | C# / .NET Standard 2.1 | Mono & IL2CPP compatible |
| **3D Modeling & CAD** | Blender 4.5 LTS | Version `4.5.16` (`bpy` Python Automation) |
| **GIS Data** | OpenStreetMap (OSM) | Real-world Karaikal road vectors & coastal bounding box |
| **Asset Tracking** | Git LFS | Version `3.8.1` for binary assets (`.blend`, `.fbx`, audio) |
| **Analysis Framework**| REA | Version `6.10.1` reverse engineering & performance suite |

---

## 🎮 6. Controls Reference

| Action | Keyboard & Mouse | Gamepad (Xbox / DualShock) |
|---|---|---|
| **Move / Steer** | `W`, `A`, `S`, `D` | Left Stick |
| **Look / Aim** | Mouse Movement | Right Stick |
| **Sprint / Sonic Boost** | `Left Shift` | Left Stick Click (`L3`) |
| **Jump / Ascend (Flight)** | `Space` | `A` (Cross) |
| **Crouch / Descend (Flight)**| `Left Ctrl` / `C` | `B` (Circle) |
| **Toggle Flight Mode** | `F` | `Y` (Triangle) |
| **Primary Attack / Blast** | `Left Mouse Button` | Right Trigger (`RT`) |
| **Telekinesis / Grab Debris**| `Right Mouse Button` | Left Trigger (`LT`) |
| **Ground Slam** | `Q` (while airborne) | `X` (Square) |
| **Enter / Exit Vehicle** | `E` | `Y` (Hold) |
| **Pause Menu** | `Escape` | `Start` / `Options` |

---

## 🚀 7. Developer Getting Started

### Prerequisites
1. **Unity Editor 6.0 LTS** (`6000.0.84f1` or later) with Universal RP support.
2. **Git** with **Git LFS** installed:
   ```powershell
   git lfs install
   ```
3. **Blender 4.5 LTS** (optional, required only for regenerating 3D models via scripts):
   - Set executable path in environment or use default `C:\Program Files\Blender Foundation\Blender 4.5\blender.exe`.

### Clone & Setup
```powershell
# Clone the repository
git clone https://github.com/developer-vmmch/God-Mode.git
cd God-Mode

# Pull all Git LFS binary assets (.blend, .fbx, audio)
git lfs pull
```

### Opening in Unity
1. Open **Unity Hub**.
2. Click **Add > Add project from disk**.
3. Select the `UnityProject` folder inside `God-Mode/`.
4. Launch with **Unity 6000.0.84f1**.
5. In the Project window, navigate to `Assets/Scenes/` and open `Karaikal_OpenWorld_VerticalSlice.unity` (or `MainScene.unity`).
6. Press **Play** (`Ctrl + P`) to launch the playable sandbox.

---

## 📂 8. Architecture & Repository Layout

```
CSVS-GodMode/
├── BlenderAssets/                 # Procedurally generated Blender source files (.blend)
│   ├── Hero_Armored.blend
│   ├── AutoRickshaw.blend
│   ├── Commercial_Bazaar.blend
│   ├── Motorcycle.blend
│   └── UtilityPole.blend
├── Tools/
│   └── Blender/                   # Python automation scripts for Blender bpy
│       ├── blender_utils.py
│       ├── generate_hero.py
│       ├── generate_auto_rickshaw.py
│       ├── generate_commercial_building.py
│       ├── generate_motorcycle.py
│       ├── generate_street_props.py
│       └── export_unity_assets.py
├── UnityProject/                  # Canonical Unity 6 project
│   ├── Assets/
│   │   ├── Art/
│   │   │   ├── Models/            # Exported .fbx models and prefabs
│   │   │   └── Materials/         # URP Lit materials
│   │   ├── Audio/                 # SFX and ambient audio clips
│   │   ├── Scenes/                # Open-world test and gameplay scenes
│   │   ├── Scripts/               # C# Gameplay, Camera, AI, Destruction managers
│   │   │   ├── Core/              # GameInputManager, GameDirector
│   │   │   ├── Player/            # SuperheroFlightController, SuperheroCameraController
│   │   │   ├── Vehicles/          # VehiclePhysicsController, AutoRickshawController
│   │   │   ├── World/             # KaraikalMapManager, RoadNetworkGenerator, BlenderAssetBridge
│   │   │   ├── Destruction/       # VoronoiDestructionController, BuildingDemolition
│   │   │   └── Tests/             # PlayMode and EditMode automated test suites
│   │   └── Settings/              # URP Render Pipeline assets and Quality settings
│   ├── Packages/                  # Unity Package manifest
│   └── ProjectSettings/           # Engine configuration and layer definitions
├── Docs/                          # Architecture specs, GIS maps, investigation reports
├── .gitattributes                 # Git LFS configuration
├── .gitignore                     # Production Unity 6 gitignore
└── README.md                      # Project documentation
```

---

## 🗺️ 9. Development Roadmap

- [x] **v0.1.0 Prototype Foundation:** Physics sandbox, basic player flight, cube procedural buildings, URP shader pipeline.
- [x] **v0.2.0 Karaikal GIS Integration:** Real OpenStreetMap coordinate bounds, Arasalar River delta, coastal beach boundary, South Indian color schemes.
- [x] **v0.2.2 Living City & Vehicles:** Auto-rickshaw handling, basic NPC pedestrians, police response escalation, minimap radar.
- [x] **v0.3.0 Blender 4.5 Asset Pipeline:** Automated Python 3D model generation, armored hero mesh, vertical slice integration, Git LFS repository backup.
- [ ] **v0.4.0 Dynamic Combat & Soundscape:** Authentic Tamil street ambience, localized voice acting, enhanced Voronoi building fracture, advanced AI behavior trees.
- [ ] **v0.5.0 Story Campaign & Missions:** Faction missions across Karaikal Port, Temple defense, high-speed highway chases.
- [ ] **v1.0.0 Commercial Release:** Full optimization, multi-platform builds (Windows, WebGL, Linux), HDR lighting, full controller support.

---

## ⚖️ 10. Legal & Attribution Disclaimer

**CSVS GOD MODE — CITY OF CHAOS** is an independent, original video game property created and developed by **CSVS IT Tech**.
- All characters, vehicles, gameplay systems, code, and 3D procedural models are original creative assets of CSVS IT Tech.
- Geographic depictions of Karaikal, Puducherry, India are based on publicly available geographical data and real-world topological references.
- This project contains no proprietary assets, code, or trademarks belonging to any external publisher or studio.

---

**© 2026 CSVS IT Tech. All Rights Reserved.**
