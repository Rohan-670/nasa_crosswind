using UnityEngine;
using UnityEngine.InputSystem;

namespace Outpost.System1.Gameplay.Player
{
    public class ThirdPersonCameraController : MonoBehaviour
    {
        [Header("Target & Pivot")]
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 pivotOffset = new Vector3(0f, 1.5f, 0f);

        [Header("Distance & Orbit")]
        [SerializeField] private float defaultDistance = 4.5f;
        [SerializeField] private float minDistance = 1.2f;
        [SerializeField] private float maxDistance = 7.5f;
        [SerializeField] private float zoomSpeed = 2.0f;

        [Header("Look Sensitivity & Limits")]
        [SerializeField] private float mouseSensitivity = 1.8f;
        [SerializeField] private float minPitch = -25f;
        [SerializeField] private float maxPitch = 65f;
        [SerializeField] private float smoothTime = 0.05f;

        [Header("Camera Collision / Occlusion Avoidance")]
        [SerializeField] private bool enableCollisionAvoidance = true;
        [SerializeField] private float collisionRadius = 0.25f;
        [SerializeField] private float collisionPadding = 0.15f;
        [SerializeField] private LayerMask collisionLayers = ~0;

        private float currentYaw = 0f;
        private float currentPitch = 15f;
        private float targetDistance = 4.5f;
        private float currentDistance = 4.5f;
        private Vector2 smoothMouseVelocity;
        private Vector2 currentMouseDelta;
        private bool isControlLocked = false;

        public Transform Target => target;
        public Vector3 PivotPosition => target != null ? target.position + pivotOffset : transform.position;
        public Vector3 ForwardOnPlane => Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        public Vector3 RightOnPlane => Vector3.ProjectOnPlane(transform.right, Vector3.up).normalized;

        private void Start()
        {
            if (target == null)
            {
                GameObject player = GameObject.FindWithTag("Player");
                if (player != null) target = player.transform;
            }

            targetDistance = defaultDistance;
            currentDistance = defaultDistance;
            currentYaw = transform.eulerAngles.y;
            currentPitch = transform.eulerAngles.x;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        public void SetControlLock(bool locked)
        {
            isControlLocked = locked;
            Cursor.lockState = locked ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = locked;
        }

        private void LateUpdate()
        {
            if (target == null) return;

            HandleInput();
            UpdateCameraPosition();
        }

        private void HandleInput()
        {
            if (isControlLocked) return;

            // Browser-compatible mouse input reading
            float mouseX = 0f;
            float mouseY = 0f;

            if (Mouse.current != null)
            {
                Vector2 delta = Mouse.current.delta.ReadValue() * (mouseSensitivity * 0.1f);
                mouseX = delta.x;
                mouseY = delta.y;
            }
            else
            {
                mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
                mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
            }

            Vector2 rawInput = new Vector2(mouseX, mouseY);
            currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, rawInput, ref smoothMouseVelocity, smoothTime);

            currentYaw += currentMouseDelta.x;
            currentPitch -= currentMouseDelta.y;
            currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);

            // Scroll zoom
            float scroll = 0f;
            if (Mouse.current != null)
            {
                scroll = Mouse.current.scroll.ReadValue().y;
            }
            else
            {
                scroll = Input.mouseScrollDelta.y;
            }

            if (Mathf.Abs(scroll) > 0.01f)
            {
                targetDistance = Mathf.Clamp(targetDistance - Mathf.Sign(scroll) * zoomSpeed * 0.5f, minDistance, maxDistance);
            }
        }

        private void UpdateCameraPosition()
        {
            Vector3 pivot = target.position + pivotOffset;
            Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
            Vector3 desiredDir = rotation * -Vector3.forward;

            float desiredDist = targetDistance;

            // Spring Arm / Camera Collision handling
            if (enableCollisionAvoidance)
            {
                Ray ray = new Ray(pivot, desiredDir);
                if (Physics.SphereCast(ray, collisionRadius, out RaycastHit hit, targetDistance, collisionLayers))
                {
                    desiredDist = Mathf.Max(minDistance, hit.distance - collisionPadding);
                }
            }

            currentDistance = Mathf.Lerp(currentDistance, desiredDist, 15f * Time.deltaTime);

            transform.position = pivot + desiredDir * currentDistance;
            transform.rotation = rotation;
        }
    }
}
