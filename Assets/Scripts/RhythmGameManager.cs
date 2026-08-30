using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DeusEx.Rhythm
{
    public enum Judgment { Perfect, Great, Good, Miss }

    /// <summary>一次判定的上下文：档位、所在轨道、以及该轨道判定线处的世界坐标（供在击中位置弹出提示）。</summary>
    public readonly struct JudgementInfo
    {
        public readonly Judgment judgment;
        public readonly int lane;
        public readonly Vector3 worldPosition;

        public JudgementInfo(Judgment judgment, int lane, Vector3 worldPosition)
        {
            this.judgment = judgment;
            this.lane = lane;
            this.worldPosition = worldPosition;
        }
    }

    /// <summary>
    /// 下落式音游主控：从场景里的 Playfield（轨道+判定线，自己拖进场景的预制体）读取布局，
    /// 在其上落下音符并处理输入判定、计分连击、HUD 与实时偏移校准。
    /// 场景中没有 Playfield 时会用程序化精灵搭一个兜底，保证零资源也能直接运行。
    /// 未指定歌曲时会生成一段节拍器 Demo，直接按 Play 即可体验同步与判定。
    /// </summary>
    public class RhythmGameManager : MonoBehaviour
    {
        [Header("判定区")]
        [Tooltip("场景里的 Playfield（含轨道与判定线）。留空则自动在场景中查找；都没有则程序化兜底。")]
        public Playfield playfield;
        [Tooltip("可选：UI 轨道布局（对齐 tracks.png）。指定后音符沿 HUD 上的 spawn→hit 路径下落，不再跟世界 Playfield 平面。")]
        public UiNoteTrackLayout uiTrack;

        [Header("资源（可留空：留空则生成节拍器 Demo 谱面）")]
        [Tooltip("歌曲音频。留空将自动生成一段节拍器点击音轨用于测试。")]
        public AudioClip song;
        [Tooltip("谱面 JSON（TextAsset）。留空且无歌曲时自动生成 Demo 谱面。")]
        public TextAsset chartJson;

        [Header("音符预制体（可留空，留空则用程序化精灵）")]
        [Tooltip("单点音符预制体，根节点需有 SpriteRenderer。UI 轨道模式下忽略，改用圆形 Image。")]
        public SpriteRenderer notePrefab;
        [Tooltip("长按条预制体，根节点需有 SpriteRenderer。")]
        public SpriteRenderer holdBodyPrefab;

        [Header("下落")]
        [Tooltip("下落速度（世界单位/秒），越大音符越快。仅世界 Playfield 模式使用。")]
        public float scrollSpeed = 9f;

        [Header("判定窗口（毫秒，单侧）")]
        public float perfectMs = 35f;
        public float greatMs = 70f;
        public float goodMs = 120f;

        [Header("长按")]
        [Tooltip("勾选后，长按头部命中后还需要持键至尾部：提前松开算 Miss，尾部会按时机额外结算一次判定。关闭时只判定头部，长条在尾端自动消失。")]
        public bool judgeHoldTail = false;

        [Header("音符外观（未指定 prefab 时）")]
        [Tooltip("程序化音符颜色。默认黑色圆形以贴近当前美术。有正式 note 图后拖到 notePrefab 即可替换。")]
        public Color noteColor = Color.black;
        public Color holdColor = new Color(0.15f, 0.15f, 0.15f, 0.7f);
        [Tooltip("未指定 prefab 时生成黑色圆形，而不是菱形色块。")]
        public bool proceduralCircleNote = true;

        [Header("可选（默认不改动相机、不画任何 UI）")]
        [Tooltip("勾选后开始时把现有正交相机对准判定区（不改背景色、不新建相机）。默认关闭，相机由你自己摆。")]
        public bool controlCamera = false;
        [Tooltip("勾选后用 IMGUI 画一个调试 HUD（分数/连击/校准等）。默认关闭，正式界面建议自己用 UGUI/UI Toolkit 做。")]
        public bool showDebugOverlay = false;

        // ---- 运行时状态 ----
        private Conductor _conductor;
        private ChartData _chart;
        private int _laneCount;
        private bool _ready;
        private Sprite _whiteSprite;
        private Sprite _circleSprite;
        private readonly List<ActiveNote> _notes = new List<ActiveNote>();
        private bool _useUiTrack;

        // 在 Playfield 局部坐标系里缓存的几何（让音符贴合带旋转/位移的 Playfield）
        private Transform _pfTransform;
        private Quaternion _pfRotation;
        private float[] _laneLocalX;     // 各轨道中心在 Playfield 局部空间的 X
        private float _judgeLocalY;      // 判定线在 Playfield 局部空间的 Y
        private float _spawnLocalY;      // 可见上界在 Playfield 局部空间的 Y

        private double _audioOffset;   // 秒
        private double _inputOffset;   // 秒

        // 统计
        private int _score, _combo, _maxCombo;
        private int _cPerfect, _cGreat, _cGood, _cMiss;
        private double _accSum; private int _accCount;
        private Judgment _lastJudgment = Judgment.Perfect;
        private float _lastJudgeTime = -10f;
        private GUIStyle _hud, _hudBig;

        // 对外只读状态与事件，供 UI（如 TMP 显示 combo）订阅，避免 UI 直接耦合内部逻辑
        public int Combo => _combo;
        public int Score => _score;
        public int MaxCombo => _maxCombo;
        public int PerfectCount => _cPerfect;
        public int GreatCount => _cGreat;
        public int GoodCount => _cGood;
        public int MissCount => _cMiss;
        /// <summary>按判定档位取累计总数，便于 UI 统一处理。</summary>
        public int GetJudgementCount(Judgment j) => j switch
        {
            Judgment.Perfect => _cPerfect,
            Judgment.Great => _cGreat,
            Judgment.Good => _cGood,
            _ => _cMiss,
        };
        /// <summary>歌曲是否正在播放。供战斗系统等外部逻辑判断游戏是否进行中。</summary>
        public bool IsPlaying => _conductor != null && _conductor.IsPlaying;
        /// <summary>停止歌曲（例如战斗中任一方血量归零、游戏结束时调用）。停止后音符不再下落与判定。</summary>
        public void StopSong() { if (_conductor != null) _conductor.Stop(); }

        /// <summary>当前歌曲/节奏播放速度倍率（1=原速）。歌曲未开始时回退为 1。</summary>
        public double PlaybackSpeed => _conductor != null ? _conductor.PlaybackSpeed : 1.0;

        /// <summary>设置歌曲/节奏播放速度倍率（会同时改变音高与音符下落节奏）。供卡牌"变慢/变快"效果调用。</summary>
        public void SetPlaybackSpeed(double speed) { if (_conductor != null) _conductor.PlaybackSpeed = speed; }

        /// <summary>
        /// 尝试把当前 combo 作为费用消耗掉 <paramref name="amount"/> 点（供卡牌系统出牌扣费）。
        /// combo 不足时返回 false 且不扣除；成功时扣除并触发 <see cref="ComboChanged"/> 通知 UI。
        /// 只影响"当前 combo"，历史最大连击（MaxCombo）不受影响。
        /// </summary>
        public bool TrySpendCombo(int amount)
        {
            if (amount <= 0) return true;   // 0 费或负数视为无需扣费，直接允许
            if (_combo < amount) return false;
            _combo -= amount;
            ComboChanged?.Invoke(_combo);
            return true;
        }
        /// <summary>combo 变化时触发，参数为最新 combo（Miss 归零也会触发）。</summary>
        public event System.Action<int> ComboChanged;
        /// <summary>每次判定触发，参数为判定档位。</summary>
        public event System.Action<Judgment> Judged;
        /// <summary>每次判定触发，带轨道编号与该轨道判定线处的世界坐标，供在击中位置弹出 Perfect/Great/Good/Miss 提示。</summary>
        public event System.Action<JudgementInfo> JudgedAt;

        /// <summary>
        /// 判定窗口倍率（1=原始，1.5=放宽 50%）。供卡牌效果临时放宽/收紧判定，会同时影响 Perfect/Great/Good
        /// 与自动 Miss 的时机。由 <see cref="CardEffectRunner"/> 设置。
        /// </summary>
        public float JudgementWindowScale { get; set; } = 1f;

        private double PerfectW => perfectMs / 1000.0 * JudgementWindowScale;
        private double GreatW => greatMs / 1000.0 * JudgementWindowScale;
        private double GoodW => goodMs / 1000.0 * JudgementWindowScale;

        private class ActiveNote
        {
            public NoteData data;
            public SpriteRenderer head;
            public SpriteRenderer body; // 仅长按有（世界模式）
            public RectTransform headUi; // UI 轨道模式
            public Image headImage;
            public Vector3 bodyBaseScale;
            public bool headJudged;
            public bool holding;
            public bool finished;
        }

        private void Start()
        {
            _audioOffset = PlayerPrefs.GetFloat("rg_audioOffset", 0f);
            _inputOffset = PlayerPrefs.GetFloat("rg_inputOffset", 0f);

            AcquirePlayfield();
            if (!_ready) return; // 没有可用的 Playfield 就直接停下，不生成任何东西

            _whiteSprite = BuildWhiteSprite();
            if (proceduralCircleNote) _circleSprite = BuildCircleSprite(64);
            SetupCamera();
            LoadChart();
            BuildNotes();
            StartConductor();
        }

        // ---------------- Playfield 获取 ----------------

        private void AcquirePlayfield()
        {
            // 若 Inspector 里拖的是"预制体资源"而不是场景实例（gameObject 不属于任何已加载场景），先实例化进场景
            if (playfield != null && !playfield.gameObject.scene.IsValid())
            {
                Debug.Log($"[Rhythm] Playfield 字段指向的是预制体资源，已实例化到场景：{playfield.name}");
                playfield = Instantiate(playfield);
            }

            // 未手动指定时，自动在场景中查找
            if (playfield == null)
            {
                playfield = FindObjectOfType<Playfield>();
                if (playfield != null)
                    Debug.Log($"[Rhythm] 自动找到场景中的 Playfield：{playfield.name}");
            }

            // 没有可用的 Playfield：直接停下，绝不生成任何兜底物体
            if (playfield == null || playfield.LaneCount == 0)
            {
                Debug.LogError("[Rhythm] 场景中没有可用的 Playfield（未找到，或其 lanes 列表为空），已停止运行。" +
                               "请把 Generated/Playfield.prefab 拖进场景；若 lanes 为空，可在 Playfield 组件右键选 " +
                               "\"Collect Lanes From Children\"。");
                _ready = false;
                enabled = false;
                return;
            }

            Debug.Log($"[Rhythm] 使用场景中的 Playfield「{playfield.name}」，共 {playfield.LaneCount} 轨。");
            _laneCount = playfield.LaneCount;
            CachePlayfieldGeometry();

            if (uiTrack == null) uiTrack = FindObjectOfType<UiNoteTrackLayout>();
            _useUiTrack = uiTrack != null && uiTrack.LaneCount > 0;
            if (_useUiTrack)
            {
                _laneCount = Mathf.Min(_laneCount, uiTrack.LaneCount);
                Debug.Log($"[Rhythm] 启用 UI 轨道布局「{uiTrack.name}」，音符将沿 HUD 路径下落（{_laneCount} 轨）。");
            }

            _ready = true;
        }

        /// <summary>
        /// 把轨道/判定线/可见上界换算到 Playfield 的局部坐标系并缓存。
        /// 之后音符在局部空间里下落，再用 Playfield 的 Transform 变换回世界，
        /// 这样 Playfield 的旋转与位移都会被自动跟随（音符贴合轨道）。
        /// </summary>
        private void CachePlayfieldGeometry()
        {
            _pfTransform = playfield.transform;
            _pfRotation = _pfTransform.rotation;

            _laneLocalX = new float[_laneCount];
            for (int i = 0; i < _laneCount; i++)
            {
                Lane lane = playfield.GetLane(i);
                _laneLocalX[i] = lane != null ? _pfTransform.InverseTransformPoint(lane.transform.position).x : 0f;
            }

            _judgeLocalY = playfield.judgeLine != null
                ? _pfTransform.InverseTransformPoint(playfield.judgeLine.position).y
                : 0f;
            _spawnLocalY = playfield.spawnTop != null
                ? _pfTransform.InverseTransformPoint(playfield.spawnTop.position).y
                : playfield.spawnTopY;
        }

        // ---------------- 资源/相机 ----------------

        private static Sprite BuildWhiteSprite()
        {
            var tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            tex.SetPixel(0, 0, Color.white);
            tex.filterMode = FilterMode.Point;
            tex.Apply();
            // pixelsPerUnit = 1 → transform.localScale 即等于世界单位尺寸
            return Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
        }

        private static Sprite BuildCircleSprite(int size)
        {
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float cx = (size - 1) * 0.5f;
            float r = size * 0.5f - 1.5f;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float d = Vector2.Distance(new Vector2(x, y), new Vector2(cx, cx));
                    float a = Mathf.Clamp01(r - d + 1f);
                    tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
                }
            }
            tex.Apply(false, true);
            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        }

        private void SetupCamera()
        {
            if (!controlCamera) return;
            var cam = Camera.main;
            if (cam == null) return; // 不新建相机，沿用场景里的

            cam.orthographic = true;
            float top = playfield.SpawnTopY + 1f;
            float bottom = playfield.JudgeLineY - 1.5f;
            cam.orthographicSize = Mathf.Max(0.5f, (top - bottom) * 0.5f);
            var p = cam.transform.position;
            cam.transform.position = new Vector3(p.x, (top + bottom) * 0.5f, p.z);
        }

        private SpriteRenderer Spawn(string name, Color color, int order, Transform parent = null)
        {
            var go = new GameObject(name);
            if (parent != null) go.transform.SetParent(parent);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = _whiteSprite;
            sr.color = color;
            sr.sortingOrder = order;
            return sr;
        }

        /// <summary>有预制体则实例化，否则用程序化白色精灵生成一个 SpriteRenderer。</summary>
        private SpriteRenderer CreateSprite(SpriteRenderer prefab, string name, Color fallbackColor, int fallbackOrder)
        {
            if (prefab != null) return Instantiate(prefab);
            return Spawn(name, fallbackColor, fallbackOrder);
        }

        // ---------------- 谱面 ----------------

        private void LoadChart()
        {
            if (chartJson != null)
            {
                _chart = ChartData.FromJson(chartJson.text);
            }
            else if (song == null)
            {
                GenerateDemo(); // 同时生成点击音轨与匹配谱面
            }

            if (_chart == null)
            {
                Debug.LogWarning("[Rhythm] 未提供谱面，生成空谱面。请在 Inspector 指定 chartJson。");
                _chart = new ChartData();
            }

            if (_chart.laneCount > _laneCount)
                Debug.LogWarning($"[Rhythm] 谱面需要 {_chart.laneCount} 轨，但 Playfield 只有 {_laneCount} 轨，超出的音符会被忽略。");
        }

        private void BuildNotes()
        {
            if (_chart.notes.Count > 4000)
                Debug.LogWarning("[Rhythm] 音符数较多，原型未做对象池，超大谱面可能影响内存。");

            foreach (var n in _chart.notes)
            {
                if (n.lane < 0 || n.lane >= _laneCount) continue;
                if (playfield.GetLane(n.lane) == null) continue;

                var an = new ActiveNote { data = n };

                if (_useUiTrack)
                {
                    CreateUiNote(an, n);
                }
                else
                {
                    an.head = CreateSprite(notePrefab, "Note", n.IsHold ? holdColor : noteColor, 20);
                    if (proceduralCircleNote && notePrefab == null && _circleSprite != null)
                    {
                        an.head.sprite = _circleSprite;
                        an.head.transform.localScale = Vector3.one * 0.55f;
                    }
                    an.head.transform.rotation = _pfRotation;

                    if (n.IsHold)
                    {
                        an.body = CreateSprite(holdBodyPrefab, "HoldBody", new Color(holdColor.r, holdColor.g, holdColor.b, 0.55f), 18);
                        an.body.transform.rotation = _pfRotation;
                        an.bodyBaseScale = an.body.transform.localScale;
                    }

                    an.head.enabled = false;
                    if (an.body != null) an.body.enabled = false;
                }

                _notes.Add(an);
            }
        }

        private void CreateUiNote(ActiveNote an, NoteData n)
        {
            RectTransform root = uiTrack.NoteRoot;
            var go = new GameObject($"NoteUI_{n.lane}_{n.time:0.00}", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            var rect = go.GetComponent<RectTransform>();
            rect.SetParent(root, false);
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            float size = uiTrack.noteSize;
            rect.sizeDelta = new Vector2(size, size);

            var image = go.GetComponent<Image>();
            image.sprite = _circleSprite != null ? _circleSprite : _whiteSprite;
            image.color = n.IsHold ? holdColor : (uiTrack != null ? uiTrack.noteColor : noteColor);
            image.raycastTarget = false;
            image.enabled = false;
            image.preserveAspect = true;

            an.headUi = rect;
            an.headImage = image;
        }

        private void StartConductor()
        {
            _conductor = gameObject.AddComponent<Conductor>();
            _conductor.AudioOffset = _audioOffset;
            var clip = song != null ? song : _demoClip;
            if (clip == null)
            {
                Debug.LogError("[Rhythm] 没有可播放的音频。");
                return;
            }
            _conductor.Play(clip, _chart.offset);
        }

        // ---------------- 主循环 ----------------

        private void Update()
        {
            HandleCalibration();
            if (_conductor == null || !_conductor.IsPlaying) return;

            double songPos = _conductor.SongPosition;
            UpdateNotePositions(songPos);
            HandleInput(songPos);
            UpdateLanePressState();
        }

        private void UpdateNotePositions(double songPos)
        {
            double leadTime = _useUiTrack
                ? uiTrack.approachSeconds
                : ((_spawnLocalY - _judgeLocalY) + 2f) / scrollSpeed;

            foreach (var an in _notes)
            {
                if (an.finished) continue;

                double dt = an.data.time - songPos;

                // 自动 Miss：单点/长按头超过 Good 窗口仍未击中
                if (!an.headJudged && an.data.time < songPos - GoodW)
                {
                    RegisterMiss(an.data.lane);
                    an.headJudged = true;
                    an.finished = true;
                    HideNote(an);
                    continue;
                }

                // 长按到尾部后均结束并隐藏。仅开启尾判时才在此额外计分/显示判定。
                if (an.data.IsHold && an.headJudged && songPos >= an.data.EndTime)
                {
                    if (judgeHoldTail && an.holding)
                        RegisterHit(Judgment.Perfect, an.data.lane);
                    an.holding = false;
                    an.finished = true;
                    HideNote(an);
                    continue;
                }

                // 可见性：在窗口内才显示
                bool visible = dt < leadTime && an.data.EndTime - songPos > -GoodW * 2;
                SetNoteVisible(an, visible && !an.headJudged, visible);
                if (!visible) continue;

                if (_useUiTrack)
                {
                    float progress = leadTime > 1e-5 ? Mathf.Clamp01((float)(dt / leadTime)) : 0f;
                    if (uiTrack.TrySample(an.data.lane, progress, out Vector2 anchored) && an.headUi != null)
                        an.headUi.anchoredPosition = anchored;
                }
                else
                {
                    float laneX = _laneLocalX[an.data.lane];
                    float headLocalY = _judgeLocalY + (float)dt * scrollSpeed;
                    an.head.transform.position = _pfTransform.TransformPoint(new Vector3(laneX, headLocalY, 0f));

                    if (an.body != null)
                    {
                        float tailLocalY = _judgeLocalY + (float)(an.data.EndTime - songPos) * scrollSpeed;
                        float bottomLocalY = an.headJudged ? _judgeLocalY : headLocalY;
                        float h = Mathf.Max(0.01f, tailLocalY - bottomLocalY);
                        float centerLocalY = (bottomLocalY + tailLocalY) * 0.5f;
                        an.body.transform.position = _pfTransform.TransformPoint(new Vector3(laneX, centerLocalY, 0f));
                        an.body.transform.localScale = new Vector3(an.bodyBaseScale.x, h, an.bodyBaseScale.z);
                    }
                }
            }
        }

        private static void SetNoteVisible(ActiveNote an, bool headVisible, bool bodyVisible)
        {
            if (an.headImage != null) an.headImage.enabled = headVisible;
            if (an.head != null) an.head.enabled = headVisible;
            if (an.body != null) an.body.enabled = bodyVisible && an.body != null;
        }

        private void HandleInput(double songPos)
        {
            double inputTime = songPos + _inputOffset;

            for (int lane = 0; lane < _laneCount; lane++)
            {
                Lane laneObj = playfield.GetLane(lane);
                if (laneObj == null) continue;
                KeyCode key = laneObj.key;

                if (Input.GetKeyDown(key))
                {
                    ActiveNote best = null;
                    double bestAbs = double.MaxValue;
                    foreach (var an in _notes)
                    {
                        if (an.finished || an.headJudged || an.data.lane != lane) continue;
                        double abs = System.Math.Abs(an.data.time - inputTime);
                        if (abs <= GoodW && abs < bestAbs) { bestAbs = abs; best = an; }
                    }

                    if (best != null)
                    {
                        Judgment j = Classify(bestAbs);
                        RegisterHit(j, lane);
                        best.headJudged = true;
                        if (best.data.IsHold)
                        {
                            best.holding = true;
                            SetNoteVisible(best, false, true);
                        }
                        else
                        {
                            best.finished = true;
                            HideNote(best);
                        }
                    }
                }

                // 关闭尾判时，提前松开不影响已经命中的长条；它会在尾端自然消失。
                if (judgeHoldTail && Input.GetKeyUp(key))
                {
                    foreach (var an in _notes)
                    {
                        if (an.finished || !an.holding || an.data.lane != lane) continue;
                        // 在尾部 Good 窗口之前松手算断（Miss）
                        if (songPos < an.data.EndTime - GoodW)
                        {
                            RegisterMiss(an.data.lane);
                        }
                        else
                        {
                            double abs = System.Math.Abs(inputTime - an.data.EndTime);
                            RegisterHit(Classify(abs), an.data.lane);
                        }
                        an.holding = false;
                        an.finished = true;
                        HideNote(an);
                        break;
                    }
                }
            }
        }

        private void UpdateLanePressState()
        {
            for (int lane = 0; lane < _laneCount; lane++)
            {
                Lane laneObj = playfield.GetLane(lane);
                if (laneObj != null) laneObj.SetPressed(Input.GetKey(laneObj.key));
            }
        }

        private Judgment Classify(double absDelta)
        {
            if (absDelta <= PerfectW) return Judgment.Perfect;
            if (absDelta <= GreatW) return Judgment.Great;
            return Judgment.Good;
        }

        private void HideNote(ActiveNote an)
        {
            SetNoteVisible(an, false, false);
        }

        // ---------------- 计分 ----------------

        private void RegisterHit(Judgment j, int lane)
        {
            switch (j)
            {
                case Judgment.Perfect: _score += 300; _combo++; _cPerfect++; _accSum += 100; break;
                case Judgment.Great: _score += 200; _combo++; _cGreat++; _accSum += 70; break;
                case Judgment.Good: _score += 100; _combo++; _cGood++; _accSum += 40; break;
            }
            _accCount++;
            if (_combo > _maxCombo) _maxCombo = _combo;
            FlashJudgment(j);
            ComboChanged?.Invoke(_combo);
            Judged?.Invoke(j);
            JudgedAt?.Invoke(new JudgementInfo(j, lane, JudgeWorldPosition(lane)));
        }

        private void RegisterMiss(int lane)
        {
            _combo = 0;
            _cMiss++;
            _accCount++;
            FlashJudgment(Judgment.Miss);
            ComboChanged?.Invoke(_combo);
            Judged?.Invoke(Judgment.Miss);
            JudgedAt?.Invoke(new JudgementInfo(Judgment.Miss, lane, JudgeWorldPosition(lane)));
        }

        /// <summary>取某条轨道判定线处的世界坐标（已跟随 Playfield 的旋转/位移）。lane 越界时回退到 Playfield 原点。</summary>
        public Vector3 JudgeWorldPosition(int lane)
        {
            if (_useUiTrack && uiTrack != null)
            {
                Camera cam = Camera.main;
                if (cam != null)
                {
                    float depth = _pfTransform != null
                        ? cam.WorldToScreenPoint(_pfTransform.position).z
                        : Mathf.Max(1f, Mathf.Abs(cam.transform.position.z));
                    return uiTrack.HitScreenToWorld(lane, cam, depth);
                }
                return uiTrack.HitWorldPosition(lane);
            }

            if (_pfTransform == null || _laneLocalX == null || lane < 0 || lane >= _laneLocalX.Length)
                return _pfTransform != null ? _pfTransform.position : Vector3.zero;
            return _pfTransform.TransformPoint(new Vector3(_laneLocalX[lane], _judgeLocalY, 0f));
        }

        /// <summary>是否正在用 HUD 上的 UI 轨道下落音符（判定特效也应挂到同一 Canvas）。</summary>
        public bool UsesUiTrack => _useUiTrack && uiTrack != null;

        private void FlashJudgment(Judgment j)
        {
            _lastJudgment = j;
            _lastJudgeTime = Time.time;
        }

        // ---------------- 偏移校准 ----------------

        private void HandleCalibration()
        {
            bool changed = false;
            if (Input.GetKeyDown(KeyCode.F1)) { _audioOffset -= 0.005; changed = true; }
            if (Input.GetKeyDown(KeyCode.F2)) { _audioOffset += 0.005; changed = true; }
            if (Input.GetKeyDown(KeyCode.F3)) { _inputOffset -= 0.005; changed = true; }
            if (Input.GetKeyDown(KeyCode.F4)) { _inputOffset += 0.005; changed = true; }
            if (Input.GetKeyDown(KeyCode.F5)) { _audioOffset = 0; _inputOffset = 0; changed = true; }

            if (changed)
            {
                if (_conductor != null) _conductor.AudioOffset = _audioOffset;
                PlayerPrefs.SetFloat("rg_audioOffset", (float)_audioOffset);
                PlayerPrefs.SetFloat("rg_inputOffset", (float)_inputOffset);
            }

            if (Input.GetKeyDown(KeyCode.R)) RestartScene();
        }

        private void RestartScene()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }

        // ---------------- HUD ----------------

        private void OnGUI()
        {
            if (!showDebugOverlay) return;

            if (_hud == null)
            {
                _hud = new GUIStyle(GUI.skin.label) { fontSize = 18, richText = true };
                _hudBig = new GUIStyle(GUI.skin.label) { fontSize = 48, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
            }

            float acc = _accCount > 0 ? (float)(_accSum / _accCount) : 100f;
            string title = _chart != null ? $"{_chart.title}" : "";

            GUI.Label(new Rect(20, 16, 600, 30), $"<b>{title}</b>", _hud);
            GUI.Label(new Rect(20, 46, 600, 30), $"分数 {_score}", _hud);
            GUI.Label(new Rect(20, 72, 600, 30), $"连击 {_combo}  (最大 {_maxCombo})", _hud);
            GUI.Label(new Rect(20, 98, 600, 30), $"准度 {acc:0.00}%", _hud);
            GUI.Label(new Rect(20, 124, 600, 30),
                $"P {_cPerfect}  Great {_cGreat}  Good {_cGood}  <color=#ff6464>Miss {_cMiss}</color>", _hud);

            GUI.Label(new Rect(20, Screen.height - 78, 900, 30),
                $"音频偏移 {_audioOffset * 1000:0} ms (F1/F2)   输入偏移 {_inputOffset * 1000:0} ms (F3/F4)   F5 归零   R 重开", _hud);
            string keys = "";
            if (playfield != null)
                for (int i = 0; i < _laneCount; i++)
                {
                    Lane ln = playfield.GetLane(i);
                    if (ln != null) keys += ln.key + " ";
                }
            GUI.Label(new Rect(20, Screen.height - 50, 900, 30), $"轨道按键: {keys.Trim()}", _hud);

            // 最近一次判定大字提示
            if (Time.time - _lastJudgeTime < 0.5f)
            {
                Color col = _lastJudgment switch
                {
                    Judgment.Perfect => new Color(1f, 0.95f, 0.4f),
                    Judgment.Great => new Color(0.4f, 1f, 0.6f),
                    Judgment.Good => new Color(0.5f, 0.8f, 1f),
                    _ => new Color(1f, 0.4f, 0.4f),
                };
                string txt = _lastJudgment switch
                {
                    Judgment.Perfect => "PERFECT",
                    Judgment.Great => "GREAT",
                    Judgment.Good => "GOOD",
                    _ => "MISS",
                };
                var prev = GUI.color;
                GUI.color = col;
                GUI.Label(new Rect(0, Screen.height * 0.35f, Screen.width, 80), txt, _hudBig);
                GUI.color = prev;
            }
        }

        // ---------------- Demo 生成（节拍器音轨 + 匹配谱面） ----------------

        private AudioClip _demoClip;

        private void GenerateDemo()
        {
            const double bpm = 150.0;
            const double durationSec = 40.0;
            double secPerBeat = 60.0 / bpm;

            _demoClip = GenerateClickTrack(bpm, durationSec);

            int lanes = Mathf.Max(1, _laneCount);
            var chart = new ChartData { title = "Demo (节拍器)", artist = "DeusExMachina", laneCount = lanes, bpm = bpm, offset = 0 };
            int leadInBeats = 8;
            int totalBeats = (int)(durationSec / secPerBeat) - 2;

            for (int b = leadInBeats; b < totalBeats; b++)
            {
                int lane = b % lanes;
                double t = b * secPerBeat;

                if (b % 8 == 0)
                {
                    // 每 8 拍来一个 1 拍长按，测试 Hold
                    chart.notes.Add(new NoteData { lane = lane, time = t, duration = secPerBeat });
                }
                else
                {
                    chart.notes.Add(new NoteData { lane = lane, time = t, duration = 0 });
                    if (b % 2 == 1)
                    {
                        // 插入一个反向轨的 8 分音符增加密度
                        chart.notes.Add(new NoteData { lane = lanes - 1 - lane, time = t + secPerBeat * 0.5, duration = 0 });
                    }
                }
            }

            _chart = ChartData.FromJson(chart.ToJson());
        }

        private static AudioClip GenerateClickTrack(double bpm, double durationSec)
        {
            int sr = AudioSettings.outputSampleRate;
            if (sr <= 0) sr = 48000;
            int totalSamples = (int)(sr * durationSec);
            var data = new float[totalSamples];

            double secPerBeat = 60.0 / bpm;
            int beats = (int)(durationSec / secPerBeat);
            int beepSamples = (int)(sr * 0.05); // 50ms 短音

            for (int b = 0; b < beats; b++)
            {
                int start = (int)(b * secPerBeat * sr);
                double freq = (b % 4 == 0) ? 1500.0 : 1000.0; // 强拍更高
                for (int i = 0; i < beepSamples && start + i < totalSamples; i++)
                {
                    double env = 1.0 - (double)i / beepSamples; // 线性衰减
                    data[start + i] += (float)(Mathf.Sin((float)(2.0 * Mathf.PI * freq * i / sr)) * env * 0.5);
                }
            }

            var clip = AudioClip.Create("DemoClick", totalSamples, 1, sr, false);
            clip.SetData(data, 0);
            return clip;
        }
    }
}
