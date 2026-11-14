using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Outpost.UI.Dialogue
{
    /// <summary>
    /// Presentation component for multi-character group discussions and crew briefings.
    /// Highlights the active speaker while displaying listening astronauts with their emotional reaction expressions.
    /// </summary>
    public class GroupDiscussionPresentation : MonoBehaviour
    {
        [System.Serializable]
        public class CrewSlot
        {
            public CharacterIdentifier Character;
            public GameObject SlotRoot;
            public Image PortraitImage;
            public Image HighlightBorder;
            public TextMeshProUGUI NameLabel;
            public CanvasGroup CanvasGroup;
            public CharacterExpression CurrentExpression = CharacterExpression.Neutral;
        }

        [Header("Crew Slots Gallery")]
        [SerializeField] private List<CrewSlot> _slots = new List<CrewSlot>();

        [Header("Styling")]
        [SerializeField] private float _dimmedAlpha = 0.5f;
        [SerializeField] private float _activeAlpha = 1.0f;
        [SerializeField] private float _activeScale = 1.15f;

        private readonly Dictionary<CharacterIdentifier, CrewSlot> _slotLookup =
            new Dictionary<CharacterIdentifier, CrewSlot>();

        private void Awake()
        {
            foreach (var slot in _slots)
            {
                if (slot != null)
                {
                    _slotLookup[slot.Character] = slot;
                }
            }
        }

        public void SetActiveSpeaker(CharacterIdentifier activeSpeaker, CharacterExpression expression, List<ListenerReaction> reactions = null)
        {
            foreach (var kvp in _slotLookup)
            {
                var slot = kvp.Value;
                bool isSpeaker = (slot.Character == activeSpeaker);

                if (slot.CanvasGroup != null)
                {
                    slot.CanvasGroup.alpha = isSpeaker ? _activeAlpha : _dimmedAlpha;
                }

                if (slot.SlotRoot != null)
                {
                    float scale = isSpeaker ? _activeScale : 1.0f;
                    slot.SlotRoot.transform.localScale = new Vector3(scale, scale, 1f);
                }

                if (slot.HighlightBorder != null)
                {
                    slot.HighlightBorder.enabled = isSpeaker;
                }

                // If this is the active speaker, update their expression
                if (isSpeaker)
                {
                    slot.CurrentExpression = expression;
                }
                else if (reactions != null)
                {
                    // Check if a specific listener reaction was supplied
                    var match = reactions.Find(r => r.Listener == slot.Character);
                    if (match.Listener == slot.Character)
                    {
                        slot.CurrentExpression = match.Expression;
                    }
                }
            }
        }

        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }
    }
}
