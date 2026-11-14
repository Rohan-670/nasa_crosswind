using UnityEngine;
using Outpost.Core.Data;
using Outpost.Core.Events;

namespace Outpost.System1.Gameplay.Equipment
{
    public class WaterRecyclingEquipment : StationEquipmentBase
    {
        [Header("Water Recovery System (WRS)")]
        [SerializeField] private float nominalRecyclingRateLps = 0.11f;
        [SerializeField] [Range(0.5f, 0.99f)] private float recoveryEfficiency = 0.92f;
        [SerializeField] private float filterClogPercent = 0.0f;

        public float EffectiveRecyclingRateLps => (isOperational && isPowered) ? (nominalRecyclingRateLps * (conditionPercent / 100f) * (1f - (filterClogPercent / 100f))) : 0f;
        public float Efficiency => recoveryEfficiency;

        protected override void Awake()
        {
            locationId = StationLocationId.MainHabitat;
            equipmentId = "equipment_water_recycler";
            displayName = "Water Recovery Loop (WRS)";
            powerDrawKw = 4.2f;
            priority = PowerPriority.WaterAndGreenhouse;
            base.Awake();
        }

        public void CleanFilters()
        {
            filterClogPercent = 0f;
            conditionPercent = 100f;
            SetWarningVisuals(false);
            NotifyStateChanged();
        }

        protected override void ExecuteInteraction(GameObject interactor)
        {
            base.ExecuteInteraction(interactor);
            OutpostEventChannel.RaiseDialogueLinePrompted(new DialogueLine(
                CharacterId.Daniel,
                $"Water Recovery System running at {recoveryEfficiency * 100f:F0}% efficiency. Filter bed differential pressure is nominal."
            ));
        }
    }
}
