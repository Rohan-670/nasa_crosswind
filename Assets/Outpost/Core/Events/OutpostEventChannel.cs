using System;
using UnityEngine;
using Outpost.Core.Data;

namespace Outpost.Core.Events
{
    public static class OutpostEventChannel
    {
        // Mission Events
        public static event Action<MissionChapterId, string, string> ChapterChanged;
        public static event Action<string> ObjectiveUpdated;
        public static event Action<RepairChoice> RepairChoiceMade;

        // Resource / Life-support Events
        public static event Action<float, float, float, float> TelemetryUpdated; // o2, co2, powerKW, battery%
        public static event Action<bool, string> EmergencyTriggered; // active, reason

        // Navigation & Geo Map Events
        public static event Action<StationLocationId> TargetWaypointSet;
        public static event Action<StationLocationId> LocationArrived;
        public static event Action<bool> GeoMapToggled; // isOpen
        public static event Action<string> InteractionPromptChanged;

        // Dialogue & Presentation Events
        public static event Action<DialogueLine> DialogueLinePrompted;
        public static event Action DialogueCompleted;
        public static event Action<EducationalFact> EducationalFactPrompted;

        // Interactive Puzzle Events
        public static event Action FiltrationChallengeOpened;
        public static event Action<bool> FiltrationChallengeCompleted; // success
        public static event Action StrategicChoiceOpened;

        // Audio events
        public static event Action<string> PlaySfx;

        // Broadcast Helpers
        public static void RaiseChapterChanged(MissionChapterId id, string title, string obj) => ChapterChanged?.Invoke(id, title, obj);
        public static void RaiseObjectiveUpdated(string obj) => ObjectiveUpdated?.Invoke(obj);
        public static void RaiseRepairChoiceMade(RepairChoice choice) => RepairChoiceMade?.Invoke(choice);

        public static void RaiseTelemetryUpdated(float o2, float co2, float pwr, float bat) => TelemetryUpdated?.Invoke(o2, co2, pwr, bat);
        public static void RaiseEmergencyTriggered(bool active, string reason) => EmergencyTriggered?.Invoke(active, reason);

        public static void RaiseTargetWaypointSet(StationLocationId id) => TargetWaypointSet?.Invoke(id);
        public static void RaiseLocationArrived(StationLocationId id) => LocationArrived?.Invoke(id);
        public static void RaiseGeoMapToggled(bool open) => GeoMapToggled?.Invoke(open);
        public static void RaiseInteractionPromptChanged(string prompt) => InteractionPromptChanged?.Invoke(prompt);

        public static void RaiseDialogueLinePrompted(DialogueLine line) => DialogueLinePrompted?.Invoke(line);
        public static void RaiseDialogueCompleted() => DialogueCompleted?.Invoke();
        public static void RaiseEducationalFactPrompted(EducationalFact fact) => EducationalFactPrompted?.Invoke(fact);

        public static void RaiseFiltrationChallengeOpened() => FiltrationChallengeOpened?.Invoke();
        public static void RaiseFiltrationChallengeCompleted(bool success) => FiltrationChallengeCompleted?.Invoke(success);
        public static void RaiseStrategicChoiceOpened() => StrategicChoiceOpened?.Invoke();

        public static void RaisePlaySfx(string cue) => PlaySfx?.Invoke(cue);
    }
}
