using System;
using UnityEngine;
using Outpost.UI.HUD;
using Outpost.UI.ORION;

namespace Outpost.UI.Core
{
    /// <summary>
    /// Event contract for Computer 1 (Gameplay / Missions / Resources) to drive System 3 UI progression.
    /// Fully decouples mission and resource code from UI presentation objects.
    /// </summary>
    public interface IMissionProgressionBridge
    {
        // Objectives & Prompts
        void SetMissionObjective(string objectiveId, string title, string description);
        void ShowInteractionPrompt(string promptText);
        void ClearInteractionPrompt();

        // Dialogue & Notifications
        void TriggerConversation(string conversationId, Action onCompleted = null);
        void PostNotification(string title, string message, NotificationType type);

        // Phase 2: Resource Telemetry (Computer 1 Integration)
        void PushResourceSnapshot(OutpostResourceSnapshot snapshot);
        void UpdateResource(string resourceName, float current, float max, string explanation = "");

        // Phase 2: ORION AI Communications
        void PostORIONMessage(ORIONMessage message);
        void PostORIONEquipmentWarning(string equipmentName, string details);
        void PostORIONResourceWarning(string resourceName, float current, float threshold);
        void PostORIONNavigation(string destinationName, float distanceMeters, string advice = "");
        void PostORIONScience(string conceptTitle, string explanation);
        void PostORIONObjectiveReminder(string currentObjective);
        void SetStationAlert(AlertLevel level, string reason);

        // Phase 2: Map & Navigation
        void SetMapWaypoint(string locationId);
        void UnlockMapLocation(string locationId);
    }

    /// <summary>
    /// Default bridge implementation forwarding calls into UIEventBus, UIManager, and ORIONInterface.
    /// </summary>
    public class MissionProgressionBridge : MonoBehaviour, IMissionProgressionBridge
    {
        public static MissionProgressionBridge Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void SetMissionObjective(string objectiveId, string title, string description)
        {
            UIEventBus.PublishObjective(objectiveId, description);
        }

        public void ShowInteractionPrompt(string promptText)
        {
            UIManager.Instance?.GetComponentInChildren<HUDController>()?.SetInteractionPrompt(promptText);
        }

        public void ClearInteractionPrompt()
        {
            UIManager.Instance?.GetComponentInChildren<HUDController>()?.ClearInteractionPrompt();
        }

        public void TriggerConversation(string conversationId, Action onCompleted = null)
        {
            UIManager.Instance?.GetComponentInChildren<Dialogue.DialogueManager>()?.StartConversationById(conversationId, onCompleted);
        }

        public void PostNotification(string title, string message, NotificationType type)
        {
            UIEventBus.PublishNotification(title, message, type);
        }

        public void PushResourceSnapshot(OutpostResourceSnapshot snapshot)
        {
            UIEventBus.PublishResourceSnapshot(snapshot);
        }

        public void UpdateResource(string resourceName, float current, float max, string explanation = "")
        {
            switch (resourceName.ToLower())
            {
                case "oxygen":
                    UIEventBus.PublishOxygenData(current, max, explanation);
                    break;
                case "water":
                    UIEventBus.PublishWaterData(current, max, explanation);
                    break;
                case "food":
                    UIEventBus.PublishFoodData(current, max, explanation);
                    break;
                case "electricity":
                    UIEventBus.PublishElectricityData(current, max, explanation);
                    break;
                case "battery":
                    UIEventBus.PublishBatteryData(current, max, explanation);
                    break;
                case "crewhealth":
                case "crew health":
                    UIEventBus.PublishCrewHealthData(current, explanation);
                    break;
                case "equipmentcondition":
                case "equipment condition":
                    UIEventBus.PublishEquipmentConditionData(current, explanation);
                    break;
                case "spareparts":
                case "spare parts":
                    UIEventBus.PublishSparePartsData(Mathf.RoundToInt(current), explanation);
                    break;
            }
        }

        public void PostORIONMessage(ORIONMessage message)
        {
            UIManager.Instance?.GetComponentInChildren<ORIONInterface>()?.PostMessage(message);
        }

        public void PostORIONEquipmentWarning(string equipmentName, string details)
        {
            UIManager.Instance?.GetComponentInChildren<ORIONInterface>()?.PostEquipmentWarning(equipmentName, details);
        }

        public void PostORIONResourceWarning(string resourceName, float current, float threshold)
        {
            UIManager.Instance?.GetComponentInChildren<ORIONInterface>()?.PostResourceWarning(resourceName, current, threshold);
        }

        public void PostORIONNavigation(string destinationName, float distanceMeters, string advice = "")
        {
            UIManager.Instance?.GetComponentInChildren<ORIONInterface>()?.PostNavigationGuidance(destinationName, distanceMeters, advice);
        }

        public void PostORIONScience(string conceptTitle, string explanation)
        {
            UIManager.Instance?.GetComponentInChildren<ORIONInterface>()?.PostScientificExplanation(conceptTitle, explanation);
        }

        public void PostORIONObjectiveReminder(string currentObjective)
        {
            UIManager.Instance?.GetComponentInChildren<ORIONInterface>()?.PostObjectiveReminder(currentObjective);
        }

        public void SetStationAlert(AlertLevel level, string reason)
        {
            UIEventBus.PublishAlertLevel(level, reason);
        }

        public void SetMapWaypoint(string locationId)
        {
            UIManager.Instance?.GetComponentInChildren<Map.LunarMapUI>()?.SetWaypoint(locationId);
        }

        public void UnlockMapLocation(string locationId)
        {
            UIManager.Instance?.GetComponentInChildren<Map.LunarMapUI>()?.MarkLocationDiscovered(locationId);
        }
    }
}
