using System;
using System.Collections.Generic;
using UnityEngine;
using Outpost.Core.Data;
using Outpost.Core.Interfaces;
using Outpost.Core.Events;

namespace Outpost.System1.Gameplay.Mission
{
    /// <summary>
    /// Mission Progression Manager for System 1.
    /// Implements IMissionManager (for mission progression) and IMapDataProvider (supplying live player & waypoint data to Geo Map).
    /// </summary>
    public class MissionManager : MonoBehaviour, IMissionManager, IMapDataProvider
    {
        [Header("Mission State")]
        [SerializeField] private MissionChapterId currentChapterId = MissionChapterId.Chapter1_ArrivalOnTheMoon;
        [SerializeField] private RepairChoice currentRepairChoice = RepairChoice.None;

        [Header("Player Tracking")]
        [SerializeField] private Transform playerTransform;

        private Dictionary<MissionChapterId, MissionChapter> chapters = new Dictionary<MissionChapterId, MissionChapter>();
        private List<StationLocationData> outpostLocations = new List<StationLocationData>();

        // IMissionManager
        public MissionChapterId CurrentChapterId => currentChapterId;
        public string CurrentChapterTitle => chapters.ContainsKey(currentChapterId) ? chapters[currentChapterId].Title : string.Empty;
        public string CurrentObjective => chapters.ContainsKey(currentChapterId) ? chapters[currentChapterId].ObjectiveDescription : string.Empty;
        public StationLocationId TargetLocation => chapters.ContainsKey(currentChapterId) ? chapters[currentChapterId].TargetLocation : StationLocationId.MainHabitat;
        public RepairChoice CurrentRepairChoice => currentRepairChoice;

        public event Action<MissionChapterId, string, string> OnChapterChanged;
        public event Action<string> OnObjectiveUpdated;
        public event Action<RepairChoice> OnRepairChoiceSelected;

        // IMapDataProvider
        public Vector3 PlayerWorldPosition => playerTransform != null ? playerTransform.position : Vector3.zero;
        public Quaternion PlayerRotation => playerTransform != null ? playerTransform.rotation : Quaternion.identity;
        public StationLocationId ActiveWaypointId => TargetLocation;
        public Vector3 ActiveWaypointWorldPosition => GetLocationWorldPosition(TargetLocation);
        public string ActiveObjectiveDescription => CurrentObjective;
        public IReadOnlyList<StationLocationData> OutpostLocations => outpostLocations;

        public event Action<Vector3> OnPlayerPositionUpdated;
        public event Action<StationLocationId, Vector3, string> OnWaypointUpdated;
        public event Action<StationLocationId> OnLocationDiscovered;

        public static MissionManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            InitializeChapters();
            InitializeLocations();
        }

        private void Start()
        {
            if (playerTransform == null)
            {
                GameObject player = GameObject.FindWithTag("Player");
                if (player != null) playerTransform = player.transform;
            }

            BroadcastCurrentChapter();
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
            if (playerTransform != null)
            {
                OnPlayerPositionUpdated?.Invoke(playerTransform.position);
            }
        }

        private void InitializeChapters()
        {
            chapters[MissionChapterId.Chapter1_ArrivalOnTheMoon] = new MissionChapter(
                MissionChapterId.Chapter1_ArrivalOnTheMoon,
                "Chapter 1: Arrival on the Moon",
                "Touchdown confirmed! Walk from the Landing Zone to Main Habitat Alpha and initialize ORION.",
                StationLocationId.MainHabitat
            );

            chapters[MissionChapterId.Chapter2_EstablishingTheOutpost] = new MissionChapter(
                MissionChapterId.Chapter2_EstablishingTheOutpost,
                "Chapter 2: Establishing the Outpost",
                "Activate Outpost Subsystems: Align the Solar Array Ridge and inspect the Hydroponic Greenhouse.",
                StationLocationId.SolarFarm
            );

            chapters[MissionChapterId.Chapter3_TheFirstWarning] = new MissionChapter(
                MissionChapterId.Chapter3_TheFirstWarning,
                "Chapter 3: The First Warning",
                "Solar flare detected! Report to the Comms Tower and investigate the micrometeoroid anomaly.",
                StationLocationId.CommunicationTower
            );

            chapters[MissionChapterId.Chapter4_TheLifeSupportEmergency] = new MissionChapter(
                MissionChapterId.Chapter4_TheLifeSupportEmergency,
                "Chapter 4: The Life-Support Emergency",
                "Electrical fault detected! CO2 scrubbers offline. Rush to Equipment Storage for replacement parts.",
                StationLocationId.EquipmentStorage
            );

            chapters[MissionChapterId.Chapter5_TheFiltrationChallenge] = new MissionChapter(
                MissionChapterId.Chapter5_TheFiltrationChallenge,
                "Chapter 5: The Filtration Challenge",
                "Apollo 13 Improvisation: Assemble an emergency adapter to fit square LiOH canisters into round sockets!",
                StationLocationId.EquipmentStorage
            );

            chapters[MissionChapterId.Chapter6_SaveTheOutpost] = new MissionChapter(
                MissionChapterId.Chapter6_SaveTheOutpost,
                "Chapter 6: Save the Outpost",
                "Strategic Choice: Choose between Astronaut EVA, Robotic Maintenance, or Power Conservation.",
                StationLocationId.EmergencyBreachSite
            );

            chapters[MissionChapterId.Chapter7_ANewBeginning] = new MissionChapter(
                MissionChapterId.Chapter7_ANewBeginning,
                "Chapter 7: A New Beginning",
                "Power restored! Gather at Main Habitat Alpha with Elena, Marcus, Aisha, and Daniel to celebrate safety.",
                StationLocationId.MainHabitat
            );
        }

        private void InitializeLocations()
        {
            outpostLocations.Clear();
            outpostLocations.AddRange(StationLocationData.GetDefaultOutpostLayout());
        }

        private Vector3 GetLocationWorldPosition(StationLocationId id)
        {
            StationLocationData loc = outpostLocations.Find(l => l.Id == id);
            return loc != null ? loc.WorldPosition : Vector3.zero;
        }

        public bool IsLocationDiscovered(StationLocationId id)
        {
            StationLocationData loc = outpostLocations.Find(l => l.Id == id);
            return loc != null && loc.IsDiscovered;
        }

        public bool IsLocationAvailable(StationLocationId id)
        {
            StationLocationData loc = outpostLocations.Find(l => l.Id == id);
            return loc != null && loc.IsAvailable;
        }

        private void BroadcastCurrentChapter()
        {
            if (chapters.TryGetValue(currentChapterId, out MissionChapter ch))
            {
                OnChapterChanged?.Invoke(currentChapterId, ch.Title, ch.ObjectiveDescription);
                OnObjectiveUpdated?.Invoke(ch.ObjectiveDescription);
                OnWaypointUpdated?.Invoke(ch.TargetLocation, GetLocationWorldPosition(ch.TargetLocation), ch.ObjectiveDescription);

                OutpostEventChannel.RaiseChapterChanged(currentChapterId, ch.Title, ch.ObjectiveDescription);
                OutpostEventChannel.RaiseObjectiveUpdated(ch.ObjectiveDescription);
                OutpostEventChannel.RaiseTargetWaypointSet(ch.TargetLocation);
            }
        }

        private void HandleLocationArrived(StationLocationId arrivedLocation)
        {
            StationLocationData loc = outpostLocations.Find(l => l.Id == arrivedLocation);
            if (loc != null && !loc.IsDiscovered)
            {
                loc.IsDiscovered = true;
                OnLocationDiscovered?.Invoke(arrivedLocation);
            }

            if (!chapters.ContainsKey(currentChapterId)) return;

            if (arrivedLocation == chapters[currentChapterId].TargetLocation)
            {
                AdvanceFromCurrentObjective();
            }
        }

        private void AdvanceFromCurrentObjective()
        {
            switch (currentChapterId)
            {
                case MissionChapterId.Chapter1_ArrivalOnTheMoon:
                    AdvanceToChapter(MissionChapterId.Chapter2_EstablishingTheOutpost);
                    break;
                case MissionChapterId.Chapter2_EstablishingTheOutpost:
                    AdvanceToChapter(MissionChapterId.Chapter3_TheFirstWarning);
                    break;
                case MissionChapterId.Chapter3_TheFirstWarning:
                    AdvanceToChapter(MissionChapterId.Chapter4_TheLifeSupportEmergency);
                    break;
                case MissionChapterId.Chapter4_TheLifeSupportEmergency:
                    AdvanceToChapter(MissionChapterId.Chapter5_TheFiltrationChallenge);
                    OutpostEventChannel.RaiseFiltrationChallengeOpened();
                    break;
                case MissionChapterId.Chapter5_TheFiltrationChallenge:
                    break;
                case MissionChapterId.Chapter6_SaveTheOutpost:
                    OutpostEventChannel.RaiseStrategicChoiceOpened();
                    break;
                case MissionChapterId.Chapter7_ANewBeginning:
                    OutpostEventChannel.RaiseDialogueLinePrompted(new DialogueLine(
                        CharacterId.Elena,
                        "Mission 1 is a triumph! The outpost is fully operational and the crew is safe."
                    ));
                    break;
            }
        }

        private void HandleFiltrationChallengeCompleted(bool success)
        {
            if (success && currentChapterId == MissionChapterId.Chapter5_TheFiltrationChallenge)
            {
                AdvanceToChapter(MissionChapterId.Chapter6_SaveTheOutpost);
            }
        }

        private void HandleRepairChoiceMade(RepairChoice choice)
        {
            currentRepairChoice = choice;
            OnRepairChoiceSelected?.Invoke(choice);
            AdvanceToChapter(MissionChapterId.Chapter7_ANewBeginning);
        }

        public void CompleteCurrentObjective() => AdvanceFromCurrentObjective();

        public void AdvanceToChapter(MissionChapterId nextChapter)
        {
            currentChapterId = nextChapter;
            BroadcastCurrentChapter();
        }

        public void SetRepairChoice(RepairChoice choice) => HandleRepairChoiceMade(choice);
    }
}
