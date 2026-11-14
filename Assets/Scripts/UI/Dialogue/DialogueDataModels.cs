using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Outpost.UI.Core;

namespace Outpost.UI.Dialogue
{
    public enum CameraFramingType
    {
        Default = 0,
        CloseUpSpeaker = 1,
        MediumCrewGroup = 2,
        WideOutpost = 3,
        FocusEquipment = 4,
        ORIONHologram = 5
    }

    [System.Serializable]
    public struct ListenerReaction
    {
        public CharacterIdentifier Listener;
        public CharacterExpression Expression;
    }

    [System.Serializable]
    public class DialogueLine
    {
        public CharacterIdentifier Speaker = CharacterIdentifier.CommanderElena;
        public CharacterExpression Expression = CharacterExpression.Neutral;
        [TextArea(2, 5)] public string DialogueText = "";
        [TextArea(1, 3)] public string EducationalConceptNote = "";
        public AudioClip VoiceClip;
        public float DelayAfterLine = 0.5f;
        public bool RequirePlayerClickToAdvance = true;

        [Header("Cinematic & Group Framing")]
        public CameraFramingType Framing = CameraFramingType.Default;
        public List<ListenerReaction> CrewReactions = new List<ListenerReaction>();
    }

    [System.Serializable]
    public class DialogueChoice
    {
        public string ChoiceText = "";
        [TextArea(1, 2)] public string EducationalHint = "";
        public string ResultingConversationId = "";
        public EmergencyRepairOption EmergencyOption = EmergencyRepairOption.None;
    }

    /// <summary>
    /// ScriptableObject defining complete narrative conversations.
    /// Supports two-character, multi-character, branching decisions, and educational popups.
    /// </summary>
    [CreateAssetMenu(fileName = "NewConversation", menuName = "Outpost/Dialogue/Conversation Data")]
    public class ConversationData : ScriptableObject
    {
        [Header("Conversation Identification")]
        public string ConversationId = "Mission1_Scene1_SolarWarning";
        public string MissionChapter = "Mission 1";
        public ConversationTriggerType TriggerType = ConversationTriggerType.MissionMilestone;

        [Header("Presentation Mode")]
        public bool IsGroupDiscussion = false;
        public bool IsCinematicConversation = false;

        [Header("Dialogue Sequence")]
        public List<DialogueLine> Lines = new List<DialogueLine>();

        [Header("Branching Choices (Optional)")]
        public List<DialogueChoice> Choices = new List<DialogueChoice>();

        [Header("Flow Control")]
        public string NextConversationId = "";

        [Header("Completion Events")]
        public UnityEvent OnConversationCompleted;
    }
}
