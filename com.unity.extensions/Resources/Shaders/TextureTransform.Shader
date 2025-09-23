Shader "Custom/TextureTransform" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _FlipHorizontal ("Flip Horizontal", Float) = 0
        _FlipVertical ("Flip Vertical", Float) = 0
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
            float4x4 _RotationMatrix;
            float _FlipHorizontal;
            float _FlipVertical;

            v2f vert (appdata_t v) {
                v2f o;

                float2 uv = v.uv;

                if (_FlipHorizontal > 0.5) uv.x = 1.0 - uv.x;
                if (_FlipVertical > 0.5) uv.y = 1.0 - uv.y;

                uv -= 0.5;
                uv = mul((float2x2)_RotationMatrix, uv);
                uv += 0.5;

                o.uv = uv;
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
