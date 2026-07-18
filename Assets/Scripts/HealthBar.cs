using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DeusEx.Rhythm
{
    /// <summary>
    /// 把 <see cref="CombatSystem"/> 的玩家或敌人血量显示到一条 UI 血条上。
    /// 订阅对应的血量事件更新填充比例，并可选地把数值写到一个 TMP 文本（写法参考 <see cref="ComboDisplay"/>）。
    ///
    /// 支持两种血条载体（按需任选其一或同时使用）：
    /// - Slider：拖入 slider 字段即可，组件会把它的 min/max 锁定为 0~1，并用 value 表现血量比例；
    ///   建议在 Inspector 把 Slider 的 Interactable 取消勾选（纯显示，不让玩家拖动）。
    /// - Image：把 Image 的 Image Type 设为 Filled、Fill Method 选 Horizontal，拖入 fillImage 字段。
    ///
    /// 用法：在血条物体（或任意物体）上挂本组件，选好 target（玩家/敌人）、拖入 slider 或 fillImage
    /// （可选再拖一个 TMP 文本显示数值）。combat 留空会自动在场景中查找 CombatSystem。
    /// </summary>
    public class HealthBar : MonoBehaviour
    {
        public enum Side { Player, Enemy }

        [Tooltip("留空则自动在场景中查找 CombatSystem。")]
        public CombatSystem combat;

        [Tooltip("这条血条表示玩家还是敌人。")]
        public Side target = Side.Player;

        [Tooltip("用 value 表现血量的 Slider（其 min/max 会被自动锁定为 0~1，建议取消 Interactable）。")]
        public Slider slider;

        [Tooltip("可选：用 fillAmount 表现血量的 Image（Image Type 需设为 Filled）。")]
        public Image fillImage;

        [Tooltip("可选：显示数值的 TMP 文本。")]
        public TMP_Text label;

        [Tooltip("数值文本格式：{0}=当前血量(取整)，{1}=最大血量(取整)，{2}=百分比(0~100 取整)。")]
        public string format = "{0}/{1}";

        [Header("平滑")]
        [Tooltip("勾选后血条填充平滑过渡，否则瞬间跳变。")]
        public bool smooth = true;
        [Tooltip("平滑过渡速度（越大越快跟上目标值）。")]
        public float smoothSpeed = 8f;

        private float _targetFill = 1f; // 目标填充比例 0~1
        private float _shownFill = 1f;  // 当前显示比例 0~1

        private void OnEnable()
        {
            // 用归一化的 0~1 驱动 Slider，锁定其取值范围避免被 Inspector 里的设置干扰
            if (slider != null)
            {
                slider.minValue = 0f;
                slider.maxValue = 1f;
                slider.wholeNumbers = false;
            }

            if (combat == null) combat = FindObjectOfType<CombatSystem>();
            if (combat == null) return;

            if (target == Side.Player)
            {
                combat.PlayerHealthChanged += OnHealthChanged;
                OnHealthChanged(combat.PlayerHealth, combat.PlayerMaxHealth);
            }
            else
            {
                combat.EnemyHealthChanged += OnHealthChanged;
                OnHealthChanged(combat.EnemyHealth, combat.EnemyMaxHealth);
            }
        }

        private void OnDisable()
        {
            if (combat == null) return;
            if (target == Side.Player) combat.PlayerHealthChanged -= OnHealthChanged;
            else combat.EnemyHealthChanged -= OnHealthChanged;
        }

        private void OnHealthChanged(float current, float max)
        {
            _targetFill = max > 0f ? Mathf.Clamp01(current / max) : 0f;
            if (!smooth) _shownFill = _targetFill;

            if (label != null)
            {
                float pct = _targetFill * 100f;
                label.text = string.Format(format,
                    Mathf.CeilToInt(current), Mathf.CeilToInt(max), Mathf.RoundToInt(pct));
            }

            ApplyFill();
        }

        private void Update()
        {
            if (!smooth || Mathf.Approximately(_shownFill, _targetFill)) return;
            _shownFill = Mathf.MoveTowards(_shownFill, _targetFill, Time.deltaTime * smoothSpeed);
            ApplyFill();
        }

        private void ApplyFill()
        {
            if (slider != null) slider.value = _shownFill;
            if (fillImage != null) fillImage.fillAmount = _shownFill;
        }
    }
}
