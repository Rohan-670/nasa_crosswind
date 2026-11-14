using UnityEngine;
using Outpost.Core.Data;
using Outpost.Core.Interfaces;
using Outpost.Core.Events;

namespace Outpost.System2.Environment.Characters
{
    public class CrewVisualAnchor : MonoBehaviour, IInteractable
    {
        [SerializeField] private CharacterId characterId;
        [SerializeField] private Transform headAnchor;

        private CharacterProfile profile;

        public CharacterId CharacterId => characterId;
        public string PromptText => $"Press [E] to Speak with {profile?.FullName ?? characterId.ToString()}";
        public Vector3 InteractionPosition => transform.position;
        public bool CanInteract => true;

        private void Awake()
        {
            profile = CharacterProfile.GetProfile(characterId);
        }

        public void Interact()
        {
            OutpostEventChannel.RaisePlaySfx("radio_chirp");

            switch (characterId)
            {
                case CharacterId.Elena:
                    OutpostEventChannel.RaiseDialogueLinePrompted(new DialogueLine(
                        CharacterId.Elena,
                        "Stay focused, team. Monitor your suit telemetry and check the lunar Geo Map [M] if you need station directions."
                    ));
                    break;
                case CharacterId.Marcus:
                    OutpostEventChannel.RaiseDialogueLinePrompted(new DialogueLine(
                        CharacterId.Marcus,
                        "Keep an eye on that power grid! Photovoltaic arrays are running hot, but the battery banks are loving it."
                    ));
                    break;
                case CharacterId.Aisha:
                    OutpostEventChannel.RaiseDialogueLinePrompted(new DialogueLine(
                        CharacterId.Aisha,
                        "Did you know Shackleton crater ice is over two billion years old? It holds secrets from the birth of our Solar System!"
                    ));
                    break;
                case CharacterId.Daniel:
                    OutpostEventChannel.RaiseDialogueLinePrompted(new DialogueLine(
                        CharacterId.Daniel,
                        "Vitals look steady! Remember that moving in lunar gravity burns calories quickly—don't hesitate to take a breather."
                    ));
                    break;
                case CharacterId.Orion:
                    OutpostEventChannel.RaiseDialogueLinePrompted(new DialogueLine(
                        CharacterId.Orion,
                        "O.R.I.O.N. online. All outpost subsystems report green across the primary bus. Press [M] to open station Geo Map."
                    ));
                    break;
            }
        }
    }
}
