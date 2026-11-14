using System;
using Outpost.Core.Data;

namespace Outpost.Core.Interfaces
{
    public interface IDialogueService
    {
        bool IsDialogueActive { get; }

        event Action<DialogueLine> OnLineStarted;
        event Action OnConversationCompleted;

        void PlayConversation(ConversationSequence sequence);
        void PlaySingleLine(CharacterId speaker, string text, float duration = 4.0f);
        void DismissCurrentLine();
    }
}
