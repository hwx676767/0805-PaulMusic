// Boss 半透明 glitch：腿部青雾+数据块遮罩，身体偶发切片/扫描线/浮动 UI 框。
Shader "DeusEx/BossGlitch"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1, 1, 1, 1)

        [Header(Leg Mist)]
        _MistColor ("Mist Color", Color) = (0.45, 0.72, 0.78, 0.85)
        _MistColorDeep ("Mist Deep", Color) = (0.22, 0.42, 0.48, 0.95)
        _LegTop ("Leg Zone Top (UV.y)", Range(0, 1)) = 0.42
        _LegSoft ("Leg Soft Edge", Range(0.01, 0.4)) = 0.14
        _MistStrength ("Mist Strength", Range(0, 2)) = 1.15
        _MistScale ("Mist Scale", Float) = 6.5
        _MistSpeed ("Mist Speed", Float) = 0.35
        _HideFeet ("Hide Feet Amount", Range(0, 1)) = 0.85
        _FeetCutoff ("Feet Cutoff UV.y", Range(0, 0.5)) = 0.12

        [Header(Data Blocks)]
        _DataColor ("Data Tint", Color) = (0.55, 0.82, 0.88, 0.55)
        _DataDensity ("Data Density", Range(1, 40)) = 14
        _DataStrength ("Data Strength", Range(0, 2)) = 0.15

        [Header(Body Random Glitch)]
        _BodyGlitch ("Body Glitch Amount", Range(0, 1)) = 0.12
        _SliceStrength ("Slice Offset", Range(0, 0.08)) = 0.008
        _ScanColor ("Scan Line Color", Color) = (0.95, 0.55, 0.28, 0.85)
        _ScanChance ("Scan Chance", Range(0, 1)) = 0.12
        _RectChance ("UI Rect Chance", Range(0, 1)) = 0.05

        [Header(Chromatic)]
        _Fringe ("Color Fringe", Range(0, 0.02)) = 0.006
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "IgnoreProjector" = "True"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "BossGlitch"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float4 _MistColor;
                float4 _MistColorDeep;
                float _LegTop;
                float _LegSoft;
                float _MistStrength;
                float _MistScale;
                float _MistSpeed;
                float _HideFeet;
                float _FeetCutoff;
                float4 _DataColor;
                float _DataDensity;
                float _DataStrength;
                float _BodyGlitch;
                float _SliceStrength;
                float4 _ScanColor;
                float _ScanChance;
                float _RectChance;
                float _Fringe;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            float hash21(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            float vnoise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                float a = hash21(i);
                float b = hash21(i + float2(1, 0));
                float c = hash21(i + float2(0, 1));
                float d = hash21(i + float2(1, 1));
                float2 u = f * f * (3.0 - 2.0 * f);
                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
            }

            float fbm(float2 p)
            {
                float v = 0.0;
                float a = 0.5;
                for (int i = 0; i < 4; i++)
                {
                    v += a * vnoise(p);
                    p = p * 2.03 + float2(17.1, 9.7);
                    a *= 0.5;
                }
                return v;
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.color = IN.color * _Color;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;
                float t = _Time.y;

                // 水平切片位移（身体偶发）
                float band = floor(uv.y * 48.0);
                float bandPulse = step(0.72, hash21(float2(band, floor(t * 7.0))));
                float slice = (hash21(float2(band, 3.1)) - 0.5) * 2.0 * _SliceStrength * bandPulse * _BodyGlitch;
                float2 guv = uv + float2(slice, 0);

                // 轻微色差采样
                float fringe = _Fringe * (0.4 + bandPulse);
                half4 cR = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, guv + float2(fringe, 0));
                half4 cG = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, guv);
                half4 cB = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, guv - float2(fringe, 0));
                half4 baseCol = half4(cR.r, cG.g, cB.b, cG.a) * IN.color;

                if (baseCol.a < 0.01) return half4(0, 0, 0, 0);

                // 腿部权重：越靠下越强
                float leg = 1.0 - smoothstep(_LegTop - _LegSoft, _LegTop + _LegSoft * 0.25, uv.y);
                leg *= saturate(baseCol.a * 1.2);

                // 青雾
                float2 mistUv = uv * _MistScale + float2(t * _MistSpeed * 0.4, t * _MistSpeed);
                float mist = fbm(mistUv);
                mist = smoothstep(0.28, 0.78, mist);
                float2 mistUv2 = uv * (_MistScale * 1.7) - float2(t * 0.15, t * 0.22);
                mist = saturate(mist * 0.7 + fbm(mistUv2) * 0.55);

                half3 mistRgb = lerp(_MistColorDeep.rgb, _MistColor.rgb, mist);
                float mistA = mist * _MistStrength * leg * _MistColor.a;

                // 底部脚直接“溶进雾里”
                float feet = 1.0 - smoothstep(_FeetCutoff, _FeetCutoff + 0.1, uv.y);
                float hide = feet * _HideFeet * leg;
                baseCol.a *= 1.0 - hide * 0.92;
                mistA = max(mistA, hide * 0.95);

                // 数据块 / 技术 UI 矩形
                float2 cell = floor(uv * float2(_DataDensity * 0.7, _DataDensity));
                float cellId = hash21(cell + floor(t * 2.5));
                float showCell = step(0.62, cellId) * leg;
                float2 fcell = frac(uv * float2(_DataDensity * 0.7, _DataDensity));
                float grid = max(step(0.92, fcell.x), step(0.92, fcell.y));
                float inner = step(0.08, fcell.x) * step(0.08, fcell.y) * step(fcell.x, 0.92) * step(fcell.y, 0.92);
                // 细横线“代码”
                float code = step(0.55, hash21(float2(cell.x, floor(fcell.y * 8.0)))) * inner * 0.35;
                float dataMask = (grid * 0.65 + code + showCell * inner * 0.25) * _DataStrength;
                half3 dataRgb = _DataColor.rgb;
                float dataA = dataMask * _DataColor.a * leg;

                // 身体随机半透明 UI 框
                float rectPulse = step(1.0 - _RectChance, hash21(float2(floor(t * 1.8), 9.2)));
                float2 ruv = frac(uv * float2(3.2, 5.0) + float2(hash21(float2(floor(t * 1.8), 1)), 0));
                float rectBorder = max(step(0.96, ruv.x), max(step(0.96, ruv.y), max(step(ruv.x, 0.04), step(ruv.y, 0.04))));
                float bodyRect = rectBorder * rectPulse * _BodyGlitch * (1.0 - leg * 0.35) * baseCol.a;

                // 橙色 / 白色竖扫描线
                float scanPulse = step(1.0 - _ScanChance, hash21(float2(floor(t * 3.3), 4.4)));
                float sx = frac(uv.x * 11.0 + hash21(float2(floor(t * 3.3), 2.2)));
                float scan = smoothstep(0.0, 0.01, sx) * smoothstep(0.04, 0.015, sx);
                float scanA = scan * scanPulse * _BodyGlitch * baseCol.a * _ScanColor.a;

                // 合成
                half3 rgb = baseCol.rgb;
                rgb = lerp(rgb, mistRgb, saturate(mistA));
                rgb = lerp(rgb, dataRgb, saturate(dataA));
                rgb = lerp(rgb, _DataColor.rgb * 0.85, saturate(bodyRect * 0.55));
                rgb = lerp(rgb, _ScanColor.rgb, saturate(scanA));

                float a = saturate(baseCol.a + mistA * 0.35);
                a = max(a, bodyRect * 0.4);
                a = max(a, scanA);

                return half4(rgb, a);
            }
            ENDHLSL
        }
    }
    FallBack "Sprites/Default"
}
