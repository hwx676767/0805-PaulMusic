using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DeusEx.Rhythm
{
    /// <summary>
    /// 单张卡的视觉视图：插画始终完整显示；仅灰色遮罩使用 Filled 表现加载。
    /// 卡名与说明由 CardWheelUI 输出到场景中的公共 DisplayText；无插画时卡面上显示占位色与卡名。
    /// </summary>
    [DisallowMultipleComponent]
    public class CardView : MonoBehaviour
    {
        [Header("基础引用")]
        public CanvasGroup canvasGroup;
        public RectTransform rectTransform;
        public Image background;
        public Graphic outline;
        // 保留这两个字段以兼容旧的自定义 prefab；公共信息默认写到 DisplayText。
        public TMP_Text nameText;
        public TMP_Text descriptionText;
        public TMP_Text costText;

        [Header("卡面加载")]
        [Tooltip("完整显示的卡图，不使用 Filled。")]
        public Image iconImage;
        [Tooltip("盖在卡图上方的半透明灰色遮罩；只有此 Image 使用 Filled。")]
        public Image loadingMask;
        [Tooltip("无卡定义时的默认加载时间（秒）。有卡定义时改用 CardDefinition.ResolveImageLoadDuration。")]
        [Min(0.05f)]
        public float imageLoadDuration = 1.5f;

        [Header("可用高亮")]
        public Image topBorder;
        public Image bottomBorder;
        public Image leftBorder;
        public Image rightBorder;
        public bool tintBackgroundWithCardColor = true;

        public CardInstance Card { get; private set; }
        public bool IsImageLoaded { get; private set; }
        public bool HasUsableBorders => topBorder != null && bottomBorder != null &&
                                        leftBorder != null && rightBorder != null;
        public event System.Action<CardView> ImageLoadCompleted;

        private bool _highlightRequested;
        private bool _highlightCanPlay;
        private Color _highlightColor = Color.white;
        private Coroutine _loadRoutine;
        private bool _loadStarted;
        private TMP_Text _faceTitle;
        private static Sprite _oneUnitMaskSprite;

        private void Awake() => EnsureRefs();

        private void Update()
        {
            if (!HasUsableBorders) return;
            float alpha = _highlightRequested && IsImageLoaded
                ? (_highlightCanPlay
                    ? 0.78f + Mathf.Sin(Time.unscaledTime * 9f) * 0.22f
                    : 0.55f + Mathf.Sin(Time.unscaledTime * 6f) * 0.15f)
                : 0f;
            SetBorderColor(new Color(_highlightColor.r, _highlightColor.g, _highlightColor.b, alpha));
        }

        public void EnsureRefs()
        {
            if (rectTransform == null) rectTransform = transform as RectTransform;
            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
                if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
            if (iconImage == null) iconImage = GetComponent<Image>();
            EnsureVisualTemplate();
        }

        public void Bind(CardInstance card)
        {
            Card = card;
            CardDefinition def = card != null ? card.definition : null;
            if (costText != null) costText.text = card != null ? card.Cost.ToString() : string.Empty;

            imageLoadDuration = def != null
                ? def.ResolveImageLoadDuration()
                : Mathf.Max(0.05f, imageLoadDuration);

            // 旧 prefab 的根 Image 就是插画，不能以卡色染色，否则会压暗原图。
            if (background != null && background != iconImage && tintBackgroundWithCardColor && def != null)
                background.color = def.color;

            PrepareImage(def);
        }

        /// <summary>只允许当前首张卡启动遮罩加载。</summary>
        public void SetCurrentCardLoading(bool isCurrent)
        {
            if (!isCurrent || _loadStarted || IsImageLoaded || iconImage == null || !iconImage.enabled) return;
            _loadStarted = true;
            _loadRoutine = StartCoroutine(LoadMask());
        }

        public void SetUsableHighlight(bool usable, Color color, bool canPlay = false)
        {
            _highlightRequested = usable;
            _highlightCanPlay = canPlay;
            _highlightColor = color;
        }

        private void PrepareImage(CardDefinition def)
        {
            if (_loadRoutine != null) StopCoroutine(_loadRoutine);
            _loadRoutine = null;
            _loadStarted = false;
            IsImageLoaded = false;

            Sprite art = def != null ? def.icon : null;
            bool hasArt = art != null;
            Color faceColor = def != null ? def.color : new Color(0.22f, 0.45f, 0.85f, 1f);

            if (iconImage != null)
            {
                if (hasArt)
                {
                    iconImage.sprite = art;
                    iconImage.color = Color.white;
                }
                else
                {
                    // 无插画：用纯色占位，避免“空框”
                    iconImage.sprite = OneUnitMaskSprite;
                    iconImage.color = faceColor;
                }
                iconImage.enabled = true;
                iconImage.type = Image.Type.Simple;
                iconImage.fillAmount = 1f;
                iconImage.preserveAspect = hasArt;
            }

            if (loadingMask != null)
            {
                loadingMask.enabled = true;
                loadingMask.type = Image.Type.Filled;
                loadingMask.fillMethod = Image.FillMethod.Vertical;
                loadingMask.fillOrigin = (int)Image.OriginVertical.Bottom;
                loadingMask.fillAmount = 1f;
            }

            UpdateFaceTitle(hasArt ? null : def);
            RestackFaceLayers();
        }

        private void RestackFaceLayers()
        {
            // 自下而上：插画 → 卡名占位 → 加载遮罩 → 费用 → 边框
            if (iconImage != null) iconImage.transform.SetAsFirstSibling();
            if (_faceTitle != null) _faceTitle.transform.SetSiblingIndex(iconImage != null ? 1 : 0);
            if (loadingMask != null)
            {
                int maskIndex = 0;
                if (iconImage != null) maskIndex++;
                if (_faceTitle != null && _faceTitle.gameObject.activeSelf) maskIndex++;
                loadingMask.transform.SetSiblingIndex(maskIndex);
            }
            Transform costBadge = transform.Find("CostBadge");
            if (costBadge != null) costBadge.SetAsLastSibling();
            if (costText != null && costBadge == null) costText.transform.SetAsLastSibling();
            if (topBorder != null) topBorder.transform.SetAsLastSibling();
            if (bottomBorder != null) bottomBorder.transform.SetAsLastSibling();
            if (leftBorder != null) leftBorder.transform.SetAsLastSibling();
            if (rightBorder != null) rightBorder.transform.SetAsLastSibling();
        }

        private IEnumerator LoadMask()
        {
            float duration = Mathf.Max(0.05f, imageLoadDuration);
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                if (loadingMask != null) loadingMask.fillAmount = 1f - Mathf.SmoothStep(0f, 1f, t);
                yield return null;
            }
            if (loadingMask != null)
            {
                loadingMask.fillAmount = 0f;
                loadingMask.enabled = false;
            }
            IsImageLoaded = true;
            _loadRoutine = null;
            ImageLoadCompleted?.Invoke(this);
        }

        private void EnsureVisualTemplate()
        {
            if (rectTransform == null) return;
            if (background == null) background = iconImage;

            loadingMask = loadingMask != null ? loadingMask : FindOrCreateImage("LoadingMask");
            SetRect(loadingMask.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            loadingMask.sprite = OneUnitMaskSprite;
            loadingMask.color = new Color(0.35f, 0.35f, 0.38f, 0.68f);
            loadingMask.raycastTarget = false;

            EnsureFaceTitle();
            EnsureCostBadge();
            EnsureBorders();
        }

        private void EnsureFaceTitle()
        {
            if (_faceTitle != null) return;
            Transform existing = transform.Find("FaceTitle");
            if (existing != null)
            {
                _faceTitle = existing.GetComponent<TMP_Text>();
                if (_faceTitle != null) return;
            }

            var go = new GameObject("FaceTitle", typeof(RectTransform));
            go.transform.SetParent(transform, false);
            var rect = go.GetComponent<RectTransform>();
            SetRect(rect, new Vector2(0.08f, 0.35f), new Vector2(0.92f, 0.72f), Vector2.zero, Vector2.zero);
            _faceTitle = go.AddComponent<TextMeshProUGUI>();
            _faceTitle.alignment = TextAlignmentOptions.Center;
            _faceTitle.fontSize = 28f;
            _faceTitle.fontStyle = FontStyles.Bold;
            _faceTitle.color = Color.white;
            _faceTitle.enableWordWrapping = true;
            _faceTitle.raycastTarget = false;
            _faceTitle.gameObject.SetActive(false);
        }

        private void UpdateFaceTitle(CardDefinition def)
        {
            EnsureFaceTitle();
            if (_faceTitle == null) return;

            bool show = def != null && def.icon == null;
            _faceTitle.gameObject.SetActive(show);
            if (!show) return;

            _faceTitle.text = string.IsNullOrWhiteSpace(def.cardName) ? "CARD" : def.cardName;
            // 深色底用浅字，浅色底用深字
            float lum = def.color.r * 0.299f + def.color.g * 0.587f + def.color.b * 0.114f;
            _faceTitle.color = lum > 0.55f
                ? new Color(0.08f, 0.08f, 0.1f, 0.95f)
                : new Color(1f, 1f, 1f, 0.95f);
        }

        private void EnsureCostBadge()
        {
            if (costText == null) return;
            Image badge = FindOrCreateImage("CostBadge");
            SetRect(badge.rectTransform, new Vector2(1f, 1f), new Vector2(1f, 1f),
                new Vector2(-58f, -58f), new Vector2(-10f, -10f));
            badge.color = new Color(0.005f, 0.01f, 0.04f, 0.88f);
            costText.rectTransform.SetParent(badge.rectTransform, false);
            SetRect(costText.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            costText.alignment = TextAlignmentOptions.Center;
        }

        private void EnsureBorders()
        {
            topBorder = topBorder != null ? topBorder : FindOrCreateImage("UsableBorderTop");
            bottomBorder = bottomBorder != null ? bottomBorder : FindOrCreateImage("UsableBorderBottom");
            leftBorder = leftBorder != null ? leftBorder : FindOrCreateImage("UsableBorderLeft");
            rightBorder = rightBorder != null ? rightBorder : FindOrCreateImage("UsableBorderRight");
            SetRect(topBorder.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(4f, -8f), new Vector2(-4f, -3f));
            SetRect(bottomBorder.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(4f, 3f), new Vector2(-4f, 8f));
            SetRect(leftBorder.rectTransform, new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(3f, 8f), new Vector2(8f, -8f));
            SetRect(rightBorder.rectTransform, new Vector2(1f, 0f), new Vector2(1f, 1f), new Vector2(-8f, 8f), new Vector2(-3f, -8f));
            SetBorderColor(Color.clear);
        }

        private Image FindOrCreateImage(string childName)
        {
            Transform child = transform.Find(childName);
            if (child != null && child.TryGetComponent(out Image existing)) return existing;
            var go = new GameObject(childName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(transform, false);
            return go.GetComponent<Image>();
        }

        /// <summary>显式创建 1×1、PPU=1 的白色 Sprite，供 UI Image 的 Filled 模式可靠裁切。</summary>
        private static Sprite OneUnitMaskSprite
        {
            get
            {
                if (_oneUnitMaskSprite != null) return _oneUnitMaskSprite;

                var texture = new Texture2D(1, 1, TextureFormat.RGBA32, false)
                {
                    name = "CardLoadingMask_1Unit"
                };
                texture.SetPixel(0, 0, Color.white);
                texture.filterMode = FilterMode.Point;
                texture.Apply(false, true);
                _oneUnitMaskSprite = Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f),
                    new Vector2(0.5f, 0.5f), 1f);
                _oneUnitMaskSprite.name = "CardLoadingMask_1Unit";
                return _oneUnitMaskSprite;
            }
        }

        private void SetBorderColor(Color color)
        {
            if (!HasUsableBorders) return;
            topBorder.color = color;
            bottomBorder.color = color;
            leftBorder.color = color;
            rightBorder.color = color;
        }

        private static void SetRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax,
            Vector2 offsetMin, Vector2 offsetMax)
        {
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
        }
    }
}
