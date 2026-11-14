# SYSTEM 1: PHASE 1 – PLAYER AND INTERACTION FOUNDATION
## Public Interface Specifications & Architecture Documentation

**Document Version:** 1.0.0  
**Target Environment:** Unity 6 (6000.6.9f1) / URP / WebGL  
**Assembly Definitions:** `Outpost.Core`, `Outpost.System1.Gameplay`  
**System Lead:** Lead Gameplay Programmer (System 1)

---

## 1. System Overview & Boundaries

Phase 1 establishes the foundational locomotion, camera, interaction, mission lifecycle, and map telemetry systems for *Outpost: Survive Beyond Earth* (Mission 1 – Lunar South Pole).

### Decoupled Subsystem Architecture
```
+-----------------------------------------------------------------------------------+
|                                  Outpost.Core                                     |
|  - OutpostEventChannel (InteractionPromptChanged, Telemetry, Dialogue, etc.)      |
|  - Outpost.Core.Interfaces (IInteractable, IMapDataProvider, IGameService, etc.)   |
|  - Outpost.Core.Data (FiltrationChallengeLogic, SubsystemStatus, Resources)       |
+-----------------------------------------------------------------------------------+
            ^                                                   ^
            | references                                        | references
+---------------------------------------+   +---------------------------------------+
|      Outpost.System1.Gameplay         |   |       Outpost.System3.Interface       |
|  - ThirdPersonPlayerController        |   |  - OutpostHUD                         |
|  - ThirdPersonCameraController        |   |  - MapWindowController (Map UI)       |
|  - InteractionManager & Interactables |   |  - DialogueWindowController           |
|  - MissionManager (IMapDataProvider)  |   |  - FiltrationPuzzleUI                 |
+---------------------------------------+   +---------------------------------------+
```

> **Design Boundary Rule:**  
> `Outpost.System3.Interface` never directly references `Outpost.System1.Gameplay`.  
> All cross-system communication (interaction prompts, telemetry streaming, map data query, dialogue requests) is routed through `Outpost.Core.Interfaces` and `OutpostEventChannel`.

---

## 2. Task A: Player & Camera Locomotion API

### Locomotion Controller: `ThirdPersonPlayerController`
Namespace: `Outpost.System1.Gameplay.Player`  
Assembly: `Outpost.System1.Gameplay.dll`  
Base Class: `MonoBehaviour` (Also aliased by `PlayerController` for backward compatibility)

#### Physics & Locomotion Specifications
- **Lunar Gravity:** $-1.62\text{ m/s}^2$ applied continuously when ungrounded.
- **Terminal Velocity:** Clamped to $-25.0\text{ m/s}$.
- **Ground Detection:** Dual-check using `CharacterController.isGrounded` combined with downward `Physics.SphereCast` ($r = 0.35\text{ m}$, distance $= 0.25\text{ m}$) against ground layers.
- **Walk Speed:** $3.2\text{ m/s}$ (default configurable).
- **Run Speed:** $6.2\text{ m/s}$ (triggered by Left Shift).
- **Jump Height:** $2.2\text{ m}$ (calculated via $v = \sqrt{2 \cdot |g| \cdot h}$ yielding $\approx 2.67\text{ m/s}$ initial jump impulse under lunar gravity).
- **Low-Gravity Floatiness & Air Damping:** Air control factor of $0.85$ allows realistic lunar inertia without sacrificing player responsiveness.
- **Rotation:** Smooth spherical linear interpolation (`Quaternion.Slerp`) toward movement direction relative to the active camera facing angle at $12.0\text{ rad/s}$.
- **Platform Compatibility:** Pure `Input.GetAxisRaw("Horizontal")`, `Input.GetAxisRaw("Vertical")`, and `Input.GetButton("Jump")` compatible with browser WebGL canvas focus and pointer lock.

#### Public API
```csharp
public class ThirdPersonPlayerController : MonoBehaviour
{
    // Configuration Properties
    public float WalkSpeed { get; set; }
    public float RunSpeed { get; set; }
    public float RotationSpeed { get; set; }
    public float JumpHeight { get; set; }
    public float Gravity { get; set; }
    public bool IsGrounded { get; }
    public bool IsRunning { get; }
    public Vector3 Velocity { get; }

    // Control lock for dialogue sequences, cutscenes, and minigames
    public void SetMovementLocked(bool locked);
    
    // Teleportation utility for respawns or chapter transitions
    public void Teleport(Vector3 position, Quaternion rotation);
}
```

---

### Camera Controller: `ThirdPersonCameraController`
Namespace: `Outpost.System1.Gameplay.Player`  
Assembly: `Outpost.System1.Gameplay.dll`  
Base Class: `MonoBehaviour` (Also aliased by `CameraController` for backward compatibility)

#### Features
- **Follow Target:** Smooth exponential damping (`Vector3.SmoothDamp`) following player root or head anchor.
- **Orbit Controls:** Browser pointer-lock aware mouse look (`Input.GetAxis("Mouse X")`, `Input.GetAxis("Mouse Y")`).
- **Pitch Clamping:** Vertical look angle constrained between $-35^\circ$ and $+70^\circ$ to prevent gimbal inversion.
- **Spring-Arm Occlusion Handling:** `Physics.SphereCast` ($r = 0.2\text{ m}$) detects terrain, habitat structures, and equipment collisions, smoothly moving the camera forward to avoid clipping through solid objects.
- **Scroll Zoom:** Configurable distance ($2.0\text{ m}$ to $8.0\text{ m}$, default $4.5\text{ m}$).

#### Public API
```csharp
public class ThirdPersonCameraController : MonoBehaviour
{
    public Transform Target { get; set; }
    public float Distance { get; set; }
    public float SensitivityX { get; set; }
    public float SensitivityY { get; set; }
    public LayerMask CollisionLayers { get; set; }

    public void SetControlLock(bool locked);
    public void SetLock(bool locked); // Backward compatibility alias
    public void FocusOnPoint(Vector3 worldPoint, float duration = 1.5f);
    public void ResetFocus();
}
```

---

## 3. Task B: Reusable Interaction System

The interaction system replaces legacy hardcoded raycasts with a decoupled interface-driven architecture.

### Shared Contract: `IInteractable`
Namespace: `Outpost.Core.Interfaces`  
Assembly: `Outpost.Core.dll`

```csharp
public interface IInteractable
{
    string InteractionPrompt { get; }
    string ContextDescription { get; }
    bool CanInteract(GameObject interactor);
    void Interact(GameObject interactor);
    void OnFocusEnter();
    void OnFocusExit();
    Transform Transform { get; }
}
```

### Shared Abstract Base: `InteractableBase`
Namespace: `Outpost.System1.Gameplay.Interactions`  
Provides:
- Range validation (`interactionRadius = 3.0f`).
- Cooldown throttling to prevent rapid double-clicks.
- Audio trigger callbacks (`AudioClip interactSound`).
- Visual highlight hooks (`Renderer outlineRenderer` or material emissive pulse).
- Abstract method `protected abstract void ExecuteInteraction(GameObject interactor)`.

### Concrete Interactable Implementations

| Component | Concrete Class | Primary Functionality | Example Mission 1 Usage |
|---|---|---|---|
| **Equipment Inspector** | `EquipmentInspectable` | Scans machinery status, reports diagnostic data to crew/ORION. | Inspecting Solar Array inverter, Oxygen generator seals. |
| **Habitat Door** | `HabitatDoorInteractable` | Smooth pneumatic sliding airlock cycle, toggles NavMeshObstacle & colliders. | Airlock entrance between lunar exterior and central habitat. |
| **System Console** | `ConsoleInteractable` | Toggles subsystem power, raises subsystem state changes. | Life support emergency console, CO2 scrubbing controls. |
| **Scientific Sample** | `ScientificSampleInteractable` | Spectrometry scan, triggers STEM lunar geology educational facts. | Water-ice core in Shackleton Crater, anorthosite regolith sample. |
| **Mission Objective** | `MissionObjectInteractable` | Milestones that advance mission objectives or trigger chapters. | Lander hatch opening, antenna deployment switch. |
| **Crew Dialogue** | `DialogueInteractable` | Initiates contextual multi-character spoken briefings and dialogues. | Talking to Commander Elena, Marcus, Aisha, or Daniel. |

### Interaction Manager: `InteractionManager`
Namespace: `Outpost.System1.Gameplay.Interactions`  
Attached to: Player GameObject or Gameplay Services.

- **Detection Mechanism:** Continuous forward `Physics.SphereCast` ($r = 0.6\text{ m}$, distance $= 3.2\text{ m}$) from camera center combined with distance falloff check.
- **Event Dispatch:** When target changes, publishes `OutpostEventChannel.RaiseInteractionPromptChanged(promptText, canInteract, worldPosition)`.
- **UI Decoupling:** `OutpostHUD` (System 3) listens to `InteractionPromptChanged` and displays the floating prompt without knowing the gameplay implementation.
- **Execution:** Processes `KeyCode.E` (configurable `InteractionKey`) to call `interactable.Interact(gameObject)`.

---

## 4. Task C: Mission State Foundation

### State Model
Namespace: `Outpost.System1.Gameplay.Mission`  
Assembly: `Outpost.System1.Gameplay.dll`

```csharp
public enum MissionStatus
{
    NotStarted,
    Active,
    Paused,
    Failed,
    Completed
}

public enum ObjectiveStatus
{
    Inactive,
    Active,
    Completed,
    Failed
}

[System.Serializable]
public class MissionObjective
{
    public string ObjectiveId;
    public string Title;
    public string Description;
    public ObjectiveStatus Status;
    public Vector3 WaypointLocation;
    public bool IsOptional;
    public string RequiredLocationId;
}

[System.Serializable]
public class MissionChapterModel
{
    public int ChapterNumber;
    public string ChapterTitle;
    public string ChapterSummary;
    public List<MissionObjective> Objectives;
    public bool IsCompleted;
}
```

### Mission Manager: `MissionManager`
Namespace: `Outpost.System1.Gameplay.Mission`  
Implements: `IMapDataProvider`, `IGameService`

#### Mission 1 Chapter Graph:
1. **Chapter 1 – Arrival on the Moon:** Inspect landing gear, traverse to Habitat, meet ORION.
2. **Chapter 2 – Establishing the Outpost:** Activate Solar Farm, verify Oxygen Generator, check Greenhouse.
3. **Chapter 3 – The First Warning:** Consult Aisha on solar storm readings, inspect external relay.
4. **Chapter 4 – The Life-Support Emergency:** Investigate power blackout, inspect CO2 scrubber failure.
5. **Chapter 5 – The Filtration Challenge:** Assemble emergency filtration adapter (inspired by Apollo 13).
6. **Chapter 6 – Save the Outpost:** Choose between astronaut EVA repair, robotic maintenance, or power conservation.
7. **Chapter 7 – A New Beginning:** Restore atmospheric cycle, confirm outpost safety.

#### Public API
```csharp
public class MissionManager : MonoBehaviour, IMapDataProvider
{
    public static MissionManager Instance { get; }
    public MissionStatus CurrentStatus { get; }
    public int CurrentChapterNumber { get; }
    public MissionChapterModel CurrentChapter { get; }
    public MissionObjective ActiveObjective { get; }

    // Mission Lifecycle Control
    public void StartMission();
    public void ActivateObjective(string objectiveId);
    public void CompleteObjective(string objectiveId);
    public void CompleteChapter(int chapterNumber);
    public void FailMission(string failureReason);
    public void CompleteMission();

    // Event Subscriptions
    public event Action<int, string> OnChapterStarted;
    public event Action<int, string> OnChapterCompleted;
    public event Action<MissionObjective> OnObjectiveActivated;
    public event Action<MissionObjective> OnObjectiveCompleted;
    public event Action<string> OnMissionFailed;
    public event Action OnMissionCompleted;
}
```

---

## 5. Task D: Map Data Provider Interface

System 1 does NOT render any visual map elements. Instead, it acts as the authoritative source of truth for all spatial, objective, and facility data required by Computer 3's Map UI.

### Shared Contract: `IMapDataProvider`
Namespace: `Outpost.Core.Interfaces`  
Assembly: `Outpost.Core.dll`

```csharp
public interface IMapDataProvider
{
    Vector3 PlayerWorldPosition { get; }
    Quaternion PlayerWorldRotation { get; }
    bool HasActiveWaypoint { get; }
    Vector3 ActiveWaypointPosition { get; }
    string ActiveObjectiveText { get; }
    IReadOnlyList<StationMapMarker> GetStationMarkers();
}
```

### Station Marker Data: `StationMapMarker`
Namespace: `Outpost.Core.Data`  
Assembly: `Outpost.Core.dll`

```csharp
[System.Serializable]
public struct StationMapMarker
{
    public string Id;
    public string DisplayName;
    public string Category; // "Habitat", "Power", "Science", "Logistics", "Emergency"
    public Vector3 WorldCoordinates;
    public bool IsDiscovered;
    public bool IsAccessible;
    public string Description;
}
```

### Canonical Lunar South Pole Outpost Coordinates (Unity World Space)

| Facility Identifier | Marker Name | Category | World Position $(X, Y, Z)$ |
|---|---|---|---|
| `facility_habitat` | Central Habitat Complex | Habitat | `(0.0, 0.0, 0.0)` |
| `facility_landing` | Landing Zone Artemis | Logistics | `(-120.0, 0.0, -80.0)` |
| `facility_solar` | Photovoltaic Solar Farm | Power | `(100.0, 15.0, 60.0)` |
| `facility_greenhouse` | Hydroponic Greenhouse | Life Support | `(35.0, 0.0, 20.0)` |
| `facility_laboratory` | Lunar Science Laboratory | Science | `(-40.0, 0.0, 30.0)` |
| `facility_comms` | Deep Space Comms Tower | Communications | `(140.0, 30.0, -50.0)` |
| `facility_rover` | Rover Bay & Maintenance | Logistics | `(-60.0, 0.0, -35.0)` |
| `facility_storage` | Cryogenic & Tool Storage | Logistics | `(-20.0, 0.0, -15.0)` |
| `facility_geology` | Shackleton Ridge Geological Site | Science | `(-160.0, -10.0, 110.0)` |
| `facility_emergency` | Primary Substation (Emergency) | Emergency | `(85.0, 10.0, 45.0)` |

---

## 6. Verification and Integration Guidelines for Peers

- **Computer 2 (Environment & Assets):**
  - Place interactive objects in the scene and attach the appropriate concrete `InteractableBase` component.
  - Set collision layers to `Default` or `Interactable` so the player's SphereCast and camera SpringArm detect them.
  - Position facilities matching or snapping to the canonical coordinates above.
- **Computer 3 (Interface & Audio):**
  - Subscribe to `OutpostEventChannel.OnInteractionPromptChanged` to show/hide the HUD interaction prompt.
  - Retrieve `IMapDataProvider` via `ServiceLocator.Get<IMapDataProvider>()` or `MissionManager.Instance` to drive the interactive lunar geo map.
  - Do NOT modify gameplay locomotion, player colliders, or mission state logic directly.
