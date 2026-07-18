#if UNITY_EDITOR
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace FlexiSprite
{
    public static class AnimatedSpriteHLSLGenerator
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

    

        static EditorKeywordMode GetEditorKeywordMode()
        {
            var settings = AnimSpriteProjectSettings.Instance;
            if (settings == null) return EditorKeywordMode.MultiCompile;
            var fi = settings.GetType().GetField("editorKeywordMode");
            if (fi != null) return (EditorKeywordMode)fi.GetValue(settings);
            return EditorKeywordMode.MultiCompile;
        }


        public static void RegenerateWithMode(EditorKeywordMode mode)
        {
            _forcedMode = mode;
            _modeForced = true;
            GenerateEffectsHLSL();
            GenerateKeywordsHLSL();
            _modeForced = false;
        }

        static bool _modeForced;
        static EditorKeywordMode _forcedMode;

        static EditorKeywordMode ActiveMode => _modeForced ? _forcedMode : GetEditorKeywordMode();

        //   AIO_URP_MAJOR <n>    — e.g. 14, 15, 17 (Unity 6)
        //   AIO_URP_MINOR <n>
        //   AIO_URP_12_PLUS       — URP 12+ has SurfaceData2D / InputData2D structs
        //   AIO_URP_14_PLUS       — URP 14+ (Unity 2022.2+) has Forward+ / clustered light-loop support
        //   AIO_URP_17_PLUS       — URP 17+ (Unity 6) has APV (PROBE_VOLUMES_L1/L2, SAMPLE_GI macro)
        static (int major, int minor) DetectURPVersion()
        {
            int fallbackMajor = 0;
#if UNITY_6000_0_OR_NEWER
            fallbackMajor = 17;
#elif UNITY_2023_1_OR_NEWER
            fallbackMajor = 15;
#elif UNITY_2022_1_OR_NEWER
            fallbackMajor = 14;
#elif UNITY_2021_3_OR_NEWER
            fallbackMajor = 12;
#endif

            try
            {
                var pkg = UnityEditor.PackageManager.PackageInfo.FindForAssetPath(
                    "Packages/com.unity.render-pipelines.universal/package.json");
                if (pkg == null) return (fallbackMajor, 0);
                var parts = pkg.version.Split('.');
                int major = 0, minor = 0;
                if (parts.Length >= 1) int.TryParse(parts[0], out major);
                if (parts.Length >= 2) int.TryParse(parts[1], out minor);
                return major > 0 ? (major, minor) : (fallbackMajor, 0);
            }
            catch { return (fallbackMajor, 0); }
        }

        static (int major, int minor) DetectHDRPVersion()
        {
            try
            {
                var pkg = UnityEditor.PackageManager.PackageInfo.FindForAssetPath(
                    "Packages/com.unity.render-pipelines.high-definition/package.json");
                if (pkg == null) return (0, 0);
                var parts = pkg.version.Split('.');
                int major = 0, minor = 0;
                if (parts.Length >= 1) int.TryParse(parts[0], out major);
                if (parts.Length >= 2) int.TryParse(parts[1], out minor);
                return (major, minor);
            }
            catch { return (0, 0); }
        }

        /// <summary>
        /// URP defines (tracks the URP package version, not the Unity editor version):
        ///   AIO_URP_MAJOR/MINOR — raw version numbers
        ///   AIO_URP_12_PLUS — SurfaceData2D/InputData2D structs available (fixes 2D Lit type errors)
        ///   AIO_URP_14_PLUS — Forward+ / clustered light-loop path (LIGHT_LOOP_BEGIN macro)
        ///   AIO_URP_17_PLUS — APV / Adaptive Probe Volumes (PROBE_VOLUMES_L1/L2, SAMPLE_GI macro)
        ///
        /// HDRP defines (tracks HDRP package version):
        ///   AIO_HDRP_MAJOR/MINOR
        ///   AIO_HDRP_14_PLUS — HDRP 14 (Unity 2022.2+); LightData/DirectionalLightData field layout we target
        ///   AIO_HDRP_17_PLUS — HDRP 17 (Unity 6); APV + volumetric changes
        ///
        /// Built-In has no package — We emit AIO_BIRP_UNITY_6 gated
        /// on UNITY_6000_0_OR_NEWER for the handful of Built-In code paths that changed in Unity 6
        /// (e.g. shadow coord macros).
        /// </summary>
 

        static void EmitVersionDefines(StringBuilder sb)
        {

            var (urpMajor, urpMinor) = DetectURPVersion();
            if (urpMajor > 0)
            {
                sb.AppendLine("#ifndef AIO_URP_MAJOR");
                sb.AppendLine($"#define AIO_URP_MAJOR {urpMajor}");
                sb.AppendLine("#endif");
                sb.AppendLine("#ifndef AIO_URP_MINOR");
                sb.AppendLine($"#define AIO_URP_MINOR {urpMinor}");
                sb.AppendLine("#endif");
                if (urpMajor >= 12)
                {
                    sb.AppendLine("#ifndef AIO_URP_12_PLUS");
                    sb.AppendLine("#define AIO_URP_12_PLUS 1");
                    sb.AppendLine("#endif");
                }
                if (urpMajor >= 14)
                {
                    sb.AppendLine("#ifndef AIO_URP_14_PLUS");
                    sb.AppendLine("#define AIO_URP_14_PLUS 1");
                    sb.AppendLine("#endif");
                }
                if (urpMajor >= 17)
                {
                    sb.AppendLine("#ifndef AIO_URP_17_PLUS");
                    sb.AppendLine("#define AIO_URP_17_PLUS 1");
                    sb.AppendLine("#endif");
                }
            }
            var (hdrpMajor, hdrpMinor) = DetectHDRPVersion();
            if (hdrpMajor > 0)
            {
                sb.AppendLine("#ifndef AIO_HDRP_MAJOR");
                sb.AppendLine($"#define AIO_HDRP_MAJOR {hdrpMajor}");
                sb.AppendLine("#endif");
                sb.AppendLine("#ifndef AIO_HDRP_MINOR");
                sb.AppendLine($"#define AIO_HDRP_MINOR {hdrpMinor}");
                sb.AppendLine("#endif");
                if (hdrpMajor >= 14)
                {
                    sb.AppendLine("#ifndef AIO_HDRP_14_PLUS");
                    sb.AppendLine("#define AIO_HDRP_14_PLUS 1");
                    sb.AppendLine("#endif");
                }
                if (hdrpMajor >= 17)
                {
                    sb.AppendLine("#ifndef AIO_HDRP_17_PLUS");
                    sb.AppendLine("#define AIO_HDRP_17_PLUS 1");
                    sb.AppendLine("#endif");
                }
            }
            sb.AppendLine();
        }


        public static void GenerateEffectsHLSL()
        {
            var settings = AnimSpriteProjectSettings.Instance;
            if (settings) { settings.EnsureGroups(); settings.ApplyOrderToRegistry(); }

            var mode = ActiveMode;

            // Step 1: Extract effect bodies from existing source files
            var bodies = ExtractAllEffectBodies();
            Debug.Log($"[HLSL Gen] Extracted {bodies.Count} effect implementations from source files (mode: {mode})");

            // Step 2: Generate the output
            var sb = new StringBuilder();
            var groupOrder = settings != null ? settings.GetGroupsInRenderOrder() : null;

            EmitHeader(sb);
            EmitUtilities(sb, true);

            // Get ordered group list
            EffectGroup[] groups;
            if (groupOrder != null)
                groups = groupOrder.Select(g => g.group).ToArray();
            else
                groups = new[] { EffectGroup.UV, EffectGroup.Filter, EffectGroup.Color, EffectGroup.ColorExt,
                    EffectGroup.Edge, EffectGroup.Dissolve, EffectGroup.VFX, EffectGroup.Game };

            bool useIfDefs = true;

            foreach (var group in groups)
            {
                var effects = settings != null
                    ? settings.GetOrderedEffects(group)
                    : EffectRegistry.GetByGroup(group);

                if (effects.Count == 0) continue;
                EmitGroup(sb, group, effects, bodies, useIfDefs);
            }

            EmitCompose(sb, groups, useIfDefs);
            EmitVertFrag(sb);
            sb.AppendLine("#endif // ALLINONE_EFFECTS_INCLUDED");

            // Write
            string outPath = ShaderDir + "Include/AllInOneEffects.hlsl";
            var dir = Path.GetDirectoryName(outPath);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            File.WriteAllText(outPath, sb.ToString());

            // Stats
            int totalFx = 0;
            foreach (var g in groups) totalFx += EffectRegistry.GetByGroup(g).Count;
            int matched = bodies.Count;
            Debug.Log($"[HLSL Gen] Generated AllInOneEffects.hlsl: {totalFx} effects ({matched} with HLSL) in {groups.Length} groups");

            foreach (var def in EffectRegistry.All)
            {
                if (!bodies.ContainsKey(def.enableShader) && !def.id.ToString().StartsWith("_"))
                    Debug.LogWarning($"[HLSL Gen] No HLSL body for: {def.id} ({def.enableShader})");
            }

            AssetDatabase.Refresh();
        }

  
        public static void GenerateKeywordsHLSL()
        {
            var settings = AnimSpriteProjectSettings.Instance;
            var groups = settings != null ? settings.GetGroupsInRenderOrder() : null;
            var mode = ActiveMode;

            var sb = new StringBuilder();
            sb.AppendLine("#ifndef ALLINONE_KEYWORDS_INCLUDED");
            sb.AppendLine("#define ALLINONE_KEYWORDS_INCLUDED");

            if (mode == EditorKeywordMode.NoKeywords)
            {
                sb.AppendLine("// EditorKeywordMode: NoKeywords — all groups always active (uber-shader)");
                sb.AppendLine("// All keywords force-defined. No #pragma = zero variant compilation.");
                if (groups != null)
                    foreach (var g in groups)
                        sb.AppendLine($"#define {EffectGroupInfo.GetKeyword(g.group)} 1");
                else
                    for (int i = 0; i < (int)EffectGroup._Count; i++)
                        sb.AppendLine($"#define {EffectGroupInfo.Keywords[i]} 1");
            }
            else
            {
                string pragma = mode == EditorKeywordMode.ShaderFeature
                    ? "shader_feature_local"
                    : "multi_compile_local";

                sb.AppendLine($"// EditorKeywordMode: {mode} — using #pragma {pragma}");
                sb.AppendLine($"// MUST use: #include_with_pragmas \"AllInOneKeywords.hlsl\"");

                if (groups != null)
                    foreach (var g in groups)
                        sb.AppendLine($"#pragma {pragma} _ {EffectGroupInfo.GetKeyword(g.group)}");
                else
                    for (int i = 0; i < (int)EffectGroup._Count; i++)
                        sb.AppendLine($"#pragma {pragma} _ {EffectGroupInfo.Keywords[i]}");
            }

            sb.AppendLine("#endif");

            string outPath = ShaderDir + "Include/AllInOneKeywords.hlsl";
            var dir = Path.GetDirectoryName(outPath);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            File.WriteAllText(outPath, sb.ToString());

       
            var sbForce = new StringBuilder();
            sbForce.AppendLine("#ifndef ALLINONE_KEYWORDS_INCLUDED");
            sbForce.AppendLine("#define ALLINONE_KEYWORDS_INCLUDED");
            if (groups != null)
                foreach (var g in groups)
                    sbForce.AppendLine($"#define {EffectGroupInfo.GetKeyword(g.group)} 1");
            else
                for (int i = 0; i < (int)EffectGroup._Count; i++)
                    sbForce.AppendLine($"#define {EffectGroupInfo.Keywords[i]} 1");
            sbForce.AppendLine("#endif");
            File.WriteAllText(ShaderDir + "Include/AllInOneKeywordsForceDefine.hlsl", sbForce.ToString());

            AssetDatabase.Refresh();
            Debug.Log($"[HLSL Gen] Generated AllInOneKeywords.hlsl (mode: {mode}) and AllInOneKeywordsForceDefine.hlsl (uber)");
        }

     

        static Dictionary<string, string> ExtractAllEffectBodies()
        {
            var bodies = new Dictionary<string, string>();

            foreach (var effect in AllInOneEffectLibrary.Effects)
            {
                string cleanBody = effect.Body.Trim();
                if (!bodies.ContainsKey(effect.Keyword))
                {
                    bodies[effect.Keyword] = cleanBody;
                }
                else
                {
                    UnityEngine.Debug.LogWarning($"[Shader Generator] Duplicate effect found for {effect.Keyword}. Ignoring duplicates.");
                }
            }

            return bodies;
        }

        static void EmitHeader(StringBuilder sb)
        {
            sb.AppendLine("#ifndef ALLINONE_EFFECTS_INCLUDED");
            sb.AppendLine("#define ALLINONE_EFFECTS_INCLUDED");
            sb.AppendLine("// Generated by AnimatedSpriteHLSLGenerator");
            EmitVersionDefines(sb);
            sb.AppendLine("#if defined(AIO_COMPACT_PROPERTIES) || defined(AIO_BUILTIN_COMPACT_PROPERTIES)");
            sb.AppendLine("    #include \"AllInOneProperties_Compact.hlsl\"");
            sb.AppendLine("#else");
            sb.AppendLine("    #include \"AllInOneProperties.hlsl\"");
            sb.AppendLine("#endif");
            sb.AppendLine();
            sb.AppendLine("#if defined(AIO_URP_2D_LIT) || defined(AIO_URP_2D_NORMALS_PASS)");
            sb.AppendLine("TEXTURE2D(_MaskTex); SAMPLER(sampler_MaskTex);");
            sb.AppendLine("TEXTURE2D(_NormalMap); SAMPLER(sampler_NormalMap);");
            sb.AppendLine("float _NormalScale;");
            sb.AppendLine("#endif");
            sb.AppendLine();
        }

        static void EmitGroup(StringBuilder sb, EffectGroup group, List<EffectDef> effects, Dictionary<string, string> bodies, bool useIfDefs)
        {
            string kw = EffectGroupInfo.GetKeyword(group);
            string display = EffectGroupInfo.GetDisplayName(group);
            bool isUV = group == EffectGroup.UV;

            sb.AppendLine($"// \u2550\u2550\u2550 {display} ({effects.Count} effects) \u2550\u2550\u2550");
            if (useIfDefs) sb.AppendLine($"#ifdef {kw}");
            sb.AppendLine();

            // Effect implementations
            foreach (var def in effects)
            {
                if (!bodies.TryGetValue(def.enableShader, out string body)) continue;

                sb.AppendLine($"// [{def.id}] {def.name}");
                if (isUV)
                {
                    sb.AppendLine($"float2 AIO_{def.id}(float2 uv, half time, float3 worldPos)");
                    sb.AppendLine("{");
                    sb.AppendLine(body);
                    if (!body.Contains("return uv")) sb.AppendLine("    return uv;");
                    sb.AppendLine("}");
                }
                else
                {
                    sb.AppendLine(
                        $"void AIO_{def.id}(inout half4 col, float2 uv, half time, float3 worldPos, float4 screenPos, half4 masks, half eMask)");
                    sb.AppendLine("{");
                    sb.AppendLine(body);
                    sb.AppendLine("}");
                }
                sb.AppendLine();
            }

            // Dispatch
            sb.AppendLine($"// ---- Dispatch ----");
            if (isUV)
            {
                sb.AppendLine($"float2 ApplyGroup_{group}(float2 uv, half time, float3 worldPos, half4 masks, half eMask, inout half uvAlpha)");
                sb.AppendLine("{");
                foreach (var def in effects)
                {
                    if (!bodies.ContainsKey(def.enableShader)) continue;

                    if (def.id == EffectId.UVScaleOffset)
                    {
                        sb.AppendLine(
                            $"    if ({def.enableShader} > 0.5) {{ float2 _pre = uv; float2 _next = AIO_{def.id}(uv, time, worldPos); half _m = eMask * SampleEffectMask(_pre, masks, {def.maskShader}); float2 _mixed = lerp(_pre, _next, _m); uvAlpha *= AtlasRectMask(_mixed); uv = ClampToAtlas(_mixed); }}");
                    }
                    else
                    {
                        sb.AppendLine(
                            $"    if ({def.enableShader} > 0.5) {{ float2 _pre = uv; uv = AIO_{def.id}(uv, time, worldPos); half _m = eMask * SampleEffectMask(_pre, masks, {def.maskShader}); uv = lerp(_pre, uv, _m); uv = ClampToAtlas(uv); }}");
                    }
                }
                sb.AppendLine("    return uv;");
                sb.AppendLine("}");
            }
            else
            {
                sb.AppendLine(
                    $"void ApplyGroup_{group}(inout half4 col, float2 uv, half time, float3 worldPos, float4 screenPos, half4 masks, half eMask)");
                sb.AppendLine("{");
                foreach (var def in effects)
                    if (bodies.ContainsKey(def.enableShader))
                        sb.AppendLine($"    if ({def.enableShader} > 0.5) AIO_{def.id}(col, uv, time, worldPos, screenPos, masks, eMask);");
                sb.AppendLine("}");
            }

            sb.AppendLine();
            if (useIfDefs) sb.AppendLine($"#endif // {kw}");
            sb.AppendLine();
        }

        static void EmitCompose(StringBuilder sb, EffectGroup[] groups, bool useIfDefs)
        {
            sb.AppendLine("// \u2550\u2550\u2550 COMPOSE \u2550\u2550\u2550");
            sb.AppendLine("half4 ComposeAllEffects(float2 uv, float3 worldPos, float4 screenPos)");
            sb.AppendLine("{");
            sb.AppendLine("    half time = GetTime();");
            sb.AppendLine("    if (_SpriteFlipX > 0.5) uv.x = _MinXUV + _MaxXUV - uv.x;");
            sb.AppendLine("    if (_SpriteFlipY > 0.5) uv.y = _MinYUV + _MaxYUV - uv.y;");
            sb.AppendLine();
            sb.AppendLine("    half4 masks = GetEffectMasks(uv);");
            sb.AppendLine("    half eMask = GetGlobalMask(uv) * _GlobalEffectFade;");
            sb.AppendLine("    half uvAlpha = 1.0;");

            if (groups.Contains(EffectGroup.UV))
            {
                if (useIfDefs) sb.AppendLine("#ifdef _FX_UV");
                sb.AppendLine("    uv = ApplyGroup_UV(uv, time, worldPos, masks, eMask, uvAlpha);");
                if (useIfDefs) sb.AppendLine("#endif");
            }

            sb.AppendLine("    if (_PixelArtSnap_En > 0.5) uv = PixelSnap(uv, _PixelSnapResolution);");
            sb.AppendLine("    uv = ClampToAtlas(uv);");
            sb.AppendLine();
            sb.AppendLine("    // Re-sample masks after UV effects. The first mask sample is only for masking UV transforms themselves.");
            sb.AppendLine("    // Later effects must use the transformed UV so masks travel with rotate/scale/distort effects.");
            sb.AppendLine("    masks = GetEffectMasks(uv);");
            sb.AppendLine("    eMask = GetGlobalMask(uv) * _GlobalEffectFade;");
            sb.AppendLine();
            sb.AppendLine("    half4 col = STEX(_MainTex, sampler_MainTex, uv);");
            sb.AppendLine("    col.rgb *= smoothstep(0, 0.1, col.a);");
            sb.AppendLine("    if (_AlphaCutoff_En > 0.5 && _AlphaCutoff > 0.001) { col.a = step(_AlphaCutoff, col.a); col.rgb *= col.a; }");

            if (groups.Contains(EffectGroup.Filter))
            {
                if (useIfDefs) sb.AppendLine("#ifdef _FX_FILTER");
                sb.AppendLine("    ApplyGroup_Filter(col, uv, time, worldPos, screenPos, masks, eMask);");
                if (useIfDefs) sb.AppendLine("#endif");
            }

            sb.AppendLine("    if (_PremulAlpha > 0.5 && col.a > 0.001) col.rgb /= col.a;");
            sb.AppendLine();
            sb.AppendLine("    #ifdef _FX_EDGE");
            sb.AppendLine("    half4 ogCol = col;");
            sb.AppendLine("    if (_OutlineEffectsOnly_En > 0.5) {");
            sb.AppendLine("        half width = max(_OutlineEffectWidth, 1e-5);");
            sb.AppendLine("        half ringA = SampleOutlineAlpha(uv, width);");
            sb.AppendLine("        half3 outlineRGB = _OutlineColor.rgb;");
            sb.AppendLine("        col.rgb = lerp(outlineRGB, col.rgb, step(0.01, col.a));");
            sb.AppendLine("        col.a = ringA - col.a;");
            sb.AppendLine("    }");
            sb.AppendLine("    #endif");

            foreach (var group in groups)
            {
                if (group == EffectGroup.UV || group == EffectGroup.Filter) continue;
                string kw = EffectGroupInfo.GetKeyword(group);
                if (useIfDefs) sb.AppendLine($"#ifdef {kw}");
                sb.AppendLine($"    ApplyGroup_{group}(col, uv, time, worldPos, screenPos, masks, eMask);");
                if (useIfDefs) sb.AppendLine($"#endif");
            }
            sb.AppendLine("    #ifdef _FX_EDGE");
            sb.AppendLine("    if (_OutlineEffectsOnly_En > 0.5){");
            sb.AppendLine("    col.a = max(col.a, ogCol.a);");
            sb.AppendLine("    col.rgb = col.rgb * (1.0 - ogCol.a) + ogCol.rgb * ogCol.a;}");
            sb.AppendLine("    #endif");
            sb.AppendLine("    col *= uvAlpha;");
            sb.AppendLine("    if (_PremulAlpha > 0.5) col.rgb *= col.a;");
            sb.AppendLine("    return col;");
            sb.AppendLine("}");
            sb.AppendLine();
        }

        static void EmitVertFrag(StringBuilder sb)
        {
            sb.AppendLine("// ═══ VERT / FRAG ═══");
            sb.AppendLine();
            sb.AppendLine("// URP main-light wrapper. Version gates use Unity's documented UNITY_XXXX_X_OR_NEWER");
            sb.AppendLine("// macros so package-version mismatches with the Unity editor version don't silently");
            sb.AppendLine("// pick the wrong GetMainLight signature.");
            sb.AppendLine("#if defined(AIO_URP_3D_LIT)");
            sb.AppendLine("    Light AIO_GetMainLight(float3 posWS, float4 sc) {");
            sb.AppendLine("        #if defined(AIO_URP_14_PLUS) // URP 14+, Forward+ signature");
            sb.AppendLine("            half4 shadowMask = half4(1,1,1,1);");
            sb.AppendLine("            return GetMainLight(sc, posWS, shadowMask);");
            sb.AppendLine("        #elif defined(AIO_URP_12_PLUS) // URP 12-13");
            sb.AppendLine("            return GetMainLight(sc);");
            sb.AppendLine("        #else // URP 10-11 and older");
            sb.AppendLine("            return GetMainLight();");
            sb.AppendLine("        #endif");
            sb.AppendLine("    }");
            sb.AppendLine("#endif");
            sb.AppendLine();
            sb.AppendLine(
                "struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; half4 color : COLOR; float3 normal : NORMAL; UNITY_VERTEX_INPUT_INSTANCE_ID };");
            sb.AppendLine("struct v2f {");
            sb.AppendLine("    float4 pos : SV_POSITION; float2 uv : TEXCOORD0; half4 color : COLOR;");
            sb.AppendLine("    float3 worldPos : TEXCOORD1; float3 normalWS : TEXCOORD2; float4 screenPos : TEXCOORD3;");
            sb.AppendLine("#if defined(AIO_URP) && defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)");
            sb.AppendLine("    float4 shadowCoord : TEXCOORD4;");
            sb.AppendLine("#elif defined(AIO_BUILTIN) && (defined(UNITY_PASS_FORWARDBASE) || defined(UNITY_PASS_FORWARDADD))");
            sb.AppendLine("    UNITY_LIGHTING_COORDS(4, 5)");
            sb.AppendLine("#endif");
            sb.AppendLine("    UNITY_VERTEX_OUTPUT_STEREO");
            sb.AppendLine("};");
            sb.AppendLine();
            sb.AppendLine("v2f vert(appdata v) {");
            sb.AppendLine("    v2f o = (v2f)0; UNITY_SETUP_INSTANCE_ID(v); UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);");
            sb.AppendLine("    if (_VertexUVExpand > 0.001) {");
            sb.AppendLine("        // UV stays unchanged — the atlas region sampled does not shrink.");
            sb.AppendLine("        // Vertices expand outward from mesh origin so outline/glow/shadow effects");
            sb.AppendLine("        // have extra room to render beyond the original sprite bounds.");
            sb.AppendLine("        float expand = _VertexUVExpand;");
            sb.AppendLine("        v.vertex.xy *= (1.0 + expand * 2.0);");
            sb.AppendLine("    }");
            sb.AppendLine("    o.pos = AIO_CLIP_POS(v.vertex); o.uv = v.uv; o.color = v.color;");
            sb.AppendLine("    o.worldPos = AIO_WORLD_POS(v.vertex);");
            sb.AppendLine("    // Lit sprites are flat cards. Use the mesh normal so directional lighting");
            sb.AppendLine("    // and received shadows are uniform across the whole sprite surface.");
            sb.AppendLine("    float3 localN = dot(v.normal, v.normal) > 0.0001 ? normalize(v.normal) : float3(0, 0, -1);");
            sb.AppendLine("#if defined(AIO_BUILTIN)");
            sb.AppendLine("    o.normalWS = normalize(UnityObjectToWorldNormal(localN));");
            sb.AppendLine("#elif defined(AIO_URP) || defined(AIO_HDRP)");
            sb.AppendLine("    o.normalWS = normalize(TransformObjectToWorldNormal(localN));");
            sb.AppendLine("#else");
            sb.AppendLine("    o.normalWS = normalize(mul((float3x3)AIO_OBJ_TO_WORLD, localN));");
            sb.AppendLine("#endif");
            sb.AppendLine("    o.screenPos = AIO_ComputeScreenPos(o.pos);");
            sb.AppendLine("#if defined(AIO_URP) && defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)");
            sb.AppendLine("    #if defined(_MAIN_LIGHT_SHADOWS_SCREEN) && !defined(_SURFACE_TYPE_TRANSPARENT)");
            sb.AppendLine("        o.shadowCoord = AIO_ComputeScreenPos(o.pos);");
            sb.AppendLine("    #else");
            sb.AppendLine("        o.shadowCoord = TransformWorldToShadowCoord(o.worldPos);");
            sb.AppendLine("    #endif");
            sb.AppendLine("#elif defined(AIO_BUILTIN)");
            sb.AppendLine("    #if defined(UNITY_PASS_FORWARDBASE) || defined(UNITY_PASS_FORWARDADD)");
            sb.AppendLine("        UNITY_TRANSFER_LIGHTING(o, v.uv);");
            sb.AppendLine("    #endif");
            sb.AppendLine("    #if defined(PIXELSNAP_ON)");
            sb.AppendLine("        o.pos = UnityPixelSnap(o.pos);");
            sb.AppendLine("    #endif");
            sb.AppendLine("#endif");
            sb.AppendLine("    return o;");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("half4 frag(v2f i, bool isFrontFace : SV_IsFrontFace) : SV_Target {");
            sb.AppendLine("    half4 col = ComposeAllEffects(i.uv, i.worldPos, i.screenPos); col *= i.color;");
            sb.AppendLine("#if defined(AIO_CUTOUT)");
            sb.AppendLine("    clip(col.a - _ShadowAlphaClip);");
            sb.AppendLine("#endif");
            sb.AppendLine();
            sb.AppendLine("    // Two-sided lighting: flip normal for back faces so the 'other side' of the sprite");
            sb.AppendLine("    // still receives light. Gated by _FlipNormals; when off behaves as a flat sprite.");
            sb.AppendLine("    half3 N = AIO_GetFaceNormal(i.normalWS, isFrontFace);");
            sb.AppendLine();
            sb.AppendLine("#if defined(AIO_URP_2D_LIT)");
            sb.AppendLine(
                "    #if defined(USE_SHAPE_LIGHT_TYPE_0) || defined(USE_SHAPE_LIGHT_TYPE_1) || defined(USE_SHAPE_LIGHT_TYPE_2) || defined(USE_SHAPE_LIGHT_TYPE_3)");
            sb.AppendLine("        half2 lightingUV = i.screenPos.xy / max(i.screenPos.w, 0.001);");
            sb.AppendLine("        half4 lightMask = STEX(_MaskTex, sampler_MaskTex, i.uv);");
            sb.AppendLine("        half4 colBase = col;");
            sb.AppendLine("        // URP 12+ introduced SurfaceData2D/InputData2D structs replacing the old");
            sb.AppendLine("        // CombinedShapeLightShared(color, mask, uv) signature. Gate on the package-version");
            sb.AppendLine("        // define (emitted by the C# generator from PackageInfo) so shaders match the");
            sb.AppendLine("        // actually installed URP version — not the editor version. This fixes the");
            sb.AppendLine("        // 'cannot implicitly convert from half4 to struct SurfaceData2D' error when");
            sb.AppendLine("        // package version and editor version disagree.");
            sb.AppendLine("        #if defined(AIO_URP_12_PLUS)");
            sb.AppendLine("            SurfaceData2D surfaceData; InputData2D inputData;");
            sb.AppendLine("            InitializeSurfaceData(col.rgb, col.a, lightMask, surfaceData);");
            sb.AppendLine("            InitializeInputData(i.uv, lightingUV, inputData);");
            sb.AppendLine("            col = CombinedShapeLightShared(surfaceData, inputData);");
            sb.AppendLine("        #else");
            sb.AppendLine("            col = CombinedShapeLightShared(col, lightMask, lightingUV);");
            sb.AppendLine("        #endif");
            sb.AppendLine("        // Project-wide direct-light multiplier. Lerp back toward unlit color when intensity != 1.");
            sb.AppendLine("        half3 litDelta = col.rgb - colBase.rgb;");
            sb.AppendLine("        col.rgb = colBase.rgb + litDelta * _GlobalLightIntensity;");
            sb.AppendLine("    #endif");
            sb.AppendLine("#endif");
            sb.AppendLine();
            sb.AppendLine("#if defined(AIO_URP_3D_LIT)");
            sb.AppendLine("    {");
            sb.AppendLine("        // Ambient / indirect from SH probes.");
            sb.AppendLine("        //   - Unity 6 + URP 17: SAMPLE_GI also reads Adaptive Probe Volumes (APV) when");
            sb.AppendLine("        //     a volume is present. SampleSH alone ignores APV and would render dark");
            sb.AppendLine("        //     in APV-only lit scenes.");
            sb.AppendLine("        //   - Unity 2021.3 — 2022.x: SampleSH(N) is the classic path (no APV available).");
            sb.AppendLine("        half3 ambient;");
            sb.AppendLine("        #if defined(AIO_URP_17_PLUS)");
            sb.AppendLine("            // URP 17+ changes SAMPLE_GI arity depending on lightmap/APV keywords.");
            sb.AppendLine("            // SampleProbeSHVertex keeps one stable signature and still routes to APV when enabled.");
            sb.AppendLine("            half3 bakedGI = SampleProbeSHVertex(i.worldPos, N, normalize(_WorldSpaceCameraPos - i.worldPos));");
            sb.AppendLine("            ambient = bakedGI * _AmbientLightIntensity;");
            sb.AppendLine("        #else");
            sb.AppendLine("            // URP 12-16: classic SH-only path (no APV).");
            sb.AppendLine("            ambient = SampleSH(N) * _AmbientLightIntensity;");
            sb.AppendLine("        #endif");
            sb.AppendLine();
            sb.AppendLine("        // Build shadow coord. URP versions differ in whether it's interpolated (REQUIRES_...) or");
            sb.AppendLine("        // recomputed per-pixel. The compile-time keyword is identical across URP 11-17.");
            sb.AppendLine("        float4 sc = float4(0,0,0,0);");
            sb.AppendLine(
                "        #if defined(_MAIN_LIGHT_SHADOWS) || defined(_MAIN_LIGHT_SHADOWS_CASCADE) || defined(_MAIN_LIGHT_SHADOWS_SCREEN)");
            sb.AppendLine("            #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)");
            sb.AppendLine("                sc = i.shadowCoord;");
            sb.AppendLine("            #else");
            sb.AppendLine("                sc = TransformWorldToShadowCoord(i.worldPos);");
            sb.AppendLine("            #endif");
            sb.AppendLine("        #endif");
            sb.AppendLine("        if (_ReceiveShadows < 0.5) sc = float4(0,0,0,0);");
            sb.AppendLine();
            sb.AppendLine("        // Main directional light — diffused by N.L.");
            sb.AppendLine("        Light mainLight = AIO_GetMainLight(i.worldPos, sc);");
            sb.AppendLine("        half mainNdL = saturate(dot(N, mainLight.direction));");
            sb.AppendLine("        half3 direct = mainLight.color * (mainLight.distanceAttenuation * mainLight.shadowAttenuation * mainNdL);");
            sb.AppendLine();
            sb.AppendLine("        // Additional lights:");
            sb.AppendLine("        //   Forward+ (URP 14+): LIGHT_LOOP_BEGIN/END macros with tile-index iteration");
            sb.AppendLine("        //   Classic (URP 10-17 with _ADDITIONAL_LIGHTS): flat count loop");
            sb.AppendLine("        #if (defined(_FORWARD_PLUS) || defined(_CLUSTER_LIGHT_LOOP)) && defined(AIO_URP_14_PLUS)");
            sb.AppendLine("            uint pixelLightCount = GetAdditionalLightsCount();");
            sb.AppendLine("            InputData inputData = (InputData)0;");
            sb.AppendLine("            inputData.positionWS = i.worldPos;");
            sb.AppendLine("            inputData.normalWS = N;");
            sb.AppendLine("            inputData.viewDirectionWS = GetWorldSpaceNormalizeViewDir(i.worldPos);");
            sb.AppendLine("            inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(i.pos);");
            sb.AppendLine("            inputData.shadowCoord = sc;");
            sb.AppendLine("            LIGHT_LOOP_BEGIN(pixelLightCount)");
            sb.AppendLine("                Light addLight = GetAdditionalLight(lightIndex, i.worldPos);");
            sb.AppendLine("                half NdL = saturate(dot(N, addLight.direction));");
            sb.AppendLine("                direct += addLight.color * (addLight.distanceAttenuation * addLight.shadowAttenuation * NdL);");
            sb.AppendLine("            LIGHT_LOOP_END");
            sb.AppendLine("        #elif defined(_ADDITIONAL_LIGHTS)");
            sb.AppendLine("            uint addCount = GetAdditionalLightsCount();");
            sb.AppendLine("            for (uint li = 0u; li < addCount; li++) {");
            sb.AppendLine("                Light addLight = GetAdditionalLight(li, i.worldPos);");
            sb.AppendLine("                half NdL = saturate(dot(N, addLight.direction));");
            sb.AppendLine("                direct += addLight.color * (addLight.distanceAttenuation * addLight.shadowAttenuation * NdL);");
            sb.AppendLine("            }");
            sb.AppendLine("        #endif");
            sb.AppendLine();
            sb.AppendLine("        col.rgb *= ambient + direct * _GlobalLightIntensity;");
            sb.AppendLine("    }");
            sb.AppendLine("#endif");
            sb.AppendLine();
            sb.AppendLine("#if defined(AIO_BUILTIN) && defined(UNITY_PASS_FORWARDBASE)");
            sb.AppendLine("    {");
            sb.AppendLine("        half3 ambient = ShadeSH9(half4(N, 1)) * _AmbientLightIntensity;");
            sb.AppendLine("        half3 lightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));");
            sb.AppendLine("        half NdL = saturate(dot(N, lightDir));");
            sb.AppendLine("        UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);");
            sb.AppendLine("        atten = (_ReceiveShadows > 0.5) ? atten : 1.0;");
            sb.AppendLine("        half3 direct = _LightColor0.rgb * NdL * atten * _GlobalLightIntensity;");
            sb.AppendLine("        col.rgb *= ambient + direct;");
            sb.AppendLine("    }");
            sb.AppendLine("#endif");
            sb.AppendLine("#if defined(AIO_BUILTIN) && defined(UNITY_PASS_FORWARDADD)");
            sb.AppendLine("    {");
            sb.AppendLine("        half3 lightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));");
            sb.AppendLine("        half NdL = saturate(dot(N, lightDir));");
            sb.AppendLine("        UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);");
            sb.AppendLine("        atten = (_ReceiveShadows > 0.5) ? atten : 1.0;");
            sb.AppendLine("        col.rgb *= _LightColor0.rgb * NdL * atten * _GlobalLightIntensity;");
            sb.AppendLine("    }");
            sb.AppendLine("#endif");
            sb.AppendLine();
            sb.AppendLine("    // ---- HDRP Lit ----");
            sb.AppendLine("    // KNOWN ISSUE (Unity 6): HDRP's full LightLoop is version-specific and deeply tied to PBR");
            sb.AppendLine("    // surfaces. Directional + point lights from HDRP do NOT automatically reach this shader;");
            sb.AppendLine("    // only ambient/SH (baked probes / sky) contributes by default. Unity 2021 HDRP had a");
            sb.AppendLine("    // similar limitation. This produces the 'GI works, direct lights don't' behaviour.");
            sb.AppendLine("    //");
            sb.AppendLine("    // Opt-in manual path: define AIO_HDRP_MANUAL_LIGHTS in your HDRP Lit .shader file to");
            sb.AppendLine("    // iterate the exposed _DirectionalLightDatas / _LightDatas buffers directly. This is a");
            sb.AppendLine("    // best-effort brute-force loop (no tile culling) and may need adjustment per HDRP version.");
            sb.AppendLine("    // Test before relying on it -structure fields like DirectionalLightData.forward and");
            sb.AppendLine("    // LightData.positionRWS have changed between HDRP versions.");
            sb.AppendLine("#if defined(AIO_HDRP_LIT)");
            sb.AppendLine("    {");
            sb.AppendLine("        // --- Ambient from SH coefficients (environment/baked probes) — this always works ---");
            sb.AppendLine("        real4 SHCoefficients[7];");
            sb.AppendLine("        SHCoefficients[0] = unity_SHAr; SHCoefficients[1] = unity_SHAg;");
            sb.AppendLine("        SHCoefficients[2] = unity_SHAb; SHCoefficients[3] = unity_SHBr;");
            sb.AppendLine("        SHCoefficients[4] = unity_SHBg; SHCoefficients[5] = unity_SHBb;");
            sb.AppendLine("        SHCoefficients[6] = unity_SHC;");
            sb.AppendLine("        // HDRP SH9 returns Illuminance (Lux). * 1/PI → diffuse Radiance.");
            sb.AppendLine("        half3 ambient = max(SampleSH9(SHCoefficients, N), 0.0) * 0.318309886f;");
            sb.AppendLine("        ambient *= _AmbientLightIntensity * _HDRPAmbientMul;");
            sb.AppendLine("        half3 direct = half3(0, 0, 0);");
            sb.AppendLine();
            sb.AppendLine("        // --- Opt-in manual directional + point light iteration ---");
            sb.AppendLine("        #ifdef AIO_HDRP_MANUAL_LIGHTS");
            sb.AppendLine("        // If you enable this define, make sure the following include is in your shader:");
            sb.AppendLine(
                "        //   #include \"Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/LightDefinition.cs.hlsl\"");
            sb.AppendLine("        // so DirectionalLightData / LightData structs are visible. Field names below match");
            sb.AppendLine("        // HDRP 14-17 (Unity 2022.3 – Unity 6) — older HDRP may differ.");
            sb.AppendLine("        for (uint dli = 0u; dli < _DirectionalLightCount; dli++) {");
            sb.AppendLine("            DirectionalLightData dl = _DirectionalLightDatas[dli];");
            sb.AppendLine("            half NdL = saturate(dot(N, -dl.forward));");
            sb.AppendLine("            direct += dl.color * NdL;");
            sb.AppendLine("        }");
            sb.AppendLine("        for (uint pli = 0u; pli < _PunctualLightCount; pli++) {");
            sb.AppendLine("            LightData ld = _LightDatas[pli];");
            sb.AppendLine("            float3 L = ld.positionRWS - GetCameraRelativePositionWS(i.worldPos);");
            sb.AppendLine("            half dist = length(L);");
            sb.AppendLine("            L /= max(dist, 1e-5);");
            sb.AppendLine("            half NdL = saturate(dot(N, L));");
            sb.AppendLine("            half atten = saturate(1.0 - dist * ld.rangeAttenuationScale);");
            sb.AppendLine("            atten *= atten;");
            sb.AppendLine("            direct += ld.color * NdL * atten;");
            sb.AppendLine("        }");
            sb.AppendLine("        #endif // AIO_HDRP_MANUAL_LIGHTS");
            sb.AppendLine();
            sb.AppendLine("        col.rgb *= ambient + direct * _GlobalLightIntensity;");
            sb.AppendLine("    }");
            sb.AppendLine("#endif");
            sb.AppendLine();
            sb.AppendLine("    // HDRP Color Buffer expects pre-exposed values; _HDRPExposureComp is in stops (×2^comp).");
            sb.AppendLine("#if defined(AIO_HDRP_LIT) || defined(AIO_HDRP)");
            sb.AppendLine("    #if defined(SHADERVARIABLES_INCLUDED)");
            sb.AppendLine("        col.rgb *= GetCurrentExposureMultiplier() * exp2(_HDRPExposureComp);");
            sb.AppendLine("    #else");
            sb.AppendLine("        col.rgb *= exp2(_HDRPExposureComp);");
            sb.AppendLine("    #endif");
            sb.AppendLine("#endif");
            sb.AppendLine();
            sb.AppendLine("    return col;");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("half4 fragShadow(v2f i, bool isFrontFace : SV_IsFrontFace) : SV_Target {");
            sb.AppendLine("    if (_CastShadows < 0.5) discard;");
            sb.AppendLine("    half4 col = ComposeAllEffects(i.uv, i.worldPos, i.screenPos); col *= i.color;");
            sb.AppendLine("    clip(col.a - _ShadowAlphaClip); return 0;");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("#if defined(AIO_URP_2D_NORMALS_PASS)");
            sb.AppendLine("struct appdata2DNormals { float4 vertex : POSITION; float2 uv : TEXCOORD0; half4 color : COLOR; float3 normal : NORMAL; float4 tangent : TANGENT; UNITY_VERTEX_INPUT_INSTANCE_ID };");
            sb.AppendLine("struct v2f2DNormals { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; half4 color : COLOR; half3 normalWS : TEXCOORD1; half3 tangentWS : TEXCOORD2; half3 bitangentWS : TEXCOORD3; UNITY_VERTEX_OUTPUT_STEREO };");
            sb.AppendLine();
            sb.AppendLine("half3 AIO_UnpackNormalScale2D(half4 packedNormal, half scale) {");
            sb.AppendLine("    half3 normalTS;");
            sb.AppendLine("    #if defined(UNITY_NO_DXT5nm)");
            sb.AppendLine("        normalTS = packedNormal.xyz * 2.0h - 1.0h;");
            sb.AppendLine("        normalTS.xy *= scale;");
            sb.AppendLine("    #else");
            sb.AppendLine("        normalTS.xy = packedNormal.wy * 2.0h - 1.0h;");
            sb.AppendLine("        normalTS.xy *= scale;");
            sb.AppendLine("        normalTS.z = sqrt(saturate(1.0h - dot(normalTS.xy, normalTS.xy)));");
            sb.AppendLine("    #endif");
            sb.AppendLine("    return normalize(normalTS);");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("v2f2DNormals vert2DNormals(appdata2DNormals v) {");
            sb.AppendLine("    v2f2DNormals o = (v2f2DNormals)0;");
            sb.AppendLine("    UNITY_SETUP_INSTANCE_ID(v);");
            sb.AppendLine("    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);");
            sb.AppendLine("    o.pos = AIO_CLIP_POS(v.vertex);");
            sb.AppendLine("    o.uv = v.uv;");
            sb.AppendLine("    o.color = v.color;");
            sb.AppendLine("    float3 localN = dot(v.normal, v.normal) > 0.0001 ? normalize(v.normal) : float3(0, 0, -1);");
            sb.AppendLine("    float3 localT = dot(v.tangent.xyz, v.tangent.xyz) > 0.0001 ? normalize(v.tangent.xyz) : float3(1, 0, 0);");
            sb.AppendLine("    half tangentSign = v.tangent.w == 0.0 ? 1.0h : v.tangent.w;");
            sb.AppendLine("    o.normalWS = normalize(TransformObjectToWorldNormal(localN));");
            sb.AppendLine("    o.tangentWS = normalize(TransformObjectToWorldDir(localT));");
            sb.AppendLine("    o.bitangentWS = normalize(cross(o.normalWS, o.tangentWS) * tangentSign);");
            sb.AppendLine("    return o;");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("half4 frag2DNormals(v2f2DNormals i) : SV_Target {");
            sb.AppendLine("    half alpha = STEX(_MainTex, sampler_MainTex, i.uv).a * i.color.a;");
            sb.AppendLine("    clip(alpha - _ShadowAlphaClip);");
            sb.AppendLine("    half3 normalTS = AIO_UnpackNormalScale2D(STEX(_NormalMap, sampler_NormalMap, i.uv), _NormalScale);");
            sb.AppendLine("    half3 normalWS = normalize(i.tangentWS * normalTS.x + i.bitangentWS * normalTS.y + i.normalWS * normalTS.z);");
            sb.AppendLine("    return half4(normalWS * 0.5h + 0.5h, alpha);");
            sb.AppendLine("}");
            sb.AppendLine("#endif");
            sb.AppendLine();
            sb.AppendLine("#if defined(AIO_BUILTIN)");
            sb.AppendLine("struct v2fShadowBuiltIn {");
            sb.AppendLine("    V2F_SHADOW_CASTER;");
            sb.AppendLine("    float2 uv : TEXCOORD1;");
            sb.AppendLine("    half4 color : COLOR0;");
            sb.AppendLine("    float3 worldPos : TEXCOORD2;");
            sb.AppendLine("    float4 screenPos : TEXCOORD3;");
            sb.AppendLine("    UNITY_VERTEX_OUTPUT_STEREO");
            sb.AppendLine("};");
            sb.AppendLine();
            sb.AppendLine("v2fShadowBuiltIn vertShadowBuiltIn(appdata v) {");
            sb.AppendLine("    v2fShadowBuiltIn o = (v2fShadowBuiltIn)0;");
            sb.AppendLine("    UNITY_SETUP_INSTANCE_ID(v);");
            sb.AppendLine("    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);");
            sb.AppendLine();
            sb.AppendLine("    if (_VertexUVExpand > 0.001) {");
            sb.AppendLine("        float expand = _VertexUVExpand;");
            sb.AppendLine("        v.vertex.xy *= (1.0 + expand * 2.0);");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    o.uv = v.uv;");
            sb.AppendLine("    o.color = v.color;");
            sb.AppendLine("    o.worldPos = AIO_WORLD_POS(v.vertex);");
            sb.AppendLine("    o.screenPos = AIO_ComputeScreenPos(AIO_CLIP_POS(v.vertex));");
            sb.AppendLine();
            sb.AppendLine("    float3 normalOS = dot(v.normal, v.normal) > 0.0001 ? v.normal : float3(0, 0, -1);");
            sb.AppendLine("    v.normal = normalOS;");
            sb.AppendLine("    TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)");
            sb.AppendLine("    return o;");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("float4 fragShadowBuiltIn(v2fShadowBuiltIn i) : SV_Target {");
            sb.AppendLine("    if (_CastShadows < 0.5)");
            sb.AppendLine("        discard;");
            sb.AppendLine();
            sb.AppendLine("    half4 col = ComposeAllEffects(i.uv, i.worldPos, i.screenPos) * i.color;");
            sb.AppendLine("    clip(col.a - _ShadowAlphaClip);");
            sb.AppendLine("    SHADOW_CASTER_FRAGMENT(i)");
            sb.AppendLine("}");
            sb.AppendLine("#endif");
            sb.AppendLine();
        }

        static void EmitUtilities(StringBuilder sb, bool useIfDefs = true)
        {
            sb.AppendLine("// \u2550\u2550\u2550 UTILITIES \u2550\u2550\u2550");
            sb.AppendLine("#define STEX(tex, samp, uv) SAMPLE_TEXTURE2D(tex, samp, uv)");
            sb.AppendLine();
            sb.AppendLine("// Safety macros — guard against NaN-producing degenerate inputs.");
            sb.AppendLine("#define AIO_SmoothStep(a, b, x) smoothstep((a), max((a) + 1e-4, (b)), (x))");
            sb.AppendLine("#define AIO_Pow(f, e) pow(max((f), 1e-5), (e))");
            sb.AppendLine();

            sb.AppendLine("// Pipeline-safe ScreenPos (Fixes HDRP ComputeScreenPos Missing Definition)");
            sb.AppendLine("inline float4 AIO_ComputeScreenPos(float4 pos) {");
            sb.AppendLine("    float4 o = pos * 0.5f;");
            sb.AppendLine("    o.xy = float2(o.x, o.y * _ProjectionParams.x) + o.w;");
            sb.AppendLine("    o.zw = pos.zw;");
            sb.AppendLine("    return o;");
            sb.AppendLine("}");
            sb.AppendLine();

            sb.AppendLine("#if defined(AIO_URP) || defined(AIO_HDRP)");
            sb.AppendLine("    #define AIO_CLIP_POS(v) TransformObjectToHClip(v.xyz)");
            sb.AppendLine("    #define AIO_WORLD_POS(v) TransformObjectToWorld(v.xyz)");
            sb.AppendLine("    #define AIO_OBJ_TO_WORLD UNITY_MATRIX_M");
            sb.AppendLine("#else");
            sb.AppendLine("    #define AIO_CLIP_POS(v) UnityObjectToClipPos(v)");
            sb.AppendLine("    #define AIO_WORLD_POS(v) mul(unity_ObjectToWorld, v).xyz");
            sb.AppendLine("    #define AIO_OBJ_TO_WORLD unity_ObjectToWorld");
            sb.AppendLine("#endif");
            sb.AppendLine();
            sb.AppendLine("// Two-sided lighting: flips the normal on back-facing pixels so the reverse of a sprite");
            sb.AppendLine("// receives light rather than going black. Three things interact here:");
            sb.AppendLine("//   1) _FlipNormals — per-material toggle (set from C# `twoSidedLighting` bool).");
            sb.AppendLine("//   2) SV_IsFrontFace — should return true for front faces on all modern Unity back-ends");
            sb.AppendLine("//      (D3D11/12, Metal, Vulkan, and modern OpenGL). Legacy OpenGL ES or odd mobile drivers");
            sb.AppendLine("//      sometimes invert it. If you see the LIT side going dark while the back is lit, add");
            sb.AppendLine("//      AIO_FLIP_FRONTFACE to scripting define symbols; the shader picks up the inversion.");
            sb.AppendLine("//   3) Cull Off — the SubShader turns off backface culling so both sides rasterize in the");
            sb.AppendLine("//      first place. If Cull were On, only the front would ever shade.");
            sb.AppendLine("inline half3 AIO_GetFaceNormal(half3 normalWS, bool isFront) {");
            sb.AppendLine("    #ifdef AIO_FLIP_FRONTFACE");
            sb.AppendLine("        isFront = !isFront;");
            sb.AppendLine("    #endif");
            sb.AppendLine("    half flip = (_FlipNormals > 0.5) ? (isFront ? 1.0 : -1.0) : 1.0;");
            sb.AppendLine("    return normalWS * flip;");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine(
                "inline half GetTime() { half t = _UseCustomTime > 0.5 ? _CustomTime : _Time.y; t *= _TimeScale; if (_TimeFPS > 0.5) t = floor(t * _TimeFPS) / _TimeFPS; return t; }");
            sb.AppendLine(
                "inline half SampleEffectMask(float2 uv, half4 masks, half ch) { if (ch<0.5) return 1; if (ch<1.5) return masks.r; if (ch<2.5) return masks.g; if (ch<3.5) return masks.b; return masks.a; }");
            sb.AppendLine(
                "inline half4 GetEffectMasks(float2 uv) { return _MaskTex_En > 0.5 ? STEX(_EffectMasks,sampler_EffectMasks,uv) : half4(1,1,1,1); }");
            sb.AppendLine(
                "inline half GetGlobalMask(float2 uv) { return _EffectMask_En > 0.5 ? STEX(_GlobalEffectMask,sampler_GlobalEffectMask,uv).r : 1; }");
            sb.AppendLine("inline float2 GetScreenUV(float4 sp) { return sp.xy / max(sp.w, 0.001); }");
            sb.AppendLine(
                "float2 ClampToAtlas(float2 uv) {if(_MaxXUV<=0.001) {_MaxXUV = _MaxYUV = 1.0; }; return float2(clamp(uv.x,_MinXUV,_MaxXUV),clamp(uv.y,_MinYUV,_MaxYUV)); }");
            sb.AppendLine("float2 PixelSnap(float2 uv, half res) { return floor(uv*res+0.5)/res; }");
            sb.AppendLine(
                "half SampleAtlasClamped(float2 uv) { float2 c=ClampToAtlas(uv); return any(abs(c-uv)>0.0001)?0:STEX(_MainTex,sampler_MainTex,c).a; }");
            sb.AppendLine(
                "inline half AtlasRectMask(float2 uv) { float2 mn = float2(_MinXUV, _MinYUV); float2 mx = (_MaxXUV <= 0.001) ? float2(1.0, 1.0) : float2(_MaxXUV, _MaxYUV); half2 a = step(mn - 0.00001, uv); half2 b = step(uv, mx + 0.00001); return a.x * a.y * b.x * b.y; }");
            sb.AppendLine(
                "half4 SampleMainClean(float2 uv) { uv=ClampToAtlas(uv); half4 s=STEX(_MainTex,sampler_MainTex,uv); half cl=smoothstep(0,0.1,s.a); s.rgb*=cl; s.a*=cl; return s; }");
            sb.AppendLine("inline float2 AtlasCenter() { return float2((_MinXUV + _MaxXUV) * 0.5, (_MinYUV + _MaxYUV) * 0.5); }");
            sb.AppendLine("inline float2 AtlasHalfSize() { return float2((_MaxXUV - _MinXUV) * 0.5, (_MaxYUV - _MinYUV) * 0.5); }");
            sb.AppendLine(
                "inline float2 LocalUV(float2 uv) { float2 sz = float2(_MaxXUV - _MinXUV, _MaxYUV - _MinYUV); return (uv - float2(_MinXUV, _MinYUV)) / max(sz, 0.0001); }");
            sb.AppendLine("half ColorDist(half3 a, half3 b) { half3 d=abs(a-b); return (d.r+d.g+d.b)/3; }");
            sb.AppendLine();
            sb.AppendLine("half3 RGB2HSV(half3 c) { half4 K=half4(0,-1.0/3.0,2.0/3.0,-1); half4 p=lerp(half4(c.bg,K.wz),half4(c.gb,K.xy),step(c.b,c.g)); half4 q=lerp(half4(p.xyw,c.r),half4(c.r,p.yzx),step(p.x,c.r)); half d=q.x-min(q.w,q.y); half e=1e-10; return half3(abs(q.z+(q.w-q.y)/(6*d+e)),d/(q.x+e),q.x); }");
            sb.AppendLine(
                "half3 HSV2RGB(half3 c) { half4 K=half4(1,2.0/3.0,1.0/3.0,3); half3 p=abs(frac(c.xxx+K.xyz)*6-K.www); return c.z*lerp(K.xxx,saturate(p-K.xxx),c.y); }");
            sb.AppendLine();
            sb.AppendLine("half SimpleNoise(float2 uv) { return frac(sin(dot(uv,float2(12.9898,78.233)))*43758.5453); }");
            sb.AppendLine(
                "half ValueNoise(float2 uv) { float2 i=floor(uv);float2 f=frac(uv);f=f*f*(3-2*f); return lerp(lerp(SimpleNoise(i),SimpleNoise(i+float2(1,0)),f.x),lerp(SimpleNoise(i+float2(0,1)),SimpleNoise(i+float2(1,1)),f.x),f.y); }");
            sb.AppendLine(
                "float2 Hash2D(float2 p) { float2 h=float2(dot(p,float2(127.1,311.7)),dot(p,float2(269.5,183.3))); return frac(sin(h)*43758.5453)*2-1; }");
            sb.AppendLine("half GradientNoise(float2 uv) { float2 i=floor(uv);float2 f=frac(uv);float2 u=f*f*(3-2*f); return saturate(lerp(lerp(dot(Hash2D(i),f),dot(Hash2D(i+float2(1,0)),f-float2(1,0)),u.x),lerp(dot(Hash2D(i+float2(0,1)),f-float2(0,1)),dot(Hash2D(i+float2(1,1)),f-float2(1,1)),u.x),u.y)*0.7+0.5); }");
            sb.AppendLine("half CellularNoise(float2 uv) { float2 i=floor(uv);float2 f=frac(uv);half md=1; for(int x=-1;x<=1;x++)for(int y=-1;y<=1;y++){float2 nb=float2(x,y);float2 pt=float2(SimpleNoise(i+nb),SimpleNoise(i+nb+float2(137.2,0)));md=min(md,length(nb+pt-f));}return smoothstep(0,0.7,md); }");
            sb.AppendLine(
                "half FBMNoise(float2 uv,int oct){half v=0;half a=0.5;half fr=1;for(int i=0;i<oct;i++){v+=a*ValueNoise(uv*fr);fr*=2;a*=0.5;}return v;}");
            sb.AppendLine(
                "float2 CurlNoise(float2 uv){half e=0.01;half n=ValueNoise(uv);return float2(-(ValueNoise(uv+float2(0,e))-n)/e,(ValueNoise(uv+float2(e,0))-n)/e);}");
            sb.AppendLine(
                "half SampleNoise(float2 uv,half nt,half s,half4 ts){if(nt<0.5)return ts.r;float2 nu=uv*s;return nt<1.5?GradientNoise(nu):CellularNoise(nu);}");
            sb.AppendLine("void VoronoiNoise(float2 uv,float time,out half ed,out half cid){float2 cell=floor(uv);float2 fr=frac(uv);half md=10;half md2=10;float2 bc=float2(0,0);for(int x=-1;x<=1;x++)for(int y=-1;y<=1;y++){float2 nb=float2(x,y);float2 pt=float2(SimpleNoise(cell+nb),SimpleNoise(cell+nb+float2(137.2,0)));pt=0.5+0.5*sin(time*0.5+6.2831*pt);float d=length(nb+pt-fr);if(d<md){md2=md;md=d;bc=cell+nb;}else if(d<md2)md2=d;}ed=md2-md;cid=SimpleNoise(bc);}");
            sb.AppendLine();
            if (useIfDefs) sb.AppendLine("#ifdef _FX_EDGE");
            sb.AppendLine("half SampleSDF(float2 uv) {");
            sb.AppendLine("    float2 spriteUV = (uv - _OutlineSDFRect.xy) / _OutlineSDFRect.zw;");
            sb.AppendLine("    float2 sdfRes = _OutlineSDFTex_TexelSize.zw;");
            sb.AppendLine("    float2 spriteRes = sdfRes - _OutlineSDFPadding * 2;");
            sb.AppendLine("    float2 sdfUV = (spriteUV * spriteRes + _OutlineSDFPadding) / sdfRes;");
            sb.AppendLine("    return STEX(_OutlineSDFTex, sampler_OutlineSDFTex, sdfUV).r; }");
            sb.AppendLine();
            sb.AppendLine("half SampleInverseOutlineAlpha(float2 uv, half size) {");
            sb.AppendLine("    half avgTexel = (_MainTex_TexelSize.x + _MainTex_TexelSize.y) * 0.5;");
            sb.AppendLine("    half sizeUV = (_OutlineFixedWidthPx > 0.5) ? (avgTexel * _OutlineFixedWidthPx) : size;");
            sb.AppendLine("    half minA = 1.0;");
            sb.AppendLine("    if (_OutlineMode < 1.5) {");
            sb.AppendLine("        minA = min(minA, SampleAtlasClamped(uv + float2(-1.0, -1.0) * sizeUV));");
            sb.AppendLine("        minA = min(minA, SampleAtlasClamped(uv + float2( 0.0, -1.0) * sizeUV));");
            sb.AppendLine("        minA = min(minA, SampleAtlasClamped(uv + float2( 1.0, -1.0) * sizeUV));");
            sb.AppendLine("        minA = min(minA, SampleAtlasClamped(uv + float2(-1.0,  0.0) * sizeUV));");
            sb.AppendLine("        minA = min(minA, SampleAtlasClamped(uv + float2( 1.0,  0.0) * sizeUV));");
            sb.AppendLine("        minA = min(minA, SampleAtlasClamped(uv + float2(-1.0,  1.0) * sizeUV));");
            sb.AppendLine("        minA = min(minA, SampleAtlasClamped(uv + float2( 0.0,  1.0) * sizeUV));");
            sb.AppendLine("        minA = min(minA, SampleAtlasClamped(uv + float2( 1.0,  1.0) * sizeUV));");
            sb.AppendLine("        return (1.0 - minA);");
            sb.AppendLine("    } else {");
            sb.AppendLine("        half sdf = SampleSDF(uv); half softness = max(_OutlineSDFSoftness, fwidth(sdf));");
            sb.AppendLine("        half sdfWidth = (sizeUV / max(avgTexel, 1e-6)) / max(_OutlineSDFSpread * 2, 1);");
            sb.AppendLine(
                "        return smoothstep(0.5, 0.5 + softness, sdf) * (1.0 - smoothstep(0.5 + sdfWidth, 0.5 + sdfWidth + softness, sdf)); } }");
            sb.AppendLine();
            sb.AppendLine("half SampleOutlineAlpha(float2 uv, half size) {");
            sb.AppendLine("    half avgTexel = (_MainTex_TexelSize.x + _MainTex_TexelSize.y) * 0.5;");
            sb.AppendLine("    half sizeUV = (_OutlineFixedWidthPx > 0.5) ? (avgTexel * _OutlineFixedWidthPx) : size;");
            sb.AppendLine("    half maxA = 0;");
            sb.AppendLine("    if (_OutlineMode < 0.5) {");
            sb.AppendLine("        maxA = max(maxA, SampleAtlasClamped(uv + float2(-1.0, -1.0) * sizeUV));");
            sb.AppendLine("        maxA = max(maxA, SampleAtlasClamped(uv + float2( 0.0, -1.0) * sizeUV));");
            sb.AppendLine("        maxA = max(maxA, SampleAtlasClamped(uv + float2( 1.0, -1.0) * sizeUV));");
            sb.AppendLine("        maxA = max(maxA, SampleAtlasClamped(uv + float2(-1.0,  0.0) * sizeUV));");
            sb.AppendLine("        maxA = max(maxA, SampleAtlasClamped(uv + float2( 1.0,  0.0) * sizeUV));");
            sb.AppendLine("        maxA = max(maxA, SampleAtlasClamped(uv + float2(-1.0,  1.0) * sizeUV));");
            sb.AppendLine("        maxA = max(maxA, SampleAtlasClamped(uv + float2( 0.0,  1.0) * sizeUV));");
            sb.AppendLine("        maxA = max(maxA, SampleAtlasClamped(uv + float2( 1.0,  1.0) * sizeUV));");
            sb.AppendLine("    }");
            sb.AppendLine("    else if (_OutlineMode < 1.5) {");
            sb.AppendLine("        int rings = max(1, (int)_OutlineRings);");
            sb.AppendLine("        for (int r = 1; r <= rings; r++) { half radius = sizeUV * (half)r / (half)rings;");
            sb.AppendLine("            maxA = max(maxA, SampleAtlasClamped(uv + half2( 1,  0) * radius));");
            sb.AppendLine("            maxA = max(maxA, SampleAtlasClamped(uv + half2(-1,  0) * radius));");
            sb.AppendLine("            maxA = max(maxA, SampleAtlasClamped(uv + half2( 0,  1) * radius));");
            sb.AppendLine("            maxA = max(maxA, SampleAtlasClamped(uv + half2( 0, -1) * radius));");
            sb.AppendLine("            maxA = max(maxA, SampleAtlasClamped(uv + half2( 1,  1) * radius));");
            sb.AppendLine("            maxA = max(maxA, SampleAtlasClamped(uv + half2(-1,  1) * radius));");
            sb.AppendLine("            maxA = max(maxA, SampleAtlasClamped(uv + half2( 1, -1) * radius));");
            sb.AppendLine("            maxA = max(maxA, SampleAtlasClamped(uv + half2(-1, -1) * radius)); }");
            sb.AppendLine("    } else {");
            sb.AppendLine("        half sdf = SampleSDF(uv); half softness = max(_OutlineSDFSoftness, fwidth(sdf));");
            sb.AppendLine("        half sizePx = (_OutlineFixedWidthPx > 0.5) ? _OutlineFixedWidthPx : (sizeUV / max(avgTexel, 1e-6));");
            sb.AppendLine("        half sdfWidth = sizePx / max(_OutlineSDFSpread * 2, 1); sdfWidth = max(sdfWidth, 0.001);");
            sb.AppendLine("        half outerEdge = 0.5 - sdfWidth;");
            sb.AppendLine("        half outer = smoothstep(outerEdge - softness, outerEdge, sdf);");
            sb.AppendLine("        half inner = smoothstep(0.5, 0.5 + softness, sdf);");
            sb.AppendLine("        maxA = outer * (1.0 - inner); }");
            sb.AppendLine("    return maxA; }");
            sb.AppendLine();
            sb.AppendLine("half MaskOutline(half maxA, half colA) { return _OutlineMode > 1.5 ? maxA : maxA * (1 - colA); }");
            sb.AppendLine();
            sb.AppendLine("// Outline-effects-only mask: when enabled, restricts effects to the outline ring.");
            sb.AppendLine("// Lives inside _FX_EDGE because it depends on SampleOutlineAlpha/MaskOutline.");
            sb.AppendLine("half ComputeOutlineEffectMask(float2 uv, half4 col) {");
            sb.AppendLine("    if (_OutlineEffectsOnly_En < 0.5) return 1;");
            sb.AppendLine("    half width = max(_OutlineEffectWidth, 1e-5);");
            sb.AppendLine("    half ringA = SampleOutlineAlpha(uv, width);");
            sb.AppendLine("    return MaskOutline(ringA, col.a);");
            sb.AppendLine("}");
            if (useIfDefs) sb.AppendLine("#endif // _FX_EDGE");
            //  Fallback when _FX_EDGE is not active: mask is always 1.
            if (useIfDefs)
            {
                sb.AppendLine("#ifndef _FX_EDGE");
                sb.AppendLine("half ComputeOutlineEffectMask(float2 uv, half4 col) { return 1; }");
                sb.AppendLine("#endif");
            }
            sb.AppendLine();
        }
    }
}
#endif
