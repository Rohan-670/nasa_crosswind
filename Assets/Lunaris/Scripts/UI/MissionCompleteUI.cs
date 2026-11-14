using UnityEngine;
using Lunaris.Core;

namespace Lunaris.UI
{
    public class MissionCompleteUI : MonoBehaviour
    {
        private void OnGUI()
        {
            if (LunarisGameManager.Instance == null) return;

            GameState state = LunarisGameManager.Instance.CurrentState;
            if (state == GameState.MissionCompleted)
            {
                DrawMissionSuccess();
            }
            else if (state == GameState.MissionFailed)
            {
                DrawMissionFailed();
            }
        }

        private void DrawMissionSuccess()
        {
            float screenW = Screen.width;
            float screenH = Screen.height;

            float panelW = Mathf.Min(680f, screenW - 60f);
            float panelH = 440f;
            float px = (screenW - panelW) / 2f;
            float py = (screenH - panelH) / 2f;

            GUI.Box(new Rect(px, py, panelW, panelH), "NASA / LUNARIS MISSION DEBRIEFING");

            GUIStyle titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 26,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            titleStyle.normal.textColor = Color.green;
            GUI.Label(new Rect(px + 20, py + 30, panelW - 40, 35), "★ MISSION ACCOMPLISHED ★", titleStyle);

            GUIStyle subStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                fontStyle = FontStyle.Italic,
                alignment = TextAnchor.MiddleCenter
            };
            subStyle.normal.textColor = Color.cyan;
            GUI.Label(new Rect(px + 20, py + 68, panelW - 40, 25), "Lunar South Pole Expedition 01 Touchdown & Return Verified", subStyle);

            var mission = LunarisGameManager.Instance.CurrentMission;

            GUIStyle statStyle = new GUIStyle(GUI.skin.label) { fontSize = 15 };
            statStyle.normal.textColor = Color.white;

            int yPos = (int)py + 115;
            GUI.Label(new Rect(px + 50, yPos, panelW - 100, 25), $"ELAPSED MISSION DAYS: {mission.elapsedMissionDays:F2} Earth Days", statStyle); yPos += 35;
            GUI.Label(new Rect(px + 50, yPos, panelW - 100, 25), $"SCIENTIFIC SAMPLES COLLECTED: {mission.samplesCollected} / {mission.totalRequiredSamples} Specimens", statStyle); yPos += 35;
            GUI.Label(new Rect(px + 50, yPos, panelW - 100, 25), $"TRANS-LUNAR ROUTE ACCURACY: {mission.corridorAccuracyPercentage:F1}% Compliance", statStyle); yPos += 35;
            GUI.Label(new Rect(px + 50, yPos, panelW - 100, 25), $"MAXIMUM RECORDED DRIFT: {mission.maxRecordedDeviation:F1} meters", statStyle); yPos += 45;

            GUIStyle rankStyle = new GUIStyle(statStyle) { fontStyle = FontStyle.Bold };
            rankStyle.normal.textColor = Color.yellow;
            GUI.Label(new Rect(px + 50, yPos, panelW - 100, 30), "PROMOTION EARNED: Lunar Explorer Class 1", rankStyle);

            // Bottom Buttons
            float btnW = 200f;
            float btnH = 42f;

            if (GUI.Button(new Rect(px + 60, py + panelH - 65, btnW, btnH), "↻ REPLAY MISSION"))
            {
                if (SoundManager.Instance != null) SoundManager.Instance.PlayButtonClick();
                LunarisGameManager.Instance.RestartMission();
            }

            if (GUI.Button(new Rect(px + panelW - btnW - 60, py + panelH - 65, btnW, btnH), "⌂ MAIN MENU"))
            {
                if (SoundManager.Instance != null) SoundManager.Instance.PlayButtonClick();
                LunarisGameManager.Instance.ReturnToMainMenu();
            }
        }

        private void DrawMissionFailed()
        {
            float screenW = Screen.width;
            float screenH = Screen.height;

            float panelW = Mathf.Min(640f, screenW - 60f);
            float panelH = 380f;
            float px = (screenW - panelW) / 2f;
            float py = (screenH - panelH) / 2f;

            GUI.color = new Color(1f, 0.4f, 0.4f, 1f);
            GUI.Box(new Rect(px, py, panelW, panelH), "MISSION TERMINATION LOG");
            GUI.color = Color.white;

            GUIStyle titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 28,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            titleStyle.normal.textColor = Color.red;
            GUI.Label(new Rect(px + 20, py + 35, panelW - 40, 40), "⚠ MISSION FAILED ⚠", titleStyle);

            var mission = LunarisGameManager.Instance.CurrentMission;

            GUIStyle statStyle = new GUIStyle(GUI.skin.label) { fontSize = 15 };
            statStyle.normal.textColor = Color.white;

            int yPos = (int)py + 95;
            GUI.Label(new Rect(px + 50, yPos, panelW - 100, 25), $"TERMINATION CAUSE: {mission.failureReason}", statStyle); yPos += 35;
            GUI.Label(new Rect(px + 50, yPos, panelW - 100, 25), $"ELAPSED MISSION DAYS: {mission.elapsedMissionDays:F2} Days", statStyle); yPos += 35;
            GUI.Label(new Rect(px + 50, yPos, panelW - 100, 25), $"LAST RECORDED DEVIATION: {mission.maxRecordedDeviation:F1} meters", statStyle); yPos += 45;

            GUIStyle warningNote = new GUIStyle(statStyle) { fontStyle = FontStyle.Italic };
            warningNote.normal.textColor = Color.yellow;
            GUI.Label(new Rect(px + 50, yPos, panelW - 100, 30), "Recommendation: Maintain vector within the 65m corridor.", warningNote);

            // Bottom Buttons
            float btnW = 190f;
            float btnH = 42f;

            if (GUI.Button(new Rect(px + 50, py + panelH - 65, btnW, btnH), "↻ RESTART MISSION"))
            {
                if (SoundManager.Instance != null) SoundManager.Instance.PlayButtonClick();
                LunarisGameManager.Instance.RestartMission();
            }

            if (GUI.Button(new Rect(px + panelW - btnW - 50, py + panelH - 65, btnW, btnH), "⌂ MAIN MENU"))
            {
                if (SoundManager.Instance != null) SoundManager.Instance.PlayButtonClick();
                LunarisGameManager.Instance.ReturnToMainMenu();
            }
        }
    }
}
