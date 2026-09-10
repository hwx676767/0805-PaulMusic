using DeusEx.Rhythm;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace DeusEx.Rhythm.EditorTools
{
    /// <summary>
    /// 在当前打开的局内场景挂上 Pause 面板（按 P 由 RhythmGameManager 切换）。
    /// </summary>
    internal static class PauseMenuBuilder
    {
        [MenuItem("Tools/DeusEx/Setup Pause Overlay In Open Scene", false, 21)]
        public static void SetupPauseOverlay()
        {
            var scene = EditorSceneManager.GetActiveScene();
            if (!scene.IsValid())
            {
                Debug.LogError("[Pause] 没有打开的场景。");
                return;
            }

            var existing = Object.FindObjectOfType<PauseMenuUI>();
            if (existing != null)
            {
                Selection.activeGameObject = existing.gameObject;
                Debug.Log("[Pause] 场景里已有 PauseMenuUI，已选中。", existing);
                return;
            }

            var manager = Object.FindObjectOfType<RhythmGameManager>();

            // 尽量挂到已有 Overlay Canvas 下
            Canvas canvas = null;
            foreach (var c in Object.FindObjectsOfType<Canvas>())
            {
                if (c.renderMode == RenderMode.ScreenSpaceOverlay)
                {
                    canvas = c;
                    break;
                }
            }

            if (canvas == null)
            {
                var canvasGo = new GameObject("PauseCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
                Undo.RegisterCreatedObjectUndo(canvasGo, "Create PauseCanvas");
                canvas = canvasGo.GetComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 200;
                var scaler = canvasGo.GetComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920f, 1080f);
            }

            var root = new GameObject("PauseMenu", typeof(RectTransform), typeof(PauseMenuUI));
            Undo.RegisterCreatedObjectUndo(root, "Create PauseMenu");
            root.transform.SetParent(canvas.transform, false);
            var rootRect = root.GetComponent<RectTransform>();
            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.offsetMin = Vector2.zero;
            rootRect.offsetMax = Vector2.zero;

            var panel = new GameObject("Panel", typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(root.transform, false);
            var panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;
            var dim = panel.GetComponent<Image>();
            dim.color = new Color(0.05f, 0.06f, 0.08f, 0.55f);
            dim.raycastTarget = true;

            var textGo = new GameObject("Hint", typeof(RectTransform));
            textGo.transform.SetParent(panel.transform, false);
            var textRect = textGo.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0.2f, 0.4f);
            textRect.anchorMax = new Vector2(0.8f, 0.6f);
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            var tmp = textGo.AddComponent<TextMeshProUGUI>();
            tmp.text = "PAUSED\nPress <b>P</b> to resume";
            tmp.fontSize = 56f;
            tmp.fontStyle = FontStyles.Bold;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;
            tmp.raycastTarget = false;

            var ui = root.GetComponent<PauseMenuUI>();
            ui.manager = manager;
            ui.pausePanel = panel;
            ui.hintText = tmp;
            panel.SetActive(false);

            EditorSceneManager.MarkSceneDirty(scene);
            Selection.activeGameObject = root;
            Debug.Log("[Pause] 已在当前场景加入 Pause 面板。局内按 P 暂停/继续。", root);
        }
    }
}
