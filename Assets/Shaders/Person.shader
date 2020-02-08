Shader "Landon/Person" {

Properties {
      _MainTex ("Texture (RGB) Skin (A)", 2D) = "white" {}
	  _Mult ("Skin", Range(0, 1)) = 0
    }
    SubShader {
      Tags { "RenderType" = "Opaque" }
      CGPROGRAM
      #pragma surface surf Lambert
      struct Input {
          float2 uv_MainTex;
      };
      sampler2D _MainTex;
	  fixed _Mult;
      void surf (Input IN, inout SurfaceOutput o) {
		  fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
		  fixed3 color = fixed3(0.18,0.12,0.08);
          o.Albedo = tex.rgb * lerp(clamp((color + tex.a),0,1),1,_Mult);
      }
      ENDCG
    } 
    Fallback "Diffuse"
  }