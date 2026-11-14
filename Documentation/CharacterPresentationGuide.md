# OUTPOST: SURVIVE BEYOND EARTH
## Character Presentation & Dialogue Camera Guide (Phase 2)

**Author:** System 2 (3D Environment, Characters & Asset Architecture)  
**Target Systems:** Computer 1 (Gameplay & Interaction), Computer 3 (UI & Dialogue Cameras)  
**Date:** 2045 Lunar South Pole Mission Simulation  

---

### 1. Overview & Character Identities

Four distinct astronaut characters have been modeled, textured, rigged, and prefaced for *Outpost: Survive Beyond Earth*. All characters wear standardized NASA xEMU-inspired lunar exploration suits with character-specific color accents, accessories, and distinct facial features visible through semi-transparent gold-tinted visors.

| Character | Role | Accent Color | Height | Key Visual Identifiers & Gear |
|---|---|---|---|---|
| **Commander Elena** | Mission Commander | Royal Blue (`#0D61C7`) & Gold | 1.76 m | Commander stripes, dual comm antennas on PLSS backpack, left wrist telemetry display terminal |
| **Engineer Marcus** | Systems Engineer | Industrial Orange (`#F2660D`) & Dark Titanium | 1.83 m | Utility tool belt with pouches and wrench holster, helmet-mounted dual LED work spotlights, heavy knee pads |
| **Scientist Aisha** | Lunar Scientist | Science Cyan (`#0DC2C2`) | 1.72 m | Chest-mounted rock core sample canister rack, hip-mounted lunar spectrometer scanner, high-gain sensor antenna |
| **Medical Officer Daniel** | Medical Officer | Medical Green (`#14AD52`) & White | 1.78 m | Thigh-mounted trauma first-aid kit pouch, bio-telemetry life-sign status readout strip across chest & PLSS |

---

### 2. Character Prefab Hierarchy & Component Integration

Each character prefab is located under `Assets/Art/Characters/<CharacterName>/`:
- `Assets/Art/Characters/Elena/Prefab_Astronaut_Elena.prefab`
- `Assets/Art/Characters/Marcus/Prefab_Astronaut_Marcus.prefab`
- `Assets/Art/Characters/Aisha/Prefab_Astronaut_Aisha.prefab`
- `Assets/Art/Characters/Daniel/Prefab_Astronaut_Daniel.prefab`

#### Prefab Hierarchy Tree:
```text
Prefab_Astronaut_<Name>               [CapsuleCollider: H=1.84, R=0.40, Center=(0, 0.92, 0)]
├── VisualModel                       [MeshFilter, MeshRenderer (4 Materials: Suit, Accent, Visor, Face)]
├── DialogueCameraTarget              [Transform: LocalPos=(0, 1.62, 0.85), LocalRot=(0, 180, 0)]
├── LookAtTarget                      [Transform: LocalPos=(0, 1.62, 0)]
├── InteractionTrigger                [SphereCollider: R=1.80, IsTrigger=True]
└── Rig_Root                          [Transform: LocalPos=(0, 0, 0)]
    └── Hips                          [LocalPos=(0, 0.94, 0)]
        ├── Spine                     [LocalPos=(0, 0.16, 0)]
        │   └── Chest                 [LocalPos=(0, 0.18, 0)]
        │       ├── Neck              [LocalPos=(0, 0.16, 0)]
        │       │   └── Head          [LocalPos=(0, 0.16, 0)]
        │       ├── LeftArm           [LocalPos=(-0.28, 0.10, 0)]
        │       │   └── LeftForearm   [LocalPos=(0, -0.24, 0)]
        │       └── RightArm          [LocalPos=(0.28, 0.10, 0)]
        │           └── RightForearm  [LocalPos=(0, -0.24, 0)]
        ├── LeftLeg                   [LocalPos=(-0.16, -0.10, 0)]
        └── RightLeg                  [LocalPos=(0.16, -0.10, 0)]
```

---

### 3. Dialogue Camera Framing (For Computer 3 - Interface & Dialogue)

For chapter briefings, emergency discussions, and character conversations (Chapters 1–7):

1. **Camera Position & Framing:**
   - Anchor your dialogue camera or Cinemachine Virtual Camera to `DialogueCameraTarget`.
   - `DialogueCameraTarget` is situated at eye level ($Y = 1.62\text{ m}$), $0.85\text{ m}$ in front of the astronaut, with a local $180^\circ$ rotation pointing directly into the helmet visor.
   - Recommended Field of View (FOV): $36^\circ$ to $42^\circ$ for a cinematic medium close-up (bust shot) showing the faceplate, mission insignia, and chest status readout.
2. **LookAt Target:**
   - Point camera orientation / aim constraints to `LookAtTarget` ($Y = 1.62\text{ m}$, center of head).
3. **Lighting Note:**
   - The visor material (`M_Astronaut_Visor`) uses URP Lit with smooth specular reflectance ($0.95$) and metallic reflection ($0.85$) while maintaining semi-transparency ($\alpha = 0.55$). A key light positioned at $30^\circ$ to $45^\circ$ elevation illuminates the facial features inside without obscuring glare.

---

### 4. Gameplay & Interaction Integration (For Computer 1 - Gameplay)

1. **Player Interaction Trigger:**
   - `InteractionTrigger` contains a `SphereCollider` with radius $1.8\text{ m}$ set as `IsTrigger = true`.
   - When the player enters this volume, Computer 1's HUD system should display the contextual prompt:
     - Elena: *"Press [E] to speak with Commander Elena"*
     - Marcus: *"Press [E] to consult Engineer Marcus"*
     - Aisha: *"Press [E] to review scientific data with Dr. Aisha"*
     - Daniel: *"Press [E] to check crew vitals with Dr. Daniel"*
2. **Animation / Mecanim Readiness:**
   - The `Rig_Root` hierarchy maps 1:1 to standard humanoid joint chains. You can attach an `Animator` component with a Humanoid or Generic avatar configuration for idle breathing, gesturing, or walking animations.
3. **Physics Collision:**
   - The root GameObject contains a calibrated `CapsuleCollider` ($H = 1.84\text{ m}$, $R = 0.40\text{ m}$) ensuring proper physical collisions with the lunar terrain, station floors, and obstacles.
