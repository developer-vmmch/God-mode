# MISSION SYSTEM REPORT — CSVS GOD MODE V0.3

## 1. Mission Framework Architecture

The V0.3 mission system (`MissionSystem.cs`) is an entirely original CSVS IT Tech story and mechanics framework. No Rockstar Games missions, characters, scripts, cutscene logic, or voice lines are reproduced.

### Core Components

| Component | File | Function |
| :--- | :--- | :--- |
| `MissionSystem` | `Scripts/Missions/MissionSystem.cs` | Runtime mission orchestrator |
| `MissionData` | (embedded class) | Mission definition: ID, title, objectives, rewards |
| `MissionObjective` | (embedded class) | Individual objective: type, target, progress tracking |
| `MissionHUDController` | `Scripts/Missions/MissionHUDController.cs` | HUD display bridge |
| `ReputationChaosSystem` | `Scripts/Core/ReputationChaosSystem.cs` | Reward disbursement |

---

## 2. Original Story — The Karaikal Chronicles

**Setting**: Karaikal, Union Territory of Puducherry, coastal South India.  
**Protagonist**: An ordinary citizen who discovers latent superhuman abilities following a mysterious energy event at sea.  
**Premise**: Balancing civilian life with the responsibility of protecting Karaikal from escalating threats, while the city's reputation as a peaceful coastal town hangs in the balance.

---

## 3. Mission Definitions

### Mission 1 — "Welcome to Karaikal"
- **Type**: Tutorial / Exploration  
- **Player Mode**: Human  
- **Objectives**: Walk to Karaikal Railway Station → Drive to Town Center (Pettai Bazaar)  
- **Reward**: +10 Reputation  
- **Purpose**: Introduces human movement, camera controls, vehicle entry (`G` key), and driving.

### Mission 2 — "Awakening"
- **Type**: Transformation Tutorial  
- **Player Mode**: Human → Superhero  
- **Objectives**: Transform (`T` key) → Fly to Karaikal Beach → Reach 100m altitude  
- **Reward**: +25 Reputation  
- **Purpose**: Introduces superhero transformation, flight mechanics, and camera transition.

### Mission 3 — "Coastal Rescue"
- **Type**: Rescue / Exploration  
- **Player Mode**: Superhero  
- **Objectives**: Fly to coastal zone → Rescue 3 civilians in distress → Return to shore  
- **Reward**: +40 Reputation  
- **Purpose**: Reinforces flight and power targeting; introduces NPC interaction.

### Mission 4 — "Industrial Crisis"
- **Type**: Combat / Destruction  
- **Player Mode**: Superhero  
- **Setting**: Fictional CSVS Industrial Complex (not based on any real Karaikal facility)  
- **Objectives**: Fly to complex → Destroy 5 hazard containers → Secure perimeter (30s)  
- **Reward**: +50 Reputation  
- **Purpose**: Introduces destruction combat and countdown timers.

### Mission 5 — "City Guardian"
- **Type**: Combat / Defense  
- **Player Mode**: Superhero  
- **Objectives**: Reach Pettai Bazaar → Defeat 10 hostile combatants → Survive 60s  
- **Reward**: +75 Reputation  
- **Purpose**: Introduces the chaos and reputation system; demonstrates NPC threat escalation.

### Mission 6 — "Power Unleashed — Chaos Arena"
- **Type**: Power Training / Sandbox  
- **Player Mode**: Superhero  
- **Setting**: Fictional combat training arena outside Karaikal (safe for full destruction)  
- **Objectives**: Fly to arena → Destroy 20 targets → Complete 120s challenge  
- **Reward**: +100 Reputation  
- **Purpose**: Free-form power showcase; demonstrates all superhero abilities.

---

## 4. Objective Types

| Type | Description |
| :--- | :--- |
| `ReachLocation` | Player must move within `Radius` metres of `TargetPosition` |
| `ProtectCivilian` | NPC civilians survive within mission zone |
| `DestroyTarget` | Player hits `RequiredCount` destructible targets |
| `CollectItem` | Player picks up mission item objects |
| `SurviveTime` | Player remains alive for `TimeLimitSeconds` |
| `AltitudeFlight` | Player's Y position exceeds `TargetPosition.y` |

---

## 5. Progression & Save State

- Missions unlock sequentially via `UnlocksMissionID` chaining.
- Completed mission state and Reputation score are persisted via `PlayerPrefs`.
- Free Roam mode (`PlayerPrefs.GetInt("FreeRoam")`) bypasses mission gating.
- All missions can be replayed after completion.

