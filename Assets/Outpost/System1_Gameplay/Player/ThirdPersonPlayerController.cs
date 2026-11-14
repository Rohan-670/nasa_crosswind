using UnityEngine;
using UnityEngine.InputSystem;
using Outpost.Core.Events;

namespace Outpost.System1.Gameplay.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class ThirdPersonPlayerController : MonoBehaviour
    {
        [Header("Movement Speeds (Configurable)")]
        [SerializeField] private float walkSpeed = 3.2f;
        [SerializeField] private float runSpeed = 6.2f;
        [SerializeField] private float acceleration = 12.0f;
        [SerializeField] private float deceleration = 10.0f;
        [SerializeField] private float rotationSpeed = 10.0f;

        [Header("Lunar Physics")]
        [Tooltip("Realistic lunar surface gravity (-1.62 m/s^2)")]
        [SerializeField] private float lunarGravity = -1.62f;
        [Tooltip("Buoyant jump height in low gravity")]
        [SerializeField] private float jumpHeight = 2.2f;
        [Tooltip("Vacuum inertial preservation when airborne (0 to 1)")]
        [SerializeField] private float airControlDamping = 0.85f;

        [Header("Ground Detection & Collisions")]
        [SerializeField] private float groundCheckDistance = 0.2f;
        [SerializeField] private LayerMask groundLayers = ~0;

        [Header("Camera Reference")]
        [SerializeField] private ThirdPersonCameraController cameraController;

        private CharacterController characterController;
        private Vector3 currentVelocity = Vector3.zero;
        private float verticalVelocity = 0f;
        private bool isGrounded = false;
        private bool isControlLocked = false;
        private float footstepTimer = 0f;

        public CharacterController Controller => characterController;
        public Vector3 Velocity => characterController != null ? characterController.velocity : Vector3.zero;
        public bool IsGrounded => isGrounded;
        public float WalkSpeed { get => walkSpeed; set => walkSpeed = value; }
        public float RunSpeed { get => runSpeed; set => runSpeed = value; }
        public float LunarGravity { get => lunarGravity; set => lunarGravity = value; }

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
        }

        private void Start()
        {
            if (cameraController == null)
            {
                cameraController = FindAnyObjectByType<ThirdPersonCameraController>();
            }
        }

        public void SetControlLock(bool locked)
        {
            isControlLocked = locked;
            if (locked) currentVelocity = Vector3.zero;
            if (cameraController != null) cameraController.SetControlLock(locked);
        }

        private void Update()
        {
            CheckGroundStatus();

            if (isControlLocked)
            {
                ApplyGravityOnly();
                return;
            }

            HandleMovement();
            HandleFootsteps();
        }

        private void CheckGroundStatus()
        {
            // Primary check via CharacterController
            bool ccGrounded = characterController.isGrounded;

            // Supplementary raycast for soft lunar slope detection
            Vector3 origin = transform.position + Vector3.up * 0.1f;
            bool rayGrounded = Physics.Raycast(origin, Vector3.down, 0.15f + groundCheckDistance, groundLayers);

            isGrounded = ccGrounded || rayGrounded;

            if (isGrounded && verticalVelocity < 0f)
            {
                verticalVelocity = -0.5f; // Small ground adherence bias
            }
        }

        private void HandleMovement()
        {
            // Browser-compatible horizontal and vertical input reading
            float horizontal = 0f;
            float vertical = 0f;

            if (Keyboard.current != null)
            {
                if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) vertical += 1f;
                if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) vertical -= 1f;
                if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) horizontal -= 1f;
                if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) horizontal += 1f;
            }
            else
            {
                horizontal = Input.GetAxisRaw("Horizontal");
                vertical = Input.GetAxisRaw("Vertical");
            }

            Vector2 inputDir = new Vector2(horizontal, vertical).normalized;

            // Determine if running
            bool isRunning = false;
            if (Keyboard.current != null)
            {
                isRunning = Keyboard.current.leftShiftKey.isPressed;
            }
            else
            {
                isRunning = Input.GetKey(KeyCode.LeftShift);
            }

            float targetSpeed = isRunning ? runSpeed : walkSpeed;

            // Calculate movement direction relative to camera horizontal plane
            Vector3 camForward = cameraController != null ? cameraController.ForwardOnPlane : Vector3.forward;
            Vector3 camRight = cameraController != null ? cameraController.RightOnPlane : Vector3.right;

            Vector3 desiredMoveDir = (camForward * inputDir.y + camRight * inputDir.x).normalized;

            // Smooth Character Rotation toward movement vector
            if (desiredMoveDir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(desiredMoveDir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            Vector3 targetVelocity = desiredMoveDir * targetSpeed;

            if (isGrounded)
            {
                // Smooth acceleration / deceleration on ground
                float rate = desiredMoveDir.sqrMagnitude > 0.001f ? acceleration : deceleration;
                currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, rate * Time.deltaTime);

                // Lunar low-gravity jump: v = sqrt(2 * |g| * h)
                bool jumpPressed = false;
                if (Keyboard.current != null) jumpPressed = Keyboard.current.spaceKey.wasPressedThisFrame;
                else jumpPressed = Input.GetKeyDown(KeyCode.Space);

                if (jumpPressed)
                {
                    verticalVelocity = Mathf.Sqrt(2f * Mathf.Abs(lunarGravity) * jumpHeight);
                    OutpostEventChannel.RaisePlaySfx("lunar_hop");
                }
            }
            else
            {
                // Air control with lunar momentum glide
                currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, airControlDamping * Time.deltaTime);
            }

            // Apply realistic lunar gravity acceleration
            verticalVelocity += lunarGravity * Time.deltaTime;

            Vector3 finalMotion = currentVelocity;
            finalMotion.y = verticalVelocity;

            // Move the CharacterController
            characterController.Move(finalMotion * Time.deltaTime);
        }

        private void ApplyGravityOnly()
        {
            verticalVelocity += lunarGravity * Time.deltaTime;
            characterController.Move(new Vector3(0f, verticalVelocity, 0f) * Time.deltaTime);
        }

        private void HandleFootsteps()
        {
            if (!isGrounded) return;

            float horizontalSpeed = new Vector3(characterController.velocity.x, 0f, characterController.velocity.z).magnitude;
            if (horizontalSpeed > 0.4f)
            {
                footstepTimer += Time.deltaTime * horizontalSpeed;
                if (footstepTimer >= 3.2f)
                {
                    footstepTimer = 0f;
                    OutpostEventChannel.RaisePlaySfx("interact");
                }
            }
            else
            {
                footstepTimer = 0f;
            }
        }
    }
}
