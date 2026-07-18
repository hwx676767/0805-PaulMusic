using System.Collections.Generic;
using UnityEngine;

namespace DeusEx.Rhythm
{
    /// <summary>
    /// 卡牌效果的执行器：订阅 <see cref="CardSystem.CardPlayed"/>，把被打出卡牌的 <see cref="CardEffectSpec"/>
    /// 施加为"计时增益"，到期自动回收。多个同类增益按乘法叠加（例如两张攻击卡叠成 ×2.25）。
    ///
    /// 各类效果分别推给对应系统：
    /// - 攻击力  → <see cref="CombatSystem.DamageDealtMultiplier"/>
    /// - 判定窗口 → <see cref="RhythmGameManager.JudgementWindowScale"/>
    /// - 歌曲/节奏速度 → <see cref="RhythmGameManager.SetPlaybackSpeed"/>
    /// - 洗牌   → <see cref="CardSystem.ShuffleDrawPile"/>（瞬时）
    ///
    /// 用法：在场景里任意 GameObject（例如挂着 CardSystem 的那个）上添加本组件即可，各引用留空会自动查找。
    /// 与音游/战斗核心解耦：只通过公开属性/方法与事件交互，禁用时会把所有倍率复位。
    /// </summary>
    public class CardEffectRunner : MonoBehaviour
    {
        [Header("引用（留空自动查找）")]
        public CardSystem cardSystem;
        public RhythmGameManager manager;
        public CombatSystem combat;

        [Header("调试")]
        [Tooltip("勾选后用 IMGUI 显示当前生效中的增益（倍率与剩余时间）。")]
        public bool showDebugOverlay = false;

        private class TimedMod
        {
            public float multiplier;
            public float timeLeft;
            public bool permanent; // duration<=0：持续到本局结束（禁用时才清）
        }

        private readonly List<TimedMod> _damage = new List<TimedMod>();
        private readonly List<TimedMod> _window = new List<TimedMod>();
        private readonly List<TimedMod> _speed = new List<TimedMod>();

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
            Recompute(); // 复位所有倍率到默认值
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
            }
        }

        private static TimedMod NewMod(CardEffectSpec spec) => new TimedMod
        {
            multiplier = Mathf.Approximately(spec.magnitude, 0f) ? 1f : spec.magnitude,
            timeLeft = spec.duration,
            permanent = spec.duration <= 0f,
        };

        private void Update()
        {
            bool changed = false;
            changed |= Tick(_damage);
            changed |= Tick(_window);
            changed |= Tick(_speed);
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
            if (combat != null) combat.DamageDealtMultiplier = Product(_damage);
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

        private void OnGUI()
        {
            if (!showDebugOverlay) return;
            if (_label == null) _label = new GUIStyle(GUI.skin.label) { fontSize = 14, richText = true };

            float y = 150f;
            GUI.Label(new Rect(Screen.width - 320f, y, 300f, 22f),
                $"攻击 ×{Product(_damage):0.00}  ({_damage.Count})", _label); y += 20f;
            GUI.Label(new Rect(Screen.width - 320f, y, 300f, 22f),
                $"判定窗口 ×{Product(_window):0.00}  ({_window.Count})", _label); y += 20f;
            GUI.Label(new Rect(Screen.width - 320f, y, 300f, 22f),
                $"速度 ×{Product(_speed):0.00}  ({_speed.Count})", _label);
        }
    }
}
