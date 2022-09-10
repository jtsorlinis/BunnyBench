   Shader "Instanced/InstancedShader" {
    Properties {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
    }
    SubShader {

        Pass {
            Blend SrcAlpha OneMinusSrcAlpha
            Tags {"Queue" = "Transparent"}

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5

            #include "UnityCG.cginc"

            sampler2D _MainTex;

        #if SHADER_TARGET >= 45
            StructuredBuffer<float4> positionBuffer;
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
                v -= 0.5;
                v = float2(v.x * c - v.y * s, v.x * s + v.y * c);
                v += 0.5;
            }

            v2f vert (appdata_base v, uint instanceID : SV_InstanceID)
            {
            #if SHADER_TARGET >= 45
                float4 data = positionBuffer[instanceID];
            #else
                float4 data = 0;
            #endif

                rotate2D(v.vertex.xy, data.w);
                float3 worldPosition = v.vertex.xyz + data.xyz;
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