using System;
using UnityEngine;
using Outpost.Core.Interfaces;
using Outpost.Core.Events;

namespace Outpost.System1.Gameplay.Cinematics
{
    public class CinematicManager : MonoBehaviour, ICinematicTrigger
    {
        [SerializeField] private Player.CameraController cameraController;

        private string activeCinematicId = string.Empty;
        private bool isPlaying = false;
        private Action onCompleteCallback;

        public string CinematicId => activeCinematicId;
        public bool IsPlaying => isPlaying;

        public event Action<string> OnCinematicStarted;
        public event Action<string> OnCinematicCompleted;

        public static CinematicManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            if (cameraController == null)
            {
                cameraController = FindAnyObjectByType<Player.CameraController>();
            }
        }

        public void PlayCinematic(string cinematicId, Action onCompleted = null)
        {
            activeCinematicId = cinematicId;
            isPlaying = true;
            onCompleteCallback = onCompleted;

            OnCinematicStarted?.Invoke(cinematicId);
            OutpostEventChannel.RaisePlaySfx("info_chime");

            // Look for a camera focal anchor with this name
            GameObject anchor = GameObject.Find($"Anchor_{cinematicId}");
            if (anchor != null && cameraController != null)
            {
                cameraController.SetCinematicTarget(anchor.transform);
            }
        }

        public void StopCinematic()
        {
            if (!isPlaying) return;

            isPlaying = false;
            if (cameraController != null)
            {
                cameraController.SetCinematicTarget(null);
            }

            string finishedId = activeCinematicId;
            activeCinematicId = string.Empty;

            OnCinematicCompleted?.Invoke(finishedId);
            onCompleteCallback?.Invoke();
            onCompleteCallback = null;
        }
    }
}
