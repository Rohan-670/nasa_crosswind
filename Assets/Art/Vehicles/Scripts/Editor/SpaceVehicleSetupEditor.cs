#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

namespace NASA.Crosswind.Vehicles.Editor
{
    public static class SpaceVehicleSetupEditor
    {
        private const string MODEL_PATH = "Assets/Art/Vehicles/NASA_SpaceExplorationVehicle.obj";
        private const string MAT_DIR = "Assets/Art/Vehicles/Materials";
        private const string PREFAB_PATH = "Assets/Art/Vehicles/Prefabs/NASA_SpaceExplorationVehicle.prefab";

        [MenuItem("NASA/Space Vehicle/Instantiate in Scene", false, 10)]
        public static void InstantiateVehicleInScene()
        {
            GameObject modelAsset = AssetDatabase.LoadAssetAtPath<GameObject>(MODEL_PATH);
            if (modelAsset == null)
            {
                EditorUtility.DisplayDialog("Error", "Could not find model at " + MODEL_PATH, "OK");
                return;
            }

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(modelAsset);
            instance.name = "NASA_SpaceExplorationVehicle";
            instance.transform.position = new Vector3(0, 0.5f, 0);

            ConfigureVehicleComponents(instance);

            // Register undo
            Undo.RegisterCreatedObjectUndo(instance, "Instantiate NASA Space Vehicle");
            Selection.activeGameObject = instance;
            SceneView.FrameLastActiveSceneView();

            Debug.Log("[NASA] Successfully instantiated NASA Space Exploration Vehicle in scene with complete physics, lighting, and controller!");
        }

        [MenuItem("NASA/Space Vehicle/Create or Update Prefab", false, 20)]
        public static void CreateOrUpdatePrefab()
        {
            GameObject modelAsset = AssetDatabase.LoadAssetAtPath<GameObject>(MODEL_PATH);
            if (modelAsset == null)
            {
                EditorUtility.DisplayDialog("Error", "Could not find model at " + MODEL_PATH, "OK");
                return;
            }

            GameObject tempInstance = Object.Instantiate(modelAsset);
            tempInstance.name = "NASA_SpaceExplorationVehicle";

            ConfigureVehicleComponents(tempInstance);

            // Ensure directory exists
            string prefabDir = Path.GetDirectoryName(PREFAB_PATH);
            if (!Directory.Exists(prefabDir))
            {
                Directory.CreateDirectory(prefabDir);
            }

            PrefabUtility.SaveAsPrefabAsset(tempInstance, PREFAB_PATH);
            Object.DestroyImmediate(tempInstance);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[NASA] Saved prefab asset to " + PREFAB_PATH);
            EditorUtility.DisplayDialog("Success", "NASA Space Exploration Vehicle prefab created at " + PREFAB_PATH, "OK");
        }

        public static void ConfigureVehicleComponents(GameObject root)
        {
            // 1. Assign Materials
            Material matHull = AssetDatabase.LoadAssetAtPath<Material>(MAT_DIR + "/M_SpaceVehicle_Hull.mat");
            Material matCockpit = AssetDatabase.LoadAssetAtPath<Material>(MAT_DIR + "/M_SpaceVehicle_Cockpit.mat");
            Material matChassis = AssetDatabase.LoadAssetAtPath<Material>(MAT_DIR + "/M_SpaceVehicle_Chassis.mat");
            Material matWheels = AssetDatabase.LoadAssetAtPath<Material>(MAT_DIR + "/M_SpaceVehicle_Wheels.mat");
            Material matSolar = AssetDatabase.LoadAssetAtPath<Material>(MAT_DIR + "/M_SpaceVehicle_SolarPanels.mat");
            Material matEmissive = AssetDatabase.LoadAssetAtPath<Material>(MAT_DIR + "/M_SpaceVehicle_Emissive.mat");

            MeshRenderer[] renderers = root.GetComponentsInChildren<MeshRenderer>(true);
            foreach (var r in renderers)
            {
                string n = r.gameObject.name;
                if (n.Contains("Cabin") || n.Contains("Antenna"))
                    r.sharedMaterial = matHull != null ? matHull : r.sharedMaterial;
                else if (n.Contains("Cockpit_Canopy"))
                    r.sharedMaterial = matCockpit != null ? matCockpit : r.sharedMaterial;
                else if (n.Contains("Wheel"))
                    r.sharedMaterial = matWheels != null ? matWheels : r.sharedMaterial;
                else if (n.Contains("Solar_Wing") && !n.Contains("Gimbal"))
                    r.sharedMaterial = matSolar != null ? matSolar : r.sharedMaterial;
                else if (n.Contains("Thruster") || n.Contains("Headlights"))
                    r.sharedMaterial = matEmissive != null ? matEmissive : r.sharedMaterial;
                else
                    r.sharedMaterial = matChassis != null ? matChassis : r.sharedMaterial;
            }

            // 2. Rigidbody
            Rigidbody rb = root.GetComponent<Rigidbody>();
            if (rb == null) rb = root.AddComponent<Rigidbody>();
            rb.mass = 1200f;
            rb.centerOfMass = new Vector3(0, 0.65f, -0.1f);
            rb.linearDamping = 0.5f;
            rb.angularDamping = 1.5f;

            // 3. Colliders
            // Cabin Box Collider
            BoxCollider cabinCol = root.GetComponent<BoxCollider>();
            if (cabinCol == null) cabinCol = root.AddComponent<BoxCollider>();
            cabinCol.center = new Vector3(0, 1.6f, -0.2f);
            cabinCol.size = new Vector3(2.3f, 1.4f, 3.2f);

            // Chassis Box Collider
            Transform chassis = root.transform.Find("Chassis_Frame");
            if (chassis != null)
            {
                BoxCollider chassisCol = chassis.GetComponent<BoxCollider>();
                if (chassisCol == null) chassisCol = chassis.gameObject.AddComponent<BoxCollider>();
                chassisCol.center = new Vector3(0, 0.7f, -0.1f);
                chassisCol.size = new Vector3(2.0f, 0.4f, 4.2f);
            }

            // Sphere / Wheel Colliders for the 6 Wheels
            string[] wheelNames = new string[] {
                "Wheel_Front_Left", "Wheel_Front_Right",
                "Wheel_Mid_Left",   "Wheel_Mid_Right",
                "Wheel_Rear_Left",  "Wheel_Rear_Right"
            };
            foreach (string wn in wheelNames)
            {
                Transform wt = root.transform.Find(wn);
                if (wt != null)
                {
                    SphereCollider sc = wt.GetComponent<SphereCollider>();
                    if (sc == null) sc = wt.gameObject.AddComponent<SphereCollider>();
                    sc.radius = 0.44f;
                }
            }

            // 4. Spotlights (Forward Headlights)
            Transform headlights = root.transform.Find("Headlights");
            if (headlights != null && headlights.childCount == 0)
            {
                GameObject lightL = new GameObject("Spotlight_Left");
                lightL.transform.SetParent(headlights);
                lightL.transform.localPosition = new Vector3(-0.42f, 2.18f, 1.35f);
                lightL.transform.localRotation = Quaternion.Euler(10f, -5f, 0f);
                Light l1 = lightL.AddComponent<Light>();
                l1.type = LightType.Spot;
                l1.range = 35f;
                l1.spotAngle = 60f;
                l1.color = new Color(1f, 0.98f, 0.9f);
                l1.intensity = 3f;

                GameObject lightR = new GameObject("Spotlight_Right");
                lightR.transform.SetParent(headlights);
                lightR.transform.localPosition = new Vector3(0.42f, 2.18f, 1.35f);
                lightR.transform.localRotation = Quaternion.Euler(10f, 5f, 0f);
                Light l2 = lightR.AddComponent<Light>();
                l2.type = LightType.Spot;
                l2.range = 35f;
                l2.spotAngle = 60f;
                l2.color = new Color(1f, 0.98f, 0.9f);
                l2.intensity = 3f;
            }

            // 5. Interior Cockpit Ambient Glow
            Transform cockpit = root.transform.Find("Cockpit_Canopy");
            if (cockpit != null && cockpit.childCount == 0)
            {
                GameObject intLight = new GameObject("Cockpit_Glow");
                intLight.transform.SetParent(cockpit);
                intLight.transform.localPosition = new Vector3(0, 1.6f, 1.5f);
                Light l = intLight.AddComponent<Light>();
                l.type = LightType.Point;
                l.range = 3.5f;
                l.color = new Color(0f, 0.85f, 1f); // Cyan sci-fi cockpit HUD glow
                l.intensity = 1.5f;
            }

            // 6. Controller script
            SpaceVehicleController controller = root.GetComponent<SpaceVehicleController>();
            if (controller == null) controller = root.AddComponent<SpaceVehicleController>();
            controller.AutoFindVehicleParts();
        }
    }
}
#endif
