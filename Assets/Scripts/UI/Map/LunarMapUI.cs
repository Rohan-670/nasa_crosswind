using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Outpost.UI.Core;

namespace Outpost.UI.Map
{
    /// <summary>
    /// Master controller for the interactive Lunar Geo Map.
    /// Provides zoom, pan, location dossier selection, real-time player orientation tracking, and waypoint dispatch.
    /// </summary>
    public class LunarMapUI : UIView, ILunarMapUI
    {
        [Header("Pan & Zoom")]
        [SerializeField] private MapPanZoomController _panZoomController;
        [SerializeField] private RectTransform _mapContentContainer;

        [Header("Player Tracking")]
        [SerializeField] private RectTransform _playerMapMarker;
        [SerializeField] private Transform _playerWorldTransform;

        [Header("Landmark Database")]
        [SerializeField] private List<MapLocationData> _registeredLocations = new List<MapLocationData>();
        [SerializeField] private GameObject _markerPrefab;

        [Header("Location Info Drawer")]
        [SerializeField] private GameObject _infoDrawer;
        [SerializeField] private TextMeshProUGUI _drawerTitleText;
        [SerializeField] private TextMeshProUGUI _drawerSectorText;
        [SerializeField] private TextMeshProUGUI _drawerDescriptionText;
        [SerializeField] private TextMeshProUGUI _drawerScienceFactText;
        [SerializeField] private Button _setWaypointButton;
        [SerializeField] private Button _closeDrawerButton;

        [Header("Navigation Buttons")]
        [SerializeField] private Button _returnToPlayerButton;
        [SerializeField] private Button _zoomInButton;
        [SerializeField] private Button _zoomOutButton;
        [SerializeField] private Button _closeMapButton;

        [Header("Scale Mapping (Meters to UI Pixels)")]
        [SerializeField] private float _pixelsPerMeter = 1.2f;

        private readonly Dictionary<string, MapMarkerUI> _markerInstances = new Dictionary<string, MapMarkerUI>();
        private MapLocationData _selectedLocation;

        protected override void Awake()
        {
            base.Awake();
            InitializeButtons();
            SpawnLandmarkMarkers();
            if (_infoDrawer != null) _infoDrawer.SetActive(false);
        }

        private void InitializeButtons()
        {
            if (_returnToPlayerButton != null) _returnToPlayerButton.onClick.AddListener(FocusOnPlayer);
            if (_zoomInButton != null) _zoomInButton.onClick.AddListener(() => _panZoomController?.Zoom(0.25f));
            if (_zoomOutButton != null) _zoomOutButton.onClick.AddListener(() => _panZoomController?.Zoom(-0.25f));
            if (_closeMapButton != null) _closeMapButton.onClick.AddListener(CloseMap);
            if (_closeDrawerButton != null) _closeDrawerButton.onClick.AddListener(() => _infoDrawer?.SetActive(false));
            if (_setWaypointButton != null) _setWaypointButton.onClick.AddListener(SetWaypointOnSelected);
        }

        private void SpawnLandmarkMarkers()
        {
            if (_markerPrefab == null || _mapContentContainer == null) return;

            foreach (var loc in _registeredLocations)
            {
                if (loc == null) continue;

                GameObject go = Instantiate(_markerPrefab, _mapContentContainer);
                RectTransform rt = go.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(loc.WorldPosition.x * _pixelsPerMeter, loc.WorldPosition.z * _pixelsPerMeter);

                MapMarkerUI markerUI = go.GetComponent<MapMarkerUI>();
                if (markerUI != null)
                {
                    markerUI.Initialize(loc, OnLocationMarkerSelected);
                    _markerInstances[loc.LocationId] = markerUI;
                }
            }
        }

        private void Update()
        {
            if (!IsVisible) return;
            UpdatePlayerMarker();
        }

        private void UpdatePlayerMarker()
        {
            if (_playerWorldTransform == null)
            {
                // Attempt to find player if not assigned
                var player = GameObject.FindWithTag("Player");
                if (player != null) _playerWorldTransform = player.transform;
                if (_playerWorldTransform == null) return;
            }

            Vector3 worldPos = _playerWorldTransform.position;
            Vector2 mapPos = new Vector2(worldPos.x * _pixelsPerMeter, worldPos.z * _pixelsPerMeter);

            if (_playerMapMarker != null)
            {
                _playerMapMarker.anchoredPosition = mapPos;
                // Rotate to match player compass heading
                _playerMapMarker.localRotation = Quaternion.Euler(0f, 0f, -_playerWorldTransform.eulerAngles.y);
            }
        }

        public void OpenMap()
        {
            Show();
            UIEventBus.PublishUIStateChanged(UIState.InteractiveMap);
            FocusOnPlayer();
        }

        public void CloseMap()
        {
            Hide();
            UIEventBus.PublishUIStateChanged(UIState.GameplayHUD);
        }

        public void FocusOnPlayer()
        {
            if (_playerWorldTransform != null && _panZoomController != null)
            {
                Vector2 playerMapPos = new Vector2(
                    _playerWorldTransform.position.x * _pixelsPerMeter,
                    _playerWorldTransform.position.z * _pixelsPerMeter
                );
                _panZoomController.FocusOn(playerMapPos);
            }
            else if (_panZoomController != null)
            {
                _panZoomController.ResetToCenter();
            }
        }

        public void FocusOnLocation(string locationId)
        {
            if (_markerInstances.TryGetValue(locationId, out var marker))
            {
                Vector2 locPos = new Vector2(
                    marker.LocationData.WorldPosition.x * _pixelsPerMeter,
                    marker.LocationData.WorldPosition.z * _pixelsPerMeter
                );
                _panZoomController?.FocusOn(locPos);
                OnLocationMarkerSelected(marker.LocationData);
            }
        }

        public void SetWaypoint(string locationId)
        {
            if (_markerInstances.TryGetValue(locationId, out var marker))
            {
                UIEventBus.PublishWaypoint(marker.LocationData.LocationId, marker.LocationData.WorldPosition, marker.LocationData.LocationName);
                marker.SetObjectiveHighlight(true);
            }
        }

        public void MarkLocationDiscovered(string locationId)
        {
            if (_markerInstances.TryGetValue(locationId, out var marker))
            {
                marker.gameObject.SetActive(true);
            }
        }

        private void OnLocationMarkerSelected(MapLocationData data)
        {
            _selectedLocation = data;
            if (_infoDrawer != null)
            {
                _infoDrawer.SetActive(true);
                if (_drawerTitleText != null) _drawerTitleText.text = data.LocationName;
                if (_drawerSectorText != null) _drawerSectorText.text = $"Sector Grid: {data.GridSector}";
                if (_drawerDescriptionText != null) _drawerDescriptionText.text = data.FacilityDescription;
                if (_drawerScienceFactText != null) _drawerScienceFactText.text = $"Lunar Science: {data.EducationalScienceFact}";
            }
        }

        private void SetWaypointOnSelected()
        {
            if (_selectedLocation != null)
            {
                SetWaypoint(_selectedLocation.LocationId);
                UIEventBus.PublishNotification("Navigation", $"Waypoint pinned: {_selectedLocation.LocationName}", NotificationType.ObjectiveUpdate);
            }
        }
    }
}
