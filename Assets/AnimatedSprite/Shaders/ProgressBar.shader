Shader "Custom/Sprites/ProgressBar"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Background Color", Color) = (0.1, 0.1, 0.1, 0.8)
        _ThumbColor("Thumb Color", Color) = (1, 1, 1, 1)
        _BorderColor("Border Color", Color) = (0, 0, 0, 1)

        _BorderThickness("Border Thickness", Vector) = (0.02, 0.1, 0, 0)
        _ThumbWidth("Thumb Width", Range(0, 1)) = 0.2
        _ThumbCenter("Thumb Center", Range(0, 1)) = 0.5
    }

        SubShader
        {
            Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off ZWrite Off

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
                    float4 color : COLOR;
                };

                struct v2f {
                    float4 vertex : SV_POSITION;
                    float2 uv : TEXCOORD0;
                    fixed4 color : COLOR;
                };

                fixed4 _Color;
                fixed4 _ThumbColor;
                fixed4 _BorderColor;
                float2 _BorderThickness;
                float _ThumbWidth;
                float _ThumbCenter;

                v2f vert(appdata v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    o.color = v.color;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target {
                    float2 uv = i.uv;

             
                    float border = step(uv.x, _BorderThickness.x) + step(1.0 - uv.x, _BorderThickness.x) +
                                   step(uv.y, _BorderThickness.y) + step(1.0 - uv.y, _BorderThickness.y);
                    border = saturate(border);

                    float dist = frac(uv.x - _ThumbCenter + 0.5) - 0.5;
                    float thumbMask = step(abs(dist), _ThumbWidth * 0.5);

          
                    fixed4 col = lerp(_Color, _ThumbColor, thumbMask);
                    col = lerp(col, _BorderColor, border);

                    return col * i.color;
                }
                ENDCG
            }
        }
}