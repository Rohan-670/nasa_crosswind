using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Outpost.UI.Map
{
    /// <summary>
    /// Renders dynamic navigational route lines and breadcrumbs between the player and the active waypoint.
    /// Operates on 2D uGUI canvas using pooled line segments or UI vertex quads to maintain 60 FPS in WebGL.
    /// </summary>
    public class MapRouteRenderer : MonoBehaviour
    {
        [Header("Route Visuals")]
        [SerializeField] private RectTransform _routeContainer;
        [SerializeField] private GameObject _dotPrefab;
        [SerializeField] private int _dotPoolSize = 25;
        [SerializeField] private float _dotSpacing = 30f;
        [SerializeField] private Color _routeColor = new Color(0f, 0.95f, 1f, 0.85f);

        private readonly List<RectTransform> _dots = new List<RectTransform>();
        private bool _isRouteActive = false;
        private Vector2 _startPoint;
        private Vector2 _endPoint;

        private void Awake()
        {
            InitializeDots();
            ClearRoute();
        }

        private void InitializeDots()
        {
            if (_dotPrefab == null || _routeContainer == null) return;

            for (int i = 0; i < _dotPoolSize; i++)
            {
                GameObject go = Instantiate(_dotPrefab, _routeContainer);
                go.SetActive(false);
                var img = go.GetComponent<Image>();
                if (img != null) img.color = _routeColor;
                _dots.Add(go.GetComponent<RectTransform>());
            }
        }

        public void SetRoute(Vector2 startMapPos, Vector2 endMapPos)
        {
            _startPoint = startMapPos;
            _endPoint = endMapPos;
            _isRouteActive = true;
            UpdateDots();
        }

        public void ClearRoute()
        {
            _isRouteActive = false;
            foreach (var dot in _dots)
            {
                if (dot != null) dot.gameObject.SetActive(false);
            }
        }

        public void UpdateStartPoint(Vector2 newStartPos)
        {
            if (_isRouteActive)
            {
                _startPoint = newStartPos;
                UpdateDots();
            }
        }

        private void UpdateDots()
        {
            if (!_isRouteActive || _dots.Count == 0) return;

            Vector2 diff = _endPoint - _startPoint;
            float totalDistance = diff.magnitude;

            if (totalDistance < 10f)
            {
                ClearRoute();
                return;
            }

            int count = Mathf.Min(_dots.Count, Mathf.Max(2, Mathf.FloorToInt(totalDistance / _dotSpacing)));
            Vector2 step = diff / (count - 1);

            for (int i = 0; i < _dots.Count; i++)
            {
                if (i < count)
                {
                    _dots[i].gameObject.SetActive(true);
                    _dots[i].anchoredPosition = _startPoint + step * i;
                }
                else
                {
                    _dots[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
