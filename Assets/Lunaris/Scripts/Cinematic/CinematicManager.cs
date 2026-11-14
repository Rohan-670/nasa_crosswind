using System.Collections;
using UnityEngine;
using Lunaris.Core;

namespace Lunaris.Cinematic
{
    public class CinematicManager : MonoBehaviour
    {
        [Header("Cinematic Celestial Actors")]
        [SerializeField] private Transform earthTransform;
        [SerializeField] private Transform moonTransform;
        [SerializeField] private Camera cinematicCamera;

        [Header("Cinematic Sequence Settings")]
        [SerializeField] private float narrationDuration = 18f;
        [SerializeField] private float cameraPanSpeed = 1.2f;

        private float cinematicTimer = 0f;
        private bool isCinematicPlaying = false;
        private string narrationText = "Humanity has always looked beyond the stars. Every generation has dreamed of exploring what lies beyond our world. Today, a new generation is preparing to take that next step.";

        private void Start()
        {
            if (LunarisGameManager.Instance != null)
            {
                LunarisGameManager.Instance.OnStateChanged += HandleStateChanged;
                if (LunarisGameManager.Instance.CurrentState == GameState.OpeningCinematic)
                {
                    StartCinematic();
                }
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
            if (newState == GameState.OpeningCinematic)
            {
                StartCinematic();
            }
            else
            {
                isCinematicPlaying = false;
            }
        }

        public void StartCinematic()
        {
            isCinematicPlaying = true;
            cinematicTimer = 0f;
            StartCoroutine(RunCinematicSequence());
        }

        private IEnumerator RunCinematicSequence()
        {
            Debug.Log("[CinematicManager] Opening Deep Space Cinematic Started.");

            while (isCinematicPlaying && cinematicTimer < narrationDuration)
            {
                cinematicTimer += Time.deltaTime;

                // Rotate Earth slowly
                if (earthTransform != null)
                {
                    earthTransform.Rotate(Vector3.up, 3f * Time.deltaTime, Space.World);
                }

                // Slow camera drift toward Earth and Moon
                if (cinematicCamera != null)
                {
                    cinematicCamera.transform.position += cinematicCamera.transform.forward * (cameraPanSpeed * Time.deltaTime);
                }

                yield return null;
            }

            // Cinematic finished -> Transition to Training Facility
            EndCinematic();
        }

        public void SkipCinematic()
        {
            StopAllCoroutines();
            EndCinematic();
        }

        private void EndCinematic()
        {
            isCinematicPlaying = false;
            Debug.Log("[CinematicManager] Cinematic completed. Transitioning to Astronaut Training Center.");

            if (LunarisGameManager.Instance != null)
            {
                LunarisGameManager.Instance.TransitionToState(GameState.TrainingFacility);
            }

            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.StartDialogueSequence();
            }
        }

        private void OnGUI()
        {
            if (!isCinematicPlaying) return;

            float screenW = Screen.width;
            float screenH = Screen.height;

            // Cinematic black letterbox bars
            float barH = 50f;
            GUI.color = Color.black;
            GUI.DrawTexture(new Rect(0, 0, screenW, barH), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(0, screenH - barH, screenW, barH), Texture2D.whiteTexture);
            GUI.color = Color.white;

            // Narration Subtitle
            GUIStyle narrStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 18,
                fontStyle = FontStyle.Italic,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true
            };
            narrStyle.normal.textColor = new Color(0.95f, 0.95f, 1f, 0.9f);

            float subW = Mathf.Min(800f, screenW - 80f);
            GUI.Label(new Rect((screenW - subW) / 2f, screenH - barH - 65f, subW, 60f), $"\"{narrationText}\"", narrStyle);

            // Skip Button
            if (GUI.Button(new Rect(screenW - 130, 15, 110, 30), "SKIP ⏭"))
            {
                SkipCinematic();
            }
        }
    }
}
