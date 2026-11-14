# SYSTEM 1: PHASE 4 – COMPLETE MISSION 1 GAMEPLAY ARCHITECTURE
## Technical Specifications, Chapter Graph & Verification Report

**Project:** *OUTPOST: SURVIVE BEYOND EARTH*  
**Mission:** Mission 1 – The First Frontier  
**Setting:** Lunar South Pole (Shackleton Crater Rim, 2045)  
**Target Platform:** Browser-Based Unity WebGL / Universal Render Pipeline (URP)  
**Target Audience:** Children aged 7–12 (STEM Education & Crisis Survival Simulation)  
**System Lead:** Lead Gameplay Programmer (System 1)  
**Document Version:** 1.0.0  
**Assemblies:** `Outpost.Core`, `Outpost.System1.Gameplay`, `Outpost.System1.Gameplay.Editor`  

---

## 1. Executive Summary & Narrative Overview

In Phase 4, the complete end-to-end mission loop of **Mission 1: The First Frontier** has been implemented, validated, and tested. The mission guides players through a seven-chapter storyline following four astronauts and an AI assistant as they land near the lunar south pole, establish an operational outpost, survive a critical space emergency, execute strategic engineering repairs, and stabilize Outpost Alpha into a permanent, self-sustaining scientific research station.

```
+-----------------------------------------------------------------------------------------------------------------+
|                                          MISSION 1 SEVEN-CHAPTER GRAPH                                          |
+-----------------------------------------------------------------------------------------------------------------+
| [Ch 1: Arrival] -> [Ch 2: Outpost Establishment] -> [Ch 3: The First Warning] -> [Ch 4: Life-Support Crisis]    |
|                                                                                             |                   |
| [Ch 7: Ending & Credits] <- [Ch 6: Recovery & Save] <- [Ch 5: Save The Outpost (3 Pathways)] <------------------+
+-----------------------------------------------------------------------------------------------------------------+
```

### The Crew & Roles
1. **Commander Elena:** Mission Commander (Calm, disciplined, decisive leadership, emergency repair coordination).
2. **Engineer Marcus:** Systems Engineer (Hardware repairs, electrical microgrid, photovoltaic solar arrays, EVA spacewalks).
3. **Scientist Aisha:** Lunar Scientist (Lunar geology, ice volatiles spectrometry, coronal mass ejection detection).
4. **Medical Officer Daniel:** Medical Officer (Crew health, pulse-oximetry, hypercapnia warnings, radiation monitoring).
5. **O.R.I.O.N.:** Onboard Robotic Intelligence and Operations Network (AI monitoring, navigation waypoints, life-support alarms, STEM education).

---

## 2. Complete 7-Chapter Progression Graph

The mission features 33 distinct objectives across all 7 chapters, fully defined in [`MissionManager.cs`](file:///d:/nasa_crosswind/Assets/Outpost/System1_Gameplay/Mission/MissionManager.cs) and exposed via [`IMapDataProvider`](file:///d:/nasa_crosswind/Assets/Outpost/Core/Interfaces/IMapDataProvider.cs) to Computer 3's visual Geo Map:

### Chapter 1: Arrival on the Moon (`MissionChapterId.Chapter1_Arrival`)
*Storyline: The crew touches down at Landing Zone Artemis, inspects descent hardware, hops across low-gravity regolith, cycles through the habitat airlock, meets ORION, and conducts the arrival briefing.*

| Step | Objective ID | Objective Title | Description | Target Station | Coordinates $(X, Y, Z)$ |
|---|---|---|---|---|---|
| **1** | `obj_ch1_1` | Touchdown Inspection | Inspect the lunar lander descent stage and verify personal EVA suit telemetry. | Landing Zone | $(0.0, 0.0, 0.0)$ |
| **2** | `obj_ch1_2` | Traverse Lunar Surface | Traverse the low-gravity lunar regolith toward Central Habitat Alpha. | Main Habitat | $(0.0, 0.0, 35.0)$ |
| **3** | `obj_ch1_3` | Airlock Entry & Inspection | Inspect the exterior airlock seal and enter Central Habitat Alpha. | Main Habitat | $(0.0, 0.0, 35.0)$ |
| **4** | `obj_ch1_4` | ORION AI Initialization | Activate the Central Habitat console to initialize O.R.I.O.N. operations network. | Main Habitat | $(0.0, 0.0, 35.0)$ |
| **5** | `obj_ch1_5` | Crew Arrival Briefing | Conduct the arrival briefing with Commander Elena, Marcus, Aisha, and Daniel. | Main Habitat | $(0.0, 0.0, 35.0)$ |

### Chapter 2: Establishing the Outpost (`MissionChapterId.Chapter2_OutpostEstablishment`)
*Storyline: Crew brings online all core life-support and scientific infrastructure: solar tracking arrays, battery banks, Sabatier electrolysis, water recycling, hydroponic greenhouse, and the Shackleton ice lab.*

| Step | Objective ID | Objective Title | Description | Target Station | Coordinates $(X, Y, Z)$ |
|---|---|---|---|---|---|
| **1** | `obj_ch2_1` | Deploy Solar Arrays | Align photovoltaic solar panels on the Ridge of Eternal Light for maximum solar tracking. | Solar Farm | $(45.0, 8.5, 95.0)$ |
| **2** | `obj_ch2_2` | Verify Battery Bank Reserves | Inspect primary and auxiliary battery storage capacity at the habitat power bus. | Main Habitat | $(0.0, 0.0, 35.0)$ |
| **3** | `obj_ch2_3` | Start Oxygen Generator | Start the Sabatier electrolysis oxygen generator to establish a breathable cabin atmosphere. | Main Habitat | $(0.0, 0.0, 35.0)$ |
| **4** | `obj_ch2_4` | Activate Water Recycling System | Verify the closed-loop catalytic water recycling unit ensuring 98% moisture recovery. | Equipment Storage | $(-28.0, 1.2, 50.0)$ |
| **5** | `obj_ch2_5` | Initialize Hydroponic Greenhouse | Initialize nutrient water flow and LED grow lights for salad crops and dwarf wheat. | Greenhouse | $(-35.0, 0.5, 60.0)$ |
| **6** | `obj_ch2_6` | Science Laboratory Exploration | Calibrate the spectrometer in the Science Lab for Shackleton crater ice samples. | Research Laboratory | $(30.0, 1.0, 45.0)$ |
| **7** | `obj_ch2_7` | Outpost Operational Readiness | Review all station telemetry with Commander Elena to confirm baseline readiness. | Main Habitat | $(0.0, 0.0, 35.0)$ |

### Chapter 3: The First Warning (`MissionChapterId.Chapter3_FirstWarning`)
*Storyline: Aisha's spectrometer detects an anomalous coronal flare and solar proton flux. Crew holds an urgent protocol conference, attempts comms dish calibration, shelters from incoming micrometeoroids, and investigates a severe power fault.*

| Step | Objective ID | Objective Title | Description | Target Station | Coordinates $(X, Y, Z)$ |
|---|---|---|---|---|---|
| **1** | `obj_ch3_1` | Solar Activity Flare Telemetry | Review Aisha's spectrometer readings detecting an incoming solar proton flare. | Research Laboratory | $(30.0, 1.0, 45.0)$ |
| **2** | `obj_ch3_2` | Pre-Incident Crew Conference | Assemble in Habitat Command with Elena, Marcus, and Daniel to review solar storm protocol. | Main Habitat | $(0.0, 0.0, 35.0)$ |
| **3** | `obj_ch3_3` | Earth Telemetry Calibration | Calibrate high-gain dish antenna at Communication Tower before storm interference. | Communication Tower | $(-60.0, 12.0, 110.0)$ |
| **4** | `obj_ch3_4` | Micrometeoroid Shelter Order | Seek immediate shelter inside shielded Central Habitat as debris swarm approaches. | Main Habitat | $(0.0, 0.0, 35.0)$ |
| **5** | `obj_ch3_5` | Electrical Substation Fault Inspection | Inspect tripped primary substation breakers at breach site following micrometeoroid impact. | Emergency Breach Site | $(42.0, 6.0, 88.0)$ |

### Chapter 4: Life-Support Crisis (`MissionChapterId.Chapter4_LifeSupportCrisis`)
*Storyline: Secondary power severed; Sabatier O2 offline and circular CO2 scrubbers saturated. Daniel warns of hypercapnia symptoms. ORION guides player to Equipment Storage. Player solves Apollo 13 filtration puzzle and restores safe cabin airflow.*

| Step | Objective ID | Objective Title | Description | Target Station | Coordinates $(X, Y, Z)$ |
|---|---|---|---|---|---|
| **1** | `obj_ch4_1` | Atmospheric Emergency Alert | Respond to critical life-support alarms: oxygen generation interrupted and CO2 scrubbers offline! | Main Habitat | $(0.0, 0.0, 35.0)$ |
| **2** | `obj_ch4_2` | Crew Health Monitoring | Consult Medical Officer Daniel regarding hypercapnia symptoms and crew pulse-oximetry. | Main Habitat | $(0.0, 0.0, 35.0)$ |
| **3** | `obj_ch4_3` | Locate Replacement Canisters | Follow ORION guidance to Equipment Storage to retrieve spare LiOH canisters and adapter materials. | Equipment Storage | $(-28.0, 1.2, 50.0)$ |
| **4** | `obj_ch4_4` | Apollo 13 Improvisation Challenge | Assemble an airtight adapter using square LiOH canister, suit hose, manual cardboard, and duct tape. | Equipment Storage | $(-28.0, 1.2, 50.0)$ |
| **5** | `obj_ch4_5` | Restore Airflow Circulation | Install the improvised adapter into circular scrubber intake and confirm CO2 drops below 650 ppm. | Main Habitat | $(0.0, 0.0, 35.0)$ |

### Chapter 5: Save the Outpost (`MissionChapterId.Chapter5_SaveTheOutpost`)
*Storyline: With life support stabilized, the station faces a critical battery discharge from the severed array. Commander Elena convenes a strategic briefing where the player evaluates and selects one of three distinct repair approaches.*

| Step | Objective ID | Objective Title | Description | Target Station | Coordinates $(X, Y, Z)$ |
|---|---|---|---|---|---|
| **1** | `obj_ch5_1` | Strategic Repair Briefing | Review power conduit breach and repair alternatives with Commander Elena. | Main Habitat | $(0.0, 0.0, 35.0)$ |
| **2** | `obj_ch5_2` | Implement Strategic Repair Decision | Choose between: Approach A (Marcus EVA), Approach B (Rover Robotic Arm), or Approach C (Conserve Power). | Main Habitat | $(0.0, 0.0, 35.0)$ |
| **3** | `obj_ch5_3` | Execute Repair Operation | Monitor execution telemetry, verify power grid reconnection, and resolve station emergency. | Emergency Breach Site | $(42.0, 6.0, 88.0)$ |

### Chapter 6: Recovery (`MissionChapterId.Chapter6_Recovery`)
*Storyline: Power grid stabilized. Crew re-energizes secondary equipment, confirms all life-support meters have returned to nominal, undergoes crew medical screening with Daniel, and commits operational status to station archives.*

| Step | Objective ID | Objective Title | Description | Target Station | Coordinates $(X, Y, Z)$ |
|---|---|---|---|---|---|
| **1** | `obj_ch6_1` | Restore Essential Equipment | Re-energize secondary electrical sub-buses, reactivate greenhouse lighting, and cycle radiator valves. | Solar Farm | $(45.0, 8.5, 95.0)$ |
| **2** | `obj_ch6_2` | Resource Telemetry Verification | Verify all resource meters (Oxygen, CO2, Power kW, Battery %, Water Liters) return to nominal levels. | Main Habitat | $(0.0, 0.0, 35.0)$ |
| **3** | `obj_ch6_3` | Crew Safety & Health Check | Undergo comprehensive medical and radiation dosimeter screening with Medical Officer Daniel. | Main Habitat | $(0.0, 0.0, 35.0)$ |
| **4** | `obj_ch6_4` | Log Outpost Status & Save Progress | Commit station recovery log to lunar records and save operational progress. | Main Habitat | $(0.0, 0.0, 35.0)$ |

### Chapter 7: Ending - A New Beginning (`MissionChapterId.Chapter7_Ending`)
*Storyline: Commander Elena debriefs the crew at the Observation Cupola overlooking Earthrise. The player is awarded the Lunar Pioneer Ribbon, reviews key STEM survival science, and triggers the final mission credits.*

| Step | Objective ID | Objective Title | Description | Target Station | Coordinates $(X, Y, Z)$ |
|---|---|---|---|---|---|
| **1** | `obj_ch7_1` | Mission 1 Debriefing | Gather at the Observation Cupola with Elena, Marcus, Aisha, and Daniel for final mission review. | Main Habitat | $(0.0, 0.0, 35.0)$ |
| **2** | `obj_ch7_2` | Lunar Pioneer Commendation | Receive the official Lunar Pioneer Ribbon for outstanding survival leadership and engineering ingenuity. | Main Habitat | $(0.0, 0.0, 35.0)$ |
| **3** | `obj_ch7_3` | Educational STEM Summary | Review key scientific concepts: lunar gravity, solar ridge, closed-loop life support, and Apollo 13 improvisation. | Main Habitat | $(0.0, 0.0, 35.0)$ |
| **4** | `obj_ch7_4` | Commence Lunar Operations & Credits | Celebrate Outpost Alpha's permanent operational status and trigger final mission credits. | Main Habitat | $(0.0, 0.0, 35.0)$ |

---

## 3. The Three Strategic Repair Pathways (Chapter 5)

All three repair decisions are fully implemented in [`EmergencyManager.cs`](file:///d:/nasa_crosswind/Assets/Outpost/System1_Gameplay/Emergency/EmergencyManager.cs) with real resource trade-offs, and all three provide a guaranteed, fair, and educational pathway to mission success:

| Metric | Approach A: Astronaut EVA Spacewalk | Approach B: Rover Robotic Arm | Approach C: Conserve Power & Delay |
|---|---|---|---|
| **Actor** | Engineer Marcus (Suited EVA) | Automated Rover Robotic Arm | Full Crew in Habitat Command |
| **Repair Duration** | **45 seconds** (Fastest) | **90 seconds** (Moderate) | **180 seconds** (Slowest) |
| **Restored Power** | **$55.0\text{ kW}$** ($100\%$ full capacity) | **$44.0\text{ kW}$** ($80\%$ operational) | **$49.5\text{ kW}$** ($90\%$ post-storm) |
| **Equipment Condition** | $100\%$ (Precision hand splicing) | $80\%$ (Coarse mechanical alignment) | $90\%$ (Safe post-storm repair) |
| **Crew Radiation Impact** | **$-10\%$ Health** (Solar flux exposure) | **$0\%$ Impact** (Crew safe in Hab) | **$0\%$ Impact** (Crew safe in Hab) |
| **Suit Oxygen Drain** | **$15.0\text{ Liters}$** (High metabolic burn) | **$0.0\text{ Liters}$** (Autonomous robot) | **$0.0\text{ Liters}$** (No spacewalk) |
| **Battery Energy Cost** | $5.0\text{ kWh}$ (Suit heating/comms) | $12.0\text{ kWh}$ (Rover arm actuators) | **$35.0\text{ kWh}$** (Extended shadow drain) |
| **Educational Lesson** | High courage and speed carries direct physical and radiation trade-offs. | Automation protects human life, but robotics has mechanical limits. | Disciplined resource rationing buys time for safe engineering. |
| **Story Outcome** | "Marcus torques down the relay. Solar reads a solid 55 kW! Taking minor radiation hits, heading inside." | "Elena: Rover arm cleared the junction. 80% array restored with zero crew exposure!" | "Aisha: Solar storm subsided. Marcus and rover completed repair under safe baseline conditions." |

---

## 4. Apollo 13-Inspired Filtration Challenge (Chapter 4)

Managed by [`FiltrationChallengeController.cs`](file:///d:/nasa_crosswind/Assets/Outpost/System1_Gameplay/Emergency/FiltrationChallengeController.cs) and rendered by Computer 3's [`FiltrationPuzzleUI.cs`](file:///d:/nasa_crosswind/Assets/Outpost/System3_Interface/Minigames/FiltrationPuzzleUI.cs):
- **Diagnostic Challenge:** The habitat's circular intake socket ($15\text{ cm}$) does not mate with the stored square LiOH replacement canisters. Cabin CO2 climbs toward hypercapnia ($1,650\text{ ppm}$).
- **Required Parts:**
  1. *Square LiOH Canister* (Chemical CO2 absorption).
  2. *Flight Manual Cardboard Cover* (Rolled into square-to-round geometric transition funnel).
  3. *Suit Umbilical Hose* (Flexible breathing tube routing intake airflow).
  4. *Space Duct Tape* (Hermetic airtight seal).
- **Red Herrings:** Copper wire, glass pipettes, lunar regolith (rejected with explanatory educational feedback).
- **Resolution:** Assembling the four valid parts emits `OutpostEventChannel.RaiseFiltrationChallengeCompleted(true)`, completing `obj_ch4_4`, clearing `CarbonDioxideAccumulation`, and restoring cabin CO2 below $650\text{ ppm}$.

---

## 5. Educational STEM Summary for Children (Ages 7–12)

Implemented in [`EducationalManager.cs`](file:///d:/nasa_crosswind/Assets/Outpost/System1_Gameplay/Education/EducationalManager.cs) and broadcast upon mission completion via `OutpostEventChannel.RaiseMissionSummaryPrompted`:

1. **Lunar Gravity (1.62 m/s²):** The Moon's gravity is only 1/6th of Earth's gravity. While astronauts feel very light and can hop easily, their mass and inertia remain identical, meaning stopping when running still requires deliberate effort!
2. **Ridge of Eternal Light (Polar Solar Power):** Because the Moon has a tiny axial tilt ($1.5^\circ$), crater ridges near the lunar south pole receive near-constant sunlight. Solar arrays track the low horizon to generate uninterrupted power.
3. **Closed-Loop Water Recovery (WRS):** Hauling water from Earth is extremely expensive. Outpost systems purify and recycle over $92\%$ of all humidity, breath vapor, and wash water into pristine drinking water.
4. **Carbon Dioxide Removal:** Every exhaled breath releases CO2. Without Lithium Hydroxide (LiOH) chemical scrubbers, toxic gas accumulates, causing headaches and disorientation.
5. **Apollo 13 Engineering Ingenuity:** When unexpected failures strike 384,000 km from Earth, astronauts and engineers use physics, geometric creativity, and simple materials (cardboard and duct tape) to solve complex emergencies.

---

## 6. Cross-System Integration & Decoupling Contracts

System 1 adheres strictly to the architectural decoupling boundaries:

```
[System 1: Gameplay] ---> [OutpostEventChannel (Outpost.Core)] <--- [System 3: Interface]
     - MissionManager                - ChapterChanged                    - OutpostHUD
     - EmergencyManager              - ObjectiveUpdated                  - DialogueUI
     - ResourceManager               - TargetWaypointSet                 - LunarGeoMapUI
     - EducationalManager            - FiltrationChallengeCompleted      - FiltrationPuzzleUI
     - SaveManager                   - RepairChoiceMade                  - StrategicChoiceUI
                                     - MissionRewardAwarded              - OrionAssistantUI
                                     - CreditsTriggered
```

- **Zero Visual UI in System 1:** System 1 generates no canvas elements, dialog panels, or map widgets.
- **Live Geo Map Coordinates:** `MissionManager` implements `IMapDataProvider`, exposing real player $(X, Y, Z)$ coordinates and 10 station map marker positions to Computer 3's `LunarGeoMapUI`.
- **Dialogue Synchronization:** Milestone events trigger `ContextualConversationTrigger`, emitting `ConversationSequence` objects directly to Computer 3's `DialogueUI`.
- **Game State Persistence:** `SaveManager` serializes chapter progress, chosen repair strategy, player position, and complete resource telemetry snapshots into `PlayerPrefs`, with automatic persistence on chapter transitions.

---

## 7. Verification & Automated Test Suite Results

All three subsystem test suites were executed in Unity batchmode with zero errors:

### Phase 4 Test Suite ([`MissionPhase4Tests.cs`](file:///d:/nasa_crosswind/Assets/Outpost/System1_Gameplay/Tests/MissionPhase4Tests.cs))
- **Test 1 (Chapter & Objective Graph):** Verified all 7 chapters, 33 objectives, station coordinates, and `IMapDataProvider` metadata. **[PASS]**
- **Test 2 (Linear Progression Ch 1–4 & Apollo 13):** Verified smooth progression through Chapters 1, 2, 3, life-support failure trigger in Chapter 4, Apollo 13 puzzle resolution, and transition to Chapter 5. **[PASS]**
- **Test 3 (Multi-Pathway Strategic Repair):** Verified that Approach A (EVA), Approach B (Robotic Arm), and Approach C (Conserve Power) each execute distinct, mathematically valid resource trade-offs and transition cleanly to Chapter 6. **[PASS]**
- **Test 4 (Recovery & State Persistence):** Verified equipment re-energization, resource telemetry verification, Daniel's health check, and full JSON state save/restore via `SaveManager`. **[PASS]**
- **Test 5 (Ending Commendation & STEM Summary):** Verified Commander Elena's debrief, awarding of the "Lunar Pioneer Ribbon", generation of the 5-point STEM report, and credits event dispatch. **[PASS]**
- **Test 6 (End-to-End Fresh Browser Session Playthrough):** Simulated a complete, uninterrupted browser playthrough from initial lander touchdown to mission completion. **[PASS]**

### Full Regression Test Summary
- **Phase 2 Tests ([`ResourceSystemPhase2Tests.cs`](file:///d:/nasa_crosswind/Assets/Outpost/System1_Gameplay/Tests/ResourceSystemPhase2Tests.cs)):** 7/7 Tests Passed.
- **Phase 3 Tests ([`EmergencyPhase3Tests.cs`](file:///d:/nasa_crosswind/Assets/Outpost/System1_Gameplay/Tests/EmergencyPhase3Tests.cs)):** 4/4 Tests Passed.
- **Phase 4 Tests ([`MissionPhase4Tests.cs`](file:///d:/nasa_crosswind/Assets/Outpost/System1_Gameplay/Tests/MissionPhase4Tests.cs)):** 6/6 Tests Passed.
- **Total Test Score:** **17 / 17 Automated Tests Passed (100% Success Rate).**
