using System.Collections.Generic;
using UnityEngine;

namespace Lunaris.Navigation
{
    [RequireComponent(typeof(LineRenderer))]
    public class RouteRenderer : MonoBehaviour
    {
        [Header("Corridor Visuals")]
        [SerializeField] private Material corridorMaterial;
        [SerializeField] private Material checkpointRingMaterial;
        [SerializeField] private float lineWidth = 4f;
        [SerializeField] private int pointsPerSegment = 12;

        private LineRenderer centerLine;
        private List<GameObject> checkpointVisualRings = new List<GameObject>();

        private void Awake()
        {
            centerLine = GetComponent<LineRenderer>();
            ConfigureLineRenderer();
        }

        private void Start()
        {
            RenderCorridor();
        }

        private void ConfigureLineRenderer()
        {
            centerLine.startWidth = lineWidth;
            centerLine.endWidth = lineWidth;
            centerLine.useWorldSpace = true;
            centerLine.material = corridorMaterial;
            centerLine.numCapVertices = 4;
            centerLine.numCornerVertices = 4;
            centerLine.startColor = new Color(0f, 0.8f, 1f, 0.9f);
            centerLine.endColor = new Color(0.2f, 1f, 0.8f, 0.9f);
        }

        public void RenderCorridor()
        {
            if (NavigationRouteManager.Instance == null) return;

            var checkpoints = NavigationRouteManager.Instance.Checkpoints;
            if (checkpoints == null || checkpoints.Count < 2) return;

            List<Vector3> smoothPoints = new List<Vector3>();

            for (int i = 0; i < checkpoints.Count - 1; i++)
            {
                Vector3 p0 = i > 0 ? checkpoints[i - 1].position : checkpoints[i].position;
                Vector3 p1 = checkpoints[i].position;
                Vector3 p2 = checkpoints[i + 1].position;
                Vector3 p3 = (i + 2 < checkpoints.Count) ? checkpoints[i + 2].position : p2;

                for (int step = 0; step < pointsPerSegment; step++)
                {
                    float t = (float)step / pointsPerSegment;
                    Vector3 pt = CalculateCatmullRom(p0, p1, p2, p3, t);
                    smoothPoints.Add(pt);
                }
            }
            smoothPoints.Add(checkpoints[checkpoints.Count - 1].position);

            centerLine.positionCount = smoothPoints.Count;
            centerLine.SetPositions(smoothPoints.ToArray());

            SpawnCheckpointRings(checkpoints);
        }

        private void SpawnCheckpointRings(IReadOnlyList<RouteCheckpoint> checkpoints)
        {
            // Clear existing rings
            foreach (var r in checkpointVisualRings)
            {
                if (r != null) Destroy(r);
            }
            checkpointVisualRings.Clear();

            for (int i = 0; i < checkpoints.Count; i++)
            {
                var cp = checkpoints[i];
                GameObject ringObj = new GameObject($"CheckpointRing_{cp.sequenceNumber}");
                ringObj.transform.SetParent(transform);
                ringObj.transform.position = cp.position;

                LineRenderer ringLine = ringObj.AddComponent<LineRenderer>();
                ringLine.useWorldSpace = false;
                ringLine.startWidth = 1.5f;
                ringLine.endWidth = 1.5f;
                ringLine.loop = true;
                ringLine.material = checkpointRingMaterial != null ? checkpointRingMaterial : centerLine.material;
                ringLine.startColor = new Color(0f, 0.9f, 1f, 0.7f);
                ringLine.endColor = new Color(0f, 0.9f, 1f, 0.7f);

                int segments = 32;
                ringLine.positionCount = segments;
                Vector3[] circlePoints = new Vector3[segments];
                for (int s = 0; s < segments; s++)
                {
                    float angle = (float)s / segments * Mathf.PI * 2f;
                    circlePoints[s] = new Vector3(Mathf.Cos(angle) * cp.arrivalRadius, Mathf.Sin(angle) * cp.arrivalRadius, 0f);
                }
                ringLine.SetPositions(circlePoints);

                checkpointVisualRings.Add(ringObj);
            }
        }

        private Vector3 CalculateCatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            float t2 = t * t;
            float t3 = t2 * t;

            return 0.5f * (
                (2f * p1) +
                (-p0 + p2) * t +
                (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
                (-p0 + 3f * p1 - 3f * p2 + p3) * t3
            );
        }
    }
}
