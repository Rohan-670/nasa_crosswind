using System;
using System.Collections.Generic;

namespace Outpost.Core.Data
{
    public enum MissionChapterId
    {
        Chapter1_Arrival = 1,
        Chapter2_OutpostEstablishment = 2,
        Chapter3_FirstWarning = 3,
        Chapter4_LifeSupportCrisis = 4,
        Chapter5_SaveTheOutpost = 5,
        Chapter6_Recovery = 6,
        Chapter7_Ending = 7,

        // Canonical aliases for earlier naming
        Chapter1_ArrivalOnTheMoon = 1,
        Chapter2_EstablishingTheOutpost = 2,
        Chapter3_TheFirstWarning = 3,
        Chapter4_TheLifeSupportEmergency = 4,
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

    [Serializable]
    public class MissionSummaryReport
    {
        public string MissionTitle = "Mission 1: The First Frontier";
        public string Location = "Lunar South Pole (Shackleton Crater Rim, 2045)";
        public string AwardTitle = "Lunar Pioneer Ribbon";
        public string AwardDescription = "Awarded for exceptional leadership, operational discipline, and crisis engineering during the establishment of Outpost Alpha.";
        public string ChosenRepairStrategy = "Standard Protocol";
        public float FinalCrewHealth = 100f;
        public float FinalSolarKw = 55f;
        public float FinalBatteryCharge = 100f;
        public List<string> Accomplishments = new List<string>();
        public List<string> StemLearnings = new List<string>();
    }
}
