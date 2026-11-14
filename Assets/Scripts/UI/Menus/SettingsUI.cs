using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Outpost.UI.Core;

namespace Outpost.UI.Menus
{
    /// <summary>
    /// Settings and accessibility interface.
    /// Supports volume controls, subtitle toggles, text sizing, and high-contrast modes for ages 7-12.
    /// </summary>
    public class SettingsUI : UIView
    {
        [Header("Audio Sliders")]
        [SerializeField] private Slider _masterVolumeSlider;
        [SerializeField] private Slider _musicVolumeSlider;
        [SerializeField] private Slider _sfxVolumeSlider;
        [SerializeField] private Slider _voiceVolumeSlider;

        [Header("Accessibility Toggles")]
        [SerializeField] private Toggle _subtitlesToggle;
        [SerializeField] private Toggle _highContrastToggle;
        [SerializeField] private TMP_Dropdown _textSizeDropdown;

        [Header("Close Button")]
        [SerializeField] private Button _backButton;

        protected override void Awake()
        {
            base.Awake();
            InitializeListeners();
            LoadPreferences();
        }

        private void InitializeListeners()
        {
            if (_masterVolumeSlider != null)
                _masterVolumeSlider.onValueChanged.AddListener(val => AudioListener.volume = val);

            if (_backButton != null)
            {
                _backButton.onClick.AddListener(() =>
                {
                    UIManager.Instance?.ChangeState(UIState.GameplayHUD);
                });
            }
        }

        private void LoadPreferences()
        {
            if (_masterVolumeSlider != null) _masterVolumeSlider.value = PlayerPrefs.GetFloat("Vol_Master", 1.0f);
            if (_musicVolumeSlider != null) _musicVolumeSlider.value = PlayerPrefs.GetFloat("Vol_Music", 0.8f);
            if (_sfxVolumeSlider != null) _sfxVolumeSlider.value = PlayerPrefs.GetFloat("Vol_SFX", 0.9f);
            if (_voiceVolumeSlider != null) _voiceVolumeSlider.value = PlayerPrefs.GetFloat("Vol_Voice", 1.0f);

            if (_subtitlesToggle != null)
                _subtitlesToggle.isOn = PlayerPrefs.GetInt("Pref_Subtitles", 1) == 1;
        }

        public void SavePreferences()
        {
            if (_masterVolumeSlider != null) PlayerPrefs.SetFloat("Vol_Master", _masterVolumeSlider.value);
            if (_musicVolumeSlider != null) PlayerPrefs.SetFloat("Vol_Music", _musicVolumeSlider.value);
            if (_sfxVolumeSlider != null) PlayerPrefs.SetFloat("Vol_SFX", _sfxVolumeSlider.value);
            if (_voiceVolumeSlider != null) PlayerPrefs.SetFloat("Vol_Voice", _voiceVolumeSlider.value);

            if (_subtitlesToggle != null)
                PlayerPrefs.SetInt("Pref_Subtitles", _subtitlesToggle.isOn ? 1 : 0);

            PlayerPrefs.Save();
        }
    }
}
