using UnityEngine;
using Outpost.Core.Data;
using Outpost.Core.Events;

namespace Outpost.System1.Gameplay.Equipment
{
    public class OxygenGeneratorEquipment : StationEquipmentBase
    {
        [Header("O2 Production")]
        [SerializeField] private float nominalProductionLps = 0.20f;
        [SerializeField] private GeneratorStatus generatorStatus = GeneratorStatus.Nominal;
        [SerializeField] private float operatingTemperatureCelsius = 42.0f;

        public GeneratorStatus Status => isOperational && isPowered ? generatorStatus : GeneratorStatus.Offline;
        public float ProductionRateLps => (isOperational && isPowered && generatorStatus != GeneratorStatus.Offline) ? (nominalProductionLps * (conditionPercent / 100f)) : 0f;

        protected override void Awake()
        {
            locationId = StationLocationId.MainHabitat;
            equipmentId = "equipment_o2_generator";
            displayName = "Sabatier Oxygen Generator";
            powerDrawKw = 6.5f;
            priority = PowerPriority.CriticalLifeSupport;
            base.Awake();
        }

        public void SetGeneratorStatus(GeneratorStatus status)
        {
            generatorStatus = status;
            isOperational = (status != GeneratorStatus.Offline && status != GeneratorStatus.Fault);
            SetWarningVisuals(status == GeneratorStatus.Fault || status == GeneratorStatus.Overheating);
            NotifyStateChanged();
        }

        protected override void ExecuteInteraction(GameObject interactor)
        {
            base.ExecuteInteraction(interactor);
            string statusStr = Status.ToString();
            OutpostEventChannel.RaiseDialogueLinePrompted(new DialogueLine(
                CharacterId.Orion,
                $"Oxygen Generator status: {statusStr}. Electrolysis cell temperature: {operatingTemperatureCelsius:F1}°C. Current output: {ProductionRateLps:F2} L/s."
            ));
        }
    }
}
