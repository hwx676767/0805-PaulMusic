Shader "GlitchLabPro/DecoderSlipGlitch"
{
    Properties
    {
        _MainTex("Main Texture", 2DArray) = "grey" {}
    }

    HLSLINCLUDE

    #pragma target 4.5
    #pragma only_renderers d3d11 playstation xboxone xboxseries vulkan metal switch

    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/FXAA.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/RTUpscale.hlsl"

    struct Attributes
    {
        uint vertexID : SV_VertexID;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float2 texcoord   : TEXCOORD0;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    Varyings Vert(Attributes input)
    {
        Varyings output;
        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
        output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
        output.texcoord = GetFullScreenTriangleTexCoord(input.vertexID);
        return output;
    }

    TEXTURE2D_X(_MainTex);

    TEXTURE2D(_Mask);
    SAMPLER(sampler_Mask);

    float _TimeX;
    float Offset;
    float resM;

    float Speed;
    float Bands;

    float TearChance;

    float HoldChance;
    float _FadeMultiplier;
    float _adj;

    #pragma shader_feature ALPHA_CHANNEL


    float rand(float2 n)
    {
        return frac(sin(dot(n, float2(12.9898, 78.233))) * 43758.5453);
    }

    float trunc1(float x, float levels)
    {
        return floor(x * levels) / levels;
    }

    float3 rgb2yuv(float3 rgb)
    {
        float3 yuv;
        yuv.x = dot(rgb, float3(0.299, 0.587, 0.114));
        yuv.y = dot(rgb, float3(-0.14713, -0.28886, 0.436));
        yuv.z = dot(rgb, float3(0.615, -0.51499, -0.10001));
        return yuv;
    }

    float3 yuv2rgb(float3 yuv)
    {
        float3 rgb;
        rgb.r = yuv.x + yuv.z * 1.13983;
        rgb.g = yuv.x + dot(float2(-0.39465, -0.58060), yuv.yz);
        rgb.b = yuv.x + yuv.y * 2.03211;
        return rgb;
    }

    float4 Frag(Varyings i) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

        float2 uv = i.texcoord;
        float4 src = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uv);

        // ----- Mask multiplier (no runtime if) -----
        float applyMask = step(1e-6, _FadeMultiplier);

        #if ALPHA_CHANNEL
            float maskSample = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, uv).a;
        #else
            float maskSample = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, uv).r;
        #endif

        float alphaMask = step(_adj, maskSample);                // 0/1
        float maskMul   = lerp(1.0, alphaMask, applyMask);       // 1 when mask disabled, else alphaMask

        // ----- Time stepping (codec-like) -----
        float ct = trunc1(_Time.y*Speed, 4.0);                          // stable chunks like your original
        float t0 = trunc1(_Time.y*Speed, 18.0 * max(0.25, resM));       // faster stepping layer

        // ----- Per-line/band id -----
        float bands = max(1.0, Bands);
        float lineId = floor(uv.y * bands);

        // Randoms for this line
        float r0 = rand(float2(lineId + 11.0, 150.0 * ct));
        float r1 = rand(float2(lineId + 37.0, 31.0 * t0));
        float r2 = rand(float2(lineId + 91.0, 7.0  * t0));

        // Trigger masks (no runtime if)
        float tear = step(1.0 - saturate(TearChance), r0);       // 1 when tearing happens
        float hold = step(1.0 - saturate(HoldChance), r2);       // 1 when hold happens

        // Overall intensity
        float k = saturate(Offset) * maskMul;


        float2 texel = 1.0 / _ScreenParams.xy;

        // ----- Line slip (horizontal) -----
        float jitterSign = (r1 * 2.0 - 1.0);
        float slip = jitterSign * 100 * texel.x * (tear * k);

        float2 uv2 = uv;
        uv2.x = saturate(uv2.x + slip);

        // ----- Line hold (stepped Y in some bands) -----

        float heldY = floor(uv.y * 28) / 28;                    // line repeat / hold
        float holdMix = hold * k;
        uv2.y = lerp(uv2.y, heldY, holdMix);

        // Sample after spatial corruption
        float4 s = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uv2);

        // ----- YUV “decoder” corruption -----
        float3 yuv = rgb2yuv(s.rgb);

        // Luma posterize only during tear (keeps normal image most of the time)
        float lumaMix = tear * k;


        // Chroma rotate during tear
        float ang = (jitterSign) * 1.25 * lumaMix;
        float sn, cs;
        sincos(ang, sn, cs);
        float2 uvCh = yuv.yz;
        yuv.y = uvCh.x * cs - uvCh.y * sn;
        yuv.z = uvCh.x * sn + uvCh.y * cs;

        // Chroma boost / “overdecode”
        float cb = 1.0 + 2 * lumaMix;
        yuv.yz *= cb;

        float3 outRgb = yuv2rgb(yuv);
        float4 fx = float4(outRgb, s.a);

        // Blend
        float f = maskMul;
        return lerp(src, fx, f);
    }

    ENDHLSL

    SubShader
    {
            PackageRequirements
    {
        "com.unity.render-pipelines.high-definition": "0.0.0"
    }
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            ENDHLSL
        }
    }
            FallBack "Diffuse"

}
