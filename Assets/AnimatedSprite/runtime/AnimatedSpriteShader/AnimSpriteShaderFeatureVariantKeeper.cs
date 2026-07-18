#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FlexiSprite
{
    /// <summary>
    /// Keeps shader_feature_local group variants alive for FlexiSprite.
    ///
    /// shader_feature variants are only kept by Unity when a persistent asset references the exact
    /// keyword combination. FlexiSprite normally creates/generated materials dynamically and pushes
    /// values with MPBs, so Unity can strip combinations that are only used by scene components.
    /// This helper generates tiny Resources materials only for the shader variant + _FX_* keyword
    /// masks used by AnimatedSpriteShader components in enabled Build Settings scenes.
    /// </summary>
    public static class AnimSpriteShaderFeatureVariantKeeper
    {
        const string ResourcesRoot = "Assets/AnimatedSprite/Resources";
        const string KeepAliveFolder = ResourcesRoot + "/FlexiSpriteShaderFeatureKeepAlive";
        const string SessionRefreshKey = "FlexiSprite.ShaderFeatureKeepAlive.RefreshedThisReload";

        static bool _queued;

        struct ShaderVariantMask : IEquatable<ShaderVariantMask>
        {
            public readonly AnimatedSpriteShader.ShaderVariant variant;
            public readonly int mask;

            public ShaderVariantMask(AnimatedSpriteShader.ShaderVariant variant, int mask)
            {
                this.variant = variant;
                this.mask = mask;
            }

            public bool Equals(ShaderVariantMask other) => variant == other.variant && mask == other.mask;
            public override bool Equals(object obj) => obj is ShaderVariantMask other && Equals(other);
            public override int GetHashCode() => ((int)variant * 397) ^ mask;
        }

        [InitializeOnLoadMethod]
        static void RefreshAfterDomainReload()
        {
            EditorApplication.delayCall += () =>
            {
                if (SessionState.GetBool(SessionRefreshKey, false))
                    return;

                var settings = AnimSpriteProjectSettings.Instance;
                if (settings == null || settings.editorKeywordMode != EditorKeywordMode.ShaderFeature)
                    return;

                SessionState.SetBool(SessionRefreshKey, true);
                RegenerateShaderFeatureFilesAndKeepAlive(settings);
            };
        }

        public static void RegenerateShaderFeatureFilesAndKeepAlive(AnimSpriteProjectSettings settings = null)
        {
            settings = settings != null ? settings : AnimSpriteProjectSettings.Instance;
            if (settings == null)
                return;

            settings.EnsureGroups();
            settings.ApplyOrderToRegistry();
            EditorUtility.SetDirty(settings);

            AnimatedSpriteHLSLGenerator.RegenerateWithMode(EditorKeywordMode.ShaderFeature);
            AnimatedSpriteShaderFileGenerator.GenerateAll(settings.Pipeline);
            RefreshForCurrentProject(true);
        }

        public static void QueueRefreshIfShaderFeature()
        {
            var settings = AnimSpriteProjectSettings.Instance;
            if (settings == null || settings.editorKeywordMode != EditorKeywordMode.ShaderFeature)
                return;

            if (_queued)
                return;

            _queued = true;
            EditorApplication.delayCall += () =>
            {
                _queued = false;
                if (EditorApplication.isPlayingOrWillChangePlaymode)
                    return;

                var s = AnimSpriteProjectSettings.Instance;
                if (s == null || s.editorKeywordMode != EditorKeywordMode.ShaderFeature)
                    return;

                // Property edits usually happen in already-loaded scenes. Do not open every Build
                // Settings scene on every inspector change; keep this lightweight and scene-local.
                RefreshForLoadedBuildScenesOnly();
            };
        }

        /// <summary>
        /// Generates keep-alive materials for AnimatedSpriteShader components found in enabled
        /// Build Settings scenes. This is the path used by Master Setup and the pre-build hook.
        /// </summary>
        public static void RefreshForCurrentProject(bool scanEnabledBuildScenes = true)
        {
            RefreshInternal(scanEnabledBuildScenes);
        }

        /// <summary>
        /// Lightweight editor refresh for currently loaded scenes only. If Build Settings contains
        /// enabled scenes, only loaded scenes that are included there are scanned.
        /// </summary>
        public static void RefreshForLoadedBuildScenesOnly()
        {
            RefreshInternal(false);
        }

        static void RefreshInternal(bool scanEnabledBuildScenes)
        {
            var settings = AnimSpriteProjectSettings.Instance;
            if (settings == null)
                return;

            settings.EnsureGroups();

            var used = CollectSceneVariantMasks(scanEnabledBuildScenes);
            if (settings.excludedVariants != null && settings.excludedVariants.Count > 0)
                used.RemoveWhere(k => settings.excludedVariants.Contains(k.variant));

            if (used.Count == 0)
            {
                ClearGeneratedAssets();
                Debug.LogWarning("[AnimSprite ShaderFeature] No AnimatedSpriteShader scene usage found. " +
                                 "No shader_feature keep-alive materials were generated. Add scenes to Build Settings or open a scene that uses FlexiSprite.");
                return;
            }

            EnsureFolder(ResourcesRoot);
            if (AssetDatabase.IsValidFolder(KeepAliveFolder))
                AssetDatabase.DeleteAsset(KeepAliveFolder);
            EnsureFolder(KeepAliveFolder);

            int created = 0;

            try
            {
                AssetDatabase.StartAssetEditing();

                foreach (var key in used.OrderBy(k => (int)k.variant).ThenBy(k => k.mask))
                {
                    int vi = (int)key.variant;
                    if (vi < 0 || vi >= AnimatedSpriteShader.ShaderPaths.Length)
                        continue;

                    Shader shader = Shader.Find(AnimatedSpriteShader.ShaderPaths[vi]);
                    if (!shader)
                        continue;

                    var mat = new Material(shader)
                    {
                        name = $"AIO_Keep_{key.variant}_{key.mask:X2}",
                        hideFlags = HideFlags.NotEditable
                    };

                    ApplyGroupMask(mat, key.mask);

                    string safeVariant = key.variant.ToString().Replace('/', '_').Replace(' ', '_');
                    string path = $"{KeepAliveFolder}/{safeVariant}_{key.mask:X2}.mat";
                    AssetDatabase.CreateAsset(mat, path);
                    created++;
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            foreach (var variant in used.Select(k => k.variant).Distinct())
            {
                int vi = (int)variant;
                if (vi < 0 || vi >= AnimatedSpriteShader.ShaderPaths.Length)
                    continue;

                Shader shader = Shader.Find(AnimatedSpriteShader.ShaderPaths[vi]);
                if (!shader)
                    continue;

                string shaderPath = AssetDatabase.GetAssetPath(shader);
                if (!string.IsNullOrEmpty(shaderPath))
                    AssetDatabase.ImportAsset(shaderPath, ImportAssetOptions.ForceUpdate);
            }

            Debug.Log($"[AnimSprite ShaderFeature] Generated {created} scene-used keep-alive material asset(s) " +
                      $"from {used.Count} shader variant/keyword mask combination(s). Only enabled Build Settings scenes are scanned during full refresh.");
        }

        public static void ClearGeneratedAssets()
        {
            if (!AssetDatabase.IsValidFolder(KeepAliveFolder))
                return;

            AssetDatabase.DeleteAsset(KeepAliveFolder);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[AnimSprite ShaderFeature] Removed generated shader_feature keep-alive materials.");
        }

        static HashSet<ShaderVariantMask> CollectSceneVariantMasks(bool scanEnabledBuildScenes)
        {
            var result = new HashSet<ShaderVariantMask>();
            var sharedUnionByVariant = new Dictionary<AnimatedSpriteShader.ShaderVariant, int>();
            var buildScenePaths = GetEnabledBuildScenePaths();

            if (scanEnabledBuildScenes && buildScenePaths.Count > 0)
            {
                var setup = EditorSceneManager.GetSceneManagerSetup();
                string activeScenePath = SceneManager.GetActiveScene().path;

                try
                {
                    foreach (string path in buildScenePaths)
                    {
                        Scene scene = GetLoadedSceneByPath(path);
                        if (!scene.IsValid() || !scene.isLoaded)
                            scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);

                        CollectFromScene(scene, result, sharedUnionByVariant);
                    }
                }
                finally
                {
                    EditorSceneManager.RestoreSceneManagerSetup(setup);

                    if (!string.IsNullOrEmpty(activeScenePath))
                    {
                        Scene restoredActive = SceneManager.GetSceneByPath(activeScenePath);
                        if (restoredActive.IsValid() && restoredActive.isLoaded)
                            SceneManager.SetActiveScene(restoredActive);
                    }
                }
            }
            else
            {
                bool restrictToBuildScenes = buildScenePaths.Count > 0;

                if (!restrictToBuildScenes)
                {
                    Debug.LogWarning("[AnimSprite ShaderFeature] No enabled scenes found in Build Settings. " +
                                     "Falling back to currently loaded scenes only.");
                }

                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    Scene scene = SceneManager.GetSceneAt(i);
                    if (!scene.IsValid() || !scene.isLoaded)
                        continue;

                    if (restrictToBuildScenes && !buildScenePaths.Contains(scene.path))
                        continue;

                    CollectFromScene(scene, result, sharedUnionByVariant);
                }
            }

            // Shared material mode combines all active group keywords per shader variant at runtime.
            // Add the union mask too, otherwise two shared sprites using Color and Edge separately can
            // require the Color|Edge shader_feature variant even though no single component has both.
            foreach (var pair in sharedUnionByVariant)
                result.Add(new ShaderVariantMask(pair.Key, pair.Value));

            return result;
        }

        static HashSet<string> GetEnabledBuildScenePaths()
        {
            var paths = new HashSet<string>();
            foreach (var scene in EditorBuildSettings.scenes)
            {
                if (scene == null || !scene.enabled || string.IsNullOrEmpty(scene.path))
                    continue;

                paths.Add(scene.path);
            }
            return paths;
        }

        static Scene GetLoadedSceneByPath(string path)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.IsValid() && scene.path == path)
                    return scene;
            }
            return default;
        }

        static void CollectFromScene(
            Scene scene,
            HashSet<ShaderVariantMask> result,
            Dictionary<AnimatedSpriteShader.ShaderVariant, int> sharedUnionByVariant)
        {
            if (!scene.IsValid() || !scene.isLoaded)
                return;

            foreach (var root in scene.GetRootGameObjects())
            {
                if (!root)
                    continue;

                var components = root.GetComponentsInChildren<AnimatedSpriteShader>(true);
                foreach (var inst in components)
                {
                    if (!inst)
                        continue;

                    if ((inst.hideFlags & HideFlags.HideAndDontSave) != 0)
                        continue;

                    var variant = inst.CurrentVariant;
                    int mask = ComputeSceneGroupMask(inst);
                    result.Add(new ShaderVariantMask(variant, mask));

                    if (inst.CurrentMaterialMode == AnimatedSpriteShader.MaterialMode.Shared)
                    {
                        if (!sharedUnionByVariant.TryGetValue(variant, out int unionMask))
                            unionMask = 0;

                        sharedUnionByVariant[variant] = unionMask | mask;
                    }
                }
            }
        }

        static int ComputeSceneGroupMask(AnimatedSpriteShader inst)
        {
            int mask = 0;
            int groupCount = Mathf.Clamp((int)EffectGroup._Count, 0, 30);
            for (int g = 0; g < groupCount; g++)
            {
                if (inst.IsGroupActive((EffectGroup)g))
                    mask |= 1 << g;
            }
            return mask;
        }

        static void ApplyGroupMask(Material mat, int mask)
        {
            int groupCount = Mathf.Clamp((int)EffectGroup._Count, 0, 30);
            string[] groupKeywords = EffectGroupInfo.Keywords;
            for (int g = 0; g < groupCount && g < groupKeywords.Length; g++)
                SetMaterialKeyword(mat, groupKeywords[g], (mask & (1 << g)) != 0);
        }

        static void SetMaterialKeyword(Material mat, string keyword, bool enabled)
        {
            if (!mat || string.IsNullOrEmpty(keyword)) return;

#if UNITY_2021_2_OR_NEWER
            if (mat.shader)
            {
                var localKeyword = new UnityEngine.Rendering.LocalKeyword(mat.shader, keyword);
                if (localKeyword.isValid)
                {
                    if (enabled) mat.EnableKeyword(localKeyword);
                    else mat.DisableKeyword(localKeyword);
                    return;
                }
            }
#endif
            if (enabled) mat.EnableKeyword(keyword);
            else mat.DisableKeyword(keyword);
        }

        static void EnsureFolder(string folder)
        {
            if (AssetDatabase.IsValidFolder(folder))
                return;

            string parent = Path.GetDirectoryName(folder).Replace('\\', '/');
            string name = Path.GetFileName(folder);
            if (!string.IsNullOrEmpty(parent))
                EnsureFolder(parent);

            if (!AssetDatabase.IsValidFolder(folder))
                AssetDatabase.CreateFolder(parent, name);
        }
    }
}
#endif
