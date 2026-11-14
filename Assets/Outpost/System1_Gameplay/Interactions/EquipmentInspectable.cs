using UnityEngine;
using Outpost.Core.Data;
using Outpost.Core.Events;

namespace Outpost.System1.Gameplay.Interactions
{
    public class EquipmentInspectable : InteractableBase
    {
        [Header("Equipment Details")]
        [SerializeField] private string equipmentModel = "Solar Array Junction Box";
        [TextArea(2, 4)]
        [SerializeField] private string inspectionReport = "Circuit breakers are nominal. Photovoltaic power routing to Main Habitat is functioning at 98% efficiency.";
        [SerializeField] private CharacterId reporter = CharacterId.Marcus;

        protected override void Awake()
        {
            promptAction = "Inspect Equipment";
            objectName = equipmentModel;
            audioCue = "terminal_beep";
        }

        protected override void ExecuteInteraction(GameObject user)
        {
            OutpostEventChannel.RaiseDialogueLinePrompted(new DialogueLine(
                reporter,
                $"[{equipmentModel} Inspection]: {inspectionReport}"
            ));
        }
    }
}
