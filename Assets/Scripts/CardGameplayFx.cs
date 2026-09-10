using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DeusEx.Rhythm
{
    /// <summary>
    /// 卡牌画面/音效反馈：
    /// - 出牌瞬间闪光
    /// - 加速：屏幕彩虹外框
    /// - 减速：色差边 + lofi 颗粒暗角 + 音频低通
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class CardGameplayFx : MonoBehaviour
    {
        [Header("引用（留空自动找）")]
        public CardSystem cardSystem;
        public RhythmGameManager manager;

        [Header("覆盖层")]
        public Material overlayMaterial;
        [Range(0.02f, 0.2f)] public float borderWidth = 0.075f;
        public float flashSeconds = 0.45f;

        [Header("加速")]
        [Tooltip("PlaybackSpeed 超过此值视为加速。")]
        public float speedUpThreshold = 1.05f;
        public float speedUpBlend = 1f;

        [Header("减速 / Lofi")]
        [Tooltip("PlaybackSpeed 低于此值视为减速。")]
        public float slowDownThreshold = 0.95f;
        public float slowDownBlend = 1f;
        public float lofiCutoffHz = 1600f;
        public float normalCutoffHz = 22000f;

        private Image _overlayImage;
        private Material _matInstance;
        private float _flash;
        private Color _flashColor = new Color(1f, 0.92f, 0.55f, 1f);
        private AudioLowPassFilter _lowPass;
        private AudioSource _audio;

        private void OnEnable()
        {
            if (cardSystem == null) cardSystem = FindObjectOfType<CardSystem>();
            if (manager == null) manager = FindObjectOfType<RhythmGameManager>();
            if (cardSystem != null) cardSystem.CardPlayed += OnCardPlayed;
            EnsureOverlay();
            EnsureAudioFilter();
            EnsureBossGlitch();
        }

        private void OnDisable()
        {
            if (cardSystem != null) cardSystem.CardPlayed -= OnCardPlayed;
            if (_lowPass != null) _lowPass.cutoffFrequency = normalCutoffHz;
        }

        private void OnDestroy()
        {
            if (_matInstance != null) Destroy(_matInstance);
        }

        private void Update()
        {
            if (_lowPass == null) EnsureAudioFilter();
            if (_matInstance == null) EnsureOverlay();
            if (_matInstance == null) return;

            double speed = manager != null ? manager.PlaybackSpeed : 1.0;
            float speedUp = 0f;
            float slowDown = 0f;
            if (speed >= speedUpThreshold)
                speedUp = Mathf.Clamp01(((float)speed - 1f) / 0.35f) * speedUpBlend;
            else if (speed <= slowDownThreshold)
                slowDown = Mathf.Clamp01((1f - (float)speed) / 0.35f) * slowDownBlend;

            if (_flash > 0f)
                _flash = Mathf.MoveTowards(_flash, 0f, Time.unscaledDeltaTime / Mathf.Max(0.05f, flashSeconds));

            _matInstance.SetFloat("_SpeedUp", speedUp);
            _matInstance.SetFloat("_SlowDown", slowDown);
            _matInstance.SetFloat("_PlayFlash", _flash);
            _matInstance.SetColor("_FlashColor", _flashColor);
            _matInstance.SetFloat("_BorderWidth", borderWidth);

            if (_overlayImage != null)
                _overlayImage.enabled = speedUp > 0.01f || slowDown > 0.01f || _flash > 0.01f;

            if (_lowPass != null)
            {
                float target = Mathf.Lerp(normalCutoffHz, lofiCutoffHz, Mathf.Clamp01(slowDown));
                _lowPass.cutoffFrequency = Mathf.MoveTowards(
                    _lowPass.cutoffFrequency, target, Time.unscaledDeltaTime * 12000f);
            }
        }

        private void OnCardPlayed(CardInstance card)
        {
            _flash = 1f;
            var def = card != null ? card.definition : null;
            _flashColor = def != null ? Color.Lerp(def.color, Color.white, 0.35f) : new Color(1f, 0.92f, 0.55f, 1f);

            // 加速/减速牌用更有指向性的闪光色
            if (def != null && def.effects != null)
            {
                for (int i = 0; i < def.effects.Count; i++)
                {
                    if (def.effects[i].type != CardEffectType.SongSpeedMultiplier) continue;
                    if (def.effects[i].magnitude > 1f)
                        _flashColor = new Color(1f, 0.45f, 0.85f, 1f);
                    else if (def.effects[i].magnitude < 1f)
                        _flashColor = new Color(0.45f, 0.75f, 1f, 1f);
                }
            }
        }

        private void EnsureOverlay()
        {
            if (_overlayImage != null && _matInstance != null) return;

            Material source = overlayMaterial;
#if UNITY_EDITOR
            if (source == null)
                source = AssetDatabase.LoadAssetAtPath<Material>("Assets/Scripts/Shaders/CardFxOverlay.mat");
#endif
            if (source == null)
            {
                Shader shader = Shader.Find("DeusEx/CardFxOverlay");
                if (shader == null) return;
                source = new Material(shader) { name = "CardFxOverlay (Runtime)" };
                overlayMaterial = source;
            }

            _matInstance = new Material(source) { name = source.name + " (Instance)" };

            var canvasGo = new GameObject("CardFxOverlayCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler));
            canvasGo.transform.SetParent(transform, false);
            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 180;
            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);

            var imgGo = new GameObject("Fx", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            imgGo.transform.SetParent(canvasGo.transform, false);
            var rect = imgGo.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            _overlayImage = imgGo.GetComponent<Image>();
            _overlayImage.raycastTarget = false;
            _overlayImage.color = Color.white;
            _overlayImage.material = _matInstance;
            _overlayImage.enabled = false;
        }

        private void EnsureAudioFilter()
        {
            if (_lowPass != null) return;
            if (manager == null) manager = FindObjectOfType<RhythmGameManager>();
            if (manager == null) return;

            _audio = manager.GetComponent<AudioSource>();
            if (_audio == null) _audio = manager.GetComponentInChildren<AudioSource>();
            if (_audio == null) return;

            _lowPass = _audio.GetComponent<AudioLowPassFilter>();
            if (_lowPass == null) _lowPass = _audio.gameObject.AddComponent<AudioLowPassFilter>();
            _lowPass.cutoffFrequency = normalCutoffHz;
            _lowPass.lowpassResonanceQ = 1.2f;
        }

        private static void EnsureBossGlitch()
        {
            var bosses = FindObjectsOfType<BossAnimationDriver>();
            for (int i = 0; i < bosses.Length; i++)
            {
                if (bosses[i] == null) continue;
                if (bosses[i].GetComponent<BossGlitchDriver>() == null)
                    bosses[i].gameObject.AddComponent<BossGlitchDriver>();
            }

            // TitleBoss 也可能要
            var title = GameObject.Find("TitleBoss");
            if (title != null && title.GetComponent<BossGlitchDriver>() == null
                && title.GetComponent<SpriteRenderer>() != null)
                title.AddComponent<BossGlitchDriver>();
        }
    }
}
