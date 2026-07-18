Shader "GlitchLabPro/RFInterferenceGlitchURP"
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

	float _FadeMultiplier; // 0 = no mask, 1 = use mask
    float _adj;            // mask threshold
    #pragma shader_feature ALPHA_CHANNEL // read mask A or R

	float randAmount; // chance threshold
	float fade;       // mix amount
	int stop;         // 1 = bypass
	float maxiters;   // loop count
	float speed;      // random speed

	// Simple random 0..1
	float rand(float2 n)
	{
		return frac(sin(dot(n, float2(12.9898, 4.1414))) * 43758.5453);
	}

	float4 Frag(Varyings i) : SV_Target
	{
		 float2 uv = i.texcoord;

		 if (stop == 1) return SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv); // no effect

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

		 float scaledT = _Time.x * 5.0;

		 float4 color = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv);

		 // Animated counter 0..maxiters-1
		 int iters = int(scaledT - float(maxiters) * floor(scaledT / float(maxiters)));

		 // Repeated multiply, adds some derivative-based noise (ddx/ddy)
		 for (int i = 0; i < maxiters; i++)
		 {
		     color *= 0.8 + (clamp((float(i < iters)) + (1.0 / float(iters)) * float4(float2(_ScreenParams.x * ddx(color.rg)), float2(_ScreenParams.y * ddy(color.ba))), 0.0, 1.0) * 0.25);
		 }

		 // Random gate (on/off)
		 float timeS = floor(_Time.x * 228 * speed);
		 float temp_rand = clamp(rand(float2(timeS, timeS)), 0.001, 1);

		 float4 col = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv);

		 if (temp_rand > randAmount) {
			 return lerp(col, color, fade);
		 }
		 else {
			 return col;
		 }

		 return color;
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
