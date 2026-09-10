using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DeusEx.Rhythm
{
    /// <summary>
    /// Boss glitch：保留腿部青雾材质，并在左下（角色前）/ 右上（角色后）生成实体数据矩形。
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class BossGlitchDriver : MonoBehaviour
    {
        [Tooltip("留空则用 Assets/Scripts/Shaders/BossGlitch.mat 或运行时创建。")]
        public Material glitchMaterial;

        [Tooltip("实体面板材质。留空则用 BossGlitchPanel / 运行时创建。")]
        public Material panelMaterial;

        [Header("腿部雾（写到 Boss 材质）")]
        [Range(0f, 1f)] public float legTop = 0.42f;
        [Range(0f, 2f)] public float mistStrength = 1.05f;
        [Range(0f, 1f)] public float hideFeet = 0.85f;
        [Range(0f, 0.35f)] public float bodyGlitch = 0.12f;

        [Header("实体方块数量")]
        [Min(1)] public int frontBlockCount = 7;
        [Min(0)] public int backBlockCount = 3;

        [Header("左下角（角色前面）")]
        public Vector2 frontAnchor = new Vector2(-0.28f, -0.32f);
        public Vector2 frontJitter = new Vector2(0.34f, 0.28f);
        public Vector2 frontSizeMin = new Vector2(0.55f, 0.35f);
        public Vector2 frontSizeMax = new Vector2(1.85f, 1.55f);

        [Header("右上角（角色后面）")]
        public Vector2 backAnchor = new Vector2(0.32f, 0.28f);
        public Vector2 backJitter = new Vector2(0.3f, 0.22f);
        public Vector2 backSizeMin = new Vector2(0.85f, 0.55f);
        public Vector2 backSizeMax = new Vector2(2.2f, 1.8f);

        [Header("形状")]
        [Tooltip("越大越容易生成接近正方形的大块。")]
        [Range(0f, 1f)] public float squareBias = 0.55f;

        [Header("动画")]
        [Tooltip("每隔多久重新排布一次方块（秒）。")]
        public float reshuffleSeconds = 1.35f;
        [Range(0.2f, 1f)] public float panelOpacity = 0.78f;

        private SpriteRenderer _sr;
        private Material _instance;
        private Material _original;
        private Material _panelSource;
        private Transform _frontRoot;
        private Transform _backRoot;
        private readonly List<PanelItem> _front = new List<PanelItem>();
        private readonly List<PanelItem> _back = new List<PanelItem>();
        private float _reshuffleLeft;
        private static Mesh _quadMesh;

        private struct PanelItem
        {
            public Transform transform;
            public MeshRenderer renderer;
            public Material material;
            public float seed;
            public float phase;
        }

        private void Awake() => Apply();

        private void OnEnable()
        {
            Apply();
            EnsureBlocks();
            ReshuffleAll(force: true);
        }

        private void OnDisable()
        {
            if (_sr != null && _original != null)
                _sr.sharedMaterial = _original;
            SetBlocksActive(false);
        }

        private void OnDestroy()
        {
            if (_instance != null) Destroy(_instance);
            ClearPanels(_front);
            ClearPanels(_back);
        }

        private void LateUpdate()
        {
            if (_instance != null)
            {
                _instance.SetFloat("_LegTop", legTop);
                _instance.SetFloat("_MistStrength", mistStrength);
                _instance.SetFloat("_HideFeet", hideFeet);
                _instance.SetFloat("_BodyGlitch", bodyGlitch);
                // 减弱 shader 内“噪点格子”，实体方块改由 Mesh 承担
                _instance.SetFloat("_DataStrength", 0.15f);
                _instance.SetFloat("_RectChance", 0.05f);
                _instance.SetFloat("_ScanChance", 0.12f);
                _instance.SetFloat("_SliceStrength", 0.008f);
            }

            SyncBlockRoots();

            _reshuffleLeft -= Time.unscaledDeltaTime;
            if (_reshuffleLeft <= 0f)
            {
                ReshuffleAll(force: false);
                _reshuffleLeft = reshuffleSeconds;
            }

            // 轻微呼吸缩放，保持实体感
            float t = Time.unscaledTime;
            Pulse(_front, t);
            Pulse(_back, t);
        }

        [ContextMenu("Reapply Glitch + Blocks")]
        public void Apply()
        {
            _sr = GetComponent<SpriteRenderer>();
            if (_sr == null) return;

            if (_original == null)
                _original = _sr.sharedMaterial;

            Material source = ResolveBossMaterial();
            if (source != null)
            {
                if (_instance == null)
                    _instance = new Material(source) { name = source.name + " (Boss Instance)" };
                _sr.material = _instance;
            }

            _panelSource = ResolvePanelMaterial();
            EnsureBlocks();
        }

        private void EnsureBlocks()
        {
            if (_sr == null) _sr = GetComponent<SpriteRenderer>();
            if (_frontRoot == null)
            {
                _frontRoot = new GameObject("GlitchBlocks_Front").transform;
                _frontRoot.SetParent(transform, false);
            }
            if (_backRoot == null)
            {
                _backRoot = new GameObject("GlitchBlocks_Back").transform;
                _backRoot.SetParent(transform, false);
            }

            EnsureList(_front, _frontRoot, frontBlockCount, front: true);
            EnsureList(_back, _backRoot, backBlockCount, front: false);
            SetBlocksActive(true);
            SyncBlockRoots();
        }

        private void SyncBlockRoots()
        {
            if (_sr == null) return;
            int order = _sr.sortingOrder;
            string layer = _sr.sortingLayerName;

            if (_frontRoot != null)
            {
                _frontRoot.localPosition = Vector3.zero;
                for (int i = 0; i < _front.Count; i++)
                {
                    if (_front[i].renderer == null) continue;
                    _front[i].renderer.sortingLayerName = layer;
                    _front[i].renderer.sortingOrder = order + 2 + i;
                }
            }

            if (_backRoot != null)
            {
                _backRoot.localPosition = Vector3.zero;
                for (int i = 0; i < _back.Count; i++)
                {
                    if (_back[i].renderer == null) continue;
                    _back[i].renderer.sortingLayerName = layer;
                    _back[i].renderer.sortingOrder = order - 2 - i;
                }
            }
        }

        private void EnsureList(List<PanelItem> list, Transform root, int count, bool front)
        {
            while (list.Count > count)
            {
                int last = list.Count - 1;
                if (list[last].transform != null) Destroy(list[last].transform.gameObject);
                if (list[last].material != null) Destroy(list[last].material);
                list.RemoveAt(last);
            }

            while (list.Count < count)
            {
                var go = new GameObject(front ? $"FrontPanel_{list.Count}" : $"BackPanel_{list.Count}");
                go.transform.SetParent(root, false);
                var mf = go.AddComponent<MeshFilter>();
                mf.sharedMesh = QuadMesh();
                var mr = go.AddComponent<MeshRenderer>();
                mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                mr.receiveShadows = false;

                Material mat = _panelSource != null
                    ? new Material(_panelSource)
                    : new Material(Shader.Find("DeusEx/BossGlitchPanel") ?? Shader.Find("Sprites/Default"));
                mat.name = "BossGlitchPanel (Instance)";
                mr.sharedMaterial = mat;

                list.Add(new PanelItem
                {
                    transform = go.transform,
                    renderer = mr,
                    material = mat,
                    seed = Random.value * 100f,
                    phase = Random.value * Mathf.PI * 2f,
                });
            }
        }

        private void ReshuffleAll(bool force)
        {
            Bounds b = _sr != null ? _sr.bounds : new Bounds(transform.position, Vector3.one * 2f);
            // 用本地空间：以 sprite 本地尺寸估算
            Vector2 localSize = EstimateLocalSize();

            LayoutCluster(_front, front: true, localSize, force);
            LayoutCluster(_back, front: false, localSize, force);
            _reshuffleLeft = reshuffleSeconds;
        }

        private Vector2 EstimateLocalSize()
        {
            if (_sr == null || _sr.sprite == null) return new Vector2(2.5f, 3.5f);
            Vector2 s = _sr.sprite.bounds.size;
            Vector3 ls = transform.lossyScale;
            return new Vector2(Mathf.Abs(s.x * ls.x), Mathf.Abs(s.y * ls.y));
        }

        private void LayoutCluster(List<PanelItem> list, bool front, Vector2 localSize, bool force)
        {
            Vector2 anchor = front ? frontAnchor : backAnchor;
            Vector2 jitter = front ? frontJitter : backJitter;
            Vector2 sizeMin = front ? frontSizeMin : backSizeMin;
            Vector2 sizeMax = front ? frontSizeMax : backSizeMax;

            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                if (item.transform == null) continue;

                // 锚点是相对角色中心的归一化偏移（按 sprite 尺寸）
                float jx = (Random.value - 0.5f) * 2f * jitter.x;
                float jy = (Random.value - 0.5f) * 2f * jitter.y;
                Vector3 localPos = new Vector3(
                    (anchor.x + jx) * localSize.x,
                    (anchor.y + jy) * localSize.y,
                    front ? -0.02f : 0.04f);

                // 转成本地坐标（父级是 Boss）
                if (transform.lossyScale.x != 0f && transform.lossyScale.y != 0f)
                {
                    localPos.x /= transform.lossyScale.x;
                    localPos.y /= transform.lossyScale.y;
                }

                float w = Mathf.Lerp(sizeMin.x, sizeMax.x, Random.value);
                float h = Mathf.Lerp(sizeMin.y, sizeMax.y, Random.value);

                // 大块更容易变成接近正方形；小块仍可偏扁长条
                float sizeNorm = Mathf.InverseLerp(
                    (sizeMin.x + sizeMin.y) * 0.5f,
                    (sizeMax.x + sizeMax.y) * 0.5f,
                    (w + h) * 0.5f);
                bool makeSquare = Random.value < (squareBias * 0.35f + sizeNorm * squareBias);
                if (makeSquare)
                {
                    float side = Mathf.Max(w, h);
                    // 大块再略放大一点
                    if (sizeNorm > 0.55f) side *= Mathf.Lerp(1f, 1.2f, sizeNorm);
                    w = side;
                    h = side * Random.Range(0.88f, 1.08f);
                }
                else if (front && Random.value < 0.25f)
                {
                    // 少量细长条保留层次，但比例不再压得太扁
                    h *= Random.Range(0.7f, 0.9f);
                }

                if (!front && !makeSquare) w *= 1.1f;

                if (transform.lossyScale.x != 0f && transform.lossyScale.y != 0f)
                {
                    w /= Mathf.Abs(transform.lossyScale.x);
                    h /= Mathf.Abs(transform.lossyScale.y);
                }

                item.transform.localPosition = localPos;
                item.transform.localRotation = Quaternion.identity;
                item.transform.localScale = new Vector3(w, h, 1f);

                if (item.material != null)
                {
                    float seed = force ? item.seed : Random.value * 100f;
                    item.seed = seed;
                    item.material.SetFloat("_Seed", seed);
                    item.material.SetFloat("_Opacity", panelOpacity * (front ? 0.9f : 0.55f));
                    item.material.SetFloat("_ShowAccent", (front && i == 0) || (!front && i == 0) ? 1f : (Random.value < 0.25f ? 1f : 0f));
                    var col = front
                        ? new Color(0.38f, 0.66f, 0.72f, 0.82f)
                        : new Color(0.48f, 0.62f, 0.70f, 0.45f);
                    item.material.SetColor("_Color", col);
                }

                list[i] = item;
            }
        }

        private void Pulse(List<PanelItem> list, float t)
        {
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                if (item.transform == null) continue;
                float pulse = 1f + Mathf.Sin(t * 1.6f + item.phase) * 0.03f;
                Vector3 s = item.transform.localScale;
                // 不改基础尺寸，只做极轻呼吸：用 material opacity
                if (item.material != null)
                {
                    float baseOp = panelOpacity * (item.transform.parent == _frontRoot ? 0.9f : 0.55f);
                    item.material.SetFloat("_Opacity", baseOp * (0.92f + 0.08f * Mathf.Sin(t * 2.2f + item.phase)));
                }
                _ = pulse;
                _ = s;
            }
        }

        private void SetBlocksActive(bool on)
        {
            if (_frontRoot != null) _frontRoot.gameObject.SetActive(on);
            if (_backRoot != null) _backRoot.gameObject.SetActive(on);
        }

        private void ClearPanels(List<PanelItem> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].material != null) Destroy(list[i].material);
                if (list[i].transform != null) Destroy(list[i].transform.gameObject);
            }
            list.Clear();
        }

        private Material ResolveBossMaterial()
        {
            if (glitchMaterial != null) return glitchMaterial;
#if UNITY_EDITOR
            glitchMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Scripts/Shaders/BossGlitch.mat");
            if (glitchMaterial != null) return glitchMaterial;
#endif
            Shader shader = Shader.Find("DeusEx/BossGlitch");
            if (shader == null) return null;
            glitchMaterial = new Material(shader) { name = "BossGlitch (Runtime)" };
            return glitchMaterial;
        }

        private Material ResolvePanelMaterial()
        {
            if (panelMaterial != null) return panelMaterial;
#if UNITY_EDITOR
            panelMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Scripts/Shaders/BossGlitchPanel.mat");
            if (panelMaterial != null) return panelMaterial;
#endif
            Shader shader = Shader.Find("DeusEx/BossGlitchPanel");
            if (shader == null) return null;
            panelMaterial = new Material(shader) { name = "BossGlitchPanel (Runtime)" };
            return panelMaterial;
        }

        private static Mesh QuadMesh()
        {
            if (_quadMesh != null) return _quadMesh;
            _quadMesh = new Mesh
            {
                name = "BossGlitchPanelQuad",
                vertices = new[]
                {
                    new Vector3(-0.5f, -0.5f, 0f),
                    new Vector3( 0.5f, -0.5f, 0f),
                    new Vector3(-0.5f,  0.5f, 0f),
                    new Vector3( 0.5f,  0.5f, 0f),
                },
                uv = new[]
                {
                    new Vector2(0f, 0f), new Vector2(1f, 0f),
                    new Vector2(0f, 1f), new Vector2(1f, 1f),
                },
                triangles = new[] { 0, 2, 1, 2, 3, 1 },
            };
            _quadMesh.RecalculateBounds();
            return _quadMesh;
        }
    }
}
