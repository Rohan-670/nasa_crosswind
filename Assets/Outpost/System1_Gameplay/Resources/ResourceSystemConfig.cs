using System;
using UnityEngine;

namespace Outpost.System1.Gameplay.Resources
{
    [CreateAssetMenu(fileName = "ResourceSystemConfig", menuName = "Outpost/Resource System Config")]
    public class ResourceSystemConfig : ScriptableObject
    {
        [Header("Simulation Timing")]
        [Tooltip("Interval between simulation recalculations in seconds (prevents Update polling).")]
        [Range(0.1f, 5.0f)]
        public float SimulationTickInterval = 1.0f;

        [Header("Oxygen & Atmosphere")]
        public float MaxOxygenStorageLiters = 5000.0f;
        public float InitialOxygenPercent = 99.2f;
        public float BaseOxygenConsumptionPerAstronaut = 0.04f; // L/s per astronaut (total 0.16 L/s for 4 crew)
        public float OxygenGeneratorProductionRate = 0.20f;     // L/s when operational and powered
        public float GreenhouseOxygenContribution = 0.04f;      // L/s produced via photosynthesis
        public float InitialCarbonDioxidePpm = 420.0f;
        public float ScrubberRemovalRatePpm = 20.0f;            // ppm/s removed when scrubber active
        public float CrewCo2ExhalationRatePpm = 3.5f;           // ppm/s added by crew respiration

        [Header("Water Loop")]
        public float MaxWaterStorageLiters = 1000.0f;
        public float InitialWaterReserveLiters = 850.0f;
        public float BaseWaterConsumptionPerAstronaut = 0.02f;  // L/s per astronaut (total 0.08 L/s)
        public float GreenhouseWaterConsumption = 0.04f;        // L/s for hydroponic beds
        public float WaterRecyclerEfficiency = 0.92f;           // 92% recovery of consumed water
        public float WaterRecyclerProductionRate = 0.11f;       // L/s when operational and powered

        [Header("Food & Greenhouse Biomass")]
        public float MaxFoodRations = 200.0f;
        public float InitialFoodRations = 120.0f;
        public float CrewFoodConsumptionRate = 0.04f;          // rations/sec
        public float GreenhouseFoodYieldRate = 0.05f;           // rations/sec produced when healthy
        public float InitialBiomassPercent = 78.0f;

        [Header("Electricity & Power Grid")]
        public float MaxBatteryCapacityKwh = 250.0f;
        public float InitialBatteryPercent = 95.0f;
        public float NominalSolarGenerationKw = 55.0f;
        public float EmergencySolarGenerationKw = 8.0f;        // Damaged array trickle
        public float BaseHabitatLoadKw = 18.0f;                // Life support, heating, seals

        [Header("Other Systems")]
        public int InitialSpareParts = 15;
        public float InitialCrewHealth = 100.0f;
        public float InitialRadiationShielding = 95.0f;

        [Header("Alert Thresholds")]
        public float OxygenWarningPercent = 85.0f;
        public float OxygenCriticalPercent = 75.0f;
        public float Co2WarningPpm = 700.0f;
        public float Co2CriticalPpm = 1200.0f;
        public float BatteryWarningPercent = 25.0f;
        public float BatteryCriticalPercent = 10.0f;
        public float WaterWarningLiters = 250.0f;
        public float WaterCriticalLiters = 100.0f;
        public float FoodWarningRations = 30.0f;
        public float FoodCriticalRations = 10.0f;
        public float CrewHealthCriticalPercent = 25.0f;

        public static ResourceSystemConfig CreateDefault()
        {
            var config = ScriptableObject.CreateInstance<ResourceSystemConfig>();
            return config;
        }
    }
}
