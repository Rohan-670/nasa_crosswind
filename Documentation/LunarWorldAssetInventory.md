# OUTPOST: SURVIVE BEYOND EARTH
## Mission 1: Lunar World Master Asset Inventory (Phase 4)

**Environment:** Lunar South Pole (Shackleton Crater Rim Sector)  
**Platform:** Unity WebGL (Universal Render Pipeline - URP 17.6.0)  
**System Owner:** SYSTEM 2 (3D Environment, Characters, VFX, Materials & Prefabs)  
**Date:** 2045 Lunar Simulation  
**Document Version:** 1.0.0 (Final Release)  

---

### 1. Executive Summary & WebGL Performance Audit

The visual world for Mission 1 has been built to meet strict browser-based WebGL constraints:

| Performance Metric | Established Target Budget | Actual World Metric | Headroom Status |
| :--- | :--- | :--- | :--- |
| **Total Scene Geometry** | $\le 120,000$ Triangles | **28,196** Triangles (All assets combined) | **76.5% Headroom** |
| **Total Vertex Count** | $\le 100,000$ Vertices | **24,240** Vertices | **75.8% Headroom** |
| **Material Reuse** | High Atlas / Shared PBR | **27 Unique Materials** across 36 models | **Optimized** |
| **Texture Memory Footprint** | $\le 64\,\text{MB}$ VRAM | **$<\,1.2\,\text{MB}$ Disk / $< 8\,\text{MB}$ VRAM** | **Exceeds Target** |
| **Draw Calls / Batches** | $\le 150$ Draw Calls | **$\sim 45 - 65$ Batches** (Static & GPU instancing) | **60+ FPS Safe** |
| **Dynamic Particle Limits** | $\le 100$ Active Particles | **$\le 30$ Max Particles** per system | **Optimized** |

---

### 2. Complete 3D Model Inventory (36 Files)

All models are authored in standard `.obj` format with normal and UV coordinates.

#### A. Astronaut Character Models (`Assets/Art/Characters/`)
| Model File | Vertices | Faces (Tris) | Associated Character | Features |
| :--- | :--- | :--- | :--- | :--- |
| `Astronaut_Elena.obj` | 1,389 | 2,408 | Commander Elena | Navy/Gold Commander trim, Command Delta patch, female proportion rig |
| `Astronaut_Marcus.obj` | 1,335 | 2,316 | Engineer Marcus | Amber/Orange utility trim, Gear insignia, heavy tool belt |
| `Astronaut_Aisha.obj` | 1,421 | 2,472 | Scientist Aisha | Teal/Cyan science trim, Atom patch, sensor harness |
| `Astronaut_Daniel.obj` | 1,311 | 2,276 | Medical Officer Daniel | Crimson medical trim, Medical Cross patch, health telemetry harness |

#### B. Modular Lunar Outpost Facilities (`Assets/Art/Environment/` & `Assets/Art/Equipment/`)
| Model File | Directory | Vertices | Faces | Primary Function |
| :--- | :--- | :--- | :--- | :--- |
| `MainHabitat_Exterior.obj` | `Environment/Habitat/` | 498 | 912 | Main inflatable central dome with perimeter airlock docks |
| `Habitat_Interior.obj` | `Environment/Habitat/` | 214 | 368 | Living quarters, command console, holographic table, crew bunks |
| `Airlock_Module.obj` | `Environment/Habitat/` | 122 | 208 | Pressurization vestibule with walk-through tunnel |
| `Greenhouse_Dome.obj` | `Environment/Exterior/` | 262 | 464 | Hydroponic agriculture biodome with transparent geodesic panels |
| `ResearchLab_Module.obj` | `Environment/Exterior/` | 58 | 92 | Geological sample analyzer science unit |
| `SolarFarm_Array.obj` | `Environment/Exterior/` | 88 | 148 | Dual-axis tracking photovoltaic sun-absorber |
| `BatteryStorage_Unit.obj` | `Environment/Exterior/` | 64 | 96 | High-capacity regenerative energy bank |
| `OxygenGenerator_ECLSS.obj` | `Equipment/` | 124 | 212 | Sabatier electrolysis and CO2 scrubber housing |
| `WaterRecycler_ECLSS.obj` | `Equipment/` | 112 | 196 | Closed-loop greywater purification filtration unit |
| `RoverStation_Gantry.obj` | `Environment/Exterior/` | 88 | 136 | Rover maintenance cradle and charging boom |
| `StorageFacility_Depot.obj` | `Environment/Exterior/` | 64 | 96 | Logistics spare parts and cartridge storage |
| `CommTower_Antenna.obj` | `Environment/Exterior/` | 147 | 228 | Earth-pointing parabolic telecommunications dish |

#### C. Planetary Terrain & Geology Models (`Assets/Art/Environment/LunarTerrain/` & `Assets/Art/Props/`)
| Model File | Directory | Vertices | Faces | Description |
| :--- | :--- | :--- | :--- | :--- |
| `LunarTerrain_SouthPole.obj` | `Environment/LunarTerrain/` | 5,041 | 9,800 | 280m x 280m Shackleton rim terrain mesh with craters and ridges |
| `LunarCrag_Large.obj` | `Props/` | 128 | 252 | Massive monolithic crater rim rock formation |
| `LunarRock_Medium.obj` | `Props/` | 86 | 168 | Medium scatter boulder for perimeter obstacle field |
| `LunarRock_Small.obj` | `Props/` | 34 | 64 | Small foreground regolith rock |
| `LunarSample_Geological.obj` | `Props/` | 42 | 80 | Collectable mineral rock sample (Anorthosite / Ilmenite) |

#### D. Surface Exploration Vehicles (`Assets/Art/Vehicles/`)
| Model File | Directory | Vertices | Faces | Description |
| :--- | :--- | :--- | :--- | :--- |
| `NASA_SpaceExplorationVehicle.obj` | `Vehicles/` | 10,644 | 3,548 | 12-wheel NASA SEV pressurized rover with pivot cockpit |

#### E. Emergency Damaged & Repaired Models (`Assets/Art/Equipment/` & `Assets/Art/Environment/Exterior/`)
| Model File | Directory | Vertices | Faces | Emergency Storyline Role |
| :--- | :--- | :--- | :--- | :--- |
| `SolarPanel_Damaged.obj` | `Environment/Exterior/` | 58 | 96 | Shattered solar cells, scorch halo, severed conduit |
| `SolarPanel_Repaired.obj` | `Environment/Exterior/` | 32 | 48 | Bolted titanium reinforcement splice, restored cells |
| `ElectricalJunction_Damaged.obj` | `Equipment/` | 70 | 116 | Charred casing, blown breakers, exposed arcing core |
| `ElectricalJunction_Repaired.obj` | `Equipment/` | 42 | 68 | Insulated wiring harness, sealed junction box, green LED |
| `OxygenFilterBay_Fault.obj` | `Equipment/` | 42 | 68 | Clogged CO2 cartridge bay, open rupture vent |
| `OxygenFilterBay_Repaired.obj` | `Equipment/` | 50 | 80 | Sealed filter bay, new cylindrical scrubber locked |
| `WaterValve_Fault.obj` | `Equipment/` | 130 | 228 | Blown pressure relief valve, ruptured flange seal |
| `WaterValve_Repaired.obj` | `Equipment/` | 134 | 240 | Bolted pressure sleeve, restored high-pressure valve |
| `RoverWheel_Damaged.obj` | `Environment/Exterior/` | 68 | 124 | Punctured wire-mesh tire, bent titanium hub |
| `RoverWheel_Repaired.obj` | `Environment/Exterior/` | 82 | 152 | Fresh lunar spring-tire and reinforced hub |
| `EquipmentPanel_Damaged.obj` | `Environment/Exterior/` | 54 | 88 | Punctured exterior micrometeoroid hull breach |
| `EquipmentPanel_Repaired.obj` | `Environment/Exterior/` | 80 | 132 | Welded airtight patch with perimeter pressure rivets |
| `EmergencyBeacon_Unit.obj` | `Equipment/` | 86 | 156 | Heavy-duty rotating strobe beacon |
| `WarningIndicator_Console.obj` | `Equipment/` | 40 | 60 | 3-tier status alarm terminal (Red / Amber / Green) |

---

### 3. PBR Materials Directory (`Assets/Art/Materials/`)

All materials use the Universal Render Pipeline standard Lit shader (`guid: 933532a4fcc9baf4fa0491de14d08ed7`) or Particles/Unlit (`guid: 0406db5a14f94604a8c57ccfbc9f3b46`):

1. `M_Astronaut_Suit_Shared.mat`: High-durability beta-cloth white fabric with microfiber weave.
2. `M_Astronaut_BootsGloves_Shared.mat`: Dark grey silicone-coated abrasion-resistant outer boots/gloves.
3. `M_Astronaut_Visor_Gold.mat`: Mirror-reflective gold sun visor coating.
4. `M_Astronaut_Trim_Elena.mat`: Navy blue and gold command markings.
5. `M_Astronaut_Trim_Marcus.mat`: High-visibility orange utility trim.
6. `M_Astronaut_Trim_Aisha.mat`: Teal/Cyan scientific spectroscopy trim.
7. `M_Astronaut_Trim_Daniel.mat`: Crimson medical chevron markings.
8. `M_Astronaut_Patch_Elena.mat`: Commander Delta mission patch.
9. `M_Astronaut_Patch_Marcus.mat`: Engineering Gear insignia patch.
10. `M_Astronaut_Patch_Aisha.mat`: Atomic orbit science patch.
11. `M_Astronaut_Patch_Daniel.mat`: Medical Cross health insignia.
12. `M_Station_Hull_Exterior.mat`: Multi-layer insulation (MLI) off-white thermal blanket.
13. `M_Station_Machinery.mat`: Anodized metallic chassis and structural titanium struts.
14. `M_Station_SolarPanels.mat`: Deep blue photovoltaic crystal with anti-reflective coating.
15. `M_Station_Interior_Deck.mat`: Ribbed composite deck plating.
16. `M_Station_Interior_Walls.mat`: Padded acoustic interior habitability panels.
17. `M_Station_Glass_Geodesic.mat`: Transparent high-strength tinted greenhouse glass.
18. `M_LunarRegolith_Ground.mat`: High-albedo lunar South Pole basalt and anorthosite regolith.
19. `M_LunarRock_Crag.mat`: Dark basaltic rock with sharp micro-craters.
20. `M_Emergency_Scorch_Damaged.mat`: Carbonized scorch marks, blackened heat damage.
21. `M_Emergency_Patched_Repaired.mat`: Fresh titanium alloy weld plate with pressure seals.
22. `M_Emergency_Warning_Red.mat`: Emissive high-intensity emergency red alarm ($12\,\text{cd/m}^2$).
23. `M_Emergency_Warning_Amber.mat`: Emissive caution amber warning light ($8\,\text{cd/m}^2$).
24. `M_Emergency_Nominal_Green.mat`: Emissive nominal status green indicator ($6\,\text{cd/m}^2$).
25. `M_VFX_SparkAdditive.mat`: Unlit additive spark particle material.
26. `M_VFX_DustSmoke.mat`: Alpha-blended dust puff and vapor leak material.
27. `M_VFX_LightGlow.mat`: Soft radial lens glow billboard material.

---

### 4. Animation Clips Directory (`Assets/Art/CharacterAnimations/`)

| Animation Clip | Duration | Loop | Bone Bindings | Context |
| :--- | :--- | :--- | :--- | :--- |
| `Anim_Astronaut_Idle.anim` | $2.0\,\text{s}$ | Yes | `Rig_Root/Hips/...` | Natural low-gravity breathing idle |
| `Anim_Astronaut_Walk.anim` | $1.2\,\text{s}$ | Yes | `Rig_Root/Hips/...` | Bounding surface gait, rhythmic arm swing |
| `Anim_Astronaut_Run.anim` | $0.8\,\text{s}$ | Yes | `Rig_Root/Hips/...` | Forward dynamic lunar jog / sprint |
| `Anim_Astronaut_Talk.anim` | $2.4\,\text{s}$ | Yes | `Rig_Root/Hips/...` | Conversational gesturing and helmet comms |
| `Anim_Astronaut_Point.anim` | $2.0\,\text{s}$ | Hold | `Rig_Root/Hips/...` | Arm raised pointing at equipment/waypoint |
| `Anim_Astronaut_Inspect.anim` | $2.2\,\text{s}$ | Yes | `Rig_Root/Hips/...` | Torso lean, telemetry diagnosis |
| `Anim_Astronaut_Repair.anim` | $1.8\,\text{s}$ | Yes | `Rig_Root/Hips/...` | Kneeling maintenance, cartridge replacement |
| `Anim_Astronaut_OperateEquipment.anim`| $1.6\,\text{s}$ | Yes | `Rig_Root/Hips/...` | Console typing, breaker switches |
| `Anim_Astronaut_ConcernedReaction.anim`| $2.0\,\text{s}$ | Once | `Rig_Root/Hips/...` | Alarmed step-back, hands raised to visor |
| `Anim_Astronaut_TeamDiscussion.anim` | $2.8\,\text{s}$ | Yes | `Rig_Root/Hips/...` | Standing face-to-face partner conversation |
| `Anim_Astronaut_SeatedDiscussion.anim`| $2.5\,\text{s}$ | Yes | `Rig_Root/Hips/...` | Seated $90^\circ$ pose at briefing table |
| `Anim_Astronaut_EquipmentDemo.anim` | $2.2\,\text{s}$ | Yes | `Rig_Root/Hips/...` | Sweeping hand gesture presenting machinery |
| `Astronaut_Controller.controller` | N/A | State Machine | Full Skeletal Rig | Parameters: `Speed`, `IsInteracting`, `ActionState` |

---

### 5. VFX Particle Systems (`Assets/Art/Prefabs/VFX/`)

| VFX Prefab | Max Particles | Rate Over Time | Lifetime | Shader |
| :--- | :--- | :--- | :--- | :--- |
| `VFX_ElectricalSparks.prefab` | 30 | 18 / sec | $0.3 - 0.6\,\text{s}$ | `URP/Particles/Unlit` (Additive) |
| `VFX_ImpactDust_Lunar.prefab` | 25 | Burst (20) | $0.8 - 1.4\,\text{s}$ | `URP/Particles/Unlit` (Alpha Blend) |
| `VFX_EquipmentVaporVent.prefab` | 20 | 12 / sec | $0.5 - 0.9\,\text{s}$ | `URP/Particles/Unlit` (Alpha Blend) |
| `VFX_EmergencyStrobe_Light.prefab` | N/A (Light) | Frequency: $1.5\,\text{Hz}$| N/A | Point Light (Red $3.5\,\text{cd/m}^2$, $r=15\text{m}$) |
| `VFX_MalfunctionArc_Flicker.prefab`| 15 | 8 / sec | $0.1 - 0.3\,\text{s}$ | `URP/Particles/Unlit` (Additive Arc) |
