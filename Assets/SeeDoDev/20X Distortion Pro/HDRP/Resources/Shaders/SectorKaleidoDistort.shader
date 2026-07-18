Shader "DistortionsPro_20X/SectorKaleidoDistortHDRP"
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
        TEXTURE2D(_Mask);
        SAMPLER  (sampler_Mask);
        SAMPLER  (sampler_linear_repeat);

        float  _FadeMultiplier;
        float  MaskThreshold;
        float  InnerRadius;
        float  OuterRadius;
        float  BlendFactor;
        float  SectorIntensity;
        float  FillRatio;
        float  AddBlack;

        // Per-eye optical center (projects forward (0,0,-1) into UV 0..1)
        float2 EyeCenterUV()
        {
            float4 clip = mul(UNITY_MATRIX_P, float4(0,0,-1,1));
            float2 ndc  = clip.xy / max(clip.w, 1e-6);
            return ndc * 0.5 + 0.5;
        }

        float4 Frag(Varyings IN) : SV_Target
        {
            UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

            float2 uv = ClampAndScaleUVForBilinearPostProcessTexture(IN.texcoord.xy);

            // Center on current eye instead of (0.5,0.5) to avoid VR double image
            float2 centre = EyeCenterUV();

            // Shift to centre-origin and fix aspect
            float2 centredUV = uv - centre;
            centredUV.x     *= _ScreenParams.x / _ScreenParams.y;

            // Polar coords
            float ANGLE_WRAP   = acos(-FillRatio);
            float polarAngle   = atan2(centredUV.y, centredUV.x);
            float radius       = length(centredUV);

            // Sector ring mask
            float sectorMask = smoothstep(InnerRadius - 0.001, InnerRadius + 0.001, radius);
            sectorMask      *= 1.0 - smoothstep(OuterRadius - 0.001, OuterRadius + 0.001, radius);

            // Map polar to 0–1 UV (original logic)
            float2 kaleidoUV;
            kaleidoUV.x = (polarAngle + ANGLE_WRAP) / (ANGLE_WRAP * 2.0);
            kaleidoUV.y = (radius - InnerRadius) / (OuterRadius - InnerRadius);

            // Intensity blend (original logic)
            float2 warpedUV = lerp(uv, kaleidoUV, SectorIntensity);

            // Sampling
            float4 sectorColor = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, warpedUV);
            float4 baseColor   = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uv);

            // Optional black fill
            sectorMask = saturate(sectorMask + AddBlack);
            sectorColor *= sectorMask;

            // Mask-driven BlendFactor (unchanged)
            if (_FadeMultiplier > 0.0)
            {
                float maskSample;
            #if ALPHA_CHANNEL
                maskSample = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, uv).a;
            #else
                maskSample = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, uv).r;
            #endif
                float fadeVal = smoothstep(MaskThreshold - 0.05, MaskThreshold + 0.05, maskSample);
                BlendFactor *= fadeVal;
            }

            return lerp(baseColor, sectorColor, BlendFactor);
        }
    ENDHLSL

    SubShader
    {
        PackageRequirements
        {
            "com.unity.render-pipelines.high-definition": "0.0.0"
        }
        Name "#SectorKaleidoDistort#"
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
