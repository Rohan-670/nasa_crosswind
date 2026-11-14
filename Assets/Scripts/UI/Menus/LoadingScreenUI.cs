using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Outpost.UI.Core;

namespace Outpost.UI.Menus
{
    /// <summary>
    /// Smooth loading screen for Mission 1 transitions with educational science tips.
    /// Optimized for Unity WebGL browser load times.
    /// </summary>
    public class LoadingScreenUI : UIView
    {
        [Header("UI Elements")]
        [SerializeField] private Slider _progressBar;
        [SerializeField] private TextMeshProUGUI _progressPercentText;
        [SerializeField] private TextMeshProUGUI _loadingStatusText;
        [SerializeField] private TextMeshProUGUI _educationalTipText;

        [Header("Lunar Science Tips (Ages 7–12)")]
        [SerializeField] private List<string> _lunarTips = new List<string>
        {
            "Did you know? The Moon has only one-sixth of Earth's gravity. A 30 kg child would weigh only 5 kg on the Moon!",
            "Space Fact: The Lunar South Pole has peaks of eternal light where solar panels can generate electricity almost continuously.",
            "Life Support Tip: Astronauts make breathable oxygen on the Moon by splitting water molecules with electricity, a process called electrolysis!",
            "Regolith Alert: Lunar dust particles are as sharp as tiny shards of broken glass because there is no wind or rain to erode them.",
            "Communication Note: Radio waves traveling between the Moon and Earth take approximately 1.3 seconds at the speed of light.",
            "Engineering Fact: In April 1970, Apollo 13 astronauts used gray duct tape and flight manual covers to fix their life-support filters in an emergency!"
        };

        private Coroutine _loadingCoroutine;

        protected override void Awake()
        {
            base.Awake();
            PickRandomTip();
        }

        public void PickRandomTip()
        {
            if (_educationalTipText != null && _lunarTips.Count > 0)
            {
                int idx = UnityEngine.Random.Range(0, _lunarTips.Count);
                _educationalTipText.text = $"<b>Lunar Science Fact:</b>\n{_lunarTips[idx]}";
            }
        }

        public void SetProgress(float progress, string status = "")
        {
            progress = Mathf.Clamp01(progress);
            if (_progressBar != null) _progressBar.value = progress;
            if (_progressPercentText != null) _progressPercentText.text = $"{Mathf.RoundToInt(progress * 100f)}%";
            if (!string.IsNullOrEmpty(status) && _loadingStatusText != null) _loadingStatusText.text = status;
        }

        public void SimulateLoading(float duration, Action onComplete = null)
        {
            Show();
            PickRandomTip();
            if (_loadingCoroutine != null) StopCoroutine(_loadingCoroutine);
            _loadingCoroutine = StartCoroutine(SimulateRoutine(duration, onComplete));
        }

        private IEnumerator SimulateRoutine(float duration, Action onComplete)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float p = Mathf.Clamp01(elapsed / duration);
                string status = p switch
                {
                    < 0.3f => "Establishing Lunar Telemetry...",
                    < 0.6f => "Calibrating Life-Support Scrubbers...",
                    < 0.85f => "Powering ORION AI Network...",
                    _ => "Approaching Shackleton Crater Rim..."
                };
                SetProgress(p, status);
                yield return null;
            }

            SetProgress(1f, "Mission 1 Ready!");
            yield return new WaitForSecondsRealtime(0.4f);

            Hide(false, onComplete);
            _loadingCoroutine = null;
        }
    }
}
