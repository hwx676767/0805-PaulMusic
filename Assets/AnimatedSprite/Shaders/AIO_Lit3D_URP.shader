Shader "AnimatedSprite/3D Lit URP"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [HideInInspector] _ShadowAlphaClip ("Shadow Alpha Clip", Float) = 0.1
        [HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil ("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255

        // --- Auto-generated SRP Batcher Properties ---
        [HideInInspector] _MainTex_TexelSize ("_MainTex_TexelSize", Vector) = (0,0,0,0)
        [HideInInspector] _OutlineSDFTex_TexelSize ("_OutlineSDFTex_TexelSize", Vector) = (0,0,0,0)
        [HideInInspector] _OutlineEffectsOnly_En ("_OutlineEffectsOnly_En", Float) = 0
        [HideInInspector] _GlobalEffectFade ("_GlobalEffectFade", Float) = 0
        [HideInInspector] _EdgeCleanup ("_EdgeCleanup", Float) = 0
        [HideInInspector] _PixelSnapResolution ("_PixelSnapResolution", Float) = 0
        [HideInInspector] _TimeScale ("_TimeScale", Float) = 0
        [HideInInspector] _TimeFPS ("_TimeFPS", Float) = 0
        [HideInInspector] _UseCustomTime ("_UseCustomTime", Float) = 0
        [HideInInspector] _CustomTime ("_CustomTime", Float) = 0
        [HideInInspector] _CastShadows ("_CastShadows", Float) = 0
        [HideInInspector] _ReceiveShadows ("_ReceiveShadows", Float) = 0
        [HideInInspector] _FlipNormals ("_FlipNormals", Float) = 0
        [HideInInspector] _GlobalLightIntensity ("_GlobalLightIntensity", Float) = 0
        [HideInInspector] _AmbientLightIntensity ("_AmbientLightIntensity", Float) = 0
        [HideInInspector] _HDRPExposureComp ("_HDRPExposureComp", Float) = 0
        [HideInInspector] _HDRPAmbientMul ("_HDRPAmbientMul", Float) = 0
        [HideInInspector] _HDRPIndirectMul ("_HDRPIndirectMul", Float) = 0
        [HideInInspector] _EffectMask_En ("_EffectMask_En", Float) = 0
        [HideInInspector] _PixelArtSnap_En ("_PixelArtSnap_En", Float) = 0
        [HideInInspector] _MaskTex_En ("_MaskTex_En", Float) = 0
        [HideInInspector] _MinXUV ("_MinXUV", Float) = 0
        [HideInInspector] _MaxXUV ("_MaxXUV", Float) = 0
        [HideInInspector] _MinYUV ("_MinYUV", Float) = 0
        [HideInInspector] _MaxYUV ("_MaxYUV", Float) = 0
        [HideInInspector] _VertexUVExpand ("_VertexUVExpand", Float) = 0
        [HideInInspector] _SpriteAspect ("_SpriteAspect", Float) = 0
        [HideInInspector] _AlphaCutoff_En ("_AlphaCutoff_En", Float) = 0
        [HideInInspector] _AlphaCutoff ("_AlphaCutoff", Float) = 0
        [HideInInspector] _WorldTileOffset ("_WorldTileOffset", Vector) = (0,0,0,0)
        [HideInInspector] _WorldTileRotation ("_WorldTileRotation", Float) = 0
        [HideInInspector] _EnchantedMaskCh ("_EnchantedMaskCh", Float) = 0
        [HideInInspector] _CamoColor1 ("_CamoColor1", Vector) = (0,0,0,0)
        [HideInInspector] _CamoColor2 ("_CamoColor2", Vector) = (0,0,0,0)
        [HideInInspector] _CamoColor3 ("_CamoColor3", Vector) = (0,0,0,0)
        [HideInInspector] _CamoScale ("_CamoScale", Float) = 0
        [HideInInspector] _CamoSpeed ("_CamoSpeed", Float) = 0
        [HideInInspector] _CamoFade ("_CamoFade", Float) = 0
        [HideInInspector] _CRTDensity ("_CRTDensity", Float) = 0
        [HideInInspector] _CRTIntensity ("_CRTIntensity", Float) = 0
        [HideInInspector] _CRTSpeed ("_CRTSpeed", Float) = 0
        [HideInInspector] _CRTCurvature ("_CRTCurvature", Float) = 0
        [HideInInspector] _ThermalIntensity ("_ThermalIntensity", Float) = 0
        [HideInInspector] _ThermalHDRGlow ("_ThermalHDRGlow", Float) = 0
        [HideInInspector] _RecolorR ("_RecolorR", Vector) = (0,0,0,0)
        [HideInInspector] _RecolorG ("_RecolorG", Vector) = (0,0,0,0)
        [HideInInspector] _RecolorB ("_RecolorB", Vector) = (0,0,0,0)
        [HideInInspector] _RecolorCyan ("_RecolorCyan", Vector) = (0,0,0,0)
        [HideInInspector] _RecolorMagenta ("_RecolorMagenta", Vector) = (0,0,0,0)
        [HideInInspector] _RecolorYellow ("_RecolorYellow", Vector) = (0,0,0,0)
        [HideInInspector] _RecolorFade ("_RecolorFade", Float) = 0
        [HideInInspector] _DirOutlineColor ("_DirOutlineColor", Vector) = (0,0,0,0)
        [HideInInspector] _DirOutlineWidth ("_DirOutlineWidth", Float) = 0
        [HideInInspector] _DirOutlineInt ("_DirOutlineInt", Float) = 0
        [HideInInspector] _DirOutlineDir ("_DirOutlineDir", Vector) = (0,0,0,0)
        [HideInInspector] _DirOutlineSoft ("_DirOutlineSoft", Float) = 0
        [HideInInspector] _DirOutlineMaskCh ("_DirOutlineMaskCh", Float) = 0
        [HideInInspector] _DistortAmount ("_DistortAmount", Float) = 0
        [HideInInspector] _DistortSpeed ("_DistortSpeed", Float) = 0
        [HideInInspector] _DistortTiling ("_DistortTiling", Vector) = (0,0,0,0)
        [HideInInspector] _DistortIsNormalMap ("_DistortIsNormalMap", Float) = 0
        [HideInInspector] _WorldTileScale ("_WorldTileScale", Float) = 0
        [HideInInspector] _FlipBookLerp ("_FlipBookLerp", Float) = 0
        [HideInInspector] _EnchantedColor ("_EnchantedColor", Vector) = (0,0,0,0)
        [HideInInspector] _EnchantedSpeed ("_EnchantedSpeed", Float) = 0
        [HideInInspector] _EnchantedDensity ("_EnchantedDensity", Float) = 0
        [HideInInspector] _EnchantedBrightness ("_EnchantedBrightness", Float) = 0
        [HideInInspector] _EnchantedFade ("_EnchantedFade", Float) = 0
        [HideInInspector] _CheckerCoordSpace ("_CheckerCoordSpace", Float) = 0
        [HideInInspector] _CheckerScale ("_CheckerScale", Float) = 0
        [HideInInspector] _CheckerColor1 ("_CheckerColor1", Vector) = (0,0,0,0)
        [HideInInspector] _CheckerColor2 ("_CheckerColor2", Vector) = (0,0,0,0)
        [HideInInspector] _HealthDamageNoise ("_HealthDamageNoise", Float) = 0
        [HideInInspector] _HealthEdgeGrain ("_HealthEdgeGrain", Float) = 0
        [HideInInspector] _HealthPulseSpeed ("_HealthPulseSpeed", Float) = 0
        [HideInInspector] _HealthVignetteStr ("_HealthVignetteStr", Float) = 0
        [HideInInspector] _HealthDesaturation ("_HealthDesaturation", Float) = 0
        [HideInInspector] _DialogueBobSpeed ("_DialogueBobSpeed", Float) = 0
        [HideInInspector] _DialogueBobAmount ("_DialogueBobAmount", Float) = 0
        [HideInInspector] _DialogueBrightness ("_DialogueBrightness", Float) = 0
        [HideInInspector] _CrackleScale ("_CrackleScale", Float) = 0
        [HideInInspector] _CrackleSpeed ("_CrackleSpeed", Float) = 0
        [HideInInspector] _CrackleEdgeWidth ("_CrackleEdgeWidth", Float) = 0
        [HideInInspector] _CrackleEdgeColor ("_CrackleEdgeColor", Vector) = (0,0,0,0)
        [HideInInspector] _CrackleCellTint ("_CrackleCellTint", Float) = 0
        [HideInInspector] _CrackleFade ("_CrackleFade", Float) = 0
        [HideInInspector] _CrackleMaskCh ("_CrackleMaskCh", Float) = 0
        [HideInInspector] _InkBleedAmount ("_InkBleedAmount", Float) = 0
        [HideInInspector] _InkBleedWidth ("_InkBleedWidth", Float) = 0
        [HideInInspector] _InkBleedScale ("_InkBleedScale", Float) = 0
        [HideInInspector] _PaperScale ("_PaperScale", Float) = 0
        [HideInInspector] _PaperAmount ("_PaperAmount", Float) = 0
        [HideInInspector] _InkBleedFade ("_InkBleedFade", Float) = 0
        [HideInInspector] _InkBleedMaskCh ("_InkBleedMaskCh", Float) = 0
        [HideInInspector] _AuraColor ("_AuraColor", Vector) = (0,0,0,0)
        [HideInInspector] _AuraRadius ("_AuraRadius", Float) = 0
        [HideInInspector] _AuraFrequency ("_AuraFrequency", Float) = 0
        [HideInInspector] _AuraSpeed ("_AuraSpeed", Float) = 0
        [HideInInspector] _AuraIntensity ("_AuraIntensity", Float) = 0
        [HideInInspector] _AuraFade ("_AuraFade", Float) = 0
        [HideInInspector] _AuraMaskCh ("_AuraMaskCh", Float) = 0
        [HideInInspector] _RetroPixelSize ("_RetroPixelSize", Float) = 0
        [HideInInspector] _RetroDitherStrength ("_RetroDitherStrength", Float) = 0
        [HideInInspector] _RetroFade ("_RetroFade", Float) = 0
        [HideInInspector] _RetroMaskCh ("_RetroMaskCh", Float) = 0
        [HideInInspector] _SmearDirection ("_SmearDirection", Vector) = (0,0,0,0)
        [HideInInspector] _SmearAmount ("_SmearAmount", Float) = 0
        [HideInInspector] _SmearSamples ("_SmearSamples", Float) = 0
        [HideInInspector] _SmearFade ("_SmearFade", Float) = 0
        [HideInInspector] _SmearMaskCh ("_SmearMaskCh", Float) = 0
        [HideInInspector] _ChromaMode ("_ChromaMode", Float) = 0
        [HideInInspector] _ChromaRadialCenter ("_ChromaRadialCenter", Vector) = (0,0,0,0)
        [HideInInspector] _OutlineMode ("_OutlineMode", Float) = 0
        [HideInInspector] _OutlineRings ("_OutlineRings", Float) = 0
        [HideInInspector] _OutlineAA ("_OutlineAA", Float) = 0
        [HideInInspector] _OutlineSDFSoftness ("_OutlineSDFSoftness", Float) = 0
        [HideInInspector] _OutlineSDFPadding ("_OutlineSDFPadding", Float) = 0
        [HideInInspector] _OutlineSDFSpread ("_OutlineSDFSpread", Float) = 0
        [HideInInspector] _OutlineSDFRect ("_OutlineSDFRect", Vector) = (0,0,0,0)
        [HideInInspector] _PremulAlpha ("_PremulAlpha", Float) = 0
        [HideInInspector] _SpriteFlipX ("_SpriteFlipX", Float) = 0
        [HideInInspector] _SpriteFlipY ("_SpriteFlipY", Float) = 0
        [HideInInspector] _LUTSize ("_LUTSize", Float) = 0
        [HideInInspector] _Enchanted_En ("_Enchanted_En", Float) = 0
        [HideInInspector] _EnergyAura_En ("_EnergyAura_En", Float) = 0
        [HideInInspector] _InkBleed_En ("_InkBleed_En", Float) = 0
        [HideInInspector] _StrongTintMaskCh ("_StrongTintMaskCh", Float) = 0
        [HideInInspector] _AddColorMaskCh ("_AddColorMaskCh", Float) = 0
        [HideInInspector] _AlphaTintMaskCh ("_AlphaTintMaskCh", Float) = 0
        [HideInInspector] _HitEffectMaskCh ("_HitEffectMaskCh", Float) = 0
        [HideInInspector] _NegativeMaskCh ("_NegativeMaskCh", Float) = 0
        [HideInInspector] _GreyscaleMaskCh ("_GreyscaleMaskCh", Float) = 0
        [HideInInspector] _SaturationMaskCh ("_SaturationMaskCh", Float) = 0
        [HideInInspector] _BrightnessMaskCh ("_BrightnessMaskCh", Float) = 0
        [HideInInspector] _ContrastMaskCh ("_ContrastMaskCh", Float) = 0
        [HideInInspector] _ColorGradingMaskCh ("_ColorGradingMaskCh", Float) = 0
        [HideInInspector] _SepiaMaskCh ("_SepiaMaskCh", Float) = 0
        [HideInInspector] _PingPongGlowMaskCh ("_PingPongGlowMaskCh", Float) = 0
        [HideInInspector] _ColorCycleMaskCh ("_ColorCycleMaskCh", Float) = 0
        [HideInInspector] _StrongTintColor ("_StrongTintColor", Vector) = (0,0,0,0)
        [HideInInspector] _StrongTintFade ("_StrongTintFade", Float) = 0
        [HideInInspector] _AddColorAmount ("_AddColorAmount", Vector) = (0,0,0,0)
        [HideInInspector] _AddColorFade ("_AddColorFade", Float) = 0
        [HideInInspector] _AlphaTintColor ("_AlphaTintColor", Vector) = (0,0,0,0)
        [HideInInspector] _AlphaTintFade ("_AlphaTintFade", Float) = 0
        [HideInInspector] _HitEffectColor ("_HitEffectColor", Vector) = (0,0,0,0)
        [HideInInspector] _HitEffectBlend ("_HitEffectBlend", Float) = 0
        [HideInInspector] _NegativeAmount ("_NegativeAmount", Float) = 0
        [HideInInspector] _GreyscaleBlend ("_GreyscaleBlend", Float) = 0
        [HideInInspector] _GreyscaleLuminosity ("_GreyscaleLuminosity", Vector) = (0,0,0,0)
        [HideInInspector] _SaturationAmount ("_SaturationAmount", Float) = 0
        [HideInInspector] _BrightnessAmount ("_BrightnessAmount", Float) = 0
        [HideInInspector] _ContrastAmount ("_ContrastAmount", Float) = 0
        [HideInInspector] _CGExposure ("_CGExposure", Float) = 0
        [HideInInspector] _CGGamma ("_CGGamma", Float) = 0
        [HideInInspector] _CGLift ("_CGLift", Vector) = (0,0,0,0)
        [HideInInspector] _SepiaIntensity ("_SepiaIntensity", Float) = 0
        [HideInInspector] _SepiaColor ("_SepiaColor", Vector) = (0,0,0,0)
        [HideInInspector] _PingPongGlowColor ("_PingPongGlowColor", Vector) = (0,0,0,0)
        [HideInInspector] _PingPongGlowSpeed ("_PingPongGlowSpeed", Float) = 0
        [HideInInspector] _PingPongGlowMin ("_PingPongGlowMin", Float) = 0
        [HideInInspector] _PingPongGlowMax ("_PingPongGlowMax", Float) = 0
        [HideInInspector] _ColorCycleA ("_ColorCycleA", Vector) = (0,0,0,0)
        [HideInInspector] _ColorCycleB ("_ColorCycleB", Vector) = (0,0,0,0)
        [HideInInspector] _ColorCycleC ("_ColorCycleC", Vector) = (0,0,0,0)
        [HideInInspector] _ColorCycleD ("_ColorCycleD", Vector) = (0,0,0,0)
        [HideInInspector] _ColorCycleSpeed ("_ColorCycleSpeed", Float) = 0
        [HideInInspector] _ColorCycleFade ("_ColorCycleFade", Float) = 0
        [HideInInspector] _AddColor_En ("_AddColor_En", Float) = 0
        [HideInInspector] _AlphaTint_En ("_AlphaTint_En", Float) = 0
        [HideInInspector] _Brightness_En ("_Brightness_En", Float) = 0
        [HideInInspector] _ColorCycle_En ("_ColorCycle_En", Float) = 0
        [HideInInspector] _ColorGrading_En ("_ColorGrading_En", Float) = 0
        [HideInInspector] _Contrast_En ("_Contrast_En", Float) = 0
        [HideInInspector] _Greyscale_En ("_Greyscale_En", Float) = 0
        [HideInInspector] _HitEffect_En ("_HitEffect_En", Float) = 0
        [HideInInspector] _Negative_En ("_Negative_En", Float) = 0
        [HideInInspector] _PingPongGlow_En ("_PingPongGlow_En", Float) = 0
        [HideInInspector] _Saturation_En ("_Saturation_En", Float) = 0
        [HideInInspector] _Sepia_En ("_Sepia_En", Float) = 0
        [HideInInspector] _StrongTint_En ("_StrongTint_En", Float) = 0
        [HideInInspector] _BloomMaskCh ("_BloomMaskCh", Float) = 0
        [HideInInspector] _ColorReplaceMaskCh ("_ColorReplaceMaskCh", Float) = 0
        [HideInInspector] _LUTReplaceMaskCh ("_LUTReplaceMaskCh", Float) = 0
        [HideInInspector] _HueShiftMaskCh ("_HueShiftMaskCh", Float) = 0
        [HideInInspector] _AddHueMaskCh ("_AddHueMaskCh", Float) = 0
        [HideInInspector] _SplitToningMaskCh ("_SplitToningMaskCh", Float) = 0
        [HideInInspector] _PosterizeMaskCh ("_PosterizeMaskCh", Float) = 0
        [HideInInspector] _PoisonMaskCh ("_PoisonMaskCh", Float) = 0
        [HideInInspector] _FrozenMaskCh ("_FrozenMaskCh", Float) = 0
        [HideInInspector] _MetalMaskCh ("_MetalMaskCh", Float) = 0
        [HideInInspector] _CamouflageMaskCh ("_CamouflageMaskCh", Float) = 0
        [HideInInspector] _RainbowMaskCh ("_RainbowMaskCh", Float) = 0
        [HideInInspector] _BlackTintMaskCh ("_BlackTintMaskCh", Float) = 0
        [HideInInspector] _WaterReflectMaskCh ("_WaterReflectMaskCh", Float) = 0
        [HideInInspector] _ChromaticMaskCh ("_ChromaticMaskCh", Float) = 0
        [HideInInspector] _PaletteSwapMaskCh ("_PaletteSwapMaskCh", Float) = 0
        [HideInInspector] _DitheringMaskCh ("_DitheringMaskCh", Float) = 0
        [HideInInspector] _CRTScanlinesMaskCh ("_CRTScanlinesMaskCh", Float) = 0
        [HideInInspector] _VignetteMaskCh ("_VignetteMaskCh", Float) = 0
        [HideInInspector] _ThermalVisionMaskCh ("_ThermalVisionMaskCh", Float) = 0
        [HideInInspector] _RecolorRGBMaskCh ("_RecolorRGBMaskCh", Float) = 0
        [HideInInspector] _RainbowOutlineMaskCh ("_RainbowOutlineMaskCh", Float) = 0
        [HideInInspector] _BloomMode ("_BloomMode", Float) = 0
        [HideInInspector] _BloomColor ("_BloomColor", Vector) = (0,0,0,0)
        [HideInInspector] _BloomThreshold ("_BloomThreshold", Float) = 0
        [HideInInspector] _BloomIntensity ("_BloomIntensity", Float) = 0
        [HideInInspector] _BloomSize ("_BloomSize", Float) = 0
        [HideInInspector] _BloomSamples ("_BloomSamples", Float) = 0
        [HideInInspector] _BloomSoftKnee ("_BloomSoftKnee", Float) = 0
        [HideInInspector] _BloomMaskColor ("_BloomMaskColor", Vector) = (0,0,0,0)
        [HideInInspector] _BloomMaskRange ("_BloomMaskRange", Float) = 0
        [HideInInspector] _BloomMaskSoft ("_BloomMaskSoft", Float) = 0
        [HideInInspector] _BloomAlphaBleed ("_BloomAlphaBleed", Float) = 0
        [HideInInspector] _ColorReplaceSrc1 ("_ColorReplaceSrc1", Vector) = (0,0,0,0)
        [HideInInspector] _ColorReplaceDst1 ("_ColorReplaceDst1", Vector) = (0,0,0,0)
        [HideInInspector] _ColorReplaceSrc2 ("_ColorReplaceSrc2", Vector) = (0,0,0,0)
        [HideInInspector] _ColorReplaceDst2 ("_ColorReplaceDst2", Vector) = (0,0,0,0)
        [HideInInspector] _ColorReplaceSrc3 ("_ColorReplaceSrc3", Vector) = (0,0,0,0)
        [HideInInspector] _ColorReplaceDst3 ("_ColorReplaceDst3", Vector) = (0,0,0,0)
        [HideInInspector] _ColorReplaceSrc4 ("_ColorReplaceSrc4", Vector) = (0,0,0,0)
        [HideInInspector] _ColorReplaceDst4 ("_ColorReplaceDst4", Vector) = (0,0,0,0)
        [HideInInspector] _ColorReplaceCount ("_ColorReplaceCount", Float) = 0
        [HideInInspector] _ColorReplaceRange ("_ColorReplaceRange", Float) = 0
        [HideInInspector] _ColorReplaceFade ("_ColorReplaceFade", Float) = 0
        [HideInInspector] _LUTReplaceFade ("_LUTReplaceFade", Float) = 0
        [HideInInspector] _HueShiftAmount ("_HueShiftAmount", Float) = 0
        [HideInInspector] _HueShiftSpeed ("_HueShiftSpeed", Float) = 0
        [HideInInspector] _HueShiftFade ("_HueShiftFade", Float) = 0
        [HideInInspector] _AddHueAmount ("_AddHueAmount", Float) = 0
        [HideInInspector] _AddHueSpeed ("_AddHueSpeed", Float) = 0
        [HideInInspector] _AddHueSaturation ("_AddHueSaturation", Float) = 0
        [HideInInspector] _AddHueBrightness ("_AddHueBrightness", Float) = 0
        [HideInInspector] _AddHueFade ("_AddHueFade", Float) = 0
        [HideInInspector] _SplitShadowColor ("_SplitShadowColor", Vector) = (0,0,0,0)
        [HideInInspector] _SplitMidColor ("_SplitMidColor", Vector) = (0,0,0,0)
        [HideInInspector] _SplitHighlightColor ("_SplitHighlightColor", Vector) = (0,0,0,0)
        [HideInInspector] _SplitBalance ("_SplitBalance", Float) = 0
        [HideInInspector] _SplitFade ("_SplitFade", Float) = 0
        [HideInInspector] _PosterizeSteps ("_PosterizeSteps", Float) = 0
        [HideInInspector] _PosterizeFade ("_PosterizeFade", Float) = 0
        [HideInInspector] _PoisonColor ("_PoisonColor", Vector) = (0,0,0,0)
        [HideInInspector] _PoisonColor2 ("_PoisonColor2", Vector) = (0,0,0,0)
        [HideInInspector] _PoisonColor3 ("_PoisonColor3", Vector) = (0,0,0,0)
        [HideInInspector] _PoisonSpeed ("_PoisonSpeed", Float) = 0
        [HideInInspector] _PoisonDensity ("_PoisonDensity", Float) = 0
        [HideInInspector] _PoisonFade ("_PoisonFade", Float) = 0
        [HideInInspector] _FrozenColor ("_FrozenColor", Vector) = (0,0,0,0)
        [HideInInspector] _FrozenColor2 ("_FrozenColor2", Vector) = (0,0,0,0)
        [HideInInspector] _FrozenHighlightColor ("_FrozenHighlightColor", Vector) = (0,0,0,0)
        [HideInInspector] _FrozenContrast ("_FrozenContrast", Float) = 0
        [HideInInspector] _FrozenHighlight ("_FrozenHighlight", Float) = 0
        [HideInInspector] _FrozenFade ("_FrozenFade", Float) = 0
        [HideInInspector] _MetalColor ("_MetalColor", Vector) = (0,0,0,0)
        [HideInInspector] _MetalColor2 ("_MetalColor2", Vector) = (0,0,0,0)
        [HideInInspector] _MetalHighlight ("_MetalHighlight", Vector) = (0,0,0,0)
        [HideInInspector] _MetalContrast ("_MetalContrast", Float) = 0
        [HideInInspector] _MetalSpeed ("_MetalSpeed", Float) = 0
        [HideInInspector] _MetalFade ("_MetalFade", Float) = 0
        [HideInInspector] _RainbowSpeed ("_RainbowSpeed", Float) = 0
        [HideInInspector] _RainbowSaturation ("_RainbowSaturation", Float) = 0
        [HideInInspector] _RainbowBrightness ("_RainbowBrightness", Float) = 0
        [HideInInspector] _RainbowFade ("_RainbowFade", Float) = 0
        [HideInInspector] _RainbowDensity ("_RainbowDensity", Float) = 0
        [HideInInspector] _BlackTintColor ("_BlackTintColor", Vector) = (0,0,0,0)
        [HideInInspector] _BlackTintThreshold ("_BlackTintThreshold", Float) = 0
        [HideInInspector] _BlackTintFade ("_BlackTintFade", Float) = 0
        [HideInInspector] _WaterReflectYPoint ("_WaterReflectYPoint", Float) = 0
        [HideInInspector] _WaterReflectDepth ("_WaterReflectDepth", Float) = 0
        [HideInInspector] _WaterReflectOpacity ("_WaterReflectOpacity", Float) = 0
        [HideInInspector] _WaterReflectFade ("_WaterReflectFade", Float) = 0
        [HideInInspector] _WaterReflectTint ("_WaterReflectTint", Vector) = (0,0,0,0)
        [HideInInspector] _WaterReflectTintAmount ("_WaterReflectTintAmount", Float) = 0
        [HideInInspector] _WaterReflectWaveFreq ("_WaterReflectWaveFreq", Float) = 0
        [HideInInspector] _WaterReflectWaveSpeed ("_WaterReflectWaveSpeed", Float) = 0
        [HideInInspector] _WaterReflectWaveAmp ("_WaterReflectWaveAmp", Float) = 0
        [HideInInspector] _ChromaticAmount ("_ChromaticAmount", Float) = 0
        [HideInInspector] _ChromaticAngle ("_ChromaticAngle", Float) = 0
        [HideInInspector] _PaletteSwapColors ("_PaletteSwapColors", Float) = 0
        [HideInInspector] _PaletteSwapFade ("_PaletteSwapFade", Float) = 0
        [HideInInspector] _DitheringSize ("_DitheringSize", Float) = 0
        [HideInInspector] _DitheringLevels ("_DitheringLevels", Float) = 0
        [HideInInspector] _DitheringFade ("_DitheringFade", Float) = 0
        [HideInInspector] _DitheringAlphaMode ("_DitheringAlphaMode", Float) = 0
        [HideInInspector] _DitheringAlphaThreshold ("_DitheringAlphaThreshold", Float) = 0
        [HideInInspector] _DitheringScreenSpace ("_DitheringScreenSpace", Float) = 0
        [HideInInspector] _VignetteColor ("_VignetteColor", Vector) = (0,0,0,0)
        [HideInInspector] _VignetteIntensity ("_VignetteIntensity", Float) = 0
        [HideInInspector] _VignetteSmoothness ("_VignetteSmoothness", Float) = 0
        [HideInInspector] _VignetteAlpha ("_VignetteAlpha", Float) = 0
        [HideInInspector] _VignetteCenter ("_VignetteCenter", Vector) = (0,0,0,0)
        [HideInInspector] _VignetteOffset ("_VignetteOffset", Vector) = (0,0,0,0)
        [HideInInspector] _RainbowOutlineColor ("_RainbowOutlineColor", Vector) = (0,0,0,0)
        [HideInInspector] _RainbowOutlineSize ("_RainbowOutlineSize", Float) = 0
        [HideInInspector] _RainbowOutlineSpeed ("_RainbowOutlineSpeed", Float) = 0
        [HideInInspector] _RainbowOutlineDensity ("_RainbowOutlineDensity", Float) = 0
        [HideInInspector] _AddHue_En ("_AddHue_En", Float) = 0
        [HideInInspector] _BlackTint_En ("_BlackTint_En", Float) = 0
        [HideInInspector] _Bloom_En ("_Bloom_En", Float) = 0
        [HideInInspector] _Camouflage_En ("_Camouflage_En", Float) = 0
        [HideInInspector] _Chromatic_En ("_Chromatic_En", Float) = 0
        [HideInInspector] _ColorReplace_En ("_ColorReplace_En", Float) = 0
        [HideInInspector] _CRTScanlines_En ("_CRTScanlines_En", Float) = 0
        [HideInInspector] _Dithering_En ("_Dithering_En", Float) = 0
        [HideInInspector] _Frozen_En ("_Frozen_En", Float) = 0
        [HideInInspector] _HueShift_En ("_HueShift_En", Float) = 0
        [HideInInspector] _LUTReplace_En ("_LUTReplace_En", Float) = 0
        [HideInInspector] _Metal_En ("_Metal_En", Float) = 0
        [HideInInspector] _PaletteSwap_En ("_PaletteSwap_En", Float) = 0
        [HideInInspector] _Poison_En ("_Poison_En", Float) = 0
        [HideInInspector] _Posterize_En ("_Posterize_En", Float) = 0
        [HideInInspector] _RainbowOutline_En ("_RainbowOutline_En", Float) = 0
        [HideInInspector] _Rainbow_En ("_Rainbow_En", Float) = 0
        [HideInInspector] _RecolorRGB_En ("_RecolorRGB_En", Float) = 0
        [HideInInspector] _SplitToning_En ("_SplitToning_En", Float) = 0
        [HideInInspector] _ThermalVision_En ("_ThermalVision_En", Float) = 0
        [HideInInspector] _Vignette_En ("_Vignette_En", Float) = 0
        [HideInInspector] _WaterReflect_En ("_WaterReflect_En", Float) = 0
        [HideInInspector] _Gradient_En ("_Gradient_En", Float) = 0
        [HideInInspector] _GradientColorA ("_GradientColorA", Vector) = (0,0,0,0)
        [HideInInspector] _GradientColorB ("_GradientColorB", Vector) = (0,0,0,0)
        [HideInInspector] _GradientAngle ("_GradientAngle", Float) = 0
        [HideInInspector] _GradientFade ("_GradientFade", Float) = 0
        [HideInInspector] _GradientMaskCh ("_GradientMaskCh", Float) = 0
        [HideInInspector] _GradientUseTex ("_GradientUseTex", Float) = 0
        [HideInInspector] _GradientBlendMode ("_GradientBlendMode", Float) = 0
        [HideInInspector] _RadialGradient_En ("_RadialGradient_En", Float) = 0
        [HideInInspector] _RadialGradCenterColor ("_RadialGradCenterColor", Vector) = (0,0,0,0)
        [HideInInspector] _RadialGradEdgeColor ("_RadialGradEdgeColor", Vector) = (0,0,0,0)
        [HideInInspector] _RadialGradCenter ("_RadialGradCenter", Vector) = (0,0,0,0)
        [HideInInspector] _RadialGradRadius ("_RadialGradRadius", Float) = 0
        [HideInInspector] _RadialGradFade ("_RadialGradFade", Float) = 0
        [HideInInspector] _RadialGradMaskCh ("_RadialGradMaskCh", Float) = 0
        [HideInInspector] _RadialGradUseTex ("_RadialGradUseTex", Float) = 0
        [HideInInspector] _RadialGradBlendMode ("_RadialGradBlendMode", Float) = 0
        [HideInInspector] _ColorRamp_En ("_ColorRamp_En", Float) = 0
        [HideInInspector] _ColorRampMode ("_ColorRampMode", Float) = 0
        [HideInInspector] _ColorRampIntensity ("_ColorRampIntensity", Float) = 0
        [HideInInspector] _ColorRampFade ("_ColorRampFade", Float) = 0
        [HideInInspector] _ColorRampMaskCh ("_ColorRampMaskCh", Float) = 0
        [HideInInspector] _ColorRampColorA ("_ColorRampColorA", Vector) = (0,0,0,0)
        [HideInInspector] _ColorRampColorB ("_ColorRampColorB", Vector) = (0,0,0,0)
        [HideInInspector] _OutlineEffectWidth ("_OutlineEffectWidth", Float) = 0
        [HideInInspector] _OutlineFixedWidthPx ("_OutlineFixedWidthPx", Float) = 0
        [HideInInspector] _OutlineMaskCh ("_OutlineMaskCh", Float) = 0
        [HideInInspector] _InnerOutlineMaskCh ("_InnerOutlineMaskCh", Float) = 0
        [HideInInspector] _TexOutlineMaskCh ("_TexOutlineMaskCh", Float) = 0
        [HideInInspector] _DistortOutlineMaskCh ("_DistortOutlineMaskCh", Float) = 0
        [HideInInspector] _GlowMaskCh ("_GlowMaskCh", Float) = 0
        [HideInInspector] _ShadowMaskCh ("_ShadowMaskCh", Float) = 0
        [HideInInspector] _SoftShadowMaskCh ("_SoftShadowMaskCh", Float) = 0
        [HideInInspector] _OutlineOnlyMaskCh ("_OutlineOnlyMaskCh", Float) = 0
        [HideInInspector] _GradOutlineMaskCh ("_GradOutlineMaskCh", Float) = 0
        [HideInInspector] _AnimOutlineMaskCh ("_AnimOutlineMaskCh", Float) = 0
        [HideInInspector] _SelectHighlightMaskCh ("_SelectHighlightMaskCh", Float) = 0
        [HideInInspector] _GlowDissolveMaskCh ("_GlowDissolveMaskCh", Float) = 0
        [HideInInspector] _OutlineColor ("_OutlineColor", Vector) = (0,0,0,0)
        [HideInInspector] _OutlineSize ("_OutlineSize", Float) = 0
        [HideInInspector] _OutlineAlpha ("_OutlineAlpha", Float) = 0
        [HideInInspector] _InnerOutlineColor ("_InnerOutlineColor", Vector) = (0,0,0,0)
        [HideInInspector] _InnerOutlineThickness ("_InnerOutlineThickness", Float) = 0
        [HideInInspector] _InnerOutlineAlpha ("_InnerOutlineAlpha", Float) = 0
        [HideInInspector] _TexOutlineColor ("_TexOutlineColor", Vector) = (0,0,0,0)
        [HideInInspector] _TexOutlineSize ("_TexOutlineSize", Float) = 0
        [HideInInspector] _TexOutlineSpeed ("_TexOutlineSpeed", Vector) = (0,0,0,0)
        [HideInInspector] _TexOutlineTiling ("_TexOutlineTiling", Vector) = (0,0,0,0)
        [HideInInspector] _DistortOutlineColor ("_DistortOutlineColor", Vector) = (0,0,0,0)
        [HideInInspector] _DistortOutlineSize ("_DistortOutlineSize", Float) = 0
        [HideInInspector] _DistortOutlineAmount ("_DistortOutlineAmount", Float) = 0
        [HideInInspector] _DistortOutlineSpeed ("_DistortOutlineSpeed", Float) = 0
        [HideInInspector] _GlowColor ("_GlowColor", Vector) = (0,0,0,0)
        [HideInInspector] _GlowSize ("_GlowSize", Float) = 0
        [HideInInspector] _GlowIntensity ("_GlowIntensity", Float) = 0
        [HideInInspector] _ShadowColor ("_ShadowColor", Vector) = (0,0,0,0)
        [HideInInspector] _ShadowOffset ("_ShadowOffset", Vector) = (0,0,0,0)
        [HideInInspector] _SoftShadowColor ("_SoftShadowColor", Vector) = (0,0,0,0)
        [HideInInspector] _SoftShadowOffset ("_SoftShadowOffset", Vector) = (0,0,0,0)
        [HideInInspector] _SoftShadowSoftness ("_SoftShadowSoftness", Float) = 0
        [HideInInspector] _SoftShadowSamples ("_SoftShadowSamples", Float) = 0
        [HideInInspector] _OutlineOnlyColor ("_OutlineOnlyColor", Vector) = (0,0,0,0)
        [HideInInspector] _OutlineOnlyWidth ("_OutlineOnlyWidth", Float) = 0
        [HideInInspector] _GradOutlineColorA ("_GradOutlineColorA", Vector) = (0,0,0,0)
        [HideInInspector] _GradOutlineColorB ("_GradOutlineColorB", Vector) = (0,0,0,0)
        [HideInInspector] _GradOutlineSize ("_GradOutlineSize", Float) = 0
        [HideInInspector] _GradOutlineAngle ("_GradOutlineAngle", Float) = 0
        [HideInInspector] _AnimOutlineColor ("_AnimOutlineColor", Vector) = (0,0,0,0)
        [HideInInspector] _AnimOutlineMin ("_AnimOutlineMin", Float) = 0
        [HideInInspector] _AnimOutlineMax ("_AnimOutlineMax", Float) = 0
        [HideInInspector] _AnimOutlineSpeed ("_AnimOutlineSpeed", Float) = 0
        [HideInInspector] _SelectHighlightColor ("_SelectHighlightColor", Vector) = (0,0,0,0)
        [HideInInspector] _SelectHighlightSize ("_SelectHighlightSize", Float) = 0
        [HideInInspector] _SelectHighlightDash ("_SelectHighlightDash", Float) = 0
        [HideInInspector] _SelectHighlightSpeed ("_SelectHighlightSpeed", Float) = 0
        [HideInInspector] _GlowDissolveNoiseType ("_GlowDissolveNoiseType", Float) = 0
        [HideInInspector] _GlowDissolveNoiseScale ("_GlowDissolveNoiseScale", Float) = 0
        [HideInInspector] _GlowDissolveAmount ("_GlowDissolveAmount", Float) = 0
        [HideInInspector] _GlowDissolveColor ("_GlowDissolveColor", Vector) = (0,0,0,0)
        [HideInInspector] _GlowDissolveWidth ("_GlowDissolveWidth", Float) = 0
        [HideInInspector] _AnimOutline_En ("_AnimOutline_En", Float) = 0
        [HideInInspector] _DistortOutline_En ("_DistortOutline_En", Float) = 0
        [HideInInspector] _GlowDissolve_En ("_GlowDissolve_En", Float) = 0
        [HideInInspector] _Glow_En ("_Glow_En", Float) = 0
        [HideInInspector] _GradOutline_En ("_GradOutline_En", Float) = 0
        [HideInInspector] _InnerOutline_En ("_InnerOutline_En", Float) = 0
        [HideInInspector] _OutlineOnly_En ("_OutlineOnly_En", Float) = 0
        [HideInInspector] _Outline_En ("_Outline_En", Float) = 0
        [HideInInspector] _DirOutline_En ("_DirOutline_En", Float) = 0
        [HideInInspector] _SelectHighlight_En ("_SelectHighlight_En", Float) = 0
        [HideInInspector] _Shadow_En ("_Shadow_En", Float) = 0
        [HideInInspector] _SoftShadow_En ("_SoftShadow_En", Float) = 0
        [HideInInspector] _TexOutline_En ("_TexOutline_En", Float) = 0
        [HideInInspector] _FakeLighting_En ("_FakeLighting_En", Float) = 0
        [HideInInspector] _FakeLightDir ("_FakeLightDir", Vector) = (0,0,0,0)
        [HideInInspector] _FakeLightColor ("_FakeLightColor", Vector) = (0,0,0,0)
        [HideInInspector] _FakeLightIntensity ("_FakeLightIntensity", Float) = 0
        [HideInInspector] _FakeLightAmbient ("_FakeLightAmbient", Float) = 0
        [HideInInspector] _FakeLightNDist ("_FakeLightNDist", Float) = 0
        [HideInInspector] _FakeLightNZ ("_FakeLightNZ", Float) = 0
        [HideInInspector] _FakeLightSpecPow ("_FakeLightSpecPow", Float) = 0
        [HideInInspector] _FakeLightSpecInt ("_FakeLightSpecInt", Float) = 0
        [HideInInspector] _FakeLightFade ("_FakeLightFade", Float) = 0
        [HideInInspector] _FakeLightMaskCh ("_FakeLightMaskCh", Float) = 0
        [HideInInspector] _PixelateMaskCh ("_PixelateMaskCh", Float) = 0
        [HideInInspector] _WaveMaskCh ("_WaveMaskCh", Float) = 0
        [HideInInspector] _TwistMaskCh ("_TwistMaskCh", Float) = 0
        [HideInInspector] _UVDistortMaskCh ("_UVDistortMaskCh", Float) = 0
        [HideInInspector] _FullDistortMaskCh ("_FullDistortMaskCh", Float) = 0
        [HideInInspector] _DirDistortMaskCh ("_DirDistortMaskCh", Float) = 0
        [HideInInspector] _UVScrollMaskCh ("_UVScrollMaskCh", Float) = 0
        [HideInInspector] _RotateMaskCh ("_RotateMaskCh", Float) = 0
        [HideInInspector] _SqueezeMaskCh ("_SqueezeMaskCh", Float) = 0
        [HideInInspector] _SquishMaskCh ("_SquishMaskCh", Float) = 0
        [HideInInspector] _WiggleMaskCh ("_WiggleMaskCh", Float) = 0
        [HideInInspector] _WindMaskCh ("_WindMaskCh", Float) = 0
        [HideInInspector] _WorldTilingMaskCh ("_WorldTilingMaskCh", Float) = 0
        [HideInInspector] _ShockwaveMaskCh ("_ShockwaveMaskCh", Float) = 0
        [HideInInspector] _SineScaleMaskCh ("_SineScaleMaskCh", Float) = 0
        [HideInInspector] _HeartbeatMaskCh ("_HeartbeatMaskCh", Float) = 0
        [HideInInspector] _ShockwaveSpeed ("_ShockwaveSpeed", Float) = 0
        [HideInInspector] _BreathingMaskCh ("_BreathingMaskCh", Float) = 0
        [HideInInspector] _LiquidWobbleMaskCh ("_LiquidWobbleMaskCh", Float) = 0
        [HideInInspector] _SmoothPixelArtMaskCh ("_SmoothPixelArtMaskCh", Float) = 0
        [HideInInspector] _FBMNoiseMaskCh ("_FBMNoiseMaskCh", Float) = 0
        [HideInInspector] _CurlNoiseMaskCh ("_CurlNoiseMaskCh", Float) = 0
        [HideInInspector] _UVScaleOffsetMaskCh ("_UVScaleOffsetMaskCh", Float) = 0
        [HideInInspector] _DialogueEmphasisMaskCh ("_DialogueEmphasisMaskCh", Float) = 0
        [HideInInspector] _PixelateSize ("_PixelateSize", Float) = 0
        [HideInInspector] _WaveSpeed ("_WaveSpeed", Float) = 0
        [HideInInspector] _WaveAmplitude ("_WaveAmplitude", Float) = 0
        [HideInInspector] _WaveFrequency ("_WaveFrequency", Float) = 0
        [HideInInspector] _TwistAmount ("_TwistAmount", Float) = 0
        [HideInInspector] _TwistSpeed ("_TwistSpeed", Float) = 0
        [HideInInspector] _TwistRadius ("_TwistRadius", Float) = 0
        [HideInInspector] _TwistOffset ("_TwistOffset", Vector) = (0,0,0,0)
        [HideInInspector] _FullDistortAmount ("_FullDistortAmount", Float) = 0
        [HideInInspector] _FullDistortSpeed ("_FullDistortSpeed", Float) = 0
        [HideInInspector] _FullDistortDensity ("_FullDistortDensity", Float) = 0
        [HideInInspector] _DirDistortAngle ("_DirDistortAngle", Float) = 0
        [HideInInspector] _DirDistortAmount ("_DirDistortAmount", Float) = 0
        [HideInInspector] _DirDistortSpeed ("_DirDistortSpeed", Float) = 0
        [HideInInspector] _FBMNoiseAmount ("_FBMNoiseAmount", Float) = 0
        [HideInInspector] _FBMNoiseSpeed ("_FBMNoiseSpeed", Float) = 0
        [HideInInspector] _FBMNoiseDensity ("_FBMNoiseDensity", Float) = 0
        [HideInInspector] _FBMNoiseOctaves ("_FBMNoiseOctaves", Float) = 0
        [HideInInspector] _CurlNoiseAmount ("_CurlNoiseAmount", Float) = 0
        [HideInInspector] _CurlNoiseSpeed ("_CurlNoiseSpeed", Float) = 0
        [HideInInspector] _CurlNoiseDensity ("_CurlNoiseDensity", Float) = 0
        [HideInInspector] _UVScaleOffset ("_UVScaleOffset", Vector) = (0,0,0,0)
        [HideInInspector] _UVScrollSpeedX ("_UVScrollSpeedX", Float) = 0
        [HideInInspector] _UVScrollSpeedY ("_UVScrollSpeedY", Float) = 0
        [HideInInspector] _RotateSpeed ("_RotateSpeed", Float) = 0
        [HideInInspector] _RotateAmount ("_RotateAmount", Float) = 0
        [HideInInspector] _SqueezeAmount ("_SqueezeAmount", Vector) = (0,0,0,0)
        [HideInInspector] _SqueezeSpeed ("_SqueezeSpeed", Float) = 0
        [HideInInspector] _SqueezePower ("_SqueezePower", Float) = 0
        [HideInInspector] _SquishAmount ("_SquishAmount", Vector) = (0,0,0,0)
        [HideInInspector] _SquishSpeed ("_SquishSpeed", Float) = 0
        [HideInInspector] _SquishPower ("_SquishPower", Float) = 0
        [HideInInspector] _WiggleSpeed ("_WiggleSpeed", Float) = 0
        [HideInInspector] _WiggleAmount ("_WiggleAmount", Float) = 0
        [HideInInspector] _WiggleFrequency ("_WiggleFrequency", Float) = 0
        [HideInInspector] _WindSpeed ("_WindSpeed", Float) = 0
        [HideInInspector] _WindAmount ("_WindAmount", Float) = 0
        [HideInInspector] _WindFrequency ("_WindFrequency", Float) = 0
        [HideInInspector] _WindBottomY ("_WindBottomY", Float) = 0
        [HideInInspector] _ShockwaveCenter ("_ShockwaveCenter", Vector) = (0,0,0,0)
        [HideInInspector] _ShockwaveRadius ("_ShockwaveRadius", Float) = 0
        [HideInInspector] _ShockwaveWidth ("_ShockwaveWidth", Float) = 0
        [HideInInspector] _ShockwaveAmplitude ("_ShockwaveAmplitude", Float) = 0
        [HideInInspector] _SineScaleAmountX ("_SineScaleAmountX", Float) = 0
        [HideInInspector] _SineScaleAmountY ("_SineScaleAmountY", Float) = 0
        [HideInInspector] _SineScaleSpeed ("_SineScaleSpeed", Float) = 0
        [HideInInspector] _HeartbeatSpeed ("_HeartbeatSpeed", Float) = 0
        [HideInInspector] _HeartbeatAmount ("_HeartbeatAmount", Float) = 0
        [HideInInspector] _BreathingSpeed ("_BreathingSpeed", Float) = 0
        [HideInInspector] _BreathingAmount ("_BreathingAmount", Float) = 0
        [HideInInspector] _LiquidWobbleSpeed ("_LiquidWobbleSpeed", Float) = 0
        [HideInInspector] _LiquidWobbleAmount ("_LiquidWobbleAmount", Float) = 0
        [HideInInspector] _LiquidWobbleFreq ("_LiquidWobbleFreq", Float) = 0
        [HideInInspector] _SmoothPixelArtTexelSize ("_SmoothPixelArtTexelSize", Float) = 0
        [HideInInspector] _UVScrollOverlayBlend ("_UVScrollOverlayBlend", Float) = 0
        [HideInInspector] _UVScrollOverlaySpeedX ("_UVScrollOverlaySpeedX", Float) = 0
        [HideInInspector] _UVScrollOverlaySpeedY ("_UVScrollOverlaySpeedY", Float) = 0
        [HideInInspector] _UVScrollOverlayMaskCh ("_UVScrollOverlayMaskCh", Float) = 0
        [HideInInspector] _UVScrollOverlayTint ("_UVScrollOverlayTint", Vector) = (0,0,0,0)
        [HideInInspector] _UVScrollOverlayTiling ("_UVScrollOverlayTiling", Vector) = (0,0,0,0)
        [HideInInspector] _Breathing_En ("_Breathing_En", Float) = 0
        [HideInInspector] _CurlNoise_En ("_CurlNoise_En", Float) = 0
        [HideInInspector] _DialogueEmphasis_En ("_DialogueEmphasis_En", Float) = 0
        [HideInInspector] _DirDistort_En ("_DirDistort_En", Float) = 0
        [HideInInspector] _FBMNoise_En ("_FBMNoise_En", Float) = 0
        [HideInInspector] _FullDistort_En ("_FullDistort_En", Float) = 0
        [HideInInspector] _Heartbeat_En ("_Heartbeat_En", Float) = 0
        [HideInInspector] _LiquidWobble_En ("_LiquidWobble_En", Float) = 0
        [HideInInspector] _Pixelate_En ("_Pixelate_En", Float) = 0
        [HideInInspector] _Rotate_En ("_Rotate_En", Float) = 0
        [HideInInspector] _Shockwave_En ("_Shockwave_En", Float) = 0
        [HideInInspector] _SineScale_En ("_SineScale_En", Float) = 0
        [HideInInspector] _SmoothPixelArt_En ("_SmoothPixelArt_En", Float) = 0
        [HideInInspector] _Squeeze_En ("_Squeeze_En", Float) = 0
        [HideInInspector] _Squish_En ("_Squish_En", Float) = 0
        [HideInInspector] _Twist_En ("_Twist_En", Float) = 0
        [HideInInspector] _UVDistort_En ("_UVDistort_En", Float) = 0
        [HideInInspector] _UVScaleOffset_En ("_UVScaleOffset_En", Float) = 0
        [HideInInspector] _UVScrollOverlay_En ("_UVScrollOverlay_En", Float) = 0
        [HideInInspector] _UVScroll_En ("_UVScroll_En", Float) = 0
        [HideInInspector] _Wave_En ("_Wave_En", Float) = 0
        [HideInInspector] _Wiggle_En ("_Wiggle_En", Float) = 0
        [HideInInspector] _Wind_En ("_Wind_En", Float) = 0
        [HideInInspector] _WorldTiling_En ("_WorldTiling_En", Float) = 0
        [HideInInspector] _UVBendPoint_En ("_UVBendPoint_En", Float) = 0
        [HideInInspector] _BendLocalPoint ("_BendLocalPoint", Vector) = (0,0,0,0)
        [HideInInspector] _BendWorldPoint ("_BendWorldPoint", Vector) = (0,0,0,0)
        [HideInInspector] _BendUseWorldSpace ("_BendUseWorldSpace", Float) = 0
        [HideInInspector] _BendWorldScale ("_BendWorldScale", Float) = 0
        [HideInInspector] _BendRadius ("_BendRadius", Float) = 0
        [HideInInspector] _BendAmount ("_BendAmount", Float) = 0
        [HideInInspector] _BendFalloff ("_BendFalloff", Float) = 0
        [HideInInspector] _BendInvert ("_BendInvert", Float) = 0
        [HideInInspector] _BendMaskCh ("_BendMaskCh", Float) = 0
        [HideInInspector] _SharpenMaskCh ("_SharpenMaskCh", Float) = 0
        [HideInInspector] _GaussBlurMaskCh ("_GaussBlurMaskCh", Float) = 0
        [HideInInspector] _DirBlurMaskCh ("_DirBlurMaskCh", Float) = 0
        [HideInInspector] _FlipBookBlendMaskCh ("_FlipBookBlendMaskCh", Float) = 0
        [HideInInspector] _SharpenAmount ("_SharpenAmount", Float) = 0
        [HideInInspector] _SharpenSize ("_SharpenSize", Float) = 0
        [HideInInspector] _GaussBlurSize ("_GaussBlurSize", Float) = 0
        [HideInInspector] _GaussBlurSamples ("_GaussBlurSamples", Float) = 0
        [HideInInspector] _DirBlurAngle ("_DirBlurAngle", Float) = 0
        [HideInInspector] _DirBlurSize ("_DirBlurSize", Float) = 0
        [HideInInspector] _DirBlurSamples ("_DirBlurSamples", Float) = 0
        [HideInInspector] _DirBlur_En ("_DirBlur_En", Float) = 0
        [HideInInspector] _FlipBookBlend_En ("_FlipBookBlend_En", Float) = 0
        [HideInInspector] _GaussBlur_En ("_GaussBlur_En", Float) = 0
        [HideInInspector] _Sharpen_En ("_Sharpen_En", Float) = 0
        [HideInInspector] _DissolveMaskCh ("_DissolveMaskCh", Float) = 0
        [HideInInspector] _SrcDissolveMaskCh ("_SrcDissolveMaskCh", Float) = 0
        [HideInInspector] _DirFadeMaskCh ("_DirFadeMaskCh", Float) = 0
        [HideInInspector] _DirGlowFadeMaskCh ("_DirGlowFadeMaskCh", Float) = 0
        [HideInInspector] _FullAlphaDissolveMaskCh ("_FullAlphaDissolveMaskCh", Float) = 0
        [HideInInspector] _HalftoneMaskCh ("_HalftoneMaskCh", Float) = 0
        [HideInInspector] _RadialWipeMaskCh ("_RadialWipeMaskCh", Float) = 0
        [HideInInspector] _BlindsMaskCh ("_BlindsMaskCh", Float) = 0
        [HideInInspector] _DiamondDissolveMaskCh ("_DiamondDissolveMaskCh", Float) = 0
        [HideInInspector] _ScanlineDissolveMaskCh ("_ScanlineDissolveMaskCh", Float) = 0
        [HideInInspector] _SpreadFadeMaskCh ("_SpreadFadeMaskCh", Float) = 0
        [HideInInspector] _PixelDissolveMaskCh ("_PixelDissolveMaskCh", Float) = 0
        [HideInInspector] _SpiralWipeMaskCh ("_SpiralWipeMaskCh", Float) = 0
        [HideInInspector] _HexDissolveMaskCh ("_HexDissolveMaskCh", Float) = 0
        [HideInInspector] _DissolveNoiseType ("_DissolveNoiseType", Float) = 0
        [HideInInspector] _DissolveNoiseScale ("_DissolveNoiseScale", Float) = 0
        [HideInInspector] _DissolveAmount ("_DissolveAmount", Float) = 0
        [HideInInspector] _DissolveBorderWidth ("_DissolveBorderWidth", Float) = 0
        [HideInInspector] _DissolveBorderColor ("_DissolveBorderColor", Vector) = (0,0,0,0)
        [HideInInspector] _DissolveBurnWidth ("_DissolveBurnWidth", Float) = 0
        [HideInInspector] _DissolveBurnColor ("_DissolveBurnColor", Vector) = (0,0,0,0)
        [HideInInspector] _DissolveTiling ("_DissolveTiling", Vector) = (0,0,0,0)
        [HideInInspector] _SrcDissolveAmount ("_SrcDissolveAmount", Float) = 0
        [HideInInspector] _SrcDissolvePosition ("_SrcDissolvePosition", Vector) = (0,0,0,0)
        [HideInInspector] _SrcDissolveBorderColor ("_SrcDissolveBorderColor", Vector) = (0,0,0,0)
        [HideInInspector] _SrcDissolveBorderWidth ("_SrcDissolveBorderWidth", Float) = 0
        [HideInInspector] _DirFadeAngle ("_DirFadeAngle", Float) = 0
        [HideInInspector] _DirFadeAmount ("_DirFadeAmount", Float) = 0
        [HideInInspector] _DirFadeSmoothness ("_DirFadeSmoothness", Float) = 0
        [HideInInspector] _DirGlowFadeAngle ("_DirGlowFadeAngle", Float) = 0
        [HideInInspector] _DirGlowFadeAmount ("_DirGlowFadeAmount", Float) = 0
        [HideInInspector] _DirGlowFadeColor ("_DirGlowFadeColor", Vector) = (0,0,0,0)
        [HideInInspector] _DirGlowFadeWidth ("_DirGlowFadeWidth", Float) = 0
        [HideInInspector] _FullAlphaDissolveAmount ("_FullAlphaDissolveAmount", Float) = 0
        [HideInInspector] _HalftoneSize ("_HalftoneSize", Float) = 0
        [HideInInspector] _HalftoneAmount ("_HalftoneAmount", Float) = 0
        [HideInInspector] _HalftoneSmoothness ("_HalftoneSmoothness", Float) = 0
        [HideInInspector] _RadialWipeAmount ("_RadialWipeAmount", Float) = 0
        [HideInInspector] _RadialWipeSmoothness ("_RadialWipeSmoothness", Float) = 0
        [HideInInspector] _RadialWipeAngle ("_RadialWipeAngle", Float) = 0
        [HideInInspector] _BlindsAmount ("_BlindsAmount", Float) = 0
        [HideInInspector] _BlindsCount ("_BlindsCount", Float) = 0
        [HideInInspector] _BlindsDirection ("_BlindsDirection", Float) = 0
        [HideInInspector] _DiamondDissolveAmount ("_DiamondDissolveAmount", Float) = 0
        [HideInInspector] _DiamondDissolveScale ("_DiamondDissolveScale", Float) = 0
        [HideInInspector] _ScanlineDissolveAmount ("_ScanlineDissolveAmount", Float) = 0
        [HideInInspector] _ScanlineDissolveDensity ("_ScanlineDissolveDensity", Float) = 0
        [HideInInspector] _SpreadFadeAmount ("_SpreadFadeAmount", Float) = 0
        [HideInInspector] _SpreadFadeOrigin ("_SpreadFadeOrigin", Vector) = (0,0,0,0)
        [HideInInspector] _SpreadFadeSmoothness ("_SpreadFadeSmoothness", Float) = 0
        [HideInInspector] _PixelDissolveAmount ("_PixelDissolveAmount", Float) = 0
        [HideInInspector] _PixelDissolveGrid ("_PixelDissolveGrid", Float) = 0
        [HideInInspector] _SpiralWipeAmount ("_SpiralWipeAmount", Float) = 0
        [HideInInspector] _SpiralWipeSmoothness ("_SpiralWipeSmoothness", Float) = 0
        [HideInInspector] _SpiralWipeArms ("_SpiralWipeArms", Float) = 0
        [HideInInspector] _HexDissolveAmount ("_HexDissolveAmount", Float) = 0
        [HideInInspector] _HexDissolveScale ("_HexDissolveScale", Float) = 0
        [HideInInspector] _DissolveNoiseSpeed ("_DissolveNoiseSpeed", Float) = 0
        [HideInInspector] _Blinds_En ("_Blinds_En", Float) = 0
        [HideInInspector] _DiamondDissolve_En ("_DiamondDissolve_En", Float) = 0
        [HideInInspector] _DirFade_En ("_DirFade_En", Float) = 0
        [HideInInspector] _DirGlowFade_En ("_DirGlowFade_En", Float) = 0
        [HideInInspector] _Dissolve_En ("_Dissolve_En", Float) = 0
        [HideInInspector] _FullAlphaDissolve_En ("_FullAlphaDissolve_En", Float) = 0
        [HideInInspector] _Halftone_En ("_Halftone_En", Float) = 0
        [HideInInspector] _HexDissolve_En ("_HexDissolve_En", Float) = 0
        [HideInInspector] _PixelDissolve_En ("_PixelDissolve_En", Float) = 0
        [HideInInspector] _RadialWipe_En ("_RadialWipe_En", Float) = 0
        [HideInInspector] _ScanlineDissolve_En ("_ScanlineDissolve_En", Float) = 0
        [HideInInspector] _SpiralWipe_En ("_SpiralWipe_En", Float) = 0
        [HideInInspector] _SpreadFade_En ("_SpreadFade_En", Float) = 0
        [HideInInspector] _SrcDissolve_En ("_SrcDissolve_En", Float) = 0
        [HideInInspector] _BurnDirection_En ("_BurnDirection_En", Float) = 0
        [HideInInspector] _BurnDirAngle ("_BurnDirAngle", Float) = 0
        [HideInInspector] _BurnDirAmount ("_BurnDirAmount", Float) = 0
        [HideInInspector] _BurnDirEdgeWidth ("_BurnDirEdgeWidth", Float) = 0
        [HideInInspector] _BurnDirFireColor ("_BurnDirFireColor", Vector) = (0,0,0,0)
        [HideInInspector] _BurnDirCharcoalColor ("_BurnDirCharcoalColor", Vector) = (0,0,0,0)
        [HideInInspector] _BurnDirCharcoalWidth ("_BurnDirCharcoalWidth", Float) = 0
        [HideInInspector] _BurnDirNoiseScale ("_BurnDirNoiseScale", Float) = 0
        [HideInInspector] _BurnDirMaskCh ("_BurnDirMaskCh", Float) = 0
        [HideInInspector] _HologramMaskCh ("_HologramMaskCh", Float) = 0
        [HideInInspector] _GlitchMaskCh ("_GlitchMaskCh", Float) = 0
        [HideInInspector] _FlickerMaskCh ("_FlickerMaskCh", Float) = 0
        [HideInInspector] _ShineMaskCh ("_ShineMaskCh", Float) = 0
        [HideInInspector] _SmokeMaskCh ("_SmokeMaskCh", Float) = 0
        [HideInInspector] _FlameMaskCh ("_FlameMaskCh", Float) = 0
        [HideInInspector] _ShiftingMaskCh ("_ShiftingMaskCh", Float) = 0
        [HideInInspector] _ShiftRainbowMaskCh ("_ShiftRainbowMaskCh", Float) = 0
        [HideInInspector] _OverlayMaskCh ("_OverlayMaskCh", Float) = 0
        [HideInInspector] _HologramStripesAmount ("_HologramStripesAmount", Float) = 0
        [HideInInspector] _HologramMinAlpha ("_HologramMinAlpha", Float) = 0
        [HideInInspector] _HologramStripesSpeed ("_HologramStripesSpeed", Float) = 0
        [HideInInspector] _HologramColor ("_HologramColor", Vector) = (0,0,0,0)
        [HideInInspector] _HologramBlend ("_HologramBlend", Float) = 0
        [HideInInspector] _HologramDistortion ("_HologramDistortion", Float) = 0
        [HideInInspector] _GlitchAmount ("_GlitchAmount", Float) = 0
        [HideInInspector] _GlitchSpeed ("_GlitchSpeed", Float) = 0
        [HideInInspector] _GlitchSize ("_GlitchSize", Float) = 0
        [HideInInspector] _GlitchColorShift ("_GlitchColorShift", Float) = 0
        [HideInInspector] _FlickerFreq ("_FlickerFreq", Float) = 0
        [HideInInspector] _FlickerPercent ("_FlickerPercent", Float) = 0
        [HideInInspector] _ShineColor ("_ShineColor", Vector) = (0,0,0,0)
        [HideInInspector] _ShineWidth ("_ShineWidth", Float) = 0
        [HideInInspector] _ShineSpeed ("_ShineSpeed", Float) = 0
        [HideInInspector] _ShineDelay ("_ShineDelay", Float) = 0
        [HideInInspector] _ShineRotation ("_ShineRotation", Float) = 0
        [HideInInspector] _ShineGlow ("_ShineGlow", Float) = 0
        [HideInInspector] _SmokeColor ("_SmokeColor", Vector) = (0,0,0,0)
        [HideInInspector] _SmokeDensity ("_SmokeDensity", Float) = 0
        [HideInInspector] _SmokeSpeed ("_SmokeSpeed", Float) = 0
        [HideInInspector] _SmokeFade ("_SmokeFade", Float) = 0
        [HideInInspector] _SmokeTurbulence ("_SmokeTurbulence", Float) = 0
        [HideInInspector] _SmokeColorStr ("_SmokeColorStr", Float) = 0
        [HideInInspector] _SmokeDirection ("_SmokeDirection", Vector) = (0,0,0,0)
        [HideInInspector] _FlameColor1 ("_FlameColor1", Vector) = (0,0,0,0)
        [HideInInspector] _FlameColor2 ("_FlameColor2", Vector) = (0,0,0,0)
        [HideInInspector] _FlameSpeed ("_FlameSpeed", Float) = 0
        [HideInInspector] _FlameDensity ("_FlameDensity", Float) = 0
        [HideInInspector] _FlameIntensity ("_FlameIntensity", Float) = 0
        [HideInInspector] _FlameOnlyMode ("_FlameOnlyMode", Float) = 0
        [HideInInspector] _FlameExtendSides ("_FlameExtendSides", Float) = 0
        [HideInInspector] _FlameSideFalloff ("_FlameSideFalloff", Float) = 0
        [HideInInspector] _ShiftingColor1 ("_ShiftingColor1", Vector) = (0,0,0,0)
        [HideInInspector] _ShiftingColor2 ("_ShiftingColor2", Vector) = (0,0,0,0)
        [HideInInspector] _ShiftingSpeed ("_ShiftingSpeed", Float) = 0
        [HideInInspector] _ShiftingFade ("_ShiftingFade", Float) = 0
        [HideInInspector] _ShiftRainbowSpeed ("_ShiftRainbowSpeed", Float) = 0
        [HideInInspector] _ShiftRainbowDensity ("_ShiftRainbowDensity", Float) = 0
        [HideInInspector] _ShiftRainbowFade ("_ShiftRainbowFade", Float) = 0
        [HideInInspector] _OverlayColor ("_OverlayColor", Vector) = (0,0,0,0)
        [HideInInspector] _OverlayBlend ("_OverlayBlend", Float) = 0
        [HideInInspector] _OverlaySpeed ("_OverlaySpeed", Vector) = (0,0,0,0)
        [HideInInspector] _OverlayTiling ("_OverlayTiling", Vector) = (0,0,0,0)
        [HideInInspector] _Flame_En ("_Flame_En", Float) = 0
        [HideInInspector] _Flicker_En ("_Flicker_En", Float) = 0
        [HideInInspector] _Glitch_En ("_Glitch_En", Float) = 0
        [HideInInspector] _Hologram_En ("_Hologram_En", Float) = 0
        [HideInInspector] _MotionSmear_En ("_MotionSmear_En", Float) = 0
        [HideInInspector] _Overlay_En ("_Overlay_En", Float) = 0
        [HideInInspector] _Shifting_En ("_Shifting_En", Float) = 0
        [HideInInspector] _ShiftRainbow_En ("_ShiftRainbow_En", Float) = 0
        [HideInInspector] _Shine_En ("_Shine_En", Float) = 0
        [HideInInspector] _Smoke_En ("_Smoke_En", Float) = 0
        [HideInInspector] _VoronoiCrackle_En ("_VoronoiCrackle_En", Float) = 0
        [HideInInspector] _EnchantedColorVariation ("_EnchantedColorVariation", Float) = 0
        [HideInInspector] _EnchantedSizeMix ("_EnchantedSizeMix", Float) = 0
        [HideInInspector] _EnchantedShimmer ("_EnchantedShimmer", Float) = 0
        [HideInInspector] _GlobalSmoke_En ("_GlobalSmoke_En", Float) = 0
        [HideInInspector] _GlobalSmokeColor ("_GlobalSmokeColor", Vector) = (0,0,0,0)
        [HideInInspector] _GlobalSmokeDensity ("_GlobalSmokeDensity", Float) = 0
        [HideInInspector] _GlobalSmokeSpeed ("_GlobalSmokeSpeed", Float) = 0
        [HideInInspector] _GlobalSmokeOpacity ("_GlobalSmokeOpacity", Float) = 0
        [HideInInspector] _GlobalSmokeDir ("_GlobalSmokeDir", Vector) = (0,0,0,0)
        [HideInInspector] _GlobalSmokeMaskCh ("_GlobalSmokeMaskCh", Float) = 0
        [HideInInspector] _GlobalSmokeOverlayOnly ("_GlobalSmokeOverlayOnly", Float) = 0
        [HideInInspector] _HoloCard_En ("_HoloCard_En", Float) = 0
        [HideInInspector] _HoloCardDensity ("_HoloCardDensity", Float) = 0
        [HideInInspector] _HoloCardSpeed ("_HoloCardSpeed", Float) = 0
        [HideInInspector] _HoloCardSat ("_HoloCardSat", Float) = 0
        [HideInInspector] _HoloCardBright ("_HoloCardBright", Float) = 0
        [HideInInspector] _HoloCardFPow ("_HoloCardFPow", Float) = 0
        [HideInInspector] _HoloCardFShift ("_HoloCardFShift", Float) = 0
        [HideInInspector] _HoloCardScanDen ("_HoloCardScanDen", Float) = 0
        [HideInInspector] _HoloCardScanSpd ("_HoloCardScanSpd", Float) = 0
        [HideInInspector] _HoloCardScanInt ("_HoloCardScanInt", Float) = 0
        [HideInInspector] _HoloCardFade ("_HoloCardFade", Float) = 0
        [HideInInspector] _HoloCardMaskCh ("_HoloCardMaskCh", Float) = 0
        [HideInInspector] _CloudShadows_En ("_CloudShadows_En", Float) = 0
        [HideInInspector] _CloudShadowScale ("_CloudShadowScale", Float) = 0
        [HideInInspector] _CloudShadowSpeed ("_CloudShadowSpeed", Vector) = (0,0,0,0)
        [HideInInspector] _CloudShadowDark ("_CloudShadowDark", Float) = 0
        [HideInInspector] _CloudShadowThresh ("_CloudShadowThresh", Float) = 0
        [HideInInspector] _CloudShadowSoft ("_CloudShadowSoft", Float) = 0
        [HideInInspector] _CloudShadowFade ("_CloudShadowFade", Float) = 0
        [HideInInspector] _CloudShadowMaskCh ("_CloudShadowMaskCh", Float) = 0
        [HideInInspector] _CloudShadowOverlayOnly ("_CloudShadowOverlayOnly", Float) = 0
        [HideInInspector] _TrailA_En ("_TrailA_En", Float) = 0
        [HideInInspector] _TrailAVel ("_TrailAVel", Vector) = (0,0,0,0)
        [HideInInspector] _TrailASlowCol ("_TrailASlowCol", Vector) = (0,0,0,0)
        [HideInInspector] _TrailAFastCol ("_TrailAFastCol", Vector) = (0,0,0,0)
        [HideInInspector] _TrailAMaxSpd ("_TrailAMaxSpd", Float) = 0
        [HideInInspector] _TrailACurve ("_TrailACurve", Float) = 0
        [HideInInspector] _TrailAInt ("_TrailAInt", Float) = 0
        [HideInInspector] _TrailAMaskCh ("_TrailAMaskCh", Float) = 0
        [HideInInspector] _TrailB_En ("_TrailB_En", Float) = 0
        [HideInInspector] _TrailBVel ("_TrailBVel", Vector) = (0,0,0,0)
        [HideInInspector] _TrailBCol ("_TrailBCol", Vector) = (0,0,0,0)
        [HideInInspector] _TrailBMaxSpd ("_TrailBMaxSpd", Float) = 0
        [HideInInspector] _TrailBCurve ("_TrailBCurve", Float) = 0
        [HideInInspector] _TrailBInt ("_TrailBInt", Float) = 0
        [HideInInspector] _TrailBSegs ("_TrailBSegs", Float) = 0
        [HideInInspector] _TrailBFade ("_TrailBFade", Float) = 0
        [HideInInspector] _TrailBMaskCh ("_TrailBMaskCh", Float) = 0
        [HideInInspector] _TrailC_En ("_TrailC_En", Float) = 0
        [HideInInspector] _TrailCVel ("_TrailCVel", Vector) = (0,0,0,0)
        [HideInInspector] _TrailCColA ("_TrailCColA", Vector) = (0,0,0,0)
        [HideInInspector] _TrailCColB ("_TrailCColB", Vector) = (0,0,0,0)
        [HideInInspector] _TrailCMaxSpd ("_TrailCMaxSpd", Float) = 0
        [HideInInspector] _TrailCCurve ("_TrailCCurve", Float) = 0
        [HideInInspector] _TrailCInt ("_TrailCInt", Float) = 0
        [HideInInspector] _TrailCRbSpd ("_TrailCRbSpd", Float) = 0
        [HideInInspector] _TrailCMaskCh ("_TrailCMaskCh", Float) = 0
        [HideInInspector] _Charging_En ("_Charging_En", Float) = 0
        [HideInInspector] _ChargeColor ("_ChargeColor", Vector) = (0,0,0,0)
        [HideInInspector] _ChargeCenter ("_ChargeCenter", Vector) = (0,0,0,0)
        [HideInInspector] _ChargeRadius ("_ChargeRadius", Float) = 0
        [HideInInspector] _ChargeWidth ("_ChargeWidth", Float) = 0
        [HideInInspector] _ChargeSpeed ("_ChargeSpeed", Float) = 0
        [HideInInspector] _ChargeFreq ("_ChargeFreq", Float) = 0
        [HideInInspector] _ChargeDensity ("_ChargeDensity", Float) = 0
        [HideInInspector] _ChargeInt ("_ChargeInt", Float) = 0
        [HideInInspector] _ChargeLines ("_ChargeLines", Float) = 0
        [HideInInspector] _ChargeFade ("_ChargeFade", Float) = 0
        [HideInInspector] _ChargeMaskCh ("_ChargeMaskCh", Float) = 0
        [HideInInspector] _CheckerboardMaskCh ("_CheckerboardMaskCh", Float) = 0
        [HideInInspector] _UVVisualizeMaskCh ("_UVVisualizeMaskCh", Float) = 0
        [HideInInspector] _UVCheckerMaskCh ("_UVCheckerMaskCh", Float) = 0
        [HideInInspector] _UVNumericMaskCh ("_UVNumericMaskCh", Float) = 0
        [HideInInspector] _HealthTintMaskCh ("_HealthTintMaskCh", Float) = 0
        [HideInInspector] _XRaySilhouetteMaskCh ("_XRaySilhouetteMaskCh", Float) = 0
        [HideInInspector] _MinimapPingMaskCh ("_MinimapPingMaskCh", Float) = 0
        [HideInInspector] _GhostingMaskCh ("_GhostingMaskCh", Float) = 0
        [HideInInspector] _MinimapPingOffset ("_MinimapPingOffset", Vector) = (0,0,0,0)
        [HideInInspector] _UVVisualizeBlend ("_UVVisualizeBlend", Float) = 0
        [HideInInspector] _UVCheckerScale ("_UVCheckerScale", Float) = 0
        [HideInInspector] _UVCheckerBlend ("_UVCheckerBlend", Float) = 0
        [HideInInspector] _UVNumericScale ("_UVNumericScale", Float) = 0
        [HideInInspector] _UVNumericBlend ("_UVNumericBlend", Float) = 0
        [HideInInspector] _HealthTintLowColor ("_HealthTintLowColor", Vector) = (0,0,0,0)
        [HideInInspector] _HealthTintHighColor ("_HealthTintHighColor", Vector) = (0,0,0,0)
        [HideInInspector] _HealthTintAmount ("_HealthTintAmount", Float) = 0
        [HideInInspector] _XRaySilhouetteColor ("_XRaySilhouetteColor", Vector) = (0,0,0,0)
        [HideInInspector] _XRaySilhouetteBlend ("_XRaySilhouetteBlend", Float) = 0
        [HideInInspector] _MinimapPingColor ("_MinimapPingColor", Vector) = (0,0,0,0)
        [HideInInspector] _MinimapPingSpeed ("_MinimapPingSpeed", Float) = 0
        [HideInInspector] _MinimapPingWidth ("_MinimapPingWidth", Float) = 0
        [HideInInspector] _GhostingCount ("_GhostingCount", Float) = 0
        [HideInInspector] _GhostingOffset ("_GhostingOffset", Vector) = (0,0,0,0)
        [HideInInspector] _GhostingFade ("_GhostingFade", Float) = 0
        [HideInInspector] _RetroPaletteSize ("_RetroPaletteSize", Float) = 0
        [HideInInspector] _Checkerboard_En ("_Checkerboard_En", Float) = 0
        [HideInInspector] _Ghosting_En ("_Ghosting_En", Float) = 0
        [HideInInspector] _HealthTint_En ("_HealthTint_En", Float) = 0
        [HideInInspector] _MinimapPing_En ("_MinimapPing_En", Float) = 0
        [HideInInspector] _RetroPalette_En ("_RetroPalette_En", Float) = 0
        [HideInInspector] _UVChecker_En ("_UVChecker_En", Float) = 0
        [HideInInspector] _UVNumeric_En ("_UVNumeric_En", Float) = 0
        [HideInInspector] _UVVisualize_En ("_UVVisualize_En", Float) = 0
        [HideInInspector] _XRaySilhouette_En ("_XRaySilhouette_En", Float) = 0
        [HideInInspector] _Blueprint_En ("_Blueprint_En", Float) = 0
        [HideInInspector] _BlueprintBgCol ("_BlueprintBgCol", Vector) = (0,0,0,0)
        [HideInInspector] _BlueprintLineCol ("_BlueprintLineCol", Vector) = (0,0,0,0)
        [HideInInspector] _BlueprintGridCol ("_BlueprintGridCol", Vector) = (0,0,0,0)
        [HideInInspector] _BlueprintEdgeSz ("_BlueprintEdgeSz", Float) = 0
        [HideInInspector] _BlueprintLineWidth ("_BlueprintLineWidth", Float) = 0
        [HideInInspector] _BlueprintLineInt ("_BlueprintLineInt", Float) = 0
        [HideInInspector] _BlueprintGridDen ("_BlueprintGridDen", Float) = 0
        [HideInInspector] _BlueprintGridOp ("_BlueprintGridOp", Float) = 0
        [HideInInspector] _BlueprintFade ("_BlueprintFade", Float) = 0
        [HideInInspector] _BlueprintMaskCh ("_BlueprintMaskCh", Float) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "RenderPipeline"="UniversalPipeline" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Pass
        {
            Tags { "LightMode" = "UniversalForward" }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT _SHADOWS_SOFT_LOW _SHADOWS_SOFT_MEDIUM _SHADOWS_SOFT_HIGH
            #if UNITY_VERSION >= 600010
            #pragma multi_compile _ _CLUSTER_LIGHT_LOOP
            #else
            #pragma multi_compile_fragment _ _FORWARD_PLUS
            #endif
            #pragma multi_compile_fragment _ _LIGHT_LAYERS
            #pragma multi_compile_fragment _ _LIGHT_COOKIES
            #pragma multi_compile _ PROBE_VOLUMES_L1 PROBE_VOLUMES_L2

            #define AIO_URP 1
            #ifndef AIO_URP_MAJOR
            #define AIO_URP_MAJOR 17
            #endif
            #ifndef AIO_URP_12_PLUS
            #define AIO_URP_12_PLUS 1
            #endif
            #ifndef AIO_URP_14_PLUS
            #define AIO_URP_14_PLUS 1
            #endif
            #ifndef AIO_URP_17_PLUS
            #define AIO_URP_17_PLUS 1
            #endif
            #define AIO_COMPACT_PROPERTIES 1

            #include_with_pragmas "Include/AllInOneKeywords.hlsl"

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #if UNITY_2023_3_OR_NEWER || UNITY_6000_0_OR_NEWER
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/GlobalIllumination.hlsl"
            #endif
            #define AIO_URP_3D_LIT 1
            #define AIO_LIT 1

            #include "Include/AllInOneEffects.hlsl"
            ENDHLSL
        }

        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            ZWrite On ZTest LEqual Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment fragShadow
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_instancing

            #define AIO_URP 1
            #ifndef AIO_URP_MAJOR
            #define AIO_URP_MAJOR 17
            #endif
            #ifndef AIO_URP_12_PLUS
            #define AIO_URP_12_PLUS 1
            #endif
            #ifndef AIO_URP_14_PLUS
            #define AIO_URP_14_PLUS 1
            #endif
            #ifndef AIO_URP_17_PLUS
            #define AIO_URP_17_PLUS 1
            #endif
            #define AIO_COMPACT_PROPERTIES 1
            #include_with_pragmas "Include/AllInOneKeywords.hlsl"

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #if UNITY_2023_3_OR_NEWER || UNITY_6000_0_OR_NEWER
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/GlobalIllumination.hlsl"
            #endif
            #define AIO_URP_3D_LIT 1
            #define AIO_LIT 1

            #include "Include/AllInOneEffects.hlsl"
            ENDHLSL
        }
    }
    Fallback "Sprites/Default"
}
