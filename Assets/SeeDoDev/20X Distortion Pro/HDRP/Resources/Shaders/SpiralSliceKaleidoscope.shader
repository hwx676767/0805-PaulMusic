Shader "DistortionsPro_20X/SpiralSliceKaleidoscopeHDRP"
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
        float  _FadeMultiplier;        // enables mask‑based fading
        float  MaskThreshold;

        uint   IterationCount;         // number of spiral slices drawn
        float  SpiralScale;            // global scale of spirals
        float  SpiralRotation;         // per‑slice rotation
        float  SpiralOffset;           // centre shift amplitude
        float  NoiseWarpStrength;      // horizontal noise warp amount
        float  BlendFactor;            // lerp between original & kaleido
        float  AnimationSpeed;         // time multiplier
        half   BaseDarkening;          // factor to darken background before compositing
        float2 EyeCenterUV()
        {
            float4 clip = mul(UNITY_MATRIX_P, float4(0,0,-1,1));
            float2 ndc  = clip.xy / max(clip.w, 1e-6); // -1..1
            return ndc * 0.5 + 0.5;                    // -> UV 0..1 
        }
        // per-eye screen size (без unity_StereoScaleOffset)
        float2 EyeScreenWH()
        {
            #if defined(UNITY_SINGLE_PASS_STEREO) && !defined(STEREO_INSTANCING_ON) && !defined(STEREO_MULTIVIEW_ON)
                return float2(_ScreenParams.x * 0.5, _ScreenParams.y); // double-wide: ширина/2
            #else
                return _ScreenParams.xy; // instancing/multiview/mono
            #endif
        }

        // Noise helpers
        float Hash1(float p) { return frac(sin(p) * 10000.0); }
        float GridNoise(float2 p) { return Hash1(p.x + p.y * 10000.0); }

        float2 SW(float2 p) { return floor(p); }
        float2 SE(float2 p) { return float2(ceil(p.x), floor(p.y)); }
        float2 NW(float2 p) { return float2(floor(p.x), ceil(p.y)); }
        float2 NE(float2 p) { return ceil(p); }

        float SmoothNoise(float2 p)
        {
            float2 interp = smoothstep(0.0, 1.0, frac(p));
            float south = lerp(GridNoise(SW(p)), GridNoise(SE(p)), interp.x);
            float north = lerp(GridNoise(NW(p)), GridNoise(NE(p)), interp.x);
            return lerp(south, north, interp.y);
        }

        float FBMNoise(float2 p)
        {
            float t = _Time.y * AnimationSpeed;
            float sum = 0.0;
            sum += SmoothNoise(p - t);
            sum += SmoothNoise(p * 2.0 + t)     / 2.0;
            sum += SmoothNoise(p * 4.0 - t)     / 4.0;
            sum += SmoothNoise(p * 8.0 + t)     / 8.0;
            sum += SmoothNoise(p * 16.0 - t)    / 16.0;
            return sum * (1.0 / 1.9375);
        }

        float4 Frag(Varyings IN) : SV_Target
        {
            UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
            float2 c = EyeCenterUV();
            float2 uv     = ClampAndScaleUVForBilinearPostProcessTexture(IN.texcoord.xy);
            float2 baseUV = uv;

            // Time‑varying spiral parameters
            float dynamicScale    = SpiralScale * 0.9 + cos(_Time.y * 2.0 * AnimationSpeed) * 0.1;
            float dynamicRotation = sin(_Time.y * 0.5 * AnimationSpeed) * SpiralRotation;
            float2 dynamicOffset  = 0.25 * SpiralOffset
                                  + 0.25 * float2(cos(_Time.y * AnimationSpeed), sin(_Time.y * AnimationSpeed));

            // Background colour darkened by BaseDarkening when spiral slices are transparent
            float4 baseColor = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, baseUV);
            float4 composite = baseColor * BaseDarkening;

            // Horizontal noise warp
            uv.x += FBMNoise(uv) * NoiseWarpStrength;
            float2 eyeWH = EyeScreenWH();
            // Draw each slice iteratively
            [loop]
            for (uint idx = 0u; idx < IterationCount; ++idx)
            {
                float2 pos = uv - dynamicOffset;
                pos /= pow(abs(dynamicScale), float(idx));
                pos += dynamicOffset;

                float theta = radians(-dynamicRotation) * float(idx);
                float s = sin(theta), c = cos(theta);

                pos -= c;
                pos *= float2(_ScreenParams.x * 0.5, _ScreenParams.y);
                pos = float2(pos.x * c - pos.y * s, pos.x * s + pos.y * c);
                pos /= float2(_ScreenParams.x * 0.5, _ScreenParams.y);
                pos += c + float2(- NoiseWarpStrength * 0.5, 0);

                if (all(pos >= 0.0) && all(pos <= 1.0))
                {
                    float4 sliceCol = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, pos);
                    composite = sliceCol + (1.0 - sliceCol.a) * composite;
                }
            }

            // Mask‑based modulation of BlendFactor
            float blend = BlendFactor;
            if (_FadeMultiplier > 0.0)
            {
                float maskSample;
            #if ALPHA_CHANNEL
                maskSample = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, baseUV).a;
            #else
                maskSample = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, baseUV).r;
            #endif
                float fadeVal = smoothstep(MaskThreshold - 0.05, MaskThreshold + 0.05, maskSample);
                blend *= fadeVal;
            }

            return lerp(baseColor, composite, blend);
        }

    ENDHLSL

    SubShader
    {
        PackageRequirements
        {
            "com.unity.render-pipelines.high-definition": "0.0.0"
        }
        Name "#SpiralSliceKaleidoscope#"
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
