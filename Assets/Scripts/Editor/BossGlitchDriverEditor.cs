#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace DeusEx.Rhythm
{
    [CustomEditor(typeof(BossGlitchDriver))]
    public sealed class BossGlitchDriverEditor : Editor
    {
        private void OnSceneGUI()
        {
            var driver = (BossGlitchDriver)target;
            if (driver == null) return;

            driver.ApplyZonesFromFields();

            EditZone(driver, driver.frontZone, isFront: true);
            EditZone(driver, driver.backZone, isFront: false);
        }

        private static void EditZone(BossGlitchDriver driver, Transform zone, bool isFront)
        {
            if (zone == null) return;

            Handles.color = isFront ? new Color(0.2f, 1f, 0.35f, 1f) : new Color(0.35f, 0.95f, 0.55f, 1f);
            Handles.matrix = Matrix4x4.identity;

            EditorGUI.BeginChangeCheck();
            Vector3 pos = Handles.PositionHandle(zone.position, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(driver, isFront ? "Move Front Glitch Zone" : "Move Back Glitch Zone");
                Undo.RecordObject(zone, "Move Glitch Zone Transform");
                zone.position = pos;
                driver.CaptureFieldsFromZones();
                driver.ApplyZonesFromFields();
                EditorUtility.SetDirty(driver);
            }

            // 角点缩放：按父节点局部空间算半宽半高，避免累积误差
            Vector3 corner = zone.TransformPoint(new Vector3(0.5f, 0.5f, 0f));
            float handleSize = HandleUtility.GetHandleSize(corner) * 0.12f;
            EditorGUI.BeginChangeCheck();
            Vector3 newCorner = Handles.FreeMoveHandle(corner, handleSize, Vector3.zero, Handles.RectangleHandleCap);
            if (EditorGUI.EndChangeCheck() && zone.parent != null)
            {
                Undo.RecordObject(driver, isFront ? "Resize Front Glitch Zone" : "Resize Back Glitch Zone");
                Vector3 parentLocal = zone.parent.InverseTransformPoint(newCorner);
                Vector3 center = zone.localPosition;
                float halfX = Mathf.Abs(parentLocal.x - center.x);
                float halfY = Mathf.Abs(parentLocal.y - center.y);
                zone.localScale = new Vector3(Mathf.Max(0.05f, halfX * 2f), Mathf.Max(0.05f, halfY * 2f), 1f);
                driver.CaptureFieldsFromZones();
                driver.ApplyZonesFromFields();
                EditorUtility.SetDirty(driver);
            }

            Handles.Label(zone.position + Vector3.up * HandleUtility.GetHandleSize(zone.position) * 0.25f,
                isFront ? "Glitch Front (拖我)" : "Glitch Back (拖我)");
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var driver = (BossGlitchDriver)target;
            EditorGUILayout.Space(6);
            if (GUILayout.Button("Apply Layout Now"))
            {
                driver.ApplyZonesFromFields();
                EditorUtility.SetDirty(driver);
            }
            EditorGUILayout.HelpBox(
                "改 Front/Back Center、Size 会立刻移动绿色框。\n" +
                "也可在 Scene 视图拖绿色 Position 手柄，或拖右上角方块改大小。\n" +
                "方块运行时偶尔闪现，不会一直占满。",
                MessageType.Info);
        }
    }
}
#endif
