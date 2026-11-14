using UnityEngine;
using Lunaris.Core;

namespace Lunaris.Spacecraft
{
    [RequireComponent(typeof(Rigidbody))]
    public class SpacecraftFlightController : MonoBehaviour
    {
        [Header("Flight Dynamics")]
        [SerializeField] private float thrustForce = 12000f;
        [SerializeField] private float reverseThrustForce = 6000f;
        [SerializeField] private float boostMultiplier = 2.0f;
        [SerializeField] private float verticalThrustForce = 8000f;

        [Header("Attitude Torque (Pitch, Yaw, Roll)")]
        [SerializeField] private float pitchTorque = 4500f;
        [SerializeField] private float yawTorque = 4500f;
        [SerializeField] private float rollTorque = 3500f;

        [Header("Speed & Safety Limits")]
        [SerializeField] private float maxNormalSpeed = 65f;
        [SerializeField] private float maxBoostSpeed = 120f;

        [Header("Resources & Integrity")]
        [SerializeField] private float maxFuel = 100f;
        [SerializeField] private float currentFuel = 100f;
        [SerializeField] private float fuelBurnRatePerSecond = 1.5f;
        [SerializeField] private float boostFuelMultiplier = 3.0f;
        [SerializeField] private float hullIntegrity = 100f;

        public float CurrentSpeed => rb != null ? rb.linearVelocity.magnitude : 0f;
        public float CurrentSpeedKmh => CurrentSpeed * 3.6f;
        public float FuelPercentage => (currentFuel / maxFuel) * 100f;
        public float HullIntegrityPercentage => hullIntegrity;
        public bool IsBoosting => isBoosting;
        public float ThrottleInput => throttleInput;

        private Rigidbody rb;
        private bool isBoosting = false;
        private float throttleInput = 0f;
        private bool controlsEnabled = true;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            ConfigureRigidbody();
        }

        private void ConfigureRigidbody()
        {
            rb.mass = 4500f; // 4.5 metric tons deep-space crewed craft
            rb.linearDamping = 0.25f; // Slight space drag for playable stability
            rb.angularDamping = 2.0f; // High angular damping prevents uncontrolled tumbling
            rb.useGravity = false;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }

        private void Update()
        {
            if (!controlsEnabled) return;

            ReadFlightInput();
        }

        private void FixedUpdate()
        {
            if (!controlsEnabled) return;

            ApplyThrust();
            ApplyAttitudeTorque();
            ConsumeFuel();
        }

        private void ReadFlightInput()
        {
            // W / S: Throttle
            throttleInput = Input.GetAxis("Vertical");

            // Left Shift: Boost
            isBoosting = Input.GetKey(KeyCode.LeftShift) && currentFuel > 0f && throttleInput > 0f;
        }

        private void ApplyThrust()
        {
            Vector3 force = Vector3.zero;

            // Forward / Reverse
            if (throttleInput > 0.05f)
            {
                float multiplier = isBoosting ? boostMultiplier : 1.0f;
                force += transform.forward * (throttleInput * thrustForce * multiplier);
            }
            else if (throttleInput < -0.05f)
            {
                force += transform.forward * (throttleInput * reverseThrustForce);
            }

            // Space: Upward vertical thrust
            if (Input.GetKey(KeyCode.Space))
            {
                force += transform.up * verticalThrustForce;
            }

            rb.AddForce(force, ForceMode.Force);

            // Speed clamping
            float speedLimit = isBoosting ? maxBoostSpeed : maxNormalSpeed;
            if (rb.linearVelocity.magnitude > speedLimit)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * speedLimit;
            }

            // Audio feedback
            if (SoundManager.Instance != null)
            {
                float enginePower = Mathf.Abs(throttleInput) * (isBoosting ? 1.5f : 1.0f);
                SoundManager.Instance.SetEngineThrust(enginePower);
            }
        }

        private void ApplyAttitudeTorque()
        {
            float yawInput = Input.GetAxis("Horizontal"); // A / D
            float rollInput = 0f;
            if (Input.GetKey(KeyCode.Q)) rollInput += 1f;
            if (Input.GetKey(KeyCode.E)) rollInput -= 1f;

            // Mouse pitch / yaw look
            float mousePitch = -Input.GetAxis("Mouse Y") * 1.5f;
            float mouseYaw = Input.GetAxis("Mouse X") * 1.5f;

            Vector3 torque = (transform.up * (yawInput + mouseYaw) * yawTorque) +
                             (transform.forward * rollInput * rollTorque) +
                             (transform.right * mousePitch * pitchTorque);

            rb.AddTorque(torque, ForceMode.Force);
        }

        private void ConsumeFuel()
        {
            if (Mathf.Abs(throttleInput) > 0.05f)
            {
                float rate = fuelBurnRatePerSecond * (isBoosting ? boostFuelMultiplier : 1.0f);
                currentFuel = Mathf.Max(0f, currentFuel - rate * Time.fixedDeltaTime);
            }
        }

        public void DisableControls()
        {
            controlsEnabled = false;
            throttleInput = 0f;
            isBoosting = false;
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.SetEngineThrust(0f);
            }
        }

        public void EnableControls()
        {
            controlsEnabled = true;
        }

        public void TakeDamage(float amount)
        {
            hullIntegrity = Mathf.Max(0f, hullIntegrity - amount);
            if (hullIntegrity <= 0f)
            {
                DisableControls();
                GetComponent<SpacecraftDestruction>()?.TriggerDestruction();
            }
        }
    }
}
