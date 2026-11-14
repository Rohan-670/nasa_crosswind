using UnityEngine;
using Outpost.Core.Data;
using Outpost.Core.Events;

namespace Outpost.System1.Gameplay.Equipment
{
    public class RadiationShieldEquipment : StationEquipmentBase
    {
        [Header("Radiation Protection")]
        [SerializeField] private float baseShieldingPercent = 95.0f;
        [SerializeField] private bool activeDeflectionFieldEnabled = true;

        public float EffectiveShieldingPercent => (isOperational && isPowered && activeDeflectionFieldEnabled) ? baseShieldingPercent : (baseShieldingPercent * 0.45f);

        protected override void Awake()
        {
            locationId = StationLocationId.MainHabitat;
            equipmentId = "equipment_radiation_shield";
            displayName = "Electromagnetic Radiation Shield";
            powerDrawKw = 2.8f;
            priority = PowerPriority.AtmosphereAndThermal;
            base.Awake();
        }

        public void SetDeflectionField(bool enabled)
        {
            activeDeflectionFieldEnabled = enabled;
            NotifyStateChanged();
        }

        protected override void ExecuteInteraction(GameObject interactor)
        {
            base.ExecuteInteraction(interactor);
            OutpostEventChannel.RaiseDialogueLinePrompted(new DialogueLine(
                CharacterId.Daniel,
                $"Habitat radiation dosimeter is baseline. Magnetic deflection coils and lunar regolith shielding attenuate 95% of cosmic flux."
            ));
        }
    }
}
