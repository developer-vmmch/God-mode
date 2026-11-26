# CSVS GOD MODE — INPUT BINDINGS SPECIFICATION
**Version:** V0.2.2  
**Studio:** CSVS IT Tech  
**Status:** Unified & Conflict-Free  

---

## 1. Centralized Input Map (`GameInputManager.cs`)

All user input handling has been unified into `GameInputManager.cs` under the new Unity Input System. Conflicting bindings from previous prototypes have been eliminated:

| Action Category | Key / Button | Function | Context / Mutual Exclusivity |
|---|---|---|---|
| **Locomotion** | `W`, `A`, `S`, `D` | Ground Move / Flight Direction / Vehicle Drive | Universal |
| **Camera Orbit** | `Mouse Delta` | 360° Yaw and Pitch look | Active when cursor locked |
| **Vertical / Jump** | `Space` | Jump (Ground) / Ascend (Flight) / Handbrake (Vehicle) | Contextual |
| **Descend** | `Left Ctrl` or `C` | Descend downward in flight / hover | Flight only |
| **Supersonic Boost** | `Left Shift` | Sprint (Ground) / Supersonic Flight up to 95 m/s | Ground / Flight |
| **Super Punch** | `F` | Kinetic physical strike (1,200 N, localized fracture) | Cannot activate while holding object |
| **Vehicle Interact** | `G` | Enter vehicle (within 3.5m) / Exit vehicle | Decoupled from F key |
| **Grab / Interact** | `E` | Grab car or concrete debris / Drop held object | Mutual exclusivity via AbilityState |
| **Throw Object** | `Left Mouse Button` | Launch held physics object at 45 m/s | When holding object |
| **Heat Vision** | `Left Mouse Button` | Twin continuous thermal laser beams (450 DPS) | When hands free |
| **Crush Object** | `Right Mouse Button` | Crush held vehicle or debris into fragments | When holding object |
| **Gravity Tether** | `Q` | Connect and suspend physics objects in anti-gravity | Press/Hold Q |
| **Ground Slam** | Space $\rightarrow$ Dive $\rightarrow$ Ground | High-velocity downward strike with radial shockwave | Initiated mid-air |
| **Camera Perspective** | `V` | Toggle First-Person / Third-Person view | Smooth damping |
| **Full-Screen Map** | `M` | Toggle full-screen Karaikal GPS landmark map | Overlay modal |
| **Scene Reset** | `R` | Reload main city scene and reset score | Debug / Gameplay |
| **Pause** | `Esc` or `P` | Pause simulation and open menu | UI modal |

---

## 2. On-Screen Dynamic Control Hints

`GameInputManager.GetCurrentControlHints(...)` dynamically supplies context-sensitive HUD hints:
- **Driving:** `[W/S] Accelerate/Brake   [A/D] Steer   [SPACE] Handbrake   [G] Exit Vehicle   [V] Camera`
- **Holding Object:** `[LMB] Throw Object   [RMB] Crush Object   [E] Drop   [WASD] Move   [V] Camera`
- **Free Flight:** `[WASD] Fly   [L-SHIFT] Supersonic (95 m/s)   [SPACE] Ascend   [CTRL] Descend   [LMB] Heat Vision   [F] Super Punch   [Q] Tether`
- **Ground:** `[WASD] Move   [L-SHIFT] Sprint   [SPACE x2] Takeoff   [F] Super Punch   [E] Grab Car/Debris   [LMB] Heat Vision   [Q] Tether`
