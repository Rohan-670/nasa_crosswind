using System;
using UnityEngine;

namespace Outpost.UI.Audio
{
    public enum SoundCategory
    {
        Master = 0,
        Music = 1,
        SFX = 2,
        Voice = 3,
        UI = 4,
        Ambience = 5
    }

    /// <summary>
    /// ScriptableObject defining an audio playback asset with volume variance, pitch modulation, and loop settings.
    /// </summary>
    [CreateAssetMenu(fileName = "NewAudioCue", menuName = "Outpost/Audio/Audio Cue")]
    public class AudioCue : ScriptableObject
    {
        public string CueId = "SFX_Click";
        public SoundCategory Category = SoundCategory.SFX;
        public AudioClip[] AudioClips;
        [Range(0f, 1f)] public float BaseVolume = 1f;
        [Range(0.5f, 1.5f)] public float BasePitch = 1f;
        [Range(0f, 0.2f)] public float PitchVariance = 0.05f;
        public bool IsLooping = false;

        public AudioClip GetRandomClip()
        {
            if (AudioClips == null || AudioClips.Length == 0) return null;
            return AudioClips[UnityEngine.Random.Range(0, AudioClips.Length)];
        }
    }
}
