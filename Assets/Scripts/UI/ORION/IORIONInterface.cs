using System;
using Outpost.UI.Core;

namespace Outpost.UI.ORION
{
    public enum ORIONMessageType
    {
        MissionStatus = 0,
        EquipmentWarning = 1,
        ResourceWarning = 2,
        NavigationGuidance = 3,
        ScientificExplanation = 4,
        EmergencyNotification = 5,
        ObjectiveReminder = 6
    }

    [System.Serializable]
    public struct ORIONMessage
    {
        public ORIONMessageType Type;
        public string Subject;
        public string MessageText;
        public AlertLevel Alert;
        public float DisplayDuration;
    }

    /// <summary>
    /// Service contract for ORION AI communication across all mission chapters.
    /// Provides consistent language, warnings, tutorials, and navigation guidance.
    /// </summary>
    public interface IORIONInterface
    {
        AlertLevel CurrentAlertLevel { get; }

        void PostMessage(ORIONMessage message);
        void PostMissionStatus(string statusReport);
        void PostEquipmentWarning(string equipmentName, string details);
        void PostResourceWarning(string resourceName, float current, float threshold);
        void PostNavigationGuidance(string destinationName, float distanceMeters, string advice = "");
        void PostScientificExplanation(string conceptTitle, string explanation);
        void PostEmergencyNotification(string emergencyDetails);
        void PostObjectiveReminder(string currentObjective);
    }
}
