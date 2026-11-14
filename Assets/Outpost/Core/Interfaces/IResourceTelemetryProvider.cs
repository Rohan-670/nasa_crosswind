using System;
using Outpost.Core.Data;

namespace Outpost.Core.Interfaces
{
    /// <summary>
    /// Cross-System Contract between System 1 (Gameplay) and System 3 (HUD / UI).
    /// Supplies real-time telemetry of oxygen, CO2, power generation, battery, and water reserves.
    /// </summary>
    public interface IResourceTelemetryProvider
    {
        float OxygenPercent { get; }
        float CarbonDioxidePpm { get; }
        float SolarPowerGeneratedKw { get; }
        float OutpostPowerConsumptionKw { get; }
        float BatteryChargePercent { get; }
        float WaterReserveLiters { get; }
        float GreenhouseBiomassPercent { get; }
        bool IsEmergencyActive { get; }
        string EmergencyAlertMessage { get; }

        ResourceStateData CurrentState { get; }

        event Action<float, float, float, float> OnTelemetryUpdated; // o2, co2, pwr, battery
        event Action<bool, string> OnEmergencyStateChanged;
        event Action<ResourceStateData> OnDetailedTelemetryUpdated;
        event Action<ResourceCategory, ResourceAlertLevel, string> OnResourceAlert;
    }
}
