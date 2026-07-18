Shader "GlitchLabPro/MosaicGlitch"
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
	uniform TEXTURE2D(_Mask);
	uniform SAMPLER(sampler_Mask);

	float _FadeMultiplier; // 0 = no mask, 1 = use mask
    float _adj;            // mask threshold
    #pragma shader_feature ALPHA_CHANNEL // read mask A or R

	uniform float Size1;      // pattern scale
	uniform float Speed;      // animation speed
	uniform float Strength;   // UV offset strength
	uniform float Fade;       // mix amount
	uniform float rgb_split;  // channel offset
	uniform float randAmount; // chance threshold

	uniform float stop; // 1 = bypass

	// Simple random 0..1
	float rand(float2 n)
	{
		return frac(sin(dot(n, float2(12.9898, 4.1414))) * 43758.5453);
	}

	float mod(float x, float y) {
		return   x - y * floor(x / y);
	}

	// Hash for noise
	float hash(float n)
	{
		n = mod(n, 64.0);
		return frac(sin(n) * 43758.5453);
	}

	float noise(float2 p)
	{
		return hash(p.x + p.y * 57.0);
	}

	// Cell jitter for voronoi
	float2 cell(float2 cell)
	{
		return float2(noise(cell) + cos(cell.y) * 0.3, noise(cell * 0.3) + sin(cell.x) * 0.3);
	}

	// Accumulate one neighbor cell
	inline void VoroAcc(float2 q, float2 t, float pw, inout float3 cl, inout float wsum)
	{
		float2 q2 = q - floor(q / 8.0) * 8.0;
		float2 c  = q + cell(q2);
		float2 r  = c - t;

		float  cos1 = 0.5 + 0.5 * cos((q2.x + q2.y * 119.0) * 8.0);
		float2 s    = smoothstep(0.0, 1.0, 1.0 - abs(r));
		float  w    = pow(s.x, pw) * pow(s.y, pw);

		cl   += float3(cos1, cos1, cos1) * w;
		wsum += w;
	}

	// Voronoi noise (weighted neighbors)
	float3 voronoiNoize2(float2 t, float pw)
	{
		float2 p = floor(t);
		float3 cl = 0.0;
		float  wsum = 0.0;

		VoroAcc(p + float2(-1,-1), t, pw, cl, wsum);
		VoroAcc(p + float2( 0,-1), t, pw, cl, wsum);
		VoroAcc(p + float2( 1,-1), t, pw, cl, wsum);

		VoroAcc(p + float2(-1, 0), t, pw, cl, wsum);
		VoroAcc(p + float2( 0, 0), t, pw, cl, wsum);
		VoroAcc(p + float2( 1, 0), t, pw, cl, wsum);

		VoroAcc(p + float2(-1, 1), t, pw, cl, wsum);
		VoroAcc(p + float2( 0, 1), t, pw, cl, wsum);
		VoroAcc(p + float2( 1, 1), t, pw, cl, wsum);

		float invW = 1.0 / max(wsum, 1e-6);
		return pow(cl * invW, 0.5) * 2.0;
	}

	// Mix a few voronoi layers
	float3 voronoiNoize(float2 t)
	{
		float3 v1 = voronoiNoize2(t * 0.25, 16.0);
		return v1 * voronoiNoize2(t * 0.5 + v1.xy, 2.0) + voronoiNoize2(t * 0.5, 4.0) * 0.5;
	}

	float4 Frag(Varyings input) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input); // XR support

		float time = _Time.x * Speed;
		float2 uv = input.texcoord;

		if (stop == 1) return SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uv); // no effect

		// Mask reduces Fade to 0 in blocked areas
		if (_FadeMultiplier > 0)
		{
			#if ALPHA_CHANNEL
					float alpha_Mask = step(_adj, SAMPLE_TEXTURE2D(_Mask, sampler_Mask, input.texcoord).a);
			#else
					float alpha_Mask = step(_adj, SAMPLE_TEXTURE2D(_Mask, sampler_Mask, input.texcoord).r);
			#endif
			Fade *= alpha_Mask;
		}

		float2 t = uv + (float2(sin(time), cos(time)) - time * 2.0) * 0.4;

		float size = 332.2;
		float2 tt = frac(t * 0.5) * size;

		float4 col = float4(0.0,0.0,0.0,0.0);

		// Voronoi gives mosaic-like UV offsets
		float2 off = voronoiNoize(tt * Size1 + 90).rg;

		col.r = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, clamp(uv + (1 - off) * Strength, 0.0, 1.0)).r;
		col.g = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, clamp(uv + (1 - off  + rgb_split) * Strength, 0.0, 1.0)).g;
		col.b = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, clamp(uv + (1 - off  + 2 * rgb_split) * Strength, 0.0, 1.0)).b;
		col.a = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, clamp(uv + (1 - off  + 3 * rgb_split) * Strength, 0.0, 1.0)).a;

		// Random gate (on/off)
		float timeS = floor(_Time.x * 228);
		float temp_rand = clamp(rand(float2(timeS, timeS)), 0.001, 1);

		float4 col1 = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, uv);

		if (temp_rand > randAmount) {
			return lerp(col1, col, Fade);
		}
		else {
			return col1;
		}
	}

		ENDHLSL

		SubShader
	{
		    PackageRequirements
    {
        "com.unity.render-pipelines.high-definition": "0.0.0"
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
