using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DeusEx.Rhythm.EditorTools
{
    /// <summary>
    /// 在 UI Canvas / KnightHUD 下生成 NoteTrack 标记点，让音符沿 tracks.png 四条线下落。
    /// </summary>
    internal static class NoteTrackBuilder
    {
        private const string ScenePath = "Assets/Scenes/SampleScene.unity";
        private const string RootName = "NoteTrack";

        [MenuItem("Tools/DeusEx/Build Note Track Markers On HUD", false, 41)]
        public static void BuildFromMenu()
        {
            Build();
        }

        public static void Build()
        {
            Scene scene = EditorSceneManager.GetActiveScene();
            if (scene.path != ScenePath)
                scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

            Canvas canvas = FindUiCanvas();
            if (canvas == null)
                throw new System.InvalidOperationException("找不到含 CardWheel 的 UI Canvas。");

            Transform hud = canvas.transform.Find("KnightHUD");
            Transform parent = hud != null ? hud : canvas.transform;

            Transform existing = parent.Find(RootName);
            if (existing != null) Object.DestroyImmediate(existing.gameObject);

            var rootGo = new GameObject(RootName, typeof(RectTransform), typeof(UiNoteTrackLayout));
            Undo.RegisterCreatedObjectUndo(rootGo, "Build Note Track");
            var root = rootGo.GetComponent<RectTransform>();
            root.SetParent(parent, false);
            StretchFull(root);
            root.SetAsLastSibling();

            var noteLayer = new GameObject("Notes", typeof(RectTransform));
            var noteRoot = noteLayer.GetComponent<RectTransform>();
            noteRoot.SetParent(root, false);
            StretchFull(noteRoot);

            var layout = rootGo.GetComponent<UiNoteTrackLayout>();
            layout.noteRoot = noteRoot;
            layout.approachSeconds = 1.55f;
            layout.noteColor = Color.black;
            layout.noteSize = 52f;
            layout.lanes.Clear();

            // 默认对齐底部透视四轨（1920x1080 归一化锚点，可在 Scene 里再拖）
            Vector2[] spawns =
            {
                new Vector2(0.455f, 0.30f),
                new Vector2(0.485f, 0.30f),
                new Vector2(0.515f, 0.30f),
                new Vector2(0.545f, 0.30f),
            };
            Vector2[] hits =
            {
                new Vector2(0.30f, 0.085f),
                new Vector2(0.42f, 0.085f),
                new Vector2(0.58f, 0.085f),
                new Vector2(0.70f, 0.085f),
            };

            for (int i = 0; i < 4; i++)
            {
                var laneGo = new GameObject($"Lane{i}", typeof(RectTransform));
                var laneRect = laneGo.GetComponent<RectTransform>();
                laneRect.SetParent(root, false);
                StretchFull(laneRect);

                RectTransform spawn = CreateMarker(laneRect, "Spawn", spawns[i], new Color(0.3f, 0.75f, 1f, 0.85f));
                RectTransform hit = CreateMarker(laneRect, "Hit", hits[i], new Color(1f, 0.85f, 0.2f, 0.9f));
                layout.lanes.Add(new UiNoteTrackLayout.LanePath { spawn = spawn, hit = hit });
            }

            var manager = Object.FindFirstObjectByType<RhythmGameManager>();
            if (manager != null)
            {
                Undo.RecordObject(manager, "Assign UI Note Track");
                manager.uiTrack = layout;
                manager.proceduralCircleNote = true;
                manager.noteColor = Color.black;
                EditorUtility.SetDirty(manager);
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Selection.activeGameObject = rootGo;
            Debug.Log("[NoteTrack] 已生成 NoteTrack 标记。在 Scene 视图拖动各 Lane 的 Spawn/Hit 对齐 tracks.png 即可。", rootGo);
        }

        private static Canvas FindUiCanvas()
        {
            foreach (Canvas c in Object.FindObjectsByType<Canvas>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            {
                if (c != null && c.transform.Find("CardWheel") != null) return c;
            }
            return Object.FindFirstObjectByType<Canvas>();
        }

        private static RectTransform CreateMarker(RectTransform parent, string name, Vector2 anchor, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            var rect = go.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = rect.anchorMax = anchor;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(28f, 28f);
            rect.anchoredPosition = Vector2.zero;

            var img = go.GetComponent<Image>();
            img.color = color;
            img.raycastTarget = false;
            return rect;
        }

        private static void StretchFull(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.localScale = Vector3.one;
            rect.localRotation = Quaternion.identity;
        }
    }
}
