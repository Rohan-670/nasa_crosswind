using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Outpost.UI.Core;

namespace Outpost.UI.Emergency
{
    /// <summary>
    /// Interactive diagnostic screen illustrating the life-support electrical disturbance and rising CO2 levels.
    /// Bridges into the emergency filtration challenge.
    /// </summary>
    public class LifeSupportDiagnosticScreen : UIView
    {
        [Header("Diagnostic Telemetry Readouts")]
        [SerializeField] private TextMeshProUGUI _powerBusStatusText;
        [SerializeField] private TextMeshProUGUI _oxygenGeneratorStatusText;
        [SerializeField] private TextMeshProUGUI _co2ScrubberStatusText;
        [SerializeField] private TextMeshProUGUI _currentCo2PpmText;
        [SerializeField] private Slider _co2PpmMeter;

        [Header("Affected Modules Checklist")]
        [SerializeField] private TextMeshProUGUI _moduleAirlockStatusText;
        [SerializeField] private TextMeshProUGUI _moduleHabCoreStatusText;
        [SerializeField] private TextMeshProUGUI _moduleScienceLabStatusText;

        [Header("Navigation Buttons")]
        [SerializeField] private Button _startFiltrationChallengeButton;
        [SerializeField] private Button _closeDiagnosticButton;

        private Action _onLaunchFiltrationChallengeCallback;

        protected override void Awake()
        {
            base.Awake();
            if (_closeDiagnosticButton != null)
                _closeDiagnosticButton.onClick.AddListener(() => Hide());

            if (_startFiltrationChallengeButton != null)
                _startFiltrationChallengeButton.onClick.AddListener(OnLaunchChallengeClicked);

            SetEmergencyState();
        }

        public void OpenDiagnostic(Action onLaunchFiltration)
        {
            _onLaunchFiltrationChallengeCallback = onLaunchFiltration;
            SetEmergencyState();
            Show();
        }

        private void SetEmergencyState()
        {
            if (_powerBusStatusText != null)
            {
                _powerBusStatusText.text = "POWER BUS 2: <color=#E74C3C>SEVERED (EXTERNAL IMPACT)</color>";
            }
            if (_oxygenGeneratorStatusText != null)
            {
                _oxygenGeneratorStatusText.text = "O2 ELECTROLYSIS: <color=#F5B041>OFFLINE (LOW VOLTAGE)</color>";
            }
            if (_co2ScrubberStatusText != null)
            {
                _co2ScrubberStatusText.text = "CO2 SCRUBBERS: <color=#E74C3C>FAULT — BLOWER INTERRUPTED</color>";
            }
            if (_currentCo2PpmText != null)
            {
                _currentCo2PpmText.text = "ATMOSPHERIC CO2: <color=#E74C3C>1,620 PPM (RISING DANGEROUSLY)</color>";
            }
            if (_co2PpmMeter != null)
            {
                _co2PpmMeter.value = 0.82f;
            }

            if (_moduleAirlockStatusText != null) _moduleAirlockStatusText.text = "• Airlock Decompression: <color=#2ECC71>SEALED</color>";
            if (_moduleHabCoreStatusText != null) _moduleHabCoreStatusText.text = "• Hab Core Atmosphere: <color=#E74C3C>CRITICAL CO2 ACCUMULATION</color>";
            if (_moduleScienceLabStatusText != null) _moduleScienceLabStatusText.text = "• Science Lab Scrubbing: <color=#F5B041>AUXILIARY FAN ONLY</color>";
        }

        private void OnLaunchChallengeClicked()
        {
            Hide(false, () => _onLaunchFiltrationChallengeCallback?.Invoke());
        }
    }
}
