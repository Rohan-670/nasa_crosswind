using System;
using UnityEngine;

namespace Outpost.Environment
{
    public enum StationLocationType
    {
        MainHabitat,
        LandingZone,
        SolarFarm,
        Greenhouse,
        ResearchLab,
        CommTower,
        RoverStation,
        GeologicalResearchArea,
        EquipmentStorage,
        EmergencySubstation
    }

    [SelectionBase]
    [ExecuteInEditMode]
    public class OutpostLocationAnchor : MonoBehaviour
    {
        [Header("Anchor Configuration")]
        [SerializeField] private StationLocationType stationType = StationLocationType.MainHabitat;
        [SerializeField] private string stationDisplayName = "Main Habitat";
        [SerializeField] private Color markerColor = new Color(0.2f, 0.8f, 1f, 1f);
        [SerializeField] private float zoneRadius = 15f;

        public StationLocationType StationType => stationType;
        public string StationDisplayName => stationDisplayName;
        public Color MarkerColor => markerColor;
        public float ZoneRadius => zoneRadius;
        public Vector3 WorldPosition => transform.position;

        private void OnDrawGizmos()
        {
            Gizmos.color = markerColor;
            Gizmos.DrawWireSphere(transform.position, 1.5f);
            Gizmos.DrawWireCube(transform.position + Vector3.up * 1f, new Vector3(0.5f, 2f, 0.5f));

            // Zone boundary outline
            Gizmos.color = new Color(markerColor.r, markerColor.g, markerColor.b, 0.25f);
            Gizmos.DrawWireSphere(transform.position, zoneRadius);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, zoneRadius);
        }
    }
}
