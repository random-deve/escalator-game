Shader "Custom/RGBGlitchShader"
{
    Properties
    {
        _MainTex("Base (RGB)", 2D) = "white" {}
        _Amount("Glitch Amount", Float) = 0.0
        _TimeOffset("Time Offset", Float) = 0.0
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 100

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                struct appdata_t
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
                float _Amount;
                float _TimeOffset;

                v2f vert(appdata_t v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                float4 frag(v2f i) : SV_Target
                {
                    float time = _TimeOffset + _Time.y * 20.0;
                    // float2 offset = float2(_Amount * sin(i.uv.y * 10.0 + time), _Amount * cos(i.uv.x * 10.0 + time));
                    float2 offset = float2(_Amount * sin(i.uv.y * 10.0 + time), 0);
                    float4 color;
                    color.r = tex2D(_MainTex, i.uv + offset).r;
                    color.g = tex2D(_MainTex, i.uv).g;
                    color.b = tex2D(_MainTex, i.uv - offset).b;
                    color.a = 1.0;
                    return color;
                }
                ENDCG
            }
        }
}
