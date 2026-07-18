using System.Collections.Generic;


namespace FlexiSprite
{
    public enum ShowcaseAnimMode { PingPong, SawtoothUp, SawtoothDown }

    [System.Serializable]
    public struct ShowcaseAnimClip
    {
        public string field;
        public float from;
        public float to;
        public float period;
        public ShowcaseAnimMode mode;

        public ShowcaseAnimClip(string field, float from, float to, float period, ShowcaseAnimMode mode = ShowcaseAnimMode.PingPong)
        {
            this.field = field;
            this.from = from;
            this.to = to;
            this.period = period;
            this.mode = mode;
        }

        public float Evaluate(float time)
        {
            if (period <= 0f) return to;
            float t = time / period;

            switch (mode)
            {
                case ShowcaseAnimMode.PingPong:
                    t = t % 1f;
                    t = t < 0.5f ? t * 2f : 2f - t * 2f;
                    break;
                case ShowcaseAnimMode.SawtoothUp:
                    t = t % 1f;
                    break;
                case ShowcaseAnimMode.SawtoothDown:
                    t = 1f - (t % 1f);
                    break;
            }

            return from + (to - from) * t;
        }
    }

    public static class EffectShowcaseAutoAnim
    {
   
            static readonly Dictionary<EffectId, ShowcaseAnimClip[]> _overrides = new()
            {
                [EffectId.StrongTint] = new[] { Pp("strongTintFade", 0f, 1f, 3f) },
                [EffectId.AddColor] = new[] { Pp("addColorFade", 0f, 1f, 3f) },
                [EffectId.AlphaTint] = new[] { Pp("alphaTintFade", 0f, 1f, 3f) },
                [EffectId.HitEffect] = new[] { Pp("hitEffectBlend", 0f, 1f, 3f) },
                [EffectId.Negative] = new[] { Pp("negativeAmount", 0f, 1f, 3f) },
                [EffectId.Greyscale] = new[] { Pp("greyscaleBlend", 0f, 1f, 3f) },
                [EffectId.Bloom] = new[]
                {
                    Pp("bloomThreshold", 0f, 1f, 3f),
                    Pp("bloomIntensity", 0f, 10f, 3f),
                    Pp("bloomSize", 0f, 0.02f, 3f),
                    Pp("bloomSoftKnee", 0f, 1f, 3f),
                    Pp("bloomMaskRange", 0f, 1f, 3f),
                    Pp("bloomMaskSoft", 0f, 1f, 3f),
                    Pp("bloomAlphaBleed", 0f, 1f, 3f),
                },
                [EffectId.ColorReplace] = new[] { Pp("colorReplaceRange", 0f, 1f, 3f), Pp("colorReplaceFade", 0f, 1f, 3f) },
                [EffectId.LUTReplace] = new[] { Pp("lutReplaceFade", 0f, 1f, 3f) },
                [EffectId.HueShift] = new[] { Saw("hueShiftAmount", 0f, 1f, 4f), Saw("hueShiftFade", 0f, 1f, 4f) },
                [EffectId.AddHue] = new[]
                {
                    Saw("addHueAmount", 0f, 1f, 4f),
                    Saw("addHueSaturation", 0f, 1f, 4f),
                    Saw("addHueBrightness", 0f, 2f, 4f),
                    Saw("addHueFade", 0f, 1f, 4f),
                },
                [EffectId.Saturation] = new[] { Pp("saturationAmount", 0f, 3f, 3f) },
                [EffectId.Brightness] = new[] { Pp("brightnessAmount", -1f, 1f, 3f) },
                [EffectId.Contrast] = new[] { Pp("contrastAmount", 0f, 3f, 3f) },
                [EffectId.SplitToning] = new[] { Pp("splitBalance", -1f, 1f, 3f), Pp("splitFade", 0f, 1f, 3f) },
                [EffectId.Posterize] = new[] { Pp("posterizeSteps", 2f, 32f, 3f), Pp("posterizeFade", 0f, 1f, 3f) },
                [EffectId.Poison] = new[] { Pp("poisonDensity", 0f, 30f, 3f), Pp("poisonFade", 0f, 1f, 3f) },
                [EffectId.Frozen] = new[]
                {
                    Pp("frozenContrast", 0f, 3f, 3f),
                    Pp("frozenHighlight", 0f, 3f, 3f),
                    Pp("frozenFade", 0f, 1f, 3f),
                },
                [EffectId.Metal] = new[] { Pp("metalContrast", 0f, 3f, 3f), Pp("metalFade", 0f, 1f, 3f) },
                [EffectId.Camouflage] = new[] { Pp("camoScale", 0f, 30f, 3f), Pp("camoFade", 0f, 1f, 3f) },
                [EffectId.Rainbow] = new[]
                {
                    Pp("rainbowSaturation", 0f, 1f, 3f),
                    Pp("rainbowBrightness", 0f, 2f, 3f),
                    Pp("rainbowFade", 0f, 1f, 3f),
                    Pp("rainbowDensity", 0f, 10f, 3f),
                },
                [EffectId.BlackTint] = new[] { Pp("blackTintThreshold", 0f, 1f, 3f), Pp("blackTintFade", 0f, 1f, 3f) },
           
                [EffectId.Chromatic] = new[] { Pp("chromaticAmount", 0f, 0.1f, 3f), Pp("chromaticAngle", -3.15f, 3.15f, 3f) },
                [EffectId.PaletteSwap] = new[] { Pp("paletteSwapFade", 0f, 1f, 3f) },
                [EffectId.Dithering] = new[]
                {
                    Pp("ditheringSize", 1f, 16f, 3f),
                    Pp("ditheringLevels", 2f, 16f, 3f),
                    Pp("ditheringFade", 0f, 1f, 3f),
                    Pp("ditheringAlphaThreshold", 0f, 1f, 3f),
                },
                [EffectId.CRTScanlines] = new[]
                {
                    Pp("crtDensity", 10f, 500f, 3f),
                    Pp("crtIntensity", 0f, 1f, 3f),
                    Pp("crtCurvature", 0f, 0.5f, 3f),
                },
                [EffectId.Vignette] = new[]
                {
                    Pp("vignetteIntensity", 0f, 3f, 3f),
                    Pp("vignetteSmoothness", 0f, 1f, 3f),
                    Pp("vignetteAlpha", 0f, 1f, 3f),
                },
                [EffectId.ColorGrading] = new[] { Pp("cgExposure", -3f, 3f, 3f), Pp("cgGamma", 0.1f, 3f, 3f) },
                [EffectId.Sepia] = new[] { Pp("sepiaIntensity", 0f, 1f, 3f) },
                [EffectId.ThermalVision] = new[] { Pp("thermalIntensity", 0f, 1f, 3f) },
                [EffectId.RecolorRGB] = new[] { Pp("recolorFade", 0f, 1f, 3f) },
                [EffectId.PingPongGlow] = new[] { Pp("pingPongGlowMin", 0f, 3f, 3f), Pp("pingPongGlowMax", 0f, 3f, 3f) },
                [EffectId.ColorCycle] = new[] { Pp("colorCycleFade", 0f, 1f, 3f) },
                [EffectId.Outline] = new[] { Pp("outlineSize", 0f, 0.1f, 3f), Pp("outlineAlpha", 0f, 1f, 3f) },
                [EffectId.InnerOutline] = new[]
                {
                    Pp("innerOutlineThickness", 0f, 0.05f, 3f),
                    Pp("innerOutlineAlpha", 0f, 1f, 3f),
                },
                [EffectId.TexOutline] = new[] { Pp("texOutlineSize", 0f, 0.1f, 3f) },
                [EffectId.RainbowOutline] = new[] { Pp("rainbowOutlineSize", 0f, 0.1f, 3f) },
                [EffectId.DistortOutline] = new[]
                {
                    Pp("distortOutlineSize", 0f, 0.1f, 3f),
                    Pp("distortOutlineAmount", 0f, 0.5f, 3f),
                },
                [EffectId.Glow] = new[] { Pp("glowSize", 0f, 0.15f, 3f), Pp("glowIntensity", 0f, 10f, 3f) },
                [EffectId.Shadow] = System.Array.Empty<ShowcaseAnimClip>(), // Uses Vector4 (Offset)
                [EffectId.SoftShadow] = new[] { Pp("softShadowSoftness", 0f, 0.05f, 3f) },
                [EffectId.OutlineOnly] = new[] { Pp("outlineOnlyWidth", 0f, 0.1f, 3f) },
                [EffectId.GradientOutline] = new[]
                {
                    Pp("gradOutlineAngle", -3.15f, 3.15f, 3f),
                    Pp("gradOutlineSize", 0f, 0.1f, 3f),
                },
                [EffectId.AnimOutlineThick] = new[] { Pp("animOutlineMin", 0f, 0.05f, 3f), Pp("animOutlineMax", 0f, 0.1f, 3f) },
                [EffectId.SelectionHighlight] = new[]
                {
                    Pp("selectHighlightSize", 0f, 0.1f, 3f),
                    Pp("selectHighlightDash", 2f, 50f, 3f),
                },
                [EffectId.Pixelate] = new[] { Pp("pixelateSize", 32f, 128f, 3f) },
                [EffectId.Wave] = new[] { Pp("waveAmplitude", 0f, 0.5f, 3f), Pp("waveFrequency", 0f, 30f, 3f) },
                [EffectId.Twist] = new[] { Pp("twistAmount", -10f, 10f, 3f), Pp("twistRadius", 0f, 2f, 3f) },
                [EffectId.UVDistort] = new[] { Pp("distortAmount", 0f, 0.5f, 3f) },
                [EffectId.FullDistort] = new[] { Pp("fullDistortAmount", 0f, 1f, 3f), Pp("fullDistortDensity", 1f, 30f, 3f) },
                [EffectId.DirDistort] = new[] { Pp("dirDistortAngle", -3.15f, 3.15f, 3f), Pp("dirDistortAmount", 0f, 0.5f, 3f) },
                [EffectId.Rotate] = new[] { Saw("rotateAmount", -3.15f, 3.15f, 4f) },
                [EffectId.Squeeze] = new[] { Pp("squeezePower", 0f, 1f, 3f) },
                [EffectId.Squish] = new[] { Pp("squishPower", 0f, 1f, 3f) },
                [EffectId.Wiggle] = new[] { Pp("wiggleAmount", 0f, 0.3f, 3f), Pp("wiggleFrequency", 0f, 30f, 3f) },
                [EffectId.Wind] = new[]
                {
                    Pp("windAmount", 0f, 0.5f, 3f),
                    Pp("windBottomY", 0f, 1f, 3f),
                    Pp("windFrequency", 0f, 10f, 3f),
                },
                [EffectId.Sharpen] = new[] { Pp("sharpenAmount", 0f, 5f, 3f), Pp("sharpenSize", 0f, 0.01f, 3f) },
                [EffectId.GaussBlur] = new[] { Pp("gaussBlurSize", 0f, 0.05f, 3f) },
                [EffectId.DirBlur] = new[] { Pp("dirBlurAngle", -3.15f, 3.15f, 3f), Pp("dirBlurSize", 0f, 0.1f, 3f) },
                [EffectId.Shockwave] = new[]
                {
                    Pp("shockwaveRadius", 0f, 2f, 3f),
                    Pp("shockwaveWidth", 0.01f, 0.3f, 3f),
                    Pp("shockwaveAmplitude", 0f, 0.3f, 3f),
                },
                [EffectId.SineScale] = new[] { Pp("sineScaleAmountX", 0f, 0.5f, 3f), Pp("sineScaleAmountY", 0f, 0.5f, 3f) },
                [EffectId.Heartbeat] = new[] { Pp("heartbeatAmount", 0f, 0.3f, 3f) },
                [EffectId.Breathing] = new[] { Pp("breathingAmount", 0f, 0.3f, 3f) },
                [EffectId.LiquidWobble] = new[] { Pp("liquidWobbleAmount", 0f, 0.3f, 3f), Pp("liquidWobbleFreq", 1f, 30f, 3f) },
                [EffectId.FlipBookBlend] = new[] { Pp("flipBookLerp", 0f, 1f, 3f) },
                [EffectId.SmoothPixelArt] = new[] { Pp("smoothPixelArtTexelSize", 0.001f, 0.05f, 3f) },
                [EffectId.Dissolve] = new[] { Pp("dissolveAmount", 0f, 1f, 3f), Pp("dissolveBorderWidth", 0f, 0.3f, 3f) },
                [EffectId.GlowDissolve] = new[] { Pp("glowDissolveAmount", 0f, 1f, 3f), Pp("glowDissolveWidth", 0f, 0.5f, 3f) },
                [EffectId.SrcDissolve] = new[] { Pp("srcDissolveAmount", 0f, 1f, 3f) },
                [EffectId.DirFade] = new[] { Pp("dirFadeAngle", -3.15f, 3.15f, 3f), Pp("dirFadeAmount", 0f, 1f, 3f) },
                [EffectId.Halftone] = new[] { Pp("halftoneSize", 1f, 100f, 3f), Pp("halftoneAmount", 0f, 1f, 3f) },
                [EffectId.RadialWipe] = new[] { Pp("radialWipeAmount", 0f, 1f, 3f), Pp("radialWipeAngle", -3.15f, 3.15f, 4f) },
                [EffectId.DirGlowFade] = new[] { Pp("dirGlowFadeAmount", 0f, 1f, 3f) },
                [EffectId.SpreadFade] = new[] { Pp("spreadFadeAmount", 0f, 0.65f, 3f) },
                [EffectId.Blinds] = new[] { Pp("blindsAmount", 0f, 1f, 3f), Pp("blindsCount", 2f, 30f, 3f) },
                [EffectId.SpiralWipe] = new[] { Pp("spiralWipeAmount", 0f, 1f, 3f), Pp("spiralWipeArms", 0f, 10f, 4f) },
                [EffectId.Hologram] = new[]
                {
                    Pp("hologramStripesAmount", 1f, 200f, 3f),
                    Pp("hologramBlend", 0f, 1f, 3f),
                    Pp("hologramDistortion", 0f, 0.1f, 3f),
                },
                [EffectId.Glitch] = new[] { Pp("glitchAmount", 0f, 1f, 3f), Pp("glitchSize", 1f, 100f, 3f) },
                [EffectId.Shine] = new[]
                {
                    Pp("shineWidth", 0.01f, 1f, 3f),
                    Pp("shineRotation", -1f, 1f, 3f),
                    Pp("shineGlow", 0f, 3f, 3f),
                },
                [EffectId.Smoke] = new[] { Pp("smokeDensity", 0f, 20f, 3f), Pp("smokeFade", 0f, 1f, 3f) },
                [EffectId.Flame] = new[] { Pp("flameIntensity", 0f, 3f, 3f) },
                [EffectId.HealthTint] = new[] { Pp("healthTintAmount", 0f, 1f, 3f), Pp("healthDesaturation", 0f, 1f, 3f) },
                [EffectId.VoronoiCrackle] = new[] { Pp("crackleScale", 1f, 50f, 3f), Pp("crackleFade", 0f, 1f, 3f) },
                [EffectId.WaterReflect] = new[] { Pp("waterReflectOpacity", 0f, 1f, 3f), Pp("waterReflectFade", 0f, 1f, 3f) },
                [EffectId.EnergyAura] = new[]
                {
                    Pp("auraRadius", 0f, 0.5f, 3f),
                    Pp("auraIntensity", 0f, 3f, 3f),
                    Pp("auraFade", 0f, 1f, 3f),
                },
            };
       

      static ShowcaseAnimClip Pp(string field, float from, float to, float period)
            => new(field, from, to, period, ShowcaseAnimMode.PingPong);

        static ShowcaseAnimClip Saw(string field, float from, float to, float period)
            => new(field, from, to, period, ShowcaseAnimMode.SawtoothUp);

        public static ShowcaseAnimClip[] GetClips(EffectId id)
        {
            if (_overrides.TryGetValue(id, out var clips))
                return clips;

            return AutoDetectClips(id);
        }

        static ShowcaseAnimClip[] AutoDetectClips(EffectId id)
        {
            var def = EffectRegistry.FindById(id);
            if (def == null || def.parms == null || def.parms.Length == 0)
                return System.Array.Empty<ShowcaseAnimClip>();

            for (int i = 0; i < def.parms.Length; i++)
            {
                var p = def.parms[i];
                if (p.type != ParamType.Float) continue;

                string lower = p.field.ToLowerInvariant();
                if (lower.Contains("speed") || lower.Contains("maskch")) continue;

                if (lower.Contains("fade") || lower.Contains("blend") || lower.Contains("amount") || lower.Contains("intensity"))
                    return new[] { Pp(p.field, p.min, p.max, 3f) };
            }

            for (int i = 0; i < def.parms.Length; i++)
            {
                var p = def.parms[i];
                if (p.type != ParamType.Float) continue;

                string lower = p.field.ToLowerInvariant();
                if (lower.Contains("speed") || lower.Contains("maskch")) continue;

                return new[] { Pp(p.field, p.min, p.max, 3f) };
            }

            return System.Array.Empty<ShowcaseAnimClip>();
        }
    }
}
