using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Outpost.UI.Core;

namespace Outpost.UI.Emergency
{
    /// <summary>
    /// High-priority visual and audio alert banner for emergencies (Micrometeoroid Impact, Life-Support Disturbance).
    /// Features pulsing red hazard borders and audio alarm siren cues.
    /// </summary>
    public class EmergencyAlertPanel : UIView
    {
        [Header("UI Elements")]
        [SerializeField] private Image _hazardBorderImage;
        [SerializeField] private TextMeshProUGUI _alertHeaderTitleText;
        [SerializeField] private TextMeshProUGUI _alertDescriptionText;
        [SerializeField] private Button _acknowledgeButton;

        [Header("Hazard Colors")]
        [SerializeField] private Color _emergencyRed = new Color(0.95f, 0.2f, 0.2f, 1f);
        [SerializeField] private Color _hazardAmber = new Color(0.96f, 0.72f, 0.15f, 1f);

        private Coroutine _strobeCoroutine;
        private Action _onAcknowledgeCallback;

        protected override void Awake()
        {
            base.Awake();
            if (_acknowledgeButton != null)
            {
                _acknowledgeButton.onClick.AddListener(OnAcknowledgeClicked);
            }
        }

        public void DisplayEmergency(string title, string description, AlertLevel level = AlertLevel.Emergency, Action onAcknowledge = null)
        {
            _onAcknowledgeCallback = onAcknowledge;
            if (_alertHeaderTitleText != null) _alertHeaderTitleText.text = title.ToUpper();
            if (_alertDescriptionText != null) _alertDescriptionText.text = description;

            Color targetColor = level == AlertLevel.Emergency ? _emergencyRed : _hazardAmber;
            if (_hazardBorderImage != null) _hazardBorderImage.color = targetColor;

            Show();
            if (_strobeCoroutine != null) StopCoroutine(_strobeCoroutine);
            _strobeCoroutine = StartCoroutine(StrobeRoutine(targetColor));
        }

        private void OnAcknowledgeClicked()
        {
            if (_strobeCoroutine != null) StopCoroutine(_strobeCoroutine);
            Hide(false, () => _onAcknowledgeCallback?.Invoke());
        }

        private IEnumerator StrobeRoutine(Color baseColor)
        {
            while (true)
            {
                float t = (Mathf.Sin(Time.unscaledTime * 6f) + 1f) * 0.5f;
                if (_hazardBorderImage != null)
                {
                    _hazardBorderImage.color = Color.Lerp(baseColor * 0.4f, baseColor, t);
                }
                yield return null;
            }
        }
    }
}
