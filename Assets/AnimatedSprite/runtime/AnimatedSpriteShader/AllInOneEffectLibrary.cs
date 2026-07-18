using System.Collections.Generic;

namespace FlexiSprite
{

    public struct EffectDefinition
    {
        public string Keyword;
        public string Body;
    }

    /// <summary>
    /// Central repository of all available shader effects.
    /// The shader generator iterates through this list to build the final shader dynamically.
    /// </summary>
    public static class AllInOneEffectLibrary
    {
        // Holds all effect HLSL snippets. Order here doesn't dictate render order 
        // (that's handled by the Master Setup window), just definition order.
        public static readonly List<EffectDefinition> Effects = new List<EffectDefinition>
        {
            new EffectDefinition {
                Keyword = "_Pixelate_En",
                Body = "half pxSize = _PixelateSize;\n"
                + "        uv = floor(uv*pxSize+0.5)/pxSize;\n"
                + "    return uv;"
            },
            new EffectDefinition {
                Keyword = "_Wave_En",
                Body = "uv.x += sin(uv.y*_WaveFrequency+_WaveSpeed * time)*_WaveAmplitude;\n"
                + "        uv.y += cos(uv.x*_WaveFrequency+_WaveSpeed * time)*_WaveAmplitude;\n"
                + "    return uv;"
            },
            new EffectDefinition {
                Keyword = "_Twist_En",
                Body = "float2 center = AtlasCenter() + _TwistOffset.xy;\n"
                + "        float2 delta = uv-center;\n"
                + "        half dist = length(delta);\n"
                + "        half falloff = saturate(1-dist/_TwistRadius);\n"
                + "        half angle = _TwistAmount * falloff + _TwistSpeed * time * 0.1 * falloff;\n"
                + "        half s,c; sincos(angle,s,c);\n"
                + "        uv = center+float2(delta.x*c-delta.y*s,delta.x*s+delta.y*c);\n"
                + "    return uv;"
            },
            new EffectDefinition {
                Keyword = "_UVDistort_En",
                Body = "half2 distortUV = uv * _DistortTiling.xy + _DistortTiling.zw + _DistortSpeed * time * 0.1;\n"
                + "        half4 rawTex = STEX(_DistortTex,sampler_DistortTex,distortUV);\n"
                + "        half2 distort = _DistortIsNormalMap > 0.5 ? (half2(rawTex.a, rawTex.g) * 2.0 - 1.0) : (rawTex.rg - 0.5);\n"
                + "        uv += distort*_DistortAmount;\n"
                + "    return uv;"
            },
            new EffectDefinition {
                Keyword = "_FullDistort_En",
                Body = "half2 fd = half2(\n"
                + "            ValueNoise(uv*_FullDistortDensity+half2(_FullDistortSpeed * time,0)),\n"
                + "            ValueNoise(uv*_FullDistortDensity+half2(0,_FullDistortSpeed * time)+100)\n"
                + "        ) - 0.5;\n"
                + "        uv += fd*_FullDistortAmount;\n"
                + "    return uv;"
            },
            new EffectDefinition {
                Keyword = "_DirDistort_En",
                Body = "half2 ddir = half2(cos(_DirDistortAngle),sin(_DirDistortAngle));\n"
                + "        uv += ddir*sin(dot(uv,ddir)*20+_DirDistortSpeed * time)*_DirDistortAmount;\n"
                + "    return uv;"
            },
            new EffectDefinition {
                Keyword = "_UVScroll_En",
                Body = "float2 atlasSize = float2(_MaxXUV - _MinXUV, _MaxYUV - _MinYUV);\n"
                + "        float2 atlasMin = float2(_MinXUV, _MinYUV);\n"
                + "        float2 local = (uv - atlasMin) / max(atlasSize, 0.0001);\n"
                + "        local += half2(_UVScrollSpeedX * time, _UVScrollSpeedY * time);\n"
                + "        local = frac(local);\n"
                + "        uv = local * atlasSize + atlasMin;\n"
                + "    return uv;"
            },
            new EffectDefinition {
                Keyword = "_Rotate_En",
                Body = "float2 rc=AtlasCenter(); float2 rd=uv-rc;\n"
                + "        half ra=_RotateAmount+_RotateSpeed * time; half rs,rcos; sincos(ra,rs,rcos);\n"
                + "        uv = rc+float2(rd.x*rcos-rd.y*rs,rd.x*rs+rd.y*rcos);\n"
                + "    return uv;"
            },
            new EffectDefinition {
                Keyword = "_Squeeze_En",
                Body = "float2 ac=AtlasCenter(); float2 sc=uv-ac;\n"
                + "        sc.x *= 1+sin(_SqueezeSpeed * time)*_SqueezeAmount.x*_SqueezePower;\n"
                + "        sc.y *= 1+cos(_SqueezeSpeed * time)*_SqueezeAmount.y*_SqueezePower;\n"
                + "        uv = sc+ac;\n"
                + "    return uv;"
            },
            new EffectDefinition {
                Keyword = "_Squish_En",
                Body = "float2 ac=AtlasCenter(); float2 sq=uv-ac;\n"
                + "        half st=sin(_SquishSpeed * time)*_SquishPower;\n"
                + "        sq.x *= 1+st*_SquishAmount.x; sq.y *= 1-st*_SquishAmount.y;\n"
                + "        uv = sq+ac;\n"
                + "    return uv;"
            },
            new EffectDefinition {
                Keyword = "_Wiggle_En",
                Body = "uv.x += sin(uv.y*_WiggleFrequency+_WiggleSpeed * time)*_WiggleAmount;\n"
                + "    return uv;"
            },
            new EffectDefinition {
                Keyword = "_Wind_En",
                Body = "half windGradient = saturate((uv.y - _WindBottomY) / max(1.0 - _WindBottomY, 0.001));\n"
                + "        uv.x += sin(uv.y*_WindFrequency+_WindSpeed * time)*_WindAmount*windGradient;\n"
                + "    return uv;"
            },
            new EffectDefinition {
                Keyword = "_WorldTiling_En",
                Body = "float s = sin(_WorldTileRotation);\n"
                + "            float c = cos(_WorldTileRotation);\n"
                + "            float2 wp = worldPos.xy + _WorldTileOffset.xy;\n"
                + "            float2 rwp = float2(wp.x * c - wp.y * s, wp.x * s + wp.y * c);\n"
                + "            float2 tuv = frac(rwp * _WorldTileScale);\n"
                + "            // Map tiled UV back into this sprite's atlas rect so we never sample neighbors\n"
                + "            uv = float2(lerp(_MinXUV, _MaxXUV, tuv.x), lerp(_MinYUV, _MaxYUV, tuv.y));\n"
                + "    return uv;"
            },
            new EffectDefinition {
                Keyword = "_UVScaleOffset_En",
                Body = "float2 atlasMin = float2(_MinXUV, _MinYUV);\n"
                + "        float2 atlasMax = (_MaxXUV <= 0.001) ? float2(1.0, 1.0) : float2(_MaxXUV, _MaxYUV);\n"
                + "        float2 atlasSize = max(atlasMax - atlasMin, float2(0.0001, 0.0001));\n"
                + "        float2 local = (uv - atlasMin) / atlasSize;\n"
                + "        float2 scale = _UVScaleOffset.xy;\n"
                + "        if (abs(scale.x) <= 0.0001 || abs(scale.y) <= 0.0001) return float2(-99999.0, -99999.0);\n"
                + "        float2 sourceLocal = (local - 0.5 - _UVScaleOffset.zw) / scale + 0.5;\n"
                + "        uv = atlasMin + sourceLocal * atlasSize;\n"
                + "    return uv;"
            },
            new EffectDefinition {
                Keyword = "_CurlNoise_En",
                Body = "float2 curlOff = CurlNoise(uv * _CurlNoiseDensity + _CurlNoiseSpeed * time * 0.1);\n"
                + "        uv += curlOff * _CurlNoiseAmount;\n"
                + "    return uv;"
            },
            new EffectDefinition {
                Keyword = "_FBMNoise_En",
                Body = "half2 fbmOff = half2(\n"
                + "            FBMNoise(uv * _FBMNoiseDensity + half2(_FBMNoiseSpeed * time, 0), (int)_FBMNoiseOctaves),\n"
                + "            FBMNoise(uv * _FBMNoiseDensity + half2(0, _FBMNoiseSpeed * time) + 50, (int)_FBMNoiseOctaves)\n"
                + "        ) - 0.5;\n"
                + "        uv += fbmOff * _FBMNoiseAmount;\n"
                + "    return uv;"
            },
            new EffectDefinition {
                Keyword = "_Shockwave_En",
                Body = "float2 swDelta = uv - _ShockwaveCenter.xy;\n"
                 + " swDelta.x *= _SpriteAspect;\n"
                + "        swDelta.x *= _SpriteAspect; half swDist = length(swDelta);\n"
                + "        // Progress 0-1 drives radius from 0 to max reach\n"
                + "        half maxRadius = 1.0;\n"
                + "        half radius = _ShockwaveRadius * maxRadius;\n"
                + "        half swRing = 1 - saturate(abs(swDist - radius) / _ShockwaveWidth);\n"
                + "        uv += normalize(swDelta + 0.0001) * swRing * _ShockwaveAmplitude;\n"
                + "    return uv;"
            },
            new EffectDefinition {
                Keyword = "_SineScale_En",
                Body = "float2 ac=AtlasCenter(); float2 ssc = uv - ac;\n"
                + "        ssc.x *= 1 + sin(_SineScaleSpeed * time) * _SineScaleAmountX;\n"
                + "        ssc.y *= 1 + sin(_SineScaleSpeed * time * 1.1) * _SineScaleAmountY;\n"
                + "        uv = ssc + ac;\n"
                + "    return uv;"
            },
            new EffectDefinition {
                Keyword = "_Heartbeat_En",
                Body = "float2 ac=AtlasCenter(); float2 hbc = uv - ac;\n"
                + "        half hbPulse = pow(abs(sin(_HeartbeatSpeed * time)), 8);\n"
                + "        hbc *= 1 + hbPulse * _HeartbeatAmount;\n"
                + "        uv = hbc + ac;\n"
                + "    return uv;"
            },
            new EffectDefinition {
                Keyword = "_Breathing_En",
                Body = "float2 ac=AtlasCenter(); float2 brc = uv - ac;\n"
                + "        brc.y *= 1 + sin(_BreathingSpeed * time) * _BreathingAmount;\n"
                + "        uv = brc + ac;\n"
                + "    return uv;"
            },
            new EffectDefinition {
                Keyword = "_LiquidWobble_En",
                Body = "float2 ac=AtlasCenter(); float2 hs=AtlasHalfSize();\n"
                + "        half yNorm = abs(uv.y - ac.y) / max(hs.y, 0.001);\n"
                + "        uv.x += sin(uv.y * _LiquidWobbleFreq + _LiquidWobbleSpeed * time) * _LiquidWobbleAmount * (1 - saturate(yNorm));\n"
                + "        uv.y += cos(uv.x * _LiquidWobbleFreq * 0.7 + _LiquidWobbleSpeed * time * 0.8) * _LiquidWobbleAmount * 0.5;\n"
                + "    return uv;"
            },
            new EffectDefinition {
                Keyword = "_SmoothPixelArt_En",
                Body = "float2 spUV = uv / _SmoothPixelArtTexelSize;\n"
                + "        float2 spFl = floor(spUV + 0.5);\n"
                + "        float2 spFr = spUV - spFl;\n"
                + "        spFr = clamp(spFr / fwidth(spUV), -0.5, 0.5);\n"
                + "        uv = (spFl + spFr) * _SmoothPixelArtTexelSize;\n"
                + "    return uv;"
            },
            new EffectDefinition {
                Keyword = "_UVBendPoint_En",
                Body = "float2 bc = _BendUseWorldSpace>0.5 ? (worldPos.xy-_BendWorldPoint.xy)*_BendWorldScale+0.5 : _BendLocalPoint.xy;\n"
                + "    float2 delta = uv-bc; half dist = length(delta);\n"
                + "    half fo = 1.0-saturate(dist/_BendRadius); fo = pow(fo,_BendFalloff);\n"
                + "    float2 dir = normalize(delta+0.0001);\n"
                + "    uv += dir*fo*_BendAmount*(_BendInvert>0.5?1.0:-1.0);\n"
                + "    return uv;"
            },
            new EffectDefinition {
                Keyword = "_Sharpen_En",
                Body = "half4 avg = (SampleMainClean(uv+half2(_SharpenSize,0))\n"
                + "                    +SampleMainClean(uv-half2(_SharpenSize,0))\n"
                + "                    +SampleMainClean(uv+half2(0,_SharpenSize))\n"
                + "                    +SampleMainClean(uv-half2(0,_SharpenSize)))*0.25;\n"
                + "        col.rgb += (col.rgb-avg.rgb)*_SharpenAmount;"
            },
            new EffectDefinition {
                Keyword = "_GaussBlur_En",
                Body = "half4 blur = col;\n"
                + "        half gs = _GaussBlurSize;\n"
                + "        half q = lerp(3,7,_GaussBlurSamples);\n"
                + "        half total = 1;\n"
                + "        for(half i=1;i<=q;i++){half w=1-i/q; blur+=SampleMainClean(uv+half2(gs*i,0))*w; blur+=SampleMainClean(uv-half2(gs*i,0))*w; blur+=SampleMainClean(uv+half2(0,gs*i))*w; blur+=SampleMainClean(uv-half2(0,gs*i))*w; total+=w*4;}\n"
                + "        col = blur/total;"
            },
            new EffectDefinition {
                Keyword = "_DirBlur_En",
                Body = "half2 bdir = half2(cos(_DirBlurAngle),sin(_DirBlurAngle))*_DirBlurSize;\n"
                + "        half4 dblur = col; half dtotal = 1;\n"
                + "        for(half i=1;i<=_DirBlurSamples;i++){half w=1-i/_DirBlurSamples; dblur+=SampleMainClean(uv+bdir*i)*w; dblur+=SampleMainClean(uv-bdir*i)*w; dtotal+=w*2;}\n"
                + "        col = dblur/dtotal;"
            },
            new EffectDefinition {
                Keyword = "_FlipBookBlend_En",
                Body = "half4 nextFrame = STEX(_FlipBookNextFrame, sampler_FlipBookNextFrame, uv);\n"
                + "        if (nextFrame.a > 0.01)\n"
                + "            col = lerp(col, nextFrame, _FlipBookLerp);\n"
                + "        else\n"
                + "            col = lerp(col, float4(col.xyz, 0.0), _FlipBookLerp);"
            },
            new EffectDefinition {
                Keyword = "_UVScrollOverlay_En",
                Body = "{ float2 overlayUV = uv * _UVScrollOverlayTiling.xy + _UVScrollOverlayTiling.zw;\n"
                + "          overlayUV += half2(_UVScrollOverlaySpeedX * time, _UVScrollOverlaySpeedY * time);\n"
                + "          half4 overlay = STEX(_UVScrollOverlayTex, sampler_UVScrollOverlayTex, overlayUV) * _UVScrollOverlayTint;\n"
                + "          col.rgb = lerp(col.rgb, col.rgb * overlay.rgb, _UVScrollOverlayBlend * overlay.a * col.a); }"
            },
            new EffectDefinition {
                Keyword = "_StrongTint_En",
                Body = "half m = _StrongTintFade * eMask * SampleEffectMask(uv,masks,_StrongTintMaskCh);\n"
                + "          col.rgb = lerp(col.rgb, _StrongTintColor.rgb, m * col.a);"
            },
            new EffectDefinition {
                Keyword = "_AddColor_En",
                Body = "half m = _AddColorFade * eMask * SampleEffectMask(uv,masks,_AddColorMaskCh);\n"
                + "          col.rgb += _AddColorAmount.rgb * m * col.a;"
            },
            new EffectDefinition {
                Keyword = "_AlphaTint_En",
                Body = "half m = _AlphaTintFade * eMask * SampleEffectMask(uv,masks,_AlphaTintMaskCh);\n"
                + "          col.rgb = lerp(col.rgb, _AlphaTintColor.rgb * col.a, m);"
            },
            new EffectDefinition {
                Keyword = "_HitEffect_En",
                Body = "half m = _HitEffectBlend * eMask * SampleEffectMask(uv,masks,_HitEffectMaskCh);\n"
                + "          col.rgb = lerp(col.rgb, _HitEffectColor.rgb, m * col.a);"
            },
            new EffectDefinition {
                Keyword = "_Negative_En",
                Body = "half m = _NegativeAmount * eMask * SampleEffectMask(uv,masks,_NegativeMaskCh);\n"
                + "          col.rgb = lerp(col.rgb, 1-col.rgb, m * col.a);"
            },
            new EffectDefinition {
                Keyword = "_Greyscale_En",
                Body = "half m = _GreyscaleBlend * eMask * SampleEffectMask(uv,masks,_GreyscaleMaskCh);\n"
                + "          half g = dot(col.rgb, _GreyscaleLuminosity.rgb);\n"
                + "          col.rgb = lerp(col.rgb, half3(g,g,g), m * col.a);"
            },
            new EffectDefinition {
                Keyword = "_Saturation_En",
                Body = "half m = eMask * SampleEffectMask(uv,masks,_SaturationMaskCh);\n"
                + "          half g = dot(col.rgb, half3(0.299,0.587,0.114));\n"
                + "          col.rgb = lerp(half3(g,g,g), col.rgb, lerp(1,_SaturationAmount,m));"
            },
            new EffectDefinition {
                Keyword = "_Brightness_En",
                Body = "half m = eMask * SampleEffectMask(uv,masks,_BrightnessMaskCh);\n"
                + "          col.rgb += _BrightnessAmount * m;"
            },
            new EffectDefinition {
                Keyword = "_Contrast_En",
                Body = "half m = eMask * SampleEffectMask(uv,masks,_ContrastMaskCh);\n"
                + "          col.rgb = lerp(col.rgb, (col.rgb-0.5)*_ContrastAmount+0.5, m);"
            },
            new EffectDefinition {
                Keyword = "_Sepia_En",
                Body = "half m = _SepiaIntensity * eMask * SampleEffectMask(uv,masks,_SepiaMaskCh);\n"
                + "          half lum = dot(col.rgb, half3(0.299,0.587,0.114));\n"
                + "          col.rgb = lerp(col.rgb, lum * _SepiaColor.rgb + lum * 0.2, m * col.a);"
            },
            new EffectDefinition {
                Keyword = "_ColorGrading_En",
                Body = "half m = eMask * SampleEffectMask(uv,masks,_ColorGradingMaskCh);\n"
                + "          half3 graded = col.rgb * pow(2, _CGExposure);\n"
                + "          graded = pow(max(graded, 0.001), 1.0 / _CGGamma);\n"
                + "          graded += _CGLift.rgb;\n"
                + "          col.rgb = lerp(col.rgb, graded, m);"
            },
            new EffectDefinition {
                Keyword = "_PingPongGlow_En",
                Body = "half m = eMask * SampleEffectMask(uv,masks,_PingPongGlowMaskCh);\n"
                + "          half pp = lerp(_PingPongGlowMin, _PingPongGlowMax, sin(_Time.y * _PingPongGlowSpeed) * 0.5 + 0.5);\n"
                + "          col.rgb += _PingPongGlowColor.rgb * pp * m * col.a;"
            },
            new EffectDefinition {
                Keyword = "_ColorCycle_En",
                Body = "half m = _ColorCycleFade * eMask * SampleEffectMask(uv,masks,_ColorCycleMaskCh);\n"
                + "          half t4 = frac(_ColorCycleSpeed * time * 0.25) * 4;\n"
                + "          half3 cyc;\n"
                + "          if(t4 < 1) cyc = lerp(_ColorCycleA.rgb, _ColorCycleB.rgb, t4);\n"
                + "          else if(t4 < 2) cyc = lerp(_ColorCycleB.rgb, _ColorCycleC.rgb, t4-1);\n"
                + "          else if(t4 < 3) cyc = lerp(_ColorCycleC.rgb, _ColorCycleD.rgb, t4-2);\n"
                + "          else cyc = lerp(_ColorCycleD.rgb, _ColorCycleA.rgb, t4-3);\n"
                + "          half lum = dot(col.rgb, half3(0.299,0.587,0.114));\n"
                + "          col.rgb = lerp(col.rgb, cyc * lum, m * col.a);"
            },
            new EffectDefinition {
                Keyword = "_Bloom_En",
                Body = "half m = eMask * SampleEffectMask(uv,masks,_BloomMaskCh);\n"
                + "          half lum = dot(col.rgb, half3(0.299,0.587,0.114));\n"
                + "          half4 blur = half4(0,0,0,0); half bTotal = 0;\n"
                + "          half radius = _BloomSize * _BloomSamples;\n"
                + "          for(int bi=-_BloomSamples/2.0;bi<=_BloomSamples/2.0;bi++) for(int bj=-_BloomSamples/2.0;bj<=_BloomSamples/2.0;bj++) {\n"
                + "              half bw = 1.0/(1+abs(bi)+abs(bj));\n"
                + "              half4 bSamp = SampleMainClean(uv+half2(bi,bj)*radius);\n"
                + "              blur += bSamp * bSamp.a * bw;\n"
                + "              bTotal += bw * bSamp.a; }\n"
                + "          blur /= max(bTotal, 0.001);\n"
                + "          half bright = 1;\n"
                + "          if(_BloomMode < 0.5) {\n"
                + "              half knee = _BloomThreshold * _BloomSoftKnee;\n"
                + "              half soft = lum - _BloomThreshold + knee;\n"
                + "              soft = clamp(soft, 0, 2*knee);\n"
                + "              soft = soft*soft / (4*knee+0.0001);\n"
                + "              bright = max(soft, lum-_BloomThreshold) / max(lum, 0.0001);\n"
                + "          } else if(_BloomMode < 1.5) {\n"
                + "              half cdist = ColorDist(col.rgb, _BloomMaskColor.rgb);\n"
                + "              bright = 1-AIO_SmoothStep(_BloomMaskRange-_BloomMaskSoft, _BloomMaskRange+_BloomMaskSoft+0.001, cdist);\n"
                + "          } else if(_BloomMode < 2.5) {\n"
                + "              half knee = _BloomThreshold * _BloomSoftKnee;\n"
                + "              half soft = lum - _BloomThreshold + knee;\n"
                + "              soft = clamp(soft, 0, 2*knee);\n"
                + "              soft = soft*soft / (4*knee+0.0001);\n"
                + "              half lumBright = max(soft, lum-_BloomThreshold) / max(lum, 0.0001);\n"
                + "              half cdist = ColorDist(col.rgb, _BloomMaskColor.rgb);\n"
                + "              half colorW = 1-AIO_SmoothStep(_BloomMaskRange-_BloomMaskSoft, _BloomMaskRange+_BloomMaskSoft+0.001, cdist);\n"
                + "              bright = max(lumBright, colorW);\n"
                + "          }\n"
                + "          half3 bloomResult = blur.rgb * bright * _BloomColor.rgb * _BloomIntensity;\n"
                + "          col.rgb += bloomResult * m;\n"
                + "          // Allow bloom to show into fully transparent pixels by raising alpha from bloom energy.\n"
                + "          if (_BloomAlphaBleed > 0.001)\n"
                + "          {\n"
                + "              half bloomA = saturate(max(bloomResult.r, max(bloomResult.g, bloomResult.b)));\n"
                + "              col.a = max(col.a, bloomA * _BloomAlphaBleed * m);\n"
                + "          }"
            },
            new EffectDefinition {
                Keyword = "_ColorReplace_En",
                Body = "half m = eMask * SampleEffectMask(uv,masks,_ColorReplaceMaskCh);\n"
                + "          half fade = _ColorReplaceFade * m;\n"
                + "          half rng = max(_ColorReplaceRange, 0.001);\n"
                + "          // Euclidean distance with smooth falloff for clean edges\n"
                + "          half d1 = length(col.rgb - _ColorReplaceSrc1.rgb);\n"
                + "          half r1 = 1.0 - AIO_SmoothStep(rng * 0.5, rng, d1);\n"
                + "          col.rgb = lerp(col.rgb, _ColorReplaceDst1.rgb, r1*fade);\n"
                + "          if(_ColorReplaceCount > 1.5) {\n"
                + "              half d2 = length(col.rgb - _ColorReplaceSrc2.rgb);\n"
                + "              half r2 = 1.0 - AIO_SmoothStep(rng * 0.5, rng, d2);\n"
                + "              col.rgb = lerp(col.rgb, _ColorReplaceDst2.rgb, r2*fade); }\n"
                + "          if(_ColorReplaceCount > 2.5) {\n"
                + "              half d3 = length(col.rgb - _ColorReplaceSrc3.rgb);\n"
                + "              half r3 = 1.0 - AIO_SmoothStep(rng * 0.5, rng, d3);\n"
                + "              col.rgb = lerp(col.rgb, _ColorReplaceDst3.rgb, r3*fade); }\n"
                + "          if(_ColorReplaceCount > 3.5) {\n"
                + "              half d4 = length(col.rgb - _ColorReplaceSrc4.rgb);\n"
                + "              half r4 = 1.0 - AIO_SmoothStep(rng * 0.5, rng, d4);\n"
                + "              col.rgb = lerp(col.rgb, _ColorReplaceDst4.rgb, r4*fade); }"
            },
            new EffectDefinition {
                Keyword = "_LUTReplace_En",
                Body = "half m = _LUTReplaceFade * eMask * SampleEffectMask(uv,masks,_LUTReplaceMaskCh);\n"
                + "          half lutN = max(_LUTSize, 2);\n"
                + "          half b_coord = col.b * (lutN - 1);\n"
                + "          half bFloor = floor(b_coord); half bFrac = b_coord - bFloor;\n"
                + "          half cells = lutN;\n"
                + "          half2 lutUV1 = half2((bFloor*lutN + col.r*(lutN-1))/(cells*lutN), col.g*(lutN-1)/lutN);\n"
                + "          half2 lutUV2 = half2(((bFloor+1)*lutN + col.r*(lutN-1))/(cells*lutN), col.g*(lutN-1)/lutN);\n"
                + "          half3 lut1 = STEX(_LUTReplaceTex, sampler_LUTReplaceTex, lutUV1).rgb;\n"
                + "          half3 lut2 = STEX(_LUTReplaceTex, sampler_LUTReplaceTex, lutUV2).rgb;\n"
                + "          col.rgb = lerp(col.rgb, lerp(lut1, lut2, bFrac), m * col.a);"
            },
            new EffectDefinition {
                Keyword = "_HueShift_En",
                Body = "half m = _HueShiftFade * eMask * SampleEffectMask(uv,masks,_HueShiftMaskCh);\n"
                + "          half3 hsv = RGB2HSV(col.rgb);\n"
                + "          hsv.x = frac(hsv.x + _HueShiftAmount + _HueShiftSpeed * time);\n"
                + "          half3 shifted = HSV2RGB(hsv);\n"
                + "          col.rgb = lerp(col.rgb, shifted, m);"
            },
            new EffectDefinition {
                Keyword = "_AddHue_En",
                Body = "half m = _AddHueFade * eMask * SampleEffectMask(uv,masks,_AddHueMaskCh);\n"
                + "          half hue = frac(_AddHueAmount + _AddHueSpeed * time);\n"
                + "          half3 added = HSV2RGB(half3(hue, _AddHueSaturation, _AddHueBrightness));\n"
                + "          col.rgb = lerp(col.rgb, col.rgb+added, m);"
            },
            new EffectDefinition {
                Keyword = "_SplitToning_En",
                Body = "half m = _SplitFade * eMask * SampleEffectMask(uv,masks,_SplitToningMaskCh);\n"
                + "          half lum = dot(col.rgb, half3(0.299,0.587,0.114));\n"
                + "          half gradientPos = saturate(lum + _SplitBalance * 0.5);\n"
                + "          half3 toned = lerp(_SplitShadowColor.rgb, _SplitMidColor.rgb, saturate(gradientPos * 2));\n"
                + "          toned = lerp(toned, _SplitHighlightColor.rgb, saturate(gradientPos * 2 - 1));\n"
                + "          col.rgb = lerp(col.rgb, toned * lum + col.rgb * (1-lum), m);"
            },
            new EffectDefinition {
                Keyword = "_Posterize_En",
                Body = "half m = _PosterizeFade * eMask * SampleEffectMask(uv,masks,_PosterizeMaskCh);\n"
                + "          half3 hsv = RGB2HSV(col.rgb);\n"
                + "          hsv.z = floor(hsv.z * _PosterizeSteps + 0.5) / _PosterizeSteps;\n"
                + "          hsv.y = floor(hsv.y * max(_PosterizeSteps * 0.5, 2) + 0.5) / max(_PosterizeSteps * 0.5, 2);\n"
                + "          half3 poster = HSV2RGB(hsv);\n"
                + "          col.rgb = lerp(col.rgb, poster, m * col.a);"
            },
            new EffectDefinition {
                Keyword = "_Chromatic_En",
                Body = "half m = eMask * SampleEffectMask(uv,masks,_ChromaticMaskCh);\n"
                + "          half2 dir;\n"
                + "          if(_ChromaMode < 0.5) {\n"
                + "              dir = half2(cos(_ChromaticAngle),sin(_ChromaticAngle))*_ChromaticAmount*m;\n"
                + "          } else {\n"
                + "              half2 delta = uv - _ChromaRadialCenter.xy;\n"
                + "              dir = delta * _ChromaticAmount * length(delta) * m;\n"
                + "          }\n"
                + "          half4 rSamp = SampleMainClean(uv+dir);\n"
                + "          half4 bSamp = SampleMainClean(uv-dir);\n"
                + "          col.r = lerp(col.r, rSamp.r, rSamp.a);\n"
                + "          col.b = lerp(col.b, bSamp.b, bSamp.a);"
            },
            new EffectDefinition {
                Keyword = "_Poison_En",
                Body = "half m = _PoisonFade * eMask * SampleEffectMask(uv,masks,_PoisonMaskCh);\n"
                + "          half n = ValueNoise(uv*_PoisonDensity+_PoisonSpeed * time);\n"
                + "          half3 poisonCol = lerp(_PoisonColor.rgb, _PoisonColor2.rgb, n);\n"
                + "          poisonCol = lerp(poisonCol, _PoisonColor3.rgb, ValueNoise(uv*_PoisonDensity*1.7+_PoisonSpeed * time*0.6+50));\n"
                + "          col.rgb = lerp(col.rgb, poisonCol*n, m * col.a);"
            },
            new EffectDefinition {
                Keyword = "_Frozen_En",
                Body = "half m = _FrozenFade * eMask * SampleEffectMask(uv, masks, _FrozenMaskCh);\n"
                + "            half lum = dot(col.rgb, half3(0.299, 0.587, 0.114));\n"
                + "            // Frost base: blend between two ice colors based on UV + luminance\n"
                + "            half iceT = saturate(lum * _FrozenContrast);\n"
                + "            half crystalPattern = frac(uv.x * 6 + uv.y * 6) * 0.3 + 0.7;\n"
                + "            half3 frostBase = lerp(_FrozenColor.rgb, _FrozenColor2.rgb, iceT * crystalPattern);\n"
                + "            // Highlights: edge sparkle controlled by Highlight param\n"
                + "            half edgeSparkle = pow(saturate(fwidth(frac(uv.x * 8 + uv.y * 6)) * 8), 2);\n"
                + "            half specular = pow(max(lum,0.0001), 3) * 0.5 + edgeSparkle * 0.5;\n"
                + "            half3 frozen = frostBase + _FrozenHighlightColor.rgb * specular * _FrozenHighlight;\n"
                + "            // Contrast: control how much original luminance structure shows through\n"
                + "            frozen = lerp(frostBase, frozen, saturate(lum * _FrozenContrast));\n"
                + "            col.rgb = lerp(col.rgb, frozen, m * col.a);"
            },
            new EffectDefinition {
                Keyword = "_Metal_En",
                Body = "half m = _MetalFade * eMask * SampleEffectMask(uv,masks,_MetalMaskCh);\n"
                + "          half lum = dot(col.rgb, half3(0.299,0.587,0.114));\n"
                + "          half shine = sin(uv.x*30+uv.y*20+_MetalSpeed * time)*0.5+0.5;\n"
                + "          half3 metalBase = lerp(_MetalColor.rgb, _MetalColor2.rgb, lum);\n"
                + "          half3 metal = metalBase * pow(max(lum,0.0001), 1/_MetalContrast) + _MetalHighlight.rgb*shine*pow(max(lum,0.0001),2);\n"
                + "          col.rgb = lerp(col.rgb, metal, m * col.a);"
            },
            new EffectDefinition {
                Keyword = "_Camouflage_En",
                Body = "half m = _CamoFade * eMask * SampleEffectMask(uv,masks,_CamouflageMaskCh);\n"
                + "          half n1=ValueNoise(uv*_CamoScale+_CamoSpeed * time);\n"
                + "          half n2=ValueNoise(uv*_CamoScale*1.5+_CamoSpeed * time*0.7+100);\n"
                + "          half3 camo = n1<0.33 ? _CamoColor1.rgb : (n1<0.66 ? _CamoColor2.rgb : _CamoColor3.rgb);\n"
                + "          camo = lerp(camo, n2<0.5?_CamoColor1.rgb:_CamoColor3.rgb, 0.3);\n"
                + "          col.rgb = lerp(col.rgb, camo, m * col.a);"
            },
            new EffectDefinition {
                Keyword = "_Rainbow_En",
                Body = "half m = _RainbowFade * eMask * SampleEffectMask(uv,masks,_RainbowMaskCh);\n"
                + "          half hue = frac(uv.x*_RainbowDensity + _RainbowSpeed * time);\n"
                + "          half3 rb = HSV2RGB(half3(hue, _RainbowSaturation, _RainbowBrightness));\n"
                + "          col.rgb = lerp(col.rgb, rb, m * col.a);"
            },
            new EffectDefinition {
                Keyword = "_BlackTint_En",
                Body = "half m = _BlackTintFade * eMask * SampleEffectMask(uv,masks,_BlackTintMaskCh);\n"
                + "          half lum = dot(col.rgb, half3(0.299,0.587,0.114));\n"
                + "          half w = saturate(1-lum/_BlackTintThreshold);\n"
                + "          col.rgb = lerp(col.rgb, _BlackTintColor.rgb, w*m);"
            },
            new EffectDefinition {
                Keyword = "_PaletteSwap_En",
                Body = "half m = _PaletteSwapFade * eMask * SampleEffectMask(uv,masks,_PaletteSwapMaskCh);\n"
                + "          half lum = dot(col.rgb, half3(0.299,0.587,0.114));\n"
                + "          half idx = floor(lum * (_PaletteSwapColors - 0.001)) / max(_PaletteSwapColors - 1, 1);\n"
                + "          half3 palCol = STEX(_PaletteSwapTex, sampler_PaletteSwapTex, half2(idx, 0.5)).rgb;\n"
                + "          col.rgb = lerp(col.rgb, palCol, m);"
            },
            new EffectDefinition {
                Keyword = "_Dithering_En",
                Body = "half m = _DitheringFade * eMask * SampleEffectMask(uv,masks,_DitheringMaskCh);\n"
                + "          float2 ditherCoord = _DitheringScreenSpace > 0.5 ? float2(GetScreenUV(screenPos).x * _ScreenParams.x/_ScreenParams.y, GetScreenUV(screenPos).y) : uv;\n"
                + "          float2 ditherPos = floor(fmod(ditherCoord * _DitheringSize * 64, 4));\n"
                + "          int idx = (int)ditherPos.x + (int)ditherPos.y * 4;\n"
                + "          half bayer = (idx==0?0:idx==1?8:idx==2?2:idx==3?10:idx==4?12:idx==5?4:idx==6?14:idx==7?6:idx==8?3:idx==9?11:idx==10?1:idx==11?9:idx==12?15:idx==13?7:idx==14?13:5) / 16.0;\n"
                + "          half lum = dot(col.rgb, half3(0.299,0.587,0.114));\n"
                + "          half quantized = floor(lum * _DitheringLevels + bayer) / _DitheringLevels;\n"
                + "          half3 dithered = col.rgb * (quantized / max(lum, 0.001));\n"
                + "          col.rgb = lerp(col.rgb, dithered, m);\n"
                + "          if(_DitheringAlphaMode > 0.5) {\n"
                + "              half alphaDither = step(bayer + 0.002, col.a * _DitheringAlphaThreshold);\n"
                + "              col.a *= lerp(1, alphaDither, m);\n"
                + "          }"
            },
            new EffectDefinition {
                Keyword = "_CRTScanlines_En",
                Body = "half m = eMask * SampleEffectMask(uv,masks,_CRTScanlinesMaskCh);\n"
                + "          float2 cuv = LocalUV(uv) - 0.5;\n"
                + "          cuv *= 1.0 + dot(cuv, cuv) * _CRTCurvature * 8.0;\n"
                + "          cuv += 0.5;\n"
                + "          half scanline = sin((cuv.y + _CRTSpeed * time * 0.01) * _CRTDensity * 3.14159) * 0.5 + 0.5;\n"
                + "          col.rgb *= lerp(1, 1 - scanline * _CRTIntensity, m);"
            },
            new EffectDefinition {
                Keyword = "_Vignette_En",
                Body = "half m = eMask * SampleEffectMask(uv,masks,_VignetteMaskCh);\n"
                + "          float2 vigLuv = LocalUV(uv); vigLuv.x *= _SpriteAspect; half2 vigC = _VignetteCenter.xy + _VignetteOffset.xy; vigC.x *= _SpriteAspect; half2 vig = vigLuv - vigC;\n"
                + "          half vigDist = length(vig);\n"
                + "          half outerEdge = 0.707;\n"
                + "          half innerEdge = outerEdge * (1.0 - _VignetteIntensity);\n"
                + "          half vigFactor = AIO_SmoothStep(innerEdge, innerEdge + _VignetteSmoothness * outerEdge, vigDist);\n"
                + "          col.rgb = lerp(col.rgb, _VignetteColor.rgb, vigFactor * m);\n"
                + "          col.a *= lerp(1.0, 1.0 - vigFactor * _VignetteAlpha, m);"
            },
            new EffectDefinition {
                Keyword = "_ThermalVision_En",
                Body = "half m = _ThermalIntensity * eMask * SampleEffectMask(uv,masks,_ThermalVisionMaskCh);\n"
                + "          half lum = dot(col.rgb, half3(0.299,0.587,0.114));\n"
                + "          half3 thermal = lerp(half3(0,0,0.2), half3(0.5,0,0.5), saturate(lum*4));\n"
                + "          thermal = lerp(thermal, half3(1,0.2,0), saturate(lum*4-1));\n"
                + "          thermal = lerp(thermal, half3(1,1,0), saturate(lum*4-2));\n"
                + "          thermal = lerp(thermal, half3(1,1,1), saturate(lum*4-3));\n"
                + "          // HDR glow: push bright areas beyond 1.0 for bloom post-processing\n"
                + "          thermal *= 1.0 + _ThermalHDRGlow * lum * 2.0;\n"
                + "          col.rgb = lerp(col.rgb, thermal, m * col.a);"
            },
            new EffectDefinition {
                Keyword = "_RecolorRGB_En",
                Body = "half m = _RecolorFade * eMask * SampleEffectMask(uv,masks,_RecolorRGBMaskCh);\n"
                + "          half3 recolored = col.r * _RecolorR.rgb + col.g * _RecolorG.rgb + col.b * _RecolorB.rgb;\n"
                + "          half3 cmy = (1 - col.r) * _RecolorCyan.rgb * 0.33 + (1 - col.g) * _RecolorMagenta.rgb * 0.33 + (1 - col.b) * _RecolorYellow.rgb * 0.33;\n"
                + "          col.rgb = lerp(col.rgb, recolored + cmy, m * col.a);"
            },
            new EffectDefinition {
                Keyword = "_WaterReflect_En",
                Body = "half m = _WaterReflectFade * eMask * SampleEffectMask(uv,masks,_WaterReflectMaskCh);\n"
                + "          half yStart = _WaterReflectYPoint;\n"
                + "          if(uv.y < yStart) {\n"
                + "              half reflectY = yStart + (yStart - uv.y);\n"
                + "              half2 reflUV = half2(uv.x, reflectY);\n"
                + "              half waveOff = sin(uv.x * _WaterReflectWaveFreq + _WaterReflectWaveSpeed * time) * _WaterReflectWaveAmp;\n"
                + "              reflUV.x += waveOff; reflUV.y += waveOff * 0.3;\n"
                + "              reflUV = ClampToAtlas(reflUV);\n"
                + "              half4 reflCol = SampleMainClean(reflUV);\n"
                + "              half depth = saturate((yStart - uv.y) / max(_WaterReflectDepth, 0.001));\n"
                + "              half rayFade = 1.0 - depth; rayFade *= rayFade;\n"
                + "              half3 waterTinted = lerp(reflCol.rgb, _WaterReflectTint.rgb, _WaterReflectTintAmount * depth);\n"
                + "              col.rgb = lerp(col.rgb, waterTinted, rayFade * m * reflCol.a * _WaterReflectOpacity);\n"
                + "              col.a = max(col.a, reflCol.a * rayFade * m * _WaterReflectOpacity);\n"
                + "          }"
            },
            new EffectDefinition {
                Keyword = "_Gradient_En",
                Body = "half m=_GradientFade*eMask*SampleEffectMask(uv,masks,_GradientMaskCh);\n"
                + "        half2 gd=half2(cos(_GradientAngle),sin(_GradientAngle));\n"
                + "        half gT=saturate(dot(LocalUV(uv)-0.5,gd)+0.5);\n"
                + "        half3 gradCol = lerp(_GradientColorA.rgb, _GradientColorB.rgb, gT);\n"
                + "        if (_GradientUseTex > 0.5) gradCol = STEX(_GradientTex, sampler_GradientTex, half2(gT, 0.5)).rgb;\n"
                + "        if (_GradientBlendMode < 0.5) col.rgb = lerp(col.rgb, col.rgb + gradCol, m * col.a);\n"
                + "        else col.rgb = lerp(col.rgb, col.rgb * gradCol, m * col.a);"
            },
            new EffectDefinition {
                Keyword = "_RadialGradient_En",
                Body = "half m=_RadialGradFade*eMask*SampleEffectMask(uv,masks,_RadialGradMaskCh);\n"
                + "        float2 rgLuv=LocalUV(uv); rgLuv.x*=_SpriteAspect; float2 rgC=_RadialGradCenter.xy; rgC.x*=_SpriteAspect; half t=saturate(distance(rgLuv,rgC)/_RadialGradRadius);\n"
                + "        half3 gradCol=lerp(_RadialGradCenterColor.rgb,_RadialGradEdgeColor.rgb,t);\n"
                + "        if (_RadialGradUseTex > 0.5) gradCol = STEX(_RadialGradTex, sampler_RadialGradTex, half2(t, 0.5)).rgb;\n"
                + "        if (_RadialGradBlendMode < 0.5) col.rgb=lerp(col.rgb,col.rgb+gradCol,m*col.a);\n"
                + "        else col.rgb=lerp(col.rgb,col.rgb*gradCol,m*col.a);"
            },
            new EffectDefinition {
                Keyword = "_ColorRamp_En",
                Body = "half m=_ColorRampFade*eMask*SampleEffectMask(uv,masks,_ColorRampMaskCh);\n"
                + "        half3 rampColor;\n"
                + "        if (_ColorRampMode < 0.5) {\n"
                + "            rampColor.r = STEX(_ColorRampTex,sampler_ColorRampTex,half2(col.r,0.5)).r;\n"
                + "            rampColor.g = STEX(_ColorRampTex,sampler_ColorRampTex,half2(col.g,0.5)).g;\n"
                + "            rampColor.b = STEX(_ColorRampTex,sampler_ColorRampTex,half2(col.b,0.5)).b;\n"
                + "        } else if (_ColorRampMode < 1.5) {\n"
                + "            half lum=dot(col.rgb,half3(0.299,0.587,0.114));\n"
                + "            rampColor=STEX(_ColorRampTex,sampler_ColorRampTex,half2(lum,0.5)).rgb;\n"
                + "        } else {\n"
                + "            half lum=dot(col.rgb,half3(0.299,0.587,0.114));\n"
                + "            rampColor=lerp(_ColorRampColorA.rgb,_ColorRampColorB.rgb,lum);\n"
                + "        }\n"
                + "        rampColor*=_ColorRampIntensity;\n"
                + "        col.rgb=lerp(col.rgb,rampColor,m*col.a);"
            },
            new EffectDefinition {
                Keyword = "_Outline_En",
                Body = "half m = eMask * SampleEffectMask(uv, masks, _OutlineMaskCh);\n"
                + "\n"
                + "            half maxA = SampleOutlineAlpha(uv, _OutlineSize);\n"
                + "            half outline = MaskOutline(maxA, col.a) * _OutlineAlpha;\n"
                + "            half aa = _OutlineAA > 0.001 ? AIO_SmoothStep(0, _OutlineAA, outline) : outline;\n"
                + "            col = half4(lerp(col.rgb, _OutlineColor.rgb, aa * m), max(col.a, aa * m));"
            },
            new EffectDefinition {
                Keyword = "_InnerOutline_En",
                Body = "half m = eMask * SampleEffectMask(uv, masks, _InnerOutlineMaskCh);\n"
                + "            half inner = SampleInverseOutlineAlpha(uv, _InnerOutlineThickness) * _InnerOutlineAlpha;\n"
                + "            col.rgb = lerp(col.rgb, _InnerOutlineColor.rgb, inner * m * col.a);"
            },
            new EffectDefinition {
                Keyword = "_TexOutline_En",
                Body = "half m = eMask * SampleEffectMask(uv, masks, _TexOutlineMaskCh);\n"
                + "            half maxA = SampleOutlineAlpha(uv, _TexOutlineSize);\n"
                + "            half outline = MaskOutline(maxA, col.a);\n"
                + "            float2 texUV = uv * _TexOutlineTiling.xy + _TexOutlineTiling.zw + half2(_TexOutlineSpeed.x * time, _TexOutlineSpeed.y * time);\n"
                + "            half4 texCol = STEX(_TexOutlineTex, sampler_TexOutlineTex, texUV);\n"
                + "            half4 outCol = texCol * _TexOutlineColor;\n"
                + "            col = half4(lerp(col.rgb, outCol.rgb, outline * m), max(col.a, outline * m));"
            },
            new EffectDefinition {
                Keyword = "_RainbowOutline_En",
                Body = "half m = eMask * SampleEffectMask(uv, masks, _RainbowOutlineMaskCh);\n"
                + "            half maxA = SampleOutlineAlpha(uv, _RainbowOutlineSize);\n"
                + "            half outline = MaskOutline(maxA, col.a);\n"
                + "            half density = max(_RainbowOutlineDensity, 0.0001);\n"
                + "            half hue = frac((uv.x + uv.y) * density + _RainbowOutlineSpeed * time);\n"
                + "            half3 rb = HSV2RGB(half3(hue, 1, 1)) * _RainbowOutlineColor.rgb;\n"
                + "            col = half4(lerp(col.rgb, rb, outline * m), max(col.a, outline * m));"
            },
            new EffectDefinition {
                Keyword = "_DistortOutline_En",
                Body = "half m = eMask * SampleEffectMask(uv, masks, _DistortOutlineMaskCh);\n"
                + "            half actualSize = ((_DistortOutlineSize > 0.1) ? (_DistortOutlineSize * 0.015) : _DistortOutlineSize);\n"
                + "            half noise = sin(uv.y * 50 + _DistortOutlineSpeed * time) * _DistortOutlineAmount;\n"
                + "            half distortedA = SampleOutlineAlpha(uv + half2(noise, 0), actualSize);\n"
                + "            half normalA = SampleOutlineAlpha(uv, actualSize);\n"
                + "            half maxA = max(distortedA, normalA);\n"
                + "            half outline = MaskOutline(maxA, col.a);\n"
                + "            col = half4(lerp(col.rgb, _DistortOutlineColor.rgb, outline * m), max(col.a, outline * m));"
            },
            new EffectDefinition {
                Keyword = "_Glow_En",
                Body = "half m = eMask * SampleEffectMask(uv,masks,_GlowMaskCh);\n"
                + "          half glowA = 0;\n"
                + "          if(_OutlineMode > 1.5) {\n"
                + "              half sdf = SampleSDF(uv);\n"
                + "              half gAvgTexel = (_MainTex_TexelSize.x + _MainTex_TexelSize.y) * 0.5;\n"
                + "              half glowPx = _GlowSize / max(gAvgTexel, 1e-6);\n"
                + "              half glowSdfW = glowPx / max(_OutlineSDFSpread * 2, 1);\n"
                + "              half glowFade = AIO_SmoothStep(0.5 - glowSdfW, 0.5, sdf);\n"
                + "              glowA = glowFade * (1.0 - AIO_SmoothStep(0.5, 0.5 + fwidth(sdf), sdf));\n"
                + "          } else {\n"
                + "              half total=0;\n"
                + "              for(int gx=-2;gx<=2;gx++) for(int gy=-2;gy<=2;gy++) {\n"
                + "                  half w = 1.0/(1+abs(gx)+abs(gy));\n"
                + "                  glowA += SampleAtlasClamped(uv+half2(gx,gy)*_GlowSize)*w;\n"
                + "                  total += w; }\n"
                + "              glowA = glowA/total*(1-col.a);\n"
                + "          }\n"
                + "          col.rgb += _GlowColor.rgb*glowA*_GlowIntensity*m;\n"
                + "          col.a = max(col.a, glowA*m);"
            },
            new EffectDefinition {
                Keyword = "_Shadow_En",
                Body = "half m = eMask * SampleEffectMask(uv,masks,_ShadowMaskCh);\n"
                + "          half4 shadow = SampleMainClean(uv-_ShadowOffset.xy);\n"
                + "          half shadowA = shadow.a * _ShadowColor.a * (1-col.a);\n"
                + "          col = lerp(col, half4(_ShadowColor.rgb,1), shadowA*m);"
            },
            new EffectDefinition {
                Keyword = "_SoftShadow_En",
                Body = "half m = eMask * SampleEffectMask(uv,masks,_SoftShadowMaskCh);\n"
                + "          half shadowA = 0; half total = 0;\n"
                + "          int samples = (int)_SoftShadowSamples;\n"
                + "          for(int sx=-samples;sx<=samples;sx++) for(int sy=-samples;sy<=samples;sy++) {\n"
                + "              half w = 1.0/(1+abs(sx)+abs(sy));\n"
                + "              float2 sampleUV = ClampToAtlas(uv-_SoftShadowOffset.xy+half2(sx,sy)*_SoftShadowSoftness);\n"
                + "              shadowA += SampleMainClean(sampleUV).a * w;\n"
                + "              total += w; }\n"
                + "          shadowA = shadowA/total * _SoftShadowColor.a * (1-col.a);\n"
                + "          col = lerp(col, half4(_SoftShadowColor.rgb,1), shadowA*m);"
            },
            new EffectDefinition {
                Keyword = "_DirOutline_En",
                Body = "half m = eMask * SampleEffectMask(uv, masks, _DirOutlineMaskCh);\n"
                + "            if (col.a < 0.01) return;\n"
                + "            half nd = _DirOutlineWidth;\n"
                + "            half aR = SampleAtlasClamped(uv + half2(nd, 0));\n"
                + "            half aL = SampleAtlasClamped(uv - half2(nd, 0));\n"
                + "            half aU = SampleAtlasClamped(uv + half2(0, nd));\n"
                + "            half aD = SampleAtlasClamped(uv - half2(0, nd));\n"
                + "            half2 normal2D = half2(aL - aR, aD - aU);\n"
                + "            half edgeness = length(normal2D);\n"
                + "            if (_OutlineMode > 1.5) {\n"
                + "                half sR = SampleSDF(uv + half2(nd, 0));\n"
                + "                half sL = SampleSDF(uv - half2(nd, 0));\n"
                + "                half sU = SampleSDF(uv + half2(0, nd));\n"
                + "                half sD = SampleSDF(uv - half2(0, nd));\n"
                + "                half2 sdfNormal = half2(sL - sR, sD - sU);\n"
                + "                if (length(sdfNormal) > 0.0001) normal2D = sdfNormal;\n"
                + "            }\n"
                + "            half2 dir = normalize(_DirOutlineDir.xy + 0.0001);\n"
                + "            half NdD = saturate(dot(normalize(normal2D + 0.0001), dir));\n"
                + "            NdD = AIO_Pow(NdD, lerp(4.0, 0.5, _DirOutlineSoft));\n"
                + "            half dirOutline = edgeness * NdD * _DirOutlineInt;\n"
                + "            col.rgb = lerp(col.rgb, _DirOutlineColor.rgb, saturate(dirOutline * m));"
            },
            new EffectDefinition {
                Keyword = "_OutlineOnly_En",
                Body = "half m = eMask * SampleEffectMask(uv,masks,_OutlineOnlyMaskCh);\n"
                + "          half maxA = SampleOutlineAlpha(uv, _OutlineOnlyWidth);\n"
                + "          half outline = MaskOutline(maxA, col.a);\n"
                + "          col = half4(_OutlineOnlyColor.rgb, outline*_OutlineOnlyColor.a*m);"
            },
            new EffectDefinition {
                Keyword = "_GradOutline_En",
                Body = "half m = eMask * SampleEffectMask(uv,masks,_GradOutlineMaskCh);\n"
                + "          half maxA = SampleOutlineAlpha(uv, _GradOutlineSize);\n"
                + "          half outline = MaskOutline(maxA, col.a);\n"
                + "          half2 gradDir = half2(cos(_GradOutlineAngle), sin(_GradOutlineAngle));\n"
                + "          half gradT = dot(LocalUV(uv) - 0.5, gradDir) + 0.5;\n"
                + "          half3 gradCol = lerp(_GradOutlineColorA.rgb, _GradOutlineColorB.rgb, saturate(gradT));\n"
                + "          col = half4(lerp(col.rgb, gradCol, outline*m), max(col.a, outline*m));"
            },
            new EffectDefinition {
                Keyword = "_AnimOutline_En",
                Body = "half m = eMask * SampleEffectMask(uv, masks, _AnimOutlineMaskCh);\n"
                + "            half sz = lerp(_AnimOutlineMin, _AnimOutlineMax, sin(_AnimOutlineSpeed * time) * 0.5 + 0.5);\n"
                + "            half maxA = SampleOutlineAlpha(uv, sz);\n"
                + "            half outline = MaskOutline(maxA, col.a);\n"
                + "            col = half4(lerp(col.rgb, _AnimOutlineColor.rgb, outline * m), max(col.a, outline * m));"
            },
            new EffectDefinition {
                Keyword = "_SelectHighlight_En",
                Body = "half m = eMask * SampleEffectMask(uv,masks,_SelectHighlightMaskCh);\n"
                + "          half maxA = SampleOutlineAlpha(uv, _SelectHighlightSize);\n"
                + "          half outline = MaskOutline(maxA, col.a);\n"
                + "          half dash = step(0.5, frac((uv.x+uv.y)*_SelectHighlightDash + _SelectHighlightSpeed * time));\n"
                + "          col = half4(lerp(col.rgb, _SelectHighlightColor.rgb, outline*dash*m), max(col.a, outline*dash*m));"
            },
            new EffectDefinition {
                Keyword = "_EnergyAura_En",
                Body = "half m = _AuraFade * eMask * SampleEffectMask(uv,masks,_AuraMaskCh);\n"
                + "          half maxAlpha = 0;\n"
                + "          half totalAlpha = 0;\n"
                + "          for(int ai = 0; ai < 16; ai++)\n"
                + "          {\n"
                + "              half angle = ai * 0.3927;\n"
                + "              half2 dir = half2(cos(angle), sin(angle));\n"
                + "              for(int ri = 1; ri <= 3; ri++)\n"
                + "              {\n"
                + "                  half r = _AuraRadius * (half)ri * 0.333;\n"
                + "                  half sampA = SampleAtlasClamped(uv + dir * r);\n"
                + "                  maxAlpha = max(maxAlpha, sampA);\n"
                + "                  totalAlpha += sampA;\n"
                + "              }\n"
                + "          }\n"
                + "          half proximity = maxAlpha * (1.0 - col.a);\n"
                + "          half noiseVal = ValueNoise(uv * _AuraFrequency + _AuraSpeed * time * 0.3);\n"
                + "          half noiseVal2 = ValueNoise(uv * _AuraFrequency * 1.7 - _AuraSpeed * time * 0.2 + 50);\n"
                + "          half combinedNoise = noiseVal * 0.6 + noiseVal2 * 0.4;\n"
                + "          half wave = sin(totalAlpha * 2.0 - _AuraSpeed * time) * 0.3 + 0.7;\n"
                + "          half aura = proximity * combinedNoise * wave;\n"
                + "          col.rgb += _AuraColor.rgb * aura * _AuraIntensity * m;\n"
                + "          col.a = max(col.a, aura * _AuraIntensity * m);"
            },
            new EffectDefinition {
                Keyword = "_FakeLighting_En",
                Body = "half m=_FakeLightFade*eMask*SampleEffectMask(uv,masks,_FakeLightMaskCh);half nd=_FakeLightNDist;\n"
                + "        half aR=SampleAtlasClamped(uv+half2(nd,0));half aL=SampleAtlasClamped(uv-half2(nd,0));half aU=SampleAtlasClamped(uv+half2(0,nd));half aD=SampleAtlasClamped(uv-half2(0,nd));\n"
                + "        half3 normal=normalize(half3(aL-aR,aD-aU,_FakeLightNZ));half3 ld=normalize(_FakeLightDir.xyz);half NdL=saturate(dot(normal,ld));\n"
                + "        half3 lit=col.rgb*(_FakeLightAmbient+_FakeLightColor.rgb*NdL*_FakeLightIntensity);half3 hd=normalize(ld+half3(0,0,1));\n"
                + "        lit+=_FakeLightColor.rgb*pow(saturate(dot(normal,hd)),_FakeLightSpecPow)*_FakeLightSpecInt;col.rgb=lerp(col.rgb,lit,m*col.a);"
            },
            new EffectDefinition {
                Keyword = "_Dissolve_En",
                Body = "half m = SampleEffectMask(uv,masks,_DissolveMaskCh);\n"
                + "          half rawAmount = _DissolveAmount;\n"
                + "          if (rawAmount > 0.999) { col.a = 0; }\n"
                + "          else if (rawAmount > 0.001) {\n"
                + "          float2 noiseUV = uv + half2(time*_DissolveNoiseSpeed, 0);\n"
                + "          half4 texSample = half4(0.5,0.5,0.5,1);\n"
                + "          if(_DissolveNoiseType < 0.5)\n"
                + "              texSample = STEX(_DissolveTex,sampler_DissolveTex,noiseUV * _DissolveTiling.xy + _DissolveTiling.zw);\n"
                + "          half noise = SampleNoise(noiseUV, _DissolveNoiseType, _DissolveNoiseScale, texSample) + (1.0 - m); // Mask offsets noise: protected areas need higher amount to dissolve\n"
                + "          half totalBorder = _DissolveBorderWidth + _DissolveBurnWidth;\n"
                + "          half edge = totalBorder + 0.01;\n"
                + "          half threshold = lerp(-edge, 1.0, rawAmount);\n"
                + "          half fw = 0.005;\n"
                + "          half dissolve = AIO_SmoothStep(threshold, threshold + fw, noise);\n"
                + "          half borderMask = AIO_SmoothStep(threshold - _DissolveBorderWidth, threshold, noise);\n"
                + "          half border = max(borderMask - dissolve, 0);\n"
                + "          half burn = 0;\n"
                + "          if(_DissolveBurnWidth > 0.001) {\n"
                + "              half burnMask = AIO_SmoothStep(threshold - totalBorder, threshold - _DissolveBorderWidth, noise);\n"
                + "              burn = max(burnMask - borderMask, 0); }\n"
                + "          half3 fireEdge = lerp(_DissolveBurnColor.rgb, _DissolveBorderColor.rgb, saturate(border * 5.0));\n"
                + "          half edgeAlpha = saturate(border + burn);\n"
                + "          col.rgb = lerp(col.rgb, fireEdge, edgeAlpha);\n"
                + "          col.a *= saturate(dissolve + edgeAlpha); }"
            },
            new EffectDefinition {
                Keyword = "_GlowDissolve_En",
                Body = "half m = SampleEffectMask(uv,masks,_GlowDissolveMaskCh);\n"
                + "          half rawAmt = _GlowDissolveAmount;\n"
                + "          if (rawAmt > 0.999) { col.a = 0; }\n"
                + "          else if (rawAmt > 0.001) {\n"
                + "          half4 texSample = STEX(_GlowDissolveTex,sampler_GlowDissolveTex,uv);\n"
                + "          half noise = SampleNoise(uv, _GlowDissolveNoiseType, _GlowDissolveNoiseScale, texSample) + (1.0 - m);\n"
                + "          half glowW = max(_GlowDissolveWidth, 0.001);\n"
                + "          half edge = glowW + 0.01;\n"
                + "          half threshold = lerp(-edge, 1.0, rawAmt);\n"
                + "          half fw = 0.005;\n"
                + "          half dissolve = AIO_SmoothStep(threshold, threshold + fw, noise);\n"
                + "          half glow = saturate(1-abs(noise-threshold)/glowW) * (1-dissolve);\n"
                + "          col.rgb += _GlowDissolveColor.rgb * glow * col.a;\n"
                + "          col.a *= saturate(dissolve+glow); }"
            },
            new EffectDefinition {
                Keyword = "_SrcDissolve_En",
                Body = "half m = SampleEffectMask(uv,masks,_SrcDissolveMaskCh);\n"
                + "          half rawAmt = _SrcDissolveAmount * m;\n"
                + "          if (rawAmt > 0.999) { col.a = 0; }\n"
                + "          else if (rawAmt > 0.001) {\n"
                + "          half2 luv = LocalUV(uv);\n"
                + "          half2 aPos = _SrcDissolvePosition.xy * half2(_SpriteAspect, 1);\n"
                + "          half2 aLuv = luv * half2(_SpriteAspect, 1);\n"
                + "          half dist = distance(aLuv, aPos);\n"
                + "          // Dynamic max distance from position to farthest corner\n"
                + "          half maxDist = max(max(distance(aPos, half2(0,0)), distance(aPos, half2(_SpriteAspect,0))),\n"
                + "                          max(distance(aPos, half2(0,1)), distance(aPos, half2(_SpriteAspect,1))));\n"
                + "          half bw = max(_SrcDissolveBorderWidth, 0.001);\n"
                + "          half edge = rawAmt * (maxDist + bw);\n"
                + "          half fw = 0.005;\n"
                + "          half dissolve = AIO_SmoothStep(edge - fw, edge, dist);\n"
                + "          half border = (1.0 - AIO_SmoothStep(edge, edge + bw, dist)) * dissolve;\n"
                + "          col.rgb = lerp(col.rgb, _SrcDissolveBorderColor.rgb, border);\n"
                + "          col.a *= dissolve; }"
            },
            new EffectDefinition {
                Keyword = "_DirFade_En",
                Body = "half m = SampleEffectMask(uv,masks,_DirFadeMaskCh);\n"
                + "          half rawAmt = _DirFadeAmount;\n"
                + "          if (rawAmt > 0.999) { col.a *= 1-m; }\n"
                + "          else if (rawAmt > 0.001) {\n"
                + "          half2 dir = half2(cos(_DirFadeAngle),sin(_DirFadeAngle));\n"
                + "          half2 luv = LocalUV(uv);\n"
                + "          half d = dot(luv-0.5, dir)+0.5;\n"
                + "          // Dynamic max extent: project all 4 corners onto dir, find range\n"
                + "          half c0=dot(float2(-0.5,-0.5),dir)+0.5, c1=dot(float2(0.5,-0.5),dir)+0.5,\n"
                + "               c2=dot(float2(-0.5,0.5),dir)+0.5, c3=dot(float2(0.5,0.5),dir)+0.5;\n"
                + "          half dMin=min(min(c0,c1),min(c2,c3)), dMax=max(max(c0,c1),max(c2,c3));\n"
                + "          half smooth = max(_DirFadeSmoothness, 0.001);\n"
                + "          half threshold = lerp(dMin - smooth, dMax, rawAmt);\n"
                + "          half fw = 0.005;\n"
                + "          col.a *= lerp(1, AIO_SmoothStep(threshold - fw, threshold + smooth, d), m); }"
            },
            new EffectDefinition {
                Keyword = "_DirGlowFade_En",
                Body = "half m = SampleEffectMask(uv,masks,_DirGlowFadeMaskCh);\n"
                + "          half rawAmt = _DirGlowFadeAmount;\n"
                + "          if (rawAmt > 0.999) { col.a *= 1-m; }\n"
                + "          else if (rawAmt > 0.001) {\n"
                + "          half2 dir = half2(cos(_DirGlowFadeAngle),sin(_DirGlowFadeAngle));\n"
                + "          half2 luv = LocalUV(uv);\n"
                + "          half d = dot(luv-0.5, dir)+0.5;\n"
                + "          half c0=dot(float2(-0.5,-0.5),dir)+0.5, c1=dot(float2(0.5,-0.5),dir)+0.5,\n"
                + "               c2=dot(float2(-0.5,0.5),dir)+0.5, c3=dot(float2(0.5,0.5),dir)+0.5;\n"
                + "          half dMin=min(min(c0,c1),min(c2,c3)), dMax=max(max(c0,c1),max(c2,c3));\n"
                + "          half glowW = max(_DirGlowFadeWidth, 0.001);\n"
                + "          half threshold = lerp(dMin - glowW, dMax, rawAmt);\n"
                + "          half fw = 0.005;\n"
                + "          half fade = AIO_SmoothStep(threshold - fw, threshold + fw, d);\n"
                + "          half glow = saturate(1-abs(d-threshold)/glowW);\n"
                + "          col.rgb += _DirGlowFadeColor.rgb * glow * m * col.a;\n"
                + "          col.a *= lerp(1, fade, m); }"
            },
            new EffectDefinition {
                Keyword = "_FullAlphaDissolve_En",
                Body = "half m = SampleEffectMask(uv,masks,_FullAlphaDissolveMaskCh);\n"
                + "          col.a *= lerp(1, 1-_FullAlphaDissolveAmount, m);"
            },
            new EffectDefinition {
                Keyword = "_Halftone_En",
                Body = "half m = SampleEffectMask(uv,masks,_HalftoneMaskCh);\n"
                + "          half rawAmt = _HalftoneAmount;\n"
                + "          if (rawAmt > 0.999) { col.a *= 1-m; }\n"
                + "          else if (rawAmt > 0.001) {\n"
                + "          half2 htUV = LocalUV(uv) * _HalftoneSize;\n"
                + "          half dot2 = length(frac(htUV)-0.5);\n"
                + "          half smooth = max(_HalftoneSmoothness, 0.001);\n"
                + "          half threshold = lerp(-smooth, 0.72, rawAmt);\n"
                + "          half fw = 0.005;\n"
                + "          half ht = AIO_SmoothStep(threshold, threshold + max(smooth, fw), dot2);\n"
                + "          col.a *= lerp(1, ht, m); }"
            },
            new EffectDefinition {
                Keyword = "_RadialWipe_En",
                Body = "half m = SampleEffectMask(uv,masks,_RadialWipeMaskCh);\n"
                + "          half rawAmt = _RadialWipeAmount;\n"
                + "          if (rawAmt > 0.999) { col.a *= 1-m; }\n"
                + "          else if (rawAmt > 0.001) {\n"
                + "          half2 delta = LocalUV(uv) - 0.5;\n"
                + "          half angle = atan2(delta.y, delta.x) - _RadialWipeAngle;\n"
                + "          // Wrap to [0,1] without frac() seam: use smooth atan2-safe remapping\n"
                + "          half norm = angle / (2 * 3.14159265) + 0.5;\n"
                + "          norm = norm - floor(norm); // same as frac but explicit\n"
                + "          half smooth = max(_RadialWipeSmoothness, 0.001);\n"
                + "          // Avoid seam: clamp smoothstep away from the 0/1 boundary\n"
                + "          half fw = 0.005;\n"
                + "          half wipe = AIO_SmoothStep(rawAmt - smooth, rawAmt + 0.002, norm);\n"
                + "          col.a *= lerp(1, wipe, m); }"
            },
            new EffectDefinition {
                Keyword = "_IrisWipe_En",
                Body = "half m = SampleEffectMask(uv,masks,_IrisWipeMaskCh);\n"
                + "          half rawAmt = _IrisWipeAmount;\n"
                + "          if (rawAmt > 0.999) { col.a *= 1-m; }\n"
                + "          else if (rawAmt > 0.001) {\n"
                + "          float2 luv = LocalUV(uv);\n"
                + "          float2 aCtr = _IrisWipeCenter.xy * float2(_SpriteAspect, 1); float2 aLuv = luv * float2(_SpriteAspect, 1);\n"
                + "  half dist = distance(aLuv, aCtr);\n"
                + "          half maxDist = max(max(distance(irisC, float2(0,0)), distance(irisC, float2(_SpriteAspect,0))),\n"
                + "                          max(distance(irisC, float2(0,1)), distance(irisC, float2(_SpriteAspect,1))));\n"
                + "          half smooth = max(_IrisWipeSmoothness, 0.001);\n"
                + "          half radius = (1.0 - rawAmt) * (maxDist + smooth);\n"
                + "          half fw = 0.005;\n"
                + "          col.a *= lerp(1, AIO_SmoothStep(radius - fw, radius + smooth, dist), m); }"
            },
            new EffectDefinition {
                Keyword = "_Blinds_En",
                Body = "half m = SampleEffectMask(uv,masks,_BlindsMaskCh);\n"
                + "          half rawAmt = _BlindsAmount;\n"
                + "          if (rawAmt > 0.999) { col.a *= 1-m; }\n"
                + "          else if (rawAmt > 0.001) {\n"
                + "          float2 blLuv = LocalUV(uv); half coord = _BlindsDirection < 0.5 ? blLuv.x : blLuv.y;\n"
                + "          half strip = frac(coord * _BlindsCount);\n"
                + "          half blind = step(rawAmt, strip);\n"
                + "          col.a *= lerp(1, blind, m); }"
            },
            new EffectDefinition {
                Keyword = "_DiamondDissolve_En",
                Body = "half m = SampleEffectMask(uv,masks,_DiamondDissolveMaskCh);\n"
                + "          half rawAmt = _DiamondDissolveAmount;\n"
                + "          if (rawAmt > 0.999) { col.a *= 1-m; }\n"
                + "          else if (rawAmt > 0.001) {\n"
                + "          float2 ddLuv = LocalUV(uv); half2 dc = abs(ddLuv * _DiamondDissolveScale - floor(ddLuv * _DiamondDissolveScale + 0.5));\n"
                + "          half diamond = (dc.x + dc.y);\n"
                + "          col.a *= lerp(1, step(rawAmt, diamond), m); }"
            },
            new EffectDefinition {
                Keyword = "_ScanlineDissolve_En",
                Body = "half m = SampleEffectMask(uv,masks,_ScanlineDissolveMaskCh);\n"
                + "          half rawAmt = _ScanlineDissolveAmount;\n"
                + "          if (rawAmt > 0.999) { col.a *= 1-m; }\n"
                + "          else if (rawAmt > 0.001) {\n"
                + "          half scanY = frac(LocalUV(uv).y * _ScanlineDissolveDensity);\n"
                + "          half survive = step(rawAmt, scanY);\n"
                + "          col.a *= lerp(1, survive, m); }"
            },
            new EffectDefinition {
                Keyword = "_SpreadFade_En",
                Body = "half m = SampleEffectMask(uv,masks,_SpreadFadeMaskCh);\n"
                + "          half rawAmt = _SpreadFadeAmount * m;\n"
                + "          if (rawAmt > 0.999) { col.a = 0; }\n"
                + "          else if (rawAmt > 0.001) {\n"
                + "          half2 luv = LocalUV(uv);\n"
                + "          half2 org = _SpreadFadeOrigin.xy;\n"
                + "          half2 aOrg = org * half2(_SpriteAspect, 1.0);\n"
                + "          half2 aLuv = luv * half2(_SpriteAspect, 1.0);\n"
                + "          half dist = distance(aLuv, aOrg);\n"
                + "          // Dynamic max distance from origin to farthest corner\n"
                + "          half maxDist = max(max(distance(aOrg, half2(0,0)), distance(aOrg, half2(_SpriteAspect,0))),\n"
                + "                          max(distance(aOrg, half2(0,1)), distance(aOrg, half2(_SpriteAspect,1))));\n"
                + "          half smooth = max(_SpreadFadeSmoothness, 0.001);\n"
                + "          // Radius grows from 0 to maxDist as Amount increases\n"
                + "          half radius = rawAmt * (maxDist + smooth);\n"
                + "          // pixels closer than radius become 0 (transparent), pixels further remain 1 (visible)\n"
                + "          half fade = AIO_SmoothStep(radius, radius + smooth, dist);\n"
                + "          col.a *= fade; }"
            },
            new EffectDefinition {
                Keyword = "_PixelDissolve_En",
                Body = "half m = SampleEffectMask(uv,masks,_PixelDissolveMaskCh);\n"
                + "          half rawAmt = _PixelDissolveAmount;\n"
                + "          if (rawAmt > 0.999) { col.a = 0; }\n"
                + "          else if (rawAmt > 0.001) {\n"
                + "          half2 pixUV = floor(LocalUV(uv) * _PixelDissolveGrid) / _PixelDissolveGrid;\n"
                + "          half noise = SimpleNoise(pixUV * 100) + (1.0 - m);\n"
                + "          col.a *= step(rawAmt, noise); }"
            },
            new EffectDefinition {
                Keyword = "_SpiralWipe_En",
                Body = "half m = SampleEffectMask(uv,masks,_SpiralWipeMaskCh);\n"
                + "          half rawAmt = _SpiralWipeAmount;\n"
                + "          if (rawAmt > 0.999) { col.a *= 1-m; }\n"
                + "          else if (rawAmt > 0.001) {\n"
                + "          half2 delta = LocalUV(uv) - 0.5;\n"
                + "          half angle = atan2(delta.y, delta.x) / (2*3.14159265) + 0.5;\n"
                + "          half dist = length(delta) * 2;\n"
                + "          half spiral = angle + dist * _SpiralWipeArms;\n"
                + "          spiral = spiral - floor(spiral);\n"
                + "          half smooth = max(_SpiralWipeSmoothness, 0.001);\n"
                + "          half fw = 0.005;\n"
                + "          half wipe = AIO_SmoothStep(rawAmt - smooth, rawAmt + 0.002, spiral);\n"
                + "          col.a *= lerp(1, wipe, m); }"
            },
            new EffectDefinition {
                Keyword = "_HexDissolve_En",
                Body = "half m = SampleEffectMask(uv,masks,_HexDissolveMaskCh);\n"
                + "          half rawAmt = _HexDissolveAmount * m;\n"
                + "          if (rawAmt > 0.999) { col.a = 0; }\n"
                + "          else if (rawAmt > 0.001) {\n"
                + "          // Use LocalUV and Aspect correction so hexes don't stretch\n"
                + "          float2 luv = LocalUV(uv);\n"
                + "          luv.x *= _SpriteAspect;\n"
                + "          float2 hexUV = luv * _HexDissolveScale;\n"
                + "          float2 r = float2(1.0, 1.73205); float2 hr = r * 0.5;\n"
                + "          float2 a = (hexUV - r * floor(hexUV / r)) - hr;\n"
                + "          float2 bv = hexUV - hr; float2 b = (bv - r * floor(bv / r)) - hr;\n"
                + "          float2 gv = dot(a,a) < dot(b,b) ? a : b;\n"
                + "          float2 hexCell = hexUV - gv;\n"
                + "          // CRITICAL FIX: Round the cell ID to eliminate floating-point boundary tearing\n"
                + "          hexCell = round(hexCell * 100.0) / 100.0;\n"
                + "          half noise = SimpleNoise(hexCell);\n"
                + "          col.a *= step(rawAmt, noise); }"
            },
            new EffectDefinition {
                Keyword = "_BurnDirection_En",
                Body = "half m=SampleEffectMask(uv,masks,_BurnDirMaskCh);half ra=_BurnDirAmount*m;\n"
                + "        if(ra>0.999){col.a=0;}else if(ra>0.001){\n"
                + "        half2 dir=half2(cos(_BurnDirAngle),sin(_BurnDirAngle));half2 luv=LocalUV(uv);\n"
                + "        half d=dot(luv-0.5,dir)+0.5;\n"
                + "        // Dynamic extent\n"
                + "        half c0=dot(float2(-0.5,-0.5),dir)+0.5, c1=dot(float2(0.5,-0.5),dir)+0.5,\n"
                + "             c2=dot(float2(-0.5,0.5),dir)+0.5, c3=dot(float2(0.5,0.5),dir)+0.5;\n"
                + "        half dMin=min(min(c0,c1),min(c2,c3)), dMax=max(max(c0,c1),max(c2,c3));\n"
                + "        half noise=ValueNoise(uv*_BurnDirNoiseScale+time*0.3)*0.1;half ad=d+noise;\n"
                + "        half te=_BurnDirEdgeWidth+_BurnDirCharcoalWidth;\n"
                + "        half threshold=lerp(dMin-te,dMax,ra);\n"
                + "        half fw=0.005;\n"
                + "        half alive=AIO_SmoothStep(threshold,threshold+fw,ad);\n"
                + "        half fireMask=AIO_SmoothStep(threshold-_BurnDirEdgeWidth,threshold,ad);\n"
                + "        half fire=max(fireMask-alive,0);\n"
                + "        half charMask=AIO_SmoothStep(threshold-te,threshold-_BurnDirEdgeWidth,ad);\n"
                + "        half char=max(charMask-fireMask,0);\n"
                + "        half flicker=0.7+0.3*sin(time*8+uv.x*20);\n"
                + "        col.rgb=lerp(col.rgb,_BurnDirCharcoalColor.rgb,char);\n"
                + "        col.rgb=lerp(col.rgb,_BurnDirFireColor.rgb*flicker,fire);\n"
                + "        col.a*=saturate(alive+fire+char*0.8);}"
            },
            new EffectDefinition {
                Keyword = "_Hologram_En",
                Body = "half m = eMask * SampleEffectMask(uv,masks,_HologramMaskCh);\n"
                + "          half stripes = sin(uv.y*_HologramStripesAmount+_HologramStripesSpeed * time);\n"
                + "          half alpha = lerp(_HologramMinAlpha, 1, (stripes*0.5+0.5));\n"
                + "          col.rgb = lerp(col.rgb, _HologramColor.rgb, _HologramBlend*m);\n"
                + "          col.a *= lerp(1, alpha, m);\n"
                + "          col.rgb += (SimpleNoise(half2(floor(uv.y*100),time))-0.5)*_HologramDistortion*m;"
            },
            new EffectDefinition {
                Keyword = "_Glitch_En",
                Body = "half m = eMask * SampleEffectMask(uv,masks,_GlitchMaskCh);\n"
                + "          half rowScale = max(_GlitchSize, 1.0);\n"
                + "          half row = floor(uv.y * rowScale);\n"
                + "          // Use stepped frames instead of feeding tiny time deltas into a hash noise.\n"
                + "          // _GlitchSpeed now behaves predictably as pattern updates per second:\n"
                + "          // 0 = static glitch, 0.01 = one update per 100 seconds, 5 = five updates per second.\n"
                + "          half glitchSpeed = max(_GlitchSpeed, 0.0);\n"
                + "          half glitchFrame = (glitchSpeed > 0.0001) ? floor(time * glitchSpeed) : 0.0;\n"
                + "          half glitchLine = step(0.5, SimpleNoise(float2(row + 17.0, glitchFrame + 3.0)));\n"
                + "          half shiftSeed = SimpleNoise(float2(row * 0.37 + 41.0, glitchFrame + 11.0));\n"
                + "          half shift = (shiftSeed - 0.5) * _GlitchAmount * glitchLine * m;\n"
                + "          if(abs(shift) > 0.001) {\n"
                + "              half4 origHere = SampleMainClean(uv);\n"
                + "              half4 shifted = SampleMainClean(uv+half2(shift,0));\n"
                + "              // Preserve color modifications: compute ratio of processed/original, apply to shifted\n"
                + "              half3 ratio = col.rgb / max(origHere.rgb, 0.01);\n"
                + "              col.rgb = lerp(col.rgb, shifted.rgb * ratio, shifted.a);\n"
                + "              col.a = max(col.a, shifted.a * 0.5);\n"
                + "          }\n"
                + "          half4 rShift = SampleMainClean(uv+half2(shift+_GlitchColorShift*m,0));\n"
                + "          half4 bShift = SampleMainClean(uv+half2(shift-_GlitchColorShift*m,0));\n"
                + "          col.r = lerp(col.r, rShift.r, rShift.a);\n"
                + "          col.b = lerp(col.b, bShift.b, bShift.a);"
            },
            new EffectDefinition {
                Keyword = "_Flicker_En",
                Body = "half m = SampleEffectMask(uv,masks,_FlickerMaskCh);\n"
                + "          half flick = step(_FlickerPercent, frac(_FlickerFreq * time));\n"
                + "          col.a *= lerp(1, flick, m);"
            },
            new EffectDefinition {
                Keyword = "_Shine_En",
                Body = "half m = eMask * SampleEffectMask(uv,masks,_ShineMaskCh);\n"
                + "          half2 shineUV = LocalUV(uv)-0.5;\n"
                + "          half s,c; sincos(_ShineRotation,s,c);\n"
                + "          half proj = shineUV.x*c+shineUV.y*s;\n"
                + "          half rawTravel = frac(_ShineSpeed * time);\n"
                + "          half speedAbs = max(abs(_ShineSpeed), 0.0001);\n"
                + "          half travelDuration = rcp(speedAbs);\n"
                + "          half totalDuration = travelDuration + max(_ShineDelay, 0.0);\n"
                + "          half cycleTime = frac(time / max(totalDuration, 0.0001)) * totalDuration;\n"
                + "          half delayedTravel = saturate(cycleTime / max(travelDuration, 0.0001));\n"
                + "          delayedTravel = lerp(delayedTravel, 1.0 - delayedTravel, step(_ShineSpeed, -0.0001));\n"
                + "          half useDelay = step(0.0001, _ShineDelay) * step(0.0001, abs(_ShineSpeed));\n"
                + "          half travel = lerp(rawTravel, delayedTravel, useDelay);\n"
                + "          half active = lerp(1.0, step(cycleTime, travelDuration), useDelay);\n"
                + "          half shine = saturate(1 - abs(proj - travel * 2 + 1) / _ShineWidth) * active;\n"
                + "          shine = AIO_Pow(shine,2)*_ShineGlow;\n"
                + "          col.rgb += _ShineColor.rgb*shine*m*col.a;"
            },
            new EffectDefinition {
                Keyword = "_Smoke_En",
                Body = "half m = _SmokeFade * eMask * SampleEffectMask(uv,masks,_SmokeMaskCh);\n"
                + "          half n1 = ValueNoise(uv*_SmokeDensity+half2(_SmokeSpeed * time*_SmokeDirection.x, _SmokeSpeed * time*_SmokeDirection.y));\n"
                + "          half n2 = ValueNoise(uv*_SmokeDensity*2+half2(_SmokeSpeed * time*0.7*_SmokeDirection.x, _SmokeSpeed * time*0.7*_SmokeDirection.y)+100);\n"
                + "          half n3 = ValueNoise(uv*_SmokeDensity*0.5+half2(_SmokeSpeed * time*0.3, 0)+200);\n"
                + "          half smoke = n1*n2;\n"
                + "          smoke = lerp(smoke, smoke*n3, _SmokeTurbulence);\n"
                + "          col.rgb = lerp(col.rgb, _SmokeColor.rgb, saturate(smoke*m*col.a*_SmokeColorStr));"
            },
            new EffectDefinition {
                Keyword = "_Flame_En",
                Body = "half m = _FlameIntensity * eMask * SampleEffectMask(uv,masks,_FlameMaskCh);\n"
                + "          half n1 = ValueNoise(half2(uv.x*5*_FlameDensity, uv.y*3*_FlameDensity-_FlameSpeed * time));\n"
                + "          half n2 = ValueNoise(half2(uv.x*8*_FlameDensity, uv.y*5*_FlameDensity-_FlameSpeed * time*1.3)+50);\n"
                + "          half n = n1 * 0.6 + n2 * 0.4;\n"
                + "          half flameShape = n * 2.0 - uv.y;\n"
                + "\n"
                + "          if(_FlameExtendSides > 0.01) {\n"
                + "              half xDist = abs(uv.x - 0.5) * 2.0;\n"
                + "              // Improved pinch math: uses a curve so the bottom stays wide, but the top pinches sharply.\n"
                + "              // Multiplied by 5.0 to give the falloff slider a much stronger, visible effect.\n"
                + "              half pinch = 1.0 + AIO_Pow(uv.y, 2.0) * (_FlameSideFalloff * 5.0);\n"
                + "              \n"
                + "              // CRITICAL FIX: AIO_SmoothStep(min, max, value) followed by 1.0 - result.\n"
                + "              half minEdge = _FlameExtendSides * 0.2;\n"
                + "              half maxEdge = _FlameExtendSides;\n"
                + "              half sideMask = 1.0 - AIO_SmoothStep(minEdge, maxEdge, xDist * pinch);\n"
                + "              \n"
                + "              flameShape *= sideMask;\n"
                + "          }\n"
                + "\n"
                + "          flameShape = saturate(flameShape);\n"
                + "          half3 flameCol = lerp(_FlameColor2.rgb, _FlameColor1.rgb, flameShape);\n"
                + "\n"
                + "          if(_FlameOnlyMode > 0.5) {\n"
                + "              col.rgb = flameCol * flameShape * m;\n"
                + "              col.a = flameShape * m;\n"
                + "          } else {\n"
                + "              col.rgb += flameCol * flameShape * m * col.a;\n"
                + "          }"
            },
            new EffectDefinition {
                Keyword = "_Enchanted_En",
                Body = "half m = _EnchantedFade * eMask * SampleEffectMask(uv,masks,_EnchantedMaskCh);\n"
                + "          // 1. Create a smooth, slowly drifting base noise field\n"
                + "          half n = ValueNoise(uv * _EnchantedDensity + time * 0.05);\n"
                + "\n"
                + "          // 2. Contour mapping: Multiply the noise to create frequency, wrap in sin()\n"
                + "          // Subtracting time makes the rings constantly radiate outward from the centers\n"
                + "          half ripples = sin(n * 30.0 - time * _EnchantedSpeed * 5.0);\n"
                + "\n"
                + "          // 3. Make it 'Punchy': Separate into a sharp white-hot core and a neon glow\n"
                + "          half core = pow(saturate(ripples), 12.0);      // Very sharp inner line\n"
                + "          half glow = pow(saturate(ripples), 2.0) * 0.5; // Wider, softer aura\n"
                + "\n"
                + "          // 4. Combine for high-contrast energy\n"
                + "          half energy = (core + glow) * _EnchantedBrightness;\n"
                + "\n"
                + "          // 5. Add to the sprite (additive blend for that glowing screen effect)\n"
                + "          col.rgb += _EnchantedColor.rgb * energy * m * col.a;"
            },
            new EffectDefinition {
                Keyword = "_Shifting_En",
                Body = "half m = _ShiftingFade * eMask * SampleEffectMask(uv,masks,_ShiftingMaskCh);\n"
                + "          half t = sin(_ShiftingSpeed * time)*0.5+0.5;\n"
                + "          half3 shift = lerp(_ShiftingColor1.rgb, _ShiftingColor2.rgb, t);\n"
                + "          col.rgb = lerp(col.rgb, shift*dot(col.rgb,half3(0.299,0.587,0.114)), m * col.a);"
            },
            new EffectDefinition {
                Keyword = "_ShiftRainbow_En",
                Body = "half m = _ShiftRainbowFade * eMask * SampleEffectMask(uv,masks,_ShiftRainbowMaskCh);\n"
                + "          half hue = frac(_ShiftRainbowSpeed * time+dot(uv,half2(1,1))*_ShiftRainbowDensity);\n"
                + "          half3 rb = HSV2RGB(half3(hue,1,1));\n"
                + "          half lum = dot(col.rgb,half3(0.299,0.587,0.114));\n"
                + "          col.rgb = lerp(col.rgb, rb*lum, m * col.a);"
            },
            new EffectDefinition {
                Keyword = "_Overlay_En",
                Body = "half m = _OverlayBlend * eMask * SampleEffectMask(uv,masks,_OverlayMaskCh);\n"
                + "          float2 ovUV = uv * _OverlayTiling.xy + _OverlayTiling.zw + half2(_OverlaySpeed.x * time, _OverlaySpeed.y * time);\n"
                + "          half4 ov = STEX(_OverlayTex,sampler_OverlayTex,ovUV)*_OverlayColor;\n"
                + "          col.rgb = lerp(col.rgb, ov.rgb, m*col.a);"
            },
            new EffectDefinition {
                Keyword = "_DialogueEmphasis_En",
                Body = "half m = eMask * SampleEffectMask(uv,masks,_DialogueEmphasisMaskCh);\n"
                + "          half bob = sin(_DialogueBobSpeed * time) * _DialogueBobAmount * m;\n"
                + "          half4 emphCol = SampleMainClean(uv + half2(0, bob));\n"
                + "          emphCol.rgb += _DialogueBrightness * m * emphCol.a;\n"
                + "          col = lerp(col, emphCol, m);"
            },
            new EffectDefinition {
                Keyword = "_VoronoiCrackle_En",
                Body = "half m = _CrackleFade * eMask * SampleEffectMask(uv,masks,_CrackleMaskCh);\n"
                + "          half edgeDist, cellID;\n"
                + "          VoronoiNoise(uv * _CrackleScale, _CrackleSpeed * time, edgeDist, cellID);\n"
                + "          half edge = 1.0 - AIO_SmoothStep(0, _CrackleEdgeWidth, edgeDist);\n"
                + "          half3 cellTint = lerp(half3(1,1,1), half3(cellID, cellID*0.8, cellID*0.6), _CrackleCellTint);\n"
                + "          col.rgb = lerp(col.rgb * cellTint, _CrackleEdgeColor.rgb, edge * m * col.a);"
            },
            new EffectDefinition {
                Keyword = "_MotionSmear_En",
                Body = "half m = _SmearFade * eMask * SampleEffectMask(uv,masks,_SmearMaskCh);\n"
                + "          half4 smear = col;\n"
                + "          half2 smearDir = _SmearDirection.xy * _SmearAmount;\n"
                + "          int samples = max(2, (int)_SmearSamples);\n"
                + "          half totalW = 1;\n"
                + "          for(int si = 1; si <= samples; si++) {\n"
                + "              half t = si / (half)samples;\n"
                + "              half4 s = SampleMainClean(uv - smearDir * t);\n"
                + "              half w = 1 - t;\n"
                + "              smear += s * w;\n"
                + "              totalW += w;\n"
                + "          }\n"
                + "          smear /= totalW;\n"
                + "          col = lerp(col, smear, m);\n"
                + "          col.a = max(col.a, smear.a * m * 0.5);"
            },
            new EffectDefinition {
                Keyword = "_GlobalSmoke_En",
                Body = "half m=_GlobalSmokeOpacity*eMask*SampleEffectMask(uv,masks,_GlobalSmokeMaskCh);float2 su=GetScreenUV(screenPos);\n"
                + "        half n1=ValueNoise(su*_GlobalSmokeDensity+half2(_GlobalSmokeSpeed * time*_GlobalSmokeDir.x,_GlobalSmokeSpeed * time*_GlobalSmokeDir.y));\n"
                + "        half n2=ValueNoise(su*_GlobalSmokeDensity*2+half2(_GlobalSmokeSpeed * time*0.6*_GlobalSmokeDir.x,_GlobalSmokeSpeed * time*0.6*_GlobalSmokeDir.y)+100);\n"
                + "        half smoke=n1*n2*m;\n"
                + "        if (_GlobalSmokeOverlayOnly > 0.5) { col.rgb = _GlobalSmokeColor.rgb; col.a = smoke; }\n"
                + "        else { col.rgb=lerp(col.rgb,_GlobalSmokeColor.rgb,smoke);col.a=max(col.a,smoke*0.3); }"
            },
            new EffectDefinition {
                Keyword = "_HoloCard_En",
                Body = "half m=_HoloCardFade*eMask*SampleEffectMask(uv,masks,_HoloCardMaskCh);float2 vd=LocalUV(uv)-0.5;\n"
                + "        half fresnelBase=saturate(1.0-abs(dot(vd/(length(vd)+0.00001),float2(0,1))));half fresnel=AIO_Pow(fresnelBase,max(_HoloCardFPow,0.001));half hue=frac(dot(uv,half2(1,1))*_HoloCardDensity+_HoloCardSpeed * time+fresnel*_HoloCardFShift);\n"
                + "        half3 foil=HSV2RGB(half3(hue,_HoloCardSat,_HoloCardBright));col.rgb=lerp(col.rgb,col.rgb+foil*fresnel,m*col.a);\n"
                + "        col.rgb+=foil*(sin(uv.y*_HoloCardScanDen+_HoloCardScanSpd * time)*0.5+0.5)*_HoloCardScanInt*m*col.a;"
            },
            new EffectDefinition {
                Keyword = "_CloudShadows_En",
                Body = "half m=_CloudShadowFade*eMask*SampleEffectMask(uv,masks,_CloudShadowMaskCh);float2 cu=worldPos.xy*_CloudShadowScale+half2(_CloudShadowSpeed.x * time, _CloudShadowSpeed.y * time);\n"
                + "        half cloud=ValueNoise(cu)*ValueNoise(cu*2+50);cloud=AIO_SmoothStep(_CloudShadowThresh,_CloudShadowThresh+_CloudShadowSoft,cloud);\n"
                + "        if (_CloudShadowOverlayOnly > 0.5) { col.rgb = half3(0,0,0); col.a = cloud * m * _CloudShadowDark; }\n"
                + "        else { col.rgb*=lerp(1,1-_CloudShadowDark,cloud*m); }"
            },
            new EffectDefinition {
                Keyword = "_TrailA_En",
                Body = "half m=_TrailAInt*eMask*SampleEffectMask(uv,masks,_TrailAMaskCh);half spd=length(_TrailAVel.xy);if(spd>0.01 && col.a>0.01){\n"
                + "        half2 td=normalize(_TrailAVel.xy);half tt=dot(uv-AtlasCenter(),td)*2;half sn=saturate(spd/max(_TrailAMaxSpd,0.01));\n"
                + "        half tm=saturate(1-tt)*sn;\n"
                + "        col.rgb+=lerp(_TrailASlowCol.rgb,_TrailAFastCol.rgb,AIO_Pow(sn,_TrailACurve))*tm*m*col.a;}"
            },
            new EffectDefinition {
                Keyword = "_TrailB_En",
                Body = "half m=_TrailBInt*eMask*SampleEffectMask(uv,masks,_TrailBMaskCh);half spd=length(_TrailBVel.xy);if(spd>0.01){\n"
                + "        half2 td=_TrailBVel.xy/max(spd,0.001);half sn=saturate(spd/max(_TrailBMaxSpd,0.01));\n"
                + "        half offsetScale=sn*0.08;int segs=max(2,(int)_TrailBSegs);\n"
                + "        half4 ghost=half4(0,0,0,0);half totalW=0;\n"
                + "        for(int si=1;si<=segs;si++){\n"
                + "        half t=(half)si/(half)segs;half w=AIO_Pow(saturate(1-t),1/max(_TrailBFade,0.01));\n"
                + "        ghost+=SampleMainClean(uv+td*offsetScale*(half)si)*w;totalW+=w;}\n"
                + "        ghost/=max(totalW,0.001);half3 tinted=ghost.rgb*_TrailBCol.rgb;\n"
                + "        col.rgb=lerp(col.rgb,col.rgb+tinted,m*sn*ghost.a);col.a=max(col.a,ghost.a*m*sn*0.5);}"
            },
            new EffectDefinition {
                Keyword = "_TrailC_En",
                Body = "half m=_TrailCInt*eMask*SampleEffectMask(uv,masks,_TrailCMaskCh);half spd=length(_TrailCVel.xy);if(spd>0.01 && col.a>0.01){\n"
                + "        half2 td=normalize(_TrailCVel.xy);half tt=dot(uv-AtlasCenter(),td)*2;half sn=saturate(spd/max(_TrailCMaxSpd,0.01));\n"
                + "        half tm=saturate(1-tt)*sn;\n"
                + "        half hue=frac(_TrailCRbSpd * time+tt*0.5);half3 rc=lerp(_TrailCColA.rgb,_TrailCColB.rgb,sin(hue*6.2831)*0.5+0.5);\n"
                + "        rc=lerp(rc,HSV2RGB(half3(hue,1,1)),0.5);col.rgb+=rc*tm*AIO_Pow(sn,_TrailCCurve)*m*col.a;}"
            },
            new EffectDefinition {
                Keyword = "_Charging_En",
                Body = "half m = _ChargeFade * eMask * SampleEffectMask(uv,masks,_ChargeMaskCh); if (col.a < 0.01 || m <= 0.001) return;\n"
                + "        float2 chLuv = LocalUV(uv); chLuv.x *= _SpriteAspect;\n"
                + "        float2 chC = _ChargeCenter.xy; chC.x *= _SpriteAspect;\n"
                + "        half dist = distance(chLuv, chC);\n"
                + "        half radius = max(_ChargeRadius, 0.0001);\n"
                + "        half width = max(_ChargeWidth, 0.0001);\n"
                + "        half ring = saturate(1.0 - abs(dist - radius) / width);\n"
                + "        ring = ring * ring * (3.0 - 2.0 * ring);\n"
                + "        half pulse = sin(_ChargeSpeed * time - dist * _ChargeFreq) * 0.5 + 0.5;\n"
                + "        half density = max(_ChargeDensity, 0.001);\n"
                + "        half gn = lerp(1.0, ValueNoise(chLuv * density + half2(_ChargeSpeed * time * 0.5, 0)), saturate(_ChargeDensity));\n"
                + "        half ce = ring * pulse * gn * _ChargeInt;\n"
                + "        half angle = atan2(chLuv.y - chC.y, chLuv.x - chC.x);\n"
                + "        half lineWave = abs(sin(angle * max(_ChargeLines, 1.0) + _ChargeSpeed * time * 2.0));\n"
                + "        half lines = pow(lineWave, 8.0) * ring;\n"
                + "        ce += lines * 0.5 * _ChargeInt;\n"
                + "        col.rgb += _ChargeColor.rgb * ce * m * col.a;"
            },
            new EffectDefinition {
                Keyword = "_HealthTint_En",
                Body = "half m = eMask * SampleEffectMask(uv,masks,_HealthTintMaskCh);\n"
                + "          half health = saturate(_HealthTintAmount);\n"
                + "          half damage = 1 - health;\n"
                + "\n"
                + "          half lum = dot(col.rgb, half3(0.299,0.587,0.114));\n"
                + "          half3 desaturated = lerp(col.rgb, half3(lum,lum,lum), _HealthDesaturation * damage * m);\n"
                + "\n"
                + "          half3 tintColor = lerp(_HealthTintLowColor.rgb, _HealthTintHighColor.rgb, health);\n"
                + "          half3 tinted = lerp(desaturated, tintColor * lum, damage * m);\n"
                + "\n"
                + "          half pulse = 1.0;\n"
                + "          if(_HealthPulseSpeed > 0.01)\n"
                + "          {\n"
                + "              half pulseWave = sin(_HealthPulseSpeed * time * (1 + damage * 3)) * 0.5 + 0.5;\n"
                + "              pulse = lerp(1.0, lerp(0.7, 1.3, pulseWave), damage * damage * m);\n"
                + "          }\n"
                + "          tinted *= pulse;\n"
                + "\n"
                + "          if(_HealthVignetteStr > 0.01)\n"
                + "          {\n"
                + "              half2 vig = LocalUV(uv) - 0.5;\n"
                + "              half vigDist = length(vig) * 1.414;\n"
                + "              half vigFade = AIO_SmoothStep(0.3, 1.0, vigDist) * _HealthVignetteStr * damage * m;\n"
                + "              tinted = lerp(tinted, tintColor * 0.2, vigFade);\n"
                + "          }\n"
                + "\n"
                + "          if(_HealthDamageNoise > 0.01) {\n"
                + "              half dmgNoise = ValueNoise(uv * 50 + time * 3) * _HealthDamageNoise * damage * m;\n"
                + "              tinted += dmgNoise * tintColor; }\n"
                + "          if(_HealthEdgeGrain > 0.01) {\n"
                + "              half minA = 1;\n"
                + "              minA = min(minA, STEX(_MainTex,sampler_MainTex,uv+float2(-1.0,-1.0)*0.01).a);\n"
                + "              minA = min(minA, STEX(_MainTex,sampler_MainTex,uv+float2( 0.0,-1.0)*0.01).a);\n"
                + "              minA = min(minA, STEX(_MainTex,sampler_MainTex,uv+float2( 1.0,-1.0)*0.01).a);\n"
                + "              minA = min(minA, STEX(_MainTex,sampler_MainTex,uv+float2(-1.0, 0.0)*0.01).a);\n"
                + "              minA = min(minA, STEX(_MainTex,sampler_MainTex,uv+float2( 1.0, 0.0)*0.01).a);\n"
                + "              minA = min(minA, STEX(_MainTex,sampler_MainTex,uv+float2(-1.0, 1.0)*0.01).a);\n"
                + "              minA = min(minA, STEX(_MainTex,sampler_MainTex,uv+float2( 0.0, 1.0)*0.01).a);\n"
                + "              minA = min(minA, STEX(_MainTex,sampler_MainTex,uv+float2( 1.0, 1.0)*0.01).a);\n"
                + "              half edgeness = col.a * (1 - minA);\n"
                + "              half grain = SimpleNoise(uv * 100 + time) * edgeness * _HealthEdgeGrain * damage * m;\n"
                + "              tinted += grain * tintColor; }\n"
                + "          col.rgb = tinted;"
            },
            new EffectDefinition {
                Keyword = "_XRaySilhouette_En",
                Body = "half m = _XRaySilhouetteBlend * eMask * SampleEffectMask(uv,masks,_XRaySilhouetteMaskCh);\n"
                + "          col.rgb = lerp(col.rgb, _XRaySilhouetteColor.rgb, m * col.a);\n"
                + "          col.a = lerp(col.a, col.a * _XRaySilhouetteColor.a, m);"
            },
            new EffectDefinition {
                Keyword = "_MinimapPing_En",
                Body = "half m = eMask * SampleEffectMask(uv,masks,_MinimapPingMaskCh);\n"
                + "          float2 mpLuv=LocalUV(uv); mpLuv.x*=_SpriteAspect; float2 mpC=half2(0.5,0.5)+_MinimapPingOffset.xy; mpC.x*=_SpriteAspect; half dist = distance(mpLuv, mpC);\n"
                + "          half ring = frac(dist - _MinimapPingSpeed * time * 0.2);\n"
                + "          half pingAlpha = 1 - AIO_SmoothStep(0, _MinimapPingWidth, abs(ring - 0.5));\n"
                + "          col.rgb += _MinimapPingColor.rgb * pingAlpha * m * col.a;"
            },
            new EffectDefinition {
                Keyword = "_Ghosting_En",
                Body = "half m = eMask * SampleEffectMask(uv,masks,_GhostingMaskCh);\n"
                + "          half4 ghost = half4(0,0,0,0);\n"
                + "          for(int gi = 1; gi <= (int)_GhostingCount; gi++) {\n"
                + "              half gfade = AIO_Pow(_GhostingFade, gi);\n"
                + "              ghost += SampleMainClean(uv - _GhostingOffset.xy * gi) * gfade; }\n"
                + "          ghost /= max(_GhostingCount, 1);\n"
                + "          col.rgb = lerp(col.rgb, col.rgb + ghost.rgb * 0.5, m);\n"
                + "          col.a = max(col.a, ghost.a * m * 0.5);"
            },
            new EffectDefinition {
                Keyword = "_Checkerboard_En",
                Body = "half m = eMask * SampleEffectMask(uv,masks,_CheckerboardMaskCh);\n"
                + "          float2 cuv = _CheckerCoordSpace < 0.5 ? uv : (_CheckerCoordSpace < 1.5 ? worldPos.xy : float2(GetScreenUV(screenPos).x * _ScreenParams.x/_ScreenParams.y, GetScreenUV(screenPos).y));\n"
                + "          float2 grid = floor(cuv*_CheckerScale);\n"
                + "          half checker = fmod(grid.x+grid.y, 2);\n"
                + "          half3 checkCol = lerp(_CheckerColor1.rgb, _CheckerColor2.rgb, checker);\n"
                + "          col.rgb = lerp(col.rgb, col.rgb * checkCol, m);"
            },
            new EffectDefinition {
                Keyword = "_RetroPalette_En",
                Body = "half m = _RetroFade * eMask * SampleEffectMask(uv,masks,_RetroMaskCh);\n"
                + "          float2 pixUV = floor(uv * _RetroPixelSize + 0.5) / _RetroPixelSize;\n"
                + "          half4 pixCol = SampleMainClean(pixUV);\n"
                + "          if (pixCol.a < 0.01) { col.a *= (1 - m); }\n"
                + "          else {\n"
                + "          half lum = dot(pixCol.rgb, half3(0.299,0.587,0.114));\n"
                + "          float2 ditherPos = floor(fmod(uv * _RetroPixelSize, 4));\n"
                + "          int didx = (int)ditherPos.x + (int)ditherPos.y * 4;\n"
                + "          half dbayer = (didx==0?0:didx==1?8:didx==2?2:didx==3?10:didx==4?12:didx==5?4:didx==6?14:didx==7?6:didx==8?3:didx==9?11:didx==10?1:didx==11?9:didx==12?15:didx==13?7:didx==14?13:5) / 16.0;\n"
                + "          half quantized = floor(lum * _RetroPaletteSize + dbayer * _RetroDitherStrength) / _RetroPaletteSize;\n"
                + "          half3 retro = STEX(_RetroPaletteTex, sampler_RetroPaletteTex, half2(quantized, 0.5)).rgb;\n"
                + "          col.rgb = lerp(col.rgb, retro, m * pixCol.a);\n"
                + "          col.a = lerp(col.a, pixCol.a, m); }"
            },
            new EffectDefinition {
                Keyword = "_UVVisualize_En",
                Body = "half m = _UVVisualizeBlend * eMask * SampleEffectMask(uv,masks,_UVVisualizeMaskCh);\n"
                + "          col.rgb = lerp(col.rgb, half3(uv.x, uv.y, 0), m);"
            },
            new EffectDefinition {
                Keyword = "_Blueprint_En",
                Body = "half m=_BlueprintFade*eMask*SampleEffectMask(uv,masks,_BlueprintMaskCh);half es=_BlueprintEdgeSz;half lw=_BlueprintLineWidth;\n"
                + "        half eH=0;half eV=0;\n"
                + "        for(int k=1;k<=3;k++){half r=es*(half)k;\n"
                + "        eH=max(eH,abs(SampleMainClean(uv+float2(r,0)).a-SampleMainClean(uv-float2(r,0)).a));\n"
                + "        eV=max(eV,abs(SampleMainClean(uv+float2(0,r)).a-SampleMainClean(uv-float2(0,r)).a));}\n"
                + "        half edge=saturate((eH+eV)*lw);half lum=dot(col.rgb,half3(0.299,0.587,0.114));\n"
                + "        half lumR=dot(SampleMainClean(uv+float2(es*2,0)).rgb,half3(0.299,0.587,0.114));\n"
                + "        half lumU=dot(SampleMainClean(uv+float2(0,es*2)).rgb,half3(0.299,0.587,0.114));edge=max(edge,saturate(abs(lum-lumR)+abs(lum-lumU))*lw);\n"
                + "        half gX=step(1.0-0.03*lw,frac(uv.x*_BlueprintGridDen));half gY=step(1.0-0.03*lw,frac(uv.y*_BlueprintGridDen));half grid=max(gX,gY)*_BlueprintGridOp;\n"
                + "        half3 bp=lerp(_BlueprintBgCol.rgb,_BlueprintGridCol.rgb,grid);bp=lerp(bp,_BlueprintLineCol.rgb,edge*_BlueprintLineInt);col.rgb=lerp(col.rgb,bp,m*col.a);"
            }
        };
    }
}