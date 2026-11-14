using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Outpost.UI.Map
{
    /// <summary>
    /// Interactive pin marker representing an outpost sector on the Lunar Geo Map.
    /// </summary>
    public class MapMarkerUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Button _markerButton;
        [SerializeField] private Image _iconImage;
        [SerializeField] private TextMeshProUGUI _labelNameText;
        [SerializeField] private GameObject _objectiveRing;

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
        }

        public void SetObjectiveHighlight(bool active)
        {
            if (_objectiveRing != null) _objectiveRing.SetActive(active);
        }
    }
}
