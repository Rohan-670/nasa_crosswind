using System;
using System.Collections.Generic;
using UnityEngine;
using Outpost.UI.HUD;
using Outpost.UI.Dialogue;
using Outpost.UI.Map;
using Outpost.UI.Menus;
using Outpost.UI.Notifications;
using Outpost.UI.Cinematics;

namespace Outpost.UI.Core
{
    /// <summary>
    /// Master UI Manager for System 3.
    /// Controls UI lifecycle, coordinates views, handles WebGL keyboard shortcuts, and enforces single-point UI management.
    /// </summary>
    public class UIManager : MonoBehaviour, IUIManager
    {
        public static UIManager Instance { get; private set; }

        [Header("Managed Views")]
        [SerializeField] private MainMenuUI _mainMenuUI;
        [SerializeField] private HUDController _hudController;
        [SerializeField] private DialogueManager _dialogueManager;
        [SerializeField] private LunarMapUI _lunarMapUI;
        [SerializeField] private PauseMenu _pauseMenu;
        [SerializeField] private SettingsUI _settingsUI;
        [SerializeField] private MissionNotificationUI _notificationUI;
        [SerializeField] private CinematicPresentation _cinematicPresentation;
        [SerializeField] private TutorialOverlayUI _tutorialOverlay;
        [SerializeField] private StationActivationPresentation _stationActivation;
        [SerializeField] private EducationalSummaryModal _educationalSummary;
        [SerializeField] private MissionPerformanceScreen _performanceScreen;

        [Header("Initial Configuration")]
        [SerializeField] private UIState _startingState = UIState.GameplayHUD;

        private UIState _currentState = UIState.None;
        private UIState _previousState = UIState.None;

        public UIState CurrentState => _currentState;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            ChangeState(_startingState);
        }

        private void Update()
        {
            HandleGlobalShortcuts();
        }

        private void HandleGlobalShortcuts()
        {
            // Keyboard navigation shortcuts [M] Map, [ESC] Pause
            if (Input.GetKeyDown(KeyCode.M))
            {
                if (_currentState == UIState.GameplayHUD)
                {
                    ChangeState(UIState.InteractiveMap);
                }
                else if (_currentState == UIState.InteractiveMap)
                {
                    ChangeState(UIState.GameplayHUD);
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (_currentState == UIState.InteractiveMap || _currentState == UIState.Settings)
                {
                    ChangeState(UIState.GameplayHUD);
                }
                else if (_currentState == UIState.GameplayHUD)
                {
                    ChangeState(UIState.PauseMenu);
                }
                else if (_currentState == UIState.PauseMenu)
                {
                    ChangeState(UIState.GameplayHUD);
                }
            }
        }

        public void ChangeState(UIState newState)
        {
            if (_currentState == newState) return;

            _previousState = _currentState;
            _currentState = newState;

            // Update visible views according to new state
            UpdateViewVisibilities();

            // Notify external listeners through event bus
            UIEventBus.PublishUIStateChanged(newState);
        }

        private void UpdateViewVisibilities()
        {
            switch (_currentState)
            {
                case UIState.MainMenu:
                    _mainMenuUI?.Show();
                    _hudController?.Hide();
                    _lunarMapUI?.Hide();
                    _pauseMenu?.Hide();
                    _settingsUI?.Hide();
                    Time.timeScale = 1f;
                    break;

                case UIState.GameplayHUD:
                    _mainMenuUI?.Hide();
                    _hudController?.Show();
                    _lunarMapUI?.Hide();
                    _pauseMenu?.Hide();
                    _settingsUI?.Hide();
                    Time.timeScale = 1f;
                    break;

                case UIState.InteractiveMap:
                    _lunarMapUI?.Show();
                    _pauseMenu?.Hide();
                    _settingsUI?.Hide();
                    break;

                case UIState.PauseMenu:
                    _pauseMenu?.Show();
                    _lunarMapUI?.Hide();
                    _settingsUI?.Hide();
                    Time.timeScale = 0f;
                    break;

                case UIState.Settings:
                    _settingsUI?.Show();
                    break;

                case UIState.Dialogue:
                    _hudController?.Show();
                    _lunarMapUI?.Hide();
                    _pauseMenu?.Hide();
                    break;

                case UIState.Cinematic:
                    _hudController?.Hide();
                    _lunarMapUI?.Hide();
                    _pauseMenu?.Hide();
                    break;
            }
        }

        public void ToggleMap()
        {
            if (_currentState == UIState.InteractiveMap)
                ChangeState(UIState.GameplayHUD);
            else if (_currentState == UIState.GameplayHUD)
                ChangeState(UIState.InteractiveMap);
        }

        public void TogglePause()
        {
            if (_currentState == UIState.PauseMenu)
                ChangeState(UIState.GameplayHUD);
            else if (_currentState == UIState.GameplayHUD)
                ChangeState(UIState.PauseMenu);
        }

        public void ShowNotification(string title, string message, NotificationType type)
        {
            _notificationUI?.DisplayNotification(title, message, type);
        }

        public void SetAlertLevel(AlertLevel level, string reason)
        {
            UIEventBus.PublishAlertLevel(level, reason);
        }

        public TutorialOverlayUI TutorialOverlay => _tutorialOverlay;
        public StationActivationPresentation StationActivation => _stationActivation;
        public EducationalSummaryModal EducationalSummary => _educationalSummary;
        public MissionPerformanceScreen PerformanceScreen => _performanceScreen;
        public CinematicPresentation Cinematics => _cinematicPresentation;
        public DialogueManager Dialogues => _dialogueManager;
        public LunarMapUI Map => _lunarMapUI;
        public HUDController HUD => _hudController;
    }
}
