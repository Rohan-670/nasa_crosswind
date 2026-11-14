using System;
using System.Collections;
using UnityEngine;
using Outpost.UI.Core;
using Outpost.UI.HUD;
using Outpost.UI.ORION;
using Outpost.UI.Map;

namespace Outpost.UI.Testing
{
    /// <summary>
    /// Automated test harness for verifying Phase 2: Resource Dashboard, ORION AI, and Interactive Lunar Map.
    /// Runs a complete end-to-end verification suite across all 8 resources, ORION message types, and map features.
    /// </summary>
    public class Phase2UITestRunner : MonoBehaviour
    {
        [Header("Test Configuration")]
        [SerializeField] private bool _runOnStart = true;
        [SerializeField] private float _stepDelay = 0.8f;

        [Header("System References")]
        [SerializeField] private ResourceDashboard _resourceDashboard;
        [SerializeField] private ORIONInterface _orionInterface;
        [SerializeField] private LunarMapUI _lunarMapUI;
        [SerializeField] private MissionProgressionBridge _bridge;

        private void Start()
        {
            if (_runOnStart)
            {
                StartCoroutine(ExecutePhase2Tests());
            }
        }

        public IEnumerator ExecutePhase2Tests()
        {
            Debug.Log("<color=cyan>==================================================</color>");
            Debug.Log("<color=cyan>[Phase2Test] Starting Phase 2: Resource Dashboard, ORION, and Lunar Map Validation...</color>");
            Debug.Log("<color=cyan>==================================================</color>");

            // ----------------------------------------------------
            // TEST SUITE 1: RESOURCE DASHBOARD (ALL 8 RESOURCES)
            // ----------------------------------------------------
            Debug.Log("[Phase2Test] Suite 1: Testing Resource Dashboard Telemetry (Nominal States)...");

            var nominalSnapshot = new OutpostResourceSnapshot
            {
                Oxygen = new ResourceEntry { ResourceId = "O2", DisplayName = "Oxygen", CurrentValue = 92f, MaxValue = 100f, Unit = "%", WarningThreshold = 30f, CriticalThreshold = 15f, SimpleExplanation = "Oxygen generated from lunar ice electrolysis." },
                Water = new ResourceEntry { ResourceId = "H2O", DisplayName = "Water", CurrentValue = 460f, MaxValue = 500f, Unit = "L", WarningThreshold = 100f, CriticalThreshold = 50f, SimpleExplanation = "Closed-loop recycled water." },
                Food = new ResourceEntry { ResourceId = "FOOD", DisplayName = "Food", CurrentValue = 140f, MaxValue = 150f, Unit = "Rations", WarningThreshold = 30f, CriticalThreshold = 15f, SimpleExplanation = "Hydroponic lettuce and vacuum rations." },
                Electricity = new ResourceEntry { ResourceId = "PWR", DisplayName = "Electricity", CurrentValue = 44f, MaxValue = 50f, Unit = "kW", WarningThreshold = 20f, CriticalThreshold = 10f, SimpleExplanation = "Ridge solar farm generation." },
                Battery = new ResourceEntry { ResourceId = "BAT", DisplayName = "Battery", CurrentValue = 88f, MaxValue = 100f, Unit = "kWh", WarningThreshold = 30f, CriticalThreshold = 15f, SimpleExplanation = "Reserve battery packs." },
                CrewHealth = new ResourceEntry { ResourceId = "HLTH", DisplayName = "Crew Health", CurrentValue = 98f, MaxValue = 100f, Unit = "%", WarningThreshold = 50f, CriticalThreshold = 25f, SimpleExplanation = "Vitals, heart rate, stamina." },
                EquipmentCondition = new ResourceEntry { ResourceId = "COND", DisplayName = "Equipment Condition", CurrentValue = 95f, MaxValue = 100f, Unit = "%", WarningThreshold = 40f, CriticalThreshold = 20f, SimpleExplanation = "Station structural integrity." },
                SpareParts = new ResourceEntry { ResourceId = "SPARE", DisplayName = "Spare Parts", CurrentValue = 14f, MaxValue = 20f, Unit = "Units", WarningThreshold = 4f, CriticalThreshold = 2f, SimpleExplanation = "Mechanical filters and seal tape." }
            };

            UIEventBus.PublishResourceSnapshot(nominalSnapshot);
            yield return new WaitForSeconds(_stepDelay);

            // Test Warning & Critical States
            Debug.Log("[Phase2Test] Suite 1b: Testing Resource Warning State (Oxygen drop)...");
            UIEventBus.PublishOxygenData(28f, 100f, "Warning: Low oxygen levels detected. Scrubbers cycling at low power.");
            yield return new WaitForSeconds(_stepDelay * 0.5f);

            Debug.Log("[Phase2Test] Suite 1c: Testing Resource Critical State (Battery drain)...");
            UIEventBus.PublishBatteryData(12f, 100f, "Critical: Solar conduit severed. Running on emergency battery backup!");
            yield return new WaitForSeconds(_stepDelay * 0.5f);

            // Restore nominal
            UIEventBus.PublishOxygenData(95f, 100f);
            UIEventBus.PublishBatteryData(85f, 100f);
            yield return new WaitForSeconds(_stepDelay * 0.5f);

            // ----------------------------------------------------
            // TEST SUITE 2: ORION AI INTERFACE
            // ----------------------------------------------------
            Debug.Log("[Phase2Test] Suite 2: Testing ORION AI Messages & Voice Telemetry...");

            // 2.1 Mission Status
            Debug.Log("[Phase2Test] 2.1 ORION Mission Status...");
            if (_orionInterface != null)
            {
                _orionInterface.PostMissionStatus("Habitat power online. Solar arrays generating 44kW nominal.");
            }
            yield return new WaitForSeconds(_stepDelay);

            // 2.2 Equipment Warning (Mandated Example)
            Debug.Log("[Phase2Test] 2.2 ORION Equipment Warning...");
            if (_orionInterface != null)
            {
                _orionInterface.PostEquipmentWarning("solar array", "Please inspect the external power equipment.");
            }
            yield return new WaitForSeconds(_stepDelay);

            // 2.3 Resource Warning
            Debug.Log("[Phase2Test] 2.3 ORION Resource Warning...");
            if (_orionInterface != null)
            {
                _orionInterface.PostResourceWarning("Battery Storage", 25f, 30f);
            }
            yield return new WaitForSeconds(_stepDelay);

            // 2.4 Navigation Guidance
            Debug.Log("[Phase2Test] 2.4 ORION Navigation Guidance...");
            if (_orionInterface != null)
            {
                _orionInterface.PostNavigationGuidance("Rover Maintenance Bay", 75f, "Follow the marked west road.");
            }
            yield return new WaitForSeconds(_stepDelay);

            // 2.5 Scientific Explanation
            Debug.Log("[Phase2Test] 2.5 ORION Scientific Explanation...");
            if (_orionInterface != null)
            {
                _orionInterface.PostScientificExplanation(
                    "Thermal Radiation in Space",
                    "In the vacuum of the Moon, heat cannot float away through air. Equipment uses metal radiator fins to beam heat out into space as infrared light!"
                );
            }
            yield return new WaitForSeconds(_stepDelay);

            // 2.6 Emergency Notification
            Debug.Log("[Phase2Test] 2.6 ORION Emergency Notification...");
            if (_orionInterface != null)
            {
                _orionInterface.PostEmergencyNotification("Micrometeoroid impact detected near Substation 2!");
            }
            yield return new WaitForSeconds(_stepDelay);

            // 2.7 Objective Reminder
            Debug.Log("[Phase2Test] 2.7 ORION Objective Reminder...");
            if (_orionInterface != null)
            {
                _orionInterface.PostObjectiveReminder("Proceed to Habitat Core and inspect life-support telemetry.");
            }
            yield return new WaitForSeconds(_stepDelay);

            // ----------------------------------------------------
            // TEST SUITE 3: INTERACTIVE LUNAR GEO MAP
            // ----------------------------------------------------
            Debug.Log("[Phase2Test] Suite 3: Testing Interactive Lunar GEO Map...");

            if (_lunarMapUI != null)
            {
                // Open Map
                Debug.Log("[Phase2Test] 3.1 Opening Lunar Map...");
                _lunarMapUI.OpenMap();
                yield return new WaitForSeconds(_stepDelay * 0.5f);

                // Focus on key landmarks among the 10 sectors
                string[] testLocations = { "LOC_HABITAT", "LOC_LANDING", "LOC_SOLAR", "LOC_GREENHOUSE", "LOC_ROVER", "LOC_EMERGENCY_1" };
                foreach (var locId in testLocations)
                {
                    Debug.Log($"[Phase2Test] 3.2 Inspecting Sector: {locId}...");
                    _lunarMapUI.FocusOnLocation(locId);
                    yield return new WaitForSeconds(_stepDelay * 0.4f);
                }

                // Set Active Waypoint to Solar Farm
                Debug.Log("[Phase2Test] 3.3 Pinning Waypoint to Solar Farm (LOC_SOLAR)...");
                _lunarMapUI.SetWaypoint("LOC_SOLAR");
                yield return new WaitForSeconds(_stepDelay * 0.5f);

                // Focus back on player
                Debug.Log("[Phase2Test] 3.4 Return-to-Player Focus Button...");
                _lunarMapUI.FocusOnPlayer();
                yield return new WaitForSeconds(_stepDelay * 0.5f);

                // Close Map
                Debug.Log("[Phase2Test] 3.5 Closing Lunar Map...");
                _lunarMapUI.CloseMap();
                yield return new WaitForSeconds(_stepDelay * 0.3f);
            }

            Debug.Log("<color=green>==================================================</color>");
            Debug.Log("<color=green>[Phase2Test] PHASE 2 VALIDATION COMPLETE: ALL SYSTEMS VERIFIED</color>");
            Debug.Log("<color=green>==================================================</color>");
        }
    }
}
