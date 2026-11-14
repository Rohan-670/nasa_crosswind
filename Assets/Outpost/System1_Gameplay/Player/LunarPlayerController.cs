using UnityEngine;
using UnityEngine.InputSystem;
using Outpost.Core.Events;

namespace Outpost.System1.Gameplay.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class LunarPlayerController : MonoBehaviour
    {
        [Header("Movement Speeds")]
        [SerializeField] private float walkSpeed = 3.5f;
        [SerializeField] private float sprintSpeed = 5.5f;

        [Header("Lunar Physics")]
        [Tooltip("Realistic Moon surface gravity (-1.62 m/s^2)")]
        [SerializeField] private float lunarGravity = -1.62f;
        [Tooltip("Low-gravity jump impulse")]
        [SerializeField] private float jumpHeight = 2.2f;
        [Tooltip("Air inertia damping in lunar vacuum")]
        [SerializeField] private float airInertiaFactor = 0.85f;

        [Header("Camera Look")]
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private float mouseSensitivity = 1.8f;
        [SerializeField] private float lookXLimit = 80f;

        private CharacterController characterController;
        private Vector3 moveDirection = Vector3.zero;
        private float verticalVelocity = 0f;
        private float rotationX = 0f;
        private bool isGrounded = false;
        private bool isControlLocked = false;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            if (cameraTransform == null)
            {
                Camera cam = GetComponentInChildren<Camera>();
                if (cam != null) cameraTransform = cam.transform;
            }
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void OnEnable()
        {
            OutpostEventChannel.GeoMapToggled += HandleModalToggled;
            OutpostEventChannel.FiltrationChallengeOpened += HandleModalOpened;
            OutpostEventChannel.FiltrationChallengeCompleted += HandleModalClosed;
            OutpostEventChannel.StrategicChoiceOpened += HandleModalOpened;
            OutpostEventChannel.RepairChoiceMade += HandleChoiceClosed;
        }

        private void OnDisable()
        {
            OutpostEventChannel.GeoMapToggled -= HandleModalToggled;
            OutpostEventChannel.FiltrationChallengeOpened -= HandleModalOpened;
            OutpostEventChannel.FiltrationChallengeCompleted -= HandleModalClosed;
            OutpostEventChannel.StrategicChoiceOpened -= HandleModalOpened;
            OutpostEventChannel.RepairChoiceMade -= HandleChoiceClosed;
        }

        private void HandleModalToggled(bool open) => SetControlLock(open);
        private void HandleModalOpened() => SetControlLock(true);
        private void HandleModalClosed(bool _) => SetControlLock(false);
        private void HandleChoiceClosed(Outpost.Core.Data.RepairChoice _) => SetControlLock(false);

        public void SetControlLock(bool locked)
        {
            isControlLocked = locked;
            Cursor.lockState = locked ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = locked;
        }

        private void Update()
        {
            if (isControlLocked) return;

            HandleLook();
            HandleMovement();
        }

        private void HandleLook()
        {
            if (cameraTransform == null) return;

            float mouseX = 0f;
            float mouseY = 0f;

            if (Mouse.current != null)
            {
                Vector2 delta = Mouse.current.delta.ReadValue() * (mouseSensitivity * 0.1f);
                mouseX = delta.x;
                mouseY = delta.y;
            }

            rotationX -= mouseY;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            cameraTransform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);

            transform.Rotate(Vector3.up * mouseX);
        }

        private void HandleMovement()
        {
            isGrounded = characterController.isGrounded;

            if (isGrounded && verticalVelocity < 0f)
            {
                verticalVelocity = -0.5f; // Small ground stick force
            }

            // Input reading
            float horizontal = 0f;
            float vertical = 0f;

            if (Keyboard.current != null)
            {
                if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) vertical += 1f;
                if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) vertical -= 1f;
                if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) horizontal -= 1f;
                if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) horizontal += 1f;
            }

            bool isSprinting = Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed;
            float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

            Vector3 targetMove = (transform.right * horizontal + transform.forward * vertical).normalized * currentSpeed;

            if (isGrounded)
            {
                moveDirection = targetMove;

                // Lunar low-gravity jump: v = sqrt(2 * g * h)
                if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
                {
                    verticalVelocity = Mathf.Sqrt(2f * Mathf.Abs(lunarGravity) * jumpHeight);
                    OutpostEventChannel.RaisePlaySfx("lunar_hop");
                }
            }
            else
            {
                // Air control with lunar momentum
                moveDirection = Vector3.Lerp(moveDirection, targetMove, airInertiaFactor * Time.deltaTime);
            }

            // Apply realistic lunar gravity
            verticalVelocity += lunarGravity * Time.deltaTime;

            Vector3 finalVelocity = moveDirection;
            finalVelocity.y = verticalVelocity;

            characterController.Move(finalVelocity * Time.deltaTime);
        }
    }
}
