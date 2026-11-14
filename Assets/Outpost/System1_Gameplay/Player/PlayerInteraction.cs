using UnityEngine;
using Outpost.System1.Gameplay.Interactions;

namespace Outpost.System1.Gameplay.Player
{
    /// <summary>
    /// Legacy integration bridge for PlayerInteraction.
    /// Delegates to the unified InteractionManager to maintain compatibility with System 3 (OutpostHUD).
    /// </summary>
    public class PlayerInteraction : MonoBehaviour
    {
        [SerializeField] private InteractionManager interactionManager;

        private void Awake()
        {
            if (interactionManager == null)
            {
                interactionManager = GetComponent<InteractionManager>();
                if (interactionManager == null) interactionManager = FindFirstObjectByType<InteractionManager>();
            }
        }

        public string GetCurrentPrompt()
        {
            if (interactionManager != null)
            {
                return interactionManager.GetPromptText();
            }
            return string.Empty;
        }
    }
}
