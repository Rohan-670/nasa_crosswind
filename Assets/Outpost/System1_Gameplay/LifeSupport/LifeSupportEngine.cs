using System;
using UnityEngine;
using Outpost.Core.Interfaces;
using Outpost.System1.Gameplay.Resources;

namespace Outpost.System1.Gameplay.LifeSupport
{
    /// <summary>
    /// Legacy compatibility bridge for LifeSupportEngine.
    /// Delegates to the centralized ResourceManager.
    /// </summary>
    public class LifeSupportEngine : MonoBehaviour, ILifeSupportSystem
    {
        public static LifeSupportEngine Instance { get; private set; }

        public float OxygenLevel => ResourceManager.Instance != null ? ResourceManager.Instance.OxygenLevel : 100f;
        public float CarbonDioxideLevel => ResourceManager.Instance != null ? ResourceManager.Instance.CarbonDioxideLevel : 400f;
        public float PowerGenerationKW => ResourceManager.Instance != null ? ResourceManager.Instance.PowerGenerationKW : 50f;
        public float PowerConsumptionKW => ResourceManager.Instance != null ? ResourceManager.Instance.PowerConsumptionKW : 30f;
        public float BatteryChargePercent => ResourceManager.Instance != null ? ResourceManager.Instance.BatteryChargePercent : 100f;
        public float WaterReserveLiters => ResourceManager.Instance != null ? ResourceManager.Instance.WaterReserveLiters : 800f;
        public float GreenhouseBiomass => ResourceManager.Instance != null ? ResourceManager.Instance.GreenhouseBiomass : 75f;
        public bool IsEmergencyActive => ResourceManager.Instance != null && ResourceManager.Instance.IsEmergencyActive;
        public string EmergencyStatusMessage => ResourceManager.Instance != null ? ResourceManager.Instance.EmergencyStatusMessage : string.Empty;

        public event Action<float, float> OnResourceLevelsChanged;
        public event Action<bool, string> OnEmergencyStateChanged;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void SetSubsystemActive(string subsystemName, bool active) => ResourceManager.Instance?.SetSubsystemActive(subsystemName, active);
        public void TriggerMicrometeoroidDamage() => ResourceManager.Instance?.TriggerMicrometeoroidDamage();
        public void RepairFiltrationCanister() => ResourceManager.Instance?.RepairFiltrationCanister();
        public void RestoreFullPower() => ResourceManager.Instance?.RestoreFullPower();
    }
}
