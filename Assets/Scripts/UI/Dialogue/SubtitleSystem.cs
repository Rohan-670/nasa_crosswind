using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Outpost.UI.Dialogue
{
    /// <summary>
    /// Accessible subtitle rendering system.
    /// Ensures high legibility on WebGL for young audiences (ages 7-12) with typewriter effects and sound descriptor tags.
    /// </summary>
    public class SubtitleSystem : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject _subtitlePanel;
        [SerializeField] private TextMeshProUGUI _subtitleText;
        [SerializeField] private Image _backgroundPill;

        [Header("Typewriter Settings")]
        [SerializeField] private float _charactersPerSecond = 35f; // Child-friendly reading pace
        [SerializeField] private bool _useTypewriter = true;

        private Coroutine _typewriterCoroutine;
        private Action _onLineComplete;
        private string _currentFullText = "";
        private bool _isTyping = false;

        public bool IsTyping => _isTyping;

        public void DisplayLine(string text, Action onComplete = null)
        {
            if (_subtitlePanel != null) _subtitlePanel.SetActive(true);
            _currentFullText = text;
            _onLineComplete = onComplete;

            if (_typewriterCoroutine != null)
            {
                StopCoroutine(_typewriterCoroutine);
            }

            if (_useTypewriter && _charactersPerSecond > 0f)
            {
                _typewriterCoroutine = StartCoroutine(TypewriterRoutine(text));
            }
            else
            {
                if (_subtitleText != null) _subtitleText.text = text;
                _isTyping = false;
                _onLineComplete?.Invoke();
            }
        }

        public void FastForward()
        {
            if (_isTyping)
            {
                if (_typewriterCoroutine != null) StopCoroutine(_typewriterCoroutine);
                if (_subtitleText != null) _subtitleText.text = _currentFullText;
                _isTyping = false;
                _onLineComplete?.Invoke();
            }
        }

        public void HideSubtitles()
        {
            if (_typewriterCoroutine != null) StopCoroutine(_typewriterCoroutine);
            _isTyping = false;
            if (_subtitlePanel != null) _subtitlePanel.SetActive(false);
        }

        private IEnumerator TypewriterRoutine(string fullText)
        {
            _isTyping = true;
            if (_subtitleText != null) _subtitleText.text = "";

            float delay = 1f / _charactersPerSecond;
            int totalChars = fullText.Length;

            for (int i = 0; i <= totalChars; i++)
            {
                if (_subtitleText != null)
                {
                    _subtitleText.text = fullText.Substring(0, i);
                }
                yield return new WaitForSecondsRealtime(delay);
            }

            _isTyping = false;
            _typewriterCoroutine = null;
            _onLineComplete?.Invoke();
        }
    }
}
