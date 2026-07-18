Shader "GlitchLabPro/ChromaticNoise"
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

    uniform float T;        // time (from script)
    uniform float Speed;    // animation speed
    uniform float Strength; // offset strength
    uniform float Fade;     // mix amount

    // Constants for noise
    float x = 127.1;
    float angleY = 311.7;
    float y = 43758.5453123;

    uniform float Stretch = 0.02; // extra thin noise

    half mR = 0.08; // R channel shift
    half mG = 0.07; // G channel shift
    half mB = 0.0;  // B channel shift

    // Hash in [-1..1]
    float hash(float2 d)
    {
        float m = dot(d, float2(x, angleY));
        return -1.0 + 2.0 * frac(sin(m) * y);
    }

    // 2D smooth noise
    float noise(float2 d)
    {
        float2 i = floor(d);
        float2 f = frac(d);
        float2 u = f * f * (3.0 - 2.0 * f);

        return lerp(
            lerp(hash(i), hash(i + float2(1.0, 0.0)), u.x),
            lerp(hash(i + float2(0.0, 1.0)), hash(i + float2(1.0, 1.0)), u.x),
            u.y
        );
    }

    float4 Frag(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input); // XR support

        float2 uv = input.texcoord;

        // Glitch pulse 0..1
        float glitch = pow(abs(cos(T * Speed) * 1.2 + 1.0), 1.2);
        glitch = saturate(glitch);

        float2 hp = float2(0.0, uv.y); // scanline coord

        // Horizontal noise (mostly by Y)
        float nh = noise(hp * 7.0 + T * Speed * 10.0) * (noise(hp + T * Speed * 0.3) * 0.8);
        nh += noise(hp * 100.0 + T * Speed * 10.0) * Stretch;

        float rnd = hash(uv) * glitch;

        nh *= glitch + rnd;

        half shiftR = 0.08 * mR;
        half shiftG = 0.07 * mG;
        half shiftB = mB;

        // Mask controls Strength
        float applyMask = step(1e-6, _FadeMultiplier);

        #if ALPHA_CHANNEL
            float maskSample = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, uv).a;
        #else
            float maskSample = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, uv).r;
        #endif

        float alpha_Mask = step(_adj, maskSample);

        float strength = Strength * lerp(1.0, alpha_Mask, applyMask);

        // Sample each channel with small different offsets
        float4 r   = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uv + float2(nh,          shiftR) * nh * strength);
        float4 g   = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uv + float2(nh - shiftG, 0.0)    * nh * strength);
        float4 b   = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uv + float2(nh,          shiftB) * nh * strength);
        float4 src = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uv);

        float4 col = float4(r.r, g.g, b.b, src.a + r.a + g.a + b.a);

        return lerp(src, col, Fade);
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
