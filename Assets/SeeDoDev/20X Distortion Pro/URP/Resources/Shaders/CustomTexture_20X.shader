Shader "DistortionsPro_20X/CustomTexture_20X"
{
    Properties
    {
        _BlitTexture("Texture", 2D) = "white" {}
    }
    HLSLINCLUDE
// Include

	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			// The Blit.hlsl file provides the vertex shader (Vert),
			// the input structure (Attributes), and the output structure (Varyings)
	#include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
	TEXTURE2D(_CustomTexture);
	SAMPLER(sampler_CustomTexture);
	half _Fade;
	half _AlphaCutoff;

	float4 Frag(Varyings i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
              float2 uv = i.texcoord;

      float4 baseCol = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv);
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
}
