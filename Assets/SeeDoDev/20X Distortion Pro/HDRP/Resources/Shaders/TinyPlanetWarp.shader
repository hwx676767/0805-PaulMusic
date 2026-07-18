Shader "DistortionsPro_20X/TinyPlanetWarpHDRP"
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

    float _Strength;
    TEXTURE2D_X(_MainTex);

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


    float4 Frag(Varyings i) : SV_Target
    {
        const float long0 = 0.525333333333;
        const float lat1  = 0.70666666666;
        const float r     = 0.17;

        const float latm  = 0.31830988618379069; // 1/pi
        const float longm = 0.15915494309189535; // 1/(2*pi)
        const float lato  = -1.5707963267948966; // -pi/2
        const float longo = -3.1415926535897932; // -pi

        float s = saturate(_Strength);

        float2 res = _ScreenParams.xy;
        float2 center = float2(0.5, 0.5);

        // aspect-correct coords for projection math
        float2 coords = i.texcoord.xy;
        coords.x = (coords.x - 0.5) * (res.x / res.y) + 0.5;

        float2 d = coords - center;
        float pLen = length(d);
        float p = max(pLen, 1e-6);

        float c = 2.0 * atan2(p, 2.0 * r);

        float longg = long0 + atan2(d.x * sin(c),
                                    p * cos(lat1) * cos(c) - d.y * sin(lat1) * sin(c));

        float lat = asin(cos(c) * sin(lat1) + (d.y * sin(c) * cos(lat1)) / p);

        // tinyplanet uv (lon periodic, lat clamped)
        float uTiny = frac((longg - longo) * longm);
        float vTiny = saturate((lat   - lato ) * latm);

        float2 uvTiny01 = float2(uTiny, vTiny);

        // ✅ strength controls warp amount (UV), not color fade
        float2 uvWarp = lerp(i.texcoord.xy, uvTiny01, s);

        float u = frac(uvWarp.x);
        float v = saturate(uvWarp.y);

        // ----------------------------
        // Seam blend around u=0/1 (scaled by strength)
        // ----------------------------
        float seamBand = 0.030 * s;                 // 0..0.03
        float du = min(u, 1.0 - u);
        float seamT = smoothstep(0.0, seamBand, du);

        float2 uvA = float2(u, v);
        float2 uvB = float2(1.0 - u, v);

        float3 colA = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uvA).rgb;
        float3 colB = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uvB).rgb;
        float3 colSeam = lerp(colB, colA, seamT);

        // ----------------------------
        // Pole fix (top/bottom): average longitudes near poles
        // ----------------------------
        float poleBand = 0.035 * s;                 // 0..0.035
        float poleTop  = smoothstep(1.0 - poleBand, 1.0, v);
        float poleBot  = smoothstep(poleBand, 0.0, v);
        float poleW    = saturate(poleTop + poleBot);

        float3 p0 = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, float2(frac(u + 0.00), v)).rgb;
        float3 p1 = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, float2(frac(u + 0.25), v)).rgb;
        float3 p2 = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, float2(frac(u + 0.50), v)).rgb;
        float3 p3 = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, float2(frac(u + 0.75), v)).rgb;
        float3 colPole = (p0 + p1 + p2 + p3) * 0.25;

        float3 col = lerp(colSeam, colPole, poleW);
        return float4(col, 1.0);
    }
    ENDHLSL

    SubShader
    {
                PackageRequirements
        {
        "com.unity.render-pipelines.high-definition": "0.0.0"
        }

        Name "#TinyPlanetWarp#"
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