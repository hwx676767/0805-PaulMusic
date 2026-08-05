using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DeusEx.Rhythm
{
    /// <summary>
    /// 卡牌的纯视觉视图。它不会修改牌库、费用或卡牌效果；只负责将 CardDefinition
    /// 显示成统一的图像卡面，并在抽到新牌时播放图片加载与可用状态反馈。
    /// </summary>
    [DisallowMultipleComponent]
    public class CardView : MonoBehaviour
    {
        [Header("基础引用")]
        public CanvasGroup canvasGroup;
        public RectTransform rectTransform;
        public Image background;
        public Graphic outline;
        public TMP_Text nameText;
        public TMP_Text descriptionText;
        public TMP_Text costText;

        [Header("卡面加载")]
        [Tooltip("卡牌插画。新卡抽到时会以 Image Filled 模式由 0% 加载至 100%。")]
        public Image iconImage;
        [Min(0.05f)]
        [Tooltip("插画加载动画时长（秒）。")]
        public float imageLoadDuration = 0.65f;

        [Header("可用高亮")]
        [Tooltip("可用状态的四条边框；留空时会由本组件在运行时创建统一版式。")]
        public Image topBorder;
        public Image bottomBorder;
        public Image leftBorder;
        public Image rightBorder;
        public bool tintBackgroundWithCardColor = true;

        public CardInstance Card { get; private set; }
        public bool IsImageLoaded { get; private set; }
        public bool HasUsableBorders => topBorder != null && bottomBorder != null &&
                                        leftBorder != null && rightBorder != null;

        private bool _highlightRequested;
        private Color _highlightColor = Color.white;
        private Coroutine _loadRoutine;

        private void Awake() => EnsureRefs();

        private void Update()
        {
            if (!HasUsableBorders) return;

            float alpha = _highlightRequested && IsImageLoaded
                ? 0.72f + Mathf.Sin(Time.unscaledTime * 7f) * 0.22f
                : 0f;
            SetBorderColor(new Color(_highlightColor.r, _highlightColor.g, _highlightColor.b, alpha));
        }

        /// <summary>保证手工 prefab 与旧 prefab 都能获得相同的公共卡面版式。</summary>
        public void EnsureRefs()
        {
            if (rectTransform == null) rectTransform = transform as RectTransform;
            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
                if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            if (iconImage == null) iconImage = GetComponent<Image>();
            EnsureSharedVisualTemplate();
        }

        public void Bind(CardInstance card)
        {
            Card = card;
            CardDefinition def = card != null ? card.definition : null;

            if (nameText != null) nameText.text = def != null ? def.cardName : string.Empty;
            if (descriptionText != null) descriptionText.text = def != null ? def.description : string.Empty;
            if (costText != null) costText.text = card != null ? card.Cost.ToString() : string.Empty;
            // 旧 prefab 只有一张根 Image，它同时是插画容器；不能再把它染成底色，
            // 否则会压暗玩家提供的卡图。自定义 prefab 若有独立 background 仍可使用颜色。
            if (background != null && background != iconImage && tintBackgroundWithCardColor && def != null)
            {
                Color c = Color.Lerp(new Color(0.01f, 0.02f, 0.07f, 0.92f), def.color, 0.28f);
                c.a = 0.92f;
                background.color = c;
            }

            StartImageLoad(def != null ? def.icon : null);
        }

        /// <summary>由 CardWheelUI 调用。只有当前卡、费用足够且加载完成时才真正发光。</summary>
        public void SetUsableHighlight(bool usable, Color color)
        {
            _highlightRequested = usable;
            _highlightColor = color;
        }

        private void StartImageLoad(Sprite sprite)
        {
            if (_loadRoutine != null) StopCoroutine(_loadRoutine);
            IsImageLoaded = sprite == null;

            if (iconImage == null) return;

            iconImage.sprite = sprite;
            iconImage.enabled = sprite != null;
            iconImage.type = Image.Type.Filled;
            iconImage.fillMethod = Image.FillMethod.Vertical;
            iconImage.fillOrigin = (int)Image.OriginVertical.Bottom;
            iconImage.fillAmount = sprite != null ? 0f : 1f;

            if (sprite != null) _loadRoutine = StartCoroutine(LoadImage());
        }

        private IEnumerator LoadImage()
        {
            float elapsed = 0f;
            while (elapsed < imageLoadDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / imageLoadDuration);
                iconImage.fillAmount = Mathf.SmoothStep(0f, 1f, t);
                yield return null;
            }

            iconImage.fillAmount = 1f;
            IsImageLoaded = true;
            _loadRoutine = null;
        }

        private void EnsureSharedVisualTemplate()
        {
            if (rectTransform == null) return;

            // 旧 prefab 的根 Image 同时作为卡面背景与可填充的插画容器。
            if (background == null) background = iconImage;

            Image titlePanel = FindOrCreateImage("TitlePanel");
            SetRect(titlePanel.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f),
                new Vector2(10f, -52f), new Vector2(-10f, -10f));
            titlePanel.color = new Color(0.005f, 0.01f, 0.04f, 0.78f);

            Image descriptionPanel = FindOrCreateImage("DescriptionPanel");
            SetRect(descriptionPanel.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 0f),
                new Vector2(10f, 10f), new Vector2(-10f, 48f));
            descriptionPanel.color = new Color(0.005f, 0.01f, 0.04f, 0.78f);

            if (nameText == null)
                nameText = CreateText("CardName", titlePanel.rectTransform, 22f, FontStyles.Bold,
                    TextAlignmentOptions.Left, new Vector2(10f, 3f), new Vector2(-10f, -3f));
            if (descriptionText == null)
                descriptionText = CreateText("CardDescription", descriptionPanel.rectTransform, 13f, FontStyles.Normal,
                    TextAlignmentOptions.Center, new Vector2(8f, 2f), new Vector2(-8f, -2f));

            EnsureCostBadge();
            EnsureBorders();
        }

        private void EnsureCostBadge()
        {
            Image badge = FindOrCreateImage("CostBadge");
            SetRect(badge.rectTransform, new Vector2(1f, 1f), new Vector2(1f, 1f),
                new Vector2(-58f, -58f), new Vector2(-10f, -10f));
            badge.color = new Color(0.005f, 0.01f, 0.04f, 0.88f);

            if (costText == null)
                costText = CreateText("Cost", badge.rectTransform, 27f, FontStyles.Bold,
                    TextAlignmentOptions.Center, Vector2.zero, Vector2.zero);
            else
            {
                costText.rectTransform.SetParent(badge.rectTransform, false);
                SetRect(costText.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
                costText.alignment = TextAlignmentOptions.Center;
            }
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

        private TMP_Text CreateText(string childName, RectTransform parent, float size, FontStyles style,
            TextAlignmentOptions alignment, Vector2 offsetMin, Vector2 offsetMax)
        {
            var go = new GameObject(childName, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            var rect = go.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            SetRect(rect, Vector2.zero, Vector2.one, offsetMin, offsetMax);

            var text = go.GetComponent<TextMeshProUGUI>();
            if (costText != null && costText.font != null) text.font = costText.font;
            text.fontSize = size;
            text.fontStyle = style;
            text.alignment = alignment;
            text.color = Color.white;
            text.textWrappingMode = TextWrappingModes.Normal;
            text.raycastTarget = false;
            return text;
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
