using UnityEngine;

namespace Outpost.Core.Interfaces
{
    public interface IInteractable
    {
        string PromptText { get; }
        Vector3 InteractionPosition { get; }
        bool CanInteract { get; }
        void Interact();
    }
}
