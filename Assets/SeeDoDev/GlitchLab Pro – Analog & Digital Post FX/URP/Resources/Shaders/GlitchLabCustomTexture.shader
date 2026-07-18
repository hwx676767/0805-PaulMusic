Shader "GlitchLabPro/GlitchLabCustomTextureURP"
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
	TEXTURE2D(_CustomTex);
	SAMPLER(sampler_CustomTex);
	half _Fade;
	half _AlphaCutoff;

	float4 Frag(Varyings i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
		float4 col = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp,i.texcoord);
		float4 col2 = SAMPLE_TEXTURE2D(_CustomTex, sampler_CustomTex, i.texcoord);
      float mx = max(col2.r, max(col2.g, col2.b));
      float a = smoothstep(_AlphaCutoff, _AlphaCutoff + 0.05, mx);

      float3 outRgb = lerp(col.rgb, col2.rgb, a * _Fade);
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
	        FallBack "Diffuse"

}