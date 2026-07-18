Shader "GlitchLabPro/ScanlineJitterURP" 
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

		half strength = 1; // current jitter strength

		float mod(float x, float y)
		{
			return x - y * floor(x / y);
		}

		float jitterVFreq = 0.5; // how often jitter happens
		float jitterVRate = 0.5; // how long it stays

		#pragma shader_feature VHS_JITTER_V_ON
		#pragma shader_feature JITTER_V_CUSTOM

		float jitterVAmount = 1.0;  // jitter size
		float jitterVSpeed = 1.0;   // jitter speed
		float2 JitterVRandSeed;     // not used here
		float time_ = 0.0;          // external time

		// RGB <-> YIQ for VHS-like channel shift
		half3 rgb2yiq(half3 c)
			{   
			return half3(
				(0.2989*c.x + 0.5959*c.y + 0.2115*c.z),
				(0.5870*c.x - 0.2744*c.y - 0.5229*c.z),
				(0.1140*c.x - 0.3216*c.y + 0.3114*c.z)
				);
			};
		half3 yiq2rgb(half3 c)
			{				
			return half3(
				(	 1.0*c.x +	  1.0*c.y + 	1.0*c.z),
				( 0.956*c.x - 0.2720*c.y - 1.1060*c.z),
				(0.6210*c.x - 0.6474*c.y + 1.7046*c.z)
				);
			};

			// Simple random 0..1
			float rnd_rd(float2 co)
			{
			    float a = 22.9898;
				float b = 58.233;
				float c = 56058.5453;
				float dt= dot(co.xy ,float2(a,b));
				float sn= fmod(dt,3.14);
				return frac(sin(sn) * c);
			}

			// Sample Y/I/Q from different X offsets (scanline jitter)
			float4 yiqDist(float2 uv, float m, float t)
				{					
				m *= 0.001; // small scale
				float3 offsetX = float3( uv.x, uv.x, uv.x );	
				offsetX.r +=  sin(rnd_rd(float2(t*0.2, uv.y)))*m;
				offsetX.g +=  sin(t*9.0)*m;

				half4 signal = half4(0.0, 0.0, 0.0, 0.0);
				signal.r = rgb2yiq(SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, float2(offsetX.r, uv.y) ).rgb ).x; // Y
				signal.g = rgb2yiq(SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, float2(offsetX.g, uv.y) ).rgb ).y; // I
				signal.b = rgb2yiq(SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, float2(offsetX.b, uv.y) ).rgb ).z; // Q

				// Alpha is summed from a few samples
				signal.a = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp,uv).a + SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, float2(offsetX.g, uv.y)).a + SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, float2(offsetX.r, uv.y)).a ;
				return signal;					    
				}

        float4 Frag(Varyings i) : SV_Target
        {
			float t = time_;			
			float2 p = i.texcoord;				

			half4 col = half4(0.0,0.0,0.0,0.0);
			half4 signal = half4(0.0,0.0,0.0,0.0);

			#if JITTER_V_CUSTOM
				strength = 1;
			#else
				// Pulse strength over time (on/off)
				strength = smoothstep(jitterVFreq * jitterVRate, jitterVFreq, jitterVFreq - mod(_Time.y, jitterVFreq));
			#endif			

			// Mask can set strength to 0 in blocked areas
			if (_FadeMultiplier > 0)
			{
				#if ALPHA_CHANNEL
                        float alpha_Mask = step(_adj, SAMPLE_TEXTURE2D(_Mask, sampler_Mask, i.texcoord).a);
                #else
                        float alpha_Mask = step(_adj, SAMPLE_TEXTURE2D(_Mask, sampler_Mask, i.texcoord).r);
                #endif					
				strength *= alpha_Mask;
			}

			signal = yiqDist(p, jitterVAmount*strength, t*jitterVSpeed);
			col.rgb = yiq2rgb(signal.rgb);

			// Note: yiqDist is called again only to get alpha
			return half4(col.rgb, (yiqDist(p, jitterVAmount*strength, t*jitterVSpeed)).a); 
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
