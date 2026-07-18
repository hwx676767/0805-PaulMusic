Shader "GlitchLabPro/UVEchoGlitchURP"
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
#include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl" // Fullscreen blit + _BlitTexture
	TEXTURE2D(_Mask);
	SAMPLER(sampler_Mask);

	float _FadeMultiplier; // 0 = no mask, 1 = use mask
    float _adj;            // mask threshold
    #pragma shader_feature ALPHA_CHANNEL // read mask A or R

	float amount;     // effect strength
	float speed;      // rotation speed
	float randAmount; // chance threshold
	int stop;         // 1 = bypass
	float Rspeed;     // random change speed
	float fade;       // mix with base

	// Simple random 0..1
	float rand(float2 n)
	{
		return frac(sin(dot(n, float2(12.9898, 4.1414))) * 43758.5453);
	}

	#define SIN01(a) (sin(a)*0.5 + 0.5) // sin -> 0..1

	// RGB to HSV (h in [0..1])
	float3 rgb2hsv(float3 c)
	{
		float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
		float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
		float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));

		float d = q.x - min(q.w, q.y);
		float e = 1.0e-10;
		return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
	}

	float4 Frag(Varyings i) : SV_Target
	{
		float2 uv = i.texcoord;

		if (stop == 1) return SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv); // no effect

		// Mask can set amount to 0 in blocked areas
		if (_FadeMultiplier > 0)
		{
                #if ALPHA_CHANNEL
                        float alpha_Mask = step(_adj, SAMPLE_TEXTURE2D(_Mask, sampler_Mask, i.texcoord).a);
                #else
                        float alpha_Mask = step(_adj, SAMPLE_TEXTURE2D(_Mask, sampler_Mask, i.texcoord).r);
                #endif
				amount *= alpha_Mask;
		}

		// Use hue/saturation to drive the UV offset direction and size
		float3 hsv = rgb2hsv(SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv).rgb);

		float angle = hsv.x + atan2(uv.y, uv.x) + _Time.x * speed;
		float2x2 RotationMatrix = float2x2(cos(angle), -sin(angle), sin(angle), cos(angle));

		// log(...) makes a stronger jump when amount is high, but safe with max(...,0)
		float4 col = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp,
			uv + mul(RotationMatrix,
				float2(log(max(SIN01(_Time.x * 0.7) - 0.2, 0.) * amount + 1.), 0)) * hsv.y);

		// Random gate: sometimes blend to "echo", sometimes show normal
		float timeS = floor(_Time.x * 228 * Rspeed);
		float temp_rand = clamp(rand(float2(timeS, timeS)), 0.001, 1);

		float4 col1 = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv);

		if (temp_rand > randAmount) {
			return lerp(col1, col, fade); // mix to echo
		}
		else {
			return col1; // base
		}
	}

	ENDHLSL

		SubShader
	{
		        PackageRequirements
        {
            "com.unity.render-pipelines.universal": "0.0.0"
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
