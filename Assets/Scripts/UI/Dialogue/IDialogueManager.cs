using System;

namespace Outpost.UI.Dialogue
{
    /// <summary>
    /// Service contract for triggering dialogues and conversation sequences.
    /// Used by System 1 (Gameplay) and System 3.
    /// </summary>
    public interface IDialogueManager
    {
        bool IsDialogueActive { get; }
        void StartConversation(ConversationData conversation, Action onComplete = null);
        void StartConversationById(string conversationId, Action onComplete = null);
        void AdvanceDialogue();
        void SelectChoice(int choiceIndex);
    }
}
