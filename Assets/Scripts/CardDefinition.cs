using System;
using System.Collections.Generic;
using UnityEngine;

namespace DeusEx.Rhythm
{
    /// <summary>卡牌效果类型。具体的施加与回收由 <see cref="CardEffectRunner"/> 处理。</summary>
    public enum CardEffectType
    {
        /// <summary>攻击力（命中对敌人的伤害）乘以 magnitude，持续 duration 秒。</summary>
        DamageMultiplier,
        /// <summary>判定窗口（Perfect/Great/Good）乘以 magnitude，持续 duration 秒。</summary>
        JudgementWindowScale,
        /// <summary>歌曲与节奏的播放速度乘以 magnitude（如 0.8 减速、1.2 加速），持续 duration 秒。</summary>
        SongSpeedMultiplier,
        /// <summary>立即洗匀抽牌堆（magnitude/duration 忽略）。</summary>
        ShuffleDrawPile,
        /// <summary>立刻治疗玩家 magnitude 点生命（duration 忽略）。</summary>
        HealPlayer,
        /// <summary>立刻对敌人造成 magnitude 点伤害（duration 忽略）。</summary>
        FlatDamage,
        /// <summary>获得 magnitude 层护盾；每层抵消下一次受到的伤害（Miss / Boss 攻击）。duration&gt;0 时到期清零剩余层数。</summary>
        ShieldCharges,
        /// <summary>受到的伤害乘以 magnitude（0=免疫），持续 duration 秒。</summary>
        DamageTakenMultiplier,
    }

    /// <summary>一条卡牌效果配置。多条组合即一张牌的完整效果。</summary>
    [Serializable]
    public struct CardEffectSpec
    {
        [Tooltip("效果类型。")]
        public CardEffectType type;
        [Tooltip("数值含义随类型变化：倍率 / 治疗量 / 直伤 / 护盾层数等。")]
        public float magnitude;
        [Tooltip("持续秒数；<=0 对持续类效果表示到本局结束。瞬时效果（治疗/直伤/洗牌）忽略。")]
        public float duration;
    }

    /// <summary>
    /// 一张卡牌的"模板"定义（ScriptableObject 资源）：描述展示信息、费用与打出后的效果列表。
    ///
    /// 用法：在工程里 Assets &gt; Create &gt; DeusEx &gt; Card 创建若干张卡资源，
    /// 填好名字/描述/费用/卡面颜色与效果（<see cref="effects"/>），再拖进场景里 <see cref="CardSystem"/> 的"起始牌库"列表。
    /// 若牌库留空，<see cref="CardSystem"/> 会在运行时自动生成一副含这些效果的 Demo 牌库，零配置即可测试。
    /// 效果的实际生效需要场景里挂一个 <see cref="CardEffectRunner"/>（订阅出牌事件来施加/回收）。
    /// </summary>
    [CreateAssetMenu(fileName = "Card", menuName = "DeusEx/Card", order = 0)]
    public class CardDefinition : ScriptableObject
    {
        [Tooltip("卡牌显示名。")]
        public string cardName = "New Card";

        [TextArea]
        [Tooltip("卡牌描述（用于展示）。")]
        public string description = "";

        [Min(0)]
        [Tooltip("打出该牌需要消耗的 combo 费用。弃牌不消耗费用。")]
        public int cost = 5;

        [Tooltip("卡面主色（用于程序化卡面 CardWheelUI 的底色）。")]
        public Color color = new Color(0.20f, 0.55f, 1f);

        [Tooltip("可选卡面图标（留空则用卡色底 + 卡名占位，不再显示空框）。")]
        public Sprite icon;

        [Tooltip("卡面加载秒数。<=0 时按费用与效果强度自动估算（越强越久）。")]
        [Min(0f)]
        public float imageLoadDuration;

        [Tooltip("打出该牌时依次施加的效果（可为空 = 无效果）。")]
        public List<CardEffectSpec> effects = new List<CardEffectSpec>();

        /// <summary>
        /// 实际用于卡面加载动画的秒数：手动配置优先，否则按「费用 + 效果强度」估算。
        /// 弱牌约 2.2～3.0s，强牌约 4.5～6.0s。
        /// </summary>
        public float ResolveImageLoadDuration()
        {
            if (imageLoadDuration > 0.01f) return imageLoadDuration;
            float score = EstimatePowerScore();
            return Mathf.Clamp(2.0f + score * 0.85f, 2.2f, 6.0f);
        }

        /// <summary>综合费用与效果，得到相对“强度分”（仅用于加载时长等表现）。</summary>
        public float EstimatePowerScore()
        {
            float score = Mathf.Max(0, cost) * 0.12f;
            if (effects == null) return score;
            for (int i = 0; i < effects.Count; i++)
                score += EffectPowerScore(effects[i]);
            return score;
        }

        public static float EffectPowerScore(CardEffectSpec spec)
        {
            switch (spec.type)
            {
                case CardEffectType.FlatDamage:
                    return Mathf.Max(0f, spec.magnitude) / 35f;
                case CardEffectType.HealPlayer:
                    return Mathf.Max(0f, spec.magnitude) / 40f;
                case CardEffectType.DamageMultiplier:
                    return Mathf.Max(0f, spec.magnitude - 1f) * 2.2f + Mathf.Max(0f, spec.duration) * 0.04f;
                case CardEffectType.JudgementWindowScale:
                    return Mathf.Max(0f, spec.magnitude - 1f) * 2.0f + Mathf.Max(0f, spec.duration) * 0.04f;
                case CardEffectType.SongSpeedMultiplier:
                    return Mathf.Abs(spec.magnitude - 1f) * 2.5f + Mathf.Max(0f, spec.duration) * 0.035f;
                case CardEffectType.ShieldCharges:
                    return Mathf.Max(1f, spec.magnitude) * 0.55f + Mathf.Max(0f, spec.duration) * 0.03f;
                case CardEffectType.DamageTakenMultiplier:
                    return Mathf.Max(0f, 1f - spec.magnitude) * 2.5f + Mathf.Max(0f, spec.duration) * 0.04f;
                case CardEffectType.ShuffleDrawPile:
                    return 0.15f;
                default:
                    return 0.2f;
            }
        }

        /// <summary>优先用手写描述；为空时根据 effects 里的数值自动生成。</summary>
        public string ResolveDescription()
        {
            if (!string.IsNullOrWhiteSpace(description)) return description.Trim();
            return FormatEffectsSummary("；");
        }

        /// <summary>把 effects 里的数值格式化成可读文案（供 UI / 出牌提示调用）。</summary>
        public string FormatEffectsSummary(string separator = "\n")
        {
            if (effects == null || effects.Count == 0) return "No effect";
            var parts = new List<string>(effects.Count);
            for (int i = 0; i < effects.Count; i++)
            {
                string line = FormatEffect(effects[i]);
                if (!string.IsNullOrEmpty(line)) parts.Add(line);
            }
            return parts.Count > 0 ? string.Join(separator, parts) : "No effect";
        }

        public static string FormatEffect(CardEffectSpec spec)
        {
            switch (spec.type)
            {
                case CardEffectType.DamageMultiplier:
                    return DurationSuffix($"ATK x{spec.magnitude:0.##}", spec.duration);
                case CardEffectType.JudgementWindowScale:
                    return DurationSuffix($"Judgement window x{spec.magnitude:0.##}", spec.duration);
                case CardEffectType.SongSpeedMultiplier:
                    return DurationSuffix($"Song speed x{spec.magnitude:0.##}", spec.duration);
                case CardEffectType.ShuffleDrawPile:
                    return "Shuffle draw pile";
                case CardEffectType.HealPlayer:
                    return $"Heal {Mathf.RoundToInt(spec.magnitude)} HP";
                case CardEffectType.FlatDamage:
                    return $"Deal {Mathf.RoundToInt(spec.magnitude)} damage to Boss";
                case CardEffectType.ShieldCharges:
                    return DurationSuffix(
                        $"Gain {Mathf.Max(1, Mathf.RoundToInt(spec.magnitude))} shield",
                        spec.duration);
                case CardEffectType.DamageTakenMultiplier:
                    return DurationSuffix($"Damage taken x{spec.magnitude:0.##}", spec.duration);
                default:
                    return string.Empty;
            }
        }

        private static string DurationSuffix(string core, float duration)
        {
            if (duration <= 0f) return core;
            return $"{core} ({duration:0.#}s)";
        }
    }
}
