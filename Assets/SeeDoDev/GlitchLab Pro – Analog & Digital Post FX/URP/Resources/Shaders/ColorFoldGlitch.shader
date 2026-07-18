Shader "GlitchLabPro/ColorFoldGlitchURP"
{
    Properties
    {
        _BlitTexture("Texture", 2D) = "white" {} // Source (camera color)
    }
    HLSLINCLUDE
// Include

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        // The Blit.hlsl file provides the vertex shader (Vert),
        // the input structure (Attributes), and the output structure (Varyings)
#include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

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

        float4 color = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv);
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

        float4 color2 = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uvOff);

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
            "com.unity.render-pipelines.universal": "0.0.0"
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
