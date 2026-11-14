# SYSTEM 1: PHASE 3 – EMERGENCY, REPAIR AND CHARACTER INTERACTION LOGIC
## Technical Specifications & Integration Guide

**Document Version:** 1.0.0  
**Target Environment:** Unity 6 (6000.6.9f1) / URP / WebGL  
**Assembly Definitions:** `Outpost.Core`, `Outpost.System1.Gameplay`, `Outpost.System1.Gameplay.Editor`  
**System Lead:** Lead Gameplay Programmer (System 1)

---

## 1. System Overview & Architecture

Phase 3 implements the full emergency lifecycle, Apollo 13-inspired filtration challenge, astronaut verbal interaction triggers, and strategic multi-pathway decision support for *Outpost: Survive Beyond Earth* (Mission 1 – Lunar South Pole).

### Subsystem Wiring
```
+-----------------------------------------------------------------------------------------+
|                                      Outpost.Core                                       |
|  - Outpost.Core.Data (EmergencyModel, EmergencyType, EmergencySeverity, RepairOutcome)  |
|  - OutpostEventChannel (EmergencyStateChanged, RepairOutcomeEvaluated, Dialogue)         |
+-----------------------------------------------------------------------------------------+
            ^                                                                 ^
            | references                                                      | references
+---------------------------------------------+   +---------------------------------------+
|          Outpost.System1.Gameplay           |   |       Outpost.System3.Interface       |
|  - EmergencyManager (Central 8 Emergencies) |   |  - OutpostHUD (Emergency Banner)      |
|  - FiltrationChallengeController (Apollo 13)|   |  - StrategicChoiceUI (Modal Dialog)   |
|  - ContextualConversationTrigger (Crew Chat)|   |  - FiltrationPuzzleUI (Minigame UI)   |
|  - ResourceManager (Consequence Execution)  |   |  - DialogueUI (Conversation Player)   |
+---------------------------------------------+   +---------------------------------------+
```

> **UI Decoupling Rule:**  
> System 1 contains zero visual UI elements.  
> All modal presentation, puzzle graphics, and dialogue badges are rendered exclusively by Computer 3 (`Outpost.System3.Interface`) via `OutpostEventChannel`.

---

## 2. The Eight Mission 1 Emergencies

Every emergency is managed by [`EmergencyManager.cs`](file:///d:/nasa_crosswind/Assets/Outpost/System1_Gameplay/Emergency/EmergencyManager.cs) with explicit triggers, severity ratings, equipment impacts, resource penalties, player objectives, repair conditions, and recovery states:

| # | Emergency Type | Severity | Trigger | Equipment Consequence | Resource Consequence | Player Objective | Repair Condition | Recovery State |
|---|---|---|---|---|---|---|---|---|
| **1** | **Solar Panel Damage** | `Critical` | Micrometeoroid swarm during Chapter 3 solar storm. | `SolarArrayEquipment` condition drops to $15\%$; array offline. | Solar output drops to emergency $8.0\text{ kW}$ trickle; battery discharges rapidly. | Inspect damaged array at Solar Farm and select repair approach. | Astronaut EVA splicing or Rover robotic maintenance. | Array restored to $100\%$; $55.0\text{ kW}$ generation restored. |
| **2** | **Electrical Distribution Fault** | `Critical` | Electrical surge from severed array short-circuit. | Primary substation breakers locked out; sub-bus feeds cut. | Grid enters `LoadShedding` / `Brownout`. Non-essential loads shed. | Reset primary distribution breakers at Emergency Substation. | Manual breaker toggle using Cable Splicer tool. | Bus re-energized; nominal power distribution restored. |
| **3** | **Oxygen Generator Interruption** | `High` | Power fluctuation triggers Sabatier electrolysis thermal trip. | `OxygenGeneratorEquipment` enters `GeneratorStatus.Fault`. | O2 production halts ($0\text{ L/s}$); cabin relies on reserve buffer. | Cycle thermal purge valve on Hab O2 Console. | Cool down cell and clear thermal fault on console. | Electrolysis nominal; $+0.20\text{ L/s}$ production restored. |
| **4** | **Carbon Dioxide Accumulation** | `Critical` | Primary LiOH absorption canister saturated; bypass seal torn. | `CarbonDioxideScrubberEquipment` condition drops to $0\%$. | CO2 climbs from $420\text{ ppm}$ toward toxic hypercapnia ($1,650\text{ ppm}$). | Assemble and install emergency filtration adapter (Apollo 13). | Complete Apollo 13 puzzle with canister, cardboard, hose, tape. | Scrubber operational; CO2 levels decreasing below $650\text{ ppm}$. |
| **5** | **Water Recycling Failure** | `Medium` | Particulate matter clogs catalytic oxidizer filter bed. | `WaterRecyclingEquipment` recovery efficiency drops to $0\%$. | Water recycling halts ($0\text{ L/s}$); potable reserves drain. | Backflush catalytic particulate filter at WRS manifold. | Perform filter purge or replace filter membrane. | WRS closed loop restored at $92\%$ recovery efficiency. |
| **6** | **Greenhouse Power Shortage** | `Medium` | Grow light circuits shed during battery brownout. | `GreenhouseHydroponicsEquipment` lights unpowered; pumps idle. | Crop vitality drops; food ration yield and supplemental O2 halt. | Route emergency auxiliary battery feed to Greenhouse. | Restore main power grid or re-enable auxiliary breaker. | Grow LED arrays active; crop biomass growth resumes. |
| **7** | **Communication Blackout** | `Medium` | High-gain dish tracking motor frozen by solar storm flux. | Deep space dish loses tracking; Earth telemetry link lost. | Remote mission control advisories cut; ORION offline mode. | Re-align antenna dish azimuth at Communication Tower. | Manual gimbal calibration at Tower terminal. | Earth telemetry restored; high-speed link confirmed. |
| **8** | **Habitat Temperature Issue** | `High` | External thermal radiator loop freezes in lunar shadow. | Radiator loop valve stuck; internal cabin temperature drops. | Thermal heaters draw emergency $+8\text{ kW}$; battery drains faster. | Cycle radiator bypass valve on Life Support manifold. | Manual valve actuation and thermal loop purge. | Cabin temperature normalizes at $21^\circ\text{C}$; heater load nominal. |

---

## 3. Apollo 13-Inspired Emergency Filtration Challenge

Managed by [`FiltrationChallengeController.cs`](file:///d:/nasa_crosswind/Assets/Outpost/System1_Gameplay/Emergency/FiltrationChallengeController.cs):
- **Problem Diagnostics:**
  The lunar habitat's circular intake socket ($15\text{ cm}$ diameter) does not accept the square LiOH replacement canisters stored from the Command Module. Airflow bypass causes rapid carbon dioxide accumulation.
- **Available Components:**
  1. *Square LiOH Canister* (Chemical CO2 absorption filter containing lithium hydroxide).
  2. *Flight Manual Cardboard Cover* (Rigid laminated cover rolled into a square-to-round geometric funnel).
  3. *Suit Umbilical Hose* (Flexible breathing tube channeling blower airflow).
  4. *Heavy-Duty Space Duct Tape* (Pressure-rated adhesive providing an airtight hermetic seal).
  5. *Red Herrings* (Copper wire, glass pipettes, lunar regolith—rejected with educational error feedback).
- **Validation Rules & Feedback:**
  - *Missing tape:* Fails with `"AIR LEAK: Cardboard seams are leaking carbon dioxide! Seal all edges with Space Duct Tape!"`
  - *Missing cardboard:* Fails with `"GEOMETRIC MISMATCH: A square canister cannot mate to a round hole without the Cardboard Manual funnel!"`
  - *Missing hose:* Fails with `"NO AIRFLOW: Blower intake requires the flexible Suit Umbilical Hose!"`
  - *Red herrings included:* Fails with `"INCOMPATIBLE PARTS: Foreign laboratory tools or abrasive regolith cannot be used in a life-support airflow circuit!"`
  - *Valid assembly:* Emits success chime, resolves emergency, and triggers `ResourceManager.Instance.RepairFiltrationCanister()`.

---

## 4. Contextual Character Interaction Logic

Astronaut conversations are triggered exclusively by actual gameplay milestones via [`ContextualConversationTrigger.cs`](file:///d:/nasa_crosswind/Assets/Outpost/System1_Gameplay/Core/ContextualConversationTrigger.cs):

### 1. Marcus Requests Elena's Approval Before External Repair
- **Trigger:** Approaching the EVA airlock during `SolarPanelDamage` or initiating Chapter 6.
- **Participants:** Marcus, Elena.
- **Dialogue Excerpt:**
  - *Marcus:* "Commander, I've suited up. If I go out on EVA, I can manually splice the high-voltage relay in forty minutes."
  - *Elena:* "Marcus, radiation monitors are still elevated from the coronal flare. An external spacewalk carries genuine radiation exposure."
  - *Marcus:* "I know the risks, Elena. But if we don't restore the primary array, our life-support batteries will drain completely. I need your green light."
  - *Elena:* "Understood, Marcus. Review the radiation threshold with Daniel first. If dosimeter readings cross five millisieverts, abort immediately."

### 2. Aisha Reports Scientific Solar Flare Observations
- **Trigger:** Transition into Chapter 3 or spectrometer scanning of lunar surface.
- **Participants:** Aisha, Elena, Daniel.
- **Dialogue Excerpt:**
  - *Aisha:* "Commander Elena, our Shackleton spectrometer readings just registered an anomalous flare in solar proton flux!"
  - *Elena:* "Is the coronal mass ejection heading directly toward our coordinates, Aisha?"
  - *Aisha:* "Yes. The solar wind velocity is over 600 kilometers per second. We have about fifteen minutes before secondary micrometeoroids and electromagnetic interference hit our ridge."
  - *Daniel:* "Crew radiation dosimeters are calibrated. Everyone inside the central habitat module now."

### 3. Daniel Warns Crew About Health Risks
- **Trigger:** Carbon dioxide exceeds $700\text{ ppm}$ during life support failure.
- **Participants:** Daniel, Elena, Marcus.
- **Dialogue Excerpt:**
  - *Daniel:* "Attention crew: habitat atmospheric readings are deteriorating! Carbon dioxide has passed 700 parts per million."
  - *Elena:* "Daniel, what are the physiological thresholds?"
  - *Daniel:* "At 1,000 ppm, we'll experience headaches, fatigue, and impaired decision-making. Above 1,500 ppm, hypercapnia causes dizziness and loss of consciousness. We must bring the scrubbers back online immediately!"
  - *Marcus:* "Aisha and I are pulling spare parts from storage now, Doc. Hang tight!"

### 4. Elena Coordinates Strategic Repair Options
- **Trigger:** Chapter 6 strategic decision modal opened (`OutpostEventChannel.StrategicChoiceOpened`).
- **Participants:** Elena, Aisha, Daniel.
- **Dialogue Excerpt:**
  - *Elena:* "Team, we have three distinct pathways to restore outpost power. Option A: Marcus performs an external EVA repair. Option B: We deploy the rover's robotic arm to repair the relay remotely. Option C: We shed non-essential loads, conserve battery reserves, and wait out the solar storm."
  - *Aisha:* "The robotic option protects the crew from radiation, but the rover's manipulator arm has limited dexterity."
  - *Daniel:* "Conserving power avoids all EVA risks, but our life-support margins will be razor-thin."
  - *Elena:* "Every option has consequences. Player, evaluate our resources and choose our strategy."

---

## 5. Decision Support: Three Strategic Repair Pathways

Implemented in [`EmergencyManager.cs`](file:///d:/nasa_crosswind/Assets/Outpost/System1_Gameplay/Emergency/EmergencyManager.cs):

| Parameter | Approach A: Marcus EVA Repair | Approach B: Robotic Maintenance | Approach C: Conserve Power & Delay |
|---|---|---|---|
| **Repair Duration** | Fast ($45\text{ seconds}$) | Moderate ($90\text{ seconds}$) | Slow ($180\text{ seconds}$) |
| **Restored Power** | $55.0\text{ kW}$ ($100\%$ full power) | $44.0\text{ kW}$ ($80\%$ capacity) | $49.5\text{ kW}$ ($90\%$ capacity) |
| **Equipment Condition** | $100\%$ (Human dexterity) | $80\%$ (Coarse mechanical grip) | $90\%$ (Post-storm repair) |
| **Crew Health Impact** | $-10\%$ health (Radiation exposure) | $0\%$ health impact (Crew safe) | $0\%$ radiation exposure |
| **Battery Resource Cost** | $5.0\text{ kWh}$ (Suit heating) | $12.0\text{ kWh}$ (Rover motors) | $35.0\text{ kWh}$ (Drained during wait) |
| **Oxygen Resource Cost** | $15.0\text{ L}$ (Suit EVA breathing) | $0.0\text{ L}$ (Zero astronaut O2) | $0.0\text{ L}$ (Zero EVA O2) |
| **Operational Impact** | Restores power immediately; medical monitoring needed for Marcus. | Slower power restoration; preserves crew health completely. | Deep battery brownout; greenhouse biomass drops by $15\%$. |

Executing `EmergencyManager.Instance.ApplyRepairChoice(choice)` updates `ResourceManager.Instance.CurrentState` with the exact consequences above, resolves the solar/electrical emergencies, and emits `OutpostEventChannel.RaiseRepairOutcomeEvaluated(outcome)`.
