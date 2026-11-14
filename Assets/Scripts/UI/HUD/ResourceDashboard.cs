using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Outpost.UI.Core;

namespace Outpost.UI.HUD
{
    /// <summary>
    /// Displays life-support meters and station utilities with educational tooltips.
    /// Event-driven to eliminate per-frame UI allocations.
    /// </summary>
    public class ResourceDashboard : MonoBehaviour, IResourceDisplay
    {
        [Header("Oxygen UI")]
        [SerializeField] private Slider _oxygenSlider;
        [SerializeField] private TextMeshProUGUI _oxygenText;
        [SerializeField] private Image _oxygenFillImage;

        [Header("Power UI")]
        [SerializeField] private Slider _powerSlider;
        [SerializeField] private TextMeshProUGUI _powerText;
        [SerializeField] private Image _powerFillImage;

        [Header("Water UI")]
        [SerializeField] private Slider _waterSlider;
        [SerializeField] private TextMeshProUGUI _waterText;

        [Header("Carbon Dioxide UI (CO2)")]
        [SerializeField] private TextMeshProUGUI _co2Text;
        [SerializeField] private Image _co2StatusBadge;

        [Header("Status Colors")]
        [SerializeField] private Color _nominalColor = new Color(0.2f, 0.8f, 0.3f, 1f);  // Green
        [SerializeField] private Color _warningColor = new Color(0.95f, 0.7f, 0.1f, 1f); // Amber
        [SerializeField] private Color _criticalColor = new Color(0.95f, 0.2f, 0.2f, 1f); // Red

        private void OnEnable()
        {
            UIEventBus.OnOxygenUpdated += SetOxygen;
            UIEventBus.OnPowerUpdated += SetPower;
            UIEventBus.OnWaterUpdated += SetWater;
            UIEventBus.OnCarbonDioxideUpdated += SetCarbonDioxide;
        }

        private void OnDisable()
        {
            UIEventBus.OnOxygenUpdated -= SetOxygen;
            UIEventBus.OnPowerUpdated -= SetPower;
            UIEventBus.OnWaterUpdated -= SetWater;
            UIEventBus.OnCarbonDioxideUpdated -= SetCarbonDioxide;
        }

        public void SetOxygen(float current, float max, float deltaRate)
        {
            float ratio = max > 0f ? Mathf.Clamp01(current / max) : 0f;
            if (_oxygenSlider != null) _oxygenSlider.value = ratio;
            if (_oxygenText != null) _oxygenText.text = $"O2: {Mathf.RoundToInt(ratio * 100f)}%";

            if (_oxygenFillImage != null)
            {
                _oxygenFillImage.color = ratio > 0.4f ? _nominalColor : (ratio > 0.2f ? _warningColor : _criticalColor);
            }
        }

        public void SetPower(float current, float max, float deltaRate)
        {
            float ratio = max > 0f ? Mathf.Clamp01(current / max) : 0f;
            if (_powerSlider != null) _powerSlider.value = ratio;
            if (_powerText != null) _powerText.text = $"Power: {Mathf.RoundToInt(current)}kW";

            if (_powerFillImage != null)
            {
                _powerFillImage.color = ratio > 0.35f ? _nominalColor : (ratio > 0.15f ? _warningColor : _criticalColor);
            }
        }

        public void SetWater(float current, float max, float deltaRate)
        {
            float ratio = max > 0f ? Mathf.Clamp01(current / max) : 0f;
            if (_waterSlider != null) _waterSlider.value = ratio;
            if (_waterText != null) _waterText.text = $"H2O: {Mathf.RoundToInt(ratio * 100f)}%";
        }

        public void SetCarbonDioxide(float currentPpm, bool isScrubberActive)
        {
            if (_co2Text != null)
            {
                _co2Text.text = $"CO2: {Mathf.RoundToInt(currentPpm)} ppm";
                _co2Text.color = currentPpm < 800f ? _nominalColor : (currentPpm < 1500f ? _warningColor : _criticalColor);
            }

            if (_co2StatusBadge != null)
            {
                _co2StatusBadge.color = isScrubberActive ? _nominalColor : _criticalColor;
            }
        }
    }
}
