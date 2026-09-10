using UnityEngine;

namespace DeusEx.Rhythm
{
    /// <summary>
    /// 战斗机制：玩家与敌人各有一条血。玩家每命中一个音符就对敌人造成伤害；每 Miss 一次就对自己造成伤害。
    /// 任一方血量归零即分出胜负（可选自动停歌）。
    ///
    /// 实现上通过订阅 <see cref="RhythmGameManager.Judged"/> 事件工作，与音游判定核心解耦
    /// （写法参考 <see cref="ComboDisplay"/>），因此挂上即用、无需改动核心逻辑。
    /// 零场景布置也能跑：勾选 showDebugBars 即用 IMGUI 画出两条血条；正式 UI 建议改挂
    /// <see cref="HealthBar"/> 组件订阅本类的血量事件。
    ///
    /// 用法：在场景里任意 GameObject（例如挂着 RhythmGameManager 的那个）上添加本组件即可。
    /// </summary>
    public class CombatSystem : MonoBehaviour
    {
        [Header("引用")]
        [Tooltip("留空则自动在场景中查找 RhythmGameManager。")]
        public RhythmGameManager manager;

        [Header("最大血量")]
        public float playerMaxHealth = 100f;
        public float enemyMaxHealth = 100f;

        [Header("命中对敌人造成的伤害（按判定档位）")]
        [Tooltip("Perfect 判定对敌人造成的伤害。")]
        public float perfectDamage = 5f;
        [Tooltip("Great 判定对敌人造成的伤害。")]
        public float greatDamage = 3.5f;
        [Tooltip("Good 判定对敌人造成的伤害。")]
        public float goodDamage = 2f;

        [Header("Miss 对玩家造成的伤害")]
        public float missDamage = 6f;

        [Header("行为")]
        [Tooltip("任一方血量归零时停止歌曲（游戏结束）。Demo 录屏可关掉。")]
        public bool stopSongOnDefeat = true;
        [Tooltip("任一方血量归零时是否正式结束对局（锁伤害、触发胜负事件）。Demo 阶段可关掉以便一直打、一直记数据。")]
        public bool endRunOnDefeat = true;

        [Header("调试显示（默认关闭，正式 UI 请改用 HealthBar 组件订阅事件）")]
        [Tooltip("勾选后用 IMGUI 在屏幕顶/底画出敌人/玩家血条与胜负提示，方便零布置直接测试。")]
        public bool showDebugBars = false;

        // ---- 运行时只读状态 ----
        public float PlayerHealth { get; private set; }
        public float EnemyHealth { get; private set; }
        public float PlayerMaxHealth => playerMaxHealth;
        public float EnemyMaxHealth => enemyMaxHealth;
        /// <summary>是否已分出胜负（结束后不再结算伤害）。</summary>
        public bool IsOver { get; private set; }
        /// <summary>胜负结果：true=玩家胜（敌人倒下），false=玩家败。仅在 IsOver 时有意义。</summary>
        public bool PlayerWon { get; private set; }

        /// <summary>
        /// 命中对敌人造成伤害的整体倍率（1=原始，1.5=攻击力提升 50%）。供卡牌"攻击力x1.5"效果临时提升，
        /// 由 <see cref="CardEffectRunner"/> 设置。仅作用于命中音符对敌人的伤害。
        /// </summary>
        public float DamageDealtMultiplier { get; set; } = 1f;

        /// <summary>
        /// 玩家受到伤害的整体倍率（1=原始，0=免疫）。由 <see cref="CardEffectRunner"/> 设置。
        /// 作用于 Miss 与 Boss 攻击；护盾会先于倍率结算。
        /// </summary>
        public float DamageTakenMultiplier { get; set; } = 1f;

        /// <summary>当前护盾层数：每层抵消一次即将到来的玩家受伤。</summary>
        public int ShieldCharges { get; private set; }

        // ---- 对外事件，供 UI（如 HealthBar）订阅，避免直接耦合 ----
        /// <summary>玩家血量变化时触发，参数为 (当前血量, 最大血量)，便于 UI 直接算填充比例。</summary>
        public event System.Action<float, float> PlayerHealthChanged;
        /// <summary>敌人血量变化时触发，参数为 (当前血量, 最大血量)。</summary>
        public event System.Action<float, float> EnemyHealthChanged;
        /// <summary>护盾层数变化时触发。</summary>
        public event System.Action<int> ShieldChargesChanged;
        /// <summary>玩家血量归零（战败）时触发一次。</summary>
        public event System.Action PlayerDied;
        /// <summary>敌人血量归零（战胜）时触发一次。</summary>
        public event System.Action EnemyDefeated;

        private GUIStyle _barLabel;
        private GUIStyle _bigLabel;

        private void Awake()
        {
            PlayerHealth = playerMaxHealth;
            EnemyHealth = enemyMaxHealth;
        }

        private void OnEnable()
        {
            if (manager == null) manager = FindObjectOfType<RhythmGameManager>();
            if (manager != null) manager.Judged += OnJudged;
        }

        private void OnDisable()
        {
            if (manager != null) manager.Judged -= OnJudged;
        }

        private void Start()
        {
            // 通知 UI 初始满血（HealthBar 在 OnEnable 里也会主动拉一次，这里双保险）
            PlayerHealthChanged?.Invoke(PlayerHealth, playerMaxHealth);
            EnemyHealthChanged?.Invoke(EnemyHealth, enemyMaxHealth);
        }

        private void OnJudged(Judgment j)
        {
            if (IsOver) return;

            if (j == Judgment.Miss)
            {
                DamagePlayer(missDamage);
            }
            else
            {
                float dmg = j switch
                {
                    Judgment.Perfect => perfectDamage,
                    Judgment.Great => greatDamage,
                    _ => goodDamage,
                };
                DamageEnemy(dmg * DamageDealtMultiplier);
            }
        }

        /// <summary>对敌人造成伤害（也可被技能等外部逻辑调用）。</summary>
        public void DamageEnemy(float amount)
        {
            if (IsOver || amount <= 0f) return;
            EnemyHealth = Mathf.Max(0f, EnemyHealth - amount);
            EnemyHealthChanged?.Invoke(EnemyHealth, enemyMaxHealth);
            if (EnemyHealth <= 0f && endRunOnDefeat) EndGame(playerWon: true);
        }

        /// <summary>对玩家造成伤害（先消耗护盾，再乘以 <see cref="DamageTakenMultiplier"/>）。</summary>
        public void DamagePlayer(float amount)
        {
            if (IsOver || amount <= 0f) return;

            if (ShieldCharges > 0)
            {
                ShieldCharges--;
                ShieldChargesChanged?.Invoke(ShieldCharges);
                return;
            }

            amount *= DamageTakenMultiplier;
            if (amount <= 0f) return;

            PlayerHealth = Mathf.Max(0f, PlayerHealth - amount);
            PlayerHealthChanged?.Invoke(PlayerHealth, playerMaxHealth);
            if (PlayerHealth <= 0f && endRunOnDefeat) EndGame(playerWon: false);
        }

        /// <summary>给玩家回血（上限为最大血量）。</summary>
        public void HealPlayer(float amount)
        {
            if (IsOver || amount <= 0f) return;
            PlayerHealth = Mathf.Min(playerMaxHealth, PlayerHealth + amount);
            PlayerHealthChanged?.Invoke(PlayerHealth, playerMaxHealth);
        }

        /// <summary>增加护盾层数（每层抵消一次受伤）。</summary>
        public void AddShieldCharges(int charges)
        {
            if (IsOver || charges <= 0) return;
            ShieldCharges += charges;
            ShieldChargesChanged?.Invoke(ShieldCharges);
        }

        /// <summary>清空护盾层数（卡牌减伤到期或强制驱散时用）。</summary>
        public void ClearShieldCharges()
        {
            if (ShieldCharges == 0) return;
            ShieldCharges = 0;
            ShieldChargesChanged?.Invoke(ShieldCharges);
        }

        private void EndGame(bool playerWon)
        {
            if (IsOver) return;
            IsOver = true;
            PlayerWon = playerWon;
            if (stopSongOnDefeat && manager != null) manager.StopSong();
            if (playerWon) EnemyDefeated?.Invoke();
            else PlayerDied?.Invoke();
        }

        // ---------------- 调试血条（IMGUI，零布置即可看到效果） ----------------

        private void OnGUI()
        {
            if (!showDebugBars) return;

            if (_barLabel == null)
            {
                _barLabel = new GUIStyle(GUI.skin.label) { fontSize = 15, fontStyle = FontStyle.Bold };
                _bigLabel = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 52,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter
                };
            }

            float w = Mathf.Min(440f, Screen.width - 40f);
            float h = 24f;
            float x = (Screen.width - w) * 0.5f;

            DrawBar(new Rect(x, 16f, w, h), "敌人", EnemyHealth, enemyMaxHealth, new Color(0.9f, 0.3f, 0.3f));
            string playerLabel = ShieldCharges > 0 ? $"玩家 盾x{ShieldCharges}" : "玩家";
            DrawBar(new Rect(x, Screen.height - 16f - h, w, h), playerLabel, PlayerHealth, playerMaxHealth, new Color(0.3f, 0.85f, 0.45f));

            if (IsOver)
            {
                var prev = GUI.color;
                GUI.color = PlayerWon ? new Color(1f, 0.9f, 0.4f) : new Color(1f, 0.4f, 0.4f);
                GUI.Label(new Rect(0, Screen.height * 0.42f, Screen.width, 80),
                    PlayerWon ? "胜利！" : "败北…", _bigLabel);
                GUI.color = prev;
            }
        }

        private void DrawBar(Rect r, string name, float hp, float max, Color fill)
        {
            float pct = max > 0f ? Mathf.Clamp01(hp / max) : 0f;

            var prev = GUI.color;
            GUI.color = new Color(0f, 0f, 0f, 0.55f);
            GUI.DrawTexture(r, Texture2D.whiteTexture);
            GUI.color = fill;
            GUI.DrawTexture(new Rect(r.x, r.y, r.width * pct, r.height), Texture2D.whiteTexture);
            GUI.color = prev;

            GUI.Label(new Rect(r.x + 8f, r.y + 2f, r.width, r.height),
                $"{name}  {Mathf.CeilToInt(hp)} / {Mathf.CeilToInt(max)}", _barLabel);
        }
    }
}
