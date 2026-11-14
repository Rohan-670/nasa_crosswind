using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Outpost.UI.Core;

namespace Outpost.UI.Dialogue
{
    /// <summary>
    /// Master dialogue orchestrator for System 3.
    /// Manages two-person and multi-character group conversations, emotional expressions,
    /// camera framing directions for Computer 2, branching choices, and completion events.
    /// Guarantees strictly no overlapping conversations via a thread-safe / coroutine-safe FIFO queue.
    /// </summary>
    public class DialogueManager : UIView, IDialogueManager
    {
        [Header("Components")]
        [SerializeField] private CharacterDialoguePresentation _characterPresentation;
        [SerializeField] private GroupDiscussionPresentation _groupPresentation;
        [SerializeField] private SubtitleSystem _subtitleSystem;
        [SerializeField] private ConversationSequenceManager _sequenceManager;

        [Header("Choices UI")]
        [SerializeField] private GameObject _choicesContainer;
        [SerializeField] private Button[] _choiceButtons;
        [SerializeField] private TextMeshProUGUI[] _choiceTexts;
        [SerializeField] private TextMeshProUGUI[] _choiceHintTexts;

        [Header("Next Indicator")]
        [SerializeField] private GameObject _continueIndicator;

        private struct QueuedConversation
        {
            public ConversationData Conversation;
            public Action OnComplete;
        }

        private readonly Queue<QueuedConversation> _conversationQueue = new Queue<QueuedConversation>();

        private ConversationData _currentConversation;
        private int _currentLineIndex = -1;
        private Action _onConversationCompleteCallback;
        private bool _isWaitingForInput = false;

        public bool IsDialogueActive => _currentConversation != null;
        public int QueuedCount => _conversationQueue.Count;

        protected override void Awake()
        {
            base.Awake();
            if (_choicesContainer != null) _choicesContainer.SetActive(false);
            if (_continueIndicator != null) _continueIndicator.SetActive(false);
            if (_groupPresentation != null) _groupPresentation.SetVisible(false);
        }

        private void Update()
        {
            if (!IsDialogueActive) return;

            // Advance dialogue on Space or Left Mouse Click if not waiting for a branching choice
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                if (_subtitleSystem != null && _subtitleSystem.IsTyping)
                {
                    _subtitleSystem.FastForward();
                }
                else if (_isWaitingForInput)
                {
                    AdvanceDialogue();
                }
            }
        }

        public void StartConversation(ConversationData conversation, Action onComplete = null)
        {
            if (conversation == null || conversation.Lines.Count == 0) return;

            // Enforce requirement: strictly no overlapping conversations.
            if (IsDialogueActive)
            {
                _conversationQueue.Enqueue(new QueuedConversation
                {
                    Conversation = conversation,
                    OnComplete = onComplete
                });
                return;
            }

            _currentConversation = conversation;
            _currentLineIndex = -1;
            _onConversationCompleteCallback = onComplete;

            Show();
            UIEventBus.PublishUIStateChanged(UIState.Dialogue);

            // Toggle Group Discussion vs Standard Dialogue Mode
            bool isGroup = conversation.IsGroupDiscussion;
            if (_groupPresentation != null) _groupPresentation.SetVisible(isGroup);
            if (_characterPresentation != null) _characterPresentation.gameObject.SetActive(!isGroup);

            AdvanceDialogue();
        }

        public void StartConversationById(string conversationId, Action onComplete = null)
        {
            if (_sequenceManager != null)
            {
                var conv = _sequenceManager.GetConversation(conversationId);
                if (conv != null)
                {
                    StartConversation(conv, onComplete);
                    return;
                }
            }
            Debug.LogWarning($"[DialogueManager] Conversation ID '{conversationId}' not found.");
        }

        public void AdvanceDialogue()
        {
            _isWaitingForInput = false;
            if (_continueIndicator != null) _continueIndicator.SetActive(false);

            _currentLineIndex++;

            if (_currentLineIndex < _currentConversation.Lines.Count)
            {
                var line = _currentConversation.Lines[_currentLineIndex];

                // Notify Computer 2 of camera framing request
                UIEventBus.PublishCameraFraming(line.Framing, line.Speaker);

                // Update visual speaker presentation
                if (_currentConversation.IsGroupDiscussion && _groupPresentation != null)
                {
                    _groupPresentation.SetActiveSpeaker(line.Speaker, line.Expression, line.CrewReactions);
                }
                else if (_characterPresentation != null)
                {
                    _characterPresentation.DisplaySpeaker(line.Speaker, line.Expression, line.EducationalConceptNote);
                }

                // Subtitle display: guarantee speaker attribution and subtitle readability
                string formattedSpeaker = GetSpeakerDisplayName(line.Speaker);
                string subtitleContent = $"<b>{formattedSpeaker}:</b> \"{line.DialogueText}\"";

                _subtitleSystem?.DisplayLine(subtitleContent, () =>
                {
                    _isWaitingForInput = line.RequirePlayerClickToAdvance;
                    if (_continueIndicator != null) _continueIndicator.SetActive(_isWaitingForInput);
                });
            }
            else
            {
                // Reached end of line list. Check if branching choices exist
                if (_currentConversation.Choices != null && _currentConversation.Choices.Count > 0)
                {
                    DisplayChoices(_currentConversation.Choices);
                }
                else
                {
                    EndConversation();
                }
            }
        }

        private string GetSpeakerDisplayName(CharacterIdentifier speaker)
        {
            return speaker switch
            {
                CharacterIdentifier.CommanderElena => "COMMANDER ELENA",
                CharacterIdentifier.EngineerMarcus => "ENGINEER MARCUS",
                CharacterIdentifier.ScientistAisha => "SCIENTIST AISHA",
                CharacterIdentifier.MedicalOfficerDaniel => "DR. DANIEL",
                CharacterIdentifier.ORION => "ORION AI",
                _ => "CREW"
            };
        }

        private void DisplayChoices(List<DialogueChoice> choices)
        {
            if (_choicesContainer != null) _choicesContainer.SetActive(true);

            for (int i = 0; i < _choiceButtons.Length; i++)
            {
                if (i < choices.Count)
                {
                    int choiceIndex = i;
                    _choiceButtons[i].gameObject.SetActive(true);
                    if (_choiceTexts != null && i < _choiceTexts.Length && _choiceTexts[i] != null)
                    {
                        _choiceTexts[i].text = choices[i].ChoiceText;
                    }
                    if (_choiceHintTexts != null && i < _choiceHintTexts.Length && _choiceHintTexts[i] != null)
                    {
                        _choiceHintTexts[i].text = choices[i].EducationalHint;
                    }
                    _choiceButtons[i].onClick.RemoveAllListeners();
                    _choiceButtons[i].onClick.AddListener(() => SelectChoice(choiceIndex));
                }
                else
                {
                    _choiceButtons[i].gameObject.SetActive(false);
                }
            }
        }

        public void SelectChoice(int choiceIndex)
        {
            if (_currentConversation == null || choiceIndex >= _currentConversation.Choices.Count) return;

            var chosen = _currentConversation.Choices[choiceIndex];

            // If an emergency option was selected (Chapter 5), notify system
            if (chosen.EmergencyOption != EmergencyRepairOption.None)
            {
                UIEventBus.PublishEmergencyOption(chosen.EmergencyOption);
            }

            if (_choicesContainer != null) _choicesContainer.SetActive(false);

            // Follow branch if next conversation specified
            if (!string.IsNullOrEmpty(chosen.ResultingConversationId))
            {
                StartConversationById(chosen.ResultingConversationId, _onConversationCompleteCallback);
            }
            else
            {
                EndConversation();
            }
        }

        private void EndConversation()
        {
            var completedConv = _currentConversation;
            _currentConversation = null;

            _subtitleSystem?.HideSubtitles();
            if (_choicesContainer != null) _choicesContainer.SetActive(false);
            if (_continueIndicator != null) _continueIndicator.SetActive(false);
            if (_groupPresentation != null) _groupPresentation.SetVisible(false);

            Hide(false, () =>
            {
                completedConv?.OnConversationCompleted?.Invoke();
                _onConversationCompleteCallback?.Invoke();

                // Process next conversation in queue if any
                if (_conversationQueue.Count > 0)
                {
                    var next = _conversationQueue.Dequeue();
                    StartConversation(next.Conversation, next.OnComplete);
                }
                else
                {
                    UIEventBus.PublishUIStateChanged(UIState.GameplayHUD);
                }
            });
        }
    }
}
