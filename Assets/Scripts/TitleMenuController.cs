using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DeusEx.Rhythm
{
    /// <summary>
    /// 开始菜单：开始游戏按钮跳转局内场景。布局物体由 TitleMenuBuilder 生成，可在场景中自由调整。
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class TitleMenuController : MonoBehaviour
    {
        [Tooltip("开始游戏后加载的场景名（需在 Build Settings 里）。")]
        public string gameSceneName = "GameDemo";

        [Tooltip("开始按钮。留空则在子物体里找名为 StartButton 的 Button。")]
        public Button startButton;

        [Tooltip("标题文本（可改游戏名）。")]
        public TMP_Text titleText;

        [Tooltip("默认显示的游戏名。")]
        public string gameTitle = "DEUS EX MACHINA";

        private void Awake()
        {
            if (titleText != null && !string.IsNullOrWhiteSpace(gameTitle))
                titleText.text = gameTitle;

            if (startButton == null)
            {
                var t = transform.Find("Canvas/RightPanel/StartButton");
                if (t != null) startButton = t.GetComponent<Button>();
            }

            if (startButton != null)
            {
                startButton.onClick.RemoveListener(OnStartClicked);
                startButton.onClick.AddListener(OnStartClicked);
            }
        }

        public void OnStartClicked()
        {
            if (string.IsNullOrEmpty(gameSceneName))
            {
                Debug.LogError("[Title] gameSceneName 为空。");
                return;
            }
            SceneManager.LoadScene(gameSceneName);
        }
    }
}
