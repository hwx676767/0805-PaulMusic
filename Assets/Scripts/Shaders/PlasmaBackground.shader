// 程序化蓝色流动等离子背景（URP / Forward 渲染器）。
// 用域扭曲(domain-warp)的分形噪声在屏幕空间生成不断变形的发光蓝色光纹，
// 完美无缝循环、无水印、不依赖任何贴图或视频文件。
Shader "DeusEx/PlasmaBackground"
{
    Properties
    {
        [HDR] _GlowColor ("Glow Color (亮纹)", Color) = (0.7, 0.92, 1.0, 1)
        _MidColor ("Mid Color (主蓝)", Color) = (0.06, 0.30, 0.88, 1)
        _DeepColor ("Deep Color (暗部)", Color) = (0.0, 0.02, 0.12, 1)
        _Scale ("Pattern Scale (纹理密度)", Float) = 2.2
        _Speed ("Flow Speed (流动速度)", Float) = 0.5
        _Wander ("Wander (随机游走/消除方向性)", Range(0, 3)) = 1.0
        _WarpStrength ("Warp Strength (扭曲强度)", Float) = 3.0
        _GlowSharpness ("Glow Sharpness (亮纹锐度)", Range(0.5, 4)) = 1.3
        _Brightness ("Brightness (整体亮度)", Float) = 1.0
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
            Name "PlasmaBackground"

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
                float4 screenPos : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _GlowColor;
                float4 _MidColor;
                float4 _DeepColor;
                float _Scale;
                float _Speed;
                float _Wander;
                float _WarpStrength;
                float _GlowSharpness;
                float _Brightness;
            CBUFFER_END

            float hash21(float2 p)
            {
                p = frac(p * float2(123.34, 345.45));
                p += dot(p, p + 34.345);
                return frac(p.x * p.y);
            }

            // 二维值噪声（带平滑插值）
            float vnoise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                float2 u = f * f * (3.0 - 2.0 * f);
                float a = hash21(i + float2(0.0, 0.0));
                float b = hash21(i + float2(1.0, 0.0));
                float c = hash21(i + float2(0.0, 1.0));
                float d = hash21(i + float2(1.0, 1.0));
                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
            }

            float fbm(float2 p)
            {
                float v = 0.0;
                float amp = 0.5;
                [unroll]
                for (int i = 0; i < 5; i++)
                {
                    v += amp * vnoise(p);
                    p *= 2.02;
                    amp *= 0.5;
                }
                return v;
            }

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                VertexPositionInputs vp = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionHCS = vp.positionCS;
                OUT.screenPos = vp.positionNDC;
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                // 屏幕空间 UV：背景始终铺满，不随网格 UV/视口变化而拉伸
                float2 uv = IN.screenPos.xy / max(IN.screenPos.w, 1e-5);
                float aspect = _ScreenParams.x / max(_ScreenParams.y, 1.0);

                float t = _Time.y * _Speed;

                // 以屏幕中心为轴极缓慢地旋转采样空间：消除“暗部长期固定在某一侧”的方向性偏置
                float2 c = float2((uv.x - 0.5) * aspect, uv.y - 0.5); // 居中并做宽高比校正
                float ang = t * 0.08 * _Wander;
                float ca = cos(ang), sa = sin(ang);
                c = float2(c.x * ca - c.y * sa, c.x * sa + c.y * ca);
                float2 p = c * _Scale;

                // 每层噪声用“绕圈”的二维偏移（cos/sin，且频率互不通约），
                // 方向不断变化、没有净漂移方向 —— 颜色走向因此随机且不再固定在某一侧
                float wob = 1.6 * _Wander;
                float2 m1 = float2(cos(t * 0.31),        sin(t * 0.27))        * wob;
                float2 m2 = float2(cos(t * 0.23 + 2.10), sin(t * 0.19 + 1.30)) * wob;
                float2 m3 = float2(cos(t * 0.17 + 4.00), sin(t * 0.13 + 0.70)) * wob;

                // 两级域扭曲，得到有机流动的等离子形态
                float2 q;
                q.x = fbm(p + m1);
                q.y = fbm(p + m2 + float2(5.2, 1.3));

                float2 r;
                r.x = fbm(p + _WarpStrength * q + m3 + float2(1.7, 9.2));
                r.y = fbm(p + _WarpStrength * q + m1.yx + float2(8.3, 2.8));

                float f = saturate(fbm(p + _WarpStrength * r));

                // 颜色渐变：暗蓝 -> 主蓝 -> 高亮蓝白光纹
                float3 col = lerp(_DeepColor.rgb, _MidColor.rgb, smoothstep(0.28, 0.62, f));
                float glow = pow(smoothstep(0.60, 0.95, f), _GlowSharpness);
                col = lerp(col, _GlowColor.rgb, glow);

                col *= _Brightness;
                return half4(col, 1.0);
            }
            ENDHLSL
        }
    }

    Fallback Off
}
