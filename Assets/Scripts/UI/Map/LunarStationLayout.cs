using System;
using System.Collections.Generic;
using UnityEngine;

namespace Outpost.UI.Map
{
    /// <summary>
    /// Master registry of physical Lunar South Pole station landmarks and coordinates.
    /// Ensures 100% correlation between the 3D Unity scene and the 2D Geo Map without arbitrary offsets.
    /// </summary>
    public static class LunarStationLayout
    {
        public static readonly Vector3 MainHabitat = new Vector3(0.0f, 0.0f, 0.0f);
        public static readonly Vector3 LandingZone = new Vector3(-180.0f, 5.0f, -120.0f);
        public static readonly Vector3 SolarFarm = new Vector3(120.0f, 25.0f, 160.0f);
        public static readonly Vector3 Greenhouse = new Vector3(-40.0f, 2.0f, 70.0f);
        public static readonly Vector3 ResearchLab = new Vector3(60.0f, 2.0f, -30.0f);
        public static readonly Vector3 CommTower = new Vector3(150.0f, 45.0f, -90.0f);
        public static readonly Vector3 RoverStation = new Vector3(-75.0f, 0.0f, -45.0f);
        public static readonly Vector3 EquipmentStorage = new Vector3(-25.0f, 0.0f, -60.0f);
        public static readonly Vector3 GeologyArea = new Vector3(-220.0f, -15.0f, 250.0f);
        public static readonly Vector3 EmergencyImpactSite = new Vector3(110.0f, 22.0f, 140.0f);
        public static readonly Vector3 EmergencyLifeSupportHub = new Vector3(15.0f, 0.0f, 30.0f);

        public static Vector3 GetCoordinate(string locationId)
        {
            return locationId switch
            {
                "LOC_HABITAT" => MainHabitat,
                "LOC_LANDING" => LandingZone,
                "LOC_SOLAR" => SolarFarm,
                "LOC_GREENHOUSE" => Greenhouse,
                "LOC_LAB" => ResearchLab,
                "LOC_COMM" => CommTower,
                "LOC_ROVER" => RoverStation,
                "LOC_STORAGE" => EquipmentStorage,
                "LOC_GEOLOGY" => GeologyArea,
                "LOC_EMERGENCY_1" => EmergencyImpactSite,
                "LOC_EMERGENCY_2" => EmergencyLifeSupportHub,
                _ => Vector3.zero
            };
        }
    }
}
