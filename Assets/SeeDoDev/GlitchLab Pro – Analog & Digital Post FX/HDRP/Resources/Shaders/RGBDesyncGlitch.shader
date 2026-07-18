Shader "GlitchLabPro/RGBDesyncGlitch"
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

    float _FadeMultiplier; // 0 = no mask, 1 = use mask
    float _adj;            // mask threshold
    #pragma shader_feature ALPHA_CHANNEL // read mask A or R

    // Random 0..1
    float random2d(float2 n)
    {
        return frac(sin(dot(n, float2(12.9898, 4.1414))) * 43758.5453);
    }

    float randomRange(float2 seed, float minV, float maxV)
    {
        return minV + random2d(seed) * (maxV - minV); // random in [minV..maxV]
    }

    // 1 when v in [bottom..top), else 0
    float insideRange(float v, float bottom, float top)
    {
        return step(bottom, v) - step(top, v);
    }

    float val1 = 0.2; // slice offset size
    float val2 = 0.2; // slice count scale
    float val3 = 0.2; // single-channel offset size
    float SPEED = 0.6;

    float randAmount; // chance threshold
    float fade;       // mix amount
    int   stop;       // 1 = bypass

    float rand(float2 n)
    {
        return frac(sin(dot(n, float2(12.9898, 4.1414))) * 43758.5453);
    }

    float4 Frag(Varyings v) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(v); // XR support

        float2 uv = v.texcoord;

        float4 baseCol = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uv);

        float stop01 = step(0.5, (float)stop);

        // If _FadeMultiplier is 0, mask is ignored
        float applyMask = step(1e-6, _FadeMultiplier);

        #if ALPHA_CHANNEL
            float maskSample = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, uv).a;
        #else
            float maskSample = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, uv).r;
        #endif

        float alphaMask = step(_adj, maskSample);
        float fadeLocal = fade * lerp(1.0, alphaMask, applyMask);

        // Step time for stable random
        float time = floor(_Time.x * SPEED * 60.0);
        float maxOffset = val1 * 0.5;

        float4 outCol = baseCol;

        // Horizontal "slices" on random Y ranges
        for (float j = 0.0; j < 10.0 * val2; j += 1.0)
        {
            float sliceY  = random2d(float2(time, 2345.0 + j));
            float sliceH  = random2d(float2(time, 9035.0 + j)) * 0.25;
            float hOffset = randomRange(float2(time, 9625.0 + j), -maxOffset, maxOffset);

            float2 uvOff = uv;
            uvOff.x += hOffset;

            float inSlice = insideRange(uv.y, sliceY, frac(sliceY + sliceH));
            float4 samp = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uvOff);

            outCol = lerp(outCol, samp, inSlice);
        }

        // Offset only one color channel (R or G or B)
        float maxColOffset = val3 / 6.0;
        float rnd = random2d(float2(time, 9545.0));

        float2 colOffset = float2(
            randomRange(float2(time, 9545.0), -maxColOffset, maxColOffset),
            randomRange(float2(time, 7205.0), -maxColOffset, maxColOffset)
        );

        float4 offSamp = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uv + colOffset);

        float wR = 1.0 - step(0.33, rnd);
        float wG = step(0.33, rnd) * (1.0 - step(0.66, rnd));
        float wB = step(0.66, rnd);

        outCol.r = lerp(outCol.r, offSamp.r, wR);
        outCol.g = lerp(outCol.g, offSamp.g, wG);
        outCol.b = lerp(outCol.b, offSamp.b, wB);

        // Random gate (on/off)
        float timeS = floor(_Time.x * 228.0);
        float temp_rand = clamp(rand(float2(timeS, timeS)), 0.001, 1.0);
        float trigger = step(randAmount, temp_rand);

        float blend = fadeLocal * trigger;
        float4 result = lerp(baseCol, outCol, blend);

        return lerp(result, baseCol, stop01);
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
