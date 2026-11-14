using UnityEngine;
using Lunaris.Core;

namespace Lunaris.LunarSurface
{
    [RequireComponent(typeof(CharacterController))]
    public class AstronautController : MonoBehaviour
    {
        [Header("Lunar Locomotion (1/6th Earth Gravity)")]
        [SerializeField] private float walkSpeed = 3.5f;
        [SerializeField] private float runSpeed = 6.0f;
        [SerializeField] private float jumpHeight = 3.2f; // Bounding lunar leap
        [SerializeField] private float lunarGravity = -3.2f; // Low gravity

        [Header("Camera & Look")]
        [SerializeField] private Camera astronautCamera;
        [SerializeField] private float mouseSensitivity = 2.0f;

        [Header("Equipment")]
        [SerializeField] private Light helmetLight;

        private CharacterController controller;
        private Vector3 velocity;
        private float xRotation = 0f;
        private bool isGrounded;
        private bool helmetLightOn = true;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            if (LunarisGameManager.Instance != null &&
                LunarisGameManager.Instance.CurrentState != GameState.LunarExpedition)
            {
                return;
            }

            HandleLook();
            HandleMovement();
            HandleEquipment();
        }

        private void HandleLook()
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -80f, 85f);

            if (astronautCamera != null)
            {
                astronautCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            }
            transform.Rotate(Vector3.up * mouseX);
        }

        private void HandleMovement()
        {
            isGrounded = controller.isGrounded;
            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -1.5f;
            }

            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
            Vector3 move = transform.right * x + transform.forward * z;

            controller.Move(move * (speed * Time.deltaTime));

            // Lunar Bounding Jump
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * lunarGravity);
            }

            // Apply Lunar Gravity
            velocity.y += lunarGravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }

        private void HandleEquipment()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                helmetLightOn = !helmetLightOn;
                if (helmetLight != null) helmetLight.enabled = helmetLightOn;
            }

            // Interaction key [E] for nearby samples or lander
            if (Input.GetKeyDown(KeyCode.E))
            {
                InteractWithNearbyObjects();
            }
        }

        private void InteractWithNearbyObjects()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, 3.5f);
            foreach (var hit in hits)
            {
                LunarSampleItem sample = hit.GetComponent<LunarSampleItem>();
                if (sample != null)
                {
                    sample.CollectSample();
                    return;
                }

                if (hit.CompareTag("Respawn") || hit.name.Contains("Lander") || hit.name.Contains("Spacecraft"))
                {
                    CheckReturnToLander();
                    return;
                }
            }
        }

        public void CheckReturnToLander()
        {
            if (LunarisGameManager.Instance != null && LunarisGameManager.Instance.CurrentMission != null)
            {
                var mission = LunarisGameManager.Instance.CurrentMission;
                if (mission.samplesCollected >= mission.totalRequiredSamples)
                {
                    mission.hasReturnedToLander = true;
                    Debug.Log("[AstronautController] Returned to lander with all geological samples! Mission complete.");
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    LunarisGameManager.Instance.CompleteMission();
                }
                else
                {
                    Debug.Log($"[AstronautController] Cannot leave yet: {mission.samplesCollected}/{mission.totalRequiredSamples} samples collected.");
                }
            }
        }

        private void OnGUI()
        {
            if (LunarisGameManager.Instance != null && LunarisGameManager.Instance.CurrentState != GameState.LunarExpedition)
                return;

            float screenW = Screen.width;
            float screenH = Screen.height;

            // Reticle Crosshair
            GUI.color = new Color(0f, 1f, 1f, 0.7f);
            GUI.Box(new Rect((screenW - 4) / 2f, (screenH - 4) / 2f, 4, 4), "");
            GUI.color = Color.white;

            // Astronaut HUD Overlay
            float hudW = 320f;
            float hudH = 130f;
            GUI.Box(new Rect(20, 20, hudW, hudH), "EVA SUIT TELEMETRY — LUNAR SOUTH POLE");

            var mission = LunarisGameManager.Instance != null ? LunarisGameManager.Instance.CurrentMission : null;
            int samples = mission != null ? mission.samplesCollected : 0;
            int total = mission != null ? mission.totalRequiredSamples : 3;

            GUIStyle hudText = new GUIStyle(GUI.skin.label) { fontSize = 13 };
            hudText.normal.textColor = Color.cyan;

            GUI.Label(new Rect(35, 48, 290, 22), $"GEOLOGICAL SAMPLES: {samples}/{total}", hudText);
            GUI.Label(new Rect(35, 70, 290, 22), $"O2 RESERVE: 94.2%  |  BATTERY: 98%", hudText);
            GUI.Label(new Rect(35, 92, 290, 22), $"GRAVITY: 1.62 m/s² (0.166 g)", hudText);
            GUI.Label(new Rect(35, 114, 290, 22), samples >= total ? "OBJECTIVE COMPLETE: RETURN TO LANDER [E]" : "SEARCH FOR GLOWING SAMPLES [E]",
                new GUIStyle(hudText) { normal = { textColor = samples >= total ? Color.green : Color.yellow } });
        }
    }
}
