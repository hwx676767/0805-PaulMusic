using UnityEngine;


namespace FlexiSprite
{
    public enum EffectId
    {
        StrongTint, AddColor, AlphaTint, HitEffect, Negative, Greyscale,
        Bloom, ColorReplace, LUTReplace, HueShift, AddHue, Saturation, Brightness,
        Contrast, SplitToning, Posterize, Poison, Frozen,
        Metal, Camouflage, Rainbow, BlackTint, Chromatic,
        PaletteSwap, Dithering, CRTScanlines, Vignette, ColorGrading, Sepia,
        ThermalVision, RecolorRGB, PingPongGlow, ColorCycle,
        Outline, InnerOutline, TexOutline, RainbowOutline, DistortOutline,
        Glow, Shadow, SoftShadow, RimLight, OutlineOnly,
        _DeprecatedPixelOutline, GradientOutline, AnimOutlineThick, SelectionHighlight, _DeprecatedUnifiedOutline,
        Pixelate, Wave, Twist, UVDistort, FullDistort, DirDistort,
        UVScroll, Rotate, Squeeze, Squish, Wiggle, Wind,
        Sharpen, GaussBlur, DirBlur, WorldTiling,
        Shockwave, SineScale, Heartbeat, Breathing, LiquidWobble, FlipBookBlend, SmoothPixelArt,
        UVScaleOffset, CurlNoise, FBMNoise, UVScrollOverlay,
        Dissolve, GlowDissolve, SrcDissolve, DirFade, DirGlowFade,
        FullAlphaDissolve, Halftone, AlphaCutoff,
        RadialWipe, IrisWipe, Blinds, DiamondDissolve, ScanlineDissolve, SpreadFade,
        PixelDissolve, SpiralWipe, HexDissolve,
        Hologram, Glitch, Flicker, Shine, Smoke, Flame,
        Enchanted, Shifting, ShiftRainbow, Overlay, Checkerboard,
        UVVisualize, HealthTint, XRaySilhouette, MinimapPing, DialogueEmphasis,
        VoronoiCrackle, WaterReflect, MotionSmear, DepthParallax,
        InkBleed, EnergyAura, RetroPalette, Ghosting,
        Gradient, RadialGradient, ColorRamp, BurnDirection, ImprovedEnchanted,
        GlobalSmoke, HolographicCard, WindMotionVector, CloudShadows,
        ColorfulTrailA, ColorfulTrailB, ColorfulTrailC,
        FakeLighting, DashShader, ChargingAbility, Blueprint, UVBendPoint,
        DirOutline, UVChecker, UVNumeric
    }

    public enum EffectCategory
    {
        Color,
        Edge,
        UV,
        Dissolve,
        Special,
    }

    public enum EffectGroup
    {
        Color,
        ColorExt,
        Edge,
        UV,
        Filter,
        Dissolve,
        VFX,
        Game,
        _Count,
    }

    public enum ParamType
    {
        Float,
        Color,
        HDRColor,
        Vector2,
        Vector3,
        Vector4,
        Texture,
        Int,
    }

    public enum RenderPipelineTarget
    {
        BuiltIn,
        URP,
        HDRP,
    }

    public enum PushStrategy
    {
        SRPBatcher,
        MPBShared,
        MPBInstanced,
    }

    /// <summary>
    /// Controls how shader keywords are compiled in editor vs build.
    /// Set in AnimSpriteProjectSettings (Master Setup window).
    /// </summary>
    public enum EditorKeywordMode
    {
        MultiCompile = 0,
        ShaderFeature = 1,
        NoKeywords = 2,
    }

    /// <summary>
    /// Controls what keyword mode is regenerated immediately before a build.
    /// Default is MultiCompile to keep all group variants available in builds.
    /// </summary>
    public enum BuildKeywordMode
    {
        MultiCompile = 0,
        ForceAllGroupsNoKeywords = 1,
        KeepEditorMode = 2,
        ShaderFeature = 3
    }

    public struct EffectParam
    {
        public string field;
        public string shader;
        public string display;
        public string defaultValue;
        public string rangeAttr;
        public ParamType type;
        public float min;
        public float max;
        public static EffectParam Float(string f, string s, string d, float mn, float mx, string def = null)
        {
            if (def == null)
            {
                def = mn > 0 ? mn.ToString("G") : "0";
            }

            return new EffectParam
            {
                field = f,
                shader = s,
                display = d,
                type = ParamType.Float,
                min = mn,
                max = mx,
                defaultValue = def,
                rangeAttr = mn != 0 || mx != 0 ? $"[Range({mn}f,{mx}f)]" : "",
            };
        }

        public static EffectParam Col(string f, string s, string d, string def = "Color.white")
            => new EffectParam
            {
                field = f,
                shader = s,
                display = d,
                type = ParamType.Color,
                defaultValue = def,
            };

        public static EffectParam Hdr(string f, string s, string d, string def = "Color.white")
            => new EffectParam
            {
                field = f,
                shader = s,
                display = d,
                type = ParamType.HDRColor,
                defaultValue = def,
            };

        public static EffectParam Vec(string f, string s, string d, string def = "Vector4.zero")
            => new EffectParam
            {
                field = f,
                shader = s,
                display = d,
                type = ParamType.Vector4,
                defaultValue = def,
            };

        public static EffectParam Vec2(string f, string s, string d, string def = "Vector2.zero")
            => new EffectParam
            {
                field = f,
                shader = s,
                display = d,
                type = ParamType.Vector2,
                defaultValue = def,
            };

        public static EffectParam Vec3(string f, string s, string d, string def = "Vector3.zero")
            => new EffectParam
            {
                field = f,
                shader = s,
                display = d,
                type = ParamType.Vector3,
                defaultValue = def,
            };

        public static EffectParam Tex(string f, string s, string d)
            => new EffectParam
            {
                field = f,
                shader = s,
                display = d,
                type = ParamType.Texture,
            };

        public static EffectParam Int(string f, string s, string d, int mn, int mx, int def = 0)
            => new EffectParam
            {
                field = f,
                shader = s,
                display = d,
                type = ParamType.Int,
                min = mn,
                max = mx,
                defaultValue = def.ToString(),
                rangeAttr = $"[Range({mn},{mx})]",
            };
    }

    public class EffectDef
    {
        public EffectId id;
        public string name;
        public string keyword;
        public string enableField;
        public string enableShader;
        public string maskField;
        public string maskShader;
        public EffectCategory category;
        public EffectGroup group;
        public bool hasMask;
        public EffectParam[] parms;
        public int orderInGroup;
    }

    public static class EffectGroupInfo
    {
        public static readonly string[] Keywords =
        {
            "_FX_COLOR",
            "_FX_COLOR_EXT",
            "_FX_EDGE",
            "_FX_UV",
            "_FX_FILTER",
            "_FX_DISSOLVE",
            "_FX_VFX",
            "_FX_GAME",
        };

        public static readonly string[] DisplayNames =
        {
            "Color - Basic",
            "Color - Extended",
            "Edge",
            "UV - Transform",
            "Filter",
            "Dissolve / Fade",
            "VFX - Effects",
            "Gameplay / Utility",
        };
        public static string GetKeyword(EffectGroup g) => Keywords[(int)g];
        public static string GetDisplayName(EffectGroup g) => DisplayNames[(int)g];
    }

    public static class PipelineRouting
    {
        public static bool IsVariantSRP(AnimatedSpriteShader.ShaderVariant v) => v switch
        {
            AnimatedSpriteShader.ShaderVariant.Lit2D_URP or AnimatedSpriteShader.ShaderVariant.Lit3D_URP or
            AnimatedSpriteShader.ShaderVariant.Lit3DCutout_URP or AnimatedSpriteShader.ShaderVariant.Lit2DAdditive_URP or
            AnimatedSpriteShader.ShaderVariant.Lit3DAdditive_URP or
            AnimatedSpriteShader.ShaderVariant.Standard_URP or AnimatedSpriteShader.ShaderVariant.Additive_URP or
            AnimatedSpriteShader.ShaderVariant.Premultiply_URP or AnimatedSpriteShader.ShaderVariant.GUI_URP or
            AnimatedSpriteShader.ShaderVariant.GUIAdditive_URP or
            AnimatedSpriteShader.ShaderVariant.HDRP_Unlit or AnimatedSpriteShader.ShaderVariant.HDRP_Additive or
            AnimatedSpriteShader.ShaderVariant.HDRP_Lit => true,
            _ => false,
        };

        /// <summary>Returns true if the variant renders through UGUI (Canvas / Image component) rather than a sprite/mesh renderer.</summary>
        public static bool IsVariantGUI(AnimatedSpriteShader.ShaderVariant v) => v switch
        {
            AnimatedSpriteShader.ShaderVariant.GUI or AnimatedSpriteShader.ShaderVariant.GUIAdditive or
            AnimatedSpriteShader.ShaderVariant.GUIPremultiply or
            AnimatedSpriteShader.ShaderVariant.GUI_URP or AnimatedSpriteShader.ShaderVariant.GUIAdditive_URP => true,
            _ => false,
        };

        public static PushStrategy GetStrategy(AnimatedSpriteShader.ShaderVariant v, AnimatedSpriteShader.MaterialMode m)
        {
            // UGUI Image cannot consume MaterialPropertyBlocks, so GUI variants must push directly
            // to the material regardless of pipeline — treat them as SRPBatcher-style.
            if (IsVariantGUI(v))
            {
                return PushStrategy.SRPBatcher;
            }
            return IsVariantSRP(v) ? PushStrategy.SRPBatcher
                 : m == AnimatedSpriteShader.MaterialMode.Instanced ? PushStrategy.MPBInstanced
                 : PushStrategy.MPBShared;
        }
        public static bool NeedsInstancedMaterial(PushStrategy s) => s != PushStrategy.MPBShared;

        public static bool UsesMPB(PushStrategy s) => s != PushStrategy.SRPBatcher;

        public static bool PushesToMaterial(PushStrategy s) => s == PushStrategy.SRPBatcher;
    }

    /// <summary>
    /// Performance categorization for effects. Used by editor for perf dot coloring.
    /// HEAVY: PC or 1 mobile sprite max (nested loops, >8 texture fetches, multi-octave noise).
    /// MEDIUM: High-end phones (procedural noise, trig, branching, 3-5 texture taps).
    /// LIGHT: Everything else (pure math, single texture lookup, simple blending).
    /// </summary>
    public static class EffectCategorization
    {
        private static readonly System.Collections.Generic.HashSet<string> HeavyEffects = new()
        {
            "Bloom", "Gaussian Blur", "Directional Blur", "Soft Shadow", "Glow",
            "Ghosting", "Motion Smear", "Energy Aura", "FBM Noise",
            "Distort Outline", "Blueprint", "Trail - Afterimage", "Health Tint"
        };

        private static readonly System.Collections.Generic.HashSet<string> MediumEffects = new()
        {
            // Outlines (4-8 taps)
            "Outline", "Inner Outline", "Texture Outline", "Rainbow Outline", "Directional Outline",
            "Gradient Outline", "Anim Outline Thickness", "Selection Highlight", "Outline Only",
            // Noise & Distortion
            "UV Distort", "Full Distort", "Curl Noise", "Voronoi Crackle", "Liquid Wobble",
            "Smoke", "Global Smoke", "Flame", "Enchanted", "Poison", "Camouflage",
            "Cloud Shadows", "Frozen",
            // Texture lookups
            "Water Reflection", "Chromatic Aberration", "Sharpen", "Color Replace",
            "LUT Replace", "Retro Palette", "Glitch",
            // Dissolves with noise
            "Dissolve", "Glow Dissolve", "Pixel Dissolve", "Hex Dissolve", "Burn Direction",
            "Shockwave", "UV Scroll Overlay", "Ink Bleed"
        };

        public static int GetPerfLevel(string effectName)
        {
            if (HeavyEffects.Contains(effectName))
            {
                return 2;
            }

            if (MediumEffects.Contains(effectName))
            {
                return 1;
            }
            return 0;
        }

        /// <summary>0 = light (green), 1 = medium (yellow), 2 = heavy (red)</summary>
        public static int GetPerfLevel(EffectDef def) => GetPerfLevel(def.name);
    }
}
