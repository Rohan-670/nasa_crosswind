using System;
using System.Collections.Generic;
using UnityEngine;
using Outpost.Core.Data;
using Outpost.Core.Events;
using Outpost.System1.Gameplay.Equipment;
using Outpost.System1.Gameplay.Resources;

namespace Outpost.System1.Gameplay.Emergency
{
    public enum EmergencyStage
    {
        Nominal,
        SolarActivityWarning,
        MicrometeoroidImpact,
        LifeSupportFailure,
        FiltrationAssembly,
        StrategicRepairPending,
        FullyResolved
    }

    /// <summary>
    /// Central manager for all 8 Mission 1 Emergencies, equipment repair conditions,
    /// Apollo 13 filtration integration, and 3-pathway strategic decision execution.
    /// </summary>
    public class EmergencyManager : MonoBehaviour
    {
        [Header("Emergency Stage")]
        [SerializeField] private EmergencyStage currentStage = EmergencyStage.Nominal;

        [Header("Active Emergency Models")]
        [SerializeField] private List<EmergencyModel> emergencies = new List<EmergencyModel>();

        public EmergencyStage CurrentStage => currentStage;
        public bool IsEmergencyActive => GetActiveEmergencies().Count > 0 || (currentStage >= EmergencyStage.MicrometeoroidImpact && currentStage < EmergencyStage.FullyResolved);

        public event Action<EmergencyStage> OnEmergencyStageChanged;
        public event Action<EmergencyModel> OnEmergencyTriggered;
        public event Action<EmergencyModel> OnEmergencyResolved;
        public event Action<RepairOutcomeData> OnRepairOutcomeApplied;

        public static EmergencyManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else if (Instance != this)
            {
                if (Application.isPlaying) Destroy(gameObject);
                else DestroyImmediate(gameObject);
                return;
            }

            EnsureInitialized();
        }

        public void EnsureInitialized()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            if (emergencies == null || emergencies.Count == 0)
            {
                InitializeEmergencies();
            }
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        private void OnEnable()
        {
            OutpostEventChannel.ChapterChanged += HandleChapterChanged;
            OutpostEventChannel.FiltrationChallengeCompleted += HandleFiltrationChallengeCompleted;
            OutpostEventChannel.RepairChoiceMade += HandleRepairChoiceMade;
        }

        private void OnDisable()
        {
            OutpostEventChannel.ChapterChanged -= HandleChapterChanged;
            OutpostEventChannel.FiltrationChallengeCompleted -= HandleFiltrationChallengeCompleted;
            OutpostEventChannel.RepairChoiceMade -= HandleRepairChoiceMade;
        }

        private void InitializeEmergencies()
        {
            emergencies.Clear();

            // 1. Solar Panel Damage
            emergencies.Add(new EmergencyModel(
                EmergencyType.SolarPanelDamage,
                "Solar Panel Damage",
                "High-velocity micrometeoroid punctured the secondary photovoltaic cell matrix.",
                EmergencySeverity.Critical,
                StationLocationId.SolarFarm,
                "Micrometeoroid swarm preceding solar storm.",
                "SolarArrayEquipment condition drops to 15%; array offline.",
                "Solar generation drops to emergency 8.0 kW; batteries discharge rapidly.",
                "Inspect the damaged solar array at the Solar Farm and select a repair approach.",
                "EVA astronaut splicing or robotic manipulator repair.",
                "Array restored to operational status; 55.0 kW solar grid power restored."
            ));

            // 2. Electrical Distribution Fault
            emergencies.Add(new EmergencyModel(
                EmergencyType.ElectricalDistributionFault,
                "Electrical Distribution Fault",
                "High-voltage bus breaker tripped by array short-circuit, severing sub-bus feeds.",
                EmergencySeverity.Critical,
                StationLocationId.EmergencyBreachSite,
                "Electrical surge from damaged solar array conduit.",
                "Substation breakers locked out; secondary module power severed.",
                "PowerGrid enters LoadShedding; non-essential systems de-energized.",
                "Reset primary distribution breakers at the Emergency Breach Substation.",
                "Manual breaker toggle using Cable Splicer tool.",
                "Primary power bus re-energized; all subsystem circuits operational."
            ));

            // 3. Oxygen Generator Interruption
            emergencies.Add(new EmergencyModel(
                EmergencyType.OxygenGeneratorInterruption,
                "Oxygen Generator Interruption",
                "Sabatier electrolysis reactor thermal purge locked, forcing emergency shutdown.",
                EmergencySeverity.High,
                StationLocationId.MainHabitat,
                "Power fluctuation caused thermal runaway in electrolysis cell.",
                "OxygenGenerator enters GeneratorStatus.Fault; output 0 L/s.",
                "O2 production halts; cabin relies entirely on reserve tank buffer.",
                "Cycle thermal purge valve on Oxygen Generator Console in Central Hab.",
                "Cool down cell and clear fault on Habitat Console.",
                "Sabatier electrolysis nominal; +0.20 L/s O2 generation restored."
            ));

            // 4. Carbon Dioxide Accumulation
            emergencies.Add(new EmergencyModel(
                EmergencyType.CarbonDioxideAccumulation,
                "Carbon Dioxide Accumulation",
                "Primary round LiOH scrubber canister saturated; air bypass seal ruptured.",
                EmergencySeverity.Critical,
                StationLocationId.MainHabitat,
                "Chemical exhaustion of primary lithium hydroxide absorption canister.",
                "CarbonDioxideScrubber offline; condition drops to 0%.",
                "CO2 rises rapidly toward toxic hypercapnic threshold (1,650 ppm).",
                "Construct and fit emergency filtration adapter using flight manuals and tape (Apollo 13 Challenge).",
                "Assemble square LiOH canister, cardboard funnel, suit hose, and duct tape.",
                "Scrubber operating through emergency adapter; CO2 levels decreasing below 650 ppm."
            ));

            // 5. Water Recycling Failure
            emergencies.Add(new EmergencyModel(
                EmergencyType.WaterRecyclingFailure,
                "Water Recycling Failure",
                "Particulate debris clogged catalytic oxidizer bed in Water Recovery System.",
                EmergencySeverity.Medium,
                StationLocationId.MainHabitat,
                "Contaminant build-up in primary filtration bed.",
                "WaterRecyclingEquipment recovery efficiency drops to 0%.",
                "Water recycling halts (0 L/s); habitat drains unreplenished reserves.",
                "Backflush the catalytic particulate filter at the WRS manifold.",
                "Perform filter purge or install replacement membrane.",
                "WRS closed-loop recovery operational at 92% efficiency (0.11 L/s)."
            ));

            // 6. Greenhouse Power Shortage
            emergencies.Add(new EmergencyModel(
                EmergencyType.GreenhousePowerShortage,
                "Greenhouse Power Shortage",
                "Hydroponic grow light arrays shed during battery conservation brownout.",
                EmergencySeverity.Medium,
                StationLocationId.Greenhouse,
                "Load shedding tripped grow light circuits to preserve life support.",
                "GreenhouseHydroponics lights unpowered; pumps idle.",
                "Crop vitality drops; food ration production and supplemental O2 halt.",
                "Route auxiliary power feed to Greenhouse hydroponics array.",
                "Restore main power grid or re-enable auxiliary grow circuit.",
                "Hydroponic LED spectrum active; crop growth and biomass yield resume."
            ));

            // 7. Communication Blackout
            emergencies.Add(new EmergencyModel(
                EmergencyType.CommunicationBlackout,
                "Communication Blackout",
                "Deep space high-gain antenna tracking motor frozen by solar storm interference.",
                EmergencySeverity.Medium,
                StationLocationId.CommunicationTower,
                "Ionospheric storm interference and tracking gimbal lock.",
                "Communication dish lost alignment; Earth telemetry downlink severed.",
                "Remote Earth mission control data unavailable; ORION operating in local offline mode.",
                "Re-align antenna dish azimuth at the Communication Tower console.",
                "Manual gimbal calibration at Tower terminal.",
                "Earth telemetry restored; high-speed communications link confirmed."
            ));

            // 8. Habitat Temperature-Control Issue
            emergencies.Add(new EmergencyModel(
                EmergencyType.HabitatTemperatureControlIssue,
                "Habitat Temperature Control Issue",
                "External thermal radiator bypass valve stuck open during crater shadow transit.",
                EmergencySeverity.High,
                StationLocationId.MainHabitat,
                "Thermal loop coolant freezing at -130°C in shadow.",
                "Habitat radiator loop locked; internal cabin temperature drops.",
                "Electric thermal heaters draw emergency +8 kW; battery drains faster.",
                "Cycle thermal radiator bypass valve on Life Support manifold.",
                "Manual valve actuation and thermal loop purge.",
                "Cabin temperature stabilizes at 21°C; thermal power draw normalizes."
            ));
        }

        #region Emergency Lifecycle Operations

        public void TriggerEmergency(EmergencyType type)
        {
            var emergency = emergencies.Find(e => e.Type == type);
            if (emergency == null) return;

            emergency.Status = EmergencyStatus.Active;

            // Apply specific equipment and resource consequences
            ApplyEmergencyConsequences(emergency);

            OnEmergencyTriggered?.Invoke(emergency);
            OutpostEventChannel.RaiseEmergencyStateChanged(emergency);
            OutpostEventChannel.RaiseObjectiveUpdated(emergency.PlayerObjective);
            OutpostEventChannel.RaiseEmergencyTriggered(true, $"{emergency.Title}: {emergency.Description}");
            OutpostEventChannel.RaisePlaySfx("alarm_siren");
        }

        public void ResolveEmergency(EmergencyType type)
        {
            var emergency = emergencies.Find(e => e.Type == type);
            if (emergency == null) return;

            emergency.Status = EmergencyStatus.Resolved;

            // Apply recovery state
            ApplyEmergencyRecovery(emergency);

            OnEmergencyResolved?.Invoke(emergency);
            OutpostEventChannel.RaiseEmergencyStateChanged(emergency);
            OutpostEventChannel.RaisePlaySfx("system_restored");

            // If no active emergencies remain, clear alert banner
            if (GetActiveEmergencies().Count == 0)
            {
                OutpostEventChannel.RaiseEmergencyTriggered(false, "Outpost nominal: All critical emergency conditions resolved.");
            }
        }

        private void ApplyEmergencyConsequences(EmergencyModel emergency)
        {
            switch (emergency.Type)
            {
                case EmergencyType.SolarPanelDamage:
                    var solar = EquipmentManager.Instance?.GetEquipmentById("equipment_solar_array") as SolarArrayEquipment;
                    solar?.SimulateMicrometeoroidDamage();
                    ResourceManager.Instance?.TriggerMicrometeoroidDamage();
                    break;

                case EmergencyType.ElectricalDistributionFault:
                    EquipmentManager.Instance?.ApplyLoadShedding(PowerPriority.WaterAndGreenhouse);
                    break;

                case EmergencyType.OxygenGeneratorInterruption:
                    var o2 = EquipmentManager.Instance?.GetEquipmentById("equipment_o2_generator") as OxygenGeneratorEquipment;
                    o2?.SetGeneratorStatus(GeneratorStatus.Fault);
                    break;

                case EmergencyType.CarbonDioxideAccumulation:
                    var scrubber = EquipmentManager.Instance?.GetEquipmentById("equipment_co2_scrubber") as CarbonDioxideScrubberEquipment;
                    scrubber?.Damage(100f);
                    break;

                case EmergencyType.WaterRecyclingFailure:
                    var wrs = EquipmentManager.Instance?.GetEquipmentById("equipment_water_recycler") as WaterRecyclingEquipment;
                    wrs?.Damage(70f);
                    break;

                case EmergencyType.GreenhousePowerShortage:
                    var gh = EquipmentManager.Instance?.GetEquipmentById("equipment_greenhouse_hydroponics") as GreenhouseHydroponicsEquipment;
                    gh?.SetPoweredState(false);
                    break;

                case EmergencyType.CommunicationBlackout:
                    var comms = EquipmentManager.Instance?.GetEquipmentById("equipment_comms_tower");
                    comms?.SetOperationalState(false);
                    break;

                case EmergencyType.HabitatTemperatureControlIssue:
                    var shield = EquipmentManager.Instance?.GetEquipmentById("equipment_radiation_shield") as RadiationShieldEquipment;
                    shield?.Damage(40f);
                    break;
            }
        }

        private void ApplyEmergencyRecovery(EmergencyModel emergency)
        {
            switch (emergency.Type)
            {
                case EmergencyType.SolarPanelDamage:
                case EmergencyType.ElectricalDistributionFault:
                    var solar = EquipmentManager.Instance?.GetEquipmentById("equipment_solar_array") as SolarArrayEquipment;
                    solar?.Repair(100f);
                    EquipmentManager.Instance?.RestoreAllEquipmentPower();
                    ResourceManager.Instance?.RestoreFullPower();
                    break;

                case EmergencyType.OxygenGeneratorInterruption:
                    var o2 = EquipmentManager.Instance?.GetEquipmentById("equipment_o2_generator") as OxygenGeneratorEquipment;
                    o2?.SetGeneratorStatus(GeneratorStatus.Nominal);
                    break;

                case EmergencyType.CarbonDioxideAccumulation:
                    var scrubber = EquipmentManager.Instance?.GetEquipmentById("equipment_co2_scrubber") as CarbonDioxideScrubberEquipment;
                    scrubber?.FitEmergencyAdapter(true);
                    ResourceManager.Instance?.RepairFiltrationCanister();
                    break;

                case EmergencyType.WaterRecyclingFailure:
                    var wrs = EquipmentManager.Instance?.GetEquipmentById("equipment_water_recycler") as WaterRecyclingEquipment;
                    wrs?.CleanFilters();
                    break;

                case EmergencyType.GreenhousePowerShortage:
                    var gh = EquipmentManager.Instance?.GetEquipmentById("equipment_greenhouse_hydroponics") as GreenhouseHydroponicsEquipment;
                    gh?.SetPoweredState(true);
                    break;

                case EmergencyType.CommunicationBlackout:
                    var comms = EquipmentManager.Instance?.GetEquipmentById("equipment_comms_tower");
                    comms?.SetOperationalState(true);
                    break;

                case EmergencyType.HabitatTemperatureControlIssue:
                    var shield = EquipmentManager.Instance?.GetEquipmentById("equipment_radiation_shield") as RadiationShieldEquipment;
                    shield?.Repair(100f);
                    break;
            }
        }

        public EmergencyModel GetEmergency(EmergencyType type)
        {
            EnsureInitialized();
            return emergencies.Find(e => e.Type == type);
        }

        public IReadOnlyList<EmergencyModel> GetAllEmergencies()
        {
            EnsureInitialized();
            return emergencies;
        }

        public List<EmergencyModel> GetActiveEmergencies()
        {
            EnsureInitialized();
            List<EmergencyModel> active = new List<EmergencyModel>();
            for (int i = 0; i < emergencies.Count; i++)
            {
                if (emergencies[i].IsActive) active.Add(emergencies[i]);
            }
            return active;
        }

        #endregion

        #region Strategic Repair Decision Support (Chapter 6 Pathways)

        public RepairOutcomeData EvaluateRepairChoice(RepairChoice choice)
        {
            switch (choice)
            {
                case RepairChoice.AstronautEVA:
                    return new RepairOutcomeData(
                        choice,
                        "Approach A: Marcus External EVA Repair",
                        "Marcus suits up and performs a rapid spacewalk to splice the high-voltage relay with hand tools.",
                        45.0f,      // Fast repair duration
                        55.0f,      // 100% full power (55 kW)
                        100.0f,     // 100% equipment condition
                        -10.0f,     // Radiation exposure causes 10% health reduction
                        5.0f,       // Suit battery cost
                        15.0f,      // Suit oxygen cost (15 L)
                        "Marcus: 'Relay spliced and torqued down. Solar output reading a solid 55 kW! Taking minor radiation hits, but I'm heading inside.'"
                    );

                case RepairChoice.RoboticMaintenance:
                    return new RepairOutcomeData(
                        choice,
                        "Approach B: Rover Robotic Arm Maintenance",
                        "The rover's robotic manipulator arm executes the repair remotely from safe cabin telemetry.",
                        90.0f,      // Moderate repair duration
                        44.0f,      // 80% restored power (44 kW)
                        80.0f,      // Coarse mechanical alignment (80% condition)
                        0.0f,       // Zero crew radiation exposure
                        12.0f,      // Rover battery discharge
                        0.0f,       // Zero astronaut oxygen consumed
                        "Elena: 'Rover robotic manipulator arm successfully cleared the damaged junction. Array restored to 80% capacity with zero crew exposure!'"
                    );

                case RepairChoice.ConservePowerDelay:
                    return new RepairOutcomeData(
                        choice,
                        "Approach C: Conserve Power & Delay Repair",
                        "The crew sheds non-essential lab circuits and rations battery reserves until the solar storm clears.",
                        180.0f,     // Slow repair duration
                        49.5f,      // 90% restored power once storm passes
                        90.0f,      // 90% equipment condition
                        0.0f,       // Zero radiation exposure
                        35.0f,      // High battery drain during long wait
                        0.0f,       // Zero suit oxygen used
                        "Aisha: 'Solar proton storm has subsided. Marcus and the rover conducted the repair under safe baseline conditions.'"
                    );

                default:
                    return null;
            }
        }

        public RepairOutcomeData ApplyRepairChoice(RepairChoice choice)
        {
            RepairOutcomeData outcome = EvaluateRepairChoice(choice);
            if (outcome == null) return null;

            // Apply resource consequences to ResourceManager
            if (ResourceManager.Instance != null)
            {
                var state = ResourceManager.Instance.CurrentState;
                if (outcome.CrewHealthImpactPercent < 0f)
                {
                    state.CrewHealthPercent = Mathf.Clamp(state.CrewHealthPercent + outcome.CrewHealthImpactPercent, 0f, 100f);
                }
                if (outcome.OxygenCostLiters > 0f)
                {
                    state.OxygenCurrentLiters = Mathf.Clamp(state.OxygenCurrentLiters - outcome.OxygenCostLiters, 0f, state.OxygenMaxCapacityLiters);
                }
                if (outcome.BatteryCostKwh > 0f)
                {
                    state.BatteryChargeKwh = Mathf.Clamp(state.BatteryChargeKwh - outcome.BatteryCostKwh, 0f, state.BatteryCapacityKwh);
                }

                state.SolarGenerationKw = outcome.RestoredPowerKw;
                state.AverageEquipmentConditionPercent = outcome.EquipmentConditionPercent;
                state.GridStatus = PowerGridStatus.Nominal;
                state.IsEmergencyActive = false;
            }

            // Resolve solar and electrical emergencies
            ResolveEmergency(EmergencyType.SolarPanelDamage);
            ResolveEmergency(EmergencyType.ElectricalDistributionFault);

            SetStage(EmergencyStage.FullyResolved);

            OnRepairOutcomeApplied?.Invoke(outcome);
            OutpostEventChannel.RaiseRepairOutcomeEvaluated(outcome);
            OutpostEventChannel.RaiseRepairChoiceMade(choice);
            OutpostEventChannel.RaisePlaySfx("victory_chime");

            return outcome;
        }

        #endregion

        #region Event Handlers & Stage Progression

        public void SetStage(EmergencyStage stage)
        {
            currentStage = stage;
            OnEmergencyStageChanged?.Invoke(stage);
        }

        private void HandleChapterChanged(MissionChapterId chapter, string title, string obj)
        {
            switch (chapter)
            {
                case MissionChapterId.Chapter3_TheFirstWarning:
                    SetStage(EmergencyStage.SolarActivityWarning);
                    break;
                case MissionChapterId.Chapter4_TheLifeSupportEmergency:
                    SetStage(EmergencyStage.LifeSupportFailure);
                    TriggerEmergency(EmergencyType.SolarPanelDamage);
                    TriggerEmergency(EmergencyType.ElectricalDistributionFault);
                    TriggerEmergency(EmergencyType.CarbonDioxideAccumulation);
                    break;
                case MissionChapterId.Chapter5_TheFiltrationChallenge:
                    SetStage(EmergencyStage.FiltrationAssembly);
                    break;
                case MissionChapterId.Chapter6_SaveTheOutpost:
                    SetStage(EmergencyStage.StrategicRepairPending);
                    break;
                case MissionChapterId.Chapter7_ANewBeginning:
                    SetStage(EmergencyStage.FullyResolved);
                    ResolveEmergency(EmergencyType.SolarPanelDamage);
                    ResolveEmergency(EmergencyType.ElectricalDistributionFault);
                    ResolveEmergency(EmergencyType.CarbonDioxideAccumulation);
                    break;
            }
        }

        private void HandleFiltrationChallengeCompleted(bool success)
        {
            if (success)
            {
                ResolveEmergency(EmergencyType.CarbonDioxideAccumulation);
                SetStage(EmergencyStage.StrategicRepairPending);
            }
        }

        private void HandleRepairChoiceMade(RepairChoice choice)
        {
            ApplyRepairChoice(choice);
        }

        #endregion
    }
}
