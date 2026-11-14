using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Outpost.UI.Core;

namespace Outpost.UI.Menus
{
    /// <summary>
    /// Mission Performance Evaluation screen for Chapter 7.
    /// Evaluates the young player's resource conservation, problem-solving speed,
    /// and emergency recovery choices.
    /// </summary>
    public class MissionPerformanceScreen : UIView
    {
        [Header("Performance Badges & Ranks")]
        [SerializeField] private TextMeshProUGUI _rankBadgeText;
        [SerializeField] private TextMeshProUGUI _missionScoreText;

        [Header("Detailed Evaluation Readouts")]
        [SerializeField] private TextMeshProUGUI _lifeSupportScoreText;
        [SerializeField] private TextMeshProUGUI _strategyChoiceText;
        [SerializeField] private TextMeshProUGUI _filtrationChallengeText;
        [SerializeField] private TextMeshProUGUI _crewSafetyText;

        [Header("Navigation Button")]
        [SerializeField] private Button _viewCreditsButton;
        [SerializeField] private TextMeshProUGUI _viewCreditsButtonText;

        private Action _onProceedToCreditsCallback;

        protected override void Awake()
        {
            base.Awake();
            if (_viewCreditsButton != null)
            {
                _viewCreditsButton.onClick.AddListener(OnProceedClicked);
            }
        }

        public void DisplayEvaluation(EmergencyRepairOption chosenStrategy, Action onProceedToCredits = null)
        {
            _onProceedToCreditsCallback = onProceedToCredits;

            if (_rankBadgeText != null)
            {
                _rankBadgeText.text = "<b>EXPLORER RANK: MASTER LUNAR PIONEER</b>\n<color=#F1C40F>★ ★ ★ ★ ★ (GRADE: S)</color>";
            }

            if (_missionScoreText != null)
            {
                _missionScoreText.text = "MISSION COMPLETION SCORE: 9,850 PTS";
            }

            if (_lifeSupportScoreText != null)
            {
                _lifeSupportScoreText.text = "• <b>Life Support Efficiency:</b> <color=#2ECC71>98.4% Nominal</color> (CO2 restored to 450 ppm)";
            }

            string strategyDesc = chosenStrategy switch
            {
                EmergencyRepairOption.AstronautEVA => "Commander Elena's Spacewalk (Manual Cable Splice)",
                EmergencyRepairOption.RoboticMaintenance => "Marcus & Aisha's Crosswind Rover (Remote Robotic Weld)",
                EmergencyRepairOption.PowerConservation => "Daniel & ORION's Power Reroute (Battery Bus Reconfiguration)",
                _ => "Emergency Strategic Procedure"
            };

            if (_strategyChoiceText != null)
            {
                _strategyChoiceText.text = $"• <b>Emergency Recovery Strategy:</b> <color=#3498DB>{strategyDesc}</color>";
            }

            if (_filtrationChallengeText != null)
            {
                _filtrationChallengeText.text = "• <b>Filtration Challenge:</b> <color=#2ECC71>4 / 4 Steps Successfully Engineered</color>";
            }

            if (_crewSafetyText != null)
            {
                _crewSafetyText.text = "• <b>Crew Safety & Health:</b> <color=#2ECC71>100% (All 4 Astronauts & ORION Safe)</color>";
            }

            if (_viewCreditsButtonText != null)
            {
                _viewCreditsButtonText.text = "View Mission Credits";
            }

            Show();
        }

        private void OnProceedClicked()
        {
            Hide(false, () =>
            {
                _onProceedToCreditsCallback?.Invoke();
                _onProceedToCreditsCallback = null;
            });
        }
    }
}
