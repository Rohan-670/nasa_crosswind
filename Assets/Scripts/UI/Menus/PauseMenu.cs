using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Outpost.UI.Core;

namespace Outpost.UI.Menus
{
    /// <summary>
    /// Pause screen accessible anytime during gameplay with [ESC].
    /// Displays pause indicator, allows resume, settings, and return to main menu.
    /// </summary>
    public class PauseMenu : UIView
    {
        [Header("Pause Header")]
        [SerializeField] private TextMeshProUGUI _pauseIndicatorText;
        [SerializeField] private TextMeshProUGUI _currentChapterText;

        [Header("Menu Buttons")]
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _mainMenuButton;

        protected override void Awake()
        {
            base.Awake();
            InitializeListeners();
            if (_pauseIndicatorText != null && string.IsNullOrEmpty(_pauseIndicatorText.text))
            {
                _pauseIndicatorText.text = "MISSION PAUSED";
            }
            if (_currentChapterText != null && string.IsNullOrEmpty(_currentChapterText.text))
            {
                _currentChapterText.text = "Mission 1: The First Frontier (Lunar South Pole)";
            }
        }

        public override void Show(bool immediate = false, Action onComplete = null)
        {
            base.Show(immediate, onComplete);
            UIEventBus.PublishPauseToggled(true);
        }

        public override void Hide(bool immediate = false, Action onComplete = null)
        {
            base.Hide(immediate, onComplete);
            UIEventBus.PublishPauseToggled(false);
        }

        private void InitializeListeners()
        {
            if (_resumeButton != null)
            {
                _resumeButton.onClick.AddListener(ResumeGame);
            }
            if (_settingsButton != null)
            {
                _settingsButton.onClick.AddListener(() =>
                {
                    UIManager.Instance?.ChangeState(UIState.Settings);
                });
            }
            if (_mainMenuButton != null)
            {
                _mainMenuButton.onClick.AddListener(ReturnToMainMenu);
            }
        }

        public void ResumeGame()
        {
            UIManager.Instance?.ChangeState(UIState.GameplayHUD);
        }

        public void ReturnToMainMenu()
        {
            UIManager.Instance?.ChangeState(UIState.MainMenu);
        }
    }
}
