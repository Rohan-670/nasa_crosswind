# OUTPOST: SURVIVE BEYOND EARTH
## Mission 1: Final Prefab Catalog & Hierarchy Directory (Phase 4)

**Environment:** Lunar South Pole (Shackleton Crater Rim Sector)  
**System Owner:** SYSTEM 2 (3D Environment, Characters & Asset Architecture)  
**Target Systems:** Computer 1 (Gameplay & Systems), Computer 3 (UI & Dialogue)  
**Document Version:** 1.0.0 (Final Release)  

---

### 1. Prefab Categories Overview

All prefabs are authored as complete, self-contained assets with collision geometry, interaction sockets, camera targets, and materials.

```text
Assets/Art/Prefabs/
├── Outpost/
│   ├── Base Facilities (12 Prefabs)
│   ├── Emergency State-Switch Units (8 Prefabs)
│   └── Dialogue & Staging Units (2 Prefabs)
├── Characters/
│   └── Astronaut Crew (4 Prefabs)
└── VFX/
    └── Emergency Particle Systems (5 Prefabs)
```

---

### 2. Base Modular Outpost Facilities (`Assets/Art/Prefabs/Outpost/`)

#### 1. Main Habitat Exterior (`Prefab_MainHabitat_Exterior.prefab`)
- **Location Anchor:** `MainHabitat` `(0.0, 0.0, 0.0)`
- **Dimensions:** $18.4\text{m}$ diameter, $6.4\text{m}$ dome height.
- **Accessible Walk-in Geometry:** Utilizes 5 perimeter wall colliders (`Collider_WallNorth`, `Collider_WallEast`, `Collider_WallWest`, `Collider_WallSouthEast`, `Collider_WallSouthWest`) and roof collider. The South entrance at $Z = -8.8\text{m}$, $X \in [-2.2, 2.2]$ is completely open, connecting directly to the airlock corridor.
- **Children:** `VisualModel`, `InteractionPoint` `(0, 0, -9.8)`, `InteractionTrigger` `(0, 1.5, -9.8)`, `InspectionCameraTarget` `(0, 2.5, -8.0)`.

#### 2. Habitat Interior (`Prefab_Habitat_Interior.prefab`)
- **Location Anchor:** `MainHabitat` `(0.0, 0.0, 0.0)`
- **Dimensions:** $16.5\text{m}$ diameter, $3.2\text{m}$ ceiling height.
- **Walkable Deck:** Includes composite deck floor collider `(16.5, 0.2, 16.5)`. Interior features central holographic operations table, crew sleep berths, life-support monitor, and communication console.
- **Children:** `VisualModel`, `InteractionPoint` `(0, 0, 1.8)`, `InteractionTrigger` `(0, 1, 0)`, `InspectionCameraTarget` `(0, 2.2, 2.8)`.

#### 3. Airlock Module (`Prefab_Airlock_Module.prefab`)
- **Location Anchor:** Connected to Main Habitat at `(0.0, 0.0, -10.5)`
- **Accessible Corridor:** Features left/right wall colliders (`Collider_WallLeft`, `Collider_WallRight`), roof collider, and floor ramp collider. Central corridor ($X \in [-1.2, 1.2]$, $Y \in [0.1, 2.7]$) is completely open for seamless player/crew ingress.
- **Children:** `VisualModel`, `InteractionPoint` `(0, 0, -2.8)`, `InteractionTrigger` `(0, 1.5, -2.6)`, `InspectionCameraTarget` `(0, 1.6, -1.8)`.

#### 4. Solar Farm Array (`Prefab_SolarFarm_Array.prefab`)
- **Location Anchor:** `SolarFarm` `(75.0, 4.26, 20.0)`
- **Description:** Dual tracking photovoltaic panel mast with sun sensor.
- **Children:** `VisualModel`, `InteractionPoint` `(0, 0, -2.0)`, `InteractionTrigger` `(0, 1.2, -1.8)`, `InspectionCameraTarget` `(0, 1.8, -1.5)`.

#### 5. Battery Storage Unit (`Prefab_BatteryStorage_Unit.prefab`)
- **Location Anchor:** `EmergencySubstation` `(55.0, 0.98, 14.0)`
- **Description:** Pressurized power storage cell bank with master breaker.
- **Children:** `VisualModel`, `InteractionPoint` `(0, 0, -1.5)`, `InteractionTrigger` `(0, 1.0, -1.3)`, `InspectionCameraTarget` `(0, 1.4, -1.0)`.

#### 6. Oxygen Generator ECLSS (`Prefab_OxygenGenerator_ECLSS.prefab`)
- **Location Anchor:** `EmergencySubstation` `(48.0, 0.35, 10.0)`
- **Description:** Sabatier electrolysis and CO2 scrubber system.
- **Children:** `VisualModel`, `InteractionPoint` `(0, 0, -1.6)`, `InteractionTrigger` `(0, 1.2, -1.4)`, `InspectionCameraTarget` `(0, 1.3, -0.6)`.

#### 7. Water Recycler ECLSS (`Prefab_WaterRecycler_ECLSS.prefab`)
- **Location Anchor:** `EmergencySubstation` `(50.0, 0.26, 6.0)`
- **Description:** Closed-loop filtration module with pressure valves.
- **Children:** `VisualModel`, `InteractionPoint` `(0, 0, -1.4)`, `InteractionTrigger` `(0, 1.1, -1.2)`, `InspectionCameraTarget` `(0, 1.2, -0.8)`.

#### 8. Greenhouse Dome (`Prefab_Greenhouse_Dome.prefab`)
- **Location Anchor:** `Greenhouse` `(-45.0, -1.97, 35.0)`
- **Description:** Geodesic dome with hydroponic vertical crop racks and LED grow lighting.
- **Children:** `VisualModel`, `InteractionPoint` `(0, 0, -6.5)`, `InteractionTrigger` `(0, 1.5, -6.2)`, `InspectionCameraTarget` `(0, 2.0, -5.0)`.

#### 9. Research Laboratory (`Prefab_ResearchLab_Module.prefab`)
- **Location Anchor:** `ResearchLab` `(40.0, 0.21, 45.0)`
- **Description:** Pressurized science module with sample airlock and spectrometry terminals.
- **Children:** `VisualModel`, `InteractionPoint` `(0, 0, -3.2)`, `InteractionTrigger` `(0, 1.2, -3.0)`, `InspectionCameraTarget` `(0, 1.6, -2.2)`.

#### 10. Communication Tower (`Prefab_CommTower_Antenna.prefab`)
- **Location Anchor:** `CommTower` `(60.0, 11.18, -40.0)`
- **Description:** High southern ridge telemetry mast with Earth-aligned high-gain dish.
- **Children:** `VisualModel`, `InteractionPoint` `(0, 0, -2.2)`, `InteractionTrigger` `(0, 1.5, -2.0)`, `InspectionCameraTarget` `(0, 2.5, -3.0)`.

#### 11. Rover Station & Gantry (`Prefab_RoverStation_Gantry.prefab`)
- **Location Anchor:** `RoverStation` `(-35.0, 0.0, -30.0)`
- **Description:** Rover parking gantry with maintenance crane and charging boom.
- **Children:** `VisualModel`, `InteractionPoint` `(0, 0, -3.5)`, `InteractionTrigger` `(0, 1.5, -3.0)`, `InspectionCameraTarget` `(0, 2.2, -4.0)`.

#### 12. Storage Facility Depot (`Prefab_StorageFacility_Depot.prefab`)
- **Location Anchor:** `EquipmentStorage` `(25.0, -0.38, -45.0)`
- **Description:** Pressurized depot module for spare parts, tools, and extra filter canisters.
- **Children:** `VisualModel`, `InteractionPoint` `(0, 0, -3.8)`, `InteractionTrigger` `(0, 1.5, -3.5)`, `InspectionCameraTarget` `(0, 2.0, -2.8)`.

---

### 3. Astronaut Character Prefabs (`Assets/Art/Characters/`)

All astronaut prefabs share the unified 17-bone/anchor skeletal hierarchy and are pre-bound with `Astronaut_Controller.controller`:

```text
Prefab_Astronaut_<Name>
├── VisualModel              [MeshFilter & MeshRenderer with distinct character PBR materials]
├── DialogueCameraTarget     [Transform: Eye level (0, 1.62, 0.15) for dialogue cameras]
├── LookAtTarget             [Transform: Face forward vector (0, 1.55, 0.8)]
├── InteractionTrigger       [CapsuleCollider: IsTrigger=1, E prompt detection]
└── Rig_Root
    └── Hips
        ├── Spine -> Chest -> Neck -> Head
        ├── LeftArm -> LeftForearm
        ├── RightArm -> RightForearm
        ├── LeftLeg
        └── RightLeg
```

1. **Commander Elena** (`Assets/Art/Characters/Elena/Prefab_Astronaut_Elena.prefab`)
   - Mission Patch: Command Delta | Suit Accents: Navy Blue & Gold.
2. **Engineer Marcus** (`Assets/Art/Characters/Marcus/Prefab_Astronaut_Marcus.prefab`)
   - Mission Patch: Systems Gear | Suit Accents: High-Vis Hazard Amber/Orange.
3. **Scientist Aisha** (`Assets/Art/Characters/Aisha/Prefab_Astronaut_Aisha.prefab`)
   - Mission Patch: Atomic Orbit | Suit Accents: Teal / Science Cyan.
4. **Medical Officer Daniel** (`Assets/Art/Characters/Daniel/Prefab_Astronaut_Daniel.prefab`)
   - Mission Patch: Caduceus Cross | Suit Accents: Medical Crimson.

---

### 4. Emergency State-Switch Prefabs (`Assets/Art/Prefabs/Outpost/`)

Standardized dual-state prefabs enabling Computer 1 to switch between damaged and repaired visuals without mesh destruction:

```text
Prefab_Emergency_<Name>_StateSwitch
├── State_Damaged            [Active by default during emergency: Damaged mesh + VFX Mount]
├── State_Repaired           [Inactive by default: Clean patched mesh + Green status light]
├── InteractionPoint         [Astronaut standing coordinate]
├── InteractionTrigger       [BoxCollider: IsTrigger=1 for player interaction]
└── InspectionCameraTarget   [Target transform for close-up repair minigame camera]
```

1. `Prefab_Emergency_SolarPanel_StateSwitch.prefab`: Photovoltaic cell impact damage & titanium splice repair.
2. `Prefab_Emergency_ElectricalJunction_StateSwitch.prefab`: Blown circuit breakers & insulated bypass harness.
3. `Prefab_Emergency_OxygenFilter_StateSwitch.prefab`: Clogged scrubber filter & new cartridge insertion bay.
4. `Prefab_Emergency_WaterValve_StateSwitch.prefab`: Ruptured pressure valve & high-pressure collar clamp.
5. `Prefab_Emergency_RoverWheel_StateSwitch.prefab`: Punctured spring tire & replacement lunar wheel assembly.
6. `Prefab_Emergency_HullPanel_StateSwitch.prefab`: Micrometeoroid perforation & airtight composite patch.
7. `Prefab_Emergency_HazardBeacon.prefab`: Station hazard beacon with rotating red/amber strobe.
8. `Prefab_Emergency_WarningConsole.prefab`: Triple-status visual alarm terminal (Red/Amber/Green).

---

### 5. Dialogue & Staging Prefabs (`Assets/Art/Prefabs/Outpost/`)

1. **Two Astronaut Conversation Staging** (`Prefab_Staging_TwoAstronautConversation.prefab`)
   - Sockets: `Slot_Astronaut_A` `(-0.9, 0, 0)`, `Slot_Astronaut_B` `(0.9, 0, 0)` ($1.8\text{m}$ mutual facing).
   - Cameras: `CamTarget_OTS_A`, `CamTarget_OTS_B` (over-the-shoulder framing), `CamTarget_TwoShot` (medium profile).
2. **Habitat Crew Briefing Staging** (`Prefab_Staging_CrewBriefing_Habitat.prefab`)
   - Sockets: `BriefingAnchor_Commander` `(0, 0, -2.2)`, `BriefingAnchor_Engineer` `(-2.0, 0, -0.5)`, `BriefingAnchor_Scientist` `(2.0, 0, -0.5)`, `BriefingAnchor_Medic` `(0, 0, 1.8)`.
   - Cameras: `CameraTarget_GroupWide` `(0, 2.8, -3.8)`, `CameraTarget_CommanderFocus` `(0, 1.62, -1.0)`.

---

### 6. Emergency VFX Particle Prefabs (`Assets/Art/Prefabs/VFX/`)

1. `VFX_ElectricalSparks.prefab`: Erratic high-velocity electrical sparks for breaker faults.
2. `VFX_ImpactDust_Lunar.prefab`: Regolith vacuum ejection puff for meteorite impacts.
3. `VFX_EquipmentVaporVent.prefab`: Pressurized gas venting for valve/ECLSS leaks.
4. `VFX_EmergencyStrobe_Light.prefab`: Synchronized emergency red pulsing warning light.
5. `VFX_MalfunctionArc_Flicker.prefab`: Continuous intermittent electrical arcing and flicker.
