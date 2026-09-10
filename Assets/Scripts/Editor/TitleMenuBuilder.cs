using DeusEx.Rhythm;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DeusEx.Rhythm.EditorTools
{
    /// <summary>
    /// 生成可编辑的开始场景：左半边呼吸 Boss，右上游戏名，下方开始按钮。
    /// </summary>
    internal static class TitleMenuBuilder
    {
        private const string ScenePath = "Assets/Scenes/TitleMenu.unity";
        private const string BossPrefabPath = "Assets/Generated/Boss/Boss.prefab";
        private const string GameScenePath = "Assets/Scenes/GameDemo.unity";

        [MenuItem("Tools/DeusEx/Setup Title Menu Scene", false, 20)]
        public static void SetupTitleMenuScene()
        {
            EnsureSceneAsset();
            Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

            // 清掉旧的自动生成内容，避免重复执行叠多层
            DestroyNamed(scene, "TitleRoot");
            DestroyNamed(scene, "TitleBoss");
            DestroyNamed(scene, "Main Camera");
            DestroyNamed(scene, "EventSystem");
            DestroyNamed(scene, "Directional Light");

            CreateCamera();
            CreateLight();
            CreateEventSystem();

            GameObject titleRoot = new GameObject("TitleRoot", typeof(TitleMenuController));
            Undo.RegisterCreatedObjectUndo(titleRoot, "Create TitleRoot");
            var controller = titleRoot.GetComponent<TitleMenuController>();
            controller.gameSceneName = "GameDemo";
            controller.gameTitle = "DEUS EX MACHINA";

            // ---- Boss（左半屏，世界空间）----
            GameObject boss = PlaceBoss();
            var breath = boss.AddComponent<UiIdleMotion>();
            breath.rotateDegreesPerSecond = 0f;
            breath.scaleAmplitude = 0.035f;
            breath.scaleFrequency = 0.55f;
            breath.captureBaseScaleOnAwake = true;
            breath.useUnscaledTime = true;

            // 关掉局内行为，标题页只展示 Idle
            var combatDriver = boss.GetComponent<BossAnimationDriver>();
            if (combatDriver != null) combatDriver.playHitWhenEnemyDamaged = false;
            var behavior = boss.GetComponent<BossBehavior>();
            if (behavior != null) behavior.enabled = false;

            // ---- UI ----
            BuildUi(titleRoot.transform, controller);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            EnsureBuildSettings();
            Selection.activeGameObject = titleRoot;
            Debug.Log("[Title] TitleMenu 场景已生成。可直接挪 Boss / 标题 / 按钮。Build Settings 已把 TitleMenu 放在第一位。", titleRoot);
        }

        private static void EnsureSceneAsset()
        {
            if (System.IO.File.Exists(ScenePath)) return;
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            EditorSceneManager.SaveScene(scene, ScenePath);
        }

        private static void DestroyNamed(Scene scene, string name)
        {
            foreach (var root in scene.GetRootGameObjects())
            {
                if (root != null && root.name == name)
                    Undo.DestroyObjectImmediate(root);
            }
        }

        private static void CreateCamera()
        {
            var go = new GameObject("Main Camera");
            Undo.RegisterCreatedObjectUndo(go, "Create Main Camera");
            go.tag = "MainCamera";
            var cam = go.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.94f, 0.95f, 0.97f, 1f);
            cam.orthographic = true;
            cam.orthographicSize = 5f;
            cam.nearClipPlane = 0.1f;
            cam.farClipPlane = 100f;
            go.transform.position = new Vector3(0f, 0f, -10f);
            go.AddComponent<AudioListener>();
        }

        private static void CreateLight()
        {
            var go = new GameObject("Directional Light");
            Undo.RegisterCreatedObjectUndo(go, "Create Light");
            var light = go.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1f;
            go.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        }

        private static void CreateEventSystem()
        {
            if (Object.FindObjectOfType<EventSystem>() != null) return;
            var go = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            Undo.RegisterCreatedObjectUndo(go, "Create EventSystem");
        }

        private static GameObject PlaceBoss()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(BossPrefabPath);
            GameObject boss;
            if (prefab != null)
            {
                boss = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                Undo.RegisterCreatedObjectUndo(boss, "Instantiate Title Boss");
            }
            else
            {
                boss = new GameObject("TitleBoss", typeof(SpriteRenderer));
                Undo.RegisterCreatedObjectUndo(boss, "Create TitleBoss placeholder");
            }

            boss.name = "TitleBoss";
            // 左半屏：正交相机 size=5、aspect≈16:9 时世界宽约 17.8，左侧约 x=-4
            boss.transform.position = new Vector3(-3.6f, -0.4f, 0f);
            boss.transform.localScale = Vector3.one * 1.15f;

            var sr = boss.GetComponent<SpriteRenderer>();
            if (sr != null) sr.sortingOrder = 5;
            return boss;
        }

        private static void BuildUi(Transform titleRoot, TitleMenuController controller)
        {
            var canvasGo = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Undo.RegisterCreatedObjectUndo(canvasGo, "Create Title Canvas");
            canvasGo.transform.SetParent(titleRoot, false);

            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;

            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            // 右半区容器，方便整体挪
            var right = CreateRect("RightPanel", canvasGo.transform);
            Stretch(right, new Vector2(0.5f, 0f), new Vector2(1f, 1f), Vector2.zero, Vector2.zero);

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
            var colors = button.colors;
            colors.highlightedColor = new Color(0.25f, 0.28f, 0.34f, 1f);
            colors.pressedColor = new Color(0.08f, 0.09f, 0.12f, 1f);
            button.colors = colors;

            var labelGo = CreateRect("Label", buttonGo);
            Stretch(labelGo, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            var label = labelGo.gameObject.AddComponent<TextMeshProUGUI>();
            label.text = "START GAME";
            label.fontSize = 40f;
            label.fontStyle = FontStyles.Bold;
            label.alignment = TextAlignmentOptions.Center;
            label.color = Color.white;
            label.raycastTarget = false;

            controller.startButton = button;
        }

        private static RectTransform CreateRect(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go.GetComponent<RectTransform>();
        }

        private static void Stretch(RectTransform rect, Vector2 min, Vector2 max, Vector2 offMin, Vector2 offMax)
        {
            rect.anchorMin = min;
            rect.anchorMax = max;
            rect.offsetMin = offMin;
            rect.offsetMax = offMax;
        }

        private static void SetAnchored(RectTransform rect, Vector2 min, Vector2 max)
        {
            rect.anchorMin = min;
            rect.anchorMax = max;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static void EnsureBuildSettings()
        {
            var title = new EditorBuildSettingsScene(ScenePath, true);
            var game = new EditorBuildSettingsScene(GameScenePath, true);
            EditorBuildSettings.scenes = new[] { title, game };
        }
    }
}
