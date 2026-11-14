using System;
using System.Collections.Generic;
using UnityEngine;
using Outpost.Core.Data;
using Outpost.Core.Interfaces;
using Outpost.Core.Events;

namespace Outpost.System1.Gameplay.Equipment
{
    public enum EquipmentType
    {
        LiOHCanister,           // Square lithium hydroxide scrubber canister
        DuctTape,               // Heavy-duty grey space adhesive
        SuitHose,               // Flexible suit breathing tube
        CardboardManual,        // Stiff flight manual cover
        SolarCableSplicer,      // High-voltage electrical junction tool
        RoverRemoteTerminal,    // Wireless robotic control unit
        GeologicalCoreDrill     // Sub-surface ice-core drill
    }

    [Serializable]
    public class EquipmentItem
    {
        public EquipmentType Type;
        public string ItemName;
        public string Description;
        public bool IsAcquired;
        public bool IsEquipped;

        public EquipmentItem(EquipmentType type, string name, string description, bool acquired = false)
        {
            Type = type;
            ItemName = name;
            Description = description;
            IsAcquired = acquired;
            IsEquipped = false;
        }
    }

    /// <summary>
    /// Central manager for both station equipment prefabs (IStationEquipment registry)
    /// and portable astronaut tools/spare parts inventory.
    /// </summary>
    public class EquipmentManager : MonoBehaviour
    {
        [Header("Equipment Inventory")]
        [SerializeField] private List<EquipmentItem> inventory = new List<EquipmentItem>();

        [Header("Registered Station Equipment")]
        [SerializeField] private List<IStationEquipment> registeredEquipment = new List<IStationEquipment>();

        public event Action<EquipmentType> OnEquipmentAcquired;
        public event Action<EquipmentType> OnEquipmentConsumed;
        public event Action<IStationEquipment> OnEquipmentRegistered;
        public event Action<IStationEquipment> OnEquipmentUnregistered;
        public event Action<IStationEquipment> OnEquipmentStateUpdated;

        public static EquipmentManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else
            {
                if (Application.isPlaying) Destroy(gameObject);
                else DestroyImmediate(gameObject);
                return;
            }

            InitializeDefaultEquipment();
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        private void InitializeDefaultEquipment()
        {
            inventory.Clear();
            inventory.Add(new EquipmentItem(EquipmentType.LiOHCanister, "Square LiOH Canister", "Command module replacement carbon dioxide absorption filter."));
            inventory.Add(new EquipmentItem(EquipmentType.DuctTape, "Heavy-Duty Space Tape", "Hermetic grey adhesive tape for emergency sealing."));
            inventory.Add(new EquipmentItem(EquipmentType.SuitHose, "Suit Umbilical Hose", "Flexible air hose from EVA suits for airflow channeling."));
            inventory.Add(new EquipmentItem(EquipmentType.CardboardManual, "Flight Manual Cover", "Stiff cardboard backing used as a geometric adapter funnel."));
            inventory.Add(new EquipmentItem(EquipmentType.SolarCableSplicer, "High-Voltage Cable Splicer", "Marcus's electrical tool for external solar array repairs."));
            inventory.Add(new EquipmentItem(EquipmentType.RoverRemoteTerminal, "Rover Wireless Terminal", "Remote interface to control the rover's robotic manipulator arm."));
            inventory.Add(new EquipmentItem(EquipmentType.GeologicalCoreDrill, "Cryogenic Core Drill", "Aisha's sampling drill for extracting prehistoric ice volatiles."));
        }

        #region Portable Inventory & Spares

        public bool HasEquipment(EquipmentType type)
        {
            EquipmentItem item = inventory.Find(i => i.Type == type);
            return item != null && item.IsAcquired;
        }

        public void AcquireEquipment(EquipmentType type)
        {
            EquipmentItem item = inventory.Find(i => i.Type == type);
            if (item != null && !item.IsAcquired)
            {
                item.IsAcquired = true;
                OnEquipmentAcquired?.Invoke(type);
                OutpostEventChannel.RaisePlaySfx("ui_click");
            }
        }

        public void ConsumeEquipment(EquipmentType type)
        {
            EquipmentItem item = inventory.Find(i => i.Type == type);
            if (item != null && item.IsAcquired)
            {
                item.IsAcquired = false;
                item.IsEquipped = false;
                OnEquipmentConsumed?.Invoke(type);
            }
        }

        public void EquipItem(EquipmentType type)
        {
            foreach (var item in inventory)
            {
                item.IsEquipped = (item.Type == type);
            }
        }

        public EquipmentItem GetItem(EquipmentType type) => inventory.Find(i => i.Type == type);
        public IReadOnlyList<EquipmentItem> GetAllItems() => inventory;

        #endregion

        #region Station Equipment Registry

        public void RegisterEquipment(IStationEquipment equipment)
        {
            if (equipment == null || registeredEquipment.Contains(equipment)) return;

            registeredEquipment.Add(equipment);
            OnEquipmentRegistered?.Invoke(equipment);
            OutpostEventChannel.RaiseEquipmentStateChanged(equipment);
        }

        public void UnregisterEquipment(IStationEquipment equipment)
        {
            if (equipment == null || !registeredEquipment.Contains(equipment)) return;

            registeredEquipment.Remove(equipment);
            OnEquipmentUnregistered?.Invoke(equipment);
        }

        public void HandleEquipmentStateChanged(IStationEquipment equipment)
        {
            OnEquipmentStateUpdated?.Invoke(equipment);
        }

        public float GetTotalActivePowerDrawKw()
        {
            float total = 0f;
            for (int i = 0; i < registeredEquipment.Count; i++)
            {
                var eq = registeredEquipment[i];
                if (eq != null && eq.IsOperational && eq.IsPowered)
                {
                    total += eq.PowerDrawKw;
                }
            }
            return total;
        }

        public float GetAverageEquipmentCondition()
        {
            if (registeredEquipment.Count == 0) return 100f;

            float sum = 0f;
            int count = 0;
            for (int i = 0; i < registeredEquipment.Count; i++)
            {
                if (registeredEquipment[i] != null)
                {
                    sum += registeredEquipment[i].ConditionPercent;
                    count++;
                }
            }
            return count > 0 ? (sum / count) : 100f;
        }

        public IStationEquipment GetEquipmentById(string id)
        {
            return registeredEquipment.Find(e => e != null && e.EquipmentId == id);
        }

        public List<IStationEquipment> GetEquipmentByLocation(StationLocationId loc)
        {
            List<IStationEquipment> list = new List<IStationEquipment>();
            for (int i = 0; i < registeredEquipment.Count; i++)
            {
                if (registeredEquipment[i] != null && registeredEquipment[i].LocationId == loc)
                {
                    list.Add(registeredEquipment[i]);
                }
            }
            return list;
        }

        public void ApplyLoadShedding(PowerPriority highestShedPriority)
        {
            // Priority ordering: 0 is highest (CriticalLifeSupport), 4 is lowest (ScienceAndAuxiliary).
            // When load shedding, equipment with priority >= highestShedPriority are turned off.
            for (int i = 0; i < registeredEquipment.Count; i++)
            {
                var eq = registeredEquipment[i];
                if (eq == null) continue;

                if (eq.Priority >= highestShedPriority)
                {
                    eq.SetPoweredState(false);
                }
                else
                {
                    eq.SetPoweredState(true);
                }
            }
        }

        public void RestoreAllEquipmentPower()
        {
            for (int i = 0; i < registeredEquipment.Count; i++)
            {
                if (registeredEquipment[i] != null)
                {
                    registeredEquipment[i].SetPoweredState(true);
                }
            }
        }

        public IReadOnlyList<IStationEquipment> GetAllEquipment() => registeredEquipment;

        #endregion
    }
}
