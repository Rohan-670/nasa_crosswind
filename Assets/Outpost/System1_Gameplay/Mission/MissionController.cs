using System;
using UnityEngine;
using Outpost.Core.Data;
using Outpost.Core.Interfaces;

namespace Outpost.System1.Gameplay.Mission
{
    /// <summary>
    /// Legacy compatibility bridge for MissionController.
    /// Delegates to the centralized MissionManager.
    /// </summary>
    public class MissionController : MonoBehaviour, IMissionManager
    {
        public static MissionController Instance { get; private set; }

        public MissionChapterId CurrentChapterId => MissionManager.Instance != null ? MissionManager.Instance.CurrentChapterId : MissionChapterId.Chapter1_ArrivalOnTheMoon;
        public string CurrentChapterTitle => MissionManager.Instance != null ? MissionManager.Instance.CurrentChapterTitle : string.Empty;
        public string CurrentObjective => MissionManager.Instance != null ? MissionManager.Instance.CurrentObjective : string.Empty;
        public StationLocationId TargetLocation => MissionManager.Instance != null ? MissionManager.Instance.TargetLocation : StationLocationId.MainHabitat;
        public RepairChoice CurrentRepairChoice => MissionManager.Instance != null ? MissionManager.Instance.CurrentRepairChoice : RepairChoice.None;

        public event Action<MissionChapterId, string, string> OnChapterChanged;
        public event Action<string> OnObjectiveUpdated;
        public event Action<RepairChoice> OnRepairChoiceSelected;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void CompleteCurrentObjective() => MissionManager.Instance?.CompleteCurrentObjective();
        public void AdvanceToChapter(MissionChapterId nextChapter) => MissionManager.Instance?.AdvanceToChapter(nextChapter);
        public void SetRepairChoice(RepairChoice choice) => MissionManager.Instance?.SetRepairChoice(choice);
    }
}
