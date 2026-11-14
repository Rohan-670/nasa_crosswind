using System;
using System.Collections.Generic;
using UnityEngine;
using Outpost.Core.Data;

namespace Outpost.Core.Interfaces
{
    public interface IGeoMapService
    {
        IReadOnlyList<StationLocationData> AllLocations { get; }
        StationLocationData GetLocation(StationLocationId id);
        Vector3 PlayerWorldPosition { get; }
        StationLocationId ActiveTargetId { get; }

        event Action<StationLocationId> OnLocationDiscovered;
        event Action<StationLocationId> OnTargetLocationChanged;

        void MarkLocationDiscovered(StationLocationId id);
        void SetTargetLocation(StationLocationId id);
        Vector2 WorldToMapCoordinate(Vector3 worldPos, Vector2 mapRectSize);
    }
}
