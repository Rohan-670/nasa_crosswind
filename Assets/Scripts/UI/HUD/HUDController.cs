using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Outpost.UI.Core;

namespace Outpost.UI.HUD
{
    /// <summary>
    /// Master controller for the Phase 1 in-game heads-up display.
    /// Manages mission title, active objective, interaction prompts, subtitles, and pause indicators.
    /// </summary>
    public class HUDController : UIView
    {
        [Header("Mission Title")]
        [SerializeField] private TextMeshProUGUI _missionTitleText;
        [SerializeField] private TextMeshProUGUI _locationBannerText;

        [Header("Objective Banner")]
        [SerializeField] private TextMeshProUGUI _currentObjectiveText;
        [SerializeField] private Image _objectiveIcon;

        [Header("Interaction Prompt")]
        [SerializeField] private GameObject _interactionPromptPanel;
        [SerializeField] private TextMeshProUGUI _interactionPromptText;
        [SerializeField] private Image _crosshair;

        [Header("Pause Indicator")]
        [SerializeField] private GameObject _pauseIndicatorBadge;

        [Header("Sub-Displays")]
        [SerializeField] private ResourceDashboard _resourceDashboard;
        [SerializeField] private ObjectiveMarkerUI _objectiveMarkerUI;

        public ResourceDashboard Resources => _resourceDashboard;
        public ObjectiveMarkerUI Markers => _objectiveMarkerUI;

        protected override void Awake()
        {
            base.Awake();
            if (_missionTitleText != null && string.IsNullOrEmpty(_missionTitleText.text))
            {
                _missionTitleText.text = "OUTPOST: SURVIVE BEYOND EARTH";
            }
            if (_locationBannerText != null && string.IsNullOrEmpty(_locationBannerText.text))
            {
                _locationBannerText.text = "LUNAR SOUTH POLE - 2045";
            }
            if (_pauseIndicatorBadge != null) _pauseIndicatorBadge.SetActive(false);
            ClearInteractionPrompt();
        }

        private void OnEnable()
        {
            UIEventBus.OnObjectiveUpdated += SetCurrentObjective;
            UIEventBus.OnPauseToggled += SetPauseIndicator;
        }

        private void OnDisable()
        {
            UIEventBus.OnObjectiveUpdated -= SetCurrentObjective;
            UIEventBus.OnPauseToggled -= SetPauseIndicator;
        }

        public void SetCurrentObjective(string objectiveId, string description)
        {
            if (_currentObjectiveText != null)
            {
                _currentObjectiveText.text = description;
            }
        }

        public void SetInteractionPrompt(string prompt)
        {
            bool hasPrompt = !string.IsNullOrEmpty(prompt);
            if (_interactionPromptPanel != null) _interactionPromptPanel.SetActive(hasPrompt);
            if (_interactionPromptText != null) _interactionPromptText.text = prompt;

            if (_crosshair != null)
            {
                _crosshair.color = hasPrompt ? new Color(1f, 0.9f, 0.2f, 1f) : Color.white;
            }
        }

        public void ClearInteractionPrompt()
        {
            SetInteractionPrompt(string.Empty);
        }

        public void SetPauseIndicator(bool isPaused)
        {
            if (_pauseIndicatorBadge != null)
            {
                _pauseIndicatorBadge.SetActive(isPaused);
            }
        }
    }
}
