using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Outpost.UI.Core;

namespace Outpost.UI.Emergency
{
    /// <summary>
    /// Decision modal presenting the three actual gameplay recovery choices supplied by Computer 1.
    /// Informs the player of risks, benefits, and consequences for children aged 7–12.
    /// </summary>
    public class EmergencyDecisionModal : UIView
    {
        [Header("Decision Cards UI")]
        [SerializeField] private Button _btnOptionElena;
        [SerializeField] private Button _btnOptionMarcus;
        [SerializeField] private Button _btnOptionDaniel;

        [Header("Decision Confirmation Feedback")]
        [SerializeField] private GameObject _confirmationPanel;
        [SerializeField] private TextMeshProUGUI _confirmationTitleText;
        [SerializeField] private TextMeshProUGUI _confirmationBodyText;
        [SerializeField] private Button _proceedButton;

        private EmergencyRepairOption _selectedOption = EmergencyRepairOption.None;
        private Action<EmergencyRepairOption> _onDecisionMadeCallback;

        protected override void Awake()
        {
            base.Awake();
            InitializeButtons();
            if (_confirmationPanel != null) _confirmationPanel.SetActive(false);
        }

        private void InitializeButtons()
        {
            if (_btnOptionElena != null)
                _btnOptionElena.onClick.AddListener(() => SelectOption(EmergencyRepairOption.AstronautEVA));

            if (_btnOptionMarcus != null)
                _btnOptionMarcus.onClick.AddListener(() => SelectOption(EmergencyRepairOption.RoboticMaintenance));

            if (_btnOptionDaniel != null)
                _btnOptionDaniel.onClick.AddListener(() => SelectOption(EmergencyRepairOption.PowerConservation));

            if (_proceedButton != null)
                _proceedButton.onClick.AddListener(OnProceedClicked);
        }

        public void OpenDecisionModal(Action<EmergencyRepairOption> onDecisionMade)
        {
            _onDecisionMadeCallback = onDecisionMade;
            _selectedOption = EmergencyRepairOption.None;
            if (_confirmationPanel != null) _confirmationPanel.SetActive(false);
            Show();
        }

        private void SelectOption(EmergencyRepairOption option)
        {
            _selectedOption = option;

            if (_confirmationPanel != null)
            {
                _confirmationPanel.SetActive(true);

                switch (option)
                {
                    case EmergencyRepairOption.AstronautEVA:
                        if (_confirmationTitleText != null) _confirmationTitleText.text = "STRATEGY A: COMMANDER ELENA'S EVA SPACEWALK";
                        if (_confirmationBodyText != null)
                        {
                            _confirmationBodyText.text =
                                "<b>Plan:</b> Elena suits up for an external spacewalk to manually splice the high-voltage cable.\n\n" +
                                "<b>Benefit:</b> Restores full 45kW power grid in under 15 minutes.\n" +
                                "<b>Risk:</b> Consumes 25% suit oxygen reserves; Elena must watch for loose regolith on the crater ridge.";
                        }
                        break;

                    case EmergencyRepairOption.RoboticMaintenance:
                        if (_confirmationTitleText != null) _confirmationTitleText.text = "STRATEGY B: MARCUS & AISHA'S ROBOTIC ROVER WELD";
                        if (_confirmationBodyText != null)
                        {
                            _confirmationBodyText.text =
                                "<b>Plan:</b> Remote-pilot the NASA Crosswind Exploration Rover to weld the severed junction with its robotic arm.\n\n" +
                                "<b>Benefit:</b> 100% safe for crew members inside the reinforced habitat.\n" +
                                "<b>Risk:</b> High electrical drain on rover battery; requires careful driving over boulders.";
                        }
                        break;

                    case EmergencyRepairOption.PowerConservation:
                        if (_confirmationTitleText != null) _confirmationTitleText.text = "STRATEGY C: DANIEL & ORION'S POWER REROUTE";
                        if (_confirmationBodyText != null)
                        {
                            _confirmationBodyText.text =
                                "<b>Plan:</b> Reroute secondary battery bus through the greenhouse line and brown out non-essential base lighting.\n\n" +
                                "<b>Benefit:</b> Zero external hazards; no spacewalk required.\n" +
                                "<b>Risk:</b> Base interior temperature drops to 16°C; delay repair until high sun.";
                        }
                        break;
                }
            }
        }

        private void OnProceedClicked()
        {
            if (_selectedOption == EmergencyRepairOption.None) return;

            UIEventBus.PublishEmergencyOption(_selectedOption);
            Hide(false, () => _onDecisionMadeCallback?.Invoke(_selectedOption));
        }
    }
}
