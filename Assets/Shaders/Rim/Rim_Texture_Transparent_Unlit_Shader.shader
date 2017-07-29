﻿// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Rim/Texture_Transparent_Unlit" {
	Properties {
		_MainTex ("Main Texture", 2d) = "white" {}
    	_EmissionColor ("Emission Color", Color) = (1,1,1,1)
    	_EmissionIntensity ("Emission Intensity", Range(0.0,1.0)) =0.05
    	_RimColor ("Rim Color", Color) = (0.8,0.95,1.0,1.0)
    	_RimAngle ("Rim Angle", Range(0.0,3.0)) = 0.1
    	_RimIntensity ("Rim Intensity", Range(0.0,3.0)) = 1.5
	}
	SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	
	ZWrite Off
	Blend SrcAlpha OneMinusSrcAlpha 
			
	Pass {  
		CGPROGRAM
			#pragma noambient noforwardadd nofog
			#pragma alpha:blend
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"
			
			//uniform
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform fixed4 _EmissionColor;
			uniform fixed _EmissionIntensity;
			uniform fixed4 _RimColor;
			uniform half _RimAngle;
			uniform half _RimIntensity;
			
			struct vertexInput {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tex : TEXCOORD0;
			};
			struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 col : COLOR;
				float4 texcoord : TEXCOORD0;
				UNITY_FOG_COORDS(1)
			};
			
			vertexOutput vert(vertexInput v){
				vertexOutput o;
				//Set position
				o.pos = UnityObjectToClipPos(v.vertex);
				o.texcoord=v.tex;
				UNITY_TRANSFER_FOG(o,o.pos);
				
				//Calculate useful variables
				half3 normalDirection = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject).xyz);
				half3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld,v.vertex).xyz);
		
				//Calculate emission color
				half4 emissionColor=_EmissionColor*_EmissionIntensity;
				
				//Calculate rim color
    			half4 rimAngleColor = 1.0 - saturate(dot (normalize(viewDirection), normalDirection));
				rimAngleColor=pow(rimAngleColor,_RimAngle);
				half4 rimColor=_RimColor*_RimIntensity*rimAngleColor*_RimColor.xyzw;
				
				//Calculate final color
				o.col = emissionColor+saturate(rimColor);
				return o;
			}
			
			float4 frag(vertexOutput o) : COLOR
			{	
				half4 mainTex = tex2D(_MainTex,_MainTex_ST.xy*o.texcoord.xy + _MainTex_ST.zw);
//				mainTex.a*=o.col.r;
				
				o.col*=mainTex;
				
				UNITY_APPLY_FOG(o.fogCoord, o.col);
				
				return o.col;
			}
			ENDCG
		}
	}
}