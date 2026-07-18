using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DeusEx.Rhythm
{
    /// <summary>
    /// 挂在"卡牌 prefab"根节点上的展示组件：声明卡面各可视元素的引用，供 <see cref="CardWheelUI"/>
    /// 在运行时填充内容（名字/描述/费用/底色/图标）并驱动淡入淡出与高亮。
    ///
    /// 做自己的卡牌 prefab 时：在根节点挂本组件，把子物体里的文本/图片拖到对应字段（都可留空，
    /// 留空的项会被跳过）。根节点建议是带 RectTransform 的 UI 物体；CanvasGroup 若不填会自动补一个。
    /// 若不指定 prefab，CardWheelUI 会程序化生成一张带本组件的默认卡面。
    /// </summary>
    [DisallowMultipleComponent]
    public class CardView : MonoBehaviour
    {
        [Tooltip("控制整张卡淡入淡出的 CanvasGroup。留空会自动在根节点上取/加。")]
        public CanvasGroup canvasGroup;
        [Tooltip("卡面根 RectTransform。留空自动取本物体的。")]
        public RectTransform rectTransform;
        [Tooltip("卡面底色图（可选）。tintBackgroundWithCardColor 勾选时会被染成卡牌颜色。")]
        public Image background;
        [Tooltip("高亮外框（可选，Image/其它 Graphic 均可）。轮到它是第一张时 CardWheelUI 会把它染成高亮色。")]
        public Graphic outline;
        [Tooltip("名称文本（可选）。")]
        public TMP_Text nameText;
        [Tooltip("描述文本（可选）。")]
        public TMP_Text descriptionText;
        [Tooltip("费用文本（可选）。CardWheelUI 会按当前 combo 是否够费给它染色。")]
        public TMP_Text costText;
        [Tooltip("图标（可选）。卡牌配了 icon 才显示。")]
        public Image iconImage;
        [Tooltip("是否用卡牌颜色给 background 底色染色。用美术卡面时可关掉，保留贴图本色。")]
        public bool tintBackgroundWithCardColor = true;

        /// <summary>当前绑定的卡牌实例。</summary>
        public CardInstance Card { get; private set; }

        private void Awake() => EnsureRefs();

        /// <summary>补齐必要引用（RectTransform / CanvasGroup），可安全重复调用。</summary>
        public void EnsureRefs()
        {
            if (rectTransform == null) rectTransform = transform as RectTransform;
            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
                if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        /// <summary>把一张牌的展示内容填充到卡面上。</summary>
        public void Bind(CardInstance card)
        {
            Card = card;
            CardDefinition def = card != null ? card.definition : null;

            if (nameText != null) nameText.text = def != null ? def.cardName : "";
            if (descriptionText != null) descriptionText.text = def != null ? def.description : "";
            if (costText != null) costText.text = card != null ? card.Cost.ToString() : "";
            if (background != null && tintBackgroundWithCardColor && def != null) background.color = def.color;
            if (iconImage != null)
            {
                iconImage.sprite = def != null ? def.icon : null;
                iconImage.enabled = iconImage.sprite != null;
            }
        }
    }
}
