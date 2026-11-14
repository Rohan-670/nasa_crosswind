# OUTPOST: SURVIVE BEYOND EARTH
## 3D Asset Architecture & Performance Specification
**Target Platform:** WebGL (Modern Desktop Browsers, WebAssembly 2023, URP 17.6+)  
**Target Framerate:** 60 FPS (Minimum 30 FPS on entry-level hardware)  
**System Ownership:** SYSTEM 2 (3D Environment, Characters, Props, Materials, Prefabs)  
**Document Version:** 1.0.0  

---

### 1. Architectural Scope & Directory Hierarchy

System 2 owns all 3D assets, environmental meshes, characters, props, vehicles, textures, materials, and prefabs. The directory structure is centralized under `Assets/Art/` to maintain clean separation from System 1 (Gameplay) and System 3 (Interface), preventing merge conflicts and scene contamination.

```
Assets/Art/
├── Characters/                          # Astronaut crew models and rigs
│   ├── Elena/                          # Commander Elena (Commander Blue / Gold)
│   ├── Marcus/                         # Engineer Marcus (Industrial Safety Orange)
│   ├── Aisha/                          # Scientist Aisha (Research Cyan / Teal)
│   ├── Daniel/                         # Medical Officer Daniel (Medical Green / White)
│   └── Portraits/                      # 2D character UI visual reference textures
├── CharacterAnimations/                 # Shared Mecanim Humanoid animations
│   ├── Locomotion/                     # Lunar walk, bounding run, low-gravity leap
│   ├── Interactions/                   # Console typing, filter repair, valve turning
│   └── Emotes/                         # Briefing nod, alert/warning reaction, celebratory wave
├── Environment/                         # Lunar surface and station architectural modules
│   ├── LunarTerrain/                   # Regolith surface mesh, crater rims, boulder scatters
│   ├── Habitat/                        # Central Command Dome, airlock module, corridors
│   ├── Exterior/                       # Solar Array Farm, Comm Tower, Rover Station, Storage Depot
│   └── Interior/                       # Greenhouse hydro-trays, Research Lab consoles, Life Support racks
├── Equipment/                           # Active interactive mechanical station machinery
│   ├── LifeSupport/                    # O2 Generators, CO2 Scrubbers (Sabatier & Amine canisters)
│   ├── PowerGrid/                      # Photovoltaic tracking panels, Li-ion battery banks
│   └── ResourceRecycling/              # Greywater distillation units, atmospheric condenser
├── Props/                               # Small movable and static world dressing items
│   ├── Geological/                     # Lunar regolith core drill samples, ilmenite rocks, anorthosite basalt
│   ├── Maintenance/                    # Torque wrenches, umbilical diagnostic cables, sealing duct tape
│   └── HabitatDressing/                # Food rations, EVA sample crates, emergency oxygen tanks
├── Vehicles/                            # Planetary exploration rover systems
│   ├── NASA_SpaceExplorationVehicle.obj # NASA SEV 6WD planetary rover (22 modular sub-objects)
│   ├── Materials/                      # Chassis, Hull, Cockpit, Wheels, Solar, Emissive materials
│   ├── Textures/                       # 2048 PBR maps (Albedo, Normal, MetallicSmoothness, Emission)
│   ├── Prefabs/                        # Fully assembled NASA_SpaceExplorationVehicle prefab
│   └── Scripts/                        # SpaceVehicleController (6WD, suspension, thrusters, lights)
├── Materials/                           # Shared master PBR materials across asset classes
│   ├── StationExterior.mat             # MLI gold foil, white ceramic thermal tiles
│   ├── StationInterior.mat             # Anodized aluminum beams, polycarbonate walls
│   └── GlassTransparent.mat            # Visor, greenhouse panes, observation dome glass
├── Textures/                            # Master shared texture atlases, decals, and detail maps
│   ├── Atlases/                        # Props texture atlas (combines small station props)
│   └── Decals/                         # NASA insignias, hazard chevrons, station sector signage
├── Prefabs/                             # Assembled, collider-ready prefab templates for scenes
│   ├── Stations/                       # Modular station modules with baked LODs
│   ├── Interactables/                  # Interactive machinery containing trigger colliders
│   └── Hazards/                        # Micrometeoroid impact breaches, shorted power conduits
└── VFX/                                 # Visual particle effects and shader graphs
    ├── Thrusters/                      # Ion RCS flares, landing descent dust plumes
    ├── Atmosphere/                     # Habitat pressurization steam, CO2 venting vapor
    └── Sparks/                         # Electrical short-circuit sparks from micrometeoroid damage
```

---

### 2. Unity WebGL Performance & Budget Constraints

Running complex 3D simulation scenes inside a WebGL browser container introduces strict CPU/GPU memory, draw call, and garbage collection limitations. System 2 enforces the following technical guidelines:

#### A. Draw Call & Batching Budgets
- **Maximum Target Draw Calls:** 150 – 200 per frame.
- **SRP Batcher Compatibility:** Every custom shader and material must conform to the Universal Render Pipeline (URP) `UnityPerMaterial` CBUFFER structure to ensure 100% SRP Batcher pass rates.
- **Static Batching:** All static environment geometry (station walls, fixed solar frames, crater rocks) must be tagged `Batching Static`.
- **GPU Instancing:** Enabled on repetitive structural assets (solar panels, structural struts, perimeter beacons, regolith boulders).

#### B. Triangle & Polygon Budgets

| Asset Category | Target Tris (LOD0) | Target Tris (LOD1) | Target Tris (LOD2) | Maximum Bounds |
| :--- | :--- | :--- | :--- | :--- |
| **Astronaut Hero Characters** | 8,000 – 10,000 | 4,000 – 5,000 | 1,500 – 2,000 (Culled at 50m) | $1.0\text{m} \times 0.8\text{m} \times 1.9\text{m}$ |
| **Central Habitat Dome** | 12,000 – 18,000 | 6,000 – 8,000 | 2,500 – 3,500 | $25\text{m} \times 25\text{m} \times 8\text{m}$ |
| **Solar Array Module (x1)** | 2,000 – 3,500 | 1,000 – 1,500 | 400 – 600 (Culled at 80m) | $8\text{m} \times 4\text{m} \times 4\text{m}$ |
| **NASA SEV Planetary Rover** | 7,000 – 8,000 | 3,500 – 4,500 | 1,200 – 1,800 | $5.2\text{m} \times 3.6\text{m} \times 2.6\text{m}$ |
| **Subsystem Machine (Scrubber/O2)** | 1,500 – 3,000 | 800 – 1,200 | Culled | $1.5\text{m} \times 1.2\text{m} \times 2.0\text{m}$ |
| **Small Prop (Filter, Tool, Rock)** | 200 – 600 | 100 – 250 | Culled at 20m | $0.3\text{m} \times 0.3\text{m} \times 0.4\text{m}$ |
| **Total Visible Scene Polycount** | **< 120,000 Tris** | **< 60,000 Tris** | **< 25,000 Tris** | Whole Outpost Sector |

---

### 3. Texturing, Materials & Channel Packing Standards

To eliminate texture fetch pressure and minimize WebAssembly heap allocations, System 2 implements channel packing:

#### A. Channel-Packed Metallic-Smoothness Standard
Textures follow standard URP PBR metallic workflow:
- **Albedo Map (`_BaseMap`):** RGB Color (sRGB), Alpha is used strictly for cutout opacity (e.g., solar panel grid mesh).
- **Normal Map (`_BumpMap`):** Tangent-space Normal (Linear), 16-bit packed or BC5/DXTnm compression.
- **Metallic-Gloss Map (`_MetallicGlossMap`):**
  - **Red Channel:** Metallic value ($0.0 = \text{dielectric}, 1.0 = \text{pure metal}$).
  - **Green Channel:** Ambient Occlusion (Cavity micro-shadowing).
  - **Blue Channel:** Detail mask or unused (retained at $0.0$).
  - **Alpha Channel:** Smoothness ($0.0 = \text{rough/diffuse}, 1.0 = \text{mirror-smooth}$).
- **Emission Map (`_EmissionMap`):** RGB (Linear, HDR enabled for status LEDs, monitors, illuminated helmet visors, and thruster flares).

#### B. Texture Resolution Standards
- **Hero Vehicles & Main Habitat Exterior:** Max $2048 \times 2048$ pixels.
- **Characters & Primary Subsystems:** Max $1024 \times 1024$ pixels.
- **Props, Tools & Geological Samples:** Max $512 \times 512$ pixels.
- **Decals, Particle VFX & HUD overlays:** Max $256 \times 256$ pixels.
- **Compression Profile:** BC7 (Desktop WebGL) / ASTC 6x6 (Mobile WebGL) via Unity Texture Import overrides. Mipmaps enabled with box filtering.

---

### 4. Level of Detail (LOD) & Culling Policy

Every prefab with a triangle count exceeding 1,500 tris must incorporate a `LODGroup` component configured according to:

| LOD Level | Screen Relative Metric | Rendering Features | Shadow Casting |
| :--- | :--- | :--- | :--- |
| **LOD 0** | $100\% - 40\%$ Screen Height | Full detail mesh, normal maps, full emission | Hard & Soft Shadows |
| **LOD 1** | $40\% - 15\%$ Screen Height | $50\%$ decimation, combined draw groups | Hard Shadows Only |
| **LOD 2** | $15\% - 4\%$ Screen Height | $20\%$ decimation, basic silhouette | Shadows Disabled |
| **Culled** | $< 4\%$ Screen Height | Renderer Disabled | None |

- **Small Object Culling:** Any object smaller than 0.5m in world dimensions (loose tools, rock pebbles) is aggressively culled beyond 25 meters using camera layer culling distances.
- **Frustum Culling & Occlusion Culling:** Station interior rooms use room-portal occlusion or manual trigger culling volumes so that external lunar terrain is unrendered while the player is inside the habitat.

---

### 5. Collider & Physics Policy

Expensive physics checks must be avoided in WebGL:

1. **Strict Prohibition of Complex Dynamic MeshColliders:**  
   Dynamic actors (characters, rovers, movable crates) must NEVER utilize non-convex or high-poly `MeshCollider` components.
2. **Standard Locomotion Colliders:**
   - Astronauts: Standard Unity `CharacterController` (Radius: $0.4\text{m}$, Height: $1.85\text{m}$, Step Offset: $0.3\text{m}$).
   - SEV Rover: Low-center-of-mass compound box colliders for cabin chassis + 6 cylindrical/capsule wheel colliders.
3. **Environment Colliders:**
   - Habitat modules: Compound primitive Box and Capsule colliders.
   - Terrain Surface: Low-resolution static `MeshCollider` generated from decimated collision topology, completely separated from visual render meshes.
4. **Trigger Volumes:**
   - Station airlocks, oxygen zones, and interactive consoles use lightweight primitive Box triggers marked `IsTrigger = true` on dedicated physics layers (`Interactable`, `TriggerVolume`).

---

### 6. WebGL Mesh Import Settings

All 3D assets imported into the project must enforce the following Unity asset import parameters:
- **Model Tab:**
  - `Mesh Compression`: Medium (or High for background terrain).
  - `Read/Write Enabled`: **OFF** (critical to avoid doubling vertex memory in JavaScript/WASM heap).
  - `Optimize Mesh`: Everything (reorders vertex buffers for GPU cache efficiency).
  - `Import BlendShapes`: OFF (unless facial animation rig specifically requires them).
  - `Generate Lightmap UVs`: Enabled for static architectural pieces.
- **Rig Tab:**
  - Characters: `Humanoid` avatar definition for cross-character animation retargeting.
  - Vehicles & Mechanical Props: `Generic` or `None`.
