# OUTPOST: SURVIVE BEYOND EARTH
## Mission 1: Master Integration & Cross-System Guide (Phase 4)

**Environment:** Lunar South Pole (Shackleton Crater Rim Sector)  
**System Owner:** SYSTEM 2 (3D Environment, Characters, VFX & Asset Architecture)  
**Target Systems:** Computer 1 (Gameplay / Systems / Objectives), Computer 3 (UI / Dialogue / GEO Map)  
**Document Version:** 1.0.0 (Final Release)  

---

### 1. Integration Architecture & System Ownership

```text
+--------------------------------------------------------------------------+
|                       MISSION 1 MASTER ARCHITECTURE                      |
+--------------------------------------------------------------------------+
|  COMPUTER 1 (Gameplay)     |  COMPUTER 2 (Environment) |  COMPUTER 3 (UI)|
|  ------------------------- |  ------------------------ |  -------------- |
|  - Chapter Progression     |  - 3D Terrain & Craters   |  - GEO Map HUD  |
|  - ECLSS Logic & Systems   |  - 12 Outpost Buildings   |  - Dialogue Box |
|  - Astronaut Controller    |  - 4 Astronaut Models     |  - Subtitles    |
|  - Inventory & Scrubber    |  - 12 Animation Clips     |  - Camera Rigs  |
|  - Emergency State Logic   |  - 8 State-Switch Prefabs |  - Mission Logs |
|  - Minigame Mechanics      |  - 5 WebGL VFX Prefabs    |  - Warning HUD  |
+--------------------------------------------------------------------------+
```

---

### 2. World Coordinate Calibration & GEO Map Integration (For Computer 3)

The physical 3D environment in `System2_LunarTerrainTest.unity` is calibrated to the master registry:

- **Origin $(0, 0, 0)$:** Center of the Main Habitat command dome.
- **Orientation:** Unity Standard (+$X$ = East, +$Y$ = Elevation/Up, +$Z$ = North).
- **Scale:** $1.0\,\text{Unity Unit} = 1.0\,\text{Meter}$.

#### Station Coordinates for GEO Map Pins:
```csharp
public static readonly Dictionary<StationType, Vector3> StationCoordinates = new()
{
    { StationType.MainHabitat,            new Vector3(  0.0f,  0.00f,   0.0f) },
    { StationType.LandingZone,            new Vector3(  0.0f,  0.04f, -80.0f) },
    { StationType.SolarFarm,              new Vector3( 75.0f,  4.26f,  20.0f) },
    { StationType.Greenhouse,             new Vector3(-45.0f, -1.97f,  35.0f) },
    { StationType.ResearchLab,            new Vector3( 40.0f,  0.21f,  45.0f) },
    { StationType.CommTower,              new Vector3( 60.0f, 11.18f, -40.0f) },
    { StationType.RoverStation,           new Vector3(-35.0f,  0.00f, -30.0f) },
    { StationType.GeologicalResearchArea, new Vector3(-80.0f,  1.61f,  70.0f) },
    { StationType.EquipmentStorage,       new Vector3( 25.0f, -0.38f, -45.0f) },
    { StationType.EmergencySubstation,    new Vector3( 50.0f,  0.35f,  10.0f) }
};
```

---

### 3. Equipment State-Switching Integration (For Computer 1 - Gameplay)

During emergencies (Chapter 3 micrometeoroid strike, Chapter 4 ECLSS fault, Chapter 5 filtration challenge), state-switch prefabs toggle visual appearance without instantiating or destroying meshes at runtime.

#### Hierarchy Convention:
```text
Prefab_Emergency_<Name>_StateSwitch
├── State_Damaged           [Active during emergency: Damaged mesh + VFX Mount]
├── State_Repaired          [Active once fixed: Welded/clean mesh + Green LED]
├── InteractionPoint        [Transform: Player standing position]
├── InteractionTrigger      [BoxCollider: IsTrigger=1 for E key prompt]
└── InspectionCameraTarget  [Transform: Minigame camera framing]
```

#### C# Integration Example:
```csharp
public class StationEquipmentState : MonoBehaviour
{
    [SerializeField] private GameObject damagedVisual;
    [SerializeField] private GameObject repairedVisual;
    [SerializeField] private ParticleSystem emergencyVFX;
    
    public void SetRepaired(bool repaired)
    {
        damagedVisual.SetActive(!repaired);
        repairedVisual.SetActive(repaired);
        
        if (emergencyVFX != null)
        {
            if (repaired) emergencyVFX.Stop();
            else emergencyVFX.Play();
        }
    }
}
```

---

### 4. Character Animation & Control Integration (For Computer 1 & Computer 3)

All four astronauts (Elena, Marcus, Aisha, Daniel) use `Astronaut_Controller.controller`. Gameplay scripts control locomotion and gestures via three standard parameters:

| Parameter | Type | Values | Description |
| :--- | :--- | :--- | :--- |
| `Speed` | `Float` | `0.0` (Idle), `1.2` (Walk), `3.0` (Run) | Drives locomotion blend tree |
| `IsInteracting` | `Bool` | `true` / `false` | Locks movement during interaction |
| `ActionState` | `Int` | `0` = Locomotion<br>`1` = Talk<br>`2` = Point<br>`3` = Inspect<br>`4` = Repair<br>`5` = Operate Console<br>`6` = Concerned Alarm<br>`7` = Team Discussion<br>`8` = Seated Discussion<br>`9` = Equipment Demo | Triggers specific narrative gesture |

#### C# Animation Control Example:
```csharp
public class AstronautAnimationBridge : MonoBehaviour
{
    private Animator _animator;
    
    private void Awake() => _animator = GetComponent<Animator>();
    
    public void SetLocomotion(float speed)
    {
        _animator.SetFloat("Speed", speed);
        _animator.SetInteger("ActionState", 0);
    }
    
    public void TriggerRepairPose()
    {
        _animator.SetBool("IsInteracting", true);
        _animator.SetInteger("ActionState", 4); // Repair
    }
    
    public void TriggerConcernedAlarm()
    {
        _animator.SetInteger("ActionState", 6); // ConcernedReaction
    }
}
```

---

### 5. Multi-Astronaut Dialogue & Staging Integration (For Computer 3 - UI/Cinematics)

System 2 provides two calibrated dialogue staging prefabs in `Assets/Art/Prefabs/Outpost/`:

#### A. Two-Astronaut Face-to-Face Dialogue (`Prefab_Staging_TwoAstronautConversation`)
- **Spacing:** $1.8\text{m}$ mutual eye contact distance.
- **Workflow:**
  1. Position character A at `Slot_Astronaut_A`.
  2. Position character B at `Slot_Astronaut_B`.
  3. Set Character A `ActionState = 1` (Talk) and Character B `ActionState = 7` (TeamDiscussion).
  4. Point Cinemachine or Virtual Camera at `CamTarget_OTS_A` (Over-the-shoulder A), `CamTarget_OTS_B` (Over-the-shoulder B), or `CamTarget_TwoShot` (Medium two-shot).

#### B. Full Crew Habitat Briefing (`Prefab_Staging_CrewBriefing_Habitat`)
- **Placement:** Center of Habitat Interior at `(0, 0, 0)`.
- **Workflow:**
  1. Snap Commander Elena to `BriefingAnchor_Commander`.
  2. Snap Engineer Marcus, Scientist Aisha, and Medic Daniel to `BriefingAnchor_Engineer`, `BriefingAnchor_Scientist`, `BriefingAnchor_Medic`.
  3. Set Elena `ActionState = 9` (EquipmentDemo / Briefing gesture) and crew `ActionState = 8` (SeatedDiscussion).
  4. Point camera to `CameraTarget_GroupWide` for master shot or `CameraTarget_CommanderFocus` for Elena's speech.

---

### 6. Verification Scene (`Assets/Scenes/System2_LunarTerrainTest.unity`)

Computer 1 and Computer 3 can open `Assets/Scenes/System2_LunarTerrainTest.unity` to immediately verify:
1. Walk from Landing Zone $(0, 0.08, -80)$ up the smooth regolith runway to the Main Habitat.
2. Enter the Airlock Module at $(0, 0, -10.5)$ through the open corridor.
3. Pass into the Habitat Interior at $(0, 0, 0)$ without hitting any blocking colliders.
4. Inspect the 4 animated astronauts at their narrative stations.
5. Inspect the 8 emergency state-switch repair locations.
6. Test rover driving from Rover Station $(-35, 0, -30)$ across all navigable routes.
