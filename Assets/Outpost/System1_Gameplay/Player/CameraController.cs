using UnityEngine;
using UnityEngine.InputSystem;

namespace Outpost.System1.Gameplay.Player
{
    public class CameraController : MonoBehaviour
    {
        [Header("Target & Rotation")]
        [SerializeField] private Transform playerBody;
        [SerializeField] private float mouseSensitivity = 1.8f;
        [SerializeField] private float lookXLimit = 80f;
        [SerializeField] private bool smoothLook = true;
        [SerializeField] private float smoothTime = 0.05f;

        [Header("Lunar Head Bob")]
        [SerializeField] private bool enableHeadBob = true;
        [SerializeField] private float bobFrequency = 3.5f;
        [SerializeField] private float bobHeight = 0.04f;

        private float rotationX = 0f;
        private Vector2 currentMouseDelta;
        private Vector2 smoothMouseDelta;
        private Vector2 smoothLookVelocity;
        private Vector3 defaultCameraLocalPos;
        private float bobTimer = 0f;
        private bool isLocked = false;

        private Transform cinematicTarget;
        private bool isCinematicActive = false;

        private void Start()
        {
            defaultCameraLocalPos = transform.localPosition;
            if (playerBody == null && transform.parent != null)
            {
                playerBody = transform.parent;
            }
        }

        public void SetLock(bool locked)
        {
            isLocked = locked;
        }

        public void SetCinematicTarget(Transform target)
        {
            cinematicTarget = target;
            isCinematicActive = target != null;
        }

        private void LateUpdate()
        {
            if (isCinematicActive && cinematicTarget != null)
            {
                Vector3 targetDir = cinematicTarget.position - transform.position;
                if (targetDir != Vector3.zero)
                {
                    Quaternion targetRot = Quaternion.LookRotation(targetDir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 3f * Time.deltaTime);
                }
                return;
            }

            if (isLocked) return;

            HandleLookRotation();
            HandleHeadBob();
        }

        private void HandleLookRotation()
        {
            if (Mouse.current == null) return;

            Vector2 targetDelta = Mouse.current.delta.ReadValue() * (mouseSensitivity * 0.1f);

            if (smoothLook)
            {
                smoothMouseDelta = Vector2.SmoothDamp(smoothMouseDelta, targetDelta, ref smoothLookVelocity, smoothTime);
                currentMouseDelta = smoothMouseDelta;
            }
            else
            {
                currentMouseDelta = targetDelta;
            }

            rotationX -= currentMouseDelta.y;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);

            if (playerBody != null)
            {
                playerBody.Rotate(Vector3.up * currentMouseDelta.x);
            }
        }

        private void HandleHeadBob()
        {
            if (!enableHeadBob || playerBody == null) return;

            PlayerController pc = playerBody.GetComponent<PlayerController>();
            if (pc == null || !pc.IsGrounded)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, defaultCameraLocalPos, 5f * Time.deltaTime);
                return;
            }

            float speed = new Vector3(pc.Velocity.x, 0f, pc.Velocity.z).magnitude;
            if (speed > 0.2f)
            {
                bobTimer += Time.deltaTime * bobFrequency * (speed / 3.5f);
                float newY = defaultCameraLocalPos.y + Mathf.Sin(bobTimer) * bobHeight;
                transform.localPosition = new Vector3(defaultCameraLocalPos.x, newY, defaultCameraLocalPos.z);
            }
            else
            {
                bobTimer = 0f;
                transform.localPosition = Vector3.Lerp(transform.localPosition, defaultCameraLocalPos, 5f * Time.deltaTime);
            }
        }
    }
}
