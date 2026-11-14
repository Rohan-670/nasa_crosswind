using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Outpost.UI.Core;

namespace Outpost.UI.Notifications
{
    /// <summary>
    /// Individual animated notification banner/toast.
    /// Recycled in a pool to prevent WebGL heap fragmentation.
    /// </summary>
    public class NotificationToast : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Image _iconImage;
        [SerializeField] private Image _borderImage;
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _messageText;
        [SerializeField] private CanvasGroup _canvasGroup;

        [Header("Type Colors")]
        [SerializeField] private Color _infoColor = new Color(0.2f, 0.6f, 1f, 1f);
        [SerializeField] private Color _objectiveColor = new Color(1f, 0.8f, 0.2f, 1f);
        [SerializeField] private Color _discoveryColor = new Color(0.2f, 0.9f, 0.5f, 1f);
        [SerializeField] private Color _emergencyColor = new Color(1f, 0.2f, 0.2f, 1f);

        private Action<NotificationToast> _onDismissedCallback;
        private Coroutine _displayCoroutine;

        public void Initialize(string title, string message, NotificationType type, float duration, Action<NotificationToast> onDismissed)
        {
            _onDismissedCallback = onDismissed;
            if (_titleText != null) _titleText.text = title;
            if (_messageText != null) _messageText.text = message;

            Color accent = type switch
            {
                NotificationType.ObjectiveUpdate => _objectiveColor,
                NotificationType.Discovery => _discoveryColor,
                NotificationType.EmergencyAlert => _emergencyColor,
                _ => _infoColor
            };

            if (_borderImage != null) _borderImage.color = accent;
            if (_titleText != null) _titleText.color = accent;

            if (_displayCoroutine != null) StopCoroutine(_displayCoroutine);
            _displayCoroutine = StartCoroutine(ToastLifecycle(duration));
        }

        private IEnumerator ToastLifecycle(float duration)
        {
            if (_canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();

            // Fade In
            float t = 0f;
            while (t < 0.25f)
            {
                t += Time.unscaledDeltaTime;
                if (_canvasGroup != null) _canvasGroup.alpha = t / 0.25f;
                yield return null;
            }
            if (_canvasGroup != null) _canvasGroup.alpha = 1f;

            yield return new WaitForSecondsRealtime(duration);

            // Fade Out
            t = 0f;
            while (t < 0.25f)
            {
                t += Time.unscaledDeltaTime;
                if (_canvasGroup != null) _canvasGroup.alpha = 1f - (t / 0.25f);
                yield return null;
            }

            gameObject.SetActive(false);
            _displayCoroutine = null;
            _onDismissedCallback?.Invoke(this);
        }
    }
}
