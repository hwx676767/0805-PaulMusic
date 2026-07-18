Shader "GlitchLabPro/LineShakeGlitch"
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

    float screenLinesNum;
    float jitterHAmount;
    half  speed;
    float time_;

    float _LinesAxis;
    float _ShiftH;
                float randAmount;


    float _FadeMultiplier;
    float _adj;

    #pragma shader_feature ALPHA_CHANNEL

    float Rand2(float2 n) { return frac(sin(dot(n, float2(12.9898, 4.1414) ) ) * 43758.5453); }

    float4 Frag(Varyings i) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

        float2 p = i.texcoord;
        float2 uv = i.texcoord;

        float t = floor(_Time.x * 228.0);
        float temp_rand = Rand2(float2(t, t));
        float trigger = step((1-randAmount), temp_rand); 


        float ONE_X = 1.0 / _ScreenParams.x;

        // ---- Mask strength
        float applyMask = step(1e-6, _FadeMultiplier);

        #if ALPHA_CHANNEL
            float maskSample = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, i.texcoord).a;
        #else
            float maskSample = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, i.texcoord).r;
        #endif

        float alpha_Mask = step(_adj, maskSample);     
        
        float strength   = lerp(1.0, alpha_Mask, applyMask);    

        // ---- Lines condition 
        float axis = lerp(p.x, p.y, saturate(_LinesAxis));
        float lineMask = 1.0 - step(0.5, frac(axis * screenLinesNum * 0.5)); 

        // ---- Jitter ----
        float jitter = ONE_X * cos(time_ * speed * 100) * jitterHAmount*trigger;
        jitter *= lineMask * strength;

        // ---- Apply shift axis 
        float sh = saturate(_ShiftH); // 0..1
            
        p += float2(jitter * sh, jitter * (1.0 - sh));

        half4 src_col = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uv);
        half4 dist_col = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, p);
        return dist_col;
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
