# OUTPOST: SURVIVE BEYOND EARTH
## SYSTEM 3 – PHASE 2: CROSS-SYSTEM INTERFACE CONTRACTS
**System**: System 3 (UI, Presentation, ORION, Geo Map, Audio)  
**Target Systems**: System 1 (Gameplay & Resources) and System 2 (3D Environment & Characters)  
**Document**: Phase 2 Cross-System Integration Contract (`Documentation/System3_Phase2_InterfaceContracts.md`)  

---

### 1. Overview & Collaboration Principles

In accordance with the Three-System Parallel Development Policy:
- **System 3 owns the presentation layer** (HUD, Resource Dashboard visualizer, ORION holographic interface, 2D Lunar Geo Map, Subtitles, Audio).
- **System 1 owns all gameplay logic, physics, resource simulation, and mission state.**
- **System 2 owns 3D meshes, environments, character models, animations, and world transforms.**

System 3 **never** simulates resource decay, oxygen consumption, battery discharge, or player movement. Instead, System 3 exposes strongly-typed contracts and decoupled C# events that Systems 1 and 2 invoke.

---

### 2. Interface 1: Resource Telemetry (Computer 1 $\to$ System 3)

#### Data Packet: `OutpostResourceSnapshot`
```csharp
namespace Outpost.UI.HUD
{
    [System.Serializable]
    public struct OutpostResourceSnapshot
    {
        public ResourceEntry Oxygen;             // kg or percentage (0–100%)
        public ResourceEntry Water;              // Liters
        public ResourceEntry Food;               // Rations count
        public ResourceEntry Electricity;        // Current kW generation vs demand
        public ResourceEntry Battery;            // Current kWh stored
        public ResourceEntry CrewHealth;         // Overall vitality (0–100%)
        public ResourceEntry EquipmentCondition; // Mechanical integrity (0–100%)
        public ResourceEntry SpareParts;         // Inventory units
    }

    [System.Serializable]
    public struct ResourceEntry
    {
        public string ResourceId;
        public string DisplayName;
        public float CurrentValue;
        public float MaxValue;
        public string Unit;
        public float WarningThreshold;
        public float CriticalThreshold;
        public bool IsLowerBetter;
        public string SimpleExplanation;
    }
}
```

#### How Computer 1 Pushes Resource Data
Computer 1 can push either complete snapshots or individual resource updates via `MissionProgressionBridge` or `UIEventBus`:

```csharp
using Outpost.UI.Core;
using Outpost.UI.HUD;

// Option A: Bulk Update Snapshot (Recommended on resource tick)
MissionProgressionBridge.Instance.PushResourceSnapshot(mySnapshot);

// Option B: Single Resource Delta Updates
MissionProgressionBridge.Instance.UpdateResource("Oxygen", 28f, 100f, "Low oxygen warning: scrubbers running on backup.");
MissionProgressionBridge.Instance.UpdateResource("Battery", 14f, 100f, "Critical battery drain: conduit severed!");
MissionProgressionBridge.Instance.UpdateResource("SpareParts", 12f, 20f);
```

---

### 3. Interface 2: ORION AI Communications (Computer 1 $\to$ System 3)

System 3 provides ORION's holographic avatar, animated speech wave, alert level strobes, and educational tutorial drawer.

#### How Computer 1 Sends ORION Messages
```csharp
using Outpost.UI.Core;
using Outpost.UI.ORION;

// 1. Equipment Warning (Mandated Example)
MissionProgressionBridge.Instance.PostORIONEquipmentWarning(
    "solar array",
    "Operating below expected performance. Please inspect the external power equipment."
);

// 2. Resource Warning
MissionProgressionBridge.Instance.PostORIONResourceWarning("Battery Storage", 25f, 30f);

// 3. Navigation Assistance
MissionProgressionBridge.Instance.PostORIONNavigation("Rover Maintenance Bay", 75f, "Follow the marked west road.");

// 4. Scientific Explanation (Kids 7–12)
MissionProgressionBridge.Instance.PostORIONScience(
    "Lunar Regolith & Dust",
    "Moon dust is razor sharp! Without wind or rain to erode it, tiny particles cling to spacesuits through electrostatic charge."
);

// 5. Emergency Warning (Pulsing Red Hologram + Alarm Chime)
MissionProgressionBridge.Instance.SetStationAlert(AlertLevel.Emergency, "Micrometeoroid Impact Detected!");
```

---

### 4. Interface 3: Interactive Lunar GEO Map (Computer 1 & 2 $\to$ System 3)

#### Outpost Landmark Register
All coordinates match the physical Unity scene environment (with datum origin $(0, 0, 0)$ at the Main Habitat):

| Location ID | Facility Name | World Position $(X, Y, Z)$ | Grid | Owner |
| :--- | :--- | :--- | :--- | :--- |
| `LOC_HABITAT` | **Main Habitat** | `(0.0, 0.0, 0.0)` | `E-5` | System 2 Core |
| `LOC_LANDING` | **Landing Zone** | `(-180.0, 5.0, -120.0)` | `B-2` | System 2 Landing Pad |
| `LOC_SOLAR` | **Solar Farm** | `(120.0, 25.0, 160.0)` | `H-8` | System 2 Crater Ridge |
| `LOC_GREENHOUSE`| **Greenhouse Biodome** | `(-40.0, 2.0, 70.0)` | `D-7` | System 2 Agriculture |
| `LOC_LAB` | **Research Laboratory**| `(60.0, 2.0, -30.0)` | `F-4` | System 2 Science Wing |
| `LOC_COMM` | **Communication Tower**| `(150.0, 45.0, -90.0)` | `I-3` | System 2 High Ridge |
| `LOC_ROVER` | **Rover Station** | `(-75.0, 0.0, -45.0)` | `C-4` | System 2 Garage |
| `LOC_STORAGE` | **Equipment Storage** | `(-25.0, 0.0, -60.0)` | `D-3` | System 2 Airlock Depot |
| `LOC_GEOLOGY` | **Geological Ice Crater** | `(-220.0, -15.0, 250.0)`| `A-10`| System 2 Shadowed Crater |
| `LOC_EMERGENCY_1`| **Micrometeoroid Site**| `(110.0, 22.0, 140.0)` | `G-7` | System 1 Damage Hazard |

#### Required Transform Anchors from System 2
System 2 (Environment & Characters) provides world transforms for live map tracking:
1. **Player Object**: GameObject with tag `"Player"`.
2. **Crew Objects**:
   - `CREW_ELENA` $\to$ Elena's character `Transform`
   - `CREW_MARCUS` $\to$ Marcus's character `Transform`
   - `CREW_AISHA` $\to$ Aisha's character `Transform`
   - `CREW_DANIEL` $\to$ Daniel's character `Transform`
3. **Vehicle Object**:
   - `EQUIP_ROVER` $\to$ NASA Crosswind Rover `Transform`

```csharp
// Computer 2 can register dynamic transforms at runtime:
var tracker = FindObjectOfType<Outpost.UI.Map.MapEntityTracker>();
tracker.RegisterEntityTransform("CREW_ELENA", elenaGameObject.transform);
tracker.RegisterEntityTransform("EQUIP_ROVER", roverGameObject.transform);
```

#### How Computer 1 Sets Map Waypoints
```csharp
// Pin active mission waypoint
MissionProgressionBridge.Instance.SetMapWaypoint("LOC_SOLAR");

// Unlock newly explored sector
MissionProgressionBridge.Instance.UnlockMapLocation("LOC_GEOLOGY");
```
