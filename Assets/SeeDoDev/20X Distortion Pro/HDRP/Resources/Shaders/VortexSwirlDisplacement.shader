Shader "DistortionsPro_20X/VortexSwirlDisplacementHDRP"
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
        float  _FadeMultiplier;   // turn mask fade on/off
        float  MaskThreshold;     // mask level for smoothstep
        half   AnimSpeed;         // swirl speed
        half   BlendFactor;       // final mix strength
        half   IterationCount;    // number of samples

        float4 _CenterOffset;     // use x and y
        half   _RotScale;         // rotation strength
        half   _FalloffExp;       // fade by radius

        // helpers
        #define TAU 6.2831853     // 2*pi
        #define rot(a) float2x2(cos(a), -sin(a), sin(a), cos(a))

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

            float2 uv     = ClampAndScaleUVForBilinearPostProcessTexture(IN.texcoord.xy); // read UV
            float2 baseUV = uv;          // keep a copy

            // move center by offset, keep in 0..1
            float2 c = saturate(EyeCenterUV() + _CenterOffset.xy);

            // UV relative to center
            float2 centred = uv - c;
            // time in radians
            float  globalT = fmod(_Time.y * AnimSpeed, TAU);

            // sum color from many turns
            float4 swirlCol = 0.0;
            [loop]
            for (float idx = 0.0; idx < IterationCount; idx += 1.0)
            {
                // phase for this layer
                float layerPhase = globalT + TAU / 3.0 * idx;
                // weight of this layer
                float weight = (0.5 - 0.5 * cos(layerPhase)) / 1.5;

                // radius and falloff
                float r        = max(length(centred), 1e-4);
                float signTerm = (-0.5 + frac(layerPhase / TAU));
                float falloff  = pow(r, -_FalloffExp);
                // final rotation amount
                float rotationCoeff = _RotScale * signTerm * falloff;

                // rotate and sample
                float2 rotatedUV = mul(centred, rot(rotationCoeff)) + c;
                swirlCol += SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, rotatedUV) * weight;
            }

            // blend with optional mask
            float blend = BlendFactor;
            if (_FadeMultiplier > 0.0)
            {
                float maskSample;
            #if ALPHA_CHANNEL
                maskSample = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, baseUV).a; // read alpha
            #else
                maskSample = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, baseUV).r; // read red
            #endif
                // use mask to change blend
                blend *= smoothstep(MaskThreshold - 0.05, MaskThreshold + 0.05, maskSample);
            }

            // original color
            float4 originalCol = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, baseUV);
            // mix original and swirl
            return lerp(originalCol, swirlCol, blend);
        }

    ENDHLSL

    SubShader
    {
        PackageRequirements
        {
            "com.unity.render-pipelines.high-definition": "0.0.0"
        }
        Name "#VortexSwirlDisplacement#"
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
