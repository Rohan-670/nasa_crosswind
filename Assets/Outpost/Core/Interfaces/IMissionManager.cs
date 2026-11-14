using System;
using Outpost.Core.Data;

namespace Outpost.Core.Interfaces
{
    public interface IMissionManager
    {
        MissionChapterId CurrentChapterId { get; }
        string CurrentChapterTitle { get; }
        string CurrentObjective { get; }
        StationLocationId TargetLocation { get; }
        RepairChoice CurrentRepairChoice { get; }

        event Action<MissionChapterId, string, string> OnChapterChanged; // id, title, objective
        event Action<string> OnObjectiveUpdated;
        event Action<RepairChoice> OnRepairChoiceSelected;

        void CompleteCurrentObjective();
        void AdvanceToChapter(MissionChapterId nextChapter);
        void SetRepairChoice(RepairChoice choice);
    }
}
