Shader "Instanced/InstancedShader" {
  Properties {
    _MainTex ("Albedo (RGB)", 2D) = "white" { }
  }
  SubShader {
    Pass {
      Blend SrcAlpha OneMinusSrcAlpha
      Tags { "Queue" = "Transparent" }

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

      sampler2D _MainTex;

      #if SHADER_TARGET >= 45
        StructuredBuffer<Bunny> bunnies;
      #endif

      struct appdata {
        float4 vertex : POSITION;
        float2 texcoord : TEXCOORD0;
        uint instanceID : SV_InstanceID;
      };

      struct v2f {
        float4 pos : SV_POSITION;
        float2 uv : TEXCOORD0;
      };

      void rotate2D(inout float2 v, float r) {
        float s, c;
        sincos(r, s, c);
        v = float2(v.x * c - v.y * s, v.x * s + v.y * c);
      }

      v2f vert(appdata v) {
        #if SHADER_TARGET >= 45
          Bunny data = bunnies[v.instanceID];
        #else
          Bunny data = 0;
        #endif
        rotate2D(v.vertex.xy, data.pos.w);
        float2 worldPosition = v.vertex.xy + data.pos.xy;
        v2f o;
        o.pos = UnityObjectToClipPos(float4(worldPosition, 0, 1));
        o.uv = v.texcoord;
        return o;
      }

      float4 frag(v2f i) : SV_Target {
        float4 colour = tex2D(_MainTex, i.uv);
        return colour;
      }

      ENDCG
    }
  }
}