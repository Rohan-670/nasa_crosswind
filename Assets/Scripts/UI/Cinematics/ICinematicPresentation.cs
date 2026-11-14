using System;

namespace Outpost.UI.Cinematics
{
    /// <summary>
    /// Service contract for letterboxing, camera fades, and chapter title presentations.
    /// </summary>
    public interface ICinematicPresentation
    {
        void PlayChapterIntro(string chapterTitle, string subtitle, Action onComplete = null);
        void SetLetterbox(bool active, float duration = 0.5f);
        void FadeToBlack(float duration = 1.0f, Action onComplete = null);
        void FadeFromBlack(float duration = 1.0f, Action onComplete = null);
    }
}
