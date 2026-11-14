using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Outpost.Core.Data;
using Outpost.Core.Interfaces;
using Outpost.Core.Events;

namespace Outpost.System3.Interface.Dialogue
{
    public class DialogueUI : MonoBehaviour, IDialogueService
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private Image portraitBadge;
        [SerializeField] private Text speakerNameText;
        [SerializeField] private Text dialogueBodyText;
        [SerializeField] private Button continueButton;

        [Header("Typing Animation")]
        [SerializeField] private float typeSpeed = 0.02f;

        private Queue<DialogueLine> dialogueQueue = new Queue<DialogueLine>();
        private Coroutine activeTypingCoroutine;
        private bool isTyping = false;
        private string fullCurrentText = string.Empty;

        public bool IsDialogueActive => dialoguePanel != null && dialoguePanel.activeSelf;

        public event Action<DialogueLine> OnLineStarted;
        public event Action OnConversationCompleted;

        public static DialogueUI Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            if (continueButton != null) continueButton.onClick.AddListener(DismissCurrentLine);
        }

        private void Start()
        {
            if (dialoguePanel != null) dialoguePanel.SetActive(false);
        }

        private void OnEnable()
        {
            OutpostEventChannel.DialogueLinePrompted += HandleDialogueLinePrompted;
            OutpostEventChannel.ConversationSequencePrompted += PlayConversation;
        }

        private void OnDisable()
        {
            OutpostEventChannel.DialogueLinePrompted -= HandleDialogueLinePrompted;
            OutpostEventChannel.ConversationSequencePrompted -= PlayConversation;
        }

        private void HandleDialogueLinePrompted(DialogueLine line)
        {
            PlaySingleLine(line.Speaker, line.Text, line.DisplayDuration);
        }

        public void PlayConversation(ConversationSequence sequence)
        {
            dialogueQueue.Clear();
            foreach (var line in sequence.Lines)
            {
                dialogueQueue.Enqueue(line);
            }

            ShowNextQueuedLine();
        }

        public void PlaySingleLine(CharacterId speaker, string text, float duration = 4f)
        {
            dialogueQueue.Clear();
            dialogueQueue.Enqueue(new DialogueLine(speaker, text, duration));
            ShowNextQueuedLine();
        }

        private void ShowNextQueuedLine()
        {
            if (dialogueQueue.Count == 0)
            {
                if (dialoguePanel != null) dialoguePanel.SetActive(false);
                OnConversationCompleted?.Invoke();
                OutpostEventChannel.RaiseDialogueCompleted();
                return;
            }

            DialogueLine next = dialogueQueue.Dequeue();
            DisplayLine(next);
        }

        private void DisplayLine(DialogueLine line)
        {
            if (dialoguePanel != null) dialoguePanel.SetActive(true);

            CharacterProfile profile = CharacterProfile.GetProfile(line.Speaker);
            if (speakerNameText != null) speakerNameText.text = $"{profile.FullName} ({profile.Role})";
            if (portraitBadge != null) portraitBadge.color = profile.ThemeColor;

            OnLineStarted?.Invoke(line);
            OutpostEventChannel.RaisePlaySfx(line.AudioCue);

            fullCurrentText = line.Text;
            if (activeTypingCoroutine != null) StopCoroutine(activeTypingCoroutine);
            activeTypingCoroutine = StartCoroutine(TypewriteText(line.Text, line.DisplayDuration));
        }

        private IEnumerator TypewriteText(string text, float autoDismissDelay)
        {
            isTyping = true;
            if (dialogueBodyText != null) dialogueBodyText.text = string.Empty;

            for (int i = 0; i < text.Length; i++)
            {
                if (dialogueBodyText != null) dialogueBodyText.text += text[i];
                yield return new WaitForSeconds(typeSpeed);
            }

            isTyping = false;

            if (autoDismissDelay > 0f)
            {
                yield return new WaitForSeconds(autoDismissDelay);
                ShowNextQueuedLine();
            }
        }

        public void DismissCurrentLine()
        {
            if (isTyping)
            {
                // Instantly complete typing
                if (activeTypingCoroutine != null) StopCoroutine(activeTypingCoroutine);
                if (dialogueBodyText != null) dialogueBodyText.text = fullCurrentText;
                isTyping = false;
            }
            else
            {
                ShowNextQueuedLine();
            }
        }
    }
}
