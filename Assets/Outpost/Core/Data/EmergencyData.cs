using System;
using System.Collections.Generic;
using UnityEngine;

namespace Outpost.Core.Data
{
    public enum EmergencyType
    {
        SolarPanelDamage,                  // 1. Micrometeoroid punctured solar array relay
        ElectricalDistributionFault,       // 2. High-voltage bus breaker tripped
        OxygenGeneratorInterruption,       // 3. Sabatier reactor cell overheated/unpowered
        CarbonDioxideAccumulation,         // 4. LiOH scrubber canister saturated, CO2 climbing
        WaterRecyclingFailure,             // 5. WRS catalytic oxidizer clogged
        GreenhousePowerShortage,           // 6. Hydroponic grow lights shed from power loss
        CommunicationBlackout,             // 7. Deep space high-gain antenna tracking failure
        HabitatTemperatureControlIssue     // 8. Radiator thermal loop valve locked/frozen
    }

    public enum EmergencySeverity
    {
        Low,
        Medium,
        High,
        Critical
    }

    public enum EmergencyStatus
    {
        Inactive,
        Active,
        UnderRepair,
        Resolved
    }

    [Serializable]
    public class EmergencyModel
    {
        public EmergencyType Type;
        public string Title;
        public string Description;
        public EmergencySeverity Severity;
        public EmergencyStatus Status;
        public StationLocationId Location;
        public string TriggerCause;
        public string EquipmentConsequences;
        public string ResourceConsequences;
        public string PlayerObjective;
        public string RepairCondition;
        public string RecoveryState;

        public bool IsActive => Status == EmergencyStatus.Active || Status == EmergencyStatus.UnderRepair;
        public bool IsResolved => Status == EmergencyStatus.Resolved;

        public EmergencyModel(
            EmergencyType type,
            string title,
            string description,
            EmergencySeverity severity,
            StationLocationId location,
            string trigger,
            string equipCons,
            string resCons,
            string objective,
            string repairCond,
            string recovery)
        {
            Type = type;
            Title = title;
            Description = description;
            Severity = severity;
            Status = EmergencyStatus.Inactive;
            Location = location;
            TriggerCause = trigger;
            EquipmentConsequences = equipCons;
            ResourceConsequences = resCons;
            PlayerObjective = objective;
            RepairCondition = repairCond;
            RecoveryState = recovery;
        }
    }

    [Serializable]
    public class RepairOutcomeData
    {
        public RepairChoice Choice;
        public string Title;
        public string Summary;
        public float RepairDurationSeconds;
        public float RestoredPowerKw;
        public float EquipmentConditionPercent;
        public float CrewHealthImpactPercent;    // Negative for health loss
        public float BatteryCostKwh;
        public float OxygenCostLiters;
        public string DialogueLog;

        public RepairOutcomeData(
            RepairChoice choice,
            string title,
            string summary,
            float duration,
            float powerKw,
            float condition,
            float healthImpact,
            float batteryCost,
            float o2Cost,
            string dialogue)
        {
            Choice = choice;
            Title = title;
            Summary = summary;
            RepairDurationSeconds = duration;
            RestoredPowerKw = powerKw;
            EquipmentConditionPercent = condition;
            CrewHealthImpactPercent = healthImpact;
            BatteryCostKwh = batteryCost;
            OxygenCostLiters = o2Cost;
            DialogueLog = dialogue;
        }
    }
}
