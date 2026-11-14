using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Outpost.UI.Core;

namespace Outpost.UI.Cinematics
{
    /// <summary>
    /// Master cinematic presentation system for System 3.
    /// Manages widescreen 2.39:1 letterboxing, chapter title cards, screen fades,
    /// dynamic narration subtitles, and guaranteed skip functionality (Space / Esc / Skip Button)
    /// for WebGL browser gameplay.
    /// </summary>
    public class CinematicPresentation : UIView, ICinematicPresentation
    {
        [Header("Letterbox Bars")]
        [SerializeField] private RectTransform _topBar;
        [SerializeField] private RectTransform _bottomBar;
        [SerializeField] private float _letterboxHeight = 100f;

        [Header("Title Splash Elements")]
        [SerializeField] private GameObject _titleCardPanel;
        [SerializeField] private TextMeshProUGUI _chapterNumberText;
        [SerializeField] private TextMeshProUGUI _chapterTitleText;

        [Header("Narration & Captions")]
        [SerializeField] private GameObject _narrationPanel;
        [SerializeField] private TextMeshProUGUI _narrationText;

        [Header("Skip Controls")]
        [SerializeField] private GameObject _skipPromptPanel;
        [SerializeField] private Button _skipButton;
        [SerializeField] private TextMeshProUGUI _skipButtonText;

        [Header("Screen Fader")]
        [SerializeField] private CameraFader _cameraFader;

        private Coroutine _letterboxCoroutine;
        private Coroutine _cinematicSequenceCoroutine;
        private Action _activeSequenceCallback;
        private bool _isCinematicPlaying = false;

        public bool IsCinematicPlaying => _isCinematicPlaying;

        protected override void Awake()
        {
            base.Awake();
            if (_titleCardPanel != null) _titleCardPanel.SetActive(false);
            if (_narrationPanel != null) _narrationPanel.SetActive(false);
            if (_skipPromptPanel != null) _skipPromptPanel.SetActive(false);
            if (_skipButton != null) _skipButton.onClick.AddListener(SkipCurrentCinematic);
            SetLetterboxInstant(0f);
        }

        private void Update()
        {
            if (!_isCinematicPlaying) return;

            // Allow young players to skip cinematics with Space or Escape keys
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape))
            {
                SkipCurrentCinematic();
            }
        }

        public void PlayChapterIntro(string chapterTitle, string subtitle, Action onComplete = null)
        {
            StartCinematicSequence(ChapterIntroSequence(chapterTitle, subtitle), onComplete);
        }

        public void PlayArrivalCinematic(Action onComplete = null)
        {
            StartCinematicSequence(ArrivalCinematicSequence(), onComplete);
        }

        public void PlayImpactCinematic(Action onComplete = null)
        {
            StartCinematicSequence(ImpactCinematicSequence(), onComplete);
        }

        public void PlayRecoveryCinematic(Action onComplete = null)
        {
            StartCinematicSequence(RecoveryCinematicSequence(), onComplete);
        }

        public void SkipCurrentCinematic()
        {
            if (!_isCinematicPlaying) return;

            if (_cinematicSequenceCoroutine != null)
            {
                StopCoroutine(_cinematicSequenceCoroutine);
                _cinematicSequenceCoroutine = null;
            }

            if (_letterboxCoroutine != null)
            {
                StopCoroutine(_letterboxCoroutine);
                _letterboxCoroutine = null;
            }

            // Immediately reset visual elements
            SetLetterboxInstant(0f);
            if (_titleCardPanel != null) _titleCardPanel.SetActive(false);
            if (_narrationPanel != null) _narrationPanel.SetActive(false);
            if (_skipPromptPanel != null) _skipPromptPanel.SetActive(false);

            _cameraFader?.Fade(0f, 0.2f);
            Hide();

            _isCinematicPlaying = false;
            UIEventBus.PublishUIStateChanged(UIState.GameplayHUD);

            var cb = _activeSequenceCallback;
            _activeSequenceCallback = null;
            cb?.Invoke();
        }

        public void SetLetterbox(bool active, float duration = 0.5f)
        {
            if (_letterboxCoroutine != null) StopCoroutine(_letterboxCoroutine);
            _letterboxCoroutine = StartCoroutine(LetterboxRoutine(active ? _letterboxHeight : 0f, duration));
        }

        public void FadeToBlack(float duration = 1.0f, Action onComplete = null)
        {
            _cameraFader?.Fade(1.0f, duration, onComplete);
        }

        public void FadeFromBlack(float duration = 1.0f, Action onComplete = null)
        {
            _cameraFader?.Fade(0.0f, duration, onComplete);
        }

        private void StartCinematicSequence(IEnumerator sequence, Action onComplete)
        {
            if (_cinematicSequenceCoroutine != null)
            {
                StopCoroutine(_cinematicSequenceCoroutine);
            }

            _isCinematicPlaying = true;
            _activeSequenceCallback = onComplete;

            Show();
            UIEventBus.PublishUIStateChanged(UIState.Cinematic);
            if (_skipPromptPanel != null) _skipPromptPanel.SetActive(true);
            if (_skipButtonText != null) _skipButtonText.text = "Skip [Space]";

            _cinematicSequenceCoroutine = StartCoroutine(RunCinematicWrapper(sequence));
        }

        private IEnumerator RunCinematicWrapper(IEnumerator sequence)
        {
            yield return StartCoroutine(sequence);

            if (_titleCardPanel != null) _titleCardPanel.SetActive(false);
            if (_narrationPanel != null) _narrationPanel.SetActive(false);
            if (_skipPromptPanel != null) _skipPromptPanel.SetActive(false);

            SetLetterbox(false, 0.5f);
            yield return new WaitForSecondsRealtime(0.5f);

            Hide();
            _isCinematicPlaying = false;
            UIEventBus.PublishUIStateChanged(UIState.GameplayHUD);

            var cb = _activeSequenceCallback;
            _activeSequenceCallback = null;
            cb?.Invoke();
        }

        private IEnumerator ChapterIntroSequence(string chapterTitle, string subtitle)
        {
            SetLetterbox(true, 0.6f);

            if (_titleCardPanel != null)
            {
                _titleCardPanel.SetActive(true);
                if (_chapterNumberText != null) _chapterNumberText.text = chapterTitle;
                if (_chapterTitleText != null) _chapterTitleText.text = subtitle;
            }

            yield return new WaitForSecondsRealtime(3.0f);
        }

        private IEnumerator ArrivalCinematicSequence()
        {
            SetLetterbox(true, 0.5f);
            FadeFromBlack(1.2f);

            DisplayNarration("Year 2045. Shackleton Crater Rim, Lunar South Pole.");
            yield return new WaitForSecondsRealtime(2.5f);

            DisplayNarration("NASA Artemis Outpost landing module touches down on ancient regolith.");
            yield return new WaitForSecondsRealtime(3.0f);

            DisplayTitle("OUTPOST: SURVIVE BEYOND EARTH", "CHAPTER 1: ARRIVAL ON THE MOON");
            yield return new WaitForSecondsRealtime(3.0f);
        }

        private IEnumerator ImpactCinematicSequence()
        {
            SetLetterbox(true, 0.3f);

            DisplayTitle("WARNING: HIGH-SPEED ANOMALY", "MICROMETEOROID SHOWER DETECTED");
            UIEventBus.PublishAlertLevel(AlertLevel.Warning, "Micrometeoroid swarm trajectory intersecting ridge power grid!");
            yield return new WaitForSecondsRealtime(2.0f);

            DisplayNarration("<color=#E74C3C><b>[ IMPACT DETECTED ]</b></color> Solar junction bus severed!");
            yield return new WaitForSecondsRealtime(2.5f);
        }

        private IEnumerator RecoveryCinematicSequence()
        {
            SetLetterbox(true, 0.5f);

            DisplayTitle("OUTPOST: SURVIVE BEYOND EARTH", "CHAPTER 6: OUTPOST RESTORATION");
            yield return new WaitForSecondsRealtime(2.0f);

            DisplayNarration("High-voltage power conduits re-energized. Solar array tracks the lunar horizon.");
            yield return new WaitForSecondsRealtime(2.5f);

            DisplayNarration("<color=#2ECC71>Oxygen generation and thermal control restored to 100% capacity.</color>");
            yield return new WaitForSecondsRealtime(2.5f);
        }

        private void DisplayTitle(string header, string subheader)
        {
            if (_titleCardPanel != null)
            {
                _titleCardPanel.SetActive(true);
                if (_chapterNumberText != null) _chapterNumberText.text = header;
                if (_chapterTitleText != null) _chapterTitleText.text = subheader;
            }
        }

        private void DisplayNarration(string text)
        {
            if (_narrationPanel != null) _narrationPanel.SetActive(true);
            if (_narrationText != null) _narrationText.text = text;
        }

        private void SetLetterboxInstant(float height)
        {
            if (_topBar != null) _topBar.sizeDelta = new Vector2(_topBar.sizeDelta.x, height);
            if (_bottomBar != null) _bottomBar.sizeDelta = new Vector2(_bottomBar.sizeDelta.x, height);
        }

        private IEnumerator LetterboxRoutine(float targetHeight, float duration)
        {
            float startHeight = _topBar != null ? _topBar.sizeDelta.y : 0f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float currentH = Mathf.Lerp(startHeight, targetHeight, elapsed / duration);
                SetLetterboxInstant(currentH);
                yield return null;
            }

            SetLetterboxInstant(targetHeight);
            _letterboxCoroutine = null;
        }
    }
}
