# OUTPOST: SURVIVE BEYOND EARTH
## Mission 1: Outpost World Coordinate & Location Anchor Registry
**Environment:** Lunar South Pole (Shackleton Crater Rim Sector)  
**System Owner:** SYSTEM 2 (3D Environment & World Layout)  
**Target Systems:** SYSTEM 1 (Gameplay / Objectives) & SYSTEM 3 (Interface / Interactive Geo Map)  
**Datum Point (0, 0, 0):** Central Air-lock / Ground Hub of the Main Habitat Dome  
**Coordinate Orientation:** Unity Standard (+$X$ = East, +$Y$ = Elevation/Up, +$Z$ = North)  
**Document Version:** 1.0.0  

---

### 1. Master Anchor Registry Table

The coordinates below correspond to exact transforms in `System2_LunarTerrainTest.unity` and the forthcoming master integration scene. All interactive triggers, waypoints on the Geo Map, and camera cinematics must align with these values.

| Station / Landmark | Station Type Enum | Unity Vector3 ($X, Y, Z$) | Zone Radius | Elevation / Description |
| :--- | :--- | :--- | :--- | :--- |
| **Main Habitat** | `MainHabitat` | `(0.0, 0.0, 0.0)` | $22.0\text{m}$ | Central inflatable command dome, life-support hub, and airlock. |
| **Landing Zone** | `LandingZone` | `(0.0, 0.0, -80.0)` | $20.0\text{m}$ | Leveled circular landing pad ($r=18\text{m}$) with 4 cyan solar beacons. |
| **Solar Power Farm** | `SolarFarm` | `(75.0, 4.5, 20.0)` | $18.0\text{m}$ | Elevated eastern sun-tracking ridge maximizing solar incidence. |
| **Greenhouse Biodome** | `Greenhouse` | `(-45.0, 0.0, 35.0)` | $16.0\text{m}$ | Western hydroponic farming biodome and greywater recycler link. |
| **Research Laboratory** | `ResearchLab` | `(40.0, 0.0, 45.0)` | $16.0\text{m}$ | Science module with geological core analyzers and airlock dock. |
| **Communication Tower** | `CommTower` | `(60.0, 11.5, -40.0)` | $14.0\text{m}$ | High southern ridge crest offering direct line-of-sight to Earth. |
| **Rover Station** | `RoverStation` | `(-35.0, 0.0, -30.0)` | $15.0\text{m}$ | NASA SEV planetary rover parking, charging umbilical, and ramp. |
| **Geological Research Area** | `GeologicalResearchArea` | `(-80.0, -1.5, 70.0)` | $25.0\text{m}$ | NW crater rim boulder field containing collectable mineral rocks. |
| **Equipment Storage Depot** | `EquipmentStorage` | `(25.0, 0.0, -45.0)` | $14.0\text{m}$ | Pressurized spare parts modules, replacement filter storage. |
| **Emergency Power Substation** | `EmergencySubstation` | `(50.0, 1.8, 10.0)` | $12.0\text{m}$ | Solar power conduit junction box (location of Chapter 3/4 incident). |

---

### 2. Terrain Topography & Natural Landmarks

```
                       LUNAR SOUTH POLE SECTOR MAP
                       
                                  [NORTH]
                                     |
                          (-85, 75)  |  (15, 65)
                         [GEOLOGICAL] |  [CRATER S5]
                       CRATER (r=24m)|
                                     |
         (-60, 40)                   |                 (40, 45)
      [MEDIUM CRATER 1]              |             [RESEARCH LAB]
                                     |
                (-45, 35)            |
              [GREENHOUSE]           |       (50, 10)         (75, 20)
                                     |      [SUBSTATION]    [SOLAR FARM]
   [WEST] ----------------------- (0, 0) ----------------------- [EAST]
                                [MAIN HAB]
                 (-35, -30)          |
               [ROVER STATION]       |
                                     |
                                     |       (25, -45)        (60, -40)
                                     |      [STORAGE]       [COMM TOWER]
                                     |                      (Elev. +11.5m)
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
- **Natural Boundary Amphitheater:** Beyond $r = 92\text{m}$ from the origin, the Shackleton crater rim walls rise progressively from $0\text{m}$ to $+32\text{m}$, presenting steep impassable scree slopes.
- **Physical Barrier Colliders:** An octagonal ring of invisible wall colliders sits at $r = 132\text{m}$ with a height of $60\text{m}$ to prevent accidental player or vehicle falloff into the void.

---

### 3. Integration Guidelines for System 1 & System 3

#### For SYSTEM 1 (Gameplay & Objectives):
1. **Player Spawn Point:** Set player starting transform at the Landing Zone: `(0.0, 0.5, -75.0)`, facing North toward the Main Habitat (`Rotation: (0, 0, 0)`).
2. **Rover Spawn Point:** Set NASA SEV Rover transform at the Rover Station: `(-35.0, 0.35, -30.0)`, facing `Rotation: (0, 120, 0)`.
3. **Chapter 3 Emergency Trigger:** Position the micrometeoroid impact particle effect and damaged conduit at the `EmergencySubstation`: `(50.0, 1.8, 10.0)`.
4. **Chapter 5 Filtration Challenge:** Interactive airlock filter housing is mounted on the Habitat outer airlock port at `(0.0, 1.0, 12.0)`.

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
