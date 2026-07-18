using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DeusEx.Rhythm
{
    /// <summary>
    /// 程序化蓝色流动等离子背景。把这个组件加到一个空物体上即可：
    /// 它会自动生成一块朝向相机、铺满整屏的四边形，并贴上 DeusEx/PlasmaBackground 材质，
    /// 还会随相机的 FOV / 宽高比自动调整大小，保证任何分辨率下都铺满。
    /// 无需视频文件、无水印、完美无缝循环。
    /// </summary>
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class PlasmaBackground : MonoBehaviour
    {
        [Tooltip("等离子背景材质。留空时会自动用 DeusEx/PlasmaBackground 着色器即时创建。")]
        public Material backgroundMaterial;

        [Tooltip("作为背景参照的相机。留空则用 Camera.main 或场景中第一个相机。")]
        public Camera targetCamera;

        [Tooltip("背景放在相机前方多远（世界单位）。透视相机下会按此距离计算铺满尺寸。")]
        public float distance = 20f;

        [Tooltip("额外放大余量，>1 用来保证完全铺满边缘。")]
        public float padding = 1.1f;

        [Tooltip("每帧跟随相机自适应（相机会移动/改 FOV 时建议开启）。")]
        public bool fitEveryFrame = true;

        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private Material _runtimeMaterial;

        private void OnEnable()
        {
            EnsureSetup();
            Fit();
        }

        private void Update()
        {
            if (fitEveryFrame) Fit();
        }

        private void EnsureSetup()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();

            if (_meshFilter.sharedMesh == null)
                _meshFilter.sharedMesh = BuildQuad();

            Material mat = ResolveMaterial();
            if (mat != null && _meshRenderer.sharedMaterial != mat)
                _meshRenderer.sharedMaterial = mat;

            _meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            _meshRenderer.receiveShadows = false;
            _meshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            _meshRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            _meshRenderer.allowOcclusionWhenDynamic = false;
        }

        private Material ResolveMaterial()
        {
            if (backgroundMaterial != null) return backgroundMaterial;
            if (_runtimeMaterial != null) return _runtimeMaterial;

            Shader shader = Shader.Find("DeusEx/PlasmaBackground");
            if (shader == null) return null;

            _runtimeMaterial = new Material(shader) { name = "PlasmaBackground (Runtime)" };
            return _runtimeMaterial;
        }

        private Camera ResolveCamera()
        {
            if (targetCamera != null) return targetCamera;
            if (Camera.main != null) return Camera.main;
#if UNITY_2023_1_OR_NEWER
            return Object.FindFirstObjectByType<Camera>();
#else
            return Camera.current;
#endif
        }

        private void Fit()
        {
            Camera cam = ResolveCamera();
            if (cam == null) return;

            float d = Mathf.Clamp(distance, cam.nearClipPlane + 0.05f, cam.farClipPlane - 0.05f);

            float height, width;
            if (cam.orthographic)
            {
                height = cam.orthographicSize * 2f;
                width = height * cam.aspect;
            }
            else
            {
                height = 2f * d * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
                width = height * cam.aspect;
            }

            transform.position = cam.transform.position + cam.transform.forward * d;
            transform.rotation = cam.transform.rotation;
            transform.localScale = new Vector3(width * padding, height * padding, 1f);
        }

        private static Mesh BuildQuad()
        {
            Mesh mesh = new Mesh { name = "PlasmaBackgroundQuad" };
            mesh.vertices = new[]
            {
                new Vector3(-0.5f, -0.5f, 0f),
                new Vector3( 0.5f, -0.5f, 0f),
                new Vector3(-0.5f,  0.5f, 0f),
                new Vector3( 0.5f,  0.5f, 0f),
            };
            mesh.uv = new[]
            {
                new Vector2(0f, 0f), new Vector2(1f, 0f),
                new Vector2(0f, 1f), new Vector2(1f, 1f),
            };
            mesh.triangles = new[] { 0, 2, 1, 2, 3, 1 };
            mesh.RecalculateBounds();
            return mesh;
        }

#if UNITY_EDITOR
        private void Reset()
        {
            // 在编辑器里添加组件时，自动找到 PlasmaBackground.mat 并填入材质槽
            if (backgroundMaterial == null)
            {
                string[] guids = AssetDatabase.FindAssets("PlasmaBackground t:Material");
                if (guids.Length > 0)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    backgroundMaterial = AssetDatabase.LoadAssetAtPath<Material>(path);
                }
            }
            EnsureSetup();
            Fit();
        }
#endif
    }
}
