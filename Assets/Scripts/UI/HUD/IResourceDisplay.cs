using System;

namespace Outpost.UI.HUD
{
    /// <summary>
    /// Data packet representing the state of an individual outpost resource.
    /// Supplied by Computer 1 (Gameplay / ResourceManager).
    /// </summary>
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

        public ResourceState State
        {
            get
            {
                if (IsLowerBetter)
                {
                    if (CurrentValue >= CriticalThreshold) return ResourceState.Critical;
                    if (CurrentValue >= WarningThreshold) return ResourceState.Warning;
                    return ResourceState.Nominal;
                }
                else
                {
                    if (CurrentValue <= CriticalThreshold) return ResourceState.Critical;
                    if (CurrentValue <= WarningThreshold) return ResourceState.Warning;
                    return ResourceState.Nominal;
                }
            }
        }
    }

    public enum ResourceState
    {
        Nominal = 0,   // Green / Safe
        Warning = 1,   // Amber / Caution
        Critical = 2   // Red / Danger
    }

    /// <summary>
    /// Complete snapshot of all 8 primary outpost resources.
    /// Supplied by Computer 1 without creating duplicate simulations in System 3.
    /// </summary>
    [System.Serializable]
    public struct OutpostResourceSnapshot
    {
        public ResourceEntry Oxygen;            // kg or percentage
        public ResourceEntry Water;             // Liters or percentage
        public ResourceEntry Food;              // Rations / days
        public ResourceEntry Electricity;       // kW generation vs demand
        public ResourceEntry Battery;           // kWh stored
        public ResourceEntry CrewHealth;        // Overall crew vitality %
        public ResourceEntry EquipmentCondition;// Outpost integrity %
        public ResourceEntry SpareParts;        // Inventory count
    }

    /// <summary>
    /// Service contract for Computer 1 to push real resource data into System 3's Resource Dashboard.
    /// </summary>
    public interface IResourceDisplay
    {
        void UpdateAllResources(OutpostResourceSnapshot snapshot);
        void UpdateOxygen(float current, float max, string explanation = "");
        void UpdateWater(float current, float max, string explanation = "");
        void UpdateFood(float current, float max, string explanation = "");
        void UpdateElectricity(float currentKw, float maxKw, string explanation = "");
        void UpdateBattery(float currentKwh, float maxKwh, string explanation = "");
        void UpdateCrewHealth(float percentage, string explanation = "");
        void UpdateEquipmentCondition(float percentage, string explanation = "");
        void UpdateSpareParts(int count, string explanation = "");
    }
}
