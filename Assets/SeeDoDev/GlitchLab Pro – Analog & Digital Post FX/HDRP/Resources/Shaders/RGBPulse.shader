Shader "GlitchLabPro/RGBPulse" 
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
		TEXTURE2D(_Mask);
		SAMPLER(sampler_Mask);

		float _FadeMultiplier; // 0 = no mask, 1 = use mask
        float _adj;            // mask threshold
        #pragma shader_feature ALPHA_CHANNEL // read mask A or R

		uniform float _GlitchInterval; // pulse period
		uniform float _GlitchRate;     // pulse length
        uniform float _RGBSplit;       // split amount
        uniform float _Speed;          // wave speed
        uniform float _Amount;         // wave strength

		float2 _Res; // resolution (pixels)

		#define NOISE_SIMPLEX_1_DIV_289 0.00346020761245674740484429065744f

		float2 mod289(float2 x) {
			return x - floor(x * NOISE_SIMPLEX_1_DIV_289) * 289.0;
		}

		float3 mod289(float3 x) {
			return x - floor(x * NOISE_SIMPLEX_1_DIV_289) * 289.0;
		}
		float3 permute(float3 x) {
			return (x * x * 34.0 + x) - floor((x * x * 34.0 + x) * NOISE_SIMPLEX_1_DIV_289) * 289.0;
		}

		// 2D simplex noise
		float snoise(float2 v)
		{
			const float4 C = float4(
				0.211324865405187, 
				0.366025403784439, 
				-0.577350269189626, 
				0.024390243902439  
				);
		
			float2 i = floor(v + dot(v, C.yy));
			float2 x0 = v - i + dot(i, C.xx);

			float s = step(x0.y, x0.x);
			float2 i1 = float2(s, 1.0 - s);
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;

			i = i - floor(i * 0.00346020) * 289.0; 
			float3 p1 = i.y + float3(0.0, i1.y, 1.0);
			p1 = (p1 * p1 * 34.0 + p1) - floor((p1 * p1 * 34.0 + p1) * 0.00346020) * 289.0;

			float3 p2 = p1 + i.x + float3(0.0, i1.x, 1.0);
			p2 = (p2 * p2 * 34.0 + p2) - floor((p2 * p2 * 34.0 + p2) * 0.00346020) * 289.0;
			float3 p = p2;

			float3 m = max(0.5 - float3(dot(x0, x0),dot(x12.xy, x12.xy),dot(x12.zw, x12.zw)),0.0);
			m = m * m;
			m = m * m;

			float3 x = 2.0 * frac(p * C.www) - 1.0;
			float3 h = abs(x) - 0.5;
			
			float3 a0 = x - floor(x + 0.5);

			m *= 1.79284291400159 - 0.85373472095314 * (a0 * a0 + h * h);

			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot(m, g);
		}

		// Simple random 0..1
		float random(float2 c) 
        {
			return frac(sin(dot(c.xy, float2(12.9898, 78.233))) * 43758.5453);
		}

		float mod(float x, float y)
		{
			return x - y * floor(x / y);
		}

        float4 Frag(Varyings i) : SV_Target
        {
			half strength = 0.;
			float2 shake = float2(0., 0.);

			// Pulse on/off by time
			strength = smoothstep(_GlitchInterval * _GlitchRate, _GlitchInterval, _GlitchInterval - mod(_Time.y, _GlitchInterval));

			// Small random UV shake (in pixels -> UV)
			shake = float2(strength , strength ) * float2(random(float2(_Time.xy)) * 2.0 - 1.0, random(float2(_Time.y * 2.0, _Time.y * 2.0)) * 2.0 - 1.0) / float2(_Res.x, _Res.y);

			// Mask can reduce strength/shake to 0
			if (_FadeMultiplier > 0)
			{
				#if ALPHA_CHANNEL
                        float alpha_Mask = step(_adj, SAMPLE_TEXTURE2D(_Mask, sampler_Mask, i.texcoord).a);
                #else
                        float alpha_Mask = step(_adj, SAMPLE_TEXTURE2D(_Mask, sampler_Mask, i.texcoord).r);
                #endif					
				strength *= alpha_Mask;
				shake *= alpha_Mask;
			}

			float y = i.texcoord.y * _Res.y; // scanline in pixels
			float rgbWave = 0.;

			// Noise wave along Y, animated by time
			rgbWave = (snoise(float2( y * 0.01, _Time.y * _Speed*20)) * ( strength * _Amount*32.0) * snoise(float2( y * 0.02, _Time.y * _Speed*10)) * (strength * _Amount*4.0) ) / _Res.x;

			float rgbDiff = (_RGBSplit*50 + (20.0 * strength + 1.0)) / _Res.x;
			rgbDiff = rgbDiff * rgbWave;

			float rgbUvX = i.texcoord.x + rgbWave;

			// G from center, R/B from shifted UV
			float4 g = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, float2(rgbUvX, i.texcoord.y) );
			float4 rb = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, float2(rgbUvX + rgbDiff, i.texcoord.y) );

			float4 ret = float4(rb.x, g.y, rb.z, rb.a+g.a );

			return ret;
        }

    ENDHLSL

    SubShader
    {
		    PackageRequirements
    {
        "com.unity.render-pipelines.high-definition": "0.0.0"
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
