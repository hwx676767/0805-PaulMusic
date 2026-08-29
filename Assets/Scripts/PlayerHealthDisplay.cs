using TMPro;
using UnityEngine;

namespace DeusEx.Rhythm
{
    /// <summary>
    /// 玩家血量 HUD：不占用 Boss 的 HealthBar/Slider，只读 <see cref="CombatSystem"/> 玩家侧事件。
    /// label 留空时会在本物体下自动建一个 TMP；也可挂到已有文本上。
    /// </summary>
    public class PlayerHealthDisplay : MonoBehaviour
    {
        [Tooltip("留空则自动查找 CombatSystem。")]
        public CombatSystem combat;

        [Tooltip("留空则在本物体下自动创建 TMP。")]
        public TMP_Text label;

        [Tooltip("{0}=当前血量 {1}=最大血量 {2}=护盾层数。")]
        public string format = "HP {0}/{1}";
        [Tooltip("有护盾时追加的后缀，{0}=护盾层数。")]
        public string shieldSuffix = "  ·  盾×{0}";

        private void Awake()
        {
            if (label == null) label = GetComponent<TMP_Text>();
            if (label == null) label = CreateLabel();
        }

        private void OnEnable()
        {
            if (combat == null) combat = FindFirstObjectByType<CombatSystem>();
            if (combat == null) return;
            combat.PlayerHealthChanged += OnHealthChanged;
            combat.ShieldChargesChanged += OnShieldChanged;
            Refresh(combat.PlayerHealth, combat.PlayerMaxHealth, combat.ShieldCharges);
        }

        private void OnDisable()
        {
            if (combat == null) return;
            combat.PlayerHealthChanged -= OnHealthChanged;
            combat.ShieldChargesChanged -= OnShieldChanged;
        }

        private void OnHealthChanged(float current, float max) =>
            Refresh(current, max, combat != null ? combat.ShieldCharges : 0);

        private void OnShieldChanged(int charges) =>
            Refresh(combat != null ? combat.PlayerHealth : 0f,
                combat != null ? combat.PlayerMaxHealth : 0f, charges);

        private void Refresh(float current, float max, int shields)
        {
            if (label == null) return;
            string text = string.Format(format, Mathf.CeilToInt(current), Mathf.CeilToInt(max), shields);
            if (shields > 0 && !string.IsNullOrEmpty(shieldSuffix))
                text += string.Format(shieldSuffix, shields);
            label.text = text;
        }

        private TMP_Text CreateLabel()
        {
            var go = new GameObject("PlayerHealthLabel", typeof(RectTransform));
            var rect = go.GetComponent<RectTransform>();
            rect.SetParent(transform, false);
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0f, 0f);
            rect.anchoredPosition = new Vector2(24f, 24f);
            rect.sizeDelta = new Vector2(420f, 48f);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.fontSize = 28f;
            tmp.fontStyle = FontStyles.Bold;
            tmp.alignment = TextAlignmentOptions.BottomLeft;
            tmp.color = new Color(0.45f, 1f, 0.65f, 1f);
            tmp.raycastTarget = false;
            return tmp;
        }
    }
}
