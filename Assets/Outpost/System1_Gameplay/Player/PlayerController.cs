using UnityEngine;
using UnityEngine.InputSystem;
using Outpost.Core.Events;

namespace Outpost.System1.Gameplay.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Speeds")]
        [SerializeField] private float walkSpeed = 3.5f;
        [SerializeField] private float sprintSpeed = 5.5f;

        [Header("Lunar Physics")]
        [Tooltip("Realistic lunar surface gravity (-1.62 m/s^2)")]
        [SerializeField] private float lunarGravity = -1.62f;
        [Tooltip("Buoyant jump height in low gravity")]
        [SerializeField] private float jumpHeight = 2.2f;
        [Tooltip("Air inertia factor in lunar vacuum")]
        [SerializeField] private float airInertiaFactor = 0.85f;

        [Header("Suit Stamina")]
        [SerializeField] private float maxSuitStamina = 100f;
        [SerializeField] private float staminaDrainPerSecond = 12f;
        [SerializeField] private float staminaRecoveryPerSecond = 18f;

        private CharacterController characterController;
        private Vector3 moveDirection = Vector3.zero;
        private float verticalVelocity = 0f;
        private float currentStamina = 100f;
        private bool isGrounded = false;
        private bool isControlLocked = false;
        private float footstepTimer = 0f;

        public Vector3 Velocity => characterController != null ? characterController.velocity : Vector3.zero;
        public bool IsGrounded => isGrounded;
        public float StaminaPercent => currentStamina / maxSuitStamina;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            currentStamina = maxSuitStamina;
        }

        public void SetControlLock(bool locked)
        {
            isControlLocked = locked;
            if (locked) moveDirection = Vector3.zero;
        }

        private void Update()
        {
            if (isControlLocked)
            {
                ApplyGravityOnly();
                return;
            }

            HandleMovement();
            HandleFootsteps();
        }

        private void HandleMovement()
        {
            isGrounded = characterController.isGrounded;

            if (isGrounded && verticalVelocity < 0f)
            {
                verticalVelocity = -0.5f; // Small ground adherence force
            }

            // Input reading via new Input System
            float horizontal = 0f;
            float vertical = 0f;

            if (Keyboard.current != null)
            {
                if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) vertical += 1f;
                if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) vertical -= 1f;
                if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) horizontal -= 1f;
                if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) horizontal += 1f;
            }

            bool wantsSprint = Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed;
            bool canSprint = wantsSprint && currentStamina > 5f && (horizontal != 0f || vertical != 0f);

            if (canSprint)
            {
                currentStamina = Mathf.Max(0f, currentStamina - staminaDrainPerSecond * Time.deltaTime);
            }
            else
            {
                currentStamina = Mathf.Min(maxSuitStamina, currentStamina + staminaRecoveryPerSecond * Time.deltaTime);
            }

            float currentSpeed = canSprint ? sprintSpeed : walkSpeed;
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

        private void ApplyGravityOnly()
        {
            isGrounded = characterController.isGrounded;
            if (isGrounded && verticalVelocity < 0f) verticalVelocity = -0.5f;
            verticalVelocity += lunarGravity * Time.deltaTime;
            characterController.Move(new Vector3(0f, verticalVelocity, 0f) * Time.deltaTime);
        }

        private void HandleFootsteps()
        {
            if (!isGrounded) return;

            float speed = new Vector3(characterController.velocity.x, 0f, characterController.velocity.z).magnitude;
            if (speed > 0.5f)
            {
                footstepTimer += Time.deltaTime * speed;
                if (footstepTimer >= 3.0f)
                {
                    footstepTimer = 0f;
                    OutpostEventChannel.RaisePlaySfx("interact");
                }
            }
        }
    }
}
