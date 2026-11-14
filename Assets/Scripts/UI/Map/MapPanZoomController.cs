using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Outpost.UI.Map
{
    /// <summary>
    /// Handles smooth pan and zoom interactions for the Lunar Geo Map.
    /// Supports mouse drag, keyboard WASD, and scroll wheel zooming with boundary clamping.
    /// </summary>
    public class MapPanZoomController : MonoBehaviour, IDragHandler, IScrollHandler
    {
        [Header("Zoom Constraints")]
        [SerializeField] private float _minZoom = 0.6f;
        [SerializeField] private float _maxZoom = 2.5f;
        [SerializeField] private float _zoomSpeed = 0.15f;

        [Header("Pan Constraints")]
        [SerializeField] private float _panSpeed = 400f;
        [SerializeField] private Vector2 _panLimitMin = new Vector2(-400f, -400f);
        [SerializeField] private Vector2 _panLimitMax = new Vector2(400f, 400f);

        [Header("Target Container")]
        [SerializeField] private RectTransform _contentTransform;

        private float _currentZoom = 1.0f;
        private Vector2 _currentPan = Vector2.zero;

        public float CurrentZoom => _currentZoom;
        public Vector2 CurrentPan => _currentPan;

        private void Update()
        {
            HandleKeyboardPan();
        }

        private void HandleKeyboardPan()
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            if (Mathf.Abs(h) > 0.01f || Mathf.Abs(v) > 0.01f)
            {
                Vector2 move = new Vector2(-h, -v) * (_panSpeed * Time.unscaledDeltaTime);
                Pan(move);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            Pan(eventData.delta);
        }

        public void OnScroll(PointerEventData eventData)
        {
            float zoomDelta = eventData.scrollDelta.y * _zoomSpeed;
            Zoom(zoomDelta);
        }

        public void Pan(Vector2 delta)
        {
            _currentPan += delta;
            _currentPan.x = Mathf.Clamp(_currentPan.x, _panLimitMin.x * _currentZoom, _panLimitMax.x * _currentZoom);
            _currentPan.y = Mathf.Clamp(_currentPan.y, _panLimitMin.y * _currentZoom, _panLimitMax.y * _currentZoom);
            ApplyTransform();
        }

        public void Zoom(float delta)
        {
            _currentZoom = Mathf.Clamp(_currentZoom + delta, _minZoom, _maxZoom);
            ApplyTransform();
        }

        public void ResetToCenter()
        {
            _currentPan = Vector2.zero;
            _currentZoom = 1.0f;
            ApplyTransform();
        }

        public void FocusOn(Vector2 localMapPos)
        {
            _currentPan = -localMapPos * _currentZoom;
            ApplyTransform();
        }

        private void ApplyTransform()
        {
            if (_contentTransform != null)
            {
                _contentTransform.anchoredPosition = _currentPan;
                _contentTransform.localScale = new Vector3(_currentZoom, _currentZoom, 1f);
            }
        }
    }
}
