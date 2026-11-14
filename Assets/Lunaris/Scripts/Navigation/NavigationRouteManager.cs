using System;
using System.Collections.Generic;
using UnityEngine;
using Lunaris.Core;

namespace Lunaris.Navigation
{
    public enum NavigationCorridorState
    {
        Normal,      // < 80% corridor radius
        Caution,     // 80% - 100% corridor radius
        OutOfRoute,  // > 100% corridor radius
        Recovery,    // Countdown active
        Recovered    // Returned to safety
    }

    public class NavigationRouteManager : MonoBehaviour
    {
        public static NavigationRouteManager Instance { get; private set; }

        [Header("Route Configuration")]
        [Tooltip("Allowed flight corridor radius in meters")]
        [SerializeField] private float corridorRadius = 65f;
        public float CorridorRadius => corridorRadius;

        [Tooltip("Duration in seconds for the recovery countdown")]
        [SerializeField] private float recoveryCountdownDuration = 15f;

        [Header("Waypoints & Checkpoints")]
        [SerializeField] private List<RouteCheckpoint> checkpoints = new List<RouteCheckpoint>();
        public IReadOnlyList<RouteCheckpoint> Checkpoints => checkpoints;

        [SerializeField] private int activeCheckpointIndex = 0;
        public int ActiveCheckpointIndex => activeCheckpointIndex;
        public RouteCheckpoint ActiveCheckpoint => (checkpoints != null && activeCheckpointIndex < checkpoints.Count) ? checkpoints[activeCheckpointIndex] : null;

        [Header("Live Navigation Telemetry")]
        [SerializeField] private NavigationCorridorState corridorState = NavigationCorridorState.Normal;
        public NavigationCorridorState CorridorState => corridorState;

        [SerializeField] private float currentDeviation = 0f;
        public float CurrentDeviation => currentDeviation;

        [SerializeField] private float currentRecoveryCountdown = 15f;
        public float CurrentRecoveryCountdown => currentRecoveryCountdown;

        [SerializeField] private Vector3 recoveryDirection = Vector3.zero;
        public Vector3 RecoveryDirection => recoveryDirection;

        [SerializeField] private Vector3 closestPointOnRoute = Vector3.zero;
        public Vector3 ClosestPointOnRoute => closestPointOnRoute;

        [SerializeField] private float totalRouteProgress = 0f;
        public float TotalRouteProgress => totalRouteProgress;

        public event Action<NavigationCorridorState> OnCorridorStateChanged;
        public event Action<int, RouteCheckpoint> OnCheckpointReached;

        private Transform spacecraftTransform;
        private bool isCountdownActive = false;
        private bool hasDestructionTriggered = false;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            if (checkpoints.Count == 0)
            {
                GenerateTransLunarRoute();
            }
        }

        private void Start()
        {
            currentRecoveryCountdown = recoveryCountdownDuration;
        }

        private void Update()
        {
            if (spacecraftTransform == null)
            {
                FindSpacecraft();
                if (spacecraftTransform == null) return;
            }

            UpdateRouteMath();
            UpdateCheckpoints();
            UpdateCorridorStatus();
        }

        public void RegisterSpacecraft(Transform craft)
        {
            spacecraftTransform = craft;
        }

        private void FindSpacecraft()
        {
            GameObject craftObj = GameObject.FindWithTag("Player");
            if (craftObj != null)
            {
                spacecraftTransform = craftObj.transform;
            }
        }

        public void GenerateTransLunarRoute()
        {
            checkpoints.Clear();
            // Earth Low Orbit Insertion
            checkpoints.Add(new RouteCheckpoint(0, "TLI Ignition (Earth Orbit)", new Vector3(0, 0, 0), 40f));
            // Trans-Lunar Waypoint 1 (Van Allen Belt Clearance)
            checkpoints.Add(new RouteCheckpoint(1, "Van Allen Clearance", new Vector3(25, 10, 200), 50f));
            // Mid-Course Correction 1
            checkpoints.Add(new RouteCheckpoint(2, "Mid-Course Correction Alpha", new Vector3(60, 25, 450), 55f));
            // Equigravis Boundary (Earth-Moon Neutral Gravity)
            checkpoints.Add(new RouteCheckpoint(3, "Equigravis Boundary", new Vector3(40, -15, 750), 60f));
            // Lunar Orbit Insertion Approach
            checkpoints.Add(new RouteCheckpoint(4, "LOI Approach Vector", new Vector3(15, -5, 1050), 50f));
            // Lunar South Pole Landing Corridor
            checkpoints.Add(new RouteCheckpoint(5, "Shackleton Crater Gateway", new Vector3(0, 0, 1300), 45f));

            Debug.Log($"[NavigationRouteManager] Generated {checkpoints.Count} Trans-Lunar route checkpoints.");
        }

        private void UpdateRouteMath()
        {
            if (checkpoints.Count < 2) return;

            Vector3 craftPos = spacecraftTransform.position;

            // Identify current active segment [A, B]
            int prevIdx = Mathf.Max(0, activeCheckpointIndex - 1);
            int nextIdx = Mathf.Min(checkpoints.Count - 1, activeCheckpointIndex);

            if (prevIdx == nextIdx)
            {
                nextIdx = Mathf.Min(checkpoints.Count - 1, prevIdx + 1);
            }

            Vector3 segA = checkpoints[prevIdx].position;
            Vector3 segB = checkpoints[nextIdx].position;

            Vector3 segVector = segB - segA;
            float segLenSqr = segVector.sqrMagnitude;

            if (segLenSqr > 0.0001f)
            {
                float t = Vector3.Dot(craftPos - segA, segVector) / segLenSqr;
                t = Mathf.Clamp01(t);
                closestPointOnRoute = segA + t * segVector;
            }
            else
            {
                closestPointOnRoute = segA;
            }

            // Perpendicular deviation vector from craft to closest route point
            Vector3 diff = craftPos - closestPointOnRoute;
            currentDeviation = diff.magnitude;

            // Unit direction pointing back into the center of the corridor
            recoveryDirection = (closestPointOnRoute - craftPos).normalized;

            // Route completion percentage
            float totalDist = checkpoints[checkpoints.Count - 1].position.z - checkpoints[0].position.z;
            float currentZ = Mathf.Clamp(craftPos.z - checkpoints[0].position.z, 0, totalDist);
            totalRouteProgress = totalDist > 0 ? (currentZ / totalDist) * 100f : 0f;

            if (LunarisGameManager.Instance != null && LunarisGameManager.Instance.CurrentMission != null)
            {
                bool compliant = currentDeviation <= corridorRadius;
                LunarisGameManager.Instance.CurrentMission.RecordDeviation(currentDeviation, compliant);
            }
        }

        private void UpdateCheckpoints()
        {
            if (activeCheckpointIndex >= checkpoints.Count) return;

            RouteCheckpoint cp = checkpoints[activeCheckpointIndex];
            float dist = Vector3.Distance(spacecraftTransform.position, cp.position);

            if (dist <= cp.arrivalRadius)
            {
                cp.isCompleted = true;
                Debug.Log($"[NavigationRouteManager] Checkpoint {cp.sequenceNumber} ({cp.checkpointName}) Completed!");
                OnCheckpointReached?.Invoke(activeCheckpointIndex, cp);

                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlayCheckpointSound();
                }

                activeCheckpointIndex++;

                // Destination Reached!
                if (activeCheckpointIndex >= checkpoints.Count)
                {
                    Debug.Log("[NavigationRouteManager] Moon Destination Reached! Starting Lunar Landing Sequence.");
                    if (LunarisGameManager.Instance != null)
                    {
                        LunarisGameManager.Instance.TransitionToState(GameState.LunarLanding);
                    }
                }
            }
        }

        private void UpdateCorridorStatus()
        {
            NavigationCorridorState previous = corridorState;

            float cautionThreshold = corridorRadius * 0.8f;

            if (currentDeviation <= cautionThreshold)
            {
                // Normal
                corridorState = NavigationCorridorState.Normal;
                if (isCountdownActive)
                {
                    CancelRecoveryCountdown();
                }
            }
            else if (currentDeviation <= corridorRadius)
            {
                // Caution
                corridorState = NavigationCorridorState.Caution;
                if (isCountdownActive)
                {
                    CancelRecoveryCountdown();
                }
            }
            else
            {
                // Out of Route
                corridorState = NavigationCorridorState.OutOfRoute;
                StartOrContinueCountdown();
            }

            if (corridorState != previous)
            {
                OnCorridorStateChanged?.Invoke(corridorState);
            }
        }

        private void StartOrContinueCountdown()
        {
            if (!isCountdownActive)
            {
                isCountdownActive = true;
                currentRecoveryCountdown = recoveryCountdownDuration;
                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.StartAlarmSiren(false);
                }
            }
            else
            {
                currentRecoveryCountdown -= Time.deltaTime;

                if (currentRecoveryCountdown <= 5f && SoundManager.Instance != null)
                {
                    SoundManager.Instance.StartAlarmSiren(true); // Escalated siren
                }

                if (currentRecoveryCountdown <= 0f && !hasDestructionTriggered)
                {
                    hasDestructionTriggered = true;
                    TriggerCorridorBreachDestruction();
                }
            }
        }

        private void CancelRecoveryCountdown()
        {
            if (isCountdownActive)
            {
                isCountdownActive = false;
                currentRecoveryCountdown = recoveryCountdownDuration;
                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.StopAlarmSiren();
                }
                corridorState = NavigationCorridorState.Recovered;
                OnCorridorStateChanged?.Invoke(corridorState);
                Debug.Log("[NavigationRouteManager] Route Recovered successfully.");
            }
        }

        private void TriggerCorridorBreachDestruction()
        {
            Debug.LogError("[NavigationRouteManager] Corridor breach countdown expired! Spacecraft destruction initiated.");
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.StopAlarmSiren();
                SoundManager.Instance.PlayExplosion();
            }

            if (LunarisGameManager.Instance != null)
            {
                LunarisGameManager.Instance.FailMission($"Navigation corridor breach (Max deviation: {currentDeviation:F1}m)");
            }
        }
    }
}
