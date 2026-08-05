using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DeusEx.Rhythm
{
    /// <summary>
    /// 单张卡的视觉视图：插画始终完整显示；仅灰色遮罩使用 Filled 表现加载。
    /// 卡名与说明由 CardWheelUI 输出到场景中的公共 DisplayText，不显示在卡面上。
    /// </summary>
    [DisallowMultipleComponent]
    public class CardView : MonoBehaviour
    {
        [Header("基础引用")]
        public CanvasGroup canvasGroup;
        public RectTransform rectTransform;
        public Image background;
        public Graphic outline;
        // 保留这两个字段以兼容旧的自定义 prefab；本项目的公共信息不再写入卡面。
        public TMP_Text nameText;
        public TMP_Text descriptionText;
        public TMP_Text costText;

        [Header("卡面加载")]
        [Tooltip("完整显示的卡图，不使用 Filled。")]
        public Image iconImage;
        [Tooltip("盖在卡图上方的半透明灰色遮罩；只有此 Image 使用 Filled。")]
        public Image loadingMask;
        [Min(0.05f)]
        public float imageLoadDuration = 0.65f;

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
        private Color _highlightColor = Color.white;
        private Coroutine _loadRoutine;
        private bool _loadStarted;

        private void Awake() => EnsureRefs();

        private void Update()
        {
            if (!HasUsableBorders) return;
            float alpha = _highlightRequested && IsImageLoaded
                ? 0.72f + Mathf.Sin(Time.unscaledTime * 7f) * 0.22f
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

            // 旧 prefab 的根 Image 就是插画，不能以卡色染色，否则会压暗原图。
            if (background != null && background != iconImage && tintBackgroundWithCardColor && def != null)
                background.color = def.color;

            PrepareImage(def != null ? def.icon : null);
        }

        /// <summary>只允许当前首张卡启动遮罩加载。</summary>
        public void SetCurrentCardLoading(bool isCurrent)
        {
            if (!isCurrent || _loadStarted || IsImageLoaded || iconImage == null || iconImage.sprite == null) return;
            _loadStarted = true;
            _loadRoutine = StartCoroutine(LoadMask());
        }

        public void SetUsableHighlight(bool usable, Color color)
        {
            _highlightRequested = usable;
            _highlightColor = color;
        }

        private void PrepareImage(Sprite sprite)
        {
            if (_loadRoutine != null) StopCoroutine(_loadRoutine);
            _loadRoutine = null;
            _loadStarted = false;
            IsImageLoaded = sprite == null;

            if (iconImage != null)
            {
                iconImage.sprite = sprite;
                iconImage.enabled = sprite != null;
                iconImage.type = Image.Type.Simple;
                iconImage.fillAmount = 1f;
            }
            if (loadingMask != null)
            {
                loadingMask.enabled = sprite != null;
                loadingMask.type = Image.Type.Filled;
                loadingMask.fillMethod = Image.FillMethod.Vertical;
                loadingMask.fillOrigin = (int)Image.OriginVertical.Bottom;
                loadingMask.fillAmount = sprite != null ? 1f : 0f;
            }
        }

        private IEnumerator LoadMask()
        {
            float elapsed = 0f;
            while (elapsed < imageLoadDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / imageLoadDuration);
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
            loadingMask.color = new Color(0.35f, 0.35f, 0.38f, 0.68f);
            loadingMask.raycastTarget = false;

            EnsureCostBadge();
            EnsureBorders();
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
