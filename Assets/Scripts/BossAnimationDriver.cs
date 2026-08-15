using UnityEngine;

namespace DeusEx.Rhythm
{
    /// <summary>
    /// Boss 动画的轻量表现层：默认播放待机；敌人受到伤害时播放受击。
    /// 不参与任何伤害或胜负计算，攻击动画可由后续 Boss 行为脚本调用 PlayAttack。
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class BossAnimationDriver : MonoBehaviour
    {
        [Header("引用")]
        [Tooltip("留空会从同一物体获取 Animator。")]
        public Animator animator;
        [Tooltip("留空会自动查找场景中的 CombatSystem。")]
        public CombatSystem combatSystem;

        [Header("表现")]
        [Tooltip("敌人血量下降时自动播放一次受击动画。")]
        public bool playHitWhenEnemyDamaged = true;
        [Tooltip("连续多段判定之间最短的受击触发间隔，避免动画每帧重置。")]
        [Min(0f)] public float hitCooldown = 0.12f;

        private float _lastEnemyHealth;
        private float _lastHitTime = float.NegativeInfinity;
        private bool _hasHealthSample;

        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int Hit = Animator.StringToHash("Hit");

        private void Awake()
        {
            if (animator == null) animator = GetComponent<Animator>();
            if (combatSystem == null) combatSystem = FindFirstObjectByType<CombatSystem>();
        }

        private void OnEnable()
        {
            if (combatSystem == null) combatSystem = FindFirstObjectByType<CombatSystem>();
            if (combatSystem == null) return;

            _lastEnemyHealth = combatSystem.EnemyHealth;
            _hasHealthSample = true;
            combatSystem.EnemyHealthChanged += OnEnemyHealthChanged;
        }

        private void OnDisable()
        {
            if (combatSystem != null)
                combatSystem.EnemyHealthChanged -= OnEnemyHealthChanged;
        }

        /// <summary>供后续 Boss 行为、时间轴或按钮事件调用。</summary>
        public void PlayAttack()
        {
            if (animator != null) animator.SetTrigger(Attack);
        }

        /// <summary>供后续 Boss 行为、时间轴或按钮事件调用。</summary>
        public void PlayHit()
        {
            if (animator != null) animator.SetTrigger(Hit);
        }

        private void OnEnemyHealthChanged(float current, float maximum)
        {
            bool tookDamage = _hasHealthSample && current < _lastEnemyHealth;
            _lastEnemyHealth = current;
            _hasHealthSample = true;

            if (playHitWhenEnemyDamaged && tookDamage && Time.unscaledTime >= _lastHitTime + hitCooldown)
            {
                _lastHitTime = Time.unscaledTime;
                PlayHit();
            }
        }
    }
}
