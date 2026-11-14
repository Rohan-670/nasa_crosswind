using UnityEngine;
using Outpost.Core.Data;
using Outpost.Core.Events;

namespace Outpost.System1.Gameplay.Equipment
{
    public class GreenhouseHydroponicsEquipment : StationEquipmentBase
    {
        [Header("Hydroponic Crop Beds")]
        [SerializeField] private float waterDrawLps = 0.04f;
        [SerializeField] private float nominalFoodYieldRate = 0.05f;   // rations/sec
        [SerializeField] private float o2YieldLps = 0.04f;              // L/s produced
        [SerializeField] [Range(0f, 100f)] private float cropHealthPercent = 88.0f;

        public float WaterConsumptionLps => (isOperational && isPowered) ? waterDrawLps : 0f;
        public float FoodProductionRate => (isOperational && isPowered && cropHealthPercent > 20f) ? (nominalFoodYieldRate * (cropHealthPercent / 100f)) : 0f;
        public float OxygenProductionLps => (isOperational && isPowered && cropHealthPercent > 20f) ? (o2YieldLps * (cropHealthPercent / 100f)) : 0f;
        public float CropHealth => cropHealthPercent;

        protected override void Awake()
        {
            locationId = StationLocationId.Greenhouse;
            equipmentId = "equipment_greenhouse_hydroponics";
            displayName = "Hydroponic Greenhouse Array";
            powerDrawKw = 5.0f;
            priority = PowerPriority.WaterAndGreenhouse;
            base.Awake();
        }

        public void WaterCrops(bool hasWater)
        {
            if (hasWater)
            {
                cropHealthPercent = Mathf.Min(100f, cropHealthPercent + 0.1f);
            }
            else
            {
                cropHealthPercent = Mathf.Max(0f, cropHealthPercent - 0.25f);
                if (cropHealthPercent < 30f)
                {
                    SetWarningVisuals(true);
                }
            }
            NotifyStateChanged();
        }

        protected override void ExecuteInteraction(GameObject interactor)
        {
            base.ExecuteInteraction(interactor);
            OutpostEventChannel.RaiseDialogueLinePrompted(new DialogueLine(
                CharacterId.Aisha,
                $"Dwarf wheat and microgreen trays are flourishing at {cropHealthPercent:F0}% vitality. Photosynthesis rate: +{OxygenProductionLps:F2} L/s O2."
            ));
        }
    }
}
