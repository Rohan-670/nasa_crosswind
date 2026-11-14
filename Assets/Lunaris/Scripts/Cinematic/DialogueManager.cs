using System;
using System.Collections.Generic;
using UnityEngine;
using Lunaris.Core;

namespace Lunaris.Cinematic
{
    [Serializable]
    public class DialogueLine
    {
        public string speakerName;
        public string speakerRole;
        public string text;
        public Texture2D portrait;

        public DialogueLine(string name, string role, string text, Texture2D portrait = null)
        {
            this.speakerName = name;
            this.speakerRole = role;
            this.text = text;
            this.portrait = portrait;
        }
    }

    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }

        [Header("Character Portraits")]
        [SerializeField] private Texture2D portraitAlex;
        [SerializeField] private Texture2D portraitElena;
        [SerializeField] private Texture2D portraitJunior;

        private List<DialogueLine> scriptLines = new List<DialogueLine>();
        private int currentLineIndex = 0;
        private bool isDialogueActive = false;
        private bool showBriefingPanel = false;

        public event Action OnDialogueCompleted;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            LoadPortraitsIfNull();
            BuildOpeningDialogueScript();
        }

        private void LoadPortraitsIfNull()
        {
            if (portraitAlex == null)
                portraitAlex = Resources.Load<Texture2D>("Portrait_AlexMorgan");
            if (portraitElena == null)
                portraitElena = Resources.Load<Texture2D>("Portrait_ElenaCarter");
            if (portraitJunior == null)
                portraitJunior = Resources.Load<Texture2D>("Portrait_JuniorAstronaut");
        }

        public void BuildOpeningDialogueScript()
        {
            scriptLines.Clear();
            scriptLines.Add(new DialogueLine(
                "Commander Alex Morgan", "Senior Astronaut & Mission Commander",
                "Welcome, recruit. Today, you begin your journey towards becoming a lunar explorer.",
                portraitAlex));

            scriptLines.Add(new DialogueLine(
                "Dr. Elena Carter", "Mission-Control & Navigation Officer",
                "Your destination is the Moon. But before you reach it, you must prove that you can navigate a spacecraft safely.",
                portraitElena));

            scriptLines.Add(new DialogueLine(
                "Junior Astronaut", "Player Recruit",
                "I'm ready, Commander. I'll do everything I can to complete the mission.",
                portraitJunior));

            scriptLines.Add(new DialogueLine(
                "Commander Alex Morgan", "Senior Astronaut & Mission Commander",
                "Your first task is to pilot the spacecraft along the designated navigation corridor. Stay within the permitted flight path. Your navigation system will guide you.",
                portraitAlex));

            scriptLines.Add(new DialogueLine(
                "Dr. Elena Carter", "Mission-Control & Navigation Officer",
                "Be careful. If you move outside the route, you'll receive a warning. You must correct your trajectory before your recovery time expires. Failure to recover will result in mission termination.",
                portraitElena));

            scriptLines.Add(new DialogueLine(
                "Commander Alex Morgan", "Senior Astronaut & Mission Commander",
                "Once you reach the Moon, you'll perform a surface expedition. Explore the terrain, collect geological samples and complete your scientific objectives.",
                portraitAlex));

            scriptLines.Add(new DialogueLine(
                "Dr. Elena Carter", "Mission-Control & Navigation Officer",
                "Navigation systems are ready. Mission control is standing by.",
                portraitElena));

            scriptLines.Add(new DialogueLine(
                "Commander Alex Morgan", "Senior Astronaut & Mission Commander",
                "The future of lunar exploration begins with you. Good luck, astronaut.",
                portraitAlex));
        }

        public void StartDialogueSequence()
        {
            currentLineIndex = 0;
            isDialogueActive = true;
            showBriefingPanel = false;
        }

        public void AdvanceDialogue()
        {
            if (SoundManager.Instance != null) SoundManager.Instance.PlayButtonClick();

            currentLineIndex++;
            if (currentLineIndex >= scriptLines.Count)
            {
                isDialogueActive = false;
                showBriefingPanel = true;
            }
        }

        public void SkipDialogue()
        {
            if (SoundManager.Instance != null) SoundManager.Instance.PlayButtonClick();
            isDialogueActive = false;
            showBriefingPanel = true;
        }

        private void OnGUI()
        {
            if (LunarisGameManager.Instance == null) return;
            GameState state = LunarisGameManager.Instance.CurrentState;

            if (state != GameState.TrainingFacility && state != GameState.MissionBriefing && !isDialogueActive && !showBriefingPanel)
            {
                return;
            }

            if (isDialogueActive && currentLineIndex < scriptLines.Count)
            {
                DrawDialogueBox(scriptLines[currentLineIndex]);
            }
            else if (showBriefingPanel)
            {
                DrawMissionBriefingPanel();
            }
        }

        private void DrawDialogueBox(DialogueLine line)
        {
            float screenW = Screen.width;
            float screenH = Screen.height;

            float boxW = Mathf.Min(860f, screenW - 60f);
            float boxH = 190f;
            float bx = (screenW - boxW) / 2f;
            float by = screenH - boxH - 40f;

            // Background Card
            GUI.Box(new Rect(bx, by, boxW, boxH), "");

            // Portrait (Left)
            float portSize = 130f;
            Rect portRect = new Rect(bx + 25, by + 30, portSize, portSize);
            if (line.portrait != null)
            {
                GUI.DrawTexture(portRect, line.portrait, ScaleMode.ScaleToFit);
            }
            else
            {
                GUI.Box(portRect, "PORTRAIT");
            }

            // Name and Role Header
            GUIStyle nameStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold
            };
            nameStyle.normal.textColor = new Color(0.2f, 0.8f, 1f);
            GUI.Label(new Rect(bx + 175, by + 20, boxW - 300, 26), line.speakerName.ToUpper(), nameStyle);

            GUIStyle roleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Italic
            };
            roleStyle.normal.textColor = new Color(0.7f, 0.75f, 0.8f);
            GUI.Label(new Rect(bx + 175, by + 46, boxW - 300, 20), line.speakerRole, roleStyle);

            // Dialogue text
            GUIStyle textStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 16,
                wordWrap = true
            };
            textStyle.normal.textColor = Color.white;
            GUI.Label(new Rect(bx + 175, by + 72, boxW - 200, 75), $"\"{line.text}\"", textStyle);

            // Controls (Next / Skip)
            if (GUI.Button(new Rect(bx + boxW - 130, by + boxH - 45, 110, 32), "NEXT ▶"))
            {
                AdvanceDialogue();
            }

            if (GUI.Button(new Rect(bx + boxW - 245, by + boxH - 45, 100, 32), "SKIP ⏭"))
            {
                SkipDialogue();
            }
        }

        private void DrawMissionBriefingPanel()
        {
            float screenW = Screen.width;
            float screenH = Screen.height;

            float panelW = Mathf.Min(740f, screenW - 60f);
            float panelH = 460f;
            float px = (screenW - panelW) / 2f;
            float py = (screenH - panelH) / 2f;

            GUI.Box(new Rect(px, py, panelW, panelH), "NASA / LUNARIS MISSION BRIEFING");

            GUIStyle titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 20,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            titleStyle.normal.textColor = Color.yellow;
            GUI.Label(new Rect(px + 20, py + 35, panelW - 40, 30), "FLIGHT MANIFEST — LUNAR EXPEDITION 01", titleStyle);

            GUIStyle bodyStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                wordWrap = true
            };
            bodyStyle.normal.textColor = Color.white;

            int yPos = (int)py + 80;
            GUI.Label(new Rect(px + 40, yPos, panelW - 80, 25), "MISSION NAME: Lunar Expedition 01", bodyStyle); yPos += 28;
            GUI.Label(new Rect(px + 40, yPos, panelW - 80, 25), "DESTINATION: Lunar South Pole (Shackleton Rim)", bodyStyle); yPos += 28;
            GUI.Label(new Rect(px + 40, yPos, panelW - 80, 25), "MISSION TYPE: Spacecraft navigation and lunar surface exploration", bodyStyle); yPos += 28;
            GUI.Label(new Rect(px + 40, yPos, panelW - 80, 25), "PLAYER: Junior Astronaut Recruit", bodyStyle); yPos += 35;

            GUIStyle objHeader = new GUIStyle(bodyStyle) { fontStyle = FontStyle.Bold };
            objHeader.normal.textColor = Color.cyan;
            GUI.Label(new Rect(px + 40, yPos, panelW - 80, 25), "PRIMARY MISSION OBJECTIVES:", objHeader); yPos += 26;

            string[] objectives = new string[] {
                "1. Pilot spacecraft inside designated Trans-Lunar navigation corridor.",
                "2. Maintain trajectory compliance (< 65m corridor deviation).",
                "3. Reach the Moon and initiate orbital deceleration.",
                "4. Complete automated lunar landing sequence at South Pole.",
                "5. Deploy astronaut EVA and explore lunar surface terrain.",
                "6. Collect 3 geological science samples (basalt, anorthosite, ice core).",
                "7. Return safely to the landing craft to complete the mission."
            };

            foreach (var obj in objectives)
            {
                GUI.Label(new Rect(px + 55, yPos, panelW - 95, 22), obj, bodyStyle);
                yPos += 24;
            }

            // Big Start Mission Button
            if (GUI.Button(new Rect(px + (panelW - 240) / 2f, py + panelH - 60, 240, 44), "🚀 START MISSION"))
            {
                if (SoundManager.Instance != null) SoundManager.Instance.PlayButtonClick();
                showBriefingPanel = false;
                if (LunarisGameManager.Instance != null)
                {
                    LunarisGameManager.Instance.StartMissionFromBriefing();
                }
            }
        }
    }
}
