using UnityEngine;
using Outpost.Core.Data;
using Outpost.Core.Events;

namespace Outpost.System1.Gameplay.Interactions
{
    public class MissionObjectInteractable : InteractableBase
    {
        [Header("Mission Object Data")]
        [SerializeField] private StationLocationId associatedStation = StationLocationId.LandingZone;
        [SerializeField] private string missionObjectName = "Lunar Descent Module Hatch";
        [SerializeField] private string interactionAction = "Deploy Outpost Beacon";
        [SerializeField] private bool triggerLocationArrival = true;

        protected override void Awake()
        {
            objectName = missionObjectName;
            promptAction = interactionAction;
            audioCue = "waypoint_reached";
        }

        protected override void ExecuteInteraction(GameObject user)
        {
            if (triggerLocationArrival)
            {
                OutpostEventChannel.RaiseLocationArrived(associatedStation);
            }
        }
    }
}
