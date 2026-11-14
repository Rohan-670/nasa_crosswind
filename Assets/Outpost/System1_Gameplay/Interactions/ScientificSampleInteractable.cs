using UnityEngine;
using Outpost.Core.Data;
using Outpost.Core.Events;

namespace Outpost.System1.Gameplay.Interactions
{
    public class ScientificSampleInteractable : InteractableBase
    {
        [Header("Sample Scientific Data")]
        [SerializeField] private string sampleId = "Shackleton Ice Core #12";
        [SerializeField] private string geologicalMineral = "Water-Ice (H2O) + Anorthosite Feldspar";
        [TextArea(2, 4)]
        [SerializeField] private string analysisSummary = "Spectrometer scan confirms high volatile concentration. Water extraction yield is estimated at 8.4% by weight.";

        protected override void Awake()
        {
            objectName = sampleId;
            promptAction = "Examine Scientific Sample";
            audioCue = "sample_scan";
        }

        protected override void ExecuteInteraction(GameObject user)
        {
            OutpostEventChannel.RaiseDialogueLinePrompted(new DialogueLine(
                CharacterId.Aisha,
                $"[{sampleId} Analysis]: {analysisSummary} ({geologicalMineral})"
            ));

            OutpostEventChannel.RaiseEducationalFactPrompted(
                EducationalFact.GetEducationalDatabase()[(int)EducationalTopic.LunarGeology]
            );
        }
    }
}
