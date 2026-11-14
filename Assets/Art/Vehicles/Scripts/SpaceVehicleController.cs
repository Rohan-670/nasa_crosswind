using UnityEngine;

namespace NASA.Crosswind.Vehicles
{
    /// <summary>
    /// Space Exploration Vehicle (SEV) Controller
    /// Controls 6-wheel planetary surface locomotion, RCS attitude thrusters,
    /// VTOL descent thrusters, lighting, solar panel deployment, and science equipment.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [AddComponentMenu("NASA/Vehicles/Space Vehicle Controller")]
    public class SpaceVehicleController : MonoBehaviour
    {
        [Header("Locomotion (6-Wheel Drive)")]
        [Tooltip("Motor acceleration torque applied to wheels")]
        [SerializeField] private float motorForce = 1500f;

        [Tooltip("Max speed in meters per second (approx 35 km/h for SEV)")]
        [SerializeField] private float maxSpeed = 10f;

        [Tooltip("Steering turn speed in degrees per second")]
        [SerializeField] private float turnSpeed = 45f;

        [Tooltip("Braking deceleration force")]
        [SerializeField] private float brakeForce = 2500f;

        [Header("Planetary Thrusters & RCS (Low-Gravity / Flight Mode)")]
        [Tooltip("Upward thrust from main descent rocket bells")]
        [SerializeField] private float descentThrustForce = 3500f;

        [Tooltip("Attitude torque from 4 quad RCS thruster blocks")]
        [SerializeField] private float rcsTorque = 1200f;

        [Header("Wheel Transforms")]
        [SerializeField] private Transform wheelFrontLeft;
        [SerializeField] private Transform wheelFrontRight;
        [SerializeField] private Transform wheelMidLeft;
        [SerializeField] private Transform wheelMidRight;
        [SerializeField] private Transform wheelRearLeft;
        [SerializeField] private Transform wheelRearRight;

        [Header("Solar Array Wings")]
        [SerializeField] private Transform solarWingLeft;
        [SerializeField] private Transform solarWingRight;
        [SerializeField] private bool solarWingsDeployed = true;
        [SerializeField] private float wingDeploySpeed = 45f;

        [Header("Science & Telemetry")]
        [SerializeField] private Transform antennaHighGain;
        [SerializeField] private Transform sensorMast;
        [SerializeField] private Transform roboticArm;

        [Header("Lighting Systems")]
        [SerializeField] private Light[] forwardHeadlights;
        [SerializeField] private Light[] interiorCockpitLights;
        [SerializeField] private ParticleSystem descentThrusterParticles;
        [SerializeField] private ParticleSystem[] rcsThrusterParticles;

        [Header("Telemetry Status (Read-Only)")]
        [SerializeField] private float currentSpeedKmh;
        [SerializeField] private bool isGrounded = true;
        [SerializeField] private float powerReservePercent = 100f;

        private Rigidbody rb;
        private float currentSteerAngle = 0f;
        private bool headlightsActive = true;
        private float wingAngleCurrent = 0f;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.mass = 1200f; // 1.2 metric tons (NASA SEV dry mass)
            rb.centerOfMass = new Vector3(0f, 0.65f, -0.1f); // Low center of gravity
            rb.linearDamping = 0.5f;
            rb.angularDamping = 1.5f;

            AutoFindVehicleParts();
        }

        private void Start()
        {
            SetHeadlights(headlightsActive);
            wingAngleCurrent = solarWingsDeployed ? 0f : 85f;
        }

        private void Update()
        {
            HandleInputs();
            UpdateWheelVisuals();
            UpdateSolarWings();
            UpdateAntennaTracking();
            UpdateTelemetry();
        }

        private void FixedUpdate()
        {
            float moveInput = Input.GetAxis("Vertical");
            float steerInput = Input.GetAxis("Horizontal");
            bool jumpInput = Input.GetKey(KeyCode.Space);

            CheckGrounded();

            if (isGrounded)
            {
                ApplyDrive(moveInput, steerInput);
            }
            else
            {
                ApplyRCSControl(steerInput, moveInput);
            }

            if (jumpInput)
            {
                ApplyDescentThrust();
            }
            else if (descentThrusterParticles != null && descentThrusterParticles.isPlaying)
            {
                descentThrusterParticles.Stop();
            }
        }

        private void HandleInputs()
        {
            // Toggle Headlights (Key L)
            if (Input.GetKeyDown(KeyCode.L))
            {
                headlightsActive = !headlightsActive;
                SetHeadlights(headlightsActive);
            }

            // Toggle Solar Array Deploy/Retract (Key X)
            if (Input.GetKeyDown(KeyCode.X))
            {
                solarWingsDeployed = !solarWingsDeployed;
            }
        }

        private void ApplyDrive(float move, float steer)
        {
            float speed = Vector3.Dot(rb.linearVelocity, transform.forward);

            if (Mathf.Abs(move) > 0.05f)
            {
                if (Mathf.Abs(speed) < maxSpeed)
                {
                    Vector3 force = transform.forward * (move * motorForce);
                    rb.AddForce(force, ForceMode.Force);
                }
            }
            else
            {
                // Active rolling friction / braking
                rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, new Vector3(0, rb.linearVelocity.y, 0), Time.fixedDeltaTime * (brakeForce / rb.mass));
            }

            // Steering: Yaw rotation
            if (Mathf.Abs(steer) > 0.05f)
            {
                float steerFactor = Mathf.Clamp(speed / 3f, -1f, 1f);
                if (Mathf.Abs(steerFactor) < 0.2f && Mathf.Abs(move) > 0.05f)
                {
                    steerFactor = Mathf.Sign(move) * 0.5f; // Low speed pivot
                }

                float yaw = steer * turnSpeed * (Mathf.Abs(move) > 0.01f ? steerFactor : 0.8f) * Time.fixedDeltaTime;
                rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, yaw, 0f));
            }
        }

        private void ApplyRCSControl(float roll, float pitch)
        {
            Vector3 torque = (transform.forward * (-roll) + transform.right * pitch) * rcsTorque;
            rb.AddTorque(torque, ForceMode.Force);
        }

        private void ApplyDescentThrust()
        {
            rb.AddForce(transform.up * descentThrustForce, ForceMode.Force);

            if (descentThrusterParticles != null && !descentThrusterParticles.isPlaying)
            {
                descentThrusterParticles.Play();
            }
        }

        private void CheckGrounded()
        {
            // Raycast down from vehicle chassis
            Ray ray = new Ray(transform.position + Vector3.up * 0.5f, Vector3.down);
            isGrounded = Physics.Raycast(ray, 0.85f);
        }

        private void UpdateWheelVisuals()
        {
            float speed = Vector3.Dot(rb.linearVelocity, transform.forward);
            float rotationAmount = (speed / 0.44f) * Mathf.Rad2Deg * Time.deltaTime;

            Transform[] wheels = new Transform[] {
                wheelFrontLeft, wheelFrontRight,
                wheelMidLeft,   wheelMidRight,
                wheelRearLeft,  wheelRearRight
            };

            foreach (Transform w in wheels)
            {
                if (w != null)
                {
                    w.Rotate(Vector3.right, rotationAmount, Space.Self);
                }
            }

            // Front wheel steering angle visual
            float targetSteerAngle = Input.GetAxis("Horizontal") * 25f;
            currentSteerAngle = Mathf.Lerp(currentSteerAngle, targetSteerAngle, Time.deltaTime * 8f);

            if (wheelFrontLeft != null) wheelFrontLeft.localRotation = Quaternion.Euler(0f, currentSteerAngle, 0f);
            if (wheelFrontRight != null) wheelFrontRight.localRotation = Quaternion.Euler(0f, currentSteerAngle, 0f);
            if (wheelRearLeft != null) wheelRearLeft.localRotation = Quaternion.Euler(0f, -currentSteerAngle * 0.5f, 0f);
            if (wheelRearRight != null) wheelRearRight.localRotation = Quaternion.Euler(0f, -currentSteerAngle * 0.5f, 0f);
        }

        private void UpdateSolarWings()
        {
            float targetAngle = solarWingsDeployed ? 0f : 85f;
            wingAngleCurrent = Mathf.MoveTowards(wingAngleCurrent, targetAngle, Time.deltaTime * wingDeploySpeed);

            if (solarWingLeft != null)
            {
                solarWingLeft.localRotation = Quaternion.Euler(0f, 0f, -wingAngleCurrent);
            }
            if (solarWingRight != null)
            {
                solarWingRight.localRotation = Quaternion.Euler(0f, 0f, wingAngleCurrent);
            }
        }

        private void UpdateAntennaTracking()
        {
            if (antennaHighGain != null)
            {
                // Slowly track celestial target / base station
                antennaHighGain.Rotate(Vector3.up, 4f * Time.deltaTime, Space.Self);
            }
        }

        private void UpdateTelemetry()
        {
            currentSpeedKmh = rb.linearVelocity.magnitude * 3.6f;
        }

        public void SetHeadlights(bool active)
        {
            headlightsActive = active;
            if (forwardHeadlights != null)
            {
                foreach (Light light in forwardHeadlights)
                {
                    if (light != null) light.enabled = active;
                }
            }
        }

        public void AutoFindVehicleParts()
        {
            if (wheelFrontLeft == null) wheelFrontLeft = transform.Find("Wheel_Front_Left");
            if (wheelFrontRight == null) wheelFrontRight = transform.Find("Wheel_Front_Right");
            if (wheelMidLeft == null) wheelMidLeft = transform.Find("Wheel_Mid_Left");
            if (wheelMidRight == null) wheelMidRight = transform.Find("Wheel_Mid_Right");
            if (wheelRearLeft == null) wheelRearLeft = transform.Find("Wheel_Rear_Left");
            if (wheelRearRight == null) wheelRearRight = transform.Find("Wheel_Rear_Right");

            if (solarWingLeft == null) solarWingLeft = transform.Find("Solar_Wing_Left");
            if (solarWingRight == null) solarWingRight = transform.Find("Solar_Wing_Right");

            if (antennaHighGain == null) antennaHighGain = transform.Find("Antenna_HighGain");
            if (sensorMast == null) sensorMast = transform.Find("Sensor_Mast");
            if (roboticArm == null) roboticArm = transform.Find("Robotic_Arm");
        }
    }
}
