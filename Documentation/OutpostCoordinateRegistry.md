# OUTPOST: SURVIVE BEYOND EARTH
## Mission 1: Outpost World Coordinate & Location Anchor Registry
**Environment:** Lunar South Pole (Shackleton Crater Rim Sector)  
**System Owner:** SYSTEM 2 (3D Environment & World Layout)  
**Target Systems:** SYSTEM 1 (Gameplay / Objectives) & SYSTEM 3 (Interface / Interactive Geo Map)  
**Datum Point (0, 0, 0):** Central Air-lock / Ground Hub of the Main Habitat Dome  
**Coordinate Orientation:** Unity Standard (+$X$ = East, +$Y$ = Elevation/Up, +$Z$ = North)  
**Document Version:** 1.1.0 (Calibrated Ground Elevations & Verified Navigable Paths)  

---

### 1. Master Anchor Registry Table (Calibrated Phase 4)

The coordinates below correspond to exact transforms in `System2_LunarTerrainTest.unity` and the master integration scene. All interactive triggers, waypoints on the Geo Map, and camera cinematics align with these values:

| Station / Landmark | Station Type Enum | Unity Vector3 ($X, Y, Z$) | Zone Radius | Elevation / Description |
| :--- | :--- | :--- | :--- | :--- |
| **Main Habitat** | `MainHabitat` | `(0.0, 0.0, 0.0)` | $22.0\text{m}$ | Central inflatable command dome, life-support hub, and airlock. |
| **Landing Zone** | `LandingZone` | `(0.0, 0.04, -80.0)` | $20.0\text{m}$ | Leveled circular landing pad ($r=18\text{m}$) with 4 cyan solar beacons. |
| **Solar Power Farm** | `SolarFarm` | `(75.0, 4.26, 20.0)` | $18.0\text{m}$ | Elevated eastern sun-tracking ridge maximizing solar incidence. |
| **Greenhouse Biodome** | `Greenhouse` | `(-45.0, -1.97, 35.0)` | $16.0\text{m}$ | Western hydroponic farming biodome seated on crater terrace. |
| **Research Laboratory** | `ResearchLab` | `(40.0, 0.21, 45.0)` | $16.0\text{m}$ | Science module with geological core analyzers and airlock dock. |
| **Communication Tower** | `CommTower` | `(60.0, 11.18, -40.0)` | $14.0\text{m}$ | High southern ridge crest offering direct line-of-sight to Earth. |
| **Rover Station** | `RoverStation` | `(-35.0, 0.0, -30.0)` | $15.0\text{m}$ | NASA SEV planetary rover parking, charging umbilical, and ramp. |
| **Geological Research Area** | `GeologicalResearchArea` | `(-80.0, 1.61, 70.0)` | $25.0\text{m}$ | NW crater rim boulder field containing collectable mineral rocks. |
| **Equipment Storage Depot** | `EquipmentStorage` | `(25.0, -0.38, -45.0)` | $14.0\text{m}$ | Pressurized spare parts modules, replacement filter storage. |
| **Emergency Power Substation** | `EmergencySubstation` | `(50.0, 0.35, 10.0)` | $12.0\text{m}$ | Solar power conduit junction box (location of Chapter 3/4 incident). |

---

### 2. Emergency Repair & Incident Locations (Phase 3 & 4)

| Emergency Asset | Unity Vector3 ($X, Y, Z$) | Host Facility | Storyline Role |
| :--- | :--- | :--- | :--- |
| **Solar Panel Impact Damage** | `(72.0, 4.30, 18.0)` | Solar Farm Ridge | Chapter 3: Micrometeoroid strike, power loss |
| **Electrical Junction Damage** | `(50.0, 0.35, 10.0)` | Substation | Chapter 3 & 6: Blown breakers, sparking bypass |
| **Oxygen Filter Bay Fault** | `(48.0, 0.35, 10.0)` | Substation / ECLSS | Chapter 4 & 5: Clogged filter bay, scrubber repair |
| **Water Recycler Valve Rupture** | `(50.0, 0.26, 6.0)` | Substation / ECLSS | Chapter 4: Overpressure rupture, vapor leak |
| **Damaged Rover Wheel** | `(-33.0, 0.00, -28.0)` | Rover Station Berth | Chapter 3/6: Impact debris puncture, wheel replacement |
| **External Hull Panel Breach** | `(9.4, 1.50, 0.0)` | Habitat Outer Flank | Chapter 3: Surface hull perforation, vacuum patch |
| **Emergency Hazard Beacon (Airlock)** | `(0.0, 3.20, -9.5)` | Airlock Roof | Chapters 3–6: Station red/amber emergency alert |
| **Emergency Hazard Beacon (Substation)** | `(50.0, 2.50, 10.0)` | Substation Mast | Chapters 3–6: Substation hazard strobe |
| **Warning Console (Habitat)** | `(0.0, 0.00, 2.5)` | Habitat Interior | Chapters 3–7: Visual 3-tier status alarm terminal |
| **Warning Console (Substation)** | `(50.0, 0.35, 8.5)` | Substation | Chapters 3–7: Field breaker status terminal |

---

### 3. Dialogue & Cinematics Staging Locations

| Staging Unit | Unity Vector3 ($X, Y, Z$) | Location | Staging Type |
| :--- | :--- | :--- | :--- |
| **Staging_CrewBriefing_Habitat** | `(0.0, 0.0, 0.0)` | Habitat Interior Command Deck | 5-person briefing (Commander + 4 crew) with wide & focus cams |
| **Staging_TwoAstronautConversation**| `(0.0, 0.0, -18.0)` | Airlock Plaza | Face-to-face 2-person dialogue with over-the-shoulder cameras |

---

### 4. Terrain Topography & Natural Landmarks

```
                       LUNAR SOUTH POLE SECTOR MAP
                       
                                  [NORTH]
                                     |
                          (-80, 70)  |  (15, 65)
                         [GEOLOGICAL] |  [CRATER S5]
                       CRATER (r=24m)|
                                     |
         (-60, 40)                   |                 (40, 45)
      [MEDIUM CRATER 1]              |             [RESEARCH LAB]
                                     |
                (-45, 35)            |
              [GREENHOUSE]           |       (50, 10)         (75, 20)
             (Terrace -1.97m)        |      [SUBSTATION]    [SOLAR FARM]
                                     |      (Elev. +0.35m)  (Elev. +4.26m)
   [WEST] ----------------------- (0, 0) ----------------------- [EAST]
                                [MAIN HAB]
                 (-35, -30)          |
               [ROVER STATION]       |
                                     |
                                     |       (25, -45)        (60, -40)
                                     |      [STORAGE]       [COMM TOWER]
                                     |      (Elev. -0.38m)  (Elev. +11.18m)
                                     |
                                 (0, -80)
                              [LANDING ZONE]
                                (Disc Pad)
                                     |
                                     |       (50, -60)
                                     |    [MEDIUM CRATER 2]
                                     |
                                  [SOUTH]
```

#### A. Playable Radius & Boundaries
- **Core Base Perimeter:** Spans from $Z = -100\text{m}$ to $Z = +90\text{m}$, and $X = -100\text{m}$ to $X = +90\text{m}$.
- **Natural Boundary Amphitheater:** Beyond $r = 92\text{m}$ from the origin, the Shackleton crater rim walls rise progressively from $0\text{m}$ to $+36.6\text{m}$, presenting steep impassable scree slopes.
- **Physical Barrier Colliders:** An octagonal ring of invisible wall colliders sits at $r = 132\text{m}$ with a height of $60\text{m}$ to prevent accidental player or vehicle falloff into the void.

---

### 5. Verified Navigable Routes & Slopes

Automated raycast and height sampling confirms the following slopes between all base nodes:

| Route | Distance | Max Grade | Status | Notes |
| :--- | :--- | :--- | :--- | :--- |
| **Landing Zone $\rightarrow$ Main Habitat** | $65.0\text{m}$ | $6.1\%$ | **Navigable** | Smooth, leveled regolith runway |
| **Main Habitat $\rightarrow$ Rover Station** | $46.1\text{m}$ | $0.5\%$ | **Navigable** | Level apron |
| **Main Habitat $\rightarrow$ Substation** | $51.0\text{m}$ | $4.9\%$ | **Navigable** | Flat conduit trench |
| **Substation $\rightarrow$ Solar Farm** | $26.9\text{m}$ | $27.3\%$ | **Navigable** | Gentle ridge approach ramp |
| **Main Habitat $\rightarrow$ Research Lab** | $60.2\text{m}$ | $7.3\%$ | **Navigable** | Broad northern pathway |
| **Main Habitat $\rightarrow$ Storage Depot** | $51.5\text{m}$ | $3.9\%$ | **Navigable** | Direct southern transit lane |
| **Main Habitat $\rightarrow$ Greenhouse** | $57.0\text{m}$ | $31.2\%$ | **Navigable (Ramped)** | Follows south rim terrace into crater depression |
| **Greenhouse $\rightarrow$ Geological Area** | $49.5\text{m}$ | $28.0\%$ | **Navigable (Contour)** | Traverses rim bench avoiding central boulder crag |
| **Storage Depot $\rightarrow$ Comm Tower** | $35.4\text{m}$ | $26.5\%$ | **Navigable (Switchback)**| Switchback path ascending southern ridge to summit |

---

### 6. Integration Guidelines for System 1 & System 3

#### For SYSTEM 1 (Gameplay & Objectives):
1. **Player Spawn Point:** Set player starting transform at the Landing Zone: `(0.0, 0.08, -75.0)`, facing North toward the Main Habitat (`Rotation: (0, 0, 0)`).
2. **Rover Spawn Point:** Set NASA SEV Rover transform at the Rover Station: `(-35.0, 0.35, -30.0)`, facing `Rotation: (0, 120, 0)`.
3. **Walk-in Habitat Interior:** Player walks directly through the Airlock corridor at `(0.0, 0.0, -10.5)` through the open South door of the Main Habitat at `(0.0, 0.0, -8.8)` into the interior command deck at `(0.0, 0.0, 0.0)`.

#### For SYSTEM 3 (Interface & Lunar GEO Map):
1. **Map Calibration Ratio:**
   - Map center $(0, 0)$ corresponds directly to Unity world $(0, 0)$.
   - $1.0\text{m}$ Unity World Space = $1.0\text{m}$ Geo Map coordinate.
   - Map viewport bounds: Minimum $[-120, -120]$ to Maximum $[+120, +120]$.
2. **Anchor Color Scheme for Legend:**
   - `MainHabitat`: Cyan / Blue (`#33CCFF`)
   - `LandingZone`: Green (`#4DFF70`)
   - `SolarFarm`: Yellow (`#FFD819`)
   - `Greenhouse`: Lime (`#33FF4D`)
   - `ResearchLab`: Bright Cyan (`#19E5E5`)
   - `CommTower`: Purple (`#E566FF`)
   - `RoverStation`: Orange (`#FF8019`)
   - `GeologicalResearchArea`: Slate White (`#CCCCCC`)
   - `EmergencySubstation`: Warning Red (`#FF3333`)
