using UnityEngine;

namespace Outpost.System2.Environment.Lunar
{
    [ExecuteAlways]
    public class LunarEnvironmentController : MonoBehaviour
    {
        [Header("Lunar Sun (Directional Light)")]
        [SerializeField] private Light sunLight;
        [Tooltip("Grazing sun altitude angle at Lunar South Pole (~10-15 degrees)")]
        [SerializeField] private float sunAltitudeAngle = 12.0f;
        [SerializeField] private float sunAzimuthAngle = 45.0f;
        [SerializeField] private float sunIntensity = 1.35f;
        [SerializeField] private Color sunColor = new Color(1.0f, 0.98f, 0.95f);

        [Header("Lunar Vacuum & Ambient")]
        [SerializeField] private Color deepSpaceAmbient = new Color(0.04f, 0.04f, 0.06f);

        private void Start()
        {
            ApplyEnvironmentSettings();
        }

        private void OnValidate()
        {
            ApplyEnvironmentSettings();
        }

        public void ApplyEnvironmentSettings()
        {
            if (sunLight != null)
            {
                sunLight.type = LightType.Directional;
                sunLight.transform.rotation = Quaternion.Euler(sunAltitudeAngle, sunAzimuthAngle, 0f);
                sunLight.intensity = sunIntensity;
                sunLight.color = sunColor;
                sunLight.shadows = LightShadows.Hard; // Lunar shadows have zero atmospheric diffusion
            }

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = deepSpaceAmbient;
            RenderSettings.fog = false; // No atmospheric fog on the Moon
        }
    }
}
