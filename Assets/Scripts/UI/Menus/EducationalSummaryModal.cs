using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Outpost.UI.Core;

namespace Outpost.UI.Menus
{
    /// <summary>
    /// Educational Summary Screen presented upon completing Mission 1: The First Frontier.
    /// Synthesizes real NASA lunar science concepts for students and children aged 7–12:
    /// 1. Why we explore the Lunar South Pole (Water ice & eternal solar light).
    /// 2. How closed-loop life support recycling works on an outpost.
    /// 3. Clear historical distinction honoring Apollo 13's 1970 engineering miracle.
    /// </summary>
    public class EducationalSummaryModal : UIView
    {
        [Header("Educational Content")]
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _topic1Text; // South Pole & Water Ice
        [SerializeField] private TextMeshProUGUI _topic2Text; // Closed Loop Life Support
        [SerializeField] private TextMeshProUGUI _topic3Text; // Apollo 13 Inspiration vs 2045 Fiction

        [Header("Controls")]
        [SerializeField] private Button _continueButton;
        [SerializeField] private TextMeshProUGUI _continueButtonText;

        private Action _onProceedCallback;

        protected override void Awake()
        {
            base.Awake();
            if (_continueButton != null)
            {
                _continueButton.onClick.AddListener(OnContinueClicked);
            }
            PopulateEducationalText();
        }

        public void DisplayEducationalSummary(Action onProceed = null)
        {
            _onProceedCallback = onProceed;
            Show();
        }

        private void PopulateEducationalText()
        {
            if (_titleText != null)
            {
                _titleText.text = "MISSION 1: LUNAR SCIENCE & DISCOVERY SUMMARY";
            }

            if (_topic1Text != null)
            {
                _topic1Text.text =
                    "<b>1. WHY THE LUNAR SOUTH POLE?</b>\n" +
                    "The Moon's south pole features unique terrain where high crater rims receive near-constant sunlight (peaks of eternal light), providing continuous solar power. Nearby deep crater floors, like Shackleton Crater, remain in permanent shadow and hold billions of kilograms of water ice!";
            }

            if (_topic2Text != null)
            {
                _topic2Text.text =
                    "<b>2. CLOSED-LOOP LIFE SUPPORT:</b>\n" +
                    "To survive far from Earth, every drop of water and breath of air must be recycled. Solar electricity splits lunar water ice into hydrogen and breathable oxygen, while chemical canisters (Lithium Hydroxide) remove exhaled carbon dioxide so the crew can breathe safely.";
            }

            if (_topic3Text != null)
            {
                _topic3Text.text =
                    "<b>3. HISTORICAL INSPIRATION // NASA APOLLO 13 (APRIL 1970):</b>\n" +
                    "<i>In 1970, Apollo 13 suffered an oxygen tank explosion en route to the Moon. Through teamwork, astronauts and Mission Control engineered an emergency adapter to fit square carbon dioxide scrubbers into round sockets using duct tape and plastic flight manuals. Our 2045 outpost emergency is a fictional educational adventure inspired by their heroic ingenuity!</i>";
            }

            if (_continueButtonText != null)
            {
                _continueButtonText.text = "View Mission Performance Evaluation";
            }
        }

        private void OnContinueClicked()
        {
            Hide(false, () =>
            {
                _onProceedCallback?.Invoke();
                _onProceedCallback = null;
            });
        }
    }
}
