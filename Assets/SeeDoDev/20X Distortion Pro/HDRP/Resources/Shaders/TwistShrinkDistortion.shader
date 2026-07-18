Shader "DistortionsPro_20X/TwistShrinkDistortionHDRP"
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
        // Texture bindings
        TEXTURE2D(_Mask);
        SAMPLER  (sampler_Mask);

        // Uniforms
        float _FadeMultiplier;
        float MaskThreshold;
        float BlendFactor;
        float TwistStrength;
        float LayerPeriod;
        float AnimSpeed;

        // Helpers
        #define Cos01(t) (0.5 - 0.5 * cos(t))
        float2 EyeCenterUV()
        {
            float4 clip = mul(UNITY_MATRIX_P, float4(0,0,-1,1));
            float2 ndc  = clip.xy / max(clip.w, 1e-6); // -1..1
            return ndc * 0.5 + 0.5;                    // -> UV 0..1 
        }
        // Render a twisting, shrinking layer
        float4 RenderLayer(float layerTime, float2 uv)
        {
            float ramp     = frac(layerTime / LayerPeriod);
            float envelope = Cos01(2.0 * PI * ramp);
            float2 c = EyeCenterUV();
            float angle    = TwistStrength * ramp * length(uv-c);
            float2x2 R     = float2x2(cos(angle), -sin(angle), sin(angle), cos(angle));
            float shrink   = lerp(1.0, 0.5, ramp);
            float2 warped  = mul(R, uv) * shrink;
            return envelope * SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, warped);
        }

        // Fragment shader
        float4 Frag(Varyings IN) : SV_Target
        {
            UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
            float2 uv = ClampAndScaleUVForBilinearPostProcessTexture(IN.texcoord.xy);
            float t   = _Time.y * AnimSpeed;

            float4 color = RenderLayer(t, uv);
            color += RenderLayer(t + LayerPeriod / 3.0, uv);
            color += RenderLayer(t + 2.0 * LayerPeriod / 3.0, uv);

            float blend = BlendFactor;
            if (_FadeMultiplier > 0.0)
            {
                float mask = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, uv).r;
            #if ALPHA_CHANNEL
                mask = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, uv).a;
            #endif
                blend *= smoothstep(MaskThreshold - 0.05, MaskThreshold + 0.05, mask);
            }

            float4 baseCol = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uv);
            return lerp(baseCol, color, blend);
        }

    ENDHLSL

    SubShader
    {
        PackageRequirements
        {
            "com.unity.render-pipelines.high-definition": "0.0.0"
        }
        Name "#TwistShrinkDistortion#"
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