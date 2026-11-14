using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Outpost.UI.Core;

namespace Outpost.UI.Menus
{
    /// <summary>
    /// Credits presentation panel for Outpost: Survive Beyond Earth.
    /// Honors the astronaut crew, ORION AI, historical spaceflight inspiration (Apollo 13), and mission contributors.
    /// </summary>
    public class CreditsUI : UIView
    {
        [Header("UI Elements")]
        [SerializeField] private Button _backButton;
        [SerializeField] private TextMeshProUGUI _creditsContentText;
        [SerializeField] private ScrollRect _scrollRect;

        [Header("Scroll Animation")]
        [SerializeField] private bool _autoScroll = true;
        [SerializeField] private float _scrollSpeed = 25f;

        private bool _isScrolling = false;

        protected override void Awake()
        {
            base.Awake();
            if (_backButton != null)
            {
                _backButton.onClick.AddListener(OnBackClicked);
            }
            PopulateCreditsTextIfEmpty();
        }

        public override void Show(bool immediate = false, Action onComplete = null)
        {
            base.Show(immediate, onComplete);
            if (_scrollRect != null) _scrollRect.verticalNormalizedPosition = 1f;
            _isScrolling = _autoScroll;
        }

        private void Update()
        {
            if (_isScrolling && _scrollRect != null)
            {
                if (_scrollRect.verticalNormalizedPosition > 0f)
                {
                    _scrollRect.verticalNormalizedPosition -= (_scrollSpeed * Time.unscaledDeltaTime) * 0.05f;
                }
                else
                {
                    _isScrolling = false;
                }
            }
        }

        private void OnBackClicked()
        {
            _isScrolling = false;
            Hide(false, () =>
            {
                UIManager.Instance?.ChangeState(UIState.MainMenu);
            });
        }

        private void PopulateCreditsTextIfEmpty()
        {
            if (_creditsContentText == null || !string.IsNullOrEmpty(_creditsContentText.text)) return;

            _creditsContentText.text =
                "<b>OUTPOST: SURVIVE BEYOND EARTH</b>\n" +
                "<size=80%>MISSION 1: THE FIRST FRONTIER (2045)</size>\n\n" +
                "<b>THE ASTRONAUT CREW</b>\n" +
                "Commander Elena — Mission Commander & Emergency Lead\n" +
                "Engineer Marcus — Systems & Mechanical Engineer\n" +
                "Scientist Aisha — Lunar Geologist & Environmental Analyst\n" +
                "Medical Officer Daniel — Life Support & Crew Health\n\n" +
                "<b>ARTIFICIAL INTELLIGENCE</b>\n" +
                "ORION — Onboard Robotic Intelligence & Operations Network\n\n" +
                "<b>HISTORICAL INSPIRATION</b>\n" +
                "NASA Apollo Program & The Heroic Engineering of Apollo 13 (April 1970)\n" +
                "Artemis Lunar Exploration Architecture\n\n" +
                "<b>DEVELOPMENT TEAM</b>\n" +
                "System 1 — Gameplay, Physics & Mission Logic\n" +
                "System 2 — 3D Environment, Characters & Art\n" +
                "System 3 — UI, Dialogue, ORION, Geo Map & Audio Engine\n\n" +
                "<size=70%>Designed for future space explorers aged 7–12.</size>";
        }
    }
}
