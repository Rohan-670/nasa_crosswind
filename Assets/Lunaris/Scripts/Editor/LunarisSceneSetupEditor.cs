#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using Lunaris.Core;
using Lunaris.Navigation;
using Lunaris.Spacecraft;
using Lunaris.Cinematic;
using Lunaris.LunarSurface;
using Lunaris.UI;
using Lunaris.API;

namespace Lunaris.Editor
{
    public static class LunarisSceneSetupEditor
    {
        private const string SCENE_PATH = "Assets/Scenes/Lunaris_MasterMission.unity";
        private const string EARTH_MAT_PATH = "Assets/Lunaris/Art/Earth/Materials/earthmat.mat";
        private const string MOON_MAT_PATH = "Assets/Lunaris/Art/Moon/Materials/moonmat.mat";
        private const string CORRIDOR_MAT_PATH = "Assets/Lunaris/Art/Space/Materials/corridormat.mat";
        private const string SPACECRAFT_MODEL_PATH = "Assets/Art/Vehicles/NASA_SpaceExplorationVehicle.obj";

        [MenuItem("LUNARIS/Build & Setup Master Game Scene (Lunaris_MasterMission)", false, 1)]
        public static void BuildMasterGameScene()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            scene.name = "Lunaris_MasterMission";

            // 1. Core Managers GameObject
            GameObject coreObj = new GameObject("Core_GameManager");
            coreObj.AddComponent<LunarisGameManager>();
            coreObj.AddComponent<SoundManager>();
            coreObj.AddComponent<NasaApiClient>();
            coreObj.AddComponent<CinematicManager>();
            coreObj.AddComponent<DialogueManager>();
            coreObj.AddComponent<MainMenuController>();
            coreObj.AddComponent<MissionCompleteUI>();

            // 2. Earth Celestial System
            GameObject earthRoot = new GameObject("Earth_System");
            earthRoot.transform.position = new Vector3(0, 0, -120f);

            GameObject earthSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            earthSphere.name = "Earth_Surface";
            earthSphere.transform.SetParent(earthRoot.transform);
            earthSphere.transform.localPosition = Vector3.zero;
            earthSphere.transform.localScale = Vector3.one * 80f;

            Material earthMat = AssetDatabase.LoadAssetAtPath<Material>(EARTH_MAT_PATH);
            if (earthMat != null)
            {
                earthSphere.GetComponent<MeshRenderer>().sharedMaterial = earthMat;
            }

            // Atmosphere Shell
            GameObject earthAtmo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            earthAtmo.name = "Earth_Atmosphere";
            earthAtmo.transform.SetParent(earthRoot.transform);
            earthAtmo.transform.localPosition = Vector3.zero;
            earthAtmo.transform.localScale = Vector3.one * 83f;
            Object.DestroyImmediate(earthAtmo.GetComponent<Collider>());

            // 3. Moon Celestial System
            GameObject moonRoot = new GameObject("Moon_System");
            moonRoot.transform.position = new Vector3(0, 0, 1350f);

            GameObject moonSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            moonSphere.name = "Moon_Surface";
            moonSphere.transform.SetParent(moonRoot.transform);
            moonSphere.transform.localPosition = Vector3.zero;
            moonSphere.transform.localScale = Vector3.one * 50f;

            Material moonMat = AssetDatabase.LoadAssetAtPath<Material>(MOON_MAT_PATH);
            if (moonMat != null)
            {
                moonSphere.GetComponent<MeshRenderer>().sharedMaterial = moonMat;
            }

            // 4. Navigation Corridor System
            GameObject navRoot = new GameObject("Navigation_Corridor_System");
            NavigationRouteManager navMgr = navRoot.AddComponent<NavigationRouteManager>();
            RouteRenderer routeRenderer = navRoot.AddComponent<RouteRenderer>();

            Material corrMat = AssetDatabase.LoadAssetAtPath<Material>(CORRIDOR_MAT_PATH);
            if (corrMat != null)
            {
                navRoot.GetComponent<LineRenderer>().sharedMaterial = corrMat;
            }

            // 5. Spacecraft Player Actor
            GameObject craftObj = new GameObject("Spacecraft_Lunaris");
            craftObj.tag = "Player";
            craftObj.transform.position = new Vector3(0, 0, 10f);

            // Instantiate visual model
            GameObject craftModelAsset = AssetDatabase.LoadAssetAtPath<GameObject>(SPACECRAFT_MODEL_PATH);
            if (craftModelAsset != null)
            {
                GameObject visual = (GameObject)PrefabUtility.InstantiatePrefab(craftModelAsset, craftObj.transform);
                visual.name = "Model";
                visual.transform.localPosition = Vector3.zero;
            }
            else
            {
                GameObject cubePlace = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cubePlace.name = "Model";
                cubePlace.transform.SetParent(craftObj.transform);
                cubePlace.transform.localScale = new Vector3(2f, 1.2f, 4f);
            }

            // Add Components to Spacecraft
            BoxCollider craftCol = craftObj.AddComponent<BoxCollider>();
            craftCol.size = new Vector3(3.2f, 2.2f, 5.0f);
            craftObj.AddComponent<SpacecraftFlightController>();
            craftObj.AddComponent<SpacecraftCockpitHUD>();
            craftObj.AddComponent<SpacecraftDestruction>();

            // Follow Camera
            GameObject mainCamObj = GameObject.FindWithTag("MainCamera");
            if (mainCamObj != null)
            {
                mainCamObj.transform.SetParent(craftObj.transform);
                mainCamObj.transform.localPosition = new Vector3(0f, 2.2f, -6.5f);
                mainCamObj.transform.localRotation = Quaternion.Euler(12f, 0f, 0f);
            }

            // 6. Lunar Surface Expedition Zone
            GameObject lunarExpeditionRoot = new GameObject("Lunar_Expedition_Zone");
            lunarExpeditionRoot.transform.position = new Vector3(0, 0, 1300f);

            // Ground Plane (Lunar Regolith)
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Lunar_Ground_Shackleton";
            ground.transform.SetParent(lunarExpeditionRoot.transform);
            ground.transform.localPosition = Vector3.zero;
            ground.transform.localScale = Vector3.one * 15f; // 150m x 150m exploration area
            if (moonMat != null) ground.GetComponent<MeshRenderer>().sharedMaterial = moonMat;

            // Collectible Scientific Samples
            SpawnSampleItem(lunarExpeditionRoot.transform, new Vector3(15f, 0.4f, 20f), "Basaltic Lunar Core", 1.8f);
            SpawnSampleItem(lunarExpeditionRoot.transform, new Vector3(-25f, 0.4f, 35f), "Anorthosite Crust Specimen", 2.2f);
            SpawnSampleItem(lunarExpeditionRoot.transform, new Vector3(5f, 0.4f, 50f), "Permanently Shadowed Ice Core", 0.9f);

            // 7. Save Scene and add to Build Settings
            string sceneDir = Path.GetDirectoryName(SCENE_PATH);
            if (!Directory.Exists(sceneDir)) Directory.CreateDirectory(sceneDir);

            EditorSceneManager.SaveScene(scene, SCENE_PATH);
            AddSceneToBuildSettings(SCENE_PATH);

            Debug.Log("[LUNARIS] Successfully built Master Game Scene at " + SCENE_PATH);
            EditorUtility.DisplayDialog("LUNARIS Build Complete", "Master Game Scene 'Lunaris_MasterMission' built and saved successfully with full Earth, Moon, Navigation, Cockpit, and Lunar Surface systems!", "OK");
        }

        private static void SpawnSampleItem(Transform parent, Vector3 localPos, string name, float weight)
        {
            GameObject sampleObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            sampleObj.name = "Sample_" + name.Replace(" ", "_");
            sampleObj.transform.SetParent(parent);
            sampleObj.transform.localPosition = localPos;
            sampleObj.transform.localScale = new Vector3(0.5f, 0.3f, 0.5f);

            LunarSampleItem item = sampleObj.AddComponent<LunarSampleItem>();

            // Beacon point light
            GameObject lightObj = new GameObject("BeaconLight");
            lightObj.transform.SetParent(sampleObj.transform);
            lightObj.transform.localPosition = Vector3.up * 1.5f;
            Light l = lightObj.AddComponent<Light>();
            l.type = LightType.Point;
            l.color = new Color(0f, 1f, 0.9f);
            l.range = 8f;
            l.intensity = 2f;
        }

        private static void AddSceneToBuildSettings(string path)
        {
            EditorBuildSettingsScene[] original = EditorBuildSettings.scenes;
            foreach (var s in original)
            {
                if (s.path == path) return;
            }

            EditorBuildSettingsScene[] updated = new EditorBuildSettingsScene[original.Length + 1];
            for (int i = 0; i < original.Length; i++) updated[i] = original[i];
            updated[original.Length] = new EditorBuildSettingsScene(path, true);
            EditorBuildSettings.scenes = updated;
            Debug.Log("[LUNARIS] Added " + path + " to Editor Build Settings.");
        }
    }
}
#endif
