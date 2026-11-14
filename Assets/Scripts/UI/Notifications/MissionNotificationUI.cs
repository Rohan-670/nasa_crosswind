using System;
using System.Collections.Generic;
using UnityEngine;
using Outpost.UI.Core;

namespace Outpost.UI.Notifications
{
    /// <summary>
    /// Notification dispatcher utilizing an object pool to maintain smooth WebGL performance.
    /// Listens to UIEventBus for mission milestones, emergencies, and radio chatter.
    /// </summary>
    public class MissionNotificationUI : MonoBehaviour, IMissionNotificationUI
    {
        [Header("Toast Pool")]
        [SerializeField] private GameObject _toastPrefab;
        [SerializeField] private RectTransform _toastContainer;
        [SerializeField] private int _poolSize = 6;

        private readonly Queue<NotificationToast> _availablePool = new Queue<NotificationToast>();

        private void Awake()
        {
            InitializePool();
        }

        private void OnEnable()
        {
            UIEventBus.OnNotificationRequested += (title, msg, type) => DisplayNotification(title, msg, type);
        }

        private void OnDisable()
        {
            UIEventBus.OnNotificationRequested -= (title, msg, type) => DisplayNotification(title, msg, type);
        }

        private void InitializePool()
        {
            if (_toastPrefab == null || _toastContainer == null) return;

            for (int i = 0; i < _poolSize; i++)
            {
                GameObject go = Instantiate(_toastPrefab, _toastContainer);
                go.SetActive(false);
                var toast = go.GetComponent<NotificationToast>();
                if (toast != null) _availablePool.Enqueue(toast);
            }
        }

        public void DisplayNotification(string title, string message, NotificationType type, float duration = 4.0f)
        {
            NotificationToast toast = null;

            if (_availablePool.Count > 0)
            {
                toast = _availablePool.Dequeue();
            }
            else if (_toastPrefab != null && _toastContainer != null)
            {
                // Fallback instantiation if pool exhausted
                GameObject go = Instantiate(_toastPrefab, _toastContainer);
                toast = go.GetComponent<NotificationToast>();
            }

            if (toast != null)
            {
                toast.gameObject.SetActive(true);
                toast.transform.SetAsLastSibling();
                toast.Initialize(title, message, type, duration, RecycleToast);
            }
        }

        public void DisplayEmergencyAlert(string alertMessage)
        {
            DisplayNotification("EMERGENCY ALERT", alertMessage, NotificationType.EmergencyAlert, 6.0f);
        }

        private void RecycleToast(NotificationToast toast)
        {
            if (toast != null)
            {
                _availablePool.Enqueue(toast);
            }
        }
    }
}
