using System;
using System.Collections.Generic;
using UnityEngine;
using Outpost.Core.Data;
using Outpost.Core.Interfaces;
using Outpost.Core.Events;
using Outpost.System1.Gameplay.Resources;

namespace Outpost.System1.Gameplay.Education
{
    /// <summary>
    /// Educational STEM Engine for System 1.
    /// Provides age-appropriate (7-12) scientific explanations for lunar living:
    /// 1. Lunar Gravity (1.62 m/s², low-gravity movement)
    /// 2. Solar Ridge Tracking (Peaks of eternal light)
    /// 3. Closed-Loop Life Support (Electrolysis & water purification)
    /// 4. CO2 Scrubbing & Apollo 13 Engineering Ingenuity
    /// 5. Decision Trade-Off Reflection
    /// </summary>
    public class EducationalManager : MonoBehaviour, IEducationalService
    {
        private readonly Dictionary<EducationalTopic, EducationalFact> facts = new Dictionary<EducationalTopic, EducationalFact>();

        public event Action<EducationalFact> OnFactPresented;
        public event Action<MissionSummaryReport> OnMissionSummaryPresented;

        public static EducationalManager Instance { get; private set; }

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

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        public void EnsureInitialized()
        {
            if (Instance == null) Instance = this;

            if (facts.Count == 0)
            {
                InitializeFactDatabase();
            }
        }

        private void OnEnable()
        {
            OutpostEventChannel.ChapterChanged += HandleChapterChanged;
        }

        private void OnDisable()
        {
            OutpostEventChannel.ChapterChanged -= HandleChapterChanged;
        }

        private void InitializeFactDatabase()
        {
            facts.Clear();
            EducationalFact[] list = EducationalFact.GetEducationalDatabase();
            foreach (EducationalFact f in list)
            {
                facts[f.Topic] = f;
            }
        }

        private void HandleChapterChanged(MissionChapterId id, string title, string obj)
        {
            EnsureInitialized();

            switch (id)
            {
                case MissionChapterId.Chapter1_Arrival:
                    TriggerFactPopup(EducationalTopic.LunarGravity);
                    break;
                case MissionChapterId.Chapter2_OutpostEstablishment:
                    TriggerFactPopup(EducationalTopic.SolarPower);
                    break;
                case MissionChapterId.Chapter3_FirstWarning:
                    TriggerFactPopup(EducationalTopic.SpaceCommunication);
                    break;
                case MissionChapterId.Chapter4_LifeSupportCrisis:
                    TriggerFactPopup(EducationalTopic.CarbonDioxideRemoval);
                    break;
                case MissionChapterId.Chapter5_SaveTheOutpost:
                    TriggerFactPopup(EducationalTopic.EmergencyEngineering);
                    break;
                case MissionChapterId.Chapter6_Recovery:
                    TriggerFactPopup(EducationalTopic.WaterRecycling);
                    break;
                case MissionChapterId.Chapter7_Ending:
                    TriggerFactPopup(EducationalTopic.LunarGeology);
                    break;
            }
        }

        public EducationalFact GetFact(EducationalTopic topic)
        {
            EnsureInitialized();
            facts.TryGetValue(topic, out EducationalFact fact);
            return fact;
        }

        public void TriggerFactPopup(EducationalTopic topic)
        {
            EnsureInitialized();
            if (facts.TryGetValue(topic, out EducationalFact fact))
            {
                OnFactPresented?.Invoke(fact);
                OutpostEventChannel.RaiseEducationalFactPrompted(fact);
                OutpostEventChannel.RaisePlaySfx("info_chime");
            }
        }

        /// <summary>
        /// Generates the complete Mission 1 educational summary report.
        /// </summary>
        public MissionSummaryReport GenerateMissionSummary(RepairChoice repairChoice)
        {
            ResourceStateData rState = ResourceManager.Instance != null
                ? ResourceManager.Instance.CurrentState
                : new ResourceStateData();

            MissionSummaryReport report = new MissionSummaryReport
            {
                MissionTitle = "Mission 1: The First Frontier",
                Location = "Lunar South Pole (Shackleton Crater Rim, 2045)",
                AwardTitle = "Lunar Pioneer Ribbon",
                AwardDescription = "Awarded for exceptional leadership, operational discipline, and crisis engineering during the establishment of Outpost Alpha.",
                FinalCrewHealth = rState.CrewHealthPercent,
                FinalSolarKw = rState.SolarGenerationKw,
                FinalBatteryCharge = rState.BatteryPercent
            };

            switch (repairChoice)
            {
                case RepairChoice.AstronautEVA:
                    report.ChosenRepairStrategy = "Approach A: Astronaut Spacewalk (Direct EVA Relay Splicing)";
                    report.Accomplishments.Add("Marcus successfully executed a fast external spacewalk under elevated radiation conditions.");
                    report.Accomplishments.Add("Primary solar bus restored to 100% (55 kW), enabling rapid battery recovery.");
                    break;
                case RepairChoice.RoboticMaintenance:
                    report.ChosenRepairStrategy = "Approach B: Rover Robotic Arm Remote Maintenance";
                    report.Accomplishments.Add("The automated rover arm cleared the severed junction remotely with 0% crew radiation exposure.");
                    report.Accomplishments.Add("Solar bus restored to 80% (44 kW), balancing safety and operational power.");
                    break;
                case RepairChoice.ConservePowerDelay:
                    report.ChosenRepairStrategy = "Approach C: Power Conservation & Delayed Repair";
                    report.Accomplishments.Add("The crew shed non-critical lab modules, rationed battery reserves, and safely repaired the conduit after the storm.");
                    report.Accomplishments.Add("Demonstrated disciplined resource management with 0% radiation exposure.");
                    break;
                default:
                    report.ChosenRepairStrategy = "Autonomous Outpost Protocols";
                    report.Accomplishments.Add("Outpost life-support systems stabilized successfully.");
                    break;
            }

            // Core Educational STEM Learnings for children (Ages 7-12)
            report.StemLearnings.Add("1. Lunar Gravity: The Moon's gravity is only 1/6th of Earth's (1.62 m/s²), which makes astronauts feel very light, but momentum and inertia still require effort to stop!");
            report.StemLearnings.Add("2. Polar Solar Power: Mountain ridges at the Moon's South Pole receive near-permanent sunlight, providing clean, uninterrupted solar energy.");
            report.StemLearnings.Add("3. Closed-Loop Water Recovery: Bringing water from Earth is costly, so outpost systems purify over 92% of all humidity, breath vapor, and greywater.");
            report.StemLearnings.Add("4. Carbon Dioxide Removal: Enclosed habitats require chemical Lithium Hydroxide (LiOH) scrubbers to capture toxic exhaled CO2.");
            report.StemLearnings.Add("5. Apollo 13 Engineering: When incompatible hardware breaks, astronauts and engineers use creativity, geometry, and simple materials (cardboard and duct tape) to solve emergencies.");

            return report;
        }

        public void TriggerMissionCompleteSummary(RepairChoice choice)
        {
            EnsureInitialized();
            MissionSummaryReport summary = GenerateMissionSummary(choice);
            OnMissionSummaryPresented?.Invoke(summary);
            OutpostEventChannel.RaiseMissionSummaryPrompted(summary);
            OutpostEventChannel.RaisePlaySfx("victory_chime");
        }
    }
}
