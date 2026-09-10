using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DeusEx.Rhythm.EditorTools
{
    /// <summary>
    /// 在 Assets/Scenes/fog/Fog.unity 里搭好厚重云雾试验台（不动 SampleScene）。
    /// </summary>
    internal static class FogLabBuilder
    {
        private const string ScenePath = "Assets/Scenes/fog/Fog.unity";
        private const string MatPath = "Assets/Scripts/Shaders/RollingStormFog.mat";
        private const string ShaderPath = "Assets/Scripts/Shaders/RollingStormFog.shader";

        [MenuItem("Tools/DeusEx/Setup Fog Lab Scene", false, 55)]
        public static void SetupFogLab()
        {
            EnsureMaterial();

            Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            Camera cam = Object.FindFirstObjectByType<Camera>();
            if (cam == null)
                throw new System.InvalidOperationException("Fog 场景里找不到 Camera。");

            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.45f, 0.48f, 0.52f, 1f);

            RollingStormFog existing = Object.FindFirstObjectByType<RollingStormFog>();
            GameObject fogGo;
            if (existing != null) fogGo = existing.gameObject;
            else
            {
                fogGo = new GameObject("RollingStormFog", typeof(RollingStormFog));
                Undo.RegisterCreatedObjectUndo(fogGo, "Create RollingStormFog");
            }

            var fog = fogGo.GetComponent<RollingStormFog>();
            fog.targetCamera = cam;
            fog.fogMaterial = AssetDatabase.LoadAssetAtPath<Material>(MatPath);
            fog.useFixedVolume = true;
            fog.volumeOffset = new Vector3(0f, 0.15f, 7f);
            fog.volumeSize = new Vector2(11f, 7.5f);
            fog.layerCount = 3;
            fog.layerSpacing = 0.45f;
            fog.emitMode = RollingStormFog.EmitMode.Point;
            fog.emitOriginUV = new Vector2(0.5f, 0.12f);
            fog.sourceRadius = 0.1f;
            fog.emitReach = 0.92f;
            fog.emitStrength = 1.5f;
            fog.emitFalloff = 1.05f;
            fog.expandSpeed = 0.2f;
            fog.billow = 0.35f;
            fog.swirl = 0.5f;
            fog.opacity = 1.9f;
            fog.coverage = 0.95f;
            fog.density = 2.35f;
            fog.haze = 0.6f;
            fog.denseGray = new Color(0.22f, 0.23f, 0.25f, 1f);
            fog.midGray = new Color(0.38f, 0.39f, 0.42f, 0.92f);
            fog.edgeGray = new Color(0.52f, 0.53f, 0.56f, 0.55f);

            EnsureGroundPlane();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Selection.activeGameObject = fogGo;
            Debug.Log("[FogLab] 点源厚重云雾已就绪。可把 Emit Mode 改成 HorizontalLine 试线源。", fogGo);
        }

        private static void EnsureMaterial()
        {
            var mat = AssetDatabase.LoadAssetAtPath<Material>(MatPath);
            Shader shader = AssetDatabase.LoadAssetAtPath<Shader>(ShaderPath);
            if (shader == null) shader = Shader.Find("DeusEx/RollingStormFog");
            if (shader == null)
                throw new System.InvalidOperationException("找不到 RollingStormFog.shader，请先等 Unity 编译。");

            if (mat == null)
            {
                mat = new Material(shader) { name = "RollingStormFog" };
                AssetDatabase.CreateAsset(mat, MatPath);
            }
            else
            {
                mat.shader = shader;
                EditorUtility.SetDirty(mat);
            }

            mat.SetColor("_FogColorDeep", new Color(0.22f, 0.23f, 0.25f, 1f));
            mat.SetColor("_FogColorMid", new Color(0.38f, 0.39f, 0.42f, 0.92f));
            mat.SetColor("_FogColorEdge", new Color(0.52f, 0.53f, 0.56f, 0.55f));
            mat.SetFloat("_Coverage", 0.95f);
            mat.SetFloat("_Density", 2.35f);
            mat.SetFloat("_Contrast", 0.85f);
            mat.SetFloat("_Opacity", 1.9f);
            mat.SetFloat("_Haze", 0.6f);
            mat.SetFloat("_NoiseScale", 1.55f);
            mat.SetFloat("_WarpStrength", 0.85f);
            mat.SetFloat("_ExpandSpeed", 0.2f);
            mat.SetFloat("_Billow", 0.35f);
            mat.SetFloat("_AngularTwist", 0.5f);
            mat.SetFloat("_EmitMode", 0f);
            mat.SetVector("_EmitOrigin", new Vector4(0.5f, 0.12f, 0f, 0f));
            mat.SetFloat("_EmitRadius", 0.1f);
            mat.SetFloat("_EmitReach", 0.92f);
            mat.SetFloat("_EmitStrength", 1.5f);
            mat.SetFloat("_EmitFalloff", 1.05f);
            EditorUtility.SetDirty(mat);
            AssetDatabase.SaveAssets();
        }

        private static void EnsureGroundPlane()
        {
            GameObject ground = GameObject.Find("FogLabGround");
            if (ground == null)
            {
                ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
                ground.name = "FogLabGround";
                Undo.RegisterCreatedObjectUndo(ground, "Create FogLabGround");
            }

            ground.transform.position = new Vector3(0f, -1.2f, 8f);
            ground.transform.localScale = new Vector3(3f, 1f, 3f);
            var renderer = ground.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                Shader lit = Shader.Find("Universal Render Pipeline/Lit");
                if (lit != null)
                {
                    var gmat = new Material(lit);
                    gmat.color = new Color(0.18f, 0.19f, 0.2f, 1f);
                    renderer.sharedMaterial = gmat;
                }
            }
        }
    }
}
