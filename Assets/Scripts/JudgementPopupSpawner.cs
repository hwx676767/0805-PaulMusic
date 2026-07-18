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

        private Judgment _lastJudgment;
        private GameObject _current;
        private JudgementPopup _currentPopup;

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
            if (prefab == null) return;

            // 判定线世界坐标（已包含 Playfield 旋转/位移）；不贴轨道则用本组件位置
            Vector3 worldPos = spawnAtHitLane ? info.worldPosition : transform.position;

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
    }
}
