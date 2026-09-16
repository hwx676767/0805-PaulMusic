using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;

namespace InkCloud.Editor
{
    public static class InkCloudSetup
    {
        private const string Root = "Assets/InkCloudBackground";
        [MenuItem("Tools/Ink Cloud/Create Demo Scene")]
        public static void CreateDemo()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            Shader shader = AssetDatabase.LoadAssetAtPath<Shader>(Root+"/InkCloud.shader");
            if (!shader || ShaderUtil.ShaderHasError(shader))
            {
                Debug.LogError("InkCloud shader is missing or has compilation errors.");
                return;
            }
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
            var cam = new GameObject("Main Camera",typeof(Camera)).GetComponent<Camera>();
            cam.tag = "MainCamera";
            cam.orthographic = true;
            cam.orthographicSize = 5;
            cam.transform.position = new Vector3(0,0,-10);
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Color.white;
            cam.allowHDR = false;
            Material material = AssetDatabase.LoadAssetAtPath<Material>(Root+"/InkCloud.mat");
            if (!material)
            {
                material = new Material(shader);
                AssetDatabase.CreateAsset(material,Root+"/InkCloud.mat");
            }
            var go = GameObject.CreatePrimitive(PrimitiveType.Quad);
            go.name = "Ink Cloud Background";
            Object.DestroyImmediate(go.GetComponent<Collider>());
            var renderer = go.GetComponent<MeshRenderer>();
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            renderer.receiveShadows = false;
            var effect = go.AddComponent<InkCloudBackground>();
            // Prefab resolves MainCamera on import; no demo-camera reference is serialized.
            effect.distance = 50;
            PrefabUtility.SaveAsPrefabAsset(go,Root+"/InkCloudBackground.prefab");
            EditorSceneManager.SaveScene(scene,Root+"/InkCloudDemo.unity");
            AssetDatabase.SaveAssets();
            Selection.activeGameObject = go;
            EditorApplication.ExecuteMenuItem("Window/General/Game");
            Debug.Log("InkCloud demo ready. Press Play to preview the animated background.");
        }
        [MenuItem("Tools/Ink Cloud/Export Package")]
        public static void Export()
        {
            string destination = EditorUtility.SaveFilePanel("Export Ink Cloud", "", "InkCloudBackground", "unitypackage");
            if (string.IsNullOrEmpty(destination)) return;
            #pragma warning disable CS0618 // Retain compatibility with earlier Unity 6 editors.
            AssetDatabase.ExportPackage(Root,destination,ExportPackageOptions.Recurse);
            #pragma warning restore CS0618
            Debug.Log("InkCloud exported: "+destination);
        }
        [MenuItem("Tools/Ink Cloud/Save Preview PNG")]
        public static void SavePreview()
        {
            if (!Application.isPlaying)
            {
                Debug.Log("Enter Play mode before saving the preview.");
                return;
            }
            string destination = EditorUtility.SaveFilePanel("Save Cloud Preview", "", "InkCloudPreview", "png");
            if (!string.IsNullOrEmpty(destination)) ScreenCapture.CaptureScreenshot(destination);
        }
    }
}
