using System;
using UnityEngine;

namespace Lunaris.Core
{
    [Serializable]
    public class PlayerSaveData
    {
        public bool hasSavedGame = false;
        public int currentMissionIndex = 1;
        public string astronautRank = "Junior Astronaut";
        public float bestCorridorAccuracy = 0f;
        public int samplesCollectedTotal = 0;
        public float totalFlightTimeMinutes = 0f;
        public string lastSaveDate = "";
    }

    public static class SaveSystem
    {
        private const string SAVE_KEY = "LUNARIS_SAVE_DATA";

        public static void Save(PlayerSaveData data)
        {
            try
            {
                data.hasSavedGame = true;
                data.lastSaveDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                string json = JsonUtility.ToJson(data, true);
                PlayerPrefs.SetString(SAVE_KEY, json);
                PlayerPrefs.Save();
                Debug.Log("[Lunaris SaveSystem] Game progress saved successfully.");
            }
            catch (Exception ex)
            {
                Debug.LogError("[Lunaris SaveSystem] Failed to save: " + ex.Message);
            }
        }

        public static PlayerSaveData Load()
        {
            if (!HasSave())
            {
                return new PlayerSaveData();
            }

            try
            {
                string json = PlayerPrefs.GetString(SAVE_KEY);
                return JsonUtility.FromJson<PlayerSaveData>(json);
            }
            catch (Exception ex)
            {
                Debug.LogError("[Lunaris SaveSystem] Corrupt save file, returning new: " + ex.Message);
                return new PlayerSaveData();
            }
        }

        public static bool HasSave()
        {
            return PlayerPrefs.HasKey(SAVE_KEY) && !string.IsNullOrEmpty(PlayerPrefs.GetString(SAVE_KEY));
        }

        public static void ClearSave()
        {
            PlayerPrefs.DeleteKey(SAVE_KEY);
            PlayerPrefs.Save();
            Debug.Log("[Lunaris SaveSystem] Save data cleared.");
        }
    }
}
