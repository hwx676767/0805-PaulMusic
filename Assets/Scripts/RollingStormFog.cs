using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DeusEx.Rhythm
{
    /// <summary>
    /// 厚重灰云雾：从点 / 线源向外涌散，在固定体积内翻滚。
    /// </summary>
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public sealed class RollingStormFog : MonoBehaviour
    {
        public enum EmitMode
        {
            Point = 0,
            HorizontalLine = 1,
            VerticalLine = 2,
        }

        [Header("引用")]
        public Material fogMaterial;
        public Camera targetCamera;

        [Header("体积范围")]
        [Tooltip("开启后用固定宽高体积（推荐）。")]
        public bool useFixedVolume = true;
        public Vector3 volumeOffset = new Vector3(0f, 0.15f, 7f);
        public Vector2 volumeSize = new Vector2(11f, 7.5f);
        [Range(1, 5)] public int layerCount = 3;
        public float nearDistance = 6f;
        public float layerSpacing = 0.45f;
        public float padding = 1.15f;
        public bool fitEveryFrame = true;

        [Header("发射源（点 / 线 → 向外涌）")]
        public EmitMode emitMode = EmitMode.Point;
        [Tooltip("源在体积 UV 上的位置（0~1）。点源默认靠下中；水平线用 Y；垂直线用 X。")]
        public Vector2 emitOriginUV = new Vector2(0.5f, 0.12f);
        [Range(0.01f, 0.4f)] public float sourceRadius = 0.1f;
        [Range(0.15f, 1.5f)] public float emitReach = 0.92f;
        [Range(0f, 2.5f)] public float emitStrength = 1.5f;
        [Range(0.3f, 4f)] public float emitFalloff = 1.05f;
        [Tooltip("向外涌散速度。")]
        public float expandSpeed = 0.2f;
        [Range(0f, 1.5f)] public float billow = 0.35f;
        [Range(0f, 2f)] public float swirl = 0.5f;

        [Header("观感（厚重云雾）")]
        [Range(0f, 2.5f)] public float opacity = 1.9f;
        [Range(0.1f, 1.4f)] public float coverage = 0.95f;
        [Range(0.5f, 8f)] public float density = 2.35f;
        [Range(0f, 1f)] public float haze = 0.6f;
        public Color denseGray = new Color(0.22f, 0.23f, 0.25f, 1f);
        public Color midGray = new Color(0.38f, 0.39f, 0.42f, 0.92f);
        public Color edgeGray = new Color(0.52f, 0.53f, 0.56f, 0.55f);

        private Transform _layersRoot;
        private Mesh _quadMesh;
        private Material _sharedRuntime;
        private readonly Material[] _layerMats = new Material[5];

        private void OnEnable()
        {
            EnsureBuilt();
            ApplyMaterialParams();
            Fit();
        }

        private void OnDestroy() => CleanupRuntime();

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
                child.GetComponent<MeshFilter>().sharedMesh = _quadMesh;
                var mr = child.GetComponent<MeshRenderer>();

                if (_layerMats[i] == null)
                {
                    Material src = _sharedRuntime != null ? _sharedRuntime : fogMaterial;
                    if (src == null) continue;
                    _layerMats[i] = new Material(src) { name = $"HeavyCloud Layer {i}" };
                }

                float k = wanted <= 1 ? 0f : i / (float)(wanted - 1);
                _layerMats[i].SetFloat("_LayerOffset", i * 1.73f);
                _layerMats[i].SetFloat("_NoiseScale", 1.35f + k * 0.85f);
                _layerMats[i].SetFloat("_WarpStrength", 0.7f + k * 0.3f);
                mr.sharedMaterial = _layerMats[i];
                mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                mr.receiveShadows = false;
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
                    var mat = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(guids[0]));
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
                float k = wanted <= 1 ? 0f : i / (float)(wanted - 1);

                m.SetColor("_FogColorDeep", denseGray);
                m.SetColor("_FogColorMid", midGray);
                m.SetColor("_FogColorEdge", edgeGray);
                m.SetFloat("_Opacity", opacity * Mathf.Lerp(1.05f, 0.88f, k));
                m.SetFloat("_Coverage", coverage);
                m.SetFloat("_Density", density);
                m.SetFloat("_Haze", haze * Mathf.Lerp(1.05f, 0.85f, k));
                m.SetFloat("_Billow", billow);
                m.SetFloat("_ExpandSpeed", expandSpeed * Mathf.Lerp(1.1f, 0.75f, k));
                m.SetFloat("_AngularTwist", swirl);
                m.SetFloat("_EmitMode", (float)emitMode);
                m.SetVector("_EmitOrigin", new Vector4(emitOriginUV.x, emitOriginUV.y, 0f, 0f));
                m.SetFloat("_EmitRadius", sourceRadius);
                m.SetFloat("_EmitReach", emitReach);
                m.SetFloat("_EmitStrength", emitStrength);
                m.SetFloat("_EmitFalloff", emitFalloff);
                m.SetFloat("_BoundsSoftness", 0.14f);
                m.SetFloat("_BoundsInset", 0.02f);
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

                if (useFixedVolume)
                {
                    Vector3 center = cam.transform.TransformPoint(volumeOffset + Vector3.forward * (i * layerSpacing));
                    child.position = center;
                    child.rotation = Quaternion.LookRotation(cam.transform.forward, cam.transform.up);
                    child.localScale = new Vector3(volumeSize.x, volumeSize.y, 1f);
                }
                else
                {
                    float dist = nearDistance + i * layerSpacing;
                    child.position = cam.transform.position + cam.transform.forward * dist;
                    child.rotation = Quaternion.LookRotation(cam.transform.forward, cam.transform.up);
                    float height = cam.orthographic
                        ? cam.orthographicSize * 2f * padding
                        : 2f * dist * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad) * padding;
                    child.localScale = new Vector3(height * cam.aspect, height, 1f);
                }
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
        private void OnDrawGizmosSelected()
        {
            if (!useFixedVolume) return;
            Camera cam = ResolveCamera();
            if (cam == null) return;

            Vector3 center = cam.transform.TransformPoint(volumeOffset);
            Vector3 right = cam.transform.right * volumeSize.x * 0.5f;
            Vector3 up = cam.transform.up * volumeSize.y * 0.5f;
            Gizmos.color = new Color(0.65f, 0.65f, 0.7f, 0.9f);
            Vector3 a = center - right - up;
            Vector3 b = center + right - up;
            Vector3 c = center + right + up;
            Vector3 d = center - right + up;
            Gizmos.DrawLine(a, b); Gizmos.DrawLine(b, c); Gizmos.DrawLine(c, d); Gizmos.DrawLine(d, a);

            // 源：点 / 线
            Vector3 origin = center
                + cam.transform.right * ((emitOriginUV.x - 0.5f) * volumeSize.x)
                + cam.transform.up * ((emitOriginUV.y - 0.5f) * volumeSize.y);
            Gizmos.color = Color.yellow;
            if (emitMode == EmitMode.Point)
            {
                Gizmos.DrawWireSphere(origin, Mathf.Max(0.15f, sourceRadius * volumeSize.y));
                Gizmos.DrawRay(origin, cam.transform.up * volumeSize.y * 0.35f);
                Gizmos.DrawRay(origin, cam.transform.right * volumeSize.x * 0.2f);
                Gizmos.DrawRay(origin, -cam.transform.right * volumeSize.x * 0.2f);
            }
            else if (emitMode == EmitMode.HorizontalLine)
            {
                Vector3 l = origin - right;
                Vector3 r = origin + right;
                Gizmos.DrawLine(l, r);
                Gizmos.DrawRay(origin, cam.transform.up * volumeSize.y * 0.4f);
                Gizmos.DrawRay(origin, -cam.transform.up * volumeSize.y * 0.15f);
            }
            else
            {
                Vector3 bot = origin - up;
                Vector3 top = origin + up;
                Gizmos.DrawLine(bot, top);
                Gizmos.DrawRay(origin, cam.transform.right * volumeSize.x * 0.35f);
                Gizmos.DrawRay(origin, -cam.transform.right * volumeSize.x * 0.35f);
            }
        }
#endif
    }
}
