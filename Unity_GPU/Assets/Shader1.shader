   Shader "Instanced/InstancedShader" {
    Properties {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _ScaleX ("Scale_X", Float) = 0.43
        _ScaleY ("Scale_Y", Float) = 0.625
    }
    SubShader {

        Pass {
            Blend SrcAlpha OneMinusSrcAlpha
            Tags {"Queue" = "Transparent"}

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5

            struct Bunny {
                float4 pos;
                float2 vec;
                float pad0;
                float pad1;
            };

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _ScaleX;
            float _ScaleY;

        #if SHADER_TARGET >= 45
            StructuredBuffer<Bunny> bunnies;
        #endif

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv_MainTex : TEXCOORD0;
            };

            void rotate2D(inout float2 v, float r)
            {
                float s, c;
                sincos(r, s, c);
                v = float2(v.x * c - v.y * s, v.x * s + v.y * c);
            }

            v2f vert (appdata_base v, uint instanceID : SV_InstanceID)
            {
            #if SHADER_TARGET >= 45
                Bunny data = bunnies[instanceID];
            #else
                Bunny data = 0;
            #endif
                v.vertex.xy *= float2(_ScaleX, _ScaleY);
                rotate2D(v.vertex.xy, data.pos.w);
                float3 worldPosition = v.vertex.xyz + data.pos.xyz;
                v2f o;
                o.pos = mul(UNITY_MATRIX_VP, float4(worldPosition, 1.0f));
                o.uv_MainTex = v.texcoord;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 colour = tex2D(_MainTex, i.uv_MainTex);
                return colour;
            }

            ENDCG
        }
    }
}