using UnityEngine;

namespace DeusEx.Rhythm
{
    /// <summary>
    /// Boss 主动攻击循环：冷却 → 预警 → 播攻击动画并对玩家造成伤害。
    /// 不负责受击动画（仍由 <see cref="BossAnimationDriver"/> 处理）。
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class BossBehavior : MonoBehaviour
    {
        public enum AttackPhase
        {
            Idle,
            Telegraph,
            Striking,
        }

        [Header("引用（留空自动查找）")]
        public CombatSystem combat;
        public BossAnimationDriver animationDriver;
        public RhythmGameManager manager;

        [Header("节奏")]
        [Tooltip("开局后第一次进入预警前的等待（秒）。")]
        [Min(0f)] public float firstAttackDelay = 8f;
        [Tooltip("两次攻击结算之间的冷却（秒，从本次结算起算）。")]
        [Min(1f)] public float attackInterval = 10f;
        [Tooltip("预警时长（秒）；结束后立刻播攻击并结算伤害。")]
        [Min(0.1f)] public float telegraphDuration = 1.5f;
        [Tooltip("仅在歌曲播放中才推进攻击循环。")]
        public bool requireSongPlaying = true;

        [Header("伤害")]
        [Tooltip("每次攻击对玩家造成的基础伤害（仍受护盾 / 减伤倍率影响）。")]
        [Min(0f)] public float attackDamage = 18f;

        [Header("预警表现")]
        [Tooltip("预警期间给 Boss Sprite 套上的叠加色。")]
        public Color telegraphTint = new Color(1f, 0.45f, 0.35f, 1f);
        [Tooltip("勾选后用 IMGUI 在屏幕上方显示攻击预警。")]
        public bool showTelegraphHud = true;

        public AttackPhase Phase { get; private set; } = AttackPhase.Idle;
        public float PhaseTimeLeft { get; private set; }

        public event System.Action TelegraphStarted;
        public event System.Action AttackExecuted;

        private SpriteRenderer _sprite;
        private Color _baseColor = Color.white;
        private bool _hasBaseColor;
        private float _cooldownLeft;
        private GUIStyle _warnStyle;

        private void Awake()
        {
            if (combat == null) combat = FindFirstObjectByType<CombatSystem>();
            if (animationDriver == null) animationDriver = GetComponent<BossAnimationDriver>();
            if (animationDriver == null) animationDriver = FindFirstObjectByType<BossAnimationDriver>();
            if (manager == null) manager = FindFirstObjectByType<RhythmGameManager>();
            _sprite = GetComponent<SpriteRenderer>();
            if (_sprite != null)
            {
                _baseColor = _sprite.color;
                _hasBaseColor = true;
            }
            _cooldownLeft = firstAttackDelay;
        }

        private void OnDisable()
        {
            RestoreTint();
            Phase = AttackPhase.Idle;
        }

        private void Update()
        {
            if (combat != null && combat.IsOver)
            {
                if (Phase != AttackPhase.Idle)
                {
                    Phase = AttackPhase.Idle;
                    RestoreTint();
                }
                return;
            }

            if (requireSongPlaying && (manager == null || !manager.IsPlaying))
                return;

            float dt = Time.deltaTime;
            switch (Phase)
            {
                case AttackPhase.Idle:
                    _cooldownLeft -= dt;
                    if (_cooldownLeft <= 0f) BeginTelegraph();
                    break;
                case AttackPhase.Telegraph:
                    PhaseTimeLeft -= dt;
                    if (PhaseTimeLeft <= 0f) ExecuteAttack();
                    break;
                case AttackPhase.Striking:
                    // 攻击结算是瞬时的；保留一帧状态便于外部读到 Striking
                    Phase = AttackPhase.Idle;
                    break;
            }
        }

        private void BeginTelegraph()
        {
            Phase = AttackPhase.Telegraph;
            PhaseTimeLeft = telegraphDuration;
            ApplyTelegraphTint();
            TelegraphStarted?.Invoke();
        }

        private void ExecuteAttack()
        {
            Phase = AttackPhase.Striking;
            PhaseTimeLeft = 0f;
            RestoreTint();

            if (animationDriver != null) animationDriver.PlayAttack();
            if (combat != null && attackDamage > 0f) combat.DamagePlayer(attackDamage);

            AttackExecuted?.Invoke();
            _cooldownLeft = attackInterval;
            Phase = AttackPhase.Idle;
        }

        /// <summary>立刻取消当前预警并回到冷却（例如被特殊卡打断时用）。</summary>
        public void CancelTelegraph(float cooldownOverride = -1f)
        {
            if (Phase != AttackPhase.Telegraph) return;
            Phase = AttackPhase.Idle;
            RestoreTint();
            _cooldownLeft = cooldownOverride >= 0f ? cooldownOverride : attackInterval;
        }

        private void ApplyTelegraphTint()
        {
            if (_sprite == null) return;
            if (!_hasBaseColor)
            {
                _baseColor = _sprite.color;
                _hasBaseColor = true;
            }
            _sprite.color = telegraphTint;
        }

        private void RestoreTint()
        {
            if (_sprite != null && _hasBaseColor) _sprite.color = _baseColor;
        }

        private void OnGUI()
        {
            if (!showTelegraphHud || Phase != AttackPhase.Telegraph) return;
            if (_warnStyle == null)
            {
                _warnStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 36,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter,
                };
            }

            var prev = GUI.color;
            GUI.color = new Color(1f, 0.35f, 0.25f, 1f);
            GUI.Label(new Rect(0f, Screen.height * 0.12f, Screen.width, 48f),
                $"BOSS 攻击！ {PhaseTimeLeft:0.0}s", _warnStyle);
            GUI.color = prev;
        }
    }
}
