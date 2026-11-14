using System;
using System.Collections.Generic;
using UnityEngine;
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

    public class EquipmentManager : MonoBehaviour
    {
        [Header("Equipment Inventory")]
        [SerializeField] private List<EquipmentItem> inventory = new List<EquipmentItem>();

        public event Action<EquipmentType> OnEquipmentAcquired;
        public event Action<EquipmentType> OnEquipmentConsumed;

        public static EquipmentManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            InitializeDefaultEquipment();
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

        public EquipmentItem GetItem(EquipmentType type)
        {
            return inventory.Find(i => i.Type == type);
        }

        public IReadOnlyList<EquipmentItem> GetAllItems()
        {
            return inventory;
        }
    }
}
