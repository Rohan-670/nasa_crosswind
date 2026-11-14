using System.Collections;
using UnityEngine;
using Lunaris.Core;

namespace Lunaris.LunarSurface
{
    public class LunarLandingSequence : MonoBehaviour
    {
        [Header("Landing Actors")]
        [SerializeField] private GameObject landerSpacecraft;
        [SerializeField] private GameObject astronautPlayer;
        [SerializeField] private Camera landingCinematicCam;

        [Header("Landing Animation")]
        [SerializeField] private Vector3 approachPosition = new Vector3(0, 120, 1300);
        [SerializeField] private Vector3 touchdownPosition = new Vector3(0, 2.5f, 1300);
        [SerializeField] private float descentDuration = 8f;

        private bool isLanding = false;

        private void Start()
        {
            if (LunarisGameManager.Instance != null)
            {
                LunarisGameManager.Instance.OnStateChanged += HandleStateChanged;
            }
        }

        private void OnDestroy()
        {
            if (LunarisGameManager.Instance != null)
            {
                LunarisGameManager.Instance.OnStateChanged -= HandleStateChanged;
            }
        }

        private void HandleStateChanged(GameState newState)
        {
            if (newState == GameState.LunarLanding)
            {
                StartLandingSequence();
            }
        }

        public void StartLandingSequence()
        {
            if (isLanding) return;
            isLanding = true;

            StartCoroutine(DoDescentAndDeploy());
        }

        private IEnumerator DoDescentAndDeploy()
        {
            Debug.Log("[LunarLandingSequence] Initiating automated retrograde burn and touchdown...");

            if (landingCinematicCam != null)
            {
                landingCinematicCam.gameObject.SetActive(true);
            }

            float elapsed = 0f;
            while (elapsed < descentDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / descentDuration);

                if (landerSpacecraft != null)
                {
                    landerSpacecraft.transform.position = Vector3.Lerp(approachPosition, touchdownPosition, t);
                }

                yield return null;
            }

            Debug.Log("[LunarLandingSequence] Touchdown confirmed! Welcome to the Lunar South Pole.");

            // Deploy Astronaut
            if (astronautPlayer != null)
            {
                astronautPlayer.transform.position = touchdownPosition + new Vector3(3f, 0.5f, 3f);
                astronautPlayer.SetActive(true);
            }

            if (landingCinematicCam != null)
            {
                landingCinematicCam.gameObject.SetActive(false);
            }

            isLanding = false;
            if (LunarisGameManager.Instance != null)
            {
                LunarisGameManager.Instance.TransitionToState(GameState.LunarExpedition);
            }
        }
    }
}
