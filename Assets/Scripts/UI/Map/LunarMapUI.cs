using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Outpost.UI.Core;

namespace Outpost.UI.Map
{
    /// <summary>
    /// Master controller for the Phase 2 Interactive Lunar Geo Map.
    /// Fully integrates pan, zoom, location selection dossiers, route rendering, crew/rover entity tracking,
    /// and mission status banners according to actual Lunar South Pole scene coordinates.
    /// </summary>
    public class LunarMapUI : UIView, ILunarMapUI
    {
        [Header("Mission Status Banner")]
        [SerializeField] private TextMeshProUGUI _missionStatusText;
        [SerializeField] private TextMeshProUGUI _activeObjectiveBannerText;

        [Header("Pan & Zoom Controls")]
        [SerializeField] private MapPanZoomController _panZoomController;
        [SerializeField] private RectTransform _mapContentContainer;

        [Header("Route & Entity Sub-Systems")]
        [SerializeField] private MapRouteRenderer _routeRenderer;
        [SerializeField] private MapEntityTracker _entityTracker;

        [Header("Player Tracking")]
        [SerializeField] private RectTransform _playerMapMarker;
        [SerializeField] private Transform _playerWorldTransform;

        [Header("Landmark Database (10 Core Sectors)")]
        [SerializeField] private List<MapLocationData> _registeredLocations = new List<MapLocationData>();
        [SerializeField] private GameObject _markerPrefab;

        [Header("Location Selection Dossier Drawer")]
        [SerializeField] private GameObject _infoDrawer;
        [SerializeField] private TextMeshProUGUI _drawerTitleText;
        [SerializeField] private TextMeshProUGUI _drawerSectorText;
        [SerializeField] private TextMeshProUGUI _drawerPurposeText;
        [SerializeField] private TextMeshProUGUI _drawerAvailabilityText;
        [SerializeField] private TextMeshProUGUI _drawerCurrentObjectiveText;
        [SerializeField] private TextMeshProUGUI _drawerNavigationInfoText;
        [SerializeField] private TextMeshProUGUI _drawerScienceFactText;
        [SerializeField] private Button _setWaypointButton;
        [SerializeField] private Button _closeDrawerButton;

        [Header("Interactive Legend Toggles")]
        [SerializeField] private Toggle _toggleFacilities;
        [SerializeField] private Toggle _toggleCrewAndRover;
        [SerializeField] private Toggle _toggleHazards;

        [Header("Navigation Buttons")]
        [SerializeField] private Button _returnToPlayerButton;
        [SerializeField] private Button _zoomInButton;
        [SerializeField] private Button _zoomOutButton;
        [SerializeField] private Button _closeMapButton;

        [Header("Scale Mapping (Meters to UI Pixels)")]
        [SerializeField] private float _pixelsPerMeter = 1.2f;

        private readonly Dictionary<string, MapMarkerUI> _markerInstances = new Dictionary<string, MapMarkerUI>();
        private MapLocationData _selectedLocation;
        private Vector3 _activeWaypointWorldPos = Vector3.zero;

        protected override void Awake()
        {
            base.Awake();
            InitializeButtons();
            InitializeLegendToggles();
            SpawnLandmarkMarkers();
            if (_infoDrawer != null) _infoDrawer.SetActive(false);
            if (_missionStatusText != null && string.IsNullOrEmpty(_missionStatusText.text))
            {
                _missionStatusText.text = "MISSION 1: THE FIRST FRONTIER — OUTPOST STATUS: OPERATIONAL";
            }
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

        private void InitializeLegendToggles()
        {
            if (_toggleFacilities != null)
                _toggleFacilities.onValueChanged.AddListener(show => FilterLocations(OutpostFacilityType.Habitat, show));
            if (_toggleCrewAndRover != null)
                _toggleCrewAndRover.onValueChanged.AddListener(show => {
                    if (_entityTracker != null) _entityTracker.gameObject.SetActive(show);
                });
            if (_toggleHazards != null)
                _toggleHazards.onValueChanged.AddListener(show => FilterLocations(OutpostFacilityType.EmergencyHazard, show));
        }

        private void SpawnLandmarkMarkers()
        {
            if (_markerPrefab == null || _mapContentContainer == null) return;

            // Ensure all 10 standard landmarks exist if not populated in inspector
            if (_registeredLocations.Count == 0)
            {
                PopulateDefaultLocations();
            }

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

        private void PopulateDefaultLocations()
        {
            // Populate runtime definitions for all 10 locations matching LunarStationLayout
            AddLocationRuntime("LOC_HABITAT", "Main Habitat", OutpostFacilityType.Habitat, "E-5", LunarStationLayout.MainHabitat,
                "Central pressurized habitat, life support, and living quarters.", "Verify habitat airlock seals.", "Direct access via south airlock ramp.");

            AddLocationRuntime("LOC_LANDING", "Landing Zone", OutpostFacilityType.LandingSite, "B-2", LunarStationLayout.LandingZone,
                "Reinforced touchdown pad with protective blast berms.", "Initial touchdown confirmed.", "Follow the marked transport trail west-southwest.");

            AddLocationRuntime("LOC_SOLAR", "Solar Farm", OutpostFacilityType.Power, "H-8", LunarStationLayout.SolarFarm,
                "Photovoltaic array on Shackleton Crater rim ridge.", "Inspect power distribution line.", "Climb ridge trail northeast. Watch for steep loose regolith.");

            AddLocationRuntime("LOC_GREENHOUSE", "Greenhouse Biodome", OutpostFacilityType.LifeSupport, "D-7", LunarStationLayout.Greenhouse,
                "Agricultural module growing fresh crops in sealed hydroponics.", "Check moisture sensors.", "Direct pressurized tunnel connected north of habitat.");

            AddLocationRuntime("LOC_LAB", "Research Laboratory", OutpostFacilityType.Science, "F-4", LunarStationLayout.ResearchLab,
                "Sample analysis cleanrooms, mass spectrometers, and rock saws.", "Analyze regolith drill core.", "East science wing.");

            AddLocationRuntime("LOC_COMM", "Communication Tower", OutpostFacilityType.Communications, "I-3", LunarStationLayout.CommTower,
                "High-gain microwave array maintaining real-time telemetry to Earth.", "Align deep space antenna.", "Elevated ridge east of science lab.");

            AddLocationRuntime("LOC_ROVER", "Rover Maintenance Bay", OutpostFacilityType.Vehicles, "C-4", LunarStationLayout.RoverStation,
                "Garage, tool gantry, and inductive charging dock for NASA Rover SER-01.", "Inspect rover tire springs.", "Follow the west road 75 meters.");

            AddLocationRuntime("LOC_STORAGE", "Equipment Storage", OutpostFacilityType.Habitat, "D-3", LunarStationLayout.EquipmentStorage,
                "Tool containers, spare batteries, and EVA suit charging racks.", "Retrieve backup air filter parts.", "South depot adjacent to airlock.");

            AddLocationRuntime("LOC_GEOLOGY", "Geological Ice Crater", OutpostFacilityType.Science, "A-10", LunarStationLayout.GeologyArea,
                "Deep permanently shadowed crater holding billion-year-old water ice.", "Survey ice deposits.", "Northwest expedition route. Cold suit heaters required.");

            AddLocationRuntime("LOC_EMERGENCY_1", "Micrometeoroid Impact Site", OutpostFacilityType.EmergencyHazard, "G-7", LunarStationLayout.EmergencyImpactSite,
                "Severed high-voltage power conduit at Substation 2.", "Emergency electrical repair.", "Danger: High-voltage arcing. Use insulated tools.");
        }

        private void AddLocationRuntime(string id, string name, OutpostFacilityType type, string sector, Vector3 worldPos, string purpose, string obj, string nav)
        {
            var data = ScriptableObject.CreateInstance<MapLocationData>();
            data.LocationId = id;
            data.LocationName = name;
            data.FacilityType = type;
            data.GridSector = sector;
            data.WorldPosition = worldPos;
            data.FacilityPurpose = purpose;
            data.CurrentObjectiveHere = obj;
            data.NavigationAdvice = nav;
            data.EducationalScienceFact = "Lunar outpost positioning utilizes crater rim ridges for continuous solar power.";
            data.Availability = FacilityAvailability.Operational;
            data.IsVisited = id == "LOC_HABITAT" || id == "LOC_LANDING";
            _registeredLocations.Add(data);
        }

        private void Update()
        {
            if (!IsVisible) return;

            UpdatePlayerMarker();
            if (_entityTracker != null) _entityTracker.UpdateEntityMarkers();
            UpdateRouteLine();
        }

        private void UpdatePlayerMarker()
        {
            if (_playerWorldTransform == null)
            {
                var player = GameObject.FindWithTag("Player");
                if (player != null) _playerWorldTransform = player.transform;
                if (_playerWorldTransform == null) return;
            }

            Vector3 worldPos = _playerWorldTransform.position;
            Vector2 mapPos = new Vector2(worldPos.x * _pixelsPerMeter, worldPos.z * _pixelsPerMeter);

            if (_playerMapMarker != null)
            {
                _playerMapMarker.anchoredPosition = mapPos;
                _playerMapMarker.localRotation = Quaternion.Euler(0f, 0f, -_playerWorldTransform.eulerAngles.y);
            }

            // Update visited status for nearby landmarks (within 35m)
            foreach (var loc in _registeredLocations)
            {
                if (!loc.IsVisited && Vector3.Distance(worldPos, loc.WorldPosition) < 35f)
                {
                    loc.IsVisited = true;
                    if (_markerInstances.TryGetValue(loc.LocationId, out var marker))
                    {
                        marker.SetVisited(true);
                    }
                    UIEventBus.PublishNotification("Exploration", $"Sector Discovered: {loc.LocationName}", NotificationType.Discovery);
                }
            }
        }

        private void UpdateRouteLine()
        {
            if (_routeRenderer == null || _playerMapMarker == null) return;

            if (_activeWaypointWorldPos != Vector3.zero)
            {
                Vector2 targetMapPos = new Vector2(_activeWaypointWorldPos.x * _pixelsPerMeter, _activeWaypointWorldPos.z * _pixelsPerMeter);
                _routeRenderer.SetRoute(_playerMapMarker.anchoredPosition, targetMapPos);
            }
            else
            {
                _routeRenderer.ClearRoute();
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
                _activeWaypointWorldPos = marker.LocationData.WorldPosition;
                UIEventBus.PublishWaypoint(marker.LocationData.LocationId, marker.LocationData.WorldPosition, marker.LocationData.LocationName);
                marker.SetObjectiveHighlight(true);
            }
        }

        public void MarkLocationDiscovered(string locationId)
        {
            if (_markerInstances.TryGetValue(locationId, out var marker))
            {
                marker.SetVisited(true);
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
                if (_drawerPurposeText != null) _drawerPurposeText.text = $"<b>Purpose:</b> {data.FacilityPurpose}";

                if (_drawerAvailabilityText != null)
                {
                    string availStr = data.Availability switch
                    {
                        FacilityAvailability.Caution => "CAUTION // STORM ALERT",
                        FacilityAvailability.EmergencyDamaged => "EMERGENCY // REPAIR NEEDED",
                        FacilityAvailability.Unexplored => "UNEXPLORED SECTOR",
                        _ => "OPERATIONAL // ACCESSIBLE"
                    };
                    _drawerAvailabilityText.text = $"<b>Status:</b> {availStr}";
                }

                if (_drawerCurrentObjectiveText != null)
                {
                    string obj = string.IsNullOrEmpty(data.CurrentObjectiveHere) ? "No active objective at this facility." : data.CurrentObjectiveHere;
                    _drawerCurrentObjectiveText.text = $"<b>Objective:</b> {obj}";
                }

                if (_drawerNavigationInfoText != null)
                {
                    float dist = _playerWorldTransform != null
                        ? Vector3.Distance(_playerWorldTransform.position, data.WorldPosition)
                        : Vector3.Distance(Vector3.zero, data.WorldPosition);
                    _drawerNavigationInfoText.text = $"<b>Navigation:</b> {Mathf.RoundToInt(dist)}m away. {data.NavigationAdvice}";
                }

                if (_drawerScienceFactText != null)
                {
                    _drawerScienceFactText.text = $"<b>Lunar Science:</b> {data.EducationalScienceFact}";
                }
            }
        }

        private void SetWaypointOnSelected()
        {
            if (_selectedLocation != null)
            {
                SetWaypoint(_selectedLocation.LocationId);
                UIEventBus.PublishNotification("Navigation", $"Waypoint Pinned: {_selectedLocation.LocationName}", NotificationType.ObjectiveUpdate);
            }
        }

        private void FilterLocations(OutpostFacilityType type, bool visible)
        {
            foreach (var kvp in _markerInstances)
            {
                if (kvp.Value.LocationData.FacilityType == type)
                {
                    kvp.Value.gameObject.SetActive(visible);
                }
            }
        }
    }
}
