# CSVS GOD MODE — PLAYABLE INDIAN VEHICLES REPORT
**Version:** V0.2.1  
**Studio:** CSVS IT Tech  
**Setting:** Karaikal, Puducherry, India  
**Date:** November 26, 2026  

---

## 1. Indian Vehicle Lineup

In `PlayableIndianVehicle.cs`, four authentic Indian vehicle archetypes are implemented with procedural geometry, physical suspension, steering, and damage handling:

1. **Indian Hatchback:**
   - Inspired by compact city cars (Maruti Swift / Hyundai i10).
   - Mass: $1,100\,\text{kg}$, Engine: $1,800\,\text{N}$, Top Speed: $90\,\text{km/h}$.
   - Dual front headlights, glass cabin, 4 rotating rubber wheels.
2. **Indian Sedan:**
   - Executive passenger vehicle (Honda City / Maruti Dzire).
   - Mass: $1,250\,\text{kg}$, Engine: $2,100\,\text{N}$, Top Speed: $105\,\text{km/h}$.
   - Extended trunk, tinted glass, smooth suspension damping.
3. **South Indian Auto-Rickshaw:**
   - Iconic three-wheeled auto-rickshaw (Bajaj RE).
   - Color: Characteristic yellow upper canopy with black lower passenger tub.
   - Mass: $450\,\text{kg}$, 3 wheels (single centered front wheel), high agility ($80^\circ/\text{s}$ turn rate).
4. **Indian Motorcycle:**
   - Standard 150cc commuter motorcycle (Hero Splendor / Pulsar).
   - Mass: $140\,\text{kg}$, Top Speed: $95\,\text{km/h}$.

---

## 2. Interactive Superhero Mechanics

- **Player Enter / Exit (`F` Key):** Approaching within $3.5\,\text{m}$ allows the player to enter the vehicle. The camera smoothly blends to vehicle-follow mode, flight physics are suspended, and player throttle/steering drives the vehicle. Pressing `F` again ejects the superhero smoothly onto the sidewalk.
- **Left-Hand Traffic Circulation:** When not player-driven, AI vehicles circulate autonomously along designated left lanes at $30\text{–}50\,\text{km/h}$.
- **Destruction & Super Strength Grab:** All vehicles implement `IDamageable` ($350\,\text{HP}$). The superhero can pick up any vehicle using Super Strength (`E` key), carry it overhead, throw it at buildings, or tether it using Gravity Tether (`RMB`).
