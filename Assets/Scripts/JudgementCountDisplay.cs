using TMPro;
using UnityEngine;

namespace DeusEx.Rhythm
{
    /// <summary>
    /// 用 TextMeshPro（TMP_Text，UI 或世界空间均可）显示 Perfect/Great/Good/Miss 的累计总数。
    /// 订阅 RhythmGameManager.Judged 事件，每次判定后刷新对应数字（重开场景会随管理器自动归零）。
    /// 用法：在 Canvas 下放好 TMP 文本，把它们拖到对应字段；本脚本挂在任意物体上即可（label 为空的项会跳过）。
    /// </summary>
    public class JudgementCountDisplay : MonoBehaviour
    {
        [Tooltip("留空则自动在场景中查找 RhythmGameManager。")]
        public RhythmGameManager manager;

        [Header("分项文本（只填需要的；{0} 为该档位总数）")]
        public TMP_Text perfectLabel;
        public TMP_Text greatLabel;
        public TMP_Text goodLabel;
        public TMP_Text missLabel;

        [Tooltip("分项文本格式，{0} 为数字。例如 \"{0}\" 或 \"PERFECT {0}\"。")]
        public string perfectFormat = "{0}";
        public string greatFormat = "{0}";
        public string goodFormat = "{0}";
        public string missFormat = "{0}";

        [Header("合并文本（可选，{0}~{3} = Perfect/Great/Good/Miss）")]
        public TMP_Text combinedLabel;
        public string combinedFormat = "PERFECT {0}\nGREAT {1}\nGOOD {2}\nMISS {3}";

        [Header("文本")]
        [Tooltip("关闭自动换行：数字位数变多后不会折行向下溢出、压到下一行。建议保持开启。")]
        public bool disableWordWrap = true;

        [Header("变化回弹动画（作用于发生变化的分项文本）")]
        public bool punchOnChange = true;
        public float punchScale = 1.2f;
        public float recoverSpeed = 9f;

        private Vector3 _basePerfect = Vector3.one;
        private Vector3 _baseGreat = Vector3.one;
        private Vector3 _baseGood = Vector3.one;
        private Vector3 _baseMiss = Vector3.one;

        private void Awake()
        {
            if (perfectLabel != null) _basePerfect = perfectLabel.transform.localScale;
            if (greatLabel != null) _baseGreat = greatLabel.transform.localScale;
            if (goodLabel != null) _baseGood = goodLabel.transform.localScale;
            if (missLabel != null) _baseMiss = missLabel.transform.localScale;

            if (disableWordWrap)
            {
                ApplyNoWrap(perfectLabel);
                ApplyNoWrap(greatLabel);
                ApplyNoWrap(goodLabel);
                ApplyNoWrap(missLabel);
                // 合并文本本身用 \n 控制换行，关掉自动换行不影响显式换行
                ApplyNoWrap(combinedLabel);
            }
        }

        // 关掉自动换行，避免一行变长后折行、向下溢出压到相邻行。
        private static void ApplyNoWrap(TMP_Text label)
        {
            if (label == null) return;
#pragma warning disable CS0618 // enableWordWrapping 在新版 TMP 标记为过时，但仍可用，兼容旧版本
            label.enableWordWrapping = false;
#pragma warning restore CS0618
        }

        private void OnEnable()
        {
            if (manager == null) manager = FindObjectOfType<RhythmGameManager>();
            if (manager != null) manager.Judged += OnJudged;
            RefreshAll();
        }

        private void OnDisable()
        {
            if (manager != null) manager.Judged -= OnJudged;
        }

        private void OnJudged(Judgment j)
        {
            RefreshAll();
            if (!punchOnChange) return;

            switch (j)
            {
                case Judgment.Perfect: if (perfectLabel != null) perfectLabel.transform.localScale = _basePerfect * punchScale; break;
                case Judgment.Great: if (greatLabel != null) greatLabel.transform.localScale = _baseGreat * punchScale; break;
                case Judgment.Good: if (goodLabel != null) goodLabel.transform.localScale = _baseGood * punchScale; break;
                case Judgment.Miss: if (missLabel != null) missLabel.transform.localScale = _baseMiss * punchScale; break;
            }
        }

        private void Update()
        {
            if (!punchOnChange) return;
            LerpBack(perfectLabel, _basePerfect);
            LerpBack(greatLabel, _baseGreat);
            LerpBack(goodLabel, _baseGood);
            LerpBack(missLabel, _baseMiss);
        }

        private void LerpBack(TMP_Text label, Vector3 baseScale)
        {
            if (label == null) return;
            Transform t = label.transform;
            if (t.localScale != baseScale)
                t.localScale = Vector3.Lerp(t.localScale, baseScale, Time.deltaTime * recoverSpeed);
        }

        private void RefreshAll()
        {
            int p = manager != null ? manager.PerfectCount : 0;
            int gr = manager != null ? manager.GreatCount : 0;
            int gd = manager != null ? manager.GoodCount : 0;
            int m = manager != null ? manager.MissCount : 0;

            if (perfectLabel != null) perfectLabel.text = string.Format(perfectFormat, p);
            if (greatLabel != null) greatLabel.text = string.Format(greatFormat, gr);
            if (goodLabel != null) goodLabel.text = string.Format(goodFormat, gd);
            if (missLabel != null) missLabel.text = string.Format(missFormat, m);
            if (combinedLabel != null) combinedLabel.text = string.Format(combinedFormat, p, gr, gd, m);
        }
    }
}
