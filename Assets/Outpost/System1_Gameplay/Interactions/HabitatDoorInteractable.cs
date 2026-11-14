using System.Collections;
using UnityEngine;
using Outpost.Core.Events;

namespace Outpost.System1.Gameplay.Interactions
{
    public class HabitatDoorInteractable : InteractableBase
    {
        [Header("Door Movement")]
        [SerializeField] private Transform doorTransform;
        [SerializeField] private Vector3 openOffset = new Vector3(0f, 2.8f, 0f);
        [SerializeField] private float slideDuration = 1.2f;
        [SerializeField] private Collider doorCollider;

        private bool isOpen = false;
        private bool isMoving = false;
        private Vector3 closedPosition;
        private Vector3 openPosition;

        public bool IsOpen => isOpen;

        protected override void Awake()
        {
            promptAction = isOpen ? "Close Habitat Door" : "Open Habitat Door";
            objectName = "Airlock Door";
            audioCue = "interact";

            if (doorTransform == null) doorTransform = transform;
            closedPosition = doorTransform.localPosition;
            openPosition = closedPosition + openOffset;
        }

        public override string PromptText => $"{objectName}: Press [E] to {(isOpen ? "Close" : "Open")}";

        protected override void ExecuteInteraction(GameObject user)
        {
            if (isMoving) return;
            StartCoroutine(AnimateDoor(!isOpen));
        }

        private IEnumerator AnimateDoor(bool targetOpen)
        {
            isMoving = true;
            canInteract = false;

            OutpostEventChannel.RaisePlaySfx(targetOpen ? "radio_chirp" : "ui_click");

            Vector3 startPos = doorTransform.localPosition;
            Vector3 endPos = targetOpen ? openPosition : closedPosition;
            float elapsed = 0f;

            while (elapsed < slideDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / slideDuration);
                doorTransform.localPosition = Vector3.Lerp(startPos, endPos, t);
                yield return null;
            }

            doorTransform.localPosition = endPos;
            isOpen = targetOpen;
            isMoving = false;
            canInteract = true;

            if (doorCollider != null)
            {
                doorCollider.enabled = !isOpen; // Disabled when open so player walks through
            }
        }
    }
}
