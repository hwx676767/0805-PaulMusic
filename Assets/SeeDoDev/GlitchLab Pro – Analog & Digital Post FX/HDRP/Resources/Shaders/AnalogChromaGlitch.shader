Shader "GlitchLabPro/AnalogChromaGlitch" 
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
            float _FadeMultiplier;
    float _adj;
    #pragma shader_feature ALPHA_CHANNEL
        float _Intensity;

        uniform float Speed;
        uniform float AMPLITUDE;
        uniform float Fade;
        uniform float randAmount;
        uniform int stop;


        float2 uvp(float2 uv) {
            return clamp(uv, 0.0, 1.0);
        }

        float rand(float2 co) {
            return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
        }

        float4 Frag(Varyings input) : SV_Target
        {

            float4 col = float4(0.0,0.0,0.0,0.0);
            float amp;

            amp = AMPLITUDE;

            float2 uv = input.texcoord;
            if (stop == 1)return SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uv);
		 if (_FadeMultiplier > 0)
		 {
                #if ALPHA_CHANNEL
                        float alpha_Mask = step(_adj, SAMPLE_TEXTURE2D(_Mask, sampler_Mask, input.texcoord).a);
                #else
                        float alpha_Mask = step(_adj, SAMPLE_TEXTURE2D(_Mask, sampler_Mask, input.texcoord).r);
                #endif
				Fade *= alpha_Mask;
		 }
            [unroll]for (int i = 0; i < 4; i++) {
                uv += float2(sin(_Time.x + float(i) + amp), cos(_Time.x + float(i) + amp)) * amp * 0.2;
                float4 texOrig = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uvp(uv));

                uv.x = uv.x + (rand(float2(uv.y + float(i), _Time.x)) * 2.0 - 1.0) * amp * 0.8 * (texOrig[i] + 0.2);
                uv.y = uv.y + (rand(float2(uv.x, _Time.x + float(i))) * 2.0 - 1.0) * amp * 0.1 * (texOrig[i] + 0.2);

                float4 tex = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uvp(uv)); 

                tex += (rand(uv + _Time.x + float(i)) * 2.0 - 1.0) * amp * 0.1;

                tex += (rand(uv + _Time.x + tex[i] + float(i)) * 2.0 - 1.0) * amp * 0.2;

                tex += lerp(1.0, rand(uv + tex[i] + (float(i) * 253.6) + _Time.x) * tex.r * 5.0, amp);
                
                tex += abs(tex[i] - texOrig[i]); 

                tex *= rand(uv) * amp + 1.0;

                tex = frac(tex);

                col[i] = tex[i];
            }
            
            float4 col1 = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, input.texcoord);
            float time = floor(_Time.x * Speed);
            float temp_rand = clamp(rand(float2(time, time)), 0.001, 1);

            if (temp_rand > randAmount) {
                return lerp(col1 , float4(col.rgb,col1.a/col.a), Fade);
            }
            else {
                return col1;
            }
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