Shader "GlitchLabPro/SoftRGBGlitch" 
{
    Properties
    {
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
        SAMPLER(sampler_Mask);
            float _FadeMultiplier; // 0 = no mask, 1 = use mask
        float _adj;            // mask threshold

        float AMPLITUDE = 0.8;     // shift size
        float SPEED = 0.7;         // noise speed
        float SpikeExponent = 8;   // makes noise more "spiky"
        int stop;                  // 1 = bypass
        float fade;                // mix amount
        float randAmount;          // chance threshold

        // 4 random values from 2D input
        float4 hash4(float2 p)
        {
            float4 d = float4(
                dot(p, float2(127.1, 311.7)),
                dot(p, float2(269.5, 183.3)),
                dot(p, float2(419.2, 371.9)),
                dot(p, float2(  8.3,  17.1))
            );
            return frac(sin(d) * 43758.5453);
        }

        // 256x256 binary noise (0/1), smoothed by lerp
        float4 noise256_rgbBits(float2 uv)
        {
            uv = frac(uv);             
            float2 p = uv * 256.0;     
            float2 i = floor(p);
            float2 f = frac(p);

            float4 a = step(0.5, hash4(i));
            float4 b = step(0.5, hash4(i + float2(1.0, 0.0)));
            float4 c = step(0.5, hash4(i + float2(0.0, 1.0)));
            float4 d = step(0.5, hash4(i + float2(1.0, 1.0)));

            return lerp(lerp(a, b, f.x), lerp(c, d, f.x), f.y);
        }

        // Sample RGB from different UVs, using shift as direction
        float4 rgbShift(float2 p, float4 shift) {
            float s = (2.0 * shift.w - 1.0) * 0.1; // signed scale
            float2 rs = float2( shift.x, -shift.y) * s;
            float2 gs = float2( shift.y, -shift.z) * s;
            float2 bs = float2( shift.z, -shift.x) * s;

            rs = clamp(rs, -0.05, 0.05);
            gs = clamp(gs, -0.05, 0.05);
            bs = clamp(bs, -0.05, 0.05);

            float4 r = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, p + rs);
            float4 g = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, p + gs);
            float4 b = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, p + bs);

            return float4(r.x, g.y, b.z, (r.a+g.a+b.a)/3); // take R,G,B from samples
        }

        // Simple random 0..1
        float rand(float2 n)
        {
            return frac(sin(dot(n, float2(12.9898, 4.1414))) * 43758.5453);
        }

        float4 Frag(Varyings input) : SV_Target
        {
            UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
            float2 uv = input.texcoord;
            if (stop == 1)return SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uv); // no effect

            // Time-based noise for shift
            float4 n = noise256_rgbBits(float2(SPEED * _Time.x, 2.0 * SPEED * _Time.x / 25.0));
            float3 xyz = pow(n.rgb, SpikeExponent) * AMPLITUDE;
            float4 shift = float4(xyz, n.a);

            // Mask reduces fade to 0 in blocked areas
            if (_FadeMultiplier > 0)
		     {
                    #if ALPHA_CHANNEL
                            float alpha_Mask = step(_adj, SAMPLE_TEXTURE2D(_Mask, sampler_Mask, input.texcoord).a);
                    #else
                            float alpha_Mask = step(_adj, SAMPLE_TEXTURE2D(_Mask, sampler_Mask, input.texcoord).r);
                    #endif
				    fade *= alpha_Mask;
		     }

            // Random gate (updates in steps)
            float time = floor(_Time.x * 228);
            float temp_rand = rand(floor(_Time.x * 228.0).xx);

            float4 col = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uv);
            if (temp_rand > randAmount) {
                return lerp(col, rgbShift(uv, shift), fade);
            }
            else {
                return col;
            }
        }

    ENDHLSL

    SubShader
    {
            PackageRequirements
    {
        "com.unity.render-pipelines.high-definition": "0.0.0"
    }
		Cull Off ZWrite On ZTest Always Lighting On

        Pass
        {
            HLSLPROGRAM

                #pragma vertex Vert
                #pragma fragment Frag

            ENDHLSL
        }
    }
        FallBack "Diffuse"
}
