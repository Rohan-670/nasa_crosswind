using UnityEngine;
using UnityEngine.InputSystem;
using Outpost.Core.Interfaces;
using Outpost.Core.Events;

namespace Outpost.System1.Gameplay.Player
{
    public class PlayerInteraction : MonoBehaviour
    {
        [SerializeField] private float interactionDistance = 3.5f;
        [SerializeField] private LayerMask interactableLayer = ~0;
        [SerializeField] private Transform raycastOrigin;

        private IInteractable currentInteractable;
        private bool isMapOpen = false;

        private void Awake()
        {
            if (raycastOrigin == null)
            {
                Camera cam = GetComponentInChildren<Camera>();
                if (cam != null) raycastOrigin = cam.transform;
                else raycastOrigin = transform;
            }
        }

        private void Update()
        {
            CheckMapToggleInput();
            DetectInteractable();
            HandleInteractionInput();
        }

        private void CheckMapToggleInput()
        {
            if (Keyboard.current != null && Keyboard.current.mKey.wasPressedThisFrame)
            {
                isMapOpen = !isMapOpen;
                OutpostEventChannel.RaiseGeoMapToggled(isMapOpen);
                OutpostEventChannel.RaisePlaySfx("ui_click");
            }
        }

        private void DetectInteractable()
        {
            Ray ray = new Ray(raycastOrigin.position, raycastOrigin.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, interactableLayer))
            {
                IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();
                if (interactable != null && interactable.CanInteract)
                {
                    currentInteractable = interactable;
                    return;
                }
            }

            currentInteractable = null;
        }

        private void HandleInteractionInput()
        {
            if (currentInteractable == null) return;

            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                currentInteractable.Interact();
                OutpostEventChannel.RaisePlaySfx("interact");
            }
        }

        public string GetCurrentPrompt()
        {
            return currentInteractable != null ? currentInteractable.PromptText : string.Empty;
        }
    }
}
