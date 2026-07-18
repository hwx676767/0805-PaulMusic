Shader "DistortionsPro_20X/CustomTextureHDRP"
{
    Properties
    {
        _MainTex("Main Texture", 2DArray) = "grey" {}
        _CustomTexture("Main Texture", 2D) = "black" {}
    }

    HLSLINCLUDE

    #pragma target 4.5
    #pragma only_renderers d3d11 ps4 xboxone vulkan metal switch

    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/FXAA.hlsl"
    #pragma shader_feature CUSTOM_ALPHA_ON
    #pragma shader_feature CUSTOM_ALPHA_OFF

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

    float _Intensity;
    TEXTURE2D_X(_MainTex);
	    TEXTURE2D(_CustomTexture);
    SAMPLER(sampler_CustomTexture);
    float _Fade;
    float _AlphaCutoff;

    float4 CustomPostProcess(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float2 uv = input.texcoord;

        float4 baseCol = SAMPLE_TEXTURE2D_X(_MainTex, s_point_clamp_sampler, uv);
        float4 overCol = SAMPLE_TEXTURE2D(_CustomTexture, sampler_CustomTexture, uv);


        float mx = max(overCol.r, max(overCol.g, overCol.b));
        float a = smoothstep(_AlphaCutoff, _AlphaCutoff + 0.05, mx);

        float3 outRgb = lerp(baseCol.rgb, overCol.rgb, a * _Fade);
        return float4(outRgb,1);
    }

    ENDHLSL

    SubShader
    {
        PackageRequirements
        {
            "com.unity.render-pipelines.high-definition": "0.0.0"
        }
        Pass
        {
            Name "#CustomTexture#"

            ZWrite Off
            ZTest Always
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off

            HLSLPROGRAM
                #pragma fragment CustomPostProcess
                #pragma vertex Vert
            ENDHLSL
        }
    }
    Fallback Off
}
