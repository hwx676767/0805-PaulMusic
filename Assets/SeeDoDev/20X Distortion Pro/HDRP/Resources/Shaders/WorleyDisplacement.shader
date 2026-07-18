Shader "DistortionsPro_20X/WorleyDisplacementHDRP"
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
        float  _FadeMultiplier;   // enable mask fade
        float  MaskThreshold;     // mask threshold

        float  DisplacePixels;    // pixel offset amount
        float  BlendAmount;       // lerp weight
        float  NoiseScale;        // spatial frequency
        float  FlowSpeed;         // animation speed

        // utility math
        float lengthSq(float2 v) { return dot(v, v); }

        // simple hash noise
        float HashNoise(float2 p)
        {
            return frac(sin(frac(sin(p.x) * 4313.13311) + p.y) * 3131.0011);
        }

        // eye center in UV 0..1
        float2 EyeCenterUV()
        {
            float4 clip = mul(UNITY_MATRIX_P, float4(0,0,-1,1));
            float2 ndc  = clip.xy / max(clip.w, 1e-6);
            return ndc * 0.5 + 0.5;
        }

        // single-octave Worley distance field mapped to smooth ridges
        float WorleyF1(float2 pos)
        {
            float minDist = 1e30;
            for (int ox = -1; ox <= 1; ++ox)
            {
                for (int oy = -1; oy <= 1; ++oy)
                {
                    float2 cell   = floor(pos) + float2(ox, oy);
                    float2 jitter = float2(HashNoise(cell), HashNoise(cell));
                    float distSq  = lengthSq(pos - cell - jitter);
                    minDist = min(minDist, distSq);
                }
            }
            return 3.0 * exp(-4.0 * abs(2.0 * minDist - 1.0));
        }

        // fractal Worley with animated phase
        float FractalWorley(float2 pos)
        {
            float t = _Time.y * 10.0 * FlowSpeed;
            float term = pow(WorleyF1(pos + t), 2.0);
            term *= WorleyF1(pos * 2.0 + 1.3 + t * 0.5);
            term *= WorleyF1(pos * 4.0 + 2.3 + t * 0.25);
            term *= WorleyF1(pos * 8.0 + 3.3 + t * 0.125);
            term *= WorleyF1(pos * 32.0 + 4.3 + t * 0.125);
            term *= sqrt(WorleyF1(pos * 64.0 + 5.3 + t * 0.0625));
            term *= sqrt(sqrt(WorleyF1(pos * 128.0 + 7.3)));
            return sqrt(sqrt(sqrt(term))); // strong falloff
        }

        // fragment shader
        float4 Frag(Varyings IN) : SV_Target
        {
            UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

            float2 uv     = ClampAndScaleUVForBilinearPostProcessTexture(IN.texcoord.xy);
            float2 baseUV = uv;

            // mask-controlled blend
            float blend = BlendAmount;
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

            float2 c = EyeCenterUV();     // current eye center
            float2 e = uv - c;            // eye-space coords (local from center)

            // Worley in eye-space pixels (via _ScreenParams)
            float2 ePx      = e * _ScreenParams.xy;
            float  worleyVal = FractalWorley(ePx / (5000.0 * NoiseScale));

            // radial falloff around eye center (same as abs(2*uv-1))
            worleyVal *= exp(-4.0 * lengthSq(e));

            // offset in the same space (keep your units)
            float2 offset1 = float2(DisplacePixels * worleyVal, DisplacePixels * worleyVal);
            float2 offset2 = float2(DisplacePixels, DisplacePixels) / _ScreenParams.xy;
            e += offset1 - offset2;
            e.y -= DisplacePixels * 0.12;

            // back to global UV
            float2 warpedUV = clamp(c + e, 0.0, 1.0);

            // samples
            float4 baseCol   = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, baseUV);
            float4 warpedCol = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, warpedUV);
            return lerp(baseCol, warpedCol, blend);
        }

    ENDHLSL

    SubShader
    {
        PackageRequirements
        {
            "com.unity.render-pipelines.high-definition": "0.0.0"
        }
        Name "#WorleyDisplacement#"
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
