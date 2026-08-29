using System.Collections.Generic;
using UnityEngine;

namespace DeusEx.Rhythm
{
    /// <summary>
    /// 一张牌在运行时的实例。用唯一 id 标识（即使同一张 <see cref="CardDefinition"/> 被复制多份，
    /// 各实例也能区分开），方便 UI 跨"补位/洗牌"稳定地追踪同一张牌，也为后续给单张牌加临时状态预留。
    /// </summary>
    public class CardInstance
    {
        public readonly int uid;
        public readonly CardDefinition definition;

        public CardInstance(int uid, CardDefinition definition)
        {
            this.uid = uid;
            this.definition = definition;
        }

        public string DisplayName => definition != null ? definition.cardName : "(空)";
        public int Cost => definition != null ? Mathf.Max(0, definition.cost) : 0;
    }

    /// <summary>
    /// 卡牌系统核心逻辑（不含具体牌效）：维护"抽牌堆 / 手牌 / 弃牌堆"三个区，
    /// 手牌固定 <see cref="handSize"/> 张（默认 3），大部分牌藏在看不见的抽牌堆里。
    ///
    /// 玩家只有两种操作（默认按键见下）：
    /// - 出牌（<see cref="playKey"/>）：打出手牌第一张。需要足够 combo 作费用，成功后扣除对应 combo，
    ///   其余牌向前补位，并从抽牌堆抽一张作为新的最后一张。combo 不足则出牌失败（触发 <see cref="CardPlayBlocked"/>）。
    /// - 弃牌（<see cref="discardKey"/>）：弃掉手牌第一张（不消耗 combo），随后同样补位、抽新牌。
    ///
    /// 抽牌堆抽空时（可选）自动把弃牌堆洗回抽牌堆。牌效由场景中的 <see cref="CardEffectRunner"/> 订阅
    /// <see cref="CardPlayed"/> 施加。
    ///
    /// 与音游核心解耦：通过 <see cref="RhythmGameManager.TrySpendCombo"/> 扣费，通过事件通知 UI
    /// （写法参考 <see cref="ComboDisplay"/> / <see cref="CombatSystem"/>）。挂上即用；
    /// 未配置牌库时会自动生成 Demo 牌库，勾选 <see cref="showDebugOverlay"/> 即可用 IMGUI 零配置测试。
    /// </summary>
    public class CardSystem : MonoBehaviour
    {
        [Header("引用")]
        [Tooltip("留空则自动在场景中查找 RhythmGameManager（用于把 combo 作为出牌费用扣除）。")]
        public RhythmGameManager manager;

        [Header("牌库（留空则运行时自动生成一副 Demo 牌库）")]
        [Tooltip("起始牌库。同一张卡资源可重复放多份表示多张。留空则自动生成 Demo 牌库。")]
        public List<CardDefinition> startingDeck = new List<CardDefinition>();

        [Header("规则")]
        [Min(1)]
        [Tooltip("手牌（轮盘）张数。")]
        public int handSize = 3;
        [Tooltip("开局把牌库洗牌。")]
        public bool shuffleOnStart = true;
        [Tooltip("抽牌堆抽空时，自动把弃牌堆洗回抽牌堆继续抽。")]
        public bool reshuffleDiscardWhenEmpty = true;
        [Tooltip("仅在歌曲进行中才允许出牌/弃牌（留空 manager 或未开始时可自行放开）。")]
        public bool requireSongPlaying = false;
        [Tooltip("当前首张卡需要完成卡面加载后才能打出。由 CardWheelUI 报告加载状态。")]
        public bool requireActiveCardVisualLoad = true;

        [Header("按键")]
        [Tooltip("打出手牌第一张的按键。")]
        public KeyCode playKey = KeyCode.Space;
        [Tooltip("弃掉手牌第一张的按键。")]
        public KeyCode discardKey = KeyCode.LeftShift;

        [Header("调试显示（默认关闭；正式表现请用 CardWheelUI）")]
        [Tooltip("勾选后用 IMGUI 画出手牌/牌堆数量/当前 combo 与操作提示，方便零配置直接测试。")]
        public bool showDebugOverlay = false;

        // ---- 运行时状态 ----
        private readonly List<CardInstance> _drawPile = new List<CardInstance>();
        private readonly List<CardInstance> _hand = new List<CardInstance>();
        private readonly List<CardInstance> _discardPile = new List<CardInstance>();
        private int _nextUid;
        private bool _built;
        // 有 CardWheelUI 时由 UI 在当前卡的图片加载期间设为 false；无 UI 时保持 true，兼容原有逻辑。
        private int _activeReadyUid = -1;
        private bool _activeCardReady = true;

        // ---- 对外只读状态 ----
        /// <summary>当前手牌（轮盘），索引 0 为可被打出/弃掉的"第一张"。</summary>
        public IReadOnlyList<CardInstance> Hand => _hand;
        /// <summary>手牌第一张（可打出/弃掉的那张）；手牌为空时为 null。</summary>
        public CardInstance ActiveCard => _hand.Count > 0 ? _hand[0] : null;
        public int DrawPileCount => _drawPile.Count;
        public int DiscardPileCount => _discardPile.Count;
        public int HandCount => _hand.Count;

        /// <summary>当前可出牌卡是否完成了它的视觉加载。</summary>
        public bool IsActiveCardReady => !requireActiveCardVisualLoad || ActiveCard == null ||
            (_activeReadyUid == ActiveCard.uid && _activeCardReady);

        // ---- 对外事件（供 UI 订阅，避免直接耦合） ----
        /// <summary>手牌内容发生变化（出牌/弃牌补位、开局发牌）后触发，供 UI 重建/刷新轮盘。</summary>
        public event System.Action HandChanged;
        /// <summary>成功打出一张牌时触发，参数为被打出的牌（已进入弃牌堆）。供牌效/表现挂钩。</summary>
        public event System.Action<CardInstance> CardPlayed;
        /// <summary>弃掉一张牌时触发，参数为被弃掉的牌。</summary>
        public event System.Action<CardInstance> CardDiscarded;
        /// <summary>出牌被拦截（combo 不足）时触发，参数为想打出的牌。供 UI 做"费用不足"反馈。</summary>
        public event System.Action<CardInstance> CardPlayBlocked;
        /// <summary>牌堆数量（抽牌堆/弃牌堆）变化时触发，供 UI 刷新计数。</summary>
        public event System.Action PilesChanged;

        private void Awake()
        {
            if (manager == null) manager = FindObjectOfType<RhythmGameManager>();
            BuildAndDeal();
        }

        private void Start()
        {
            // 开局补一次通知：此时所有 UI 的 OnEnable 都已订阅，能收到初始手牌与牌堆数量。
            HandChanged?.Invoke();
            PilesChanged?.Invoke();
        }

        private void Update()
        {
            if (requireSongPlaying && (manager == null || !manager.IsPlaying)) return;

            if (Input.GetKeyDown(playKey)) TryPlayActiveCard();
            else if (Input.GetKeyDown(discardKey)) DiscardActiveCard();
        }

        // ---------------- 初始化 / 发牌 ----------------

        private void BuildAndDeal()
        {
            if (_built) return;
            _built = true;

            _drawPile.Clear();
            _hand.Clear();
            _discardPile.Clear();

            List<CardDefinition> source = (startingDeck != null && startingDeck.Count > 0)
                ? startingDeck
                : BuildDemoDeck();

            foreach (var def in source)
            {
                if (def == null) continue;
                _drawPile.Add(new CardInstance(_nextUid++, def));
            }

            if (shuffleOnStart) Shuffle(_drawPile);
            RefillHand();
        }

        /// <summary>
        /// 无配置时的兜底 Demo 牌库：包含本期实现的效果卡（攻击/判定/减速/加速/洗牌），
        /// 每种若干张，便于零配置直接测试。效果的实际生效需场景里有 <see cref="CardEffectRunner"/>。
        /// </summary>
        private static List<CardDefinition> BuildDemoDeck()
        {
            var list = new List<CardDefinition>();

            void Add(string name, int cost, Color color, string desc, CardEffectSpec effect, int copies)
            {
                for (int i = 0; i < copies; i++)
                {
                    var def = ScriptableObject.CreateInstance<CardDefinition>();
                    def.name = name;
                    def.cardName = name;
                    def.description = desc;
                    def.cost = cost;
                    def.color = color;
                    def.effects = new List<CardEffectSpec> { effect };
                    list.Add(def);
                }
            }

            Add("攻击 x1.5", 15, new Color(1f, 0.45f, 0.4f), "10 秒内攻击力 ×1.5",
                new CardEffectSpec { type = CardEffectType.DamageMultiplier, magnitude = 1.5f, duration = 10f }, 2);
            Add("判定 x1.5", 12, new Color(0.4f, 0.75f, 1f), "10 秒内判定窗口 ×1.5",
                new CardEffectSpec { type = CardEffectType.JudgementWindowScale, magnitude = 1.5f, duration = 10f }, 2);
            Add("减速 20%", 8, new Color(0.5f, 0.95f, 0.6f), "10 秒内歌曲与节奏 ×0.8",
                new CardEffectSpec { type = CardEffectType.SongSpeedMultiplier, magnitude = 0.8f, duration = 10f }, 2);
            Add("加速 20%", 8, new Color(1f, 0.7f, 0.25f), "10 秒内歌曲与节奏 ×1.2",
                new CardEffectSpec { type = CardEffectType.SongSpeedMultiplier, magnitude = 1.2f, duration = 10f }, 2);
            Add("洗牌", 3, new Color(0.75f, 0.5f, 1f), "立即洗匀抽牌堆",
                new CardEffectSpec { type = CardEffectType.ShuffleDrawPile }, 2);
            Add("治疗", 8, new Color(0.35f, 0.9f, 0.55f), "立刻回复 25 点生命",
                new CardEffectSpec { type = CardEffectType.HealPlayer, magnitude = 25f }, 2);
            Add("直击", 12, new Color(1f, 0.25f, 0.2f), "立刻对 Boss 造成 80 点伤害",
                new CardEffectSpec { type = CardEffectType.FlatDamage, magnitude = 80f }, 2);
            Add("护盾", 6, new Color(0.55f, 0.8f, 1f), "获得 1 层护盾（抵消下一次受伤），最多保留 12 秒",
                new CardEffectSpec { type = CardEffectType.ShieldCharges, magnitude = 1f, duration = 12f }, 2);

            return list;
        }

        // ---------------- 操作：出牌 / 弃牌 ----------------

        /// <summary>
        /// 打出手牌第一张。需要足够 combo 作费用：成功则扣除对应 combo，把该牌移入弃牌堆，
        /// 其余牌向前补位并抽一张新牌补到末位；combo 不足则不出牌并触发 <see cref="CardPlayBlocked"/>。
        /// 返回是否成功打出。
        /// </summary>
        public bool TryPlayActiveCard()
        {
            var active = ActiveCard;
            if (active == null) return false;

            // 图片加载也是当前卡的就绪条件；未完成时不消耗 combo、不移动任何牌。
            if (!IsActiveCardReady)
            {
                CardPlayBlocked?.Invoke(active);
                return false;
            }

            // combo 作为费用：交给音游管理器扣除（无 manager 时视为免费，便于独立测试）
            if (manager != null && !manager.TrySpendCombo(active.Cost))
            {
                CardPlayBlocked?.Invoke(active);
                return false;
            }

            _hand.RemoveAt(0);
            _discardPile.Add(active);
            RefillHand();

            CardPlayed?.Invoke(active);
            HandChanged?.Invoke();
            PilesChanged?.Invoke();
            return true;
        }

        /// <summary>仅供卡牌 UI 报告当前首张卡的图片加载状态，不改变任何牌库或效果规则。</summary>
        public void SetActiveCardReady(CardInstance card, bool ready)
        {
            if (card == null || ActiveCard == null || card.uid != ActiveCard.uid) return;
            _activeReadyUid = card.uid;
            _activeCardReady = ready;
        }

        /// <summary>
        /// 弃掉手牌第一张（不消耗 combo）。该牌移入弃牌堆，其余牌向前补位并抽一张新牌补到末位。
        /// 返回是否发生了弃牌（手牌为空时返回 false）。
        /// </summary>
        public bool DiscardActiveCard()
        {
            var active = ActiveCard;
            if (active == null) return false;

            _hand.RemoveAt(0);
            _discardPile.Add(active);
            RefillHand();

            CardDiscarded?.Invoke(active);
            HandChanged?.Invoke();
            PilesChanged?.Invoke();
            return true;
        }

        // ---------------- 抽牌 / 补位 / 洗牌 ----------------

        private void RefillHand()
        {
            while (_hand.Count < handSize)
            {
                var c = DrawOne();
                if (c == null) break; // 牌真的抽光了：手牌暂时不足 handSize，之后回收弃牌堆再补
                _hand.Add(c);
            }
        }

        private CardInstance DrawOne()
        {
            if (_drawPile.Count == 0)
            {
                if (!reshuffleDiscardWhenEmpty || _discardPile.Count == 0) return null;
                _drawPile.AddRange(_discardPile);
                _discardPile.Clear();
                Shuffle(_drawPile);
                PilesChanged?.Invoke();
            }

            int last = _drawPile.Count - 1; // 从堆顶（列表末尾）抽，O(1)
            var c = _drawPile[last];
            _drawPile.RemoveAt(last);
            return c;
        }

        /// <summary>洗匀当前抽牌堆（供"洗牌"卡效果调用）。不动手牌与弃牌堆。</summary>
        public void ShuffleDrawPile()
        {
            Shuffle(_drawPile);
            PilesChanged?.Invoke();
        }

        private static void Shuffle(List<CardInstance> list)
        {
            // Fisher–Yates
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        // ---------------- 调试显示（IMGUI，零配置即可看到效果） ----------------

        private GUIStyle _label;
        private GUIStyle _card;

        private void OnGUI()
        {
            if (!showDebugOverlay) return;

            if (_label == null)
            {
                _label = new GUIStyle(GUI.skin.label) { fontSize = 15, richText = true };
                _card = new GUIStyle(GUI.skin.box) { fontSize = 14, alignment = TextAnchor.MiddleCenter, wordWrap = true };
            }

            int combo = manager != null ? manager.Combo : 0;
            float y = Screen.height - 150f;

            GUI.Label(new Rect(20, y - 26, 900, 24),
                $"手牌轮盘（第一张最左）   combo:<b>{combo}</b>   抽牌堆:{_drawPile.Count}   弃牌堆:{_discardPile.Count}", _label);

            float x = 20f;
            for (int i = 0; i < _hand.Count; i++)
            {
                var c = _hand[i];
                bool afford = combo >= c.Cost;
                string tag = i == 0 ? "▶ " : "";
                string affordTag = (i == 0 && !afford) ? "  <color=#ff6464>(combo不足)</color>" : "";
                var prev = GUI.color;
                GUI.color = i == 0 ? Color.white : new Color(1f, 1f, 1f, 0.7f);
                GUI.Box(new Rect(x, y, 120, 90),
                    $"{tag}{c.DisplayName}\n费用 {c.Cost}{affordTag}", _card);
                GUI.color = prev;
                x += 130f;
            }

            GUI.Label(new Rect(20, y + 96, 900, 24),
                $"[{playKey}] 出牌（扣 combo）    [{discardKey}] 弃牌（免费）", _label);
        }
    }
}
