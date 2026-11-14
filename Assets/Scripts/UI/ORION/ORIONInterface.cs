using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Outpost.UI.Core;

namespace Outpost.UI.ORION
{
    /// <summary>
    /// Master visual presentation for the Onboard Robotic Intelligence and Operations Network (ORION).
    /// Provides consistent, supportive AI companion messaging, equipment warnings, science tutoring, and navigation.
    /// </summary>
    public class ORIONInterface : MonoBehaviour, IORIONInterface
    {
        [Header("Holographic Avatar")]
        [SerializeField] private Image _hologramCoreImage;
        [SerializeField] private Image _pulsingOuterRing;
        [SerializeField] private TextMeshProUGUI _statusLabelText;

        [Header("Active Message Banner")]
        [SerializeField] private GameObject _messageBannerPanel;
        [SerializeField] private Image _messageTypeIcon;
        [SerializeField] private TextMeshProUGUI _messageTypeText;
        [SerializeField] private TextMeshProUGUI _messageBodyText;
        [SerializeField] private Image _bannerBorderImage;

        [Header("Science Tutorial Drawer")]
        [SerializeField] private GameObject _scienceDrawer;
        [SerializeField] private TextMeshProUGUI _scienceTitleText;
        [SerializeField] private TextMeshProUGUI _scienceBodyText;
        [SerializeField] private Button _closeScienceDrawerButton;

        [Header("Message Log Drawer")]
        [SerializeField] private GameObject _logDrawer;
        [SerializeField] private TextMeshProUGUI _logHistoryText;
        [SerializeField] private Button _toggleLogButton;
        [SerializeField] private Button _closeLogButton;

        [Header("Status Theme Colors")]
        [SerializeField] private Color _nominalCyan = new Color(0f, 0.95f, 1f, 1f);
        [SerializeField] private Color _cautionYellow = new Color(1f, 0.8f, 0.2f, 1f);
        [SerializeField] private Color _emergencyRed = new Color(1f, 0.25f, 0.25f, 1f);

        private AlertLevel _currentAlertLevel = AlertLevel.Nominal;
        private Coroutine _pulseCoroutine;
        private Coroutine _bannerCoroutine;
        private readonly List<string> _messageHistory = new List<string>();

        public AlertLevel CurrentAlertLevel => _currentAlertLevel;

        private void Awake()
        {
            if (_messageBannerPanel != null) _messageBannerPanel.SetActive(false);
            if (_scienceDrawer != null) _scienceDrawer.SetActive(false);
            if (_logDrawer != null) _logDrawer.SetActive(false);

            if (_closeScienceDrawerButton != null)
                _closeScienceDrawerButton.onClick.AddListener(CloseScienceExplanation);

            if (_toggleLogButton != null)
                _toggleLogButton.onClick.AddListener(ToggleLogDrawer);

            if (_closeLogButton != null)
                _closeLogButton.onClick.AddListener(() => _logDrawer?.SetActive(false));
        }

        private void OnEnable()
        {
            UIEventBus.OnAlertLevelChanged += (level, reason) => SetAlertLevel(level, reason);
            StartPulsing();
        }

        private void OnDisable()
        {
            UIEventBus.OnAlertLevelChanged -= (level, reason) => SetAlertLevel(level, reason);
            if (_pulseCoroutine != null) StopCoroutine(_pulseCoroutine);
            if (_bannerCoroutine != null) StopCoroutine(_bannerCoroutine);
        }

        public void PostMessage(ORIONMessage message)
        {
            SetAlertLevel(message.Alert, message.Subject);
            TriggerHologramPulse();

            // Record in message log
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            string logEntry = $"[{timestamp}] <b>{message.Type.ToString().ToUpper()}</b>: {message.MessageText}";
            _messageHistory.Add(logEntry);
            UpdateLogHistoryDisplay();

            // Display active banner
            if (_bannerCoroutine != null) StopCoroutine(_bannerCoroutine);
            _bannerCoroutine = StartCoroutine(DisplayBannerRoutine(message));
        }

        public void PostMissionStatus(string statusReport)
        {
            PostMessage(new ORIONMessage
            {
                Type = ORIONMessageType.MissionStatus,
                Subject = "MISSION TELEMETRY",
                MessageText = $"ORION Telemetry: {statusReport}",
                Alert = AlertLevel.Nominal,
                DisplayDuration = 4.5f
            });
        }

        public void PostEquipmentWarning(string equipmentName, string details)
        {
            PostMessage(new ORIONMessage
            {
                Type = ORIONMessageType.EquipmentWarning,
                Subject = "EQUIPMENT CAUTION",
                MessageText = $"Commander Elena, {equipmentName} is operating below its expected game-performance level. {details}",
                Alert = AlertLevel.Caution,
                DisplayDuration = 5.5f
            });
        }

        public void PostResourceWarning(string resourceName, float current, float threshold)
        {
            PostMessage(new ORIONMessage
            {
                Type = ORIONMessageType.ResourceWarning,
                Subject = "RESOURCE ADVISORY",
                MessageText = $"Attention crew: {resourceName} reserves have dropped to {Mathf.RoundToInt(current)}. Threshold is {Mathf.RoundToInt(threshold)}.",
                Alert = AlertLevel.Caution,
                DisplayDuration = 5.0f
            });
        }

        public void PostNavigationGuidance(string destinationName, float distanceMeters, string advice = "")
        {
            string msg = $"Navigation guidance: {destinationName} is {Mathf.RoundToInt(distanceMeters)} meters away.";
            if (!string.IsNullOrEmpty(advice)) msg += $" {advice}";

            PostMessage(new ORIONMessage
            {
                Type = ORIONMessageType.NavigationGuidance,
                Subject = "NAVIGATION ASSIST",
                MessageText = msg,
                Alert = AlertLevel.Nominal,
                DisplayDuration = 4.0f
            });
        }

        public void PostScientificExplanation(string conceptTitle, string explanation)
        {
            OpenScienceExplanation(conceptTitle, explanation);
            PostMessage(new ORIONMessage
            {
                Type = ORIONMessageType.ScientificExplanation,
                Subject = "SCIENCE ARCHIVE",
                MessageText = $"Educational briefing opened: {conceptTitle}.",
                Alert = AlertLevel.Nominal,
                DisplayDuration = 3.5f
            });
        }

        public void PostEmergencyNotification(string emergencyDetails)
        {
            PostMessage(new ORIONMessage
            {
                Type = ORIONMessageType.EmergencyNotification,
                Subject = "CRITICAL EMERGENCY",
                MessageText = $"EMERGENCY ALERT: {emergencyDetails} Immediate crew attention required!",
                Alert = AlertLevel.Emergency,
                DisplayDuration = 7.0f
            });
        }

        public void PostObjectiveReminder(string currentObjective)
        {
            PostMessage(new ORIONMessage
            {
                Type = ORIONMessageType.ObjectiveReminder,
                Subject = "OBJECTIVE REMINDER",
                MessageText = $"Active Mission Milestone: {currentObjective}",
                Alert = AlertLevel.Nominal,
                DisplayDuration = 4.0f
            });
        }

        public void SetAlertLevel(AlertLevel level, string message = "")
        {
            _currentAlertLevel = level;

            Color targetColor = level switch
            {
                AlertLevel.Caution => _cautionYellow,
                AlertLevel.Emergency => _emergencyRed,
                _ => _nominalCyan
            };

            if (_hologramCoreImage != null) _hologramCoreImage.color = targetColor;
            if (_pulsingOuterRing != null) _pulsingOuterRing.color = targetColor;

            if (_statusLabelText != null)
            {
                string statusText = level switch
                {
                    AlertLevel.Caution => "ORION: CAUTION DETECTED",
                    AlertLevel.Emergency => "ORION: EMERGENCY ALERT",
                    _ => "ORION: ALL SYSTEMS NOMINAL"
                };
                if (!string.IsNullOrEmpty(message)) statusText += $" — {message}";
                _statusLabelText.text = statusText;
                _statusLabelText.color = targetColor;
            }
        }

        public void TriggerHologramPulse()
        {
            if (_pulseCoroutine != null) StopCoroutine(_pulseCoroutine);
            _pulseCoroutine = StartCoroutine(PulseRoutine());
        }

        public void OpenScienceExplanation(string conceptTitle, string explanation)
        {
            if (_scienceDrawer != null)
            {
                _scienceDrawer.SetActive(true);
                if (_scienceTitleText != null) _scienceTitleText.text = $"ORION Science Guide: {conceptTitle}";
                if (_scienceBodyText != null) _scienceBodyText.text = explanation;
            }
        }

        public void CloseScienceExplanation()
        {
            if (_scienceDrawer != null) _scienceDrawer.SetActive(false);
        }

        public void ToggleLogDrawer()
        {
            if (_logDrawer != null)
            {
                _logDrawer.SetActive(!_logDrawer.activeSelf);
                if (_logDrawer.activeSelf) UpdateLogHistoryDisplay();
            }
        }

        private void UpdateLogHistoryDisplay()
        {
            if (_logHistoryText != null)
            {
                _logHistoryText.text = string.Join("\n\n", _messageHistory);
            }
        }

        private IEnumerator DisplayBannerRoutine(ORIONMessage msg)
        {
            if (_messageBannerPanel != null)
            {
                _messageBannerPanel.SetActive(true);
                if (_messageTypeText != null) _messageTypeText.text = $"ORION // {msg.Subject}";
                if (_messageBodyText != null) _messageBodyText.text = msg.MessageText;

                Color accent = msg.Alert switch
                {
                    AlertLevel.Emergency => _emergencyRed,
                    AlertLevel.Caution => _cautionYellow,
                    _ => _nominalCyan
                };

                if (_bannerBorderImage != null) _bannerBorderImage.color = accent;
                if (_messageTypeText != null) _messageTypeText.color = accent;

                yield return new WaitForSecondsRealtime(msg.DisplayDuration > 0f ? msg.DisplayDuration : 4.0f);

                _messageBannerPanel.SetActive(false);
            }
            _bannerCoroutine = null;
        }

        private void StartPulsing()
        {
            if (_pulseCoroutine != null) StopCoroutine(_pulseCoroutine);
            _pulseCoroutine = StartCoroutine(IdlePulseRoutine());
        }

        private IEnumerator IdlePulseRoutine()
        {
            while (true)
            {
                float t = (Mathf.Sin(Time.unscaledTime * 3f) + 1f) * 0.5f;
                if (_pulsingOuterRing != null)
                {
                    float scale = Mathf.Lerp(1.0f, 1.15f, t);
                    _pulsingOuterRing.transform.localScale = new Vector3(scale, scale, 1f);
                }
                yield return null;
            }
        }

        private IEnumerator PulseRoutine()
        {
            float elapsed = 0f;
            float duration = 0.5f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float progress = elapsed / duration;
                float scale = Mathf.Lerp(1.0f, 1.35f, Mathf.Sin(progress * Mathf.PI));
                if (_pulsingOuterRing != null)
                {
                    _pulsingOuterRing.transform.localScale = new Vector3(scale, scale, 1f);
                }
                yield return null;
            }
            StartPulsing();
        }
    }
}
