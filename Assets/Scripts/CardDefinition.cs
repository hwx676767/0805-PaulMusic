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
    }

    /// <summary>一条卡牌效果配置。多条组合即一张牌的完整效果。</summary>
    [Serializable]
    public struct CardEffectSpec
    {
        [Tooltip("效果类型。")]
        public CardEffectType type;
        [Tooltip("倍率（DamageMultiplier/JudgementWindowScale/SongSpeedMultiplier 用；洗牌忽略）。")]
        public float magnitude;
        [Tooltip("持续秒数；<=0 表示持续到本局结束。洗牌等瞬时效果忽略。")]
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

        [Tooltip("可选卡面图标（留空则纯色卡面）。")]
        public Sprite icon;

        [Tooltip("打出该牌时依次施加的效果（可为空 = 无效果）。")]
        public List<CardEffectSpec> effects = new List<CardEffectSpec>();
    }
}
