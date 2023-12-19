Shader"Custom/TileXShader" {
    Properties {
        [NoScaleOffset]_MainTex ("Texture", 2D) = "white" {}
        _TileX ("Tile X", Range(0.1, 10)) = 1
    }
    SubShader {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Pass {
Cull Off

ZWrite Off

ZTest Always

Blend SrcAlpha
OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
#include "UnityCG.cginc"

struct appdata_t
{
    float4 vertex : POSITION;
    float2 texcoord : TEXCOORD0;
    fixed4 color : COLOR;
};

struct v2f
{
    float2 uv : TEXCOORD0;
    fixed4 color : COLOR;
    float4 vertex : SV_POSITION;
};

sampler2D _MainTex;
float _TileX;

v2f vert(appdata_t v)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
                
                // _TileX로 UV 좌표를 나누어 사용
    o.uv = v.texcoord * _TileX;
                
    o.color = v.color;
    return o;
}

fixed4 frag(v2f i) : SV_Target
{
                // UV 좌표를 사용하여 텍스처를 샘플링
    fixed4 col = tex2D(_MainTex, i.uv) * i.color;
    return col;
}
            ENDCG
        }
    }
}