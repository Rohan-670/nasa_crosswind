using System;
using UnityEngine;

namespace Lunaris.Core
{
    [Serializable]
    public class MissionProgress
    {
        public string missionName = "Lunar Expedition 01";
        public string destination = "Lunar South Pole (Shackleton Rim)";
        public float missionStartTime = 0f;
        public float elapsedMissionDays = 0f;
        public float corridorAccuracyPercentage = 100f;
        public int totalCorridorChecks = 0;
        public int compliantCorridorChecks = 0;
        public int samplesCollected = 0;
        public int totalRequiredSamples = 3;
        public bool hasLandedSafely = false;
        public bool hasReturnedToLander = false;
        public float maxRecordedDeviation = 0f;
        public string failureReason = "";

        public void RecordDeviation(float deviation, bool isCompliant)
        {
            totalCorridorChecks++;
            if (isCompliant) compliantCorridorChecks++;
            corridorAccuracyPercentage = totalCorridorChecks > 0 ? ((float)compliantCorridorChecks / totalCorridorChecks) * 100f : 100f;
            if (deviation > maxRecordedDeviation) maxRecordedDeviation = deviation;
        }

        public void UpdateElapsedDays()
        {
            // 1 minute of flight gameplay represents ~1.2 days of trans-lunar flight
            float realSeconds = Time.time - missionStartTime;
            elapsedMissionDays = 0.5f + (realSeconds / 60f) * 1.2f;
        }
    }
}
