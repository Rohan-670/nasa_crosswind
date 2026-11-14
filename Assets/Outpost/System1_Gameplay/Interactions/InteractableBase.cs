using System;
using UnityEngine;
using Outpost.Core.Interfaces;
using Outpost.Core.Events;

namespace Outpost.System1.Gameplay.Interactions
{
    public abstract class InteractableBase : MonoBehaviour, IInteractable
    {
        [Header("Interaction Configuration")]
        [SerializeField] protected string promptAction = "Interact";
        [SerializeField] protected string objectName = "Object";
        [SerializeField] protected float maxInteractionDistance = 3.5f;
        [SerializeField] protected bool canInteract = true;
        [SerializeField] protected string audioCue = "interact";

        public virtual string PromptText => $"{objectName}: Press [E] to {promptAction}";
        public virtual Vector3 InteractionPosition => transform.position;
        public virtual bool CanInteract => canInteract && enabled && gameObject.activeInHierarchy;
        public float MaxDistance => maxInteractionDistance;

        public event Action<GameObject> OnInteracted;
        public event Action<bool> OnFocusStateChanged;

        protected virtual void Awake()
        {
        }

        public void Interact()
        {
            Interact(null);
        }

        public virtual void Interact(GameObject user)
        {
            if (!CanInteract) return;

            if (!string.IsNullOrEmpty(audioCue))
            {
                OutpostEventChannel.RaisePlaySfx(audioCue);
            }

            OnInteracted?.Invoke(user);
            ExecuteInteraction(user);
        }

        protected abstract void ExecuteInteraction(GameObject user);

        public virtual void OnFocusGained(GameObject user)
        {
            OnFocusStateChanged?.Invoke(true);
        }

        public virtual void OnFocusLost(GameObject user)
        {
            OnFocusStateChanged?.Invoke(false);
        }

        public void SetInteractable(bool state)
        {
            canInteract = state;
        }
    }
}
