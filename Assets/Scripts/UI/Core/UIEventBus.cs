using System;
using UnityEngine;
using Outpost.UI.HUD;
using Outpost.UI.Dialogue;

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

        // Life Support / Resource Telemetry (Computer 1 Integration)
        public static event Action<OutpostResourceSnapshot> OnResourceSnapshotUpdated;
        public static event Action<float, float, float> OnOxygenUpdated;
        public static event Action<float, float, float> OnPowerUpdated;
        public static event Action<float, float, float> OnWaterUpdated;
        public static event Action<float, bool> OnCarbonDioxideUpdated;

        // Phase 2 Resource Updates
        public static event Action<float, float, string> OnOxygenDataChanged;
        public static event Action<float, float, string> OnWaterDataChanged;
        public static event Action<float, float, string> OnFoodDataChanged;
        public static event Action<float, float, string> OnElectricityDataChanged;
        public static event Action<float, float, string> OnBatteryDataChanged;
        public static event Action<float, string> OnCrewHealthDataChanged;
        public static event Action<float, string> OnEquipmentConditionDataChanged;
        public static event Action<int, string> OnSparePartsDataChanged;

        // Phase 3: Camera Framing & Group Discussion (Computer 2 Integration)
        public static event Action<CameraFramingType, CharacterIdentifier> OnCameraFramingRequested;

        // Phase 3: Emergency & Filtration Challenge (Computer 1 Integration)
        public static event Action<bool> OnFiltrationChallengeCompleted;
        public static event Action<EmergencyRepairOption> OnEmergencyOptionSelected;

        // Mission & Objectives
        public static event Action<string, string> OnObjectiveUpdated;
        public static event Action<string, Vector3, string> OnWaypointPinned;
        public static event Action<string> OnWaypointCleared;

        // Notifications & Alerts
        public static event Action<string, string, NotificationType> OnNotificationRequested;
        public static event Action<AlertLevel, string> OnAlertLevelChanged;

        // Triggers
        public static void PublishUIStateChanged(UIState newState) => OnUIStateChanged?.Invoke(newState);
        public static void PublishPauseToggled(bool isPaused) => OnPauseToggled?.Invoke(isPaused);

        public static void PublishResourceSnapshot(OutpostResourceSnapshot snapshot) =>
            OnResourceSnapshotUpdated?.Invoke(snapshot);

        public static void PublishOxygen(float cur, float max, float rate) => OnOxygenUpdated?.Invoke(cur, max, rate);
        public static void PublishPower(float cur, float max, float rate) => OnPowerUpdated?.Invoke(cur, max, rate);
        public static void PublishWater(float cur, float max, float rate) => OnWaterUpdated?.Invoke(cur, max, rate);
        public static void PublishCarbonDioxide(float ppm, bool active) => OnCarbonDioxideUpdated?.Invoke(ppm, active);

        public static void PublishOxygenData(float cur, float max, string exp = "") => OnOxygenDataChanged?.Invoke(cur, max, exp);
        public static void PublishWaterData(float cur, float max, string exp = "") => OnWaterDataChanged?.Invoke(cur, max, exp);
        public static void PublishFoodData(float cur, float max, string exp = "") => OnFoodDataChanged?.Invoke(cur, max, exp);
        public static void PublishElectricityData(float curKw, float maxKw, string exp = "") => OnElectricityDataChanged?.Invoke(curKw, maxKw, exp);
        public static void PublishBatteryData(float curKwh, float maxKwh, string exp = "") => OnBatteryDataChanged?.Invoke(curKwh, maxKwh, exp);
        public static void PublishCrewHealthData(float pct, string exp = "") => OnCrewHealthDataChanged?.Invoke(pct, exp);
        public static void PublishEquipmentConditionData(float pct, string exp = "") => OnEquipmentConditionDataChanged?.Invoke(pct, exp);
        public static void PublishSparePartsData(int count, string exp = "") => OnSparePartsDataChanged?.Invoke(count, exp);

        public static void PublishCameraFraming(CameraFramingType framing, CharacterIdentifier subject) =>
            OnCameraFramingRequested?.Invoke(framing, subject);

        public static void PublishFiltrationOutcome(bool success) =>
            OnFiltrationChallengeCompleted?.Invoke(success);

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
