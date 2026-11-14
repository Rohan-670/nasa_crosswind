using UnityEngine;
using UnityEngine.UI;
using Outpost.Core.Data;
using Outpost.Core.Events;

namespace Outpost.System3.Interface.Orion
{
    public class OrionAssistantUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject orionPanel;
        [SerializeField] private Text titleText;
        [SerializeField] private Text bodyText;
        [SerializeField] private Text funFactText;
        [SerializeField] private Button dismissButton;

        private void Awake()
        {
            if (dismissButton != null) dismissButton.onClick.AddListener(ClosePanel);
        }

        private void Start()
        {
            if (orionPanel != null) orionPanel.SetActive(false);
        }

        private void OnEnable()
        {
            OutpostEventChannel.EducationalFactPrompted += HandleEducationalFactPrompted;
            OutpostEventChannel.EmergencyTriggered += HandleEmergencyTriggered;
        }

        private void OnDisable()
        {
            OutpostEventChannel.EducationalFactPrompted -= HandleEducationalFactPrompted;
            OutpostEventChannel.EmergencyTriggered -= HandleEmergencyTriggered;
        }

        private void HandleEducationalFactPrompted(EducationalFact fact)
        {
            if (orionPanel != null) orionPanel.SetActive(true);

            if (titleText != null) titleText.text = $"O.R.I.O.N. SCIENCE LOG: {fact.Title}";
            if (bodyText != null) bodyText.text = $"{fact.ShortSummary}\n\n{fact.DetailedExplanation}";
            if (funFactText != null) funFactText.text = $"DID YOU KNOW?\n{fact.FunFact}";
        }

        private void HandleEmergencyTriggered(bool active, string reason)
        {
            if (active)
            {
                if (orionPanel != null) orionPanel.SetActive(true);
                if (titleText != null) titleText.text = "O.R.I.O.N. EMERGENCY ALERT";
                if (bodyText != null) bodyText.text = $"{reason}\n\nImmediate crew action required to safeguard life support!";
                if (funFactText != null) funFactText.text = "Protocol 13: Utilize available equipment storage components to build emergency adapters.";
            }
        }

        public void ClosePanel()
        {
            if (orionPanel != null) orionPanel.SetActive(false);
            OutpostEventChannel.RaisePlaySfx("ui_click");
        }
    }
}
