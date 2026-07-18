Shader "GlitchLabPro/CompressionCodecGlitch"
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

    float amount;        // main strength
    float speed;         // animation speed
    float size;          // block size for noise
    float posterize_val; // base posterize steps
    float fade;          // mix amount
    float randAmount;    // chance threshold

    float _FadeMultiplier; // 0 = no mask, 1 = use mask
    float _adj;            // mask threshold
    #pragma shader_feature ALPHA_CHANNEL // read mask A or R

    int stop; // 1 = bypass


    // Random helpers
    float Rand1(float n) { return frac(sin(n) * 43758.5453123); }
    float Rand2(float2 n) { return frac(sin(dot(n, float2(12.9898, 4.1414) ) ) * 43758.5453); }

    // 1D smooth noise
    float Noise1(float p)
    {
        float fl = floor(p);
        float fc = frac(p);
        return lerp(Rand1(fl), Rand1(fl + 1.0), fc);
    }

    // 2D smooth noise
    float Noise2(float2 p)
    {
        float2 ip = floor(p);
        float2 u = frac(p);
        u = u * u * (3.0 - 2.0 * u);

        float res = lerp(
            lerp(Rand2(ip),               Rand2(ip + float2(1.0, 0.0)), u.x),
            lerp(Rand2(ip + float2(0.0, 1.0)), Rand2(ip + float2(1.0, 1.0)), u.x),
            u.y
        );
        return res * res;
    }

    // RGB <-> HSV
    float4 rgb2hsv(float4 c)
    {
        float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
        float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
        float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));
        float d = q.x - min(q.w, q.y);
        float e = 1.0e-10;
        return float4(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x, 1);
    }

    float4 hsv2rgb(float4 c)
    {
        float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
        float4 p = abs(frac(c + K) * 6.0 - K);
        return c.z * lerp(K, clamp(p - K, 0.0, 1.0), c.y);
    }

    // Snap UV to pixel center
    float2 QuantizeUV(float2 uv)
    {
        float2 res = _ScreenParams.xy;
        float2 p   = floor(uv * res);
        return (p + 0.5) / res;
    }

    float4 downsample(float2 uv)
    {
        return SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, saturate(QuantizeUV(uv)));
    }

    // Simple edge from neighbor samples
    float4 edge(float2 uv)
    {
        float2 res = _ScreenParams.xy;
        float2 p   = uv * res;
        float2 ip  = floor(p);
        float2 f   = frac(p);

        float2 uvL = (ip + float2(-1, 0) + 0.5) / res;
        float2 uvR = (ip + float2( 1, 0) + 0.5) / res;
        float2 uvD = (ip + float2( 0,-1) + 0.5) / res;
        float2 uvU = (ip + float2( 0, 1) + 0.5) / res;

        float4 hx = lerp(
            SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler,saturate(uvL)),
            SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, saturate(uvR)),
            f.x
        );

        float4 hy = lerp(
            SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, saturate(uvD)),
            SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, saturate(uvU)),
            f.y
        );

        float4 c = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uv);
        return (hx + hy) * 0.5 - c;
    }

    // Posterize color
    float4 posterize(float4 color, float steps)
    {
        steps = max(1.0, steps);
        return floor(color * steps + 1e-6) / steps;
    }

    float quantize(float n, float steps)
    {
        steps = max(1.0, steps);
        return floor(n * steps + 1e-6) / steps;
    }

    // Main distortion: edge-driven shift + jitter + HSV changes + posterize
    float4 distort(float2 uv, float Amount)
    {
        float2 pixel = 1.0 / _ScreenParams.xy;

        float4 field = rgb2hsv(edge(uv));
        float2 d = pixel * sin(field.rb * (PI * 2.0)); // small direction from edges

        float tY = _Time.y*speed;

        float shiftx = Noise2(float2(quantize(uv.y + 31.5, _ScreenParams.y / size) * tY, frac(tY) * 300.0));
        float shifty = Noise2(float2(quantize(uv.x + 11.5, _ScreenParams.x / size) * tY, frac(tY) * 100.0));

        float2 jitter = (pixel * 0.5) * float2(shiftx, shifty) * (100.0 * Amount);

        float4 rgb = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uv + (d + jitter) * Amount);

        float4 hsv = rgb2hsv(rgb);
        hsv.y = frac(hsv.y + shifty * pow(Amount, 5.0) * 0.25); // shift saturation

        float a5 = 1.0 - pow(1.0 - Amount, 5.0);

        // Dynamic posterize steps
        float steps = floor(lerp(posterize_val, pow(1.0 - hsv.z - 0.5, 2.0) * 64.0 * shiftx + 4.0, a5));

        return posterize(hsv2rgb(hsv), steps);
    }

    float4 Frag(Varyings i) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i); // XR support

        float2 uv = i.texcoord;
        float4 col = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uv);

        float stop01 = step(0.5, (float)stop);

        // Amount changes by scanlines
        float wow = Noise1(_Time.y* speed + uv.y) * 2.0 * amount - 1.0;
        float Amount = wow * amount;

        // Mask controls Amount and Fade
        float applyMask = step(1e-6, _FadeMultiplier);

        #if ALPHA_CHANNEL
            float maskSample = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, uv).a;
        #else
            float maskSample = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, uv).r;
        #endif

        float alpha_Mask = step(_adj, maskSample);

        float maskMul = lerp(1.0, alpha_Mask, applyMask);
        Amount *= maskMul;
        float Fade = fade * maskMul;

        // Random gate (on/off)
        float t = floor(_Time.x * 228.0);
        float temp_rand = Rand2(float2(t, t));
        float trigger = step(randAmount + 1e-6, temp_rand);

        float4 dcol = distort(uv, Amount);

        float4 outCol = lerp(col, dcol, Fade * trigger);

        return lerp(outCol, col, stop01);
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