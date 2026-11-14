using UnityEngine;

namespace Outpost.System1.Gameplay.Player
{
    /// <summary>
    /// Base / Unified CameraController for System 1.
    /// Inherits from ThirdPersonCameraController to provide orbit camera following,
    /// mouse rotation, spring arm collision avoidance, and cinematic focus.
    /// </summary>
    public class CameraController : ThirdPersonCameraController
    {
        private Transform cinematicTarget;
        private bool isCinematicActive = false;

        public void SetLock(bool locked)
        {
            SetControlLock(locked);
        }

        public void SetCinematicTarget(Transform target)
        {
            cinematicTarget = target;
            isCinematicActive = target != null;
            if (isCinematicActive)
            {
                SetTarget(target);
            }
        }
    }
}
