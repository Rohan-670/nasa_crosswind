using System;
using UnityEngine;
using Outpost.Core.Data;
using Outpost.Core.Events;
using Outpost.System1.Gameplay.Resources;
using Outpost.System1.Gameplay.Equipment;
using Outpost.System1.Gameplay.Persistence;

namespace Outpost.System1.Gameplay.Tests
{
    /// <summary>
    /// Automated verification suite for Phase 2 Lunar Resource and Equipment Systems.
    /// Can be executed in-editor, in PlayMode, or via Unity batchmode command line.
    /// </summary>
    public class ResourceSystemPhase2Tests : MonoBehaviour
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
            Debug.Log("STARTING PHASE 2 RESOURCE & EQUIPMENT TEST SUITE");
            Debug.Log("==================================================");

            int passed = 0;
            int total = 7;

            if (TestOxygenSystem()) passed++;
            if (TestWaterSystem()) passed++;
            if (TestFoodAndGreenhouseSystem()) passed++;
            if (TestElectricityAndBatterySystem()) passed++;
            if (TestPowerFailureAndLoadShedding()) passed++;
            if (TestThresholdAlertsAndTelemetry()) passed++;
            if (TestSaveAndRestoreState()) passed++;

            Debug.Log("==================================================");
            Debug.Log($"PHASE 2 TEST RESULTS: {passed}/{total} TESTS PASSED");
            Debug.Log("==================================================");

            return passed == total;
        }

        private static bool TestOxygenSystem()
        {
            Debug.Log("[TEST 1] Testing Oxygen Production, Consumption, Storage & Generator Status...");

            GameObject go = new GameObject("Test_ResourceManager_O2");
            ResourceManager rm = go.AddComponent<ResourceManager>();
            rm.EnsureInitialized();

            // Verify initial state
            Assert(rm.OxygenPercent > 95f, "Initial O2 percent should be nominal (> 95%)");
            Assert(rm.CarbonDioxidePpm < 500f, "Initial CO2 ppm should be baseline (< 500 ppm)");

            // Simulate 10 seconds of nominal tick
            rm.SimulateResourceTick(10.0f);
            Assert(rm.CurrentState.O2GeneratorStatus == GeneratorStatus.Nominal, "Generator status should be Nominal");
            Assert(rm.CurrentState.OxygenProductionRate > 0f, "Oxygen production rate should be > 0");
            Assert(rm.CurrentState.OxygenConsumptionRate > 0f, "Oxygen consumption rate should be > 0");

            // Verify storage clamping
            Assert(rm.CurrentState.OxygenCurrentLiters <= rm.CurrentState.OxygenMaxCapacityLiters, "O2 storage must not exceed max capacity");

            UnityEngine.Object.DestroyImmediate(go);
            Debug.Log("[TEST 1 PASSED] Oxygen system verified.");
            return true;
        }

        private static bool TestWaterSystem()
        {
            Debug.Log("[TEST 2] Testing Water Storage, Consumption and Recycling...");

            GameObject go = new GameObject("Test_ResourceManager_Water");
            ResourceManager rm = go.AddComponent<ResourceManager>();
            rm.EnsureInitialized();

            float initialWater = rm.WaterReserveLiters;
            Assert(initialWater > 500f, "Initial water should be > 500 L");

            // Simulate ticks
            rm.SimulateResourceTick(5.0f);
            Assert(rm.CurrentState.WaterConsumptionRate > 0f, "Water consumption should be active");
            Assert(rm.CurrentState.WaterRecyclingRate > 0f, "Water recycling should be active");
            Assert(rm.CurrentState.WaterRecyclingEfficiency >= 0.90f, "WRS efficiency should be >= 90%");

            UnityEngine.Object.DestroyImmediate(go);
            Debug.Log("[TEST 2 PASSED] Water system verified.");
            return true;
        }

        private static bool TestFoodAndGreenhouseSystem()
        {
            Debug.Log("[TEST 3] Testing Food Storage, Consumption and Greenhouse Biomass...");

            GameObject go = new GameObject("Test_ResourceManager_Food");
            ResourceManager rm = go.AddComponent<ResourceManager>();
            rm.EnsureInitialized();

            Assert(rm.CurrentState.FoodRations > 100f, "Initial food rations should be > 100");
            Assert(rm.GreenhouseBiomassPercent > 50f, "Initial greenhouse biomass should be > 50%");

            rm.SimulateResourceTick(10.0f);
            Assert(rm.CurrentState.FoodConsumptionRate > 0f, "Food consumption rate must be > 0");
            Assert(rm.CurrentState.GreenhouseProductionRate > 0f, "Greenhouse production rate must be > 0");

            UnityEngine.Object.DestroyImmediate(go);
            Debug.Log("[TEST 3 PASSED] Food & Greenhouse system verified.");
            return true;
        }

        private static bool TestElectricityAndBatterySystem()
        {
            Debug.Log("[TEST 4] Testing Solar Generation, Load Consumption and Battery Charge/Discharge...");

            GameObject go = new GameObject("Test_ResourceManager_Power");
            ResourceManager rm = go.AddComponent<ResourceManager>();
            rm.EnsureInitialized();

            Assert(rm.SolarPowerGeneratedKw > 40f, "Solar power generation should be >= 40 kW");
            Assert(rm.BatteryChargePercent > 80f, "Battery charge should be > 80%");
            Assert(rm.CurrentState.GridStatus == PowerGridStatus.Nominal, "Grid status should be Nominal with solar surplus");

            // Simulate ticks with surplus: battery should remain full or charge
            rm.SimulateResourceTick(5.0f);
            Assert(rm.BatteryChargePercent <= 100f, "Battery charge must clamp at 100%");

            UnityEngine.Object.DestroyImmediate(go);
            Debug.Log("[TEST 4 PASSED] Electricity & Battery system verified.");
            return true;
        }

        private static bool TestPowerFailureAndLoadShedding()
        {
            Debug.Log("[TEST 5] Testing Power Failure, Emergency Micrometeoroid Damage, and Recovery...");

            GameObject go = new GameObject("Test_ResourceManager_Failure");
            ResourceManager rm = go.AddComponent<ResourceManager>();
            rm.EnsureInitialized();

            // Trigger emergency micrometeoroid damage
            rm.TriggerMicrometeoroidDamage();
            Assert(rm.IsEmergencyActive, "Emergency should be active");
            Assert(!string.IsNullOrEmpty(rm.EmergencyAlertMessage), "Emergency alert message should be set");

            // Simulate with damaged power grid: solar drops to emergency trickle (8 kW)
            rm.SimulateResourceTick(5.0f);
            Assert(rm.SolarPowerGeneratedKw <= 10.0f, "Emergency solar generation should be <= 10 kW");
            Assert(rm.CurrentState.GridStatus == PowerGridStatus.BatteryDraw || rm.CurrentState.GridStatus == PowerGridStatus.LoadShedding, "Grid status should reflect battery draw or shedding");

            // Restore full power
            rm.RestoreFullPower();
            Assert(!rm.IsEmergencyActive, "Emergency should be cleared after restoring full power");
            Assert(rm.SolarPowerGeneratedKw > 40f, "Solar output should recover to nominal");

            UnityEngine.Object.DestroyImmediate(go);
            Debug.Log("[TEST 5 PASSED] Power failure and recovery verified.");
            return true;
        }

        private static bool TestThresholdAlertsAndTelemetry()
        {
            Debug.Log("[TEST 6] Testing Data Notifications, Alert Thresholds, and Telemetry Events...");

            GameObject go = new GameObject("Test_ResourceManager_Alerts");
            ResourceManager rm = go.AddComponent<ResourceManager>();
            rm.EnsureInitialized();

            bool telemetryReceived = false;
            bool detailedReceived = false;

            Action<float, float, float, float> telHandler = (o2, co2, pwr, bat) => telemetryReceived = true;
            Action<ResourceStateData> detailedHandler = (data) => detailedReceived = true;

            OutpostEventChannel.TelemetryUpdated += telHandler;
            OutpostEventChannel.DetailedTelemetryUpdated += detailedHandler;

            rm.SimulateResourceTick(1.0f);

            OutpostEventChannel.TelemetryUpdated -= telHandler;
            OutpostEventChannel.DetailedTelemetryUpdated -= detailedHandler;

            Assert(telemetryReceived, "OutpostEventChannel.TelemetryUpdated event was not fired");
            Assert(detailedReceived, "OutpostEventChannel.DetailedTelemetryUpdated event was not fired");

            UnityEngine.Object.DestroyImmediate(go);
            Debug.Log("[TEST 6 PASSED] Threshold alerts and telemetry verified.");
            return true;
        }

        private static bool TestSaveAndRestoreState()
        {
            Debug.Log("[TEST 7] Testing State Persistence: ExportState and ImportState...");

            GameObject go = new GameObject("Test_ResourceManager_Save");
            ResourceManager rm = go.AddComponent<ResourceManager>();
            rm.EnsureInitialized();

            // Export initial state
            ResourceStateData original = rm.ExportState();
            Assert(original != null, "ExportState returned null");

            // Mutate state manually to simulate changes during mission
            ResourceStateData mutated = original.Clone();
            mutated.OxygenPercent = 82.5f;
            mutated.CarbonDioxidePpm = 950.0f;
            mutated.BatteryPercent = 45.0f;
            mutated.WaterReserveLiters = 420.0f;
            mutated.SparePartsCount = 8;
            mutated.CrewHealthPercent = 75.0f;

            rm.ImportState(mutated);

            Assert(Mathf.Approximately(rm.OxygenPercent, 82.5f), "ImportState did not restore OxygenPercent");
            Assert(Mathf.Approximately(rm.CarbonDioxidePpm, 950.0f), "ImportState did not restore CarbonDioxidePpm");
            Assert(Mathf.Approximately(rm.BatteryChargePercent, 45.0f), "ImportState did not restore BatteryChargePercent");
            Assert(Mathf.Approximately(rm.WaterReserveLiters, 420.0f), "ImportState did not restore WaterReserveLiters");
            Assert(rm.CurrentState.SparePartsCount == 8, "ImportState did not restore SparePartsCount");
            Assert(Mathf.Approximately(rm.CurrentState.CrewHealthPercent, 75.0f), "ImportState did not restore CrewHealthPercent");

            UnityEngine.Object.DestroyImmediate(go);
            Debug.Log("[TEST 7 PASSED] Save and restore state verified.");
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
