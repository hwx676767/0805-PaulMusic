using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DeusEx.Rhythm
{
    /// <summary>
    /// 把 <see cref="CardSystem"/> 的手牌以"轮盘/扇形"的形式呈现在屏幕上，并订阅其事件：
    /// - 出牌/弃牌补位时，剩余的牌沿轮盘旋转到前一档位，被打出/弃掉的牌飞出淡出，新抽到的牌从末位淡入。
    /// - 第一张（可操作的那张）位于 activeAngleDeg 处、最大且高亮，且始终竖直不翻转。
    /// - 依据当前 combo 是否够费用，给第一张的费用文本上色（够=白，不够=红）。
    /// - 出牌被拦截（combo 不足）时，第一张卡抖动反馈。
    ///
    /// 轮盘根节点由你手动摆放（<see cref="wheelRoot"/>）：脚本只把它当作卡牌父级来引用，其本地原点即轮盘圆心，
    /// 卡牌绕它按半径/角度排成扇形。<b>绝不会去实例化它</b>，因此不会出现无限克隆。
    /// - <see cref="wheelRoot"/> 指定：用它作圆心（尊重你的摆放，不改其位置）。
    /// - 留空且本组件自身在 Canvas 下：直接用本物体的 RectTransform 作轮盘根（= 你把本物体摆到哪，圆心就在哪）。
    /// - 都不满足：程序化新建 Canvas 与根节点，放到 <see cref="wheelCenter"/>（彻底零配置兜底）。
    ///
    /// 卡牌可用 <see cref="cardPrefab"/>（根节点需挂 <see cref="CardView"/> 组件）；留空则程序化生成默认卡面。
    ///
    /// 用法：把本组件挂在你摆好的轮盘物体上（或任意物体上并指定 wheelRoot）。cardSystem 留空会自动查找。
    /// </summary>
    public class CardWheelUI : MonoBehaviour
    {
        [Header("引用")]
        [Tooltip("留空则自动在场景中查找 CardSystem。")]
        public CardSystem cardSystem;
        [Tooltip("留空则自动查找 RhythmGameManager（用于判断当前 combo 是否够费用）。")]
        public RhythmGameManager manager;
        [Tooltip("留空则自动创建一个覆盖式 Canvas；也可指定一个已有 Canvas 作为父级。")]
        public Canvas canvas;
        [Tooltip("公共信息区的 TMP 文本。留空时按名称自动查找场景中的 DisplayText。")]
        public TMP_Text displayText;
        [TextArea]
        [Tooltip("{0}=卡名，{1}=效果/描述（从卡牌数值生成），{2}=操作提示（能否打出）。")]
        public string displayTextFormat = "<b>{0}</b>\n{1}\n{2}";

        [Header("参与感反馈")]
        [Tooltip("当前卡变为可打出时，首张卡短暂放大。")]
        public bool punchWhenAffordable = true;
        [Tooltip("可打出时的放大倍数。")]
        public float affordablePunchScale = 1.18f;
        [Tooltip("出牌成功时屏幕中央弹出效果文案。")]
        public bool showPlayToast = true;
        [Tooltip("出牌提示停留时间（秒）。")]
        public float playToastDuration = 1.35f;
        [Tooltip("可打出提示的颜色。")]
        public Color canPlayHintColor = new Color(1f, 0.92f, 0.35f, 1f);
        [Tooltip("不可打出提示的颜色。")]
        public Color cannotPlayHintColor = new Color(1f, 0.55f, 0.55f, 1f);

        [Header("轮盘根节点（手动摆放，脚本只引用不实例化）")]
        [Tooltip("手动摆放在场景 Canvas 下的轮盘根节点：卡牌的父级，其本地原点即轮盘圆心（可在里面放背景美术）。\n" +
                 "留空则：本组件若本身在 Canvas 下就用自身作轮盘根；否则自动新建 Canvas 与根节点。\n" +
                 "注意：不要把带 CardWheelUI 的物体拖进这里。")]
        public RectTransform wheelRoot;
        [Tooltip("勾选后把轮盘根节点移动到下方 wheelCenter（底部中央锚点）；默认关闭以尊重你的手动摆放。")]
        public bool overrideWheelPlacement = false;

        [Header("卡牌外观")]
        [Tooltip("卡牌 prefab：根节点需挂 CardView 组件。留空则程序化生成默认卡面。")]
        public CardView cardPrefab;

        [Header("轮盘布局（基于 1920x1080 参考分辨率）")]
        [Tooltip("轮盘圆心相对屏幕底部中央的锚点偏移；圆心一般放在屏幕下方，卡牌落在弧线上方。")]
        public Vector2 wheelCenter = new Vector2(0f, -40f);
        [Tooltip("轮盘半径（圆心到卡牌中心）。")]
        public float radius = 300f;
        [Tooltip("第一张所在角度：90=正上方，270(或 -90)=正下方，0=右侧，180=左侧。第一张始终竖直不翻转。")]
        public float activeAngleDeg = 90f;
        [Tooltip("相邻两张之间的角度间隔（度）。正值表示后续卡牌向右下方展开。")]
        public float anglePerCardDeg = 26f;
        [Tooltip("程序化卡面尺寸（第一张的基准尺寸；用了卡牌 prefab 时以 prefab 自身尺寸为准）。")]
        public Vector2 cardSize = new Vector2(150f, 210f);
        [Tooltip("每往后一档，卡面缩小的比例（0.16 表示第二张 0.84 倍，第三张 0.68 倍…）。")]
        public float scaleFalloff = 0.16f;
        [Tooltip("卡面最小缩放下限。")]
        public float minScale = 0.5f;
        [Tooltip("每往后一档，卡面变淡的比例。")]
        public float alphaFalloff = 0.18f;
        [Tooltip("卡牌是否随轮盘倾斜（形成扇形）。关闭则所有卡牌保持竖直。")]
        public bool tiltWithWheel = true;

        [Header("动画")]
        [Tooltip("卡牌移动/缩放/旋转向目标插值的速度（越大越快）。")]
        public float lerpSpeed = 11f;
        [Tooltip("被打出的牌飞出方向与距离（相对第一张位置）。")]
        public Vector2 playExitOffset = new Vector2(0f, 260f);
        [Tooltip("被弃掉的牌飞出方向与距离（相对第一张位置）。")]
        public Vector2 discardExitOffset = new Vector2(-220f, -180f);
        [Tooltip("费用不足被拦截时，第一张卡的抖动幅度（像素）。")]
        public float shakeAmplitude = 16f;
        [Tooltip("抖动持续时间（秒）。")]
        public float shakeDuration = 0.35f;

        [Header("卡面外观（作用于程序化卡面，及 CardView 里指定了 outline/costText 的 prefab）")]
        [Tooltip("卡面外框（描边）颜色。")]
        public Color outlineColor = new Color(0f, 0f, 0f, 0.85f);
        [Tooltip("第一张（高亮）外框颜色。")]
        public Color activeOutlineColor = new Color(1f, 0.95f, 0.5f, 1f);
        [Tooltip("当前卡图片加载完成、但 combo 尚不足时的就绪边框颜色。")]
        public Color readyOutlineColor = new Color(0.25f, 0.9f, 1f, 1f);
        [Tooltip("费用够时的角标颜色。")]
        public Color affordColor = new Color(1f, 1f, 1f, 1f);
        [Tooltip("费用不足时的角标颜色。")]
        public Color notAffordColor = new Color(1f, 0.35f, 0.35f, 1f);

        [Header("编辑器预览（Scene 视图线框，不进播放即可调参）")]
        [Tooltip("在 Scene 视图用线框实时预览手牌轮盘布局；随参数变化即时更新，不会生成任何物体。")]
        public bool previewInEditor = true;
        [Range(1, 10)]
        [Tooltip("预览的卡牌张数（指定了 cardSystem 时改用其 handSize）。")]
        public int previewCount = 3;
        [Tooltip("预览卡牌线框颜色。")]
        public Color previewColor = new Color(0.4f, 0.8f, 1f, 1f);
        [Tooltip("预览中第一张（可操作）线框颜色。")]
        public Color previewActiveColor = new Color(1f, 0.9f, 0.4f, 1f);

        // ---- 运行时 ----
        private RectTransform _wheelRoot;
        private readonly List<CardSlot> _slots = new List<CardSlot>();
        private int _pendingExitUid = -1;
        private bool _pendingExitWasPlay;
        private bool _wasAffordable;
        private float _activePunch;
        private TMP_Text _toastText;
        private CanvasGroup _toastGroup;
        private float _toastTimeLeft;
        private string _toastOverride;
        private float _toastOverrideLeft;

        /// <summary>一张卡的可视对象（CardView 组件）与它当前的动画目标状态。</summary>
        private class CardSlot
        {
            public CardView view;

            public Vector2 targetPos;
            public float targetScale;
            public float targetRot;
            public float targetAlpha;
            public bool isActive;    // 是否是手牌第一张（高亮）
            public bool exiting;      // 正在飞出销毁
            public float shakeTimeLeft;

            public CardInstance Card => view != null ? view.Card : null;
            public RectTransform Rect => view != null ? view.rectTransform : null;
        }

        private void OnEnable()
        {
            if (cardSystem == null) cardSystem = FindObjectOfType<CardSystem>();
            if (manager == null) manager = cardSystem != null ? cardSystem.manager : FindObjectOfType<RhythmGameManager>();
            ResolveDisplayText();

            // 安全防护：卡牌 prefab 不能带 CardWheelUI，否则每张牌又会生成一套轮盘 → 无限递归卡死
            if (cardPrefab != null && cardPrefab.GetComponentInChildren<CardWheelUI>(true) != null)
            {
                Debug.LogError("[CardWheelUI] cardPrefab 上包含 CardWheelUI，会导致无限生成，已忽略该 prefab。请改用只挂 CardView 的卡牌 prefab。");
                cardPrefab = null;
            }

            EnsureCanvasAndRoot();

            if (cardSystem != null)
            {
                cardSystem.HandChanged += OnHandChanged;
                cardSystem.CardPlayed += OnCardPlayed;
                cardSystem.CardDiscarded += OnCardDiscarded;
                cardSystem.CardPlayBlocked += OnCardPlayBlocked;
            }
            if (manager != null) manager.ComboChanged += OnComboChanged;

            SyncToHand(); // 初始按当前手牌建好轮盘
        }

        private void OnDisable()
        {
            if (cardSystem != null)
            {
                cardSystem.HandChanged -= OnHandChanged;
                cardSystem.CardPlayed -= OnCardPlayed;
                cardSystem.CardDiscarded -= OnCardDiscarded;
                cardSystem.CardPlayBlocked -= OnCardPlayBlocked;
            }
            if (manager != null) manager.ComboChanged -= OnComboChanged;
        }

        // ---------------- 事件回调 ----------------

        private void OnCardPlayed(CardInstance c)
        {
            _pendingExitUid = c != null ? c.uid : -1;
            _pendingExitWasPlay = true;
            ShowPlayedFeedback(c);
        }

        private void OnCardDiscarded(CardInstance c)
        {
            _pendingExitUid = c != null ? c.uid : -1;
            _pendingExitWasPlay = false;
            if (c != null) FlashDisplayOverride($"已弃掉 <b>{c.DisplayName}</b>", 0.8f);
        }

        private void OnHandChanged() { SyncToHand(); }
        private void OnComboChanged(int combo)
        {
            UpdateAffordability(combo);
            UpdateDisplayText();
        }

        private void OnCardPlayBlocked(CardInstance c)
        {
            // 第一张卡抖动，提示费用不足 / 未加载完
            foreach (var s in _slots)
                if (!s.exiting && s.isActive) s.shakeTimeLeft = shakeDuration;

            if (c == null) return;
            bool loading = cardSystem != null && !cardSystem.IsActiveCardReady;
            string reason = loading
                ? "卡面加载中…"
                : $"Combo 不足（需要 {c.Cost}）";
            FlashDisplayOverride($"<color=#ff8888>无法打出 {c.DisplayName}</color>\n{reason}", 1f);
        }

        // ---------------- 与手牌同步（核心：补位/旋转/进出场） ----------------

        private void SyncToHand()
        {
            if (_wheelRoot == null || cardSystem == null) return;

            IReadOnlyList<CardInstance> hand = cardSystem.Hand;

            // 1) 已不在手牌里的可视对象 → 标记飞出
            foreach (var s in _slots)
            {
                if (s.exiting) continue;
                if (s.Card == null || !HandContains(hand, s.Card.uid))
                {
                    s.exiting = true;
                    s.isActive = false;
                    bool wasPlayed = s.Card != null && s.Card.uid == _pendingExitUid && _pendingExitWasPlay;
                    Vector2 baseSlot = SlotPos(0);
                    s.targetPos = baseSlot + (wasPlayed ? playExitOffset : discardExitOffset);
                    s.targetScale = wasPlayed ? 1.15f : 0.7f;
                    s.targetRot = 0f;
                    s.targetAlpha = 0f;
                }
            }
            _pendingExitUid = -1;

            // 2) 为每张手牌确保有对应可视对象，并设定其目标档位
            for (int i = 0; i < hand.Count; i++)
            {
                CardInstance card = hand[i];
                CardSlot s = FindSlot(card.uid);
                if (s == null)
                {
                    s = SpawnSlot(card);
                    // 新抽到的牌：从目标位置略小、透明淡入
                    PlaceImmediate(s, SlotPos(i), SlotScale(i) * 0.6f, SlotRot(i), 0f);
                }
                ApplySlot(s, i);
            }

            UpdateActiveCardLoading();
            RefreshOutlines();
            UpdateAffordability(manager != null ? manager.Combo : 0);
            UpdateDisplayText();
        }

        private void ApplySlot(CardSlot s, int index)
        {
            s.exiting = false;
            s.isActive = index == 0;
            s.targetPos = SlotPos(index);
            s.targetScale = SlotScale(index);
            s.targetRot = SlotRot(index);
            s.targetAlpha = SlotAlpha(index);
        }

        // ---------------- 轮盘几何 ----------------

        private Vector2 SlotPos(int index)
        {
            float ang = (activeAngleDeg - index * anglePerCardDeg) * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * radius;
        }

        private float SlotScale(int index) => Mathf.Max(minScale, 1f - index * scaleFalloff);
        private float SlotAlpha(int index) => Mathf.Clamp01(1f - index * alphaFalloff);
        // 倾斜相对"第一张"来算：第一张永远竖直（0°），其余按 anglePerCardDeg 依次扇形展开。
        // 这样不论 activeAngleDeg 放在何处（顶端/底部/侧边），第一张都不会被翻转。
        private float SlotRot(int index) => tiltWithWheel ? -index * anglePerCardDeg : 0f;

        // ---------------- 每帧插值 ----------------

        private void Update()
        {
            float t = 1f - Mathf.Exp(-lerpSpeed * Time.deltaTime); // 帧率无关的指数逼近

            for (int i = _slots.Count - 1; i >= 0; i--)
            {
                CardSlot s = _slots[i];
                RectTransform rect = s.Rect;
                if (rect == null) { _slots.RemoveAt(i); continue; }

                Vector2 shake = Vector2.zero;
                if (s.shakeTimeLeft > 0f)
                {
                    s.shakeTimeLeft -= Time.deltaTime;
                    float damp = Mathf.Clamp01(s.shakeTimeLeft / Mathf.Max(0.0001f, shakeDuration));
                    shake = new Vector2(Mathf.Sin(s.shakeTimeLeft * 60f) * shakeAmplitude * damp, 0f);
                }

                rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, s.targetPos, t) + shake;

                float sc = Mathf.Lerp(rect.localScale.x, s.targetScale, t);
                if (s.isActive && !s.exiting && _activePunch > 0.01f)
                    sc *= 1f + (_activePunch * (affordablePunchScale - 1f));
                rect.localScale = new Vector3(sc, sc, 1f);

                float z = Mathf.LerpAngle(rect.localEulerAngles.z, s.targetRot, t);
                rect.localRotation = Quaternion.Euler(0f, 0f, z);

                CanvasGroup group = s.view != null ? s.view.canvasGroup : null;
                if (group != null) group.alpha = Mathf.Lerp(group.alpha, s.targetAlpha, t);

                // 新版 CardView 使用四条独立边框，并且只会在“当前卡 + 费用足够 + 图片加载完成”时发光。
                // 保留旧的单 Graphic 描边路径，以兼容仍在使用的旧/自定义卡牌 prefab。
                if (s.view != null && s.view.HasUsableBorders)
                {
                    bool affordable = s.Card != null && manager != null && manager.Combo >= s.Card.Cost;
                    bool loadedCurrent = !s.exiting && s.isActive && s.view.IsImageLoaded;
                    bool canPlay = loadedCurrent && affordable &&
                                   (cardSystem == null || cardSystem.IsActiveCardReady);
                    // 可打出：金边脉冲；已加载但费用不足：青边提示“这是当前卡”
                    s.view.SetUsableHighlight(loadedCurrent, canPlay ? activeOutlineColor : readyOutlineColor, canPlay);
                }
                else
                {
                    Graphic outline = s.view != null ? s.view.outline : null;
                    if (outline != null)
                    {
                        Color target = s.isActive ? activeOutlineColor : outlineColor;
                        outline.color = Color.Lerp(outline.color, target, t);
                    }
                }

                // 飞出且已基本透明 → 销毁回收
                if (s.exiting && (group == null || group.alpha < 0.03f))
                {
                    if (s.view != null) s.view.ImageLoadCompleted -= OnCardImageLoadCompleted;
                    Destroy(rect.gameObject);
                    _slots.RemoveAt(i);
                }
            }

            if (_activePunch > 0f)
                _activePunch = Mathf.MoveTowards(_activePunch, 0f, Time.deltaTime * 3.2f);

            if (_toastOverrideLeft > 0f)
            {
                _toastOverrideLeft -= Time.deltaTime;
                if (_toastOverrideLeft <= 0f)
                {
                    _toastOverride = null;
                    UpdateDisplayText();
                }
            }

            TickPlayToast();
        }

        // ---------------- 费用够不够的文本上色 ----------------

        private void UpdateAffordability(int combo)
        {
            bool anyAffordableReady = false;
            foreach (var s in _slots)
            {
                TMP_Text cost = s.view != null ? s.view.costText : null;
                if (!s.exiting && cost != null && s.isActive)
                    cost.color = (s.Card != null && combo >= s.Card.Cost) ? affordColor : notAffordColor;
                else if (!s.exiting && cost != null)
                    cost.color = affordColor;

                if (s.view != null && s.view.HasUsableBorders)
                {
                    bool affordable = s.Card != null && combo >= s.Card.Cost;
                    bool loadedCurrent = !s.exiting && s.isActive && s.view.IsImageLoaded;
                    bool canPlay = loadedCurrent && affordable &&
                                   (cardSystem == null || cardSystem.IsActiveCardReady);
                    s.view.SetUsableHighlight(loadedCurrent, canPlay ? activeOutlineColor : readyOutlineColor, canPlay);
                    if (canPlay) anyAffordableReady = true;
                }
            }

            if (punchWhenAffordable && anyAffordableReady && !_wasAffordable)
                _activePunch = 1f;
            _wasAffordable = anyAffordableReady;
        }

        private void RefreshOutlines()
        {
            foreach (var s in _slots)
            {
                if (s.view != null && s.view.HasUsableBorders)
                {
                    int combo = manager != null ? manager.Combo : 0;
                    bool affordable = s.Card != null && combo >= s.Card.Cost;
                    bool loadedCurrent = !s.exiting && s.isActive && s.view.IsImageLoaded;
                    bool canPlay = loadedCurrent && affordable &&
                                   (cardSystem == null || cardSystem.IsActiveCardReady);
                    s.view.SetUsableHighlight(loadedCurrent, canPlay ? activeOutlineColor : readyOutlineColor, canPlay);
                    continue;
                }

                Graphic outline = s.view != null ? s.view.outline : null;
                if (outline != null && !s.exiting)
                    outline.color = s.isActive ? activeOutlineColor : outlineColor;
            }
        }

        // ---------------- 查找辅助 ----------------

        private static bool HandContains(IReadOnlyList<CardInstance> hand, int uid)
        {
            for (int i = 0; i < hand.Count; i++)
                if (hand[i] != null && hand[i].uid == uid) return true;
            return false;
        }

        private CardSlot FindSlot(int uid)
        {
            foreach (var s in _slots)
                if (!s.exiting && s.Card != null && s.Card.uid == uid) return s;
            return null;
        }

        // ---------------- 搭建 UI ----------------

        private void EnsureCanvasAndRoot()
        {
            if (_wheelRoot != null) return;

            // 1) 你手动指定了轮盘根节点：直接引用，绝不实例化（尊重你的摆放）
            if (wheelRoot != null)
            {
                _wheelRoot = wheelRoot;
                if (canvas == null) canvas = _wheelRoot.GetComponentInParent<Canvas>();
                if (overrideWheelPlacement) PlaceWheelRootAtCenter();
                return;
            }

            // 2) 未指定：本组件自身若在 Canvas 下，就用自己的 RectTransform 作轮盘根（= 你把本物体摆到哪，圆心就在哪）
            var selfRect = transform as RectTransform;
            if (selfRect != null && GetComponentInParent<Canvas>() != null)
            {
                _wheelRoot = selfRect;
                if (canvas == null) canvas = GetComponentInParent<Canvas>();
                if (overrideWheelPlacement) PlaceWheelRootAtCenter();
                return;
            }

            // 3) 彻底零配置：自建覆盖式 Canvas + 程序化轮盘根，摆到 wheelCenter
            if (canvas == null)
            {
                var canvasGo = new GameObject("CardWheelCanvas");
                canvas = canvasGo.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 50; // 盖在游戏画面之上
                var scaler = canvasGo.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920f, 1080f);
                scaler.matchWidthOrHeight = 0.5f;
                canvasGo.AddComponent<GraphicRaycaster>();
            }

            var rootGo = new GameObject("WheelRoot", typeof(RectTransform));
            _wheelRoot = rootGo.GetComponent<RectTransform>();
            _wheelRoot.SetParent(canvas.transform, false);
            PlaceWheelRootAtCenter();
        }

        private void PlaceWheelRootAtCenter()
        {
            _wheelRoot.anchorMin = _wheelRoot.anchorMax = new Vector2(0.5f, 0f);
            _wheelRoot.pivot = new Vector2(0.5f, 0.5f);
            _wheelRoot.sizeDelta = Vector2.zero;
            _wheelRoot.anchoredPosition = wheelCenter;
        }

        private void PlaceImmediate(CardSlot s, Vector2 pos, float scale, float rot, float alpha)
        {
            RectTransform rect = s.Rect;
            if (rect == null) return;
            rect.anchoredPosition = pos;
            rect.localScale = new Vector3(scale, scale, 1f);
            rect.localRotation = Quaternion.Euler(0f, 0f, rot);
            CanvasGroup group = s.view != null ? s.view.canvasGroup : null;
            if (group != null) group.alpha = alpha;
        }

        /// <summary>为一张牌生成可视对象：优先用 cardPrefab，否则程序化生成默认卡面。</summary>
        private CardSlot SpawnSlot(CardInstance card)
        {
            CardView view = cardPrefab != null
                ? Instantiate(cardPrefab, _wheelRoot, false)
                : BuildProceduralCardView();

            view.name = $"Card_{card.uid}";
            view.EnsureRefs();
            view.ImageLoadCompleted += OnCardImageLoadCompleted;

            // 卡牌需绕自身中心旋转/缩放并按 anchoredPosition 定位到弧线点，统一把锚点/轴心设为居中
            RectTransform rect = view.rectTransform;
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);

            view.Bind(card);

            var slot = new CardSlot { view = view };
            _slots.Add(slot);
            return slot;
        }

        private void UpdateActiveCardLoading()
        {
            foreach (var s in _slots)
            {
                if (s.exiting || s.view == null || s.Card == null || !s.isActive) continue;
                s.view.SetCurrentCardLoading(true);
                if (cardSystem != null) cardSystem.SetActiveCardReady(s.Card, s.view.IsImageLoaded);
                return;
            }
        }

        private void OnCardImageLoadCompleted(CardView view)
        {
            if (view == null || view.Card == null || cardSystem == null) return;
            cardSystem.SetActiveCardReady(view.Card, true);
            UpdateAffordability(manager != null ? manager.Combo : 0);
            UpdateDisplayText();
        }

        private void ResolveDisplayText()
        {
            if (displayText != null) return;
            foreach (var label in FindObjectsOfType<TMP_Text>(true))
            {
                if (label.gameObject.name != "DisplayText") continue;
                displayText = label;
                return;
            }
        }

        private void UpdateDisplayText()
        {
            ResolveDisplayText();
            if (displayText == null) return;

            if (!string.IsNullOrEmpty(_toastOverride) && _toastOverrideLeft > 0f)
            {
                displayText.text = _toastOverride;
                return;
            }

            CardInstance active = cardSystem != null ? cardSystem.ActiveCard : null;
            CardDefinition def = active != null ? active.definition : null;
            if (def == null)
            {
                displayText.text = string.Empty;
                return;
            }

            string effects = def.ResolveDescription();
            string hint = BuildPlayHint(active);
            displayText.text = string.Format(displayTextFormat, def.cardName, effects, hint);
        }

        private string BuildPlayHint(CardInstance active)
        {
            if (active == null || cardSystem == null) return string.Empty;

            string playKey = KeyLabel(cardSystem.playKey);
            string discardKey = KeyLabel(cardSystem.discardKey);
            int combo = manager != null ? manager.Combo : 0;
            bool ready = cardSystem.IsActiveCardReady;
            bool afford = combo >= active.Cost;

            if (!ready)
                return Colored($"加载中… 完成后按 {playKey} 打出 / {discardKey} 弃牌", cannotPlayHintColor);
            if (!afford)
                return Colored($"Combo {combo}/{active.Cost} · 再连击即可按 {playKey} 打出 · {discardKey} 弃牌", cannotPlayHintColor);
            return Colored($"▶ 可打出！按 {playKey} 使用（耗 {active.Cost} Combo）· {discardKey} 弃牌", canPlayHintColor);
        }

        private void ShowPlayedFeedback(CardInstance card)
        {
            if (card == null) return;
            CardDefinition def = card.definition;
            string name = def != null ? def.cardName : card.DisplayName;
            string effects = def != null ? def.FormatEffectsSummary(" · ") : string.Empty;
            string body = string.IsNullOrEmpty(effects)
                ? $"已打出 <b>{name}</b>"
                : $"已打出 <b>{name}</b>\n{effects}";

            FlashDisplayOverride(Colored(body, canPlayHintColor), playToastDuration);
            if (showPlayToast) ShowPlayToast(name, effects);
        }

        private void FlashDisplayOverride(string richText, float duration)
        {
            _toastOverride = richText;
            _toastOverrideLeft = duration;
            ResolveDisplayText();
            if (displayText != null) displayText.text = richText;
        }

        private void EnsurePlayToast()
        {
            if (_toastText != null) return;
            EnsureCanvasAndRoot();
            Transform parent = canvas != null ? canvas.transform : (_wheelRoot != null ? _wheelRoot : transform);

            var go = new GameObject("CardPlayToast", typeof(RectTransform), typeof(CanvasGroup));
            var rect = go.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = new Vector2(0.5f, 0.55f);
            rect.anchorMax = new Vector2(0.5f, 0.55f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(900f, 160f);
            rect.anchoredPosition = Vector2.zero;
            _toastGroup = go.GetComponent<CanvasGroup>();
            _toastGroup.alpha = 0f;
            _toastGroup.blocksRaycasts = false;

            var tmpGo = new GameObject("Text", typeof(RectTransform));
            var tmpRect = tmpGo.GetComponent<RectTransform>();
            tmpRect.SetParent(rect, false);
            tmpRect.anchorMin = Vector2.zero;
            tmpRect.anchorMax = Vector2.one;
            tmpRect.offsetMin = Vector2.zero;
            tmpRect.offsetMax = Vector2.zero;
            _toastText = tmpGo.AddComponent<TextMeshProUGUI>();
            _toastText.alignment = TextAlignmentOptions.Center;
            _toastText.fontSize = 42f;
            _toastText.fontStyle = FontStyles.Bold;
            _toastText.color = Color.white;
            _toastText.raycastTarget = false;
            _toastText.enableWordWrapping = true;
        }

        private void ShowPlayToast(string cardName, string effects)
        {
            EnsurePlayToast();
            _toastText.text = string.IsNullOrEmpty(effects)
                ? $"✦ {cardName}"
                : $"✦ {cardName}\n<size=70%>{effects}</size>";
            _toastGroup.alpha = 1f;
            _toastTimeLeft = playToastDuration;
            if (_toastText.transform.parent is RectTransform rt)
                rt.localScale = Vector3.one * 1.12f;
        }

        private void TickPlayToast()
        {
            if (_toastGroup == null || _toastTimeLeft <= 0f) return;
            _toastTimeLeft -= Time.deltaTime;
            float t = Mathf.Clamp01(_toastTimeLeft / Mathf.Max(0.0001f, playToastDuration));
            _toastGroup.alpha = t;
            if (_toastText != null && _toastText.transform.parent is RectTransform rt)
            {
                float s = Mathf.Lerp(1f, 1.12f, t);
                rt.localScale = new Vector3(s, s, 1f);
            }
        }

        private static string KeyLabel(KeyCode key) => key.ToString().ToUpperInvariant();

        private static string Colored(string text, Color color)
        {
            string hex = ColorUtility.ToHtmlStringRGBA(color);
            return $"<color=#{hex}>{text}</color>";
        }

        // ---------------- 程序化默认卡面 ----------------

        private CardView BuildProceduralCardView()
        {
            // 根：卡面（底色 Image + CanvasGroup + CardView）
            var go = new GameObject("Card", typeof(RectTransform), typeof(CanvasGroup), typeof(Image), typeof(CardView));
            var rect = go.GetComponent<RectTransform>();
            rect.SetParent(_wheelRoot, false);
            rect.sizeDelta = cardSize;
            var bg = go.GetComponent<Image>();

            // 描边：放在卡面后面、略大一圈
            var outlineGo = new GameObject("Outline", typeof(RectTransform), typeof(Image));
            var outlineRect = outlineGo.GetComponent<RectTransform>();
            outlineRect.SetParent(rect, false);
            outlineRect.anchorMin = Vector2.zero;
            outlineRect.anchorMax = Vector2.one;
            outlineRect.offsetMin = new Vector2(-6f, -6f);
            outlineRect.offsetMax = new Vector2(6f, 6f);
            outlineRect.SetAsFirstSibling();
            var outline = outlineGo.GetComponent<Image>();
            outline.color = outlineColor;

            // 名称（上方）
            var nameText = CreateText(rect, "Name", "", 24, FontStyles.Bold,
                TextAlignmentOptions.Top, new Vector2(0f, 0f), new Vector2(1f, 1f),
                new Vector2(8f, 8f), new Vector2(-8f, -14f));

            // 描述（中部，小字）
            var descText = CreateText(rect, "Desc", "", 15, FontStyles.Normal,
                TextAlignmentOptions.Center, new Vector2(0f, 0f), new Vector2(1f, 1f),
                new Vector2(10f, 10f), new Vector2(-10f, -70f));

            // 费用角标（左上角）
            var badgeGo = new GameObject("CostBadge", typeof(RectTransform), typeof(Image));
            var badgeRect = badgeGo.GetComponent<RectTransform>();
            badgeRect.SetParent(rect, false);
            badgeRect.anchorMin = badgeRect.anchorMax = new Vector2(0f, 1f); // 左上角
            badgeRect.pivot = new Vector2(0.5f, 0.5f);
            badgeRect.sizeDelta = new Vector2(48f, 48f);
            badgeRect.anchoredPosition = new Vector2(6f, -6f);
            badgeGo.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.6f);
            var costText = CreateText(badgeRect, "Cost", "", 26, FontStyles.Bold,
                TextAlignmentOptions.Center, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            costText.color = affordColor;

            var view = go.GetComponent<CardView>();
            view.rectTransform = rect;
            view.canvasGroup = go.GetComponent<CanvasGroup>();
            view.background = bg;
            view.outline = outline;
            view.nameText = nameText;
            view.descriptionText = descText;
            view.costText = costText;
            view.tintBackgroundWithCardColor = true;
            return view;
        }

        private static TMP_Text CreateText(RectTransform parent, string name, string text, float size,
            FontStyles style, TextAlignmentOptions align, Vector2 anchorMin, Vector2 anchorMax,
            Vector2 offsetMin, Vector2 offsetMax)
        {
            var go = new GameObject(name, typeof(RectTransform));
            var rect = go.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;

            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = size;
            tmp.fontStyle = style;
            tmp.alignment = align;
            tmp.color = Color.white;
            tmp.raycastTarget = false;
            return tmp;
        }

#if UNITY_EDITOR
        // ---------------- 编辑器预览（Scene 视图线框，实时反映参数，不进播放也能调） ----------------

        private void OnDrawGizmos()
        {
            if (!previewInEditor || Application.isPlaying) return;

            // 与运行时一致：指定了卡牌 prefab 时用 prefab 自身尺寸，否则才用 cardSize
            Vector2 previewSize = GetPreviewCardSize();
            if (previewSize.x <= 0f || previewSize.y <= 0f) return;

            // 预览参考系：优先手动指定的 wheelRoot，其次本物体（RectTransform 优先）。
            RectTransform rt = wheelRoot != null ? wheelRoot : transform as RectTransform;
            Transform root = rt != null ? (Transform)rt : transform;
            Vector2 centerLocal = rt != null
                ? new Vector2((0.5f - rt.pivot.x) * rt.rect.width, (0.5f - rt.pivot.y) * rt.rect.height)
                : Vector2.zero;

            int count = cardSystem != null ? Mathf.Max(1, cardSystem.handSize) : Mathf.Max(1, previewCount);

            Matrix4x4 baseM = root.localToWorldMatrix;
            Vector3 wheelWorldCenter = baseM.MultiplyPoint3x4(new Vector3(centerLocal.x, centerLocal.y, 0f));

            var cardBox = new Vector3(previewSize.x, previewSize.y, 1f);

            // 从后往前画，保证第一张在最上层
            for (int i = count - 1; i >= 0; i--)
            {
                Vector2 slot = centerLocal + SlotPos(i);
                Matrix4x4 m = baseM * Matrix4x4.TRS(
                    new Vector3(slot.x, slot.y, 0f),
                    Quaternion.Euler(0f, 0f, SlotRot(i)),
                    new Vector3(SlotScale(i), SlotScale(i), 1f));

                bool active = i == 0;
                Color line = active ? previewActiveColor : previewColor;
                Color fill = line; fill.a *= 0.18f;

                Gizmos.matrix = m;
                Gizmos.color = fill;
                Gizmos.DrawCube(Vector3.zero, cardBox);
                Gizmos.color = line;
                Gizmos.DrawWireCube(Vector3.zero, cardBox);

                // 圆心 → 卡牌中心连线，并标注序号
                Vector3 cardWorldCenter = m.MultiplyPoint3x4(Vector3.zero);
                Gizmos.matrix = Matrix4x4.identity;
                Gizmos.color = new Color(line.r, line.g, line.b, 0.35f);
                Gizmos.DrawLine(wheelWorldCenter, cardWorldCenter);
                Handles.Label(cardWorldCenter, active ? "1 (第一张)" : (i + 1).ToString());
            }

            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(wheelWorldCenter, 8f);
        }

        // 预览用的卡牌基准尺寸：指定了卡牌 prefab 时读 prefab 根 RectTransform 的实际尺寸，否则用 cardSize。
        private Vector2 GetPreviewCardSize()
        {
            if (cardPrefab != null)
            {
                var prt = cardPrefab.transform as RectTransform;
                if (prt != null)
                {
                    Vector2 sz = prt.rect.size;
                    if (sz.x <= 0f || sz.y <= 0f) sz = prt.sizeDelta;
                    if (sz.x > 0f && sz.y > 0f) return sz;
                }
            }
            return cardSize;
        }
#endif
    }
}
