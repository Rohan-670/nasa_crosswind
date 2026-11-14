using System;
using UnityEngine;

namespace Lunaris.Navigation
{
    [Serializable]
    public class RouteCheckpoint
    {
        public int sequenceNumber;
        public string checkpointName;
        public Vector3 position;
        public float arrivalRadius = 35f;
        public bool isCompleted = false;

        public RouteCheckpoint(int seq, string name, Vector3 pos, float radius = 35f)
        {
            sequenceNumber = seq;
            checkpointName = name;
            position = pos;
            arrivalRadius = radius;
            isCompleted = false;
        }
    }
}
