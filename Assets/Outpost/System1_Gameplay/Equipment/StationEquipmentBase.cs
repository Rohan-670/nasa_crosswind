using System;
using UnityEngine;
using Outpost.Core.Data;
using Outpost.Core.Interfaces;
using Outpost.Core.Events;
using Outpost.System1.Gameplay.Interactions;

namespace Outpost.System1.Gameplay.Equipment
{
    /// <summary>
    /// Reusable base component for all 3D station equipment prefabs.
    /// Implements both IStationEquipment (telemetry, power grid, state) and IInteractable (player inspection & repair).
    /// Registers with EquipmentManager automatically.
    /// </summary>
    public abstract class StationEquipmentBase : InteractableBase, IStationEquipment
    {
        [Header("Equipment Identity")]
        [SerializeField] protected StationLocationId locationId = StationLocationId.MainHabitat;
        [SerializeField] protected string equipmentId = "equipment_generic";
        [SerializeField] protected string displayName = "Station Equipment";

        [Header("Power & Status")]
        [SerializeField] protected float powerDrawKw = 4.0f;
        [SerializeField] protected PowerPriority priority = PowerPriority.CriticalLifeSupport;
        [SerializeField] [Range(0f, 100f)] protected float conditionPercent = 100.0f;
        [SerializeField] protected bool isOperational = true;
        [SerializeField] protected bool isPowered = true;
        [SerializeField] protected bool hasVisualAlert = false;

        [Header("Feedback Indicators")]
        [SerializeField] protected GameObject alertLight;
        [SerializeField] protected AudioClip stateChangeSound;

        // IStationEquipment Implementation
        public StationLocationId LocationId => locationId;
        public string EquipmentId => equipmentId;
        public string DisplayName => displayName;
        public Transform Transform => transform;
        public bool IsOperational => isOperational;
        public bool IsPowered => isPowered;
        public bool HasVisualAlert => hasVisualAlert;
        public float ConditionPercent => conditionPercent;
        public float PowerDrawKw => (isOperational && isPowered) ? powerDrawKw : 0f;
        public PowerPriority Priority => priority;

        public event Action<IStationEquipment> OnEquipmentStateChanged;

        // InteractableBase Overrides
        public override string PromptText => $"{displayName}: Press [E] to Inspect";
        public string ContextDescription => $"Status: {(isOperational ? "Operational" : "Offline")}, Condition: {conditionPercent:F0}%, Power: {(isPowered ? $"{powerDrawKw:F1} kW" : "No Power")}";

        protected override void Awake()
        {
            base.Awake();
            objectName = displayName;
            promptAction = "Inspect";
        }

        protected virtual void OnEnable()
        {
            EquipmentManager.Instance?.RegisterEquipment(this);
        }

        protected virtual void OnDisable()
        {
            EquipmentManager.Instance?.UnregisterEquipment(this);
        }

        public virtual void SetOperationalState(bool operational)
        {
            if (isOperational != operational)
            {
                isOperational = operational;
                NotifyStateChanged();
            }
        }

        public virtual void SetPoweredState(bool powered)
        {
            if (isPowered != powered)
            {
                isPowered = powered;
                NotifyStateChanged();
            }
        }

        public virtual void SetWarningVisuals(bool warningActive)
        {
            hasVisualAlert = warningActive;
            if (alertLight != null)
            {
                alertLight.SetActive(warningActive);
            }
        }

        public virtual void Repair(float conditionGain)
        {
            conditionPercent = Mathf.Clamp(conditionPercent + conditionGain, 0f, 100f);
            if (conditionPercent > 20f && !isOperational)
            {
                isOperational = true;
            }
            if (conditionPercent > 50f && hasVisualAlert)
            {
                SetWarningVisuals(false);
            }
            NotifyStateChanged();
            OutpostEventChannel.RaisePlaySfx("system_restored");
        }

        public virtual void Damage(float conditionLoss)
        {
            conditionPercent = Mathf.Clamp(conditionPercent - conditionLoss, 0f, 100f);
            if (conditionPercent <= 0f)
            {
                isOperational = false;
                SetWarningVisuals(true);
            }
            else if (conditionPercent < 35f)
            {
                SetWarningVisuals(true);
            }
            NotifyStateChanged();
            OutpostEventChannel.RaisePlaySfx("alarm_siren");
        }

        public virtual void TriggerInteractionFeedback()
        {
            if (!string.IsNullOrEmpty(audioCue))
            {
                OutpostEventChannel.RaisePlaySfx(audioCue);
            }
            if (stateChangeSound != null)
            {
                AudioSource.PlayClipAtPoint(stateChangeSound, transform.position);
            }
        }

        protected override void ExecuteInteraction(GameObject interactor)
        {
            TriggerInteractionFeedback();
            OutpostEventChannel.RaisePlaySfx("terminal_beep");
        }

        protected void NotifyStateChanged()
        {
            OnEquipmentStateChanged?.Invoke(this);
            EquipmentManager.Instance?.HandleEquipmentStateChanged(this);
            OutpostEventChannel.RaiseEquipmentStateChanged(this);
        }
    }
}
