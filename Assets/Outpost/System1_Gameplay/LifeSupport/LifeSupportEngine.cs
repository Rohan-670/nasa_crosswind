using System;
using UnityEngine;
using Outpost.Core.Interfaces;
using Outpost.Core.Events;

namespace Outpost.System1.Gameplay.LifeSupport
{
    public class LifeSupportEngine : MonoBehaviour, ILifeSupportSystem
    {
        [Header("Telemetry Settings")]
        [SerializeField] private float oxygenLevel = 99.2f;
        [SerializeField] private float carbonDioxideLevel = 420.0f; // ppm
        [SerializeField] private float powerGenerationKW = 50.0f;
        [SerializeField] private float powerConsumptionKW = 32.0f;
        [SerializeField] private float batteryChargePercent = 95.0f;
        [SerializeField] private float waterReserveLiters = 850.0f;
        [SerializeField] private float greenhouseBiomass = 78.0f;

        [Header("Simulation Rates")]
        [SerializeField] private float telemetryTickInterval = 1.0f;

        private float telemetryTimer = 0f;
        private bool isEmergency = false;
        private string emergencyMessage = string.Empty;
        private bool isFiltrationRepaired = false;
        private bool isPowerRestored = false;

        public float OxygenLevel => oxygenLevel;
        public float CarbonDioxideLevel => carbonDioxideLevel;
        public float PowerGenerationKW => powerGenerationKW;
        public float PowerConsumptionKW => powerConsumptionKW;
        public float BatteryChargePercent => batteryChargePercent;
        public float WaterReserveLiters => waterReserveLiters;
        public float GreenhouseBiomass => greenhouseBiomass;
        public bool IsEmergencyActive => isEmergency;
        public string EmergencyStatusMessage => emergencyMessage;

        public event Action<float, float> OnResourceLevelsChanged;
        public event Action<bool, string> OnEmergencyStateChanged;

        public static LifeSupportEngine Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Update()
        {
            telemetryTimer += Time.deltaTime;
            if (telemetryTimer >= telemetryTickInterval)
            {
                telemetryTimer = 0f;
                SimulateTick();
            }
        }

        private void SimulateTick()
        {
            if (isEmergency)
            {
                // In emergency: power generation is severed, battery discharges
                powerGenerationKW = Mathf.Max(8.0f, powerGenerationKW - 0.2f);
                batteryChargePercent = Mathf.Max(12.0f, batteryChargePercent - 0.15f);

                if (!isFiltrationRepaired)
                {
                    // CO2 scrubbers offline: CO2 accumulates steadily
                    carbonDioxideLevel = Mathf.Min(1650.0f, carbonDioxideLevel + 8.5f);
                    oxygenLevel = Mathf.Max(82.0f, oxygenLevel - 0.12f);
                }
                else
                {
                    // Emergency filtration adapter active: scrubbers recover CO2
                    carbonDioxideLevel = Mathf.Max(480.0f, carbonDioxideLevel - 15.0f);
                    oxygenLevel = Mathf.Min(98.5f, oxygenLevel + 0.25f);
                }
            }
            else
            {
                // Normal optimal operations
                if (isPowerRestored)
                {
                    powerGenerationKW = 52.0f;
                    batteryChargePercent = Mathf.Min(100.0f, batteryChargePercent + 0.1f);
                    carbonDioxideLevel = Mathf.Max(410.0f, carbonDioxideLevel - 5.0f);
                    oxygenLevel = Mathf.Min(99.5f, oxygenLevel + 0.1f);
                }
            }

            OnResourceLevelsChanged?.Invoke(oxygenLevel, carbonDioxideLevel);
            OutpostEventChannel.RaiseTelemetryUpdated(oxygenLevel, carbonDioxideLevel, powerGenerationKW, batteryChargePercent);
        }

        public void SetSubsystemActive(string subsystemName, bool active)
        {
            if (active)
            {
                powerConsumptionKW += 2.5f;
            }
            else
            {
                powerConsumptionKW = Mathf.Max(10f, powerConsumptionKW - 2.5f);
            }
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
