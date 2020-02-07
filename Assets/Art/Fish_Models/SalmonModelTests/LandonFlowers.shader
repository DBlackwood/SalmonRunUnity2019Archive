Shader "Landon/Flowers"{
 Properties{
     _Diff   ("Tint Mask (RGBA) Glow (A)", 2D)   = "white"{}
     [NoScaleOffset] _Norm   ("Normal (RGB) Specular Mask (A)", 2D) = "bump"{}
     _Color      ("Color1 (RGB) Gloss(A)", Color)                        = (.5, .2, .1, 1)
     _Color2     ("Color2 (RGB) Gloss(A)", Color)                        = (.5, .2, .1, 1)
     _Color3     ("Color3 (RGB) Gloss(A)", Color)                        = (.1, .5, .2, 1)
     _Color4     ("Color4 (RGB) Gloss(A)", Color)                        = (.2, .1, .5, 1)
     _SpecColor  ("Specular (RGB) Gloss Strength (A)", Color)            = (.15, .1, .1, .3)
 }

	SubShader {
		Tags { "RenderType"="Opaque" "Queue" = "Geometry" } //"RenderType"="Opaque" "Queue" = "Transparent"
		LOD 400
     Cull Back
     CGPROGRAM
     #pragma surface surf BlinnPhong
     #pragma target 3.0

     sampler2D   _Diff, _Norm;
     half4       _Color, _Color2, _Color3, _Color4;

     struct Input{
         half2 uv_Diff;
     };

     void surf(Input IN, inout SurfaceOutput o){

         half4 m = tex2D (_Diff, IN.uv_Diff);
         half4 n = tex2D (_Norm, IN.uv_Diff);
         half s = pow(n.a * ((_Color.a * m.r) + (_Color2.a * m.g) + (_Color3.a * m.b) + (_Color4.a * (1 - m.a))),2);
         
		 //half3 norm;
		 //norm.xy = n.wy * 2 - 1;
		 //norm.z = sqrt(1 - saturate(dot(norm.xy, norm.xy)));
		 //o.Normal = norm;

         //o.Normal = n;
		o.Normal = normalize(n.xyz * 2 - 1);  //dont use Normal for normal maps. Use Type Default.
		//o.Normal = n.xyz * 2 - 1;
		// o.Normal = UnpackNormal (n);
         o.Specular = clamp(s, .1, 1);
         o.Gloss = s * _SpecColor.a * 100;
         o.Albedo = _Color.rgb * m.r + _Color2.rgb * m.g + _Color3.rgb * m.b + _Color4.rgb * (1-m.a);
        // o.Emission = 7 * _Color4.rgb * (1 - m.a);
     }
     ENDCG
  }
 FallBack "Diffuse"
}