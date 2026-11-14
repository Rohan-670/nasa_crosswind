using System;

namespace Outpost.UI.Cinematics
{
    /// <summary>
    /// Service contract for letterboxing, camera fades, chapter title cards,
    /// and cinematic sequences with guaranteed skip functionality.
    /// </summary>
    public interface ICinematicPresentation
    {
        bool IsCinematicPlaying { get; }
        void PlayChapterIntro(string chapterTitle, string subtitle, Action onComplete = null);
        void PlayArrivalCinematic(Action onComplete = null);
        void PlayImpactCinematic(Action onComplete = null);
        void PlayRecoveryCinematic(Action onComplete = null);
        void SkipCurrentCinematic();
        void SetLetterbox(bool active, float duration = 0.5f);
        void FadeToBlack(float duration = 1.0f, Action onComplete = null);
        void FadeFromBlack(float duration = 1.0f, Action onComplete = null);
    }
}
