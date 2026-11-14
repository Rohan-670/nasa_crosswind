using System;
using UnityEngine;

namespace Outpost.Core.Interfaces
{
    /// <summary>
    /// Cross-System Contract for Cinematic Triggers and camera sequences.
    /// Used during landing, micrometeoroid impact camera focus, and celebration sequences.
    /// </summary>
    public interface ICinematicTrigger
    {
        string CinematicId { get; }
        bool IsPlaying { get; }

        event Action<string> OnCinematicStarted;
        event Action<string> OnCinematicCompleted;

        void PlayCinematic(string cinematicId, Action onCompleted = null);
        void StopCinematic();
    }
}
