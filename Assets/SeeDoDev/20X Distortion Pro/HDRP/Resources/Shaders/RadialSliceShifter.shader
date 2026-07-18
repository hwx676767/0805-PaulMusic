Shader "DistortionsPro_20X/RadialSliceShifterHDRP"
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
        // Texture bindings
        TEXTURE2D(_Mask);
        SAMPLER  (sampler_Mask);

        float  _FadeMultiplier;      // enables mask-driven modulation
        float  MaskThreshold;        // edge softness of the mask

        // controls
        float  BaseSliceWidth;       // thickness of the displaced wedge
        float  LineExpansionRate;    // how fast the slice grows with time
        float  Phase;                // phase input for oscillation
        // Per-eye optical center in UV 0..1
        float2 EyeCenterUV()
        {
            float4 clip = mul(UNITY_MATRIX_P, float4(0,0,-1,1));
            float2 ndc  = clip.xy / max(clip.w, 1e-6);  // -1..1
            return ndc * 0.5 + 0.5;                     // -> 0..1
        }

        // Fragment shader
        float4 Frag(Varyings IN) : SV_Target
        {
            UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

            float2 uv    = ClampAndScaleUVForBilinearPostProcessTexture(IN.texcoord.xy);
            float  animT = 0.5 * (1.0 + sin(Phase));

            // per-eye center + aspect-correct metric space
            float2 c      = EyeCenterUV();
            float  aspect = _ScreenParams.x / _ScreenParams.y;

            // axis direction in metric space (unit)
            float a   = radians(animT * 180.0);
            float2 uM = normalize(float2(cos(a) * aspect, sin(a)));    // axis (metric)
            float2 vM = float2(-uM.y, uM.x);                           // perpendicular (metric)

            // project current pixel to axis (metric), then back to UV
            float2 p    = uv - c;
            float2 pM   = float2(p.x * aspect, p.y);
            float2 qM   = uM * dot(pM, uM);                            // closest point on axis (metric)
            float2 qUV  = c + float2(qM.x / aspect, qM.y);             // back to UV

            float distToAxis = length(pM - qM);

            // mask-modulated slice width
            float sliceWidth = BaseSliceWidth;
            if (_FadeMultiplier > 0.0)
            {
                float m;
              #if ALPHA_CHANNEL
                m = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, uv).a;
              #else
                m = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, uv).r;
              #endif
                sliceWidth *= smoothstep(MaskThreshold - 0.05, MaskThreshold + 0.05, m);
            }
            float dynamicWidth = sliceWidth * (1.0 + animT * LineExpansionRate);

            // side sign (metric), UV offset converted from metric
            float sideSign = sign(dot(pM, vM));
            float2 offsetUV = float2(vM.x / aspect, vM.y) * dynamicWidth;

            // sample
            float4 baseCol   = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uv);
            float4 axisCol   = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, qUV);
            float4 shiftedCol= SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, saturate(uv + offsetUV * sideSign));

            // fill gap on the axis
            float4 finalColor = (distToAxis <= dynamicWidth) ? axisCol : shiftedCol;

            // fallback if width ~0
            return (dynamicWidth < 0.001) ? baseCol : finalColor;
        }

    ENDHLSL

    SubShader
    {
        PackageRequirements
        {
            "com.unity.render-pipelines.high-definition": "0.0.0"
        }
        Name "#RadialSliceShifter#"
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
