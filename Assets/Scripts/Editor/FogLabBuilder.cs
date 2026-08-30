using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DeusEx.Rhythm.EditorTools
{
    /// <summary>
    /// 在 Assets/Scenes/fog/Fog.unity 里搭好乌云翻滚雾气试验台（不动 SampleScene）。
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

            // 深色天空，方便看雾
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.08f, 0.09f, 0.12f, 1f);

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
            fog.layerCount = 3;
            fog.nearDistance = 6f;
            fog.layerSpacing = 2.5f;
            fog.opacity = 1.05f;
            fog.coverage = 0.78f;
            fog.scrollSpeed = new Vector2(0.028f, 0.01f);
            fog.billow = 0.22f;

            // 简单地面参考，方便判断景深
            EnsureGroundPlane();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Selection.activeGameObject = fogGo;
            Debug.Log("[FogLab] 已写入 RollingStormFog。打开 Fog 场景 Play 即可调参；满意后再迁到 SampleScene。", fogGo);
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

            mat.SetColor("_FogColorDeep", new Color(0.04f, 0.05f, 0.08f, 0.9f));
            mat.SetColor("_FogColorMid", new Color(0.12f, 0.14f, 0.2f, 0.6f));
            mat.SetColor("_FogColorEdge", new Color(0.32f, 0.35f, 0.42f, 0.25f));
            mat.SetFloat("_Coverage", 0.78f);
            mat.SetFloat("_Density", 1.4f);
            mat.SetFloat("_Contrast", 1.5f);
            mat.SetFloat("_Opacity", 1.05f);
            mat.SetFloat("_NoiseScale", 1.6f);
            mat.SetFloat("_WarpStrength", 0.58f);
            mat.SetVector("_Scroll", new Vector4(0.028f, 0.01f, 0f, 0f));
            mat.SetFloat("_Billow", 0.22f);
            mat.SetFloat("_VerticalBias", 0.3f);
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
                    gmat.color = new Color(0.12f, 0.13f, 0.15f, 1f);
                    renderer.sharedMaterial = gmat;
                }
            }
        }
    }
}
