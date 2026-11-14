using System;
using System.Collections.Generic;
using UnityEngine;
using Outpost.Core.Data;
using Outpost.Core.Events;
using Outpost.System1.Gameplay.Mission;
using Outpost.System1.Gameplay.Emergency;
using Outpost.System1.Gameplay.Resources;
using Outpost.System1.Gameplay.Education;
using Outpost.System1.Gameplay.Persistence;
using Outpost.System1.Gameplay.Core;

namespace Outpost.System1.Gameplay.Tests
{
    public static class MissionPhase4Tests
    {
        public static bool RunAllTests()
        {
            int passed = 0;
            int failed = 0;

            RunTest("Test 1: All 7 Chapters & 33 Objectives Configured", Test_01_AllSevenChaptersAndObjectivesConfigured, ref passed, ref failed);
            RunTest("Test 2: Chapter 1 Through 4 Progression & Apollo 13 Challenge", Test_02_Chapter1Through4ProgressionAndApollo13Challenge, ref passed, ref failed);
            RunTest("Test 3: Chapter 5 Multi-Pathway Strategic Repair Decisions", Test_03_Chapter5MultiPathwayRepairDecisions, ref passed, ref failed);
            RunTest("Test 4: Chapter 6 Recovery & State Persistence", Test_04_Chapter6RecoveryAndStatePersistence, ref passed, ref failed);
            RunTest("Test 5: Chapter 7 Ending, Final Reward & STEM Summary", Test_05_Chapter7EndingRewardAndSTEMSummary, ref passed, ref failed);
            RunTest("Test 6: Full End-to-End Mission Playthrough From Fresh Session", Test_06_EndToEndMissionPlaythroughFromFreshSession, ref passed, ref failed);

            Debug.Log($"\n======================================================\n[Phase4Tests] SUMMARY: {passed} PASSED, {failed} FAILED\n======================================================");
            return failed == 0;
        }

        private static void RunTest(string testName, Action testMethod, ref int passed, ref int failed)
        {
            try
            {
                testMethod();
                Debug.Log($"[Phase4Tests] [PASS] {testName}");
                passed++;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Phase4Tests] [FAIL] {testName}\nException: {ex.Message}\nStack: {ex.StackTrace}");
                failed++;
            }
        }

        private static void SetupTestEnvironment(out GameObject rootGo, out MissionManager mm, out EmergencyManager em, out ResourceManager rm, out EducationalManager edm, out SaveManager sm)
        {
            OutpostEventChannel.ClearAllSubscribers();

            rootGo = new GameObject("Phase4_Test_Environment");
            rm = rootGo.AddComponent<ResourceManager>();
            rm.EnsureInitialized();

            em = rootGo.AddComponent<EmergencyManager>();
            em.EnsureInitialized();

            edm = rootGo.AddComponent<EducationalManager>();
            edm.EnsureInitialized();

            sm = rootGo.AddComponent<SaveManager>();
            sm.EnsureInitialized();

            mm = rootGo.AddComponent<MissionManager>();
            mm.EnsureInitialized();

            // Set dummy player transform
            GameObject playerObj = new GameObject("PlayerAnchor");
            playerObj.transform.position = new Vector3(0f, 0f, 0f);
            playerObj.transform.SetParent(rootGo.transform);
            mm.SetPlayerTransform(playerObj.transform);
        }

        private static void CleanupTestEnvironment(GameObject rootGo)
        {
            OutpostEventChannel.ClearAllSubscribers();
            if (rootGo != null)
            {
                UnityEngine.Object.DestroyImmediate(rootGo);
            }
        }

        private static void Test_01_AllSevenChaptersAndObjectivesConfigured()
        {
            SetupTestEnvironment(out GameObject root, out MissionManager mm, out _, out _, out _, out _);
            try
            {
                Assert(mm != null, "MissionManager instance should not be null.");

                // Verify each of the 7 chapters exists with expected objective count
                MissionChapterModel ch1 = mm.GetChapter(MissionChapterId.Chapter1_Arrival);
                Assert(ch1 != null, "Chapter 1 should exist.");
                Assert(ch1.Objectives.Count == 5, $"Chapter 1 should have 5 objectives, found {ch1.Objectives.Count}");

                MissionChapterModel ch2 = mm.GetChapter(MissionChapterId.Chapter2_OutpostEstablishment);
                Assert(ch2 != null, "Chapter 2 should exist.");
                Assert(ch2.Objectives.Count == 7, $"Chapter 2 should have 7 objectives, found {ch2.Objectives.Count}");

                MissionChapterModel ch3 = mm.GetChapter(MissionChapterId.Chapter3_FirstWarning);
                Assert(ch3 != null, "Chapter 3 should exist.");
                Assert(ch3.Objectives.Count == 5, $"Chapter 3 should have 5 objectives, found {ch3.Objectives.Count}");

                MissionChapterModel ch4 = mm.GetChapter(MissionChapterId.Chapter4_LifeSupportCrisis);
                Assert(ch4 != null, "Chapter 4 should exist.");
                Assert(ch4.Objectives.Count == 5, $"Chapter 4 should have 5 objectives, found {ch4.Objectives.Count}");

                MissionChapterModel ch5 = mm.GetChapter(MissionChapterId.Chapter5_SaveTheOutpost);
                Assert(ch5 != null, "Chapter 5 should exist.");
                Assert(ch5.Objectives.Count == 3, $"Chapter 5 should have 3 objectives, found {ch5.Objectives.Count}");

                MissionChapterModel ch6 = mm.GetChapter(MissionChapterId.Chapter6_Recovery);
                Assert(ch6 != null, "Chapter 6 should exist.");
                Assert(ch6.Objectives.Count == 4, $"Chapter 6 should have 4 objectives, found {ch6.Objectives.Count}");

                MissionChapterModel ch7 = mm.GetChapter(MissionChapterId.Chapter7_Ending);
                Assert(ch7 != null, "Chapter 7 should exist.");
                Assert(ch7.Objectives.Count == 4, $"Chapter 7 should have 4 objectives, found {ch7.Objectives.Count}");

                // Verify map data markers are supplied to Computer 3
                var markers = mm.GetMapMarkers();
                Assert(markers != null && markers.Count >= 7, "Map markers should supply at least 7 station locations.");
                Assert(mm.PlayerWorldPosition == Vector3.zero, "Player world position should be readable.");
            }
            finally
            {
                CleanupTestEnvironment(root);
            }
        }

        private static void Test_02_Chapter1Through4ProgressionAndApollo13Challenge()
        {
            SetupTestEnvironment(out GameObject root, out MissionManager mm, out EmergencyManager em, out ResourceManager rm, out _, out _);
            try
            {
                mm.StartMission();
                Assert(mm.CurrentStatus == MissionStatus.Active, "Mission should be active.");
                Assert(mm.CurrentChapterId == MissionChapterId.Chapter1_Arrival, "Should start at Chapter 1.");

                // Progress through Chapter 1 (5 objectives)
                mm.CompleteObjective("obj_ch1_1");
                mm.CompleteObjective("obj_ch1_2");
                mm.CompleteObjective("obj_ch1_3");
                mm.CompleteObjective("obj_ch1_4");
                mm.CompleteObjective("obj_ch1_5");

                Assert(mm.CurrentChapterId == MissionChapterId.Chapter2_OutpostEstablishment,
                    $"Should advance to Chapter 2, current is {mm.CurrentChapterId}");

                // Progress through Chapter 2 (7 objectives)
                for (int i = 1; i <= 7; i++)
                {
                    mm.CompleteObjective($"obj_ch2_{i}");
                }

                Assert(mm.CurrentChapterId == MissionChapterId.Chapter3_FirstWarning,
                    $"Should advance to Chapter 3, current is {mm.CurrentChapterId}");

                // Progress through Chapter 3 (5 objectives)
                for (int i = 1; i <= 5; i++)
                {
                    mm.CompleteObjective($"obj_ch3_{i}");
                }

                Assert(mm.CurrentChapterId == MissionChapterId.Chapter4_LifeSupportCrisis,
                    $"Should advance to Chapter 4, current is {mm.CurrentChapterId}");

                // In Chapter 4, verify emergency is triggered
                Assert(em.IsEmergencyActiveByType(EmergencyType.CarbonDioxideAccumulation), "CO2 scrubber emergency should be active in Chapter 4.");

                // Complete first 3 objectives in Chapter 4
                mm.CompleteObjective("obj_ch4_1");
                mm.CompleteObjective("obj_ch4_2");
                mm.CompleteObjective("obj_ch4_3");

                Assert(mm.GetActiveObjective().ObjectiveId == "obj_ch4_4", "Should be on Apollo 13 improvisation objective.");

                // Trigger Apollo 13 puzzle completion event
                OutpostEventChannel.RaiseFiltrationChallengeCompleted(true);

                // obj_ch4_4 should now be completed, advancing to obj_ch4_5
                Assert(mm.GetActiveObjective().ObjectiveId == "obj_ch4_5",
                    $"Should advance to obj_ch4_5 after puzzle success, current is {mm.GetActiveObjective().ObjectiveId}");

                // Scrubber emergency should now be resolved
                Assert(!em.IsEmergencyActiveByType(EmergencyType.CarbonDioxideAccumulation), "CO2 emergency should be resolved by Apollo 13 adapter.");

                // Complete obj_ch4_5 to finish Chapter 4
                mm.CompleteObjective("obj_ch4_5");

                Assert(mm.CurrentChapterId == MissionChapterId.Chapter5_SaveTheOutpost,
                    $"Should advance to Chapter 5, current is {mm.CurrentChapterId}");
            }
            finally
            {
                CleanupTestEnvironment(root);
            }
        }

        private static void Test_03_Chapter5MultiPathwayRepairDecisions()
        {
            // Test Approach A: Astronaut EVA
            {
                SetupTestEnvironment(out GameObject rootA, out MissionManager mmA, out EmergencyManager emA, out ResourceManager rmA, out _, out _);
                try
                {
                    mmA.AdvanceToChapter(MissionChapterId.Chapter5_SaveTheOutpost);
                    emA.TriggerEmergency(EmergencyType.SolarPanelDamage);
                    emA.TriggerEmergency(EmergencyType.ElectricalDistributionFault);

                    mmA.CompleteObjective("obj_ch5_1");
                    Assert(mmA.GetActiveObjective().ObjectiveId == "obj_ch5_2", "Should be on repair decision objective.");

                    // Choose Astronaut EVA
                    mmA.SetRepairChoice(RepairChoice.AstronautEVA);

                    Assert(mmA.CurrentRepairChoice == RepairChoice.AstronautEVA, "Choice A should be recorded.");
                    Assert(mmA.GetActiveObjective().ObjectiveId == "obj_ch5_3", "Should advance to obj_ch5_3.");
                    Assert(!emA.IsEmergencyActiveByType(EmergencyType.SolarPanelDamage), "Solar emergency should be resolved.");
                    Assert(rmA.CurrentState.SolarGenerationKw == 55f, "EVA repair should restore full 55 kW power.");
                    Assert(rmA.CurrentState.CrewHealthPercent < 100f, "Crew health should reflect EVA radiation exposure.");

                    mmA.CompleteObjective("obj_ch5_3");
                    Assert(mmA.CurrentChapterId == MissionChapterId.Chapter6_Recovery, "Should advance to Chapter 6.");
                }
                finally
                {
                    CleanupTestEnvironment(rootA);
                }
            }

            // Test Approach B: Rover Robotic Arm
            {
                SetupTestEnvironment(out GameObject rootB, out MissionManager mmB, out EmergencyManager emB, out ResourceManager rmB, out _, out _);
                try
                {
                    mmB.AdvanceToChapter(MissionChapterId.Chapter5_SaveTheOutpost);
                    emB.TriggerEmergency(EmergencyType.SolarPanelDamage);
                    emB.TriggerEmergency(EmergencyType.ElectricalDistributionFault);

                    mmB.CompleteObjective("obj_ch5_1");
                    mmB.SetRepairChoice(RepairChoice.RoboticMaintenance);

                    Assert(mmB.CurrentRepairChoice == RepairChoice.RoboticMaintenance, "Choice B should be recorded.");
                    Assert(rmA_CrewHealthUnchanged(rmB), "Robotic arm repair should produce zero crew radiation exposure.");
                    Assert(rmB.CurrentState.SolarGenerationKw == 44f, "Robotic repair should restore 44 kW (80%) power.");

                    mmB.CompleteObjective("obj_ch5_3");
                    Assert(mmB.CurrentChapterId == MissionChapterId.Chapter6_Recovery, "Should advance to Chapter 6.");
                }
                finally
                {
                    CleanupTestEnvironment(rootB);
                }
            }

            // Test Approach C: Conserve Power & Delay
            {
                SetupTestEnvironment(out GameObject rootC, out MissionManager mmC, out EmergencyManager emC, out ResourceManager rmC, out _, out _);
                try
                {
                    mmC.AdvanceToChapter(MissionChapterId.Chapter5_SaveTheOutpost);
                    emC.TriggerEmergency(EmergencyType.SolarPanelDamage);
                    emC.TriggerEmergency(EmergencyType.ElectricalDistributionFault);

                    mmC.CompleteObjective("obj_ch5_1");
                    mmC.SetRepairChoice(RepairChoice.ConservePowerDelay);

                    Assert(mmC.CurrentRepairChoice == RepairChoice.ConservePowerDelay, "Choice C should be recorded.");
                    Assert(rmC.CurrentState.SolarGenerationKw == 49.5f, "Delayed repair should restore 49.5 kW (90%) power.");
                    Assert(rmA_CrewHealthUnchanged(rmC), "Conserve power repair should produce zero radiation exposure.");

                    mmC.CompleteObjective("obj_ch5_3");
                    Assert(mmC.CurrentChapterId == MissionChapterId.Chapter6_Recovery, "Should advance to Chapter 6.");
                }
                finally
                {
                    CleanupTestEnvironment(rootC);
                }
            }
        }

        private static bool rmA_CrewHealthUnchanged(ResourceManager rm)
        {
            return rm.CurrentState.CrewHealthPercent == 100f;
        }

        private static void Test_04_Chapter6RecoveryAndStatePersistence()
        {
            SetupTestEnvironment(out GameObject root, out MissionManager mm, out _, out ResourceManager rm, out _, out SaveManager sm);
            try
            {
                sm.DeleteSave();

                mm.AdvanceToChapter(MissionChapterId.Chapter6_Recovery);
                Assert(mm.CurrentChapterId == MissionChapterId.Chapter6_Recovery, "Should be at Chapter 6.");

                mm.CompleteObjective("obj_ch6_1");
                mm.CompleteObjective("obj_ch6_2");
                mm.CompleteObjective("obj_ch6_3");

                Assert(mm.GetActiveObjective().ObjectiveId == "obj_ch6_4", "Should be on log outpost status objective.");

                // Complete obj_ch6_4 which auto-saves
                mm.CompleteObjective("obj_ch6_4");

                // Verify save data exists
                OutpostSaveModel saveModel = sm.LoadGameState();
                Assert(saveModel != null, "Save data should exist after Chapter 6 save.");
                Assert(saveModel.ChapterId == (int)MissionChapterId.Chapter7_Ending,
                    $"Saved chapter should be Chapter 7 upon transition, found {saveModel.ChapterId}");
                Assert(saveModel.ResourceState != null, "Resource snapshot should be persisted.");

                Assert(mm.CurrentChapterId == MissionChapterId.Chapter7_Ending,
                    $"Should advance to Chapter 7, current is {mm.CurrentChapterId}");
            }
            finally
            {
                sm.DeleteSave();
                CleanupTestEnvironment(root);
            }
        }

        private static void Test_05_Chapter7EndingRewardAndSTEMSummary()
        {
            SetupTestEnvironment(out GameObject root, out MissionManager mm, out _, out _, out EducationalManager edm, out _);
            try
            {
                bool rewardReceived = false;
                string rewardTitle = string.Empty;
                bool creditsReceived = false;
                bool summaryReceived = false;
                MissionSummaryReport summaryReport = null;

                Action<string, string> rewardHandler = (title, desc) =>
                {
                    rewardReceived = true;
                    rewardTitle = title;
                };

                Action<string> creditsHandler = (text) =>
                {
                    creditsReceived = true;
                };

                Action<MissionSummaryReport> summaryHandler = (report) =>
                {
                    summaryReceived = true;
                    summaryReport = report;
                };

                OutpostEventChannel.MissionRewardAwarded += rewardHandler;
                OutpostEventChannel.CreditsTriggered += creditsHandler;
                OutpostEventChannel.MissionSummaryPrompted += summaryHandler;

                try
                {
                    mm.AdvanceToChapter(MissionChapterId.Chapter7_Ending);
                    Assert(mm.CurrentChapterId == MissionChapterId.Chapter7_Ending, "Should be at Chapter 7.");

                    mm.CompleteObjective("obj_ch7_1");
                    mm.CompleteObjective("obj_ch7_2");
                    mm.CompleteObjective("obj_ch7_3");
                    mm.CompleteObjective("obj_ch7_4");

                    // Completing obj_ch7_4 finishes Chapter 7 and completes the entire mission
                    Assert(mm.CurrentStatus == MissionStatus.Completed,
                        $"Mission status should be Completed, found {mm.CurrentStatus}");

                    Assert(rewardReceived, "Lunar Pioneer Ribbon reward should be awarded.");
                    Assert(rewardTitle.Contains("Lunar Pioneer"), $"Reward title should contain 'Lunar Pioneer', found '{rewardTitle}'");
                    Assert(creditsReceived, "Credits event should be triggered.");
                    Assert(summaryReceived, "Educational STEM summary should be prompted.");
                    Assert(summaryReport != null && summaryReport.StemLearnings.Count >= 5,
                        $"Educational STEM learnings should contain 5 core concepts, found {summaryReport?.StemLearnings.Count ?? 0}");
                }
                finally
                {
                    OutpostEventChannel.MissionRewardAwarded -= rewardHandler;
                    OutpostEventChannel.CreditsTriggered -= creditsHandler;
                    OutpostEventChannel.MissionSummaryPrompted -= summaryHandler;
                }
            }
            finally
            {
                CleanupTestEnvironment(root);
            }
        }

        private static void Test_06_EndToEndMissionPlaythroughFromFreshSession()
        {
            SetupTestEnvironment(out GameObject root, out MissionManager mm, out EmergencyManager em, out ResourceManager rm, out EducationalManager edm, out SaveManager sm);
            try
            {
                sm.DeleteSave();

                // Boot fresh session
                mm.StartMission();
                Assert(mm.CurrentStatus == MissionStatus.Active, "Mission should be active.");
                Assert(mm.CurrentChapterId == MissionChapterId.Chapter1_Arrival, "Should start at Chapter 1.");

                // CHAPTER 1 (5 objectives)
                mm.CompleteObjective("obj_ch1_1");
                mm.CompleteObjective("obj_ch1_2");
                mm.CompleteObjective("obj_ch1_3");
                mm.CompleteObjective("obj_ch1_4");
                mm.CompleteObjective("obj_ch1_5");
                Assert(mm.CurrentChapterId == MissionChapterId.Chapter2_OutpostEstablishment, "Should be Chapter 2.");

                // CHAPTER 2 (7 objectives)
                for (int i = 1; i <= 7; i++) mm.CompleteObjective($"obj_ch2_{i}");
                Assert(mm.CurrentChapterId == MissionChapterId.Chapter3_FirstWarning, "Should be Chapter 3.");

                // CHAPTER 3 (5 objectives)
                for (int i = 1; i <= 5; i++) mm.CompleteObjective($"obj_ch3_{i}");
                Assert(mm.CurrentChapterId == MissionChapterId.Chapter4_LifeSupportCrisis, "Should be Chapter 4.");

                // CHAPTER 4 (5 objectives including Apollo 13 minigame)
                mm.CompleteObjective("obj_ch4_1");
                mm.CompleteObjective("obj_ch4_2");
                mm.CompleteObjective("obj_ch4_3");
                OutpostEventChannel.RaiseFiltrationChallengeCompleted(true);
                mm.CompleteObjective("obj_ch4_5");
                Assert(mm.CurrentChapterId == MissionChapterId.Chapter5_SaveTheOutpost, "Should be Chapter 5.");

                // CHAPTER 5 (3 objectives including strategic repair choice)
                mm.CompleteObjective("obj_ch5_1");
                mm.SetRepairChoice(RepairChoice.RoboticMaintenance);
                mm.CompleteObjective("obj_ch5_3");
                Assert(mm.CurrentChapterId == MissionChapterId.Chapter6_Recovery, "Should be Chapter 6.");

                // CHAPTER 6 (4 objectives including save)
                for (int i = 1; i <= 4; i++) mm.CompleteObjective($"obj_ch6_{i}");
                Assert(mm.CurrentChapterId == MissionChapterId.Chapter7_Ending, "Should be Chapter 7.");

                // CHAPTER 7 (4 objectives concluding mission)
                for (int i = 1; i <= 4; i++) mm.CompleteObjective($"obj_ch7_{i}");

                Assert(mm.CurrentStatus == MissionStatus.Completed, "End-to-end mission must finish in Completed status.");
            }
            finally
            {
                sm.DeleteSave();
                CleanupTestEnvironment(root);
            }
        }

        private static void Assert(bool condition, string message)
        {
            if (!condition)
            {
                throw new Exception($"ASSERTION FAILED: {message}");
            }
        }
    }
}
