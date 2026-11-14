using System;
using UnityEngine;
using Outpost.Core.Data;
using Outpost.Core.Events;

namespace Outpost.System1.Gameplay.Core
{
    public enum GameplayState
    {
        Initializing,
        LandedArrival,          // Chapter 1
        OutpostEstablishment,   // Chapter 2
        SolarWarning,           // Chapter 3
        EmergencyLifeSupport,   // Chapter 4
        FiltrationPuzzle,       // Chapter 5
        StrategicDecision,      // Chapter 6
        MissionComplete,        // Chapter 7
        Paused
    }

    /// <summary>
    /// Central Gameplay Manager for System 1.
    /// Orchestrates session lifecycle, pauses, state transitions, and subsystems.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Header("State Settings")]
        [SerializeField] private GameplayState currentState = GameplayState.Initializing;

        [Header("Subsystem References")]
        [SerializeField] private Player.PlayerController playerController;
        [SerializeField] private Player.CameraController cameraController;
        [SerializeField] private Interactions.InteractionManager interactionManager;
        [SerializeField] private Resources.ResourceManager resourceManager;
        [SerializeField] private Equipment.EquipmentManager equipmentManager;
        [SerializeField] private Emergency.EmergencyManager emergencyManager;
        [SerializeField] private Mission.MissionManager missionManager;
        [SerializeField] private Persistence.SaveManager saveManager;

        public GameplayState CurrentState => currentState;
        public bool IsPaused => currentState == GameplayState.Paused;

        public event Action<GameplayState> OnGameStateChanged;

        public static GameManager Instance { get; private set; }

        private GameplayState prePauseState;

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
                return;
            }

            LocateSubsystems();
        }

        private void Start()
        {
            SetState(GameplayState.LandedArrival);
        }

        private void OnEnable()
        {
            OutpostEventChannel.ChapterChanged += HandleChapterChanged;
            OutpostEventChannel.GeoMapToggled += HandleGeoMapToggled;
            OutpostEventChannel.FiltrationChallengeOpened += HandleFiltrationPuzzleOpened;
            OutpostEventChannel.FiltrationChallengeCompleted += HandleFiltrationPuzzleCompleted;
            OutpostEventChannel.StrategicChoiceOpened += HandleStrategicChoiceOpened;
            OutpostEventChannel.RepairChoiceMade += HandleRepairChoiceMade;
        }

        private void OnDisable()
        {
            OutpostEventChannel.ChapterChanged -= HandleChapterChanged;
            OutpostEventChannel.GeoMapToggled -= HandleGeoMapToggled;
            OutpostEventChannel.FiltrationChallengeOpened -= HandleFiltrationPuzzleOpened;
            OutpostEventChannel.FiltrationChallengeCompleted -= HandleFiltrationPuzzleCompleted;
            OutpostEventChannel.StrategicChoiceOpened -= HandleStrategicChoiceOpened;
            OutpostEventChannel.RepairChoiceMade -= HandleRepairChoiceMade;
        }

        private void LocateSubsystems()
        {
            if (playerController == null) playerController = FindAnyObjectByType<Player.PlayerController>();
            if (cameraController == null) cameraController = FindAnyObjectByType<Player.CameraController>();
            if (interactionManager == null) interactionManager = FindAnyObjectByType<Interactions.InteractionManager>();
            if (resourceManager == null) resourceManager = FindAnyObjectByType<Resources.ResourceManager>();
            if (equipmentManager == null) equipmentManager = FindAnyObjectByType<Equipment.EquipmentManager>();
            if (emergencyManager == null) emergencyManager = FindAnyObjectByType<Emergency.EmergencyManager>();
            if (missionManager == null) missionManager = FindAnyObjectByType<Mission.MissionManager>();
            if (saveManager == null) saveManager = FindAnyObjectByType<Persistence.SaveManager>();
        }

        public void SetState(GameplayState newState)
        {
            if (currentState == newState) return;

            currentState = newState;
            OnGameStateChanged?.Invoke(newState);

            switch (newState)
            {
                case GameplayState.FiltrationPuzzle:
                case GameplayState.StrategicDecision:
                case GameplayState.Paused:
                    SetControlsEnabled(false);
                    break;
                default:
                    SetControlsEnabled(true);
                    break;
            }
        }

        public void TogglePause()
        {
            if (currentState == GameplayState.Paused)
            {
                Time.timeScale = 1.0f;
                SetState(prePauseState);
            }
            else
            {
                prePauseState = currentState;
                Time.timeScale = 0.0f;
                SetState(GameplayState.Paused);
            }
        }

        public void SetControlsEnabled(bool enabled)
        {
            if (playerController != null) playerController.SetControlLock(!enabled);
            if (cameraController != null) cameraController.SetLock(!enabled);
            if (interactionManager != null) interactionManager.enabled = enabled;

            Cursor.lockState = enabled ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !enabled;
        }

        private void HandleChapterChanged(MissionChapterId id, string title, string obj)
        {
            switch (id)
            {
                case MissionChapterId.Chapter1_ArrivalOnTheMoon:
                    SetState(GameplayState.LandedArrival);
                    break;
                case MissionChapterId.Chapter2_EstablishingTheOutpost:
                    SetState(GameplayState.OutpostEstablishment);
                    break;
                case MissionChapterId.Chapter3_TheFirstWarning:
                    SetState(GameplayState.SolarWarning);
                    break;
                case MissionChapterId.Chapter4_TheLifeSupportEmergency:
                    SetState(GameplayState.EmergencyLifeSupport);
                    break;
                case MissionChapterId.Chapter5_TheFiltrationChallenge:
                    SetState(GameplayState.FiltrationPuzzle);
                    break;
                case MissionChapterId.Chapter6_SaveTheOutpost:
                    SetState(GameplayState.StrategicDecision);
                    break;
                case MissionChapterId.Chapter7_ANewBeginning:
                    SetState(GameplayState.MissionComplete);
                    break;
            }
        }

        private void HandleGeoMapToggled(bool open)
        {
            SetControlsEnabled(!open);
        }

        private void HandleFiltrationPuzzleOpened()
        {
            SetState(GameplayState.FiltrationPuzzle);
        }

        private void HandleFiltrationPuzzleCompleted(bool _)
        {
            SetState(GameplayState.StrategicDecision);
        }

        private void HandleStrategicChoiceOpened()
        {
            SetState(GameplayState.StrategicDecision);
        }

        private void HandleRepairChoiceMade(RepairChoice _)
        {
            SetState(GameplayState.MissionComplete);
        }
    }
}
