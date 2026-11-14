using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Outpost.UI.Core;

namespace Outpost.UI.HUD
{
    public enum TutorialType
    {
        MapNavigation = 0,
        ResourceDashboard = 1
    }

    /// <summary>
    /// Interactive educational tutorial overlay for Chapter 2.
    /// Guides young players (ages 7–12) through the Interactive Lunar Map and Life Support Telemetry Dashboard.
    /// </summary>
    public class TutorialOverlayUI : UIView
    {
        [Header("Tutorial Card UI Elements")]
        [SerializeField] private TextMeshProUGUI _tutorialTitleText;
        [SerializeField] private TextMeshProUGUI _tutorialBodyText;
        [SerializeField] private TextMeshProUGUI _stepIndicatorText;
        [SerializeField] private Button _nextButton;
        [SerializeField] private TextMeshProUGUI _nextButtonText;
        [SerializeField] private Button _skipTutorialButton;

        private TutorialType _currentTutorialType;
        private int _currentStep = 0;
        private Action _onTutorialCompleteCallback;

        protected override void Awake()
        {
            base.Awake();
            if (_nextButton != null) _nextButton.onClick.AddListener(OnNextStepClicked);
            if (_skipTutorialButton != null) _skipTutorialButton.onClick.AddListener(CompleteTutorial);
        }

        public void StartTutorial(TutorialType tutorialType, Action onComplete = null)
        {
            _currentTutorialType = tutorialType;
            _currentStep = 0;
            _onTutorialCompleteCallback = onComplete;

            Show();
            UpdateStepContent();
        }

        private void OnNextStepClicked()
        {
            _currentStep++;
            if (IsLastStep())
            {
                CompleteTutorial();
            }
            else
            {
                UpdateStepContent();
            }
        }

        private bool IsLastStep()
        {
            int totalSteps = _currentTutorialType == TutorialType.MapNavigation ? 3 : 3;
            return _currentStep >= totalSteps;
        }

        private void UpdateStepContent()
        {
            if (_currentTutorialType == TutorialType.MapNavigation)
            {
                switch (_currentStep)
                {
                    case 0:
                        SetCard(
                            "LUNAR MAP TUTORIAL: STATION LAYOUT",
                            "<b>Step 1:</b> Press [M] or tap the <b>Map</b> button on your HUD to open the top-down station view. Your astronaut position is marked with a glowing blue chevron marker.",
                            "Step 1 of 3",
                            "Next [Space]"
                        );
                        break;
                    case 1:
                        SetCard(
                            "LUNAR MAP TUTORIAL: PAN & ZOOM",
                            "<b>Step 2:</b> Click and drag the map to pan across the Shackleton Crater rim. Use your mouse scroll wheel or the [+ / -] zoom buttons to view distant facilities like the Solar Farm and Rover Station.",
                            "Step 2 of 3",
                            "Next [Space]"
                        );
                        break;
                    case 2:
                        SetCard(
                            "LUNAR MAP TUTORIAL: WAYPOINTS & OBJECTIVES",
                            "<b>Step 3:</b> Click on any station icon to read its purpose and availability. Click <b>'Set Navigation Waypoint'</b> to project a guidance route directly onto your rover compass!",
                            "Step 3 of 3",
                            "Got It!"
                        );
                        break;
                }
            }
            else if (_currentTutorialType == TutorialType.ResourceDashboard)
            {
                switch (_currentStep)
                {
                    case 0:
                        SetCard(
                            "RESOURCE DASHBOARD: THE 8 VITAL METRICS",
                            "<b>Step 1:</b> The top HUD monitors your 8 core survival systems: Oxygen, Recycled Water, Hydroponic Food, Solar Electricity, Battery Reserve, Crew Health, Equipment Condition, and Spare Parts.",
                            "Step 1 of 3",
                            "Next [Space]"
                        );
                        break;
                    case 1:
                        SetCard(
                            "RESOURCE DASHBOARD: ALERT COLOR CODING",
                            "<b>Step 2:</b> Keep an eye on status colors:\n• <color=#2ECC71><b>GREEN (Nominal):</b></color> Outpost operating smoothly.\n• <color=#F39C12><b>AMBER (Warning):</b></color> Consumption exceeds generation.\n• <color=#E74C3C><b>RED (Critical):</b></color> Immediate action required!",
                            "Step 2 of 3",
                            "Next [Space]"
                        );
                        break;
                    case 2:
                        SetCard(
                            "RESOURCE DASHBOARD: SCIENCE TELEMETRY",
                            "<b>Step 3:</b> Click any resource bar to open ORION's <b>Scientific Telemetry Modal</b>, explaining real lunar physics like water electrolysis and solar tracking!",
                            "Step 3 of 3",
                            "Got It!"
                        );
                        break;
                }
            }
        }

        private void SetCard(string title, string body, string step, string buttonLabel)
        {
            if (_tutorialTitleText != null) _tutorialTitleText.text = title;
            if (_tutorialBodyText != null) _tutorialBodyText.text = body;
            if (_stepIndicatorText != null) _stepIndicatorText.text = step;
            if (_nextButtonText != null) _nextButtonText.text = buttonLabel;
        }

        private void CompleteTutorial()
        {
            Hide(false, () =>
            {
                _onTutorialCompleteCallback?.Invoke();
                _onTutorialCompleteCallback = null;
            });
        }
    }
}
