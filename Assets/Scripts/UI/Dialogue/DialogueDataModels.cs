using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Outpost.UI.Core;

namespace Outpost.UI.Dialogue
{
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
        public string ConversationId = "Mission1_Ch1_Arrival";
        public string MissionChapter = "Chapter 1: Arrival on the Moon";
        public ConversationTriggerType TriggerType = ConversationTriggerType.MissionMilestone;

        [Header("Dialogue Sequence")]
        public List<DialogueLine> Lines = new List<DialogueLine>();

        [Header("Branching Choices (Optional)")]
        public List<DialogueChoice> Choices = new List<DialogueChoice>();

        [Header("Flow Control")]
        public string NextConversationId = "";
        public bool IsCinematicConversation = false;

        [Header("Completion Events")]
        public UnityEvent OnConversationCompleted;
    }
}
