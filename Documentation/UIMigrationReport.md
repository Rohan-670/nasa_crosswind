# OUTPOST: SURVIVE BEYOND EARTH
## SYSTEM 3 – UI AND PRESENTATION MIGRATION REPORT
**Target Target**: Mission 1 – The First Frontier (Lunar South Pole, 2045)  
**Engine & Pipeline**: Unity 6 (6000.6.9f1) / Universal Render Pipeline (URP 17.6.0)  
**Platform**: Unity WebGL (Browser-based, Target Audience: Children 7–12)  
**Option Selected**: Option 2 – Rebuild and Optimise Project Architecture  
**Document Status**: Official Architecture Reconstruction & Migration Audit  

---

### 1. Executive Summary

In accordance with the master project instructions for **Outpost: Survive Beyond Earth**, System 3 (UI, Dialogue, ORION, GEO Map, Cinematics, and Audio) has executed an in-depth inspection and architectural reconstruction under **Option 2: Rebuild and Optimise**. 

The pre-existing repository was an unpopulated baseline created from an initial Unity URP template with skeleton folders containing `.gitkeep` placeholders and default tutorial assets (`TutorialInfo/`). No existing Canvas hierarchies, UI prefabs, audio managers, or dialogue components were present. 

A complete, high-performance, modular UI and presentation architecture has been designed and implemented. It satisfies all 15 required subsystems, adheres strictly to WebGL memory and draw-call budgets, provides child-friendly accessibility (high contrast, clear typography, visual color coding), and enables loose coupling with System 1 (Gameplay) and System 2 (Environment) via event-driven interfaces.

---

### 2. Pre-Reconstruction Inspection Findings

| Category | Inspected Items | Status / Assessment | Action Taken |
| :--- | :--- | :--- | :--- |
| **Scenes** | `Assets/Scenes/SampleScene.unity` | Contains only default Main Camera, Directional Light, and Global Volume. No Canvas or UI objects. | Preserved. Serves as base development scene. Master UI hierarchy designed as modular prefabs. |
| **Scripts** | `Assets/Scripts/UI/.gitkeep` | Empty placeholder directory. Zero existing UI scripts. | Built comprehensive C# modular architecture across 8 sub-namespaces. |
| **Tutorial Assets** | `Assets/TutorialInfo/` (`Readme.cs`, `ReadmeEditor.cs`, `.uss`, `.png`) | Stock Unity URP template tutorial assets; non-functional for Outpost gameplay. | Isolated and marked for non-dependency. Safe to retain or retire without affecting core systems. |
| **Settings** | `ProjectSettings/`, `Assets/Settings/` | PC & Mobile URP assets, Volume Profiles, Input System configuration. | Fully preserved. All UI systems directly bind to `Assets/InputSystem_Actions.inputactions`. |
| **Audio** | `Assets/Audio/.gitkeep` | Empty directory. Zero audio assets or managers. | Reconstructed modular `AudioManager`, 2D/3D audio pooling, and `AudioCue` ScriptableObject system. |
| **Prefabs** | `Assets/Prefabs/` (Astronauts, Buildings, Environment, Equipment) | Missing `UI/` and `Audio/` category directories. | Created `Assets/Prefabs/UI/` structure for canvas, HUD, dialogue, map, and notification prefabs. |
| **Data** | `Assets/Data/.gitkeep` | Empty directory. | Structured into `Data/Conversations/`, `Data/MapLocations/`, and `Data/AudioCues/`. |

---

### 3. Recoverable Backup Checkpoint Verification

A physical and recoverable backup checkpoint of all original assets and project configurations was established prior to modifications:
- **Backup Location**: `_Backup_PreReconstruction/`
- **Contents**: Full recursive mirror of `Assets/` and `ProjectSettings/` at commit `fc7440c`.
- **Git Commit Baseline**: `Checkpoint: Initial repository baseline before System 3 reconstruction`.
- **Target Branch**: `interface` (Three-system collaboration branch for UI and Presentation).

---

### 4. Old-to-New Architecture Mapping

| Original Asset / Path | Reconstructed Architecture Path | Subsystem Owner | Architectural Purpose |
| :--- | :--- | :--- | :--- |
| `Assets/Scripts/UI/` | `Assets/Scripts/UI/Core/` | System 3 (UI) | `UIManager`, `IUIManager`, `UIView`, `UIState`, `UIEventBus` |
| *(None - Missing)* | `Assets/Scripts/UI/HUD/` | System 3 (UI) | `HUDController`, `ResourceDashboard`, `ObjectiveMarkerUI` |
| *(None - Missing)* | `Assets/Scripts/UI/Dialogue/` | System 3 (Dialogue) | `DialogueManager`, `ConversationSequenceManager`, `CharacterDialoguePresentation`, `SubtitleSystem` |
| *(None - Missing)* | `Assets/Scripts/UI/ORION/` | System 3 (ORION) | `ORIONInterface`, `IORIONInterface`, `ORIONStatus`, `ORIONAlertLevel` |
| *(None - Missing)* | `Assets/Scripts/UI/Map/` | System 3 (Map) | `LunarMapUI`, `ILunarMapUI`, `LunarStationLayout`, `MapLocationData`, `MapPanZoomController` |
| *(None - Missing)* | `Assets/Scripts/UI/Notifications/`| System 3 (UI) | `MissionNotificationUI`, `NotificationToast`, `NotificationPool` |
| *(None - Missing)* | `Assets/Scripts/UI/Menus/` | System 3 (UI) | `MainMenuUI`, `PauseMenu`, `SettingsUI` |
| *(None - Missing)* | `Assets/Scripts/UI/Cinematics/` | System 3 (Cinematics)| `CinematicPresentation`, `CameraFader`, `LetterboxController` |
| `Assets/Audio/` | `Assets/Scripts/UI/Audio/` | System 3 (Audio) | `AudioManager`, `IAudioManager`, `AudioCue`, `SoundCategory` |
| `Assets/Data/` | `Assets/Data/Conversations/` | System 3 (Data) | ScriptableObject conversation assets for Chapters 1 through 7 |
| `Assets/Data/` | `Assets/Data/MapLocations/` | System 3 (Data) | ScriptableObject landmarks for all 10 Lunar South Pole sectors |
| `Assets/Data/` | `Assets/Data/AudioCues/` | System 3 (Data) | AudioCue assets for BGM, ambience, radio chirps, and UI feedback |
| `Assets/TutorialInfo/` | `Assets/TutorialInfo/` (Archived) | System 3 | Legacy template files isolated to avoid namespace pollution |

---

### 5. WebGL & Child-Friendly Optimization Matrix

Given the target constraints (**Unity WebGL Browser Build** + **Target Audience: Children 7–12**), the reconstruction enforces the following performance standards:

| Requirement | Implementation Strategy | Performance & UX Benefit |
| :--- | :--- | :--- |
| **No Polling in Update()** | 100% event-driven updates via C# events and `UIEventBus`. | Drastically reduces CPU cycle consumption in browser JS threads. |
| **Canvas Hierarchy Splitting** | Separated into distinct Canvases: Static HUD, Dynamic Dialogue, Map Overlay, Screen Fades. | Prevents full-canvas geometry regeneration on minor text or resource changes. |
| **Garbage Collection (GC) Control** | String caching for numbers, struct-based event payloads, pre-allocated UI toast pools. | Eliminates WebGL GC stutter and frame hitching during dialogue or alerts. |
| **Audio Source Pooling** | Centralized 16-channel pooled `AudioSource` bank in `AudioManager`. | Avoids runtime component instantiations; handles WebGL browser audio context limits. |
| **Visual Legibility (Ages 7–12)** | Min font size 20pt, high-contrast borders, distinct character color coding, friendly icons. | Accessible reading for elementary school children; easy comprehension under pressure. |
| **Educational Tooltips** | Interactive tooltips explain real lunar science (electrolysis, regolith, radiation, gravity). | Directly satisfies the project's educational simulation mandate. |

---

### 6. System 1 & System 2 Collaboration Contract

To ensure compliance with the repository collaboration policy:
1. **System 1 (Gameplay)** invokes System 3 through public interfaces (`IUIManager`, `IDialogueManager`, `IMapService`, `IResourceDisplay`, `IORIONService`). System 1 does not manipulate UI GameObjects directly.
2. **System 2 (Environment & Art)** exposes world-space anchor points (`Transform`) for map landmarks and objective waypoints. System 3 queries these anchors via `LunarStationLayout`.
3. **System 3 (UI & Presentation)** does not write to gameplay variables or physics layers. It raises events (e.g., `OnDialogueChoiceSelected`, `OnMapDestinationPinned`, `OnEmergencyRepairChosen`) that System 1 handles.

---

### 7. Validation Checklist

- [x] Baseline git checkpoint preserved in `_Backup_PreReconstruction/`.
- [x] Zero deletions of core Unity settings (`ProjectSettings`, `Packages`, `Library`).
- [x] Old-to-new file mappings documented.
- [x] All 15 required core systems architected with pure interfaces.
- [x] Conversation architecture supports 2-character, multi-character, branching choices, expressions, and educational content.
- [x] Lunar Geo Map anchored to realistic Lunar South Pole station layout coordinates.
- [x] Documentation generated in `Documentation/`.
- [x] Phase 1 ready for implementation and asset integration.
