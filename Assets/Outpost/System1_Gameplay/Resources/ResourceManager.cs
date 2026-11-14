using System;
using UnityEngine;
using Outpost.Core.Data;
using Outpost.Core.Interfaces;
using Outpost.Core.Events;
using Outpost.System1.Gameplay.Equipment;

namespace Outpost.System1.Gameplay.Resources
{
    /// <summary>
    /// Centralised Lunar Resource and Life-Support Simulation Engine.
    /// Manages Oxygen, Water, Food, Electricity, Battery Storage, Equipment wear, Spare parts, and Crew Health.
    /// Implements IResourceTelemetryProvider for Computer 3 and ILifeSupportSystem for subsystem compatibility.
    /// Uses tick-based simulation to prevent excessive per-frame Update polling.
    /// </summary>
    public class ResourceManager : MonoBehaviour, IResourceTelemetryProvider, ILifeSupportSystem
    {
        [Header("Configuration")]
        [SerializeField] private ResourceSystemConfig config;

        [Header("Live Resource State")]
        [SerializeField] private ResourceStateData state = new ResourceStateData();

        [Header("Simulation Timing")]
        [SerializeField] private float simulationTickRate = 1.0f;
        private float tickTimer = 0f;

        // Emergency & filtration state flags
        private bool isFiltrationRepaired = false;
        private bool isPowerRestored = false;

        // Previous alert levels for delta notification
        private ResourceAlertLevel prevO2Alert = ResourceAlertLevel.Nominal;
        private ResourceAlertLevel prevCo2Alert = ResourceAlertLevel.Nominal;
        private ResourceAlertLevel prevBatteryAlert = ResourceAlertLevel.Nominal;
        private ResourceAlertLevel prevWaterAlert = ResourceAlertLevel.Nominal;
        private ResourceAlertLevel prevFoodAlert = ResourceAlertLevel.Nominal;
        private ResourceAlertLevel prevHealthAlert = ResourceAlertLevel.Nominal;

        // IResourceTelemetryProvider Implementation
        public float OxygenPercent => state.OxygenPercent;
        public float CarbonDioxidePpm => state.CarbonDioxidePpm;
        public float SolarPowerGeneratedKw => state.SolarGenerationKw;
        public float OutpostPowerConsumptionKw => state.PowerConsumptionKw;
        public float BatteryChargePercent => state.BatteryPercent;
        public float WaterReserveLiters => state.WaterReserveLiters;
        public float GreenhouseBiomassPercent => state.GreenhouseBiomassPercent;
        public bool IsEmergencyActive => state.IsEmergencyActive;
        public string EmergencyAlertMessage => state.EmergencyMessage;
        public ResourceStateData CurrentState => state;

        public event Action<float, float, float, float> OnTelemetryUpdated;
        public event Action<bool, string> OnEmergencyStateChanged;
        public event Action<ResourceStateData> OnDetailedTelemetryUpdated;
        public event Action<ResourceCategory, ResourceAlertLevel, string> OnResourceAlert;

        // ILifeSupportSystem Compatibility Implementation
        public float OxygenLevel => state.OxygenPercent;
        public float CarbonDioxideLevel => state.CarbonDioxidePpm;
        public float PowerGenerationKW => state.SolarGenerationKw;
        public float PowerConsumptionKW => state.PowerConsumptionKw;
        public float GreenhouseBiomass => state.GreenhouseBiomassPercent;
        public string EmergencyStatusMessage => state.EmergencyMessage;

        public event Action<float, float> OnResourceLevelsChanged;

        public static ResourceManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                if (Application.isPlaying) Destroy(gameObject);
                else DestroyImmediate(gameObject);
                return;
            }

            EnsureInitialized();
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        public void EnsureInitialized()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            if (config == null)
            {
                config = ResourceSystemConfig.CreateDefault();
            }

            simulationTickRate = config.SimulationTickInterval;
            if (state == null)
            {
                state = new ResourceStateData();
            }
            InitializeStateFromConfig();
        }

        private void InitializeStateFromConfig()
        {
            state.OxygenMaxCapacityLiters = config.MaxOxygenStorageLiters;
            state.OxygenPercent = config.InitialOxygenPercent;
            state.OxygenCurrentLiters = (config.InitialOxygenPercent / 100f) * config.MaxOxygenStorageLiters;
            state.CarbonDioxidePpm = config.InitialCarbonDioxidePpm;

            state.WaterMaxCapacityLiters = config.MaxWaterStorageLiters;
            state.WaterReserveLiters = config.InitialWaterReserveLiters;
            state.WaterRecyclingEfficiency = config.WaterRecyclerEfficiency;

            state.FoodMaxRations = config.MaxFoodRations;
            state.FoodRations = config.InitialFoodRations;
            state.GreenhouseBiomassPercent = config.InitialBiomassPercent;

            state.BatteryCapacityKwh = config.MaxBatteryCapacityKwh;
            state.BatteryPercent = config.InitialBatteryPercent;
            state.BatteryChargeKwh = (config.InitialBatteryPercent / 100f) * config.MaxBatteryCapacityKwh;
            state.SolarGenerationKw = config.NominalSolarGenerationKw;
            state.PowerConsumptionKw = config.BaseHabitatLoadKw;

            state.SparePartsCount = config.InitialSpareParts;
            state.CrewHealthPercent = config.InitialCrewHealth;
            state.RadiationShieldingPercent = config.InitialRadiationShielding;
        }

        private void Update()
        {
            // Avoid excessive Update polling: only step the simulation on fixed tick intervals
            tickTimer += Time.deltaTime;
            if (tickTimer >= simulationTickRate)
            {
                float delta = tickTimer;
                tickTimer = 0f;
                SimulateResourceTick(delta);
            }
        }

        public void SimulateResourceTick(float deltaSeconds)
        {
            if (config == null || state == null)
            {
                EnsureInitialized();
            }

            SimulateElectricity(deltaSeconds);
            SimulateOxygenAndAtmosphere(deltaSeconds);
            SimulateWater(deltaSeconds);
            SimulateFoodAndGreenhouse(deltaSeconds);
            SimulateCrewHealthAndEquipment(deltaSeconds);

            CheckResourceThresholds();
            PublishTelemetryEvents();
        }

        #region Subsystem Simulations

        private void SimulateElectricity(float deltaSeconds)
        {
            // 1. Calculate Solar Generation from registered equipment or defaults
            float solarKw = 0f;
            bool foundSolarEquipment = false;

            if (EquipmentManager.Instance != null)
            {
                var solarArrays = EquipmentManager.Instance.GetEquipmentByLocation(StationLocationId.SolarFarm);
                for (int i = 0; i < solarArrays.Count; i++)
                {
                    if (solarArrays[i] is SolarArrayEquipment solarEq)
                    {
                        solarKw += solarEq.CurrentGenerationKw;
                        foundSolarEquipment = true;
                    }
                }
            }

            if (!foundSolarEquipment)
            {
                solarKw = state.IsEmergencyActive ? config.EmergencySolarGenerationKw : config.NominalSolarGenerationKw;
            }
            state.SolarGenerationKw = solarKw;

            // 2. Calculate Load Consumption
            float equipmentLoadKw = (EquipmentManager.Instance != null) ? EquipmentManager.Instance.GetTotalActivePowerDrawKw() : 14.0f;
            float totalLoadKw = config.BaseHabitatLoadKw + equipmentLoadKw;
            state.PowerConsumptionKw = totalLoadKw;

            // 3. Power Distribution & Battery Storage Flow
            float netKw = solarKw - totalLoadKw;

            if (netKw >= 0f)
            {
                // Surplus: Charge battery
                float energyAddedKwh = (netKw * deltaSeconds) / 3600f;
                state.BatteryChargeKwh = Mathf.Min(state.BatteryCapacityKwh, state.BatteryChargeKwh + energyAddedKwh);
                state.GridStatus = PowerGridStatus.Nominal;
            }
            else
            {
                // Deficit: Discharge battery
                float energyNeededKwh = (-netKw * deltaSeconds) / 3600f;
                if (state.BatteryChargeKwh >= energyNeededKwh)
                {
                    state.BatteryChargeKwh -= energyNeededKwh;
                    state.GridStatus = PowerGridStatus.BatteryDraw;
                }
                else
                {
                    // Battery depleted: Brownout / Blackout condition
                    state.BatteryChargeKwh = 0f;
                    state.GridStatus = PowerGridStatus.LoadShedding;

                    // Shed non-essential equipment to preserve life support
                    EquipmentManager.Instance?.ApplyLoadShedding(PowerPriority.WaterAndGreenhouse);
                }
            }

            state.BatteryPercent = state.BatteryCapacityKwh > 0f ? (state.BatteryChargeKwh / state.BatteryCapacityKwh) * 100f : 0f;
            if (state.BatteryPercent <= 0f && solarKw < config.BaseHabitatLoadKw)
            {
                state.GridStatus = PowerGridStatus.Blackout;
            }
        }

        private void SimulateOxygenAndAtmosphere(float deltaSeconds)
        {
            // 1. Oxygen Production
            float o2ProdLps = 0f;
            bool hasO2Equipment = false;

            if (EquipmentManager.Instance != null)
            {
                var o2Equip = EquipmentManager.Instance.GetEquipmentById("equipment_o2_generator") as OxygenGeneratorEquipment;
                if (o2Equip != null)
                {
                    o2ProdLps = o2Equip.ProductionRateLps;
                    state.O2GeneratorStatus = o2Equip.Status;
                    hasO2Equipment = true;
                }

                var ghEquip = EquipmentManager.Instance.GetEquipmentById("equipment_greenhouse_hydroponics") as GreenhouseHydroponicsEquipment;
                if (ghEquip != null)
                {
                    o2ProdLps += ghEquip.OxygenProductionLps;
                }
            }

            if (!hasO2Equipment)
            {
                if (state.IsEmergencyActive)
                {
                    o2ProdLps = isPowerRestored ? config.OxygenGeneratorProductionRate : 0.02f;
                    state.O2GeneratorStatus = isPowerRestored ? GeneratorStatus.Nominal : GeneratorStatus.Fault;
                }
                else
                {
                    o2ProdLps = config.OxygenGeneratorProductionRate + config.GreenhouseOxygenContribution;
                    state.O2GeneratorStatus = GeneratorStatus.Nominal;
                }
            }

            // 2. Oxygen Consumption (4 astronauts)
            float o2ConsLps = config.BaseOxygenConsumptionPerAstronaut * 4f;

            state.OxygenProductionRate = o2ProdLps;
            state.OxygenConsumptionRate = o2ConsLps;

            float netO2Liters = (o2ProdLps - o2ConsLps) * deltaSeconds;
            state.OxygenCurrentLiters = Mathf.Clamp(state.OxygenCurrentLiters + netO2Liters, 0f, state.OxygenMaxCapacityLiters);
            state.OxygenPercent = (state.OxygenCurrentLiters / state.OxygenMaxCapacityLiters) * 100f;

            // 3. Carbon Dioxide Accumulation & Scrubbing
            float co2ProducedPpm = config.CrewCo2ExhalationRatePpm * deltaSeconds;
            float co2RemovedPpm = 0f;

            var scrubber = EquipmentManager.Instance?.GetEquipmentById("equipment_co2_scrubber") as CarbonDioxideScrubberEquipment;
            if (scrubber != null)
            {
                co2RemovedPpm = scrubber.ScrubRatePpm * deltaSeconds;
            }
            else
            {
                if (state.IsEmergencyActive && !isFiltrationRepaired)
                {
                    co2RemovedPpm = 0f; // Scrubber offline
                }
                else
                {
                    co2RemovedPpm = config.ScrubberRemovalRatePpm * deltaSeconds;
                }
            }

            state.CarbonDioxidePpm = Mathf.Clamp(state.CarbonDioxidePpm + co2ProducedPpm - co2RemovedPpm, 380f, 2500f);
        }

        private void SimulateWater(float deltaSeconds)
        {
            float waterConsLps = (config.BaseWaterConsumptionPerAstronaut * 4f) + config.GreenhouseWaterConsumption;
            float waterRecycleLps = 0f;

            var wrs = EquipmentManager.Instance?.GetEquipmentById("equipment_water_recycler") as WaterRecyclingEquipment;
            if (wrs != null)
            {
                waterRecycleLps = wrs.EffectiveRecyclingRateLps;
                state.WaterRecyclingEfficiency = wrs.Efficiency;
            }
            else
            {
                if (state.GridStatus != PowerGridStatus.Blackout)
                {
                    waterRecycleLps = waterConsLps * config.WaterRecyclerEfficiency;
                }
            }

            state.WaterConsumptionRate = waterConsLps;
            state.WaterRecyclingRate = waterRecycleLps;

            float netWaterLiters = (waterRecycleLps - waterConsLps) * deltaSeconds;
            state.WaterReserveLiters = Mathf.Clamp(state.WaterReserveLiters + netWaterLiters, 0f, state.WaterMaxCapacityLiters);
        }

        private void SimulateFoodAndGreenhouse(float deltaSeconds)
        {
            float foodCons = config.CrewFoodConsumptionRate * deltaSeconds;
            float foodProd = 0f;

            var greenhouse = EquipmentManager.Instance?.GetEquipmentById("equipment_greenhouse_hydroponics") as GreenhouseHydroponicsEquipment;
            if (greenhouse != null)
            {
                bool hasWater = state.WaterReserveLiters > 50f;
                greenhouse.WaterCrops(hasWater);
                foodProd = greenhouse.FoodProductionRate * deltaSeconds;
                state.GreenhouseBiomassPercent = greenhouse.CropHealth;
            }
            else
            {
                if (state.WaterReserveLiters > 50f && state.GridStatus == PowerGridStatus.Nominal)
                {
                    foodProd = config.GreenhouseFoodYieldRate * deltaSeconds;
                    state.GreenhouseBiomassPercent = Mathf.Min(100f, state.GreenhouseBiomassPercent + 0.05f * deltaSeconds);
                }
                else
                {
                    state.GreenhouseBiomassPercent = Mathf.Max(0f, state.GreenhouseBiomassPercent - 0.2f * deltaSeconds);
                }
            }

            state.FoodConsumptionRate = config.CrewFoodConsumptionRate;
            state.GreenhouseProductionRate = (deltaSeconds > 0f) ? (foodProd / deltaSeconds) : 0f;
            state.FoodRations = Mathf.Clamp(state.FoodRations - foodCons + foodProd, 0f, state.FoodMaxRations);
        }

        private void SimulateCrewHealthAndEquipment(float deltaSeconds)
        {
            // Calculate health impacts from atmospheric or resource deprivation
            float healthDelta = 0f;

            // Critical oxygen deficit
            if (state.OxygenPercent < config.OxygenCriticalPercent)
            {
                healthDelta -= 1.5f * deltaSeconds;
            }

            // High carbon dioxide toxicity
            if (state.CarbonDioxidePpm > config.Co2CriticalPpm)
            {
                healthDelta -= 2.0f * deltaSeconds;
            }
            else if (state.CarbonDioxidePpm > config.Co2WarningPpm)
            {
                healthDelta -= 0.5f * deltaSeconds;
            }

            // Water starvation
            if (state.WaterReserveLiters <= 0f)
            {
                healthDelta -= 1.0f * deltaSeconds;
            }

            // Nominal recovery if all life support conditions are safe
            if (healthDelta >= 0f && state.OxygenPercent >= config.OxygenWarningPercent && state.CarbonDioxidePpm <= config.Co2WarningPpm && state.WaterReserveLiters > config.WaterWarningLiters)
            {
                healthDelta = 0.5f * deltaSeconds; // Passive recovery
            }

            state.CrewHealthPercent = Mathf.Clamp(state.CrewHealthPercent + healthDelta, 0f, 100f);

            // Equipment wear & condition
            if (EquipmentManager.Instance != null)
            {
                state.AverageEquipmentConditionPercent = EquipmentManager.Instance.GetAverageEquipmentCondition();
            }
        }

        #endregion

        #region Threshold Alerts & Notification Publishing

        private void CheckResourceThresholds()
        {
            // Oxygen
            ResourceAlertLevel o2Alert = (state.OxygenPercent <= config.OxygenCriticalPercent) ? ResourceAlertLevel.Critical :
                                         (state.OxygenPercent <= config.OxygenWarningPercent) ? ResourceAlertLevel.Warning : ResourceAlertLevel.Nominal;
            if (o2Alert != prevO2Alert)
            {
                prevO2Alert = o2Alert;
                if (o2Alert != ResourceAlertLevel.Nominal)
                {
                    RaiseAlert(ResourceCategory.Oxygen, o2Alert, $"OXYGEN ALERT: Habitat atmospheric oxygen is at {state.OxygenPercent:F1}%!");
                }
            }

            // CO2
            ResourceAlertLevel co2Alert = (state.CarbonDioxidePpm >= config.Co2CriticalPpm) ? ResourceAlertLevel.Critical :
                                          (state.CarbonDioxidePpm >= config.Co2WarningPpm) ? ResourceAlertLevel.Warning : ResourceAlertLevel.Nominal;
            if (co2Alert != prevCo2Alert)
            {
                prevCo2Alert = co2Alert;
                if (co2Alert != ResourceAlertLevel.Nominal)
                {
                    RaiseAlert(ResourceCategory.Oxygen, co2Alert, $"CO2 ALERT: Carbon dioxide accumulation reaching {state.CarbonDioxidePpm:F0} ppm!");
                }
            }

            // Battery
            ResourceAlertLevel batAlert = (state.BatteryPercent <= config.BatteryCriticalPercent) ? ResourceAlertLevel.Critical :
                                          (state.BatteryPercent <= config.BatteryWarningPercent) ? ResourceAlertLevel.Warning : ResourceAlertLevel.Nominal;
            if (batAlert != prevBatteryAlert)
            {
                prevBatteryAlert = batAlert;
                if (batAlert != ResourceAlertLevel.Nominal)
                {
                    RaiseAlert(ResourceCategory.Electricity, batAlert, $"POWER ALERT: Battery reserves at {state.BatteryPercent:F0}%!");
                }
            }

            // Water
            ResourceAlertLevel waterAlert = (state.WaterReserveLiters <= config.WaterCriticalLiters) ? ResourceAlertLevel.Critical :
                                            (state.WaterReserveLiters <= config.WaterWarningLiters) ? ResourceAlertLevel.Warning : ResourceAlertLevel.Nominal;
            if (waterAlert != prevWaterAlert)
            {
                prevWaterAlert = waterAlert;
                if (waterAlert != ResourceAlertLevel.Nominal)
                {
                    RaiseAlert(ResourceCategory.Water, waterAlert, $"WATER ALERT: Potable water reserve down to {state.WaterReserveLiters:F0} L!");
                }
            }

            // Food
            ResourceAlertLevel foodAlert = (state.FoodRations <= config.FoodCriticalRations) ? ResourceAlertLevel.Critical :
                                           (state.FoodRations <= config.FoodWarningRations) ? ResourceAlertLevel.Warning : ResourceAlertLevel.Nominal;
            if (foodAlert != prevFoodAlert)
            {
                prevFoodAlert = foodAlert;
                if (foodAlert != ResourceAlertLevel.Nominal)
                {
                    RaiseAlert(ResourceCategory.Food, foodAlert, $"RATION ALERT: Galley food supply down to {state.FoodRations:F0} meals!");
                }
            }

            // Crew Health
            ResourceAlertLevel healthAlert = (state.CrewHealthPercent <= config.CrewHealthCriticalPercent) ? ResourceAlertLevel.Critical : ResourceAlertLevel.Nominal;
            if (healthAlert != prevHealthAlert)
            {
                prevHealthAlert = healthAlert;
                if (healthAlert != ResourceAlertLevel.Nominal)
                {
                    RaiseAlert(ResourceCategory.CrewHealth, healthAlert, $"MEDICAL EMERGENCY: Crew health dropping rapidly to {state.CrewHealthPercent:F0}%!");
                }
            }
        }

        private void RaiseAlert(ResourceCategory category, ResourceAlertLevel level, string message)
        {
            OnResourceAlert?.Invoke(category, level, message);
            OutpostEventChannel.RaiseResourceAlertTriggered(category, level, message);

            if (level == ResourceAlertLevel.Critical)
            {
                OutpostEventChannel.RaisePlaySfx("alarm_siren");
            }
            else
            {
                OutpostEventChannel.RaisePlaySfx("terminal_beep");
            }
        }

        private void PublishTelemetryEvents()
        {
            OnTelemetryUpdated?.Invoke(state.OxygenPercent, state.CarbonDioxidePpm, state.SolarGenerationKw, state.BatteryPercent);
            OnResourceLevelsChanged?.Invoke(state.OxygenPercent, state.CarbonDioxidePpm);
            OnDetailedTelemetryUpdated?.Invoke(state);

            OutpostEventChannel.RaiseTelemetryUpdated(state.OxygenPercent, state.CarbonDioxidePpm, state.SolarGenerationKw, state.BatteryPercent);
            OutpostEventChannel.RaiseDetailedTelemetryUpdated(state);
        }

        #endregion

        #region Mission Milestones & External Controls

        public void SetSubsystemActive(string subsystemName, bool active)
        {
            var eq = EquipmentManager.Instance?.GetEquipmentById(subsystemName);
            if (eq != null)
            {
                eq.SetOperationalState(active);
            }
        }

        public void TriggerMicrometeoroidDamage()
        {
            state.IsEmergencyActive = true;
            isFiltrationRepaired = false;
            isPowerRestored = false;
            state.SolarGenerationKw = config.EmergencySolarGenerationKw;
            state.GridStatus = PowerGridStatus.BatteryDraw;
            state.EmergencyMessage = "CRITICAL: External relay hit by micrometeoroid! Solar grid severed. CO2 scrubbers offline!";

            var solar = EquipmentManager.Instance?.GetEquipmentById("equipment_solar_array") as SolarArrayEquipment;
            solar?.SimulateMicrometeoroidDamage();

            var scrubber = EquipmentManager.Instance?.GetEquipmentById("equipment_co2_scrubber") as CarbonDioxideScrubberEquipment;
            scrubber?.Damage(90f);

            OnEmergencyStateChanged?.Invoke(true, state.EmergencyMessage);
            OutpostEventChannel.RaiseEmergencyTriggered(true, state.EmergencyMessage);
            OutpostEventChannel.RaisePlaySfx("alarm_siren");
            PublishTelemetryEvents();
        }

        public void RepairFiltrationCanister()
        {
            isFiltrationRepaired = true;
            state.EmergencyMessage = "STATUS: Emergency LiOH filtration adapter fitted! CO2 levels stabilizing.";

            var scrubber = EquipmentManager.Instance?.GetEquipmentById("equipment_co2_scrubber") as CarbonDioxideScrubberEquipment;
            scrubber?.FitEmergencyAdapter(true);

            OnEmergencyStateChanged?.Invoke(state.IsEmergencyActive, state.EmergencyMessage);
            OutpostEventChannel.RaiseEmergencyTriggered(state.IsEmergencyActive, state.EmergencyMessage);
            OutpostEventChannel.RaisePlaySfx("system_restored");
            PublishTelemetryEvents();
        }

        public void RestoreFullPower()
        {
            state.IsEmergencyActive = false;
            isPowerRestored = true;
            state.SolarGenerationKw = config.NominalSolarGenerationKw;
            state.GridStatus = PowerGridStatus.Nominal;
            state.EmergencyMessage = "NOMINAL: Power grid restored. All life-support systems operating at 100%.";

            var solar = EquipmentManager.Instance?.GetEquipmentById("equipment_solar_array") as SolarArrayEquipment;
            solar?.Repair(100f);

            EquipmentManager.Instance?.RestoreAllEquipmentPower();

            OnEmergencyStateChanged?.Invoke(false, state.EmergencyMessage);
            OutpostEventChannel.RaiseEmergencyTriggered(false, state.EmergencyMessage);
            OutpostEventChannel.RaisePlaySfx("victory_chime");
            PublishTelemetryEvents();
        }

        #endregion

        #region State Persistence (Save & Restore)

        public ResourceStateData ExportState()
        {
            return state.Clone();
        }

        public void ImportState(ResourceStateData imported)
        {
            if (imported == null) return;

            state = imported.Clone();
            PublishTelemetryEvents();
        }

        #endregion
    }
}
