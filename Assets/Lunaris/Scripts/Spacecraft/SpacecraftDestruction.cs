using System.Collections;
using UnityEngine;
using Lunaris.Core;

namespace Lunaris.Spacecraft
{
    public class SpacecraftDestruction : MonoBehaviour
    {
        [Header("Explosion FX")]
        [SerializeField] private GameObject explosionPrefab;
        [SerializeField] private float shakeMagnitude = 1.2f;
        [SerializeField] private float shakeDuration = 2.0f;

        private bool isDestroyed = false;

        public void TriggerDestruction()
        {
            if (isDestroyed) return;
            isDestroyed = true;

            Debug.Log("[SpacecraftDestruction] Spacecraft destruction initiated!");

            // 1. Disable flight controls
            SpacecraftFlightController controller = GetComponent<SpacecraftFlightController>();
            if (controller != null) controller.DisableControls();

            // 2. Audio
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayExplosion();
            }

            // 3. Camera Shake
            StartCoroutine(DoCameraShake());

            // 4. Hide ship mesh & spawn debris particles
            Transform modelChild = transform.Find("Model");
            if (modelChild != null) modelChild.gameObject.SetActive(false);

            SpawnDebrisFX();

            // 5. Notify Game Manager
            if (LunarisGameManager.Instance != null)
            {
                LunarisGameManager.Instance.FailMission("Navigation corridor breach — structural failure.");
            }
        }

        private void SpawnDebrisFX()
        {
            // Create procedural debris particles if no prefab assigned
            GameObject debrisObj = new GameObject("DebrisFX");
            debrisObj.transform.position = transform.position;

            ParticleSystem ps = debrisObj.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.startColor = new ParticleSystem.MinMaxGradient(new Color(1f, 0.4f, 0f), new Color(1f, 0.9f, 0.2f));
            main.startSize = 1.5f;
            main.startSpeed = 25f;
            main.startLifetime = 3.5f;
            main.loop = false;

            var emission = ps.emission;
            emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 150) });

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 2f;

            ps.Play();
            Destroy(debrisObj, 5f);
        }

        private IEnumerator DoCameraShake()
        {
            Camera cam = Camera.main;
            if (cam == null) yield break;

            Vector3 origPos = cam.transform.localPosition;
            float elapsed = 0f;

            while (elapsed < shakeDuration)
            {
                float x = (Random.value * 2f - 1f) * shakeMagnitude * (1f - (elapsed / shakeDuration));
                float y = (Random.value * 2f - 1f) * shakeMagnitude * (1f - (elapsed / shakeDuration));
                cam.transform.localPosition = origPos + new Vector3(x, y, 0f);

                elapsed += Time.deltaTime;
                yield return null;
            }

            cam.transform.localPosition = origPos;
        }
    }
}
