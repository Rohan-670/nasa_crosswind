using UnityEngine;
using Outpost.Core.Data;
using Outpost.Core.Events;

namespace Outpost.System1.Gameplay.Equipment
{
    public class CarbonDioxideScrubberEquipment : StationEquipmentBase
    {
        [Header("CO2 Scrubbing")]
        [SerializeField] private float nominalScrubRatePpm = 20.0f;
        [SerializeField] private bool hasLiOHCanister = true;
        [SerializeField] private bool hasEmergencyAdapterFitted = false;

        public bool HasCanister => hasLiOHCanister || hasEmergencyAdapterFitted;
        public float ScrubRatePpm => (isOperational && isPowered && HasCanister) ? (nominalScrubRatePpm * (conditionPercent / 100f)) : 0f;

        protected override void Awake()
        {
            locationId = StationLocationId.MainHabitat;
            equipmentId = "equipment_co2_scrubber";
            displayName = "LiOH Carbon Dioxide Scrubber";
            powerDrawKw = 3.5f;
            priority = PowerPriority.CriticalLifeSupport;
            base.Awake();
        }

        public void InstallCanister(bool installed)
        {
            hasLiOHCanister = installed;
            NotifyStateChanged();
        }

        public void FitEmergencyAdapter(bool fitted)
        {
            hasEmergencyAdapterFitted = fitted;
            if (fitted)
            {
                isOperational = true;
                conditionPercent = Mathf.Max(conditionPercent, 80f);
                SetWarningVisuals(false);
            }
            NotifyStateChanged();
        }

        protected override void ExecuteInteraction(GameObject interactor)
        {
            base.ExecuteInteraction(interactor);
            if (!HasCanister)
            {
                OutpostEventChannel.RaiseDialogueLinePrompted(new DialogueLine(
                    CharacterId.Daniel,
                    "The primary scrubber canister is saturated! Without a replacement LiOH cartridge or adapter, carbon dioxide will quickly reach toxic levels."
                ));
            }
            else
            {
                OutpostEventChannel.RaiseDialogueLinePrompted(new DialogueLine(
                    CharacterId.Marcus,
                    $"Airflow intake is pulling smoothly through the LiOH filters. CO2 scrubber is removing {ScrubRatePpm:F1} ppm per second."
                ));
            }
        }
    }
}
