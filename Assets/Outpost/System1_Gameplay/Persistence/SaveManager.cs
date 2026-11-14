using System;
using UnityEngine;
using Outpost.Core.Data;
using Outpost.Core.Events;

namespace Outpost.System1.Gameplay.Persistence
{
    [Serializable]
    public class OutpostSaveModel
    {
        public int ChapterId;
        public int RepairChoice;
        public float PlayerPosX;
        public float PlayerPosY;
        public float PlayerPosZ;
        public float OxygenLevel;
        public float BatteryLevel;
        public string SaveTimestamp;

        // Comprehensive Resource State Snapshot
        public ResourceStateData ResourceState;
    }

    public class SaveManager : MonoBehaviour
    {
        private const string SaveKey = "Outpost_Mission1_SaveData";

        public static SaveManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void OnEnable()
        {
            OutpostEventChannel.ChapterChanged += HandleAutoSaveOnChapter;
        }

        private void OnDisable()
        {
            OutpostEventChannel.ChapterChanged -= HandleAutoSaveOnChapter;
        }

        private void HandleAutoSaveOnChapter(MissionChapterId id, string title, string obj)
        {
            SaveGameState(id, RepairChoice.None, Vector3.zero);
        }

        public void SaveGameState(MissionChapterId chapter, RepairChoice choice, Vector3 playerPos)
        {
            ResourceStateData rState = Resources.ResourceManager.Instance != null
                ? Resources.ResourceManager.Instance.ExportState()
                : new ResourceStateData();

            OutpostSaveModel model = new OutpostSaveModel
            {
                ChapterId = (int)chapter,
                RepairChoice = (int)choice,
                PlayerPosX = playerPos.x,
                PlayerPosY = playerPos.y,
                PlayerPosZ = playerPos.z,
                OxygenLevel = rState.OxygenPercent,
                BatteryLevel = rState.BatteryPercent,
                SaveTimestamp = DateTime.UtcNow.ToString("o"),
                ResourceState = rState
            };

            string json = JsonUtility.ToJson(model);
            PlayerPrefs.SetString(SaveKey, json);
            PlayerPrefs.Save();
        }

        public OutpostSaveModel LoadGameState()
        {
            if (!PlayerPrefs.HasKey(SaveKey)) return null;

            try
            {
                string json = PlayerPrefs.GetString(SaveKey);
                OutpostSaveModel model = JsonUtility.FromJson<OutpostSaveModel>(json);

                if (model != null && model.ResourceState != null && Resources.ResourceManager.Instance != null)
                {
                    Resources.ResourceManager.Instance.ImportState(model.ResourceState);
                }

                return model;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[SaveManager] Failed to load game state: {ex.Message}");
                return null;
            }
        }

        public void DeleteSave()
        {
            PlayerPrefs.DeleteKey(SaveKey);
            PlayerPrefs.Save();
        }
    }
}
