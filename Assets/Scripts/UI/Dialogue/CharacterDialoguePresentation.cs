using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Outpost.UI.Dialogue
{
    /// <summary>
    /// Visual presentation of the active speaker during dialogue.
    /// Manages 2D portraits, expressive emotional swatches, and color-coded name badges.
    /// </summary>
    public class CharacterDialoguePresentation : MonoBehaviour
    {
        [System.Serializable]
        public struct CharacterVisualProfile
        {
            public CharacterIdentifier Identifier;
            public string DisplayName;
            public string RoleTitle;
            public Color IdentityColor;
            public Sprite PortraitNeutral;
            public Sprite PortraitHappy;
            public Sprite PortraitConcerned;
            public Sprite PortraitDetermined;
            public Sprite PortraitSerious;
            public Sprite PortraitAlert;
        }

        [Header("UI References")]
        [SerializeField] private Image _portraitImage;
        [SerializeField] private TextMeshProUGUI _speakerNameText;
        [SerializeField] private TextMeshProUGUI _roleTitleText;
        [SerializeField] private Image _identityColorAccent;
        [SerializeField] private GameObject _educationalBadge;
        [SerializeField] private TextMeshProUGUI _educationalNoteText;

        [Header("Character Registry")]
        [SerializeField] private List<CharacterVisualProfile> _profiles = new List<CharacterVisualProfile>();

        private readonly Dictionary<CharacterIdentifier, CharacterVisualProfile> _profileLookup =
            new Dictionary<CharacterIdentifier, CharacterVisualProfile>();

        private void Awake()
        {
            BuildDefaultProfilesIfEmpty();
            foreach (var prof in _profiles)
            {
                _profileLookup[prof.Identifier] = prof;
            }
        }

        private void BuildDefaultProfilesIfEmpty()
        {
            if (_profiles.Count > 0) return;

            _profiles.Add(new CharacterVisualProfile
            {
                Identifier = CharacterIdentifier.CommanderElena,
                DisplayName = "Commander Elena",
                RoleTitle = "Mission Commander",
                IdentityColor = new Color(0.12f, 0.34f, 0.63f, 1f) // Navy Blue
            });

            _profiles.Add(new CharacterVisualProfile
            {
                Identifier = CharacterIdentifier.EngineerMarcus,
                DisplayName = "Engineer Marcus",
                RoleTitle = "Systems Engineer",
                IdentityColor = new Color(0.96f, 0.53f, 0.12f, 1f) // Amber Orange
            });

            _profiles.Add(new CharacterVisualProfile
            {
                Identifier = CharacterIdentifier.ScientistAisha,
                DisplayName = "Scientist Aisha",
                RoleTitle = "Lunar Geologist & Researcher",
                IdentityColor = new Color(0f, 0.66f, 0.59f, 1f) // Teal
            });

            _profiles.Add(new CharacterVisualProfile
            {
                Identifier = CharacterIdentifier.MedicalDaniel,
                DisplayName = "Medical Officer Daniel",
                RoleTitle = "Life Support & Crew Health",
                IdentityColor = new Color(0.18f, 0.55f, 0.34f, 1f) // Emerald Green
            });

            _profiles.Add(new CharacterVisualProfile
            {
                Identifier = CharacterIdentifier.ORION,
                DisplayName = "ORION AI",
                RoleTitle = "Robotic Operations Network",
                IdentityColor = new Color(0f, 0.96f, 1f, 1f) // Hologram Cyan
            });
        }

        public void DisplaySpeaker(CharacterIdentifier speaker, CharacterExpression expression, string educationalNote = "")
        {
            if (_profileLookup.TryGetValue(speaker, out var profile))
            {
                if (_speakerNameText != null) _speakerNameText.text = profile.DisplayName;
                if (_roleTitleText != null) _roleTitleText.text = profile.RoleTitle;

                if (_identityColorAccent != null) _identityColorAccent.color = profile.IdentityColor;
                if (_speakerNameText != null) _speakerNameText.color = profile.IdentityColor;

                // Select expression portrait
                Sprite targetSprite = expression switch
                {
                    CharacterExpression.Happy => profile.PortraitHappy ?? profile.PortraitNeutral,
                    CharacterExpression.Concerned => profile.PortraitConcerned ?? profile.PortraitNeutral,
                    CharacterExpression.Determined => profile.PortraitDetermined ?? profile.PortraitNeutral,
                    CharacterExpression.Serious => profile.PortraitSerious ?? profile.PortraitNeutral,
                    CharacterExpression.Alert => profile.PortraitAlert ?? profile.PortraitNeutral,
                    _ => profile.PortraitNeutral
                };

                if (_portraitImage != null && targetSprite != null)
                {
                    _portraitImage.sprite = targetSprite;
                    _portraitImage.enabled = true;
                }
            }

            // Educational Concept Note display
            bool hasEducation = !string.IsNullOrEmpty(educationalNote);
            if (_educationalBadge != null) _educationalBadge.SetActive(hasEducation);
            if (_educationalNoteText != null && hasEducation)
            {
                _educationalNoteText.text = $"Science Fact: {educationalNote}";
            }
        }
    }
}
