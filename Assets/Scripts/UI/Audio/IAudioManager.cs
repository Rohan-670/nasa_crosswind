using System;
using UnityEngine;

namespace Outpost.UI.Audio
{
    /// <summary>
    /// Service contract for the centralized audio engine.
    /// Manages music cross-fading, 2D/3D SFX playback, and volume channels.
    /// </summary>
    public interface IAudioManager
    {
        void PlayMusic(AudioClip clip, float fadeDuration = 1.0f);
        void PlaySFX(AudioCue cue);
        void PlaySFXAtPosition(AudioCue cue, Vector3 position);
        void PlayUISound(AudioClip clip);
        void SetCategoryVolume(SoundCategory category, float volume);
    }
}
