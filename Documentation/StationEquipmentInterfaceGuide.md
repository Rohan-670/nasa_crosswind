# OUTPOST: SURVIVE BEYOND EARTH
## Station Equipment & Interactive Outpost Interface Guide (Phase 2)

**Author:** System 2 (3D Environment, Characters & Asset Architecture)  
**Target Systems:** Computer 1 (Gameplay, ECLSS, Interactions), Computer 3 (Interface, Cameras, Geo-Map)  
**Date:** 2045 Lunar South Pole Mission Simulation  

---

### 1. Executive Summary

System 2 has completed modeling, texturing, collider setup, and prefab assembly for all 12 required modular Lunar Outpost assets. Every interactive asset contains standardized interface transforms:
1. `InteractionPoint`: Spatial anchor where the player astronaut stands during interaction.
2. `InteractionTrigger`: Primitive `BoxCollider` (`IsTrigger = true`) triggering Computer 1's HUD prompt and interaction listener.
3. `InspectionCameraTarget`: Calibrated camera anchor framing the relevant console, valve, cartridge dock, or hatch for close-up inspection and mission minigames.

---

### 2. Outpost Asset Directory & Prefab Index

All prefabs are located under `Assets/Art/Prefabs/Outpost/`:

| Asset # | Outpost Asset | Prefab Path | World Anchor / Coordinate | Primary Gameplay Role |
|---|---|---|---|---|
| **1** | **Main Habitat Exterior** | `Assets/Art/Prefabs/Outpost/Prefab_MainHabitat_Exterior.prefab` | `MainHabitat` `(0, 0, 0)` | Inflatable dome shelter, central station hub |
| **2** | **Habitat Interior** | `Assets/Art/Prefabs/Outpost/Prefab_Habitat_Interior.prefab` | `MainHabitat` `(0, 0, 0)` | Command deck, central holographic operations table |
| **3** | **Airlock Module** | `Assets/Art/Prefabs/Outpost/Prefab_Airlock_Module.prefab` | `(0, 0, -10.5)` | Station egress / ingress, suit depressurization |
| **4** | **Solar Farm Array** | `Assets/Art/Prefabs/Outpost/Prefab_SolarFarm_Array.prefab` | `SolarFarm` `(75, 4.5, 20)` | Photovoltaic generation, inverter junction |
| **5** | **Battery Storage Unit** | `Assets/Art/Prefabs/Outpost/Prefab_BatteryStorage_Unit.prefab` | `EmergencySubstation` `(55, 2.2, 14)` | Power reserves, master circuit breaker (Ch. 3, 6) |
| **6** | **Oxygen Generator (ECLSS)** | `Assets/Art/Prefabs/Outpost/Prefab_OxygenGenerator_ECLSS.prefab` | `EmergencySubstation` `(48, 1.8, 10)` | Sabatier electrolysis, filter replacement (Ch. 4, 5) |
| **7** | **Water Recycler (ECLSS)** | `Assets/Art/Prefabs/Outpost/Prefab_WaterRecycler_ECLSS.prefab` | `EmergencySubstation` `(50, 1.8, 6)` | Closed-loop filtration, water distribution |
| **8** | **Greenhouse Dome** | `Assets/Art/Prefabs/Outpost/Prefab_Greenhouse_Dome.prefab` | `Greenhouse` `(-45, 0, 35)` | Hydroponic crop beds, LED lighting, food growth |
| **9** | **Research Laboratory** | `Assets/Art/Prefabs/Outpost/Prefab_ResearchLab_Module.prefab` | `ResearchLab` `(40, 0, 45)` | Geology mass spectrometry, sample airlock |
| **10** | **Communication Tower** | `Assets/Art/Prefabs/Outpost/Prefab_CommTower_Antenna.prefab` | `CommTower` `(60, 11.5, -40)` | High-gain Earth link, microwave telemetry dish |
| **11** | **Rover Station & Gantry** | `Assets/Art/Prefabs/Outpost/Prefab_RoverStation_Gantry.prefab` | `RoverStation` `(-35, 0, -30)` | Service berth, charging boom, maintenance ramp |
| **12** | **Storage Facility Depot** | `Assets/Art/Prefabs/Outpost/Prefab_StorageFacility_Depot.prefab` | `EquipmentStorage` `(25, 0, -45)` | Cargo airlock door, emergency spare parts |

---

### 3. Interactive Component Specification (For Computer 1 - Gameplay)

Each prefab root possesses a dedicated physical collider preventing players from clipping into equipment, as well as distinct child transforms:

#### A. Oxygen Generator (`Prefab_OxygenGenerator_ECLSS`) - Critical for Chapter 4 & 5
- **Interaction Point:** Local `(0, 0, -1.6)` (Facing the front maintenance access hatch).
- **Inspection Camera Target:** Local `(0, 1.3, -0.6)`, Rot `(0, 180, 0)` (Framed directly on the removable filter cartridge slot).
- **Interaction Trigger:** Center `(0, 1.2, -1.4)`, Size `(2.0, 2.0, 1.8)`.
- **Gameplay Hook:** Chapter 5 emergency filtration repair. When the player brings the improvised CO2 scrubber cartridge, prompt:
  - *"Press [E] to install Emergency Filtration Cartridge"*

#### B. Battery Storage Unit (`Prefab_BatteryStorage_Unit`) - Critical for Chapter 3 & 6
- **Interaction Point:** Local `(2.4, 0, -2.4)` (In front of master breaker panel).
- **Inspection Camera Target:** Local `(2.4, 1.6, -1.3)`, Rot `(0, 180, 0)` (Close-up of electrical breakers).
- **Interaction Trigger:** Center `(2.4, 1.5, -2.2)`, Size `(2.2, 2.5, 2.0)`.
- **Gameplay Hook:** Chapter 3 micrometeoroid electrical fault / Chapter 6 power routing choice:
  - *"Press [E] to inspect Auxiliary Battery Distribution Panel"*

#### C. Central Habitat Command Table (`Prefab_Habitat_Interior`)
- **Interaction Point:** Local `(0, 0, 1.8)` (Facing central holographic projector).
- **Inspection Camera Target:** Local `(0, 2.2, 2.8)`, Rot `(35, 180, 0)` (Top-down oblique view of station status map).
- **Interaction Trigger:** Center `(0, 1.0, 0)`, Size `(4.0, 2.0, 4.0)`.
- **Gameplay Hook:** Chapter 1 outpost activation and chapter briefings:
  - *"Press [E] to access Station Operations Core & ORION Interface"*

#### D. Airlock Module (`Prefab_Airlock_Module`)
- **Interaction Point:** Local `(0, 0, -2.8)` (Exterior EVA hatch platform).
- **Inspection Camera Target:** Local `(0, 1.6, -1.8)`, Rot `(0, 0, 0)`.
- **Interaction Trigger:** Center `(0, 1.5, -2.6)`, Size `(2.5, 2.5, 2.0)`.
- **Gameplay Hook:** Pressurization cycle between Lunar Surface and Habitat Interior:
  - *"Press [E] to cycle Airlock decompression sequence"*

#### E. Rover Station (`Prefab_RoverStation_Gantry`) & NASA SEV Rover
- **Berth Interaction Point:** Local `(-3.1, 0, 3.0)` (Diagnostic status console).
- **Charging Umbilical Boom:** Positioned to align with the NASA SEV charging port.
- **NASA SEV Prefab:** `Assets/Art/Vehicles/Prefabs/NASA_SpaceExplorationVehicle.prefab` parked on the gantry platform at `(-35, 0.4, -30)`.

---

### 4. Rendering & Material Guidelines

All 12 assets are rendered using 6 unified PBR materials under `Assets/Art/Materials/`:
- `M_Station_Hull_Exterior.mat` (Segmented micrometeoroid whipple shielding)
- `M_Station_Interior_Deck.mat` (Non-slip structural habitat flooring)
- `M_Station_Interior_Walls.mat` (Insulated habitat partition composite)
- `M_Station_SolarPanels.mat` (High-efficiency multi-junction silicon photovoltaic array)
- `M_Station_Machinery.mat` (Titanium/alloy structural frames, conduits, consoles)
- `M_Station_GreenhouseGlass.mat` (Transparent radiation-attenuating aerogel glazing)

These materials support the **Universal Render Pipeline (URP)** SRP Batcher, guaranteeing that rendering all 12 outpost structures takes under **35 total draw calls** on WebGL.
