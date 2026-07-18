Shader "DistortionsPro_20X/FracturedNoiseDisplacementHDRP"
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
        SAMPLER(sampler_Mask);

        // Mask parameters
        float  _FadeMultiplier;       // 0 = ignore mask, >0 = blend with mask
        float  MaskThreshold;         // fade edge threshold
        // Control parameters
        float  NoiseSpeed;            // animation rate of noise
        float  DisplaceMagnitude;     // maximum distortion amount
        float  BlendFactor;           // final lerp strength

        // Noise helpers
        // Simple 2D hash returning repeatable pseudo-random value in 0-1
        float HashRand(float2 st)
        {
            return frac(sin(dot(st, float2(12.9898, 78.233))) * 43758.5453123);
        }
        float2 EyeCenterUV()
        {
            // per-eye optical center in 0..1 UV
            float4 clip = mul(UNITY_MATRIX_P, float4(0,0,-1,1));
            float2 ndc  = clip.xy / max(clip.w, 1e-6); // -1..1
            return ndc * 0.5 + 0.5;                    // 0..1
        }

        // Gradient noise (classic Perlin) with Hermite interpolation
        float PerlinNoise(float2 st)
        {
            float2 cell     = floor(st);      // integer lattice coordinates
            float2 fracPart = frac(st);       // position inside cell

            // Corner pseudo-random values
            float v00 = HashRand(cell);
            float v10 = HashRand(cell + float2(1.0, 0.0));
            float v01 = HashRand(cell + float2(0.0, 1.0));
            float v11 = HashRand(cell + float2(1.0, 1.0));

            // Hermite curve for smooth interpolation
            float2 smooth = fracPart * fracPart * (3.0 - 2.0 * fracPart);

            // Bilinear blend of the four corners
            float interpX1 = lerp(v00, v10, smooth.x);
            float interpX2 = lerp(v01, v11, smooth.x);
            return lerp(interpX1, interpX2, smooth.y);
        }

        // Fragment shader
        float4 Frag(Varyings i) : SV_Target
        {
            UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

            float2 uv  = ClampAndScaleUVForBilinearPostProcessTexture(i.texcoord.xy);
            float2 c   = EyeCenterUV();
            float2 uvc = uv - c;                 // eye-local UV (same phase per eye)

            // Time-varying amplitudes for each axis
            float ampX = (sin(_Time.y * NoiseSpeed) + 1.0) * 0.5 * DisplaceMagnitude;
            float ampY = (cos(_Time.y * NoiseSpeed) + 1.0) * 0.5 * DisplaceMagnitude;

            // Offset samples using independent noise fields per axis
            float2 warpedUV = uv + float2(PerlinNoise(uvc * ampX), PerlinNoise(uvc * ampY));

            float4 warpedColor = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, warpedUV);
            float4 baseColor   = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uv);

            // Optional mask modulation
            if (_FadeMultiplier > 0.0)
            {
                float maskSample = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, uv).r;
            #if ALPHA_CHANNEL
                maskSample = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, uv).a;
            #endif
                float fadeVal = smoothstep(MaskThreshold - 0.05, MaskThreshold + 0.05, maskSample);
                BlendFactor *= fadeVal;
            }

            return lerp(baseColor, warpedColor, BlendFactor);
        }
    ENDHLSL

    SubShader
    {
        PackageRequirements
        {
            "com.unity.render-pipelines.high-definition": "0.0.0"
        }
        Name "#FracturedNoiseDisplacement#"
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
