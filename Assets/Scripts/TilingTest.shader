Shader "Custom/TileShader2"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _TileX("Tile X", Range(0, 10)) = 1
    }
        SubShader
        {
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
                float _TileX;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = float2(v.uv.x * _TileX, v.uv.y);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    float2 uv = i.uv;
                    uv.x = uv.x - floor(uv.x);
                    fixed4 col = tex2D(_MainTex, uv);
                    return col;
                }
                ENDCG
            }
        }
}