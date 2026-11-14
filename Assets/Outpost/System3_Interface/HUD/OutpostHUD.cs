using UnityEngine;
using UnityEngine.UI;
using Outpost.Core.Data;
using Outpost.Core.Events;

namespace Outpost.System3.Interface.HUD
{
    public class OutpostHUD : MonoBehaviour
    {
        [Header("Mission Banner")]
        [SerializeField] private Text chapterTitleText;
        [SerializeField] private Text objectiveDescriptionText;

        [Header("Life Support Gauges")]
        [SerializeField] private Text oxygenText;
        [SerializeField] private Image oxygenFill;
        [SerializeField] private Text co2Text;
        [SerializeField] private Image co2Fill;
        [SerializeField] private Text powerText;
        [SerializeField] private Image powerFill;
        [SerializeField] private Text batteryText;
        [SerializeField] private Image batteryFill;

        [Header("Alerts & Tooltips")]
        [SerializeField] private GameObject emergencyBanner;
        [SerializeField] private Text emergencyBannerText;
        [SerializeField] private GameObject interactionTooltip;
        [SerializeField] private Text interactionPromptText;

        [Header("Player Reference")]
        [SerializeField] private Gameplay.Player.PlayerInteraction playerInteraction;

        private void Start()
        {
            if (emergencyBanner != null) emergencyBanner.SetActive(false);
            if (interactionTooltip != null) interactionTooltip.SetActive(false);

            if (playerInteraction == null)
            {
                playerInteraction = FindFirstObjectByType<Gameplay.Player.PlayerInteraction>();
            }
        }

        private void OnEnable()
        {
            OutpostEventChannel.ChapterChanged += HandleChapterChanged;
            OutpostEventChannel.ObjectiveUpdated += HandleObjectiveUpdated;
            OutpostEventChannel.TelemetryUpdated += HandleTelemetryUpdated;
            OutpostEventChannel.EmergencyTriggered += HandleEmergencyTriggered;
        }

        private void OnDisable()
        {
            OutpostEventChannel.ChapterChanged -= HandleChapterChanged;
            OutpostEventChannel.ObjectiveUpdated -= HandleObjectiveUpdated;
            OutpostEventChannel.TelemetryUpdated -= HandleTelemetryUpdated;
            OutpostEventChannel.EmergencyTriggered -= HandleEmergencyTriggered;
        }

        private void Update()
        {
            UpdateInteractionTooltip();
        }

        private void UpdateInteractionTooltip()
        {
            if (playerInteraction == null || interactionTooltip == null) return;

            string prompt = playerInteraction.GetCurrentPrompt();
            if (!string.IsNullOrEmpty(prompt))
            {
                interactionTooltip.SetActive(true);
                if (interactionPromptText != null) interactionPromptText.text = prompt;
            }
            else
            {
                interactionTooltip.SetActive(false);
            }
        }

        private void HandleChapterChanged(MissionChapterId id, string title, string objective)
        {
            if (chapterTitleText != null) chapterTitleText.text = title;
            if (objectiveDescriptionText != null) objectiveDescriptionText.text = $"OBJECTIVE: {objective}";
        }

        private void HandleObjectiveUpdated(string objective)
        {
            if (objectiveDescriptionText != null) objectiveDescriptionText.text = $"OBJECTIVE: {objective}";
        }

        private void HandleTelemetryUpdated(float o2, float co2, float pwr, float bat)
        {
            // Oxygen
            if (oxygenText != null) oxygenText.text = $"O2: {o2:F1}%";
            if (oxygenFill != null) oxygenFill.fillAmount = o2 / 100f;

            // CO2
            if (co2Text != null)
            {
                co2Text.text = $"CO2: {co2:F0} ppm";
                co2Text.color = co2 > 1000f ? Color.red : (co2 > 650f ? Color.yellow : Color.green);
            }
            if (co2Fill != null) co2Fill.fillAmount = Mathf.Clamp01(co2 / 2000f);

            // Power
            if (powerText != null) powerText.text = $"SOLAR: {pwr:F1} kW";
            if (powerFill != null) powerFill.fillAmount = Mathf.Clamp01(pwr / 60f);

            // Battery
            if (batteryText != null) batteryText.text = $"BATTERY: {bat:F0}%";
            if (batteryFill != null) batteryFill.fillAmount = bat / 100f;
        }

        private void HandleEmergencyTriggered(bool active, string reason)
        {
            if (emergencyBanner != null)
            {
                emergencyBanner.SetActive(active);
                if (active && emergencyBannerText != null) emergencyBannerText.text = reason;
            }
        }
    }
}
