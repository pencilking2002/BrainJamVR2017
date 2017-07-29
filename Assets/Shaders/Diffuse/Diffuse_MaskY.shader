Shader "Custom/Diffuse/MaskY" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_EmissionColor ("Emission Color", Color) = (0,0,0,1)
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_MaskY ("Mask Y",Range(0,1.0)) = 1.0
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
}
SubShader {
	Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
	LOD 200

	CGPROGRAM
	#pragma surface surf Lambert alphatest:_Cutoff

	sampler2D _MainTex;
	fixed4 _Color;
	fixed4 _EmissionColor;
	fixed _MaskY;

	struct Input {
		float2 uv_MainTex;
//		float2 uv2_MainTex2;
		float3 worldPos;
	};

	void surf (Input IN, inout SurfaceOutput o) {

		fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);

		o.Albedo = tex*_Color;
		o.Alpha = -sign((IN.uv_MainTex.y)-_MaskY);
		o.Emission = tex*_EmissionColor;
	}
	ENDCG
	}


Fallback "Legacy Shaders/Transparent/Cutout/VertexLit"
}
