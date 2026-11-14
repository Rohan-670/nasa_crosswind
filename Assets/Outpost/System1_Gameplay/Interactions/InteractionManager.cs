using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Outpost.Core.Interfaces;
using Outpost.Core.Events;

namespace Outpost.System1.Gameplay.Interactions
{
    public class InteractionManager : MonoBehaviour
    {
        [Header("Detection Range & Layers")]
        [SerializeField] private Transform raycastOrigin;
        [SerializeField] private float maxDetectionDistance = 3.8f;
        [SerializeField] private float sphereCastRadius = 0.4f;
        [SerializeField] private LayerMask interactableLayers = ~0;

        private IInteractable activeInteractable;
        private InteractableBase activeInteractableBase;
        private bool isMapOpen = false;

        public IInteractable ActiveInteractable => activeInteractable;
        public bool HasInteractable => activeInteractable != null && activeInteractable.CanInteract;

        public event Action<IInteractable> OnInteractableTargetChanged;

        public static InteractionManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

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
            DetectInteractable();
            HandleInteractionExecution();
        }

        private void CheckMapToggle()
        {
            bool mapPressed = false;
            if (Keyboard.current != null) mapPressed = Keyboard.current.mKey.wasPressedThisFrame;
            else mapPressed = Input.GetKeyDown(KeyCode.M);

            if (mapPressed)
            {
                isMapOpen = !isMapOpen;
                OutpostEventChannel.RaiseGeoMapToggled(isMapOpen);
                OutpostEventChannel.RaisePlaySfx("ui_click");
            }
        }

        private void DetectInteractable()
        {
            if (raycastOrigin == null) return;

            Ray ray = new Ray(raycastOrigin.position, raycastOrigin.forward);
            IInteractable foundInteractable = null;

            // SphereCast gives smooth, forgiving detection for third-person player interactions
            if (Physics.SphereCast(ray, sphereCastRadius, out RaycastHit hit, maxDetectionDistance, interactableLayers))
            {
                IInteractable target = hit.collider.GetComponentInParent<IInteractable>();
                if (target != null && target.CanInteract)
                {
                    foundInteractable = target;
                }
            }

            // Fallback direct raycast if spherecast missed a thin object
            if (foundInteractable == null && Physics.Raycast(ray, out RaycastHit directHit, maxDetectionDistance, interactableLayers))
            {
                IInteractable target = directHit.collider.GetComponentInParent<IInteractable>();
                if (target != null && target.CanInteract)
                {
                    foundInteractable = target;
                }
            }

            // Update focus states
            if (activeInteractable != foundInteractable)
            {
                if (activeInteractableBase != null)
                {
                    activeInteractableBase.OnFocusLost(gameObject);
                }

                activeInteractable = foundInteractable;
                activeInteractableBase = foundInteractable as InteractableBase;

                if (activeInteractableBase != null)
                {
                    activeInteractableBase.OnFocusGained(gameObject);
                }

                OnInteractableTargetChanged?.Invoke(activeInteractable);
                OutpostEventChannel.RaiseInteractionPromptChanged(GetPromptText());
            }
        }

        private void HandleInteractionExecution()
        {
            if (activeInteractable == null || !activeInteractable.CanInteract) return;

            bool interactPressed = false;
            if (Keyboard.current != null) interactPressed = Keyboard.current.eKey.wasPressedThisFrame;
            else interactPressed = Input.GetKeyDown(KeyCode.E);

            if (interactPressed)
            {
                if (activeInteractableBase != null)
                {
                    activeInteractableBase.Interact(gameObject);
                }
                else
                {
                    activeInteractable.Interact();
                }
            }
        }

        public string GetPromptText()
        {
            return activeInteractable != null && activeInteractable.CanInteract ? activeInteractable.PromptText : string.Empty;
        }
    }
}
