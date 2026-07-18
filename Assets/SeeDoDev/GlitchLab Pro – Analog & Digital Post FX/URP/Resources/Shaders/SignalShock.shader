Shader "GlitchLabPro/SignalShockURP"
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

	#define q(x,p) (floor((x)/(p))*(p)) // quantize helper (not used)

    float _FadeMultiplier; // 0 = no mask, 1 = use mask
    float _adj;            // mask threshold
    #pragma shader_feature ALPHA_CHANNEL // read mask A or R

	float fade;        // mix amount
	float2 size;       // noise grid size
	float randAmount;  // chance threshold
	float amount;      // noise strength
	float light;       // 1 = blend with base

	// Simple random 0..1
	float rand(float2 st)
	{
		return frac(sin(dot(st.xy,
			float2(12.9898, 78.233))) *
			43758.5453123);
	}

	// Block noise (by grid cells)
	float fractal_noise(float2 uv_in, float time) {
		float2 uv = uv_in;
		float split_size_x = 20.0*size.x;
		float split_size_y = 100.0*size.y;
		float x = floor(uv.x * split_size_x);
		float y = floor(uv.y * split_size_y);
		return rand(float2(x, y) + float2(cos(time), cos(time)));
	}

	float4 Frag(Varyings i) : SV_Target
	{
		float2 uv = i.texcoord;
		float4 col;

		float time = floor(_Time.x * 228); // step time
		float temp_rand = clamp(rand(float2(time, time)), 0.001, 1);

		float4 image = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv);
		float4 image1 = image;
		float4 image2 = image;
		float4 image3 = image;

        // Mask reduces fade to 0 in blocked areas
        if (_FadeMultiplier > 0)
        {
            #if ALPHA_CHANNEL
                    float alpha_Mask = step(_adj, SAMPLE_TEXTURE2D(_Mask, sampler_Mask, i.texcoord).a);
            #else
                    float alpha_Mask = step(_adj, SAMPLE_TEXTURE2D(_Mask, sampler_Mask, i.texcoord).r);
            #endif              
            fade *= alpha_Mask;
        }

		// Random gate: sometimes apply the shock
		if (temp_rand > randAmount) {
			float noise = fractal_noise(i.texcoord, time) * amount;

			// R from right shift, G from center, B from left shift
			image1 = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2(0.1 * noise, 0)) - float4(0.1,0.1,0.1,0.1);
			image2 = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv) - float4(0.1,0.1,0.1,0.1);
			image3 = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv - float2(0.1 * noise, 0)) - float4(0.1,0.1,0.1,0.1);
		}

		if (light == 1)
			col = float4((image1.x + image.x) / 2, (image2.y + image.y) / 2, (image3.z + image.z) / 2,  (image1.a + image2.a + image3.a) / 3);
		else
			col = float4(image1.x, image2.y, image3.z, (image1.a + image2.a + image3.a) / 3);

		return lerp(image, col, fade); // fade mixes base and effect
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
