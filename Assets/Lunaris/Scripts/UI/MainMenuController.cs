using UnityEngine;
using Lunaris.Core;

namespace Lunaris.UI
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("Cinematic Background Actors")]
        [SerializeField] private Transform rotatingSpacecraft;
        [SerializeField] private Transform rotatingEarth;
        [SerializeField] private float rotationSpeed = 6f;

        private bool showSettings = false;
        private bool showCredits = false;
        private bool showMissionSelect = false;

        private void Update()
        {
            if (rotatingSpacecraft != null)
            {
                rotatingSpacecraft.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
            }
            if (rotatingEarth != null)
            {
                rotatingEarth.Rotate(Vector3.up, (rotationSpeed * 0.4f) * Time.deltaTime, Space.World);
            }
        }

        private void OnGUI()
        {
            if (LunarisGameManager.Instance != null && LunarisGameManager.Instance.CurrentState != GameState.MainMenu)
            {
                return;
            }

            DrawMainMenu();

            if (showSettings) DrawSettingsModal();
            if (showCredits) DrawCreditsModal();
            if (showMissionSelect) DrawMissionSelectModal();
        }

        private void DrawMainMenu()
        {
            float screenW = Screen.width;
            float screenH = Screen.height;

            // Title Header
            GUIStyle mainTitleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 42,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft
            };
            mainTitleStyle.normal.textColor = Color.white;

            GUIStyle subTitleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft
            };
            subTitleStyle.normal.textColor = new Color(0.2f, 0.8f, 1f);

            GUIStyle metaStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Italic
            };
            metaStyle.normal.textColor = new Color(0.7f, 0.75f, 0.8f);

            float leftMargin = 70f;
            float topMargin = 80f;

            GUI.Label(new Rect(leftMargin, topMargin, 600, 50), "LUNARIS", mainTitleStyle);
            GUI.Label(new Rect(leftMargin, topMargin + 48, 600, 30), "BEYOND THE FRONTIER", subTitleStyle);
            GUI.Label(new Rect(leftMargin, topMargin + 76, 600, 25), "Junior Astronaut Training & Lunar Expedition Simulator", metaStyle);

            // Menu Buttons
            float btnW = 260f;
            float btnH = 46f;
            float btnY = topMargin + 130f;
            float spacing = 56f;

            bool hasSave = SaveSystem.HasSave();

            // 1. NEW GAME
            if (GUI.Button(new Rect(leftMargin, btnY, btnW, btnH), "▶  NEW GAME"))
            {
                if (SoundManager.Instance != null) SoundManager.Instance.PlayButtonClick();
                if (LunarisGameManager.Instance != null)
                {
                    LunarisGameManager.Instance.StartNewGame();
                }
            }
            btnY += spacing;

            // 2. CONTINUE
            GUI.enabled = hasSave;
            if (GUI.Button(new Rect(leftMargin, btnY, btnW, btnH), hasSave ? "↻  CONTINUE" : "↻  CONTINUE (NO SAVE)"))
            {
                if (SoundManager.Instance != null) SoundManager.Instance.PlayButtonClick();
                if (LunarisGameManager.Instance != null)
                {
                    LunarisGameManager.Instance.ContinueGame();
                }
            }
            GUI.enabled = true;
            btnY += spacing;

            // 3. MISSION SELECT
            if (GUI.Button(new Rect(leftMargin, btnY, btnW, btnH), "◈  MISSION SELECT"))
            {
                if (SoundManager.Instance != null) SoundManager.Instance.PlayButtonClick();
                showMissionSelect = true;
            }
            btnY += spacing;

            // 4. SETTINGS
            if (GUI.Button(new Rect(leftMargin, btnY, btnW, btnH), "⚙  SETTINGS"))
            {
                if (SoundManager.Instance != null) SoundManager.Instance.PlayButtonClick();
                showSettings = true;
            }
            btnY += spacing;

            // 5. CREDITS
            if (GUI.Button(new Rect(leftMargin, btnY, btnW, btnH), "★  CREDITS"))
            {
                if (SoundManager.Instance != null) SoundManager.Instance.PlayButtonClick();
                showCredits = true;
            }
            btnY += spacing;

            // 6. EXIT GAME
            if (GUI.Button(new Rect(leftMargin, btnY, btnW, btnH), "✕  EXIT GAME"))
            {
                if (SoundManager.Instance != null) SoundManager.Instance.PlayButtonClick();
                Application.Quit();
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                #endif
            }

            // Version Footer
            GUI.Label(new Rect(leftMargin, screenH - 35, 400, 25), "v1.0.0 LTS • Unity 6 Universal Render Pipeline • NASA Ephemeris Enabled", metaStyle);
        }

        private void DrawSettingsModal()
        {
            float mWidth = 460f;
            float mHeight = 320f;
            float mx = (Screen.width - mWidth) / 2f;
            float my = (Screen.height - mHeight) / 2f;

            GUI.Box(new Rect(mx, my, mWidth, mHeight), "LUNARIS FLIGHT SETTINGS");

            int py = (int)my + 45;
            GUI.Label(new Rect(mx + 30, py, 200, 25), "Master Volume:");
            if (LunarisGameManager.Instance != null)
            {
                LunarisGameManager.Instance.masterVolume = GUI.HorizontalSlider(new Rect(mx + 200, py + 5, 200, 20), LunarisGameManager.Instance.masterVolume, 0f, 1f);
                AudioListener.volume = LunarisGameManager.Instance.masterVolume;
            }
            py += 40;

            GUI.Label(new Rect(mx + 30, py, 200, 25), "Flight Controls Mode:");
            GUI.Label(new Rect(mx + 200, py, 220, 25), "6-DoF Rigidbody Physics (W/S/A/D/Q/E)");
            py += 40;

            GUI.Label(new Rect(mx + 30, py, 200, 25), "NASA API Live Ephemeris:");
            GUI.Label(new Rect(mx + 200, py, 220, 25), "ENABLED (FastAPI Port 8000)");
            py += 40;

            GUI.Label(new Rect(mx + 30, py, 200, 25), "Corridor Tolerance:");
            GUI.Label(new Rect(mx + 200, py, 220, 25), "65.0 Meters (Standard)");
            py += 55;

            if (GUI.Button(new Rect(mx + (mWidth - 140) / 2f, py, 140, 36), "CLOSE"))
            {
                if (SoundManager.Instance != null) SoundManager.Instance.PlayButtonClick();
                showSettings = false;
            }
        }

        private void DrawCreditsModal()
        {
            float mWidth = 520f;
            float mHeight = 300f;
            float mx = (Screen.width - mWidth) / 2f;
            float my = (Screen.height - mHeight) / 2f;

            GUI.Box(new Rect(mx, my, mWidth, mHeight), "LUNARIS: BEYOND THE FRONTIER — CREDITS");

            GUIStyle cStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, wordWrap = true };
            cStyle.normal.textColor = Color.white;

            int py = (int)my + 40;
            GUI.Label(new Rect(mx + 20, py, mWidth - 40, 25), "LUNARIS: Junior Astronaut Training & Lunar Expedition Simulator", cStyle); py += 30;
            GUI.Label(new Rect(mx + 20, py, mWidth - 40, 25), "Developed with Unity 6.3 LTS & Universal Render Pipeline (URP)", cStyle); py += 25;
            GUI.Label(new Rect(mx + 20, py, mWidth - 40, 25), "Authentic NASA Imagery: NASA Images API, NASA EPIC, JPL Horizons", cStyle); py += 35;
            GUI.Label(new Rect(mx + 20, py, mWidth - 40, 25), "Characters: Commander Alex Morgan, Dr. Elena Carter, Junior Astronaut", cStyle); py += 45;

            if (GUI.Button(new Rect(mx + (mWidth - 140) / 2f, py, 140, 36), "CLOSE"))
            {
                if (SoundManager.Instance != null) SoundManager.Instance.PlayButtonClick();
                showCredits = false;
            }
        }

        private void DrawMissionSelectModal()
        {
            float mWidth = 500f;
            float mHeight = 300f;
            float mx = (Screen.width - mWidth) / 2f;
            float my = (Screen.height - mHeight) / 2f;

            GUI.Box(new Rect(mx, my, mWidth, mHeight), "MISSION SELECT");

            int py = (int)my + 45;
            if (GUI.Button(new Rect(mx + 30, py, mWidth - 60, 45), "MISSION 01: LUNAR SOUTH POLE EXPEDITION (ACTIVE)"))
            {
                if (SoundManager.Instance != null) SoundManager.Instance.PlayButtonClick();
                showMissionSelect = false;
                if (LunarisGameManager.Instance != null) LunarisGameManager.Instance.StartNewGame();
            }
            py += 55;

            GUI.enabled = false;
            GUI.Button(new Rect(mx + 30, py, mWidth - 60, 45), "MISSION 02: MARE TRANQUILLITATIS SURVEY (LOCKED)"); py += 55;
            GUI.Button(new Rect(mx + 30, py, mWidth - 60, 45), "MISSION 03: FAR SIDE RADIO TELESCOPE DEPLOYMENT (LOCKED)"); py += 55;
            GUI.enabled = true;

            if (GUI.Button(new Rect(mx + (mWidth - 120) / 2f, py, 120, 32), "CLOSE"))
            {
                if (SoundManager.Instance != null) SoundManager.Instance.PlayButtonClick();
                showMissionSelect = false;
            }
        }
    }
}
