using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DeusEx.Rhythm;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DeusEx.Rhythm.EditorTools
{
    /// <summary>把 Resources/Boss 下的序列帧转换为可编辑的动画、状态机、预制体和场景实例。</summary>
    internal static class BossAnimationBuilder
    {
        private const string SourceRoot = "Assets/Resources/Boss";
        private const string OutputRoot = "Assets/Generated/Boss";
        private const string ScenePath = "Assets/Scenes/SampleScene.unity";
        private const float FramesPerSecond = 24f;
        private const float PixelsPerUnit = 640f;
        private const int MaxTextureSize = 1024;

        [MenuItem("Tools/DeusEx/Build Boss Animations", false, 30)]
        private static void BuildBossFromMenu()
        {
            BuildBoss();
        }

        /// <summary>供命令行 -executeMethod 调用。</summary>
        public static void BuildBoss()
        {
            if (!AssetDatabase.IsValidFolder(SourceRoot))
                throw new DirectoryNotFoundException($"找不到 Boss 序列帧目录：{SourceRoot}");

            EnsureFolder("Assets/Generated");
            EnsureFolder(OutputRoot);
            ConfigureSourceTextures();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            AnimationClip idle = CreateClip("Idle", "idle", loop: true);
            AnimationClip attack = CreateClip("Attack", "attack", loop: false);
            AnimationClip hit = CreateClip("Hit", "hit", loop: false);
            AnimatorController controller = CreateController(idle, attack, hit);
            GameObject prefab = CreatePrefab(controller, idle);
            PutBossInSampleScene(prefab);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[Boss] 动画、状态机、预制体和场景实例已生成。", prefab);
        }

        private static void ConfigureSourceTextures()
        {
            foreach (string path in Directory.GetFiles(SourceRoot, "*.png", SearchOption.AllDirectories))
            {
                string assetPath = path.Replace('\\', '/');
                var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                if (importer == null) continue;

                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.spritePixelsPerUnit = PixelsPerUnit;
                importer.mipmapEnabled = false;
                importer.alphaIsTransparency = true;
                importer.maxTextureSize = MaxTextureSize;
                importer.textureCompression = TextureImporterCompression.Compressed;
                importer.crunchedCompression = true;
                importer.compressionQuality = 75;
                importer.SaveAndReimport();
            }
        }

        private static AnimationClip CreateClip(string clipName, string sourceFolder, bool loop)
        {
            string clipPath = $"{OutputRoot}/{clipName}.anim";
            AssetDatabase.DeleteAsset(clipPath);

            Sprite[] frames = LoadFrames(sourceFolder);
            if (frames.Length == 0)
                throw new InvalidOperationException($"{sourceFolder} 中没有可用 PNG 序列帧。");

            var clip = new AnimationClip { name = clipName, frameRate = FramesPerSecond };
            var binding = EditorCurveBinding.PPtrCurve(string.Empty, typeof(SpriteRenderer), "m_Sprite");
            var keyframes = frames.Select((sprite, index) => new ObjectReferenceKeyframe
            {
                time = index / FramesPerSecond,
                value = sprite,
            }).ToArray();
            AnimationUtility.SetObjectReferenceCurve(clip, binding, keyframes);

            var settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = loop;
            settings.loopBlend = loop;
            settings.stopTime = frames.Length / FramesPerSecond;
            AnimationUtility.SetAnimationClipSettings(clip, settings);
            AssetDatabase.CreateAsset(clip, clipPath);
            return clip;
        }

        private static Sprite[] LoadFrames(string sourceFolder)
        {
            string folder = $"{SourceRoot}/{sourceFolder}";
            return Directory.GetFiles(folder, "*.png", SearchOption.TopDirectoryOnly)
                .OrderBy(ExtractFrameNumber)
                .ThenBy(path => path, StringComparer.Ordinal)
                .Select(path => AssetDatabase.LoadAssetAtPath<Sprite>(path.Replace('\\', '/')))
                .Where(sprite => sprite != null)
                .ToArray();
        }

        private static int ExtractFrameNumber(string path)
        {
            string file = Path.GetFileNameWithoutExtension(path);
            MatchCollection matches = Regex.Matches(file, @"\d+");
            return matches.Count > 0 && int.TryParse(matches[matches.Count - 1].Value, out int number)
                ? number
                : int.MaxValue;
        }

        private static AnimatorController CreateController(AnimationClip idle, AnimationClip attack, AnimationClip hit)
        {
            string path = $"{OutputRoot}/Boss.controller";
            AssetDatabase.DeleteAsset(path);
            var controller = AnimatorController.CreateAnimatorControllerAtPath(path);
            controller.AddParameter("Attack", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Hit", AnimatorControllerParameterType.Trigger);

            AnimatorStateMachine stateMachine = controller.layers[0].stateMachine;
            AnimatorState idleState = stateMachine.AddState("Idle");
            AnimatorState attackState = stateMachine.AddState("Attack");
            AnimatorState hitState = stateMachine.AddState("Hit");
            idleState.motion = idle;
            attackState.motion = attack;
            hitState.motion = hit;
            stateMachine.defaultState = idleState;

            AddTriggerTransition(stateMachine.AddAnyStateTransition(attackState), "Attack");
            AddTriggerTransition(stateMachine.AddAnyStateTransition(hitState), "Hit");
            AddReturnTransition(attackState, idleState);
            AddReturnTransition(hitState, idleState);
            EditorUtility.SetDirty(controller);
            return controller;
        }

        private static void AddTriggerTransition(AnimatorStateTransition transition, string trigger)
        {
            transition.hasExitTime = false;
            transition.hasFixedDuration = true;
            transition.duration = 0.08f;
            transition.canTransitionToSelf = false;
            transition.AddCondition(AnimatorConditionMode.If, 0f, trigger);
        }

        private static void AddReturnTransition(AnimatorState from, AnimatorState to)
        {
            AnimatorStateTransition transition = from.AddTransition(to);
            transition.hasExitTime = true;
            transition.exitTime = 0.98f;
            transition.hasFixedDuration = true;
            transition.duration = 0.1f;
        }

        private static GameObject CreatePrefab(AnimatorController controller, AnimationClip idle)
        {
            string path = $"{OutputRoot}/Boss.prefab";
            var root = new GameObject("Boss", typeof(SpriteRenderer), typeof(Animator), typeof(BossAnimationDriver), typeof(BossBehavior));
            var spriteRenderer = root.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = LoadFrames("idle").FirstOrDefault();
            spriteRenderer.sortingOrder = 10;

            var animator = root.GetComponent<Animator>();
            animator.runtimeAnimatorController = controller;
            var driver = root.GetComponent<BossAnimationDriver>();
            driver.animator = animator;
            driver.playHitWhenEnemyDamaged = true;
            driver.hitCooldown = 0.12f;

            var behavior = root.GetComponent<BossBehavior>();
            behavior.animationDriver = driver;

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
            UnityEngine.Object.DestroyImmediate(root);
            return prefab;
        }

        private static void PutBossInSampleScene(GameObject prefab)
        {
            Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            GameObject existing = GameObject.Find("Boss");
            if (existing != null) UnityEngine.Object.DestroyImmediate(existing);

            var boss = (GameObject)PrefabUtility.InstantiatePrefab(prefab, scene);
            boss.name = "Boss";
            boss.transform.position = new Vector3(0f, 3.35f, 2f);
            boss.transform.localScale = Vector3.one;
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static void EnsureFolder(string folder)
        {
            if (AssetDatabase.IsValidFolder(folder)) return;
            string parent = Path.GetDirectoryName(folder)?.Replace('\\', '/');
            string name = Path.GetFileName(folder);
            if (string.IsNullOrEmpty(parent) || string.IsNullOrEmpty(name))
                throw new InvalidOperationException($"无效目录：{folder}");
            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, name);
        }
    }
}
