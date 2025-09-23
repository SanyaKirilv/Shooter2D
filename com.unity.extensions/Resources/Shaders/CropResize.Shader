Shader "Custom/CropResize" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _CropRect ("Crop Rect", Vector) = (0, 0, 1, 1)
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _CropRect;

            v2f vert (appdata_t v) {
                v2f o;
                o.uv = lerp(_CropRect.xy, _CropRect.zw, v.uv);
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}
