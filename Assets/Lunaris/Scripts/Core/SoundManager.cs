using UnityEngine;

namespace Lunaris.Core
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        private AudioSource sfxSource;
        private AudioSource warningSource;
        private AudioSource engineSource;
        private AudioSource musicSource;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeAudioSources();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeAudioSources()
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            warningSource = gameObject.AddComponent<AudioSource>();
            warningSource.loop = true;

            engineSource = gameObject.AddComponent<AudioSource>();
            engineSource.loop = true;

            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
        }

        public void PlayButtonClick()
        {
            PlayProceduralTone(880f, 0.08f, 0.4f);
        }

        public void PlayWarningBeep()
        {
            PlayProceduralTone(1200f, 0.15f, 0.6f);
        }

        public void StartAlarmSiren(bool isCritical)
        {
            if (!warningSource.isPlaying)
            {
                float freq = isCritical ? 1400f : 950f;
                warningSource.clip = CreateToneClip("AlarmSiren", freq, 0.5f, true);
                warningSource.Play();
            }
        }

        public void StopAlarmSiren()
        {
            if (warningSource.isPlaying)
            {
                warningSource.Stop();
            }
        }

        public void PlayCheckpointSound()
        {
            PlayProceduralTone(523.25f, 0.1f, 0.5f); // C5
            Invoke(nameof(PlayCheckpointChordSecond), 0.1f);
        }

        private void PlayCheckpointChordSecond()
        {
            PlayProceduralTone(659.25f, 0.2f, 0.5f); // E5
        }

        public void PlaySampleCollected()
        {
            PlayProceduralTone(783.99f, 0.25f, 0.6f); // G5
        }

        public void PlayExplosion()
        {
            StopAlarmSiren();
            AudioClip boom = CreateNoiseClip("Explosion", 1.8f);
            sfxSource.PlayOneShot(boom, 0.9f);
        }

        public void SetEngineThrust(float power)
        {
            if (power > 0.05f)
            {
                if (!engineSource.isPlaying)
                {
                    engineSource.clip = CreateNoiseClip("EngineRumble", 1.0f);
                    engineSource.Play();
                }
                engineSource.volume = Mathf.Clamp01(power * 0.7f);
                engineSource.pitch = 0.8f + power * 0.5f;
            }
            else
            {
                if (engineSource.isPlaying) engineSource.Stop();
            }
        }

        private void PlayProceduralTone(float frequency, float duration, float volume)
        {
            AudioClip clip = CreateToneClip("Tone_" + frequency, frequency, duration, false);
            sfxSource.PlayOneShot(clip, volume);
        }

        private AudioClip CreateToneClip(string name, float frequency, float duration, bool loop)
        {
            int sampleRate = 44100;
            int samples = (int)(sampleRate * duration);
            float[] data = new float[samples];

            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / sampleRate;
                float envelope = loop ? 1f : Mathf.Clamp01(1f - (t / duration));
                data[i] = Mathf.Sin(2f * Mathf.PI * frequency * t) * envelope * 0.5f;
            }

            AudioClip clip = AudioClip.Create(name, samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private AudioClip CreateNoiseClip(string name, float duration)
        {
            int sampleRate = 44100;
            int samples = (int)(sampleRate * duration);
            float[] data = new float[samples];

            float last = 0f;
            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / sampleRate;
                float white = (Random.value * 2f - 1f);
                // Brown/pink filter
                last = (last + (0.05f * white)) / 1.05f;
                float decay = Mathf.Clamp01(1f - (t / duration));
                data[i] = last * decay * 0.8f;
            }

            AudioClip clip = AudioClip.Create(name, samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }
    }
}
