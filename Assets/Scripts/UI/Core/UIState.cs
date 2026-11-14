using System;

namespace Outpost.UI.Core
{
    /// <summary>
    /// Represents the high-level states of the user interface.
    /// </summary>
    public enum UIState
    {
        None = 0,
        MainMenu = 1,
        GameplayHUD = 2,
        Dialogue = 3,
        InteractiveMap = 4,
        PauseMenu = 5,
        Settings = 6,
        Cinematic = 7
    }

    /// <summary>
    /// Visual alert levels for ORION and station emergency systems.
    /// </summary>
    public enum AlertLevel
    {
        Nominal = 0,    // Green
        Caution = 1,    // Yellow / Amber
        Emergency = 2   // Pulsing Red
    }

    /// <summary>
    /// Classification for in-game notification toasts.
    /// </summary>
    public enum NotificationType
    {
        Info = 0,
        ObjectiveUpdate = 1,
        Discovery = 2,
        EmergencyAlert = 3,
        RadioComms = 4
    }

    /// <summary>
    /// Emergency repair branch chosen by the player in Chapter 6.
    /// </summary>
    public enum EmergencyRepairOption
    {
        None = 0,
        AstronautEVA = 1,        // Elena's choice: Manual spacewalk
        RoboticMaintenance = 2,  // Marcus & Aisha's choice: Remote rover
        PowerConservation = 3    // Daniel's choice: Reroute & conserve
    }
}
