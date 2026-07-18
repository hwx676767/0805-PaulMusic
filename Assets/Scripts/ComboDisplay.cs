using TMPro;
using UnityEngine;

namespace DeusEx.Rhythm
{
    /// <summary>
    /// 把 RhythmGameManager 的 combo 显示到一个 TextMeshPro 文本上（UI 或世界空间均可）。
    /// 订阅 manager.ComboChanged 事件更新数字，并在变化时做一个轻微的放大回弹（juice）。
    /// 用法：在你的 TMP 文本物体上挂这个脚本（label 不填会自动取同物体上的 TMP_Text）。
    /// </summary>
    public class ComboDisplay : MonoBehaviour
    {
        [Tooltip("留空则自动在场景中查找 RhythmGameManager。")]
        public RhythmGameManager manager;

        [Tooltip("要驱动的 TMP 文本。留空则取本物体上的 TMP_Text。")]
        public TMP_Text label;

        [Tooltip("combo 低于该值时隐藏文本（典型音游 0~1 连击不显示）。")]
        public int hideBelow = 2;

        [Tooltip("文本格式，{0} 为 combo 数字。例如 \"{0}\" 或 \"{0} COMBO\"。")]
        public string format = "{0}";

        [Header("变化回弹动画")]
        public bool punchOnChange = true;
        [Tooltip("变化瞬间放大的倍数。")]
        public float punchScale = 1.25f;
        [Tooltip("回弹恢复速度（越大恢复越快）。")]
        public float recoverSpeed = 9f;

        private Vector3 _baseScale;

        private void Awake()
        {
            if (label == null) label = GetComponent<TMP_Text>();
            _baseScale = transform.localScale;
        }

        private void OnEnable()
        {
            if (manager == null) manager = FindObjectOfType<RhythmGameManager>();
            if (manager != null) manager.ComboChanged += OnComboChanged;
            Refresh(manager != null ? manager.Combo : 0);
        }

        private void OnDisable()
        {
            if (manager != null) manager.ComboChanged -= OnComboChanged;
        }

        private void OnComboChanged(int combo)
        {
            Refresh(combo);
            // 仅在 combo 增长（可见状态）时回弹，归零时不弹
            if (punchOnChange && combo >= hideBelow)
                transform.localScale = _baseScale * punchScale;
        }

        private void Update()
        {
            if (punchOnChange && transform.localScale != _baseScale)
                transform.localScale = Vector3.Lerp(transform.localScale, _baseScale, Time.deltaTime * recoverSpeed);
        }

        private void Refresh(int combo)
        {
            if (label == null) return;
            bool show = combo >= hideBelow;
            label.enabled = show;
            if (show) label.text = string.Format(format, combo);
        }
    }
}
