#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

/// <summary>
/// Pre-build hook for AnimatedSprite keyword generation.
/// The build keyword mode is selected in AnimSpriteMasterSetup / AnimSpriteProjectSettings:
/// - MultiCompile: previous safe build default; emits multi_compile_local group variants.
/// - ForceAllGroupsNoKeywords: emits an uber shader keyword file where all _FX_* groups are #defined.
/// - KeepEditorMode: does not override the editor keyword mode for builds.
/// - ShaderFeature: emits shader_feature_local group variants for scene-used FlexiSprite combinations in enabled Build Settings scenes.
/// Post-build restores the editor keyword mode when it was temporarily overridden.
/// </summary>
namespace FlexiSprite
{
    public class AnimSpriteBuildKeywordHook : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        public int callbackOrder => -50;

        static EditorKeywordMode _savedMode;
        static bool _needsRestore;

        static EditorKeywordMode ResolveBuildMode(AnimSpriteProjectSettings settings, EditorKeywordMode editorMode)
        {
            if (settings == null) return EditorKeywordMode.MultiCompile;

            return settings.buildKeywordMode switch
            {
                BuildKeywordMode.ForceAllGroupsNoKeywords => EditorKeywordMode.NoKeywords,
                BuildKeywordMode.KeepEditorMode => editorMode,
                BuildKeywordMode.ShaderFeature => EditorKeywordMode.ShaderFeature,
                _ => EditorKeywordMode.MultiCompile
            };
        }

        static string DescribeMode(EditorKeywordMode mode)
        {
            return mode switch
            {
                EditorKeywordMode.NoKeywords => "ForceAllGroupsNoKeywords / all _FX_* groups #defined",
                EditorKeywordMode.ShaderFeature => "ShaderFeature / shader_feature_local group variants",
                EditorKeywordMode.MultiCompile => "MultiCompile / multi_compile_local group variants",
                _ => mode.ToString()
            };
        }

        static RenderPipelineTarget SyncSettingsToActiveProjectPipeline(AnimSpriteProjectSettings settings)
        {
            var projectPipeline = AnimSpriteProjectSettings.DetectPipeline();

            if (settings.Pipeline == projectPipeline)
                return projectPipeline;

            Debug.LogWarning(
                $"[AnimSprite Build] FlexiSprite shader target was {settings.Pipeline}, but the active Unity project pipeline is {projectPipeline}. " +
                $"Syncing shader generation to {projectPipeline} before build.");

            settings.Pipeline = projectPipeline;
            EditorUtility.SetDirty(settings);
            return projectPipeline;
        }

        static void RegenerateAllKeywordDependentFiles(EditorKeywordMode mode, RenderPipelineTarget pipeline)
        {
            var settings = AnimSpriteProjectSettings.Instance;
            if (settings == null) return;

            if (settings.Pipeline != pipeline)
                settings.Pipeline = pipeline;

            settings.editorKeywordMode = mode;
            settings.ApplyOrderToRegistry();
            EditorUtility.SetDirty(settings);

            AnimatedSpriteHLSLGenerator.GenerateKeywordsHLSL();
            AnimatedSpriteHLSLGenerator.GenerateEffectsHLSL();
            AnimatedSpriteShaderFileGenerator.GenerateAll(pipeline);

            if (mode == EditorKeywordMode.ShaderFeature)
                AnimSpriteShaderFeatureVariantKeeper.RefreshForCurrentProject(true);
            else
                AnimSpriteShaderFeatureVariantKeeper.ClearGeneratedAssets();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public void OnPreprocessBuild(BuildReport report)
        {
            var settings = AnimSpriteProjectSettings.Instance;
            if (settings == null) return;

            settings.EnsureBuildKeywordModeDefault();
            var buildPipeline = SyncSettingsToActiveProjectPipeline(settings);

            _savedMode = settings.editorKeywordMode;
            var buildMode = ResolveBuildMode(settings, _savedMode);

            _needsRestore = settings.buildKeywordMode != BuildKeywordMode.KeepEditorMode && buildMode != _savedMode;

            if (settings.buildKeywordMode == BuildKeywordMode.KeepEditorMode)
            {
                Debug.Log($"[AnimSprite Build] Build keyword override is KeepEditorMode. Regenerating {buildPipeline} shader files with editor mode: {DescribeMode(_savedMode)}.");
                RegenerateAllKeywordDependentFiles(_savedMode, buildPipeline);
                return;
            }

            Debug.Log($"[AnimSprite Build] Build keyword override is {settings.buildKeywordMode}. Regenerating {buildPipeline} shader files as {DescribeMode(buildMode)} for this build...");
            RegenerateAllKeywordDependentFiles(buildMode, buildPipeline);
        }

        public void OnPostprocessBuild(BuildReport report)
        {
            if (!_needsRestore) return;

            var settings = AnimSpriteProjectSettings.Instance;
            var restorePipeline = settings != null ? settings.Pipeline : AnimSpriteProjectSettings.DetectPipeline();

            Debug.Log($"[AnimSprite Build] Restoring editor keyword mode to {_savedMode} for {restorePipeline}...");
            RegenerateAllKeywordDependentFiles(_savedMode, restorePipeline);
            _needsRestore = false;
            Debug.Log($"[AnimSprite Build] Editor keyword mode restored to {_savedMode}.");
        }

        [InitializeOnLoadMethod]
        static void DomainReloadSafety()
        {
            if (!_needsRestore) return;

            var settings = AnimSpriteProjectSettings.Instance;
            var restorePipeline = settings != null ? settings.Pipeline : AnimSpriteProjectSettings.DetectPipeline();

            Debug.LogWarning("[AnimSprite Build] Detected incomplete build — restoring editor keyword mode.");
            RegenerateAllKeywordDependentFiles(_savedMode, restorePipeline);
            _needsRestore = false;
        }
    }
}
#endif
