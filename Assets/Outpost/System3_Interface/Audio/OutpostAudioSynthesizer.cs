using System.Collections.Generic;
using UnityEngine;
using Outpost.Core.Events;

namespace Outpost.System3.Interface.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class OutpostAudioSynthesizer : MonoBehaviour
    {
        private AudioSource audioSource;
        private Dictionary<string, AudioClip> synthesizedClips = new Dictionary<string, AudioClip>();

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f; // 2D UI / Comms audio

            GenerateClips();
        }

        private void OnEnable()
        {
            OutpostEventChannel.PlaySfx += PlaySound;
        }

        private void OnDisable()
        {
            OutpostEventChannel.PlaySfx -= PlaySound;
        }

        private void GenerateClips()
        {
            synthesizedClips["radio_chirp"] = CreateToneClip("radio_chirp", new float[] { 1760f, 2200f }, 0.08f);
            synthesizedClips["ui_click"] = CreateToneClip("ui_click", new float[] { 1200f }, 0.04f);
            synthesizedClips["terminal_beep"] = CreateToneClip("terminal_beep", new float[] { 880f }, 0.12f);
            synthesizedClips["waypoint_reached"] = CreateToneClip("waypoint_reached", new float[] { 523.25f, 659.25f }, 0.15f);
            synthesizedClips["lunar_hop"] = CreateToneClip("lunar_hop", new float[] { 220f, 330f }, 0.18f);
            synthesizedClips["alarm_siren"] = CreateToneClip("alarm_siren", new float[] { 440f, 880f, 440f, 880f }, 0.35f);
            synthesizedClips["info_chime"] = CreateToneClip("info_chime", new float[] { 523.25f, 659.25f, 783.99f }, 0.22f);
            synthesizedClips["victory_chime"] = CreateToneClip("victory_chime", new float[] { 523.25f, 659.25f, 783.99f, 1046.50f }, 0.30f);
            synthesizedClips["system_restored"] = CreateToneClip("system_restored", new float[] { 330f, 440f, 550f, 660f }, 0.25f);
            synthesizedClips["sample_scan"] = CreateToneClip("sample_scan", new float[] { 900f, 1100f, 1300f }, 0.15f);
            synthesizedClips["interact"] = CreateToneClip("interact", new float[] { 700f, 850f }, 0.08f);
        }

        private AudioClip CreateToneClip(string clipName, float[] frequencies, float totalDuration)
        {
            int sampleRate = 44100;
            int totalSamples = Mathf.CeilToInt(sampleRate * totalDuration);
            float[] samples = new float[totalSamples];

            int noteCount = frequencies.Length;
            int samplesPerNote = totalSamples / noteCount;

            for (int note = 0; note < noteCount; note++)
            {
                float freq = frequencies[note];
                int start = note * samplesPerNote;
                int end = (note == noteCount - 1) ? totalSamples : (note + 1) * samplesPerNote;

                for (int i = start; i < end; i++)
                {
                    float t = (float)(i - start) / samplesPerNote;
                    float envelope = Mathf.Sin(t * Mathf.PI); // Smooth attack/decay window
                    samples[i] = Mathf.Sin(2f * Mathf.PI * freq * ((float)i / sampleRate)) * envelope * 0.35f;
                }
            }

            AudioClip clip = AudioClip.Create(clipName, totalSamples, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        public void PlaySound(string cue)
        {
            if (synthesizedClips.TryGetValue(cue, out AudioClip clip))
            {
                audioSource.PlayOneShot(clip);
            }
            else if (synthesizedClips.TryGetValue("ui_click", out AudioClip fallback))
            {
                audioSource.PlayOneShot(fallback);
            }
        }
    }
}
