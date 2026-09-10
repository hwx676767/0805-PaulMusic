using TMPro;
using UnityEngine;

namespace DeusEx.Rhythm
{
    /// <summary>
    /// 判定弹出提示的通用动画 + 自动销毁：入场放大回弹、向上浮动、末段淡出，到时销毁自身。
    /// 与具体判定档位无关——四种判定各用各自的 prefab（图案/文字/颜色自定），把本脚本挂上即可获得统一的弹出手感。
    /// 全程在**局部空间**里做动画（localPosition / localScale），所以挂在 Playfield 下的子物体里时会自动贴合其旋转/缩放。
    /// 对所有子物体上的 TMP_Text 与 SpriteRenderer 生效（淡出其透明度）。
    /// 也可以不挂本脚本而用自己的 Animator/粒子：那样由 JudgementPopupSpawner 的 fallbackLifetime 兜底销毁。
    /// </summary>
    public class JudgementPopup : MonoBehaviour
    {
        [Header("生命周期")]
        [Tooltip("从出现到销毁的总时长（秒）。")]
        public float lifetime = 0.7f;

        [Header("上浮")]
        [Tooltip("整个生命周期内沿局部 +Y 方向上浮的距离（局部单位，会随父物体缩放）。设 0 则不移动。")]
        public float riseDistance = 0.6f;
        [Tooltip("上浮缓动：横轴 0~1 为时间进度，纵轴 0~1 为位移比例。默认先快后慢。")]
        public AnimationCurve riseCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("入场放大回弹")]
        [Tooltip("出现瞬间相对基准缩放的倍数（>1 表示放大入场，再收回到原大小）。设 1 则不缩放。")]
        public float popScale = 1.3f;
        [Tooltip("缩放回弹所占总时长的比例。")]
        [Range(0.01f, 1f)] public float popFraction = 0.25f;

        [Header("淡出")]
        [Tooltip("从该时间点（占总时长比例）开始线性淡出到全透明。设 1 则不淡出。")]
        [Range(0f, 1f)] public float fadeStart = 0.55f;

        private float _elapsed;
        private Vector3 _baseScale;
        private Vector3 _startLocalPos;
        private TMP_Text[] _texts;
        private float[] _textAlpha;
        private SpriteRenderer[] _sprites;
        private float[] _spriteAlpha;
        private bool _ready;
        private bool _begun;

        private void Start()
        {
            // Spawner 会在摆好位置后立刻 Begin()；若未调用则兜底启动，避免旧 prefab 卡住。
            if (!_begun) Begin();
        }

        /// <summary>
        /// 在摆好父级/位置后立即开播（不等下一帧 Start），消除一帧延迟。
        /// </summary>
        public void Begin()
        {
            _begun = true;
            _elapsed = 0f;
            _baseScale = transform.localScale;
            _startLocalPos = transform.localPosition;

            _texts = GetComponentsInChildren<TMP_Text>(true);
            _textAlpha = new float[_texts.Length];
            for (int i = 0; i < _texts.Length; i++) _textAlpha[i] = _texts[i].alpha;

            _sprites = GetComponentsInChildren<SpriteRenderer>(true);
            _spriteAlpha = new float[_sprites.Length];
            for (int i = 0; i < _sprites.Length; i++) _spriteAlpha[i] = _sprites[i].color.a;

            _ready = true;
            Apply(0f);
        }

        private void Update()
        {
            if (!_ready) return;
            _elapsed += Time.unscaledDeltaTime;
            float t = lifetime > 0f ? Mathf.Clamp01(_elapsed / lifetime) : 1f;
            Apply(t);
            if (_elapsed >= lifetime) Destroy(gameObject);
        }

        /// <summary>从头重播一次弹出动画（用于“判定与上一个相同则复用而非新建”）。以当前 localPosition 作为新起点，避免连续重播时位置漂移。</summary>
        public void Replay()
        {
            if (!_ready)
            {
                Begin();
                return;
            }
            _elapsed = 0f;
            _startLocalPos = transform.localPosition;
            Apply(0f);
        }

        private void Apply(float t)
        {
            transform.localPosition = _startLocalPos + Vector3.up * (riseCurve.Evaluate(t) * riseDistance);

            float popT = popFraction > 0f ? Mathf.Clamp01(t / popFraction) : 1f;
            transform.localScale = _baseScale * Mathf.Lerp(popScale, 1f, popT);

            float a = t < fadeStart
                ? 1f
                : (fadeStart < 1f ? 1f - (t - fadeStart) / (1f - fadeStart) : 0f);
            a = Mathf.Clamp01(a);

            if (_texts != null)
                for (int i = 0; i < _texts.Length; i++)
                    if (_texts[i] != null) _texts[i].alpha = _textAlpha[i] * a;
            if (_sprites != null)
                for (int i = 0; i < _sprites.Length; i++)
                    if (_sprites[i] != null)
                    {
                        var c = _sprites[i].color;
                        c.a = _spriteAlpha[i] * a;
                        _sprites[i].color = c;
                    }
        }
    }
}
