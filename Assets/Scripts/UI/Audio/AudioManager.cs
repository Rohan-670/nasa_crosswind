using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Outpost.UI.Audio
{
    /// <summary>
    /// Master Audio Manager for System 3.
    /// Manages 16-channel pooled AudioSource bank to prevent GC allocations and browser audio hiccups in WebGL.
    /// </summary>
    public class AudioManager : MonoBehaviour, IAudioManager
    {
        public static AudioManager Instance { get; private set; }

        [Header("Music Channels (Cross-Fading)")]
        [SerializeField] private AudioSource _musicSourceA;
        [SerializeField] private AudioSource _musicSourceB;

        [Header("Ambience & UI")]
        [SerializeField] private AudioSource _ambienceSource;
        [SerializeField] private AudioSource _uiSource;

        [Header("SFX Pool Configuration")]
        [SerializeField] private int _sfxPoolSize = 16;
        private readonly List<AudioSource> _sfxPool = new List<AudioSource>();
        private int _poolIndex = 0;

        private AudioSource _activeMusicSource;
        private Coroutine _musicFadeCoroutine;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeAudioSources();
        }

        private void InitializeAudioSources()
        {
            if (_musicSourceA == null) _musicSourceA = gameObject.AddComponent<AudioSource>();
            if (_musicSourceB == null) _musicSourceB = gameObject.AddComponent<AudioSource>();
            if (_ambienceSource == null) _ambienceSource = gameObject.AddComponent<AudioSource>();
            if (_uiSource == null) _uiSource = gameObject.AddComponent<AudioSource>();

            _musicSourceA.loop = true;
            _musicSourceB.loop = true;
            _ambienceSource.loop = true;

            _activeMusicSource = _musicSourceA;

            // Pre-allocate 16-channel pooled bank for SFX
            for (int i = 0; i < _sfxPoolSize; i++)
            {
                var sfxSrc = gameObject.AddComponent<AudioSource>();
                sfxSrc.playOnAwake = false;
                _sfxPool.Add(sfxSrc);
            }
        }

        public void PlayMusic(AudioClip clip, float fadeDuration = 1.0f)
        {
            if (clip == null) return;
            if (_musicFadeCoroutine != null) StopCoroutine(_musicFadeCoroutine);
            _musicFadeCoroutine = StartCoroutine(CrossfadeMusicRoutine(clip, fadeDuration));
        }

        public void PlaySFX(AudioCue cue)
        {
            if (cue == null) return;
            AudioClip clip = cue.GetRandomClip();
            if (clip == null) return;

            var src = GetAvailableSFXSource();
            src.spatialBlend = 0f; // 2D
            float pitch = cue.BasePitch + UnityEngine.Random.Range(-cue.PitchVariance, cue.PitchVariance);
            src.pitch = pitch;
            src.volume = cue.BaseVolume;
            src.PlayOneShot(clip);
        }

        public void PlaySFXAtPosition(AudioCue cue, Vector3 position)
        {
            if (cue == null) return;
            AudioClip clip = cue.GetRandomClip();
            if (clip == null) return;

            var src = GetAvailableSFXSource();
            src.transform.position = position;
            src.spatialBlend = 1f; // 3D
            src.minDistance = 3f;
            src.maxDistance = 60f;
            src.pitch = cue.BasePitch + UnityEngine.Random.Range(-cue.PitchVariance, cue.PitchVariance);
            src.volume = cue.BaseVolume;
            src.PlayOneShot(clip);
        }

        public void PlayUISound(AudioClip clip)
        {
            if (clip != null && _uiSource != null)
            {
                _uiSource.PlayOneShot(clip);
            }
        }

        public void SetCategoryVolume(SoundCategory category, float volume)
        {
            volume = Mathf.Clamp01(volume);
            switch (category)
            {
                case SoundCategory.Master:
                    AudioListener.volume = volume;
                    break;
                case SoundCategory.Music:
                    if (_activeMusicSource != null) _activeMusicSource.volume = volume;
                    break;
                case SoundCategory.UI:
                    if (_uiSource != null) _uiSource.volume = volume;
                    break;
                case SoundCategory.Ambience:
                    if (_ambienceSource != null) _ambienceSource.volume = volume;
                    break;
            }
        }

        private AudioSource GetAvailableSFXSource()
        {
            var src = _sfxPool[_poolIndex];
            _poolIndex = (_poolIndex + 1) % _sfxPool.Count;
            return src;
        }

        private IEnumerator CrossfadeMusicRoutine(AudioClip newClip, float duration)
        {
            AudioSource fadingOut = _activeMusicSource;
            AudioSource fadingIn = (fadingOut == _musicSourceA) ? _musicSourceB : _musicSourceA;

            fadingIn.clip = newClip;
            fadingIn.volume = 0f;
            fadingIn.Play();

            float elapsed = 0f;
            float initialVol = fadingOut.volume;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / duration;
                fadingIn.volume = Mathf.Lerp(0f, 0.8f, t);
                fadingOut.volume = Mathf.Lerp(initialVol, 0f, t);
                yield return null;
            }

            fadingOut.Stop();
            fadingIn.volume = 0.8f;
            _activeMusicSource = fadingIn;
            _musicFadeCoroutine = null;
        }
    }
}
