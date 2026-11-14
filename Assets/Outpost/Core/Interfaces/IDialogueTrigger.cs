using Outpost.Core.Data;

namespace Outpost.Core.Interfaces
{
    /// <summary>
    /// Cross-System Contract for Computer 3 (Interface & Dialogue).
    /// Gameplay triggers call this to execute briefing cutscenes and autonomous character banter.
    /// </summary>
    public interface IDialogueTrigger
    {
        string TriggerId { get; }
        void TriggerDialogue(ConversationSequence sequence);
        void TriggerSingleLine(CharacterId speaker, string text, float duration = 4.0f);
    }
}
