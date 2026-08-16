using System;
using System.Collections.Generic;
using UnityEngine;

namespace DeusEx.Rhythm
{
    /// <summary>
    /// 击中（或 Miss）音符时，在对应位置弹出一个判定提示。Perfect/Great/Good/Miss 各用一个独立 prefab。
    /// 订阅 RhythmGameManager.JudgedAt 事件，按判定档位实例化对应 prefab；默认弹在被击中轨道的判定线处。
    ///
    /// 想“贴合 Playfield”（跟随它的旋转/缩放/位移）：把 <see cref="spawnParent"/> 设为 Playfield 下的一个子物体
    /// （例如它里面的 World Space Canvas，或一个空物体）。弹出物会成为它的子物体并在其**局部空间**里定位，
    /// 于是自动继承整条父链（Playfield→Canvas）的朝向与缩放——无需再单独“对齐旋转”。
    /// </summary>
    public class JudgementPopupSpawner : MonoBehaviour
    {
        [Tooltip("留空则自动在场景中查找 RhythmGameManager。")]
        public RhythmGameManager manager;

        [Header("四种判定各用一个 Prefab（留空的档位不弹出）")]
        public GameObject perfectPrefab;
        public GameObject greatPrefab;
        public GameObject goodPrefab;
        public GameObject missPrefab;

        [Header("父物体（贴合 Playfield 的关键）")]
        [Tooltip("弹出物会作为它的子物体、在其局部空间里定位。设为 Playfield 下的子物体（如其中的 World Space Canvas）即可自动贴合 Playfield 的旋转/缩放。留空则放到场景根，不跟随 Playfield。")]
        public Transform spawnParent;

        [Header("生成位置")]
        [Tooltip("勾选：在被击中轨道的判定线处弹出；取消：固定在 spawnParent（无则本物体）原点弹出。")]
        public bool spawnAtHitLane = true;
        [Tooltip("在生成点基础上的额外偏移。有 spawnParent 时按其局部空间（贴合 Playfield 时 +Y 即“沿轨道向上”）；无父物体时按世界空间。")]
        public Vector3 spawnOffset = new Vector3(0f, 0.6f, 0f);

        [Header("同一时刻只保留一个提示")]
        [Tooltip("当本次判定与上一个相同、且上一个提示还在时，复用它并重播动画，而不是新建一个（避免相同判定在同一处越堆越多）。需要 prefab 上有 JudgementPopup 才能重播。")]
        public bool reuseWhenSameJudgment = true;
        [Tooltip("新建提示前，先把上一个立即销毁，保证“新提示出现时旧的已完全消失”，不会两个并存淡出。固定位置时建议开启。")]
        public bool clearPreviousOnNew = true;

        [Header("清理")]
        [Tooltip("当 prefab 上没有 JudgementPopup（自带销毁）时，过这么多秒后兜底销毁。设 0 表示不自动销毁、由你的 prefab 自理。")]
        public float fallbackLifetime = 1f;

        [Header("判定序列帧特效")]
        [Tooltip("命中后在对应轨道判定位置播放 Resources/effect 下的 PNG 序列帧。")]
        public bool spawnFrameEffects = true;
        [Tooltip("序列帧播放速度。30 FPS 时 Perfect 约 0.77 秒、Good 约 0.9 秒、Miss 约 1.5 秒。")]
        [Min(1f)] public float effectFramesPerSecond = 30f;
        [Tooltip("序列帧生成 Sprite 时使用的 Pixels Per Unit。素材为 1920×1080 透明画布，100 可让中心图案贴合判定点。")]
        [Min(1f)] public float effectPixelsPerUnit = 100f;
        [Tooltip("特效在结束前淡出的秒数。")]
        [Min(0f)] public float effectFadeOutSeconds = 0.12f;
        [Tooltip("特效的 SpriteRenderer 排序顺序，应高于音符（音符默认约为 20）。")]
        public int effectSortingOrder = 40;
        [Tooltip("Great 尚无独立序列帧时，复用 Good 特效。")]
        public bool useGoodEffectForGreat = true;

        private Judgment _lastJudgment;
        private GameObject _current;
        private JudgementPopup _currentPopup;
        private readonly Dictionary<string, Sprite[]> _effectFrames = new Dictionary<string, Sprite[]>();

        private void OnEnable()
        {
            if (manager == null) manager = FindObjectOfType<RhythmGameManager>();
            if (manager != null) manager.JudgedAt += OnJudgedAt;
        }

        private void OnDisable()
        {
            if (manager != null) manager.JudgedAt -= OnJudgedAt;
        }

        private void OnJudgedAt(JudgementInfo info)
        {
            GameObject prefab = PrefabFor(info.judgment);

            // 判定线世界坐标（已包含 Playfield 旋转/位移）；不贴轨道则用本组件位置
            Vector3 worldPos = spawnAtHitLane ? info.worldPosition : transform.position;

            if (spawnFrameEffects) SpawnFrameEffect(info.judgment, worldPos);
            if (prefab == null) return;

            // 与上一个判定相同且上一个提示还在：复用并重播，不新建（_current 销毁后 != null 为 false，会自然新建）
            if (reuseWhenSameJudgment && _current != null && _currentPopup != null && info.judgment == _lastJudgment)
            {
                Place(_current.transform, prefab.transform, worldPos);
                _currentPopup.Replay();
                return;
            }

            // 新建之前，先让上一个彻底消失，保证同一时刻只有一个提示（不并存淡出）
            if (clearPreviousOnNew && _current != null) Destroy(_current);

            GameObject inst = Instantiate(prefab);
            Place(inst.transform, prefab.transform, worldPos);

            _current = inst;
            _currentPopup = inst.GetComponentInChildren<JudgementPopup>(true);
            _lastJudgment = info.judgment;

            // prefab 自带 JudgementPopup 时由它自行销毁；否则按 fallbackLifetime 兜底，避免越积越多
            if (fallbackLifetime > 0f && _currentPopup == null)
                Destroy(inst, fallbackLifetime);
        }

        // 把实例摆到目标位置：有父物体则进入其局部空间（继承 Playfield 的旋转/缩放，从而“贴合”）
        private void Place(Transform t, Transform prefabT, Vector3 worldPos)
        {
            if (spawnParent != null)
            {
                t.SetParent(spawnParent, false);
                t.localRotation = prefabT.localRotation;
                t.localScale = prefabT.localScale;
                t.localPosition = spawnParent.InverseTransformPoint(worldPos) + spawnOffset;
            }
            else
            {
                t.SetParent(null, false);
                t.rotation = prefabT.rotation;
                t.localScale = prefabT.localScale;
                t.position = worldPos + spawnOffset;
            }
        }

        private GameObject PrefabFor(Judgment j) => j switch
        {
            Judgment.Perfect => perfectPrefab,
            Judgment.Great => greatPrefab,
            Judgment.Good => goodPrefab,
            _ => missPrefab,
        };

        private void SpawnFrameEffect(Judgment judgment, Vector3 worldPos)
        {
            string path = EffectPathFor(judgment);
            if (string.IsNullOrEmpty(path)) return;

            Sprite[] frames = LoadEffectFrames(path);
            if (frames.Length == 0)
            {
                Debug.LogWarning($"[Rhythm] 未找到判定特效序列帧：Resources/{path}");
                return;
            }

            var effect = new GameObject($"JudgementEffect_{judgment}", typeof(SpriteRenderer), typeof(JudgementFrameEffect));
            Quaternion rotation = manager != null && manager.playfield != null
                ? manager.playfield.transform.rotation
                : Quaternion.identity;
            effect.transform.SetPositionAndRotation(worldPos + spawnOffset, rotation);

            var renderer = effect.GetComponent<SpriteRenderer>();
            renderer.sortingOrder = effectSortingOrder;
            renderer.color = Color.white;

            effect.GetComponent<JudgementFrameEffect>().Initialize(renderer, frames, effectFramesPerSecond, effectFadeOutSeconds);
        }

        private string EffectPathFor(Judgment judgment) => judgment switch
        {
            Judgment.Perfect => "effect/perfect",
            Judgment.Good => "effect/good",
            Judgment.Miss => "effect/missed",
            Judgment.Great when useGoodEffectForGreat => "effect/good",
            _ => null,
        };

        private Sprite[] LoadEffectFrames(string path)
        {
            if (_effectFrames.TryGetValue(path, out Sprite[] cached)) return cached;

            Texture2D[] textures = Resources.LoadAll<Texture2D>(path);
            Array.Sort(textures, (a, b) => string.CompareOrdinal(a.name, b.name));

            var frames = new List<Sprite>(textures.Length);
            foreach (Texture2D texture in textures)
            {
                if (texture == null) continue;
                frames.Add(Sprite.Create(
                    texture,
                    new Rect(0f, 0f, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f),
                    effectPixelsPerUnit,
                    0,
                    SpriteMeshType.FullRect));
            }

            cached = frames.ToArray();
            _effectFrames[path] = cached;
            return cached;
        }
    }

    /// <summary>
    /// Plays one transparent PNG frame sequence on a SpriteRenderer, then destroys itself.
    /// Frames are supplied by <see cref="JudgementPopupSpawner"/> from Resources.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class JudgementFrameEffect : MonoBehaviour
    {
        private SpriteRenderer _renderer;
        private Sprite[] _frames;
        private float _framesPerSecond;
        private float _fadeOutSeconds;
        private float _elapsed;
        private int _frameIndex = -1;

        public void Initialize(SpriteRenderer renderer, Sprite[] frames, float framesPerSecond, float fadeOutSeconds)
        {
            _renderer = renderer;
            _frames = frames;
            _framesPerSecond = Mathf.Max(1f, framesPerSecond);
            _fadeOutSeconds = Mathf.Max(0f, fadeOutSeconds);

            if (_renderer == null || _frames == null || _frames.Length == 0)
            {
                Destroy(gameObject);
                return;
            }

            ShowFrame(0);
        }

        private void Update()
        {
            if (_renderer == null || _frames == null || _frames.Length == 0) return;

            _elapsed += Time.unscaledDeltaTime;
            float duration = _frames.Length / _framesPerSecond;
            int nextFrame = Mathf.Min(Mathf.FloorToInt(_elapsed * _framesPerSecond), _frames.Length - 1);
            ShowFrame(nextFrame);

            if (_fadeOutSeconds > 0f && _elapsed > duration - _fadeOutSeconds)
            {
                var color = _renderer.color;
                color.a = Mathf.Clamp01((duration - _elapsed) / _fadeOutSeconds);
                _renderer.color = color;
            }

            if (_elapsed >= duration) Destroy(gameObject);
        }

        private void ShowFrame(int index)
        {
            if (index == _frameIndex) return;
            _frameIndex = index;
            _renderer.sprite = _frames[index];
        }
    }
}
