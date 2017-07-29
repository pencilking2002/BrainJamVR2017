// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Unlit shader. Simplest possible textured shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Custom/Circulatory/Fog Quad" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_FadeDistance ("Fade Distance", Range(0,1000)) = 10
}

SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	
	ZWrite Off
	Blend SrcAlpha OneMinusSrcAlpha 
	
	Pass {  
		CGPROGRAM
			#pragma alpha:blend
			
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				fixed vertexDistance : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;
			half _FadeDistance;
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.vertexDistance = length(_WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld,v.vertex).xyz);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 finalColor;
				
				fixed4 tex = tex2D(_MainTex, i.texcoord);
				fixed4 texScrolled = tex2D(_MainTex, i.texcoord + _MainTex_ST.zw*_Time.y);
				
//				i.col.r = distance(_WorldSpaceCameraPos, i.vertex);
				
				half distanceMultiplier = (i.vertexDistance-20)/(_FadeDistance);
				
				finalColor.a = tex.r*_Color.a*texScrolled.g*distanceMultiplier;
				finalColor.rgb = _Color.rgb;
				
				return finalColor;
			}
		ENDCG
	}
}

}
