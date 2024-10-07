Shader "Custom/Posterize"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Levels("Color Levels", Range(2, 256)) = 16
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                sampler2D _MainTex;
                float _Levels;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                float4 frag(v2f i) : SV_Target
                {
                    float4 color = tex2D(_MainTex, i.uv);

                    // Posterization effect: limit colors to a certain level
                    color.rgb = floor(color.rgb * _Levels) / _Levels;

                    return color;
                }
                ENDCG
            }
        }
}
