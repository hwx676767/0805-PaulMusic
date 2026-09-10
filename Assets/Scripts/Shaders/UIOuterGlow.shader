// 仅外发光/泛光：在线条外侧做软晕，不改写原图像素颜色。
Shader "DeusEx/UIOuterGlow"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1, 1, 1, 1)

        _GlowColor ("Glow Color", Color) = (0.65, 0.88, 1.0, 1)
        _GlowStrength ("Glow Strength", Range(0, 3)) = 1.1
        _GlowSpread ("Glow Spread (px)", Range(1, 24)) = 8
        _GlowSoftness ("Glow Softness", Range(0.25, 3)) = 1.35
        _PulseAmount ("Pulse Amount", Range(0, 0.4)) = 0.08
        _PulseSpeed ("Pulse Speed", Float) = 1.1

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
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
        // Additive：只加亮，适合外发光/泛光
        Blend One One
        ColorMask [_ColorMask]

        Pass
        {
            Name "UIOuterGlow"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _MainTex_TexelSize;
                float4 _Color;
                float4 _GlowColor;
                float _GlowStrength;
                float _GlowSpread;
                float _GlowSoftness;
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

            float SampleA(float2 uv)
            {
                return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv).a;
            }

            // 多圈采样，形成外扩软晕
            float SoftDilate(float2 uv, float spreadPx)
            {
                float2 px = _MainTex_TexelSize.xy * spreadPx;
                float a = 0;
                // 十字 + 对角，两圈半径
                a += SampleA(uv + float2( px.x, 0));
                a += SampleA(uv + float2(-px.x, 0));
                a += SampleA(uv + float2(0,  px.y));
                a += SampleA(uv + float2(0, -px.y));
                a += SampleA(uv + float2( px.x,  px.y)) * 0.85;
                a += SampleA(uv + float2(-px.x,  px.y)) * 0.85;
                a += SampleA(uv + float2( px.x, -px.y)) * 0.85;
                a += SampleA(uv + float2(-px.x, -px.y)) * 0.85;

                float2 px2 = px * 1.75;
                a += SampleA(uv + float2( px2.x, 0)) * 0.55;
                a += SampleA(uv + float2(-px2.x, 0)) * 0.55;
                a += SampleA(uv + float2(0,  px2.y)) * 0.55;
                a += SampleA(uv + float2(0, -px2.y)) * 0.55;
                a += SampleA(uv + float2( px2.x,  px2.y)) * 0.4;
                a += SampleA(uv + float2(-px2.x,  px2.y)) * 0.4;
                a += SampleA(uv + float2( px2.x, -px2.y)) * 0.4;
                a += SampleA(uv + float2(-px2.x, -px2.y)) * 0.4;

                return saturate(a * 0.085);
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float core = SampleA(IN.uv);
                float dilated = SoftDilate(IN.uv, _GlowSpread);

                // 只要外侧光晕：挖掉实线本体，避免“换色”
                float ring = saturate(dilated - core * _GlowSoftness);
                ring = pow(saturate(ring), 0.85);

                float pulse = 1.0 + sin(_Time.y * _PulseSpeed) * _PulseAmount;
                float intensity = ring * _GlowStrength * pulse * IN.color.a;

                #ifdef UNITY_UI_CLIP_RECT
                half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(IN.worldPosition.xy * 2.0 - _ClipRect.xy - _ClipRect.zw)) * 1000.0);
                intensity *= m.x * m.y;
                #endif

                half3 rgb = _GlowColor.rgb * intensity;
                // Additive 输出：rgb 即贡献，a 可置 0
                return half4(rgb, 0);
            }
            ENDHLSL
        }
    }
}
