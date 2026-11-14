using System;
using UnityEngine;

namespace Outpost.UI.Map
{
    public enum OutpostFacilityType
    {
        Habitat = 0,
        Power = 1,
        LifeSupport = 2,
        Science = 3,
        Communications = 4,
        Vehicles = 5,
        LandingSite = 6,
        EmergencyHazard = 7
    }

    public enum FacilityAvailability
    {
        Operational = 0,   // Fully accessible
        Caution = 1,       // Reduced power or solar activity warning
        EmergencyDamaged = 2,// Severed cables or life-support failure
        Unexplored = 3      // Beyond safe comms range
    }

    /// <summary>
    /// ScriptableObject defining an outpost landmark on the interactive Lunar Geo Map.
    /// Anchors to real Unity world coordinates and carries educational science facts.
    /// </summary>
    [CreateAssetMenu(fileName = "NewMapLocation", menuName = "Outpost/Map/Location Data")]
    public class MapLocationData : ScriptableObject
    {
        [Header("Landmark Details")]
        public string LocationId = "LOC_HABITAT";
        public string LocationName = "Main Habitat";
        public OutpostFacilityType FacilityType = OutpostFacilityType.Habitat;
        public FacilityAvailability Availability = FacilityAvailability.Operational;
        public string GridSector = "E-5";

        [Header("World Space Coordinates")]
        public Vector3 WorldPosition = Vector3.zero;

        [Header("Facility Purpose & Objectives")]
        [TextArea(2, 4)] public string FacilityPurpose = "";
        [TextArea(1, 2)] public string CurrentObjectiveHere = "";
        [TextArea(1, 3)] public string NavigationAdvice = "";

        [Header("Educational Dossier")]
        [TextArea(2, 4)] public string EducationalScienceFact = "";

        [Header("Exploration State")]
        public bool IsVisited = false;
        public Sprite LocationThumbnail;
    }
}
