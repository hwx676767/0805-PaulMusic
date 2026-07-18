Shader "GlitchLabPro/NoiseEdgeShiftURP"
{
    Properties
    {
        _BlitTexture("Texture", 2D) = "white" {} // Source (camera color)
    }
    HLSLINCLUDE
// Include

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        // The Blit.hlsl file provides the vertex shader (Vert),
        // the input structure (Attributes), and the output structure (Varyings)
#include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
	TEXTURE2D(_Mask);
	SAMPLER(sampler_Mask);

	float width; // edge threshold
	float fade;  // mix amount

	float _FadeMultiplier; // 0 = no mask, 1 = use mask
    float _adj;            // mask threshold
    #pragma shader_feature ALPHA_CHANNEL // read mask A or R

	// Not used in Frag (kept as is)
	float3 hash3(uint n)
	{
		n = (n << 13U) ^ n;
		n = n * (n * n * 15731U + 789221U) + 1376312589U;
		uint3 k = n * uint3(n, n * 16807U, n * 48271U);
		return float3(k & uint3(0x7fffffffU, 0x7fffffffU, 0x7fffffffU)) / float(0x7fffffff);
	}

	// Simple random 0..1
	float rand(float2 st)
	{
		return frac(sin(dot(st.xy,
			float2(12.9898, 78.233))) *
			43758.5453123);
	}

	float4 Frag(Varyings i) : SV_Target
	{
		float2 uv = i.texcoord;

		uint2 p = uint2(uv); // not used (kept)

		float4 col = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv);
		float4 col2 = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + 0.002); // small offset sample

		float4 diff = abs(col - col2); // edge measure
		float4 fragColor;

        // Mask changes threshold
        if (_FadeMultiplier > 0)
        {
            #if ALPHA_CHANNEL
                    float alpha_Mask = step(_adj, SAMPLE_TEXTURE2D(_Mask, sampler_Mask, i.texcoord).a);
            #else
                    float alpha_Mask = step(_adj, SAMPLE_TEXTURE2D(_Mask, sampler_Mask, i.texcoord).r);
            #endif
            width /= alpha_Mask; // note: alpha_Mask can be 0
        }

		// Replace edges with random noise color
		if (diff.r > width) {
			fragColor = float4(rand(0.5 + uv * _Time.x), rand(-0.2 + uv * _Time.x), rand(0.8 + uv * _Time.x), 1);
		}
		else {
			fragColor = col;
		}

		return lerp(col, fragColor, fade);
	}

		ENDHLSL

		SubShader
	{
		        PackageRequirements
        {
            "com.unity.render-pipelines.universal": "0.0.0"
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
