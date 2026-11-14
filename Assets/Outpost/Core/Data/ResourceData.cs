using System;

namespace Outpost.Core.Data
{
    public enum ResourceCategory
    {
        Oxygen,
        Water,
        Food,
        Electricity,
        SpareParts,
        CrewHealth,
        EquipmentCondition,
        Radiation
    }

    public enum ResourceAlertLevel
    {
        Nominal,
        Warning,
        Critical
    }

    public enum GeneratorStatus
    {
        Offline,
        Nominal,
        Degraded,
        Overheating,
        Fault
    }

    public enum PowerGridStatus
    {
        Nominal,        // Solar generation >= consumption; battery charging or full
        BatteryDraw,    // Solar generation < consumption; battery supplying deficit
        LoadShedding,   // Battery low; non-essential systems powered down
        Brownout,       // Battery critically low; partial life support operation
        Blackout        // Zero power; complete system shutdown
    }

    public enum PowerPriority
    {
        CriticalLifeSupport = 0,    // O2 generator, emergency CO2 scrubber, airlock seals
        AtmosphereAndThermal = 1,   // Thermal control loop, cabin pressure maintenance
        WaterAndGreenhouse = 2,     // Water recycling unit, hydroponic LED arrays
        Communications = 3,         // High-gain antenna, Earth downlink
        ScienceAndAuxiliary = 4     // Spectrometer, drill charging, cabin interior lighting
    }

    /// <summary>
    /// Comprehensive snapshot of all outpost resources for telemetry, UI, and save/load state.
    /// Shared across Core, System 1 (Gameplay), and System 3 (Interface).
    /// </summary>
    [Serializable]
    public class ResourceStateData
    {
        // --- OXYGEN ---
        public float OxygenPercent = 99.2f;
        public float OxygenCurrentLiters = 4960.0f;
        public float OxygenMaxCapacityLiters = 5000.0f;
        public float OxygenProductionRate = 0.20f;   // L/sec (approx 12 L/min)
        public float OxygenConsumptionRate = 0.16f;  // L/sec (4 astronauts @ ~0.84 kg O2/day)
        public GeneratorStatus O2GeneratorStatus = GeneratorStatus.Nominal;
        public float CarbonDioxidePpm = 420.0f;

        // --- WATER ---
        public float WaterReserveLiters = 850.0f;
        public float WaterMaxCapacityLiters = 1000.0f;
        public float WaterConsumptionRate = 0.12f;   // L/sec (crew hydration + greenhouse)
        public float WaterRecyclingRate = 0.11f;     // L/sec (WRS output)
        public float WaterRecyclingEfficiency = 0.92f; // 92% recovery

        // --- FOOD ---
        public float FoodRations = 120.0f;          // 120 crew meals (~30 person-days)
        public float FoodMaxRations = 200.0f;
        public float FoodConsumptionRate = 0.04f;   // rations/sec simulation rate
        public float GreenhouseProductionRate = 0.05f; // rations/sec (when greenhouse active)
        public float GreenhouseBiomassPercent = 78.0f;

        // --- ELECTRICITY ---
        public float SolarGenerationKw = 50.0f;
        public float PowerConsumptionKw = 32.0f;
        public float BatteryChargeKwh = 237.5f;     // 95% of 250 kWh
        public float BatteryCapacityKwh = 250.0f;
        public float BatteryPercent = 95.0f;
        public PowerGridStatus GridStatus = PowerGridStatus.Nominal;

        // --- OTHER RESOURCES ---
        public int SparePartsCount = 15;
        public float CrewHealthPercent = 100.0f;
        public float AverageEquipmentConditionPercent = 98.0f;
        public float RadiationShieldingPercent = 95.0f;
        public bool IsRadiationStormActive = false;

        // --- EMERGENCY STATUS ---
        public bool IsEmergencyActive = false;
        public string EmergencyMessage = string.Empty;

        public ResourceStateData Clone()
        {
            return (ResourceStateData)this.MemberwiseClone();
        }
    }
}
