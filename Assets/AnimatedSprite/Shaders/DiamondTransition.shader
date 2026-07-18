Shader "Custom/DiamondTransition"
{
    Properties
    {
        [Header(Background Settings)]
        _MainColor("BG Color (Center)", Color) = (0.1, 0.1, 0.1, 1)
        _MainGradientColor("BG Color (Edges)", Color) = (0.2, 0.2, 0.2, 1)

        [Header(Diamond Settings)]
        _SecondaryColor("Diamond Color (Center)", Color) = (1, 0.8, 0.2, 1)
        _GradientColor("Diamond Color (Edges)", Color) = (1, 0.2, 0.5, 1)

        [Header(Pattern Controls)]
        _Scale("Scale", Float) = 10
        _ScrollSpeed("Scroll Speed (XY)", Vector) = (0.5, 0.2, 0, 0)
        _Progress("Transition Progress", Range(0, 1.5)) = 0.5
        _Smoothness("Edge Smoothness", Range(0.01, 0.2)) = 0.02
        _Spread("Center Spread Intensity", Range(0, 2)) = 1.0
    }
        SubShader
        {
            Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
            LOD 100

            Pass
            {
                CGPROGRAM
            #define AIO_URP 1
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                float4 _MainColor;
                float4 _MainGradientColor;
                float4 _SecondaryColor;
                float4 _GradientColor;
                float _Scale;
                float4 _ScrollSpeed;
                float _Progress;
                float _Smoothness;
                float _Spread;

                v2f vert(appdata v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target {
                    // 1. Distance from Y-Center (0 at middle, 1 at top/bottom)
                    float distFromCenter = abs(i.uv.y - 0.5) * 2.0;

                // 2. Determine Background and Diamond colors based on that distance
                float4 bgColor = lerp(_MainColor, _MainGradientColor, distFromCenter);
                float4 diamondColor = lerp(_SecondaryColor, _GradientColor, 0.5-distFromCenter);

                // 3. Tiling and Scrolling
                float2 scrolledUV = i.uv + (_Time.y * _ScrollSpeed.xy);
                float2 fraction = frac(scrolledUV * _Scale);

                // 4. Diamond Shape Math
                float2 centeredUV = abs(fraction - 0.5);
                float diamondDist = centeredUV.x + centeredUV.y;

                // 5. Combine Diamond Shape with Y-Distance for the "Radiating" effect
                float transitionLogic = diamondDist + (distFromCenter * _Spread);
                float mask = smoothstep(_Progress + _Smoothness, _Progress, transitionLogic);

                // 6. Final Blend: Mix the dynamic BG color with the dynamic Diamond color
                return lerp(bgColor, diamondColor, mask);
            }
            ENDCG
        }
        }
}