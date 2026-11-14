using System;
using UnityEngine;
using Outpost.Core.Data;
using Outpost.Core.Interfaces;

namespace Outpost.Core.Events
{
    public static class OutpostEventChannel
    {
        // Mission Events
        public static event Action<MissionChapterId, string, string> ChapterChanged;
        public static event Action<string> ObjectiveUpdated;
        public static event Action<RepairChoice> RepairChoiceMade;
        public static event Action<string> MissionCompleted;
        public static event Action<string, string> MissionRewardAwarded; // rewardTitle, description
        public static event Action<string> CreditsTriggered;
        public static event Action<MissionSummaryReport> MissionSummaryPrompted;

        // Resource / Life-support Events
        public static event Action<float, float, float, float> TelemetryUpdated; // o2, co2, powerKW, battery%
        public static event Action<ResourceStateData> DetailedTelemetryUpdated;
        public static event Action<ResourceCategory, ResourceAlertLevel, string> ResourceAlertTriggered;
        public static event Action<bool, string> EmergencyTriggered; // active, reason
        public static event Action<EmergencyModel> EmergencyStateChanged;
        public static event Action<RepairOutcomeData> RepairOutcomeEvaluated;
        public static event Action<string, bool> FiltrationFeedbackPrompted;

        // Equipment Events
        public static event Action<IStationEquipment> EquipmentStateChanged;

        // Navigation & Geo Map Events
        public static event Action<StationLocationId> TargetWaypointSet;
        public static event Action<StationLocationId> LocationArrived;
        public static event Action<bool> GeoMapToggled; // isOpen
        public static event Action<string> InteractionPromptChanged;

        // Dialogue & Presentation Events
        public static event Action<DialogueLine> DialogueLinePrompted;
        public static event Action<ConversationSequence> ConversationSequencePrompted;
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
        public static void RaiseMissionCompleted(string message) => MissionCompleted?.Invoke(message);
        public static void RaiseMissionRewardAwarded(string title, string desc) => MissionRewardAwarded?.Invoke(title, desc);
        public static void RaiseCreditsTriggered(string creditsText) => CreditsTriggered?.Invoke(creditsText);
        public static void RaiseMissionSummaryPrompted(MissionSummaryReport summary) => MissionSummaryPrompted?.Invoke(summary);

        public static void RaiseTelemetryUpdated(float o2, float co2, float pwr, float bat) => TelemetryUpdated?.Invoke(o2, co2, pwr, bat);
        public static void RaiseDetailedTelemetryUpdated(ResourceStateData state) => DetailedTelemetryUpdated?.Invoke(state);
        public static void RaiseResourceAlertTriggered(ResourceCategory cat, ResourceAlertLevel lvl, string msg) => ResourceAlertTriggered?.Invoke(cat, lvl, msg);
        public static void RaiseEmergencyTriggered(bool active, string reason) => EmergencyTriggered?.Invoke(active, reason);
        public static void RaiseEmergencyStateChanged(EmergencyModel model) => EmergencyStateChanged?.Invoke(model);
        public static void RaiseRepairOutcomeEvaluated(RepairOutcomeData outcome) => RepairOutcomeEvaluated?.Invoke(outcome);
        public static void RaiseFiltrationFeedbackPrompted(string msg, bool success) => FiltrationFeedbackPrompted?.Invoke(msg, success);

        public static void RaiseEquipmentStateChanged(IStationEquipment equipment) => EquipmentStateChanged?.Invoke(equipment);

        public static void RaiseTargetWaypointSet(StationLocationId id) => TargetWaypointSet?.Invoke(id);
        public static void RaiseLocationArrived(StationLocationId id) => LocationArrived?.Invoke(id);
        public static void RaiseGeoMapToggled(bool open) => GeoMapToggled?.Invoke(open);
        public static void RaiseInteractionPromptChanged(string prompt) => InteractionPromptChanged?.Invoke(prompt);

        public static void RaiseDialogueLinePrompted(DialogueLine line) => DialogueLinePrompted?.Invoke(line);
        public static void RaiseConversationSequencePrompted(ConversationSequence seq) => ConversationSequencePrompted?.Invoke(seq);
        public static void RaiseDialogueCompleted() => DialogueCompleted?.Invoke();
        public static void RaiseEducationalFactPrompted(EducationalFact fact) => EducationalFactPrompted?.Invoke(fact);

        public static void RaiseFiltrationChallengeOpened() => FiltrationChallengeOpened?.Invoke();
        public static void RaiseFiltrationChallengeCompleted(bool success) => FiltrationChallengeCompleted?.Invoke(success);
        public static void RaiseStrategicChoiceOpened() => StrategicChoiceOpened?.Invoke();

        public static void RaisePlaySfx(string cue) => PlaySfx?.Invoke(cue);

        /// <summary>
        /// Clears all static event subscribers. Essential for isolated unit test execution.
        /// </summary>
        public static void ClearAllSubscribers()
        {
            ChapterChanged = null;
            ObjectiveUpdated = null;
            RepairChoiceMade = null;
            MissionCompleted = null;
            MissionRewardAwarded = null;
            CreditsTriggered = null;
            MissionSummaryPrompted = null;

            TelemetryUpdated = null;
            DetailedTelemetryUpdated = null;
            ResourceAlertTriggered = null;
            EmergencyTriggered = null;
            EmergencyStateChanged = null;
            RepairOutcomeEvaluated = null;
            FiltrationFeedbackPrompted = null;

            EquipmentStateChanged = null;

            TargetWaypointSet = null;
            LocationArrived = null;
            GeoMapToggled = null;
            InteractionPromptChanged = null;

            DialogueLinePrompted = null;
            ConversationSequencePrompted = null;
            DialogueCompleted = null;
            EducationalFactPrompted = null;

            FiltrationChallengeOpened = null;
            FiltrationChallengeCompleted = null;
            StrategicChoiceOpened = null;

            PlaySfx = null;
        }
    }
}
