#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace FlexiSprite
{
    [InitializeOnLoad]
    public static class AnimSpriteImportPipelinePrompt
    {
        const string PromptVersion = "pipeline-demo-package-auto-import-v3-clean-shaders";
        const string SessionKey = "FlexiSprite.ImportPipelinePrompt.SessionShown";

        static AnimSpriteImportPipelinePrompt()
        {
            EditorApplication.delayCall += TryShowAfterImport;
        }

        [MenuItem("Tools/AnimatedSprite/First Import Setup", priority = -10)]
        public static void ShowFirstImportSetup()
        {
            ShowPrompt(false);
        }

        [MenuItem("Tools/AnimatedSprite/Reset First Import Setup Prompt", priority = 999)]
        public static void ResetPrompt()
        {
            EditorPrefs.DeleteKey(ProjectPromptKey);
            SessionState.SetBool(SessionKey, false);
            Debug.Log("[FlexiSprite] First import setup prompt was reset for this project.");
        }

        [MenuItem("Tools/AnimatedSprite/Import Demo Packages For Current Pipeline", priority = -9)]
        public static void ImportDemoPackagesForCurrentPipeline()
        {
            var result = AnimSpriteDemoPackageImporter.ImportForCurrentPipeline(true);

            EditorUtility.DisplayDialog(
                "FlexiSprite Demo Packages",
                result.Message,
                "OK");
        }

        [MenuItem("Tools/AnimatedSprite/Cleanup Generated Shaders For Current Pipeline", priority = -8)]
        public static void CleanupGeneratedShadersForCurrentPipeline()
        {
            var projectPipeline = AnimSpriteProjectSettings.DetectPipeline();
            int removed = AnimatedSpriteShaderFileGenerator.DeleteShadersNotForPipeline(projectPipeline);

            EditorUtility.DisplayDialog(
                "FlexiSprite Shader Cleanup",
                removed > 0
                    ? $"Removed {removed} shader files from other pipelines. Run Master Setup → Full Rebuild for {projectPipeline} next."
                    : $"No shader files from other pipelines were found. Current project pipeline: {projectPipeline}.",
                "OK");
        }

        static void TryShowAfterImport()
        {
            if (Application.isBatchMode)
                return;

            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += TryShowAfterImport;
                return;
            }

            if (SessionState.GetBool(SessionKey, false))
                return;

            if (EditorPrefs.GetString(ProjectPromptKey, string.Empty) == PromptVersion)
                return;

            var projectPipeline = AnimSpriteProjectSettings.DetectPipeline();
            int removedShaders = AnimatedSpriteShaderFileGenerator.DeleteShadersNotForPipeline(projectPipeline);
            if (removedShaders > 0)
            {
                Debug.Log($"[FlexiSprite] Removed {removedShaders} shader files from other pipelines before setup. Run the setup prompt or Master Setup to regenerate for the current pipeline.");
            }

            SessionState.SetBool(SessionKey, true);
            EditorPrefs.SetString(ProjectPromptKey, PromptVersion);
            ShowPrompt(true);
        }

        static void ShowPrompt(bool automatic)
        {
            var projectPipeline = AnimSpriteProjectSettings.DetectPipeline();
            var settings = AnimSpriteProjectSettings.Instance;
            var shaderBuild = settings != null ? settings.Pipeline : projectPipeline;

            string title = automatic ? "FlexiSprite Imported" : "FlexiSprite First Import Setup";
            string message =
                $"Unity project pipeline: {projectPipeline}\n" +
                $"FlexiSprite shader build target: {shaderBuild}\n\n" +
                "Do you want to regenerate FlexiSprite shader/generated files for this project's current pipeline now?\n\n" +
                "This rebuild updates generated shader files, pipeline defines, and generated C# bindings for the active project pipeline. " +
                "After the rebuild, FlexiSprite will automatically import matching demo scene .unitypackage installers found under your Scenes folder.\n\n" +
                "Pipeline-specific demo scenes are stored as optional .unitypackage installers, usually in Assets/AnimatedSprite/Scenes/. " +
                "Keep only pipeline-specific content inside those packages, then let this setup import the package matching the current project pipeline:\n\n" +
                "• Built-In Lit\n" +
                "• URP All\n" +
                "• URP 2D Lighting\n" +
                "• HDRP All";

            int choice = EditorUtility.DisplayDialogComplex(
                title,
                message,
                "Regenerate + Demos",
                "Open Master Setup",
                "Later");

            if (choice == 0)
            {
                RebuildNow(projectPipeline);
                return;
            }

            if (choice == 1)
            {
                AnimSpriteMasterSetup.ShowWindow();
            }
        }

        static void RebuildNow(RenderPipelineTarget projectPipeline)
        {
            try
            {
                var result = AnimSpriteMasterSetup.RebuildShadersForPipeline(projectPipeline);
                var demoImport = AnimSpriteDemoPackageImporter.ImportForPipeline(projectPipeline, false);

                string message = result.Message + "\n\n" + demoImport.Message;

                EditorUtility.DisplayDialog(
                    result.Success ? "FlexiSprite Setup Complete" : "FlexiSprite Setup Needs Attention",
                    message,
                    "OK");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                EditorUtility.DisplayDialog(
                    "FlexiSprite Setup Failed",
                    "Automatic regeneration failed. Open Tools > AnimatedSprite > Master Setup and run Full Rebuild manually.\n\n" + e.Message,
                    "OK");
            }
        }

        static string ProjectPromptKey =>
            "FlexiSprite.ImportPipelinePrompt." + StableProjectHash(Application.dataPath) + "." + PromptVersion;

        static int StableProjectHash(string value)
        {
            unchecked
            {
                int hash = 23;
                for (int i = 0; i < value.Length; i++)
                    hash = hash * 31 + value[i];
                return hash;
            }
        }
    }
}
#endif
