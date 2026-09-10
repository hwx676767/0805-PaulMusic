// 乌云翻滚雾气：多层域扭曲噪声，适合大面积缓慢涌动的暗云/雾墙。
// 比粒子更适合“云层翻涌”；粒子可另加作边缘碎雾点缀。
Shader "DeusEx/RollingStormFog"
{
    Properties
    {
        _FogColorDeep ("Deep Cloud Color", Color) = (0.05, 0.06, 0.09, 0.85)
        _FogColorMid ("Mid Cloud Color", Color) = (0.14, 0.16, 0.22, 0.55)
        _FogColorEdge ("Edge Mist Color", Color) = (0.35, 0.38, 0.45, 0.22)

        _Coverage ("Coverage", Range(0, 1.5)) = 0.72
        _Density ("Density Softness", Range(0.2, 4)) = 1.35
        _Contrast ("Contrast", Range(0.5, 3)) = 1.45
        _Opacity ("Overall Opacity", Range(0, 1.5)) = 1.0

        _NoiseScale ("Noise Scale", Float) = 1.65
        _WarpStrength ("Domain Warp", Range(0, 1.5)) = 0.55
        _Scroll ("Scroll Speed XY", Vector) = (0.035, 0.012, 0, 0)
        _Billow ("Billow Speed", Float) = 0.18
        _LayerOffset ("Layer UV Offset", Float) = 0

        _VerticalBias ("Bottom Heavy", Range(-1, 1)) = 0.25
        _EdgeFade ("Screen Edge Fade", Range(0, 0.45)) = 0.08
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
                float _NoiseScale;
                float _WarpStrength;
                float4 _Scroll;
                float _Billow;
                float _LayerOffset;
                float _VerticalBias;
                float _EdgeFade;
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

                // 垂直加权：偏底部更浓（乌云压境感）
                float yBias = saturate(1.0 - uv.y + _VerticalBias * 0.35);

                float2 scroll = _Scroll.xy * t + float2(_LayerOffset * 1.7, _LayerOffset * 0.9);
                float2 p = (uv + scroll) * _NoiseScale;

                // 域扭曲：云团内部翻涌
                float2 warp = float2(
                    fbm(p + float2(0.0, t * _Billow * 0.35)),
                    fbm(p + float2(5.2, -t * _Billow * 0.28)));
                p += (warp - 0.5) * _WarpStrength * 2.0;

                float n1 = fbm(p);
                float n2 = fbm(p * 1.7 + float2(t * _Billow * 0.15, 3.1));
                float n3 = fbm(p * 0.55 - float2(t * 0.02, t * 0.01));

                float cloud = n1 * 0.55 + n2 * 0.28 + n3 * 0.17;
                cloud = pow(saturate(cloud), _Contrast);
                cloud *= lerp(0.75, 1.15, yBias);

                float mask = saturate((cloud - (1.0 - _Coverage)) * _Density);
                mask = smoothstep(0.0, 1.0, mask);

                // 边缘更淡的雾丝
                float edge = saturate((cloud - 0.25) * 1.8);
                float3 col = lerp(_FogColorEdge.rgb, _FogColorMid.rgb, saturate(mask * 1.2));
                col = lerp(col, _FogColorDeep.rgb, saturate(mask * mask * 1.4));

                float alpha = mask * _Opacity;
                alpha = max(alpha, edge * _FogColorEdge.a * _Opacity * 0.45);
                alpha *= lerp(_FogColorEdge.a, _FogColorDeep.a, mask);

                // 屏幕四周轻微淡出，避免硬边
                float2 e = smoothstep(0.0, _EdgeFade, uv) * smoothstep(0.0, _EdgeFade, 1.0 - uv);
                alpha *= e.x * e.y;

                return half4(col, saturate(alpha));
            }
            ENDHLSL
        }
    }
    FallBack Off
}
