using DeusEx.Rhythm;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace DeusEx.Rhythm.EditorTools
{
    /// <summary>
    /// 在当前打开的局内场景挂上暂停面板（半透明 + PAUSED 文案）。
    /// </summary>
    internal static class PauseOverlayBuilder
    {
        [MenuItem("Tools/DeusEx/Setup Pause Overlay In Open Scene", false, 21)]
        public static void SetupPauseOverlay()
        {
            var scene = EditorSceneManager.GetActiveScene();
            if (!scene.IsValid())
            {
                Debug.LogError("[Pause] 请先打开 GameDemo 场景。");
                return;
            }

            var existing = Object.FindObjectOfType<PauseMenuUI>();
            if (existing != null)
            {
                Selection.activeGameObject = existing.gameObject;
                Debug.Log("[Pause] 场景里已有 PauseMenuUI，已选中。", existing);
                return;
            }

            var root = new GameObject("PauseMenu", typeof(PauseMenuUI));
            Undo.RegisterCreatedObjectUndo(root, "Create PauseMenu");

            var canvasGo = new GameObject("PauseCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Undo.RegisterCreatedObjectUndo(canvasGo, "Create PauseCanvas");
            canvasGo.transform.SetParent(root.transform, false);

            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 200;

            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            var panelGo = new GameObject("PausePanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            panelGo.transform.SetParent(canvasGo.transform, false);
            var panelRect = panelGo.GetComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;
            var panelImg = panelGo.GetComponent<Image>();
            panelImg.color = new Color(0.05f, 0.06f, 0.08f, 0.55f);
            panelImg.raycastTarget = true;

            var textGo = new GameObject("Hint", typeof(RectTransform), typeof(TextMeshProUGUI));
            textGo.transform.SetParent(panelGo.transform, false);
            var textRect = textGo.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0.2f, 0.4f);
            textRect.anchorMax = new Vector2(0.8f, 0.6f);
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            var tmp = textGo.GetComponent<TextMeshProUGUI>();
            tmp.text = "PAUSED\nPress <b>P</b> to resume";
            tmp.fontSize = 56f;
            tmp.fontStyle = FontStyles.Bold;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;
            tmp.raycastTarget = false;

            var ui = root.GetComponent<PauseMenuUI>();
            ui.manager = Object.FindObjectOfType<RhythmGameManager>();
            ui.pausePanel = panelGo;
            ui.hintText = tmp;
            panelGo.SetActive(false);

            EditorSceneManager.MarkSceneDirty(scene);
            Selection.activeGameObject = root;
            Debug.Log("[Pause] 已添加 PauseMenu。局内按 P 暂停/继续。", root);
        }
    }
}
