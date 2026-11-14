using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Outpost.UI.Audio
{
    /// <summary>
    /// Visual indicator and gesture listener for WebGL browser autoplay policy.
    /// Informs young players to click or tap anywhere to enable audio, then cleanly disappears.
    /// </summary>
    public class AudioAutoplayGuard : MonoBehaviour
    {
        [Header("Banner UI Elements")]
        [SerializeField] private GameObject _promptBanner;
        [SerializeField] private Button _interactionButton;
        [SerializeField] private TextMeshProUGUI _promptText;

        private void Awake()
        {
            if (_interactionButton != null)
            {
                _interactionButton.onClick.AddListener(OnInteraction);
            }

            if (_promptText != null && string.IsNullOrEmpty(_promptText.text))
            {
                _promptText.text = "<b>🔊 CLICK TO ENABLE OUTPOST SOUND</b>";
            }
        }

        private void Start()
        {
            CheckAudioStatus();
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.OnAudioUnlocked += HandleAudioUnlocked;
            }
        }

        private void OnDestroy()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.OnAudioUnlocked -= HandleAudioUnlocked;
            }
        }

        private void CheckAudioStatus()
        {
            if (AudioManager.Instance == null) return;

            bool isUnlocked = AudioManager.Instance.IsAudioUnlocked;
            if (_promptBanner != null)
            {
                _promptBanner.SetActive(!isUnlocked);
            }
        }

        private void OnInteraction()
        {
            AudioManager.Instance?.UnlockAudio();
        }

        private void HandleAudioUnlocked()
        {
            if (_promptBanner != null)
            {
                _promptBanner.SetActive(false);
            }
        }
    }
}
