using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Outpost.UI.Cinematics
{
    /// <summary>
    /// Smooth full-screen black/white fader for scene and chapter transitions.
    /// Operates without allocations and obeys unscaledDeltaTime.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class CameraFader : MonoBehaviour
    {
        [SerializeField] private Image _fadeImage;
        private CanvasGroup _canvasGroup;
        private Coroutine _fadeCoroutine;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 0f;
            _canvasGroup.blocksRaycasts = false;
        }

        public void Fade(float targetAlpha, float duration, Action onComplete = null)
        {
            if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = StartCoroutine(FadeRoutine(targetAlpha, duration, onComplete));
        }

        private IEnumerator FadeRoutine(float targetAlpha, float duration, Action onComplete)
        {
            _canvasGroup.blocksRaycasts = targetAlpha > 0.1f;
            float startAlpha = _canvasGroup.alpha;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                _canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
                yield return null;
            }

            _canvasGroup.alpha = targetAlpha;
            _canvasGroup.blocksRaycasts = targetAlpha > 0.1f;
            _fadeCoroutine = null;
            onComplete?.Invoke();
        }
    }
}
