Shader "DistortionsPro_20X/RecursiveVortexWarpHDRP"
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
        SAMPLER  (sampler_Mask);

        // parameters
        float _FadeMultiplier;       // enable mask-driven blend if > 0
        float maskThreshold;         // centre of smoothstep range
        float animTime;              // drives vortex evolution
        float iterationCount;        // depth of recursive loop
        float blendFactor;           // final mix amount
        float2 EyeCenterUV()
        {
            
            float4 clip = mul(UNITY_MATRIX_P, float4(0,0,-1,1));
            float2 ndc  = clip.xy / max(clip.w, 1e-6); // -1..1
            return ndc * 0.5 + 0.5;                    // -> UV 0..1 
        }
        // Fragment shader
        float4 Frag(Varyings IN) : SV_Target
        {
            UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

            // Use texture coordinate provided by Blit pipeline
            float2 uv = ClampAndScaleUVForBilinearPostProcessTexture(IN.texcoord.xy);
            float2 c = EyeCenterUV();
            // Normalised coordinates centred on (0,0) and stretched slightly for dramatic swirl
            float2 centredCoord = (uv - c) * 1.5;
            centredCoord = centredCoord.yx * 1.5; // rotate 90° then scale for asymmetry

            // Recursive complex-plane iteration variable
            float2 accumCoord = float2(0.0, 0.0);

            // Phase term varies with time and iteration depth, creating inward-spiralling motion
            float phaseSteps = iterationCount;
            float phase = sin(animTime / phaseSteps) * phaseSteps;
            accumCoord = centredCoord * phase;

            // Iterate a simple z = sin(z^2 + c) style fractal (Julia-like)
            [loop]
            for (int idx = 0; idx < (int)iterationCount; ++idx)
            {
                float2 zSquared = float2(accumCoord.x * accumCoord.x - accumCoord.y * accumCoord.y,
                                         accumCoord.x * accumCoord.y);
                accumCoord = sin(zSquared + centredCoord);
            }

            // Sample original image and warped coordinate image
            float4 baseCol   = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uv); // replaced MainTex with BlitTexture
            float2 sampleUV = c + (accumCoord.yx * 0.5);   
            sampleUV = clamp(sampleUV, 0.0, 1.0);
            float4 vortexCol = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, sampleUV);
            // Optional mask-controlled blend attenuation
            if (_FadeMultiplier > 0.0)
            {
                float maskSample;
            #if ALPHA_CHANNEL
                maskSample = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, uv).a;
            #else
                maskSample = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, uv).r;
            #endif
                float fadeVal = smoothstep(maskThreshold - 0.05, maskThreshold + 0.05, maskSample);
                blendFactor *= fadeVal;
            }

            return lerp(baseCol, vortexCol, blendFactor);
        }

    ENDHLSL

    SubShader
    {
        PackageRequirements
        {
            "com.unity.render-pipelines.high-definition": "0.0.0"
        }
        Name "#RecursiveVortexWarp#"
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
