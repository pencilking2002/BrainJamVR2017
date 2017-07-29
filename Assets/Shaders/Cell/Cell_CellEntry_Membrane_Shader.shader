﻿// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Cell/Cell Entry/Membrane" {
	Properties {
		_MainTex ("Emission", 2D) = "white" {}
		_MainTex2 ("Emission", 2D) = "white" {}
		_MainTex3 ("Emission", 2D) = "white" {}
		_BumpTex ("Normal", 2D) = "white" {}
		_BumpTex2 ("Normal 2", 2D) = "white" {}
    	_EmissionColor ("Emission Color", Color) = (1,1,1,1)
    	_EmissionIntensity ("Emission Intensity", Range(0.0,1.0)) =1.0
    	_RimColor ("Rim Color", Color) = (0.8,0.95,1.0,1.0)
    	_RimAngle ("Rim Angle", Range(0.0,12.0)) = 1.5
    	_RimIntensity ("Rim Intensity", Range(0.0,3.0)) = 1.5
//    	_PortalPercent ("Portal Percent", Range(0.0,1.0)) = 0.5
//    	_PortalX ("Portal X", Range(0.0,1.0)) = 0.5
//    	_PortalY ("Portal Y", Range(0.0,1.0)) = 0.5
	}
	SubShader
	{
	
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "LightMode"="ForwardBase"}
		
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha 
	
		Pass{
			
		
			CGPROGRAM
			#pragma approxview noambient noshadow nolightmap nodynlightmap nodirlightmap nometa exclude_path:prepass
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			//uniform
			uniform sampler2D _MainTex;
			uniform fixed4 _MainTex_ST;
			uniform sampler2D _MainTex2;
			uniform fixed4 _MainTex2_ST;
			uniform sampler2D _MainTex3;
			uniform fixed4 _MainTex3_ST;
			uniform sampler2D _BumpTex;
			uniform fixed4 _BumpTex_ST;
			uniform sampler2D _BumpTex2;
			uniform fixed4 _BumpTex2_ST;
			uniform fixed4 _EmissionColor;
			uniform fixed _EmissionIntensity;
			uniform fixed4 _RimColor;
			uniform half _RimAngle;
			uniform half _RimIntensity;
			uniform half _PortalPercent;
			uniform half _PortalX;
			uniform half _PortalY;
		
			struct vertexInput {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tex : TEXCOORD0;
				float4 tangent : TANGENT;
			};
			
			struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 col : COLOR;
				float4 texcoord : TEXCOORD0;
				float3 tangentSpaceLightDir : TEXCOORD1;
			};
			
			vertexOutput vert(vertexInput v){
				vertexOutput o;
				
				//Calculate useful variables
				half3 normalDirection = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject).xyz);
				half3 lightDirection= normalize(_WorldSpaceLightPos0.xyz);
				float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
                float3 binormal = cross( normalize(v.normal), normalize(v.tangent.xyz) );
                float3x3 rotation = float3x3( v.tangent.xyz, binormal, v.normal );
               
				//Calculate diffuse color
				half3 diffuseColor=saturate(dot(normalDirection,lightDirection));
				
               	//Send variables to frag
                o.tangentSpaceLightDir = mul(rotation, viewDir);
				o.pos = UnityObjectToClipPos(v.vertex);
				o.texcoord=v.tex;
				o.col=float4(diffuseColor,1.0);
				
				return o;
			}
			
			float4 frag(vertexOutput i) : COLOR
			{	
				half3 viewDirection=i.tangentSpaceLightDir;
				
				half atten=1.0;
			
				//textures
				float2 coords = _MainTex_ST.xy*i.texcoord.xy + _MainTex_ST.zw;
				fixed4 tex1 = tex2D(_MainTex,coords);
				coords = _MainTex2_ST.xy*i.texcoord.xy + _MainTex2_ST.zw;
				fixed4 tex2 = tex2D(_MainTex2,coords);
				coords = _MainTex3_ST.xy*i.texcoord.xy + _MainTex3_ST.zw;
				fixed4 tex3 = tex2D(_MainTex3,coords);
				
				fixed4 tex = (tex1+tex2+tex3)*.333333;
				
				coords = _BumpTex_ST.xy*i.texcoord.xy + _BumpTex_ST.zw;
				fixed4 texN1 = tex2D(_BumpTex,coords);
				coords = _BumpTex2_ST.xy*i.texcoord.xy + _BumpTex2_ST.zw;
				fixed4 texN2 = tex2D(_BumpTex2,coords);
				
				fixed4 texN = (texN1+texN2)*.5;
				
             	half3 normalDirection = (texN.rgb * 2.0) - 1.0;
				
				//rim
				half rim = 1 - saturate(dot(normalize(viewDirection),normalDirection));
				half3 rimLighting= _RimColor.rgb * pow(rim,_RimAngle);
				rimLighting*=_RimIntensity;
				
				
				//final
				half3 texFinal=tex.rgb;
				half3 rimFinal = rimLighting;
				half3 emissionFinal = _EmissionColor*_EmissionIntensity;
				half3 lightFinal=i.col*emissionFinal;
				
				fixed3 colorFinal=texFinal*lightFinal+rimFinal*i.col;
				
				float xDistance =  abs(i.texcoord.x - (_PortalX));
				float yDistance =  abs(i.texcoord.y - (_PortalY));
				
//				fixed alpha = 1;
//				if((xDistance+yDistance)<.01)
//					alpha = 0;
//				alpha = sqrt(xDistance*xDistance+yDistance*yDistance*.25)*150;
//				alpha = clamp(alpha,0,1);
//				alpha += (1.0-_PortalPercent);
//				alpha = pow(alpha,8);
				
				return float4(colorFinal,tex1.a);
			}
				
			
			ENDCG
		} 
	}	
}