# OUTPOST: SURVIVE BEYOND EARTH
## System 1: Cross-System Integration Contracts

### 1. Integration Principles
The Three-System architecture establishes strict ownership boundaries:
- **System 1 (Lead Gameplay Programmer)** owns player movement, physics, resources, equipment, emergencies, mission progression, and save/load.
- **System 2 (Environment & 3D Assets)** owns 3D characters, environment terrain, models, materials, animations, and equipment prefabs.
- **System 3 (Interface & Audio)** owns UI, dialogue presentation, ORION display, interactive Geo Map, cinematics, and audio synthesis.

To prevent circular assembly dependencies, all cross-system contracts are defined in **`Outpost.Core.Interfaces`** within the shared `Outpost.Core` assembly. Both System 2 and System 3 reference `Outpost.Core` without needing direct dependencies on System 1's concrete implementation classes.

---

### 2. Contract Specifications

#### A. Contract with Computer 2: Character Prefabs (`ICrewCharacter`)
- **Namespace**: `Outpost.Core.Interfaces`
- **Location**: `Assets/Outpost/Core/Interfaces/ICrewCharacter.cs`
- **Owner / Implementer**: System 2 (Environment & Characters)
- **Consumer**: System 1 (Gameplay)

```csharp
namespace Outpost.Core.Interfaces
{
    public interface ICrewCharacter
    {
        CharacterId CharacterId { get; }
        Transform Transform { get; }
        Vector3 HeadPosition { get; }
        bool IsInteracting { get; set; }

        void PlayAnimation(string animationTrigger);
        void SetBadgeHighlight(bool highlighted);
        void LookAtTarget(Vector3 targetPosition);
    }
}
```

*Usage Example (System 1)*:
```csharp
// System 1 detects an astronaut character through InteractionManager
ICrewCharacter crew = hit.collider.GetComponentInParent<ICrewCharacter>();
if (crew != null)
{
    crew.LookAtTarget(playerTransform.position);
    crew.PlayAnimation("Speak");
    // System 1 dispatches dialogue through OutpostEventChannel
}
```

---

#### B. Contract with Computer 2: Station Equipment Prefabs (`IStationEquipment`)
- **Namespace**: `Outpost.Core.Interfaces`
- **Location**: `Assets/Outpost/Core/Interfaces/IStationEquipment.cs`
- **Owner / Implementer**: System 2 (Environment & Props)
- **Consumer**: System 1 (Gameplay)

```csharp
namespace Outpost.Core.Interfaces
{
    public interface IStationEquipment
    {
        StationLocationId LocationId { get; }
        string EquipmentId { get; }
        Transform Transform { get; }
        bool IsOperational { get; }
        bool HasVisualAlert { get; }

        void SetOperationalState(bool operational);
        void SetWarningVisuals(bool warningActive);
        void TriggerInteractionFeedback();
    }
}
```

*Usage Example (System 1)*:
```csharp
// When micrometeoroid damage occurs in Chapter 4:
IStationEquipment relay = FindEquipment(StationLocationId.EmergencyBreachSite);
relay.SetOperationalState(false);
relay.SetWarningVisuals(true);
```

---

#### C. Contract with Computer 3: Map Markers & Objectives (`IMapDataProvider`)
- **Namespace**: `Outpost.Core.Interfaces`
- **Location**: `Assets/Outpost/Core/Interfaces/IMapDataProvider.cs`
- **Owner / Implementer**: System 1 (`MissionManager`)
- **Consumer**: System 3 (`LunarGeoMapUI`)

```csharp
namespace Outpost.Core.Interfaces
{
    public interface IMapDataProvider
    {
        Vector3 PlayerWorldPosition { get; }
        Quaternion PlayerRotation { get; }
        StationLocationId ActiveWaypointId { get; }
        Vector3 ActiveWaypointWorldPosition { get; }
        string ActiveObjectiveDescription { get; }
        IReadOnlyList<StationLocationData> OutpostLocations { get; }

        event Action<Vector3> OnPlayerPositionUpdated;
        event Action<StationLocationId, Vector3, string> OnWaypointUpdated;
        event Action<StationLocationId> OnLocationDiscovered;

        bool IsLocationDiscovered(StationLocationId id);
        bool IsLocationAvailable(StationLocationId id);
    }
}
```

*Usage Example (System 3)*:
```csharp
// System 3's LunarGeoMapUI updates player cursor and active waypoint without touching gameplay internals:
void Update()
{
    Vector3 playerWorldPos = mapDataProvider.PlayerWorldPosition;
    Vector2 mapPos = geoMapService.WorldToMapCoordinate(playerWorldPos, mapContentRect.sizeDelta);
    playerMarkerRect.anchoredPosition = mapPos;
}
```

---

#### D. Contract with Computer 3: UI Resource Updates (`IResourceTelemetryProvider`)
- **Namespace**: `Outpost.Core.Interfaces`
- **Location**: `Assets/Outpost/Core/Interfaces/IResourceTelemetryProvider.cs`
- **Owner / Implementer**: System 1 (`ResourceManager`)
- **Consumer**: System 3 (`OutpostHUD`)

```csharp
namespace Outpost.Core.Interfaces
{
    public interface IResourceTelemetryProvider
    {
        float OxygenPercent { get; }
        float CarbonDioxidePpm { get; }
        float SolarPowerGeneratedKw { get; }
        float OutpostPowerConsumptionKw { get; }
        float BatteryChargePercent { get; }
        float WaterReserveLiters { get; }
        float GreenhouseBiomassPercent { get; }
        bool IsEmergencyActive { get; }
        string EmergencyAlertMessage { get; }

        event Action<float, float, float, float> OnTelemetryUpdated; // o2, co2, pwr, bat
        event Action<bool, string> OnEmergencyStateChanged;
    }
}
```

*Usage Example (System 3)*:
```csharp
// System 3's OutpostHUD subscribes to telemetry updates:
void OnEnable()
{
    telemetryProvider.OnTelemetryUpdated += HandleTelemetry;
}

void HandleTelemetry(float o2, float co2, float pwr, float bat)
{
    oxygenFill.fillAmount = o2 / 100f;
    co2Text.text = $"{co2:F0} ppm";
}
```

---

#### E. Contract with Computer 3: Dialogue Events (`IDialogueTrigger`)
- **Namespace**: `Outpost.Core.Interfaces`
- **Location**: `Assets/Outpost/Core/Interfaces/IDialogueTrigger.cs`
- **Owner / Consumer**: System 1 (Triggers dialogue sequences)
- **Implementer**: System 3 (`DialogueUI`)

```csharp
namespace Outpost.Core.Interfaces
{
    public interface IDialogueTrigger
    {
        string TriggerId { get; }
        void TriggerDialogue(ConversationSequence sequence);
        void TriggerSingleLine(CharacterId speaker, string text, float duration = 4.0f);
    }
}
```

---

#### F. Contract for Cinematic Triggers (`ICinematicTrigger`)
- **Namespace**: `Outpost.Core.Interfaces`
- **Location**: `Assets/Outpost/Core/Interfaces/ICinematicTrigger.cs`
- **Owner / Implementer**: System 1 (`CinematicManager`)
- **Consumer**: System 1, System 2, System 3

```csharp
namespace Outpost.Core.Interfaces
{
    public interface ICinematicTrigger
    {
        string CinematicId { get; }
        bool IsPlaying { get; }

        event Action<string> OnCinematicStarted;
        event Action<string> OnCinematicCompleted;

        void PlayCinematic(string cinematicId, Action onCompleted = null);
        void StopCinematic();
    }
}
```

---

### 3. Summary of Dependencies
- **`Outpost.Core`**: Has **zero** dependencies on System 1, System 2, or System 3.
- **`Outpost.System1.Gameplay`**: Depends only on `Outpost.Core` and `Unity.InputSystem`.
- **`Outpost.System2.Environment`**: Depends only on `Outpost.Core`.
- **`Outpost.System3.Interface`**: Depends only on `Outpost.Core`, `Unity.InputSystem`, and `UnityEngine.UI`.

No circular references exist. All integration is compile-time guaranteed and runtime validated.
