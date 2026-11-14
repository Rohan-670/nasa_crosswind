using System;
using UnityEngine;
using UnityEngine.UI;

namespace Outpost.UI.Core
{
    /// <summary>
    /// Dynamic responsive adapter for browser-based Unity WebGL.
    /// Automatically manages CanvasScaler reference resolutions, safe anchors, and aspect ratio matching
    /// so that HUD, Lunar Map, and Subtitles scale crisply across 16:9, 16:10, 4:3, and ultrawide viewports.
    /// Prevents excessive UI rebuilds and requires zero per-frame garbage collection.
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasScaler))]
    public class ResponsiveUIAdapter : MonoBehaviour
    {
        [Header("Target WebGL Canvas Settings")]
        [SerializeField] private Vector2 _referenceResolution = new Vector2(1920f, 1080f);
        [SerializeField] private float _defaultMatchWidthOrHeight = 0.5f;

        private CanvasScaler _scaler;
        private int _lastScreenWidth = 0;
        private int _lastScreenHeight = 0;

        private void Awake()
        {
            _scaler = GetComponent<CanvasScaler>();
            ConfigureCanvasScaler();
            ApplyResponsiveMatching();
        }

        private void Start()
        {
            ApplyResponsiveMatching();
        }

        private void Update()
        {
            // Only update when the browser window or canvas actually resizes
            if (Screen.width != _lastScreenWidth || Screen.height != _lastScreenHeight)
            {
                ApplyResponsiveMatching();
            }
        }

        private void ConfigureCanvasScaler()
        {
            if (_scaler == null) return;

            _scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            _scaler.referenceResolution = _referenceResolution;
            _scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            _scaler.matchWidthOrHeight = _defaultMatchWidthOrHeight;
        }

        private void ApplyResponsiveMatching()
        {
            _lastScreenWidth = Screen.width;
            _lastScreenHeight = Screen.height;

            if (_scaler == null || _lastScreenHeight <= 0) return;

            float currentAspect = (float)_lastScreenWidth / _lastScreenHeight;
            float targetAspect = _referenceResolution.x / _referenceResolution.y; // ~1.777 (16:9)

            // Adjust match factor to prevent HUD clipping on narrow (4:3 / 16:10) or ultrawide (21:9) displays
            if (currentAspect < 1.6f)
            {
                // Narrow displays: prioritize width to prevent UI elements clipping horizontally
                _scaler.matchWidthOrHeight = 0.0f;
            }
            else if (currentAspect > 2.0f)
            {
                // Ultrawide displays: prioritize height to keep elements proportional
                _scaler.matchWidthOrHeight = 1.0f;
            }
            else
            {
                // Standard 16:9 widescreen: balanced scaling
                _scaler.matchWidthOrHeight = 0.5f;
            }
        }
    }
}
