using UnityEngine;
using Outpost.Core.Data;

namespace Outpost.Core.Interfaces
{
    /// <summary>
    /// Cross-System Contract for Computer 2 (Environment & Equipment).
    /// Implemented by 3D station equipment prefabs (Solar panels, airlocks, scrubbers, comms dish, rover dock).
    /// </summary>
    public interface IStationEquipment
    {
        StationLocationId LocationId { get; }
        string EquipmentId { get; }
        Transform Transform { get; }
        bool IsOperational { get; }
        bool HasVisualAlert { get; }

        void SetOperationalState(bool operational);
        void SetWarningVisuals(bool warningActive);
        void TriggerInteractionFeedback();
    }
}
