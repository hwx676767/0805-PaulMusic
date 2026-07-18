Shader "DistortionsPro_20X/LiquidGlass"
{
    Properties
    {
        _BlitTexture("CTexture", 2D) = "white" {}
    }

    HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

    float4 _Lens;
    float _Amount;
    float _RingSize;
    float _RingWidth;
    float _RingDistortion;
    float _RingNoise;
    float _Speed;
    float _RimRing;
    float _Bulge;


    // Helpers
    inline float CircleMaskAA(float2 uv, float2 center, float radius, float soft01)
    {
        float aspect = _ScreenParams.x / _ScreenParams.y;
        float2 p = (uv - center) / max(radius, 1e-6);
        p.x *= aspect;

        float r  = length(p);
        float aa = fwidth(r);
        return 1.0 - smoothstep(1.0 - soft01 - aa, 1.0 + aa, r);
    }

    inline float RingAA(float2 uv, float2 center, float radius, float ringWidth01)
    {
        float aspect = _ScreenParams.x / _ScreenParams.y;
        float2 p = (uv - center) / max(radius, 1e-6);
        p.x *= aspect;

        float r  = length(p);
        float aa = fwidth(r);

        float a = smoothstep(1.0 - ringWidth01 - aa, 1.0 - ringWidth01 + aa, r);
        float b = smoothstep(1.0 - aa,              1.0 + aa,              r);
        return saturate(a - b);
    }

    inline float Hash12(float2 p)
    {
        p = frac(p * float2(123.34, 456.21));
        p += dot(p, p + 45.32);
        return frac(p.x * p.y);
    }

    half4 Frag(Varyings i) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

        float time = _Time.y*_Speed;

        float2 uv = i.texcoord;
        float4 src = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv);

        float2 center = _Lens.xy;
        float  a      = saturate(_Amount);

        float radius  = _RingSize;

        float2 texel  = rcp(_ScreenParams.xy);
        float  aspect = _ScreenParams.x / _ScreenParams.y;

        float rimStartMin = 0.6*_RingWidth;
        float rimStartMax = .9*_RingWidth;
        float rimStart    = lerp(rimStartMax, rimStartMin, a);

        float rimGamma = .6;

        float compressPMin = 2.0;
        float compressPMax = 3.0;
        float p = lerp(compressPMin, compressPMax, a);

        float swirl = lerp(0.0, 2.0, a);

        float caPx   = lerp(0.0, 1.0, a);
        float blurPx = lerp(0.0, 0.75, a);

        // ----------------------------
        // Disk mask
        // ----------------------------
        float softEdge = lerp(0.003, 0.010, a);
        float inside = CircleMaskAA(uv, center, radius, softEdge);
        if (inside <= 0.0) return src;

        // disk coords (aspect-correct)
        float2 pUV = uv - center;
        float2 q   = pUV / radius; q.x *= aspect;

        float r  = length(q);
        float rr = saturate(r);

        float invR = rcp(max(r, 1e-6));
        float2 dirA = q * invR;
        float theta = atan2(dirA.y, dirA.x);

     
// Center lens
float b = saturate(_Bulge);

float bulgeStrength = lerp(0.0, 1.2, b) * a;  

float bulgeCurve = lerp(6.0, 1.4, b);          

float wC = pow(saturate(1.0 - rr), bulgeCurve); 
float2 uvCenter = center + pUV * (1.0 - bulgeStrength * wC);

        // ----------------------------
        // Rim mask
        // ----------------------------
        float t = saturate((rr - rimStart) / max(1.0 - rimStart, 1e-6));
        float rim = t * t * (3.0 - 2.0 * t);
        rim = pow(rim, rimGamma);

float stickPow = .9315;  
float spinPow  = _RingDistortion;  

float stick = pow(rim, stickPow);
float spin  = pow(rim, spinPow);

        float rrComp = pow(1.0 - rr, p);
float rr2 = lerp(rr, rrComp, stick);

        // angle variation (animated)
        float ang01 = theta * 0.1591549431 + 0.5;

        float wv  = sin(theta * 3.0 + rr * 2.0 + time * 0.7) * 0.55
                  + sin(theta * 7.0 - rr * 5.0 + time * 1.1) * 0.25
                  + sin(theta * 11.0 + rr * 9.0 + time * 0.9) * 0.15;

        wv += (Hash12(float2(ang01 * 17.0 + time * 0.35, rr * 23.0 + time * 0.22)) - 0.5) * _RingNoise;

float theta2 = theta + wv * (swirl * spin) + (rr2 * rr2) * (0.55 * a) * spin;

        // reconstruct rim uv
        float2 q2A = rr2 * float2(cos(theta2), sin(theta2));
        float2 uvRim = center + float2(q2A.x / aspect, q2A.y) * radius;

float2 uvF = lerp(uvCenter, uvRim, stick);


float2 dirUV0 = float2(dirA.x / aspect, dirA.y);      
float2 uvEdge = center + dirUV0 * radius;           
float edgePull = stick * (0.05 + 0.05 * a);          
uvF = lerp(uvF, uvEdge, edgePull);

        // tangent/radial in UV space at theta2
        float2 tanA  = float2(-sin(theta2), cos(theta2));
        float2 tanUV = float2(tanA.x / aspect, tanA.y);
        float2 dirUV = float2(dirA.x / aspect, dirA.y);

        // current sampling (CA/blur)
float2 caOff   = tanUV * ((caPx * spin) * texel);
float2 blurOff = tanUV * ((blurPx * spin) * texel);

        float2 uG = saturate(uvF);
        float2 uR = saturate(uvF + caOff);
        float2 uB = saturate(uvF - caOff);

        float3 c0;
        c0.r = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uR).r;
        c0.g = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uG).g;
        c0.b = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uB).b;

        float3 c1 = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, saturate(uG + blurOff)).rgb;
        float3 c2 = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, saturate(uG - blurOff)).rgb;

        float3 curCol = (c0 + c1 + c2) * (1.0 / 3.0);


        float3 col = curCol;

        // optional rim ring 
        float ringW   = lerp(0.0010, 0.0020, a);
        float ringInt = 1 * a;
        float ring = RingAA(uv, center, radius, _RimRing);
        col += ring * ringInt;

        float3 outRgb = lerp(src.rgb, col, inside);
        return half4(outRgb, src.a);
    }

    ENDHLSL

    SubShader
    {
                PackageRequirements
        {
            "com.unity.render-pipelines.universal": "0.0.0"
        }

        Name "#liquidglass#"
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