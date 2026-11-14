using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Outpost.Core.Data;
using Outpost.Core.Interfaces;
using Outpost.Core.Events;

namespace Outpost.System3.Interface.GeoMap
{
    public class LunarGeoMapUI : MonoBehaviour
    {
        [Header("Map Container & Navigation")]
        [SerializeField] private GameObject mapRoot;
        [SerializeField] private RectTransform mapContentRect;
        [SerializeField] private RectTransform playerMarkerRect;
        [SerializeField] private RectTransform targetMarkerRect;

        [Header("Map Controls")]
        [SerializeField] private float zoomSpeed = 0.2f;
        [SerializeField] private float minZoom = 0.6f;
        [SerializeField] private float maxZoom = 2.5f;

        [Header("Selection & Info Panel")]
        [SerializeField] private Text locationTitleText;
        [SerializeField] private Text locationDescText;
        [SerializeField] private Text locationCoordsText;
        [SerializeField] private Text locationStatusText;

        [Header("Legend & Buttons")]
        [SerializeField] private Button returnToPlayerButton;
        [SerializeField] private Button closeMapButton;

        private IGeoMapService geoMapService;
        private float currentZoom = 1.0f;
        private Vector2 currentPan = Vector2.zero;
        private bool isMapVisible = false;

        private void Awake()
        {
            if (returnToPlayerButton != null) returnToPlayerButton.onClick.AddListener(CenterOnPlayer);
            if (closeMapButton != null) closeMapButton.onClick.AddListener(CloseMap);
        }

        private void Start()
        {
            geoMapService = LunarStationLayoutLocator();
            if (mapRoot != null) mapRoot.SetActive(false);
        }

        private IGeoMapService LunarStationLayoutLocator()
        {
            MonoBehaviour[] monos = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
            foreach (var m in monos)
            {
                if (m is IGeoMapService service) return service;
            }
            return null;
        }

        private void OnEnable()
        {
            OutpostEventChannel.GeoMapToggled += SetMapVisible;
            OutpostEventChannel.TargetWaypointSet += HandleWaypointSet;
        }

        private void OnDisable()
        {
            OutpostEventChannel.GeoMapToggled -= SetMapVisible;
            OutpostEventChannel.TargetWaypointSet -= HandleWaypointSet;
        }

        public void SetMapVisible(bool visible)
        {
            isMapVisible = visible;
            if (mapRoot != null) mapRoot.SetActive(visible);

            if (visible)
            {
                CenterOnPlayer();
                UpdateLocationDetails(geoMapService?.ActiveTargetId ?? StationLocationId.MainHabitat);
            }
        }

        public void CloseMap()
        {
            SetMapVisible(false);
            OutpostEventChannel.RaiseGeoMapToggled(false);
        }

        private void Update()
        {
            if (!isMapVisible) return;

            UpdatePlayerMarker();
            UpdateTargetMarker();
            HandlePanAndZoomInput();
        }

        private void UpdatePlayerMarker()
        {
            if (geoMapService == null || playerMarkerRect == null || mapContentRect == null) return;

            Vector2 mapPos = geoMapService.WorldToMapCoordinate(geoMapService.PlayerWorldPosition, mapContentRect.sizeDelta);
            playerMarkerRect.anchoredPosition = mapPos;
        }

        private void UpdateTargetMarker()
        {
            if (geoMapService == null || targetMarkerRect == null || mapContentRect == null) return;

            StationLocationData targetLoc = geoMapService.GetLocation(geoMapService.ActiveTargetId);
            if (targetLoc != null)
            {
                targetMarkerRect.gameObject.SetActive(true);
                Vector2 mapPos = geoMapService.WorldToMapCoordinate(targetLoc.WorldPosition, mapContentRect.sizeDelta);
                targetMarkerRect.anchoredPosition = mapPos;
            }
            else
            {
                targetMarkerRect.gameObject.SetActive(false);
            }
        }

        private void HandlePanAndZoomInput()
        {
            // Zoom with mouse scroll
            float scroll = Input.mouseScrollDelta.y;
            if (Mathf.Abs(scroll) > 0.01f)
            {
                currentZoom = Mathf.Clamp(currentZoom + scroll * zoomSpeed, minZoom, maxZoom);
                if (mapContentRect != null) mapContentRect.localScale = Vector3.one * currentZoom;
            }

            // Keyboard pan [I, J, K, L] or Arrow keys
            float panX = 0f;
            float panY = 0f;
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.J)) panX += 200f * Time.deltaTime;
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.L)) panX -= 200f * Time.deltaTime;
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.I)) panY -= 200f * Time.deltaTime;
            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.K)) panY += 200f * Time.deltaTime;

            if (mapContentRect != null && (panX != 0f || panY != 0f))
            {
                currentPan += new Vector2(panX, panY);
                mapContentRect.anchoredPosition = currentPan;
            }
        }

        public void CenterOnPlayer()
        {
            if (geoMapService == null || mapContentRect == null) return;

            Vector2 playerMapPos = geoMapService.WorldToMapCoordinate(geoMapService.PlayerWorldPosition, mapContentRect.sizeDelta);
            currentPan = -playerMapPos * currentZoom;
            mapContentRect.anchoredPosition = currentPan;
            currentZoom = 1.0f;
            mapContentRect.localScale = Vector3.one;
        }

        public void UpdateLocationDetails(StationLocationId id)
        {
            if (geoMapService == null) return;

            StationLocationData loc = geoMapService.GetLocation(id);
            if (loc == null) return;

            if (locationTitleText != null) locationTitleText.text = $"{loc.DisplayName} [{loc.ShortCode}]";
            if (locationDescText != null) locationDescText.text = loc.Description;
            if (locationCoordsText != null) locationCoordsText.text = $"Coordinates: X={loc.WorldPosition.x:F0}m, Y={loc.WorldPosition.y:F0}m, Z={loc.WorldPosition.z:F0}m";
            
            if (locationStatusText != null)
            {
                if (loc.IsEmergency) locationStatusText.text = "STATUS: CRITICAL ANOMALY / BREACH";
                else if (loc.IsDiscovered) locationStatusText.text = "STATUS: VISITED & ONLINE";
                else locationStatusText.text = "STATUS: UNVISITED - MISSION OBJECTIVE";
            }
        }

        private void HandleWaypointSet(StationLocationId id)
        {
            UpdateLocationDetails(id);
        }
    }
}
