using System;

namespace Outpost.UI.HUD
{
    /// <summary>
    /// Interface for displaying life-support and station resource telemetry.
    /// Used by System 1 (Gameplay / ResourceManager).
    /// </summary>
    public interface IResourceDisplay
    {
        void SetOxygen(float current, float max, float deltaRate);
        void SetPower(float current, float max, float deltaRate);
        void SetWater(float current, float max, float deltaRate);
        void SetCarbonDioxide(float currentPpm, bool isScrubberActive);
    }
}
