using System;
using UnityEngine;
using Outpost.Core.Data;
using Outpost.Core.Events;

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

    public class EmergencyManager : MonoBehaviour
    {
        [Header("Emergency State")]
        [SerializeField] private EmergencyStage currentStage = EmergencyStage.Nominal;
        [SerializeField] private FiltrationChallengeLogic filtrationLogic = new FiltrationChallengeLogic();

        public EmergencyStage CurrentStage => currentStage;
        public bool IsEmergencyActive => currentStage >= EmergencyStage.MicrometeoroidImpact && currentStage < EmergencyStage.FullyResolved;
        public FiltrationChallengeLogic FiltrationLogic => filtrationLogic;

        public event Action<EmergencyStage> OnEmergencyStageChanged;

        public static EmergencyManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
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
                    Resources.ResourceManager.Instance?.TriggerMicrometeoroidDamage();
                    break;
                case MissionChapterId.Chapter5_TheFiltrationChallenge:
                    SetStage(EmergencyStage.FiltrationAssembly);
                    break;
                case MissionChapterId.Chapter6_SaveTheOutpost:
                    SetStage(EmergencyStage.StrategicRepairPending);
                    break;
                case MissionChapterId.Chapter7_ANewBeginning:
                    SetStage(EmergencyStage.FullyResolved);
                    Resources.ResourceManager.Instance?.RestoreFullPower();
                    break;
            }
        }

        private void HandleFiltrationChallengeCompleted(bool success)
        {
            if (success)
            {
                Resources.ResourceManager.Instance?.RepairFiltrationCanister();
                SetStage(EmergencyStage.StrategicRepairPending);
            }
        }

        private void HandleRepairChoiceMade(RepairChoice choice)
        {
            SetStage(EmergencyStage.FullyResolved);
            Resources.ResourceManager.Instance?.RestoreFullPower();
        }
    }
}
