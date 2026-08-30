using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DeusEx.Rhythm.EditorTools
{
    /// <summary>
    /// 在 KnightHUD 下生成可拖动的 Spawn/Hit 标记，供假透视轨道路径使用。
    /// </summary>
    internal static class NoteTrackBuilder
    {
        private const string ScenePath = "Assets/Scenes/SampleScene.unity";
        private const string RootName = "NoteTrack";
        private const string AutoFlag = "DeusEx.NoteTrack.AutoBuilt.v2";

        [InitializeOnLoadMethod]
        private static void AutoBuildIfMissing()
        {
            if (SessionState.GetBool(AutoFlag, false)) return;
            EditorApplication.delayCall += () =>
            {
                if (SessionState.GetBool(AutoFlag, false)) return;
                if (Application.isPlaying) return;
                if (EditorSceneManager.GetActiveScene().path != ScenePath) return;

                Canvas canvas = FindUiCanvas();
                if (canvas == null) return;
                Transform hud = canvas.transform.Find("KnightHUD");
                Transform parent = hud != null ? hud : canvas.transform;
                if (parent.Find(RootName) != null)
                {
                    SessionState.SetBool(AutoFlag, true);
                    WireManager(parent.Find(RootName).GetComponent<UiNoteTrackLayout>());
                    return;
                }

                try
                {
                    BuildInternal(save: true);
                    SessionState.SetBool(AutoFlag, true);
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"[NoteTrack] 自动生成失败，请点菜单 Tools/DeusEx/Build Note Track Markers On HUD\n{e}");
                }
            };
        }

        [MenuItem("Tools/DeusEx/Build Note Track Markers On HUD", false, 41)]
        public static void BuildFromMenu()
        {
            SessionState.EraseBool(AutoFlag);
            BuildInternal(save: true);
            SessionState.SetBool(AutoFlag, true);
        }

        public static void Build() => BuildInternal(save: true);

        private static void BuildInternal(bool save)
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

            // 默认假透视：上端收拢、下端张开（对齐底部 tracks 视觉，可再拖）
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

                RectTransform spawn = CreateMarker(laneRect, "Spawn", spawns[i], new Color(0.25f, 0.85f, 1f, 0.95f));
                RectTransform hit = CreateMarker(laneRect, "Hit", hits[i], new Color(1f, 0.85f, 0.15f, 0.95f));
                layout.lanes.Add(new UiNoteTrackLayout.LanePath { spawn = spawn, hit = hit });
            }

            WireManager(layout);

            EditorSceneManager.MarkSceneDirty(scene);
            if (save) EditorSceneManager.SaveScene(scene);
            Selection.activeGameObject = rootGo;
            Debug.Log("[NoteTrack] 已生成可拖动起点/终点。选中 NoteTrack，在 Scene 里拖蓝色 Spawn / 黄色 Hit。", rootGo);
        }

        private static void WireManager(UiNoteTrackLayout layout)
        {
            if (layout == null) return;
            var manager = Object.FindFirstObjectByType<RhythmGameManager>();
            if (manager == null) return;
            Undo.RecordObject(manager, "Assign UI Note Track");
            manager.uiTrack = layout;
            manager.proceduralCircleNote = true;
            manager.noteColor = Color.black;
            // UI 轨道模式不用世界 notePrefab，避免仍显示青色菱形
            manager.notePrefab = null;
            EditorUtility.SetDirty(manager);
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
            rect.sizeDelta = new Vector2(40f, 40f);
            rect.anchoredPosition = Vector2.zero;

            var img = go.GetComponent<Image>();
            img.color = color;
            img.raycastTarget = true; // 方便在 Game/Scene 点选
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
