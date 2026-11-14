using System;
using UnityEngine;

namespace Outpost.UI.Core
{
    /// <summary>
    /// Master UI manager interface governing UI screen states and modal overlays.
    /// </summary>
    public interface IUIManager
    {
        UIState CurrentState { get; }
        void ChangeState(UIState newState);
        void ToggleMap();
        void TogglePause();
        void ShowNotification(string title, string message, NotificationType type);
        void SetAlertLevel(AlertLevel level, string reason);
    }
}
