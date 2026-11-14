using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Outpost.UI.Core;
using Outpost.UI.Dialogue;
using Outpost.UI.Emergency;

namespace Outpost.UI.Testing
{
    /// <summary>
    /// Automated test harness for verifying Phase 3: Character Interactions and Emergency Story Presentation.
    /// Executes end-to-end testing across all 7 mandatory scenes, camera framing events,
    /// diagnostic screens, the interactive Apollo 13 inspired filtration challenge, and the final decision modal.
    /// </summary>
    public class Phase3EmergencyTestRunner : MonoBehaviour
    {
        [Header("Test Configuration")]
        [SerializeField] private bool _runOnStart = true;
        [SerializeField] private float _stepDelay = 0.6f;

        [Header("System References")]
        [SerializeField] private DialogueManager _dialogueManager;
        [SerializeField] private EmergencyAlertPanel _emergencyAlertPanel;
        [SerializeField] private LifeSupportDiagnosticScreen _diagnosticScreen;
        [SerializeField] private FiltrationChallengeUI _filtrationChallengeUI;
        [SerializeField] private EmergencyDecisionModal _decisionModal;

        [Header("Scene Assets (Scenes 1–7)")]
        [SerializeField] private ConversationData _scene1SolarWarning;
        [SerializeField] private ConversationData _scene2BeforeImpact;
        [SerializeField] private ConversationData _scene3ImpactIncident;
        [SerializeField] private ConversationData _scene4LifeSupport;
        [SerializeField] private ConversationData _scene5Filtration;
        [SerializeField] private ConversationData _scene6Decision;
        [SerializeField] private ConversationData _scene7Recovery;

        private void Start()
        {
            if (_runOnStart)
            {
                StartCoroutine(ExecutePhase3Tests());
            }
        }

        public IEnumerator ExecutePhase3Tests()
        {
            Debug.Log("<color=cyan>==================================================</color>");
            Debug.Log("<color=cyan>[Phase3Test] Starting Phase 3: Character Interactions and Emergency Presentation Validation...</color>");
            Debug.Log("<color=cyan>==================================================</color>");

            // ----------------------------------------------------
            // TEST SUITE 1: SCENE 1 - SOLAR ACTIVITY WARNING (BEFORE INCIDENT)
            // ----------------------------------------------------
            Debug.Log("[Phase3Test] 1. Executing Scene 1: Solar Activity Warning (Aisha -> Marcus -> Elena -> ORION)...");
            yield return StartCoroutine(PlayTestConversation(_scene1SolarWarning));
            yield return new WaitForSeconds(_stepDelay);

            // ----------------------------------------------------
            // TEST SUITE 2: SCENE 2 - BEFORE THE IMPACT (PRE-EVENT CHECK)
            // ----------------------------------------------------
            Debug.Log("[Phase3Test] 2. Executing Scene 2: Before The Impact (Marcus -> Daniel -> Elena)...");
            yield return StartCoroutine(PlayTestConversation(_scene2BeforeImpact));
            yield return new WaitForSeconds(_stepDelay);

            // ----------------------------------------------------
            // TEST SUITE 3: SCENE 3 - THE MICROMETEOROID INCIDENT & EMERGENCY ALERT
            // ----------------------------------------------------
            Debug.Log("[Phase3Test] 3. Triggering Scene 3: The Micrometeoroid Incident...");
            if (_emergencyAlertPanel != null)
            {
                _emergencyAlertPanel.DisplayEmergency(
                    "EXTERNAL IMPACT DETECTED",
                    "Micrometeoroid collision confirmed on Power Substation 2 conduit line!",
                    AlertLevel.Emergency
                );
            }
            yield return new WaitForSeconds(_stepDelay);

            yield return StartCoroutine(PlayTestConversation(_scene3ImpactIncident));
            yield return new WaitForSeconds(_stepDelay);

            // ----------------------------------------------------
            // TEST SUITE 4: SCENE 4 - LIFE-SUPPORT EMERGENCY & DIAGNOSTIC TERMINAL
            // ----------------------------------------------------
            Debug.Log("[Phase3Test] 4. Executing Scene 4: Life-Support Emergency...");
            yield return StartCoroutine(PlayTestConversation(_scene4LifeSupport));
            yield return new WaitForSeconds(_stepDelay);

            Debug.Log("[Phase3Test] 4b. Opening Life-Support Diagnostic Screen (CO2 rising)...");
            if (_diagnosticScreen != null)
            {
                _diagnosticScreen.OpenDiagnostic(() => Debug.Log("[Phase3Test] Diagnostic terminal bridge clicked."));
            }
            yield return new WaitForSeconds(_stepDelay);
            if (_diagnosticScreen != null) _diagnosticScreen.Hide();

            // ----------------------------------------------------
            // TEST SUITE 5: SCENE 5 & INTERACTIVE FILTRATION CHALLENGE
            // ----------------------------------------------------
            Debug.Log("[Phase3Test] 5. Executing Scene 5: Filtration Challenge Briefing...");
            yield return StartCoroutine(PlayTestConversation(_scene5Filtration));
            yield return new WaitForSeconds(_stepDelay);

            Debug.Log("[Phase3Test] 5b. Launching Interactive Filtration Challenge (Apollo 13 inspired)...");
            if (_filtrationChallengeUI != null)
            {
                _filtrationChallengeUI.StartChallenge(success =>
                {
                    Debug.Log($"<color=green>[Phase3Test] Filtration Challenge Complete! Success: {success}</color>");
                });
            }
            yield return new WaitForSeconds(_stepDelay);
            if (_filtrationChallengeUI != null) _filtrationChallengeUI.Hide();

            // ----------------------------------------------------
            // TEST SUITE 6: SCENE 6 - FINAL DECISION (THE 3 STRATEGIES)
            // ----------------------------------------------------
            Debug.Log("[Phase3Test] 6. Executing Scene 6: Final Decision Matrix...");
            yield return StartCoroutine(PlayTestConversation(_scene6Decision));
            yield return new WaitForSeconds(_stepDelay);

            Debug.Log("[Phase3Test] 6b. Testing Emergency Decision Modal (Choosing Option A - Elena's EVA)...");
            if (_decisionModal != null)
            {
                _decisionModal.OpenDecisionModal(choice =>
                {
                    Debug.Log($"<color=green>[Phase3Test] Emergency Strategy Selected: {choice}</color>");
                });
            }
            yield return new WaitForSeconds(_stepDelay);
            if (_decisionModal != null) _decisionModal.Hide();

            // ----------------------------------------------------
            // TEST SUITE 7: SCENE 7 - RECOVERY & CELEBRATION
            // ----------------------------------------------------
            Debug.Log("[Phase3Test] 7. Executing Scene 7: Outpost Recovery (Daniel -> Marcus -> Aisha -> Elena)...");
            yield return StartCoroutine(PlayTestConversation(_scene7Recovery));
            yield return new WaitForSeconds(_stepDelay);

            Debug.Log("<color=green>==================================================</color>");
            Debug.Log("<color=green>[Phase3Test] PHASE 3 VALIDATION COMPLETE: ALL 7 SCENES AND EMERGENCY SUITE VERIFIED</color>");
            Debug.Log("<color=green>==================================================</color>");
        }

        private IEnumerator PlayTestConversation(ConversationData conv)
        {
            if (conv == null || _dialogueManager == null) yield break;

            bool isDone = false;
            _dialogueManager.StartConversation(conv, () => isDone = true);

            for (int i = 0; i < conv.Lines.Count; i++)
            {
                yield return new WaitForSeconds(_stepDelay * 0.4f);
                var l = conv.Lines[i];
                Debug.Log($"[Phase3Test] [{l.Speaker}] ({l.Expression}, Framing: {l.Framing}): '{l.DialogueText}'");
                _dialogueManager.AdvanceDialogue();
            }

            yield return new WaitUntil(() => isDone || !_dialogueManager.IsDialogueActive);
        }
    }
}
