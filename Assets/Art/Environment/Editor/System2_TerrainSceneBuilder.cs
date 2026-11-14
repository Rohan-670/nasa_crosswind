#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using Outpost.Environment;
using Lunaris.LunarSurface;

namespace Outpost.Environment.Editor
{
    public static class System2_TerrainSceneBuilder
    {
        private const string SCENE_PATH = "Assets/Scenes/System2_LunarTerrainTest.unity";
        private const string TERRAIN_OBJ_PATH = "Assets/Art/Environment/LunarTerrain/LunarTerrain_SouthPole.obj";
        private const string REGOLITH_MAT_PATH = "Assets/Art/Materials/M_LunarRegolith_PBR.mat";
        private const string ROCK_MAT_PATH = "Assets/Art/Materials/M_LunarRock_PBR.mat";
        private const string LANDING_PAD_MAT_PATH = "Assets/Art/Materials/M_LandingPad_PBR.mat";
        private const string EARTH_MAT_PATH = "Assets/Art/Materials/M_EarthGlobe.mat";
        private const string SEV_PREFAB_PATH = "Assets/Art/Vehicles/Prefabs/NASA_SpaceExplorationVehicle.prefab";

        [MenuItem("OUTPOST/System 2 - Build Lunar Terrain Test Scene", false, 10)]
        public static void BuildScene()
        {
            Debug.Log("[System 2] Starting Lunar Terrain and Environment generation...");

            // 1. Force refresh AssetDatabase to ensure all models and materials are imported
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            // 2. Build or update Rock Prefabs
            BuildRockPrefabs();

            // 3. Create fresh new scene
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "System2_LunarTerrainTest";

            // 4. Set Environment Lighting & Skybox
            ConfigureRenderSettings();

            // 5. Sun Directional Light (Lunar South Pole low-angle perpetual sunlight)
            CreateSunLight();

            // 6. Earth Celestial Globe in space
            CreateDistantEarth();

            // 7. Lunar Ground Terrain
            GameObject terrainObj = CreateLunarTerrain();

            // 8. Landing Pad & Blast Clearance
            CreateLandingPad();

            // 9. Boundary Colliders (Invisible amphitheater barrier)
            CreatePerimeterBoundaryColliders();

            // 10. Outpost Location Anchors with Visual Beacons
            CreateAllLocationAnchors();

            // 11. Rock Formations & Boulder Scatters
            ScatterRocksAndBoulders();

            // 12. Deploy NASA SEV Rover at Rover Station
            DeploySEVRover();

            // 13. Player Actor (Astronaut EVA Controller & Camera)
            CreatePlayerActor();

            // 14. Save Scene
            EditorSceneManager.SaveScene(scene, SCENE_PATH);
            Debug.Log($"[System 2] Lunar Terrain test scene successfully created and saved to: {SCENE_PATH}");
        }

        private static void ConfigureRenderSettings()
        {
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientSkyColor = new Color(0.04f, 0.045f, 0.06f, 1f);
            RenderSettings.ambientIntensity = 0.8f;
            RenderSettings.fog = false; // No atmospheric fog on the vacuum surface of the Moon
            RenderSettings.skybox = null; // Deep black starry void
        }

        private static void CreateSunLight()
        {
            GameObject sunObj = new GameObject("Sun_DirectionalLight");
            Light sun = sunObj.AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.color = new Color(1.0f, 0.98f, 0.94f);
            sun.intensity = 2.2f;
            sun.shadows = LightShadows.Soft;
            sun.shadowResolution = UnityEngine.Rendering.LightShadowResolution.High;
            sun.shadowBias = 0.05f;
            sun.shadowNormalBias = 0.4f;

            // Lunar South Pole low solar elevation (6.5 degrees above horizon)
            sunObj.transform.rotation = Quaternion.Euler(6.5f, 45f, 0f);
        }

        private static void CreateDistantEarth()
        {
            GameObject earthRoot = new GameObject("Celestial_Earth_System");
            earthRoot.transform.position = new Vector3(140f, 85f, 520f);

            GameObject earthSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            earthSphere.name = "Earth_Globe";
            earthSphere.transform.SetParent(earthRoot.transform);
            earthSphere.transform.localPosition = Vector3.zero;
            earthSphere.transform.localScale = Vector3.one * 48f;
            earthSphere.transform.localRotation = Quaternion.Euler(23.5f, 160f, 0f);

            Object.DestroyImmediate(earthSphere.GetComponent<Collider>());

            Material earthMat = AssetDatabase.LoadAssetAtPath<Material>(EARTH_MAT_PATH);
            if (earthMat != null)
            {
                earthSphere.GetComponent<MeshRenderer>().sharedMaterial = earthMat;
            }
        }

        private static GameObject CreateLunarTerrain()
        {
            GameObject terrainObj = new GameObject("Lunar_Surface_Terrain");
            terrainObj.isStatic = true;
            terrainObj.tag = "Untagged";
            terrainObj.layer = 0; // Default layer

            MeshFilter mf = terrainObj.AddComponent<MeshFilter>();
            MeshRenderer mr = terrainObj.AddComponent<MeshRenderer>();
            MeshCollider mc = terrainObj.AddComponent<MeshCollider>();

            // Load generated terrain mesh
            Mesh terrainMesh = AssetDatabase.LoadAssetAtPath<Mesh>(TERRAIN_OBJ_PATH);
            if (terrainMesh != null)
            {
                mf.sharedMesh = terrainMesh;
                mc.sharedMesh = terrainMesh;
            }
            else
            {
                // Fallback: load model root
                GameObject terrainModel = AssetDatabase.LoadAssetAtPath<GameObject>(TERRAIN_OBJ_PATH);
                if (terrainModel != null)
                {
                    MeshFilter modelMf = terrainModel.GetComponentInChildren<MeshFilter>();
                    if (modelMf != null)
                    {
                        mf.sharedMesh = modelMf.sharedMesh;
                        mc.sharedMesh = modelMf.sharedMesh;
                    }
                }
            }

            Material regolithMat = AssetDatabase.LoadAssetAtPath<Material>(REGOLITH_MAT_PATH);
            if (regolithMat != null)
            {
                mr.sharedMaterial = regolithMat;
            }

            return terrainObj;
        }

        private static void CreateLandingPad()
        {
            GameObject padRoot = new GameObject("Landing_Pad_Platform");
            padRoot.transform.position = new Vector3(0f, 0.04f, -80f);

            GameObject padCylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            padCylinder.name = "Pad_Concrete_Disc";
            padCylinder.transform.SetParent(padRoot.transform);
            padCylinder.transform.localPosition = Vector3.zero;
            padCylinder.transform.localScale = new Vector3(36f, 0.08f, 36f);

            Material padMat = AssetDatabase.LoadAssetAtPath<Material>(LANDING_PAD_MAT_PATH);
            if (padMat != null)
            {
                padCylinder.GetComponent<MeshRenderer>().sharedMaterial = padMat;
            }

            // 4 Solar Navigation Beacons around landing pad
            float radius = 17f;
            Vector3[] beaconPositions = new Vector3[]
            {
                new Vector3(-radius, 0.5f, -radius),
                new Vector3(radius, 0.5f, -radius),
                new Vector3(-radius, 0.5f, radius),
                new Vector3(radius, 0.5f, radius)
            };

            for (int i = 0; i < beaconPositions.Length; i++)
            {
                GameObject beacon = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                beacon.name = $"Landing_Beacon_{i + 1}";
                beacon.transform.SetParent(padRoot.transform);
                beacon.transform.localPosition = beaconPositions[i];
                beacon.transform.localScale = new Vector3(0.4f, 0.6f, 0.4f);

                Light bLight = beacon.AddComponent<Light>();
                bLight.type = LightType.Point;
                bLight.color = new Color(0.2f, 0.9f, 1.0f); // Cyan aviation guidance
                bLight.range = 10f;
                bLight.intensity = 1.8f;
            }
        }

        private static void CreatePerimeterBoundaryColliders()
        {
            GameObject boundaryRoot = new GameObject("Perimeter_Boundary_Colliders");
            boundaryRoot.transform.position = Vector3.zero;

            // Octagonal perimeter barrier wall at radius 132m, height 50m
            int segments = 8;
            float radius = 132f;
            float wallWidth = 105f;

            for (int i = 0; i < segments; i++)
            {
                float angle = i * (360f / segments) * Mathf.Deg2Rad;
                float x = Mathf.Sin(angle) * radius;
                float z = Mathf.Cos(angle) * radius;

                GameObject wall = new GameObject($"Boundary_Wall_{i + 1}");
                wall.transform.SetParent(boundaryRoot.transform);
                wall.transform.position = new Vector3(x, 25f, z);
                wall.transform.rotation = Quaternion.Euler(0f, i * (360f / segments) + 90f, 0f);

                BoxCollider box = wall.AddComponent<BoxCollider>();
                box.size = new Vector3(wallWidth, 60f, 6f);
            }
        }

        private static void CreateAllLocationAnchors()
        {
            GameObject anchorsGroup = new GameObject("=== OUTPOST LOCATION ANCHORS ===");

            CreateAnchor(anchorsGroup, StationLocationType.MainHabitat, "Main Habitat (Central Command)",
                new Vector3(0f, 0f, 0f), new Color(0.2f, 0.8f, 1f), 22f);

            CreateAnchor(anchorsGroup, StationLocationType.LandingZone, "Landing Zone",
                new Vector3(0f, 0f, -80f), new Color(0.3f, 1f, 0.5f), 20f);

            CreateAnchor(anchorsGroup, StationLocationType.SolarFarm, "Solar Power Array Farm",
                new Vector3(75f, 4.5f, 20f), new Color(1f, 0.85f, 0.1f), 18f);

            CreateAnchor(anchorsGroup, StationLocationType.Greenhouse, "Greenhouse Hydroponics Biodome",
                new Vector3(-45f, 0f, 35f), new Color(0.2f, 1f, 0.3f), 16f);

            CreateAnchor(anchorsGroup, StationLocationType.ResearchLab, "Geological & Science Laboratory",
                new Vector3(40f, 0f, 45f), new Color(0.1f, 0.9f, 0.9f), 16f);

            CreateAnchor(anchorsGroup, StationLocationType.CommTower, "Earth Communication Relay Tower",
                new Vector3(60f, 11.5f, -40f), new Color(0.9f, 0.4f, 1f), 14f);

            CreateAnchor(anchorsGroup, StationLocationType.RoverStation, "NASA SEV Rover Maintenance Station",
                new Vector3(-35f, 0f, -30f), new Color(1f, 0.5f, 0.1f), 15f);

            CreateAnchor(anchorsGroup, StationLocationType.GeologicalResearchArea, "Geological Rim Research Area",
                new Vector3(-80f, -1.5f, 70f), new Color(0.8f, 0.8f, 0.9f), 25f);

            CreateAnchor(anchorsGroup, StationLocationType.EquipmentStorage, "Equipment & Spare Parts Storage",
                new Vector3(25f, 0f, -45f), new Color(0.7f, 0.7f, 0.7f), 14f);

            CreateAnchor(anchorsGroup, StationLocationType.EmergencySubstation, "Power Conduit Substation (Ch 3 Incident)",
                new Vector3(50f, 1.8f, 10f), new Color(1f, 0.2f, 0.2f), 12f);
        }

        private static void CreateAnchor(GameObject parent, StationLocationType type, string displayName, Vector3 pos, Color color, float radius)
        {
            GameObject anchorObj = new GameObject($"Anchor_{type}");
            anchorObj.transform.SetParent(parent.transform);
            anchorObj.transform.position = pos;

            OutpostLocationAnchor anchorComp = anchorObj.AddComponent<OutpostLocationAnchor>();

            // Serialized fields reflection or standard assignment
            var so = new SerializedObject(anchorComp);
            so.FindProperty("stationType").enumValueIndex = (int)type;
            so.FindProperty("stationDisplayName").stringValue = displayName;
            so.FindProperty("markerColor").colorValue = color;
            so.FindProperty("zoneRadius").floatValue = radius;
            so.ApplyModifiedProperties();

            // Visual mast pole marker
            GameObject mast = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            mast.name = "Anchor_Mast";
            mast.transform.SetParent(anchorObj.transform);
            mast.transform.localPosition = new Vector3(0f, 2.5f, 0f);
            mast.transform.localScale = new Vector3(0.2f, 2.5f, 0.2f);
            Object.DestroyImmediate(mast.GetComponent<Collider>());

            GameObject beacon = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            beacon.name = "Anchor_Beacon_Orb";
            beacon.transform.SetParent(anchorObj.transform);
            beacon.transform.localPosition = new Vector3(0f, 5.2f, 0f);
            beacon.transform.localScale = Vector3.one * 0.6f;
            Object.DestroyImmediate(beacon.GetComponent<Collider>());

            Light pLight = beacon.AddComponent<Light>();
            pLight.type = LightType.Point;
            pLight.color = color;
            pLight.range = 8f;
            pLight.intensity = 2.0f;
        }

        private static void BuildRockPrefabs()
        {
            string[] rockNames = new string[] { "LunarRock_Small", "LunarRock_Medium", "LunarCrag_Large", "LunarSample_Geological" };
            Material rockMat = AssetDatabase.LoadAssetAtPath<Material>(ROCK_MAT_PATH);

            foreach (var rName in rockNames)
            {
                string objPath = $"Assets/Art/Props/{rName}.obj";
                string prefabPath = $"Assets/Art/Prefabs/Prefab_{rName}.prefab";

                GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(objPath);
                if (model == null) continue;

                GameObject instance = Object.Instantiate(model);
                instance.name = $"Prefab_{rName}";

                MeshFilter mf = instance.GetComponentInChildren<MeshFilter>();
                MeshRenderer mr = instance.GetComponentInChildren<MeshRenderer>();
                if (mr != null && rockMat != null)
                {
                    mr.sharedMaterial = rockMat;
                }

                // Add appropriate collider
                if (rName.Contains("Small"))
                {
                    instance.AddComponent<SphereCollider>();
                }
                else if (rName.Contains("Medium"))
                {
                    instance.AddComponent<BoxCollider>();
                }
                else if (rName.Contains("Large"))
                {
                    MeshCollider mc = instance.AddComponent<MeshCollider>();
                    mc.convex = true;
                }
                else if (rName.Contains("Sample"))
                {
                    SphereCollider sc = instance.AddComponent<SphereCollider>();
                    sc.radius = 0.4f;
                    sc.isTrigger = true;
                }

                PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
                Object.DestroyImmediate(instance);
                Debug.Log($"[System 2] Built rock prefab: {prefabPath}");
            }
        }

        private static void ScatterRocksAndBoulders()
        {
            GameObject rocksRoot = new GameObject("Environment_Rock_Formations");
            Material rockMat = AssetDatabase.LoadAssetAtPath<Material>(ROCK_MAT_PATH);

            GameObject smallRockPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Art/Prefabs/Prefab_LunarRock_Small.prefab");
            GameObject medRockPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Art/Prefabs/Prefab_LunarRock_Medium.prefab");
            GameObject cragPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Art/Prefabs/Prefab_LunarCrag_Large.prefab");
            GameObject samplePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Art/Prefabs/Prefab_LunarSample_Geological.prefab");

            // Strategic scatter positions along crater rims and Geological Area
            Vector3[] cragPositions = new Vector3[]
            {
                new Vector3(-78f, 0.8f, 42f),
                new Vector3(-45f, 0.9f, 48f),
                new Vector3(58f, 0.7f, -54f),
                new Vector3(-92f, -0.5f, 85f),
                new Vector3(-75f, -1.0f, 65f),
                new Vector3(72f, 11.2f, -38f)
            };

            for (int i = 0; i < cragPositions.Length; i++)
            {
                GameObject crag = (cragPrefab != null) ? PrefabUtility.InstantiatePrefab(cragPrefab) as GameObject
                                                      : GameObject.CreatePrimitive(PrimitiveType.Cube);
                crag.name = $"Crag_Formation_{i + 1}";
                crag.transform.SetParent(rocksRoot.transform);
                crag.transform.position = cragPositions[i];
                crag.transform.rotation = Quaternion.Euler(0f, (i * 67f) % 360f, 0f);
            }

            // Scatter medium boulders
            Vector3[] medPositions = new Vector3[]
            {
                new Vector3(-25f, 0.2f, 15f),
                new Vector3(30f, 0.1f, -15f),
                new Vector3(-65f, -1.2f, 38f),
                new Vector3(45f, -0.8f, -62f),
                new Vector3(-82f, -1.8f, 72f)
            };

            for (int i = 0; i < medPositions.Length; i++)
            {
                GameObject boulder = (medRockPrefab != null) ? PrefabUtility.InstantiatePrefab(medRockPrefab) as GameObject
                                                             : GameObject.CreatePrimitive(PrimitiveType.Sphere);
                boulder.name = $"Boulder_Medium_{i + 1}";
                boulder.transform.SetParent(rocksRoot.transform);
                boulder.transform.position = medPositions[i];
                boulder.transform.rotation = Quaternion.Euler(0f, (i * 93f) % 360f, 0f);
            }

            // Scatter geological samples in the Geological Research Area
            Vector3[] samplePositions = new Vector3[]
            {
                new Vector3(-78f, -1.4f, 68f),
                new Vector3(-82f, -1.6f, 74f),
                new Vector3(-85f, -1.5f, 70f)
            };

            for (int i = 0; i < samplePositions.Length; i++)
            {
                GameObject sample = (samplePrefab != null) ? PrefabUtility.InstantiatePrefab(samplePrefab) as GameObject
                                                           : GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sample.name = $"Geological_Sample_Rock_{i + 1}";
                sample.transform.SetParent(rocksRoot.transform);
                sample.transform.position = samplePositions[i];
                sample.transform.localScale = Vector3.one * 1.2f;
            }
        }

        private static void DeploySEVRover()
        {
            GameObject roverPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(SEV_PREFAB_PATH);
            if (roverPrefab != null)
            {
                GameObject rover = PrefabUtility.InstantiatePrefab(roverPrefab) as GameObject;
                rover.name = "NASA_SpaceExplorationVehicle";
                rover.transform.position = new Vector3(-35f, 0.35f, -30f);
                rover.transform.rotation = Quaternion.Euler(0f, 120f, 0f);
                Debug.Log("[System 2] NASA SEV Rover deployed to Rover Station (-35, 0.35, -30).");
            }
        }

        private static void CreatePlayerActor()
        {
            GameObject player = new GameObject("Astronaut_Player");
            player.tag = "Player";
            player.transform.position = new Vector3(0f, 0.5f, -75f); // On Landing Pad
            player.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

            CharacterController cc = player.AddComponent<CharacterController>();
            cc.radius = 0.45f;
            cc.height = 1.85f;
            cc.center = new Vector3(0f, 0.925f, 0f);
            cc.stepOffset = 0.35f;

            AstronautController ac = player.AddComponent<AstronautController>();

            // Setup Main Camera child
            GameObject camObj = new GameObject("Main Camera");
            camObj.tag = "MainCamera";
            camObj.transform.SetParent(player.transform);
            camObj.transform.localPosition = new Vector3(0f, 1.6f, 0f);
            camObj.transform.localRotation = Quaternion.identity;

            Camera cam = camObj.AddComponent<Camera>();
            cam.nearClipPlane = 0.1f;
            cam.farClipPlane = 1000f;
            cam.fieldOfView = 65f;
            camObj.AddComponent<AudioListener>();

            // Wire camera to controller
            var so = new SerializedObject(ac);
            so.FindProperty("astronautCamera").objectReferenceValue = cam;

            // Helmet Spotlight
            GameObject lightObj = new GameObject("Helmet_Light");
            lightObj.transform.SetParent(camObj.transform);
            lightObj.transform.localPosition = new Vector3(0f, 0.05f, 0.15f);
            lightObj.transform.localRotation = Quaternion.identity;

            Light hLight = lightObj.AddComponent<Light>();
            hLight.type = LightType.Spot;
            hLight.range = 30f;
            hLight.spotAngle = 55f;
            hLight.intensity = 2.5f;
            hLight.color = new Color(0.95f, 0.97f, 1.0f);
            hLight.shadows = LightShadows.Hard;

            so.FindProperty("helmetLight").objectReferenceValue = hLight;
            so.ApplyModifiedProperties();

            Debug.Log("[System 2] Astronaut Player Actor created at (0, 0.5, -75) with CharacterController, Camera, and Helmet Light.");
        }
    }
}
#endif
