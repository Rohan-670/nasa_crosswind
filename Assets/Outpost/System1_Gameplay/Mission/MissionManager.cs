using System;
using System.Collections.Generic;
using UnityEngine;
using Outpost.Core.Data;
using Outpost.Core.Interfaces;
using Outpost.Core.Events;
using Outpost.System1.Gameplay.Persistence;
using Outpost.System1.Gameplay.Education;

namespace Outpost.System1.Gameplay.Mission
{
    /// <summary>
    /// Mission & Map Data Engine for System 1 (Mission 1: The First Frontier).
    /// Manages the complete 7-Chapter mission lifecycle:
    /// Chapter 1: Arrival on the Moon
    /// Chapter 2: Establishing the Outpost
    /// Chapter 3: The First Warning
    /// Chapter 4: Life-Support Crisis
    /// Chapter 5: Save the Outpost (Multi-pathway strategic repair)
    /// Chapter 6: Recovery
    /// Chapter 7: Ending (Reward, Educational Summary, Credits)
    /// 
    /// Implements IMapDataProvider to supply live coordinates to Computer 3's visual Geo Map.
    /// </summary>
    public class MissionManager : MonoBehaviour, IMissionManager, IMapDataProvider
    {
        [Header("Mission Lifecycle")]
        [SerializeField] private MissionStatus currentStatus = MissionStatus.NotStarted;
        [SerializeField] private MissionChapterId activeChapterId = MissionChapterId.Chapter1_Arrival;
        [SerializeField] private RepairChoice currentRepairChoice = RepairChoice.None;

        [Header("Player Tracking")]
        [SerializeField] private Transform playerTransform;

        private readonly Dictionary<MissionChapterId, MissionChapterModel> chapters = new Dictionary<MissionChapterId, MissionChapterModel>();
        private readonly List<StationLocationData> outpostLocations = new List<StationLocationData>();
        private string activeFailureReason = string.Empty;

        // Lifecycle Events (Task C)
        public event Action<string> OnMissionStarted;
        public event Action<MissionObjective> OnObjectiveActivated;
        public event Action<MissionObjective> OnObjectiveCompleted;
        public event Action<MissionChapterId> OnChapterCompleted;
        public event Action<string> OnMissionFailed;
        public event Action<string> OnMissionCompleted;

        // IMissionManager Events
        public event Action<MissionChapterId, string, string> OnChapterChanged;
        public event Action<string> OnObjectiveUpdated;
        public event Action<RepairChoice> OnRepairChoiceSelected;

        // IMapDataProvider Events (Task D)
        public event Action<Vector3> OnPlayerPositionUpdated;
        public event Action<StationLocationId, Vector3, string> OnWaypointUpdated;
        public event Action<StationLocationId> OnLocationDiscovered;

        // Status & Property Accessors
        public MissionStatus CurrentStatus => currentStatus;
        public MissionChapterId CurrentChapterId => activeChapterId;
        public RepairChoice CurrentRepairChoice => currentRepairChoice;

        public string CurrentChapterTitle => chapters.ContainsKey(activeChapterId) ? chapters[activeChapterId].ChapterTitle : string.Empty;
        public string CurrentObjective => GetActiveObjective() != null ? GetActiveObjective().Description : string.Empty;
        public StationLocationId TargetLocation => GetActiveObjective() != null ? GetActiveObjective().TargetLocationId : StationLocationId.MainHabitat;

        // IMapDataProvider Properties
        public Vector3 PlayerWorldPosition => playerTransform != null ? playerTransform.position : Vector3.zero;
        public Quaternion PlayerRotation => playerTransform != null ? playerTransform.rotation : Quaternion.identity;
        public StationLocationId ActiveWaypointId => TargetLocation;
        public Vector3 ActiveWaypointWorldPosition => GetActiveObjective() != null ? GetActiveObjective().TargetWorldPosition : GetStationPosition(StationLocationId.MainHabitat);
        public string ActiveObjectiveDescription => CurrentObjective;
        public IReadOnlyList<StationLocationData> OutpostLocations => outpostLocations;

        public static MissionManager Instance { get; private set; }

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

        private bool isSubscribed = false;

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
            UnsubscribeEvents();
        }

        private void Start()
        {
            if (playerTransform == null)
            {
                GameObject player = GameObject.FindWithTag("Player");
                if (player != null) playerTransform = player.transform;
            }

            if (currentStatus == MissionStatus.NotStarted)
            {
                StartMission();
            }
        }

        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void OnDisable()
        {
            UnsubscribeEvents();
        }

        public void SubscribeEvents()
        {
            if (isSubscribed) return;
            isSubscribed = true;

            OutpostEventChannel.LocationArrived += HandleLocationArrived;
            OutpostEventChannel.FiltrationChallengeCompleted += HandleFiltrationChallengeCompleted;
            OutpostEventChannel.RepairChoiceMade += HandleRepairChoiceMade;
        }

        public void UnsubscribeEvents()
        {
            if (!isSubscribed) return;
            isSubscribed = false;

            OutpostEventChannel.LocationArrived -= HandleLocationArrived;
            OutpostEventChannel.FiltrationChallengeCompleted -= HandleFiltrationChallengeCompleted;
            OutpostEventChannel.RepairChoiceMade -= HandleRepairChoiceMade;
        }

        private void Update()
        {
            if (playerTransform != null && currentStatus == MissionStatus.Active)
            {
                OnPlayerPositionUpdated?.Invoke(playerTransform.position);
            }
        }

        public void EnsureInitialized()
        {
            if (Instance == null) Instance = this;

            if (outpostLocations.Count == 0)
            {
                InitializeStations();
            }

            if (chapters.Count == 0)
            {
                InitializeMissionStructure();
            }

            SubscribeEvents();
        }

        public void SetPlayerTransform(Transform t)
        {
            playerTransform = t;
        }

        #region Initialization
        private void InitializeStations()
        {
            outpostLocations.Clear();
            outpostLocations.AddRange(StationLocationData.GetDefaultOutpostLayout());
        }

        private Vector3 GetStationPosition(StationLocationId id)
        {
            StationLocationData loc = outpostLocations.Find(l => l.Id == id);
            return loc != null ? loc.WorldPosition : Vector3.zero;
        }

        private void InitializeMissionStructure()
        {
            chapters.Clear();

            // CHAPTER 1 – ARRIVAL ON THE MOON
            var ch1 = new MissionChapterModel(MissionChapterId.Chapter1_Arrival, "Chapter 1: Arrival on the Moon");
            ch1.Objectives.Add(new MissionObjective(
                "obj_ch1_1",
                "Touchdown Inspection",
                "Inspect the lunar lander descent stage and verify personal EVA suit telemetry.",
                StationLocationId.LandingZone,
                GetStationPosition(StationLocationId.LandingZone)
            ));
            ch1.Objectives.Add(new MissionObjective(
                "obj_ch1_2",
                "Traverse Lunar Surface",
                "Traverse the low-gravity lunar regolith toward Central Habitat Alpha.",
                StationLocationId.MainHabitat,
                GetStationPosition(StationLocationId.MainHabitat)
            ));
            ch1.Objectives.Add(new MissionObjective(
                "obj_ch1_3",
                "Airlock Entry & Habitat Inspection",
                "Inspect the exterior airlock seal and enter Central Habitat Alpha.",
                StationLocationId.MainHabitat,
                GetStationPosition(StationLocationId.MainHabitat)
            ));
            ch1.Objectives.Add(new MissionObjective(
                "obj_ch1_4",
                "ORION AI Initialization",
                "Activate the Central Habitat console to initialize O.R.I.O.N. operations network.",
                StationLocationId.MainHabitat,
                GetStationPosition(StationLocationId.MainHabitat)
            ));
            ch1.Objectives.Add(new MissionObjective(
                "obj_ch1_5",
                "Crew Arrival Briefing",
                "Conduct the arrival briefing with Commander Elena, Marcus, Aisha, and Daniel.",
                StationLocationId.MainHabitat,
                GetStationPosition(StationLocationId.MainHabitat)
            ));
            chapters[ch1.ChapterId] = ch1;

            // CHAPTER 2 – ESTABLISHING THE OUTPOST
            var ch2 = new MissionChapterModel(MissionChapterId.Chapter2_OutpostEstablishment, "Chapter 2: Establishing the Outpost");
            ch2.Objectives.Add(new MissionObjective(
                "obj_ch2_1",
                "Deploy Solar Arrays",
                "Align photovoltaic solar panels on the Ridge of Eternal Light for maximum solar tracking.",
                StationLocationId.SolarFarm,
                GetStationPosition(StationLocationId.SolarFarm)
            ));
            ch2.Objectives.Add(new MissionObjective(
                "obj_ch2_2",
                "Verify Battery Bank Reserves",
                "Inspect primary and auxiliary battery storage capacity at the habitat power bus.",
                StationLocationId.MainHabitat,
                GetStationPosition(StationLocationId.MainHabitat)
            ));
            ch2.Objectives.Add(new MissionObjective(
                "obj_ch2_3",
                "Start Oxygen Generator",
                "Start the Sabatier electrolysis oxygen generator to establish a breathable cabin atmosphere.",
                StationLocationId.MainHabitat,
                GetStationPosition(StationLocationId.MainHabitat)
            ));
            ch2.Objectives.Add(new MissionObjective(
                "obj_ch2_4",
                "Activate Water Recycling System",
                "Verify the closed-loop catalytic water recycling unit ensuring 98% moisture recovery.",
                StationLocationId.EquipmentStorage,
                GetStationPosition(StationLocationId.EquipmentStorage)
            ));
            ch2.Objectives.Add(new MissionObjective(
                "obj_ch2_5",
                "Initialize Hydroponic Greenhouse",
                "Initialize nutrient water flow and LED grow lights for salad crops and dwarf wheat.",
                StationLocationId.Greenhouse,
                GetStationPosition(StationLocationId.Greenhouse)
            ));
            ch2.Objectives.Add(new MissionObjective(
                "obj_ch2_6",
                "Science Laboratory Exploration",
                "Calibrate the spectrometer in the Science Lab for Shackleton crater ice samples.",
                StationLocationId.ScienceLab,
                GetStationPosition(StationLocationId.ScienceLab)
            ));
            ch2.Objectives.Add(new MissionObjective(
                "obj_ch2_7",
                "Outpost Operational Readiness",
                "Review all station telemetry with Commander Elena to confirm baseline readiness.",
                StationLocationId.MainHabitat,
                GetStationPosition(StationLocationId.MainHabitat)
            ));
            chapters[ch2.ChapterId] = ch2;

            // CHAPTER 3 – THE FIRST WARNING
            var ch3 = new MissionChapterModel(MissionChapterId.Chapter3_FirstWarning, "Chapter 3: The First Warning");
            ch3.Objectives.Add(new MissionObjective(
                "obj_ch3_1",
                "Solar Activity Flare Telemetry",
                "Review Aisha's spectrometer readings detecting an incoming solar proton flare.",
                StationLocationId.ScienceLab,
                GetStationPosition(StationLocationId.ScienceLab)
            ));
            ch3.Objectives.Add(new MissionObjective(
                "obj_ch3_2",
                "Pre-Incident Crew Conference",
                "Assemble in Habitat Command with Elena, Marcus, and Daniel to review solar storm protocol.",
                StationLocationId.MainHabitat,
                GetStationPosition(StationLocationId.MainHabitat)
            ));
            ch3.Objectives.Add(new MissionObjective(
                "obj_ch3_3",
                "Earth Telemetry Calibration",
                "Calibrate high-gain dish antenna at Communication Tower before storm interference.",
                StationLocationId.CommunicationTower,
                GetStationPosition(StationLocationId.CommunicationTower)
            ));
            ch3.Objectives.Add(new MissionObjective(
                "obj_ch3_4",
                "Micrometeoroid Shelter Order",
                "Seek immediate shelter inside shielded Central Habitat as debris swarm approaches.",
                StationLocationId.MainHabitat,
                GetStationPosition(StationLocationId.MainHabitat)
            ));
            ch3.Objectives.Add(new MissionObjective(
                "obj_ch3_5",
                "Electrical Substation Fault Inspection",
                "Inspect tripped primary substation breakers at breach site following micrometeoroid impact.",
                StationLocationId.EmergencyBreachSite,
                GetStationPosition(StationLocationId.EmergencyBreachSite)
            ));
            chapters[ch3.ChapterId] = ch3;

            // CHAPTER 4 – LIFE-SUPPORT CRISIS
            var ch4 = new MissionChapterModel(MissionChapterId.Chapter4_LifeSupportCrisis, "Chapter 4: Life-Support Crisis");
            ch4.Objectives.Add(new MissionObjective(
                "obj_ch4_1",
                "Atmospheric Emergency Alert",
                "Respond to critical life-support alarms: oxygen generation interrupted and CO2 scrubbers offline!",
                StationLocationId.MainHabitat,
                GetStationPosition(StationLocationId.MainHabitat)
            ));
            ch4.Objectives.Add(new MissionObjective(
                "obj_ch4_2",
                "Crew Health Monitoring",
                "Consult Medical Officer Daniel regarding hypercapnia symptoms and crew pulse-oximetry.",
                StationLocationId.MainHabitat,
                GetStationPosition(StationLocationId.MainHabitat)
            ));
            ch4.Objectives.Add(new MissionObjective(
                "obj_ch4_3",
                "Locate Replacement Canisters",
                "Follow ORION guidance to Equipment Storage to retrieve spare LiOH canisters and adapter materials.",
                StationLocationId.EquipmentStorage,
                GetStationPosition(StationLocationId.EquipmentStorage)
            ));
            ch4.Objectives.Add(new MissionObjective(
                "obj_ch4_4",
                "Apollo 13 Improvisation Challenge",
                "Assemble an airtight adapter using square LiOH canister, suit hose, manual cardboard, and duct tape.",
                StationLocationId.EquipmentStorage,
                GetStationPosition(StationLocationId.EquipmentStorage)
            ));
            ch4.Objectives.Add(new MissionObjective(
                "obj_ch4_5",
                "Restore Airflow Circulation",
                "Install the improvised adapter into circular scrubber intake and confirm CO2 drops below 650 ppm.",
                StationLocationId.MainHabitat,
                GetStationPosition(StationLocationId.MainHabitat)
            ));
            chapters[ch4.ChapterId] = ch4;

            // CHAPTER 5 – SAVE THE OUTPOST
            var ch5 = new MissionChapterModel(MissionChapterId.Chapter5_SaveTheOutpost, "Chapter 5: Save the Outpost");
            ch5.Objectives.Add(new MissionObjective(
                "obj_ch5_1",
                "Strategic Repair Briefing",
                "Review power conduit breach and repair alternatives with Commander Elena.",
                StationLocationId.MainHabitat,
                GetStationPosition(StationLocationId.MainHabitat)
            ));
            ch5.Objectives.Add(new MissionObjective(
                "obj_ch5_2",
                "Implement Strategic Repair Decision",
                "Choose between: Approach A (Marcus EVA), Approach B (Rover Robotic Arm), or Approach C (Conserve Power).",
                StationLocationId.MainHabitat,
                GetStationPosition(StationLocationId.MainHabitat)
            ));
            ch5.Objectives.Add(new MissionObjective(
                "obj_ch5_3",
                "Execute Repair Operation",
                "Monitor execution telemetry, verify power grid reconnection, and resolve station emergency.",
                StationLocationId.EmergencyBreachSite,
                GetStationPosition(StationLocationId.EmergencyBreachSite)
            ));
            chapters[ch5.ChapterId] = ch5;

            // CHAPTER 6 – RECOVERY
            var ch6 = new MissionChapterModel(MissionChapterId.Chapter6_Recovery, "Chapter 6: Recovery");
            ch6.Objectives.Add(new MissionObjective(
                "obj_ch6_1",
                "Restore Essential Equipment",
                "Re-energize secondary electrical sub-buses, reactivate greenhouse lighting, and cycle radiator valves.",
                StationLocationId.SolarFarm,
                GetStationPosition(StationLocationId.SolarFarm)
            ));
            ch6.Objectives.Add(new MissionObjective(
                "obj_ch6_2",
                "Resource Telemetry Verification",
                "Verify all resource meters (Oxygen, CO2, Power kW, Battery %, Water Liters) return to nominal levels.",
                StationLocationId.MainHabitat,
                GetStationPosition(StationLocationId.MainHabitat)
            ));
            ch6.Objectives.Add(new MissionObjective(
                "obj_ch6_3",
                "Crew Safety & Health Check",
                "Undergo comprehensive medical and radiation dosimeter screening with Medical Officer Daniel.",
                StationLocationId.MainHabitat,
                GetStationPosition(StationLocationId.MainHabitat)
            ));
            ch6.Objectives.Add(new MissionObjective(
                "obj_ch6_4",
                "Log Outpost Status & Save Progress",
                "Commit station recovery log to lunar records and save operational progress.",
                StationLocationId.MainHabitat,
                GetStationPosition(StationLocationId.MainHabitat)
            ));
            chapters[ch6.ChapterId] = ch6;

            // CHAPTER 7 – ENDING (A NEW BEGINNING)
            var ch7 = new MissionChapterModel(MissionChapterId.Chapter7_Ending, "Chapter 7: Ending - A New Beginning");
            ch7.Objectives.Add(new MissionObjective(
                "obj_ch7_1",
                "Mission 1 Debriefing",
                "Gather at the Observation Cupola with Elena, Marcus, Aisha, and Daniel for final mission review.",
                StationLocationId.MainHabitat,
                GetStationPosition(StationLocationId.MainHabitat)
            ));
            ch7.Objectives.Add(new MissionObjective(
                "obj_ch7_2",
                "Lunar Pioneer Commendation",
                "Receive the official Lunar Pioneer Ribbon for outstanding survival leadership and engineering ingenuity.",
                StationLocationId.MainHabitat,
                GetStationPosition(StationLocationId.MainHabitat)
            ));
            ch7.Objectives.Add(new MissionObjective(
                "obj_ch7_3",
                "Educational STEM Summary",
                "Review key scientific concepts: lunar gravity, solar ridge, closed-loop life support, and Apollo 13 improvisation.",
                StationLocationId.MainHabitat,
                GetStationPosition(StationLocationId.MainHabitat)
            ));
            ch7.Objectives.Add(new MissionObjective(
                "obj_ch7_4",
                "Commence Lunar Operations & Credits",
                "Celebrate Outpost Alpha's permanent operational status and trigger final mission credits.",
                StationLocationId.MainHabitat,
                GetStationPosition(StationLocationId.MainHabitat)
            ));
            chapters[ch7.ChapterId] = ch7;
        }
        #endregion

        #region Mission Lifecycle Control (Task C)
        public void StartMission()
        {
            EnsureInitialized();

            currentStatus = MissionStatus.Active;
            activeChapterId = MissionChapterId.Chapter1_Arrival;
            currentRepairChoice = RepairChoice.None;

            OnMissionStarted?.Invoke("Mission 1: The First Frontier initialized.");
            OutpostEventChannel.RaisePlaySfx("info_chime");

            ActivateCurrentChapter();
        }

        private void ActivateCurrentChapter()
        {
            if (!chapters.ContainsKey(activeChapterId)) return;

            MissionChapterModel chapter = chapters[activeChapterId];
            chapter.ActiveObjectiveIndex = 0;
            chapter.IsCompleted = false;

            MissionObjective obj = chapter.CurrentObjective;
            if (obj != null)
            {
                obj.Status = ObjectiveStatus.Active;
                OnObjectiveActivated?.Invoke(obj);
            }

            string desc = obj != null ? obj.Description : string.Empty;
            OnChapterChanged?.Invoke(activeChapterId, CurrentChapterTitle, desc);
            OutpostEventChannel.RaiseChapterChanged(activeChapterId, CurrentChapterTitle, desc);

            BroadcastObjectiveState();
        }

        public void ActivateObjective(string objectiveId)
        {
            if (!chapters.ContainsKey(activeChapterId)) return;

            MissionChapterModel chapter = chapters[activeChapterId];
            for (int i = 0; i < chapter.Objectives.Count; i++)
            {
                if (chapter.Objectives[i].ObjectiveId == objectiveId)
                {
                    chapter.ActiveObjectiveIndex = i;
                    chapter.Objectives[i].Status = ObjectiveStatus.Active;
                    OnObjectiveActivated?.Invoke(chapter.Objectives[i]);
                    BroadcastObjectiveState();
                    return;
                }
            }
        }

        public void CompleteObjective(string objectiveId)
        {
            if (!chapters.ContainsKey(activeChapterId)) return;

            MissionChapterModel chapter = chapters[activeChapterId];
            MissionObjective current = chapter.CurrentObjective;

            if (current != null && current.ObjectiveId == objectiveId)
            {
                current.Status = ObjectiveStatus.Completed;
                OnObjectiveCompleted?.Invoke(current);
                OutpostEventChannel.RaisePlaySfx("waypoint_reached");

                // Execute contextual objective actions
                ExecuteObjectiveSideEffects(objectiveId);

                // Advance to next objective in chapter if available
                if (chapter.ActiveObjectiveIndex + 1 < chapter.Objectives.Count)
                {
                    chapter.ActiveObjectiveIndex++;
                    MissionObjective next = chapter.CurrentObjective;
                    next.Status = ObjectiveStatus.Active;
                    OnObjectiveActivated?.Invoke(next);
                    BroadcastObjectiveState();
                }
                else
                {
                    // Chapter is completed!
                    CompleteChapter(activeChapterId);
                }
            }
        }

        public void CompleteChapter(MissionChapterId chapterId)
        {
            if (!chapters.ContainsKey(chapterId)) return;

            chapters[chapterId].IsCompleted = true;
            OnChapterCompleted?.Invoke(chapterId);

            // Auto-save game state on chapter transition
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.SaveGameState(chapterId, currentRepairChoice, PlayerWorldPosition);
            }

            int nextId = (int)chapterId + 1;
            if (Enum.IsDefined(typeof(MissionChapterId), nextId) && nextId <= (int)MissionChapterId.Chapter7_Ending)
            {
                activeChapterId = (MissionChapterId)nextId;
                ActivateCurrentChapter();
            }
            else
            {
                CompleteMission();
            }
        }

        public void FailMission(string failureReason)
        {
            currentStatus = MissionStatus.Failed;
            activeFailureReason = failureReason;

            OnMissionFailed?.Invoke(failureReason);
            OutpostEventChannel.RaiseEmergencyTriggered(true, $"MISSION FAILED: {failureReason}");
            OutpostEventChannel.RaisePlaySfx("alarm_siren");
        }

        public void CompleteMission()
        {
            currentStatus = MissionStatus.Completed;

            // Trigger Educational STEM Summary
            if (EducationalManager.Instance != null)
            {
                EducationalManager.Instance.TriggerMissionCompleteSummary(currentRepairChoice);
            }

            // Award Lunar Pioneer Ribbon
            OutpostEventChannel.RaiseMissionRewardAwarded(
                "Lunar Pioneer Ribbon",
                "Awarded for exceptional leadership, operational discipline, and crisis engineering during the establishment of Outpost Alpha."
            );

            // Trigger Credits & Victory
            OutpostEventChannel.RaiseCreditsTriggered(
                "OUTPOST: SURVIVE BEYOND EARTH\nMission 1: The First Frontier\n\nCOMMANDER ELENA\nENGINEER MARCUS\nSCIENTIST AISHA\nMEDICAL OFFICER DANIEL\nO.R.I.O.N.\n\nThank you for playing!"
            );

            OnMissionCompleted?.Invoke("Mission 1 completed successfully! Outpost Alpha is secure.");
            OutpostEventChannel.RaiseMissionCompleted("Mission 1 completed successfully! Outpost Alpha is secure.");
            OutpostEventChannel.RaisePlaySfx("victory_chime");
        }

        public void AdvanceToChapter(MissionChapterId nextChapter)
        {
            activeChapterId = nextChapter;
            ActivateCurrentChapter();
        }

        public void CompleteCurrentObjective()
        {
            MissionObjective current = GetActiveObjective();
            if (current != null)
            {
                CompleteObjective(current.ObjectiveId);
            }
        }

        public void SetRepairChoice(RepairChoice choice)
        {
            if (currentRepairChoice == choice) return;
            currentRepairChoice = choice;
            OnRepairChoiceSelected?.Invoke(choice);

            // If we are on the repair choice objective in Chapter 5, complete it
            MissionObjective current = GetActiveObjective();
            if (current != null && current.ObjectiveId == "obj_ch5_2" && current.Status != ObjectiveStatus.Completed)
            {
                CompleteObjective("obj_ch5_2");
            }
        }

        public MissionChapterModel GetChapter(MissionChapterId id)
        {
            chapters.TryGetValue(id, out MissionChapterModel model);
            return model;
        }

        public void ResetMission()
        {
            currentStatus = MissionStatus.NotStarted;
            activeChapterId = MissionChapterId.Chapter1_Arrival;
            currentRepairChoice = RepairChoice.None;
            InitializeMissionStructure();
        }
        #endregion

        #region Side Effects and Automation
        private void ExecuteObjectiveSideEffects(string objectiveId)
        {
            switch (objectiveId)
            {
                case "obj_ch4_4": // Apollo 13 puzzle completed
                    // Resolve scrubber emergency
                    Emergency.EmergencyManager.Instance?.ResolveEmergency(EmergencyType.CarbonDioxideAccumulation);
                    Resources.ResourceManager.Instance?.RepairFiltrationCanister();
                    break;

                case "obj_ch5_2": // Strategic repair choice made
                    if (currentRepairChoice != RepairChoice.None)
                    {
                        Emergency.EmergencyManager.Instance?.ApplyRepairChoice(currentRepairChoice);
                    }
                    break;

                case "obj_ch6_4": // Progress logged
                    if (SaveManager.Instance != null)
                    {
                        SaveManager.Instance.SaveGameState(activeChapterId, currentRepairChoice, PlayerWorldPosition);
                    }
                    break;
            }
        }
        #endregion

        #region Map Data Foundation (Task D)
        public StationLocationData GetStationLocation(StationLocationId id)
        {
            return outpostLocations.Find(l => l.Id == id);
        }

        public bool IsLocationDiscovered(StationLocationId id)
        {
            StationLocationData loc = GetStationLocation(id);
            return loc != null && loc.IsDiscovered;
        }

        public bool IsLocationAvailable(StationLocationId id)
        {
            StationLocationData loc = GetStationLocation(id);
            return loc != null && loc.IsAvailable;
        }

        public List<StationMapMarker> GetMapMarkers()
        {
            List<StationMapMarker> markers = new List<StationMapMarker>();
            StationLocationId activeWaypoint = TargetLocation;

            foreach (var loc in outpostLocations)
            {
                bool isObjective = (loc.Id == activeWaypoint);
                markers.Add(new StationMapMarker(
                    loc.Id,
                    loc.DisplayName,
                    loc.ShortCode,
                    loc.WorldPosition,
                    loc.IsDiscovered,
                    loc.IsAvailable,
                    isObjective
                ));
            }

            return markers;
        }

        public MissionObjective GetActiveObjective()
        {
            return chapters.ContainsKey(activeChapterId) ? chapters[activeChapterId].CurrentObjective : null;
        }

        private void BroadcastObjectiveState()
        {
            MissionObjective obj = GetActiveObjective();
            if (obj == null) return;

            OnObjectiveUpdated?.Invoke(obj.Description);
            OnWaypointUpdated?.Invoke(obj.TargetLocationId, obj.TargetWorldPosition, obj.Description);

            OutpostEventChannel.RaiseObjectiveUpdated(obj.Description);
            OutpostEventChannel.RaiseTargetWaypointSet(obj.TargetLocationId);
        }

        private void HandleLocationArrived(StationLocationId arrivedLocation)
        {
            StationLocationData loc = GetStationLocation(arrivedLocation);
            if (loc != null && !loc.IsDiscovered)
            {
                loc.IsDiscovered = true;
                OnLocationDiscovered?.Invoke(arrivedLocation);
            }

            MissionObjective current = GetActiveObjective();
            if (current != null && current.TargetLocationId == arrivedLocation)
            {
                CompleteCurrentObjective();
            }
        }

        private void HandleFiltrationChallengeCompleted(bool success)
        {
            if (success && activeChapterId == MissionChapterId.Chapter4_LifeSupportCrisis)
            {
                MissionObjective current = GetActiveObjective();
                if (current != null && current.ObjectiveId == "obj_ch4_4")
                {
                    CompleteObjective("obj_ch4_4");
                }
            }
        }

        private void HandleRepairChoiceMade(RepairChoice choice)
        {
            if (activeChapterId == MissionChapterId.Chapter5_SaveTheOutpost && currentRepairChoice != choice)
            {
                SetRepairChoice(choice);
            }
        }
        #endregion
    }
}
