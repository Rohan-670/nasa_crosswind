using System;
using UnityEngine;
using Outpost.Core.Data;

namespace Outpost.System1.Gameplay.Persistence
{
    [Serializable]
    public class OutpostSaveData
    {
        public int CurrentChapter;
        public int RepairChoice;
        public float PlayerPosX;
        public float PlayerPosY;
        public float PlayerPosZ;
        public string Timestamp;
    }

    public static class OutpostSaveSystem
    {
        private const string SaveKey = "Outpost_SurviveBeyondEarth_Save";

        public static void SaveProgress(MissionChapterId chapter, RepairChoice choice, Vector3 playerPos)
        {
            OutpostSaveData data = new OutpostSaveData
            {
                CurrentChapter = (int)chapter,
                RepairChoice = (int)choice,
                PlayerPosX = playerPos.x,
                PlayerPosY = playerPos.y,
                PlayerPosZ = playerPos.z,
                Timestamp = DateTime.UtcNow.ToString("o")
            };

            string json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(SaveKey, json);
            PlayerPrefs.Save();
        }

        public static OutpostSaveData LoadProgress()
        {
            if (!PlayerPrefs.HasKey(SaveKey)) return null;

            string json = PlayerPrefs.GetString(SaveKey);
            try
            {
                return JsonUtility.FromJson<OutpostSaveData>(json);
            }
            catch
            {
                return null;
            }
        }

        public static void ClearSave()
        {
            PlayerPrefs.DeleteKey(SaveKey);
            PlayerPrefs.Save();
        }
    }
}
