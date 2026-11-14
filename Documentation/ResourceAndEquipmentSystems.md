# SYSTEM 1: PHASE 2 – LUNAR RESOURCE AND EQUIPMENT SYSTEMS
## Technical Specifications & Integration Guide

**Document Version:** 1.0.0  
**Target Environment:** Unity 6 (6000.6.9f1) / URP / WebGL  
**Assembly Definitions:** `Outpost.Core`, `Outpost.System1.Gameplay`, `Outpost.System1.Gameplay.Editor`  
**System Lead:** Lead Gameplay Programmer (System 1)

---

## 1. System Overview & Architecture

Phase 2 implements the central closed-loop life support, electrical grid, resource simulation, equipment communication architecture, and contextual dialogue integration for *Outpost: Survive Beyond Earth* (Mission 1 – Lunar South Pole).

### Subsystem Architecture Diagram
```
+-----------------------------------------------------------------------------------------+
|                                      Outpost.Core                                       |
|  - Outpost.Core.Data (ResourceStateData, ResourceCategory, GeneratorStatus, etc.)      |
|  - Outpost.Core.Interfaces (IStationEquipment, IResourceTelemetryProvider, etc.)        |
|  - OutpostEventChannel (TelemetryUpdated, DetailedTelemetryUpdated, Alerts, Dialogue)   |
+-----------------------------------------------------------------------------------------+
            ^                                                                 ^
            | references                                                      | references
+---------------------------------------------+   +---------------------------------------+
|          Outpost.System1.Gameplay           |   |       Outpost.System3.Interface       |
|  - ResourceManager (Central Life Support)   |   |  - OutpostHUD (Gauges, Alerts)        |
|  - ResourceSystemConfig (ScriptableObject)  |   |  - DialogueUI (Conversation Player)   |
|  - EquipmentManager (Registry & Inventory)  |   |  - LunarGeoMapUI (Facility Status)    |
|  - StationEquipmentBase & Concrete Prefabs  |   |  - OrionAssistantUI (Advisories)      |
|  - ContextualConversationTrigger            |   +---------------------------------------+
|  - SaveManager (Persistence Integration)    |
+---------------------------------------------+
```

---

## 2. Resource Specifications & Simulation Dynamics

### Tick-Based Simulation Model
To prevent per-frame `Update()` polling and maintain high WebGL frame rates:
- The simulation runs on a configurable discrete tick interval (`SimulationTickInterval = 1.0f`).
- Frame updates only increment an accumulator timer; calculations execute strictly once per tick.
- Event dispatching filters notifications and threshold changes to eliminate redundant message spam.

### Detailed Resource Loops

#### A. Oxygen & Atmosphere
- **Production:**
  - `OxygenGeneratorEquipment`: Sabatier reactor / water electrolysis cell ($0.20\text{ L/s}$ nominal $\approx 12\text{ L/min}$).
  - `GreenhouseHydroponicsEquipment`: Supplemental photosynthesis production ($+0.04\text{ L/s}$).
- **Consumption:**
  - 4 astronauts consuming $0.04\text{ L/s}$ each ($0.16\text{ L/s}$ total $\approx 0.84\text{ kg O}_2\text{/person/day}$).
- **Storage:**
  - High-pressure cryogenic tanks ($5,000\text{ L}$ capacity), clamping between $0\text{ L}$ and $5,000\text{ L}$ ($0$ to $100\%$).
- **Carbon Dioxide Dynamics:**
  - Crew respiration adds $+3.5\text{ ppm/s}$ of CO2.
  - `CarbonDioxideScrubberEquipment` removes $-20.0\text{ ppm/s}$ when powered and fitted with LiOH canisters.
  - In emergency (Chapter 4), scrubber damage causes CO2 to climb to toxic levels until the emergency adapter is fitted (Chapter 5).
- **Generator Status:**
  - Enumeration: `Offline`, `Nominal`, `Degraded`, `Overheating`, `Fault`.

#### B. Water Loop
- **Storage:**
  - Potable water tanks ($1,000\text{ L}$ maximum capacity, $850\text{ L}$ initial).
- **Consumption:**
  - Crew metabolic hydration and hygiene ($0.08\text{ L/s}$ total).
  - Greenhouse hydroponic irrigation ($0.04\text{ L/s}$).
- **Recycling:**
  - `WaterRecyclingEquipment` (WRS) provides $0.11\text{ L/s}$ output with $92\%$ closed-loop recovery efficiency.

#### C. Food & Greenhouse Biomass
- **Storage:**
  - Pantry rations ($120$ initial meals, maximum $200$ rations).
- **Consumption:**
  - Crew food consumption ($0.04\text{ rations/s}$).
- **Production:**
  - Hydroponic growth beds produce $+0.05\text{ rations/s}$ when powered and irrigated.
  - Biomass health ($0$ to $100\%$) reflects crop vigor; withers if water reserve drops to $0$.

#### D. Electricity & Power Grid
- **Generation:**
  - `SolarArrayEquipment`: $55.0\text{ kW}$ nominal output at the lunar south pole ridge.
  - Emergency damaged generation: drops to $8.0\text{ kW}$ trickle.
- **Consumption:**
  - Baseload ($18.0\text{ kW}$) + active equipment power draw calculated dynamically by `EquipmentManager.GetTotalActivePowerDrawKw()`.
- **Battery Storage:**
  - Regenerative fuel cell / Li-ion bank ($250.0\text{ kWh}$ capacity, $95\%$ initial).
  - Surplus solar power charges batteries; deficits discharge batteries.
- **Power Distribution & Load Shedding:**
  - Statuses: `Nominal`, `BatteryDraw`, `LoadShedding`, `Brownout`, `Blackout`.
  - When battery depletes, `EquipmentManager.ApplyLoadShedding(PowerPriority.WaterAndGreenhouse)` sheds lower-priority loads (`ScienceAndAuxiliary`, `Communications`, `WaterAndGreenhouse`) to protect `CriticalLifeSupport`.

#### E. Other Systems
- **Spare Parts:** Discrete inventory ($15$ units) used for mechanical and electrical repairs.
- **Crew Health:**
  - $0$ to $100\%$. Degraded by atmospheric hypoxia ($\text{O}_2 < 75\%$), CO2 toxicity ($> 1200\text{ ppm}$), water starvation, or radiation exposure.
  - Gradually recovers when all life support parameters return to nominal.
- **Equipment Condition:**
  - Individual condition tracked per prefab ($0$ to $100\%$). Outpost average exposed via telemetry.
- **Radiation Protection:**
  - Regolith berms and electromagnetic deflection coils attenuate $95\%$ of cosmic rays and solar storm flux.

---

## 3. Equipment Interface Contracts (`IStationEquipment`)

### Interface Definition (`Outpost.Core.Interfaces`)
```csharp
public interface IStationEquipment
{
    StationLocationId LocationId { get; }
    string EquipmentId { get; }
    string DisplayName { get; }
    Transform Transform { get; }
    bool IsOperational { get; }
    bool IsPowered { get; }
    bool HasVisualAlert { get; }
    float ConditionPercent { get; }
    float PowerDrawKw { get; }
    PowerPriority Priority { get; }

    void SetOperationalState(bool operational);
    void SetPoweredState(bool powered);
    void SetWarningVisuals(bool warningActive);
    void Repair(float conditionGain);
    void Damage(float conditionLoss);
    void TriggerInteractionFeedback();

    event Action<IStationEquipment> OnEquipmentStateChanged;
}
```

### Implementing Equipment Prefabs (Computer 2 Guide)
All 3D equipment prefabs should attach `StationEquipmentBase` or one of its concrete subclasses:
1. `SolarArrayEquipment`: Photovoltaic array tracking sun, generates power.
2. `OxygenGeneratorEquipment`: Sabatier electrolysis generator producing oxygen.
3. `CarbonDioxideScrubberEquipment`: LiOH scrubber removing CO2.
4. `WaterRecyclingEquipment`: Greywater recovery loop.
5. `GreenhouseHydroponicsEquipment`: Dwarf wheat and salad crop arrays.
6. `BatteryBankEquipment`: Central energy storage cells.
7. `RadiationShieldEquipment`: Active deflection coils.

Every equipment component automatically registers with `EquipmentManager` upon `OnEnable()` and unregisters on `OnDisable()`.

---

## 4. Contextual Conversation Integration

The gameplay system prepares and broadcasts multi-line crew dialogues using `ConversationSequence` via `OutpostEventChannel.RaiseConversationSequencePrompted`:

| Trigger Condition | Primary Participants | Educational & Story Purpose |
|---|---|---|
| **Solar Activation** | Elena, Marcus, ORION | Details lunar south pole eternal light and 55 kW power generation. |
| **Electrical Output Report** | Elena, Marcus | Reviews power budget, baseload margins, and battery reserves. |
| **Greenhouse Science** | Elena, Aisha, Marcus | Explains closed-loop biology, plant photosynthesis, and crew mental wellness. |
| **Water Importance** | Daniel, Marcus | Explains low-gravity cardiovascular fluid shifts and 92% WRS recycling. |

Zero visual UI is rendered in System 1; Computer 3's `DialogueUI` listens to `ConversationSequencePrompted` and displays the conversation.

---

## 5. State Persistence (Save & Restore)

`SaveManager` serializes the full `ResourceStateData` object into `OutpostSaveModel`:
- **Save:** `ResourceManager.Instance.ExportState()` creates an isolated clone of all resource levels, battery charge, equipment wear, and generator status.
- **Restore:** `ResourceManager.Instance.ImportState(model.ResourceState)` instantly restores all meters and emits telemetry events.
- **Storage:** Persisted safely in browser-compatible `PlayerPrefs` JSON format.
