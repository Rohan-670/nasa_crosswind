using System;

namespace Outpost.Core.Interfaces
{
    public interface ILifeSupportSystem
    {
        float OxygenLevel { get; }          // 0 to 100%
        float CarbonDioxideLevel { get; }   // 300 to 2000 ppm (safe < 800)
        float PowerGenerationKW { get; }    // Solar generation (e.g. 50 kW normal)
        float PowerConsumptionKW { get; }   // Current load (e.g. 35 kW normal)
        float BatteryChargePercent { get; } // 0 to 100%
        float WaterReserveLiters { get; }   // 0 to 1000 Liters
        float GreenhouseBiomass { get; }    // 0 to 100%
        bool IsEmergencyActive { get; }
        string EmergencyStatusMessage { get; }

        event Action<float, float> OnResourceLevelsChanged; // oxygen, co2
        event Action<bool, string> OnEmergencyStateChanged;

        void SetSubsystemActive(string subsystemName, bool active);
        void TriggerMicrometeoroidDamage();
        void RepairFiltrationCanister();
        void RestoreFullPower();
    }
}
