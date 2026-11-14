using System;
using UnityEngine;
using Outpost.Core.Interfaces;
using Outpost.Core.Events;

namespace Outpost.System1.Gameplay.Resources
{
    /// <summary>
    /// Resource and Life-Support Simulation Engine for System 1.
    /// Implements both IResourceTelemetryProvider (for UI) and ILifeSupportSystem (for Outpost Subsystems).
    /// </summary>
    public class ResourceManager : MonoBehaviour, IResourceTelemetryProvider, ILifeSupportSystem
    {
        [Header("Telemetry Levels")]
        [SerializeField] private float oxygenLevel = 99.2f;
        [SerializeField] private float carbonDioxidePpm = 420.0f;
        [SerializeField] private float solarPowerGeneratedKw = 50.0f;
        [SerializeField] private float powerConsumptionKw = 32.0f;
        [SerializeField] private float batteryChargePercent = 95.0f;
        [SerializeField] private float waterReserveLiters = 850.0f;
        [SerializeField] private float greenhouseBiomassPercent = 78.0f;

        [Header("Simulation Timing")]
        [SerializeField] private float simulationTickRate = 1.0f;

        private float tickTimer = 0f;
        private bool isEmergency = false;
        private string emergencyMessage = string.Empty;
        private bool isFiltrationRepaired = false;
        private bool isPowerRestored = false;

        // IResourceTelemetryProvider Implementation
        public float OxygenPercent => oxygenLevel;
        public float CarbonDioxidePpm => carbonDioxidePpm;
        public float SolarPowerGeneratedKw => solarPowerGeneratedKw;
        public float OutpostPowerConsumptionKw => powerConsumptionKw;
        public float BatteryChargePercent => batteryChargePercent;
        public float WaterReserveLiters => waterReserveLiters;
        public float GreenhouseBiomassPercent => greenhouseBiomassPercent;
        public bool IsEmergencyActive => isEmergency;
        public string EmergencyAlertMessage => emergencyMessage;

        public event Action<float, float, float, float> OnTelemetryUpdated;
        public event Action<bool, string> OnEmergencyStateChanged;

        // ILifeSupportSystem Compatibility
        public float OxygenLevel => oxygenLevel;
        public float CarbonDioxideLevel => carbonDioxidePpm;
        public float PowerGenerationKW => solarPowerGeneratedKw;
        public float PowerConsumptionKW => powerConsumptionKw;
        public float GreenhouseBiomass => greenhouseBiomassPercent;
        public string EmergencyStatusMessage => emergencyMessage;

        public event Action<float, float> OnResourceLevelsChanged;

        public static ResourceManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Update()
        {
            tickTimer += Time.deltaTime;
            if (tickTimer >= simulationTickRate)
            {
                tickTimer = 0f;
                SimulateResourceTick();
            }
        }

        private void SimulateResourceTick()
        {
            if (isEmergency)
            {
                // In emergency: power generation drops, batteries discharge
                solarPowerGeneratedKw = Mathf.Max(8.0f, solarPowerGeneratedKw - 0.2f);
                batteryChargePercent = Mathf.Max(12.0f, batteryChargePercent - 0.15f);

                if (!isFiltrationRepaired)
                {
                    // CO2 scrubbers offline: CO2 accumulates steadily
                    carbonDioxidePpm = Mathf.Min(1650.0f, carbonDioxidePpm + 8.5f);
                    oxygenLevel = Mathf.Max(82.0f, oxygenLevel - 0.12f);
                }
                else
                {
                    // Emergency filtration adapter active: scrubbers recover CO2
                    carbonDioxidePpm = Mathf.Max(480.0f, carbonDioxidePpm - 15.0f);
                    oxygenLevel = Mathf.Min(98.5f, oxygenLevel + 0.25f);
                }
            }
            else
            {
                if (isPowerRestored)
                {
                    solarPowerGeneratedKw = 52.0f;
                    batteryChargePercent = Mathf.Min(100.0f, batteryChargePercent + 0.1f);
                    carbonDioxidePpm = Mathf.Max(410.0f, carbonDioxidePpm - 5.0f);
                    oxygenLevel = Mathf.Min(99.5f, oxygenLevel + 0.1f);
                }
            }

            OnTelemetryUpdated?.Invoke(oxygenLevel, carbonDioxidePpm, solarPowerGeneratedKw, batteryChargePercent);
            OnResourceLevelsChanged?.Invoke(oxygenLevel, carbonDioxidePpm);

            OutpostEventChannel.RaiseTelemetryUpdated(oxygenLevel, carbonDioxidePpm, solarPowerGeneratedKw, batteryChargePercent);
        }

        public void SetSubsystemActive(string subsystemName, bool active)
        {
            if (active) powerConsumptionKw += 2.5f;
            else powerConsumptionKw = Mathf.Max(10f, powerConsumptionKw - 2.5f);
        }

        public void TriggerMicrometeoroidDamage()
        {
            isEmergency = true;
            isFiltrationRepaired = false;
            isPowerRestored = false;
            emergencyMessage = "CRITICAL: External relay hit by micrometeoroid! Solar grid severed. CO2 scrubbers offline!";

            OnEmergencyStateChanged?.Invoke(true, emergencyMessage);
            OutpostEventChannel.RaiseEmergencyTriggered(true, emergencyMessage);
            OutpostEventChannel.RaisePlaySfx("alarm_siren");
        }

        public void RepairFiltrationCanister()
        {
            isFiltrationRepaired = true;
            emergencyMessage = "STATUS: Emergency LiOH filtration adapter fitted! CO2 levels stabilizing.";

            OnEmergencyStateChanged?.Invoke(isEmergency, emergencyMessage);
            OutpostEventChannel.RaiseEmergencyTriggered(isEmergency, emergencyMessage);
            OutpostEventChannel.RaisePlaySfx("system_restored");
        }

        public void RestoreFullPower()
        {
            isEmergency = false;
            isPowerRestored = true;
            emergencyMessage = "NOMINAL: Power grid restored. All life-support systems operating at 100%.";

            OnEmergencyStateChanged?.Invoke(false, emergencyMessage);
            OutpostEventChannel.RaiseEmergencyTriggered(false, emergencyMessage);
            OutpostEventChannel.RaisePlaySfx("victory_chime");
        }
    }
}
