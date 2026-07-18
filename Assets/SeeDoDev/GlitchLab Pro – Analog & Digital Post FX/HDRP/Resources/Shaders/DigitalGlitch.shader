Shader "GlitchLabPro/DigitalGlitch"
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

    uniform float _TimeX; // custom time
    uniform float Offset1; // shift enable
    uniform float Amount;  // threshold
    uniform float resM;    // scale
    float randAmount;      // chance (not used in output)

    // Random 0..1
    float rand(float2 n) { return frac(sin(dot(n.xy, float2(12.9898, 78.233))) * 43758.5453); }

    // Quantize
    float trunc(float x, float num_levels) { return floor(x * num_levels) / num_levels; }
    float2 trunc(float2 x, float2 num_levels) { return floor(x * num_levels) / num_levels; }

    // RGB <-> YUV
    float3 rgb2yuv(float3 rgb)
    {
        float3 yuv;
        yuv.x = dot(rgb, float3(0.299, 0.587, 0.114));
        yuv.y = dot(rgb, float3(-0.14713, -0.28886, 0.436));
        yuv.z = dot(rgb, float3(0.615, -0.51499, -0.10001));
        return yuv;
    }

    float3 yuv2rgb(float3 yuv)
    {
        float3 rgb;
        rgb.r = yuv.x + yuv.z * 1.13983;
        rgb.g = yuv.x + dot(float2(-0.39465, -0.58060), yuv.yz);
        rgb.b = yuv.x + yuv.y * 2.03211;
        return rgb;
    }

    float4 Frag(Varyings i) : SV_Target
    {
        float _TimeX_s = _TimeX;
        float2 uv = i.texcoord.xy;

        // Random trigger (computed but not used)
        float tt = floor(_Time.x * 228.0);
        float temp_rand = rand(float2(tt, tt));
        float trigger = step((1-randAmount), temp_rand);

        // Mask can disable effect by making Amount very big
        float applyMask = step(1e-6, _FadeMultiplier);

        #if ALPHA_CHANNEL
            float maskSample = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, i.texcoord).a;
        #else
            float maskSample = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, i.texcoord).r;
        #endif

        float alpha_Mask = step(_adj, maskSample); // 0 or 1

        float maskedOff = applyMask * (1.0 - alpha_Mask);
        float AmountLocal = lerp(Amount, 1e9, maskedOff);

        float ct = trunc(_TimeX_s, 4.0);

        float change_rnd = rand(trunc(uv.yy, float2(16,16)) + 150.0 * ct);

        float tf = max(1e-3, 24.0 * change_rnd);

        float t = 12.0 * trunc(_TimeX_s, tf);

        // Vertical block random, then map to [-1..1]
        float vt_rnd = 0.5 * rand(trunc(uv.yy + t, float2(11 * resM, 11 * resM)));
        vt_rnd += 0.5 * rand(trunc(uv.yy + t, float2(7,7)));
        vt_rnd = vt_rnd * 2.0 - 1.0;

        // Keep only strong values (AmountLocal is threshold)
        vt_rnd = sign(vt_rnd) * saturate((abs(vt_rnd) - AmountLocal) / 0.4);
        vt_rnd = lerp(0.0, vt_rnd, Offset1);

        float2 uv_nm = uv;
        uv_nm = saturate(uv_nm + float2(0.1 * vt_rnd, 0.0)); // X shift

        float rn = trunc(_TimeX_s, 2.0);
        float rnd = rand(float2(rn, rn)); // not used

        float4 sample = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uv_nm);

        // Do color shift in YUV
        float3 sample_yuv = rgb2yuv(sample.rgb);
        sample_yuv.y /= 1.0 - 3.0 * abs(vt_rnd) * saturate(0.5 - vt_rnd);
        sample_yuv.z += 0.125 * vt_rnd * saturate(vt_rnd - 0.5);

        float4 fx  = float4(yuv2rgb(sample_yuv), (sample_yuv.r+sample_yuv.g+sample_yuv.b)/3);

        return fx;
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
