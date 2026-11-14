using System;
using System.Collections.Generic;
using UnityEngine;
using Outpost.Core.Data;
using Outpost.Core.Events;
using Outpost.System1.Gameplay.Emergency;
using Outpost.System1.Gameplay.Resources;
using Outpost.System1.Gameplay.Equipment;
using Outpost.System1.Gameplay.Core;

namespace Outpost.System1.Gameplay.Tests
{
    /// <summary>
    /// Automated test suite for Phase 3: Emergencies, Apollo 13 Filtration Puzzle,
    /// Strategic Repair Decision Support, and Contextual Character Interactions.
    /// </summary>
    public class EmergencyPhase3Tests : MonoBehaviour
    {
        [SerializeField] private bool runOnStart = false;

        private void Start()
        {
            if (runOnStart)
            {
                RunAllTests();
            }
        }

        public static bool RunAllTests()
        {
            Debug.Log("==================================================");
            Debug.Log("STARTING PHASE 3 EMERGENCY & REPAIR TEST SUITE");
            Debug.Log("==================================================");

            int passed = 0;
            int total = 4;

            if (TestAllEightEmergencies()) passed++;
            if (TestApollo13FiltrationPuzzle()) passed++;
            if (TestStrategicRepairPathways()) passed++;
            if (TestCharacterInteractionEvents()) passed++;

            Debug.Log("==================================================");
            Debug.Log($"PHASE 3 TEST RESULTS: {passed}/{total} TESTS PASSED");
            Debug.Log("==================================================");

            return passed == total;
        }

        private static bool TestAllEightEmergencies()
        {
            Debug.Log("[TEST 1] Testing All 8 Mission 1 Emergencies (Triggers, Severity, Consequences, Recovery)...");

            GameObject rmGo = new GameObject("Test_RM");
            ResourceManager rm = rmGo.AddComponent<ResourceManager>();
            rm.EnsureInitialized();

            GameObject emGo = new GameObject("Test_EM");
            EmergencyManager em = emGo.AddComponent<EmergencyManager>();
            em.EnsureInitialized();

            var allEmergencies = em.GetAllEmergencies();
            Assert(allEmergencies.Count == 8, $"Expected exactly 8 emergencies, found {allEmergencies.Count}");

            EmergencyType[] types = (EmergencyType[])Enum.GetValues(typeof(EmergencyType));
            foreach (var type in types)
            {
                var model = em.GetEmergency(type);
                Assert(model != null, $"Emergency model for {type} should exist");
                Assert(!string.IsNullOrEmpty(model.Title), $"Title for {type} should not be empty");
                Assert(!string.IsNullOrEmpty(model.PlayerObjective), $"PlayerObjective for {type} should not be empty");
                Assert(!string.IsNullOrEmpty(model.RepairCondition), $"RepairCondition for {type} should not be empty");
                Assert(!string.IsNullOrEmpty(model.RecoveryState), $"RecoveryState for {type} should not be empty");

                // Trigger emergency
                em.TriggerEmergency(type);
                Assert(model.IsActive, $"Emergency {type} should be active after trigger");

                // Resolve emergency
                em.ResolveEmergency(type);
                Assert(model.IsResolved, $"Emergency {type} should be resolved after resolve call");
            }

            UnityEngine.Object.DestroyImmediate(emGo);
            UnityEngine.Object.DestroyImmediate(rmGo);

            Debug.Log("[TEST 1 PASSED] All 8 emergencies verified.");
            return true;
        }

        private static bool TestApollo13FiltrationPuzzle()
        {
            Debug.Log("[TEST 2] Testing Apollo 13-Inspired Filtration Repair Challenge...");

            GameObject fcGo = new GameObject("Test_Filtration");
            FiltrationChallengeController fc = fcGo.AddComponent<FiltrationChallengeController>();
            fc.EnsureInitialized();

            // 1. Inspect Problem
            string diag = fc.InspectProblem();
            Assert(!string.IsNullOrEmpty(diag), "Diagnostic report should not be empty");
            Assert(diag.Contains("square") && diag.Contains("round"), "Diagnostic report should describe the square-to-round mismatch");

            // 2. Examine Components
            var components = fc.GetAvailableComponents();
            Assert(components.Count >= 4, "Should have at least 4 components available");

            fc.ResetPuzzle();

            // 3. Test Incomplete Solution: missing tape
            fc.SelectComponent("canister", true);
            fc.SelectComponent("cardboard", true);
            fc.SelectComponent("hose", true);
            fc.SelectComponent("tape", false);

            bool successWithoutTape = fc.SubmitSolution(out string feedbackTape);
            Assert(!successWithoutTape, "Solution should fail when duct tape is missing");
            Assert(feedbackTape.Contains("AIR LEAK") || feedbackTape.Contains("Tape"), "Feedback should indicate air leak from missing tape");

            // 4. Test Incompatible Solution: with copper wire
            fc.SelectComponent("tape", true);
            fc.SelectComponent("copper_wire", true);

            bool successWithWire = fc.SubmitSolution(out string feedbackWire);
            Assert(!successWithWire, "Solution should fail when incompatible parts are included");
            Assert(feedbackWire.Contains("INCOMPATIBLE"), "Feedback should indicate incompatible parts");

            // 5. Test Valid Solution: Exactly Canister, Cardboard, Hose, Tape
            fc.SelectComponent("copper_wire", false);
            bool successValid = fc.SubmitSolution(out string feedbackValid);
            Assert(successValid, "Solution should succeed when exact 4 compatible parts are assembled");
            Assert(fc.IsChallengeResolved, "Challenge state should be resolved");
            Assert(feedbackValid.Contains("SUCCESS"), "Feedback should confirm airtight seal success");

            UnityEngine.Object.DestroyImmediate(fcGo);

            Debug.Log("[TEST 2 PASSED] Apollo 13 filtration puzzle verified.");
            return true;
        }

        private static bool TestStrategicRepairPathways()
        {
            Debug.Log("[TEST 3] Testing 3 Strategic Repair Decision Pathways (Approach A, B, and C)...");

            GameObject rmGo = new GameObject("Test_RM_Decisions");
            ResourceManager rm = rmGo.AddComponent<ResourceManager>();
            rm.EnsureInitialized();

            GameObject emGo = new GameObject("Test_EM_Decisions");
            EmergencyManager em = emGo.AddComponent<EmergencyManager>();
            em.EnsureInitialized();

            // Approach A: Marcus Astronaut EVA
            var outcomeA = em.EvaluateRepairChoice(RepairChoice.AstronautEVA);
            Assert(outcomeA != null, "Outcome A should not be null");
            Assert(outcomeA.RestoredPowerKw == 55.0f, "Approach A should restore 55.0 kW full power");
            Assert(outcomeA.EquipmentConditionPercent == 100.0f, "Approach A should restore 100% equipment condition");
            Assert(outcomeA.CrewHealthImpactPercent < 0f, "Approach A should have radiation health impact on crew");
            Assert(outcomeA.RepairDurationSeconds == 45.0f, "Approach A should be fast (45 seconds)");

            // Approach B: Robotic Maintenance
            var outcomeB = em.EvaluateRepairChoice(RepairChoice.RoboticMaintenance);
            Assert(outcomeB != null, "Outcome B should not be null");
            Assert(outcomeB.RestoredPowerKw == 44.0f, "Approach B should restore 44.0 kW (80%)");
            Assert(outcomeB.EquipmentConditionPercent == 80.0f, "Approach B should have 80% equipment condition");
            Assert(outcomeB.CrewHealthImpactPercent == 0f, "Approach B should have ZERO crew health impact");
            Assert(outcomeB.RepairDurationSeconds == 90.0f, "Approach B should have moderate duration (90 seconds)");

            // Approach C: Conserve Power & Delay
            var outcomeC = em.EvaluateRepairChoice(RepairChoice.ConservePowerDelay);
            Assert(outcomeC != null, "Outcome C should not be null");
            Assert(outcomeC.RestoredPowerKw == 49.5f, "Approach C should restore 49.5 kW (90%)");
            Assert(outcomeC.CrewHealthImpactPercent == 0f, "Approach C should have ZERO radiation exposure");
            Assert(outcomeC.BatteryCostKwh > outcomeA.BatteryCostKwh, "Approach C should consume more battery during delay");
            Assert(outcomeC.RepairDurationSeconds == 180.0f, "Approach C should be slow (180 seconds)");

            // Test applying Approach A
            float initialHealth = rm.CurrentState.CrewHealthPercent;
            em.ApplyRepairChoice(RepairChoice.AstronautEVA);
            Assert(rm.CurrentState.CrewHealthPercent < initialHealth, "Crew health should reflect EVA radiation exposure");
            Assert(Mathf.Approximately(rm.SolarPowerGeneratedKw, 55.0f), "Solar power should be 55 kW after Approach A");

            UnityEngine.Object.DestroyImmediate(emGo);
            UnityEngine.Object.DestroyImmediate(rmGo);

            Debug.Log("[TEST 3 PASSED] 3 strategic repair pathways verified.");
            return true;
        }

        private static bool TestCharacterInteractionEvents()
        {
            Debug.Log("[TEST 4] Testing Character Interaction Conversations on Actual Gameplay Events...");

            GameObject ctGo = new GameObject("Test_CT");
            ContextualConversationTrigger ct = ctGo.AddComponent<ContextualConversationTrigger>();
            ct.EnsureInitialized();

            bool conversationReceived = false;
            ConversationSequence receivedSeq = null;

            Action<ConversationSequence> handler = (seq) =>
            {
                conversationReceived = true;
                receivedSeq = seq;
            };

            OutpostEventChannel.ConversationSequencePrompted += handler;

            // 1. Marcus requests Elena's approval before external repair
            conversationReceived = false;
            ct.TriggerEvaApprovalConversation(true);
            Assert(conversationReceived, "Marcus EVA approval conversation should be dispatched");
            Assert(receivedSeq.ConversationId == "conv_eva_approval", "Conversation ID should be conv_eva_approval");
            Assert(receivedSeq.Lines.Count >= 4, "Conversation should feature multi-astronaut exchange");

            // 2. Aisha reports scientific observations
            conversationReceived = false;
            ct.TriggerScientificReportConversation(true);
            Assert(conversationReceived, "Aisha scientific report conversation should be dispatched");
            Assert(receivedSeq.ConversationId == "conv_aisha_science_report", "Conversation ID should be conv_aisha_science_report");

            // 3. Daniel warns crew about health risks
            conversationReceived = false;
            ct.TriggerHealthWarningConversation(true);
            Assert(conversationReceived, "Daniel health warning conversation should be dispatched");
            Assert(receivedSeq.ConversationId == "conv_daniel_health_warning", "Conversation ID should be conv_daniel_health_warning");

            // 4. Elena coordinates the repair
            conversationReceived = false;
            ct.TriggerRepairCoordinationConversation(true);
            Assert(conversationReceived, "Elena repair coordination conversation should be dispatched");
            Assert(receivedSeq.ConversationId == "conv_elena_repair_coordination", "Conversation ID should be conv_elena_repair_coordination");

            OutpostEventChannel.ConversationSequencePrompted -= handler;

            UnityEngine.Object.DestroyImmediate(ctGo);

            Debug.Log("[TEST 4 PASSED] Character interaction conversations verified.");
            return true;
        }

        private static void Assert(bool condition, string failureMessage)
        {
            if (!condition)
            {
                throw new Exception($"ASSERTION FAILED: {failureMessage}");
            }
        }
    }
}
