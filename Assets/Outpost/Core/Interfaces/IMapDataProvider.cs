using System;
using System.Collections.Generic;
using UnityEngine;
using Outpost.Core.Data;

namespace Outpost.Core.Interfaces
{
    /// <summary>
    /// Cross-System Contract between System 1 (Gameplay) and System 3 (Interface Geo Map).
    /// Supplies live player world coordinates, active objective waypoint, and location discovery states.
    /// </summary>
    public interface IMapDataProvider
    {
        Vector3 PlayerWorldPosition { get; }
        Quaternion PlayerRotation { get; }
        StationLocationId ActiveWaypointId { get; }
        Vector3 ActiveWaypointWorldPosition { get; }
        string ActiveObjectiveDescription { get; }
        IReadOnlyList<StationLocationData> OutpostLocations { get; }

        event Action<Vector3> OnPlayerPositionUpdated;
        event Action<StationLocationId, Vector3, string> OnWaypointUpdated;
        event Action<StationLocationId> OnLocationDiscovered;

        bool IsLocationDiscovered(StationLocationId id);
        bool IsLocationAvailable(StationLocationId id);
    }
}
