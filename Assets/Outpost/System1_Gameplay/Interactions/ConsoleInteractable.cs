using UnityEngine;
using Outpost.Core.Events;

namespace Outpost.System1.Gameplay.Interactions
{
    public class ConsoleInteractable : InteractableBase
    {
        [Header("Console State")]
        [SerializeField] private string systemLabel = "Main Power Grid Console";
        [SerializeField] private bool isActive = false;
        [SerializeField] private Renderer indicatorRenderer;
        [SerializeField] private Color activeColor = Color.green;
        [SerializeField] private Color inactiveColor = Color.yellow;

        public bool IsActive => isActive;

        protected override void Awake()
        {
            objectName = systemLabel;
            promptAction = isActive ? "Deactivate Console" : "Activate Console";
            audioCue = "terminal_beep";
            UpdateVisuals();
        }

        public override string PromptText => $"{objectName}: Press [E] to {(isActive ? "Deactivate" : "Activate")}";

        protected override void ExecuteInteraction(GameObject user)
        {
            isActive = !isActive;
            UpdateVisuals();

            OutpostEventChannel.RaisePlaySfx(isActive ? "system_restored" : "ui_click");
        }

        private void UpdateVisuals()
        {
            if (indicatorRenderer != null && indicatorRenderer.material != null)
            {
                indicatorRenderer.material.color = isActive ? activeColor : inactiveColor;
            }
        }
    }
}
