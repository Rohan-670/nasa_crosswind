using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Outpost.UI.Dialogue
{
    /// <summary>
    /// Manages complex multi-character conversations, autonomous background crew chatter, and pre-event briefings.
    /// Works with DialogueManager to advance conversations across chapters.
    /// </summary>
    public class ConversationSequenceManager : MonoBehaviour
    {
        [Header("Conversation Database")]
        [SerializeField] private List<ConversationData> _registeredConversations = new List<ConversationData>();

        private readonly Dictionary<string, ConversationData> _conversationMap =
            new Dictionary<string, ConversationData>();

        private void Awake()
        {
            RegisterConversations();
        }

        private void RegisterConversations()
        {
            foreach (var conv in _registeredConversations)
            {
                if (conv != null && !string.IsNullOrEmpty(conv.ConversationId))
                {
                    _conversationMap[conv.ConversationId] = conv;
                }
            }
        }

        public ConversationData GetConversation(string conversationId)
        {
            _conversationMap.TryGetValue(conversationId, out var conversation);
            return conversation;
        }

        public void AddConversation(ConversationData conversation)
        {
            if (conversation != null && !string.IsNullOrEmpty(conversation.ConversationId))
            {
                _registeredConversations.Add(conversation);
                _conversationMap[conversation.ConversationId] = conversation;
            }
        }
    }
}
