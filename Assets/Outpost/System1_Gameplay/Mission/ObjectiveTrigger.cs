using UnityEngine;
using Outpost.Core.Data;
using Outpost.Core.Events;

namespace Outpost.System1.Gameplay.Mission
{
    [RequireComponent(typeof(Collider))]
    public class ObjectiveTrigger : MonoBehaviour
    {
        [SerializeField] private StationLocationId locationId;
        [SerializeField] private bool triggerOnce = false;

        private bool hasTriggered = false;

        private void OnTriggerEnter(Collider other)
        {
            if (triggerOnce && hasTriggered) return;

            // Check if player
            if (other.CompareTag("Player") || other.GetComponent<Player.LunarPlayerController>() != null)
            {
                hasTriggered = true;
                OutpostEventChannel.RaiseLocationArrived(locationId);
                OutpostEventChannel.RaisePlaySfx("waypoint_reached");
            }
        }
    }
}
