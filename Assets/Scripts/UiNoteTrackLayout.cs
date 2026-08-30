using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DeusEx.Rhythm
{
    /// <summary>
    /// 把音符路径对齐到 HUD 上的四条视觉轨道：每轨一个出现点(spawn)与判定点(hit)。
    /// 拖动 Scene 里的空 RectTransform 即可对上 tracks.png，无需改谱面逻辑。
    /// </summary>
    [DisallowMultipleComponent]
    public class UiNoteTrackLayout : MonoBehaviour
    {
        [Serializable]
        public class LanePath
        {
            [Tooltip("音符刚出现的位置（靠近轨道汇聚点）。")]
            public RectTransform spawn;
            [Tooltip("判定点（靠近轨道底端判定横线）。")]
            public RectTransform hit;
        }

        [Tooltip("从左到右 4 条轨。")]
        public List<LanePath> lanes = new List<LanePath>();

        [Tooltip("从出现点落到判定点所需秒数（替代世界距离 / scrollSpeed）。")]
        [Min(0.2f)] public float approachSeconds = 1.55f;

        [Tooltip("音符 UI 父节点；留空则用本物体。")]
        public RectTransform noteRoot;

        [Header("占位音符外观（无 prefab 时）")]
        public Color noteColor = Color.black;
        [Min(8f)] public float noteSize = 48f;

        public int LaneCount => lanes != null ? lanes.Count : 0;

        public RectTransform NoteRoot => noteRoot != null ? noteRoot : transform as RectTransform;

        public bool TrySample(int lane, float progressFromHitToSpawn, out Vector2 anchoredPos)
        {
            anchoredPos = Vector2.zero;
            if (lanes == null || lane < 0 || lane >= lanes.Count) return false;
            LanePath path = lanes[lane];
            if (path == null || path.spawn == null || path.hit == null) return false;

            RectTransform root = NoteRoot;
            if (root == null) return false;

            Vector2 spawnLocal = root.InverseTransformPoint(path.spawn.position);
            Vector2 hitLocal = root.InverseTransformPoint(path.hit.position);
            float t = Mathf.Clamp01(progressFromHitToSpawn);
            anchoredPos = Vector2.Lerp(hitLocal, spawnLocal, t);
            return true;
        }

        public Vector3 HitWorldPosition(int lane)
        {
            if (lanes == null || lane < 0 || lane >= lanes.Count || lanes[lane]?.hit == null)
                return transform.position;
            return lanes[lane].hit.position;
        }

        public RectTransform GetHit(int lane)
        {
            if (lanes == null || lane < 0 || lane >= lanes.Count) return null;
            return lanes[lane]?.hit;
        }

        /// <summary>
        /// Overlay 轨道路径的 hit.position 是屏幕像素坐标；把它投到主相机前的深度，
        /// 供世界空间 TMP / SpriteRenderer 判定特效使用。
        /// </summary>
        public Vector3 HitScreenToWorld(int lane, Camera cam, float depthWorldZ)
        {
            Vector3 screen = HitWorldPosition(lane);
            if (cam == null) return screen;
            return cam.ScreenToWorldPoint(new Vector3(screen.x, screen.y, depthWorldZ));
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (lanes == null) return;
            for (int i = 0; i < lanes.Count; i++)
            {
                LanePath p = lanes[i];
                if (p?.spawn == null || p.hit == null) continue;
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(p.spawn.position, p.hit.position);
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(p.hit.position, 8f);
                Gizmos.color = new Color(0.3f, 0.7f, 1f, 0.8f);
                Gizmos.DrawWireSphere(p.spawn.position, 8f);
                Handles.Label(p.hit.position, $"Hit {i}");
                Handles.Label(p.spawn.position, $"Spawn {i}");
            }
        }
#endif
    }
}
