Shader "GlitchLabPro/VHSTrackingGlitchURP"
{
    Properties
    {
        _BlitTexture("Texture", 2D) = "white" {} // Source (camera color)
    }
    HLSLINCLUDE
// Include
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl" // Fullscreen blit + _BlitTexture

    TEXTURE2D(_Mask);
    SAMPLER(sampler_Mask);

    float _FadeMultiplier; // 0 = no mask, 1 = use mask
    float _adj;            // mask threshold

    #pragma shader_feature ALPHA_CHANNEL // read mask A or R

    float _Intensity;
    float _BandShift;
    float _ChromaSplit;
    float _BandWidth;
    float _BandYPos;
    float Speed;

    // Simple random 0..1 from 2 numbers
    float rand(float2 co)
    {
        return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
    }

    // Soft band on Y (1 inside, 0 outside)
    float barShape(float pos, float uvY, float range)
    {
        float edge0 = pos - range;
        float edge1 = pos + range;

        float s = smoothstep(edge0, pos, uvY) - smoothstep(pos, edge1, uvY);
        return s; 
    }

    float4 Frag(Varyings i) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i); // XR support

        float2 uv  = i.texcoord;
        float2 uv1 = i.texcoord;

        float maskW = 1.0;
        if (_FadeMultiplier > 0.0)
        {
        #if ALPHA_CHANNEL
            float m = step(_adj, SAMPLE_TEXTURE2D(_Mask, sampler_Mask, uv1).a);
        #else
            float m = step(_adj, SAMPLE_TEXTURE2D(_Mask, sampler_Mask, uv1).r);
        #endif
            maskW = lerp(1.0, m, saturate(_FadeMultiplier)); // blend mask on/off
        }

        float master = _Intensity * maskW; // final strength
        float bandShift = _BandShift * master;
        float chroma = _ChromaSplit * master;

        float t  = _Time.y;
        float tq = floor(t * Speed) * (1.0 / Speed); // step time (stable random)

        float bandMask = 0.0;

        float wide = lerp(0.06, 0.18, _BandWidth);
        float thin = lerp(0.006, 0.02, _BandWidth);

        for (float k = 0.0; k < 0.71; k += 0.1313)
        {
            float base = 0.15*_BandYPos + 0.35*_BandYPos * frac(t * 0.4);
            float d = frac(base + k * 0.11 + rand(float2(k * 9.1, tq)) * 0.05); // band Y

            float isWide = 1.0 - step(0.2, k); // first bands are wide
            float localRange = lerp(thin, wide, isWide);
            float o = (rand(float2(k * 19.3, tq)) * 2.0 - 1.0); // -1..1

            o *= bandShift * lerp(3.5, 12.0, isWide);
            float s = barShape(d, uv.y, localRange);

            uv.x += s * o; // shift inside band

            bandMask = max(bandMask, abs(s)); // remember band area
        }

        bandMask = saturate(bandMask);

        float rgb = chroma * bandMask; // chroma only in bands

        float2 offsetR = float2(0.006 * sin(t), 0.0) * rgb;
        float2 offsetG = float2(0.0073 * cos(t * 0.97), 0.0) * rgb;

        float r = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + offsetR).r;
        float g = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv - offsetG).g;
        float b = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv).b;
        float a = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv).a;

        return float4(r, g, b, a);
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
