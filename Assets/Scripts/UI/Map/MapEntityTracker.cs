using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Outpost.UI.Map
{
    [System.Serializable]
    public class TrackedEntity
    {
        public string EntityId;
        public string EntityName;
        public Transform WorldTransform;
        public Color EntityColor = Color.white;
        public RectTransform MapMarkerInstance;
        public Image MarkerIcon;
        public TextMeshProUGUI MarkerLabel;
    }

    /// <summary>
    /// Tracks dynamic world objects (Astronaut crew members, NASA Rover) and projects them onto the Lunar Geo Map.
    /// Uses real scene transforms supplied by Computer 1 (Gameplay) and Computer 2 (Environment).
    /// </summary>
    public class MapEntityTracker : MonoBehaviour
    {
        [Header("Entity Marker Prefab & Container")]
        [SerializeField] private GameObject _entityMarkerPrefab;
        [SerializeField] private RectTransform _markersContainer;
        [SerializeField] private float _pixelsPerMeter = 1.2f;

        [Header("Tracked Outpost Entities")]
        [SerializeField] private List<TrackedEntity> _entities = new List<TrackedEntity>();

        private readonly Dictionary<string, TrackedEntity> _entityMap = new Dictionary<string, TrackedEntity>();

        private void Awake()
        {
            RegisterDefaultCrewIfEmpty();
            SpawnEntityMarkers();
        }

        private void RegisterDefaultCrewIfEmpty()
        {
            if (_entities.Count > 0) return;

            _entities.Add(new TrackedEntity
            {
                EntityId = "CREW_ELENA",
                EntityName = "Elena",
                EntityColor = new Color(0.12f, 0.34f, 0.63f, 1f) // Navy
            });

            _entities.Add(new TrackedEntity
            {
                EntityId = "CREW_MARCUS",
                EntityName = "Marcus",
                EntityColor = new Color(0.96f, 0.53f, 0.12f, 1f) // Amber
            });

            _entities.Add(new TrackedEntity
            {
                EntityId = "CREW_AISHA",
                EntityName = "Aisha",
                EntityColor = new Color(0f, 0.66f, 0.59f, 1f) // Teal
            });

            _entities.Add(new TrackedEntity
            {
                EntityId = "CREW_DANIEL",
                EntityName = "Daniel",
                EntityColor = new Color(0.18f, 0.55f, 0.34f, 1f) // Emerald
            });

            _entities.Add(new TrackedEntity
            {
                EntityId = "EQUIP_ROVER",
                EntityName = "Rover SER-01",
                EntityColor = new Color(0.85f, 0.88f, 0.92f, 1f) // Titanium White
            });
        }

        private void SpawnEntityMarkers()
        {
            if (_entityMarkerPrefab == null || _markersContainer == null) return;

            foreach (var entity in _entities)
            {
                GameObject go = Instantiate(_entityMarkerPrefab, _markersContainer);
                RectTransform rt = go.GetComponent<RectTransform>();
                Image img = go.GetComponentInChildren<Image>();
                TextMeshProUGUI lbl = go.GetComponentInChildren<TextMeshProUGUI>();

                if (img != null) img.color = entity.EntityColor;
                if (lbl != null)
                {
                    lbl.text = entity.EntityName;
                    lbl.color = entity.EntityColor;
                }

                entity.MapMarkerInstance = rt;
                entity.MarkerIcon = img;
                entity.MarkerLabel = lbl;

                _entityMap[entity.EntityId] = entity;
            }
        }

        public void RegisterEntityTransform(string entityId, Transform worldTransform)
        {
            if (_entityMap.TryGetValue(entityId, out var entity))
            {
                entity.WorldTransform = worldTransform;
            }
        }

        public void UpdateEntityMarkers()
        {
            foreach (var entity in _entities)
            {
                if (entity.MapMarkerInstance == null) continue;

                if (entity.WorldTransform != null)
                {
                    entity.MapMarkerInstance.gameObject.SetActive(true);
                    Vector3 wPos = entity.WorldTransform.position;
                    entity.MapMarkerInstance.anchoredPosition = new Vector2(wPos.x * _pixelsPerMeter, wPos.z * _pixelsPerMeter);
                }
                else
                {
                    // Fallback to stationary position near habitat if Computer 2 hasn't spawned physical model yet
                    Vector3 defaultPos = entity.EntityId switch
                    {
                        "CREW_ELENA" => new Vector3(5f, 0f, 5f),
                        "CREW_MARCUS" => new Vector3(8f, 0f, -5f),
                        "CREW_AISHA" => new Vector3(-8f, 0f, 6f),
                        "CREW_DANIEL" => new Vector3(-5f, 0f, -8f),
                        "EQUIP_ROVER" => new Vector3(-75f, 0f, -45f), // At Rover Station
                        _ => Vector3.zero
                    };
                    entity.MapMarkerInstance.gameObject.SetActive(true);
                    entity.MapMarkerInstance.anchoredPosition = new Vector2(defaultPos.x * _pixelsPerMeter, defaultPos.z * _pixelsPerMeter);
                }
            }
        }
    }
}
