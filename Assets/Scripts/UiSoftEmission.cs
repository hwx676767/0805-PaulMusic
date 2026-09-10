using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DeusEx.Rhythm
{
    /// <summary>
    /// 外发光/泛光：不改原图材质与颜色，在同物体下挂一层 Additive 外发光。
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Image))]
    public sealed class UiSoftEmission : MonoBehaviour
    {
        [Tooltip("留空则用 UIOuterGlow.mat / 运行时创建。")]
        public Material glowMaterial;

        public Color glowColor = new Color(0.65f, 0.88f, 1f, 1f);
        [Range(0f, 3f)] public float glowStrength = 1.1f;
        [Range(1f, 24f)] public float glowSpread = 8f;
        [Range(0.25f, 3f)] public float glowSoftness = 1.35f;
        [Range(0f, 0.4f)] public float pulseAmount = 0.08f;
        public float pulseSpeed = 1.1f;

        private Image _source;
        private Image _glowImage;
        private Material _instance;

        private void Awake() => EnsureGlowLayer();
        private void OnEnable() => EnsureGlowLayer();

        private void OnDisable()
        {
            if (_glowImage != null)
                _glowImage.enabled = false;
        }

        private void OnDestroy()
        {
            if (_instance != null) Destroy(_instance);
        }

        private void LateUpdate()
        {
            if (_source == null) _source = GetComponent<Image>();
            if (_glowImage == null || _instance == null) EnsureGlowLayer();
            if (_glowImage == null || _instance == null) return;

            // 同步你当前的 sprite / 颜色布局，不覆盖原 Image 的材质
            _glowImage.enabled = isActiveAndEnabled && _source != null && _source.enabled;
            if (_source != null)
            {
                _glowImage.sprite = _source.sprite;
                _glowImage.type = _source.type;
                _glowImage.preserveAspect = _source.preserveAspect;
                _glowImage.color = Color.white;
            }

            _instance.SetColor("_GlowColor", glowColor);
            _instance.SetFloat("_GlowStrength", glowStrength);
            _instance.SetFloat("_GlowSpread", glowSpread);
            _instance.SetFloat("_GlowSoftness", glowSoftness);
            _instance.SetFloat("_PulseAmount", pulseAmount);
            _instance.SetFloat("_PulseSpeed", pulseSpeed);
        }

        [ContextMenu("Rebuild Outer Glow Layer")]
        public void EnsureGlowLayer()
        {
            _source = GetComponent<Image>();
            if (_source == null) return;

            // 关键：原图必须保持默认材质，避免“换色”
            if (_source.material != null && _source.material.shader != null &&
                (_source.material.shader.name.Contains("UISoftEmission") ||
                 _source.material.shader.name.Contains("UIOuterGlow")))
            {
                _source.material = null;
            }

            Transform existing = transform.Find("OuterGlow");
            GameObject glowGo;
            if (existing != null) glowGo = existing.gameObject;
            else
            {
                glowGo = new GameObject("OuterGlow", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                glowGo.transform.SetParent(transform, false);
            }

            var rect = glowGo.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.localScale = Vector3.one;
            rect.SetAsLastSibling(); // Additive 外圈叠在原图之上；本体处无贡献，不改色

            _glowImage = glowGo.GetComponent<Image>();
            _glowImage.raycastTarget = false;
            _glowImage.maskable = _source.maskable;
            _glowImage.sprite = _source.sprite;
            _glowImage.color = Color.white;

            Material sourceMat = ResolveMaterial();
            if (sourceMat == null)
            {
                Debug.LogWarning("[UiSoftEmission] 找不到 DeusEx/UIOuterGlow。", this);
                return;
            }

            if (_instance == null)
                _instance = new Material(sourceMat) { name = sourceMat.name + " (OuterGlow)" };

            _glowImage.material = _instance;
            _glowImage.enabled = true;
        }

        private Material ResolveMaterial()
        {
            if (glowMaterial != null) return glowMaterial;
#if UNITY_EDITOR
            glowMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Scripts/Shaders/UIOuterGlow.mat");
            if (glowMaterial != null) return glowMaterial;
            // 兼容旧路径名
            glowMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Scripts/Shaders/UISoftEmission.mat");
#endif
            Shader shader = Shader.Find("DeusEx/UIOuterGlow");
            if (shader == null) return null;
            glowMaterial = new Material(shader) { name = "UIOuterGlow (Runtime)" };
            return glowMaterial;
        }
    }
}
