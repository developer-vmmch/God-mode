# VEHICLE PHYSICS & DYNAMICS REPORT — KARAIKAL V0.2.2

## 1. Vehicle Fleet Roster & Physical Specifications

All vehicles in **CSVS GOD MODE** are generated with authentic 3D multi-element geometry and simulated through customized Rigidbody physics with tire friction curves.

| Vehicle Type | Model Reference | Mass | Top Speed | Wheel Config | Center of Mass | Suspension Rigidity |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **Auto-Rickshaw** | Bajaj RE / Piaggio Ape | $650\text{ kg}$ | $60\text{ km/h}$ | 3-wheel delta (1F, 2R) | Lowered $(Y = -0.35\text{ m})$ | Medium-Soft |
| **Indian Hatchback** | Maruti Suzuki Swift | $1050\text{ kg}$ | $140\text{ km/h}$ | 4-wheel independent | Balanced $(Y = -0.20\text{ m})$ | Balanced |
| **Indian Sedan** | Tata Tigor / Dzire | $1200\text{ kg}$ | $155\text{ km/h}$ | 4-wheel independent | Balanced $(Y = -0.20\text{ m})$ | Balanced Comfort |
| **Commuter Motorcycle**| Hero Splendor / Pulsar | $140\text{ kg}$ | $100\text{ km/h}$ | 2-wheel in-line | Dynamic Lean $(Y = -0.10\text{ m})$| Stiff Commuter |
| **City Transit Bus** | Ashok Leyland PRTC | $9500\text{ kg}$ | $75\text{ km/h}$ | 6-wheel heavy dual-rear| Elevated $(Y = +0.10\text{ m})$ | Heavy-Duty Stiff |

---

## 2. Auto-Rickshaw Specialized 3-Wheel Dynamics

The iconic South Indian Auto-Rickshaw features unique physical tuning to capture authentic handling while preventing unrealistic rollovers:
- **Steering Fork Geometry**: Single front fork with positive caster angle ($18^\circ$) providing natural straight-line centering.
- **Anti-Roll COM Offset**: Center of Mass artificially lowered below the passenger floorpan to allow drifting through tight Karaikal bazaar corners without tipping over.
- **Visual Detailing**:
  - Yellow canvas weather canopy with side flap openings.
  - Black passenger tub with authentic side entrance contours.
  - Single front chrome circular headlight with protective wire guard.
  - Rear engine louvers, tail lights, and bumper guard.

---

## 3. Vehicle Interaction & Control Integration

- **Entry / Exit Trigger**: Decoupled to dedicated interaction key `G` via `GameInputManager.WasVehicleInteractTriggered`.
- **Proximity Detection**: Spherical trigger zone ($3.5\text{ m}$ radius) around driver door/opening.
- **Camera Handoff**: Smooth lerp from third-person superhero camera to low-angle vehicle chase camera ($4.5\text{ m}$ follow distance, $1.8\text{ m}$ elevation).
- **Steering & Throttle**: Seamless analog input through `WASD` / Arrow keys with speed-sensitive steering dampening to ensure high-speed stability.

---

## 4. Verification Status

- **Automated Test**: `VehicleModelGenerator_GeneratesDetailedAutoRickshawModel` passed.
- **In-Game Screenshot**: `gameplay_v022_auto_rickshaw.png` captured and verified (0.00% magenta pixels).
