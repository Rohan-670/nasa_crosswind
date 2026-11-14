using System;
using System.Collections;
using UnityEngine;

namespace Outpost.UI.Core
{
    /// <summary>
    /// Base class for all managed UI panels and screens.
    /// Provides smooth CanvasGroup fading without GC allocations and manages input interactivity.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class UIView : MonoBehaviour
    {
        [Header("View Settings")]
        [SerializeField] private UIState _associatedState;
        [SerializeField] private bool _hideOnAwake = true;
        [SerializeField] private float _fadeDuration = 0.2f;

        private CanvasGroup _canvasGroup;
        private Coroutine _fadeCoroutine;

        public UIState AssociatedState => _associatedState;
        public bool IsVisible => _canvasGroup != null && _canvasGroup.alpha > 0.01f;

        protected virtual void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            if (_hideOnAwake)
            {
                SetInstant(false);
            }
        }

        public virtual void Show(bool immediate = false, Action onComplete = null)
        {
            gameObject.SetActive(true);
            if (immediate || _fadeDuration <= 0f)
            {
                SetInstant(true);
                onComplete?.Invoke();
            }
            else
            {
                StartFade(1f, true, onComplete);
            }
        }

        public virtual void Hide(bool immediate = false, Action onComplete = null)
        {
            if (immediate || _fadeDuration <= 0f)
            {
                SetInstant(false);
                gameObject.SetActive(false);
                onComplete?.Invoke();
            }
            else
            {
                StartFade(0f, false, () =>
                {
                    gameObject.SetActive(false);
                    onComplete?.Invoke();
                });
            }
        }

        private void SetInstant(bool visible)
        {
            if (_canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = visible ? 1f : 0f;
            _canvasGroup.interactable = visible;
            _canvasGroup.blocksRaycasts = visible;
        }

        private void StartFade(float targetAlpha, bool interactable, Action onComplete)
        {
            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
            }
            _fadeCoroutine = StartCoroutine(FadeRoutine(targetAlpha, interactable, onComplete));
        }

        private IEnumerator FadeRoutine(float targetAlpha, bool interactable, Action onComplete)
        {
            float startAlpha = _canvasGroup.alpha;
            float elapsed = 0f;

            if (targetAlpha > 0.5f)
            {
                _canvasGroup.blocksRaycasts = true;
            }

            while (elapsed < _fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                _canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / _fadeDuration);
                yield return null;
            }

            _canvasGroup.alpha = targetAlpha;
            _canvasGroup.interactable = interactable;
            _canvasGroup.blocksRaycasts = interactable;
            _fadeCoroutine = null;
            onComplete?.Invoke();
        }
    }
}
