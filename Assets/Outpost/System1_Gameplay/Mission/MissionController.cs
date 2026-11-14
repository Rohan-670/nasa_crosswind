using System;
using System.Collections.Generic;
using UnityEngine;
using Outpost.Core.Data;
using Outpost.Core.Interfaces;
using Outpost.Core.Events;

namespace Outpost.System1.Gameplay.Mission
{
    public class MissionController : MonoBehaviour, IMissionManager
    {
        [SerializeField] private MissionChapterId currentChapterId = MissionChapterId.Chapter1_ArrivalOnTheMoon;
        [SerializeField] private RepairChoice currentRepairChoice = RepairChoice.None;

        private Dictionary<MissionChapterId, MissionChapter> chapters = new Dictionary<MissionChapterId, MissionChapter>();

        public MissionChapterId CurrentChapterId => currentChapterId;
        public string CurrentChapterTitle => chapters.ContainsKey(currentChapterId) ? chapters[currentChapterId].Title : string.Empty;
        public string CurrentObjective => chapters.ContainsKey(currentChapterId) ? chapters[currentChapterId].ObjectiveDescription : string.Empty;
        public StationLocationId TargetLocation => chapters.ContainsKey(currentChapterId) ? chapters[currentChapterId].TargetLocation : StationLocationId.MainHabitat;
        public RepairChoice CurrentRepairChoice => currentRepairChoice;

        public event Action<MissionChapterId, string, string> OnChapterChanged;
        public event Action<string> OnObjectiveUpdated;
        public event Action<RepairChoice> OnRepairChoiceSelected;

        public static MissionController Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            InitializeChapters();
        }

        private void Start()
        {
            // Initial broadcast
            BroadcastCurrentChapter();
            TriggerChapterBriefing(currentChapterId);
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

        private void BroadcastCurrentChapter()
        {
            if (chapters.TryGetValue(currentChapterId, out MissionChapter ch))
            {
                OnChapterChanged?.Invoke(currentChapterId, ch.Title, ch.ObjectiveDescription);
                OnObjectiveUpdated?.Invoke(ch.ObjectiveDescription);

                OutpostEventChannel.RaiseChapterChanged(currentChapterId, ch.Title, ch.ObjectiveDescription);
                OutpostEventChannel.RaiseObjectiveUpdated(ch.ObjectiveDescription);
                OutpostEventChannel.RaiseTargetWaypointSet(ch.TargetLocation);
            }
        }

        private void HandleLocationArrived(StationLocationId arrivedLocation)
        {
            if (!chapters.ContainsKey(currentChapterId)) return;

            StationLocationId target = chapters[currentChapterId].TargetLocation;

            if (arrivedLocation == target)
            {
                OnTargetLocationReached();
            }
        }

        private void OnTargetLocationReached()
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
                    // Trigger emergency damage
                    LifeSupport.LifeSupportEngine.Instance?.TriggerMicrometeoroidDamage();
                    AdvanceToChapter(MissionChapterId.Chapter4_TheLifeSupportEmergency);
                    break;
                case MissionChapterId.Chapter4_TheLifeSupportEmergency:
                    AdvanceToChapter(MissionChapterId.Chapter5_TheFiltrationChallenge);
                    // Open the filtration puzzle
                    OutpostEventChannel.RaiseFiltrationChallengeOpened();
                    break;
                case MissionChapterId.Chapter5_TheFiltrationChallenge:
                    // Handled upon puzzle completion
                    break;
                case MissionChapterId.Chapter6_SaveTheOutpost:
                    // Open strategic choice modal
                    OutpostEventChannel.RaiseStrategicChoiceOpened();
                    break;
                case MissionChapterId.Chapter7_ANewBeginning:
                    OutpostEventChannel.RaiseDialogueLinePrompted(new DialogueLine(
                        CharacterId.Elena,
                        "Outstanding work, crew! Mission 1 is a complete success. The outpost is safe and self-sustaining!"
                    ));
                    break;
            }
        }

        private void HandleFiltrationChallengeCompleted(bool success)
        {
            if (success && currentChapterId == MissionChapterId.Chapter5_TheFiltrationChallenge)
            {
                LifeSupport.LifeSupportEngine.Instance?.RepairFiltrationCanister();
                AdvanceToChapter(MissionChapterId.Chapter6_SaveTheOutpost);
            }
        }

        private void HandleRepairChoiceMade(RepairChoice choice)
        {
            currentRepairChoice = choice;
            OnRepairChoiceSelected?.Invoke(choice);

            switch (choice)
            {
                case RepairChoice.AstronautEVA:
                    OutpostEventChannel.RaiseDialogueLinePrompted(new DialogueLine(
                        CharacterId.Elena,
                        "Commander Elena & Marcus suited up! Fast manual EVA completed outside. High energy used, but power restored immediately!"
                    ));
                    break;
                case RepairChoice.RoboticMaintenance:
                    OutpostEventChannel.RaiseDialogueLinePrompted(new DialogueLine(
                        CharacterId.Marcus,
                        "Rover robotic arm deployed! The mechanical gripper spliced the severed solar conduit. Safe and steady!"
                    ));
                    break;
                case RepairChoice.ConservePowerDelay:
                    OutpostEventChannel.RaiseDialogueLinePrompted(new DialogueLine(
                        CharacterId.Daniel,
                        "Smart conservation! Non-critical pod heaters shut down. Battery preserved until remote diagnostic rerouted power."
                    ));
                    break;
            }

            LifeSupport.LifeSupportEngine.Instance?.RestoreFullPower();
            AdvanceToChapter(MissionChapterId.Chapter7_ANewBeginning);
        }

        public void CompleteCurrentObjective()
        {
            OnTargetLocationReached();
        }

        public void AdvanceToChapter(MissionChapterId nextChapter)
        {
            currentChapterId = nextChapter;
            BroadcastCurrentChapter();
            TriggerChapterBriefing(nextChapter);
        }

        public void SetRepairChoice(RepairChoice choice)
        {
            HandleRepairChoiceMade(choice);
        }

        private void TriggerChapterBriefing(MissionChapterId chapter)
        {
            switch (chapter)
            {
                case MissionChapterId.Chapter1_ArrivalOnTheMoon:
                    OutpostEventChannel.RaiseDialogueLinePrompted(new DialogueLine(
                        CharacterId.Elena,
                        "Team, touchdown at the Lunar South Pole confirmed! Check your suit pressure and proceed to Main Habitat Alpha."
                    ));
                    break;
                case MissionChapterId.Chapter2_EstablishingTheOutpost:
                    OutpostEventChannel.RaiseDialogueLinePrompted(new DialogueLine(
                        CharacterId.Orion,
                        "Welcome Commander. I am O.R.I.O.N. Outpost life-support systems require activation. Please inspect the Solar Ridge."
                    ));
                    break;
                case MissionChapterId.Chapter3_TheFirstWarning:
                    OutpostEventChannel.RaiseDialogueLinePrompted(new DialogueLine(
                        CharacterId.Orion,
                        "ALERT: Solar telemetry indicates elevated coronal mass activity. Check communications dish immediately."
                    ));
                    break;
                case MissionChapterId.Chapter4_TheLifeSupportEmergency:
                    OutpostEventChannel.RaiseDialogueLinePrompted(new DialogueLine(
                        CharacterId.Daniel,
                        "Carbon dioxide levels are climbing! We need replacement filtration canisters from Equipment Storage now!"
                    ));
                    break;
                case MissionChapterId.Chapter5_TheFiltrationChallenge:
                    OutpostEventChannel.RaiseDialogueLinePrompted(new DialogueLine(
                        CharacterId.Marcus,
                        "Wait—the spare canisters are square, but our scrubber sockets are round! We're gonna have to pull an Apollo 13!"
                    ));
                    break;
                case MissionChapterId.Chapter6_SaveTheOutpost:
                    OutpostEventChannel.RaiseDialogueLinePrompted(new DialogueLine(
                        CharacterId.Elena,
                        "The scrubbers are holding! Now we have a strategic decision on how to repair the severed external relay."
                    ));
                    break;
                case MissionChapterId.Chapter7_ANewBeginning:
                    OutpostEventChannel.RaiseDialogueLinePrompted(new DialogueLine(
                        CharacterId.Aisha,
                        "All systems nominal! Power is flowing, oxygen is pure, and our lunar outpost is officially open for science!"
                    ));
                    break;
            }
        }
    }
}
