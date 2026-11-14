using System;
using UnityEngine;
using Outpost.Core.Data;

namespace Outpost.System1.Gameplay.Persistence
{
    /// <summary>
    /// Legacy compatibility bridge for OutpostSaveSystem.
    /// Delegates to the centralized SaveManager.
    /// </summary>
    public static class OutpostSaveSystem
    {
        public static void SaveProgress(MissionChapterId chapter, RepairChoice choice, Vector3 playerPos)
        {
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.SaveGameState(chapter, choice, playerPos);
            }
        }

        public static OutpostSaveModel LoadProgress()
        {
            return SaveManager.Instance != null ? SaveManager.Instance.LoadGameState() : null;
        }

        public static void ClearSave()
        {
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.DeleteSave();
            }
        }
    }
}
