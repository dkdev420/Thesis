Shader "Custom/GlowShader"
{
	Properties
	{
	 _ColorTint("Color Tint", Color) = (1, 1, 1, 1)
	 _BumpMap("Normal Map", 2D) = "bump" {}
	 _RimColor("Rim Color", Color) = (1, 1, 1, 1)
	 _RimPower("Rim Power", Range(1.0, 6.0)) = 3.0

	}
		SubShader{

		 Tags { "RenderType" = "Opaque" }

		 CGPROGRAM
		 #pragma surface surf Lambert

		 struct Input {

		  float4 color : Color;
		  float2 uv_MainTex;
		  float2 uv_BumpMap;
		  float3 viewDir;

		 };

		 float4 _ColorTint;
		 sampler2D _BumpMap;
		 float4 _RimColor;
		 float _RimPower;

		 void surf(Input IN, inout SurfaceOutput o)
		 {


		  o.Albedo = _ColorTint;
		  half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
		  o.Emission = _RimColor.rgb * pow(rim, _RimPower);

		 }

		 ENDCG
	 }
		 FallBack "Diffuse"
}﻿