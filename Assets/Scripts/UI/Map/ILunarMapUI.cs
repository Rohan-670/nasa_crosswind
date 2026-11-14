using System;
using UnityEngine;

namespace Outpost.UI.Map
{
    /// <summary>
    /// Service contract for interacting with the Lunar Geo Map.
    /// </summary>
    public interface ILunarMapUI
    {
        void OpenMap();
        void CloseMap();
        void FocusOnPlayer();
        void FocusOnLocation(string locationId);
        void SetWaypoint(string locationId);
        void MarkLocationDiscovered(string locationId);
    }
}
