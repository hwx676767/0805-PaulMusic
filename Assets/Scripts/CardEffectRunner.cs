using System.Collections.Generic;
using UnityEngine;

namespace DeusEx.Rhythm
{
    /// <summary>
    /// 卡牌效果的执行器：订阅 <see cref="CardSystem.CardPlayed"/>，把被打出卡牌的 <see cref="CardEffectSpec"/>
    /// 施加为"计时增益"或瞬时效果，到期自动回收。多个同类持续增益按乘法叠加。
    ///
    /// 各类效果分别推给对应系统：
    /// - 攻击力  → <see cref="CombatSystem.DamageDealtMultiplier"/>
    /// - 判定窗口 → <see cref="RhythmGameManager.JudgementWindowScale"/>
    /// - 歌曲/节奏速度 → <see cref="RhythmGameManager.SetPlaybackSpeed"/>
    /// - 洗牌   → <see cref="CardSystem.ShuffleDrawPile"/>（瞬时）
    /// - 治疗 / 直伤 / 护盾 / 减伤 → <see cref="CombatSystem"/>
    ///
    /// 用法：在场景里任意 GameObject（例如挂着 CardSystem 的那个）上添加本组件即可，各引用留空会自动查找。
    /// </summary>
    public class CardEffectRunner : MonoBehaviour
    {
        [Header("引用（留空自动查找）")]
        public CardSystem cardSystem;
        public RhythmGameManager manager;
        public CombatSystem combat;

        [Header("调试 / 玩家可见增益")]
        [Tooltip("勾选后用 IMGUI 显示当前生效中的增益（倍率与剩余时间）。")]
        public bool showDebugOverlay = false;
        [Tooltip("给玩家看的增益条（左上），打出持续类卡后显示数值。")]
        public bool showPlayerBuffHud = true;

        private class TimedMod
        {
            public float multiplier;
            public float timeLeft;
            public bool permanent; // duration<=0：持续到本局结束（禁用时才清）
        }

        private readonly List<TimedMod> _damage = new List<TimedMod>();
        private readonly List<TimedMod> _window = new List<TimedMod>();
        private readonly List<TimedMod> _speed = new List<TimedMod>();
        private readonly List<TimedMod> _taken = new List<TimedMod>();
        private float _shieldExpireLeft;
        private bool _shieldTimed;

        private void OnEnable()
        {
            if (cardSystem == null) cardSystem = FindObjectOfType<CardSystem>();
            if (manager == null) manager = cardSystem != null && cardSystem.manager != null
                ? cardSystem.manager : FindObjectOfType<RhythmGameManager>();
            if (combat == null) combat = FindObjectOfType<CombatSystem>();

            if (cardSystem != null) cardSystem.CardPlayed += OnCardPlayed;
        }

        private void OnDisable()
        {
            if (cardSystem != null) cardSystem.CardPlayed -= OnCardPlayed;
            _damage.Clear();
            _window.Clear();
            _speed.Clear();
            _taken.Clear();
            _shieldTimed = false;
            _shieldExpireLeft = 0f;
            if (combat != null) combat.ClearShieldCharges();
            Recompute();
        }

        private void OnCardPlayed(CardInstance card)
        {
            var def = card != null ? card.definition : null;
            if (def == null || def.effects == null) return;

            foreach (var spec in def.effects) Apply(spec);
            Recompute();
        }

        private void Apply(CardEffectSpec spec)
        {
            switch (spec.type)
            {
                case CardEffectType.DamageMultiplier:
                    _damage.Add(NewMod(spec));
                    break;
                case CardEffectType.JudgementWindowScale:
                    _window.Add(NewMod(spec));
                    break;
                case CardEffectType.SongSpeedMultiplier:
                    _speed.Add(NewMod(spec));
                    break;
                case CardEffectType.ShuffleDrawPile:
                    if (cardSystem != null) cardSystem.ShuffleDrawPile();
                    break;
                case CardEffectType.HealPlayer:
                    if (combat != null) combat.HealPlayer(Mathf.Max(0f, spec.magnitude));
                    break;
                case CardEffectType.FlatDamage:
                    if (combat != null) combat.DamageEnemy(Mathf.Max(0f, spec.magnitude));
                    break;
                case CardEffectType.ShieldCharges:
                    if (combat != null)
                    {
                        int charges = Mathf.Max(1, Mathf.RoundToInt(spec.magnitude));
                        combat.AddShieldCharges(charges);
                        if (spec.duration > 0f)
                        {
                            _shieldTimed = true;
                            _shieldExpireLeft = Mathf.Max(_shieldExpireLeft, spec.duration);
                        }
                    }
                    break;
                case CardEffectType.DamageTakenMultiplier:
                    _taken.Add(NewMod(spec));
                    break;
            }
        }

        private static TimedMod NewMod(CardEffectSpec spec) => new TimedMod
        {
            multiplier = Mathf.Approximately(spec.magnitude, 0f) &&
                         spec.type != CardEffectType.DamageTakenMultiplier
                ? 1f
                : spec.magnitude,
            timeLeft = spec.duration,
            permanent = spec.duration <= 0f,
        };

        private void Update()
        {
            bool changed = false;
            changed |= Tick(_damage);
            changed |= Tick(_window);
            changed |= Tick(_speed);
            changed |= Tick(_taken);

            if (_shieldTimed)
            {
                _shieldExpireLeft -= Time.deltaTime;
                if (_shieldExpireLeft <= 0f)
                {
                    _shieldTimed = false;
                    _shieldExpireLeft = 0f;
                    if (combat != null) combat.ClearShieldCharges();
                }
            }

            if (changed) Recompute();
        }

        private static bool Tick(List<TimedMod> mods)
        {
            bool removed = false;
            for (int i = mods.Count - 1; i >= 0; i--)
            {
                if (mods[i].permanent) continue;
                mods[i].timeLeft -= Time.deltaTime;
                if (mods[i].timeLeft <= 0f)
                {
                    mods.RemoveAt(i);
                    removed = true;
                }
            }
            return removed;
        }

        private void Recompute()
        {
            if (combat != null)
            {
                combat.DamageDealtMultiplier = Product(_damage);
                combat.DamageTakenMultiplier = Product(_taken);
            }
            if (manager != null)
            {
                manager.JudgementWindowScale = Product(_window);
                manager.SetPlaybackSpeed(Product(_speed));
            }
        }

        private static float Product(List<TimedMod> mods)
        {
            float p = 1f;
            for (int i = 0; i < mods.Count; i++) p *= mods[i].multiplier;
            return p;
        }

        private GUIStyle _label;
        private GUIStyle _buffStyle;

        private void OnGUI()
        {
            if (showPlayerBuffHud) DrawPlayerBuffHud();
            if (!showDebugOverlay) return;
            if (_label == null) _label = new GUIStyle(GUI.skin.label) { fontSize = 14, richText = true };

            float y = 150f;
            GUI.Label(new Rect(Screen.width - 320f, y, 300f, 22f),
                $"攻击 ×{Product(_damage):0.00}  ({_damage.Count})", _label); y += 20f;
            GUI.Label(new Rect(Screen.width - 320f, y, 300f, 22f),
                $"判定窗口 ×{Product(_window):0.00}  ({_window.Count})", _label); y += 20f;
            GUI.Label(new Rect(Screen.width - 320f, y, 300f, 22f),
                $"速度 ×{Product(_speed):0.00}  ({_speed.Count})", _label); y += 20f;
            GUI.Label(new Rect(Screen.width - 320f, y, 300f, 22f),
                $"受伤 ×{Product(_taken):0.00}  ({_taken.Count})", _label); y += 20f;
            int shields = combat != null ? combat.ShieldCharges : 0;
            GUI.Label(new Rect(Screen.width - 320f, y, 300f, 22f),
                $"护盾 {shields}" + (_shieldTimed ? $"  ({_shieldExpireLeft:0.0}s)" : ""), _label);
        }

        private void DrawPlayerBuffHud()
        {
            if (_buffStyle == null)
            {
                _buffStyle = new GUIStyle(GUI.skin.box)
                {
                    fontSize = 18,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleLeft,
                    richText = true,
                    padding = new RectOffset(12, 12, 8, 8),
                };
            }

            var lines = new List<string>();
            AppendBuffLines(lines, "攻击", _damage);
            AppendBuffLines(lines, "判定", _window);
            AppendBuffLines(lines, "曲速", _speed);
            AppendBuffLines(lines, "减伤", _taken);
            if (combat != null && combat.ShieldCharges > 0)
            {
                string shield = $"护盾 ×{combat.ShieldCharges}";
                if (_shieldTimed) shield += $"  {_shieldExpireLeft:0.0}s";
                lines.Add(shield);
            }

            if (lines.Count == 0) return;

            float width = 280f;
            float lineH = 28f;
            float height = 16f + lines.Count * lineH;
            var prev = GUI.color;
            GUI.color = new Color(0f, 0f, 0f, 0.55f);
            GUI.Box(new Rect(18f, 18f, width, height), GUIContent.none);
            GUI.color = Color.white;
            for (int i = 0; i < lines.Count; i++)
                GUI.Label(new Rect(28f, 24f + i * lineH, width - 24f, lineH), lines[i], _buffStyle);
            GUI.color = prev;
        }

        private static void AppendBuffLines(List<string> lines, string title, List<TimedMod> mods)
        {
            if (mods == null || mods.Count == 0) return;
            float product = Product(mods);
            if (Mathf.Approximately(product, 1f) && mods.Count == 0) return;
            if (Mathf.Approximately(product, 1f)) return;

            float soonest = float.PositiveInfinity;
            for (int i = 0; i < mods.Count; i++)
            {
                if (mods[i].permanent) continue;
                soonest = Mathf.Min(soonest, mods[i].timeLeft);
            }

            string time = float.IsPositiveInfinity(soonest) ? "整局" : $"{soonest:0.0}s";
            lines.Add($"{title} ×{product:0.##}  ·  {time}");
        }
    }
}
