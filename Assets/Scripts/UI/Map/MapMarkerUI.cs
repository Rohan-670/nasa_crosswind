using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Outpost.UI.Map
{
    /// <summary>
    /// Interactive pin marker representing an outpost sector on the Lunar Geo Map.
    /// Handles visual state styling (Nominal, Caution, Emergency, Visited) and selection.
    /// </summary>
    public class MapMarkerUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Button _markerButton;
        [SerializeField] private Image _iconImage;
        [SerializeField] private Image _pinBackgroundImage;
        [SerializeField] private TextMeshProUGUI _labelNameText;
        [SerializeField] private GameObject _objectiveRing;
        [SerializeField] private GameObject _visitedBadge;

        [Header("State Colors")]
        [SerializeField] private Color _operationalColor = new Color(0.18f, 0.8f, 0.44f, 1f);
        [SerializeField] private Color _cautionColor = new Color(0.96f, 0.72f, 0.15f, 1f);
        [SerializeField] private Color _emergencyColor = new Color(0.94f, 0.26f, 0.21f, 1f);
        [SerializeField] private Color _unexploredColor = new Color(0.55f, 0.6f, 0.65f, 0.8f);

        private MapLocationData _locationData;
        private Action<MapLocationData> _onSelectedCallback;

        public MapLocationData LocationData => _locationData;

        public void Initialize(MapLocationData data, Action<MapLocationData> onSelected)
        {
            _locationData = data;
            _onSelectedCallback = onSelected;

            if (_labelNameText != null) _labelNameText.text = data.LocationName;
            if (_markerButton != null)
            {
                _markerButton.onClick.RemoveAllListeners();
                _markerButton.onClick.AddListener(() => _onSelectedCallback?.Invoke(_locationData));
            }

            UpdateVisualState();
        }

        public void UpdateVisualState()
        {
            if (_locationData == null) return;

            Color stateColor = _locationData.Availability switch
            {
                FacilityAvailability.Caution => _cautionColor,
                FacilityAvailability.EmergencyDamaged => _emergencyColor,
                FacilityAvailability.Unexplored => _unexploredColor,
                _ => _operationalColor
            };

            if (_pinBackgroundImage != null) _pinBackgroundImage.color = stateColor;
            if (_labelNameText != null) _labelNameText.color = stateColor;

            bool hasObjective = !string.IsNullOrEmpty(_locationData.CurrentObjectiveHere);
            if (_objectiveRing != null) _objectiveRing.SetActive(hasObjective);

            if (_visitedBadge != null) _visitedBadge.SetActive(_locationData.IsVisited);
        }

        public void SetObjectiveHighlight(bool active)
        {
            if (_objectiveRing != null) _objectiveRing.SetActive(active);
        }

        public void SetVisited(bool visited)
        {
            if (_locationData != null)
            {
                _locationData.IsVisited = visited;
                UpdateVisualState();
            }
        }
    }
}
