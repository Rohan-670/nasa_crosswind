using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Outpost.Core.Interfaces;
using Outpost.Core.Events;

namespace Outpost.System1.Gameplay.Interactions
{
    public class InteractionManager : MonoBehaviour
    {
        [Header("Raycast Settings")]
        [SerializeField] private Transform raycastOrigin;
        [SerializeField] private float maxInteractionDistance = 3.5f;
        [SerializeField] private LayerMask interactableLayers = ~0;

        private IInteractable activeInteractable;
        private bool isMapOpen = false;

        public IInteractable ActiveInteractable => activeInteractable;
        public bool HasInteractable => activeInteractable != null && activeInteractable.CanInteract;

        public event Action<IInteractable> OnInteractableTargetChanged;

        private void Start()
        {
            if (raycastOrigin == null)
            {
                Camera cam = Camera.main;
                if (cam != null) raycastOrigin = cam.transform;
                else raycastOrigin = transform;
            }
        }

        private void Update()
        {
            CheckMapToggle();
            PerformInteractionRaycast();
            HandleInteractionExecution();
        }

        private void CheckMapToggle()
        {
            if (Keyboard.current != null && Keyboard.current.mKey.wasPressedThisFrame)
            {
                isMapOpen = !isMapOpen;
                OutpostEventChannel.RaiseGeoMapToggled(isMapOpen);
                OutpostEventChannel.RaisePlaySfx("ui_click");
            }
        }

        private void PerformInteractionRaycast()
        {
            if (raycastOrigin == null) return;

            Ray ray = new Ray(raycastOrigin.position, raycastOrigin.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, maxInteractionDistance, interactableLayers))
            {
                IInteractable target = hit.collider.GetComponentInParent<IInteractable>();
                if (target != null && target.CanInteract)
                {
                    if (activeInteractable != target)
                    {
                        activeInteractable = target;
                        OnInteractableTargetChanged?.Invoke(activeInteractable);
                    }
                    return;
                }
            }

            if (activeInteractable != null)
            {
                activeInteractable = null;
                OnInteractableTargetChanged?.Invoke(null);
            }
        }

        private void HandleInteractionExecution()
        {
            if (activeInteractable == null || !activeInteractable.CanInteract) return;

            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                activeInteractable.Interact();
                OutpostEventChannel.RaisePlaySfx("interact");
            }
        }

        public string GetPromptText()
        {
            return activeInteractable != null ? activeInteractable.PromptText : string.Empty;
        }
    }
}
