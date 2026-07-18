Shader "DistortionsPro_20X/ZigzagJitterDisplacementHDRP"
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
        float  _FadeMultiplier;   // enable fade via mask
        float  MaskThreshold;     // mask threshold for smoothstep
        float  JitterSpeed;       // noise scroll speed
        float  NoiseScale;        // controls wavelength
        float  JitterAmount;      // displacement strength
        half   BlendFactor;       // final mix weight

        // hash helpers
        float RandHash(float n) { return frac(sin(n) * 43758.5453123); }

        // 1-D value noise with linear fade curve
        float ValueNoise(float p)
        {
            float fl = floor(p);
            float fc = frac(p);
            return lerp(RandHash(fl), RandHash(fl + 1.0), fc);
        }

        float2 EyeCenterUV()
        {
            // get eye center in UV
            float4 clip = mul(UNITY_MATRIX_P, float4(0,0,-1,1)); // project forward
            float2 ndc  = clip.xy / max(clip.w, 1e-6);           // to -1..1
            return ndc * 0.5 + 0.5;                              // to 0..1
        }

        // fragment shader
        float4 Frag(Varyings IN) : SV_Target
        {
            UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

            float2 uv     = ClampAndScaleUVForBilinearPostProcessTexture(IN.texcoord.xy);
            float2 baseUV = uv;

            float2 c = EyeCenterUV();     // current eye center in UV
            float2 e = uv - c;            // local coords relative to center

            e.y += ValueNoise(_Time.y * JitterSpeed + e.x * 50.0 / NoiseScale) * JitterAmount;
            e.x += ValueNoise(_Time.y * JitterSpeed + e.y * 25.0 / NoiseScale) * JitterAmount;

            float2 jitterUV = c + e;      // back to global UV
            jitterUV = clamp(jitterUV, 0.0, 1.0);

            // mask-controlled blend factor
            float blend = BlendFactor;
            if (_FadeMultiplier > 0.0)
            {
                float maskSample;
            #if ALPHA_CHANNEL
                maskSample = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, baseUV).a;
            #else
                maskSample = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, baseUV).r;
            #endif
                blend *= smoothstep(MaskThreshold - 0.05, MaskThreshold + 0.05, maskSample);
            }

            float4 originalCol = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, baseUV);
            float4 jitterCol   = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, jitterUV);
            return lerp(originalCol, jitterCol, blend);
        }

    ENDHLSL

    SubShader
    {
        PackageRequirements
        {
            "com.unity.render-pipelines.high-definition": "0.0.0"
        }
        Name "#ZigzagJitterDisplacement#"
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
