Shader "InkCloud/Watercolor Halftone URP"
{
    Properties
    {
        _PaperColor ("Paper", Color) = (0.985,0.987,0.988,1)
        _InkColor ("Ink", Color) = (0.40,0.48,0.55,1)
        _Density ("Density", Range(0,2)) = 1
        _Speed ("Flow Speed", Range(0,2)) = 0.6
        _Travel ("Cloud Travel", Range(0,1.5)) = 1
        _WhiteSpace ("White Space", Range(0.15,0.45)) = 0.28
        _Coverage ("Coverage", Range(0.5,1.5)) = 1
        _DotCount ("Dots Across Width", Range(80,600)) = 340
        _DotStrength ("Halftone", Range(0,1)) = 0.85
        _Detail ("Watercolor Variation", Range(0,1)) = 0.65
        _Seed ("Seed", Float) = 7
        _Aspect ("Aspect", Float) = 1.77778
        _CloudTime ("Time", Float) = 0
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent-100" "RenderType"="Opaque" }
        Pass
        {
            Name "InkCloud"
            Tags { "LightMode"="SRPDefaultUnlit" }
            Cull Off ZWrite Off ZTest LEqual
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma target 3.0
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            struct Attributes { float4 positionOS: POSITION; float2 uv: TEXCOORD0; };
            struct Varyings { float4 positionCS: SV_POSITION; float2 uv: TEXCOORD0; };
            CBUFFER_START(UnityPerMaterial)
            float4 _PaperColor, _InkColor;
            float _Density, _Speed, _Coverage, _DotCount, _DotStrength;
            float _Detail, _Seed, _Aspect, _CloudTime;
            float _Travel, _WhiteSpace;
            CBUFFER_END
            Varyings Vert(Attributes v)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                return o;
            }
            float Hash(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }
            float Noise(float2 p)
            {
                float2 i = floor(p), f = frac(p);
                float2 u = f*f*(3.0-2.0*f);
                return lerp(lerp(Hash(i), Hash(i+float2(1,0)), u.x),
                    lerp(Hash(i+float2(0,1)),Hash(i+1),u.x),u.y);
            }
            float Fbm(float2 p)
            {
                float n = 0, a = 0.5;
                [unroll] for (int i=0; i<5; i++)
                {
                    n += a * Noise(p);
                    p = mul(float2x2(0.8,-0.6,0.6,0.8),p)*2.03 + 13.7;
                    a *= 0.5;
                }
                return n;
            }
            half4 Frag(Varyings i): SV_Target
            {
                float2 uv = i.uv;
                float2 p = uv * float2(_Aspect,1) * 4.2 + _Seed*2.13;
                float t = _CloudTime * _Speed * 0.5;
                float2 drift = float2(t*0.47, -t*0.31);
                float2 warp = float2(Fbm(p*0.65+drift),Fbm(p*0.65-drift+19.4))-0.5;
                float n = Fbm(p + warp*2.4 + drift);
                float fine = Fbm(p*3.7-warp+float2(-t*0.21,t*0.17));
                float2 q = uv + warp * float2(0.22,0.25);
                float c = max(_Coverage,0.01);
                float phase = t*0.72;
                float2 centerA = float2(0.34,0.63) + _Travel*float2(0.30*sin(phase),0.30*sin(phase*0.79+0.6));
                float2 centerB = float2(0.65,0.35) + _Travel*float2(0.29*sin(phase*0.83+2.8),0.28*cos(phase*0.67+1.9));
                float angle = t*0.41;
                float2 da = q-centerA;
                da = mul(float2x2(cos(angle),-sin(angle),sin(angle),cos(angle)),da);
                float left = 1.0-length(da/(float2(0.30,0.44)*c*(1+0.12*sin(t*1.13))));
                float right = 1.0-length((q-centerB)/(float2(0.35,0.24)*c*(1+0.14*cos(t*0.93))));
                float field = max(left,right*0.85) + (n-0.5)*0.78 + (fine-0.5)*0.16;
                // Fixed screen-space protection: noise and motion cannot enter the rhythm lanes.
                float clearMask = max(smoothstep(0.70,0.82,uv.x),
                    smoothstep(_WhiteSpace,_WhiteSpace+0.12,uv.y));
                float coverage = smoothstep(-0.13,0.25,field);
                coverage *= clearMask;
                float interior = smoothstep(0.08,0.38,field);
                float2 grid = uv * float2(_DotCount,_DotCount/max(_Aspect,0.1));
                float2 cell = frac(grid)-0.5;
                float radius = sqrt(coverage)*0.69;
                float aa = max(fwidth(length(cell)),0.015);
                float dots = 1.0-smoothstep(radius-aa,radius+aa,length(cell));
                // A stationary screen lattice dissolves the moving contour into ink dots.
                float edge = lerp(coverage, dots*coverage, _DotStrength);
                float mask = lerp(edge,coverage,interior);
                float wash = lerp(0.68, 0.35+n*0.62+fine*0.30, _Detail);
                float ink = saturate(mask*wash*_Density);
                float paper = (Hash(floor(uv*float2(1920,1080)))-0.5)*0.008;
                float3 color = lerp(_PaperColor.rgb, _InkColor.rgb, ink) + paper;
                return half4(color,1);
            }
            ENDHLSL
        }
    }
}
