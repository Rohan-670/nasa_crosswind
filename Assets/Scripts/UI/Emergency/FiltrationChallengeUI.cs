using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Outpost.UI.Core;

namespace Outpost.UI.Emergency
{
    /// <summary>
    /// Interactive puzzle presentation for the Chapter 5 emergency filtration challenge.
    /// Inspired by the historical Apollo 13 engineering improvisation while clearly distinguishing
    /// the real 1970 event from this fictional 2045 lunar outpost mission.
    /// </summary>
    public class FiltrationChallengeUI : UIView
    {
        [Header("Historical Inspiration Note")]
        [SerializeField] private TextMeshProUGUI _historicalBannerText;

        [Header("Puzzle Progression Steps")]
        [SerializeField] private TextMeshProUGUI _currentStepPromptText;
        [SerializeField] private Slider _assemblyProgressBar;
        [SerializeField] private TextMeshProUGUI _feedbackMessageText;

        [Header("Component Selection Buttons")]
        [SerializeField] private Button _btnRigidCover;
        [SerializeField] private Button _btnFlexibleHose;
        [SerializeField] private Button _btnSpaceTape;
        [SerializeField] private Button _btnLithiumCanister;

        [Header("Status Feedback Mappings")]
        [SerializeField] private Image[] _assemblyStepCheckmarks;
        [SerializeField] private TextMeshProUGUI _co2ReadoutText;

        [Header("Completion Action")]
        [SerializeField] private Button _finishButton;

        private int _currentStepIndex = 0; // 0 = Cover, 1 = Hose, 2 = Tape, 3 = Canister
        private Action<bool> _onChallengeCompleteCallback;

        protected override void Awake()
        {
            base.Awake();
            InitializeListeners();
            SetHistoricalBannerText();
            if (_finishButton != null) _finishButton.gameObject.SetActive(false);
        }

        private void SetHistoricalBannerText()
        {
            if (_historicalBannerText != null)
            {
                _historicalBannerText.text =
                    "<b>HISTORICAL INSPIRATION // NASA APOLLO 13 (APRIL 1970)</b>\n" +
                    "<i>In 1970, Apollo 13 astronauts survived an in-flight emergency by adapting square lithium hydroxide canisters to round sockets using plastic bags, cardboard, and gray tape. Our 2045 outpost scenario is a fictional simulation inspired by their real-world teamwork and problem-solving!</i>";
            }
        }

        private void InitializeListeners()
        {
            if (_btnRigidCover != null)
                _btnRigidCover.onClick.AddListener(() => OnComponentSelected(0, "Rigid Manual Cover & Adapter Frame"));

            if (_btnFlexibleHose != null)
                _btnFlexibleHose.onClick.AddListener(() => OnComponentSelected(1, "Flexible Thermal Duct Hose"));

            if (_btnSpaceTape != null)
                _btnSpaceTape.onClick.AddListener(() => OnComponentSelected(2, "Airtight Gray Space Duct Tape"));

            if (_btnLithiumCanister != null)
                _btnLithiumCanister.onClick.AddListener(() => OnComponentSelected(3, "Lithium Hydroxide (LiOH) Filter Canister"));

            if (_finishButton != null)
                _finishButton.onClick.AddListener(OnFinishChallengeClicked);
        }

        public void StartChallenge(Action<bool> onComplete = null)
        {
            _onChallengeCompleteCallback = onComplete;
            _currentStepIndex = 0;
            if (_assemblyProgressBar != null) _assemblyProgressBar.value = 0f;
            if (_finishButton != null) _finishButton.gameObject.SetActive(false);

            for (int i = 0; i < _assemblyStepCheckmarks.Length; i++)
            {
                if (_assemblyStepCheckmarks[i] != null) _assemblyStepCheckmarks[i].color = Color.gray;
            }

            UpdateStepPrompt();
            Show();
        }

        private void UpdateStepPrompt()
        {
            string prompt = _currentStepIndex switch
            {
                0 => "<b>Step 1 of 4:</b> The backup canister is round, but our habitat wall socket is square! Select the <b>Rigid Plastic/Cardboard Cover</b> to construct the adapter base.",
                1 => "<b>Step 2 of 4:</b> Connect the <b>Flexible Thermal Duct Hose</b> to direct exhaled habitat air into the filtration chamber.",
                2 => "<b>Step 3 of 4:</b> Seal all air leakage gaps around the edges using <b>Airtight Gray Space Duct Tape</b>.",
                3 => "<b>Step 4 of 4:</b> Seat the <b>Lithium Hydroxide (LiOH) Canister</b> into the adapter and engage the auxiliary electric blower fan.",
                _ => "<b>Filtration Challenge Complete!</b> Air scrubbers are circulating nominal clean air."
            };

            if (_currentStepPromptText != null) _currentStepPromptText.text = prompt;
            if (_co2ReadoutText != null)
            {
                float ppm = Mathf.Lerp(1620f, 450f, _currentStepIndex / 4f);
                _co2ReadoutText.text = $"Habitat CO2: <color={(_currentStepIndex == 4 ? "#2ECC71" : "#E74C3C")}>{Mathf.RoundToInt(ppm)} ppm</color>";
            }
        }

        private void OnComponentSelected(int componentIndex, string componentName)
        {
            if (componentIndex == _currentStepIndex)
            {
                // Correct Step
                if (_currentStepIndex < _assemblyStepCheckmarks.Length && _assemblyStepCheckmarks[_currentStepIndex] != null)
                {
                    _assemblyStepCheckmarks[_currentStepIndex].color = new Color(0.18f, 0.8f, 0.44f, 1f); // Green
                }

                _currentStepIndex++;
                if (_assemblyProgressBar != null)
                {
                    _assemblyProgressBar.value = _currentStepIndex / 4f;
                }

                if (_feedbackMessageText != null)
                {
                    _feedbackMessageText.text = $"<color=#2ECC71>✓ Correct: Fitted {componentName}!</color>";
                }

                if (_currentStepIndex >= 4)
                {
                    OnChallengeSuccess();
                }
                else
                {
                    UpdateStepPrompt();
                }
            }
            else
            {
                // Incorrect Step Guidance
                if (_feedbackMessageText != null)
                {
                    _feedbackMessageText.text = $"<color=#F5B041>⚠ Hold on! We cannot attach {componentName} yet. Follow the engineering assembly sequence.</color>";
                }
            }
        }

        private void OnChallengeSuccess()
        {
            UpdateStepPrompt();
            if (_finishButton != null) _finishButton.gameObject.SetActive(true);
            if (_feedbackMessageText != null)
            {
                _feedbackMessageText.text = "<color=#2ECC71><b>SUCCESS:</b> Emergency filtration adapter is online! Carbon dioxide is dropping rapidly back to safe levels (450 ppm).</color>";
            }

            UIEventBus.PublishFiltrationOutcome(true);
            UIEventBus.PublishNotification("Life Support", "Emergency filtration active. CO2 scrubbers stabilized.", NotificationType.ObjectiveUpdate);
        }

        private void OnFinishChallengeClicked()
        {
            Hide(false, () => _onChallengeCompleteCallback?.Invoke(true));
        }
    }
}
