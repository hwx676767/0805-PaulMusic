using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DeusEx.Rhythm
{
    /// <summary>
    /// 游戏内暂停 UI：订阅 <see cref="RhythmGameManager.PauseChanged"/>，按 P 由 Manager 切换。
    /// 未指定面板时会在运行时自动生成一层半透明提示（仍可之后在场景里换成手调版）。
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PauseMenuUI : MonoBehaviour
    {
        [Tooltip("留空则自动查找 RhythmGameManager。")]
        public RhythmGameManager manager;

        [Tooltip("暂停时显示的面板根节点（建议全屏半透明底）。留空则运行时自动生成。")]
        public GameObject pausePanel;

        [Tooltip("可选：显示“PAUSED / Press P to resume”的文本。")]
        public TMP_Text hintText;

        [Tooltip("打开暂停面板时的默认文案。")]
        public string pausedHint = "PAUSED\nPress <b>P</b> to resume";

        private void Awake()
        {
            if (manager == null) manager = FindObjectOfType<RhythmGameManager>();
            if (pausePanel == null) BuildDefaultPanel();
        }

        private void OnEnable()
        {
            if (manager == null) manager = FindObjectOfType<RhythmGameManager>();
            if (manager != null) manager.PauseChanged += OnPauseChanged;
            Apply(manager != null && manager.IsPaused);
        }

        private void OnDisable()
        {
            if (manager != null) manager.PauseChanged -= OnPauseChanged;
        }

        private void OnPauseChanged(bool paused) => Apply(paused);

        private void Apply(bool paused)
        {
            if (pausePanel != null) pausePanel.SetActive(paused);
            if (hintText != null && paused) hintText.text = pausedHint;
        }

        private void BuildDefaultPanel()
        {
            var canvasGo = new GameObject("PauseCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasGo.transform.SetParent(transform, false);
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
            panelGo.GetComponent<Image>().color = new Color(0.05f, 0.06f, 0.08f, 0.55f);

            var textGo = new GameObject("Hint", typeof(RectTransform), typeof(TextMeshProUGUI));
            textGo.transform.SetParent(panelGo.transform, false);
            var textRect = textGo.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0.2f, 0.4f);
            textRect.anchorMax = new Vector2(0.8f, 0.6f);
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            hintText = textGo.GetComponent<TextMeshProUGUI>();
            hintText.text = pausedHint;
            hintText.fontSize = 56f;
            hintText.fontStyle = FontStyles.Bold;
            hintText.alignment = TextAlignmentOptions.Center;
            hintText.color = Color.white;
            hintText.raycastTarget = false;

            pausePanel = panelGo;
            panelGo.SetActive(false);
        }
    }
}
