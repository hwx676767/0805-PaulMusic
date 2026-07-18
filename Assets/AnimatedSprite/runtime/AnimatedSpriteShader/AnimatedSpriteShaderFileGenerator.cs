#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;


namespace FlexiSprite
{
    /// <summary>
    /// Generates all 17 .shader variant files.
    /// </summary>

    public static class AnimatedSpriteShaderFileGenerator
    {
        static string ShaderDir
        {
            get
            {

                string[] guids = AssetDatabase.FindAssets($"{"AnimatedSprite"} t:Folder");
                if (guids.Length > 0)
                {
                    string scriptPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                    string directory = Path.GetDirectoryName(scriptPath).Replace('\\', '/');


                    int lastSlash = directory.LastIndexOf('/');
                    if (lastSlash > 0)
                    {
                        return directory.Substring(0, lastSlash) + "/Shaders/";
                    }
                }
        
                return "Assets/AnimatedSprite/Shaders/";
            }
        }
        const string IncludePath = "Include/"; // relative to shader files

        struct VariantDef
        {
            public string fileName;
            public string shaderPath;
            public string pipeline;
            public string blend;
            public string tags;
            public bool cutout;
            public bool premulAlpha;
            public bool needsShadowPass;
            public bool isLit;
            public bool is2DLit; // URP 2D Lit specifically (needs SHAPE_LIGHT, CombinedShapeLightShared)
            public bool screenSpace;
            public bool useCompactProperties;
        }

        static readonly VariantDef[] Variants = {
            // Built-In
            new VariantDef { fileName = "AIO_Standard", shaderPath = "AnimatedSprite/Standard",
                pipeline = "AIO_BUILTIN", blend = "Blend SrcAlpha OneMinusSrcAlpha", tags = "\"Queue\"=\"Transparent\" \"RenderType\"=\"Transparent\" \"IgnoreProjector\"=\"True\"" },
            new VariantDef { fileName = "AIO_Additive", shaderPath = "AnimatedSprite/Additive",
                pipeline = "AIO_BUILTIN", blend = "Blend SrcAlpha One", tags = "\"Queue\"=\"Transparent\" \"RenderType\"=\"Transparent\" \"IgnoreProjector\"=\"True\"" },
            new VariantDef { fileName = "AIO_Multiply", shaderPath = "AnimatedSprite/Multiply",
                pipeline = "AIO_BUILTIN", blend = "Blend DstColor Zero", tags = "\"Queue\"=\"Transparent\" \"RenderType\"=\"Transparent\" \"IgnoreProjector\"=\"True\"" },
            new VariantDef { fileName = "AIO_GUI", shaderPath = "AnimatedSprite/GUI",
                pipeline = "AIO_BUILTIN", blend = "Blend SrcAlpha OneMinusSrcAlpha", tags = "\"Queue\"=\"Transparent\" \"RenderType\"=\"Transparent\" \"IgnoreProjector\"=\"True\"" },
            new VariantDef { fileName = "AIO_GUIAdditive", shaderPath = "AnimatedSprite/GUI Additive",
                pipeline = "AIO_BUILTIN", blend = "Blend SrcAlpha One", tags = "\"Queue\"=\"Transparent\" \"RenderType\"=\"Transparent\" \"IgnoreProjector\"=\"True\"" },
            new VariantDef { fileName = "AIO_StandardPremultiply", shaderPath = "AnimatedSprite/Standard Premultiply",
                pipeline = "AIO_BUILTIN", blend = "Blend One OneMinusSrcAlpha", premulAlpha = true, tags = "\"Queue\"=\"Transparent\" \"RenderType\"=\"Transparent\"" },
            new VariantDef { fileName = "AIO_AdditiveSoft", shaderPath = "AnimatedSprite/Additive Soft",
                pipeline = "AIO_BUILTIN", blend = "Blend One OneMinusSrcColor", tags = "\"Queue\"=\"Transparent\" \"RenderType\"=\"Transparent\"" },
            new VariantDef { fileName = "AIO_GUIPremultiply", shaderPath = "AnimatedSprite/GUI Premultiply",
                pipeline = "AIO_BUILTIN", blend = "Blend One OneMinusSrcAlpha", premulAlpha = true, tags = "\"Queue\"=\"Transparent\" \"RenderType\"=\"Transparent\"" },
            new VariantDef { fileName = "AIO_ScreenSpaceOverlay", shaderPath = "AnimatedSprite/Screen Space Overlay",
                pipeline = "AIO_BUILTIN", blend = "Blend SrcAlpha OneMinusSrcAlpha", tags = "\"Queue\"=\"Overlay+100\" \"RenderType\"=\"Transparent\"", screenSpace = true },

            // Built-In 3D Lit
            new VariantDef { fileName = "AIO_Lit3D_BuiltIn", shaderPath = "AnimatedSprite/3D Lit Built-In",
                pipeline = "AIO_BUILTIN", blend = "Blend SrcAlpha OneMinusSrcAlpha", isLit = true, needsShadowPass = true,
                tags = "\"Queue\"=\"Transparent\" \"RenderType\"=\"Transparent\"" },
            new VariantDef { fileName = "AIO_Lit3DCutout_BuiltIn", shaderPath = "AnimatedSprite/3D Lit Cutout Built-In",
                pipeline = "AIO_BUILTIN", blend = "Blend SrcAlpha OneMinusSrcAlpha", isLit = true, cutout = true, needsShadowPass = true, useCompactProperties = true,
                tags = "\"Queue\"=\"AlphaTest\" \"RenderType\"=\"TransparentCutout\"" },

            // Urp Unlit
            new VariantDef { fileName = "AIO_Standard_URP", shaderPath = "AnimatedSprite/Standard URP",
                pipeline = "AIO_URP", blend = "Blend SrcAlpha OneMinusSrcAlpha",
                tags = "\"Queue\"=\"Transparent\" \"RenderType\"=\"Transparent\" \"RenderPipeline\"=\"UniversalPipeline\"" },
            new VariantDef { fileName = "AIO_Additive_URP", shaderPath = "AnimatedSprite/Additive URP",
                pipeline = "AIO_URP", blend = "Blend SrcAlpha One",
                tags = "\"Queue\"=\"Transparent\" \"RenderType\"=\"Transparent\" \"RenderPipeline\"=\"UniversalPipeline\"" },
            new VariantDef { fileName = "AIO_Premultiply_URP", shaderPath = "AnimatedSprite/Premultiply URP",
                pipeline = "AIO_URP", blend = "Blend One OneMinusSrcAlpha", premulAlpha = true,
                tags = "\"Queue\"=\"Transparent\" \"RenderType\"=\"Transparent\" \"RenderPipeline\"=\"UniversalPipeline\"" },
            new VariantDef { fileName = "AIO_GUI_URP", shaderPath = "AnimatedSprite/GUI URP",
                pipeline = "AIO_URP", blend = "Blend SrcAlpha OneMinusSrcAlpha",
                tags = "\"Queue\"=\"Transparent\" \"RenderType\"=\"Transparent\" \"RenderPipeline\"=\"UniversalPipeline\"" },
            new VariantDef { fileName = "AIO_GUIAdditive_URP", shaderPath = "AnimatedSprite/GUI Additive URP",
                pipeline = "AIO_URP", blend = "Blend SrcAlpha One",
                tags = "\"Queue\"=\"Transparent\" \"RenderType\"=\"Transparent\" \"RenderPipeline\"=\"UniversalPipeline\"" },

            // Urp 2D Lit
            new VariantDef { fileName = "AIO_Lit2D_URP", shaderPath = "AnimatedSprite/2D Lit URP",
                pipeline = "AIO_URP", blend = "Blend SrcAlpha OneMinusSrcAlpha", isLit = true, is2DLit = true,
                tags = "\"Queue\"=\"Transparent\" \"RenderType\"=\"Transparent\" \"RenderPipeline\"=\"UniversalPipeline\"" },
            new VariantDef { fileName = "AIO_Lit2DAdditive_URP", shaderPath = "AnimatedSprite/2D Lit Additive URP",
                pipeline = "AIO_URP", blend = "Blend SrcAlpha One", isLit = true, is2DLit = true,
                tags = "\"Queue\"=\"Transparent\" \"RenderType\"=\"Transparent\" \"RenderPipeline\"=\"UniversalPipeline\"" },

            // Urp 3D Lit
            new VariantDef { fileName = "AIO_Lit3D_URP", shaderPath = "AnimatedSprite/3D Lit URP",
                pipeline = "AIO_URP", blend = "Blend SrcAlpha OneMinusSrcAlpha", isLit = true, needsShadowPass = true,
                tags = "\"Queue\"=\"Transparent\" \"RenderType\"=\"Transparent\" \"RenderPipeline\"=\"UniversalPipeline\"" },
            new VariantDef { fileName = "AIO_Lit3DCutout_URP", shaderPath = "AnimatedSprite/3D Lit Cutout URP",
                pipeline = "AIO_URP", blend = "Blend SrcAlpha OneMinusSrcAlpha", isLit = true, cutout = true, needsShadowPass = true,
                tags = "\"Queue\"=\"AlphaTest\" \"RenderType\"=\"TransparentCutout\" \"RenderPipeline\"=\"UniversalPipeline\"" },
            new VariantDef { fileName = "AIO_Lit3DAdditive_URP", shaderPath = "AnimatedSprite/3D Lit Additive URP",
                pipeline = "AIO_URP", blend = "Blend SrcAlpha One", isLit = true,
                tags = "\"Queue\"=\"Transparent\" \"RenderType\"=\"Transparent\" \"RenderPipeline\"=\"UniversalPipeline\"" },

            // Hdrp
            new VariantDef { fileName = "AIO_HDRP_Unlit", shaderPath = "AnimatedSprite/HDRP Unlit",
                pipeline = "AIO_HDRP", blend = "Blend SrcAlpha OneMinusSrcAlpha",
                tags = "\"Queue\"=\"Transparent\" \"RenderType\"=\"Transparent\" \"RenderPipeline\"=\"HDRenderPipeline\"" },
            new VariantDef { fileName = "AIO_HDRP_Additive", shaderPath = "AnimatedSprite/HDRP Additive",
                pipeline = "AIO_HDRP", blend = "Blend SrcAlpha One",
                tags = "\"Queue\"=\"Transparent\" \"RenderType\"=\"Transparent\" \"RenderPipeline\"=\"HDRenderPipeline\"" },
            new VariantDef { fileName = "AIO_HDRP_Lit", shaderPath = "AnimatedSprite/HDRP Lit",
                pipeline = "AIO_HDRP", blend = "Blend SrcAlpha OneMinusSrcAlpha", isLit = true, needsShadowPass = true,
                tags = "\"Queue\"=\"Transparent\" \"RenderType\"=\"Transparent\" \"RenderPipeline\"=\"HDRenderPipeline\"" },
        };


        static bool UsesCompactProperties(VariantDef variant)
        {
            return variant.isLit || variant.useCompactProperties;
        }

        static int DetectURPMajorVersion()
        {
            int fallback = 0;
#if UNITY_6000_0_OR_NEWER
            fallback = 17;
#elif UNITY_2023_1_OR_NEWER
            fallback = 15;
#elif UNITY_2022_1_OR_NEWER
            fallback = 14;
#elif UNITY_2021_3_OR_NEWER
            fallback = 12;
#endif

            try
            {
                var package = UnityEditor.PackageManager.PackageInfo.FindForAssetPath(
                    "Packages/com.unity.render-pipelines.universal/package.json");

                if (package == null)
                    return fallback;

                string[] parts = package.version.Split('.');

                if (parts.Length == 0)
                    return fallback;

                int.TryParse(parts[0], out int major);
                return major > 0 ? major : fallback;
            }
            catch
            {
                return fallback;
            }
        }

        static bool URPIncludeExists(string packagePath)
        {
            return AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(packagePath) != null;
        }

        static bool UseModernURP2DIncludes()
        {
            // Unity 6 / URP 17 Sprite-Lit-Default uses Core2D + ShapeLightShared style includes.
            // URP 12-16 can use the older handwritten shader route: Core.hlsl + LightingUtility +
            // CombinedShapeLightShared. This avoids generating a hard Core2D include in projects
            // where that package file is not available.
            return DetectURPMajorVersion() >= 17 &&
                   URPIncludeExists("Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl");
        }
        static void EmitPipelineVersionDefines(StringBuilder sb, VariantDef v)
        {
            if (v.pipeline == "AIO_URP")
            {
                int urpMajor = DetectURPMajorVersion();
                if (urpMajor > 0)
                {
                    sb.AppendLine($"            #ifndef AIO_URP_MAJOR");
                    sb.AppendLine($"            #define AIO_URP_MAJOR {urpMajor}");
                    sb.AppendLine($"            #endif");
                    if (urpMajor >= 12)
                    {
                        sb.AppendLine("            #ifndef AIO_URP_12_PLUS");
                        sb.AppendLine("            #define AIO_URP_12_PLUS 1");
                        sb.AppendLine("            #endif");
                    }
                    if (urpMajor >= 14)
                    {
                        sb.AppendLine("            #ifndef AIO_URP_14_PLUS");
                        sb.AppendLine("            #define AIO_URP_14_PLUS 1");
                        sb.AppendLine("            #endif");
                    }
                    if (urpMajor >= 17)
                    {
                        sb.AppendLine("            #ifndef AIO_URP_17_PLUS");
                        sb.AppendLine("            #define AIO_URP_17_PLUS 1");
                        sb.AppendLine("            #endif");
                    }
                }
            }
        }


        public static void GenerateAll()
        {
            var settings = AnimSpriteProjectSettings.Instance;
            var target = settings != null ? settings.Pipeline : AnimSpriteProjectSettings.DetectPipeline();
            GenerateAll(target);
        }

        public static void GenerateAll(RenderPipelineTarget target)
        {
            if (!Directory.Exists(ShaderDir)) Directory.CreateDirectory(ShaderDir);

            string srpProps = ExtractSRPProperties();
            string activePipeline = PipelineDefineFor(target);

            var settings = AnimSpriteProjectSettings.Instance;
            if (settings != null && settings.Pipeline != target)
            {
                settings.Pipeline = target;
                EditorUtility.SetDirty(settings);
            }

            int generated = 0, removed = 0;
            foreach (var v in Variants)
            {
                string path = ShaderDir + v.fileName + ".shader";
                if (v.pipeline == activePipeline)
                {
                    GenerateShaderFile(v, srpProps);
                    generated++;
                }
                else
                {
                    // Delete shader files for other pipelines so they don't compile in the wrong project.
                    if (File.Exists(path))
                    {
                        File.Delete(path);

                        string metaPath = path + ".meta";
                        if (File.Exists(metaPath))
                            File.Delete(metaPath);

                        removed++;
                    }
                }
            }

            AssetDatabase.Refresh();
            Debug.Log($"[AnimSprite] Generated {generated} shaders, removed {removed} non-{activePipeline} shaders.");
        }

        public static int DeleteShadersNotForPipeline(RenderPipelineTarget target)
        {
            if (!Directory.Exists(ShaderDir))
                return 0;

            string activePipeline = PipelineDefineFor(target);
            int removed = 0;

            foreach (var v in Variants)
            {
                if (v.pipeline == activePipeline)
                    continue;

                string path = ShaderDir + v.fileName + ".shader";
                if (!File.Exists(path))
                    continue;

                File.Delete(path);

                string metaPath = path + ".meta";
                if (File.Exists(metaPath))
                    File.Delete(metaPath);

                removed++;
            }

            if (removed > 0)
                AssetDatabase.Refresh();

            return removed;
        }

        static string PipelineDefineFor(RenderPipelineTarget target)
        {
            switch (target)
            {
                case RenderPipelineTarget.URP:
                    return "AIO_URP";
                case RenderPipelineTarget.HDRP:
                    return "AIO_HDRP";
                default:
                    return "AIO_BUILTIN";
            }
        }

        static string ExtractSRPProperties()
        {
            string[] files = Directory.GetFiles(Application.dataPath, "AllInOneProperties.hlsl", SearchOption.AllDirectories);
            if (files.Length == 0)
            {
                Debug.LogWarning("[AnimSprite] AllInOneProperties.hlsl not found. Cannot auto-generate SRP properties.");
                return "";
            }

            string text = File.ReadAllText(files[0]);
            int start = text.IndexOf("CBUFFER_START(UnityPerMaterial)");
            if (start == -1) return "";
            int end = text.IndexOf("CBUFFER_END", start);
            if (end == -1) return "";

            string block = text.Substring(start, end - start);
            StringBuilder props = new StringBuilder();

            // Match variable declarations such as "float _Name;" or "half4 _Color;".
            var regex = new Regex(@"(?:float|half)([234]?)\s+([_a-zA-Z0-9]+)\s*;");
            var matches = regex.Matches(block);

            // Prevent duplicating properties already explicitly defined in EmitProperties()
            HashSet<string> ignoredProps = new HashSet<string> {
                "_MainTex", "_Color", "_ShadowAlphaClip", "_StencilComp",
                "_Stencil", "_StencilOp", "_StencilWriteMask", "_StencilReadMask", "_ColorMask"
            };

            props.AppendLine();
            props.AppendLine("        // --- Auto-generated SRP Batcher Properties ---");
            foreach (Match m in matches)
            {
                string typeSuffix = m.Groups[1].Value;
                string varName = m.Groups[2].Value;

                if (ignoredProps.Contains(varName)) continue;

                if (string.IsNullOrEmpty(typeSuffix))
                {
                    props.AppendLine($"        [HideInInspector] {varName} (\"{varName}\", Float) = 0");
                }
                else
                {
                    props.AppendLine($"        [HideInInspector] {varName} (\"{varName}\", Vector) = (0,0,0,0)");
                }
            }
            return props.ToString();
        }

        static void GenerateShaderFile(VariantDef v, string srpProps)
        {
            var sb = new StringBuilder();
            bool isURP = v.pipeline == "AIO_URP";
            bool isHDRP = v.pipeline == "AIO_HDRP";
            bool isBuiltIn = v.pipeline == "AIO_BUILTIN";
            int urpMajor = isURP ? DetectURPMajorVersion() : 0;
            string lang = (isURP || isHDRP) ? "HLSL" : "CG";
            string progStart = lang == "HLSL" ? "HLSLPROGRAM" : "CGPROGRAM";
            string progEnd = lang == "HLSL" ? "ENDHLSL" : "ENDCG";

            sb.AppendLine($"Shader \"{v.shaderPath}\"");
            sb.AppendLine("{");

            EmitProperties(sb, v, srpProps);

            sb.AppendLine("    SubShader");
            sb.AppendLine("    {");
            sb.AppendLine($"        Tags {{ {v.tags} }}");
            sb.AppendLine(v.cutout ? "        Blend One Zero" : $"        {v.blend}");
            sb.AppendLine("        Cull Off");
            sb.AppendLine(v.cutout ? "        ZWrite On" : "        ZWrite Off");

            // Stencil block applies to every variant so stencilEnabled on the component works
            // regardless of sprite or UI usage. GUI variants additionally write _ColorMask and
            // ZTest for Canvas rendering.
            sb.AppendLine();
            sb.AppendLine("        Stencil");
            sb.AppendLine("        {");
            sb.AppendLine("            Ref [_Stencil]");
            sb.AppendLine("            Comp [_StencilComp]");
            sb.AppendLine("            Pass [_StencilOp]");
            sb.AppendLine("            ReadMask [_StencilReadMask]");
            sb.AppendLine("            WriteMask [_StencilWriteMask]");
            sb.AppendLine("        }");

            // Canvas passes unity_GUIZTestMode dynamically. UI breaks without it.
            if (v.fileName.Contains("GUI"))
            {
                sb.AppendLine("        ZTest [unity_GUIZTestMode]");
                sb.AppendLine("        ColorMask [_ColorMask]");
            }
            else if (v.screenSpace)
            {
                sb.AppendLine("        ZTest Always");
            }
            sb.AppendLine();

            // Main pass
            sb.AppendLine("        Pass");
            sb.AppendLine("        {");
            // Pass tags differ by variant type
            if (isURP && v.is2DLit)
                sb.AppendLine("            Tags { \"LightMode\" = \"Universal2D\" }");
            else if (isURP && v.isLit)
                sb.AppendLine("            Tags { \"LightMode\" = \"UniversalForward\" }");
            else if (isBuiltIn && v.isLit)
                sb.AppendLine("            Tags { \"LightMode\" = \"ForwardBase\" }");
            else if (v.tags.Contains("LightMode")) { /* already in SubShader tags */ }

            sb.AppendLine($"            {progStart}");
            sb.AppendLine("            #pragma vertex vert");
            sb.AppendLine("            #pragma fragment frag");
            sb.AppendLine("            #pragma multi_compile_instancing");
            if (isBuiltIn) sb.AppendLine("            #pragma multi_compile _ PIXELSNAP_ON");

            // URP 2D lighting: unconditional #defines for all 4 shape-light types + DEBUG_DISPLAY off.
            // Rationale: the classic `#pragma multi_compile USE_SHAPE_LIGHT_TYPE_N __` pattern generates
            // 2^4 = 16 variants per pixel program just for shape lights, multiplied by DEBUG_DISPLAY = 32,
            // before any of our FX keywords. We only need the variant where all 4 types are active
            // (sprite lit by any combination of 2D lights in the scene) — forcing them via #define
            // collapses these 32 variants to 1 with zero runtime cost (URP's 2D Lit composition
            // already branches on per-light data, not these keywords).
            if (isURP && v.is2DLit)
            {
                sb.AppendLine("            #define USE_SHAPE_LIGHT_TYPE_0 1");
                sb.AppendLine("            #define USE_SHAPE_LIGHT_TYPE_1 1");
                sb.AppendLine("            #define USE_SHAPE_LIGHT_TYPE_2 1");
                sb.AppendLine("            #define USE_SHAPE_LIGHT_TYPE_3 1");
                // DEBUG_DISPLAY is URP's render-mode-debug keyword. Our sprites never participate in
                // the URP debug display, so we don't need the variant. Not emitting the multicompile
                // leaves the keyword undefined = debug disabled, which is the default we want.
            }
            // URP 3D lighting pragmas. These collectively produce ~thousands of variants already —
            // main light shadows × soft shadows × additional lights × Forward+ × light layers ×
            // cookies × APV. On Lit shaders we must NOT stack our 256 FX variants on top.
            if (isURP && v.isLit && !v.is2DLit)
            {
                sb.AppendLine("            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN");
                sb.AppendLine("            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS");
                sb.AppendLine("            #pragma multi_compile_fragment _ _SHADOWS_SOFT _SHADOWS_SOFT_LOW _SHADOWS_SOFT_MEDIUM _SHADOWS_SOFT_HIGH");
                if (urpMajor >= 17)
                {
                    sb.AppendLine("            #if UNITY_VERSION >= 600010");
                    sb.AppendLine("            #pragma multi_compile _ _CLUSTER_LIGHT_LOOP");
                    sb.AppendLine("            #else");
                    sb.AppendLine("            #pragma multi_compile_fragment _ _FORWARD_PLUS");
                    sb.AppendLine("            #endif");
                }
                else
                {
                    sb.AppendLine("            #pragma multi_compile_fragment _ _FORWARD_PLUS");
                }
                sb.AppendLine("            #pragma multi_compile_fragment _ _LIGHT_LAYERS");
                sb.AppendLine("            #pragma multi_compile_fragment _ _LIGHT_COOKIES");
                sb.AppendLine("            #pragma multi_compile _ PROBE_VOLUMES_L1 PROBE_VOLUMES_L2");
            }
            // Built-In 3D lighting pragmas
            if (isBuiltIn && v.isLit)
            {
                sb.AppendLine("            #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight");
            }
            sb.AppendLine();

            sb.AppendLine($"            #define {v.pipeline} 1");
            EmitPipelineVersionDefines(sb, v);
            if (UsesCompactProperties(v))
                sb.AppendLine("            #define AIO_COMPACT_PROPERTIES 1");
            if (v.cutout)
                sb.AppendLine("            #define AIO_CUTOUT 1");
            sb.AppendLine();

            // Choose the keyword include from the active project keyword mode. This applies to both
            // lit and unlit variants, so MultiCompile / ShaderFeature / NoKeywords all work consistently.
            var keywordMode = AnimSpriteProjectSettings.Instance != null
                ? AnimSpriteProjectSettings.Instance.editorKeywordMode
                : EditorKeywordMode.MultiCompile;

            if (keywordMode == EditorKeywordMode.NoKeywords)
                sb.AppendLine($"            #include \"{IncludePath}AllInOneKeywordsForceDefine.hlsl\"");
            else
                sb.AppendLine($"            #include_with_pragmas \"{IncludePath}AllInOneKeywords.hlsl\"");
            sb.AppendLine();

            EmitIncludesAndEffects(sb, v, false);

            sb.AppendLine($"            {progEnd}");
            sb.AppendLine("        }");

            // URP 2D renderer requests this pass when normals are enabled on 2D lights.
            if (isURP && v.is2DLit)
            {
                EmitURP2DNormalsPass(sb, v, progStart, progEnd);
            }

            // Shadow caster pass
            if (v.needsShadowPass)
            {
                sb.AppendLine();
                sb.AppendLine("        Pass");
                sb.AppendLine("        {");
                sb.AppendLine("            Name \"ShadowCaster\"");
                sb.AppendLine("            Tags { \"LightMode\" = \"ShadowCaster\" }");
                sb.AppendLine("            ZWrite On ZTest LEqual Cull Off");
                sb.AppendLine();
                sb.AppendLine($"            {progStart}");
                if (isBuiltIn)
                {
                    sb.AppendLine("            #pragma vertex vertShadowBuiltIn");
                    sb.AppendLine("            #pragma fragment fragShadowBuiltIn");
                }
                else
                {
                    sb.AppendLine("            #pragma vertex vert");
                    sb.AppendLine("            #pragma fragment fragShadow");
                }
                sb.AppendLine("            #pragma multi_compile_shadowcaster");
                sb.AppendLine("            #pragma multi_compile_instancing");
                sb.AppendLine();
                sb.AppendLine($"            #define {v.pipeline} 1");
                EmitPipelineVersionDefines(sb, v);
                if (UsesCompactProperties(v))
                    sb.AppendLine("            #define AIO_COMPACT_PROPERTIES 1");
                if (v.cutout)
                    sb.AppendLine("            #define AIO_CUTOUT 1");
                // Shadow caster mirrors the same keyword strategy as the forward pass.
                if (keywordMode == EditorKeywordMode.NoKeywords)
                    sb.AppendLine($"            #include \"{IncludePath}AllInOneKeywordsForceDefine.hlsl\"");
                else
                    sb.AppendLine($"            #include_with_pragmas \"{IncludePath}AllInOneKeywords.hlsl\"");
                sb.AppendLine();

                EmitIncludesAndEffects(sb, v, true);

                sb.AppendLine($"            {progEnd}");
                sb.AppendLine("        }");
            }

            // Built-In additional lights pass (spotlights, point lights)
            if (isBuiltIn && v.isLit)
            {
                sb.AppendLine();
                sb.AppendLine("        Pass");
                sb.AppendLine("        {");
                sb.AppendLine("            Name \"ForwardAdd\"");
                sb.AppendLine("            Tags { \"LightMode\" = \"ForwardAdd\" }");
                sb.AppendLine("            Blend SrcAlpha One");
                sb.AppendLine("            ZWrite Off Cull Off");
                sb.AppendLine();
                sb.AppendLine($"            {progStart}");
                sb.AppendLine("            #pragma vertex vert");
                sb.AppendLine("            #pragma fragment frag");
                sb.AppendLine("            #pragma multi_compile_fwdadd");
                sb.AppendLine("            #pragma skip_variants POINT_COOKIE DIRECTIONAL_COOKIE");
                sb.AppendLine("            #pragma multi_compile_instancing");
                sb.AppendLine();
                sb.AppendLine($"            #define {v.pipeline} 1");
                EmitPipelineVersionDefines(sb, v);
                if (UsesCompactProperties(v))
                    sb.AppendLine("            #define AIO_COMPACT_PROPERTIES 1");
                if (v.cutout)
                    sb.AppendLine("            #define AIO_CUTOUT 1");
                // ForwardAdd mirrors the same keyword strategy as the forward pass.
                if (keywordMode == EditorKeywordMode.NoKeywords)
                    sb.AppendLine($"            #include \"{IncludePath}AllInOneKeywordsForceDefine.hlsl\"");
                else
                    sb.AppendLine($"            #include_with_pragmas \"{IncludePath}AllInOneKeywords.hlsl\"");
                sb.AppendLine();

                EmitIncludesAndEffects(sb, v, false);

                sb.AppendLine($"            {progEnd}");
                sb.AppendLine("        }");
            }

            sb.AppendLine("    }");
            sb.AppendLine("    Fallback \"Sprites/Default\"");
            sb.AppendLine("}");

            string path = ShaderDir + v.fileName + ".shader";
            File.WriteAllText(path, sb.ToString());
        }

        static void EmitURP2DNormalsPass(StringBuilder sb, VariantDef v, string progStart, string progEnd)
        {
            sb.AppendLine();
            sb.AppendLine("        Pass");
            sb.AppendLine("        {");
            sb.AppendLine("            Name \"NormalsRendering\"");
            sb.AppendLine("            Tags { \"LightMode\" = \"NormalsRendering\" }");
            sb.AppendLine("            Blend One Zero");
            sb.AppendLine("            ZWrite Off");
            sb.AppendLine("            Cull Off");
            sb.AppendLine();
            sb.AppendLine($"            {progStart}");
            sb.AppendLine("            #pragma vertex vert2DNormals");
            sb.AppendLine("            #pragma fragment frag2DNormals");
            sb.AppendLine("            #pragma multi_compile_instancing");
            sb.AppendLine();
            sb.AppendLine("            #define AIO_URP 1");
            EmitPipelineVersionDefines(sb, v);
            if (UsesCompactProperties(v))
                sb.AppendLine("            #define AIO_COMPACT_PROPERTIES 1");
            sb.AppendLine("            #define AIO_URP_2D_NORMALS_PASS 1");
            sb.AppendLine();
            sb.AppendLine("            #include \"Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl\"");
            sb.AppendLine($"            #include \"{IncludePath}AllInOneEffects.hlsl\"");
            sb.AppendLine();
            sb.AppendLine($"            {progEnd}");
            sb.AppendLine("        }");
        }

        static void EmitIncludesAndEffects(StringBuilder sb, VariantDef v, bool isShadowPass)
        {
            bool isURP = v.pipeline == "AIO_URP";
            bool isHDRP = v.pipeline == "AIO_HDRP";

            // Pipeline includes (directly, no __has_include)
            if (isURP)
            {
                if (v.is2DLit)
                {
                    int urpMajor = DetectURPMajorVersion();

                    bool useModern2DIncludes = UseModernURP2DIncludes();

                    sb.AppendLine("            // AIO_FIX_URP2D_INCLUDE_ORDER_V8");
                    sb.AppendLine("            #include \"Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl\"");

                    if (useModern2DIncludes)
                    {
                        sb.AppendLine("            // URP 17+ / Unity 6 path. Core2D exists here and prepares the 2D sprite helper structs.");
                        sb.AppendLine("            #include \"Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl\"");
                        sb.AppendLine("            #include \"Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/InputData2D.hlsl\"");
                        sb.AppendLine("            #include \"Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/SurfaceData2D.hlsl\"");
                        sb.AppendLine("            // CombinedShapeLightShared.hlsl includes LightingUtility.hlsl in this path, so manual SHAPE_LIGHT declarations are not emitted.");
                    }
                    else
                    {
                        sb.AppendLine("            // URP 12-16 / fallback path. Do not include Core2D.hlsl here; some URP package layouts do not expose it.");
                        sb.AppendLine("            #include \"Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/LightingUtility.hlsl\"");
                        sb.AppendLine("            #if USE_SHAPE_LIGHT_TYPE_0");
                        sb.AppendLine("            SHAPE_LIGHT(0)");
                        sb.AppendLine("            #endif");
                        sb.AppendLine("            #if USE_SHAPE_LIGHT_TYPE_1");
                        sb.AppendLine("            SHAPE_LIGHT(1)");
                        sb.AppendLine("            #endif");
                        sb.AppendLine("            #if USE_SHAPE_LIGHT_TYPE_2");
                        sb.AppendLine("            SHAPE_LIGHT(2)");
                        sb.AppendLine("            #endif");
                        sb.AppendLine("            #if USE_SHAPE_LIGHT_TYPE_3");
                        sb.AppendLine("            SHAPE_LIGHT(3)");
                        sb.AppendLine("            #endif");
                    }

                    sb.AppendLine("            #include \"Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/CombinedShapeLightShared.hlsl\"");
                    sb.AppendLine("            #define AIO_URP_2D_LIT 1");
                }
                else
                {
                    sb.AppendLine("            #include \"Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl\"");
                }

                if (v.isLit && !v.is2DLit)
                {
                    sb.AppendLine("            #include \"Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl\"");
                    // Unity 6 / URP 17: pull in GlobalIllumination for SAMPLE_GI and APV helpers.
                    // In older URP versions these symbols are provided by Lighting.hlsl directly, so
                    // this include is only needed (and only exists as a standalone file) on URP 17+.
                    sb.AppendLine("            #if UNITY_2023_3_OR_NEWER || UNITY_6000_0_OR_NEWER");
                    sb.AppendLine("                #include \"Packages/com.unity.render-pipelines.universal/ShaderLibrary/GlobalIllumination.hlsl\"");
                    sb.AppendLine("            #endif");
                    sb.AppendLine("            #define AIO_URP_3D_LIT 1");
                }

                if (v.isLit)
                    sb.AppendLine("            #define AIO_LIT 1");
            }
            else if (isHDRP)
            {
                sb.AppendLine("            #include \"Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl\"");
                sb.AppendLine("            #include \"Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl\"");

                if (v.isLit)
                {
                    sb.AppendLine("            #include \"Packages/com.unity.render-pipelines.core/ShaderLibrary/EntityLighting.hlsl\"");
                    sb.AppendLine("            #define AIO_HDRP_LIT 1");
                    sb.AppendLine("            #define AIO_LIT 1");
                }
            }
            else
            {
                // Built-In RP
                sb.AppendLine("            #include \"UnityCG.cginc\"");
                if (v.isLit)
                {
                    sb.AppendLine("            #include \"AutoLight.cginc\"");
                    sb.AppendLine("            #include \"Lighting.cginc\"");
                    sb.AppendLine("            #define AIO_LIT 1");
                }
            }

            sb.AppendLine();
            sb.AppendLine($"            #include \"{IncludePath}AllInOneEffects.hlsl\"");
        }

        static void EmitProperties(StringBuilder sb, VariantDef v, string srpProps)
        {
            sb.AppendLine("    Properties");
            sb.AppendLine("    {");
            sb.AppendLine("        [PerRendererData] _MainTex (\"Sprite Texture\", 2D) = \"white\" {}");
            sb.AppendLine("        _Color (\"Tint\", Color) = (1,1,1,1)");
            if (v.pipeline == "AIO_URP" && v.is2DLit)
            {
                sb.AppendLine("        [PerRendererData] _MaskTex (\"Mask\", 2D) = \"white\" {}");
                sb.AppendLine("        _NormalMap (\"Normal Map\", 2D) = \"bump\" {}");
                sb.AppendLine("        _NormalScale (\"Normal Scale\", Float) = 1");
            }
            sb.AppendLine("        [HideInInspector] _ShadowAlphaClip (\"Shadow Alpha Clip\", Float) = 0.1");

            // Stencil properties are declared on ALL variants — the Stencil { } block in the
            // SubShader references these by name. GUI variants additionally need _ColorMask for
            // Canvas UI; this is added below in the GUI-only block.
            sb.AppendLine("        [HideInInspector] _StencilComp (\"Stencil Comparison\", Float) = 8");
            sb.AppendLine("        [HideInInspector] _Stencil (\"Stencil ID\", Float) = 0");
            sb.AppendLine("        [HideInInspector] _StencilOp (\"Stencil Operation\", Float) = 0");
            sb.AppendLine("        [HideInInspector] _StencilWriteMask (\"Stencil Write Mask\", Float) = 255");
            sb.AppendLine("        [HideInInspector] _StencilReadMask (\"Stencil Read Mask\", Float) = 255");

            if (v.fileName.Contains("GUI"))
            {
                sb.AppendLine("        // Canvas UI-specific property");
                sb.AppendLine("        [HideInInspector] _ColorMask (\"Color Mask\", Float) = 15");
            }

            // Inject SRP Batcher properties for URP and HDRP
            if (v.pipeline == "AIO_URP" || v.pipeline == "AIO_HDRP")
            {
                sb.Append(srpProps);
            }

            sb.AppendLine("    }");
        }
    }
}
#endif