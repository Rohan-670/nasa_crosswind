using System;

namespace Outpost.Core.Data
{
    public enum MissionChapterId
    {
        Chapter1_ArrivalOnTheMoon = 1,
        Chapter2_EstablishingTheOutpost = 2,
        Chapter3_TheFirstWarning = 3,
        Chapter4_TheLifeSupportEmergency = 4,
        Chapter5_TheFiltrationChallenge = 5,
        Chapter6_SaveTheOutpost = 6,
        Chapter7_ANewBeginning = 7
    }

    public enum RepairChoice
    {
        None = 0,
        AstronautEVA = 1,        // Elena & Marcus suit up for quick, direct manual EVA repair (High resource drain, immediate fix)
        RoboticMaintenance = 2,  // Marcus deploys the automated rover robotic arm (Conserves human energy, moderate delay)
        ConservePowerDelay = 3   // Crew shuts down non-critical pods to buy time and analyze diagnostics (Safe, educational calculation)
    }

    [Serializable]
    public class MissionChapter
    {
        public MissionChapterId ChapterId;
        public string Title;
        public string ObjectiveDescription;
        public StationLocationId TargetLocation;
        public bool IsCompleted;

        public MissionChapter(MissionChapterId chapterId, string title, string objectiveDescription, StationLocationId targetLocation)
        {
            ChapterId = chapterId;
            Title = title;
            ObjectiveDescription = objectiveDescription;
            TargetLocation = targetLocation;
            IsCompleted = false;
        }
    }
}
