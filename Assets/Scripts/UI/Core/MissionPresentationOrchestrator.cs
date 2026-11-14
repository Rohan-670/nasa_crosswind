using System;
using System.Collections;
using UnityEngine;
using Outpost.UI.Cinematics;
using Outpost.UI.Dialogue;
using Outpost.UI.HUD;
using Outpost.UI.Map;
using Outpost.UI.Menus;
using Outpost.UI.ORION;
using Outpost.UI.Emergency;

namespace Outpost.UI.Core
{
    public enum MissionChapterStage
    {
        NotStarted = 0,
        Chapter1_Arrival = 1,
        Chapter2_OutpostEstablishment = 2,
        Chapter3_TheFirstWarning = 3,
        Chapter4_LifeSupportEmergency = 4,
        Chapter5_TheFinalDecision = 5,
        Chapter6_Recovery = 6,
        Chapter7_MissionComplete = 7
    }

    /// <summary>
    /// Master Mission Story and Presentation Orchestrator for System 3.
    /// Drives the end-to-end presentation flow of Mission 1: The First Frontier across all 7 Chapters.
    /// Coordinates cinematics (with skip), group discussions, tutorials, emergency UI, and educational scoring.
    /// </summary>
    public class MissionPresentationOrchestrator : MonoBehaviour
    {
        public static MissionPresentationOrchestrator Instance { get; private set; }

        [Header("Mission Flow Status")]
        [SerializeField] private MissionChapterStage _currentStage = MissionChapterStage.NotStarted;
        [SerializeField] private EmergencyRepairOption _chosenEmergencyOption = EmergencyRepairOption.None;

        [Header("Cinematic & Presentation Engines")]
        [SerializeField] private CinematicPresentation _cinematics;
        [SerializeField] private DialogueManager _dialogueManager;
        [SerializeField] private HUDController _hud;
        [SerializeField] private LunarMapUI _map;
        [SerializeField] private ORIONInterface _orion;

        [Header("Chapter 2 Activation & Tutorials")]
        [SerializeField] private TutorialOverlayUI _tutorialOverlay;
        [SerializeField] private StationActivationPresentation _stationActivation;

        [Header("Chapter 4 & 5 Emergency Suite")]
        [SerializeField] private EmergencyAlertPanel _alertPanel;
        [SerializeField] private LifeSupportDiagnosticScreen _diagnosticScreen;
        [SerializeField] private FiltrationChallengeUI _filtrationChallenge;
        [SerializeField] private EmergencyDecisionModal _decisionModal;

        [Header("Chapter 7 Completion Screens")]
        [SerializeField] private EducationalSummaryModal _educationalSummary;
        [SerializeField] private MissionPerformanceScreen _performanceScreen;
        [SerializeField] private CreditsUI _credits;

        public MissionChapterStage CurrentStage => _currentStage;
        public EmergencyRepairOption ChosenEmergencyOption => _chosenEmergencyOption;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void OnEnable()
        {
            UIEventBus.OnEmergencyOptionSelected += HandleEmergencyOptionSelected;
        }

        private void OnDisable()
        {
            UIEventBus.OnEmergencyOptionSelected -= HandleEmergencyOptionSelected;
        }

        private void HandleEmergencyOptionSelected(EmergencyRepairOption option)
        {
            _chosenEmergencyOption = option;
        }

        // ==========================================
        // CHAPTER 1: ARRIVAL ON THE MOON
        // ==========================================
        public void StartChapter1Arrival(Action onComplete = null)
        {
            _currentStage = MissionChapterStage.Chapter1_Arrival;
            UIEventBus.PublishObjective("M1_CH1_OBJ", "Inspect landing module and initiate crew check.");

            // 1. Play introductory cinematic (skippable)
            if (_cinematics != null)
            {
                _cinematics.PlayArrivalCinematic(() =>
                {
                    // 2. Introduce ORION and crew conversation
                    _dialogueManager?.StartConversationById("Mission1_Ch1_Arrival", () =>
                    {
                        _orion?.PostMessage(new ORIONMessage
                        {
                            Category = ORIONMessageType.Status,
                            Header = "ORION ONLINE",
                            Body = "Environmental telemetry established. All landing module seals nominal. Welcome to the lunar south pole.",
                            Priority = AlertLevel.Nominal
                        });

                        onComplete?.Invoke();
                    });
                });
            }
            else
            {
                _dialogueManager?.StartConversationById("Mission1_Ch1_Arrival", onComplete);
            }
        }

        // ==========================================
        // CHAPTER 2: OUTPOST ESTABLISHMENT
        // ==========================================
        public void StartChapter2OutpostEstablishment(Action onComplete = null)
        {
            _currentStage = MissionChapterStage.Chapter2_OutpostEstablishment;
            UIEventBus.PublishObjective("M1_CH2_OBJ", "Activate solar arrays, inspect habitat pressure locks, and review station map.");

            // Sequence: Solar Activation Presentation -> Habitat Inspection -> Greenhouse -> Tutorials
            if (_stationActivation != null)
            {
                _stationActivation.PresentSolarArrayActivation(() =>
                {
                    _dialogueManager?.StartConversationById("Mission1_Ch2_HabitatInspection", () =>
                    {
                        _stationActivation.PresentHabitatInspection(() =>
                        {
                            _dialogueManager?.StartConversationById("Mission1_Ch2_GreenhouseIntro", () =>
                            {
                                _stationActivation.PresentGreenhouseInitiation(() =>
                                {
                                    // Start Map & Resource Tutorials
                                    StartChapter2Tutorials(onComplete);
                                });
                            });
                        });
                    });
                });
            }
            else
            {
                StartChapter2Tutorials(onComplete);
            }
        }

        private void StartChapter2Tutorials(Action onComplete)
        {
            if (_tutorialOverlay != null)
            {
                _tutorialOverlay.StartTutorial(TutorialType.MapNavigation, () =>
                {
                    _tutorialOverlay.StartTutorial(TutorialType.ResourceDashboard, onComplete);
                });
            }
            else
            {
                onComplete?.Invoke();
            }
        }

        // ==========================================
        // CHAPTER 3: THE FIRST WARNING
        // ==========================================
        public void StartChapter3TheFirstWarning(Action onComplete = null)
        {
            _currentStage = MissionChapterStage.Chapter3_FirstWarning;
            UIEventBus.PublishObjective("M1_CH3_OBJ", "Monitor solar telemetry and prepare external equipment for solar flare activity.");

            // 1. Scene 1: Solar activity briefing
            _dialogueManager?.StartConversationById("Mission1_Scene1_SolarWarning", () =>
            {
                // 2. Scene 2: Before the impact
                _dialogueManager?.StartConversationById("Mission1_Scene2_BeforeImpact", () =>
                {
                    // 3. Micrometeoroid impact cinematic
                    if (_cinematics != null)
                    {
                        _cinematics.PlayImpactCinematic(() =>
                        {
                            // 4. Scene 3: Impact detected
                            _dialogueManager?.StartConversationById("Mission1_Scene3_MicrometeoroidIncident", onComplete);
                        });
                    }
                    else
                    {
                        _dialogueManager?.StartConversationById("Mission1_Scene3_MicrometeoroidIncident", onComplete);
                    }
                });
            });
        }

        // ==========================================
        // CHAPTER 4: LIFE-SUPPORT EMERGENCY
        // ==========================================
        public void StartChapter4LifeSupportEmergency(Action onComplete = null)
        {
            _currentStage = MissionChapterStage.Chapter4_LifeSupportEmergency;
            UIEventBus.PublishObjective("M1_CH4_OBJ", "Diagnose life support failure, stabilize CO2 levels, and build emergency filtration adapter.");

            // 1. Scene 4: Life Support Emergency Dialogue
            _dialogueManager?.StartConversationById("Mission1_Scene4_LifeSupportEmergency", () =>
            {
                // 2. Diagnostic Screen
                _diagnosticScreen?.OpenDiagnostic(() =>
                {
                    // 3. Scene 5: Filtration Challenge Briefing
                    _dialogueManager?.StartConversationById("Mission1_Scene5_FiltrationChallenge", () =>
                    {
                        // 4. Filtration Puzzle UI
                        _filtrationChallenge?.StartChallenge((success) =>
                        {
                            onComplete?.Invoke();
                        });
                    });
                });
            });
        }

        // ==========================================
        // CHAPTER 5: THE FINAL DECISION
        // ==========================================
        public void StartChapter5TheFinalDecision(Action<EmergencyRepairOption> onComplete = null)
        {
            _currentStage = MissionChapterStage.Chapter5_FinalDecision;
            UIEventBus.PublishObjective("M1_CH5_OBJ", "Evaluate the three recovery strategies and choose the safest plan.");

            // 1. Scene 6: Final Decision Dialogue & Choice Presentation
            _dialogueManager?.StartConversationById("Mission1_Scene6_FinalDecision", () =>
            {
                // If decision modal is open, let player confirm
                if (_decisionModal != null)
                {
                    _decisionModal.OpenDecisionModal((option) =>
                    {
                        _chosenEmergencyOption = option;
                        onComplete?.Invoke(option);
                    });
                }
                else
                {
                    onComplete?.Invoke(_chosenEmergencyOption);
                }
            });
        }

        // ==========================================
        // CHAPTER 6: RECOVERY
        // ==========================================
        public void StartChapter6Recovery(Action onComplete = null)
        {
            _currentStage = MissionChapterStage.Chapter6_Recovery;
            UIEventBus.PublishObjective("M1_CH6_OBJ", "Verify grid restoration and confirm crew safety.");

            // 1. Recovery cinematic (skippable)
            if (_cinematics != null)
            {
                _cinematics.PlayRecoveryCinematic(() =>
                {
                    // 2. Scene 7: Recovery conversation
                    _dialogueManager?.StartConversationById("Mission1_Scene7_Recovery", () =>
                    {
                        _orion?.PostMessage(new ORIONMessage
                        {
                            Category = ORIONMessageType.Status,
                            Header = "STATION RECOVERED",
                            Body = "Primary electrical bus online. Habitat CO2 stabilized at 450 ppm. All crew vitals nominal.",
                            Priority = AlertLevel.Nominal
                        });

                        onComplete?.Invoke();
                    });
                });
            }
            else
            {
                _dialogueManager?.StartConversationById("Mission1_Scene7_Recovery", onComplete);
            }
        }

        // ==========================================
        // CHAPTER 7: MISSION COMPLETE
        // ==========================================
        public void StartChapter7MissionComplete(Action onComplete = null)
        {
            _currentStage = MissionChapterStage.Chapter7_MissionComplete;
            UIEventBus.PublishObjective("M1_CH7_OBJ", "Mission 1 debriefing and educational wrap-up.");

            // 1. Final character debriefing dialogue
            _dialogueManager?.StartConversationById("Mission1_Ch7_MissionComplete", () =>
            {
                // 2. Educational Summary Modal
                if (_educationalSummary != null)
                {
                    _educationalSummary.DisplayEducationalSummary(() =>
                    {
                        // 3. Mission Performance Screen
                        if (_performanceScreen != null)
                        {
                            _performanceScreen.DisplayEvaluation(_chosenEmergencyOption, () =>
                            {
                                // 4. Roll Credits
                                _credits?.Show(false, onComplete);
                            });
                        }
                        else
                        {
                            _credits?.Show(false, onComplete);
                        }
                    });
                }
                else
                {
                    _credits?.Show(false, onComplete);
                }
            });
        }
    }
}
