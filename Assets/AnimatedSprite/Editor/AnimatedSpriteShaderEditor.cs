using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;


namespace FlexiSprite
{
    [CustomEditor(typeof(AnimatedSpriteShader))]
    public class AnimatedSpriteShaderEditor : Editor
    {
        // Constants
        private static readonly Color[] MaskDotColors = {
            Color.gray,
            Color.red,
            Color.green,
            new Color(0.3f, 0.5f, 1),
            new Color(1, 0.9f, 0.2f),
        };
        private static readonly string[] OutlineStyleLabels = {
            "Classic 3x3",
            "Multi-Ring",
            "SDF Precomputed",
        };
        private static readonly string[] NoiseTypeLabels = { "Texture", "Gradient", "Cellular" };
        private static readonly string[] CoordSpaceLabels = {
            "Object UV",
            "World Position",
            "Screen Position",
        };
        private static readonly string[] BloomModeLabels = {
            "Luminance",
            "Color Mask",
            "Luminance + Color",
            "Full (No Threshold)",
        };
        private static readonly string[] SampleCountLabels = { "1", "2", "4", "8", "16", "32" };
        private static readonly int[] SampleCountValues = { 1, 2, 4, 8, 16, 32 };
        private static readonly Color HeaderBg = new Color(0.08f, 0.08f, 0.10f, 1);
        private static readonly Color ToolsBg = new Color(0.14f, 0.14f, 0.16f, 1);
        private static readonly Color ToolDefault = new Color(0.30f, 0.30f, 0.33f, 1);
        private static readonly Color ToolRed = new Color(0.55f, 0.20f, 0.18f, 1);
        private static readonly Color SectionBg = new Color(0.10f, 0.10f, 0.12f, 1);
        private static readonly Color PerfGreen = new Color(0.3f, 0.85f, 0.3f);
        private static readonly Color PerfYellow = new Color(0.9f, 0.8f, 0.2f);
        private static readonly Color PerfRed = new Color(0.9f, 0.3f, 0.2f);

        // Short descriptions shown in the inspector.
        private static readonly Dictionary<EffectId, string> _effectTooltips = new()
        {
            // Color
            {
                EffectId.StrongTint,
                "Paints over the sprite with a solid color while keeping transparency. Great for damage flashes, frozen states, or solid silhouettes."
            },
            {
                EffectId.AddColor,
                "Brightens the sprite by adding a color tint on top. Perfect for glowing, magical, or 'supercharged' states."
            },
            {
                EffectId.AlphaTint,
                "Tints the sprite while keeping dark areas dark. Excellent for ambient mood lighting (like night-time blue) or underwater scenes."
            },
            {
                EffectId.HitEffect,
                "A harsh, flat color override. Essential for that classic arcade-style white 'damage flash' when an enemy is struck."
            },
            { EffectId.Negative, "Inverts the colors. Gives a spooky, corrupted, or 'phase-shifted' look." },
            {
                EffectId.Greyscale,
                "Drains all color to black and white. Perfect for death states, petrification, or cinematic flashbacks."
            },
            {
                EffectId.Bloom,
                "Makes the bright areas of your sprite glow softly. Essential for neon signs, energy cores, or dreamy atmospheres."
            },
            {
                EffectId.ColorReplace,
                "Swaps up to 4 specific colors for new ones. The absolute best tool for making alternate character skins or palette swaps."
            },
            {
                EffectId.HueShift,
                "Slides the sprite's colors around the rainbow. Good for psychedelic effects or rapid 'invincibility' color-cycling."
            },
            {
                EffectId.Saturation,
                "Boosts or drains color intensity. Push it high for a vibrant, punchy look, or drop it low for a muted, gloomy, desaturated vibe."
            },
            { EffectId.Brightness, "Lightens or darkens the entire sprite uniformly." },
            {
                EffectId.Contrast,
                "Makes darks darker and lights lighter. Good for making a washed-out sprite 'pop' with more definition."
            },
            {
                EffectId.Posterize,
                "Reduces the number of colors, creating chunky, distinct bands of color. Gives a retro, low-bit, or comic-book 'cel-shaded' style."
            },
            {
                EffectId.Poison,
                "Adds a sickly, bubbling, multi-colored noise overlay. Perfect for toxic, radioactive, or infected status effects."
            },
            {
                EffectId.Frozen,
                "Covers the sprite in an icy, crystalline texture with bright, frosty highlights. Great for 'chilled' status effects."
            },
            {
                EffectId.Metal,
                "Gives the sprite a shiny, reflective, metallic sheen that catches the light as it moves."
            },
            {
                EffectId.Rainbow,
                "Washes the sprite in a scrolling rainbow gradient. Classic visual for invincibility stars or ultimate abilities."
            },
            {
                EffectId.Chromatic,
                "Splits the red and blue color channels. Creates a glitchy '3D glasses' look or a heavy, dizzying impact effect."
            },
            {
                EffectId.Sepia,
                "Tints everything a warm, vintage brown. Gives the look of an antique photograph or an old western."
            },
            {
                EffectId.PingPongGlow,
                "Smoothly pulses a glowing color on and off. Great for gentle idle alarms, 'ready' states, or glowing runes."
            },
            {
                EffectId.ColorCycle,
                "Smoothly crossfades through four custom colors over time. Great for enchanted items or shifting elemental states."
            },

            // Color EXT
            {
                EffectId.Gradient,
                "Washes a smooth two-color gradient across the sprite at any angle. Good for lighting shifts or stylized shading."
            },
            {
                EffectId.RadialGradient,
                "Adds a circular gradient radiating outward from a center point. Good for a localized glow or darkening the edges."
            },
            {
                EffectId.ColorRamp,
                "Completely recolors the sprite using a custom gradient map. Amazing for achieving extreme stylized lighting or a 'thermal vision' look."
            },

            // Edge
            {
                EffectId.Outline,
                "Adds a solid border around the sprite. Essential to make characters pop against backgrounds. Use 'SDF Mode' for super smooth, scalable, high-res outlines."
            },
            {
                EffectId.InnerOutline,
                "Draws a border *inside* the sprite. Good for subtle selection highlights that don't change the sprite's outer footprint."
            },
            {
                EffectId.TexOutline,
                "Uses a custom texture for the outline. Imagine a border made of scrolling fire, starry space, or flowing water."
            },
            { EffectId.RainbowOutline, "A solid outline that constantly cycles through the colors of the rainbow." },
            {
                EffectId.DistortOutline,
                "A wobbly, wavy, unstable outline. Great for jittery energy fields, slimes, or underwater borders."
            },
            {
                EffectId.Glow,
                "Adds a soft, fuzzy, luminous aura around the outside of the sprite. Perfect for magical items or glowing auras."
            },
            {
                EffectId.Shadow,
                "Drops a crisp, solid shadow offset behind the sprite. Gives a classic 2D UI or retro paper-craft feel."
            },
            {
                EffectId.SoftShadow,
                "Drops a blurred, realistic shadow behind the sprite to add depth and separate it from the background."
            },
            {
                EffectId.DirOutline,
                "Highlights only the edges facing a certain direction. Perfect for faking a rim-light from a sun, moon, or nearby torch."
            },
            {
                EffectId.GradientOutline,
                "An outline that fades smoothly between two colors from one side to the other."
            },
            {
                EffectId.SelectionHighlight,
                "An animated, dashed 'marching ants' outline. The standard 'this unit is selected' strategy game look."
            },

            // UV and distortion
            {
                EffectId.Pixelate,
                "Makes the sprite blocky and low-res. Great for censoring, 'retro-demakes', or transitioning into a digital world."
            },
            {
                EffectId.Wave,
                "Makes the sprite ripple gently. Perfect for a flag flapping in the wind or a reflection in a pool of water."
            },
            {
                EffectId.Twist,
                "Swirls the sprite around a center point, like it's being sucked into a vortex or a black hole."
            },
            {
                EffectId.UVDistort,
                "Warps the sprite using a custom noise texture. Good for heavy heat haze, underwater wobbles, or messy glitches."
            },
            {
                EffectId.FullDistort,
                "A built-in, chaotic, bubbly distortion. Looks like boiling liquid, heavy heat shimmer, or chaotic energy."
            },
            {
                EffectId.DirDistort,
                "Smudges the sprite back and forth along a specific angle. Good for faking fast, vibrating movement."
            },
            {
                EffectId.UVScroll,
                "Slides the sprite's texture continuously. Essential for animating conveyor belts, flowing water, or continuous laser beams."
            },
            { EffectId.Rotate, "Spins the texture around its center point continuously or sets it to a fixed angle." },
            {
                EffectId.Squeeze,
                "Pinches and stretches the sprite repeatedly. Good for a cartoonish 'breathing' or 'squishy' idle animation."
            },
            {
                EffectId.Squish,
                "A classic animation squash-and-stretch effect. Use it for heavy impacts, jump wind-ups, or bouncing."
            },
            {
                EffectId.Wiggle,
                "Shakes the sprite rapidly side-to-side. Perfect for an 'about to explode', 'frozen shivering', or 'taking damage' state."
            },
            {
                EffectId.Wind,
                "Bends the top of the sprite while keeping the bottom planted. Perfect for grass, trees, or hair blowing in the breeze."
            },
            {
                EffectId.WorldTiling,
                "Repeats the sprite infinitely across the world. Great for making seamless backgrounds, walls, or floors out of a single small sprite."
            },
            { EffectId.UVScaleOffset, "Zooms in or shifts the texture around inside its bounding box." },
            {
                EffectId.CurlNoise,
                "A smooth, swirling, magical-looking distortion that warps the image without tearing it."
            },
            {
                EffectId.FBMNoise,
                "A cloudy, smoky, complex distortion that makes the sprite look like it's dissolving into gas."
            },
            {
                EffectId.Shockwave,
                "Creates a ripple that pushes the sprite outward like a sonic boom or an explosion shockwave."
            },
            { EffectId.SineScale, "Smoothly pulses the entire size of the sprite up and down." },
            { EffectId.Heartbeat, "A sharp, rhythmic double-pulse. Literally mimics a beating heart." },
            {
                EffectId.Breathing,
                "A slow, gentle vertical stretch, simulating a character calmly inhaling and exhaling."
            },
            {
                EffectId.LiquidWobble,
                "A jiggly, gelatinous movement. Essential for animating slimes, jellies, or water elementals."
            },
            {
                EffectId.SmoothPixelArt,
                "A technical trick: keeps pixel art looking incredibly crisp but allows it to rotate and move smoothly without shimmering or jiggling edges."
            },
            {
                EffectId.DialogueEmphasis,
                "A bouncy, joyful up-and-down hop. Used commonly in RPGs when a character is talking excitedly."
            },
            {
                EffectId.UVBendPoint,
                "Sucks the sprite inwards towards a specific point (like a black hole) or pushes it away (like a magnetic repulsion)."
            },

            // FILTER
            {
                EffectId.Sharpen,
                "Enhances the crispness of edges. Good for making slightly blurry assets pop with a bit more definition."
            },
            {
                EffectId.GaussBlur,
                "Blurs the sprite out of focus smoothly. Use for background elements, depth of field, or faking fast movement."
            },
            {
                EffectId.DirBlur,
                "Blurs the sprite only in one specific direction. The classic 'speed lines' motion blur look for fast dashes."
            },
            {
                EffectId.FlipBookBlend,
                "Smoothly crossfades between animation frames to make choppy, low-framerate animations look fluid."
            },
            {
                EffectId.UVScrollOverlay,
                "Slides a secondary texture over the sprite. Good for adding moving TV static, rain, or flowing energy across a character."
            },

            // Dissolve
            {
                EffectId.Dissolve,
                "The classic 'burn away' effect using a noise texture. Leaves a colored, crusty edge where it burns. Perfect for dying enemies."
            },
            {
                EffectId.GlowDissolve,
                "Like normal Dissolve, but the burning edge glows brightly. Great for magical creatures fading away."
            },
            {
                EffectId.SrcDissolve,
                "Erases the sprite starting from a specific point and spreading outward, like a growing hole burning through paper."
            },
            { EffectId.DirFade, "Wipes the sprite away cleanly in a straight line from any angle." },
            {
                EffectId.DirGlowFade,
                "Wipes the sprite away, but leaves a glowing laser-like edge sweeping along the wipe."
            },
            { EffectId.FullAlphaDissolve, "Simply fades the whole sprite to transparent smoothly." },
            { EffectId.Halftone, "Dissolves the sprite into comic-book style dots that shrink away into nothing." },
            {
                EffectId.RadialWipe,
                "Wipes the sprite away in a circle, like a clock hand sweeping around. Good for cooldown timers."
            },
            { EffectId.Blinds, "Slices the sprite into horizontal or vertical strips that smoothly disappear." },
            { EffectId.DiamondDissolve, "Dissolves the sprite into little shrinking diamond or rhombus shapes." },
            { EffectId.ScanlineDissolve, "Dissolves the sprite by erasing horizontal TV scanlines one by one." },
            { EffectId.SpreadFade, "Erases the sprite outward from a center point, leaving soft, smooth edges." },
            {
                EffectId.PixelDissolve,
                "Dissolves the sprite into chunky, 8-bit style blocks. Great for retro teleports or digital deaths."
            },
            { EffectId.SpiralWipe, "Erases the sprite in a swirling galaxy pattern." },
            {
                EffectId.HexDissolve,
                "Dissolves the sprite into a high-tech honeycomb/hexagonal grid. Great for sci-fi shields or holograms."
            },
            {
                EffectId.BurnDirection,
                "A directional wipe that leaves a vivid, flickering fire and charcoal edge as it burns the sprite away."
            },

            // VFX
            {
                EffectId.Hologram,
                "Adds scrolling scanlines, transparency, and a bit of signal jitter for a classic sci-fi hologram look."
            },
            {
                EffectId.Glitch,
                "Slices the sprite horizontally and misaligns the color channels to look like a broken digital broadcast or a cyberpunk error."
            },
            {
                EffectId.Flicker,
                "Flashes the sprite completely invisible and visible rapidly. The classic 'invincibility frames' effect."
            },
            {
                EffectId.Shine,
                "Sweeps a bright, angled glare across the sprite. Good for shiny rare cards, gems, or unsheathing a clean sword."
            },
            { EffectId.Smoke, "Billows an animated, volumetric-looking smoke texture over the sprite." },
            {
                EffectId.Flame,
                "Wraps the sprite in a flickering fire effect. You can choose to show the fire *over* the sprite, or make the sprite *made of* fire."
            },
            { EffectId.Enchanted, "Sprinkles glittering, magical sparkles over the sprite." },
            {
                EffectId.Shifting,
                "Smoothly pulses the sprite's brightness or color between two tones. Good for idle magical items."
            },
            { EffectId.ShiftRainbow, "Washes a smoothly animated rainbow gradient over the sprite's lighting." },
            {
                EffectId.Overlay,
                "Plops any custom texture on top of the sprite and lets you blend it in using scrolling UVs."
            },
            {
                EffectId.VoronoiCrackle,
                "Cracks the sprite like shattered glass, dry earth, or lava rock, leaving glowing fissures between the chunks."
            },
            {
                EffectId.MotionSmear,
                "Stretches the sprite backwards along its movement path to fake fast, snappy animation (animation smearing)."
            },
            {
                EffectId.GlobalSmoke,
                "Adds a layer of smoke that stays consistent across the whole screen, rather than sticking to the sprite's local position."
            },
            {
                EffectId.HolographicCard,
                "Recreates the shiny, iridescent foil look of a rare physical trading card, complete with rainbow angles that shift as it moves."
            },
            {
                EffectId.CloudShadows,
                "Casts soft, moving shadows over the sprite, like clouds passing overhead in a sunny top-down game."
            },
            {
                EffectId.ColorfulTrailA,
                "Leaves a color trail that changes hue based on how fast the sprite is moving."
            },
            {
                EffectId.ColorfulTrailB,
                "Leaves a 'ghosting' trail of fading clones behind the moving sprite. A classic ninja/dash effect."
            },
            {
                EffectId.ColorfulTrailC,
                "Leaves a vibrant, rainbow-colored ribbon trail behind the sprite as it dashes."
            },
            {
                EffectId.ChargingAbility,
                "Draws pulsing energy rings and electric static around the center. Perfect for a 'charging up an ultimate attack' visual."
            },

            // GAMEPLAY / UTILITY
            {
                EffectId.HealthTint,
                "Automatically shifts the sprite's color to a 'danger' tint (like red) as health drops, adding optional panic-inducing visual noise and vignettes at low health."
            },
            {
                EffectId.XRaySilhouette,
                "Shows a flat colored cutout of the sprite *only* when it is hidden behind a wall or obstacle. Essential for keeping track of the player."
            },
            {
                EffectId.MinimapPing,
                "Emits a radar-like expanding ring from the sprite. Good for highlighting objectives or unit locations."
            },
            { EffectId.Ghosting, "Creates persistent fading afterimages that follow the sprite like an echo." },
            {
                EffectId.Checkerboard,
                "Covers the sprite in a racing or chess-board pattern that can lock to the screen or the world."
            },
            {
                EffectId.RetroPalette,
                "Forces the sprite to only use colors from a specific limited palette (like GameBoy or NES colors), optionally adding retro dot-dithering for shading."
            },
            {
                EffectId.UVVisualize,
                "A technical debug tool that shows the underlying 3D math coordinates in Red and Green."
            },
            {
                EffectId.Blueprint,
                "Turns the sprite into a technical drawing, pulling out the edges as sketchy lines on a blueprint grid background."
            },
            {
                EffectId.FakeLighting,
                "Simulates a simple directional light with specular highlights. Great for adding depth to flat sprites without real lights."
            },
            {
                EffectId.WaterReflect,
                "Mirrors the top half of the sprite downward with animated ripples for a water reflection."
            },
            { EffectId.EnergyAura, "Radiates a pulsing, luminous energy field around the sprite." },
            {
                EffectId.Dithering,
                "Adds a retro ordered-dithering pattern over the sprite. Screen-space mode keeps the pattern stable during movement."
            },
            {
                EffectId.CRTScanlines,
                "Applies CRT TV scanlines with optional barrel distortion for a retro monitor look."
            },
            { EffectId.Vignette, "Darkens the edges of the sprite, drawing the eye to the center." },
            { EffectId.ThermalVision, "Remaps the sprite to a heat-map color scheme (cold=blue, hot=white)." },
        };

        private struct SectionStyle
        { public string label; public Color accent; public string icon; }

        private static readonly SectionStyle[] Sections = {
            new SectionStyle{label="Color Effects",   accent=new Color(0.85f,0.30f,0.30f,1), icon=""},
            new SectionStyle{label="Edge Effects",    accent=new Color(0.90f,0.75f,0.25f,1), icon=""},
            new SectionStyle{label="UV / Distortion", accent=new Color(0.30f,0.75f,0.40f,1), icon=""},
            new SectionStyle{label="Dissolve / Fade", accent=new Color(0.65f,0.35f,0.80f,1), icon=""},
            new SectionStyle{label="Special Effects", accent=new Color(0.30f,0.70f,0.85f,1), icon=""},
        };

        // Cached State

        private Dictionary<string, SerializedProperty> _propCache;
        private Dictionary<string, GUIContent> _contentCache;
        private EffectDef[][] _categoryGroups;
        private int[] _categoryEnabledCounts;
        private string _searchFilter = "";
        private string _prevSearchFilter = "";
        private List<EffectDef> _filteredEffects;

        private bool _toolsOpen = true;
        private bool _globalOpen = true;
        private bool[] _sectionOpen = new bool[5];
        private Dictionary<EffectId, bool> _effectFoldouts = new();

        private static GUIStyle _headerStyle;
        private static GUIStyle _sectionLabelStyle;
        private static GUIStyle _subSectionStyle;
        private static GUIStyle _badgeStyle;
        private static GUIStyle _miniButtonStyle;
        private static GUIStyle _searchStyle;
        private static GUIStyle _activeStripStyle;
        private static GUIStyle _perfDotStyle;
        private static GUIStyle _codeButtonStyle;

        // Preview state
        private Dictionary<EffectId, Texture2D> _previewCache = new();

        private Dictionary<EffectId, bool> _previewDirty = new();
        private Material _previewMaterial;
        private const int PREVIEW_SIZE = 144;
        private static readonly GUIContent KeyButtonContent = new GUIContent("K", "Open keyframe editor for this parameter");
        private double _previewDebounceTime;
        private const double PREVIEW_DEBOUNCE_DELAY = 0.2;

        // Copy/paste
        private static Dictionary<string, object> _clipboard;

        private static EffectId? _clipboardEffectId;

        private AnimatedSpriteShader Mgr => (AnimatedSpriteShader)target;

        // Lifecycle

        private void OnEnable()
        {
            RebuildPropertyCache();
            RebuildCategoryGroups();
            EditorApplication.update -= OnEditorPreviewUpdate;
            EditorApplication.update += OnEditorPreviewUpdate;
        }

        private void OnDisable()
        {
            CleanupPreviews();
            // Strip any pending Repaint delayCalls we may have queued. Without this, if the user
            // selects a different object while a delayed repaint is pending, Unity will invoke
            // Repaint() on a disposed Editor instance and log a "SerializedObject has been
            // disposed" exception. Unsubscribing both endpoints is safe because delayCall uses
            // a multicast delegate — and removing a non-subscribed handler is a no-op.
            EditorApplication.delayCall -= Repaint;
            EditorApplication.update -= OnEditorPreviewUpdate;
        }

        private void OnEditorPreviewUpdate()
        {
            if (target == null)
                return;

            if (!HasOpenPreview())
                return;

            foreach (var kvp in _effectFoldouts)
            {
                if (kvp.Value)
                    _previewDirty[kvp.Key] = true;
            }

            // Animated previews are visual-only. Do not apply serialized properties or mark the
            // component dirty here; just repaint the inspector so the preview material gets a new
            // editor-time value.
            _previewDebounceTime = 0;
            Repaint();
        }

        private bool HasOpenPreview()
        {
            foreach (var kvp in _effectFoldouts)
            {
                if (kvp.Value)
                    return true;
            }

            return false;
        }

        private void RebuildPropertyCache()
        {
            _propCache = new Dictionary<string, SerializedProperty>(512);
            _contentCache = new Dictionary<string, GUIContent>(512);

            foreach (var def in EffectRegistry.All)
            {
                CacheProp(def.enableField);
                CacheProp(def.maskField);
                foreach (var p in def.parms)
                {
                    CacheProp(p.field);
                    if (!_contentCache.ContainsKey(p.field))
                        _contentCache[p.field] = new GUIContent(p.display, $"{p.display}\nField: {p.field}\nShader: {p.shader}");
                }
            }

            string[] globals = {"_shaderVariant","_materialMode","globalEffectFade","edgeCleanup",
                "effectMaskEnabled",
                "effectMaskTexture","pixelArtSnapEnabled","pixelSnapResolution","timeScale","timeFPS",
                "timeMode","useCustomTime","customTime","outlineEffectsOnly","outlineEffectWidth",
                "masksEnabled","maskR","maskG","maskB","maskA","packedMasks",
                "premulAlpha","spriteFlipX","spriteFlipY","dissolveNoiseSpeed","lutSize",
                "stencilEnabled","stencilRef","stencilComp","stencilPass","stencilReadMask","stencilWriteMask",
                "alphaCutoffEnabled","alphaCutoff","outlineSDFAlphaThreshold",
                "castShadows","receiveShadows","shadowAlphaClip","vertexUVExpand","twoSidedLighting",
                "trailVelocitySource","smearVelocitySource","motionSmearEnabled",
                "trailAVelocity","trailBVelocity","trailCVelocity","smearDirection",
                "velocityDecay",
                "outlineMode","outlineRings","outlineAA","outlineFixedWidthPx"};
            foreach (var g in globals) CacheProp(g);
        }

        private void CacheProp(string name)
        {
            if (!_propCache.ContainsKey(name))
            {
                var sp = serializedObject.FindProperty(name);
                if (sp != null) _propCache[name] = sp;
            }
        }

        private SerializedProperty Prop(string name)
        {
            if (_propCache != null && _propCache.TryGetValue(name, out var p)) return p;
            return serializedObject.FindProperty(name);
        }

        private GUIContent Content(string field)
        {
            if (_contentCache != null && _contentCache.TryGetValue(field, out var c)) return c;
            return GUIContent.none;
        }

        private void RebuildCategoryGroups()
        {
            _categoryGroups = new EffectDef[5][];
            _categoryEnabledCounts = new int[5];
            var buckets = new List<EffectDef>[5];
            for (int i = 0; i < 5; i++) buckets[i] = new List<EffectDef>();

            bool usedSettings = false;
            try
            {
                var settings = AnimSpriteProjectSettings.Instance;
                if (settings != null)
                {
                    settings.EnsureGroups();
                    var groups = settings.Groups;
                    if (groups != null && groups.Count > 0)
                    {
                        // Sort by renderOrder — same as MasterSetup does
                        var sorted = new List<int>(groups.Count);
                        for (int i = 0; i < groups.Count; i++) sorted.Add(i);
                        sorted.Sort((a, b) => groups[a].renderOrder.CompareTo(groups[b].renderOrder));

                        var added = new HashSet<EffectId>();

                        foreach (int gi in sorted)
                        {
                            var gc = groups[gi];
                            var ordered = settings.GetOrderedEffects(gc.group);
                            if (ordered == null) continue;
                            foreach (var def in ordered)
                            {
                                if (added.Add(def.id))
                                    buckets[(int)def.category].Add(def);
                            }
                        }

                        // Safety net: add any effects not covered by groups
                        foreach (var def in EffectRegistry.All)
                        {
                            if (added.Add(def.id))
                                buckets[(int)def.category].Add(def);
                        }

                        usedSettings = true;
                    }
                }
            }
            catch (Exception)
            {
                // AnimSpriteProjectSettings missing or API changed — fall back gracefully
                usedSettings = false;
            }

            if (!usedSettings)
            {
                for (int i = 0; i < 5; i++) buckets[i].Clear();
                foreach (var def in EffectRegistry.All)
                    buckets[(int)def.category].Add(def);
            }

            for (int i = 0; i < 5; i++)
                _categoryGroups[i] = buckets[i].ToArray();
        }

        private void UpdateEnabledCounts()
        {
            if (_categoryGroups == null || _categoryEnabledCounts == null)
                RebuildCategoryGroups();
            if (_propCache == null)
                RebuildPropertyCache();

            for (int i = 0; i < 5; i++)
            {
                int count = 0;
                foreach (var def in _categoryGroups[i])
                {
                    var p = Prop(def.enableField);
                    if (p != null && p.boolValue) count++;
                }
                _categoryEnabledCounts[i] = count;
            }
        }

        private void InitStyles()
        {
            // Guard on the style reference rather than a separate bool. Unity's domain-reload cycle
            // can leave the bool as 'true' while the style references themselves are null (because
            // EditorStyles are torn down and rebuilt across reloads). Using the style itself as the
            // sentinel means we re-init whenever the references have gone stale.
            if (_headerStyle != null) return;
            _headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 12,
                normal = { textColor = new Color(0.50f, 0.50f, 0.55f) }
            };

            _sectionLabelStyle = new GUIStyle(EditorStyles.boldLabel) { fontSize = 13 };

            _subSectionStyle = new GUIStyle(EditorStyles.boldLabel) { fontSize = 12, normal = { textColor = new Color(0.72f, 0.72f, 0.8f) } };

            _badgeStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 9,
                normal = { textColor = Color.white },
                padding = new RectOffset(4, 4, 1, 1)
            };

            _miniButtonStyle = new GUIStyle(EditorStyles.miniButton)
            {
                fontSize = 9,
                fixedHeight = 16,
                fixedWidth = 24,
                padding = new RectOffset(2, 2, 1, 1)
            };

            _searchStyle = new GUIStyle(EditorStyles.toolbarSearchField);

            _activeStripStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                wordWrap = true,
                normal = { textColor = new Color(0.5f, 0.7f, 0.5f) },
                fontSize = 9
            };

            _perfDotStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 10,
                fixedWidth = 14,
                fixedHeight = 14,
                alignment = TextAnchor.MiddleCenter,
                padding = new RectOffset(0, 0, 0, 0)
            };

            _codeButtonStyle = new GUIStyle(EditorStyles.miniButton)
            {
                fontSize = 9,
                fixedHeight = 18,
                padding = new RectOffset(4, 4, 1, 1)
            };
        }

        // Main Draw

        public override void OnInspectorGUI()
        {
            InitStyles();
            serializedObject.Update();
            UpdateEnabledCounts();

            DrawHeaderFS();
            EditorGUILayout.Space(2);
            DrawActiveEffectsSummary();
            EditorGUILayout.Space(2);
            DrawSearchBar();
            EditorGUILayout.Space(2);
            DrawGlobalCodeButton();
            EditorGUILayout.Space(2);
            DrawTools();
            EditorGUILayout.Space(2);
            DrawSystem();
            EditorGUILayout.Space(2);
            DrawGlobal();
            EditorGUILayout.Space(4);

            // REMOVED DrawTrailVelocity() from here!

            bool hasFilter = !string.IsNullOrEmpty(_searchFilter);

            if (hasFilter)
            {
                RebuildFilteredEffects();
                BeginDarkBox();

                // Draw here if searching (it auto-hides if no trails are active)
                DrawTrailVelocity();

                if (_filteredEffects.Count == 0)
                    EditorGUILayout.LabelField("No effects match \"" + _searchFilter + "\"", EditorStyles.centeredGreyMiniLabel);
                else
                    foreach (var def in _filteredEffects)
                        DrawEffect(def);
                EndDarkBox();
            }
            else
            {
                for (int s = 0; s < 5; s++)
                {
                    DrawSectionHeader(s);
                    if (_sectionOpen[s])
                    {
                        BeginDarkBox();

                        if (s == 1)
                        {
                            DrawGlobalOutlineSamplingControls();
                            EditorGUILayout.Space(4);
                        }

                        if (s == 4)
                        {
                            DrawTrailVelocity();
                        }

                        // Grid layout: collapsed effects flow in rows, expanded effects take full width
                        DrawEffectGrid(_categoryGroups[s]);
                        EndDarkBox();
                    }
                    EditorGUILayout.Space(2);
                }
            }

            DrawRemoveButton();
            if (serializedObject.ApplyModifiedProperties())
                Mgr.MarkDirty();
        }

        // Header + Keyword Budget

        private void DrawHeaderFS()
        {
            var r = GUILayoutUtility.GetRect(0, 36, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(r, HeaderBg);
            GUI.Label(r, "Animated Sprite Shader System", _headerStyle);
        }


        private void DrawActiveEffectsSummary()
        {
            var sb = new StringBuilder();
            int total = 0;
            foreach (var def in EffectRegistry.All)
            {
                var p = Prop(def.enableField);
                if (p != null && p.boolValue)
                {
                    if (sb.Length > 0) sb.Append("  \u2022  ");
                    sb.Append(def.name);
                    total++;
                }
            }

            if (total == 0) return;

            var r = GUILayoutUtility.GetRect(0, 14, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(r, new Color(0.1f, 0.12f, 0.1f));

            var style = _activeStripStyle;
            GUI.Label(new Rect(r.x + 4, r.y, 30, r.height), $"[{total}]", EditorStyles.centeredGreyMiniLabel);
            GUI.Label(new Rect(r.x + 32, r.y, r.width - 36, r.height), sb.ToString(), style);
        }

        // Search Bar

        private void DrawSearchBar()
        {
            EditorGUILayout.Space(2);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("\U0001F50D", GUILayout.Width(20), GUILayout.Height(22));
            _searchFilter = EditorGUILayout.TextField(_searchFilter, _searchStyle, GUILayout.Height(22));
            if (GUILayout.Button("X", EditorStyles.miniButton, GUILayout.Width(24), GUILayout.Height(22)))
                _searchFilter = "";
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(2);
        }

        private void RebuildFilteredEffects()
        {
            if (_searchFilter == _prevSearchFilter && _filteredEffects != null) return;
            _prevSearchFilter = _searchFilter;

            var filter = _searchFilter.ToLowerInvariant();
            _filteredEffects = new List<EffectDef>();
            foreach (var def in EffectRegistry.All)
            {
                if (def.name.ToLowerInvariant().Contains(filter) ||
                    def.keyword.ToLowerInvariant().Contains(filter) ||
                    def.category.ToString().ToLowerInvariant().Contains(filter))
                    _filteredEffects.Add(def);
            }
        }

        // Global Code Button

        private void DrawGlobalCodeButton()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("[C#] Copy All Active Settings", GUILayout.Width(240), GUILayout.Height(24)))
                CopyAllActiveSettingsToClipboard();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private void CopyAllActiveSettingsToClipboard()
        {
            var sb = new StringBuilder();
            sb.AppendLine("// AnimatedSpriteShader settings — generated");
            sb.AppendLine("var mgr = GetComponent<AnimatedSpriteShader>();");

            foreach (var def in EffectRegistry.All)
            {
                var ep = Prop(def.enableField);
                if (ep == null || !ep.boolValue) continue;

                sb.AppendLine($"mgr.{def.enableField} = true;");
                foreach (var p in def.parms)
                {
                    var sp = Prop(p.field);
                    if (sp == null) continue;
                    sb.AppendLine($"mgr.{p.field} = {FormatPropertyForCode(sp, p.type)};");
                }
                var mp = Prop(def.maskField);
                if (mp != null && mp.intValue > 0)
                    sb.AppendLine($"mgr.{def.maskField} = {mp.intValue};");
                sb.AppendLine();
            }

            GUIUtility.systemCopyBuffer = sb.ToString();
            Debug.Log("[AnimSprite] All active effect settings copied to clipboard.");
        }

        private string FormatPropertyForCode(SerializedProperty sp, ParamType type)
        {
        
            if (sp.propertyType == SerializedPropertyType.Boolean)
                return sp.boolValue ? "true" : "false";

            switch (type)
            {
                case ParamType.Float: return sp.floatValue.ToString("G") + "f";
                case ParamType.Int: return sp.intValue.ToString();
                case ParamType.Color:
                case ParamType.HDRColor:
                    var c = sp.colorValue;
                    return $"new Color({c.r:F3}f,{c.g:F3}f,{c.b:F3}f,{c.a:F3}f)";

                case ParamType.Vector2:
                    var v2 = sp.vector2Value;
                    return $"new Vector2({v2.x:F3}f,{v2.y:F3}f)";

                case ParamType.Vector3:
                    var v3 = sp.vector3Value;
                    return $"new Vector3({v3.x:F3}f,{v3.y:F3}f,{v3.z:F3}f)";

                case ParamType.Vector4:
                    var v = sp.vector4Value;
                    return $"new Vector4({v.x:F3}f,{v.y:F3}f,{v.z:F3}f,{v.w:F3}f)";

                case ParamType.Texture:
                    return sp.objectReferenceValue != null ? $"/* {sp.objectReferenceValue.name} */" : "null";

                default: return "0";
            }
        }

        // Section Headers

        // Section render order (actual shader dispatch order)
        // UV/Filter → Color/ColorExt → Edge → Dissolve → VFX/Game
        private static readonly int[] _sectionRenderOrder = { 2, 3, 1, 4, 5 }; // sections 0-4
        private static GUIStyle _orderBadgeStyle;

        private static void DrawStableFoldoutTriangle(Rect rect, bool open, Color color)
        {
            if (Event.current.type != EventType.Repaint)
                return;

            float x = rect.x;
            float y = rect.y;
            float w = rect.width;
            float h = rect.height;

            Vector3[] points;
            if (open)
            {
                points = new[]
                {
                    new Vector3(x + 1f, y + 3f, 0f),
                    new Vector3(x + w - 1f, y + 3f, 0f),
                    new Vector3(x + w * 0.5f, y + h - 2f, 0f)
                };
            }
            else
            {
                points = new[]
                {
                    new Vector3(x + 3f, y + 1f, 0f),
                    new Vector3(x + 3f, y + h - 1f, 0f),
                    new Vector3(x + w - 2f, y + h * 0.5f, 0f)
                };
            }

            Handles.BeginGUI();
            Color prev = Handles.color;
            Handles.color = color;
            Handles.DrawAAConvexPolygon(points);
            Handles.color = prev;
            Handles.EndGUI();
        }

        private static bool DrawStableFoldoutLabel(Rect rect, bool open, string label, GUIStyle labelStyle, Color triangleColor)
        {
            Rect triangleRect = new Rect(rect.x + 2f, rect.y + Mathf.Max(0f, (rect.height - 10f) * 0.5f), 10f, 10f);
            DrawStableFoldoutTriangle(triangleRect, open, triangleColor);

            Rect labelRect = new Rect(rect.x + 17f, rect.y, Mathf.Max(0f, rect.width - 17f), rect.height);
            GUI.Label(labelRect, label, labelStyle);

            if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
            {
                Event.current.Use();
                return !open;
            }

            return open;
        }

        private void DrawSectionHeader(int idx)
        {
            var sec = Sections[idx];
            int count = _categoryEnabledCounts[idx];

            var r = GUILayoutUtility.GetRect(0, 24, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(r, SectionBg);
            EditorGUI.DrawRect(new Rect(r.x, r.y, 3, r.height), sec.accent);

            Rect foldoutRect = new Rect(r.x + 8f, r.y + 7f, 10f, 10f);
            DrawStableFoldoutTriangle(foldoutRect, _sectionOpen[idx], sec.accent);

            _sectionLabelStyle.normal.textColor = sec.accent;
            GUI.Label(new Rect(r.x + 26, r.y, r.width - 138, r.height), $"{sec.icon} {sec.label}", _sectionLabelStyle);

            // Render order badge (cached style)
            if (_orderBadgeStyle == null)
                _orderBadgeStyle = new GUIStyle(EditorStyles.miniLabel)
                {
                    alignment = TextAnchor.MiddleCenter,
                    normal = { textColor = new Color(0.5f, 0.5f, 0.55f) }
                };
            GUI.Label(new Rect(r.xMax - 80, r.y, 36, r.height), $"#{_sectionRenderOrder[idx]}", _orderBadgeStyle);

            if (count > 0)
            {
                var badgeRect = new Rect(r.xMax - 36, r.y + 3, 28, 16);
                EditorGUI.DrawRect(badgeRect, sec.accent * 0.6f);
                GUI.Label(badgeRect, count.ToString(), _badgeStyle);
            }

            if (Event.current.type == EventType.MouseDown && r.Contains(Event.current.mousePosition))
            {
                _sectionOpen[idx] = !_sectionOpen[idx];
                Event.current.Use();
            }
        }

        // Effect Drawing

        private static GUIStyle _effectLabelOpen;
        private static GUIStyle _effectLabelClosed;

        private void DrawGlobalOutlineSamplingControls()
        {
            var modeProp = Prop("outlineMode");
            if (modeProp == null) return;

            var ringsProp = Prop("outlineRings");
            var aaProp = Prop("outlineAA");
            var sdfTexProp = Prop("outlineSDFTex");
            var sdfSoftProp = Prop("outlineSDFSoftness");

            var r = GUILayoutUtility.GetRect(0, 18, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(r, new Color(0.12f, 0.12f, 0.14f, 1));
            GUI.Label(new Rect(r.x + 6, r.y, r.width - 12, r.height), "Outline Sampling (Global)", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;

            modeProp.intValue = EditorGUILayout.Popup(
                new GUIContent("Style"),
                Mathf.Clamp(modeProp.intValue, 0, OutlineStyleLabels.Length - 1),
                OutlineStyleLabels);

            int mode = modeProp.intValue;

            if (mode == 1)
            {
                if (ringsProp != null) EditorGUILayout.PropertyField(ringsProp, new GUIContent("Rings"));
                if (aaProp != null) EditorGUILayout.PropertyField(aaProp, new GUIContent("AA"));
            }
            else if (mode == 2)
            {
                if (sdfTexProp != null) EditorGUILayout.PropertyField(sdfTexProp, new GUIContent("SDF Texture"));
                if (sdfSoftProp != null) EditorGUILayout.PropertyField(sdfSoftProp, new GUIContent("Edge Softness"));

                var threshProp = Prop("outlineSDFAlphaThreshold");
                if (threshProp != null)
                {
                    EditorGUILayout.PropertyField(
                        threshProp,
                        new GUIContent(
                            "Bake Alpha Threshold",
                            "Pixels above this alpha are treated as solid when baking. Lower = include semi-transparent pixels."));
                }

                if (sdfTexProp != null && sdfTexProp.objectReferenceValue is Texture2D tex && tex != null)
                {
                    var pr = GUILayoutUtility.GetRect(0, 68, GUILayout.ExpandWidth(true));
                    var texRect = new Rect(pr.x + 6, pr.y + 6, 56, 56);
                    EditorGUI.DrawRect(new Rect(pr.x + 4, pr.y + 4, 60, 60), new Color(0.08f, 0.08f, 0.10f, 1));
                    EditorGUI.DrawPreviewTexture(texRect, tex, null, ScaleMode.ScaleToFit);
                }

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Bake SDF Texture", GUILayout.Width(130)))
                {
                    Undo.RecordObject(Mgr, "Bake SDF");
                    var sdf = Mgr.BakeOutlineSDF();
                    if (sdf != null)
                    {
                        var path = EditorUtility.SaveFilePanelInProject("Save SDF", "OutlineSDF", "exr", "Save");
                        if (!string.IsNullOrEmpty(path))
                        {
                            System.IO.File.WriteAllBytes(path, sdf.EncodeToEXR(Texture2D.EXRFlags.OutputAsFloat));
                            AssetDatabase.ImportAsset(path);
                            var ti = AssetImporter.GetAtPath(path) as TextureImporter;
                            if (ti != null)
                            {
                                ti.sRGBTexture = false;
                                ti.textureCompression = TextureImporterCompression.Uncompressed;
                                var ps = ti.GetDefaultPlatformTextureSettings();
                                ps.format = TextureImporterFormat.RFloat;
                                ti.SetPlatformTextureSettings(ps);
                                ti.SaveAndReimport();
                            }
                            var imported = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                            if (imported)
                            {
                                Mgr.outlineSDFTex = imported;
                                EditorUtility.SetDirty(Mgr);
                                serializedObject.Update();
                            }
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.HelpBox("Bake a signed distance field from your sprite.\nSet Sprite Mesh Type to 'Full Rect' for outlines to extend beyond the sprite edges.\nLower Alpha Threshold to include semi-transparent pixels as solid.", MessageType.Info);
            }

            EditorGUI.indentLevel--;

            // Divider
            var div = GUILayoutUtility.GetRect(0, 1, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(div, new Color(0.18f, 0.18f, 0.20f, 1));
        }

        // Grid layout: collapsed effects in rows, expanded effects full-width
        private void DrawEffectGrid(EffectDef[] defs)
        {
            float inspectorW = EditorGUIUtility.currentViewWidth - 30;
            float cellW = Mathf.Max(160, inspectorW / Mathf.Floor(inspectorW / 170)); // ~170px per cell
            bool inRow = false;
            int colIdx = 0;
            int maxCols = Mathf.Max(1, Mathf.FloorToInt(inspectorW / cellW));

            for (int i = 0; i < defs.Length; i++)
            {
                var def = defs[i];
                var enableProp = Prop(def.enableField);
                if (enableProp == null) continue;

                bool isEnabled = enableProp.boolValue;
                bool isOpen = isEnabled && _effectFoldouts.ContainsKey(def.id) && _effectFoldouts[def.id];

                if (isOpen)
                {
                    // End current row if needed
                    if (inRow) { EditorGUILayout.EndHorizontal(); inRow = false; colIdx = 0; }
                    // Draw full-width expanded effect
                    DrawEffect(def);
                }
                else
                {
                    // Start new row if needed
                    if (!inRow) { EditorGUILayout.BeginHorizontal(); inRow = true; colIdx = 0; }

                    // Compact cell
                    DrawEffectCompact(def, cellW);
                    colIdx++;

                    // Wrap row
                    if (colIdx >= maxCols)
                    {
                        EditorGUILayout.EndHorizontal();
                        inRow = false;
                        colIdx = 0;
                    }
                }
            }

            // Close any open row
            if (inRow) { GUILayout.FlexibleSpace(); EditorGUILayout.EndHorizontal(); }
        }

        // Cached styles for grid cells (prevents GC alloc every frame)
        private static GUIStyle _compactLabelEnabled;
        private static GUIStyle _compactLabelDisabled;
        private static GUIStyle _tipStyle;

        private void DrawEffectCompact(EffectDef def, float width)
        {
            var enableProp = Prop(def.enableField);
            if (enableProp == null) return;

            if (!_effectFoldouts.ContainsKey(def.id))
                _effectFoldouts[def.id] = false;

            // Cache styles once
            if (_compactLabelEnabled == null)
            {
                _compactLabelEnabled = new GUIStyle(EditorStyles.label)
                {
                    fontSize = 12,
                    normal = { textColor = new Color(0.98f, 0.96f, 0.97f) },
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleLeft
                };
                _compactLabelDisabled = new GUIStyle(EditorStyles.label)
                {
                    fontSize = 12,
                    normal = { textColor = new Color(0.82f, 0.82f, 0.86f) },
                    alignment = TextAnchor.MiddleLeft
                };
            }

            var rect = GUILayoutUtility.GetRect(width - 4, 20, GUILayout.Width(width - 4));

            bool isEnabled = enableProp.boolValue;
            Color bgCol = isEnabled ? new Color(0.22f, 0.25f, 0.28f) : new Color(0.16f, 0.16f, 0.18f);
            EditorGUI.DrawRect(rect, bgCol);

            // Perf dot
            int perf = EffectCategorization.GetPerfLevel(def);
            Color dotCol = perf == 2 ? new Color(0.9f, 0.3f, 0.3f) : perf == 1 ? new Color(0.9f, 0.75f, 0.25f) : new Color(0.3f, 0.75f, 0.4f);
            EditorGUI.DrawRect(new Rect(rect.x + 2, rect.y + 7, 6, 6), dotCol);

            // Toggle
            EditorGUI.BeginChangeCheck();
            bool nowEnabled = EditorGUI.Toggle(new Rect(rect.x + 12, rect.y + 2, 16, 16), isEnabled);
            if (EditorGUI.EndChangeCheck())
            {
                enableProp.boolValue = nowEnabled;
                if (nowEnabled && !isEnabled)
                    _effectFoldouts[def.id] = true;
            }

            // Name label (clickable to expand)
            var labelRect = new Rect(rect.x + 30, rect.y, rect.width - 34, rect.height);
            if (GUI.Button(labelRect, def.name, isEnabled ? _compactLabelEnabled : _compactLabelDisabled))
            {
                if (isEnabled)
                    _effectFoldouts[def.id] = !_effectFoldouts[def.id];
            }
        }

        private void DrawEffect(EffectDef def)
        {
            var enableProp = Prop(def.enableField);
            if (enableProp == null) return;

            if (!_effectFoldouts.ContainsKey(def.id))
                _effectFoldouts[def.id] = false;

            if (_effectLabelOpen == null)
            {
                _effectLabelOpen = new GUIStyle(EditorStyles.boldLabel)
                {
                    normal = { textColor = new Color(0.95f, 0.97f, 1f) },
                    fixedHeight = 20
                };
                _effectLabelClosed = new GUIStyle(EditorStyles.label)
                {
                    normal = { textColor = new Color(0.78f, 0.78f, 0.80f) },
                    fixedHeight = 20
                };
            }

            EditorGUILayout.Space(1);

            EditorGUILayout.BeginHorizontal();

            DrawPerfDotLayout(def);

            EditorGUI.BeginChangeCheck();
            bool wasEnabled = enableProp.boolValue;
            Rect toggleArea = GUILayoutUtility.GetRect(16, 20, GUILayout.Width(16), GUILayout.Height(20));
            bool nowEnabled = EditorGUI.Toggle(new Rect(toggleArea.x, toggleArea.y + 2f, 16f, 16f), wasEnabled);
            if (EditorGUI.EndChangeCheck())
            {
                enableProp.boolValue = nowEnabled;
                if (nowEnabled && !wasEnabled)
                {
                    _effectFoldouts[def.id] = true;
                    MarkPreviewDirty(def.id);
                }
            }

            if (nowEnabled)
            {
                bool wasOpen = _effectFoldouts[def.id];
                Rect labelArea = GUILayoutUtility.GetRect(80f, 20f, GUILayout.ExpandWidth(true), GUILayout.Height(20));
                _effectFoldouts[def.id] = DrawStableFoldoutLabel(
                    labelArea,
                    wasOpen,
                    def.name,
                    wasOpen ? _effectLabelOpen : _effectLabelClosed,
                    (wasOpen ? _effectLabelOpen : _effectLabelClosed).normal.textColor);

                if (GUILayout.Button("C#", _codeButtonStyle, GUILayout.Width(36)))
                    CopyEffectCode(def);
                if (GUILayout.Button("Copy", _codeButtonStyle, GUILayout.Width(38)))
                    CopyEffectSettings(def);
                if (_clipboardEffectId == def.id)
                    if (GUILayout.Button("Paste", _codeButtonStyle, GUILayout.Width(40)))
                        PasteEffectSettings(def);
                if (GUILayout.Button("Reset", _codeButtonStyle, GUILayout.Width(40)))
                    ResetEffect(def);
            }
            else
            {
                GUILayout.Label(def.name, _effectLabelClosed, GUILayout.Height(20), GUILayout.ExpandWidth(true));
            }

            EditorGUILayout.EndHorizontal();

            if (!nowEnabled || !_effectFoldouts[def.id]) return;

            EditorGUI.indentLevel++;

            // Per-effect usage tooltip
            if (_effectTooltips.TryGetValue(def.id, out string tip))
            {
                if (_tipStyle == null)
                {
                    _tipStyle = new GUIStyle(EditorStyles.helpBox) { fontSize = 10, wordWrap = true };
                    _tipStyle.normal.textColor = new Color(0.65f, 0.65f, 0.70f);
                }
                EditorGUILayout.LabelField(tip, _tipStyle);
            }

            float inspectorWidth = EditorGUIUtility.currentViewWidth;
            bool showPreview = inspectorWidth > 400;

            if (showPreview)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical();
            }

            DrawEffectPreParams(def);
            DrawEffectParams(def);

            DrawEffectSpecialButtons(def);

            if (showPreview)
            {
                EditorGUILayout.EndVertical();
                GUILayout.Space(4);
                EditorGUILayout.BeginVertical(GUILayout.Width(PREVIEW_SIZE + 4));
                DrawEffectPreview(def);
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }

            EditorGUI.indentLevel--;
        }

        private void DrawEffectSpecialButtons(EffectDef def)
        {
            if (def.id == EffectId.Wind)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Auto Bottom", GUILayout.Width(90)))
                {
                    Sprite sourceSprite = Mgr.GetActiveSprite();
                    if (sourceSprite)
                    {
                        Undo.RecordObject(Mgr, "Auto Bottom");
                        float bottomY = FindLowestOpaquePixelY(sourceSprite);
                        Mgr.windBottomY = bottomY;
                        serializedObject.Update();
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            if (def.id == EffectId.Pixelate)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Snap Pixel Perfect", GUILayout.Width(120)))
                {
                    var sr = Mgr.GetComponent<SpriteRenderer>();
                    if (sr && sr.sprite)
                    {
                        Undo.RecordObject(Mgr, "Pixel Perfect");
                        int texW = sr.sprite.texture.width;
                        Mgr.pixelateSize = Mathf.Max(4, texW);
                        serializedObject.Update();
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            if (def.id == EffectId.PaletteSwap)
            {
                EditorGUILayout.Space(2);
                DrawPaletteSwapDiagram();
            }

            if (def.id == EffectId.SmoothPixelArt)
            {
                EditorGUILayout.Space(2);
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Setup Sub-Pixel Filtering", GUILayout.Width(170)))
                {
                    Mgr.SetupSubPixelFiltering();
                    serializedObject.Update();
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.HelpBox(
                    "Sub-Pixel Pixel Art (t3ssel8r/Colton Ogden method):\n" +
                    "• Requires texture filter set to Bilinear (button above does this)\n" +
                    "• Uses fwidth() to compute precise texel-space snapping\n" +
                    "• Preserves pixel edges while allowing smooth sub-pixel movement/rotation\n" +
                    "• Also call SetupPixelPerfect() from script to auto-set texel size", MessageType.Info);
            }

            // Color Ramp / Gradient / Radial Gradient editors moved to DrawEffectPreParams
        }

        private void DrawPaletteSwapDiagram()
        {
            var r = GUILayoutUtility.GetRect(0, 48, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(r, new Color(0.08f, 0.08f, 0.10f));

            float x = r.x + 4;
            float y = r.y + 4;
            float cellW = Mathf.Min(20, (r.width - 8) / 8);
            float cellH = 18;

            GUI.Label(new Rect(x, y, 60, 12), "Source", EditorStyles.miniLabel);
            GUI.Label(new Rect(x, y + cellH + 2, 60, 12), "Target", EditorStyles.miniLabel);

            Color[] demo = {
                Color.red,
                new Color(1, 0.5f, 0),
                Color.yellow,
                Color.green,
                Color.cyan,
                Color.blue,
                new Color(0.5f, 0, 1),
                Color.magenta,
            };
            for (int i = 0; i < 8; i++)
            {
                var cellRect = new Rect(x + 50 + i * (cellW + 2), y, cellW, cellH - 2);
                EditorGUI.DrawRect(cellRect, demo[i]);
                var targetRect = new Rect(x + 50 + i * (cellW + 2), y + cellH + 2, cellW, cellH - 2);
                EditorGUI.DrawRect(targetRect, Color.HSVToRGB(i / 8f, 0.5f, 0.8f));
            }
            GUI.Label(
                new Rect(x, r.yMax - 12, r.width - 8, 12),
                "Palette: horizontal 1D strip ─ source pixel (0..7) → target pixel (0..7)",
                EditorStyles.centeredGreyMiniLabel);
        }

        private float FindLowestOpaquePixelY(Sprite sprite)
        {
            var tex = sprite.texture;
            if (!tex.isReadable) return 0;
            var pixels = tex.GetPixels32();
            int w = tex.width;
            int h = tex.height;
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    if (pixels[y * w + x].a > 10)
                        return (float)y / h;
            return 0;
        }

        private void DrawPerfDotLayout(EffectDef def)
        {
            int level = EffectCategorization.GetPerfLevel(def);
            Color col = level == 2 ? PerfRed : level == 1 ? PerfYellow : PerfGreen;

            var r = GUILayoutUtility.GetRect(10, 10, GUILayout.Width(10), GUILayout.Height(18));
            var dotRect = new Rect(r.x, r.y + 5, 8, 8);
            EditorGUI.DrawRect(dotRect, col);
        }

        // Fields to skip in DrawEffectParams for certain effects (hidden by gradient editor etc.)
        private static readonly HashSet<string> _skipFields = new HashSet<string>();

        /// <summary>Draws gradient editors ABOVE param fields for effects that need them.</summary>
        private void DrawEffectPreParams(EffectDef def)
        {
            _skipFields.Clear();

            // Color Ramp: gradient on top, hide texture + procedural colors
            if (def.id == EffectId.ColorRamp)
            {
                _skipFields.Add("colorRampTexture");
                _skipFields.Add("colorRampColorA");
                _skipFields.Add("colorRampColorB");

                EditorGUI.BeginChangeCheck();
                var gradProp = serializedObject.FindProperty("colorRampGradient");
                if (gradProp != null)
                    EditorGUILayout.PropertyField(gradProp, new GUIContent("Gradient"));
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    Undo.RecordObject(Mgr, "Bake Color Ramp Gradient");
                    Mgr.colorRampBakedGradient = AnimatedSpriteShader.BakeGradientToTexture(Mgr.colorRampGradient, Mgr.colorRampBakedGradient);
                    Mgr.colorRampTexture = Mgr.colorRampBakedGradient;
                    Mgr.colorRampColorA = Mgr.colorRampGradient.Evaluate(0f);
                    Mgr.colorRampColorB = Mgr.colorRampGradient.Evaluate(1f);
                    EditorUtility.SetDirty(Mgr);
                    MarkPreviewDirty(def.id);
                }
                // Auto-rebake if texture was lost (domain reload, scene reload)
                if (Mgr.colorRampEnabled && Mgr.colorRampBakedGradient == null && Mgr.colorRampGradient.colorKeys.Length > 0)
                {
                    Mgr.colorRampBakedGradient = AnimatedSpriteShader.BakeGradientToTexture(Mgr.colorRampGradient, null);
                    Mgr.colorRampTexture = Mgr.colorRampBakedGradient;
                }
                EditorGUILayout.Space(2);
            }

            // Gradient: checkbox + gradient on top, bake to texture, blend mode
            if (def.id == EffectId.Gradient)
            {
                _skipFields.Add("gradientUseGradientEditor");
                _skipFields.Add("gradientBlendMode");
                _skipFields.Add("gradientBakedTexture");
                var useGrad = serializedObject.FindProperty("gradientUseGradientEditor");
                if (useGrad != null)
                {
                    EditorGUILayout.PropertyField(useGrad, new GUIContent("Use Gradient Editor"));
                    if (useGrad.boolValue)
                    {
                        _skipFields.Add("gradientColorA");
                        _skipFields.Add("gradientColorB");
                        EditorGUI.BeginChangeCheck();
                        var gradProp = serializedObject.FindProperty("gradientGradient");
                        if (gradProp != null)
                            EditorGUILayout.PropertyField(gradProp, new GUIContent("Gradient"));
                        if (EditorGUI.EndChangeCheck())
                        {
                            serializedObject.ApplyModifiedProperties();
                            Undo.RecordObject(Mgr, "Bake Gradient Texture");
                            Mgr.gradientBakedTexture = AnimatedSpriteShader.BakeGradientToTexture(Mgr.gradientGradient, Mgr.gradientBakedTexture);
                            Mgr.gradientColorA = Mgr.gradientGradient.Evaluate(0f);
                            Mgr.gradientColorB = Mgr.gradientGradient.Evaluate(1f);
                            EditorUtility.SetDirty(Mgr);
                            MarkPreviewDirty(def.id);
                        }
                        if (Mgr.gradientBakedTexture == null && Mgr.gradientGradient.colorKeys.Length > 0)
                            Mgr.gradientBakedTexture = AnimatedSpriteShader.BakeGradientToTexture(Mgr.gradientGradient, null);
                    }
                }
                // Blend mode dropdown
                EditorGUI.BeginChangeCheck();
                var blendProp = serializedObject.FindProperty("gradientBlendMode");
                if (blendProp != null)
                    blendProp.intValue = EditorGUILayout.Popup(new GUIContent("Blend Mode"), blendProp.intValue, new[] { "Additive", "Multiply" });
                if (EditorGUI.EndChangeCheck()) MarkPreviewDirty(def.id);
                EditorGUILayout.Space(2);
            }

            // Radial Gradient: same pattern
            if (def.id == EffectId.RadialGradient)
            {
                _skipFields.Add("radialGradUseGradientEditor");
                _skipFields.Add("radialGradBlendMode");
                _skipFields.Add("radialGradBakedTexture");
                var useGrad = serializedObject.FindProperty("radialGradUseGradientEditor");
                if (useGrad != null)
                {
                    EditorGUILayout.PropertyField(useGrad, new GUIContent("Use Gradient Editor"));
                    if (useGrad.boolValue)
                    {
                        _skipFields.Add("radialGradCenterColor");
                        _skipFields.Add("radialGradEdgeColor");
                        EditorGUI.BeginChangeCheck();
                        var gradProp = serializedObject.FindProperty("radialGradGradient");
                        if (gradProp != null)
                            EditorGUILayout.PropertyField(gradProp, new GUIContent("Gradient"));
                        if (EditorGUI.EndChangeCheck())
                        {
                            serializedObject.ApplyModifiedProperties();
                            Undo.RecordObject(Mgr, "Bake Radial Gradient Texture");
                            Mgr.radialGradBakedTexture = AnimatedSpriteShader.BakeGradientToTexture(
                                Mgr.radialGradGradient,
                                Mgr.radialGradBakedTexture);
                            Mgr.radialGradCenterColor = Mgr.radialGradGradient.Evaluate(0f);
                            Mgr.radialGradEdgeColor = Mgr.radialGradGradient.Evaluate(1f);
                            EditorUtility.SetDirty(Mgr);
                            MarkPreviewDirty(def.id);
                        }
                        if (Mgr.radialGradBakedTexture == null && Mgr.radialGradGradient.colorKeys.Length > 0)
                            Mgr.radialGradBakedTexture = AnimatedSpriteShader.BakeGradientToTexture(Mgr.radialGradGradient, null);
                    }
                }
                EditorGUI.BeginChangeCheck();
                var blendProp2 = serializedObject.FindProperty("radialGradBlendMode");
                if (blendProp2 != null)
                    blendProp2.intValue = EditorGUILayout.Popup(new GUIContent("Blend Mode"), blendProp2.intValue, new[] { "Additive", "Multiply" });
                if (EditorGUI.EndChangeCheck()) MarkPreviewDirty(def.id);
                EditorGUILayout.Space(2);
            }
        }

        private void DrawEffectParams(EffectDef def)
        {
            float prevLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 120;

            for (int i = 0; i < def.parms.Length; i++)
            {
                var p = def.parms[i];
                var prop = Prop(p.field);
                if (prop == null) continue;

                if (ShouldSkipBloomParam(p.field)) continue;
                if (_skipFields.Contains(p.field)) continue;

                switch (p.type)
                {
                    case ParamType.Float:
                        bool hasRange = p.min != 0 || p.max != 0;
                        EditorGUILayout.BeginHorizontal();
                        if (hasRange && prop.propertyType == SerializedPropertyType.Float)
                        {
                            EditorGUI.BeginChangeCheck();
                            EditorGUILayout.Slider(prop, p.min, p.max, Content(p.field));
                            if (EditorGUI.EndChangeCheck()) MarkPreviewDirty(def.id);
                        }
                        else
                        {
                            EditorGUI.BeginChangeCheck();
                            EditorGUILayout.PropertyField(prop, Content(p.field));
                            if (EditorGUI.EndChangeCheck()) MarkPreviewDirty(def.id);
                        }
                        if (GUILayout.Button(KeyButtonContent, EditorStyles.miniButton, GUILayout.Width(22), GUILayout.Height(18)))
                            AnimSpriteKeyframeEditor.OpenFloat(Mgr, p.field, p.display, prop.floatValue, p.min, p.max);
                        EditorGUILayout.EndHorizontal();
                        break;

                    case ParamType.Color:
                        EditorGUILayout.BeginHorizontal();
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(prop, Content(p.field));
                        if (EditorGUI.EndChangeCheck()) MarkPreviewDirty(def.id);
                        if (GUILayout.Button(KeyButtonContent, EditorStyles.miniButton, GUILayout.Width(22), GUILayout.Height(18)))
                            AnimSpriteKeyframeEditor.OpenColor(Mgr, p.field, p.display, prop.colorValue);
                        EditorGUILayout.EndHorizontal();
                        break;

                    case ParamType.HDRColor:
                        EditorGUILayout.BeginHorizontal();
                        EditorGUI.BeginChangeCheck();
                        var c = EditorGUILayout.ColorField(Content(p.field), prop.colorValue, true, true, true);
                        if (EditorGUI.EndChangeCheck()) { prop.colorValue = c; MarkPreviewDirty(def.id); }
                        if (GUILayout.Button(KeyButtonContent, EditorStyles.miniButton, GUILayout.Width(22), GUILayout.Height(18)))
                            AnimSpriteKeyframeEditor.OpenColor(Mgr, p.field, p.display, prop.colorValue);
                        EditorGUILayout.EndHorizontal();
                        break;

                    case ParamType.Vector2:
                        EditorGUILayout.BeginHorizontal();
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(prop, Content(p.field));
                        if (EditorGUI.EndChangeCheck()) MarkPreviewDirty(def.id);
                        // (No keyframe UI for Vector2 in existing keyframe editor)
                        EditorGUILayout.EndHorizontal();
                        break;

                    case ParamType.Vector3:
                        EditorGUILayout.BeginHorizontal();
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(prop, Content(p.field));
                        if (EditorGUI.EndChangeCheck()) MarkPreviewDirty(def.id);
                        // (No keyframe UI for Vector3 in existing keyframe editor)
                        EditorGUILayout.EndHorizontal();
                        break;

                    case ParamType.Vector4:
                        DrawVector4Expanded(prop, p, def);
                        break;

                    case ParamType.Texture:
                        DrawTextureWithPreview(prop, p);
                        break;

                    case ParamType.Int:
                        DrawIntParam(prop, p, def);
                        break;
                }
            }

            HideBloomModeParams(def);

            if (def.hasMask && Mgr.masksEnabled)
                DrawMaskChannel(def);

            EditorGUIUtility.labelWidth = prevLabelWidth;
        }

        private void DrawTextureWithPreview(SerializedProperty prop, EffectParam p)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(prop, Content(p.field), GUILayout.Height(18));
            // Tiling/Offset is handled via dedicated Vector4 params in registry (e.g. distortTiling, overlayTiling)

            var tex = prop.objectReferenceValue as Texture2D;
            var thumbRect = GUILayoutUtility.GetRect(48, 48, GUILayout.Width(48));
            EditorGUI.DrawRect(thumbRect, new Color(0.1f, 0.1f, 0.1f));

            if (tex != null)
            {
                GUI.DrawTexture(thumbRect, tex, ScaleMode.ScaleToFit);
            }
            else
            {
                DrawCheckerboard(thumbRect, 6);
                GUI.Label(thumbRect, "None", EditorStyles.centeredGreyMiniLabel);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawCheckerboard(Rect r, int cellSize)
        {
            Color dark = new Color(0.15f, 0.15f, 0.15f);
            Color light = new Color(0.25f, 0.25f, 0.25f);
            for (int y = 0; y < r.height; y += cellSize)
                for (int x = 0; x < r.width; x += cellSize)
                {
                    bool isDark = ((x / cellSize) + (y / cellSize)) % 2 == 0;
                    var cell = new Rect(r.x + x, r.y + y,
                        Mathf.Min(cellSize, r.width - x), Mathf.Min(cellSize, r.height - y));
                    EditorGUI.DrawRect(cell, isDark ? dark : light);
                }
        }

        private void DrawVector4Expanded(SerializedProperty prop, EffectParam p, EffectDef def)
        {
            // Special case: UV Scale Offset keeps a Vector4 internally (xy=scale, zw=offset)
            // but renders as two distinct Vector2 fields so the Offset isn't hidden.
            if (def.id == EffectId.UVScaleOffset)
            {
                Vector4 v = prop.vector4Value;
                EditorGUI.BeginChangeCheck();
                Vector2 scale = EditorGUILayout.Vector2Field(new GUIContent("Scale (XY)",
                    $"Scale applied to UVs before sampling.\nField: {p.field}.xy\nShader: {p.shader}.xy"),
                    new Vector2(v.x, v.y));
                Vector2 offset = EditorGUILayout.Vector2Field(new GUIContent("Offset (ZW)",
                    $"Offset added to UVs after scaling.\nField: {p.field}.zw\nShader: {p.shader}.zw"),
                    new Vector2(v.z, v.w));
                if (EditorGUI.EndChangeCheck())
                {
                    prop.vector4Value = new Vector4(scale.x, scale.y, offset.x, offset.y);
                    MarkPreviewDirty(def.id);
                }
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(KeyButtonContent, EditorStyles.miniButton, GUILayout.Width(22), GUILayout.Height(18)))
                    AnimSpriteKeyframeEditor.OpenVector(Mgr, p.field, p.display, prop.vector4Value);
                EditorGUILayout.EndHorizontal();
                return;
            }

            bool isXYOnly = p.display.Contains("Direction") || p.display.Contains("Center") ||
                            p.display.Contains("Offset") || p.display.Contains("Position") ||
                            p.display.Contains("Speed") ||
                            def.id == EffectId.Squish || def.id == EffectId.Squeeze;

            if (isXYOnly)
            {
                // Compact Vector2 display
                EditorGUILayout.BeginHorizontal();
                Vector4 v = prop.vector4Value;
                EditorGUI.BeginChangeCheck();
                Vector2 v2 = EditorGUILayout.Vector2Field(Content(p.field), new Vector2(v.x, v.y));
                if (EditorGUI.EndChangeCheck()) { prop.vector4Value = new Vector4(v2.x, v2.y, v.z, v.w); MarkPreviewDirty(def.id); }
                if (GUILayout.Button(KeyButtonContent, EditorStyles.miniButton, GUILayout.Width(22), GUILayout.Height(18)))
                    AnimSpriteKeyframeEditor.OpenVector(Mgr, p.field, p.display, prop.vector4Value);
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                float rangeMin = -2f, rangeMax = 2f;
                if (p.display.Contains("Tiling")) { rangeMin = -4f; rangeMax = 4f; }
                if (p.display.Contains("Amount")) { rangeMin = -1f; rangeMax = 1f; }

                EditorGUILayout.LabelField(Content(p.field), EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                Vector4 v = prop.vector4Value;
                EditorGUI.BeginChangeCheck();
                v.x = EditorGUILayout.Slider("X", v.x, rangeMin, rangeMax);
                v.y = EditorGUILayout.Slider("Y", v.y, rangeMin, rangeMax);
                v.z = EditorGUILayout.Slider("Z", v.z, rangeMin, rangeMax);
                v.w = EditorGUILayout.Slider("W", v.w, rangeMin, rangeMax);
                if (EditorGUI.EndChangeCheck()) { prop.vector4Value = v; MarkPreviewDirty(def.id); }
                EditorGUI.indentLevel--;

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(KeyButtonContent, EditorStyles.miniButton, GUILayout.Width(22), GUILayout.Height(18)))
                    AnimSpriteKeyframeEditor.OpenVector(Mgr, p.field, p.display, prop.vector4Value);
                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawIntParam(SerializedProperty prop, EffectParam p, EffectDef def)
        {
            string[] enumLabels = null;
            if (p.display == "Noise Type") enumLabels = NoiseTypeLabels;
            else if (p.display == "Mode" && def.id == EffectId.Bloom) enumLabels = BloomModeLabels;
            else if (p.display == "Mode" && def.id == EffectId.Outline) enumLabels = OutlineStyleLabels;
            else if (p.display == "Mode" && def.id == EffectId.Chromatic) enumLabels = new[] { "Directional", "Radial" };
            else if (p.display == "Space") enumLabels = CoordSpaceLabels;

            // Power-of-2 sample count for loop effects
            if (p.display == "Quality" || p.display == "Samples")
            {
                EditorGUI.BeginChangeCheck();
                int current = prop.propertyType == SerializedPropertyType.Integer ? prop.intValue : (int)prop.floatValue;
                int selectedIdx = 0;
                for (int si = 0; si < SampleCountValues.Length; si++)
                    if (SampleCountValues[si] <= current) selectedIdx = si;
                int newIdx = EditorGUILayout.Popup(Content(p.field), selectedIdx, SampleCountLabels);
                if (EditorGUI.EndChangeCheck())
                {
                    if (prop.propertyType == SerializedPropertyType.Integer) prop.intValue = SampleCountValues[newIdx];
                    else prop.floatValue = SampleCountValues[newIdx];
                    MarkPreviewDirty(def.id);
                }
                return;
            }

            if (enumLabels != null)
            {
                EditorGUI.BeginChangeCheck();
                int ev = EditorGUILayout.Popup(Content(p.field), prop.intValue, enumLabels);
                if (EditorGUI.EndChangeCheck()) { prop.intValue = ev; MarkPreviewDirty(def.id); }
            }
            else if (p.min != 0 || p.max != 0)
            {
                EditorGUI.BeginChangeCheck();
                int iv = EditorGUILayout.IntSlider(Content(p.field), prop.intValue, (int)p.min, (int)p.max);
                if (EditorGUI.EndChangeCheck()) { prop.intValue = iv; MarkPreviewDirty(def.id); }
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(prop, Content(p.field));
                if (EditorGUI.EndChangeCheck()) MarkPreviewDirty(def.id);
            }
        }

        private void HideBloomModeParams(EffectDef def)
        {
            if (def.id != EffectId.Bloom) return;
        }

        private bool ShouldSkipBloomParam(string fieldName)
        {
            if (!Mgr.bloomEnabled) return false;
            int mode = Mgr.bloomMode;
            bool showThreshold = mode == 0 || mode == 2;
            bool showMaskColor = mode == 1 || mode == 2;

            if (!showThreshold && (fieldName == "bloomThreshold" || fieldName == "bloomSoftKnee"))
                return true;
            if (!showMaskColor && (fieldName == "bloomMaskColor" || fieldName == "bloomMaskRange" || fieldName == "bloomMaskSoft"))
                return true;
            return false;
        }

        private void DrawMaskChannel(EffectDef def)
        {
            var maskProp = Prop(def.maskField);
            if (maskProp == null) return;

            EditorGUILayout.Space(2);
            EditorGUILayout.LabelField("Mask Channel", EditorStyles.miniLabel);
            EditorGUILayout.BeginHorizontal();

            int current = maskProp.intValue;
            string[] labels = { "None", "R", "G", "B", "A" };
            for (int ch = 0; ch < 5; ch++)
            {
                var oldBg = GUI.backgroundColor;
                bool selected = ch == current;
                GUI.backgroundColor = selected ? MaskDotColors[ch] : new Color(0.25f, 0.25f, 0.25f);
                var style = selected ? EditorStyles.toolbarButton : EditorStyles.miniButton;
                if (GUILayout.Button(labels[ch], style, GUILayout.Height(22), GUILayout.MinWidth(36)))
                    maskProp.intValue = ch;
                GUI.backgroundColor = oldBg;
            }
            EditorGUILayout.EndHorizontal();
        }

        // Perf Dot

        // Effect performance categorization moved to EffectCategorization class in EffectDefinition.cs

        // Effect Preview

        private void DrawEffectPreview(EffectDef def)
        {
            bool dirty = !_previewCache.ContainsKey(def.id) ||
                         (_previewDirty.ContainsKey(def.id) && _previewDirty[def.id]);

            if (dirty)
            {
                // Debounce: only re-render after 200ms of no changes
                double now = EditorApplication.timeSinceStartup;
                if (_previewDebounceTime == 0 || now - _previewDebounceTime > PREVIEW_DEBOUNCE_DELAY)
                {
                    RenderEffectPreview(def);
                    _previewDebounceTime = 0;
                }
                else
                {
                    // Schedule a delayed repaint (don't use SetDirty — causes repaint storm)
                    EditorApplication.delayCall -= Repaint;
                    EditorApplication.delayCall += Repaint;
                }
            }

            if (_previewCache.TryGetValue(def.id, out var tex) && tex != null)
            {
                EditorGUILayout.LabelField("Preview", EditorStyles.centeredGreyMiniLabel, GUILayout.Width(PREVIEW_SIZE));
                var r = GUILayoutUtility.GetRect(PREVIEW_SIZE, PREVIEW_SIZE, GUILayout.Width(PREVIEW_SIZE));
                DrawCheckerboard(r);
                GUI.DrawTexture(r, tex, ScaleMode.ScaleToFit);
            }
        }

        private static Texture2D _checkerTex;

        private Shader ResolvePreviewShader()
        {
            // Inspector previews are rendered with Graphics.Blit, outside the normal SpriteRenderer,
            // Canvas, and URP 2D lighting paths. Lit/GUI variants can therefore render black or skip
            // the effect pass in the preview even though the scene material is correct. Use the
            // equivalent unlit preview shader for those cases, but keep the same pipeline family.
            string preferred = Mgr.CurrentVariant switch
            {
                AnimatedSpriteShader.ShaderVariant.Standard_URP or
                AnimatedSpriteShader.ShaderVariant.Additive_URP or
                AnimatedSpriteShader.ShaderVariant.Premultiply_URP or
                AnimatedSpriteShader.ShaderVariant.GUI_URP or
                AnimatedSpriteShader.ShaderVariant.GUIAdditive_URP or
                AnimatedSpriteShader.ShaderVariant.Lit2D_URP or
                AnimatedSpriteShader.ShaderVariant.Lit2DAdditive_URP or
                AnimatedSpriteShader.ShaderVariant.Lit3D_URP or
                AnimatedSpriteShader.ShaderVariant.Lit3DCutout_URP or
                AnimatedSpriteShader.ShaderVariant.Lit3DAdditive_URP => "AnimatedSprite/Standard URP",

                AnimatedSpriteShader.ShaderVariant.HDRP_Unlit or
                AnimatedSpriteShader.ShaderVariant.HDRP_Additive or
                AnimatedSpriteShader.ShaderVariant.HDRP_Lit => "AnimatedSprite/HDRP Unlit",

                AnimatedSpriteShader.ShaderVariant.GUI or
                AnimatedSpriteShader.ShaderVariant.GUIAdditive or
                AnimatedSpriteShader.ShaderVariant.GUIPremultiply or
                AnimatedSpriteShader.ShaderVariant.ScreenSpaceOverlay => "AnimatedSprite/Standard",

                _ => null
            };

            int currentVariant = (int)Mgr.CurrentVariant;
            if (string.IsNullOrEmpty(preferred) && currentVariant >= 0 && currentVariant < AnimatedSpriteShader.ShaderPaths.Length)
                preferred = AnimatedSpriteShader.ShaderPaths[currentVariant];

            var preferredShader = !string.IsNullOrEmpty(preferred) ? Shader.Find(preferred) : null;
            if (preferredShader) return preferredShader;

            // Fallbacks keep the preview usable if the selected generated shader has not been
            // imported yet. URP is preferred before Built-In because most users adding the
            // component in URP expect URP preview keywords and passes.
            string[] fallbacks =
            {
                "AnimatedSprite/Standard URP",
                "AnimatedSprite/Standard",
                "AnimatedSprite/HDRP Unlit",
                "Sprites/Default"
            };

            foreach (var name in fallbacks)
            {
                var shader = Shader.Find(name);
                if (shader) return shader;
            }

            return null;
        }

        private static void DrawCheckerboard(Rect r)
        {
            if (!_checkerTex)
            {
                int sz = 16;
                int cell = 4;
                _checkerTex = new Texture2D(sz, sz, TextureFormat.RGBA32, false);
                _checkerTex.filterMode = FilterMode.Point;
                _checkerTex.hideFlags = HideFlags.HideAndDontSave;
                var light = new Color(0.35f, 0.35f, 0.35f, 1f);
                var dark = new Color(0.22f, 0.22f, 0.22f, 1f);
                for (int y = 0; y < sz; y++)
                    for (int x = 0; x < sz; x++)
                        _checkerTex.SetPixel(x, y, ((x / cell + y / cell) % 2 == 0) ? light : dark);
                _checkerTex.Apply();
            }
            GUI.DrawTextureWithTexCoords(r, _checkerTex,
                new Rect(0, 0, r.width / _checkerTex.width, r.height / _checkerTex.height));
        }

        private void RenderEffectPreview(EffectDef def)
        {
            Sprite sourceSprite = Mgr.GetActiveSprite();
            if (!sourceSprite) return;

            var spriteTex = sourceSprite.texture;
            if (!spriteTex) return;

            var previewShader = ResolvePreviewShader();
            if (!previewShader) return;

            if (!_previewMaterial || _previewMaterial.shader != previewShader)
            {
                if (_previewMaterial) DestroyImmediate(_previewMaterial);
                _previewMaterial = new Material(previewShader)
                {
                    name = "AnimatedSprite Effect Preview",
                    hideFlags = HideFlags.HideAndDontSave
                };
            }

            // Disable all per-effect keywords and enable only the target effect's group + enable flag
            foreach (var otherDef in EffectRegistry.All)
            {
                if (otherDef.keyword != null) _previewMaterial.DisableKeyword(otherDef.keyword);
                if (otherDef.enableShader != null) _previewMaterial.SetFloat(otherDef.enableShader, 0f);
            }

            // Enable the group keyword for this effect (e.g., _FX_COLOR)
            string groupKw = EffectGroupInfo.GetKeyword(def.group);
            _previewMaterial.EnableKeyword(groupKw);
            // Also enable the per-effect keyword if it exists
            if (def.keyword != null) _previewMaterial.EnableKeyword(def.keyword);

            _previewMaterial.SetFloat("_MinXUV", 0);
            _previewMaterial.SetFloat("_MaxXUV", 1);
            _previewMaterial.SetFloat("_MinYUV", 0);
            _previewMaterial.SetFloat("_MaxYUV", 1);
            _previewMaterial.SetTexture("_MainTex", spriteTex);
            float previewAspect = sourceSprite.rect.height > 1e-5f
                ? sourceSprite.rect.width / sourceSprite.rect.height
                : 1f;
            _previewMaterial.SetFloat("_SpriteAspect", previewAspect);
            _previewMaterial.SetVector("_MainTex_TexelSize", new Vector4(1f / PREVIEW_SIZE, 1f / PREVIEW_SIZE, PREVIEW_SIZE, PREVIEW_SIZE));
            _previewMaterial.SetFloat("_GlobalEffectFade", 1);
            _previewMaterial.SetFloat("_TimeScale", 1);
            _previewMaterial.SetFloat("_TimeFPS", 0);
            _previewMaterial.SetFloat("_UseCustomTime", 1);
            _previewMaterial.SetFloat("_CustomTime", (float)EditorApplication.timeSinceStartup);
            // _EdgeCleanup removed — Alpha Cutoff is the single alpha-threshold control now.
            _previewMaterial.SetFloat("_SpriteFlipX", 0);
            _previewMaterial.SetFloat("_SpriteFlipY", 0);
            _previewMaterial.SetFloat("_EffectMask_En", 0);
            _previewMaterial.SetFloat("_MaskTex_En", 0);

            // Use the actual enableShader property name
            if (def.enableShader != null)
                _previewMaterial.SetFloat(def.enableShader, 1f);

            foreach (var p in def.parms)
            {
                var sp = Prop(p.field);
                if (sp == null) continue;
                switch (p.type)
                {
                    case ParamType.Float:
                    case ParamType.Int:
                        if (sp.propertyType == SerializedPropertyType.Boolean)
                            _previewMaterial.SetFloat(p.shader, sp.boolValue ? 1f : 0f);
                        else if (sp.propertyType == SerializedPropertyType.Integer)
                            _previewMaterial.SetFloat(p.shader, sp.intValue);
                        else
                            _previewMaterial.SetFloat(p.shader, sp.floatValue);
                        break;

                    case ParamType.Color:
                    case ParamType.HDRColor:
                        _previewMaterial.SetColor(p.shader, sp.colorValue);
                        break;

                    case ParamType.Vector4:
                        _previewMaterial.SetVector(p.shader, sp.vector4Value);
                        break;

                    case ParamType.Vector2:
                        if (sp.propertyType == SerializedPropertyType.Vector2)
                        { var v2p = sp.vector2Value; _previewMaterial.SetVector(p.shader, new Vector4(v2p.x, v2p.y, 0, 0)); }
                        else if (sp.propertyType == SerializedPropertyType.Vector3)
                        { var v3p = sp.vector3Value; _previewMaterial.SetVector(p.shader, new Vector4(v3p.x, v3p.y, 0, 0)); }
                        else if (sp.propertyType == SerializedPropertyType.Vector4)
                        { _previewMaterial.SetVector(p.shader, sp.vector4Value); }
                        break;

                    case ParamType.Texture:
                        if (sp.objectReferenceValue is Texture t) _previewMaterial.SetTexture(p.shader, t);
                        break;
                }
            }

            var sourceRt = RenderTexture.GetTemporary(PREVIEW_SIZE, PREVIEW_SIZE, 0, RenderTextureFormat.ARGB32);
            var rt = RenderTexture.GetTemporary(PREVIEW_SIZE, PREVIEW_SIZE, 0, RenderTextureFormat.ARGB32);
            var prev = RenderTexture.active;

            try
            {
                RenderTexture.active = sourceRt;
                GL.Clear(true, true, Color.clear);

                Rect texRect = sourceSprite.textureRect;
                Vector2 scale = new Vector2(texRect.width / spriteTex.width, texRect.height / spriteTex.height);
                Vector2 offset = new Vector2(texRect.x / spriteTex.width, texRect.y / spriteTex.height);

                // Crop to the actual sprite rect first. This makes previews correct for both
                // SpriteRenderer sprites and UGUI Image sprites, including sprites packed in atlases.
                Graphics.Blit(spriteTex, sourceRt, scale, offset);

                RenderTexture.active = rt;
                GL.Clear(true, true, Color.clear);
                Graphics.Blit(sourceRt, rt, _previewMaterial);

                if (!_previewCache.ContainsKey(def.id))
                    _previewCache[def.id] = new Texture2D(PREVIEW_SIZE, PREVIEW_SIZE, TextureFormat.RGBA32, false);

                _previewCache[def.id].ReadPixels(new Rect(0, 0, PREVIEW_SIZE, PREVIEW_SIZE), 0, 0);
                _previewCache[def.id].Apply();
            }
            finally
            {
                RenderTexture.active = prev;
                RenderTexture.ReleaseTemporary(sourceRt);
                RenderTexture.ReleaseTemporary(rt);
            }

            _previewDirty[def.id] = false;
        }

        private void MarkPreviewDirty(EffectId id)
        {
            _previewDirty[id] = true;
            _previewDebounceTime = EditorApplication.timeSinceStartup;
        }

        private void CleanupPreviews()
        {
            foreach (var kvp in _previewCache)
                if (kvp.Value) DestroyImmediate(kvp.Value);
            _previewCache.Clear();
            if (_previewMaterial) DestroyImmediate(_previewMaterial);
        }

        // Copy / Paste / Reset

        private void CopyEffectCode(EffectDef def)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"mgr.{def.enableField} = true;");
            foreach (var p in def.parms)
            {
                var sp = Prop(p.field);
                if (sp != null)
                    sb.AppendLine($"mgr.{p.field} = {FormatPropertyForCode(sp, p.type)};");
            }
            GUIUtility.systemCopyBuffer = sb.ToString();
            Debug.Log($"[AnimSprite] {def.name} code copied.");
        }

        private void CopyEffectSettings(EffectDef def)
        {
            _clipboard = new Dictionary<string, object>();
            _clipboardEffectId = def.id;
            foreach (var p in def.parms)
            {
                var sp = Prop(p.field);
                if (sp == null) continue;
                switch (p.type)
                {
                    case ParamType.Float:
                        if (sp.propertyType == SerializedPropertyType.Boolean)
                            _clipboard[p.field] = sp.boolValue ? 1f : 0f;
                        else
                            _clipboard[p.field] = sp.floatValue;
                        break;
                    case ParamType.Color:
                    case ParamType.HDRColor: _clipboard[p.field] = sp.colorValue;
                    break;
                    case ParamType.Vector2: _clipboard[p.field] = sp.vector2Value;
                    break;
                    case ParamType.Vector3: _clipboard[p.field] = sp.vector3Value;
                    break;
                    case ParamType.Vector4: _clipboard[p.field] = sp.vector4Value;
                    break;
                    case ParamType.Int: _clipboard[p.field] = sp.intValue;
                    break;
                }
            }
            Debug.Log($"[AnimSprite] {def.name} settings copied.");
        }

        private void PasteEffectSettings(EffectDef def)
        {
            if (_clipboard == null) return;
            Undo.RecordObject(Mgr, "Paste Effect");
            foreach (var p in def.parms)
            {
                if (!_clipboard.TryGetValue(p.field, out var val)) continue;
                var sp = Prop(p.field);
                if (sp == null) continue;
                switch (p.type)
                {
                    case ParamType.Float: sp.floatValue = (float)val;
                    break;
                    case ParamType.Color:
                    case ParamType.HDRColor: sp.colorValue = (Color)val;
                    break;
                    case ParamType.Vector2: sp.vector2Value = (Vector2)val;
                    break;
                    case ParamType.Vector3: sp.vector3Value = (Vector3)val;
                    break;
                    case ParamType.Vector4: sp.vector4Value = (Vector4)val;
                    break;
                    case ParamType.Int: sp.intValue = (int)val;
                    break;
                }
            }
            MarkPreviewDirty(def.id);
            Debug.Log($"[AnimSprite] Settings pasted to {def.name}.");
        }

        private void ResetEffect(EffectDef def)
        {
            Undo.RecordObject(Mgr, "Reset Effect");
            foreach (var p in def.parms)
            {
                var sp = Prop(p.field);
                if (sp == null) continue;
                switch (p.type)
                {
                    case ParamType.Float:
                        float defF;
                        if (float.TryParse(p.defaultValue, System.Globalization.NumberStyles.Float,
                            System.Globalization.CultureInfo.InvariantCulture, out defF))
                            sp.floatValue = defF;
                        else sp.floatValue = 0;
                        break;

                    case ParamType.Int:
                        int defI;
                        if (int.TryParse(p.defaultValue, out defI)) sp.intValue = defI;
                        else sp.intValue = 0;
                        break;

                    case ParamType.Color:
                    case ParamType.HDRColor:
                        // Keep this simple & safe: default to white unless explicitly clear/black is requested.
                        if (!string.IsNullOrEmpty(p.defaultValue) && p.defaultValue.Contains("Color.clear")) sp.colorValue = Color.clear;
                        else if (!string.IsNullOrEmpty(p.defaultValue) && p.defaultValue.Contains("Color.black")) sp.colorValue = Color.black;
                        else sp.colorValue = Color.white;
                        break;

                    case ParamType.Vector2:
                        sp.vector2Value = Vector2.zero;
                        break;

                    case ParamType.Vector3:
                        sp.vector3Value = Vector3.zero;
                        break;

                    case ParamType.Vector4:
                        sp.vector4Value = Vector4.zero;
                        break;

                    case ParamType.Texture:
                        sp.objectReferenceValue = null;
                        break;
                }
            }
            MarkPreviewDirty(def.id);
        }

        // EXISTING SECTIONS (preserved)

        private void DrawTools()
        {
            _toolsOpen = EditorGUILayout.Foldout(_toolsOpen, "Tools", true, EditorStyles.foldoutHeader);
            if (!_toolsOpen) return;
            EditorGUIUtility.labelWidth = 160;

            var bgRect = EditorGUILayout.BeginVertical();
            EditorGUI.DrawRect(bgRect, ToolsBg);
            GUILayout.Space(4);

            ButtonRow(
                ("Deactivate All", ToolRed, (Action)(() => { Undo.RecordObject(Mgr, "Deactivate"); Mgr.DeactivateAllEffects(); })),
                ("Apply Children", ToolDefault, (Action)(() => Mgr.ApplyToChildren()))
            );
            ButtonRow(
                ("Atlas Setup", ToolDefault, (Action)(() => Mgr.SetupSpriteAtlas())),
                ("Bake PNG", ToolDefault, (Action)(() =>
                {
                    int[] scales = { 1, 2, 4 };
                    int choice = EditorUtility.DisplayDialogComplex("Bake Resolution", "Choose bake resolution multiplier:", "1x", "2x", "4x");
                    int scale = scales[choice];
                    Sprite sourceSprite = Mgr.GetActiveSprite();
                    int w = 0, h = 0;
                    if (sourceSprite && sourceSprite.texture) { w = Mathf.RoundToInt(sourceSprite.textureRect.width) * scale; h = Mathf.RoundToInt(sourceSprite.textureRect.height) * scale; }
                    var tex = Mgr.BakeMaterialToTexture(w, h);
                    if (tex == null) return;
                    var path = EditorUtility.SaveFilePanelInProject("Save Baked", "BakedSprite", "png", "Save");
                    if (!string.IsNullOrEmpty(path)) { System.IO.File.WriteAllBytes(path, tex.EncodeToPNG()); AssetDatabase.ImportAsset(path); }
                }))
            );

            if (Mgr.masksEnabled)
            {
                ButtonRow(("Pack Masks to RGBA", ToolDefault, (Action)(() =>
                {
                    var tex = Mgr.BakePackedMasks();
                    var path = EditorUtility.SaveFilePanelInProject("Save Packed", "PackedMasks", "png", "Save");
                    if (!string.IsNullOrEmpty(path))
                    {
                        System.IO.File.WriteAllBytes(path, tex.EncodeToPNG());
                        AssetDatabase.ImportAsset(path);
                        var imported = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                        if (imported) { Undo.RecordObject(Mgr, "Assign Packed"); Mgr.packedMasks = imported; }
                    }
                })));
            }

            ButtonRow(("Setup Animator", ToolDefault, (Action)(() =>
            {
                if (!Mgr.GetComponent<Animator>()) Mgr.gameObject.AddComponent<Animator>();
                EditorApplication.ExecuteMenuItem("Window/Animation/Animation");
            })));

            DrawAnimationBrowser();

            EditorGUILayout.Space(2);
            EditorGUILayout.LabelField("Effect Presets", EditorStyles.boldLabel);
            ButtonRow(
                ("Save Preset", ToolDefault, (Action)(() =>
                {
                    var path = EditorUtility.SaveFilePanelInProject("Save", "EffectPreset", "asset", "Save");
                    if (string.IsNullOrEmpty(path)) return;
                    var p = ScriptableObject.CreateInstance<AnimSpriteEffectPreset>();
                    p.SaveFrom(Mgr);
                    AssetDatabase.CreateAsset(p, path);
                    AssetDatabase.SaveAssets();
                })),
                ("Load Preset", ToolDefault, (Action)(() =>
                {
                    var path = EditorUtility.OpenFilePanel("Load", "Assets", "asset");
                    if (string.IsNullOrEmpty(path)) return;
                    path = "Assets" + path.Substring(Application.dataPath.Length);
                    var p = AssetDatabase.LoadAssetAtPath<AnimSpriteEffectPreset>(path);
                    if (p) { Undo.RecordObject(Mgr, "Load Preset"); p.ApplyTo(Mgr); EditorUtility.SetDirty(Mgr); }
                }))
            );

            GUILayout.Space(4);
            EditorGUILayout.EndVertical();
        }


        // Pipeline detection by variant name (no hardcoded indices)
        private static string GetVariantPipeline(string variantName)
        {
            if (variantName.Contains("URP")) return "AIO_URP";
            if (variantName.Contains("HDRP")) return "AIO_HDRP";
            return "AIO_BUILTIN";
        }

        private static string GetVariantPipelineDisplay(string variantName)
        {
            string p = GetVariantPipeline(variantName);
            return p == "AIO_URP" ? "URP" : p == "AIO_HDRP" ? "HDRP" : "Built-In";
        }

        private bool IsVariantAvailable(string variantName)
        {
            var settings = AnimSpriteProjectSettings.Instance;
            if (!settings) return true;
            var pipeline = settings.Pipeline;
            string vp = GetVariantPipeline(variantName);
            if (vp == "AIO_URP") return pipeline == RenderPipelineTarget.URP;
            if (vp == "AIO_HDRP") return pipeline == RenderPipelineTarget.HDRP;
            return pipeline == RenderPipelineTarget.BuiltIn;
        }

        private static GUIStyle _pipLabelStyle;

        private void DrawSystem()
        {
            BeginDarkBox();

            // Material mode + Pipeline label on one line
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(Prop("_materialMode"), new GUIContent("Material Mode"), GUILayout.MinWidth(140));
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                Mgr.CurrentMaterialMode = Mgr.CurrentMaterialMode;
            }

            var settings = AnimSpriteProjectSettings.Instance;
            string activePipeline = settings != null ? settings.PipelineDefine : "AIO_BUILTIN";
            string activePipelineLabel = activePipeline == "AIO_URP" ? "URP" : activePipeline == "AIO_HDRP" ? "HDRP" : "Built-In";
            if (_pipLabelStyle == null)
                _pipLabelStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    alignment = TextAnchor.MiddleRight,
                    normal = { textColor = new Color(0.5f, 0.8f, 0.5f) }
                };
            GUILayout.Label(activePipelineLabel, _pipLabelStyle, GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();

            // Shader variant dropdown
            var varProp = Prop("_shaderVariant");
            var allNames = System.Enum.GetNames(typeof(AnimatedSpriteShader.ShaderVariant));
            int currentIdx = varProp.enumValueIndex;

            // Build sorted display: active pipeline first, then others dimmed
            var sortedIndices = new System.Collections.Generic.List<int>();
            // Group: active pipeline variants first
            string[] pipelineOrder = { activePipeline,
                activePipeline == "AIO_BUILTIN" ? "AIO_URP" : "AIO_BUILTIN",
                activePipeline == "AIO_HDRP" ? "AIO_URP" : "AIO_HDRP" };
            // Remove duplicates
            var orderedPipelines = new System.Collections.Generic.List<string>();
            foreach (var p in pipelineOrder) if (!orderedPipelines.Contains(p)) orderedPipelines.Add(p);

            foreach (var pip in orderedPipelines)
            {
                for (int i = 0; i < allNames.Length; i++)
                {
                    if (GetVariantPipeline(allNames[i]) == pip)
                        sortedIndices.Add(i);
                }
            }

            // Build display names with pipeline headers and dimming
            string lastPip = "";
            var popupLabels = new System.Collections.Generic.List<string>();
            var popupToEnum = new System.Collections.Generic.List<int>(); // maps popup index → enum index
            int popupCurrent = 0;

            foreach (int enumIdx in sortedIndices)
            {
                string name = allNames[enumIdx];
                string pip = GetVariantPipelineDisplay(name);
                bool available = IsVariantAvailable(name);

                // Add pipeline separator
                if (pip != lastPip)
                {
                    lastPip = pip;
                    if (popupLabels.Count > 0)
                    {
                        popupLabels.Add("");
                        popupToEnum.Add(-1);
                    }
                }

                string label = name.Replace("_", " ");
                if (!available)
                    label = "\u2003" + label + "  \u2014 requires " + pip; // indented + dimmed text

                if (enumIdx == currentIdx)
                    popupCurrent = popupLabels.Count;

                popupLabels.Add(label);
                popupToEnum.Add(enumIdx);
            }

            EditorGUI.BeginChangeCheck();
            int selected = EditorGUILayout.Popup("Shader Variant", popupCurrent, popupLabels.ToArray());
            if (EditorGUI.EndChangeCheck() && selected >= 0 && selected < popupToEnum.Count)
            {
                int enumIdx = popupToEnum[selected];
                if (enumIdx < 0) { /* separator, ignore */ }
                else if (!IsVariantAvailable(allNames[enumIdx]))
                {
                    string needed = GetVariantPipelineDisplay(allNames[enumIdx]);
                    EditorUtility.DisplayDialog("Pipeline Mismatch",
                        $"This variant requires {needed}.\nCurrent pipeline: {activePipelineLabel}\n\nSwitch pipeline in AnimatedSprite > Master Setup.", "OK");
                }
                else
                {
                    Undo.RecordObject(Mgr, "Change Shader Variant");
                    Mgr.ChangeShaderVariant((AnimatedSpriteShader.ShaderVariant)enumIdx);
                    serializedObject.Update();
                }
            }

            // GUI + HDRP incompatibility warning — HDRP has no Canvas shader, so picking a GUI
            // variant while the project is on HDRP is functionally broken. Tell the user now
            // rather than letting them discover the Image renders as a black quad at runtime.
            if (activePipeline == "AIO_HDRP" && PipelineRouting.IsVariantGUI(Mgr.CurrentVariant))
            {
                EditorGUILayout.HelpBox(
                    "This component is on a GUI-type shader variant, but the project is on HDRP — which " +
                    "has no Canvas/UGUI shader. Canvas UI will not render correctly. " +
                    "Switch this variant to HDRP Unlit, or keep UI sprites on a separate Built-In / URP scene.",
                    MessageType.Warning);
            }

            EndDarkBox();
        }

        static bool _globalAdvancedOpen;
        static bool _stencilSubOpen = true;

        private void DrawGlobal()
        {
            _globalOpen = EditorGUILayout.Foldout(_globalOpen, "Global Settings", true, EditorStyles.foldoutHeader);
            if (!_globalOpen) return;
            EditorGUIUtility.labelWidth = 120;

            BeginDarkBox();

            // Core settings (always visible, compact)
            EditorGUILayout.PropertyField(Prop("globalEffectFade"), new GUIContent("Effect Fade"));
            // NOTE: legacy "Edge Cleanup" field removed from inspector — its behaviour is now covered
            // by Alpha Cutoff below. The serialized `edgeCleanup` field remains on the component
            // purely for backward compatibility with existing scene data; it no longer affects rendering.

            ToggleSection("alphaCutoffEnabled", "Alpha Cutoff", () =>
                EditorGUILayout.PropertyField(Prop("alphaCutoff"), new GUIContent("Threshold")));
            ToggleSection("effectMaskEnabled", "Effect Mask", () =>
                EditorGUILayout.PropertyField(Prop("effectMaskTexture"), new GUIContent("Mask Texture")));
            ToggleSection("pixelArtSnapEnabled", "Pixel Art Snap", () =>
                EditorGUILayout.PropertyField(Prop("pixelSnapResolution"), new GUIContent("Resolution")));

            // Sprite: two-line compact layout
            EditorGUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 120;
            EditorGUILayout.PropertyField(Prop("premulAlpha"), new GUIContent("Premul Alpha"));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 80;
            EditorGUILayout.PropertyField(Prop("spriteFlipX"), new GUIContent("Flip X"));
            EditorGUILayout.PropertyField(Prop("spriteFlipY"), new GUIContent("Flip Y"));
            EditorGUIUtility.labelWidth = 120;
            if (GUILayout.Button("Auto-Detect", GUILayout.Width(100), GUILayout.Height(18)))
            { Undo.RecordObject(Mgr, "Auto Flip"); Mgr.AutoDetectSpriteFlip(); serializedObject.Update(); }

            EditorGUILayout.EndHorizontal();

            // Time
            var timeModeProp = Prop("timeMode");
            var legacyUseCustomTimeProp = Prop("useCustomTime");
            if (timeModeProp != null)
            {
                if (legacyUseCustomTimeProp != null && legacyUseCustomTimeProp.boolValue)
                    timeModeProp.enumValueIndex = (int)AnimatedSpriteShader.ShaderTimeMode.Custom;

                EditorGUILayout.IntPopup(
                    timeModeProp,
                    new[]
                    {
                        new GUIContent("Real Time * Time Scale", "Uses Unity scaled shader time. This pauses/slows when Time.timeScale changes."),
                        new GUIContent("Unscaled Time", "Uses Time.unscaledTime. GUI effects default to this so they keep animating during game pause."),
                        new GUIContent("Custom Time", "Uses the Custom Time value below.")
                    },
                    new[]
                    {
                        (int)AnimatedSpriteShader.ShaderTimeMode.ScaledTime,
                        (int)AnimatedSpriteShader.ShaderTimeMode.UnscaledTime,
                        (int)AnimatedSpriteShader.ShaderTimeMode.Custom
                    },
                    new GUIContent("Time Source"));

                if (legacyUseCustomTimeProp != null)
                    legacyUseCustomTimeProp.boolValue = timeModeProp.enumValueIndex == (int)AnimatedSpriteShader.ShaderTimeMode.Custom;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 70;
            EditorGUILayout.PropertyField(Prop("timeScale"), new GUIContent("Time Scale"));
            EditorGUIUtility.labelWidth = 30;
            EditorGUILayout.PropertyField(Prop("timeFPS"), new GUIContent("FPS"), GUILayout.Width(80));
            EditorGUIUtility.labelWidth = 120;
            EditorGUILayout.EndHorizontal();

            if (timeModeProp != null && timeModeProp.enumValueIndex == (int)AnimatedSpriteShader.ShaderTimeMode.Custom)
                EditorGUILayout.PropertyField(Prop("customTime"), new GUIContent("Custom Time"));

            EditorGUILayout.PropertyField(Prop("vertexUVExpand"), new GUIContent("UV Expand"));

            // Advanced (collapsed by default)
            EditorGUI.indentLevel++;
            _globalAdvancedOpen = EditorGUILayout.Foldout(_globalAdvancedOpen, "Advanced", true);
            EditorGUI.indentLevel--;
            if (_globalAdvancedOpen)
            {
                EditorGUI.indentLevel++;
                EditorGUIUtility.labelWidth = 150;
                ToggleSection("outlineEffectsOnly", "Outline Effects Only", () =>
                    EditorGUILayout.PropertyField(Prop("outlineEffectWidth"), new GUIContent("Width")));

                // Shadows compact
                EditorGUIUtility.labelWidth = 150;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(Prop("castShadows"), new GUIContent("Cast shadows"));
                EditorGUILayout.PropertyField(Prop("receiveShadows"), new GUIContent("Recieve shadows"));
                EditorGUILayout.EndHorizontal();
                EditorGUIUtility.labelWidth = 150;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(Prop("shadowAlphaClip"), new GUIContent("Shadow A-Clip"));
                EditorGUILayout.EndHorizontal();

                // Two-sided lighting toggle — flips the normal on back faces so the reverse of the
                // sprite is lit instead of going black. Only meaningful on lit variants.
                EditorGUILayout.PropertyField(Prop("twoSidedLighting"),
                    new GUIContent("Two-Sided Lighting",
                    "When enabled, back faces use a flipped normal so they receive light as if facing the camera. " +
                    "Only applies to lit shader variants. Turn off if you want the back of the sprite to be dark."));

                // Project-wide lighting multipliers live in AnimSpriteProjectSettings — surface them as
                // a shortcut so users don't have to hunt through the Master Setup window for them.
                EditorGUILayout.Space(2);
                EditorGUILayout.LabelField("Project-wide lighting (Master Setup)", EditorStyles.miniBoldLabel);
                var settings = AnimSpriteProjectSettings.Instance;
                if (settings != null)
                {
                    var so = new SerializedObject(settings);
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(so.FindProperty("globalLightIntensity"),
                        new GUIContent("Direct Light", "Multiplier on direct light contribution for all sprites in the project."));
                    EditorGUILayout.PropertyField(so.FindProperty("ambientLightIntensity"),
                        new GUIContent("Ambient", "Multiplier on ambient/SH contribution for all sprites in the project."));
                    if (EditorGUI.EndChangeCheck())
                    {
                        so.ApplyModifiedProperties();
                        AnimatedSpriteShader.BroadcastGlobalsDirty(true);
                        UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
                    }
                }
                EditorGUIUtility.labelWidth = 150;

                ToggleSection("stencilEnabled", "Stencil", () =>
                {
                    _stencilSubOpen = EditorGUILayout.Foldout(_stencilSubOpen, "Stencil Settings", true);
                    if (_stencilSubOpen)
                    {
                        EditorGUI.indentLevel++;
                        float prevLabelW = EditorGUIUtility.labelWidth;
                        EditorGUIUtility.labelWidth = 110;
                        EditorGUILayout.PropertyField(
                            Prop("stencilRef"),
                            new GUIContent(
                                "Ref",
                                "Reference value used when comparing against the stencil buffer."));
                        EditorGUILayout.PropertyField(
                            Prop("stencilComp"),
                            new GUIContent(
                                "Compare",
                                "Comparison function applied between Ref and the current stencil value."));
                        EditorGUILayout.PropertyField(
                            Prop("stencilPass"),
                            new GUIContent(
                                "Pass Op",
                                "Operation performed on the stencil buffer when the stencil test passes."));
                        EditorGUILayout.PropertyField(
                            Prop("stencilReadMask"),
                            new GUIContent(
                                "Read Mask",
                                "Bitmask AND-ed with both Ref and the current stencil value before comparing."));
                        EditorGUILayout.PropertyField(
                            Prop("stencilWriteMask"),
                            new GUIContent(
                                "Write Mask",
                                "Bitmask controlling which bits of the stencil buffer are written."));
                        EditorGUIUtility.labelWidth = prevLabelW;
                        EditorGUI.indentLevel--;
                    }
                });
                EditorGUIUtility.labelWidth = 120;
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(2);
            EditorGUILayout.PropertyField(
                Prop("editorVisualUpdatesInEditMode"),
                new GUIContent(
                    "Edit Mode Visual Update",
                    "Animates time-based shader effects in edit mode without pushing effect values or dirtying the scene. Disabled by default."));

            // Effect Masks
            EditorGUILayout.Space(2);
            EditorGUILayout.LabelField("Effect Masks", _subSectionStyle);
            ToggleSection("masksEnabled", "Enable Masks", () =>
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(Prop("maskR"), new GUIContent("Mask 1 (R)"));
                EditorGUILayout.PropertyField(Prop("maskG"), new GUIContent("Mask 2 (G)"));
                EditorGUILayout.PropertyField(Prop("maskB"), new GUIContent("Mask 3 (B)"));
                EditorGUILayout.PropertyField(Prop("maskA"), new GUIContent("Mask 4 (A)"));
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    EditorApplication.delayCall += () =>
                    {
                        if (Mgr) { Mgr.BakePackedMasks(); EditorUtility.SetDirty(Mgr); }
                    };
                }
                EditorGUILayout.PropertyField(Prop("packedMasks"), new GUIContent("Packed (auto)"));
            });

            EditorGUILayout.Space(2);
            EndDarkBox();
        }

        // DrawMasks content moved into DrawGlobal

        private int _animBrowserClipIndex;
        private bool _animBrowserOpen;

        private void DrawAnimationBrowser()
        {
            var animator = Mgr.GetComponent<Animator>();
            if (!animator || !animator.runtimeAnimatorController) return;

            var clips = animator.runtimeAnimatorController.animationClips;
            if (clips == null || clips.Length == 0) return;

            EditorGUILayout.Space(4);

            var headerRect = GUILayoutUtility.GetRect(0, 20, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(headerRect, SectionBg);
            EditorGUI.DrawRect(new Rect(headerRect.x, headerRect.y, 3, headerRect.height), new Color(0.3f, 0.7f, 0.85f));

            if (GUI.Button(headerRect, "", GUIStyle.none))
                _animBrowserOpen = !_animBrowserOpen;

            string arrow = _animBrowserOpen ? "\u25BC" : "\u25B6";
            GUI.Label(new Rect(headerRect.x + 8, headerRect.y, headerRect.width, headerRect.height),
                $"{arrow}  Animations ({clips.Length})",
                EditorStyles.miniLabel);

            if (!_animBrowserOpen) return;

            BeginDarkBox();

            var clipNames = new string[clips.Length];
            for (int i = 0; i < clips.Length; i++) clipNames[i] = clips[i].name;
            _animBrowserClipIndex = Mathf.Clamp(_animBrowserClipIndex, 0, clips.Length - 1);

            EditorGUILayout.BeginHorizontal();
            _animBrowserClipIndex = EditorGUILayout.Popup(_animBrowserClipIndex, clipNames);
            if (GUILayout.Button("Open", GUILayout.Width(46), GUILayout.Height(16)))
            {
                Selection.activeGameObject = Mgr.gameObject;
                EditorApplication.ExecuteMenuItem("Window/Animation/Animation");
                TrySetActiveClip(clips[_animBrowserClipIndex]);
            }
            EditorGUILayout.EndHorizontal();

            var clip = clips[_animBrowserClipIndex];
            var bindings = AnimationUtility.GetCurveBindings(clip);
            var mgrType = typeof(AnimatedSpriteShader);
            int count = 0;

            for (int b = 0; b < bindings.Length; b++)
            {
                if (bindings[b].type != mgrType) continue;
                count++;

                string raw = bindings[b].propertyName;
                string stripped = raw;
                string ch = "";
                if (raw.Length > 2 && raw[raw.Length - 2] == '.')
                {
                    ch = " [" + raw[raw.Length - 1] + "]";
                    stripped = raw.Substring(0, raw.Length - 2);
                }

                var curve = AnimationUtility.GetEditorCurve(clip, bindings[b]);
                string range = "";
                if (curve != null && curve.length > 0)
                {
                    float lo = float.MaxValue, hi = float.MinValue;
                    for (int k = 0; k < curve.length; k++)
                    {
                        if (curve[k].value < lo) lo = curve[k].value;
                        if (curve[k].value > hi) hi = curve[k].value;
                    }
                    range = $"{lo:F2} \u2192 {hi:F2}";
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(ObjectNames.NicifyVariableName(stripped) + ch, EditorStyles.miniLabel);
                if (range.Length > 0)
                    EditorGUILayout.LabelField(range, EditorStyles.miniLabel, GUILayout.Width(90));
                EditorGUILayout.EndHorizontal();
            }

            if (count == 0)
                EditorGUILayout.LabelField("No shader properties animated.", EditorStyles.miniLabel);

            var settings = AnimationUtility.GetAnimationClipSettings(clip);
            EditorGUILayout.LabelField(
                $"{clip.length:F2}s  \u00B7  {clip.frameRate} fps  \u00B7  {(settings.loopTime ? "Loop" : "Once")}",
                EditorStyles.centeredGreyMiniLabel);

            EndDarkBox();
        }

        private void TrySetActiveClip(AnimationClip clip)
        {
            var windowType = System.Type.GetType("UnityEditor.AnimationWindow,UnityEditor");
            if (windowType == null) return;
            var windows = Resources.FindObjectsOfTypeAll(windowType);
            if (windows.Length == 0) return;
            var animWindow = windows[0] as EditorWindow;
            var stateProp = windowType.GetProperty("state",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (stateProp == null) return;
            var state = stateProp.GetValue(animWindow);
            if (state == null) return;
            var clipProp = state.GetType().GetProperty("activeAnimationClip",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (clipProp != null && clipProp.CanWrite)
                clipProp.SetValue(state, clip);
            animWindow.Repaint();
        }

        private void DrawRemoveButton()
        {
            EditorGUILayout.Space(6);
            var old = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.45f, 0.16f, 0.16f);
            if (GUILayout.Button("Remove Component", GUILayout.Height(22)))
            {
                if (EditorUtility.DisplayDialog("Remove Animated Sprite Shader?",
                    "This will restore Sprites/Default shader.", "Remove", "Cancel"))
                {
                    var rend = Mgr.GetComponent<Renderer>();
                    if (rend) rend.sharedMaterial = new Material(Shader.Find("Sprites/Default"));
                    Undo.DestroyObjectImmediate(Mgr);
                    return;
                }
            }
            GUI.backgroundColor = old;
        }

        // Helpers

        private void ToggleSection(string boolField, string label, Action draw)
        {
            var prop = Prop(boolField);
            if (prop == null) return;
            EditorGUI.BeginChangeCheck();
            bool v = EditorGUILayout.Toggle(label, prop.boolValue);
            if (EditorGUI.EndChangeCheck()) prop.boolValue = v;
            if (v) { EditorGUI.indentLevel++; draw(); EditorGUI.indentLevel--; }
        }

        private void ButtonRow(params (string label, Color bg, Action click)[] btns)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(8);
            foreach (var b in btns)
            {
                var old = GUI.backgroundColor;
                GUI.backgroundColor = b.bg;
                if (GUILayout.Button(b.label, GUILayout.Height(22))) b.click?.Invoke();
                GUI.backgroundColor = old;
            }
            GUILayout.Space(8);
            EditorGUILayout.EndHorizontal();
        }

        private void BeginDarkBox()
        {
            var r = EditorGUILayout.BeginVertical();
            EditorGUI.DrawRect(r, SectionBg);
            GUILayout.Space(2);
        }

        private void EndDarkBox()
        {
            GUILayout.Space(2);
            EditorGUILayout.EndVertical();
        }

        // Trail Velocity

        private void DrawTrailVelocity()
        {
            bool anyTrail = false;
            var tA = Prop("colorfulTrailAEnabled");
            var tB = Prop("colorfulTrailBEnabled");
            var tC = Prop("colorfulTrailCEnabled");
            if (tA != null && tA.boolValue) anyTrail = true;
            if (tB != null && tB.boolValue) anyTrail = true;
            if (tC != null && tC.boolValue) anyTrail = true;
            var smearEn = Prop("motionSmearEnabled");
            bool anySmear = smearEn != null && smearEn.boolValue;

            // Only draw if at least one trail or smear is active
            if (!anyTrail && !anySmear) return;

            // Sub-section Header
            var headerRect = GUILayoutUtility.GetRect(0, 18, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(headerRect, new Color(0.12f, 0.12f, 0.14f, 1));
            GUI.Label(
                new Rect(headerRect.x + 6, headerRect.y, headerRect.width - 12, headerRect.height),
                "Velocity Auto-Source (Trails & Smear)",
                EditorStyles.boldLabel);

            EditorGUILayout.Space(2);
            EditorGUILayout.HelpBox(
                "Set velocity source for trail and smear effects. Auto sources use smoothing with decay for stable results.",
                MessageType.None);

            // Decay slider
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(Prop("velocityDecay"), new GUIContent("Velocity Decay",
                "Decay multiplier per second (frame-rate independent). Higher = longer trail persistence. "
                + "0.95 is default, 0.99 for very persistent trails."));
            EditorGUI.indentLevel--;

            // Trail velocity source
            if (anyTrail)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(Prop("trailVelocitySource"), new GUIContent("Trail Source",
                    "Manual: set trailAVelocity etc from script.\nRigidbody2D: auto-read from Rigidbody2D.velocity.\nTransform: computed from position delta each frame."));
                var src = Prop("trailVelocitySource");
                if (src != null)
                {
                    if (src.enumValueIndex == 0) // Manual
                    {
                        EditorGUILayout.HelpBox("Set trailAVelocity / trailBVelocity / trailCVelocity from your movement script.", MessageType.None);
                        if (tA != null && tA.boolValue) EditorGUILayout.PropertyField(Prop("trailAVelocity"), new GUIContent("Trail A Velocity"));
                        if (tB != null && tB.boolValue) EditorGUILayout.PropertyField(Prop("trailBVelocity"), new GUIContent("Trail B Velocity"));
                        if (tC != null && tC.boolValue) EditorGUILayout.PropertyField(Prop("trailCVelocity"), new GUIContent("Trail C Velocity"));
                    }
                    else if (src.enumValueIndex == 1) // Rigidbody2D
                        EditorGUILayout.HelpBox("Velocity auto-read from Rigidbody2D.velocity each frame.", MessageType.None);
                    else // Transform
                        EditorGUILayout.HelpBox("Velocity computed from transform.position delta / deltaTime.", MessageType.None);
                }
                EditorGUI.indentLevel--;
            }

            // Motion Smear velocity source
            if (anySmear)
            {
                EditorGUILayout.Space(2);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(Prop("smearVelocitySource"), new GUIContent("Smear Source",
                    "Manual: set smearDirection from script.\nRigidbody2D: auto-read from Rigidbody2D.velocity.\nTransform: computed from position delta each frame."));
                var smearSrc = Prop("smearVelocitySource");
                if (smearSrc != null)
                {
                    if (smearSrc.enumValueIndex == 0) // Manual
                    {
                        EditorGUILayout.HelpBox("Set smearDirection from your movement script, or switch to auto source.", MessageType.None);
                        EditorGUILayout.PropertyField(Prop("smearDirection"), new GUIContent("Smear Direction"));
                    }
                    else if (smearSrc.enumValueIndex == 1)
                        EditorGUILayout.HelpBox("Smear direction auto-read from Rigidbody2D.velocity each frame.", MessageType.None);
                    else
                        EditorGUILayout.HelpBox("Smear direction computed from transform.position delta / deltaTime.", MessageType.None);
                }
                EditorGUI.indentLevel--;
            }

            // Divider line separating the source block from the individual effect foldouts
            var div = GUILayoutUtility.GetRect(0, 1, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(div, new Color(0.18f, 0.18f, 0.20f, 1));
            EditorGUILayout.Space(4);
        }

    }
    }
