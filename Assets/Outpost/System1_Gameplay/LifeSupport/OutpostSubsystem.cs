using UnityEngine;
using Outpost.Core.Data;
using Outpost.Core.Interfaces;
using Outpost.Core.Events;
using Outpost.System1.Gameplay.Interactions;

namespace Outpost.System1.Gameplay.LifeSupport
{
    public class OutpostSubsystem : InteractableBase
    {
        [SerializeField] private StationLocationId locationId;
        [SerializeField] private string subsystemName = "Outpost System";
        [SerializeField] private bool isOperational = true;

        public StationLocationId LocationId => locationId;
        public string SubsystemName => subsystemName;
        public bool IsOperational => isOperational;

        public override string PromptText => $"{subsystemName}: Press [E] to Inspect";
        public string ContextDescription => $"{subsystemName} Status: {(isOperational ? "Operational" : "Offline")}";

        protected override void Awake()
        {
            base.Awake();
            objectName = subsystemName;
            promptAction = "Inspect";
        }

        public void SetOperational(bool operational)
        {
            isOperational = operational;
        }

        protected override void ExecuteInteraction(GameObject interactor)
        {
            OutpostEventChannel.RaiseLocationArrived(locationId);
            OutpostEventChannel.RaisePlaySfx("terminal_beep");

            // Contextual dialogue or interaction reaction
            switch (locationId)
            {
                case StationLocationId.SolarFarm:
                    OutpostEventChannel.RaiseDialogueLinePrompted(new DialogueLine(
                        CharacterId.Marcus,
                        "Solar arrays aligned to the horizon sun! Photovoltaic efficiency is nominal at 50 kW."
                    ));
                    break;
                case StationLocationId.Greenhouse:
                    OutpostEventChannel.RaiseDialogueLinePrompted(new DialogueLine(
                        CharacterId.Elena,
                        "Hydroponic lettuce and dwarf wheat chambers look vibrant. Oxygen contribution confirmed."
                    ));
                    break;
                case StationLocationId.ResearchLaboratory:
                    OutpostEventChannel.RaiseDialogueLinePrompted(new DialogueLine(
                        CharacterId.Aisha,
                        "Mass spectrometer initialized! Spectrometry confirms high hydrogen signatures in the shadowed regolith."
                    ));
                    break;
                case StationLocationId.EquipmentStorage:
                    OutpostEventChannel.RaiseDialogueLinePrompted(new DialogueLine(
                        CharacterId.Daniel,
                        "Medical supplies cataloged. Lithium hydroxide spare canisters stored in the secondary rack."
                    ));
                    break;
                case StationLocationId.EmergencyBreachSite:
                    OutpostEventChannel.RaiseDialogueLinePrompted(new DialogueLine(
                        CharacterId.Orion,
                        "WARNING: High-velocity micrometeoroid impact detected here! High-voltage relay is damaged."
                    ));
                    break;
                default:
                    OutpostEventChannel.RaiseDialogueLinePrompted(new DialogueLine(
                        CharacterId.Orion,
                        $"{subsystemName} diagnostics verified. Telemetry synchronized with main outpost bus."
                    ));
                    break;
            }
        }
    }
}
