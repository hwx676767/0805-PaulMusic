#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using VariantMap = System.Collections.Generic.Dictionary<FlexiSprite.AnimatedSpriteShader.ShaderVariant, FlexiSprite.AnimatedSpriteShader.ShaderVariant>;
using UnityEditor.Build;
using UnityEditor.Rendering;


namespace FlexiSprite
{
    public class AnimSpriteMasterSetup : EditorWindow
    {
        AnimSpriteProjectSettings _s;
        Vector2 _scrollPos;
        string _statusMsg;
        MessageType _statusType;
        double _statusTime;

        // Colors
        static readonly Color HeaderBg = new Color(0.08f, 0.08f, 0.12f);
        static readonly Color GroupBg = new Color(0.17f, 0.17f, 0.21f);
        static readonly Color GroupBgAlt = new Color(0.15f, 0.15f, 0.19f);
        static readonly Color AccentBlue = new Color(0.35f, 0.65f, 1f);
        static readonly Color DimText = new Color(0.5f, 0.5f, 0.55f);

        [MenuItem("Tools/AnimatedSprite/Master Setup", priority = 0)]
        public static void ShowWindow()
        {
            var w = GetWindow<AnimSpriteMasterSetup>("AnimSprite Master");
            w.minSize = new Vector2(500, 600);
        }

        public readonly struct ShaderRebuildResult
        {
            public readonly bool Success;
            public readonly string Message;
            public readonly MessageType Type;

            public ShaderRebuildResult(bool success, string message, MessageType type)
            {
                Success = success;
                Message = message;
                Type = type;
            }
        }

        public static ShaderRebuildResult RebuildShadersForPipeline(RenderPipelineTarget target)
        {
            var settings = AnimSpriteProjectSettings.Instance;

            if (!settings)
            {
                return new ShaderRebuildResult(
                    false,
                    "FlexiSprite project settings could not be loaded or created.",
                    MessageType.Error);
            }

            if (settings.Pipeline != target)
            {
                Undo.RecordObject(settings, "Set FlexiSprite Shader Build Target");
                settings.Pipeline = target;
                EditorUtility.SetDirty(settings);
            }

            settings.ApplyOrderToRegistry();
            AnimatedSpriteHLSLGenerator.GenerateEffectsHLSL();
            AnimatedSpriteHLSLGenerator.GenerateKeywordsHLSL();
            AnimatedSpriteShaderFileGenerator.GenerateAll(target);

            var patchResult = PatchShaderFilesForSettings(settings);
            if (!patchResult.Success)
                return patchResult;

            AnimatedSpriteShaderGenerator.Generate();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            string message = $"Full rebuild complete for {settings.Pipeline} ({settings.PipelineDefine}). "
                + $"Patched {patchResult.Message}.";

            Debug.Log($"[FlexiSprite] {message}");
            return new ShaderRebuildResult(true, message, MessageType.Info);
        }

        void OnEnable() => _s = AnimSpriteProjectSettings.Instance;

        void OnGUI()
        {
            if (!_s) { if (GUILayout.Button("Create Project Settings")) _s = AnimSpriteProjectSettings.Instance; return; }

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            DrawHeader();
            EditorGUILayout.Space(6);
            DrawPipelineStateBar();
            EditorGUILayout.Space(6);
            DrawVariantSelectionSection();
            EditorGUILayout.Space(6);
            DrawBuildSection();
            EditorGUILayout.Space(4);
            DrawMigrationSection();
            EditorGUILayout.Space(6);
            DrawGlobalLightingSection();
            EditorGUILayout.Space(6);
            DrawGroupSection();
            EditorGUILayout.Space(4);
            DrawStatus();

            EditorGUILayout.EndScrollView();
        }

        // Header

        void DrawHeader()
        {
            var r = EditorGUILayout.GetControlRect(false, 52);
            EditorGUI.DrawRect(r, HeaderBg);
            var s = new GUIStyle(EditorStyles.boldLabel) { fontSize = 16, alignment = TextAnchor.MiddleCenter };
            s.normal.textColor = AccentBlue;
            EditorGUI.LabelField(r, "AnimatedSprite - Master Setup", s);

            var sub = new Rect(r.x, r.y + 30, r.width, 18);
            var subStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
            int fxCount = EffectRegistry.All?.Length ?? 0;
            EditorGUI.LabelField(sub, $"{fxCount} effects  \u00B7  {(int)EffectGroup._Count} groups  \u00B7  {_s.VariantCount} variants", subStyle);
        }

        void DrawPipelineStateBar()
        {
            var projectPipeline = AnimSpriteProjectSettings.DetectPipeline();
            var shaderBuildTarget = _s.Pipeline;
            bool isSynced = projectPipeline == shaderBuildTarget;

            var bgRect = EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.DrawRect(bgRect, new Color(0.10f, 0.10f, 0.13f));

            EditorGUILayout.LabelField("Project / Shader Build State", EditorStyles.boldLabel);
            DrawPipelineStateRow("Unity project pipeline", projectPipeline.ToString());
            DrawPipelineStateRow("FlexiSprite shader build", $"{shaderBuildTarget} ({_s.PipelineDefine})");
            DrawPipelineStateRow("Migration target", "Pick in Pipeline Migration below");

            EditorGUILayout.Space(2);
            if (isSynced)
            {
                EditorGUILayout.HelpBox(
                    "FlexiSprite shaders are built for the current Unity render pipeline.",
                    MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox(
                    $"Mismatch: Unity is running {projectPipeline}, but FlexiSprite shaders are built for {shaderBuildTarget}. " +
                    "Migration only changes variants/material bindings; it does not make wrong-pipeline shaders render. " +
                    "Set the shader build target and run Full Rebuild before migrating to that pipeline.",
                    MessageType.Warning);
            }

            EditorGUILayout.BeginHorizontal();
            GUI.enabled = !isSynced;
            if (GUILayout.Button("Set Target to Project Pipeline"))
                SetShaderBuildTarget(projectPipeline);
            GUI.enabled = true;

            if (GUILayout.Button("Full Rebuild for Project Pipeline"))
                RunFullRebuildFor(projectPipeline);

            if (GUILayout.Button("Full Rebuild Current Target"))
                RunFullRebuild();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Import Demo Packages for Project Pipeline"))
            {
                var result = AnimSpriteDemoPackageImporter.ImportForPipeline(projectPipeline, true);
                Status(result.Message, result.Imported > 0 ? MessageType.Info : MessageType.Warning);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        static void DrawPipelineStateRow(string label, string value)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, GUILayout.Width(160));
            EditorGUILayout.SelectableLabel(value, EditorStyles.miniLabel, GUILayout.Height(16));
            EditorGUILayout.EndHorizontal();
        }

        void SetShaderBuildTarget(RenderPipelineTarget target)
        {
            if (_s.Pipeline == target)
                return;

            Undo.RecordObject(_s, "Set FlexiSprite Shader Build Target");
            _s.Pipeline = target;
            EditorUtility.SetDirty(_s);
            AssetDatabase.SaveAssets();
            Status($"Shader build target set to {target}. Run Full Rebuild before migrating materials to this pipeline.", MessageType.Warning);
        }

        // Compile Variants

        static bool _variantsFoldout;

        void DrawVariantSelectionSection()
        {
            _variantsFoldout = EditorGUILayout.Foldout(_variantsFoldout, "Shader Variants to Compile", true, EditorStyles.foldoutHeader);
            if (!_variantsFoldout) return;

            var bgRect = EditorGUILayout.BeginVertical();
            EditorGUI.DrawRect(bgRect, new Color(0.12f, 0.12f, 0.15f));
            GUILayout.Space(4);

            var field = _s.GetType().GetField("excludedVariants", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            var excludedList = field.GetValue(_s) as System.Collections.IList;
            if (excludedList == null)
            {
                var listType = typeof(System.Collections.Generic.List<>).MakeGenericType(typeof(AnimatedSpriteShader.ShaderVariant));
                excludedList = (System.Collections.IList)Activator.CreateInstance(listType);
                field.SetValue(_s, excludedList);
            }


            bool autoInclude = EditorPrefs.GetBool("AnimSprite_AutoIncludeBuildVariants", true);
            bool newAutoInclude = EditorGUILayout.ToggleLeft(
                " Auto-include variants used in Build Settings scenes",
                autoInclude,
                EditorStyles.boldLabel);
            if (newAutoInclude != autoInclude) EditorPrefs.SetBool("AnimSprite_AutoIncludeBuildVariants", newAutoInclude);

            EditorGUILayout.HelpBox(
                "Unchecked variants will be completely excluded from the final compiled game build. Active pipeline shaders are highlighted.",
                MessageType.Info);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Compile All", EditorStyles.miniButtonLeft))
            {
                Undo.RecordObject(_s, "Change Compiled Variants");
                excludedList.Clear();
                EditorUtility.SetDirty(_s);
            }

    
            if (GUILayout.Button("Scan Build Scenes", EditorStyles.miniButtonMid))
            {
                Undo.RecordObject(_s, "Auto-Enable Build Variants");
                var scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
                int foundCount = 0;
                foreach (var path in scenes)
                {
                    try
                    {
                        var scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(path, UnityEditor.SceneManagement.OpenSceneMode.Additive);
                        foreach (var root in scene.GetRootGameObjects())
                        {
                            var sprites = root.GetComponentsInChildren<AnimatedSpriteShader>(true);
                            foreach (var sprite in sprites)
                            {
                                if (excludedList.Contains(sprite.CurrentVariant))
                                {
                                    excludedList.Remove(sprite.CurrentVariant);
                                    foundCount++;
                                }
                            }
                        }
                        UnityEditor.SceneManagement.EditorSceneManager.CloseScene(scene, true);
                    }
                    catch { }
                }
                Status($"Scan complete. Enabled {foundCount} missing variants found in build scenes.", MessageType.Info);
                EditorUtility.SetDirty(_s);
            }

            if (GUILayout.Button("Active Pipeline Only", EditorStyles.miniButtonRight))
            {
                Undo.RecordObject(_s, "Change Compiled Variants");
                excludedList.Clear();
                var allVars = (AnimatedSpriteShader.ShaderVariant[])Enum.GetValues(typeof(AnimatedSpriteShader.ShaderVariant));
                foreach (var variant in allVars)
                {
                    string vName = variant.ToString();
                    bool isCurrent = (_s.Pipeline == RenderPipelineTarget.BuiltIn && !vName.Contains("URP") && !vName.Contains("HDRP"))
                        || (_s.Pipeline == RenderPipelineTarget.URP && vName.Contains("URP"))
                        || (_s.Pipeline == RenderPipelineTarget.HDRP && vName.Contains("HDRP"));

                    if (!isCurrent)
                        excludedList.Add(variant);
                }
                EditorUtility.SetDirty(_s);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(4);

            EditorGUI.BeginChangeCheck();

            var allVariants = (AnimatedSpriteShader.ShaderVariant[])Enum.GetValues(typeof(AnimatedSpriteShader.ShaderVariant));


            var sortedVariants = allVariants.OrderByDescending(v =>
            {
                string vName = v.ToString();
                return (_s.Pipeline == RenderPipelineTarget.BuiltIn && !vName.Contains("URP") && !vName.Contains("HDRP"))
                    || (_s.Pipeline == RenderPipelineTarget.URP && vName.Contains("URP"))
                    || (_s.Pipeline == RenderPipelineTarget.HDRP && vName.Contains("HDRP"));
            }).ThenBy(v => v.ToString()).ToList();


            GUIStyle highlightStyle = new GUIStyle(EditorStyles.label);
            highlightStyle.normal.textColor = new Color(0.45f, 0.75f, 0.5f); 
            highlightStyle.hover.textColor = highlightStyle.normal.textColor;
            highlightStyle.onNormal.textColor = highlightStyle.normal.textColor;
            highlightStyle.fontStyle = FontStyle.Bold;

            GUIStyle dimStyle = new GUIStyle(EditorStyles.label);
            dimStyle.normal.textColor = DimText;
            dimStyle.hover.textColor = DimText;
            dimStyle.onNormal.textColor = DimText;

            EditorGUI.indentLevel++;
            foreach (var variant in sortedVariants)
            {
                string vName = variant.ToString();
                bool isCurrentPipeline = (_s.Pipeline == RenderPipelineTarget.BuiltIn && !vName.Contains("URP") && !vName.Contains("HDRP"))
                    || (_s.Pipeline == RenderPipelineTarget.URP && vName.Contains("URP"))
                    || (_s.Pipeline == RenderPipelineTarget.HDRP && vName.Contains("HDRP"));

                bool isExcluded = excludedList.Contains(variant);
                bool compile = !isExcluded;

                GUIStyle activeStyle = isCurrentPipeline ? highlightStyle : dimStyle;

                bool newCompile = EditorGUILayout.ToggleLeft(vName.Replace("_", " "), compile, activeStyle);

                if (newCompile != compile)
                {
                    Undo.RecordObject(_s, "Toggle Variant Compilation");
                    if (newCompile)
                        excludedList.Remove(variant);
                    else
                        excludedList.Add(variant);
                }
            }
            EditorGUI.indentLevel--;

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(_s);
            }

            GUILayout.Space(4);
            EditorGUILayout.EndVertical();
        }

        // Pipeline Migration

        enum MigrationScope { AllScenes, OpenScenes, SelectedFolder }
        static MigrationScope _migrationScope = MigrationScope.OpenScenes;
        static string _migrationFolder = "Assets";
        static bool _migrationFoldout;

        static readonly VariantMap BuiltInToURP = new()
        {
            { AnimatedSpriteShader.ShaderVariant.Standard, AnimatedSpriteShader.ShaderVariant.Standard_URP },
            { AnimatedSpriteShader.ShaderVariant.Additive, AnimatedSpriteShader.ShaderVariant.Additive_URP },
            { AnimatedSpriteShader.ShaderVariant.Multiply, AnimatedSpriteShader.ShaderVariant.Standard_URP },
            { AnimatedSpriteShader.ShaderVariant.GUI, AnimatedSpriteShader.ShaderVariant.GUI_URP },
            { AnimatedSpriteShader.ShaderVariant.GUIAdditive, AnimatedSpriteShader.ShaderVariant.GUIAdditive_URP },
            { AnimatedSpriteShader.ShaderVariant.StandardPremultiply, AnimatedSpriteShader.ShaderVariant.Premultiply_URP },
            { AnimatedSpriteShader.ShaderVariant.AdditiveSoft, AnimatedSpriteShader.ShaderVariant.Additive_URP },
            { AnimatedSpriteShader.ShaderVariant.GUIPremultiply, AnimatedSpriteShader.ShaderVariant.GUI_URP },
            { AnimatedSpriteShader.ShaderVariant.ScreenSpaceOverlay, AnimatedSpriteShader.ShaderVariant.GUI_URP },
            { AnimatedSpriteShader.ShaderVariant.Lit3D_BuiltIn, AnimatedSpriteShader.ShaderVariant.Lit3D_URP },
            { AnimatedSpriteShader.ShaderVariant.Lit3DCutout_BuiltIn, AnimatedSpriteShader.ShaderVariant.Lit3DCutout_URP },
        };

        static readonly VariantMap URPToBuiltIn = new()
        {
            { AnimatedSpriteShader.ShaderVariant.Standard_URP, AnimatedSpriteShader.ShaderVariant.Standard },
            { AnimatedSpriteShader.ShaderVariant.Additive_URP, AnimatedSpriteShader.ShaderVariant.Additive },
            { AnimatedSpriteShader.ShaderVariant.Premultiply_URP, AnimatedSpriteShader.ShaderVariant.StandardPremultiply },
            { AnimatedSpriteShader.ShaderVariant.GUI_URP, AnimatedSpriteShader.ShaderVariant.GUI },
            { AnimatedSpriteShader.ShaderVariant.GUIAdditive_URP, AnimatedSpriteShader.ShaderVariant.GUIAdditive },
            { AnimatedSpriteShader.ShaderVariant.Lit2D_URP, AnimatedSpriteShader.ShaderVariant.Standard },
            { AnimatedSpriteShader.ShaderVariant.Lit2DAdditive_URP, AnimatedSpriteShader.ShaderVariant.Additive },
            { AnimatedSpriteShader.ShaderVariant.Lit3D_URP, AnimatedSpriteShader.ShaderVariant.Lit3D_BuiltIn },
            { AnimatedSpriteShader.ShaderVariant.Lit3DCutout_URP, AnimatedSpriteShader.ShaderVariant.Lit3DCutout_BuiltIn },
            { AnimatedSpriteShader.ShaderVariant.Lit3DAdditive_URP, AnimatedSpriteShader.ShaderVariant.Additive },
        };

        static readonly VariantMap BuiltInToHDRP = new()
        {
            { AnimatedSpriteShader.ShaderVariant.Standard, AnimatedSpriteShader.ShaderVariant.HDRP_Unlit },
            { AnimatedSpriteShader.ShaderVariant.Additive, AnimatedSpriteShader.ShaderVariant.HDRP_Additive },
            { AnimatedSpriteShader.ShaderVariant.Multiply, AnimatedSpriteShader.ShaderVariant.HDRP_Unlit },
            { AnimatedSpriteShader.ShaderVariant.GUI, AnimatedSpriteShader.ShaderVariant.HDRP_Unlit },
            { AnimatedSpriteShader.ShaderVariant.GUIAdditive, AnimatedSpriteShader.ShaderVariant.HDRP_Additive },
            { AnimatedSpriteShader.ShaderVariant.StandardPremultiply, AnimatedSpriteShader.ShaderVariant.HDRP_Unlit },
            { AnimatedSpriteShader.ShaderVariant.AdditiveSoft, AnimatedSpriteShader.ShaderVariant.HDRP_Additive },
            { AnimatedSpriteShader.ShaderVariant.GUIPremultiply, AnimatedSpriteShader.ShaderVariant.HDRP_Unlit },
            { AnimatedSpriteShader.ShaderVariant.ScreenSpaceOverlay, AnimatedSpriteShader.ShaderVariant.HDRP_Unlit },
            { AnimatedSpriteShader.ShaderVariant.Lit3D_BuiltIn, AnimatedSpriteShader.ShaderVariant.HDRP_Lit },
            { AnimatedSpriteShader.ShaderVariant.Lit3DCutout_BuiltIn, AnimatedSpriteShader.ShaderVariant.HDRP_Lit },
        };

        static readonly VariantMap HDRPToBuiltIn = new()
        {
            { AnimatedSpriteShader.ShaderVariant.HDRP_Unlit, AnimatedSpriteShader.ShaderVariant.Standard },
            { AnimatedSpriteShader.ShaderVariant.HDRP_Additive, AnimatedSpriteShader.ShaderVariant.Additive },
            { AnimatedSpriteShader.ShaderVariant.HDRP_Lit, AnimatedSpriteShader.ShaderVariant.Lit3D_BuiltIn },
        };

        static readonly VariantMap URPToHDRP = new()
        {
            { AnimatedSpriteShader.ShaderVariant.Standard_URP, AnimatedSpriteShader.ShaderVariant.HDRP_Unlit },
            { AnimatedSpriteShader.ShaderVariant.Additive_URP, AnimatedSpriteShader.ShaderVariant.HDRP_Additive },
            { AnimatedSpriteShader.ShaderVariant.Premultiply_URP, AnimatedSpriteShader.ShaderVariant.HDRP_Unlit },
            { AnimatedSpriteShader.ShaderVariant.GUI_URP, AnimatedSpriteShader.ShaderVariant.HDRP_Unlit },
            { AnimatedSpriteShader.ShaderVariant.GUIAdditive_URP, AnimatedSpriteShader.ShaderVariant.HDRP_Additive },
            { AnimatedSpriteShader.ShaderVariant.Lit2D_URP, AnimatedSpriteShader.ShaderVariant.HDRP_Lit },
            { AnimatedSpriteShader.ShaderVariant.Lit2DAdditive_URP, AnimatedSpriteShader.ShaderVariant.HDRP_Additive },
            { AnimatedSpriteShader.ShaderVariant.Lit3D_URP, AnimatedSpriteShader.ShaderVariant.HDRP_Lit },
            { AnimatedSpriteShader.ShaderVariant.Lit3DCutout_URP, AnimatedSpriteShader.ShaderVariant.HDRP_Lit },
            { AnimatedSpriteShader.ShaderVariant.Lit3DAdditive_URP, AnimatedSpriteShader.ShaderVariant.HDRP_Additive },
        };

        static readonly VariantMap HDRPToURP = new()
        {
            { AnimatedSpriteShader.ShaderVariant.HDRP_Unlit, AnimatedSpriteShader.ShaderVariant.Standard_URP },
            { AnimatedSpriteShader.ShaderVariant.HDRP_Additive, AnimatedSpriteShader.ShaderVariant.Additive_URP },
            { AnimatedSpriteShader.ShaderVariant.HDRP_Lit, AnimatedSpriteShader.ShaderVariant.Lit3D_URP },
        };

        static VariantMap
        GetMigrationMap(RenderPipelineTarget from, RenderPipelineTarget to)
        {
            if (from == RenderPipelineTarget.BuiltIn && to == RenderPipelineTarget.URP) return BuiltInToURP;
            if (from == RenderPipelineTarget.URP && to == RenderPipelineTarget.BuiltIn) return URPToBuiltIn;
            if (from == RenderPipelineTarget.BuiltIn && to == RenderPipelineTarget.HDRP) return BuiltInToHDRP;
            if (from == RenderPipelineTarget.HDRP && to == RenderPipelineTarget.BuiltIn) return HDRPToBuiltIn;
            if (from == RenderPipelineTarget.URP && to == RenderPipelineTarget.HDRP) return URPToHDRP;
            if (from == RenderPipelineTarget.HDRP && to == RenderPipelineTarget.URP) return HDRPToURP;
            return null;
        }

        static GUIStyle _migWarnStyle;
        static GUIStyle _migOkStyle;
        static GUIStyle _migHeaderStyle;
        static GUIStyle _migFallbackStyle;
        static bool[] _migDetailFoldout = new bool[6];

        void DrawMigrationSection()
        {
            _migrationFoldout = EditorGUILayout.Foldout(_migrationFoldout, "Pipeline Migration", true, EditorStyles.foldoutHeader);
            if (!_migrationFoldout) return;

            if (_migWarnStyle == null)
            {
                _migWarnStyle = new GUIStyle(EditorStyles.miniLabel) { normal = { textColor = new Color(0.85f, 0.6f, 0.25f) } };
                _migOkStyle = new GUIStyle(EditorStyles.miniLabel) { normal = { textColor = new Color(0.4f, 0.7f, 0.4f) } };
                _migHeaderStyle = new GUIStyle(EditorStyles.boldLabel) { fontSize = 11, normal = { textColor = new Color(0.75f, 0.75f, 0.8f) } };
                _migFallbackStyle = new GUIStyle(EditorStyles.miniLabel)
                {
                    normal = { textColor = new Color(0.7f, 0.55f, 0.3f) },
                    wordWrap = true,
                    padding = new RectOffset(20, 4, 0, 2)
                };
            }

            var bgRect = EditorGUILayout.BeginVertical();
            EditorGUI.DrawRect(bgRect, new Color(0.12f, 0.12f, 0.15f));
            GUILayout.Space(4);

            EditorGUI.indentLevel++;
            _migrationScope = (MigrationScope)EditorGUILayout.EnumPopup("Scope", _migrationScope);

            if (_migrationScope == MigrationScope.SelectedFolder)
            {
                EditorGUILayout.BeginHorizontal();
                _migrationFolder = EditorGUILayout.TextField("Folder", _migrationFolder);
                if (GUILayout.Button("...", GUILayout.Width(28)))
                {
                    string f = EditorUtility.OpenFolderPanel("Scene Folder", _migrationFolder, "");
                    if (!string.IsNullOrEmpty(f) && f.StartsWith(Application.dataPath))
                        _migrationFolder = "Assets" + f.Substring(Application.dataPath.Length);
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space(6);

            var projectPipeline = AnimSpriteProjectSettings.DetectPipeline();
            var shaderBuildTarget = _s.Pipeline;

            EditorGUILayout.HelpBox(
                $"Migration changes AnimatedSprite variants and material bindings only. It does not rebuild shader files.\n" +
                $"Unity project pipeline: {projectPipeline}\n" +
                $"FlexiSprite shader build: {shaderBuildTarget} ({_s.PipelineDefine})",
                MessageType.Info);

            if (projectPipeline != shaderBuildTarget)
            {
                EditorGUILayout.HelpBox(
                    $"Pipeline mismatch: Unity is {projectPipeline}, but FlexiSprite shaders are built for {shaderBuildTarget}. " +
                    "Migrate only after rebuilding shaders for the target pipeline, otherwise materials can stay pink or keep old shader bindings.",
                    MessageType.Warning);
            }

            EditorGUILayout.Space(4);

            var allPipelines = new[] { RenderPipelineTarget.BuiltIn, RenderPipelineTarget.URP, RenderPipelineTarget.HDRP };
            var current = _s.Pipeline;
            int btnIdx = 0;

            foreach (var from in allPipelines)
            {
                bool isCurrent = (from == current);
                string marker = isCurrent ? "  \u25C0 shader build" : "";
                EditorGUILayout.LabelField($"From {from}{marker}", _migHeaderStyle);

                foreach (var to in allPipelines)
                {
                    if (from == to) continue;
                    var map = GetMigrationMap(from, to);
                    if (map == null) { btnIdx++; continue; }

                    var unmapped = GetUnmappedVariants(from, map);
                    var fallbackMap = GetFallbackDetails(from, to, map);
                    var degraded = GetDegradedMappings(from, to, map);

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(16);
                    var oldBg = GUI.backgroundColor;
                    bool shadersBuiltForTarget = shaderBuildTarget == to;
                    if (shadersBuiltForTarget)
                        GUI.backgroundColor = isCurrent ? new Color(0.25f, 0.5f, 0.65f) : new Color(0.25f, 0.25f, 0.28f);
                    else
                        GUI.backgroundColor = new Color(0.55f, 0.34f, 0.18f);

                    string buttonLabel = shadersBuiltForTarget ? $"\u2192 {to}" : $"\u2192 {to}  (rebuild first)";
                    if (GUILayout.Button(buttonLabel, GUILayout.Height(20), GUILayout.Width(180)))
                    {
                        if (ValidateMigrationPreflight(from, to))
                        {
                            string warning = "";
                            if (unmapped.Count > 0)
                                warning += "\n\nNo direct equivalent (" + unmapped.Count + " variants):\n\u2022 " + string.Join("\n\u2022 ", fallbackMap);
                            if (degraded.Count > 0)
                                warning += "\n\nDegraded mappings (" + degraded.Count + "):\n\u2022 " + string.Join("\n\u2022 ", degraded);

                            if (EditorUtility.DisplayDialog("Pipeline Migration",
                                $"Migrate sprites from {from} to {to}?\nScope: {_migrationScope}{warning}", "Migrate", "Cancel"))
                            {
                                int count = RunMigration(from, to);
                                EditorUtility.DisplayDialog("Done", $"Updated {count} sprites/material bindings.", "OK");
                            }
                        }
                    }
                    GUI.backgroundColor = oldBg;

                    if (unmapped.Count > 0)
                    {
                        GUILayout.Label($"{unmapped.Count} \u2192 fallback", _migWarnStyle, GUILayout.Width(70));
                        int idx = btnIdx;
                        if (idx < _migDetailFoldout.Length)
                        {
                            string arrow = _migDetailFoldout[idx] ? "Hide" : "Show";
                            if (GUILayout.Button(arrow, EditorStyles.miniButton, GUILayout.Width(20)))
                                _migDetailFoldout[idx] = !_migDetailFoldout[idx];
                        }
                    }
                    else if (degraded.Count > 0)
                    {
                        GUILayout.Label($"{degraded.Count} GUI \u2192 Unlit", _migWarnStyle, GUILayout.Width(100));
                        int idx = btnIdx;
                        if (idx < _migDetailFoldout.Length)
                        {
                            string arrow = _migDetailFoldout[idx] ? "Hide" : "Show";
                            if (GUILayout.Button(arrow, EditorStyles.miniButton, GUILayout.Width(20)))
                                _migDetailFoldout[idx] = !_migDetailFoldout[idx];
                        }
                    }
                    else
                    {
                        GUILayout.Label("1:1 all mapped", _migOkStyle);
                    }
                    EditorGUILayout.EndHorizontal();

                    int detailIdx = btnIdx;
                    if (detailIdx < _migDetailFoldout.Length && _migDetailFoldout[detailIdx])
                    {
                        foreach (var line in fallbackMap)
                            EditorGUILayout.LabelField(line, _migFallbackStyle);
                        foreach (var line in degraded)
                            EditorGUILayout.LabelField(line, _migFallbackStyle);
                        if (fallbackMap.Count > 0 || degraded.Count > 0)
                            GUILayout.Space(2);
                    }

                    btnIdx++;
                }
                GUILayout.Space(4);
            }

            EditorGUI.indentLevel--;
            GUILayout.Space(4);
            EditorGUILayout.EndVertical();
        }

        bool ValidateMigrationPreflight(RenderPipelineTarget from, RenderPipelineTarget to)
        {
            var projectPipeline = AnimSpriteProjectSettings.DetectPipeline();
            var shaderBuildTarget = _s.Pipeline;

            if (shaderBuildTarget != to)
            {
                bool rebuild = EditorUtility.DisplayDialog(
                    "Rebuild Shaders Before Migration",
                    $"You are trying to migrate sprites from {from} to {to}, but FlexiSprite shader files are currently built for {shaderBuildTarget}.\n\n" +
                    "Migration only changes component variants and material bindings. If the shaders were not rebuilt for the target pipeline, migrated materials can stay pink or keep wrong shader passes.\n\n" +
                    $"Set the shader build target to {to} and run Full Rebuild now?",
                    "Set Target + Full Rebuild",
                    "Cancel");

                if (rebuild)
                {
                    RunFullRebuildFor(to);
                    Status($"Shaders rebuilt for {to}. Run the migration again when Unity finishes importing.", MessageType.Info);
                }

                return false;
            }

            if (projectPipeline == to)
                return true;

            return EditorUtility.DisplayDialog(
                "Project Pipeline Differs",
                $"Unity is currently running {projectPipeline}, but you selected migration to {to}.\n\n" +
                $"FlexiSprite shaders are built for {shaderBuildTarget}, so the asset side is ready, but Unity itself is not using {to}. " +
                "Only continue if you are preparing scenes for another pipeline or you will switch the Unity render pipeline asset next.",
                "Continue Migration",
                "Cancel");
        }

        List<string> GetFallbackDetails(RenderPipelineTarget from, RenderPipelineTarget to,
            VariantMap map)
        {
            var result = new List<string>();
            string fallbackName = to == RenderPipelineTarget.URP ? "Standard_URP"
                : to == RenderPipelineTarget.HDRP ? "HDRP_Unlit" : "Standard";

            foreach (var vName in System.Enum.GetNames(typeof(AnimatedSpriteShader.ShaderVariant)))
            {
                bool isSource = (from == RenderPipelineTarget.BuiltIn && !vName.Contains("URP") && !vName.Contains("HDRP"))
                    || (from == RenderPipelineTarget.URP && vName.Contains("URP"))
                    || (from == RenderPipelineTarget.HDRP && vName.Contains("HDRP"));
                if (!isSource) continue;
                var enumVal = (AnimatedSpriteShader.ShaderVariant)System.Enum.Parse(typeof(AnimatedSpriteShader.ShaderVariant), vName);
                if (!map.ContainsKey(enumVal))
                    result.Add($"{vName.Replace("_", " ")} \u2192 {fallbackName}");
            }
            return result;
        }

        List<string> GetDegradedMappings(RenderPipelineTarget from, RenderPipelineTarget to,
            VariantMap map)
        {
            var result = new List<string>();
            foreach (var kv in map)
            {
                bool sourceIsGui = kv.Key.ToString().Contains("GUI");
                bool targetIsGui = kv.Value.ToString().Contains("GUI");
                if (sourceIsGui && !targetIsGui)
                    result.Add($"{kv.Key.ToString().Replace("_", " ")} \u2192 {kv.Value.ToString().Replace("_", " ")} (Canvas UI lost)");
            }
            return result;
        }

        List<string> GetUnmappedVariants(RenderPipelineTarget from, VariantMap map)
        {
            var unmapped = new List<string>();
            foreach (var vName in System.Enum.GetNames(typeof(AnimatedSpriteShader.ShaderVariant)))
            {
                bool isSource = (from == RenderPipelineTarget.BuiltIn && !vName.Contains("URP") && !vName.Contains("HDRP"))
                    || (from == RenderPipelineTarget.URP && vName.Contains("URP"))
                    || (from == RenderPipelineTarget.HDRP && vName.Contains("HDRP"));
                if (!isSource) continue;
                var enumVal = (AnimatedSpriteShader.ShaderVariant)System.Enum.Parse(typeof(AnimatedSpriteShader.ShaderVariant), vName);
                if (!map.ContainsKey(enumVal))
                    unmapped.Add(vName.Replace("_", " "));
            }
            return unmapped;
        }

        int RunMigration(RenderPipelineTarget from, RenderPipelineTarget to)
        {
            var map = GetMigrationMap(from, to);
            if (map == null) return 0;

            var scenePaths = new System.Collections.Generic.List<string>();

            switch (_migrationScope)
            {
                case MigrationScope.AllScenes:
                    foreach (var guid in AssetDatabase.FindAssets("t:Scene"))
                        scenePaths.Add(AssetDatabase.GUIDToAssetPath(guid));
                    break;

                case MigrationScope.OpenScenes:
                    for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
                    {
                        var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                        if (scene.isLoaded) scenePaths.Add(scene.path);
                    }
                    break;

                case MigrationScope.SelectedFolder:
                    foreach (var guid in AssetDatabase.FindAssets("t:Scene", new[] { _migrationFolder }))
                        scenePaths.Add(AssetDatabase.GUIDToAssetPath(guid));
                    break;
            }

            int updated = 0;
            var fallback = GetFallbackVariant(to);
            var allSprites = UnityEngine.Object.FindObjectsOfType<AnimatedSpriteShader>(true);

            foreach (var sprite in allSprites)
            {
                if (TryApplyMigration(sprite, from, to, map, fallback, true))
                    updated++;
            }

            if (_migrationScope != MigrationScope.OpenScenes)
            {
                foreach (var path in scenePaths)
                {
                    bool alreadyOpen = false;
                    for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
                    {
                        if (UnityEngine.SceneManagement.SceneManager.GetSceneAt(i).path == path)
                        {
                            alreadyOpen = true;
                            break;
                        }
                    }

                    if (alreadyOpen)
                        continue;

                    var scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(
                        path,
                        UnityEditor.SceneManagement.OpenSceneMode.Additive);

                    var roots = scene.GetRootGameObjects();
                    bool changed = false;

                    foreach (var root in roots)
                    {
                        foreach (var sprite in root.GetComponentsInChildren<AnimatedSpriteShader>(true))
                        {
                            if (!TryApplyMigration(sprite, from, to, map, fallback, false))
                                continue;

                            changed = true;
                            updated++;
                        }
                    }

                    if (changed)
                        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);

                    UnityEditor.SceneManagement.EditorSceneManager.CloseScene(scene, true);
                }
            }

            AssetDatabase.SaveAssets();
            return updated;
        }

        static AnimatedSpriteShader.ShaderVariant GetFallbackVariant(RenderPipelineTarget pipeline)
        {
            return pipeline switch
            {
                RenderPipelineTarget.URP => AnimatedSpriteShader.ShaderVariant.Standard_URP,
                RenderPipelineTarget.HDRP => AnimatedSpriteShader.ShaderVariant.HDRP_Unlit,
                _ => AnimatedSpriteShader.ShaderVariant.Standard
            };
        }

        static bool TryApplyMigration(
            AnimatedSpriteShader sprite,
            RenderPipelineTarget from,
            RenderPipelineTarget to,
            VariantMap map,
            AnimatedSpriteShader.ShaderVariant fallback,
            bool useUndo)
        {
            if (!sprite)
                return false;

            var oldVariant = sprite.CurrentVariant;
            bool isSource = IsVariantInPipeline(oldVariant, from);
            bool isTarget = IsVariantInPipeline(oldVariant, to);

            // Target-pipeline sprites are still refreshed. This repairs scenes where the enum already
            // migrated but the Renderer/Image kept the old material from a previous tool version.
            if (!isSource && !isTarget)
                return false;

            var newVariant = oldVariant;
            if (isSource && !map.TryGetValue(oldVariant, out newVariant))
                newVariant = fallback;

            RecordMigrationObjects(sprite, useUndo);
            sprite.ChangeShaderVariant(newVariant);
            EditorUtility.SetDirty(sprite);
            return true;
        }

        static bool IsVariantInPipeline(AnimatedSpriteShader.ShaderVariant variant, RenderPipelineTarget pipeline)
        {
            string name = variant.ToString();

            return pipeline switch
            {
                RenderPipelineTarget.URP => name.Contains("URP"),
                RenderPipelineTarget.HDRP => name.Contains("HDRP"),
                _ => !name.Contains("URP") && !name.Contains("HDRP")
            };
        }

        static void RecordMigrationObjects(AnimatedSpriteShader sprite, bool useUndo)
        {
            if (useUndo)
                Undo.RecordObject(sprite, "Pipeline Migration");

            var renderer = sprite.GetComponent<Renderer>();
            if (renderer)
            {
                if (useUndo)
                    Undo.RecordObject(renderer, "Pipeline Migration");

                EditorUtility.SetDirty(renderer);
            }

            var image = sprite.GetUIImage();
            if (image)
            {
                if (useUndo)
                    Undo.RecordObject(image, "Pipeline Migration");

                EditorUtility.SetDirty(image);
            }
        }

        // Global Lighting

        static bool _lightingFoldout = true;
        static bool _hdrpEnvFoldout = true;

        void DrawGlobalLightingSection()
        {
            _lightingFoldout = EditorGUILayout.Foldout(_lightingFoldout, "Global Lighting", true, EditorStyles.foldoutHeader);
            if (!_lightingFoldout) return;

            var bgRect = EditorGUILayout.BeginVertical();
            EditorGUI.DrawRect(bgRect, new Color(0.12f, 0.12f, 0.15f));
            GUILayout.Space(4);

            var so = new SerializedObject(_s);
            so.Update();

            EditorGUILayout.LabelField("Project-wide Multipliers", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "These multipliers affect every AnimatedSprite in the project. Direct Light scales contributions " +
                "from directional/point lights; Ambient scales SH (indirect) contribution. 1 = neutral.",
                MessageType.None);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(so.FindProperty("globalLightIntensity"), new GUIContent("Direct Light"));
            EditorGUILayout.PropertyField(so.FindProperty("ambientLightIntensity"), new GUIContent("Ambient"));

            if (_s.Pipeline == RenderPipelineTarget.HDRP)
            {
                EditorGUILayout.Space(4);
                _hdrpEnvFoldout = EditorGUILayout.Foldout(_hdrpEnvFoldout, "Precise HDRP Environment", true);
                if (_hdrpEnvFoldout)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.HelpBox(
                        "HDRP-only controls. These only affect the HDRP Lit shader variant (not Unlit/Additive). " +
                        "For scene-wide exposure/fog/ambient changes, use an HDRP Volume. These are for quickly " +
                        "tuning how sprites respond to the HDRP environment without adjusting the whole scene.",
                        MessageType.Info);
                    EditorGUILayout.PropertyField(so.FindProperty("hdrpExposureCompensation"),
                        new GUIContent("Exposure Comp (stops)", "Added to HDRP's current exposure, in stops (powers of 2). 0 = unchanged."));
                    EditorGUILayout.PropertyField(so.FindProperty("hdrpAmbientMultiplier"),
                        new GUIContent("Ambient Multiplier", "Multiplier on the SH ambient term, on top of Ambient above."));
                    EditorGUILayout.PropertyField(so.FindProperty("hdrpIndirectMultiplier"),
                        new GUIContent("Indirect Multiplier", "Multiplier on indirect/bounce contributions (currently routed through ambient)."));
                    EditorGUI.indentLevel--;
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                so.ApplyModifiedProperties();
                EditorUtility.SetDirty(_s);

                int notified = AnimatedSpriteShader.BroadcastGlobalsDirty(true);
                if (notified > 0) Status($"Lighting applied to {notified} sprite(s).", MessageType.None);
                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
            }

            GUILayout.Space(4);
            EditorGUILayout.EndVertical();
        }

        static bool _groupsFoldout;

        void DrawGroupSection()
        {
            _groupsFoldout = EditorGUILayout.Foldout(_groupsFoldout, "Effect Groups & Order", true, EditorStyles.foldoutHeader);
            if (!_groupsFoldout) return;

            EditorGUILayout.LabelField("Reorder groups to change render order. Expand to reorder effects within.", EditorStyles.wordWrappedMiniLabel);
            EditorGUILayout.Space(2);

            _s.EnsureGroups();
            var groups = _s.Groups;

            var sorted = new List<int>(groups.Count);
            for (int i = 0; i < groups.Count; i++) sorted.Add(i);
            sorted.Sort((a, b) => groups[a].renderOrder.CompareTo(groups[b].renderOrder));

            for (int si = 0; si < sorted.Count; si++)
            {
                int gi = sorted[si];
                var gc = groups[gi];
                var effects = _s.GetOrderedEffects(gc.group);
                bool isAlt = si % 2 == 1;

                var groupRect = EditorGUILayout.BeginVertical();
                EditorGUI.DrawRect(groupRect, isAlt ? GroupBgAlt : GroupBg);

                EditorGUILayout.BeginHorizontal();

                bool lockedGroup = AnimSpriteProjectSettings.IsLockedGroup(gc.group);
                bool lockedAbove = si > 0 && AnimSpriteProjectSettings.IsLockedGroup(groups[sorted[si - 1]].group);
                bool lockedBelow = si < sorted.Count - 1 && AnimSpriteProjectSettings.IsLockedGroup(groups[sorted[si + 1]].group);

                GUI.enabled = !lockedGroup && !lockedAbove && si > 0;
                if (GUILayout.Button("Up", GUILayout.Width(28), GUILayout.Height(20)))
                {
                    int otherGi = sorted[si - 1];
                    SwapGroupRenderOrder(groups, gi, otherGi);
                }
                GUI.enabled = !lockedGroup && !lockedBelow && si < sorted.Count - 1;
                if (GUILayout.Button("Dn", GUILayout.Width(28), GUILayout.Height(20)))
                {
                    int otherGi = sorted[si + 1];
                    SwapGroupRenderOrder(groups, gi, otherGi);
                }
                GUI.enabled = true;

                if (lockedGroup && !gc.enabled)
                    gc.enabled = true;

                EditorGUI.BeginDisabledGroup(lockedGroup);
                EditorGUI.BeginChangeCheck();
                gc.enabled = EditorGUILayout.Toggle(gc.enabled, GUILayout.Width(16));
                if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(_s);
                EditorGUI.EndDisabledGroup();

                var nameStyle = new GUIStyle(EditorStyles.foldout);
                if (!gc.enabled) nameStyle.normal.textColor = DimText;

                string label = lockedGroup
                    ? $"{EffectGroupInfo.GetDisplayName(gc.group)} ({effects.Count})  - locked first"
                    : $"{EffectGroupInfo.GetDisplayName(gc.group)} ({effects.Count})";
                gc.foldout = EditorGUILayout.Foldout(gc.foldout, label, true, nameStyle);

                var kwStyle = new GUIStyle(EditorStyles.miniLabel) { alignment = TextAnchor.MiddleRight };
                kwStyle.normal.textColor = DimText;
                EditorGUILayout.LabelField(EffectGroupInfo.GetKeyword(gc.group), kwStyle, GUILayout.Width(110));

                var orderStyle = new GUIStyle(EditorStyles.miniLabel) { alignment = TextAnchor.MiddleCenter };
                orderStyle.normal.textColor = AccentBlue;
                EditorGUILayout.LabelField($"#{gc.renderOrder + 1}", orderStyle, GUILayout.Width(24));

                EditorGUILayout.EndHorizontal();

                if (gc.foldout)
                {
                    EditorGUI.indentLevel += 2;
                    for (int ei = 0; ei < effects.Count; ei++)
                    {
                        var def = effects[ei];
                        EditorGUILayout.BeginHorizontal();

                        GUI.enabled = ei > 0;
                        if (GUILayout.Button("\u25B2", EditorStyles.miniButton, GUILayout.Width(18), GUILayout.Height(16)))
                            SwapEffects(effects, ei, ei - 1);
                        GUI.enabled = ei < effects.Count - 1;
                        if (GUILayout.Button("\u25BC", EditorStyles.miniButton, GUILayout.Width(18), GUILayout.Height(16)))
                            SwapEffects(effects, ei, ei + 1);
                        GUI.enabled = true;

                        var efxNameStyle = new GUIStyle(EditorStyles.label) { fontSize = 11 };
                        EditorGUILayout.LabelField(def.name, efxNameStyle, GUILayout.MinWidth(140));

                        var enStyle = new GUIStyle(EditorStyles.miniLabel) { alignment = TextAnchor.MiddleRight };
                        enStyle.normal.textColor = new Color(0.45f, 0.45f, 0.5f);
                        EditorGUILayout.LabelField(def.enableShader, enStyle, GUILayout.Width(160));

                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUI.indentLevel -= 2;
                    EditorGUILayout.Space(2);
                }

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Space(4);
            if (GUILayout.Button("Reset All Order to Default", GUILayout.Height(20)))
            {
                Undo.RecordObject(_s, "Reset Order");
                _s.Groups.Clear();
                _s.EnsureGroups();
                foreach (var def in EffectRegistry.All)
                    _s.SetEffectOrder(def.enableShader, _s.GetDefaultEffectOrder(def.enableShader));
                Status("Order reset to default.", MessageType.Info);
            }
        }

        // Build

        void DrawBuildSection()
        {
            _s.EnsureBuildKeywordModeDefault();
            var detected = AnimSpriteProjectSettings.DetectPipeline();
            bool pipelineMismatch = _s.Pipeline != detected;
            if (pipelineMismatch)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox(
                    $"Unity is running {detected}, but FlexiSprite shaders are built for {_s.Pipeline}. " +
                    "Set the target and rebuild before migrating materials to the active project pipeline.",
                    MessageType.Warning);
                if (GUILayout.Button("Set Target", GUILayout.Width(76), GUILayout.Height(36)))
                    SetShaderBuildTarget(detected);
                if (GUILayout.Button("Set + Rebuild", GUILayout.Width(96), GUILayout.Height(36)))
                    RunFullRebuildFor(detected);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space(4);
            }

            if (_s.Pipeline == RenderPipelineTarget.HDRP)
            {
                EditorGUILayout.HelpBox(
                    "HDRP has no Canvas/UGUI shader. GUI variants fall back to HDRP Unlit — they render " +
                    "as world-space Unlit quads, not as Canvas UI. Keep UI sprites on Built-In or URP.",
                    MessageType.Info);
                EditorGUILayout.Space(4);
            }

            EditorGUILayout.Space(2);
            var oldBg = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.55f, 0.55f, 0.58f);
            var rebuildStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                fixedHeight = 38,
                alignment = TextAnchor.MiddleCenter
            };
            if (GUILayout.Button("FULL REBUILD - HLSL + C# + Shaders", rebuildStyle))
                RunFullRebuild();
            GUI.backgroundColor = oldBg;

            EditorGUILayout.Space(6);

            var kwModeProp = _s.GetType().GetField("editorKeywordMode");
            if (kwModeProp != null)
            {
                var headerRect = EditorGUILayout.GetControlRect(false, 20);
                EditorGUI.DrawRect(headerRect, new Color(0.12f, 0.12f, 0.15f));
                var boldStyle = new GUIStyle(EditorStyles.boldLabel) { fontSize = 12 };
                EditorGUI.LabelField(headerRect, "  Keyword Compilation Mode", boldStyle);

                EditorGUI.BeginChangeCheck();
                var currentMode = (EditorKeywordMode)kwModeProp.GetValue(_s);
                EditorGUILayout.Space(2);
                var newMode = (EditorKeywordMode)EditorGUILayout.EnumPopup(currentMode, EditorStyles.popup);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_s, "Change Keyword Mode");
                    kwModeProp.SetValue(_s, newMode);
                    EditorUtility.SetDirty(_s);
                    AnimatedSpriteHLSLGenerator.GenerateKeywordsHLSL();
                    AnimatedSpriteHLSLGenerator.GenerateEffectsHLSL();
                    AnimatedSpriteShaderFileGenerator.GenerateAll();
                    if (newMode == EditorKeywordMode.ShaderFeature)
                        AnimSpriteShaderFeatureVariantKeeper.RefreshForCurrentProject();
                    else
                        AnimSpriteShaderFeatureVariantKeeper.ClearGeneratedAssets();
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    Status($"Keyword mode changed to {newMode}. HLSL, keywords, shader files, and shader_feature keep-alive assets regenerated.", MessageType.Info);
                }

                string desc = currentMode switch
                {
                    EditorKeywordMode.MultiCompile => "All variants compiled. InEditor compilation may be visible",
                    EditorKeywordMode.ShaderFeature =>
                        "Only referenced variants compile. FlexiSprite generates keep-alive material assets for used shader variants because component materials are dynamic.",
                    EditorKeywordMode.NoKeywords => "Uber-shader, zero compile time. If possible please keep this option on for " +
                    "seamless developement with this asset. Build behavior is controlled by Build Keyword Override below.",
                    _ => ""
                };
                MessageType mt = currentMode switch
                {
                    EditorKeywordMode.NoKeywords => MessageType.Warning,
                    EditorKeywordMode.ShaderFeature => MessageType.Info,
                    _ => MessageType.None
                };
                EditorGUILayout.HelpBox(desc, mt);

                EditorGUILayout.Space(4);
                var buildHeaderRect = EditorGUILayout.GetControlRect(false, 20);
                EditorGUI.DrawRect(buildHeaderRect, new Color(0.12f, 0.12f, 0.15f));
                EditorGUI.LabelField(buildHeaderRect, "  Build Keyword Override", boldStyle);

                EditorGUI.BeginChangeCheck();
                var newBuildMode = (BuildKeywordMode)EditorGUILayout.EnumPopup(_s.buildKeywordMode, EditorStyles.popup);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_s, "Change Build Keyword Override");
                    _s.buildKeywordMode = newBuildMode;
                    EditorUtility.SetDirty(_s);
                    AssetDatabase.SaveAssets();
                    Status($"Build keyword override changed to {newBuildMode}.", MessageType.Info);
                }

                string buildDesc = _s.buildKeywordMode switch
                {
                    BuildKeywordMode.MultiCompile =>
                        "Builds regenerate group keywords as multi_compile_local. This was the previous default and keeps all group variants available.",
                    BuildKeywordMode.ForceAllGroupsNoKeywords =>
                        "Builds regenerate as an uber/no-feature-block shader: all _FX_* groups are #defined as 1. "
                        + "No group variant stripping, but larger compiled shaders.",
                    BuildKeywordMode.KeepEditorMode =>
                        "Builds keep the current editor keyword mode exactly. "
                        + "Use this when you want ShaderFeature or NoKeywords to remain active in builds.",
                    BuildKeywordMode.ShaderFeature =>
                        "Builds regenerate group keywords as shader_feature_local and generate Resources keep-alive materials "
                        + "for used FlexiSprite shader variants so Unity does not strip dynamic _FX_* combinations.",
                    _ => ""
                };
                MessageType buildMt = _s.buildKeywordMode switch
                {
                    BuildKeywordMode.ForceAllGroupsNoKeywords => MessageType.Warning,
                    BuildKeywordMode.KeepEditorMode => MessageType.Info,
                    BuildKeywordMode.ShaderFeature => MessageType.Warning,
                    _ => MessageType.Info
                };
                EditorGUILayout.HelpBox(buildDesc, buildMt);
            }
        }

        void RunFullRebuildFor(RenderPipelineTarget target)
        {
            var result = RebuildShadersForPipeline(target);
            Status(result.Message, result.Type);
        }

        void RunFullRebuild()
        {
            var result = RebuildShadersForPipeline(_s.Pipeline);
            Status(result.Message, result.Type);
        }

        // Status

        void DrawStatus()
        {
            if (!string.IsNullOrEmpty(_statusMsg) && EditorApplication.timeSinceStartup - _statusTime < 5)
                EditorGUILayout.HelpBox(_statusMsg, _statusType);
        }

        void Status(string msg, MessageType type)
        {
            _statusMsg = msg;
            _statusType = type;
            _statusTime = EditorApplication.timeSinceStartup;
            Repaint();
        }

        // Actions

        void SwapGroupRenderOrder(List<AnimSpriteProjectSettings.GroupConfig> list, int idxA, int idxB)
        {
            if (AnimSpriteProjectSettings.IsLockedGroup(list[idxA].group) ||
                AnimSpriteProjectSettings.IsLockedGroup(list[idxB].group))
                return;

            Undo.RecordObject(_s, "Reorder Groups");
            int orderA = list[idxA].renderOrder;
            int orderB = list[idxB].renderOrder;
            list[idxA].renderOrder = orderB;
            list[idxB].renderOrder = orderA;
            _s.EnsureGroups();
            EditorUtility.SetDirty(_s);
        }

        void SwapEffects(List<EffectDef> effects, int a, int b)
        {
            Undo.RecordObject(_s, "Reorder Effects");
            int orderA = _s.GetEffectOrder(effects[a].enableShader);
            int orderB = _s.GetEffectOrder(effects[b].enableShader);
            _s.SetEffectOrder(effects[a].enableShader, orderB);
            _s.SetEffectOrder(effects[b].enableShader, orderA);
        }

        // Shader File Patching

        void PatchShaderFiles()
        {
            var result = PatchShaderFilesForSettings(_s);
            Status(result.Message, result.Type);
        }

        static ShaderRebuildResult PatchShaderFilesForSettings(AnimSpriteProjectSettings settings)
        {
            string folder = settings.ShaderFolder;
            if (!Directory.Exists(folder))
            {
                return new ShaderRebuildResult(
                    false,
                    $"Folder not found: {folder}",
                    MessageType.Error);
            }

            string[] shaderFiles = Directory.GetFiles(folder, "*.shader", SearchOption.AllDirectories);
            if (shaderFiles.Length == 0)
            {
                return new ShaderRebuildResult(
                    false,
                    $"No .shader files found in {folder}",
                    MessageType.Warning);
            }

            string targetDefine = settings.PipelineDefine;
            string[] allDefines = { "AIO_URP", "AIO_HDRP", "AIO_BUILTIN" };
            int patched = 0;

            foreach (string path in shaderFiles)
            {
                string content = File.ReadAllText(path);
                string original = content;

                bool hasAnyDefine = false;
                foreach (string def in allDefines)
                {
                    string patternDefine = $"#define {def}";
                    if (!content.Contains(patternDefine))
                        continue;

                    hasAnyDefine = true;

                    if (def != targetDefine)
                        content = content.Replace(patternDefine, $"#define {targetDefine}");
                }

                if (!hasAnyDefine)
                {
                    content = Regex.Replace(
                        content,
                        @"(HLSLPROGRAM|CGPROGRAM)\s*\n",
                        $"$1\n            #define {targetDefine} 1\n",
                        RegexOptions.Multiline);
                }

                content = content.Replace(
                    "#include \"AllInOneKeywords.hlsl\"",
                    "#include_with_pragmas \"AllInOneKeywords.hlsl\"");

                content = Regex.Replace(
                    content,
                    @"#include\s+""([^""]*AllInOneKeywords\.hlsl)""",
                    "#include_with_pragmas \"$1\"");

                if (content == original)
                    continue;

                File.WriteAllText(path, content);
                patched++;
            }

            return new ShaderRebuildResult(
                true,
                $"{patched}/{shaderFiles.Length} .shader files with #define {targetDefine} 1",
                MessageType.Info);
        }

        // Hlsl Generation

        void GenerateHLSL()
        {
            _s.ApplyOrderToRegistry();

            if (HasType("AnimatedSpriteHLSLGenerator"))
            {
                var genType = Type.GetType("AnimatedSpriteHLSLGenerator");
                var method = genType?.GetMethod("GenerateEffectsHLSL");
                method?.Invoke(null, null);

                var kwMethod = genType?.GetMethod("GenerateKeywordsHLSL");
                kwMethod?.Invoke(null, null);

                Status("HLSL generated. Effects ordered by current configuration.", MessageType.Info);
            }
            else
            {
                if (HasType("AnimatedSpriteHLSLRefactorer"))
                {
                    var refType = Type.GetType("AnimatedSpriteHLSLRefactorer");
                    refType?.GetMethod("Refactor")?.Invoke(null, null);
                    Status("HLSL refactored via auto-extractor.", MessageType.Info);
                }
                else
                {
                    Status("No HLSL generator found. Add AnimatedSpriteHLSLGenerator.cs or AnimatedSpriteHLSLRefactorer.cs", MessageType.Error);
                }
            }
        }

        static bool HasType(string name)
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (asm.GetType(name) != null) return true;
            }
            return false;
        }
    }
    public class AnimSpriteVariantScanner : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;
        public static HashSet<AnimatedSpriteShader.ShaderVariant> UsedVariantsInBuild = new HashSet<AnimatedSpriteShader.ShaderVariant>();

        public void OnPreprocessBuild(BuildReport report)
        {
            UsedVariantsInBuild.Clear();
            if (!EditorPrefs.GetBool("AnimSprite_AutoIncludeBuildVariants", true)) return;

            var scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
            foreach (var path in scenes)
            {
                try
                {
                    var scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(path, UnityEditor.SceneManagement.OpenSceneMode.Additive);
                    foreach (var root in scene.GetRootGameObjects())
                    {
                        var sprites = root.GetComponentsInChildren<AnimatedSpriteShader>(true);
                        foreach (var sprite in sprites)
                        {
                            if(!UsedVariantsInBuild.Contains(sprite.CurrentVariant)) UsedVariantsInBuild.Add(sprite.CurrentVariant);
                        }
                    }
                    UnityEditor.SceneManagement.EditorSceneManager.CloseScene(scene, true);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[AnimSprite] Failed to scan scene {path} for variants: {e.Message}");
                }
            }

            if (UsedVariantsInBuild.Count > 0)
            {
                Debug.Log($"[AnimSprite] Auto-included {UsedVariantsInBuild.Count} variants found actively used in Build Settings scenes.");
            }
        }
    }
    public class AnimSpriteBuildStripper : IPreprocessShaders
    {
        public int callbackOrder => 0;

        public void OnProcessShader(Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> data)
        {
            if (!shader.name.Contains("AnimatedSprite")) return;

            var settings = AnimSpriteProjectSettings.Instance;
            if (settings == null || settings.excludedVariants == null || settings.excludedVariants.Count == 0) return;

            bool autoIncludeEnabled = EditorPrefs.GetBool("AnimSprite_AutoIncludeBuildVariants", true);

            foreach (var excludedVariant in settings.excludedVariants)
            {
                int variantIndex = (int)excludedVariant;
                bool isExcludedTarget = false;

                if (variantIndex >= 0 && variantIndex < AnimatedSpriteShader.ShaderPaths.Length)
                {
                    string exactShaderName = AnimatedSpriteShader.ShaderPaths[variantIndex];
                    if (shader.name == exactShaderName)
                    {
                        isExcludedTarget = true;
                    }
                }
                else
                {
                    string vName = excludedVariant.ToString();
                    if (shader.name.EndsWith(vName) || shader.name.EndsWith(vName.Replace("_", " ")))
                    {
                        isExcludedTarget = true;
                    }
                }

                if (isExcludedTarget)
                {
                    if (autoIncludeEnabled
                        && AnimSpriteVariantScanner.UsedVariantsInBuild.Contains((AnimatedSpriteShader.ShaderVariant)excludedVariant))
                    {
                        continue;
                    }

                    data.Clear();
                    break;
                }
            }
        }
    }

}
#endif