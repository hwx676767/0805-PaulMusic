Shader "DistortionsPro_20X/DripMeltDistortionHDRP"
{
    Properties
    {
        // This property is necessary to make the CommandBuffer.Blit bind the source texture to _MainTex
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
    // Texture interfaces
    TEXTURE2D(_Mask);
    SAMPLER(sampler_Mask);

    #pragma shader_feature FOCUS_CENTER

    // External controls
    float _FadeMultiplier;    // enables mask-driven fade
    float MaskThreshold;      // threshold center for mask

    // Distortion controls
    float WaveSpeed;          // animation rate
    float DistortStrength;    // overall distortion amplitude
    float WaveScale;          // scales UV frequency
    // --- add once in HLSLINCLUDE ---
    float2 EyeCenterUV()
    {
        // current eye optical center in UV (0..1)
        float4 clip = mul(UNITY_MATRIX_P, float4(0,0,-1,1));
        float2 ndc  = clip.xy / max(clip.w, 1e-6); // -1..1
        return ndc * 0.5 + 0.5;
    }

    // Fragment – dripping melt warp
    float4 Frag(Varyings IN) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
        float2 uvBase   = ClampAndScaleUVForBilinearPostProcessTexture(IN.texcoord.xy);
        // Mask-driven strength fade
        if (_FadeMultiplier > 0.0)
        {
        #if ALPHA_CHANNEL
            float m = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, uvBase).a;
        #else
            float m = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, uvBase).r;
        #endif
            float maskVal = smoothstep(MaskThreshold - 0.05,
                                       MaskThreshold + 0.05, m);
            DistortStrength *= maskVal;
        }

        // eye-centered UV (per-eye), then to "pixel" space like before
        float2 c        = EyeCenterUV();
        float2 uve      = uvBase - c + 0.5;                  // eye-space 0..1
        float2 uvScreen = uve * _ScreenParams.xy;            // keep frequency behavior

        // time offset (unchanged)
        float2 timeOffset = float2(_Time.y * 10.0, _Time.y * 10.0) * 0.1 * WaveSpeed;

        // wave displacement (unchanged math)
        float2 displaced = uvScreen + sin(uvScreen * WaveScale - timeOffset) * (10.0 * DistortStrength);

        // back to global UV, undo eye-space shift
        float2 finalUV = displaced / _ScreenParams.xy;       // back to 0..1 eye-space
        finalUV = finalUV - 0.5 + c;                         // back to global 0..1
        finalUV = clamp(finalUV, 0.0, 1.0);

        return SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, finalUV);
    }

ENDHLSL

SubShader
{
    PackageRequirements
    {
        "com.unity.render-pipelines.high-definition": "0.0.0"
    }
    Name "#DripMeltDistortion#"
    Cull Off ZWrite Off ZTest Always
    Pass
    {
        HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
        ENDHLSL
    }
}


}
