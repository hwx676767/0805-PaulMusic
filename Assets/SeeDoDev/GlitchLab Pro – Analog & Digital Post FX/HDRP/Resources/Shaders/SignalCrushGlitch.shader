Shader "GlitchLabPro/SignalCrushGlitch" 
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

		uniform float _TimeX;  // custom time (stable control from script)
		uniform float Offset;  // noise strength

		float _FadeMultiplier; // 0 = no mask, 1 = use mask
        float _adj;            // mask threshold
        #pragma shader_feature ALPHA_CHANNEL // read mask A or R

		uniform float Fade; // mix amount

		// Random 0..1, changes in steps by _TimeX
		float random(float2 seed)
		{
			return frac(sin(dot(seed * floor(_TimeX * 30.0), float2(127.1,311.7))) * 43758.5453123);
		}

		float random(float seed)
		{
			return random(float2(seed, 1.0));
		}

        float4 Frag(Varyings i) : SV_Target
        {
			float2 uv = i.texcoord;

			// Two grids: small blocks and large blocks
			float2 blockS = floor(uv * float2(24., 9.));
			float2 blockL = floor(uv * float2(8., 4.));

			// Line noise: mostly small, sometimes big spikes
			float lineNoise = pow(random(blockS), 8.0) * Offset * pow(random(blockL), 3.0) - pow(random(7.2341), 17.0) * 2.;

            // Mask can turn noise off in blocked areas
            if (_FadeMultiplier > 0)
            {
                #if ALPHA_CHANNEL
                        float alpha_Mask = step(_adj, SAMPLE_TEXTURE2D(_Mask, sampler_Mask, i.texcoord).a);
                #else
                        float alpha_Mask = step(_adj, SAMPLE_TEXTURE2D(_Mask, sampler_Mask, i.texcoord).r);
                #endif                
                lineNoise *= alpha_Mask;
            }

			float4 col1 = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uv);
			float4 col2 = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uv + float2(lineNoise * 0.05 * random(5.0), 0));
			float4 col3 = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uv - float2(lineNoise * 0.05 * random(31.0), 0));

			// Take R/G/B from different samples
			float4 result = float4(float3(col1.x, col2.y, col3.z), col1.a+col2.a+col3.a);
			result = lerp(col1, result, Fade);

			return result;
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
