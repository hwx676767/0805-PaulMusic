using UnityEngine;
using UnityEngine.UI;

namespace DeusEx.Rhythm
{
    /// <summary>
    /// 低频、柔和的大事件屏幕反馈（避免每次 Miss/命中狂闪）：
    /// - 红：仅 Boss 大招命中（<see cref="BossBehavior.AttackExecuted"/>），Miss 不触发
    /// - 白：仅玩家打出卡牌（大招），普通音符命中不触发
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ScreenImpactFx : MonoBehaviour
    {
        [Header("引用（留空自动找）")]
        public BossBehavior boss;
        public CardSystem cardSystem;
        public Camera targetCamera;

        [Header("震动（偏轻）")]
        public float hitShakeAmplitude = 0.08f;
        public float attackShakeAmplitude = 0.06f;
        public float shakeDuration = 0.28f;
        public float shakeFrequency = 14f;

        [Header("闪屏（低对比、慢淡出）")]
        public Color hitFlashColor = new Color(0.85f, 0.18f, 0.16f, 0.14f);
        public Color attackFlashColor = new Color(0.82f, 0.92f, 1f, 0.11f);
        [Tooltip("淡出总时长；越长越柔和。")]
        public float flashDuration = 0.45f;
        [Tooltip("同色/同类反馈最短间隔，防止连发刺眼。")]
        public float cooldownSeconds = 1.25f;

        private Image _flashImage;
        private float _flash;
        private Color _flashColor = Color.white;
        private float _shakeLeft;
        private float _shakeAmp;
        private Vector3 _camBaseLocalPos;
        private bool _hasCamBase;
        private Transform _camTransform;
        private float _hitCooldownLeft;
        private float _attackCooldownLeft;

        private void OnEnable()
        {
            if (boss == null) boss = FindObjectOfType<BossBehavior>();
            if (cardSystem == null) cardSystem = FindObjectOfType<CardSystem>();

            if (boss != null) boss.AttackExecuted += OnBossUltimate;
            if (cardSystem != null) cardSystem.CardPlayed += OnCardUltimate;

            EnsureFlashOverlay();
            CacheCamera();
        }

        private void OnDisable()
        {
            if (boss != null) boss.AttackExecuted -= OnBossUltimate;
            if (cardSystem != null) cardSystem.CardPlayed -= OnCardUltimate;
            RestoreCamera();
        }

        private void LateUpdate()
        {
            float dt = Time.unscaledDeltaTime;
            if (_hitCooldownLeft > 0f) _hitCooldownLeft -= dt;
            if (_attackCooldownLeft > 0f) _attackCooldownLeft -= dt;

            if (_flash > 0f)
            {
                // 先快后慢的淡出，避免硬切
                float step = dt / Mathf.Max(0.08f, flashDuration);
                _flash = Mathf.MoveTowards(_flash, 0f, step);
                float soft = _flash * _flash; // 峰值也更柔
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

        private void OnBossUltimate()
        {
            if (_hitCooldownLeft > 0f) return;
            _hitCooldownLeft = cooldownSeconds;
            Trigger(hitFlashColor, hitShakeAmplitude);
        }

        private void OnCardUltimate(CardInstance _)
        {
            if (_attackCooldownLeft > 0f) return;
            _attackCooldownLeft = cooldownSeconds;
            Trigger(attackFlashColor, attackShakeAmplitude);
        }

        public void Trigger(Color flash, float shakeAmp)
        {
            _flashColor = flash;
            _flash = 1f;
            EnsureFlashOverlay();
            if (_flashImage != null)
            {
                var c = flash;
                c.a = flash.a * 0.35f; // 起手也别满强度
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
            if (FindObjectOfType<BossBehavior>() == null && FindObjectOfType<CardSystem>() == null) return;
            var go = new GameObject("ScreenImpactFx");
            go.AddComponent<ScreenImpactFx>();
        }
    }
}
