Shader "DistortionsPro_20X/VortexPixelShatterHDRP"
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
        // textures
        TEXTURE2D(_Mask);
        SAMPLER  (sampler_Mask);

        // uniforms
        float _FadeMultiplier;      // enables mask blend
        float MaskThreshold;

        float PixelateStrength;
        float2 ShatterCenter;

        float2 EyeCenterUV()
        {
            float4 clip = mul(UNITY_MATRIX_P, float4(0,0,-1,1));
            float2 ndc  = clip.xy / max(clip.w, 1e-6); // -1..1
            return ndc * 0.5 + 0.5;                    // -> UV 0..1 
        }

        float4 Frag(Varyings IN) : SV_Target
        {
            UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

            float2 uv      = ClampAndScaleUVForBilinearPostProcessTexture(IN.texcoord.xy);
            float2 c  = EyeCenterUV();
            float2 baseUV  = uv;
            float  blend   = 1.0;

            // Distance from effect centre
            float distFromCentre = length(uv - c - ShatterCenter);

            // optional mask modulation
            if (_FadeMultiplier > 0.0)
            {
                float maskVal = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, baseUV).r;
            #ifdef ALPHA_CHANNEL
                maskVal = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, baseUV).a;
            #endif
                blend *= smoothstep(MaskThreshold - 0.05, MaskThreshold + 0.05, maskVal);
            }

            // pixelation clarity factor grows toward centre
            float clarity = (1.01 - PixelateStrength) * 100.0;
            clarity /= min(distFromCentre, 1.0);

            // snap UVs to grid for shatter effect
            float2 shattered = floor(uv * clarity) / clarity;

            float4 originalCol = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, baseUV);
            float4 shatteredCol = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, shattered);
            return lerp(originalCol, shatteredCol, blend);
        }


    ENDHLSL

    SubShader
    {
        PackageRequirements
        {
            "com.unity.render-pipelines.high-definition": "0.0.0"
        }
        Name "#VortexPixelShatter#"
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            HLSLPROGRAM
                #pragma multi_compile _ STEREO_INSTANCING_ON STEREO_MULTIVIEW_ON UNITY_SINGLE_PASS_STEREO
                #pragma vertex Vert
                #pragma fragment Frag
            ENDHLSL
        }
    }
}
