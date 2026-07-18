#ifndef ALLINONE_PROPERTIES_INCLUDED
#define ALLINONE_PROPERTIES_INCLUDED

// Pipeline detection: .shader files define AIO_URP, AIO_HDRP, or AIO_BUILTIN
#if defined(AIO_URP) || defined(AIO_HDRP)
#define AIO_SRP 1
#endif

#ifndef SAMPLE_TEXTURE2D
// Use native HLSL sampling to prevent Unity from double-prefixing "sampler"
#define SAMPLE_TEXTURE2D(tex, samp, uv) tex.Sample(samp, uv)
#endif

#ifndef TEXTURE2D
// Declares the texture without a built-in sampler
#define TEXTURE2D(tex) UNITY_DECLARE_TEX2D_NOSAMPLER(tex)
// Declares the standalone sampler state
#define SAMPLER(samp) SamplerState samp
#endif

// Compact material layout for lit variants.
// Used by variants that define AIO_COMPACT_PROPERTIES.
// Lit shaders already carry Unity lighting/shadow buffers. Keeping all FlexiSprite
// material fields in one cbuffer leaves more room for those pipeline buffers while
// leaving the normal grouped layout available to unlit variants.
#ifndef CBUFFER_START
#define CBUFFER_START(name)
#define CBUFFER_END
#endif

#ifndef UNITY_VERTEX_INPUT_INSTANCE_ID
#define UNITY_VERTEX_INPUT_INSTANCE_ID
#endif
#ifndef UNITY_VERTEX_OUTPUT_STEREO
#define UNITY_VERTEX_OUTPUT_STEREO
#endif
#ifndef UNITY_SETUP_INSTANCE_ID
#define UNITY_SETUP_INSTANCE_ID(v)
#endif
#ifndef UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO
#define UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o)
#endif

// ==================== TEXTURES ====================
// Shared samplers reduce sampler count from 20+ to 9 (mobile DX11/OpenGL safe)
// On Built-In RP, SAMPLER() is a no-op and sampler2D embeds its own — no effect.
// On SRP, TEXTURE2D declares texture-only; SAMPLER declares sampler-only.

// ---- Unique samplers (each texture needs its own filtering/wrap) ----
TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
TEXTURE2D(_GlobalEffectMask); SAMPLER(sampler_GlobalEffectMask);
TEXTURE2D(_EffectMasks); SAMPLER(sampler_EffectMasks);
TEXTURE2D(_OutlineSDFTex); SAMPLER(sampler_OutlineSDFTex);
TEXTURE2D(_FlipBookNextFrame); SAMPLER(sampler_FlipBookNextFrame);
TEXTURE2D(_TexOutlineTex); SAMPLER(sampler_TexOutlineTex);

// ---- Shared clamp sampler: all 1D gradients, ramps, palettes, LUTs ----
// These are all 256x1 or small, Clamp wrap mode, Bilinear filter
SAMPLER(sampler_linear_clamp);
TEXTURE2D(_ColorRampTex);
TEXTURE2D(_GradientTex);
TEXTURE2D(_RadialGradTex);
TEXTURE2D(_RetroPaletteTex);
TEXTURE2D(_PaletteSwapTex);
TEXTURE2D(_LUTReplaceTex);
// Alias per-texture sampler names to the shared sampler (STEX macro builds sampler_##texName)
#define sampler_ColorRampTex sampler_linear_clamp
#define sampler_GradientTex sampler_linear_clamp
#define sampler_RadialGradTex sampler_linear_clamp
#define sampler_RetroPaletteTex sampler_linear_clamp
#define sampler_PaletteSwapTex sampler_linear_clamp
#define sampler_LUTReplaceTex sampler_linear_clamp

// ---- Shared repeat sampler: all noise/dissolve/overlay textures ----
// These all tile (Repeat wrap mode, Bilinear filter)
SAMPLER(sampler_linear_repeat);
TEXTURE2D(_DistortTex);
TEXTURE2D(_DissolveTex);
TEXTURE2D(_GlowDissolveTex);
TEXTURE2D(_BurnTex);
TEXTURE2D(_OverlayTex);
TEXTURE2D(_UVScrollOverlayTex);
TEXTURE2D(_PaperTex);
TEXTURE2D(_ParallaxHeightTex);
#define sampler_DistortTex sampler_linear_repeat
#define sampler_DissolveTex sampler_linear_repeat
#define sampler_GlowDissolveTex sampler_linear_repeat
#define sampler_BurnTex sampler_linear_repeat
#define sampler_OverlayTex sampler_linear_repeat
#define sampler_UVScrollOverlayTex sampler_linear_repeat
#define sampler_PaperTex sampler_linear_repeat
#define sampler_ParallaxHeightTex sampler_linear_repeat



// ============================================================================
// SRP PATH: Single UnityPerMaterial CBUFFER for SRP Batcher compatibility
// ============================================================================
#if defined(AIO_SRP)

CBUFFER_START(UnityPerMaterial)

// ---- Globals ----
float4 _MainTex_TexelSize;
float4 _OutlineSDFTex_TexelSize;
float _OutlineEffectsOnly_En;
float _GlobalEffectFade;
float _EdgeCleanup;
float _PixelSnapResolution;
float _TimeScale;
float _TimeFPS;
float _UseCustomTime;
float _CustomTime;
float _CastShadows;
float _ReceiveShadows;
float _FlipNormals;
float _ShadowAlphaClip;
// ---- Project-wide lighting multipliers ----
float _GlobalLightIntensity;
float _AmbientLightIntensity;
float _HDRPExposureComp;
float _HDRPAmbientMul;
float _HDRPIndirectMul;
float _EffectMask_En;
float _PixelArtSnap_En;
float _MaskTex_En;
float _MinXUV;
float _MaxXUV;
float _MinYUV;
float _MaxYUV;
float _VertexUVExpand;
float _SpriteAspect;
float _AlphaCutoff_En;
float _AlphaCutoff;
float2 _WorldTileOffset;
float _WorldTileRotation;
float _EnchantedMaskCh;
float4 _CamoColor1;
float4 _CamoColor2;
float4 _CamoColor3;
float _CamoScale;
float _CamoSpeed;
float _CamoFade;
float _CRTDensity;
float _CRTIntensity;
float _CRTSpeed;
float _CRTCurvature;
float _ThermalIntensity;
float _ThermalHDRGlow;
float4 _RecolorR;
float4 _RecolorG;
float4 _RecolorB;
float4 _RecolorCyan;
float4 _RecolorMagenta;
float4 _RecolorYellow;
float _RecolorFade;
float4 _DirOutlineColor;
float _DirOutlineWidth;
float _DirOutlineInt; float2 _DirOutlineDir; float _DirOutlineSoft; float _DirOutlineMaskCh;
float _DistortAmount;
float _DistortSpeed;
float4 _DistortTiling;
float _DistortIsNormalMap;
float _WorldTileScale;
float _FlipBookLerp;
float4 _EnchantedColor;
float _EnchantedSpeed;
float _EnchantedDensity;
float _EnchantedBrightness;
float _EnchantedFade;
float _CheckerCoordSpace;
float _CheckerScale;
float4 _CheckerColor1;
float4 _CheckerColor2;
float _HealthDamageNoise;
float _HealthEdgeGrain;
float _HealthPulseSpeed;
float _HealthVignetteStr;
float _HealthDesaturation;
float _DialogueBobSpeed;
float _DialogueBobAmount;
float _DialogueBrightness;
float _CrackleScale;
float _CrackleSpeed;
float _CrackleEdgeWidth;
float4 _CrackleEdgeColor;
float _CrackleCellTint;
float _CrackleFade;
float _CrackleMaskCh;
float _InkBleedAmount;
float _InkBleedWidth;
float _InkBleedScale;
float _PaperScale;
float _PaperAmount;
float _InkBleedFade;
float _InkBleedMaskCh;
float4 _AuraColor;
float _AuraRadius;
float _AuraFrequency;
float _AuraSpeed;
float _AuraIntensity;
float _AuraFade;
float _AuraMaskCh;
float _RetroPixelSize;
float _RetroDitherStrength;
float _RetroFade;
float _RetroMaskCh;
float4 _SmearDirection;
float _SmearAmount;
float _SmearSamples;
float _SmearFade;
float _SmearMaskCh;
float _ChromaMode;
float4 _ChromaRadialCenter;
float _OutlineMode;
float _OutlineRings;
float _OutlineAA;
float _OutlineSDFSoftness;
float _OutlineSDFPadding;
float _OutlineSDFSpread;
float4 _OutlineSDFRect;
float _PremulAlpha;
float _SpriteFlipX;
float _SpriteFlipY;
float _LUTSize;
float _Enchanted_En;
float _EnergyAura_En;
float _InkBleed_En;

// ---- Color ----
float _StrongTintMaskCh;
float _AddColorMaskCh;
float _AlphaTintMaskCh;
float _HitEffectMaskCh;
float _NegativeMaskCh;
float _GreyscaleMaskCh;
float _SaturationMaskCh;
float _BrightnessMaskCh;
float _ContrastMaskCh;
float _ColorGradingMaskCh;
float _SepiaMaskCh;
float _PingPongGlowMaskCh;
float _ColorCycleMaskCh;
float4 _StrongTintColor;
float _StrongTintFade;
float4 _AddColorAmount;
float _AddColorFade;
float4 _AlphaTintColor;
float _AlphaTintFade;
float4 _HitEffectColor;
float _HitEffectBlend;
float _NegativeAmount;
float _GreyscaleBlend;
float4 _GreyscaleLuminosity;
float _SaturationAmount;
float _BrightnessAmount;
float _ContrastAmount;
float _CGExposure;
float _CGGamma;
float4 _CGLift;
float _SepiaIntensity;
float4 _SepiaColor;
float4 _PingPongGlowColor;
float _PingPongGlowSpeed;
float _PingPongGlowMin;
float _PingPongGlowMax;
float4 _ColorCycleA;
float4 _ColorCycleB;
float4 _ColorCycleC;
float4 _ColorCycleD;
float _ColorCycleSpeed;
float _ColorCycleFade;
float _AddColor_En;
float _AlphaTint_En;
float _Brightness_En;
float _ColorCycle_En;
float _ColorGrading_En;
float _Contrast_En;
float _Greyscale_En;
float _HitEffect_En;
float _Negative_En;
float _PingPongGlow_En;
float _Saturation_En;
float _Sepia_En;
float _StrongTint_En;

// ---- ColorExt ----
float _BloomMaskCh;
float _ColorReplaceMaskCh;
float _LUTReplaceMaskCh;
float _HueShiftMaskCh;
float _AddHueMaskCh;
float _SplitToningMaskCh;
float _PosterizeMaskCh;
float _PoisonMaskCh;
float _FrozenMaskCh;
float _MetalMaskCh;
float _CamouflageMaskCh;
float _RainbowMaskCh;
float _BlackTintMaskCh;
float _WaterReflectMaskCh;
float _ChromaticMaskCh;
float _PaletteSwapMaskCh;
float _DitheringMaskCh;
float _CRTScanlinesMaskCh;
float _VignetteMaskCh;
float _ThermalVisionMaskCh;
float _RecolorRGBMaskCh;
float _RainbowOutlineMaskCh;
float _BloomMode;
float4 _BloomColor;
float _BloomThreshold;
float _BloomIntensity;
float _BloomSize;
float _BloomSamples;
float _BloomSoftKnee;
float4 _BloomMaskColor;
float _BloomMaskRange;
float _BloomMaskSoft;
float _BloomAlphaBleed;
float4 _ColorReplaceSrc1;
float4 _ColorReplaceDst1;
float4 _ColorReplaceSrc2;
float4 _ColorReplaceDst2;
float4 _ColorReplaceSrc3;
float4 _ColorReplaceDst3;
float4 _ColorReplaceSrc4;
float4 _ColorReplaceDst4;
float _ColorReplaceCount;
float _ColorReplaceRange;
float _ColorReplaceFade;
float _LUTReplaceFade;
float _HueShiftAmount;
float _HueShiftSpeed;
float _HueShiftFade;
float _AddHueAmount;
float _AddHueSpeed;
float _AddHueSaturation;
float _AddHueBrightness;
float _AddHueFade;
float4 _SplitShadowColor;
float4 _SplitMidColor;
float4 _SplitHighlightColor;
float _SplitBalance;
float _SplitFade;
float _PosterizeSteps;
float _PosterizeFade;
float4 _PoisonColor;
float4 _PoisonColor2;
float4 _PoisonColor3;
float _PoisonSpeed;
float _PoisonDensity;
float _PoisonFade;
float4 _FrozenColor;
float4 _FrozenColor2;
float4 _FrozenHighlightColor;
float _FrozenContrast;
float _FrozenHighlight;
float _FrozenFade;
float4 _MetalColor;
float4 _MetalColor2;
float4 _MetalHighlight;
float _MetalContrast;
float _MetalSpeed;
float _MetalFade;
float _RainbowSpeed;
float _RainbowSaturation;
float _RainbowBrightness;
float _RainbowFade;
float _RainbowDensity;
float4 _BlackTintColor;
float _BlackTintThreshold;
float _BlackTintFade;
float _WaterReflectYPoint;
float _WaterReflectDepth;
float _WaterReflectOpacity;
float _WaterReflectFade;
float4 _WaterReflectTint;
float _WaterReflectTintAmount;
float _WaterReflectWaveFreq;
float _WaterReflectWaveSpeed;
float _WaterReflectWaveAmp;
float _ChromaticAmount;
float _ChromaticAngle;
float _PaletteSwapColors;
float _PaletteSwapFade;
float _DitheringSize;
float _DitheringLevels;
float _DitheringFade;
float _DitheringAlphaMode;
float _DitheringAlphaThreshold;
float _DitheringScreenSpace;
float4 _VignetteColor;
float _VignetteIntensity;
float _VignetteSmoothness;
float _VignetteAlpha;
float4 _VignetteCenter;
float2 _VignetteOffset;
float4 _RainbowOutlineColor;
float _RainbowOutlineSize;
float _RainbowOutlineSpeed;
float _RainbowOutlineDensity;
float _AddHue_En;
float _BlackTint_En;
float _Bloom_En;
float _Camouflage_En;
float _Chromatic_En;
float _ColorReplace_En;
float _CRTScanlines_En;
float _Dithering_En;
float _Frozen_En;
float _HueShift_En;
float _LUTReplace_En;
float _Metal_En;
float _PaletteSwap_En;
float _Poison_En;
float _Posterize_En;
float _RainbowOutline_En;
float _Rainbow_En;
float _RecolorRGB_En;
float _SplitToning_En;
float _ThermalVision_En;
float _Vignette_En;
float _WaterReflect_En;

// ---- NEW: Part 2 ----
float _Gradient_En; float4 _GradientColorA; float4 _GradientColorB; float _GradientAngle; float _GradientFade; float _GradientMaskCh; float _GradientUseTex; float _GradientBlendMode;
float _RadialGradient_En; float4 _RadialGradCenterColor; float4 _RadialGradEdgeColor; float4 _RadialGradCenter; float _RadialGradRadius; float _RadialGradFade; float _RadialGradMaskCh; float _RadialGradUseTex; float _RadialGradBlendMode;
float _ColorRamp_En; float _ColorRampMode; float _ColorRampIntensity; float _ColorRampFade; float _ColorRampMaskCh; float4 _ColorRampColorA; float4 _ColorRampColorB;


// ---- Edge ----
float _OutlineEffectWidth;
float _OutlineFixedWidthPx;

float _OutlineMaskCh;
float _InnerOutlineMaskCh;
float _TexOutlineMaskCh;
float _DistortOutlineMaskCh;
float _GlowMaskCh;
float _ShadowMaskCh;
float _SoftShadowMaskCh;

float _OutlineOnlyMaskCh;
float _GradOutlineMaskCh;
float _AnimOutlineMaskCh;
float _SelectHighlightMaskCh;
float _GlowDissolveMaskCh;
float4 _OutlineColor;
float _OutlineSize;
float _OutlineAlpha;
float4 _InnerOutlineColor;
float _InnerOutlineThickness;
float _InnerOutlineAlpha;
float4 _TexOutlineColor;
float _TexOutlineSize;
float4 _TexOutlineSpeed;
float4 _TexOutlineTiling;
float4 _DistortOutlineColor;
float _DistortOutlineSize;
float _DistortOutlineAmount;
float _DistortOutlineSpeed;
float4 _GlowColor;
float _GlowSize;
float _GlowIntensity;
float4 _ShadowColor;
float4 _ShadowOffset;
float4 _SoftShadowColor;
float4 _SoftShadowOffset;
float _SoftShadowSoftness;
float _SoftShadowSamples;
float4 _OutlineOnlyColor;
float _OutlineOnlyWidth;
float4 _GradOutlineColorA;
float4 _GradOutlineColorB;
float _GradOutlineSize;
float _GradOutlineAngle;
float4 _AnimOutlineColor;
float _AnimOutlineMin;
float _AnimOutlineMax;
float _AnimOutlineSpeed;
float4 _SelectHighlightColor;
float _SelectHighlightSize;
float _SelectHighlightDash;
float _SelectHighlightSpeed;
float _GlowDissolveNoiseType;
float _GlowDissolveNoiseScale;
float _GlowDissolveAmount;
float4 _GlowDissolveColor;
float _GlowDissolveWidth;
float _AnimOutline_En;
float _DistortOutline_En;
float _GlowDissolve_En;
float _Glow_En;
float _GradOutline_En;
float _InnerOutline_En;
float _OutlineOnly_En;
float _Outline_En;
float _DirOutline_En;
float _SelectHighlight_En;
float _Shadow_En;
float _SoftShadow_En;
float _TexOutline_En;

// ---- NEW: Part 2 ----
float _FakeLighting_En; float4 _FakeLightDir; float4 _FakeLightColor; float _FakeLightIntensity; float _FakeLightAmbient;
float _FakeLightNDist; float _FakeLightNZ; float _FakeLightSpecPow; float _FakeLightSpecInt; float _FakeLightFade; float _FakeLightMaskCh;


// ---- UV ----
float _PixelateMaskCh;
float _WaveMaskCh;
float _TwistMaskCh;
float _UVDistortMaskCh;
float _FullDistortMaskCh;
float _DirDistortMaskCh;
float _UVScrollMaskCh;
float _RotateMaskCh;
float _SqueezeMaskCh;
float _SquishMaskCh;
float _WiggleMaskCh;
float _WindMaskCh;
float _WorldTilingMaskCh;
float _ShockwaveMaskCh;
float _SineScaleMaskCh;
float _HeartbeatMaskCh;
float _ShockwaveSpeed;
float _BreathingMaskCh;
float _LiquidWobbleMaskCh;
float _SmoothPixelArtMaskCh;
float _FBMNoiseMaskCh;
float _CurlNoiseMaskCh;
float _UVScaleOffsetMaskCh;
float _DialogueEmphasisMaskCh;
float _PixelateSize;
float _WaveSpeed;
float _WaveAmplitude;
float _WaveFrequency;
float _TwistAmount;
float _TwistSpeed;
float _TwistRadius;
float2 _TwistOffset;
float _FullDistortAmount;
float _FullDistortSpeed;
float _FullDistortDensity;
float _DirDistortAngle;
float _DirDistortAmount;
float _DirDistortSpeed;
float _FBMNoiseAmount;
float _FBMNoiseSpeed;
float _FBMNoiseDensity;
float _FBMNoiseOctaves;
float _CurlNoiseAmount;
float _CurlNoiseSpeed;
float _CurlNoiseDensity;
float4 _UVScaleOffset;
float _UVScrollSpeedX;
float _UVScrollSpeedY;
float _RotateSpeed;
float _RotateAmount;
float4 _SqueezeAmount;
float _SqueezeSpeed;
float _SqueezePower;
float4 _SquishAmount;
float _SquishSpeed;
float _SquishPower;
float _WiggleSpeed;
float _WiggleAmount;
float _WiggleFrequency;
float _WindSpeed;
float _WindAmount;
float _WindFrequency;
float _WindBottomY;
float4 _ShockwaveCenter;
float _ShockwaveRadius;
float _ShockwaveWidth;
float _ShockwaveAmplitude;
float _SineScaleAmountX;
float _SineScaleAmountY;
float _SineScaleSpeed;
float _HeartbeatSpeed;
float _HeartbeatAmount;
float _BreathingSpeed;
float _BreathingAmount;
float _LiquidWobbleSpeed;
float _LiquidWobbleAmount;
float _LiquidWobbleFreq;
float _SmoothPixelArtTexelSize;
float _UVScrollOverlayBlend;
float _UVScrollOverlaySpeedX;
float _UVScrollOverlaySpeedY;
float _UVScrollOverlayMaskCh;
float4 _UVScrollOverlayTint;
float4 _UVScrollOverlayTiling;
float _Breathing_En;
float _CurlNoise_En;
float _DialogueEmphasis_En;
float _DirDistort_En;
float _FBMNoise_En;
float _FullDistort_En;
float _Heartbeat_En;
float _LiquidWobble_En;
float _Pixelate_En;
float _Rotate_En;
float _Shockwave_En;
float _SineScale_En;
float _SmoothPixelArt_En;
float _Squeeze_En;
float _Squish_En;
float _Twist_En;
float _UVDistort_En;
float _UVScaleOffset_En;
float _UVScrollOverlay_En;
float _UVScroll_En;
float _Wave_En;
float _Wiggle_En;
float _Wind_En;
float _WorldTiling_En;

// ---- NEW: Part 2 ----
float _UVBendPoint_En; float4 _BendLocalPoint; float4 _BendWorldPoint; float _BendUseWorldSpace; float _BendWorldScale;
float _BendRadius; float _BendAmount; float _BendFalloff; float _BendInvert; float _BendMaskCh;


// ---- Filter ----
float _SharpenMaskCh;
float _GaussBlurMaskCh;
float _DirBlurMaskCh;
float _FlipBookBlendMaskCh;
float _SharpenAmount;
float _SharpenSize;
float _GaussBlurSize;
float _GaussBlurSamples;
float _DirBlurAngle;
float _DirBlurSize;
float _DirBlurSamples;
float _DirBlur_En;
float _FlipBookBlend_En;
float _GaussBlur_En;
float _Sharpen_En;

// ---- Dissolve ----
float _DissolveMaskCh;
float _SrcDissolveMaskCh;
float _DirFadeMaskCh;
float _DirGlowFadeMaskCh;
float _FullAlphaDissolveMaskCh;
float _HalftoneMaskCh;
float _RadialWipeMaskCh;
float _BlindsMaskCh;
float _DiamondDissolveMaskCh;
float _ScanlineDissolveMaskCh;
float _SpreadFadeMaskCh;
float _PixelDissolveMaskCh;
float _SpiralWipeMaskCh;
float _HexDissolveMaskCh;
float _DissolveNoiseType;
float _DissolveNoiseScale;
float _DissolveAmount;
float _DissolveBorderWidth;
float4 _DissolveBorderColor;
float _DissolveBurnWidth;
float4 _DissolveBurnColor;
float4 _DissolveTiling;
float _SrcDissolveAmount;
float4 _SrcDissolvePosition;
float4 _SrcDissolveBorderColor;
float _SrcDissolveBorderWidth;
float _DirFadeAngle;
float _DirFadeAmount;
float _DirFadeSmoothness;
float _DirGlowFadeAngle;
float _DirGlowFadeAmount;
float4 _DirGlowFadeColor;
float _DirGlowFadeWidth;
float _FullAlphaDissolveAmount;
float _HalftoneSize;
float _HalftoneAmount;
float _HalftoneSmoothness;
float _RadialWipeAmount;
float _RadialWipeSmoothness;
float _RadialWipeAngle;
float _BlindsAmount;
float _BlindsCount;
float _BlindsDirection;
float _DiamondDissolveAmount;
float _DiamondDissolveScale;
float _ScanlineDissolveAmount;
float _ScanlineDissolveDensity;
float _SpreadFadeAmount;
float4 _SpreadFadeOrigin;
float _SpreadFadeSmoothness;
float _PixelDissolveAmount;
float _PixelDissolveGrid;
float _SpiralWipeAmount;
float _SpiralWipeSmoothness;
float _SpiralWipeArms;
float _HexDissolveAmount;
float _HexDissolveScale;
float _DissolveNoiseSpeed;
float _Blinds_En;
float _DiamondDissolve_En;
float _DirFade_En;
float _DirGlowFade_En;
float _Dissolve_En;
float _FullAlphaDissolve_En;
float _Halftone_En;
float _HexDissolve_En;
float _PixelDissolve_En;
float _RadialWipe_En;
float _ScanlineDissolve_En;
float _SpiralWipe_En;
float _SpreadFade_En;
float _SrcDissolve_En;

// ---- NEW: Part 2 ----
float _BurnDirection_En; float _BurnDirAngle; float _BurnDirAmount; float _BurnDirEdgeWidth;
float4 _BurnDirFireColor; float4 _BurnDirCharcoalColor; float _BurnDirCharcoalWidth; float _BurnDirNoiseScale; float _BurnDirMaskCh;


// ---- VFX ----
float _HologramMaskCh;
float _GlitchMaskCh;
float _FlickerMaskCh;
float _ShineMaskCh;
float _SmokeMaskCh;
float _FlameMaskCh;
float _ShiftingMaskCh;
float _ShiftRainbowMaskCh;
float _OverlayMaskCh;
float _HologramStripesAmount;
float _HologramMinAlpha;
float _HologramStripesSpeed;
float4 _HologramColor;
float _HologramBlend;
float _HologramDistortion;
float _GlitchAmount;
float _GlitchSpeed;
float _GlitchSize;
float _GlitchColorShift;
float _FlickerFreq;
float _FlickerPercent;
float4 _ShineColor;
float _ShineWidth;
float _ShineSpeed;
float _ShineDelay;
float _ShineRotation;
float _ShineGlow;
float4 _SmokeColor;
float _SmokeDensity;
float _SmokeSpeed;
float _SmokeFade;
float _SmokeTurbulence;
float _SmokeColorStr;
float4 _SmokeDirection;
float4 _FlameColor1;
float4 _FlameColor2;
float _FlameSpeed;
float _FlameDensity;
float _FlameIntensity;
float _FlameOnlyMode;
float _FlameExtendSides;
float _FlameSideFalloff;
float4 _ShiftingColor1;
float4 _ShiftingColor2;
float _ShiftingSpeed;
float _ShiftingFade;
float _ShiftRainbowSpeed;
float _ShiftRainbowDensity;
float _ShiftRainbowFade;
float4 _OverlayColor;
float _OverlayBlend;
float4 _OverlaySpeed;
float4 _OverlayTiling;
float _Flame_En;
float _Flicker_En;
float _Glitch_En;
float _Hologram_En;
float _MotionSmear_En;
float _Overlay_En;
float _Shifting_En;
float _ShiftRainbow_En;
float _Shine_En;
float _Smoke_En;
float _VoronoiCrackle_En;

// ---- NEW: Part 2 ----
float _EnchantedColorVariation; float _EnchantedSizeMix; float _EnchantedShimmer;
float _GlobalSmoke_En; float4 _GlobalSmokeColor; float _GlobalSmokeDensity; float _GlobalSmokeSpeed; float _GlobalSmokeOpacity; float4 _GlobalSmokeDir; float _GlobalSmokeMaskCh; float _GlobalSmokeOverlayOnly;
float _HoloCard_En; float _HoloCardDensity; float _HoloCardSpeed; float _HoloCardSat; float _HoloCardBright;
float _HoloCardFPow; float _HoloCardFShift; float _HoloCardScanDen; float _HoloCardScanSpd; float _HoloCardScanInt; float _HoloCardFade; float _HoloCardMaskCh;
float _CloudShadows_En; float _CloudShadowScale; float4 _CloudShadowSpeed; float _CloudShadowDark; float _CloudShadowThresh; float _CloudShadowSoft; float _CloudShadowFade; float _CloudShadowMaskCh; float _CloudShadowOverlayOnly;
float _TrailA_En; float4 _TrailAVel; float4 _TrailASlowCol; float4 _TrailAFastCol; float _TrailAMaxSpd; float _TrailACurve; float _TrailAInt; float _TrailAMaskCh;
float _TrailB_En; float4 _TrailBVel; float4 _TrailBCol; float _TrailBMaxSpd; float _TrailBCurve; float _TrailBInt; float _TrailBSegs; float _TrailBFade; float _TrailBMaskCh;
float _TrailC_En; float4 _TrailCVel; float4 _TrailCColA; float4 _TrailCColB; float _TrailCMaxSpd; float _TrailCCurve; float _TrailCInt; float _TrailCRbSpd; float _TrailCMaskCh;
float _Charging_En; float4 _ChargeColor; float4 _ChargeCenter; float _ChargeRadius; float _ChargeWidth;
float _ChargeSpeed; float _ChargeFreq; float _ChargeDensity; float _ChargeInt; float _ChargeLines; float _ChargeFade; float _ChargeMaskCh;


// ---- Game ----
float _CheckerboardMaskCh;
float _UVVisualizeMaskCh;
float _UVCheckerMaskCh;
float _UVNumericMaskCh;
float _HealthTintMaskCh;
float _XRaySilhouetteMaskCh;
float _MinimapPingMaskCh;
float _GhostingMaskCh;
float2 _MinimapPingOffset;
float _UVVisualizeBlend;
float _UVCheckerScale;
float _UVCheckerBlend;
float _UVNumericScale;
float _UVNumericBlend;
float4 _HealthTintLowColor;
float4 _HealthTintHighColor;
float _HealthTintAmount;
float4 _XRaySilhouetteColor;
float _XRaySilhouetteBlend;
float4 _MinimapPingColor;
float _MinimapPingSpeed;
float _MinimapPingWidth;
float _GhostingCount;
float4 _GhostingOffset;
float _GhostingFade;
float _RetroPaletteSize;
float _Checkerboard_En;
float _Ghosting_En;
float _HealthTint_En;
float _MinimapPing_En;
float _RetroPalette_En;
float _UVChecker_En;
float _UVNumeric_En;
float _UVVisualize_En;
float _XRaySilhouette_En;


// ---- NEW: Part 2 ----
float _Blueprint_En; float4 _BlueprintBgCol; float4 _BlueprintLineCol; float4 _BlueprintGridCol;
float _BlueprintEdgeSz; float _BlueprintLineWidth; float _BlueprintLineInt; float _BlueprintGridDen; float _BlueprintGridOp; float _BlueprintFade; float _BlueprintMaskCh;

CBUFFER_END

// ============================================================================
// BUILT-IN PATH: one compact material CBUFFER
// Keeps FlexiSprite uniforms in a cbuffer while using only one D3D11 slot.
// Group guards still strip inactive fields from variants.
// ============================================================================
#else // !AIO_SRP

CBUFFER_START(AllInOneMaterial)
float4 _MainTex_TexelSize;
float4 _OutlineSDFTex_TexelSize;
half _OutlineEffectsOnly_En;
half _GlobalEffectFade;
half _EdgeCleanup;
half _PixelSnapResolution;
half _TimeScale;
half _TimeFPS;
half _UseCustomTime;
half _CustomTime;
half _CastShadows;
half _ReceiveShadows;
half _FlipNormals;
half _ShadowAlphaClip;
// ---- Project-wide lighting multipliers ----
half _GlobalLightIntensity;
half _AmbientLightIntensity;
half _HDRPExposureComp;
half _HDRPAmbientMul;
half _HDRPIndirectMul;
half _EffectMask_En;
half _PixelArtSnap_En;
half _MaskTex_En;
half _MinXUV;
half _MaxXUV;
half _MinYUV;
half _MaxYUV;
half _VertexUVExpand;
half _SpriteAspect;
half _AlphaCutoff_En;
half _AlphaCutoff;
half2 _WorldTileOffset;
half _WorldTileRotation;
half _EnchantedMaskCh;
half4 _CamoColor1;
half4 _CamoColor2;
half4 _CamoColor3;
half _CamoScale;
half _CamoSpeed;
half _CamoFade;
half _CRTDensity;
half _CRTIntensity;
half _CRTSpeed;
half _CRTCurvature;
half _ThermalIntensity;
half _ThermalHDRGlow;
half4 _RecolorR;
half4 _RecolorG;
half4 _RecolorB;
half4 _RecolorCyan;
half4 _RecolorMagenta;
half4 _RecolorYellow;
half _RecolorFade;
half4 _DirOutlineColor;
half _DirOutlineWidth;
half _DirOutlineInt;
half2 _DirOutlineDir;
half _DirOutlineSoft;
half _DirOutlineMaskCh;
half _DistortAmount;
half _DistortSpeed;
half4 _DistortTiling;
half _DistortIsNormalMap;
half _WorldTileScale;
half _FlipBookLerp;
half4 _EnchantedColor;
half _EnchantedSpeed;
half _EnchantedDensity;
half _EnchantedBrightness;
half _EnchantedFade;
half _CheckerCoordSpace;
half _CheckerScale;
half4 _CheckerColor1;
half4 _CheckerColor2;
half _HealthDamageNoise;
half _HealthEdgeGrain;
half _HealthPulseSpeed;
half _HealthVignetteStr;
half _HealthDesaturation;
half _DialogueBobSpeed;
half _DialogueBobAmount;
half _DialogueBrightness;
half _CrackleScale;
half _CrackleSpeed;
half _CrackleEdgeWidth;
half4 _CrackleEdgeColor;
half _CrackleCellTint;
half _CrackleFade;
half _CrackleMaskCh;
half _InkBleedAmount;
half _InkBleedWidth;
half _InkBleedScale;
half _PaperScale;
half _PaperAmount;
half _InkBleedFade;
half _InkBleedMaskCh;
half4 _AuraColor;
half _AuraRadius;
half _AuraFrequency;
half _AuraSpeed;
half _AuraIntensity;
half _AuraFade;
half _AuraMaskCh;
half _RetroPixelSize;
half _RetroDitherStrength;
half _RetroFade;
half _RetroMaskCh;
half4 _SmearDirection;
half _SmearAmount;
half _SmearSamples;
half _SmearFade;
half _SmearMaskCh;
half _ChromaMode;
half4 _ChromaRadialCenter;
half _OutlineMode;
half _OutlineRings;
half _OutlineAA;
half _OutlineSDFSoftness;
half _OutlineSDFPadding;
half _OutlineSDFSpread;
half4 _OutlineSDFRect;
half _PremulAlpha;
half _SpriteFlipX;
half _SpriteFlipY;
half _LUTSize;
half _Enchanted_En;
half _EnergyAura_En;
half _InkBleed_En;

#ifdef _FX_COLOR
half _StrongTintMaskCh;
half _AddColorMaskCh;
half _AlphaTintMaskCh;
half _HitEffectMaskCh;
half _NegativeMaskCh;
half _GreyscaleMaskCh;
half _SaturationMaskCh;
half _BrightnessMaskCh;
half _ContrastMaskCh;
half _ColorGradingMaskCh;
half _SepiaMaskCh;
half _PingPongGlowMaskCh;
half _ColorCycleMaskCh;
half4 _StrongTintColor;
half _StrongTintFade;
half4 _AddColorAmount;
half _AddColorFade;
half4 _AlphaTintColor;
half _AlphaTintFade;
half4 _HitEffectColor;
half _HitEffectBlend;
half _NegativeAmount;
half _GreyscaleBlend;
half4 _GreyscaleLuminosity;
half _SaturationAmount;
half _BrightnessAmount;
half _ContrastAmount;
half _CGExposure;
half _CGGamma;
half4 _CGLift;
half _SepiaIntensity;
half4 _SepiaColor;
half4 _PingPongGlowColor;
half _PingPongGlowSpeed;
half _PingPongGlowMin;
half _PingPongGlowMax;
half4 _ColorCycleA;
half4 _ColorCycleB;
half4 _ColorCycleC;
half4 _ColorCycleD;
half _ColorCycleSpeed;
half _ColorCycleFade;
half _AddColor_En;
half _AlphaTint_En;
half _Brightness_En;
half _ColorCycle_En;
half _ColorGrading_En;
half _Contrast_En;
half _Greyscale_En;
half _HitEffect_En;
half _Negative_En;
half _PingPongGlow_En;
half _Saturation_En;
half _Sepia_En;
half _StrongTint_En;
#endif

#ifdef _FX_COLOR_EXT
half _BloomMaskCh;
half _ColorReplaceMaskCh;
half _LUTReplaceMaskCh;
half _HueShiftMaskCh;
half _AddHueMaskCh;
half _SplitToningMaskCh;
half _PosterizeMaskCh;
half _PoisonMaskCh;
half _FrozenMaskCh;
half _MetalMaskCh;
half _CamouflageMaskCh;
half _RainbowMaskCh;
half _BlackTintMaskCh;
half _WaterReflectMaskCh;
half _ChromaticMaskCh;
half _PaletteSwapMaskCh;
half _DitheringMaskCh;
half _CRTScanlinesMaskCh;
half _VignetteMaskCh;
half _ThermalVisionMaskCh;
half _RecolorRGBMaskCh;
half _BloomMode;
half4 _BloomColor;
half _BloomThreshold;
half _BloomIntensity;
half _BloomSize;
half _BloomSamples;
half _BloomSoftKnee;
half4 _BloomMaskColor;
half _BloomMaskRange;
half _BloomMaskSoft;
half _BloomAlphaBleed;
half4 _ColorReplaceSrc1;
half4 _ColorReplaceDst1;
half4 _ColorReplaceSrc2;
half4 _ColorReplaceDst2;
half4 _ColorReplaceSrc3;
half4 _ColorReplaceDst3;
half4 _ColorReplaceSrc4;
half4 _ColorReplaceDst4;
half _ColorReplaceCount;
half _ColorReplaceRange;
half _ColorReplaceFade;
half _LUTReplaceFade;
half _HueShiftAmount;
half _HueShiftSpeed;
half _HueShiftFade;
half _AddHueAmount;
half _AddHueSpeed;
half _AddHueSaturation;
half _AddHueBrightness;
half _AddHueFade;
half4 _SplitShadowColor;
half4 _SplitMidColor;
half4 _SplitHighlightColor;
half _SplitBalance;
half _SplitFade;
half _PosterizeSteps;
half _PosterizeFade;
half4 _PoisonColor;
half4 _PoisonColor2;
half4 _PoisonColor3;
half _PoisonSpeed;
half _PoisonDensity;
half _PoisonFade;
half4 _FrozenColor;
half4 _FrozenColor2;
half4 _FrozenHighlightColor;
half _FrozenContrast;
half _FrozenHighlight;
half _FrozenFade;
half4 _MetalColor;
half4 _MetalColor2;
half4 _MetalHighlight;
half _MetalContrast;
half _MetalSpeed;
half _MetalFade;
half _RainbowSpeed;
half _RainbowSaturation;
half _RainbowBrightness;
half _RainbowFade;
half _RainbowDensity;
half4 _BlackTintColor;
half _BlackTintThreshold;
half _BlackTintFade;
half _WaterReflectYPoint;
half _WaterReflectDepth;
half _WaterReflectOpacity;
half _WaterReflectFade;
half4 _WaterReflectTint;
half _WaterReflectTintAmount;
half _WaterReflectWaveFreq;
half _WaterReflectWaveSpeed;
half _WaterReflectWaveAmp;
half _ChromaticAmount;
half _ChromaticAngle;
half _PaletteSwapColors;
half _PaletteSwapFade;
half _DitheringSize;
half _DitheringLevels;
half _DitheringFade;
half _DitheringAlphaMode;
half _DitheringAlphaThreshold;
half _DitheringScreenSpace;
half4 _VignetteColor;
half _VignetteIntensity;
half _VignetteSmoothness;
half _VignetteAlpha;
half4 _VignetteCenter;
half2 _VignetteOffset;

half _AddHue_En;
half _BlackTint_En;
half _Bloom_En;
half _Camouflage_En;
half _Chromatic_En;
half _ColorReplace_En;
half _CRTScanlines_En;
half _Dithering_En;
half _Frozen_En;
half _HueShift_En;
half _LUTReplace_En;
half _Metal_En;
half _PaletteSwap_En;
half _Poison_En;
half _Posterize_En;

half _Rainbow_En;
half _RecolorRGB_En;
half _SplitToning_En;
half _ThermalVision_En;
half _Vignette_En;
half _WaterReflect_En;

// ---- NEW: Part 2 ----
half _Gradient_En; half4 _GradientColorA; half4 _GradientColorB; half _GradientAngle; half _GradientFade; half _GradientMaskCh; half _GradientUseTex; half _GradientBlendMode;
half _RadialGradient_En; half4 _RadialGradCenterColor; half4 _RadialGradEdgeColor; half4 _RadialGradCenter; half _RadialGradRadius; half _RadialGradFade; half _RadialGradMaskCh; half _RadialGradUseTex; half _RadialGradBlendMode;
half _ColorRamp_En; half _ColorRampMode; half _ColorRampIntensity; half _ColorRampFade; half _ColorRampMaskCh; half4 _ColorRampColorA; half4 _ColorRampColorB;
#endif

#ifdef _FX_EDGE
half4 _RainbowOutlineColor;
half _RainbowOutlineSize;
half _RainbowOutlineSpeed;
half _RainbowOutlineDensity;
half _RainbowOutlineMaskCh;
half _OutlineEffectWidth;
half _RainbowOutline_En;
half _OutlineFixedWidthPx;

half _OutlineMaskCh;
half _InnerOutlineMaskCh;
half _TexOutlineMaskCh;
half _DistortOutlineMaskCh;
half _GlowMaskCh;
half _ShadowMaskCh;
half _SoftShadowMaskCh;

half _OutlineOnlyMaskCh;
half _GradOutlineMaskCh;
half _AnimOutlineMaskCh;
half _SelectHighlightMaskCh;

half4 _OutlineColor;
half _OutlineSize;
half _OutlineAlpha;
half4 _InnerOutlineColor;
half _InnerOutlineThickness;
half _InnerOutlineAlpha;
half4 _TexOutlineColor;
half _TexOutlineSize;
half4 _TexOutlineSpeed;
half4 _TexOutlineTiling;
half4 _DistortOutlineColor;
half _DistortOutlineSize;
half _DistortOutlineAmount;
half _DistortOutlineSpeed;
half4 _GlowColor;
half _GlowSize;
half _GlowIntensity;
half4 _ShadowColor;
half4 _ShadowOffset;
half4 _SoftShadowColor;
half4 _SoftShadowOffset;
half _SoftShadowSoftness;
half _SoftShadowSamples;
half4 _OutlineOnlyColor;
half _OutlineOnlyWidth;
half4 _GradOutlineColorA;
half4 _GradOutlineColorB;
half _GradOutlineSize;
half _GradOutlineAngle;
half4 _AnimOutlineColor;
half _AnimOutlineMin;
half _AnimOutlineMax;
half _AnimOutlineSpeed;
half4 _SelectHighlightColor;
half _SelectHighlightSize;
half _SelectHighlightDash;
half _SelectHighlightSpeed;
half _AnimOutline_En;
half _DistortOutline_En;
half _Glow_En;
half _GradOutline_En;
half _InnerOutline_En;
half _OutlineOnly_En;
half _Outline_En;
half _DirOutline_En;
half _SelectHighlight_En;
half _Shadow_En;
half _SoftShadow_En;
half _TexOutline_En;

// ---- NEW: Part 2 ----
half _FakeLighting_En; half4 _FakeLightDir; half4 _FakeLightColor; half _FakeLightIntensity; half _FakeLightAmbient;
half _FakeLightNDist; half _FakeLightNZ; half _FakeLightSpecPow; half _FakeLightSpecInt; half _FakeLightFade; half _FakeLightMaskCh;
#endif

#ifdef _FX_UV

half _PixelateMaskCh;
half _WaveMaskCh;
half _TwistMaskCh;
half _UVDistortMaskCh;
half _FullDistortMaskCh;
half _DirDistortMaskCh;
half _UVScrollMaskCh;
half _RotateMaskCh;
half _SqueezeMaskCh;
half _SquishMaskCh;
half _WiggleMaskCh;
half _WindMaskCh;
half _WorldTilingMaskCh;
half _ShockwaveMaskCh;
half _SineScaleMaskCh;
half _HeartbeatMaskCh;
half _ShockwaveSpeed;
half _BreathingMaskCh;
half _LiquidWobbleMaskCh;
half _SmoothPixelArtMaskCh;
half _FBMNoiseMaskCh;
half _CurlNoiseMaskCh;
half _UVScaleOffsetMaskCh;
half _PixelateSize;
half _WaveSpeed;
half _WaveAmplitude;
half _WaveFrequency;
half _TwistAmount;
half _TwistSpeed;
half _TwistRadius;
half2 _TwistOffset;
half _FullDistortAmount;
half _FullDistortSpeed;
half _FullDistortDensity;
half _DirDistortAngle;
half _DirDistortAmount;
half _DirDistortSpeed;
half _FBMNoiseAmount;
half _FBMNoiseSpeed;
half _FBMNoiseDensity;
half _FBMNoiseOctaves;
half _CurlNoiseAmount;
half _CurlNoiseSpeed;
half _CurlNoiseDensity;
half4 _UVScaleOffset;
half _UVScrollSpeedX;
half _UVScrollSpeedY;
half _RotateSpeed;
half _RotateAmount;
half4 _SqueezeAmount;
half _SqueezeSpeed;
half _SqueezePower;
half4 _SquishAmount;
half _SquishSpeed;
half _SquishPower;
half _WiggleSpeed;
half _WiggleAmount;
half _WiggleFrequency;
half _WindSpeed;
half _WindAmount;
half _WindFrequency;
half _WindBottomY;
half4 _ShockwaveCenter;
half _ShockwaveRadius;
half _ShockwaveWidth;
half _ShockwaveAmplitude;
half _SineScaleAmountX;
half _SineScaleAmountY;
half _SineScaleSpeed;
half _HeartbeatSpeed;
half _HeartbeatAmount;
half _BreathingSpeed;
half _BreathingAmount;
half _LiquidWobbleSpeed;
half _LiquidWobbleAmount;
half _LiquidWobbleFreq;
half _SmoothPixelArtTexelSize;

half _Breathing_En;
half _CurlNoise_En;

half _DirDistort_En;
half _FBMNoise_En;
half _FullDistort_En;
half _Heartbeat_En;
half _LiquidWobble_En;
half _Pixelate_En;
half _Rotate_En;
half _Shockwave_En;
half _SineScale_En;
half _SmoothPixelArt_En;
half _Squeeze_En;
half _Squish_En;
half _Twist_En;
half _UVDistort_En;
half _UVScaleOffset_En;
half _UVScroll_En;
half _Wave_En;
half _Wiggle_En;
half _Wind_En;
half _WorldTiling_En;

// ---- NEW: Part 2 ----
half _UVBendPoint_En; half4 _BendLocalPoint; half4 _BendWorldPoint; half _BendUseWorldSpace; half _BendWorldScale;
half _BendRadius; half _BendAmount; half _BendFalloff; half _BendInvert; half _BendMaskCh;
#endif

#ifdef _FX_FILTER
half _UVScrollOverlay_En;
half _UVScrollOverlayBlend;
half _UVScrollOverlaySpeedX;
half _UVScrollOverlaySpeedY;
half _UVScrollOverlayMaskCh;
half4 _UVScrollOverlayTint;
half4 _UVScrollOverlayTiling;
half _SharpenMaskCh;
half _GaussBlurMaskCh;
half _DirBlurMaskCh;
half _FlipBookBlendMaskCh;
half _SharpenAmount;
half _SharpenSize;
half _GaussBlurSize;
half _GaussBlurSamples;
half _DirBlurAngle;
half _DirBlurSize;
half _DirBlurSamples;
half _DirBlur_En;
half _FlipBookBlend_En;
half _GaussBlur_En;
half _Sharpen_En;
#endif

#ifdef _FX_DISSOLVE
half _GlowDissolve_En;
half _GlowDissolveNoiseType;
half _GlowDissolveNoiseScale;
half _GlowDissolveAmount;
half4 _GlowDissolveColor;
half _GlowDissolveWidth;
half _GlowDissolveMaskCh;
half _DissolveMaskCh;
half _SrcDissolveMaskCh;
half _DirFadeMaskCh;
half _DirGlowFadeMaskCh;
half _FullAlphaDissolveMaskCh;
half _HalftoneMaskCh;
half _RadialWipeMaskCh;
half _BlindsMaskCh;
half _DiamondDissolveMaskCh;
half _ScanlineDissolveMaskCh;
half _SpreadFadeMaskCh;
half _PixelDissolveMaskCh;
half _SpiralWipeMaskCh;
half _HexDissolveMaskCh;
half _DissolveNoiseType;
half _DissolveNoiseScale;
half _DissolveAmount;
half _DissolveBorderWidth;
half4 _DissolveBorderColor;
half _DissolveBurnWidth;
half4 _DissolveBurnColor;
half4 _DissolveTiling;
half _SrcDissolveAmount;
half4 _SrcDissolvePosition;
half4 _SrcDissolveBorderColor;
half _SrcDissolveBorderWidth;
half _DirFadeAngle;
half _DirFadeAmount;
half _DirFadeSmoothness;
half _DirGlowFadeAngle;
half _DirGlowFadeAmount;
half4 _DirGlowFadeColor;
half _DirGlowFadeWidth;
half _FullAlphaDissolveAmount;
half _HalftoneSize;
half _HalftoneAmount;
half _HalftoneSmoothness;
half _RadialWipeAmount;
half _RadialWipeSmoothness;
half _RadialWipeAngle;
half _BlindsAmount;
half _BlindsCount;
half _BlindsDirection;
half _DiamondDissolveAmount;
half _DiamondDissolveScale;
half _ScanlineDissolveAmount;
half _ScanlineDissolveDensity;
half _SpreadFadeAmount;
half4 _SpreadFadeOrigin;
half _SpreadFadeSmoothness;
half _PixelDissolveAmount;
half _PixelDissolveGrid;
half _SpiralWipeAmount;
half _SpiralWipeSmoothness;
half _SpiralWipeArms;
half _HexDissolveAmount;
half _HexDissolveScale;
half _DissolveNoiseSpeed;
half _Blinds_En;
half _DiamondDissolve_En;
half _DirFade_En;
half _DirGlowFade_En;
half _Dissolve_En;
half _FullAlphaDissolve_En;
half _Halftone_En;
half _HexDissolve_En;
half _PixelDissolve_En;
half _RadialWipe_En;
half _ScanlineDissolve_En;
half _SpiralWipe_En;
half _SpreadFade_En;
half _SrcDissolve_En;

// ---- NEW: Part 2 ----
half _BurnDirection_En; half _BurnDirAngle; half _BurnDirAmount; half _BurnDirEdgeWidth;
half4 _BurnDirFireColor; half4 _BurnDirCharcoalColor; half _BurnDirCharcoalWidth; half _BurnDirNoiseScale; half _BurnDirMaskCh;
#endif

#ifdef _FX_VFX
half _HologramMaskCh;
half _GlitchMaskCh;
half _FlickerMaskCh;
half _ShineMaskCh;
half _SmokeMaskCh;
half _FlameMaskCh;
half _ShiftingMaskCh;
half _ShiftRainbowMaskCh;
half _DialogueEmphasis_En;
half _DialogueEmphasisMaskCh;
half _OverlayMaskCh;
half _HologramStripesAmount;
half _HologramMinAlpha;
half _HologramStripesSpeed;
half4 _HologramColor;
half _HologramBlend;
half _HologramDistortion;
half _GlitchAmount;
half _GlitchSpeed;
half _GlitchSize;
half _GlitchColorShift;
half _FlickerFreq;
half _FlickerPercent;
half4 _ShineColor;
half _ShineWidth;
half _ShineSpeed;
half _ShineDelay;
half _ShineRotation;
half _ShineGlow;
half4 _SmokeColor;
half _SmokeDensity;
half _SmokeSpeed;
half _SmokeFade;
half _SmokeTurbulence;
half _SmokeColorStr;
half4 _SmokeDirection;
half4 _FlameColor1;
half4 _FlameColor2;
half _FlameSpeed;
half _FlameDensity;
half _FlameIntensity;
half _FlameOnlyMode;
half _FlameExtendSides;
half _FlameSideFalloff;
half4 _ShiftingColor1;
half4 _ShiftingColor2;
half _ShiftingSpeed;
half _ShiftingFade;
half _ShiftRainbowSpeed;
half _ShiftRainbowDensity;
half _ShiftRainbowFade;
half4 _OverlayColor;
half _OverlayBlend;
half4 _OverlaySpeed;
half4 _OverlayTiling;
half _Flame_En;
half _Flicker_En;
half _Glitch_En;
half _Hologram_En;
half _MotionSmear_En;
half _Overlay_En;
half _Shifting_En;
half _ShiftRainbow_En;
half _Shine_En;
half _Smoke_En;
half _VoronoiCrackle_En;

// ---- NEW: Part 2 ----
half _EnchantedColorVariation; half _EnchantedSizeMix; half _EnchantedShimmer;
half _GlobalSmoke_En; half4 _GlobalSmokeColor; half _GlobalSmokeDensity; half _GlobalSmokeSpeed; half _GlobalSmokeOpacity; half4 _GlobalSmokeDir; half _GlobalSmokeMaskCh; half _GlobalSmokeOverlayOnly;
half _HoloCard_En; half _HoloCardDensity; half _HoloCardSpeed; half _HoloCardSat; half _HoloCardBright;
half _HoloCardFPow; half _HoloCardFShift; half _HoloCardScanDen; half _HoloCardScanSpd; half _HoloCardScanInt; half _HoloCardFade; half _HoloCardMaskCh;
half _CloudShadows_En; half _CloudShadowScale; half4 _CloudShadowSpeed; half _CloudShadowDark; half _CloudShadowThresh; half _CloudShadowSoft; half _CloudShadowFade; half _CloudShadowMaskCh; half _CloudShadowOverlayOnly;
half _TrailA_En; half4 _TrailAVel; half4 _TrailASlowCol; half4 _TrailAFastCol; half _TrailAMaxSpd; half _TrailACurve; half _TrailAInt; half _TrailAMaskCh;
half _TrailB_En; half4 _TrailBVel; half4 _TrailBCol; half _TrailBMaxSpd; half _TrailBCurve; half _TrailBInt; half _TrailBSegs; half _TrailBFade; half _TrailBMaskCh;
half _TrailC_En; half4 _TrailCVel; half4 _TrailCColA; half4 _TrailCColB; half _TrailCMaxSpd; half _TrailCCurve; half _TrailCInt; half _TrailCRbSpd; half _TrailCMaskCh;
half _Charging_En; half4 _ChargeColor; half4 _ChargeCenter; half _ChargeRadius; half _ChargeWidth;
half _ChargeSpeed; half _ChargeFreq; half _ChargeDensity; half _ChargeInt; half _ChargeLines; half _ChargeFade; half _ChargeMaskCh;
#endif

#ifdef _FX_GAME
half _CheckerboardMaskCh;
half _UVVisualizeMaskCh;
half _UVCheckerMaskCh;
half _UVNumericMaskCh;
half _HealthTintMaskCh;
half _XRaySilhouetteMaskCh;
half _MinimapPingMaskCh;
half _GhostingMaskCh;
half2 _MinimapPingOffset;
half _UVVisualizeBlend;
half _UVCheckerScale;
half _UVCheckerBlend;
half _UVNumericScale;
half _UVNumericBlend;
half4 _HealthTintLowColor;
half4 _HealthTintHighColor;
half _HealthTintAmount;
half4 _XRaySilhouetteColor;
half _XRaySilhouetteBlend;
half4 _MinimapPingColor;
half _MinimapPingSpeed;
half _MinimapPingWidth;
half _GhostingCount;
half4 _GhostingOffset;
half _GhostingFade;
half _RetroPaletteSize;
half _Checkerboard_En;
half _Ghosting_En;
half _HealthTint_En;
half _MinimapPing_En;
half _RetroPalette_En;
half _UVChecker_En;
half _UVNumeric_En;
half _UVVisualize_En;
half _XRaySilhouette_En;

// ---- NEW: Part 2 ----
half _Blueprint_En; half4 _BlueprintBgCol; half4 _BlueprintLineCol; half4 _BlueprintGridCol;
half _BlueprintEdgeSz; half _BlueprintLineWidth; half _BlueprintLineInt; half _BlueprintGridDen; half _BlueprintGridOp; half _BlueprintFade; half _BlueprintMaskCh;
#endif

CBUFFER_END

#endif // !AIO_SRP
#endif // ALLINONE_PROPERTIES_INCLUDED