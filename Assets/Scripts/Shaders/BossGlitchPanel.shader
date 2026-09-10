// 实体数据面板：青灰半透明矩形 + 扫描线/网格/伪代码，用作 Boss 前后的实体方块。
Shader "DeusEx/BossGlitchPanel"
{
    Properties
    {
        _Color ("Panel Color", Color) = (0.42, 0.68, 0.74, 0.72)
        _LineColor ("Line Color", Color) = (0.82, 0.95, 0.98, 0.9)
        _AccentColor ("Accent", Color) = (0.95, 0.55, 0.28, 0.85)
        _Opacity ("Opacity", Range(0, 1)) = 0.75
        _ScanDensity ("Scan Density", Float) = 42
        _CodeDensity ("Code Density", Float) = 18
        _Seed ("Seed", Float) = 0
        _ShowAccent ("Show Accent Line", Range(0, 1)) = 0
        _Border ("Border Width", Range(0, 0.2)) = 0.04
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "IgnoreProjector" = "True"
        }

        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "BossGlitchPanel"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float4 _LineColor;
                float4 _AccentColor;
                float _Opacity;
                float _ScanDensity;
                float _CodeDensity;
                float _Seed;
                float _ShowAccent;
                float _Border;
            CBUFFER_END

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

            float hash21(float2 p)
            {
                p = frac(p * float2(123.34, 456.21) + _Seed);
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;
                float t = _Time.y;

                // 实心底色（实体感）
                half3 rgb = _Color.rgb * 0.92;
                float a = _Color.a * _Opacity;

                // 细扫描线
                float scan = sin(uv.y * _ScanDensity * 3.14159 + t * 0.8) * 0.5 + 0.5;
                scan = lerp(0.75, 1.0, scan);
                rgb *= scan;

                // 网格
                float2 g = abs(frac(uv * float2(8.0, 12.0)) - 0.5);
                float grid = 1.0 - smoothstep(0.0, 0.04, min(g.x, g.y));
                rgb = lerp(rgb, _LineColor.rgb, grid * 0.35);
                a = max(a, grid * 0.15 * _Opacity);

                // 伪代码横条（实体块内部纹理）
                float row = floor(uv.y * _CodeDensity);
                float rowOn = step(0.35, hash21(float2(row, 2.7)));
                float colLen = 0.15 + hash21(float2(row, 8.1)) * 0.7;
                float code = rowOn * step(uv.x, colLen) * step(0.08, frac(uv.y * _CodeDensity));
                // 码块分段
                float seg = step(0.55, hash21(float2(row, floor(uv.x * 20.0))));
                code *= seg;
                rgb = lerp(rgb, _LineColor.rgb, code * 0.55);
                a = max(a, code * 0.45 * _Opacity);

                // 边框加实体厚度感
                float b = _Border;
                float border = max(
                    max(step(uv.x, b), step(1.0 - b, uv.x)),
                    max(step(uv.y, b), step(1.0 - b, uv.y)));
                rgb = lerp(rgb, _LineColor.rgb, border * 0.65);
                a = max(a, border * 0.55 * _Opacity);

                // 偶发橙色竖线点缀
                float accentX = frac(0.18 + hash21(float2(_Seed, 3.3)) * 0.6);
                float accent = smoothstep(0.0, 0.008, abs(uv.x - accentX)) * smoothstep(0.02, 0.008, abs(uv.x - accentX));
                accent *= _ShowAccent;
                rgb = lerp(rgb, _AccentColor.rgb, accent);
                a = max(a, accent * _AccentColor.a);

                // 轻微时间闪烁，保持实体不消失
                a *= 0.88 + 0.12 * sin(t * (2.0 + frac(_Seed) * 3.0) + _Seed);

                return half4(rgb, saturate(a));
            }
            ENDHLSL
        }
    }
}
