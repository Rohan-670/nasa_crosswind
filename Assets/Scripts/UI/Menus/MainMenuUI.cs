using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Outpost.UI.Core;

namespace Outpost.UI.Menus
{
    /// <summary>
    /// Master entry screen for Outpost: Survive Beyond Earth.
    /// Manages Start Mission, Settings, Credits, Crew Dossier, and Loading Screen transitions.
    /// </summary>
    public class MainMenuUI : UIView
    {
        [Header("Menu Title")]
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _subtitleText;

        [Header("Menu Buttons")]
        [SerializeField] private Button _startMissionButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _creditsButton;
        [SerializeField] private Button _crewDossierButton;
        [SerializeField] private Button _quitButton;

        [Header("Sub-Screens")]
        [SerializeField] private LoadingScreenUI _loadingScreen;
        [SerializeField] private CreditsUI _creditsUI;
        [SerializeField] private GameObject _dossierPanel;
        [SerializeField] private Button _closeDossierButton;

        protected override void Awake()
        {
            base.Awake();
            InitializeListeners();
            if (_titleText != null && string.IsNullOrEmpty(_titleText.text))
            {
                _titleText.text = "OUTPOST: SURVIVE BEYOND EARTH";
            }
            if (_subtitleText != null && string.IsNullOrEmpty(_subtitleText.text))
            {
                _subtitleText.text = "MISSION 1: THE FIRST FRONTIER";
            }
            if (_dossierPanel != null) _dossierPanel.SetActive(false);
        }

        private void InitializeListeners()
        {
            if (_startMissionButton != null)
            {
                _startMissionButton.onClick.AddListener(OnStartMissionClicked);
            }
            if (_settingsButton != null)
            {
                _settingsButton.onClick.AddListener(() => UIManager.Instance?.ChangeState(UIState.Settings));
            }
            if (_creditsButton != null)
            {
                _creditsButton.onClick.AddListener(OnCreditsClicked);
            }
            if (_crewDossierButton != null)
            {
                _crewDossierButton.onClick.AddListener(() => _dossierPanel?.SetActive(true));
            }
            if (_closeDossierButton != null)
            {
                _closeDossierButton.onClick.AddListener(() => _dossierPanel?.SetActive(false));
            }
            if (_quitButton != null)
            {
                _quitButton.onClick.AddListener(() => Application.Quit());
            }
        }

        private void OnStartMissionClicked()
        {
            Hide(false, () =>
            {
                if (_loadingScreen != null)
                {
                    _loadingScreen.SimulateLoading(2.5f, () =>
                    {
                        UIManager.Instance?.ChangeState(UIState.GameplayHUD);
                    });
                }
                else
                {
                    UIManager.Instance?.ChangeState(UIState.GameplayHUD);
                }
            });
        }

        private void OnCreditsClicked()
        {
            if (_creditsUI != null)
            {
                Hide(false, () => _creditsUI.Show());
            }
            else
            {
                Debug.Log("[MainMenuUI] Credits screen requested.");
            }
        }
    }
}
