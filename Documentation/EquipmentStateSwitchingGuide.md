# OUTPOST: SURVIVE BEYOND EARTH
## Equipment State-Switching & Emergency Assets Guide (Phase 3)

**Author:** System 2 (3D Environment, Characters & Asset Architecture)  
**Target Systems:** Computer 1 (Gameplay & Emergency Systems), Computer 3 (UI, Alerts & Cameras)  
**Date:** 2045 Lunar South Pole Mission Simulation  

---

### 1. Overview & Architecture

To support the Mission 1 storyline (Chapter 3 micrometeoroid strike, Chapter 4 life-support emergency, and Chapter 5 Apollo 13-inspired emergency filtration repair) without overwriting base outpost assets, System 2 has built a non-destructive **Two-State Modular Hierarchy**.

Original undamaged prefabs (`Prefab_SolarFarm_Array`, `Prefab_OxygenGenerator_ECLSS`, etc.) remain intact. Specialized modular state-switch prefabs are available under `Assets/Art/Prefabs/Outpost/`:

| Damaged Equipment | State-Switching Prefab Path | Primary Storyline Role |
|---|---|---|
| **Solar Panel Impact Damage** | `Assets/Art/Prefabs/Outpost/Prefab_Emergency_SolarPanel_StateSwitch.prefab` | Chapter 3: Micrometeoroid strike, power loss |
| **Electrical Junction Damage** | `Assets/Art/Prefabs/Outpost/Prefab_Emergency_ElectricalJunction_StateSwitch.prefab` | Chapter 3 & 6: Blown breakers, sparking bypass |
| **Oxygen Generator Fault** | `Assets/Art/Prefabs/Outpost/Prefab_Emergency_OxygenFilter_StateSwitch.prefab` | Chapter 4 & 5: Clogged filter bay, scrubber repair |
| **Water Recycler Fault** | `Assets/Art/Prefabs/Outpost/Prefab_Emergency_WaterValve_StateSwitch.prefab` | Chapter 4: Overpressure rupture, vapor leak |
| **Damaged Rover Wheel** | `Assets/Art/Prefabs/Outpost/Prefab_Emergency_RoverWheel_StateSwitch.prefab` | Chapter 3/6: Impact debris puncture, wheel replacement |
| **External Hull Panel Breach** | `Assets/Art/Prefabs/Outpost/Prefab_Emergency_HullPanel_StateSwitch.prefab` | Chapter 3: Surface hull perforation, vacuum patch |
| **Emergency Hazard Beacon** | `Assets/Art/Prefabs/Outpost/Prefab_Emergency_HazardBeacon.prefab` | Chapters 3–6: Station red/amber emergency alert |
| **Warning Indicator Console** | `Assets/Art/Prefabs/Outpost/Prefab_Emergency_WarningConsole.prefab` | Chapters 3–7: Visual 3-tier status alarm terminal |

---

### 2. State-Switch Hierarchy

Every state-switch prefab is constructed with identical child conventions:
```text
Prefab_Emergency_<Name>_StateSwitch       [BoxCollider (Physical boundaries)]
├── State_Damaged                         [Active by default during emergencies]
│   ├── MeshFilter & MeshRenderer         [Damaged model with scorch, cracks, warning lights]
│   └── VFX_Mount                         [Attachment transform socket for sparks / vapor / dust]
├── State_Repaired                        [Inactive by default]
│   └── MeshFilter & MeshRenderer         [Clean repaired model with fresh patch, green OK light]
├── InteractionPoint                      [Transform: player standing coordinate]
├── InteractionTrigger                    [BoxCollider: IsTrigger=True, for E key detection]
└── InspectionCameraTarget                [Transform: close-up camera for repair minigame]
```

---

### 3. Equipment State-Switching Instructions (For Computer 1 - Gameplay)

Computer 1 does not need to destroy or instantiate new meshes at runtime. State transitions are performed simply by toggling child GameObjects:

#### Example C# State Switch Logic:
```csharp
public enum EquipmentVisualState
{
    Damaged,
    Repaired
}

public void SetEquipmentVisualState(GameObject equipmentRoot, EquipmentVisualState state)
{
    Transform damagedState = equipmentRoot.transform.Find("State_Damaged");
    Transform repairedState = equipmentRoot.transform.Find("State_Repaired");

    if (state == EquipmentVisualState.Damaged)
    {
        if (damagedState != null) damagedState.gameObject.SetActive(true);
        if (repairedState != null) repairedState.gameObject.SetActive(false);
    }
    else if (state == EquipmentVisualState.Repaired)
    {
        if (damagedState != null) damagedState.gameObject.SetActive(false);
        if (repairedState != null) repairedState.gameObject.SetActive(true);
    }
}
```

#### Detailed Interaction Triggers:
1. **Oxygen Filter Scrubber Repair (Chapter 5):**
   - Trigger: `InteractionTrigger` on `Prefab_Emergency_OxygenFilter_StateSwitch`.
   - Prompt: *"Press [E] to seat improvised CO2 scrubber cartridge"*
   - On completion: Call `SetEquipmentVisualState(go, EquipmentVisualState.Repaired)`. The saturated cartridge is replaced with the sealed adapter flange, and the status light turns from Amber to Green.
2. **Electrical Breaker Bypass (Chapter 3 / 6):**
   - Trigger: `InteractionTrigger` on `Prefab_Emergency_ElectricalJunction_StateSwitch`.
   - Prompt: *"Press [E] to engage emergency circuit breaker bypass"*
   - On completion: Toggle state to `Repaired`. Loose sparking wires are replaced with sealed heavy-duty breaker switch; light turns from Red to Green.
3. **Solar Panel Replacement (Chapter 3 / 6):**
   - Trigger: `InteractionTrigger` on `Prefab_Emergency_SolarPanel_StateSwitch`.
   - Prompt: *"Press [E] to install replacement photovoltaic array"*
   - On completion: Toggle state to `Repaired`. Shattered crater section is replaced with fresh photovoltaic grid with retention clamps.
