using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Outpost.UI.Core;

namespace Outpost.UI.Cinematics
{
    /// <summary>
    /// Master cinematic presentation system.
    /// Manages widescreen 2.39:1 letterboxing, chapter title cards, and smooth screen fades.
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

        [Header("Screen Fader")]
        [SerializeField] private CameraFader _cameraFader;

        private Coroutine _letterboxCoroutine;

        protected override void Awake()
        {
            base.Awake();
            if (_titleCardPanel != null) _titleCardPanel.SetActive(false);
            SetLetterboxInstant(0f);
        }

        public void PlayChapterIntro(string chapterTitle, string subtitle, Action onComplete = null)
        {
            StartCoroutine(ChapterIntroSequence(chapterTitle, subtitle, onComplete));
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

        private IEnumerator ChapterIntroSequence(string chapterTitle, string subtitle, Action onComplete)
        {
            Show();
            UIEventBus.PublishUIStateChanged(UIState.Cinematic);

            SetLetterbox(true, 0.6f);

            if (_titleCardPanel != null)
            {
                _titleCardPanel.SetActive(true);
                if (_chapterNumberText != null) _chapterNumberText.text = chapterTitle;
                if (_chapterTitleText != null) _chapterTitleText.text = subtitle;
            }

            yield return new WaitForSecondsRealtime(3.5f);

            if (_titleCardPanel != null) _titleCardPanel.SetActive(false);
            SetLetterbox(false, 0.6f);

            Hide();
            UIEventBus.PublishUIStateChanged(UIState.GameplayHUD);
            onComplete?.Invoke();
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
