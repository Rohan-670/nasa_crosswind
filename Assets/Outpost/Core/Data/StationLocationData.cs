using System;
using UnityEngine;

namespace Outpost.Core.Data
{
    public enum StationLocationId
    {
        MainHabitat,            // 1. Central Hab
        LandingZone,            // 2. Lunar descent module touchdown site
        SolarFarm,              // 3. Ridge of eternal light solar array
        Greenhouse,             // 4. Hydroponics and food biomass pod
        ResearchLaboratory,     // 5. Geology and scientific analysis
        CommunicationTower,     // 6. Deep space communications dish
        RoverStation,           // 7. Lunar rover garage and charging pad
        EquipmentStorage,       // 8. Airway, spare canisters, tools
        GeologicalResearchArea, // 9. Crater rim & water-ice volatiles
        EmergencyBreachSite     // 10. Damaged electrical relay junction
    }

    [Serializable]
    public class StationLocationData
    {
        public StationLocationId Id;
        public string DisplayName;
        public string ShortCode;
        public string Description;
        public Vector3 WorldPosition;
        public bool IsDiscovered;
        public bool IsAvailable;
        public bool IsEmergency;

        public StationLocationData(
            StationLocationId id,
            string displayName,
            string shortCode,
            string description,
            Vector3 worldPosition,
            bool isDiscovered = false,
            bool isAvailable = true,
            bool isEmergency = false)
        {
            Id = id;
            DisplayName = displayName;
            ShortCode = shortCode;
            Description = description;
            WorldPosition = worldPosition;
            IsDiscovered = isDiscovered;
            IsAvailable = isAvailable;
            IsEmergency = isEmergency;
        }

        public static StationLocationData[] GetDefaultOutpostLayout()
        {
            return new StationLocationData[]
            {
                new StationLocationData(
                    StationLocationId.MainHabitat,
                    "Main Habitat Alpha",
                    "HAB",
                    "Central living module, crew quarters, life-support distribution hub, and ORION AI interface.",
                    new Vector3(0f, 0f, 0f),
                    isDiscovered: true,
                    isAvailable: true
                ),
                new StationLocationData(
                    StationLocationId.LandingZone,
                    "Landing Zone Orion-1",
                    "LZ",
                    "Touchdown coordinates for the lunar descent module. Mission start point.",
                    new Vector3(-120f, 0f, -80f),
                    isDiscovered: true,
                    isAvailable: true
                ),
                new StationLocationData(
                    StationLocationId.SolarFarm,
                    "Solar Array Ridge",
                    "SOLAR",
                    "High-efficiency photovoltaic panels angled along the Ridge of Eternal Light.",
                    new Vector3(100f, 15f, 60f),
                    isDiscovered: false,
                    isAvailable: true
                ),
                new StationLocationData(
                    StationLocationId.Greenhouse,
                    "Hydroponic Greenhouse",
                    "BIO",
                    "Automated food production and oxygen-refreshing biomass growth chambers.",
                    new Vector3(35f, 0f, 20f),
                    isDiscovered: false,
                    isAvailable: true
                ),
                new StationLocationData(
                    StationLocationId.ResearchLaboratory,
                    "Geology & Research Lab",
                    "LAB",
                    "Clean laboratory equipped for lunar regolith, mineral, and ice-core analysis.",
                    new Vector3(-40f, 0f, 30f),
                    isDiscovered: false,
                    isAvailable: true
                ),
                new StationLocationData(
                    StationLocationId.CommunicationTower,
                    "Deep Space Comms Tower",
                    "COMMS",
                    "High-gain parabolic dish maintaining continuous laser/radio links with Earth Ground Control.",
                    new Vector3(140f, 30f, -50f),
                    isDiscovered: false,
                    isAvailable: true
                ),
                new StationLocationData(
                    StationLocationId.RoverStation,
                    "Rover Dock & Recharger",
                    "ROVER",
                    "Surface mobility vehicle maintenance station and inductive fast-charging pad.",
                    new Vector3(-60f, 0f, -35f),
                    isDiscovered: false,
                    isAvailable: true
                ),
                new StationLocationData(
                    StationLocationId.EquipmentStorage,
                    "EVA Equipment & Storage",
                    "STORE",
                    "Spare lithium hydroxide canisters, replacement solar cables, tools, and EVA suits.",
                    new Vector3(20f, 0f, -50f),
                    isDiscovered: false,
                    isAvailable: true
                ),
                new StationLocationData(
                    StationLocationId.GeologicalResearchArea,
                    "Shackleton Crater Ice Field",
                    "GEO",
                    "Permanently shadowed cold trap containing deposits of prehistoric water-ice.",
                    new Vector3(-180f, -10f, 110f),
                    isDiscovered: false,
                    isAvailable: true
                ),
                new StationLocationData(
                    StationLocationId.EmergencyBreachSite,
                    "External Relay Junction #4",
                    "EMERG",
                    "Impact point of high-velocity micrometeoroid debris. Power conduit severed.",
                    new Vector3(85f, 12f, 45f),
                    isDiscovered: false,
                    isAvailable: false,
                    isEmergency: true
                )
            };
        }
    }
}
