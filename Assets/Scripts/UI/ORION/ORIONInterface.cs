using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Outpost.UI.Core;

namespace Outpost.UI.ORION
{
    /// <summary>
    /// Visual and animated presentation of the ORION AI companion.
    /// Manages holographic visual pulses, alert status rings, and science tutorial drawers.
    /// </summary>
    public class ORIONInterface : MonoBehaviour, IORIONInterface
    {
        [Header("Holographic Avatar")]
        [SerializeField] private Image _hologramCoreImage;
        [SerializeField] private Image _pulsingOuterRing;
        [SerializeField] private TextMeshProUGUI _statusLabelText;

        [Header("Science Tutorial Drawer")]
        [SerializeField] private GameObject _scienceDrawer;
        [SerializeField] private TextMeshProUGUI _scienceTitleText;
        [SerializeField] private TextMeshProUGUI _scienceBodyText;
        [SerializeField] private Button _closeScienceDrawerButton;

        [Header("Status Theme Colors")]
        [SerializeField] private Color _nominalCyan = new Color(0f, 0.95f, 1f, 1f);
        [SerializeField] private Color _cautionYellow = new Color(1f, 0.8f, 0.2f, 1f);
        [SerializeField] private Color _emergencyRed = new Color(1f, 0.25f, 0.25f, 1f);

        private AlertLevel _currentAlertLevel = AlertLevel.Nominal;
        private Coroutine _pulseCoroutine;

        public AlertLevel CurrentAlertLevel => _currentAlertLevel;

        private void Awake()
        {
            if (_scienceDrawer != null) _scienceDrawer.SetActive(false);
            if (_closeScienceDrawerButton != null)
            {
                _closeScienceDrawerButton.onClick.AddListener(CloseScienceExplanation);
            }
        }

        private void OnEnable()
        {
            UIEventBus.OnAlertLevelChanged += SetAlertLevel;
            StartPulsing();
        }

        private void OnDisable()
        {
            UIEventBus.OnAlertLevelChanged -= SetAlertLevel;
            if (_pulseCoroutine != null) StopCoroutine(_pulseCoroutine);
        }

        public void SetAlertLevel(AlertLevel level, string message = "")
        {
            _currentAlertLevel = level;

            Color targetColor = level switch
            {
                AlertLevel.Caution => _cautionYellow,
                AlertLevel.Emergency => _emergencyRed,
                _ => _nominalCyan
            };

            if (_hologramCoreImage != null) _hologramCoreImage.color = targetColor;
            if (_pulsingOuterRing != null) _pulsingOuterRing.color = targetColor;

            if (_statusLabelText != null)
            {
                string statusText = level switch
                {
                    AlertLevel.Caution => "ORION: CAUTION DETECTED",
                    AlertLevel.Emergency => "ORION: EMERGENCY ALERT",
                    _ => "ORION: ALL SYSTEMS NOMINAL"
                };
                if (!string.IsNullOrEmpty(message)) statusText += $" - {message}";
                _statusLabelText.text = statusText;
                _statusLabelText.color = targetColor;
            }
        }

        public void TriggerHologramPulse()
        {
            if (_pulseCoroutine != null) StopCoroutine(_pulseCoroutine);
            _pulseCoroutine = StartCoroutine(PulseRoutine());
        }

        public void SpeakNotification(string speechText)
        {
            TriggerHologramPulse();
            UIEventBus.PublishNotification("ORION AI", speechText, NotificationType.RadioComms);
        }

        public void OpenScienceExplanation(string conceptTitle, string explanation)
        {
            if (_scienceDrawer != null)
            {
                _scienceDrawer.SetActive(true);
                if (_scienceTitleText != null) _scienceTitleText.text = conceptTitle;
                if (_scienceBodyText != null) _scienceBodyText.text = explanation;
            }
        }

        public void CloseScienceExplanation()
        {
            if (_scienceDrawer != null) _scienceDrawer.SetActive(false);
        }

        private void StartPulsing()
        {
            if (_pulseCoroutine != null) StopCoroutine(_pulseCoroutine);
            _pulseCoroutine = StartCoroutine(IdlePulseRoutine());
        }

        private IEnumerator IdlePulseRoutine()
        {
            while (true)
            {
                float t = (Mathf.Sin(Time.unscaledTime * 3f) + 1f) * 0.5f;
                if (_pulsingOuterRing != null)
                {
                    float scale = Mathf.Lerp(1.0f, 1.15f, t);
                    _pulsingOuterRing.transform.localScale = new Vector3(scale, scale, 1f);
                }
                yield return null;
            }
        }

        private IEnumerator PulseRoutine()
        {
            float elapsed = 0f;
            float duration = 0.5f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float progress = elapsed / duration;
                float scale = Mathf.Lerp(1.0f, 1.4f, Mathf.Sin(progress * Mathf.PI));
                if (_pulsingOuterRing != null)
                {
                    _pulsingOuterRing.transform.localScale = new Vector3(scale, scale, 1f);
                }
                yield return null;
            }
            StartPulsing();
        }
    }
}
