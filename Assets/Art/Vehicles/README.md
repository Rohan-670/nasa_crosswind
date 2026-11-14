# NASA Space Exploration Vehicle (SEV) - Crosswind Edition
**Project:** NASA Crosswind Exploration Mission  
**Asset Category:** 3D Vehicles  
**Location:** `Assets/Art/Vehicles/`

---

## 🚀 Overview

The **NASA Space Exploration Vehicle (SEV)** is a multi-mission surface rover and planetary scout engineered for lunar and Martian exploration. Designed for long-duration sorties across rugged extraterrestrial terrain, the SEV features a pressurized crew cabin, 6-wheel rocker-bogie active suspension, deployable photovoltaic solar wings, a steerable high-gain parabolic antenna, an articulated geological robotic arm, and attitude RCS thrusters for low-gravity stabilization and descent braking.

---

## 📦 Asset Contents & Directory Structure

```text
Assets/Art/Vehicles/
├── NASA_SpaceExplorationVehicle.obj       # High-detail 3D model (22 modular sub-objects)
├── NASA_SpaceExplorationVehicle.mtl       # Material definition library
├── Materials/                             # Universal Render Pipeline (URP Lit) materials
│   ├── M_SpaceVehicle_Hull.mat            # Thermal ceramic tiles & NASA mission decals
│   ├── M_SpaceVehicle_Cockpit.mat         # Panoramic observation glass & HUD displays
│   ├── M_SpaceVehicle_Chassis.mat         # Carbon-titanium spaceframe & suspension
│   ├── M_SpaceVehicle_Wheels.mat          # Rugged Martian/Lunar chevron treads & hub motors
│   ├── M_SpaceVehicle_SolarPanels.mat     # Crystalline silicon photovoltaic cells
│   └── M_SpaceVehicle_Emissive.mat        # LED floodlights & ion thruster exhaust cores
├── Textures/                              # 2048x2048 high-resolution PBR texture maps
│   ├── NASA_SEV_Albedo.png                # Base color, NASA insignias, US flag, hazard stripes
│   ├── NASA_SEV_Normal.png                # Tangent-space normal map (panel seams, bolts, treads)
│   ├── NASA_SEV_MetallicSmoothness.png    # Channel-packed (R: Metallic, A: Smoothness)
│   └── NASA_SEV_Emission.png              # Glowing headlights, cyan cockpit HUD, thrusters
├── Prefabs/
│   └── NASA_SpaceExplorationVehicle.prefab # Ready-to-spawn prefab with physics & lights
└── Scripts/
    ├── SpaceVehicleController.cs          # 6WD drive, suspension, thrusters, lights controller
    └── Editor/
        └── SpaceVehicleSetupEditor.cs     # 1-click Editor menu to setup and spawn vehicle
```

---

## 🛠️ 3D Model Specifications

- **Dimensions:** Length: 5.2m | Width: 3.6m | Height: 2.6m (3.4m with antenna/sensors)
- **Scale:** Real-world metric (1 unit = 1 meter)
- **Geometry Format:** Wavefront `.obj` (Triangles & Quads with smooth vertex normals and UV coordinates)
- **Modular Sub-Objects:**
  1. `Cabin_Hull` — Pressurized crew module with chamfered aerodynamic walls and rear EVA airlock collar.
  2. `Cockpit_Canopy` — Panoramic observation windscreen with interior flight computer displays.
  3. `Cockpit_Frame` — Titanium roll-cage struts.
  4. `Chassis_Frame` — Lower longitudinal box beams, cross-braces, and armored skid plate.
  5. `Suspension_Arms` — 6 Rocker-bogie wishbones, pivots, and hydraulic shock dampers.
  6. `Wheel_Front_Left`, `Wheel_Front_Right` — Steerable front rover wheels with chevron treads.
  7. `Wheel_Mid_Left`, `Wheel_Mid_Right` — Heavy-duty middle drive wheels.
  8. `Wheel_Rear_Left`, `Wheel_Rear_Right` — Counter-steering rear drive wheels.
  9. `Solar_Wing_Left`, `Solar_Wing_Right` — Deployable solar arrays with gold MLI backing.
  10. `Solar_Wing_Left_Gimbal`, `Solar_Wing_Right_Gimbal` — Articulated mounting brackets.
  11. `Antenna_HighGain` — 0.52m steerable parabolic reflector dish and gold sub-reflector feed horn.
  12. `Sensor_Mast` — Pan-tilt mast with stereoscopic navigation optics and LIDAR scanner dome.
  13. `Robotic_Arm` — Articulated 3-segment manipulator with rock core drill and sample claws.
  14. `RTG_Generator` — Radioisotope Thermoelectric Generator with 8 radial cooling radiator fins.
  15. `RCS_Thrusters` — 4 Quad attitude control blocks with gold foil wraps.
  16. `Main_Descent_Thrusters` — Dual divergent rocket bells for high-thrust soft landing.
  17. `Headlights` — Forward LED searchlight housings and driving spot beams.

---

## 🎮 Interactive Controls (`SpaceVehicleController.cs`)

| Key / Input | Action | Description |
|:---|:---|:---|
| **W / S** (Vertical) | Throttle / Reverse | 6-wheel electric drive torque with realistic acceleration curve |
| **A / D** (Horizontal) | Steering / Turn | Front and rear wheel Ackermann steering + low-speed skid pivot |
| **Space** | VTOL Descent Thrusters | Fires under-chassis rocket bells for soft landings and crater hops |
| **L** | Toggle Floodlights | Turns forward LED high-beam searchlights on and off |
| **X** | Solar Array Deploy / Retract | Deploys solar array wings to generate power or folds them |
| **A / D + W / S (In Air)** | 3-Axis RCS Attitude | Stabilizes vehicle pitch and roll while airborne in low gravity |

---

## ⚡ Unity Integration & Editor Tools

1. **One-Click Scene Setup:**
   In the Unity Editor top menu, select:
   `NASA -> Space Vehicle -> Instantiate in Scene`
   This automatically places the vehicle in your active scene, configures materials, colliders, Rigidbody, headlights, cockpit glow, and wires up the vehicle controller.

2. **Prefab Generation:**
   To re-bake or update the prefab at any time:
   `NASA -> Space Vehicle -> Create or Update Prefab`

3. **Render Pipeline:**
   Configured out-of-the-box for **Unity 6 Universal Render Pipeline (URP 17.6+)** with full PBR shading (Albedo, Tangent Normal, Metallic-Smoothness packing, and Emission).
