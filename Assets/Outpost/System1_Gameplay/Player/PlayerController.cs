using UnityEngine;

namespace Outpost.System1.Gameplay.Player
{
    /// <summary>
    /// Base / Unified PlayerController for System 1.
    /// Inherits from ThirdPersonPlayerController to provide third-person locomotion,
    /// lunar gravity (g = -1.62 m/s²), walking, running, smooth rotation, and collision detection.
    /// </summary>
    public class PlayerController : ThirdPersonPlayerController
    {
    }
}
