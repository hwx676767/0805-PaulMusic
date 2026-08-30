using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DeusEx.Rhythm.EditorTools
{
    /// <summary>
    /// 根据 Resources/UI 切图与参考构图，在 SampleScene 的 Canvas 下生成 Knight HUD（编辑器内写入场景，非运行时生成）。
    /// </summary>
    internal static class KnightHudBuilder
    {
        private const string ScenePath = "Assets/Scenes/SampleScene.unity";
        private const string UiRoot = "Assets/Resources/UI";
        private const string HudName = "KnightHUD";
        private const string AutoFlag = "DeusEx.KnightHud.AutoBuilt.v2";

        [InitializeOnLoadMethod]
        private static void AutoBuildIfMissing()
        {
            // Unity 正打开工程时：脚本编译完成后若正确 Canvas 下还没有 HUD，则自动写入一次。
            if (SessionState.GetBool(AutoFlag, false)) return;
            EditorApplication.delayCall += () =>
            {
                if (SessionState.GetBool(AutoFlag, false)) return;
                if (Application.isPlaying) return;
                if (EditorSceneManager.GetActiveScene().path != ScenePath) return;

                Canvas uiCanvas = FindUiCanvas();
                if (uiCanvas == null) return;
                if (uiCanvas.transform.Find(HudName) != null)
                {
                    SessionState.SetBool(AutoFlag, true);
                    return;
                }

                try
                {
                    BuildInternal(reopenScene: false);
                    SessionState.SetBool(AutoFlag, true);
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"[KnightHUD] 自动生成失败，请手动点菜单 Tools/DeusEx/Build Knight HUD。\n{e}");
                }
            };
        }

        [MenuItem("Tools/DeusEx/Build Knight HUD In SampleScene", false, 40)]
        public static void BuildFromMenu()
        {
            SessionState.EraseBool(AutoFlag);
            Build();
            SessionState.SetBool(AutoFlag, true);
        }

        /// <summary>供命令行 -executeMethod 调用。</summary>
        public static void Build()
        {
            BuildInternal(reopenScene: true);
        }

        private static void BuildInternal(bool reopenScene)
        {
            Scene scene = reopenScene
                ? EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single)
                : EditorSceneManager.GetActiveScene();

            if (scene.path != ScenePath)
                throw new System.InvalidOperationException($"当前场景不是 SampleScene：{scene.path}");

            // 必须挂到带 CardWheel/DisplayText 的根 UI Canvas，而不是 Playfield 里的 Canvas。
            Canvas canvas = FindUiCanvas();
            if (canvas == null)
                throw new System.InvalidOperationException(
                    "找不到 UI Canvas（需包含 CardWheel 或 DisplayText 的那个 Canvas）。");

            // 清理场景里所有旧 KnightHUD（可能误挂在 Playfield Canvas 下）
            DestroyAllNamed(HudName);

            var hud = new GameObject(HudName, typeof(RectTransform));
            Undo.RegisterCreatedObjectUndo(hud, "Build Knight HUD");
            var hudRect = hud.GetComponent<RectTransform>();
            hudRect.SetParent(canvas.transform, false);
            StretchFull(hudRect);
            hudRect.SetAsFirstSibling(); // 在 CardWheel / DisplayText 之下
            hud.layer = canvas.gameObject.layer;

            // 图层自下而上（与参考截图叠层一致）
            CreateLayer(hudRect, "Layer_Background", "background.png", 0);
            CreateLayer(hudRect, "Layer_Tracks", "tracks.png", 1);
            CreateLayer(hudRect, "Layer_Lines", "lines.png", 2);

            Image hpFill = CreateLayer(hudRect, "BossHpFill", "hp bar.png", 3);
            hpFill.type = Image.Type.Filled;
            hpFill.fillMethod = Image.FillMethod.Horizontal;
            hpFill.fillOrigin = (int)Image.OriginHorizontal.Left;
            hpFill.fillAmount = 1f;
            var healthBar = hud.AddComponent<HealthBar>();
            healthBar.target = HealthBar.Side.Enemy;
            healthBar.fillImage = hpFill;
            healthBar.slider = null;
            healthBar.combat = Object.FindFirstObjectByType<CombatSystem>();
            healthBar.smooth = true;

            CreateLayer(hudRect, "Layer_BossTitle", "the knight.png", 4);

            Image comboBg = CreateLayer(hudRect, "Layer_ComboBg", "combo数量背后的矩形.png", 5);
            BuildComboBlock(hudRect, comboBg);

            CreateLayer(hudRect, "Layer_CostOutside", "cost outside.png", 6);
            Image costInside = CreateLayer(hudRect, "Layer_CostInside", "cost inside.png", 7);
            if (costInside != null && costInside.GetComponent<UiIdleMotion>() == null)
                costInside.gameObject.AddComponent<UiIdleMotion>();
            CreateLayer(hudRect, "Layer_Drive", "drive.png", 8);

            BuildInfoPanel(hudRect);
            RelayoutExistingHud(canvas.transform);
            DisableLegacyBossSlider();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Debug.Log($"[KnightHUD] 已写入到 UI Canvas「{canvas.name}」下。", hud);
        }

        /// <summary>
        /// 选中真正的游戏 HUD Canvas：优先含 CardWheel / DisplayText / NoteCounts 的那个，
        /// 避免误选 Playfield 预制体内部的 Canvas（常带 Drive / SpawnParent）。
        /// </summary>
        private static Canvas FindUiCanvas()
        {
            Canvas[] canvases = Object.FindObjectsByType<Canvas>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            Canvas best = null;
            int bestScore = -1;
            foreach (Canvas c in canvases)
            {
                if (c == null) continue;
                Transform t = c.transform;
                int score = 0;
                if (t.Find("CardWheel") != null) score += 10;
                if (t.Find("DisplayText") != null) score += 8;
                if (t.Find("NoteCounts") != null) score += 6;
                if (c.gameObject.name == "Canvas" && t.parent == null) score += 3;
                if (c.renderMode == RenderMode.ScreenSpaceOverlay) score += 2;
                // Playfield 内嵌 Canvas 常有这些子物体，降权
                if (t.Find("SpawnParent") != null) score -= 20;
                if (t.Find("Drive") != null && t.Find("CardWheel") == null) score -= 10;
                if (c.gameObject.name == "InFrameCanvas") score -= 15;

                if (score > bestScore)
                {
                    bestScore = score;
                    best = c;
                }
            }

            return bestScore > 0 ? best : null;
        }

        private static void DestroyAllNamed(string objectName)
        {
            var toDestroy = new System.Collections.Generic.List<GameObject>();
            var roots = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (GameObject root in roots)
            {
                foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
                {
                    if (child != null && child.name == objectName)
                        toDestroy.Add(child.gameObject);
                }
            }

            for (int i = 0; i < toDestroy.Count; i++)
            {
                if (toDestroy[i] != null)
                    Object.DestroyImmediate(toDestroy[i]);
            }
        }

        private static void BuildComboBlock(RectTransform hud, Image comboBg)
        {
            // 参考图：右上 Combo 数字 + COMBO 字样，叠在矩形底上
            var comboRoot = new GameObject("ComboBlock", typeof(RectTransform));
            var root = comboRoot.GetComponent<RectTransform>();
            root.SetParent(hud, false);
            root.anchorMin = new Vector2(0.62f, 0.72f);
            root.anchorMax = new Vector2(0.92f, 0.96f);
            root.offsetMin = Vector2.zero;
            root.offsetMax = Vector2.zero;
            root.SetAsLastSibling();

            TMP_Text number = CreateTmp(root, "ComboNumber", "0", 96f, FontStyles.Bold | FontStyles.Italic,
                TextAlignmentOptions.BottomRight, new Color(0.55f, 0.55f, 0.58f, 1f));
            var numberRect = number.rectTransform;
            numberRect.anchorMin = new Vector2(0f, 0.35f);
            numberRect.anchorMax = new Vector2(1f, 1f);
            numberRect.offsetMin = new Vector2(10f, 0f);
            numberRect.offsetMax = new Vector2(-18f, -8f);

            TMP_Text label = CreateTmp(root, "ComboLabel", "COMBO", 28f, FontStyles.Bold,
                TextAlignmentOptions.TopRight, new Color(0.12f, 0.12f, 0.14f, 1f));
            var labelRect = label.rectTransform;
            labelRect.anchorMin = new Vector2(0f, 0f);
            labelRect.anchorMax = new Vector2(1f, 0.42f);
            labelRect.offsetMin = new Vector2(10f, 8f);
            labelRect.offsetMax = new Vector2(-22f, 0f);
            label.characterSpacing = 12f;

            var display = comboRoot.AddComponent<ComboDisplay>();
            display.label = number;
            display.format = "{0}";
            display.hideBelow = 0;
            display.manager = Object.FindFirstObjectByType<RhythmGameManager>();
            display.punchOnChange = true;

            if (comboBg != null)
                comboBg.raycastTarget = false;
        }

        private static void BuildInfoPanel(RectTransform hud)
        {
            var go = new GameObject("SongInfoBox", typeof(RectTransform));
            var rect = go.GetComponent<RectTransform>();
            rect.SetParent(hud, false);
            rect.anchorMin = new Vector2(0.78f, 0.78f);
            rect.anchorMax = new Vector2(0.98f, 0.94f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var outline = go.AddComponent<Image>();
            outline.color = new Color(0.1f, 0.1f, 0.12f, 0.18f);
            outline.raycastTarget = false;

            TMP_Text text = CreateTmp(rect, "SongInfoText",
                "String Song_name = \"Demo\";\nint attack = 200;",
                18f, FontStyles.Normal, TextAlignmentOptions.TopLeft,
                new Color(0.15f, 0.15f, 0.18f, 1f));
            text.fontStyle = FontStyles.Normal;
            text.enableWordWrapping = true;
            var tr = text.rectTransform;
            tr.offsetMin = new Vector2(14f, 10f);
            tr.offsetMax = new Vector2(-14f, -10f);

            var binder = go.AddComponent<SongInfoHud>();
            binder.label = text;
        }

        private static void RelayoutExistingHud(Transform canvas)
        {
            // DisplayText：参考图右侧信息区下方，靠近卡牌
            var display = canvas.Find("DisplayText") as RectTransform;
            if (display != null)
            {
                display.anchorMin = new Vector2(1f, 0f);
                display.anchorMax = new Vector2(1f, 0f);
                display.pivot = new Vector2(1f, 0f);
                display.anchoredPosition = new Vector2(-40f, 280f);
                display.sizeDelta = new Vector2(520f, 160f);
            }

            // CardWheel：贴合右下 Drive / Cost 轮盘区域
            var wheel = canvas.Find("CardWheel") as RectTransform;
            if (wheel != null)
            {
                wheel.anchorMin = new Vector2(1f, 0f);
                wheel.anchorMax = new Vector2(1f, 0f);
                wheel.pivot = new Vector2(0.5f, 0.5f);
                wheel.anchoredPosition = new Vector2(-280f, 220f);
                wheel.sizeDelta = new Vector2(450f, 450f);
                wheel.SetAsLastSibling();
            }

            if (display != null)
                display.SetAsLastSibling();
        }

        private static void DisableLegacyBossSlider()
        {
            // 旧的世界空间 Slider 血条改为关闭，避免与新顶栏血条重复
            foreach (var hb in Object.FindObjectsByType<HealthBar>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (hb == null || hb.gameObject.name != "Slider") continue;
                if (hb.target != HealthBar.Side.Enemy) continue;
                if (hb.GetComponentInParent<Canvas>()?.name == "InFrameCanvas")
                {
                    hb.gameObject.SetActive(false);
                    break;
                }
            }
        }

        private static Image CreateLayer(RectTransform parent, string name, string fileName, int sibling)
        {
            Sprite sprite = LoadUiSprite(fileName);
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            var rect = go.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            StretchFull(rect);
            rect.SetSiblingIndex(Mathf.Clamp(sibling, 0, parent.childCount));

            var image = go.GetComponent<Image>();
            image.sprite = sprite;
            image.preserveAspect = false;
            image.raycastTarget = false;
            image.color = Color.white;
            if (sprite == null)
                Debug.LogWarning($"[KnightHUD] 未找到 UI 图：{fileName}", go);
            return image;
        }

        private static Sprite LoadUiSprite(string fileName)
        {
            string path = $"{UiRoot}/{fileName}";
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sprite != null) return sprite;

            // 若导入尚未生成 Sprite 子资源，强制设为 Sprite 再取
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null && importer.textureType != TextureImporterType.Sprite)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.alphaIsTransparency = true;
                importer.mipmapEnabled = false;
                importer.SaveAndReimport();
                sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            }
            return sprite;
        }

        private static TMP_Text CreateTmp(RectTransform parent, string name, string content, float size,
            FontStyles style, TextAlignmentOptions align, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform));
            var rect = go.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            StretchFull(rect);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = content;
            tmp.fontSize = size;
            tmp.fontStyle = style;
            tmp.alignment = align;
            tmp.color = color;
            tmp.raycastTarget = false;
            tmp.enableWordWrapping = false;
            tmp.overflowMode = TextOverflowModes.Overflow;
            return tmp;
        }

        private static void StretchFull(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.localScale = Vector3.one;
            rect.localRotation = Quaternion.identity;
        }
    }
}
