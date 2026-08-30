using UnityEditor;
using UnityEngine;

namespace DeusEx.Rhythm.EditorTools
{
    /// <summary>
    /// 在 Scene 视图为每条轨的 Spawn/Hit 画可拖动手柄，方便做假透视对齐。
    /// </summary>
    [CustomEditor(typeof(UiNoteTrackLayout))]
    public class UiNoteTrackLayoutEditor : Editor
    {
        private void OnSceneGUI()
        {
            var layout = (UiNoteTrackLayout)target;
            if (layout.lanes == null) return;

            for (int i = 0; i < layout.lanes.Count; i++)
            {
                var path = layout.lanes[i];
                if (path == null) continue;

                Handles.color = new Color(0.25f, 0.85f, 1f, 1f);
                if (path.spawn != null)
                    DragMarker(path.spawn, $"Spawn {i}", new Color(0.25f, 0.85f, 1f));

                Handles.color = new Color(1f, 0.85f, 0.15f, 1f);
                if (path.hit != null)
                    DragMarker(path.hit, $"Hit {i}", new Color(1f, 0.85f, 0.15f));

                if (path.spawn != null && path.hit != null)
                {
                    Handles.color = Color.yellow;
                    Handles.DrawLine(path.spawn.position, path.hit.position);
                }
            }
        }

        private static void DragMarker(RectTransform marker, string label, Color color)
        {
            EditorGUI.BeginChangeCheck();
            Vector3 pos = marker.position;
            float size = HandleUtility.GetHandleSize(pos) * 0.15f;

            Handles.color = color;
            Handles.SphereHandleCap(0, pos, Quaternion.identity, size, EventType.Repaint);

            Vector3 newPos = Handles.PositionHandle(pos, Quaternion.identity);
            Handles.Label(pos + Vector3.up * size, label);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(marker, $"Move {label}");
                // 用世界坐标回写，保持 anchor 模式：改 anchoredPosition
                Transform parent = marker.parent;
                if (parent != null)
                {
                    Vector3 local = parent.InverseTransformPoint(newPos);
                    // 父级是拉伸满屏的 Lane：用父 Rect 把 local 转成锚点
                    if (parent is RectTransform parentRect && parentRect.rect.width > 1f && parentRect.rect.height > 1f)
                    {
                        Rect r = parentRect.rect;
                        float nx = (local.x - r.xMin) / r.width;
                        float ny = (local.y - r.yMin) / r.height;
                        marker.anchorMin = marker.anchorMax = new Vector2(nx, ny);
                        marker.anchoredPosition = Vector2.zero;
                    }
                    else
                    {
                        marker.position = newPos;
                    }
                }
                else
                {
                    marker.position = newPos;
                }

                EditorUtility.SetDirty(marker);
            }
        }
    }
}
