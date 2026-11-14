using System;
using UnityEngine;

namespace Outpost.UI.Core
{
    /// <summary>
    /// Event contract for Computer 1 (Gameplay / Missions) to drive System 3 UI progression.
    /// Fully decouples mission code from UI presentation objects.
    /// </summary>
    public interface IMissionProgressionBridge
    {
        /// <summary>
        /// Updates the primary mission title and active objective displayed on the HUD.
        /// </summary>
        void SetMissionObjective(string objectiveId, string title, string description);

        /// <summary>
        /// Displays contextual interaction prompts when the player is near an interactable object.
        /// e.g. "[E] Talk to Commander Elena" or "[E] Inspect Solar Conduit".
        /// </summary>
        void ShowInteractionPrompt(string promptText);

        /// <summary>
        /// Clears the interaction prompt when the player moves away.
        /// </summary>
        void ClearInteractionPrompt();

        /// <summary>
        /// Starts a mission dialogue sequence by unique conversation ID.
        /// </summary>
        void TriggerConversation(string conversationId, Action onCompleted = null);

        /// <summary>
        /// Dispatches a high-priority on-screen mission notification toast.
        /// </summary>
        void PostNotification(string title, string message, NotificationType type);

        /// <summary>
        /// Sets ORION's visual and audio alert status (Nominal, Caution, Emergency).
        /// </summary>
        void SetStationAlert(AlertLevel level, string reason);
    }

    /// <summary>
    /// Default bridge implementation forwarding calls into UIEventBus and UIManager.
    /// </summary>
    public class MissionProgressionBridge : MonoBehaviour, IMissionProgressionBridge
    {
        public static MissionProgressionBridge Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void SetMissionObjective(string objectiveId, string title, string description)
        {
            UIEventBus.PublishObjective(objectiveId, description);
        }

        public void ShowInteractionPrompt(string promptText)
        {
            UIManager.Instance?.GetComponentInChildren<HUD.HUDController>()?.SetInteractionPrompt(promptText);
        }

        public void ClearInteractionPrompt()
        {
            UIManager.Instance?.GetComponentInChildren<HUD.HUDController>()?.ClearInteractionPrompt();
        }

        public void TriggerConversation(string conversationId, Action onCompleted = null)
        {
            UIManager.Instance?.GetComponentInChildren<Dialogue.DialogueManager>()?.StartConversationById(conversationId, onCompleted);
        }

        public void PostNotification(string title, string message, NotificationType type)
        {
            UIEventBus.PublishNotification(title, message, type);
        }

        public void SetStationAlert(AlertLevel level, string reason)
        {
            UIEventBus.PublishAlertLevel(level, reason);
        }
    }
}
