Shader "GlitchLabPro/DTVSignalBreakupURP"
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

	float Intensity = 0.01;           // main strength
	float interlaceIntesnsity = 0.01; // interlace noise
	float _Speed = 0.01;              // animation speed
	float _RGBSplit = 0.01;           // RGB offset size
	float _InterlaceScale = 0.01;     // lines density
	float _DisplaceIntesnsity = 0.01; // UV shift strength
	float dropoutIntensity = 0.1;     // dropout chance

	// Random 0..1
	float rand(float n) { return frac(sin(n) * 43758.5453123); }

	// 1D smooth noise
	float noise(float p) {
		float fl = floor(p);
		float fc = frac(p);
		return lerp(rand(fl), rand(fl + 1.0), fc);
	}

	// Block noise, returns small signed value
	float blockyNoise(float2 uv, float threshold, float scale, float seed)
	{
		float scroll = floor(_Time.x*_Speed + sin(11.0 * _Time.x*_Speed) + sin(_Time.y*_Speed)) * .7;
		float2 noiseUV = uv.yy / scale + scroll;
		noiseUV = frac(noiseUV);

		float noise2 = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, noiseUV).r;
		float id = floor(noise2 * 200.0);
		id = noise(id + seed) - 0.5;
		id *= step(abs(id), threshold); // keep only small values

		return id;
	}

	float4 Frag(Varyings i) : SV_Target
	{
		float rgbIntesnsity = 0.1 + 0.1 * sin(_Time.x * 3.7*_Speed);

		float displaceIntesnsity = (0.2 + 0.3 * pow(sin(_Time.x * 1.2), 5.0)) * _DisplaceIntesnsity * Intensity;

		float2 uv = i.texcoord;

		// Two block noises multiplied -> stronger breakup
		float displace = blockyNoise(uv + float2(uv.y, 0.0), displaceIntesnsity, 25.0, 66.6);
		displace *= blockyNoise(uv.yx + float2(0.0, uv.x), displaceIntesnsity, 111.0, 13.7);

		// Mask reduces displacement
		if (_FadeMultiplier > 0)
		{
                #if ALPHA_CHANNEL
                        float alpha_Mask = step(_adj, SAMPLE_TEXTURE2D(_Mask, sampler_Mask, i.texcoord).a);
                #else
                        float alpha_Mask = step(_adj, SAMPLE_TEXTURE2D(_Mask, sampler_Mask, i.texcoord).r);
                #endif
				displace *= alpha_Mask * Intensity;
		}

		uv.x += displace;

		// RGB split offset (only X)
		float2 offs = Intensity * _RGBSplit * float2(blockyNoise(uv.xy + float2(uv.y, 0.0), rgbIntesnsity, 65.0, 341.0), 0.0);

		float colr = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv - offs).r;
		float colg = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv).g;
		float colb = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + offs).b;

		// 3-step interlace mask
		float lines = frac(uv.y * _InterlaceScale);
		float s1 = step(0.333, lines);
		float s2 = step(0.666, lines);

		float4 mask0 = float4(3.0, 0.0, 0.0, 0.0);
		float4 mask1 = float4(0.0, 3.0, 0.0, 0.0);
		float4 mask2 = float4(0.0, 0.0, 3.0, 3.0);

		float4 mask = lerp(mask0, mask1, s1);
		mask = lerp(mask,  mask2, s2);

		// Noise can disable the interlace mask
		float maskNoise = blockyNoise(uv, interlaceIntesnsity*Intensity, 1, _Time.x*10) * max(displace, offs.x);

		if (_FadeMultiplier > 0)
		{
                #if ALPHA_CHANNEL
                        float alpha_Mask = step(_adj, SAMPLE_TEXTURE2D(_Mask, sampler_Mask, i.texcoord).a);
                #else
                        float alpha_Mask = step(_adj, SAMPLE_TEXTURE2D(_Mask, sampler_Mask, i.texcoord).r);
                #endif
				maskNoise *= alpha_Mask;
		}

		maskNoise = 1.0 - maskNoise;

		float isOne = 1.0 - step(1e-6, abs(maskNoise - 1.0));
		mask = lerp(mask, float4(1.0,1.0,1.0,1.0), isOne);

		// Dropout makes darker blocks
		float dropout = blockyNoise(uv, dropoutIntensity*Intensity, 11.0, _Time.x) * blockyNoise(uv.yx, dropoutIntensity*Intensity, 90.0, _Time.x);
		mask *= (1.0 - 5.0 * dropout);

		return float4(mask * float4(colr, colg, colb, (colr+colg+colb)/3));
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
