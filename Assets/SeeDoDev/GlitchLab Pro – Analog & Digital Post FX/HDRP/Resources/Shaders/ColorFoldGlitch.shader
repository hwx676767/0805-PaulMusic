Shader "GlitchLabPro/ColorFoldGlitch"
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

    uniform float Speed;      // (not used here)
    uniform float fade;       // mix amount
    uniform float randAmount; // chance threshold
    uniform int   stop;       // 1 = bypass
    uniform float m_offset;   // UV offset size

    // Random 0..1
    float rand(float n)
    {
        return frac(sin(n) * 43758.5453123);
    }

    // Repeat 0..2
    float mod2(float x)
    {
        return frac(x * 0.5) * 2.0;
    }

    float4 Frag(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input); // XR support

        float2 uv = input.texcoord;

        float4 color = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uv);
        if (stop == 1) return color;

        float localFade = fade;

        // Mask reduces localFade to 0 in blocked areas
        if (_FadeMultiplier > 0)
        {
        #if ALPHA_CHANNEL
            float alpha_Mask = step(_adj, SAMPLE_TEXTURE2D(_Mask, sampler_Mask, uv).a);
        #else
            float alpha_Mask = step(_adj, SAMPLE_TEXTURE2D(_Mask, sampler_Mask, uv).r);
        #endif
            localFade *= alpha_Mask;
        }

        // Offset comes from current pixel color (R,B)
        float2 offset = color.xz * 2.0 - 1.0;

        float2 uvOff = uv + offset * m_offset;
        uvOff = clamp(uvOff, 0.0, 1.0);

        float4 color2 = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uvOff);

        // Safe divide
        const float eps = 1e-4;
        float4 denom = max(color2, eps);

        float4 ratio = color / denom;

        // Fold values into 0..2 range
        float4 m = float4(
            mod2(ratio.x),
            mod2(ratio.y),
            mod2(ratio.z),
            mod2(ratio.w)
        );

        float time = floor(_Time.x * 228);
        float temp_rand = rand(time);

        // 1 when temp_rand >= randAmount
        float glitchOn = step(randAmount, temp_rand);

        float4 glitched = color * m;

        return lerp(color, glitched, localFade * glitchOn);
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
