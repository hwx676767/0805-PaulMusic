Shader "GlitchLabPro/DataGlitch" 
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
        #pragma shader_feature ALPHA_CHANNEL // read mask A or R

        float speed;      // random step speed
        float blockSize;  // block grid size
        float maxOffsetX; // max X shift
        float maxOffsetY; // max Y shift
        float amount;     // final strength
        float randAmount; // chance threshold

        // Random 0..1
        float Rand2(float2 n) { return frac(sin(dot(n, float2(12.9898, 4.1414) ) ) * 43758.5453); }

        // Random 0..1, changes in steps by time*speed
        float rand(float2 seed)
        {
            return frac(sin(dot(seed * floor(_Time.y * speed), float2(127.1, 311.7))) * 43758.5453123);
        }

         float rand(float seed)
         {
            return rand(float2(seed, 1.0));
         }

        float4 Frag(Varyings i) : SV_Target
        {
            UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
            float2 uv = i.texcoord;

            // Random trigger (0 or 1)
            float t = floor(_Time.x * 228.0);
            float temp_rand = Rand2(float2(t, t));
            float trigger = step((1-randAmount), temp_rand);

            float4 src = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uv);

            // Mask reduces offsets and block size
            if (_FadeMultiplier > 0)
            {
                #if ALPHA_CHANNEL
                            float alpha_Mask = step(_adj, SAMPLE_TEXTURE2D(_Mask, sampler_Mask, i.texcoord).a);
                #else
                            float alpha_Mask = step(_adj, SAMPLE_TEXTURE2D(_Mask, sampler_Mask, i.texcoord).r);
                #endif
                maxOffsetX *= alpha_Mask;
                maxOffsetY *= alpha_Mask;
                blockSize *= alpha_Mask; // note: can become 0
            }

            // One random value per block
            float2 block = rand(floor(i.texcoord * blockSize));

            float OffsetX = pow(block.x, 8.0) * pow(block.x, 3.0) - pow(rand(7.2341), 17.0) * maxOffsetX;
            float OffsetY = pow(block.x, 8.0) * pow(block.x, 3.0) - pow(rand(7.2341), 17.0) * maxOffsetY;

            // Take R/G/B from different shifted samples
            float4 r = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uv);
            float4 g = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uv + half2(OffsetX * 0.05 * rand(7.0),  OffsetY * 0.05 * rand(12.0)) * amount);
            float4 b = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uv - half2(OffsetX * 0.05 * rand(13.0), OffsetY * 0.05 * rand(12.0)) * amount);

            float4 glitchCol = half4(r.r, g.g, b.b, (r.a + g.a + b.a));

            return lerp(src, glitchCol, trigger);
        }

    ENDHLSL

    SubShader
    {
            PackageRequirements
    {
        "com.unity.render-pipelines.high-definition": "0.0.0"
    }
        Cull Off ZWrite Off ZTest Always

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
