using System;
using System.Collections.Generic;
using UnityEngine;
using Outpost.Core.Data;
using Outpost.Core.Interfaces;
using Outpost.Core.Events;

namespace Outpost.System1.Gameplay.Mission
{
    /// <summary>
    /// Mission & Map Data Engine for System 1.
    /// Manages the complete mission lifecycle (Started, Objective Activated, Objective Completed, Chapter Completed, Failed, Completed).
    /// Implements IMapDataProvider to expose player position, objective coordinates, and station markers to Computer 3.
    /// </summary>
    public class MissionManager : MonoBehaviour, IMissionManager, IMapDataProvider
    {
        [Header("Mission Lifecycle")]
        [SerializeField] private MissionStatus currentStatus = MissionStatus.NotStarted;
        [SerializeField] private MissionChapterId activeChapterId = MissionChapterId.Chapter1_ArrivalOnTheMoon;
        [SerializeField] private RepairChoice currentRepairChoice = RepairChoice.None;

        [Header("Player Tracking")]
        [SerializeField] private Transform playerTransform;

        private Dictionary<MissionChapterId, MissionChapterModel> chapters = new Dictionary<MissionChapterId, MissionChapterModel>();
        private List<StationLocationData> outpostLocations = new List<StationLocationData>();
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
            else Destroy(gameObject);

            InitializeStations();
            InitializeMissionStructure();
        }

        private void Start()
        {
            if (playerTransform == null)
            {
                GameObject player = GameObject.FindWithTag("Player");
                if (player != null) playerTransform = player.transform;
            }

            StartMission();
        }

        private void OnEnable()
        {
            OutpostEventChannel.LocationArrived += HandleLocationArrived;
            OutpostEventChannel.FiltrationChallengeCompleted += HandleFiltrationChallengeCompleted;
            OutpostEventChannel.RepairChoiceMade += HandleRepairChoiceMade;
        }

        private void OnDisable()
        {
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

            // Chapter 1
            var ch1 = new MissionChapterModel(MissionChapterId.Chapter1_ArrivalOnTheMoon, "Chapter 1: Arrival on the Moon");
            ch1.Objectives.Add(new MissionObjective(
                "obj_ch1_1",
                "Touchdown at Landing Zone",
                "Inspect the lunar descent module and verify suit pressure.",
                StationLocationId.LandingZone,
                GetStationPosition(StationLocationId.LandingZone)
            ));
            ch1.Objectives.Add(new MissionObjective(
                "obj_ch1_2",
                "Proceed to Main Habitat Alpha",
                "Walk across the regolith to Main Habitat Alpha and meet O.R.I.O.N.",
                StationLocationId.MainHabitat,
                GetStationPosition(StationLocationId.MainHabitat)
            ));
            chapters[ch1.ChapterId] = ch1;

            // Chapter 2
            var ch2 = new MissionChapterModel(MissionChapterId.Chapter2_EstablishingTheOutpost, "Chapter 2: Establishing the Outpost");
            ch2.Objectives.Add(new MissionObjective(
                "obj_ch2_1",
                "Align Solar Array Ridge",
                "Activate the photovoltaic solar arrays on the Ridge of Eternal Light.",
                StationLocationId.SolarFarm,
                GetStationPosition(StationLocationId.SolarFarm)
            ));
            ch2.Objectives.Add(new MissionObjective(
                "obj_ch2_2",
                "Inspect Hydroponic Greenhouse",
                "Verify water recycling and biomass chambers at the Greenhouse Pod.",
                StationLocationId.Greenhouse,
                GetStationPosition(StationLocationId.Greenhouse)
            ));
            chapters[ch2.ChapterId] = ch2;

            // Chapter 3
            var ch3 = new MissionChapterModel(MissionChapterId.Chapter3_TheFirstWarning, "Chapter 3: The First Warning");
            ch3.Objectives.Add(new MissionObjective(
                "obj_ch3_1",
                "Report to Comms Tower",
                "Calibrate the parabolic antenna to receive Earth solar storm telemetry.",
                StationLocationId.CommunicationTower,
                GetStationPosition(StationLocationId.CommunicationTower)
            ));
            chapters[ch3.ChapterId] = ch3;

            // Chapter 4
            var ch4 = new MissionChapterModel(MissionChapterId.Chapter4_TheLifeSupportEmergency, "Chapter 4: The Life-Support Emergency");
            ch4.Objectives.Add(new MissionObjective(
                "obj_ch4_1",
                "Life-Support Crisis",
                "Scrubbers offline! Rush to Equipment Storage for replacement canisters.",
                StationLocationId.EquipmentStorage,
                GetStationPosition(StationLocationId.EquipmentStorage)
            ));
            chapters[ch4.ChapterId] = ch4;

            // Chapter 5
            var ch5 = new MissionChapterModel(MissionChapterId.Chapter5_TheFiltrationChallenge, "Chapter 5: The Filtration Challenge");
            ch5.Objectives.Add(new MissionObjective(
                "obj_ch5_1",
                "Apollo 13 Improvisation",
                "Assemble an airtight adapter using LiOH canisters, hoses, cardboard, and tape.",
                StationLocationId.EquipmentStorage,
                GetStationPosition(StationLocationId.EquipmentStorage)
            ));
            chapters[ch5.ChapterId] = ch5;

            // Chapter 6
            var ch6 = new MissionChapterModel(MissionChapterId.Chapter6_SaveTheOutpost, "Chapter 6: Save the Outpost");
            ch6.Objectives.Add(new MissionObjective(
                "obj_ch6_1",
                "Resolve Power Breach",
                "Choose between Astronaut EVA, Robotic Maintenance, or Power Conservation.",
                StationLocationId.EmergencyBreachSite,
                GetStationPosition(StationLocationId.EmergencyBreachSite)
            ));
            chapters[ch6.ChapterId] = ch6;

            // Chapter 7
            var ch7 = new MissionChapterModel(MissionChapterId.Chapter7_ANewBeginning, "Chapter 7: A New Beginning");
            ch7.Objectives.Add(new MissionObjective(
                "obj_ch7_1",
                "Celebrate Mission Success",
                "Gather with Commander Elena, Marcus, Aisha, and Daniel at Main Habitat Alpha.",
                StationLocationId.MainHabitat,
                GetStationPosition(StationLocationId.MainHabitat)
            ));
            chapters[ch7.ChapterId] = ch7;
        }
        #endregion

        #region Mission Lifecycle Control (Task C)
        public void StartMission()
        {
            currentStatus = MissionStatus.Active;
            activeChapterId = MissionChapterId.Chapter1_ArrivalOnTheMoon;

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

            BroadcastMissionState();
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
                    BroadcastMissionState();
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

                // Advance to next objective in chapter if available
                if (chapter.ActiveObjectiveIndex + 1 < chapter.Objectives.Count)
                {
                    chapter.ActiveObjectiveIndex++;
                    MissionObjective next = chapter.CurrentObjective;
                    next.Status = ObjectiveStatus.Active;
                    OnObjectiveActivated?.Invoke(next);
                    BroadcastMissionState();
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

            int nextId = (int)chapterId + 1;
            if (Enum.IsDefined(typeof(MissionChapterId), nextId))
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
            OnMissionCompleted?.Invoke("Mission 1 completed successfully! Outpost Alpha is secure.");
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
            currentRepairChoice = choice;
            OnRepairChoiceSelected?.Invoke(choice);
            CompleteCurrentObjective();
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

        /// <summary>
        /// Supplies map marker metadata for Habitat, Landing Zone, Solar Farm, Greenhouse, Lab, Comms Tower, Rover Station.
        /// Consumed by Computer 3's visual Geo Map without Computer 1 generating visual UI.
        /// </summary>
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

        private MissionObjective GetActiveObjective()
        {
            return chapters.ContainsKey(activeChapterId) ? chapters[activeChapterId].CurrentObjective : null;
        }

        private void BroadcastMissionState()
        {
            MissionObjective obj = GetActiveObjective();
            if (obj == null) return;

            OnChapterChanged?.Invoke(activeChapterId, CurrentChapterTitle, obj.Description);
            OnObjectiveUpdated?.Invoke(obj.Description);
            OnWaypointUpdated?.Invoke(obj.TargetLocationId, obj.TargetWorldPosition, obj.Description);

            OutpostEventChannel.RaiseChapterChanged(activeChapterId, CurrentChapterTitle, obj.Description);
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
            if (success && activeChapterId == MissionChapterId.Chapter5_TheFiltrationChallenge)
            {
                CompleteCurrentObjective();
            }
        }

        private void HandleRepairChoiceMade(RepairChoice choice)
        {
            if (activeChapterId == MissionChapterId.Chapter6_SaveTheOutpost)
            {
                SetRepairChoice(choice);
            }
        }
        #endregion
    }
}
