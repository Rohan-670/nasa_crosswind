using System;
using Outpost.UI.Core;

namespace Outpost.UI.ORION
{
    /// <summary>
    /// Contract for driving ORION's holographic animations, audio vocal chirps, and telemetry status.
    /// </summary>
    public interface IORIONInterface
    {
        AlertLevel CurrentAlertLevel { get; }
        void SetAlertLevel(AlertLevel level, string message = "");
        void TriggerHologramPulse();
        void SpeakNotification(string speechText);
        void OpenScienceExplanation(string conceptTitle, string explanation);
    }
}
