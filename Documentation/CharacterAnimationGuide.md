# OUTPOST: SURVIVE BEYOND EARTH
## Character Animation & Multi-Astronaut Interaction Guide (Phase 3)

**Author:** System 2 (3D Environment, Characters & Asset Architecture)  
**Target Systems:** Computer 1 (Gameplay & Animation Control), Computer 3 (Dialogue & Cinematics)  
**Date:** 2045 Lunar South Pole Mission Simulation  

---

### 1. Overview

System 2 has created 12 dedicated animation clips, an integrated Unity Animator Controller (`Astronaut_Controller.controller`), and multi-astronaut dialogue staging prefabs. These assets directly drive the standard bone hierarchy (`Rig_Root/Hips/...`) integrated into all 4 astronaut characters (Elena, Marcus, Aisha, Daniel).

All character animation files reside in: `Assets/Art/CharacterAnimations/`.

---

### 2. Animation Clips Directory

| Clip Name | Path | Duration | Loop | Gameplay & Storyline Context |
|---|---|---|---|---|
| **Anim_Astronaut_Idle** | `Assets/Art/CharacterAnimations/Anim_Astronaut_Idle.anim` | 2.0 s | Yes | Default relaxed stance, rhythmic breathing, subtle weight shift in lunar gravity |
| **Anim_Astronaut_Walk** | `Assets/Art/CharacterAnimations/Anim_Astronaut_Walk.anim` | 1.2 s | Yes | Low-gravity bounding surface walk, natural arm swing |
| **Anim_Astronaut_Run** | `Assets/Art/CharacterAnimations/Anim_Astronaut_Run.anim` | 0.8 s | Yes | Dynamic lunar jog / forward sprint, torso lean |
| **Anim_Astronaut_Talk** | `Assets/Art/CharacterAnimations/Anim_Astronaut_Talk.anim` | 2.4 s | Yes | Conversational gesturing, head nod, helmet comm emphasis |
| **Anim_Astronaut_Point** | `Assets/Art/CharacterAnimations/Anim_Astronaut_Point.anim` | 2.0 s | Hold | Right arm raised pointing at damaged equipment, waypoints, or lunar landmarks |
| **Anim_Astronaut_Inspect** | `Assets/Art/CharacterAnimations/Anim_Astronaut_Inspect.anim` | 2.2 s | Yes | Torso leaning forward, hands raised examining telemetry, fault diagnosis |
| **Anim_Astronaut_Repair** | `Assets/Art/CharacterAnimations/Anim_Astronaut_Repair.anim` | 1.8 s | Yes | Crouched/kneeling maintenance pose, hands working with tools / filter cartridge |
| **Anim_Astronaut_OperateEquipment** | `Assets/Art/CharacterAnimations/Anim_Astronaut_OperateEquipment.anim` | 1.6 s | Yes | Standing console interaction, typing, switching breakers |
| **Anim_Astronaut_ConcernedReaction** | `Assets/Art/CharacterAnimations/Anim_Astronaut_ConcernedReaction.anim` | 2.0 s | Once | Emergency reaction, stepping back in alarm, hands raised defensively to helmet |
| **Anim_Astronaut_TeamDiscussion** | `Assets/Art/CharacterAnimations/Anim_Astronaut_TeamDiscussion.anim` | 2.8 s | Yes | Standing conversational pose, facing partner, natural listening idle |
| **Anim_Astronaut_SeatedDiscussion** | `Assets/Art/CharacterAnimations/Anim_Astronaut_SeatedDiscussion.anim` | 2.5 s | Yes | Seated posture (legs at $90^\circ$, hips lowered), hands resting on briefing table |
| **Anim_Astronaut_EquipmentDemo** | `Assets/Art/CharacterAnimations/Anim_Astronaut_EquipmentDemo.anim` | 2.2 s | Yes | Presenting equipment with sweeping hand gesture while turning head toward crew |

---

### 3. Animator Controller Specification (`Astronaut_Controller.controller`)

The controller is pre-configured with parameters and transitions for easy control from gameplay scripts:

#### Parameters:
- `Speed` (`Float`, default `0.0`): Drives locomotion blending:
  - `Speed < 0.1` $\rightarrow$ `Idle`
  - `0.1 <= Speed < 2.5` $\rightarrow$ `Walk`
  - `Speed >= 2.5` $\rightarrow$ `Run`
- `IsInteracting` (`Bool`, default `false`): General interaction lock.
- `ActionState` (`Int`, default `0`): Triggers specific action gestures:
  - `0`: Locomotion / Neutral
  - `1`: Talk
  - `2`: Point
  - `3`: Inspect
  - `4`: Repair
  - `5`: OperateEquipment
  - `6`: ConcernedReaction
  - `7`: TeamDiscussion (Standing)
  - `8`: SeatedDiscussion
  - `9`: EquipmentDemo

#### C# Integration Example (For Computer 1):
```csharp
Animator animator = GetComponent<Animator>();

// For walking / running
animator.SetFloat("Speed", currentMoveSpeed);

// To trigger emergency concern reaction
animator.SetInteger("ActionState", 6);

// To transition astronaut into repairing equipment
animator.SetInteger("ActionState", 4);

// To return to idle locomotion
animator.SetInteger("ActionState", 0);
```

---

### 4. Multi-Astronaut Interaction & Dialogue Staging

To support two or more astronauts interacting during conversations (as mandated in Section B of Master Instructions):

#### A. Two-Astronaut Face-to-Face Staging (`Prefab_Staging_TwoAstronautConversation.prefab`)
- **Location:** `Assets/Art/Prefabs/Outpost/Prefab_Staging_TwoAstronautConversation.prefab`
- **Structure:**
  - `ActorAnchor_A`: Transform at $(0, 0, -0.9\text{ m})$, facing $+Z$ ($0^\circ$).
  - `ActorAnchor_B`: Transform at $(0, 0, 0.9\text{ m})$, facing $-Z$ ($180^\circ$).
  - Distance: $1.8\text{ m}$ (ideal spacing for lunar EVA suits).
- **Dialogue Camera Framing Targets (For Computer 3):**
  - `CameraTarget_OverShoulder_A`: Over Actor A's shoulder framing Actor B's faceplate.
  - `CameraTarget_OverShoulder_B`: Over Actor B's shoulder framing Actor A's faceplate.
  - `CameraTarget_TwoShot_Profile`: Side profile capturing both astronauts in conversation.

#### B. Full-Crew Habitat Briefing Staging (`Prefab_Staging_CrewBriefing_Habitat.prefab`)
- **Location:** `Assets/Art/Prefabs/Outpost/Prefab_Staging_CrewBriefing_Habitat.prefab`
- **Structure:** Positioned around the central holographic operations table ($R=1.5\text{ m}$):
  - `BriefingAnchor_Commander`: Commander Elena presenting at $(0, 0, -2.2\text{ m})$.
  - `BriefingAnchor_Engineer`: Engineer Marcus at $(-2.0\text{ m}, 0, -0.5\text{ m})$ (angled $75^\circ$).
  - `BriefingAnchor_Scientist`: Scientist Aisha at $(2.0\text{ m}, 0, -0.5\text{ m})$ (angled $-75^\circ$).
  - `BriefingAnchor_Medic`: Medical Officer Daniel at $(0, 0, 1.8\text{ m})$ (angled $180^\circ$).
- **Briefing Camera Targets:**
  - `CameraTarget_GroupWide`: Wide angle ($Y=2.8\text{ m}$, pitch $30^\circ$) viewing all 4 astronauts around the table.
  - `CameraTarget_CommanderFocus`: Medium shot framed on Commander Elena during chapter mission briefings.
