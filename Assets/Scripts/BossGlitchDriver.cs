using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DeusEx.Rhythm
{
    /// <summary>
    /// Boss 实体 glitch 方块：稀疏、偶发闪现。
    /// 位置由 Inspector 的 Center/Size（相对 Boss 精灵 extents）驱动——改数值会立刻改绿色框与方块。
    /// Scene 里用绿色手柄拖，也会写回这些数值。
    /// </summary>
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class BossGlitchDriver : MonoBehaviour
    {
        [Tooltip("留空则用 BossGlitch.mat / 运行时创建。")]
        public Material glitchMaterial;
        [Tooltip("留空则用 BossGlitchPanel.mat / 运行时创建。")]
        public Material panelMaterial;

        [Header("腿部雾")]
        [Range(0f, 1f)] public float legTop = 0.42f;
        [Range(0f, 2f)] public float mistStrength = 1.05f;
        [Range(0f, 1f)] public float hideFeet = 0.85f;
        [Range(0f, 0.35f)] public float bodyGlitch = 0.12f;

        [Header("前区（角色前 / 左下）— 改这些数值即可调位置")]
        [Tooltip("相对精灵中心，单位 = 半宽/半高（-1~1 大致在体内）。")]
        public Vector2 frontCenter = new Vector2(-0.35f, -0.45f);
        [Tooltip("相对精灵全宽/全高（0.2~0.6 较合适）。")]
        public Vector2 frontSize = new Vector2(0.42f, 0.38f);
        [Range(1, 6)] public int frontMaxBlocks = 4;

        [Header("后区（角色后 / 右上）")]
        public Vector2 backCenter = new Vector2(0.38f, 0.42f);
        public Vector2 backSize = new Vector2(0.32f, 0.28f);
        [Range(0, 4)] public int backMaxBlocks = 2;

        [Header("偶发闪现")]
        [Tooltip("每次闪现最少/最多同时出现的方块数。")]
        [Range(1, 6)] public int burstMin = 2;
        [Range(1, 6)] public int burstMax = 5;
        [Tooltip("方块可见时长（秒）。")]
        public Vector2 burstVisibleSeconds = new Vector2(0.35f, 0.85f);
        [Tooltip("两次闪现之间的等待（秒）。")]
        public Vector2 burstGapSeconds = new Vector2(1.8f, 4.2f);

        [Header("块大小（相对区域框 0~1）")]
        [Range(0.15f, 0.9f)] public float minSizeFrac = 0.28f;
        [Range(0.2f, 1f)] public float maxSizeFrac = 0.55f;
        [Range(0f, 1f)] public float squareBias = 0.55f;
        [Range(0.2f, 1f)] public float panelOpacity = 0.72f;
        public bool drawZoneGizmos = true;

        // 运行时区域（由 Center/Size 驱动，勿手拖指望它“持久”，请改上方数值或用 Scene 手柄）
        [HideInInspector] public Transform frontZone;
        [HideInInspector] public Transform backZone;

        private SpriteRenderer _sr;
        private Material _instance;
        private Material _original;
        private Material _panelSource;
        private readonly List<PanelItem> _pool = new List<PanelItem>();
        private float _phaseTimer;
        private bool _burstActive;
        private static Mesh _quadMesh;

        private struct PanelItem
        {
            public Transform transform;
            public MeshRenderer renderer;
            public Material material;
            public float seed;
            public float phase;
            public bool front;
        }

        private void Awake()
        {
            if (Application.isPlaying) Apply();
        }

        private void OnEnable()
        {
            ApplyZonesFromFields();
            if (!Application.isPlaying) return;

            Apply();
            EnsurePool();
            HideAllPanels();
            _burstActive = false;
            _phaseTimer = Random.Range(burstGapSeconds.x, burstGapSeconds.y) * 0.35f;
        }

        private void OnDisable()
        {
            if (!Application.isPlaying) return;
            if (_sr != null && _original != null)
                _sr.sharedMaterial = _original;
        }

        private void OnDestroy()
        {
            if (_instance != null)
            {
                if (Application.isPlaying) Destroy(_instance);
                else DestroyImmediate(_instance);
            }
            ClearPool();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            frontMaxBlocks = Mathf.Clamp(frontMaxBlocks, 1, 6);
            backMaxBlocks = Mathf.Clamp(backMaxBlocks, 0, 4);
            burstMin = Mathf.Clamp(burstMin, 1, 6);
            burstMax = Mathf.Max(burstMin, Mathf.Clamp(burstMax, 1, 6));
            frontSize.x = Mathf.Max(0.05f, frontSize.x);
            frontSize.y = Mathf.Max(0.05f, frontSize.y);
            backSize.x = Mathf.Max(0.05f, backSize.x);
            backSize.y = Mathf.Max(0.05f, backSize.y);
            if (!Application.isPlaying)
                ApplyZonesFromFields();
        }
#endif

        private void LateUpdate()
        {
            // 编辑器与运行时都把数值推到区域框，保证「改数值 = 改位置」
            ApplyZonesFromFields();

            if (!Application.isPlaying) return;

            if (_instance != null)
            {
                _instance.SetFloat("_LegTop", legTop);
                _instance.SetFloat("_MistStrength", mistStrength);
                _instance.SetFloat("_HideFeet", hideFeet);
                _instance.SetFloat("_BodyGlitch", bodyGlitch);
                _instance.SetFloat("_DataStrength", 0.12f);
                _instance.SetFloat("_RectChance", 0.04f);
                _instance.SetFloat("_ScanChance", 0.1f);
                _instance.SetFloat("_SliceStrength", 0.006f);
            }

            TickBurst(Time.unscaledDeltaTime);

            if (_burstActive)
            {
                float t = Time.unscaledTime;
                for (int i = 0; i < _pool.Count; i++)
                {
                    if (_pool[i].material == null || _pool[i].renderer == null || !_pool[i].renderer.enabled)
                        continue;
                    float baseOp = panelOpacity * (_pool[i].front ? 0.9f : 0.5f);
                    _pool[i].material.SetFloat("_Opacity",
                        baseOp * (0.9f + 0.1f * Mathf.Sin(t * 3.5f + _pool[i].phase)));
                }
            }
        }

        [ContextMenu("Reapply Glitch Material")]
        public void Apply()
        {
            _sr = GetComponent<SpriteRenderer>();
            if (_sr == null) return;
            if (_original == null) _original = _sr.sharedMaterial;

            Material source = ResolveBossMaterial();
            if (source != null)
            {
                if (_instance == null)
                    _instance = new Material(source) { name = source.name + " (Boss Instance)" };
                _sr.material = _instance;
            }
            _panelSource = ResolvePanelMaterial();
        }

        /// <summary>把 Inspector 的 Center/Size 立刻写到区域框（Scene 手柄与运行时都靠它）。</summary>
        public void ApplyZonesFromFields()
        {
            if (_sr == null) _sr = GetComponent<SpriteRenderer>();
            frontZone = ResolveOrCreateZone(frontZone, "GlitchZone_Front");
            backZone = ResolveOrCreateZone(backZone, "GlitchZone_Back");
            PlaceZone(frontZone, frontCenter, frontSize, -0.02f);
            PlaceZone(backZone, backCenter, backSize, 0.05f);
        }

        /// <summary>从当前区域框 Transform 读回 Center/Size（Scene 拖动手柄后调用）。</summary>
        public void CaptureFieldsFromZones()
        {
            if (_sr == null) _sr = GetComponent<SpriteRenderer>();
            if (_sr == null || _sr.sprite == null) return;
            Bounds sb = _sr.sprite.bounds;
            if (frontZone != null)
                ReadZone(frontZone, sb, out frontCenter, out frontSize);
            if (backZone != null)
                ReadZone(backZone, sb, out backCenter, out backSize);
        }

        private static void ReadZone(Transform zone, Bounds sb, out Vector2 center, out Vector2 size)
        {
            float ex = Mathf.Max(1e-4f, sb.extents.x);
            float ey = Mathf.Max(1e-4f, sb.extents.y);
            center = new Vector2(
                (zone.localPosition.x - sb.center.x) / ex,
                (zone.localPosition.y - sb.center.y) / ey);
            size = new Vector2(
                Mathf.Max(0.05f, zone.localScale.x / Mathf.Max(1e-4f, sb.size.x)),
                Mathf.Max(0.05f, zone.localScale.y / Mathf.Max(1e-4f, sb.size.y)));
        }

        private Transform ResolveOrCreateZone(Transform current, string name)
        {
            if (current == null)
            {
                var found = transform.Find(name);
                if (found != null) current = found;
            }

            if (current == null)
            {
                var go = new GameObject(name);
                go.transform.SetParent(transform, false);
                current = go.transform;
            }

            EnsureZonePicker(current);
            return current;
        }

        private static void EnsureZonePicker(Transform zone)
        {
            if (zone == null) return;
            var box = zone.GetComponent<BoxCollider>();
            if (box == null) box = zone.gameObject.AddComponent<BoxCollider>();
            box.size = Vector3.one;
            box.center = Vector3.zero;
            box.isTrigger = true;
        }

        private void PlaceZone(Transform zone, Vector2 center, Vector2 size, float z)
        {
            if (zone == null || _sr == null || _sr.sprite == null) return;
            Bounds sb = _sr.sprite.bounds;
            zone.localPosition = new Vector3(
                sb.center.x + center.x * sb.extents.x,
                sb.center.y + center.y * sb.extents.y,
                z);
            zone.localRotation = Quaternion.identity;
            zone.localScale = new Vector3(
                Mathf.Max(0.05f, size.x * sb.size.x),
                Mathf.Max(0.05f, size.y * sb.size.y),
                1f);
        }

        private void EnsurePool()
        {
            int need = Mathf.Max(frontMaxBlocks + backMaxBlocks, burstMax);
            while (_pool.Count < need)
            {
                var go = new GameObject($"GlitchPanel_{_pool.Count}");
                go.transform.SetParent(transform, false);
                go.AddComponent<MeshFilter>().sharedMesh = QuadMesh();
                var mr = go.AddComponent<MeshRenderer>();
                mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                mr.receiveShadows = false;
                mr.enabled = false;

                Material mat = _panelSource != null
                    ? new Material(_panelSource)
                    : new Material(Shader.Find("DeusEx/BossGlitchPanel") ?? Shader.Find("Sprites/Default"));
                mat.name = "BossGlitchPanel (Instance)";
                mr.sharedMaterial = mat;

                _pool.Add(new PanelItem
                {
                    transform = go.transform,
                    renderer = mr,
                    material = mat,
                    seed = Random.value * 100f,
                    phase = Random.value * Mathf.PI * 2f,
                });
            }
        }

        private void TickBurst(float dt)
        {
            _phaseTimer -= dt;
            if (_phaseTimer > 0f) return;

            if (_burstActive)
            {
                HideAllPanels();
                _burstActive = false;
                _phaseTimer = Random.Range(burstGapSeconds.x, burstGapSeconds.y);
                return;
            }

            StartBurst();
            _burstActive = true;
            _phaseTimer = Random.Range(burstVisibleSeconds.x, burstVisibleSeconds.y);
        }

        private void StartBurst()
        {
            EnsurePool();
            ApplyZonesFromFields();
            HideAllPanels();

            int totalWanted = Random.Range(burstMin, burstMax + 1);
            int frontWanted = Mathf.Min(frontMaxBlocks, Mathf.Max(1, totalWanted - Random.Range(0, backMaxBlocks + 1)));
            if (backMaxBlocks <= 0) frontWanted = Mathf.Min(frontMaxBlocks, totalWanted);
            int backWanted = Mathf.Min(backMaxBlocks, Mathf.Max(0, totalWanted - frontWanted));
            if (frontWanted + backWanted < totalWanted && frontWanted < frontMaxBlocks)
                frontWanted = Mathf.Min(frontMaxBlocks, totalWanted - backWanted);

            int used = 0;
            used = PlaceBurstSubset(used, frontWanted, frontZone, front: true);
            PlaceBurstSubset(used, backWanted, backZone, front: false);
        }

        private int PlaceBurstSubset(int poolStart, int count, Transform zone, bool front)
        {
            if (zone == null || count <= 0) return poolStart;
            if (_sr == null) return poolStart;

            int order = _sr.sortingOrder;
            string layer = _sr.sortingLayerName;

            for (int i = 0; i < count; i++)
            {
                int idx = poolStart + i;
                if (idx >= _pool.Count) break;
                var item = _pool[idx];
                if (item.transform == null) continue;

                item.transform.SetParent(zone, false);
                LayoutOne(ref item, front);
                if (item.renderer != null)
                {
                    item.renderer.enabled = true;
                    item.renderer.sortingLayerName = layer;
                    item.renderer.sortingOrder = front ? order + 2 + i : order - 2 - i;
                }
                item.front = front;
                _pool[idx] = item;
            }
            return poolStart + count;
        }

        private void LayoutOne(ref PanelItem item, bool front)
        {
            float w = Random.Range(minSizeFrac, maxSizeFrac);
            float h = Random.Range(minSizeFrac, maxSizeFrac);
            if (Random.value < squareBias)
            {
                float side = Mathf.Max(w, h);
                w = side;
                h = side * Random.Range(0.88f, 1.05f);
            }
            else if (!front)
            {
                w = Mathf.Max(w, h * Random.Range(1.05f, 1.35f));
            }

            w = Mathf.Min(w, 0.85f);
            h = Mathf.Min(h, 0.85f);
            float halfW = w * 0.5f;
            float halfH = h * 0.5f;
            float x = Random.Range(-0.5f + halfW, 0.5f - halfW);
            float y = Random.Range(-0.5f + halfH, 0.5f - halfH);

            item.transform.localPosition = new Vector3(x, y, 0f);
            item.transform.localRotation = Quaternion.identity;
            item.transform.localScale = new Vector3(w, h, 1f);

            if (item.material != null)
            {
                item.seed = Random.value * 100f;
                item.material.SetFloat("_Seed", item.seed);
                item.material.SetFloat("_Opacity", panelOpacity * (front ? 0.9f : 0.5f));
                item.material.SetFloat("_ShowAccent", Random.value < 0.25f ? 1f : 0f);
                item.material.SetColor("_Color", front
                    ? new Color(0.38f, 0.66f, 0.72f, 0.82f)
                    : new Color(0.48f, 0.62f, 0.70f, 0.45f));
            }
        }

        private void HideAllPanels()
        {
            for (int i = 0; i < _pool.Count; i++)
            {
                if (_pool[i].renderer != null)
                    _pool[i].renderer.enabled = false;
            }
        }

        private void ClearPool()
        {
            for (int i = 0; i < _pool.Count; i++)
            {
                if (_pool[i].material != null)
                {
                    if (Application.isPlaying) Destroy(_pool[i].material);
                    else DestroyImmediate(_pool[i].material);
                }
                if (_pool[i].transform != null)
                {
                    if (Application.isPlaying) Destroy(_pool[i].transform.gameObject);
                    else DestroyImmediate(_pool[i].transform.gameObject);
                }
            }
            _pool.Clear();
        }

        private void OnDrawGizmos()
        {
            if (!drawZoneGizmos) return;
            ApplyZonesFromFields();
            DrawZoneGizmo(frontZone, new Color(0.15f, 1f, 0.25f, 0.2f));
            DrawZoneGizmo(backZone, new Color(0.2f, 0.9f, 0.45f, 0.16f));
        }

        private void OnDrawGizmosSelected()
        {
            DrawZoneGizmo(frontZone, new Color(0.2f, 1f, 0.3f, 0.4f));
            DrawZoneGizmo(backZone, new Color(0.25f, 0.95f, 0.45f, 0.35f));
        }

        private static void DrawZoneGizmo(Transform zone, Color color)
        {
            if (zone == null) return;
            Gizmos.color = color;
            Gizmos.matrix = zone.localToWorldMatrix;
            Gizmos.DrawCube(Vector3.zero, Vector3.one);
            Gizmos.color = new Color(color.r, color.g, color.b, 1f);
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
            Gizmos.matrix = Matrix4x4.identity;
        }

        private Material ResolveBossMaterial()
        {
            if (glitchMaterial != null) return glitchMaterial;
#if UNITY_EDITOR
            glitchMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Scripts/Shaders/BossGlitch.mat");
            if (glitchMaterial != null) return glitchMaterial;
#endif
            Shader shader = Shader.Find("DeusEx/BossGlitch");
            return shader == null ? null : (glitchMaterial = new Material(shader) { name = "BossGlitch (Runtime)" });
        }

        private Material ResolvePanelMaterial()
        {
            if (panelMaterial != null) return panelMaterial;
#if UNITY_EDITOR
            panelMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Scripts/Shaders/BossGlitchPanel.mat");
            if (panelMaterial != null) return panelMaterial;
#endif
            Shader shader = Shader.Find("DeusEx/BossGlitchPanel");
            return shader == null ? null : (panelMaterial = new Material(shader) { name = "BossGlitchPanel (Runtime)" });
        }

        private static Mesh QuadMesh()
        {
            if (_quadMesh != null) return _quadMesh;
            _quadMesh = new Mesh
            {
                name = "BossGlitchPanelQuad",
                vertices = new[]
                {
                    new Vector3(-0.5f, -0.5f, 0f), new Vector3(0.5f, -0.5f, 0f),
                    new Vector3(-0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0f),
                },
                uv = new[]
                {
                    new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1),
                },
                triangles = new[] { 0, 2, 1, 2, 3, 1 },
            };
            _quadMesh.RecalculateBounds();
            return _quadMesh;
        }
    }
}
