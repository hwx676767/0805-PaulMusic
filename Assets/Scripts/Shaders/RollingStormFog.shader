// 厚重灰云雾：从点 / 线源向外涌散，云团翻滚（非薄烟飘在纯色底上）。
Shader "DeusEx/RollingStormFog"
{
    Properties
    {
        _FogColorDeep ("Cloud Core (厚重核心)", Color) = (0.22, 0.23, 0.25, 1.0)
        _FogColorMid ("Cloud Body", Color) = (0.38, 0.39, 0.42, 0.92)
        _FogColorEdge ("Cloud Soft Edge", Color) = (0.52, 0.53, 0.56, 0.55)

        _Coverage ("Cloud Fill", Range(0.1, 1.4)) = 0.92
        _Density ("Edge Softness Inv", Range(0.5, 8)) = 2.4
        _Contrast ("Contrast", Range(0.3, 2.5)) = 0.85
        _Opacity ("Overall Opacity", Range(0, 2.5)) = 1.85
        _Haze ("Base Haze (底层厚雾)", Range(0, 1)) = 0.55

        _NoiseScale ("Cloud Scale", Float) = 1.55
        _WarpStrength ("Billow Warp", Range(0, 1.5)) = 0.85
        _ExpandSpeed ("Outward Surge Speed", Float) = 0.18
        _Billow ("Internal Billow", Float) = 0.32
        _LayerOffset ("Layer Offset", Float) = 0
        _AngularTwist ("Swirl", Range(0, 2)) = 0.45

        [Header(Source)]
        _EmitMode ("Emit Mode (0=Point 1=HLine 2=VLine)", Float) = 0
        _EmitOrigin ("Emit Origin UV", Vector) = (0.5, 0.15, 0, 0)
        _EmitRadius ("Source Core Radius", Range(0.01, 0.4)) = 0.08
        _EmitReach ("Max Reach (涌散距离)", Range(0.15, 1.5)) = 0.85
        _EmitStrength ("Source Strength", Range(0, 2.5)) = 1.45
        _EmitFalloff ("Distance Falloff", Range(0.3, 4)) = 1.1

        _BoundsSoftness ("Volume Edge Softness", Range(0.02, 0.45)) = 0.14
        _BoundsInset ("Volume Inset", Range(0, 0.35)) = 0.02
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "IgnoreProjector" = "True"
        }

        Cull Off
        ZWrite Off
        ZTest LEqual
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "RollingStormFog"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _FogColorDeep;
                float4 _FogColorMid;
                float4 _FogColorEdge;
                float _Coverage;
                float _Density;
                float _Contrast;
                float _Opacity;
                float _Haze;
                float _NoiseScale;
                float _WarpStrength;
                float _ExpandSpeed;
                float _Billow;
                float _LayerOffset;
                float _AngularTwist;
                float _EmitMode;
                float4 _EmitOrigin;
                float _EmitRadius;
                float _EmitReach;
                float _EmitStrength;
                float _EmitFalloff;
                float _BoundsSoftness;
                float _BoundsInset;
            CBUFFER_END

            float hash21(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            float vnoise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                float a = hash21(i);
                float b = hash21(i + float2(1, 0));
                float c = hash21(i + float2(0, 1));
                float d = hash21(i + float2(1, 1));
                float2 u = f * f * (3.0 - 2.0 * f);
                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
            }

            float fbm(float2 p)
            {
                float v = 0.0;
                float a = 0.5;
                [unroll]
                for (int i = 0; i < 5; i++)
                {
                    v += a * vnoise(p);
                    p = p * 2.02 + float2(17.1, 9.3);
                    a *= 0.5;
                }
                return v;
            }

            // 软云雾：大团绵密，不像细烟丝
            float cloudNoise(float2 p)
            {
                float n = fbm(p);
                float n2 = fbm(p * 1.7 + 3.1);
                float soft = n * 0.65 + n2 * 0.35;
                // 轻微 ridged 增加翻涌褶皱，仍保持云感
                float ridge = 1.0 - abs(fbm(p * 0.9 + 8.2) * 2.0 - 1.0);
                return saturate(soft * 0.78 + ridge * 0.35);
            }

            float volumeMask(float2 uv)
            {
                float2 inset = float2(_BoundsInset, _BoundsInset);
                float2 soft = float2(_BoundsSoftness, _BoundsSoftness);
                float2 a = smoothstep(inset, inset + soft, uv);
                float2 b = smoothstep(inset, inset + soft, 1.0 - uv);
                return a.x * a.y * b.x * b.y;
            }

            // 返回：到源的距离(已归一)、向外单位方向、沿源的切向坐标
            void SourceSpace(float2 uv, out float dist, out float2 outward, out float along)
            {
                float2 o = _EmitOrigin.xy;
                int mode = (int)round(_EmitMode);

                if (mode == 1)
                {
                    // 水平线：沿 X 延展，向上/下涌出
                    along = uv.x;
                    float dy = uv.y - o.y;
                    dist = abs(dy);
                    outward = float2(0.0, dy >= 0.0 ? 1.0 : -1.0);
                }
                else if (mode == 2)
                {
                    // 垂直线：沿 Y 延展，向左/右涌出
                    along = uv.y;
                    float dx = uv.x - o.x;
                    dist = abs(dx);
                    outward = float2(dx >= 0.0 ? 1.0 : -1.0, 0.0);
                }
                else
                {
                    // 点源：径向向外
                    float2 d = uv - o;
                    dist = length(d);
                    outward = dist > 1e-4 ? d / dist : float2(0.0, 1.0);
                    along = atan2(d.y, d.x);
                }
            }

            float sourceEnvelope(float dist)
            {
                float core = 1.0 - smoothstep(0.0, _EmitRadius, dist);
                float body = 1.0 - smoothstep(_EmitRadius, _EmitReach, dist);
                body = pow(saturate(body), _EmitFalloff);
                return saturate(core * 0.85 + body) * _EmitStrength;
            }

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                float t = _Time.y;
                float2 uv = i.uv;

                float vol = volumeMask(uv);
                if (vol <= 0.001) return half4(0, 0, 0, 0);

                float dist;
                float2 outward;
                float along;
                SourceSpace(uv, dist, outward, along);

                float env = sourceEnvelope(dist);
                if (env <= 0.001) return half4(0, 0, 0, 0);

                // 向外涌动：采样坐标沿径向回退，看起来烟团从源冲出
                float surge = dist - t * _ExpandSpeed;
                float swirl = along * _AngularTwist + t * _Billow * 0.15;
                float2 polar = float2(along * 0.55 + swirl * 0.25, surge);
                float2 p = (polar + float2(_LayerOffset * 0.9, _LayerOffset * 1.3)) * _NoiseScale;

                // 域扭曲 → 云雾翻涌
                float2 w1 = float2(
                    cloudNoise(p + float2(0.0, t * _Billow * 0.4)),
                    cloudNoise(p + float2(5.2, -t * _Billow * 0.33)));
                p += (w1 - 0.5) * _WarpStrength * 2.2;

                float2 w2 = float2(
                    fbm(p * 1.25 + t * _Billow * 0.18),
                    fbm(p * 1.25 - t * _Billow * 0.14));
                p += (w2 - 0.5) * _WarpStrength * 0.9;

                float c1 = cloudNoise(p);
                float c2 = cloudNoise(p * 1.55 + 11.0);
                float cloud = saturate(c1 * 0.7 + c2 * 0.4);
                cloud = pow(cloud, _Contrast);

                // 厚重填充：coverage 高时接近成片云雾，仍保留翻涌结构
                float threshold = 1.0 - saturate(_Coverage);
                float detail = saturate((cloud - threshold) * _Density);
                detail = smoothstep(0.0, 1.0, detail);

                // 底层霾：靠近源头的厚雾基底，避免“白烟丝飘在底上”
                float haze = _Haze * env * (0.55 + 0.45 * cloud);
                float mask = saturate(max(detail * env, haze));
                mask *= vol;

                // 源心更实
                float coreBoost = 1.0 - smoothstep(0.0, _EmitRadius * 2.2, dist);
                mask = saturate(mask + coreBoost * 0.25 * env);

                float3 col = lerp(_FogColorEdge.rgb, _FogColorMid.rgb, saturate(mask * 1.1));
                col = lerp(col, _FogColorDeep.rgb, saturate(mask * mask * 1.25 + coreBoost * 0.35));

                float alpha = mask * _Opacity;
                alpha *= lerp(_FogColorEdge.a, _FogColorDeep.a, saturate(mask));
                alpha = saturate(alpha);

                return half4(col, alpha);
            }
            ENDHLSL
        }
    }
    FallBack Off
}
