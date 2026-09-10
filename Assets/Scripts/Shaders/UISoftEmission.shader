// UI 微弱自发光：提亮不透明像素，并对细线做轻微外扩光晕。
Shader "DeusEx/UISoftEmission"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1, 1, 1, 1)

        _EmissionStrength ("Emission Strength", Range(0, 3)) = 0.55
        _EmissionColor ("Emission Color", Color) = (0.75, 0.92, 1.0, 1)
        _GlowSpread ("Glow Spread (px)", Range(0, 12)) = 3.2
        _GlowStrength ("Glow Strength", Range(0, 2)) = 0.55
        _PulseAmount ("Pulse Amount", Range(0, 0.5)) = 0.1
        _PulseSpeed ("Pulse Speed", Float) = 1.2

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
            "RenderPipeline" = "UniversalPipeline"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "UISoftEmission"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _MainTex_TexelSize;
                float4 _Color;
                float _EmissionStrength;
                float4 _EmissionColor;
                float _GlowSpread;
                float _GlowStrength;
                float _PulseAmount;
                float _PulseSpeed;
                float4 _ClipRect;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.worldPosition = IN.positionOS;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.color = IN.color * _Color;
                return OUT;
            }

            half4 SampleUI(float2 uv)
            {
                return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 baseCol = SampleUI(IN.uv) * IN.color;

                #ifdef UNITY_UI_CLIP_RECT
                half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(IN.worldPosition.xy * 2.0 - _ClipRect.xy - _ClipRect.zw)) * 1000.0);
                baseCol.a *= m.x * m.y;
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip(baseCol.a - 0.001);
                #endif

                float pulse = 1.0 + sin(_Time.y * _PulseSpeed) * _PulseAmount;
                float lum = dot(baseCol.rgb, float3(0.299, 0.587, 0.114));
                float emitMask = saturate(baseCol.a) * saturate(lum * 1.8 + 0.15);

                // 自发光提亮本体线条
                half3 emit = baseCol.rgb + _EmissionColor.rgb * (_EmissionStrength * emitMask * pulse);

                // 轻微外扩采样，形成微光晕（只在细线周围）
                float2 px = _MainTex_TexelSize.xy * _GlowSpread;
                half glowA = 0;
                glowA += SampleUI(IN.uv + float2( px.x, 0)).a;
                glowA += SampleUI(IN.uv + float2(-px.x, 0)).a;
                glowA += SampleUI(IN.uv + float2(0,  px.y)).a;
                glowA += SampleUI(IN.uv + float2(0, -px.y)).a;
                glowA += SampleUI(IN.uv + float2( px.x,  px.y)).a * 0.7;
                glowA += SampleUI(IN.uv + float2(-px.x,  px.y)).a * 0.7;
                glowA += SampleUI(IN.uv + float2( px.x, -px.y)).a * 0.7;
                glowA += SampleUI(IN.uv + float2(-px.x, -px.y)).a * 0.7;
                glowA *= 0.14;

                half3 glowRgb = _EmissionColor.rgb * glowA * _GlowStrength * pulse;
                // 光晕不盖住已有实线，只补透明处
                float halo = saturate(glowA - baseCol.a);
                half3 rgb = emit + glowRgb * (0.65 + halo);
                float a = saturate(baseCol.a + glowA * _GlowStrength * 0.55);

                return half4(rgb, a);
            }
            ENDHLSL
        }
    }
}
