// 半调网点氛围背景：参考「白底 + 左侧网点云雾 + 右侧网点远山」风格。
// 程序化、无贴图依赖，可在材质上调左右强度与网点密度。
Shader "DeusEx/HalftoneAtmosphere"
{
    Properties
    {
        _PaperColor ("Paper Color (底色)", Color) = (0.97, 0.97, 0.98, 1)
        _MistColor ("Mist Color (左云雾)", Color) = (0.52, 0.60, 0.70, 1)
        _LandColor ("Land Color (右远山)", Color) = (0.72, 0.75, 0.78, 1)
        _ShardColor ("Shard Color (碎屑粒子)", Color) = (0.45, 0.58, 0.72, 1)

        _MistStrength ("Mist Strength", Range(0, 1.5)) = 1.0
        _LandStrength ("Land Strength", Range(0, 1.5)) = 0.85
        _ShardStrength ("Shard Strength", Range(0, 1)) = 0.55
        _ScanlineStrength ("Scanline Strength", Range(0, 1)) = 0.18

        _DotDensity ("Halftone Density (网点密度)", Float) = 95
        _MistScale ("Mist Noise Scale", Float) = 2.4
        _MistSpeed ("Mist Drift Speed", Float) = 0.08
        _ShardSpeed ("Shard Drift Speed", Float) = 0.22
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Background+10"
        }

        Cull Off
        ZWrite Off
        ZTest LEqual

        Pass
        {
            Name "HalftoneAtmosphere"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float4 screenPos : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _PaperColor;
                float4 _MistColor;
                float4 _LandColor;
                float4 _ShardColor;
                float _MistStrength;
                float _LandStrength;
                float _ShardStrength;
                float _ScanlineStrength;
                float _DotDensity;
                float _MistScale;
                float _MistSpeed;
                float _ShardSpeed;
            CBUFFER_END

            float hash21(float2 p)
            {
                p = frac(p * float2(123.34, 345.45));
                p += dot(p, p + 34.345);
                return frac(p.x * p.y);
            }

            float vnoise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                float2 u = f * f * (3.0 - 2.0 * f);
                float a = hash21(i);
                float b = hash21(i + float2(1, 0));
                float c = hash21(i + float2(0, 1));
                float d = hash21(i + float2(1, 1));
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
                    p = p * 2.03 + float2(17.1, 9.7);
                    a *= 0.5;
                }
                return v;
            }

            // 半调网点：tone 越大，网点越大（越“墨重”）
            float Halftone(float2 uv, float tone, float density)
            {
                float2 cell = uv * density;
                float2 f = frac(cell) - 0.5;
                float r = length(f);
                float radius = saturate(tone) * 0.48;
                return 1.0 - smoothstep(radius, radius + 0.035, r);
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                VertexPositionInputs vp = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionHCS = vp.positionCS;
                OUT.screenPos = vp.positionNDC;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.screenPos.xy / max(IN.screenPos.w, 1e-5);
                float aspect = _ScreenParams.x / max(_ScreenParams.y, 1.0);
                float2 p = float2((uv.x - 0.5) * aspect, uv.y - 0.5);
                float t = _Time.y;

                // -------- 左侧云雾体（有机噪声团 + 左侧重） --------
                float2 mistUv = float2(uv.x * aspect, uv.y) * _MistScale;
                mistUv += float2(t * _MistSpeed * 0.35, t * _MistSpeed * 0.12);
                float n = fbm(mistUv);
                float n2 = fbm(mistUv * 1.7 + float2(3.1, -1.4) + t * _MistSpeed * 0.2);

                // 主体偏左、略偏上；右侧快速衰减，避免盖住出牌区
                float leftBias = smoothstep(0.78, 0.05, uv.x);
                float vertical = smoothstep(0.05, 0.35, uv.y) * smoothstep(1.05, 0.45, uv.y);
                float plume = saturate(n * 0.75 + n2 * 0.45);
                plume = pow(plume, 1.15);
                float mistMask = saturate(plume * leftBias * vertical * _MistStrength);
                // 边缘更“破”，像被吹散的墨雾
                mistMask *= lerp(0.55, 1.0, smoothstep(0.2, 0.75, n2));

                float mistDots = Halftone(float2(uv.x * aspect, uv.y), mistMask, _DotDensity);
                float mistInk = mistDots * saturate(mistMask * 1.35);

                // -------- 右侧远山剪影（浅灰网点） --------
                float x = uv.x;
                float ridge =
                    0.28
                    + 0.10 * sin(x * 9.0 + 0.4)
                    + 0.07 * sin(x * 17.0 + 1.7)
                    + 0.04 * sin(x * 31.0 - 0.6)
                    + 0.05 * fbm(float2(x * 4.0, 0.2));
                // 山体落在中下偏右
                float landBand = smoothstep(0.42, 0.72, x) * smoothstep(1.05, 0.78, x);
                float landHeight = ridge * landBand;
                float landMask = saturate((landHeight - uv.y) * 6.0) * landBand * _LandStrength;
                landMask *= 0.55 + 0.45 * fbm(float2(x * 6.0, uv.y * 3.0));
                float landDots = Halftone(float2(uv.x * aspect * 0.9, uv.y), landMask * 0.85, _DotDensity * 0.85);
                float landInk = landDots * saturate(landMask);

                // -------- 右侧扫描线 / 数字噪点氛围 --------
                float rightBias = smoothstep(0.45, 0.95, uv.x);
                float scan = abs(sin(uv.y * _ScreenParams.y * 0.35 + t * 0.7));
                scan = smoothstep(0.86, 1.0, scan) * rightBias * _ScanlineStrength;

                // -------- 碎屑粒子（细长小片，向右飘） --------
                float shardAccum = 0.0;
                [unroll]
                for (int i = 0; i < 18; i++)
                {
                    float fi = (float)i;
                    float seed = hash21(float2(fi * 12.9898, fi * 78.233));
                    float seed2 = hash21(float2(fi * 41.17, fi * 7.13));

                    // 出生在左中，随时间向右上漂移并循环
                    float life = frac(t * _ShardSpeed * (0.35 + seed * 0.65) + seed);
                    float2 origin = float2(-0.15 + seed * 0.35, 0.25 + seed2 * 0.55);
                    float2 drift = float2(0.95 * life, 0.12 * sin(life * 6.28 + seed * 6.0) + 0.08 * life);
                    float2 center = origin + drift;

                    float2 d = float2((uv.x - center.x) * aspect, uv.y - center.y);
                    // 拉长成碎屑/花瓣感
                    float ang = seed * 6.2832 + life * 1.5;
                    float ca = cos(ang), sa = sin(ang);
                    d = float2(d.x * ca - d.y * sa, d.x * sa + d.y * ca);
                    d.x *= 3.2;
                    float soft = exp(-dot(d, d) * (70.0 + seed * 80.0));
                    float fade = smoothstep(0.0, 0.12, life) * smoothstep(1.0, 0.7, life);
                    shardAccum += soft * fade * (0.35 + seed2 * 0.65);
                }
                shardAccum = saturate(shardAccum) * _ShardStrength;

                // -------- 合成：白纸上叠网点墨色 --------
                float3 col = _PaperColor.rgb;
                col = lerp(col, _MistColor.rgb, mistInk);
                col = lerp(col, _LandColor.rgb, landInk);
                col = lerp(col, _ShardColor.rgb, shardAccum);
                col = lerp(col, col * 0.88, scan);

                // 极淡的全屏网点底噪，接近参考图右侧纸感
                float paperTone = 0.06 * rightBias * (0.5 + 0.5 * fbm(float2(uv.x * 2.0, uv.y)));
                float paperDots = Halftone(float2(uv.x * aspect, uv.y), paperTone, _DotDensity * 0.55);
                col = lerp(col, _LandColor.rgb, paperDots * 0.25);

                return half4(col, 1.0);
            }
            ENDHLSL
        }
    }

    Fallback Off
}
