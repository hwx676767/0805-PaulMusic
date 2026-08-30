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

        [Header("Gizmos（Scene 对齐用）")]
        [Tooltip("画出每条轨从 SpawnTop 到 JudgeLine 的射线。")]
        public bool drawLaneRays = true;
        [Tooltip("画出判定线与出现线横条。")]
        public bool drawJudgeAndSpawnLines = true;
        [Tooltip("在端点画小球与轨号。")]
        public bool drawLaneLabels = true;
        [Tooltip("射线端点球体半径（Playfield 局部单位）。")]
        [Min(0.01f)] public float gizmoMarkerRadius = 0.12f;
        [Tooltip("判定线横条半宽（局部 X）。")]
        [Min(0.5f)] public float gizmoLineHalfWidth = 4f;

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
            DrawPlayfieldGizmos(selected: false);
        }

        private void OnDrawGizmosSelected()
        {
            DrawPlayfieldGizmos(selected: true);
        }

        private void DrawPlayfieldGizmos(bool selected)
        {
            // 在 Playfield 局部空间画，这样倾斜后射线仍贴合真实音符路径
            Matrix4x4 prev = Gizmos.matrix;
            Gizmos.matrix = transform.localToWorldMatrix;

            float judgeY = ResolveJudgeLocalY();
            float spawnY = ResolveSpawnLocalY();
            float half = gizmoLineHalfWidth;
            float boost = selected ? 1f : 0.75f;

            if (drawJudgeAndSpawnLines)
            {
                Gizmos.color = new Color(0.2f, 1f, 1f, 0.95f * boost);
                Gizmos.DrawLine(new Vector3(-half, judgeY, 0f), new Vector3(half, judgeY, 0f));

                Gizmos.color = new Color(0.35f, 0.65f, 1f, 0.7f * boost);
                Gizmos.DrawLine(new Vector3(-half, spawnY, 0f), new Vector3(half, spawnY, 0f));
            }

            if (drawLaneRays && lanes != null)
            {
                for (int i = 0; i < lanes.Count; i++)
                {
                    Lane lane = lanes[i];
                    if (lane == null) continue;

                    float x = transform.InverseTransformPoint(lane.transform.position).x;
                    Vector3 spawn = new Vector3(x, spawnY, 0f);
                    Vector3 hit = new Vector3(x, judgeY, 0f);

                    // 轨射线：出现点 → 判定点（与音符下落路径一致）
                    Gizmos.color = LaneRayColor(i, boost);
                    Gizmos.DrawLine(spawn, hit);

                    // 方向小箭头（靠近判定线）
                    Vector3 dir = (hit - spawn).normalized;
                    if (dir.sqrMagnitude > 1e-6f)
                    {
                        Vector3 tip = hit;
                        Vector3 side = Vector3.Cross(dir, Vector3.forward).normalized * (gizmoMarkerRadius * 1.2f);
                        Gizmos.DrawLine(tip, tip - dir * (gizmoMarkerRadius * 2.2f) + side);
                        Gizmos.DrawLine(tip, tip - dir * (gizmoMarkerRadius * 2.2f) - side);
                    }

                    if (drawLaneLabels)
                    {
                        Gizmos.color = new Color(0.3f, 0.85f, 1f, 0.9f * boost);
                        Gizmos.DrawWireSphere(spawn, gizmoMarkerRadius);
                        Gizmos.color = new Color(1f, 0.9f, 0.25f, 0.95f * boost);
                        Gizmos.DrawWireSphere(hit, gizmoMarkerRadius);
                    }
                }
            }

            Gizmos.matrix = prev;

#if UNITY_EDITOR
            if (drawLaneLabels && lanes != null)
            {
                for (int i = 0; i < lanes.Count; i++)
                {
                    Lane lane = lanes[i];
                    if (lane == null) continue;
                    float x = transform.InverseTransformPoint(lane.transform.position).x;
                    Vector3 hitWorld = transform.TransformPoint(new Vector3(x, judgeY, 0f));
                    Vector3 spawnWorld = transform.TransformPoint(new Vector3(x, spawnY, 0f));
                    UnityEditor.Handles.Label(spawnWorld + Vector3.up * 0.05f, $"Spawn {i}");
                    UnityEditor.Handles.Label(hitWorld + Vector3.up * 0.05f, $"Hit {i} [{lane.key}]");
                }
            }
#endif
        }

        private float ResolveJudgeLocalY()
        {
            if (judgeLine != null) return transform.InverseTransformPoint(judgeLine.position).y;
            return 0f;
        }

        private float ResolveSpawnLocalY()
        {
            if (spawnTop != null) return transform.InverseTransformPoint(spawnTop.position).y;
            // 无 spawnTop 时：用局部默认高度（与 prefab 默认 5.5 一致）
            return 5.5f;
        }

        private static Color LaneRayColor(int index, float boost)
        {
            // 四轨易辨色
            Color c = (index % 4) switch
            {
                0 => new Color(1f, 0.35f, 0.35f),
                1 => new Color(1f, 0.75f, 0.25f),
                2 => new Color(0.35f, 0.95f, 0.45f),
                _ => new Color(0.4f, 0.7f, 1f),
            };
            c.a = 0.95f * boost;
            return c;
        }
    }
}
