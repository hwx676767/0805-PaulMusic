using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DeusEx.Rhythm
{
    /// <summary>
    /// 全屏程序化背景。把本组件加到空物体上即可：自动生成朝向相机的四边形并铺满屏幕。
    /// 可拖入任意背景材质（如 <c>HalftoneAtmosphere</c> 半调云雾远山，或旧版 <c>PlasmaBackground</c>）。
    /// </summary>
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class PlasmaBackground : MonoBehaviour
    {
        [Tooltip("背景材质。推荐 HalftoneAtmosphere（半调云雾/远山）；留空则回退到 DeusEx/PlasmaBackground。")]
        public Material backgroundMaterial;

        [Tooltip("作为背景参照的相机。留空则用 Camera.main 或场景中第一个相机。")]
        public Camera targetCamera;

        [Tooltip("背景放在相机前方多远（世界单位）。透视相机下会按此距离计算铺满尺寸。")]
        public float distance = 20f;

        [Tooltip("额外放大余量，>1 用来保证完全铺满边缘。")]
        public float padding = 1.1f;

        [Tooltip("运行时每帧自动贴合相机（会覆盖手动拖的位置）。手调位置时请关掉，或只改 Manual Offset。")]
        public bool fitEveryFrame = false;

        [Tooltip("编辑模式下是否也自动贴合。默认关闭，否则 Scene 里一拖就被拉回去。")]
        public bool autoFitInEditMode = false;

        [Tooltip("开启后：位置 = 相机前方 distance + Manual Offset。")]
        public bool useManualOffset = true;

        [Tooltip("相对相机默认铺满点的偏移。改这个即可挪背景，且不会被每帧清掉。")]
        public Vector3 manualOffset = Vector3.zero;

        [Tooltip("勾选后不再强制对齐相机朝向。")]
        public bool keepManualRotation = false;

        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private Material _runtimeMaterial;

        private void OnEnable()
        {
            EnsureSetup();
            if (ShouldAutoFit()) Fit(applyPosition: true);
            else Fit(applyPosition: false);
        }

        private void Update()
        {
            if (ShouldAutoFit()) Fit(applyPosition: true);
            else Fit(applyPosition: false); // 仍更新尺寸，不改位置
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!Application.isPlaying && useManualOffset && ShouldAutoFit())
                Fit(applyPosition: true);
        }
#endif

        private bool ShouldAutoFit()
        {
            if (Application.isPlaying) return fitEveryFrame;
            return autoFitInEditMode && fitEveryFrame;
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

        /// <summary>更新铺满尺寸；是否改写位置由 applyPosition / ShouldAutoFit 决定。</summary>
        public void Fit(bool applyPosition)
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

            if (applyPosition)
            {
                Vector3 fittedPos = cam.transform.position + cam.transform.forward * d;
                if (useManualOffset) fittedPos += manualOffset;
                transform.position = fittedPos;
            }

            if (!keepManualRotation)
                transform.rotation = cam.transform.rotation;

            transform.localScale = new Vector3(width * padding, height * padding, 1f);
        }

        [ContextMenu("Fit Now (apply position)")]
        public void FitNow() => Fit(applyPosition: true);

        [ContextMenu("Capture Offset From Current Position")]
        public void CaptureOffsetFromCurrentTransform()
        {
            Camera cam = ResolveCamera();
            if (cam == null) return;
            float d = Mathf.Clamp(distance, cam.nearClipPlane + 0.05f, cam.farClipPlane - 0.05f);
            Vector3 fittedPos = cam.transform.position + cam.transform.forward * d;
            manualOffset = transform.position - fittedPos;
            useManualOffset = true;
            fitEveryFrame = true; // 之后用 offset 跟随，不再丢你的偏移
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
                string[] guids = AssetDatabase.FindAssets("HalftoneAtmosphere t:Material");
                if (guids.Length == 0)
                    guids = AssetDatabase.FindAssets("PlasmaBackground t:Material");
                if (guids.Length > 0)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    backgroundMaterial = AssetDatabase.LoadAssetAtPath<Material>(path);
                }
            }
            EnsureSetup();
            Fit(applyPosition: true);
        }
#endif
    }
}
