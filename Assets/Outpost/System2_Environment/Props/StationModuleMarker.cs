using UnityEngine;
using Outpost.Core.Data;

namespace Outpost.System2.Environment.Props
{
    public class StationModuleMarker : MonoBehaviour
    {
        [SerializeField] private StationLocationId locationId;
        [SerializeField] private Light beaconLight;
        [SerializeField] private Color normalColor = new Color(0.2f, 0.7f, 1.0f);
        [SerializeField] private Color emergencyColor = new Color(1.0f, 0.2f, 0.1f);

        private void Start()
        {
            if (beaconLight == null)
            {
                beaconLight = GetComponentInChildren<Light>();
            }

            SetEmergencyState(locationId == StationLocationId.EmergencyBreachSite);
        }

        public void SetEmergencyState(bool isEmergency)
        {
            if (beaconLight != null)
            {
                beaconLight.color = isEmergency ? emergencyColor : normalColor;
            }
        }
    }
}
