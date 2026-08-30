using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DeusEx.Rhythm
{
    /// <summary>
    /// 乌云翻滚雾气：用多层透明平面 + <c>DeusEx/RollingStormFog</c> 噪声着色器。
    /// 适合大范围云墙涌动；比纯粒子更稳、更好调。粒子可另挂作碎雾点缀。
    /// </summary>
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public sealed class RollingStormFog : MonoBehaviour
    {
        [Header("引用")]
        [Tooltip("雾气材质（Shader = DeusEx/RollingStormFog）。留空则运行时自动创建。")]
        public Material fogMaterial;
        [Tooltip("参照相机。留空用 Main Camera。")]
        public Camera targetCamera;

        [Header("布局")]
        [Tooltip("雾层数量（建议 2～4）。")]
        [Range(1, 5)] public int layerCount = 3;
        [Tooltip("第一层相对相机的距离。")]
        public float nearDistance = 8f;
        [Tooltip("每层再往远处推多少。")]
        public float layerSpacing = 2.2f;
        [Tooltip("铺满屏幕的额外余量。")]
        public float padding = 1.25f;
        [Tooltip("每帧跟随相机自适应尺寸。")]
        public bool fitEveryFrame = true;

        [Header("观感覆盖（写入材质实例，不改共享资源）")]
        [Range(0f, 1.5f)] public float opacity = 1f;
        [Range(0f, 1.5f)] public float coverage = 0.72f;
        public Vector2 scrollSpeed = new Vector2(0.035f, 0.012f);
        [Range(0f, 1f)] public float billow = 0.18f;

        private Transform _layersRoot;
        private Mesh _quadMesh;
        private Material _sharedRuntime;
        private readonly Material[] _layerMats = new Material[5];
        private readonly MeshRenderer[] _renderers = new MeshRenderer[5];

        private void OnEnable()
        {
            EnsureBuilt();
            ApplyMaterialParams();
            Fit();
        }

        private void OnDisable()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                CleanupEditorTemps();
#endif
        }

        private void OnDestroy()
        {
            CleanupRuntime();
        }

        private void LateUpdate()
        {
            if (fitEveryFrame) Fit();
            ApplyMaterialParams();
        }

        private void EnsureBuilt()
        {
            if (_quadMesh == null) _quadMesh = BuildQuad();

            if (_layersRoot == null)
            {
                Transform existing = transform.Find("FogLayers");
                if (existing != null) _layersRoot = existing;
                else
                {
                    var go = new GameObject("FogLayers");
                    go.transform.SetParent(transform, false);
                    _layersRoot = go.transform;
                }
            }

            EnsureMaterialSource();

            int wanted = Mathf.Clamp(layerCount, 1, 5);
            for (int i = 0; i < 5; i++)
            {
                string name = $"FogLayer_{i}";
                Transform child = _layersRoot.Find(name);
                if (i >= wanted)
                {
                    if (child != null) child.gameObject.SetActive(false);
                    continue;
                }

                if (child == null)
                {
                    var go = new GameObject(name, typeof(MeshFilter), typeof(MeshRenderer));
                    go.transform.SetParent(_layersRoot, false);
                    child = go.transform;
                }

                child.gameObject.SetActive(true);
                var mf = child.GetComponent<MeshFilter>();
                var mr = child.GetComponent<MeshRenderer>();
                mf.sharedMesh = _quadMesh;

                if (_layerMats[i] == null)
                {
                    _layerMats[i] = new Material(_sharedRuntime != null ? _sharedRuntime : fogMaterial)
                    {
                        name = $"RollingStormFog Layer {i} (Instance)"
                    };
                }

                // 每层略不同：速度、覆盖、偏移，形成前后翻涌差
                float k = i / (float)Mathf.Max(1, wanted - 1);
                _layerMats[i].SetFloat("_LayerOffset", i * 1.37f);
                _layerMats[i].SetFloat("_NoiseScale", 1.35f + k * 0.9f);
                _layerMats[i].SetFloat("_WarpStrength", 0.4f + k * 0.35f);
                mr.sharedMaterial = _layerMats[i];
                mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                mr.receiveShadows = false;
                _renderers[i] = mr;
            }
        }

        private void EnsureMaterialSource()
        {
            if (fogMaterial != null)
            {
                _sharedRuntime = fogMaterial;
                return;
            }

            if (_sharedRuntime != null) return;

            Shader shader = Shader.Find("DeusEx/RollingStormFog");
            if (shader == null)
            {
                Debug.LogError("[RollingStormFog] 找不到 Shader DeusEx/RollingStormFog。");
                return;
            }

            _sharedRuntime = new Material(shader) { name = "RollingStormFog (Runtime)" };
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                string[] guids = AssetDatabase.FindAssets("RollingStormFog t:Material");
                if (guids != null && guids.Length > 0)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
                    if (mat != null)
                    {
                        fogMaterial = mat;
                        _sharedRuntime = mat;
                    }
                }
            }
#endif
        }

        private void ApplyMaterialParams()
        {
            int wanted = Mathf.Clamp(layerCount, 1, 5);
            for (int i = 0; i < wanted; i++)
            {
                Material m = _layerMats[i];
                if (m == null) continue;
                float k = i / (float)Mathf.Max(1, wanted - 1);
                m.SetFloat("_Opacity", opacity * Mathf.Lerp(0.85f, 1.1f, k));
                m.SetFloat("_Coverage", coverage * Mathf.Lerp(1.05f, 0.9f, k));
                m.SetFloat("_Billow", billow);
                m.SetVector("_Scroll", new Vector4(
                    scrollSpeed.x * Mathf.Lerp(1.15f, 0.65f, k),
                    scrollSpeed.y * Mathf.Lerp(1.0f, 0.7f, k),
                    0f, 0f));
            }
        }

        private void Fit()
        {
            Camera cam = ResolveCamera();
            if (cam == null || _layersRoot == null) return;

            int wanted = Mathf.Clamp(layerCount, 1, 5);
            for (int i = 0; i < wanted; i++)
            {
                Transform child = _layersRoot.Find($"FogLayer_{i}");
                if (child == null) continue;

                float dist = nearDistance + i * layerSpacing;
                Vector3 pos = cam.transform.position + cam.transform.forward * dist;
                child.position = pos;
                child.rotation = Quaternion.LookRotation(cam.transform.forward, cam.transform.up);

                float height;
                float width;
                if (cam.orthographic)
                {
                    height = cam.orthographicSize * 2f * padding;
                    width = height * cam.aspect;
                }
                else
                {
                    height = 2f * dist * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad) * padding;
                    width = height * cam.aspect;
                }

                child.localScale = new Vector3(width, height, 1f);
            }
        }

        private Camera ResolveCamera()
        {
            if (targetCamera != null) return targetCamera;
            if (Camera.main != null) return Camera.main;
            return FindObjectOfType<Camera>();
        }

        private static Mesh BuildQuad()
        {
            var mesh = new Mesh { name = "RollingStormFogQuad" };
            mesh.vertices = new[]
            {
                new Vector3(-0.5f, -0.5f, 0f),
                new Vector3(0.5f, -0.5f, 0f),
                new Vector3(-0.5f, 0.5f, 0f),
                new Vector3(0.5f, 0.5f, 0f),
            };
            mesh.uv = new[]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(1, 1),
            };
            mesh.triangles = new[] { 0, 2, 1, 2, 3, 1 };
            mesh.RecalculateBounds();
            return mesh;
        }

        private void CleanupRuntime()
        {
            for (int i = 0; i < _layerMats.Length; i++)
            {
                if (_layerMats[i] == null) continue;
                if (Application.isPlaying) Destroy(_layerMats[i]);
                else DestroyImmediate(_layerMats[i]);
                _layerMats[i] = null;
            }

            if (_sharedRuntime != null && fogMaterial == null)
            {
                if (Application.isPlaying) Destroy(_sharedRuntime);
                else DestroyImmediate(_sharedRuntime);
            }
            _sharedRuntime = null;

            if (_quadMesh != null)
            {
                if (Application.isPlaying) Destroy(_quadMesh);
                else DestroyImmediate(_quadMesh);
                _quadMesh = null;
            }
        }

#if UNITY_EDITOR
        private void CleanupEditorTemps() { }
#endif
    }
}
