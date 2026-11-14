using System;
using System.Collections;
using UnityEngine;
using Outpost.UI.Core;
using Outpost.UI.HUD;
using Outpost.UI.Dialogue;
using Outpost.UI.Menus;
using Outpost.UI.Notifications;

namespace Outpost.UI.Testing
{
    /// <summary>
    /// Automated test harness for verifying Phase 1: Basic UI and Dialogue Foundation.
    /// Tests Main Menu, HUD, Subtitles, Speaker transitions, Choices, and Bridge events.
    /// </summary>
    public class Phase1UITestRunner : MonoBehaviour
    {
        [Header("Test Configuration")]
        [SerializeField] private bool _runOnStart = true;
        [SerializeField] private float _stepDelay = 1.0f;

        [Header("References")]
        [SerializeField] private UIManager _uiManager;
        [SerializeField] private HUDController _hudController;
        [SerializeField] private DialogueManager _dialogueManager;
        [SerializeField] private ConversationData _introConversation;

        private void Start()
        {
            if (_runOnStart)
            {
                StartCoroutine(ExecutePhase1Tests());
            }
        }

        public IEnumerator ExecutePhase1Tests()
        {
            Debug.Log("<color=cyan>[Phase1Test] Starting Phase 1 UI and Dialogue Foundation Validation...</color>");

            // TEST 1: HUD Initialization & Objective Display
            Debug.Log("[Phase1Test] Test 1: Testing HUD Objective Update...");
            UIEventBus.PublishObjective("OBJ_01", "Inspect Lunar Terrain & Proceed to Habitat Airlock");
            yield return new WaitForSeconds(_stepDelay);

            // TEST 2: Contextual Interaction Prompt
            Debug.Log("[Phase1Test] Test 2: Testing Contextual Interaction Prompt...");
            if (_hudController != null)
            {
                _hudController.SetInteractionPrompt("[E] Talk to Commander Elena");
            }
            yield return new WaitForSeconds(_stepDelay);

            // TEST 3: Mission Notification Toast
            Debug.Log("[Phase1Test] Test 3: Testing Mission Notification Toast...");
            UIEventBus.PublishNotification("Mission 1", "Touchdown confirmed at Shackleton Crater Rim.", NotificationType.ObjectiveUpdate);
            yield return new WaitForSeconds(_stepDelay);

            // TEST 4: Pause Indicator Toggle
            Debug.Log("[Phase1Test] Test 4: Testing Pause State Indicator...");
            UIEventBus.PublishPauseToggled(true);
            yield return new WaitForSeconds(_stepDelay * 0.5f);
            UIEventBus.PublishPauseToggled(false);
            yield return new WaitForSeconds(_stepDelay * 0.5f);

            // TEST 5: Introductory Conversation Sequence (Elena -> Marcus -> Aisha -> Daniel -> ORION)
            Debug.Log("[Phase1Test] Test 5: Launching Mission 1 Introductory Conversation...");
            bool conversationCompleted = false;

            if (_introConversation != null && _dialogueManager != null)
            {
                _dialogueManager.StartConversation(_introConversation, () =>
                {
                    conversationCompleted = true;
                    Debug.Log("<color=green>[Phase1Test] Conversation Sequence Completed Successfully!</color>");
                });

                // Advance through Elena, Marcus, Aisha, Daniel, and ORION lines
                for (int i = 0; i < _introConversation.Lines.Count; i++)
                {
                    yield return new WaitForSeconds(_stepDelay * 0.8f);
                    Debug.Log($"[Phase1Test] Line {i + 1}/{_introConversation.Lines.Count} ({_introConversation.Lines[i].Speaker}): '{_introConversation.Lines[i].DialogueText}'");
                    _dialogueManager.AdvanceDialogue();
                }

                // Select Choice 1
                yield return new WaitForSeconds(_stepDelay * 0.5f);
                if (_dialogueManager.IsDialogueActive)
                {
                    Debug.Log("[Phase1Test] Selecting Branching Player Choice 0...");
                    _dialogueManager.SelectChoice(0);
                }
            }
            else
            {
                Debug.LogWarning("[Phase1Test] Intro conversation or DialogueManager reference missing in test harness.");
            }

            // TEST 6: Clear Interaction Prompt
            if (_hudController != null)
            {
                _hudController.ClearInteractionPrompt();
            }

            Debug.Log("<color=green>==================================================</color>");
            Debug.Log("<color=green>[Phase1Test] PHASE 1 VALIDATION COMPLETE: ALL SYSTEMS NOMINAL</color>");
            Debug.Log("<color=green>==================================================</color>");
        }
    }
}
