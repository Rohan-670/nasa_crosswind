using UnityEngine;
using Outpost.Core.Data;
using Outpost.Core.Interfaces;
using Outpost.Core.Events;

namespace Outpost.System2.Environment.Props
{
    public class GeologicalSample : MonoBehaviour, IInteractable
    {
        [SerializeField] private string sampleName = "Regolith Ice Core #04";
        [SerializeField] private string chemicalFormula = "H2O Volatiles + Ilmenite (FeTiO3)";

        public string PromptText => $"Press [E] to Sample: {sampleName}";
        public Vector3 InteractionPosition => transform.position;
        public bool CanInteract => true;

        public void Interact()
        {
            OutpostEventChannel.RaisePlaySfx("sample_scan");
            OutpostEventChannel.RaiseDialogueLinePrompted(new DialogueLine(
                CharacterId.Aisha,
                $"Spectrometry scan complete for {sampleName}: {chemicalFormula}. Water-ice extraction efficiency is 94%!"
            ));
            OutpostEventChannel.RaiseEducationalFactPrompted(
                EducationalFact.GetEducationalDatabase()[(int)EducationalTopic.LunarGeology]
            );
        }
    }
}
