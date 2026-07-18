// AUTO-GENERATED — do not edit. Run AnimatedSprite/Generate Push Methods.
namespace FlexiSprite
{
using UnityEngine;

public partial class AnimatedSpriteShader
{
    void PushEffect_StrongTint(MaterialPropertyBlock b)
    {
        b.SetFloat("_StrongTint_En", strongTintEnabled ? 1f : 0f);
        b.SetFloat("_StrongTintMaskCh", (float)strongTintMaskCh);
        b.SetColor("_StrongTintColor", strongTintColor);
        b.SetFloat("_StrongTintFade", strongTintFade);
    }

    void PushEffect_AddColor(MaterialPropertyBlock b)
    {
        b.SetFloat("_AddColor_En", addColorEnabled ? 1f : 0f);
        b.SetFloat("_AddColorMaskCh", (float)addColorMaskCh);
        b.SetColor("_AddColorAmount", addColorAmount);
        b.SetFloat("_AddColorFade", addColorFade);
    }

    void PushEffect_AlphaTint(MaterialPropertyBlock b)
    {
        b.SetFloat("_AlphaTint_En", alphaTintEnabled ? 1f : 0f);
        b.SetFloat("_AlphaTintMaskCh", (float)alphaTintMaskCh);
        b.SetColor("_AlphaTintColor", alphaTintColor);
        b.SetFloat("_AlphaTintFade", alphaTintFade);
    }

    void PushEffect_HitEffect(MaterialPropertyBlock b)
    {
        b.SetFloat("_HitEffect_En", hitEffectEnabled ? 1f : 0f);
        b.SetFloat("_HitEffectMaskCh", (float)hitEffectMaskCh);
        b.SetColor("_HitEffectColor", hitEffectColor);
        b.SetFloat("_HitEffectBlend", hitEffectBlend);
    }

    void PushEffect_Negative(MaterialPropertyBlock b)
    {
        b.SetFloat("_Negative_En", negativeEnabled ? 1f : 0f);
        b.SetFloat("_NegativeMaskCh", (float)negativeMaskCh);
        b.SetFloat("_NegativeAmount", negativeAmount);
    }

    void PushEffect_Greyscale(MaterialPropertyBlock b)
    {
        b.SetFloat("_Greyscale_En", greyscaleEnabled ? 1f : 0f);
        b.SetFloat("_GreyscaleMaskCh", (float)greyscaleMaskCh);
        b.SetFloat("_GreyscaleBlend", greyscaleBlend);
        b.SetVector("_GreyscaleLuminosity", greyscaleLuminosity);
    }

    void PushEffect_Bloom(MaterialPropertyBlock b)
    {
        b.SetFloat("_Bloom_En", bloomEnabled ? 1f : 0f);
        b.SetFloat("_BloomMaskCh", (float)bloomMaskCh);
        b.SetFloat("_BloomMode", (float)bloomMode);
        b.SetColor("_BloomColor", bloomColor);
        b.SetFloat("_BloomThreshold", bloomThreshold);
        b.SetFloat("_BloomIntensity", bloomIntensity);
        b.SetFloat("_BloomSize", bloomSize);
        b.SetFloat("_BloomSamples", (float)bloomSamples);
        b.SetFloat("_BloomSoftKnee", bloomSoftKnee);
        b.SetColor("_BloomMaskColor", bloomMaskColor);
        b.SetFloat("_BloomMaskRange", bloomMaskRange);
        b.SetFloat("_BloomMaskSoft", bloomMaskSoft);
        b.SetFloat("_BloomAlphaBleed", bloomAlphaBleed);
    }

    void PushEffect_ColorReplace(MaterialPropertyBlock b)
    {
        b.SetFloat("_ColorReplace_En", colorReplaceEnabled ? 1f : 0f);
        b.SetFloat("_ColorReplaceMaskCh", (float)colorReplaceMaskCh);
        b.SetColor("_ColorReplaceSrc1", colorReplaceSrc1);
        b.SetColor("_ColorReplaceDst1", colorReplaceDst1);
        b.SetColor("_ColorReplaceSrc2", colorReplaceSrc2);
        b.SetColor("_ColorReplaceDst2", colorReplaceDst2);
        b.SetColor("_ColorReplaceSrc3", colorReplaceSrc3);
        b.SetColor("_ColorReplaceDst3", colorReplaceDst3);
        b.SetColor("_ColorReplaceSrc4", colorReplaceSrc4);
        b.SetColor("_ColorReplaceDst4", colorReplaceDst4);
        b.SetFloat("_ColorReplaceCount", (float)colorReplaceCount);
        b.SetFloat("_ColorReplaceRange", colorReplaceRange);
        b.SetFloat("_ColorReplaceFade", colorReplaceFade);
    }

    void PushEffect_LUTReplace(MaterialPropertyBlock b)
    {
        b.SetFloat("_LUTReplace_En", lutReplaceEnabled ? 1f : 0f);
        b.SetFloat("_LUTReplaceMaskCh", (float)lutReplaceMaskCh);
        if (lutReplaceTexture) b.SetTexture("_LUTReplaceTex", lutReplaceTexture);
        b.SetFloat("_LUTSize", lutSize);
        b.SetFloat("_LUTReplaceFade", lutReplaceFade);
    }

    void PushEffect_HueShift(MaterialPropertyBlock b)
    {
        b.SetFloat("_HueShift_En", hueShiftEnabled ? 1f : 0f);
        b.SetFloat("_HueShiftMaskCh", (float)hueShiftMaskCh);
        b.SetFloat("_HueShiftAmount", hueShiftAmount);
        b.SetFloat("_HueShiftSpeed", hueShiftSpeed);
        b.SetFloat("_HueShiftFade", hueShiftFade);
    }

    void PushEffect_AddHue(MaterialPropertyBlock b)
    {
        b.SetFloat("_AddHue_En", addHueEnabled ? 1f : 0f);
        b.SetFloat("_AddHueMaskCh", (float)addHueMaskCh);
        b.SetFloat("_AddHueAmount", addHueAmount);
        b.SetFloat("_AddHueSpeed", addHueSpeed);
        b.SetFloat("_AddHueSaturation", addHueSaturation);
        b.SetFloat("_AddHueBrightness", addHueBrightness);
        b.SetFloat("_AddHueFade", addHueFade);
    }

    void PushEffect_Saturation(MaterialPropertyBlock b)
    {
        b.SetFloat("_Saturation_En", saturationEnabled ? 1f : 0f);
        b.SetFloat("_SaturationMaskCh", (float)saturationMaskCh);
        b.SetFloat("_SaturationAmount", saturationAmount);
    }

    void PushEffect_Brightness(MaterialPropertyBlock b)
    {
        b.SetFloat("_Brightness_En", brightnessEnabled ? 1f : 0f);
        b.SetFloat("_BrightnessMaskCh", (float)brightnessMaskCh);
        b.SetFloat("_BrightnessAmount", brightnessAmount);
    }

    void PushEffect_Contrast(MaterialPropertyBlock b)
    {
        b.SetFloat("_Contrast_En", contrastEnabled ? 1f : 0f);
        b.SetFloat("_ContrastMaskCh", (float)contrastMaskCh);
        b.SetFloat("_ContrastAmount", contrastAmount);
    }

    void PushEffect_SplitToning(MaterialPropertyBlock b)
    {
        b.SetFloat("_SplitToning_En", splitToningEnabled ? 1f : 0f);
        b.SetFloat("_SplitToningMaskCh", (float)splitToningMaskCh);
        b.SetColor("_SplitShadowColor", splitShadowColor);
        b.SetColor("_SplitHighlightColor", splitHighlightColor);
        b.SetFloat("_SplitBalance", splitBalance);
        b.SetColor("_SplitMidColor", splitMidColor);
        b.SetFloat("_SplitFade", splitFade);
    }

    void PushEffect_Posterize(MaterialPropertyBlock b)
    {
        b.SetFloat("_Posterize_En", posterizeEnabled ? 1f : 0f);
        b.SetFloat("_PosterizeMaskCh", (float)posterizeMaskCh);
        b.SetFloat("_PosterizeSteps", posterizeSteps);
        b.SetFloat("_PosterizeFade", posterizeFade);
    }

    void PushEffect_Poison(MaterialPropertyBlock b)
    {
        b.SetFloat("_Poison_En", poisonEnabled ? 1f : 0f);
        b.SetFloat("_PoisonMaskCh", (float)poisonMaskCh);
        b.SetColor("_PoisonColor", poisonColor);
        b.SetFloat("_PoisonSpeed", poisonSpeed);
        b.SetFloat("_PoisonDensity", poisonDensity);
        b.SetColor("_PoisonColor2", poisonColor2);
        b.SetColor("_PoisonColor3", poisonColor3);
        b.SetFloat("_PoisonFade", poisonFade);
    }

    void PushEffect_Frozen(MaterialPropertyBlock b)
    {
        b.SetFloat("_Frozen_En", frozenEnabled ? 1f : 0f);
        b.SetFloat("_FrozenMaskCh", (float)frozenMaskCh);
        b.SetColor("_FrozenColor", frozenColor);
        b.SetFloat("_FrozenContrast", frozenContrast);
        b.SetFloat("_FrozenHighlight", frozenHighlight);
        b.SetColor("_FrozenColor2", frozenColor2);
        b.SetColor("_FrozenHighlightColor", frozenHighlightColor);
        b.SetFloat("_FrozenFade", frozenFade);
    }

    void PushEffect_Metal(MaterialPropertyBlock b)
    {
        b.SetFloat("_Metal_En", metalEnabled ? 1f : 0f);
        b.SetFloat("_MetalMaskCh", (float)metalMaskCh);
        b.SetColor("_MetalColor", metalColor);
        b.SetColor("_MetalHighlight", metalHighlight);
        b.SetFloat("_MetalContrast", metalContrast);
        b.SetFloat("_MetalSpeed", metalSpeed);
        b.SetColor("_MetalColor2", metalColor2);
        b.SetFloat("_MetalFade", metalFade);
    }

    void PushEffect_Camouflage(MaterialPropertyBlock b)
    {
        b.SetFloat("_Camouflage_En", camouflageEnabled ? 1f : 0f);
        b.SetFloat("_CamouflageMaskCh", (float)camouflageMaskCh);
        b.SetColor("_CamoColor1", camoColor1);
        b.SetColor("_CamoColor2", camoColor2);
        b.SetColor("_CamoColor3", camoColor3);
        b.SetFloat("_CamoScale", camoScale);
        b.SetFloat("_CamoSpeed", camoSpeed);
        b.SetFloat("_CamoFade", camoFade);
    }

    void PushEffect_Rainbow(MaterialPropertyBlock b)
    {
        b.SetFloat("_Rainbow_En", rainbowEnabled ? 1f : 0f);
        b.SetFloat("_RainbowMaskCh", (float)rainbowMaskCh);
        b.SetFloat("_RainbowSpeed", rainbowSpeed);
        b.SetFloat("_RainbowSaturation", rainbowSaturation);
        b.SetFloat("_RainbowBrightness", rainbowBrightness);
        b.SetFloat("_RainbowFade", rainbowFade);
        b.SetFloat("_RainbowDensity", rainbowDensity);
    }

    void PushEffect_BlackTint(MaterialPropertyBlock b)
    {
        b.SetFloat("_BlackTint_En", blackTintEnabled ? 1f : 0f);
        b.SetFloat("_BlackTintMaskCh", (float)blackTintMaskCh);
        b.SetColor("_BlackTintColor", blackTintColor);
        b.SetFloat("_BlackTintThreshold", blackTintThreshold);
        b.SetFloat("_BlackTintFade", blackTintFade);
    }

    void PushEffect_Chromatic(MaterialPropertyBlock b)
    {
        b.SetFloat("_Chromatic_En", chromaticEnabled ? 1f : 0f);
        b.SetFloat("_ChromaticMaskCh", (float)chromaticMaskCh);
        b.SetFloat("_ChromaticAmount", chromaticAmount);
        b.SetFloat("_ChromaticAngle", chromaticAngle);
    }

    void PushEffect_PaletteSwap(MaterialPropertyBlock b)
    {
        b.SetFloat("_PaletteSwap_En", paletteSwapEnabled ? 1f : 0f);
        b.SetFloat("_PaletteSwapMaskCh", (float)paletteSwapMaskCh);
        if (paletteSwapTexture) b.SetTexture("_PaletteSwapTex", paletteSwapTexture);
        b.SetFloat("_PaletteSwapColors", (float)paletteSwapColors);
        b.SetFloat("_PaletteSwapFade", paletteSwapFade);
    }

    void PushEffect_Dithering(MaterialPropertyBlock b)
    {
        b.SetFloat("_Dithering_En", ditheringEnabled ? 1f : 0f);
        b.SetFloat("_DitheringMaskCh", (float)ditheringMaskCh);
        b.SetFloat("_DitheringSize", ditheringSize);
        b.SetFloat("_DitheringLevels", ditheringLevels);
        b.SetFloat("_DitheringFade", ditheringFade);
        b.SetFloat("_DitheringAlphaMode", (float)ditheringAlphaMode);
        b.SetFloat("_DitheringAlphaThreshold", ditheringAlphaThreshold);
        b.SetFloat("_DitheringScreenSpace", ditheringScreenSpace ? 1f : 0f);
    }

    void PushEffect_CRTScanlines(MaterialPropertyBlock b)
    {
        b.SetFloat("_CRTScanlines_En", crtScanlinesEnabled ? 1f : 0f);
        b.SetFloat("_CRTScanlinesMaskCh", (float)crtScanlinesMaskCh);
        b.SetFloat("_CRTDensity", crtDensity);
        b.SetFloat("_CRTIntensity", crtIntensity);
        b.SetFloat("_CRTSpeed", crtSpeed);
        b.SetFloat("_CRTCurvature", crtCurvature);
    }

    void PushEffect_Vignette(MaterialPropertyBlock b)
    {
        b.SetFloat("_Vignette_En", vignetteEnabled ? 1f : 0f);
        b.SetFloat("_VignetteMaskCh", (float)vignetteMaskCh);
        b.SetColor("_VignetteColor", vignetteColor);
        b.SetFloat("_VignetteIntensity", vignetteIntensity);
        b.SetFloat("_VignetteSmoothness", vignetteSmoothness);
        b.SetFloat("_VignetteAlpha", vignetteAlpha);
        b.SetVector("_VignetteCenter", vignetteCenter);
        b.SetVector("_VignetteOffset", new Vector4(vignetteOffset.x, vignetteOffset.y, 0, 0));
    }

    void PushEffect_ColorGrading(MaterialPropertyBlock b)
    {
        b.SetFloat("_ColorGrading_En", colorGradingEnabled ? 1f : 0f);
        b.SetFloat("_ColorGradingMaskCh", (float)colorGradingMaskCh);
        b.SetFloat("_CGExposure", cgExposure);
        b.SetFloat("_CGGamma", cgGamma);
        b.SetVector("_CGLift", cgLift);
    }

    void PushEffect_Sepia(MaterialPropertyBlock b)
    {
        b.SetFloat("_Sepia_En", sepiaEnabled ? 1f : 0f);
        b.SetFloat("_SepiaMaskCh", (float)sepiaMaskCh);
        b.SetFloat("_SepiaIntensity", sepiaIntensity);
        b.SetColor("_SepiaColor", sepiaColor);
    }

    void PushEffect_ThermalVision(MaterialPropertyBlock b)
    {
        b.SetFloat("_ThermalVision_En", thermalVisionEnabled ? 1f : 0f);
        b.SetFloat("_ThermalVisionMaskCh", (float)thermalVisionMaskCh);
        b.SetFloat("_ThermalIntensity", thermalIntensity);
        b.SetFloat("_ThermalHDRGlow", thermalHDRGlow);
    }

    void PushEffect_RecolorRGB(MaterialPropertyBlock b)
    {
        b.SetFloat("_RecolorRGB_En", recolorRGBEnabled ? 1f : 0f);
        b.SetFloat("_RecolorRGBMaskCh", (float)recolorRGBMaskCh);
        b.SetColor("_RecolorR", recolorR);
        b.SetColor("_RecolorG", recolorG);
        b.SetColor("_RecolorB", recolorB);
        b.SetColor("_RecolorCyan", recolorCyan);
        b.SetColor("_RecolorMagenta", recolorMagenta);
        b.SetColor("_RecolorYellow", recolorYellow);
        b.SetFloat("_RecolorFade", recolorFade);
    }

    void PushEffect_PingPongGlow(MaterialPropertyBlock b)
    {
        b.SetFloat("_PingPongGlow_En", pingPongGlowEnabled ? 1f : 0f);
        b.SetFloat("_PingPongGlowMaskCh", (float)pingPongGlowMaskCh);
        b.SetColor("_PingPongGlowColor", pingPongGlowColor);
        b.SetFloat("_PingPongGlowSpeed", pingPongGlowSpeed);
        b.SetFloat("_PingPongGlowMin", pingPongGlowMin);
        b.SetFloat("_PingPongGlowMax", pingPongGlowMax);
    }

    void PushEffect_ColorCycle(MaterialPropertyBlock b)
    {
        b.SetFloat("_ColorCycle_En", colorCycleEnabled ? 1f : 0f);
        b.SetFloat("_ColorCycleMaskCh", (float)colorCycleMaskCh);
        b.SetColor("_ColorCycleA", colorCycleA);
        b.SetColor("_ColorCycleB", colorCycleB);
        b.SetColor("_ColorCycleC", colorCycleC);
        b.SetColor("_ColorCycleD", colorCycleD);
        b.SetFloat("_ColorCycleSpeed", colorCycleSpeed);
        b.SetFloat("_ColorCycleFade", colorCycleFade);
    }

    void PushEffect_Outline(MaterialPropertyBlock b)
    {
        b.SetFloat("_Outline_En", outlineEnabled ? 1f : 0f);
        b.SetFloat("_OutlineMaskCh", (float)outlineMaskCh);
        b.SetColor("_OutlineColor", outlineColor);
        b.SetFloat("_OutlineSize", outlineSize);
        b.SetFloat("_OutlineAlpha", outlineAlpha);
    }

    void PushEffect_InnerOutline(MaterialPropertyBlock b)
    {
        b.SetFloat("_InnerOutline_En", innerOutlineEnabled ? 1f : 0f);
        b.SetFloat("_InnerOutlineMaskCh", (float)innerOutlineMaskCh);
        b.SetColor("_InnerOutlineColor", innerOutlineColor);
        b.SetFloat("_InnerOutlineThickness", innerOutlineThickness);
        b.SetFloat("_InnerOutlineAlpha", innerOutlineAlpha);
    }

    void PushEffect_TexOutline(MaterialPropertyBlock b)
    {
        b.SetFloat("_TexOutline_En", texOutlineEnabled ? 1f : 0f);
        b.SetFloat("_TexOutlineMaskCh", (float)texOutlineMaskCh);
        if (texOutlineTexture) b.SetTexture("_TexOutlineTex", texOutlineTexture);
        b.SetColor("_TexOutlineColor", texOutlineColor);
        b.SetFloat("_TexOutlineSize", texOutlineSize);
        b.SetVector("_TexOutlineTiling", texOutlineTiling);
        b.SetVector("_TexOutlineSpeed", texOutlineSpeed);
    }

    void PushEffect_RainbowOutline(MaterialPropertyBlock b)
    {
        b.SetFloat("_RainbowOutline_En", rainbowOutlineEnabled ? 1f : 0f);
        b.SetFloat("_RainbowOutlineMaskCh", (float)rainbowOutlineMaskCh);
        b.SetColor("_RainbowOutlineColor", rainbowOutlineColor);
        b.SetFloat("_RainbowOutlineSize", rainbowOutlineSize);
        b.SetFloat("_RainbowOutlineSpeed", rainbowOutlineSpeed);
        b.SetFloat("_RainbowOutlineDensity", rainbowOutlineDensity);
    }

    void PushEffect_DistortOutline(MaterialPropertyBlock b)
    {
        b.SetFloat("_DistortOutline_En", distortOutlineEnabled ? 1f : 0f);
        b.SetFloat("_DistortOutlineMaskCh", (float)distortOutlineMaskCh);
        b.SetColor("_DistortOutlineColor", distortOutlineColor);
        b.SetFloat("_DistortOutlineSize", distortOutlineSize);
        b.SetFloat("_DistortOutlineAmount", distortOutlineAmount);
        b.SetFloat("_DistortOutlineSpeed", distortOutlineSpeed);
    }

    void PushEffect_Glow(MaterialPropertyBlock b)
    {
        b.SetFloat("_Glow_En", glowEnabled ? 1f : 0f);
        b.SetFloat("_GlowMaskCh", (float)glowMaskCh);
        b.SetColor("_GlowColor", glowColor);
        b.SetFloat("_GlowSize", glowSize);
        b.SetFloat("_GlowIntensity", glowIntensity);
    }

    void PushEffect_Shadow(MaterialPropertyBlock b)
    {
        b.SetFloat("_Shadow_En", shadowEnabled ? 1f : 0f);
        b.SetFloat("_ShadowMaskCh", (float)shadowMaskCh);
        b.SetColor("_ShadowColor", shadowColor);
        b.SetVector("_ShadowOffset", shadowOffset);
    }

    void PushEffect_SoftShadow(MaterialPropertyBlock b)
    {
        b.SetFloat("_SoftShadow_En", softShadowEnabled ? 1f : 0f);
        b.SetFloat("_SoftShadowMaskCh", (float)softShadowMaskCh);
        b.SetColor("_SoftShadowColor", softShadowColor);
        b.SetVector("_SoftShadowOffset", softShadowOffset);
        b.SetFloat("_SoftShadowSoftness", softShadowSoftness);
        b.SetFloat("_SoftShadowSamples", (float)softShadowSamples);
    }

    void PushEffect_DirOutline(MaterialPropertyBlock b)
    {
        b.SetFloat("_DirOutline_En", dirOutlineEnabled ? 1f : 0f);
        b.SetFloat("_DirOutlineMaskCh", (float)dirOutlineMaskCh);
        b.SetColor("_DirOutlineColor", dirOutlineColor);
        b.SetFloat("_DirOutlineWidth", dirOutlineWidth);
        b.SetFloat("_DirOutlineInt", dirOutlineIntensity);
        b.SetVector("_DirOutlineDir", new Vector4(dirOutlineDirection.x, dirOutlineDirection.y, 0, 0));
        b.SetFloat("_DirOutlineSoft", dirOutlineSoftness);
    }

    void PushEffect_OutlineOnly(MaterialPropertyBlock b)
    {
        b.SetFloat("_OutlineOnly_En", outlineOnlyEnabled ? 1f : 0f);
        b.SetFloat("_OutlineOnlyMaskCh", (float)outlineOnlyMaskCh);
        b.SetColor("_OutlineOnlyColor", outlineOnlyColor);
        b.SetFloat("_OutlineOnlyWidth", outlineOnlyWidth);
    }

    void PushEffect_GradientOutline(MaterialPropertyBlock b)
    {
        b.SetFloat("_GradOutline_En", gradOutlineEnabled ? 1f : 0f);
        b.SetFloat("_GradOutlineMaskCh", (float)gradOutlineMaskCh);
        b.SetColor("_GradOutlineColorA", gradOutlineColorA);
        b.SetColor("_GradOutlineColorB", gradOutlineColorB);
        b.SetFloat("_GradOutlineAngle", gradOutlineAngle);
        b.SetFloat("_GradOutlineSize", gradOutlineSize);
    }

    void PushEffect_AnimOutlineThick(MaterialPropertyBlock b)
    {
        b.SetFloat("_AnimOutline_En", animOutlineEnabled ? 1f : 0f);
        b.SetFloat("_AnimOutlineMaskCh", (float)animOutlineMaskCh);
        b.SetColor("_AnimOutlineColor", animOutlineColor);
        b.SetFloat("_AnimOutlineMin", animOutlineMin);
        b.SetFloat("_AnimOutlineMax", animOutlineMax);
        b.SetFloat("_AnimOutlineSpeed", animOutlineSpeed);
    }

    void PushEffect_SelectionHighlight(MaterialPropertyBlock b)
    {
        b.SetFloat("_SelectHighlight_En", selectHighlightEnabled ? 1f : 0f);
        b.SetFloat("_SelectHighlightMaskCh", (float)selectHighlightMaskCh);
        b.SetColor("_SelectHighlightColor", selectHighlightColor);
        b.SetFloat("_SelectHighlightSize", selectHighlightSize);
        b.SetFloat("_SelectHighlightDash", selectHighlightDash);
        b.SetFloat("_SelectHighlightSpeed", selectHighlightSpeed);
    }

    void PushEffect_Pixelate(MaterialPropertyBlock b)
    {
        b.SetFloat("_Pixelate_En", pixelateEnabled ? 1f : 0f);
        b.SetFloat("_PixelateMaskCh", (float)pixelateMaskCh);
        b.SetFloat("_PixelateSize", pixelateSize);
    }

    void PushEffect_Wave(MaterialPropertyBlock b)
    {
        b.SetFloat("_Wave_En", waveEnabled ? 1f : 0f);
        b.SetFloat("_WaveMaskCh", (float)waveMaskCh);
        b.SetFloat("_WaveSpeed", waveSpeed);
        b.SetFloat("_WaveAmplitude", waveAmplitude);
        b.SetFloat("_WaveFrequency", waveFrequency);
    }

    void PushEffect_Twist(MaterialPropertyBlock b)
    {
        b.SetFloat("_Twist_En", twistEnabled ? 1f : 0f);
        b.SetFloat("_TwistMaskCh", (float)twistMaskCh);
        b.SetFloat("_TwistAmount", twistAmount);
        b.SetFloat("_TwistSpeed", twistSpeed);
        b.SetVector("_TwistOffset", new Vector4(twistOffset.x, twistOffset.y, 0, 0));
        b.SetFloat("_TwistRadius", twistRadius);
    }

    void PushEffect_UVDistort(MaterialPropertyBlock b)
    {
        b.SetFloat("_UVDistort_En", uvDistortEnabled ? 1f : 0f);
        b.SetFloat("_UVDistortMaskCh", (float)uvDistortMaskCh);
        if (distortTexture) b.SetTexture("_DistortTex", distortTexture);
        b.SetFloat("_DistortAmount", distortAmount);
        b.SetVector("_DistortTiling", distortTiling);
        b.SetFloat("_DistortSpeed", distortSpeed);
        b.SetFloat("_DistortIsNormalMap", (float)distortIsNormalMap);
    }

    void PushEffect_FullDistort(MaterialPropertyBlock b)
    {
        b.SetFloat("_FullDistort_En", fullDistortEnabled ? 1f : 0f);
        b.SetFloat("_FullDistortMaskCh", (float)fullDistortMaskCh);
        b.SetFloat("_FullDistortAmount", fullDistortAmount);
        b.SetFloat("_FullDistortSpeed", fullDistortSpeed);
        b.SetFloat("_FullDistortDensity", fullDistortDensity);
    }

    void PushEffect_DirDistort(MaterialPropertyBlock b)
    {
        b.SetFloat("_DirDistort_En", dirDistortEnabled ? 1f : 0f);
        b.SetFloat("_DirDistortMaskCh", (float)dirDistortMaskCh);
        b.SetFloat("_DirDistortAngle", dirDistortAngle);
        b.SetFloat("_DirDistortAmount", dirDistortAmount);
        b.SetFloat("_DirDistortSpeed", dirDistortSpeed);
    }

    void PushEffect_UVScroll(MaterialPropertyBlock b)
    {
        b.SetFloat("_UVScroll_En", uvScrollEnabled ? 1f : 0f);
        b.SetFloat("_UVScrollMaskCh", (float)uvScrollMaskCh);
        b.SetFloat("_UVScrollSpeedX", uvScrollSpeedX);
        b.SetFloat("_UVScrollSpeedY", uvScrollSpeedY);
    }

    void PushEffect_Rotate(MaterialPropertyBlock b)
    {
        b.SetFloat("_Rotate_En", rotateEnabled ? 1f : 0f);
        b.SetFloat("_RotateMaskCh", (float)rotateMaskCh);
        b.SetFloat("_RotateSpeed", rotateSpeed);
        b.SetFloat("_RotateAmount", rotateAmount);
    }

    void PushEffect_Squeeze(MaterialPropertyBlock b)
    {
        b.SetFloat("_Squeeze_En", squeezeEnabled ? 1f : 0f);
        b.SetFloat("_SqueezeMaskCh", (float)squeezeMaskCh);
        b.SetVector("_SqueezeAmount", squeezeAmount);
        b.SetFloat("_SqueezeSpeed", squeezeSpeed);
        b.SetFloat("_SqueezePower", squeezePower);
    }

    void PushEffect_Squish(MaterialPropertyBlock b)
    {
        b.SetFloat("_Squish_En", squishEnabled ? 1f : 0f);
        b.SetFloat("_SquishMaskCh", (float)squishMaskCh);
        b.SetVector("_SquishAmount", squishAmount);
        b.SetFloat("_SquishSpeed", squishSpeed);
        b.SetFloat("_SquishPower", squishPower);
    }

    void PushEffect_Wiggle(MaterialPropertyBlock b)
    {
        b.SetFloat("_Wiggle_En", wiggleEnabled ? 1f : 0f);
        b.SetFloat("_WiggleMaskCh", (float)wiggleMaskCh);
        b.SetFloat("_WiggleSpeed", wiggleSpeed);
        b.SetFloat("_WiggleAmount", wiggleAmount);
        b.SetFloat("_WiggleFrequency", wiggleFrequency);
    }

    void PushEffect_Wind(MaterialPropertyBlock b)
    {
        b.SetFloat("_Wind_En", windEnabled ? 1f : 0f);
        b.SetFloat("_WindMaskCh", (float)windMaskCh);
        b.SetFloat("_WindSpeed", windSpeed);
        b.SetFloat("_WindAmount", windAmount);
        b.SetFloat("_WindBottomY", windBottomY);
        b.SetFloat("_WindFrequency", windFrequency);
    }

    void PushEffect_Sharpen(MaterialPropertyBlock b)
    {
        b.SetFloat("_Sharpen_En", sharpenEnabled ? 1f : 0f);
        b.SetFloat("_SharpenMaskCh", (float)sharpenMaskCh);
        b.SetFloat("_SharpenAmount", sharpenAmount);
        b.SetFloat("_SharpenSize", sharpenSize);
    }

    void PushEffect_GaussBlur(MaterialPropertyBlock b)
    {
        b.SetFloat("_GaussBlur_En", gaussBlurEnabled ? 1f : 0f);
        b.SetFloat("_GaussBlurMaskCh", (float)gaussBlurMaskCh);
        b.SetFloat("_GaussBlurSize", gaussBlurSize);
        b.SetFloat("_GaussBlurSamples", gaussBlurSamples);
    }

    void PushEffect_DirBlur(MaterialPropertyBlock b)
    {
        b.SetFloat("_DirBlur_En", dirBlurEnabled ? 1f : 0f);
        b.SetFloat("_DirBlurMaskCh", (float)dirBlurMaskCh);
        b.SetFloat("_DirBlurAngle", dirBlurAngle);
        b.SetFloat("_DirBlurSize", dirBlurSize);
        b.SetFloat("_DirBlurSamples", dirBlurSamples);
    }

    void PushEffect_WorldTiling(MaterialPropertyBlock b)
    {
        b.SetFloat("_WorldTiling_En", worldTilingEnabled ? 1f : 0f);
        b.SetFloat("_WorldTilingMaskCh", (float)worldTilingMaskCh);
        b.SetFloat("_WorldTileScale", worldTileScale);
        b.SetVector("_WorldTileOffset", new Vector4(worldTileOffset.x, worldTileOffset.y, 0, 0));
        b.SetFloat("_WorldTileRotation", worldTileRotation);
    }

    void PushEffect_UVScaleOffset(MaterialPropertyBlock b)
    {
        b.SetFloat("_UVScaleOffset_En", uvScaleOffsetEnabled ? 1f : 0f);
        b.SetFloat("_UVScaleOffsetMaskCh", (float)uvScaleOffsetMaskCh);
        b.SetVector("_UVScaleOffset", uvScaleOffset);
    }

    void PushEffect_CurlNoise(MaterialPropertyBlock b)
    {
        b.SetFloat("_CurlNoise_En", curlNoiseEnabled ? 1f : 0f);
        b.SetFloat("_CurlNoiseMaskCh", (float)curlNoiseMaskCh);
        b.SetFloat("_CurlNoiseAmount", curlNoiseAmount);
        b.SetFloat("_CurlNoiseSpeed", curlNoiseSpeed);
        b.SetFloat("_CurlNoiseDensity", curlNoiseDensity);
    }

    void PushEffect_FBMNoise(MaterialPropertyBlock b)
    {
        b.SetFloat("_FBMNoise_En", fbmNoiseEnabled ? 1f : 0f);
        b.SetFloat("_FBMNoiseMaskCh", (float)fbmNoiseMaskCh);
        b.SetFloat("_FBMNoiseAmount", fbmNoiseAmount);
        b.SetFloat("_FBMNoiseSpeed", fbmNoiseSpeed);
        b.SetFloat("_FBMNoiseDensity", fbmNoiseDensity);
        b.SetFloat("_FBMNoiseOctaves", (float)fbmNoiseOctaves);
    }

    void PushEffect_UVScrollOverlay(MaterialPropertyBlock b)
    {
        b.SetFloat("_UVScrollOverlay_En", uvScrollOverlayEnabled ? 1f : 0f);
        b.SetFloat("_UVScrollOverlayMaskCh", (float)uvScrollOverlayMaskCh);
        if (uvScrollOverlayTex) b.SetTexture("_UVScrollOverlayTex", uvScrollOverlayTex);
        b.SetFloat("_UVScrollOverlaySpeedX", uvScrollOverlaySpeedX);
        b.SetFloat("_UVScrollOverlaySpeedY", uvScrollOverlaySpeedY);
        b.SetFloat("_UVScrollOverlayBlend", uvScrollOverlayBlend);
        b.SetColor("_UVScrollOverlayTint", uvScrollOverlayTint);
        b.SetVector("_UVScrollOverlayTiling", uvScrollOverlayTiling);
    }

    void PushEffect_Shockwave(MaterialPropertyBlock b)
    {
        b.SetFloat("_Shockwave_En", shockwaveEnabled ? 1f : 0f);
        b.SetFloat("_ShockwaveMaskCh", (float)shockwaveMaskCh);
        b.SetVector("_ShockwaveCenter", new Vector4(shockwaveCenter.x, shockwaveCenter.y, 0, 0));
        b.SetFloat("_ShockwaveRadius", shockwaveRadius);
        b.SetFloat("_ShockwaveWidth", shockwaveWidth);
        b.SetFloat("_ShockwaveAmplitude", shockwaveAmplitude);
    }

    void PushEffect_SineScale(MaterialPropertyBlock b)
    {
        b.SetFloat("_SineScale_En", sineScaleEnabled ? 1f : 0f);
        b.SetFloat("_SineScaleMaskCh", (float)sineScaleMaskCh);
        b.SetFloat("_SineScaleAmountX", sineScaleAmountX);
        b.SetFloat("_SineScaleAmountY", sineScaleAmountY);
        b.SetFloat("_SineScaleSpeed", sineScaleSpeed);
    }

    void PushEffect_Heartbeat(MaterialPropertyBlock b)
    {
        b.SetFloat("_Heartbeat_En", heartbeatEnabled ? 1f : 0f);
        b.SetFloat("_HeartbeatMaskCh", (float)heartbeatMaskCh);
        b.SetFloat("_HeartbeatSpeed", heartbeatSpeed);
        b.SetFloat("_HeartbeatAmount", heartbeatAmount);
    }

    void PushEffect_Breathing(MaterialPropertyBlock b)
    {
        b.SetFloat("_Breathing_En", breathingEnabled ? 1f : 0f);
        b.SetFloat("_BreathingMaskCh", (float)breathingMaskCh);
        b.SetFloat("_BreathingSpeed", breathingSpeed);
        b.SetFloat("_BreathingAmount", breathingAmount);
    }

    void PushEffect_LiquidWobble(MaterialPropertyBlock b)
    {
        b.SetFloat("_LiquidWobble_En", liquidWobbleEnabled ? 1f : 0f);
        b.SetFloat("_LiquidWobbleMaskCh", (float)liquidWobbleMaskCh);
        b.SetFloat("_LiquidWobbleSpeed", liquidWobbleSpeed);
        b.SetFloat("_LiquidWobbleAmount", liquidWobbleAmount);
        b.SetFloat("_LiquidWobbleFreq", liquidWobbleFreq);
    }

    void PushEffect_FlipBookBlend(MaterialPropertyBlock b)
    {
        b.SetFloat("_FlipBookBlend_En", flipBookBlendEnabled ? 1f : 0f);
        b.SetFloat("_FlipBookBlendMaskCh", (float)flipBookBlendMaskCh);
        if (flipBookNextFrame) b.SetTexture("_FlipBookNextFrame", flipBookNextFrame);
        b.SetFloat("_FlipBookLerp", flipBookLerp);
    }

    void PushEffect_SmoothPixelArt(MaterialPropertyBlock b)
    {
        b.SetFloat("_SmoothPixelArt_En", smoothPixelArtEnabled ? 1f : 0f);
        b.SetFloat("_SmoothPixelArtMaskCh", (float)smoothPixelArtMaskCh);
        b.SetFloat("_SmoothPixelArtTexelSize", smoothPixelArtTexelSize);
    }

    void PushEffect_Dissolve(MaterialPropertyBlock b)
    {
        b.SetFloat("_Dissolve_En", dissolveEnabled ? 1f : 0f);
        b.SetFloat("_DissolveMaskCh", (float)dissolveMaskCh);
        b.SetFloat("_DissolveNoiseType", (float)dissolveNoiseType);
        if (dissolveTexture) b.SetTexture("_DissolveTex", dissolveTexture);
        b.SetFloat("_DissolveNoiseScale", dissolveNoiseScale);
        b.SetFloat("_DissolveAmount", dissolveAmount);
        b.SetFloat("_DissolveBorderWidth", dissolveBorderWidth);
        b.SetFloat("_DissolveBurnWidth", dissolveBurnWidth);
        b.SetColor("_DissolveBurnColor", dissolveBurnColor);
        b.SetVector("_DissolveTiling", dissolveTiling);
        b.SetColor("_DissolveBorderColor", dissolveBorderColor);
    }

    void PushEffect_GlowDissolve(MaterialPropertyBlock b)
    {
        b.SetFloat("_GlowDissolve_En", glowDissolveEnabled ? 1f : 0f);
        b.SetFloat("_GlowDissolveMaskCh", (float)glowDissolveMaskCh);
        b.SetFloat("_GlowDissolveNoiseType", (float)glowDissolveNoiseType);
        if (glowDissolveTexture) b.SetTexture("_GlowDissolveTex", glowDissolveTexture);
        b.SetFloat("_GlowDissolveNoiseScale", glowDissolveNoiseScale);
        b.SetFloat("_GlowDissolveAmount", glowDissolveAmount);
        b.SetColor("_GlowDissolveColor", glowDissolveColor);
        b.SetFloat("_GlowDissolveWidth", glowDissolveWidth);
    }

    void PushEffect_SrcDissolve(MaterialPropertyBlock b)
    {
        b.SetFloat("_SrcDissolve_En", srcDissolveEnabled ? 1f : 0f);
        b.SetFloat("_SrcDissolveMaskCh", (float)srcDissolveMaskCh);
        b.SetFloat("_SrcDissolveAmount", srcDissolveAmount);
        b.SetVector("_SrcDissolvePosition", new Vector4(srcDissolvePosition.x, srcDissolvePosition.y, 0, 0));
        b.SetColor("_SrcDissolveBorderColor", srcDissolveBorderColor);
        b.SetFloat("_SrcDissolveBorderWidth", srcDissolveBorderWidth);
    }

    void PushEffect_DirFade(MaterialPropertyBlock b)
    {
        b.SetFloat("_DirFade_En", dirFadeEnabled ? 1f : 0f);
        b.SetFloat("_DirFadeMaskCh", (float)dirFadeMaskCh);
        b.SetFloat("_DirFadeAngle", dirFadeAngle);
        b.SetFloat("_DirFadeAmount", dirFadeAmount);
        b.SetFloat("_DirFadeSmoothness", dirFadeSmoothness);
    }

    void PushEffect_DirGlowFade(MaterialPropertyBlock b)
    {
        b.SetFloat("_DirGlowFade_En", dirGlowFadeEnabled ? 1f : 0f);
        b.SetFloat("_DirGlowFadeMaskCh", (float)dirGlowFadeMaskCh);
        b.SetFloat("_DirGlowFadeAngle", dirGlowFadeAngle);
        b.SetFloat("_DirGlowFadeAmount", dirGlowFadeAmount);
        b.SetColor("_DirGlowFadeColor", dirGlowFadeColor);
        b.SetFloat("_DirGlowFadeWidth", dirGlowFadeWidth);
    }

    void PushEffect_FullAlphaDissolve(MaterialPropertyBlock b)
    {
        b.SetFloat("_FullAlphaDissolve_En", fullAlphaDissolveEnabled ? 1f : 0f);
        b.SetFloat("_FullAlphaDissolveMaskCh", (float)fullAlphaDissolveMaskCh);
        b.SetFloat("_FullAlphaDissolveAmount", fullAlphaDissolveAmount);
    }

    void PushEffect_Halftone(MaterialPropertyBlock b)
    {
        b.SetFloat("_Halftone_En", halftoneEnabled ? 1f : 0f);
        b.SetFloat("_HalftoneMaskCh", (float)halftoneMaskCh);
        b.SetFloat("_HalftoneSize", halftoneSize);
        b.SetFloat("_HalftoneAmount", halftoneAmount);
        b.SetFloat("_HalftoneSmoothness", halftoneSmoothness);
    }

    void PushEffect_RadialWipe(MaterialPropertyBlock b)
    {
        b.SetFloat("_RadialWipe_En", radialWipeEnabled ? 1f : 0f);
        b.SetFloat("_RadialWipeMaskCh", (float)radialWipeMaskCh);
        b.SetFloat("_RadialWipeAmount", radialWipeAmount);
        b.SetFloat("_RadialWipeSmoothness", radialWipeSmoothness);
        b.SetFloat("_RadialWipeAngle", radialWipeAngle);
    }

    void PushEffect_Blinds(MaterialPropertyBlock b)
    {
        b.SetFloat("_Blinds_En", blindsEnabled ? 1f : 0f);
        b.SetFloat("_BlindsMaskCh", (float)blindsMaskCh);
        b.SetFloat("_BlindsAmount", blindsAmount);
        b.SetFloat("_BlindsCount", blindsCount);
        b.SetFloat("_BlindsDirection", (float)blindsDirection);
    }

    void PushEffect_DiamondDissolve(MaterialPropertyBlock b)
    {
        b.SetFloat("_DiamondDissolve_En", diamondDissolveEnabled ? 1f : 0f);
        b.SetFloat("_DiamondDissolveMaskCh", (float)diamondDissolveMaskCh);
        b.SetFloat("_DiamondDissolveAmount", diamondDissolveAmount);
        b.SetFloat("_DiamondDissolveScale", diamondDissolveScale);
    }

    void PushEffect_ScanlineDissolve(MaterialPropertyBlock b)
    {
        b.SetFloat("_ScanlineDissolve_En", scanlineDissolveEnabled ? 1f : 0f);
        b.SetFloat("_ScanlineDissolveMaskCh", (float)scanlineDissolveMaskCh);
        b.SetFloat("_ScanlineDissolveAmount", scanlineDissolveAmount);
        b.SetFloat("_ScanlineDissolveDensity", scanlineDissolveDensity);
    }

    void PushEffect_SpreadFade(MaterialPropertyBlock b)
    {
        b.SetFloat("_SpreadFade_En", spreadFadeEnabled ? 1f : 0f);
        b.SetFloat("_SpreadFadeMaskCh", (float)spreadFadeMaskCh);
        b.SetFloat("_SpreadFadeAmount", spreadFadeAmount);
        b.SetVector("_SpreadFadeOrigin", spreadFadeOrigin);
        b.SetFloat("_SpreadFadeSmoothness", spreadFadeSmoothness);
    }

    void PushEffect_PixelDissolve(MaterialPropertyBlock b)
    {
        b.SetFloat("_PixelDissolve_En", pixelDissolveEnabled ? 1f : 0f);
        b.SetFloat("_PixelDissolveMaskCh", (float)pixelDissolveMaskCh);
        b.SetFloat("_PixelDissolveAmount", pixelDissolveAmount);
        b.SetFloat("_PixelDissolveGrid", pixelDissolveGrid);
    }

    void PushEffect_SpiralWipe(MaterialPropertyBlock b)
    {
        b.SetFloat("_SpiralWipe_En", spiralWipeEnabled ? 1f : 0f);
        b.SetFloat("_SpiralWipeMaskCh", (float)spiralWipeMaskCh);
        b.SetFloat("_SpiralWipeAmount", spiralWipeAmount);
        b.SetFloat("_SpiralWipeSmoothness", spiralWipeSmoothness);
        b.SetFloat("_SpiralWipeArms", spiralWipeArms);
    }

    void PushEffect_HexDissolve(MaterialPropertyBlock b)
    {
        b.SetFloat("_HexDissolve_En", hexDissolveEnabled ? 1f : 0f);
        b.SetFloat("_HexDissolveMaskCh", (float)hexDissolveMaskCh);
        b.SetFloat("_HexDissolveAmount", hexDissolveAmount);
        b.SetFloat("_HexDissolveScale", hexDissolveScale);
    }

    void PushEffect_Hologram(MaterialPropertyBlock b)
    {
        b.SetFloat("_Hologram_En", hologramEnabled ? 1f : 0f);
        b.SetFloat("_HologramMaskCh", (float)hologramMaskCh);
        b.SetFloat("_HologramStripesAmount", hologramStripesAmount);
        b.SetFloat("_HologramMinAlpha", hologramMinAlpha);
        b.SetFloat("_HologramStripesSpeed", hologramStripesSpeed);
        b.SetColor("_HologramColor", hologramColor);
        b.SetFloat("_HologramBlend", hologramBlend);
        b.SetFloat("_HologramDistortion", hologramDistortion);
    }

    void PushEffect_Glitch(MaterialPropertyBlock b)
    {
        b.SetFloat("_Glitch_En", glitchEnabled ? 1f : 0f);
        b.SetFloat("_GlitchMaskCh", (float)glitchMaskCh);
        b.SetFloat("_GlitchAmount", glitchAmount);
        b.SetFloat("_GlitchSpeed", glitchSpeed);
        b.SetFloat("_GlitchSize", glitchSize);
        b.SetFloat("_GlitchColorShift", glitchColorShift);
    }

    void PushEffect_Flicker(MaterialPropertyBlock b)
    {
        b.SetFloat("_Flicker_En", flickerEnabled ? 1f : 0f);
        b.SetFloat("_FlickerMaskCh", (float)flickerMaskCh);
        b.SetFloat("_FlickerFreq", flickerFreq);
        b.SetFloat("_FlickerPercent", flickerPercent);
    }

    void PushEffect_Shine(MaterialPropertyBlock b)
    {
        b.SetFloat("_Shine_En", shineEnabled ? 1f : 0f);
        b.SetFloat("_ShineMaskCh", (float)shineMaskCh);
        b.SetColor("_ShineColor", shineColor);
        b.SetFloat("_ShineWidth", shineWidth);
        b.SetFloat("_ShineSpeed", shineSpeed);
        b.SetFloat("_ShineDelay", shineDelay);
        b.SetFloat("_ShineRotation", shineRotation);
        b.SetFloat("_ShineGlow", shineGlow);
    }

    void PushEffect_Smoke(MaterialPropertyBlock b)
    {
        b.SetFloat("_Smoke_En", smokeEnabled ? 1f : 0f);
        b.SetFloat("_SmokeMaskCh", (float)smokeMaskCh);
        b.SetColor("_SmokeColor", smokeColor);
        b.SetFloat("_SmokeDensity", smokeDensity);
        b.SetFloat("_SmokeSpeed", smokeSpeed);
        b.SetFloat("_SmokeTurbulence", smokeTurbulence);
        b.SetVector("_SmokeDirection", smokeDirection);
        b.SetFloat("_SmokeColorStr", smokeColorStrength);
        b.SetFloat("_SmokeFade", smokeFade);
    }

    void PushEffect_Flame(MaterialPropertyBlock b)
    {
        b.SetFloat("_Flame_En", flameEnabled ? 1f : 0f);
        b.SetFloat("_FlameMaskCh", (float)flameMaskCh);
        b.SetColor("_FlameColor1", flameColor1);
        b.SetColor("_FlameColor2", flameColor2);
        b.SetFloat("_FlameSpeed", flameSpeed);
        b.SetFloat("_FlameDensity", flameDensity);
        b.SetFloat("_FlameOnlyMode", (float)flameOnlyMode);
        b.SetFloat("_FlameExtendSides", flameExtendSides);
        b.SetFloat("_FlameSideFalloff", flameSideFalloff);
        b.SetFloat("_FlameIntensity", flameIntensity);
    }

    void PushEffect_Enchanted(MaterialPropertyBlock b)
    {
        b.SetFloat("_Enchanted_En", enchantedEnabled ? 1f : 0f);
        b.SetFloat("_EnchantedMaskCh", (float)enchantedMaskCh);
        b.SetColor("_EnchantedColor", enchantedColor);
        b.SetFloat("_EnchantedSpeed", enchantedSpeed);
        b.SetFloat("_EnchantedDensity", enchantedDensity);
        b.SetFloat("_EnchantedBrightness", enchantedBrightness);
        b.SetFloat("_EnchantedFade", enchantedFade);
    }

    void PushEffect_Shifting(MaterialPropertyBlock b)
    {
        b.SetFloat("_Shifting_En", shiftingEnabled ? 1f : 0f);
        b.SetFloat("_ShiftingMaskCh", (float)shiftingMaskCh);
        b.SetColor("_ShiftingColor1", shiftingColor1);
        b.SetColor("_ShiftingColor2", shiftingColor2);
        b.SetFloat("_ShiftingSpeed", shiftingSpeed);
        b.SetFloat("_ShiftingFade", shiftingFade);
    }

    void PushEffect_ShiftRainbow(MaterialPropertyBlock b)
    {
        b.SetFloat("_ShiftRainbow_En", shiftRainbowEnabled ? 1f : 0f);
        b.SetFloat("_ShiftRainbowMaskCh", (float)shiftRainbowMaskCh);
        b.SetFloat("_ShiftRainbowSpeed", shiftRainbowSpeed);
        b.SetFloat("_ShiftRainbowDensity", shiftRainbowDensity);
        b.SetFloat("_ShiftRainbowFade", shiftRainbowFade);
    }

    void PushEffect_Overlay(MaterialPropertyBlock b)
    {
        b.SetFloat("_Overlay_En", overlayEnabled ? 1f : 0f);
        b.SetFloat("_OverlayMaskCh", (float)overlayMaskCh);
        if (overlayTexture) b.SetTexture("_OverlayTex", overlayTexture);
        b.SetColor("_OverlayColor", overlayColor);
        b.SetFloat("_OverlayBlend", overlayBlend);
        b.SetVector("_OverlayTiling", overlayTiling);
        b.SetVector("_OverlaySpeed", overlaySpeed);
    }

    void PushEffect_Checkerboard(MaterialPropertyBlock b)
    {
        b.SetFloat("_Checkerboard_En", checkerboardEnabled ? 1f : 0f);
        b.SetFloat("_CheckerboardMaskCh", (float)checkerboardMaskCh);
        b.SetFloat("_CheckerCoordSpace", (float)checkerCoordSpace);
        b.SetFloat("_CheckerScale", checkerScale);
        b.SetColor("_CheckerColor1", checkerColor1);
        b.SetColor("_CheckerColor2", checkerColor2);
    }

    void PushEffect_UVVisualize(MaterialPropertyBlock b)
    {
        b.SetFloat("_UVVisualize_En", uvVisualizeEnabled ? 1f : 0f);
        b.SetFloat("_UVVisualizeMaskCh", (float)uvVisualizeMaskCh);
        b.SetFloat("_UVVisualizeBlend", uvVisualizeBlend);
    }

    void PushEffect_HealthTint(MaterialPropertyBlock b)
    {
        b.SetFloat("_HealthTint_En", healthTintEnabled ? 1f : 0f);
        b.SetFloat("_HealthTintMaskCh", (float)healthTintMaskCh);
        b.SetColor("_HealthTintLowColor", healthTintLowColor);
        b.SetColor("_HealthTintHighColor", healthTintHighColor);
        b.SetFloat("_HealthTintAmount", healthTintAmount);
        b.SetFloat("_HealthDamageNoise", healthDamageNoise);
        b.SetFloat("_HealthEdgeGrain", healthEdgeGrain);
        b.SetFloat("_HealthPulseSpeed", healthPulseSpeed);
        b.SetFloat("_HealthVignetteStr", healthVignetteStr);
        b.SetFloat("_HealthDesaturation", healthDesaturation);
    }

    void PushEffect_XRaySilhouette(MaterialPropertyBlock b)
    {
        b.SetFloat("_XRaySilhouette_En", xraySilhouetteEnabled ? 1f : 0f);
        b.SetFloat("_XRaySilhouetteMaskCh", (float)xraySilhouetteMaskCh);
        b.SetColor("_XRaySilhouetteColor", xraySilhouetteColor);
        b.SetFloat("_XRaySilhouetteBlend", xraySilhouetteBlend);
    }

    void PushEffect_MinimapPing(MaterialPropertyBlock b)
    {
        b.SetFloat("_MinimapPing_En", minimapPingEnabled ? 1f : 0f);
        b.SetFloat("_MinimapPingMaskCh", (float)minimapPingMaskCh);
        b.SetColor("_MinimapPingColor", minimapPingColor);
        b.SetFloat("_MinimapPingSpeed", minimapPingSpeed);
        b.SetFloat("_MinimapPingWidth", minimapPingWidth);
        b.SetVector("_MinimapPingOffset", new Vector4(minimapPingOffset.x, minimapPingOffset.y, 0, 0));
    }

    void PushEffect_DialogueEmphasis(MaterialPropertyBlock b)
    {
        b.SetFloat("_DialogueEmphasis_En", dialogueEmphasisEnabled ? 1f : 0f);
        b.SetFloat("_DialogueEmphasisMaskCh", (float)dialogueEmphasisMaskCh);
        b.SetFloat("_DialogueBobSpeed", dialogueBobSpeed);
        b.SetFloat("_DialogueBobAmount", dialogueBobAmount);
        b.SetFloat("_DialogueBrightness", dialogueBrightness);
    }

    void PushEffect_VoronoiCrackle(MaterialPropertyBlock b)
    {
        b.SetFloat("_VoronoiCrackle_En", voronoiCrackleEnabled ? 1f : 0f);
        b.SetFloat("_CrackleMaskCh", (float)crackleMaskCh);
        b.SetFloat("_CrackleScale", crackleScale);
        b.SetFloat("_CrackleSpeed", crackleSpeed);
        b.SetFloat("_CrackleEdgeWidth", crackleEdgeWidth);
        b.SetColor("_CrackleEdgeColor", crackleEdgeColor);
        b.SetFloat("_CrackleCellTint", crackleCellTint);
        b.SetFloat("_CrackleFade", crackleFade);
    }

    void PushEffect_WaterReflect(MaterialPropertyBlock b)
    {
        b.SetFloat("_WaterReflect_En", waterReflectEnabled ? 1f : 0f);
        b.SetFloat("_WaterReflectMaskCh", (float)waterReflectMaskCh);
        b.SetColor("_WaterReflectTint", waterReflectTint);
        b.SetFloat("_WaterReflectYPoint", waterReflectYPoint);
        b.SetFloat("_WaterReflectDepth", waterReflectDepth);
        b.SetFloat("_WaterReflectOpacity", waterReflectOpacity);
        b.SetFloat("_WaterReflectTintAmount", waterReflectTintAmount);
        b.SetFloat("_WaterReflectWaveFreq", waterReflectWaveFreq);
        b.SetFloat("_WaterReflectWaveSpeed", waterReflectWaveSpeed);
        b.SetFloat("_WaterReflectWaveAmp", waterReflectWaveAmp);
        b.SetFloat("_WaterReflectFade", waterReflectFade);
    }

    void PushEffect_MotionSmear(MaterialPropertyBlock b)
    {
        b.SetFloat("_MotionSmear_En", motionSmearEnabled ? 1f : 0f);
        b.SetFloat("_SmearMaskCh", (float)smearMaskCh);
        b.SetVector("_SmearDirection", smearDirection);
        b.SetFloat("_SmearAmount", smearAmount);
        b.SetFloat("_SmearSamples", smearSamples);
        b.SetFloat("_SmearFade", smearFade);
    }

    void PushEffect_EnergyAura(MaterialPropertyBlock b)
    {
        b.SetFloat("_EnergyAura_En", energyAuraEnabled ? 1f : 0f);
        b.SetFloat("_AuraMaskCh", (float)auraMaskCh);
        b.SetColor("_AuraColor", auraColor);
        b.SetFloat("_AuraRadius", auraRadius);
        b.SetFloat("_AuraFrequency", auraFrequency);
        b.SetFloat("_AuraSpeed", auraSpeed);
        b.SetFloat("_AuraIntensity", auraIntensity);
        b.SetFloat("_AuraFade", auraFade);
    }

    void PushEffect_Gradient(MaterialPropertyBlock b)
    {
        b.SetFloat("_Gradient_En", gradientEnabled ? 1f : 0f);
        b.SetFloat("_GradientMaskCh", (float)gradientMaskCh);
        b.SetColor("_GradientColorA", gradientColorA);
        b.SetColor("_GradientColorB", gradientColorB);
        b.SetFloat("_GradientAngle", gradientAngle);
        b.SetFloat("_GradientFade", gradientFade);
        b.SetFloat("_GradientUseTex", gradientUseGradientEditor ? 1f : 0f);
        if (gradientBakedTexture) b.SetTexture("_GradientTex", gradientBakedTexture);
        b.SetFloat("_GradientBlendMode", (float)gradientBlendMode);
    }

    void PushEffect_RadialGradient(MaterialPropertyBlock b)
    {
        b.SetFloat("_RadialGradient_En", radialGradientEnabled ? 1f : 0f);
        b.SetFloat("_RadialGradMaskCh", (float)radialGradMaskCh);
        b.SetColor("_RadialGradCenterColor", radialGradCenterColor);
        b.SetColor("_RadialGradEdgeColor", radialGradEdgeColor);
        b.SetVector("_RadialGradCenter", new Vector4(radialGradCenter.x, radialGradCenter.y, 0, 0));
        b.SetFloat("_RadialGradRadius", radialGradRadius);
        b.SetFloat("_RadialGradFade", radialGradFade);
        b.SetFloat("_RadialGradUseTex", radialGradUseGradientEditor ? 1f : 0f);
        if (radialGradBakedTexture) b.SetTexture("_RadialGradTex", radialGradBakedTexture);
        b.SetFloat("_RadialGradBlendMode", (float)radialGradBlendMode);
    }

    void PushEffect_ColorRamp(MaterialPropertyBlock b)
    {
        b.SetFloat("_ColorRamp_En", colorRampEnabled ? 1f : 0f);
        b.SetFloat("_ColorRampMaskCh", (float)colorRampMaskCh);
        if (colorRampTexture) b.SetTexture("_ColorRampTex", colorRampTexture);
        b.SetFloat("_ColorRampMode", (float)colorRampMode);
        b.SetFloat("_ColorRampIntensity", colorRampIntensity);
        b.SetColor("_ColorRampColorA", colorRampColorA);
        b.SetColor("_ColorRampColorB", colorRampColorB);
        b.SetFloat("_ColorRampFade", colorRampFade);
    }

    void PushEffect_FakeLighting(MaterialPropertyBlock b)
    {
        b.SetFloat("_FakeLighting_En", fakeLightingEnabled ? 1f : 0f);
        b.SetFloat("_FakeLightMaskCh", (float)fakeLightMaskCh);
        b.SetVector("_FakeLightDir", fakeLightDirection);
        b.SetColor("_FakeLightColor", fakeLightColor);
        b.SetFloat("_FakeLightIntensity", fakeLightIntensity);
        b.SetFloat("_FakeLightAmbient", fakeLightAmbient);
        b.SetFloat("_FakeLightNDist", fakeLightNormalDist);
        b.SetFloat("_FakeLightNZ", fakeLightNormalZ);
        b.SetFloat("_FakeLightSpecPow", fakeLightSpecPower);
        b.SetFloat("_FakeLightSpecInt", fakeLightSpecIntensity);
        b.SetFloat("_FakeLightFade", fakeLightFade);
    }

    void PushEffect_UVBendPoint(MaterialPropertyBlock b)
    {
        b.SetFloat("_UVBendPoint_En", uvBendPointEnabled ? 1f : 0f);
        b.SetFloat("_BendMaskCh", (float)bendMaskCh);
        b.SetVector("_BendLocalPoint", bendLocalPoint);
        b.SetVector("_BendWorldPoint", bendWorldPoint);
        b.SetFloat("_BendUseWorldSpace", bendUseWorldSpace ? 1f : 0f);
        b.SetFloat("_BendWorldScale", bendWorldScale);
        b.SetFloat("_BendRadius", bendRadius);
        b.SetFloat("_BendAmount", bendAmount);
        b.SetFloat("_BendFalloff", bendFalloff);
        b.SetFloat("_BendInvert", bendInvert ? 1f : 0f);
    }

    void PushEffect_BurnDirection(MaterialPropertyBlock b)
    {
        b.SetFloat("_BurnDirection_En", burnDirectionEnabled ? 1f : 0f);
        b.SetFloat("_BurnDirMaskCh", (float)burnDirMaskCh);
        b.SetFloat("_BurnDirAngle", burnDirAngle);
        b.SetFloat("_BurnDirAmount", burnDirAmount);
        b.SetFloat("_BurnDirEdgeWidth", burnDirEdgeWidth);
        b.SetColor("_BurnDirFireColor", burnDirFireColor);
        b.SetColor("_BurnDirCharcoalColor", burnDirCharcoalColor);
        b.SetFloat("_BurnDirCharcoalWidth", burnDirCharcoalWidth);
        b.SetFloat("_BurnDirNoiseScale", burnDirNoiseScale);
    }

    void PushEffect_GlobalSmoke(MaterialPropertyBlock b)
    {
        b.SetFloat("_GlobalSmoke_En", globalSmokeEnabled ? 1f : 0f);
        b.SetFloat("_GlobalSmokeMaskCh", (float)globalSmokeMaskCh);
        b.SetColor("_GlobalSmokeColor", globalSmokeColor);
        b.SetFloat("_GlobalSmokeDensity", globalSmokeDensity);
        b.SetFloat("_GlobalSmokeSpeed", globalSmokeSpeed);
        b.SetFloat("_GlobalSmokeOpacity", globalSmokeOpacity);
        b.SetVector("_GlobalSmokeDir", globalSmokeDirection);
        b.SetFloat("_GlobalSmokeOverlayOnly", globalSmokeOverlayOnly ? 1f : 0f);
    }

    void PushEffect_HolographicCard(MaterialPropertyBlock b)
    {
        b.SetFloat("_HoloCard_En", holographicCardEnabled ? 1f : 0f);
        b.SetFloat("_HoloCardMaskCh", (float)holoCardMaskCh);
        b.SetFloat("_HoloCardDensity", holoCardDensity);
        b.SetFloat("_HoloCardSpeed", holoCardSpeed);
        b.SetFloat("_HoloCardSat", holoCardSaturation);
        b.SetFloat("_HoloCardBright", holoCardBrightness);
        b.SetFloat("_HoloCardFPow", holoCardFresnelPower);
        b.SetFloat("_HoloCardFShift", holoCardFresnelShift);
        b.SetFloat("_HoloCardScanDen", holoCardScanDensity);
        b.SetFloat("_HoloCardScanSpd", holoCardScanSpeed);
        b.SetFloat("_HoloCardScanInt", holoCardScanIntensity);
        b.SetFloat("_HoloCardFade", holoCardFade);
    }

    void PushEffect_CloudShadows(MaterialPropertyBlock b)
    {
        b.SetFloat("_CloudShadows_En", cloudShadowsEnabled ? 1f : 0f);
        b.SetFloat("_CloudShadowMaskCh", (float)cloudShadowMaskCh);
        b.SetFloat("_CloudShadowScale", cloudShadowScale);
        b.SetVector("_CloudShadowSpeed", cloudShadowSpeed);
        b.SetFloat("_CloudShadowDark", cloudShadowDarkness);
        b.SetFloat("_CloudShadowThresh", cloudShadowThreshold);
        b.SetFloat("_CloudShadowSoft", cloudShadowSoftness);
        b.SetFloat("_CloudShadowFade", cloudShadowFade);
        b.SetFloat("_CloudShadowOverlayOnly", cloudShadowOverlayOnly ? 1f : 0f);
    }

    void PushEffect_ColorfulTrailA(MaterialPropertyBlock b)
    {
        b.SetFloat("_TrailA_En", colorfulTrailAEnabled ? 1f : 0f);
        b.SetFloat("_TrailAMaskCh", (float)trailAMaskCh);
        b.SetVector("_TrailAVel", trailAVelocity);
        b.SetColor("_TrailASlowCol", trailASlowColor);
        b.SetColor("_TrailAFastCol", trailAFastColor);
        b.SetFloat("_TrailAMaxSpd", trailAMaxSpeed);
        b.SetFloat("_TrailACurve", trailACurvePower);
        b.SetFloat("_TrailAInt", trailAIntensity);
    }

    void PushEffect_ColorfulTrailB(MaterialPropertyBlock b)
    {
        b.SetFloat("_TrailB_En", colorfulTrailBEnabled ? 1f : 0f);
        b.SetFloat("_TrailBMaskCh", (float)trailBMaskCh);
        b.SetVector("_TrailBVel", trailBVelocity);
        b.SetColor("_TrailBCol", trailBColor);
        b.SetFloat("_TrailBMaxSpd", trailBMaxSpeed);
        b.SetFloat("_TrailBCurve", trailBCurvePower);
        b.SetFloat("_TrailBInt", trailBIntensity);
        b.SetFloat("_TrailBSegs", (float)trailBSegments);
        b.SetFloat("_TrailBFade", trailBFadeOut);
    }

    void PushEffect_ColorfulTrailC(MaterialPropertyBlock b)
    {
        b.SetFloat("_TrailC_En", colorfulTrailCEnabled ? 1f : 0f);
        b.SetFloat("_TrailCMaskCh", (float)trailCMaskCh);
        b.SetVector("_TrailCVel", trailCVelocity);
        b.SetColor("_TrailCColA", trailCColorA);
        b.SetColor("_TrailCColB", trailCColorB);
        b.SetFloat("_TrailCMaxSpd", trailCMaxSpeed);
        b.SetFloat("_TrailCCurve", trailCCurvePower);
        b.SetFloat("_TrailCInt", trailCIntensity);
        b.SetFloat("_TrailCRbSpd", trailCRainbowSpeed);
    }

    void PushEffect_ChargingAbility(MaterialPropertyBlock b)
    {
        b.SetFloat("_Charging_En", chargingAbilityEnabled ? 1f : 0f);
        b.SetFloat("_ChargeMaskCh", (float)chargeMaskCh);
        b.SetColor("_ChargeColor", chargeColor);
        b.SetVector("_ChargeCenter", chargeCenter);
        b.SetFloat("_ChargeRadius", chargeRadius);
        b.SetFloat("_ChargeWidth", chargeWidth);
        b.SetFloat("_ChargeSpeed", chargeSpeed);
        b.SetFloat("_ChargeFreq", chargeFrequency);
        b.SetFloat("_ChargeDensity", chargeDensity);
        b.SetFloat("_ChargeInt", chargeIntensity);
        b.SetFloat("_ChargeLines", chargeLines);
        b.SetFloat("_ChargeFade", chargeFade);
    }

    void PushEffect_Blueprint(MaterialPropertyBlock b)
    {
        b.SetFloat("_Blueprint_En", blueprintEnabled ? 1f : 0f);
        b.SetFloat("_BlueprintMaskCh", (float)blueprintMaskCh);
        b.SetColor("_BlueprintBgCol", blueprintBgColor);
        b.SetColor("_BlueprintLineCol", blueprintLineColor);
        b.SetColor("_BlueprintGridCol", blueprintGridColor);
        b.SetFloat("_BlueprintEdgeSz", blueprintEdgeSize);
        b.SetFloat("_BlueprintLineWidth", blueprintLineWidth);
        b.SetFloat("_BlueprintLineInt", blueprintLineIntensity);
        b.SetFloat("_BlueprintGridDen", blueprintGridDensity);
        b.SetFloat("_BlueprintGridOp", blueprintGridOpacity);
        b.SetFloat("_BlueprintFade", blueprintFade);
    }

    void PushEffect_Ghosting(MaterialPropertyBlock b)
    {
        b.SetFloat("_Ghosting_En", ghostingEnabled ? 1f : 0f);
        b.SetFloat("_GhostingMaskCh", (float)ghostingMaskCh);
        b.SetFloat("_GhostingCount", (float)ghostingCount);
        b.SetVector("_GhostingOffset", ghostingOffset);
        b.SetFloat("_GhostingFade", ghostingFade);
    }

    void PushEffect_RetroPalette(MaterialPropertyBlock b)
    {
        b.SetFloat("_RetroPalette_En", retroPaletteEnabled ? 1f : 0f);
        b.SetFloat("_RetroMaskCh", (float)retroMaskCh);
        b.SetFloat("_RetroPixelSize", retroPixelSize);
        b.SetFloat("_RetroPaletteSize", retroPaletteSize);
        if (retroPaletteTex) b.SetTexture("_RetroPaletteTex", retroPaletteTex);
        b.SetFloat("_RetroDitherStrength", retroDitherStrength);
        b.SetFloat("_RetroFade", retroFade);
    }

    void PushEffect_StrongTint(Material m)
    {
        m.SetFloat("_StrongTint_En", strongTintEnabled ? 1f : 0f);
        m.SetFloat("_StrongTintMaskCh", (float)strongTintMaskCh);
        m.SetColor("_StrongTintColor", strongTintColor);
        m.SetFloat("_StrongTintFade", strongTintFade);
    }

    void PushEffect_AddColor(Material m)
    {
        m.SetFloat("_AddColor_En", addColorEnabled ? 1f : 0f);
        m.SetFloat("_AddColorMaskCh", (float)addColorMaskCh);
        m.SetColor("_AddColorAmount", addColorAmount);
        m.SetFloat("_AddColorFade", addColorFade);
    }

    void PushEffect_AlphaTint(Material m)
    {
        m.SetFloat("_AlphaTint_En", alphaTintEnabled ? 1f : 0f);
        m.SetFloat("_AlphaTintMaskCh", (float)alphaTintMaskCh);
        m.SetColor("_AlphaTintColor", alphaTintColor);
        m.SetFloat("_AlphaTintFade", alphaTintFade);
    }

    void PushEffect_HitEffect(Material m)
    {
        m.SetFloat("_HitEffect_En", hitEffectEnabled ? 1f : 0f);
        m.SetFloat("_HitEffectMaskCh", (float)hitEffectMaskCh);
        m.SetColor("_HitEffectColor", hitEffectColor);
        m.SetFloat("_HitEffectBlend", hitEffectBlend);
    }

    void PushEffect_Negative(Material m)
    {
        m.SetFloat("_Negative_En", negativeEnabled ? 1f : 0f);
        m.SetFloat("_NegativeMaskCh", (float)negativeMaskCh);
        m.SetFloat("_NegativeAmount", negativeAmount);
    }

    void PushEffect_Greyscale(Material m)
    {
        m.SetFloat("_Greyscale_En", greyscaleEnabled ? 1f : 0f);
        m.SetFloat("_GreyscaleMaskCh", (float)greyscaleMaskCh);
        m.SetFloat("_GreyscaleBlend", greyscaleBlend);
        m.SetVector("_GreyscaleLuminosity", greyscaleLuminosity);
    }

    void PushEffect_Bloom(Material m)
    {
        m.SetFloat("_Bloom_En", bloomEnabled ? 1f : 0f);
        m.SetFloat("_BloomMaskCh", (float)bloomMaskCh);
        m.SetFloat("_BloomMode", (float)bloomMode);
        m.SetColor("_BloomColor", bloomColor);
        m.SetFloat("_BloomThreshold", bloomThreshold);
        m.SetFloat("_BloomIntensity", bloomIntensity);
        m.SetFloat("_BloomSize", bloomSize);
        m.SetFloat("_BloomSamples", (float)bloomSamples);
        m.SetFloat("_BloomSoftKnee", bloomSoftKnee);
        m.SetColor("_BloomMaskColor", bloomMaskColor);
        m.SetFloat("_BloomMaskRange", bloomMaskRange);
        m.SetFloat("_BloomMaskSoft", bloomMaskSoft);
        m.SetFloat("_BloomAlphaBleed", bloomAlphaBleed);
    }

    void PushEffect_ColorReplace(Material m)
    {
        m.SetFloat("_ColorReplace_En", colorReplaceEnabled ? 1f : 0f);
        m.SetFloat("_ColorReplaceMaskCh", (float)colorReplaceMaskCh);
        m.SetColor("_ColorReplaceSrc1", colorReplaceSrc1);
        m.SetColor("_ColorReplaceDst1", colorReplaceDst1);
        m.SetColor("_ColorReplaceSrc2", colorReplaceSrc2);
        m.SetColor("_ColorReplaceDst2", colorReplaceDst2);
        m.SetColor("_ColorReplaceSrc3", colorReplaceSrc3);
        m.SetColor("_ColorReplaceDst3", colorReplaceDst3);
        m.SetColor("_ColorReplaceSrc4", colorReplaceSrc4);
        m.SetColor("_ColorReplaceDst4", colorReplaceDst4);
        m.SetFloat("_ColorReplaceCount", (float)colorReplaceCount);
        m.SetFloat("_ColorReplaceRange", colorReplaceRange);
        m.SetFloat("_ColorReplaceFade", colorReplaceFade);
    }

    void PushEffect_LUTReplace(Material m)
    {
        m.SetFloat("_LUTReplace_En", lutReplaceEnabled ? 1f : 0f);
        m.SetFloat("_LUTReplaceMaskCh", (float)lutReplaceMaskCh);
        if (lutReplaceTexture) m.SetTexture("_LUTReplaceTex", lutReplaceTexture);
        m.SetFloat("_LUTSize", lutSize);
        m.SetFloat("_LUTReplaceFade", lutReplaceFade);
    }

    void PushEffect_HueShift(Material m)
    {
        m.SetFloat("_HueShift_En", hueShiftEnabled ? 1f : 0f);
        m.SetFloat("_HueShiftMaskCh", (float)hueShiftMaskCh);
        m.SetFloat("_HueShiftAmount", hueShiftAmount);
        m.SetFloat("_HueShiftSpeed", hueShiftSpeed);
        m.SetFloat("_HueShiftFade", hueShiftFade);
    }

    void PushEffect_AddHue(Material m)
    {
        m.SetFloat("_AddHue_En", addHueEnabled ? 1f : 0f);
        m.SetFloat("_AddHueMaskCh", (float)addHueMaskCh);
        m.SetFloat("_AddHueAmount", addHueAmount);
        m.SetFloat("_AddHueSpeed", addHueSpeed);
        m.SetFloat("_AddHueSaturation", addHueSaturation);
        m.SetFloat("_AddHueBrightness", addHueBrightness);
        m.SetFloat("_AddHueFade", addHueFade);
    }

    void PushEffect_Saturation(Material m)
    {
        m.SetFloat("_Saturation_En", saturationEnabled ? 1f : 0f);
        m.SetFloat("_SaturationMaskCh", (float)saturationMaskCh);
        m.SetFloat("_SaturationAmount", saturationAmount);
    }

    void PushEffect_Brightness(Material m)
    {
        m.SetFloat("_Brightness_En", brightnessEnabled ? 1f : 0f);
        m.SetFloat("_BrightnessMaskCh", (float)brightnessMaskCh);
        m.SetFloat("_BrightnessAmount", brightnessAmount);
    }

    void PushEffect_Contrast(Material m)
    {
        m.SetFloat("_Contrast_En", contrastEnabled ? 1f : 0f);
        m.SetFloat("_ContrastMaskCh", (float)contrastMaskCh);
        m.SetFloat("_ContrastAmount", contrastAmount);
    }

    void PushEffect_SplitToning(Material m)
    {
        m.SetFloat("_SplitToning_En", splitToningEnabled ? 1f : 0f);
        m.SetFloat("_SplitToningMaskCh", (float)splitToningMaskCh);
        m.SetColor("_SplitShadowColor", splitShadowColor);
        m.SetColor("_SplitHighlightColor", splitHighlightColor);
        m.SetFloat("_SplitBalance", splitBalance);
        m.SetColor("_SplitMidColor", splitMidColor);
        m.SetFloat("_SplitFade", splitFade);
    }

    void PushEffect_Posterize(Material m)
    {
        m.SetFloat("_Posterize_En", posterizeEnabled ? 1f : 0f);
        m.SetFloat("_PosterizeMaskCh", (float)posterizeMaskCh);
        m.SetFloat("_PosterizeSteps", posterizeSteps);
        m.SetFloat("_PosterizeFade", posterizeFade);
    }

    void PushEffect_Poison(Material m)
    {
        m.SetFloat("_Poison_En", poisonEnabled ? 1f : 0f);
        m.SetFloat("_PoisonMaskCh", (float)poisonMaskCh);
        m.SetColor("_PoisonColor", poisonColor);
        m.SetFloat("_PoisonSpeed", poisonSpeed);
        m.SetFloat("_PoisonDensity", poisonDensity);
        m.SetColor("_PoisonColor2", poisonColor2);
        m.SetColor("_PoisonColor3", poisonColor3);
        m.SetFloat("_PoisonFade", poisonFade);
    }

    void PushEffect_Frozen(Material m)
    {
        m.SetFloat("_Frozen_En", frozenEnabled ? 1f : 0f);
        m.SetFloat("_FrozenMaskCh", (float)frozenMaskCh);
        m.SetColor("_FrozenColor", frozenColor);
        m.SetFloat("_FrozenContrast", frozenContrast);
        m.SetFloat("_FrozenHighlight", frozenHighlight);
        m.SetColor("_FrozenColor2", frozenColor2);
        m.SetColor("_FrozenHighlightColor", frozenHighlightColor);
        m.SetFloat("_FrozenFade", frozenFade);
    }

    void PushEffect_Metal(Material m)
    {
        m.SetFloat("_Metal_En", metalEnabled ? 1f : 0f);
        m.SetFloat("_MetalMaskCh", (float)metalMaskCh);
        m.SetColor("_MetalColor", metalColor);
        m.SetColor("_MetalHighlight", metalHighlight);
        m.SetFloat("_MetalContrast", metalContrast);
        m.SetFloat("_MetalSpeed", metalSpeed);
        m.SetColor("_MetalColor2", metalColor2);
        m.SetFloat("_MetalFade", metalFade);
    }

    void PushEffect_Camouflage(Material m)
    {
        m.SetFloat("_Camouflage_En", camouflageEnabled ? 1f : 0f);
        m.SetFloat("_CamouflageMaskCh", (float)camouflageMaskCh);
        m.SetColor("_CamoColor1", camoColor1);
        m.SetColor("_CamoColor2", camoColor2);
        m.SetColor("_CamoColor3", camoColor3);
        m.SetFloat("_CamoScale", camoScale);
        m.SetFloat("_CamoSpeed", camoSpeed);
        m.SetFloat("_CamoFade", camoFade);
    }

    void PushEffect_Rainbow(Material m)
    {
        m.SetFloat("_Rainbow_En", rainbowEnabled ? 1f : 0f);
        m.SetFloat("_RainbowMaskCh", (float)rainbowMaskCh);
        m.SetFloat("_RainbowSpeed", rainbowSpeed);
        m.SetFloat("_RainbowSaturation", rainbowSaturation);
        m.SetFloat("_RainbowBrightness", rainbowBrightness);
        m.SetFloat("_RainbowFade", rainbowFade);
        m.SetFloat("_RainbowDensity", rainbowDensity);
    }

    void PushEffect_BlackTint(Material m)
    {
        m.SetFloat("_BlackTint_En", blackTintEnabled ? 1f : 0f);
        m.SetFloat("_BlackTintMaskCh", (float)blackTintMaskCh);
        m.SetColor("_BlackTintColor", blackTintColor);
        m.SetFloat("_BlackTintThreshold", blackTintThreshold);
        m.SetFloat("_BlackTintFade", blackTintFade);
    }

    void PushEffect_Chromatic(Material m)
    {
        m.SetFloat("_Chromatic_En", chromaticEnabled ? 1f : 0f);
        m.SetFloat("_ChromaticMaskCh", (float)chromaticMaskCh);
        m.SetFloat("_ChromaticAmount", chromaticAmount);
        m.SetFloat("_ChromaticAngle", chromaticAngle);
    }

    void PushEffect_PaletteSwap(Material m)
    {
        m.SetFloat("_PaletteSwap_En", paletteSwapEnabled ? 1f : 0f);
        m.SetFloat("_PaletteSwapMaskCh", (float)paletteSwapMaskCh);
        if (paletteSwapTexture) m.SetTexture("_PaletteSwapTex", paletteSwapTexture);
        m.SetFloat("_PaletteSwapColors", (float)paletteSwapColors);
        m.SetFloat("_PaletteSwapFade", paletteSwapFade);
    }

    void PushEffect_Dithering(Material m)
    {
        m.SetFloat("_Dithering_En", ditheringEnabled ? 1f : 0f);
        m.SetFloat("_DitheringMaskCh", (float)ditheringMaskCh);
        m.SetFloat("_DitheringSize", ditheringSize);
        m.SetFloat("_DitheringLevels", ditheringLevels);
        m.SetFloat("_DitheringFade", ditheringFade);
        m.SetFloat("_DitheringAlphaMode", (float)ditheringAlphaMode);
        m.SetFloat("_DitheringAlphaThreshold", ditheringAlphaThreshold);
        m.SetFloat("_DitheringScreenSpace", ditheringScreenSpace ? 1f : 0f);
    }

    void PushEffect_CRTScanlines(Material m)
    {
        m.SetFloat("_CRTScanlines_En", crtScanlinesEnabled ? 1f : 0f);
        m.SetFloat("_CRTScanlinesMaskCh", (float)crtScanlinesMaskCh);
        m.SetFloat("_CRTDensity", crtDensity);
        m.SetFloat("_CRTIntensity", crtIntensity);
        m.SetFloat("_CRTSpeed", crtSpeed);
        m.SetFloat("_CRTCurvature", crtCurvature);
    }

    void PushEffect_Vignette(Material m)
    {
        m.SetFloat("_Vignette_En", vignetteEnabled ? 1f : 0f);
        m.SetFloat("_VignetteMaskCh", (float)vignetteMaskCh);
        m.SetColor("_VignetteColor", vignetteColor);
        m.SetFloat("_VignetteIntensity", vignetteIntensity);
        m.SetFloat("_VignetteSmoothness", vignetteSmoothness);
        m.SetFloat("_VignetteAlpha", vignetteAlpha);
        m.SetVector("_VignetteCenter", vignetteCenter);
        m.SetVector("_VignetteOffset", new Vector4(vignetteOffset.x, vignetteOffset.y, 0, 0));
    }

    void PushEffect_ColorGrading(Material m)
    {
        m.SetFloat("_ColorGrading_En", colorGradingEnabled ? 1f : 0f);
        m.SetFloat("_ColorGradingMaskCh", (float)colorGradingMaskCh);
        m.SetFloat("_CGExposure", cgExposure);
        m.SetFloat("_CGGamma", cgGamma);
        m.SetVector("_CGLift", cgLift);
    }

    void PushEffect_Sepia(Material m)
    {
        m.SetFloat("_Sepia_En", sepiaEnabled ? 1f : 0f);
        m.SetFloat("_SepiaMaskCh", (float)sepiaMaskCh);
        m.SetFloat("_SepiaIntensity", sepiaIntensity);
        m.SetColor("_SepiaColor", sepiaColor);
    }

    void PushEffect_ThermalVision(Material m)
    {
        m.SetFloat("_ThermalVision_En", thermalVisionEnabled ? 1f : 0f);
        m.SetFloat("_ThermalVisionMaskCh", (float)thermalVisionMaskCh);
        m.SetFloat("_ThermalIntensity", thermalIntensity);
        m.SetFloat("_ThermalHDRGlow", thermalHDRGlow);
    }

    void PushEffect_RecolorRGB(Material m)
    {
        m.SetFloat("_RecolorRGB_En", recolorRGBEnabled ? 1f : 0f);
        m.SetFloat("_RecolorRGBMaskCh", (float)recolorRGBMaskCh);
        m.SetColor("_RecolorR", recolorR);
        m.SetColor("_RecolorG", recolorG);
        m.SetColor("_RecolorB", recolorB);
        m.SetColor("_RecolorCyan", recolorCyan);
        m.SetColor("_RecolorMagenta", recolorMagenta);
        m.SetColor("_RecolorYellow", recolorYellow);
        m.SetFloat("_RecolorFade", recolorFade);
    }

    void PushEffect_PingPongGlow(Material m)
    {
        m.SetFloat("_PingPongGlow_En", pingPongGlowEnabled ? 1f : 0f);
        m.SetFloat("_PingPongGlowMaskCh", (float)pingPongGlowMaskCh);
        m.SetColor("_PingPongGlowColor", pingPongGlowColor);
        m.SetFloat("_PingPongGlowSpeed", pingPongGlowSpeed);
        m.SetFloat("_PingPongGlowMin", pingPongGlowMin);
        m.SetFloat("_PingPongGlowMax", pingPongGlowMax);
    }

    void PushEffect_ColorCycle(Material m)
    {
        m.SetFloat("_ColorCycle_En", colorCycleEnabled ? 1f : 0f);
        m.SetFloat("_ColorCycleMaskCh", (float)colorCycleMaskCh);
        m.SetColor("_ColorCycleA", colorCycleA);
        m.SetColor("_ColorCycleB", colorCycleB);
        m.SetColor("_ColorCycleC", colorCycleC);
        m.SetColor("_ColorCycleD", colorCycleD);
        m.SetFloat("_ColorCycleSpeed", colorCycleSpeed);
        m.SetFloat("_ColorCycleFade", colorCycleFade);
    }

    void PushEffect_Outline(Material m)
    {
        m.SetFloat("_Outline_En", outlineEnabled ? 1f : 0f);
        m.SetFloat("_OutlineMaskCh", (float)outlineMaskCh);
        m.SetColor("_OutlineColor", outlineColor);
        m.SetFloat("_OutlineSize", outlineSize);
        m.SetFloat("_OutlineAlpha", outlineAlpha);
    }

    void PushEffect_InnerOutline(Material m)
    {
        m.SetFloat("_InnerOutline_En", innerOutlineEnabled ? 1f : 0f);
        m.SetFloat("_InnerOutlineMaskCh", (float)innerOutlineMaskCh);
        m.SetColor("_InnerOutlineColor", innerOutlineColor);
        m.SetFloat("_InnerOutlineThickness", innerOutlineThickness);
        m.SetFloat("_InnerOutlineAlpha", innerOutlineAlpha);
    }

    void PushEffect_TexOutline(Material m)
    {
        m.SetFloat("_TexOutline_En", texOutlineEnabled ? 1f : 0f);
        m.SetFloat("_TexOutlineMaskCh", (float)texOutlineMaskCh);
        if (texOutlineTexture) m.SetTexture("_TexOutlineTex", texOutlineTexture);
        m.SetColor("_TexOutlineColor", texOutlineColor);
        m.SetFloat("_TexOutlineSize", texOutlineSize);
        m.SetVector("_TexOutlineTiling", texOutlineTiling);
        m.SetVector("_TexOutlineSpeed", texOutlineSpeed);
    }

    void PushEffect_RainbowOutline(Material m)
    {
        m.SetFloat("_RainbowOutline_En", rainbowOutlineEnabled ? 1f : 0f);
        m.SetFloat("_RainbowOutlineMaskCh", (float)rainbowOutlineMaskCh);
        m.SetColor("_RainbowOutlineColor", rainbowOutlineColor);
        m.SetFloat("_RainbowOutlineSize", rainbowOutlineSize);
        m.SetFloat("_RainbowOutlineSpeed", rainbowOutlineSpeed);
        m.SetFloat("_RainbowOutlineDensity", rainbowOutlineDensity);
    }

    void PushEffect_DistortOutline(Material m)
    {
        m.SetFloat("_DistortOutline_En", distortOutlineEnabled ? 1f : 0f);
        m.SetFloat("_DistortOutlineMaskCh", (float)distortOutlineMaskCh);
        m.SetColor("_DistortOutlineColor", distortOutlineColor);
        m.SetFloat("_DistortOutlineSize", distortOutlineSize);
        m.SetFloat("_DistortOutlineAmount", distortOutlineAmount);
        m.SetFloat("_DistortOutlineSpeed", distortOutlineSpeed);
    }

    void PushEffect_Glow(Material m)
    {
        m.SetFloat("_Glow_En", glowEnabled ? 1f : 0f);
        m.SetFloat("_GlowMaskCh", (float)glowMaskCh);
        m.SetColor("_GlowColor", glowColor);
        m.SetFloat("_GlowSize", glowSize);
        m.SetFloat("_GlowIntensity", glowIntensity);
    }

    void PushEffect_Shadow(Material m)
    {
        m.SetFloat("_Shadow_En", shadowEnabled ? 1f : 0f);
        m.SetFloat("_ShadowMaskCh", (float)shadowMaskCh);
        m.SetColor("_ShadowColor", shadowColor);
        m.SetVector("_ShadowOffset", shadowOffset);
    }

    void PushEffect_SoftShadow(Material m)
    {
        m.SetFloat("_SoftShadow_En", softShadowEnabled ? 1f : 0f);
        m.SetFloat("_SoftShadowMaskCh", (float)softShadowMaskCh);
        m.SetColor("_SoftShadowColor", softShadowColor);
        m.SetVector("_SoftShadowOffset", softShadowOffset);
        m.SetFloat("_SoftShadowSoftness", softShadowSoftness);
        m.SetFloat("_SoftShadowSamples", (float)softShadowSamples);
    }

    void PushEffect_DirOutline(Material m)
    {
        m.SetFloat("_DirOutline_En", dirOutlineEnabled ? 1f : 0f);
        m.SetFloat("_DirOutlineMaskCh", (float)dirOutlineMaskCh);
        m.SetColor("_DirOutlineColor", dirOutlineColor);
        m.SetFloat("_DirOutlineWidth", dirOutlineWidth);
        m.SetFloat("_DirOutlineInt", dirOutlineIntensity);
        m.SetVector("_DirOutlineDir", new Vector4(dirOutlineDirection.x, dirOutlineDirection.y, 0, 0));
        m.SetFloat("_DirOutlineSoft", dirOutlineSoftness);
    }

    void PushEffect_OutlineOnly(Material m)
    {
        m.SetFloat("_OutlineOnly_En", outlineOnlyEnabled ? 1f : 0f);
        m.SetFloat("_OutlineOnlyMaskCh", (float)outlineOnlyMaskCh);
        m.SetColor("_OutlineOnlyColor", outlineOnlyColor);
        m.SetFloat("_OutlineOnlyWidth", outlineOnlyWidth);
    }

    void PushEffect_GradientOutline(Material m)
    {
        m.SetFloat("_GradOutline_En", gradOutlineEnabled ? 1f : 0f);
        m.SetFloat("_GradOutlineMaskCh", (float)gradOutlineMaskCh);
        m.SetColor("_GradOutlineColorA", gradOutlineColorA);
        m.SetColor("_GradOutlineColorB", gradOutlineColorB);
        m.SetFloat("_GradOutlineAngle", gradOutlineAngle);
        m.SetFloat("_GradOutlineSize", gradOutlineSize);
    }

    void PushEffect_AnimOutlineThick(Material m)
    {
        m.SetFloat("_AnimOutline_En", animOutlineEnabled ? 1f : 0f);
        m.SetFloat("_AnimOutlineMaskCh", (float)animOutlineMaskCh);
        m.SetColor("_AnimOutlineColor", animOutlineColor);
        m.SetFloat("_AnimOutlineMin", animOutlineMin);
        m.SetFloat("_AnimOutlineMax", animOutlineMax);
        m.SetFloat("_AnimOutlineSpeed", animOutlineSpeed);
    }

    void PushEffect_SelectionHighlight(Material m)
    {
        m.SetFloat("_SelectHighlight_En", selectHighlightEnabled ? 1f : 0f);
        m.SetFloat("_SelectHighlightMaskCh", (float)selectHighlightMaskCh);
        m.SetColor("_SelectHighlightColor", selectHighlightColor);
        m.SetFloat("_SelectHighlightSize", selectHighlightSize);
        m.SetFloat("_SelectHighlightDash", selectHighlightDash);
        m.SetFloat("_SelectHighlightSpeed", selectHighlightSpeed);
    }

    void PushEffect_Pixelate(Material m)
    {
        m.SetFloat("_Pixelate_En", pixelateEnabled ? 1f : 0f);
        m.SetFloat("_PixelateMaskCh", (float)pixelateMaskCh);
        m.SetFloat("_PixelateSize", pixelateSize);
    }

    void PushEffect_Wave(Material m)
    {
        m.SetFloat("_Wave_En", waveEnabled ? 1f : 0f);
        m.SetFloat("_WaveMaskCh", (float)waveMaskCh);
        m.SetFloat("_WaveSpeed", waveSpeed);
        m.SetFloat("_WaveAmplitude", waveAmplitude);
        m.SetFloat("_WaveFrequency", waveFrequency);
    }

    void PushEffect_Twist(Material m)
    {
        m.SetFloat("_Twist_En", twistEnabled ? 1f : 0f);
        m.SetFloat("_TwistMaskCh", (float)twistMaskCh);
        m.SetFloat("_TwistAmount", twistAmount);
        m.SetFloat("_TwistSpeed", twistSpeed);
        m.SetVector("_TwistOffset", new Vector4(twistOffset.x, twistOffset.y, 0, 0));
        m.SetFloat("_TwistRadius", twistRadius);
    }

    void PushEffect_UVDistort(Material m)
    {
        m.SetFloat("_UVDistort_En", uvDistortEnabled ? 1f : 0f);
        m.SetFloat("_UVDistortMaskCh", (float)uvDistortMaskCh);
        if (distortTexture) m.SetTexture("_DistortTex", distortTexture);
        m.SetFloat("_DistortAmount", distortAmount);
        m.SetVector("_DistortTiling", distortTiling);
        m.SetFloat("_DistortSpeed", distortSpeed);
        m.SetFloat("_DistortIsNormalMap", (float)distortIsNormalMap);
    }

    void PushEffect_FullDistort(Material m)
    {
        m.SetFloat("_FullDistort_En", fullDistortEnabled ? 1f : 0f);
        m.SetFloat("_FullDistortMaskCh", (float)fullDistortMaskCh);
        m.SetFloat("_FullDistortAmount", fullDistortAmount);
        m.SetFloat("_FullDistortSpeed", fullDistortSpeed);
        m.SetFloat("_FullDistortDensity", fullDistortDensity);
    }

    void PushEffect_DirDistort(Material m)
    {
        m.SetFloat("_DirDistort_En", dirDistortEnabled ? 1f : 0f);
        m.SetFloat("_DirDistortMaskCh", (float)dirDistortMaskCh);
        m.SetFloat("_DirDistortAngle", dirDistortAngle);
        m.SetFloat("_DirDistortAmount", dirDistortAmount);
        m.SetFloat("_DirDistortSpeed", dirDistortSpeed);
    }

    void PushEffect_UVScroll(Material m)
    {
        m.SetFloat("_UVScroll_En", uvScrollEnabled ? 1f : 0f);
        m.SetFloat("_UVScrollMaskCh", (float)uvScrollMaskCh);
        m.SetFloat("_UVScrollSpeedX", uvScrollSpeedX);
        m.SetFloat("_UVScrollSpeedY", uvScrollSpeedY);
    }

    void PushEffect_Rotate(Material m)
    {
        m.SetFloat("_Rotate_En", rotateEnabled ? 1f : 0f);
        m.SetFloat("_RotateMaskCh", (float)rotateMaskCh);
        m.SetFloat("_RotateSpeed", rotateSpeed);
        m.SetFloat("_RotateAmount", rotateAmount);
    }

    void PushEffect_Squeeze(Material m)
    {
        m.SetFloat("_Squeeze_En", squeezeEnabled ? 1f : 0f);
        m.SetFloat("_SqueezeMaskCh", (float)squeezeMaskCh);
        m.SetVector("_SqueezeAmount", squeezeAmount);
        m.SetFloat("_SqueezeSpeed", squeezeSpeed);
        m.SetFloat("_SqueezePower", squeezePower);
    }

    void PushEffect_Squish(Material m)
    {
        m.SetFloat("_Squish_En", squishEnabled ? 1f : 0f);
        m.SetFloat("_SquishMaskCh", (float)squishMaskCh);
        m.SetVector("_SquishAmount", squishAmount);
        m.SetFloat("_SquishSpeed", squishSpeed);
        m.SetFloat("_SquishPower", squishPower);
    }

    void PushEffect_Wiggle(Material m)
    {
        m.SetFloat("_Wiggle_En", wiggleEnabled ? 1f : 0f);
        m.SetFloat("_WiggleMaskCh", (float)wiggleMaskCh);
        m.SetFloat("_WiggleSpeed", wiggleSpeed);
        m.SetFloat("_WiggleAmount", wiggleAmount);
        m.SetFloat("_WiggleFrequency", wiggleFrequency);
    }

    void PushEffect_Wind(Material m)
    {
        m.SetFloat("_Wind_En", windEnabled ? 1f : 0f);
        m.SetFloat("_WindMaskCh", (float)windMaskCh);
        m.SetFloat("_WindSpeed", windSpeed);
        m.SetFloat("_WindAmount", windAmount);
        m.SetFloat("_WindBottomY", windBottomY);
        m.SetFloat("_WindFrequency", windFrequency);
    }

    void PushEffect_Sharpen(Material m)
    {
        m.SetFloat("_Sharpen_En", sharpenEnabled ? 1f : 0f);
        m.SetFloat("_SharpenMaskCh", (float)sharpenMaskCh);
        m.SetFloat("_SharpenAmount", sharpenAmount);
        m.SetFloat("_SharpenSize", sharpenSize);
    }

    void PushEffect_GaussBlur(Material m)
    {
        m.SetFloat("_GaussBlur_En", gaussBlurEnabled ? 1f : 0f);
        m.SetFloat("_GaussBlurMaskCh", (float)gaussBlurMaskCh);
        m.SetFloat("_GaussBlurSize", gaussBlurSize);
        m.SetFloat("_GaussBlurSamples", gaussBlurSamples);
    }

    void PushEffect_DirBlur(Material m)
    {
        m.SetFloat("_DirBlur_En", dirBlurEnabled ? 1f : 0f);
        m.SetFloat("_DirBlurMaskCh", (float)dirBlurMaskCh);
        m.SetFloat("_DirBlurAngle", dirBlurAngle);
        m.SetFloat("_DirBlurSize", dirBlurSize);
        m.SetFloat("_DirBlurSamples", dirBlurSamples);
    }

    void PushEffect_WorldTiling(Material m)
    {
        m.SetFloat("_WorldTiling_En", worldTilingEnabled ? 1f : 0f);
        m.SetFloat("_WorldTilingMaskCh", (float)worldTilingMaskCh);
        m.SetFloat("_WorldTileScale", worldTileScale);
        m.SetVector("_WorldTileOffset", new Vector4(worldTileOffset.x, worldTileOffset.y, 0, 0));
        m.SetFloat("_WorldTileRotation", worldTileRotation);
    }

    void PushEffect_UVScaleOffset(Material m)
    {
        m.SetFloat("_UVScaleOffset_En", uvScaleOffsetEnabled ? 1f : 0f);
        m.SetFloat("_UVScaleOffsetMaskCh", (float)uvScaleOffsetMaskCh);
        m.SetVector("_UVScaleOffset", uvScaleOffset);
    }

    void PushEffect_CurlNoise(Material m)
    {
        m.SetFloat("_CurlNoise_En", curlNoiseEnabled ? 1f : 0f);
        m.SetFloat("_CurlNoiseMaskCh", (float)curlNoiseMaskCh);
        m.SetFloat("_CurlNoiseAmount", curlNoiseAmount);
        m.SetFloat("_CurlNoiseSpeed", curlNoiseSpeed);
        m.SetFloat("_CurlNoiseDensity", curlNoiseDensity);
    }

    void PushEffect_FBMNoise(Material m)
    {
        m.SetFloat("_FBMNoise_En", fbmNoiseEnabled ? 1f : 0f);
        m.SetFloat("_FBMNoiseMaskCh", (float)fbmNoiseMaskCh);
        m.SetFloat("_FBMNoiseAmount", fbmNoiseAmount);
        m.SetFloat("_FBMNoiseSpeed", fbmNoiseSpeed);
        m.SetFloat("_FBMNoiseDensity", fbmNoiseDensity);
        m.SetFloat("_FBMNoiseOctaves", (float)fbmNoiseOctaves);
    }

    void PushEffect_UVScrollOverlay(Material m)
    {
        m.SetFloat("_UVScrollOverlay_En", uvScrollOverlayEnabled ? 1f : 0f);
        m.SetFloat("_UVScrollOverlayMaskCh", (float)uvScrollOverlayMaskCh);
        if (uvScrollOverlayTex) m.SetTexture("_UVScrollOverlayTex", uvScrollOverlayTex);
        m.SetFloat("_UVScrollOverlaySpeedX", uvScrollOverlaySpeedX);
        m.SetFloat("_UVScrollOverlaySpeedY", uvScrollOverlaySpeedY);
        m.SetFloat("_UVScrollOverlayBlend", uvScrollOverlayBlend);
        m.SetColor("_UVScrollOverlayTint", uvScrollOverlayTint);
        m.SetVector("_UVScrollOverlayTiling", uvScrollOverlayTiling);
    }

    void PushEffect_Shockwave(Material m)
    {
        m.SetFloat("_Shockwave_En", shockwaveEnabled ? 1f : 0f);
        m.SetFloat("_ShockwaveMaskCh", (float)shockwaveMaskCh);
        m.SetVector("_ShockwaveCenter", new Vector4(shockwaveCenter.x, shockwaveCenter.y, 0, 0));
        m.SetFloat("_ShockwaveRadius", shockwaveRadius);
        m.SetFloat("_ShockwaveWidth", shockwaveWidth);
        m.SetFloat("_ShockwaveAmplitude", shockwaveAmplitude);
    }

    void PushEffect_SineScale(Material m)
    {
        m.SetFloat("_SineScale_En", sineScaleEnabled ? 1f : 0f);
        m.SetFloat("_SineScaleMaskCh", (float)sineScaleMaskCh);
        m.SetFloat("_SineScaleAmountX", sineScaleAmountX);
        m.SetFloat("_SineScaleAmountY", sineScaleAmountY);
        m.SetFloat("_SineScaleSpeed", sineScaleSpeed);
    }

    void PushEffect_Heartbeat(Material m)
    {
        m.SetFloat("_Heartbeat_En", heartbeatEnabled ? 1f : 0f);
        m.SetFloat("_HeartbeatMaskCh", (float)heartbeatMaskCh);
        m.SetFloat("_HeartbeatSpeed", heartbeatSpeed);
        m.SetFloat("_HeartbeatAmount", heartbeatAmount);
    }

    void PushEffect_Breathing(Material m)
    {
        m.SetFloat("_Breathing_En", breathingEnabled ? 1f : 0f);
        m.SetFloat("_BreathingMaskCh", (float)breathingMaskCh);
        m.SetFloat("_BreathingSpeed", breathingSpeed);
        m.SetFloat("_BreathingAmount", breathingAmount);
    }

    void PushEffect_LiquidWobble(Material m)
    {
        m.SetFloat("_LiquidWobble_En", liquidWobbleEnabled ? 1f : 0f);
        m.SetFloat("_LiquidWobbleMaskCh", (float)liquidWobbleMaskCh);
        m.SetFloat("_LiquidWobbleSpeed", liquidWobbleSpeed);
        m.SetFloat("_LiquidWobbleAmount", liquidWobbleAmount);
        m.SetFloat("_LiquidWobbleFreq", liquidWobbleFreq);
    }

    void PushEffect_FlipBookBlend(Material m)
    {
        m.SetFloat("_FlipBookBlend_En", flipBookBlendEnabled ? 1f : 0f);
        m.SetFloat("_FlipBookBlendMaskCh", (float)flipBookBlendMaskCh);
        if (flipBookNextFrame) m.SetTexture("_FlipBookNextFrame", flipBookNextFrame);
        m.SetFloat("_FlipBookLerp", flipBookLerp);
    }

    void PushEffect_SmoothPixelArt(Material m)
    {
        m.SetFloat("_SmoothPixelArt_En", smoothPixelArtEnabled ? 1f : 0f);
        m.SetFloat("_SmoothPixelArtMaskCh", (float)smoothPixelArtMaskCh);
        m.SetFloat("_SmoothPixelArtTexelSize", smoothPixelArtTexelSize);
    }

    void PushEffect_Dissolve(Material m)
    {
        m.SetFloat("_Dissolve_En", dissolveEnabled ? 1f : 0f);
        m.SetFloat("_DissolveMaskCh", (float)dissolveMaskCh);
        m.SetFloat("_DissolveNoiseType", (float)dissolveNoiseType);
        if (dissolveTexture) m.SetTexture("_DissolveTex", dissolveTexture);
        m.SetFloat("_DissolveNoiseScale", dissolveNoiseScale);
        m.SetFloat("_DissolveAmount", dissolveAmount);
        m.SetFloat("_DissolveBorderWidth", dissolveBorderWidth);
        m.SetFloat("_DissolveBurnWidth", dissolveBurnWidth);
        m.SetColor("_DissolveBurnColor", dissolveBurnColor);
        m.SetVector("_DissolveTiling", dissolveTiling);
        m.SetColor("_DissolveBorderColor", dissolveBorderColor);
    }

    void PushEffect_GlowDissolve(Material m)
    {
        m.SetFloat("_GlowDissolve_En", glowDissolveEnabled ? 1f : 0f);
        m.SetFloat("_GlowDissolveMaskCh", (float)glowDissolveMaskCh);
        m.SetFloat("_GlowDissolveNoiseType", (float)glowDissolveNoiseType);
        if (glowDissolveTexture) m.SetTexture("_GlowDissolveTex", glowDissolveTexture);
        m.SetFloat("_GlowDissolveNoiseScale", glowDissolveNoiseScale);
        m.SetFloat("_GlowDissolveAmount", glowDissolveAmount);
        m.SetColor("_GlowDissolveColor", glowDissolveColor);
        m.SetFloat("_GlowDissolveWidth", glowDissolveWidth);
    }

    void PushEffect_SrcDissolve(Material m)
    {
        m.SetFloat("_SrcDissolve_En", srcDissolveEnabled ? 1f : 0f);
        m.SetFloat("_SrcDissolveMaskCh", (float)srcDissolveMaskCh);
        m.SetFloat("_SrcDissolveAmount", srcDissolveAmount);
        m.SetVector("_SrcDissolvePosition", new Vector4(srcDissolvePosition.x, srcDissolvePosition.y, 0, 0));
        m.SetColor("_SrcDissolveBorderColor", srcDissolveBorderColor);
        m.SetFloat("_SrcDissolveBorderWidth", srcDissolveBorderWidth);
    }

    void PushEffect_DirFade(Material m)
    {
        m.SetFloat("_DirFade_En", dirFadeEnabled ? 1f : 0f);
        m.SetFloat("_DirFadeMaskCh", (float)dirFadeMaskCh);
        m.SetFloat("_DirFadeAngle", dirFadeAngle);
        m.SetFloat("_DirFadeAmount", dirFadeAmount);
        m.SetFloat("_DirFadeSmoothness", dirFadeSmoothness);
    }

    void PushEffect_DirGlowFade(Material m)
    {
        m.SetFloat("_DirGlowFade_En", dirGlowFadeEnabled ? 1f : 0f);
        m.SetFloat("_DirGlowFadeMaskCh", (float)dirGlowFadeMaskCh);
        m.SetFloat("_DirGlowFadeAngle", dirGlowFadeAngle);
        m.SetFloat("_DirGlowFadeAmount", dirGlowFadeAmount);
        m.SetColor("_DirGlowFadeColor", dirGlowFadeColor);
        m.SetFloat("_DirGlowFadeWidth", dirGlowFadeWidth);
    }

    void PushEffect_FullAlphaDissolve(Material m)
    {
        m.SetFloat("_FullAlphaDissolve_En", fullAlphaDissolveEnabled ? 1f : 0f);
        m.SetFloat("_FullAlphaDissolveMaskCh", (float)fullAlphaDissolveMaskCh);
        m.SetFloat("_FullAlphaDissolveAmount", fullAlphaDissolveAmount);
    }

    void PushEffect_Halftone(Material m)
    {
        m.SetFloat("_Halftone_En", halftoneEnabled ? 1f : 0f);
        m.SetFloat("_HalftoneMaskCh", (float)halftoneMaskCh);
        m.SetFloat("_HalftoneSize", halftoneSize);
        m.SetFloat("_HalftoneAmount", halftoneAmount);
        m.SetFloat("_HalftoneSmoothness", halftoneSmoothness);
    }

    void PushEffect_RadialWipe(Material m)
    {
        m.SetFloat("_RadialWipe_En", radialWipeEnabled ? 1f : 0f);
        m.SetFloat("_RadialWipeMaskCh", (float)radialWipeMaskCh);
        m.SetFloat("_RadialWipeAmount", radialWipeAmount);
        m.SetFloat("_RadialWipeSmoothness", radialWipeSmoothness);
        m.SetFloat("_RadialWipeAngle", radialWipeAngle);
    }

    void PushEffect_Blinds(Material m)
    {
        m.SetFloat("_Blinds_En", blindsEnabled ? 1f : 0f);
        m.SetFloat("_BlindsMaskCh", (float)blindsMaskCh);
        m.SetFloat("_BlindsAmount", blindsAmount);
        m.SetFloat("_BlindsCount", blindsCount);
        m.SetFloat("_BlindsDirection", (float)blindsDirection);
    }

    void PushEffect_DiamondDissolve(Material m)
    {
        m.SetFloat("_DiamondDissolve_En", diamondDissolveEnabled ? 1f : 0f);
        m.SetFloat("_DiamondDissolveMaskCh", (float)diamondDissolveMaskCh);
        m.SetFloat("_DiamondDissolveAmount", diamondDissolveAmount);
        m.SetFloat("_DiamondDissolveScale", diamondDissolveScale);
    }

    void PushEffect_ScanlineDissolve(Material m)
    {
        m.SetFloat("_ScanlineDissolve_En", scanlineDissolveEnabled ? 1f : 0f);
        m.SetFloat("_ScanlineDissolveMaskCh", (float)scanlineDissolveMaskCh);
        m.SetFloat("_ScanlineDissolveAmount", scanlineDissolveAmount);
        m.SetFloat("_ScanlineDissolveDensity", scanlineDissolveDensity);
    }

    void PushEffect_SpreadFade(Material m)
    {
        m.SetFloat("_SpreadFade_En", spreadFadeEnabled ? 1f : 0f);
        m.SetFloat("_SpreadFadeMaskCh", (float)spreadFadeMaskCh);
        m.SetFloat("_SpreadFadeAmount", spreadFadeAmount);
        m.SetVector("_SpreadFadeOrigin", spreadFadeOrigin);
        m.SetFloat("_SpreadFadeSmoothness", spreadFadeSmoothness);
    }

    void PushEffect_PixelDissolve(Material m)
    {
        m.SetFloat("_PixelDissolve_En", pixelDissolveEnabled ? 1f : 0f);
        m.SetFloat("_PixelDissolveMaskCh", (float)pixelDissolveMaskCh);
        m.SetFloat("_PixelDissolveAmount", pixelDissolveAmount);
        m.SetFloat("_PixelDissolveGrid", pixelDissolveGrid);
    }

    void PushEffect_SpiralWipe(Material m)
    {
        m.SetFloat("_SpiralWipe_En", spiralWipeEnabled ? 1f : 0f);
        m.SetFloat("_SpiralWipeMaskCh", (float)spiralWipeMaskCh);
        m.SetFloat("_SpiralWipeAmount", spiralWipeAmount);
        m.SetFloat("_SpiralWipeSmoothness", spiralWipeSmoothness);
        m.SetFloat("_SpiralWipeArms", spiralWipeArms);
    }

    void PushEffect_HexDissolve(Material m)
    {
        m.SetFloat("_HexDissolve_En", hexDissolveEnabled ? 1f : 0f);
        m.SetFloat("_HexDissolveMaskCh", (float)hexDissolveMaskCh);
        m.SetFloat("_HexDissolveAmount", hexDissolveAmount);
        m.SetFloat("_HexDissolveScale", hexDissolveScale);
    }

    void PushEffect_Hologram(Material m)
    {
        m.SetFloat("_Hologram_En", hologramEnabled ? 1f : 0f);
        m.SetFloat("_HologramMaskCh", (float)hologramMaskCh);
        m.SetFloat("_HologramStripesAmount", hologramStripesAmount);
        m.SetFloat("_HologramMinAlpha", hologramMinAlpha);
        m.SetFloat("_HologramStripesSpeed", hologramStripesSpeed);
        m.SetColor("_HologramColor", hologramColor);
        m.SetFloat("_HologramBlend", hologramBlend);
        m.SetFloat("_HologramDistortion", hologramDistortion);
    }

    void PushEffect_Glitch(Material m)
    {
        m.SetFloat("_Glitch_En", glitchEnabled ? 1f : 0f);
        m.SetFloat("_GlitchMaskCh", (float)glitchMaskCh);
        m.SetFloat("_GlitchAmount", glitchAmount);
        m.SetFloat("_GlitchSpeed", glitchSpeed);
        m.SetFloat("_GlitchSize", glitchSize);
        m.SetFloat("_GlitchColorShift", glitchColorShift);
    }

    void PushEffect_Flicker(Material m)
    {
        m.SetFloat("_Flicker_En", flickerEnabled ? 1f : 0f);
        m.SetFloat("_FlickerMaskCh", (float)flickerMaskCh);
        m.SetFloat("_FlickerFreq", flickerFreq);
        m.SetFloat("_FlickerPercent", flickerPercent);
    }

    void PushEffect_Shine(Material m)
    {
        m.SetFloat("_Shine_En", shineEnabled ? 1f : 0f);
        m.SetFloat("_ShineMaskCh", (float)shineMaskCh);
        m.SetColor("_ShineColor", shineColor);
        m.SetFloat("_ShineWidth", shineWidth);
        m.SetFloat("_ShineSpeed", shineSpeed);
        m.SetFloat("_ShineDelay", shineDelay);
        m.SetFloat("_ShineRotation", shineRotation);
        m.SetFloat("_ShineGlow", shineGlow);
    }

    void PushEffect_Smoke(Material m)
    {
        m.SetFloat("_Smoke_En", smokeEnabled ? 1f : 0f);
        m.SetFloat("_SmokeMaskCh", (float)smokeMaskCh);
        m.SetColor("_SmokeColor", smokeColor);
        m.SetFloat("_SmokeDensity", smokeDensity);
        m.SetFloat("_SmokeSpeed", smokeSpeed);
        m.SetFloat("_SmokeTurbulence", smokeTurbulence);
        m.SetVector("_SmokeDirection", smokeDirection);
        m.SetFloat("_SmokeColorStr", smokeColorStrength);
        m.SetFloat("_SmokeFade", smokeFade);
    }

    void PushEffect_Flame(Material m)
    {
        m.SetFloat("_Flame_En", flameEnabled ? 1f : 0f);
        m.SetFloat("_FlameMaskCh", (float)flameMaskCh);
        m.SetColor("_FlameColor1", flameColor1);
        m.SetColor("_FlameColor2", flameColor2);
        m.SetFloat("_FlameSpeed", flameSpeed);
        m.SetFloat("_FlameDensity", flameDensity);
        m.SetFloat("_FlameOnlyMode", (float)flameOnlyMode);
        m.SetFloat("_FlameExtendSides", flameExtendSides);
        m.SetFloat("_FlameSideFalloff", flameSideFalloff);
        m.SetFloat("_FlameIntensity", flameIntensity);
    }

    void PushEffect_Enchanted(Material m)
    {
        m.SetFloat("_Enchanted_En", enchantedEnabled ? 1f : 0f);
        m.SetFloat("_EnchantedMaskCh", (float)enchantedMaskCh);
        m.SetColor("_EnchantedColor", enchantedColor);
        m.SetFloat("_EnchantedSpeed", enchantedSpeed);
        m.SetFloat("_EnchantedDensity", enchantedDensity);
        m.SetFloat("_EnchantedBrightness", enchantedBrightness);
        m.SetFloat("_EnchantedFade", enchantedFade);
    }

    void PushEffect_Shifting(Material m)
    {
        m.SetFloat("_Shifting_En", shiftingEnabled ? 1f : 0f);
        m.SetFloat("_ShiftingMaskCh", (float)shiftingMaskCh);
        m.SetColor("_ShiftingColor1", shiftingColor1);
        m.SetColor("_ShiftingColor2", shiftingColor2);
        m.SetFloat("_ShiftingSpeed", shiftingSpeed);
        m.SetFloat("_ShiftingFade", shiftingFade);
    }

    void PushEffect_ShiftRainbow(Material m)
    {
        m.SetFloat("_ShiftRainbow_En", shiftRainbowEnabled ? 1f : 0f);
        m.SetFloat("_ShiftRainbowMaskCh", (float)shiftRainbowMaskCh);
        m.SetFloat("_ShiftRainbowSpeed", shiftRainbowSpeed);
        m.SetFloat("_ShiftRainbowDensity", shiftRainbowDensity);
        m.SetFloat("_ShiftRainbowFade", shiftRainbowFade);
    }

    void PushEffect_Overlay(Material m)
    {
        m.SetFloat("_Overlay_En", overlayEnabled ? 1f : 0f);
        m.SetFloat("_OverlayMaskCh", (float)overlayMaskCh);
        if (overlayTexture) m.SetTexture("_OverlayTex", overlayTexture);
        m.SetColor("_OverlayColor", overlayColor);
        m.SetFloat("_OverlayBlend", overlayBlend);
        m.SetVector("_OverlayTiling", overlayTiling);
        m.SetVector("_OverlaySpeed", overlaySpeed);
    }

    void PushEffect_Checkerboard(Material m)
    {
        m.SetFloat("_Checkerboard_En", checkerboardEnabled ? 1f : 0f);
        m.SetFloat("_CheckerboardMaskCh", (float)checkerboardMaskCh);
        m.SetFloat("_CheckerCoordSpace", (float)checkerCoordSpace);
        m.SetFloat("_CheckerScale", checkerScale);
        m.SetColor("_CheckerColor1", checkerColor1);
        m.SetColor("_CheckerColor2", checkerColor2);
    }

    void PushEffect_UVVisualize(Material m)
    {
        m.SetFloat("_UVVisualize_En", uvVisualizeEnabled ? 1f : 0f);
        m.SetFloat("_UVVisualizeMaskCh", (float)uvVisualizeMaskCh);
        m.SetFloat("_UVVisualizeBlend", uvVisualizeBlend);
    }

    void PushEffect_HealthTint(Material m)
    {
        m.SetFloat("_HealthTint_En", healthTintEnabled ? 1f : 0f);
        m.SetFloat("_HealthTintMaskCh", (float)healthTintMaskCh);
        m.SetColor("_HealthTintLowColor", healthTintLowColor);
        m.SetColor("_HealthTintHighColor", healthTintHighColor);
        m.SetFloat("_HealthTintAmount", healthTintAmount);
        m.SetFloat("_HealthDamageNoise", healthDamageNoise);
        m.SetFloat("_HealthEdgeGrain", healthEdgeGrain);
        m.SetFloat("_HealthPulseSpeed", healthPulseSpeed);
        m.SetFloat("_HealthVignetteStr", healthVignetteStr);
        m.SetFloat("_HealthDesaturation", healthDesaturation);
    }

    void PushEffect_XRaySilhouette(Material m)
    {
        m.SetFloat("_XRaySilhouette_En", xraySilhouetteEnabled ? 1f : 0f);
        m.SetFloat("_XRaySilhouetteMaskCh", (float)xraySilhouetteMaskCh);
        m.SetColor("_XRaySilhouetteColor", xraySilhouetteColor);
        m.SetFloat("_XRaySilhouetteBlend", xraySilhouetteBlend);
    }

    void PushEffect_MinimapPing(Material m)
    {
        m.SetFloat("_MinimapPing_En", minimapPingEnabled ? 1f : 0f);
        m.SetFloat("_MinimapPingMaskCh", (float)minimapPingMaskCh);
        m.SetColor("_MinimapPingColor", minimapPingColor);
        m.SetFloat("_MinimapPingSpeed", minimapPingSpeed);
        m.SetFloat("_MinimapPingWidth", minimapPingWidth);
        m.SetVector("_MinimapPingOffset", new Vector4(minimapPingOffset.x, minimapPingOffset.y, 0, 0));
    }

    void PushEffect_DialogueEmphasis(Material m)
    {
        m.SetFloat("_DialogueEmphasis_En", dialogueEmphasisEnabled ? 1f : 0f);
        m.SetFloat("_DialogueEmphasisMaskCh", (float)dialogueEmphasisMaskCh);
        m.SetFloat("_DialogueBobSpeed", dialogueBobSpeed);
        m.SetFloat("_DialogueBobAmount", dialogueBobAmount);
        m.SetFloat("_DialogueBrightness", dialogueBrightness);
    }

    void PushEffect_VoronoiCrackle(Material m)
    {
        m.SetFloat("_VoronoiCrackle_En", voronoiCrackleEnabled ? 1f : 0f);
        m.SetFloat("_CrackleMaskCh", (float)crackleMaskCh);
        m.SetFloat("_CrackleScale", crackleScale);
        m.SetFloat("_CrackleSpeed", crackleSpeed);
        m.SetFloat("_CrackleEdgeWidth", crackleEdgeWidth);
        m.SetColor("_CrackleEdgeColor", crackleEdgeColor);
        m.SetFloat("_CrackleCellTint", crackleCellTint);
        m.SetFloat("_CrackleFade", crackleFade);
    }

    void PushEffect_WaterReflect(Material m)
    {
        m.SetFloat("_WaterReflect_En", waterReflectEnabled ? 1f : 0f);
        m.SetFloat("_WaterReflectMaskCh", (float)waterReflectMaskCh);
        m.SetColor("_WaterReflectTint", waterReflectTint);
        m.SetFloat("_WaterReflectYPoint", waterReflectYPoint);
        m.SetFloat("_WaterReflectDepth", waterReflectDepth);
        m.SetFloat("_WaterReflectOpacity", waterReflectOpacity);
        m.SetFloat("_WaterReflectTintAmount", waterReflectTintAmount);
        m.SetFloat("_WaterReflectWaveFreq", waterReflectWaveFreq);
        m.SetFloat("_WaterReflectWaveSpeed", waterReflectWaveSpeed);
        m.SetFloat("_WaterReflectWaveAmp", waterReflectWaveAmp);
        m.SetFloat("_WaterReflectFade", waterReflectFade);
    }

    void PushEffect_MotionSmear(Material m)
    {
        m.SetFloat("_MotionSmear_En", motionSmearEnabled ? 1f : 0f);
        m.SetFloat("_SmearMaskCh", (float)smearMaskCh);
        m.SetVector("_SmearDirection", smearDirection);
        m.SetFloat("_SmearAmount", smearAmount);
        m.SetFloat("_SmearSamples", smearSamples);
        m.SetFloat("_SmearFade", smearFade);
    }

    void PushEffect_EnergyAura(Material m)
    {
        m.SetFloat("_EnergyAura_En", energyAuraEnabled ? 1f : 0f);
        m.SetFloat("_AuraMaskCh", (float)auraMaskCh);
        m.SetColor("_AuraColor", auraColor);
        m.SetFloat("_AuraRadius", auraRadius);
        m.SetFloat("_AuraFrequency", auraFrequency);
        m.SetFloat("_AuraSpeed", auraSpeed);
        m.SetFloat("_AuraIntensity", auraIntensity);
        m.SetFloat("_AuraFade", auraFade);
    }

    void PushEffect_Gradient(Material m)
    {
        m.SetFloat("_Gradient_En", gradientEnabled ? 1f : 0f);
        m.SetFloat("_GradientMaskCh", (float)gradientMaskCh);
        m.SetColor("_GradientColorA", gradientColorA);
        m.SetColor("_GradientColorB", gradientColorB);
        m.SetFloat("_GradientAngle", gradientAngle);
        m.SetFloat("_GradientFade", gradientFade);
        m.SetFloat("_GradientUseTex", gradientUseGradientEditor ? 1f : 0f);
        if (gradientBakedTexture) m.SetTexture("_GradientTex", gradientBakedTexture);
        m.SetFloat("_GradientBlendMode", (float)gradientBlendMode);
    }

    void PushEffect_RadialGradient(Material m)
    {
        m.SetFloat("_RadialGradient_En", radialGradientEnabled ? 1f : 0f);
        m.SetFloat("_RadialGradMaskCh", (float)radialGradMaskCh);
        m.SetColor("_RadialGradCenterColor", radialGradCenterColor);
        m.SetColor("_RadialGradEdgeColor", radialGradEdgeColor);
        m.SetVector("_RadialGradCenter", new Vector4(radialGradCenter.x, radialGradCenter.y, 0, 0));
        m.SetFloat("_RadialGradRadius", radialGradRadius);
        m.SetFloat("_RadialGradFade", radialGradFade);
        m.SetFloat("_RadialGradUseTex", radialGradUseGradientEditor ? 1f : 0f);
        if (radialGradBakedTexture) m.SetTexture("_RadialGradTex", radialGradBakedTexture);
        m.SetFloat("_RadialGradBlendMode", (float)radialGradBlendMode);
    }

    void PushEffect_ColorRamp(Material m)
    {
        m.SetFloat("_ColorRamp_En", colorRampEnabled ? 1f : 0f);
        m.SetFloat("_ColorRampMaskCh", (float)colorRampMaskCh);
        if (colorRampTexture) m.SetTexture("_ColorRampTex", colorRampTexture);
        m.SetFloat("_ColorRampMode", (float)colorRampMode);
        m.SetFloat("_ColorRampIntensity", colorRampIntensity);
        m.SetColor("_ColorRampColorA", colorRampColorA);
        m.SetColor("_ColorRampColorB", colorRampColorB);
        m.SetFloat("_ColorRampFade", colorRampFade);
    }

    void PushEffect_FakeLighting(Material m)
    {
        m.SetFloat("_FakeLighting_En", fakeLightingEnabled ? 1f : 0f);
        m.SetFloat("_FakeLightMaskCh", (float)fakeLightMaskCh);
        m.SetVector("_FakeLightDir", fakeLightDirection);
        m.SetColor("_FakeLightColor", fakeLightColor);
        m.SetFloat("_FakeLightIntensity", fakeLightIntensity);
        m.SetFloat("_FakeLightAmbient", fakeLightAmbient);
        m.SetFloat("_FakeLightNDist", fakeLightNormalDist);
        m.SetFloat("_FakeLightNZ", fakeLightNormalZ);
        m.SetFloat("_FakeLightSpecPow", fakeLightSpecPower);
        m.SetFloat("_FakeLightSpecInt", fakeLightSpecIntensity);
        m.SetFloat("_FakeLightFade", fakeLightFade);
    }

    void PushEffect_UVBendPoint(Material m)
    {
        m.SetFloat("_UVBendPoint_En", uvBendPointEnabled ? 1f : 0f);
        m.SetFloat("_BendMaskCh", (float)bendMaskCh);
        m.SetVector("_BendLocalPoint", bendLocalPoint);
        m.SetVector("_BendWorldPoint", bendWorldPoint);
        m.SetFloat("_BendUseWorldSpace", bendUseWorldSpace ? 1f : 0f);
        m.SetFloat("_BendWorldScale", bendWorldScale);
        m.SetFloat("_BendRadius", bendRadius);
        m.SetFloat("_BendAmount", bendAmount);
        m.SetFloat("_BendFalloff", bendFalloff);
        m.SetFloat("_BendInvert", bendInvert ? 1f : 0f);
    }

    void PushEffect_BurnDirection(Material m)
    {
        m.SetFloat("_BurnDirection_En", burnDirectionEnabled ? 1f : 0f);
        m.SetFloat("_BurnDirMaskCh", (float)burnDirMaskCh);
        m.SetFloat("_BurnDirAngle", burnDirAngle);
        m.SetFloat("_BurnDirAmount", burnDirAmount);
        m.SetFloat("_BurnDirEdgeWidth", burnDirEdgeWidth);
        m.SetColor("_BurnDirFireColor", burnDirFireColor);
        m.SetColor("_BurnDirCharcoalColor", burnDirCharcoalColor);
        m.SetFloat("_BurnDirCharcoalWidth", burnDirCharcoalWidth);
        m.SetFloat("_BurnDirNoiseScale", burnDirNoiseScale);
    }

    void PushEffect_GlobalSmoke(Material m)
    {
        m.SetFloat("_GlobalSmoke_En", globalSmokeEnabled ? 1f : 0f);
        m.SetFloat("_GlobalSmokeMaskCh", (float)globalSmokeMaskCh);
        m.SetColor("_GlobalSmokeColor", globalSmokeColor);
        m.SetFloat("_GlobalSmokeDensity", globalSmokeDensity);
        m.SetFloat("_GlobalSmokeSpeed", globalSmokeSpeed);
        m.SetFloat("_GlobalSmokeOpacity", globalSmokeOpacity);
        m.SetVector("_GlobalSmokeDir", globalSmokeDirection);
        m.SetFloat("_GlobalSmokeOverlayOnly", globalSmokeOverlayOnly ? 1f : 0f);
    }

    void PushEffect_HolographicCard(Material m)
    {
        m.SetFloat("_HoloCard_En", holographicCardEnabled ? 1f : 0f);
        m.SetFloat("_HoloCardMaskCh", (float)holoCardMaskCh);
        m.SetFloat("_HoloCardDensity", holoCardDensity);
        m.SetFloat("_HoloCardSpeed", holoCardSpeed);
        m.SetFloat("_HoloCardSat", holoCardSaturation);
        m.SetFloat("_HoloCardBright", holoCardBrightness);
        m.SetFloat("_HoloCardFPow", holoCardFresnelPower);
        m.SetFloat("_HoloCardFShift", holoCardFresnelShift);
        m.SetFloat("_HoloCardScanDen", holoCardScanDensity);
        m.SetFloat("_HoloCardScanSpd", holoCardScanSpeed);
        m.SetFloat("_HoloCardScanInt", holoCardScanIntensity);
        m.SetFloat("_HoloCardFade", holoCardFade);
    }

    void PushEffect_CloudShadows(Material m)
    {
        m.SetFloat("_CloudShadows_En", cloudShadowsEnabled ? 1f : 0f);
        m.SetFloat("_CloudShadowMaskCh", (float)cloudShadowMaskCh);
        m.SetFloat("_CloudShadowScale", cloudShadowScale);
        m.SetVector("_CloudShadowSpeed", cloudShadowSpeed);
        m.SetFloat("_CloudShadowDark", cloudShadowDarkness);
        m.SetFloat("_CloudShadowThresh", cloudShadowThreshold);
        m.SetFloat("_CloudShadowSoft", cloudShadowSoftness);
        m.SetFloat("_CloudShadowFade", cloudShadowFade);
        m.SetFloat("_CloudShadowOverlayOnly", cloudShadowOverlayOnly ? 1f : 0f);
    }

    void PushEffect_ColorfulTrailA(Material m)
    {
        m.SetFloat("_TrailA_En", colorfulTrailAEnabled ? 1f : 0f);
        m.SetFloat("_TrailAMaskCh", (float)trailAMaskCh);
        m.SetVector("_TrailAVel", trailAVelocity);
        m.SetColor("_TrailASlowCol", trailASlowColor);
        m.SetColor("_TrailAFastCol", trailAFastColor);
        m.SetFloat("_TrailAMaxSpd", trailAMaxSpeed);
        m.SetFloat("_TrailACurve", trailACurvePower);
        m.SetFloat("_TrailAInt", trailAIntensity);
    }

    void PushEffect_ColorfulTrailB(Material m)
    {
        m.SetFloat("_TrailB_En", colorfulTrailBEnabled ? 1f : 0f);
        m.SetFloat("_TrailBMaskCh", (float)trailBMaskCh);
        m.SetVector("_TrailBVel", trailBVelocity);
        m.SetColor("_TrailBCol", trailBColor);
        m.SetFloat("_TrailBMaxSpd", trailBMaxSpeed);
        m.SetFloat("_TrailBCurve", trailBCurvePower);
        m.SetFloat("_TrailBInt", trailBIntensity);
        m.SetFloat("_TrailBSegs", (float)trailBSegments);
        m.SetFloat("_TrailBFade", trailBFadeOut);
    }

    void PushEffect_ColorfulTrailC(Material m)
    {
        m.SetFloat("_TrailC_En", colorfulTrailCEnabled ? 1f : 0f);
        m.SetFloat("_TrailCMaskCh", (float)trailCMaskCh);
        m.SetVector("_TrailCVel", trailCVelocity);
        m.SetColor("_TrailCColA", trailCColorA);
        m.SetColor("_TrailCColB", trailCColorB);
        m.SetFloat("_TrailCMaxSpd", trailCMaxSpeed);
        m.SetFloat("_TrailCCurve", trailCCurvePower);
        m.SetFloat("_TrailCInt", trailCIntensity);
        m.SetFloat("_TrailCRbSpd", trailCRainbowSpeed);
    }

    void PushEffect_ChargingAbility(Material m)
    {
        m.SetFloat("_Charging_En", chargingAbilityEnabled ? 1f : 0f);
        m.SetFloat("_ChargeMaskCh", (float)chargeMaskCh);
        m.SetColor("_ChargeColor", chargeColor);
        m.SetVector("_ChargeCenter", chargeCenter);
        m.SetFloat("_ChargeRadius", chargeRadius);
        m.SetFloat("_ChargeWidth", chargeWidth);
        m.SetFloat("_ChargeSpeed", chargeSpeed);
        m.SetFloat("_ChargeFreq", chargeFrequency);
        m.SetFloat("_ChargeDensity", chargeDensity);
        m.SetFloat("_ChargeInt", chargeIntensity);
        m.SetFloat("_ChargeLines", chargeLines);
        m.SetFloat("_ChargeFade", chargeFade);
    }

    void PushEffect_Blueprint(Material m)
    {
        m.SetFloat("_Blueprint_En", blueprintEnabled ? 1f : 0f);
        m.SetFloat("_BlueprintMaskCh", (float)blueprintMaskCh);
        m.SetColor("_BlueprintBgCol", blueprintBgColor);
        m.SetColor("_BlueprintLineCol", blueprintLineColor);
        m.SetColor("_BlueprintGridCol", blueprintGridColor);
        m.SetFloat("_BlueprintEdgeSz", blueprintEdgeSize);
        m.SetFloat("_BlueprintLineWidth", blueprintLineWidth);
        m.SetFloat("_BlueprintLineInt", blueprintLineIntensity);
        m.SetFloat("_BlueprintGridDen", blueprintGridDensity);
        m.SetFloat("_BlueprintGridOp", blueprintGridOpacity);
        m.SetFloat("_BlueprintFade", blueprintFade);
    }

    void PushEffect_Ghosting(Material m)
    {
        m.SetFloat("_Ghosting_En", ghostingEnabled ? 1f : 0f);
        m.SetFloat("_GhostingMaskCh", (float)ghostingMaskCh);
        m.SetFloat("_GhostingCount", (float)ghostingCount);
        m.SetVector("_GhostingOffset", ghostingOffset);
        m.SetFloat("_GhostingFade", ghostingFade);
    }

    void PushEffect_RetroPalette(Material m)
    {
        m.SetFloat("_RetroPalette_En", retroPaletteEnabled ? 1f : 0f);
        m.SetFloat("_RetroMaskCh", (float)retroMaskCh);
        m.SetFloat("_RetroPixelSize", retroPixelSize);
        m.SetFloat("_RetroPaletteSize", retroPaletteSize);
        if (retroPaletteTex) m.SetTexture("_RetroPaletteTex", retroPaletteTex);
        m.SetFloat("_RetroDitherStrength", retroDitherStrength);
        m.SetFloat("_RetroFade", retroFade);
    }

    void PushEffects(MaterialPropertyBlock b)
    {
        PushEffect_StrongTint(b);
        PushEffect_AddColor(b);
        PushEffect_AlphaTint(b);
        PushEffect_HitEffect(b);
        PushEffect_Negative(b);
        PushEffect_Greyscale(b);
        PushEffect_Bloom(b);
        PushEffect_ColorReplace(b);
        PushEffect_LUTReplace(b);
        PushEffect_HueShift(b);
        PushEffect_AddHue(b);
        PushEffect_Saturation(b);
        PushEffect_Brightness(b);
        PushEffect_Contrast(b);
        PushEffect_SplitToning(b);
        PushEffect_Posterize(b);
        PushEffect_Poison(b);
        PushEffect_Frozen(b);
        PushEffect_Metal(b);
        PushEffect_Camouflage(b);
        PushEffect_Rainbow(b);
        PushEffect_BlackTint(b);
        PushEffect_Chromatic(b);
        PushEffect_PaletteSwap(b);
        PushEffect_Dithering(b);
        PushEffect_CRTScanlines(b);
        PushEffect_Vignette(b);
        PushEffect_ColorGrading(b);
        PushEffect_Sepia(b);
        PushEffect_ThermalVision(b);
        PushEffect_RecolorRGB(b);
        PushEffect_PingPongGlow(b);
        PushEffect_ColorCycle(b);
        PushEffect_Outline(b);
        PushEffect_InnerOutline(b);
        PushEffect_TexOutline(b);
        PushEffect_RainbowOutline(b);
        PushEffect_DistortOutline(b);
        PushEffect_Glow(b);
        PushEffect_Shadow(b);
        PushEffect_SoftShadow(b);
        PushEffect_DirOutline(b);
        PushEffect_OutlineOnly(b);
        PushEffect_GradientOutline(b);
        PushEffect_AnimOutlineThick(b);
        PushEffect_SelectionHighlight(b);
        PushEffect_Pixelate(b);
        PushEffect_Wave(b);
        PushEffect_Twist(b);
        PushEffect_UVDistort(b);
        PushEffect_FullDistort(b);
        PushEffect_DirDistort(b);
        PushEffect_UVScroll(b);
        PushEffect_Rotate(b);
        PushEffect_Squeeze(b);
        PushEffect_Squish(b);
        PushEffect_Wiggle(b);
        PushEffect_Wind(b);
        PushEffect_Sharpen(b);
        PushEffect_GaussBlur(b);
        PushEffect_DirBlur(b);
        PushEffect_WorldTiling(b);
        PushEffect_UVScaleOffset(b);
        PushEffect_CurlNoise(b);
        PushEffect_FBMNoise(b);
        PushEffect_UVScrollOverlay(b);
        PushEffect_Shockwave(b);
        PushEffect_SineScale(b);
        PushEffect_Heartbeat(b);
        PushEffect_Breathing(b);
        PushEffect_LiquidWobble(b);
        PushEffect_FlipBookBlend(b);
        PushEffect_SmoothPixelArt(b);
        PushEffect_Dissolve(b);
        PushEffect_GlowDissolve(b);
        PushEffect_SrcDissolve(b);
        PushEffect_DirFade(b);
        PushEffect_DirGlowFade(b);
        PushEffect_FullAlphaDissolve(b);
        PushEffect_Halftone(b);
        PushEffect_RadialWipe(b);
        PushEffect_Blinds(b);
        PushEffect_DiamondDissolve(b);
        PushEffect_ScanlineDissolve(b);
        PushEffect_SpreadFade(b);
        PushEffect_PixelDissolve(b);
        PushEffect_SpiralWipe(b);
        PushEffect_HexDissolve(b);
        PushEffect_Hologram(b);
        PushEffect_Glitch(b);
        PushEffect_Flicker(b);
        PushEffect_Shine(b);
        PushEffect_Smoke(b);
        PushEffect_Flame(b);
        PushEffect_Enchanted(b);
        PushEffect_Shifting(b);
        PushEffect_ShiftRainbow(b);
        PushEffect_Overlay(b);
        PushEffect_Checkerboard(b);
        PushEffect_UVVisualize(b);
        PushEffect_HealthTint(b);
        PushEffect_XRaySilhouette(b);
        PushEffect_MinimapPing(b);
        PushEffect_DialogueEmphasis(b);
        PushEffect_VoronoiCrackle(b);
        PushEffect_WaterReflect(b);
        PushEffect_MotionSmear(b);
        PushEffect_EnergyAura(b);
        PushEffect_Gradient(b);
        PushEffect_RadialGradient(b);
        PushEffect_ColorRamp(b);
        PushEffect_FakeLighting(b);
        PushEffect_UVBendPoint(b);
        PushEffect_BurnDirection(b);
        PushEffect_GlobalSmoke(b);
        PushEffect_HolographicCard(b);
        PushEffect_CloudShadows(b);
        PushEffect_ColorfulTrailA(b);
        PushEffect_ColorfulTrailB(b);
        PushEffect_ColorfulTrailC(b);
        PushEffect_ChargingAbility(b);
        PushEffect_Blueprint(b);
        PushEffect_Ghosting(b);
        PushEffect_RetroPalette(b);
    }

    /// <summary>
    /// Generated material-direct push for GUI variants. Mirrors <see cref="PushEffects(MaterialPropertyBlock)"/>.
    /// Matches the partial declaration in AnimatedSpriteShader.cs. If the generator hasn't
    /// been run since the asset was imported, this body is absent and the call is elided,
    /// in which case PushGUI() additionally calls PushEffectsToMat() (reflection-based) so
    /// effects still push. Running Full Rebuild in Master Setup generates this file.
    /// </summary>
    partial void PushEffectsToMaterial(Material mat)
    {
        mat.SetFloat("_AIO_GenPushSentinel", 1f);
        PushEffect_StrongTint(mat);
        PushEffect_AddColor(mat);
        PushEffect_AlphaTint(mat);
        PushEffect_HitEffect(mat);
        PushEffect_Negative(mat);
        PushEffect_Greyscale(mat);
        PushEffect_Bloom(mat);
        PushEffect_ColorReplace(mat);
        PushEffect_LUTReplace(mat);
        PushEffect_HueShift(mat);
        PushEffect_AddHue(mat);
        PushEffect_Saturation(mat);
        PushEffect_Brightness(mat);
        PushEffect_Contrast(mat);
        PushEffect_SplitToning(mat);
        PushEffect_Posterize(mat);
        PushEffect_Poison(mat);
        PushEffect_Frozen(mat);
        PushEffect_Metal(mat);
        PushEffect_Camouflage(mat);
        PushEffect_Rainbow(mat);
        PushEffect_BlackTint(mat);
        PushEffect_Chromatic(mat);
        PushEffect_PaletteSwap(mat);
        PushEffect_Dithering(mat);
        PushEffect_CRTScanlines(mat);
        PushEffect_Vignette(mat);
        PushEffect_ColorGrading(mat);
        PushEffect_Sepia(mat);
        PushEffect_ThermalVision(mat);
        PushEffect_RecolorRGB(mat);
        PushEffect_PingPongGlow(mat);
        PushEffect_ColorCycle(mat);
        PushEffect_Outline(mat);
        PushEffect_InnerOutline(mat);
        PushEffect_TexOutline(mat);
        PushEffect_RainbowOutline(mat);
        PushEffect_DistortOutline(mat);
        PushEffect_Glow(mat);
        PushEffect_Shadow(mat);
        PushEffect_SoftShadow(mat);
        PushEffect_DirOutline(mat);
        PushEffect_OutlineOnly(mat);
        PushEffect_GradientOutline(mat);
        PushEffect_AnimOutlineThick(mat);
        PushEffect_SelectionHighlight(mat);
        PushEffect_Pixelate(mat);
        PushEffect_Wave(mat);
        PushEffect_Twist(mat);
        PushEffect_UVDistort(mat);
        PushEffect_FullDistort(mat);
        PushEffect_DirDistort(mat);
        PushEffect_UVScroll(mat);
        PushEffect_Rotate(mat);
        PushEffect_Squeeze(mat);
        PushEffect_Squish(mat);
        PushEffect_Wiggle(mat);
        PushEffect_Wind(mat);
        PushEffect_Sharpen(mat);
        PushEffect_GaussBlur(mat);
        PushEffect_DirBlur(mat);
        PushEffect_WorldTiling(mat);
        PushEffect_UVScaleOffset(mat);
        PushEffect_CurlNoise(mat);
        PushEffect_FBMNoise(mat);
        PushEffect_UVScrollOverlay(mat);
        PushEffect_Shockwave(mat);
        PushEffect_SineScale(mat);
        PushEffect_Heartbeat(mat);
        PushEffect_Breathing(mat);
        PushEffect_LiquidWobble(mat);
        PushEffect_FlipBookBlend(mat);
        PushEffect_SmoothPixelArt(mat);
        PushEffect_Dissolve(mat);
        PushEffect_GlowDissolve(mat);
        PushEffect_SrcDissolve(mat);
        PushEffect_DirFade(mat);
        PushEffect_DirGlowFade(mat);
        PushEffect_FullAlphaDissolve(mat);
        PushEffect_Halftone(mat);
        PushEffect_RadialWipe(mat);
        PushEffect_Blinds(mat);
        PushEffect_DiamondDissolve(mat);
        PushEffect_ScanlineDissolve(mat);
        PushEffect_SpreadFade(mat);
        PushEffect_PixelDissolve(mat);
        PushEffect_SpiralWipe(mat);
        PushEffect_HexDissolve(mat);
        PushEffect_Hologram(mat);
        PushEffect_Glitch(mat);
        PushEffect_Flicker(mat);
        PushEffect_Shine(mat);
        PushEffect_Smoke(mat);
        PushEffect_Flame(mat);
        PushEffect_Enchanted(mat);
        PushEffect_Shifting(mat);
        PushEffect_ShiftRainbow(mat);
        PushEffect_Overlay(mat);
        PushEffect_Checkerboard(mat);
        PushEffect_UVVisualize(mat);
        PushEffect_HealthTint(mat);
        PushEffect_XRaySilhouette(mat);
        PushEffect_MinimapPing(mat);
        PushEffect_DialogueEmphasis(mat);
        PushEffect_VoronoiCrackle(mat);
        PushEffect_WaterReflect(mat);
        PushEffect_MotionSmear(mat);
        PushEffect_EnergyAura(mat);
        PushEffect_Gradient(mat);
        PushEffect_RadialGradient(mat);
        PushEffect_ColorRamp(mat);
        PushEffect_FakeLighting(mat);
        PushEffect_UVBendPoint(mat);
        PushEffect_BurnDirection(mat);
        PushEffect_GlobalSmoke(mat);
        PushEffect_HolographicCard(mat);
        PushEffect_CloudShadows(mat);
        PushEffect_ColorfulTrailA(mat);
        PushEffect_ColorfulTrailB(mat);
        PushEffect_ColorfulTrailC(mat);
        PushEffect_ChargingAbility(mat);
        PushEffect_Blueprint(mat);
        PushEffect_Ghosting(mat);
        PushEffect_RetroPalette(mat);
    }

    partial void PushEffectToMaterial(Material mat, EffectDef def)
    {
        mat.SetFloat("_AIO_GenPushSentinel", 1f);
        switch (def.id)
        {
            case EffectId.StrongTint: PushEffect_StrongTint(mat); return;
            case EffectId.AddColor: PushEffect_AddColor(mat); return;
            case EffectId.AlphaTint: PushEffect_AlphaTint(mat); return;
            case EffectId.HitEffect: PushEffect_HitEffect(mat); return;
            case EffectId.Negative: PushEffect_Negative(mat); return;
            case EffectId.Greyscale: PushEffect_Greyscale(mat); return;
            case EffectId.Bloom: PushEffect_Bloom(mat); return;
            case EffectId.ColorReplace: PushEffect_ColorReplace(mat); return;
            case EffectId.LUTReplace: PushEffect_LUTReplace(mat); return;
            case EffectId.HueShift: PushEffect_HueShift(mat); return;
            case EffectId.AddHue: PushEffect_AddHue(mat); return;
            case EffectId.Saturation: PushEffect_Saturation(mat); return;
            case EffectId.Brightness: PushEffect_Brightness(mat); return;
            case EffectId.Contrast: PushEffect_Contrast(mat); return;
            case EffectId.SplitToning: PushEffect_SplitToning(mat); return;
            case EffectId.Posterize: PushEffect_Posterize(mat); return;
            case EffectId.Poison: PushEffect_Poison(mat); return;
            case EffectId.Frozen: PushEffect_Frozen(mat); return;
            case EffectId.Metal: PushEffect_Metal(mat); return;
            case EffectId.Camouflage: PushEffect_Camouflage(mat); return;
            case EffectId.Rainbow: PushEffect_Rainbow(mat); return;
            case EffectId.BlackTint: PushEffect_BlackTint(mat); return;
            case EffectId.Chromatic: PushEffect_Chromatic(mat); return;
            case EffectId.PaletteSwap: PushEffect_PaletteSwap(mat); return;
            case EffectId.Dithering: PushEffect_Dithering(mat); return;
            case EffectId.CRTScanlines: PushEffect_CRTScanlines(mat); return;
            case EffectId.Vignette: PushEffect_Vignette(mat); return;
            case EffectId.ColorGrading: PushEffect_ColorGrading(mat); return;
            case EffectId.Sepia: PushEffect_Sepia(mat); return;
            case EffectId.ThermalVision: PushEffect_ThermalVision(mat); return;
            case EffectId.RecolorRGB: PushEffect_RecolorRGB(mat); return;
            case EffectId.PingPongGlow: PushEffect_PingPongGlow(mat); return;
            case EffectId.ColorCycle: PushEffect_ColorCycle(mat); return;
            case EffectId.Outline: PushEffect_Outline(mat); return;
            case EffectId.InnerOutline: PushEffect_InnerOutline(mat); return;
            case EffectId.TexOutline: PushEffect_TexOutline(mat); return;
            case EffectId.RainbowOutline: PushEffect_RainbowOutline(mat); return;
            case EffectId.DistortOutline: PushEffect_DistortOutline(mat); return;
            case EffectId.Glow: PushEffect_Glow(mat); return;
            case EffectId.Shadow: PushEffect_Shadow(mat); return;
            case EffectId.SoftShadow: PushEffect_SoftShadow(mat); return;
            case EffectId.DirOutline: PushEffect_DirOutline(mat); return;
            case EffectId.OutlineOnly: PushEffect_OutlineOnly(mat); return;
            case EffectId.GradientOutline: PushEffect_GradientOutline(mat); return;
            case EffectId.AnimOutlineThick: PushEffect_AnimOutlineThick(mat); return;
            case EffectId.SelectionHighlight: PushEffect_SelectionHighlight(mat); return;
            case EffectId.Pixelate: PushEffect_Pixelate(mat); return;
            case EffectId.Wave: PushEffect_Wave(mat); return;
            case EffectId.Twist: PushEffect_Twist(mat); return;
            case EffectId.UVDistort: PushEffect_UVDistort(mat); return;
            case EffectId.FullDistort: PushEffect_FullDistort(mat); return;
            case EffectId.DirDistort: PushEffect_DirDistort(mat); return;
            case EffectId.UVScroll: PushEffect_UVScroll(mat); return;
            case EffectId.Rotate: PushEffect_Rotate(mat); return;
            case EffectId.Squeeze: PushEffect_Squeeze(mat); return;
            case EffectId.Squish: PushEffect_Squish(mat); return;
            case EffectId.Wiggle: PushEffect_Wiggle(mat); return;
            case EffectId.Wind: PushEffect_Wind(mat); return;
            case EffectId.Sharpen: PushEffect_Sharpen(mat); return;
            case EffectId.GaussBlur: PushEffect_GaussBlur(mat); return;
            case EffectId.DirBlur: PushEffect_DirBlur(mat); return;
            case EffectId.WorldTiling: PushEffect_WorldTiling(mat); return;
            case EffectId.UVScaleOffset: PushEffect_UVScaleOffset(mat); return;
            case EffectId.CurlNoise: PushEffect_CurlNoise(mat); return;
            case EffectId.FBMNoise: PushEffect_FBMNoise(mat); return;
            case EffectId.UVScrollOverlay: PushEffect_UVScrollOverlay(mat); return;
            case EffectId.Shockwave: PushEffect_Shockwave(mat); return;
            case EffectId.SineScale: PushEffect_SineScale(mat); return;
            case EffectId.Heartbeat: PushEffect_Heartbeat(mat); return;
            case EffectId.Breathing: PushEffect_Breathing(mat); return;
            case EffectId.LiquidWobble: PushEffect_LiquidWobble(mat); return;
            case EffectId.FlipBookBlend: PushEffect_FlipBookBlend(mat); return;
            case EffectId.SmoothPixelArt: PushEffect_SmoothPixelArt(mat); return;
            case EffectId.Dissolve: PushEffect_Dissolve(mat); return;
            case EffectId.GlowDissolve: PushEffect_GlowDissolve(mat); return;
            case EffectId.SrcDissolve: PushEffect_SrcDissolve(mat); return;
            case EffectId.DirFade: PushEffect_DirFade(mat); return;
            case EffectId.DirGlowFade: PushEffect_DirGlowFade(mat); return;
            case EffectId.FullAlphaDissolve: PushEffect_FullAlphaDissolve(mat); return;
            case EffectId.Halftone: PushEffect_Halftone(mat); return;
            case EffectId.RadialWipe: PushEffect_RadialWipe(mat); return;
            case EffectId.Blinds: PushEffect_Blinds(mat); return;
            case EffectId.DiamondDissolve: PushEffect_DiamondDissolve(mat); return;
            case EffectId.ScanlineDissolve: PushEffect_ScanlineDissolve(mat); return;
            case EffectId.SpreadFade: PushEffect_SpreadFade(mat); return;
            case EffectId.PixelDissolve: PushEffect_PixelDissolve(mat); return;
            case EffectId.SpiralWipe: PushEffect_SpiralWipe(mat); return;
            case EffectId.HexDissolve: PushEffect_HexDissolve(mat); return;
            case EffectId.Hologram: PushEffect_Hologram(mat); return;
            case EffectId.Glitch: PushEffect_Glitch(mat); return;
            case EffectId.Flicker: PushEffect_Flicker(mat); return;
            case EffectId.Shine: PushEffect_Shine(mat); return;
            case EffectId.Smoke: PushEffect_Smoke(mat); return;
            case EffectId.Flame: PushEffect_Flame(mat); return;
            case EffectId.Enchanted: PushEffect_Enchanted(mat); return;
            case EffectId.Shifting: PushEffect_Shifting(mat); return;
            case EffectId.ShiftRainbow: PushEffect_ShiftRainbow(mat); return;
            case EffectId.Overlay: PushEffect_Overlay(mat); return;
            case EffectId.Checkerboard: PushEffect_Checkerboard(mat); return;
            case EffectId.UVVisualize: PushEffect_UVVisualize(mat); return;
            case EffectId.HealthTint: PushEffect_HealthTint(mat); return;
            case EffectId.XRaySilhouette: PushEffect_XRaySilhouette(mat); return;
            case EffectId.MinimapPing: PushEffect_MinimapPing(mat); return;
            case EffectId.DialogueEmphasis: PushEffect_DialogueEmphasis(mat); return;
            case EffectId.VoronoiCrackle: PushEffect_VoronoiCrackle(mat); return;
            case EffectId.WaterReflect: PushEffect_WaterReflect(mat); return;
            case EffectId.MotionSmear: PushEffect_MotionSmear(mat); return;
            case EffectId.EnergyAura: PushEffect_EnergyAura(mat); return;
            case EffectId.Gradient: PushEffect_Gradient(mat); return;
            case EffectId.RadialGradient: PushEffect_RadialGradient(mat); return;
            case EffectId.ColorRamp: PushEffect_ColorRamp(mat); return;
            case EffectId.FakeLighting: PushEffect_FakeLighting(mat); return;
            case EffectId.UVBendPoint: PushEffect_UVBendPoint(mat); return;
            case EffectId.BurnDirection: PushEffect_BurnDirection(mat); return;
            case EffectId.GlobalSmoke: PushEffect_GlobalSmoke(mat); return;
            case EffectId.HolographicCard: PushEffect_HolographicCard(mat); return;
            case EffectId.CloudShadows: PushEffect_CloudShadows(mat); return;
            case EffectId.ColorfulTrailA: PushEffect_ColorfulTrailA(mat); return;
            case EffectId.ColorfulTrailB: PushEffect_ColorfulTrailB(mat); return;
            case EffectId.ColorfulTrailC: PushEffect_ColorfulTrailC(mat); return;
            case EffectId.ChargingAbility: PushEffect_ChargingAbility(mat); return;
            case EffectId.Blueprint: PushEffect_Blueprint(mat); return;
            case EffectId.Ghosting: PushEffect_Ghosting(mat); return;
            case EffectId.RetroPalette: PushEffect_RetroPalette(mat); return;
        }
    }

    void PushEffect(MaterialPropertyBlock b, EffectDef def)
    {
        switch (def.id)
        {
            case EffectId.StrongTint: PushEffect_StrongTint(b); return;
            case EffectId.AddColor: PushEffect_AddColor(b); return;
            case EffectId.AlphaTint: PushEffect_AlphaTint(b); return;
            case EffectId.HitEffect: PushEffect_HitEffect(b); return;
            case EffectId.Negative: PushEffect_Negative(b); return;
            case EffectId.Greyscale: PushEffect_Greyscale(b); return;
            case EffectId.Bloom: PushEffect_Bloom(b); return;
            case EffectId.ColorReplace: PushEffect_ColorReplace(b); return;
            case EffectId.LUTReplace: PushEffect_LUTReplace(b); return;
            case EffectId.HueShift: PushEffect_HueShift(b); return;
            case EffectId.AddHue: PushEffect_AddHue(b); return;
            case EffectId.Saturation: PushEffect_Saturation(b); return;
            case EffectId.Brightness: PushEffect_Brightness(b); return;
            case EffectId.Contrast: PushEffect_Contrast(b); return;
            case EffectId.SplitToning: PushEffect_SplitToning(b); return;
            case EffectId.Posterize: PushEffect_Posterize(b); return;
            case EffectId.Poison: PushEffect_Poison(b); return;
            case EffectId.Frozen: PushEffect_Frozen(b); return;
            case EffectId.Metal: PushEffect_Metal(b); return;
            case EffectId.Camouflage: PushEffect_Camouflage(b); return;
            case EffectId.Rainbow: PushEffect_Rainbow(b); return;
            case EffectId.BlackTint: PushEffect_BlackTint(b); return;
            case EffectId.Chromatic: PushEffect_Chromatic(b); return;
            case EffectId.PaletteSwap: PushEffect_PaletteSwap(b); return;
            case EffectId.Dithering: PushEffect_Dithering(b); return;
            case EffectId.CRTScanlines: PushEffect_CRTScanlines(b); return;
            case EffectId.Vignette: PushEffect_Vignette(b); return;
            case EffectId.ColorGrading: PushEffect_ColorGrading(b); return;
            case EffectId.Sepia: PushEffect_Sepia(b); return;
            case EffectId.ThermalVision: PushEffect_ThermalVision(b); return;
            case EffectId.RecolorRGB: PushEffect_RecolorRGB(b); return;
            case EffectId.PingPongGlow: PushEffect_PingPongGlow(b); return;
            case EffectId.ColorCycle: PushEffect_ColorCycle(b); return;
            case EffectId.Outline: PushEffect_Outline(b); return;
            case EffectId.InnerOutline: PushEffect_InnerOutline(b); return;
            case EffectId.TexOutline: PushEffect_TexOutline(b); return;
            case EffectId.RainbowOutline: PushEffect_RainbowOutline(b); return;
            case EffectId.DistortOutline: PushEffect_DistortOutline(b); return;
            case EffectId.Glow: PushEffect_Glow(b); return;
            case EffectId.Shadow: PushEffect_Shadow(b); return;
            case EffectId.SoftShadow: PushEffect_SoftShadow(b); return;
            case EffectId.DirOutline: PushEffect_DirOutline(b); return;
            case EffectId.OutlineOnly: PushEffect_OutlineOnly(b); return;
            case EffectId.GradientOutline: PushEffect_GradientOutline(b); return;
            case EffectId.AnimOutlineThick: PushEffect_AnimOutlineThick(b); return;
            case EffectId.SelectionHighlight: PushEffect_SelectionHighlight(b); return;
            case EffectId.Pixelate: PushEffect_Pixelate(b); return;
            case EffectId.Wave: PushEffect_Wave(b); return;
            case EffectId.Twist: PushEffect_Twist(b); return;
            case EffectId.UVDistort: PushEffect_UVDistort(b); return;
            case EffectId.FullDistort: PushEffect_FullDistort(b); return;
            case EffectId.DirDistort: PushEffect_DirDistort(b); return;
            case EffectId.UVScroll: PushEffect_UVScroll(b); return;
            case EffectId.Rotate: PushEffect_Rotate(b); return;
            case EffectId.Squeeze: PushEffect_Squeeze(b); return;
            case EffectId.Squish: PushEffect_Squish(b); return;
            case EffectId.Wiggle: PushEffect_Wiggle(b); return;
            case EffectId.Wind: PushEffect_Wind(b); return;
            case EffectId.Sharpen: PushEffect_Sharpen(b); return;
            case EffectId.GaussBlur: PushEffect_GaussBlur(b); return;
            case EffectId.DirBlur: PushEffect_DirBlur(b); return;
            case EffectId.WorldTiling: PushEffect_WorldTiling(b); return;
            case EffectId.UVScaleOffset: PushEffect_UVScaleOffset(b); return;
            case EffectId.CurlNoise: PushEffect_CurlNoise(b); return;
            case EffectId.FBMNoise: PushEffect_FBMNoise(b); return;
            case EffectId.UVScrollOverlay: PushEffect_UVScrollOverlay(b); return;
            case EffectId.Shockwave: PushEffect_Shockwave(b); return;
            case EffectId.SineScale: PushEffect_SineScale(b); return;
            case EffectId.Heartbeat: PushEffect_Heartbeat(b); return;
            case EffectId.Breathing: PushEffect_Breathing(b); return;
            case EffectId.LiquidWobble: PushEffect_LiquidWobble(b); return;
            case EffectId.FlipBookBlend: PushEffect_FlipBookBlend(b); return;
            case EffectId.SmoothPixelArt: PushEffect_SmoothPixelArt(b); return;
            case EffectId.Dissolve: PushEffect_Dissolve(b); return;
            case EffectId.GlowDissolve: PushEffect_GlowDissolve(b); return;
            case EffectId.SrcDissolve: PushEffect_SrcDissolve(b); return;
            case EffectId.DirFade: PushEffect_DirFade(b); return;
            case EffectId.DirGlowFade: PushEffect_DirGlowFade(b); return;
            case EffectId.FullAlphaDissolve: PushEffect_FullAlphaDissolve(b); return;
            case EffectId.Halftone: PushEffect_Halftone(b); return;
            case EffectId.RadialWipe: PushEffect_RadialWipe(b); return;
            case EffectId.Blinds: PushEffect_Blinds(b); return;
            case EffectId.DiamondDissolve: PushEffect_DiamondDissolve(b); return;
            case EffectId.ScanlineDissolve: PushEffect_ScanlineDissolve(b); return;
            case EffectId.SpreadFade: PushEffect_SpreadFade(b); return;
            case EffectId.PixelDissolve: PushEffect_PixelDissolve(b); return;
            case EffectId.SpiralWipe: PushEffect_SpiralWipe(b); return;
            case EffectId.HexDissolve: PushEffect_HexDissolve(b); return;
            case EffectId.Hologram: PushEffect_Hologram(b); return;
            case EffectId.Glitch: PushEffect_Glitch(b); return;
            case EffectId.Flicker: PushEffect_Flicker(b); return;
            case EffectId.Shine: PushEffect_Shine(b); return;
            case EffectId.Smoke: PushEffect_Smoke(b); return;
            case EffectId.Flame: PushEffect_Flame(b); return;
            case EffectId.Enchanted: PushEffect_Enchanted(b); return;
            case EffectId.Shifting: PushEffect_Shifting(b); return;
            case EffectId.ShiftRainbow: PushEffect_ShiftRainbow(b); return;
            case EffectId.Overlay: PushEffect_Overlay(b); return;
            case EffectId.Checkerboard: PushEffect_Checkerboard(b); return;
            case EffectId.UVVisualize: PushEffect_UVVisualize(b); return;
            case EffectId.HealthTint: PushEffect_HealthTint(b); return;
            case EffectId.XRaySilhouette: PushEffect_XRaySilhouette(b); return;
            case EffectId.MinimapPing: PushEffect_MinimapPing(b); return;
            case EffectId.DialogueEmphasis: PushEffect_DialogueEmphasis(b); return;
            case EffectId.VoronoiCrackle: PushEffect_VoronoiCrackle(b); return;
            case EffectId.WaterReflect: PushEffect_WaterReflect(b); return;
            case EffectId.MotionSmear: PushEffect_MotionSmear(b); return;
            case EffectId.EnergyAura: PushEffect_EnergyAura(b); return;
            case EffectId.Gradient: PushEffect_Gradient(b); return;
            case EffectId.RadialGradient: PushEffect_RadialGradient(b); return;
            case EffectId.ColorRamp: PushEffect_ColorRamp(b); return;
            case EffectId.FakeLighting: PushEffect_FakeLighting(b); return;
            case EffectId.UVBendPoint: PushEffect_UVBendPoint(b); return;
            case EffectId.BurnDirection: PushEffect_BurnDirection(b); return;
            case EffectId.GlobalSmoke: PushEffect_GlobalSmoke(b); return;
            case EffectId.HolographicCard: PushEffect_HolographicCard(b); return;
            case EffectId.CloudShadows: PushEffect_CloudShadows(b); return;
            case EffectId.ColorfulTrailA: PushEffect_ColorfulTrailA(b); return;
            case EffectId.ColorfulTrailB: PushEffect_ColorfulTrailB(b); return;
            case EffectId.ColorfulTrailC: PushEffect_ColorfulTrailC(b); return;
            case EffectId.ChargingAbility: PushEffect_ChargingAbility(b); return;
            case EffectId.Blueprint: PushEffect_Blueprint(b); return;
            case EffectId.Ghosting: PushEffect_Ghosting(b); return;
            case EffectId.RetroPalette: PushEffect_RetroPalette(b); return;
        }
    }

    void MarkEnabledEffectsDirty()
    {
        if (strongTintEnabled) SetEffectDirtyBit(0);
        if (addColorEnabled) SetEffectDirtyBit(1);
        if (alphaTintEnabled) SetEffectDirtyBit(2);
        if (hitEffectEnabled) SetEffectDirtyBit(3);
        if (negativeEnabled) SetEffectDirtyBit(4);
        if (greyscaleEnabled) SetEffectDirtyBit(5);
        if (bloomEnabled) SetEffectDirtyBit(6);
        if (colorReplaceEnabled) SetEffectDirtyBit(7);
        if (lutReplaceEnabled) SetEffectDirtyBit(8);
        if (hueShiftEnabled) SetEffectDirtyBit(9);
        if (addHueEnabled) SetEffectDirtyBit(10);
        if (saturationEnabled) SetEffectDirtyBit(11);
        if (brightnessEnabled) SetEffectDirtyBit(12);
        if (contrastEnabled) SetEffectDirtyBit(13);
        if (splitToningEnabled) SetEffectDirtyBit(14);
        if (posterizeEnabled) SetEffectDirtyBit(15);
        if (poisonEnabled) SetEffectDirtyBit(16);
        if (frozenEnabled) SetEffectDirtyBit(17);
        if (metalEnabled) SetEffectDirtyBit(18);
        if (camouflageEnabled) SetEffectDirtyBit(19);
        if (rainbowEnabled) SetEffectDirtyBit(20);
        if (blackTintEnabled) SetEffectDirtyBit(21);
        if (chromaticEnabled) SetEffectDirtyBit(22);
        if (paletteSwapEnabled) SetEffectDirtyBit(23);
        if (ditheringEnabled) SetEffectDirtyBit(24);
        if (crtScanlinesEnabled) SetEffectDirtyBit(25);
        if (vignetteEnabled) SetEffectDirtyBit(26);
        if (colorGradingEnabled) SetEffectDirtyBit(27);
        if (sepiaEnabled) SetEffectDirtyBit(28);
        if (thermalVisionEnabled) SetEffectDirtyBit(29);
        if (recolorRGBEnabled) SetEffectDirtyBit(30);
        if (pingPongGlowEnabled) SetEffectDirtyBit(31);
        if (colorCycleEnabled) SetEffectDirtyBit(32);
        if (outlineEnabled) SetEffectDirtyBit(33);
        if (innerOutlineEnabled) SetEffectDirtyBit(34);
        if (texOutlineEnabled) SetEffectDirtyBit(35);
        if (rainbowOutlineEnabled) SetEffectDirtyBit(36);
        if (distortOutlineEnabled) SetEffectDirtyBit(37);
        if (glowEnabled) SetEffectDirtyBit(38);
        if (shadowEnabled) SetEffectDirtyBit(39);
        if (softShadowEnabled) SetEffectDirtyBit(40);
        if (dirOutlineEnabled) SetEffectDirtyBit(41);
        if (outlineOnlyEnabled) SetEffectDirtyBit(42);
        if (gradOutlineEnabled) SetEffectDirtyBit(43);
        if (animOutlineEnabled) SetEffectDirtyBit(44);
        if (selectHighlightEnabled) SetEffectDirtyBit(45);
        if (pixelateEnabled) SetEffectDirtyBit(46);
        if (waveEnabled) SetEffectDirtyBit(47);
        if (twistEnabled) SetEffectDirtyBit(48);
        if (uvDistortEnabled) SetEffectDirtyBit(49);
        if (fullDistortEnabled) SetEffectDirtyBit(50);
        if (dirDistortEnabled) SetEffectDirtyBit(51);
        if (uvScrollEnabled) SetEffectDirtyBit(52);
        if (rotateEnabled) SetEffectDirtyBit(53);
        if (squeezeEnabled) SetEffectDirtyBit(54);
        if (squishEnabled) SetEffectDirtyBit(55);
        if (wiggleEnabled) SetEffectDirtyBit(56);
        if (windEnabled) SetEffectDirtyBit(57);
        if (sharpenEnabled) SetEffectDirtyBit(58);
        if (gaussBlurEnabled) SetEffectDirtyBit(59);
        if (dirBlurEnabled) SetEffectDirtyBit(60);
        if (worldTilingEnabled) SetEffectDirtyBit(61);
        if (uvScaleOffsetEnabled) SetEffectDirtyBit(62);
        if (curlNoiseEnabled) SetEffectDirtyBit(63);
        if (fbmNoiseEnabled) SetEffectDirtyBit(64);
        if (uvScrollOverlayEnabled) SetEffectDirtyBit(65);
        if (shockwaveEnabled) SetEffectDirtyBit(66);
        if (sineScaleEnabled) SetEffectDirtyBit(67);
        if (heartbeatEnabled) SetEffectDirtyBit(68);
        if (breathingEnabled) SetEffectDirtyBit(69);
        if (liquidWobbleEnabled) SetEffectDirtyBit(70);
        if (flipBookBlendEnabled) SetEffectDirtyBit(71);
        if (smoothPixelArtEnabled) SetEffectDirtyBit(72);
        if (dissolveEnabled) SetEffectDirtyBit(73);
        if (glowDissolveEnabled) SetEffectDirtyBit(74);
        if (srcDissolveEnabled) SetEffectDirtyBit(75);
        if (dirFadeEnabled) SetEffectDirtyBit(76);
        if (dirGlowFadeEnabled) SetEffectDirtyBit(77);
        if (fullAlphaDissolveEnabled) SetEffectDirtyBit(78);
        if (halftoneEnabled) SetEffectDirtyBit(79);
        if (radialWipeEnabled) SetEffectDirtyBit(80);
        if (blindsEnabled) SetEffectDirtyBit(81);
        if (diamondDissolveEnabled) SetEffectDirtyBit(82);
        if (scanlineDissolveEnabled) SetEffectDirtyBit(83);
        if (spreadFadeEnabled) SetEffectDirtyBit(84);
        if (pixelDissolveEnabled) SetEffectDirtyBit(85);
        if (spiralWipeEnabled) SetEffectDirtyBit(86);
        if (hexDissolveEnabled) SetEffectDirtyBit(87);
        if (hologramEnabled) SetEffectDirtyBit(88);
        if (glitchEnabled) SetEffectDirtyBit(89);
        if (flickerEnabled) SetEffectDirtyBit(90);
        if (shineEnabled) SetEffectDirtyBit(91);
        if (smokeEnabled) SetEffectDirtyBit(92);
        if (flameEnabled) SetEffectDirtyBit(93);
        if (enchantedEnabled) SetEffectDirtyBit(94);
        if (shiftingEnabled) SetEffectDirtyBit(95);
        if (shiftRainbowEnabled) SetEffectDirtyBit(96);
        if (overlayEnabled) SetEffectDirtyBit(97);
        if (checkerboardEnabled) SetEffectDirtyBit(98);
        if (uvVisualizeEnabled) SetEffectDirtyBit(99);
        if (healthTintEnabled) SetEffectDirtyBit(100);
        if (xraySilhouetteEnabled) SetEffectDirtyBit(101);
        if (minimapPingEnabled) SetEffectDirtyBit(102);
        if (dialogueEmphasisEnabled) SetEffectDirtyBit(103);
        if (voronoiCrackleEnabled) SetEffectDirtyBit(104);
        if (waterReflectEnabled) SetEffectDirtyBit(105);
        if (motionSmearEnabled) SetEffectDirtyBit(106);
        if (energyAuraEnabled) SetEffectDirtyBit(107);
        if (gradientEnabled) SetEffectDirtyBit(108);
        if (radialGradientEnabled) SetEffectDirtyBit(109);
        if (colorRampEnabled) SetEffectDirtyBit(110);
        if (fakeLightingEnabled) SetEffectDirtyBit(111);
        if (uvBendPointEnabled) SetEffectDirtyBit(112);
        if (burnDirectionEnabled) SetEffectDirtyBit(113);
        if (globalSmokeEnabled) SetEffectDirtyBit(114);
        if (holographicCardEnabled) SetEffectDirtyBit(115);
        if (cloudShadowsEnabled) SetEffectDirtyBit(116);
        if (colorfulTrailAEnabled) SetEffectDirtyBit(117);
        if (colorfulTrailBEnabled) SetEffectDirtyBit(118);
        if (colorfulTrailCEnabled) SetEffectDirtyBit(119);
        if (chargingAbilityEnabled) SetEffectDirtyBit(120);
        if (blueprintEnabled) SetEffectDirtyBit(121);
        if (ghostingEnabled) SetEffectDirtyBit(122);
        if (retroPaletteEnabled) SetEffectDirtyBit(123);
    }

    // ---- Snapshot fields ----
    bool _prev_strongTintEnabled;
    int _prev_strongTintMaskCh;
    Color _prev_strongTintColor;
    float _prev_strongTintFade;
    bool _prev_addColorEnabled;
    int _prev_addColorMaskCh;
    Color _prev_addColorAmount;
    float _prev_addColorFade;
    bool _prev_alphaTintEnabled;
    int _prev_alphaTintMaskCh;
    Color _prev_alphaTintColor;
    float _prev_alphaTintFade;
    bool _prev_hitEffectEnabled;
    int _prev_hitEffectMaskCh;
    Color _prev_hitEffectColor;
    float _prev_hitEffectBlend;
    bool _prev_negativeEnabled;
    int _prev_negativeMaskCh;
    float _prev_negativeAmount;
    bool _prev_greyscaleEnabled;
    int _prev_greyscaleMaskCh;
    float _prev_greyscaleBlend;
    Vector4 _prev_greyscaleLuminosity;
    bool _prev_bloomEnabled;
    int _prev_bloomMaskCh;
    int _prev_bloomMode;
    Color _prev_bloomColor;
    float _prev_bloomThreshold;
    float _prev_bloomIntensity;
    float _prev_bloomSize;
    int _prev_bloomSamples;
    float _prev_bloomSoftKnee;
    Color _prev_bloomMaskColor;
    float _prev_bloomMaskRange;
    float _prev_bloomMaskSoft;
    float _prev_bloomAlphaBleed;
    bool _prev_colorReplaceEnabled;
    int _prev_colorReplaceMaskCh;
    Color _prev_colorReplaceSrc1;
    Color _prev_colorReplaceDst1;
    Color _prev_colorReplaceSrc2;
    Color _prev_colorReplaceDst2;
    Color _prev_colorReplaceSrc3;
    Color _prev_colorReplaceDst3;
    Color _prev_colorReplaceSrc4;
    Color _prev_colorReplaceDst4;
    int _prev_colorReplaceCount;
    float _prev_colorReplaceRange;
    float _prev_colorReplaceFade;
    bool _prev_lutReplaceEnabled;
    int _prev_lutReplaceMaskCh;
    Texture2D _prev_lutReplaceTexture;
    float _prev_lutSize;
    float _prev_lutReplaceFade;
    bool _prev_hueShiftEnabled;
    int _prev_hueShiftMaskCh;
    float _prev_hueShiftAmount;
    float _prev_hueShiftSpeed;
    float _prev_hueShiftFade;
    bool _prev_addHueEnabled;
    int _prev_addHueMaskCh;
    float _prev_addHueAmount;
    float _prev_addHueSpeed;
    float _prev_addHueSaturation;
    float _prev_addHueBrightness;
    float _prev_addHueFade;
    bool _prev_saturationEnabled;
    int _prev_saturationMaskCh;
    float _prev_saturationAmount;
    bool _prev_brightnessEnabled;
    int _prev_brightnessMaskCh;
    float _prev_brightnessAmount;
    bool _prev_contrastEnabled;
    int _prev_contrastMaskCh;
    float _prev_contrastAmount;
    bool _prev_splitToningEnabled;
    int _prev_splitToningMaskCh;
    Color _prev_splitShadowColor;
    Color _prev_splitHighlightColor;
    float _prev_splitBalance;
    Color _prev_splitMidColor;
    float _prev_splitFade;
    bool _prev_posterizeEnabled;
    int _prev_posterizeMaskCh;
    float _prev_posterizeSteps;
    float _prev_posterizeFade;
    bool _prev_poisonEnabled;
    int _prev_poisonMaskCh;
    Color _prev_poisonColor;
    float _prev_poisonSpeed;
    float _prev_poisonDensity;
    Color _prev_poisonColor2;
    Color _prev_poisonColor3;
    float _prev_poisonFade;
    bool _prev_frozenEnabled;
    int _prev_frozenMaskCh;
    Color _prev_frozenColor;
    float _prev_frozenContrast;
    float _prev_frozenHighlight;
    Color _prev_frozenColor2;
    Color _prev_frozenHighlightColor;
    float _prev_frozenFade;
    bool _prev_metalEnabled;
    int _prev_metalMaskCh;
    Color _prev_metalColor;
    Color _prev_metalHighlight;
    float _prev_metalContrast;
    float _prev_metalSpeed;
    Color _prev_metalColor2;
    float _prev_metalFade;
    bool _prev_camouflageEnabled;
    int _prev_camouflageMaskCh;
    Color _prev_camoColor1;
    Color _prev_camoColor2;
    Color _prev_camoColor3;
    float _prev_camoScale;
    float _prev_camoSpeed;
    float _prev_camoFade;
    bool _prev_rainbowEnabled;
    int _prev_rainbowMaskCh;
    float _prev_rainbowSpeed;
    float _prev_rainbowSaturation;
    float _prev_rainbowBrightness;
    float _prev_rainbowFade;
    float _prev_rainbowDensity;
    bool _prev_blackTintEnabled;
    int _prev_blackTintMaskCh;
    Color _prev_blackTintColor;
    float _prev_blackTintThreshold;
    float _prev_blackTintFade;
    bool _prev_chromaticEnabled;
    int _prev_chromaticMaskCh;
    float _prev_chromaticAmount;
    float _prev_chromaticAngle;
    bool _prev_paletteSwapEnabled;
    int _prev_paletteSwapMaskCh;
    Texture2D _prev_paletteSwapTexture;
    int _prev_paletteSwapColors;
    float _prev_paletteSwapFade;
    bool _prev_ditheringEnabled;
    int _prev_ditheringMaskCh;
    float _prev_ditheringSize;
    float _prev_ditheringLevels;
    float _prev_ditheringFade;
    int _prev_ditheringAlphaMode;
    float _prev_ditheringAlphaThreshold;
    bool _prev_ditheringScreenSpace;
    bool _prev_crtScanlinesEnabled;
    int _prev_crtScanlinesMaskCh;
    float _prev_crtDensity;
    float _prev_crtIntensity;
    float _prev_crtSpeed;
    float _prev_crtCurvature;
    bool _prev_vignetteEnabled;
    int _prev_vignetteMaskCh;
    Color _prev_vignetteColor;
    float _prev_vignetteIntensity;
    float _prev_vignetteSmoothness;
    float _prev_vignetteAlpha;
    Vector4 _prev_vignetteCenter;
    Vector2 _prev_vignetteOffset;
    bool _prev_colorGradingEnabled;
    int _prev_colorGradingMaskCh;
    float _prev_cgExposure;
    float _prev_cgGamma;
    Vector4 _prev_cgLift;
    bool _prev_sepiaEnabled;
    int _prev_sepiaMaskCh;
    float _prev_sepiaIntensity;
    Color _prev_sepiaColor;
    bool _prev_thermalVisionEnabled;
    int _prev_thermalVisionMaskCh;
    float _prev_thermalIntensity;
    float _prev_thermalHDRGlow;
    bool _prev_recolorRGBEnabled;
    int _prev_recolorRGBMaskCh;
    Color _prev_recolorR;
    Color _prev_recolorG;
    Color _prev_recolorB;
    Color _prev_recolorCyan;
    Color _prev_recolorMagenta;
    Color _prev_recolorYellow;
    float _prev_recolorFade;
    bool _prev_pingPongGlowEnabled;
    int _prev_pingPongGlowMaskCh;
    Color _prev_pingPongGlowColor;
    float _prev_pingPongGlowSpeed;
    float _prev_pingPongGlowMin;
    float _prev_pingPongGlowMax;
    bool _prev_colorCycleEnabled;
    int _prev_colorCycleMaskCh;
    Color _prev_colorCycleA;
    Color _prev_colorCycleB;
    Color _prev_colorCycleC;
    Color _prev_colorCycleD;
    float _prev_colorCycleSpeed;
    float _prev_colorCycleFade;
    bool _prev_outlineEnabled;
    int _prev_outlineMaskCh;
    Color _prev_outlineColor;
    float _prev_outlineSize;
    float _prev_outlineAlpha;
    bool _prev_innerOutlineEnabled;
    int _prev_innerOutlineMaskCh;
    Color _prev_innerOutlineColor;
    float _prev_innerOutlineThickness;
    float _prev_innerOutlineAlpha;
    bool _prev_texOutlineEnabled;
    int _prev_texOutlineMaskCh;
    Texture2D _prev_texOutlineTexture;
    Color _prev_texOutlineColor;
    float _prev_texOutlineSize;
    Vector4 _prev_texOutlineTiling;
    Vector4 _prev_texOutlineSpeed;
    bool _prev_rainbowOutlineEnabled;
    int _prev_rainbowOutlineMaskCh;
    Color _prev_rainbowOutlineColor;
    float _prev_rainbowOutlineSize;
    float _prev_rainbowOutlineSpeed;
    float _prev_rainbowOutlineDensity;
    bool _prev_distortOutlineEnabled;
    int _prev_distortOutlineMaskCh;
    Color _prev_distortOutlineColor;
    float _prev_distortOutlineSize;
    float _prev_distortOutlineAmount;
    float _prev_distortOutlineSpeed;
    bool _prev_glowEnabled;
    int _prev_glowMaskCh;
    Color _prev_glowColor;
    float _prev_glowSize;
    float _prev_glowIntensity;
    bool _prev_shadowEnabled;
    int _prev_shadowMaskCh;
    Color _prev_shadowColor;
    Vector4 _prev_shadowOffset;
    bool _prev_softShadowEnabled;
    int _prev_softShadowMaskCh;
    Color _prev_softShadowColor;
    Vector4 _prev_softShadowOffset;
    float _prev_softShadowSoftness;
    int _prev_softShadowSamples;
    bool _prev_dirOutlineEnabled;
    int _prev_dirOutlineMaskCh;
    Color _prev_dirOutlineColor;
    float _prev_dirOutlineWidth;
    float _prev_dirOutlineIntensity;
    Vector2 _prev_dirOutlineDirection;
    float _prev_dirOutlineSoftness;
    bool _prev_outlineOnlyEnabled;
    int _prev_outlineOnlyMaskCh;
    Color _prev_outlineOnlyColor;
    float _prev_outlineOnlyWidth;
    bool _prev_gradOutlineEnabled;
    int _prev_gradOutlineMaskCh;
    Color _prev_gradOutlineColorA;
    Color _prev_gradOutlineColorB;
    float _prev_gradOutlineAngle;
    float _prev_gradOutlineSize;
    bool _prev_animOutlineEnabled;
    int _prev_animOutlineMaskCh;
    Color _prev_animOutlineColor;
    float _prev_animOutlineMin;
    float _prev_animOutlineMax;
    float _prev_animOutlineSpeed;
    bool _prev_selectHighlightEnabled;
    int _prev_selectHighlightMaskCh;
    Color _prev_selectHighlightColor;
    float _prev_selectHighlightSize;
    float _prev_selectHighlightDash;
    float _prev_selectHighlightSpeed;
    bool _prev_pixelateEnabled;
    int _prev_pixelateMaskCh;
    float _prev_pixelateSize;
    bool _prev_waveEnabled;
    int _prev_waveMaskCh;
    float _prev_waveSpeed;
    float _prev_waveAmplitude;
    float _prev_waveFrequency;
    bool _prev_twistEnabled;
    int _prev_twistMaskCh;
    float _prev_twistAmount;
    float _prev_twistSpeed;
    Vector2 _prev_twistOffset;
    float _prev_twistRadius;
    bool _prev_uvDistortEnabled;
    int _prev_uvDistortMaskCh;
    Texture2D _prev_distortTexture;
    float _prev_distortAmount;
    Vector4 _prev_distortTiling;
    float _prev_distortSpeed;
    int _prev_distortIsNormalMap;
    bool _prev_fullDistortEnabled;
    int _prev_fullDistortMaskCh;
    float _prev_fullDistortAmount;
    float _prev_fullDistortSpeed;
    float _prev_fullDistortDensity;
    bool _prev_dirDistortEnabled;
    int _prev_dirDistortMaskCh;
    float _prev_dirDistortAngle;
    float _prev_dirDistortAmount;
    float _prev_dirDistortSpeed;
    bool _prev_uvScrollEnabled;
    int _prev_uvScrollMaskCh;
    float _prev_uvScrollSpeedX;
    float _prev_uvScrollSpeedY;
    bool _prev_rotateEnabled;
    int _prev_rotateMaskCh;
    float _prev_rotateSpeed;
    float _prev_rotateAmount;
    bool _prev_squeezeEnabled;
    int _prev_squeezeMaskCh;
    Vector4 _prev_squeezeAmount;
    float _prev_squeezeSpeed;
    float _prev_squeezePower;
    bool _prev_squishEnabled;
    int _prev_squishMaskCh;
    Vector4 _prev_squishAmount;
    float _prev_squishSpeed;
    float _prev_squishPower;
    bool _prev_wiggleEnabled;
    int _prev_wiggleMaskCh;
    float _prev_wiggleSpeed;
    float _prev_wiggleAmount;
    float _prev_wiggleFrequency;
    bool _prev_windEnabled;
    int _prev_windMaskCh;
    float _prev_windSpeed;
    float _prev_windAmount;
    float _prev_windBottomY;
    float _prev_windFrequency;
    bool _prev_sharpenEnabled;
    int _prev_sharpenMaskCh;
    float _prev_sharpenAmount;
    float _prev_sharpenSize;
    bool _prev_gaussBlurEnabled;
    int _prev_gaussBlurMaskCh;
    float _prev_gaussBlurSize;
    float _prev_gaussBlurSamples;
    bool _prev_dirBlurEnabled;
    int _prev_dirBlurMaskCh;
    float _prev_dirBlurAngle;
    float _prev_dirBlurSize;
    float _prev_dirBlurSamples;
    bool _prev_worldTilingEnabled;
    int _prev_worldTilingMaskCh;
    float _prev_worldTileScale;
    Vector2 _prev_worldTileOffset;
    float _prev_worldTileRotation;
    bool _prev_uvScaleOffsetEnabled;
    int _prev_uvScaleOffsetMaskCh;
    Vector4 _prev_uvScaleOffset;
    bool _prev_curlNoiseEnabled;
    int _prev_curlNoiseMaskCh;
    float _prev_curlNoiseAmount;
    float _prev_curlNoiseSpeed;
    float _prev_curlNoiseDensity;
    bool _prev_fbmNoiseEnabled;
    int _prev_fbmNoiseMaskCh;
    float _prev_fbmNoiseAmount;
    float _prev_fbmNoiseSpeed;
    float _prev_fbmNoiseDensity;
    int _prev_fbmNoiseOctaves;
    bool _prev_uvScrollOverlayEnabled;
    int _prev_uvScrollOverlayMaskCh;
    Texture2D _prev_uvScrollOverlayTex;
    float _prev_uvScrollOverlaySpeedX;
    float _prev_uvScrollOverlaySpeedY;
    float _prev_uvScrollOverlayBlend;
    Color _prev_uvScrollOverlayTint;
    Vector4 _prev_uvScrollOverlayTiling;
    bool _prev_shockwaveEnabled;
    int _prev_shockwaveMaskCh;
    Vector4 _prev_shockwaveCenter;
    float _prev_shockwaveRadius;
    float _prev_shockwaveWidth;
    float _prev_shockwaveAmplitude;
    bool _prev_sineScaleEnabled;
    int _prev_sineScaleMaskCh;
    float _prev_sineScaleAmountX;
    float _prev_sineScaleAmountY;
    float _prev_sineScaleSpeed;
    bool _prev_heartbeatEnabled;
    int _prev_heartbeatMaskCh;
    float _prev_heartbeatSpeed;
    float _prev_heartbeatAmount;
    bool _prev_breathingEnabled;
    int _prev_breathingMaskCh;
    float _prev_breathingSpeed;
    float _prev_breathingAmount;
    bool _prev_liquidWobbleEnabled;
    int _prev_liquidWobbleMaskCh;
    float _prev_liquidWobbleSpeed;
    float _prev_liquidWobbleAmount;
    float _prev_liquidWobbleFreq;
    bool _prev_flipBookBlendEnabled;
    int _prev_flipBookBlendMaskCh;
    Texture2D _prev_flipBookNextFrame;
    float _prev_flipBookLerp;
    bool _prev_smoothPixelArtEnabled;
    int _prev_smoothPixelArtMaskCh;
    float _prev_smoothPixelArtTexelSize;
    bool _prev_dissolveEnabled;
    int _prev_dissolveMaskCh;
    int _prev_dissolveNoiseType;
    Texture2D _prev_dissolveTexture;
    float _prev_dissolveNoiseScale;
    float _prev_dissolveAmount;
    float _prev_dissolveBorderWidth;
    float _prev_dissolveBurnWidth;
    Color _prev_dissolveBurnColor;
    Vector4 _prev_dissolveTiling;
    Color _prev_dissolveBorderColor;
    bool _prev_glowDissolveEnabled;
    int _prev_glowDissolveMaskCh;
    int _prev_glowDissolveNoiseType;
    Texture2D _prev_glowDissolveTexture;
    float _prev_glowDissolveNoiseScale;
    float _prev_glowDissolveAmount;
    Color _prev_glowDissolveColor;
    float _prev_glowDissolveWidth;
    bool _prev_srcDissolveEnabled;
    int _prev_srcDissolveMaskCh;
    float _prev_srcDissolveAmount;
    Vector4 _prev_srcDissolvePosition;
    Color _prev_srcDissolveBorderColor;
    float _prev_srcDissolveBorderWidth;
    bool _prev_dirFadeEnabled;
    int _prev_dirFadeMaskCh;
    float _prev_dirFadeAngle;
    float _prev_dirFadeAmount;
    float _prev_dirFadeSmoothness;
    bool _prev_dirGlowFadeEnabled;
    int _prev_dirGlowFadeMaskCh;
    float _prev_dirGlowFadeAngle;
    float _prev_dirGlowFadeAmount;
    Color _prev_dirGlowFadeColor;
    float _prev_dirGlowFadeWidth;
    bool _prev_fullAlphaDissolveEnabled;
    int _prev_fullAlphaDissolveMaskCh;
    float _prev_fullAlphaDissolveAmount;
    bool _prev_halftoneEnabled;
    int _prev_halftoneMaskCh;
    float _prev_halftoneSize;
    float _prev_halftoneAmount;
    float _prev_halftoneSmoothness;
    bool _prev_radialWipeEnabled;
    int _prev_radialWipeMaskCh;
    float _prev_radialWipeAmount;
    float _prev_radialWipeSmoothness;
    float _prev_radialWipeAngle;
    bool _prev_blindsEnabled;
    int _prev_blindsMaskCh;
    float _prev_blindsAmount;
    float _prev_blindsCount;
    int _prev_blindsDirection;
    bool _prev_diamondDissolveEnabled;
    int _prev_diamondDissolveMaskCh;
    float _prev_diamondDissolveAmount;
    float _prev_diamondDissolveScale;
    bool _prev_scanlineDissolveEnabled;
    int _prev_scanlineDissolveMaskCh;
    float _prev_scanlineDissolveAmount;
    float _prev_scanlineDissolveDensity;
    bool _prev_spreadFadeEnabled;
    int _prev_spreadFadeMaskCh;
    float _prev_spreadFadeAmount;
    Vector4 _prev_spreadFadeOrigin;
    float _prev_spreadFadeSmoothness;
    bool _prev_pixelDissolveEnabled;
    int _prev_pixelDissolveMaskCh;
    float _prev_pixelDissolveAmount;
    float _prev_pixelDissolveGrid;
    bool _prev_spiralWipeEnabled;
    int _prev_spiralWipeMaskCh;
    float _prev_spiralWipeAmount;
    float _prev_spiralWipeSmoothness;
    float _prev_spiralWipeArms;
    bool _prev_hexDissolveEnabled;
    int _prev_hexDissolveMaskCh;
    float _prev_hexDissolveAmount;
    float _prev_hexDissolveScale;
    bool _prev_hologramEnabled;
    int _prev_hologramMaskCh;
    float _prev_hologramStripesAmount;
    float _prev_hologramMinAlpha;
    float _prev_hologramStripesSpeed;
    Color _prev_hologramColor;
    float _prev_hologramBlend;
    float _prev_hologramDistortion;
    bool _prev_glitchEnabled;
    int _prev_glitchMaskCh;
    float _prev_glitchAmount;
    float _prev_glitchSpeed;
    float _prev_glitchSize;
    float _prev_glitchColorShift;
    bool _prev_flickerEnabled;
    int _prev_flickerMaskCh;
    float _prev_flickerFreq;
    float _prev_flickerPercent;
    bool _prev_shineEnabled;
    int _prev_shineMaskCh;
    Color _prev_shineColor;
    float _prev_shineWidth;
    float _prev_shineSpeed;
    float _prev_shineDelay;
    float _prev_shineRotation;
    float _prev_shineGlow;
    bool _prev_smokeEnabled;
    int _prev_smokeMaskCh;
    Color _prev_smokeColor;
    float _prev_smokeDensity;
    float _prev_smokeSpeed;
    float _prev_smokeTurbulence;
    Vector4 _prev_smokeDirection;
    float _prev_smokeColorStrength;
    float _prev_smokeFade;
    bool _prev_flameEnabled;
    int _prev_flameMaskCh;
    Color _prev_flameColor1;
    Color _prev_flameColor2;
    float _prev_flameSpeed;
    float _prev_flameDensity;
    int _prev_flameOnlyMode;
    float _prev_flameExtendSides;
    float _prev_flameSideFalloff;
    float _prev_flameIntensity;
    bool _prev_enchantedEnabled;
    int _prev_enchantedMaskCh;
    Color _prev_enchantedColor;
    float _prev_enchantedSpeed;
    float _prev_enchantedDensity;
    float _prev_enchantedBrightness;
    float _prev_enchantedFade;
    bool _prev_shiftingEnabled;
    int _prev_shiftingMaskCh;
    Color _prev_shiftingColor1;
    Color _prev_shiftingColor2;
    float _prev_shiftingSpeed;
    float _prev_shiftingFade;
    bool _prev_shiftRainbowEnabled;
    int _prev_shiftRainbowMaskCh;
    float _prev_shiftRainbowSpeed;
    float _prev_shiftRainbowDensity;
    float _prev_shiftRainbowFade;
    bool _prev_overlayEnabled;
    int _prev_overlayMaskCh;
    Texture2D _prev_overlayTexture;
    Color _prev_overlayColor;
    float _prev_overlayBlend;
    Vector4 _prev_overlayTiling;
    Vector4 _prev_overlaySpeed;
    bool _prev_checkerboardEnabled;
    int _prev_checkerboardMaskCh;
    int _prev_checkerCoordSpace;
    float _prev_checkerScale;
    Color _prev_checkerColor1;
    Color _prev_checkerColor2;
    bool _prev_uvVisualizeEnabled;
    int _prev_uvVisualizeMaskCh;
    float _prev_uvVisualizeBlend;
    bool _prev_healthTintEnabled;
    int _prev_healthTintMaskCh;
    Color _prev_healthTintLowColor;
    Color _prev_healthTintHighColor;
    float _prev_healthTintAmount;
    float _prev_healthDamageNoise;
    float _prev_healthEdgeGrain;
    float _prev_healthPulseSpeed;
    float _prev_healthVignetteStr;
    float _prev_healthDesaturation;
    bool _prev_xraySilhouetteEnabled;
    int _prev_xraySilhouetteMaskCh;
    Color _prev_xraySilhouetteColor;
    float _prev_xraySilhouetteBlend;
    bool _prev_minimapPingEnabled;
    int _prev_minimapPingMaskCh;
    Color _prev_minimapPingColor;
    float _prev_minimapPingSpeed;
    float _prev_minimapPingWidth;
    Vector2 _prev_minimapPingOffset;
    bool _prev_dialogueEmphasisEnabled;
    int _prev_dialogueEmphasisMaskCh;
    float _prev_dialogueBobSpeed;
    float _prev_dialogueBobAmount;
    float _prev_dialogueBrightness;
    bool _prev_voronoiCrackleEnabled;
    int _prev_crackleMaskCh;
    float _prev_crackleScale;
    float _prev_crackleSpeed;
    float _prev_crackleEdgeWidth;
    Color _prev_crackleEdgeColor;
    float _prev_crackleCellTint;
    float _prev_crackleFade;
    bool _prev_waterReflectEnabled;
    int _prev_waterReflectMaskCh;
    Color _prev_waterReflectTint;
    float _prev_waterReflectYPoint;
    float _prev_waterReflectDepth;
    float _prev_waterReflectOpacity;
    float _prev_waterReflectTintAmount;
    float _prev_waterReflectWaveFreq;
    float _prev_waterReflectWaveSpeed;
    float _prev_waterReflectWaveAmp;
    float _prev_waterReflectFade;
    bool _prev_motionSmearEnabled;
    int _prev_smearMaskCh;
    Vector4 _prev_smearDirection;
    float _prev_smearAmount;
    float _prev_smearSamples;
    float _prev_smearFade;
    bool _prev_energyAuraEnabled;
    int _prev_auraMaskCh;
    Color _prev_auraColor;
    float _prev_auraRadius;
    float _prev_auraFrequency;
    float _prev_auraSpeed;
    float _prev_auraIntensity;
    float _prev_auraFade;
    bool _prev_gradientEnabled;
    int _prev_gradientMaskCh;
    Color _prev_gradientColorA;
    Color _prev_gradientColorB;
    float _prev_gradientAngle;
    float _prev_gradientFade;
    bool _prev_gradientUseGradientEditor;
    Texture2D _prev_gradientBakedTexture;
    int _prev_gradientBlendMode;
    bool _prev_radialGradientEnabled;
    int _prev_radialGradMaskCh;
    Color _prev_radialGradCenterColor;
    Color _prev_radialGradEdgeColor;
    Vector4 _prev_radialGradCenter;
    float _prev_radialGradRadius;
    float _prev_radialGradFade;
    bool _prev_radialGradUseGradientEditor;
    Texture2D _prev_radialGradBakedTexture;
    int _prev_radialGradBlendMode;
    bool _prev_colorRampEnabled;
    int _prev_colorRampMaskCh;
    Texture2D _prev_colorRampTexture;
    int _prev_colorRampMode;
    float _prev_colorRampIntensity;
    Color _prev_colorRampColorA;
    Color _prev_colorRampColorB;
    float _prev_colorRampFade;
    bool _prev_fakeLightingEnabled;
    int _prev_fakeLightMaskCh;
    Vector4 _prev_fakeLightDirection;
    Color _prev_fakeLightColor;
    float _prev_fakeLightIntensity;
    float _prev_fakeLightAmbient;
    float _prev_fakeLightNormalDist;
    float _prev_fakeLightNormalZ;
    float _prev_fakeLightSpecPower;
    float _prev_fakeLightSpecIntensity;
    float _prev_fakeLightFade;
    bool _prev_uvBendPointEnabled;
    int _prev_bendMaskCh;
    Vector4 _prev_bendLocalPoint;
    Vector4 _prev_bendWorldPoint;
    bool _prev_bendUseWorldSpace;
    float _prev_bendWorldScale;
    float _prev_bendRadius;
    float _prev_bendAmount;
    float _prev_bendFalloff;
    bool _prev_bendInvert;
    bool _prev_burnDirectionEnabled;
    int _prev_burnDirMaskCh;
    float _prev_burnDirAngle;
    float _prev_burnDirAmount;
    float _prev_burnDirEdgeWidth;
    Color _prev_burnDirFireColor;
    Color _prev_burnDirCharcoalColor;
    float _prev_burnDirCharcoalWidth;
    float _prev_burnDirNoiseScale;
    bool _prev_globalSmokeEnabled;
    int _prev_globalSmokeMaskCh;
    Color _prev_globalSmokeColor;
    float _prev_globalSmokeDensity;
    float _prev_globalSmokeSpeed;
    float _prev_globalSmokeOpacity;
    Vector4 _prev_globalSmokeDirection;
    bool _prev_globalSmokeOverlayOnly;
    bool _prev_holographicCardEnabled;
    int _prev_holoCardMaskCh;
    float _prev_holoCardDensity;
    float _prev_holoCardSpeed;
    float _prev_holoCardSaturation;
    float _prev_holoCardBrightness;
    float _prev_holoCardFresnelPower;
    float _prev_holoCardFresnelShift;
    float _prev_holoCardScanDensity;
    float _prev_holoCardScanSpeed;
    float _prev_holoCardScanIntensity;
    float _prev_holoCardFade;
    bool _prev_cloudShadowsEnabled;
    int _prev_cloudShadowMaskCh;
    float _prev_cloudShadowScale;
    Vector4 _prev_cloudShadowSpeed;
    float _prev_cloudShadowDarkness;
    float _prev_cloudShadowThreshold;
    float _prev_cloudShadowSoftness;
    float _prev_cloudShadowFade;
    bool _prev_cloudShadowOverlayOnly;
    bool _prev_colorfulTrailAEnabled;
    int _prev_trailAMaskCh;
    Vector4 _prev_trailAVelocity;
    Color _prev_trailASlowColor;
    Color _prev_trailAFastColor;
    float _prev_trailAMaxSpeed;
    float _prev_trailACurvePower;
    float _prev_trailAIntensity;
    bool _prev_colorfulTrailBEnabled;
    int _prev_trailBMaskCh;
    Vector4 _prev_trailBVelocity;
    Color _prev_trailBColor;
    float _prev_trailBMaxSpeed;
    float _prev_trailBCurvePower;
    float _prev_trailBIntensity;
    int _prev_trailBSegments;
    float _prev_trailBFadeOut;
    bool _prev_colorfulTrailCEnabled;
    int _prev_trailCMaskCh;
    Vector4 _prev_trailCVelocity;
    Color _prev_trailCColorA;
    Color _prev_trailCColorB;
    float _prev_trailCMaxSpeed;
    float _prev_trailCCurvePower;
    float _prev_trailCIntensity;
    float _prev_trailCRainbowSpeed;
    bool _prev_chargingAbilityEnabled;
    int _prev_chargeMaskCh;
    Color _prev_chargeColor;
    Vector4 _prev_chargeCenter;
    float _prev_chargeRadius;
    float _prev_chargeWidth;
    float _prev_chargeSpeed;
    float _prev_chargeFrequency;
    float _prev_chargeDensity;
    float _prev_chargeIntensity;
    float _prev_chargeLines;
    float _prev_chargeFade;
    bool _prev_blueprintEnabled;
    int _prev_blueprintMaskCh;
    Color _prev_blueprintBgColor;
    Color _prev_blueprintLineColor;
    Color _prev_blueprintGridColor;
    float _prev_blueprintEdgeSize;
    float _prev_blueprintLineWidth;
    float _prev_blueprintLineIntensity;
    float _prev_blueprintGridDensity;
    float _prev_blueprintGridOpacity;
    float _prev_blueprintFade;
    bool _prev_ghostingEnabled;
    int _prev_ghostingMaskCh;
    int _prev_ghostingCount;
    Vector4 _prev_ghostingOffset;
    float _prev_ghostingFade;
    bool _prev_retroPaletteEnabled;
    int _prev_retroMaskCh;
    float _prev_retroPixelSize;
    float _prev_retroPaletteSize;
    Texture2D _prev_retroPaletteTex;
    float _prev_retroDitherStrength;
    float _prev_retroFade;

    bool CheckEffect_StrongTint()
    {
        bool changed = false;
        if (strongTintMaskCh != _prev_strongTintMaskCh) { _prev_strongTintMaskCh = strongTintMaskCh; changed = true; }
        if (strongTintColor != _prev_strongTintColor) { _prev_strongTintColor = strongTintColor; changed = true; }
        if (strongTintFade != _prev_strongTintFade) { _prev_strongTintFade = strongTintFade; changed = true; }
        return changed;
    }

    bool CheckEffect_AddColor()
    {
        bool changed = false;
        if (addColorMaskCh != _prev_addColorMaskCh) { _prev_addColorMaskCh = addColorMaskCh; changed = true; }
        if (addColorAmount != _prev_addColorAmount) { _prev_addColorAmount = addColorAmount; changed = true; }
        if (addColorFade != _prev_addColorFade) { _prev_addColorFade = addColorFade; changed = true; }
        return changed;
    }

    bool CheckEffect_AlphaTint()
    {
        bool changed = false;
        if (alphaTintMaskCh != _prev_alphaTintMaskCh) { _prev_alphaTintMaskCh = alphaTintMaskCh; changed = true; }
        if (alphaTintColor != _prev_alphaTintColor) { _prev_alphaTintColor = alphaTintColor; changed = true; }
        if (alphaTintFade != _prev_alphaTintFade) { _prev_alphaTintFade = alphaTintFade; changed = true; }
        return changed;
    }

    bool CheckEffect_HitEffect()
    {
        bool changed = false;
        if (hitEffectMaskCh != _prev_hitEffectMaskCh) { _prev_hitEffectMaskCh = hitEffectMaskCh; changed = true; }
        if (hitEffectColor != _prev_hitEffectColor) { _prev_hitEffectColor = hitEffectColor; changed = true; }
        if (hitEffectBlend != _prev_hitEffectBlend) { _prev_hitEffectBlend = hitEffectBlend; changed = true; }
        return changed;
    }

    bool CheckEffect_Negative()
    {
        bool changed = false;
        if (negativeMaskCh != _prev_negativeMaskCh) { _prev_negativeMaskCh = negativeMaskCh; changed = true; }
        if (negativeAmount != _prev_negativeAmount) { _prev_negativeAmount = negativeAmount; changed = true; }
        return changed;
    }

    bool CheckEffect_Greyscale()
    {
        bool changed = false;
        if (greyscaleMaskCh != _prev_greyscaleMaskCh) { _prev_greyscaleMaskCh = greyscaleMaskCh; changed = true; }
        if (greyscaleBlend != _prev_greyscaleBlend) { _prev_greyscaleBlend = greyscaleBlend; changed = true; }
        if (greyscaleLuminosity != _prev_greyscaleLuminosity) { _prev_greyscaleLuminosity = greyscaleLuminosity; changed = true; }
        return changed;
    }

    bool CheckEffect_Bloom()
    {
        bool changed = false;
        if (bloomMaskCh != _prev_bloomMaskCh) { _prev_bloomMaskCh = bloomMaskCh; changed = true; }
        if (bloomMode != _prev_bloomMode) { _prev_bloomMode = bloomMode; changed = true; }
        if (bloomColor != _prev_bloomColor) { _prev_bloomColor = bloomColor; changed = true; }
        if (bloomThreshold != _prev_bloomThreshold) { _prev_bloomThreshold = bloomThreshold; changed = true; }
        if (bloomIntensity != _prev_bloomIntensity) { _prev_bloomIntensity = bloomIntensity; changed = true; }
        if (bloomSize != _prev_bloomSize) { _prev_bloomSize = bloomSize; changed = true; }
        if (bloomSamples != _prev_bloomSamples) { _prev_bloomSamples = bloomSamples; changed = true; }
        if (bloomSoftKnee != _prev_bloomSoftKnee) { _prev_bloomSoftKnee = bloomSoftKnee; changed = true; }
        if (bloomMaskColor != _prev_bloomMaskColor) { _prev_bloomMaskColor = bloomMaskColor; changed = true; }
        if (bloomMaskRange != _prev_bloomMaskRange) { _prev_bloomMaskRange = bloomMaskRange; changed = true; }
        if (bloomMaskSoft != _prev_bloomMaskSoft) { _prev_bloomMaskSoft = bloomMaskSoft; changed = true; }
        if (bloomAlphaBleed != _prev_bloomAlphaBleed) { _prev_bloomAlphaBleed = bloomAlphaBleed; changed = true; }
        return changed;
    }

    bool CheckEffect_ColorReplace()
    {
        bool changed = false;
        if (colorReplaceMaskCh != _prev_colorReplaceMaskCh) { _prev_colorReplaceMaskCh = colorReplaceMaskCh; changed = true; }
        if (colorReplaceSrc1 != _prev_colorReplaceSrc1) { _prev_colorReplaceSrc1 = colorReplaceSrc1; changed = true; }
        if (colorReplaceDst1 != _prev_colorReplaceDst1) { _prev_colorReplaceDst1 = colorReplaceDst1; changed = true; }
        if (colorReplaceSrc2 != _prev_colorReplaceSrc2) { _prev_colorReplaceSrc2 = colorReplaceSrc2; changed = true; }
        if (colorReplaceDst2 != _prev_colorReplaceDst2) { _prev_colorReplaceDst2 = colorReplaceDst2; changed = true; }
        if (colorReplaceSrc3 != _prev_colorReplaceSrc3) { _prev_colorReplaceSrc3 = colorReplaceSrc3; changed = true; }
        if (colorReplaceDst3 != _prev_colorReplaceDst3) { _prev_colorReplaceDst3 = colorReplaceDst3; changed = true; }
        if (colorReplaceSrc4 != _prev_colorReplaceSrc4) { _prev_colorReplaceSrc4 = colorReplaceSrc4; changed = true; }
        if (colorReplaceDst4 != _prev_colorReplaceDst4) { _prev_colorReplaceDst4 = colorReplaceDst4; changed = true; }
        if (colorReplaceCount != _prev_colorReplaceCount) { _prev_colorReplaceCount = colorReplaceCount; changed = true; }
        if (colorReplaceRange != _prev_colorReplaceRange) { _prev_colorReplaceRange = colorReplaceRange; changed = true; }
        if (colorReplaceFade != _prev_colorReplaceFade) { _prev_colorReplaceFade = colorReplaceFade; changed = true; }
        return changed;
    }

    bool CheckEffect_LUTReplace()
    {
        bool changed = false;
        if (lutReplaceMaskCh != _prev_lutReplaceMaskCh) { _prev_lutReplaceMaskCh = lutReplaceMaskCh; changed = true; }
        if (lutReplaceTexture != _prev_lutReplaceTexture) { _prev_lutReplaceTexture = lutReplaceTexture; changed = true; }
        if (lutSize != _prev_lutSize) { _prev_lutSize = lutSize; changed = true; }
        if (lutReplaceFade != _prev_lutReplaceFade) { _prev_lutReplaceFade = lutReplaceFade; changed = true; }
        return changed;
    }

    bool CheckEffect_HueShift()
    {
        bool changed = false;
        if (hueShiftMaskCh != _prev_hueShiftMaskCh) { _prev_hueShiftMaskCh = hueShiftMaskCh; changed = true; }
        if (hueShiftAmount != _prev_hueShiftAmount) { _prev_hueShiftAmount = hueShiftAmount; changed = true; }
        if (hueShiftSpeed != _prev_hueShiftSpeed) { _prev_hueShiftSpeed = hueShiftSpeed; changed = true; }
        if (hueShiftFade != _prev_hueShiftFade) { _prev_hueShiftFade = hueShiftFade; changed = true; }
        return changed;
    }

    bool CheckEffect_AddHue()
    {
        bool changed = false;
        if (addHueMaskCh != _prev_addHueMaskCh) { _prev_addHueMaskCh = addHueMaskCh; changed = true; }
        if (addHueAmount != _prev_addHueAmount) { _prev_addHueAmount = addHueAmount; changed = true; }
        if (addHueSpeed != _prev_addHueSpeed) { _prev_addHueSpeed = addHueSpeed; changed = true; }
        if (addHueSaturation != _prev_addHueSaturation) { _prev_addHueSaturation = addHueSaturation; changed = true; }
        if (addHueBrightness != _prev_addHueBrightness) { _prev_addHueBrightness = addHueBrightness; changed = true; }
        if (addHueFade != _prev_addHueFade) { _prev_addHueFade = addHueFade; changed = true; }
        return changed;
    }

    bool CheckEffect_Saturation()
    {
        bool changed = false;
        if (saturationMaskCh != _prev_saturationMaskCh) { _prev_saturationMaskCh = saturationMaskCh; changed = true; }
        if (saturationAmount != _prev_saturationAmount) { _prev_saturationAmount = saturationAmount; changed = true; }
        return changed;
    }

    bool CheckEffect_Brightness()
    {
        bool changed = false;
        if (brightnessMaskCh != _prev_brightnessMaskCh) { _prev_brightnessMaskCh = brightnessMaskCh; changed = true; }
        if (brightnessAmount != _prev_brightnessAmount) { _prev_brightnessAmount = brightnessAmount; changed = true; }
        return changed;
    }

    bool CheckEffect_Contrast()
    {
        bool changed = false;
        if (contrastMaskCh != _prev_contrastMaskCh) { _prev_contrastMaskCh = contrastMaskCh; changed = true; }
        if (contrastAmount != _prev_contrastAmount) { _prev_contrastAmount = contrastAmount; changed = true; }
        return changed;
    }

    bool CheckEffect_SplitToning()
    {
        bool changed = false;
        if (splitToningMaskCh != _prev_splitToningMaskCh) { _prev_splitToningMaskCh = splitToningMaskCh; changed = true; }
        if (splitShadowColor != _prev_splitShadowColor) { _prev_splitShadowColor = splitShadowColor; changed = true; }
        if (splitHighlightColor != _prev_splitHighlightColor) { _prev_splitHighlightColor = splitHighlightColor; changed = true; }
        if (splitBalance != _prev_splitBalance) { _prev_splitBalance = splitBalance; changed = true; }
        if (splitMidColor != _prev_splitMidColor) { _prev_splitMidColor = splitMidColor; changed = true; }
        if (splitFade != _prev_splitFade) { _prev_splitFade = splitFade; changed = true; }
        return changed;
    }

    bool CheckEffect_Posterize()
    {
        bool changed = false;
        if (posterizeMaskCh != _prev_posterizeMaskCh) { _prev_posterizeMaskCh = posterizeMaskCh; changed = true; }
        if (posterizeSteps != _prev_posterizeSteps) { _prev_posterizeSteps = posterizeSteps; changed = true; }
        if (posterizeFade != _prev_posterizeFade) { _prev_posterizeFade = posterizeFade; changed = true; }
        return changed;
    }

    bool CheckEffect_Poison()
    {
        bool changed = false;
        if (poisonMaskCh != _prev_poisonMaskCh) { _prev_poisonMaskCh = poisonMaskCh; changed = true; }
        if (poisonColor != _prev_poisonColor) { _prev_poisonColor = poisonColor; changed = true; }
        if (poisonSpeed != _prev_poisonSpeed) { _prev_poisonSpeed = poisonSpeed; changed = true; }
        if (poisonDensity != _prev_poisonDensity) { _prev_poisonDensity = poisonDensity; changed = true; }
        if (poisonColor2 != _prev_poisonColor2) { _prev_poisonColor2 = poisonColor2; changed = true; }
        if (poisonColor3 != _prev_poisonColor3) { _prev_poisonColor3 = poisonColor3; changed = true; }
        if (poisonFade != _prev_poisonFade) { _prev_poisonFade = poisonFade; changed = true; }
        return changed;
    }

    bool CheckEffect_Frozen()
    {
        bool changed = false;
        if (frozenMaskCh != _prev_frozenMaskCh) { _prev_frozenMaskCh = frozenMaskCh; changed = true; }
        if (frozenColor != _prev_frozenColor) { _prev_frozenColor = frozenColor; changed = true; }
        if (frozenContrast != _prev_frozenContrast) { _prev_frozenContrast = frozenContrast; changed = true; }
        if (frozenHighlight != _prev_frozenHighlight) { _prev_frozenHighlight = frozenHighlight; changed = true; }
        if (frozenColor2 != _prev_frozenColor2) { _prev_frozenColor2 = frozenColor2; changed = true; }
        if (frozenHighlightColor != _prev_frozenHighlightColor) { _prev_frozenHighlightColor = frozenHighlightColor; changed = true; }
        if (frozenFade != _prev_frozenFade) { _prev_frozenFade = frozenFade; changed = true; }
        return changed;
    }

    bool CheckEffect_Metal()
    {
        bool changed = false;
        if (metalMaskCh != _prev_metalMaskCh) { _prev_metalMaskCh = metalMaskCh; changed = true; }
        if (metalColor != _prev_metalColor) { _prev_metalColor = metalColor; changed = true; }
        if (metalHighlight != _prev_metalHighlight) { _prev_metalHighlight = metalHighlight; changed = true; }
        if (metalContrast != _prev_metalContrast) { _prev_metalContrast = metalContrast; changed = true; }
        if (metalSpeed != _prev_metalSpeed) { _prev_metalSpeed = metalSpeed; changed = true; }
        if (metalColor2 != _prev_metalColor2) { _prev_metalColor2 = metalColor2; changed = true; }
        if (metalFade != _prev_metalFade) { _prev_metalFade = metalFade; changed = true; }
        return changed;
    }

    bool CheckEffect_Camouflage()
    {
        bool changed = false;
        if (camouflageMaskCh != _prev_camouflageMaskCh) { _prev_camouflageMaskCh = camouflageMaskCh; changed = true; }
        if (camoColor1 != _prev_camoColor1) { _prev_camoColor1 = camoColor1; changed = true; }
        if (camoColor2 != _prev_camoColor2) { _prev_camoColor2 = camoColor2; changed = true; }
        if (camoColor3 != _prev_camoColor3) { _prev_camoColor3 = camoColor3; changed = true; }
        if (camoScale != _prev_camoScale) { _prev_camoScale = camoScale; changed = true; }
        if (camoSpeed != _prev_camoSpeed) { _prev_camoSpeed = camoSpeed; changed = true; }
        if (camoFade != _prev_camoFade) { _prev_camoFade = camoFade; changed = true; }
        return changed;
    }

    bool CheckEffect_Rainbow()
    {
        bool changed = false;
        if (rainbowMaskCh != _prev_rainbowMaskCh) { _prev_rainbowMaskCh = rainbowMaskCh; changed = true; }
        if (rainbowSpeed != _prev_rainbowSpeed) { _prev_rainbowSpeed = rainbowSpeed; changed = true; }
        if (rainbowSaturation != _prev_rainbowSaturation) { _prev_rainbowSaturation = rainbowSaturation; changed = true; }
        if (rainbowBrightness != _prev_rainbowBrightness) { _prev_rainbowBrightness = rainbowBrightness; changed = true; }
        if (rainbowFade != _prev_rainbowFade) { _prev_rainbowFade = rainbowFade; changed = true; }
        if (rainbowDensity != _prev_rainbowDensity) { _prev_rainbowDensity = rainbowDensity; changed = true; }
        return changed;
    }

    bool CheckEffect_BlackTint()
    {
        bool changed = false;
        if (blackTintMaskCh != _prev_blackTintMaskCh) { _prev_blackTintMaskCh = blackTintMaskCh; changed = true; }
        if (blackTintColor != _prev_blackTintColor) { _prev_blackTintColor = blackTintColor; changed = true; }
        if (blackTintThreshold != _prev_blackTintThreshold) { _prev_blackTintThreshold = blackTintThreshold; changed = true; }
        if (blackTintFade != _prev_blackTintFade) { _prev_blackTintFade = blackTintFade; changed = true; }
        return changed;
    }

    bool CheckEffect_Chromatic()
    {
        bool changed = false;
        if (chromaticMaskCh != _prev_chromaticMaskCh) { _prev_chromaticMaskCh = chromaticMaskCh; changed = true; }
        if (chromaticAmount != _prev_chromaticAmount) { _prev_chromaticAmount = chromaticAmount; changed = true; }
        if (chromaticAngle != _prev_chromaticAngle) { _prev_chromaticAngle = chromaticAngle; changed = true; }
        return changed;
    }

    bool CheckEffect_PaletteSwap()
    {
        bool changed = false;
        if (paletteSwapMaskCh != _prev_paletteSwapMaskCh) { _prev_paletteSwapMaskCh = paletteSwapMaskCh; changed = true; }
        if (paletteSwapTexture != _prev_paletteSwapTexture) { _prev_paletteSwapTexture = paletteSwapTexture; changed = true; }
        if (paletteSwapColors != _prev_paletteSwapColors) { _prev_paletteSwapColors = paletteSwapColors; changed = true; }
        if (paletteSwapFade != _prev_paletteSwapFade) { _prev_paletteSwapFade = paletteSwapFade; changed = true; }
        return changed;
    }

    bool CheckEffect_Dithering()
    {
        bool changed = false;
        if (ditheringMaskCh != _prev_ditheringMaskCh) { _prev_ditheringMaskCh = ditheringMaskCh; changed = true; }
        if (ditheringSize != _prev_ditheringSize) { _prev_ditheringSize = ditheringSize; changed = true; }
        if (ditheringLevels != _prev_ditheringLevels) { _prev_ditheringLevels = ditheringLevels; changed = true; }
        if (ditheringFade != _prev_ditheringFade) { _prev_ditheringFade = ditheringFade; changed = true; }
        if (ditheringAlphaMode != _prev_ditheringAlphaMode) { _prev_ditheringAlphaMode = ditheringAlphaMode; changed = true; }
        if (ditheringAlphaThreshold != _prev_ditheringAlphaThreshold) { _prev_ditheringAlphaThreshold = ditheringAlphaThreshold; changed = true; }
        if (ditheringScreenSpace != _prev_ditheringScreenSpace) { _prev_ditheringScreenSpace = ditheringScreenSpace; changed = true; }
        return changed;
    }

    bool CheckEffect_CRTScanlines()
    {
        bool changed = false;
        if (crtScanlinesMaskCh != _prev_crtScanlinesMaskCh) { _prev_crtScanlinesMaskCh = crtScanlinesMaskCh; changed = true; }
        if (crtDensity != _prev_crtDensity) { _prev_crtDensity = crtDensity; changed = true; }
        if (crtIntensity != _prev_crtIntensity) { _prev_crtIntensity = crtIntensity; changed = true; }
        if (crtSpeed != _prev_crtSpeed) { _prev_crtSpeed = crtSpeed; changed = true; }
        if (crtCurvature != _prev_crtCurvature) { _prev_crtCurvature = crtCurvature; changed = true; }
        return changed;
    }

    bool CheckEffect_Vignette()
    {
        bool changed = false;
        if (vignetteMaskCh != _prev_vignetteMaskCh) { _prev_vignetteMaskCh = vignetteMaskCh; changed = true; }
        if (vignetteColor != _prev_vignetteColor) { _prev_vignetteColor = vignetteColor; changed = true; }
        if (vignetteIntensity != _prev_vignetteIntensity) { _prev_vignetteIntensity = vignetteIntensity; changed = true; }
        if (vignetteSmoothness != _prev_vignetteSmoothness) { _prev_vignetteSmoothness = vignetteSmoothness; changed = true; }
        if (vignetteAlpha != _prev_vignetteAlpha) { _prev_vignetteAlpha = vignetteAlpha; changed = true; }
        if (vignetteCenter != _prev_vignetteCenter) { _prev_vignetteCenter = vignetteCenter; changed = true; }
        if (vignetteOffset != _prev_vignetteOffset) { _prev_vignetteOffset = vignetteOffset; changed = true; }
        return changed;
    }

    bool CheckEffect_ColorGrading()
    {
        bool changed = false;
        if (colorGradingMaskCh != _prev_colorGradingMaskCh) { _prev_colorGradingMaskCh = colorGradingMaskCh; changed = true; }
        if (cgExposure != _prev_cgExposure) { _prev_cgExposure = cgExposure; changed = true; }
        if (cgGamma != _prev_cgGamma) { _prev_cgGamma = cgGamma; changed = true; }
        if (cgLift != _prev_cgLift) { _prev_cgLift = cgLift; changed = true; }
        return changed;
    }

    bool CheckEffect_Sepia()
    {
        bool changed = false;
        if (sepiaMaskCh != _prev_sepiaMaskCh) { _prev_sepiaMaskCh = sepiaMaskCh; changed = true; }
        if (sepiaIntensity != _prev_sepiaIntensity) { _prev_sepiaIntensity = sepiaIntensity; changed = true; }
        if (sepiaColor != _prev_sepiaColor) { _prev_sepiaColor = sepiaColor; changed = true; }
        return changed;
    }

    bool CheckEffect_ThermalVision()
    {
        bool changed = false;
        if (thermalVisionMaskCh != _prev_thermalVisionMaskCh) { _prev_thermalVisionMaskCh = thermalVisionMaskCh; changed = true; }
        if (thermalIntensity != _prev_thermalIntensity) { _prev_thermalIntensity = thermalIntensity; changed = true; }
        if (thermalHDRGlow != _prev_thermalHDRGlow) { _prev_thermalHDRGlow = thermalHDRGlow; changed = true; }
        return changed;
    }

    bool CheckEffect_RecolorRGB()
    {
        bool changed = false;
        if (recolorRGBMaskCh != _prev_recolorRGBMaskCh) { _prev_recolorRGBMaskCh = recolorRGBMaskCh; changed = true; }
        if (recolorR != _prev_recolorR) { _prev_recolorR = recolorR; changed = true; }
        if (recolorG != _prev_recolorG) { _prev_recolorG = recolorG; changed = true; }
        if (recolorB != _prev_recolorB) { _prev_recolorB = recolorB; changed = true; }
        if (recolorCyan != _prev_recolorCyan) { _prev_recolorCyan = recolorCyan; changed = true; }
        if (recolorMagenta != _prev_recolorMagenta) { _prev_recolorMagenta = recolorMagenta; changed = true; }
        if (recolorYellow != _prev_recolorYellow) { _prev_recolorYellow = recolorYellow; changed = true; }
        if (recolorFade != _prev_recolorFade) { _prev_recolorFade = recolorFade; changed = true; }
        return changed;
    }

    bool CheckEffect_PingPongGlow()
    {
        bool changed = false;
        if (pingPongGlowMaskCh != _prev_pingPongGlowMaskCh) { _prev_pingPongGlowMaskCh = pingPongGlowMaskCh; changed = true; }
        if (pingPongGlowColor != _prev_pingPongGlowColor) { _prev_pingPongGlowColor = pingPongGlowColor; changed = true; }
        if (pingPongGlowSpeed != _prev_pingPongGlowSpeed) { _prev_pingPongGlowSpeed = pingPongGlowSpeed; changed = true; }
        if (pingPongGlowMin != _prev_pingPongGlowMin) { _prev_pingPongGlowMin = pingPongGlowMin; changed = true; }
        if (pingPongGlowMax != _prev_pingPongGlowMax) { _prev_pingPongGlowMax = pingPongGlowMax; changed = true; }
        return changed;
    }

    bool CheckEffect_ColorCycle()
    {
        bool changed = false;
        if (colorCycleMaskCh != _prev_colorCycleMaskCh) { _prev_colorCycleMaskCh = colorCycleMaskCh; changed = true; }
        if (colorCycleA != _prev_colorCycleA) { _prev_colorCycleA = colorCycleA; changed = true; }
        if (colorCycleB != _prev_colorCycleB) { _prev_colorCycleB = colorCycleB; changed = true; }
        if (colorCycleC != _prev_colorCycleC) { _prev_colorCycleC = colorCycleC; changed = true; }
        if (colorCycleD != _prev_colorCycleD) { _prev_colorCycleD = colorCycleD; changed = true; }
        if (colorCycleSpeed != _prev_colorCycleSpeed) { _prev_colorCycleSpeed = colorCycleSpeed; changed = true; }
        if (colorCycleFade != _prev_colorCycleFade) { _prev_colorCycleFade = colorCycleFade; changed = true; }
        return changed;
    }

    bool CheckEffect_Outline()
    {
        bool changed = false;
        if (outlineMaskCh != _prev_outlineMaskCh) { _prev_outlineMaskCh = outlineMaskCh; changed = true; }
        if (outlineColor != _prev_outlineColor) { _prev_outlineColor = outlineColor; changed = true; }
        if (outlineSize != _prev_outlineSize) { _prev_outlineSize = outlineSize; changed = true; }
        if (outlineAlpha != _prev_outlineAlpha) { _prev_outlineAlpha = outlineAlpha; changed = true; }
        return changed;
    }

    bool CheckEffect_InnerOutline()
    {
        bool changed = false;
        if (innerOutlineMaskCh != _prev_innerOutlineMaskCh) { _prev_innerOutlineMaskCh = innerOutlineMaskCh; changed = true; }
        if (innerOutlineColor != _prev_innerOutlineColor) { _prev_innerOutlineColor = innerOutlineColor; changed = true; }
        if (innerOutlineThickness != _prev_innerOutlineThickness) { _prev_innerOutlineThickness = innerOutlineThickness; changed = true; }
        if (innerOutlineAlpha != _prev_innerOutlineAlpha) { _prev_innerOutlineAlpha = innerOutlineAlpha; changed = true; }
        return changed;
    }

    bool CheckEffect_TexOutline()
    {
        bool changed = false;
        if (texOutlineMaskCh != _prev_texOutlineMaskCh) { _prev_texOutlineMaskCh = texOutlineMaskCh; changed = true; }
        if (texOutlineTexture != _prev_texOutlineTexture) { _prev_texOutlineTexture = texOutlineTexture; changed = true; }
        if (texOutlineColor != _prev_texOutlineColor) { _prev_texOutlineColor = texOutlineColor; changed = true; }
        if (texOutlineSize != _prev_texOutlineSize) { _prev_texOutlineSize = texOutlineSize; changed = true; }
        if (texOutlineTiling != _prev_texOutlineTiling) { _prev_texOutlineTiling = texOutlineTiling; changed = true; }
        if (texOutlineSpeed != _prev_texOutlineSpeed) { _prev_texOutlineSpeed = texOutlineSpeed; changed = true; }
        return changed;
    }

    bool CheckEffect_RainbowOutline()
    {
        bool changed = false;
        if (rainbowOutlineMaskCh != _prev_rainbowOutlineMaskCh) { _prev_rainbowOutlineMaskCh = rainbowOutlineMaskCh; changed = true; }
        if (rainbowOutlineColor != _prev_rainbowOutlineColor) { _prev_rainbowOutlineColor = rainbowOutlineColor; changed = true; }
        if (rainbowOutlineSize != _prev_rainbowOutlineSize) { _prev_rainbowOutlineSize = rainbowOutlineSize; changed = true; }
        if (rainbowOutlineSpeed != _prev_rainbowOutlineSpeed) { _prev_rainbowOutlineSpeed = rainbowOutlineSpeed; changed = true; }
        if (rainbowOutlineDensity != _prev_rainbowOutlineDensity) { _prev_rainbowOutlineDensity = rainbowOutlineDensity; changed = true; }
        return changed;
    }

    bool CheckEffect_DistortOutline()
    {
        bool changed = false;
        if (distortOutlineMaskCh != _prev_distortOutlineMaskCh) { _prev_distortOutlineMaskCh = distortOutlineMaskCh; changed = true; }
        if (distortOutlineColor != _prev_distortOutlineColor) { _prev_distortOutlineColor = distortOutlineColor; changed = true; }
        if (distortOutlineSize != _prev_distortOutlineSize) { _prev_distortOutlineSize = distortOutlineSize; changed = true; }
        if (distortOutlineAmount != _prev_distortOutlineAmount) { _prev_distortOutlineAmount = distortOutlineAmount; changed = true; }
        if (distortOutlineSpeed != _prev_distortOutlineSpeed) { _prev_distortOutlineSpeed = distortOutlineSpeed; changed = true; }
        return changed;
    }

    bool CheckEffect_Glow()
    {
        bool changed = false;
        if (glowMaskCh != _prev_glowMaskCh) { _prev_glowMaskCh = glowMaskCh; changed = true; }
        if (glowColor != _prev_glowColor) { _prev_glowColor = glowColor; changed = true; }
        if (glowSize != _prev_glowSize) { _prev_glowSize = glowSize; changed = true; }
        if (glowIntensity != _prev_glowIntensity) { _prev_glowIntensity = glowIntensity; changed = true; }
        return changed;
    }

    bool CheckEffect_Shadow()
    {
        bool changed = false;
        if (shadowMaskCh != _prev_shadowMaskCh) { _prev_shadowMaskCh = shadowMaskCh; changed = true; }
        if (shadowColor != _prev_shadowColor) { _prev_shadowColor = shadowColor; changed = true; }
        if (shadowOffset != _prev_shadowOffset) { _prev_shadowOffset = shadowOffset; changed = true; }
        return changed;
    }

    bool CheckEffect_SoftShadow()
    {
        bool changed = false;
        if (softShadowMaskCh != _prev_softShadowMaskCh) { _prev_softShadowMaskCh = softShadowMaskCh; changed = true; }
        if (softShadowColor != _prev_softShadowColor) { _prev_softShadowColor = softShadowColor; changed = true; }
        if (softShadowOffset != _prev_softShadowOffset) { _prev_softShadowOffset = softShadowOffset; changed = true; }
        if (softShadowSoftness != _prev_softShadowSoftness) { _prev_softShadowSoftness = softShadowSoftness; changed = true; }
        if (softShadowSamples != _prev_softShadowSamples) { _prev_softShadowSamples = softShadowSamples; changed = true; }
        return changed;
    }

    bool CheckEffect_DirOutline()
    {
        bool changed = false;
        if (dirOutlineMaskCh != _prev_dirOutlineMaskCh) { _prev_dirOutlineMaskCh = dirOutlineMaskCh; changed = true; }
        if (dirOutlineColor != _prev_dirOutlineColor) { _prev_dirOutlineColor = dirOutlineColor; changed = true; }
        if (dirOutlineWidth != _prev_dirOutlineWidth) { _prev_dirOutlineWidth = dirOutlineWidth; changed = true; }
        if (dirOutlineIntensity != _prev_dirOutlineIntensity) { _prev_dirOutlineIntensity = dirOutlineIntensity; changed = true; }
        if (dirOutlineDirection != _prev_dirOutlineDirection) { _prev_dirOutlineDirection = dirOutlineDirection; changed = true; }
        if (dirOutlineSoftness != _prev_dirOutlineSoftness) { _prev_dirOutlineSoftness = dirOutlineSoftness; changed = true; }
        return changed;
    }

    bool CheckEffect_OutlineOnly()
    {
        bool changed = false;
        if (outlineOnlyMaskCh != _prev_outlineOnlyMaskCh) { _prev_outlineOnlyMaskCh = outlineOnlyMaskCh; changed = true; }
        if (outlineOnlyColor != _prev_outlineOnlyColor) { _prev_outlineOnlyColor = outlineOnlyColor; changed = true; }
        if (outlineOnlyWidth != _prev_outlineOnlyWidth) { _prev_outlineOnlyWidth = outlineOnlyWidth; changed = true; }
        return changed;
    }

    bool CheckEffect_GradientOutline()
    {
        bool changed = false;
        if (gradOutlineMaskCh != _prev_gradOutlineMaskCh) { _prev_gradOutlineMaskCh = gradOutlineMaskCh; changed = true; }
        if (gradOutlineColorA != _prev_gradOutlineColorA) { _prev_gradOutlineColorA = gradOutlineColorA; changed = true; }
        if (gradOutlineColorB != _prev_gradOutlineColorB) { _prev_gradOutlineColorB = gradOutlineColorB; changed = true; }
        if (gradOutlineAngle != _prev_gradOutlineAngle) { _prev_gradOutlineAngle = gradOutlineAngle; changed = true; }
        if (gradOutlineSize != _prev_gradOutlineSize) { _prev_gradOutlineSize = gradOutlineSize; changed = true; }
        return changed;
    }

    bool CheckEffect_AnimOutlineThick()
    {
        bool changed = false;
        if (animOutlineMaskCh != _prev_animOutlineMaskCh) { _prev_animOutlineMaskCh = animOutlineMaskCh; changed = true; }
        if (animOutlineColor != _prev_animOutlineColor) { _prev_animOutlineColor = animOutlineColor; changed = true; }
        if (animOutlineMin != _prev_animOutlineMin) { _prev_animOutlineMin = animOutlineMin; changed = true; }
        if (animOutlineMax != _prev_animOutlineMax) { _prev_animOutlineMax = animOutlineMax; changed = true; }
        if (animOutlineSpeed != _prev_animOutlineSpeed) { _prev_animOutlineSpeed = animOutlineSpeed; changed = true; }
        return changed;
    }

    bool CheckEffect_SelectionHighlight()
    {
        bool changed = false;
        if (selectHighlightMaskCh != _prev_selectHighlightMaskCh) { _prev_selectHighlightMaskCh = selectHighlightMaskCh; changed = true; }
        if (selectHighlightColor != _prev_selectHighlightColor) { _prev_selectHighlightColor = selectHighlightColor; changed = true; }
        if (selectHighlightSize != _prev_selectHighlightSize) { _prev_selectHighlightSize = selectHighlightSize; changed = true; }
        if (selectHighlightDash != _prev_selectHighlightDash) { _prev_selectHighlightDash = selectHighlightDash; changed = true; }
        if (selectHighlightSpeed != _prev_selectHighlightSpeed) { _prev_selectHighlightSpeed = selectHighlightSpeed; changed = true; }
        return changed;
    }

    bool CheckEffect_Pixelate()
    {
        bool changed = false;
        if (pixelateMaskCh != _prev_pixelateMaskCh) { _prev_pixelateMaskCh = pixelateMaskCh; changed = true; }
        if (pixelateSize != _prev_pixelateSize) { _prev_pixelateSize = pixelateSize; changed = true; }
        return changed;
    }

    bool CheckEffect_Wave()
    {
        bool changed = false;
        if (waveMaskCh != _prev_waveMaskCh) { _prev_waveMaskCh = waveMaskCh; changed = true; }
        if (waveSpeed != _prev_waveSpeed) { _prev_waveSpeed = waveSpeed; changed = true; }
        if (waveAmplitude != _prev_waveAmplitude) { _prev_waveAmplitude = waveAmplitude; changed = true; }
        if (waveFrequency != _prev_waveFrequency) { _prev_waveFrequency = waveFrequency; changed = true; }
        return changed;
    }

    bool CheckEffect_Twist()
    {
        bool changed = false;
        if (twistMaskCh != _prev_twistMaskCh) { _prev_twistMaskCh = twistMaskCh; changed = true; }
        if (twistAmount != _prev_twistAmount) { _prev_twistAmount = twistAmount; changed = true; }
        if (twistSpeed != _prev_twistSpeed) { _prev_twistSpeed = twistSpeed; changed = true; }
        if (twistOffset != _prev_twistOffset) { _prev_twistOffset = twistOffset; changed = true; }
        if (twistRadius != _prev_twistRadius) { _prev_twistRadius = twistRadius; changed = true; }
        return changed;
    }

    bool CheckEffect_UVDistort()
    {
        bool changed = false;
        if (uvDistortMaskCh != _prev_uvDistortMaskCh) { _prev_uvDistortMaskCh = uvDistortMaskCh; changed = true; }
        if (distortTexture != _prev_distortTexture) { _prev_distortTexture = distortTexture; changed = true; }
        if (distortAmount != _prev_distortAmount) { _prev_distortAmount = distortAmount; changed = true; }
        if (distortTiling != _prev_distortTiling) { _prev_distortTiling = distortTiling; changed = true; }
        if (distortSpeed != _prev_distortSpeed) { _prev_distortSpeed = distortSpeed; changed = true; }
        if (distortIsNormalMap != _prev_distortIsNormalMap) { _prev_distortIsNormalMap = distortIsNormalMap; changed = true; }
        return changed;
    }

    bool CheckEffect_FullDistort()
    {
        bool changed = false;
        if (fullDistortMaskCh != _prev_fullDistortMaskCh) { _prev_fullDistortMaskCh = fullDistortMaskCh; changed = true; }
        if (fullDistortAmount != _prev_fullDistortAmount) { _prev_fullDistortAmount = fullDistortAmount; changed = true; }
        if (fullDistortSpeed != _prev_fullDistortSpeed) { _prev_fullDistortSpeed = fullDistortSpeed; changed = true; }
        if (fullDistortDensity != _prev_fullDistortDensity) { _prev_fullDistortDensity = fullDistortDensity; changed = true; }
        return changed;
    }

    bool CheckEffect_DirDistort()
    {
        bool changed = false;
        if (dirDistortMaskCh != _prev_dirDistortMaskCh) { _prev_dirDistortMaskCh = dirDistortMaskCh; changed = true; }
        if (dirDistortAngle != _prev_dirDistortAngle) { _prev_dirDistortAngle = dirDistortAngle; changed = true; }
        if (dirDistortAmount != _prev_dirDistortAmount) { _prev_dirDistortAmount = dirDistortAmount; changed = true; }
        if (dirDistortSpeed != _prev_dirDistortSpeed) { _prev_dirDistortSpeed = dirDistortSpeed; changed = true; }
        return changed;
    }

    bool CheckEffect_UVScroll()
    {
        bool changed = false;
        if (uvScrollMaskCh != _prev_uvScrollMaskCh) { _prev_uvScrollMaskCh = uvScrollMaskCh; changed = true; }
        if (uvScrollSpeedX != _prev_uvScrollSpeedX) { _prev_uvScrollSpeedX = uvScrollSpeedX; changed = true; }
        if (uvScrollSpeedY != _prev_uvScrollSpeedY) { _prev_uvScrollSpeedY = uvScrollSpeedY; changed = true; }
        return changed;
    }

    bool CheckEffect_Rotate()
    {
        bool changed = false;
        if (rotateMaskCh != _prev_rotateMaskCh) { _prev_rotateMaskCh = rotateMaskCh; changed = true; }
        if (rotateSpeed != _prev_rotateSpeed) { _prev_rotateSpeed = rotateSpeed; changed = true; }
        if (rotateAmount != _prev_rotateAmount) { _prev_rotateAmount = rotateAmount; changed = true; }
        return changed;
    }

    bool CheckEffect_Squeeze()
    {
        bool changed = false;
        if (squeezeMaskCh != _prev_squeezeMaskCh) { _prev_squeezeMaskCh = squeezeMaskCh; changed = true; }
        if (squeezeAmount != _prev_squeezeAmount) { _prev_squeezeAmount = squeezeAmount; changed = true; }
        if (squeezeSpeed != _prev_squeezeSpeed) { _prev_squeezeSpeed = squeezeSpeed; changed = true; }
        if (squeezePower != _prev_squeezePower) { _prev_squeezePower = squeezePower; changed = true; }
        return changed;
    }

    bool CheckEffect_Squish()
    {
        bool changed = false;
        if (squishMaskCh != _prev_squishMaskCh) { _prev_squishMaskCh = squishMaskCh; changed = true; }
        if (squishAmount != _prev_squishAmount) { _prev_squishAmount = squishAmount; changed = true; }
        if (squishSpeed != _prev_squishSpeed) { _prev_squishSpeed = squishSpeed; changed = true; }
        if (squishPower != _prev_squishPower) { _prev_squishPower = squishPower; changed = true; }
        return changed;
    }

    bool CheckEffect_Wiggle()
    {
        bool changed = false;
        if (wiggleMaskCh != _prev_wiggleMaskCh) { _prev_wiggleMaskCh = wiggleMaskCh; changed = true; }
        if (wiggleSpeed != _prev_wiggleSpeed) { _prev_wiggleSpeed = wiggleSpeed; changed = true; }
        if (wiggleAmount != _prev_wiggleAmount) { _prev_wiggleAmount = wiggleAmount; changed = true; }
        if (wiggleFrequency != _prev_wiggleFrequency) { _prev_wiggleFrequency = wiggleFrequency; changed = true; }
        return changed;
    }

    bool CheckEffect_Wind()
    {
        bool changed = false;
        if (windMaskCh != _prev_windMaskCh) { _prev_windMaskCh = windMaskCh; changed = true; }
        if (windSpeed != _prev_windSpeed) { _prev_windSpeed = windSpeed; changed = true; }
        if (windAmount != _prev_windAmount) { _prev_windAmount = windAmount; changed = true; }
        if (windBottomY != _prev_windBottomY) { _prev_windBottomY = windBottomY; changed = true; }
        if (windFrequency != _prev_windFrequency) { _prev_windFrequency = windFrequency; changed = true; }
        return changed;
    }

    bool CheckEffect_Sharpen()
    {
        bool changed = false;
        if (sharpenMaskCh != _prev_sharpenMaskCh) { _prev_sharpenMaskCh = sharpenMaskCh; changed = true; }
        if (sharpenAmount != _prev_sharpenAmount) { _prev_sharpenAmount = sharpenAmount; changed = true; }
        if (sharpenSize != _prev_sharpenSize) { _prev_sharpenSize = sharpenSize; changed = true; }
        return changed;
    }

    bool CheckEffect_GaussBlur()
    {
        bool changed = false;
        if (gaussBlurMaskCh != _prev_gaussBlurMaskCh) { _prev_gaussBlurMaskCh = gaussBlurMaskCh; changed = true; }
        if (gaussBlurSize != _prev_gaussBlurSize) { _prev_gaussBlurSize = gaussBlurSize; changed = true; }
        if (gaussBlurSamples != _prev_gaussBlurSamples) { _prev_gaussBlurSamples = gaussBlurSamples; changed = true; }
        return changed;
    }

    bool CheckEffect_DirBlur()
    {
        bool changed = false;
        if (dirBlurMaskCh != _prev_dirBlurMaskCh) { _prev_dirBlurMaskCh = dirBlurMaskCh; changed = true; }
        if (dirBlurAngle != _prev_dirBlurAngle) { _prev_dirBlurAngle = dirBlurAngle; changed = true; }
        if (dirBlurSize != _prev_dirBlurSize) { _prev_dirBlurSize = dirBlurSize; changed = true; }
        if (dirBlurSamples != _prev_dirBlurSamples) { _prev_dirBlurSamples = dirBlurSamples; changed = true; }
        return changed;
    }

    bool CheckEffect_WorldTiling()
    {
        bool changed = false;
        if (worldTilingMaskCh != _prev_worldTilingMaskCh) { _prev_worldTilingMaskCh = worldTilingMaskCh; changed = true; }
        if (worldTileScale != _prev_worldTileScale) { _prev_worldTileScale = worldTileScale; changed = true; }
        if (worldTileOffset != _prev_worldTileOffset) { _prev_worldTileOffset = worldTileOffset; changed = true; }
        if (worldTileRotation != _prev_worldTileRotation) { _prev_worldTileRotation = worldTileRotation; changed = true; }
        return changed;
    }

    bool CheckEffect_UVScaleOffset()
    {
        bool changed = false;
        if (uvScaleOffsetMaskCh != _prev_uvScaleOffsetMaskCh) { _prev_uvScaleOffsetMaskCh = uvScaleOffsetMaskCh; changed = true; }
        if (uvScaleOffset != _prev_uvScaleOffset) { _prev_uvScaleOffset = uvScaleOffset; changed = true; }
        return changed;
    }

    bool CheckEffect_CurlNoise()
    {
        bool changed = false;
        if (curlNoiseMaskCh != _prev_curlNoiseMaskCh) { _prev_curlNoiseMaskCh = curlNoiseMaskCh; changed = true; }
        if (curlNoiseAmount != _prev_curlNoiseAmount) { _prev_curlNoiseAmount = curlNoiseAmount; changed = true; }
        if (curlNoiseSpeed != _prev_curlNoiseSpeed) { _prev_curlNoiseSpeed = curlNoiseSpeed; changed = true; }
        if (curlNoiseDensity != _prev_curlNoiseDensity) { _prev_curlNoiseDensity = curlNoiseDensity; changed = true; }
        return changed;
    }

    bool CheckEffect_FBMNoise()
    {
        bool changed = false;
        if (fbmNoiseMaskCh != _prev_fbmNoiseMaskCh) { _prev_fbmNoiseMaskCh = fbmNoiseMaskCh; changed = true; }
        if (fbmNoiseAmount != _prev_fbmNoiseAmount) { _prev_fbmNoiseAmount = fbmNoiseAmount; changed = true; }
        if (fbmNoiseSpeed != _prev_fbmNoiseSpeed) { _prev_fbmNoiseSpeed = fbmNoiseSpeed; changed = true; }
        if (fbmNoiseDensity != _prev_fbmNoiseDensity) { _prev_fbmNoiseDensity = fbmNoiseDensity; changed = true; }
        if (fbmNoiseOctaves != _prev_fbmNoiseOctaves) { _prev_fbmNoiseOctaves = fbmNoiseOctaves; changed = true; }
        return changed;
    }

    bool CheckEffect_UVScrollOverlay()
    {
        bool changed = false;
        if (uvScrollOverlayMaskCh != _prev_uvScrollOverlayMaskCh) { _prev_uvScrollOverlayMaskCh = uvScrollOverlayMaskCh; changed = true; }
        if (uvScrollOverlayTex != _prev_uvScrollOverlayTex) { _prev_uvScrollOverlayTex = uvScrollOverlayTex; changed = true; }
        if (uvScrollOverlaySpeedX != _prev_uvScrollOverlaySpeedX) { _prev_uvScrollOverlaySpeedX = uvScrollOverlaySpeedX; changed = true; }
        if (uvScrollOverlaySpeedY != _prev_uvScrollOverlaySpeedY) { _prev_uvScrollOverlaySpeedY = uvScrollOverlaySpeedY; changed = true; }
        if (uvScrollOverlayBlend != _prev_uvScrollOverlayBlend) { _prev_uvScrollOverlayBlend = uvScrollOverlayBlend; changed = true; }
        if (uvScrollOverlayTint != _prev_uvScrollOverlayTint) { _prev_uvScrollOverlayTint = uvScrollOverlayTint; changed = true; }
        if (uvScrollOverlayTiling != _prev_uvScrollOverlayTiling) { _prev_uvScrollOverlayTiling = uvScrollOverlayTiling; changed = true; }
        return changed;
    }

    bool CheckEffect_Shockwave()
    {
        bool changed = false;
        if (shockwaveMaskCh != _prev_shockwaveMaskCh) { _prev_shockwaveMaskCh = shockwaveMaskCh; changed = true; }
        if (shockwaveCenter != _prev_shockwaveCenter) { _prev_shockwaveCenter = shockwaveCenter; changed = true; }
        if (shockwaveRadius != _prev_shockwaveRadius) { _prev_shockwaveRadius = shockwaveRadius; changed = true; }
        if (shockwaveWidth != _prev_shockwaveWidth) { _prev_shockwaveWidth = shockwaveWidth; changed = true; }
        if (shockwaveAmplitude != _prev_shockwaveAmplitude) { _prev_shockwaveAmplitude = shockwaveAmplitude; changed = true; }
        return changed;
    }

    bool CheckEffect_SineScale()
    {
        bool changed = false;
        if (sineScaleMaskCh != _prev_sineScaleMaskCh) { _prev_sineScaleMaskCh = sineScaleMaskCh; changed = true; }
        if (sineScaleAmountX != _prev_sineScaleAmountX) { _prev_sineScaleAmountX = sineScaleAmountX; changed = true; }
        if (sineScaleAmountY != _prev_sineScaleAmountY) { _prev_sineScaleAmountY = sineScaleAmountY; changed = true; }
        if (sineScaleSpeed != _prev_sineScaleSpeed) { _prev_sineScaleSpeed = sineScaleSpeed; changed = true; }
        return changed;
    }

    bool CheckEffect_Heartbeat()
    {
        bool changed = false;
        if (heartbeatMaskCh != _prev_heartbeatMaskCh) { _prev_heartbeatMaskCh = heartbeatMaskCh; changed = true; }
        if (heartbeatSpeed != _prev_heartbeatSpeed) { _prev_heartbeatSpeed = heartbeatSpeed; changed = true; }
        if (heartbeatAmount != _prev_heartbeatAmount) { _prev_heartbeatAmount = heartbeatAmount; changed = true; }
        return changed;
    }

    bool CheckEffect_Breathing()
    {
        bool changed = false;
        if (breathingMaskCh != _prev_breathingMaskCh) { _prev_breathingMaskCh = breathingMaskCh; changed = true; }
        if (breathingSpeed != _prev_breathingSpeed) { _prev_breathingSpeed = breathingSpeed; changed = true; }
        if (breathingAmount != _prev_breathingAmount) { _prev_breathingAmount = breathingAmount; changed = true; }
        return changed;
    }

    bool CheckEffect_LiquidWobble()
    {
        bool changed = false;
        if (liquidWobbleMaskCh != _prev_liquidWobbleMaskCh) { _prev_liquidWobbleMaskCh = liquidWobbleMaskCh; changed = true; }
        if (liquidWobbleSpeed != _prev_liquidWobbleSpeed) { _prev_liquidWobbleSpeed = liquidWobbleSpeed; changed = true; }
        if (liquidWobbleAmount != _prev_liquidWobbleAmount) { _prev_liquidWobbleAmount = liquidWobbleAmount; changed = true; }
        if (liquidWobbleFreq != _prev_liquidWobbleFreq) { _prev_liquidWobbleFreq = liquidWobbleFreq; changed = true; }
        return changed;
    }

    bool CheckEffect_FlipBookBlend()
    {
        bool changed = false;
        if (flipBookBlendMaskCh != _prev_flipBookBlendMaskCh) { _prev_flipBookBlendMaskCh = flipBookBlendMaskCh; changed = true; }
        if (flipBookNextFrame != _prev_flipBookNextFrame) { _prev_flipBookNextFrame = flipBookNextFrame; changed = true; }
        if (flipBookLerp != _prev_flipBookLerp) { _prev_flipBookLerp = flipBookLerp; changed = true; }
        return changed;
    }

    bool CheckEffect_SmoothPixelArt()
    {
        bool changed = false;
        if (smoothPixelArtMaskCh != _prev_smoothPixelArtMaskCh) { _prev_smoothPixelArtMaskCh = smoothPixelArtMaskCh; changed = true; }
        if (smoothPixelArtTexelSize != _prev_smoothPixelArtTexelSize) { _prev_smoothPixelArtTexelSize = smoothPixelArtTexelSize; changed = true; }
        return changed;
    }

    bool CheckEffect_Dissolve()
    {
        bool changed = false;
        if (dissolveMaskCh != _prev_dissolveMaskCh) { _prev_dissolveMaskCh = dissolveMaskCh; changed = true; }
        if (dissolveNoiseType != _prev_dissolveNoiseType) { _prev_dissolveNoiseType = dissolveNoiseType; changed = true; }
        if (dissolveTexture != _prev_dissolveTexture) { _prev_dissolveTexture = dissolveTexture; changed = true; }
        if (dissolveNoiseScale != _prev_dissolveNoiseScale) { _prev_dissolveNoiseScale = dissolveNoiseScale; changed = true; }
        if (dissolveAmount != _prev_dissolveAmount) { _prev_dissolveAmount = dissolveAmount; changed = true; }
        if (dissolveBorderWidth != _prev_dissolveBorderWidth) { _prev_dissolveBorderWidth = dissolveBorderWidth; changed = true; }
        if (dissolveBurnWidth != _prev_dissolveBurnWidth) { _prev_dissolveBurnWidth = dissolveBurnWidth; changed = true; }
        if (dissolveBurnColor != _prev_dissolveBurnColor) { _prev_dissolveBurnColor = dissolveBurnColor; changed = true; }
        if (dissolveTiling != _prev_dissolveTiling) { _prev_dissolveTiling = dissolveTiling; changed = true; }
        if (dissolveBorderColor != _prev_dissolveBorderColor) { _prev_dissolveBorderColor = dissolveBorderColor; changed = true; }
        return changed;
    }

    bool CheckEffect_GlowDissolve()
    {
        bool changed = false;
        if (glowDissolveMaskCh != _prev_glowDissolveMaskCh) { _prev_glowDissolveMaskCh = glowDissolveMaskCh; changed = true; }
        if (glowDissolveNoiseType != _prev_glowDissolveNoiseType) { _prev_glowDissolveNoiseType = glowDissolveNoiseType; changed = true; }
        if (glowDissolveTexture != _prev_glowDissolveTexture) { _prev_glowDissolveTexture = glowDissolveTexture; changed = true; }
        if (glowDissolveNoiseScale != _prev_glowDissolveNoiseScale) { _prev_glowDissolveNoiseScale = glowDissolveNoiseScale; changed = true; }
        if (glowDissolveAmount != _prev_glowDissolveAmount) { _prev_glowDissolveAmount = glowDissolveAmount; changed = true; }
        if (glowDissolveColor != _prev_glowDissolveColor) { _prev_glowDissolveColor = glowDissolveColor; changed = true; }
        if (glowDissolveWidth != _prev_glowDissolveWidth) { _prev_glowDissolveWidth = glowDissolveWidth; changed = true; }
        return changed;
    }

    bool CheckEffect_SrcDissolve()
    {
        bool changed = false;
        if (srcDissolveMaskCh != _prev_srcDissolveMaskCh) { _prev_srcDissolveMaskCh = srcDissolveMaskCh; changed = true; }
        if (srcDissolveAmount != _prev_srcDissolveAmount) { _prev_srcDissolveAmount = srcDissolveAmount; changed = true; }
        if (srcDissolvePosition != _prev_srcDissolvePosition) { _prev_srcDissolvePosition = srcDissolvePosition; changed = true; }
        if (srcDissolveBorderColor != _prev_srcDissolveBorderColor) { _prev_srcDissolveBorderColor = srcDissolveBorderColor; changed = true; }
        if (srcDissolveBorderWidth != _prev_srcDissolveBorderWidth) { _prev_srcDissolveBorderWidth = srcDissolveBorderWidth; changed = true; }
        return changed;
    }

    bool CheckEffect_DirFade()
    {
        bool changed = false;
        if (dirFadeMaskCh != _prev_dirFadeMaskCh) { _prev_dirFadeMaskCh = dirFadeMaskCh; changed = true; }
        if (dirFadeAngle != _prev_dirFadeAngle) { _prev_dirFadeAngle = dirFadeAngle; changed = true; }
        if (dirFadeAmount != _prev_dirFadeAmount) { _prev_dirFadeAmount = dirFadeAmount; changed = true; }
        if (dirFadeSmoothness != _prev_dirFadeSmoothness) { _prev_dirFadeSmoothness = dirFadeSmoothness; changed = true; }
        return changed;
    }

    bool CheckEffect_DirGlowFade()
    {
        bool changed = false;
        if (dirGlowFadeMaskCh != _prev_dirGlowFadeMaskCh) { _prev_dirGlowFadeMaskCh = dirGlowFadeMaskCh; changed = true; }
        if (dirGlowFadeAngle != _prev_dirGlowFadeAngle) { _prev_dirGlowFadeAngle = dirGlowFadeAngle; changed = true; }
        if (dirGlowFadeAmount != _prev_dirGlowFadeAmount) { _prev_dirGlowFadeAmount = dirGlowFadeAmount; changed = true; }
        if (dirGlowFadeColor != _prev_dirGlowFadeColor) { _prev_dirGlowFadeColor = dirGlowFadeColor; changed = true; }
        if (dirGlowFadeWidth != _prev_dirGlowFadeWidth) { _prev_dirGlowFadeWidth = dirGlowFadeWidth; changed = true; }
        return changed;
    }

    bool CheckEffect_FullAlphaDissolve()
    {
        bool changed = false;
        if (fullAlphaDissolveMaskCh != _prev_fullAlphaDissolveMaskCh) { _prev_fullAlphaDissolveMaskCh = fullAlphaDissolveMaskCh; changed = true; }
        if (fullAlphaDissolveAmount != _prev_fullAlphaDissolveAmount) { _prev_fullAlphaDissolveAmount = fullAlphaDissolveAmount; changed = true; }
        return changed;
    }

    bool CheckEffect_Halftone()
    {
        bool changed = false;
        if (halftoneMaskCh != _prev_halftoneMaskCh) { _prev_halftoneMaskCh = halftoneMaskCh; changed = true; }
        if (halftoneSize != _prev_halftoneSize) { _prev_halftoneSize = halftoneSize; changed = true; }
        if (halftoneAmount != _prev_halftoneAmount) { _prev_halftoneAmount = halftoneAmount; changed = true; }
        if (halftoneSmoothness != _prev_halftoneSmoothness) { _prev_halftoneSmoothness = halftoneSmoothness; changed = true; }
        return changed;
    }

    bool CheckEffect_RadialWipe()
    {
        bool changed = false;
        if (radialWipeMaskCh != _prev_radialWipeMaskCh) { _prev_radialWipeMaskCh = radialWipeMaskCh; changed = true; }
        if (radialWipeAmount != _prev_radialWipeAmount) { _prev_radialWipeAmount = radialWipeAmount; changed = true; }
        if (radialWipeSmoothness != _prev_radialWipeSmoothness) { _prev_radialWipeSmoothness = radialWipeSmoothness; changed = true; }
        if (radialWipeAngle != _prev_radialWipeAngle) { _prev_radialWipeAngle = radialWipeAngle; changed = true; }
        return changed;
    }

    bool CheckEffect_Blinds()
    {
        bool changed = false;
        if (blindsMaskCh != _prev_blindsMaskCh) { _prev_blindsMaskCh = blindsMaskCh; changed = true; }
        if (blindsAmount != _prev_blindsAmount) { _prev_blindsAmount = blindsAmount; changed = true; }
        if (blindsCount != _prev_blindsCount) { _prev_blindsCount = blindsCount; changed = true; }
        if (blindsDirection != _prev_blindsDirection) { _prev_blindsDirection = blindsDirection; changed = true; }
        return changed;
    }

    bool CheckEffect_DiamondDissolve()
    {
        bool changed = false;
        if (diamondDissolveMaskCh != _prev_diamondDissolveMaskCh) { _prev_diamondDissolveMaskCh = diamondDissolveMaskCh; changed = true; }
        if (diamondDissolveAmount != _prev_diamondDissolveAmount) { _prev_diamondDissolveAmount = diamondDissolveAmount; changed = true; }
        if (diamondDissolveScale != _prev_diamondDissolveScale) { _prev_diamondDissolveScale = diamondDissolveScale; changed = true; }
        return changed;
    }

    bool CheckEffect_ScanlineDissolve()
    {
        bool changed = false;
        if (scanlineDissolveMaskCh != _prev_scanlineDissolveMaskCh) { _prev_scanlineDissolveMaskCh = scanlineDissolveMaskCh; changed = true; }
        if (scanlineDissolveAmount != _prev_scanlineDissolveAmount) { _prev_scanlineDissolveAmount = scanlineDissolveAmount; changed = true; }
        if (scanlineDissolveDensity != _prev_scanlineDissolveDensity) { _prev_scanlineDissolveDensity = scanlineDissolveDensity; changed = true; }
        return changed;
    }

    bool CheckEffect_SpreadFade()
    {
        bool changed = false;
        if (spreadFadeMaskCh != _prev_spreadFadeMaskCh) { _prev_spreadFadeMaskCh = spreadFadeMaskCh; changed = true; }
        if (spreadFadeAmount != _prev_spreadFadeAmount) { _prev_spreadFadeAmount = spreadFadeAmount; changed = true; }
        if (spreadFadeOrigin != _prev_spreadFadeOrigin) { _prev_spreadFadeOrigin = spreadFadeOrigin; changed = true; }
        if (spreadFadeSmoothness != _prev_spreadFadeSmoothness) { _prev_spreadFadeSmoothness = spreadFadeSmoothness; changed = true; }
        return changed;
    }

    bool CheckEffect_PixelDissolve()
    {
        bool changed = false;
        if (pixelDissolveMaskCh != _prev_pixelDissolveMaskCh) { _prev_pixelDissolveMaskCh = pixelDissolveMaskCh; changed = true; }
        if (pixelDissolveAmount != _prev_pixelDissolveAmount) { _prev_pixelDissolveAmount = pixelDissolveAmount; changed = true; }
        if (pixelDissolveGrid != _prev_pixelDissolveGrid) { _prev_pixelDissolveGrid = pixelDissolveGrid; changed = true; }
        return changed;
    }

    bool CheckEffect_SpiralWipe()
    {
        bool changed = false;
        if (spiralWipeMaskCh != _prev_spiralWipeMaskCh) { _prev_spiralWipeMaskCh = spiralWipeMaskCh; changed = true; }
        if (spiralWipeAmount != _prev_spiralWipeAmount) { _prev_spiralWipeAmount = spiralWipeAmount; changed = true; }
        if (spiralWipeSmoothness != _prev_spiralWipeSmoothness) { _prev_spiralWipeSmoothness = spiralWipeSmoothness; changed = true; }
        if (spiralWipeArms != _prev_spiralWipeArms) { _prev_spiralWipeArms = spiralWipeArms; changed = true; }
        return changed;
    }

    bool CheckEffect_HexDissolve()
    {
        bool changed = false;
        if (hexDissolveMaskCh != _prev_hexDissolveMaskCh) { _prev_hexDissolveMaskCh = hexDissolveMaskCh; changed = true; }
        if (hexDissolveAmount != _prev_hexDissolveAmount) { _prev_hexDissolveAmount = hexDissolveAmount; changed = true; }
        if (hexDissolveScale != _prev_hexDissolveScale) { _prev_hexDissolveScale = hexDissolveScale; changed = true; }
        return changed;
    }

    bool CheckEffect_Hologram()
    {
        bool changed = false;
        if (hologramMaskCh != _prev_hologramMaskCh) { _prev_hologramMaskCh = hologramMaskCh; changed = true; }
        if (hologramStripesAmount != _prev_hologramStripesAmount) { _prev_hologramStripesAmount = hologramStripesAmount; changed = true; }
        if (hologramMinAlpha != _prev_hologramMinAlpha) { _prev_hologramMinAlpha = hologramMinAlpha; changed = true; }
        if (hologramStripesSpeed != _prev_hologramStripesSpeed) { _prev_hologramStripesSpeed = hologramStripesSpeed; changed = true; }
        if (hologramColor != _prev_hologramColor) { _prev_hologramColor = hologramColor; changed = true; }
        if (hologramBlend != _prev_hologramBlend) { _prev_hologramBlend = hologramBlend; changed = true; }
        if (hologramDistortion != _prev_hologramDistortion) { _prev_hologramDistortion = hologramDistortion; changed = true; }
        return changed;
    }

    bool CheckEffect_Glitch()
    {
        bool changed = false;
        if (glitchMaskCh != _prev_glitchMaskCh) { _prev_glitchMaskCh = glitchMaskCh; changed = true; }
        if (glitchAmount != _prev_glitchAmount) { _prev_glitchAmount = glitchAmount; changed = true; }
        if (glitchSpeed != _prev_glitchSpeed) { _prev_glitchSpeed = glitchSpeed; changed = true; }
        if (glitchSize != _prev_glitchSize) { _prev_glitchSize = glitchSize; changed = true; }
        if (glitchColorShift != _prev_glitchColorShift) { _prev_glitchColorShift = glitchColorShift; changed = true; }
        return changed;
    }

    bool CheckEffect_Flicker()
    {
        bool changed = false;
        if (flickerMaskCh != _prev_flickerMaskCh) { _prev_flickerMaskCh = flickerMaskCh; changed = true; }
        if (flickerFreq != _prev_flickerFreq) { _prev_flickerFreq = flickerFreq; changed = true; }
        if (flickerPercent != _prev_flickerPercent) { _prev_flickerPercent = flickerPercent; changed = true; }
        return changed;
    }

    bool CheckEffect_Shine()
    {
        bool changed = false;
        if (shineMaskCh != _prev_shineMaskCh) { _prev_shineMaskCh = shineMaskCh; changed = true; }
        if (shineColor != _prev_shineColor) { _prev_shineColor = shineColor; changed = true; }
        if (shineWidth != _prev_shineWidth) { _prev_shineWidth = shineWidth; changed = true; }
        if (shineSpeed != _prev_shineSpeed) { _prev_shineSpeed = shineSpeed; changed = true; }
        if (shineDelay != _prev_shineDelay) { _prev_shineDelay = shineDelay; changed = true; }
        if (shineRotation != _prev_shineRotation) { _prev_shineRotation = shineRotation; changed = true; }
        if (shineGlow != _prev_shineGlow) { _prev_shineGlow = shineGlow; changed = true; }
        return changed;
    }

    bool CheckEffect_Smoke()
    {
        bool changed = false;
        if (smokeMaskCh != _prev_smokeMaskCh) { _prev_smokeMaskCh = smokeMaskCh; changed = true; }
        if (smokeColor != _prev_smokeColor) { _prev_smokeColor = smokeColor; changed = true; }
        if (smokeDensity != _prev_smokeDensity) { _prev_smokeDensity = smokeDensity; changed = true; }
        if (smokeSpeed != _prev_smokeSpeed) { _prev_smokeSpeed = smokeSpeed; changed = true; }
        if (smokeTurbulence != _prev_smokeTurbulence) { _prev_smokeTurbulence = smokeTurbulence; changed = true; }
        if (smokeDirection != _prev_smokeDirection) { _prev_smokeDirection = smokeDirection; changed = true; }
        if (smokeColorStrength != _prev_smokeColorStrength) { _prev_smokeColorStrength = smokeColorStrength; changed = true; }
        if (smokeFade != _prev_smokeFade) { _prev_smokeFade = smokeFade; changed = true; }
        return changed;
    }

    bool CheckEffect_Flame()
    {
        bool changed = false;
        if (flameMaskCh != _prev_flameMaskCh) { _prev_flameMaskCh = flameMaskCh; changed = true; }
        if (flameColor1 != _prev_flameColor1) { _prev_flameColor1 = flameColor1; changed = true; }
        if (flameColor2 != _prev_flameColor2) { _prev_flameColor2 = flameColor2; changed = true; }
        if (flameSpeed != _prev_flameSpeed) { _prev_flameSpeed = flameSpeed; changed = true; }
        if (flameDensity != _prev_flameDensity) { _prev_flameDensity = flameDensity; changed = true; }
        if (flameOnlyMode != _prev_flameOnlyMode) { _prev_flameOnlyMode = flameOnlyMode; changed = true; }
        if (flameExtendSides != _prev_flameExtendSides) { _prev_flameExtendSides = flameExtendSides; changed = true; }
        if (flameSideFalloff != _prev_flameSideFalloff) { _prev_flameSideFalloff = flameSideFalloff; changed = true; }
        if (flameIntensity != _prev_flameIntensity) { _prev_flameIntensity = flameIntensity; changed = true; }
        return changed;
    }

    bool CheckEffect_Enchanted()
    {
        bool changed = false;
        if (enchantedMaskCh != _prev_enchantedMaskCh) { _prev_enchantedMaskCh = enchantedMaskCh; changed = true; }
        if (enchantedColor != _prev_enchantedColor) { _prev_enchantedColor = enchantedColor; changed = true; }
        if (enchantedSpeed != _prev_enchantedSpeed) { _prev_enchantedSpeed = enchantedSpeed; changed = true; }
        if (enchantedDensity != _prev_enchantedDensity) { _prev_enchantedDensity = enchantedDensity; changed = true; }
        if (enchantedBrightness != _prev_enchantedBrightness) { _prev_enchantedBrightness = enchantedBrightness; changed = true; }
        if (enchantedFade != _prev_enchantedFade) { _prev_enchantedFade = enchantedFade; changed = true; }
        return changed;
    }

    bool CheckEffect_Shifting()
    {
        bool changed = false;
        if (shiftingMaskCh != _prev_shiftingMaskCh) { _prev_shiftingMaskCh = shiftingMaskCh; changed = true; }
        if (shiftingColor1 != _prev_shiftingColor1) { _prev_shiftingColor1 = shiftingColor1; changed = true; }
        if (shiftingColor2 != _prev_shiftingColor2) { _prev_shiftingColor2 = shiftingColor2; changed = true; }
        if (shiftingSpeed != _prev_shiftingSpeed) { _prev_shiftingSpeed = shiftingSpeed; changed = true; }
        if (shiftingFade != _prev_shiftingFade) { _prev_shiftingFade = shiftingFade; changed = true; }
        return changed;
    }

    bool CheckEffect_ShiftRainbow()
    {
        bool changed = false;
        if (shiftRainbowMaskCh != _prev_shiftRainbowMaskCh) { _prev_shiftRainbowMaskCh = shiftRainbowMaskCh; changed = true; }
        if (shiftRainbowSpeed != _prev_shiftRainbowSpeed) { _prev_shiftRainbowSpeed = shiftRainbowSpeed; changed = true; }
        if (shiftRainbowDensity != _prev_shiftRainbowDensity) { _prev_shiftRainbowDensity = shiftRainbowDensity; changed = true; }
        if (shiftRainbowFade != _prev_shiftRainbowFade) { _prev_shiftRainbowFade = shiftRainbowFade; changed = true; }
        return changed;
    }

    bool CheckEffect_Overlay()
    {
        bool changed = false;
        if (overlayMaskCh != _prev_overlayMaskCh) { _prev_overlayMaskCh = overlayMaskCh; changed = true; }
        if (overlayTexture != _prev_overlayTexture) { _prev_overlayTexture = overlayTexture; changed = true; }
        if (overlayColor != _prev_overlayColor) { _prev_overlayColor = overlayColor; changed = true; }
        if (overlayBlend != _prev_overlayBlend) { _prev_overlayBlend = overlayBlend; changed = true; }
        if (overlayTiling != _prev_overlayTiling) { _prev_overlayTiling = overlayTiling; changed = true; }
        if (overlaySpeed != _prev_overlaySpeed) { _prev_overlaySpeed = overlaySpeed; changed = true; }
        return changed;
    }

    bool CheckEffect_Checkerboard()
    {
        bool changed = false;
        if (checkerboardMaskCh != _prev_checkerboardMaskCh) { _prev_checkerboardMaskCh = checkerboardMaskCh; changed = true; }
        if (checkerCoordSpace != _prev_checkerCoordSpace) { _prev_checkerCoordSpace = checkerCoordSpace; changed = true; }
        if (checkerScale != _prev_checkerScale) { _prev_checkerScale = checkerScale; changed = true; }
        if (checkerColor1 != _prev_checkerColor1) { _prev_checkerColor1 = checkerColor1; changed = true; }
        if (checkerColor2 != _prev_checkerColor2) { _prev_checkerColor2 = checkerColor2; changed = true; }
        return changed;
    }

    bool CheckEffect_UVVisualize()
    {
        bool changed = false;
        if (uvVisualizeMaskCh != _prev_uvVisualizeMaskCh) { _prev_uvVisualizeMaskCh = uvVisualizeMaskCh; changed = true; }
        if (uvVisualizeBlend != _prev_uvVisualizeBlend) { _prev_uvVisualizeBlend = uvVisualizeBlend; changed = true; }
        return changed;
    }

    bool CheckEffect_HealthTint()
    {
        bool changed = false;
        if (healthTintMaskCh != _prev_healthTintMaskCh) { _prev_healthTintMaskCh = healthTintMaskCh; changed = true; }
        if (healthTintLowColor != _prev_healthTintLowColor) { _prev_healthTintLowColor = healthTintLowColor; changed = true; }
        if (healthTintHighColor != _prev_healthTintHighColor) { _prev_healthTintHighColor = healthTintHighColor; changed = true; }
        if (healthTintAmount != _prev_healthTintAmount) { _prev_healthTintAmount = healthTintAmount; changed = true; }
        if (healthDamageNoise != _prev_healthDamageNoise) { _prev_healthDamageNoise = healthDamageNoise; changed = true; }
        if (healthEdgeGrain != _prev_healthEdgeGrain) { _prev_healthEdgeGrain = healthEdgeGrain; changed = true; }
        if (healthPulseSpeed != _prev_healthPulseSpeed) { _prev_healthPulseSpeed = healthPulseSpeed; changed = true; }
        if (healthVignetteStr != _prev_healthVignetteStr) { _prev_healthVignetteStr = healthVignetteStr; changed = true; }
        if (healthDesaturation != _prev_healthDesaturation) { _prev_healthDesaturation = healthDesaturation; changed = true; }
        return changed;
    }

    bool CheckEffect_XRaySilhouette()
    {
        bool changed = false;
        if (xraySilhouetteMaskCh != _prev_xraySilhouetteMaskCh) { _prev_xraySilhouetteMaskCh = xraySilhouetteMaskCh; changed = true; }
        if (xraySilhouetteColor != _prev_xraySilhouetteColor) { _prev_xraySilhouetteColor = xraySilhouetteColor; changed = true; }
        if (xraySilhouetteBlend != _prev_xraySilhouetteBlend) { _prev_xraySilhouetteBlend = xraySilhouetteBlend; changed = true; }
        return changed;
    }

    bool CheckEffect_MinimapPing()
    {
        bool changed = false;
        if (minimapPingMaskCh != _prev_minimapPingMaskCh) { _prev_minimapPingMaskCh = minimapPingMaskCh; changed = true; }
        if (minimapPingColor != _prev_minimapPingColor) { _prev_minimapPingColor = minimapPingColor; changed = true; }
        if (minimapPingSpeed != _prev_minimapPingSpeed) { _prev_minimapPingSpeed = minimapPingSpeed; changed = true; }
        if (minimapPingWidth != _prev_minimapPingWidth) { _prev_minimapPingWidth = minimapPingWidth; changed = true; }
        if (minimapPingOffset != _prev_minimapPingOffset) { _prev_minimapPingOffset = minimapPingOffset; changed = true; }
        return changed;
    }

    bool CheckEffect_DialogueEmphasis()
    {
        bool changed = false;
        if (dialogueEmphasisMaskCh != _prev_dialogueEmphasisMaskCh) { _prev_dialogueEmphasisMaskCh = dialogueEmphasisMaskCh; changed = true; }
        if (dialogueBobSpeed != _prev_dialogueBobSpeed) { _prev_dialogueBobSpeed = dialogueBobSpeed; changed = true; }
        if (dialogueBobAmount != _prev_dialogueBobAmount) { _prev_dialogueBobAmount = dialogueBobAmount; changed = true; }
        if (dialogueBrightness != _prev_dialogueBrightness) { _prev_dialogueBrightness = dialogueBrightness; changed = true; }
        return changed;
    }

    bool CheckEffect_VoronoiCrackle()
    {
        bool changed = false;
        if (crackleMaskCh != _prev_crackleMaskCh) { _prev_crackleMaskCh = crackleMaskCh; changed = true; }
        if (crackleScale != _prev_crackleScale) { _prev_crackleScale = crackleScale; changed = true; }
        if (crackleSpeed != _prev_crackleSpeed) { _prev_crackleSpeed = crackleSpeed; changed = true; }
        if (crackleEdgeWidth != _prev_crackleEdgeWidth) { _prev_crackleEdgeWidth = crackleEdgeWidth; changed = true; }
        if (crackleEdgeColor != _prev_crackleEdgeColor) { _prev_crackleEdgeColor = crackleEdgeColor; changed = true; }
        if (crackleCellTint != _prev_crackleCellTint) { _prev_crackleCellTint = crackleCellTint; changed = true; }
        if (crackleFade != _prev_crackleFade) { _prev_crackleFade = crackleFade; changed = true; }
        return changed;
    }

    bool CheckEffect_WaterReflect()
    {
        bool changed = false;
        if (waterReflectMaskCh != _prev_waterReflectMaskCh) { _prev_waterReflectMaskCh = waterReflectMaskCh; changed = true; }
        if (waterReflectTint != _prev_waterReflectTint) { _prev_waterReflectTint = waterReflectTint; changed = true; }
        if (waterReflectYPoint != _prev_waterReflectYPoint) { _prev_waterReflectYPoint = waterReflectYPoint; changed = true; }
        if (waterReflectDepth != _prev_waterReflectDepth) { _prev_waterReflectDepth = waterReflectDepth; changed = true; }
        if (waterReflectOpacity != _prev_waterReflectOpacity) { _prev_waterReflectOpacity = waterReflectOpacity; changed = true; }
        if (waterReflectTintAmount != _prev_waterReflectTintAmount) { _prev_waterReflectTintAmount = waterReflectTintAmount; changed = true; }
        if (waterReflectWaveFreq != _prev_waterReflectWaveFreq) { _prev_waterReflectWaveFreq = waterReflectWaveFreq; changed = true; }
        if (waterReflectWaveSpeed != _prev_waterReflectWaveSpeed) { _prev_waterReflectWaveSpeed = waterReflectWaveSpeed; changed = true; }
        if (waterReflectWaveAmp != _prev_waterReflectWaveAmp) { _prev_waterReflectWaveAmp = waterReflectWaveAmp; changed = true; }
        if (waterReflectFade != _prev_waterReflectFade) { _prev_waterReflectFade = waterReflectFade; changed = true; }
        return changed;
    }

    bool CheckEffect_MotionSmear()
    {
        bool changed = false;
        if (smearMaskCh != _prev_smearMaskCh) { _prev_smearMaskCh = smearMaskCh; changed = true; }
        if (smearDirection != _prev_smearDirection) { _prev_smearDirection = smearDirection; changed = true; }
        if (smearAmount != _prev_smearAmount) { _prev_smearAmount = smearAmount; changed = true; }
        if (smearSamples != _prev_smearSamples) { _prev_smearSamples = smearSamples; changed = true; }
        if (smearFade != _prev_smearFade) { _prev_smearFade = smearFade; changed = true; }
        return changed;
    }

    bool CheckEffect_EnergyAura()
    {
        bool changed = false;
        if (auraMaskCh != _prev_auraMaskCh) { _prev_auraMaskCh = auraMaskCh; changed = true; }
        if (auraColor != _prev_auraColor) { _prev_auraColor = auraColor; changed = true; }
        if (auraRadius != _prev_auraRadius) { _prev_auraRadius = auraRadius; changed = true; }
        if (auraFrequency != _prev_auraFrequency) { _prev_auraFrequency = auraFrequency; changed = true; }
        if (auraSpeed != _prev_auraSpeed) { _prev_auraSpeed = auraSpeed; changed = true; }
        if (auraIntensity != _prev_auraIntensity) { _prev_auraIntensity = auraIntensity; changed = true; }
        if (auraFade != _prev_auraFade) { _prev_auraFade = auraFade; changed = true; }
        return changed;
    }

    bool CheckEffect_Gradient()
    {
        bool changed = false;
        if (gradientMaskCh != _prev_gradientMaskCh) { _prev_gradientMaskCh = gradientMaskCh; changed = true; }
        if (gradientColorA != _prev_gradientColorA) { _prev_gradientColorA = gradientColorA; changed = true; }
        if (gradientColorB != _prev_gradientColorB) { _prev_gradientColorB = gradientColorB; changed = true; }
        if (gradientAngle != _prev_gradientAngle) { _prev_gradientAngle = gradientAngle; changed = true; }
        if (gradientFade != _prev_gradientFade) { _prev_gradientFade = gradientFade; changed = true; }
        if (gradientUseGradientEditor != _prev_gradientUseGradientEditor) { _prev_gradientUseGradientEditor = gradientUseGradientEditor; changed = true; }
        if (gradientBakedTexture != _prev_gradientBakedTexture) { _prev_gradientBakedTexture = gradientBakedTexture; changed = true; }
        if (gradientBlendMode != _prev_gradientBlendMode) { _prev_gradientBlendMode = gradientBlendMode; changed = true; }
        return changed;
    }

    bool CheckEffect_RadialGradient()
    {
        bool changed = false;
        if (radialGradMaskCh != _prev_radialGradMaskCh) { _prev_radialGradMaskCh = radialGradMaskCh; changed = true; }
        if (radialGradCenterColor != _prev_radialGradCenterColor) { _prev_radialGradCenterColor = radialGradCenterColor; changed = true; }
        if (radialGradEdgeColor != _prev_radialGradEdgeColor) { _prev_radialGradEdgeColor = radialGradEdgeColor; changed = true; }
        if (radialGradCenter != _prev_radialGradCenter) { _prev_radialGradCenter = radialGradCenter; changed = true; }
        if (radialGradRadius != _prev_radialGradRadius) { _prev_radialGradRadius = radialGradRadius; changed = true; }
        if (radialGradFade != _prev_radialGradFade) { _prev_radialGradFade = radialGradFade; changed = true; }
        if (radialGradUseGradientEditor != _prev_radialGradUseGradientEditor) { _prev_radialGradUseGradientEditor = radialGradUseGradientEditor; changed = true; }
        if (radialGradBakedTexture != _prev_radialGradBakedTexture) { _prev_radialGradBakedTexture = radialGradBakedTexture; changed = true; }
        if (radialGradBlendMode != _prev_radialGradBlendMode) { _prev_radialGradBlendMode = radialGradBlendMode; changed = true; }
        return changed;
    }

    bool CheckEffect_ColorRamp()
    {
        bool changed = false;
        if (colorRampMaskCh != _prev_colorRampMaskCh) { _prev_colorRampMaskCh = colorRampMaskCh; changed = true; }
        if (colorRampTexture != _prev_colorRampTexture) { _prev_colorRampTexture = colorRampTexture; changed = true; }
        if (colorRampMode != _prev_colorRampMode) { _prev_colorRampMode = colorRampMode; changed = true; }
        if (colorRampIntensity != _prev_colorRampIntensity) { _prev_colorRampIntensity = colorRampIntensity; changed = true; }
        if (colorRampColorA != _prev_colorRampColorA) { _prev_colorRampColorA = colorRampColorA; changed = true; }
        if (colorRampColorB != _prev_colorRampColorB) { _prev_colorRampColorB = colorRampColorB; changed = true; }
        if (colorRampFade != _prev_colorRampFade) { _prev_colorRampFade = colorRampFade; changed = true; }
        return changed;
    }

    bool CheckEffect_FakeLighting()
    {
        bool changed = false;
        if (fakeLightMaskCh != _prev_fakeLightMaskCh) { _prev_fakeLightMaskCh = fakeLightMaskCh; changed = true; }
        if (fakeLightDirection != _prev_fakeLightDirection) { _prev_fakeLightDirection = fakeLightDirection; changed = true; }
        if (fakeLightColor != _prev_fakeLightColor) { _prev_fakeLightColor = fakeLightColor; changed = true; }
        if (fakeLightIntensity != _prev_fakeLightIntensity) { _prev_fakeLightIntensity = fakeLightIntensity; changed = true; }
        if (fakeLightAmbient != _prev_fakeLightAmbient) { _prev_fakeLightAmbient = fakeLightAmbient; changed = true; }
        if (fakeLightNormalDist != _prev_fakeLightNormalDist) { _prev_fakeLightNormalDist = fakeLightNormalDist; changed = true; }
        if (fakeLightNormalZ != _prev_fakeLightNormalZ) { _prev_fakeLightNormalZ = fakeLightNormalZ; changed = true; }
        if (fakeLightSpecPower != _prev_fakeLightSpecPower) { _prev_fakeLightSpecPower = fakeLightSpecPower; changed = true; }
        if (fakeLightSpecIntensity != _prev_fakeLightSpecIntensity) { _prev_fakeLightSpecIntensity = fakeLightSpecIntensity; changed = true; }
        if (fakeLightFade != _prev_fakeLightFade) { _prev_fakeLightFade = fakeLightFade; changed = true; }
        return changed;
    }

    bool CheckEffect_UVBendPoint()
    {
        bool changed = false;
        if (bendMaskCh != _prev_bendMaskCh) { _prev_bendMaskCh = bendMaskCh; changed = true; }
        if (bendLocalPoint != _prev_bendLocalPoint) { _prev_bendLocalPoint = bendLocalPoint; changed = true; }
        if (bendWorldPoint != _prev_bendWorldPoint) { _prev_bendWorldPoint = bendWorldPoint; changed = true; }
        if (bendUseWorldSpace != _prev_bendUseWorldSpace) { _prev_bendUseWorldSpace = bendUseWorldSpace; changed = true; }
        if (bendWorldScale != _prev_bendWorldScale) { _prev_bendWorldScale = bendWorldScale; changed = true; }
        if (bendRadius != _prev_bendRadius) { _prev_bendRadius = bendRadius; changed = true; }
        if (bendAmount != _prev_bendAmount) { _prev_bendAmount = bendAmount; changed = true; }
        if (bendFalloff != _prev_bendFalloff) { _prev_bendFalloff = bendFalloff; changed = true; }
        if (bendInvert != _prev_bendInvert) { _prev_bendInvert = bendInvert; changed = true; }
        return changed;
    }

    bool CheckEffect_BurnDirection()
    {
        bool changed = false;
        if (burnDirMaskCh != _prev_burnDirMaskCh) { _prev_burnDirMaskCh = burnDirMaskCh; changed = true; }
        if (burnDirAngle != _prev_burnDirAngle) { _prev_burnDirAngle = burnDirAngle; changed = true; }
        if (burnDirAmount != _prev_burnDirAmount) { _prev_burnDirAmount = burnDirAmount; changed = true; }
        if (burnDirEdgeWidth != _prev_burnDirEdgeWidth) { _prev_burnDirEdgeWidth = burnDirEdgeWidth; changed = true; }
        if (burnDirFireColor != _prev_burnDirFireColor) { _prev_burnDirFireColor = burnDirFireColor; changed = true; }
        if (burnDirCharcoalColor != _prev_burnDirCharcoalColor) { _prev_burnDirCharcoalColor = burnDirCharcoalColor; changed = true; }
        if (burnDirCharcoalWidth != _prev_burnDirCharcoalWidth) { _prev_burnDirCharcoalWidth = burnDirCharcoalWidth; changed = true; }
        if (burnDirNoiseScale != _prev_burnDirNoiseScale) { _prev_burnDirNoiseScale = burnDirNoiseScale; changed = true; }
        return changed;
    }

    bool CheckEffect_GlobalSmoke()
    {
        bool changed = false;
        if (globalSmokeMaskCh != _prev_globalSmokeMaskCh) { _prev_globalSmokeMaskCh = globalSmokeMaskCh; changed = true; }
        if (globalSmokeColor != _prev_globalSmokeColor) { _prev_globalSmokeColor = globalSmokeColor; changed = true; }
        if (globalSmokeDensity != _prev_globalSmokeDensity) { _prev_globalSmokeDensity = globalSmokeDensity; changed = true; }
        if (globalSmokeSpeed != _prev_globalSmokeSpeed) { _prev_globalSmokeSpeed = globalSmokeSpeed; changed = true; }
        if (globalSmokeOpacity != _prev_globalSmokeOpacity) { _prev_globalSmokeOpacity = globalSmokeOpacity; changed = true; }
        if (globalSmokeDirection != _prev_globalSmokeDirection) { _prev_globalSmokeDirection = globalSmokeDirection; changed = true; }
        if (globalSmokeOverlayOnly != _prev_globalSmokeOverlayOnly) { _prev_globalSmokeOverlayOnly = globalSmokeOverlayOnly; changed = true; }
        return changed;
    }

    bool CheckEffect_HolographicCard()
    {
        bool changed = false;
        if (holoCardMaskCh != _prev_holoCardMaskCh) { _prev_holoCardMaskCh = holoCardMaskCh; changed = true; }
        if (holoCardDensity != _prev_holoCardDensity) { _prev_holoCardDensity = holoCardDensity; changed = true; }
        if (holoCardSpeed != _prev_holoCardSpeed) { _prev_holoCardSpeed = holoCardSpeed; changed = true; }
        if (holoCardSaturation != _prev_holoCardSaturation) { _prev_holoCardSaturation = holoCardSaturation; changed = true; }
        if (holoCardBrightness != _prev_holoCardBrightness) { _prev_holoCardBrightness = holoCardBrightness; changed = true; }
        if (holoCardFresnelPower != _prev_holoCardFresnelPower) { _prev_holoCardFresnelPower = holoCardFresnelPower; changed = true; }
        if (holoCardFresnelShift != _prev_holoCardFresnelShift) { _prev_holoCardFresnelShift = holoCardFresnelShift; changed = true; }
        if (holoCardScanDensity != _prev_holoCardScanDensity) { _prev_holoCardScanDensity = holoCardScanDensity; changed = true; }
        if (holoCardScanSpeed != _prev_holoCardScanSpeed) { _prev_holoCardScanSpeed = holoCardScanSpeed; changed = true; }
        if (holoCardScanIntensity != _prev_holoCardScanIntensity) { _prev_holoCardScanIntensity = holoCardScanIntensity; changed = true; }
        if (holoCardFade != _prev_holoCardFade) { _prev_holoCardFade = holoCardFade; changed = true; }
        return changed;
    }

    bool CheckEffect_CloudShadows()
    {
        bool changed = false;
        if (cloudShadowMaskCh != _prev_cloudShadowMaskCh) { _prev_cloudShadowMaskCh = cloudShadowMaskCh; changed = true; }
        if (cloudShadowScale != _prev_cloudShadowScale) { _prev_cloudShadowScale = cloudShadowScale; changed = true; }
        if (cloudShadowSpeed != _prev_cloudShadowSpeed) { _prev_cloudShadowSpeed = cloudShadowSpeed; changed = true; }
        if (cloudShadowDarkness != _prev_cloudShadowDarkness) { _prev_cloudShadowDarkness = cloudShadowDarkness; changed = true; }
        if (cloudShadowThreshold != _prev_cloudShadowThreshold) { _prev_cloudShadowThreshold = cloudShadowThreshold; changed = true; }
        if (cloudShadowSoftness != _prev_cloudShadowSoftness) { _prev_cloudShadowSoftness = cloudShadowSoftness; changed = true; }
        if (cloudShadowFade != _prev_cloudShadowFade) { _prev_cloudShadowFade = cloudShadowFade; changed = true; }
        if (cloudShadowOverlayOnly != _prev_cloudShadowOverlayOnly) { _prev_cloudShadowOverlayOnly = cloudShadowOverlayOnly; changed = true; }
        return changed;
    }

    bool CheckEffect_ColorfulTrailA()
    {
        bool changed = false;
        if (trailAMaskCh != _prev_trailAMaskCh) { _prev_trailAMaskCh = trailAMaskCh; changed = true; }
        if (trailAVelocity != _prev_trailAVelocity) { _prev_trailAVelocity = trailAVelocity; changed = true; }
        if (trailASlowColor != _prev_trailASlowColor) { _prev_trailASlowColor = trailASlowColor; changed = true; }
        if (trailAFastColor != _prev_trailAFastColor) { _prev_trailAFastColor = trailAFastColor; changed = true; }
        if (trailAMaxSpeed != _prev_trailAMaxSpeed) { _prev_trailAMaxSpeed = trailAMaxSpeed; changed = true; }
        if (trailACurvePower != _prev_trailACurvePower) { _prev_trailACurvePower = trailACurvePower; changed = true; }
        if (trailAIntensity != _prev_trailAIntensity) { _prev_trailAIntensity = trailAIntensity; changed = true; }
        return changed;
    }

    bool CheckEffect_ColorfulTrailB()
    {
        bool changed = false;
        if (trailBMaskCh != _prev_trailBMaskCh) { _prev_trailBMaskCh = trailBMaskCh; changed = true; }
        if (trailBVelocity != _prev_trailBVelocity) { _prev_trailBVelocity = trailBVelocity; changed = true; }
        if (trailBColor != _prev_trailBColor) { _prev_trailBColor = trailBColor; changed = true; }
        if (trailBMaxSpeed != _prev_trailBMaxSpeed) { _prev_trailBMaxSpeed = trailBMaxSpeed; changed = true; }
        if (trailBCurvePower != _prev_trailBCurvePower) { _prev_trailBCurvePower = trailBCurvePower; changed = true; }
        if (trailBIntensity != _prev_trailBIntensity) { _prev_trailBIntensity = trailBIntensity; changed = true; }
        if (trailBSegments != _prev_trailBSegments) { _prev_trailBSegments = trailBSegments; changed = true; }
        if (trailBFadeOut != _prev_trailBFadeOut) { _prev_trailBFadeOut = trailBFadeOut; changed = true; }
        return changed;
    }

    bool CheckEffect_ColorfulTrailC()
    {
        bool changed = false;
        if (trailCMaskCh != _prev_trailCMaskCh) { _prev_trailCMaskCh = trailCMaskCh; changed = true; }
        if (trailCVelocity != _prev_trailCVelocity) { _prev_trailCVelocity = trailCVelocity; changed = true; }
        if (trailCColorA != _prev_trailCColorA) { _prev_trailCColorA = trailCColorA; changed = true; }
        if (trailCColorB != _prev_trailCColorB) { _prev_trailCColorB = trailCColorB; changed = true; }
        if (trailCMaxSpeed != _prev_trailCMaxSpeed) { _prev_trailCMaxSpeed = trailCMaxSpeed; changed = true; }
        if (trailCCurvePower != _prev_trailCCurvePower) { _prev_trailCCurvePower = trailCCurvePower; changed = true; }
        if (trailCIntensity != _prev_trailCIntensity) { _prev_trailCIntensity = trailCIntensity; changed = true; }
        if (trailCRainbowSpeed != _prev_trailCRainbowSpeed) { _prev_trailCRainbowSpeed = trailCRainbowSpeed; changed = true; }
        return changed;
    }

    bool CheckEffect_ChargingAbility()
    {
        bool changed = false;
        if (chargeMaskCh != _prev_chargeMaskCh) { _prev_chargeMaskCh = chargeMaskCh; changed = true; }
        if (chargeColor != _prev_chargeColor) { _prev_chargeColor = chargeColor; changed = true; }
        if (chargeCenter != _prev_chargeCenter) { _prev_chargeCenter = chargeCenter; changed = true; }
        if (chargeRadius != _prev_chargeRadius) { _prev_chargeRadius = chargeRadius; changed = true; }
        if (chargeWidth != _prev_chargeWidth) { _prev_chargeWidth = chargeWidth; changed = true; }
        if (chargeSpeed != _prev_chargeSpeed) { _prev_chargeSpeed = chargeSpeed; changed = true; }
        if (chargeFrequency != _prev_chargeFrequency) { _prev_chargeFrequency = chargeFrequency; changed = true; }
        if (chargeDensity != _prev_chargeDensity) { _prev_chargeDensity = chargeDensity; changed = true; }
        if (chargeIntensity != _prev_chargeIntensity) { _prev_chargeIntensity = chargeIntensity; changed = true; }
        if (chargeLines != _prev_chargeLines) { _prev_chargeLines = chargeLines; changed = true; }
        if (chargeFade != _prev_chargeFade) { _prev_chargeFade = chargeFade; changed = true; }
        return changed;
    }

    bool CheckEffect_Blueprint()
    {
        bool changed = false;
        if (blueprintMaskCh != _prev_blueprintMaskCh) { _prev_blueprintMaskCh = blueprintMaskCh; changed = true; }
        if (blueprintBgColor != _prev_blueprintBgColor) { _prev_blueprintBgColor = blueprintBgColor; changed = true; }
        if (blueprintLineColor != _prev_blueprintLineColor) { _prev_blueprintLineColor = blueprintLineColor; changed = true; }
        if (blueprintGridColor != _prev_blueprintGridColor) { _prev_blueprintGridColor = blueprintGridColor; changed = true; }
        if (blueprintEdgeSize != _prev_blueprintEdgeSize) { _prev_blueprintEdgeSize = blueprintEdgeSize; changed = true; }
        if (blueprintLineWidth != _prev_blueprintLineWidth) { _prev_blueprintLineWidth = blueprintLineWidth; changed = true; }
        if (blueprintLineIntensity != _prev_blueprintLineIntensity) { _prev_blueprintLineIntensity = blueprintLineIntensity; changed = true; }
        if (blueprintGridDensity != _prev_blueprintGridDensity) { _prev_blueprintGridDensity = blueprintGridDensity; changed = true; }
        if (blueprintGridOpacity != _prev_blueprintGridOpacity) { _prev_blueprintGridOpacity = blueprintGridOpacity; changed = true; }
        if (blueprintFade != _prev_blueprintFade) { _prev_blueprintFade = blueprintFade; changed = true; }
        return changed;
    }

    bool CheckEffect_Ghosting()
    {
        bool changed = false;
        if (ghostingMaskCh != _prev_ghostingMaskCh) { _prev_ghostingMaskCh = ghostingMaskCh; changed = true; }
        if (ghostingCount != _prev_ghostingCount) { _prev_ghostingCount = ghostingCount; changed = true; }
        if (ghostingOffset != _prev_ghostingOffset) { _prev_ghostingOffset = ghostingOffset; changed = true; }
        if (ghostingFade != _prev_ghostingFade) { _prev_ghostingFade = ghostingFade; changed = true; }
        return changed;
    }

    bool CheckEffect_RetroPalette()
    {
        bool changed = false;
        if (retroMaskCh != _prev_retroMaskCh) { _prev_retroMaskCh = retroMaskCh; changed = true; }
        if (retroPixelSize != _prev_retroPixelSize) { _prev_retroPixelSize = retroPixelSize; changed = true; }
        if (retroPaletteSize != _prev_retroPaletteSize) { _prev_retroPaletteSize = retroPaletteSize; changed = true; }
        if (retroPaletteTex != _prev_retroPaletteTex) { _prev_retroPaletteTex = retroPaletteTex; changed = true; }
        if (retroDitherStrength != _prev_retroDitherStrength) { _prev_retroDitherStrength = retroDitherStrength; changed = true; }
        if (retroFade != _prev_retroFade) { _prev_retroFade = retroFade; changed = true; }
        return changed;
    }

    void CheckEnabledEffectsForChanges()
    {
        if (strongTintEnabled != _prev_strongTintEnabled) { _prev_strongTintEnabled = strongTintEnabled; _dirty = true; }
        if (addColorEnabled != _prev_addColorEnabled) { _prev_addColorEnabled = addColorEnabled; _dirty = true; }
        if (alphaTintEnabled != _prev_alphaTintEnabled) { _prev_alphaTintEnabled = alphaTintEnabled; _dirty = true; }
        if (hitEffectEnabled != _prev_hitEffectEnabled) { _prev_hitEffectEnabled = hitEffectEnabled; _dirty = true; }
        if (negativeEnabled != _prev_negativeEnabled) { _prev_negativeEnabled = negativeEnabled; _dirty = true; }
        if (greyscaleEnabled != _prev_greyscaleEnabled) { _prev_greyscaleEnabled = greyscaleEnabled; _dirty = true; }
        if (bloomEnabled != _prev_bloomEnabled) { _prev_bloomEnabled = bloomEnabled; _dirty = true; }
        if (colorReplaceEnabled != _prev_colorReplaceEnabled) { _prev_colorReplaceEnabled = colorReplaceEnabled; _dirty = true; }
        if (lutReplaceEnabled != _prev_lutReplaceEnabled) { _prev_lutReplaceEnabled = lutReplaceEnabled; _dirty = true; }
        if (hueShiftEnabled != _prev_hueShiftEnabled) { _prev_hueShiftEnabled = hueShiftEnabled; _dirty = true; }
        if (addHueEnabled != _prev_addHueEnabled) { _prev_addHueEnabled = addHueEnabled; _dirty = true; }
        if (saturationEnabled != _prev_saturationEnabled) { _prev_saturationEnabled = saturationEnabled; _dirty = true; }
        if (brightnessEnabled != _prev_brightnessEnabled) { _prev_brightnessEnabled = brightnessEnabled; _dirty = true; }
        if (contrastEnabled != _prev_contrastEnabled) { _prev_contrastEnabled = contrastEnabled; _dirty = true; }
        if (splitToningEnabled != _prev_splitToningEnabled) { _prev_splitToningEnabled = splitToningEnabled; _dirty = true; }
        if (posterizeEnabled != _prev_posterizeEnabled) { _prev_posterizeEnabled = posterizeEnabled; _dirty = true; }
        if (poisonEnabled != _prev_poisonEnabled) { _prev_poisonEnabled = poisonEnabled; _dirty = true; }
        if (frozenEnabled != _prev_frozenEnabled) { _prev_frozenEnabled = frozenEnabled; _dirty = true; }
        if (metalEnabled != _prev_metalEnabled) { _prev_metalEnabled = metalEnabled; _dirty = true; }
        if (camouflageEnabled != _prev_camouflageEnabled) { _prev_camouflageEnabled = camouflageEnabled; _dirty = true; }
        if (rainbowEnabled != _prev_rainbowEnabled) { _prev_rainbowEnabled = rainbowEnabled; _dirty = true; }
        if (blackTintEnabled != _prev_blackTintEnabled) { _prev_blackTintEnabled = blackTintEnabled; _dirty = true; }
        if (chromaticEnabled != _prev_chromaticEnabled) { _prev_chromaticEnabled = chromaticEnabled; _dirty = true; }
        if (paletteSwapEnabled != _prev_paletteSwapEnabled) { _prev_paletteSwapEnabled = paletteSwapEnabled; _dirty = true; }
        if (ditheringEnabled != _prev_ditheringEnabled) { _prev_ditheringEnabled = ditheringEnabled; _dirty = true; }
        if (crtScanlinesEnabled != _prev_crtScanlinesEnabled) { _prev_crtScanlinesEnabled = crtScanlinesEnabled; _dirty = true; }
        if (vignetteEnabled != _prev_vignetteEnabled) { _prev_vignetteEnabled = vignetteEnabled; _dirty = true; }
        if (colorGradingEnabled != _prev_colorGradingEnabled) { _prev_colorGradingEnabled = colorGradingEnabled; _dirty = true; }
        if (sepiaEnabled != _prev_sepiaEnabled) { _prev_sepiaEnabled = sepiaEnabled; _dirty = true; }
        if (thermalVisionEnabled != _prev_thermalVisionEnabled) { _prev_thermalVisionEnabled = thermalVisionEnabled; _dirty = true; }
        if (recolorRGBEnabled != _prev_recolorRGBEnabled) { _prev_recolorRGBEnabled = recolorRGBEnabled; _dirty = true; }
        if (pingPongGlowEnabled != _prev_pingPongGlowEnabled) { _prev_pingPongGlowEnabled = pingPongGlowEnabled; _dirty = true; }
        if (colorCycleEnabled != _prev_colorCycleEnabled) { _prev_colorCycleEnabled = colorCycleEnabled; _dirty = true; }
        if (outlineEnabled != _prev_outlineEnabled) { _prev_outlineEnabled = outlineEnabled; _dirty = true; }
        if (innerOutlineEnabled != _prev_innerOutlineEnabled) { _prev_innerOutlineEnabled = innerOutlineEnabled; _dirty = true; }
        if (texOutlineEnabled != _prev_texOutlineEnabled) { _prev_texOutlineEnabled = texOutlineEnabled; _dirty = true; }
        if (rainbowOutlineEnabled != _prev_rainbowOutlineEnabled) { _prev_rainbowOutlineEnabled = rainbowOutlineEnabled; _dirty = true; }
        if (distortOutlineEnabled != _prev_distortOutlineEnabled) { _prev_distortOutlineEnabled = distortOutlineEnabled; _dirty = true; }
        if (glowEnabled != _prev_glowEnabled) { _prev_glowEnabled = glowEnabled; _dirty = true; }
        if (shadowEnabled != _prev_shadowEnabled) { _prev_shadowEnabled = shadowEnabled; _dirty = true; }
        if (softShadowEnabled != _prev_softShadowEnabled) { _prev_softShadowEnabled = softShadowEnabled; _dirty = true; }
        if (dirOutlineEnabled != _prev_dirOutlineEnabled) { _prev_dirOutlineEnabled = dirOutlineEnabled; _dirty = true; }
        if (outlineOnlyEnabled != _prev_outlineOnlyEnabled) { _prev_outlineOnlyEnabled = outlineOnlyEnabled; _dirty = true; }
        if (gradOutlineEnabled != _prev_gradOutlineEnabled) { _prev_gradOutlineEnabled = gradOutlineEnabled; _dirty = true; }
        if (animOutlineEnabled != _prev_animOutlineEnabled) { _prev_animOutlineEnabled = animOutlineEnabled; _dirty = true; }
        if (selectHighlightEnabled != _prev_selectHighlightEnabled) { _prev_selectHighlightEnabled = selectHighlightEnabled; _dirty = true; }
        if (pixelateEnabled != _prev_pixelateEnabled) { _prev_pixelateEnabled = pixelateEnabled; _dirty = true; }
        if (waveEnabled != _prev_waveEnabled) { _prev_waveEnabled = waveEnabled; _dirty = true; }
        if (twistEnabled != _prev_twistEnabled) { _prev_twistEnabled = twistEnabled; _dirty = true; }
        if (uvDistortEnabled != _prev_uvDistortEnabled) { _prev_uvDistortEnabled = uvDistortEnabled; _dirty = true; }
        if (fullDistortEnabled != _prev_fullDistortEnabled) { _prev_fullDistortEnabled = fullDistortEnabled; _dirty = true; }
        if (dirDistortEnabled != _prev_dirDistortEnabled) { _prev_dirDistortEnabled = dirDistortEnabled; _dirty = true; }
        if (uvScrollEnabled != _prev_uvScrollEnabled) { _prev_uvScrollEnabled = uvScrollEnabled; _dirty = true; }
        if (rotateEnabled != _prev_rotateEnabled) { _prev_rotateEnabled = rotateEnabled; _dirty = true; }
        if (squeezeEnabled != _prev_squeezeEnabled) { _prev_squeezeEnabled = squeezeEnabled; _dirty = true; }
        if (squishEnabled != _prev_squishEnabled) { _prev_squishEnabled = squishEnabled; _dirty = true; }
        if (wiggleEnabled != _prev_wiggleEnabled) { _prev_wiggleEnabled = wiggleEnabled; _dirty = true; }
        if (windEnabled != _prev_windEnabled) { _prev_windEnabled = windEnabled; _dirty = true; }
        if (sharpenEnabled != _prev_sharpenEnabled) { _prev_sharpenEnabled = sharpenEnabled; _dirty = true; }
        if (gaussBlurEnabled != _prev_gaussBlurEnabled) { _prev_gaussBlurEnabled = gaussBlurEnabled; _dirty = true; }
        if (dirBlurEnabled != _prev_dirBlurEnabled) { _prev_dirBlurEnabled = dirBlurEnabled; _dirty = true; }
        if (worldTilingEnabled != _prev_worldTilingEnabled) { _prev_worldTilingEnabled = worldTilingEnabled; _dirty = true; }
        if (uvScaleOffsetEnabled != _prev_uvScaleOffsetEnabled) { _prev_uvScaleOffsetEnabled = uvScaleOffsetEnabled; _dirty = true; }
        if (curlNoiseEnabled != _prev_curlNoiseEnabled) { _prev_curlNoiseEnabled = curlNoiseEnabled; _dirty = true; }
        if (fbmNoiseEnabled != _prev_fbmNoiseEnabled) { _prev_fbmNoiseEnabled = fbmNoiseEnabled; _dirty = true; }
        if (uvScrollOverlayEnabled != _prev_uvScrollOverlayEnabled) { _prev_uvScrollOverlayEnabled = uvScrollOverlayEnabled; _dirty = true; }
        if (shockwaveEnabled != _prev_shockwaveEnabled) { _prev_shockwaveEnabled = shockwaveEnabled; _dirty = true; }
        if (sineScaleEnabled != _prev_sineScaleEnabled) { _prev_sineScaleEnabled = sineScaleEnabled; _dirty = true; }
        if (heartbeatEnabled != _prev_heartbeatEnabled) { _prev_heartbeatEnabled = heartbeatEnabled; _dirty = true; }
        if (breathingEnabled != _prev_breathingEnabled) { _prev_breathingEnabled = breathingEnabled; _dirty = true; }
        if (liquidWobbleEnabled != _prev_liquidWobbleEnabled) { _prev_liquidWobbleEnabled = liquidWobbleEnabled; _dirty = true; }
        if (flipBookBlendEnabled != _prev_flipBookBlendEnabled) { _prev_flipBookBlendEnabled = flipBookBlendEnabled; _dirty = true; }
        if (smoothPixelArtEnabled != _prev_smoothPixelArtEnabled) { _prev_smoothPixelArtEnabled = smoothPixelArtEnabled; _dirty = true; }
        if (dissolveEnabled != _prev_dissolveEnabled) { _prev_dissolveEnabled = dissolveEnabled; _dirty = true; }
        if (glowDissolveEnabled != _prev_glowDissolveEnabled) { _prev_glowDissolveEnabled = glowDissolveEnabled; _dirty = true; }
        if (srcDissolveEnabled != _prev_srcDissolveEnabled) { _prev_srcDissolveEnabled = srcDissolveEnabled; _dirty = true; }
        if (dirFadeEnabled != _prev_dirFadeEnabled) { _prev_dirFadeEnabled = dirFadeEnabled; _dirty = true; }
        if (dirGlowFadeEnabled != _prev_dirGlowFadeEnabled) { _prev_dirGlowFadeEnabled = dirGlowFadeEnabled; _dirty = true; }
        if (fullAlphaDissolveEnabled != _prev_fullAlphaDissolveEnabled) { _prev_fullAlphaDissolveEnabled = fullAlphaDissolveEnabled; _dirty = true; }
        if (halftoneEnabled != _prev_halftoneEnabled) { _prev_halftoneEnabled = halftoneEnabled; _dirty = true; }
        if (radialWipeEnabled != _prev_radialWipeEnabled) { _prev_radialWipeEnabled = radialWipeEnabled; _dirty = true; }
        if (blindsEnabled != _prev_blindsEnabled) { _prev_blindsEnabled = blindsEnabled; _dirty = true; }
        if (diamondDissolveEnabled != _prev_diamondDissolveEnabled) { _prev_diamondDissolveEnabled = diamondDissolveEnabled; _dirty = true; }
        if (scanlineDissolveEnabled != _prev_scanlineDissolveEnabled) { _prev_scanlineDissolveEnabled = scanlineDissolveEnabled; _dirty = true; }
        if (spreadFadeEnabled != _prev_spreadFadeEnabled) { _prev_spreadFadeEnabled = spreadFadeEnabled; _dirty = true; }
        if (pixelDissolveEnabled != _prev_pixelDissolveEnabled) { _prev_pixelDissolveEnabled = pixelDissolveEnabled; _dirty = true; }
        if (spiralWipeEnabled != _prev_spiralWipeEnabled) { _prev_spiralWipeEnabled = spiralWipeEnabled; _dirty = true; }
        if (hexDissolveEnabled != _prev_hexDissolveEnabled) { _prev_hexDissolveEnabled = hexDissolveEnabled; _dirty = true; }
        if (hologramEnabled != _prev_hologramEnabled) { _prev_hologramEnabled = hologramEnabled; _dirty = true; }
        if (glitchEnabled != _prev_glitchEnabled) { _prev_glitchEnabled = glitchEnabled; _dirty = true; }
        if (flickerEnabled != _prev_flickerEnabled) { _prev_flickerEnabled = flickerEnabled; _dirty = true; }
        if (shineEnabled != _prev_shineEnabled) { _prev_shineEnabled = shineEnabled; _dirty = true; }
        if (smokeEnabled != _prev_smokeEnabled) { _prev_smokeEnabled = smokeEnabled; _dirty = true; }
        if (flameEnabled != _prev_flameEnabled) { _prev_flameEnabled = flameEnabled; _dirty = true; }
        if (enchantedEnabled != _prev_enchantedEnabled) { _prev_enchantedEnabled = enchantedEnabled; _dirty = true; }
        if (shiftingEnabled != _prev_shiftingEnabled) { _prev_shiftingEnabled = shiftingEnabled; _dirty = true; }
        if (shiftRainbowEnabled != _prev_shiftRainbowEnabled) { _prev_shiftRainbowEnabled = shiftRainbowEnabled; _dirty = true; }
        if (overlayEnabled != _prev_overlayEnabled) { _prev_overlayEnabled = overlayEnabled; _dirty = true; }
        if (checkerboardEnabled != _prev_checkerboardEnabled) { _prev_checkerboardEnabled = checkerboardEnabled; _dirty = true; }
        if (uvVisualizeEnabled != _prev_uvVisualizeEnabled) { _prev_uvVisualizeEnabled = uvVisualizeEnabled; _dirty = true; }
        if (healthTintEnabled != _prev_healthTintEnabled) { _prev_healthTintEnabled = healthTintEnabled; _dirty = true; }
        if (xraySilhouetteEnabled != _prev_xraySilhouetteEnabled) { _prev_xraySilhouetteEnabled = xraySilhouetteEnabled; _dirty = true; }
        if (minimapPingEnabled != _prev_minimapPingEnabled) { _prev_minimapPingEnabled = minimapPingEnabled; _dirty = true; }
        if (dialogueEmphasisEnabled != _prev_dialogueEmphasisEnabled) { _prev_dialogueEmphasisEnabled = dialogueEmphasisEnabled; _dirty = true; }
        if (voronoiCrackleEnabled != _prev_voronoiCrackleEnabled) { _prev_voronoiCrackleEnabled = voronoiCrackleEnabled; _dirty = true; }
        if (waterReflectEnabled != _prev_waterReflectEnabled) { _prev_waterReflectEnabled = waterReflectEnabled; _dirty = true; }
        if (motionSmearEnabled != _prev_motionSmearEnabled) { _prev_motionSmearEnabled = motionSmearEnabled; _dirty = true; }
        if (energyAuraEnabled != _prev_energyAuraEnabled) { _prev_energyAuraEnabled = energyAuraEnabled; _dirty = true; }
        if (gradientEnabled != _prev_gradientEnabled) { _prev_gradientEnabled = gradientEnabled; _dirty = true; }
        if (radialGradientEnabled != _prev_radialGradientEnabled) { _prev_radialGradientEnabled = radialGradientEnabled; _dirty = true; }
        if (colorRampEnabled != _prev_colorRampEnabled) { _prev_colorRampEnabled = colorRampEnabled; _dirty = true; }
        if (fakeLightingEnabled != _prev_fakeLightingEnabled) { _prev_fakeLightingEnabled = fakeLightingEnabled; _dirty = true; }
        if (uvBendPointEnabled != _prev_uvBendPointEnabled) { _prev_uvBendPointEnabled = uvBendPointEnabled; _dirty = true; }
        if (burnDirectionEnabled != _prev_burnDirectionEnabled) { _prev_burnDirectionEnabled = burnDirectionEnabled; _dirty = true; }
        if (globalSmokeEnabled != _prev_globalSmokeEnabled) { _prev_globalSmokeEnabled = globalSmokeEnabled; _dirty = true; }
        if (holographicCardEnabled != _prev_holographicCardEnabled) { _prev_holographicCardEnabled = holographicCardEnabled; _dirty = true; }
        if (cloudShadowsEnabled != _prev_cloudShadowsEnabled) { _prev_cloudShadowsEnabled = cloudShadowsEnabled; _dirty = true; }
        if (colorfulTrailAEnabled != _prev_colorfulTrailAEnabled) { _prev_colorfulTrailAEnabled = colorfulTrailAEnabled; _dirty = true; }
        if (colorfulTrailBEnabled != _prev_colorfulTrailBEnabled) { _prev_colorfulTrailBEnabled = colorfulTrailBEnabled; _dirty = true; }
        if (colorfulTrailCEnabled != _prev_colorfulTrailCEnabled) { _prev_colorfulTrailCEnabled = colorfulTrailCEnabled; _dirty = true; }
        if (chargingAbilityEnabled != _prev_chargingAbilityEnabled) { _prev_chargingAbilityEnabled = chargingAbilityEnabled; _dirty = true; }
        if (blueprintEnabled != _prev_blueprintEnabled) { _prev_blueprintEnabled = blueprintEnabled; _dirty = true; }
        if (ghostingEnabled != _prev_ghostingEnabled) { _prev_ghostingEnabled = ghostingEnabled; _dirty = true; }
        if (retroPaletteEnabled != _prev_retroPaletteEnabled) { _prev_retroPaletteEnabled = retroPaletteEnabled; _dirty = true; }
        if (_dirty) return;

        if (strongTintEnabled && CheckEffect_StrongTint()) SetEffectDirtyBit(0);
        if (addColorEnabled && CheckEffect_AddColor()) SetEffectDirtyBit(1);
        if (alphaTintEnabled && CheckEffect_AlphaTint()) SetEffectDirtyBit(2);
        if (hitEffectEnabled && CheckEffect_HitEffect()) SetEffectDirtyBit(3);
        if (negativeEnabled && CheckEffect_Negative()) SetEffectDirtyBit(4);
        if (greyscaleEnabled && CheckEffect_Greyscale()) SetEffectDirtyBit(5);
        if (bloomEnabled && CheckEffect_Bloom()) SetEffectDirtyBit(6);
        if (colorReplaceEnabled && CheckEffect_ColorReplace()) SetEffectDirtyBit(7);
        if (lutReplaceEnabled && CheckEffect_LUTReplace()) SetEffectDirtyBit(8);
        if (hueShiftEnabled && CheckEffect_HueShift()) SetEffectDirtyBit(9);
        if (addHueEnabled && CheckEffect_AddHue()) SetEffectDirtyBit(10);
        if (saturationEnabled && CheckEffect_Saturation()) SetEffectDirtyBit(11);
        if (brightnessEnabled && CheckEffect_Brightness()) SetEffectDirtyBit(12);
        if (contrastEnabled && CheckEffect_Contrast()) SetEffectDirtyBit(13);
        if (splitToningEnabled && CheckEffect_SplitToning()) SetEffectDirtyBit(14);
        if (posterizeEnabled && CheckEffect_Posterize()) SetEffectDirtyBit(15);
        if (poisonEnabled && CheckEffect_Poison()) SetEffectDirtyBit(16);
        if (frozenEnabled && CheckEffect_Frozen()) SetEffectDirtyBit(17);
        if (metalEnabled && CheckEffect_Metal()) SetEffectDirtyBit(18);
        if (camouflageEnabled && CheckEffect_Camouflage()) SetEffectDirtyBit(19);
        if (rainbowEnabled && CheckEffect_Rainbow()) SetEffectDirtyBit(20);
        if (blackTintEnabled && CheckEffect_BlackTint()) SetEffectDirtyBit(21);
        if (chromaticEnabled && CheckEffect_Chromatic()) SetEffectDirtyBit(22);
        if (paletteSwapEnabled && CheckEffect_PaletteSwap()) SetEffectDirtyBit(23);
        if (ditheringEnabled && CheckEffect_Dithering()) SetEffectDirtyBit(24);
        if (crtScanlinesEnabled && CheckEffect_CRTScanlines()) SetEffectDirtyBit(25);
        if (vignetteEnabled && CheckEffect_Vignette()) SetEffectDirtyBit(26);
        if (colorGradingEnabled && CheckEffect_ColorGrading()) SetEffectDirtyBit(27);
        if (sepiaEnabled && CheckEffect_Sepia()) SetEffectDirtyBit(28);
        if (thermalVisionEnabled && CheckEffect_ThermalVision()) SetEffectDirtyBit(29);
        if (recolorRGBEnabled && CheckEffect_RecolorRGB()) SetEffectDirtyBit(30);
        if (pingPongGlowEnabled && CheckEffect_PingPongGlow()) SetEffectDirtyBit(31);
        if (colorCycleEnabled && CheckEffect_ColorCycle()) SetEffectDirtyBit(32);
        if (outlineEnabled && CheckEffect_Outline()) SetEffectDirtyBit(33);
        if (innerOutlineEnabled && CheckEffect_InnerOutline()) SetEffectDirtyBit(34);
        if (texOutlineEnabled && CheckEffect_TexOutline()) SetEffectDirtyBit(35);
        if (rainbowOutlineEnabled && CheckEffect_RainbowOutline()) SetEffectDirtyBit(36);
        if (distortOutlineEnabled && CheckEffect_DistortOutline()) SetEffectDirtyBit(37);
        if (glowEnabled && CheckEffect_Glow()) SetEffectDirtyBit(38);
        if (shadowEnabled && CheckEffect_Shadow()) SetEffectDirtyBit(39);
        if (softShadowEnabled && CheckEffect_SoftShadow()) SetEffectDirtyBit(40);
        if (dirOutlineEnabled && CheckEffect_DirOutline()) SetEffectDirtyBit(41);
        if (outlineOnlyEnabled && CheckEffect_OutlineOnly()) SetEffectDirtyBit(42);
        if (gradOutlineEnabled && CheckEffect_GradientOutline()) SetEffectDirtyBit(43);
        if (animOutlineEnabled && CheckEffect_AnimOutlineThick()) SetEffectDirtyBit(44);
        if (selectHighlightEnabled && CheckEffect_SelectionHighlight()) SetEffectDirtyBit(45);
        if (pixelateEnabled && CheckEffect_Pixelate()) SetEffectDirtyBit(46);
        if (waveEnabled && CheckEffect_Wave()) SetEffectDirtyBit(47);
        if (twistEnabled && CheckEffect_Twist()) SetEffectDirtyBit(48);
        if (uvDistortEnabled && CheckEffect_UVDistort()) SetEffectDirtyBit(49);
        if (fullDistortEnabled && CheckEffect_FullDistort()) SetEffectDirtyBit(50);
        if (dirDistortEnabled && CheckEffect_DirDistort()) SetEffectDirtyBit(51);
        if (uvScrollEnabled && CheckEffect_UVScroll()) SetEffectDirtyBit(52);
        if (rotateEnabled && CheckEffect_Rotate()) SetEffectDirtyBit(53);
        if (squeezeEnabled && CheckEffect_Squeeze()) SetEffectDirtyBit(54);
        if (squishEnabled && CheckEffect_Squish()) SetEffectDirtyBit(55);
        if (wiggleEnabled && CheckEffect_Wiggle()) SetEffectDirtyBit(56);
        if (windEnabled && CheckEffect_Wind()) SetEffectDirtyBit(57);
        if (sharpenEnabled && CheckEffect_Sharpen()) SetEffectDirtyBit(58);
        if (gaussBlurEnabled && CheckEffect_GaussBlur()) SetEffectDirtyBit(59);
        if (dirBlurEnabled && CheckEffect_DirBlur()) SetEffectDirtyBit(60);
        if (worldTilingEnabled && CheckEffect_WorldTiling()) SetEffectDirtyBit(61);
        if (uvScaleOffsetEnabled && CheckEffect_UVScaleOffset()) SetEffectDirtyBit(62);
        if (curlNoiseEnabled && CheckEffect_CurlNoise()) SetEffectDirtyBit(63);
        if (fbmNoiseEnabled && CheckEffect_FBMNoise()) SetEffectDirtyBit(64);
        if (uvScrollOverlayEnabled && CheckEffect_UVScrollOverlay()) SetEffectDirtyBit(65);
        if (shockwaveEnabled && CheckEffect_Shockwave()) SetEffectDirtyBit(66);
        if (sineScaleEnabled && CheckEffect_SineScale()) SetEffectDirtyBit(67);
        if (heartbeatEnabled && CheckEffect_Heartbeat()) SetEffectDirtyBit(68);
        if (breathingEnabled && CheckEffect_Breathing()) SetEffectDirtyBit(69);
        if (liquidWobbleEnabled && CheckEffect_LiquidWobble()) SetEffectDirtyBit(70);
        if (flipBookBlendEnabled && CheckEffect_FlipBookBlend()) SetEffectDirtyBit(71);
        if (smoothPixelArtEnabled && CheckEffect_SmoothPixelArt()) SetEffectDirtyBit(72);
        if (dissolveEnabled && CheckEffect_Dissolve()) SetEffectDirtyBit(73);
        if (glowDissolveEnabled && CheckEffect_GlowDissolve()) SetEffectDirtyBit(74);
        if (srcDissolveEnabled && CheckEffect_SrcDissolve()) SetEffectDirtyBit(75);
        if (dirFadeEnabled && CheckEffect_DirFade()) SetEffectDirtyBit(76);
        if (dirGlowFadeEnabled && CheckEffect_DirGlowFade()) SetEffectDirtyBit(77);
        if (fullAlphaDissolveEnabled && CheckEffect_FullAlphaDissolve()) SetEffectDirtyBit(78);
        if (halftoneEnabled && CheckEffect_Halftone()) SetEffectDirtyBit(79);
        if (radialWipeEnabled && CheckEffect_RadialWipe()) SetEffectDirtyBit(80);
        if (blindsEnabled && CheckEffect_Blinds()) SetEffectDirtyBit(81);
        if (diamondDissolveEnabled && CheckEffect_DiamondDissolve()) SetEffectDirtyBit(82);
        if (scanlineDissolveEnabled && CheckEffect_ScanlineDissolve()) SetEffectDirtyBit(83);
        if (spreadFadeEnabled && CheckEffect_SpreadFade()) SetEffectDirtyBit(84);
        if (pixelDissolveEnabled && CheckEffect_PixelDissolve()) SetEffectDirtyBit(85);
        if (spiralWipeEnabled && CheckEffect_SpiralWipe()) SetEffectDirtyBit(86);
        if (hexDissolveEnabled && CheckEffect_HexDissolve()) SetEffectDirtyBit(87);
        if (hologramEnabled && CheckEffect_Hologram()) SetEffectDirtyBit(88);
        if (glitchEnabled && CheckEffect_Glitch()) SetEffectDirtyBit(89);
        if (flickerEnabled && CheckEffect_Flicker()) SetEffectDirtyBit(90);
        if (shineEnabled && CheckEffect_Shine()) SetEffectDirtyBit(91);
        if (smokeEnabled && CheckEffect_Smoke()) SetEffectDirtyBit(92);
        if (flameEnabled && CheckEffect_Flame()) SetEffectDirtyBit(93);
        if (enchantedEnabled && CheckEffect_Enchanted()) SetEffectDirtyBit(94);
        if (shiftingEnabled && CheckEffect_Shifting()) SetEffectDirtyBit(95);
        if (shiftRainbowEnabled && CheckEffect_ShiftRainbow()) SetEffectDirtyBit(96);
        if (overlayEnabled && CheckEffect_Overlay()) SetEffectDirtyBit(97);
        if (checkerboardEnabled && CheckEffect_Checkerboard()) SetEffectDirtyBit(98);
        if (uvVisualizeEnabled && CheckEffect_UVVisualize()) SetEffectDirtyBit(99);
        if (healthTintEnabled && CheckEffect_HealthTint()) SetEffectDirtyBit(100);
        if (xraySilhouetteEnabled && CheckEffect_XRaySilhouette()) SetEffectDirtyBit(101);
        if (minimapPingEnabled && CheckEffect_MinimapPing()) SetEffectDirtyBit(102);
        if (dialogueEmphasisEnabled && CheckEffect_DialogueEmphasis()) SetEffectDirtyBit(103);
        if (voronoiCrackleEnabled && CheckEffect_VoronoiCrackle()) SetEffectDirtyBit(104);
        if (waterReflectEnabled && CheckEffect_WaterReflect()) SetEffectDirtyBit(105);
        if (motionSmearEnabled && CheckEffect_MotionSmear()) SetEffectDirtyBit(106);
        if (energyAuraEnabled && CheckEffect_EnergyAura()) SetEffectDirtyBit(107);
        if (gradientEnabled && CheckEffect_Gradient()) SetEffectDirtyBit(108);
        if (radialGradientEnabled && CheckEffect_RadialGradient()) SetEffectDirtyBit(109);
        if (colorRampEnabled && CheckEffect_ColorRamp()) SetEffectDirtyBit(110);
        if (fakeLightingEnabled && CheckEffect_FakeLighting()) SetEffectDirtyBit(111);
        if (uvBendPointEnabled && CheckEffect_UVBendPoint()) SetEffectDirtyBit(112);
        if (burnDirectionEnabled && CheckEffect_BurnDirection()) SetEffectDirtyBit(113);
        if (globalSmokeEnabled && CheckEffect_GlobalSmoke()) SetEffectDirtyBit(114);
        if (holographicCardEnabled && CheckEffect_HolographicCard()) SetEffectDirtyBit(115);
        if (cloudShadowsEnabled && CheckEffect_CloudShadows()) SetEffectDirtyBit(116);
        if (colorfulTrailAEnabled && CheckEffect_ColorfulTrailA()) SetEffectDirtyBit(117);
        if (colorfulTrailBEnabled && CheckEffect_ColorfulTrailB()) SetEffectDirtyBit(118);
        if (colorfulTrailCEnabled && CheckEffect_ColorfulTrailC()) SetEffectDirtyBit(119);
        if (chargingAbilityEnabled && CheckEffect_ChargingAbility()) SetEffectDirtyBit(120);
        if (blueprintEnabled && CheckEffect_Blueprint()) SetEffectDirtyBit(121);
        if (ghostingEnabled && CheckEffect_Ghosting()) SetEffectDirtyBit(122);
        if (retroPaletteEnabled && CheckEffect_RetroPalette()) SetEffectDirtyBit(123);
    }

    void InitEffectSnapshot()
    {
        _prev_strongTintEnabled = strongTintEnabled;
        _prev_strongTintMaskCh = strongTintMaskCh;
        _prev_strongTintColor = strongTintColor;
        _prev_strongTintFade = strongTintFade;
        _prev_addColorEnabled = addColorEnabled;
        _prev_addColorMaskCh = addColorMaskCh;
        _prev_addColorAmount = addColorAmount;
        _prev_addColorFade = addColorFade;
        _prev_alphaTintEnabled = alphaTintEnabled;
        _prev_alphaTintMaskCh = alphaTintMaskCh;
        _prev_alphaTintColor = alphaTintColor;
        _prev_alphaTintFade = alphaTintFade;
        _prev_hitEffectEnabled = hitEffectEnabled;
        _prev_hitEffectMaskCh = hitEffectMaskCh;
        _prev_hitEffectColor = hitEffectColor;
        _prev_hitEffectBlend = hitEffectBlend;
        _prev_negativeEnabled = negativeEnabled;
        _prev_negativeMaskCh = negativeMaskCh;
        _prev_negativeAmount = negativeAmount;
        _prev_greyscaleEnabled = greyscaleEnabled;
        _prev_greyscaleMaskCh = greyscaleMaskCh;
        _prev_greyscaleBlend = greyscaleBlend;
        _prev_greyscaleLuminosity = greyscaleLuminosity;
        _prev_bloomEnabled = bloomEnabled;
        _prev_bloomMaskCh = bloomMaskCh;
        _prev_bloomMode = bloomMode;
        _prev_bloomColor = bloomColor;
        _prev_bloomThreshold = bloomThreshold;
        _prev_bloomIntensity = bloomIntensity;
        _prev_bloomSize = bloomSize;
        _prev_bloomSamples = bloomSamples;
        _prev_bloomSoftKnee = bloomSoftKnee;
        _prev_bloomMaskColor = bloomMaskColor;
        _prev_bloomMaskRange = bloomMaskRange;
        _prev_bloomMaskSoft = bloomMaskSoft;
        _prev_bloomAlphaBleed = bloomAlphaBleed;
        _prev_colorReplaceEnabled = colorReplaceEnabled;
        _prev_colorReplaceMaskCh = colorReplaceMaskCh;
        _prev_colorReplaceSrc1 = colorReplaceSrc1;
        _prev_colorReplaceDst1 = colorReplaceDst1;
        _prev_colorReplaceSrc2 = colorReplaceSrc2;
        _prev_colorReplaceDst2 = colorReplaceDst2;
        _prev_colorReplaceSrc3 = colorReplaceSrc3;
        _prev_colorReplaceDst3 = colorReplaceDst3;
        _prev_colorReplaceSrc4 = colorReplaceSrc4;
        _prev_colorReplaceDst4 = colorReplaceDst4;
        _prev_colorReplaceCount = colorReplaceCount;
        _prev_colorReplaceRange = colorReplaceRange;
        _prev_colorReplaceFade = colorReplaceFade;
        _prev_lutReplaceEnabled = lutReplaceEnabled;
        _prev_lutReplaceMaskCh = lutReplaceMaskCh;
        _prev_lutReplaceTexture = lutReplaceTexture;
        _prev_lutSize = lutSize;
        _prev_lutReplaceFade = lutReplaceFade;
        _prev_hueShiftEnabled = hueShiftEnabled;
        _prev_hueShiftMaskCh = hueShiftMaskCh;
        _prev_hueShiftAmount = hueShiftAmount;
        _prev_hueShiftSpeed = hueShiftSpeed;
        _prev_hueShiftFade = hueShiftFade;
        _prev_addHueEnabled = addHueEnabled;
        _prev_addHueMaskCh = addHueMaskCh;
        _prev_addHueAmount = addHueAmount;
        _prev_addHueSpeed = addHueSpeed;
        _prev_addHueSaturation = addHueSaturation;
        _prev_addHueBrightness = addHueBrightness;
        _prev_addHueFade = addHueFade;
        _prev_saturationEnabled = saturationEnabled;
        _prev_saturationMaskCh = saturationMaskCh;
        _prev_saturationAmount = saturationAmount;
        _prev_brightnessEnabled = brightnessEnabled;
        _prev_brightnessMaskCh = brightnessMaskCh;
        _prev_brightnessAmount = brightnessAmount;
        _prev_contrastEnabled = contrastEnabled;
        _prev_contrastMaskCh = contrastMaskCh;
        _prev_contrastAmount = contrastAmount;
        _prev_splitToningEnabled = splitToningEnabled;
        _prev_splitToningMaskCh = splitToningMaskCh;
        _prev_splitShadowColor = splitShadowColor;
        _prev_splitHighlightColor = splitHighlightColor;
        _prev_splitBalance = splitBalance;
        _prev_splitMidColor = splitMidColor;
        _prev_splitFade = splitFade;
        _prev_posterizeEnabled = posterizeEnabled;
        _prev_posterizeMaskCh = posterizeMaskCh;
        _prev_posterizeSteps = posterizeSteps;
        _prev_posterizeFade = posterizeFade;
        _prev_poisonEnabled = poisonEnabled;
        _prev_poisonMaskCh = poisonMaskCh;
        _prev_poisonColor = poisonColor;
        _prev_poisonSpeed = poisonSpeed;
        _prev_poisonDensity = poisonDensity;
        _prev_poisonColor2 = poisonColor2;
        _prev_poisonColor3 = poisonColor3;
        _prev_poisonFade = poisonFade;
        _prev_frozenEnabled = frozenEnabled;
        _prev_frozenMaskCh = frozenMaskCh;
        _prev_frozenColor = frozenColor;
        _prev_frozenContrast = frozenContrast;
        _prev_frozenHighlight = frozenHighlight;
        _prev_frozenColor2 = frozenColor2;
        _prev_frozenHighlightColor = frozenHighlightColor;
        _prev_frozenFade = frozenFade;
        _prev_metalEnabled = metalEnabled;
        _prev_metalMaskCh = metalMaskCh;
        _prev_metalColor = metalColor;
        _prev_metalHighlight = metalHighlight;
        _prev_metalContrast = metalContrast;
        _prev_metalSpeed = metalSpeed;
        _prev_metalColor2 = metalColor2;
        _prev_metalFade = metalFade;
        _prev_camouflageEnabled = camouflageEnabled;
        _prev_camouflageMaskCh = camouflageMaskCh;
        _prev_camoColor1 = camoColor1;
        _prev_camoColor2 = camoColor2;
        _prev_camoColor3 = camoColor3;
        _prev_camoScale = camoScale;
        _prev_camoSpeed = camoSpeed;
        _prev_camoFade = camoFade;
        _prev_rainbowEnabled = rainbowEnabled;
        _prev_rainbowMaskCh = rainbowMaskCh;
        _prev_rainbowSpeed = rainbowSpeed;
        _prev_rainbowSaturation = rainbowSaturation;
        _prev_rainbowBrightness = rainbowBrightness;
        _prev_rainbowFade = rainbowFade;
        _prev_rainbowDensity = rainbowDensity;
        _prev_blackTintEnabled = blackTintEnabled;
        _prev_blackTintMaskCh = blackTintMaskCh;
        _prev_blackTintColor = blackTintColor;
        _prev_blackTintThreshold = blackTintThreshold;
        _prev_blackTintFade = blackTintFade;
        _prev_chromaticEnabled = chromaticEnabled;
        _prev_chromaticMaskCh = chromaticMaskCh;
        _prev_chromaticAmount = chromaticAmount;
        _prev_chromaticAngle = chromaticAngle;
        _prev_paletteSwapEnabled = paletteSwapEnabled;
        _prev_paletteSwapMaskCh = paletteSwapMaskCh;
        _prev_paletteSwapTexture = paletteSwapTexture;
        _prev_paletteSwapColors = paletteSwapColors;
        _prev_paletteSwapFade = paletteSwapFade;
        _prev_ditheringEnabled = ditheringEnabled;
        _prev_ditheringMaskCh = ditheringMaskCh;
        _prev_ditheringSize = ditheringSize;
        _prev_ditheringLevels = ditheringLevels;
        _prev_ditheringFade = ditheringFade;
        _prev_ditheringAlphaMode = ditheringAlphaMode;
        _prev_ditheringAlphaThreshold = ditheringAlphaThreshold;
        _prev_ditheringScreenSpace = ditheringScreenSpace;
        _prev_crtScanlinesEnabled = crtScanlinesEnabled;
        _prev_crtScanlinesMaskCh = crtScanlinesMaskCh;
        _prev_crtDensity = crtDensity;
        _prev_crtIntensity = crtIntensity;
        _prev_crtSpeed = crtSpeed;
        _prev_crtCurvature = crtCurvature;
        _prev_vignetteEnabled = vignetteEnabled;
        _prev_vignetteMaskCh = vignetteMaskCh;
        _prev_vignetteColor = vignetteColor;
        _prev_vignetteIntensity = vignetteIntensity;
        _prev_vignetteSmoothness = vignetteSmoothness;
        _prev_vignetteAlpha = vignetteAlpha;
        _prev_vignetteCenter = vignetteCenter;
        _prev_vignetteOffset = vignetteOffset;
        _prev_colorGradingEnabled = colorGradingEnabled;
        _prev_colorGradingMaskCh = colorGradingMaskCh;
        _prev_cgExposure = cgExposure;
        _prev_cgGamma = cgGamma;
        _prev_cgLift = cgLift;
        _prev_sepiaEnabled = sepiaEnabled;
        _prev_sepiaMaskCh = sepiaMaskCh;
        _prev_sepiaIntensity = sepiaIntensity;
        _prev_sepiaColor = sepiaColor;
        _prev_thermalVisionEnabled = thermalVisionEnabled;
        _prev_thermalVisionMaskCh = thermalVisionMaskCh;
        _prev_thermalIntensity = thermalIntensity;
        _prev_thermalHDRGlow = thermalHDRGlow;
        _prev_recolorRGBEnabled = recolorRGBEnabled;
        _prev_recolorRGBMaskCh = recolorRGBMaskCh;
        _prev_recolorR = recolorR;
        _prev_recolorG = recolorG;
        _prev_recolorB = recolorB;
        _prev_recolorCyan = recolorCyan;
        _prev_recolorMagenta = recolorMagenta;
        _prev_recolorYellow = recolorYellow;
        _prev_recolorFade = recolorFade;
        _prev_pingPongGlowEnabled = pingPongGlowEnabled;
        _prev_pingPongGlowMaskCh = pingPongGlowMaskCh;
        _prev_pingPongGlowColor = pingPongGlowColor;
        _prev_pingPongGlowSpeed = pingPongGlowSpeed;
        _prev_pingPongGlowMin = pingPongGlowMin;
        _prev_pingPongGlowMax = pingPongGlowMax;
        _prev_colorCycleEnabled = colorCycleEnabled;
        _prev_colorCycleMaskCh = colorCycleMaskCh;
        _prev_colorCycleA = colorCycleA;
        _prev_colorCycleB = colorCycleB;
        _prev_colorCycleC = colorCycleC;
        _prev_colorCycleD = colorCycleD;
        _prev_colorCycleSpeed = colorCycleSpeed;
        _prev_colorCycleFade = colorCycleFade;
        _prev_outlineEnabled = outlineEnabled;
        _prev_outlineMaskCh = outlineMaskCh;
        _prev_outlineColor = outlineColor;
        _prev_outlineSize = outlineSize;
        _prev_outlineAlpha = outlineAlpha;
        _prev_innerOutlineEnabled = innerOutlineEnabled;
        _prev_innerOutlineMaskCh = innerOutlineMaskCh;
        _prev_innerOutlineColor = innerOutlineColor;
        _prev_innerOutlineThickness = innerOutlineThickness;
        _prev_innerOutlineAlpha = innerOutlineAlpha;
        _prev_texOutlineEnabled = texOutlineEnabled;
        _prev_texOutlineMaskCh = texOutlineMaskCh;
        _prev_texOutlineTexture = texOutlineTexture;
        _prev_texOutlineColor = texOutlineColor;
        _prev_texOutlineSize = texOutlineSize;
        _prev_texOutlineTiling = texOutlineTiling;
        _prev_texOutlineSpeed = texOutlineSpeed;
        _prev_rainbowOutlineEnabled = rainbowOutlineEnabled;
        _prev_rainbowOutlineMaskCh = rainbowOutlineMaskCh;
        _prev_rainbowOutlineColor = rainbowOutlineColor;
        _prev_rainbowOutlineSize = rainbowOutlineSize;
        _prev_rainbowOutlineSpeed = rainbowOutlineSpeed;
        _prev_rainbowOutlineDensity = rainbowOutlineDensity;
        _prev_distortOutlineEnabled = distortOutlineEnabled;
        _prev_distortOutlineMaskCh = distortOutlineMaskCh;
        _prev_distortOutlineColor = distortOutlineColor;
        _prev_distortOutlineSize = distortOutlineSize;
        _prev_distortOutlineAmount = distortOutlineAmount;
        _prev_distortOutlineSpeed = distortOutlineSpeed;
        _prev_glowEnabled = glowEnabled;
        _prev_glowMaskCh = glowMaskCh;
        _prev_glowColor = glowColor;
        _prev_glowSize = glowSize;
        _prev_glowIntensity = glowIntensity;
        _prev_shadowEnabled = shadowEnabled;
        _prev_shadowMaskCh = shadowMaskCh;
        _prev_shadowColor = shadowColor;
        _prev_shadowOffset = shadowOffset;
        _prev_softShadowEnabled = softShadowEnabled;
        _prev_softShadowMaskCh = softShadowMaskCh;
        _prev_softShadowColor = softShadowColor;
        _prev_softShadowOffset = softShadowOffset;
        _prev_softShadowSoftness = softShadowSoftness;
        _prev_softShadowSamples = softShadowSamples;
        _prev_dirOutlineEnabled = dirOutlineEnabled;
        _prev_dirOutlineMaskCh = dirOutlineMaskCh;
        _prev_dirOutlineColor = dirOutlineColor;
        _prev_dirOutlineWidth = dirOutlineWidth;
        _prev_dirOutlineIntensity = dirOutlineIntensity;
        _prev_dirOutlineDirection = dirOutlineDirection;
        _prev_dirOutlineSoftness = dirOutlineSoftness;
        _prev_outlineOnlyEnabled = outlineOnlyEnabled;
        _prev_outlineOnlyMaskCh = outlineOnlyMaskCh;
        _prev_outlineOnlyColor = outlineOnlyColor;
        _prev_outlineOnlyWidth = outlineOnlyWidth;
        _prev_gradOutlineEnabled = gradOutlineEnabled;
        _prev_gradOutlineMaskCh = gradOutlineMaskCh;
        _prev_gradOutlineColorA = gradOutlineColorA;
        _prev_gradOutlineColorB = gradOutlineColorB;
        _prev_gradOutlineAngle = gradOutlineAngle;
        _prev_gradOutlineSize = gradOutlineSize;
        _prev_animOutlineEnabled = animOutlineEnabled;
        _prev_animOutlineMaskCh = animOutlineMaskCh;
        _prev_animOutlineColor = animOutlineColor;
        _prev_animOutlineMin = animOutlineMin;
        _prev_animOutlineMax = animOutlineMax;
        _prev_animOutlineSpeed = animOutlineSpeed;
        _prev_selectHighlightEnabled = selectHighlightEnabled;
        _prev_selectHighlightMaskCh = selectHighlightMaskCh;
        _prev_selectHighlightColor = selectHighlightColor;
        _prev_selectHighlightSize = selectHighlightSize;
        _prev_selectHighlightDash = selectHighlightDash;
        _prev_selectHighlightSpeed = selectHighlightSpeed;
        _prev_pixelateEnabled = pixelateEnabled;
        _prev_pixelateMaskCh = pixelateMaskCh;
        _prev_pixelateSize = pixelateSize;
        _prev_waveEnabled = waveEnabled;
        _prev_waveMaskCh = waveMaskCh;
        _prev_waveSpeed = waveSpeed;
        _prev_waveAmplitude = waveAmplitude;
        _prev_waveFrequency = waveFrequency;
        _prev_twistEnabled = twistEnabled;
        _prev_twistMaskCh = twistMaskCh;
        _prev_twistAmount = twistAmount;
        _prev_twistSpeed = twistSpeed;
        _prev_twistOffset = twistOffset;
        _prev_twistRadius = twistRadius;
        _prev_uvDistortEnabled = uvDistortEnabled;
        _prev_uvDistortMaskCh = uvDistortMaskCh;
        _prev_distortTexture = distortTexture;
        _prev_distortAmount = distortAmount;
        _prev_distortTiling = distortTiling;
        _prev_distortSpeed = distortSpeed;
        _prev_distortIsNormalMap = distortIsNormalMap;
        _prev_fullDistortEnabled = fullDistortEnabled;
        _prev_fullDistortMaskCh = fullDistortMaskCh;
        _prev_fullDistortAmount = fullDistortAmount;
        _prev_fullDistortSpeed = fullDistortSpeed;
        _prev_fullDistortDensity = fullDistortDensity;
        _prev_dirDistortEnabled = dirDistortEnabled;
        _prev_dirDistortMaskCh = dirDistortMaskCh;
        _prev_dirDistortAngle = dirDistortAngle;
        _prev_dirDistortAmount = dirDistortAmount;
        _prev_dirDistortSpeed = dirDistortSpeed;
        _prev_uvScrollEnabled = uvScrollEnabled;
        _prev_uvScrollMaskCh = uvScrollMaskCh;
        _prev_uvScrollSpeedX = uvScrollSpeedX;
        _prev_uvScrollSpeedY = uvScrollSpeedY;
        _prev_rotateEnabled = rotateEnabled;
        _prev_rotateMaskCh = rotateMaskCh;
        _prev_rotateSpeed = rotateSpeed;
        _prev_rotateAmount = rotateAmount;
        _prev_squeezeEnabled = squeezeEnabled;
        _prev_squeezeMaskCh = squeezeMaskCh;
        _prev_squeezeAmount = squeezeAmount;
        _prev_squeezeSpeed = squeezeSpeed;
        _prev_squeezePower = squeezePower;
        _prev_squishEnabled = squishEnabled;
        _prev_squishMaskCh = squishMaskCh;
        _prev_squishAmount = squishAmount;
        _prev_squishSpeed = squishSpeed;
        _prev_squishPower = squishPower;
        _prev_wiggleEnabled = wiggleEnabled;
        _prev_wiggleMaskCh = wiggleMaskCh;
        _prev_wiggleSpeed = wiggleSpeed;
        _prev_wiggleAmount = wiggleAmount;
        _prev_wiggleFrequency = wiggleFrequency;
        _prev_windEnabled = windEnabled;
        _prev_windMaskCh = windMaskCh;
        _prev_windSpeed = windSpeed;
        _prev_windAmount = windAmount;
        _prev_windBottomY = windBottomY;
        _prev_windFrequency = windFrequency;
        _prev_sharpenEnabled = sharpenEnabled;
        _prev_sharpenMaskCh = sharpenMaskCh;
        _prev_sharpenAmount = sharpenAmount;
        _prev_sharpenSize = sharpenSize;
        _prev_gaussBlurEnabled = gaussBlurEnabled;
        _prev_gaussBlurMaskCh = gaussBlurMaskCh;
        _prev_gaussBlurSize = gaussBlurSize;
        _prev_gaussBlurSamples = gaussBlurSamples;
        _prev_dirBlurEnabled = dirBlurEnabled;
        _prev_dirBlurMaskCh = dirBlurMaskCh;
        _prev_dirBlurAngle = dirBlurAngle;
        _prev_dirBlurSize = dirBlurSize;
        _prev_dirBlurSamples = dirBlurSamples;
        _prev_worldTilingEnabled = worldTilingEnabled;
        _prev_worldTilingMaskCh = worldTilingMaskCh;
        _prev_worldTileScale = worldTileScale;
        _prev_worldTileOffset = worldTileOffset;
        _prev_worldTileRotation = worldTileRotation;
        _prev_uvScaleOffsetEnabled = uvScaleOffsetEnabled;
        _prev_uvScaleOffsetMaskCh = uvScaleOffsetMaskCh;
        _prev_uvScaleOffset = uvScaleOffset;
        _prev_curlNoiseEnabled = curlNoiseEnabled;
        _prev_curlNoiseMaskCh = curlNoiseMaskCh;
        _prev_curlNoiseAmount = curlNoiseAmount;
        _prev_curlNoiseSpeed = curlNoiseSpeed;
        _prev_curlNoiseDensity = curlNoiseDensity;
        _prev_fbmNoiseEnabled = fbmNoiseEnabled;
        _prev_fbmNoiseMaskCh = fbmNoiseMaskCh;
        _prev_fbmNoiseAmount = fbmNoiseAmount;
        _prev_fbmNoiseSpeed = fbmNoiseSpeed;
        _prev_fbmNoiseDensity = fbmNoiseDensity;
        _prev_fbmNoiseOctaves = fbmNoiseOctaves;
        _prev_uvScrollOverlayEnabled = uvScrollOverlayEnabled;
        _prev_uvScrollOverlayMaskCh = uvScrollOverlayMaskCh;
        _prev_uvScrollOverlayTex = uvScrollOverlayTex;
        _prev_uvScrollOverlaySpeedX = uvScrollOverlaySpeedX;
        _prev_uvScrollOverlaySpeedY = uvScrollOverlaySpeedY;
        _prev_uvScrollOverlayBlend = uvScrollOverlayBlend;
        _prev_uvScrollOverlayTint = uvScrollOverlayTint;
        _prev_uvScrollOverlayTiling = uvScrollOverlayTiling;
        _prev_shockwaveEnabled = shockwaveEnabled;
        _prev_shockwaveMaskCh = shockwaveMaskCh;
        _prev_shockwaveCenter = shockwaveCenter;
        _prev_shockwaveRadius = shockwaveRadius;
        _prev_shockwaveWidth = shockwaveWidth;
        _prev_shockwaveAmplitude = shockwaveAmplitude;
        _prev_sineScaleEnabled = sineScaleEnabled;
        _prev_sineScaleMaskCh = sineScaleMaskCh;
        _prev_sineScaleAmountX = sineScaleAmountX;
        _prev_sineScaleAmountY = sineScaleAmountY;
        _prev_sineScaleSpeed = sineScaleSpeed;
        _prev_heartbeatEnabled = heartbeatEnabled;
        _prev_heartbeatMaskCh = heartbeatMaskCh;
        _prev_heartbeatSpeed = heartbeatSpeed;
        _prev_heartbeatAmount = heartbeatAmount;
        _prev_breathingEnabled = breathingEnabled;
        _prev_breathingMaskCh = breathingMaskCh;
        _prev_breathingSpeed = breathingSpeed;
        _prev_breathingAmount = breathingAmount;
        _prev_liquidWobbleEnabled = liquidWobbleEnabled;
        _prev_liquidWobbleMaskCh = liquidWobbleMaskCh;
        _prev_liquidWobbleSpeed = liquidWobbleSpeed;
        _prev_liquidWobbleAmount = liquidWobbleAmount;
        _prev_liquidWobbleFreq = liquidWobbleFreq;
        _prev_flipBookBlendEnabled = flipBookBlendEnabled;
        _prev_flipBookBlendMaskCh = flipBookBlendMaskCh;
        _prev_flipBookNextFrame = flipBookNextFrame;
        _prev_flipBookLerp = flipBookLerp;
        _prev_smoothPixelArtEnabled = smoothPixelArtEnabled;
        _prev_smoothPixelArtMaskCh = smoothPixelArtMaskCh;
        _prev_smoothPixelArtTexelSize = smoothPixelArtTexelSize;
        _prev_dissolveEnabled = dissolveEnabled;
        _prev_dissolveMaskCh = dissolveMaskCh;
        _prev_dissolveNoiseType = dissolveNoiseType;
        _prev_dissolveTexture = dissolveTexture;
        _prev_dissolveNoiseScale = dissolveNoiseScale;
        _prev_dissolveAmount = dissolveAmount;
        _prev_dissolveBorderWidth = dissolveBorderWidth;
        _prev_dissolveBurnWidth = dissolveBurnWidth;
        _prev_dissolveBurnColor = dissolveBurnColor;
        _prev_dissolveTiling = dissolveTiling;
        _prev_dissolveBorderColor = dissolveBorderColor;
        _prev_glowDissolveEnabled = glowDissolveEnabled;
        _prev_glowDissolveMaskCh = glowDissolveMaskCh;
        _prev_glowDissolveNoiseType = glowDissolveNoiseType;
        _prev_glowDissolveTexture = glowDissolveTexture;
        _prev_glowDissolveNoiseScale = glowDissolveNoiseScale;
        _prev_glowDissolveAmount = glowDissolveAmount;
        _prev_glowDissolveColor = glowDissolveColor;
        _prev_glowDissolveWidth = glowDissolveWidth;
        _prev_srcDissolveEnabled = srcDissolveEnabled;
        _prev_srcDissolveMaskCh = srcDissolveMaskCh;
        _prev_srcDissolveAmount = srcDissolveAmount;
        _prev_srcDissolvePosition = srcDissolvePosition;
        _prev_srcDissolveBorderColor = srcDissolveBorderColor;
        _prev_srcDissolveBorderWidth = srcDissolveBorderWidth;
        _prev_dirFadeEnabled = dirFadeEnabled;
        _prev_dirFadeMaskCh = dirFadeMaskCh;
        _prev_dirFadeAngle = dirFadeAngle;
        _prev_dirFadeAmount = dirFadeAmount;
        _prev_dirFadeSmoothness = dirFadeSmoothness;
        _prev_dirGlowFadeEnabled = dirGlowFadeEnabled;
        _prev_dirGlowFadeMaskCh = dirGlowFadeMaskCh;
        _prev_dirGlowFadeAngle = dirGlowFadeAngle;
        _prev_dirGlowFadeAmount = dirGlowFadeAmount;
        _prev_dirGlowFadeColor = dirGlowFadeColor;
        _prev_dirGlowFadeWidth = dirGlowFadeWidth;
        _prev_fullAlphaDissolveEnabled = fullAlphaDissolveEnabled;
        _prev_fullAlphaDissolveMaskCh = fullAlphaDissolveMaskCh;
        _prev_fullAlphaDissolveAmount = fullAlphaDissolveAmount;
        _prev_halftoneEnabled = halftoneEnabled;
        _prev_halftoneMaskCh = halftoneMaskCh;
        _prev_halftoneSize = halftoneSize;
        _prev_halftoneAmount = halftoneAmount;
        _prev_halftoneSmoothness = halftoneSmoothness;
        _prev_radialWipeEnabled = radialWipeEnabled;
        _prev_radialWipeMaskCh = radialWipeMaskCh;
        _prev_radialWipeAmount = radialWipeAmount;
        _prev_radialWipeSmoothness = radialWipeSmoothness;
        _prev_radialWipeAngle = radialWipeAngle;
        _prev_blindsEnabled = blindsEnabled;
        _prev_blindsMaskCh = blindsMaskCh;
        _prev_blindsAmount = blindsAmount;
        _prev_blindsCount = blindsCount;
        _prev_blindsDirection = blindsDirection;
        _prev_diamondDissolveEnabled = diamondDissolveEnabled;
        _prev_diamondDissolveMaskCh = diamondDissolveMaskCh;
        _prev_diamondDissolveAmount = diamondDissolveAmount;
        _prev_diamondDissolveScale = diamondDissolveScale;
        _prev_scanlineDissolveEnabled = scanlineDissolveEnabled;
        _prev_scanlineDissolveMaskCh = scanlineDissolveMaskCh;
        _prev_scanlineDissolveAmount = scanlineDissolveAmount;
        _prev_scanlineDissolveDensity = scanlineDissolveDensity;
        _prev_spreadFadeEnabled = spreadFadeEnabled;
        _prev_spreadFadeMaskCh = spreadFadeMaskCh;
        _prev_spreadFadeAmount = spreadFadeAmount;
        _prev_spreadFadeOrigin = spreadFadeOrigin;
        _prev_spreadFadeSmoothness = spreadFadeSmoothness;
        _prev_pixelDissolveEnabled = pixelDissolveEnabled;
        _prev_pixelDissolveMaskCh = pixelDissolveMaskCh;
        _prev_pixelDissolveAmount = pixelDissolveAmount;
        _prev_pixelDissolveGrid = pixelDissolveGrid;
        _prev_spiralWipeEnabled = spiralWipeEnabled;
        _prev_spiralWipeMaskCh = spiralWipeMaskCh;
        _prev_spiralWipeAmount = spiralWipeAmount;
        _prev_spiralWipeSmoothness = spiralWipeSmoothness;
        _prev_spiralWipeArms = spiralWipeArms;
        _prev_hexDissolveEnabled = hexDissolveEnabled;
        _prev_hexDissolveMaskCh = hexDissolveMaskCh;
        _prev_hexDissolveAmount = hexDissolveAmount;
        _prev_hexDissolveScale = hexDissolveScale;
        _prev_hologramEnabled = hologramEnabled;
        _prev_hologramMaskCh = hologramMaskCh;
        _prev_hologramStripesAmount = hologramStripesAmount;
        _prev_hologramMinAlpha = hologramMinAlpha;
        _prev_hologramStripesSpeed = hologramStripesSpeed;
        _prev_hologramColor = hologramColor;
        _prev_hologramBlend = hologramBlend;
        _prev_hologramDistortion = hologramDistortion;
        _prev_glitchEnabled = glitchEnabled;
        _prev_glitchMaskCh = glitchMaskCh;
        _prev_glitchAmount = glitchAmount;
        _prev_glitchSpeed = glitchSpeed;
        _prev_glitchSize = glitchSize;
        _prev_glitchColorShift = glitchColorShift;
        _prev_flickerEnabled = flickerEnabled;
        _prev_flickerMaskCh = flickerMaskCh;
        _prev_flickerFreq = flickerFreq;
        _prev_flickerPercent = flickerPercent;
        _prev_shineEnabled = shineEnabled;
        _prev_shineMaskCh = shineMaskCh;
        _prev_shineColor = shineColor;
        _prev_shineWidth = shineWidth;
        _prev_shineSpeed = shineSpeed;
        _prev_shineDelay = shineDelay;
        _prev_shineRotation = shineRotation;
        _prev_shineGlow = shineGlow;
        _prev_smokeEnabled = smokeEnabled;
        _prev_smokeMaskCh = smokeMaskCh;
        _prev_smokeColor = smokeColor;
        _prev_smokeDensity = smokeDensity;
        _prev_smokeSpeed = smokeSpeed;
        _prev_smokeTurbulence = smokeTurbulence;
        _prev_smokeDirection = smokeDirection;
        _prev_smokeColorStrength = smokeColorStrength;
        _prev_smokeFade = smokeFade;
        _prev_flameEnabled = flameEnabled;
        _prev_flameMaskCh = flameMaskCh;
        _prev_flameColor1 = flameColor1;
        _prev_flameColor2 = flameColor2;
        _prev_flameSpeed = flameSpeed;
        _prev_flameDensity = flameDensity;
        _prev_flameOnlyMode = flameOnlyMode;
        _prev_flameExtendSides = flameExtendSides;
        _prev_flameSideFalloff = flameSideFalloff;
        _prev_flameIntensity = flameIntensity;
        _prev_enchantedEnabled = enchantedEnabled;
        _prev_enchantedMaskCh = enchantedMaskCh;
        _prev_enchantedColor = enchantedColor;
        _prev_enchantedSpeed = enchantedSpeed;
        _prev_enchantedDensity = enchantedDensity;
        _prev_enchantedBrightness = enchantedBrightness;
        _prev_enchantedFade = enchantedFade;
        _prev_shiftingEnabled = shiftingEnabled;
        _prev_shiftingMaskCh = shiftingMaskCh;
        _prev_shiftingColor1 = shiftingColor1;
        _prev_shiftingColor2 = shiftingColor2;
        _prev_shiftingSpeed = shiftingSpeed;
        _prev_shiftingFade = shiftingFade;
        _prev_shiftRainbowEnabled = shiftRainbowEnabled;
        _prev_shiftRainbowMaskCh = shiftRainbowMaskCh;
        _prev_shiftRainbowSpeed = shiftRainbowSpeed;
        _prev_shiftRainbowDensity = shiftRainbowDensity;
        _prev_shiftRainbowFade = shiftRainbowFade;
        _prev_overlayEnabled = overlayEnabled;
        _prev_overlayMaskCh = overlayMaskCh;
        _prev_overlayTexture = overlayTexture;
        _prev_overlayColor = overlayColor;
        _prev_overlayBlend = overlayBlend;
        _prev_overlayTiling = overlayTiling;
        _prev_overlaySpeed = overlaySpeed;
        _prev_checkerboardEnabled = checkerboardEnabled;
        _prev_checkerboardMaskCh = checkerboardMaskCh;
        _prev_checkerCoordSpace = checkerCoordSpace;
        _prev_checkerScale = checkerScale;
        _prev_checkerColor1 = checkerColor1;
        _prev_checkerColor2 = checkerColor2;
        _prev_uvVisualizeEnabled = uvVisualizeEnabled;
        _prev_uvVisualizeMaskCh = uvVisualizeMaskCh;
        _prev_uvVisualizeBlend = uvVisualizeBlend;
        _prev_healthTintEnabled = healthTintEnabled;
        _prev_healthTintMaskCh = healthTintMaskCh;
        _prev_healthTintLowColor = healthTintLowColor;
        _prev_healthTintHighColor = healthTintHighColor;
        _prev_healthTintAmount = healthTintAmount;
        _prev_healthDamageNoise = healthDamageNoise;
        _prev_healthEdgeGrain = healthEdgeGrain;
        _prev_healthPulseSpeed = healthPulseSpeed;
        _prev_healthVignetteStr = healthVignetteStr;
        _prev_healthDesaturation = healthDesaturation;
        _prev_xraySilhouetteEnabled = xraySilhouetteEnabled;
        _prev_xraySilhouetteMaskCh = xraySilhouetteMaskCh;
        _prev_xraySilhouetteColor = xraySilhouetteColor;
        _prev_xraySilhouetteBlend = xraySilhouetteBlend;
        _prev_minimapPingEnabled = minimapPingEnabled;
        _prev_minimapPingMaskCh = minimapPingMaskCh;
        _prev_minimapPingColor = minimapPingColor;
        _prev_minimapPingSpeed = minimapPingSpeed;
        _prev_minimapPingWidth = minimapPingWidth;
        _prev_minimapPingOffset = minimapPingOffset;
        _prev_dialogueEmphasisEnabled = dialogueEmphasisEnabled;
        _prev_dialogueEmphasisMaskCh = dialogueEmphasisMaskCh;
        _prev_dialogueBobSpeed = dialogueBobSpeed;
        _prev_dialogueBobAmount = dialogueBobAmount;
        _prev_dialogueBrightness = dialogueBrightness;
        _prev_voronoiCrackleEnabled = voronoiCrackleEnabled;
        _prev_crackleMaskCh = crackleMaskCh;
        _prev_crackleScale = crackleScale;
        _prev_crackleSpeed = crackleSpeed;
        _prev_crackleEdgeWidth = crackleEdgeWidth;
        _prev_crackleEdgeColor = crackleEdgeColor;
        _prev_crackleCellTint = crackleCellTint;
        _prev_crackleFade = crackleFade;
        _prev_waterReflectEnabled = waterReflectEnabled;
        _prev_waterReflectMaskCh = waterReflectMaskCh;
        _prev_waterReflectTint = waterReflectTint;
        _prev_waterReflectYPoint = waterReflectYPoint;
        _prev_waterReflectDepth = waterReflectDepth;
        _prev_waterReflectOpacity = waterReflectOpacity;
        _prev_waterReflectTintAmount = waterReflectTintAmount;
        _prev_waterReflectWaveFreq = waterReflectWaveFreq;
        _prev_waterReflectWaveSpeed = waterReflectWaveSpeed;
        _prev_waterReflectWaveAmp = waterReflectWaveAmp;
        _prev_waterReflectFade = waterReflectFade;
        _prev_motionSmearEnabled = motionSmearEnabled;
        _prev_smearMaskCh = smearMaskCh;
        _prev_smearDirection = smearDirection;
        _prev_smearAmount = smearAmount;
        _prev_smearSamples = smearSamples;
        _prev_smearFade = smearFade;
        _prev_energyAuraEnabled = energyAuraEnabled;
        _prev_auraMaskCh = auraMaskCh;
        _prev_auraColor = auraColor;
        _prev_auraRadius = auraRadius;
        _prev_auraFrequency = auraFrequency;
        _prev_auraSpeed = auraSpeed;
        _prev_auraIntensity = auraIntensity;
        _prev_auraFade = auraFade;
        _prev_gradientEnabled = gradientEnabled;
        _prev_gradientMaskCh = gradientMaskCh;
        _prev_gradientColorA = gradientColorA;
        _prev_gradientColorB = gradientColorB;
        _prev_gradientAngle = gradientAngle;
        _prev_gradientFade = gradientFade;
        _prev_gradientUseGradientEditor = gradientUseGradientEditor;
        _prev_gradientBakedTexture = gradientBakedTexture;
        _prev_gradientBlendMode = gradientBlendMode;
        _prev_radialGradientEnabled = radialGradientEnabled;
        _prev_radialGradMaskCh = radialGradMaskCh;
        _prev_radialGradCenterColor = radialGradCenterColor;
        _prev_radialGradEdgeColor = radialGradEdgeColor;
        _prev_radialGradCenter = radialGradCenter;
        _prev_radialGradRadius = radialGradRadius;
        _prev_radialGradFade = radialGradFade;
        _prev_radialGradUseGradientEditor = radialGradUseGradientEditor;
        _prev_radialGradBakedTexture = radialGradBakedTexture;
        _prev_radialGradBlendMode = radialGradBlendMode;
        _prev_colorRampEnabled = colorRampEnabled;
        _prev_colorRampMaskCh = colorRampMaskCh;
        _prev_colorRampTexture = colorRampTexture;
        _prev_colorRampMode = colorRampMode;
        _prev_colorRampIntensity = colorRampIntensity;
        _prev_colorRampColorA = colorRampColorA;
        _prev_colorRampColorB = colorRampColorB;
        _prev_colorRampFade = colorRampFade;
        _prev_fakeLightingEnabled = fakeLightingEnabled;
        _prev_fakeLightMaskCh = fakeLightMaskCh;
        _prev_fakeLightDirection = fakeLightDirection;
        _prev_fakeLightColor = fakeLightColor;
        _prev_fakeLightIntensity = fakeLightIntensity;
        _prev_fakeLightAmbient = fakeLightAmbient;
        _prev_fakeLightNormalDist = fakeLightNormalDist;
        _prev_fakeLightNormalZ = fakeLightNormalZ;
        _prev_fakeLightSpecPower = fakeLightSpecPower;
        _prev_fakeLightSpecIntensity = fakeLightSpecIntensity;
        _prev_fakeLightFade = fakeLightFade;
        _prev_uvBendPointEnabled = uvBendPointEnabled;
        _prev_bendMaskCh = bendMaskCh;
        _prev_bendLocalPoint = bendLocalPoint;
        _prev_bendWorldPoint = bendWorldPoint;
        _prev_bendUseWorldSpace = bendUseWorldSpace;
        _prev_bendWorldScale = bendWorldScale;
        _prev_bendRadius = bendRadius;
        _prev_bendAmount = bendAmount;
        _prev_bendFalloff = bendFalloff;
        _prev_bendInvert = bendInvert;
        _prev_burnDirectionEnabled = burnDirectionEnabled;
        _prev_burnDirMaskCh = burnDirMaskCh;
        _prev_burnDirAngle = burnDirAngle;
        _prev_burnDirAmount = burnDirAmount;
        _prev_burnDirEdgeWidth = burnDirEdgeWidth;
        _prev_burnDirFireColor = burnDirFireColor;
        _prev_burnDirCharcoalColor = burnDirCharcoalColor;
        _prev_burnDirCharcoalWidth = burnDirCharcoalWidth;
        _prev_burnDirNoiseScale = burnDirNoiseScale;
        _prev_globalSmokeEnabled = globalSmokeEnabled;
        _prev_globalSmokeMaskCh = globalSmokeMaskCh;
        _prev_globalSmokeColor = globalSmokeColor;
        _prev_globalSmokeDensity = globalSmokeDensity;
        _prev_globalSmokeSpeed = globalSmokeSpeed;
        _prev_globalSmokeOpacity = globalSmokeOpacity;
        _prev_globalSmokeDirection = globalSmokeDirection;
        _prev_globalSmokeOverlayOnly = globalSmokeOverlayOnly;
        _prev_holographicCardEnabled = holographicCardEnabled;
        _prev_holoCardMaskCh = holoCardMaskCh;
        _prev_holoCardDensity = holoCardDensity;
        _prev_holoCardSpeed = holoCardSpeed;
        _prev_holoCardSaturation = holoCardSaturation;
        _prev_holoCardBrightness = holoCardBrightness;
        _prev_holoCardFresnelPower = holoCardFresnelPower;
        _prev_holoCardFresnelShift = holoCardFresnelShift;
        _prev_holoCardScanDensity = holoCardScanDensity;
        _prev_holoCardScanSpeed = holoCardScanSpeed;
        _prev_holoCardScanIntensity = holoCardScanIntensity;
        _prev_holoCardFade = holoCardFade;
        _prev_cloudShadowsEnabled = cloudShadowsEnabled;
        _prev_cloudShadowMaskCh = cloudShadowMaskCh;
        _prev_cloudShadowScale = cloudShadowScale;
        _prev_cloudShadowSpeed = cloudShadowSpeed;
        _prev_cloudShadowDarkness = cloudShadowDarkness;
        _prev_cloudShadowThreshold = cloudShadowThreshold;
        _prev_cloudShadowSoftness = cloudShadowSoftness;
        _prev_cloudShadowFade = cloudShadowFade;
        _prev_cloudShadowOverlayOnly = cloudShadowOverlayOnly;
        _prev_colorfulTrailAEnabled = colorfulTrailAEnabled;
        _prev_trailAMaskCh = trailAMaskCh;
        _prev_trailAVelocity = trailAVelocity;
        _prev_trailASlowColor = trailASlowColor;
        _prev_trailAFastColor = trailAFastColor;
        _prev_trailAMaxSpeed = trailAMaxSpeed;
        _prev_trailACurvePower = trailACurvePower;
        _prev_trailAIntensity = trailAIntensity;
        _prev_colorfulTrailBEnabled = colorfulTrailBEnabled;
        _prev_trailBMaskCh = trailBMaskCh;
        _prev_trailBVelocity = trailBVelocity;
        _prev_trailBColor = trailBColor;
        _prev_trailBMaxSpeed = trailBMaxSpeed;
        _prev_trailBCurvePower = trailBCurvePower;
        _prev_trailBIntensity = trailBIntensity;
        _prev_trailBSegments = trailBSegments;
        _prev_trailBFadeOut = trailBFadeOut;
        _prev_colorfulTrailCEnabled = colorfulTrailCEnabled;
        _prev_trailCMaskCh = trailCMaskCh;
        _prev_trailCVelocity = trailCVelocity;
        _prev_trailCColorA = trailCColorA;
        _prev_trailCColorB = trailCColorB;
        _prev_trailCMaxSpeed = trailCMaxSpeed;
        _prev_trailCCurvePower = trailCCurvePower;
        _prev_trailCIntensity = trailCIntensity;
        _prev_trailCRainbowSpeed = trailCRainbowSpeed;
        _prev_chargingAbilityEnabled = chargingAbilityEnabled;
        _prev_chargeMaskCh = chargeMaskCh;
        _prev_chargeColor = chargeColor;
        _prev_chargeCenter = chargeCenter;
        _prev_chargeRadius = chargeRadius;
        _prev_chargeWidth = chargeWidth;
        _prev_chargeSpeed = chargeSpeed;
        _prev_chargeFrequency = chargeFrequency;
        _prev_chargeDensity = chargeDensity;
        _prev_chargeIntensity = chargeIntensity;
        _prev_chargeLines = chargeLines;
        _prev_chargeFade = chargeFade;
        _prev_blueprintEnabled = blueprintEnabled;
        _prev_blueprintMaskCh = blueprintMaskCh;
        _prev_blueprintBgColor = blueprintBgColor;
        _prev_blueprintLineColor = blueprintLineColor;
        _prev_blueprintGridColor = blueprintGridColor;
        _prev_blueprintEdgeSize = blueprintEdgeSize;
        _prev_blueprintLineWidth = blueprintLineWidth;
        _prev_blueprintLineIntensity = blueprintLineIntensity;
        _prev_blueprintGridDensity = blueprintGridDensity;
        _prev_blueprintGridOpacity = blueprintGridOpacity;
        _prev_blueprintFade = blueprintFade;
        _prev_ghostingEnabled = ghostingEnabled;
        _prev_ghostingMaskCh = ghostingMaskCh;
        _prev_ghostingCount = ghostingCount;
        _prev_ghostingOffset = ghostingOffset;
        _prev_ghostingFade = ghostingFade;
        _prev_retroPaletteEnabled = retroPaletteEnabled;
        _prev_retroMaskCh = retroMaskCh;
        _prev_retroPixelSize = retroPixelSize;
        _prev_retroPaletteSize = retroPaletteSize;
        _prev_retroPaletteTex = retroPaletteTex;
        _prev_retroDitherStrength = retroDitherStrength;
        _prev_retroFade = retroFade;
    }
}
}
