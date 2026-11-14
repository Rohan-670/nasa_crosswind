using UnityEditor;
using UnityEngine;
using Outpost.System1.Gameplay.Tests;

namespace Outpost.System1.Gameplay.Editor
{
    public static class Phase3TestRunner
    {
        public static void RunTests()
        {
            Debug.Log("[Phase3TestRunner] Executing Phase 3 Emergency, Repair & Character Interaction Tests...");
            try
            {
                bool success = EmergencyPhase3Tests.RunAllTests();
                if (success)
                {
                    Debug.Log("[Phase3TestRunner] RESULT: ALL PHASE 3 TESTS PASSED SUCCESSFULLY!");
                    EditorApplication.Exit(0);
                }
                else
                {
                    Debug.LogError("[Phase3TestRunner] RESULT: ONE OR MORE TESTS FAILED.");
                    EditorApplication.Exit(1);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[Phase3TestRunner] EXCEPTION DURING TEST EXECUTION: {ex}");
                EditorApplication.Exit(1);
            }
        }
    }
}
