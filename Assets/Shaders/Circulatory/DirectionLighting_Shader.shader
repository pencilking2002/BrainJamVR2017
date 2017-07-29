Shader "Custom/DirectionLighting" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
//		_Glossiness ("Smoothness", Range(0,1)) = 0.5
//		_Metallic ("Metallic", Range(0,1)) = 0.0
//		_WorldPosition("WorldPos", Vector) =(1.0,0,0,1.0)
		_Direction("Direction", Vector) =(1.0,0,0,1.0)
		_StartValue ("Start Value", Float) =0.0
		_EndValue ("End Value", Float) =1.0
		_Smoothness ("Smoothness", Float) =10.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Lambert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
        float3 localPos;
        float3 worldPos;
		};

		fixed4 _Color;
		
		float4 _Direction;
		fixed _StartValue;
		fixed _EndValue;
		float _Smoothness;
		
//		static float3 _WorldPosition;

		void surf (Input IN, inout SurfaceOutput o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
//			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
//			o.Metallic = _Metallic;
//			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
//			_WorldPosition=IN.worldPos;
			
			float3 position=IN.worldPos;
			
			float3 direction=_Direction*_Smoothness;
			
			float directionStrengthX=position.x/direction.x;
			float directionStrengthY=position.y/direction.y;
			float directionStrengthZ=position.z/direction.z;
			float directionStrength=directionStrengthX+directionStrengthY+directionStrengthZ;
			directionStrength=saturate(directionStrength);
			
			directionStrength=IN.uv_MainTex.y;
			float directionValue = directionStrength*(_EndValue-_StartValue)+_StartValue;
//			float3 distanceColor = 4.5*(1.0-saturate(abs()));
			o.Emission = c*directionValue;
		}
		
		ENDCG
	} 
	FallBack "Diffuse"
}
