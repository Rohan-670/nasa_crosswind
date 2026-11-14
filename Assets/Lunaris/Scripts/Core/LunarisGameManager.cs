using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lunaris.Core
{
    public enum GameState
    {
        MainMenu,
        OpeningCinematic,
        TrainingFacility,
        MissionBriefing,
        SpacecraftCockpit,
        FlightCorridor,
        LunarLanding,
        LunarExpedition,
        MissionCompleted,
        MissionFailed,
        Paused
    }

    public class LunarisGameManager : MonoBehaviour
    {
        public static LunarisGameManager Instance { get; private set; }

        [Header("State Machine")]
        [SerializeField] private GameState currentState = GameState.MainMenu;
        public GameState CurrentState => currentState;

        public event Action<GameState> OnStateChanged;

        [Header("Mission Telemetry")]
        [SerializeField] private MissionProgress currentMission = new MissionProgress();
        public MissionProgress CurrentMission => currentMission;

        [Header("Settings & Preferences")]
        public float masterVolume = 1f;
        public float mouseSensitivity = 2f;
        public bool invertY = false;

        private GameState previousStateBeforePause;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            TransitionToState(GameState.MainMenu);
        }

        private void Update()
        {
            if (currentState == GameState.FlightCorridor || currentState == GameState.LunarExpedition)
            {
                currentMission.UpdateElapsedDays();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
        }

        public void TransitionToState(GameState newState)
        {
            if (currentState == newState) return;

            Debug.Log($"[LunarisGameManager] State Transition: {currentState} -> {newState}");
            currentState = newState;
            OnStateChanged?.Invoke(currentState);

            switch (currentState)
            {
                case GameState.MainMenu:
                    Time.timeScale = 1f;
                    break;
                case GameState.OpeningCinematic:
                    Time.timeScale = 1f;
                    break;
                case GameState.TrainingFacility:
                    Time.timeScale = 1f;
                    break;
                case GameState.MissionBriefing:
                    Time.timeScale = 1f;
                    break;
                case GameState.SpacecraftCockpit:
                    Time.timeScale = 1f;
                    currentMission = new MissionProgress();
                    currentMission.missionStartTime = Time.time;
                    break;
                case GameState.FlightCorridor:
                    Time.timeScale = 1f;
                    break;
                case GameState.LunarLanding:
                    Time.timeScale = 1f;
                    currentMission.hasLandedSafely = true;
                    break;
                case GameState.LunarExpedition:
                    Time.timeScale = 1f;
                    break;
                case GameState.MissionCompleted:
                    Time.timeScale = 1f;
                    SaveGameProgress();
                    break;
                case GameState.MissionFailed:
                    Time.timeScale = 1f;
                    break;
                case GameState.Paused:
                    Time.timeScale = 0f;
                    break;
            }
        }

        public void StartNewGame()
        {
            currentMission = new MissionProgress();
            TransitionToState(GameState.OpeningCinematic);
        }

        public void ContinueGame()
        {
            PlayerSaveData save = SaveSystem.Load();
            currentMission = new MissionProgress();
            currentMission.destination = "Lunar South Pole Mission " + save.currentMissionIndex;
            TransitionToState(GameState.SpacecraftCockpit);
        }

        public void StartMissionFromBriefing()
        {
            TransitionToState(GameState.SpacecraftCockpit);
        }

        public void FailMission(string reason)
        {
            currentMission.failureReason = reason;
            TransitionToState(GameState.MissionFailed);
        }

        public void CompleteMission()
        {
            TransitionToState(GameState.MissionCompleted);
        }

        public void RestartMission()
        {
            Time.timeScale = 1f;
            currentMission = new MissionProgress();
            TransitionToState(GameState.SpacecraftCockpit);
        }

        public void ReturnToMainMenu()
        {
            Time.timeScale = 1f;
            TransitionToState(GameState.MainMenu);
        }

        public void TogglePause()
        {
            if (currentState == GameState.Paused)
            {
                currentState = previousStateBeforePause;
                Time.timeScale = 1f;
                OnStateChanged?.Invoke(currentState);
            }
            else if (currentState == GameState.FlightCorridor || currentState == GameState.LunarExpedition)
            {
                previousStateBeforePause = currentState;
                currentState = GameState.Paused;
                Time.timeScale = 0f;
                OnStateChanged?.Invoke(currentState);
            }
        }

        private void SaveGameProgress()
        {
            PlayerSaveData save = SaveSystem.Load();
            save.currentMissionIndex++;
            save.astronautRank = "Lunar Explorer Class 1";
            save.samplesCollectedTotal += currentMission.samplesCollected;
            save.bestCorridorAccuracy = Mathf.Max(save.bestCorridorAccuracy, currentMission.corridorAccuracyPercentage);
            SaveSystem.Save(save);
        }
    }
}
