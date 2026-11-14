using System;
using UnityEngine;

namespace Outpost.UI.Core
{
    /// <summary>
    /// Centralized decoupled event broker for cross-system UI notifications.
    /// Eliminates direct references between Gameplay, Environment, and UI subsystems.
    /// </summary>
    public static class UIEventBus
    {
        // UI Navigation Events
        public static event Action<UIState> OnUIStateChanged;
        public static event Action<bool> OnPauseToggled;

        // Life Support / Resource Telemetry
        public static event Action<float, float, float> OnOxygenUpdated;      // Current, Max, Rate
        public static event Action<float, float, float> OnPowerUpdated;       // Current, Max, Rate
        public static event Action<float, float, float> OnWaterUpdated;       // Current, Max, Rate
        public static event Action<float, bool> OnCarbonDioxideUpdated;       // PPM, ScrubberActive

        // Mission & Objectives
        public static event Action<string, string> OnObjectiveUpdated;        // ID, Description
        public static event Action<string, Vector3, string> OnWaypointPinned; // ID, WorldPos, Label
        public static event Action<string> OnWaypointCleared;

        // Notifications & Alerts
        public static event Action<string, string, NotificationType> OnNotificationRequested;
        public static event Action<AlertLevel, string> OnAlertLevelChanged;

        // Emergency Decision (Chapter 6)
        public static event Action<EmergencyRepairOption> OnEmergencyOptionSelected;

        // Triggers
        public static void PublishUIStateChanged(UIState newState) => OnUIStateChanged?.Invoke(newState);
        public static void PublishPauseToggled(bool isPaused) => OnPauseToggled?.Invoke(isPaused);

        public static void PublishOxygen(float cur, float max, float rate) => OnOxygenUpdated?.Invoke(cur, max, rate);
        public static void PublishPower(float cur, float max, float rate) => OnPowerUpdated?.Invoke(cur, max, rate);
        public static void PublishWater(float cur, float max, float rate) => OnWaterUpdated?.Invoke(cur, max, rate);
        public static void PublishCarbonDioxide(float ppm, bool active) => OnCarbonDioxideUpdated?.Invoke(ppm, active);

        public static void PublishObjective(string id, string desc) => OnObjectiveUpdated?.Invoke(id, desc);
        public static void PublishWaypoint(string id, Vector3 pos, string label) => OnWaypointPinned?.Invoke(id, pos, label);
        public static void PublishClearWaypoint(string id) => OnWaypointCleared?.Invoke(id);

        public static void PublishNotification(string title, string msg, NotificationType type) =>
            OnNotificationRequested?.Invoke(title, msg, type);

        public static void PublishAlertLevel(AlertLevel level, string reason) =>
            OnAlertLevelChanged?.Invoke(level, reason);

        public static void PublishEmergencyOption(EmergencyRepairOption opt) =>
            OnEmergencyOptionSelected?.Invoke(opt);
    }
}
