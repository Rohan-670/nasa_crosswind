using System;
using UnityEngine;
using Outpost.Core.Data;

namespace Outpost.Core.Interfaces
{
    /// <summary>
    /// Cross-System Contract for Equipment Prefabs (Computer 2 & System 1).
    /// Implemented by 3D station equipment prefabs (Solar panels, airlocks, scrubbers, comms dish, rover dock).
    /// Allows prefabs to communicate state, power requirements, and operational condition to the central gameplay system.
    /// </summary>
    public interface IStationEquipment
    {
        StationLocationId LocationId { get; }
        string EquipmentId { get; }
        string DisplayName { get; }
        Transform Transform { get; }
        bool IsOperational { get; }
        bool IsPowered { get; }
        bool HasVisualAlert { get; }
        float ConditionPercent { get; }
        float PowerDrawKw { get; }
        PowerPriority Priority { get; }

        void SetOperationalState(bool operational);
        void SetPoweredState(bool powered);
        void SetWarningVisuals(bool warningActive);
        void Repair(float conditionGain);
        void Damage(float conditionLoss);
        void TriggerInteractionFeedback();

        event Action<IStationEquipment> OnEquipmentStateChanged;
    }
}
