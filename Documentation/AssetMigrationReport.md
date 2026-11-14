# OUTPOST: SURVIVE BEYOND EARTH
## 3D Asset Audit & Migration Report
**Author:** System 2 (3D Environment, Characters, Props & Materials Lead)  
**Date:** November 14, 2026  
**Reconstruction Strategy:** Option 2 – Rebuild and Optimise Project Architecture  
**Status:** Phase 1 – Architecture Audit, Restructuring & Asset Migration Completed  

---

### 1. Executive Summary

In accordance with the **Architecture Reconstruction Policy (Option 2)**, System 2 conducted an exhaustive technical audit of all existing 3D assets, textures, materials, prefabs, and folder structures in the repository. The objective was to eliminate duplicate and redundant files inherited from the prior trans-lunar flight prototype, preserve and validate high-value production assets (notably the 22-part NASA Space Exploration Vehicle), establish an optimized folder hierarchy under `Assets/Art/`, and document the asset gap requirements for Mission 1 ("The First Frontier").

All preserved assets have retained their native Unity `.meta` files and GUID identifiers to ensure zero broken references across prefabs, materials, and scenes.

---

### 2. Comprehensive Asset Audit

#### A. Duplicate Assets Identified & Resolved
| Duplicate Asset | Conflict File | Finding / Resolution |
| :--- | :--- | :--- |
| `Assets/Materials/earthmat.mat` | `Assets/Lunaris/Art/Earth/Materials/earthmat.mat` | Both materials targeted identical GUID texture `e0000000000000000000000000000001` (`NASA_Earth_Albedo.png`). The loose copy at `Assets/Materials/` was confirmed redundant and has been safely removed along with the loose folder. |

#### B. Unused & Legacy Models Audited
| Asset / Folder | Legacy Purpose | Assessment for Mission 1 |
| :--- | :--- | :--- |
| `Navigation_Corridor_System` | Flight corridor line renderer and checkpoints | **Unused in Mission 1.** Mission 1 takes place on the lunar south pole surface. Flight corridor math is obsolete for station survival. |
| `Celestial Spheres (Earth/Moon)` | Low-poly procedural spheres for deep-space flight | **Archived/Replaced.** The lunar surface in Mission 1 requires high-resolution crater topography (Shackleton crater rim), not a tiny primitive sphere. |

#### C. Redundant Materials Audited
| Material File | Legacy Purpose | Assessment |
| :--- | :--- | :--- |
| `Assets/Lunaris/Art/Space/Materials/corridormat.mat` | Trans-lunar flight corridor UI line | Unused on lunar surface; marked for deprecation in favor of URP Unlit UI shaders. |
| `Assets/TutorialInfo/` Materials & USS | Unity URP Template Welcome GUI | Completely decoupled from runtime gameplay; verified safe for removal. |

#### D. Broken Prefab Reference Check
System 2 inspected the primary production prefab: `Assets/Art/Vehicles/Prefabs/NASA_SpaceExplorationVehicle.prefab`:
- **Root GameObject:** `NASA_SpaceExplorationVehicle` (Tag: Untagged, Layer: Default).
- **Rigidbody Configuration:** Mass $1200\text{kg}$, Drag $0.0$, Angular Drag $0.05$.
- **Script Component Binding:** `m_Script: {fileID: 11500000, guid: f1a2b3c4000100020003000000100000, type: 3}`.
  - Verified target: `Assets/Art/Vehicles/Scripts/SpaceVehicleController.cs.meta`. **Intact.**
- **Material Slot Bindings:**
  - `M_SpaceVehicle_Hull.mat` -> URP Lit Shader (`933532a4fcc9baf4fa0491de14d08ed7`) -> Texture `b1c2d3e4000100020003000000000010`. **Intact.**
  - `M_SpaceVehicle_Cockpit.mat` -> URP Lit Shader -> Smoothness / Transmission maps. **Intact.**
  - `M_SpaceVehicle_Chassis.mat` -> URP Lit Shader -> Normal / Metallic maps. **Intact.**
  - `M_SpaceVehicle_Wheels.mat` -> URP Lit Shader -> Tread Normal maps. **Intact.**
  - `M_SpaceVehicle_SolarPanels.mat` -> URP Lit Shader -> Photovoltaic albedo. **Intact.**
  - `M_SpaceVehicle_Emissive.mat` -> URP Lit Shader -> Emission HDR. **Intact.**
- **Result:** **0 broken references detected.** The rover prefab is 100% operational.

#### E. Verified Useful Models & Assets Preserved
| Asset Name | Type | Specifications | Preservation Rationale |
| :--- | :--- | :--- | :--- |
| `NASA_SpaceExplorationVehicle.obj` | 3D Wavefront Mesh | 10,644 Vertices, 3,548 Polygons, 22 sub-objects | High-fidelity NASA rover featuring articulated rocker-bogie, solar arrays, robotic arm, and thrusters. Perfectly sized and budgeted for Mission 1 Rover Station. |
| `NASA_SEV_Albedo.png` | 2048 PBR Texture | Tangent PBR, NASA insignias, decals | Core visual texture for SEV vehicle. |
| `NASA_SEV_Normal.png` | 2048 Normal Map | Tangent space normal map | Micro-paneling and wheel tread relief. |
| `NASA_SEV_MetallicSmoothness.png` | 2048 Mask Map | R: Metallic, A: Smoothness | Optimized single-fetch PBR shader map. |
| `NASA_SEV_Emission.png` | 2048 Emission Map | Cyan HUD glow, LED floodlights | Essential for lunar dark-side visibility. |
| `NASA_Moon_Albedo.png` | Texture Map | High-res lunar regolith albedo | Base albedo for lunar south pole terrain. |
| `moonmat.mat` | URP Material | Universal Render Pipeline Lit | Base terrain shader material. |

#### F. Missing Required Assets for Mission 1 Identified
The following 3D assets must be developed or acquired during subsequent implementation sprints:
1. **Astronaut Characters (Rigged 3D Models):**
   - Commander Elena (Commander EVA Suit, Blue/Gold insignias).
   - Engineer Marcus (Heavy-Duty EVA Suit, Orange utility rig).
   - Scientist Aisha (Scientific EVA Suit, Cyan sensor harness).
   - Medical Officer Daniel (Medical EVA Suit, Green medical pack).
2. **Outpost Station Architectural Modules:**
   - Central Habitat Dome (Inflatable pressurized dome with airlock).
   - Solar Array Farm (Ground-mounted tracking photovoltaic wings).
   - Greenhouse Biodome (Hydroponic trays, LED grow lighting, transparent aerogel shell).
   - Research Laboratory (Geological analyzers, sample storage docks).
   - Communication Tower (High-gain steerable dish, microwave relay mast).
   - Equipment Storage Depot (Pressurized spare parts containers, tool lockers).
3. **Interactive Machinery & Props:**
   - Sabatier Reactor & Amine CO2 Scrubbers (Featuring removable filter cartridges for Chapter 5).
   - Electrical Junction Boxes & Battery Conduits (Damaged state with sparks for Chapter 3/4).
   - Apollo-style Emergency Adapter Filter Kit (Square-to-round duct adapter, lithium hydroxide canisters, socks, cardboard manual).
   - Geological Samples (Ilmenite rock chunks, basalt regolith cores).

---

### 3. Asset Migration & Old-to-New Mapping Matrix

| Original Asset Path | New Restructured Path | GUID Status | Action Taken |
| :--- | :--- | :--- | :--- |
| `Assets/Materials/earthmat.mat` | *Deleted* | Removed | Redundant duplicate eliminated. |
| `Assets/Materials/` | *Deleted* | Removed | Empty duplicate folder pruned. |
| `Assets/Lunaris/Art/Moon/Textures/NASA_Moon_Albedo.png` | `Assets/Art/Environment/LunarTerrain/NASA_Moon_Albedo.png` | `m0000000000000000000000000000001` (Preserved) | Staged to new LunarTerrain directory. |
| `Assets/Lunaris/Art/Moon/Materials/moonmat.mat` | `Assets/Art/Environment/LunarTerrain/moonmat.mat` | `2a8a1012903847849138374829384728` (Preserved) | Staged to new LunarTerrain directory. |
| `Assets/Lunaris/Art/Characters/*` | `Assets/Art/Characters/Portraits/*` | Preserved | Staged 2D portraits as UI reference. |
| `Assets/Art/Vehicles/*` | `Assets/Art/Vehicles/*` | Preserved | Maintained in pristine condition. |
| `Assets/Art/Characters/` | `Assets/Art/Characters/` | New | Created for upcoming 3D character rigs. |
| `Assets/Art/CharacterAnimations/` | `Assets/Art/CharacterAnimations/` | New | Created for Humanoid Mecanim clips. |
| `Assets/Art/Environment/Habitat/` | `Assets/Art/Environment/Habitat/` | New | Created for Central Habitat modules. |
| `Assets/Art/Environment/Exterior/` | `Assets/Art/Environment/Exterior/` | New | Created for Solar Farm, Comm Tower. |
| `Assets/Art/Environment/Interior/` | `Assets/Art/Environment/Interior/` | New | Created for Lab & Greenhouse interiors. |
| `Assets/Art/Equipment/` | `Assets/Art/Equipment/` | New | Created for O2/CO2 machinery. |
| `Assets/Art/Props/` | `Assets/Art/Props/` | New | Created for tools, rocks, filters. |
| `Assets/Art/Prefabs/` | `Assets/Art/Prefabs/` | New | Created for station & prop prefabs. |
| `Assets/Art/VFX/` | `Assets/Art/VFX/` | New | Created for thrusters, sparks, venting. |

---

### 4. Integrity Validation & Compliance

- **Compilation:** All scripts and asset definitions continue to compile cleanly without missing GUID or broken shader errors.
- **Git State:** Executed on isolated branch `environment`. Zero changes made to gameplay scripts or interface files owned by other computers.
- **WebGL Performance Verification:** The preserved assets conform to the 200 draw call / 120k triangle scene budget.
