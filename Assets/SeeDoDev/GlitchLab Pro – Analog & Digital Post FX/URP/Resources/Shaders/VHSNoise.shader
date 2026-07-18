Shader "GlitchLabPro/VHSNoiseURP"
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

	float amount;        // main strength
	float linesAmount;   // line count
	float speed;         // animation speed
	float noiseA;        // 0 = stable, >0 = random wobble
	float _FadeMultiplier; // 0 = no mask, 1 = use mask
    float _adj;            // mask threshold
    #pragma shader_feature ALPHA_CHANNEL // read mask A or R

	// Custom mod (supports negative values)
	float mod(float x, float y) {
		return   x - y * floor(x / y);
	}
	float2 mod(float2 a, float2 b)
	{
		float2 c = frac(abs(a / b)) * abs(b);
		return (a < 0) ? -c : c;
	}

	// Simple random 0..1
	float rand(float2 co) {
		return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
	}

	// Noise sample using the source texture as a noise field
	float sample_noise(float2 fragCoord)
	{
		// Move in Y with time, wrap by screen size
		float2 uv = mod(fragCoord + float2(0, 100. * _Time.x), _ScreenParams.xy);
		float value = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv).r;
		return pow(abs(value), 7.); // more contrast
	}

	float4 Frag(Varyings i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
		float2 uv = i.texcoord;

		// Mask can reduce amount to 0 in blocked areas
		if (_FadeMultiplier > 0)
		{
                #if ALPHA_CHANNEL
                        float alpha_Mask = step(_adj, SAMPLE_TEXTURE2D(_Mask, sampler_Mask, i.texcoord).a);
                #else
                        float alpha_Mask = step(_adj, SAMPLE_TEXTURE2D(_Mask, sampler_Mask, i.texcoord).r);
                #endif
				amount *= alpha_Mask;
		}

		float2 wobbl;
		if(noiseA>0)
		wobbl = float2(amount * rand(float2(_Time.x, i.texcoord.y)), 0.); // random per scanline
		else
		wobbl = float2(amount, 0.); // constant shift

		// Extra offset from tan() -> sharp "tracking" spikes
		float t_val = tan(0.25 * _Time.x * 100 * speed + uv.y * PI * linesAmount);
		float2 tan_off = float2(wobbl.x * min(0., t_val), 0.);

		float4 color1 = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + wobbl + tan_off);
		float4 color = color1;

		// Small brightness wave by scanlines
		float s_val = ((sin(2. * PI * uv.y + _Time.x * 20.) + sin(2. * PI * uv.y)) / 2.)
		* .015 * sin(_Time.x);
		color += s_val;

		// Random white lines (on/off)
		float ival = _ScreenParams.y / 10.;
		float r = rand(float2(_Time.x, i.texcoord.y));
		float on = floor(float(uint(i.texcoord.y + (_Time.x * r * 100.)) % uint(ival + 1.)) / ival);
		float wh = sample_noise(i.texcoord) * on;

		color = float4(min(1., color.r + wh),
			min(1., color.g + wh),
			min(1., color.b + wh), min(1., color.a + wh));

		float vig = 1. - sin(PI * uv.x) * sin(PI * uv.y); // computed but not used

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
