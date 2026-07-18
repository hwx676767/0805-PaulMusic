Shader "GlitchLabPro/BrokenSignal" 
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

    TEXTURE2D(_NoiseTex);
    SAMPLER(sampler_NoiseTex);

    sampler2D _TrashTex; // extra texture for artifacts

    float _Intensity;       // (not used here)
    float _ColorIntensity;  // controls glitch thresholds

    float _FadeMultiplier;  // 0 = no mask, 1 = use mask
    float _adj;             // mask threshold
    #pragma shader_feature ALPHA_CHANNEL // read mask A or R


    float4 Frag(Varyings i) : SV_Target
    {
        // Noise drives the glitch
        float4 glitch = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, i.texcoord);

        float thresh = 1.001 - 0 * 1.001; // constant (kept)

        // Base: where to apply UV shift
        float w_d = step(thresh * _ColorIntensity / SAMPLE_TEXTURE2D(_Mask, sampler_Mask, i.texcoord).r, pow(abs(glitch.z), 2.5));

        // Mask changes the threshold
        if (_FadeMultiplier > 0)
        {
            #if ALPHA_CHANNEL
                float alpha_Mask = step(_adj, SAMPLE_TEXTURE2D(_Mask, sampler_Mask, i.texcoord).a);
            #else
                float alpha_Mask = step(_adj, SAMPLE_TEXTURE2D(_Mask, sampler_Mask, i.texcoord).r);
            #endif
            w_d = step((1.001 - _ColorIntensity * alpha_Mask), pow(abs(glitch.z), 2.5));
        }
        else {
            w_d = step((1.001 - _ColorIntensity), pow(abs(glitch.z), 2.5));
        }

        float w_f = step(thresh, pow(abs(glitch.w), 2.5)); // blend with _TrashTex
        float w_c = step(thresh, pow(abs(glitch.z), 3.5)); // color invert

        // Move UV by noise, only where w_d is 1
        float2 uv = frac(i.texcoord + glitch.xy * w_d);

        float4 source = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uv);

        // Replace with trash texture sometimes
        float3 color = lerp(source, tex2D(_TrashTex, uv), w_f).rgb;

        // Simple negative color
        float3 neg = saturate(color.grb + (1 - dot(color, 1)) * 0.1);

        color = lerp(color, neg, w_c);

        return float4(color, source.a);
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
