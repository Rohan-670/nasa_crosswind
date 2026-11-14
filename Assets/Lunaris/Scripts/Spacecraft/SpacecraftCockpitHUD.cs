using UnityEngine;
using UnityEngine.UI;
using Lunaris.Core;
using Lunaris.Navigation;

namespace Lunaris.Spacecraft
{
    public class SpacecraftCockpitHUD : MonoBehaviour
    {
        [Header("Cockpit References")]
        [SerializeField] private SpacecraftFlightController flightController;
        [SerializeField] private NavigationRouteManager routeManager;
        [SerializeField] private Transform moonTarget;

        [Header("HUD Visual Theme")]
        [SerializeField] private Color normalColor = new Color(0f, 0.9f, 0.5f, 1f); // Green
        [SerializeField] private Color cautionColor = new Color(1f, 0.85f, 0f, 1f); // Yellow
        [SerializeField] private Color alertColor = new Color(1f, 0.2f, 0.2f, 1f);   // Red
        [SerializeField] private Color recoveredColor = new Color(0f, 0.8f, 1f, 1f); // Cyan

        private bool showHUD = true;

        private void Start()
        {
            if (flightController == null) flightController = GetComponent<SpacecraftFlightController>();
            if (routeManager == null) routeManager = NavigationRouteManager.Instance;
        }

        private void OnGUI()
        {
            if (LunarisGameManager.Instance != null &&
                LunarisGameManager.Instance.CurrentState != GameState.FlightCorridor &&
                LunarisGameManager.Instance.CurrentState != GameState.SpacecraftCockpit)
            {
                return;
            }

            DrawCockpitHUD();
        }

        private void DrawCockpitHUD()
        {
            float screenW = Screen.width;
            float screenH = Screen.height;

            // 1. TOP HEADER: Mission & Location Bar
            GUI.Box(new Rect(20, 15, screenW - 40, 45), "");
            GUIStyle headerStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            headerStyle.normal.textColor = Color.white;

            string dest = LunarisGameManager.Instance != null ? LunarisGameManager.Instance.CurrentMission.destination : "Lunar South Pole";
            float elapsedDays = LunarisGameManager.Instance != null ? LunarisGameManager.Instance.CurrentMission.elapsedMissionDays : 0.5f;
            GUI.Label(new Rect(30, 20, screenW - 60, 35), $"LUNARIS FLIGHT SYSTEM  |  TARGET: {dest.ToUpper()}  |  MISSION DAY: {elapsedDays:F2}", headerStyle);

            // 2. LEFT TELEMETRY PANEL (Speed, Altitude, Fuel, Hull)
            float panelW = 280;
            float panelH = 260;
            GUI.Box(new Rect(25, 75, panelW, panelH), "SPACECRAFT TELEMETRY");

            GUIStyle statStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold
            };
            statStyle.normal.textColor = Color.cyan;

            float speedKmh = flightController != null ? flightController.CurrentSpeedKmh : 0f;
            float speedMs = flightController != null ? flightController.CurrentSpeed : 0f;
            float fuel = flightController != null ? flightController.FuelPercentage : 100f;
            float hull = flightController != null ? flightController.HullIntegrityPercentage : 100f;
            bool boosting = flightController != null && flightController.IsBoosting;

            float distToMoonKm = 384400f;
            if (moonTarget != null)
            {
                distToMoonKm = Vector3.Distance(transform.position, moonTarget.position) * 10f;
            }

            int py = 105;
            GUI.Label(new Rect(40, py, 250, 25), $"SPEED: {speedKmh:F1} km/h ({speedMs:F1} m/s)", statStyle); py += 30;
            GUI.Label(new Rect(40, py, 250, 25), $"LUNAR DISTANCE: {distToMoonKm:F0} km", statStyle); py += 30;
            GUI.Label(new Rect(40, py, 250, 25), $"PROPULSION: {(boosting ? "BOOST ACTIVE" : "NOMINAL")}", statStyle); py += 30;

            // Fuel Bar
            GUI.Label(new Rect(40, py, 100, 20), $"FUEL: {fuel:F0}%");
            GUI.color = fuel > 25f ? Color.green : Color.red;
            GUI.HorizontalSlider(new Rect(140, py + 5, 130, 15), fuel, 0f, 100f);
            GUI.color = Color.white;
            py += 35;

            // Hull Integrity Bar
            GUI.Label(new Rect(40, py, 100, 20), $"HULL: {hull:F0}%");
            GUI.color = hull > 40f ? Color.green : Color.red;
            GUI.HorizontalSlider(new Rect(140, py + 5, 130, 15), hull, 0f, 100f);
            GUI.color = Color.white;
            py += 35;

            // Checkpoint Progress
            int cpIdx = routeManager != null ? routeManager.ActiveCheckpointIndex : 0;
            int cpTotal = routeManager != null ? routeManager.Checkpoints.Count : 6;
            GUI.Label(new Rect(40, py, 250, 25), $"CHECKPOINT: {cpIdx}/{cpTotal}", statStyle);

            // 3. RIGHT NAVIGATION & ROUTE CORRIDOR PANEL
            GUI.Box(new Rect(screenW - panelW - 25, 75, panelW, panelH), "NAVIGATION CORRIDOR");

            float dev = routeManager != null ? routeManager.CurrentDeviation : 0f;
            float maxRadius = routeManager != null ? routeManager.CorridorRadius : 65f;
            NavigationCorridorState state = routeManager != null ? routeManager.CorridorState : NavigationCorridorState.Normal;

            Color statusColor = normalColor;
            string statusText = "CORRIDOR: NORMAL";

            switch (state)
            {
                case NavigationCorridorState.Normal:
                    statusColor = normalColor;
                    statusText = "STATUS: ON ROUTE (NORMAL)";
                    break;
                case NavigationCorridorState.Caution:
                    statusColor = cautionColor;
                    statusText = "STATUS: CAUTION (APPROACHING EDGE)";
                    break;
                case NavigationCorridorState.OutOfRoute:
                case NavigationCorridorState.Recovery:
                    statusColor = alertColor;
                    statusText = "STATUS: WRONG ROUTE (BREACH)";
                    break;
                case NavigationCorridorState.Recovered:
                    statusColor = recoveredColor;
                    statusText = "STATUS: ROUTE RECOVERED";
                    break;
            }

            GUIStyle navHeader = new GUIStyle(GUI.skin.label)
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold
            };
            navHeader.normal.textColor = statusColor;

            py = 105;
            GUI.Label(new Rect(screenW - panelW - 10, py, panelW - 20, 25), statusText, navHeader); py += 35;

            GUIStyle devStyle = new GUIStyle(GUI.skin.label) { fontSize = 14 };
            devStyle.normal.textColor = Color.white;
            GUI.Label(new Rect(screenW - panelW - 10, py, panelW - 20, 25), $"DEVIATION: {dev:F1} m / {maxRadius:F1} m", devStyle); py += 30;

            float compliance = LunarisGameManager.Instance != null ? LunarisGameManager.Instance.CurrentMission.corridorAccuracyPercentage : 100f;
            GUI.Label(new Rect(screenW - panelW - 10, py, panelW - 20, 25), $"ROUTE ACCURACY: {compliance:F1}%", devStyle); py += 30;

            // Route Deviation Visual Bar
            float devRatio = Mathf.Clamp01(dev / maxRadius);
            GUI.Label(new Rect(screenW - panelW - 10, py, panelW - 20, 20), "CORRIDOR DRIFT:"); py += 22;
            GUI.color = statusColor;
            GUI.HorizontalSlider(new Rect(screenW - panelW - 10, py, panelW - 30, 20), devRatio * 100f, 0f, 100f);
            GUI.color = Color.white;
            py += 35;

            // Active Checkpoint Name
            string cpName = (routeManager != null && routeManager.ActiveCheckpoint != null) ? routeManager.ActiveCheckpoint.checkpointName : "En Route";
            GUI.Label(new Rect(screenW - panelW - 10, py, panelW - 20, 25), $"WAYPOINT: {cpName}", statStyle);

            // 4. CRITICAL RECOVERY COUNTDOWN / WRONG ROUTE ALERT BANNER (Center Screen)
            if (state == NavigationCorridorState.OutOfRoute || state == NavigationCorridorState.Recovery)
            {
                float countdown = routeManager != null ? routeManager.CurrentRecoveryCountdown : 15f;
                bool flash = Mathf.PingPong(Time.time * 4f, 1f) > 0.3f;

                float bannerW = 540;
                float bannerH = 120;
                float bx = (screenW - bannerW) / 2f;
                float by = 150;

                GUI.color = flash ? alertColor : new Color(0.5f, 0f, 0f, 0.9f);
                GUI.Box(new Rect(bx, by, bannerW, bannerH), "");
                GUI.color = Color.white;

                GUIStyle alertBig = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 24,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter
                };
                alertBig.normal.textColor = Color.white;

                GUI.Label(new Rect(bx, by + 10, bannerW, 35), "⚠ WRONG ROUTE — TRAJECTORY BREACH ⚠", alertBig);

                GUIStyle countdownStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 32,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter
                };
                countdownStyle.normal.textColor = Color.yellow;
                GUI.Label(new Rect(bx, by + 45, bannerW, 40), $"RECOVERY WINDOW: {countdown:F1}s", countdownStyle);

                GUIStyle hintStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 13,
                    fontStyle = FontStyle.Italic,
                    alignment = TextAnchor.MiddleCenter
                };
                hintStyle.normal.textColor = Color.white;
                GUI.Label(new Rect(bx, by + 88, bannerW, 25), "STEER TOWARD CORRIDOR CENTER TO AVERT MISSION TERMINATION", hintStyle);
            }

            // 5. BOTTOM CONTROLS HINT BAR
            GUI.Box(new Rect(20, screenH - 50, screenW - 40, 38), "");
            GUIStyle hintBar = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter
            };
            hintBar.normal.textColor = new Color(0.8f, 0.85f, 0.9f);
            GUI.Label(new Rect(30, screenH - 46, screenW - 60, 30),
                "[W/S] Throttle/Reverse  |  [A/D] Yaw Turn  |  [Q/E] Roll  |  [Space] Vertical Thrust  |  [Left Shift] Boost  |  [Esc] Pause",
                hintBar);
        }
    }
}
