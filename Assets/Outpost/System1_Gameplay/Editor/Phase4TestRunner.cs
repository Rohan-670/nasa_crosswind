using UnityEditor;
using UnityEngine;
using Outpost.System1.Gameplay.Tests;

namespace Outpost.System1.Gameplay.Editor
{
    public static class Phase4TestRunner
    {
        public static void RunTests()
        {
            Debug.Log("[Phase4TestRunner] Executing Phase 4 Complete Mission 1 Gameplay Tests...");
            try
            {
                bool success = MissionPhase4Tests.RunAllTests();
                if (success)
                {
                    Debug.Log("[Phase4TestRunner] RESULT: ALL PHASE 4 TESTS PASSED SUCCESSFULLY!");
                    EditorApplication.Exit(0);
                }
                else
                {
                    Debug.LogError("[Phase4TestRunner] RESULT: ONE OR MORE TESTS FAILED.");
                    EditorApplication.Exit(1);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[Phase4TestRunner] EXCEPTION DURING TEST EXECUTION: {ex}");
                EditorApplication.Exit(1);
            }
        }
    }
}
