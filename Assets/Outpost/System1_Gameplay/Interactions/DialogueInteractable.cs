using UnityEngine;
using Outpost.Core.Data;
using Outpost.Core.Events;

namespace Outpost.System1.Gameplay.Interactions
{
    public class DialogueInteractable : InteractableBase
    {
        [Header("Dialogue Configuration")]
        [SerializeField] private CharacterId speaker = CharacterId.Elena;
        [TextArea(2, 4)]
        [SerializeField] private string spokenText = "Welcome to Outpost Alpha, crew. Stay sharp and check your suit telemetry.";

        protected override void Awake()
        {
            CharacterProfile profile = CharacterProfile.GetProfile(speaker);
            objectName = profile.FullName;
            promptAction = $"Speak with {profile.FullName}";
            audioCue = "radio_chirp";
        }

        protected override void ExecuteInteraction(GameObject user)
        {
            OutpostEventChannel.RaiseDialogueLinePrompted(new DialogueLine(speaker, spokenText));
        }

        public void SetSpokenText(string newText)
        {
            spokenText = newText;
        }
    }
}
