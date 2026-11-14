using UnityEngine;
using Outpost.Core.Data;

namespace Outpost.Core.Interfaces
{
    /// <summary>
    /// Cross-System Contract for Computer 2 (Environment & Characters).
    /// Implemented by 3D crew character prefabs (Elena, Marcus, Aisha, Daniel, ORION).
    /// </summary>
    public interface ICrewCharacter
    {
        CharacterId CharacterId { get; }
        Transform Transform { get; }
        Vector3 HeadPosition { get; }
        bool IsInteracting { get; set; }

        void PlayAnimation(string animationTrigger);
        void SetBadgeHighlight(bool highlighted);
        void LookAtTarget(Vector3 targetPosition);
    }
}
