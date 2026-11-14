using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Outpost.UI.Core;

namespace Outpost.UI.HUD
{
    /// <summary>
    /// Visual and telemetry presentation for Chapter 2 station activations:
    /// 1. Solar array ridge deployment & power grid ramp up (0 to 45 kW).
    /// 2. Habitat environmental pressure seal inspection.
    /// 3. Greenhouse aeroponic crop cycle initiation.
    /// </summary>
    public class StationActivationPresentation : UIView
    {
        [Header("Telemetry Banner")]
        [SerializeField] private TextMeshProUGUI _systemHeaderTitleText;
        [SerializeField] private TextMeshProUGUI _systemStatusSubtitleText;
        [SerializeField] private Slider _activationMeter;
        [SerializeField] private TextMeshProUGUI _metricValueText;

        [Header("Controls")]
        [SerializeField] private Button _acknowledgeButton;
        [SerializeField] private TextMeshProUGUI _acknowledgeButtonText;

        private Coroutine _activationCoroutine;
        private Action _onActivationCompleteCallback;

        protected override void Awake()
        {
            base.Awake();
            if (_acknowledgeButton != null)
            {
                _acknowledgeButton.onClick.AddListener(OnAcknowledgeClicked);
            }
        }

        public void PresentSolarArrayActivation(Action onComplete = null)
        {
            _onActivationCompleteCallback = onComplete;
            Show();
            if (_acknowledgeButton != null) _acknowledgeButton.gameObject.SetActive(false);

            if (_activationCoroutine != null) StopCoroutine(_activationCoroutine);
            _activationCoroutine = StartCoroutine(RampPowerRoutine());
        }

        public void PresentHabitatInspection(Action onComplete = null)
        {
            _onActivationCompleteCallback = onComplete;
            Show();

            if (_systemHeaderTitleText != null) _systemHeaderTitleText.text = "HABITAT ENVIRONMENTAL SEAL INSPECTION";
            if (_systemStatusSubtitleText != null) _systemStatusSubtitleText.text = "Airlock cycled. Internal pressure confirmed at 101.3 kPa (Earth sea-level equivalent). Micrometeoroid outer shields locked.";
            if (_activationMeter != null) _activationMeter.value = 1f;
            if (_metricValueText != null) _metricValueText.text = "Atmospheric Integrity: 100% NOMINAL";

            if (_acknowledgeButton != null)
            {
                _acknowledgeButton.gameObject.SetActive(true);
                if (_acknowledgeButtonText != null) _acknowledgeButtonText.text = "Confirm Inspection";
            }
        }

        public void PresentGreenhouseInitiation(Action onComplete = null)
        {
            _onActivationCompleteCallback = onComplete;
            Show();

            if (_systemHeaderTitleText != null) _systemHeaderTitleText.text = "GREENHOUSE HYDROPONICS ONLINE";
            if (_systemStatusSubtitleText != null) _systemStatusSubtitleText.text = "Aeroponic nutrient mist active. LED daylight spectrum engaged. Plants absorbing CO2 and producing fresh oxygen.";
            if (_activationMeter != null) _activationMeter.value = 1f;
            if (_metricValueText != null) _metricValueText.text = "Photosynthetic Cycle: ACTIVE";

            if (_acknowledgeButton != null)
            {
                _acknowledgeButton.gameObject.SetActive(true);
                if (_acknowledgeButtonText != null) _acknowledgeButtonText.text = "Acknowledge";
            }
        }

        private IEnumerator RampPowerRoutine()
        {
            if (_systemHeaderTitleText != null) _systemHeaderTitleText.text = "SOLAR FARM PHOTOVOLTAIC DEPLOYMENT";
            if (_systemStatusSubtitleText != null) _systemStatusSubtitleText.text = "Tracking solar azimuth along crater ridge. Aligning photovoltaic panels...";

            float duration = 2.5f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float kw = Mathf.Lerp(0f, 45f, t);

                if (_activationMeter != null) _activationMeter.value = t;
                if (_metricValueText != null) _metricValueText.text = $"Solar Power Output: {kw:F1} kW / 45.0 kW";

                yield return null;
            }

            if (_systemStatusSubtitleText != null) _systemStatusSubtitleText.text = "<color=#2ECC71><b>SOLAR GRID ONLINE:</b> Full 45 kW generation established. Battery storage charging.</color>";
            UIEventBus.PublishElectricityData(45f, 45f, "Crater ridge solar farm operating at peak generation.");
            UIEventBus.PublishBatteryData(100f, 100f, "Battery reserves charged to 100%.");

            if (_acknowledgeButton != null)
            {
                _acknowledgeButton.gameObject.SetActive(true);
                if (_acknowledgeButtonText != null) _acknowledgeButtonText.text = "Grid Online";
            }

            _activationCoroutine = null;
        }

        private void OnAcknowledgeClicked()
        {
            Hide(false, () =>
            {
                _onActivationCompleteCallback?.Invoke();
                _onActivationCompleteCallback = null;
            });
        }
    }
}
