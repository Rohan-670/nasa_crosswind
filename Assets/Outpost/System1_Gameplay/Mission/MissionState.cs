using System;
using System.Collections.Generic;
using UnityEngine;
using Outpost.Core.Data;

namespace Outpost.System1.Gameplay.Mission
{
    public enum MissionStatus
    {
        NotStarted,
        Active,
        Paused,
        Failed,
        Completed
    }

    public enum ObjectiveStatus
    {
        Inactive,
        Active,
        Completed,
        Failed
    }

    [Serializable]
    public class MissionObjective
    {
        public string ObjectiveId;
        public string Title;
        public string Description;
        public StationLocationId TargetLocationId;
        public Vector3 TargetWorldPosition;
        public ObjectiveStatus Status;
        public bool IsMandatory;
        public float TimeLimitSeconds;

        public MissionObjective(
            string objectiveId,
            string title,
            string description,
            StationLocationId targetLocationId,
            Vector3 targetWorldPosition,
            bool isMandatory = true,
            float timeLimitSeconds = 0f)
        {
            ObjectiveId = objectiveId;
            Title = title;
            Description = description;
            TargetLocationId = targetLocationId;
            TargetWorldPosition = targetWorldPosition;
            Status = ObjectiveStatus.Inactive;
            IsMandatory = isMandatory;
            TimeLimitSeconds = timeLimitSeconds;
        }
    }

    [Serializable]
    public class MissionChapterModel
    {
        public MissionChapterId ChapterId;
        public string ChapterTitle;
        public List<MissionObjective> Objectives = new List<MissionObjective>();
        public int ActiveObjectiveIndex = 0;
        public bool IsCompleted = false;

        public MissionChapterModel(MissionChapterId chapterId, string title)
        {
            ChapterId = chapterId;
            ChapterTitle = title;
            Objectives = new List<MissionObjective>();
            ActiveObjectiveIndex = 0;
            IsCompleted = false;
        }

        public MissionObjective CurrentObjective =>
            (Objectives != null && ActiveObjectiveIndex >= 0 && ActiveObjectiveIndex < Objectives.Count)
                ? Objectives[ActiveObjectiveIndex]
                : null;
    }

    [Serializable]
    public class StationMapMarker
    {
        public StationLocationId LocationId;
        public string DisplayName;
        public string ShortCode;
        public Vector3 WorldPosition;
        public bool IsDiscovered;
        public bool IsAvailable;
        public bool IsActiveObjective;

        public StationMapMarker(StationLocationId id, string name, string code, Vector3 pos, bool discovered, bool available, bool isObjective)
        {
            LocationId = id;
            DisplayName = name;
            ShortCode = code;
            WorldPosition = pos;
            IsDiscovered = discovered;
            IsAvailable = available;
            IsActiveObjective = isObjective;
        }
    }
}
