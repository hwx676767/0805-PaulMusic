using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace DeusEx.Rhythm.EditorTools
{
    /// <summary>
    /// 编辑器工具：一键生成可在编辑器里编辑的预制体——
    /// 一个完整的 Playfield（4 轨 + 判定线 + 可见上界标记）以及 Note / HoldBody，外加一张白色精灵。
    /// 由 Unity 自己创建资源，保证 GUID/引用正确。
    /// 菜单：Tools/Rhythm/Generate Default Prefabs。
    /// </summary>
    public static class RhythmPrefabBuilder
    {
        private const string GenDir = "Assets/RhythmGame/Generated";

        // 共享布局参数（生成 Lane / Playfield 时使用）
        private const float LaneWidth = 1.2f;
        private const float JudgeY = -4f;
        private const float TopY = 5.5f;
        private const float FieldHeight = TopY - (JudgeY - 1.5f);
        private const float FieldCenterY = (TopY + (JudgeY - 1.5f)) * 0.5f;

        [MenuItem("Tools/Rhythm/Generate Default Prefabs")]
        public static void GenerateDefaultPrefabs()
        {
            EnsureFolder(GenDir);

            Sprite sprite = CreateWhiteSprite(GenDir + "/white.png");

            GameObject lanePrefab = BuildLanePrefab(GenDir + "/Lane.prefab", sprite);
            BuildPlayfieldPrefab(GenDir + "/Playfield.prefab", sprite, lanePrefab);
            GameObject note = BuildSpritePrefab(GenDir + "/Note.prefab", sprite,
                new Color(0.2f, 0.8f, 1f), 20, new Vector3(1f, 0.32f, 1f));
            GameObject hold = BuildSpritePrefab(GenDir + "/HoldBody.prefab", sprite,
                new Color(1f, 0.7f, 0.25f, 0.55f), 18, new Vector3(0.6f, 1f, 1f));

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            AssignNotePrefabsToManager(note, hold);

            EditorUtility.DisplayDialog("Rhythm",
                "已在 " + GenDir + " 生成：\n• Lane.prefab（单条轨道，编辑它可让所有轨道一起更新）\n" +
                "• Playfield.prefab（含 4 个 Lane 嵌套预制体实例 + 判定线）\n• Note.prefab / HoldBody.prefab\n\n" +
                "把 Playfield.prefab 拖进场景使用。Note/HoldBody 已自动赋给场景里的 RhythmGameManager。",
                "好的");
        }

        // ---------------- 判定弹出提示（Perfect/Great/Good/Miss 各一个 prefab） ----------------

        [MenuItem("Tools/Rhythm/Generate Judgement Popup Prefabs")]
        public static void GenerateJudgementPopupPrefabs()
        {
            // 优先写到工程里放 popup 的位置（与现有一致），否则用工具默认目录
            string dir = AssetDatabase.IsValidFolder("Assets/Prefabs") ? "Assets/Prefabs" : GenDir;
            EnsureFolder(dir);

            string basePath = dir + "/Popup_Base.prefab";
            string[] variantPaths =
            {
                dir + "/Popup_Perfect.prefab", dir + "/Popup_Great.prefab",
                dir + "/Popup_Good.prefab", dir + "/Popup_Miss.prefab",
            };

            bool anyExists = AssetDatabase.LoadAssetAtPath<GameObject>(basePath) != null;
            foreach (var p in variantPaths) anyExists |= AssetDatabase.LoadAssetAtPath<GameObject>(p) != null;
            if (anyExists && !EditorUtility.DisplayDialog("Rhythm",
                    "将在 " + dir + " 生成 Popup_Base.prefab，并把 Perfect/Great/Good/Miss 改造成它的变体（Prefab Variant）。\n\n" +
                    "已存在的同名预制体会被覆盖：字号/对齐/排序/动画等统一由 Base 控制，\n" +
                    "四个变体只保留各自的【文字】与【颜色】覆盖。\n" +
                    "（你之前对单个 popup 的字号/上浮等个别改动会被重置，之后可在变体里重新覆盖。）\n\n" +
                    "建议先用 Git 备份。是否继续？", "继续", "取消"))
                return;

            // 1) 共享基底：所有通用设置都在这里
            GameObject basePrefab = BuildPopupPrefab(basePath, "JUDGE", Color.white);

            // 2) 四个变体：只覆盖文字 + 颜色，其余继承 Base
            var perfect = SavePopupVariant(basePrefab, variantPaths[0], "PERFECT", new Color(1f, 0.85f, 0.3f));
            var great = SavePopupVariant(basePrefab, variantPaths[1], "GREAT", new Color(0.4f, 1f, 0.6f));
            var good = SavePopupVariant(basePrefab, variantPaths[2], "GOOD", new Color(0.5f, 0.8f, 1f));
            var miss = SavePopupVariant(basePrefab, variantPaths[3], "MISS", new Color(1f, 0.4f, 0.4f));

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            AssignPopupPrefabsToSpawner(perfect, great, good, miss);

            EditorUtility.DisplayDialog("Rhythm",
                "完成（在 " + dir + "）：\n" +
                "• Popup_Base.prefab —— 共享基底，改它（字号/对齐/排序/JudgementPopup 动画等）四个一起变。\n" +
                "• Popup_Perfect/Great/Good/Miss —— 它的变体，仅覆盖各自的文字与颜色。\n\n" +
                "场景里的 JudgementPopupSpawner 已重新指向这四个变体；若 spawner 在 Playfield 预制体内，\n" +
                "记得对其实例右键 Apply（或进预制体里）确认一下。",
                "好的");
        }

        // 共享基底：含所有通用设置；变体只在此之上覆盖文字/颜色
        private static GameObject BuildPopupPrefab(string path, string text, Color color)
        {
            var go = new GameObject(Path.GetFileNameWithoutExtension(path));

            var tmp = go.AddComponent<TextMeshPro>(); // 世界空间 3D 文本，会自动带上 RectTransform 与 MeshRenderer
            tmp.text = text;
            tmp.color = color;
            tmp.fontSize = 10f;
            tmp.fontStyle = FontStyles.Bold;
            tmp.alignment = TextAlignmentOptions.Center;
#pragma warning disable CS0618 // enableWordWrapping 新版标记过时但仍可用，兼容旧版本
            tmp.enableWordWrapping = false;
#pragma warning restore CS0618

            var rt = go.GetComponent<RectTransform>();
            if (rt != null) rt.sizeDelta = new Vector2(10f, 3f);

            // 画在音符(sortingOrder 20)之上
            var mr = go.GetComponent<MeshRenderer>();
            if (mr != null) mr.sortingOrder = 30;

            go.AddComponent<JudgementPopup>();

            return SavePrefab(go, path);
        }

        // 以 basePrefab 的实例为根另存 → Unity 会生成它的「预制体变体」，所改的文字/颜色记为变体覆盖
        private static GameObject SavePopupVariant(GameObject basePrefab, string path, string text, Color color)
        {
            var inst = (GameObject)PrefabUtility.InstantiatePrefab(basePrefab);
            var tmp = inst.GetComponent<TextMeshPro>();
            if (tmp != null)
            {
                tmp.text = text;
                tmp.color = color;
            }
            var variant = PrefabUtility.SaveAsPrefabAsset(inst, path);
            Object.DestroyImmediate(inst);
            return variant;
        }

        private static void AssignPopupPrefabsToSpawner(GameObject perfect, GameObject great, GameObject good, GameObject miss)
        {
            var spawner = Object.FindObjectOfType<JudgementPopupSpawner>();
            if (spawner == null)
            {
                var go = new GameObject("JudgementFeedback");
                Undo.RegisterCreatedObjectUndo(go, "Create JudgementFeedback");
                spawner = go.AddComponent<JudgementPopupSpawner>();
            }

            Undo.RecordObject(spawner, "Assign Judgement Popup Prefabs");
            spawner.perfectPrefab = perfect;
            spawner.greatPrefab = great;
            spawner.goodPrefab = good;
            spawner.missPrefab = miss;

            // 默认把父物体设为场景里的 Playfield，让弹出物自动贴合其旋转/缩放；
            // 想放到 Playfield 下的 World Space Canvas 里，把 spawnParent 改成那个 Canvas 即可。
            if (spawner.spawnParent == null)
            {
                var pf = Object.FindObjectOfType<Playfield>();
                if (pf != null) spawner.spawnParent = pf.transform;
            }
            EditorUtility.SetDirty(spawner);
        }

        // ---------------- 把现有 Playfield 的轨道转成 Lane.prefab 嵌套实例 ----------------

        private struct LaneInfo
        {
            public string name;
            public Vector3 localPosition;
            public Vector3 localScale;
            public KeyCode key;
            public bool hasBgColor;
            public Color bgColor;
        }

        [MenuItem("Tools/Rhythm/Convert Selected Playfield Lanes To Prefab")]
        public static void ConvertSelectedPlayfieldLanesToPrefab()
        {
            var selected = Selection.activeObject as GameObject;
            string pfPath = selected != null ? AssetDatabase.GetAssetPath(selected) : null;
            if (string.IsNullOrEmpty(pfPath) || !pfPath.EndsWith(".prefab"))
            {
                EditorUtility.DisplayDialog("Rhythm",
                    "请先在 Project 窗口里选中要转换的 Playfield 预制体（.prefab）。", "好的");
                return;
            }

            string dir = Path.GetDirectoryName(pfPath).Replace("\\", "/");
            string lanePath = dir + "/Lane.prefab";

            if (!EditorUtility.DisplayDialog("Rhythm",
                    "将把\n" + pfPath + "\n里的轨道转换为嵌套预制体实例，\n并用第 1 条轨道的外观（重新）生成：\n" + lanePath +
                    "\n\n每条轨道的位置/按键/底色会作为实例覆盖保留。\n建议先用 Git 备份。是否继续？",
                    "继续", "取消"))
                return;

            GameObject root = PrefabUtility.LoadPrefabContents(pfPath);
            try
            {
                var pf = root.GetComponent<Playfield>();
                if (pf == null)
                {
                    EditorUtility.DisplayDialog("Rhythm", "选中的预制体根节点没有 Playfield 组件。", "好的");
                    return;
                }

                // 收集现有轨道：优先用 lanes 列表，否则扫描子物体
                var lanes = new List<Lane>();
                if (pf.lanes != null)
                    foreach (var l in pf.lanes) if (l != null) lanes.Add(l);
                if (lanes.Count == 0)
                    lanes.AddRange(root.GetComponentsInChildren<Lane>(true));

                if (lanes.Count == 0)
                {
                    EditorUtility.DisplayDialog("Rhythm", "Playfield 里找不到任何 Lane 轨道。", "好的");
                    return;
                }

                // 如果已经是预制体实例，提示无需转换
                if (PrefabUtility.IsPartOfAnyPrefab(lanes[0].gameObject))
                {
                    EditorUtility.DisplayDialog("Rhythm", "这些轨道已经是预制体实例，无需转换。", "好的");
                    return;
                }

                // 记录每条轨道信息
                var infos = new List<LaneInfo>(lanes.Count);
                foreach (var lane in lanes)
                {
                    var info = new LaneInfo
                    {
                        name = lane.name,
                        localPosition = lane.transform.localPosition,
                        localScale = lane.transform.localScale,
                        key = lane.key,
                    };
                    if (lane.background != null)
                    {
                        info.hasBgColor = true;
                        info.bgColor = lane.background.color;
                    }
                    infos.Add(info);
                }

                // 用第 1 条轨道（去掉父级、归零位移）生成/覆盖 Lane.prefab
                GameObject laneCopy = Object.Instantiate(lanes[0].gameObject);
                laneCopy.name = "Lane";
                laneCopy.transform.SetParent(null);
                laneCopy.transform.localPosition = Vector3.zero;
                laneCopy.transform.localRotation = Quaternion.identity;
                laneCopy.transform.localScale = Vector3.one;
                GameObject lanePrefab = PrefabUtility.SaveAsPrefabAsset(laneCopy, lanePath);
                Object.DestroyImmediate(laneCopy);

                // 删除旧的普通轨道
                foreach (var lane in lanes)
                    if (lane != null) Object.DestroyImmediate(lane.gameObject);

                // 以 Lane.prefab 的嵌套实例重建，恢复位置/按键/底色作为覆盖
                pf.lanes.Clear();
                foreach (var info in infos)
                {
                    var inst = (GameObject)PrefabUtility.InstantiatePrefab(lanePrefab, root.transform);
                    inst.name = info.name;
                    inst.transform.localPosition = info.localPosition;
                    inst.transform.localScale = info.localScale;

                    var lane = inst.GetComponent<Lane>();
                    lane.key = info.key;
                    if (info.hasBgColor && lane.background != null)
                        lane.background.color = info.bgColor;
                    pf.lanes.Add(lane);
                }

                PrefabUtility.SaveAsPrefabAsset(root, pfPath);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Rhythm",
                "完成：\n" + pfPath + " 的轨道已变成 " + lanePath + " 的嵌套实例。\n" +
                "现在编辑 Lane.prefab（宽度/精灵等）会让所有轨道一起更新；位置/按键/底色仍是各自的实例覆盖。",
                "好的");
        }

        // ---------------- 单条轨道预制体 ----------------

        private static GameObject BuildLanePrefab(string path, Sprite sprite)
        {
            var root = new GameObject("Lane");
            var lane = root.AddComponent<Lane>();
            lane.flashAlpha = 0.18f;

            var bg = MakeChildSprite(root.transform, "Background", sprite, new Color(0.13f, 0.13f, 0.19f), 0,
                new Vector3(LaneWidth * 0.96f, FieldHeight, 1f));
            var flash = MakeChildSprite(root.transform, "Flash", sprite, new Color(1f, 1f, 1f, 0f), 1,
                new Vector3(LaneWidth * 0.96f, FieldHeight, 1f));

            lane.background = bg;
            lane.flash = flash;

            return SavePrefab(root, path);
        }

        // ---------------- Playfield 预制体（轨道为 Lane.prefab 的嵌套实例） ----------------

        private static void BuildPlayfieldPrefab(string path, Sprite sprite, GameObject lanePrefab)
        {
            const int count = 4;
            KeyCode[] keys = { KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K };
            float totalWidth = count * LaneWidth;
            float leftX = -totalWidth * 0.5f + LaneWidth * 0.5f;

            var root = new GameObject("Playfield");
            var pf = root.AddComponent<Playfield>();
            pf.spawnTopY = TopY;
            pf.laneSpacing = LaneWidth;

            for (int i = 0; i < count; i++)
            {
                // 以"嵌套预制体实例"的形式放入，每条轨道仍连接到 Lane.prefab
                var laneInstance = (GameObject)PrefabUtility.InstantiatePrefab(lanePrefab);
                laneInstance.name = $"Lane{i}";
                laneInstance.transform.SetParent(root.transform);
                laneInstance.transform.position = new Vector3(leftX + i * LaneWidth, FieldCenterY, 0f);

                var lane = laneInstance.GetComponent<Lane>();
                lane.key = keys[i]; // 每条轨道的按键作为实例覆盖
                pf.lanes.Add(lane);
            }

            var line = MakeChildSprite(root.transform, "JudgeLine", sprite, new Color(0.9f, 0.9f, 1f), 5,
                new Vector3(totalWidth, 0.12f, 1f));
            line.transform.position = new Vector3(0f, JudgeY, 0f);
            pf.judgeLine = line.transform;

            var spawnTop = new GameObject("SpawnTop");
            spawnTop.transform.SetParent(root.transform);
            spawnTop.transform.position = new Vector3(0f, TopY, 0f);
            pf.spawnTop = spawnTop.transform;

            SavePrefab(root, path);
        }

        private static SpriteRenderer MakeChildSprite(Transform parent, string name, Sprite sprite, Color color, int order, Vector3 scale)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = scale;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.color = color;
            sr.sortingOrder = order;
            return sr;
        }

        // ---------------- 单精灵预制体（音符 / 长按条） ----------------

        private static GameObject BuildSpritePrefab(string path, Sprite sprite, Color color, int order, Vector3 scale)
        {
            var go = new GameObject(Path.GetFileNameWithoutExtension(path));
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.color = color;
            sr.sortingOrder = order;
            go.transform.localScale = scale;
            return SavePrefab(go, path);
        }

        private static void AssignNotePrefabsToManager(GameObject note, GameObject hold)
        {
            RhythmGameManager mgr = null;
            if (Selection.activeGameObject != null)
                mgr = Selection.activeGameObject.GetComponent<RhythmGameManager>();
            if (mgr == null)
                mgr = Object.FindObjectOfType<RhythmGameManager>();
            if (mgr == null) return;

            Undo.RecordObject(mgr, "Assign Rhythm Note Prefabs");
            mgr.notePrefab = note != null ? note.GetComponent<SpriteRenderer>() : null;
            mgr.holdBodyPrefab = hold != null ? hold.GetComponent<SpriteRenderer>() : null;
            EditorUtility.SetDirty(mgr);
        }

        private static GameObject SavePrefab(GameObject go, string path)
        {
            var prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
            return prefab;
        }

        // ---------------- 白色精灵资源 ----------------

        private static Sprite CreateWhiteSprite(string path)
        {
            var tex = new Texture2D(4, 4, TextureFormat.RGBA32, false);
            var pixels = new Color32[16];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = new Color32(255, 255, 255, 255);
            tex.SetPixels32(pixels);
            tex.Apply();
            File.WriteAllBytes(path, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);

            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = 4f; // 4px / 4ppu = 1 世界单位，和运行时一致
            importer.filterMode = FilterMode.Point;
            importer.mipmapEnabled = false;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();

            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        // ---------------- 工具 ----------------

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string parent = Path.GetDirectoryName(path).Replace("\\", "/");
            string leaf = Path.GetFileName(path);
            if (!AssetDatabase.IsValidFolder(parent)) EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, leaf);
        }
    }
}
