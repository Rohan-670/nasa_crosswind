using System;
using System.Collections.Generic;

namespace Outpost.Core.Data
{
    [Serializable]
    public class DialogueLine
    {
        public CharacterId Speaker;
        public string Text;
        public float DisplayDuration;
        public string AudioCue;

        public DialogueLine(CharacterId speaker, string text, float displayDuration = 4.0f, string audioCue = "radio_chirp")
        {
            Speaker = speaker;
            Text = text;
            DisplayDuration = displayDuration;
            AudioCue = audioCue;
        }
    }

    [Serializable]
    public class ConversationSequence
    {
        public string ConversationId;
        public string Title;
        public List<DialogueLine> Lines = new List<DialogueLine>();
        public bool TriggeredOnce;

        public ConversationSequence(string id, string title)
        {
            ConversationId = id;
            Title = title;
            Lines = new List<DialogueLine>();
            TriggeredOnce = false;
        }

        public void AddLine(CharacterId speaker, string text, float duration = 4.0f)
        {
            Lines.Add(new DialogueLine(speaker, text, duration));
        }
    }
}
