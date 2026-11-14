using UnityEngine;
using Outpost.Core.Data;
using Outpost.Core.Events;

namespace Outpost.System1.Gameplay.Equipment
{
    public class BatteryBankEquipment : StationEquipmentBase
    {
        [Header("Energy Storage")]
        [SerializeField] private float capacityKwh = 250.0f;
        [SerializeField] private float currentChargeKwh = 237.5f; // 95%
        [SerializeField] private float maxChargeDischargeRateKw = 40.0f;

        public float CapacityKwh => capacityKwh;
        public float CurrentChargeKwh => currentChargeKwh;
        public float ChargePercent => capacityKwh > 0f ? (currentChargeKwh / capacityKwh) * 100f : 0f;

        protected override void Awake()
        {
            locationId = StationLocationId.MainHabitat;
            equipmentId = "equipment_battery_bank";
            displayName = "Regenerative Fuel Cell Battery Bank";
            powerDrawKw = 0f;
            priority = PowerPriority.CriticalLifeSupport;
            base.Awake();
        }

        public float FlowPower(float netKw, float deltaSeconds)
        {
            // netKw > 0 means surplus (charge battery)
            // netKw < 0 means deficit (discharge battery)
            if (netKw > 0f)
            {
                float chargeKw = Mathf.Min(netKw, maxChargeDischargeRateKw);
                float energyAddedKwh = (chargeKw * deltaSeconds) / 3600f;
                currentChargeKwh = Mathf.Min(capacityKwh, currentChargeKwh + energyAddedKwh);
                return netKw - chargeKw;
            }
            else if (netKw < 0f)
            {
                float neededKw = Mathf.Min(-netKw, maxChargeDischargeRateKw);
                float energyNeededKwh = (neededKw * deltaSeconds) / 3600f;
                if (currentChargeKwh >= energyNeededKwh)
                {
                    currentChargeKwh -= energyNeededKwh;
                    return 0f; // Deficit fully covered by battery
                }
                else
                {
                    float energySupplied = currentChargeKwh;
                    currentChargeKwh = 0f;
                    float powerSuppliedKw = (energySupplied * 3600f) / deltaSeconds;
                    return netKw + powerSuppliedKw; // Remaining uncovered deficit
                }
            }

            return 0f;
        }

        public void SetCharge(float kwh)
        {
            currentChargeKwh = Mathf.Clamp(kwh, 0f, capacityKwh);
            NotifyStateChanged();
        }

        protected override void ExecuteInteraction(GameObject interactor)
        {
            base.ExecuteInteraction(interactor);
            OutpostEventChannel.RaiseDialogueLinePrompted(new DialogueLine(
                CharacterId.Marcus,
                $"Battery Bank charge reading: {ChargePercent:F1}% ({currentChargeKwh:F1} / {capacityKwh:F0} kWh). Cell health is nominal."
            ));
        }
    }
}
