using System;
using Outpost.UI.Core;

namespace Outpost.UI.Notifications
{
    /// <summary>
    /// Service contract for dispatching on-screen player notifications and toasts.
    /// </summary>
    public interface IMissionNotificationUI
    {
        void DisplayNotification(string title, string message, NotificationType type, float duration = 4.0f);
        void DisplayEmergencyAlert(string alertMessage);
    }
}
