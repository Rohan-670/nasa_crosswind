using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Outpost.UI.Core;

namespace Outpost.UI.HUD
{
    /// <summary>
    /// Renders world-space objective markers and projects them to screen edges when off-camera.
    /// Informs young players of current objectives and distances.
    /// </summary>
    public class ObjectiveMarkerUI : MonoBehaviour
    {
        [System.Serializable]
        public class ActiveMarker
        {
            public string Id;
            public Vector3 WorldPosition;
            public string Label;
            public RectTransform MarkerInstance;
            public TextMeshProUGUI DistanceText;
            public TextMeshProUGUI LabelText;
        }

        [Header("Marker Prefab & Container")]
        [SerializeField] private GameObject _markerPrefab;
        [SerializeField] private RectTransform _markersContainer;
        [SerializeField] private Camera _mainCamera;

        [Header("Screen Clamping")]
        [SerializeField] private float _edgePadding = 50f;

        private readonly Dictionary<string, ActiveMarker> _markers = new Dictionary<string, ActiveMarker>();

        private void Awake()
        {
            if (_mainCamera == null) _mainCamera = Camera.main;
        }

        private void OnEnable()
        {
            UIEventBus.OnWaypointPinned += AddOrUpdateWaypoint;
            UIEventBus.OnWaypointCleared += RemoveWaypoint;
        }

        private void OnDisable()
        {
            UIEventBus.OnWaypointPinned -= AddOrUpdateWaypoint;
            UIEventBus.OnWaypointCleared -= RemoveWaypoint;
        }

        public void AddOrUpdateWaypoint(string id, Vector3 worldPos, string label)
        {
            if (_markers.TryGetValue(id, out var marker))
            {
                marker.WorldPosition = worldPos;
                marker.Label = label;
                if (marker.LabelText != null) marker.LabelText.text = label;
                return;
            }

            if (_markerPrefab == null || _markersContainer == null) return;

            GameObject go = Instantiate(_markerPrefab, _markersContainer);
            RectTransform rt = go.GetComponent<RectTransform>();
            TextMeshProUGUI distTxt = go.transform.Find("DistanceText")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI lblTxt = go.transform.Find("LabelText")?.GetComponent<TextMeshProUGUI>();

            if (lblTxt != null) lblTxt.text = label;

            var newMarker = new ActiveMarker
            {
                Id = id,
                WorldPosition = worldPos,
                Label = label,
                MarkerInstance = rt,
                DistanceText = distTxt,
                LabelText = lblTxt
            };

            _markers[id] = newMarker;
        }

        public void RemoveWaypoint(string id)
        {
            if (_markers.TryGetValue(id, out var marker))
            {
                if (marker.MarkerInstance != null)
                {
                    Destroy(marker.MarkerInstance.gameObject);
                }
                _markers.Remove(id);
            }
        }

        private void LateUpdate()
        {
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
                if (_mainCamera == null) return;
            }

            Vector3 camPos = _mainCamera.transform.position;

            foreach (var kvp in _markers)
            {
                var marker = kvp.Value;
                if (marker.MarkerInstance == null) continue;

                float distance = Vector3.Distance(camPos, marker.WorldPosition);
                if (marker.DistanceText != null)
                {
                    marker.DistanceText.text = $"{Mathf.RoundToInt(distance)}m";
                }

                // Screen projection
                Vector3 screenPos = _mainCamera.WorldToScreenPoint(marker.WorldPosition);
                bool isBehind = screenPos.z < 0;

                if (isBehind)
                {
                    screenPos *= -1f;
                }

                // Clamp to screen bounds
                screenPos.x = Mathf.Clamp(screenPos.x, _edgePadding, Screen.width - _edgePadding);
                screenPos.y = Mathf.Clamp(screenPos.y, _edgePadding, Screen.height - _edgePadding);

                marker.MarkerInstance.position = new Vector3(screenPos.x, screenPos.y, 0f);
            }
        }
    }
}
