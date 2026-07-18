using System.Collections.Generic;
using UnityEngine;

namespace DeusEx.Rhythm
{
    /// <summary>
    /// 整块判定区：把若干条轨道(Lane)与一条判定线打包成一个预制体。把它拖进场景后随意编辑
    /// （轨道数量/位置/宽度、判定线高度、可见上界、按键），RhythmGameManager 运行时直接从这里读取布局。
    /// </summary>
    public class Playfield : MonoBehaviour
    {
        [Tooltip("从左到右的轨道，顺序即谱面里的 0..N 号轨道。")]
        public List<Lane> lanes = new List<Lane>();

        [Tooltip("判定线 Transform。音符到达它所在的 Y 即为最佳判定时刻。")]
        public Transform judgeLine;

        [Tooltip("音符可见上界标记（可留空，留空则用下面的 spawnTopY）。")]
        public Transform spawnTop;

        [Tooltip("当未指定 spawnTop 时使用：音符出现/可见的上边界（世界 Y）。")]
        public float spawnTopY = 5.5f;

        [Header("轨道布局")]
        [Tooltip("相邻轨道中心的间距（Playfield 局部单位）。轨道与音符各自的尺寸由各自 prefab 决定，这里只控制间距。")]
        public float laneSpacing = 1.2f;
        [Tooltip("勾选后按 laneSpacing 自动居中均匀排布轨道；想完全手动摆放轨道就取消勾选。")]
        public bool autoArrangeLanes = true;

        public int LaneCount => lanes.Count;
        public Lane GetLane(int index) => lanes[index];
        public float JudgeLineY => judgeLine != null ? judgeLine.position.y : 0f;
        public float SpawnTopY => spawnTop != null ? spawnTop.position.y : spawnTopY;

        private void Awake()
        {
            // 容错：忘了填 lanes 列表时，自动从子物体收集，避免被当成"空判定区"
            if (lanes == null || lanes.Count == 0) CollectLanesFromChildren();
        }

        /// <summary>把所有子物体上的 Lane（含未激活）按层级顺序收集进 lanes 列表。</summary>
        [ContextMenu("Collect Lanes From Children")]
        public void CollectLanesFromChildren()
        {
            lanes = new List<Lane>(GetComponentsInChildren<Lane>(true));
        }

        /// <summary>按 laneSpacing 把各轨道在 Playfield 局部 X 上居中均匀排布（不改各自尺寸）。</summary>
        [ContextMenu("Arrange Lanes Evenly")]
        public void ArrangeLanesEvenly()
        {
            if (lanes == null) return;
            int n = lanes.Count;
            float total = (n - 1) * laneSpacing;
            for (int i = 0; i < n; i++)
            {
                var lane = lanes[i];
                if (lane == null) continue;
                var lp = lane.transform.localPosition;
                lp.x = -total * 0.5f + i * laneSpacing;
                lane.transform.localPosition = lp;
            }
        }

        private void Reset()
        {
            CollectLanesFromChildren();
        }

        private void OnValidate()
        {
            // 编辑器里改 laneSpacing 时即时居中排布；想手动摆放轨道就取消 autoArrangeLanes
            if (!Application.isPlaying && autoArrangeLanes) ArrangeLanesEvenly();
        }

        private void OnDrawGizmos()
        {
            // 场景视图里标出判定线（青）与可见上界（蓝），方便摆放
            float jy = JudgeLineY;
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(new Vector3(-10f, jy, 0f), new Vector3(10f, jy, 0f));

            float ty = SpawnTopY;
            Gizmos.color = new Color(0.4f, 0.6f, 1f, 0.6f);
            Gizmos.DrawLine(new Vector3(-10f, ty, 0f), new Vector3(10f, ty, 0f));
        }
    }
}
