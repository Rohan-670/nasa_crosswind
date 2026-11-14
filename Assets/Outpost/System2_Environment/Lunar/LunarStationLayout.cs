using System;
using System.Collections.Generic;
using UnityEngine;
using Outpost.Core.Data;
using Outpost.Core.Interfaces;
using Outpost.Core.Events;

namespace Outpost.System2.Environment.Lunar
{
    public class LunarStationLayout : MonoBehaviour, IGeoMapService
    {
        [Header("Player Reference")]
        [SerializeField] private Transform playerTransform;

        [Header("World Coordinate Bounds for Map Projection")]
        [SerializeField] private Vector2 worldBoundsMin = new Vector2(-220f, -120f);
        [SerializeField] private Vector2 worldBoundsMax = new Vector2(180f, 150f);

        private List<StationLocationData> locations = new List<StationLocationData>();
        private StationLocationId activeTargetId = StationLocationId.MainHabitat;

        public IReadOnlyList<StationLocationData> AllLocations => locations;
        public Vector3 PlayerWorldPosition => playerTransform != null ? playerTransform.position : Vector3.zero;
        public StationLocationId ActiveTargetId => activeTargetId;

        public event Action<StationLocationId> OnLocationDiscovered;
        public event Action<StationLocationId> OnTargetLocationChanged;

        public static LunarStationLayout Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            InitializeLocations();
        }

        private void Start()
        {
            if (playerTransform == null)
            {
                GameObject player = GameObject.FindWithTag("Player");
                if (player != null) playerTransform = player.transform;
            }
        }

        private void OnEnable()
        {
            OutpostEventChannel.TargetWaypointSet += SetTargetLocation;
            OutpostEventChannel.LocationArrived += MarkLocationDiscovered;
        }

        private void OnDisable()
        {
            OutpostEventChannel.TargetWaypointSet -= SetTargetLocation;
            OutpostEventChannel.LocationArrived -= MarkLocationDiscovered;
        }

        private void InitializeLocations()
        {
            locations.Clear();
            locations.AddRange(StationLocationData.GetDefaultOutpostLayout());
        }

        public StationLocationData GetLocation(StationLocationId id)
        {
            return locations.Find(loc => loc.Id == id);
        }

        public void MarkLocationDiscovered(StationLocationId id)
        {
            StationLocationData loc = GetLocation(id);
            if (loc != null && !loc.IsDiscovered)
            {
                loc.IsDiscovered = true;
                OnLocationDiscovered?.Invoke(id);
            }
        }

        public void SetTargetLocation(StationLocationId id)
        {
            activeTargetId = id;
            OnTargetLocationChanged?.Invoke(id);
        }

        public Vector2 WorldToMapCoordinate(Vector3 worldPos, Vector2 mapRectSize)
        {
            // Normalize world coords [0, 1] relative to lunar station bounds
            float normalizedX = Mathf.InverseLerp(worldBoundsMin.x, worldBoundsMax.x, worldPos.x);
            float normalizedY = Mathf.InverseLerp(worldBoundsMin.y, worldBoundsMax.y, worldPos.z);

            // Center relative map coords [-halfWidth, halfWidth], [-halfHeight, halfHeight]
            float mapX = (normalizedX - 0.5f) * mapRectSize.x;
            float mapY = (normalizedY - 0.5f) * mapRectSize.y;

            return new Vector2(mapX, mapY);
        }
    }
}
