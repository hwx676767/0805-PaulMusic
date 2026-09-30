using UnityEngine;
using UnityEngine.UI;

namespace DeusEx.Rhythm
{
    /// <summary>
    /// 柔和屏幕打击反馈：
    /// - Miss / 玩家受伤：震动 + 闪红
    /// - 命中音符（打中 Boss）：震动 + 闪白
    /// Miss 后进入较长冷却，避免连 Miss 诱发不适。
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ScreenImpactFx : MonoBehaviour
    {
        [Header("引用（留空自动找）")]
        public RhythmGameManager manager;
        public BossBehavior boss;
        public CardSystem cardSystem;
        public Camera targetCamera;

        [Header("震动（偏轻）")]
        public float hitShakeAmplitude = 0.1f;
        public float attackShakeAmplitude = 0.07f;
        public float shakeDuration = 0.26f;
        public float shakeFrequency = 14f;

        [Header("闪屏（低对比、慢淡出）")]
        public Color hitFlashColor = new Color(0.85f, 0.2f, 0.18f, 0.16f);
        public Color attackFlashColor = new Color(0.82f, 0.92f, 1f, 0.12f);
        [Tooltip("淡出总时长；越长越柔和。")]
        public float flashDuration = 0.42f;

        [Header("冷却")]
        [Tooltip("Miss 触发后，这段时间内不再触发任何屏幕打击反馈。")]
        public float missCooldownSeconds = 3f;
        [Tooltip("命中（白闪）最短间隔。")]
        public float hitCooldownSeconds = 0.85f;

        private Image _flashImage;
        private float _flash;
        private Color _flashColor = Color.white;
        private float _shakeLeft;
        private float _shakeAmp;
        private Vector3 _camBaseLocalPos;
        private bool _hasCamBase;
        private Transform _camTransform;
        private float _cooldownLeft;

        private void OnEnable()
        {
            if (manager == null) manager = FindObjectOfType<RhythmGameManager>();
            if (boss == null) boss = FindObjectOfType<BossBehavior>();
            if (cardSystem == null) cardSystem = FindObjectOfType<CardSystem>();

            if (manager != null) manager.Judged += OnJudged;
            if (boss != null) boss.AttackExecuted += OnBossUltimate;
            if (cardSystem != null) cardSystem.CardPlayed += OnCardUltimate;

            EnsureFlashOverlay();
            CacheCamera();
        }

        private void OnDisable()
        {
            if (manager != null) manager.Judged -= OnJudged;
            if (boss != null) boss.AttackExecuted -= OnBossUltimate;
            if (cardSystem != null) cardSystem.CardPlayed -= OnCardUltimate;
            RestoreCamera();
        }

        private void LateUpdate()
        {
            float dt = Time.unscaledDeltaTime;
            if (_cooldownLeft > 0f) _cooldownLeft -= dt;

            if (_flash > 0f)
            {
                float step = dt / Mathf.Max(0.08f, flashDuration);
                _flash = Mathf.MoveTowards(_flash, 0f, step);
                float soft = _flash * _flash;
                if (_flashImage != null)
                {
                    var c = _flashColor;
                    c.a = _flashColor.a * soft;
                    _flashImage.color = c;
                    _flashImage.enabled = soft > 0.01f;
                }
            }
            else if (_flashImage != null && _flashImage.enabled)
            {
                _flashImage.enabled = false;
            }

            if (_shakeLeft > 0f && _camTransform != null)
            {
                if (!_hasCamBase)
                {
                    _camBaseLocalPos = _camTransform.localPosition;
                    _hasCamBase = true;
                }

                _shakeLeft -= dt;
                float t = Mathf.Clamp01(_shakeLeft / Mathf.Max(0.05f, shakeDuration));
                float damp = t * t;
                float ox = (Mathf.PerlinNoise(Time.unscaledTime * shakeFrequency, 0.3f) - 0.5f) * 2f;
                float oy = (Mathf.PerlinNoise(1.7f, Time.unscaledTime * shakeFrequency) - 0.5f) * 2f;
                _camTransform.localPosition = _camBaseLocalPos + new Vector3(ox, oy, 0f) * (_shakeAmp * damp);

                if (_shakeLeft <= 0f)
                    RestoreCamera();
            }
        }

        private void OnJudged(Judgment j)
        {
            if (_cooldownLeft > 0f) return;

            if (j == Judgment.Miss)
            {
                Trigger(hitFlashColor, hitShakeAmplitude);
                _cooldownLeft = missCooldownSeconds;
                return;
            }

            // Perfect / Great / Good → 柔和白闪
            Trigger(attackFlashColor, attackShakeAmplitude);
            _cooldownLeft = hitCooldownSeconds;
        }

        private void OnBossUltimate()
        {
            if (_cooldownLeft > 0f) return;
            Trigger(hitFlashColor, hitShakeAmplitude);
            _cooldownLeft = missCooldownSeconds;
        }

        private void OnCardUltimate(CardInstance _)
        {
            if (_cooldownLeft > 0f) return;
            Trigger(attackFlashColor, attackShakeAmplitude);
            _cooldownLeft = hitCooldownSeconds;
        }

        public void Trigger(Color flash, float shakeAmp)
        {
            _flashColor = flash;
            _flash = 1f;
            EnsureFlashOverlay();
            if (_flashImage != null)
            {
                var c = flash;
                c.a = flash.a * 0.4f;
                _flashImage.color = c;
                _flashImage.enabled = true;
            }

            CacheCamera();
            if (_camTransform != null)
            {
                if (!_hasCamBase || _shakeLeft <= 0f)
                {
                    _camBaseLocalPos = _camTransform.localPosition;
                    _hasCamBase = true;
                }
                _shakeAmp = shakeAmp;
                _shakeLeft = shakeDuration;
            }
        }

        private void CacheCamera()
        {
            if (targetCamera == null) targetCamera = Camera.main;
            if (targetCamera == null) targetCamera = FindObjectOfType<Camera>();
            _camTransform = targetCamera != null ? targetCamera.transform : null;
        }

        private void RestoreCamera()
        {
            if (_camTransform != null && _hasCamBase)
                _camTransform.localPosition = _camBaseLocalPos;
            _shakeLeft = 0f;
            _shakeAmp = 0f;
        }

        private void EnsureFlashOverlay()
        {
            if (_flashImage != null) return;

            var canvasGo = new GameObject("ScreenImpactCanvas", typeof(Canvas), typeof(CanvasScaler));
            canvasGo.transform.SetParent(transform, false);
            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 9000;

            var flashGo = new GameObject("ImpactFlash", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            flashGo.transform.SetParent(canvasGo.transform, false);
            var rect = flashGo.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            _flashImage = flashGo.GetComponent<Image>();
            _flashImage.color = new Color(1f, 1f, 1f, 0f);
            _flashImage.raycastTarget = false;
            _flashImage.enabled = false;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AutoEnsure()
        {
            if (FindObjectOfType<ScreenImpactFx>() != null) return;
            if (FindObjectOfType<RhythmGameManager>() == null) return;
            var go = new GameObject("ScreenImpactFx");
            go.AddComponent<ScreenImpactFx>();
        }
    }
}
