// 全屏卡牌增益画面：加速彩框 / 减速色差+lofi 颗粒。
Shader "DeusEx/CardFxOverlay"
{
    Properties
    {
        _MainTex ("Unused", 2D) = "white" {}
        _SpeedUp ("Speed Up Amount", Range(0, 1)) = 0
        _SlowDown ("Slow Down Amount", Range(0, 1)) = 0
        _PlayFlash ("Card Play Flash", Range(0, 1)) = 0
        _FlashColor ("Flash Color", Color) = (1, 0.92, 0.55, 1)
        _BorderWidth ("Border Width", Range(0.01, 0.25)) = 0.07
        _Chromatic ("Chromatic Strength", Range(0, 0.05)) = 0.012
        _Lofi ("Lofi Amount", Range(0, 1)) = 0.65
        _Grain ("Grain", Range(0, 1)) = 0.35
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent+100"
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "IgnoreProjector" = "True"
        }

        Cull Off
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "CardFxOverlay"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float _SpeedUp;
                float _SlowDown;
                float _PlayFlash;
                float4 _FlashColor;
                float _BorderWidth;
                float _Chromatic;
                float _Lofi;
                float _Grain;
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
                p = frac(p * float2(123.34, 456.21));
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
                half3 rgb = 0;
                float a = 0;

                // 加速：彩虹外框脉冲
                if (_SpeedUp > 0.001)
                {
                    float edge = min(min(uv.x, 1.0 - uv.x), min(uv.y, 1.0 - uv.y));
                    float border = 1.0 - smoothstep(0.0, _BorderWidth, edge);
                    border *= smoothstep(0.0, _BorderWidth * 0.35, edge + 0.002);
                    float hue = frac(uv.x * 0.7 + uv.y * 0.35 + t * 0.55);
                    half3 rainbow = half3(
                        abs(hue * 6.0 - 3.0) - 1.0,
                        2.0 - abs(hue * 6.0 - 2.0),
                        2.0 - abs(hue * 6.0 - 4.0));
                    rainbow = saturate(rainbow);
                    float pulse = 0.65 + 0.35 * sin(t * 8.0);
                    rgb += rainbow * border * _SpeedUp * pulse;
                    a = max(a, border * _SpeedUp * 0.85);

                    // 内侧轻色散描边
                    float inner = 1.0 - smoothstep(0.0, _BorderWidth * 1.4, edge);
                    rgb += half3(1, 0.2, 0.35) * inner * 0.08 * _SpeedUp;
                    rgb += half3(0.2, 0.45, 1) * inner * 0.08 * _SpeedUp;
                    a = max(a, inner * 0.12 * _SpeedUp);
                }

                // 减速：色差边 + lofi 暗角颗粒 + 轻微色块
                if (_SlowDown > 0.001)
                {
                    float edge = min(min(uv.x, 1.0 - uv.x), min(uv.y, 1.0 - uv.y));
                    float vignette = 1.0 - smoothstep(0.15, 0.55, edge);
                    float chroma = _Chromatic * 40.0 * _SlowDown;
                    // 用色差色带表现（全屏 UI 无法读后缓冲，用边缘伪色差）
                    float band = 1.0 - smoothstep(0.0, 0.12, edge);
                    rgb += half3(1.0, 0.15, 0.2) * band * 0.35 * _SlowDown;
                    rgb += half3(0.15, 0.35, 1.0) * band * 0.28 * _SlowDown;
                    a = max(a, band * 0.45 * _SlowDown);

                    float grain = (hash21(uv * float2(_ScreenParams.xy) + t) - 0.5) * 2.0;
                    float lofi = _Lofi * _SlowDown;
                    rgb += grain * _Grain * lofi * 0.35;
                    // 暗角
                    rgb *= 1.0 - vignette * 0.25 * lofi;
                    a = max(a, saturate(vignette * 0.22 * lofi + abs(grain) * 0.08 * lofi));

                    // 轻微横向撕裂线
                    float tear = step(0.97, hash21(float2(floor(uv.y * 90.0), floor(t * 6.0))));
                    rgb += tear * half3(0.8, 0.85, 1.0) * 0.15 * _SlowDown;
                    a = max(a, tear * 0.2 * _SlowDown);
                }

                // 出牌瞬间闪光
                if (_PlayFlash > 0.001)
                {
                    float flash = _PlayFlash;
                    float radial = length(uv - 0.5);
                    float ring = smoothstep(0.55, 0.2, radial) * smoothstep(0.0, 0.15, radial);
                    rgb += _FlashColor.rgb * flash * (0.25 + ring * 0.55);
                    a = max(a, flash * 0.35);
                }

                return half4(rgb, saturate(a));
            }
            ENDHLSL
        }
    }
}
