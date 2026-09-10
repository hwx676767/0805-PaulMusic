using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace DeusEx.Rhythm
{
    /// <summary>
    /// 打开 TitleMenu 场景后自动生成可编辑的开始界面（左 Boss 呼吸 + 右标题 + 开始按钮）。
    /// 生成一次后可关掉 autoBuildInEditor，避免覆盖你的手动调整。
    /// </summary>
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public sealed class TitleMenuBootstrap : MonoBehaviour
    {
        [Tooltip("编辑器里打开场景时，若还没有 TitleRoot 就自动搭建。")]
        public bool autoBuildInEditor = true;

        public string gameTitle = "DEUS EX MACHINA";
        public string gameSceneName = "GameDemo";

        [Tooltip("Boss 预制体；留空则用 Generated/Boss/Boss.prefab。")]
        public GameObject bossPrefab;

        private const string BossPrefabPath = "Assets/Generated/Boss/Boss.prefab";

        private void OnEnable()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying && autoBuildInEditor)
                TryBuild();
#endif
        }

        private void Start()
        {
            if (Application.isPlaying)
                TryBuild();
        }

        [ContextMenu("Rebuild Title Menu")]
        public void TryBuild()
        {
            if (GameObject.Find("TitleRoot") != null) return;
            Build();
        }

        public void Build()
        {
            EnsureCamera();
            EnsureEventSystem();
            EnsureLight();

            var existingBoss = GameObject.Find("TitleBoss");
            if (existingBoss != null)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying) DestroyImmediate(existingBoss);
                else
#endif
                    Destroy(existingBoss);
            }

            var titleRoot = new GameObject("TitleRoot", typeof(TitleMenuController));
            var controller = titleRoot.GetComponent<TitleMenuController>();
            controller.gameTitle = gameTitle;
            controller.gameSceneName = gameSceneName;

            PlaceBoss();
            BuildUi(titleRoot.transform, controller);

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                EditorSceneManager.MarkSceneDirty(gameObject.scene);
                Selection.activeGameObject = titleRoot;
            }
#endif
            Debug.Log("[Title] 开始界面已生成：TitleRoot / TitleBoss。可在 Hierarchy 中调整。", titleRoot);
        }

        private void PlaceBoss()
        {
            GameObject prefab = bossPrefab;
#if UNITY_EDITOR
            if (prefab == null)
                prefab = AssetDatabase.LoadAssetAtPath<GameObject>(BossPrefabPath);
#endif
            GameObject boss;
            if (prefab != null)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    boss = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                else
#endif
                    boss = Instantiate(prefab);
            }
            else
            {
                boss = new GameObject("TitleBoss", typeof(SpriteRenderer));
            }

            boss.name = "TitleBoss";
            boss.transform.position = new Vector3(-3.6f, -0.4f, 0f);
            boss.transform.localScale = Vector3.one * 1.15f;

            var breath = boss.GetComponent<UiIdleMotion>();
            if (breath == null) breath = boss.AddComponent<UiIdleMotion>();
            breath.rotateDegreesPerSecond = 0f;
            breath.scaleAmplitude = 0.035f;
            breath.scaleFrequency = 0.55f;
            breath.captureBaseScaleOnAwake = true;
            breath.useUnscaledTime = true;

            var driver = boss.GetComponent<BossAnimationDriver>();
            if (driver != null) driver.playHitWhenEnemyDamaged = false;
            var behavior = boss.GetComponent<BossBehavior>();
            if (behavior != null) behavior.enabled = false;

            var sr = boss.GetComponent<SpriteRenderer>();
            if (sr != null) sr.sortingOrder = 5;
        }

        private static void BuildUi(Transform titleRoot, TitleMenuController controller)
        {
            var canvasGo = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasGo.transform.SetParent(titleRoot, false);
            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;
            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            var right = CreateRect("RightPanel", canvasGo.transform);
            Stretch(right, new Vector2(0.5f, 0f), new Vector2(1f, 1f));

            var titleGo = CreateRect("GameTitle", right);
            SetAnchored(titleGo, new Vector2(0.08f, 0.62f), new Vector2(0.92f, 0.92f));
            var titleTmp = titleGo.gameObject.AddComponent<TextMeshProUGUI>();
            titleTmp.text = controller.gameTitle;
            titleTmp.fontSize = 72f;
            titleTmp.fontStyle = FontStyles.Bold;
            titleTmp.alignment = TextAlignmentOptions.TopRight;
            titleTmp.color = new Color(0.12f, 0.14f, 0.18f, 1f);
            titleTmp.enableWordWrapping = true;
            titleTmp.raycastTarget = false;
            controller.titleText = titleTmp;

            var subtitleGo = CreateRect("Subtitle", right);
            SetAnchored(subtitleGo, new Vector2(0.2f, 0.52f), new Vector2(0.92f, 0.62f));
            var sub = subtitleGo.gameObject.AddComponent<TextMeshProUGUI>();
            sub.text = "Rhythm  ·  Cards  ·  Boss";
            sub.fontSize = 28f;
            sub.alignment = TextAlignmentOptions.TopRight;
            sub.color = new Color(0.35f, 0.4f, 0.48f, 1f);
            sub.raycastTarget = false;

            var buttonGo = CreateRect("StartButton", right);
            SetAnchored(buttonGo, new Vector2(0.35f, 0.18f), new Vector2(0.92f, 0.32f));
            var img = buttonGo.gameObject.AddComponent<Image>();
            img.color = new Color(0.12f, 0.14f, 0.18f, 0.92f);
            var button = buttonGo.gameObject.AddComponent<Button>();
            button.targetGraphic = img;

            var labelGo = CreateRect("Label", buttonGo);
            Stretch(labelGo, Vector2.zero, Vector2.one);
            var label = labelGo.gameObject.AddComponent<TextMeshProUGUI>();
            label.text = "START GAME";
            label.fontSize = 40f;
            label.fontStyle = FontStyles.Bold;
            label.alignment = TextAlignmentOptions.Center;
            label.color = Color.white;
            label.raycastTarget = false;

            controller.startButton = button;
        }

        private static void EnsureCamera()
        {
            if (Camera.main != null) return;
            var go = new GameObject("Main Camera");
            go.tag = "MainCamera";
            var cam = go.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.94f, 0.95f, 0.97f, 1f);
            cam.orthographic = true;
            cam.orthographicSize = 5f;
            go.transform.position = new Vector3(0f, 0f, -10f);
            go.AddComponent<AudioListener>();
        }

        private static void EnsureLight()
        {
            if (Object.FindObjectOfType<Light>() != null) return;
            var go = new GameObject("Directional Light");
            var light = go.AddComponent<Light>();
            light.type = LightType.Directional;
            go.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        }

        private static void EnsureEventSystem()
        {
            if (Object.FindObjectOfType<EventSystem>() != null) return;
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }

        private static RectTransform CreateRect(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go.GetComponent<RectTransform>();
        }

        private static void Stretch(RectTransform rect, Vector2 min, Vector2 max)
        {
            rect.anchorMin = min;
            rect.anchorMax = max;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static void SetAnchored(RectTransform rect, Vector2 min, Vector2 max)
        {
            rect.anchorMin = min;
            rect.anchorMax = max;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }
}
