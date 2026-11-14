using UnityEditor;
using UnityEngine;
using Outpost.System1.Gameplay.Tests;

namespace Outpost.System1.Gameplay.Editor
{
    public static class Phase2TestRunner
    {
        public static void RunTests()
        {
            Debug.Log("[Phase2TestRunner] Executing Phase 2 Resource & Equipment System Tests...");
            try
            {
                bool success = ResourceSystemPhase2Tests.RunAllTests();
                if (success)
                {
                    Debug.Log("[Phase2TestRunner] RESULT: ALL PHASE 2 TESTS PASSED SUCCESSFULLY!");
                    EditorApplication.Exit(0);
                }
                else
                {
                    Debug.LogError("[Phase2TestRunner] RESULT: ONE OR MORE TESTS FAILED.");
                    EditorApplication.Exit(1);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[Phase2TestRunner] EXCEPTION DURING TEST EXECUTION: {ex}");
                EditorApplication.Exit(1);
            }
        }
    }
}
