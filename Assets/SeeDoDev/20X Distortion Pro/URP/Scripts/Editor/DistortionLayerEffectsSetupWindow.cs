#if DistortionsX_HAS_URP
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

namespace DistortionsPro_20X
{
    public sealed class DistortionLayerEffectsSetupWindow : EditorWindow
    {
        const int RenderTextureWidth = 1024;
        const int RenderTextureHeight = 1024;

        [SerializeField] Camera sourceCamera;
        [SerializeField] Volume mainVolume;
        [SerializeField] int targetLayer = 5;

        [SerializeField] GameObject lastRoot;
        [SerializeField] Camera lastCreatedCamera;
        [SerializeField] RenderTexture lastCreatedRenderTexture;
        [SerializeField] GameObject lastCreatedUIVolume;
        [SerializeField] VolumeProfile lastCreatedLayerVolumeProfile;
        [SerializeField] VolumeProfile lastUsedMainVolumeProfile;
        [SerializeField] UnityEngine.Object lastAutoResolutionComponent;

        Vector2 scroll;
        string lastStatus;
        MessageType lastStatusType = MessageType.Info;
        [SerializeField] bool showDebugSection = false;

        [MenuItem("Window/20X Distortion Pro/Layer Effects Setup")]
        static void Open()
        {
            DistortionLayerEffectsSetupWindow window = GetWindow<DistortionLayerEffectsSetupWindow>("Distortion Layer Setup");
            window.minSize = new Vector2(700f, 720f);
            window.Show();
        }

        void OnEnable()
        {
            if (sourceCamera == null)
                sourceCamera = Camera.main;

            if (mainVolume == null)
                mainVolume = FindMainVolumeInScene();
        }

        void OnDisable()
        {
            EditorApplication.delayCall -= DelayedCreateOrUpdateSetup;
        }

        void OnGUI()
        {
            scroll = EditorGUILayout.BeginScrollView(scroll);

            DrawSetupSection();
            DrawActionSection();
            DrawNextStepsSection();
            DrawDebugSection();

            EditorGUILayout.EndScrollView();
        }
        void DrawDebugSection()
        {
            EditorGUILayout.BeginVertical("box");

            showDebugSection = EditorGUILayout.Foldout(
                showDebugSection,
                "Debug Section",
                true,
                new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold });

            if (showDebugSection)
            {
                GUILayout.Space(4f);

                DrawLastCreatedSection();

                if (!string.IsNullOrEmpty(lastStatus))
                    EditorGUILayout.HelpBox(lastStatus, lastStatusType);
            }

            EditorGUILayout.EndVertical();
            GUILayout.Space(6f);
        }

        void DrawSetupSection()
        {
            EditorGUILayout.BeginVertical("box");
            DrawSectionHeader("Setup", new Color(0.18f, 0.24f, 0.18f));

            sourceCamera = (Camera)EditorGUILayout.ObjectField(
                new GUIContent("Source Camera", "Used for initial transform and basic camera values."),
                sourceCamera,
                typeof(Camera),
                true);

            EditorGUILayout.BeginHorizontal();
            mainVolume = (Volume)EditorGUILayout.ObjectField(
                new GUIContent("Main Volume", "Must already have a saved sharedProfile asset."),
                mainVolume,
                typeof(Volume),
                true);

            using (new GUIBackgroundColorScope(new Color(0.25f, 0.55f, 0.95f)))
            {
                if (GUILayout.Button("Find", GUILayout.Width(70f)))
                    mainVolume = FindMainVolumeInScene();
            }
            EditorGUILayout.EndHorizontal();

            targetLayer = EditorGUILayout.LayerField(
                new GUIContent("Target Layer", "Layer rendered into the created Render Texture."),
                targetLayer);

            EditorGUILayout.EndVertical();
            GUILayout.Space(6f);
        }

        void DrawActionSection()
        {
            bool requestCreate = false;

            EditorGUILayout.BeginVertical("box");
            //DrawSectionHeader("Actions", new Color(0.15f, 0.17f, 0.26f));

            using (new GUIBackgroundColorScope(new Color(0.20f, 0.62f, 0.98f)))
            {
                if (GUILayout.Button("Create / Update Setup", GUILayout.Height(36f)))
                    requestCreate = true;
            }



            EditorGUILayout.EndVertical();
            GUILayout.Space(6f);

            if (requestCreate)
                RequestCreateOrUpdateSetup();
        }

        void DrawNextStepsSection()
        {
            EditorGUILayout.BeginVertical("box");
            DrawSectionHeader("Next Steps", new Color(0.26f, 0.16f, 0.16f));

            GUIStyle textStyle = new GUIStyle(EditorStyles.wordWrappedLabel)
            {
                richText = true,
                fontSize = 13,
                alignment = TextAnchor.UpperLeft,
                padding = new RectOffset(12, 12, 10, 10)
            };

            string text =
                "<b>1.</b> Open the created UI Volume and add any 20X Distortion Pro effects you want into its profile\n\n" +
                "<b>2.</b> New Layer Volume Profile and Layer Render Texture are created in that same folder as your Main Volume\n\n" +
                "<b>4.</b> Main Volume is configured automatically with <b>Distortion Custom Texture</b> and the created Render Texture";

            float height = textStyle.CalcHeight(new GUIContent(text), EditorGUIUtility.currentViewWidth - 70f);
            Rect rect = GUILayoutUtility.GetRect(1f, height + 8f, GUILayout.ExpandWidth(true));

            EditorGUI.DrawRect(rect, new Color(0.30f, 0.20f, 0.20f));
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, 5f, rect.height), new Color(0.95f, 0.55f, 0.20f));

            GUI.Label(new Rect(rect.x + 8f, rect.y + 4f, rect.width - 12f, rect.height - 8f), text, textStyle);

            EditorGUILayout.EndVertical();
        }

        void DrawLastCreatedSection()
        {
            if (lastRoot == null &&
                lastCreatedCamera == null &&
                lastCreatedRenderTexture == null &&
                lastCreatedUIVolume == null &&
                lastCreatedLayerVolumeProfile == null &&
                lastUsedMainVolumeProfile == null &&
                lastAutoResolutionComponent == null)
                return;

            DrawSectionHeader("Current Objects", new Color(0.18f, 0.18f, 0.18f));

            if (lastCreatedCamera != null)
                EditorGUILayout.ObjectField("RT Camera", lastCreatedCamera, typeof(Camera), true);

            if (lastCreatedRenderTexture != null)
                EditorGUILayout.ObjectField("Render Texture", lastCreatedRenderTexture, typeof(RenderTexture), false);

            if (lastCreatedUIVolume != null)
                EditorGUILayout.ObjectField("UI Volume", lastCreatedUIVolume, typeof(GameObject), true);

            if (lastCreatedLayerVolumeProfile != null)
                EditorGUILayout.ObjectField("Layer Volume Profile", lastCreatedLayerVolumeProfile, typeof(VolumeProfile), false);

            if (lastUsedMainVolumeProfile != null)
                EditorGUILayout.ObjectField("Main Volume Profile", lastUsedMainVolumeProfile, typeof(VolumeProfile), false);

            if (lastAutoResolutionComponent != null)
                EditorGUILayout.ObjectField("Auto Resolution Component", lastAutoResolutionComponent, typeof(UnityEngine.Object), true);

            GUILayout.Space(6f);
        }

        void RequestCreateOrUpdateSetup()
        {
            EditorApplication.delayCall -= DelayedCreateOrUpdateSetup;
            EditorApplication.delayCall += DelayedCreateOrUpdateSetup;
            GUIUtility.ExitGUI();
        }

        void DelayedCreateOrUpdateSetup()
        {
            EditorApplication.delayCall -= DelayedCreateOrUpdateSetup;
            CreateOrUpdateSetup();
            Repaint();
        }

        void CreateOrUpdateSetup()
        {
            int undoGroup = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName("GLP Layer Effects Setup");

            try
            {
                if (mainVolume == null)
                    mainVolume = FindMainVolumeInScene();

                if (mainVolume == null)
                {
                    lastStatus = "Error: Main Volume is required.";
                    lastStatusType = MessageType.Error;
                    return;
                }

                if (mainVolume.sharedProfile == null || !AssetDatabase.Contains(mainVolume.sharedProfile))
                {
                    lastStatus = "Error: Main Volume must have an existing saved sharedProfile asset.";
                    lastStatusType = MessageType.Error;
                    return;
                }

                string folderPath = GetMainVolumeProfileFolder(mainVolume.sharedProfile);
                if (string.IsNullOrEmpty(folderPath) || !AssetDatabase.IsValidFolder(folderPath))
                {
                    lastStatus = "Error: Could not resolve the folder of Main Volume sharedProfile.";
                    lastStatusType = MessageType.Error;
                    return;
                }

                string layerName = GetLayerDisplayName(targetLayer);
                string safeLayerName = SanitizeFileName(layerName);

                string rootName = $"{layerName} Layer FX Setup";
                string cameraName = $"{layerName} RT Camera";
                string uiVolumeName = $"{layerName} UI Volume";
                string rtAssetName = $"{safeLayerName} Render Texture";
                string layerProfileName = $"{safeLayerName} UI Volume Profile";

                bool rootCreated;
                bool cameraCreated;
                bool volumeCreated;
                bool rtCreated;
                bool layerProfileCreated;

                GameObject root = FindOrCreateRoot(rootName, out rootCreated);
                GameObject camGO = FindOrCreateChild(root.transform, cameraName, out cameraCreated);
                GameObject volumeGO = FindOrCreateChild(root.transform, uiVolumeName, out volumeCreated);

                Camera cam = GetOrAddComponent<Camera>(camGO);
                Volume uiVolume = GetOrAddComponent<Volume>(volumeGO);

                RenderTexture rt = LoadOrCreateRenderTexture(folderPath, rtAssetName, RenderTextureWidth, RenderTextureHeight, out rtCreated);
                VolumeProfile layerProfile = LoadOrCreateVolumeProfile(folderPath, layerProfileName, out layerProfileCreated);

                SetupCamera(camGO, cam, rt);
                SetupLayerVolume(volumeGO, uiVolume, layerProfile);

                string urpMessage = ConfigureUrpAdditionalCameraData(camGO, targetLayer);
                UnityEngine.Object autoRes = AddOrUpdateRenderTextureAutoResolution(camGO, rt, out string autoResMessage);
                string mainVolumeMessage = ConfigureMainVolume(mainVolume, rt, out VolumeProfile usedMainProfile);

                lastRoot = root;
                lastCreatedCamera = cam;
                lastCreatedRenderTexture = rt;
                lastCreatedUIVolume = volumeGO;
                lastCreatedLayerVolumeProfile = layerProfile;
                lastUsedMainVolumeProfile = usedMainProfile;
                lastAutoResolutionComponent = autoRes;

                Selection.activeObject = volumeGO;

                AssetDatabase.SaveAssets();

                if (root.scene.IsValid())
                    EditorSceneManager.MarkSceneDirty(root.scene);

                if (mainVolume.gameObject.scene.IsValid())
                    EditorSceneManager.MarkSceneDirty(mainVolume.gameObject.scene);

                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"Layer: {layerName}");
                sb.AppendLine($"Assets folder: {folderPath}");
                sb.AppendLine(rootCreated ? $"Root created: {root.name}" : $"Root reused: {root.name}");
                sb.AppendLine(cameraCreated ? $"RT Camera created: {camGO.name}" : $"RT Camera reused: {camGO.name}");
                sb.AppendLine(volumeCreated ? $"UI Volume created: {volumeGO.name}" : $"UI Volume reused: {volumeGO.name}");
                sb.AppendLine(rtCreated ? $"Render Texture created: {rt.name}" : $"Render Texture reused: {rt.name}");
                sb.AppendLine(layerProfileCreated ? $"Layer Volume Profile created: {layerProfile.name}" : $"Layer Volume Profile reused: {layerProfile.name}");

                if (!string.IsNullOrEmpty(urpMessage))
                    sb.AppendLine(urpMessage);

                if (!string.IsNullOrEmpty(autoResMessage))
                    sb.AppendLine(autoResMessage);

                if (!string.IsNullOrEmpty(mainVolumeMessage))
                    sb.AppendLine(mainVolumeMessage);

                lastStatus = sb.ToString().TrimEnd();
                lastStatusType = MessageType.Info;
            }
            catch (Exception ex)
            {
                lastStatus = "Setup creation/update failed:\n" + ex.Message;
                lastStatusType = MessageType.Error;
            }
            finally
            {
                Undo.CollapseUndoOperations(undoGroup);
            }
        }

        void SetupCamera(GameObject camGO, Camera cam, RenderTexture rt)
        {
            Camera srcCam = sourceCamera != null ? sourceCamera : Camera.main;

            if (srcCam != null)
            {
                camGO.transform.position = srcCam.transform.position;
                camGO.transform.rotation = srcCam.transform.rotation;

                cam.nearClipPlane = srcCam.nearClipPlane;
                cam.farClipPlane = srcCam.farClipPlane;
                cam.fieldOfView = srcCam.fieldOfView;
                cam.orthographic = srcCam.orthographic;
                cam.orthographicSize = srcCam.orthographicSize;
                cam.allowHDR = srcCam.allowHDR;
                cam.allowMSAA = srcCam.allowMSAA;
            }

            cam.targetTexture = rt;
            cam.cullingMask = 1 << targetLayer;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0f, 0f, 0f, 0f);
        }

        void SetupLayerVolume(GameObject volumeGO, Volume uiVolume, VolumeProfile layerProfile)
        {
            volumeGO.layer = targetLayer;
            volumeGO.transform.localPosition = Vector3.zero;
            volumeGO.transform.localRotation = Quaternion.identity;
            volumeGO.transform.localScale = Vector3.one;

            uiVolume.isGlobal = true;
            uiVolume.priority = 100f;
            uiVolume.weight = 1f;
            uiVolume.sharedProfile = layerProfile;

            EditorUtility.SetDirty(uiVolume);
        }

        static void DrawSectionHeader(string title, Color color)
        {
            Rect headerRect = GUILayoutUtility.GetRect(1f, 24f, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(headerRect, color);

            GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.MiddleLeft,
                normal = { textColor = Color.white }
            };

            GUI.Label(new Rect(headerRect.x + 8f, headerRect.y + 3f, headerRect.width - 16f, 18f), title, titleStyle);
            GUILayout.Space(4f);
        }

        static Volume FindMainVolumeInScene()
        {
            Volume[] volumes = FindObjectsByType<Volume>(FindObjectsSortMode.None);
            if (volumes == null || volumes.Length == 0)
                return null;

            Volume namedMain = volumes.FirstOrDefault(v =>
                v != null &&
                v.isGlobal &&
                v.gameObject.name.IndexOf("main", StringComparison.OrdinalIgnoreCase) >= 0);

            if (namedMain != null)
                return namedMain;

            return volumes.FirstOrDefault(v => v != null && v.isGlobal);
        }

        static string GetMainVolumeProfileFolder(VolumeProfile profile)
        {
            if (profile == null)
                return null;

            string assetPath = AssetDatabase.GetAssetPath(profile);
            if (string.IsNullOrEmpty(assetPath))
                return null;

            string folder = Path.GetDirectoryName(assetPath);
            if (string.IsNullOrEmpty(folder))
                return null;

            return folder.Replace("\\", "/");
        }

        static GameObject FindOrCreateRoot(string rootName, out bool created)
        {
            GameObject root = FindSceneObjectExact(rootName);
            if (root != null)
            {
                created = false;
                return root;
            }

            root = new GameObject(rootName);
            Undo.RegisterCreatedObjectUndo(root, $"Create {rootName}");
            created = true;
            return root;
        }

        static GameObject FindOrCreateChild(Transform parent, string childName, out bool created)
        {
            Transform child = parent.Find(childName);
            if (child != null)
            {
                created = false;
                return child.gameObject;
            }

            GameObject go = new GameObject(childName);
            Undo.RegisterCreatedObjectUndo(go, $"Create {childName}");
            Undo.SetTransformParent(go.transform, parent, $"Parent {childName}");
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
            created = true;
            return go;
        }

        static GameObject FindSceneObjectExact(string name)
        {
            GameObject[] all = FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            return all.FirstOrDefault(go => go != null && go.name == name);
        }

        static T GetOrAddComponent<T>(GameObject go) where T : Component
        {
            T component = go.GetComponent<T>();
            if (component == null)
                component = Undo.AddComponent<T>(go);
            return component;
        }

        static RenderTexture LoadOrCreateRenderTexture(string folderPath, string assetName, int width, int height, out bool created)
        {
            string assetPath = CombineAssetPath(folderPath, SanitizeFileName(assetName) + ".renderTexture");
            RenderTexture rt = AssetDatabase.LoadAssetAtPath<RenderTexture>(assetPath);

            if (rt == null)
            {
                rt = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32)
                {
                    name = assetName,
                    antiAliasing = 1,
                    filterMode = FilterMode.Bilinear,
                    wrapMode = TextureWrapMode.Clamp,
                    useMipMap = false,
                    autoGenerateMips = false
                };

                TryForceDepthStencilFormat(rt);
                AssetDatabase.CreateAsset(rt, assetPath);
                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
                rt = AssetDatabase.LoadAssetAtPath<RenderTexture>(assetPath);
                created = true;
                return rt;
            }

            rt.name = assetName;
            rt.Release();
            rt.width = width;
            rt.height = height;
            rt.antiAliasing = 1;
            rt.filterMode = FilterMode.Bilinear;
            rt.wrapMode = TextureWrapMode.Clamp;
            rt.useMipMap = false;
            rt.autoGenerateMips = false;
            TryForceDepthStencilFormat(rt);
            EditorUtility.SetDirty(rt);

            created = false;
            return rt;
        }

        static VolumeProfile LoadOrCreateVolumeProfile(string folderPath, string profileName, out bool created)
        {
            string assetPath = CombineAssetPath(folderPath, SanitizeFileName(profileName) + ".asset");
            VolumeProfile profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(assetPath);

            if (profile != null)
            {
                created = false;
                return profile;
            }

            profile = ScriptableObject.CreateInstance<VolumeProfile>();
            profile.name = profileName;
            AssetDatabase.CreateAsset(profile, assetPath);
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);

            created = true;
            return AssetDatabase.LoadAssetAtPath<VolumeProfile>(assetPath);
        }

        static string ConfigureMainVolume(Volume volume, RenderTexture renderTexture, out VolumeProfile usedProfile)
        {
            usedProfile = null;

            if (volume == null)
                return "Main Volume was not found.";

            VolumeProfile profile = volume.sharedProfile;
            usedProfile = profile;

            if (profile == null || !AssetDatabase.Contains(profile))
                return "Main Volume must have an existing saved sharedProfile asset.";


            CustomTexture_20X effect = profile.components.OfType<CustomTexture_20X>().FirstOrDefault();
            bool created = false;

            if (effect == null)
            {
                effect = profile.Add<CustomTexture_20X>(true);
                created = true;
            }

            if (effect == null)
                return "Distortion Custom Texture override could not be added.";

              effect.texture.overrideState = true;
            effect.texture.value = renderTexture;

            effect.fade.overrideState = true;
            effect.fade.value = 1f;

            effect.alpha.overrideState = true;
            effect.alpha.value = 1f;

            effect.GlobalPostProcessingSettings.overrideState = true;
            effect.GlobalPostProcessingSettings.value = false;

            EditorUtility.SetDirty(effect);
            EditorUtility.SetDirty(profile);
            EditorUtility.SetDirty(volume);

            return created
                ? $"Main Volume profile: {profile.name}. Distortion Custom Texture override added and texture assigned."
                : $"Main Volume profile: {profile.name}. Distortion Custom Texture override updated and texture assigned.";

        }

        static void TryForceDepthStencilFormat(RenderTexture rt)
        {
            if (rt == null)
                return;

            try
            {
                PropertyInfo prop = typeof(RenderTexture).GetProperty("depthStencilFormat", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (prop == null || !prop.CanWrite)
                    return;

                Type enumType = prop.PropertyType;
                string[] names = Enum.GetNames(enumType);

                string wanted =
                    names.Contains("D24_UNorm_S8_UInt") ? "D24_UNorm_S8_UInt" :
                    names.Contains("D32_SFloat_S8_UInt") ? "D32_SFloat_S8_UInt" :
                    names.Contains("D16_UNorm") ? "D16_UNorm" :
                    null;

                if (wanted != null)
                    prop.SetValue(rt, Enum.Parse(enumType, wanted), null);
            }
            catch
            {
            }
        }

        static string ConfigureUrpAdditionalCameraData(GameObject cameraObject, int layer)
        {
            Type urpType = FindTypeByName("UnityEngine.Rendering.Universal.UniversalAdditionalCameraData");
            if (urpType == null)
                return "URP Additional Camera Data not found.";

            Component urpData = cameraObject.GetComponent(urpType);
            if (urpData == null)
                urpData = Undo.AddComponent(cameraObject, urpType);

            LayerMask volumeMask = 1 << layer;

            bool volumeMaskSet = TrySetMemberValue(urpData, "volumeLayerMask", volumeMask) ||
                                 TrySetMemberValue(urpData, "m_VolumeLayerMask", volumeMask);

            bool postEnabled = TrySetMemberValue(urpData, "renderPostProcessing", true) ||
                               TrySetMemberValue(urpData, "m_RenderPostProcessing", true);

            return $"URP camera data: Volume Mask {(volumeMaskSet ? "set" : "not set")}, Post Processing {(postEnabled ? "enabled" : "not enabled")}.";
        }

        static UnityEngine.Object AddOrUpdateRenderTextureAutoResolution(GameObject cameraObject, RenderTexture rt, out string message)
        {
            message = "RenderTextureAutoResolution_20X not found.";

            Type type = FindTypeByName("RenderTextureAutoResolution_20X");
            if (type == null)
                return null;

            Component comp = cameraObject.GetComponent(type);
            bool created = false;

            if (comp == null)
            {
                comp = Undo.AddComponent(cameraObject, type);
                created = true;
            }

            bool assigned = TryAssignRenderTexture(comp, rt);

            if (assigned)
                message = created
                    ? "RenderTextureAutoResolution_20X added and texture assigned."
                    : "RenderTextureAutoResolution_20X reused and texture assigned.";
            else
                message = created
                    ? "RenderTextureAutoResolution_20X added, but texture field was not assigned automatically."
                    : "RenderTextureAutoResolution_20X reused, but texture field was not assigned automatically.";

            return comp;
        }

        static bool TryAssignRenderTexture(Component target, RenderTexture rt)
        {
            if (target == null || rt == null)
                return false;

            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            FieldInfo field = target.GetType()
                .GetFields(flags)
                .FirstOrDefault(f => typeof(RenderTexture).IsAssignableFrom(f.FieldType));

            if (field != null)
            {
                Undo.RecordObject(target, "Assign Render Texture");
                field.SetValue(target, rt);
                EditorUtility.SetDirty(target);
                return true;
            }

            PropertyInfo prop = target.GetType()
                .GetProperties(flags)
                .FirstOrDefault(p => p.CanWrite && typeof(RenderTexture).IsAssignableFrom(p.PropertyType));

            if (prop != null)
            {
                Undo.RecordObject(target, "Assign Render Texture");
                prop.SetValue(target, rt, null);
                EditorUtility.SetDirty(target);
                return true;
            }

            return false;
        }

        static bool TrySetMemberValue(object target, string memberName, object value)
        {
            if (target == null)
                return false;

            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            Type type = target.GetType();

            FieldInfo field = type.GetField(memberName, flags);
            if (field != null)
            {
                try
                {
                    field.SetValue(target, ConvertIfNeeded(field.FieldType, value));
                    return true;
                }
                catch
                {
                }
            }

            PropertyInfo prop = type.GetProperty(memberName, flags);
            if (prop != null && prop.CanWrite)
            {
                try
                {
                    prop.SetValue(target, ConvertIfNeeded(prop.PropertyType, value), null);
                    return true;
                }
                catch
                {
                }
            }

            return false;
        }

        static object ConvertIfNeeded(Type targetType, object value)
        {
            if (value == null)
                return null;

            if (targetType.IsAssignableFrom(value.GetType()))
                return value;

            if (targetType == typeof(LayerMask) && value is int maskInt)
            {
                LayerMask mask = maskInt;
                return mask;
            }

            if (targetType == typeof(int) && value is LayerMask maskValue)
                return maskValue.value;

            return value;
        }

        static Type FindTypeByName(string fullNameOrName)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    Type exact = assembly.GetType(fullNameOrName, false);
                    if (exact != null)
                        return exact;

                    Type byName = assembly.GetTypes().FirstOrDefault(t =>
                        t != null &&
                        (t.FullName == fullNameOrName || t.Name == fullNameOrName));

                    if (byName != null)
                        return byName;
                }
                catch
                {
                }
            }

            return null;
        }

        static string CombineAssetPath(string folderPath, string fileName)
        {
            return (folderPath.TrimEnd('/') + "/" + fileName).Replace("\\", "/");
        }

        static string GetLayerDisplayName(int layer)
        {
            string layerName = LayerMask.LayerToName(layer);
            return string.IsNullOrEmpty(layerName) ? $"Layer {layer}" : layerName;
        }

        static string SanitizeFileName(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "Unnamed";

            foreach (char c in Path.GetInvalidFileNameChars())
                value = value.Replace(c.ToString(), "_");

            return value.Trim();
        }

        sealed class GUIBackgroundColorScope : IDisposable
        {
            readonly Color oldColor;

            public GUIBackgroundColorScope(Color color)
            {
                oldColor = GUI.backgroundColor;
                GUI.backgroundColor = color;
            }

            public void Dispose()
            {
                GUI.backgroundColor = oldColor;
            }
        }
    }
}
#endif
