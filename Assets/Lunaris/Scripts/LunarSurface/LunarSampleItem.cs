using UnityEngine;
using Lunaris.Core;

namespace Lunaris.LunarSurface
{
    public class LunarSampleItem : MonoBehaviour
    {
        [Header("Sample Data")]
        [SerializeField] private string sampleName = "Basaltic Lunar Core";
        [SerializeField] private string sampleDescription = "High-titanium basalt rock formation from volcanic mare.";
        [SerializeField] private float sampleWeightKg = 1.4f;

        [Header("Visuals")]
        [SerializeField] private Light sampleBeaconLight;
        [SerializeField] private float rotationSpeed = 30f;

        private bool isCollected = false;

        private void Update()
        {
            if (isCollected) return;

            // Hover and rotate
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
        }

        public void CollectSample()
        {
            if (isCollected) return;
            isCollected = true;

            Debug.Log($"[LunarSampleItem] Collected {sampleName} ({sampleWeightKg} kg)!");

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySampleCollected();
            }

            if (LunarisGameManager.Instance != null && LunarisGameManager.Instance.CurrentMission != null)
            {
                LunarisGameManager.Instance.CurrentMission.samplesCollected++;
            }

            // Hide object mesh and beacon
            gameObject.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") || other.GetComponent<AstronautController>() != null)
            {
                CollectSample();
            }
        }
    }
}
