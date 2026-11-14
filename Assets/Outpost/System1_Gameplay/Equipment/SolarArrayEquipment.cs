using UnityEngine;
using Outpost.Core.Data;
using Outpost.Core.Events;

namespace Outpost.System1.Gameplay.Equipment
{
    public class SolarArrayEquipment : StationEquipmentBase
    {
        [Header("Solar Generation")]
        [SerializeField] private float nominalOutputKw = 55.0f;
        [SerializeField] private float sunIntensityFactor = 1.0f;
        [SerializeField] private bool isTrackingSun = true;

        public float CurrentGenerationKw => (isOperational && isPowered) ? (nominalOutputKw * (conditionPercent / 100f) * (isTrackingSun ? sunIntensityFactor : sunIntensityFactor * 0.4f)) : 0f;
        public bool IsTrackingSun => isTrackingSun;

        protected override void Awake()
        {
            locationId = StationLocationId.SolarFarm;
            equipmentId = "equipment_solar_array";
            displayName = "Photovoltaic Solar Array";
            powerDrawKw = 0f; // It generates power, does not consume
            priority = PowerPriority.CriticalLifeSupport;
            base.Awake();
        }

        public void SetSunIntensity(float intensity)
        {
            sunIntensityFactor = Mathf.Clamp01(intensity);
            NotifyStateChanged();
        }

        public void SimulateMicrometeoroidDamage()
        {
            Damage(85.0f);
            isOperational = false;
            SetWarningVisuals(true);
            OutpostEventChannel.RaisePlaySfx("alarm_siren");
        }

        protected override void ExecuteInteraction(GameObject interactor)
        {
            base.ExecuteInteraction(interactor);
            if (!isOperational)
            {
                OutpostEventChannel.RaiseDialogueLinePrompted(new DialogueLine(
                    CharacterId.Marcus,
                    "Solar panel cells are shattered and the inverter circuit is severed! We need to repair the relay with spare parts."
                ));
            }
            else
            {
                OutpostEventChannel.RaiseDialogueLinePrompted(new DialogueLine(
                    CharacterId.Marcus,
                    $"Solar array is tracking the horizon sun at peak efficiency. Output is reading {CurrentGenerationKw:F1} kW."
                ));
            }
        }
    }
}
