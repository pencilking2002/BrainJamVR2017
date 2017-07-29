// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Rim/Lit Global Fog" {
	Properties {
    	_Color ("Color", Color) = (1,1,1,1)
    	_EmissionIntensity ("Emission Intensity", Range(0.0,1.0)) =0.25
    	_RimColor ("Rim Color", Color) = (0.8,0.95,1.0,1.0)
    	_RimAngle ("Rim Angle", Range(0.0,4.0)) = 1.5
    	_RimIntensity ("Rim Intensity", Range(0.0,3.0)) = .5
		_Distance("Distance",  Range(0,1000)) = 200
		_ReflectCube ("Reflection Cubemap", Cube) = "_Skybox" { TexGen CubeReflect }
	}
	SubShader {
		Pass{
			Tags { "RenderType"="Geometry" "LightMode"="ForwardBase"}
			
		
			CGPROGRAM
			#pragma approxview noambient noshadow nolightmap nodynlightmap nodirlightmap nometa exclude_path:prepass
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"
			
			//uniform
			uniform fixed4 _Color;
			uniform sampler2D _MainTex;
			uniform fixed4 _MainTex_ST;
			uniform sampler2D _BumpTex;
			uniform fixed4 _BumpTex_ST;
			uniform fixed4 _EmissionColor;
			uniform fixed _EmissionIntensity;
			uniform fixed4 _RimColor;
			uniform half _RimAngle;
			uniform half _RimIntensity;
			uniform half _LightAngle;
			
			uniform samplerCUBE _ReflectCube;
		   	uniform  float _Distance;
	    
			float lengthFloatThree(float3 v)
			{
  				return sqrt(dot(v,v));
			}
		
			struct vertexInput {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tex : TEXCOORD0;
				float4 tangent : TANGENT;
			};
			
			struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 col : COLOR;
				float4 I : TEXCOORD0;
			};
			
			vertexOutput vert(vertexInput v){
				vertexOutput o;
				
				float3 viewDirection = _WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld,v.vertex).xyz;//not normalized
				o.I.a = lengthFloatThree(viewDirection);//camera distance stored in alpha
				o.I.rgb = -normalize(viewDirection).rgb;
				
				//Set position
				o.pos = UnityObjectToClipPos(v.vertex);
				
				//Calculate useful variables
				half3 normalDirection = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject).xyz);
				half3 lightDirection= normalize(_WorldSpaceLightPos0.xyz);
				
				//Calculate diffuse color
				half3 diffuseColor=max(0.0,0.6+dot(normalDirection,lightDirection));
		
				//Calculate emission color
				half3 emissionColor=_Color*_EmissionIntensity;
				
				//Calculate rim color
    			half3 rimAngleColor = 1.0 - saturate(dot (normalize(viewDirection), normalDirection));
				rimAngleColor=pow(rimAngleColor,_RimAngle);
				half3 rimColor=_RimColor*_RimIntensity*rimAngleColor*_RimColor.xyzw;
				
				//Calculate final color
				o.col =float4(emissionColor+saturate(diffuseColor*rimColor),1.0);
				return o;
				
				return o;
			}
			
			float4 frag(vertexOutput i) : COLOR
			{	
				fixed4 reflcol = texCUBE( _ReflectCube, i.I );
				float objectDistance = abs(i.I.a);
				objectDistance = pow(objectDistance,2.0);
				half fogStrength = clamp(objectDistance/(_Distance*800.0),0,1);
				
				float3 colorFinal = lerp(i.col,reflcol,fogStrength);
				
				return float4(colorFinal,1.0);
			}
				
			
			ENDCG
		} 
	}	
}